using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.NIR.Controls
{
    public partial class SpecGraphForm : Form
    {
        public SpecGraphForm()
        {
            InitializeComponent();
        }

        public void OpenDialog(List<RIPP.NIR.Spectrum> specs)
        {
            this.specGraph1.DrawSpec(specs);
            this.ShowDialog();

        }
    }
}
