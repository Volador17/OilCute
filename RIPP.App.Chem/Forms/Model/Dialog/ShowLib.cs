using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
namespace RIPP.App.Chem.Forms.Model.Dialog
{
    public partial class ShowLib : Form
    {
        public ShowLib()
        {
            InitializeComponent();
        }

        public void OpenDialog(SpecBase lib)
        {
            this.specGridView1.Specs = lib;
            this.specGraph1.DrawSpec(lib.Specs);
            this.ShowDialog();
        }
    }
}
