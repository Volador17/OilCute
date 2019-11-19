using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.UI.GridOil
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public partial class ToolStripCeckBox : ToolStripControlHost
    {
        public ToolStripCeckBox()
            : base(new CheckBox()) 
        {
            InitializeComponent();
        }

        

        public CheckBox CheckBoxControl
        {
            get { return Control as CheckBox; }
        }

        public bool Checked
        {
            get { return CheckBoxControl.Checked; }
            set { CheckBoxControl.Checked = value; }
        }

        public event EventHandler CheckedChanged;

        public void OnCheckedChanged(object sender, EventArgs e)
        {
            // not thread safe!
            if (CheckedChanged != null)
            {
                CheckedChanged(sender, e);
            }
        }

        protected override void OnSubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            (control as CheckBox).CheckedChanged += OnCheckedChanged;
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            base.OnUnsubscribeControlEvents(control);
            (control as CheckBox).CheckedChanged -= OnCheckedChanged;
        }
    }
}
