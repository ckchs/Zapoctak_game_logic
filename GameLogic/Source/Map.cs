using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Source
{
    [Serializable]
    public class Map

    { 
        public static int Size=5;
        public GameSquare[][] _gameMap;
        public GameSquare GetAt(MapCoordinates mc)
        {
            return _gameMap[mc.X][mc.Y];
        }
        public void SwapSquares(MapCoordinates mc1, MapCoordinates mc2)
        {
            var help = _gameMap[mc1.X][mc1.Y];
            _gameMap[mc1.X][mc1.Y] = _gameMap[mc2.X][mc2.Y];
            _gameMap[mc2.X][mc2.Y] = help;
        }

        //set square to emptysquare
        public void DiedAt(MapCoordinates mc)
        {
            _gameMap[mc.X][mc.Y]=new EmptySquare();
        }

        public Map(List<Character> chars, int size)
        {
            Size = size;
            _gameMap=new GameSquare[size][];
            for (var i = 0; i < size; i++)
            {
                _gameMap[i] = new GameSquare[size];
                for (var j = 0; j < size; j++)
                {
                    _gameMap[i][j]=new EmptySquare();
                }
            }

            foreach (var ch in chars)
            {
                _gameMap[ch.ActualPosition.X][ch.ActualPosition.Y] = ch;
            }
        }

        public List<Character> GetCharacters()
        {
            var result = new List<Character>();
            foreach (var gm in _gameMap)
            {
                foreach (var square in gm)
                {
                    if (square is Character c)
                    {
                        result.Add(c);
                    }
                }
            }
            return result;
        }

        public void UpdateCharacters(List<Character> toUpdate)
        {
            var actualChars = this.GetCharacters();
            foreach (var character in toUpdate)
            {
                var charToChange = actualChars.First(x => x.Name == character.Name);
                _gameMap[charToChange.ActualPosition.X][charToChange.ActualPosition.Y] = character;
            }
        }
    }

    [Serializable]
    public struct MapCoordinates
    {
        private int _x;
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                ControlMc(value);
                _x = value;
            }
        }

        private int _y;
        public int Y
        {
            get
            {
                return _y;
            }

            set
            {
                ControlMc(value);
                _y = value;
            }
        }
        public int Distance(MapCoordinates mc)
        {
            return (Math.Abs(mc.X - X) + Math.Abs(mc.Y - Y));
        }

        private void ControlMc(int x)
        {
            if (x>=Map.Size || x<0)
            {
                throw new IndexOutOfRangeException("Map coordinates too big");
            }
        }
        public MapCoordinates(int x, int y)
        {
            this._x = x;
            this._y = y;
            ControlMc(x); ControlMc(y);
        }
        public bool Equals(MapCoordinates mc)
        {
            return mc.X == X && mc.Y == Y;
        }

        public override string ToString()
        {
            return "X coordinate: "+ X.ToString() + " Y coordinate: " +Y.ToString();
        }
    }
}
