 /* 曹志伟编写 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// OilDataRow实体类
    /// </summary>
    public partial class OilDataSearchRowEntity
    {
        #region "Private Variables"
        private Int32 _ID = 0;//
        private Int32 _itemOrder = 0;//元素的次序
        private Int32 _OilTableRowID = 0;//外键，OilTableRow表的ID
        private Int32 _OilDataColID = 0; //外键，OilDataCol表的ID
        private Boolean _BelongsToA  = false; // 判断此物性是否属于A库查询（1:是,0不是）
        private Boolean _BelongsToB  = false; // 判断此物性是否属于B库查询（1:是,0不是）
        private Boolean _BelongsToApp = false; // 判断此物性是否属于应用软件查询（1:是,0不是）
        #endregion

        #region "构造函数"
        public OilDataSearchRowEntity()
        {

        }

        public OilDataSearchRowEntity(Int32 ID, Int32 oilTablerowID, Int32 oilDataColID)
        {
            this._ID = ID;
            this._OilTableRowID = oilTablerowID;
            this._OilDataColID = oilDataColID;
        }


        public OilDataSearchRowEntity(Int32 ID, Int32 oilTablerowID, Int32 oilDataColID, Boolean belongsToA, Boolean belongsToB, Boolean belongsToApp)
        {
            this._ID = ID;
            this._OilTableRowID = oilTablerowID;
            this._OilDataColID = oilDataColID;
            this._BelongsToA = belongsToA;
            this._BelongsToB = belongsToB;
            this._BelongsToApp = belongsToApp;
        }

        #endregion


        #region "Public Variables"
        /// <summary>
        /// OilDataRow表的ID
        /// </summary>
        public Int32 ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// OilDataRow的元素次序
        /// </summary>
        public Int32 itemOrder
        {
            get { return _itemOrder; }
            set { _itemOrder = value; }
        }
        /// <summary>
        /// OilTableRow表的相对应的物性对应的行的ID
        /// </summary>
        public Int32 OilTableRowID
        {
            get { return _OilTableRowID; }
            set { _OilTableRowID = value; }
        }
        /// <summary>
        /// OilTableCol表的对应的行的ID
        /// </summary>
        public Int32 OilDataColID
        {
            get { return _OilDataColID; }
            set { _OilDataColID = value; }
        }
        /// <summary>
        /// OilTableRow表的相对应的物性是否属于A库查询（1:是,0不是）
        /// </summary>
        public Boolean BelongsToA
        {
            get { return _BelongsToA; }
            set { _BelongsToA = value; }
        }
        /// <summary>
        /// OilTableRow表的相对应的物性是否属于B库查询（1:是,0不是）
        /// </summary>
        public Boolean BelongsToB
        {
            get { return _BelongsToB; }
            set { _BelongsToB = value; }
        }
        /// <summary>
        /// OilTableRow表的相对应的物性是否属于原油应用查询（1:是,0不是）
        /// </summary>
        public Boolean BelongsToApp
        {
            get { return _BelongsToApp; }
            set { _BelongsToApp = value; }
        }

        #endregion
    }
}
