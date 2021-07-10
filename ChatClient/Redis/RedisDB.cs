using ChatClient.Configs;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Redis
{
    public class RedisDB
    {
        public ConnectionMultiplexer RedisConn { get; private set; }
        public string Ip { get; protected set; }
        public int Port { get; protected set; }

        protected CoreLogger logger = new ConsoleLogger();
        public string DbName { get; protected set; }

        public IDatabase Database
        {
            get
            {
                return RedisConn.GetDatabase();
            }
        }

        protected RedisDB(string _dbName, string _ip, int _port)
        {
            DbName = _dbName;
            Ip = _ip;
            Port = _port;
        }

        public static RedisDB Create(string _dbName, RedisConfig _conf)
        {
            if (string.IsNullOrEmpty(_dbName))
                return null;
            var ret = new RedisDB(_dbName, _conf.Server, _conf.Port);
            return ret;
        }

        public bool Connect()
        {
            RedisConn = ConnectionMultiplexer.Connect($"{Ip}:{Port}");
            if (RedisConn.IsConnected == false)
            {
                logger.WriteDebug($"Failed connect to {Ip}:{Port}");
                return false;
            }
            logger.WriteDebug($"Redis connect to {Ip}:{Port}");
            return true;
        }
    }
}
