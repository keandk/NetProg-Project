using DNS.Protocol;
using Ae.Dns.Client;
using Ae.Dns.Protocol;
using Ae.Dns.Protocol.Enums;
using System.Net;
using System.Runtime.Caching;

namespace DNS_Simulation
{
    public partial class Client : Form
    {
        private DnsUdpClient dnsClient;
        private DnsCachingClient cachingClient;
        private bool dnsClientInitialized = false;
        private IPEndPoint loadBalancerEndpoint;

        public Client()
        {
            InitializeComponent();
            type.SelectedIndex = 0;
        }

        private void InitializeDnsClient()
        {
            try
            {
                // Validate the IP address and port entered by the user
                if (!IPAddress.TryParse(loadBalancerIpAddressTextBox.Text, out IPAddress loadBalancerIp))
                {
                    MessageBox.Show("Invalid IP address format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(loadBalancerPortTextBox.Text, out int loadBalancerPort))
                {
                    MessageBox.Show("Invalid port number format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                loadBalancerEndpoint = new IPEndPoint(loadBalancerIp, loadBalancerPort);

                var options = new DnsUdpClientOptions
                {
                    Endpoint = loadBalancerEndpoint,
                };

                dnsClient = new DnsUdpClient(options);

                var cache = new MemoryCache("DnsCache");

                cachingClient = new DnsCachingClient(dnsClient, cache);

                dnsClientInitialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing DNS client: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dnsClientInitialized = false;
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (!dnsClientInitialized)
            {
                InitializeDnsClient();
            }

            if (dnsClientInitialized)
            {
                loadBalancerLabel.Text = $"Connected to {loadBalancerIpAddressTextBox.Text} at port {loadBalancerPortTextBox.Text}";
            }
            else
            {
                loadBalancerLabel.Text = "Not Connected";
                MessageBox.Show("Failed to connect to the DNS server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString();
            when.Text = time;
            queryTime.Text = "";
            server.Text = "";

            RecordType selectedRecordType = (RecordType)Enum.Parse(typeof(RecordType), type.SelectedItem.ToString());

            try
            {
                // Validate the domain input
                if (string.IsNullOrWhiteSpace(domainInput.Text))
                {
                    MessageBox.Show("Please enter a valid domain name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var watch = System.Diagnostics.Stopwatch.StartNew();

                var query = new DnsMessage
                {
                    Header = new DnsHeader
                    {
                        Id = 1,
                        Host = domainInput.Text,
                        IsQueryResponse = false,
                        RecursionDesired = true,
                        QueryClass = DnsQueryClass.IN,
                        QueryType = (DnsQueryType)selectedRecordType,
                        QuestionCount = 1,
                        AdditionalRecordCount = 0
                    }
                };

                var response = await cachingClient.Query(query, CancellationToken.None);

                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds;
                string serverAddress = response.Answers.LastOrDefault()?.ToString();
                if (!string.IsNullOrEmpty(serverAddress))
                {
                    int startIndex = serverAddress.IndexOf("Resource: ") + "Resource: ".Length;
                    int endIndex = serverAddress.IndexOf(Environment.NewLine, startIndex);
                    if (startIndex > 0 && endIndex > 0)
                    {
                        serverAddress = serverAddress.Substring(startIndex, endIndex - startIndex).Trim();
                    }
                }

                if (response.Answers.Count > 0)
                {
                    for (int i = 0; i < response.Answers.Count; i++)
                    {
                        responseBox.Items.Insert(0, "-------------------------");
                        responseBox.Items.Insert(0, response.Answers[i].ToString());
                        responseBox.Items.Insert(0, time);
                        server.Text = serverAddress;
                    }
                }
                else
                {
                    responseBox.Items.Insert(0, "-------------------------");
                    responseBox.Items.Insert(0, $"No response for requested record type {selectedRecordType} for {domainInput.Text}");
                    responseBox.Items.Insert(0, time);
                    server.Text = serverAddress;
                }

                queryTime.Text = $"{elapsedMs} ms";
            }
            catch (Exception ex)
            {
                //responseBox.Items.Add(DateTime.Now.ToString());
                //responseBox.Items.Add($"Error: {ex.Message}");
                //responseBox.Items.Add("-------------------------");
                responseBox.Items.Insert(0, "-------------------------");
                responseBox.Items.Insert(0, $"Error: {ex.Message}");
                responseBox.Items.Insert(0, time);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            responseBox.Items.Clear();
        }
    }
}