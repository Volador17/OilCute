using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    [Serializable]
    public partial class CurveTypeEntity
    {
        #region "私有成员变量"

        private int _ID=0;               // ID 
        private string _typeName="";          // 曲线类别名称
        private string _typeCode="";          // 曲线类别编码
        private string _descript="";          // 描述

        #endregion

        #region "构造函数"

        public CurveTypeEntity()
        {
        }

        /// <summary>
        /// 带初值的构造函数
        /// </summary>
        /// <param name="id">ID </param>
        /// <param name="typeName">曲线类别名称</param>
        /// <param name="typeCode">曲线类别编码</param>
        /// <param name="descript">描述</param>
        public CurveTypeEntity(int id, string typeName, string typeCode, string descript)
        {
            this._ID = id;
            this._typeName = typeName;
            this._typeCode = typeCode;
            this._descript = descript;
        }

        #endregion

        #region "属性"

        /// <summary>
        /// 主键
        /// </summary>
        public int ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 曲线类别名称
        /// </summary>
        public string typeName
        {
            set { this._typeName = value; }
            get { return this._typeName; }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public string typeCode
        {
            set { this._typeCode = value; }
            get { return this._typeCode; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }

        #endregion
    }
}
