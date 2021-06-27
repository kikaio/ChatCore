using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Redis.Data
{
    public interface IRedisData
    {
        string GetStrFromObj();
        void GetObjFromStr(string _str);
    }
}
