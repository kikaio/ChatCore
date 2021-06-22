using ChatServer.Configs;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreLogger logger = new ConsoleLogger();
            try
            {
                //config files init
                ConfigMgr.Init();

                Server.Inst.ReadyToStart();
                Server.Inst.Start();

                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    //todo : unhandle excepdtion catch
                    logger.WriteDebug("UnhandledException setting is fin");
                };

                while (Server.Inst.isDown == false)
                {
                    Thread.Sleep(3000);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                Console.ReadKey();
                throw;
            }

        }
    }
}
