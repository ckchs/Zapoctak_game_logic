using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Source;

namespace Communication
{
    public class GameHostCommunication : GameCommunication
    {
        
        public GameHostCommunication(String IPAddress, String Port, bool Host, ShowResults sr, Map map)
        {
            Sr = sr;
            this.map = map;
            var socketCon = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketCon.Bind(new IPEndPoint(System.Net.IPAddress.Parse(IPAddress), Int32.Parse(Port)));

            socketCon.Listen(1);
            Socket = socketCon.Accept();
            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnReceiveHost), Socket);
        }

        private void OnReceiveHost(IAsyncResult ar)
        {
            Socket.EndReceive(ar);
            var data = Data;
            Data = new byte[2048];
            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnReceiveHost), null);
            if (Player2!=null)
            {
                throw new SystemException();
            }
            Player2 = PlayerTurn.ParseFromByte(data);
            CheckAndCalculateTurn();
        }

        private void OnSendHost(IAsyncResult ar)
        {
            Socket.EndSend(ar);
        }

        private void CheckAndCalculateTurn()
        {
            if (Player1 != null && Player2 != null)
            {
                var turnResults = TurnLogic.ResolveTurn(Player1, Player2, map);
                Player1 = null;
                Player2 = null;
                Sr(turnResults);
                var data = turnResults.ParseToByte();
                Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSendHost), null);
            }
        }
        public override void ThisPlayerTurn(PlayerTurn p)
        {
            Player1 = p;
            CheckAndCalculateTurn();
        }

    }
}
