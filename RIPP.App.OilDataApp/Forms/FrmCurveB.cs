using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.OilDataApp.FrmBase;
using RIPP.OilDB.Model;

namespace RIPP.App.OilDataApp.Forms
{
    public partial class FrmCurveB : FrmCurve
    {
        public FrmCurveB( )
        {
            InitializeComponent();
          
        }

        public FrmCurveB(CurveEntity curve)
            : base(curve)
        {
            InitializeComponent();
        }
    }
}
