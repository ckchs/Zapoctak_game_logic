using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Source
{
    //Characters
    [Serializable]
    public enum PossibleMoves
    {
        Nomove,Up, Down,Left,Right
    }

    [Serializable]
    public abstract class Character:GameSquare
    {
        public int Team;
        public int Hp;
        public int Order;
        public bool Isdead() { return Hp <= 0; }
        public int AttackStrength;
        public int CollisionStrength;
        public int AttackRange;
        public int SpecialRange;

        public string Name;
        public int MaxSteps;
        public MapCoordinates ActualPosition;
        public List<MapCoordinates> PlannedMove;
        public PlannedAttack PlannedAttack;
        public PossibleMoves[] UiPlannedMoves;
        public override bool IsMoveableTo()
        {
            return false;
        }

        //Convert UI planned moves to PlannedMoves
        public void ConvertUiPlanned()
        {
            PlannedMove=new List<MapCoordinates>();
            for (var i = 0; i < UiPlannedMoves.Length; i++)
            {
                try
                {
                    var lastMove = i == 0 ? ActualPosition : PlannedMove[i - 1];
                    switch (UiPlannedMoves[i])
                    {
                        case PossibleMoves.Up:
                            PlannedMove.Add(new MapCoordinates(lastMove.X, lastMove.Y - 1));
                            break;
                        case PossibleMoves.Down:
                            PlannedMove.Add(new MapCoordinates(lastMove.X, lastMove.Y + 1));
                            break;
                        case PossibleMoves.Left:
                            PlannedMove.Add(new MapCoordinates(lastMove.X - 1, lastMove.Y));
                            break;
                        case PossibleMoves.Right:
                            PlannedMove.Add(new MapCoordinates(lastMove.X + 1, lastMove.Y));
                            break;
                        case PossibleMoves.Nomove:
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    return;
                }
            }
        }

        //Reset character planning from previous turn
        public void ResetPlanning()
        {
            PlannedAttack = null;
            PlannedMove = new List<MapCoordinates>();
            UiPlannedMoves=new PossibleMoves[MaxSteps];
        }
        
    }
    [Serializable]
    public class PlannedAttack
    {
        public MapCoordinates Mc;
        public TypesOfAttacks Type;
    }
    [Serializable]
    public enum TypesOfAttacks { Normal, Special };

    [Serializable]
    public class Warrior : Character
    {
        public Warrior(MapCoordinates position,int team,string name)
        {
            Hp = 20;
            AttackRange = 1;
            AttackStrength = 5;
            CollisionStrength = 5;
            AttackRange = 1;
            ActualPosition = position;
            this.Team = team;
            MaxSteps = 2;
            UiPlannedMoves = new PossibleMoves[MaxSteps];
            Name = name;
        }
    }

}
