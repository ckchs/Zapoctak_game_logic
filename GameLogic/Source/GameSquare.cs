using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Source
{

    [Serializable]
    public abstract class GameSquare
    {
        public abstract bool IsMoveableTo();
    }
    [Serializable]
    public class EmptySquare : GameSquare
    {
        public override bool IsMoveableTo()
        {
            return true;
        }
    }
}
