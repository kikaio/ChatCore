using ChatClient.Configs;
using ChatClient.Redis;
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
        protected Dictionary<string, Action<string>> commandMap = new Dictionary<string, Action<string>>();
        protected UserSession session { get { return Client.mSession; } }
        public CmdStates()
        {
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

        public void RegistCmd(string _command, Action<string> _act)
        {
            commandMap[_command] = _act;
        }
    }

    public class Cmd_None : CmdStates
    {
        public Cmd_None() : base()
        {
        }

    }
    public class Cmd_BeforeConnect : CmdStates
    {
        private string cmd_GetChatList = "GET_CHAT_LIST";
        private string cmd_TryConn = "CONN";

        private RedisChatServer redisChatServer;

        public Cmd_BeforeConnect() : base()
        {
        }

        protected override void InitCommand()
        {
            commandMap[cmd_GetChatList] = GetChatList;
            commandMap[cmd_TryConn] = TryConn;
        }

        private void GetChatList(string _semi)
        {
            Task.Run(async () => {
                var list = await RedisService.ChatServer.GetLobbyServerList();
                foreach (var ele in list)
                {
                    logger.WriteDebug($"{ele.Item1}'s session cnt {ele.Item2}");
                }
            });
        }
        private void TryConn(string _semi)
        {
            string[] addr = _semi.Split(':');
            var host = "";
            var port = 0;
            if (addr.Length != 2)
            {
                host = ConfigMgr.ClientConfig.Server;
                port = ConfigMgr.ClientConfig.Port;
            }
            else
            {
                host = addr[0];
                port = int.Parse(addr[1]);
            }
            Task.Run(async () => {
            });
        }
    }
    public class Cmd_AboutSign : CmdStates
    {
        public Cmd_AboutSign() : base()
        {
        }

    }
    public class Cmd_Chat : CmdStates
    {
        public Cmd_Chat() : base()
        {
        }

    }
    public class Cmd_Disconnected : CmdStates
    {
        public Cmd_Disconnected() : base()
        {
        }

    }
    public class Cmd_TryRecnnect : CmdStates
    {
        public Cmd_TryRecnnect() : base()
        {
        }

    }
}
