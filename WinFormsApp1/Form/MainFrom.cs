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
    public partial class MainFrom : Form
    {
        private bool showVerbose = false;
        private TextBox[] InputButtonBox = new TextBox[4];
        private HostControl hostControl;

        public int playerIndex = 0;//0~3

        public MainFrom(HostControl host)
        {
            InitializeComponent();
            InputButtonBox[0] = this.textBox1;
            InputButtonBox[1] = this.textBox2;
            InputButtonBox[2] = this.textBox3;
            InputButtonBox[3] = this.textBox4;
            for (int i = 0; i < InputButtonBox.Length; i++)
            {
                InputButtonBox[i].Enabled = false;
            }
            this.textBox5.Enabled = false;
            this.hostControl = host;
        }

        private void setMainDataText()
        {
            string str = "CurrentPlayer: " + hostControl.currEntryCount + "\r\n";
            for (int i = 0; i < HostControl.MAX_PLAYER_NUM; i++)
            {
                var player = hostControl.players[i];
                if (player != null)
                {
                    str += String.Format("[{0}] : {1}\r\n"
                        , player.playerName, player.getStatus()
                        );
                }
                else
                {
                    /*
                    str += String.Format("Player{0} Status: {1}\r\n"
                        , i + 1, PlayerController.PlayerStatus.NONE
                        );
                    */
                }
            }
            this.textBox5.Text = str;
        }

        public void setPlayerText(PlayerController player, int id)
        {
            String str = "";
            if (player != null)
            {
                if (this.showVerbose)
                {
                    str = player.showVerboseDetail();
                }
                else
                {
                    str = String.Format("PlayerNum: {0}\r\nInput : {1}",
                        player.playerNum, player.InputToString());
                }
                InputButtonBox[player.playerNum - 1].Text = str;
            }
            else
            {
                InputButtonBox[id].Text = "Not Connected ...";
            }
        }

        private void setStatusText()
        {
            int idx = this.playerIndex;
            string str = "Selected player is None";
            if (idx >= 0 && idx < HostControl.MAX_PLAYER_NUM)
            {
                var player = hostControl.players[idx];
                if (player != null)
                {
                    str = String.Format("Selected player is {0} ({1}p)"
                        , player.playerName, idx + 1);
                }

            }
            this.toolStripStatusLabel1.Text = str;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.showVerbose = !this.showVerbose;
            if (this.showVerbose)
            {
                this.button2.Text = "show Succinct";
            }
            else
            {
                this.button2.Text = "show Verbose";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (hostControl.players[this.playerIndex] != null)
            {
                string msg = "BAN this player ?";
                DialogResult result = MessageBox.Show(msg, "BAN Player", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    hostControl.players[this.playerIndex] = null;
                    hostControl.currEntryCount--;
                    MessageBox.Show("Success !!!");
                }
                else if (result == System.Windows.Forms.DialogResult.No)
                {
                    //MessageBox.Show("いいえ");
                }
            }
            else
            {
                MessageBox.Show("this player was not exsit");
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddPlayerForm addPlayerForm = new AddPlayerForm();
            addPlayerForm.ShowDialog();
            addPlayerForm.addPlayer(hostControl);
            /*
            if (addPlayerForm.addPlayerData == null) return;
            String apd = addPlayerForm.addPlayerData;
            int pnum = int.Parse(apd.Substring(0, 1));
            var plat = PlayerController.GetPlatformNum(apd.Substring(1));
            hostControl.createPlayer((uint)pnum, plat);
            */
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.playerIndex = tabControl1.SelectedIndex;
            //MessageBox.Show("タブのインデックス：" + tabControl1.SelectedIndex.ToString());
        }

        public void updateForm()
        {
            this.setMainDataText();
            this.setStatusText();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AudienceSeat seat = new AudienceSeat();
            seat.Show();
        }
    }
}
