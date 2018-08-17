using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GameLogic.Source;

namespace Communication
{
    public class GameClientCommunication : GameCommunication
    {
        private enum CommunicationState
        {
            
        }

        public GameClientCommunication(String IPAddress, String Port, ShowResults sr,Map map)
        {
            this.map = map;
            Sr = sr;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(System.Net.IPAddress.Parse(IPAddress), Int32.Parse(Port));
            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnReceiveClient), Socket);

        }


        private void OnReceiveClient(IAsyncResult ar)
        {
            Socket.EndReceive(ar);
            var data = Data;
            Data=new byte[2048];
            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, new AsyncCallback(OnReceiveClient), null);
            var tr = TurnResults.ParseFromByte(data);
            Sr(tr);

        }

        private void OnSendClient(IAsyncResult ar)
        {
            Socket.EndSend(ar);
        }

        public override void ThisPlayerTurn(PlayerTurn p)
        {
            var data= p.ParseToByte();
            Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSendClient), null);
        }
    }
}
