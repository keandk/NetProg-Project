using Ae.Dns.Client;
using Ae.Dns.Metrics.InfluxDb;
using Ae.Dns.Protocol;
using Ae.Dns.Protocol.Enums;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Diagnostics;

namespace DNS_Simulation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            IServiceCollection services = new ServiceCollection();

            var dnsUri = new Uri("https://cloudflare-dns.com/");
            services.AddHttpClient<IDnsClient, DnsHttpClient>(x => x.BaseAddress = dnsUri)
                    .AddTransientHttpErrorPolicy(x =>
                        x.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));

            using ServiceProvider provider = services.BuildServiceProvider();

            using IDnsClient dnsClient = provider.GetRequiredService<IDnsClient>();

            string selectedType = rcTypeCombo.SelectedItem.ToString();

            Stopwatch stopwatch = Stopwatch.StartNew();

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
            serverLabel.Text += dnsUri;
        }
    }
}
