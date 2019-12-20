using RIPP.App.OilDataApp.Outputs.RefineryAssays;
using RIPP.Lib;
using RIPP.OilDB.BLL;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.DataCheck;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace RIPP.App.OilDataApp.Forms
{
    public partial class FrmMain : Form
    {
        #region 私有变量
        private OutLib _outLib = null;//导入的B库文件

        /// <summary>
        /// 原油ID和混合比例
        /// </summary>
        private List<CutOilRateEntity> _cutOilRates = new List<CutOilRateEntity>();

        /// <summary>
        /// 切割方案
        /// </summary>
        private List<CutMothedEntity> _cutMotheds = new List<CutMothedEntity>();

        /// <summary>
        /// 切割计算结果
        /// </summary>
        private OilInfoBEntity _oilB = new OilInfoBEntity();     //切割计算结果

        /// <summary>
        /// 切割算法
        /// </summary>
        private OilApplyBll oilApplyBll = new OilApplyBll();

        /// <summary>
        /// 镇海演示模块功能
        /// </summary>
        private bool bZhenHai = false;

        /// <summary>
        /// 镇海演示模块功能
        /// </summary>
        public bool BZH
        {
            get { return this.bZhenHai; }
            set { this.bZhenHai = value; }
        }

        public DataGridView GridListAdd { get; private set; }

        #region"step1"
        private string _sqlWhere = "1=1";

        /// <summary>
        /// 用于防止打开多个窗体
        /// </summary>
        private bool _isOilOpening = false;

        /// <summary>
        /// 存储上次查找到的原油
        /// </summary>
        private IList<CrudeIndexIDBEntity> _openOilCollection = new List<CrudeIndexIDBEntity>();//存储上次查找到的原油

        private int tabControlIndex = 0;//判断是范围查找还是相似查找
        private ListView _tempShowViewList = null;

        /// <summary>
        /// 范围查找条件集合
        /// </summary>
        private IList<OilRangeSearchEntity> _rangeSearchList = new List<OilRangeSearchEntity>();

        /// <summary>
        ///  相似查找条件集合
        /// </summary>
        private IList<OilSimilarSearchEntity> _similarSearchList = new List<OilSimilarSearchEntity>();

        /// <summary>
        /// 从C库获取满足条件的原油编号,存放查找原油的相似度（范围查找相似度为0）
        /// </summary>
        private IDictionary<string, double> tempRanSumDic = new Dictionary<string, double>();//从C库获取满足条件的原油编号,存放查找原油的相似度（范围查找相似度为0）

        /// <summary>
        /// 用来设置表头
        /// </summary>
        private DgvHeader dgvHeader = new DgvHeader();

        /// <summary>
        /// 数据处理
        /// </summary>
        private OilDataCheck oilDataCheck = new OilDataCheck();

        #endregion 私有变量

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

        #region 构造函数

        /// <summary>
        ///
        /// </summary>
        public FrmMain(bool zhenHai = false)
        {
            InitializeComponent();
            this.bZhenHai = zhenHai;
            this.WindowState = FormWindowState.Maximized; //窗口最大化
            this.butStep2.Enabled = false;
            this.butStep3.Enabled = false;
            this.butStep4.Enabled = false;
            this.btnStep5.Enabled = false;
            this.butStep6.Enabled = false;

            InitStep1();
            if (BZH)
            {
                this.tableLayoutPanelStep1Main.RowStyles[0].Height = 0;

                demo();
            }
        }

        /// <summary>
        /// 演示模版（隐藏第二步）
        /// </summary>
        private void demo()
        {
            butStep2.Visible = false;
            butShowAll.Visible = false;
            btnStep5.Location = butStep4.Location;
            butStep4.Location = butStep3.Location;
            butStep3.Location = butStep2.Location;

            butStep3.Text = "第2步 切割方案";
            butStep4.Text = "第3步 原油切割";
            btnStep5.Text = "第4步 导出Excel";
        }

        #endregion

        #region "公有函数"

        /// <summary>
        /// step1初始化
        /// </summary>
        public void InitStep1()
        {
            this.panelStep1.Visible = false;
            this.panelStep2.Visible = false;
            this.panelStep3.Visible = false;
            this.panelStep4.Visible = false;
            this.panelStep6.Visible = false;
            this.panelStep1.Dock = DockStyle.Fill;
            #region "step1"
            cmbFractionBind();
            GridListSourceBind();
            GridListSelectBind();
            GridListAddBind();
            GridListGroupBind();
            #endregion
        }

        #endregion

        #region 主窗体控件事件

        /// <summary>
        /// 取消子窗口左上角的图标
        /// </summary>
        private void menuStrip_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (e.Item.ToString() == "System.Windows.Forms.MdiControlStrip+SystemMenuItem")
            {
                e.Item.Visible = false;
            }
        }

        #endregion

        #region 菜单

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        #endregion

        #region

        /// <summary>
        /// 选择原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //this._cutMotheds.Clear();
            //this._cutOilRates.Clear();
            //this._oil.OilDatas.Clear();
            //this.button3.Enabled = false;
            //this.button4.Enabled = false;
            //this.button5.Enabled = false;
            this.panelStep1.Dock = DockStyle.Fill;  //将STEP1所用的界面铺展右侧空间
            this.panelStep1.Visible = true;          //将STEP1所用的界面可视
            this.panelStep1.BringToFront();//将STEP1所用的界面放到最上层
            this.toolStripStatusLabel.Text = "选择原油";
        }

        /// <summary>
        /// 混合方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.panelStep2.Dock = DockStyle.Fill;
            this.panelStep2.Visible = true;
            this.panelStep2.BringToFront();

            this.toolStripStatusLabel.Text = "混合方案";
        }

        /// <summary>
        /// 设置切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.panelStep3.Dock = DockStyle.Fill;
            this.panelStep3.Visible = true;
            this.panelStep3.BringToFront();

            this.toolStripStatusLabel.Text = "切割方案";
            ConfigBll config = new ConfigBll();
            string strDir = config.getDir(enumModel.AppCut);
            if (!File.Exists(strDir))
            {
                MessageBox.Show("找不到默认切割方案，请重新设置!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            else
            {
                initAllCutMothed(strDir);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.panelStep6.Dock = DockStyle.Fill;
            this.panelStep6.Visible = true;
            this.panelStep6.BringToFront();

            this.toolStripStatusLabel.Text = "物性定制";
        }

        /// <summary>
        /// 计算并输出计算结果到界面表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (this._cutMotheds.Count <= 0 || this._cutOilRates.Count <= 0)
                return;
            try
            {
                this.StartWaiting();
                this._oilB = oilApplyBll.GetCutResult(this._cutOilRates, this._cutMotheds);
            }
            catch (Exception ex)
            {
                Log.Error("原油切割错误：" + ex.ToString());
                return;
            }
            finally
            {
                this.StopWaiting();
                InitStep4();
                this.panelStep4.Dock = DockStyle.Fill;
                this.panelStep4.Visible = true;
                this.toolStripStatusLabel.Text = "切割计算";
                this.btnStep5.Enabled = true;
            }

            #region

            //ShowCutData newForm = new ShowCutData(this._oilB, this._cutMotheds);
            //newForm.Show();
            //OilPropertyAPIEntity temp = new OilPropertyAPIEntity()
            //{
            //    D20 = 0.8612f,
            //    CCR = 5.52f,
            //    N2 = 1300f,
            //    WAX = 3f,
            //    SUL = 1.8f,
            //    TWY140 = 15.44981f,
            //    TWY180 = 22.57231f,
            //    TWY240 = 32.58333f,
            //    TWY350 = 51.37775f
            //};
            //OilApplyAPIBll oilApplyAPIBll = new OilApplyAPIBll();

            //CutMothedAPIEntity a = new CutMothedAPIEntity()
            //{
            //    ICP = 15,
            //    ECP = 180,
            //    Name = CutTableName.ShiNaoYou
            //};
            //CutMothedAPIEntity b = new CutMothedAPIEntity()
            //{
            //    ICP = 140,
            //    ECP = 240,
            //    Name = CutTableName.MeiYou
            //};
            //CutMothedAPIEntity c = new CutMothedAPIEntity()
            //{
            //    ICP = 180,
            //    ECP = 350,
            //    Name = CutTableName.ChaiYou
            //};
            //CutMothedAPIEntity d = new CutMothedAPIEntity()
            //{
            //    ICP = 350,
            //    ECP = 540,
            //    Name = CutTableName.LaYou
            //};
            ////CutMothedAPIEntity f = new CutMothedAPIEntity()
            ////{
            ////    ICP = 350,
            ////    ECP = 1600,
            ////    Name = CutTableName.YuanYouXingZhi
            ////};
            //CutMothedAPIEntity g = new CutMothedAPIEntity()
            //{
            //    ICP = 540,
            //    ECP = 1600,
            //    Name = CutTableName.ZhaYou
            //};
            //List<CutMothedAPIEntity> l = new List<CutMothedAPIEntity>();
            //l.Add(a);
            //l.Add(b);
            //l.Add(c);
            //l.Add(d);
            ////l.Add(f);
            //l.Add(g);

            //List<CutMothedEntity> cutMothedList = new List<CutMothedEntity>();
            //for (int index = 0; index < l.Count; index++)
            //{
            //    CutMothedEntity cutMothed = new CutMothedEntity();
            //    cutMothed.ICP = l[index].ICP;
            //    cutMothed.ECP = l[index].ECP;
            //    cutMothed.Name = l[index].Name.GetDescription();
            //    cutMothedList.Add(cutMothed);
            //}
            //List<string> crudeIndexList = new List<string>() { "RIPP0337", "RIPP0044", "RIPP0277", "RIPP0113", "RIPP0206", "RIPP0405" };
            //List<float> cutRateList = new List<float>() { 81.56557f, 11.75237f, 2.040833f, 2.034374f, 1.848026f, 0.7588176f };
            //List<CutOilRateEntity> rateList = new List<CutOilRateEntity>();
            //for (int i = 0; i < crudeIndexList.Count; i++)
            //{
            //    CutOilRateEntity cutRateEntity = new CutOilRateEntity();
            //    cutRateEntity.crudeIndex = crudeIndexList[i];
            //    cutRateEntity.rate = cutRateList[i];
            //    rateList.Add(cutRateEntity);
            //}

            //this._oil = oilApplyAPIBll.GetCutResultAPI("RIPP0305 ", l);
            ////this._oil = oilApplyAPIBll.GetCutResultAPI(rateList, l);
            ////this._oil = oilApplyAPIBll.GetCutResultAPI(temp, l);
            ////ShowCutData newForm = new ShowCutData(this._oil, this._cutMotheds);
            //ShowCutData newForm = new ShowCutData(this._oil, cutMothedList);
            //newForm.Show();
            #endregion
        }

        /// <summary>
        /// 计算结果输出为Excel
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel.Text = "导出Excel";
            string strFilePath = string.Empty;
            try
            {
                ConfigBll config = new ConfigBll();

                string strDir = config.getDir(enumModel.AppXls);
                if (File.Exists(strDir))
                {
                    strFilePath = strDir;
                }
                else
                {
                    if (this.BZH)
                    {
                        MessageBox.Show("找不到默认输出模板，请重新设置!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }
                    else
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Title = "选择输出模版";
                        openFileDialog.Filter = "原油数据模版文件 (*.xls)|*.xls";//Excel2010与2003不兼容
                        openFileDialog.RestoreDirectory = true;
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            strFilePath = openFileDialog.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                int outResult = DataToExcelBll.DataToExcel(null, this._oilB, strFilePath, _cutMotheds, "B");
                if (outResult == 1)
                {
                    MessageBox.Show("数据导出成功!", "提示");
                }
                else if (outResult == -1)
                {
                    MessageBox.Show("当前系统尚未安装EXCEL软件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -2)
                {
                    MessageBox.Show("不能打开Excel进程,请关闭Excel后重试!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -3)
                {
                    MessageBox.Show("数据导出失败，请检查模版是否正确或者关闭正在运行的Excel重试!", "提示");
                }
                else if (outResult == -11)
                {
                    MessageBox.Show("切割数据不存在!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -99)
                {
                    MessageBox.Show("数据导出失败，找不到正确模板!", "提示");
                }
                else if (outResult == 0)//取消保存
                {
                    //MessageBox.Show("数据导出失败,错误信息：\r\n" + outResult, "提示");
                }
            }
            catch (Exception ex)
            {
                Log.Error("导出原油数据出错：" + ex.ToString());
            }
        }

        #endregion

        #region "文件"

        /// <summary>
        /// 重新开始
        /// </summary>
        private void menuItemStart_Click(object sender, EventArgs e)
        {
            GridListSourceBind();
            this._cutMotheds.Clear();
            this._cutOilRates.Clear();
            this._oilB = new OilInfoBEntity();//输出原油
            this.gridListSelect.Rows.Clear();
            this.gridListRate.Rows.Clear();
            this.gridListCut.Rows.Clear();

            this.panelStep1.Visible = false;
            this.panelStep2.Visible = false;
            this.panelStep3.Visible = false;
            this.panelStep4.Visible = false;
            this.panelStep6.Visible = false;
            this.butStep2.Enabled = false;
            this.butStep3.Enabled = false;
            this.butStep4.Enabled = false;
            this.btnStep5.Enabled = false;
            this.butStep6.Enabled = false;
            this.toolStripStatusLabel.Text = "再次切割";
        }

        #endregion

        /// <summary>
        /// 多B库合并
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuLibBMerge_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "原油数据文件 (*.libB)|*.libB";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._outLib = Serialize.Read<OutLib>(saveFileDialog.FileName);
                this.Visible = true;

                FrmLibBIn tempFrmLibBIn = (FrmLibBIn)this.GetChildFrm("FrmLibBIn");
                if (tempFrmLibBIn == null)   //导入B库，只能存在一个窗口
                {
                    FrmLibBIn frmLibBIn = new FrmLibBIn();
                    frmLibBIn.Init(this._outLib);
                    frmLibBIn.Name = "FrmLibBIn";
                    frmLibBIn.Show();
                }
                else
                    tempFrmLibBIn.Init(this._outLib);
            }
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //打开子窗口
            if (!this.IsExistChildFrm("FrmSetting"))
            {
                FrmSetting frmSetting = new FrmSetting();
                frmSetting.Show();
            }
        }

        /// <summary>
        /// 是否存在该类型的子窗体
        /// </summary>
        /// <param name="childFrmType">窗体类型</param>
        /// <returns>存在返回1并激活此窗口，不存在返回0</returns>
        public bool IsExistChildFrm(string childFrmType)
        {
            bool flag = false;
            foreach (Form frm in this.MdiChildren)
            {
                if (frm.GetType().Name == childFrmType)
                {
                    frm.Activate();
                    flag = true;
                    break;
                }
            }
            return flag;
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
        /// enter结束编辑
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                this.gridListRate.EndEdit();
                this.gridListCut.EndEdit();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void gridListRate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    this.gridListRate.EndEdit();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            gridList.CellContentClick += GridList_CellContentClick;
            gridList.CellMouseDown += GridList_CellMouseDown;
            gridListSelect.CellContentClick += GridList_CellContentClick;
            gridListSelect.CellMouseDown += GridList_CellMouseDown;
            gridListAdd.CellContentClick += GridList_CellContentClick;

            toolStripMenuItemSelectAll.Click += ToolStripMenuItemSelectAll_Click;
            toolStripMenuItemReverseSelect.Click += ToolStripMenuItemSelectAll_Click;
            toolStripMenuItemNonSelect.Click += ToolStripMenuItemSelectAll_Click;
            toolStripMenuItemAdd.Click += btnSelect_Click;
            toolStripMenuItemRemove.Click += btnDel_Click;
            toolStripMenuItemExportXml.Click += ToolStripMenuItemExportXml_Click;
        }

        SaveFileDialog saveXmlFileDialog1 = new SaveFileDialog()
        {
            Filter = "XML 文件 (*.xml)|*.xml",
            DefaultExt = "xml",
            Title = "XML 文件",

        };

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemExportXml_Click(object sender, EventArgs e)
        {
            var gv = contextMenuStrip1.Tag as DataGridView;
            if (gv == null)
                return;
            var row = gv.CurrentRow;// gv.SelectedRows.Count > 0 ? gv.SelectedRows[0] : null;
            if (row == null)
                return;

            var id = (int?)row.Cells["ID"].Value;

            if (id == null)
                return;

            OilInfoBAccess access = new OilInfoBAccess();
            var oil = access.Get(id.Value);

            saveXmlFileDialog1.FileName = $"{oil.crudeIndex} - {oil.englishName ?? oil.crudeName}";
            if (saveXmlFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var r = RefineryAssays.ConvertFrom(oil);
            var xml = r.ToXml();

            File.WriteAllText(saveXmlFileDialog1.FileName, xml);

        }

        /// <summary>
        /// 原油选择右键菜单：全选、不选、反选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemSelectAll_Click(object sender, EventArgs e)
        {
            Func<bool, bool> fun = (b) => false;
            if (sender == toolStripMenuItemSelectAll)
                fun = (b) => true;
            else if (sender == toolStripMenuItemReverseSelect)
                fun = (b) => !b;

            var gv = contextMenuStrip1.Tag as DataGridView;
            if (gv == null)
                return;

            foreach (DataGridViewRow row in gv.Rows)
            {
                var cell = row.Cells["Check"];
                var b = (bool?)cell.Value == true;
                b = fun(b);
                cell.Value = b;
            }
        }

        /// <summary>
        /// 原油选择单元格右键点击菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            var gv = sender as DataGridView;
            if (gv == null)
                return;

            contextMenuStrip1.Tag = sender;
            if (sender == gridList)
            {
                toolStripMenuItemAdd.Visible = true;
                toolStripMenuItemRemove.Visible = false;
            }
            else
            {
                toolStripMenuItemAdd.Visible = false;
                toolStripMenuItemRemove.Visible = true;
            }

            if (e.RowIndex >= 0)
            {
                gv.CurrentCell = gv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (!gv.Rows[e.RowIndex].Selected)
                {
                    gv.ClearSelection();
                    gv.Rows[e.RowIndex].Selected = true;
                }
                toolStripMenuItemExportXml.Visible = true;
            }
            else
                toolStripMenuItemExportXml.Visible = false;

            contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
        }

        /// <summary>
        /// 原油选择的复选框点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            var gv = sender as DataGridView;
            if (gv == null)
                return;

            if (gv.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                var t = gv.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;
                if ((bool?)t.Value != true)
                    t.Value = true;
                else
                    t.Value = false;
            }
        }

        /// <summary>
        /// 高级查询功能隐藏与显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectHide_Click(object sender, EventArgs e)
        {
            if (btnSelectHide.Text == "高级查询")
            {
                btnSelectHide.Text = "隐藏高级查询";
                tabControl1.Visible = true;
            }
            else {
                btnSelectHide.Text = "高级查询";
                tabControl1.Visible = false;
            }
        }


        private void gridListAdd_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y,
                this.gridListAdd.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                this.gridListAdd.RowHeadersDefaultCellStyle.Font, rectangle,
                this.gridListAdd.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        private void GridListAddBind()
        {
            dgvHeader.SetAppDataBaseBColHeader(this.gridListAdd);
            gridListAdd.Columns.Insert(0, new DataGridViewCheckBoxColumn() { Name = "Check", HeaderText = "选择", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            gridListAdd.Columns[1].ReadOnly = true;
            gridListAdd.Columns[2].ReadOnly = true;
            gridListAdd.Columns[3].ReadOnly = true;
            gridListAdd.Columns[5].Visible = false;
            gridListAdd.Columns[6].Visible = false;
            gridListAdd.Columns[7].Visible = false;
            gridListAdd.Columns[8].Visible = false;
            gridListAdd.Columns[9].Visible = false;
            gridListAdd.Columns[10].Visible = false;
            gridListAdd.Columns[11].Visible = false;
            gridListAdd.Columns[12].Visible = false;
            gridListAdd.Columns[13].Visible = false;
            gridListAdd.Columns.Add(new DataGridViewTextBoxColumn() { Name = "混炼加工量", HeaderText = "混炼加工量", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            gridListAdd.Columns.Add(new DataGridViewTextBoxColumn() { Name = "混兑比例", HeaderText = "混兑比例%",  AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.gridListAdd.Rows.Clear();
        }

        /// <summary>
        /// 加入油种信息列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in this.gridListSelect.Rows)
            {
                if ((bool?)row.Cells["Check"].Value == true)
                {
                    rows.Add(row);
                }
            }
            if (rows.Any() != true)
            {
                MessageBox.Show("请先勾选原油！");
                return;
            }
            //已经存在的原油
            var exists = new List<string>();
            foreach (DataGridViewRow rowSlect in gridListAdd.Rows)
            {
                exists.Add(rowSlect.Cells["ID"].Value.ToString());
            }
            foreach (var r in rows)
            {
                var id = r.Cells["ID"].Value?.ToString();
                if (exists.Contains(id))
                    continue;

                DataGridViewRow row = new DataGridViewRow();
                foreach (DataGridViewCell c in r.Cells)
                {
                    var c2 = c.Clone() as DataGridViewCell;
                    c2.Value = c.Value;
                    if (c.OwningColumn.Name == "Check")
                    {
                        var t = c2 as DataGridViewCheckBoxCell;
                        t.Value = false;
                    }
                    row.Cells.Add(c2);
                }
                this.gridListAdd.Rows.Add(row);
            }
        }

        /// <summary>
        /// 从油种信息列表移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            for (int i = this.gridListAdd.Rows.Count - 1; i >= 0; i--)
            {
                if ((bool?)gridListAdd.Rows[i].Cells["Check"].Value == true)
                    this.gridListAdd.Rows.RemoveAt(i);
            }
            this.gridListAdd.Refresh();
        }

        /// <summary>
        /// 油组列表表头设置
        /// </summary>
        /// <param name="dgv"></param>
        public void SetGroupColHeader(DataGridView dgv)
        {
            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "油组名称", HeaderText = "油组名称", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "油种数量", HeaderText = "油种数量", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "创建日期", HeaderText = "创建日期", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "备注", HeaderText = "备注", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
        }
        /// <summary>
        /// 油组列表行号设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridListGroup_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y,
                this.gridListAdd.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                this.gridListAdd.RowHeadersDefaultCellStyle.Font, rectangle,
                this.gridListAdd.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void GridListGroupBind()
        {
            SetGroupColHeader(this.gridListGroup);
        }

        /// <summary>
        /// 新建油组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewGroup_Click(object sender, EventArgs e)
        {
            FrmNewGroup frmNewGroup = new FrmNewGroup();
            frmNewGroup.StartPosition = FormStartPosition.Manual;
            frmNewGroup.Location = new Point(700, 400);
            frmNewGroup.TransfEvent += frmNewGroup_TransfEvent;
            frmNewGroup.Show();
        }
        void frmNewGroup_TransfEvent(string groupName, String groupRemark)
        {
            gridListGroup.Rows.Add(groupName, gridListSelect.RowCount, DateTime.Now.ToString(), groupRemark);
        }
    }
}