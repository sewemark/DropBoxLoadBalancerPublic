using System.Collections.Generic;
using System.Linq;
using DropBoxLoadBalancer.Infrastructure.Models;
using DropBoxLoadBalancer.Infrastructure.Infrastructure;
using DropBoxLoadBalancer.Models;
using System.IO;
using System.Threading;

namespace DropBoxLoadBalancer.Persistence
{
    public class DbHandler
    {
        IConfiguration configuration;
        Mutex m;
        public DbHandler(IConfiguration _configuration)
        {
            configuration = _configuration;
            m = new Mutex(false, "AddingFileMutex");
        }

        public List<DbModel> GetDiff(InitRequestModel requestModel)
        {
            var result = Serializers.DeSerializeObjectToFile<Db>(configuration.GetDbFolder());

            var rr = from dbmodel in result.Entries
                     where dbmodel.Files.Except(requestModel.Files).Count() > 0
                     select dbmodel;
            return rr.ToList();
           
        }

        public void InitDb()
        {
            if (File.Exists(configuration.GetDbFolder()))
                return;

            Db db = new Db();
            Serializers.SerializeObjectToFile<Db>(db, configuration.GetDbFolder());
        }

        public void AddFile(FileModel recievedFile,string serverName)
        {
            
            m.WaitOne();
            var result = Serializers.DeSerializeObjectToFile<Db>(configuration.GetDbFolder());
            result.Entries.Add(new DbModel()
            {
                Files = new List<string>() { recievedFile.FileName.Substring(1) },
                SerwerName = serverName.Substring(3),
                UserName = recievedFile.UserName
            });
            Serializers.SerializeObjectToFile<Db>(result, configuration.GetDbFolder());
            m.ReleaseMutex();
        }
    }
}
