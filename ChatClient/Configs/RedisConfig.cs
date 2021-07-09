using CoreNet.Utils.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Configs
{
    public class RedisConfig : JsonicObj
    {
        public string Server { get; set; }
        public int Port { get; set; }

        public RedisConfig(JObject _jobj) : base(_jobj)
        {
        }
    }
}
