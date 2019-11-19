using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public class GridOilCellItem : DataGridViewTextBoxCell
    {
        public GridOilCellGroup Group { get; private set; }
        public GridOilColumnType Type { get; private set; }
        private static OilTools tools = new OilTools();
        private string _value;//存储值
        private Color _color = Color.Red;//设置检查和审查的颜色

        public GridOilCellItem(GridOilCellGroup group, GridOilColumnType type)
        {
            Group = group;
            Type = type;
            MaxInputLength = 20;
        }

        //public string DisplayValue
        //{
        //    get
        //    {

        //    }
        //}

        public OilTableRowEntity RowEntity
        {
            get
            {            
                var row = this.OwningRow as GridOilRow;
                if (row != null)
                    return row.RowEntity;
                return null;
            }
        }
        /// <summary>
        /// 数值
        /// </summary>
        public string Value2
        {
            get
            {
                return _value;//用于显示
            }
            set
            {
                var displayValue = value;
                var storageValue = value;
                var row = RowEntity;
                if (row != null)
                {
                    //displayValue = tools.calDataDecLimit(value, row.valDigital);//显示位数
                    //storageValue = tools.calDataDecLimit(value, row.decNumber);//存储位数
                    
                    displayValue = tools.calDataDecLimit(value,row.decNumber, row.valDigital);//显示位数
                    if (row.decNumber != null)
                        storageValue = tools.calDataDecLimit(value, row.decNumber.Value + 3, row.valDigital);//存储位数
                    else
                        storageValue = tools.calDataDecLimit(value, null, row.valDigital);//存储位数
                }
                _value = storageValue;
                Value = displayValue;
                if (RemarkFlag)
                    RemarkFlag = false;
            }
        }
        /// <summary>
        /// 设置单元格的背景色
        /// </summary>
        public Color CellBackgroudColor
        {
            set
            {
                this._color = value;
            }
            get
            {
                return this._color;           
            }
        
        }
        private static Font font = new Font("Tahoma", 7);

        /// <summary>
        /// 提示
        /// </summary>
        public string Tips { get; set; }

        public static StringFormat sf = new StringFormat()
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Near,
        };

        public bool RemarkFlag
        {
            get
            {
                return this.Style.BackColor == this._color;
            }
            set
            {
                if (value)
                {
                    this.Style.BackColor = this._color;
                    this.Style.SelectionForeColor = this._color;
                }
                else
                    this.Style = myStyle.dgdViewCellStyleByRowIndex(RowIndex);
            }
        }
        protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            if (this.IsInEditMode || string.IsNullOrWhiteSpace(Tips))
                return;
            graphics.DrawString(Tips, font, Brushes.Blue, cellBounds, sf);

        }
    }
}
