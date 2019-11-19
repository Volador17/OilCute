using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using System.Threading;
using ZedGraph;
using RIPP.Lib;
namespace RIPP.App.OilDataManager.Forms.DatabaseB
{
    public partial class FrmOilDataB : Form
    {
        #region "私有变量"
        /// <summary>
        /// 新打开的一条B库原油
        /// </summary>
        public OilInfoBEntity _oilB = null;   //一条原油  
        /// <summary>
        /// 打开的一条曲线
        /// </summary>
        private CurveEntity _curve;//选择一条曲线
        /// <summary>
        /// 判断验证是否通过
        /// </summary>
        private bool _errValidate = false;     //是否验证通过 
        /// <summary>
        /// 判断表格中的数据是否有改动
        /// </summary>
        private bool _isChanged = false; //是否有改动
        /// <summary>
        /// 是否有改动 
        /// </summary>
        public bool isChanged
        {
            get
            {
                if (this.gridOilInfoB1.isChanged || this.dgvWhole.NeedSave  || this.dgvGCLevel.NeedSave)
                    this._isChanged = true;

                return _isChanged;
            }
            set
            {
                this._isChanged = value;
            }
        }
        /// <summary>
        /// 上一个选项卡的名称
        /// </summary>
        private string beforSelectTabText = string.Empty;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oil">一条原油</param>
        public FrmOilDataB(OilInfoBEntity oil)
        {
            InitializeComponent();
            this._oilB = oil;
            initTab();
            BindcmbType();
            BindcmbCurve(Convert.ToInt32(this.cmbType.SelectedValue.ToString()));
            this.Text = "应用库原油数据-" + this._oilB.crudeIndex;
            this.Name = this._oilB.crudeIndex + "B";   //在打开操作时要根据该名称判断窗口是否存在
        }
        /// <summary>
        /// 选项卡数据绑定
        /// </summary>
        private void initTab()
        {
            if (_oilB.ID > 0)
            {
                this.gridOilInfoB1.FillOilInfoB(this._oilB);
                this.dgvWhole.InitTable(this._oilB, EnumTableType.Whole);
                this.dgvGCLevel.InitTable(this._oilB, EnumTableType.GCLevel);
                this.tabControl1.TabPages.Remove(this.tabPage3);
            }
        }
        #endregion

        #region "下拉菜单事件"
        /// <summary>
        /// 绑定曲线类型
        /// </summary>
        private void BindcmbType()
        {
            this.cmbType.DisplayMember = "typeName";
            this.cmbType.ValueMember = "ID";
            this.cmbType.DataSource = _oilB.curveTypes;
        }

        /// <summary>
        /// 根据曲线类型ID，找到某类型的所有曲线绑定
        /// </summary>
        private void BindcmbCurve(int curveTypeID)
        {
            List<CurveEntity> curves = _oilB.curves.Where(o => o.curveTypeID == curveTypeID && o.oilInfoID == this._oilB.ID).ToList();
            if (curves.Count > 0 && curves != null)
            {
                this.cmbCurve.DisplayMember = "propertyY";
                this.cmbCurve.ValueMember = "ID";
                this.cmbCurve.DataSource = curves;
            }
            else
            {
                this.cmbCurve.DataSource = null;
            }
        }

        /// <summary>
        /// 曲线类别下拉列表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int curveTypeID = Convert.ToInt32(this.cmbType.SelectedValue.ToString());
            BindcmbCurve(curveTypeID);
        }
        /// <summary>
        /// 选择曲线下拉列表 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCurve_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbType.SelectedValue == null || this.cmbCurve.SelectedValue == null)
            {
                this._curve = null;
            }
            else
            {
                int curveTypeID = Convert.ToInt32(this.cmbType.SelectedValue.ToString());
                int curveID = Convert.ToInt32(this.cmbCurve.SelectedValue.ToString());

                this._curve = _oilB.curves.Where(o => o.ID == curveID && o.curveTypeID == curveTypeID).FirstOrDefault();
            }
            this.propertyGraph1.DrawCurve(this._curve);
        }
        #endregion 


        /// <summary>
        /// 保存B库
        /// </summary>
        public bool SaveB()
        {
            this.gridOilInfoB1.EndEdit();
            this.dgvGCLevel.EndEdit ();
            this.dgvWhole.EndEdit();

            DataCheck();
            this._errValidate = true;
            if (this._errValidate)
            {
                this.Save();
                this.Text = "原始库原油数据-" + this._oilB.crudeIndex;
                this.Name = this._oilB.crudeIndex + "A";
                return true;
            }
            else
            {
                MessageBox.Show("数据错误，不能保存!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public void Save()
        {
            //if(this.gridOilInfoB1.isChanged == true )    ///isChanged不起作用
                this.gridOilInfoB1.Save();

            if (this.dgvWhole.NeedSave == true)
                this.dgvWhole.Save();

            if (this.dgvGCLevel.NeedSave == true)
                this.dgvGCLevel.Save();
        }

        /// <summary>
        /// 检查关键项
        /// </summary>
        /// <returns></returns>
        public void DataCheck()
        {
            //string str = this.oilTab1.DataCheck();

            // this._errValidate = str == string .Empty? true : false;
        }
        /// <summary>
        /// 在新窗体中看看图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btViewZool_Click(object sender, EventArgs e)
        {
            //打开子窗口
            FrmMain frmMain = (FrmMain)this.MdiParent;
            if (!frmMain.IsExistChildFrm("FrmCurve") && this._curve != null)
            {
                DatabaseB.FrmCurveB frm = new DatabaseB.FrmCurveB(this._curve);
                frm.Text = this._curve.descript;
                frm.MdiParent = frmMain;
                frm.Show();
            }
        }
        /// <summary>
        /// 关闭窗口时保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmOilDataB_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.isChanged == true)
            {
                DialogResult r = MessageBox.Show("是否保存" + this._oilB.crudeIndex + "数据到应用库？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    this.SaveB();
                    FrmMain frmMain = (FrmMain)this.MdiParent;
                    DatabaseB.FrmOpenB frmOpenB = (DatabaseB.FrmOpenB)frmMain.GetChildFrm("frmOpenB");
                    if (frmOpenB != null)  //如果打开原油库B的窗口存在，则更新
                    {
                        frmOpenB.refreshGridList(false);
                    }

                    this.isChanged = false;
                }
            }
        }
        /// <summary>
        /// 保存到快速查询库
        /// </summary>
        private void SaveC()
        { 
             DialogResult r = MessageBox.Show("是否保存数据到快速查询库！", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
             if (r == DialogResult.Yes)
                 OilBll.SaveC(this._oilB);     
        }
        /// <summary>
        /// 选项卡的名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            beforSelectTabText = this.tabControl1.SelectedTab.Text;
        }  
        /// <summary>
        /// 向B库保存新数据时检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (beforSelectTabText == EnumTableType.Info.GetDescription())
            {
                if (!this.gridOilInfoB1.isChanged)
                    return;
                if (this.gridOilInfoB1.Save() < 0)
                {
                    e.Cancel = true;
                    MessageBox.Show("原油信息表中的原油编号为空或重复!必须先填写。", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    DatabaseB.FrmOpenB frmOpenB = (DatabaseB.FrmOpenB)GetChildFrm("frmOpenB");
                    if (frmOpenB != null)  //如果打开原油库B的窗口存在，则更新
                    {
                        frmOpenB.refreshGridList(false);
                    }
                }
            }    
        }

        /// <summary>
        /// 根据窗体名称获取窗体
        /// </summary>
        /// <param name="childFrmName">窗体名称</param>
        /// <returns>存在返回1并不激活此窗口，不存在返回null</returns>
        public Form GetChildFrm(string childFrmName)
        {
            Form childFrm = null;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == childFrmName)
                {
                    childFrm = frm;
                    break;
                }
            }
            return childFrm;
        }
          
    }
}
