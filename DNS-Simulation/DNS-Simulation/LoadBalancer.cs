using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DNS.Protocol;

namespace DNS_Simulation
{
    public class LoadBalancer
    {
        private readonly IPEndPoint server1EndPoint;
        private readonly IPEndPoint server2EndPoint;

        public LoadBalancer(IPEndPoint server1EndPoint, IPEndPoint server2EndPoint)
        {
            this.server1EndPoint = server1EndPoint;
            this.server2EndPoint = server2EndPoint;
        }

        public async Task StartAsync(int port)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            using UdpClient udpClient = new UdpClient(localEndPoint);

            MessageBox.Show($"Load balancer is listening on port {port}");

            while (true)
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                byte[] requestData = result.Buffer;
                IPEndPoint clientEndPoint = result.RemoteEndPoint;

                Task.Run(() => ForwardRequestAsync(udpClient, requestData, clientEndPoint));
            }
        }

        private async Task ForwardRequestAsync(UdpClient udpClient, byte[] requestData, IPEndPoint clientEndPoint)
        {
            try
            {
                // Select the server to forward the request to (e.g., round-robin or any other load balancing algorithm)
                IPEndPoint selectedServerEndPoint = GetServerEndPoint();

                // Forward the request to the selected server
                await udpClient.SendAsync(requestData, requestData.Length, selectedServerEndPoint);

                // Receive the response from the server
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                byte[] responseData = result.Buffer;

                // Send the response directly to the client
                await udpClient.SendAsync(responseData, responseData.Length, clientEndPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error forwarding request: {ex.Message}");
            }
        }

        private IPEndPoint GetServerEndPoint()
        {
            // Implement your load balancing algorithm here
            // For simplicity, this example alternates between the two servers
            return DateTime.Now.Ticks % 2 == 0 ? server1EndPoint : server2EndPoint;
        }
    }
}