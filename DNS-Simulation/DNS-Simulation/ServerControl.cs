using DNS.Protocol.ResourceRecords;
using DNS.Server;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DNS_Simulation.ServerForm;

namespace DNS_Simulation
{
    public partial class ServerControl : Form
    {
        private List<IPEndPoint> serverEndpoints = new();
        private bool isLocal;
        private bool isLan;
        private bool isTest;
        private int index;
        public LoadBalancer loadBalancer;
        private ServerForm[] servers;
        public IPAddress? loadBalancerIp;
        IPAddress? serverIpAddressLan;
        public int loadBalancerPort = 8080;

        public ServerControl()
        {
            InitializeComponent();
        }

        private async void listenButton_Click(object sender, EventArgs e)
        {
            int numOfServers = (int)serverNumDropDown.Value;
            int numOfClients = (int)clientNumDropDown.Value;
            isTest = testBalancerCheckBox.Checked;
            isLocal = localRadioButton.Checked;
            isLan = lanRadioButton.Checked;
            servers = new ServerForm[numOfServers];
            serverIpAddressLan = GetLocalIPAddress();

            // create numOfServers servers
            for (index = 0; index < numOfServers; index++)
            {
                if (localRadioButton.Checked)
                {
                    isLocal = true;
                    isLan = false;
                    servers[index] = new ServerForm(index + 1, isLocal, isLan, isTest, this);
                    servers[index].Show();
                    IPAddress ip = IPAddress.Parse("127.0.0.1");
                    serverEndpoints.Add(new IPEndPoint(ip, 8081 + index));
                    Logger.Log($"Server {index + 1} started at {ip}:{8081 + index}");
                }
                else if (lanRadioButton.Checked)
                {
                    if (serverIpAddressLan != null)
                    {
                        isLan = true;
                        isLocal = false;
                        servers[index] = new ServerForm(index + 1, isLocal, isLan, isTest, this);
                        serverEndpoints.Add(new IPEndPoint(serverIpAddressLan, 8081 + index));
                        servers[index].Show();
                        Logger.Log($"Server {index + 1} started at {serverIpAddressLan}:{8081 + index}");
                    }
                    else
                    {
                        MessageBox.Show("No available IP addresses found for LAN mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Logger.Log("No available IP addresses found for LAN mode.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("No available IP addresses found for LAN mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.Log("No available IP addresses found for LAN mode.");
                    return;
                }
            }

            int pool = 20;
            loadBalancer = new LoadBalancer(serverEndpoints, pool);

            loadBalancerIp = localRadioButton.Checked ? IPAddress.Parse("127.0.0.1") : serverIpAddressLan;
            int loadBalancerPort = 8080;
            Thread loadBalancerThread = new Thread(() => loadBalancer.StartAsync(loadBalancerIp, loadBalancerPort));
            loadBalancerThread.Start();
            if (numOfServers > 0)
            {
                loadBalancer.Show();
            }

            for (int i = 0; i < numOfClients; i++)
            {
                if (isLan)
                {
                    LanIPAsk lanIpAsk = new();
                    lanIpAsk.ShowDialog();
                    if (lanIpAsk.serverIp != null)
                    {
                        Client client = new(this, lanIpAsk.serverIp);
                        client.Show();
                    }
                }
                else
                {
                    Client client = new(this);
                    client.Show();
                }
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
                Logger.Log($"Error retrieving local IP address: {ex.Message}");
            }

            return null;
        }

        private void newClientButton_Click(object sender, EventArgs e)
        {
            Client client = new(this);
            client.Show();
        }

        private void newServerButton_Click(object sender, EventArgs e)
        {
            if (isLocal)
            {
                index++;
                ServerForm server = new(index, isLocal, isLan=false, isTest, this);
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                serverEndpoints.Add(new IPEndPoint(ip, 8081 + index));
                Logger.Log($"New server added and running at {ip}:{8081 + index}");
                server.Show();
            }
            else if (isLan)
            {
                ServerForm server = new(index, isLocal=false, isLan, isTest, this);
                serverEndpoints.Add(new IPEndPoint(serverIpAddressLan, 8081 + index));
                Logger.Log($"New server added and running at {serverIpAddressLan}:{8081 + index}");
                server.Show();
            }
        }
    }
}
