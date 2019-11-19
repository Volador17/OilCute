/********************************************************************************
    File:
          S_MoudleEntity.cs
    Description:
          S_Moudle实体类
    Author:
          DDBuildTools
          http://FrameWork.supesoft.com
    Finish DateTime:
          2012-4-12 23:37:48
    History:
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;


namespace RIPP.OilDB.Model
{
    ///<summary>
    ///S_MoudleEntity实体类(S_Moudle)
    ///</summary>
    [Serializable]
    public partial class S_MoudleEntity
    {
        #region "Private Variables"
    
        private Int32 _ID=0; // 主键
        private String _text=""; // 菜单标题
        private Int32 _pID=0; // 父ID，顶级菜单ID为0
        private String _name=""; // 菜单名称
        private Boolean _role1=false; // 角色2
        private Boolean _role2=false; // 角色1
        #endregion

        #region "Public Variables"
    
        /// <summary>
        /// 主键
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 菜单标题
        /// </summary>
        public String  text
        {
            set { this._text = value; }
            get { return this._text; }
        }
            
        /// <summary>
        /// 父ID，顶级菜单ID为0
        /// </summary>
        public Int32  pID
        {
            set { this._pID = value; }
            get { return this._pID; }
        }
            
        /// <summary>
        /// 菜单名称
        /// </summary>
        public String  name
        {
            set { this._name = value; }
            get { return this._name; }
        }
            
        /// <summary>
        /// 角色2
        /// </summary>
        public Boolean  role1
        {
            set { this._role1 = value; }
            get { return this._role1; }
        }
            
        /// <summary>
        /// 角色1
        /// </summary>
        public Boolean  role2
        {
            set { this._role2 = value; }
            get { return this._role2; }
        }
            
        #endregion
    }
}
  