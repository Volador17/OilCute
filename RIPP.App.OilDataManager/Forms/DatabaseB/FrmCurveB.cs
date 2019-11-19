using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;
using ZedGraph;
using RIPP.OilDB.Data;

namespace RIPP.App.OilDataManager.Forms.DatabaseB
{
    public partial class FrmCurveB: Form
    {
        #region 私有变量
        /// <summary>
        /// 一条性质曲线
        /// </summary>
        private CurveEntity _curve = null ; //一条性质曲线
        /// <summary> 
        /// 判断数据是否有改动，是否需要保存数据  _isChange = false
        /// </summary>
        private bool _isChange = false;//判断数据是否有改动，是否需要保存数据
        /// <summary>
        /// 馏分性质曲线和馏分曲线的固定18个点(float)
        /// </summary>
        private readonly List<float> B_XList = new List<float>() { 15, 60, 100, 140, 160, 180, 200, 220, 240, 280, 320, 350, 400, 425, 450, 500, 540, 560 };//固定x轴点
        /// <summary>
        /// 放大缩小的比例系数
        /// </summary>
        private readonly double scaleRate = 1.1; //放大缩小的比例系数
        /// <summary>
        /// 添加曲线后X轴的最大值
        /// </summary>
        private double axisMax = 0;// 添加曲线后坐标轴x轴最大值，用于适窗和全屏
        /// <summary>
        /// 添加曲线后X轴的最小值
        /// </summary>
        private double axisMin = 0; //添加曲线后坐标轴x轴最小值，用于适窗和全屏
        /// <summary>
        /// 添加曲线后Y轴的最大值
        /// </summary>
        private double aYisMax = 0; //添加曲线后坐标轴Y轴的最大值
        /// <summary>
        /// 添加曲线后Y轴的最小值
        /// </summary>
        private double aYisMin = 0;//添加曲线后坐标轴Y轴的最小值
        /// <summary>
        /// 点对应的真实B库的下标,在得到点的时候赋值
        /// </summary>
        private int BcellIndex = -1;
        /// <summary>
        /// 最后一行的行行号_lastRowNum = 1;
        /// </summary>
        private readonly int _lastRowNum = 1;
        #endregion 

        #region 构造函数
        /// <summary>
        /// 空函数
        /// </summary>
        public FrmCurveB()
        {
            InitializeComponent();          
        }

        /// <summary>
        /// 够造函数
        /// </summary>
        /// <param name="_curve">一条性质曲线</param>
        public FrmCurveB(CurveEntity curve)
        {
            InitializeComponent();
            this.zedGraphControl1.ZedExpand.ZoomChoose = false;// 放大为false
            this.zedGraphControl1.ZedExpand.IsMoving = true;//移动为true
            this.zedGraphControl1.ZedExpand.IsopenB = true; //打开B库
            init(curve); //初始化函数
        }

        #endregion

        #region 数据初始化

        /// <summary>
        /// 根据曲线类别初始化
        /// </summary>
        /// <param name="typeCode"></param>
        public void init(CurveEntity curve) 
        {
            this._curve = curve;                        //获得一条曲线 
            //initTable();
            InitStyle();                                   //表格样式         
            _setColHeader();                          //表格表头
            _setRowValues();                        //设置行的头和值
            RIPP.OilDB.UI.GridOil.myStyle.setToolStripStyle(this.toolStrip);

            drawCurve();                           //画出曲线

            this.dataGridView.CellEndEdit += new DataGridViewCellEventHandler(dataGridView_CellEndEdit);//单元格编辑结束事件
            //this.zedGraph1.MouseDown += new MouseEventHandler(zedGraph1_MouseDown); 
            this.zedGraphControl1.MouseMove +=new MouseEventHandler(zedGraphControl1_MouseMove);//鼠标移动绑定事件
            this.zedGraphControl1.MouseClick +=new MouseEventHandler(zedGraphControl1_MouseClick);//鼠标单击绑定事件
        }

        #endregion 

        #region 表格样式，设置表头和列头，设置单元格的值
        /// <summary>
        /// 初始化表格
        /// </summary>
        private void  initTable ()
        {
            if (this._curve.curveTypeID != (int)CurveTypeCode.RESIDUE)
            {


            }
            else if (this._curve.curveTypeID == (int)CurveTypeCode.RESIDUE)
            { 
            
            
            }
        
        }
        /// <summary>
        /// 表格样式
        /// </summary>
        private void InitStyle()
        {
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            this.dataGridView.DefaultCellStyle = myStyle.dgdViewCellStyle2();

            this.dataGridView.BorderStyle = BorderStyle.None;
            this.dataGridView.RowHeadersWidth = 20;                
            this.dataGridView.MultiSelect = false;
            //this.dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }
        /// <summary>
        /// 设置列头
        /// </summary>
        private void _setColHeader()
        {
            this.dataGridView.Columns.Clear();

            # region 固定的列

            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "项目",
                ReadOnly = true,
                Name = "itemName",
                Tag = "",
                Width = 110,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Frozen = true
            });

            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "单位",
                ReadOnly = true,
                Name = "itemUnit",
                Tag = "",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Frozen = true
            });

            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "库类别",
                ReadOnly = true,
                Name = "itemCode",
                Tag = "",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Frozen = true
            });

            #endregion

            if (this._curve != null)
            {
                for (int i = 0; i < this._curve.X.Count(); )
                {
                    DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                    col.HeaderText = "Cut" + ++i;
                    col.Width = 70;
                    this.dataGridView.Columns.Add(col);
                }
            }
        }

        /// <summary>
        /// 单元格的x轴和Y轴
        /// </summary>
        private void _setRowValues()
        {
            if (this._curve == null)
                return;

            string itemCodeX = _curve.propertyX;//X轴行代码
            string itemCodeY = _curve.propertyY;//Y轴行代码
            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide || o.oilTableTypeID == (int)EnumTableType.Residue).ToList();

            OilTableRowEntity tempRowX = tempRowList.Where(o => o.itemCode == itemCodeX).FirstOrDefault();//x轴行实体类
            OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == itemCodeY).FirstOrDefault();//y轴行实体类
            this.zedGraphControl1.ZedExpand.DecNumber = tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;
            //this.zedGraphControl1.ZedExpand.DecNumber = tempRowY.decNumber;
           
            GridOilRow rowX = new GridOilRow();
            rowX.RowEntity = tempRowX;
            rowX.CreateCells(this.dataGridView, tempRowX.itemName, tempRowX.itemUnit, "应用库");//设置X轴行的前三列的值
            rowX.ReadOnly = true;
            this.dataGridView.Rows.Add(rowX);
            tempRowX.RowIndex = rowX.Index;
            for (int i = 0; i < this._curve.X.Count(); i++)
            {
                this.dataGridView.Rows[0].Cells[i + 3].Value = Math.Round(this._curve.X[i], this._curve.decNumber);
            }
        
            GridOilRow rowY = new GridOilRow();
            rowY.RowEntity = tempRowY;
            rowY.CreateCells(this.dataGridView, tempRowY.itemName, tempRowY.itemUnit, "应用库");//设置Y轴行的前三列的值
            rowY.ReadOnly = false;
            this.dataGridView.Rows.Add(rowY);
            tempRowY.RowIndex = rowY.Index;
             
            for (int i = 0; i < this._curve.Y.Count(); i++)
            {
                if (float.MaxValue != this._curve.Y[i])
                    this.dataGridView.Rows[1].Cells[i + 3].Value = Math.Round(this._curve.Y[i], this._curve.decNumber);
                else
                    this.dataGridView.Rows[1].Cells[i + 3].Value = null;//如果Y的值为float的最大值，则表示该项数据为空              
            }


        }
        #endregion

        #region 绘图函数
        /// <summary>
        /// 将单元格中的数据绘制成一条曲线
        /// </summary>
        public void drawCurve()
        {
            if (_curve != null && this.dataGridView.RowCount >= 2) //有当前曲线，并且当前曲线有数据才画曲线
            {
                List<CurveAEntity> curveAs = new List<CurveAEntity>();
                rowToCurve(ref curveAs, 1);

                DrawCurve(curveAs);

                getCurrentCellBDatas();//每次绘图的时候保存单元格中的数据

                axisMax = this.zedGraphControl1.GraphPane.XAxis.Scale.Max;//获取添加曲线调整后的x轴的最大值
                axisMin = this.zedGraphControl1.GraphPane.XAxis.Scale.Min;//获取添加曲线调整后的x轴的最小值
                aYisMax = this.zedGraphControl1.GraphPane.YAxis.Scale.Max;//获取添加曲线调整后的y轴的最大值
                aYisMin = this.zedGraphControl1.GraphPane.YAxis.Scale.Min;//获取添加曲线调整后的y轴的最小值
            }
            else
            {
                this.zedGraphControl1.GraphPane.CurveList.Clear();
            }
        }

        /// <summary>
        /// 绘制性质曲线
        /// </summary>
        /// <param name="curve">一条性质曲线</param>
        public void DrawCurve(List<CurveAEntity> curveAs)
        {
            this.zedGraphControl1.GraphPane.CurveList.Clear();
            if (curveAs == null)
                return;

            foreach (CurveAEntity curve in curveAs)
            {
                if (curve.X == null)
                    return;
                //this.zedGraph1.GraphPane.AddCurve(this._curve.descript, this._curve.X.ToArray(), this._curve.Y.ToArray(), this._curve.Color, SymbolType.None);
                if (curve.isRefence == false)  //存入当前要编辑曲线的点
                {
                    PointPairList list = this.zedGraphControl1.ZedExpand.PointList;
                    list.Clear();
                    for (int i = 0; i < curve.X.Count(); i++)
                    {
                        list.Add(curve.X[i], curve.Y[i]);
                    }
                    LineItem CurveLine = this.zedGraphControl1.GraphPane.AddCurve(curve.descript, list, curve.Color, SymbolType.Square);
                    CurveLine.Line.IsSmooth = true;
                    CurveLine.Line.SmoothTension = 0.5F;//设置曲线的平滑度
                    CurveLine.Line.IsOptimizedDraw = true;
                }
                else
                {
                    LineItem myLine = this.zedGraphControl1.GraphPane.AddCurve(curve.descript, curve.X.ToArray(), curve.Y.ToArray(), Color.Blue, SymbolType.Circle);
                    myLine.Line.IsVisible = false;//设置曲线为透明，不显示曲线，仅仅显示点
                }
                this.setTitle(curve);
                this.zedGraphControl1.AxisChange();
                this.zedGraphControl1.Refresh();
            }
        }
        /// <summary>
        /// 设置绘图的样式
        /// </summary>
        private void setTitle(CurveAEntity curve)
        {
            if (this._curve == null)
                return;
            GraphPane myPane = this.zedGraphControl1.GraphPane;

            this.zedGraphControl1.IsEnableVEdit = curve.isRefence ? false : true;
            myPane.Legend.IsVisible = false;
            //字体
            myPane.Border.Width = 0;
            myPane.Border.Color = Color.White;
            myPane.Title.FontSpec.IsBold = false;
            myPane.XAxis.Title.FontSpec.IsBold = false;//设置x轴的文字粗体
            myPane.YAxis.Title.FontSpec.IsBold = false;

            myPane.XAxis.Scale.MaxGrace = 0;
            myPane.XAxis.Scale.MinGrace = 0;

            //myPane.Title.Text = curve.descript;
            //myPane.YAxis.Title.Text = curve.propertyY;
            //myPane.XAxis.Title.Text = curve.propertyX;

            if (curve.propertyX == "ECP")
            {
                myPane.YAxis.Title.Text = curve.descript;
                myPane.XAxis.Title.Text = "初切点";
                myPane.Title.Text = "初切点 - " + curve.descript;
            }
            else if (curve.propertyX == "WY")
            {
                myPane.YAxis.Title.Text = curve.descript;
                myPane.XAxis.Title.Text = "质量单收率";
                myPane.Title.Text = "质量单收率 - " + curve.descript;
            }
        }
        /// <summary>
        /// 根据行生成曲线，并加到曲线列表中
        /// </summary>
        /// <param name="curveAs"></param>
        /// <param name="rowYIndex"></param>
        public void rowToCurve(ref List<CurveAEntity> curveAs, int rowYIndex)
        {
            if (rowYIndex < 1)  //X轴在Y轴的前一行
                return;

            int rowXIndex = rowYIndex - 1; //rowXIndex要画曲线的行，rowYIndex要画曲线的列
            GridOilRow rowY = this.dataGridView.Rows[rowYIndex] as GridOilRow;
            GridOilRow rowX = this.dataGridView.Rows[rowXIndex] as GridOilRow;

            if (rowY.RowEntity == null || rowX.RowEntity == null)
                return;

            CurveAEntity curver = new CurveAEntity();
            curver.propertyX = rowX.RowEntity.itemCode;
            curver.propertyY = rowY.RowEntity.itemCode;
            curver.Color = Color.Red ;
            curver.isRefence =  false;
            curver.curveTypeID = 1;
            curver.decNumber = rowY.RowEntity.decNumber == null ? rowY.RowEntity.valDigital : rowY.RowEntity.decNumber.Value;
            //curver.decNumber = rowY.RowEntity.decNumber;
            curver.descript =  rowY.RowEntity.itemName ;
            curver.unit = rowY.RowEntity.itemUnit;
            double x, y;
            curver.X = new double[this.dataGridView.Columns.Count];
            curver.Y = new double[this.dataGridView.Columns.Count];


            for (int i = 0; i < this.dataGridView.Columns.Count; i++)
            {
                curver.X[i] = double.NaN;
                curver.Y[i] = double.NaN;
                if (this.dataGridView.Rows[rowXIndex].Cells[i] != null && this.dataGridView.Rows[rowYIndex].Cells[i] != null)
                    if (this.dataGridView.Rows[rowXIndex].Cells[i].Value != null && this.dataGridView.Rows[rowYIndex].Cells[i].Value != null)
                        if (double.TryParse(this.dataGridView.Rows[rowXIndex].Cells[i].Value.ToString(), out x) && double.TryParse(this.dataGridView.Rows[rowYIndex].Cells[i].Value.ToString(), out y))
                        {
                            curver.X[i] = x;
                            curver.Y[i] = y;
                        }
            }
            if (curver.X.Count() > 0)
                curveAs.Add(curver);
        }

            #endregion

        #region 编辑单元格结束事件、单元格选中事件、鼠标移动事件、窗体关闭提示事件
        /// <summary>
        /// 编辑单元格结束后写入数据并且重新绘图
        /// </summary>
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
             var t2 = this.dataGridView[e.ColumnIndex, e.RowIndex].Value;   //根据行列号获取单元格的值
             double calValue = 0; //单元格的值

             if (t2 != null)
             {
                 if (!double.TryParse(t2.ToString(), out calValue))
                 {
                     MessageBox.Show("要求为浮点数!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     return;
                 }
             }
             drawCurve();//重新绘制曲线
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            //this._isChange = true;//数据修改需要保存
            PointF choosePoint = new PointF();
            PointPairList list = this.zedGraphControl1.ZedExpand.PointList;//获取List列表

            if (col >= 3 && row == 1)
            {
                var cellValueY = this.dataGridView.Rows[row].Cells[col].Value;
                var cellValueX = this.dataGridView.Rows[row - 1].Cells[col].Value;
                if (cellValueY != null && cellValueX != null)
                {
                    if (cellValueY.ToString() != string.Empty && cellValueX.ToString() != string.Empty)//曹志伟添加判断cellValue是否为空
                    {
                        //double valueY = Math.Round(double.Parse(cellValueY.ToString()), 4);
                        //double valueX = double.Parse(cellValueX.ToString());

                        double valueY = Math.Round(double.Parse(cellValueY.ToString()), this.zedGraphControl1.ZedExpand.DecNumber);
                        double valueX = double.Parse(cellValueX.ToString());


                        if (list.Contains(new PointPair(valueX, valueY)))
                        {
                            //PointPair temp = list[col];
                            choosePoint.X = (float)valueX;
                            choosePoint.Y = (float)valueY;
                            // foreach (DataGridViewCell cell1 in this.dataGridView.Rows[_curveRowYIndex].Cells)  //颜色
                            //     cell1.Style = myStyle.dgdViewCellStyle2();
                            //this.dataGridView.Rows[row].Cells[col].Style.BackColor = Color.Red;
                        }
                    }
                }
            }
            this.zedGraphControl1.ZedExpand.DrawPoint(choosePoint, "choosePoint", Color.Red, zedGraphControl1);
        }

        /// <summary>
        /// 绑定鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void zedGraphControl1_MouseMove(object sender, MouseEventArgs e) 
        {
            int rowIndex = 1;
            if (this.dataGridView.Rows.Count < 2)
                return;
            
            int index = this.zedGraphControl1.ZedExpand.Index;
            double x = 0;
            double y = 0;

            if (this.zedGraphControl1.ZedExpand.PointList.Count == 0)
                return;

            if (index < this.zedGraphControl1.ZedExpand.PointList.Count)
            {
                x = this.zedGraphControl1.ZedExpand.PointList[index].X;
                y = this.zedGraphControl1.ZedExpand.PointList[index].Y;
            }
            if (index > 0)
            {
                index = FindRealIndex(x);//获取曲线中的点在单元格中的列号
            }

            if (index > 0)
            {
                BcellIndex = index;               
            }

            GridOilRow row = this.dataGridView.Rows[rowIndex] as GridOilRow;
            if (row.RowEntity == null)
                return;
            if (index > 0)
            {
                row.Cells[index].Value = y;
            }
        }
        /// <summary>
        /// 鼠标点击事件，选中对应的单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraphControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (BcellIndex > -1)
                this.dataGridView.Rows[_lastRowNum].Cells[BcellIndex].Selected = true;      //B库单元格选中事件
            BcellIndex = -1;        //B库值初始化
        }
        /// <summary>
        /// 保存当前曲线
        /// </summary>
        public void SaveCurrentCurve()
        {                             
            if (this._curve.ID > 0)
            {
                #region "曲线的保存"
                CurveDataAccess curveDatasAccess = new CurveDataAccess();
                for (int index = 0; index < this._curve.curveDatas.Count; index++)
                {
                    curveDatasAccess.Delete(this._curve.curveDatas[index].ID);//删除曲线数据
                }

                CurveEntity tempCurve = this._curve;
                tempCurve.curveDatas.Clear();//清除曲线上的点
                List<CurveDataEntity> curveDatas = new List<CurveDataEntity>();

                Dictionary<string, string> currentCurveDatas = getCurrentCellBDatas();//获得当前需要保存的数据
                foreach (string x_Key in currentCurveDatas.Keys)
                {
                    float f_x_Key = 0, f_y_Value = 0;
                    if (float.TryParse(x_Key, out f_x_Key) && float.TryParse(currentCurveDatas[x_Key], out f_y_Value))
                    {
                        CurveDataEntity curveData = new CurveDataEntity()
                        {
                            curveID = tempCurve.ID,
                            xValue = f_x_Key,
                            yValue = f_y_Value
                        };
                        curveDatas.Add(curveData);
                    }
                }
                tempCurve.curveDatas = curveDatas;
                List<CurveEntity> curves = new List<CurveEntity>();
                curves.Add(tempCurve);
                OilBll.saveCurves(curves);//插入曲线数据
                #endregion 
            }          
        }
        
        /// <summary>
        /// 窗体关闭提示事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCurveB_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._isChange == true)
            {
                DialogResult r = MessageBox.Show("是否保存数据！", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (r == DialogResult.Yes && this._curve != null)
                {
                    SaveCurrentCurve();
                    this._isChange = false;
                }
            }
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 获取曲线下标所对应的真实单元格下标的位置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private int FindRealIndex(double  x)
        {
            if (this.dataGridView.Rows.Count < 2)
            {
                return 0;
            }

            int index = 0;
            int _dataColStart = 3;//单元格初始列

            for (int i = _dataColStart; i < this.dataGridView.ColumnCount; i++)
            {
                object xValue = this.dataGridView.Rows[0].Cells[i].Value;
                if (xValue != null && xValue.ToString() != string.Empty && !double.IsNaN(double.Parse(xValue.ToString())) && Math.Round(double.Parse(xValue.ToString()), 0) == Math.Round(x, 0))
                {
                    index = i;//获取真实的对应列
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// 将单元格中的B库数据保存在字典类型中
        /// </summary>
        /// <returns>返回B库单元格中的数据</returns>
        private Dictionary<string, string> getCurrentCellBDatas()
        {
            if (this.dataGridView.Rows.Count < 2)
                return null;

            var result = new Dictionary<string, string>();
            int colIndex = 3;//起始列
            int RowIndex = 0; //起始行

            for (int i = colIndex; i < this.dataGridView.ColumnCount; i++)
            {
                object x = this.dataGridView.Rows[RowIndex].Cells[i].Value;
                object y = this.dataGridView.Rows[RowIndex + 1].Cells[i].Value;

                if (x != null && x.ToString() != string.Empty)
                    if (y != null)
                        result.Add(x.ToString(), y.ToString());
                    else
                        result.Add(x.ToString(), float.MaxValue.ToString ());
            }

            return result;
        }

        #endregion
        
        #region "窗体功能"
        /// <summary>
        /// 绘制窗体放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //this.zedGraphControl1.GraphPane.XAxis.Scale.Max = this.zedGraphControl1.GraphPane.XAxis.Scale.Max / scaleRate;
            //this.zedGraphControl1.GraphPane.YAxis.Scale.Max = this.zedGraphControl1.GraphPane.YAxis.Scale.Max / scaleRate;
            //this.zedGraphControl1.Refresh();
            this.zedGraphControl1.ZedExpand.MouseWheel(true);
        }
        /// <summary>
        /// 绘制窗体缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //this.zedGraphControl1.GraphPane.XAxis.Scale.Max = scaleRate * this.zedGraphControl1.GraphPane.XAxis.Scale.Max;
            //this.zedGraphControl1.GraphPane.YAxis.Scale.Max = scaleRate * this.zedGraphControl1.GraphPane.YAxis.Scale.Max;
            //this.zedGraphControl1.Refresh();
            this.zedGraphControl1.ZedExpand.MouseWheel(false);
        }
        /// <summary>
        /// 实窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //this.zedGraphControl1.GraphPane.XAxis.Scale.Max = axisMax;
            //this.zedGraphControl1.GraphPane.XAxis.Scale.Min = axisMin;
            //this.zedGraphControl1.GraphPane.YAxis.Scale.Max = aYisMax;
            //this.zedGraphControl1.GraphPane.YAxis.Scale.Min = aYisMin;
            this.zedGraphControl1.ZedExpand.setDefault(null);
            double addValue = this.zedGraphControl1.GraphPane.XAxis.Scale.Max * 0.2;//适窗时候添加的坐标系数
            this.zedGraphControl1.GraphPane.XAxis.Scale.Max = this.zedGraphControl1.GraphPane.XAxis.Scale.Max + addValue;
            this.zedGraphControl1.GraphPane.XAxis.Scale.Min = this.zedGraphControl1.GraphPane.XAxis.Scale.Min - addValue;
            this.zedGraphControl1.Refresh();

        }
        /// <summary>
        /// 全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //this.zedGraphControl1.GraphPane.XAxis.Scale.Max = axisMax;
            //this.zedGraphControl1.GraphPane.XAxis.Scale.Min = axisMin;
            //this.zedGraphControl1.GraphPane.YAxis.Scale.Max = aYisMax;
            //this.zedGraphControl1.GraphPane.YAxis.Scale.Min = aYisMin;
            //this.zedGraphControl1.Refresh();
            this.zedGraphControl1.ZedExpand.setDefault(null);//恢复默认
        }
        #endregion

        /// <summary>
        /// 判断数据是否改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(this.Created)
                this._isChange = true;//数据修改需要保存
        }
       
    }
}
