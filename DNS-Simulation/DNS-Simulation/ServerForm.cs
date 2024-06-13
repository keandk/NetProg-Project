﻿using DNS.Server;
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
    public partial class ServerForm : Form
    {
        private DnsServer server;
        private readonly int index;
        private readonly bool isLocal;
        private readonly bool isLAN;
        private IRequestResolver resolver;
        private readonly ServerControl serverControl;
        private const int MaxServerLogItems = 1000;
        private readonly string connectionString = "Data Source=.\\Data\\Namespace.db";
        public event EventHandler<RequestReceivedEventArgs> RequestReceived;
        public event EventHandler<IPAddressModeChangedEventArgs> IPAddressModeChanged;

        public ServerForm(int index, bool isLocal, bool isLAN, ServerControl serverControl)
        {
            this.index = index;
            this.isLocal = isLocal;
            this.isLAN = isLAN;
            this.serverControl = serverControl;
            InitializeComponent();
            InitializeServer();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(connectionString);
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
            using var command = new SQLiteCommand(tableCreationQuery, connection);
            command.ExecuteNonQuery();
            Logger.Log("Database initialized");
        }

        private void SaveEntriesToDatabase(MasterFile masterFile)
        {
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
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

                Logger.Log($"Saved record to database: {domain} {initialTtl} {formattedRemainingTtl} {value} {type}");
            }

            transaction.Commit();
        }

        private IList<IResourceRecord> GetRecordsFromDatabase(string domain, RecordType type)
        {
            IList<IResourceRecord> records = new List<IResourceRecord>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var selectCommand = new SQLiteCommand("SELECT Domain, InitialTTL, RemainingTTL, Value, Type FROM DnsRecords WHERE Domain = @Domain AND Type = @Type", connection);
                selectCommand.Parameters.AddWithValue("@Domain", domain);
                selectCommand.Parameters.AddWithValue("@Type", type.ToString());

                using var reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    string recordDomain = reader.GetString(0);
                    TimeSpan initialTtl = TimeSpan.Parse(reader.GetString(1));
                    TimeSpan remainingTtl = TimeSpan.Parse(reader.GetString(2));
                    string value = reader.GetString(3);
                    RecordType recordType = (RecordType)Enum.Parse(typeof(RecordType), reader.GetString(4));

                    IResourceRecord record = recordType switch
                    {
                        RecordType.A => new IPAddressResourceRecord(Domain.FromString(recordDomain), IPAddress.Parse(value), remainingTtl),
                        RecordType.PTR => new PointerResourceRecord(IPAddress.Parse(recordDomain), Domain.FromString(value), remainingTtl),
                        _ => throw new NotSupportedException($"Record type {recordType} is not supported"),
                    };

                    records.Add(record);

                    Logger.Log($"Retrieved record from database: {recordDomain} {initialTtl} {remainingTtl} {value} {recordType}");
                }
            }

            return records;
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

                using var connection = new SQLiteConnection(connectionString);
                connection.Open();
                var selectCommand = new SQLiteCommand("SELECT Domain, InitialTTL, RemainingTTL, Value, Type FROM DnsRecords", connection);
                using var reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    string domain = reader.GetString(0);
                    string initialTtl = reader.GetString(1);
                    string remainingTtl = reader.GetString(2);
                    string value = reader.GetString(3);
                    string type = reader.GetString(4);

                    recordGridView.Rows.Add(domain, initialTtl, remainingTtl, value, type);

                    Logger.Log($"Updated record grid view: {domain} {initialTtl} {remainingTtl} {value} {type}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating record grid view: {ex.Message}");
                Logger.Log($"Error updating record grid view: {ex.Message}");
            }
        }

        private void InitializeServer()
        {
            try
            {
                CustomMasterFile masterFile = new();
                resolver = masterFile;
                IPAddress ip;

                server = new DnsServer(masterFile, "1.1.1.1");
                //server2 = new DnsServer(masterFile, "1.1.1.1");

                masterFile.Add(new IPAddressResourceRecord(Domain.FromString("nhom3.com"), IPAddress.Parse("10.10.10.1"), TimeSpan.FromMinutes(1)));
                masterFile.Add(new IPAddressResourceRecord(Domain.FromString("thanhvien.nhom3.com"), IPAddress.Parse("10.10.10.1"), TimeSpan.FromMinutes(1)));
                masterFile.Add(new IPAddressResourceRecord(Domain.FromString("members.nhom3.com"), IPAddress.Parse("10.10.10.1"), TimeSpan.FromMinutes(1)));

                masterFile.Add(new PointerResourceRecord(IPAddress.Parse("10.10.10.1"), Domain.FromString("nhom3.com"), TimeSpan.FromMinutes(1)));
                masterFile.Add(new PointerResourceRecord(IPAddress.Parse("10.10.10.1"), Domain.FromString("thanhvien.nhom3.com"), TimeSpan.FromMinutes(1)));
                masterFile.Add(new PointerResourceRecord(IPAddress.Parse("10.10.10.1"), Domain.FromString("members.nhom3.com"), TimeSpan.FromMinutes(1)));

                SaveEntriesToDatabase(masterFile);
                UpdateRecordGridViewFromDatabase();

                server.Requested += (s, args) =>
                {
                    if (args.Request == null)
                    {
                        Debug.WriteLine("Request is null in server.Requested event.");
                        return;
                    }

                    var request = args.Request;
                    var remoteEndpoint = args.Remote;
                    var requestDomain = request.Questions[0]?.Name?.ToString();

                    string message = $"Server 1";
                    RequestReceived?.Invoke(this, new RequestReceivedEventArgs(args, message));
                    serverLog.Invoke(new Action(() =>
                    {
                        AddServerLogItemAsync($"Server {index} is handling request from: {remoteEndpoint} for {requestDomain}");
                        Logger.Log($"Server {index} is handling request from: {remoteEndpoint} for {requestDomain}");
                    }));
                };

                server.Responded += async (sender, s) =>
                {
                    _ = ThreadPool.QueueUserWorkItem(state => HandleRespondedAsync(sender, s, masterFile));
                };

                server.Errored += (sender, s) =>
                {
                    if (s.Exception == null)
                    {
                        Debug.WriteLine("Exception is null in server.Errored event.");
                        return;
                    }

                    AddServerLogItemAsync($"Error on Server {index}: {s.Exception.Message}");
                    Logger.Log($"Error on Server {index}: {s.Exception.Message}");
                };

                AddServerLogItemAsync($"Server {index} is listening...");
                Logger.Log($"Server {index} is listening...");

                if (isLocal)
                {
                    ip = IPAddress.Parse("127.0.0.1");
                    ipAddressLabel.Text = ip.ToString() + $":{8081 + index}";
                    Logger.Log($"Server {index} started at localhost"); 
                }
                else if (isLAN)
                {
                    ip = GetLocalIPAddress();
                    if (ip != null)
                    {
                        ipAddressLabel.Text = ip.ToString() + $":{8081 + index}";
                        Logger.Log($"Server {index} started at {ip}:{8080 + index}");
                    }
                }
                else
                {
                    MessageBox.Show("No available IP addresses found for LAN mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.Log("No available IP addresses found for LAN mode.");
                    return;
                }
                IPEndPoint serverEndpoints = new(ip, 8080 + index);
                server.Requested += (sender, args) => serverControl.loadBalancer.HandleRequestReceived(this, new RequestReceivedEventArgs(args, $"Server {index}"));

                Thread listenThread = new(() => server.Listen(serverEndpoints.Port, serverEndpoints.Address));
                Logger.Log($"Server {index} started at {serverEndpoints.Address}:{serverEndpoints.Port}");

                listenThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Log($"Error starting server: {ex.Message}");
            }
        }



        public class IPAddressModeChangedEventArgs : EventArgs
        {
            public string? LoadBalancerIPAddress { get; set; }
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

        private async Task HandleRespondedAsync(object sender, DnsServer.RespondedEventArgs s, CustomMasterFile masterFile)
        {
            try
            {
                if (s.Request == null || s.Response == null)
                {
                    Debug.WriteLine("Request or Response is null in Responded event.");
                    return;
                }

                //IList<IResourceRecord> answers = masterFile.Get(s.Request.Questions[0].Name, s.Request.Questions[0].Type);
                string domain = s.Request.Questions[0].Name.ToString();
                RecordType type = s.Request.Questions[0].Type;

                // Check database first
                IList<IResourceRecord> answers = GetRecordsFromDatabase(domain, type);

                if (answers.Count > 0)
                {
                    // Answers found in the database
                    foreach (var answer in answers)
                    {
                        s.Response.AnswerRecords.Add(answer);
                        AddServerLogItemAsync($"Database Response: {answer}");
                        Logger.Log($"Database Response: {answer}");
                    }
                }
                else
                {
                    answers = masterFile.Get(s.Request.Questions[0].Name, s.Request.Questions[0].Type);

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
                server?.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error disposing servers: {ex.Message}");
            }
        }

        private void newClient_Click(object sender, EventArgs e)
        {
            Client clientForm = new(serverControl);
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