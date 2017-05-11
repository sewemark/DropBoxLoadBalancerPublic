using System;
using System.IO;
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
        public StringBuilder path = new StringBuilder(@"C:\server");
        private bool idle = true;
        public FileTcpServer(int _port)
        {
            tcpListenre = new TcpListener(IPAddress.Any, _port);
            tcpListenre.Start();
            path.Append(_port.ToString());
            if (!Directory.Exists(path.ToString()))
            {
                Directory.CreateDirectory(path.ToString());
            }
        }

        public async Task RunAsync(TcpClient client, Action callback)
        {
            var task = new TaskFactory().StartNew(() =>
            {
                idle = false;
                BinaryReader reader = new BinaryReader(client.GetStream(), Encoding.ASCII);
                byte[] fileData = null;
                fileData = reader.ReadBytes(1024);
                File.WriteAllBytes(path.ToString() + "\\" + "file.txt", fileData);
                Console.WriteLine("Client readed");
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }).ContinueWith((x)=>
            {
                callback();
                idle = true;
            });
           
        }
        public bool IsIdle()
        {
            return this.idle;
        }
    }
}
