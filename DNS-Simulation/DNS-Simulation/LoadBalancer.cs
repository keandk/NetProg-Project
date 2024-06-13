using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Ae.Dns.Client;
using Ae.Dns.Client.Exceptions;
using Ae.Dns.Protocol;
using Ae.Dns.Protocol.Enums;
using DNS.Server;

namespace DNS_Simulation
{
    public partial class LoadBalancer : Form
    {
        private readonly List<string> _serverAddresses;
        private int _currentIndex;
        private readonly ConcurrentQueue<DnsClientPool> _clientPools;
        private int MaxLogItems = 500;

        public LoadBalancer()
        {
        }

        public LoadBalancer(List<IPEndPoint> servers, int maxPoolSize)
        {
            _serverAddresses = servers.Select(server => $"{server.Address}:{server.Port}").ToList();
            _currentIndex = 0;
            _clientPools = new ConcurrentQueue<DnsClientPool>(servers.Select(server => new DnsClientPool(maxPoolSize, server)));
            InitializeComponent();
        }

        public async Task StartAsync(IPAddress? ipAddress, int port)
        {
            var endpoint = new IPEndPoint(ipAddress, port);
            using (var udpClient = new UdpClient(endpoint))
            {
                while (true)
                {
                    var result = await udpClient.ReceiveAsync();
                    _ = ProcessRequestAsync(result, udpClient);
                }
            }
        }

        private async Task ProcessRequestAsync(UdpReceiveResult result, UdpClient udpClient)
        {
            try
            {
                var query = new DnsMessage();
                int offset = 0;
                query.ReadBytes(result.Buffer, ref offset);

                var header = query.Header;

                // Get the next available server using round-robin
                var selectedServer = GetNextServerAddress();
                var selectedPool = _clientPools.FirstOrDefault(pool => $"{pool.ServerEndpoint.Address}:{pool.ServerEndpoint.Port}" == selectedServer);
                if (selectedPool == null)
                {
                    // Handle the case when the selected server is not found in the client pools
                    return;
                }

                var dnsClient = selectedPool.GetClient();
                try
                {
                    var response = await dnsClient.Query(query, CancellationToken.None);

                    var responseBytes = new byte[udpClient.Client.ReceiveBufferSize];
                    offset = 0;
                    response.WriteBytes(responseBytes, ref offset);

                    await udpClient.SendAsync(responseBytes, offset, result.RemoteEndPoint);
                }
                catch (Exception ex)
                {
                    // Log the error using a separate logging mechanism
                    Logger.Log($"Error occurred while processing request: {ex.Message}");
                }
                finally
                {
                    selectedPool.ReturnClient(dnsClient);
                }
            }
            catch (Exception ex)
            {
                // Log the error using a separate logging mechanism
                Logger.Log($"Error processing request: {ex.Message}");
            }
        }

        private string GetNextServerAddress()
        {
            string address = _serverAddresses[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _serverAddresses.Count;
            return address;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            loadBalanceLog.Items.Clear();
        }

        public void HandleRequestReceived(object sender, ServerForm.RequestReceivedEventArgs args)
        {
            Task.Run(() => AddLoadBalanceLogItem(args.RequestArgs, args.ServerName));
        }

        private void AddLoadBalanceLogItem(DnsServer.RequestedEventArgs args, string serverName)
        {
            var remoteEndpoint = args.Remote;
            var requestDomain = args.Request.Questions[0]?.Name?.ToString();

            string message = $"Request from {remoteEndpoint.Address}:{remoteEndpoint.Port} for {requestDomain} - handling by {serverName}";
            Logger.Log(message);

            if (loadBalanceLog.InvokeRequired)
            {
                loadBalanceLog.Invoke(new Action(() =>
                {
                    InsertLogItem(message);
                }));
            }
            else
            {
                InsertLogItem(message);
            }
        }

        private void InsertLogItem(string message)
        {
            loadBalanceLog.Items.Insert(0, message);
            if (loadBalanceLog.Items.Count > MaxLogItems)
            {
                loadBalanceLog.Items.RemoveAt(loadBalanceLog.Items.Count - 1);
            }
        }
    }

    public class DnsClientPool
    {
        private readonly ConcurrentBag<DnsUdpClient> _clientPool;
        private readonly int _maxPoolSize;
        private readonly IPEndPoint _serverEndpoint;

        public IPEndPoint ServerEndpoint => _serverEndpoint;

        public DnsClientPool(int maxPoolSize, IPEndPoint serverEndpoint)
        {
            _clientPool = new ConcurrentBag<DnsUdpClient>();
            _maxPoolSize = maxPoolSize;
            _serverEndpoint = serverEndpoint;
        }

        public DnsUdpClient GetClient()
        {
            if (_clientPool.TryTake(out var client))
            {
                return client;
            }
            return CreateDnsUdpClient();
        }

        public void ReturnClient(DnsUdpClient client)
        {
            if (_clientPool.Count < _maxPoolSize)
            {
                _clientPool.Add(client);
            }
            else
            {
                client.Dispose();
            }
        }

        private DnsUdpClient CreateDnsUdpClient()
        {
            var options = new DnsUdpClientOptions
            {
                Endpoint = _serverEndpoint,
                Timeout = TimeSpan.FromSeconds(10)
            };
            return new DnsUdpClient(options);
        }
    }
}