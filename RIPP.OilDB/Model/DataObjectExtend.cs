using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using RIPP.OilDB.Data;
using System.Drawing;
using RIPP.OilDB.UI.GridOil.V2.Model;


namespace RIPP.OilDB.Model
{
    public partial class OilInfoEntity : IOilInfoEntity<OilDataEntity>
    {
        private List<RemarkEntity> _RemarkList = null;//批注集合
        private List<OilDataEntity> _OilDatas = null;
        private List<OilTableTypeEntity> _OilTableTypes = null;
        private List<OilTableColEntity> _OilTableCols = null;
        private List<OilTableRowEntity> _OilTableRows = null;

        private OilTableColBll _colCache = new OilTableColBll();
        private OilTableRowBll _rowCache = new OilTableRowBll();
        private OilTableTypeBll _tableCache = new OilTableTypeBll();
        
        /// <summary>
        /// 原油的批注信息集合
        /// </summary>
        public List<RemarkEntity> RemarkList
        {
            get
            {
                if (this._RemarkList != null)
                    return this._RemarkList;
                this._RemarkList = new RemarkAccess().Get(string.Format(" oilInfoID = {0}", this._ID));
                return this._RemarkList;
            }
        }
        public List<OilDataEntity> OilDatas
        {
            get
            {
                if (this._OilDatas != null)
                    return this._OilDatas;
                this._OilDatas = new OilDataAccess().Get(string.Format(" oilInfoID = {0}", this._ID));
                return this._OilDatas;
            }
        }

        public List<OilTableTypeEntity> OilTableTypes
        {
            get
            {
                //删除以下代码可加快速度
                if (this._OilTableTypes != null)
                    return this._OilTableTypes;
                var colID = this.OilDatas.Select(d => d.oilTableColID).Distinct();
                var tableID = this.OilTableCols.Where(d => colID.Contains(d.ID)).Select(t => t.oilTableTypeID).Distinct().ToArray();

                this._OilTableTypes = this._tableCache.Where(t => tableID.Contains(t.ID)).ToList();
                return this._OilTableTypes;
            }
        }

        public List<OilTableColEntity> OilTableCols
        {
            get
            {
                //删除以下代码可加快速度
                if (this._OilTableCols != null)
                    return this._OilTableCols;

                var g = this.OilDatas.Select(d => d.oilTableColID).Distinct().ToArray();
                this._OilTableCols = this._colCache.Where(c => g.Contains(c.ID)).ToList();
                return this._OilTableCols;
            }
        }

        public List<OilTableRowEntity> OilTableRows
        {
            get
            {
                //删除以下代码可加快速度
                if (this._OilTableRows != null)
                    return this._OilTableRows;

                var g = this.OilDatas.Select(d => d.oilTableRowID).Distinct().ToArray();
                this._OilTableRows = this._rowCache.Where(r => g.Contains(r.ID)).ToList();
                return this._OilTableRows;
            }
        }
    }

    public partial class OilInfoBEntity : IOilInfoEntity<OilDataBEntity>
    {
        private List<OilDataBEntity> _OilDatas = null;//未切割数据
        private List<OilDataSearchEntity> _OilDataSearchs = null;//切割数据
        private List<OilTableTypeEntity> _OilTableTypes = null;
        private List<OilTableColEntity> _OilTableCols = null;
        private List<OilTableRowEntity> _OilTableRows = null;

        private OilTableColBll _colCache = new OilTableColBll();
        private OilTableRowBll _rowCache = new OilTableRowBll();
        private OilTableTypeBll _tableCache = new OilTableTypeBll();

        public List<OilDataBEntity> OilDatas
        {
            get
            {
                if (this._OilDatas != null)
                    return this._OilDatas;
                this._OilDatas = new OilDataBAccess().Get(string.Format(" oilInfoID = {0}", this._ID));
                return this._OilDatas;
            }
        }
        /// <summary>
        /// 获取切割数据
        /// </summary>
        public List<OilDataSearchEntity> OilDataSearchs
        {
            get
            {
                if (this._OilDataSearchs != null)
                    return this._OilDataSearchs;
                this._OilDataSearchs = new OilDataSearchAccess().Get(string.Format(" oilInfoID = {0}", this._ID));
                return this._OilDataSearchs;
            }
        }

        public List<OilTableTypeEntity> OilTableTypes
        {
            get
            {
                //删除以下代码可加快速度
                if (this._OilTableTypes != null)
                    return this._OilTableTypes;
                var colID = this.OilDatas.Select(d => d.oilTableColID).Distinct();
                var tableID = this.OilTableCols.Where(d => colID.Contains(d.ID)).Select(t => t.oilTableTypeID).Distinct().ToArray();

                this._OilTableTypes = this._tableCache.Where(t => tableID.Contains(t.ID)).ToList();
                return this._OilTableTypes;
            }
        }

        public List<OilTableColEntity> OilTableCols
        {
            get
            {
                //删除以下代码可加快速度
                if (this._OilTableCols != null)
                    return this._OilTableCols;

                var g = this.OilDatas.Select(d => d.oilTableColID).Distinct().ToArray();
                this._OilTableCols = this._colCache.Where(c => g.Contains(c.ID)).ToList();
                return this._OilTableCols;
            }
        }

        public List<OilTableRowEntity> OilTableRows
        {
            get
            {
                //删除以下代码可加快速度
                if (this._OilTableRows != null)
                    return this._OilTableRows;

                var g = this.OilDatas.Select(d => d.oilTableRowID).Distinct().ToArray();
                this._OilTableRows = this._rowCache.Where(r => g.Contains(r.ID)).ToList();
                return this._OilTableRows;
            }
        }
    }
    public partial class CrudeIndexIDBEntity  
    {        
        private List<OilDataSearchEntity> _OilDataSearchs = null;//切割数据
        
        /// <summary>
        /// 获取切割数据
        /// </summary>
        public List<OilDataSearchEntity> OilDataSearchs
        {
            get
            {
                if (this._OilDataSearchs != null)
                    return this._OilDataSearchs;
                this._OilDataSearchs = new OilDataSearchAccess().Get(string.Format(" oilInfoID = {0}", this._ID));
                return this._OilDataSearchs;
            }
        }
    }

    public partial class OilTableColEntity
    {
        private OilTableTypeEntity _OilTableType = null;
        private OilTableTypeBll _tableCache = new OilTableTypeBll();
        public int ColumnIndex { set; get; }
        public OilTableTypeEntity OilTableType
        {
            get
            {
                if (this._OilTableType != null)
                    return this._OilTableType;
                this._OilTableType = this._tableCache.Where(t => t.ID == this._oilTableTypeID).FirstOrDefault();
                return this._OilTableType;
            }
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            if (!(obj is OilTableColEntity))
                return false;
            var d = obj as OilTableColEntity;
            return this.ID.Equals(d.ID);
        }

         public static bool operator ==(OilTableColEntity one, OilTableColEntity two)
         {
             bool tag = object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null);
            if (tag )
                return true;
            tag = (object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return false;
            return one.ID.Equals(two.ID);
         }

         public static bool operator !=(OilTableColEntity one, OilTableColEntity two)
        {
            bool tag =!( object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag )
                return true;
            tag =! ((object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null)));
            if (tag)
                return false;
            return !one.ID.Equals(two.ID);
        }
    }

    public partial class OilTableRowEntity
    {
        private OilTableTypeEntity _OilTableType = null;
        private OilTableTypeBll _tableCache = new OilTableTypeBll();
        public int RowIndex 
        {
            set { }
            get { return this.itemOrder - 1; }
        }

        public OilTableTypeEntity OilTableType
        {
            get
            {
                if (this._OilTableType != null)
                    return this._OilTableType;
                this._OilTableType = this._tableCache.Where(t => t.ID == this._oilTableTypeID).FirstOrDefault();
                return this._OilTableType;
            }
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            if (!(obj is OilTableRowEntity))
                return false;
            var d = obj as OilTableRowEntity;
            return this.ID.Equals(d.ID);
        }

        public static bool operator ==(OilTableRowEntity one, OilTableRowEntity two)
        {
            bool tag = object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null);
            if (tag)
                return true;
            tag = (object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return false;
            return one.ID.Equals(two.ID);
        }

        public static bool operator !=(OilTableRowEntity one, OilTableRowEntity two)
        {
            bool tag = !(object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return true;
            tag = !((object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null)));
            if (tag)
                return false;
            return !one.ID.Equals(two.ID);
        }

    }
    /// <summary>
    /// TrendParmTableEntity实体类(TrendParmTable),保存趋势数据上下范围数据
    /// </summary>
    public partial class RangeParmTableEntity
    {
        private OilTableRowEntity _OilTableRow = null;
        private OilTableRowBll _tableRowCache = new OilTableRowBll();
        OilTableTypeComparisonTableAccess trendAccess = new OilTableTypeComparisonTableAccess();
        
        /// <summary>
        /// TrendParmTable中包括的行实体
        /// </summary>
        public OilTableRowEntity OilTableRow
        {
            get
            {
                if (this._OilTableRow == null)
                {
                    List<OilTableTypeComparisonTableEntity> trendTableTypeList = trendAccess.Get("1=1").ToList();
                    OilTableTypeComparisonTableEntity trendTableType = trendTableTypeList.Where(o => o.ID == this.OilTableTypeComparisonTableID).FirstOrDefault();

                    this._OilTableRow = this._tableRowCache.Where(t => t.oilTableTypeID == trendTableType.oilTableTypeID && t.itemCode == this.itemCode).FirstOrDefault();
                    if (this._OilTableRow == null)
                    {
                        int a = 0;
                    }
                }
                return this._OilTableRow;
            }
        }                
    }


    public partial class OilDataEntity : IOilDataEntity 
    {
        private OilTableColEntity _OilTableCol = null;
        private OilTableRowEntity _OilTableRow = null;
        private OilTableColBll _colCache = new OilTableColBll();
        private OilTableRowBll _rowCache = new OilTableRowBll();
        private OilTools oilTool = new OilTools();
        public int RowIndex { set; get; }
        public int ColumnIndex { set; get; }
        private string _labShowData = string.Empty;//实测值显示
        private string _calShowData = string.Empty;//校正值显示
        /// <summary>
        /// 实测值的显示数据
        /// </summary>
        public String labShowData
        {
            get
            {
                if (this._OilTableRow != null)
                    this._labShowData = oilTool.calDataDecLimit(this.labData, this._OilTableRow.decNumber, this._OilTableRow.valDigital);

                return this._labShowData;
            }
        }
        /// <summary>
        /// 校正值的显示数据
        /// </summary>
        public String calShowData
        {
            get 
            {
                if (OilTableRow != null)
                    this._calShowData = oilTool.calDataDecLimit(this._calData, OilTableRow.decNumber, OilTableRow.valDigital);

                return this._calShowData;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public OilTableColEntity OilTableCol
        {
            get
            {
                if (this._OilTableCol != null)
                    return this._OilTableCol;
                this._OilTableCol = this._colCache.Where(t => t.ID == this._oilTableColID).FirstOrDefault();
                return this._OilTableCol;
            }
            set { this._OilTableCol = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public OilTableRowEntity OilTableRow
        {
            get
            {
                if (this._OilTableRow != null)
                    return this._OilTableRow;
                this._OilTableRow = this._rowCache.Where(t => t.ID == this._oilTableRowID).FirstOrDefault();
                return this._OilTableRow;
            }
            set { this._OilTableRow = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int OilTableTypeID
        {
            get
            {
                if (this.OilTableCol != null)
                    return this.OilTableCol.oilTableTypeID;
                else
                    return 0;
            }
        }

    }

    public partial class OilDataBEntity : IOilDataEntity
    {
        private OilTableColEntity _OilTableCol = null;
        private OilTableRowEntity _OilTableRow = null;
        private OilTableColBll _colCache = new OilTableColBll();
        private OilTableRowBll _rowCache = new OilTableRowBll();
        private OilTools oilTool = new OilTools();
       // private string _labShowData = string.Empty;//实测值显示
        private string _calShowData = string.Empty;//校正值显示
        public int RowIndex { set; get; }
        public int ColumnIndex { set; get; }
       
        /// <summary>
        /// 校正值的显示数据
        /// </summary>
        public String calShowData
        {
            get
            {
                if (this._OilTableRow != null)
                    this._calShowData = oilTool.calDataDecLimit(this._calData, this._OilTableRow.decNumber, this._OilTableRow.valDigital);

                return this._calShowData;
            }
        }
        public OilTableColEntity OilTableCol
        {
            get
            {
                if (this._OilTableCol != null)
                    return this._OilTableCol;
                this._OilTableCol = this._colCache.Where(t => t.ID == this._oilTableColID).FirstOrDefault();
                return this._OilTableCol;
            }
            set { this._OilTableCol = value; }
        }

        public OilTableRowEntity OilTableRow
        {
            get
            {
                if (this._OilTableRow != null)
                    return this._OilTableRow;
                this._OilTableRow = this._rowCache.Where(t => t.ID == this._oilTableRowID).FirstOrDefault();
                return this._OilTableRow;
            }
            set { this._OilTableRow = value; }
        }

        public int OilTableTypeID
        {
            get
            {
                if (this.OilTableCol != null)
                    return this.OilTableCol.oilTableTypeID;
                else
                    return 0;
            }
        }

    }

    /// <summary>
    /// Cru文件的编码映射
    /// </summary>
    public partial class CruCodeMapEntity
    {
        private OilTableRowEntity _oilTableRow = null; //原油表的行

        /// <summary>
        /// 原油表的行
        /// </summary>
        public OilTableRowEntity oilTableRow
        {
            get
            {
                if (this._oilTableRow != null)
                    return this._oilTableRow;
                OilTableRowBll rowBll = new OilTableRowBll();
                _oilTableRow=rowBll[itemCode,(EnumTableType)oilTableTypeID];
                return _oilTableRow;
            }
        }
    }
    
    /// <summary>
    /// 性质曲线
    /// </summary>
    public partial class CurveEntity
    {
        List<CurveDataEntity> _curveDatas = null;           // 曲线上的点
        private Color _color = Color.Black;
        private int _splineLine = 0;//判断是否进行内插
        private double[] _x = null;
        private double[] _y = null;

        /// <summary>
        /// 一条曲线上的点
        /// </summary>
        public List<CurveDataEntity> curveDatas
        {
            get
            {
                if (this._curveDatas!=null )  //如果已经有值，直接返回值，否则从数据库中读取
                    return _curveDatas;
                CurveDataAccess acess = new CurveDataAccess();
                _curveDatas = acess.Get("curveID=" + this._ID);
                return _curveDatas;               
            }
            set { _curveDatas = value; }
        }

        public Color Color
        {
            get { return this._color; }
            set { this._color = value; }
        }
        /// <summary>
        /// 判断是否内插
        /// </summary>
        public int  splineLine
        {
            get
            {               
                CurveSubTypeAccess acess = new CurveSubTypeAccess();
                CurveSubTypeEntity _curvesub = acess.Get("propertyX  = '" + this._propertyX + "' and propertyY = '" + this._propertyY +"'").FirstOrDefault();
                int temp = 0;
                if (_curvesub != null)  //如果已经有值，直接返回值，否则从数据库中读取
                    temp = _curvesub.splineLine;
               
                return temp;
            }
            set { this._splineLine = value; }
        }
        /// <summary>
        /// X轴值，一般为平均沸点,若果_curveDatas中有值则获取x值为数组
        /// </summary>       
        public double[] X
        {
            set { this._x = value; }
            get
            {
                if (this._curveDatas == null)
                {
                    CurveDataAccess acess = new CurveDataAccess();
                    _curveDatas = acess.Get("curveID=" + this._ID, 0, "xValue asc");
                }

                this._x = new double[this._curveDatas.Count];
                for (int i = 0; i < this._curveDatas.Count; i++)
                    this._x[i] = this._curveDatas[i].xValue;
                return this._x;
            }
        }
        /// <summary>
        /// Y轴值，一般为原油属性值,若果_curveDatas中有值则获取y值为数组
        /// </summary>       
        public double[] Y
        {
            set { this._y = value; }
            get
            {
                if (this._curveDatas.Count == 0)
                {
                    CurveDataAccess acess = new CurveDataAccess();
                    _curveDatas = acess.Get("curveID=" + this._ID);
                }
                this._y = new double[this._curveDatas.Count];
                for (int i = 0; i < this._curveDatas.Count; i++)
                    this._y[i] = this._curveDatas[i].yValue;
                return this._y;
            }
        }
    }
    /// <summary>
    /// OilDataRowEntity局部类
    /// </summary>
    public partial class OilDataSearchRowEntity
    {
        private OilTableRowEntity _OilTableRow = null;
        private OilTableRowAccess _tableCache = new OilTableRowAccess();
        public int ColumnIndex { set; get; }
        /// <summary>
        /// 根据表OilDataRow的OilTableRowID返回OilTableRowEntity
        /// </summary>
        public OilTableRowEntity OilTableRow
        {
            get
            {
                if (this._OilTableRow != null)
                    return this._OilTableRow;
                this._OilTableRow = this._tableCache.Get("Id =" + this.OilTableRowID).FirstOrDefault();
                return this._OilTableRow;
            }
        }
        /// <summary>
        /// 判断实体是否存在
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            if (!(obj is OilDataSearchRowEntity))
                return false;
            var d = obj as OilDataSearchRowEntity;
            return this.ID.Equals(d.ID);
        }
        /// <summary>
        /// 判断实体是否相等,相等返回True
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool operator ==(OilDataSearchRowEntity one, OilDataSearchRowEntity two)
        {
            bool tag = object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null);
            if (tag)
                return true;
            tag = (object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return false;
            return one.ID.Equals(two.ID);
        }
        /// <summary>
        /// 判断实体是否相等,相等返回False
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool operator !=(OilDataSearchRowEntity one, OilDataSearchRowEntity two)
        {
            bool tag = !(object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return true;
            tag = !((object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null)));
            if (tag)
                return false;
            return !one.ID.Equals(two.ID);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public partial class OilDataSearchColEntity
    {
        private List<OilDataSearchRowEntity> _OilDataRowList = null;
        private OilDataSearchRowAccess _tableCache = new OilDataSearchRowAccess();
        public int ColumnIndex { set; get; }
        /// <summary>
        /// 返回行实体集合
        /// </summary>
        public List<OilDataSearchRowEntity> OilDataRowList
        {
            get
            {
                if (this._OilDataRowList != null)
                    return this._OilDataRowList;
                this._OilDataRowList = this._tableCache.Get("OilDataColID =" + this.ID);
                return this._OilDataRowList;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            if (!(obj is OilDataSearchColEntity))
                return false;
            var d = obj as OilDataSearchColEntity;
            return this.ID.Equals(d.ID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool operator ==(OilDataSearchColEntity one, OilDataSearchColEntity two)
        {
            bool tag = object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null);
            if (tag)
                return true;
            tag = (object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return false;
            return one.ID.Equals(two.ID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool operator !=(OilDataSearchColEntity one, OilDataSearchColEntity two)
        {
            bool tag = !(object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return true;
            tag = !((object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null)));
            if (tag)
                return false;
            return !one.ID.Equals(two.ID);
        }
    }
    public partial class OilDataSearchEntity
    {
        private OilTableColEntity _OilTableCol = null;
        private OilTableRowEntity _OilTableRow = null;
        private OilTableColBll _colCache = new OilTableColBll();
        private OilTableRowBll _rowCache = new OilTableRowBll();
        private float? _fCal = null;
        public int RowIndex { set; get; }
        public int ColumnIndex { set; get; }
        /// <summary>
        /// float型数据
        /// </summary>
        public float? fCal
        {
            get
            {
                float temp = 0;
                if (!string.IsNullOrWhiteSpace(this._calData) && this.calData != "非数字")
                {
                    if (float.TryParse(this._calData, out temp))
                        this._fCal = temp ;
                }
                return this._fCal;
            }
            set { this._fCal = value; }
        }
        public OilTableColEntity OilTableCol
        {
            get
            {
                if (this._OilTableCol != null)
                    return this._OilTableCol;
                this._OilTableCol = this._colCache.Where(t => t.ID == this._oilTableColID).FirstOrDefault();
                return this._OilTableCol;
            }
            set { this._OilTableCol = value; }
        }

        public OilTableRowEntity OilTableRow
        {
            get
            {
                if (this._OilTableRow != null)
                    return this._OilTableRow;
                this._OilTableRow = this._rowCache.Where(t => t.ID == this._oilTableRowID).FirstOrDefault();
                return this._OilTableRow;
            }
            set { this._OilTableRow = value; }
        }
        /// <summary>
        /// 表类型
        /// </summary>
        public int OilTableTypeID
        {
            get
            {
                if (this.OilTableCol != null)
                    return this.OilTableCol.oilTableTypeID;
                else
                    return 0;
            }
        }
    }
    public partial class OilRangeSearchEntity
    {
        private OilTableColEntity _OilTableCol = null;
        private OilTableRowEntity _OilTableRow = null;
        private OilTableColBll _colCache = new OilTableColBll();
        private OilTableRowBll _rowCache = new OilTableRowBll();
        public OilTableColEntity OilTableCol
        {
            get
            {
                if (this._OilTableCol != null)
                    return this._OilTableCol;
                this._OilTableCol = this._colCache.Where(t => t.ID == this._colID).FirstOrDefault();
                return this._OilTableCol;
            }
            set { this._OilTableCol = value; }
        }

        public OilTableRowEntity OilTableRow
        {
            get
            {
                if (this._OilTableRow != null)
                    return this._OilTableRow;
                this._OilTableRow = this._rowCache.Where(t => t.ID.ToString() == this._rowID.ToString()).FirstOrDefault();
                return this._OilTableRow;
            }
            set { this._OilTableRow = value; }
        }
        /// <summary>
        /// 表类型
        /// </summary>
        public int OilTableTypeID
        {
            get
            {
                if (this.OilTableCol != null)
                    return this.OilTableCol.oilTableTypeID;
                else
                    return 0;
            }
        }
    }

    public partial class RemarkEntity
    {
        private OilTableColEntity _OilTableCol = null;
        private OilTableRowEntity _OilTableRow = null;
        private OilTableColBll _colCache = new OilTableColBll();
        private OilTableRowBll _rowCache = new OilTableRowBll();
        private string _tableName = string.Empty;
        private int _LaborCal = 0;//实测列

        /// <summary>
        /// 判断是实测列还是校正列
        /// </summary>
        public int LaborCal
        {
            get { return this._LaborCal; }
            set { this._LaborCal = value; }
        }
        
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return this._tableName;}                                
            set { this._tableName = value; }
        }
        public OilTableColEntity OilTableCol
        {
            get
            {
                if (this._OilTableCol != null)
                    return this._OilTableCol;
                this._OilTableCol = this._colCache.Where(t => t.ID == this._oilTableColID).FirstOrDefault();
                return this._OilTableCol;
            }
            set { this._OilTableCol = value; }
        }

        public OilTableRowEntity OilTableRow
        {
            get
            {
                if (this._OilTableRow != null)
                    return this._OilTableRow;
                this._OilTableRow = this._rowCache.Where(t => t.ID == this._oilTableRowID).FirstOrDefault();
                return this._OilTableRow;
            }
            set { this._OilTableRow = value; }
        }

        public int OilTableTypeID
        {
            get
            {
                if (this.OilTableCol != null)
                    return this.OilTableCol.oilTableTypeID;
                else
                    return 0;
            }
        }
    }
    //public partial class CurveTypeEntity
    //{
    //    List<CurveEntity> _curves=null;           // 某种类别的曲线集合

    //    /// <summary>
    //    /// 某种类别的曲线集合
    //    /// </summary>
    //    public List<CurveEntity> curves
    //    {
    //        get
    //        {
    //            if (this._curves != null)  //如果已经有值，直接返回值，否则从数据库中读取
    //                return this._curves;
    //            CurveAccess acess = new CurveAccess();
    //            _curves = acess.Get("curveTypeID=" + this._ID);
    //            return _curves;
    //        }
    //        set { _curves = value; }
    //    }
    //}

    #region 枚举定义

    /// <summary>
    /// 库类别：A库，B库
    /// </summary>
    public enum LibraryType{LibraryA=1,LibraryB = 2,LibraryC = 3 }

    /// <summary>
    /// 表的类别名称：1Info原油信息，2Whole原油性质，3Light轻端表，4GC统计表 ,5GCLevel标准表，6Narrow窄馏分表，7Wide宽馏分表，8Residue渣油表,9SimulatedDistillation模拟馏程表,10GCInputGC输入表,11批注信息
    /// </summary>
    public enum EnumTableType {
        None = 0,

        [Description("原油信息")]
        Info = 1,

        [Description("原油性质")]
        Whole = 2,

        [Description("轻端表")]
        Light = 3,

        [Description("GC统计表")]
        GC =4,

        [Description("GC标准表")]
        GCLevel = 5,

        [Description("窄馏分")]
        Narrow = 6,

        [Description("宽馏分")]
        Wide = 7,

        [Description("渣油")]
        Residue = 8,

        [Description("模拟馏程表")]
        SimulatedDistillation =9,

        [Description("GC输入表")]
        GCInput = 10,
        
        [Description("批注")]
        Remark = 11
    }


    /// <summary>
    /// 操作类别，0:无操作,1: 添加，2:删除, 3:修改，4:上移，5:下移
    /// </summary>
    public enum EnumOperate { NoOperate, Add, Delete, Update, MoveUp, MoveDown }
    
    #endregion
}
