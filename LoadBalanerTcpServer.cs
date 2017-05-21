using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Linq;
using System;

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

        public async Task RunAsync(List<Tuple<string, int, TcpClient>> usersDict)
        {
            tcpListenre.Start();
            while (true)
            {
                TcpClient client = await tcpListenre.AcceptTcpClientAsync();
                connectedClients.Add(client);
                await connectionHandler.Handle(client,usersDict);
            }
        }

    }
}
