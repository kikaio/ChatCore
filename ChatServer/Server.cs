using ChatCore.Packets;
using ChatCore.Protocols;
using ChatServer.Configs;
using ChatServer.DataBases.Common;
using ChatServer.Redis;
using ChatServer.Sessions;
using ChatServer.Sessions.Events;
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

        private void SetSessionEvents(UserSession _s)
        {
            _s.Connected += async (_sender, _args) => {
                SessionMgr.Inst.AddSession(_s);
                var isSuccess = await RedisService.Auth.AddNewSessionInfo(name, _s.Token, UserSession.TokenTTL.Milliseconds);
                if (isSuccess == false)
                {
                    logger.WriteDebug("something is wrong... check redis server");
                    return;
                }
                //this server's sessio cnt increase.
                await RedisService.ChatServer.IncreaseSessionCnt(name);
            };

            //call when success authenticated
            _s.Authenticated += async (_sender, _args) => {
                //get Account id and to something for redis server
                var arg = _args as AuthenticateArgs;
                if (arg == default(AuthenticateArgs))
                    return;
                logger.WriteDebug($"[Sign in : {arg.NickName}] - token : {arg.Token}");
                await RedisService.Auth.AddTokenToAccount(arg.Token, arg.AId); 
            };

            _s.Disconnected += async (_sender, _args) => {
                //remove key and value about this session.
                await RedisService.Auth.RemoveTokenInfo(_s.Token);
                await RedisService.Auth.RemoveTokenFromAccount(_s.Token, _s.SessionId);
                //decreament session cnt in this server.
                await RedisService.ChatServer.DecreamentSessionCnt(name);
                logger.WriteDebug($"Session[{_s.SessionId}:{_s.Token}] DisConnected");
                _s.DoDispose(true);
            };
        }
        public override void ReadyToStart()
        {
            //config files init
            ConfigMgr.Init();

            name = "Chat_001";

            //overwirte if servername already exist in redis
            RedisService.Init(name, true);
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
                        SetSessionEvents(nSession);

                        nSession.OnConnected(this, null);
                        
                        while (isDown ==false && nSession.Sock.Sock.Connected)
                        {
                            var p = await nSession.OnRecvTAP();
                            //disconnected
                            if (p == default(Packet))
                            {
                                var args = new DisconnArgs
                                {
                                    Desc = "this session failed recv packet",
                                    DisconnDt = DateTime.UtcNow,
                                    SId = nSession.SessionId,
                                };
                                nSession.OnDisConnected(this, args);
                                break;
                            }
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
