using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DropBoxLoadBalancer.Persistence;
using System.Linq;
using System.Net;

namespace DropBoxLoadBalancer.Infrastructure
{

    public interface IClientConnectionHandler
    {
        Task Handle(TcpClient clients);
    }

    public class ClientConnectionHandler : IClientConnectionHandler
    {
        private readonly int NUM_OF_SERVERS = 2;
        List<FileTcpServer> servers = new List<FileTcpServer>();
        List<TcpClient> pendingClients = new List<TcpClient>();
        List<TcpClient> connectedClients = new List<TcpClient>();
        IObservable<FileTcpServer> observableServers;
        Subject<TcpClient> clientSubjectss = new Subject<TcpClient>();
        Subject<FileTcpServer> clientServer = new Subject<FileTcpServer>();
        List<Tuple<string, int, TcpClient>> usersDict;
        public ClientConnectionHandler(DbHandler dbHandler, List<Tuple<string, int, TcpClient>> _usersDict)
        {
            this.usersDict = _usersDict;
            for(int i=0;i< NUM_OF_SERVERS; i++)
            {
                servers.Add(new FileTcpServer(5000 + i, connectedClients,new NetworkInfrastructure(dbHandler)));

            }
            observableServers = servers.ToObservable().Concat(clientServer);
            observableServers.Subscribe(OnNext);
        }

        private  void OnNext(FileTcpServer server)
        {
            Console.WriteLine("empty server: EMPTY " + server.IsIdle() + "\\" + server.path.ToString());
            Console.WriteLine("Still " + pendingClients.Count + " pedning clinet");
            if(pendingClients.Count > 0)
            {
                var nextClient= this.GetNextPendingClient();
                Handle(nextClient);
            }
        }

        public async Task Handle(TcpClient client)
        {
           
            connectedClients.Add(client);
            var idleServer = servers.Find(x => x.IsIdle());
            if(idleServer != null)
            {
               // servers.Remove(idleServer);
                idleServer.Run(client,()=> OnNextEmptyServer(idleServer), usersDict);
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

        public TcpClient GetNextPendingClient()
        {
            var groupedClients = pendingClients.GroupBy(x => ((IPEndPoint)x.Client.RemoteEndPoint).Port);
           
            var client = pendingClients.FirstOrDefault();
            pendingClients.Remove(client);
            return client;
        }
    }
    
}
