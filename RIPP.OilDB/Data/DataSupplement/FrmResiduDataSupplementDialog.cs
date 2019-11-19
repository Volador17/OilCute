using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.GCBLL;

namespace RIPP.OilDB.Data.DataSupplement
{
    public partial class FrmResiduDataSupplementDialog : Form
    {
        #region "私有变量"
        private GridOilViewA _wholeGridOil = null;
        private GridOilViewA _lightGridOil = null;
        private GridOilViewA _gcGridOil = null;
        private GridOilViewA _narrowGridOil = null;
        private GridOilViewA _wideGridOil = null;
        private GridOilViewA _residueGridOil = null;
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmResiduDataSupplementDialog()
        {
            InitializeComponent();
            this.button1.DialogResult = DialogResult.OK;
            this.button2.DialogResult = DialogResult.Cancel;           
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="wholeGridOil"></param>
        /// <param name="lightGridOil"></param>
        /// <param name="GCGridOil"></param>
        /// <param name="narrowGridOil"></param>
        /// <param name="wideGridOil"></param>
        /// <param name="residueGridOil"></param>
        public void Init(GridOilViewA wholeGridOil, GridOilViewA lightGridOil, GridOilViewA GCGridOil, GridOilViewA narrowGridOil, GridOilViewA wideGridOil, GridOilViewA residueGridOil)
        {
            this._wholeGridOil = wholeGridOil;
            this._lightGridOil = lightGridOil;
            this._gcGridOil = GCGridOil;
            this._narrowGridOil = narrowGridOil;
            this._wideGridOil = wideGridOil;
            this._residueGridOil = residueGridOil;

            this.comboBox1.DataSource = GetComboBoxDropItems();
            this.comboBox1.DisplayMember = "calData";
            this.comboBox1.ValueMember = "ColumnIndex";

            this.comboBox2.DataSource = GetComboBoxDropItems();
            this.comboBox2.DisplayMember = "calData";
            this.comboBox2.ValueMember = "ColumnIndex";

            this.comboBox3.DataSource = GetComboBoxDropItems();
            this.comboBox3.DisplayMember = "calData";
            this.comboBox3.ValueMember = "ColumnIndex";

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<OilDataEntity> GetComboBoxDropItems()
        {
            #region "数据筛选"
            
            List<OilDataEntity> DropList = new List<OilDataEntity>();//制作下拉菜单数据列          
            List<OilDataEntity> residueOilDataList = this._residueGridOil.GetAllData();
            if (residueOilDataList.Count <= 0)
                return DropList;
            List<OilDataEntity> oilDataList = residueOilDataList.Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> ICPOilDataList = oilDataList.Where(o => o.OilTableRow.itemCode == "ICP").ToList();

            foreach (OilDataEntity oilDataICP in ICPOilDataList)
            {
                string ICP = oilDataICP == null ? "" : oilDataICP.calData;;

                if (ICP != string.Empty)
                {
                    OilDataEntity temp = new OilDataEntity();
                    temp.calData = ICP ;
                    temp.ColumnIndex = oilDataICP.ColumnIndex;
                    DropList.Add(temp);
                }
            }

            #endregion

            return DropList;
        }
        #endregion
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {          
            if (this.CCRradioButton1.Checked)
                GlobalResiduDataSupplementDialog._CCR = "0";
            else if (this.CCRradioButton2.Checked)
                GlobalResiduDataSupplementDialog._CCR = "1";
            else if (this.CCRradioButton3.Checked)
                GlobalResiduDataSupplementDialog._CCR = "2";

            if (this.METradioButton1.Checked)
                GlobalResiduDataSupplementDialog._MET = "0";
            else if (this.METradioButton2.Checked)
                GlobalResiduDataSupplementDialog._MET = "1";
            else if (this.METradioButton3.Checked)
                GlobalResiduDataSupplementDialog._MET = "2";

            if (this.APHradioButton1.Checked)
                GlobalResiduDataSupplementDialog._APH = "0";
            else if (this.APHradioButton2.Checked)
                GlobalResiduDataSupplementDialog._APH = "1";
            else if (this.APHradioButton3.Checked)
                GlobalResiduDataSupplementDialog._APH = "2";

            //this.Close();           
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 残炭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CCRradioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CCRradioButton3.Checked)
                this.comboBox1.Enabled = true;
            else
                this.comboBox1.Enabled = false;
        }
        /// <summary>
        /// 金属
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void METradioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.METradioButton3.Checked)
                this.comboBox2.Enabled = true;
            else
                this.comboBox2.Enabled = false;
        }
        /// <summary>
        /// 胶质、沥青质
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void APHradioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.APHradioButton3.Checked)
                this.comboBox3.Enabled = true;
            else
                this.comboBox3.Enabled = false;
        }
        /// <summary>
        /// 残炭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            OilDataEntity temp = (OilDataEntity)this.comboBox1.SelectedItem;
            if (temp != null)
            GlobalResiduDataSupplementDialog._CCRDrop = temp.ColumnIndex;
        }
        /// <summary>
        /// 金属
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            OilDataEntity temp = (OilDataEntity)this.comboBox2.SelectedItem;
            if (temp != null)
            GlobalResiduDataSupplementDialog._METDrop = temp.ColumnIndex;
        }
        /// <summary>
        /// 胶质、沥青质
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            OilDataEntity temp = (OilDataEntity)this.comboBox3.SelectedItem;
            if (temp != null)
            GlobalResiduDataSupplementDialog._APHDrop = temp.ColumnIndex;
        }
          
    }
}
