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

        public static void Init()
        {
            var appSettings = ConfigurationManager.AppSettings;
            var confPath = appSettings.Get("ClientConfig");
            using (var fs = new FileStream(confPath, FileMode.OpenOrCreate))
            {
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var confStr = sr.ReadToEnd();
                    var confJson = JObject.Parse(confStr);
                    ClientConfig = new ClientConfig(confJson);
                }
            }
        }
    }
}
