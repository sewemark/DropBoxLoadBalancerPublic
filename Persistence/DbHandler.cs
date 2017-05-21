using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using DropBoxLoadBalancer.Infrastructure.Models;
using DropBoxLoadBalancer.Infrastructure.Infrastructure;
using DropBoxLoadBalancer.Models;
using System.IO;

namespace DropBoxLoadBalancer.Persistence
{
    public class DbHandler
    {
        IConfiguration configuration;

        public DbHandler(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public List<DbModel> GetDiff(InitRequestModel requestModel)
        {
            XElement xelement = XElement.Load(configuration.GetDbFolder());
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
    }
}
