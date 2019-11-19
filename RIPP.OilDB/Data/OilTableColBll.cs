using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.Data
{
     [Serializable]
    public  class OilTableColBll : ICollection<OilTableColEntity>
    {
        public static List<OilTableColEntity> _OilTableCol = new List<OilTableColEntity>();
        private static bool _dbNeed = true;

        private OilTableTypeBll _tableBllCache = new OilTableTypeBll();
        private OilTableColAccess _access = new OilTableColAccess();

        public OilTableColBll()
        {
            this.init();
        }

        private void init()
        {
            if (!_dbNeed)
                return;

            var lst = this._access.Get("1=1");
            lock (_OilTableCol)
            {
                _OilTableCol = lst.OrderBy(d=>d.colOrder).ToList();
                _dbNeed = false;
            }
        }

        /// <summary>
        /// 更新静态存储变量
        /// </summary>
        public void refreshCols()  
        {
            var lst = this._access.Get("1=1");
            lock (_OilTableCol)
            {
                _OilTableCol = lst.OrderBy(d => d.colOrder).ToList();
                _dbNeed = false;
            }
        }

        private List<OilTableColEntity> dbGet(string sqlWhere)
        {
            return _access.Get(sqlWhere);
        }

        private int dbAdd(OilTableColEntity item)
        {
            return this._access.Insert(item);
        }

        private void dbDelete(OilTableColEntity item)
        {
            if(item!=null)
            this._access.Delete(item.ID);

        }
        private void dbUpdate(OilTableColEntity item)
        {
            this._access.Update(item,item.ID.ToString());
        }

        private void beforInertUpdate(ref OilTableColEntity item)
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

        ///// <summary>
        ///// 根据表类别获取列集合
        ///// </summary>
        ///// <param name="tableType">表类别</param>
        ///// <returns></returns>
        //public List<OilTableColEntity> getItems(EnumTableType tableType)
        //{
        //    return _OilTableCol.Where(s => s.ID == (int)tableType).ToList();
        //}

        #region ICollection

        public int Count
        {
            get { return _OilTableCol.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(OilTableColEntity item)
        {
            beforInertUpdate(ref item);
            lock (_OilTableCol)
            {
                item.ID = this.dbAdd(item);
                _OilTableCol.Add(item);
            }
        }

        public void Clear()
        {
            lock (_OilTableCol)
            {
                _OilTableCol.Clear();
            }
           
        }

        public bool Contains(OilTableColEntity item)
        {
            return _OilTableCol.Select(t => t.colCode == item.colCode && t.oilTableTypeID == item.oilTableTypeID).Count() > 0;
        }

        /// <summary>
        /// 获取实体在列表中的序号
        /// </summary>
        /// <param name="item">序号，找不到-1</param>
        public int IndexOf(OilTableColEntity item)
        {
            int index = -1;
            for (int i = 0; i < this.Count; i++)
                if (this[i].ID == item.ID || this[i].colCode == item.colCode)
                {
                    index = i;
                    break;
                }
            return index;
        }

        public void Insert(int index, OilTableColEntity item)
        {
            

            beforInertUpdate(ref item);
            lock (_OilTableCol)
            {
                item.ID = this.dbAdd(item);
                _OilTableCol.Insert(index, item);
            }
        }

        public bool Remove(OilTableColEntity item)
        {
            lock (_OilTableCol)
            {
                this.dbDelete(item);
                _OilTableCol.Remove(item);
            }
            return true;

        }

        /// <summary>
        /// 根据列代码和表类别删除取列
        /// </summary>
        /// <param name="colCode">列代码</param>
        /// <param name="tableType">表类别</param>  
        public void Remove(string colCode, EnumTableType tableType)
        {
            var lst = _OilTableCol.Where(d => d.colCode == colCode && d.OilTableType.ID == (int)tableType).ToArray();
            foreach (var s in lst)
            {
                this.Remove(s);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= _OilTableCol.Count)
                throw new System.ArgumentOutOfRangeException(string.Format("the count of array is {0}, the index is [1}", _OilTableCol.Count, index));
            lock (_OilTableCol)
            {
                this.dbDelete(_OilTableCol[index]);
                _OilTableCol.RemoveAt(index);
            }
        }

        public void CopyTo(OilTableColEntity[] array, int arrayIndex = 0)
        {
            _OilTableCol.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 根据序号获取列      
        /// <param name="i">序号</param>
        /// <returns>列实体</returns>
        public OilTableColEntity this[int i]
        {
            get
            {
                return _OilTableCol[i];
            }
            set
            {
                lock (_OilTableCol)
                {
                    var item = value;
                    beforInertUpdate(ref item);
                    _OilTableCol[i] = value;
                    this.dbUpdate(item);
                }
            }
        }

        public OilTableColEntity this[int ID, EnumTableType tableType]
        {
            get
            {
                return _OilTableCol.Where(s => s.ID == ID && s.oilTableTypeID == (int)tableType).FirstOrDefault();
            }
            set
            {
                for (int i = 0; i < _OilTableCol.Count; i++)
                    if (_OilTableCol[i].ID == ID && _OilTableCol[i].oilTableTypeID == (int)tableType)
                    {
                        lock (_OilTableCol)
                        {
                            var item = value;
                            beforInertUpdate(ref item);
                            _OilTableCol[i] = value;
                            this.dbUpdate(item);
                        }
                        break;
                    }
            }

        }      

        /// <summary>
        /// 根据列代码和表类别获取列
        /// </summary>
        /// <param name="colCode">列代码</param>
        /// <param name="tableType">表类别</param>
        /// <returns>列实体</returns>
        public OilTableColEntity this[string colCode, EnumTableType tableType]
        {
            get
            {
                return _OilTableCol.Where(s => s.colCode == colCode && s.OilTableType.ID == (int)tableType).FirstOrDefault();
            }
            set
            {
                for (int i = 0; i < _OilTableCol.Count; i++)
                    if (_OilTableCol[i].colCode == colCode && _OilTableCol[i].OilTableType.ID == (int)tableType)
                    {
                        lock (_OilTableCol)
                        {
                            var item = value;
                            beforInertUpdate(ref item);
                            _OilTableCol[i] = value;
                            this.dbUpdate(item);
                        }
                        break;
                    }
            }
        }

        public IEnumerator<OilTableColEntity> GetEnumerator()
        {
            for (int i = 0; i < _OilTableCol.Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _OilTableCol.Count; i++)
                yield return this[i];
        }

        
        #endregion


    }
}
