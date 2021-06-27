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
        public static Dictionary<string, DBConf> DbConfDict { get; private set; } = new Dictionary<string, DBConf>();
        public static Dictionary<string, RedisConf> RedisServiceDict { get; private set; } = new Dictionary<string, RedisConf>();

        public static void Init()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                var serverConfPath = appSettings.Get("ServerConfig");
                var dbConfPath = appSettings.Get("DBConfig");
                var redisConfPath = appSettings.Get("RedisConfig");
                ReadyServerConfig(serverConfPath);
                ReadyDbConfig(dbConfPath);
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void ReadyServerConfig(string _confPath)
        {
            var sconfStr = "";
            using (var sr = new StreamReader(_confPath, Encoding.UTF8))
            {
                sconfStr = sr.ReadToEnd();
            }
            JObject serverConfJson = JObject.Parse(sconfStr);
            ServerConf = new ServerConf(serverConfJson);
        }

        private static void ReadyDbConfig(string _confPath)
        {
            var confStr = "";
            using (var sr = new StreamReader(_confPath, Encoding.UTF8))
            {
                confStr = sr.ReadToEnd();
            }
            JObject confObjDict = JObject.Parse(confStr);
            foreach (var jc in confObjDict)
            {
                var confToken = jc.Value as JObject;
                if (confToken == default(JObject))
                {
                    Console.WriteLine($"{jc.Key} is has not jobj");
                    continue;
                }
                var conf = new DBConf(confToken);
                DbConfDict[jc.Key] = conf;
            }
        }

        private static void ReadyRedisService(string _confPath)
        {
            var confStr = "";
            using (var fs = new StreamReader(_confPath, Encoding.UTF8))
            {
                confStr = fs.ReadToEnd();
            }

            JObject confDict = JObject.Parse(confStr);
            foreach (var jc in confDict)
            {
                Console.WriteLine($"{jc.Key} service config check");
                var jobj = jc.Value as JObject;
                if (jobj == default(JObject))
                {
                    continue;
                }
                var conf = new RedisConf(jobj);
                RedisServiceDict[jc.Key] = conf;
            }
        }


    }
}
