using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class WCutTypeEntity
    {
        
        #region "私有成员变量"

        private int _ID=0;           // 主键 
        private int _code=0;           // 代码，在程序中使用 
        private string _name="";       // 馏分段名称       

        #endregion

        #region "构造函数"

        public WCutTypeEntity()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ID">主键</param>
        /// <param name="code">代码，在程序中使用</param>
        /// <param name="name">馏分段名称</param>
        public WCutTypeEntity(int ID, int code, string name)
        {
            this._ID = ID;
            this._code = code;
            this._name = name;
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
        /// 代码，在程序中使用
        /// </summary>
        public int code
        {
            set { this._code = value; }
            get { return this._code; }
        }

        /// <summary>
        /// 馏分段名称
        /// </summary>
        public string name
        {
            set { this._name = value; }
            get { return this._name; }
        }

        #endregion
    }
}
