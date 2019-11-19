/********************************************************************************
    File:
          OilInfoEntity.cs
    Description:
          OilInfo实体类
    Author:
          DDBuildTools
          http://FrameWork.supesoft.com
    Finish DateTime:
          2012/3/12 22:00:59
    History:
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace RIPP.OilDB.Model
{
    ///<summary>
    ///OilInfoEntity实体类(OilInfo)
    ///</summary>
    [Serializable]
    public partial class OilInfoEntity
    {
        #region "Private Variables"

        private Int32 _ID = 0; // 原油信息表
        private String _crudeName = ""; // 原油名称
        private String _englishName = ""; // 英文名称
        private String _crudeIndex = ""; // 原油编号(唯一 ,<=1000)
        private String _country = ""; // 产地国家
        private String _region = ""; // 地理区域
        private String _fieldBlock = ""; // 油田区块
        private DateTime? _sampleDate =  null; // 采样日期
        private DateTime? _receiveDate = null; // 到院日期
        private String _sampleSite = ""; // 采样地点
        private String _assayDate = ""; // 评价日期
        private String _updataDate = ""; // 入库日期
        private String _sourceRef = ""; // 数据来源
        private String _assayLab = ""; // 评价单位
        private String _assayer = ""; // 评价人员
        private String _assayCustomer = ""; // 样品来源
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

        public OilInfoEntity()
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
        /// 原油编号唯一 
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
        /// 入库日期
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
        /// 样品来源
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
        /// 由于数据库的OilTableRow的英语名称和OilInfo表中的列头名称不一样，无法查找所以写出此转换。
        /// </summary>
        /// <param name="itemCode">OilTableRow的ItemCode</param>
        /// <returns>OilInfo表中的列头名称</returns>
        public static string OilTableRowItemCodetoOilInfoItemCode(string itemCode)
        {
            string temp = string.Empty;

            if (itemCode == string.Empty)
                return temp;

            Dictionary<string, string> oilInfo = new Dictionary<string, string>();

            #region
            oilInfo.Add("CNA", "crudeName");
            oilInfo.Add("ENA", "englishName");
            oilInfo.Add("IDC", "crudeIndex");
            oilInfo.Add("COU", "country");
            oilInfo.Add("GRC", "region");
            oilInfo.Add("FBC", "fieldBlock");
            oilInfo.Add("SDA", "sampleDate");
            oilInfo.Add("RDA", "receiveDate");
            oilInfo.Add("SS", "sampleSite");

            oilInfo.Add("ADA", "assayDate");
            oilInfo.Add("UDD", "updataDate");
            oilInfo.Add("SR", "sourceRef");

            oilInfo.Add("ALA", "assayLab");
            oilInfo.Add("AER", "assayer");
            oilInfo.Add("ASC", "assayCustomer");

            oilInfo.Add("RIN", "reportIndex");
            oilInfo.Add("SUM", "summary");
            oilInfo.Add("CLA", "type");

            oilInfo.Add("TYP", "classification");
            oilInfo.Add("SCL", "sulfurLevel");
            oilInfo.Add("ACL", "acidLevel");
      
            oilInfo.Add("COL", "corrosionLevel");
            oilInfo.Add("PRI", "processingIndex");
            oilInfo.Add("NIR", "NIRSpectrum");
            oilInfo.Add("BLN", "BlendingType");
            #endregion

            foreach (string code in oilInfo.Keys)
            {
                if (code == itemCode)
                    temp = oilInfo[code];
            }

            return temp;
        }
    }


    //查询属性用的列表项
    public class OilinfItem
    {
        private String _itemName = ""; // 项目中文名（属性标题）
        private String _itemEnName = ""; // 项目英文       
        private String _itemCode = ""; // 代码
        private String _fieldName = ""; // 字段名
        private String _cruCode = ""; // Cru文件的映射代码

        public OilinfItem(string itemName, string itemEnName, string itemCode, string fieldName, string cruCode)
        {
            _itemName = itemName;
            _itemEnName = itemEnName;
            _itemCode = itemCode;
            _fieldName = fieldName;
            _cruCode = cruCode;
        }

        public string itemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public string itemEnName
        {
            get { return _itemEnName; }
            set { _itemEnName = value; }
        }

        public string itemCode
        {
            get { return _itemCode; }
            set { _itemCode = value; }
        }

        public string fieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        public string cruCode
        {
            get { return _cruCode; }
            set { _cruCode = value; }
        }
    }

    //查询属性用的列表
    public class OilinfItemList
    {
        private List<OilinfItem> _oilinfItems;

        public OilinfItemList()
        {
            _oilinfItems = new List<OilinfItem>();
            _oilinfItems.Add(new OilinfItem("原油名称", "Crude Name", "CNA", "crudeName", ""));
            _oilinfItems.Add(new OilinfItem("英文名称", "English Name", "ENA", "englishName", "NAME"));
            _oilinfItems.Add(new OilinfItem("原油编号", "Crude Index", "IDC", "crudeIndex", "CRNAME"));
            _oilinfItems.Add(new OilinfItem("开采国家", "Original Country", "COU", "country", "CNTRY"));
            _oilinfItems.Add(new OilinfItem("地理区域", "Geographical Region", "GRC", "region", "AREA"));
            _oilinfItems.Add(new OilinfItem("油田区块", "Field Block", "FBC", "fieldBlock", "BSTATE"));
            _oilinfItems.Add(new OilinfItem("采样日期", "Sample Date", "SDA", "sampleDate", "ASSAYD"));
            _oilinfItems.Add(new OilinfItem("到院日期", "Receive Date", "RDA", "receiveDate", ""));

            _oilinfItems.Add(new OilinfItem("采样地点", "Sample Site", "SS", "sampleSite", "PORT"));
            _oilinfItems.Add(new OilinfItem("评价日期", "Assay Date", "ADA", "assayDate", "DATE"));
            _oilinfItems.Add(new OilinfItem("更新日期", "Updata Date", "UDD", "updataDate", "RDATE"));
            _oilinfItems.Add(new OilinfItem("数据来源", "Source Source", "SR", "sourceRef", ""));
            _oilinfItems.Add(new OilinfItem("评价单位", "Assay Lab", "ALA", "assayLab", ""));
            _oilinfItems.Add(new OilinfItem("评价人员", "Assayer", "AER", "assayer", ""));

            _oilinfItems.Add(new OilinfItem("评价缘由", "Assay Source", "ASC", "assayCustomer", ""));
            _oilinfItems.Add(new OilinfItem("报告号", "Report Index", "RIN", "reportIndex", ""));
            _oilinfItems.Add(new OilinfItem("评论", "Summary", "SUM", "summary", ""));
            _oilinfItems.Add(new OilinfItem("类别", "Crude Oil Type", "CLA", "type", ""));
            _oilinfItems.Add(new OilinfItem("基属", "Classification", "TYP", "classification", "CRUCLASS"));

            _oilinfItems.Add(new OilinfItem("硫水平", "Sulfur Level", "SCL", "sulfurLevel", "SULTYP"));
            _oilinfItems.Add(new OilinfItem("酸水平", "Acid Level", "ACL", "acidLevel", ""));
            _oilinfItems.Add(new OilinfItem("腐蚀指数", "Corrosion Level", "COL", "corrosionLevel", ""));
            _oilinfItems.Add(new OilinfItem("加工指数", "Processing Index", "PRI", "processingIndex", ""));

        }

        public List<OilinfItem> oilinfItems
        {
            get { return _oilinfItems; }
            set { _oilinfItems = value; }
        }
    }


}
