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
            int FILESENDPORT = 4999;
            Console.WriteLine("Server starting .... ");
            IConfiguration configuration = new Configruation();
            DbHandler dbHandler = new DbHandler(configuration);
            dbHandler.InitDb();

            LoadBalancerTcpServer server = new LoadBalancerTcpServer(new ClientConnectionHandler(),FILESENDPORT);
            InitalRequestListener initialRequestListener = new InitalRequestListener(new ClientInitialRequestHandler(), 3999);
            List<Tuple<string,int, TcpClient>> usersDict = new List<Tuple<string,int, TcpClient>>();


            initialRequestListener.RunAsync(usersDict);
            server.RunAsync(usersDict);
            Console.ReadKey();

        }
    }
}