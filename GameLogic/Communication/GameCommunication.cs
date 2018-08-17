using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Source;
using Action = GameLogic.Source.Action;
using System.Net.Sockets;

namespace GameLogic.Communication
{
    public abstract class GameCommunication
    {
        protected PlayerTurn Player1 = null;
        protected PlayerTurn Player2 = null;
        protected Socket Socket;
        protected byte[] Data = new byte[16384];
        protected GameManager Gm;
        
        //This player done declaring turn
        public abstract void ThisPlayerTurn(PlayerTurn p);

        //Add first byte to data and send them
        protected void SendData(byte firstByte, byte[] rest)
        {
            try
            {
                var newData = new byte[rest.Length + 1];
                Array.Copy(rest, 0, newData, 1, rest.Length);
                newData[0] = firstByte;
                Socket.BeginSend(newData, 0, newData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (SocketException)
            {
                Gm.EndOfGame();
            }
        }
        /*
         * 0 - sending TurnResults
         * 1 - sending Map
         * 2 - sending DeclarePlayerTurn
         * 3 - sending message
        */

        protected void OnSend(IAsyncResult ar)
        {
            Socket.EndSend(ar);
        }

        //Send text message to opponent
        public abstract void SendMessage(string message);
    }
}
