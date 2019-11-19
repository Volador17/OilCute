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
using RIPP.App.OilDataManager.Forms.DatabaseA.Curve;
using ZedGraph;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Interpolation.Algorithms;
using MathNet.Numerics.Interpolation;
using RIPP.Lib;

namespace RIPP.App.OilDataManager.Forms.DatabaseA
{
    public partial class NFrmCurveA : Form
    {
        #region "私有变量"
 
        /// <summary>
        /// 当前原油
        /// </summary>
        private OilInfoEntity _oil = null;     // 一条原油  
        /// <summary>
        /// 当前存放在内存中的一条B库原油
        /// </summary>
        private OilInfoBEntity _oilB = null;     // 一条原油 
        /// <summary>
        /// 当前显示的曲线类别
        /// </summary>
        private CurveTypeCode _typeCode = CurveTypeCode.YIELD;
        /// <summary>
        /// 当前显示的X轴的代码
        /// </summary>
        private CurveParmTypeEntity _curveX; //当前显示的X轴的代码
        /// <summary>
        /// 当前显示的Y轴的代码
        /// </summary>
        private CurveParmTypeEntity _curveY; //当前显示的Y轴的代码
        /// <summary>
        /// 表格中A库的X行号_ARowXIndex = 0
        /// </summary>
        private readonly int _ARowXIndex = 0;
        /// <summary>
        /// 表格中A库的Y行号_BRowYIndex = 1
        /// </summary>
        private readonly int _ARowYIndex = 1;
        /// <summary>
        /// 表格中要做曲线X行号_curveRowXIndex = 3
        /// </summary>
        private readonly int _curveRowXIndex = 3;//表格中要做曲线X行号
        /// <summary>
        /// 表格中要做曲线Y行号 _curveRowYIndex = 4
        /// </summary>
        private readonly int _curveRowYIndex = 4; //表格中要做曲线Y列的行号
        /// <summary>
        /// 表格中数据起始列 _dataColStart = 3
        /// </summary>
        private readonly int _dataColStart = 3; //数据起始列
        /// <summary>
        /// 最后一行的行行号_lastRowNum = 4;
        /// </summary>
        private readonly int _lastRowNum = 4;
        /// <summary>
        /// 收率曲线和馏分曲线B库的固定数据点个数 _dataBcol = 18
        /// </summary>
        private readonly int _dataBcol = 19; //收率曲线和馏分曲线B库的固定数据点个数
        /// <summary>
        /// 渣油曲线B库的固定数据点个数 _dataBcolResidu = 8
        /// </summary>
        private readonly int _dataBcolResidu = 8; //渣油曲线B库的固定数据点个数
        /// <summary>
        /// 馏分性质曲线和馏分曲线的固定18个点(float):15, 60, 100, 140, 160, 180, 200, 220, 240, 280, 320, 350, 400, 425, 450, 500, 540, 560 
        /// </summary>
        private readonly List<float> B_XList = new List<float>() { 15, 60,80, 100, 140, 160, 180, 200, 220, 240, 280, 320, 350, 400, 425, 450, 500, 540, 560 };//固定x轴点
        /// <summary>
        /// 渣油馏分性质曲线的8个固定点设置: 320, 350, 400, 425, 450, 500, 540, 560
        /// </summary>
        private readonly List<float> ECPList = new List<float> { 320, 350, 400, 425, 450, 500, 540, 560 };//渣油曲线固定的点
        private List<OilInfoBEntity> _oilBList = new List<OilInfoBEntity>();
        /// <summary>
        /// B库的参考原油
        /// </summary>
        public List<OilInfoBEntity> OilBList
        {
            set { this._oilBList = value; }
            get { return this._oilBList; }
        }
        /// <summary>
        /// A库的原油
        /// </summary>
        public OilInfoEntity Oil
        {
            set { this._oil = value; }
            get { return this._oil; }
        }
        /// <summary>
        /// 确定曲线调整是不是需要绘制数据
        /// </summary>
        private bool _IsDraw = true;

        /// <summary>
        /// 曲线调整时保存调整之前的临时数据,int保存列号，float保存B库的Y轴。
        /// </summary>
        private Dictionary<int, float> DatasBeforAdjust = new Dictionary<int, float>();//需要返回的字典类型
        /// <summary>
        /// //获得渣油曲线的B库X轴的值,一共8个点.
        /// </summary>
        private Dictionary<float, float> ECP_WYDic = new Dictionary<float, float>();//获得渣油曲线的B库X轴的值
        /// <summary>
        /// 放大缩小的比例系数
        /// </summary>
        //private readonly double scaleRate = 1.1; //放大缩小的比例系数
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
        /// 存储此点不参与运算的点
        /// </summary>
        private PointPairList pointListNotuse = new PointPairList ();        //存储此点不参与运算的点
        /// <summary>
        ///添加的参考原油曲线的Tag ，用于删除之前的参考原油曲线
        /// </summary>
        private string referOilTag = "referOil";//侯乐添加参考曲线的原油，方便参考曲线的清空使用
       
        /// <summary>
        /// 参数列表
        /// </summary>
        private List<CurveParmTypeEntity> _curveParmTypeList = null;
        /// <summary>
        /// 点对应的真实B库的下标,在得到点的时候赋值
        /// </summary>
        private int BcellIndex = -1;
        /// <summary>
        /// 点对应的真实A库的下标，在得到点的时候赋值
        /// </summary>
        private int AcellIndex = -1;
        
        /// <summary>
        /// 传递的数据窗体
        /// </summary>
        private DatabaseA.FrmOilDataA _frmOilDataA = null;
        /// <summary>
        /// 判断是否有可以保存的曲线
        /// </summary>
        private bool _saveDataToDataBase = false;
        /// <summary>
        /// 判断是否已经保存
        /// </summary>
        private bool _HaveSave = true;
        /// <summary>
        /// 判断数据是否需要保存
        /// </summary>
        public bool HaveSave
        {
            get
            {
                return this._HaveSave;
            }                        
            set
            {
                this._HaveSave = value;
            }
        }
        /// <summary>
        /// 当前选择的Y轴代码
        /// </summary>
        private string _currentItemCodeY = string.Empty;
        private bool _XIsChange = false;
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造空函数
        /// </summary>
        public NFrmCurveA()
        {
            InitializeComponent();
            zedGraphEvent();
        }
        #region 
        /// <summary>
        /// 构造函数
        /// </summary>  
        /// <param name="oil">一条原油</param>
        /// <param name="typeCode">曲线类型：收率，馏分，渣油</param>
        //public FrmCurveA(OilInfoEntity oil, CurveTypeCode typeCode)
        //{
        //    InitializeComponent();
        //    zedGraphEvent();
        //    _curveParmTypeAccess = new CurveParmTypeAccess();
        //    _curveParmTypeList = _curveParmTypeAccess.Get("1=1");
        //    this._oil = oil;
        //    getOilInfoBfromDataBaseB();//初始化原油B           
        //    init(typeCode);
        //    InitStyle(); //表格样式 
        //    waitingPanel = new WaitingPanel(this);
        //    this.DoubleBuffered = true;
        //}
        #endregion 

        /// <summary>
        /// 构造函数
        /// </summary>  
        /// <param name="oil">一条原油</param>
        /// <param name="typeCode">曲线类型：收率，馏分，渣油</param>
        public NFrmCurveA(DatabaseA.FrmOilDataA frmOilDataA, CurveTypeCode typeCode)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            CurveParmTypeAccess curveParmTypeAccess = new CurveParmTypeAccess();// 提取数据库中的XY参数
            this._curveParmTypeList = curveParmTypeAccess.Get("1=1");
            this._frmOilDataA = frmOilDataA;
            this._oil = frmOilDataA.getData();          
            getOilInfoBfromDataBaseB();//初始化原油B           
            init(typeCode);
            InitStyle(); //表格样式 
            zedGraphEvent();
            if (_frmOilDataA != null)
                _frmOilDataA.FormClosed += (sender, e) =>
                {
                    _frmOilDataA = null;
                };
        }
        /// <summary>
        /// zedGraph控件事件绑定
        /// </summary>
        private void zedGraphEvent()
        {           
            oilBlist.OilBList.Clear();//清除参考数据
            this.pointListNotuse = this.zedGraph1.ZedExpand.PointListNotUse;//对pointListNotUse初始化
            this.zedGraph1.ZedExpand.ResetB += new System.EventHandler(zedGraph1_ResetB);
            this.zedGraph1.ZedExpand.CopyCells += new System.EventHandler(zedGraph1_CopyCells);
            this.zedGraph1.ZedExpand.RefOil += new System.EventHandler(this.zedGraph1_RefOil);
            this.zedGraph1.ZedExpand.TotalValue += new EventHandler(toolStripButton14_Click);//累计值
            this.zedGraph1.ZedExpand.GcCal += new EventHandler(toolStripButton17_Click);//GC计算

            this.zedGraph1.ZedExpand.NotCalCulate += new EventHandler(ZedExpand_NotCalCulate);//此点不参与计算
            this.zedGraph1.ZedExpand.CalCulate += new System.EventHandler(ZedExpand_Calculate);//此点参与计算
            zedGraph1.ZedExpand.ZoomChoose = false;//打开同步锁
            zedGraph1.ZedExpand.IsMoving = true;
        }

        /// <summary>
        /// 参考原油事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraph1_RefOil(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmOpenSelectOilB"))
            {
                List<string> crudeIndexList = this.OilBList.Select(o => o.crudeIndex).ToList();
                DatabaseA.FrmOpenSelectOilB frmOpen = new DatabaseA.FrmOpenSelectOilB(this._oil.crudeIndex, crudeIndexList);
                frmOpen.Show();
            }
            if (this.Created)
                this._HaveSave = false;
        }

        #endregion

        #region "公共函数"
        /// <summary>
        /// 根据曲线类别初始化
        /// </summary>
        /// <param name="typeCode"></param>
        public void init(CurveTypeCode typeCode)
        {
            //下面的函数获取当前原油的曲线函数必须放在最前面，不能挪位置。
            DatabaseA.Curve.DataBaseACurve.getCurrentCurveFromDataGridViewDataBaseB(ref this._oilB, this.dataGridView, this._typeCode, this._curveX, this._curveY);
            
            this._typeCode = typeCode;
            this._curveX = null;
            this._curveY = null;
            this.zedGraph1.GraphPane.CurveList.Clear();//清除绘图曲线

            this.Name = this._oil.crudeIndex + "-" + "曲线";

            if (typeCode == CurveTypeCode.YIELD)
                this.Text = this._oil.crudeIndex + this._oil.crudeName + "收率曲线";
            else if (typeCode == CurveTypeCode.DISTILLATE)
                this.Text = this._oil.crudeIndex + this._oil.crudeName + "馏分性质曲线";
            else if (typeCode == CurveTypeCode.RESIDUE)
            {
                this.Text = this._oil.crudeIndex + this._oil.crudeName + "渣油性质曲线";
                ECP_WYDic = getECP_WYDatasfromYIELD();
            }
           
            initXDropList();//X、Y轴下拉菜单的物性绑定
            this.comboBox1.SelectedIndex = 0;
           
            this.cmbCurve.ComboBox.SelectedIndex = 0;
            this._curveX = (CurveParmTypeEntity)this.comboBox1.SelectedItem; //当前曲线默认为ComboBox控件中的第一条
            this._curveY = (CurveParmTypeEntity)this.cmbCurve.ComboBox.SelectedItem; //当前曲线默认为ComboBox控件中的第一条 
            this.Activate();
        }

        #region "初始化私有函数"
        /// <summary>
        /// 判断窗体是否可以打开
        /// </summary>
        /// <returns></returns>
        public bool canOpen()
        {
            bool canopen = false;
            //getCurrentCurveDataGridViewBDatas();
            DatabaseA.Curve.DataBaseACurve.getCurrentCurveFromDataGridViewDataBaseB(ref this._oilB, this.dataGridView, this._typeCode, this._curveX, this._curveY);
            
            #region "ECP-TWY判断"
            CurveEntity ECP_TWYCurve = this._oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TWY").FirstOrDefault();
            if (ECP_TWYCurve == null)
            {
                //MessageBox.Show(this._oil.crudeName + "的收率曲线不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                canopen = false;
            }
            else
            {
                if (ECP_TWYCurve.curveDatas.Count <= 0)
                {
                    //MessageBox.Show(this._oil.crudeName + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    canopen = false;
                }
                else
                {
                    canopen = true;
                }
            }
            #endregion

            return canopen;
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
            this.dataGridView.RowHeadersWidth = 30;
            this.dataGridView.MultiSelect = false;
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }

        /// <summary>
        /// 初始化X、Y轴下拉菜单
        /// </summary>
        private void initXDropList()
        {
            if (this._typeCode == CurveTypeCode.YIELD)//收率曲线
            {
                #region "收率曲线"
                List<CurveParmTypeEntity> XDropList = _curveParmTypeList.Where(o => o.IsX == 2 && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//收率曲线的X轴代码IsX=2;
                if (XDropList == null)
                    return;
                this.comboBox1.DisplayMember = "Descript";
                this.comboBox1.ValueMember = "ItemCode";
                this.comboBox1.DataSource = XDropList;

                #endregion
            }
            else if (this._typeCode == CurveTypeCode.DISTILLATE)//馏分曲线
            {
                #region "馏分曲线"

                List<CurveParmTypeEntity> DISTILLATECurveParm = _curveParmTypeList.Where(o => o.TypeCode == this._typeCode.ToString()).ToList();
                List<CurveParmTypeEntity> XDropList = DISTILLATECurveParm.Where(o => o.IsX != 0).ToList();
                if (XDropList == null)
                    return;

                CurveParmTypeEntity MCPParmEntity = XDropList.Where(o => o.ItemCode == "MCP").FirstOrDefault();
                CurveParmTypeEntity MWYParmEntity = XDropList.Where(o => o.ItemCode == "MWY").FirstOrDefault();
                XDropList.Remove(MCPParmEntity);//窄馏分和宽馏分中有相同的MCP但是计算算法不相同，所以删除窄馏分的MCP,窄馏分和宽馏分中有相同的MCP但是计算算法不相同，所以删除窄馏分的MCP
                XDropList.Remove(MWYParmEntity);//窄馏分和宽馏分中有相同的MCP但是计算算法不相同，所以删除窄馏分的MCP,窄馏分和宽馏分中有相同的MCP但是计算算法不相同，所以删除窄馏分的MCP
                
                this.comboBox1.DisplayMember = "Descript";
                this.comboBox1.ValueMember = "ItemCode";
                this.comboBox1.DataSource = XDropList;


                #endregion
            }
            else if (this._typeCode == CurveTypeCode.RESIDUE)//渣油曲线
            {
                #region "渣油曲线"

                List<CurveParmTypeEntity> RESIDUECurveParm = _curveParmTypeList.Where(o => o.TypeCode == this._typeCode.ToString()).ToList();
                List<CurveParmTypeEntity> XDropList = RESIDUECurveParm.Where(o => o.IsX != 0).ToList();

                this.comboBox1.DisplayMember = "Descript";
                this.comboBox1.ValueMember = "ItemCode";
                this.comboBox1.DataSource = XDropList;

                #endregion
            }
        }
        
        /// <summary>
        /// 初始化X、Y轴下拉菜单
        /// </summary>
        private void initYDropList()
        {
            this._curveX = (CurveParmTypeEntity)this.comboBox1.SelectedItem; //当前曲线默认为ComboBox控件中的第一条
           
            if (this._typeCode == CurveTypeCode.YIELD)
            {
                #region "收率曲线"

                List<CurveParmTypeEntity> YDropList = this._curveParmTypeList.Where(o => o.IsX == -1 && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//收率曲线的IsX == -1表示Y轴代码
                this.cmbCurve.ComboBox.DisplayMember = "Descript";
                this.cmbCurve.ComboBox.ValueMember = "ItemCode";
                this.cmbCurve.ComboBox.DataSource = YDropList;

                #endregion
            }
            else if (this._typeCode == CurveTypeCode.DISTILLATE)
            {
                #region "馏分曲线"

                List<CurveParmTypeEntity> DISTILLATECurveParm = this._curveParmTypeList.Where(o => o.TypeCode == this._typeCode.ToString()).ToList();

                if (this._curveX.ItemCode == "ECP" || this._curveX.ItemCode == "TWY")
                {            
                    List<CurveParmTypeEntity> YDropList = new List<CurveParmTypeEntity>();
                    List<CurveParmTypeEntity> WideList = DISTILLATECurveParm.Where(o => o.Show == 1 && o.OilTableTypeID == (int)EnumTableType.Wide).ToList();
                    List<CurveParmTypeEntity> NarrowList = DISTILLATECurveParm.Where(o => o.Show == 1 && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();
                    for (int wideIndex = 0; wideIndex < WideList.Count; wideIndex++)
                    {
                        CurveParmTypeEntity narrowParmEntity = NarrowList.Where(o => o.ItemCode == WideList[wideIndex].ItemCode).FirstOrDefault();
                        if (narrowParmEntity != null)
                            YDropList.Add(narrowParmEntity);
                        else
                            YDropList.Add(WideList[wideIndex]);
                        
                    }
                    
                    this.cmbCurve.ComboBox.DisplayMember = "Descript";
                    this.cmbCurve.ComboBox.ValueMember = "ItemCode";
                    this.cmbCurve.ComboBox.DataSource = YDropList;                  
                }
                else if (this._curveX.ItemCode == "TVY")
                {
                    List<CurveParmTypeEntity> YDropList = DISTILLATECurveParm.Where(o =>o.Show == 1 && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();

                    this.cmbCurve.ComboBox.DisplayMember = "Descript";
                    this.cmbCurve.ComboBox.ValueMember = "ItemCode";
                    this.cmbCurve.ComboBox.DataSource = YDropList;
                }
                #endregion
            }
            else if (this._typeCode == CurveTypeCode.RESIDUE)
            {
                #region "渣油曲线"
                List<CurveParmTypeEntity> RESIDUECurveParm = _curveParmTypeList.Where(o => o.TypeCode == this._typeCode.ToString()).ToList();
                List<CurveParmTypeEntity> YDropList = RESIDUECurveParm.Where(o => o.Show == 1).ToList();
                this.cmbCurve.ComboBox.DisplayMember = "Descript";
                this.cmbCurve.ComboBox.ValueMember = "ItemCode";
                this.cmbCurve.ComboBox.DataSource = YDropList;
                #endregion
            }           
        }

        #endregion

        #region "绘制曲线"
        /// <summary>
        /// 根据提供的曲线在绘图绘图控件上绘制数据
        /// </summary>
        public void drawCurve()
        {
            if (this._curveX != null && this._curveY != null && this.dataGridView.RowCount >= this._curveRowYIndex) //有当前曲线，并且当前曲线有数据才画曲线
            {
                List<CurveAEntity> curveAList = new List<CurveAEntity>();//曲线集合
                rowToCurve(ref curveAList, 1);
                rowToCurve(ref curveAList, this._lastRowNum);
                DrawCurve(curveAList);
                //DrawMergeCurve(curveAList);         //侯乐画出A/B合并后的曲线
                DrawReferOil();//绘制参考曲线
                this.zedGraph1.GraphPane.XAxis.Scale.MaxAuto = true;//获取添加曲线调整后的x轴的最大值
                this.zedGraph1.GraphPane.XAxis.Scale.MinAuto = true;
                this.zedGraph1.GraphPane.YAxis.Scale.MaxAuto = true;//获取添加曲线调整后的y轴的最大值
                this.zedGraph1.GraphPane.YAxis.Scale.MinAuto = true;//获取添加曲线调整后的y轴的最小值
                axisMax = this.zedGraph1.GraphPane.XAxis.Scale.Max;//获取添加曲线调整后的x轴的最大值
                axisMin = this.zedGraph1.GraphPane.XAxis.Scale.Min;//获取添加曲线调整后的x轴的最小值
                aYisMax = this.zedGraph1.GraphPane.YAxis.Scale.Max;//获取添加曲线调整后的y轴的最大值
                aYisMin = this.zedGraph1.GraphPane.YAxis.Scale.Min;//获取添加曲线调整后的y轴的最小值

                this.zedGraph1.GraphPane.AxisChange();
                this.zedGraph1.Refresh();
            }
            else
            {
                this.zedGraph1.GraphPane.CurveList.Clear();
            }
        }

        /// <summary>
        /// 根据行生成曲线，并加到曲线列表中
        /// </summary>
        /// <param name="curveAList"></param>
        /// <param name="rowYIndex"></param>
        public void rowToCurve(ref List<CurveAEntity> curveAList, int rowYIndex)
        {
            if (rowYIndex < 1)  //X轴在Y轴的前一行
                return;

            int rowXIndex = rowYIndex - 1; //rowXIndex要画曲线的行，rowYIndex要画曲线的列

            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue || o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            OilTableRowEntity rowX = tempRowList.Where(o => o.itemCode == this._curveX.ItemCode && o.oilTableTypeID == this._curveX.OilTableTypeID).FirstOrDefault();
            OilTableRowEntity rowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveY.OilTableTypeID).FirstOrDefault();


            if (rowY == null || rowX == null)
                return;

            CurveAEntity curver = new CurveAEntity();
            curver.propertyX = rowX.itemCode;
            curver.propertyY = rowY.itemCode;
            curver.Color = (rowYIndex == this._lastRowNum ? Color.Red : Color.Silver);
            curver.isRefence = (rowYIndex == this._lastRowNum ? false : true);
            curver.curveTypeID = 1;
            curver.decNumber = rowY.decNumber == null ? rowY.valDigital : rowY.decNumber.Value;
            if (rowYIndex >= 2)
                curver.descript = "应用库";
            else
                curver.descript = "原始库";
            curver.unit = rowY.itemUnit;

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
            {
                curveAList.Add(curver);
            }
        }

        /// <summary>
        /// 根据传入的曲线数据集合绘制曲线
        /// </summary>
        /// <param name="curveAList">根据传入的曲线数据集合绘制曲线</param>
        public void DrawCurve(List<CurveAEntity> curveAList)
        {
            this.zedGraph1.GraphPane.CurveList.Clear();
            if (curveAList == null)
                return;

            foreach (CurveAEntity curve in curveAList)
            {
                if (curve.X == null)
                    return;
                //this.zedGraph1.GraphPane.AddCurve(this._curve.descript, this._curve.X.ToArray(), this._curve.Y.ToArray(), this._curve.Color, SymbolType.None);
                if (curve.isRefence == false)  //存入当前要编辑曲线的点
                {
                    PointPairList list = zedGraph1.ZedExpand.PointList;
                    list.Clear();
                    for (int i = 0; i < curve.X.Count(); i++)
                    {
                        list.Add(curve.X[i], curve.Y[i]);
                    }
                    LineItem CurveLine = this.zedGraph1.GraphPane.AddCurve(curve.descript, list, curve.Color, SymbolType.Square);
                    //CurveLine.Tag = curve.descript;
                    CurveLine.Line.IsSmooth = true;
                    CurveLine.Line.SmoothTension = 0.5F;//设置曲线的平滑度
                    CurveLine.Line.IsOptimizedDraw = true;
                    //CurveLine.Line.IsVisible = false;//设置B库曲线不可见
                }
                else
                {
                    PointPairList AList = this.zedGraph1.ZedExpand.ApointList;
                    AList.Clear();
                    for (int i = 0; i < curve.X.Count(); i++)
                    {
                        AList.Add(curve.X[i], curve.Y[i]);
                    }
                    LineItem myLine = this.zedGraph1.GraphPane.AddCurve(curve.descript, curve.X.ToArray(), curve.Y.ToArray(), Color.Black, SymbolType.Circle);
                    myLine.Tag = curve.descript;
                    myLine.Line.IsVisible = false;//设置曲线为透明，不显示曲线，仅仅显示点

                    //画A库中此点不参与运算的X号标记
                    if (pointListNotuse.Count > 0)
                    {
                        for (int i = 0; i < pointListNotuse.Count; i++)
                        {
                            this.zedGraph1.ZedExpand.DrawPointNotUse(pointListNotuse[i].X, pointListNotuse[i].Y, zedGraph1);
                        }
                    }
                }

                this.setTitle(curve);
                this.zedGraph1.AxisChange();
                //this.zedGraph1.Refresh();
            }
        }
        #endregion 

        #endregion

        #region "控件函数"

        /// <summary>
        /// 曲线的X轴切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBaseACurve.refreshDGVColWidthafterCurveAdjust(ref dataGridView, DatasBeforAdjust);
            DatabaseA.Curve.DataBaseACurve.getCurrentCurveFromDataGridViewDataBaseB(ref this._oilB, this.dataGridView, this._typeCode, this._curveX, this._curveY);
            this._currentItemCodeY = this._curveY != null ? this._curveY.ItemCode : string.Empty;
            this._XIsChange = true;
            initYDropList();            
        }
        
        /// <summary>
        /// 曲线的Y轴切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCurve_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataBaseACurve.refreshDGVColWidthafterCurveAdjust(ref dataGridView ,DatasBeforAdjust);
            DatabaseA.Curve.DataBaseACurve.getCurrentCurveFromDataGridViewDataBaseB(ref this._oilB, this.dataGridView, this._typeCode, this._curveX, this._curveY);          
            XYSelectedIndexChanged();           
        }
        /// <summary>
        /// XY轴物性改变事件
        /// </summary>
        private void XYSelectedIndexChanged()
        {
            //清空空间计算值
            this.toolStripTextBox1.Text = string.Empty;
            this.toolStripTextBox2.Text = string.Empty;
            this.toolStripTextBox3.Text = string.Empty;
            //this.zedGraph1.GraphPane.XAxis.Scale.MaxAuto = true;
            
            this.toolStripButton1.Text = "扣除宽馏分";
            this.toolStripButton2.Text = "扣除窄馏分";
           
            this.pointListNotuse.Clear();
            if (this._XIsChange)
            {
                foreach (var item in this.cmbCurve.ComboBox.Items)
                {
                    CurveParmTypeEntity currentItem = (CurveParmTypeEntity)item;
                    if (currentItem.ItemCode == _currentItemCodeY && _currentItemCodeY != "")
                    {
                        this.cmbCurve.ComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            this._XIsChange = false;

            this._curveX = (CurveParmTypeEntity)this.comboBox1.SelectedItem; //当前曲线默认为ComboBox控件中的第一条
            this._curveY = (CurveParmTypeEntity)this.cmbCurve.ComboBox.SelectedItem; //当前曲线默认为ComboBox控件中的第一条
           
            CurveEntity currentCurve = this._oilB.curves.Where(o => o.propertyX == this._curveX.ItemCode && o.propertyY == this._curveY.ItemCode).FirstOrDefault();
           
            if (this._typeCode == CurveTypeCode.YIELD)
            {
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveY.OilTableTypeID).FirstOrDefault();
                this.zedGraph1.ZedExpand.DecNumber =  tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;
              
                #region "性质曲线"
                if (currentCurve != null && currentCurve.curveDatas.Count > 0)
                {
                    setYIELD();
                    addValuetoLastRowDataGridView(currentCurve.curveDatas);
                }
                else//不存在当前曲线
                {
                    setYIELD();

                    FrmCurveAFunction.frmCurveInterpolation(dataGridView, this._typeCode, this._curveX, this._curveY);
                    FrmCurveAFunction.frmCurveAutoEpitaxial(dataGridView, this._typeCode, this._curveX, this._curveY);
                }
                #endregion
            }
            else if (this._typeCode == CurveTypeCode.DISTILLATE)
            {
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveY.OilTableTypeID).FirstOrDefault();
                this.zedGraph1.ZedExpand.DecNumber = tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;
 
                #region "馏分曲线"
                if (currentCurve != null && currentCurve.curveDatas.Count > 0)
                {
                    setDISTILLATE();
                    addValuetoLastRowDataGridView(currentCurve.curveDatas);
                }
                else
                {
                    setDISTILLATE();

                    FrmCurveAFunction.frmCurveInterpolation(dataGridView, this._typeCode, this._curveX, this._curveY);
                    FrmCurveAFunction.frmCurveAutoEpitaxial(dataGridView, this._typeCode, this._curveX, this._curveY);
                }
                #endregion
            }
            else if (this._typeCode == CurveTypeCode.RESIDUE)
            {
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();              
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveY.OilTableTypeID).FirstOrDefault();
                this.zedGraph1.ZedExpand.DecNumber = tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;
 
                #region "渣油曲线"
                if (currentCurve != null && currentCurve.curveDatas.Count > 0)//存在曲线值
                {                 
                    #region "渣油曲线"
                    if (this._curveX.ItemCode == PartCurveItemCode.WY.ToString())//WY保存
                    {
                        this.toolStripButton11.Enabled = true;
                        this.toolStripButton12.Enabled = true;
                        this.toolStripButton13.Enabled = true;
                        this.toolStripButton15.Enabled = true;

                        this.toolStripButton1.Enabled = true;
                        this.toolStripButton2.Enabled = true;
                        CurveEntity ECP_WYCurve = this._oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY =="WY").FirstOrDefault();
          
                        //获得渣油表的A库的值
                        Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> residueA_XYDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Residue, Color.Red, Color.Blue);
                        _SetResidueCol(residueA_XYDatas.Count, true);//设置渣油曲线列头      
                        _setResidueRow();
                        _addResidueValueBeforeTowRowToDataGridView(residueA_XYDatas);//A库赋值
                        _addResidueECPToDataGridView();//添加ECP行
                        _addResidueValueToDataGridViewCurveXRow(ECP_WYDic ); //向B库赋值
                        addValuetoResidueLastRowDataGridView(currentCurve.curveDatas);
                    }
                    else if (this._curveX.ItemCode == PartCurveItemCode.VY.ToString())//TWY显示
                    {
                        this.toolStripButton11.Enabled = false;
                        this.toolStripButton12.Enabled = false;
                        this.toolStripButton13.Enabled = false;
                        this.toolStripButton15.Enabled = false;

                        this.toolStripButton1.Enabled = false;
                        this.toolStripButton2.Enabled = false;

                        Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> residueA_XYDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Residue, Color.Red, Color.Blue);
                        _SetResidueCol(residueA_XYDatas.Count, false);//设置渣油曲线列头  
                        _setResidueRow();
                        _addResidueValueBeforeTowRowToDataGridView(residueA_XYDatas);//
                        _addResidueECPToDataGridView();
                    }
                    #endregion
                }
                else//渣油曲线存在，需要重新计算
                {
                    setRESIDUE();
                    FrmCurveAFunction.frmCurveInterpolation(dataGridView, this._typeCode, this._curveX, this._curveY);
                    FrmCurveAFunction.frmCurveAutoEpitaxial(dataGridView, this._typeCode, this._curveX, this._curveY);
                }

                #endregion
            }
            drawCurve();
        }
        /// <summary>
        /// 设置是否启用按钮的
        /// </summary>
        /// <param name="enable"></param>
        private void setButtonEnable(bool enable)
        {
            this.toolStripButton11.Enabled = enable;
            this.toolStripButton12.Enabled = enable;
            this.toolStripButton13.Enabled = enable;
            this.toolStripButton15.Enabled = enable;
            this.toolStripButton1.Enabled = enable;
            this.toolStripButton2.Enabled = enable;     
        }


        /// <summary>
        /// 设置收率曲线
        /// </summary>
        private void setYIELD( )
        {
            setButtonEnable(true);
            string wideYItemCode = string.Empty;
            if (this._curveY.ItemCode == "TWY")
            {
                wideYItemCode = "MWY";
                #region "数据处理"
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrowDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Narrow, Color.Red, Color.Blue);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> wideDatas = getDictionary(PartCurveItemCode.MCP.ToString(), wideYItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeNarrWideDatas = MergeCurveParm(narrowDatas, wideDatas);

                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> ECP_TWYorTVYDictionary = WTorVLightOilDatas(this._oil.ICP0, Color.Green);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeLightNarrWide = MergeCurveParm(mergeNarrWideDatas, ECP_TWYorTVYDictionary);
                #endregion 

                bool HaveB_X = true;
                _SetCol(mergeLightNarrWide.Count, HaveB_X);
                _setRow();
                DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, mergeLightNarrWide);
                DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._curveRowXIndex);
            }
            if (this._curveY.ItemCode == "TVY")
            {
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrowDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Narrow, Color.Red, Color.Blue);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> ECP_TWYorTVYDictionary = WTorVLightOilDatas(this._oil.ICP0, Color.Green);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeLightNarrWide = MergeCurveParm(narrowDatas, ECP_TWYorTVYDictionary);
                bool HaveB_X = true;
                _SetCol(mergeLightNarrWide.Count, HaveB_X);
                _setRow();
                DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, mergeLightNarrWide);
                DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._curveRowXIndex);
            }

        }
        /// <summary>
        /// 设置馏分曲线
        /// </summary>
        private void setDISTILLATE( )
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrowDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();//窄馏分表中对应物性的数据
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> wideDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();//宽馏分表中对应物性的数据
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeNarrWideDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();//宽馏分和窄馏分表中对应物性的合并数据

            bool HaveB_X = false;//用于判断B库的X轴是否填写数据          
            #region "窄馏分表的数据提取和并"
            string wideXItemCode = string.Empty;

            if (this._curveX.ItemCode == PartCurveItemCode.ECP.ToString())
            {
                setButtonEnable(true);

                HaveB_X = true;
                wideXItemCode = PartCurveItemCode.MCP.ToString();
                if (this._curveY.OilTableTypeID == (int)EnumTableType.Narrow)
                {
                    narrowDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Narrow, Color.Red, Color.Blue);
                    wideDatas = getDictionary(wideXItemCode, this._curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);
                    mergeNarrWideDatas = MergeCurveParm(narrowDatas, wideDatas);
                    _SetCol(mergeNarrWideDatas.Count, HaveB_X);
                    _setRow();
                    DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, mergeNarrWideDatas);
                    DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._curveRowXIndex);
                }
                else if (this._curveY.OilTableTypeID == (int)EnumTableType.Wide)
                {
                    wideDatas = getDictionary(wideXItemCode, this._curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);
                    _SetCol(wideDatas.Count, HaveB_X);
                    _setRow();
                    DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, wideDatas);
                    DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._curveRowXIndex);
                }
            }
            else if (this._curveX.ItemCode == PartCurveItemCode.TWY.ToString())
            {
                setButtonEnable(false);
                
                wideXItemCode = PartCurveItemCode.MWY.ToString();
                HaveB_X = true;
                if (this._curveY.OilTableTypeID == (int)EnumTableType.Narrow)
                {
                    narrowDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Narrow, Color.Red, Color.Blue);
                    wideDatas = getDictionary(wideXItemCode, this._curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);
                    mergeNarrWideDatas = MergeCurveParm(narrowDatas, wideDatas);

                    _SetCol(mergeNarrWideDatas.Count, HaveB_X);
                    _setRow();
                    DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, mergeNarrWideDatas);
                    DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._ECPRowIndex);
                    //_addValueToDataGridViewLastTwoRow();
                }
                else if (this._curveY.OilTableTypeID == (int)EnumTableType.Wide)
                {
                    wideDatas = getDictionary(wideXItemCode, this._curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);
                    _SetCol(wideDatas.Count, HaveB_X);
                    _setRow();
                    DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, wideDatas);
                    DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._ECPRowIndex);
                }
            }
            else if (this._curveX.ItemCode == PartCurveItemCode.TVY.ToString())
            {
                setButtonEnable(false);
                HaveB_X = true;
                narrowDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Narrow, Color.Red, Color.Blue);
                _SetCol(narrowDatas.Count, HaveB_X);
                _setRow();
                DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, narrowDatas);
                DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._ECPRowIndex);
                //_addValueToDataGridViewLastTwoRow();
            }
            #endregion
            #region
            //if (this._curveX.OilTableTypeID == (int)EnumTableType.Narrow)
            //{
            //}
            //else if (this._curveX.OilTableTypeID == (int)EnumTableType.Wide)
            //{
            //    this.toolStripButton11.Enabled = false;
            //    this.toolStripButton12.Enabled = false;
            //    this.toolStripButton13.Enabled = false;
            //    this.toolStripButton14.Enabled = false;
            //    this.toolStripButton15.Enabled = false;
            //    #region "窄馏分表的数据提取和并"
            //    wideDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);
            //    _SetCol(wideDatas.Count, HaveB_X);
            //    _setRow();
            //    _addBeforeTowRowToDataGridView(wideDatas);
            //    _addECPToDataGridView(2);
            //    _addValueToDataGridViewLastTwoRow();
            //    #endregion
            //}
            #endregion
        }
        /// <summary>
        /// 设置渣油曲线
        /// </summary>
        private void setRESIDUE( )
        {
            #region "渣油曲线"
            if (this._curveX.ItemCode == PartCurveItemCode.WY.ToString())//WY保存
            {
                setButtonEnable(true);
                //获得渣油表的A库的值
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> residueA_XYDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Residue, Color.Red, Color.Blue);
                _SetResidueCol(residueA_XYDatas.Count, true);//设置渣油曲线列头      
                _setResidueRow();
                _addResidueValueBeforeTowRowToDataGridView(residueA_XYDatas);//
                _addResidueECPToDataGridView();
                // Dictionary<float, float> ECP_WYDic = getECP_WYDatasfromYIELD();//获得渣油曲线的B库X轴的值
                _addResidueValueToDataGridViewCurveXRow(ECP_WYDic ); //向B库赋值
            }
            else if (this._curveX.ItemCode == PartCurveItemCode.VY.ToString())//TVY显示
            {
                setButtonEnable(false);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> residueA_XYDatas = getDictionary(this._curveX.ItemCode, this._curveY.ItemCode, (int)EnumTableType.Residue, Color.Red, Color.Blue);
                _SetResidueCol(residueA_XYDatas.Count, true);//设置渣油曲线列头
                _setResidueRow();
                _addResidueValueBeforeTowRowToDataGridView(residueA_XYDatas);//
                _addResidueECPToDataGridView();
                //_addValueToDataGridViewLastTwoRow();
            }
            #endregion
        }

        #region 渣油曲线行和列设置
        /// <summary>
        /// 设置表头
        /// </summary>
        /// <param name="mergeResult">输入数据</param>
        /// <param name="HaveB_X">判断X轴是否显示数据</param>
        private void _SetResidueCol(int dataColNum, bool HaveB_X)
        {
            if (dataColNum < 0)
                return;

            this.dataGridView.Columns.Clear();

            #region 固定的列

            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "项目",
                ReadOnly = true,
                Name = "itemName",
                Tag = "true",
                Width = 120,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Frozen = true
            });

            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "单位",
                ReadOnly = true,
                Name = "itemUnit",
                Tag = "true",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Frozen = true
            });

            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "库类别",
                ReadOnly = true,
                Name = "itemCode",
                Tag = "true",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Frozen = true
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "原油性质",
                ReadOnly = true,
                Name = "whole",
                Width = 100,
                Tag = "true",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Frozen = true
            });
            #endregion

            int COLNUM = this._dataBcolResidu;//根据曲线类型判断数据的列         

            if (dataColNum <= COLNUM)
            {
                #region "根据数据显示列"
                int i = 1;
                if (HaveB_X == true)
                {
                    for (int k = 0; k < COLNUM; k++)
                    {
                        GridOilCol col = new GridOilCol()
                        {
                            HeaderText = "Cut" + (k + i).ToString(),
                            //Name = "校正",
                            Width = 70,
                            Tag = "true",
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                            SortMode = DataGridViewColumnSortMode.NotSortable
                        };
                        this.dataGridView.Columns.Add(col);
                    }
                }
                else
                {
                    for (int k = 0; k < dataColNum; k++)
                    {
                        GridOilCol col = new GridOilCol()
                        {
                            HeaderText = "Cut" + (k + i).ToString(),
                            //Name = "校正",
                            Width = 70,
                            Tag = "true",
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                            SortMode = DataGridViewColumnSortMode.NotSortable
                        };
                        this.dataGridView.Columns.Add(col);
                    }
                }
                #endregion
            }
            else
            {
                #region "根据数据显示列"
                int i = 1;

                for (int k = 0; k < dataColNum; k++)
                {
                    GridOilCol col = new GridOilCol()
                    {
                        HeaderText = "Cut" + (k + i).ToString(),
                        //Name = "校正",
                        Width = 70,
                        Tag = "true",
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    this.dataGridView.Columns.Add(col);
                }

                #endregion
            }
        }
        /// <summary>
        /// 设置渣油表的行
        /// </summary>
        private void _setResidueRow()
        {
            DatabaseA.Curve.DataBaseACurve._setResidueRow(this.dataGridView, this._curveX.ItemCode, this._curveY.ItemCode);       
        }
       
        /// <summary>
        /// 设置渣油表的行和数据
        /// </summary>
        /// <param name="mergeResult"></param>
        private void _addResidueValueBeforeTowRowToDataGridView(Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeResult)
        {
            if (mergeResult != null && mergeResult.Count > 0)
            {
                ZedGraphOilDataEntity whole = mergeResult.Keys.Where(o => o.ColumnIndex == this._dataColStart && o.D_CalShowData == 100).FirstOrDefault();
                if (whole != null)
                {
                    #region "设置第一行"
                    this.dataGridView.Rows[this._ARowXIndex].Cells[this._dataColStart] = new GridOilCell()
                    {
                        CellValue = whole,
                        Value = whole.D_CalShowData
                    };
                    this.dataGridView.Rows[this._ARowXIndex].Cells[this._dataColStart].Style.ForeColor = whole.Cell_Color; //设置字体前景色
                    #endregion

                    #region "设置第二行"
                    this.dataGridView.Rows[this._ARowYIndex].Cells[this._dataColStart] = new GridOilCell()
                    {
                        CellValue = mergeResult[whole],
                        Value = mergeResult[whole].D_CalShowData
                    };
                    this.dataGridView.Rows[this._ARowYIndex].Cells[this._dataColStart].Style.ForeColor = mergeResult[whole].Cell_Color; //设置字体前景色
                    #endregion
                    mergeResult.Remove(whole);//删除原油性质的值
                }

                int colNum = this._dataColStart + 1;
                foreach (ZedGraphOilDataEntity x in mergeResult.Keys)
                {
                    #region "设置第一行"
                    this.dataGridView.Rows[this._ARowXIndex].Cells[colNum] = new GridOilCell()
                    {
                        CellValue = x,
                        Value = x.D_CalShowData
                    };
                    this.dataGridView.Rows[this._ARowXIndex].Cells[colNum].Style.ForeColor = x.Cell_Color; //设置字体前景色
                    #endregion

                    #region "设置第二行"
                    this.dataGridView.Rows[this._ARowYIndex].Cells[colNum] = new GridOilCell()
                    {
                        CellValue = mergeResult[x],
                        Value = mergeResult[x].D_CalShowData
                    };
                    this.dataGridView.Rows[this._ARowYIndex].Cells[colNum].Style.ForeColor = mergeResult[x].Cell_Color; //设置字体前景色
                    #endregion

                    colNum++;
                }

            }
        }

        /// <summary>
        /// 渣油曲线最后两行的添加
        /// </summary>
        private void _addResidueECPToDataGridView()
        {
            if (this.dataGridView.Rows.Count < 2)
                return;

            int colIndex = this._dataColStart + 1;

            foreach (double ECP in this.ECPList)
            {
                this.dataGridView.Rows[this._ARowYIndex + 1].Cells[colIndex] = new GridOilCell()
                {
                    Value = ECP
                };
                colIndex++;
            }
        }     
        /// <summary>
        /// 获取曲线下标所对应的真实单元格下标的位置
        /// </summary>
        /// <param name="itemcodeValue">查找的值</param>
        /// <param name="rowNum">查找的值对应的行数</param>
        /// <returns>查找不_到返回-1</returns>
        private int FindItemCodeValueColIndexfromSpecRow(float itemcodeValue, int rowNum)
        {
            int index = -1;
            if (rowNum > this.dataGridView.RowCount || rowNum < 0)
                return index;

            for (int i = _dataColStart; i < this.dataGridView.ColumnCount; i++)
            {
                object xValue = this.dataGridView.Rows[rowNum].Cells[i].Value;
                if (xValue != null && xValue.ToString() != string.Empty && !float.IsNaN(float.Parse(xValue.ToString())) && float.Parse(xValue.ToString()) == itemcodeValue)
                {
                    index = i;//获取真实的对应列
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// 渣油曲线最后两行的添加
        /// </summary>
         private void _addResidueValueToDataGridViewCurveXRow(Dictionary<float, float> ECP_WY)
        {
            int colIndex = this._dataColStart;

            this.dataGridView.Rows[this._curveRowXIndex].Cells[colIndex].Value = this.dataGridView.Rows[this._ARowXIndex].Cells[colIndex].Value;//原油性性质的赋值
            this.dataGridView.Rows[this._curveRowYIndex].Cells[colIndex].Value = this.dataGridView.Rows[this._ARowYIndex].Cells[colIndex].Value;//原油性性质的赋值

            foreach (var ECP_key in ECP_WY.Keys)
            {
                int WYCol = FindItemCodeValueColIndexfromSpecRow(ECP_key, 2);
                if (WYCol != -1 && !ECP_WY[ECP_key].Equals(float.NaN))
                {
                    this.dataGridView.Rows[this._curveRowXIndex].Cells[WYCol].Value = ECP_WY[ECP_key];
                }
            }
        }
        /// <summary>
        /// 获取收率曲线中各个ECP对应的TWY的值
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        private Dictionary<float, float> getECP_TWYDatasfromYIELD()//ECP-TWY
        {
            Dictionary<float, float> ECP_TWYDIC = new Dictionary<float, float>();//存储终切点数据。  

            #region "ECP-TWY判断"
            CurveEntity ECP_TWYCurve = this._oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TWY").FirstOrDefault();
            if (ECP_TWYCurve == null)
            {
                //MessageBox.Show(this._oilB.crudeName + "的收率曲线不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ECP_TWYDIC;
            }
            if (ECP_TWYCurve.curveDatas.Count <= 0)
            {
                //MessageBox.Show(this._oilB.crudeName + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ECP_TWYDIC;
            }
            #endregion

            var ECP_TWYDatas = ECP_TWYCurve.curveDatas.OrderBy(o => o.xValue);//升序
            List<CurveDataEntity> ECP_TWYCurveDatas = ECP_TWYDatas.ToList();

            #region "存储终切点数据"

            for (int index = 0; index < ECP_TWYCurveDatas.Count; index++)
            {
                if (!ECP_TWYDIC.Keys.Contains(ECP_TWYCurveDatas[index].xValue))
                {
                    ECP_TWYDIC.Add(ECP_TWYCurveDatas[index].xValue, ECP_TWYCurveDatas[index].yValue);//ECP-TWY
                }
            }
            #endregion

            return ECP_TWYDIC;
        }

        /// <summary>
        /// 获取收率曲线中各个ECP对应的WY的值
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        private Dictionary<float, float> getECP_WYDatasfromYIELD()
        {
            Dictionary<float, float> ECP_WYDIC = new Dictionary<float, float>();

            Dictionary<float, float> ECP_TWYDIC = getECP_TWYDatasfromYIELD();//存储终切点数据。           
            if (ECP_TWYDIC.Count <= 0)
                return ECP_WYDIC;

            List<float> X_ECP = ECP_TWYDIC.Keys.ToList();//ECP
            List<float> Y_TWY = ECP_TWYDIC.Values.ToList();//TWY

            List<float> TWYList = SplineLineInterpolate.spline(X_ECP, Y_TWY, this.ECPList);//渣油表的八个点的切割
            if (TWYList.Count == 0)
                return ECP_WYDIC;

            for (int ECPIndex = 0; ECPIndex < this.ECPList.Count; ECPIndex++)
            {
                if (!TWYList[ECPIndex].Equals(double.NaN))
                {
                    float WY = 100 - TWYList[ECPIndex];
                    ECP_WYDIC.Add(ECPList[ECPIndex], WY);
                }
            }

            return ECP_WYDIC;
        }

        #endregion

        /// <summary>
        /// 设置绘图的样式
        /// </summary>
        private void setTitle(CurveAEntity curve)
        {
            if (this._curveX == null)
                return;
            GraphPane myPane = this.zedGraph1.GraphPane;

            this.zedGraph1.IsEnableVEdit = curve.isRefence ? false : true;
            //myPane.Legend.IsVisible = false;
            //字体
            myPane.Border.Width = 0;
            myPane.Border.Color = Color.White;
            myPane.Title.FontSpec.IsBold = false;
            myPane.XAxis.Title.FontSpec.IsBold = false;//设置x轴的文字粗体
            myPane.YAxis.Title.FontSpec.IsBold = false;

            myPane.XAxis.Scale.MaxGrace = 0;
            myPane.XAxis.Scale.MinGrace = 0;

            myPane.Title.Text = this._curveX.Descript + "-" + this._curveY.Descript + "曲线";
            myPane.YAxis.Title.IsVisible = false;
            myPane.XAxis.Title.IsVisible = false;
        }
        /// <summary>
        /// Y轴移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraph1_MouseMove(object sender, MouseEventArgs e)
        {
            #region
            //if (this.dataGridView.Rows.Count < 2)
            //    return;

            //int Aindex = -1;           //A库点在单元格中的下标
            //double xValue;            //存储当前坐标的X值
            //if (double.TryParse(this.zedGraph1.ZedExpand.Xstr, out xValue))
            //{
            //    #region "处理A库点选中的数据联动"
            //    Aindex = FindA_XColIndex(xValue);
            //    if (Aindex > 0)
            //        this.dataGridView.Rows[this._ARowYIndex].Cells[Aindex].Selected = true;      //单元格选中事件
            //    #endregion

            //    #region "处理B库点选中以及曲线拖动"
            //    int index = this.zedGraph1.ZedExpand.Index;
            //    //this.zedGraph1.ZedExpand.Index = 0;         //将当前的Index清零
            //    double x = 0;
            //    double y = 0;

            //    if (this.zedGraph1.ZedExpand.PointList.Count == 0)
            //        return;

            //    if (index < this.zedGraph1.ZedExpand.PointList.Count)
            //    {
            //        x = this.zedGraph1.ZedExpand.PointList[index].X;
            //        y = this.zedGraph1.ZedExpand.PointList[index].Y;
            //    }
            //    if (index > 0)
            //    {
            //        index = FindB_XColIndex(x);//获取B库曲线中的点在单元格中的列号
            //    }

            //    if (index > 0)
            //    {
            //        this.dataGridView.Rows[this._lastRowNum].Cells[index].Selected = true;      //鼠标选中事件
            //    }

            //    GridOilRow row = this.dataGridView.Rows[this._lastRowNum] as GridOilRow;
            //    if (row.RowEntity == null)
            //        return;
            //    if (index > 0)
            //        row.Cells[index].Value = y;
            //    #endregion
            //    this.zedGraph1.Refresh();
            //}
            #endregion

            if (this.dataGridView.Rows.Count < 2)
                return;

            int Aindex = -1;           //A库点在单元格中的下标
            double xValue;            //存储当前坐标的X值
            if (double.TryParse(this.zedGraph1.ZedExpand.Xstr, out xValue))
            {
                #region "处理A库点选中的数据联动"
                Aindex = FindA_XColIndex(xValue);
                if (Aindex > 0)
                    AcellIndex = Aindex;
                //this.dataGridView.Rows[this._ARowYIndex].Cells[Aindex].Selected = true;      //单元格选中事件
                #endregion

                #region "处理B库点选中以及曲线拖动"
                int index = this.zedGraph1.ZedExpand.Index;
                double x = 0;
                double y = 0;

                if (this.zedGraph1.ZedExpand.PointList.Count == 0)
                    return;

                if (index < this.zedGraph1.ZedExpand.PointList.Count)
                {
                    x = this.zedGraph1.ZedExpand.PointList[index].X;
                    y = this.zedGraph1.ZedExpand.PointList[index].Y;
                }
                if (index > 0)
                {
                    index = FindB_XColIndex(x);//获取B库曲线中的点在单元格中的列号
                    BcellIndex = index;//将点下标赋值给Bindex
                }

                GridOilRow row = this.dataGridView.Rows[this._lastRowNum] as GridOilRow;
                if (row.RowEntity == null)
                    return;
                if (index > 0)
                    row.Cells[index].Value = y;
                #endregion
                this.zedGraph1.Refresh();
            }
        }
        /// <summary>
        /// 鼠标刷新图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraph1_MouseRefresh(object sender, EventArgs e)
        {
            drawCurve();
        }
        /// 复制表格事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraph1_CopyCells(object sender, EventArgs e)
        {            
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.MultiSelect = true;
            if (this.dataGridView.Rows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView.Rows.Count; i++)
                {
                    this.dataGridView.Rows[i].Selected = true;
                }
            }

            DataGridViewSelectedCellCollection temp = this.dataGridView.SelectedCells;

            DataObject dataObj = this.dataGridView.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);

            if (this.dataGridView.Rows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView.Rows.Count; i++)
                {
                    this.dataGridView.Rows[i].Selected = false;
                }
            }
            this.dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView.MultiSelect = false;
            MessageBox.Show("表格已经复制","提示信息",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        /// <summary>
        /// 获取X值所对应B库的真实单元格下标的位置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <returns>查找不到返回-1</returns>
        private int FindB_XColIndex(double x)
        {
            int index = -1;
            if (this.dataGridView.Rows.Count < 4)
                return index;

            for (int i = _dataColStart; i < this.dataGridView.ColumnCount; i++)
            {
                object xValue = this.dataGridView.Rows[this._lastRowNum - 1].Cells[i].Value;             
                if (xValue != null && xValue.ToString() != string.Empty && !double.IsNaN(double.Parse(xValue.ToString())) && double.Parse(xValue.ToString()) == x)
                {
                    index = i;//获取真实的对应列
                    break;
                }
            }
            return index;
        }
        /// <summary>
        /// 获取X值所对应A库的真实单元格的下标的位置
        /// </summary>
        /// <param name="x"></param>
        /// <returns>查找不到返回-1</returns>
        private int FindA_XColIndex(double xValue)
        {
            int index = -1;
            if (this.dataGridView.Rows.Count < 2)
                return index;
            for (int i = _dataColStart; i < this.dataGridView.ColumnCount; i++)
            {
                object xCellValue = this.dataGridView.Rows[this._ARowXIndex].Cells[i].Value;

                double  tempXCellValue = 0 ;
                if (!xValue.Equals(double.NaN)
                    && xCellValue != null && xCellValue.ToString() != string.Empty && double.TryParse(xCellValue.ToString(), out tempXCellValue))                   
                {
                    if (Math.Abs(xValue - tempXCellValue) < 1)
                    {
                        index = i;//获取真实的对应列
                        break;                  
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// 获取X值所对应A库的真实单元格的下标的位置
        /// </summary>
        /// <param name="x"></param>
        /// <returns>查找不到返回-1</returns>
        private int FindA_XColIndex(double xValue , double yValue)
        {
            int index = -1;
            if (this.dataGridView.Rows.Count < 2)
                return index;

            if (xValue.Equals(double.NaN) || yValue.Equals(double.NaN))
                return index;

            for (int i = _dataColStart; i < this.dataGridView.ColumnCount; i++)
            {
                object xCellValue = this.dataGridView.Rows[this._ARowXIndex].Cells[i].Value;
                object yCellValue = this.dataGridView.Rows[this._ARowYIndex].Cells[i].Value;
                double tempXValue = 0, tempYValue = 0;
                if (xCellValue != null && xCellValue.ToString() != string.Empty && double.TryParse(xCellValue.ToString(), out tempXValue) && yCellValue != null && yCellValue.ToString() != string.Empty && double.TryParse(yCellValue.ToString(), out tempYValue))
                {
                    if (Math.Abs(xValue - tempXValue) < 0.001 && Math.Abs(yValue - tempYValue) < 0.001)
                    {
                        index = i;//获取真实的对应列
                        break;

                    }
                }
            }
            return index;
        }
        /// <summary>
        /// B库清空事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraph1_ResetB(object sender, EventArgs e)
        {
            refreshCurve();
        }
        /// <summary>
        /// 合并窄馏分和宽馏分中相同的点的对应对象
        /// </summary>
        /// <param name="narrow"></param>
        /// <param name="wide"></param>
        /// <returns></returns>
        private Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> MergeCurveParm(Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrow, Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> wide)
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> result = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();
            if (narrow == null)
            {
                if (wide != null && wide.Count > 0)
                {
                    foreach (var wideKey in wide.Keys)
                    {
                        var HaveSameKey = from key in result.Keys
                                          where key.D_CalShowData == wideKey.D_CalShowData
                                          select key.D_CalShowData;

                        if (HaveSameKey.Count() == 0)
                        {
                            result.Add(wideKey, wide[wideKey]);
                        }
                    }
                }
                return result;
            }

            if (wide == null)
            {
                if (narrow != null && narrow.Count > 0)
                {
                    foreach (var narrowKey in narrow.Keys)
                    {
                        var HaveSameKey = from key in result.Keys
                                          where key.D_CalShowData == narrowKey.D_CalShowData
                                          select key.D_CalShowData;

                        if (HaveSameKey.Count() == 0)
                        {
                            result.Add(narrowKey, narrow[narrowKey]);
                        }
                    }
                }
                return result;
            }


            foreach (var narrowKey in narrow.Keys)
            {
                var HaveSameKey = from key in result.Keys
                                  where key.D_CalShowData == narrowKey.D_CalShowData
                                  select key.D_CalShowData;

                if (HaveSameKey.Count() == 0)
                {
                    result.Add(narrowKey, narrow[narrowKey]);
                }
            }

            foreach (var wideKey in wide.Keys)
            {
                var HaveSameKey = from key in result.Keys
                                  where key.D_CalShowData == wideKey.D_CalShowData
                                  select key.D_CalShowData;

                if (HaveSameKey.Count() == 0)
                {
                    result.Add(wideKey, wide[wideKey]);
                }
            }

            var A_XY = from zegData in result
                       orderby zegData.Key.D_CalShowData
                       select zegData;
            result = A_XY.ToDictionary(o => o.Key, o => o.Value);

            return result;
        }
        OilTools oilTool = new OilTools();
        /// <summary>
        /// 获取对应类型的字典类型
        /// </summary>
        /// <param name="XItemcode"></param>
        /// <param name="PropertyY"></param>
        /// <param name="oilTableTypeID"></param>
        /// <returns></returns>
        private Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> getDictionary(string XItemcode, string YItemcode, int oilTableTypeID, Color narrowColor, Color wideColor)
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> result = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();

            if (this._typeCode == CurveTypeCode.RESIDUE)
            {
                #region "添加原油性质数据"
                OilDataEntity oilData = this._oil.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Whole && c.OilTableRow.itemCode == this._curveY.ItemCode).FirstOrDefault();

                if (oilData != null && oilData.calData != string.Empty)
                {
                    double D_calDataY = 0;
                    if (double.TryParse(oilData.calData, out D_calDataY))
                    {
                        ZedGraphOilDataEntity key = new ZedGraphOilDataEntity()
                        {
                            ParticipateInCalculation = "true",
                            ColumnIndex = this._dataColStart,
                            labData = "100",
                            D_CalShowData = 100
                        };

                        ZedGraphOilDataEntity value = new ZedGraphOilDataEntity()
                        {
                            ParticipateInCalculation = "true",
                            ColumnIndex = this._dataColStart,
                            labData = oilData.calData,
                            D_CalShowData = D_calDataY
                        };

                        result.Add(key, value);
                    }
                }
                #endregion
            }

            List<OilDataEntity> oilDatasAll = this._oil.OilDatas.Where(o => o.OilTableTypeID == oilTableTypeID).ToList();
            List<OilDataEntity> oilDatas = oilDatasAll.Where(c => c.OilTableRow.itemCode == XItemcode || c.OilTableRow.itemCode == YItemcode).ToList();

            if (oilDatas.Count() <= 0)
                return result;

            List<OilDataEntity> xList = oilDatas.Where(o => o.OilTableRow.itemCode == XItemcode).ToList();//x轴的坐标集合
            List<OilDataEntity> yList = oilDatas.Where(o => o.OilTableRow.itemCode == YItemcode).ToList();//y轴的坐标集合

            if (xList == null || yList == null)
                return result;
            
            foreach (var xItem in xList)
            {
                #region "添加数据"
                OilDataEntity yItem = yList.Where(o => o.OilTableCol.colCode == xItem.OilTableCol.colCode).FirstOrDefault();//保证数据对应
                if (yItem != null)
                {
                    double d_calDataX = 0, d_calDataY = 0;
                    double d_calShowDataX = 0, d_calShowDataY = 0;

                    if (double.TryParse(yItem.calShowData, out d_calShowDataY) && double.TryParse(xItem.calShowData, out d_calShowDataX)
                        && double.TryParse(yItem.calData, out d_calDataY) && double.TryParse(xItem.calData, out d_calDataX))
                    {
                        ZedGraphOilDataEntity key = new ZedGraphOilDataEntity()
                        {
                            ParticipateInCalculation = "true",
                            labData = xItem.labData,
                            D_CalData = d_calDataY,
                            D_CalShowData = d_calShowDataX,
                            Cell_Color = oilTableTypeID == (int)EnumTableType.Narrow ? narrowColor : wideColor
                        };

                        ZedGraphOilDataEntity value = new ZedGraphOilDataEntity()
                        {
                            ParticipateInCalculation = "true",
                            labData = yItem.labData,
                            D_CalData = d_calDataY,
                            D_CalShowData = d_calShowDataY,
                            Cell_Color = oilTableTypeID == (int)EnumTableType.Narrow ? narrowColor : wideColor
                        };

                        result.Add(key, value);
                    }

                }
                #endregion
            }

            return result;
        }
 

        /// <summary>
        /// 侯乐画合并A库点和B库点以后的曲线
        /// </summary>
        /// <param name="cuveList">A库点和B库点的集合</param>
        public void DrawMergeCurve(List<CurveAEntity> curveList)
        {
            string tag = "MergeAB";
            this.zedGraph1.ZedExpand.removeCurve(zedGraph1, tag);//移除掉原来的曲线
            if (null == curveList)
                return;
            if (curveList.Count <= 0)
                return;
            PointPairList list = this.zedGraph1.ZedExpand.MergeABpointList;
            list.Clear();
            for (int i = curveList.Count() - 1; i >= 0; i--)
            {
                for (int k = 0; k < curveList[i].X.Count(); k++)
                {
                    if (!double.IsNaN(curveList[i].X[k]) && NotInPointPairList(list, curveList[i].X[k]))
                    {
                        list.Add(curveList[i].X[k], curveList[i].Y[k]);//添加A库和B库中不重复的点
                    }
                }
            }

            //foreach (CurveAEntity curve in curveList)
            //{
            //    if (curve.X == null)
            //        return;
            //    for (int i = 0; i < curve.X.Count(); i++)
            //    {
            //        if (!double.IsNaN(curve.X[i]))
            //        {
            //            list.Add(curve.X[i], curve.Y[i]);//添加A库和B库中不重复的点
            //        }
            //    }
            //}

            list.Sort();//对A库和B库合并以后的点按照从小到大的顺序排序
            #region "去掉不参与计算的点"
            if (this.pointListNotuse.Count() > 0)
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    for (int j = 0; j < this.pointListNotuse.Count(); j++)
                    {
                        if (this.pointListNotuse[j].X == list[i].X && this.pointListNotuse[j].Y == list[i].Y)
                        {
                            list.RemoveAt(i); 
                            i--;
                        }
                    }
                }
            }
            #endregion

            LineItem line = this.zedGraph1.GraphPane.AddCurve("", list, Color.Red, SymbolType.Diamond);
            line.Line.IsSmooth = true;
            line.Line.SmoothTension = 0.5F;//曲线平滑度
            line.Tag = tag;
            this.zedGraph1.Refresh();
        }

        /// <summary>
        /// 判断当前点是否在C库中
        /// </summary>
        /// <param name="c">C库点的集合</param>
        /// <param name="x">点的X轴坐标</param>
        /// <returns></returns>
        private bool NotInPointPairList(PointPairList c, double x)
        {
            foreach (PointPair item in c)
            {
                if (item.X == x)
                    return false;
            }
            return true;
        }

        #region 馏分曲线表头以及行设置
        /// <summary>
        /// 设置表头
        /// </summary>
        /// <param name="mergeResult">输入数据</param>
        /// <param name="HaveB_X">判断X轴是否显示数据</param>
        private void _SetCol(int dataColNum, bool HaveB_X)
        {
            if (dataColNum < 0)
                return;

            this.dataGridView.Columns.Clear();

            #region 固定的列

            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "项目",
                ReadOnly = true,
                Name = "itemName",
                Tag = "",
                Width = 120,
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

            int COLNUM = this._dataBcol;//根据曲线类型判断数据的列

            if (this._typeCode == CurveTypeCode.RESIDUE)
                COLNUM = this._dataBcolResidu;
            else
                COLNUM = this._dataBcol;

            if (dataColNum <= COLNUM)
            {
                #region "根据数据显示列"
                int i = 1;
                if (HaveB_X == true)
                {
                    for (int k = 0; k < COLNUM; k++)
                    {
                        GridOilCol col = new GridOilCol()
                        {
                            HeaderText = "Cut" + (k + i).ToString(),
                            //Name = "校正",
                            Width = 60,
                            Tag = "true",
                            SortMode = DataGridViewColumnSortMode.NotSortable
                        };
                        this.dataGridView.Columns.Add(col);
                    }
                }
                else
                {
                    for (int k = 0; k < dataColNum; k++)
                    {
                        GridOilCol col = new GridOilCol()
                        {
                            HeaderText = "Cut" + (k + i).ToString(),
                            //Name = "校正",
                            Width = 60,
                            Tag = "true",
                            SortMode = DataGridViewColumnSortMode.NotSortable
                        };
                        this.dataGridView.Columns.Add(col);
                    }
                }
                #endregion
            }
            else
            {
                #region "根据数据显示列"
                int i = 1;

                for (int k = 0; k < dataColNum; k++)
                {
                    GridOilCol col = new GridOilCol()
                    {
                        HeaderText = "Cut" + (k + i).ToString(),
                        //Name = "校正",
                        Width = 60,
                        Tag = "true",
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    this.dataGridView.Columns.Add(col);
                }

                #endregion
            }
        }
        /// <summary>
        /// 设置非渣油表的行
        /// </summary>
        private void _setRow()
        {
            this.dataGridView.Rows.Clear();
            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            OilTableRowEntity ECPRow = tempRowList.Where(o => o.itemCode == "ECP").FirstOrDefault();
            OilTableRowEntity tempRowX = tempRowList.Where(o => o.itemCode == this._curveX.ItemCode).FirstOrDefault();
            OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode).FirstOrDefault();

            #region "添加行"
            GridOilRow rowAX = new GridOilRow();
            rowAX.RowEntity = tempRowX;
            rowAX.CreateCells(this.dataGridView, tempRowX.itemName, tempRowX.itemUnit, "原始库");
            rowAX.ReadOnly = true;
            this.dataGridView.Rows.Add(rowAX);
            tempRowX.RowIndex = rowAX.Index;

            GridOilRow rowAY = new GridOilRow();
            rowAY.RowEntity = tempRowY;
            rowAY.CreateCells(this.dataGridView, tempRowY.itemName, tempRowY.itemUnit, "原始库");
            rowAY.ReadOnly = true;
            this.dataGridView.Rows.Add(rowAY);
            tempRowY.RowIndex = rowAY.Index;
            #endregion
            #region "添加行"
            GridOilRow ICP = new GridOilRow();
            ICP.RowEntity = ECPRow;
            ICP.CreateCells(this.dataGridView, ECPRow.itemName, ECPRow.itemUnit, "  ");
            ICP.ReadOnly = true;
            this.dataGridView.Rows.Add(ICP);//添加第三行  
            #endregion
            #region "添加行"
            GridOilRow row = new GridOilRow();
            row.RowEntity = tempRowX;
            row.CreateCells(this.dataGridView, tempRowX.itemName, tempRowX.itemUnit, "应用库");
            row.ReadOnly = true;
            this.dataGridView.Rows.Add(row);//添加第四行        

            GridOilRow LastRow = new GridOilRow();
            LastRow.RowEntity = tempRowY;
            LastRow.CreateCells(this.dataGridView, tempRowY.itemName, tempRowY.itemUnit, "应用库");
            LastRow.ReadOnly = false;
            this.dataGridView.Rows.Add(LastRow);//添加第五行
            #endregion
        }
        
        /// <summary>
        /// 设置单元格最后两行,在第A库X轴填充值
        /// </summary>
        /// <param name="keys"></param>
        private void _setDataGridViewCurveXRow()
        {
            if (this.dataGridView.Rows.Count < 2)
                return;

            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            OilTableRowEntity tempRowX = tempRowList.Where(o => o.itemCode == this._curveX.ItemCode && o.oilTableTypeID == this._curveX.OilTableTypeID).FirstOrDefault();
            OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveX.OilTableTypeID).FirstOrDefault();

            GridOilRow row = new GridOilRow();
            row.RowEntity = tempRowX;
            row.CreateCells(this.dataGridView, tempRowX.itemName, tempRowX.itemUnit, "应用库");
            row.ReadOnly = true;
            this.dataGridView.Rows.Add(row);//添加第三行

            int colIndex = this._dataColStart;

            for (int i = 0; i < this.B_XList.Count; i++)
            {
                this.dataGridView.Rows[row.Index].Cells[colIndex++] = new GridOilCell()
                {
                    Value = this.B_XList[i]
                };
            }

            GridOilRow LastRow = new GridOilRow();
            LastRow.RowEntity = tempRowY;
            LastRow.CreateCells(this.dataGridView, tempRowY.itemName, tempRowY.itemUnit, "应用库");
            LastRow.ReadOnly = false;
            this.dataGridView.Rows.Add(LastRow);//添加第四行

        }
        /// <summary>
        /// 设置单元格最后两行,在第A库X轴填充值
        /// </summary>
        /// <param name="keys"></param>
        private void _addValueToDataGridViewLastTwoRow()
        {
            if (this.dataGridView.Rows.Count < 5)
                return;
            if (this._typeCode != CurveTypeCode.RESIDUE)
            {
                #region "!RESIDUE"
                CurveEntity XCurve = this._oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == this._curveX.ItemCode).FirstOrDefault();
                CurveEntity YCurve = this._oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == this._curveY.ItemCode).FirstOrDefault();

                if (XCurve == null || YCurve == null)
                    return;
                if (XCurve.curveDatas.Count <= 0 || YCurve.curveDatas.Count <= 0)
                    return;

                Dictionary<float, List<float>> ECP_X_Y = new Dictionary<float, List<float>>();

                for (int index = 0; index < XCurve.curveDatas.Count; index++)
                {
                    float ecp = XCurve.curveDatas[index].xValue;
                    CurveDataEntity tempCurveData = YCurve.curveDatas.Where(o => o.xValue == ecp).FirstOrDefault();
                    if (tempCurveData != null)
                    {
                        List<float> X_Y = new List<float>();
                        X_Y.Add(XCurve.curveDatas[index].yValue);
                        X_Y.Add(tempCurveData.yValue);
                        ECP_X_Y.Add(ecp, X_Y);
                    }
                }
                if (ECP_X_Y.Count <= 0)
                    return;

                foreach (float ECP_Key in ECP_X_Y.Keys)
                {
                    int colIndex = FindItemCodeValueColIndexfromSpecRow(ECP_Key, 2);
                    if (colIndex != -1)
                    {
                        this.dataGridView.Rows[this._curveRowXIndex].Cells[colIndex].Value = ECP_X_Y[ECP_Key][0];
                        this.dataGridView.Rows[this._curveRowYIndex].Cells[colIndex].Value = ECP_X_Y[ECP_Key][1];
                    }
                }
                #endregion
            }
            else if (this._typeCode == CurveTypeCode.RESIDUE)
            {
                #region "RESIDUE"
                CurveEntity XCurve = this._oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == this._curveX.ItemCode).FirstOrDefault();
                CurveEntity YCurve = this._oilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == this._curveY.ItemCode).FirstOrDefault();

                if (XCurve == null || YCurve == null)
                    return;
                if (XCurve.curveDatas.Count <= 0 || YCurve.curveDatas.Count <= 0)
                    return;

                Dictionary<float, List<float>> ECP_X_Y = new Dictionary<float, List<float>>();

                for (int index = 0; index < XCurve.curveDatas.Count; index++)
                {
                    float ecp = XCurve.curveDatas[index].xValue;
                    if (ECP_WYDic.Keys.Contains(ecp))
                    {
                        float wy = ECP_WYDic[ecp];
                        CurveDataEntity tempCurveData = YCurve.curveDatas.Where(o => o.xValue == wy).FirstOrDefault();
                        if (tempCurveData != null)
                        {
                            List<float> X_Y = new List<float>();
                            X_Y.Add(XCurve.curveDatas[index].yValue);
                            X_Y.Add(tempCurveData.yValue);
                            ECP_X_Y.Add(ecp, X_Y);
                        }
                    }
                }
                if (ECP_X_Y.Count <= 0)
                    return;

                foreach (float ECP_Key in ECP_X_Y.Keys)
                {
                    int colIndex = FindItemCodeValueColIndexfromSpecRow(ECP_Key, 2);
                    if (colIndex != -1)
                    {
                        this.dataGridView.Rows[this._curveRowXIndex].Cells[colIndex].Value = ECP_X_Y[ECP_Key][0];
                        this.dataGridView.Rows[this._curveRowYIndex].Cells[colIndex].Value = ECP_X_Y[ECP_Key][1];
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 设置单元格最后两行,不在第A库X轴填充值
        /// </summary>
        /// <param name="keys"></param>
        private void _setDataGridViewLastTwoRowWithoutValue()
        {
            if (this.dataGridView.Rows.Count < 2)
                return;

            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            OilTableRowEntity tempRowX = tempRowList.Where(o => o.itemCode == this._curveX.ItemCode && o.oilTableTypeID == this._curveX.OilTableTypeID).FirstOrDefault();
            OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveX.OilTableTypeID).FirstOrDefault();

            GridOilRow row = new GridOilRow();
            row.RowEntity = tempRowX;
            row.CreateCells(this.dataGridView, tempRowX.itemName, tempRowX.itemUnit, "应用库");
            row.ReadOnly = true;
            this.dataGridView.Rows.Add(row);//添加第三行

            GridOilRow LastRow = new GridOilRow();
            row.RowEntity = tempRowY;
            LastRow.CreateCells(this.dataGridView, tempRowY.itemName, tempRowY.itemUnit, "应用库");
            LastRow.ReadOnly = true;
            this.dataGridView.Rows.Add(LastRow);//添加第四行

        }
        /// <summary>
        /// 设置单元格前两行,前两行全部为空值
        /// </summary>
        private void _setDataGridViewBeforeTowRowWithoutValue()
        {
            #region " 添加前两行"

            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            OilTableRowEntity tempRowX = tempRowList.Where(o => o.itemCode == this._curveX.ItemCode).FirstOrDefault();
            OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode).FirstOrDefault();

            GridOilRow rowAX = new GridOilRow();
            rowAX.RowEntity = tempRowX;
            rowAX.CreateCells(this.dataGridView, tempRowX.itemName, tempRowX.itemUnit, "原始库");
            rowAX.ReadOnly = true;
            this.dataGridView.Rows.Add(rowAX);
            tempRowX.RowIndex = rowAX.Index;

            GridOilRow rowAY = new GridOilRow();
            rowAY.RowEntity = tempRowY;
            rowAY.CreateCells(this.dataGridView, tempRowY.itemName, tempRowY.itemUnit, "原始库");
            rowAY.ReadOnly = true;
            this.dataGridView.Rows.Add(rowAY);
            tempRowY.RowIndex = rowAY.Index;

            #endregion
        }

        #endregion

        #region 排序比较类
        /// <summary>
        /// 
        /// </summary>
        public class OilComparable : IComparer<ZedGraphOilDataEntity>
        {
            public int Compare(ZedGraphOilDataEntity one, ZedGraphOilDataEntity two)
            {
                if (one.D_CalShowData > two.D_CalShowData)
                    return 1;

                else if (one.D_CalShowData < two.D_CalShowData)
                    return -1;

                else
                    return 0;
            }
        }
        //public class DoubleComparable : IComparer<double>
        //{
        //    public int Compare(double one, double two)
        //    {
        //        if (one.D_CalData > two.D_CalData)
        //            return 1;

        //        else if (one.D_CalData < two.D_CalData)
        //            return -1;

        //        else
        //            return 0;
        //    }
        //}
        #endregion

        #endregion

        #region "按钮事件"

        #region "参考曲线"
        /// <summary>
        /// 是否存在该类型的子窗体
        /// </summary>
        /// <param name="childFrmType">窗体类型</param>
        /// <returns>存在返回1并激活此窗口，不存在返回0</returns>
        public bool IsExistChildFrm(string childFrmType)
        {
            bool flag = false;
            foreach (Form frm in Application.OpenForms)
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
        /// 绘制参考原油
        /// </summary>
        public void DrawReferOil()
        {
            this.OilBList = oilBlist.OilBList;

            if (this.OilBList != null && this.OilBList.Count > 0)
            {
                this.zedGraph1.ZedExpand.removeCurve(zedGraph1, referOilTag);//侯乐清除掉上一次的原油参考曲线

                for (int i = 0; i < this.OilBList.Count; i++)
                {
                    CurveEntity newCurve = this.OilBList[i].curves.Where(o => o.propertyX == this._curveX.ItemCode && o.propertyY == this._curveY.ItemCode).FirstOrDefault();
                    if (newCurve != null)
                    {
                        List<CurveDataEntity> dataList = newCurve.curveDatas;
                        List<double> X = new List<double>();
                        List<double> Y = new List<double>();
                        for (int j = 0; j < dataList.Count; j++)
                        {
                            X.Add(dataList[j].xValue);
                            Y.Add(dataList[j].yValue);
                        }
                        DrawReferOil(X, Y, this.OilBList[i].crudeName, RandomColor(i));
                    }
                }
            }
            else
            {
                this.zedGraph1.ZedExpand.removeCurve(zedGraph1, referOilTag);//侯乐清除掉上一次的原油参考曲线
            }
        }
        /// <summary>
        /// 画出参考曲线
        /// </summary>
        /// <param name="X">x坐标轴集合</param>
        /// <param name="y">y坐标轴集合</param>
        /// <param name="curveName">曲线名称</param>
        /// <param name="curveColor">曲线颜色</param>
        private void DrawReferOil(List<double> X, List<double> Y, string curveName, Color curveColor)
        {
            if (null == X || null == Y)
                return;
            if (X.Count <= 0 || Y.Count <= 0)
                return;
            if (X.Count != Y.Count)
                return;

            PointPairList points = new PointPairList();

            for (int i = 0; i < X.Count; i++)
            {
                points.Add(X[i], Y[i]);
            }
            LineItem line = this.zedGraph1.GraphPane.AddCurve(curveName, points, curveColor, SymbolType.None);
            line.Tag = referOilTag;//侯乐添加标记位，为了删除曲线使用                       
        }
        /// <summary>
        /// 选择参考曲线的颜色
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Color RandomColor(int index)
        {
            List<Color> colorCollection = new List<Color>() { Color.Green ,Color .GreenYellow ,Color.Yellow ,Color .Blue ,Color.Violet,Color.LightGreen ,
                Color.Pink ,Color.LemonChiffon ,Color.Gray ,Color .DarkRed ,Color .Gold};
            if (index < colorCollection.Count && index >= 0)
            {
                return colorCollection[index];
            }

            return Color.Black;
        }
        #endregion
 
        /// <summary>
        /// 曲线内插
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            this._IsDraw = false;//是否绘图   
            if (this.Created)
                this._HaveSave = false;
            DataBaseACurve.refreshDGVColWidthafterCurveAdjust(ref dataGridView, DatasBeforAdjust);
            refreshCurve();
            FrmCurveAFunction.frmCurveInterpolation(dataGridView, this._typeCode, this._curveX, this._curveY);
            drawCurve();
        }
        /// <summary>
        /// 设置非渣油曲线的单元格最后一行的值
        /// </summary>
        /// <param name="keys"></param>
         private void addValuetoLastRowDataGridView(List<CurveDataEntity> curveDatas)
        {
            if (curveDatas == null)
                return;
            if (curveDatas.Count <= 0)
                return;

            for (int i = 0; i < curveDatas.Count; i++)
            {
                int col = FindB_XColIndex(curveDatas[i].xValue);
                if (col > 0)
                {
                    this.dataGridView[col, this._lastRowNum].Value = curveDatas[i].yValue;
                }
            }
        }
        /// <summary>
        /// 设置非渣油曲线的单元格最后两行的值
        /// </summary>
        /// <param name="keys"></param>
        private void addValuetoResidueLastRowDataGridView(List<CurveDataEntity> curveDatas)
        {
            if (curveDatas == null)
                return;
            if (curveDatas.Count <= 0)
                return;

            for (int i = 0; i < curveDatas.Count; i++)
            {
                int colNum = FindItemCodeValueColIndexfromSpecRow(curveDatas[i].xValue, this._curveRowXIndex);
                if (colNum != -1)
                {
                    this.dataGridView[colNum, this._curveRowYIndex].Value = curveDatas[i].yValue;
                }
            }
        }
         
        /// <summary>
        /// 曲线外延
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            this._IsDraw = false;
            if (this.Created)
                this._HaveSave = false;
            DataBaseACurve.refreshDGVColWidthafterCurveAdjust(ref dataGridView, DatasBeforAdjust);//调整单元格的列宽
            FrmCurveAFunction.frmCurveEpitaxial(dataGridView, this._typeCode, this._curveX, this._curveY);
            drawCurve();
        }
        /// <summary>
        /// 扣除和添加窄馏分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //this._IsDraw = false;
            if (this.Created)
                this._HaveSave = false;
            DataBaseACurve.refreshDGVColWidthafterCurveAdjust(ref dataGridView, DatasBeforAdjust);//调整单元格的列宽
            
            if (this._typeCode != CurveTypeCode.RESIDUE)
            {
                //通过空间的名称来判断曲线的走势
                if (this.toolStripButton1.Text == "扣除宽馏分" && this.toolStripButton2.Text == "扣除窄馏分")
                {
                    this.toolStripButton2.Text = "添加窄馏分";

                    #region "扣除窄馏分"
                    for (int i = this._dataColStart; i < this.dataGridView.ColumnCount; i++)//扣除宽馏分
                    {
                        if (this.dataGridView.Columns[i].Tag.Equals("true") && this.dataGridView[i, 0].Style.ForeColor == Color.Red && this.dataGridView[i, 1].Style.ForeColor == Color.Red)
                        {
                            string tempX = this.dataGridView[i, 0].Value == null ? string.Empty : this.dataGridView[i, 0].Value.ToString();
                            string tempY = this.dataGridView[i, 1].Value == null ? string.Empty : this.dataGridView[i, 1].Value.ToString();
                            if (tempX == string.Empty || tempY == string.Empty)
                                continue;

                            double x = 0, y = 0;
                            if (double.TryParse(tempX, out x) && double.TryParse(tempY, out y))
                            {                 
                                PointPair currentPoint = new PointPair(x, y);
                                pointListNotuse.Add(currentPoint);      //将点加入到列表中，在同一物性下重新绘制曲线的时候使用
                                this.zedGraph1.ZedExpand.DrawPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);                                
                                this.dataGridView.Columns[i].Tag = "false";
                            }
                        }
                    }
                
                    this.zedGraph1.Refresh();

                    #region 
                    //var D_Keys = from item in DataADic
                    //             orderby item.Key
                    //             select item.Key;
                    //var D_Values = from item in DataADic
                    //               orderby item.Key
                    //               select item.Value;

                    //List<float> A_X = D_Keys.ToList();
                    //List<float> A_Y = D_Values.ToList();

                    //List<float> input = this.B_XList;
                    //List<float> output = new List<float>();
                    //output = SplineLineInterpolate.spline(A_X, A_Y, input);
                    //refreshCurve();
                    //setLastRowDataGridViewValue(output);
                    //drawCurve();
                    #endregion 

                    #endregion
                }
                else if (this.toolStripButton1.Text == "添加宽馏分" && this.toolStripButton2.Text == "扣除窄馏分")
                {
                    this.toolStripButton2.Text = "添加窄馏分";

                    #region "扣除窄馏分"
                    for (int i = this._dataColStart; i < this.dataGridView.ColumnCount; i++)//扣除宽馏分
                    {
                        if (this.dataGridView.Columns[i].Tag.Equals("true") && this.dataGridView[i, 0].Style.ForeColor == Color.Red && this.dataGridView[i, 1].Style.ForeColor == Color.Red)
                        {
                            string tempX = this.dataGridView[i, 0].Value == null ? string.Empty : this.dataGridView[i, 0].Value.ToString();
                            string tempY = this.dataGridView[i, 1].Value == null ? string.Empty : this.dataGridView[i, 1].Value.ToString();
                            if (tempX == string.Empty || tempY == string.Empty)
                                continue;

                            double x = 0, y = 0;
                            if (double.TryParse(tempX, out x) && double.TryParse(tempY, out y))
                            {
                                PointPair currentPoint = new PointPair(x, y);
                                pointListNotuse.Add(currentPoint);      //将点加入到列表中，在同一物性下重新绘制曲线的时候使用
                                this.zedGraph1.ZedExpand.DrawPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);
                                this.dataGridView.Columns[i].Tag = "false";
                            }
                        }
                    }

                    this.zedGraph1.Refresh();
                    #endregion

                    #region "非渣油曲线内插" //Color.Blue=宽馏分 --- Color.Red = 窄馏分
                    //Dictionary<float, float> DataADic = new Dictionary<float, float>();//获取宽馏分的值
                    //var D_Keys = from item in DataADic
                    //             orderby item.Key
                    //             select item.Key;
                    //var D_Values = from item in DataADic
                    //               orderby item.Key
                    //               select item.Value;

                    //List<float> A_X = D_Keys.ToList();
                    //List<float> A_Y = D_Values.ToList();

                    //List<float> input = this.B_XList;
                    //List<float> output = new List<float>();
                    //output = SplineLineInterpolate.spline(A_X, A_Y, input);
                    //refreshCurve();
                    //setLastRowDataGridViewValue(output);
                    //drawCurve();
                    #endregion
                }
                else if (this.toolStripButton1.Text == "扣除宽馏分" && this.toolStripButton2.Text == "添加窄馏分")
                {
                    this.toolStripButton2.Text = "扣除窄馏分";

                    #region "添加窄馏分"
                    for (int i = this._dataColStart; i < this.dataGridView.ColumnCount; i++)//扣除宽馏分
                    {
                        if (this.dataGridView.Columns[i].Tag.Equals("false") && this.dataGridView[i, 0].Style.ForeColor == Color.Red && this.dataGridView[i, 1].Style.ForeColor == Color.Red)
                        {
                            string tempX = this.dataGridView[i, 0].Value == null ? string.Empty : this.dataGridView[i, 0].Value.ToString();
                            string tempY = this.dataGridView[i, 1].Value == null ? string.Empty : this.dataGridView[i, 1].Value.ToString();
                            if (tempX == string.Empty || tempY == string.Empty)
                                continue;

                            double x = 0, y = 0;
                            if (double.TryParse(tempX, out x) && double.TryParse(tempY, out y))
                            {
                                PointPair currentPoint = new PointPair(x, y);
                                pointListNotuse.Remove(currentPoint);          //从不参与计算集合中删除该点
                                this.zedGraph1.ZedExpand.ClearPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);
                                this.dataGridView.Columns[i].Tag = "true";
                            }
                        }
                    }

                    this.zedGraph1.Refresh();
                    #endregion
                    
                    #region "非渣油曲线内插" //Color.Blue=宽馏分 --- Color.Red = 窄馏分
                    //Dictionary<float, float> DataADic = getDataBaseAfromDataGridView();//获取宽馏分和窄馏分的值
                    //var D_Keys = from item in DataADic
                    //             orderby item.Key
                    //             select item.Key;
                    //var D_Values = from item in DataADic
                    //               orderby item.Key
                    //               select item.Value;

                    //List<float> A_X = D_Keys.ToList();
                    //List<float> A_Y = D_Values.ToList();

                    //List<float> input = this.B_XList;
                    //List<float> output = new List<float>();
                    //output = SplineLineInterpolate.spline(A_X, A_Y, input);
                    //refreshCurve();
                    //setLastRowDataGridViewValue(output);
                    //drawCurve();
                    #endregion
                }
                else if (this.toolStripButton1.Text == "添加宽馏分" && this.toolStripButton2.Text == "添加窄馏分")
                {
                    this.toolStripButton2.Text = "扣除窄馏分";

                    #region "添加窄馏分"
                    for (int i = this._dataColStart; i < this.dataGridView.ColumnCount; i++)//扣除宽馏分
                    {
                        if (this.dataGridView.Columns[i].Tag.Equals("false") && this.dataGridView[i, 0].Style.ForeColor == Color.Red && this.dataGridView[i, 1].Style.ForeColor == Color.Red)
                        {
                            string tempX = this.dataGridView[i, 0].Value == null ? string.Empty : this.dataGridView[i, 0].Value.ToString();
                            string tempY = this.dataGridView[i, 1].Value == null ? string.Empty : this.dataGridView[i, 1].Value.ToString();
                            if (tempX == string.Empty || tempY == string.Empty)
                                continue;

                            double x = 0, y = 0;
                            if (double.TryParse(tempX, out x) && double.TryParse(tempY, out y))
                            {
                                PointPair currentPoint = new PointPair(x, y);
                                pointListNotuse.Remove(currentPoint);          //从不参与计算集合中删除该点
                                this.zedGraph1.ZedExpand.ClearPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);
                                this.dataGridView.Columns[i].Tag = "true";
                            }
                        }
                    }

                    this.zedGraph1.Refresh();
                    #endregion

                    #region "非渣油曲线内插" //Color.Blue=宽馏分 --- Color.Red = 窄馏分
                    //Dictionary<float, float> DataADic = getNarrowDataBaseAfromDataGridView(Color.Red);//获取宽馏分的值
                    //var D_Keys = from item in DataADic
                    //             orderby item.Key
                    //             select item.Key;
                    //var D_Values = from item in DataADic
                    //               orderby item.Key
                    //               select item.Value;

                    //List<float> A_X = D_Keys.ToList();
                    //List<float> A_Y = D_Values.ToList();

                    //List<float> input = this.B_XList;
                    //List<float> output = new List<float>();
                    //output = SplineLineInterpolate.spline(A_X, A_Y, input);
                    //refreshCurve();
                    //setLastRowDataGridViewValue(output);
                    //drawCurve();
                    #endregion
                }
            }
        }
        /// <summary>
        /// 扣除和添加宽馏分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //this._IsDraw = false;
            if (this.Created)
                this._HaveSave = false;

            DataBaseACurve.refreshDGVColWidthafterCurveAdjust(ref dataGridView, DatasBeforAdjust);//调整单元格的列宽

            if (this._typeCode != CurveTypeCode.RESIDUE)
            {
                if (this.toolStripButton1.Text == "扣除宽馏分" && this.toolStripButton2.Text == "扣除窄馏分")
                {
                    this.toolStripButton1.Text = "添加宽馏分";

                    #region "扣除宽馏分"
                    for (int i = this._dataColStart; i < this.dataGridView.ColumnCount; i++)//扣除宽馏分
                    {
                        if (this.dataGridView.Columns[i].Tag.Equals("true") && this.dataGridView[i, 0].Style.ForeColor == Color.Blue && this.dataGridView[i, 1].Style.ForeColor == Color.Blue)
                        {
                            string tempX = this.dataGridView[i, 0].Value == null ? string.Empty : this.dataGridView[i, 0].Value.ToString();
                            string tempY = this.dataGridView[i, 1].Value == null ? string.Empty : this.dataGridView[i, 1].Value.ToString();
                            if (tempX == string.Empty || tempY == string.Empty)
                                continue;

                            double x = 0, y = 0;
                            if (double.TryParse(tempX, out x) && double.TryParse(tempY, out y))
                            {
                                PointPair currentPoint = new PointPair(x, y);
                                this.pointListNotuse.Add(currentPoint);      //将点加入到列表中，在同一物性下重新绘制曲线的时候使用
                                this.zedGraph1.ZedExpand.DrawPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);
                                this.dataGridView.Columns[i].Tag = "false";
                            }
                        }
                    }

                    this.zedGraph1.Refresh();
                    #endregion

                    #region "非渣油曲线内插" //Color.Blue=宽馏分 --- Color.Red = 窄馏分
                    //Dictionary<float, float> DataADic = getNarrowDataBaseAfromDataGridView(Color.Red);//获取宽馏分的值
                    //var D_Keys = from item in DataADic
                    //             orderby item.Key
                    //             select item.Key;
                    //var D_Values = from item in DataADic
                    //               orderby item.Key
                    //               select item.Value;

                    //List<float> A_X = D_Keys.ToList();
                    //List<float> A_Y = D_Values.ToList();

                    //List<float> input = this.B_XList;
                    //List<float> output = new List<float>();
                    //output = SplineLineInterpolate.spline(A_X, A_Y, input);
                    //refreshCurve();
                    //setLastRowDataGridViewValue(output);
                    //drawCurve();
                    #endregion
                }
                else if (this.toolStripButton1.Text == "添加宽馏分" && this.toolStripButton2.Text == "扣除窄馏分")
                {
                    this.toolStripButton1.Text = "扣除宽馏分";

                    #region "添加宽馏分"
                    for (int i = this._dataColStart; i < this.dataGridView.ColumnCount; i++)//扣除宽馏分
                    {
                        if (this.dataGridView.Columns[i].Tag.Equals("false") && this.dataGridView[i, 0].Style.ForeColor == Color.Blue && this.dataGridView[i, 1].Style.ForeColor == Color.Blue)
                        {
                            string tempX = this.dataGridView[i, 0].Value == null ? string.Empty : this.dataGridView[i, 0].Value.ToString();
                            string tempY = this.dataGridView[i, 1].Value == null ? string.Empty : this.dataGridView[i, 1].Value.ToString();
                            if (tempX == string.Empty || tempY == string.Empty)
                                continue;

                            double x = 0, y = 0;
                            if (double.TryParse(tempX, out x) && double.TryParse(tempY, out y))
                            {
                                PointPair currentPoint = new PointPair(x, y);
                                pointListNotuse.Remove(currentPoint);          //从不参与计算集合中删除该点
                                this.zedGraph1.ZedExpand.ClearPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);
                                this.dataGridView.Columns[i].Tag = "true";
                            }
                        }
                    }

                    this.zedGraph1.Refresh();
                    #endregion

                    #region "非渣油曲线内插" //Color.Blue=宽馏分 --- Color.Red = 窄馏分
                    //Dictionary<float, float> DataADic = DataBaseACurve.getDataBaseAfromDataGridView(this.dataGridView);
                    //var D_Keys = from item in DataADic
                    //             orderby item.Key
                    //             select item.Key;
                    //var D_Values = from item in DataADic
                    //               orderby item.Key
                    //               select item.Value;

                    //List<float> A_X = D_Keys.ToList();
                    //List<float> A_Y = D_Values.ToList();

                    //List<float> input = this.B_XList;
                    //List<float> output = new List<float>();
                    //output = SplineLineInterpolate.spline(A_X, A_Y, input);
                    //refreshCurve();
                    //setLastRowDataGridViewValue(output);
                    //drawCurve();
                    #endregion
                }
                else if (this.toolStripButton1.Text == "扣除宽馏分" && this.toolStripButton2.Text == "添加窄馏分")
                {
                    this.toolStripButton1.Text = "添加宽馏分";

                    #region "扣除宽馏分"
                    for (int i = this._dataColStart; i < this.dataGridView.ColumnCount; i++)//扣除宽馏分
                    {
                        if (this.dataGridView.Columns[i].Tag.Equals("true") && this.dataGridView[i, 0].Style.ForeColor == Color.Blue && this.dataGridView[i, 1].Style.ForeColor == Color.Blue)
                        {
                            string tempX = this.dataGridView[i, 0].Value == null ? string.Empty : this.dataGridView[i, 0].Value.ToString();
                            string tempY = this.dataGridView[i, 1].Value == null ? string.Empty : this.dataGridView[i, 1].Value.ToString();
                            if (tempX == string.Empty || tempY == string.Empty)
                                continue;

                            double x = 0, y = 0;
                            if (double.TryParse(tempX, out x) && double.TryParse(tempY, out y))
                            {
                                PointPair currentPoint = new PointPair(x, y);
                                pointListNotuse.Add(currentPoint);      //将点加入到列表中，在同一物性下重新绘制曲线的时候使用
                                this.zedGraph1.ZedExpand.DrawPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);
                                this.dataGridView.Columns[i].Tag = "false";
                            }
                        }
                    }

                    this.zedGraph1.Refresh();
                    #endregion

                    #region "非渣油曲线内插" //Color.Blue=宽馏分 --- Color.Red = 窄馏分
                    //Dictionary<float, float> DataADic = new Dictionary<float,float> ();//获取宽馏分和窄馏分的值
                    //var D_Keys = from item in DataADic
                    //             orderby item.Key
                    //             select item.Key;
                    //var D_Values = from item in DataADic
                    //               orderby item.Key
                    //               select item.Value;

                    //List<float> A_X = D_Keys.ToList();
                    //List<float> A_Y = D_Values.ToList();

                    //List<float> input = this.B_XList;
                    //List<float> output = new List<float>();
                    //output = SplineLineInterpolate.spline(A_X, A_Y, input);
                    //refreshCurve();
                    //setLastRowDataGridViewValue(output);
                    //drawCurve();
                    #endregion
                }
                else if (this.toolStripButton1.Text == "添加宽馏分" && this.toolStripButton2.Text == "添加窄馏分")
                {
                    this.toolStripButton1.Text = "扣除宽馏分";

                    #region "添加宽馏分"
                    for (int i = this._dataColStart; i < this.dataGridView.ColumnCount; i++)//扣除宽馏分
                    {
                        if (this.dataGridView.Columns[i].Tag.Equals("false") && this.dataGridView[i, 0].Style.ForeColor == Color.Blue && this.dataGridView[i, 1].Style.ForeColor == Color.Blue)
                        {
                            string tempX = this.dataGridView[i, 0].Value == null ? string.Empty : this.dataGridView[i, 0].Value.ToString();
                            string tempY = this.dataGridView[i, 1].Value == null ? string.Empty : this.dataGridView[i, 1].Value.ToString();
                            if (tempX == string.Empty || tempY == string.Empty)
                                continue;

                            double x = 0, y = 0;
                            if (double.TryParse(tempX, out x) && double.TryParse(tempY, out y))
                            {
                                PointPair currentPoint = new PointPair(x, y);
                                pointListNotuse.Remove(currentPoint);          //从不参与计算集合中删除该点
                                this.zedGraph1.ZedExpand.ClearPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);
                                this.dataGridView.Columns[i].Tag = "true";
                            }
                        }
                    }

                    this.zedGraph1.Refresh();
                    #endregion

                    #region "非渣油曲线内插" //Color.Blue=宽馏分 --- Color.Red = 窄馏分
                    //Dictionary<float, float> DataADic = getNarrowDataBaseAfromDataGridView(Color.Blue);//获取宽馏分的值
                    //var D_Keys = from item in DataADic
                    //             orderby item.Key
                    //             select item.Key;
                    //var D_Values = from item in DataADic
                    //               orderby item.Key
                    //               select item.Value;

                    //List<float> A_X = D_Keys.ToList();
                    //List<float> A_Y = D_Values.ToList();

                    //List<float> input = this.B_XList;
                    //List<float> output = new List<float>();
                    //output = SplineLineInterpolate.spline(A_X, A_Y, input);
                    //refreshCurve();
                    //setLastRowDataGridViewValue(output);
                    //drawCurve();
                    #endregion
                }
            }

        }

        /// <summary>
        /// 曲线调整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            this._IsDraw = true;
            if (this.Created )
                this._HaveSave = false;

            if (this._typeCode == CurveTypeCode.RESIDUE)
                return;
            if (DatasBeforAdjust.Count > 0)
                DatasBeforAdjust.Clear();//清空数据

            DatasBeforAdjust = FrmCurveAFunction.frmCurveAdjust(dataGridView, this._typeCode, this._curveX, this._curveY);
            drawCurve();
            this.dataGridView.CellPainting += new DataGridViewCellPaintingEventHandler(dataGridView_CellPainting);
            this.dataGridView.Refresh();
        }

        /// <summary>
        /// 曲线调整的值的显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (!this._IsDraw)
                return;

            if (e.RowIndex > (this._lastRowNum - 1) && e.ColumnIndex >= _dataColStart && e.RowIndex != this.dataGridView.NewRowIndex)
            {
                Rectangle btnRect = e.CellBounds;
                btnRect.Width = btnRect.Width - 20;
                btnRect.X = btnRect.X + 1;
                btnRect.Y = btnRect.Y + 1;
                StringFormat formater = new StringFormat();
                Font cellFont = new Font("", 6);//定义字体的大小

                formater.Alignment = StringAlignment.Near;

                e.Paint(btnRect, DataGridViewPaintParts.All);

                this.dataGridView[e.ColumnIndex, e.RowIndex].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (DatasBeforAdjust.Keys.Contains(e.ColumnIndex))
                {
                    e.Graphics.DrawString(DatasBeforAdjust[e.ColumnIndex].ToString(), cellFont, new SolidBrush(Color.Blue), btnRect, formater);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// 累计值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            if (this._typeCode == CurveTypeCode.RESIDUE || this._typeCode == CurveTypeCode.YIELD)
                return;
            if (this.Created)
                this._HaveSave = false;           
            this._IsDraw = false;

            DataBaseACurve.refreshDGVColWidthafterCurveAdjust(ref dataGridView, DatasBeforAdjust);//调整单元格的列宽
            Dictionary<double, double> totalValue = FrmCurveAFunction.frmCumulativeValue(this._oilB ,dataGridView ,this._typeCode ,this._curveX , this._curveY );
             
            if (totalValue.Count() <= 0)
                return;          

            this.zedGraph1.ZedExpand.removeCurve(zedGraph1, this._curveX.Descript + "-" + this._curveY.Descript + "累计曲线");  //清除掉累计曲线
            this.zedGraph1.GraphPane.AddCurve(this._curveX.Descript + "-" + this._curveY.Descript + "累计曲线",
                totalValue.Keys.ToArray(), totalValue.Values.ToArray(), Color.Black);//绘出累计值曲线     
               
            this.zedGraph1.GraphPane.AxisChange();
            this.dataGridView.Refresh();
        }
 

        /// <summary>
        /// 选择项置顶
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            this.cmbCurve.ComboBox.SelectedIndex = 0;
        }
        /// <summary>
        /// 选择上一个选择项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (this.cmbCurve.ComboBox.SelectedIndex - 1 >= 0)
            {
                this.cmbCurve.ComboBox.SelectedIndex = this.cmbCurve.ComboBox.SelectedIndex - 1;
            }
        }
        /// <summary>
        /// 选择下一个选择项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (this.cmbCurve.ComboBox.SelectedIndex + 1 < this.cmbCurve.ComboBox.Items.Count)
            {
                this.cmbCurve.ComboBox.SelectedIndex = this.cmbCurve.ComboBox.SelectedIndex + 1;
            }
        }
        /// <summary>
        /// 选择项置底
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            this.cmbCurve.ComboBox.SelectedIndex = this.cmbCurve.ComboBox.Items.Count - 1;
        }

        #endregion

        /// <summary>
        ///  返回轻端表的数据
        /// </summary>
        /// <param name="ICP0"></param>
        /// <param name="ICP0Color"></param>
        /// <returns></returns>
        private Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> WTorVLightOilDatas(string ICP0, Color ICP0Color)
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> WYorVLightOilDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();//需要返回的WY%或V%列值的轻端表集合       
            OilTableRowEntity ICPRow = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow && o.itemCode == "ICP").FirstOrDefault();
            double DICP0 = 0,DShowICP0=0;
            OilTools oiltool = new OilTools();
            if (!string.IsNullOrWhiteSpace(ICP0))
            {
                string ICP0Show = oiltool.calDataDecLimit(ICP0, ICPRow.decNumber, ICPRow.valDigital);
                if (double.TryParse(ICP0Show, out DShowICP0) && double.TryParse(ICP0, out DICP0))
                {
                    #region "返回数据"
                    var zgKey = new ZedGraphOilDataEntity
                    {
                        //OilTableCol = tempCol,
                        //OilTableRow = ICPRow,
                        Cell_Color = ICP0Color,
                        calData = ICP0,
                        labData = ICP0,
                        D_CalShowData = DShowICP0,
                        D_CalData = DICP0,
                        oilInfoID = this._oil.ID,
                    };
                    var zgValue = new ZedGraphOilDataEntity
                    {
                        //OilTableCol = tempCol,
                        //OilTableRow = TWYRow,
                        Cell_Color = ICP0Color,
                        calData = "0",
                        labData = "0",
                        D_CalData = 0,
                        D_CalShowData = 0,
                        oilInfoID = this._oil.ID,
                    };
                    WYorVLightOilDatas.Add(zgKey, zgValue);

                    #endregion
                }
            }
            return WYorVLightOilDatas;
        }
        #region "向B库中保存原油"
        /// <summary>
        /// 从B库中取出对应的曲线数据
        /// </summary>
        /// <param name="crudeIndex">原油编码</param>
        private void getOilInfoBfromDataBaseB()
        {
            string crudeIndex = this._oil.crudeIndex;
            this._oilB = OilBll.GetOilByCrudeIndex(crudeIndex);

            if (this._oilB == null)
                initOilB();
        }
        /// <summary>
        /// 原油B的初始化(包括原油信息表、原油性质表、GC标准表)
        /// </summary>
        private void initOilB()
        {
            this._oilB = new OilInfoBEntity();
            OilBll.InfoToInfoB(this._frmOilDataA.getData(), this._oilB);
            this._oilB.ID = -1;
        }
        /// <summary>
        /// 保存到B库
        /// </summary>
        public void SaveB()
        {
            if (!this._HaveSave)
            {
                //getCurrentCurveDataGridViewBDatas();
                DatabaseA.Curve.DataBaseACurve.getCurrentCurveFromDataGridViewDataBaseB(ref this._oilB, this.dataGridView, this._typeCode, this._curveX, this._curveY);
            
                if (this._oilB.curves != null && this._oilB.curves.Count > 0)
                    this._saveDataToDataBase = true;

                if (this._saveDataToDataBase)
                {
                    //等待窗体
                    //this.IsBusy = true;
                    SavefrmCurveA();
                    //等待窗体
                    //this.IsBusy = false;
                }
                else
                {
                    var msg = MessageBox.Show("当前窗体没有曲线数据是否要继续保存覆盖以前数据?", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (msg == System.Windows.Forms.DialogResult.Yes)
                    {
                        //等待窗体
                        //this.IsBusy = true;
                        SavefrmCurveA();
                        //等待窗体
                        //this.IsBusy = false;
                    }               
                }
            }
        }
       
        /// <summary>
        /// 把A库的曲线保存到B库
        /// </summary>
        public void SavefrmCurveA()
        {
            try
            {
                OilInfoBEntity tempOilB = OilBll.GetOilByCrudeIndex(this._oil.crudeIndex);//从数据库中取出数据

                if (tempOilB != null) //数据库中存在此条数据  
                    OilBll.delete(tempOilB.ID, LibraryType.LibraryB);//删除此条数据

                this._oilB.ID = -1;

                if (this._oilB.ID < 0)//保存当前原油B
                {
                    OilBll.InfoToInfoB(this._frmOilDataA.getData(),this._oilB);
                    #region "从内存中删除原油性质和GC标准表的数据"
                    List<OilDataBEntity> whole = this._oilB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();
                    if (whole != null)
                    {
                        for (int i = 0; i < whole.Count; i++)
                        {
                            this._oilB.OilDatas.Remove(whole[i]);//从内存中删除数据
                        }
                    }
                    List<OilDataBEntity> gcLevel = this._oilB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
                    if (gcLevel != null)
                    {
                        for (int i = 0; i < gcLevel.Count; i++)
                        {
                            this._oilB.OilDatas.Remove(gcLevel[i]);//从内存中删除数据
                        }
                    }
                    #endregion

                    #region "原油性质表的转换"
                    List<OilDataEntity> AWholeDatas = this._frmOilDataA.getData().OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();//找出A库中性质表的数据
                    List<OilDataBEntity> BWholeDatas = new List<OilDataBEntity>();//B库中性质表的数据
                    if (AWholeDatas != null)
                    {
                        foreach (OilDataEntity temp in AWholeDatas)
                        {
                            OilDataBEntity oilDataB = new OilDataBEntity
                            {
                                calData = temp.calData,
                                labData = temp.labData,
                                oilInfoID = temp.oilInfoID,
                                oilTableColID = temp.oilTableColID,
                                oilTableRowID = temp.oilTableRowID
                            };
                            BWholeDatas.Add(oilDataB);
                        }
                    }
                    this._oilB.OilDatas.AddRange(BWholeDatas);
                    #endregion

                    #region "GC标准表的转换"
                    List<OilDataEntity> AGCLevelDatas = this._frmOilDataA.getData().OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();//找出A库中GC标准表的数据
                    List<OilDataBEntity> BGCLevelDatas = new List<OilDataBEntity>();//B库中GC标准表的数据
                    if (AGCLevelDatas != null)
                    {
                        foreach (OilDataEntity temp in AGCLevelDatas)
                        {
                            OilDataBEntity oilDataB = new OilDataBEntity
                            {
                                calData = temp.calData,
                                labData = temp.labData,
                                oilInfoID = temp.oilInfoID,
                                oilTableColID = temp.oilTableColID,
                                oilTableRowID = temp.oilTableRowID
                            };
                            BGCLevelDatas.Add(oilDataB);
                        }
                    }
                    this._oilB.OilDatas.AddRange(BGCLevelDatas);
                    #endregion
                    this._oilB.ID = OilBll.save(this._oilB);//保存库

                    #region "曲线值转换到数据库"
                    if (this._oilB.curves == null)
                        return;
                    if (this._oilB.curves.Count <= 0)
                        return;

                    int oilInfoID = this._oilB.ID;
                    CurveAccess curveAccess = new CurveAccess();
                    List<CurveEntity> curveList = new List<CurveEntity>();
                    for (int curveIndex = 0; curveIndex < this._oilB.curves.Count; curveIndex++)
                    {
                        CurveEntity newCurve = new CurveEntity();
                        newCurve.propertyX = this._oilB.curves[curveIndex].propertyX;
                        newCurve.propertyY = this._oilB.curves[curveIndex].propertyY;
                        newCurve.oilInfoID = oilInfoID;
                        newCurve.curveTypeID = this._oilB.curves[curveIndex].curveTypeID;
                        newCurve.unit = this._oilB.curves[curveIndex].unit;
                        newCurve.decNumber = this._oilB.curves[curveIndex].decNumber;
                        newCurve.descript = this._oilB.curves[curveIndex].descript;
                        newCurve.ID = curveAccess.Insert(newCurve);
                        newCurve.curveDatas = this._oilB.curves[curveIndex].curveDatas;
                        curveList.Add(newCurve);
                    }
                    OilBll.saveCurves(curveList);

                    #endregion

                    this._saveDataToDataBase = false;
                    this._HaveSave = true;
                }
                else
                {
                    #region
                    //DialogResult r = MessageBox.Show(this._oil.crudeIndex + "原油已存在！是否要更新", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    //if (r == DialogResult.Yes)
                    //{
                    //    OilInfoBAccess oilInfoBAccess = new OilInfoBAccess();
                    //    oilInfoBAccess.Delete("crudeIndex='" + this._oil.crudeIndex + "'");

                    //    infoB.ID = OilBll.SaveInfoB(ref infoB); //保存信息表
                    //    OilBll.ConvertToDatasB(ref infoB, this._oil.OilDatas); //保存数据                   


                    //    SaveC(infoB);//保存到C库
                    //}
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Error("数据向应用库保存不成功：" + ex.ToString());
                MessageBox.Show("数据保存不成功！", "警告信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }        
        }
        /// <summary>
        /// 关闭窗体时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCurveA_FormClosing(object sender, FormClosingEventArgs e)
        {                   
            if (!this._HaveSave)
            {
                //getCurrentCurveDataGridViewBDatas();
                DatabaseA.Curve.DataBaseACurve.getCurrentCurveFromDataGridViewDataBaseB(ref this._oilB, this.dataGridView, this._typeCode, this._curveX, this._curveY);
            
                if (this._oilB.curves != null && this._oilB.curves.Count > 0)
                    this._saveDataToDataBase = true;

                if (this._saveDataToDataBase)
                {
                    var msg = MessageBox.Show("是否保存"+ this._oil.crudeIndex+"数据到应用库？", "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (msg == System.Windows.Forms.DialogResult.Cancel)
                        e.Cancel = true;
                    else if (msg == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            //等待窗体
                            //this.IsBusy = true;
                            SavefrmCurveA();
                            //等待窗体
                            //this.IsBusy = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("曲线数据保存不成功!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log.Error("绘图窗体A结束后的向应用库和查询库中保存数据:" + ex);
                        }
                    }
                }
            }
        }
        #endregion

        #region "绘图面板点拖动事件"
        /// <summary>
        /// 单元格选中事件，表示对应ZedGraph图上的点标记出来
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //this._SaveDataToDataBase = true;//需要向数据库保存数据
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            PointF choosePoint = new PointF();
            PointPairList Alist = this.zedGraph1.ZedExpand.ApointList;      //获取A库点的集合
            PointPairList Blist = this.zedGraph1.ZedExpand.PointList;       //获取B库点的集合

            //判断当前选中的行是否为第一行和第三行
            if (col >= this._dataColStart && (row == this._lastRowNum || row == this._ARowYIndex))
            {
                var cellValueY = this.dataGridView.Rows[row].Cells[col].Value;
                var cellValueX = this.dataGridView.Rows[row - 1].Cells[col].Value;
                if (cellValueY != null && cellValueX != null)
                {
                    if (cellValueY.ToString() != string.Empty && cellValueX.ToString() != string.Empty)      //判断cellValue是否为空
                    {
                        double valueY = Math.Round(double.Parse(cellValueY.ToString()), this.zedGraph1.ZedExpand.DecNumber);
                        double valueX = double.Parse(cellValueX.ToString());
                        //      如果是第三行上的坐标点，则画红色的标记符号
                        if (row == _lastRowNum)
                        {
                            if (Blist.Contains(new PointPair(valueX, valueY)))
                            {
                                choosePoint.X = (float)valueX;
                                choosePoint.Y = (float)valueY;
                                this.zedGraph1.ZedExpand.DrawPoint(choosePoint, "choosePoint", Color.Red, zedGraph1);
                            }
                        }
                        else
                        {    //         第一行上的坐标点，画蓝色的标记符号
                            if (Alist.Contains(new PointPair(valueX, valueY)))
                            {
                                int index = Alist.IndexOf(new PointPair(valueX, valueY));
                                this.zedGraph1.ZedExpand.AcellIndex = index;
                                choosePoint.X = (float)valueX;
                                choosePoint.Y = (float)valueY;
                                this.zedGraph1.ZedExpand.DrawPoint(choosePoint, "bluePoint", Color.Blue, zedGraph1);
                            }

                        }
                    }
                }

            }

        }
        /// <summary>
        ///  单元格结束编辑以后，单元格值重新写入，并且对应图像上的点位置改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            drawCurve();   //重新生成曲线
        }

        /// <summary>
        /// 此点不参与计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZedExpand_NotCalCulate(object sender, EventArgs e)
        {
            #region 
            //double x, y;//存储该不参与计算的点的坐标
            //int xDecNumber = 0; //x坐标的约定位数
            //if (double.TryParse(this.zedGraph1.ZedExpand.Xstr, out x) &&
            //    double.TryParse(this.zedGraph1.ZedExpand.Ystr, out y))
            //{
            //    int colIndex = FindA_XColIndex(x);
            //    if (colIndex > 0)
            //    {
            //        this.dataGridView.Columns[colIndex].Tag = "false";
            //        //this.dataGridView.Rows[_ARowYIndex].Cells[colIndex].Tag = "false";//将此点设置为false
            //    }
            //    x = Math.Round(x, xDecNumber);
            //    y = Math.Round(y, this.zedGraph1.ZedExpand.DecNumber);
            //    pointListNotuse.Add(x, y);      //将点加入到列表中，在同一物性下重新绘制曲线的时候使用
            //    this.zedGraph1.ZedExpand.DrawPointNotUse(x, y, zedGraph1);
            //    this.zedGraph1.Refresh();               
            //}
            #endregion 

            PointPair currentPoint = getApoint(false);//设置当前单元格的标签为false;
            if (currentPoint == null)
                return;
            this.pointListNotuse.Add(currentPoint);      //将点加入到列表中，在同一物性下重新绘制曲线的时候使用
            this.zedGraph1.ZedExpand.DrawPointNotUse(currentPoint.X, currentPoint.Y, zedGraph1);
            this.zedGraph1.Refresh();
        }
        /// <summary>
        /// 此点参与计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZedExpand_Calculate(object sender, EventArgs e)
        {
            #region 
            //double x, y;//存储该不参与计算的点的坐标
            //int xDecNumber = 0; //x坐标的约定位数
            //if (double.TryParse(this.zedGraph1.ZedExpand.Xstr, out x) &&
            //    double.TryParse(this.zedGraph1.ZedExpand.Ystr, out y))
            //{
            //    int colIndex = FindA_XColIndex(x);
            //    if (colIndex > 0)
            //    {
            //        this.dataGridView.Columns[colIndex].Tag = "true";
            //        //this.dataGridView.Rows[_ARowYIndex].Cells[colIndex].Tag = "true";//将此点设置为false
            //    }
            //    x = Math.Round(x, xDecNumber);
            //    y = Math.Round(y, this.zedGraph1.ZedExpand.DecNumber);
            //    for (int i = 0; i < pointListNotuse.Count(); i++)
            //    {
            //        if (Math.Round(pointListNotuse[i].X, 0) == x)
            //            pointListNotuse.RemoveAt(i);
            //        break;
            //    }        //从不使用此点处删除
            //    this.zedGraph1.ZedExpand.ClearPointNotUse(x,y,zedGraph1);
            //    this.zedGraph1.Refresh();
            //}
            #endregion 

            PointPair currentApoint = getApoint(true);      //获取当前所选择的A库的点
            if (currentApoint == null)
                return;
            this.pointListNotuse.Remove(currentApoint);          //从不参与计算集合中删除该点
            this.zedGraph1.ZedExpand.ClearPointNotUse(currentApoint.X, currentApoint.Y, zedGraph1);
            this.zedGraph1.Refresh();

        }
        /// <summary>
        /// 获取当前A库所选择的点
        /// </summary>
        /// <param name="SetTag">标记此时是此点参与计算还是此点不参与计算</param>
        /// <returns>返回PointPair类型，所选择的A库的点</returns>
        private PointPair getApoint(bool SetTag)
        {
            double x = 0;
            double y = 0;
            PointPair currentPoint = new PointPair();
            //存储该不参与计算的点的坐标
            int aCellIndex = this.zedGraph1.ZedExpand.AcellIndex;       //当前A库点的下标
            if (aCellIndex > -1)
            {
                currentPoint = this.zedGraph1.ZedExpand.ApointList[aCellIndex];
                x = this.zedGraph1.ZedExpand.ApointList[aCellIndex].X;      //获取不参与计算A库的点X坐标
                y = this.zedGraph1.ZedExpand.ApointList[aCellIndex].Y;      //获取不参与计算A库点的Y坐标
                int colIndex = FindA_XColIndex(x, y);
                if (colIndex > 0)
                {                
                    //此点参与计算
                    this.dataGridView.Columns[colIndex].Tag = SetTag ? "true" : "false";                 
                }
            }
            return currentPoint;
        }

        /// <summary>
        /// 鼠标点击事件，选中对应的单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraph1_MouseClick(object sender, MouseEventArgs e)
        {
            int aPosition = zedGraph1.ZedExpand.getCurrentPosition(zedGraph1.ZedExpand.ApointList);     //在A库中点的坐标
            int bPositon = zedGraph1.ZedExpand.getCurrentPosition(zedGraph1.ZedExpand.PointList);       //在B库中点的坐标
            double aPositionX = -1; //保存当前点的在A库X坐标值
            double bPositionX = -2;  //保存当期点在B库X坐标值

            int acellIndex = FindA_XColIndex(aPositionX);   //A库点对应的单元格的下标
            int bcellIndex = FindB_XColIndex(bPositionX);   //B库点对应的单元格的下标
            int dec = 0;
            if (aPosition > -1)
            {
                aPositionX = zedGraph1.ZedExpand.ApointList[aPosition].X;
                this.dataGridView.Rows[this._ARowYIndex].Cells[AcellIndex].Selected = true;      //A库单元格选中事件
            }
            if (bPositon > -1)
            {
                bPositionX = zedGraph1.ZedExpand.PointList[bPositon].X;
                this.dataGridView.Rows[this._lastRowNum].Cells[BcellIndex].Selected = true;      //B库单元格选中事件
            }
            if (aPositionX == bPositionX)
            {
                //如果x坐标相似的话则判断Y坐标来显示对应的单元格
                double aY = zedGraph1.
                    ZedExpand.ApointList[aPosition].Y;
                double bY = zedGraph1.ZedExpand.PointList[bPositon].Y;
                double currentY = 0;
                if (double.TryParse(zedGraph1.ZedExpand.Ystr, out currentY))
                {
                    if (Math.Round(aY, dec) == Math.Round(currentY, dec))
                    {
                        this.dataGridView.Rows[this._ARowYIndex].Cells[AcellIndex].Selected = true;         //A库单元格选中事件
                    }
                    if (Math.Round(bY, dec) == Math.Round(currentY, dec))
                    {
                        this.dataGridView.Rows[this._lastRowNum].Cells[BcellIndex].Selected = true;      //B库单元格选中事件
                    }
                }
            }

        }

        #endregion

        #region "窗体功能"
        /// <summary>
        /// 绘制窗体放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void toolStripButton1_Click(object sender, EventArgs e)
        //{
        //    //this.zedGraph1.GraphPane.XAxis.Scale.Max = this.zedGraph1.GraphPane.XAxis.Scale.Max / scaleRate;
        //    //this.zedGraph1.GraphPane.YAxis.Scale.Max = this.zedGraph1.GraphPane.YAxis.Scale.Max / scaleRate;
        //    //this.zedGraph1.Refresh();
        //    this.zedGraph1.ZedExpand.MouseWheel(true);
        //}
        /// <summary>
        /// 绘制窗体缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void toolStripButton2_Click(object sender, EventArgs e)
        //{
        //    //this.zedGraph1.GraphPane.XAxis.Scale.Max = scaleRate * this.zedGraph1.GraphPane.XAxis.Scale.Max;
        //    //this.zedGraph1.GraphPane.YAxis.Scale.Max = scaleRate * this.zedGraph1.GraphPane.YAxis.Scale.Max;
        //    //this.zedGraph1.Refresh();
        //    this.zedGraph1.ZedExpand.MouseWheel(false);
        //}
        /// <summary>
        /// 实窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //this.zedGraph1.GraphPane.XAxis.Scale.Max = axisMax;
            //this.zedGraph1.GraphPane.XAxis.Scale.Min = axisMin;
            //this.zedGraph1.GraphPane.YAxis.Scale.Max = aYisMax;
            //this.zedGraph1.GraphPane.YAxis.Scale.Min = aYisMin;
            this.zedGraph1.ZedExpand.setDefault(null);
            double addValue = this.zedGraph1.GraphPane.XAxis.Scale.Max * 0.2;//适窗时候添加的坐标系数
            this.zedGraph1.GraphPane.XAxis.Scale.Max = this.zedGraph1.GraphPane.XAxis.Scale.Max + addValue;
            this.zedGraph1.GraphPane.XAxis.Scale.Min = this.zedGraph1.GraphPane.XAxis.Scale.Min - addValue;
            this.zedGraph1.Refresh();
        }
        /// <summary>
        /// 全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //this.zedGraph1.GraphPane.XAxis.Scale.Max = axisMax;
            //this.zedGraph1.GraphPane.XAxis.Scale.Min = axisMin;
            //this.zedGraph1.GraphPane.YAxis.Scale.Max = aYisMax;
            //this.zedGraph1.GraphPane.YAxis.Scale.Min = aYisMin;
            //this.zedGraph1.Refresh();

            this.zedGraph1.ZedExpand.setDefault(null);//恢复默认
            this.zedGraph1.Refresh();
        }
        #endregion

        /// <summary>
        /// 区间计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            this.toolStripTextBox3.Text = FrmCurveAFunction.CalculatedIntervalValue(this._oilB, this.dataGridView, this.toolStripTextBox1.Text, this.toolStripTextBox2.Text, this._typeCode, this._curveX, this._curveY);
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            DataBaseACurve.refreshDGVColWidthafterCurveAdjust(ref dataGridView, DatasBeforAdjust);//调整单元格的列宽

            #region "从当前录入表中获取数据"
            FrmMain frmMain = (FrmMain)this.ParentForm;
            DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)frmMain.GetChildFrm(this._oil.crudeIndex + "A");
            #endregion
            if (frmOilDataA == null)
                MessageBox.Show("相关录入表不存在，数据无法刷新!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);         
            else if (frmOilDataA != null && frmOilDataA.IsDisposed == false)
            {
                OilInfoEntity oil = frmOilDataA.getData();     // 一条原油  
                if (oil == null)
                    return;

                this._curveX = (CurveParmTypeEntity)this.comboBox1.SelectedItem; //当前曲线默认为ComboBox控件中的第一条
                this._curveY = (CurveParmTypeEntity)this.cmbCurve.ComboBox.SelectedItem; //当前曲线默认为ComboBox控件中的第一条 

                XYSelectedIndexChanged(oil);
                refreshCurve();

                this.toolStripButton1.Text = "扣除宽馏分";
                this.toolStripButton2.Text = "扣除窄馏分";

                if (this.Created)
                    this._HaveSave = false;
            }
        }
        /// <summary>
        /// 清空当前曲线的数据
        /// </summary>
        private void refreshCurve()
        {
            if (this.dataGridView.Rows.Count < 4)
                return;

            int colLength = this.dataGridView.ColumnCount;
            for (int col = this._dataColStart; col < colLength; col++)
            {
                this.dataGridView.Rows[this._curveRowYIndex].Cells[col].Value = string.Empty;
            }

            CurveEntity currentCurve = this._oilB.curves.Where(o => o.propertyX == this._curveX.ItemCode && o.propertyY == this._curveY.ItemCode && o.curveTypeID == (int)this._typeCode).FirstOrDefault();

            if (currentCurve != null)//从内存中找到曲线、删除。
            {
                this._oilB.curves.Remove(currentCurve);//此处要考虑从B库取出的数据     
                currentCurve.curveDatas.Clear();
            }

            drawCurve();//重新绘制曲线
        }

        /// <summary>
        /// XY轴物性改变事件
        /// </summary>
        private void XYSelectedIndexChanged(OilInfoEntity tempOil)
        {
            //清空空间计算值
            this.toolStripTextBox1.Text = string.Empty;
            this.toolStripTextBox2.Text = string.Empty;
            this.toolStripTextBox3.Text = string.Empty;

            this.pointListNotuse.Clear();
           
            if (this._typeCode == CurveTypeCode.YIELD)
            {
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveY.OilTableTypeID).FirstOrDefault();
                this.zedGraph1.ZedExpand.DecNumber = tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;
                setYIELD(tempOil);
            }
            else if (this._typeCode == CurveTypeCode.DISTILLATE)
            {
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveY.OilTableTypeID).FirstOrDefault();
                this.zedGraph1.ZedExpand.DecNumber = tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;
                setDISTILLATE(tempOil );
            }
            else if (this._typeCode == CurveTypeCode.RESIDUE)
            {
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == this._curveY.ItemCode && o.oilTableTypeID == this._curveY.OilTableTypeID).FirstOrDefault();
                this.zedGraph1.ZedExpand.DecNumber = tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;
                setRESIDUE(tempOil );
            }
            drawCurve();
        }
        /// <summary>
        /// 设置收率曲线
        /// </summary>
        private void setYIELD(OilInfoEntity oilInfoA )
        {
            setButtonEnable(true);
            string wideXItemCode = string.Empty;
            if (this._curveY.ItemCode == "TWY")
            {
                wideXItemCode = "MWY";
 
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrowDatas = DataBaseACurve.getDictionary(oilInfoA,this._typeCode, this._curveX.ItemCode , this._curveY.ItemCode, EnumTableType.Narrow, Color.Red, Color.Blue);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> wideDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, PartCurveItemCode.MCP.ToString(), wideXItemCode, EnumTableType.Wide, Color.Red, Color.Blue);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeNarrWideDatas = MergeCurveParm(narrowDatas, wideDatas);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> ECP_TWYorTVYDictionary = WTorVLightOilDatas(oilInfoA.ICP0, Color.Green);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeLightNarrWide = MergeCurveParm(mergeNarrWideDatas, ECP_TWYorTVYDictionary);
                bool HaveB_X = true;
                _SetCol(mergeLightNarrWide.Count, HaveB_X);//设置列头
                _setRow();
                DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, mergeLightNarrWide);
                DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._curveRowXIndex);
            }
            else
            {
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrowDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, this._curveX.ItemCode, this._curveY.ItemCode, EnumTableType.Narrow, Color.Red, Color.Blue);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> ECP_TWYorTVYDictionary = WTorVLightOilDatas(oilInfoA.ICP0, Color.Green);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeLightNarrWide = MergeCurveParm(narrowDatas, ECP_TWYorTVYDictionary);
                bool HaveB_X = true;
                _SetCol(mergeLightNarrWide.Count, HaveB_X);//设置列头
                _setRow();
                DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, mergeLightNarrWide);
                DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._curveRowXIndex);
            }
        }
        /// <summary>
        /// 设置馏分曲线
        /// </summary>
        private void setDISTILLATE(OilInfoEntity oilInfoA )
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrowDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();//窄馏分表中对应物性的数据
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> wideDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();//宽馏分表中对应物性的数据
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeNarrWideDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();//宽馏分和窄馏分表中对应物性的合并数据

            bool HaveB_X = false;//用于判断B库的X轴是否填写数据          
            #region "窄馏分表的数据提取和并"
            string wideXItemCode = string.Empty;

            if (this._curveX.ItemCode == PartCurveItemCode.ECP.ToString())
            {
                setButtonEnable(true);

                HaveB_X = true;
                wideXItemCode = PartCurveItemCode.MCP.ToString();
                if (this._curveY.OilTableTypeID == (int)EnumTableType.Narrow)
                {
                    narrowDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, this._curveX.ItemCode, this._curveY.ItemCode, EnumTableType.Narrow, Color.Red, Color.Blue);
                    wideDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, wideXItemCode, this._curveY.ItemCode, EnumTableType.Wide, Color.Red, Color.Blue);
                    mergeNarrWideDatas = MergeCurveParm(narrowDatas, wideDatas);
                    _SetCol(mergeNarrWideDatas.Count, HaveB_X);
                    _setRow();
                    DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, mergeNarrWideDatas);
                    DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._curveRowXIndex);
                }
                else if (this._curveY.OilTableTypeID == (int)EnumTableType.Wide)
                {
                    wideDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, wideXItemCode, this._curveY.ItemCode, EnumTableType.Wide, Color.Red, Color.Blue);
                    _SetCol(wideDatas.Count, HaveB_X);
                    _setRow();
                    DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, wideDatas);
                    DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._curveRowXIndex);
                    //_addBeforeTowRowToDataGridView(wideDatas);
                    //_addECPToDataGridView(this._curveRowXIndex);
                }
            }
            else if (this._curveX.ItemCode == PartCurveItemCode.TWY.ToString())
            {
                setButtonEnable(false);
                wideXItemCode = PartCurveItemCode.MWY.ToString();
                HaveB_X = true;
                if (this._curveY.OilTableTypeID == (int)EnumTableType.Narrow)
                {
                    narrowDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, this._curveX.ItemCode, this._curveY.ItemCode, EnumTableType.Narrow, Color.Red, Color.Blue);
                    wideDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, wideXItemCode, this._curveY.ItemCode, EnumTableType.Wide, Color.Red, Color.Blue);
                    mergeNarrWideDatas = MergeCurveParm(narrowDatas, wideDatas);

                    _SetCol(mergeNarrWideDatas.Count, HaveB_X);
                    _setRow();
                    DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, mergeNarrWideDatas);
                    DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._ECPRowIndex);
                    _addValueToDataGridViewLastTwoRow();
                }
                else if (this._curveY.OilTableTypeID == (int)EnumTableType.Wide)
                {
                    wideDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, wideXItemCode, this._curveY.ItemCode, EnumTableType.Wide, Color.Red, Color.Blue);
                    _SetCol(wideDatas.Count, HaveB_X);
                    _setRow();
                    DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, wideDatas);
                    DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._ECPRowIndex);
                    _addValueToDataGridViewLastTwoRow();
                }
            }
            else if (this._curveX.ItemCode == PartCurveItemCode.TVY.ToString())
            {
                setButtonEnable(false);
                HaveB_X = true;
                narrowDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, this._curveX.ItemCode, this._curveY.ItemCode, EnumTableType.Narrow, Color.Red, Color.Blue);
                _SetCol(narrowDatas.Count, HaveB_X);
                _setRow();
                DatabaseA.Curve.DataBaseACurve.addDataToDataGridViewDataBaseA(ref this.dataGridView, narrowDatas);
                DatabaseA.Curve.DataBaseACurve.addB_XListToDataGridView(ref this.dataGridView, FrmCurveAGlobal._ECPRowIndex);
                _addValueToDataGridViewLastTwoRow();
            }
            #endregion
        }
        /// <summary>
        /// 设置渣油曲线
        /// </summary>
        private void setRESIDUE(OilInfoEntity oilInfoA )
        {
            #region "渣油曲线"
            if (this._curveX.ItemCode == PartCurveItemCode.WY.ToString())//WY保存
            {
                setButtonEnable(true);
                //获得渣油表的A库的值
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> residueA_XYDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, this._curveX.ItemCode, this._curveY.ItemCode, EnumTableType.Residue, Color.Red, Color.Blue);
                _SetResidueCol(residueA_XYDatas.Count, true);//设置渣油曲线列头      
                _setResidueRow();
                _addResidueValueBeforeTowRowToDataGridView(residueA_XYDatas);//
                _addResidueECPToDataGridView();
                _addResidueValueToDataGridViewCurveXRow(ECP_WYDic ); //向B库赋值
            }
            else if (this._curveX.ItemCode == PartCurveItemCode.VY.ToString())//TVY显示
            {
                setButtonEnable(false);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> residueA_XYDatas = DataBaseACurve.getDictionary(oilInfoA, this._typeCode, this._curveX.ItemCode, this._curveY.ItemCode, EnumTableType.Residue, Color.Red, Color.Blue);
                _SetResidueCol(residueA_XYDatas.Count, true);//设置渣油曲线列头
                _setResidueRow();
                _addResidueValueBeforeTowRowToDataGridView(residueA_XYDatas);//
                _addResidueECPToDataGridView();
                _addValueToDataGridViewLastTwoRow();
            }
            #endregion
        }
        /// <summary>
        /// GC计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            if (this.Created)
                this._HaveSave = false;

            #region "输入条件"
            List<string> itemCodeList = new List<string>();
            itemCodeList.Add("MW"); itemCodeList.Add("D20"); itemCodeList.Add("PAN");
            itemCodeList.Add("PAO"); itemCodeList.Add("NAH"); itemCodeList.Add("ARM");
            itemCodeList.Add("RNC"); itemCodeList.Add("MON"); itemCodeList.Add("RVP"); itemCodeList.Add("ARP");
            
            this._oil = this._frmOilDataA.getData();
            List<OilDataEntity> gcOilDataList = this._oil.OilDatas.Where(o=>o.calData != string.Empty  && o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            CurveEntity ECP_TWYCurve = this._oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TWY").FirstOrDefault();
            #endregion 
            OilTools oilTool = new OilTools();
            
            if (this._typeCode == CurveTypeCode.DISTILLATE && this._curveX.ItemCode == "ECP" && itemCodeList.Contains(this._curveY.ItemCode))
            {
                refreshCurve();
                Dictionary<float, float> DIC = DatabaseA.Curve.DataBaseACurve.getGCInterpolationDIC(gcOilDataList, ECP_TWYCurve, this.B_XList, this._curveY.ItemCode);
                foreach (float key in DIC.Keys)
                {
                    int colIndex = FindB_XColIndex(key);
                    if (colIndex != -1)
                    {
                        this.dataGridView.Rows[this._lastRowNum].Cells[colIndex].Value = DIC[key];
                    }                                    
                }                      
            }

            drawCurve();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.Created)
                this._HaveSave = false;
        }
        /// <summary>
        /// 此点参与计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 剪贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZedExpand_Calculate(sender ,e);
        }
        /// <summary>
        /// 此点不参与计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZedExpand_NotCalCulate(sender, e); 
        }
        /// <summary>
        /// 快捷键关联
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 1 && e.ColumnIndex >=3)
                this.dataGridView.ContextMenuStrip = this.contextMenuStrip1;
            else
                this.dataGridView.ContextMenuStrip = null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraph1_KeyDown(object sender, KeyEventArgs e)
        {
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCurveA_MouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();
        }

        private void FrmCurveA_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.PageUp:
                        case Keys.Up:
                            if (this.cmbCurve.ComboBox.SelectedIndex - 1 >= 0)
                            {
                                this.cmbCurve.ComboBox.SelectedIndex = this.cmbCurve.ComboBox.SelectedIndex - 1;
                            }
                            break;
                        case Keys.PageDown:
                        case Keys.Down:
                            if (this.cmbCurve.ComboBox.SelectedIndex + 1 < this.cmbCurve.ComboBox.Items.Count)
                            {
                                this.cmbCurve.ComboBox.SelectedIndex = this.cmbCurve.ComboBox.SelectedIndex + 1;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        
    }
    ///// <summary>
    ///// 传递相似原油
    ///// </summary>
    //public class oilBlist
    //{
    //    private static List<OilInfoBEntity> _oilBList = new List<OilInfoBEntity>();
    //    /// <summary>
    //    /// 参考原油数据集
    //    /// </summary>
    //    public static List<OilInfoBEntity> OilBList
    //    {
    //        get
    //        {
    //            if (_oilBList.Count > 10)
    //            {
    //                int count = _oilBList.Count - 10;//最大10条参考原油
    //                _oilBList.RemoveRange(9, count);
    //            }
    //            return _oilBList;
    //        }
    //        set
    //        {
    //            _oilBList = value;
    //        }
    //    }


    //    private static string _crudeIndex = string.Empty;

    //    /// <summary>
    //    /// 当前绘图的原油名称
    //    /// </summary>
    //    public static string CrudeIndex
    //    {
    //        get
    //        {                
    //            return _crudeIndex;
    //        }
    //        set
    //        {
    //            _crudeIndex = value;
    //        }
    //    }
    //}
}
