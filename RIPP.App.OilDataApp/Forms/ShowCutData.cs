using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using System.Data.OleDb;

namespace RIPP.App.OilDataApp.Forms
{
    public partial class ShowCutData : Form
    {
        #region "私有变量"
        /// <summary>
        /// 切割后的B库原油
        /// </summary>
        private OilInfoBEntity _oilInfoB = new OilInfoBEntity ();
        private Dictionary<string, Dictionary<int, OilDataSearchRowEntity>> DIC = new Dictionary<string, Dictionary<int, OilDataSearchRowEntity>>();
        /// <summary>
        /// 需要填充的表格
        /// </summary>
        private DataGridView _selectGridView = null;
        private GridOilDataEdit oilEdit = null;//用于编辑数据
        /// <summary>
        /// 切割方案
        /// </summary>
        private IList<CutMothedEntity> _cutMotheds = new List<CutMothedEntity>();
        /// <summary>
        /// 收率曲线和性质曲线切割方案
        /// </summary>
        private List<CutMothedEntity> DisCutMotheds = null;
        /// <summary>
        /// 渣油切割方案
        /// </summary>
        private List<CutMothedEntity> ResCutMotheds = null;
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public ShowCutData()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oilInfoBEntity"></param>
        public ShowCutData(OilInfoBEntity oilInfoB, IList<CutMothedEntity> cutMotheds)
        {
            InitializeComponent();
            this._oilInfoB = oilInfoB;
            this.tabControl1.SelectedTab = this.tabPage1;
            this._cutMotheds = cutMotheds;
            breakCutMotheds(cutMotheds);
            this._selectGridView = this.dataGridView1;
            oilEdit = new GridOilDataEdit(this._oilInfoB);
            initDataGridView();
            this.tabControl1.TabPages.Remove(this.tabPage2);
            this.dataGridView1.ReadOnly = true;
            this.dataGridView2.ReadOnly = true;
            this.dataGridView3.ReadOnly = true;
            this.dataGridView4.ReadOnly = true;
        }

        /// <summary>
        /// 分割切割方案
        /// </summary>
        /// <param name="cutMotheds"></param>
        private void breakCutMotheds(IList<CutMothedEntity> cutMotheds)
        {
            DisCutMotheds = new List<CutMothedEntity>();//三次样条插值计算结果，普通曲线切割
            ResCutMotheds = new List<CutMothedEntity>();//线性插值计算结果,渣油曲线切割
            for (int i = 0; i < cutMotheds.Count; i++) //输入的横坐标值
            {
                if (cutMotheds[i].ECP < 1500)
                    DisCutMotheds.Add(cutMotheds[i]);//普通曲线的切割方案
                else if (cutMotheds[i].ECP >= 1500)
                    ResCutMotheds.Add(cutMotheds[i]);//渣油曲线的切割方案
            }
        }
        #endregion 
        
        /// <summary>
        /// 初始化表格
        /// </summary>
        private void initDataGridView()
        {
            List<OilDataBEntity> whole_GCDatas = this._oilInfoB.OilDatas;//此处提出原油性质表和GC表数据。
        
            #region "原油性质表"
            List<OilDataBEntity> wholeDatas = whole_GCDatas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Whole).ToList();
            List<OilTableColEntity> wholeCols = OilTableColBll._OilTableCol.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
            List<OilTableRowEntity> wholeRows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();

            initWhole_GCData(this.dataGridView1, wholeCols, wholeRows, wholeDatas);//向原油性质表中添加数据
            #endregion

            #region "GC表"
            List<OilDataBEntity> GCDatas = whole_GCDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            List<OilTableRowEntity> GCRows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            List<OilTableColEntity> GCCols = OilTableColBll._OilTableCol.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();

            initWhole_GCData(this.dataGridView2, GCCols, GCRows, GCDatas);//向原油性质表中添加数据
            #endregion

            List<ShowCurveEntity> DisshowCurves = this._oilInfoB.OilCutCurves.Where (o=>o.CurveType ==  CurveTypeCode.DISTILLATE || o.CurveType == CurveTypeCode.YIELD).ToList ();//此处提出原油性质表和GC表数据。
            initData(this.dataGridView3, DisshowCurves,DisCutMotheds);


            List<ShowCurveEntity> ResshowCurves = this._oilInfoB.OilCutCurves.Where(o => o.CurveType == CurveTypeCode.RESIDUE).ToList();//此处提出原油性质表和GC表数据。
            initData(this.dataGridView4, ResshowCurves,ResCutMotheds);

            if (this._oilInfoB.strMissValue != null)
                this.richTextBox1.Text = this._oilInfoB.strMissValue.ToString();
        }
        /// <summary>
        /// 初始化原油性质和GC表
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="Datas"></param>
        private void initWhole_GCData(DataGridView dataGridView, List<OilTableColEntity> cols, List<OilTableRowEntity> rows, List<OilDataBEntity> Datas)
        {
            if (Datas == null || cols == null || rows == null)
                return;

            #region "设置列实体  "
            Dictionary<int, OilTableColEntity> colDic = new Dictionary<int, OilTableColEntity>();//判断有多少列
            foreach (var temp in cols)
            {
                if (!colDic.Keys.Contains(temp.ID))
                {
                    colDic.Add(temp.ID, temp);
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
                ReadOnly = true
            };

            dataGridView.Columns.Add(colID);
            dataGridView.Columns.Add(Code);
            dataGridView.Columns.Add(Name);

            foreach (var temp in cols)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
                {
                    HeaderText = temp.colName,
                    Name = temp.colName,
                    Tag = temp.ID
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
            for (int i = 0; i < Datas.Count; i++)
            {
                string tableName = colDic[Datas[i].OilTableCol.ID].colName;
                int temp = rowIDDic[Datas[i].OilTableRow.ID];
                dataGridView.Rows[temp].Cells[tableName].Value = Datas[i].calData;
            }
            #endregion 

        }
        /// <summary>
        /// 初始化普通切割曲线数据
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="Datas"></param>
        private void initData(DataGridView dataGridView, List<ShowCurveEntity> showCurves, IList<CutMothedEntity> cutMotheds)
        {
            if (showCurves == null)
                return;

            #region "设置列实体  "


            Dictionary<string, int> rowIDDic = new Dictionary<string, int>();//表的行和oiltableRowID对应    
            int rowID = 0;//设置行ID=0
            foreach (var temp in showCurves)
            {
                if (!rowIDDic.Keys.Contains(temp.PropertyY))
                {
                    rowIDDic.Add(temp.PropertyY, rowID);
                    rowID++;
                }
            }
            #endregion

            #region "初列：序号，项目，代码"
            dataGridView.ReadOnly = true;
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
                ReadOnly = true
            };

            dataGridView.Columns.Add(colID);
            dataGridView.Columns.Add(Code);
            dataGridView.Columns.Add(Name);

            foreach (var temp in cutMotheds)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
                {
                    HeaderText = temp.Name,
                    Name = temp.Name,
                    Tag = temp.Name
                };
                dataGridView.Columns.Add(column);
            }
            #endregion

            #region "初始化所有行"
            dataGridView.Rows.Clear();
            int col = 0;
            foreach (var temp in showCurves)
            {
                int index = dataGridView.Rows.Add();
                dataGridView.Rows[index].Cells["序号"].Value = col;
                dataGridView.Rows[index].Cells["代码"].Value = temp.PropertyY;
                dataGridView.Rows[index].Cells["名称"].Value = temp.PropertyY;
                dataGridView.Rows[index].Tag = temp.ID;
                col++;
            }
            #endregion

            #region "赋值"
            for (int i = 0; i < showCurves.Count; i++)
            {
                int temp = rowIDDic[showCurves[i].PropertyY];
                for (int j = 0; j < showCurves[i].CutDatas.Count; j++)
                {
                    string tableName = showCurves[i].CutDatas[j].CutName;                    
                    dataGridView.Rows[temp].Cells[tableName].Value = showCurves[i].CutDatas[j].CutData;
                }              
            }
            #endregion
        }
       
        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {          
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.X:
                            oilEdit.CPasteClipboardValue(this.dataGridView1);
                            //从输入列表和数据库中删除数据
                            oilEdit.CDeleteValues(this.dataGridView1);
                            break;
                        case Keys.C:
                            oilEdit.CCopyToClipboard(this.dataGridView1);                           
                            break;
                        case Keys.V:
                            oilEdit.CPasteClipboardValue(this.dataGridView1);
                            break;
                    }
                }
                if (e.KeyCode == Keys.Delete)
                {
                    //从输入列表和数据库中删除数据
                    oilEdit.CDeleteValues(this.dataGridView1);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
               // MessageBox.Show("编辑操作失败！ " + ex.Message, "编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.X:
                            oilEdit.CPasteClipboardValue(this.dataGridView2);
                            //从输入列表和数据库中删除数据
                            oilEdit.CDeleteValues(this.dataGridView2);
                            break;
                        case Keys.C:
                            oilEdit.CCopyToClipboard(this.dataGridView2);
                            break;
                        case Keys.V:
                            oilEdit.CPasteClipboardValue(this.dataGridView2);
                            break;
                    }
                }
                if (e.KeyCode == Keys.Delete)
                {
                    //从输入列表和数据库中删除数据
                    oilEdit.CDeleteValues(this.dataGridView2);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                // MessageBox.Show("编辑操作失败！ " + ex.Message, "编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView3_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.X:
                            oilEdit.CPasteClipboardValue(this.dataGridView3);
                            //从输入列表和数据库中删除数据
                            oilEdit.CDeleteValues(this.dataGridView3);
                            break;
                        case Keys.C:
                            oilEdit.CCopyToClipboard(this.dataGridView3);
                            break;
                        case Keys.V:
                            oilEdit.CPasteClipboardValue(this.dataGridView3);
                            break;
                    }
                }
                if (e.KeyCode == Keys.Delete)
                {
                    //从输入列表和数据库中删除数据
                    oilEdit.CDeleteValues(this.dataGridView3);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                // MessageBox.Show("编辑操作失败！ " + ex.Message, "编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView1.CurrentCell.Value; //根据行列号获取单元格的值

            string str  = t2 == null ? string .Empty : t2 .ToString ();
            oilEdit.CPaste(this.dataGridView1, str, e.ColumnIndex, e.RowIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView1.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView2, str, e.ColumnIndex, e.RowIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var t2 = this.dataGridView1.CurrentCell.Value; //根据行列号获取单元格的值

            string str = t2 == null ? string.Empty : t2.ToString();
            oilEdit.CPaste(this.dataGridView3, str, e.ColumnIndex, e.RowIndex);
        }
    }
}
