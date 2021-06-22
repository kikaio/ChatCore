using ChatCore.Packets;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class Session_Chat : SessionState
    {
        public Session_Chat(UserSession _us) : base(_us)
        {
        }
    }
}
