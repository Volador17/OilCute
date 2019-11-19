using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System.Windows.Forms;
using ZedGraph;
using System.Threading;
using System.Drawing;
using RIPP.OilDB.UI.GridOil;
using System.ComponentModel;
using RIPP.Lib;
using RIPP.OilDB.UI.MergeCellsDataGridView;
namespace RIPP.OilDB.BLL.ToolBox
{ 
    /// <summary>
    /// 输出曲线
    /// </summary>
    public enum EnumAxis
    {
        [Description("X轴")]
        X = 0,

        [Description("Y左1轴")]
        YL1 = 1,

        [Description("Y左2轴")]
        YL2 = 2,

        [Description("Y左3轴")]
        YL3 = 4,

        [Description("Y右1轴")]
        YR1 = 8,

        [Description("Y右2轴")]
        YR2 = 16,

        [Description("Y右3轴")]
        YR3 = 32
 
    };
    public class OutputCurvesBll : IOutputCurves
    {
        private DataGridView _axisDgv = null;
        private DataGridView _curveDataDgv = null;
        private ZedGraphControl _zedGraphOutputCurve;
        private ComboBox _cellCmb = new ComboBox();   //cmbox控件
        private ComboBox _unitCellCmb = new ComboBox();   //cmbox控件
        private List<S_ParmEntity> _unitItemList = new List<S_ParmEntity>();
        private List<OutputAxisEntity> _axisItemList = new List<OutputAxisEntity>();
        private readonly int _curveDataDgvCol = 20;//数据列
        private readonly int _curveDataDgvRow = 100;//数据行
        private int _iSymbolType = 0;//
        private FontSpec _fontSpec = new FontSpec();
        /// <summary>
        /// 
        /// </summary>
        public FontSpec fontSpec
        {
            get { return this._fontSpec;}
            set { this._fontSpec = value;}
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="axisDgv"></param>
        /// <param name="curveDataDgv"></param>
        /// <param name="zedGraphOutputCurve"></param>
        public OutputCurvesBll(DataGridView axisDgv, DataGridView curveDataDgv, ZedGraphControl zedGraphOutputCurve)
        {
            this._unitItemList = getParmUnit();
            this._axisDgv = axisDgv;
            this._curveDataDgv = curveDataDgv;
            this._zedGraphOutputCurve = zedGraphOutputCurve;
         
            this._unitCellCmb.Visible = false;                  // 设置下拉列表框不可见          
            this._axisDgv.Controls.Add(this._unitCellCmb);   // 将下拉列表框加入到DataGridView控件中   
            this._axisDgv.CellLeave += _axisDgv_CellLeave;
            this._axisDgv.CellEnter += _axisDgv_CellEnter;
            
            
            
            this._cellCmb.Visible = false;                  // 设置下拉列表框不可见          
            this._cellCmb.SelectedIndexChanged += new EventHandler(cmb_Temp_SelectedIndexChanged);  // 添加下拉列表框事件                               
            this._curveDataDgv.Controls.Add(this._cellCmb);   // 将下拉列表框加入到DataGridView控件中   
            this._fontSpec.Size = 12f;
            this._fontSpec.Family = "仿宋;Times New Roman";
            this._fontSpec.Border.IsVisible = false;
        }

        void _axisDgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool isShow = e.ColumnIndex == 2 && e.RowIndex >= 0 ? true : false;
            unitCellCmbBind(isShow); 
        }
        private void _axisDgv_CellLeave(object sender,DataGridViewCellEventArgs e)
        {
            bool isShow = e.ColumnIndex ==2  && e.RowIndex >= 0 ? true : false;
            if (isShow)
            {
                DataGridViewCell cell = this._axisDgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Value = this._unitCellCmb.Text;
            }           
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_Temp_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._curveDataDgv.CurrentCell.Value = this._cellCmb.SelectedValue;
            this._curveDataDgv[this._curveDataDgv.CurrentCell.ColumnIndex, this._curveDataDgv.CurrentCell.RowIndex + 1].Value = this._axisItemList.Where(o => o.Axis == this._cellCmb.SelectedValue).FirstOrDefault().AxisName;
            this._curveDataDgv.Columns[this._curveDataDgv.CurrentCell.ColumnIndex].Tag = (OutputAxisEntity)this._cellCmb.SelectedItem;
        }
        /// <summary>
        /// 加载单位列表
        /// </summary>
        /// <returns></returns>
        private List<S_ParmEntity> getParmUnit()
        {        
            var parmTypeList = new S_ParmTypeAccess().Get("1=1");
            var parmList = new S_ParmAccess().Get("1=1");
            var unitList = parmList.Where(o => o.parmTypeID == parmTypeList.Where(t => t.code == "UNIT").FirstOrDefault().ID).ToList();
            
            
            S_ParmEntity square = new S_ParmEntity() { 
            parmName = "mm²/s",
            parmValue = "mm²/s",
            parmTypeID = parmTypeList.Where(t => t.code == "UNIT").FirstOrDefault().ID
            };

            S_ParmEntity cube = new S_ParmEntity()
            {
                parmName = "g/cm³",
                parmValue = "g/cm³",
                parmTypeID = parmTypeList.Where(t => t.code == "UNIT").FirstOrDefault().ID
            };
            unitList.Add(square);
            unitList.Add(cube);
            return unitList;
        }

        /// <summary>
        /// 初始化坐标轴表格
        /// </summary>
        /// <param name="axisDgv"></param>
        public void initAxisDgv()
        {
            _axisDgv.Columns.Clear();
            _axisDgv.Rows.Clear();
            //_axisDgv.SortOrder = SortOrder.None;
            #region
            DataGridViewCellStyle tempCellStyle = new DataGridViewCellStyle();
            tempCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            _axisDgv.Columns.Add(
               new DataGridViewTextBoxColumn()
               {
                   Name = "axis",
                   HeaderText = "坐标轴",
                   Width = 100,
                   ReadOnly = true,
                   DefaultCellStyle = tempCellStyle,
                   SortMode = DataGridViewColumnSortMode.NotSortable 
               });

            _axisDgv.Columns.Add(
                 new DataGridViewTextBoxColumn()
                 {
                     Name = "name",
                     HeaderText = "名称",
                     Width = 100,
                     SortMode = DataGridViewColumnSortMode.NotSortable 
                 });

            _axisDgv.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    Name = "unit",
                    HeaderText = "单位",
                    Width = 100,
                    SortMode = DataGridViewColumnSortMode.NotSortable 
                });
            _axisDgv.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    Name = "DownLimit",
                    HeaderText = "范围下限",
                    Width = 100,
                    SortMode = DataGridViewColumnSortMode.NotSortable 
                });
            _axisDgv.Columns.Add(
                new DataGridViewTextBoxColumn()
                {
                    Name = "UpLimit",
                    HeaderText = "范围上限",
                    Width = 100,
                    SortMode = DataGridViewColumnSortMode.NotSortable 
                });

            _axisDgv.Columns.Add(
              new DataGridViewTextBoxColumn()
              {
                  Name = "DecNumber",
                  HeaderText = "小数位数",
                  Width = 100,
                  SortMode = DataGridViewColumnSortMode.NotSortable 
              });
            #endregion

            #region
            _axisDgv.RowHeadersWidth = 30;
            _axisDgv.Rows.Add(7);
            _axisDgv.Rows[0].Tag = EnumAxis.X ;
            _axisDgv.Rows[1].Tag = EnumAxis.YL1 ;
            _axisDgv.Rows[2].Tag = EnumAxis.YL2 ;
            _axisDgv.Rows[3].Tag = EnumAxis.YL3 ;
            _axisDgv.Rows[4].Tag = EnumAxis.YR1 ;
            _axisDgv.Rows[5].Tag = EnumAxis.YR2 ;
            _axisDgv.Rows[6].Tag = EnumAxis.YR3 ;

            _axisDgv.Rows[0].Cells["axis"].Value = EnumAxis.X.GetDescription();
            _axisDgv.Rows[1].Cells["axis"].Value = EnumAxis.YL1.GetDescription();
            _axisDgv.Rows[2].Cells["axis"].Value = EnumAxis.YL2.GetDescription();
            _axisDgv.Rows[3].Cells["axis"].Value = EnumAxis.YL3.GetDescription();
            _axisDgv.Rows[4].Cells["axis"].Value = EnumAxis.YR1.GetDescription();
            _axisDgv.Rows[5].Cells["axis"].Value = EnumAxis.YR2.GetDescription();
            _axisDgv.Rows[6].Cells["axis"].Value = EnumAxis.YR3.GetDescription();


            _axisDgv.Rows[0].Cells["name"].Value = "温度";
            _axisDgv.Rows[1].Cells["name"].Value = "收率";
            _axisDgv.Rows[2].Cells["name"].Value = "密度（20℃）";
            _axisDgv.Rows[3].Cells["name"].Value = "粘度";
            _axisDgv.Rows[4].Cells["name"].Value = "硫含量";
            _axisDgv.Rows[5].Cells["name"].Value = "氮含量";
            _axisDgv.Rows[6].Cells["name"].Value = "凝点";


            _axisDgv.Rows[0].Cells["unit"].Value = "℃";
            _axisDgv.Rows[1].Cells["unit"].Value = "%";
            _axisDgv.Rows[2].Cells["unit"].Value = "g/cm³";
            _axisDgv.Rows[3].Cells["unit"].Value = "mm²/s";
            _axisDgv.Rows[4].Cells["unit"].Value = "%";
            _axisDgv.Rows[5].Cells["unit"].Value = "ug/g";
            _axisDgv.Rows[6].Cells["unit"].Value = "℃";
            
            #endregion

            _axisDgv.Dock = DockStyle.Fill;
        }
        public void reFreshAxisList()
        {
            this._axisItemList = getAxisFromDgv();
        }
        /// <summary>
        /// 初始化曲线数据表格
        /// </summary>
        /// <param name="CurveDataDgv"></param>
        public void initCurveDataDgv()
        {           
            _curveDataDgv.Columns.Clear();
            _curveDataDgv.Rows.Clear();
            _curveDataDgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            #region
            _curveDataDgv.Columns.Add(
                    new DataGridViewTextBoxColumn()
                    {
                        Name = "axis",
                        HeaderText = "",
                        Width  = 100,
                        MinimumWidth = 100,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                        ReadOnly = true,
                        Frozen = true ,
                        SortMode = DataGridViewColumnSortMode.NotSortable 
                    });

            for (int i = 1; i < this._curveDataDgvCol +1; i++)
            {
                _curveDataDgv.Columns.Add(
                    new DataGridViewTextBoxColumn()
                    {
                        Name = "col" + i.ToString(),
                        HeaderText = "",
                        Width  = 80,
                        MinimumWidth = 80,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                        SortMode = DataGridViewColumnSortMode.NotSortable 
                    });
            }
          
            #endregion

            #region
            _curveDataDgv.Rows.Add(_curveDataDgvRow);
            _curveDataDgv.Rows[0].ReadOnly = true;
            _curveDataDgv.Rows[1].ReadOnly = true;
            _curveDataDgv.Rows[0].Frozen = true;
            _curveDataDgv.Rows[1].Frozen = true;
            _curveDataDgv.Rows[2].Frozen = true; 
            _curveDataDgv["axis", 0].Value = "坐标轴";
            _curveDataDgv["axis", 1].Value = "坐标轴名称";
            _curveDataDgv["axis", 2].Value = "曲线名称";
            _curveDataDgv.RowHeadersWidth = 30;
            _curveDataDgv.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            _curveDataDgv.DefaultCellStyle = myStyle.dgdViewCellStyle2();
            #endregion
 
            _curveDataDgv.Dock = DockStyle.Fill;                  
        }
      
        /// <summary>
        /// 绘图控件初始化
        /// </summary>
        public void initZedGraph()
        {
            this._iSymbolType = 0;
            this._zedGraphOutputCurve.GraphPane.Border.IsVisible = false;
            
            this._zedGraphOutputCurve.Dock = DockStyle.Fill;
            this._zedGraphOutputCurve.GraphPane.LineNameLocation = enumLineNameLocation.None;
            this._zedGraphOutputCurve.GraphPane.Title.Text = "";
            this._zedGraphOutputCurve.GraphPane.CurveList.Clear();

            YAxis yAxis = this._zedGraphOutputCurve.GraphPane.YAxisList[0].Clone();
            Y2Axis y2Axis = this._zedGraphOutputCurve.GraphPane.Y2AxisList[0].Clone();

            this._zedGraphOutputCurve.GraphPane.YAxisList.Clear();
            this._zedGraphOutputCurve.GraphPane.YAxisList.Add(yAxis);
            this._zedGraphOutputCurve.GraphPane.Y2AxisList.Clear();
            this._zedGraphOutputCurve.GraphPane.Y2AxisList.Add(y2Axis);

            this._zedGraphOutputCurve.GraphPane.Legend.Position = LegendPos.Bottom;//曲线名称的位置
            this._zedGraphOutputCurve.GraphPane.Legend.FontSpec = this.fontSpec;//曲线名称的位置

            this._zedGraphOutputCurve.GraphPane.XAxis.MajorGrid.IsVisible = true;
            this._zedGraphOutputCurve.GraphPane.YAxis.MajorGrid.IsVisible = true;
            this._zedGraphOutputCurve.GraphPane.XAxis.MajorGrid.DashOff = 3;
            this._zedGraphOutputCurve.GraphPane.YAxis.MajorGrid.DashOff = 3;
            this._zedGraphOutputCurve.GraphPane.XAxis.MajorGrid.DashOn = 3;
            this._zedGraphOutputCurve.GraphPane.YAxis.MajorGrid.DashOn = 3;
            
            this._zedGraphOutputCurve.GraphPane.XAxis.MinorGrid.IsVisible = false;
            this._zedGraphOutputCurve.GraphPane.YAxis.MinorGrid.IsVisible = false;

            this._zedGraphOutputCurve.GraphPane.YAxis.MajorTic.IsOpposite = false;
            this._zedGraphOutputCurve.GraphPane.YAxis.MinorTic.IsOpposite = false;
            this._zedGraphOutputCurve.GraphPane.YAxis.MajorGrid.IsZeroLine = false;
            this._zedGraphOutputCurve.GraphPane.YAxis.Scale.Align = AlignP.Inside; 
        }
        /// <summary>
        /// 获取坐标轴
        /// </summary>
        /// <returns></returns>
        public List<OutputAxisEntity> getAxisFromDgv()
        {
            List<OutputAxisEntity> itemList = new List<OutputAxisEntity>();
            for (int row = 0; row < this._axisDgv.Rows.Count; row++)
            {
                OutputAxisEntity item = new OutputAxisEntity {
                    EAxis = (EnumAxis)this._axisDgv.Rows[row].Tag,
                    Axis = this._axisDgv.Rows[row].Cells["axis"].Value != null ? this._axisDgv.Rows[row].Cells["axis"].Value.ToString() : string.Empty,
                    AxisName = this._axisDgv.Rows[row].Cells["name"].Value != null ? this._axisDgv.Rows[row].Cells["name"].Value.ToString() : string.Empty,
                    Unit= this._axisDgv.Rows[row].Cells["unit"].Value != null ? this._axisDgv.Rows[row].Cells["unit"].Value.ToString() : string.Empty,
                    DownLimit = this._axisDgv.Rows[row].Cells["DownLimit"].Value != null ? this._axisDgv.Rows[row].Cells["DownLimit"].Value.ToString() : string.Empty,
                    UpLimit = this._axisDgv.Rows[row].Cells["UpLimit"].Value != null ? this._axisDgv.Rows[row].Cells["UpLimit"].Value.ToString() : string.Empty,
                    DecNumber = this._axisDgv.Rows[row].Cells["DecNumber"].Value != null ? this._axisDgv.Rows[row].Cells["DecNumber"].Value.ToString() : string.Empty
                };

                itemList.Add(item);
            }

            var tempList = itemList.Where(o => string.IsNullOrWhiteSpace (o.AxisName) != true).ToList();

            return tempList;
        }

        /// <summary>
        /// 绑定下拉列表框,根据参数编码查询绑定
        /// </summary>
        /// <param name="isShow"></param>
        public void axisCellCmbBind(bool isShow)
        {
            if (isShow)
            {
                List<OutputAxisEntity> itemList = this._axisItemList;
                if (itemList.Count > 0)
                {
                    this._cellCmb.ValueMember = "Axis";
                    this._cellCmb.DisplayMember = "Axis";
                    this._cellCmb.DataSource = itemList;
                    this._cellCmb.DropDownStyle = ComboBoxStyle.DropDownList;

                    Rectangle rect = this._curveDataDgv.GetCellDisplayRectangle(this._curveDataDgv.CurrentCell.ColumnIndex, this._curveDataDgv.CurrentCell.RowIndex, false);
                    this._cellCmb.Left = rect.Left;
                    this._cellCmb.Top = rect.Top;
                    this._cellCmb.Width = rect.Width;
                    this._cellCmb.Height = rect.Height + 2;
                    this._cellCmb.Visible = isShow;
                    //this._curveDataDgv.CurrentCell.Value = this._cellCmb.SelectedValue;
                    //this._curveDataDgv[this._curveDataDgv.CurrentCell.ColumnIndex, this._curveDataDgv.CurrentCell.RowIndex + 1].Value = itemList.Where(o => o.Axis == this._cellCmb.SelectedValue).FirstOrDefault().Name;
                }
            }
            else
                this._cellCmb.Visible = isShow;
        }
        /// <summary>
        /// 绑定下拉列表框,根据参数编码查询绑定
        /// </summary>
        /// <param name="isShow"></param>
        public void unitCellCmbBind(bool isShow)
        {
            if (isShow)
            {
                if (this._unitItemList.Count > 0)
                {
                    this._unitCellCmb.ValueMember = "parmName";
                    this._unitCellCmb.DisplayMember = "parmName";
                    this._unitCellCmb.DataSource = this._unitItemList;
                    this._unitCellCmb.DropDownStyle = ComboBoxStyle.DropDown;
                    this._unitCellCmb.SelectedIndex = 0;


                    Rectangle rect = this._axisDgv.GetCellDisplayRectangle(this._axisDgv.CurrentCell.ColumnIndex, this._axisDgv.CurrentCell.RowIndex, false);
                    this._unitCellCmb.Left = rect.Left;
                    this._unitCellCmb.Top = rect.Top;
                    this._unitCellCmb.Width = rect.Width;
                    this._unitCellCmb.Height = rect.Height + 2;
                    this._unitCellCmb.Visible = isShow;
                    this._axisDgv.CurrentCell.Value = this._unitCellCmb.SelectedValue;
                }
            }
            else
                this._unitCellCmb.Visible = isShow;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<OutputCurveEntity> getCurvesFromCurveDataDgv()
        {
            List<OutputCurveEntity> curveList = new List<OutputCurveEntity>();
            int curAxisCol = -1;
            Dictionary<OutputAxisEntity,PointPairList> Dic = new Dictionary<OutputAxisEntity,PointPairList>();

            for (int col = 1; col < this._curveDataDgv.Columns.Count; col++)
            {
                string axis = this._curveDataDgv["col" + col.ToString(), 0].Value != null ? this._curveDataDgv["col" + col.ToString(), 0].Value.ToString() : string.Empty;

                if (axis == EnumAxis.X.GetDescription())//遇到X轴
                {
                    #region 
                    curAxisCol = col;
                    
                    if (Dic.Count>0)
                    {
                        OutputCurveEntity curve = new OutputCurveEntity()
                        {                          
                            X = this._axisItemList.Where(o => o.Axis == axis).FirstOrDefault()
                        };
                        
                        Dictionary<OutputAxisEntity, PointPairList> tempDic = new Dictionary<OutputAxisEntity, PointPairList>();
                        foreach (var key in Dic.Keys)
                        {
                            tempDic.Add((OutputAxisEntity)key.Clone(), Dic[key]);
                        }
                        curve.Curves = tempDic;
                        curveList.Add(curve);
                        Dic.Clear();
                    }
                    
                    continue;
                    #endregion
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(axis))
                        continue;

                    string tempName = this._curveDataDgv["col" + col.ToString(), 1].Value != null ? this._curveDataDgv["col" + col.ToString(), 1].Value.ToString() : string.Empty;
                    string curveName = this._curveDataDgv["col" + col.ToString(), 2].Value != null ? this._curveDataDgv["col" + col.ToString(), 2].Value.ToString() : string.Empty;
                    
                    if (string.IsNullOrWhiteSpace(tempName) || string.IsNullOrWhiteSpace(curveName))//如果有空则跳出                                                  
                        continue;

                    PointPairList list = getPointListFromDgv(curAxisCol,col);
                    
                    OutputAxisEntity outputYAxis = (OutputAxisEntity)this._axisItemList.Where(o => o.Axis == axis).FirstOrDefault().Clone();
                    outputYAxis.CurveName = curveName;
                    if (list.Count > 0 && !string.IsNullOrWhiteSpace (curveName))
                    Dic.Add(outputYAxis, list);
                }
            }
            if (curAxisCol != -1)
            {
                string tempAxis = this._curveDataDgv["col" + curAxisCol.ToString(), 0].Value != null ? this._curveDataDgv["col" + curAxisCol.ToString(), 0].Value.ToString() : string.Empty;

                if (Dic.Count > 0)
                {
                    OutputCurveEntity curve = new OutputCurveEntity()
                    {
                        X = this._axisItemList.Where(o => o.Axis == tempAxis).FirstOrDefault()
                    };
                    Dictionary<OutputAxisEntity, PointPairList> tempDic = new Dictionary<OutputAxisEntity, PointPairList>();
                    foreach (var key in Dic.Keys)
                    {
                        tempDic.Add((OutputAxisEntity)key.Clone(), Dic[key]);
                    }
                    curve.Curves = tempDic;
                    curveList.Add(curve);
                    Dic.Clear();
                }
            }
           
            return curveList;
        }
        /// <summary>
        /// 获取点的集合
        /// </summary>
        /// <param name="curAxisCol">X坐标轴的列</param>
        /// <param name="col">Y坐标轴的列</param>
        /// <returns></returns>
        private PointPairList getPointListFromDgv(int curAxisCol,int col)
        {
            PointPairList list = new PointPairList();

            #region "取出点集合"
            for (int row = 3; row < this._curveDataDgv.Rows.Count; row++)
            {
                string pointX = this._curveDataDgv.Rows[row].Cells["col" + curAxisCol.ToString()].Value != null ? this._curveDataDgv.Rows[row].Cells["col" + curAxisCol.ToString()].Value.ToString() : string.Empty;
                string pointY = this._curveDataDgv.Rows[row].Cells["col" + col.ToString()].Value != null ? this._curveDataDgv.Rows[row].Cells["col" + col.ToString()].Value.ToString() : string.Empty;
                var colXEntity = (OutputAxisEntity)this._curveDataDgv.Columns["col" + curAxisCol.ToString()].Tag;
                var colYEntity = (OutputAxisEntity)this._curveDataDgv.Columns["col" + col.ToString()].Tag;

                if (!string.IsNullOrWhiteSpace(pointX) && !string.IsNullOrWhiteSpace(pointY))
                {
                    double x = double.NaN;
                    double y = double.NaN;
                    try
                    {
                        x = double.Parse(pointX);
                        y = double.Parse(pointY);

                        x = isBetweenDownUp(x, colXEntity);
                        y = isBetweenDownUp(y, colYEntity);
                    }
                    catch
                    {
                        continue;
                    }
                    finally
                    {
                        if (!double.IsNaN(x) && !double.IsNaN(y))
                            list.Add(x, y);
                    }
                }
            }
            #endregion

            return list;
        }


        private double isBetweenDownUp(double num, OutputAxisEntity col)
        { 
            double res = double.NaN ;
            
            if (col == null)
               res = num;
         
            if (col.dDownLimit == null && col.dUpLimit != null)
            {
                if (num < col.dUpLimit)
                    res = num;
            }
            else if (col.dDownLimit != null && col.dUpLimit == null)
            {
                if (num > col.dDownLimit)
                    res = num;
            }
            else if (col.dDownLimit != null && col.dUpLimit != null)
            {
                if (col.dDownLimit < num && num < col.dUpLimit)
                    res = num;
            }
            else
            {
                res = num;
            }
            return res;

        }
        /// <summary>
        /// 绘制曲线
        /// </summary>
        public void drawCurves()
        {
            List<OutputCurveEntity> curveList = getCurvesFromCurveDataDgv();
           
            this._zedGraphOutputCurve.GraphPane.CurveList.Clear();
            
            foreach (var outPutCurve in curveList)
            {
                setTitle(outPutCurve);        
            }
            this._zedGraphOutputCurve.GraphPane.Legend.Border.IsVisible = false;
            this._zedGraphOutputCurve.AxisChange();
            this._zedGraphOutputCurve.Invalidate();           
        }
        
        /// <summary>
        /// 设置绘图的样式
        /// </summary>
        private void setTitle(OutputCurveEntity outPutCurve)
        {
            GraphPane myPane = _zedGraphOutputCurve.GraphPane;
 
            string XAxisTitle = string.Empty;//坐标轴名称
            if (string.IsNullOrEmpty(outPutCurve.X.Unit))
            {
                XAxisTitle = outPutCurve.X.AxisName;//设置Y轴的坐标的名称
            }
            else
            {
                if (outPutCurve.X.Unit.Contains("/"))//符合复合坐标的形式
                    XAxisTitle = outPutCurve.X.AxisName + "/(" + outPutCurve.X.Unit + ")";//设置Y轴的坐标的名称
                else
                    XAxisTitle = outPutCurve.X.AxisName + "/" + outPutCurve.X.Unit;//设置Y轴的坐标的名称
            }

            myPane.XAxis.Title.Text = XAxisTitle;
            myPane.XAxis.Title.FontSpec = this.fontSpec;


            #region "设置X轴上下限"
            if (outPutCurve.X.dDownLimit == null)
                myPane.XAxis.Scale.MinAuto = true;
            else
                myPane.XAxis.Scale.Min = outPutCurve.X.dDownLimit.Value;

            if (outPutCurve.X.dUpLimit == null)
                myPane.XAxis.Scale.MaxAuto = true;
            else
                myPane.XAxis.Scale.Max = outPutCurve.X.dUpLimit.Value;
            #endregion 


            foreach (var curve in outPutCurve.Curves)//同一个X轴，含有多个相同的Y轴
            {
                #region
                LineItem CurveLine = null;
                switch (_iSymbolType)
                {
                    case 0:
                        CurveLine = myPane.AddCurve(curve.Key.CurveName,curve.Value, Color.Black, SymbolType.Square);//添加一条曲线
                        break ;
                    case 1:
                        CurveLine = myPane.AddCurve(curve.Key.CurveName, curve.Value, Color.Black, SymbolType.Square);//添加一条曲线
                        CurveLine.Symbol.Fill = new Fill(Color.Black);
                        break;
                    case 2:
                        CurveLine = myPane.AddCurve(curve.Key.CurveName, curve.Value, Color.Black, SymbolType.Circle);//添加一条曲线
                        break;
                    case 3:
                        CurveLine = myPane.AddCurve(curve.Key.CurveName, curve.Value, Color.Black, SymbolType.Circle);//添加一条曲线
                        CurveLine.Symbol.Fill = new Fill(Color.Black);
                        break;
                    //case 5:
                    //    CurveLine = myPane.AddCurve(curve.Key.CurveName, curve.Value, Color.Black, SymbolType.Diamond);//添加一条曲线
                    //    break;
                    //case 6:
                    //    CurveLine = myPane.AddCurve(curve.Key.CurveName, curve.Value, Color.Black, SymbolType.Diamond);//添加一条曲线
                    //    CurveLine.Symbol.Fill = new Fill(Color.Black);
                    //    break;
                    case 4:
                        CurveLine = myPane.AddCurve(curve.Key.CurveName, curve.Value, Color.Black, SymbolType.Triangle);//添加一条曲线
                        break;
                    case 5:
                        CurveLine = myPane.AddCurve(curve.Key.CurveName, curve.Value, Color.Black, SymbolType.Triangle);//添加一条曲线
                        CurveLine.Symbol.Fill = new Fill(Color.Black);
                        break;
                    default :
                        CurveLine = myPane.AddCurve(curve.Key.CurveName, curve.Value, Color.Black, (SymbolType)_iSymbolType);//添加一条曲线
                        break;
                }
                _iSymbolType++;
                CurveLine.Line.IsSmooth = true;
                CurveLine.Line.SmoothTension = 0.5F;//设置曲线的平滑度
                CurveLine.Label.IsVisible = true; //SymbolType.Diamond;
               

                #endregion

                string AxisTitle = string.Empty;//坐标轴名称
                if (string.IsNullOrEmpty(curve.Key.Unit))
                {
                    AxisTitle = curve.Key.AxisName;//设置Y轴的坐标的名称
                }
                else
                {
                    if (curve.Key.Unit.Contains("/"))//符合复合坐标的形式
                        AxisTitle = curve.Key.AxisName + "/(" + curve.Key.Unit + ")";//设置Y轴的坐标的名称
                    else
                        AxisTitle = curve.Key.AxisName + "/" + curve.Key.Unit;//设置Y轴的坐标的名称
                }
               
                switch (curve.Key.EAxis)//设置Y坐标轴
                {
                    case EnumAxis.YL1:
                        CurveLine.YAxisIndex = 0;

                        myPane.YAxis.Title.Text = AxisTitle;
                        myPane.YAxis.Title.FontSpec.IsBold = false;
                        myPane.YAxis.Title.FontSpec.Size = 12f;
                        myPane.YAxis.Title.FontSpec.Family = "仿宋;Times New Roman";
                        myPane.YAxis.Title.FontSpec.Border.IsVisible = false;


                        myPane.YAxis.Scale.Align = AlignP.Inside;
                                                                    
                        if (curve.Key.dDownLimit == null )
                            myPane.YAxis.Scale.MinAuto = true;
                        else
                            myPane.YAxis.Scale.Min = curve.Key.dDownLimit.Value;

                        if (curve.Key.dUpLimit == null)
                            myPane.YAxis.Scale.MaxAuto  = true;
                        else
                            myPane.YAxis.Scale.Max = curve.Key.dUpLimit.Value;
                         
                        break ;
                    case EnumAxis.YL2:
                    case EnumAxis.YL3:
                        YAxis yAxisYL = myPane.YAxisList.Where(o => o.Title.Text == AxisTitle).FirstOrDefault();

                        if (yAxisYL == null)//判断坐标轴上是否已经添加比例尺
                        {
                            yAxisYL = new YAxis(AxisTitle);
                            
                            yAxisYL.Scale.Align = AlignP.Inside;                            
                            myPane.YAxisList.Add(yAxisYL);
                        }
                        yAxisYL.Title.FontSpec.IsBold = false;
                        yAxisYL.Title.FontSpec.Size = 12f;
                        yAxisYL.Title.FontSpec.Family = "仿宋;Times New Roman";
                        yAxisYL.Title.FontSpec.Border.IsVisible = false;

                        if (EnumAxis.YL2 == curve.Key.EAxis)
                            CurveLine.YAxisIndex = 1;
                        else if (EnumAxis.YL3 == curve.Key.EAxis)
                            CurveLine.YAxisIndex = 2;

                        if (curve.Key.dDownLimit == null)
                            yAxisYL.Scale.MinAuto = true;
                        else
                            yAxisYL.Scale.Min = curve.Key.dDownLimit.Value;

                        if (curve.Key.dUpLimit == null)
                            yAxisYL.Scale.MaxAuto = true;
                        else
                            yAxisYL.Scale.Max = curve.Key.dUpLimit.Value;      
                    break;

                    case EnumAxis.YR1:

                         CurveLine.IsY2Axis = true;
                         CurveLine.YAxisIndex = 0;
                         
                         myPane.Y2Axis.IsVisible = true;
                         myPane.Y2Axis.Title.Text = AxisTitle;
                         myPane.Y2Axis.Title.FontSpec.IsBold = false;
                         myPane.Y2Axis.Title.FontSpec.Size = 12f;
                         myPane.Y2Axis.Title.FontSpec.Family = "仿宋;Times New Roman";
                         myPane.Y2Axis.Title.FontSpec.Border.IsVisible = false;
                         myPane.Y2Axis.Scale.Align = AlignP.Inside;

                         myPane.Y2Axis.MajorTic.IsInside = false;
                         myPane.Y2Axis.MinorTic.IsInside = false;
                         myPane.Y2Axis.MajorTic.IsOpposite = false;
                         myPane.Y2Axis.MinorTic.IsOpposite = false;

                         myPane.Y2Axis.Scale.Align = AlignP.Inside;

                             if (curve.Key.dDownLimit == null)
                                 myPane.Y2Axis.Scale.MinAuto = true;
                             else
                                 myPane.Y2Axis.Scale.Min = curve.Key.dDownLimit.Value;

                             if (curve.Key.dUpLimit == null)
                                 myPane.Y2Axis.Scale.MaxAuto = true;
                             else
                                 myPane.Y2Axis.Scale.Max = curve.Key.dUpLimit.Value;
                         break;
                    case EnumAxis.YR2:
                    case EnumAxis.YR3:
                       
                        Y2Axis yAxisYR = myPane.Y2AxisList.Where(o => o.Title.Text == AxisTitle).FirstOrDefault();

                        if (yAxisYR == null)
                        {
                            yAxisYR = new Y2Axis(AxisTitle);
                            myPane.Y2AxisList.Add(yAxisYR);
                            CurveLine.IsY2Axis = true;
                        }
                        if (EnumAxis.YR2 == curve.Key.EAxis)
                            CurveLine.YAxisIndex = 1;
                        else if (EnumAxis.YR3 == curve.Key.EAxis)
                            CurveLine.YAxisIndex = 2;

                        yAxisYR.IsVisible = true;
                        yAxisYR.Title.FontSpec.IsBold = false;
                        yAxisYR.Title.FontSpec.Size = 12f;
                        yAxisYR.Title.FontSpec.Family = "仿宋;Times New Roman";
                        yAxisYR.Title.FontSpec.Border.IsVisible = false;

                        if (curve.Key.dDownLimit == null)
                            yAxisYR.Scale.MinAuto = true;
                        else
                            yAxisYR.Scale.Min = curve.Key.dDownLimit.Value;

                        if (curve.Key.dUpLimit == null)
                            yAxisYR.Scale.MaxAuto = true;
                        else
                            yAxisYR.Scale.Max = curve.Key.dUpLimit.Value;
                            
                        break; 
                }
                 
            }      
        }

        public void setFont()
        {
            GraphPane myPane = _zedGraphOutputCurve.GraphPane;
            //myPane.IsFontsScaled = false;
            //myPane.fontSpec.IsBold = false; //fontDialog.Font.Bold;
            //myPane.fontSpec.IsUnderline = false; // fontDialog.Font.Underline;
            //myPane.fontSpec.Size = 12;
            //myPane.fontSpec.Family = "仿宋";
            //myPane.IsFontsScaled.fontSpec.Border.IsVisible = false;
         
            foreach (var item in myPane.YAxisList)
                item.Title.FontSpec = this.fontSpec;

            foreach (var item in myPane.Y2AxisList)
                item.Title.FontSpec = this.fontSpec;

            myPane.XAxis.Title.FontSpec = this.fontSpec;
            _zedGraphOutputCurve.AxisChange();
            _zedGraphOutputCurve.Invalidate();
        }
    }
}
