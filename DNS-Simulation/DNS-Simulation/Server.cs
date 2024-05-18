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

namespace DNS_Simulation
{
    public partial class Server : Form
    {
        private SQLiteConnection connection_Namespace;
        private SQLiteConnection connection_Resolver;
        private DnsServer server;
        private IRequestResolver resolver;
        private UdpClient udp;

        public Server()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                string connectionString_Namespace = "Data Source=Data\\NameSpace.db;Version=3;";
                connection_Namespace = new SQLiteConnection(connectionString_Namespace);
                connection_Namespace.Open();

                string connectionString_Resolver = "Data Source=Data\\Resolver.db;Version=3;";
                connection_Resolver = new SQLiteConnection(connectionString_Resolver);
                connection_Resolver.Open();

                string createTableQuery = "CREATE TABLE IF NOT EXISTS DNSRecords (DomainName TEXT PRIMARY KEY, IPAddress TEXT, Type TEXT, Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)";

                using (SQLiteCommand createTableCommand_Namespace = new SQLiteCommand(createTableQuery, connection_Namespace))
                using (SQLiteCommand createTableCommand_Resolver = new SQLiteCommand(createTableQuery, connection_Resolver))
                {
                    createTableCommand_Namespace.ExecuteNonQuery();
                    createTableCommand_Resolver.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddOrUpdateRecord(SQLiteConnection connection, string domainName, string ipAddress, string type)
        {
            try
            {
                string insertOrUpdateQuery = "INSERT OR REPLACE INTO DNSRecords (DomainName, IPAddress, Type) VALUES (@DomainName, @IPAddress, @Type)";
                using (SQLiteCommand command = new SQLiteCommand(insertOrUpdateQuery, connection))
                {
                    command.Parameters.AddWithValue("@DomainName", domainName);
                    command.Parameters.AddWithValue("@IPAddress", ipAddress);
                    command.Parameters.AddWithValue("@Type", type);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding or updating record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetIPAddress(string domainName)
        {
            try
            {
                string selectQuery = "SELECT IPAddress FROM DNSRecords WHERE DomainName = @DomainName";
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection_Namespace))
                {
                    command.Parameters.AddWithValue("@DomainName", domainName);
                    object result = command.ExecuteScalar();
                    return result != null ? result.ToString() : null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving IP address: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void UpdateRecordGridView(MasterFile masterFile)
        {
            recordGridView.Rows.Clear();

            // Use reflection to access the protected 'entries' field
            FieldInfo entriesField = typeof(MasterFile).GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance);
            IList<IResourceRecord> entries = (IList<IResourceRecord>)entriesField.GetValue(masterFile);

            foreach (var entry in entries)
            {
                string domain = entry.Name.ToString();
                string initialTtl = entry.TimeToLive.ToString();

                // Calculate the remaining TTL based on the current time and the timestamp when the record was added
                TimeSpan elapsedTime = DateTime.Now - entry.GetTimestampAdded();
                TimeSpan remainingTtl = entry.TimeToLive - elapsedTime;

                // Format the remaining TTL as a string (e.g., "mm:ss")
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
                    case ServiceResourceRecord srvRecord:
                        value = $"{srvRecord.Priority} {srvRecord.Weight} {srvRecord.Port} {srvRecord.Target}";
                        break;
                        // Add more cases for other record types as needed
                }

                // Check if the row for the current domain and record type already exists
                DataGridViewRow existingRow = null;
                foreach (DataGridViewRow row in recordGridView.Rows)
                {
                    if (row.Cells["Domain"].Value?.ToString() == domain && row.Cells["Type"].Value?.ToString() == type)
                    {
                        existingRow = row;
                        break;
                    }
                }

                if (existingRow != null)
                {
                    // Update the existing row with the new TTL and value
                    existingRow.Cells["TTL"].Value = initialTtl;
                    existingRow.Cells["TTD"].Value = formattedRemainingTtl;
                    existingRow.Cells["Value"].Value = value;
                }
                else
                {
                    // Add a new row to the recordGridView
                    recordGridView.Rows.Add(domain, initialTtl, formattedRemainingTtl, value, type);
                }
            }
        }

        private async void listenButton_Click(object sender, EventArgs e)
        {
            try
            {
                //TimeSpan ttl = TimeSpan.FromMinutes(1);
                CustomMasterFile masterFile = new CustomMasterFile();
                resolver = masterFile;
                server = new DnsServer(masterFile, "8.8.8.8");

                //masterFile.AddIPAddressResourceRecord("google.com", "127.0.0.1");
                //masterFile.AddIPAddressResourceRecord("facebook.com", "127.0.0.1");

                UpdateRecordGridView(masterFile);

                server.Requested += (s, args) =>
                {
                    var request = args.Request;
                    var remoteEndpoint = args.Remote;

                    serverLog.Invoke(new Action(() =>
                    {
                        serverLog.Items.Add($"DNS request received from: {remoteEndpoint} for {request.Questions[0].Name}");
                    }));
                };

                server.Responded += async (sender, s) =>
                {
                    // Check if the requested record exists in the master file
                    IList<IResourceRecord> answers = masterFile.Get(s.Request.Questions[0].Name, s.Request.Questions[0].Type);

                    if (answers.Count > 0)
                    {
                        // If the record exists, add the answers to the response
                        foreach (var answer in answers)
                        {
                            s.Response.AnswerRecords.Add(answer);
                        }
                    }
                    else
                    {
                        // If the record does not exist, resolve it and add it to the master file
                        try
                        {
                            await resolver.Resolve(s.Request);
                            if (s.Response.AnswerRecords.Count > 0)
                            {
                                var answer = s.Response.AnswerRecords[0];

                                switch (answer)
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
                                serverLog.Invoke(new Action(() => serverLog.Items.Add($"Response: {answer}")));
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

                    recordGridView.Invoke(new Action(() => UpdateRecordGridView(masterFile)));
                };

                server.Errored += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error: {s.Exception.Message}")));
                server.Listening += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add("Listening...")));

                IPAddress ip = IPAddress.Parse("127.0.0.1");
                await server.Listen(8080, ip);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            DeleteExpiredRecords(connection_Namespace, 60, "Namespace");
            DeleteExpiredRecords(connection_Resolver, 90, "Resolver");
        }

        private void DeleteExpiredRecords(SQLiteConnection connection, int expirationSeconds, string databaseName)
        {
            try
            {
                string deleteQuery = $"DELETE FROM DNSRecords WHERE strftime('%s', 'now') - strftime('%s', Timestamp) > {expirationSeconds};";
                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                {
                    int deletedRows = command.ExecuteNonQuery();
                    serverLog.Items.Add($"{deletedRows} {databaseName}'s records deleted.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting expired records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<IResponse> Resolve(IRequest request)
        {
            try
            {
                return await resolver.Resolve(request);
            }
            catch (Exception ex)
            {
                serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error resolving request: {ex.Message}")));
                return Response.FromRequest(request);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            server?.Dispose();
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