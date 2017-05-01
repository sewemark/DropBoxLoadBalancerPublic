using System;
using DropBoxLoadBalancer.Infrastructure;

namespace DropBoxLoadBalancer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server starting .... ");
            LoadBalancerTcpServer server = new LoadBalancerTcpServer(4999,new ClientConnectionHandler());
            server.RunAsync();
            Console.ReadKey();

        }
    }
}