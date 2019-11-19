using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace RIPP.OilDB.UI.GridOil
{
    public partial class GridList : DataGridView
    {
        public GridList()
        {
            InitializeComponent();
            InitStyle();
        }

        public GridList(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitStyle();
        }

        private void InitStyle()
        {
            this.AllowUserToAddRows = false;
          
            this.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            this.DefaultCellStyle = myStyle.dgdViewCellStyle2();
            this.BorderStyle = BorderStyle.None;
            this.RowHeadersWidth = 30;
            this.MultiSelect = false;

        }
    }
}
