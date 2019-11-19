using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.Lib;
using System.Drawing;
using System.IO;

namespace RIPP.App.OilDataApp.Forms
{
     /// <summary>
    /// step3的代码
    /// </summary>
    public partial class FrmMain
    {
        #region "私有参数"
     
        /// <summary>
        /// 切割方案编辑
        /// </summary>
        private GridOilDataEdit dataEdit = new GridOilDataEdit();
        private List<CutMothedEntity> outCutMothedList = new List<CutMothedEntity>();
        #endregion 

        #region 
        /// <summary>
        /// 初始化step3表格控件
        /// </summary>
        private void InitStep3Grid()
        {
            SetStep3ColHeader();//设定列头       
            SetRow();
            this.gridListCut.MultiSelect = true;
        }

        /// <summary>
        /// 设置表头
        /// </summary>
        private void SetStep3ColHeader()
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
        /// 切割方案添加行头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridListCut_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
           e.RowBounds.Location.Y, this.gridListCut.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.gridListCut.RowHeadersDefaultCellStyle.Font,
            rectangle,
            this.gridListCut.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

        }    
        /// <summary>
        /// 设置表头
        /// </summary>
        private void SetRow()
        {
            //清除表的行和列
            #region 添加行
            this.gridListCut.Rows.Clear();//清空以前存在的行

            this.gridListCut.Rows.Add(100);
            #endregion
        }
        /// <summary>
        /// 剪贴
        /// </summary>
        private void Cut()
        {
            GridOilDataEdit.CopyToClipboard(this.gridListCut);
            //从输入列表和数据库中删除数据
            GridOilDataEdit.DeleteValues(this.gridListCut);
           
        }
        /// <summary>
        /// 粘帖
        /// </summary>
        private void Paste()
        {
            GridOilDataEdit.PasteClipboardValue(this.gridListCut);
        }
        /// <summary>
        /// 删除
        /// </summary>
        private void Delete()
        {
            GridOilDataEdit.DeleteValues(this.gridListCut);
            
        }
        /// <summary>
        /// 切割方案快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridListCut_KeyDown(object sender, KeyEventArgs e)
        {
            this.gridListCut.EndEdit();
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.X:
                            Cut();
                            break;
                        case Keys.C:
                            GridOilDataEdit.CopyToClipboard(this.gridListCut);
                            break;
                        case Keys.V:
                            Paste();
                            break;
                    }
                }
                if (e.KeyCode == Keys.Delete)
                {
                    Delete();
                }
                
            }
            catch (Exception ex)
            {
                Log.Error("原油应用切割方案快捷键" + ex);
            }
        }
        private void gridListCut_KeyUp(object sender, KeyEventArgs e)
        {
            this.gridListCut.EndEdit();
            if (e.KeyData == Keys.Enter)
            {
                MoveNext();
            }
        }
        private void MoveNext()
        {
            int currentRowIndex = this.gridListCut.CurrentCell.RowIndex;
            int currentColIndex = this.gridListCut.CurrentCell.ColumnIndex;
            this.gridListCut.EndEdit();

            if (currentColIndex + 1 < this.gridListCut.Columns.Count)
                this.gridListCut.CurrentCell = this.gridListCut[currentColIndex +1, currentRowIndex];
            else if (currentColIndex + 1 == this.gridListCut.Columns.Count && currentRowIndex + 1 < this.gridListCut.Rows.Count)
                this.gridListCut.CurrentCell = this.gridListCut[0, currentRowIndex + 1];


            //if (currentRowIndex + 1 < this.gridListCut.Rows.Count)
            //    this.gridListCut.CurrentCell = this.gridListCut[currentColIndex, currentRowIndex +1];
            //else if (currentRowIndex + 1 < this.gridListCut.Rows.Count)
            //    this.gridListCut.CurrentCell = this.gridListCut[currentColIndex, currentRowIndex + 1];


            //this.gridListCut.en//第一种情况：只有一行,且当光标移到最后一列时,新增一行
            //if ((base.CurrentCell.ColumnIndex == (base.ColumnCount - 1)) && (base.RowCount == 1))
            //{
            //    //新增一行
            //    base.Rows.Add();
            //    base.CurrentCell = base.Rows[base.RowCount - 1].Cells[0];
            //    return true;
            //}
            ////第二种情况：有多行，且当光标移到最后一列时,移到下一行第一个单元,
            //if ((base.CurrentCell.ColumnIndex == (base.ColumnCount - 1)) && (base.CurrentCell.RowIndex < (base.RowCount - 1)))
            //{
            //    // base.Rows.Add();
            //    base.CurrentCell = base.Rows[base.CurrentCell.RowIndex + 1].Cells[0];
            //    return true;
            //}
            ////第三种情况：有多行，且当光标移到最后一行最后一列时,移到下一行第一个单元,新增一行
            //if ((base.CurrentCell.ColumnIndex == (base.ColumnCount - 1)) && (base.CurrentCell.RowIndex == base.RowCount - 1))
            //{
            //    //新增一行
            //    base.Rows.Add();
            //    base.CurrentCell = base.Rows[base.RowCount - 1].Cells[0];
            //    return true;
            //}
        }
        #endregion 
        /// <summary>
        /// 获取切割方案
        /// </summary>
        /// <returns>true 获取到切割方案，FALSE 获取失败</returns>
        private bool getCutMotheds(List<CutMothedEntity> cutMothedList)
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
                    return true;

                if (objICP == null && objECP == null)
                {
                    MessageBox.Show("馏分段数据不能为空!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (objName == null)
                {
                    MessageBox.Show("馏分段名称不能为空!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }


                if (objICP != null && !string.IsNullOrWhiteSpace( objICP.ToString()) )
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
 
                if (objECP != null && !string.IsNullOrWhiteSpace(objECP.ToString()))
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
                    cutMothedList.Add(cutMothed);
                }
            }
            return true;
        }
        
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStep3Ok_Click(object sender, EventArgs e)
        {
            this._cutMotheds.Clear();
            bool resultMothed = getCutMotheds(this._cutMotheds);
            if (resultMothed)
            {
                this.panelStep3.Visible = false;
                this.butStep4.Enabled = true;
            }
            else
            {
                MessageBox.Show("获取不到切割方案！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStep3Cancel_Click(object sender, EventArgs e)
        {
            this.panelStep3.Visible = false;
            this._cutMotheds.Clear();
        }
        #region "toolstrip按钮"
        /// <summary>
        /// 读取切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonReadCutMethod_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "切割方案文件 (*.cut)|*.cut";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                initAllCutMothed(saveFileDialog.FileName);
            }
            else
            {
                return;
            }     
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void initAllCutMothed(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    CutMothedOutLib outCutMothedLib = Serialize.Read<CutMothedOutLib>(fileName);
                    SetRow();
                    if (this.gridListCut.Rows.Count < outCutMothedLib.CutMotheds.Count)
                        this.gridListCut.Rows.Add(outCutMothedLib.CutMotheds.Count - this.gridListCut.Rows.Count);

                    for (int i = 0; i < outCutMothedLib.CutMotheds.Count; i++)
                    {
                        if (outCutMothedLib.CutMotheds[i].ICP != (int)enumCutMothedICPECP.ICPMin)
                            this.gridListCut.Rows[i].Cells["ICP"].Value = outCutMothedLib.CutMotheds[i].ICP;
                        else
                            this.gridListCut.Rows[i].Cells["ICP"].Value = null;

                        if (outCutMothedLib.CutMotheds[i].ECP == (int)enumCutMothedICPECP.ECPMax)
                            this.gridListCut.Rows[i].Cells["ECP"].Value = null;
                        else
                            this.gridListCut.Rows[i].Cells["ECP"].Value = outCutMothedLib.CutMotheds[i].ECP;

                        this.gridListCut.Rows[i].Cells["Name"].Value = outCutMothedLib.CutMotheds[i].Name;
                        this.gridListCut.Rows[i].Cells["CutType"].Value = outCutMothedLib.CutMotheds[i].CutType;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("文件内容有误！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error(""+ex.ToString());
                }
            }           
        }


        /// <summary>
        /// 保存切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSaveCutMethod_Click(object sender, EventArgs e)
        {
            this.outCutMothedList.Clear();
            if (getCutMotheds(outCutMothedList))
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "切割方案文件 (*.cut)|*.cut";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    outCut(saveFileDialog1.FileName);
                }
            }
        }
        /// <summary>
        /// 添加切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonNewCutMethod_Click(object sender, EventArgs e)
        {
            this.gridListCut.Rows.Clear();
            SetRow();
            this._cutMotheds.Clear();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDeleteCutMethod_Click(object sender, EventArgs e)
        {
            if (this.gridListCut.CurrentRow != null)
                this.gridListCut.Rows.Remove(this.gridListCut.CurrentRow);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonAddCutMethod_Click(object sender, EventArgs e)
        {
            this.gridListCut.Rows.Add(1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        private void outCut(string fileName)
        {
            this.gridListCut.EndEdit();
            getCutMotheds(this._cutMotheds);
            CutMothedOutLib outLib = new CutMothedOutLib();
            outLib.CutMotheds = outCutMothedList;
            Serialize.Write<CutMothedOutLib>(outLib, fileName);
        }
        #endregion 
    }
}
