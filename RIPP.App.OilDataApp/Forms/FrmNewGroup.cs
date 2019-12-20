using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.App.OilDataApp.Forms
{
    public delegate void TransfDelegate(String groupName,String groupRemark);
    public partial class FrmNewGroup : Form
    {
        public FrmNewGroup()
        {
            InitializeComponent();
        }
        public event TransfDelegate TransfEvent;

        private void button1_Click(object sender, EventArgs e)
        {
            TransfEvent(textBox1.Text,textBox2.Text) ;
            this.Close();
        }
    }
}
