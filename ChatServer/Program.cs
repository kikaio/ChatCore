using ChatServer.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //config files init
            ConfigMgr.Init();

            Server.Inst.ReadyToStart();
            Server.Inst.Start();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                //todo : unhandle excepdtion catch
            };

            while (Server.Inst.isDown == false)
            {

            }
        }
    }
}
