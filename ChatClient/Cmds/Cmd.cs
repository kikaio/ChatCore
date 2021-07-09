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
    public class Cmd
    {
        private CoreLogger logger = new ConsoleLogger();
        private E_CMDSTATE curState = E_CMDSTATE.NONE;
        private Dictionary<E_CMDSTATE, CmdStates> stateDict = new Dictionary<E_CMDSTATE, CmdStates>();

        public Cmd(UserSession _s)
        {
            Init(_s);
        }
        private void Init(UserSession _s)
        {
            stateDict[E_CMDSTATE.NONE] = new Cmd_None(_s);
            stateDict[E_CMDSTATE.BEFORE_CONNECT] = new Cmd_BeforeConnect(_s);
            stateDict[E_CMDSTATE.ABOUT_SIGN] = new Cmd_AboutSign(_s);
            stateDict[E_CMDSTATE.CHAT] = new Cmd_Chat(_s);
            stateDict[E_CMDSTATE.DISCONNTECTED] = new Cmd_Disconnected(_s);
            stateDict[E_CMDSTATE.TRY_RECONNECT] = new Cmd_TryRecnnect(_s);

        }
        public void SetState(E_CMDSTATE _nextState)
        {
            if (curState == _nextState)
                return;
            logger.WriteDebug($"CmdState Change {curState} to {_nextState}");
            curState = _nextState;
        }

        public void WaitCmdInputs()
        {
            var cmds = Console.ReadLine();
            stateDict[curState].DoCmd(cmds);
        }
    }

}
