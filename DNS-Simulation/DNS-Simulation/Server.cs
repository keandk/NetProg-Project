using DNS.Server;
using Ae.Dns.Server;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using System.Net;
using DNS.Client.RequestResolver;
using System.Net.Sockets;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace DNS_Simulation
{
    public partial class Server : Form
    {
        private DnsServer server1;
        private DnsServer server2;
        private IRequestResolver resolver;
        private LoadBalancer loadBalancer;
        private int server1RequestCount;
        private int server2RequestCount;

        public Server()
        {
            InitializeComponent();
        }

        private void UpdateRecordGridView(MasterFile masterFile, bool isTest)
        {
            if (!isTest)
            {

                try
                {
                    recordGridView.Rows.Clear();

                    if (masterFile == null)
                    {
                        Debug.WriteLine("MasterFile is null.");
                        return;
                    }

                    FieldInfo entriesField = typeof(MasterFile).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
                    IList<IResourceRecord> entries = (IList<IResourceRecord>)entriesField.GetValue(masterFile);

                    if (entries == null)
                    {
                        Debug.WriteLine("Entries is null.");
                        return;
                    }

                    foreach (var entry in entries)
                    {
                        string domain = entry.Name.ToString();
                        string initialTtl = entry.TimeToLive.ToString();

                        TimeSpan elapsedTime = DateTime.Now - entry.GetTimestampAdded();
                        TimeSpan remainingTtl = entry.TimeToLive - elapsedTime;

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

                        recordGridView.Rows.Add(domain, initialTtl, formattedRemainingTtl, value, type);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating record grid view: {ex.Message}");
                }
            }
        }

        private async void listenButton_Click(object sender, EventArgs e)
        {
            try
            {
                CustomMasterFile masterFile = new();
                resolver = masterFile;
                bool isTest = testCheckBox.Checked;

                server1 = new DnsServer(masterFile, "1.1.1.1");
                server2 = new DnsServer(masterFile, "1.1.1.1");

                masterFile.Add(new IPAddressResourceRecord(Domain.FromString("nhom3.com"), IPAddress.Parse("127.0.0.1"), TimeSpan.FromMinutes(1)));
                masterFile.Add(new IPAddressResourceRecord(Domain.FromString("thanhvien.nhom3.com"), IPAddress.Parse("127.0.0.1"), TimeSpan.FromMinutes(1)));
                masterFile.Add(new IPAddressResourceRecord(Domain.FromString("members.nhom3.com"), IPAddress.Parse("127.0.0.1"), TimeSpan.FromMinutes(1)));

                masterFile.Add(new PointerResourceRecord(IPAddress.Parse("127.0.0.1"), Domain.FromString("nhom3.com"), TimeSpan.FromMinutes(1)));
                masterFile.Add(new PointerResourceRecord(IPAddress.Parse("127.0.0.1"), Domain.FromString("thanhvien.nhom3.com"), TimeSpan.FromMinutes(1)));
                masterFile.Add(new PointerResourceRecord(IPAddress.Parse("127.0.0.1"), Domain.FromString("members.nhom3.com"), TimeSpan.FromMinutes(1)));

                UpdateRecordGridView(masterFile, false);

                server1.Requested += (s, args) =>
                {
                    if (args.Request == null)
                    {
                        Debug.WriteLine("Request is null in server1.Requested event.");
                        return;
                    }

                    var request = args.Request;
                    var remoteEndpoint = args.Remote;
                    var requestDomain = request.Questions[0]?.Name?.ToString();

                    if (isTest)
                    {
                        Interlocked.Increment(ref server1RequestCount);
                    }
                    else
                    {
                        serverLog.Invoke(new Action(() =>
                        {
                            serverLog.Items.Insert(0, $"DNS request received by Server 1 from: {remoteEndpoint} for {requestDomain}");
                        }));
                    }
                };

                server2.Requested += (s, args) =>
                {
                    if (args.Request == null)
                    {
                        Debug.WriteLine("Request is null in server2.Requested event.");
                        return;
                    }

                    var request = args.Request;
                    var remoteEndpoint = args.Remote;
                    var requestDomain = request.Questions[0]?.Name?.ToString();

                    if (isTest)
                    {
                        Interlocked.Increment(ref server2RequestCount);
                    }
                    else
                    {
                        serverLog.Invoke(new Action(() =>
                        {
                            serverLog.Items.Insert(0, $"DNS request received by Server 2 from: {remoteEndpoint} for {requestDomain}");
                        }));
                    }
                };

                server1.Responded += async (sender, s) =>
                {
                    try
                    {
                        if (s.Request == null || s.Response == null)
                        {
                            Debug.WriteLine("Request or Response is null in server1.Responded event.");
                            return;
                        }

                        IList<IResourceRecord> answers = masterFile.Get(s.Request.Questions[0].Name, s.Request.Questions[0].Type);

                        if (answers.Count > 0)
                        {
                            // Answers found in the master file
                        }
                        else
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

                                    serverLog.Invoke(new Action(() => serverLog.Items.Insert(0, $"Response: {response}")));
                                }
                            }
                            else
                            {
                                serverLog.Invoke(new Action(() => serverLog.Items.Insert(0, $"Record type {s.Request.Questions[0].Type} not found for domain: {s.Request.Questions[0].Name}")));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error in server1.Responded event: {ex.Message}");
                    }
                    finally
                    {
                        recordGridView.Invoke(new Action(() => UpdateRecordGridView(masterFile, isTest)));
                    }
                };

                server2.Responded += async (sender, s) =>
                {
                    try
                    {
                        if (s.Request == null || s.Response == null)
                        {
                            Debug.WriteLine("Request or Response is null in server2.Responded event.");
                            return;
                        }

                        IList<IResourceRecord> answers = masterFile.Get(s.Request.Questions[0].Name, s.Request.Questions[0].Type);

                        if (answers.Count > 0)
                        {
                            // Answers found in the master file
                        }
                        else
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

                                    serverLog.Invoke(new Action(() => serverLog.Items.Insert(0, $"Response: {response}")));
                                }
                            }
                            else
                            {
                                serverLog.Invoke(new Action(() => serverLog.Items.Insert(0, $"Record type {s.Request.Questions[0].Type} not found for domain: {s.Request.Questions[0].Name}")));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error in server2.Responded event: {ex.Message}");
                    }
                    finally
                    {
                        recordGridView.Invoke(new Action(() => UpdateRecordGridView(masterFile, isTest)));
                    }
                };

                server1.Errored += (sender, s) =>
                {
                    if (s.Exception == null)
                    {
                        Debug.WriteLine("Exception is null in server1.Errored event.");
                        return;
                    }

                    serverLog.Invoke(new Action(() => serverLog.Items.Insert(0, $"Error on Server 1: {s.Exception.Message}")));
                };

                server1.Listening += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Insert(0, "Server 1 is listening...")));

                server2.Errored += (sender, s) =>
                {
                    if (s.Exception == null)
                    {
                        Debug.WriteLine("Exception is null in server2.Errored event.");
                        return;
                    }

                    serverLog.Invoke(new Action(() => serverLog.Items.Insert(0, $"Error on Server 2: {s.Exception.Message}")));
                };

                server2.Listening += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Insert(0, "Server 2 is listening...")));

                List<IPEndPoint> serverEndpoints = new();
                IPAddress? serverIpAddressLan = GetLocalIPAddress();

                if (localRadioButton.Checked)
                {
                    IPAddress ip = IPAddress.Parse("127.0.0.1");
                    serverEndpoints.Add(new IPEndPoint(ip, 8081));
                    serverEndpoints.Add(new IPEndPoint(ip, 8082));
                    ipAddressLabel.Text = $"Load Balancer: {ip}:8080";
                }
                else if (lanRadioButton.Checked)
                {
                    if (serverIpAddressLan != null)
                    {
                        serverEndpoints.Add(new IPEndPoint(serverIpAddressLan, 8081));
                        serverEndpoints.Add(new IPEndPoint(serverIpAddressLan, 8082));
                        ipAddressLabel.Text = $"Load Balancer: {serverIpAddressLan}:8080";
                    }
                    else
                    {
                        MessageBox.Show("No available IP addresses found for LAN mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("No available IP addresses found for LAN mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int poolSize = 10;
                loadBalancer = new LoadBalancer(serverEndpoints, poolSize);
                //loadBalancer = new LoadBalancer(serverEndpoints);

                IPAddress? loadBalancerIp = localRadioButton.Checked ? IPAddress.Parse("127.0.0.1") : serverIpAddressLan;
                int loadBalancerPort = 8080;
                Task loadBalancerTask = Task.Run(() => loadBalancer.StartAsync(loadBalancerIp, loadBalancerPort));

                Task listenTask1 = Task.Run(() => server1.Listen(serverEndpoints[0].Port, serverEndpoints[0].Address));
                Task listenTask2 = Task.Run(() => server2.Listen(serverEndpoints[1].Port, serverEndpoints[1].Address));

                await Task.WhenAll(loadBalancerTask, listenTask1, listenTask2);

                if (isTest)
                {
                    serverLog.Invoke(new Action(() =>
                    {
                        serverLog.Items.Insert(0, $"Server 1 received {server1RequestCount} requests.");
                        serverLog.Items.Insert(0, $"Server 2 received {server2RequestCount} requests.");
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static IPAddress? GetLocalIPAddress()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var iface in interfaces)
                {
                    if (iface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && iface.OperationalStatus == OperationalStatus.Up)
                    {
                        var ipProps = iface.GetIPProperties();
                        foreach (var addr in ipProps.UnicastAddresses)
                        {
                            if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return addr.Address;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving local IP address: {ex.Message}");
            }

            return null;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            try
            {
                server1?.Dispose();
                server2?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error disposing servers: {ex.Message}");
            }
        }

        private void newClient_Click(object sender, EventArgs e)
        {
            Client clientForm = new();
            clientForm.Show();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            // Clear the server log
            serverLog.Items.Clear();
        }
    }

    public static class ResourceRecordExtensions
    {
        public static DateTime GetTimestampAdded(this IResourceRecord record)
        {
            return DateTime.Now;
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