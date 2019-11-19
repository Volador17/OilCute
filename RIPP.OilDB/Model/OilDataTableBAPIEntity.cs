using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
using System.Data;

namespace RIPP.OilDB.Model
{
    public  class OilDataTableBAPIEntity
    {
        #region "Private Variables"
        private String _itemCode = ""; //实体的代码名称
        private CutTableName _cutTableName = CutTableName.ShiNaoYou; // 实体属于的表的名称       
        private float _calData = 0; // 最后一次修改值
        #endregion

        public OilDataTableBAPIEntity()
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
        /// 实体属于的表的名称    
        /// </summary>
        public CutTableName cutTableName
        {
            set { this._cutTableName = value; }
            get { return this._cutTableName; }
        }
        /// <summary>
        /// 最后一次修改值
        /// </summary>
        public float  CalData
        {
            set { this._calData = value; }
            get { return this._calData; }
        }

        #endregion
    }
}
