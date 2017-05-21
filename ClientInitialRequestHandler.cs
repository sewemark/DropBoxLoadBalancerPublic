using DropBoxLoadBalancer.Infrastructure.Infrastructure;
using DropBoxLoadBalancer.Infrastructure.Models;
using DropBoxLoadBalancer.Persistence;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace DropBoxLoadBalancer
{
    public interface IClientInitialRequestHandler
    {
        Task Handle(TcpClient client, IPEndPoint address, List<Tuple<string,int, TcpClient>> usersDict);
    }
    public class ClientInitialRequestHandler : IClientInitialRequestHandler
    {
        DbHandler dbHandle;

        public ClientInitialRequestHandler()
        {
            dbHandle = new DbHandler(new Configruation());
        }
        

        public void SendFile(TcpClient client, string filePath)
        {
            BinaryWriter bw = new BinaryWriter(client.GetStream(), System.Text.Encoding.ASCII, false);
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] data = new byte[1024];
            fs.Read(data, 0, 1024);
            bw.Write(data);
            bw.Dispose();
            fs.Dispose();
          
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

        public Task Handle(TcpClient client, IPEndPoint address, List<Tuple<string,int, TcpClient>> usersDict)
        {
            return Task.Factory.StartNew(() =>
            {
                byte[] data = GetNetworkDate(client);

                InitRequestModel request = Serializers.DeserializeObject<InitRequestModel>(data);
                usersDict.Add(new Tuple<string,int, TcpClient>(request.UserName,request.Port,client));
                var diff = dbHandle.GetDiff(request);
                //TODOZamienic na taska
                diff.ForEach(async x =>
                {
                  
                    Console.WriteLine("Resending ...");
                    if (x.Files.Count == 0)
                    {
                        TcpClient clientToResend = new TcpClient();
                        var deserialized = Serializers.SerializeObject<FileModel>
                         (null);
                        await clientToResend.ConnectAsync(address.Address, request.Port);
                        clientToResend.Client.Send(deserialized);

                    }
                    else
                    {
                        x.Files.ForEach(async y =>
                        {
                            TcpClient clientToResend = new TcpClient();
                            var deserialized = Serializers.SerializeObject<FileModel>
                           (new FileModel()
                           {
                               Content = File.ReadAllBytes(@"C:\" + x.SerwerName + "\\" + y.ToString()),
                               FileName = y.ToString()
                           });
                            await clientToResend.ConnectAsync(address.Address, request.Port);
                            clientToResend.Client.Send(deserialized);
                            Console.WriteLine("Sending to client" + address.Address.ToString());
                            clientToResend.Dispose();
                        });
                    }
                    client.Dispose();

                });

            });
        }
    }
}
