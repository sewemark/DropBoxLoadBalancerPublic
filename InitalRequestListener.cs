using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace DropBoxLoadBalancer.Infrastructure
{
    public class InitalRequestListener
    {
        private TcpListener tcpListenre;
        private IClientInitialRequestHandler connectionHandler;

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
                Console.WriteLine("Wating for Initial Requests....");
                TcpClient client = await tcpListenre.AcceptTcpClientAsync();
                var address = ((IPEndPoint)client.Client.RemoteEndPoint);
                await connectionHandler.Handle(client, address,usersDict);
            }
        }
    }
}
