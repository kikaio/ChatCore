using ChatClient.Sessions;
using ChatCore.Packets;
using ChatCore.Protocols;
using CoreNet.Jobs;
using CoreNet.Networking;
using CoreNet.Protocols;
using CoreNet.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class Client : CoreNetwork
    {
        public static Client Inst { get; } = new Client();

        private UserSession mSession = default(UserSession);
        private Dictionary<string, Worker> wDict = new Dictionary<string, Worker>();

        public override void ReadyToStart()
        {
            TranslateEx.Init();
            wDict["hb"] = new Worker("hb");
            long hbDelta = TimeSpan.FromMilliseconds(CoreSession.hbDelayMilliSec * 0.75f).Ticks;
            wDict["hb"].PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, hbDelta, () => {
                if (isDown)
                    return;
                Task.Factory.StartNew(async () =>
                {
                    Packet hb = new Packet(0);
                    hb.UpdateHeader();
                    await mSession.OnSendTAP(hb);
                    logger.WriteDebug("send hb");
                });
            }));
            wDict["pkg"] = new Worker();
            wDict["pkg"].PushJob(new JobOnce(DateTime.MinValue, () => {
                while (isDown == false && mSession.Sock.Sock.Connected)
                {
                    while (true)
                    {
                        var pkg = packageQ.pop();
                        if (pkg == default(Package))
                            break;
                        PackageDispatcher(pkg);
                    }
                }
            }));


        }

        public override void Start()
        {
            Task.Factory.StartNew(async () =>
            {
                CoreTCP tcp = new CoreTCP();
                ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30000);
                tcp.Sock.Connect(ep);
                mSession = new UserSession(-1, tcp);
                logger.WriteDebug("Client connect to server,");
                foreach (var w in wDict)
                {
                    logger.WriteDebug($"{w.Key} is start");
                    w.Value.WorkStart();
                }
            });
        }

        protected override void Analizer_Ans(CoreSession _s, Packet _p)
        {
            var s = _s as UserSession;
            ChatPacket cp = new ChatPacket(_p);
            s?.Dispatch_Ans(cp);
        }

        protected override void Analizer_Noti(CoreSession _s, Packet _p)
        {
            var s = _s as UserSession;
            ChatPacket cp = new ChatPacket(_p);
            s?.Dispatch_Noti(cp);
        }

        protected override void Analizer_Req(CoreSession _s, Packet _p)
        {
            var s = _s as UserSession;
            ChatPacket cp = new ChatPacket(_p);
            s?.Dispatch_Req(cp);
        }

        protected override void Analizer_Test(CoreSession _s, Packet _p)
        {
            var s = _s as UserSession;
            ChatPacket cp = new ChatPacket(_p);
            s?.Dispatch_Test(cp);
        }
    }
}
