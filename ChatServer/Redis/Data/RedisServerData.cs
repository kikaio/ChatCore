using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Redis.Data
{
    public class RedisServerData : IRedisData
    {
        public string InstName { get; set; }
        public long SessionCnt { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public long InstId { get; set; }

        public void GetObjFromStr(string _str)
        {
            throw new NotImplementedException();
        }

        public string GetStrFromObj()
        {
            throw new NotImplementedException();
        }
    }
}
