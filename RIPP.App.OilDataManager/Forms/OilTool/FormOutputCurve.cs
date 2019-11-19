using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using RIPP.OilDB.BLL;
using RIPP.OilDB.BLL.ToolBox;
using ZedGraph;
using System.Drawing.Printing;
using System.IO;
using System.Drawing.Text;
namespace RIPP.App.OilDataManager.Forms.OilTool
{
    /// <summary>
    /// 输出曲线
    /// </summary>
    public enum EnumBackNextOC
    {
        [Description("第0步")]
        None =0,

        [Description("第1步")]
        SelAxis =1,

        [Description("第2步")]
        FillData = 2,

        [Description("第3步")]
        DrawCurev = 4      
    };

    public partial class FormOutputCurve : Form
    {
        private OutputCurvesBll outputCurvesBll = null;
        private EnumBackNextOC _currentStatue = EnumBackNextOC.None;

        StringReader lineReader = null;
        string strPrint = "Hello World";


        public FormOutputCurve()
        {
            InitializeComponent();
            this.zedGraphOutputCurve.ZedExpand.IsopenDIYDrawFrm = true;
            outputCurvesBll = new OutputCurvesBll(this.axisDgv, this.CurveDataDgv, this.zedGraphOutputCurve);
            outputCurvesBll.initAxisDgv();
            outputCurvesBll.initCurveDataDgv();
 
            NextStep();
            printDocument.PrintPage += new PrintPageEventHandler(this.printDocument_PrintPage);
        }
        /// <summary>
        /// 上一步 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemBack_Click(object sender, EventArgs e)
        {
           BackStep();
        }
        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItemNext_Click(object sender, EventArgs e)
        {
            NextStep();
        }
        private void BackStep()
        {
            switch (_currentStatue)
            {
                case EnumBackNextOC.None:
                    break;

                case EnumBackNextOC.SelAxis:               
                    this._currentStatue = EnumBackNextOC.FillData;
                    break;

                case EnumBackNextOC.FillData:
                    this.CurveDataDgv.EndEdit();
                    this.CurveDataDgv.Visible = false;
                    this.axisDgv.Visible = true;                 
                    this.zedGraphOutputCurve.Visible = false;

                    this.toolStripBack.Enabled = false;
                    this.toolStripNext.Enabled = true;

                    _currentStatue = EnumBackNextOC.SelAxis;                  
                    break;

                case EnumBackNextOC.DrawCurev:
                     this.CurveDataDgv.Visible = true;
                    this.axisDgv.Visible = false;
                    this.toolStripBack.Enabled = true;
                    this.toolStripNext.Enabled = true;
                    this.zedGraphOutputCurve.Visible = false;

                    _currentStatue = EnumBackNextOC.FillData;                   
                    break;

            }
        
        }

        private void NextStep()
        {
            switch (_currentStatue)
            {
                case EnumBackNextOC.None:
                    //outputCurvesBll.initAxisDgv();
          
                    this.axisDgv.Visible = true;
                    this.CurveDataDgv.Visible = false;
                    this.zedGraphOutputCurve.Visible = false;

                    this.toolStripBack.Enabled = false;
                    this.toolStripNext.Enabled = true;
                    
                    _currentStatue = EnumBackNextOC.SelAxis;
                    break;

                case EnumBackNextOC.SelAxis:
                    this.axisDgv.EndEdit();
                    //outputCurvesBll.initCurveDataDgv();
                    outputCurvesBll.reFreshAxisList();
                    this.axisDgv.Visible = false;
                    this.CurveDataDgv.Visible = true;
                    this.zedGraphOutputCurve.Visible = false;

                    this.toolStripBack.Enabled = true;
                    this.toolStripNext.Enabled = true;
                  
                    _currentStatue = EnumBackNextOC.FillData;                     
                    break;

                case EnumBackNextOC.FillData:
                    this.CurveDataDgv.EndEdit();
                    this.CurveDataDgv.Visible = false;
                    this.axisDgv.Visible = false;
                    this.zedGraphOutputCurve.Visible = true;
                    this.toolStripBack.Enabled = true;
                    this.toolStripNext.Enabled = false;

                    outputCurvesBll.initZedGraph();
                    outputCurvesBll.drawCurves();
                    _currentStatue = EnumBackNextOC.DrawCurev;
                    break;

                case EnumBackNextOC.DrawCurev:
                    //this.CurveDataDgv.Visible = false;
                    //this.axisDgv.Visible = false;
                    //this.zedGraphOutputCurve.Visible = true;
                    //this.ToolStripMenuItemBack.Enabled = true;
                    //this.ToolStripMenuItemNext.Enabled = false;

                    //_currentStatue = EnumBackNextOC.SelAxis;
                    break;

            }

        }
        
        #region 快捷键
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.CopyToClipboard(this.CurveDataDgv);
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.PasteClipboardValueLimit(this.CurveDataDgv);
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.CopyToClipboard(this.CurveDataDgv);
            GridOilDataEdit.DeleteValues(this.CurveDataDgv);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.DeleteValues(this.CurveDataDgv);
        }
        private void CurveDataDgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.X:
                        GridOilDataEdit.CopyToClipboard(this.CurveDataDgv);
                        GridOilDataEdit.DeleteValues(this.CurveDataDgv);
                        break;
                    case Keys.C:
                        GridOilDataEdit.CopyToClipboard(this.CurveDataDgv);
                        break;
                    case Keys.V:
                        GridOilDataEdit.PasteClipboardValueLimit(this.CurveDataDgv);
                        break;
                    case Keys.Z://撤销数据

                        break;
                    case Keys.Y://重做

                        break;
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                GridOilDataEdit.DeleteValues(this.CurveDataDgv);
            }
        }                   
        #endregion 
      
        #region 打印
      
        /// <summary>
        /// 打印设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打印设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;
            printDialog.ShowDialog();
        }
        /// <summary>
        ///  页面设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 页面设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PageSetupDialog pageSetupDialog = new PageSetupDialog();
            pageSetupDialog.Document = printDocument;
            pageSetupDialog.ShowDialog();
        }
        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打印预览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
        
            try
            {
                printPreviewDialog.ShowDialog();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region
            //PrintDialog printDialog = new PrintDialog();

            //printDialog.Document = printDocument;

            //lineReader = new StringReader(strPrint);

            //if (printDialog.ShowDialog() == DialogResult.OK)
            //{
            //    try
            //    {
            //        printDocument.Print();
            //    }

            //    catch (Exception excep)
            //    {
            //        MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        printDocument.PrintController.OnEndPrint(printDocument, new PrintEventArgs());
            //    }
            //}
            #endregion
            
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {

            Graphics g = e.Graphics; //获得绘图对象

            float linesPerPage = 0; //页面的行号

            float yPosition = 0;   //绘制字符串的纵向位置

            int count = 0; //行计数器

            float leftMargin = e.MarginBounds.Left; //左边距

            float topMargin = e.MarginBounds.Top; //上边距

            string line = null;

            Font printFont = this.Font; //当前的打印字体

            SolidBrush myBrush = new SolidBrush(Color.Black);//刷子

            linesPerPage = e.MarginBounds.Height / printFont.GetHeight(g);//每页可打印的行数

            //逐行的循环打印一页

            while (count < linesPerPage && ((line = lineReader.ReadLine()) != null))
            {

                yPosition = topMargin + (count * printFont.GetHeight(g));

                g.DrawString(line, printFont, myBrush, leftMargin, yPosition, new StringFormat());

                count++;

            }

            // 注意：使用本段代码前，要在该窗体的类中定义lineReader对象：

            //       StringReader lineReader = null;

            //如果本页打印完成而line不为空,说明还有没完成的页面,这将触发下一次的打印事件。在下一次的打印中lineReader会

            //自动读取上次没有打印完的内容，因为lineReader是这个打印方法外的类的成员，它可以记录当前读取的位置

            if (line != null)

                e.HasMorePages = true;

            else
            {

                e.HasMorePages = false;

                // 重新初始化lineReader对象，不然使用打印预览中的打印按钮打印出来是空白页

                lineReader = new StringReader(strPrint); // textBox是你要打印的文本框的内容

            }

        }
        #endregion

        #region 绘制分割线
        const int penLineWidth = 2;
        static Pen penLine = new Pen(Color.FromArgb(0xa0, 0xa0, 0xa0), penLineWidth);

        private int frozenRow = -2;
        private int frozenColumn = -2;
        private object frozenSynch = new object();


        #endregion
        private void CurveDataDgv_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            var cur = sender as DataGridView;

            if (this.CurveDataDgv.RowCount == 0)
                return;
            lock (frozenSynch)
                if (frozenRow == -2)
                {
                    frozenRow = -1;
                    frozenColumn = -1;
                    foreach (DataGridViewRow v in CurveDataDgv.Rows)
                        if (v.Frozen)
                        {
                            if (v.Visible)
                                frozenRow = v.Index;
                        }
                        else
                            break;

                    foreach (DataGridViewColumn v in CurveDataDgv.Columns)
                        if (v.Frozen)
                        {
                            if (v.Visible)
                                frozenColumn = v.Index;
                        }
                        else
                            break;
                }


            if (frozenRow >= 0)
            {
                var posLeft = this.CurveDataDgv.GetCellDisplayRectangle(0, frozenRow, false);
                var posRight = this.CurveDataDgv.GetCellDisplayRectangle(this.CurveDataDgv.ColumnCount - 1, frozenRow, false);
                e.Graphics.DrawLine(penLine, posLeft.Left, posLeft.Bottom - penLineWidth + 2, posRight.Left == 0 ? this.Width : posRight.Right, posLeft.Bottom - penLineWidth + 2);
            }
            if (frozenColumn >= 0)
            {
                var posTop = this.CurveDataDgv.GetCellDisplayRectangle(frozenColumn, 0, false);
                var posBottom = this.CurveDataDgv.GetCellDisplayRectangle(frozenColumn, this.CurveDataDgv.RowCount - 1, false);
                e.Graphics.DrawLine(penLine, posTop.Right - penLineWidth, posTop.Top, posTop.Right - penLineWidth, posBottom.Top == 0 ? this.Height : posBottom.Bottom);

            }
        }

        private void CurveDataDgv_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right
                && this.CurveDataDgv.CurrentCell.RowIndex > 2 && this.CurveDataDgv.CurrentCell.ColumnIndex > 0)
                this.CurveDataDgv.ContextMenuStrip = this.contextMenuStrip1;
            else
                this.CurveDataDgv.ContextMenuStrip = null;
        }

        private void CurveDataDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bool isShow = e.ColumnIndex > 0 && e.RowIndex == 0 ? true : false;
            outputCurvesBll.axisCellCmbBind(isShow);
        }

        private void ToolStripMenuAxisFont_Click(object sender, EventArgs e)
        {           
            //FontFamily fontFamily = new FontFamily("Times New Roman", new PrivateFontCollection ()); 
           
            //Font defaultFont = new System.Drawing.Font(fontFamily,12,FontStyle.Regular);                          
            outputCurvesBll.setFont();
            //FontDialog fontDialog = new FontDialog();
            //fontDialog.Font = defaultFont;
            //if (fontDialog.ShowDialog() == DialogResult.OK)
            //{
                
            //}
        }

        private void ToolStripMenuAxisColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                outputCurvesBll.fontSpec.FontColor = colorDialog.Color;
                outputCurvesBll.setFont();
            }
        }
        OilTools oiltool = new OilTools();

        private void CurveDataDgv_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = this.CurveDataDgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string tempCell = cell.Value as string;
            if (string.IsNullOrWhiteSpace(tempCell))
                return;

            var tempOutputAxis =  (OutputAxisEntity)this.CurveDataDgv.Columns[e.ColumnIndex].Tag;
            if (tempOutputAxis == null)
                return;

            this.CurveDataDgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = tempOutputAxis.iDecNumber == null ?
                tempCell : oiltool.calDataDecLimit(tempCell, tempOutputAxis.iDecNumber.Value);                         
        }

        private void axisDgv_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = this.axisDgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string tempCell = cell.Value as string;
            if (string.IsNullOrWhiteSpace(tempCell))
                return;

            float limit = float.MaxValue;
            int dec = int.MaxValue;

            if (this.CurveDataDgv.Columns[e.ColumnIndex].Name == "DownLimit"
                   || this.CurveDataDgv.Columns[e.ColumnIndex].Name == "UpLimit")
            {
                try
                {
                    limit = float.Parse(tempCell);
                }
                catch
                {
                    limit = float.MaxValue;
                    dec = int.MaxValue;
                }
                finally
                {
                    if (limit == float.MaxValue)
                        this.axisDgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = null;                  
                }
            }
            else if (this.CurveDataDgv.Columns[e.ColumnIndex].Name == "DecNumber")
            {
                try
                {
                    dec = int.Parse(tempCell);
                }
                catch
                {
                    dec = int.MaxValue;
                }
                finally
                {
                    if (limit == float.MaxValue)
                        this.axisDgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = null;
                }
            }         
        }

        
    }
}
