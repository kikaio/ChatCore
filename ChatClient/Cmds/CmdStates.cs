using ChatClient.Sessions;
using CoreNet.Utils;
using CoreNet.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Cmds
{
    public enum E_CMDSTATE
    {
        NONE,
        BEFORE_CONNECT,
        ABOUT_SIGN,
        CHAT,
        DISCONNTECTED,
        TRY_RECONNECT,
    }


    public class CmdStates
    {
        protected Cmd Cmd;
        protected CoreLogger logger = new ConsoleLogger();
        protected UserSession session;
        public CmdStates(UserSession _s)
        {
            session = _s;
        }
        public virtual void DoCmd(string _cmds) { }
    }

    public class Cmd_None : CmdStates
    {
        public Cmd_None(UserSession _s) : base(_s)
        {
        }

        public override void DoCmd(string _cmds)
        {
        }
    }
    public class Cmd_BeforeConnect : CmdStates
    {
        public Cmd_BeforeConnect(UserSession _s) : base(_s)
        {
        }

        public override void DoCmd(string _cmds)
        {
        }
    }
    public class Cmd_AboutSign : CmdStates
    {
        public Cmd_AboutSign(UserSession _s) : base(_s)
        {
        }

        public override void DoCmd(string _cmds)
        {
        }
    }
    public class Cmd_Chat : CmdStates
    {
        public Cmd_Chat(UserSession _s) : base(_s)
        {
        }

        public override void DoCmd(string _cmds)
        {
        }
    }
    public class Cmd_Disconnected : CmdStates
    {
        public Cmd_Disconnected(UserSession _s) : base(_s)
        {
        }

        public override void DoCmd(string _cmds)
        {
        }
    }
    public class Cmd_TryRecnnect : CmdStates
    {
        public Cmd_TryRecnnect(UserSession _s) : base(_s)
        {
        }

        public override void DoCmd(string _cmds)
        {
        }
    }
}
