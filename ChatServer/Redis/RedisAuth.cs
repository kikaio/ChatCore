using ChatServer.Configs;
using ChatServer.Redis.Data;
using ChatServer.Sessions;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Redis
{
    public class RedisAuth
    {
        /* Auth 관련 정보들
         * SessionToken
         * 
         */
        public string TokenKey { get; private set; } = "Auth:Token";
        public string ChatServerKey { get; private set; } = "Auth:Chat";
        public string AccountListKey { get; private set; } = "Auth:Account";

        public RedisConf Conf { get; private set; }
        public string DbName { get; private set; }
        protected RedisDB redis;
        private RedisConf conf;
        private CoreLogger logger = new ConsoleLogger();

        private TimeSpan keyLiveMilliTime = TimeSpan.FromSeconds(8 * 60 * 60);

        public RedisAuth(string _dbName) 
        {
            DbName = _dbName;
        }

        public void Init()
        {
            if (ConfigMgr.RedisServiceDict.ContainsKey(DbName) == false)
            {
                logger.Error($"Check Redis Config File or Data : {DbName}");
                return;
            }
            conf = ConfigMgr.RedisServiceDict[DbName];
            redis = RedisDB.Create(DbName, conf);
            if (redis.Connect() == false)
                logger.Error($"Redis connect failed, Check Redis device, is process on???");
            else
                logger.WriteDebug($"Redis connect successed : {DbName}");
        }

        public async Task<bool> AddNewSessionInfo(string _serverName, string _token, long _expireMilliSec)
        {
            if (await redis.Database.KeyExistsAsync($"{TokenKey}:{_token}"))
            {
                await RemoveTokenInfo(_token);
                return false;
            }
            await redis.SetStr($"{ChatServerKey}:{_token}", Server.Inst.name, _expireMilliSec);
            return true;
        }

        public async Task AddTokenToAccount(string _token, long _aid)
        {
            if (_aid < 1)
            {
                logger.Error($"Account Id[{_aid}] is invalid value");
                return;
            }
            await redis.Database.StringSetAsync($"{TokenKey}:{_token}", _aid.ToString(), UserSession.TokenTTL);
            await redis.Database.ListLeftPushAsync($"{AccountListKey}:{_aid.ToString()}", _token);
        }

        public async Task RemoveTokenFromAccount(string _token, long _aid)
        {
            if(_aid > 0)
                await redis.Database.ListRemoveAsync($"{AccountListKey}:{_aid.ToString()}", _token);
            await redis.Database.KeyDeleteAsync($"{TokenKey}:{_token}");
        }

        public async Task<bool> RemoveTokenInfo(string _token)
        {
            
            await redis.Database.KeyDeleteAsync($"{TokenKey}:{_token}");
            // remove this sessison where connected server name
            await redis.Database.KeyDeleteAsync($"{ChatServerKey}:{_token}");
            return true;
        }

        public async Task<long> GetAccounIdFromToken(string _token)
        {
            var ret = await redis.GetStr($"{TokenKey}:{_token}");
            long retVal = default(long);
            if (ret != "")
            {
                if (long.TryParse(ret, out retVal))
                    return retVal;
            }
            return retVal;
        }

        public async Task<List<string>> GetTokensFromAccountId(long _val)
        {
            var retList = new List<string>();
            var list = await redis.Database.ListRangeAsync($"{AccountListKey}:{_val.ToString()}");
            if (list.Length == 0)
                return retList;
            else
            {
                foreach (var redisVal in list)
                    retList.Add(redisVal.ToString());
                return retList;
            }
        }

        public async Task<bool> CheckTokenAndAccountId(string _token, long _accid)
        {
            var val = await redis.GetStr(_token);
            if(val == "")
                return false;
            return true;
        }


        public async Task<bool> CheckVerifiedToken(string _token, long _accid, bool _doKeyExtend = false)
        {
            var val = await redis.GetStr($"{TokenKey}:{_token}");
            if (val == "" || val != _accid.ToString())
                return false;

            if (_doKeyExtend)
            {
                await redis.SetExpiredKey($"{TokenKey}:{_token}", keyLiveMilliTime);
                await redis.SetExpiredKey($"{ChatServerKey}:{_accid}", keyLiveMilliTime);
            }

            return true;
        }

    }
}
