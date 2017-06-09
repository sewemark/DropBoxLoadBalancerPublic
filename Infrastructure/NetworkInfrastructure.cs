using DropBoxLoadBalancer.Infrastructure.Infrastructure;
using DropBoxLoadBalancer.Persistence;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DropBoxLoadBalancer.Infrastructure
{
    public class NetworkInfrastructure
    {
        private DbHandler dbHandler;

        public NetworkInfrastructure(DbHandler _dbHandler)
        {
            dbHandler = _dbHandler;

        }

        public static byte[] GetNetworkDate(TcpClient client)
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

        public static async Task SendFileAsync(IPAddress address , int port,byte[] data)
        {
            TcpClient clientToResend = new TcpClient();
            await clientToResend.ConnectAsync(address,port);
            clientToResend.Client.Send(data);
            Console.WriteLine("Sending to client" + address.ToString());
            clientToResend.Dispose();
        }

        public async Task SendFile(string address, int port, FileModel data)
        {
            TcpClient tempClient = new TcpClient();
            tempClient.ConnectAsync(address, port);
            byte[] recievedFile = Serializers.SerializeObject<FileModel>(data);
            tempClient.Client.Send(recievedFile);
            tempClient.Dispose();
        }

        public FileModel Save(TcpClient client, string path)
        {
            byte [] fileData = NetworkInfrastructure.GetNetworkDate(client);
            FileModel recievedFile = Serializers.DeserializeObject<FileModel>(fileData);
            File.WriteAllBytes(path.ToString() + "\\" + recievedFile.FileName, recievedFile.Content);
            Console.WriteLine("Client readed");
           // dbHandler.AddFile(recievedFile, path.ToString());
            return recievedFile;
        }
    }
}
