using Ae.Dns.Client;
using Ae.Dns.Metrics.InfluxDb;
using Ae.Dns.Protocol;
using Ae.Dns.Protocol.Enums;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Diagnostics;
using System.Net;

namespace DNS_Simulation
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        private async Task StartQuery(IDnsClient dnsClient, bool serverType)
        {
            try
            {

                // 1 if http, 0 if udp
                string selectedType = rcTypeCombo.SelectedItem.ToString();
                atTime.Text = "When: ";
                duration.Text = "Query time: ";

                Stopwatch stopwatch = Stopwatch.StartNew();

                if (serverType)
                {
                    activityContainer.Items.Add("Using AdvancedHttpClient");
                }
                else
                {
                    activityContainer.Items.Add("Using BasicUdpClient");
                }

                activityContainer.Items.Add($"Start Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                atTime.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                activityContainer.Items.Add("Querying " + domainInput.Text + " for " + selectedType + " records");

                DnsQueryType queryType = (DnsQueryType)Enum.Parse(typeof(DnsQueryType), selectedType);
                DnsMessage answer = await dnsClient.Query(DnsQueryFactory.CreateQuery(domainInput.Text, queryType));

                stopwatch.Stop();

                activityContainer.Items.Add($"End Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                activityContainer.Items.Add($"Elapsed Time: {stopwatch.Elapsed.TotalMilliseconds} milliseconds");
                activityContainer.Items.Add($"Response: {answer}");
                activityContainer.Items.Add("---------------------------------------------------");

                foreach (var line in answer.ToString().Split(Environment.NewLine))
                {
                    resContainer.Items.Add(line);
                }

                resContainer.Items.Add("---------------------------------------------------");

                duration.Text += stopwatch.Elapsed.TotalMilliseconds + " milliseconds";
            }
            catch (Exception ex)
            {
                activityContainer.Items.Add("Error: " + ex.Message);
            }
        }

        private Uri EnteredUriHttp()
        {
            if (destinationCombo.SelectedItem.ToString() == "Local 1")
            {
                var dnsUri = new Uri("http://nhom3.com/");
                return dnsUri;
            }
            else if (destinationCombo.SelectedItem.ToString() == "Local 2")
            {
                var dnsUri = new Uri("http://group3.com/");
                return dnsUri;

            }
            else if (destinationCombo.SelectedItem.ToString() == "Cloudflare")
            {
                var dnsUri = new Uri("https://cloudflare-dns.com/");
                return dnsUri;

            }
            else if (destinationCombo.SelectedItem.ToString() == "Google")
            {
                var dnsUri = new Uri("https://dns.google/");
                return dnsUri;

            }
            else
            {
                var dnsUri = new Uri("http://nhom3.com/");
                return dnsUri;
            }
        }

        private string EnteredUriUdp()
        {
            string ip = "";
            if (destinationCombo.SelectedItem.ToString() == "Local 1")
            {
                ip = "192.168.91.5";
                return ip;
            }
            else if (destinationCombo.SelectedItem.ToString() == "Local 2")
            {
                ip = "192.168.91.10";
                return ip;

            }
            else if (destinationCombo.SelectedItem.ToString() == "Cloudflare")
            {
                ip = "1.1.1.1";
                return ip;

            }
            else if (destinationCombo.SelectedItem.ToString() == "Google")
            {
                ip = "8.8.8.8";
                return ip;
            }
            else
            {
                return ip;
            }
        }

        private async Task AdvancedHttpClient()
        {
            try
            {// AdvancedHttpClient
                IServiceCollection services = new ServiceCollection();
                serverLabel.Text = "Server: ";

                Uri dnsUri = EnteredUriHttp();
                if(dnsUri == null)
                {
                    throw new Exception("You have not chose the destination!");
                }

                services.AddHttpClient<IDnsClient, DnsHttpClient>(x => x.BaseAddress = dnsUri)
                        .AddTransientHttpErrorPolicy(x =>
                                               x.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));

                using ServiceProvider provider = services.BuildServiceProvider();

                using IDnsClient dnsClient = provider.GetRequiredService<IDnsClient>();

                await StartQuery(dnsClient, true);

                serverLabel.Text += dnsUri;
            }
            catch (Exception ex)
            {
                activityContainer.Items.Add("Error: " + ex.Message);
            }
        }

        private async Task BasicUdpClient()
        {
            try
            {
                string ip = EnteredUriUdp();
                if (ip == "")
                {
                    throw new Exception("You have not chose the destination!");
                }
                serverLabel.Text = "Server: ";
                using IDnsClient dnsClient = new DnsUdpClient(IPAddress.Parse(ip));

                await StartQuery(dnsClient, false);

                serverLabel.Text += ip;
            }
            catch (Exception ex)
            {
                activityContainer.Items.Add("Error: " + ex.Message);
            }
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (httpRadioButton.Checked)
                {
                    resContainer.Items.Add("HTTP Client");
                    await AdvancedHttpClient();
                }
                else if (udpRadioButton.Checked)
                {
                    resContainer.Items.Add("UDP Client");
                    await BasicUdpClient();
                }
                else
                {
                    MessageBox.Show("Please select a server type", "Error!");
                }
            }
            catch (Exception ex)
            {
                activityContainer.Items.Add("Error: " + ex.Message);
            }
        }

        private void createNewClientButton_Click(object sender, EventArgs e)
        {
            Client client = new Client();
            client.Show();
        }
    }
}
