using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Configs
{
    public static class ConfigMgr
    {
        public static ClientConfig ClientConfig { get; private set; }
        public static Dictionary<string, RedisConfig> RedisDbDict = new Dictionary<string, RedisConfig>();
        public static void Init()
        {
            var appSettings = ConfigurationManager.AppSettings;
            var clientConfPath = appSettings.Get("ClientConfig");
            var redisConfPath = appSettings.Get("RedisConfig");

            ReadClientConf(clientConfPath);
            ReadRedisConf(redisConfPath);
        }
        private static void ReadClientConf(string _path)
        {
            if (string.IsNullOrEmpty(_path))
            {
                //todo : logging
                return;
            }
            using (var fs = new FileStream(_path, FileMode.OpenOrCreate))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var confStr = sr.ReadToEnd();
                    var confJson = JObject.Parse(confStr);
                    ClientConfig = new ClientConfig(confJson);
                }
            }

        }

        private static void ReadRedisConf(string _path)
        {
            if (string.IsNullOrEmpty(_path))
            {
                //todo : logging
                return;
            }
            using (var fs = new FileStream(_path, FileMode.OpenOrCreate))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var confStr = sr.ReadToEnd();
                    var confJson = JObject.Parse(confStr);
                    foreach (var pair in confJson)
                    {
                        RedisDbDict[pair.Key] = new RedisConfig(pair.Value as JObject);
                    }
                }
            }
        }
    }

}
