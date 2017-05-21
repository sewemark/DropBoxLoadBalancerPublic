using System;
using System.Collections.Generic;
using System.Text;

namespace DropBoxLoadBalancer
{
    public interface IConfiguration
    {
        string GetDbFolder();
    }

    public class Configruation : IConfiguration
    {
        public string GetDbFolder()
        {
            return @"C:\db\baza.xml";    

        }
    }
}
