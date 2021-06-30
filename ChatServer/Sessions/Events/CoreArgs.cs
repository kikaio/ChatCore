using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Sessions.Events
{
    public class CoreArgs : EventArgs
    {
    }

    public class DisconnArgs : CoreArgs
    {
        public long SId { get; set; }
        public string Desc { get; set; }
        public DateTime DisconnDt { get; set; }
    }

    public class AuthenticateArgs : CoreArgs
    {
        public long AId { get; set; }
        public string NickName { get; set; }
        public DateTime AuthenticatedDt { get; set; }
        public string Token { get; set; }
    }
}
