using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 切割计算，切割方案
    /// </summary>
    [Serializable]
    public class CutMothedEntity
    {
        #region "私有成员变量"

        private float _icp=0;           // 初馏点 
        private float _ecp = 0;          // 终馏点 
        private string _name = string.Empty;      // 馏分段名称       
        private string _cutType = string.Empty;//馏分类型\
        private string _Notation = string.Empty;//备注

        private string _strICP = string.Empty;
        private string _strECP = string.Empty;
        #endregion

        #region "构造函数"

        public CutMothedEntity()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="icp">初馏点</param>
        /// <param name="ecp">终馏点</param>
        /// <param name="name">馏分段名称</param>
        public CutMothedEntity(int icp, int ecp, string name)
        {
            this._icp = icp;
            this._ecp = ecp;
            this._name = name;
        }

        #endregion

        #region "属性"

        /// <summary>
        /// 初馏点 
        /// </summary>
        public float ICP
        {
            set { this._icp = value; }
            get { return this._icp; }
        }

        /// <summary>
        /// 终馏点
        /// </summary>
        public float ECP
        {
            set { this._ecp = value; }
            get { return this._ecp; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string strICP
        {
            set { this._strICP  = value; }
            get { return this._strICP; }
        }
        /// <summary>
        ///  
        /// </summary>
        public string strECP
        {
            set { this._strECP  = value; }
            get { return this._strECP; }
        }
        /// <summary>
        /// 馏分段名称,要求是字母和数字混合的3个字符
        /// </summary>
        public string Name
        {
            set { this._name = value; }
            get { return this._name; }
        }
        /// <summary>
        /// 切割馏分类型
        /// </summary>
        public string CutType
        {
            set { this._cutType = value; }
            get { return this._cutType; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Notation
        {
            set { this._Notation = value; }
            get { return this._Notation; }
        }
        #endregion
    }
}