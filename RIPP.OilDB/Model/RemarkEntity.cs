using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    ///<summary>
    ///RemarkEntity实体类(Remark)
    ///</summary>
    [Serializable]
    public partial class RemarkEntity
    {
        #region "Private Variables"
        private Int32 _ID = 0; // 原油数据，数据字段包括实测值和最后一次修改值，历史修改值放在OilDataModify表。需要时再加载。
        private Int32 _oilInfoID = 0; // 外键，原油信息ID(OilInfo表的ID,可以表示一个批次样本)
        private Int32 _oilTableColID = 0; // 数据表列的ID
        private Int32 _oilTableRowID = 0; // oilTableRowID
        private String _CalRemark = ""; //  文本数据 
        private String _LabRemark = ""; //  文本数据 

        #endregion

        public RemarkEntity()
        {

        }

        #region "Public Variables"
       
        /// <summary>
        /// 原油数据，数据字段包括实测值和最后一次修改值，历史修改值放在OilDataModify表。需要时再加载。
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 外键，原油信息ID(OilInfo表的ID,可以表示一个批次样本)
        /// </summary>
        public Int32  oilInfoID
        {
            set { this._oilInfoID = value; }
            get { return this._oilInfoID; }
        }
            
        /// <summary>
        /// 数据表列的ID
        /// </summary>
        public Int32  oilTableColID
        {
            set { this._oilTableColID = value; }
            get { return this._oilTableColID; }
        }
            
        /// <summary>
        /// oilTableRowID
        /// </summary>
        public Int32  oilTableRowID
        {
            set { this._oilTableRowID = value; }
            get { return this._oilTableRowID; }
        }
            
        /// <summary>
        ///  校正列批注文本数据 
        /// </summary>
        public String CalRemark
        {
            set { this._CalRemark = value; }
            get { return this._CalRemark; }
        }

        /// <summary>
        ///  实测列批注文本数据 
        /// </summary>
        public String LabRemark
        {
            set { this._LabRemark = value; }
            get { return this._LabRemark; }
        }
            
        #endregion

    }
}
