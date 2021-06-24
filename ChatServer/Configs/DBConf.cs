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
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Db_Name { get; set; }

        public bool IsNeedCreateFile = false;
        public DBConf(JObject _jobj) : base(_jobj)
        {
            Init();
        }
        
        private void Init()
        {
            bool IsNeedCreateFile = false;

            if (Ip == "")
            {
                IsNeedCreateFile = true;
                Ip = "127.0.0.1";
            }

            if (Port == 0)
            {
                IsNeedCreateFile = true;
                Port = 3307;
            }

            if (Db_Name == "")
            {
                IsNeedCreateFile = true;
                Db_Name = "TestDB";
            }
        }
    }
}
