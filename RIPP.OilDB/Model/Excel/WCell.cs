using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.Excel
{
    public class WCell
    {
        private string _itemCode;
        private string _labData;
        private string _colIndex;
        public WCell() { }
        public WCell(string ItemCode, string LabData, string ColIndex)
        {
            this._labData = LabData;
            this._itemCode = ItemCode;
            this._colIndex = ColIndex;
        }

        public string ItemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }

        public string LabData
        {
            set { this._labData = value; }
            get { return this._labData; }
        }

        public string ColIndex
        {
            set { this._colIndex = value; }
            get { return this._colIndex; }
        }
    }
}
