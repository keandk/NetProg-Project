using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DNS.Client;
using DNS.Server;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using System.Data.SQLite;
using System.Net;
using System.Threading.Tasks;
using DNS.Client.RequestResolver;
using System.Net.Sockets;
using System.Reflection;
using System.Net.NetworkInformation;

namespace DNS_Simulation
{
    public partial class Server : Form
    {
        private DnsServer server1;
        private DnsServer server2;
        private IRequestResolver resolver;
        private LoadBalancer loadBalancer;

        public Server()
        {
            InitializeComponent();
        }

        private void UpdateRecordGridView(MasterFile masterFile)
        {
            recordGridView.Rows.Clear();

            // Sử dụng reflection để truy cập trường 'entries' được bảo vệ
            FieldInfo entriesField = typeof(MasterFile).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
            IList<IResourceRecord> entries = (IList<IResourceRecord>)entriesField.GetValue(masterFile);

            foreach (var entry in entries)
            {
                string domain = entry.Name.ToString();
                string initialTtl = entry.TimeToLive.ToString();

                // Tính toán TTL còn lại dựa trên thời gian hiện tại và timestamp khi bản ghi được thêm
                TimeSpan elapsedTime = DateTime.Now - entry.GetTimestampAdded();
                TimeSpan remainingTtl = entry.TimeToLive - elapsedTime;

                // Định dạng TTL còn lại dưới dạng chuỗi (e.g., "mm:ss")
                string formattedRemainingTtl = $"{remainingTtl.Minutes:D2}:{remainingTtl.Seconds:D2}";

                string value = string.Empty;
                string type = entry.Type.ToString();

                switch (entry)
                {
                    case IPAddressResourceRecord ipAddressRecord:
                        value = ipAddressRecord.IPAddress.ToString();
                        break;
                    case CanonicalNameResourceRecord cnameRecord:
                        value = cnameRecord.CanonicalDomainName.ToString();
                        break;
                    case PointerResourceRecord ptrRecord:
                        value = ptrRecord.PointerDomainName.ToString();
                        break;
                    case MailExchangeResourceRecord mxRecord:
                        value = $"{mxRecord.Preference} {mxRecord.ExchangeDomainName}";
                        break;
                    case NameServerResourceRecord nsRecord:
                        value = nsRecord.NSDomainName.ToString();
                        break;
                    case TextResourceRecord txtRecord:
                        value = txtRecord.ToStringTextData();
                        break;
                }

                // Add a new row to the recordGridView
                recordGridView.Rows.Add(Guid.NewGuid().ToString(), domain, initialTtl, formattedRemainingTtl, value, type);
            }
        }


        private async void listenButton_Click(object sender, EventArgs e)
        {
            try
            {
                CustomMasterFile masterFile = new();
                resolver = masterFile;

                server1 = new DnsServer(masterFile, "1.1.1.1");
                server2 = new DnsServer(masterFile, "8.8.8.8");

                UpdateRecordGridView(masterFile);

                //Dictionary<string, bool> requestsInProgress = new Dictionary<string, bool>();

                server1.Requested += (s, args) =>
                {
                    var request = args.Request;
                    var remoteEndpoint = args.Remote;
                    var requestDomain = request.Questions[0].Name.ToString();

                    //lock (requestsInProgress)
                    //{
                    //    if (requestsInProgress.ContainsKey(requestDomain))
                    //    {
                    //        // Request is already being processed by the other server, skip processing
                    //        return;
                    //    }
                    //    requestsInProgress[requestDomain] = true;
                    //}

                    serverLog.Invoke(new Action(() =>
                    {
                        serverLog.Items.Add($"DNS request received by Server 1 from: {remoteEndpoint} for {request.Questions[0].Name}");
                    }));
                };

                server2.Requested += (s, args) =>
                {
                    var request = args.Request;
                    var remoteEndpoint = args.Remote;
                    var requestDomain = request.Questions[0].Name.ToString();

                    //lock (requestsInProgress)
                    //{
                    //    if (requestsInProgress.ContainsKey(requestDomain))
                    //    {
                    //        // Request is already being processed by the other server, skip processing
                    //        return;
                    //    }
                    //    requestsInProgress[requestDomain] = true;
                    //}

                    serverLog.Invoke(new Action(() =>
                    {
                        serverLog.Items.Add($"DNS request received by Server 2 from: {remoteEndpoint} for {request.Questions[0].Name}");
                    }));
                };

                server1.Responded += async (sender, s) =>
                {
                    IList<IResourceRecord> answers = masterFile.Get(s.Request.Questions[0].Name, s.Request.Questions[0].Type);

                    if (answers.Count > 0)
                    {
                        //foreach (var answer in answers)
                        //{
                        //    s.Response.AnswerRecords.Add(answer);
                        //}
                    }
                    else
                    {
                        try
                        {
                            await resolver.Resolve(s.Request);
                            if (s.Response.AnswerRecords.Count > 0)
                            {
                                foreach (var response in s.Response.AnswerRecords)
                                {
                                    switch (response)
                                    {
                                        case IPAddressResourceRecord ipAddressRecord:
                                            masterFile.Add(new IPAddressResourceRecord(s.Request.Questions[0].Name, ipAddressRecord.IPAddress, ipAddressRecord.TimeToLive));
                                            break;
                                        case CanonicalNameResourceRecord cnameRecord:
                                            masterFile.Add(new CanonicalNameResourceRecord(s.Request.Questions[0].Name, cnameRecord.CanonicalDomainName, cnameRecord.TimeToLive));
                                            break;
                                        case MailExchangeResourceRecord mxRecord:
                                            masterFile.AddMailExchangeResourceRecord(s.Request.Questions[0].Name.ToString(), mxRecord.Preference, mxRecord.ExchangeDomainName.ToString());
                                            break;
                                        case NameServerResourceRecord nsRecord:
                                            masterFile.Add(new NameServerResourceRecord(s.Request.Questions[0].Name, nsRecord.NSDomainName, nsRecord.TimeToLive));
                                            break;
                                        case TextResourceRecord txtRecord:
                                            masterFile.Add(new TextResourceRecord(s.Request.Questions[0].Name, txtRecord.Attribute.Value, txtRecord.TextData.ToString(), txtRecord.TimeToLive));
                                            break;
                                    }

                                    serverLog.Invoke(new Action(() => serverLog.Items.Add($"Response: {response}")));
                                }
                                //var answer = s.Response.AnswerRecords[0];
                            }
                            else
                            {
                                serverLog.Invoke(new Action(() => serverLog.Items.Add($"Record type {s.Request.Questions[0].Type} not found for domain: {s.Request.Questions[0].Name}")));
                            }
                        }
                        catch (Exception ex)
                        {
                            serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error resolving domain: {s.Request.Questions[0].Name}. {ex.Message}")));
                        }
                    }

                    //lock (requestsInProgress)
                    //{
                    //    requestsInProgress.Remove(s.Request.Questions[0].Name.ToString());
                    //}

                    recordGridView.Invoke(new Action(() => UpdateRecordGridView(masterFile)));
                };

                server2.Responded += async (sender, s) =>
                {
                    IList<IResourceRecord> answers = masterFile.Get(s.Request.Questions[0].Name, s.Request.Questions[0].Type);

                    if (answers.Count > 0)
                    {
                        //foreach (var answer in answers)
                        //{
                        //    s.Response.AnswerRecords.Add(answer);
                        //}
                    }
                    else
                    {
                        try
                        {
                            await resolver.Resolve(s.Request);
                            if (s.Response.AnswerRecords.Count > 0)
                            {
                                foreach (var response in s.Response.AnswerRecords)
                                {
                                    switch (response)
                                    {
                                        case IPAddressResourceRecord ipAddressRecord:
                                            masterFile.Add(new IPAddressResourceRecord(s.Request.Questions[0].Name, ipAddressRecord.IPAddress, ipAddressRecord.TimeToLive));
                                            break;
                                        case CanonicalNameResourceRecord cnameRecord:
                                            masterFile.Add(new CanonicalNameResourceRecord(s.Request.Questions[0].Name, cnameRecord.CanonicalDomainName, cnameRecord.TimeToLive));
                                            break;
                                        case MailExchangeResourceRecord mxRecord:
                                            masterFile.AddMailExchangeResourceRecord(s.Request.Questions[0].Name.ToString(), mxRecord.Preference, mxRecord.ExchangeDomainName.ToString());
                                            break;
                                        case NameServerResourceRecord nsRecord:
                                            masterFile.Add(new NameServerResourceRecord(s.Request.Questions[0].Name, nsRecord.NSDomainName, nsRecord.TimeToLive));
                                            break;
                                        case TextResourceRecord txtRecord:
                                            masterFile.Add(new TextResourceRecord(s.Request.Questions[0].Name, txtRecord.Attribute.Value, txtRecord.TextData.ToString(), txtRecord.TimeToLive));
                                            break;
                                    }

                                    serverLog.Invoke(new Action(() => serverLog.Items.Add($"Response: {response}")));
                                }
                                //var answer = s.Response.AnswerRecords[0];

                            }
                            else
                            {
                                serverLog.Invoke(new Action(() => serverLog.Items.Add($"Record type {s.Request.Questions[0].Type} not found for domain: {s.Request.Questions[0].Name}")));
                            }
                        }
                        catch (Exception ex)
                        {
                            serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error resolving domain: {s.Request.Questions[0].Name}. {ex.Message}")));
                        }
                    }

                    //lock (requestsInProgress)
                    //{
                    //    requestsInProgress.Remove(s.Request.Questions[0].Name.ToString());
                    //}

                    recordGridView.Invoke(new Action(() => UpdateRecordGridView(masterFile)));
                };

                server1.Errored += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error on Server 1: {s.Exception.Message}")));
                server1.Listening += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add("Server 1 is listening...")));

                server2.Errored += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error on Server 2: {s.Exception.Message}")));
                server2.Listening += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add("Server 2 is listening...")));

                // Define the server endpoints
                List<IPEndPoint> serverEndpoints = new();

                if (localRadioButton.Checked)
                {
                    IPAddress ip = IPAddress.Parse("127.0.0.1");
                    serverEndpoints.Add(new IPEndPoint(ip, 8081));
                    serverEndpoints.Add(new IPEndPoint(ip, 8082));
                    ipAddressLabel.Text = $"IP Address: {ip}";
                }
                else if (lanRadioButton.Checked)
                {
                    IPAddress serverIpAddress = IPAddress.Parse("127.0.2.2");
                    serverEndpoints.Add(new IPEndPoint(serverIpAddress, 8081));
                    serverEndpoints.Add(new IPEndPoint(serverIpAddress, 8082));
                    ipAddressLabel.Text = $"IP Address: {serverIpAddress}";
                }
                else
                {
                    MessageBox.Show("No available IP addresses found for LAN mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Initialize the load balancer with server endpoints
                loadBalancer = new LoadBalancer(serverEndpoints);

                // Start the load balancer asynchronously
                IPAddress loadBalancerIp = localRadioButton.Checked ? IPAddress.Parse("127.0.0.1") : IPAddress.Parse("127.0.2.2");
                int loadBalancerPort = 8080;
                Task loadBalancerTask = Task.Run(() => loadBalancer.StartAsync(loadBalancerIp, loadBalancerPort));

                // Start listening on the server endpoints asynchronously
                Task listenTask1 = Task.Run(() => server1.Listen(serverEndpoints[0].Port, serverEndpoints[0].Address));
                Task listenTask2 = Task.Run(() => server2.Listen(serverEndpoints[1].Port, serverEndpoints[1].Address));

                // Wait for all tasks to complete
                await Task.WhenAll(loadBalancerTask, listenTask1, listenTask2);

                MessageBox.Show("Server started successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            server1?.Dispose();
            server2?.Dispose();
        }

        private void newClient_Click(object sender, EventArgs e)
        {
            Client clientForm = new();
            clientForm.Show();
        }
    }

    public static class ResourceRecordExtensions
    {
        public static DateTime GetTimestampAdded(this IResourceRecord record)
        {
            return DateTime.Now; // Assume the current timestamp as the added timestamp
        }
    }

    public class CustomMasterFile : MasterFile
    {
        public CustomMasterFile() : base() { }

        public new IList<IResourceRecord> Get(Domain domain, RecordType type)
        {
            return base.Get(domain, type);
        }
    }
}