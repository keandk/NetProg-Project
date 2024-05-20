using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Ae.Dns.Client;
using Ae.Dns.Protocol;
using DNS.Protocol;

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

        public IPEndPoint GetNextServer()
        {
            IPEndPoint server = servers[currentIndex];
            currentIndex = (currentIndex + 1) % servers.Count;
            return server;
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
    }
}