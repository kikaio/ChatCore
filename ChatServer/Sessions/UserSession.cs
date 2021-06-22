using ChatCore.Packets;
using CoreNet.Networking;
using CoreNet.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Sessions
{
    public class UserSession : CoreSession
    {
        private ESessionState curState = ESessionState.WELCOME;
        private Dictionary<ESessionState, SessionState> mDispatchDict = new Dictionary<ESessionState, SessionState>();

        public UserSession(long _sid, CoreSock _sock) : base(_sid, _sock)
        {
            Init();
        }

        private void Init()
        {
            mDispatchDict[ESessionState.WELCOME] = new Session_Welcome(this);
            mDispatchDict[ESessionState.DH_SWAP] = new Session_DhSwap(this);
            mDispatchDict[ESessionState.CHAT] = new Session_Chat(this);
        }

        public void SetState(ESessionState _state)
        {
            if (curState == _state)
                return;
            curState = _state;
        }

        public void Dispatch_Req(ChatPacket _cp)
        {
            mDispatchDict[curState].Dispatch_Req(_cp);
        }
        public void Dispatch_Ans(ChatPacket _cp)
        {
            mDispatchDict[curState].Dispatch_Ans(_cp);
        }
        public void Dispatch_Noti(ChatPacket _cp)
        {
            mDispatchDict[curState].Dispatch_Noti(_cp);
        }
        public void Dispatch_Test(ChatPacket _cp)
        {
            mDispatchDict[curState].Dispatch_Test(_cp);
        }
    }
}
