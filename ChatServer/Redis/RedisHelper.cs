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
    public class RedisHelper
    {
        //recommended sharing this connector : Thread safe
        public ConnectionMultiplexer RedisConn { get; private set; }
        public string Ip { get; protected set; }
        public int Port { get; protected set; }

        protected CoreLogger logger = new ConsoleLogger();
        public string DbName { get; protected set; }

        protected IDatabase Database { get {
                return RedisConn.GetDatabase();
            } }

        protected RedisHelper(string _dbName, string _ip, int _port)
        {
            DbName = _dbName;
            Ip = _ip;
            Port = _port;
        }

        protected bool Connect()
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

        public async Task SetObjToString(string _key, object _obj, long _waitMilliSec = 0, IDatabase _db = GetDB())
        {
            var ser = JsonConvert.SerializeObject(_obj);
            if (_waitMilliSec == 0)
                await _db.StringSetAsync(_key, ser, TimeSpan.FromMilliseconds(_waitMilliSec));
            else
                await _db.StringSetAsync(_key, ser);
        }

        public async Task<T> GetObjFromString<T>(IDatabase _db, string _key) where T : new()
        {
            try
            {
                var ser = await _db.StringGetAsync(_key);
                if (ser.IsNullOrEmpty == false)
                {
                    return JsonConvert.DeserializeObject<T>(ser);
                }
            }
            catch (Exception e)
            {
                //logging?
            }
            return default(T);
        }
    }

}
