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

        public Server()
        {
            InitializeComponent();
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
                CustomMasterFile masterFile = new CustomMasterFile();
                resolver = masterFile;

                server1 = new DnsServer(masterFile, "8.8.8.8");
                server2 = new DnsServer(masterFile, "1.1.1.1");

                UpdateRecordGridView(masterFile);

                Dictionary<string, bool> requestsInProgress = new Dictionary<string, bool>();

                server1.Requested += (s, args) =>
                {
                    var request = args.Request;
                    var remoteEndpoint = args.Remote;
                    var requestDomain = request.Questions[0].Name.ToString();

                    lock (requestsInProgress)
                    {
                        if (requestsInProgress.ContainsKey(requestDomain))
                        {
                            // Request is already being processed by the other server, skip processing
                            return;
                        }
                        requestsInProgress[requestDomain] = true;
                    }

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

                    lock (requestsInProgress)
                    {
                        if (requestsInProgress.ContainsKey(requestDomain))
                        {
                            // Request is already being processed by the other server, skip processing
                            return;
                        }
                        requestsInProgress[requestDomain] = true;
                    }

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
                        foreach (var answer in answers)
                        {
                            s.Response.AnswerRecords.Add(answer);
                        }
                    }
                    else
                    {
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

                    lock (requestsInProgress)
                    {
                        requestsInProgress.Remove(s.Request.Questions[0].Name.ToString());
                    }

                    recordGridView.Invoke(new Action(() => UpdateRecordGridView(masterFile)));
                };

                server2.Responded += async (sender, s) =>
                {
                    IList<IResourceRecord> answers = masterFile.Get(s.Request.Questions[0].Name, s.Request.Questions[0].Type);

                    if (answers.Count > 0)
                    {
                        foreach (var answer in answers)
                        {
                            s.Response.AnswerRecords.Add(answer);
                        }
                    }
                    else
                    {
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

                    lock (requestsInProgress)
                    {
                        requestsInProgress.Remove(s.Request.Questions[0].Name.ToString());
                    }

                    recordGridView.Invoke(new Action(() => UpdateRecordGridView(masterFile)));
                };

                server1.Errored += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error on Server 1: {s.Exception.Message}")));
                server1.Listening += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add("Server 1 is listening...")));

                server2.Errored += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error on Server 2: {s.Exception.Message}")));
                server2.Listening += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add("Server 2 is listening...")));

                var ipAddresses = NetworkInterface.GetAllNetworkInterfaces()
                                                    .Where(n => n.OperationalStatus == OperationalStatus.Up)
                                                    .SelectMany(n => n.GetIPProperties().UnicastAddresses)
                                                    .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                                                    .Select(a => a.Address)
                                                    .ToList();
                if (localRadioButton.Checked)
                {
                    IPAddress ip = IPAddress.Parse("127.0.0.1");
                    Task listenTask1 = Task.Run(() => server1.Listen(8080, ip));
                    Task listenTask2 = Task.Run(() => server2.Listen(8081, ip));
                    ipAddressLabel.Text = $"IP Address: {ip}";
                    // Wait for both tasks to complete
                    await Task.WhenAll(listenTask1, listenTask2);
                }
                else if (lanRadioButton.Checked && ipAddresses.Count > 0)
                {
                    // Use the first available IP address
                    IPAddress serverIpAddress = ipAddresses[0];

                    // Create separate tasks for each Listen operation
                    Task listenTask1 = Task.Run(() => server1.Listen(8080, serverIpAddress));
                    Task listenTask2 = Task.Run(() => server2.Listen(8081, serverIpAddress));
                    ipAddressLabel.Text = $"IP Address: {serverIpAddress}";

                    // Wait for both tasks to complete
                    await Task.WhenAll(listenTask1, listenTask2);
                }
                else
                {
                    MessageBox.Show("No available IP addresses found for LAN mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            server1?.Dispose();
            server2?.Dispose();
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