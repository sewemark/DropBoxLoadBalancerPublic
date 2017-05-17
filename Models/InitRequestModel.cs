using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DropBoxLoadBalancer.Infrastructure.Models
{
    [XmlRoot("InitRequestModels")]
    public class InitRequestModel
    {
        [XmlElement("Files")]
        public string[] Files { get; set; }
        [XmlElement("UserName")]
        public string UserName { get; set; }
    }
}
