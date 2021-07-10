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

        protected Dictionary<string, Action<string>> commandMap = new Dictionary<string, Action<string>>();
        public CmdStates(UserSession _s)
        {
            session = _s;
            InitCommand();
        }

        protected virtual void InitCommand()
        {

        }

        public void DoCmd(string _cmds)
        {
            if (string.IsNullOrEmpty(_cmds))
                return;
            //command 대분류는 공백으로 구분.
            int splitPivot = _cmds.IndexOf(" ");
            var cmdType = _cmds.Substring(0, splitPivot + 1);
            var semiStr = _cmds.Substring(splitPivot + 1);
            if (commandMap.ContainsKey(cmdType) == false)
                return;
            commandMap[cmdType](semiStr);
        }
    }

    public class Cmd_None : CmdStates
    {
        public Cmd_None(UserSession _s) : base(_s)
        {
        }

    }
    public class Cmd_BeforeConnect : CmdStates
    {
        private Dictionary<string, Action<string>> semiCmdMap = new Dictionary<string, Action<string>>();

        private string cmd_GetLobbyList = "GET_LOBBY_LIST";
        private string cmd_TryConn = "CONN";
        public Cmd_BeforeConnect(UserSession _s) : base(_s)
        {
        }

        protected override void InitCommand()
        {
            semiCmdMap[cmd_GetLobbyList] = GetLobbyList;
            semiCmdMap[cmd_TryConn] = TryConn;
        }

        private void GetLobbyList(string _semi)
        {
            
        }
        private void TryConn(string _semi)
        {

        }
    }
    public class Cmd_AboutSign : CmdStates
    {
        public Cmd_AboutSign(UserSession _s) : base(_s)
        {
        }

    }
    public class Cmd_Chat : CmdStates
    {
        public Cmd_Chat(UserSession _s) : base(_s)
        {
        }

    }
    public class Cmd_Disconnected : CmdStates
    {
        public Cmd_Disconnected(UserSession _s) : base(_s)
        {
        }

    }
    public class Cmd_TryRecnnect : CmdStates
    {
        public Cmd_TryRecnnect(UserSession _s) : base(_s)
        {
        }

    }
}
