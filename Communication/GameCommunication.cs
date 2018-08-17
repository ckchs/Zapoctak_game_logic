using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Source;
using Action = GameLogic.Source.Action;

namespace Communication
{
    public abstract class GameCommunication
    {
        protected PlayerTurn Player1 = null;
        protected PlayerTurn Player2 = null;
        protected Socket Socket;
        protected byte[] Data = new byte[2048];
        protected Map map;
        public delegate void ShowResults (TurnResults results);

        protected ShowResults Sr;

        public abstract void ThisPlayerTurn(PlayerTurn p);
    }
}
