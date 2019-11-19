using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
   public class OilPropertyEntity
    {
        private string _itemCode = "";    // 属性代码
        private float _fValue = -1;    // 基础值
        private float _weight = -1;      // 权重
        private int _id = -1;//当前属性的Id
        private string _itemName = ""; //属性名称
        private int _colId = 0; //当前的ColId
        public OilPropertyEntity()
        { }
        public OilPropertyEntity(string itemCode,float fValue,float weight)
        {
            this._itemCode = itemCode;
            this._fValue = fValue;
            this._weight = weight;
        }
        public string ItemCode
        {
            get { return _itemCode; }
            set { _itemCode = value; }
        }
        public float Fvalue
        {
            get { return _fValue; }
            set { _fValue = value; }
        }
        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        public int ColId
        {
            get { return _colId; }
            set { _colId = value; }
        }
    }
}
