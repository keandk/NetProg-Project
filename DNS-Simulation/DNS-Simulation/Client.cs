using System;
using System.Linq;
using System.Windows.Forms;
using DNS.Client;
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
        private DnsRacerClient dnsRacerClient;
        private DnsUdpClient dnsClient1;
        private DnsUdpClient dnsClient2;
        private DnsCachingClient cachingClient1;
        private DnsCachingClient cachingClient2;
        private bool dnsClientsInitialized = false;

        public Client()
        {
            InitializeComponent();
        }

        private void InitializeDnsClients()
        {
            // Get the IP address from the text input
            string ipAddress = serverIpAddressInput.Text;

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                MessageBox.Show("Please enter a valid IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IPAddress parsedIpAddress;
            if (!IPAddress.TryParse(ipAddress, out parsedIpAddress))
            {
                MessageBox.Show("Invalid IP address format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var options1 = new DnsUdpClientOptions
                {
                    Endpoint = new IPEndPoint(parsedIpAddress, 8080),
                };

                var options2 = new DnsUdpClientOptions
                {
                    Endpoint = new IPEndPoint(parsedIpAddress, 8081),
                };

                dnsClient1 = new DnsUdpClient(options1);
                dnsClient2 = new DnsUdpClient(options2);

                var cache = new MemoryCache("DnsCache");

                cachingClient1 = new DnsCachingClient(dnsClient1, cache);
                cachingClient2 = new DnsCachingClient(dnsClient2, cache);

                dnsRacerClient = new DnsRacerClient(cachingClient1, cachingClient2);

                dnsClientsInitialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing DNS clients: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            if (!dnsClientsInitialized)
            {
                InitializeDnsClients();
                if (!dnsClientsInitialized)
                {
                    return;
                }
            }

            string time = DateTime.Now.ToString();
            when.Text = time;
            queryTime.Text = "";
            server.Text = "";
            messageSize.Text = "";

            RecordType selectedRecordType = (RecordType)Enum.Parse(typeof(RecordType), type.SelectedItem.ToString());

            try
            {
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

                var response = await dnsRacerClient.Query(query, CancellationToken.None);

                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds;
                string serverAddress = response.Answers.Last().ToString();
                int startIndex = serverAddress.IndexOf("Resource: ") + "Resource: ".Length;
                int endIndex = serverAddress.IndexOf(Environment.NewLine, startIndex);
                if (startIndex > 0 && endIndex > 0)
                {
                    serverAddress = serverAddress.Substring(startIndex, endIndex - startIndex).Trim();
                }

                for (int i = 0; i < response.Answers.Count; i++)
                {
                    responseBox.Items.Add(time);
                    responseBox.Items.Add(response.Answers[i].ToString());
                    responseBox.Items.Add("-------------------------");
                    server.Text = serverAddress;
                }
                if (response.Answers.Count == 0)
                {
                    responseBox.Items.Add(time);
                    responseBox.Items.Add($"No response for requested record type {selectedRecordType} for {domainInput.Text}");
                    responseBox.Items.Add("-------------------------");

                    server.Text = serverAddress;
                }

                queryTime.Text = $"{elapsedMs} ms";
            }
            catch (Exception ex)
            {
                responseBox.Items.Add(DateTime.Now.ToString());
                responseBox.Items.Add($"Error: {ex.Message}");
                responseBox.Items.Add("-------------------------");
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            responseBox.Items.Clear();
        }
    }
}