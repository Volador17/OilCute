/*曹志伟整理OilDataCol实体类*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// OilDataCol实体类
    /// </summary>
    public partial class OilDataSearchColEntity
    {
        #region "Private Variables"
        private Int32 _ID = 0;//查询项目表主键项
        private Int32 _itemOrder = 0;//项目的次序
        private string _OilTableName ;//查询项目列（物性） 
        private Int32 _OilTableColID = 0;//外键，OilTableCol表的ID
        private Boolean _BelongsToRan = true;//判断是否是范围查询
        private Boolean _BelongsToSim = true;//判断是否是范围查询
        private float _ICP = 0;
        private float _ECP = 0;
        #endregion 

        #region "构造函数"
        public OilDataSearchColEntity()
        {
        
        }
        public OilDataSearchColEntity(Int32 id,string oilTableName,Int32 oilTableColID)
        {
            this._ID  = id;
            this._OilTableName = oilTableName;
            this._OilTableColID = oilTableColID;
        }

        public OilDataSearchColEntity(Int32 id, string oilTableName, Int32 oilTableColID, Boolean BelongsToRan, Boolean BelongsToSim)
        {
            this._ID = id;
            this._OilTableName = oilTableName;
            this._OilTableColID = oilTableColID;
            this._BelongsToRan = BelongsToRan;
            this._BelongsToSim = BelongsToSim;
        }
        #endregion

        #region "Public Variables"
        /// <summary>
        /// OilDataCol的ID
        /// </summary>
        public Int32 ID
        {
            get { return  _ID; }
            set { _ID = value; }
        }
        /// <summary>
        /// 项目的排序次序
        /// </summary>
        public Int32 itemOrder
        {
            get { return this._itemOrder; }
            set { this._itemOrder = value; }
        }
        /// <summary>
        /// OilDataCol中的表的名字，即C库中应该具有的表的名字。
        /// </summary>
        public string OilTableName
        {
            get { return _OilTableName; }
            set { _OilTableName = value; }
        }
        /// <summary>
        /// OilDataCol的设置列ID
        /// </summary>
        public Int32 OilTableColID
        {
            get { return _OilTableColID; }
            set { _OilTableColID = value; }
        }
        /// <summary>
        /// 判断是不属于范围查询
        /// </summary>
        public Boolean BelongsToRan
        {
            get { return _BelongsToRan; }
            set { _BelongsToRan = value; }
        }
        /// <summary>
        /// 判断是不属于相似查询
        /// </summary>
        public Boolean BelongsToSim
        {
            get { return _BelongsToSim; }
            set { _BelongsToSim = value; }
        }
        /// <summary>
        /// 判断是不属于相似查询
        /// </summary>
        public float ICP
        {
            get { return this._ICP; }
            set { this._ICP = value; }
        }
        /// <summary>
        /// 判断是不属于相似查询
        /// </summary>
        public float ECP
        {
            get { return this._ECP; }
            set { this._ECP  = value; }
        }
        #endregion
    }
}
