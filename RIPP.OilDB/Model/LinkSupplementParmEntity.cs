using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class LinkSupplementParmEntity
    {
        #region "Private Variables"
     
        private Int32 _id = 0; // id
        private float _a = 0; // a
        private float _b = 0; // b
        private float _c = 0; // c
        #endregion

        #region "Public Variables"
       
     
        /// <summary>
        /// id
        /// </summary>
        public Int32 ID
        {
            set { this._id = value; }
            get { return this._id; }
        }

        /// <summary>
        /// a
        /// </summary>
        public float a
        {
            set { this._a = value; }
            get { return this._a; }
        }

        /// <summary>
        /// b
        /// </summary>
        public float b
        {
            set { this._b = value; }
            get { return this._b; }
        }

        /// <summary>
        /// c
        /// </summary>
        public float c
        {
            set { this._c = value; }
            get { return this._c; }
        }

        #endregion
    }
}
