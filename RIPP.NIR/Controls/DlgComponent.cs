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
   
    
    internal partial class DlgComponent : Form
    {
        private Component _comp = null;
        private bool _isAdd = false;
        private int _colIndex = 0;
        public DlgComponent()
        {
            InitializeComponent();
            
        }

        public event EventHandler<DlgComponentEventArgs> EventSave;

        public void Dialog(Component Comp= null,   bool isAdd = true,int colIndex=0)
        {
            if (!isAdd && Comp == null)
                return;
            if (!isAdd && Comp != null)
            {
                this.Text = "编辑性质";
                this._comp = Comp;
            }
            else
            {
                this.Text = "添加性质";
                this._comp = new Component();
            }
            
            this._isAdd = isAdd;
            this._colIndex = colIndex;
            this._init();
            this.ShowDialog();
        }

        private void _init()
        {
            this.txbName.Text = this._comp.Name;
            this.txbNum.Text = this._comp.Eps.ToString();
            this.txbUnit.Text = this._comp.Units;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.EventSave == null)
                return;
            this._comp.Name = this.txbName.Text.Trim();
            this._comp.Units = this.txbUnit.Text.Trim();
            this._comp.Eps = int.Parse(this.txbNum.Text.Trim());
            DlgComponentEventArgs args = new DlgComponentEventArgs()
            {
                Comp = this._comp,
                IsAdd = this._isAdd,
                ColIndex = this._colIndex
            };
            this.EventSave(this, args);
        }
       

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }

    internal class DlgComponentEventArgs : EventArgs
    {
        public Component Comp { set; get; }
        public bool IsAdd { set; get; }
        public int ColIndex { set; get; }
    }
}
