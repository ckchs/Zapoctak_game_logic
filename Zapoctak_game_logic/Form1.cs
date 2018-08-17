using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Zapoctak_game_logic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Socket Socket;
        //host
        private void button1_Click(object sender, EventArgs e)
        {
            var f = new Form2(textBox1.Text, textBox2.Text, true);
            f.Show();
            this.Hide();
        }

        //client
        private void button2_Click(object sender, EventArgs e)
        {
            var f = new Form2(textBox1.Text, textBox2.Text, false);
            f.Show();
            this.Hide();
        }
    }
}
