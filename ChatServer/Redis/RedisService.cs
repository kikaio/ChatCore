using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Redis
{
    public static class RedisService
    {
        public static RedisAuth Auth { get; private set; }
        public static RedisChatServer ChatServer { get; private set; }

        public static void Init(string _instName, bool isRedisOverwrite = false)
        {
            Auth = new RedisAuth("AUTH");
            Auth.Init();

            ChatServer = new RedisChatServer("CHATSERVER");
            ChatServer.Init(_instName, isRedisOverwrite);
        }
    }
}
