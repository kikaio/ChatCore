using ChatClient.Configs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Redis
{
    //todo : Server 프로젝트에 있는 RedisChatServer와 동일하니 합친 후 Common으로 옮길것.
    public class RedisChatServer
    {
        public string Instkey { get; protected set; } = "ChatServer:Inst";
        public string DataKey { get; protected set; } = "ChatServer:Data";

        private RedisDB redis;

        public RedisChatServer(string _dbName) 
        {
            Init(_dbName);
        }

        private void Init(string _dbName)
        {
            var conf = ConfigMgr.RedisDbDict[_dbName];
            redis = RedisDB.Create(_dbName, conf);
        }

        public async Task<List<Tuple<string, long>>> GetLobbyServerList()
        {
            var ret = new List<Tuple<string, long>>();
            var list = await redis.Database.SortedSetRangeByScoreWithScoresAsync(Instkey);
            foreach(var ele in list)
            {
                var s = new Tuple<string, long>(ele.Element, (long)ele.Score);
                ret.Add(s);
            }
            return ret;
        }

        public async Task<Tuple<string, long>> GetFirstServer()
        {
            //session cnt 별 ranking?
            var min = await redis.Database.SortedSetRangeByScoreWithScoresAsync(Instkey, 0);
            if(min == null)
                return null;
            return new Tuple<string, long>(min.First().Element, (long)min.First().Score);
        }

        private async Task<Tuple<string, long>> GetServerData(string _name)
        {
            var str = await redis.Database.StringGetAsync($"{DataKey}:{_name}");
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

    }
}
