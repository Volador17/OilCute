using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib.UI.Controls;
using RIPP.NIR;

namespace RIPP.App.Chem.Forms.Spec
{
    public partial class FrmKS : Form
    {
        private SpecBase _lib;
        public FrmKS()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmKS_Load);
        }
        public SpecBase GetLib()
        {
            return this.ksSetControl1.GetLib();
        }

        void FrmKS_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            var p1 = new FlowNodePanel("光谱预处理", 1, this.preprocessControl1, -1, FlowNodeStatu.Default) { Finished = true };
            var p2 = new FlowNodePanel("K-S分集", 2, this.ksSetControl1,1);
            this.flowControl1.SetFlows(new FlowNodePanel[] {  p2, p1 });


            this.preprocessControl1.GetInput = getinput;
            this.preprocessControl1.SetOutput = setOutput;

            // 设置子控件
            this.preprocessControl1.Dock = DockStyle.Fill;
            this.ksSetControl1.Dock = DockStyle.Fill;
            this.ksSetControl1.Visible=false;
            this.flowControl1.Active(1);
        }

        private SpecBase getinput()
        {

            return this.GetLib();
        }

        private void setOutput(SpecBase lib)
        {
          
            // this.plS1CVResult1.PLSContent.ActiveStep = 2;
            if (lib != null)
                this.ksSetControl1.LoadData(lib,this._lib);
        }
        

        public void LoadData(SpecBase lib)
        {
            this._lib = lib;
            this.ksSetControl1.LoadData(lib, lib);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
