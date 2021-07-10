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
    public class RedisLoadBalance : RedisDB
    {
        public string Instkey { get; protected set; } = "ChatServer:Inst";
        public string DataKey { get; protected set; } = "ChatServer:Data";
        public string ChatKey { get; protected set; } = "ChatServers";


        public RedisLoadBalance(string _dbName, string _ip, int _port) : base(_dbName, _ip, _port)
        {
        }

        public List<string> GetLobbyServerList()
        {
            var ret = new List<string>();
            var chatList = Database.ListRange(ChatKey, 0, -1, StackExchange.Redis.CommandFlags.FireAndForget);
            foreach (var info in chatList)
                ret.Add(info.ToString());
            return ret;
        }
        private async Task<Tuple<string, long>> GetServerData(string _name)
        {
            var str = await Database.StringGetAsync($"{DataKey}:{_name}");
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
