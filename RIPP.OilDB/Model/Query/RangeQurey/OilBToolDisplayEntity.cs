using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.Query.RangeQuery
{
    /// <summary>
    /// 原油B工具箱定制馏分段查询显示
    /// </summary>
    public class OilBToolDisplayEntity
    {
        private enumToolQueryDataBTableName _tableType = enumToolQueryDataBTableName.WhoTable;
        /// <summary>
        /// int 存储列ID
        /// </summary>
        private Dictionary<string, List<OilDataBEntity>> _tableDIC = new Dictionary<string, List<OilDataBEntity>>();
        /// <summary>
        /// int 存储列ID
        /// </summary>
        private Dictionary<string, List<CutDataEntity>> _cutDataDIC = new Dictionary<string, List<CutDataEntity>>();


        public OilBToolDisplayEntity()
        { 
        
        }
        public OilBToolDisplayEntity(enumToolQueryDataBTableName tableType)
        {
            this._tableType = tableType;

        }
        /// <summary>
        /// 表类型
        /// </summary>
        public enumToolQueryDataBTableName TableType
        {
            get { return this._tableType; }
            set { this._tableType = value; }
        }

        /// <summary>
        /// 物性名称
        /// </summary>
        public Dictionary<string, List<OilDataBEntity>> TableDIC
        {
            get { return this._tableDIC; }
            set { this._tableDIC = value; }
        }
        /// <summary>
        /// 切割数据集合
        /// </summary>
        public Dictionary<string, List<CutDataEntity>> CutDataDIC
        {
            get { return this._cutDataDIC; }
            set { this._cutDataDIC = value; }
        }
    }
}
