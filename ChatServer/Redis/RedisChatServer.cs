using ChatServer.Configs;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Redis
{
    public class RedisChatServer 
    {
        public string Instkey { get; protected set; } = "ChatServer:Inst";
        public string DataKey { get; protected set; } = "ChatServer:Data";

        private RedisDB redis;
        private CoreLogger logger = new ConsoleLogger();
        public RedisConf conf { get; private set; }
        public string DbName { get; private set; }

        public RedisChatServer(string _DBName)
        {
            DbName = _DBName;
        }

        public void Init(string _instName)
        {
            if (ConfigMgr.RedisServiceDict.ContainsKey(DbName) == false)
            {
                logger.Error($"Check Redis Config File or Data : {DbName}");
                return;
            }
            conf = ConfigMgr.RedisServiceDict[DbName];
            redis = RedisDB.Create(DbName, conf);
            if (redis.Connect() == false)
                logger.Error($"Redis connect failed, Check Redis device, is process on???");
            else
                logger.WriteDebug($"Redis connect successed : {DbName}");

            Task.Run(async()=> await RegistServerInst(_instName));
        }


        /// <summary>
        /// Server Instance data regist to redis db
        /// </summary>
        /// <param name="_name"></param>
        public async Task<bool> RegistServerInst(string _name)
        {
            if (await GetServerinst(_name) != "")
            {
                logger.Error($"Server Instance named {_name} is Exist!!, check redis or instance");
                return false;
            }
            await SetServerInst(_name);
            await SetServerData(_name);
            return true;
        }

        public async Task DeleteServerInst(string _name)
        {

        }

        private async Task<string> GetServerinst(string _name)
        {
            var ret = await redis.GetStr($"{Instkey}:{_name}");
            return ret;
        }

        private async Task SetServerInst(string _name)
        {
            //vlaue is sessionCnt
            await redis.SetStr($"{Instkey}:{_name}", 0.ToString());
        }

        private async Task<Tuple<string, long>> GetServerData(string _name)
        {
            var str = await redis.GetStr($"{DataKey}:{_name}");
            if (str == "")
                return new Tuple<string, long>("", 0);
            else
            {
                var jObj = new JObject(JsonConvert.DeserializeObject(str));
                string ip = jObj.Value<string>("Ip");
                long port = jObj.Value<long>("Port");
                return new Tuple<string, long>(ip, port);
            }
        }

        private async Task SetServerData(string _name)
        {
            var sConf = ConfigMgr.ServerConf;
            var data = new { Ip = sConf.Ip, Port = sConf.Port };
            var dataStr = JsonConvert.SerializeObject(data);
            await redis.SetStr($"{DataKey}:{_name}", dataStr);
        }


        public async Task IncreaseSessionCnt(string _name)
        {
            await redis.Database.StringIncrementAsync($"{Instkey}:{_name}");
        }

        public async Task DecreamentSessionCnt(string _name)
        {
            await redis.Database.StringDecrementAsync($"{Instkey}:{_name}");
        }


    }
}
