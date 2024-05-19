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
        //private DnsClient dnsClientForMessSize;

        public Client()
        {
            InitializeComponent();

            var options1 = new DnsUdpClientOptions
            {
                Endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080),
            };

            var options2 = new DnsUdpClientOptions
            {
                Endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081),
            };

            var dnsClient1 = new DnsUdpClient(options1);
            var dnsClient2 = new DnsUdpClient(options2);

            //dnsClientForMessSize = new DnsClient("127.0.0.1", 8080);

            var cache = new MemoryCache("DnsCache");

            var cachingClient1 = new DnsCachingClient(dnsClient1, cache);
            var cachingClient2 = new DnsCachingClient(dnsClient2, cache);

            dnsRacerClient = new DnsRacerClient(cachingClient1, cachingClient2);
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
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
                
                //IResponse response1 = await dnsClientForMessSize.Resolve(domainInput.Text, selectedRecordType);
                
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
                //server.Text = response.;
                //MessageBox.Show(response.Answers.Last().ToString());
                //messageSize.Text = $"{response1.Size} bytes";
            }
            catch (Exception ex)
            {
                responseBox.Items.Add(time);
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