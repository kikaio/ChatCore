using ChatServer.Configs;
using ChatServer.Redis.Data;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Redis
{
    //각 Redis 연결 별로 공통 필요 정보
    /*
     * Ip
     * Port
     * DbName
     * connectionMultiplexer
     */
    public class RedisDB
    {
        //recommended sharing this connector : Thread safe
        public ConnectionMultiplexer RedisConn { get; private set; }
        public string Ip { get; protected set; }
        public int Port { get; protected set; }

        protected CoreLogger logger = new ConsoleLogger();
        public string DbName { get; protected set; }

        public IDatabase Database { get {
                return RedisConn.GetDatabase();
            } }

        public static RedisDB Create(string _dbName, RedisConf _conf)
        {
            if (string.IsNullOrWhiteSpace(_conf.Server) || _conf.Port == default(int))
                return default(RedisDB);
            var ret = new RedisDB(_dbName, _conf.Server, _conf.Port);
            return ret;
        }

        protected RedisDB(string _dbName, string _ip, int _port)
        {
            DbName = _dbName;
            Ip = _ip;
            Port = _port;
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

        public async Task SetStr(string _key, string _str, long _waitMilliSec = 0)
        {
            if (_waitMilliSec != 0)
                await Database.StringSetAsync(_key, _str, TimeSpan.FromMilliseconds(_waitMilliSec));
            else
                await Database.StringSetAsync(_key, _str);
        }

        public async Task RegistInRank(string _rankKey, string _name, long _val)
        {
            await Database.SortedSetAddAsync(_rankKey, _name, _val, CommandFlags.FireAndForget);
        }

        public async Task<string> GetStr(string _key)
        {
            var ret = await Database.StringGetAsync(_key);
            if (ret.IsNullOrEmpty)
                return "";
            return ret;
        }

        //저장하는 대상 IRedisData 로 강제.
        public async Task SetObjToString(string _key, IRedisData _obj, long _expireMilliSec = 0)
        {
            var str = _obj.GetStrFromObj();
            if (_expireMilliSec != 0)
                await Database.StringSetAsync(_key, str, TimeSpan.FromMilliseconds(_expireMilliSec));
            else
                await Database.StringSetAsync(_key, str);
        }

        //읽어오는 대상 IRedisData 로 강제.
        public async Task<T> GetObjFromString<T>(IDatabase _db, string _key) where T : IRedisData, new()
        {
            try
            {
                var ret = default(T);
                ret = new T();
                var str = await _db.StringGetAsync(_key);
                if (string.IsNullOrWhiteSpace(str) == false)
                    ret.GetObjFromStr(str);
                return ret;
            }
            catch (Exception e)
            {
                //logging?
                logger.Error(e.ToString());
            }
            return default(T);
        }

        public async Task SetExpiredKey(string _key, TimeSpan _keyLiveTime)
        {
            await Database.KeyExpireAsync(_key, _keyLiveTime, CommandFlags.FireAndForget);
        }

        public async Task IncreaseCnt(string _key)
        {
            await Database.StringIncrementAsync(_key);
        }
    }
}
