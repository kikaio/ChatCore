using CoreNet.Utils.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Configs
{
    public class RedisConf : JsonicObj
    {
        public string Server { get; protected set; }
        public int Port { get; protected set; }

        public RedisConf(JObject _jobj) : base(_jobj)
        {
        }
    }
}
