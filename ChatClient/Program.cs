using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreLogger logger = new ConsoleLogger();
            try
            {
                using (var c = Client.Inst)
                {
                    c.ReadyToStart();
                    c.Start();

                    while (Client.Inst.isDown == false)
                    {
                        Thread.Sleep(3000);
                    }
                    Console.WriteLine("Client is down, press any key");
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
