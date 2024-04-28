using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DNS.Client;
using DNS.Protocol;

namespace DNS_Simulation
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }

        private async void sendButton_Click(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString();
            when.Text = time;
            queryTime.Text = "";
            server.Text = "";
            messageSize.Text = "";
            ClientRequest request = new ClientRequest("127.0.0.1", 8080);

            // Get the selected record type from the combo box
            RecordType selectedRecordType = (RecordType)Enum.Parse(typeof(RecordType), type.SelectedItem.ToString());

            request.Questions.Add(new Question(Domain.FromString(domainInput.Text), selectedRecordType, RecordClass.IN));
            request.RecursionDesired = true;

            // start timer
            var watch = System.Diagnostics.Stopwatch.StartNew();

            IResponse result = await request.Resolve();

            // stop timer
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            // Add the answer records to the responseBox
            foreach (var record in result.AnswerRecords)
            {
                responseBox.Items.Add(time);
                responseBox.Items.Add(record.ToString());
                responseBox.Items.Add("-------------------------");
            }

            queryTime.Text = $"{elapsedMs} ms";
            server.Text = "8.8.8.8";
            messageSize.Text = $"{result.Size} bytes";
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            responseBox.Items.Clear();
        }
    }
}
