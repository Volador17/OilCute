using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data ;
using System.Data;


namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 用于原油切割的第三种方法的9中切割性质的定义
    /// </summary>
    public  class OilPropertyAPIEntity
    {
        #region "OilApplyAPIBll的10个私有变量"
        private float _D20 = float.NaN;//密度
        private float _WAX = float.NaN;//酸值
        private float _SUL = float.NaN;//硫含量
        private float _N2 = float.NaN;//氮含量
        private float _CCR = float.NaN;//残炭
        private float _140TWY = float.NaN;//140累计收率
        private float _180TWY = float.NaN;//180累计收率
        private float _240TWY = float.NaN;//240累计收率
        private float _350TWY = float.NaN;//350累计收率
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public OilPropertyAPIEntity()
        { 
         
         
        }
        #endregion 

        #region "公有变量"
        /// <summary>
        /// 密度物性
        /// </summary>
         public float D20
         {
             get { return _D20; }
             set { _D20 = value; }
         }
         /// <summary>
         /// 酸值物性
         /// </summary>
         public float WAX
         {
             get { return _WAX; }
             set { _WAX = value; }
         }
         /// <summary>
         /// 硫含量
         /// </summary>
         public float SUL
         {
             get { return _SUL; }
             set { _SUL = value; }
         }
         /// <summary>
         /// 氮含量
         /// </summary>
         public float N2
         {
             get { return _N2; }
             set { _N2 = value; }
         }
         /// <summary>
         /// 残炭
         /// </summary>
         public float CCR
         {
             get { return _CCR; }
             set { _CCR = value; }
         }
         /// <summary>
         /// 140累计收率
         /// </summary>
         public float TWY140
         {
             get { return _140TWY; }
             set { _140TWY = value; }
         }

         /// <summary>
         /// 180累计收率
         /// </summary>
         public float TWY180
         {
             get { return _180TWY; }
             set { _180TWY = value; }
         }
         /// <summary>
         /// 240累计收率
         /// </summary>
         public float TWY240
         {
             get { return _240TWY; }
             set { _240TWY = value; }
         }

         /// <summary>
         /// 350累计收率
         /// </summary>
         public float TWY350
         {
             get { return _350TWY; }
             set { _350TWY = value; }
         }
        #endregion 
    }
}
