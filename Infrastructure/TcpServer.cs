using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DropBoxLoadBalancer.Infrastructure
{
    public class TcpServer
    {
        private TcpListener tcpListenre;
        public TcpServer(int port)
        {
            tcpListenre = new TcpListener(IPAddress.Any, port);
            tcpListenre.Start();
        }
        
    }
}
