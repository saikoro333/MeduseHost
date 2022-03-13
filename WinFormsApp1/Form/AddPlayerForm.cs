using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class AddPlayerForm : Form
    {
        //public String addPlayerData = null;
        private uint playerNum = 0;
        private string platform = null;
        private string playerName = null;

        public AddPlayerForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = comboBox1.SelectedIndex;
            string Text = comboBox1.Items[index].ToString().Substring(0,1);
            this.playerNum = (uint)int.Parse(Text);
            int index2 = comboBox2.SelectedIndex;
            this.platform = comboBox2.Items[index2].ToString();
            this.playerName = textBox1.Text;
            //addPlayerData = Text + Text2;
            this.Close();   
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.platform = null;
            this.playerName=null;
            this.playerNum = 0; 
            this.Close();
        }

        public void addPlayer(HostControl host)
        {
            if (platform == null || playerNum == 0) return;
            host.createPlayer(this.playerNum, this.playerName,PlayerController.GetPlatformNum(this.platform));
        }
    }
}
