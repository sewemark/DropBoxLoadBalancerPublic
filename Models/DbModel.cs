using System.Collections.Generic;

namespace DropBoxLoadBalancer.Models
{
    public class Db
    {
        public Db()
        {
            Entries = new List<DbModel>();
        }
        public List<DbModel> Entries { get; set; }
    }
        
    public class DbModel
    {
        public string UserName { get; set; }
        public string SerwerName { get; set; }
        public List<string> Files { get; set; }
    }
}
