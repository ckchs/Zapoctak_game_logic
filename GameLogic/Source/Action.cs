using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Source
{
    //Used for showing results to GUI
    [Serializable]
    public class Action
    {

    }
    [Serializable]
    public class MoveAction : Action
    {
        public MapCoordinates Character;
        public string CharacterName;
        public MapCoordinates Move;
        public MoveAction(MapCoordinates c, MapCoordinates mc, string charName)
        {
            Character = c;
            Move = mc;
            CharacterName = charName;
        }
    }
    [Serializable]
    public class TwoMoveAction : Action
    {
        public MapCoordinates Char1;
        public MapCoordinates Char2;
        public MapCoordinates Move1;
        public MapCoordinates Move2;
        public TwoMoveAction(MapCoordinates c1, MapCoordinates c2, MapCoordinates m1, MapCoordinates m2)
        {
            Char1 = c1;
            Char2 = c2;
            Move1 = m1;
            Move2 = m2;
        }
    }
    [Serializable]
    public class CollisionWithEnemyAction : Action
    {
        public MapCoordinates Char1;
        public MapCoordinates Char2;
        public CollisionWithEnemyAction(MapCoordinates c1, MapCoordinates c2)
        {
            Char1 = c1;
            Char2 = c2;
        }
    }
    [Serializable]
    public class CollisionWithAllyAction : Action
    {
        public MapCoordinates Char1;
        public MapCoordinates Char2;
        public CollisionWithAllyAction(MapCoordinates c1, MapCoordinates c2)
        {
            Char1 = c1;
            Char2 = c2;
        }
    }
    [Serializable]
    public class AttackAction : Action
    {
        public MapCoordinates Character;
        public MapCoordinates Attack;
        public TypesOfAttacks Type;
        public AttackAction(MapCoordinates c, MapCoordinates a, TypesOfAttacks t)
        {
            Character = c;
            Attack = a;
            Type = t;
        }
    }
    [Serializable]
    public class DamageAction : Action
    {
        public MapCoordinates Mc;
        public int Strength;
        public DamageAction(MapCoordinates m, int s)
        {
            Mc = m; Strength = s;
        }
    }
    [Serializable]
    public class DyingAction : Action
    {
        public MapCoordinates Character;
        public DyingAction(MapCoordinates c1)
        {
            Character = c1;
        }
    }
}
