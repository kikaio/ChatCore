using ChatCore.Packets;
using ChatServer.Configs;
using ChatServer.DataBases.Common;
using ChatServer.Sessions.Events;
using CoreNet.Cryptor;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static ChatCore.Packets.ChatNoti;

namespace ChatServer.Sessions
{
    public enum ESessionState
    {
        WELCOME,
        DH_SWAP,
        ABOUT_SIGN,
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
                        HelloReq req = new HelloReq(_cp);
                        req.SerRead();
                        using (var csp = new RSACryptoServiceProvider())
                        {
                            csp.FromXmlString(req.publicRsaXml);
                            Session.rsaPublicParam = csp.ExportParameters(false);
                        }
                        logger.WriteDebug("hello req recved");
                        Task.Factory.StartNew(async () =>
                        { 
                            WelcomeAns ans = new WelcomeAns();
                            ans.sId = Session.SessionId;
                            ans.SerWrite();
                            Session.SetState(ESessionState.DH_SWAP);
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
                        logger.WriteDebug("recv dh key req packet");
                        Task.Factory.StartNew(async () => {
                            DH_Ans ans = new DH_Ans();
                            var dhKey = ConfigMgr.ServerConf.Dh_KEY;
                            var dhIv = ConfigMgr.ServerConf.Dh_IV;

                            ans.dhKey = ConfigMgr.ServerConf.Dh_KEY;
                            ans.dhIV = ConfigMgr.ServerConf.Dh_IV;
                            logger.WriteDebug($"======Before Encrypt DH=======");
                            logger.WriteDebug($"DH_KEY:{ans.dhKey}");
                            logger.WriteDebug($"DH_IV:{ans.dhIV}");
                            {
                                ans.dhKey = CryptHelper.RsaEncryptWithBase64(ans.dhKey, Session.rsaPublicParam);
                                ans.dhIV = CryptHelper.RsaEncryptWithBase64(ans.dhIV, Session.rsaPublicParam);
                            }
                            logger.WriteDebug($"=======After Encrypt DH========");
                            logger.WriteDebug($"DH_KEY:{ans.dhKey}");
                            logger.WriteDebug($"DH_IV:{ans.dhIV}");


                            ans.SerWrite();

                            Session.SetState(ESessionState.ABOUT_SIGN);
                            await Session.OnSendTAP(ans);
                            //Encrypt communication start
                            Session.SetDhInfo(Convert.FromBase64String(dhKey), Convert.FromBase64String(dhIv));
                        });
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class Session_AboutSign : SessionState
    {
        public Session_AboutSign(UserSession _us) : base(_us)
        {
        }

        public override void Dispatch_Req(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ChatCore.Enums.ECONTENT.SIGN_UP:
                    {
                        var req = new SignUp_Req(_cp);
                        req.SerRead();
                        Task.Run(async () => {
                            var ret = await Account.SignUp(req.NickName, req.Pw);
                            Result_Ans ans = new Result_Ans();
                            ans.IsSuccessed = ret;
                            ans.SerWrite();
                            await Session.OnSendTAP(ans);
                        });
                    }
                    break;
                case ChatCore.Enums.ECONTENT.SIGN_IN:
                    {
                        var req = new SignIn_Req(_cp);
                        req.SerRead();
                        Task.Run(async () => {
                            var ret = await Account.SignIn(req.NickName, req.Pw);
                            Result_Ans ans = new Result_Ans();
                            ans.IsSuccessed = ret != default(Account);
                            ans.SerWrite();
                            if (ans.IsSuccessed)
                            {
                                Session.SetState(ESessionState.CHAT);
                                var arg = new AuthenticateArgs();
                                arg.AId = ret.Id;
                                arg.NickName = ret.NickName;
                                arg.Token = Session.Token;
                                arg.AuthenticatedDt = DateTime.UtcNow;
                                Session.OnAuthenticated(Server.Inst, arg);
                            }
                            else
                                Session.SetState(ESessionState.ABOUT_SIGN);
                            await Session.OnSendTAP(ans);
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

        public override void Dispatch_Req(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ChatCore.Enums.ECONTENT.SIGN_OUT:
                    {
                        Task.Run(async () => {
                            //session logout logic
                            var ret = new Result_Ans();
                            ret.IsSuccessed = true;
                            await Session.OnSendTAP(ret);
                            logger.WriteDebug($"{Session.SessionId} is sign out, byebye");
                            Session.SetState(ESessionState.ABOUT_SIGN);
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Dispatch_Noti(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ChatCore.Enums.ECONTENT.CHAT:
                    {
                        ChatNoti recv = new ChatNoti(_cp);
                        recv.SerRead();
                        logger.WriteDebug($"[{recv.sId}]:{recv.msg}");
                        var noti = new ChatNoti();
                        noti.sId = recv.sId;
                        noti.msg = recv.msg;
                        noti.SerWrite();
                        Server.Inst.BroadCastChatAllSessions(noti);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
