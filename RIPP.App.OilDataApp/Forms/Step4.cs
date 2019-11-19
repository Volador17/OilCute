using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using System.Data.OleDb;
using System.Windows.Forms;
namespace RIPP.App.OilDataApp.Forms
{
    /// <summary>
    /// step4的代码
    /// </summary>
    public partial class FrmMain
    {
        #region "私有变量"
        private Dictionary<string, Dictionary<int, OilDataSearchRowEntity>> DIC = new Dictionary<string, Dictionary<int, OilDataSearchRowEntity>>();
        /// <summary>
        /// 收率曲线和性质曲线切割方案
        /// </summary>
        private List<CutMothedEntity> DisCutMotheds = null;
        /// <summary>
        /// 渣油切割方案
        /// </summary>
        private List<CutMothedEntity> ResCutMotheds = null;
        private OilTools oilTool = new OilTools();
        #endregion 

        /// <summary>
        /// 初始化step4
        /// </summary>
        private void InitStep4 ()
        {
            try
            {
                this.tabControlStep4.SelectedTab = this.step4tabPage1;
                this.tabControlStep4.TabPages.Remove(this.step4tabPage2);
                breakCutMotheds(this._cutMotheds);
                initDataGridView();
                this.dgvStep4Whole.Refresh();

                this.dgvStep4Whole.ReadOnly = true;
                this.dgvStep4GC.ReadOnly = true;
                this.dgvStep4Com.ReadOnly = true;
                this.dgvStep4Res.ReadOnly = true;
            }
            catch (Exception ex)
            {
                Log.Error( "原油应用数据显示："+ex.ToString());
            }
        }        

        #region "私有函数"
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
        /// <summary>
        /// 初始化表格
        /// </summary>
        private void initDataGridView()
        {
            if (this._oilB == null)
                return;
            List<OilDataBEntity> whole_GCDatas = this._oilB.OilDatas;//此处提出原油性质表和GC表数据。

            #region "原油性质表"
            List<OilDataBEntity> wholeDatas = whole_GCDatas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Whole).ToList();
            List<OilTableColEntity> wholeCols = OilTableColBll._OilTableCol.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
            List<OilTableRowEntity> wholeRows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();

            initWhole_GCData(this.dgvStep4Whole, wholeCols, wholeRows, wholeDatas);//向原油性质表中添加数据
            #endregion

            #region "GC表"
            List<OilDataBEntity> GCDatas = whole_GCDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            List<OilTableRowEntity> GCRows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            List<OilTableColEntity> GCCols = OilTableColBll._OilTableCol.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();

            initWhole_GCData(this.dgvStep4GC, GCCols, GCRows, GCDatas);//向原油性质表中添加数据
            #endregion

            List<ShowCurveEntity> DisshowCurves = this._oilB.OilCutCurves.Where(o => o.CurveType == CurveTypeCode.DISTILLATE || o.CurveType == CurveTypeCode.YIELD).ToList();//此处提出原油性质表和GC表数据。
            initData(this.dgvStep4Com, DisshowCurves, DisCutMotheds);


            List<ShowCurveEntity> ResshowCurves = this._oilB.OilCutCurves.Where(o => o.CurveType == CurveTypeCode.RESIDUE).ToList();//此处提出原油性质表和GC表数据。
            initData(this.dgvStep4Res, ResshowCurves, ResCutMotheds);

            //if (this._oilB.strMissValue != null)
               // this.richTextBox1.Text = this._oilInfoB.strMissValue.ToString();
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
                dataGridView.Rows[temp].Cells[tableName].Value = Datas[i].calShowData;
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

            Dictionary<string, string> rowDic = new Dictionary<string, string>();

            #region "初列：序号、项目、代码"
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

            rowDic.Add("序号", null);
            rowDic.Add("代码", null);
            rowDic.Add("名称", null);

            foreach (var temp in cutMotheds)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
                {
                    HeaderText = temp.Name,
                    Name = temp.Name,
                    Tag = temp.Name
                };
                dataGridView.Columns.Add(column);
                rowDic.Add(temp.Name, string.Empty);
            }
            #endregion

            #region "初始化所有行"
            dataGridView.Rows.Clear();
            List<string> KeyList = rowDic.Keys.ToList();
            foreach (string strKey in KeyList)
            {
                if (strKey.Equals("序号"))
                    rowDic[strKey] = "0";
                else if (strKey.Equals("代码"))
                    rowDic[strKey] = "ICP";
                else if (strKey.Equals("名称"))
                    rowDic[strKey] = "初切点";
                else
                {
                    CutMothedEntity cutMothed = cutMotheds.Where(o => o.Name == strKey).FirstOrDefault();
                    rowDic[strKey] = cutMothed.ICP == -2000 ? string.Empty : cutMothed.ICP.ToString();
                }
            }
            dataGridView.Rows.Add(rowDic.Values.ToArray());

            foreach (string strKey in KeyList)
            {
                if (strKey.Equals("序号"))
                    rowDic[strKey] = "1";
                else if (strKey.Equals("代码"))
                    rowDic[strKey] = "ECP";
                else if (strKey.Equals("名称"))
                    rowDic[strKey] = "终切点";
                else
                {
                    CutMothedEntity cutMothed = cutMotheds.Where(o => o.Name == strKey).FirstOrDefault();
                    rowDic[strKey] = cutMothed.ECP == 2000 ? string.Empty : cutMothed.ECP.ToString();
                }
            }
            dataGridView.Rows.Add(rowDic.Values.ToArray());
            #endregion

            #region "赋值"
            for (int i = 0; i < showCurves.Count; i++)
            {
                foreach (string strKey in KeyList)
                {
                    if (strKey.Equals("序号"))
                        rowDic[strKey] = (i + 2).ToString();
                    else if (strKey.Equals("代码"))
                        rowDic[strKey] = showCurves[i].PropertyY;
                    else if (strKey.Equals("名称"))
                        rowDic[strKey] = showCurves[i].ItemName;
                    else
                    {
                        CutDataEntity cutData = showCurves[i].CutDatas.Where(o => o.CutName == strKey).FirstOrDefault();
                        rowDic[strKey] = cutData == null ? string.Empty : cutData.ShowCutData;
                    }
                }
                dataGridView.Rows.Add(rowDic.Values.ToArray());
            }
            #endregion
        }

        #endregion 

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butStep4Ok_Click(object sender, EventArgs e)
        {
            this.panelStep4.Visible = false;
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butStep4Cancel_Click(object sender, EventArgs e)
        {
            this.panelStep4.Visible = false;
        }
    }
}
