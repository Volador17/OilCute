using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil;
using RIPP.App.OilDataManager.Forms.DatabaseA;
using RIPP.OilDB.Data.DataCheck;
namespace RIPP.App.OilDataManager.Forms.LibManage
{
    public partial class FrmLibAIn : Form
    {
        #region "私有变量"
        /// <summary>
        /// 导入信息
        /// </summary>
        private OutLib _outLib = null;
        /// <summary>
        /// 日期格式
        /// </summary>
        private const string dateFormat = "yyyy-MM-dd";
        /// <summary>
        /// 日期格式
        /// </summary>
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        private OilDataCheck oilDataCheck = new OilDataCheck();
        private DgvHeader dgvHeader = new DgvHeader();

        private bool _selectAll = false;//用于datagridview的全部选择
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmLibAIn()
        {
            InitializeComponent();
            this.Text = "导入原始库";
            init();
            GridListBind();
        }
        /// <summary>
        /// 
        /// </summary>
        private void init()
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "原油数据文件 (*.libA)|*.libA";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _outLib = Serialize.Read<OutLib>(saveFileDialog.FileName);
                this.Visible = true;
            }
            else
                this.Visible = false;
        }
        #endregion 
     
        /// <summary>
        /// 表格控件绑定
        /// </summary>
        private void GridListBind()
        {
            dgvHeader.SetMangerDataBaseAColHeader(this.gridList, false, true);
            this.gridList.Rows.Clear();
            if (this._outLib == null)
                return;
            IList<OilInfoOut> oilInfo = _outLib.oilInfoOuts;
            
            //绑定数据
            for (int i = 0; i < oilInfo.Count; i++)
            {
                string sampleDate = oilInfo[i].sampleDate == null ? string.Empty : oilInfo[i].sampleDate.Value.ToString(dateFormat);
                string receiveDate = oilInfo[i].receiveDate == null ? string.Empty : oilInfo[i].receiveDate.Value.ToString(dateFormat);
                
                string assayDate = string.Empty;
                if (oilInfo[i].assayDate != string.Empty)
                {
                    var assayDateTime = oilDataCheck.GetDate(oilInfo[i].assayDate);
                    assayDate = assayDateTime == null ? string.Empty : assayDateTime.Value.ToString(dateFormat);
                }

                string updataDate = string.Empty;
                if (oilInfo[i].assayDate != string.Empty)
                {
                    var updataDateTime = oilDataCheck.GetDate(oilInfo[i].updataDate);
                    updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                }
                this.gridList.Rows.Add(false, i, oilInfo[i].ID, 0,
                     oilInfo[i].crudeName, oilInfo[i].englishName, oilInfo[i].crudeIndex, oilInfo[i].country,
                     oilInfo[i].region, oilInfo[i].fieldBlock, sampleDate, receiveDate,
                     oilInfo[i].sampleSite, assayDate, updataDate, oilInfo[i].sourceRef,
                     oilInfo[i].assayLab, oilInfo[i].assayer, oilInfo[i].reportIndex,
                     oilInfo[i].type, oilInfo[i].classification, oilInfo[i].sulfurLevel, oilInfo[i].acidLevel,
                     oilInfo[i].corrosionLevel, oilInfo[i].processingIndex);

              }
        }



        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnIn_Click(object sender, EventArgs e)
        {
            this.gridList.EndEdit();

            OilInfoAccess oilInfoAccess = new OilInfoAccess();
            LibManageBll libManageBll = new LibManageBll();
            string alert = "未导入的原油：";
            foreach (DataGridViewRow row in this.gridList.Rows)
            {
                #region
                if (bool.Parse(row.Cells["select"].Value.ToString()) == true)
                {
                    int oilInfoId = int.Parse(row.Cells["ID"].Value.ToString());
                    OilInfoOut oilInfoOut = this._outLib.oilInfoOuts.Where(c => c.ID == oilInfoId).FirstOrDefault();

                    OilInfoEntity oilInfoEntity = new OilInfoEntity();
                    libManageBll.toOilInfoEntity(ref  oilInfoEntity, oilInfoOut);  //转换为OilInfoEntity
                    oilInfoEntity.ID = OilBll.save(oilInfoEntity);

                    if (oilInfoEntity.ID == -1)
                    {
                        DialogResult r = MessageBox.Show(oilInfoEntity.crudeIndex + "原油已存在！是否要更新", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (r == DialogResult.Yes)
                        {
                            oilInfoAccess.Delete("crudeIndex='" + oilInfoEntity.crudeIndex + "'");  //删除原油信息数据
                            oilInfoEntity.ID = OilBll.save(oilInfoEntity);    //重新插入原油信息

                            libManageBll.toOilDatas(ref oilInfoEntity, oilInfoOut, this._outLib.oilTableRows, this._outLib.oilTableCols);
                            OilBll.saveTables(oilInfoEntity);

                            DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)GetChildFrm("frmOpenA");
                            if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
                            {
                                frmOpenA.refreshGridList(false);
                            }
                        }
                        else
                        {
                            alert += oilInfoEntity.crudeIndex + "  ";
                        }
                    }
                    else
                    {
                        try
                        {
                            libManageBll.toOilDatas(ref oilInfoEntity, oilInfoOut, this._outLib.oilTableRows, this._outLib.oilTableCols);
                            OilBll.saveTables(oilInfoEntity);

                            DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)GetChildFrm("frmOpenA");
                            if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
                            {
                                frmOpenA.refreshGridList(false);
                            }
                        }
                        catch (Exception ex)
                        {
                             Log.Error("原油导入错误！" + ex.ToString());
                             return;
                        }
                        MessageBox.Show(oilInfoEntity.crudeName + "原油导入成功！");
                    }
                }
                #endregion
            }

            if (alert != "未导入的原油：")
            {
                MessageBox.Show(alert, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
            e.RowBounds.Location.Y,
            this.gridList.RowHeadersWidth - 4,
            e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.gridList.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.gridList.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

        }

        /// <summary>
        /// 全部选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnSelectAll_Click(object sender, EventArgs e)
        {
            this.gridList.EndEdit();
            if (_selectAll == false)
            {
                foreach (DataGridViewRow row in this.gridList.Rows)
                {
                    row.Cells["select"].Value = true;
                }
                this.gridList.Refresh();
                _selectAll = true;
            }
            else
            {
                foreach (DataGridViewRow row in this.gridList.Rows)
                {
                    row.Cells["select"].Value = false;
                }
                this.gridList.Refresh();
                _selectAll = false;
            }
        }
    }
}