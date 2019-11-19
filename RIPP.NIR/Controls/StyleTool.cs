using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.NIR.Controls
{
   public  class StyleTool
    {

       public static void FormatGrid(ref DataGridView grid)
       {

           DataGridViewCellStyle dataGridViewCellStyle13 = new DataGridViewCellStyle();
           DataGridViewCellStyle dataGridViewCellStyle14 = new DataGridViewCellStyle();

           grid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
           grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;

           dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
           dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
           dataGridViewCellStyle13.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
           dataGridViewCellStyle13.ForeColor = System.Drawing.Color.FromArgb(52, 52, 52);
           dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
           dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
           dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
           grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;

           grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

           grid.EnableHeadersVisualStyles = false;
           grid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));

           grid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
           dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
           dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
           dataGridViewCellStyle14.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
           dataGridViewCellStyle14.ForeColor = System.Drawing.Color.FromArgb(52, 52, 52); dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
           dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
           dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
           grid.RowHeadersDefaultCellStyle = dataGridViewCellStyle14;
           grid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
           grid.RowTemplate.Height = 23;
          
       }

       public static void AddExportBtn(ref DataGridView grid)
       {
           if (grid == null)
               return;
           grid.ContextMenuStrip = new ContextMenuStrip();
           var menuitem1 = new ToolStripMenuItem()
           {
               Text = "导出",
           };
           menuitem1.Click += new EventHandler(menuitem1_Click);
           grid.ContextMenuStrip.Items.Add(menuitem1);
       }

       static void menuitem1_Click(object sender, EventArgs e)
       {
           //throw new NotImplementedException();
           ToolStripMenuItem item = sender as ToolStripMenuItem;
           //item.p
       }
    }
}
