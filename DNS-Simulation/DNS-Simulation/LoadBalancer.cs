using System.Net;
using System.Net.Sockets;
using Ae.Dns.Client;
using Ae.Dns.Protocol;

namespace DNS_Simulation
{
    public class LoadBalancer
    {
        private List<IPEndPoint> servers;
        private int currentIndex;

        public LoadBalancer(List<IPEndPoint> servers)
        {
            this.servers = servers;
            currentIndex = 0;
        }

        private readonly object _lock = new object();

        public IPEndPoint GetNextServer()
        {
            lock (_lock)
            {
                IPEndPoint server = servers[currentIndex];
                currentIndex = (currentIndex + 1) % servers.Count;
                return server;
            }
        }

        public async Task<DnsMessage> ResolveQuery(DnsMessage query)
        {
            IPEndPoint serverEndpoint = GetNextServer();

            var options = new DnsUdpClientOptions
            {
                Endpoint = serverEndpoint,
            };

            using (var dnsClient = new DnsUdpClient(options))
            {
                var response = await dnsClient.Query(query, CancellationToken.None);
                return response;
            }
        }

        public async Task StartAsync(IPAddress ipAddress, int port)
        {
            var endpoint = new IPEndPoint(ipAddress, port);
            using (var udpClient = new UdpClient(endpoint))
            {
                while (true)
                {
                    var result = await udpClient.ReceiveAsync();
                    var serverEndpoint = GetNextServer();

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

                    // Resolve the server query using the selected server endpoint
                    var options = new DnsUdpClientOptions
                    {
                        Endpoint = serverEndpoint,
                    };

                    using (var dnsClient = new DnsUdpClient(options))
                    {
                        var response = await dnsClient.Query(serverQuery, CancellationToken.None);

                        var responseBytes = new byte[udpClient.Client.ReceiveBufferSize];
                        offset = 0;
                        response.WriteBytes(responseBytes, ref offset);

                        await udpClient.SendAsync(responseBytes, offset, result.RemoteEndPoint);
                    }
                }
            }
        }
    }
}