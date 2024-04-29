using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DNS.Client;
using DNS.Server;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;

namespace DNS_Simulation
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        private async void listenButton_Click(object sender, EventArgs e)
        {
            // Proxy to Google's DNS
            TimeSpan ttl = new TimeSpan(0, 0, 60);
            MasterFile masterFile = new MasterFile(ttl);
            DnsServer server = new DnsServer(masterFile, "8.8.8.8");

            // Resolve the domain names to localhost
            masterFile.AddIPAddressResourceRecord("google.com", "127.0.0.1");
            masterFile.AddIPAddressResourceRecord("facebook.com", "127.0.0.1");

            IList<IResourceRecord> resourceRecords = new List<IResourceRecord>();


            // Log the requests, responses, and errors
            server.Requested += (s, args) =>
            {
                var request = args.Request;
                var remoteEndpoint = args.Remote;

                serverLog.Invoke(new Action(() =>
                {
                    serverLog.Items.Add($"DNS request received from: {remoteEndpoint} for {request.Questions[0].Name}");
                }));
            };

            server.Responded += (sender, s) =>
            {
                if (s.Response.AnswerRecords.Count > 0)                    
                {
                    serverLog.Invoke(new Action(() => serverLog.Items.Add($"Response: {s.Response.AnswerRecords[0]} -> {s.Response.AnswerRecords[0].Data}")));
                }
                else
                {
                    // Handle the case when the requested domain is not found in the MasterFile
                    serverLog.Invoke(new Action(() => serverLog.Items.Add($"Domain not found: {s.Request.Questions[0].Name}. Querying upstream DNS server...")));
                    masterFile.AddIPAddressResourceRecord(s.Request.Questions[0].Name.ToString(), s.Response.AnswerRecords[0].Data.ToString());
                    serverLog.Invoke(new Action(() => serverLog.Items.Add($"Response: {s.Response.AnswerRecords[0]} -> {s.Response.AnswerRecords[0].Data}")));
                }
            };
            server.Errored += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add($"Error: {s.Exception.Message}")));
            server.Listening += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add("Listening...")));
            // server.Received += (sender, s) => serverLog.Invoke(new Action(() => serverLog.Items.Add($"Client connected: {s.EndPoint}")));
            server.Listening += async (sender, s) =>
            {
                DnsClient client = new DnsClient("127.0.0.1", 8080);

                //await client.Lookup("google.com").ConfigureAwait(false);
                //await client.Lookup("facebook.com").ConfigureAwait(false);
                serverLog.Invoke(new Action(() => serverLog.Items.Add("Lookup complete")));
            };

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            await server.Listen(8080, ip).ConfigureAwait(false);
        }
    }
}
