using ChatCore.Enums;
using CoreNet.Networking;
using CoreNet.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatCore.Protocols
{
    public static class TranslateEx
    {
        private static bool isInit = false;
        public static void Init()
        {
            if (isInit)
                return;
            Translate.RegistCustom<ECONTENT>((NetStream _ns, object _val)=> {
                _ns.WriteUInt16((ushort)_val);
            }
            , (NetStream _ns)=> {
                var ret = default(ECONTENT);
                ret = (ECONTENT)_ns.ReadUInt16();
                return ret;
            });

            isInit = true;
        }
    }
}
