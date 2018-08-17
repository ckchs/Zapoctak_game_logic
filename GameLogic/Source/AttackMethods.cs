using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Source
{
    internal static class AttackMethods
    {
        //Deal damage to map coordinate(must be character)
        internal static void DealDamageTo(MapCoordinates where, int damage, Map map, List<Action> results)
        {
            if (!(map.GetAt(where) is Character ch))
            {
                throw new Exception("cant deal dmg to noncharacter position");
            }

            results.Add(new DamageAction(where, damage));
            ch.Hp -= damage;
            CheckDying(ch,map,results);
            
        }

        //Check if character is dying
        internal static void CheckDying(Character ch,Map map, List<Action> results)
        {
            if (ch.Hp > 0) return;
            results.Add(new DyingAction(ch.ActualPosition));
            map.DiedAt(ch.ActualPosition);
        }

        //check if character exist on map coordinate if yes deal damage
        private static void CheckAndDealDamage(MapCoordinates where, int damage, Map map, List<Action> results)
        {
            if (map.GetAt(where) is Character ch)
            {
                DealDamageTo(where, damage, map, results);
            }
        }

        //calculate relative attack when character collided
        private static MapCoordinates ToRelativeAttack(Character c)
        {
            var xadd = c.PlannedMove[c.PlannedMove.Count - 1].X - c.PlannedAttack.Mc.X;
            var yadd = c.PlannedMove[c.PlannedMove.Count - 1].Y - c.PlannedAttack.Mc.Y;
            var newAttackPositionX = c.ActualPosition.X + xadd;
            var newAttackPositionY = c.ActualPosition.Y + yadd;
            if (newAttackPositionX>=Map.Size)
            {
                newAttackPositionX = Map.Size - 1;
            }
            if (newAttackPositionY >= Map.Size)
            {
                newAttackPositionY = Map.Size - 1;
            }
            if (newAttackPositionX<0)
            {
                newAttackPositionX = 0;
            }
            if (newAttackPositionY < 0)
            {
                newAttackPositionY = 0;
            }

            return new MapCoordinates(newAttackPositionX, newAttackPositionY);
        }
        
        internal static void OnePlayerAttack(Character char1, Map map, List<Action> results)
        {
            if (char1.PlannedAttack == null)
            {
                return;
            }
            if (char1.ActualPosition.Distance(char1.PlannedAttack.Mc) > char1.AttackRange)
            {
                char1.PlannedAttack.Mc = ToRelativeAttack(char1);
            }
            switch (char1.PlannedAttack.Type)
            {
                case TypesOfAttacks.Normal:
                    CheckAndDealDamage(char1.PlannedAttack.Mc,char1.AttackStrength,map,results);
                    break;
                case TypesOfAttacks.Special:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static void TwoPlayerAttack(Character char1, Character char2, Map map, List<Action> results)
        {
            OnePlayerAttack(char1, map, results);
            OnePlayerAttack(char2, map, results);

        }
    }
}
