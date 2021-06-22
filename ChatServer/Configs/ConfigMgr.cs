using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Configs
{
    public static class ConfigMgr
    {
        public static ServerConf ServerConf { get; private set; }

        public static void Init()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                var serverConfPath = appSettings.Get("ServerConfig");
                var sconfStr = "";
                using (var sr = new StreamReader(serverConfPath, Encoding.UTF8))
                {
                    sconfStr = sr.ReadToEnd();
                }
                JObject serverConfJson = JObject.Parse(sconfStr);
                ServerConf = new ServerConf(serverConfJson);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
