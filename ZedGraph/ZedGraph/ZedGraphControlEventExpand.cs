
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ZedGraph
{
    /// <summary>
    /// 类用来处理ZedGraphControlEvent.cs类中的一些扩展事件
     ///包括用户选择的区域填充，鼠标拖动等功能
    /// </summary>
    public class ZedGraphControlEventExpand
    {
        # region 私有变量
        private List<PointF> storeList;//存储用户选择的坐标区间
        private PointF store;//临时存储坐标区间
        private PointF pointResult;//保存最后的合格坐标区间
        private readonly string chooseTag = "line";//表示所选择矩形区域的矩形线的上下两条线
        private string xstr = "";
        private string ystr = "";
        private double start = 0;
        private double end = 0;
        private double range = 0;
        private PointPairList pointList;//存储当前已画曲线上的坐标点,数据库B库的点
        private PointPairList _ApointList;// 存储当前已画去想上的A库坐标点
        private PointPairList mergeABpointList;//存储合并A库点和B库点以后的点的集合
        private PointPairList _pointListNotuse;//存储A库不参与运算的点的集合
        private bool flag = false;//标志当前点是否被选中
        private bool zoomChoose = false;//判断是否需要放大
        private int index;//当前选中B库点的下标值
        /// <summary>
        /// 选中A库的下标值，初始化为-1
        /// </summary>
        private int _aCellindex = -1;//当前选中A库点的下标值
        private bool doubleClick = false;//修复一个点击以后，然后双击窗体选中区域的bug
        private bool _isMoving = false;//判断是否需要移动点的功能,满足拖动需求
        private bool _zoom = true;//判断是否在松开鼠标的时候放大
        private bool _isOpenA = false;//判断当前是否是打开A库
        private bool _isOpenB = false;//判断当前是否是打开B库
        private bool _isOpenDIYDrawFrm = false;//判断当前是否是打开定制绘图
        private bool isRefresh = false;//判断是否对当前图像刷新
        private int _decNumber = 6; //设置List类型的位数，默认为6位
        private string _cellTag = "true";//保存单元格的tag内容,用于判断当前点是否可以参与计算
        private float _width = 794;    //生成图片默认宽度
        private float _height = 1123;      //生成图片默认高度
        private string _savePath ="temp.jpg"; //默认图片的保存路径与名称
        private bool _isPicAdjust = false;      //是否调整保存的图片
        public ZedGraphControl zg1;
        public event EventHandler SelectChange;//选择区间变化事件
        public event EventHandler ResetB;//B库清空事件
        public event EventHandler RefOil; //参考原油事件
        public event EventHandler NotCalCulate; //不参与计算的点事件
        public event EventHandler CalCulate;//此点参与计算事件
        public event EventHandler Refresh;//刷新图像事件
        public event EventHandler CopyCells;//复制表格事件
        public event EventHandler TotalValue;//累计值事件
        public event EventHandler GcCal;        //Gc计算事件
        #endregion

        #region 构造函数
        public ZedGraphControlEventExpand()
        {
            storeList = new List<PointF>();
            store = new PointF();
            pointResult = new PointF();
            pointList = new PointPairList();
            _ApointList = new PointPairList();
            mergeABpointList = new PointPairList();
            _pointListNotuse = new PointPairList();
        }
        #endregion

        #region 公有属性
        /// <summary>
        /// 松开鼠标的时候是否放大图像的标记位
        /// </summary>
        public bool Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }
        /// <summary>
        /// A库点的下标
        /// </summary>
        public int AcellIndex
        {
            get { return _aCellindex; }
            set { _aCellindex = value; }
        }
        /// <summary>
        /// 数据坐标点的位数，默认为6位
        /// </summary>
        public int DecNumber
        {
            get { return _decNumber; }
            set { _decNumber = value; }
        }
        /// <summary>
        /// 鼠标是否可以拖动点
        /// </summary>
        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }
        /// <summary>
        /// 选中B库点的下标
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        /// <summary>
        /// 是否选择放大所选的区域
        /// </summary>
        public bool ZoomChoose
        {
            get { return zoomChoose; }
            set { zoomChoose = value; }
        }
        /// <summary>
        /// 保存最后满足条件的区间
        /// </summary>
        public PointF PointResult
        {
            get { return pointResult; }
            set { pointResult = value; }
        }
        /// <summary>
        /// 存储当前已画曲线上的B库点的坐标集合
        /// </summary>
        public PointPairList PointList
        {
            get { return pointList; }
            set { pointList = value; }
        }
        /// <summary>
        /// 存取当前已画曲线A库上点的坐标集合
        /// </summary>
        public PointPairList ApointList
        {
            get { return _ApointList; }
            set { _ApointList = value; }
        }
        /// <summary>
        /// 获取A库B库点上的集合
        /// </summary>
        public PointPairList MergeABpointList
        {
            get { return mergeABpointList; }
            set { mergeABpointList = value; }
        }
        /// <summary>
        /// A库点不参与运算的点集合
        /// </summary>
        public PointPairList PointListNotUse
        {
            get { return _pointListNotuse; }
            set { _pointListNotuse = value; }
        }
        /// <summary>
        /// 存储用户选择的坐标区间
        /// </summary>
        public List<PointF> StoreList
        {
            get { return storeList; }
            set 
            { 
            reset(zg1);//清除掉以前的选项
            storeList = value;
            inputRang(storeList,zg1);
            
            }
        }
        /// <summary>
        /// 临时存储坐标区间
        /// </summary>
        public PointF Store
        {
            get { return store; }
            set { store = value; }
        }
        /// <summary>
        /// 鼠标移动到控件上的X坐标点
        /// </summary>
        public string Xstr
        {
            get { return xstr; }
            set { xstr = value; }
        }
        /// <summary>
        /// 鼠标移动到控件上的Y坐标点
        /// </summary>
        public string Ystr
        {
            get { return ystr; }
            set { ystr = value; }
        }
        /// <summary>
        /// 鼠标点击左键时x坐标
        /// </summary>
        public double Start
        {
            get { return start; }
            set { start = value; }
        }
        /// <summary>
        /// 鼠标点击左键释放时的坐标
        /// </summary>
        public double End
        {
            get { return end; }
            set { end = value; }
        }
        /// <summary>
        /// 鼠标拖动选择的区间
        /// </summary>
        public double Range
        {
            get { return range; }
            set { range = value; }
        }
        /// <summary>
        /// 是否打开B库
        /// </summary>
        public bool IsopenB
        {
            get { return _isOpenB; }
            set { _isOpenB = value; }
        }
        /// <summary>
        /// 是否打开A库
        /// </summary>
        public bool IsopenA
        {
            get { return _isOpenA; }
            set { _isOpenA = value; }
        }
        /// <summary>
        /// 是否打开定制绘图
        /// </summary>
        public bool IsopenDIYDrawFrm
        {
            get { return _isOpenDIYDrawFrm; }
            set { _isOpenDIYDrawFrm = value; }
        }
        /// <summary>
        /// A库单元格的标签，判断此点是否参与计算
        /// </summary>
        public string CellTag
        {
            get { return _cellTag; }
            set { _cellTag = value; }
        }
        /// <summary>
        /// 设置图片宽度
        /// </summary>
        public float picWidth
        {
            get { return _width; }
            set { _width = value; }
        }
        /// <summary>
        /// 设置图片的高度
        /// </summary>
        public float picHeitht
        {
            get { return _height; }
            set { _height = value; }
        }
        /// <summary>
        /// 设置图片临时存储路径
        /// </summary>
        public string SavePath
        {
            get { return _savePath; }
            set { _savePath = value; }
        }
        /// <summary>
        /// 是否调整图片，默认为false
        /// </summary>
        public bool IsPicAdjust
        {
            get { return _isPicAdjust; }
            set { _isPicAdjust = value; }
        }
        #endregion

        #region 鼠标按下事件
        /// <summary>
       /// 鼠标按下事件，记录当前坐标以及是否是可以拖动点
       /// </summary>
       /// <param name="zgc"></param>
       /// <param name="mousePt"></param>
        public void Down_Expand(ZedGraphControl zgc,Point mousePt,bool zedZoom,bool isMoving)
        {
            if (!zedZoom)
            {
                GraphPane pane = zgc.MasterPane.FindPane(mousePt);
                double x, x2, y, y2;
                PointF tempPoint = new PointF();

                pane.ReverseTransform(mousePt, out x, out y, out x2, out y2);
                this.Start = x;
                store.X = (float)x;//获取鼠标事件down时候的坐标,注意由double向float，精度降低
                if (isMoving)
                {
                    if (zgc.Cursor == Cursors.Hand)
                    {
                        tempPoint.X = (float)x;
                        tempPoint.Y = (float)y2;//存储当前的坐标
                        this.PointResult = tempPoint;//存储当前的坐标点
                        flag = true;
                    }
                }
                this.isRefresh = false;//初始化刷新值
                this.doubleClick = !this.doubleClick;

            }
        }
        #endregion

        #region 鼠标移动处理事件
        /// <summary>
        /// 鼠标移动处理事件
        /// </summary>
        /// <param name="zgc"></param>
        /// <param name="mousePt"></param>
        public void Move_Expand(ZedGraphControl zgc, Point mousePt, bool zedZoom, bool isMoving)
        {
            if (!zedZoom)
            {
                GraphPane pane = zgc.MasterPane.FindPane(mousePt);
                if (pane != null)
                {
                    double x, x2, y, y2;
                    pane.ReverseTransform(mousePt, out x, out y, out x2, out y2);
                    this.Xstr = x.ToString();//获取移动过程中的X坐标
                    this.Ystr = y2.ToString();//获取移动过程中的Y坐标

                    PointPairList tempList = this.PointList;
                    PointF temp = new PointF();
                    int decNumber = 0;          //匹配的时候相似的位数
                    /*判断当前坐标是否在划线的几个坐标之内
                        X轴的坐标转化为整数比较
                        * Y轴的坐标转化为小数点后一位，这样比较容易选中
                        * 添加代码实现判断点是否在坐标点上
                        */
                    if (isMoving)
                    {

                        #region "A库曲线画出蓝色的描述点"
                        PointPairList AList = this.ApointList;
                        for (int i = 0; i < AList.Count; i++)
                        {
                            if (Math.Round(AList[i].X, decNumber) == Math.Round(double.Parse(Xstr), decNumber) && Math.Round(AList[i].Y, decNumber) == Math.Round(double.Parse(Ystr), decNumber))
                            {
                                PointF point = new PointF();
                                point.X = (float)AList[i].X;
                                point.Y = (float)AList[i].Y;
                                DrawPoint(point, "bluePoint", Color.Blue, zgc);
                                AcellIndex = i;     //赋值当前选中点的下标
                                break;
                            }
                        }
                        #endregion

                        #region "B库曲线画出红色的描述点"
                        for (int m = 0; m < tempList.Count; m++)
                        {
                            if (Math.Round(tempList[m].X, decNumber) == Math.Round(double.Parse(Xstr), decNumber))
                            {
                                temp.X = (float)tempList[m].X;
                                temp.Y = (float)tempList[m].Y;
                                zgc.Cursor = Cursors.Hand;
                                this.index = m;//将当前的下标值赋给index
                                break;
                            }
                        }

                        PointPairList Blist = this.PointList;
                        if (true == flag)
                        {
                            for (int n = 0; n < Blist.Count; n++)
                            {
                                if (Math.Round(Blist[n].X, 0) == Math.Round(this.PointResult.X, 0))
                                {
                                    #region "B库曲线联动"
                                    PointPair CurrentPoint = new PointPair(Blist[n].X, Blist[n].Y);
                                    Blist[n].Y = Math.Round(double.Parse(this.Ystr), DecNumber);//x坐标不变而Y左边随着鼠标移动而移动
                                    PointF point = new PointF();
                                    point.X = (float)Blist[n].X;
                                    point.Y = (float)Blist[n].Y;
                                    DrawPoint(point, "choosePoint", Color.Red, zgc);//移动过程中需要重新的绘制鼠标事件选择的点
                                    Zoom = false;
                                    this.isRefresh = true;
                                    #endregion
                                    #region "AB合并曲线的数据联动"
                                    int Abindex = mergeABpointList.IndexOf(CurrentPoint);
                                    if (Abindex > -1)
                                    {
                                        mergeABpointList[Abindex].Y = Math.Round(double.Parse(this.Ystr), DecNumber);
                                        break;
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        zgc.Refresh();
                    }//endif(isMoving)
                }//end if(pane)
                //}//endif(!zedZoom)
                zgc.Refresh();
            }
        }
        #endregion

        #region 鼠标松开事件
        /// <summary>
        /// 鼠标松开事件，重绘图形
        /// </summary>
        /// <param name="zgc"></param>
        /// <param name="mousePt">鼠标松开的时候的坐标</param>
        public void Up_Expand(ZedGraphControl zgc, Point mousePt, bool zedZoom)
        {
            GraphPane pane = zgc.MasterPane.FindPane(mousePt);
            if (!zedZoom)
            {
                if (null != pane)
                {
                    double x, x2, y, y2;
                    pane.ReverseTransform(mousePt, out x, out y, out x2, out y2);
                    this.End = x;
                    store.Y = (float)x;
                    /*判断起始坐标保证y>x
                     *在鼠标右键离开的时候重新绘制图形并刷新当前的图像
                     *如果没有执行拖动功能的话则进行添加绘图
                     */
                    if (!flag && doubleClick)
                    {
                        /*如果超出坐标最大最小值时进行处理*/
                        PointF exChange = new PointF();
                        exChange = store;
                        if (store.Y < pane.XAxis.Scale.Min)
                            exChange.Y = (float)pane.XAxis.Scale.Min;
                        if (store.Y > pane.XAxis.Scale.Max)
                            exChange.Y = (float)pane.XAxis.Scale.Max;
                        if (exChange.X > exChange.Y)
                        {
                            float temp = exChange.X;
                            exChange.X = exChange.Y;
                            exChange.Y = temp;
                            store = exChange;
                        }
                        if (ZoomChoose)
                        {
                            storeList.Add(store);
                            inputRang(StoreList, zgc);//合并并且打印区间
                        }
                    }
                    this.Range = this.End - this.Start;
                    flag = false;
                    zgc.Refresh();
                }
                doubleClick = false;
            }

            #region 如果可以点移动的话则在选择之后区间之后清除所选区域
            if (IsMoving)
            {
                //reset(zgc);
                resetChooseArea(zgc);
            }
            #endregion

            if (this.isRefresh)
            {
                setDefault(pane);
            }
                //fireRefresh();//触发刷新事件
            zgc.Refresh();
        }
        #endregion
        #region 鼠标滚轮缩放事件
        /// <summary>
        /// 自定义的鼠标滚轮事件，默认滚轮的聚焦中心为坐标中心
        /// 如果是放大事件，则设置Delta的值为120，
        /// 如果是缩小事件，则设置Delta的值为-120
        /// </summary>
        /// <param name="isExpand">是否是放大事件</param>
        public void MouseWheel(bool isExpand)
        {
            double delta = isExpand ? 120 : -120;//判断是放大还是缩小事件
            //double xMax = zg1.GraphPane.XAxis.Scale.Max;
            //double xMin = zg1.GraphPane.XAxis.Scale.Min;
            //double yMax = zg1.GraphPane.YAxis.Scale.Max;
            //double yMin = zg1.GraphPane.YAxis.Scale.Min;
            ////float x  = zg1.Location.X
            //float xtemp = (float)(xMax - xMin) / 2;
            //float ytemp = (float)(yMax - yMin) / 2;

            float x = zg1.Location.X + 100;
            float y =zg1.Location.Y +100;
            zg1.MouseExpand(new PointF(x, y), delta);//497,125
        }
        #endregion
        #region 公有方法
        /// <summary>
        /// 方法用于将当前图像恢复到默认值状态，如果坐标轴传入为空，则有一个默认的pane调用
        /// </summary>
        /// <param name="pane">鼠标选择的面板</param>
        public void setDefault(GraphPane pane)
        {
            if (null != pane)
            {
                zg1.RestoreScale(pane);
            }
            else
            {
                Point _menuClick = new Point(120,324);
                GraphPane xpane = zg1.MasterPane.FindPane(_menuClick);
                if (null != xpane)
                    zg1.RestoreScale(xpane);
            }
        }
        /// <summary>
        /// 方法用于清除鼠标选择的所有区域
        /// </summary>
        /// <param name="zgc"></param>
        public void reset(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            if (myPane.CurveList.Count > 1)
            {
                for (int i = 0; i < myPane.CurveList.Count; i++)
                {
                    if (myPane.CurveList[i].Tag !=null)
                    {
                        myPane.CurveList.Remove(myPane.CurveList[i]);

                        i--;
                    }
                }
                //int a = 1;
                //a = myPane.CurveList.IndexOf(label);
                //myPane.CurveList.RemoveRange(1, myPane.CurveList.Count - 1);//清除原来已经画上去的曲线
                zgc.Refresh();
            }
            storeList = new List<PointF>();//从新new一个对象，然后清除原来的数据
        }
        /// <summary>
        /// 将特定的曲线保留而去掉其他的曲线
        /// </summary>
        /// <param name="zgc"></param>zed控件的对象
        /// <param name="label"></param>保留的曲线的名称
        public void removeCurve(ZedGraphControl zgc, string tag)
        {
            GraphPane myPane = zgc.GraphPane;
            if (myPane.CurveList.Count > 0)
            {
                for (int i = 0; i < myPane.CurveList.Count; i++)
                {
                    if (myPane.CurveList[i].Tag !=null && myPane.CurveList[i].Tag.ToString().Equals(tag))
                    {
                        myPane.CurveList.Remove(myPane.CurveList[i]);
                        i--;
                    }
                }
            }
            zgc.Refresh();
        }

        /// <summary>
        /// 画出X来
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zg1"></param>
        public void DrawPointNotUse(double x,double y,ZedGraphControl zg1)
        {
            if (double.IsNaN(x) || double.IsNaN(y))
                return;

            PointPairList pointList = new PointPairList();
            pointList.Add(x,y);
            LineItem curve = this.zg1.GraphPane.AddCurve(null, pointList, Color.Red, SymbolType.XCross);
            curve.Symbol.Size = 10;
            curve.Tag = Math.Round(x,0).ToString();
            this.zg1.Refresh();
        }

        /// <summary>
        /// 清除未使用点的X号
        /// </summary>
        /// <param name="x">坐标X的值</param>
        /// <param name="y">坐标Y的值</param>
        public void ClearPointNotUse(double x,double y,ZedGraphControl zg1)
        {
            if (0 == x || 0 == y)
                return;

            string Standardtag = Math.Round(x,0).ToString();//判断的标准Tag值

            GraphPane myPane = zg1.GraphPane;
            if (myPane.CurveList.Count > 1)
            {
                int index = myPane.CurveList.IndexOfTag(Standardtag);

                if(index >-1)
                myPane.CurveList.RemoveAt(index);

            }
            zg1.Refresh();
        }
        /// <summary>
        /// 在ZedGraph控件上显示的特定的点
        /// </summary>
        /// <param name="point"></param>要显示的点的坐标
        /// <param name="zg1"></param>
        public void DrawPoint(PointF point,string pointName,Color color, ZedGraphControl zg1)
        {
            zg1.ZedExpand.removeCurve(zg1, pointName);//将上一条曲线删除掉
            /*将所选的点绘制打图形上去*/
            GraphPane myPane = zg1.GraphPane;
            PointPairList myPoint = new PointPairList();
            myPoint.Add(point.X, point.Y);
            LineItem myPointCurve = myPane.AddCurve(null, myPoint, color, SymbolType.Circle);
            myPointCurve.Symbol.Fill = new Fill(color);
            myPointCurve.Tag = pointName;
            zg1.Refresh();
        }
        /// <summary>
        /// 得到当前坐标在曲线上的对应坐标
        /// </summary>
        /// <param name="list">曲线坐标集合</param>
        /// <returns>当前点在曲线上的坐标</returns>
        public int getCurrentPosition(PointPairList list)
        {
            int positon = -1;
            int decNumber = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (Math.Round(list[i].X, decNumber) == Math.Round(double.Parse(Xstr), decNumber))
                {
                    positon = i;     //赋值当前选中点的下标
                    break;
                }
            }
            return positon;
        }
        /// <summary>
        /// 判断当前鼠标滑过的区域是否选中A库的点
        /// </summary>
        /// <returns></returns>
        //public void isChooseAPoint()
        //{
        //    PointPairList AList = this.ApointList;
        //    int decNumber = 0;
        //    for (int i = 0; i < AList.Count; i++)
        //    {
        //        if (Math.Round(AList[i].X, decNumber) == Math.Round(double.Parse(Xstr), decNumber) && Math.Round(AList[i].Y, decNumber) == Math.Round(double.Parse(Ystr), decNumber))
        //        {
        //            AcellIndex = i;//将选中点的下标
        //            break;
        //        }
        //    }
        //}
        /// <summary>
        /// 根据下标删除当前的点
        /// </summary>
        /// <param name="index"></param>
        public void resetB()
        {
            //this.PointList.RemoveAt(Index);
            //this.removeCurve(zg1, "choosePoint");
            fireResetB();
        }
        /// <summary>
        /// 参考原油事件
        /// </summary>
        public void refOil()
        {
            fireRefOil();
        }
        /// <summary>
        /// 复制表格事件
        /// </summary>
        public void copyCells()
        {
            fireCopycells();
        }
        /// <summary>
        /// 此点不参与计算事件
        /// </summary>
        public void notCalculate()
        {
            fireNotCalCulate();
        }
        /// <summary>
        /// 此点参与计算事件
        /// </summary>
        public void calCulate()
        {
            fireCalCulate();
        }
        /// <summary>
        /// 判断此点是否参与计算
        /// </summary>
        /// <returns></returns>
        public bool isNotInCalculate()
        {
            PointPairList AList = this.PointListNotUse;
            if (AcellIndex > -1)
                return !PointListNotUse.Contains(ApointList[AcellIndex]);
            else
                return true;
            //int decNumber = 0;
            //for (int i = 0; i < AList.Count; i++)
            //{
            //    if (Math.Round(AList[i].X, decNumber) == Math.Round(double.Parse(Xstr), decNumber))
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }
        /// <summary>
        /// 累计值
        /// </summary>
        public void totalValue()
        {
            fireTotalValue();
        }
        /// <summary>
        /// GC计算
        /// </summary>
        public void gcCal()
        {
            fireGcCal();
        }
        #endregion
        #region 私有方法

        /// <summary>
        /// 方法清除用户在拖动点时候所选择的矩形区域
        /// </summary>
        /// <param name="zgc"></param>
        private void resetChooseArea(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            if (myPane.CurveList.Count > 1)
            {
                for (int i = 0; i < myPane.CurveList.Count; i++)
                {
                    if (myPane.CurveList[i].Tag != null && myPane.CurveList[i].Tag.ToString().Equals(chooseTag))
                    {
                        myPane.CurveList.Remove(myPane.CurveList[i]);
                        i--;
                    }
                    }
                zg1.Refresh();
                }
            storeList = new List<PointF>();
        }

        /// <summary>
        /// 画出指定的区间
        /// </summary>
        /// <param name="start"></param>绘图开始的位置
        /// <param name="end"></param>绘图结束的位置
        /// <param name="zgc"></param>控件名称
        private void rangePaint(double start, double end, ZedGraphControl zgc)
        {
            /*如果前边的值大于后边的值则交换两者的值*/
            if (start > end)
            {
                double temp = start;
                start = end;
                end = temp;
            }
            if (null != zgc)
            {
                GraphPane myPane = zgc.GraphPane;
                PointPairList listDown = new PointPairList();
                for (double x = start; x <= end; x++)
                {
                    double y = 0;
                    y = 0 - myPane.YAxis.Scale.Max;
                    listDown.Add(x, y);
                }
                LineItem myDownCurve = myPane.AddCurve(null, listDown, Color.Blue, SymbolType.None);
                myDownCurve.Line.Fill = new Fill(Color.Yellow);
                myDownCurve.Symbol.Fill = new Fill(Color.LightGoldenrodYellow);
                myDownCurve.Tag = chooseTag;
                PointPairList listUp = new PointPairList();
                for (double x = start; x <= end; x++)
                {
                    double y = 0;
                    y = myPane.YAxis.Scale.Max;
                    //double y = -1.2;
                    listUp.Add(x, y);
                }
                LineItem myUpCurve = myPane.AddCurve(null, listUp, Color.Blue, SymbolType.None);
                myUpCurve.Line.Fill = new Fill(Color.Yellow);
                // Make the symbols opaque  by filling them with white 
                myUpCurve.Symbol.Fill = new Fill(Color.LightGoldenrodYellow);
                myUpCurve.Tag = chooseTag;
                zgc.Refresh();//刷新当前的图像
            }

        }

        /// <summary>
        /// 双重循环的合并算法，实现了任意多个区间的合并
        /// 思路：外循环，从第一个到最后一个
        /// 内循环，找到符合合并条件的选项，将合并后的数据保存在data[i-1]中，然后删除已经被合并的
        /// 数据，同时将内循环的m初始化0，从新开始下一轮的循环，m!=i-1是为了限定在相等的时候不要
        /// 删除数据
        /// </summary>
        /// <param name="data"></param>
        private void Merge(List<PointF> data)
        {
            for (int i = 1; i < data.Count; i++)
            {
                for (int m = i; m < data.Count; m++)
                {
                    if (data[m].X <= data[i - 1].Y && data[m].Y >= data[i - 1].X && m != i - 1)
                    {
                        PointF tempPoint = new PointF();
                        tempPoint.X = data[i - 1].X > data[m].X ? data[m].X : data[i - 1].X;
                        tempPoint.Y = data[i - 1].Y > data[m].Y ? data[i - 1].Y : data[m].Y;
                        data[i - 1] = tempPoint;//结构体是值传递，要修改只能通过这种方式
                        data.Remove(data[m]);
                        m = 0;//从头开始此次循环合并
                    }
                }
            }
        }
        /// <summary>
        /// 方法用来执行打印出客户选择的区间范围
        /// </summary>
        /// <param name="data"></param>
        /// <param name="zgc"></param>
        private void inputRang(List<PointF> data, ZedGraphControl zgc)
        {
            Merge(data);
            for (int i = 0; i < data.Count; i++)
            {
                rangePaint(data[i].X, data[i].Y, zgc);
            }
            fireSelectChange();//触发事件
        }
        #endregion
        #region 自定义事件函数
        /// <summary>
        /// 用户选择区域变化，然后触发事件
        /// </summary>
        public void fireSelectChange()
        {
            if (this.SelectChange != null)
                this.SelectChange(this, null);
        }

        /// <summary>
        /// 用户右键点击B库清空时触发该事件
        /// </summary>
        public void fireResetB()
        {
            if (this.ResetB != null)
                this.ResetB(this, null);
        }
        /// <summary>
        /// 触发复制表格事件
        /// </summary>
        public void fireCopycells()
        {
            if (this.CopyCells != null)
                this.CopyCells(this, null);
        }
        /// <summary>
        /// 点不参与计算
        /// </summary>
        public void fireNotCalCulate()
        {
            if (this.NotCalCulate != null)
                this.NotCalCulate(this, null);
        }
        /// <summary>
        /// 点参与计算
        /// </summary>
        public void fireCalCulate()
        {
            if (this.CalCulate != null)
                this.CalCulate(this, null);
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void fireRefresh()
        {
            if (this.Refresh != null)
                this.Refresh(this, null);
        }
        /// <summary>
        /// 参考原油
        /// </summary>
        public void fireRefOil()
        {
            if (this.RefOil != null)
                this.RefOil(this, null);
        }
        /// <summary>
        /// 累计值
        /// </summary>
        public void fireTotalValue()
        {
            if (this.TotalValue != null)
                this.TotalValue(this, null);
        }
        /// <summary>
        /// GC计算
        /// </summary>
        public void fireGcCal()
        {
            if (this.GcCal != null)
                this.GcCal(this, null);
        }
        #endregion
    }
}
