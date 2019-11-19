using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.NIR
{
    [Serializable]
    public class ComponentList : ICollection<Component>, IDisposable
    {
        private List<Component> _comps = new List<Component>();

        public ComponentList()
        {

        }


        public void Dispose()
        {
            if (this._comps != null)
                foreach (var c in this._comps)
                    c.Dispose();
        }

        #region ICollection

        public int Count
        {
            get { return this._comps.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Contains(Component item)
        {
            if (object.Equals(item, null))
                return false;
            return this.Contains(item.Name);
        }

        public bool Contains(string name)
        {
            return this._comps.Where(c => c.Name == name).Count() > 0;
        }
        public void CopyTo(Component[] array, int arrayIndex = 0)
        {
            this._comps.CopyTo(array, arrayIndex);
        }
        public void Add(Component item)
        {
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
            if(this.Contains(item))
                return;
            this._comps.Add(item);
        }

        public void Clear()
        {
            this._comps.Clear();
        }

        public ComponentList Clone()
        {
            return RIPP.Lib.Serialize.DeepClone<ComponentList>(this);
        }




        public void Insert(int index, Component item)
        {
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
            if (this.Contains(item))
                return;
            this._comps.Insert(index, item);
        }
        public bool Remove(Component item)
        {
            return this._comps.Remove(item);

        }

        public void Remove(string name)
        {
            var lst = this._comps.Where(d => d.Name == name);
            foreach (var s in lst)
            {
                this.Remove(s);
            }
        }

        public void RemoveAt(int index)
        {
            if (index >= this._comps.Count)
                throw new System.ArgumentOutOfRangeException(string.Format("the count of array is {0}, the index is [1}", this._comps.Count, index));
            this._comps.RemoveAt(index);
        }
        public void RemoveAt(IEnumerable<int> slice)
        {
            var newslice = slice.OrderByDescending(s => s);
            foreach (int i in newslice)
            {
                this.RemoveAt(i);
            }

        }

        public Component this[Predicate<Component> f]
        {
            set
            {
                for (int i = 0; i < this._comps.Count; i++)
                    if (f(this[i]))
                        this[i] = value;
            }
        }

        public Component this[IEnumerable<int> slice]
        {
            set
            {
                foreach (int i in slice)
                    this[i] = value;
            }
        }

        public Component this[int i]
        {
            get
            {
                return this._comps[i];
            }
            set
            {
                this._comps[i] = value;
            }
        }

        public Component this[string name]
        {
            get
            {
                return this.Where(s => s.Name == name).FirstOrDefault();
            }
            set
            {
                for (int i = 0; i < this._comps.Count; i++)
                    if (this._comps[i].Name == name)
                    {
                        this._comps[i] = value;
                        break;
                    }
            }

        }



        public IEnumerator<Component> GetEnumerator()
        {
            for (int i = 0; i < this._comps.Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this._comps.Count; i++)
                yield return this[i];
        }
        #endregion

        public int GetIndex(string name)
        {
            var idx = -1;
            for (int i = 0; i < this.Count; i++)
                if (this[i].Name == name)
                    return i;
            return idx;

        }

    }
}
