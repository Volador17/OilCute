using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.Data
{
     [Serializable]
    public  class OilTableTypeBll : ICollection<OilTableTypeEntity>
    {
        private static List<OilTableTypeEntity> _OilTableType = new List<OilTableTypeEntity>();
        private static bool _dbNeed = true;
        private OilTableTypeAccess _access = new OilTableTypeAccess();
         
        public OilTableTypeBll()
        {
            this.init();
        }

        private void init()
        {
            if (!_dbNeed)
                return;

            var lst = _access.Get("1=1");
            lock (_OilTableType)
            {
                _OilTableType = lst;
                _dbNeed = false;
            }

        }

        private List<OilTableTypeEntity> dbGet(string sqlWhere)
        {
            return _access.Get(sqlWhere);
        }

        private int dbAdd(OilTableTypeEntity item)
        {
            return this._access.Insert(item);
        }

        private void dbDelete(OilTableTypeEntity item)
        {
            if (item != null)
                this._access.Delete(item.ID);

        }
        public void dbUpdate(OilTableTypeEntity item)
        {
            for (int i = 0; i < _OilTableType.Count; i++)
                if (_OilTableType[i].ID == item.ID)
                {
                    lock (_OilTableType)
                    {
                        _OilTableType[i] = item;
                        this._access.Update(item, item.ID.ToString());
                    }
                    break;
                }
           
        }
       

        #region ICollection

        public int Count
        {
            get { return _OilTableType.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(OilTableTypeEntity item)
        {
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
            if (this.Contains(item))
                return;
            lock (_OilTableType)
            {
                item.ID = this.dbAdd(item);
                _OilTableType.Add(item);

            }
        }

        public void Clear()
        {
            lock (_OilTableType)
            {
                _OilTableType.Clear();
            }
           
        }

        public bool Contains(OilTableTypeEntity item)
        {
            return _OilTableType.Select(t => t.tableName == item.tableName ).Count() > 0;
        }


        public void Insert(int index, OilTableTypeEntity item)
        {
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
           // item = this._beforeAddOrInsert(item);
            if (this.Contains(item))
                return;
            lock (_OilTableType)
            {
                item.ID = this.dbAdd(item);
                _OilTableType.Insert(index, item);
            }
        }

        public bool Remove(OilTableTypeEntity item)
        {
            lock (_OilTableType)
            {
                this.dbDelete(item);
                _OilTableType.Remove(item);
            }
            return true;

        }

        public void Remove(string name)
        {
            var lst = _OilTableType.Where(d => d.tableName == name).ToArray();
            foreach (var s in lst)
            {
                this.Remove(s);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= _OilTableType.Count)
                throw new System.ArgumentOutOfRangeException(string.Format("the count of array is {0}, the index is [1}", _OilTableType.Count, index));
            lock (_OilTableType)
            {
                this.dbDelete(_OilTableType[index]);
                _OilTableType.RemoveAt(index);
            }
        }


        public void CopyTo(OilTableTypeEntity[] array, int arrayIndex = 0)
        {
            _OilTableType.CopyTo(array, arrayIndex);
        }

        public OilTableTypeEntity this[int i]
        {
            get
            {
                return _OilTableType[i];
            }
            set
            {
                lock (_OilTableType)
                {
                    _OilTableType[i] = value;
                    this.dbUpdate(value);
                }
            }
        }

        public OilTableTypeEntity this[string name]
        {
            get
            {
                return _OilTableType.Where(s => s.tableName == name).FirstOrDefault();
            }
            set
            {
                for (int i = 0; i < _OilTableType.Count; i++)
                    if (_OilTableType[i].tableName == name)
                    {
                        lock (_OilTableType)
                        {
                            _OilTableType[i] = value;
                            this.dbUpdate(value);
                        }
                        break;
                    }
            }

        }



        public IEnumerator<OilTableTypeEntity> GetEnumerator()
        {
            for (int i = 0; i < _OilTableType.Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < _OilTableType.Count; i++)
                yield return this[i];
        }
        #endregion


    }
}

