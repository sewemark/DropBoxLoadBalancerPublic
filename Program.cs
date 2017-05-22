using System;
using DropBoxLoadBalancer.Infrastructure;
using DropBoxLoadBalancer.Models;
using DropBoxLoadBalancer.Infrastructure.Infrastructure;
using DropBoxLoadBalancer.Persistence;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;

namespace DropBoxLoadBalancer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server starting .... ");
            IConfiguration configuration = new Configruation();
            DbHandler dbHandler = new DbHandler(configuration);
            dbHandler.InitDb();
            ClientConnectionHandler clientConnectionHandler= new ClientConnectionHandler(dbHandler);
            ClientInitialRequestHandler clientInitRequestHandler = new ClientInitialRequestHandler();
            LoadBalancerTcpServer server = new LoadBalancerTcpServer(clientConnectionHandler, configuration.GetFileSendPort());
            InitalRequestListener initialRequestListener = new InitalRequestListener(clientInitRequestHandler, configuration.GetInitReqListenerPort());
            List<Tuple<string,int, TcpClient>> usersDict = new List<Tuple<string,int, TcpClient>>();
            initialRequestListener.RunAsync(usersDict);
            server.RunAsync(usersDict);
            Console.ReadKey();

        }
    }
}