using CoreNet.Utils.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Configs
{
    public class ServerConf : JsonicObj
    {
        public int Port { get; set; }
        public int Max_Thread_Cnt { get; set; }
        public string Category { get; set; }
        public string RSA_Private { get; set; }
        public string RSA_Public { get; set; }
        public string Dh_IV { get; set; }

        protected ServerConf(JObject _jobj) : base(_jobj)
        {
            GetJObjectFromProperties();
        }
    }
}
