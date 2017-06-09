using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DropBoxLoadBalancer.Persistence
{
    public class FileModel
    {
        public string FileName { get; set; }
        public byte [] Content { get; set; }
        public string UserName { get; set; }

        internal static FileModel Create(string serwerName, string fileName)
        {
            return (new FileModel()
            {
                Content = File.ReadAllBytes(@"C:\" + serwerName + "\\" + fileName),
                FileName = fileName
            });
        }
    }
}
