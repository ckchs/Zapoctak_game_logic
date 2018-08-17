using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLogic.Source;

namespace Zapoctak_game_logic
{
    public partial class PlayerMoveSetUp : Form
    {
        private Character _c;
        private List<MapCoordinates> _possibleAttacks;
        public PlayerMoveSetUp(Character ch)
        {
            _c = ch;
            InitializeComponent();
            for (var i = 0; i < ch.MaxSteps; i++)
            {
                var label = new Label();
                this.Controls.Add(label);
                label.Text = "Move number " + i.ToString();
                label.Location = new Point(10, 10 + i * 50);
                label.Enabled = true;
                label.Visible = true;
                label.BringToFront();
                
                CreateButton("Move number " + i.ToString() + " UP", "Up " + i.ToString(),100, 10 + i * 50);
                CreateButton("Move number " + i.ToString() + " Down", "Down " + i.ToString(),250, 10 + i * 50);
                CreateButton("Move number " + i.ToString() + " Left", "Left " + i.ToString(),400, 10 + i * 50);
                CreateButton("Move number " + i.ToString() + " Right", "Right " + i.ToString(),550,10+i*50);
                CreateButton("Move number " + i.ToString() + " End of moves", "DM " + i.ToString(),700, 10 + i * 50);
            }

            ActualizeInformation();


        }

        //Actualize information label
        private void ActualizeInformation()
        {
            _possibleAttacks=new List<MapCoordinates>();
            var planedmoves = _c.PlannedMove;
            var movesstring="";
            var positionToCountPossibleAttacksFrom = _c.PlannedMove!=null && _c.PlannedMove.Count !=0 ? _c.PlannedMove.Last()  : _c.ActualPosition;
            for (var i = -1*_c.AttackRange; i <= _c.AttackRange; i++)
            {
                for (var j = -1 * _c.AttackRange; j <= _c.AttackRange; j++)
                {
                    try
                    {
                        var possible = new MapCoordinates(positionToCountPossibleAttacksFrom.X + i,
                            positionToCountPossibleAttacksFrom.Y + j);
                        if (possible.Distance(positionToCountPossibleAttacksFrom) <= _c.AttackRange&& possible.Distance(positionToCountPossibleAttacksFrom) >0)
                        {
                            _possibleAttacks.Add(possible);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }
                }
            }

            if (planedmoves!=null)
            {
                foreach (var move in planedmoves)
                {
                    movesstring += move.ToString() + Environment.NewLine;
                }
            }

            var attackString="";
            foreach (var att in _possibleAttacks)
            {
                attackString += att.ToString() + Environment.NewLine;
            }


            informationLabel.Text =
                "Character HP: " + _c.Hp + Environment.NewLine +
                "Character order: "+_c.Order.ToString()+Environment.NewLine+
                "Character Planned Attack: " + _c.PlannedAttack?.Mc.ToString() + Environment.NewLine +
                "Character Planned Move: " + movesstring + Environment.NewLine +
                "Possible Attacks: " + attackString + Environment.NewLine;
        }

        //Create movement buttons
        private void CreateButton(string text, string name, int x, int y)
        {
            var but = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Enabled = true,
                Visible = true,
                Name = name,
                Size = new Size(120, 30)
            };
            but.Click += ButtonOnClick;
            Controls.Add(but);
            
        }

        //Movement buttons method
        private void ButtonOnClick(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var tokens = btn.Name.Split();
            var n = Int32.Parse(tokens[1]); 
            switch (tokens[0])
            {
                case "Up":
                    _c.UiPlannedMoves[n] = PossibleMoves.Up;
                    break;
                case "Down":
                    _c.UiPlannedMoves[n] = PossibleMoves.Down;
                    break;
                case "Left":
                    _c.UiPlannedMoves[n] = PossibleMoves.Left;
                    break;
                case "Right":
                    _c.UiPlannedMoves[n] = PossibleMoves.Right;
                    break;
                case "DM":
                    _c.UiPlannedMoves[n] = PossibleMoves.Nomove;
                    break;
            }

            _c.PlannedAttack = null;
            _c.ConvertUiPlanned();
            ActualizeInformation();
        }

        //Set attack
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var mc = new MapCoordinates(Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text));
                if (!_possibleAttacks.Contains(mc))
                {
                    return;
                }

                _c.PlannedAttack = new PlannedAttack()
                {
                    Mc = mc,
                    Type = TypesOfAttacks.Normal
                };
                ActualizeInformation();
            }
            catch (Exception)
            {
                ActualizeInformation();
            }

            
        }

        //Set order
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
               _c.Order= Int32.Parse(textBox3.Text);
                ActualizeInformation();
            }
            catch (Exception)
            {
                
            }
        }
    }
}
