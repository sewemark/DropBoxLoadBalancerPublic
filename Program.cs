using System;
using DropBoxLoadBalancer.Infrastructure;

namespace DropBoxLoadBalancer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server starting .... ");
            LoadBalancerTcpServer server = new LoadBalancerTcpServer(new ClientConnectionHandler(),4999);
            InitalRequestListener initialRequestListener = new InitalRequestListener(new ClientInitialRequestHandler(), 3999);
            server.RunAsync();
            initialRequestListener.RunAsync();
            Console.ReadKey();

        }
    }
}