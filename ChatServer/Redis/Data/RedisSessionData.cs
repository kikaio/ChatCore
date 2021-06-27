using ChatServer.Redis.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Redis
{
    /// <summary>
    /// Redis 에 저장되는 session 관련 Obj
    /// </summary>
    public class RedisSessionData : IRedisData
    {
        public string Token { get; set; }
        public long AccountId { get; set; }
        public string ChatServerName { get; set; }

        public string GetStrFromObj()
        {
            var ret = JsonConvert.SerializeObject(this);
            return ret;
        }

        public void GetObjFromStr(string _str)
        {
            var obj = JsonConvert.DeserializeObject(_str) as RedisSessionData;
            //동일 property에 한해서 값 복사.
            foreach (var pInfo in GetType().GetProperties())
            {
                pInfo.SetValue(this, pInfo.GetValue(obj));
            }
        }
    }
}
