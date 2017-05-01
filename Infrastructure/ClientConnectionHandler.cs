using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DropBoxLoadBalancer.Infrastructure
{

    public interface IClientConnectionHandler
    {
        Task Handle(TcpClient clients);
    }

    public class ClientConnectionHandler : IClientConnectionHandler
    {
        private readonly int NUM_OF_SERVERS = 5;
        List<Task> clientTasks = new List<Task>();
        List<FileTcpServer> servers = new List<FileTcpServer>();
        private Queue<TcpClient> pendingClients = new Queue<TcpClient>();

        public ClientConnectionHandler()
        {
            for(int i=0;i<5;i++)
            {
                servers.Add(new FileTcpServer(5000 + i));
            }
        }
        public async Task Handle(TcpClient client)
        {
                //Check for idle server;
            var idleServer = servers.Find(x => x.IsIdle());
            if(idleServer != null)
            {
                await idleServer.RunAsync(client);
            }
            else
            {
                pendingClients.Enqueue(client);
            }
        }
    }

}
