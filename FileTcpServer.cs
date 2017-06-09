using DropBoxLoadBalancer.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DropBoxLoadBalancer.Infrastructure
{
    public class FileTcpServer
    {
        private TcpListener tcpListenre;
       
        List<TcpClient> connectedClients;
        private bool idle = true;
        private DbHandler dbHandler;
        private NetworkInfrastructure networkInfrastructure;
        public StringBuilder path = new StringBuilder(@"C:\server");
        public int port;
        public FileTcpServer(int _port, List<TcpClient> _connectedClients, NetworkInfrastructure _networkInfrastructure)
        {
            tcpListenre = new TcpListener(IPAddress.Any, _port);
            tcpListenre.Start();
            connectedClients = _connectedClients;
            networkInfrastructure = _networkInfrastructure;
            path.Append(_port.ToString());
            
            if (!Directory.Exists(path.ToString()))
            {
                Directory.CreateDirectory(path.ToString());
            }
        }

        public void Run(TcpClient client, Action callback, List<Tuple<string, int, TcpClient>> usersDict)
        {
            Thread.Sleep(5000);
            var task = new TaskFactory().StartNew( () =>
             {
                  
                 idle = false;
                 return networkInfrastructure.Save(client, path.ToString());

             }).ContinueWith((x) =>
             {
                 callback();
                 idle = true;

                 Console.WriteLine("Empting server " + port.ToString());
                 var otherClientByUserName = usersDict.Where(z => z.Item1 == x.Result.UserName)
                                             .ToList();
                 otherClientByUserName.ForEach(connectedClient =>
                 {
                     Console.WriteLine("Resending" + connectedClient.Item1);
                     networkInfrastructure.SendFile("127.0.0.1", connectedClient.Item2 + 1, x.Result);
                 });
               
             });
        }

        public bool IsIdle()
        {
            return this.idle;
        }
    }
}
