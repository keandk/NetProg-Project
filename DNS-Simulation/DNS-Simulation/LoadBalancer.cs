using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Ae.Dns.Client;
using Ae.Dns.Client.Exceptions;
using Ae.Dns.Protocol;
using Ae.Dns.Protocol.Enums;

namespace DNS_Simulation
{
    public class DnsClientPool
    {
        private readonly ConcurrentBag<DnsUdpClient> _clientPool;
        private readonly Func<DnsUdpClient> _clientFactory;
        private readonly int _maxPoolSize;

        public DnsClientPool(int maxPoolSize, Func<DnsUdpClient> clientFactory)
        {
            _clientPool = new ConcurrentBag<DnsUdpClient>();
            _clientFactory = clientFactory;
            _maxPoolSize = maxPoolSize;
        }

        public DnsUdpClient GetClient()
        {
            if (_clientPool.TryTake(out var client))
            {
                return client;
            }
            return _clientFactory();
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
    }

    public class LoadBalancer
    {
        private readonly DnsClientPool _dnsClientPool;
        private readonly List<IPEndPoint> _servers;

        public LoadBalancer(List<IPEndPoint> servers, int maxPoolSize)
        {
            _servers = servers;
            _dnsClientPool = new DnsClientPool(maxPoolSize, () => CreateDnsUdpClient());
        }

        private DnsUdpClient CreateDnsUdpClient()
        {
            var endpoint = _servers[new Random().Next(_servers.Count)];
            var options = new DnsUdpClientOptions
            {
                Endpoint = endpoint,
                Timeout = TimeSpan.FromSeconds(10) // Adjust the timeout as needed
            };
            return new DnsUdpClient(options);
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

                var dnsClient = _dnsClientPool.GetClient();
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
                    _dnsClientPool.ReturnClient(dnsClient);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception gracefully
                Debug.WriteLine($"Error processing request: {ex.Message}");
            }
        }
    }
}