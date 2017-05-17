using System;
using System.Collections.Generic;
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
        List<TcpClient> connectedClients;
        private bool idle = true;
        public FileTcpServer(int _port, List<TcpClient> _connectedClients)
        {
            tcpListenre = new TcpListener(IPAddress.Any, _port);
            tcpListenre.Start();
            connectedClients = _connectedClients;
            path.Append(_port.ToString());
            if (!Directory.Exists(path.ToString()))
            {
                Directory.CreateDirectory(path.ToString());
            }
        }

        public async Task RunAsync(TcpClient client, Action callback)
        {
            byte[] fileData = new byte[0];
               var task = new TaskFactory().StartNew(() =>
            {
                idle = false;
                BinaryReader reader = new BinaryReader(client.GetStream(), Encoding.ASCII);
                fileData = null;
                fileData = reader.ReadBytes(1024);
                File.WriteAllBytes(path.ToString() + "\\" + "file.txt", fileData);
                Console.WriteLine("Client readed");
                
            }).ContinueWith((x)=>
            {
                callback();
                idle = true;
                connectedClients.ForEach(async connectedClient =>
                {
                    Console.WriteLine("Resending");
                    TcpClient tempClient = new TcpClient();
                    await tempClient.ConnectAsync("127.0.0.1", 4998);
                    BinaryWriter bw = new BinaryWriter(tempClient.GetStream(), System.Text.Encoding.ASCII, false);
                    FileStream fs = new FileStream(@"C:\PwJs\python.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    byte[] data = new byte[1024];
                    fs.Read(data, 0, 1024);
                    bw.Write(data);
                    bw.Dispose();
                    fs.Dispose();
                    client.Dispose();
                    // connectedClient.Client.SendTo(fileData, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4998));
                });

            });
            
        }
        public bool IsIdle()
        {
            return this.idle;
        }
    }
}
