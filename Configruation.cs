using System;
using System.Collections.Generic;
using System.Text;

namespace DropBoxLoadBalancer
{
    public interface IConfiguration
    {
        string GetDbFolder();
        int GetFileSendPort();
        int GetInitReqListenerPort();
    }

    public class Configruation : IConfiguration
    {
        public string GetDbFolder()
        {
            return @"C:\db\baza.xml";    

        }

        public int GetFileSendPort()
        {
            return 4999;
        }

        public int GetInitReqListenerPort()
        {
            return 3999;
        }
    }
}
