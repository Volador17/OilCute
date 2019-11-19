using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;

namespace RIPP.OilDB.Model
{
    public class CutMothedAPIEntity
    {
        
        #region "私有成员变量"
        private int _icp = 0;           // 初馏点 
        private int _ecp = 0;           // 终馏点 
        private CutTableName _name = CutTableName.YuanYouXingZhi;      // 馏分段名称       

        #endregion

        public CutMothedAPIEntity()
        { 
        
        }

        #region "属性"

        /// <summary>
        /// 初馏点 
        /// </summary>
        public int ICP
        {
            set { this._icp = value; }
            get { return this._icp; }
        }

        /// <summary>
        /// 终馏点
        /// </summary>
        public int ECP
        {
            set { this._ecp = value; }
            get { return this._ecp; }
        }

        public CutTableName Name
        {
            set { this._name = value; }
            get { return this._name; }
        }


        #endregion


    }
}
