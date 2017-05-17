using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

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
        List<TcpClient> pendingClients = new List<TcpClient>();
        List<TcpClient> connectedClients = new List<TcpClient>();
        IObservable<FileTcpServer> observableServers;
        IObservable<TcpClient> observablClient;
        Subject<TcpClient> clientSubjectss = new Subject<TcpClient>();
        Subject<FileTcpServer> clientServer = new Subject<FileTcpServer>();

        public ClientConnectionHandler()
        {
            for(int i=0;i<5;i++)
            {
                servers.Add(new FileTcpServer(5000 + i, connectedClients));

            }
            observableServers = servers.ToObservable().Concat(clientServer);
            observableServers.Subscribe(OnNext);
        }

        private  void OnNext(FileTcpServer server)
        {
            Console.WriteLine("empty server: EMPTY " +  server.IsIdle() + "\\" +  server.path);
            if(pendingClients.Count > 0)
            {
                pendingClients.ForEach(x =>
                {
                    Console.WriteLine("Klient " + x.ReceiveBufferSize.ToString() + x.Client.ReceiveBufferSize.ToString());
                });
            }
        }

        public async Task Handle(TcpClient client)
        {
            //Check for idle server;
            connectedClients.Add(client);
            var idleServer = servers.Find(x => x.IsIdle());
            if(idleServer != null)
            {
                servers.Remove(idleServer);
                await idleServer.RunAsync(client,()=> OnNextEmptyServer(idleServer));
            }
            else
            {
                pendingClients.Add(client);

            }
            
        }

        public void OnNextEmptyServer(FileTcpServer idleServer)
        {
            clientServer.OnNext(idleServer);
        }
    }

}
