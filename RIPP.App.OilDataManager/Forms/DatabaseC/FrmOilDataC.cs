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
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.OilApply;
using ZedGraph;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Interpolation.Algorithms;
using MathNet.Numerics.Interpolation;
using RIPP.Lib;

namespace RIPP.App.OilDataManager.Forms.DatabaseC
{
    public partial class FrmOilDataC : Form, IWaitingPanel
    {
        #region "私有变量"
        /// <summary>
        /// 要打开的C库的原油ID
        /// </summary>
        private  int _oilInfoID = 0;
        private  Dictionary<string, Dictionary<int, OilDataSearchRowEntity>> DIC = new Dictionary<string, Dictionary<int, OilDataSearchRowEntity>>();
        /// <summary>
        /// 所选中的表格
        /// </summary>
        private  DataGridView _selectGridView = null;
        private GridOilDataEdit oilEdit = null;//用于编辑数据
        private WaitingPanel waitingPanel; //全局声明
        private bool _isValueChange = false;
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
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
        /// <summary>
        /// 原油信息表是否需要保存
        /// </summary>
        private bool _dgvInfoNeedSave = false;
        /// <summary>
        /// 原油性质表是否需要保存
        /// </summary>
        private bool _dgvWholeNeedSave = false;//原油性质表是否需要保存
        /// <summary>
        /// 石脑油表是否需要保存
        /// </summary>
        private bool _dgvNaphthaNeedSave = false;//石脑油表是否需要保存
        /// <summary>
        /// 煤油表是否需要保存
        /// </summary>
        private bool _dgvAviationKeroseneNeedSave = false;//煤油表是否需要保存
        /// <summary>
        /// 柴油表是否需要保存
        /// </summary>
        private bool _dgvDieselNeedSave = false;//柴油表是否需要保存
        /// <summary>
        /// 蜡油表是否需要保存
        /// </summary>
        private bool _dgvVGONeedSave = false;
        /// <summary>
        /// 渣油表是否需要保存
        /// </summary>
        private bool _dgvResidualNeedSave = false;//渣油表是否需要保存
        /// <summary>
        /// 判断表格数据是否需要保存
        /// </summary>
        private bool _isChange = false;
        #endregion ""

        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public FrmOilDataC()
        {
            InitializeComponent();
            waitingPanel = new WaitingPanel(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oilInfoBEntity"></param>
        public FrmOilDataC(int OilInfoID)
        {
            InitializeComponent();

            this._oilInfoID = OilInfoID;
            this.tabControl1.SelectedTab = this.tabPage1;
            this._selectGridView = this.dataGridView1;
            oilEdit = new GridOilDataEdit(this._oilInfoID);
            initDataGridView();
            waitingPanel = new WaitingPanel(this);
        }
        #endregion 

        #region "表格初始化"
        /// <summary>
        /// 表格样式
        /// </summary>
        private void InitStyle(DataGridView dataGridView)
        {
            dataGridView.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            dataGridView.DefaultCellStyle = myStyle.dgdViewCellStyle2();
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.MultiSelect = true;
        }
        /// <summary>
        /// 重新刷新数据
        /// </summary>
        public void refreshData()
        { 
        
        
        
        }
        /// <summary>
        /// 初始化表格格式
        /// </summary>
        private void initDataGridView()
        {
            OilDataSearchColAccess oilDataColAccess = new OilDataSearchColAccess();
            List<OilDataSearchColEntity> OilDataCols = oilDataColAccess.Get("1=1");

            OilDataSearchRowAccess oilDataRowAccess = new OilDataSearchRowAccess();
            List<OilDataSearchRowEntity> OilDataRows = oilDataRowAccess.Get("1=1");

            OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
            List<OilDataSearchEntity> allDatas = oilDataSearchAccess.Get("oilInfoID = " + this._oilInfoID).ToList();
                    
            #region "原油信息"
            //List<OilDataSearchColEntity> oilInfoCols = OilDataCols.Where(o =>o.OilTableName == "原油信息").ToList();
            //List<OilDataSearchRowEntity> oilInfoRows = OilDataRows.Where(o => o.OilDataColID == oilInfoCols[0].ID).ToList();

            //List<OilTableRowEntity> oilInfoTableRows = new List<OilTableRowEntity>();
            //for (int i = 0; i < oilInfoRows.Count; i++)
            //{
            //    OilTableRowEntity oilTableRow = OilTableRowBll._OilTableRow.Where(o => o.ID == oilInfoRows[i].OilTableRowID).FirstOrDefault();
            //    oilInfoTableRows.Add(oilTableRow);
            //}

            //#region "做列字典和行字典"
            ////Dictionary<int, OilDataSearchColEntity> colDic = new Dictionary<int, OilDataSearchColEntity>();//判断有多少列
            ////foreach (var temp in oilInfoCols)
            ////{
            ////    if (!colDic.Keys.Contains(temp.OilTableColID))
            ////    {
            ////        colDic.Add(temp.OilTableColID, temp);
            ////    }
            ////}

            //Dictionary<string, int> rowIDDic = new Dictionary<string, int>();//表的行和oiltableRowID对应    
            //int rowID = 0;//设置行ID=0
            //foreach (var temp in oilInfoTableRows)
            //{
            //    if (!rowIDDic.Keys.Contains(temp.itemCode))
            //    {
            //        rowIDDic.Add(temp.itemCode, rowID);
            //        rowID++;
            //    }
            //}
            //#endregion

            //#region "初列：序号，项目，代码"
            //this.dataGridView1.Columns.Clear();
            
            //DataGridViewTextBoxColumn colID = new DataGridViewTextBoxColumn()
            //{
            //    Name = "序号",
            //    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            //    ReadOnly = true
            //};
            //DataGridViewTextBoxColumn Code = new DataGridViewTextBoxColumn()
            //{
            //    Name = "itemCode",
            //    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            //    ReadOnly = true
            //};
            //DataGridViewTextBoxColumn Name = new DataGridViewTextBoxColumn()
            //{
            //    Name = "itemName",
            //    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            //    ReadOnly = true
            //};

            //this.dataGridView1.Columns.Add(colID);
            //this.dataGridView1.Columns.Add(Code);
            //this.dataGridView1.Columns.Add(Name);

            //foreach (var temp in oilInfoCols)
            //{
            //    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            //    {
            //        HeaderText = temp.OilTableName,
            //        Name = temp.OilTableName,
            //        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            //        Width = 200,
            //        Tag = temp.OilTableColID
            //    };
            //    this.dataGridView1.Columns.Add(column);
            //}
            //#endregion

            //#region "初始化所有行"
            //this.dataGridView1.Rows.Clear();
            //int col = 0;
            //foreach (OilTableRowEntity temp in oilInfoTableRows)
            //{
            //    int index = this.dataGridView1.Rows.Add();
            //    this.dataGridView1.Rows[index].Cells["序号"].Value = col;
            //    this.dataGridView1.Rows[index].Cells["itemCode"].Value = temp.itemCode;
            //    this.dataGridView1.Rows[index].Cells["itemName"].Value = temp.itemName;
            //    this.dataGridView1.Rows[index].Tag = temp.ID;
            //    col++;
            //}
            //#endregion

            //this.dataGridView1.Rows[rowIDDic["CNA"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.crudeName;
            //this.dataGridView1.Rows[rowIDDic["ENA"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.englishName;
            //this.dataGridView1.Rows[rowIDDic["IDC"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.crudeIndex;
            //this.dataGridView1.Rows[rowIDDic["COU"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.country;
            //this.dataGridView1.Rows[rowIDDic["GRC"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.region;
            //this.dataGridView1.Rows[rowIDDic["ADA"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.receiveDate;
            //this.dataGridView1.Rows[rowIDDic["ALA"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.assayLab;

            //this.dataGridView1.Rows[rowIDDic["AER"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.assayer;
            //this.dataGridView1.Rows[rowIDDic["SR"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.sourceRef;
            //this.dataGridView1.Rows[rowIDDic["ASC"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.assayCustomer;
            //this.dataGridView1.Rows[rowIDDic["RIN"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.reportIndex;
            //this.dataGridView1.Rows[rowIDDic["CLA"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.type;
            //this.dataGridView1.Rows[rowIDDic["TYP"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.classification;
            //this.dataGridView1.Rows[rowIDDic["SCL"]].Cells[oilInfoCols[0].OilTableName].Value = oilInfoB.sulfurLevel;
            //InitStyle(this.dataGridView1);
            #endregion 

            InitDataGridView("原油信息", this.dataGridView1, OilDataCols, OilDataRows, allDatas);
            InitDataGridView("原油性质", this.dataGridView2, OilDataCols, OilDataRows, allDatas);
            InitDataGridView("石脑油", this.dataGridView3, OilDataCols, OilDataRows, allDatas);
            InitDataGridView("航煤", this.dataGridView4, OilDataCols, OilDataRows, allDatas);
            InitDataGridView("柴油", this.dataGridView5, OilDataCols, OilDataRows, allDatas);
            InitDataGridView("VGO", this.dataGridView6, OilDataCols, OilDataRows, allDatas);
            InitDataGridView("渣", this.dataGridView7, OilDataCols, OilDataRows, allDatas);
          
        }
        /// <summary>
        /// 初始化表格数据
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="Datas"></param>
        private void initData(DataGridView dataGridView, List<OilDataSearchColEntity> cols, List<OilTableRowEntity> rows, List<OilDataSearchEntity> Datas)
        {
            if (cols == null || rows == null)
                return;
            InitStyle(dataGridView);
            #region "   "
            Dictionary<int, OilDataSearchColEntity> colDic = new Dictionary<int, OilDataSearchColEntity>();//判断有多少列
            foreach (var temp in cols)
            {
                if (!colDic.Keys.Contains(temp.OilTableColID))
                {
                    colDic.Add(temp.OilTableColID ,temp);
                }
            }

            Dictionary<int, int> rowIDDic = new Dictionary<int, int>();//表的行和oiltableRowID对应    
            int rowID = 0;//设置行ID=0
            foreach (var temp in rows)
            {
                if (!rowIDDic.Keys.Contains(temp.ID))
                {
                    rowIDDic.Add(temp.ID, rowID);
                    rowID++;
                }
            }
            #endregion 

            #region "初列：序号，项目，代码"
            dataGridView.Columns.Clear();
            DataGridViewTextBoxColumn colID = new DataGridViewTextBoxColumn()
            {
                Name = "序号",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true
            };
            DataGridViewTextBoxColumn Code = new DataGridViewTextBoxColumn()
            {
                Name = "代码",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true
            };
            DataGridViewTextBoxColumn Name = new DataGridViewTextBoxColumn()
            {
                Name = "名称",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100,
                ReadOnly = true
            };

            dataGridView.Columns.Add(colID);
            dataGridView.Columns.Add(Code);
            dataGridView.Columns.Add(Name);

            foreach (var temp in cols)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
                {
                    HeaderText = temp.OilTableName,
                    Name = temp.OilTableName,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    Tag = temp.OilTableColID,
                    MinimumWidth = 145,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                dataGridView.Columns.Add(column);
            }
            #endregion      
        
            #region "初始化所有行"
            dataGridView.Rows.Clear();
            int col = 0;
            foreach (OilTableRowEntity temp in rows)
            {
                int index = dataGridView.Rows.Add();
                dataGridView.Rows[index].Cells["序号"].Value = col;
                dataGridView.Rows[index].Cells["代码"].Value = temp.itemCode;
                dataGridView.Rows[index].Cells["名称"].Value = temp.itemName;
                dataGridView.Rows[index].Tag = temp.ID;
                col++;
            }
            #endregion

            #region "赋值"
            if (Datas != null)
            {
                for (int i = 0; i < Datas.Count; i++)
                {
                    string tableName = colDic[Datas[i].oilTableColID].OilTableName;
                    int temp = rowIDDic[Datas[i].oilTableRowID];
                    dataGridView.Rows[temp].Cells[tableName].Value = Datas[i].calData;
                }
            }
            #endregion 
        }
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="OilDataCols"></param>
        /// <param name="OilDataRows"></param>
        /// <param name="allDatas"></param>
        /// <param name="strTableName"></param>
        private void InitDataGridView(string strTableName,DataGridView dataGridView, List<OilDataSearchColEntity> OilDataCols, List<OilDataSearchRowEntity> OilDataRows, List<OilDataSearchEntity> allDatas)
        {
            #region "内部计算"
            List<OilDataSearchColEntity> Cols = OilDataCols.Where(o => o.OilTableName.Contains(strTableName)).OrderBy(o => o.itemOrder).ToList();
            List<OilDataSearchRowEntity> Rows = OilDataRows.Where(o => o.OilDataColID == Cols[0].ID).OrderBy(o=>o.OilTableRow.itemOrder).ToList();
            List<OilTableRowEntity> TableRows = new List<OilTableRowEntity>();
            for (int i = 0; i < Rows.Count; i++)
            {
                OilTableRowEntity oilTableRow = OilTableRowBll._OilTableRow.Where(o => o.ID == Rows[i].OilTableRowID).FirstOrDefault();
                TableRows.Add(oilTableRow);
            }

            List<OilDataSearchEntity> Datas = new List<OilDataSearchEntity>();
            for (int i = 0; i < Cols.Count; i++)
            {
                List<OilDataSearchEntity> Data = allDatas.Where(o => o.oilTableColID == Cols[i].OilTableColID).ToList();
                Datas.AddRange(Data);
            }

            initData(dataGridView, Cols, TableRows, Datas);

            if (strTableName == "原油信息")
                this._dgvInfoNeedSave = false;
            else if (strTableName == "原油性质")
                this._dgvWholeNeedSave = false;
            else if (strTableName == "石脑油")
                this._dgvNaphthaNeedSave = false;
            else if (strTableName == "航煤")
                this._dgvAviationKeroseneNeedSave = false;
            else if (strTableName == "柴油")
                this._dgvDieselNeedSave = false;
            else if (strTableName == "VGO")
                this._dgvVGONeedSave = false;
            else if (strTableName == "渣")
                this._dgvResidualNeedSave = false;
            #endregion      
        }
        #endregion        
        
        #region "表格编辑"
        /// <summary>
        /// 第一个表格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView1.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView1, str, e.ColumnIndex, e.RowIndex);
        }
        /// <summary>
        /// 第二个单元格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView2.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView2, str, e.ColumnIndex, e.RowIndex);
        }
        /// <summary>
        /// 第三个单元格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView3.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView3, str, e.ColumnIndex, e.RowIndex);
        }
        /// <summary>
        /// 第四个单元格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView4.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView4, str, e.ColumnIndex, e.RowIndex);
        }
        /// <summary>
        /// 第五个单元格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView5_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView5.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView5, str, e.ColumnIndex, e.RowIndex);
        }
        /// <summary>
        /// 第六个单元格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView6_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView6.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView6, str, e.ColumnIndex, e.RowIndex);
        }
        /// <summary>
        /// 第七个单元格编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView7_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView7.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView7, str, e.ColumnIndex, e.RowIndex);
        }
        #endregion 

        #region " 快捷键"
        /// <summary>
        /// 剪贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 剪贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            oilEdit.CPasteClipboardValue(this._selectGridView);
            //从输入列表和数据库中删除数据
            oilEdit.CDeleteValues(this._selectGridView);
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            oilEdit.CCopyToClipboard(this._selectGridView);     
        }

        private void 粘帖ToolStripMenuItem_Click(object sender, EventArgs e)
        {           
            oilEdit.CPasteClipboardValue(this._selectGridView);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //从输入列表和数据库中删除数据
            oilEdit.CDeleteValues(this._selectGridView);
        }
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            oilEdit.CSave(this._selectGridView);//粘帖
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {           
            this._selectGridView = this.tabControl1.SelectedTab.Controls[0] as DataGridView;
        }
        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {               
                if (e.Modifiers == Keys.Control)
                {                  
                    switch (e.KeyCode)
                    {
                        case Keys.X:
                            oilEdit.CPasteClipboardValue(this._selectGridView);
                            //从输入列表和数据库中删除数据
                            oilEdit.CDeleteValues(this._selectGridView);
                            break;
                        case Keys.C:
                            oilEdit.CCopyToClipboard(this._selectGridView);
                            break;
                        case Keys.V:
                            oilEdit.CPasteClipboardValue(this._selectGridView);
                            break;
                        case Keys.S:
                            //this.IsBusy = true;
                            oilEdit.CSave(this._selectGridView);
                            //this.IsBusy = false;
                            break;
                    }
                    
                }
                if (e.KeyCode == Keys.Delete)
                {
                    //从输入列表和数据库中删除数据
                    oilEdit.CDeleteValues(this._selectGridView);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        #endregion   

        #region "快捷键"
        /// <summary>
        /// 原油信息表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.ColumnIndex < 3)
                    this.tabControl1.ContextMenuStrip = null;
                else
                    this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;           
            }
        }
        /// <summary>
        /// 原油性质
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.ColumnIndex < 3)
                    this.tabControl1.ContextMenuStrip = null;
                else
                    this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            }
        }
        /// <summary>
        /// 石脑油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView3_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.ColumnIndex < 3)
                    this.tabControl1.ContextMenuStrip = null;
                else
                    this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            }
        }
        /// <summary>
        /// 煤油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView4_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.ColumnIndex < 3)
                    this.tabControl1.ContextMenuStrip = null;
                else
                    this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            }
        }
        /// <summary>
        /// 柴油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView5_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.ColumnIndex < 3)
                    this.tabControl1.ContextMenuStrip = null;
                else
                    this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            }
        }
        /// <summary>
        /// 蜡油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView6_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.ColumnIndex < 3)
                    this.tabControl1.ContextMenuStrip = null;
                else
                    this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            }
        }
        /// <summary>
        /// 渣油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView7_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.ColumnIndex < 3)
                    this.tabControl1.ContextMenuStrip = null;
                else
                    this.tabControl1.ContextMenuStrip = this.contextMenuStrip1;
            }
        }
        #endregion 

        #region "判断数据是否需要保存"
        /// <summary>
        /// 原油信息表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (this.dataGridView1.Created)
                this._dgvInfoNeedSave = true;
                this._isValueChange = true;
        }
        /// <summary>
        /// 原油性质表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (this.dataGridView2.Created)
                this._dgvWholeNeedSave = true;
                this._isValueChange = true;
        }
        /// <summary>
        /// 石脑油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (this.dataGridView3.Created)
                this._dgvNaphthaNeedSave = true;
                this._isValueChange = true;
        }
        /// <summary>
        /// 煤油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView4_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (this.dataGridView4.Created)
                this._dgvAviationKeroseneNeedSave = true;
                this._isValueChange = true;
        }
        /// <summary>
        /// 柴油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView5_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (this.dataGridView5.Created)
                this._dgvDieselNeedSave = true;
                this._isValueChange = true;
        }
        /// <summary>
        /// 蜡油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView6_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (this.dataGridView6.Created)
                this._dgvVGONeedSave = true;
                this._isValueChange = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView7_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (this.dataGridView7.Created)
                this._dgvResidualNeedSave = true;
                this._isValueChange = true;
        }

        #endregion 
        /// <summary>
        /// 判断窗体数据是否需要保存
        /// </summary>
        /// <returns></returns>
        private bool IsChange()
        {          
            if (this._dgvInfoNeedSave || this._dgvNaphthaNeedSave || this._dgvWholeNeedSave
                || this._dgvAviationKeroseneNeedSave || this._dgvDieselNeedSave || this._dgvResidualNeedSave
                || this._dgvVGONeedSave)
                this._isChange  = true;

            return this._isChange;
        }
        /// <summary>
        /// 保存到C库
        /// </summary>
        private void SaveC()
        {
            if (this._dgvInfoNeedSave)
            {
                oilEdit.CSave(this.dataGridView1);
                
            
                this._dgvInfoNeedSave = false;
            }
            if (this._dgvWholeNeedSave)
            {
                oilEdit.CSave(this.dataGridView2);
           
                this._dgvWholeNeedSave = false;
            }
            if (this._dgvNaphthaNeedSave)
            {
                oilEdit.CSave(this.dataGridView3);
              
                this._dgvNaphthaNeedSave = false;
            }
            if (this._dgvAviationKeroseneNeedSave)
            {
                oilEdit.CSave(this.dataGridView4);
 
                this._dgvAviationKeroseneNeedSave = false;
            }
            if (this._dgvDieselNeedSave)
            {
                oilEdit.CSave(this.dataGridView5);
          
                this._dgvDieselNeedSave = false;
            }
            if (this._dgvVGONeedSave)
            {
                oilEdit.CSave(this.dataGridView6);
          
                this._dgvVGONeedSave = false;
            }
            if (this._dgvResidualNeedSave)
            {
                oilEdit.CSave(this.dataGridView7);
          
                this._dgvResidualNeedSave = false;           
            }
        }
        /// <summary>
        /// 关闭窗体的保存提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmOilDataC_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsChange())
            {
                DialogResult r = MessageBox.Show("是否保存数据！", "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (r == DialogResult.Yes)
                {
                    this.SaveC();

                    if (this._isValueChange)
                    {
                        var oilInfoB = new OilInfoBAccess().Get(this._oilInfoID);
                        oilInfoB.updataDate = DateTime.Now.ToString(LongDateFormat);
                        OilBll.updateOilInfoB(oilInfoB);

                        FrmMain frmMain = (FrmMain)this.MdiParent;
                        FrmOpenC frmOpenC = (FrmOpenC)frmMain.GetChildFrm("frmOpenC");
                        if (frmOpenC != null)  //如果打开原油库A的窗口存在，则更新
                        {
                            frmOpenC.refreshGridList();
                        }
                        this._isValueChange = false;
                    }
                  
                    this._isChange = false;
                }
                else if (r == DialogResult.No)
                {
                    this._isChange = false;
                }
                else if (r == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this._dgvInfoNeedSave = true;
            this._isValueChange = true;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this._dgvWholeNeedSave = true;
            this._isValueChange = true;
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this._dgvNaphthaNeedSave = true;
            this._isValueChange = true;
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this._dgvAviationKeroseneNeedSave = true;
            this._isValueChange = true;
        }

        private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
             this._dgvDieselNeedSave = true;
                this._isValueChange = true;
        }
    

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
                this._dgvVGONeedSave = true;
                this._isValueChange = true;
        }

        private void dataGridView7_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this._dgvResidualNeedSave = true;
                this._isValueChange = true;
        }

      
    }
}
