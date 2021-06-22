﻿using ChatCore.Packets;
using ChatServer.Configs;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatCore.Packets.ChatNoti;

namespace ChatServer.Sessions
{
    public enum ESessionState
    {
        WELCOME,
        DH_SWAP,
        CHAT,
    }


    public interface IState
    {
        void Dispatch_Ans(ChatPacket _cp);
        void Dispatch_Req(ChatPacket _cp);
        void Dispatch_Noti(ChatPacket _cp);
        void Dispatch_Test(ChatPacket _cp);
    }

    public class SessionState : IState
    {
        public UserSession Session { get; private set; }
        protected CoreLogger logger = new ConsoleLogger();

        public SessionState(UserSession _us)
        {
            Session = _us;
        }

        public virtual void Dispatch_Ans(ChatPacket _cp)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispatch_Noti(ChatPacket _cp)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispatch_Req(ChatPacket _cp)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispatch_Test(ChatPacket _cp)
        {
            throw new NotImplementedException();
        }
    }

    public class Session_Welcome : SessionState
    {
        public Session_Welcome(UserSession _us) : base(_us)
        {
        }

        public override void Dispatch_Req(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ChatCore.Enums.ECONTENT.HELLO:
                    {
                        Task.Factory.StartNew(async () =>
                        {
                            WelcomeAns ans = new WelcomeAns();
                            ans.sId = Session.SessionId;
                            ans.SerWrite();
                            await Session.OnSendTAP(ans);
                        });
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class Session_DhSwap : SessionState
    {
        public Session_DhSwap(UserSession _us) : base(_us)
        {
        }

        public override void Dispatch_Req(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ChatCore.Enums.ECONTENT.DH_KEY_SWAP:
                    {
                        Task.Factory.StartNew(async () => {
                            DH_Ans ans = new DH_Ans();
                            ans.dhKey = ConfigMgr.ServerConf.Dh_KEY;
                            ans.dhIV = ConfigMgr.ServerConf.Dh_IV;
                            ans.SerWrite();
                            await Session.OnSendTAP(ans);
                            //Encrypt communication start
                            Session.SetDhInfo(Convert.FromBase64String(ans.dhKey), Convert.FromBase64String(ans.dhIV));
                        });

                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class Session_Chat : SessionState
    {
        public Session_Chat(UserSession _us) : base(_us)
        {
        }
    }
}