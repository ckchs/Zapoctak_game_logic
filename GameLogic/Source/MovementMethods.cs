using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Source;

namespace GameLogic.Source
{
    internal static class MovementMethods
    {
        //resolve collision if enemy deal damage to both characters
        private static void ResolveCollision(MapCoordinates mc1, MapCoordinates mc2, Map map, List<Action> results)
        {
            if (!(map.GetAt(mc1) is Character) || !(map.GetAt(mc2) is Character))
            {
                throw new Exception("Colliding not two characters");
            }
            var char1 = map.GetAt(mc1) as Character;
            var char2 = map.GetAt(mc2) as Character;
            if (char1.Team == char2.Team)
            {
                results.Add(new CollisionWithAllyAction(char1.ActualPosition, char2.ActualPosition));
                return;
            }
            results.Add(new CollisionWithEnemyAction(char1.ActualPosition, char2.ActualPosition));
            AttackMethods.DealDamageTo(mc1, char2.CollisionStrength, map, results);
            AttackMethods.DealDamageTo(mc2, char1.CollisionStrength, map, results);


        }

        //if empty move character else resolve collision
        private static bool CheckAndMoveCharacterStep(Character c, MapCoordinates whereMove, Map map, List<Action> results)
        {
            if (map.GetAt(whereMove).IsMoveableTo())
            {
                MoveCharacterStep(c, whereMove, map, results);
                return false;
            }
            ResolveCollision(c.ActualPosition, whereMove, map, results);
            return true;

        }

        //move to empty square
        private static void MoveCharacterStep(Character c, MapCoordinates whereMove, Map map, List<Action> results)
        {
            if (!map.GetAt(whereMove).IsMoveableTo())
            {
                throw new Exception("should be empty square");
            }

            map.SwapSquares(c.ActualPosition, whereMove);
            c.ActualPosition = whereMove;
            results.Add(new MoveAction(c.ActualPosition,whereMove,c.Name));
        }

        //calculate if only one player is currently moving
        internal static bool OnePlayerMovement(Character character, Map map, List<Action> results)
        {
            for (var i = 0; i < character.PlannedMove.Count; i++)
            {
                if (CheckAndMoveCharacterStep(character, character.PlannedMove[i], map, results))
                    return true;
            }
            return false;
        }

        //calculate two characters moving at the same time
        internal static Tuple<bool, bool> TwoPlayerMovement(Character char1, Character char2, Map map, List<Action> results)
        {
            var player1StepsCount = char1.PlannedMove.Count;
            var player2StepsCount = char2.PlannedMove.Count;
            var actualStep = 0;
            var char1Collided = false;
            var char2Collided = false;
            var char1IsMoving = player1StepsCount > actualStep;
            var char2IsMoving = player2StepsCount > actualStep;
            while (char1IsMoving || char2IsMoving)
            {
                
                if (char1IsMoving && char2IsMoving)
                {
                    var char1NextStep = char1.PlannedMove[actualStep];
                    var char2NextStep = char2.PlannedMove[actualStep];
                    if (char1NextStep.Equals(char2NextStep))
                    {
                        if (!map.GetAt(char1NextStep).IsMoveableTo())
                        {
                            ResolveCollision(char1.ActualPosition, char1NextStep, map, results);
                            ResolveCollision(char2.ActualPosition, char2NextStep, map, results);
                        }
                        else
                        {
                            var r = TurnLogic.Rnd.Next(2);
                            switch (r)
                            {
                                case 0:
                                    MoveCharacterStep(char1, char1NextStep, map, results);
                                    break;
                                case 1:
                                    MoveCharacterStep(char2, char2NextStep, map, results);
                                    break;
                                default:
                                    throw new Exception("wrong random number");
                            }
                            ResolveCollision(char1.ActualPosition, char2.ActualPosition, map, results);
                        }
                        return new Tuple<bool, bool>(true, true);
                    }
                    if (char1NextStep.Equals(char2.ActualPosition) || char2NextStep.Equals(char1.ActualPosition))
                    {
                        ResolveCollision(char1.ActualPosition, char2.ActualPosition, map, results);
                        return new Tuple<bool, bool>(true, true);
                    }
                    char1Collided = CheckAndMoveCharacterStep(char1, char1NextStep, map, results);
                    char2Collided = CheckAndMoveCharacterStep(char2, char2NextStep, map, results);

                    ++actualStep;
                    char1IsMoving = char1.PlannedMove.Count > actualStep&&!char1Collided;
                    char2IsMoving = char2.PlannedMove.Count > actualStep&&!char2Collided;

                    continue;
                }

                if (char1IsMoving && !char2IsMoving)
                {
                    var char1NextStep = char1.PlannedMove[actualStep];
                    char1Collided = CheckAndMoveCharacterStep(char1, char1NextStep, map, results);

                    ++actualStep;
                    char1IsMoving = char1.PlannedMove.Count > actualStep && !char1Collided;
                    continue;
                }

                if (!char1IsMoving && char2IsMoving)
                {
                    var char2NextStep = char2.PlannedMove[actualStep];
                    char2Collided = CheckAndMoveCharacterStep(char2, char2NextStep, map, results);

                    ++actualStep;
                    char2IsMoving = char2.PlannedMove.Count > actualStep && !char2Collided;
                    continue;
                }
            }
            return new Tuple<bool, bool>(char1Collided, char2Collided);
        }
    }
}
