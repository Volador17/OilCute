using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Models;

namespace RIPP.App.AnalCenter.Forms.Dlg
{
    public partial class FrmModelDetail : Form
    {
        private BindModel _model;
        public FrmModelDetail(BindModel model)
        {
            InitializeComponent();
            this._model = model;
            this.Load += new EventHandler(FrmModelDetail_Load);
        }

        void FrmModelDetail_Load(object sender, EventArgs e)
        {
            this.allMethodDetail1.ShowMotheds(this._model);
        }
    }
}
