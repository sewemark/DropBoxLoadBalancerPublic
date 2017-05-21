using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DropBoxLoadBalancer.Infrastructure.Models
{
    public class InitRequestModel
    {
        public string[] Files { get; set; }
        public string UserName { get; set; }
        public int Port { get; set; }
    }
}
