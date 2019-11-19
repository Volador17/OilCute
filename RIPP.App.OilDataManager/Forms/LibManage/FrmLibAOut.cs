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
using System.Threading;
using RIPP.OilDB.Data.DataCheck;
namespace RIPP.App.OilDataManager.Forms.LibManage
{
    public partial class FrmLibAOut : Form 
    {
        #region "私有变量"
        private OilInfoBll _oilInfoBll = new OilInfoBll();
        private string _sqlWhere = "1=1";
        private bool _selectAll = false;//用于datagridview的全部选择
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
        #endregion 

        #region 等待线程
        /// <summary>
        /// 等待窗口
        /// </summary>
        private FrmWaiting myFrmWaiting;

        /// <summary>
        /// 等待线程
        /// </summary>
        private Thread waitingThread;

        /// <summary>
        /// 等待线程
        /// </summary>
        public void Waiting()
        {
            this.myFrmWaiting = new FrmWaiting();
            this.myFrmWaiting.ShowDialog();
        }
        /// <summary>
        /// 开始等待线程
        /// </summary>
        public void StartWaiting()
        {
            this.waitingThread = new Thread(new ThreadStart(this.Waiting));
            this.waitingThread.Start();
        }

        /// <summary>
        /// 结束等待线程
        /// </summary>
        private void StopWaiting()
        {
            if (this.waitingThread != null)
            {
                if (myFrmWaiting != null)
                {
                    Action ac = () => myFrmWaiting.Close();
                    myFrmWaiting.Invoke(ac);
                }
                this.waitingThread.Abort();
            }
        }

        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmLibAOut()
        {
            InitializeComponent();          
            this.Text = "导出原始库";
            GridListBind();
        }
        #endregion 
        /// <summary>
        /// 表格控件绑定
        /// </summary>
        private void GridListBind()
        {
            dgvHeader.SetMangerDataBaseAColHeader(this.gridList ,false ,true);
            this.gridList.Rows.Clear();
            IList<OilInfoEntity> oilInfo = _oilInfoBll.dbGet(_sqlWhere);

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnAdd_Click(object sender, EventArgs e)
        {
            if (selectOilCheck() == 0)
            {
                MessageBox.Show("请选择需要导出的原油!");
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "原油数据文件 (*.libA)|*.libA";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.StartWaiting();
                    outLib(saveFileDialog.FileName);                  
                }
                catch (Exception ex)
                {
                    Log.Error("原油导出错误！" + ex.ToString());
                    return;
                }
                finally
                {
                    this.StopWaiting();
                }
                MessageBox.Show("原油导出成功！");
            }
        }

        /// <summary>
        /// 统计导出A库时选择的原油条数,若未选择，提示用户选择
        /// </summary>
        /// <returns></returns>
        private int selectOilCheck()
        {
            int result = 0;
            foreach (DataGridViewRow row in this.gridList.Rows)
            {
                if (bool.Parse(row.Cells["select"].EditedFormattedValue.ToString()))
                {
                    result++;
                }
            }
            return result;
        }
        /// <summary>
        /// 导出Lib
        /// </summary>
        /// <param name="fileName"></param>
        private void outLib(string fileName)
        {
            this.gridList.EndEdit();
            string strWhere = "";   //获取选中的原油ID，用于在oilInfo表中获取数据
            string strWhere2 = "";  //获取选中的原油ID,用于在oilData表中获取数据
            foreach (DataGridViewRow row in this.gridList.Rows)
            {
                if (bool.Parse(row.Cells["select"].Value.ToString()) == true)
                {
                    strWhere += " or ID=" + int.Parse(row.Cells["ID"].Value.ToString());
                    strWhere2 += " or oilInfoID=" + int.Parse(row.Cells["ID"].Value.ToString());
                }
            }
            strWhere = strWhere.Trim().Substring(2);
            strWhere2 = strWhere2.Trim().Substring(2);

            OilInfoOutAccess oilInfoOutAccess = new OilInfoOutAccess();
            List<OilInfoOut> oilInfoOuts = oilInfoOutAccess.Get(strWhere);  //oilInfo表中获取数据

            OilDataOutAccess acess = new OilDataOutAccess();
            List<OilDataOut> oilDataAlls = acess.Get(strWhere2);          //在oilData表中获取数据

            OutLib outLib = new OutLib();
            OilTableRowOutAccess oilTableRowAccess = new OilTableRowOutAccess();
            OilTableColOutAccess oilTableColAccess = new OilTableColOutAccess();
            outLib.oilTableRows = oilTableRowAccess.Get("1=1");               //获取oilTableRow表中所有行
            outLib.oilTableCols = oilTableColAccess.Get("1=1");               //获取oilTableCol表中所有列

            foreach (OilInfoOut oilInfoOut in oilInfoOuts)  //构建一条一条的原油信息，形成原油列表
            {
                oilInfoOut.oilDatas = oilDataAlls.Where(c => c.oilInfoID == oilInfoOut.ID).ToList();
                outLib.oilInfoOuts.Add(oilInfoOut);
            }

            Serialize.Write<OutLib>(outLib, fileName);
        }

        /// <summary>
        /// 全部选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnSelect_Click(object sender, EventArgs e)
        {
            this.gridList.EndEdit();
            if (_selectAll == false)
            {
                foreach(DataGridViewRow row in this.gridList.Rows)
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
        /// <summary>
        /// 行标题
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
    } 
}
