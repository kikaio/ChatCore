using ChatClient.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Redis
{
    public static class RedisService
    {
        public static RedisChatServer ChatServer { get; private set; }
        private static string redisAboutChatServer = "LOADBALANCE";

        public static void Init()
        {
            ChatServer = new RedisChatServer(redisAboutChatServer);
        }
    }
}
