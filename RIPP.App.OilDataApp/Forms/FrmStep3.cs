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
using RIPP.Lib;
namespace RIPP.App.OilDataApp.Forms
{
    public partial class FrmStep3 : Form
    {
        #region "私有参数"
        /// <summary>
        /// 主窗体
        /// </summary>
        private FrmMain _frmMain;
        /// <summary>
        /// 导出的切割方案
        /// </summary>
        private CutMothedOutLib _outLib = null;
        /// <summary>
        /// 切割方案
        /// </summary>
        private List<CutMothedEntity> _CutMotheds =  new List<CutMothedEntity> ();//切割方案
        #endregion 
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="frmMain">主窗体</param>
        public FrmStep3(FrmMain frmMain)
        {
            InitializeComponent();
            this._frmMain = frmMain;
            InitGrid();
            if (this._CutMotheds.Count > 0)
            {
                for (int i = 0; i < this._CutMotheds.Count; i++)
                {
                    this.gridListCut.Rows[i].Cells["ICP"].Value = this._CutMotheds[i].ICP;
                    this.gridListCut.Rows[i].Cells["ECP"].Value = this._CutMotheds[i].ECP;
                    this.gridListCut.Rows[i].Cells["Name"].Value = this._CutMotheds[i].Name;
                    this.gridListCut.Rows[i].Cells["CutType"].Value = this._CutMotheds[i].CutType;
                }
            }
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        private void InitGrid()
        {         
            SetColHeader();//设定列头       
            SetRow();
            //if (this._frmMain.CutMotheds.Count == 0)
            //{
            //    this.gridListCut.Rows.Add("15", "180", "Cut1");
            //    this.gridListCut.Rows.Add("140", "240", "Cut2");
            //    this.gridListCut.Rows.Add("180", "350", "Cut3");
            //    this.gridListCut.Rows.Add("350", "540", "Cut4");
            //    this.gridListCut.Rows.Add("540", "1600", "Cut5");
            //    //this.gridListCut.Rows.Clear();//清空以前存在的行
            //    //this.gridListCut.Rows.Add(1);
            //}
            //else
            //{
            //    this.gridListCut.Rows.Clear();//清空以前存在的行
              
            //    for (int i = 0; i < this._frmMain.CutMotheds.Count; i++)
            //    {
            //        DataGridViewRow row = new DataGridViewRow();
            //        row.CreateCells(this.gridListCut, this._frmMain.CutMotheds[i].ICP, this._frmMain.CutMotheds[i].ECP, this._frmMain.CutMotheds[i].name);
            //        this.gridListCut.Rows.Add(row);
            //    }
            //    this.gridListCut.Rows.Add(1);
            //}
        }

        /// <summary>
        /// 设置表头
        /// </summary>
        private void SetColHeader()
        {
            //清除表的行和列
            this.gridListCut.Rows.Clear();
            this.gridListCut.Columns.Clear();

            #region 添加表头       
            this.gridListCut.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "初馏点", Name = "ICP" });
            this.gridListCut.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "终馏点", Name = "ECP" });
            this.gridListCut.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "馏分段名称", Name = "Name" });
            this.gridListCut.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "切割馏分类型", Name = "CutType" });
            #endregion
        }

        /// <summary>
        /// 设置表头
        /// </summary>
        private void SetRow()
        {
            //清除表的行和列
            #region 添加行
            this.gridListCut.Rows.Clear();//清空以前存在的行
       
            this.gridListCut.Rows.Add(16);
            #endregion
        }

        /// <summary>
        /// 确定按钮，把原油的切割方案添加到主窗体切割方案实体变量中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextStep_Click(object sender, EventArgs e)
        {
            this._frmMain.CutMotheds.Clear();
            this._CutMotheds.Clear();
            bool resultMothed = getCutMotheds();
            if (resultMothed)
            {
                this._frmMain.CutMotheds = this._CutMotheds;
                this.Close();
            }
        }
        /// <summary>
        /// 获取切割方案
        /// </summary>
        private bool  getCutMotheds()
        {
            List<string> NameList = new List<string>();

            foreach (DataGridViewRow row in this.gridListCut.Rows)
            {
                CutMothedEntity cutMothed = new CutMothedEntity();

                #region "条件判断"
                object objICP = row.Cells["ICP"].Value;
                object objECP = row.Cells["ECP"].Value;
                object objName = row.Cells["Name"].Value;
                object objCutType = row.Cells["CutType"].Value;
                float tempICP = 0; float tempECP = 0; string strName = string.Empty; string strCutType = string.Empty;
                if (objICP == null && objECP == null && objName == null)
                    return true ;

                if (objICP == null && objECP == null)
                {
                    MessageBox.Show("馏分段数据不能为空!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (objName == null)
                {
                    MessageBox.Show("馏分段名称不能为空!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false ;
                }

               
                if (objICP != null)
                {
                    if (float.TryParse(objICP.ToString(), out tempICP))
                    {
                        //MessageBox.Show(row.Cells["馏分段名称"].Value.ToString() + "馏分段的初馏点为不正常数据!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //return;
                    }
                    else
                    {
                        MessageBox.Show(objName.ToString() + "馏分段的初留点为非数据!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                else
                {
                    //if (this._frmMain.Oil.ICP0 != string.Empty)
                        //tempICP = Convert.ToSingle (this._frmMain.Oil.ICP0);
                        tempICP = -2000;
                }

                if (objECP != null)
                {
                    if (float.TryParse(objECP.ToString(), out tempECP))
                    {
                        //MessageBox.Show(row.Cells["馏分段名称"].Value.ToString() + "馏分段的初馏点为不正常数据!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //return;
                    }
                    else
                    {
                        MessageBox.Show(objName.ToString() + "馏分段的终留点为非数据!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                else
                {
                    tempECP = 2000;
                }
                #endregion 
 
                if (tempICP >= tempECP)
                {
                    MessageBox.Show("终馏点的值应大于初馏点的值", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                if (NameList.Contains(objName.ToString()))
                {
                    MessageBox.Show("馏分段名称不能重复", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                else if (!NameList.Contains(objName.ToString()) && tempICP < tempECP)
                {
                    NameList.Add(objName.ToString());
                    cutMothed.ICP = tempICP;
                    cutMothed.ECP = tempECP;
                    cutMothed.Name = objName.ToString();
                    cutMothed.CutType = objCutType == null ? string.Empty : objCutType.ToString();
                    this._CutMotheds.Add(cutMothed);
                }              
            }

            return true;
        }
        /// <summary>
        /// 添加表格的行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnAdd_Click(object sender, EventArgs e)
        {
            this.gridListCut.Rows.Add(1);
        }

        /// <summary>
        /// 删除表格的行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnDelete_Click(object sender, EventArgs e)
        {
            if (this.gridListCut.CurrentRow != null)
                this.gridListCut.Rows.Remove(this.gridListCut.CurrentRow);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 新建切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.gridListCut.Rows.Clear();
            SetRow();
            this._frmMain.CutMotheds.Clear();
            this._CutMotheds.Clear();
        }
        /// <summary>
        /// 读取切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "切割方案文件 (*.cut)|*.cut";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._outLib = Serialize.Read<CutMothedOutLib>(saveFileDialog.FileName);
                SetRow();
                if (this.gridListCut.Rows.Count < this._outLib.CutMotheds.Count)
                    this.gridListCut.Rows.Add(this._outLib.CutMotheds.Count - this.gridListCut.Rows.Count);
                for (int i = 0; i < this._outLib.CutMotheds.Count; i++)
                {
                    if (this._outLib.CutMotheds[i].ICP != -2000)
                    {
                        this.gridListCut.Rows[i].Cells["ICP"].Value = this._outLib.CutMotheds[i].ICP;
                        this.gridListCut.Rows[i].Cells["ECP"].Value = this._outLib.CutMotheds[i].ECP;
                        this.gridListCut.Rows[i].Cells["Name"].Value = this._outLib.CutMotheds[i].Name;
                        this.gridListCut.Rows[i].Cells["CutType"].Value = this._outLib.CutMotheds[i].CutType;
                    }
                    else
                    {
                        this.gridListCut.Rows[i].Cells["ICP"].Value = null;
                        this.gridListCut.Rows[i].Cells["ECP"].Value = this._outLib.CutMotheds[i].ECP;
                        this.gridListCut.Rows[i].Cells["Name"].Value = this._outLib.CutMotheds[i].Name;
                        this.gridListCut.Rows[i].Cells["CutType"].Value = this._outLib.CutMotheds[i].CutType;
                    }
                }
            }
            else
            {
                return;
            }          
        }
        /// <summary>
        /// 保存切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (getCutMotheds())
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "切割方案文件 (*.cut)|*.cut";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    outCut(saveFileDialog1.FileName);
                }
            }
        }

        private void outCut(string fileName)
        {
            //this.gridList.EndEdit();
            CutMothedOutLib outLib = new CutMothedOutLib();
            outLib.CutMotheds = this._CutMotheds;
            Serialize.Write<CutMothedOutLib>(outLib, fileName);
        }
    }
}
