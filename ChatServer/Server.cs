using ChatCore.Packets;
using ChatCore.Protocols;
using ChatServer.Configs;
using ChatServer.DataBases.Common;
using ChatServer.Sessions;
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

namespace ChatServer
{
    public class Server : CoreNetwork
    {
        public static Server Inst { get; } = new Server();

        public long ChatServerNo { get; protected set; }

        private Dictionary<string, Worker> wDict = new Dictionary<string, Worker>();
        private CoreTCP mListen;
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

        private void ReadyRedisService()
        {

        }

        public override void ReadyToStart()
        {
            //config files init
            ConfigMgr.Init();

            TranslateEx.Init();

            ep = new IPEndPoint(IPAddress.Any, port);
            mListen = new CoreTCP();
            mListen.Sock.Bind(ep);
            mListen.Sock.Listen(100);

            ReadyWorkers();
        }

        public override void Start()
        {
            Task.Factory.StartNew(async () =>
            {
                foreach (var w in wDict)
                {
                    logger.WriteDebug($"{w.Key} is start");
                    w.Value.WorkStart();
                }

                logger.WriteDebug("Accept start");
                while (isDown == false)
                {
                    Socket ns = mListen.Sock.Accept();
                    CoreTCP tcp = new CoreTCP(ns);
                    var newSId = SessionMgr.Inst.GetNextSessionId();
                    Task.Factory.StartNew(async () => {
                        var sid = newSId;
                        logger.WriteDebug($"new session accepted, id : {sid}");
                        UserSession nSession = new UserSession(newSId, tcp, this);
                        SessionMgr.Inst.AddSession(nSession);
                        while (isDown ==false && nSession.Sock.Sock.Connected)
                        {
                            var p = await nSession.OnRecvTAP();
                            if (p.GetHeader() == 0)
                            {
                                logger.WriteDebug($"Session {nSession.SessionId}'s hb update");
                                nSession.UpdateHeartBeat();
                            }
                            else
                                packageQ.Push(new Package(nSession, p));
                        }
                    });
                }
            });
        }

        public void BroadCastChatAllSessions(ChatPacket _cp)
        {
            logger.WriteDebug($"BroadCast Packet : [{_cp.pType}-{_cp.cType}]");
            Task.Factory.StartNew(async () => {
                foreach (var s in SessionMgr.Inst.ToSessonList())
                {
                    var us = s as UserSession;
                    if (us?.curState == ESessionState.CHAT)
                        await us.OnSendTAP(_cp);
                }
            });
        }

        protected override void Analizer_Ans(CoreSession _s, Packet _p)
        {
            UserSession s = _s as UserSession;
            ChatPacket cp = new ChatPacket(_p);
            s?.Dispatch_Ans(cp);

        }

        protected override void Analizer_Noti(CoreSession _s, Packet _p)
        {
            UserSession s = _s as UserSession;
            ChatPacket cp = new ChatPacket(_p);
            s?.Dispatch_Noti(cp);
        }

        protected override void Analizer_Req(CoreSession _s, Packet _p)
        {
            UserSession s = _s as UserSession;
            ChatPacket cp = new ChatPacket(_p);
            s?.Dispatch_Req(cp);
        }

        protected override void Analizer_Test(CoreSession _s, Packet _p)
        {
            UserSession s = _s as UserSession;
            ChatPacket cp = new ChatPacket(_p);
            s?.Dispatch_Test(cp);
        }
    }
}
