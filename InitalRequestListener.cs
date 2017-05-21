using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DropBoxLoadBalancer.Infrastructure
{
    public class InitalRequestListener
    {
        private TcpListener tcpListenre;
        private IClientInitialRequestHandler connectionHandler;
        private ClientInitialRequestHandler clientInitialRequestHandler;
        private int port;

        public InitalRequestListener(IClientInitialRequestHandler _connectionHandler, int _port)
        {
            this.connectionHandler = _connectionHandler;
            tcpListenre = new TcpListener(IPAddress.Any, _port);
        }

        public async Task RunAsync(List<Tuple<string,int, TcpClient>> usersDict)
        {
            tcpListenre.Start();
            while (true)
            {
                Console.WriteLine("Wating....");
                TcpClient client = await tcpListenre.AcceptTcpClientAsync();
                var address = ((IPEndPoint)client.Client.RemoteEndPoint);
                await connectionHandler.Handle(client, address,usersDict);
            }
        }
    }
}
