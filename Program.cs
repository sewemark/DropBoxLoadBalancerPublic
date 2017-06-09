using DropBoxLoadBalancer.Infrastructure;
using DropBoxLoadBalancer.Persistence;
using System.Collections.Generic;
using System.Net.Sockets;
using System;

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
            List<Tuple<string, int, TcpClient>> usersDict = new List<Tuple<string, int, TcpClient>>();

            ClientConnectionHandler clientConnectionHandler = new ClientConnectionHandler(dbHandler, usersDict);
            ClientInitialRequestHandler clientInitRequestHandler = new ClientInitialRequestHandler();

            LoadBalancerTcpServer server = new LoadBalancerTcpServer(clientConnectionHandler, configuration.GetFileSendPort());
            InitalRequestListener initialRequestListener = new InitalRequestListener(clientInitRequestHandler, configuration.GetInitReqListenerPort());
            initialRequestListener.RunAsync(usersDict);
            server.RunAsync();
            Console.ReadKey();

        }
    }
}