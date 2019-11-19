using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class AccountParmTableEntity
    {
        #region "Private Variables"
     
        private Int32 _ID =0; // 原油数据表属性（行）    
        private String _itemCode = string.Empty; //代码   
        private String _itemName = string.Empty; //名称
             
        #endregion

        public AccountParmTableEntity()
        {
            
        }

        #region "Public Variables"
        
        /// <summary>
        /// 原油数据表属性（行）
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get 
            {
                 return this._ID;
            }
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
        /// 描述
        /// </summary>
        public String itemName
        {
            set { this._itemName = value; }
            get { return this._itemName; }
        }                                       
        #endregion


    }
}
