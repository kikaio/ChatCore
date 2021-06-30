using ChatServer.Sessions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Sessions
{
    public partial class UserSession
    {
        #region Connected
        public delegate void ConnectedDelegate(Server _s, CoreArgs _e);
        public event ConnectedDelegate Connected;
        public void OnConnected(Server _s, CoreArgs _e)
        {
            Connected?.Invoke(_s, _e);
            logger.WriteDebugTrace();
        }
        #endregion

        #region Authenticated (or sign in)
        public delegate void AuthenticatedDelegate(Server _s, CoreArgs _e);
        public event AuthenticatedDelegate Authenticated;
        public void OnAuthenticated(Server _s, CoreArgs _e)
        {
            Authenticated?.Invoke(_s, _e);
            logger.WriteDebugTrace();
        }

        public delegate void SignOutDelegate(Server _s, CoreArgs _e);
        public event SignOutDelegate SignOuted;
        public void OnSignOut(Server _s, CoreArgs _e)
        {
            SignOuted?.Invoke(_s, _e);
            logger.WriteDebugTrace();
        }

        #endregion


        #region Dicsonnceted
        public delegate void DisconnectedDelegate(Server _sender, CoreArgs _e);
        public event DisconnectedDelegate Disconnected;
        public void OnDisConnected(Server _sender, CoreArgs _e)
        {
            Disconnected?.Invoke(_sender, _e);
            logger.WriteDebugTrace();
        }
        #endregion

    }
}
