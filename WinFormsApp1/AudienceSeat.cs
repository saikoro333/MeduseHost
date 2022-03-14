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
    public partial class AudienceSeat : Form
    {
        public const int SEAT_NUM = 8;
        const string NO_ENTRY = " - ";

        private int selectedIndex = 0;
        private int audienceCount = 0;
        private HostControl hostControl;

        public AudienceSeat(HostControl host)
        {
            InitializeComponent();
            this.hostControl = host;
            this.hostControl.InitialAudienceList();
            this.UpdateSeatGrid();
            this.addAudience();
            this.addAudience();
        }

        public enum SeatData
        {
            NUM = 0,
            NAME = 1,
            PLATFORM = 2,
            UUID = 3
        }


        private void UpdateSeatGrid()
        {
            for (int i = 0; i < SEAT_NUM; i++)
            {
                var ad = hostControl.audienceList[i];
                dataGridView1.Rows.Add(i, ad.name, ad.platform, ad.uuid,ad.getState());
            }
        }

        public void addAudience()
        {
            dataGridView1.Rows.Clear();
            string name = "AD-"+ this.audienceCount +"-"+WiiController.PLATFORM;
            this.setAudienceList(this.audienceCount++, name, WiiController.PLATFORM, "CC");
            this.UpdateSeatGrid();
        }
        public void setAudienceList(int index, string name, string p, string u)
        {
            AudienceData ad = new AudienceData(name,p,u);
            hostControl.audienceList[index] = ad;
        }

        private void setAudienceToGrid(int index,AudienceData ad)
        {
            dataGridView1.Rows[index].Cells[(int)SeatData.NUM].Value = index;
            dataGridView1.Rows[index].Cells[(int)SeatData.NAME].Value = ad.name;
            dataGridView1.Rows[index].Cells[(int)SeatData.PLATFORM].Value = ad.platform;
            dataGridView1.Rows[index].Cells[(int)SeatData.UUID].Value = ad.uuid;
        }


        public struct AudienceData
        {
            public bool isExsit;
            public string name;
            public string platform;
            public string uuid;

            public AudienceData()
            {
                this.isExsit = false;
                this.name = NO_ENTRY;
                this.platform = NO_ENTRY;
                this.uuid = NO_ENTRY;
            }
            public AudienceData(string name, string p, string u)
            {
                this.isExsit = true;
                this.name = name;
                this.platform = p;
                this.uuid = u;
            }
            public string getState()
            {
                return (isExsit) ? "○" : "×";
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //add player
            // dataGridView1.inde
            MessageBox.Show(this.selectedIndex.ToString());
            int playernum = hostControl.getEmptyPlayerNum(); 
            var ad = hostControl.audienceList[this.selectedIndex-1];
            var plt = PlayerController.GetPlatformNum(ad.platform);
            if (!ad.isExsit)
            {
                MessageBox.Show("No Audience ...");
                return;
            }
            if(playernum < 0)
            {
                MessageBox.Show("No Empty ...");
            }
            else
            {
                string name = ad.name+"-" + playernum + "-" + PlayerController.Platform.Wii;
                this.hostControl.createPlayer((uint)playernum, name, plt);
            }
            

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            this.selectedIndex = row+1;
        }
    }
}
