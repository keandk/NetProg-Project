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

namespace DNS_Simulation
{
    public partial class LanIPAsk : Form
    {
        public IPAddress serverIp;
        public LanIPAsk()
        {
            InitializeComponent();
        }

        private void submitIPButton_Click(object sender, EventArgs e)
        {
            string ip = serverIPInput.Text;
            
            if (ip == "")
            {
                MessageBox.Show("Please enter an IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                IPAddress ipAddr;
                bool valid = IPAddress.TryParse(ip, out ipAddr);
                if (valid)
                {
                    serverIp = ipAddr;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter a valid IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
