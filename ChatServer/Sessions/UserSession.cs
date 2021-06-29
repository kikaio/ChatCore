using ChatCore.Packets;
using CoreNet.Cryptor;
using CoreNet.Networking;
using CoreNet.Sockets;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Sessions
{
    public partial class UserSession : CoreSession
    {
        public ESessionState curState { get; private set; } = ESessionState.WELCOME;
        private Dictionary<ESessionState, SessionState> mDispatchDict = new Dictionary<ESessionState, SessionState>();
        private CoreLogger logger = new ConsoleLogger();
        internal Server server;

        internal RSAParameters rsaPublicParam;

        public string Token { get; private set; }
        private string TokenXorKey { get; } = "simpleisbest";

        public static TimeSpan TokenTTL { get; } = TimeSpan.FromMinutes(10);

        public UserSession(long _sid, CoreSock _sock, Server _server) : base(_sid, _sock)
        {
            server = _server;
            Init();
        }

        private string GenSessionToken()
        {
            string saltVal = "Hmmteresting";
            string simpleXoredToken = $"{SessionId}_{saltVal}_{server.name}";
            var bytes = Encoding.UTF8.GetBytes(TokenXorKey);
            var xoredBytes = CryptHelper.XorEncrypt(Encoding.UTF8.GetBytes(simpleXoredToken), bytes);
            return Encoding.UTF8.GetString(xoredBytes);
        }

        private void Init()
        {
            Token = GenSessionToken();
            mDispatchDict[ESessionState.WELCOME] = new Session_Welcome(this);
            mDispatchDict[ESessionState.DH_SWAP] = new Session_DhSwap(this);
            mDispatchDict[ESessionState.ABOUT_SIGN] = new Session_AboutSign(this);
            mDispatchDict[ESessionState.CHAT] = new Session_Chat(this);
        }

        public void SetState(ESessionState _state)
        {
            if (curState == _state)
                return;
            logger.WriteDebug($"state from {curState} to {_state}");
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
