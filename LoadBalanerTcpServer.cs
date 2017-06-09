using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DropBoxLoadBalancer.Infrastructure
{
    public class LoadBalancerTcpServer
    {
        private TcpListener tcpListenre;
        private IClientConnectionHandler connectionHandler;
        private List<TcpClient> connectedClients = new List<TcpClient>();
        
        public LoadBalancerTcpServer(IClientConnectionHandler _connectionHandler, int _port)
        {
            this.connectionHandler = _connectionHandler;
            tcpListenre = new TcpListener(IPAddress.Any, _port);
        }

        public async Task RunAsync()
        {
            tcpListenre.Start();
            while (true)
            {
                TcpClient client = await tcpListenre.AcceptTcpClientAsync();
                connectedClients.Add(client);
                connectionHandler.Handle(client);
            }
        }

    }
}
