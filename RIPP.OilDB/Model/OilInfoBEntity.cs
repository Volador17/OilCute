using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
using System.Data;

namespace RIPP.OilDB.Model
{
    ///<summary>
    ///OilInfoBEntity实体类(OilInfoB表)
    ///</summary>
    [Serializable]
    public partial class OilInfoBEntity
    {
        #region "Private Variables"

        private Int32 _ID = 0; // 原油信息表
        private String _crudeName = ""; // 原油名称
        private String _englishName = ""; // 英文名称
        private String _crudeIndex = ""; // 原油编号(唯一 ,<=1000)
        private String _country = ""; // 产地国家
        private String _region = ""; // 地理区域
        private String _fieldBlock = ""; // 油田区块
        private DateTime? _sampleDate = null;// 采样日期
        private DateTime? _receiveDate = null; // 到院日期
        private String _sampleSite = ""; // 采样地点
        private String _assayDate = ""; // 评价日期
        private String _updataDate = ""; // 更新日期
        private String _sourceRef = ""; // 数据来源
        private String _assayLab = ""; // 评价单位
        private String _assayer = ""; // 评价人员
        private String _assayCustomer = ""; // 评价来源
        private String _reportIndex = ""; // 报告号
        private String _summary = ""; // 评论
        private String _type = ""; // 类别
        private String _classification = ""; // 基属
        private String _sulfurLevel = ""; // 硫水平
        private String _acidLevel = ""; // 酸水平
        private String _corrosionLevel = ""; // 腐蚀指数
        private String _processingIndex = ""; // 加工指数
        private String _NIRSpectrum = "";//NIR光谱
        private String _blendingType = "";//混合类型

        private String _DataQuality = "";//数据质量
        private String _Remark = "";//备注信息
        private String _01R = "";//补充信息1
        private String _02R = "";//补充信息2
        private String _03R = "";//补充信息3
        private String _04R = "";//补充信息4
        private String _05R = "";//补充信息5
        private String _06R = "";//补充信息6
        private String _07R = "";//补充信息7
        private String _08R = "";//补充信息8
        private String _09R = "";//补充信息9
        private String _10R = "";//补充信息10
        private String _DataSource = "";//数据资源 
        private String _ICP0 = string.Empty;//窄馏分的第一个ICP列的计算结果 
        #endregion

        private List<CurveTypeEntity> _curveTypes = null;
        private List<CurveEntity> _curves = null;           // 该原油所有的曲线集合
        private DataTable _dataTable = null;//存放切割数据, 包括原油性质和原油曲线数据,因为数据不用存储所以用此方法传递数据
        private List<CutDataEntity> _cutDataEntityList = new List<CutDataEntity> ();//存放有界面切割数据, 不包括原油性质和原油曲线数据,因为数据不用存储所以用此方法传递数据
        private List<OilDataTableBAPIEntity> _oilDataTableBAPIEntityList = new List<OilDataTableBAPIEntity> ();//存放切无界面割数据, 包括原油性质和原油曲线数据,因为数据不用存储所以用此方法传递数据
        private StringBuilder _strMissValue = null;////缺失曲线提示
        private List<ShowCurveEntity> _oilCutCurves = new List<ShowCurveEntity>();//存放切无界面割数据, 包括原油性质和原油曲线数据,因为数据不用存储所以用此方法传递数据

        #region "原油混合用"
        //private List<ObjectItemEntity> itemEntityList = null;//存放一条原油对应的所选择物性的相关数据
        //private double oilRatio = 0;//存放一条原油的混合比例
        //private int oilIsSelect = 0;//存放一条原油是否被选中
        //private bool someItemValueIsEmpty = false;
        #endregion

        public OilInfoBEntity()
        {
        }

        #region "Public Variables"

        /// <summary>
        /// 原油信息表
        /// </summary>
        public Int32 ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 原油名称
        /// </summary>
        public String crudeName
        {
            set { this._crudeName = value; }
            get { return this._crudeName; }
        }

        /// <summary>
        /// 英文名称
        /// </summary>
        public String englishName
        {
            set { this._englishName = value; }
            get { return this._englishName; }
        }

        /// <summary>
        /// 原油编号(唯一 ,<=1000)
        /// </summary>
        public String crudeIndex
        {
            set { this._crudeIndex = value; }
            get { return this._crudeIndex; }
        }

        /// <summary>
        /// 产地国家
        /// </summary>
        public String country
        {
            set { this._country = value; }
            get { return this._country; }
        }

        /// <summary>
        /// 地理区域
        /// </summary>
        public String region
        {
            set { this._region = value; }
            get { return this._region; }
        }

        /// <summary>
        /// 油田区块
        /// </summary>
        public String fieldBlock
        {
            set { this._fieldBlock = value; }
            get { return this._fieldBlock; }
        }

        /// <summary>
        /// 采样日期
        /// </summary>
        public DateTime? sampleDate
        {
            set { this._sampleDate = value; }
            get { return this._sampleDate; }
        }

        /// <summary>
        /// 到院日期
        /// </summary>
        public DateTime? receiveDate
        {
            set { this._receiveDate = value; }
            get { return this._receiveDate; }
        }

        /// <summary>
        /// 采样地点
        /// </summary>
        public String sampleSite
        {
            set { this._sampleSite = value; }
            get { return this._sampleSite; }
        }

        /// <summary>
        /// 评价日期
        /// </summary>
        public string assayDate
        {
            set { this._assayDate = value; }
            get { return this._assayDate; }
        }

        /// <summary>
        /// 更新日期
        /// </summary>
        public string updataDate
        {
            set { this._updataDate = value; }
            get { return this._updataDate; }
        }

        /// <summary>
        /// 数据来源
        /// </summary>
        public String sourceRef
        {
            set { this._sourceRef = value; }
            get { return this._sourceRef; }
        }

        /// <summary>
        /// 评价单位
        /// </summary>
        public String assayLab
        {
            set { this._assayLab = value; }
            get { return this._assayLab; }
        }

        /// <summary>
        /// 评价人员
        /// </summary>
        public String assayer
        {
            set { this._assayer = value; }
            get { return this._assayer; }
        }

        /// <summary>
        /// 评价来源
        /// </summary>
        public String assayCustomer
        {
            set { this._assayCustomer = value; }
            get { return this._assayCustomer; }
        }

        /// <summary>
        /// 报告号
        /// </summary>
        public String reportIndex
        {
            set { this._reportIndex = value; }
            get { return this._reportIndex; }
        }

        /// <summary>
        /// 评论
        /// </summary>
        public String summary
        {
            set { this._summary = value; }
            get { return this._summary; }
        }

        /// <summary>
        /// 类别
        /// </summary>
        public String type
        {
            set { this._type = value; }
            get { return this._type; }
        }

        /// <summary>
        /// 基属
        /// </summary>
        public String classification
        {
            set { this._classification = value; }
            get { return this._classification; }
        }

        /// <summary>
        /// 硫水平
        /// </summary>
        public String sulfurLevel
        {
            set { this._sulfurLevel = value; }
            get { return this._sulfurLevel; }
        }

        /// <summary>
        /// 酸水平
        /// </summary>
        public String acidLevel
        {
            set { this._acidLevel = value; }
            get { return this._acidLevel; }
        }

        /// <summary>
        /// 腐蚀指数
        /// </summary>
        public String corrosionLevel
        {
            set { this._corrosionLevel = value; }
            get { return this._corrosionLevel; }
        }

        /// <summary>
        /// 加工指数
        /// </summary>
        public String processingIndex
        {
            set { this._processingIndex = value; }
            get { return this._processingIndex; }
        }

        /// <summary>
        /// NIR光谱
        /// </summary>
        public String NIRSpectrum
        {
            set { this._NIRSpectrum = value; }
            get { return this._NIRSpectrum; }
        }

        /// <summary>
        /// 混合类型
        /// </summary>
        public String BlendingType
        {
            set { this._blendingType = value; }
            get { return this._blendingType; }
        }

        /// <summary>
        /// 数据质量
        /// </summary>
        public String DataQuality
        {
            set { this._DataQuality = value; }
            get { return this._DataQuality; }
        }

        /// <summary>
        /// 备注信息
        /// </summary>
        public String Remark
        {
            set { this._Remark = value; }
            get { return this._Remark; }
        }


        /// <summary>
        /// 补充信息1
        /// </summary>
        public String S_01R
        {
            set { this._01R = value; }
            get { return this._01R; }
        }

        /// <summary>
        /// 补充信息2
        /// </summary>
        public String S_02R
        {
            set { this._02R = value; }
            get { return this._02R; }
        }


        /// <summary>
        /// 补充信息3
        /// </summary>
        public String S_03R
        {
            set { this._03R = value; }
            get { return this._03R; }
        }

        /// <summary>
        /// 补充信息4
        /// </summary>
        public String S_04R
        {
            set { this._04R = value; }
            get { return this._04R; }
        }


        /// <summary>
        /// 补充信息5
        /// </summary>
        public String S_05R
        {
            set { this._05R = value; }
            get { return this._05R; }
        }

        /// <summary>
        /// 补充信息6
        /// </summary>
        public String S_06R
        {
            set { this._06R = value; }
            get { return this._06R; }
        }
        /// <summary>
        /// 补充信息7
        /// </summary>
        public String S_07R
        {
            set { this._07R = value; }
            get { return this._07R; }
        }

        /// <summary>
        /// 补充信息8
        /// </summary>
        public String S_08R
        {
            set { this._08R = value; }
            get { return this._08R; }
        }

        /// <summary>
        /// 补充信息9
        /// </summary>
        public String S_09R
        {
            set { this._09R = value; }
            get { return this._09R; }
        }

        /// <summary>
        /// 补充信息10
        /// </summary>
        public String S_10R
        {
            set { this._10R = value; }
            get { return this._10R; }
        }
        /// <summary>
        /// 数据资源
        /// </summary>
        public String DataSource
        {
            set { this._DataSource = value; }
            get { return this._DataSource; }
        }
        /// <summary>
        /// 窄馏分的第一个ICP列的计算结果 
        /// </summary>
        public String ICP0
        {
            set { this._ICP0 = value; }
            get { return this._ICP0; }
        }
        #endregion

        /// <summary>
        /// 某种类别的曲线集合
        /// </summary>
        public List<CurveTypeEntity> curveTypes
        {
            get
            {
                if (_curveTypes != null)
                    return _curveTypes;
                CurveTypeAccess acess = new CurveTypeAccess();
                _curveTypes = acess.Get("1=1");
                return _curveTypes;
            }
            set { _curveTypes = value; }
        }

        /// <summary>
        /// 原油曲线的集合
        /// </summary>
        public List<CurveEntity> curves
        {
            get
            {
                if (this._curves != null)  //如果已经有值，直接返回值，否则从数据库中读取
                    return this._curves;
                CurveAccess acess = new CurveAccess();
                _curves = acess.Get("oilInfoID =" + ID);
                return _curves;
            }
            set { _curves = value; }
        }

        /// <summary>
        /// 原油的数据传递,包括原油性质和切割后的曲线数据
        /// </summary>
        public DataTable dataTable
        {
            set { this._dataTable = value; }
            get { return this._dataTable; }
        }
        /// <summary>
        /// 有界面的原油应用原油的数据传递,不包括原油性质和切割后的曲线数据
        /// </summary>
        public List<CutDataEntity> CutDataEntityList
        {
            set { this._cutDataEntityList = value; }
            get { return this._cutDataEntityList; }
        }
        /// <summary>
        /// 无界面的原油应用原油的数据传递,包括原油性质和切割后的曲线数据
        /// </summary>
        public List<OilDataTableBAPIEntity> OilDataTableBAPIEntityList
        {
            set { this._oilDataTableBAPIEntityList = value; }
            get { return this._oilDataTableBAPIEntityList; }
        }
        /// <summary>
        /// 切割后的数据集合
        /// </summary>
        public List<ShowCurveEntity> OilCutCurves
        {
            set { this._oilCutCurves = value; }
            get { return this._oilCutCurves; }
        }
        /// <summary>
        /// 缺失曲线提示
        /// </summary>
        public StringBuilder strMissValue
        {
            set { this._strMissValue = value; }
            get { return _strMissValue; }
        }

        ///// <summary>
        ///// 存放选择的物性值(混合优化用-存放所有选择的物性信息)
        ///// </summary>
        //public List<ObjectItemEntity> ItemEntityList
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 存放原油混合比例（在0-1之间）
        /// </summary>
        public double OilRation
        {
            get;
            set;
        }

        /// <summary>
        /// 存放该条原油是否被选中（0-1变量）
        /// </summary>
        public int OilIsSelect
        {
            get;
            set;
        }

        /// <summary>
        /// 用来标记一条原油中选择的物性是否有空值
        /// </summary>
        public bool SomeItemValueIsEmpty
        {
            get;
            set;
        }
    }
}