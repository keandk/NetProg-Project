using DNS.Server;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using System.Net;
using DNS.Client.RequestResolver;
using System.Net.Sockets;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Data.SQLite;

namespace DNS_Simulation
{
    public partial class Server : Form
    {
        private DnsServer server1;
        private DnsServer server2;
        private IRequestResolver resolver;
        private LoadBalancer loadBalancer;
        private const int MaxServerLogItems = 500;
        public event EventHandler<RequestReceivedEventArgs> RequestReceived;
        public event EventHandler<IPAddressModeChangedEventArgs> IPAddressModeChanged;

        public Server()
        {
            InitializeComponent();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection("Data Source=E:\\HK4\\LapTrinhMangCanBan\\NetProg-Project\\DNS-Simulation\\DNS-Simulation\\Data\\Namespace.db"))
            {
                connection.Open();
                string tableCreationQuery = @"
            CREATE TABLE IF NOT EXISTS DnsRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Domain TEXT,
                InitialTTL TEXT,
                RemainingTTL TEXT,
                Value TEXT,
                Type TEXT
            )";
                using (var command = new SQLiteCommand(tableCreationQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void SaveEntriesToDatabase(MasterFile masterFile)
        {
            using (var connection = new SQLiteConnection("Data Source=E:\\HK4\\LapTrinhMangCanBan\\NetProg-Project\\DNS-Simulation\\DNS-Simulation\\Data\\Namespace.db"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var deleteCommand = new SQLiteCommand("DELETE FROM DnsRecords", connection, transaction);
                    deleteCommand.ExecuteNonQuery();
                    FieldInfo entriesField = typeof(MasterFile).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
                    IList<IResourceRecord> entries = (IList<IResourceRecord>)entriesField.GetValue(masterFile);

                    foreach (var entry in entries)
                    {
                        string domain = entry.Name.ToString();
                        string initialTtl = entry.TimeToLive.ToString();
                        TimeSpan elapsedTime = DateTime.Now - entry.GetTimestampAdded();
                        TimeSpan remainingTtl = entry.TimeToLive - elapsedTime;
                        string formattedRemainingTtl = $"{remainingTtl.Minutes:D2}:{remainingTtl.Seconds:D2}";
                        string value = entry.ToString();
                        string type = entry.Type.ToString();

                        var insertCommand = new SQLiteCommand("INSERT INTO DnsRecords (Domain, InitialTTL, RemainingTTL, Value, Type) VALUES (@Domain, @InitialTTL, @RemainingTTL, @Value, @Type)", connection, transaction);
                        insertCommand.Parameters.AddWithValue("@Domain", domain);
                        insertCommand.Parameters.AddWithValue("@InitialTTL", initialTtl);
                        insertCommand.Parameters.AddWithValue("@RemainingTTL", formattedRemainingTtl);
                        insertCommand.Parameters.AddWithValue("@Value", value);
                        insertCommand.Parameters.AddWithValue("@Type", type);
                        insertCommand.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        private void UpdateRecordGridViewFromDatabase()
        {
            if (recordGridView.InvokeRequired)
            {
                _ = recordGridView.BeginInvoke(new Action(UpdateRecordGridViewFromDatabase));
                return;
            }

            try
            {
                recordGridView.Rows.Clear();

                using (var connection = new SQLiteConnection("Data Source=E:\\HK4\\LapTrinhMangCanBan\\NetProg-Project\\DNS-Simulation\\DNS-Simulation\\Data\\Namespace.db"))
                {
                    connection.Open();
                    var selectCommand = new SQLiteCommand("SELECT Domain, InitialTTL, RemainingTTL, Value, Type FROM DnsRecords", connection);
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string domain = reader.GetString(0);
                            string initialTtl = reader.GetString(1);
                            string remainingTtl = reader.GetString(2);
                            string value = reader.GetString(3);
                            string type = reader.GetString(4);

                            recordGridView.Rows.Add(domain, initialTtl, remainingTtl, value, type);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating record grid view: {ex.Message}");
            }
        }

        private System.Windows.Forms.Timer refreshTimer;

        private void SetupRefreshTimer()
        {
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 1000; // 1 second
            refreshTimer.Tick += (sender, e) => UpdateRecordGridViewFromDatabase();
            refreshTimer.Start();
        }

        // Call this method in your form's constructor or load event
        private void Initialize()
        {
            InitializeDatabase();
            SetupRefreshTimer();
        }


        private async void listenButton_Click(object sender, EventArgs e)
        {
            try
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

                    SaveEntriesToDatabase(masterFile);
                    UpdateRecordGridViewFromDatabase();

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

                    string message = $"Server 1";
                    RequestReceived?.Invoke(this, new RequestReceivedEventArgs(args, message));
                    serverLog.Invoke(new Action(() =>
                    {
                        AddServerLogItemAsync($"Server 1 is handling request from: {remoteEndpoint} for {requestDomain}");
                    }));
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

                        string message = $"Server 2";
                        RequestReceived?.Invoke(this, new RequestReceivedEventArgs(args, message));
                        serverLog.Invoke(new Action(() =>
                        {
                            AddServerLogItemAsync($"Server 2 is handling request from: {remoteEndpoint} for {requestDomain}");
                        }));

                    };

                    server1.Responded += async (sender, s) =>
                    {
                        await Task.Run(() => HandleRespondedAsync(sender, s, masterFile, isTest));

                    };

                    server2.Responded += async (sender, s) =>
                    {
                        await Task.Run(() => HandleRespondedAsync(sender, s, masterFile, isTest));
                    };

                    server1.Errored += (sender, s) =>
                    {
                        if (s.Exception == null)
                        {
                            Debug.WriteLine("Exception is null in server1.Errored event.");
                            return;
                        }

                        AddServerLogItemAsync($"Error on Server 1: {s.Exception.Message}");
                        Logger.Log($"Error on Server 1: {s.Exception.Message}");
                    };

                    AddServerLogItemAsync("Server 1 is listening...");
                    Logger.Log("Server 1 is listening...");

                    server2.Errored += (sender, s) =>
                    {
                        if (s.Exception == null)
                        {
                            Debug.WriteLine("Exception is null in server2.Errored event.");
                            return;
                        }

                        AddServerLogItem($"Error on Server 2: {s.Exception.Message}");
                        Logger.Log($"Error on Server 2: {s.Exception.Message}");
                    };

                    AddServerLogItem("Server 2 is listening...");
                    Logger.Log("Server 2 is listening...");
                    List<IPEndPoint> serverEndpoints = new();
                    IPAddress? serverIpAddressLan = GetLocalIPAddress();

                    if (localRadioButton.Checked)
                    {
                        IPAddress ip = IPAddress.Parse("127.0.0.1");
                        serverEndpoints.Add(new IPEndPoint(ip, 8081));
                        serverEndpoints.Add(new IPEndPoint(ip, 8082));
                        ipAddressLabel.Text = $"Load Balancer: {ip}:8080";

                        string loadBalancerIpAddress = "127.0.0.1";
                        IPAddressModeChanged?.Invoke(this, new IPAddressModeChangedEventArgs { LoadBalancerIPAddress = loadBalancerIpAddress, LoadBalancerPort = 8080 });
                    }
                    else if (lanRadioButton.Checked)
                    {
                        if (serverIpAddressLan != null)
                        {
                            serverEndpoints.Add(new IPEndPoint(serverIpAddressLan, 8081));
                            serverEndpoints.Add(new IPEndPoint(serverIpAddressLan, 8082));
                            ipAddressLabel.Text = $"Load Balancer: {serverIpAddressLan}:8080";

                            string loadBalancerIpAddress = serverIpAddressLan.ToString();
                            IPAddressModeChanged?.Invoke(this, new IPAddressModeChangedEventArgs { LoadBalancerIPAddress = loadBalancerIpAddress, LoadBalancerPort = 8080 });
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

                    int pool = 10;
                    loadBalancer = new LoadBalancer(serverEndpoints, pool);
                    //loadBalancerInstance = new LoadBalancer();

                    server1.Requested += (sender, args) => loadBalancer.HandleRequestReceived(this, new RequestReceivedEventArgs(args, "Server 1"));
                    server2.Requested += (sender, args) => loadBalancer.HandleRequestReceived(this, new RequestReceivedEventArgs(args, "Server 2"));

                    loadBalancer.Show();

                    IPAddress? loadBalancerIp = localRadioButton.Checked ? IPAddress.Parse("127.0.0.1") : serverIpAddressLan;
                    int loadBalancerPort = 8080;

                    IPAddressModeChanged?.Invoke(this, new IPAddressModeChangedEventArgs { LoadBalancerIPAddress = loadBalancerIp.ToString(), LoadBalancerPort = loadBalancerPort });

                    Task loadBalancerTask = Task.Run(() => loadBalancer.StartAsync(loadBalancerIp, loadBalancerPort));

                    Task listenTask1 = Task.Run(() => server1.Listen(serverEndpoints[0].Port, serverEndpoints[0].Address));
                    Task listenTask2 = Task.Run(() => server2.Listen(serverEndpoints[1].Port, serverEndpoints[1].Address));

                    await Task.WhenAll(loadBalancerTask, listenTask1, listenTask2);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 
            
        

        public class IPAddressModeChangedEventArgs : EventArgs
        {
            public string LoadBalancerIPAddress { get; set; }
            public int LoadBalancerPort { get; set; }
        }

        public class RequestReceivedEventArgs : EventArgs
        {
            public DnsServer.RequestedEventArgs RequestArgs { get; }
            public string ServerName { get; }

            public RequestReceivedEventArgs(DnsServer.RequestedEventArgs requestArgs, string serverName)
            {
                RequestArgs = requestArgs;
                ServerName = serverName;
            }
        }

        private void AddServerLogItemAsync(string message)
        {
            Task.Run(() => AddServerLogItem(message));
        }

        private void AddServerLogItem(string message)
        {
            serverLog.Invoke(new Action(() =>
            {
                serverLog.Items.Insert(0, message);
                if (serverLog.Items.Count > MaxServerLogItems)
                {
                    serverLog.Items.Clear();
                }
            }));
        }

        private async Task HandleRespondedAsync(object sender, DnsServer.RespondedEventArgs s, CustomMasterFile masterFile, bool isTest)
        {
            try
            {
                if (s.Request == null || s.Response == null)
                {
                    Debug.WriteLine("Request or Response is null in Responded event.");
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

                            AddServerLogItemAsync($"Response: {response}");
                            Logger.Log($"Response: {response}");
                        }
                    }
                    else
                    {
                        AddServerLogItemAsync($"Record type {s.Request.Questions[0].Type} not found for domain: {s.Request.Questions[0].Name}");
                        Logger.Log($"Record type {s.Request.Questions[0].Type} not found for domain: {s.Request.Questions[0].Name}");
                    }
                }
                SaveEntriesToDatabase(masterFile);
                UpdateRecordGridViewFromDatabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Responded event: {ex.Message}");
            }
            finally
            {
                UpdateRecordGridViewFromDatabase();
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
            Client clientForm = new(this);
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