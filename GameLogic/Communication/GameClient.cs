using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GameLogic.Source;

namespace GameLogic.Communication
{
    public class GameClient : GameCommunication
    {

        public GameClient(String ipaddress, String port, GameManager gm)
        {
            Gm = gm;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(System.Net.IPAddress.Parse(ipaddress), Int32.Parse(port));
            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnReceiveClient), Socket);
            Gm.ShowDebugMessage("Client connected");
        }

        //Recieving information from host
        private void OnReceiveClient(IAsyncResult ar)
        {
            try
            {
                Socket.EndReceive(ar);
                var data = Data;
                Data = new byte[16384];
                Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnReceiveClient), null);

                var newData = new byte[data.Length - 1];
                Array.Copy(data, 1, newData, 0, data.Length - 1);
                switch (data[0])
                {
                    case 0:
                        var results = Serializer.Deserialize(newData) as TurnResults;
                        Gm.ShowResultsFromGc(results);
                        break;
                    case 1:
                        var map = Serializer.Deserialize(newData) as Map;
                        Gm.Map = map;
                        Gm.ActualizeCharacters();
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

        //client is done declaring turn
        public override void ThisPlayerTurn(PlayerTurn p)
        {
            var data = Serializer.Serialize(p);
            SendData(2,data);
        }

        //Send text message to opponent
        public override void SendMessage(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            SendData(3, data);
        }
    }
}
