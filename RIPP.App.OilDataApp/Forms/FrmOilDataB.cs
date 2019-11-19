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

namespace RIPP.App.OilDataApp.Forms
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
        #endregion
        
        #region 构造函数        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oil">一条原油</param>
        public FrmOilDataB(OilInfoBEntity oilB)
        {
            InitializeComponent();
            this._oilB = oilB;
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
                this.gridOilInfoB1.ReadOnly = true;
                this.dgvWhole.ReadOnly = true;
                this.dgvGCLevel.ReadOnly = true;
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

        #region  "在新窗体中看看图"
        /// <summary>
        /// 在新窗体中看看图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btViewZool_Click(object sender, EventArgs e)
        {
            //if (!this.IsExistChildFrm("FrmCurve") && this._curve != null)
            //{
            //    FrmCurveB frm = new FrmCurveB(this._curve);
            //    frm.Text = this._curve.descript;
            //    frm.MdiParent = frmMain;
            //    frm.Show();
            //}
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
        #endregion
       
    }
}
