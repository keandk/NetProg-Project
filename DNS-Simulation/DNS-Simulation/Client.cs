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
        private readonly ServerControl serverControl;
        private IPAddress? loadBalancerIp;
        private readonly int loadBalancerPort = 8080;

        public Client(ServerControl serverControl)
        {
            InitializeComponent();
            this.serverControl = serverControl;
            InitializeDnsClient();
            if (dnsClientInitialized)
            {
                loadBalancerLabel.Text = "Connected to "+ loadBalancerIp.ToString() + $" at port {loadBalancerPort}";
                Logger.Log($"Connected to {loadBalancerIp.ToString()} at port {loadBalancerPort}");
            }
            else
            {
                loadBalancerLabel.Text = "Not Connected";
                MessageBox.Show("Failed to connect to the DNS server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log("Failed to connect to the DNS server.");
            }
            type.SelectedIndex = 0;
        }

        private void InitializeDnsClient()
        {
            try
            {
                loadBalancerIp = serverControl.loadBalancerIp;

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
                Logger.Log($"Error initializing DNS client: {ex.Message}");
                dnsClientInitialized = false;
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString();
            when.Text = time;
            queryTime.Text = "";
            server.Text = "";

            // Retrieve the selected record type on the main UI thread
            RecordType selectedRecordType = (RecordType)Enum.Parse(typeof(RecordType), type.SelectedItem.ToString());

            Thread sendRequestThread = new Thread(async () =>
            {
                try
                {
                    // Validate the domain input
                    if (string.IsNullOrWhiteSpace(domainInput.Text))
                    {
                        MessageBox.Show("Please enter a valid domain name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

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

                    if (testToggle.Checked)
                    {
                        int totalQueries = int.Parse(numOfRequests.Text);
                        int successfulResponsesW = 0;
                        int failedResponsesW = 0;
                        var testWatch = System.Diagnostics.Stopwatch.StartNew();

                        // Load Balancer Performance Test
                        for (int i = 0; i < totalQueries; i++)
                        {
                            try
                            {
                                var response = await dnsClient.Query(query, CancellationToken.None);
                                if (response.Answers.Count > 0)
                                {
                                    successfulResponsesW++;
                                }
                                else
                                {
                                    failedResponsesW++;
                                }
                            }
                            catch
                            {
                                failedResponsesW++;
                            }
                        }

                        testWatch.Stop();
                        var testElapsedMs = testWatch.ElapsedMilliseconds;

                        Invoke(new Action(() =>
                        {
                            responseBox.Items.Add("-------------------------");
                            responseBox.Items.Add($"Load Balancer Performance Test");
                            responseBox.Items.Add($"Total Queries: {totalQueries}");
                            responseBox.Items.Add($"Successful Responses: {successfulResponsesW}");
                            responseBox.Items.Add($"Failed Responses: {failedResponsesW}");
                            responseBox.Items.Add($"Test Duration: {testElapsedMs} ms");
                            responseBox.Items.Add(time);
                        }));

                        // No Load Balancer Performance Test
                        IPEndPoint server1 = new IPEndPoint(loadBalancerIp, 8081);
                        int successfulResponsesWO = 0;
                        int failedResponsesWO = 0;

                        var options = new DnsUdpClientOptions
                        {
                            Endpoint = server1,
                        };
                        dnsClient = new DnsUdpClient(options);

                        var testWatch2 = System.Diagnostics.Stopwatch.StartNew();

                        for (int i = 0; i < totalQueries; i++)
                        {
                            try
                            {
                                var response = await dnsClient.Query(query, CancellationToken.None);
                                if (response.Answers.Count > 0)
                                {
                                    successfulResponsesWO++;
                                }
                                else
                                {
                                    failedResponsesWO++;
                                }
                            }
                            catch
                            {
                                failedResponsesWO++;
                            }
                        }

                        testWatch2.Stop();
                        var testElapsed2Ms = testWatch2.ElapsedMilliseconds;

                        Invoke(new Action(() =>
                        {
                            responseBox.Items.Add("-------------------------");
                            responseBox.Items.Add($"No Load Balancer Performance Test");
                            responseBox.Items.Add($"Total Queries: {totalQueries}");
                            responseBox.Items.Add($"Successful Responses: {successfulResponsesWO}");
                            responseBox.Items.Add($"Failed Responses: {failedResponsesWO}");
                            responseBox.Items.Add($"Test Duration: {testElapsed2Ms} ms");
                            responseBox.Items.Add(time);
                        }));
                    }
                    else
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();

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

                        Invoke(new Action(() =>
                        {
                            if (response.Answers.Count > 0)
                            {
                                for (int i = 0; i < response.Answers.Count; i++)
                                {
                                    responseBox.Items.Insert(0, "-------------------------");
                                    responseBox.Items.Insert(0, response.Answers[i].ToString());
                                    responseBox.Items.Insert(0, time);
                                    server.Text = serverAddress;
                                    Logger.Log($"[{time}] {response.Answers[i]}");
                                }
                            }
                            else
                            {
                                responseBox.Items.Insert(0, "-------------------------");
                                responseBox.Items.Insert(0, $"No response for requested record type {selectedRecordType} for {domainInput.Text}");
                                responseBox.Items.Insert(0, time);
                                server.Text = serverAddress;
                                Logger.Log($"[{time}] No response for requested record type {selectedRecordType} for {domainInput.Text}");
                            }

                            queryTime.Text = $"{elapsedMs} ms";
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        responseBox.Items.Insert(0, "-------------------------");
                        responseBox.Items.Insert(0, $"Error: {ex.Message}");
                        responseBox.Items.Insert(0, time);
                        Logger.Log($"[{time}] Error: {ex.Message}");
                    }));
                }
            });

            sendRequestThread.Start();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            responseBox.Items.Clear();
        }

        // Dispose the DNS client when the form is closed
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            dnsClient?.Dispose();
        }
    }
}