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
        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly System.Windows.Forms.Timer _logTimer;
        private const int LogTimerInterval = 3000;

        public LoadBalancer()
        {
        }

        public LoadBalancer(List<IPEndPoint> servers, int maxPoolSize)
        {
            _serverAddresses = servers.Select(server => $"{server.Address}:{server.Port}").ToList();
            _currentIndex = 0;
            _clientPools = new ConcurrentQueue<DnsClientPool>(servers.Select(server => new DnsClientPool(maxPoolSize, server)));
            InitializeComponent();

            _logTimer = new System.Windows.Forms.Timer();
            _logTimer.Interval = LogTimerInterval;
            _logTimer.Tick += LogTimer_Tick;
            _logTimer.Start();
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

                var dnsClient = await selectedPool.GetClientAsync(); // Use asynchronous method to get client from pool
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

        private readonly List<string> _availableServers = new List<string>();

        private string GetNextServerAddress()
        {
            lock (_availableServers)
            {
                if (_availableServers.Count == 0)
                {
                    // Refresh the cache of available servers
                    _availableServers.AddRange(_serverAddresses);
                }

                if (_availableServers.Count > 0)
                {
                    string address = _availableServers[0];
                    _availableServers.RemoveAt(0);
                    return address;
                }
            }

            // If no available servers, return a default or handle the case appropriately
            return string.Empty;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            loadBalanceLog.Items.Clear();
        }

        public void HandleRequestReceived(object sender, ServerForm.RequestReceivedEventArgs args)
        {
            Task.Run(() => EnqueueLogItem(args.RequestArgs, args.ServerName));
        }

        private void EnqueueLogItem(DnsServer.RequestedEventArgs args, string serverName)
        {
            var remoteEndpoint = args.Remote;
            var requestDomain = args.Request.Questions[0]?.Name?.ToString();

            string message = $"Request from {remoteEndpoint.Address}:{remoteEndpoint.Port} for {requestDomain} - handling by {serverName}";
            Logger.Log(message);

            _logQueue.Enqueue(message);
        }

        private void LogTimer_Tick(object sender, EventArgs e)
        {
            if (_logQueue.IsEmpty)
                return;

            var logItems = new List<string>();
            while (_logQueue.TryDequeue(out var logItem))
            {
                logItems.Add(logItem);
            }

            if (loadBalanceLog.InvokeRequired)
            {
                loadBalanceLog.Invoke(new Action(() =>
                {
                    InsertLogItems(logItems);
                }));
            }
            else
            {
                InsertLogItems(logItems);
            }
        }

        private void InsertLogItems(List<string> logItems)
        {
            foreach (var logItem in logItems)
            {
                loadBalanceLog.Items.Insert(0, logItem);
            }

            while (loadBalanceLog.Items.Count > MaxLogItems)
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

        public async Task<DnsUdpClient> GetClientAsync()
        {
            if (_clientPool.TryTake(out var client))
            {
                return client;
            }
            return await CreateDnsUdpClientAsync();
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

        private async Task<DnsUdpClient> CreateDnsUdpClientAsync()
        {
            var options = new DnsUdpClientOptions
            {
                Endpoint = _serverEndpoint,
                Timeout = TimeSpan.FromSeconds(20)
            };
            return await Task.Run(() => new DnsUdpClient(options));
        }
    }
}