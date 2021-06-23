using ChatCore.Packets;
using CoreNet.Networking;
using CoreNet.Sockets;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Sessions
{
    public partial class UserSession : CoreSession, IDispatch
    {
        public ESessionState curState { get; protected set; } = ESessionState.TRY_HELLO;
        private Dictionary<ESessionState, SessionState> stateDict = new Dictionary<ESessionState, SessionState>();
        private CoreLogger logger = new ConsoleLogger();

        public UserSession(long _sid, CoreSock _sock) : base(_sid, _sock)
        {
            Init();
        }

        private void Init()
        {
            stateDict[ESessionState.TRY_HELLO] = new State_Try_Hello(this);
            stateDict[ESessionState.TRY_GET_DH] = new State_Try_Get_dh(this);
            stateDict[ESessionState.CHATABLE] = new State_Chatable(this);
        }

        public void UpdateState(ESessionState _s)
        {
            if (curState == _s)
                return;
            logger.WriteDebug($"session state update from {curState.ToString()} to {_s.ToString()}");
            curState = _s;
        }

        public void SendChat(string _msg)
        {
            Task.Factory.StartNew(async () =>
            {
                ChatNoti noti = new ChatNoti();
                noti.sId = SessionId;
                noti.msg = _msg;
                await OnSendTAP(noti);
            });
        }

        public void Dispatch_Ans(ChatPacket _cp)
        {
            stateDict[curState].Dispatch_Ans(_cp);
        }

        public void Dispatch_Noti(ChatPacket _cp)
        {
            stateDict[curState].Dispatch_Noti(_cp);
        }

        public void Dispatch_Req(ChatPacket _cp)
        {
            stateDict[curState].Dispatch_Req(_cp);
        }

        public void Dispatch_Test(ChatPacket _cp)
        {
            stateDict[curState].Dispatch_Test(_cp);
        }

    }
}
