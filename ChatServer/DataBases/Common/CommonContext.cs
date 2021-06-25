using ChatServer.Configs;
using CoreNet.DB;
using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.DataBases.Common
{
    //[DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class CommonContext : CoreDbContext
    {
        private const string commonDbConfName = "COMMON_DB";

        public CommonContext()
            : this(ConfigMgr.DbConfDict[commonDbConfName]?.GetConnStr())
        {
            Console.WriteLine($"Db Conn Str : {ConfigMgr.DbConfDict[commonDbConfName]?.GetConnStr()}");
        }

        protected CommonContext(string _connStr) : base(_connStr)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}
