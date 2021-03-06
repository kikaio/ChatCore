using ChatClient.Configs;
using ChatCore.Enums;
using ChatCore.Packets;
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

namespace ChatClient.Sessions
{
    public enum ESessionState
    {
        TRY_HELLO,
        TRY_GET_DH,
        ABOUT_SIGN, //before sign in, sing up, sign out 
        SIGN_UP, //sign up
        SIGN_IN, //sign in
        CHATABLE, //after sign in
        SIGN_OUT, //change state to about isgn after sign out 
    }

    public interface IDispatch
    {
        void Dispatch_Req(ChatPacket _cp);
        void Dispatch_Ans(ChatPacket _cp);
        void Dispatch_Noti(ChatPacket _cp);
        void Dispatch_Test(ChatPacket _cp);
    }

    public class SessionState : IDispatch
    {
        public UserSession Session { get; private set; }
        protected CoreLogger logger = new ConsoleLogger();

        public SessionState(UserSession _s)
        {
            Session = _s;
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

    public class State_Try_Hello : SessionState
    {
        public State_Try_Hello(UserSession _s) : base(_s)
        {
        }

        public override void Dispatch_Ans(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ChatCore.Enums.ECONTENT.WELCOME:
                    {
                        WelcomeAns ans = new WelcomeAns(_cp);
                        ans.SerRead();
                        logger.WriteDebug($"Client welcomed!!, sessionId is {ans.sId}");
                        Session.SetSessionId(ans.sId);
                        Session.UpdateState(ESessionState.TRY_GET_DH);
                        Task.Factory.StartNew(async () =>
                        {
                            var req = new DH_Req();
                            req.SerWrite();
                            await Session.OnSendTAP(req);
                            logger.WriteDebug("send dh req");
                        });
                    }
                    break;
            }
        }
    }

    public class State_Try_Get_dh : SessionState
    {
        public State_Try_Get_dh(UserSession _s) : base(_s)
        {
        }

        public override void Dispatch_Ans(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ChatCore.Enums.ECONTENT.DH_KEY_SWAP:
                    {
                        DH_Ans ans = new DH_Ans(_cp);
                        ans.SerRead();
                        logger.WriteDebug("=======Before RSA Decrypt=========");
                        logger.WriteDebug($"recv dh key:{ans.dhKey}");
                        logger.WriteDebug($"recv dh iv:{ans.dhIV}");

                        ans.dhKey = CryptHelper.RsaDecryptWithBase64(ans.dhKey, ConfigMgr.ClientConfig.privateParam);
                        ans.dhIV = CryptHelper.RsaDecryptWithBase64(ans.dhIV, ConfigMgr.ClientConfig.privateParam);

                        logger.WriteDebug("=======After RSA Decrypt=========");
                        logger.WriteDebug($"recv dh key:{ans.dhKey}");
                        logger.WriteDebug($"recv dh iv:{ans.dhIV}");

                        byte[] bytesDHKey = Convert.FromBase64String(ans.dhKey);
                        byte[] bytesDHIV = Convert.FromBase64String(ans.dhIV);
                        Session.SetDhInfo(bytesDHKey, bytesDHIV);
                        Session.UpdateState(ESessionState.ABOUT_SIGN);
                        logger.WriteDebug("now client will send chat noti to server");
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class State_Chatable : SessionState
    {
        public State_Chatable(UserSession _s) : base(_s)
        {
        }

        public override void Dispatch_Ans(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ECONTENT.REQ_RESULT:
                    {
                        var ret = new Result_Ans(_cp);
                        ret.SerRead();
                        if (ret.IsSuccessed)
                        {   //sign out success
                            logger.WriteDebug("Client sign out complete");
                            Session.UpdateState(ESessionState.ABOUT_SIGN);
                        }
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
                case ECONTENT.CHAT:
                    {
                        ChatNoti noti = new ChatNoti(_cp);
                        noti.SerRead();
                        logger.WriteDebug($"[{noti.sId}]:{noti.msg}");
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class State_About_Sign : SessionState
    {
        public State_About_Sign(UserSession _s) : base(_s)
        {
        }
    }

    public class State_Sign_Up : SessionState
    {
        public State_Sign_Up(UserSession _s) : base(_s)
        {
        }

        public override void Dispatch_Ans(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ECONTENT.REQ_RESULT:
                    {
                        var ans = new Result_Ans(_cp);
                        ans.SerRead();
                        if (ans.IsSuccessed)
                            logger.WriteDebug("Sign up completed");
                        else
                            logger.WriteDebug("Sign up failed, maybe duplicated nickname");
                        Session.UpdateState(ESessionState.ABOUT_SIGN);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class State_Sign_In : SessionState
    {
        public State_Sign_In(UserSession _s) : base(_s)
        {
        }

        public override void Dispatch_Ans(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ECONTENT.REQ_RESULT:
                    {
                        var ans = new Result_Ans(_cp);
                        ans.SerRead();
                        if (ans.IsSuccessed)
                        {
                            logger.WriteDebug("Sign in completed");
                            Session.UpdateState(ESessionState.CHATABLE);
                        }
                        else
                        {
                            logger.WriteDebug("Sign In failed, Check NickName or pw");
                            Session.UpdateState(ESessionState.ABOUT_SIGN);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class State_Sign_Out : SessionState
    {
        public State_Sign_Out(UserSession _s) : base(_s)
        {
        }

        public override void Dispatch_Req(ChatPacket _cp)
        {
            switch (_cp.cType)
            {
                case ECONTENT.REQ_RESULT:
                    {
                        var ans = new Result_Ans(_cp);
                        ans.SerRead();
                        if (ans.IsSuccessed)
                        {
                            logger.WriteDebug("Sign out completed");
                            Session.UpdateState(ESessionState.ABOUT_SIGN);
                        }
                        else
                            logger.WriteDebug("Sign out failed");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
