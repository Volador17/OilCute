using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Models;
using log4net;
using RIPP.Lib;

namespace RIPP.App.Chem.Forms.Model.Controls
{
    public partial class PLSSetControl : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        private PLSSubModel _Model;


        public PLSSubModel Model
        {
            set
            {
                this._Model = value;
                this.initArgus();
            }
            get
            {
               return this._Model;
            }
        }

        #region Interface
        public bool Save()
        {
            this.getArgus();
            if (this.OnFinished != null)
                this.OnFinished(this, null);

            return true;
        }

        public void SetVisible(bool tag)
        {
            this.Visible = tag;
        }

        public event EventHandler OnFinished;

        #endregion

        public PLSSetControl()
        {
            InitializeComponent();
        }

        private void initArgus()
        {
            this.panelANN.Dock = DockStyle.Fill;
            this.panelANN.Visible = this._Model.AnnType == PLSAnnEnum.ANN;
            
            //初始化几个dropdown
          

            this.annfunc1.Items.Clear();
            this.annfunc1.Items.Add(FuncActiveEnum.logsig.GetDescription());
            this.annfunc1.Items.Add(FuncActiveEnum.purelin.GetDescription());
            this.annfunc1.Items.Add(FuncActiveEnum.tansig.GetDescription());


            this.annfunc2.Items.Clear();
            this.annfunc2.Items.Add(FuncActiveEnum.logsig.GetDescription());
            this.annfunc2.Items.Add(FuncActiveEnum.purelin.GetDescription());
            this.annfunc2.Items.Add(FuncActiveEnum.tansig.GetDescription());


            this.annfuncTrain.Items.Clear();
            this.annfuncTrain.Items.Add(FuncTrainEnum.traingd.GetDescription());
            this.annfuncTrain.Items.Add(FuncTrainEnum.traingdm.GetDescription());
            this.annfuncTrain.Items.Add(FuncTrainEnum.trainlm.GetDescription());

            //设置值
            if (this._Model == null )
                return;
            this.anntypeNone.Checked = this._Model.AnnType == PLSAnnEnum.None;
            this.anntypeAnn.Checked = this._Model.AnnType == PLSAnnEnum.ANN;
            this.anntypeNone.Checked = this._Model.AnnType == PLSAnnEnum.FANN;

           
            this.panelANN.Visible = this._Model.AnnType == PLSAnnEnum.ANN;
            this.methodPLS.Checked = this._Model.Method == PLSMethodEnum.PLS1;
            this.methodPLSMix.Checked = this._Model.Method == PLSMethodEnum.PLSMix;

            if (this._Model.ANNAgrus != null)
            {
                this.annepolse.Text = this._Model.ANNAgrus.Epochs.ToString();
                this.annfunc1.SelectedIndex = (int)this._Model.ANNAgrus.F1;
                this.annfunc2.SelectedIndex = (int)this._Model.ANNAgrus.F2;
                this.annfuncTrain.SelectedIndex = (int)this._Model.ANNAgrus.FuncTrain;
                this.annIsGuaidFalse.Checked = !this._Model.ANNAgrus.IsGuard;
                this.annIsGuaidTrue.Checked = this._Model.ANNAgrus.IsGuard;
                this.annnumhidden.Text = this._Model.ANNAgrus.NumHidden.ToString();
                this.anntarget.Text = this._Model.ANNAgrus.Target.ToString();
                this.anntimerepeat.Text = this._Model.ANNAgrus.TimesRepeat.ToString();
                this.anntimesavg.Text = this._Model.ANNAgrus.TimesAvg.ToString();

            }


           
        }

        private void getArgus()
        {
            if (this.anntypeAnn.Checked)
                this._Model.AnnType = PLSAnnEnum.ANN;
            else if (this.anntypeFann.Checked)
                this._Model.AnnType = PLSAnnEnum.FANN;
            else
                this._Model.AnnType = PLSAnnEnum.None;
            this._Model.Method = this.methodPLS.Checked ? PLSMethodEnum.PLS1 : PLSMethodEnum.PLSMix;
          
            //ann
            this._Model.ANNAgrus.Epochs = Convert.ToUInt32(this.annepolse.Text);
            this._Model.ANNAgrus.F1 = (FuncActiveEnum)this.annfunc1.SelectedIndex;
            this._Model.ANNAgrus.F2 = (FuncActiveEnum)this.annfunc2.SelectedIndex;
            this._Model.ANNAgrus.FuncTrain = (FuncTrainEnum)this.annfuncTrain.SelectedIndex;
            this._Model.ANNAgrus.IsGuard = this.annIsGuaidTrue.Checked;
            this._Model.ANNAgrus.NumHidden = Convert.ToUInt32(this.annnumhidden.Text);
            this._Model.ANNAgrus.Target = Convert.ToDouble(this.anntarget.Text);
            this._Model.ANNAgrus.TimesAvg = Convert.ToUInt32(this.anntimesavg.Text);
            this._Model.ANNAgrus.TimesRepeat = Convert.ToUInt32(this.anntimerepeat.Text);



           

        }

        

        private void anntypeNone_CheckedChanged(object sender, EventArgs e)
        {
            this.panelANN.Visible = false;
        }
        private void anntypeAnn_CheckedChanged(object sender, EventArgs e)
        {
            this.panelANN.Visible = true;
        }

        private void anntypeFann_CheckedChanged(object sender, EventArgs e)
        {
            this.panelANN.Visible = false;
        }

        private void annIsGuaidTrue_CheckedChanged(object sender, EventArgs e)
        {
           // this.panel3.Visible = true;
        }

        private void annIsGuaidFalse_CheckedChanged(object sender, EventArgs e)
        {
       //     this.panel3.Visible = false;
        }

       
    }
}
