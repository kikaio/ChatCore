using CoreNet.Utils.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Configs
{
    public class DBConf : JsonicObj
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Uid { get; set; }
        public string Password { get; set; }
        public int ConnectionTimeout { get; set; }


        public bool IsNeedCreateFile = false;
        public DBConf(JObject _jobj) : base(_jobj)
        {
            Init();
        }
        
        private void Init()
        {
            bool IsNeedCreateFile = false;

            if (Server == "")
            {
                IsNeedCreateFile = true;
                Server = "127.0.0.1";
            }

            if (Port == 0)
            {
                IsNeedCreateFile = true;
                Port = 3306;
            }

            if (Database == "")
            {
                IsNeedCreateFile = true;
                Database = "test_db";
            }
        }

        public string GetConnStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"");
            foreach (var pInfo in this.GetType().GetProperties())
            {
                if (pInfo.Name == "ConnectionTimeout")
                    sb.Append($"Connection Timeout={pInfo.GetValue(this)};");
                else
                    sb.Append($"{pInfo.Name}={pInfo.GetValue(this)};");
            }
            return sb.ToString();
        }
    }
}
