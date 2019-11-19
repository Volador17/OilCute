/********************************************************************************
    File:
          S_ParmTypeEntity.cs
    Description:
          S_ParmType实体类
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
    ///S_ParmTypeEntity实体类(S_ParmType)
    ///</summary>
    [Serializable]
    public partial class S_ParmTypeEntity
    {
        #region "Private Variables"      
        private Int32 _ID=0; // 主键，该表为参数类别
        private String _parmTypeName=""; // 参数类别名称
        private string _code;            //代码
        private Boolean _isSystem=false; // 是否系统参数，系统参数不能修改
        private String _descript=""; // 该参数类别的说明
        #endregion

        #region "Public Variables"
       
        /// <summary>
        /// 主键，该表为参数类别
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 参数类别名称
        /// </summary>
        public String  parmTypeName
        {
            set { this._parmTypeName = value; }
            get { return this._parmTypeName; }
        }

        /// <summary>
        /// 代码
        /// </summary>
        public String code
        {
            set { this._code = value; }
            get { return this._code; }
        }
            
        /// <summary>
        /// 是否系统参数，系统参数不能修改
        /// </summary>
        public Boolean  isSystem
        {
            set { this._isSystem = value; }
            get { return this._isSystem; }
        }
            
        /// <summary>
        /// 该参数类别的说明
        /// </summary>
        public String  descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }
            
        #endregion
    }
}
  