/********************************************************************************
    File:
          GCMatch2Entity.cs
    Description:
          GCMatch2实体类
    Author:
          DDBuildTools
          http://FrameWork.supesoft.com
    Finish DateTime:
          2012-4-2 15:07:01
    History:
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;


namespace RIPP.OilDB.Model
{
    ///<summary>
    ///GCMatch2Entity实体类(GCMatch2)
    ///</summary>
    [Serializable]
    public partial class GCMatch2Entity
    {
        #region "Private Variables"
      
        private Int32 _ID=0; // 主键
        private String _itemCode=""; // 编码
        private String _descript=""; // descipt
        private Int32 _colIntC=0; // colIntC
        private String _colStrD=""; // colStrD
        private float _colFloatE=-1; // colFloatE
        private float _colFloatF=-1; // colFloatF
        private Int32 _colIntG=-1; // colIntG
        private float _colFloatH=-1; // colFloatH
        private float _colFloatI=-1; // colFloatI
        private float _colFloatJ=-1; // colFloatJ
        private float _colFloatK=-1; // colFloatK
        private Int32 _colIntL=0; // colIntL
        private Int32 _colIntM=0; // colIntM
        private float _colFloatN=0; // colFloatN
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
        /// 编码
        /// </summary>
        public String  itemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }
            
        /// <summary>
        /// descipt
        /// </summary>
        public String  descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }
            
        /// <summary>
        /// colIntC
        /// </summary>
        public Int32  colIntC
        {
            set { this._colIntC = value; }
            get { return this._colIntC; }
        }
            
        /// <summary>
        /// colStrD
        /// </summary>
        public String  colStrD
        {
            set { this._colStrD = value; }
            get { return this._colStrD; }
        }
            
        /// <summary>
        /// colFloatE
        /// </summary>
        public float  colFloatE
        {
            set { this._colFloatE = value; }
            get { return this._colFloatE; }
        }
            
        /// <summary>
        /// colFloatF
        /// </summary>
        public float  colFloatF
        {
            set { this._colFloatF = value; }
            get { return this._colFloatF; }
        }
            
        /// <summary>
        /// colIntG
        /// </summary>
        public Int32  colIntG
        {
            set { this._colIntG = value; }
            get { return this._colIntG; }
        }
            
        /// <summary>
        /// colFloatH
        /// </summary>
        public float  colFloatH
        {
            set { this._colFloatH = value; }
            get { return this._colFloatH; }
        }
            
        /// <summary>
        /// colFloatI
        /// </summary>
        public float  colFloatI
        {
            set { this._colFloatI = value; }
            get { return this._colFloatI; }
        }
            
        /// <summary>
        /// colFloatJ
        /// </summary>
        public float  colFloatJ
        {
            set { this._colFloatJ = value; }
            get { return this._colFloatJ; }
        }
            
        /// <summary>
        /// colFloatK
        /// </summary>
        public float  colFloatK
        {
            set { this._colFloatK = value; }
            get { return this._colFloatK; }
        }
            
        /// <summary>
        /// colIntL
        /// </summary>
        public Int32  colIntL
        {
            set { this._colIntL = value; }
            get { return this._colIntL; }
        }
            
        /// <summary>
        /// colIntM
        /// </summary>
        public Int32  colIntM
        {
            set { this._colIntM = value; }
            get { return this._colIntM; }
        }
            
        /// <summary>
        /// colFloatN
        /// </summary>
        public float  colFloatN
        {
            set { this._colFloatN = value; }
            get { return this._colFloatN; }
        }
            
        #endregion
    }
}
  