using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;

namespace RIPP.OilDB.Model
{
    [Serializable]
    public class OutLib
    {
        private List<OilInfoOut> _oilInfoOuts = new List<OilInfoOut>();
        private List<OilTableRowOut> _oilTableRows = new List<OilTableRowOut>();  //行,为了在导入数据时能通过导出数据库的行ID得到代码，并查找到导入数据库的行ID，构建新的oilData数据(因为两个数据库的行ID一定一样)
        private List<OilTableColOut> _oilTableCols = new List<OilTableColOut>();  //列
        private List<CurveTypeEntity> _curveTypes = new List<CurveTypeEntity>();
     

        /// <summary>
        /// 某种类别的曲线集合
        /// </summary>
        public List<CurveTypeEntity> curveTypes
        {
            get
            {               
                return _curveTypes;
            }
            set { _curveTypes = value; }
        }

        /// <summary>
        /// 原油信息
        /// </summary>
        public List<OilInfoOut> oilInfoOuts
        {
            set { this._oilInfoOuts = value; }
            get { return this._oilInfoOuts; }
        }

        public List<OilTableRowOut> oilTableRows
        {
            set { this._oilTableRows = value; }
            get { return this._oilTableRows; }
        }
        public List<OilTableColOut> oilTableCols
        {
            set { this._oilTableCols = value; }
            get { return this._oilTableCols; }
        }
    }

    [Serializable]
    public partial class OilTableColOut
    {
        #region "Private Variables"

        private Int32 _ID = 0; // 原油数据表的列
        private Int32 _oilTableTypeID = 0; // 外键，表ID(属性所属哪个表)
        private String _colName = ""; // 列名
        private Int32 _colOrder = 0; // 序号(唯一)
        private Boolean _isDisplay = false; // 是否显示该列，1：显示 0：不显示
        private String _descript = ""; // 描述说明
        private Boolean _isSystem = false; // 是否系统的固定列
        private string _colCode = "";  //列编码
        private Boolean _isDisplayLab; //是否显示实测值
        #endregion

        public OilTableColOut()
        {

        }

        #region "Public Variables"

        /// <summary>
        /// 原油数据表的列
        /// </summary>
        public Int32 ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 外键，表ID(属性所属哪个表)
        /// </summary>
        public Int32 oilTableTypeID
        {
            set { this._oilTableTypeID = value; }
            get { return this._oilTableTypeID; }
        }

        /// <summary>
        /// 列名
        /// </summary>
        public String colName
        {
            set { this._colName = value; }
            get { return this._colName; }
        }

        /// <summary>
        /// 序号(唯一)
        /// </summary>
        public Int32 colOrder
        {
            set { this._colOrder = value; }
            get { return this._colOrder; }
        }

        /// <summary>
        /// 是否显示该列，1：显示 0：不显示
        /// </summary>
        public Boolean isDisplay
        {
            set { this._isDisplay = value; }
            get { return this._isDisplay; }
        }

        /// <summary>
        /// 描述说明
        /// </summary>
        public String descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }

        /// <summary>
        /// 是否系统的固定列
        /// </summary>
        public Boolean isSystem
        {
            set { this._isSystem = value; }
            get { return this._isSystem; }
        }

        /// <summary>
        /// 描述说明
        /// </summary>
        public String colCode
        {
            set { this._colCode = value; }
            get { return this._colCode; }
        }

        /// <summary>
        /// 是否显示实测值
        /// </summary>
        public Boolean isDisplayLab
        {
            set { this._isDisplayLab = value; }
            get { return this._isDisplayLab; }
        }
        #endregion
    }

    [Serializable]
    public partial class OilTableRowOut
    {
        #region "Private Variables"

        private Int32 _ID = 0; // 原油数据表属性（行）
        private Int32 _oilTableTypeID = 0; // 外键，OilTableType表ID(属性所属哪个表)
        private Int32 _itemOrder = 0; // 序号（唯一）
        private String _itemName = ""; // 项目（属性标题）
        private String _itemEnName = ""; // 项目英文
        private String _itemUnit = ""; // 单位
        private String _itemCode = ""; // 代码
        private String _dataType = ""; // 数据类型(在程序中用枚举类型0：float 1:varchar等)
        private String _trend = ""; // 趋势类型(在程序中用枚举类型 +/-)
        private Int32 _decNumber = 0; // 小数位数
        private Int32 _valDigital = 0; // 有效数字
        private Boolean _isKey = false; // 是否是关键性质（1:是,0不是）
        private Boolean _isDisplay = false; // 是否显示该属性,1：显示 0：不显示
        private float _errDownLimit = 0; // 错误下限
        private float _errUpLimit = 0; // 错误上限
        private float _alertDownLimit = 0; // 警告下限
        private float _alertUpLimit = 0; // 警告上限
        private float _evalDownLimit = 0; // 评价下限
        private float _evalUpLimit = 0; // 评价上限
        private String _descript = ""; // 描述说明
        private String _subItemName = ""; // subItemName
        private Boolean _isSystem = false; // isSystem
        #endregion

        public OilTableRowOut()
        {

        }

        #region "Public Variables"

        /// <summary>
        /// 原油数据表属性（行）
        /// </summary>
        public Int32 ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 外键，OilTableType表ID(属性所属哪个表)
        /// </summary>
        public Int32 oilTableTypeID
        {
            set { this._oilTableTypeID = value; }
            get { return this._oilTableTypeID; }
        }

        /// <summary>
        /// 序号（唯一）
        /// </summary>
        public Int32 itemOrder
        {
            set { this._itemOrder = value; }
            get { return this._itemOrder; }
        }

        /// <summary>
        /// 项目（属性标题）
        /// </summary>
        public String itemName
        {
            set { this._itemName = value; }
            get { return this._itemName; }
        }

        /// <summary>
        /// 项目英文
        /// </summary>
        public String itemEnName
        {
            set { this._itemEnName = value; }
            get { return this._itemEnName; }
        }

        /// <summary>
        /// 单位
        /// </summary>
        public String itemUnit
        {
            set { this._itemUnit = value; }
            get { return this._itemUnit; }
        }

        /// <summary>
        /// 代码
        /// </summary>
        public String itemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }

        /// <summary>
        /// 数据类型(在程序中用枚举类型0：float 1:varchar等)
        /// </summary>
        public String dataType
        {
            set { this._dataType = value; }
            get { return this._dataType; }
        }

        /// <summary>
        /// 小数位数
        /// </summary>
        public Int32 decNumber
        {
            set { this._decNumber = value; }
            get { return this._decNumber; }
        }

        /// <summary>
        /// 有效数字
        /// </summary>
        public Int32 valDigital
        {
            set { this._valDigital = value; }
            get { return this._valDigital; }
        }

        /// <summary>
        /// 是否是关键性质（1:是,0不是）
        /// </summary>
        public Boolean isKey
        {
            set { this._isKey = value; }
            get { return this._isKey; }
        }

        /// <summary>
        /// 是否显示该属性,1：显示 0：不显示
        /// </summary>
        public Boolean isDisplay
        {
            set { this._isDisplay = value; }
            get { return this._isDisplay; }
        }
        /// <summary>
        /// 趋势类型(数据库中用+/-)
        /// </summary>
        public String trend
        {
            set { this._trend = value; }
            get { return this._trend; }
        }
        /// <summary>
        /// 错误下限
        /// </summary>
        public float errDownLimit
        {
            set { this._errDownLimit = value; }
            get { return this._errDownLimit; }
        }

        /// <summary>
        /// 错误上限
        /// </summary>
        public float errUpLimit
        {
            set { this._errUpLimit = value; }
            get { return this._errUpLimit; }
        }

        /// <summary>
        /// 警告下限
        /// </summary>
        public float alertDownLimit
        {
            set { this._alertDownLimit = value; }
            get { return this._alertDownLimit; }
        }

        /// <summary>
        /// 警告上限
        /// </summary>
        public float alertUpLimit
        {
            set { this._alertUpLimit = value; }
            get { return this._alertUpLimit; }
        }

        /// <summary>
        /// 评价下限
        /// </summary>
        public float evalDownLimit
        {
            set { this._evalDownLimit = value; }
            get { return this._evalDownLimit; }
        }

        /// <summary>
        /// 评价上限
        /// </summary>
        public float evalUpLimit
        {
            set { this._evalUpLimit = value; }
            get { return this._evalUpLimit; }
        }

        /// <summary>
        /// 描述说明
        /// </summary>
        public String descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }

        /// <summary>
        /// subItemName
        /// </summary>
        public String subItemName
        {
            set { this._subItemName = value; }
            get { return this._subItemName; }
        }

        /// <summary>
        /// isSystem
        /// </summary>
        public Boolean isSystem
        {
            set { this._isSystem = value; }
            get { return this._isSystem; }
        }

        #endregion
    }

    ///<summary>
    ///OilInfoEntity实体类(OilInfo)
    ///</summary>
    [Serializable]
    public partial class OilInfoOut
    {
        #region "Private Variables"

        private Int32 _ID = 0; // 原油信息表
        private String _crudeName = ""; // 原油名称
        private String _englishName = ""; // 英文名称
        private String _crudeIndex = ""; // 原油编号(唯一 ,<=1000)
        private String _country = ""; // 产地国家
        private String _region = ""; // 地理区域
        private String _fieldBlock = ""; // 油田区块
        private DateTime? _sampleDate = null; // 采样日期
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
        private String _ICP0 = string.Empty;//窄馏分的第一个ICP列的计算结果 

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
        private Boolean _isLibraryA = false; // 是否A库
        private Boolean _isLibraryB = false; // 是否B库

        private List<OilDataOut> _oilDatas = new List<OilDataOut>();//非曲线的未切割的数据
        private List<OilDataSearchOut> _oilDataSearchs = new List<OilDataSearchOut>();//切割后的数据
        private List<CurveEntity> _curves = new List<CurveEntity>();           // 该原油所有的曲线集合
     
        #endregion

        public OilInfoOut()
        {

        }

        #region "Public Variables"

        /// <summary>
        /// 原油曲线的集合
        /// </summary>
        public List<CurveEntity> curves
        {
            get
            {
                return _curves;
            }
            set { _curves = value; }
        }

        /// <summary>
        /// 原油数据
        /// </summary>
        public List<OilDataOut> oilDatas
        {
            set { this._oilDatas = value; }
            get { return this._oilDatas; }
        }
        /// <summary>
        /// 切割后的数据
        /// </summary>
        public List<OilDataSearchOut> oilDataSearchOuts
        {
            set { this._oilDataSearchs = value; }
            get { return this._oilDataSearchs; }
        }
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
        public String assayDate
        {
            set { this._assayDate = value; }
            get { return this._assayDate; }
        }

        /// <summary>
        /// 更新日期
        /// </summary>
        public String updataDate
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
        /// <summary>
        /// 是否已经生成B库
        /// </summary>
        public Boolean isLibraryA
        {
            set { this._isLibraryA = value; }
            get { return this._isLibraryA; }
        }
        /// <summary>
        /// 是否已经生成B库
        /// </summary>
        public Boolean isLibraryB
        {
            set { this._isLibraryB = value; }
            get { return this._isLibraryB; }
        }

        #endregion
    }

    [Serializable]
    public partial class OilDataOut
    {
        #region "Private Variables"
        private Int32 _ID = 0; // 原油数据，数据字段包括实测值和最后一次修改值，历史修改值放在OilDataModify表。需要时再加载。
        private Int32 _oilInfoID = 0; // 外键，原油信息ID(OilInfo表的ID,可以表示一个批次样本)
        private Int32 _oilTableColID = 0; // 数据表列的ID
        private Int32 _oilTableRowID = 0; // oilTableRowID
        private String _labData = ""; // 实验室实测数据(有文本数据)
        private String _calData = ""; // 最后一次修改值
        #endregion

        public OilDataOut()
        {

        }

        #region "Public Variables"

        /// <summary>
        /// 原油数据，数据字段包括实测值和最后一次修改值，历史修改值放在OilDataModify表。需要时再加载。
        /// </summary>
        public Int32 ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 外键，原油信息ID(OilInfo表的ID,可以表示一个批次样本)
        /// </summary>
        public Int32 oilInfoID
        {
            set { this._oilInfoID = value; }
            get { return this._oilInfoID; }
        }

        /// <summary>
        /// 数据表列的ID
        /// </summary>
        public Int32 oilTableColID
        {
            set { this._oilTableColID = value; }
            get { return this._oilTableColID; }
        }

        /// <summary>
        /// oilTableRowID
        /// </summary>
        public Int32 oilTableRowID
        {
            set { this._oilTableRowID = value; }
            get { return this._oilTableRowID; }
        }

        /// <summary>
        /// 实验室实测数据(有文本数据)
        /// </summary>
        public String labData
        {
            set { this._labData = value; }
            get { return this._labData; }
        }

        /// <summary>
        /// 最后一次修改值
        /// </summary>
        public String calData
        {
            set { this._calData = value; }
            get { return this._calData; }
        }

        #endregion
    }
    [Serializable]
    public partial class OilDataSearchOut
    {
        #region "Private Variables"
        private Int32 _ID = 0; // 原油数据，数据字段包括实测值和最后一次修改值，历史修改值放在OilDataModify表。需要时再加载。
        private Int32 _oilInfoID = 0; // 外键，原油信息ID(OilInfo表的ID,可以表示一个批次样本)
        private Int32 _oilTableColID = 0; // 数据表列的ID
        private Int32 _oilTableRowID = 0; // oilTableRowID
        private String _labData = ""; // 实验室实测数据(有文本数据)
        private String _calData = ""; // 最后一次修改值
        #endregion

        public OilDataSearchOut()
        {

        }

        #region "Public Variables"

        /// <summary>
        /// 原油数据，数据字段包括实测值和最后一次修改值，历史修改值放在OilDataModify表。需要时再加载。
        /// </summary>
        public Int32 ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 外键，原油信息ID(OilInfo表的ID,可以表示一个批次样本)
        /// </summary>
        public Int32 oilInfoID
        {
            set { this._oilInfoID = value; }
            get { return this._oilInfoID; }
        }

        /// <summary>
        /// 数据表列的ID
        /// </summary>
        public Int32 oilTableColID
        {
            set { this._oilTableColID = value; }
            get { return this._oilTableColID; }
        }

        /// <summary>
        /// oilTableRowID
        /// </summary>
        public Int32 oilTableRowID
        {
            set { this._oilTableRowID = value; }
            get { return this._oilTableRowID; }
        }

        /// <summary>
        /// 实验室实测数据(有文本数据)
        /// </summary>
        public String labData
        {
            set { this._labData = value; }
            get { return this._labData; }
        }

        /// <summary>
        /// 最后一次修改值
        /// </summary>
        public String calData
        {
            set { this._calData = value; }
            get { return this._calData; }
        }

        #endregion
    }
    
    [Serializable]
    public partial class CutMothedOutLib
    {
        #region"private Variables"
        private  List <CutMothedEntity> _CutMotheds = null;
        #endregion 

        public CutMothedOutLib()
        {

        }

        #region"Public Variables"
        /// <summary>
        /// 初馏点 
        /// </summary>
        public List<CutMothedEntity> CutMotheds
        {
            set { this._CutMotheds = value; }
            get { return this._CutMotheds; }
        }       
        #endregion
    }
    [Serializable]
    public partial class CutMothedDcOutLib
    {
        #region"private Variables"
        private Dictionary<string, List<CutMothedEntity>> _cutMothedDic = null;
        #endregion

        public CutMothedDcOutLib()
        {

        }

        #region"Public Variables"
        /// <summary>
        /// 初馏点 
        /// </summary>
        public Dictionary<string, List<CutMothedEntity>> CutMothedDic
        {
            set { this._cutMothedDic = value; }
            get { return this._cutMothedDic; }
        }
        #endregion
    }
    [Serializable]
    public partial class OilSearchConditionOutLib
    {
        #region"private Variables"
        private List<OilSearchConditionOutEntity> _OilSearchConditionOutEntityList = null;
        #endregion

        public OilSearchConditionOutLib()
        {

        }

        #region"Public Variables"
        /// <summary>
        /// 查找条件集合
        /// </summary>
        public List<OilSearchConditionOutEntity> OilRangeSearchList
        {
            set { this._OilSearchConditionOutEntityList = value; }
            get { return this._OilSearchConditionOutEntityList; }
        }
        #endregion
    }


}