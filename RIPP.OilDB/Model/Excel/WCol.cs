using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.Excel
{
    public class WCol
    {
        /// <summary>
        /// excel中的列id
        /// </summary>
        private int _colNum;

        private int _ICP;
        private int _ECP;
        private EnumTableType _tableType = EnumTableType.None;
        private List<WCell> _cells = new List<WCell>();


        public WCol()
        {
 
        }

        public WCol(int colNum, int ICP, int ECP)
        {
            this._colNum = colNum;
            this._ICP = ICP;
            this._ECP = ECP;
        }

        public List<WCell> Cells
        {
            get { return _cells; }
            set { _cells = value; }
        }

        public double MCP
        {
            get { return (this._ICP + this._ECP) / 2; }
        }

        public WCell this[int index]
        {
            get { return this._cells[index]; }
            set { this.Add(value); }
        }

        public void Add(WCell value)
        {
            this._cells.Add(value);
        }

        public EnumTableType TableType
        {
            get { return this._tableType; }
            set { this._tableType = value; }
        }

        public int ColNum
        {
            set { this._colNum = value;}
            get { return this._colNum; }
        }

        public int ICP
        {
            set { this._ICP = value; }
            get { return this._ICP; }
        }
        public int ECP
        {
            set { this._ECP = value; }
            get { return this._ECP; }
        }
    }
}
