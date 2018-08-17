using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Source
{
    [Serializable]
    public class PlayerTurn
    {
        public List<Character> Moves;
    }
    [Serializable]
    public class TurnResults
    {
        public List<Action> Results;
        public bool GameEnded = false;
    }
}
