using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Source;

namespace GameLogic.Communication
{
    public class GameHost : GameCommunication
    {

        public GameHost(String ipaddress, String port, GameManager gm)
        {
            Gm = gm;
            var socketCon = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketCon.Bind(new IPEndPoint(System.Net.IPAddress.Parse(ipaddress), Int32.Parse(port)));

            socketCon.Listen(1);
            Gm.ShowDebugMessage("Host waiting for client connected");
            Socket = socketCon.Accept();
            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnReceiveHost), Socket);
            Gm.ShowDebugMessage("Client connected");
        }

        //Recieving information from client
        private void OnReceiveHost(IAsyncResult ar)
        {
            try
            {
                Socket.EndReceive(ar);
                var data = Data;
                Data = new byte[16384];
                Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnReceiveHost), null);

                var newData = new byte[data.Length - 1];
                Array.Copy(data, 1, newData, 0, data.Length - 1);
                switch (data[0])
                {
                    case 2:
                        Player2 = Serializer.Deserialize(newData) as PlayerTurn;
                        Gm.ActualizeMap(Player2);
                        Gm.ActualizeCharacters();
                        CheckAndCalculateTurn();
                        break;
                    case 3:
                        var message = Encoding.UTF8.GetString(newData);
                        Gm.ShowMessage(message);
                        break;
                    default:
                        throw new SystemException("Got bad first byte");
                }
            }
            catch (SocketException)
            {
                Gm.EndOfGame();
            }
        }
        
        //if have both player declaration calculate turn show them and send results to client
        private void CheckAndCalculateTurn()
        {
            if (Player1 == null || Player2 == null) return;

            Gm.ShowMessage("Calculating turn");
            var turnResults = TurnLogic.ResolveTurn(Player1, Player2, Gm.Map);
            Player1 = null;
            Player2 = null;
            SendData(1, Serializer.Serialize(Gm.Map));
            SendData(0, Serializer.Serialize(turnResults));
            Gm.ShowResultsFromGc(turnResults);
        }

        //host is done declaring turn
        public override void ThisPlayerTurn(PlayerTurn p)
        {
            Player1 = p;
            CheckAndCalculateTurn();
        }

        //Send text message to opponent
        public override void SendMessage(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            SendData(3,data);
        }
    }
}
