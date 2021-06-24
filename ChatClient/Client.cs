using ChatClient.Configs;
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
    public class Client : CoreNetwork, IDisposable
    {
        public static Client Inst { get; } = new Client();

        private UserSession mSession = default(UserSession);
        private Dictionary<string, Worker> wDict = new Dictionary<string, Worker>();

        private bool isDisposed = false;


        public override void ReadyToStart()
        {
            ConfigMgr.Init();
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
                    packageQ.Swap();
                    while (true)
                    {
                        var pkg = packageQ.pop();
                        if (pkg == default(Package))
                            break;
                        PackageDispatcher(pkg);
                    }
                }
            }));

            wDict["cmd"] = new Worker();
            wDict["cmd"].PushJob(new JobOnce(DateTime.MinValue, async () =>
            {
                while (isDown == false && mSession.Sock.Sock.Connected)
                {
                    var input = Console.ReadLine();
                    if (mSession.curState != ESessionState.CHATABLE)
                    {
                        logger.WriteDebug($"[sesstion state]{mSession.curState}");
                    }
                    else
                    {
                        ChatNoti noti = new ChatNoti();
                        noti.sId = mSession.SessionId;
                        noti.msg = input;
                        noti.SerWrite();
                        await mSession.OnSendTAP(noti);
                        logger.WriteDebug("sended chat noti");
                    }
                }
            }));

           
        }

        public override void Start()
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

            Task.Run(async () => {
                try
                {
                    logger.WriteDebug("start packet recv from server");
                    while (isDown == false && mSession.Sock.Sock.Connected)
                    {
                        var p = await mSession.OnRecvTAP();
                        if (p == default(ChatPacket))
                        {
                            logger.Error("Session is closed?");
                            break;
                        }
                        if (p.GetHeader() == 0)
                        {
                            logger.Error("Client recv 0, Something is wrong");
                        }
                        else
                            packageQ.Push(new Package(mSession, p));
                    }
                    logger.WriteDebug($"{mSession.SessionId} finished packet recv");
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw;
                }
            });

            Task.Factory.StartNew(async () =>
            {
                while (mSession.curState == ESessionState.TRY_HELLO)
                {
                    HelloReq req = new HelloReq();
                    req.publicRsaXml = ConfigMgr.ClientConfig.rsaPublicXml;
                    req.SerWrite();
                    await mSession.OnSendTAP(req);
                    logger.WriteDebug("send hello req");
                    await Task.Delay(3 * 1000);
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

        public void Dispose()
        {
            if (isDisposed)
                return;
            isDisposed = true;
            foreach (var w in wDict)
            {
                logger.WriteDebug($"{w.Key} is Fin");
                w.Value.WorkFinish();
            }
        }
    }
}
