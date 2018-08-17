using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLogic.Source;
using Action = System.Action;

namespace Zapoctak_game_logic
{
    public partial class Form2 : Form
    {
        private GameManager _gm;
        private List<MapCharacter> _characters;
        private int _team = 0;
        public Form2(string ipaddress, string port, bool host)
        {
            InitializeComponent();
            _characters = CreateCharacters();
            if (host)
            {
                _team = 1;
            }
            
            _gm=new GameManager(_characters.Select(x=>x.Character).ToList(),5,ShowResults,ShowMessage,ActualizeCharacters,host,ipaddress,port);
        }

        //To have same character as map have
        public void ActualizeCharacters(List<Character> chars)
        {
            foreach (var mc in _characters)
            {
                mc.Character = chars.Find(x => x.Name == mc.Character.Name);
            }
        }
        
        private List<MapCharacter> CreateCharacters()
        {
            return new List<MapCharacter>()
            {
                new MapCharacter()
                {
                    Character = new Warrior(new MapCoordinates(1, 4), 1,"team1war0"),
                    Pb= CreateNewCharacterOnMap(1,4,Properties.Resources.Warrior)
                },
                new MapCharacter()
                {
                    Character= new Warrior(new MapCoordinates(3, 4), 1,"team1war1"),
                    Pb= CreateNewCharacterOnMap(3,4,Properties.Resources.Warrior)
                },
                new MapCharacter()
                {
                    Character=new Warrior(new MapCoordinates(1, 0), 0,"team0war0"),
                    Pb= CreateNewCharacterOnMap(1,0,Properties.Resources.WarriorDark)
                },
                new MapCharacter()
                {
                    Character=new Warrior(new MapCoordinates(3, 0), 0,"team0war1"),
                    Pb= CreateNewCharacterOnMap(3,0,Properties.Resources.WarriorDark)
                },
            };
        }

        //Draw char on map
        private PictureBox CreateNewCharacterOnMap(int x, int y, Bitmap image)
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            var characterpb=new PictureBox();
            characterpb.BackgroundImage = image;
            characterpb.Location = new System.Drawing.Point(x*100+55,y*100+ 28);
            characterpb.Name = "charpb";
            characterpb.Size = new System.Drawing.Size(100, 100);
            characterpb.TabIndex = 0;
            characterpb.TabStop = false;
            characterpb.Visible = true;
            characterpb.Enabled = true;
            characterpb.Click+=CharacterpbOnClick;
            characterpb.MouseEnter += char_MouseEnter;
            characterpb.MouseLeave += char_MouseLeave;
            
            this.Controls.Add(characterpb);
            characterpb.BringToFront();
            return characterpb;

        }

        //Open new form to declare char
        private void CharacterpbOnClick(object sender, EventArgs e)
        {
            if (_gm.StateOfGame!=GameManager.StatesOfGame.PlayerDeclaringTurn)
            {
                return;
            }
            if (!(sender is PictureBox but))
            {
                throw new NullReferenceException("zle volanie");
            }

            var c = _characters.Where(x=>x.Pb==but).Select(x=>x.Character).FirstOrDefault();
            if (c.Team==_team)
            {
                var f = new PlayerMoveSetUp(c);
                f.Show();
            }
        }

        //Called from GameManager to show results
        public async void ShowResults(TurnResults tr)
        {
            foreach (var act in tr.Results)
            {
                await ShowAction(act);
            }

            if (GameManager.Debug)
            {
                CheckIfEverythingIsCorrect();
            }

            ClearCharacterPlanings();
            _gm.DoneShowingResults();
            if (tr.GameEnded)
            {
                _gm.EndOfGame();
            }
        }
        //Called after turn end to remove plannigs from previous turn
        private void ClearCharacterPlanings()
        {
            foreach (var mapCharacter in _characters)
            {
                mapCharacter.Character.ResetPlanning();
            }
        }

        //Check if map and _characters are same.
        private void CheckIfEverythingIsCorrect()
        {
            var correct = true;
            foreach (var mc in _characters)
            {
                
                correct = _gm.Map.GetAt(mc.Character.ActualPosition) == mc.Character;
                correct = mc.Pb.Location.X == mc.Character.ActualPosition.X * 100 + 55;
                correct = mc.Pb.Location.Y == mc.Character.ActualPosition.Y * 100 + 28;
            }

            if (!correct)
            {
                throw new SystemException("gui and gm different chars");
            }
        }

        //Show one action from results
        private async Task ShowAction(GameLogic.Source.Action action)
        {
            if (action is MoveAction ma)
            {
                var mapCharacter = _characters.First(x => x.Character.Name.Equals(ma.CharacterName));

                var animationsteps = 10;
                var addx = ((ma.Move.X * 100 + 55) - (mapCharacter.Pb.Location.X)) / animationsteps;
                var addy = ((ma.Move.Y * 100 + 28) - (mapCharacter.Pb.Location.Y)) / animationsteps;
                for (var i = 0; i < animationsteps - 1; i++)
                {
                    ChangeControl(mapCharacter.Pb,
                        (pb) =>
                        {
                            pb.Location = new Point(mapCharacter.Pb.Location.X + addx,
                                mapCharacter.Pb.Location.Y + addy);
                        }
                    );
                    
                    await Task.Delay(100);
                }
                ChangeControl(mapCharacter.Pb,
                    (pb) => { pb.Location = new Point(ma.Move.X * 100 + 55, ma.Move.Y * 100 + 28); }
                );
                await Task.Delay(100);
            }

            if (action is AttackAction aa)
            {
                await ShowActionLabel("I am attacking Position x: " + aa.Attack.X.ToString() + " Position y: " + aa.Attack.Y.ToString(), new Point(aa.Character.X * 100 + 55, aa.Character.Y * 100 + 28),2000);
            }

            if (action is CollisionWithEnemyAction cea)
            {
                var p = new Point((cea.Char2.X + (cea.Char1.X-cea.Char2.X)/2) *100+55, (cea.Char2.Y+ (cea.Char1.Y - cea.Char2.Y)/2) * 100 + 28);
                await ShowActionLabel("We are enemies and we collided",p,2000);
            }

            if (action is CollisionWithAllyAction caa)
            {
                var p = new Point(Math.Abs(caa.Char1.X - caa.Char2.X) * 100 + 55, Math.Abs(caa.Char1.Y - caa.Char2.Y) * 100 + 28);
                await ShowActionLabel("We are Allies and we collided", p, 2000);
            }

            if (action is DamageAction dma)
            {
                await ShowActionLabel("I got damage: " + dma.Strength, new Point(dma.Mc.X*100+55,dma.Mc.Y*100+28), 2000);
            }

            if (action is DyingAction dya)
            {
                var mapCharacter = _characters.First(x => x.Character.ActualPosition.Equals(dya.Character));
                if (mapCharacter.Pb.InvokeRequired)
                {
                    var d = new Func<GameLogic.Source.Action,Task>(ShowAction);
                    this.Invoke(d, new object[] { action });
                }
                else
                {
                    await ShowActionLabel("I died", new Point(dya.Character.X * 100 + 55, dya.Character.Y * 100 + 28), 2000);
                    mapCharacter.Pb.Visible = false;
                    mapCharacter.Pb.Enabled = false;
                    _characters.Remove(mapCharacter);
                }
            }
        }

        //Change this control working from different thread
        private void ChangeControl(Control control, Action<Control> func)
        {
            if (control.InvokeRequired)
            {
                var d = new Action<Control, Action<Control>>(ChangeControl);
                this.Invoke(d, new object[] { control, func });
            }
            else
            {
                func(control);
                this.Refresh();
            }
        }

        //label that shows damage and collisions
        private async Task ShowActionLabel(string text, Point location, int howLong)
        {


            ChangeControl(actionLabel, pb =>
            {
                actionLabel.BringToFront();
                actionLabel.Location = location;
                actionLabel.Text = text;
                actionLabel.Visible = true;
            });
            await Task.Delay(howLong);
            ChangeControl(actionLabel, pb => { actionLabel.Visible = false; });
        }

        //called from GM to show messege in textbox1
        public void ShowMessage(string message)
        {
            if (this.textBox1.InvokeRequired)
            {
                var d = new Action<string>(ShowMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                this.textBox1.AppendText(message);
                this.textBox1.AppendText(System.Environment.NewLine);
            }
        }

        //end turn
        private void button1_Click(object sender, EventArgs e)
        {
            if (_gm.StateOfGame!=GameManager.StatesOfGame.PlayerDeclaringTurn)
            {
                _gm.ShowDebugMessage("Cant send new turn, wrong state of game");
                return;
            }
            _gm.ShowDebugMessage("Sending turn to gamemanager");

            foreach (var character in _characters.Select(c => c.Character).Where(c => c.Team == _team))
            {
                character.ConvertUiPlanned();
            }
            var pt = new PlayerTurn()
            {
               
                Moves = _characters.Select(c => c.Character).Where(c => c.Team == _team).OrderBy(c => c.Order).ToList()
            };
            _gm.DeclarePlayerTurn(pt);

        }

        //send message
        private void button2_Click(object sender, EventArgs e)
        {
            _gm.SendMessageToOpponent(textBox2.Text);
            textBox2.Text = "Message";
        }

        //showing hp
        void char_MouseEnter(object sender, EventArgs e)
        {
            
            if (sender is PictureBox rtb)
            {
                var mc = _characters.First(x => x.Pb == rtb);
                this.toolTip1.Show(mc.Character.Hp.ToString(), rtb);
            }
        }

        //hiding hp
        void char_MouseLeave(object sender, EventArgs e)
        {
            if (sender is PictureBox rtb)
            {
                this.toolTip1.Hide(rtb);
            }
        }

        //closing application
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
