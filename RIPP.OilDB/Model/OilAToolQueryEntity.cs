using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class OilAToolQueryEntity
    {
        private EnumTableType _tableType = EnumTableType.Whole;
        /// <summary>
        /// int 存储列ID
        /// </summary>
        private Dictionary<int, List<OilDataEntity>> _tableDIC = new Dictionary<int, List<OilDataEntity>>();
        /// <summary>
        /// int 存储列ID
        /// </summary>
        private Dictionary<string, List<RemarkEntity>> _remarkDIC = new Dictionary<string, List<RemarkEntity>>();

        /// <summary>
        /// 
        /// </summary>
        public OilAToolQueryEntity()
        { 
        
        }
        /// <summary>
        /// 
        /// </summary>
        public OilAToolQueryEntity(EnumTableType tableType)
        {
            this._tableType = tableType;

        }

        /// <summary>
        /// 表类型
        /// </summary>
        public EnumTableType TableType
        {
            get { return this._tableType; }
            set { this._tableType = value; }
        }

        /// <summary>
        /// 物性名称
        /// </summary>
        public Dictionary<int, List<OilDataEntity>> TableDIC
        {
            get { return this._tableDIC; }
            set { this._tableDIC = value; }
        }
        /// <summary>
        /// 批注的物性名称
        /// </summary>
        public Dictionary<string, List<RemarkEntity>> RemarkDIC
        {
            get { return this._remarkDIC; }
            set { this._remarkDIC = value; }
        }
    }

}
