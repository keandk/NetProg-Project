using System;
using System.Linq;
using System.Windows.Forms;
using DNS.Client;
using DNS.Protocol;

namespace DNS_Simulation
{
    public partial class Client : Form
    {
        private DnsClient dnsClient;

        public Client()
        {
            InitializeComponent();
            dnsClient = new DnsClient("127.0.0.1", 8080);
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString();
            when.Text = time;
            queryTime.Text = "";
            server.Text = "";
            messageSize.Text = "";

            // Get the selected record type from the combo box
            RecordType selectedRecordType = (RecordType)Enum.Parse(typeof(RecordType), type.SelectedItem.ToString());

            try
            {
                // Start timer
                var watch = System.Diagnostics.Stopwatch.StartNew();

                IResponse result = await dnsClient.Resolve(domainInput.Text, selectedRecordType);

                // Stop timer
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;

                if (result.AnswerRecords.Count > 0)
                {
                    // Add the answer records to the responseBox
                    foreach (var record in result.AnswerRecords)
                    {
                        responseBox.Items.Add(time);
                        responseBox.Items.Add(record.ToString());
                        responseBox.Items.Add("-------------------------");
                    }
                }
                else
                {
                    responseBox.Items.Add(time);
                    responseBox.Items.Add($"No response for requested record type {selectedRecordType} for {domainInput.Text}");
                    responseBox.Items.Add("-------------------------");
                }

                queryTime.Text = $"{elapsedMs} ms";
                server.Text = "127.0.0.1";
                messageSize.Text = $"{result.Size} bytes";
            }
            catch (ResponseException ex)
            {
                responseBox.Items.Add(time);
                responseBox.Items.Add($"Error: {ex.Message}");
                responseBox.Items.Add("-------------------------");
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