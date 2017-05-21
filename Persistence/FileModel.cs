using System;
using System.Collections.Generic;
using System.Text;

namespace DropBoxLoadBalancer.Persistence
{
    public class FileModel
    {
        public string FileName { get; set; }
        public byte [] Content { get; set; }
        public string UserName { get; set; }
    }
}
