using DropBoxLoadBalancer.Infrastructure.Infrastructure;
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
        public StringBuilder path = new StringBuilder(@"C:\server");
        List<TcpClient> connectedClients;
        private bool idle = true;
        private DbHandler dbHandler;
        public FileTcpServer(int _port, List<TcpClient> _connectedClients,DbHandler _dbHandler)
        {
            tcpListenre = new TcpListener(IPAddress.Any, _port);
            tcpListenre.Start();
            connectedClients = _connectedClients;
            path.Append(_port.ToString());
            dbHandler = _dbHandler;
            if (!Directory.Exists(path.ToString()))
            {
                Directory.CreateDirectory(path.ToString());
            }
        }

        public void Run(TcpClient client, Action callback, List<Tuple<string, int, TcpClient>> usersDict)
        {
            byte[] fileData;
            string userName = "";
            var task = new TaskFactory().StartNew(() =>
             {
                 idle = false;
                 fileData = GetNetworkDate(client);
                 FileModel recievedFile = Serializers.DeserializeObject<FileModel>(fileData);
                 userName = recievedFile.UserName;
                 File.WriteAllBytes(path.ToString() + "\\" + recievedFile.FileName, recievedFile.Content);
                 Console.WriteLine("Client readed");
                 dbHandler.AddFile(recievedFile, path.ToString());
                 return recievedFile;

             }).ContinueWith((x) =>
             {
                 callback();
                 idle = true;
                 var otherClientByUserName = usersDict.Where(z => z.Item1 == userName)
                 .ToList();
                 otherClientByUserName.ForEach(connectedClient =>
                 {
                     Console.WriteLine("Resending" + connectedClient.Item1);
                     TcpClient tempClient = new TcpClient();
                     tempClient.ConnectAsync("127.0.0.1", connectedClient.Item2 + 1);
                     byte[] recievedFile = Serializers.SerializeObject<FileModel>(x.Result);
                     tempClient.Client.Send(recievedFile);
                     tempClient.Dispose();
                 });

             });

        }
        public bool IsIdle()
        {
            return this.idle;
        }

        public byte[] GetNetworkDate(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            {
                byte[] data = new byte[1024];

                using (MemoryStream ms = new MemoryStream())
                {
                    int numOfBytes = 0;
                    while ((numOfBytes = stream.Read(data, 0, data.Length)) > 0)
                    {
                        ms.Write(data, 0, numOfBytes);
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}
