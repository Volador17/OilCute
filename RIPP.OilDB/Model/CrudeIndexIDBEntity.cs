using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public partial class CrudeIndexIDBEntity
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
        private String _updataDate = ""; // 入库日期
        private String _sourceRef = ""; // 数据来源

        private String _assayLab = ""; // 评价单位
        private String _assayer = ""; // 评价人员      
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
        #endregion 

        public CrudeIndexIDBEntity()
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

        ///// <summary>
        ///// 评价来源
        ///// </summary>
        //public String assayCustomer
        //{
        //    set { this._assayCustomer = value; }
        //    get { return this._assayCustomer; }
        //}

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


        #endregion
    }
}
