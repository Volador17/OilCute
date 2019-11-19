using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.Query.RangeQuery 
{
    public class OilBToolDisplayHeaderEntity : DisplayHeaderBaseEntity
    {
        public OilBToolDisplayHeaderEntity()
        { 
        
        }

        public OilBToolDisplayHeaderEntity(enumToolQueryDataBTableName tableName ,string itemName,string itemCode)
        {
            TableName = tableName ;
            ItemName = itemName;
            ItemCode = itemCode;
        }
        /// <summary>
        /// 
        /// </summary>
        public enumToolQueryDataBTableName TableName
        {
            get;
            set;
        }

    }
}
