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
        public string Ip { get; set; }
        public int Port { get; set; }
        public int Max_Thread_Cnt { get; set; }
        public string Category { get; set; }
        public string RSA_Private { get; set; }
        public string Dh_IV { get; set; }

        public Aes mSecret = Aes.Create();
        public string Dh_KEY;

        public RSAParameters rsaPrivateParam;
        public RSAParameters rsaPublicParam;
        public string rsaPublicXml;
        public ServerConf(JObject _jobj) : base(_jobj)
        {
            Init();
        }

        private void Init()
        {
            bool isNeedUpdate = false;
            if (RSA_Private == "")
            {
                isNeedUpdate = true;
                using (var csp = new RSACryptoServiceProvider())
                {
                    rsaPrivateParam = csp.ExportParameters(true);
                    rsaPublicParam = csp.ExportParameters(false);
                    using (var sw = new StringWriter())
                    {
                        RSA_Private = csp.ToXmlString(true);
                        rsaPublicXml = csp.ToXmlString(false);
                    }
                }
            }
            else
            {
                using (var csp =  new RSACryptoServiceProvider())
                {
                    csp.FromXmlString(RSA_Private);
                    rsaPrivateParam = csp.ExportParameters(true);
                    rsaPublicParam = csp.ExportParameters(false);
                    rsaPublicXml = csp.ToXmlString(false);
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
