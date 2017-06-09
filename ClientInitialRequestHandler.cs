using DropBoxLoadBalancer.Infrastructure.Infrastructure;
using DropBoxLoadBalancer.Infrastructure.Models;
using DropBoxLoadBalancer.Persistence;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using DropBoxLoadBalancer.Infrastructure;

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

        public Task Handle(TcpClient client, IPEndPoint address, List<Tuple<string,int, TcpClient>> usersDict)
        {
            return Task.Factory.StartNew(() =>
            {
                byte[] data = NetworkInfrastructure.GetNetworkDate(client);
                InitRequestModel request = Serializers.DeserializeObject<InitRequestModel>(data);
                usersDict.Add(new Tuple<string,int, TcpClient>(request.UserName,request.Port,client));
                var diff = dbHandle.GetDiff(request);
                diff.ForEach(x =>
                {
                    x.Files.ForEach(async y =>
                    {
                        var deserialized = Serializers.SerializeObject<FileModel>(FileModel.Create(x.SerwerName, y.ToString()));
                        await NetworkInfrastructure.SendFileAsync(address.Address, request.Port, deserialized);
                    });
                });
                client.Dispose();

            });
        }
    }
}
