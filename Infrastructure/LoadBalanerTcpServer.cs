using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Linq;


namespace DropBoxLoadBalancer.Infrastructure
{
    public class LoadBalancerTcpServer
    {
        private TcpListener tcpListenre;
        private IClientConnectionHandler connectionHandler;
        public LoadBalancerTcpServer(int _port, IClientConnectionHandler _connectionHandler)
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
                connectionHandler.Handle(client);
            }
        }

    }
}
