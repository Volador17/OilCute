using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
using System.Data;


namespace RIPP.OilDB.Model
{
    [Serializable]
    public class OilDataTableBEntity
    {
        #region "Private Variables"
        private String _itemCode = ""; //实体的代码名称
        private String _oilTableName = ""; // 实体属于的表的名称       
        private String _calData = ""; // 最后一次修改值
        #endregion



        public OilDataTableBEntity()
        { 
        
        }

        #region "Public Variables"    
        /// <summary>
        /// 实体的代码名称
        /// </summary>
        public String ItemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }
         
        /// <summary>
        /// oilTableRowID
        /// </summary>
        public String OilTableName
        {
            set { this._oilTableName = value; }
            get { return this._oilTableName; }
        }
         
        /// <summary>
        /// 最后一次修改值
        /// </summary>
        public String CalData
        {
            set { this._calData = value; }
            get { return this._calData; }
        }

        #endregion

    }
}
