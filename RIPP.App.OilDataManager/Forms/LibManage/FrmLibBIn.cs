using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.Lib;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.DataCheck;
using RIPP.OilDB.UI.GridOil;
namespace RIPP.App.OilDataManager.Forms.LibManage
{
    public partial class FrmLibBIn : Form
    {
        #region "私有变量"
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
        public FrmLibBIn()
        {
            InitializeComponent();
            this.Text = "导入应用库";
            init();
            GridListBind();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "原油数据文件 (*.libB)|*.libB";
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
            dgvHeader.SetMangerDataBaseBColHeader(this.gridList, false, true);
            this.gridList.Rows.Clear();
            if (_outLib == null)
                return;
            IList<OilInfoOut> oilInfo = _outLib.oilInfoOuts;

            //绑定数据
            for (int i = 0; i < oilInfo.Count; i++)
            {
                string receiveDate = oilInfo[i].receiveDate == null ? string.Empty : oilInfo[i].receiveDate.Value.ToString(dateFormat);
                string updataDate = string.Empty;
                if (oilInfo[i].updataDate != string.Empty)
                {
                    var updataDateTime = oilDataCheck.GetDate(oilInfo[i].updataDate);
                    updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                }
                this.gridList.Rows.Add
                   (false, i, oilInfo[i].ID, 0,
                   oilInfo[i].crudeName,
                   oilInfo[i].englishName,
                   oilInfo[i].crudeIndex,
                   oilInfo[i].country,
                   oilInfo[i].region,
                   receiveDate, updataDate,
                   oilInfo[i].sourceRef,
                   oilInfo[i].type,
                   oilInfo[i].classification,
                   oilInfo[i].sulfurLevel,
                   oilInfo[i].acidLevel
                   );
            }
        }
        /// <summary>
        /// 导入到B库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnIn_Click(object sender, EventArgs e)
        {
            this.gridList.EndEdit();
            OilInfoBAccess oilInfoAccess = new OilInfoBAccess();
            LibManageBll libManageBll = new LibManageBll();
            string alert = "未导入的原油：";
            foreach (DataGridViewRow row in this.gridList.Rows)
            {
                if (bool.Parse(row.Cells["select"].Value.ToString()) == true)
                {
                    int oilInfoId = int.Parse(row.Cells["ID"].Value.ToString());
                    OilInfoOut oilInfoOut = this._outLib.oilInfoOuts.Where(c => c.ID == oilInfoId).FirstOrDefault();

                    OilInfoBEntity oilInfoBEntity = new OilInfoBEntity();
                    libManageBll.toOilInfoEntity(ref  oilInfoBEntity, oilInfoOut);  //转换为OilInfoEntity
                    oilInfoBEntity.ID = OilBll.saveInfo(oilInfoBEntity);

                    if (oilInfoBEntity.ID == -1)
                    {
                        try
                        {
                            #region "原油冲突"
                            DialogResult r = MessageBox.Show(oilInfoBEntity.crudeIndex + "原油已存在！是否要更新", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (r == DialogResult.Yes)
                            {
                                oilInfoAccess.Delete("crudeIndex='" + oilInfoBEntity.crudeIndex + "'");  //删除原油信息数据
                                oilInfoBEntity.ID = OilBll.save(oilInfoBEntity);    //重新插入原油信息

                                libManageBll.toOilDatas(ref oilInfoBEntity, oilInfoOut, this._outLib.oilTableRows, this._outLib.oilTableCols);
                                OilBll.saveTables(oilInfoBEntity);
                                libManageBll.toCurve(ref oilInfoBEntity, oilInfoOut.curves, _outLib.curveTypes);
                                OilBll.saveCurves(oilInfoBEntity);
                                libManageBll.toOilDataSearchs(ref oilInfoBEntity, oilInfoOut);
                                OilBll.saveSearchTable(oilInfoBEntity.OilDataSearchs);

                                DatabaseB.FrmOpenB frmOpenB = (DatabaseB.FrmOpenB)GetChildFrm("frmOpenB");
                                if (frmOpenB != null)  //如果打开原油库B的窗口存在，则更新
                                {
                                    frmOpenB.refreshGridList(false);
                                }

                                DatabaseC.FrmOpenC frmOpenC = (DatabaseC.FrmOpenC)GetChildFrm("frmOpenC");
                                if (frmOpenC != null)  //如果打开原油库C的窗口存在，则更新
                                {
                                    frmOpenC.refreshGridList();
                                }  
                            }
                            else
                            {
                                alert += oilInfoBEntity.crudeIndex + "  ";
                            }
                            #endregion 
                        }
                        catch (Exception ex)
                        {
                            Log.Error("原油导入错误！" + ex.ToString());
                            return;
                        }

                        MessageBox.Show(oilInfoBEntity.crudeName + "原油导入成功！");
                    }
                    else
                    {
                        #region "原油无冲突"
                        try
                        {
                            libManageBll.toOilDatas(ref oilInfoBEntity, oilInfoOut, this._outLib.oilTableRows, this._outLib.oilTableCols);
                            OilBll.saveTables(oilInfoBEntity);
                            libManageBll.toCurve(ref oilInfoBEntity, oilInfoOut.curves, _outLib.curveTypes);
                            OilBll.saveCurves(oilInfoBEntity);
                            libManageBll.toOilDataSearchs(ref oilInfoBEntity, oilInfoOut);
                            OilBll.saveSearchTable(oilInfoBEntity.OilDataSearchs);

                            DatabaseB.FrmOpenB frmOpenB = (DatabaseB.FrmOpenB)GetChildFrm("frmOpenB");
                            if (frmOpenB != null)  //如果打开原油库A的窗口存在，则更新
                            {
                                frmOpenB.refreshGridList(false);
                            }  
                        }
                        catch (Exception ex)
                        {
                            Log.Error("原油导入错误！" + ex.ToString());
                            return;
                        }

                        MessageBox.Show( oilInfoBEntity.crudeName +"原油导入成功！");

                        #endregion 
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
        private void toolStripBtnSeleteAll_Click(object sender, EventArgs e)
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