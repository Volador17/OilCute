using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace RIPP.OilDB.UI.GridOil
{
    public class myStyle
    {
        public myStyle()
        {

        }
        public static DataGridViewCellStyle dgdViewCellStyle1()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = Color.Snow;
            style.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            Padding newPadding = new Padding(5, 0, 5, 0);
            style.Padding = newPadding;
            return style;
        }

        public static DataGridViewCellStyle dgdViewCellStyle2()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            style.ForeColor = Color.Black;
            style.BackColor = Color.FromArgb(0xe8,0xe8,0xFF);//Color.LightBlue; //
            style.SelectionBackColor = Color.SkyBlue;
            style.SelectionForeColor = Color.White;
            Padding newPadding = new Padding(5, 0, 5, 0);
            style.Padding = newPadding;
            return style;
        }

        public static DataGridViewCellStyle dgdViewCellStyleByRowIndex(int rowIndex)
        {
            return (rowIndex % 2 == 1) ? dgdViewCellStyle1() : dgdViewCellStyle2();
        }
        public static DataGridViewCellStyle dgdViewCellStyle3()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            style.ForeColor = Color.Black;
            style.BackColor = Color.Beige;
            Padding newPadding = new Padding(5, 0, 5, 0);
            style.Padding = newPadding;
            return style;
        }

        /// <summary>
        /// 错误样式
        /// </summary>
        /// <returns></returns>
        public static DataGridViewCellStyle dgdViewCellStyleErr()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();         
            style.ForeColor = Color.Black;
            style.BackColor = Color.Red;
            return style;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStrip"></param>
        public static void setToolStripStyle(ToolStrip toolStrip)
        {
            toolStrip.BackColor = Color.FromArgb(236, 237, 245);
        }
    }
}
