using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.Query.SimilarQuery
{
    public class SimilarQureyBaseEntity:QueryBaseEntity
    {
        #region "私有成员变量"
        private string _FoundationValue = "-1";   // 基础值
        private string _Weight = "-1";     // 权重值
        private float _Diff = -1; //对应条件的最大值和最小值之差
        #endregion

        public SimilarQureyBaseEntity()
        { 
        
        }

        #region "属性"  
        /// <summary>
        /// 对应条件的最大值和最小值之差
        /// </summary>
        public float Diff
        {
            get { return this._Diff; }
            set { this._Diff = value; }
        }
        /// <summary>
        /// 基础值
        /// </summary>
        public string strFoundationValue
        {
            set { this._FoundationValue = value; }
            get { return this._FoundationValue; }
        }
        /// <summary>
        /// 基础值
        /// </summary>
        public float? fFoundationValue
        {
            get {
                float F = 0;
                bool temp = float.TryParse(this._FoundationValue, out F);
                if (temp)
                    return F;
                else
                    return null;   
            }
        }
        /// <summary>
        /// 权重
        /// </summary>
        public string strWeight
        {
            set { this._Weight = value; }
            get { return this._Weight; }
        }   
        /// <summary>
        /// 权重
        /// </summary>
        public float? fWeight
        {
            get { 
                float w = 0;
                bool temp = float.TryParse(this._Weight, out w);
                if (temp)
                    return w;
                else
                    return null;          
            }
        }
        #endregion

    }
}
