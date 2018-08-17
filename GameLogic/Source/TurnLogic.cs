using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Source
{
    public static class TurnLogic
    {
        public static Random Rnd=new Random();
        
        //calculate turn return turnresults
        public static TurnResults ResolveTurn(PlayerTurn player1, PlayerTurn player2, Map map)
        {
            var results = new List<Action>(50);
            while (player1.Moves.Count>0 && player2.Moves.Count>0)
            {
                Character char1 = null;
                Character char2 = null;
                if (player1.Moves.Count > 0)
                {
                    char1 = player1.Moves[0];
                    player1.Moves.RemoveAt(0);
                }
                if (player2.Moves.Count>0)
                {
                    char2 = player2.Moves[0];
                    player2.Moves.RemoveAt(0);
                }
                if (char1?.PlannedMove != null && char2?.PlannedMove != null)
                {
                    MovementMethods.TwoPlayerMovement(char1, char2, map, results);
                    AttackMethods.TwoPlayerAttack(char1, char2, map, results);
                }
                if (char1==null&& char2?.PlannedMove != null)
                {
                    MovementMethods.OnePlayerMovement(char2, map, results);
                    AttackMethods.OnePlayerAttack(char2, map, results);
                }
                if (char1?.PlannedMove != null && char2 == null)
                {
                    MovementMethods.OnePlayerMovement(char1, map, results);
                    AttackMethods.OnePlayerAttack(char1, map, results);
                }
            }

            var res = new TurnResults
            {
                Results = results,
                GameEnded = player1.Moves.Any(x => !x.Isdead())|| player2.Moves.Any(x => !x.Isdead())
            };
            return res;
        }
    }

   
}
