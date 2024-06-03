using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ae.Dns.Client;
using Ae.Dns.Client.Exceptions;
using Ae.Dns.Protocol;
using DNS.Server;

namespace DNS_Simulation
{
    public partial class LoadBalancer : Form
    {
        private readonly ConcurrentQueue<DnsClientPool> _clientPools;
        private int _currentPoolIndex;
        private readonly object _lock = new object();
        private int MaxLogItems = 500;

        public LoadBalancer()
        {
        }

        public LoadBalancer(List<IPEndPoint> servers, int maxPoolSize)
        {
            _clientPools = new ConcurrentQueue<DnsClientPool>(servers.Select(server => new DnsClientPool(maxPoolSize, server)));
            InitializeComponent();
        }

        public async Task StartAsync(IPAddress ipAddress, int port)
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

                // Create a new DnsMessage with the extracted header
                var serverQuery = new DnsMessage
                {
                    Header = new DnsHeader
                    {
                        Id = header.Id,
                        IsQueryResponse = header.IsQueryResponse,
                        OperationCode = header.OperationCode,
                        AuthoritativeAnswer = header.AuthoritativeAnswer,
                        Truncation = header.Truncation,
                        RecursionDesired = header.RecursionDesired,
                        RecursionAvailable = header.RecursionAvailable,
                        ResponseCode = header.ResponseCode,
                        QuestionCount = header.QuestionCount,
                        AnswerRecordCount = header.AnswerRecordCount,
                        NameServerRecordCount = header.NameServerRecordCount,
                        AdditionalRecordCount = header.AdditionalRecordCount,
                        QueryType = header.QueryType,
                        QueryClass = header.QueryClass,
                        Host = header.Host
                    }
                };

                // Get the next available client pool using round-robin
                if (!_clientPools.TryDequeue(out var selectedPool))
                {
                    // Handle the case when no client pool is available
                    // You can choose to return an error response or wait for a pool to become available
                    return;
                }

                var dnsClient = selectedPool.GetClient();
                try
                {
                    var response = await dnsClient.Query(serverQuery, CancellationToken.None);

                    var responseBytes = new byte[udpClient.Client.ReceiveBufferSize];
                    offset = 0;
                    response.WriteBytes(responseBytes, ref offset);

                    await udpClient.SendAsync(responseBytes, offset, result.RemoteEndPoint);
                }
                catch (DnsClientTimeoutException ex)
                {
                    Console.WriteLine("Timeout");
                    // Handle DNS client timeout exception
                    // Retry with a different server or return an appropriate response
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error");
                    // Handle other exceptions
                    // Log the error and return an appropriate response
                }
                finally
                {
                    selectedPool.ReturnClient(dnsClient);
                    _clientPools.Enqueue(selectedPool);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception gracefully
                Debug.WriteLine($"Error processing request: {ex.Message}");
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            loadBalanceLog.Items.Clear();
        }

        public void HandleRequestReceived(object sender, Server.RequestReceivedEventArgs args)
        {
            Task.Run(() => AddLoadBalanceLogItem(args.RequestArgs, args.ServerName));
        }

        private void AddLoadBalanceLogItem(DnsServer.RequestedEventArgs args, string serverName)
        {
            var remoteEndpoint = args.Remote;
            var requestDomain = args.Request.Questions[0]?.Name?.ToString();

            string message = $"Request from {remoteEndpoint.Address}:{remoteEndpoint.Port} for {requestDomain} - handling by {serverName}";

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