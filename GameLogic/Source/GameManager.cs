using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Communication;

namespace GameLogic.Source
{
    public class GameManager
    {
        public static readonly bool Debug = true;
        public Map Map;
        
        //Gui methods
        public Action<TurnResults> ShowResults;
        public Action<string> ShowMessage;
        private Action<List<Character>> _actualizeCharacters;


        private GameCommunication _gc;
        public StatesOfGame StateOfGame;
        public enum StatesOfGame
        {
            ShowingResults, WaitingForOpponent, PlayerDeclaringTurn,GameEnded
        }

        public GameManager(List<Character> chars, int size, Action<TurnResults> sr, Action<string> sm, Action<List<Character>> actualizeCharacters, bool host,string ipAdress, string port)
        {
            Map=new Map(chars,size);
            ShowResults = sr;
            ShowMessage = sm;
            _actualizeCharacters = actualizeCharacters;
            StateOfGame = StatesOfGame.PlayerDeclaringTurn;
            if (host)
            {
                _gc=new GameHost(ipAdress,port,this);
            }
            else
            {
                _gc=new GameClient(ipAdress, port, this);
            }
        }

        //Called from everywhere to show debug message
        public void ShowDebugMessage(string msg)
        {
            if (Debug)
            {
                ShowMessage.Invoke(msg);
            }
        }

        //called from GUI
        public void DeclarePlayerTurn(PlayerTurn pt)
        {
            if (StateOfGame==StatesOfGame.GameEnded)
            {
                return;
            }
            if (StateOfGame != StatesOfGame.PlayerDeclaringTurn)
            {
                throw new ApplicationException("Wrong flow");
            }
            StateOfGame = StatesOfGame.WaitingForOpponent;
            ShowDebugMessage("Sending turn to gamecommunication");
            ShowDebugMessage("StatesOfGame.WaitingForOpponent");
            _gc.ThisPlayerTurn(pt);
            
            
        }

        //called from GUI
        public void DoneShowingResults()
        {
            if (StateOfGame == StatesOfGame.GameEnded)
            {
                return;
            }
            if (StateOfGame != StatesOfGame.ShowingResults)
            {
                throw new ApplicationException("Wrong flow");
            }

            ShowDebugMessage("StatesOfGame.PlayerDeclaringTurn");
            StateOfGame = StatesOfGame.PlayerDeclaringTurn;
        }

        //called from GUI
        public void SendMessageToOpponent(string message)
        {
            _gc.SendMessage(message);
        }

        //called from GameCommunication when results arrive
        internal void ShowResultsFromGc(TurnResults tr)
        {
            if (StateOfGame == StatesOfGame.GameEnded)
            {
                return;
            }
            if (StateOfGame!=StatesOfGame.WaitingForOpponent)
            {
                throw new ApplicationException("Wrong flow");
            }
            ShowDebugMessage("StatesOfGame.ShowingResults");
            StateOfGame = StatesOfGame.ShowingResults;
            ShowResults(tr);
            
        }

        internal void ActualizeCharacters()
        {
            _actualizeCharacters(Map.GetCharacters());
        }

        //called from GameCommunication when client turn arrive
        public void ActualizeMap(PlayerTurn oponentTurn)
        {
           Map.UpdateCharacters(oponentTurn.Moves);
        }

        public void EndOfGame()
        {
            StateOfGame = StatesOfGame.GameEnded;
            ShowMessage("Game ended");
        }
    }
}
