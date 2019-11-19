using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;

namespace RIPP.OilDB.Model
{
    public class TargetedValueEntity
    {
         #region "Private Variables"
     
        private Int32 _ID =0; // 原油数据表属性（行）
        private Int32 _S_UserID = 0; //  用户的ID
        private Int32 _OilTableTypeComparisonTableID = 0; // 外键，OilTableType表ID(属性所属哪个表)  
        private Int32 _TargetedValueColID = 0; //  TargetedValueCol的ID
        private Int32 _TargetedValueRowID = 0; //  TargetedValueRow的ID
        private float? _Value = null; //   
        private string _strValue = string.Empty;
        #endregion

        public TargetedValueEntity()
        {

        }

        #region "Public Variables"
        
        /// <summary>
        /// 原油数据表属性（行）
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 用户的ID
        /// </summary>
        public Int32 S_UserID
        {
            set { this._S_UserID = value; }
            get { return this._S_UserID; }
        }
        /// <summary>
        /// OilTableTypeComparisonTable表ID(属性所属哪个表)
        /// </summary>
        public Int32 OilTableTypeComparisonTableID
        {
            set { this._OilTableTypeComparisonTableID = value; }
            get { return this._OilTableTypeComparisonTableID; }
        }               
        /// <summary>
        /// TargetedValueCol的ID
        /// </summary>
        public Int32 TargetedValueColID
        {
            set { this._TargetedValueColID = value; }
            get { return this._TargetedValueColID; }
        }
        /// <summary>
        /// TargetedValueRow 
        /// </summary>
        public TargetedValueColEntity TargetedValueCol
        {
            get
            {
                TargetedValueColEntityAccess targetedValueColAccess = new TargetedValueColEntityAccess();
                List<TargetedValueColEntity> targetedValueColList = targetedValueColAccess.Get("1=1");

                TargetedValueColEntity col = targetedValueColList.Where(o =>o.ID == TargetedValueColID).FirstOrDefault();

                return col;
            }
        }   
        /// <summary>
        /// TargetedValueRow的ID
        /// </summary>
        public Int32 TargetedValueRowID
        {
            set { this._TargetedValueRowID = value; }
            get { return this._TargetedValueRowID; }
        }

        /// <summary>
        /// TargetedValueRow 
        /// </summary>
        public TargetedValueRowEntity TargetedValueRow
        {
            get 
            {
                TargetedValueRowEntityAccess targetedValueRowAccess = new TargetedValueRowEntityAccess();
                List<TargetedValueRowEntity> targetedValueRowlList = targetedValueRowAccess.Get("1=1");

                TargetedValueRowEntity row = targetedValueRowlList.Where(o => o.ID == TargetedValueRowID).FirstOrDefault();

                return row;             
            }
        }   

        /// <summary>
        /// 值
        /// </summary>
        public float? fValue
        {
            set { this._Value = value; }
            get { return this._Value; }
        }

        /// <summary>
        /// 值strValue
        /// </summary>
        public string strValue
        {
            set { this._strValue  = value; }
            get { return this._strValue; }
        }  
        #endregion



    }
}
