using ChatCore.Protocols;
using CoreNet.Jobs;
using CoreNet.Networking;
using CoreNet.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Server : CoreNetwork
    {
        public static Server Inst { get; } = new Server();

        private Dictionary<string, Worker> wDict = new Dictionary<string, Worker>();

        private void ReadyWorkers()
        {
            wDict["pkg"] = new Worker();
            wDict["pkg"].PushJob(new JobInfinity(() =>
            {
                if (isDown)
                    return;
                packageQ.Swap();
                while (true)
                {
                    var pkg = packageQ.pop();
                    if (pkg == default(Package))
                        break;
                    PackageDispatcher(pkg);
                }
            }));
        }

        public override void ReadyToStart()
        {
            TranslateEx.Init();
            ReadyWorkers();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Ans(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Noti(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Req(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Test(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }
    }
}
