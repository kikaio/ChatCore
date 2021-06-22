using ChatCore.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Sessions
{
    public enum ESessionState
    {
        TRY_HELLO,
        TRY_GET_DH,
        CHATABLE,
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

                    break;
            }
        }
    }

    public class State_Try_Get_dh : SessionState
    {
        public State_Try_Get_dh(UserSession _s) : base(_s)
        {
        }
    }

    public class State_Chatable : SessionState
    {
        public State_Chatable(UserSession _s) : base(_s)
        {
        }
    }

}
