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
using System.Threading;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data.DataCheck;
namespace RIPP.App.OilDataManager.Forms.LibManage
{
    public partial class FrmLibBOut : Form
    {
        #region "私有变量"
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
        public FrmLibBOut()
        {
            InitializeComponent();
            this.Text = "导出应用库";
            GridListBind();
        }

        /// <summary>
        /// 表格控件绑定
        /// </summary>
        private void GridListBind()
        {
            dgvHeader.SetMangerDataBaseBColHeader(this.gridList, false, true);
            this.gridList.Rows.Clear();
            OilInfoBAccess access = new OilInfoBAccess();
            IList<OilInfoBEntity> oilInfo = access.Get(_sqlWhere);

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
        #endregion 

        /// <summary>
        /// 导出事件
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
            saveFileDialog.Filter = "原油数据文件 (*.libB)|*.libB";
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
        /// 统计导出B库时选择的原油条数,若未选择，提示用户选择
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
        /// 以Lib的形式导出B库
        /// </summary>
        /// <param name="fileName"></param>
        private void outLib(string fileName)
        {
            this.gridList.EndEdit();
            string strWhere = "";   //获取选中的原油ID，用于在oilInfoB表中获取数据
            string strWhere2 = "";  //获取选中的原油ID,用于在oilDataB表中获取数据
            //string strWhereDataSearch = ""; //获取选中的原油ID,用于在oilDataSearch表中获取数据
            foreach (DataGridViewRow row in this.gridList.Rows)
            {
                if (bool.Parse(row.Cells["select"].Value.ToString()) == true)
                {
                    strWhere += " or ID=" + int.Parse(row.Cells["ID"].Value.ToString());   
                    strWhere2 += " or oilInfoID=" + int.Parse(row.Cells["ID"].Value.ToString());
                    //strWhereDataSearch += " or oilInfoID=" + int.Parse(row.Cells["ID"].Value.ToString());
                }
            }
            strWhere = strWhere.Trim().Substring(2);
            strWhere2 = strWhere2.Trim().Substring(2);
            // strWhereDataSearch = strWhere2.Trim().Substring(2);

            #region "要导入的oilInfoB数据"
            OilInfoOutAccess oilInfoOutAccess = new OilInfoOutAccess("OilInfoB");//导出数据连接
            List<OilInfoOut> oilInfoOuts = oilInfoOutAccess.Get(strWhere);  //oilInfoB表中获取数据
            #endregion 

            #region "B库未切割的数据"
            OilDataBAccess acess = new OilDataBAccess();
            List<OilDataBEntity> OilDataBEntityAlls = acess.Get(strWhere2);          //在OilDataB表中获取B库要的性质表，GC标准表数据
            List<OilDataBEntity> OilDataBEntityBs = OilDataBEntityAlls.Where(c => c.OilTableRow.oilTableTypeID == (int)EnumTableType.Whole || c.OilTableRow.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();


            List<OilDataOut> oilDataAlls = new List<OilDataOut>();
            foreach (OilDataBEntity OilDataBEntity in OilDataBEntityBs)  //把OilDataBEntity数据转为OilDataBOut数据
            {
                OilDataOut oilDataOut = new OilDataOut();
                oilDataOut.ID = OilDataBEntity.ID;
                oilDataOut.oilInfoID = OilDataBEntity.oilInfoID;
                oilDataOut.oilTableColID = OilDataBEntity.oilTableColID;
                oilDataOut.oilTableRowID = OilDataBEntity.oilTableRowID;
                oilDataOut.labData = "";
                oilDataOut.calData = OilDataBEntity.calData;
                oilDataAlls.Add(oilDataOut);
            }
            #endregion

            #region "B库切割后（查询库）的数据"
            OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
            List<OilDataSearchEntity> oilDataSearchEntityList = oilDataSearchAccess.Get(strWhere2);  //在oilDataSearch表中获取C库要数据
            List<OilDataSearchOut> oilDataSearchAlls = new List<OilDataSearchOut>();
            foreach (OilDataSearchEntity OilDataSearchEntity in oilDataSearchEntityList)  //把OilDataSearchEntity数据转为OilDataSearchOut数据
            {
                OilDataSearchOut oilDataSearchOut = new OilDataSearchOut();
                oilDataSearchOut.ID = OilDataSearchEntity.ID;
                oilDataSearchOut.oilInfoID = OilDataSearchEntity.oilInfoID;
                oilDataSearchOut.oilTableColID = OilDataSearchEntity.oilTableColID;
                oilDataSearchOut.oilTableRowID = OilDataSearchEntity.oilTableRowID;
                oilDataSearchOut.labData = "";
                oilDataSearchOut.calData = OilDataSearchEntity.calData;
                oilDataSearchAlls.Add(oilDataSearchOut);
            }                      
            #endregion

            #region "新建OutLib"
            OutLib outLib = new OutLib(); 
            OilTableRowOutAccess oilTableRowAccess = new OilTableRowOutAccess();
            OilTableColOutAccess oilTableColAccess = new OilTableColOutAccess();
            CurveTypeAccess curveTypeAccess = new CurveTypeAccess();          //添加曲线类别  
            outLib.oilTableRows = oilTableRowAccess.Get("1=1");               //获取oilTableRow表中所有行
            outLib.oilTableCols = oilTableColAccess.Get("1=1");               //获取oilTableCol表中所有列                             
            outLib.curveTypes = curveTypeAccess.Get("1=1");                   //添加曲线类别  
            #endregion 
        
            #region "根据选择原油编号来选择原油曲线类型"
            CurveAccess curveAccess = new CurveAccess();
            CurveDataAccess dataAccess = new CurveDataAccess();
            List<CurveEntity> curvesAll = new List<CurveEntity>();
            curvesAll = curveAccess.Get(strWhere2);            //获取选中原油的所有曲线
            #endregion

            #region "根据选择原油编号的选择原油曲线类型来选择曲线对应的数据"
            string strWherecurveID = "";//根据选中的原有的曲线ID来选择对应的曲线数据
            foreach (var curve in curvesAll)
            {
                strWherecurveID += " or curveID=" + curve.ID;
            }
            List<CurveDataEntity> curveDatasAll = new List<CurveDataEntity>();
            if (strWherecurveID.Length > 2)
            {
                strWherecurveID = strWherecurveID.Trim().Substring(2);
                curveDatasAll = dataAccess.Get(strWherecurveID); //获取选中原油的所有曲线的所有曲线数据
            }
            #endregion


            foreach (OilInfoOut oilInfoOut in oilInfoOuts) //循环查看导出了多少条数据
            {
                oilInfoOut.oilDatas = oilDataAlls.Where(c => c.oilInfoID == oilInfoOut.ID).ToList();   //添加oilDatas数据
                oilInfoOut.oilDataSearchOuts = oilDataSearchAlls.Where(c => c.oilInfoID == oilInfoOut.ID).ToList();   //添加oilDataSearchs数据
                oilInfoOut.curves = curvesAll.Where(c => c.oilInfoID == oilInfoOut.ID).ToList();   //添加一条原油数据的曲线
                
                foreach (CurveEntity curve in oilInfoOut.curves)      //添加一条原油数据的曲线的曲线点数据
                {
                    curve.curveDatas = new List<CurveDataEntity>();
                    curve.curveDatas = curveDatasAll.Where(c => c.curveID == curve.ID).ToList();
                }
                outLib.oilInfoOuts.Add(oilInfoOut);                 //构建一条一条的原油信息，形成原油列表
            } 
            Serialize.Write<OutLib>(outLib, fileName);
        }

        private void toolStripBtnSelect_Click(object sender, EventArgs e)
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
