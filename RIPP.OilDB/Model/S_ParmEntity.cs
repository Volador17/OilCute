/********************************************************************************
    File:
          S_ParmEntity.cs
    Description:
          S_Parm实体类
    Author:
          DDBuildTools
          http://FrameWork.supesoft.com
    Finish DateTime:
          2012-4-4 9:33:23
    History:
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace RIPP.OilDB.Model
{
    ///<summary>
    ///S_ParmEntity实体类(S_Parm)
    ///</summary>
    [Serializable]
    public partial class S_ParmEntity
    {
        #region "Private Variables"     
        private Int32 _ID=0; // 主键，该表为系统参数
        private Int32 _parmTypeID=0; // 参数类别ID     
        private String _parmName=""; // 参数名
        private String _parmValue=""; // 参数值
        private Int32 _parmOrder=0; // 顺序
        #endregion

        #region "Public Variables"
     
        /// <summary>
        /// 主键，该表为系统参数
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 参数类别ID
        /// </summary>
        public Int32  parmTypeID
        {
            set { this._parmTypeID = value; }
            get { return this._parmTypeID; }
        } 
            
        /// <summary>
        /// 参数名
        /// </summary>
        public String  parmName
        {
            set { this._parmName = value; }
            get { return this._parmName; }
        }
            
        /// <summary>
        /// 参数值
        /// </summary>
        public String  parmValue
        {
            set { this._parmValue = value; }
            get { return this._parmValue; }
        }
            
        /// <summary>
        /// 顺序
        /// </summary>
        public Int32  parmOrder
        {
            set { this._parmOrder = value; }
            get { return this._parmOrder; }
        }
            
        #endregion
    }
}
  