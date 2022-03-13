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
        const string NO_ENTRY = "None...";

        public AudienceSeat()
        {
            InitializeComponent();
            this.setSeatGrid();
        }

        private void setSeatGrid()
        {
            for(int i = 0; i < SEAT_NUM; i++)
            {
                dataGridView1.Rows.Add(i+1,NO_ENTRY,NO_ENTRY);
            }
        }
    }
}
