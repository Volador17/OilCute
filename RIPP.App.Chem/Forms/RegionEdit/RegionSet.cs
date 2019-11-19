using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.App.Chem.Forms.RegionEdit
{
    public partial class RegionSet : Form
    {
        public RegionSet()
        {
            InitializeComponent();
            this.init();
        }

        private void init()
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(this.guetGrid1, "1", "1336.4", "1489.2");
            this.guetGrid1.Rows.Add(row);


            row = new DataGridViewRow();
            row.CreateCells(this.guetGrid1, "2", "1689.6", "1763.3");
            this.guetGrid1.Rows.Add(row);

            row = new DataGridViewRow();
            row.CreateCells(this.guetGrid1, "3", "1951.3", "2300.8");
            this.guetGrid1.Rows.Add(row);

        }
    }
}
