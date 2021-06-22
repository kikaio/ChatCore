using CoreNet.Utils.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        public Aes mSecret = Aes.Create();
        public string Dh_KEY;

        public ServerConf(JObject _jobj) : base(_jobj)
        {
            Init();
        }

        private void Init()
        {
            bool isNeedUpdate = false;
            if (RSA_Private == "" || RSA_Public == "")
            {
                isNeedUpdate = true;

                var csp = new RSACryptoServiceProvider();
                var privateParam = csp.ExportParameters(true);
                var publicParam = csp.ExportParameters(false);

                using (var sw = new StringWriter())
                {
                    var xs = new XmlSerializer(typeof(RSAParameters));
                    xs.Serialize(sw, publicParam);
                    RSA_Public = sw.ToString();
                }
                using (var sw = new StringWriter())
                {
                    var xs = new XmlSerializer(typeof(RSAParameters));
                    xs.Serialize(sw, privateParam);
                    RSA_Private = sw.ToString();
                }
            }

            if (Port == 0)
            {
                isNeedUpdate = true;
                Port = 30000; // default port
            }

            if (Dh_IV == "")
            {
                isNeedUpdate = true;
                //to do : Create new dh IV
                mSecret.GenerateIV();
                Dh_IV = Convert.ToBase64String(mSecret.IV);
            }
            // generate key whenever start server
            mSecret.GenerateKey();
            Dh_KEY = Convert.ToBase64String(mSecret.Key);

            if (isNeedUpdate)
            {
                var newServerConf = GetJObjectFromProperties();
                //todo : WriteFile
            }
        }

    }
}
