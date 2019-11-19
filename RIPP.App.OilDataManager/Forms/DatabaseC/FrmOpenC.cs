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
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data.DataCheck;
namespace RIPP.App.OilDataManager.Forms.DatabaseC
{
    public partial class FrmOpenC : Form, IWaitingPanel
    {
        #region "私有变量"
        private WaitingPanel waitingPanel; //全局声明
        private OilInfoBEntity _oil = null;//当前原油
        private IList<OilInfoBEntity> openOilCollection = new List<OilInfoBEntity>();//存储上次查找到的原油
        private string _sqlWhere = "1=1";//查找语句
        /// <summary>
        /// 是否在繁忙状态
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return waitingPanel.IsBusy;
            }
            set
            {
                waitingPanel.IsBusy = value;
            }
        }
        private DgvHeader dgvHeader = new DgvHeader();
        private OilDataCheck oilDataCheck = new OilDataCheck();
        private const string dateFormat = "yyyy-MM-dd";
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        #endregion


        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmOpenC()
        {
            InitializeComponent();          
            this.Text = "打开查询库";
            this.Name = "frmOpenC";
            InitGridListBind();
            this.DoubleBuffered = true;
            waitingPanel = new WaitingPanel(this);
        }
        #endregion 

        #region"私有事件"
       
        /// <summary>
        /// 设置样式
        /// </summary>
        private void InitStyle()
        {
            this.gridList.AllowUserToAddRows = false;
            //this.gridList.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            //this.gridList.DefaultCellStyle = myStyle.dgdViewCellStyle2();

            this.gridList.BorderStyle = BorderStyle.None;
            this.gridList.RowHeadersWidth = 30;
            this.gridList.MultiSelect = false;
            this.gridList.ReadOnly = true;

            this.gridList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

        }
        /// <summary>
        /// 重新刷新
        /// </summary>
        public void refreshGridList()
        {
            InitGridListBind();
        }
        /// <summary>
        /// 表格控件绑定
        /// </summary>
        public  void InitGridListBind()
        {
            dgvHeader.SetAppDataBaseBColHeader(this.gridList);

            OilInfoBAccess acess = new OilInfoBAccess();
            IList<OilInfoBEntity> oilInfo = acess.Get(this._sqlWhere);
            
            OilDataSearchAccess dataSearchAccess = new OilDataSearchAccess();
            //绑定数据
            for (int i = 0; i < oilInfo.Count; i++)
            {
                List<OilDataSearchEntity> dataList = dataSearchAccess.Get("oilInfoID =" +  oilInfo[i].ID).ToList();
                if (dataList.Count > 0)
                {
                    List<OilDataSearchEntity> oilinfoDataList = dataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Info).ToList();
                    #region "日期处理"
                    string receiveDate = oilInfo[i].receiveDate == null ? string.Empty : oilInfo[i].receiveDate.Value.ToString(dateFormat);
                    string updataDate = string.Empty;
                    if (oilInfo[i].updataDate != string.Empty)
                    {
                        var updataDateTime = oilDataCheck.GetDate(oilInfo[i].updataDate);
                        updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                    }
                    #endregion
                    int rowIndex = this.gridList.Rows.Add();
                    this.gridList.Rows[rowIndex].Cells["ID"].Value = oilInfo[i].ID;
                    foreach (var data in oilinfoDataList)
                    {
                        switch (data.OilTableRow.itemCode)
                        {                            
                            case "CNA":
                                this.gridList.Rows[rowIndex].Cells["原油名称"].Value = data.calData;
                                break;
                            case "ENA":
                                this.gridList.Rows[rowIndex].Cells["英文名称"].Value = data.calData;
                                break;
                            case "IDC":
                                this.gridList.Rows[rowIndex].Cells["原油编号"].Value = data.calData;
                                break;
                            case "COU":
                                this.gridList.Rows[rowIndex].Cells["产地国家"].Value = data.calData;
                                break;
                            case "GRC":
                                this.gridList.Rows[rowIndex].Cells["地理区域"].Value = data.calData;
                                break;


                            case "ADA":
                                this.gridList.Rows[rowIndex].Cells["评价日期"].Value = data.calData;
                                break;
                            case "UDD":
                                this.gridList.Rows[rowIndex].Cells["入库日期"].Value = data.calData;
                                break;
                            case "SR":
                                this.gridList.Rows[rowIndex].Cells["数据来源"].Value = data.calData;
                                break;
                            case "CLA":
                                this.gridList.Rows[rowIndex].Cells["类别"].Value = data.calData;
                                break;


                            case "TYP":
                                this.gridList.Rows[rowIndex].Cells["基属"].Value = data.calData;
                                break;
                            case "SCL":
                                this.gridList.Rows[rowIndex].Cells["硫水平"].Value = data.calData;
                                break;
                            case "ACL":
                                this.gridList.Rows[rowIndex].Cells["酸水平"].Value = data.calData;
                                break;

                        }
                    
                    }
                    //this.gridList.Rows.Add(
                    //            oilInfo[i].ID,
                    //            oilInfo[i].crudeName,
                    //            oilInfo[i].englishName,
                    //            oilInfo[i].crudeIndex,
                    //            oilInfo[i].country,
                    //            oilInfo[i].region,
                    //            receiveDate,
                    //            updataDate,
                    //            oilInfo[i].sourceRef,
                    //            oilInfo[i].type,
                    //            oilInfo[i].classification,
                    //            oilInfo[i].sulfurLevel,
                    //            oilInfo[i].acidLevel);
                }
             }
            //lbResult.Text = "共有" + oilInfo.Count.ToString() + "条信息满足条件。";
        }

        /// <summary>
        /// 鼠标双击-打开一条原油
        /// </summary>     
        public  void openOil()
        {          
            try
            {
                //this.IsBusy = true;
                int oilInfoId = int.Parse(this.gridList.CurrentRow.Cells["ID"].Value.ToString());

                string crudeIndex = this.gridList.CurrentRow.Cells["原油编号"].Value.ToString();

                FrmMain frmMain = this.MdiParent as FrmMain;
                DatabaseC.FrmOilDataC child = (DatabaseC.FrmOilDataC)frmMain.GetChildFrm(crudeIndex + "C");
               
                if (child == null)
                {
                    DatabaseC.FrmOilDataC form = new DatabaseC.FrmOilDataC(oilInfoId);
                    form.MdiParent = frmMain;
                    form.Text = "查询库原油数据-" + crudeIndex;
                    form.Name = crudeIndex + "C";                                     
                    form.Show();
                    //form.BringToFront();
                    form.Activate();
                }
                else
                {
                    child.Activate();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(oilInfoId.ToString());
                Log.Error(ex.ToString());
            }
            finally
            {
                //this.IsBusy = false; 
            }
        }
     
        #endregion    

        #region "按钮事件"
        /// <summary>
        /// 打开C库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            openOil();
        }
        /// <summary>
        /// 生成C库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {          
            this.IsBusy = true;
            try
            {
                SaveC();
            }
            finally
            {
                this.IsBusy = false;
            }
            
        }
        /// <summary>
        /// 删除C库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            delete();
        }

       

        #endregion 
        /// <summary>
        /// 删除一条记录
        /// </summary>
        public void delete()
        {
            if (this.gridList.CurrentRow != null)
            {
                if (MessageBox.Show("是否要删除!", "信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    try
                    {
                         string strID = this.gridList.CurrentRow.Cells["ID"].Value.ToString();
                         int ID = 0;
                         if (int.TryParse(strID, out ID))
                         {
                             this.IsBusy = true;
                             OilDataSearchAccess dataSearchAccess = new OilDataSearchAccess();
                             dataSearchAccess.deleteData("Delete from OilDataSearch where oilInfoID =" + ID);
                             this.IsBusy = false;
                         }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("数据管理" + ex);
                    }
                }
            }
        }

       /// <summary>
       /// 保存到Ｃ库
       /// </summary>
        private void SaveC()
        {
            string strID = this.gridList.CurrentRow.Cells["ID"].Value.ToString();
            string crudeIndex = this.gridList.CurrentRow.Cells["原油编号"].Value.ToString();                    
            int ID = 0;
            if (int.TryParse(strID, out ID))
            {
                OilDataSearchAccess dataSearchAccess = new OilDataSearchAccess();
                List<OilDataSearchEntity> dataList = dataSearchAccess.Get("oilInfoID =" + ID).ToList();
                if (dataList.Count > 0)
                {
                    DialogResult r = MessageBox.Show("原油" + crudeIndex + "的查询库数据已经存在是否替换？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (r == DialogResult.Yes)
                    {                       
                        dataSearchAccess.deleteData("Delete from OilDataSearch where oilInfoID =" + ID);
                         
                        OilInfoBEntity oilB = OilBll.GetOilByCrudeIndex(crudeIndex);
                        OilInfoEntity oilA = OilBll.GetOilById(crudeIndex);
                        if (oilA == null)
                            OilBll.SaveC(oilB);
                        else
                            OilBll.SaveC(oilA, oilB);

                        MessageBox.Show("原油" + crudeIndex + "生成查询库成功！", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        FrmMain frmMain = this.MdiParent as FrmMain;
                        DatabaseC.FrmOilDataC child = (DatabaseC.FrmOilDataC)frmMain.GetChildFrm(crudeIndex + "C");

                        if (child != null)
                        {
                            MessageBox.Show("原油" + crudeIndex + "的数据窗体需关闭重新打开才有效！", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    DialogResult r = MessageBox.Show("是否保存数据到快速查询库！", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (r == DialogResult.Yes)
                    {
                        dataSearchAccess.deleteData("Delete from OilDataSearch where oilInfoID =" + ID);

                        OilInfoBEntity oilB = OilBll.GetOilByCrudeIndex(crudeIndex);
                        OilInfoEntity oilA = OilBll.GetOilById(crudeIndex);
                        if (oilA == null)
                            OilBll.SaveC(oilB);
                        else
                            OilBll.SaveC(oilA, oilB);

                        MessageBox.Show("原油" + crudeIndex + "生成查询库成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        FrmMain frmMain = this.MdiParent as FrmMain;
                        DatabaseC.FrmOilDataC child = (DatabaseC.FrmOilDataC)frmMain.GetChildFrm(crudeIndex + "C");

                        if (child != null)
                        {
                            MessageBox.Show("原油" + crudeIndex + "的数据窗体需关闭重新打开才有效！", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        }
                    }
                }

                
            }
            else
                MessageBox.Show("应用库无此条原油！", "警告信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            openOil();
        }
        /// <summary>
        /// 添加行标
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
