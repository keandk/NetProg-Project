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
        private DnsUdpClient dnsClient;
        private DnsCachingClient cachingClient;
        private bool dnsClientInitialized = false;
        private LoadBalancer loadBalancer;

        public Client(LoadBalancer loadBalancer)
        {
            InitializeComponent();
            this.loadBalancer = loadBalancer;
        }

        private void InitializeDnsClient()
        {
            try
            {
                // Get the next server endpoint from the load balancer
                IPEndPoint serverEndpoint = loadBalancer.GetNextServer();

                var options = new DnsUdpClientOptions
                {
                    Endpoint = serverEndpoint,
                };

                dnsClient = new DnsUdpClient(options);

                var cache = new MemoryCache("DnsCache");

                cachingClient = new DnsCachingClient(dnsClient, cache);

                dnsClientInitialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing DNS client: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            if (!dnsClientInitialized)
            {
                InitializeDnsClient();
                if (!dnsClientInitialized)
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

                var response = await cachingClient.Query(query, CancellationToken.None);
                //var response = await loadBalancer.ResolveQuery(query);

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