using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using System.Data.SqlClient;
using System.Data;
using RIPP.Lib;

namespace RIPP.OilDB.Data
{
    [Serializable]
    public  class OilTableRowBll : ICollection<OilTableRowEntity>
    {
        public static List<OilTableRowEntity> _OilTableRow = new List<OilTableRowEntity>();
        private static bool _dbNeed = true;

        private OilTableTypeBll _tableBllCache = new OilTableTypeBll();

        private OilTableRowAccess _access = new OilTableRowAccess();

        public OilTableRowBll()
        {
            this.init();
        }

        private void init()
        {
            if (!_dbNeed)
                return;
            //??
            var lst = this._access.Get("1=1");
            lock (_OilTableRow)
            {
                _OilTableRow = lst.OrderBy(d => d.itemOrder).ToList();
                _dbNeed = false;
            }
        }

        /// <summary>
        /// 更新静态存储变量
        /// </summary>
        public void refreshRows()
        {
            var lst = this._access.Get("1=1");
            lock (_OilTableRow)
            {
                _OilTableRow = lst.OrderBy(d => d.itemOrder).ToList();
                _dbNeed = false;
            }
        }

        private int dbAdd(OilTableRowEntity item)
        {
            return this._access.Insert(item);
        }

        private void dbDelete(OilTableRowEntity item)
        {
            this._access.Delete(item.ID);

        }
        private void dbUpdate(OilTableRowEntity item)
        {
            //this._access.Update(item,item.ID.ToString());

            if (item == null)
                return;

            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.CommandText = "UPDATE OilTableRow SET oilTableTypeID =@oilTableTypeID , itemOrder =@itemOrder , " +
                    "itemName =@itemName , itemEnName =@itemEnName , itemUnit =@itemUnit , itemCode =@itemCode, dataType =@dataType , " +
                    "decNumber =@decNumber , valDigital =@valDigital , isKey =@isKey , isDisplay =@isDisplay, trend =@trend , " +
                     "errDownLimit =@errDownLimit , errUpLimit =@errUpLimit ,outExcel =@outExcel , descript =@descript, subItemName =@subItemName, isSystem =@isSystem" +
                    " WHERE ID = @ID";
                cmd.Connection = new SqlConnection(SqlHelper.connectionString);
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = item.ID;
                cmd.Parameters.Add("@oilTableTypeID", SqlDbType.Int).Value = item.oilTableTypeID ;
                cmd.Parameters.Add("@itemOrder", SqlDbType.Int).Value = item.itemOrder;

                cmd.Parameters.Add("@itemName", SqlDbType.VarChar).Value = item.itemName;
                cmd.Parameters.Add("@itemEnName", SqlDbType.VarChar).Value = item.itemEnName;
                cmd.Parameters.Add("@itemUnit", SqlDbType.VarChar).Value = item.itemUnit;
                cmd.Parameters.Add("@itemCode", SqlDbType.VarChar).Value = item.itemCode;
                cmd.Parameters.Add("@dataType", SqlDbType.VarChar).Value = item.dataType;

                if (item.decNumber == null)
                    cmd.Parameters.Add("@decNumber", SqlDbType.Int).Value = DBNull.Value;
                else if (item.decNumber != null)
                    cmd.Parameters.Add("@decNumber", SqlDbType.Int).Value = item.decNumber.Value;

                cmd.Parameters.Add("@valDigital", SqlDbType.Int).Value = item.valDigital;

                cmd.Parameters.Add("@isKey", SqlDbType.Bit).Value = item.isKey;
                cmd.Parameters.Add("@isDisplay", SqlDbType.Bit).Value = item.isDisplay;
                cmd.Parameters.Add("@trend", SqlDbType.VarChar).Value = item.trend;

                if (item.errDownLimit == null)
                    cmd.Parameters.Add("@errDownLimit", SqlDbType.Float).Value = DBNull.Value;
                else if (item.errDownLimit != null)
                    cmd.Parameters.Add("@errDownLimit", SqlDbType.Float).Value = item.errDownLimit.Value;

                if (item.errUpLimit == null)
                    cmd.Parameters.Add("@errUpLimit", SqlDbType.Float).Value = DBNull.Value;
                else if (item.errUpLimit != null)
                    cmd.Parameters.Add("@errUpLimit", SqlDbType.Float).Value = item.errUpLimit.Value;
                
                cmd.Parameters.Add("@outExcel", SqlDbType.Int).Value = (int)item.OutExcel;
                cmd.Parameters.Add("@descript", SqlDbType.VarChar).Value = item.descript;
                cmd.Parameters.Add("@subItemName", SqlDbType.VarChar).Value = item.subItemName;
                cmd.Parameters.Add("@isSystem", SqlDbType.Bit).Value = item.isSystem;
               
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error("OilTableRow表格修改失败!\n\r" + ex.ToString());
            }
            finally
            {
                cmd.Connection.Close();
            }          
        }

        private void beforInertUpdate(ref OilTableRowEntity item)
        {
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
            if (item.oilTableTypeID < 1 &&
                 (item.OilTableType == null || string.IsNullOrWhiteSpace(item.OilTableType.tableName)))
                throw new System.ArgumentNullException("没有设置 oilTableTypeID或OilTableType");

            if (!this._tableBllCache.Contains(item.OilTableType))
                this._tableBllCache.Add(item.OilTableType);
            item.oilTableTypeID = this._tableBllCache[item.OilTableType.tableName].ID;
        }

        //public OilTableRowEntity getByID(int id)
        //{
        //    return _OilTableRow.Where(s => s.ID == id).FirstOrDefault();
        //}

        ///// <summary>
        ///// 根据表类别获取行集合
        ///// </summary>
        ///// <param name="tableType">表类别</param>
        ///// <returns></returns>
        //public List<OilTableRowEntity> getItems(EnumTableType tableType)
        //{
        //    return _OilTableRow.Where(s => s.ID == (int)tableType).ToList();
        //}

        #region ICollection

        public int Count
        {
            get { return _OilTableRow.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(OilTableRowEntity item)
        {
            beforInertUpdate(ref item);
            lock (_OilTableRow)
            {
                item.ID = this.dbAdd(item);
                _OilTableRow.Add(item);
            }
        }

        public void Clear()
        {
            lock (_OilTableRow)
            {
                _OilTableRow.Clear();
            }
           
        }

        public bool Contains(OilTableRowEntity item)
        {
            return _OilTableRow.Select(t => t.itemCode == item.itemCode && t.oilTableTypeID == item.oilTableTypeID).Count() > 0;
        }


        public void Insert(int index, OilTableRowEntity item)
        {
            

            beforInertUpdate(ref item);
            lock (_OilTableRow)
            {
                item.ID = this.dbAdd(item);
                _OilTableRow.Insert(index, item);
            }
        }

        public bool Remove(OilTableRowEntity item)
        {
            lock (_OilTableRow)
            {
                this.dbDelete(item);
                _OilTableRow.Remove(item);
            }
            return true;

        }

        public void Remove(string itemCode, EnumTableType tableType)
        {
            var lst = _OilTableRow.Where(d => d.itemCode == itemCode && d.OilTableType.ID ==(int) tableType).ToArray();
            foreach (var s in lst)
            {
                this.Remove(s);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= _OilTableRow.Count)
                throw new System.ArgumentOutOfRangeException(string.Format("the count of array is {0}, the index is [1}", _OilTableRow.Count, index));
            lock (_OilTableRow)
            {
                this.dbDelete(_OilTableRow[index]);
                _OilTableRow.RemoveAt(index);
            }
        }



        public void CopyTo(OilTableRowEntity[] array, int arrayIndex = 0)
        {
            _OilTableRow.CopyTo(array, arrayIndex);
        }

        public OilTableRowEntity this[int i]
        {
            get
            {
                return _OilTableRow[i];
            }
            set
            {
               
                lock (_OilTableRow)
                {
                    var item = value;
                    beforInertUpdate(ref item);
                    _OilTableRow[i] = value;
                    this.dbUpdate(item);
                }
            }
        }

        public OilTableRowEntity this[int ID, int tableID]
        {
            get
            {
                return _OilTableRow.Where(s => s.ID == ID && s.oilTableTypeID == tableID).FirstOrDefault();
            }
            set
            {
                for (int i = 0; i < _OilTableRow.Count; i++)
                    if (_OilTableRow[i].ID == ID && _OilTableRow[i].oilTableTypeID == tableID)
                    {
                        lock (_OilTableRow)
                        {
                            var item = value;
                            beforInertUpdate(ref item);
                            _OilTableRow[i] = value;
                            this.dbUpdate(item);
                        }
                        break;
                    }
            }

        }

        /// <summary>
        /// 根据代码和表类别获取行
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public OilTableRowEntity this[string itemCode, EnumTableType tableType]
        {
            get
            {
                return _OilTableRow.Where(s => s.itemCode == itemCode && s.OilTableType.ID ==(int) tableType).FirstOrDefault();
            }
            set
            {
                for (int i = 0; i < _OilTableRow.Count; i++)
                    if (_OilTableRow[i].itemCode == itemCode && _OilTableRow[i].OilTableType.ID == (int)tableType)
                    {
                        lock (_OilTableRow)
                        {
                            var item = value;
                            beforInertUpdate(ref item);
                            _OilTableRow[i] = value;
                            this.dbUpdate(item);
                        }
                        break;
                    }
            }
        }



        public IEnumerator<OilTableRowEntity> GetEnumerator()
        {
            for (int i = 0; i < _OilTableRow.Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _OilTableRow.Count; i++)
                yield return this[i];
        }
        #endregion


    }
}
