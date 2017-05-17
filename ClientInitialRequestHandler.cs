using DropBoxLoadBalancer.Infrastructure.Infrastructure;
using DropBoxLoadBalancer.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DropBoxLoadBalancer
{
    public interface IClientInitialRequestHandler
    {
        Task Handle(TcpClient client);
    }
    public class ClientInitialRequestHandler : IClientInitialRequestHandler
    {
        public ClientInitialRequestHandler()
        {

        }
        public Task Handle(TcpClient client)
        {
            return Task.Factory.StartNew(() =>
            {
                 byte[] data = GetNetworkDate(client);
               
                InitRequestModel request = Extensions.DeserializeObject<InitRequestModel>(data);
            });

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
