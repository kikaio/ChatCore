using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Redis
{
    public class RedisAuth : RedisHelper
    {
        /* Auth 관련 정보들
         * SessionToken
         * 
         */
        public string SessionTokenKey { get; private set; } = "Auth:Token";

        public RedisAuth(string _ip, int _port) 
            : base("Auth", _ip, _port)
        {
        }

        public void Init()
        {
            Connect();
        }


    }
}
