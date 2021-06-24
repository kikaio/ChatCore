using CoreNet.Utils.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Configs
{
    public class ClientConfig : JsonicObj
    {
        internal ClientConfig(JObject _jobj) : base(_jobj)
        {
            Init();
        }

        public RSAParameters privateParam;
        public RSAParameters publicParam; 

        public string Server { get; private set; }
        public int Port { get; private set; }
        public string Rsa_Private { get; private set; }

        public string rsaPublicXml;

        public void Init()
        {
            bool isNeedUpdate = false;
            if (Rsa_Private == "")
            {
                using (var csp = new RSACryptoServiceProvider())
                {
                    privateParam =  csp.ExportParameters(true);
                    privateParam =  csp.ExportParameters(false);
                    Rsa_Private = csp.ToXmlString(true);
                    rsaPublicXml = csp.ToXmlString(false);
                }
            }
            else
            {
                using (var csp = new RSACryptoServiceProvider())
                {
                    csp.FromXmlString(Rsa_Private);
                    privateParam = csp.ExportParameters(true);
                    publicParam = csp.ExportParameters(false);
                    rsaPublicXml = csp.ToXmlString(false);
                }
            }

            if (isNeedUpdate)
            {
                var curConfig = GetJObjectFromProperties();
                var appSettings = ConfigurationManager.AppSettings;
                var path = appSettings.Get("ClientConfig");
                using (var fs = new  FileStream(path, FileMode.OpenOrCreate))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(curConfig.ToString());
                    }
                }
                //file write
            }
        }
    }
}
