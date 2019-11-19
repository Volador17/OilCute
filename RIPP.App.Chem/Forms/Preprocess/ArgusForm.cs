using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Data;

namespace RIPP.App.Chem.Forms.Preprocess
{
    public partial class ArgusForm : Form
    {
        public ArgusForm()
        {
            InitializeComponent();
        }

        public event EventHandler<ArgusFormEventArgus> Changed;


        public void OpenDialog(Dictionary<string, Argu>  argus)
        {
            if (argus==null||  argus.Count == 0)
                return;
            this.argusControl1.Argus = argus;
            this.ShowDialog();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
            if (this.Changed != null)
                this.Changed(this,new ArgusFormEventArgus() { Argus = this.argusControl1.Argus });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class ArgusFormEventArgus : EventArgs
    {
        public Dictionary<string, Argu> Argus { set; get; }
    }
}
