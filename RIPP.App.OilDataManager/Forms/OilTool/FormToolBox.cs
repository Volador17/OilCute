using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using RIPP.OilDB.Data;
using System.Globalization;
using RIPP.App.OilDataManager.Properties;

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FormToolBox : Form
    {
        #region 基础变量

        protected Color RowTitleColor = Color.SaddleBrown;
        protected Color BCellColor = Color.Blue;
        //protected string pattern = @"^[1-9][0-9]*(\.[0-9]+)?$";  //判断非数字的正则表达式
        protected string pattern = @"^[+-]?((\.[0-9]*[1-9][0-9]*)|([0-9]+\.[0-9]*[0-9][0-9]*)|([0-9]*[0-9][0-9]*\.[0-9]+)|([0-9]*[0-9][0-9]*))$";
        private int commonWidth = 50;
        private int commonHeight = 36;
        private int intervalWidth = 80;
        #endregion

        #region 构造函数、初始化
        public FormToolBox()
        {
            InitializeComponent();
        }

        private void FormToolBox_Load(object sender, EventArgs e)
        {
            int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度

            int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度

            // this.Location = new Point(xWidth / 2 + 240, yHeight / 2 - 370);//这里需要再减去窗体本身的宽度和高度的一半
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = 40;

            //this.TopMost = true;//工具箱窗口一直位于最前端

            this.panel1.Visible = true;
            this.panel2.Visible = false;

            #region 单位换算
            BindDataT(); //温度
            //BindDataPressure();//压力
            //BindDataDensity();//浓度
            //BindDataQ();
            #endregion

            #region  物性关联
            //BindDataMiDu();
            //BindDataAcid();
            //BindDataViscosity();
            //BindDataMixViscosity();
            //BindDataProperty();
            //BindDataOilType();
            //BindDataDistill();
            //BindDataVapour();
            //BindDataCetane();
            //BindDataFreez();
            //BindDataAniline();
            //BindDataMol();
            //BindDataCH();
            //BindDataBMCI();
            //BindDataFour();
            //BindDataWax();
            //BindDataResidual();
            //BindDataVisPara();
            #endregion
        }
        #endregion

        #region 单位换算

        #region 重绘标签
        /// <summary>
        /// 重绘标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbcUnit_DrawItem(object sender, DrawItemEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            //   Draw   the   text 
            e.Graphics.DrawString(((TabControl)sender).TabPages[e.Index].Text,
            System.Windows.Forms.SystemInformation.MenuFont,
            new SolidBrush(Color.Black),
            e.Bounds,
            sf);
        }
        #endregion

        #region 单位换算--温度

        List<string> slT = new List<string>();

        /// <summary>
        /// 绑定温度列表
        /// </summary>
        private void BindDataT()
        {
            this.lblTE.Text = "";

            this.dgvT.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvT.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvT, "℃");
            this.dgvT.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvT, " K");
            this.dgvT.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvT, "℉");
            this.dgvT.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvT, " R");
            this.dgvT.Rows.Add(rowT);

            this.dgvT.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvT.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvT.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvT.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 30;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        /// <summary>
        /// 单位换算--温度--清除所有单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiClearT_Click(object sender, EventArgs e)
        {
            this.dgvT.EndEdit();
            TClear();
        }

        private void TClear()
        {
            this.dgvT.Rows.Clear();
            this.dgvT.Columns.Clear();
            slT.Clear();
            this.lblTE.Text = "";
            BindDataT();
        }

        /// <summary>
        /// 单位换算--温度--计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiComT_Click(object sender, EventArgs e)
        {
            this.dgvT.EndEdit();
            TComp();
        }

        private void TComp()
        {
            ClearDgv(this.dgvT, slT);

            this.lblTE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvT.Columns.Count; i++)
            {
                int isMutiValue = 0;//作为判断是否一列出现两个输入

                string tempData = "";  //临时存放每列的输入数据

                examInputCount(this.dgvT, this.lblTE, i, 1, out isMutiValue, out tempData); //输入参数个数判断

                //判断一列是否出现两个值，出现则不计算此列
                if (isMutiValue > 1)
                {
                    //不计算
                    //汇总此列
                    this.lblTE.Text += "第" + i + "列输入参数个数不对；";
                }
                else if (isMutiValue != 0)   //列中有输入数据才计算
                {
                    bool isExistRightRow0 = false;
                    int inputRow = 0; //输入行
                    string inputData = ""; //输入数据
                    int flag = examInputFormat(this.dgvT, lblTE, i, tempData, out isExistRightRow0, out inputRow, out inputData, 1);
                    if (flag == 0)
                    { continue; }

                    #region 开始计算
                    //不跳出，说明输入数字合法，调用计算函数
                    int r = 0;  //行号
                    double c = 9999999;


                    switch (inputRow)
                    {
                        case 0:
                            c = ReTComputer(0, i, inputData, isExistRightRow0, this.dgvT); //已知第0行帕Pa
                            TComputer(i, r, 0, c, this.dgvT, isExistRightRow0, slT);
                            break;
                        case 1:
                            c = ReTComputer(1, i, inputData, isExistRightRow0, this.dgvT); //已知第1行巴bar
                            TComputer(i, r, 1, c, this.dgvT, isExistRightRow0, slT);
                            break;
                        case 2:
                            c = ReTComputer(2, i, inputData, isExistRightRow0, this.dgvT); //已知第2行psi
                            TComputer(i, r, 2, c, this.dgvT, isExistRightRow0, slT);
                            break;
                        case 3:
                            c = ReTComputer(3, i, inputData, isExistRightRow0, this.dgvT); //已知第3行大气压atm
                            TComputer(i, r, 3, c, this.dgvT, isExistRightRow0, slT);
                            break;
                    }

                    #endregion
                }
            }

            //错误信息悬浮方法
            tooltipEContent(this.lblTE);

            #endregion
        }

        //行标头添加序列
        private void dgvT_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
          e.RowBounds.Location.Y,
          this.dgvT.RowHeadersWidth - 4,
          e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        //选项事件
        private void tsmiChoT_Click(object sender, EventArgs e)
        {
            TCho();
        }
        private void TCho()
        {
            this.contextMenuStrip2.Show(this.menuStripT, new Point(this.tsmiChoT.Width + intervalWidth, this.tsmiChoT.Height)); ;
        }

        //单击编辑单元格
        private void dgvT_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvT.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvT.CurrentCell = this.dgvT.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvT.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //悬浮文字
        private void lblTE_MouseHover(object sender, EventArgs e)
        {
            if (lblTE.Text.Length > 35)
            {
                this.lblTE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }

        }
        #endregion

        #region 单位换算--压力

        List<string> slPressure = new List<string>();
        //单击编辑单元格
        private void dgvPressure_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvPressure.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvPressure.CurrentCell = this.dgvPressure.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvPressure.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //单位换算--压力计算
        private void tsmiComPressure_Click(object sender, EventArgs e)
        {
            this.dgvPressure.EndEdit();
            PressureCom();
        }

        private void PressureCom()
        {
            ClearDgv(this.dgvPressure, slPressure);

            this.lblPressureE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvPressure.Columns.Count; i++)
            {
                int isMutiValue = 0;//作为判断是否一列出现两个输入

                string tempData = "";  //临时存放每列的输入数据

                examInputCount(this.dgvPressure, this.lblPressureE, i, 1, out isMutiValue, out tempData); //输入参数个数判断


                //判断一列是否出现两个值，出现则不计算此列
                if (isMutiValue > 1)
                {
                    //不计算
                    //汇总此列
                    this.lblPressureE.Text += "第" + i + "列输入参数个数不对；";
                }
                else if (isMutiValue != 0)   //列中有输入数据才计算
                {
                    bool isExistRightRow0 = false;
                    int inputRow = 0; //输入行
                    string inputData = ""; //输入数据
                    int flag = examInputFormat(this.dgvPressure, lblPressureE, i, tempData, out isExistRightRow0, out inputRow, out inputData, 2);
                    if (flag == 0)
                    { continue; }

                    #region 开始计算
                    //不跳出，说明输入数字合法，调用计算函数
                    int r = 0;  //行号
                    double pa = 0.0;


                    switch (inputRow)
                    {
                        case 0:
                            pa = RePressureComputer(0, i, inputData, isExistRightRow0, this.dgvPressure); //已知第0行帕Pa
                            PressureComputer(i, r, 0, pa, this.dgvPressure, isExistRightRow0, slPressure);
                            break;
                        case 1:
                            pa = RePressureComputer(1, i, inputData, isExistRightRow0, this.dgvPressure); //已知第1行巴bar
                            PressureComputer(i, r, 1, pa, this.dgvPressure, isExistRightRow0, slPressure);
                            break;
                        case 2:
                            pa = RePressureComputer(2, i, inputData, isExistRightRow0, this.dgvPressure); //已知第2行psi
                            PressureComputer(i, r, 2, pa, this.dgvPressure, isExistRightRow0, slPressure);
                            break;
                        case 3:
                            pa = RePressureComputer(3, i, inputData, isExistRightRow0, this.dgvPressure); //已知第3行大气压atm
                            PressureComputer(i, r, 3, pa, this.dgvPressure, isExistRightRow0, slPressure);
                            break;
                        case 4:
                            pa = RePressureComputer(4, i, inputData, isExistRightRow0, this.dgvPressure); //已知第4行汞柱mmHg
                            PressureComputer(i, r, 4, pa, this.dgvPressure, isExistRightRow0, slPressure);
                            break;
                        case 5:
                            pa = RePressureComputer(5, i, inputData, isExistRightRow0, this.dgvPressure); //已知第5行水柱mH2O
                            PressureComputer(i, r, 5, pa, this.dgvPressure, isExistRightRow0, slPressure);
                            break;
                        case 6:
                            pa = RePressureComputer(6, i, inputData, isExistRightRow0, this.dgvPressure); //已知第6行千克力kgf/cm2
                            PressureComputer(i, r, 6, pa, this.dgvPressure, isExistRightRow0, slPressure);
                            break;
                    }
                    #endregion

                }
            }

            //错误信息内容悬浮显示
            tooltipEContent(this.lblPressureE);
            #endregion
        }

        //清除所有单元格
        private void tsmiClearPressure_Click(object sender, EventArgs e)
        {
            PressureClear();
        }

        private void PressureClear()
        {
            this.dgvPressure.Rows.Clear();
            this.dgvPressure.Columns.Clear();
            slPressure.Clear();
            this.lblPressureE.Text = "";
            BindDataPressure();
        }

        //选项事件      
        private void tsmiChoPressure_Click(object sender, EventArgs e)
        {
            PressureCho();
        }
        private void PressureCho()
        {
            this.contextMenuStrip2.Show(this.menuStripPressure, new Point(this.tsmiChoPressure.Width + intervalWidth, this.tsmiChoPressure.Height));
        }

        //错误信息悬浮
        private void lblPressureE_MouseHover(object sender, EventArgs e)
        {
            if (lblPressureE.Text.Length > 35)
            {
                this.lblPressureE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        /// <summary>
        /// 绑定压力列表
        /// </summary>
        private void BindDataPressure()
        {
            this.lblPressureE.Text = "";

            this.dgvPressure.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvPressure.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvPressure, "帕Pa");
            this.dgvPressure.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvPressure, "巴bar");
            this.dgvPressure.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvPressure, "psi");
            this.dgvPressure.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvPressure, "大气压atm");
            this.dgvPressure.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvPressure, "汞柱mmHg");
            this.dgvPressure.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvPressure, "水柱mH2O");
            this.dgvPressure.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvPressure, "千克力kgf/cm2");
            this.dgvPressure.Rows.Add(rowT);

            this.dgvPressure.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvPressure.Columns[0].ReadOnly = true;


            foreach (DataGridViewRow c in this.dgvPressure.Rows)
            {
                c.Height = commonHeight;
            }

            foreach (DataGridViewColumn c in this.dgvPressure.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Frozen = true;
                    c.Width = 52;
                }
                else
                    c.Width = commonWidth;
            }
        }

        /// <summary>
        /// 行标头添加序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvPressure_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
           e.RowBounds.Location.Y,
           this.dgvPressure.RowHeadersWidth - 4,
           e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvPressure.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvPressure.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region 单位转换--浓度

        List<string> slDensity = new List<string>();

        /// <summary>
        /// 绑定压力列表
        /// </summary>
        private void BindDataDensity()
        {
            this.lblDensityE.Text = "";

            this.dgvDensity.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvDensity.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDensity, "20℃密度 g/cm3");
            this.dgvDensity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDensity, "μg/g");
            this.dgvDensity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDensity, "PPM");
            this.dgvDensity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDensity, "%");
            this.dgvDensity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDensity, "ng/g");
            this.dgvDensity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDensity, "mg/L");
            this.dgvDensity.Rows.Add(rowT);

            this.dgvDensity.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvDensity.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvDensity.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvDensity.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 60;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComDensity_Click(object sender, EventArgs e)
        {
            this.dgvDensity.EndEdit();
            DensityCom();
        }

        private void DensityCom()
        {
            ClearDgv(this.dgvDensity, slDensity); //清除单元格

            this.lblDensityE.Text = "";

            #region 判断输入格式和计算
            for (int i = 1; i < this.dgvDensity.Columns.Count; i++)//循环每一列
            {
                int isMutiValue = 0;//作为判断是否一列出现两个输入

                string tempData = "";  //临时存放每列的输入数据

                examInputCount(this.dgvDensity, this.lblDensityE, i, 0, out isMutiValue, out tempData); //输入参数个数判断

                //判断一列中的2-6行是否出现多个值，出现则不计算此列
                if (isMutiValue > 1)
                {
                    //不计算
                    //汇总此列
                    this.lblDensityE.Text += "第" + i + "列输入参数个数不对；";
                }
                else if (isMutiValue == 0 && this.dgvDensity.Rows[1].Cells[i].Value != null)//已知Ug/g，求ppm、%、ng/g
                {
                    string ugg=this.dgvDensity.Rows[1].Cells[i].Value.ToString().Trim();
                    if (Regex.IsMatch(ugg, pattern))
                    {
                        double Ugg = Convert.ToDouble(ugg);
                        double Ppm = Ugg;
                        double Percent = Ugg / 10000;
                        double Ngg = Ugg * 1000;
                        this.dgvDensity[i, 2].Value = Ppm;
                        this.dgvDensity[i, 3].Value = Percent;
                        this.dgvDensity[i, 4].Value = Ngg;
                        slDensity.Add(i + "|" + 2);
                        slDensity.Add(i + "|" + 3);
                        slDensity.Add(i + "|" + 4);
                        this.dgvDensity.Rows[1].Cells[i].Style.ForeColor = BCellColor;
                    }
                    else
                    {
                        lblDensityE.Text += "第" + i + "列ug/g为非数字";
                    }
                }
                else if (isMutiValue != 0)   //列中有输入数据才计算
                {
                    bool isExistRightRow0 = false;
                    int inputRow = 0; //输入行
                    string inputData = ""; //输入数据
                    int flag = examInputFormat(this.dgvDensity, lblDensityE, i, tempData, out isExistRightRow0, out inputRow, out inputData, 0);
                    if (flag == 0)
                    { continue; }

                    #region 开始计算
                    int r = 1;
                    double ugg = 0.0;

                    switch (inputRow)
                    {
                        case 1:
                            ugg = ReDensityComputer(1, i, inputData, isExistRightRow0, this.dgvDensity); //已知第一行ugg
                            DensityComputer(i, r, 1, ugg, this.dgvDensity, isExistRightRow0, slDensity);
                            break;
                        case 2:
                            ugg = ReDensityComputer(2, i, inputData, isExistRightRow0, this.dgvDensity); //已知第2行PPM
                            DensityComputer(i, r, 2, ugg, this.dgvDensity, isExistRightRow0, slDensity);
                            break;
                        case 3:
                            ugg = ReDensityComputer(3, i, inputData, isExistRightRow0, this.dgvDensity); //已知第3行%
                            DensityComputer(i, r, 3, ugg, this.dgvDensity, isExistRightRow0, slDensity);
                            break;
                        case 4:
                            ugg = ReDensityComputer(4, i, inputData, isExistRightRow0, this.dgvDensity); //已知第4行ng/g
                            DensityComputer(i, r, 4, ugg, this.dgvDensity, isExistRightRow0, slDensity);
                            break;
                        case 5:
                            ugg = ReDensityComputer(5, i, inputData, isExistRightRow0, this.dgvDensity); //已知第5行mg/l
                            DensityComputer(i, r, 5, ugg, this.dgvDensity, isExistRightRow0, slDensity);
                            break;
                    }
                    #endregion
                }

                if (isMutiValue == 0 && this.dgvDensity.Rows[0].Cells[i].Value != null && this.dgvDensity.Rows[1].Cells[i].Value != null)
                {
                    string D20 = this.dgvDensity[i, 0].Value.ToString().Trim();
                    string Ugg = this.dgvDensity[i, 1].Value.ToString().Trim();
                    if (Regex.IsMatch(D20, pattern) && Regex.IsMatch(Ugg, pattern))
                    {
                        double d20 = Convert.ToDouble(D20);
                        double ugg = Convert.ToDouble(Ugg);
                        double mgl = d20 * ugg;
                        this.dgvDensity[i, 5].Value = mgl;
                        slDensity.Add(i + "|" + 5);
                    }
                    else
                    {
                        lblDensityE.Text += "第" + i + "列D20或ug/g为非数字";
                    }
                }
            }

            tooltipEContent(this.lblDensityE);
            #endregion
        }

        /// <summary>
        /// 错误信息悬浮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblDensityE_MouseHover(object sender, EventArgs e)
        {
            if (lblDensityE.Text.Length > 35)
            {
                this.lblDensityE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        /// <summary>
        /// 清除表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiClearDensity_Click(object sender, EventArgs e)
        {
            DensityClear();
        }

        private void DensityClear()
        {
            this.dgvDensity.Rows.Clear();
            this.dgvDensity.Columns.Clear();
            slDensity.Clear();
            this.lblDensityE.Text = "";
            BindDataDensity();
        }

        /// <summary>
        /// 弹出快捷框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiChoDensity_Click(object sender, EventArgs e)
        {
            DensityCho();
        }
        private void DensityCho()
        {
            this.contextMenuStrip2.Show(this.menuStripDensity, new Point(this.tsmiChoDensity.Width + intervalWidth, this.tsmiChoDensity.Height));
        }

        /// <summary>
        /// 行标头添加序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDensity_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
          e.RowBounds.Location.Y,
          this.dgvDensity.RowHeadersWidth - 4,
          e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvDensity.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvDensity.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void dgvDensity_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvDensity.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvDensity.CurrentCell = this.dgvDensity.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvDensity.BeginEdit(true);//将单元格设为编辑状态
            }
        }
        #endregion

        #region 单位换算--质量

        List<string> slQ = new List<string>();

        /// <summary>
        /// 绑定质量列表
        /// </summary>
        private void BindDataQ()
        {
            this.lblQE.Text = "";

            this.dgvQ.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvQ.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowQ = new DataGridViewRow();
            rowQ.CreateCells(this.dgvQ, "20℃密度 g/cm3");
            this.dgvQ.Rows.Add(rowQ);
            rowQ = new DataGridViewRow();
            rowQ.CreateCells(this.dgvQ, "公斤");
            this.dgvQ.Rows.Add(rowQ);
            rowQ = new DataGridViewRow();
            rowQ.CreateCells(this.dgvQ, "吨");
            this.dgvQ.Rows.Add(rowQ);
            rowQ = new DataGridViewRow();
            rowQ.CreateCells(this.dgvQ, "磅");
            this.dgvQ.Rows.Add(rowQ);
            rowQ = new DataGridViewRow();
            rowQ.CreateCells(this.dgvQ, "升");
            this.dgvQ.Rows.Add(rowQ);
            rowQ = new DataGridViewRow();
            rowQ.CreateCells(this.dgvQ, "桶");
            this.dgvQ.Rows.Add(rowQ);
            rowQ = new DataGridViewRow();
            rowQ.CreateCells(this.dgvQ, "美加仑");
            this.dgvQ.Rows.Add(rowQ);
            rowQ = new DataGridViewRow();
            rowQ.CreateCells(this.dgvQ, "英加仑");
            this.dgvQ.Rows.Add(rowQ);

            this.dgvQ.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvQ.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvQ.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvQ.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 60;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComQ_Click(object sender, EventArgs e)
        {
            this.dgvQ.EndEdit();
            QCom();
        }

        Dictionary<string, string> QDc = new Dictionary<string, string>();
        private void QCom()
        {
            ClearDgv(this.dgvQ, slQ);

            this.lblQE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvQ.Columns.Count; i++)//循环每一列
            {
                QDc.Clear();
                for (int j = 0; j < this.dgvQ.Rows.Count; j++)
                {
                    if (this.dgvQ[i, j].Value != null && this.dgvQ[i, j].Value.ToString() != "")
                    {
                        QDc.Add(((Q)j).ToString(), this.dgvQ[i, j].Value.ToString());
                        this.dgvQ[i, j].Style.ForeColor = BCellColor;
                    }
                }


                int isMutiValue = 0;//作为判断是否一列出现两个输入

                string tempData = "";  //临时存放每列的输入数据

                examInputCount(this.dgvQ, this.lblQE, i, 0, out isMutiValue, out tempData);

                //判断一列是否出现两个值，出现则不计算此列
                if (isMutiValue > 1)
                {
                    //不计算
                    //汇总此列
                    //this.lblQE.Text += "第" + i + "列输入参数个数不对；";
                }
                if (isMutiValue == 0 && this.dgvQ.Rows[1].Cells[i].Value != null && this.dgvQ.Rows[0].Cells[i].Value != null && this.dgvQ.Rows[0].Cells[i].Value.ToString() != string.Empty)
                {
                    this.dgvQ[i, 0].Style.ForeColor = BCellColor;
                    this.dgvQ[i, 1].Style.ForeColor = BCellColor;
                    string d20 = this.dgvQ[i, 0].Value.ToString().Trim();
                    string kg = this.dgvQ[i, 1].Value.ToString().Trim();
                    if (Regex.IsMatch(d20, pattern) && Regex.IsMatch(kg, pattern))
                    {
                        double D20 = Convert.ToDouble(d20);
                        double Kg = Convert.ToDouble(kg);
                        double T = Kg / 1000;
                        double P = Kg * 2.205;
                        double L = Kg / D20 / 1000;
                        double B = Kg / D20 / 158.98;
                        double AG = Kg / D20 / 3.785;
                        double BG = Kg / D20 / 4.546;


                        this.dgvQ[i, 2].Value = T;
                        this.dgvQ[i, 3].Value = P;
                        this.dgvQ[i, 4].Value = L;
                        this.dgvQ[i, 5].Value = B;
                        this.dgvQ[i, 6].Value = AG;
                        this.dgvQ[i, 7].Value = BG;

                        slQ.Add(i + "|" + 2); 
                        slQ.Add(i + "|" + 3); 
                        slQ.Add(i + "|" + 4); 
                        slQ.Add(i + "|" + 5); 
                        slQ.Add(i + "|" + 6); 
                        slQ.Add(i + "|" + 7);
                    }
                    else
                    {
                        this.lblQE.Text += "第" + i + "列D20或者Kg为非数字；";
                    }
                }
                if (isMutiValue == 0 && this.dgvQ.Rows[1].Cells[i].Value != null && (this.dgvQ.Rows[0].Cells[i].Value == null || this.dgvQ.Rows[0].Cells[i].Value.ToString() == string.Empty))
                {
                    this.dgvQ[i, 1].Style.ForeColor = BCellColor;
                    string kg = this.dgvQ[i, 1].Value.ToString().Trim();
                    if (Regex.IsMatch(kg, pattern))
                    {
                        double Kg = Convert.ToDouble(kg);
                        double T = Kg / 1000;
                        double P = Kg * 2.205;
                        this.dgvQ[i, 2].Value = T;
                        this.dgvQ[i, 3].Value = P;
                        slQ.Add(i + "|" + 2);
                        slQ.Add(i + "|" + 3);
                    }
                    else
                    {
                        this.lblQE.Text += "第" + i + "列Kg为非数字；";
                    }
                }

                if (isMutiValue == 1)   //列中有输入数据才计算
                {
                    bool isExistRightRow0 = false;
                    int inputRow = 0; //输入行
                    string inputData = ""; //输入数据
                    int flag = examInputFormat(this.dgvQ, lblQE, i, tempData, out isExistRightRow0, out inputRow, out inputData, 3);
                    if (flag == 0)
                    { continue; }

                    #region 开始计算
                    int r = 1;
                    double kg = 0.0;

                    switch (inputRow)
                    {
                        case 1:
                            kg = ReQComputer(1, i, inputData, isExistRightRow0, this.dgvQ); //已知第一行kg
                            QComputer(i, r, 1, kg, this.dgvQ, isExistRightRow0, slQ);
                            break;
                        case 2:
                            kg = ReQComputer(2, i, inputData, isExistRightRow0, this.dgvQ); //已知第2行T
                            QComputer(i, r, 2, kg, this.dgvQ, isExistRightRow0, slQ);
                            break;
                        case 3:
                            kg = ReQComputer(3, i, inputData, isExistRightRow0, this.dgvQ); //已知第3行
                            QComputer(i, r, 3, kg, this.dgvQ, isExistRightRow0, slQ);
                            break;
                        case 4:
                            kg = ReQComputer(4, i, inputData, isExistRightRow0, this.dgvQ); //已知第4行
                            QComputer(i, r, 4, kg, this.dgvQ, isExistRightRow0, slQ);
                            break;
                        case 5:
                            kg = ReQComputer(5, i, inputData, isExistRightRow0, this.dgvQ); //已知第5行
                            QComputer(i, r, 5, kg, this.dgvQ, isExistRightRow0, slQ);
                            break;
                        case 6:
                            kg = ReQComputer(6, i, inputData, isExistRightRow0, this.dgvQ); //已知第6行
                            QComputer(i, r, 6, kg, this.dgvQ, isExistRightRow0, slQ);
                            break;
                        case 7:
                            kg = ReQComputer(7, i, inputData, isExistRightRow0, this.dgvQ); //已知第7行
                            QComputer(i, r, 7, kg, this.dgvQ, isExistRightRow0, slQ);
                            break;
                    }
                    #endregion
                }

                List<KeyValuePair<string, string>> inputDataD20 = QDc.Where(o => o.Key == "D20").ToList();
                List<KeyValuePair<string, string>> inputDataL = QDc.Where(o => o.Key == "L").ToList();
                List<KeyValuePair<string, string>> inputDataB = QDc.Where(o => o.Key == "B").ToList();
                List<KeyValuePair<string, string>> inputDataAG = QDc.Where(o => o.Key == "AG").ToList();
                List<KeyValuePair<string, string>> inputDataBG = QDc.Where(o => o.Key == "BG").ToList();
                if (inputDataD20.Count == 0)
                {
                    if (inputDataL.Count == 1)//输入中只有L
                    {
                        this.dgvQ[i, 4].Style.ForeColor = BCellColor;
                        string l = this.dgvQ[i, 4].Value.ToString().Trim();
                        if (Regex.IsMatch(l, pattern))
                        {
                            double L = Convert.ToDouble(l);
                            double B = L * 1000 / 158.98;
                            double AG = L / 3.785 * 1000;
                            double BG = L / 4.546 * 1000;

                            setQCellData(L, B, AG, BG, i,"L");
                        }
                    }
                    if (inputDataB.Count == 1)//输入中只有B
                    {
                        this.dgvQ[i, 5].Style.ForeColor = BCellColor;
                        string b = this.dgvQ[i, 5].Value.ToString().Trim();
                        if (Regex.IsMatch(b, pattern))
                        {
                            double B = Convert.ToDouble(b);
                            double L = B / 1000 * 158.98;
                            double AG = L / 3.785 * 1000;
                            double BG = L / 4.546 * 1000;
                            setQCellData(L, B, AG, BG,i,"B");
                        }
                    }
                    if (inputDataAG.Count == 1)//输入中只有AG
                    {
                        this.dgvQ[i, 6].Style.ForeColor = BCellColor;
                        string ag = this.dgvQ[i, 6].Value.ToString().Trim();
                        if (Regex.IsMatch(ag, pattern))
                        {
                            double AG = Convert.ToDouble(ag);
                            double L = AG * 3.785 / 1000;
                            double B = L * 1000 / 158.98;
                            double BG = L / 4.546 * 1000;
                            setQCellData(L, B, AG, BG, i,"AG");
                        }
                    }
                    if (inputDataBG.Count == 1)//输入中只有BG
                    {
                        this.dgvQ[i, 7].Style.ForeColor = BCellColor;
                        string bg = this.dgvQ[i, 7].Value.ToString().Trim();
                        if (Regex.IsMatch(bg, pattern))
                        {
                            double BG = Convert.ToDouble(bg);
                            double L = BG * 4.546 / 1000;
                            double B = L * 1000 / 158.98;
                            double AG = L / 3.785 * 1000;
                            setQCellData(L, B, AG, BG, i,"BG");
                        }
                    }
                }
            }
            //错误信息内容悬浮显示
            tooltipEContent(this.lblQE);
            #endregion
        }

        /// <summary>
        /// 给质量表中的L、B、AG、BG赋值
        /// </summary>
        /// <param name="L"></param>
        /// <param name="B"></param>
        /// <param name="AG"></param>
        /// <param name="BG"></param>
        /// <param name="i">列号</param>
        private void setQCellData(double L, double B, double AG, double BG,int i,string inPut)
        {
            this.dgvQ[i, 4].Value = L;
            this.dgvQ[i, 5].Value = B;
            this.dgvQ[i, 6].Value = AG;
            this.dgvQ[i, 7].Value = BG;
            switch (inPut)
            {
                case "L":
                    slQ.Add(i + "|" + 5);
                    slQ.Add(i + "|" + 6);
                    slQ.Add(i + "|" + 7);
                    break;
                case "B":
                    slQ.Add(i + "|" + 4);
                    slQ.Add(i + "|" + 6);
                    slQ.Add(i + "|" + 7);
                    break;
                case "AG":
                    slQ.Add(i + "|" + 4);
                    slQ.Add(i + "|" + 5);
                    slQ.Add(i + "|" + 7);
                    break;
                case "BG":
                    slQ.Add(i + "|" + 4);
                    slQ.Add(i + "|" + 5);
                    slQ.Add(i + "|" + 6);
                    break;
            }
        }

        /// <summary>
        /// 单位换算--质量--清除所有单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiClearQ_Click(object sender, EventArgs e)
        {
            QClear();
        }

        private void QClear()
        {
            this.dgvQ.Rows.Clear();
            this.dgvQ.Columns.Clear();
            slQ.Clear();
            this.lblQE.Text = "";
            BindDataQ();
        }

        /// <summary>
        /// 选项事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiChoQ_Click(object sender, EventArgs e)
        {
            QCho();
        }
        private void QCho()
        {
            this.contextMenuStrip2.Show(this.menuStripQ, new Point(this.tsmiChoQ.Width + intervalWidth, this.tsmiChoQ.Height));
        }

        /// <summary>
        /// 选项事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvQ_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvQ.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvQ.CurrentCell = this.dgvQ.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvQ.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        /// <summary>
        /// 行标头添加序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvQ_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
            e.RowBounds.Location.Y,
            this.dgvT.RowHeadersWidth - 4,
            e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
            this.dgvT.RowHeadersDefaultCellStyle.Font,
            rectangle,
            this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        /// <summary>
        /// 行标头添加序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblQE_MouseHover(object sender, EventArgs e)
        {
            if (lblQE.Text.Length > 35)
            {
                this.lblQE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        #endregion

        #region 单位换算--温度--计算

        /// <summary>
        /// 单位换算--温度--计算
        /// </summary>
        /// <param name="i">列号</param>
        /// <param name="r">行号的开始位置</param>
        /// <param name="inputr">输入数据的行号</param>
        /// <param name="inputData">输入的数据</param>
        private void TComputer(int i, int r, int inputr, double inputData, DataGridView dgv, bool isExistRightRow0, List<string> sl)
        {
            double rowinit = inputData;
            if (rowinit != 9999999)
            {
                if (inputr == 0)    //输入数据是第0行C
                {
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    dgv[i, r].Value = rowinit;
                    sl.Add(i + "|" + r);
                }

                if (inputr == 1)  //输入数据是第1行K
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row2 = rowinit + 273.15;
                    r++;
                    dgv[i, r].Value = subDouble(row2.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 2)  //输入数据是第2行F
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row3 = rowinit * 9.0 / 5 + 32;
                    r++;
                    dgv[i, r].Value = subDouble(row3.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 3)  //输入数据是第3行R
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row4 = 1.88 * (rowinit + 273.15);
                    r++;
                    dgv[i, r].Value = subDouble(row4.ToString());
                    sl.Add(i + "|" + r);
                }
            }
        }

        /// <summary>
        /// 单位换算--温度--逆计算
        /// </summary>
        /// <param name="r">行号</param>
        /// <param name="i">列号</param>
        /// <param name="inputData">输入数据</param>
        /// <param name="isExistRightRow0">是否存在数据正确的第一行</param>
        /// <returns></returns>
        private double ReTComputer(int r, int i, string inputData, bool isExistRightRow0, DataGridView dgv)
        {
            double rowinit = 9999999;
            switch (r)
            {
                case 0:
                    rowinit = double.Parse(inputData);  //输入数据为第0行C,只需转换下类型
                    break;
                case 1:
                    rowinit = double.Parse(inputData) - 273.15;//输入数据为第1行K
                    break;
                case 2:
                    rowinit = (double.Parse(inputData) - 32) * 5.0 / 9;//输入数据为第2行F
                    break;
                case 3:
                    rowinit = double.Parse(inputData) / 1.88 - 273.15;//输入数据为第3行R
                    break;
            }
            return rowinit;
        }

        #endregion

        #region 单位换算--压力--计算

        /// <summary>
        /// 单位换算--压力--计算
        /// </summary>
        /// <param name="i">列号</param>
        /// <param name="r">行号的开始位置</param>
        /// <param name="inputr">输入数据的行号</param>
        /// <param name="inputData">输入的数据</param>
        private void PressureComputer(int i, int r, int inputr, double inputData, DataGridView dgv, bool isExistRightRow0, List<string> sl)
        {
            double rowinit = inputData;
            if (rowinit != 0.0)
            {
                if (inputr == 0)    //输入数据是第0行帕Pa
                {
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    dgv[i, r].Value = rowinit;
                    sl.Add(i + "|" + r);
                }

                if (inputr == 1)  //输入数据是第1行巴bar
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row2 = rowinit / 100000;
                    r++;
                    dgv[i, r].Value = subDouble(row2.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 2)  //输入数据是第2行psi
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row3 = rowinit / 6895;
                    r++;
                    dgv[i, r].Value = subDouble(row3.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 3)  //输入数据是第3行大气压atm
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row4 = rowinit / 101000;
                    r++;
                    dgv[i, r].Value = subDouble(row4.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 4)  //输入数据是第4行汞柱mmHg
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row5 = rowinit * 0.0075;
                    r++;
                    dgv[i, r].Value = subDouble(row5.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 5)  //输入数据是第5行水柱mH2O
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row5 = rowinit / 9.8;
                    r++;
                    dgv[i, r].Value = subDouble(row5.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 6)  //输入数据是第6行千克力kgf/cm2
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row5 = rowinit / 9.8;
                    r++;
                    dgv[i, r].Value = subDouble(row5.ToString());
                    sl.Add(i + "|" + r);
                }
            }
        }

        /// <summary>
        /// 单位换算--压力--逆计算
        /// </summary>
        /// <param name="r">行号</param>
        /// <param name="i">列号</param>
        /// <param name="inputData">输入数据</param>
        /// <param name="isExistRightRow0">是否存在数据正确的第一行</param>
        /// <returns></returns>
        private double RePressureComputer(int r, int i, string inputData, bool isExistRightRow0, DataGridView dgv)
        {
            double rowinit = 0.0;
            switch (r)
            {
                case 0:
                    rowinit = double.Parse(inputData);  //输入数据为第0行帕Pa,只需转换下类型
                    break;
                case 1:
                    rowinit = double.Parse(inputData) * 100000;//输入数据为第1行巴bar
                    break;
                case 2:
                    rowinit = double.Parse(inputData) * 6895;//输入数据为第2行psi
                    break;
                case 3:
                    rowinit = double.Parse(inputData) * 101000;//输入数据为第3行大气压atm
                    break;
                case 4:
                    rowinit = double.Parse(inputData) / 0.0075;//输入数据为第4行汞柱mmHg
                    break;
                case 5:
                    rowinit = double.Parse(inputData) * 9.8;//输入数据为第5行水柱mH2O
                    break;
                case 6:
                    rowinit = double.Parse(inputData) * 9.8;//输入数据为第6行千克力kgf/cm2
                    break;
            }
            return rowinit;
        }

        #endregion

        #region 单位换算--浓度--计算
        /// <summary>
        /// 单位换算--浓度--计算
        /// </summary>
        /// <param name="i">列号</param>
        /// <param name="r">行号的开始位置</param>
        /// <param name="inputr">输入数据的行号</param>
        /// <param name="inputData">输入的数据</param>
        private void DensityComputer(int i, int r, int inputr, double inputData, DataGridView dgv, bool isExistRightRow0, List<string> sl)
        {
            double rowinit = inputData;
            if (rowinit != 0.0)
            {
                if (inputr == 1)    //输入数据是第1行ugg
                {
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    dgv[i, r].Value = rowinit;
                    sl.Add(i + "|" + r);
                }

                if (inputr == 2)  //输入数据是第2行PPM
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row2 = rowinit;
                    r++;
                    dgv[i, r].Value = subDouble(row2.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 3)  //输入数据是第3行%
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row3 = rowinit / 10000;
                    r++;
                    dgv[i, r].Value = subDouble(row3.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 4)  //输入数据是第4行ng/g
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row4 = rowinit * 1000;
                    r++;
                    dgv[i, r].Value = subDouble(row4.ToString());
                    sl.Add(i + "|" + r);
                }

                if (isExistRightRow0)
                {
                    if (inputr == 5)  //输入数据是第5行mg/l
                    {
                        r++;
                        dgv[i, r].Style.ForeColor = BCellColor;
                    }
                    else
                    {
                        double row5 = rowinit * Convert.ToDouble(dgv[i, 0].Value);
                        r++;
                        dgv[i, r].Value = subDouble(row5.ToString());
                        sl.Add(i + "|" + r);
                    }
                }
            }
        }

        /// <summary>
        /// 单位换算--浓度--计算-逆计算
        /// </summary>
        /// <param name="r">行号</param>
        /// <param name="i">列号</param>
        /// <param name="inputData">输入数据</param>
        /// <param name="isExistRightRow0">是否存在数据正确的第一行</param>
        /// <returns></returns>
        private double ReDensityComputer(int r, int i, string inputData, bool isExistRightRow0, DataGridView dgv)
        {
            double rowinit = 0.0;
            switch (r)
            {
                case 1:
                    rowinit = double.Parse(inputData);  //输入数据为第一行ug/g,只需转换下类型
                    break;
                case 2:
                    rowinit = double.Parse(inputData);//输入数据为第2行ppm
                    break;
                case 3:
                    rowinit = double.Parse(inputData) * 10000;//输入数据为第3行%
                    break;
                case 4:
                    rowinit = double.Parse(inputData) / 1000;//输入数据为第4行ng/g
                    break;
                case 5:
                    if (isExistRightRow0)
                    {
                        rowinit = double.Parse(inputData) / Convert.ToDouble(dgv[i, 0].Value);//输入数据为第5行mg/l
                    }
                    break;
            }
            return rowinit;
        }
        #endregion

        #region 单位换算--质量--计算
        /// <summary>
        /// 单位换算--质量--计算
        /// </summary>
        /// <param name="i">列号</param>
        /// <param name="r">行号的开始位置</param>
        /// <param name="inputr">输入数据的行号</param>
        /// <param name="inputData">输入的数据</param>
        private void QComputer(int i, int r, int inputr, double inputData, DataGridView dgv, bool isExistRightRow0, List<string> sl)
        {
            double rowinit = inputData;
            if (rowinit != 0.0)
            {
                if (inputr == 1)    //输入数据是第0行kg
                {
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    dgv[i, r].Value = rowinit;
                    sl.Add(i + "|" + r);
                }

                if (inputr == 2)  //输入数据是第2行T
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row2 = rowinit / 1000;
                    r++;
                    dgv[i, r].Value = subDouble(row2.ToString());
                    sl.Add(i + "|" + r);
                }

                if (inputr == 3)  //输入数据是第3行P
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    double row3 = rowinit * 2.205;
                    r++;
                    dgv[i, r].Value = subDouble(row3.ToString());
                    sl.Add(i + "|" + r);
                }

                if (isExistRightRow0)  //判断是否有D20输入
                {
                    if (inputr == 4)  //输入数据是第4行L
                    {
                        r++;
                        dgv[i, r].Style.ForeColor = BCellColor;
                    }
                    else
                    {
                        double row4 = rowinit / Convert.ToDouble(dgv[i, 0].Value) / 1000;
                        r++;
                        dgv[i, r].Value = subDouble(row4.ToString());
                        sl.Add(i + "|" + r);
                    }

                    if (inputr == 5)  //输入数据是第5行B
                    {
                        r++;
                        dgv[i, r].Style.ForeColor = BCellColor;
                    }
                    else
                    {
                        double row5 = rowinit / Convert.ToDouble(dgv[i, 0].Value) / 158.98;
                        r++;
                        dgv[i, r].Value = subDouble(row5.ToString());
                        sl.Add(i + "|" + r);
                    }

                    if (inputr == 6)  //输入数据是第6行AG
                    {
                        r++;
                        dgv[i, r].Style.ForeColor = BCellColor;
                    }
                    else
                    {
                        double row6 = rowinit / Convert.ToDouble(dgv[i, 0].Value) / 3.785;
                        r++;
                        dgv[i, r].Value = subDouble(row6.ToString());
                        sl.Add(i + "|" + r);
                    }

                    if (inputr == 7)  //输入数据是第7行BG
                    {
                        r++;
                        dgv[i, r].Style.ForeColor = BCellColor;
                    }
                    else
                    {
                        double row7 = rowinit / Convert.ToDouble(dgv[i, 0].Value) / 4.546;
                        r++;
                        dgv[i, r].Value = subDouble(row7.ToString());
                        sl.Add(i + "|" + r);
                    }
                }
            }
        }

        /// <summary>
        /// 单位换算--质量--计算-逆计算
        /// </summary>
        /// <param name="r">行号</param>
        /// <param name="i">列号</param>
        /// <param name="inputData">输入数据</param>
        /// <param name="isExistRightRow0">是否存在数据正确的第一行</param>
        /// <returns></returns>
        private double ReQComputer(int r, int i, string inputData, bool isExistRightRow0, DataGridView dgv)
        {
            double rowinit = 0.0;
            switch (r)
            {
                case 1:
                    rowinit = double.Parse(inputData);  //输入数据为第一行kg,只需转换下类型
                    break;
                case 2:
                    rowinit = double.Parse(inputData) * 1000;//输入数据为第2行T
                    break;
                case 3:
                    rowinit = double.Parse(inputData) / 2.205;//输入数据为第3行P
                    break;
                case 4:
                    if (isExistRightRow0)
                    {
                        rowinit = double.Parse(inputData) * Convert.ToDouble(dgv[i, 0].Value) * 1000;//输入数据为第4行L
                    }
                    break;
                case 5:
                    if (isExistRightRow0)
                    {
                        rowinit = double.Parse(inputData) * Convert.ToDouble(dgv[i, 0].Value) * 158.98;//输入数据为第5行B
                    }
                    break;
                case 6:
                    if (isExistRightRow0)
                    {
                        rowinit = double.Parse(inputData) * Convert.ToDouble(dgv[i, 0].Value) * 3.785;//输入数据为第6行AG
                    }
                    break;
                case 7:
                    if (isExistRightRow0)
                    {
                        rowinit = double.Parse(inputData) * Convert.ToDouble(dgv[i, 0].Value) * 4.546;//输入数据为第7行BG
                    }
                    break;
            }
            return rowinit;
        }
        #endregion

        #region 判断输入参数个数
        /// <summary>
        /// 输入格式的判断,应用于温度、压力、浓度、质量
        /// </summary>
        /// <param name="dgv">datagridview对象</param>
        /// <param name="lbl">label对象</param>
        /// <param name="i">列号</param>
        /// <param name="type">0-有可选输入数据，判断格式； 1--无可选输入数据，不判断格式</param>
        /// <param name="isMutiValue"></param>
        /// <param name="tempData">输入数据临时存放变量</param>
        private void examInputCount(DataGridView dgv, Label lbl, int i, int type, out int isMutiValue, out string tempData)
        {
            int _isMutiValue = 0;//作为判断是否一列出现两个输入

            string _tempData = "";  //临时存放每列的输入数据
            int j = 0;
            if (type == 0) //0-有可选输入数据，判断该可选输入数据格式； 1--无可选输入数据，不判断该可选输入数据格式(计算浓度和质量时第二行为可选数据)
            {
                j = 2;
                if (dgv[i, 0].Value != null && dgv[i, 0].Value.ToString() != "")  //对可选输入数据加前景色
                {
                    dgv[i, 0].Style.ForeColor = BCellColor;
                }
            }
            for (; j < dgv.Rows.Count; j++)  //检查2-6行，是否只有一输入
            {
                if (dgv[i, j].Value != null && dgv[i, j].Value.ToString() != "")
                {
                    _isMutiValue++;     //作为判断是否一列出现两个输入
                    _tempData = i.ToString() + "|" + j.ToString() + "|" + dgv[i, j].Value.ToString();
                    dgv[i, j].Style.ForeColor = BCellColor;
                }
            }
            isMutiValue = _isMutiValue;
            tempData = _tempData;
        }
        #endregion 判断输入参数个数

        #region 判断输入格式
        /// <summary>
        /// 输入格式的判断
        /// </summary>
        /// <param name="dgv">datagridview对象</param>
        /// <param name="lbl">label对象</param>
        /// <param name="i">列号</param>
        /// <param name="tempData">输入数据临时存放变量</param>
        /// <param name="isExistRightRow0"></param>
        /// <param name="inputRow">输入行</param>
        /// <param name="inputData">输入数据</param>
        /// <param name="type">0-有可选输入数据，判断格式； 2--压力的范围判断</param>
        /// <returns></returns>
        private int examInputFormat(DataGridView dgv, Label lbl, int i, string tempData, out bool isExistRightRow0, out int inputRow, out string inputData, int type)
        {
            string[] rcData = tempData.Split('|');
            int rowError = Convert.ToInt32(rcData[1]) + 1;//该变量是表示原行数从0开始，现在从1开始

            #region 第0行的可选输入数据的判断
            bool row0 = true;
            bool _isExistRightRow0 = false;
            if (type == 0 || type == 3)
            {
                if (dgv[i, 0].Value != null && dgv[i, 0].Value.ToString() != "")
                {

                    row0 = Regex.IsMatch(dgv[i, 0].Value.ToString().Trim().Trim(), pattern);
                    if (row0 == false)  //如果第0行非数字
                    {

                        lbl.Text += "第" + rcData[0] + "列第1行非数字；";
                        // continue;
                    }
                    else
                    {
                        if (Convert.ToDouble(dgv[i, 0].Value) < 0.5 || Convert.ToDouble(dgv[i, 0].Value) > 1.1)  //判断第0行范围是否溢出
                        {
                            lbl.Text += "第" + rcData[0] + "列第1行超限；";
                            row0 = false;
                            //continue;
                        }
                    }

                    if (row0 == true)
                    {
                        _isExistRightRow0 = true; //true--第0行有值并且输入格式正确
                    }
                }
            }
            #endregion

            isExistRightRow0 = _isExistRightRow0;
            inputRow = Convert.ToInt32(rcData[1]);
            inputData = rcData[2];

            bool a = Regex.IsMatch(rcData[2].Trim(), pattern);
            if (a == false)  //如果非数字
            {
                lbl.Text += "第" + rcData[0] + "列第" + rowError + "行非数字；";
                return 0;
            }

            if (type == 2 || (type == 0 && 2 != rowError) || type == 3)  //压的范围判断
            {
                if (Convert.ToDouble(rcData[2]) < 0)  //判断范围是否溢出
                {
                    lbl.Text += "第" + rcData[0] + "列第" + rowError + "行超限；";
                    return 0;
                }
            }

            if (type == 0 && 4 == rowError)  //第三行为浓度的%行数据判断
            {
                if (Convert.ToDouble(rcData[2]) < 0 || Convert.ToDouble(rcData[2]) > 100)  //判断范围是否溢出
                {
                    lbl.Text += "第" + rcData[0] + "列第" + rowError + "行超限；";
                    return 0;
                }
            }

            if (type == 4)  //第6行为密度的api(0<api<100)的数据判断，其他范围在0.5到1.1
            {
                if (6 == rowError)
                {
                    if (Convert.ToDouble(rcData[2]) < 0 || Convert.ToDouble(rcData[2]) > 100)  //判断范围是否溢出
                    {
                        lbl.Text += "第" + rcData[0] + "列第" + rowError + "行超限；";
                        return 0;
                    }
                }
                else
                {
                    if (Convert.ToDouble(rcData[2]) < 0.5 || Convert.ToDouble(rcData[2]) > 1.1)  //判断范围是否溢出
                    {
                        lbl.Text += "第" + rcData[0] + "列第" + rowError + "行超限；";
                        return 0;
                    }
                }
            }

            if (row0 == false)   //如果其他行正确，第0行数据错误，跳出函数
            {
                return 0;
            }
            return 1;
        }
        #endregion

        #region 清除单元格
        /// <summary>
        /// 清除单元格
        /// </summary>
        /// <param name="dgv"></param>
        private void ClearDgv(DataGridView dgv, List<string> sl)
        {
            if (sl.Count > 0)
            {
                foreach (string i in sl)  //循环清除计算出的结果
                {
                    string[] ar = i.Split('|');
                    dgv[Convert.ToInt32(ar[0]), Convert.ToInt32(ar[1])].Value = "";
                }
            }

            sl.Clear();

            for (int i = 1; i < dgv.Columns.Count; i++)
            {
                for (int j = 0; j < dgv.Rows.Count; j++)
                    dgv[i, j].Style.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }
        #endregion

        #endregion

        #region 重绘标签
        private void tbcPhysic_DrawItem(object sender, DrawItemEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            //   Draw   the   text 
            e.Graphics.DrawString(((TabControl)sender).TabPages[e.Index].Text,
            System.Windows.Forms.SystemInformation.MenuFont,
            new SolidBrush(Color.Black),
            e.Bounds,
            sf);
        }
        #endregion

        #region 物性关联--密度

        List<string> slMiDu = new List<string>();

        /// <summary>
        /// 绑定密度列表
        /// </summary>
        private void BindDataMiDu()
        {
            this.lblMiDuE.Text = "";

            this.dgvMiDu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvMiDu.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMiDu, "20℃，g/cm3");
            this.dgvMiDu.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMiDu, "15℃，g/cm3");
            this.dgvMiDu.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMiDu, "70℃，g/cm3");
            this.dgvMiDu.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMiDu, "60℉，g/cm3");
            this.dgvMiDu.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMiDu, "SG60F g/cm3");
            this.dgvMiDu.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMiDu, "API°");
            this.dgvMiDu.Rows.Add(rowT);

            this.dgvMiDu.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvMiDu.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvMiDu.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvMiDu.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 50;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComMiDu_Click(object sender, EventArgs e)
        {
            this.dgvMiDu.EndEdit();
            MiDuCom();
        }

        private void MiDuCom()
        {
            ClearDgv(this.dgvMiDu, slMiDu);

            this.lblMiDuE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvMiDu.Columns.Count; i++)
            {
                int isMutiValue = 0;//作为判断是否一列出现两个输入

                string tempData = "";  //临时存放每列的输入数据


                examInputCount(this.dgvMiDu, this.lblMiDuE, i, 1, out isMutiValue, out tempData);

                //判断一列是否出现两个值，出现则不计算此列
                if (isMutiValue > 1)
                {
                    //不计算
                    //汇总此列
                    this.lblMiDuE.Text += "第" + i + "列输入参数个数不对；";
                }
                else if (isMutiValue != 0)   //列中有输入数据才计算
                {
                    bool isExistRightRow0 = false;
                    int inputRow = 0; //输入行
                    string inputData = ""; //输入数据
                    int flag = examInputFormat(this.dgvMiDu, lblMiDuE, i, tempData, out isExistRightRow0, out inputRow, out inputData, 4);
                    if (flag == 0)
                    { continue; }

                    #region 开始计算
                    //不跳出，说明输入数字合法，调用计算函数
                    int r = 0;  //行号
                    double c = 0.0;


                    switch (inputRow)
                    {
                        case 0:
                            c = ReMiDuComputer(0, i, inputData, isExistRightRow0, this.dgvMiDu); //已知第0行D20
                            MiDuComputer(i, r, 0, c, this.dgvMiDu, isExistRightRow0, slMiDu);
                            break;
                        case 1:
                            c = ReMiDuComputer(1, i, inputData, isExistRightRow0, this.dgvMiDu); //已知第1行D15
                            MiDuComputer(i, r, 1, c, this.dgvMiDu, isExistRightRow0, slMiDu);
                            break;
                        case 2:
                            c = ReMiDuComputer(2, i, inputData, isExistRightRow0, this.dgvMiDu); //已知第2行D70
                            MiDuComputer(i, r, 2, c, this.dgvMiDu, isExistRightRow0, slMiDu);
                            break;
                        case 3:
                            c = ReMiDuComputer(3, i, inputData, isExistRightRow0, this.dgvMiDu); //已知第3行D60
                            MiDuComputer(i, r, 3, c, this.dgvMiDu, isExistRightRow0, slMiDu);
                            break;
                        case 4:
                            c = ReMiDuComputer(4, i, inputData, isExistRightRow0, this.dgvMiDu); //已知第4行SG
                            MiDuComputer(i, r, 4, c, this.dgvMiDu, isExistRightRow0, slMiDu);
                            break;
                        case 5:
                            c = ReMiDuComputer(5, i, inputData, isExistRightRow0, this.dgvMiDu); //已知第5行API
                            MiDuComputer(i, r, 5, c, this.dgvMiDu, isExistRightRow0, slMiDu);
                            break;
                    }

                    #endregion
                }
            }

            //错误信息内容悬浮显示
            tooltipEContent(this.lblMiDuE);
            #endregion
        }

        /// <summary>
        /// 清除所有单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiClearMiDu_Click(object sender, EventArgs e)
        {
            MiDuClear();
        }

        private void MiDuClear()
        {
            this.dgvMiDu.Rows.Clear();
            this.dgvMiDu.Columns.Clear();
            slMiDu.Clear();
            this.lblMiDuE.Text = "";
            BindDataMiDu();
        }

        private void lblMiDuE_MouseHover(object sender, EventArgs e)
        {
            if (lblMiDuE.Text.Length > 35)
            {
                this.lblMiDuE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //选项事件
        private void tsmiChoMiDu_Click(object sender, EventArgs e)
        {
            MiDuCho();
        }

        private void MiDuCho()
        {
            this.contextMenuStrip2.Show(this.menuStripMiDu, new Point(this.tsmiChoMiDu.Width + 80, this.tsmiChoMiDu.Height));
        }

        //单击编辑单元格
        private void dgvMiDu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvMiDu.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvMiDu.CurrentCell = this.dgvMiDu.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvMiDu.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        /// <summary>
        /// 行标头添加序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvMiDu_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
          e.RowBounds.Location.Y,
          this.dgvT.RowHeadersWidth - 4,
          e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region 物性关联--酸度

        List<string> slAcid = new List<string>();
        /// <summary>
        /// 绑定密度列表
        /// </summary>
        private void BindDataAcid()
        {
            this.lblAcidE.Text = "";

            this.dgvAcid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvAcid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAcid, "20℃密度，g/cm3");
            this.dgvAcid.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAcid, "酸度,mgKOH/100ml");
            this.dgvAcid.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAcid, "酸值, mgKOH/g");
            this.dgvAcid.Rows.Add(rowT);

            this.dgvAcid.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvAcid.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvAcid.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvAcid.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 90;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComAcid_Click(object sender, EventArgs e)
        {
            this.dgvAcid.EndEdit();
            
            AcidCom();
        }

        

        private void AcidCom()
        {
            ClearDgv(this.dgvAcid, slAcid);

            this.lblAcidE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvAcid.Columns.Count; i++)
            {
                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvAcid.Rows.Count; j++)
                {
                    if (this.dgvAcid[i, j].Value != null && this.dgvAcid[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Acid)j).ToString(), this.dgvAcid[i, j].Value.ToString());
                        this.dgvAcid[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" select p).ToList();
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "ACD" || p.Key == "NET" select p).ToList();
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ACD" select p).ToList();
                List<KeyValuePair<string, string>> inputData4 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "NET" select p).ToList();

                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Acid);
                if (inputData1.Count == 1 && inputData2.Count == 1)
                {
                    if (inputData4.Count() == 2) // D20,NET->ACD
                    {
                        string acd = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 22;  //  NET, D20->ACD
                        isFormatRight = examFreezeFormat(this.lblAcidE, inputData4, 0.5, 1.1, 0, 100, i, out acd, type, enumType);
                        if (isFormatRight == 0)
                        {
                            //continue;

                        }
                        else
                        {
                            if (acd.Contains('E')) //double型的字符串以指数形式表示，则截取小数后6位
                            {
                                acd = Convert.ToDouble(acd).ToString("F6"); //小数点后保留6位
                            }
                            this.dgvAcid[i, 1].Value = acd;   //给acd单元格赋值
                            slAcid.Add(i + "|" + 1);
                            this.dgvAcid[i, 1].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        }
                    }

                    if (inputData3.Count() == 2) //D20,ACD ->NET
                    {
                        string net = "";

                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 23;  //ACD, D20 ->NET
                        isFormatRight = examFreezeFormat(this.lblAcidE, inputData3, 0.5, 1.1, 0, 100, i, out net, type, enumType);

                        if (isFormatRight == 0)
                        {
                            //continue;
                            if (this.lblAcidE.Text.Length > 0)
                                this.lblAcidE.Text = subString(this.lblAcidE.Text);
                        }
                        else
                        {
                            if (net.Contains('E')) //double型的字符串以指数形式表示，则截取小数后6位
                            {
                                net = Convert.ToDouble(net).ToString("F6");
                            }
                            this.dgvAcid[i, 2].Value = net;   //给smk单元格赋值
                            slAcid.Add(i + "|" + 2);
                            this.dgvAcid[i, 2].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        }

                    }
                }
                else
                {
                    if (inputData1.Count != 0 || inputData2.Count != 0)
                    {
                        this.lblAcidE.Text += "第" + i + "列输入参数个数不对；";
                    }
                }
                #endregion
            }
            #endregion

            tooltipEContent(this.lblAcidE);
        }

        //清除所有单元格
        private void tsmiClearAcid_Click(object sender, EventArgs e)
        {
            AcidClear();
        }

        private void AcidClear()
        {
            this.dgvAcid.Rows.Clear();
            this.dgvAcid.Columns.Clear();
            slAcid.Clear();
            this.lblAcidE.Text = "";
            BindDataAcid();
        }

        private void lblAcidE_MouseHover(object sender, EventArgs e)
        {
            if (lblAcidE.Text.Length > 35)
            {
                this.lblAcidE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //选项事件
        private void tsmiChoAcid_Click(object sender, EventArgs e)
        {
            AcidCho();
        }
        private void AcidCho()
        {
            this.contextMenuStrip2.Show(this.menuStripAcid, new Point(this.tsmiChoAcid.Width + 80, this.tsmiChoAcid.Height));
        }

        //单击编辑单元格
        private void dgvAcid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvAcid.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvAcid.CurrentCell = this.dgvAcid.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvAcid.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标头添加序列
        private void dgvAcid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
         e.RowBounds.Location.Y,
         this.dgvT.RowHeadersWidth - 4,
         e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region 物性关联--三点粘度

        List<string> slViscority = new List<string>();

        /// <summary>
        /// 绑定密度列表
        /// </summary>
        private void BindDataViscosity()
        {
            this.lblViscosityE.Text = "";

            this.dgvViscosity.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvViscosity.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvViscosity, "温度1,℃");
            this.dgvViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvViscosity, "粘度1 mm2/s");
            this.dgvViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvViscosity, "温度2,℃");
            this.dgvViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvViscosity, "粘度2 mm2/s");
            this.dgvViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvViscosity, "温度3 ℃");
            this.dgvViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvViscosity, "粘度3 mm2/s");
            this.dgvViscosity.Rows.Add(rowT);

            this.dgvViscosity.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvViscosity.Columns[0].ReadOnly = true;

            foreach (DataGridViewColumn c in this.dgvViscosity.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 90;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
            this.dgvViscosity.Rows[0].Selected = false;
        }

        //计算
        private void tsmiComViscosity_Click(object sender, EventArgs e)
        {
            this.dgvViscosity.EndEdit();
            this.dgvMixViscosity.EndEdit();
            ViscosityCom();
            MixViscosityCom();
        }

        private void ViscosityCom()
        {
            ClearDgv(this.dgvViscosity, slViscority);

            this.lblViscosityE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvViscosity.Columns.Count; i++)
            {
                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvViscosity.Rows.Count; j++)
                {
                    if (this.dgvViscosity[i, j].Value != null && this.dgvViscosity[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Viscosity)j).ToString(), this.dgvViscosity[i, j].Value.ToString());
                        this.dgvViscosity[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "V1" || p.Key == "V2" || p.Key == "V3" select p).ToList();
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "T1" || p.Key == "T2" || p.Key == "T3" select p).ToList();
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "T2" || p.Key == "V2" || p.Key == "T3" || p.Key == "V3" || p.Key == "T1" select p).ToList(); //T2,V2, T3, V3, T1->V1
                List<KeyValuePair<string, string>> inputData4 = (from p in oilTypeDc where p.Key == "T1" || p.Key == "V1" || p.Key == "T3" || p.Key == "V3" || p.Key == "T2" select p).ToList(); //T1,V1, T3, V3, T2->V2
                List<KeyValuePair<string, string>> inputData5 = (from p in oilTypeDc where p.Key == "T1" || p.Key == "V1" || p.Key == "T2" || p.Key == "V2" || p.Key == "T3" select p).ToList(); //T1,V1, T2, V2, T3->V3

                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Viscosity);
                if (inputData1.Count == 2 && inputData2.Count == 3)
                {
                    if (inputData3.Count() == 5) //T2,V2, T3, V3, T1->V1
                    {
                        string v1 = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 24;  //T2,V2, T3, V3, T1->V1
                        isFormatRight = examCommonFormat(this.lblViscosityE, inputData3, -50, 150, 0, 20000, -50, 150, 0, 20000, -50, 150, -1.0, -1.0, i, out v1, type, enumType);
                        if (isFormatRight == 0)
                        {
                            continue;
                        }
                        this.dgvViscosity[i, 1].Value = v1;   //给v1单元格赋值
                        slViscority.Add(i + "|" + 1);
                        this.dgvViscosity[i, 1].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }

                    if (inputData4.Count() == 5) //T1,V1, T3, V3, T2->V2
                    {
                        string v2 = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 25;   //T1,V1, T3, V3, T2->V2
                        isFormatRight = examCommonFormat(this.lblViscosityE, inputData4, -50, 150, 0, 20000, -50, 150, 0, 20000, -50, 150, -1.0, -1.0, i, out v2, type, enumType);
                        if (isFormatRight == 0)
                        {
                            continue;
                        }
                        this.dgvViscosity[i, 3].Value = v2;   //给v2单元格赋值
                        slViscority.Add(i + "|" + 3);
                        this.dgvViscosity[i, 3].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }

                    if (inputData5.Count() == 5) //T1,V1, T2, V2, T3->V3
                    {
                        string v3 = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 25;   //T1,V1, T2, V2, T3->V3
                        isFormatRight = examCommonFormat(this.lblViscosityE, inputData5, -50, 150, 0, 20000, -50, 150, 0, 20000, -50, 150, -1.0, -1.0, i, out v3, type, enumType);
                        if (isFormatRight == 0)
                        {
                            continue;
                        }
                        this.dgvViscosity[i, 5].Value = v3;   //给v3单元格赋值
                        slViscority.Add(i + "|" + 5);
                        this.dgvViscosity[i, 5].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                else
                {
                    if (inputData1.Count >= 1 || inputData2.Count >= 1)
                    {
                        this.lblViscosityE.Text += "第" + i + "列输入参数个数不对；";
                    }
                }
                #endregion
            }
            #endregion

            tooltipEContent(this.lblViscosityE);
        }

        //清除所有单元格
        private void tsmiClearSViscosity_Click(object sender, EventArgs e)
        {
            ViscosityClear();
        }

        private void ViscosityClear()
        {
            this.dgvViscosity.Rows.Clear();
            this.dgvViscosity.Columns.Clear();
            slViscority.Clear();
            this.lblViscosityE.Text = "";
            BindDataViscosity();
        }

        //选项事件
        private void tsmiChoViscosity_Click(object sender, EventArgs e)
        {
            ViscosityCho();
        }
        private void ViscosityCho()
        {
            this.contextMenuStrip2.Show(this.menuStripViscosity, new Point(this.tsmiChoViscosity.Width + 80, this.tsmiChoViscosity.Height));
        }

        //单击编辑单元格
        private void dgvViscosity_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvViscosity.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvViscosity.CurrentCell = this.dgvViscosity.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvViscosity.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        private void lblViscosityE_MouseHover(object sender, EventArgs e)
        {
            if (lblViscosityE.Text.Length > 35)
            {
                this.lblViscosityE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //行标头添加序列
        private void dgvViscosity_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--混合粘度

        List<string> slMixViscosity = new List<string>();

        /// <summary>
        /// 绑定混合粘度列表
        /// </summary>
        private void BindDataMixViscosity()
        {
            this.lblMixViscosityE.Text = "";

            this.dgvMixViscosity.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvMixViscosity.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "组成1混合量");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "组成1粘度,mm2/s");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "组成2混合量");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "组成2粘度,mm2/s");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "组成3混合量");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "组成3粘度,mm2/s");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "组成4混合量");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "组成4粘度,mm2/s");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "混兑油总量");
            this.dgvMixViscosity.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMixViscosity, "混兑油粘度，mm2/s");
            this.dgvMixViscosity.Rows.Add(rowT);

            this.dgvMixViscosity.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvMixViscosity.Columns[0].ReadOnly = true;

            foreach (DataGridViewColumn c in this.dgvMixViscosity.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 120;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
            this.dgvMixViscosity.Rows[0].Selected = false;
        }

        //计算
        private void MixViscosityCom()
        {
            ClearDgv(this.dgvMixViscosity, slMixViscosity);

            this.lblMixViscosityE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvMixViscosity.Columns.Count; i++)
            {
                int isMutiValue = 0;//存放输入对数个数

                string tempData = "";  //临时存放每列的输入数据

                //flag==false,由组求总   ==true,由总求组
                bool flag = examMutiInputCount(this.dgvMixViscosity, this.lblMixViscosityE, i, out isMutiValue, out tempData); //输入参数个数判断

                //判断一列是否只有一对输入，仅有一对输入就不计算
                if (isMutiValue < 2 && isMutiValue > 0)
                {
                    //不计算
                    //汇总此列
                    this.lblMixViscosityE.Text += "第" + i + "列输入参数个数不对；";
                }
                else if (isMutiValue >= 2)  //列中有两对以上输入数据才计算
                {
                    string inputData1 = ""; //组合混合量
                    string inputData2 = ""; //组合粘度
                    int inputE = examMutiInputFormat(this.dgvMixViscosity, lblMixViscosityE, i, tempData, out inputData1, out  inputData2);
                    if (inputE == 1)  //返回值为1--输入数据格式有错误，跳出此次循环
                    { continue; }

                    #region 开始计算
                    string[] inputC = inputData1.Split('|');
                    string[] inputV = inputData2.Split('|');
                    double sumB = 0.0;  //混兑油的总量
                    double sumVB = 0.0;//混兑油的粘度
                    double sumV = 0.0;
                    int tempk = 0;

                    if (flag == false)   //由组量、组粘度求混合油总量、总粘度
                    {

                        for (int k = 0; k < inputC.Length - 1; k++)//循环求混兑油的总量
                        {
                            sumB += Convert.ToDouble(inputC[k]);
                        }
                        this.dgvMixViscosity[i, 8].Value = sumB;
                        slMixViscosity.Add(i + "|" + 8);
                        this.dgvMixViscosity[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;

                        for (int k = 0; k < inputV.Length - 1; k++)
                        {
                            sumV += Convert.ToDouble(BaseFunction.IndexFunVIS(inputV[k])) * Convert.ToDouble(inputC[k]);
                        }

                        sumVB = Convert.ToDouble(BaseFunction.InverseFunIndexVIS((sumV / sumB).ToString()));
                        this.dgvMixViscosity[i, 9].Value = sumVB;
                        slMixViscosity.Add(i + "|" + 9);
                        this.dgvMixViscosity[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                    else //由混合油总量、总粘度求组量、组粘度
                    {
                        sumB = Convert.ToDouble(inputC[inputC.Length - 2]);
                        for (int k = 0; k < inputC.Length - 2; k++)//循环求混兑油的总量
                        {
                            sumB -= Convert.ToDouble(inputC[k]);
                        }
                        for (int k = 0; k < dgvMixViscosity.Rows.Count - 2; k++)
                        {
                            if (this.dgvMixViscosity[i, k].Value == null || this.dgvMixViscosity[i, k].Value.ToString() == "")
                            {
                                this.dgvMixViscosity[i, k].Value = sumB;
                                slMixViscosity.Add(i + "|" + k);
                                this.dgvMixViscosity[i, k].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                                tempk = k;
                                break;
                            }
                        }
                        for (int k = 0; k < inputV.Length - 2; k++)
                        {
                            sumV += Convert.ToDouble(BaseFunction.IndexFunVIS(inputV[k])) * Convert.ToDouble(inputC[k]);
                        }
                        this.dgvMixViscosity[i, tempk + 1].Value = BaseFunction.InverseFunIndexVIS(((Convert.ToDouble(BaseFunction.IndexFunVIS(inputV[inputV.Length - 2])) * Convert.ToDouble(inputC[inputC.Length - 2]) - sumV) / sumB).ToString());
                        slMixViscosity.Add(i + "|" + (tempk + 1));
                        this.dgvMixViscosity[i, tempk + 1].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                    #endregion
                }
            }
            tooltipEContent(this.lblMixViscosityE);
            #endregion
        }

        //清除所有单元格
        private void tsmiClearMViscosity_Click(object sender, EventArgs e)
        {
            MixViscosityClear();
        }
        private void MixViscosityClear()
        {
            this.dgvMixViscosity.Rows.Clear();
            this.dgvMixViscosity.Columns.Clear();
            slMixViscosity.Clear();
            this.lblMixViscosityE.Text = "";
            BindDataMixViscosity();
        }

        //单击编辑单元格
        private void dgvMixViscosity_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvMixViscosity.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvMixViscosity.CurrentCell = this.dgvMixViscosity.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvMixViscosity.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        private void lblMixViscosityE_MouseHover(object sender, EventArgs e)
        {
            if (lblMixViscosityE.Text.Length > 35)
            {
                this.lblMixViscosityE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //行标头添加序列
        private void dgvMixViscosity_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region 物性关联--特性因素

        List<string> slProperty = new List<string>();

        /// <summary>
        /// 绑定特性因素列表
        /// </summary>
        private void BindDataProperty()
        {
            this.lblPropertyE.Text = "";

            this.dgvProperty.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvProperty.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "20℃密度,g/cm3");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "50℃粘度，mm2/s");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "100℃粘度，mm2/s");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "窄馏分：初切点,℃");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "窄馏分：终切点,℃");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "宽馏分：10%点,℃");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "宽馏分：30%点,℃");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "宽馏分：50%点,℃");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "宽馏分：70%点,℃");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "宽馏分：90%点,℃");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "中平均沸点,℃");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "特性因数");
            this.dgvProperty.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvProperty, "相关指数");
            this.dgvProperty.Rows.Add(rowT);

            this.dgvProperty.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvProperty.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvProperty.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvProperty.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 63;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComProperty_Click(object sender, EventArgs e)
        {
            this.dgvProperty.EndEdit();
            PropertyCom();
        }

        private void PropertyCom()
        {
            ClearDgv(this.dgvProperty, slProperty);

            this.lblPropertyE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvProperty.Columns.Count; i++)//循环每列
            {
                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvProperty.Rows.Count; j++)//循环每行
                {
                    if (this.dgvProperty[i, j].Value != null && this.dgvProperty[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Property)j).ToString(), this.dgvProperty[i, j].Value.ToString());
                        this.dgvProperty[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" select p).ToList();
                List<KeyValuePair<string, string>> inputData2 = oilTypeDc.Where(p => p.Key == "D20" || p.Key == "V05").ToList();
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "V10" select p).ToList();  //V10,D20->KFC 26 
                List<KeyValuePair<string, string>> inputData4 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ICP" || p.Key == "ECP" select p).ToList(); //ICP,ECP, D20->KFC 27  ICP,ECP, D20->BMI 28 
                List<KeyValuePair<string, string>> inputData5 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "A10" || p.Key == "A30" || p.Key == "A50" || p.Key == "A70" || p.Key == "A90" select p).ToList(); //A10,A30,A50,A70,A90, D20->KFC    A10,A30,A50,A70,A90,D20->BMI 
                List<KeyValuePair<string, string>> inputData6 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "MCP" select p).ToList();  //MCP, D20->KFC 31  MCP,D20->BMI 32
                List<KeyValuePair<string, string>> inputData7 = (from p in oilTypeDc where p.Key == "KFC" || p.Key == "BMI" select p).ToList();  //MCP, D20->KFC 31  MCP,D20->BMI 32
                if (inputData7.Count > 0)
                {
                    this.lblPropertyE.Text += "第" + i.ToString() + "列" + "第12和13行必须为空；";
                    continue;
                }

                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Property);
                if (inputData1.Count == 1)
                {

                    if (inputData2.Count == 2)//V05,D20 -> KFC
                    {
                        string kfc = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 46;  //V05,D20->KFC 46
                        isFormatRight = examFreezeFormat(this.lblPropertyE, inputData2, 0.5, 1.1, 0, 20000, i, out kfc, type, enumType);
                        if (isFormatRight == 0)
                        {
                            // continue;
                            if (this.lblPropertyE.Text.Length > 0)
                                this.lblPropertyE.Text = subString(this.lblPropertyE.Text);
                        }
                        else
                        {
                            this.dgvProperty[i, 11].Value = kfc;   //给kfc单元格赋值
                            slProperty.Add(i + "|" + 11);
                            this.dgvProperty[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        }
                    }

                    if (inputData3.Count() == 2) //V10,D20->KFC 26 
                    {
                        string kfc = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 26;  //V10,D20->KFC 26
                        isFormatRight = examFreezeFormat(this.lblPropertyE, inputData3, 0.5, 1.1, 0, 20000, i, out kfc, type, enumType);
                        if (isFormatRight == 0)
                        {
                            // continue;
                            if (this.lblPropertyE.Text.Length > 0)
                                this.lblPropertyE.Text = subString(this.lblPropertyE.Text);
                        }
                        else
                        {
                            this.dgvProperty[i, 11].Value = kfc;   //给kfc单元格赋值
                            slProperty.Add(i + "|" + 11);
                            this.dgvProperty[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        }
                    }

                    if (inputData4.Count() == 3) //ICP,ECP, D20->KFC 27  ICP,ECP, D20->BMI 28
                    {
                        string kfc = "";
                        string bmi = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 27;  //ICP,ECP, D20->KFC 27
                        isFormatRight = examFreezeFormat(this.lblPropertyE, inputData4, 0.5, 1.1, 15, 560, i, out kfc, type, enumType);

                        type = 28;  //ICP,ECP, D20->BMI 28
                        isFormatRight = examFreezeFormat(this.lblPropertyE, inputData4, 0.5, 1.1, 15, 560, i, out bmi, type, enumType);


                        if (isFormatRight == 0)
                        {
                            //if (lblPropertyE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                            //{
                            //    lblPropertyE.Text = lblPropertyE.Text.Substring(0, lblPropertyE.Text.Length / 2);
                            //}
                            if (this.lblPropertyE.Text.Length > 0)
                                this.lblPropertyE.Text = subString(this.lblPropertyE.Text);
                            //  continue;
                        }
                        else
                        {
                            this.dgvProperty[i, 11].Value = kfc;   //给kfc单元格赋值
                            this.dgvProperty[i, 12].Value = bmi;   //给bmi单元格赋值
                            slProperty.Add(i + "|" + 11);
                            this.dgvProperty[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            slProperty.Add(i + "|" + 12);
                            this.dgvProperty[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        }
                    }

                    if (inputData5.Count() == 6) //A10,A30,A50,A70,A90, D20->KFC    A10,A30,A50,A70,A90,D20->BMI 
                    {
                        string kfc = "";
                        string bmi = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 29;  //A10,A30,A50,A70,A90, D20->KFC  29
                        isFormatRight = examFreezeFormat(this.lblPropertyE, inputData5, 0.5, 1.1, 15, 560, i, out kfc, type, enumType);

                        type = 30;  //A10,A30,A50,A70,A90,D20->BMI  30
                        isFormatRight = examFreezeFormat(this.lblPropertyE, inputData5, 0.5, 1.1, 15, 560, i, out bmi, type, enumType);

                        if (isFormatRight == 0)
                        {
                            //if (lblPropertyE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                            //{
                            //    lblPropertyE.Text = lblPropertyE.Text.Substring(0, lblPropertyE.Text.Length / 2);
                            //}
                            if (this.lblPropertyE.Text.Length > 0)
                                this.lblPropertyE.Text = subString(this.lblPropertyE.Text);
                            //  continue;
                        }
                        else
                        {
                            this.dgvProperty[i, 11].Value = kfc;   //给kfc单元格赋值
                            this.dgvProperty[i, 12].Value = bmi;   //给bmi单元格赋值
                            slProperty.Add(i + "|" + 11);
                            this.dgvProperty[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            slProperty.Add(i + "|" + 12);
                            this.dgvProperty[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        }
                    }

                    if (inputData6.Count() == 2) //MCP, D20->KFC 31  MCP,D20->BMI 32
                    {
                        string kfc = "";
                        string bmi = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 31;  //MCP, D20->KFC 31
                        isFormatRight = examFreezeFormat(this.lblPropertyE, inputData6, 0.5, 1.1, 15, 560, i, out kfc, type, enumType);

                        type = 32;  // MCP,D20->BMI 32
                        isFormatRight = examFreezeFormat(this.lblPropertyE, inputData6, 0.5, 1.1, 15, 560, i, out bmi, type, enumType);

                        if (isFormatRight == 0)
                        {
                            //if (lblPropertyE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                            //{
                            //    lblPropertyE.Text = lblPropertyE.Text.Substring(0, lblPropertyE.Text.Length / 2);
                            //}
                            if (this.lblPropertyE.Text.Length > 0)
                                this.lblPropertyE.Text = subString(this.lblPropertyE.Text);
                            //  continue;
                        }
                        else
                        {
                            this.dgvProperty[i, 11].Value = kfc;   //给kfc单元格赋值
                            this.dgvProperty[i, 12].Value = bmi;   //给bmi单元格赋值
                            slProperty.Add(i + "|" + 11);
                            this.dgvProperty[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            slProperty.Add(i + "|" + 12);
                            this.dgvProperty[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        }
                    }
                }
                else
                {
                    if (inputData3.Count != 0 || inputData4.Count != 0 || inputData5.Count != 0 || inputData6.Count != 0)
                        this.lblPropertyE.Text += "第1行必须有数据；";
                }
                #endregion
            }
            #endregion

            tooltipEContent(this.lblPropertyE);
        }

        private void lblPropertyE_MouseHover(object sender, EventArgs e)
        {
            if (lblPropertyE.Text.Length > 35)
            {
                this.lblPropertyE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //清除所有单元格
        private void tsmiClearProperty_Click(object sender, EventArgs e)
        {
            PropertyClear();
        }

        private void PropertyClear()
        {
            this.dgvProperty.Rows.Clear();
            this.dgvProperty.Columns.Clear();
            slProperty.Clear();
            this.lblPropertyE.Text = "";
            BindDataProperty();
        }

        //选项事件
        private void tsmiChoProperty_Click(object sender, EventArgs e)
        {
            PropertyCho();
        }

        private void PropertyCho()
        {
            this.contextMenuStrip2.Show(this.menuStripProperty, new Point(this.tsmiChoProperty.Width + 80, this.tsmiChoProperty.Height));
        }

        //单击编辑单元格
        private void dgvProperty_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvProperty.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvProperty.CurrentCell = this.dgvProperty.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvProperty.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标头添加序列
        private void dgvProperty_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
       e.RowBounds.Location.Y,
       this.dgvT.RowHeadersWidth - 4,
       e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region 物性关联--原油类型

        List<string> slOilType = new List<string>();

        /// <summary>
        /// 绑定原油类型列表
        /// </summary>
        private void BindDataOilType()
        {
            this.lblOilTypeE.Text = "";

            this.dgvOilType.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvOilType.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "第一关键馏分：API°");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "第一关键馏分：20℃ 密度,g/cm3");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "第一关键馏分：特性 因数");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "第二关键馏分：API°");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "第二关键馏分：20℃ 密度,g/cm3");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "第二关键馏分：特性因数");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油：特性因数");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油：API");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油：20℃密度 g/cm3");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油 ：硫含量，%");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油 ：酸值，mgKOH/g");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油 ：蜡含量，%");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油类型");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油基属");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油硫水平");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油酸水平");
            this.dgvOilType.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvOilType, "原油蜡水平");
            this.dgvOilType.Rows.Add(rowT);

            this.dgvOilType.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvOilType.Columns[0].ReadOnly = true;

            this.dgvOilType.Rows[1].Height = 40;
            this.dgvOilType.Rows[4].Height = 40;

            foreach (DataGridViewColumn c in this.dgvOilType.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 145;
                    c.Frozen = true;
                }
                else
                    c.Width = 75;
            }
        }

        Dictionary<string, string> oilTypeDc = new Dictionary<string, string>();
        //计算
        private void tsmiComOilType_Click(object sender, EventArgs e)
        {
            this.dgvOilType.EndEdit();
            OilTypeCom();
        }

        private void OilTypeCom()
        {
            ClearDgv(this.dgvOilType, slOilType);

            this.lblOilTypeE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvOilType.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvOilType.Rows.Count; j++)
                {
                    if (this.dgvOilType[i, j].Value != null && this.dgvOilType[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((OilType)j).ToString(), this.dgvOilType[i, j].Value.ToString());
                        this.dgvOilType[i, j].Style.ForeColor = BCellColor;
                    }
                }

                //List<KeyValuePair<string, string>> resultDataFormat = (from p in oilTypeDc where p.Key == "WAX" || p.Key == "CLA" || p.Key == "TYP" || p.Key == "SCL" || p.Key == "ACL" || p.Key == "WCL" select p).ToList();

                //if (resultDataFormat.Count > 0)
                //{
                //    this.lblOilTypeE.Text += "第" + i.ToString() + "列" + "第12行到17行必须为空;";
                //    continue;
                //}

                #region 开始计算
                #region 求typ
                List<KeyValuePair<string, string>> apis = (from p in oilTypeDc where p.Key == "API1" || p.Key == "API2" select p).ToList();
                List<KeyValuePair<string, string>> d20s = (from p in oilTypeDc where p.Key == "D201" || p.Key == "D202" select p).ToList();
                List<KeyValuePair<string, string>> kfcs = (from p in oilTypeDc where p.Key == "KFC1" || p.Key == "KFC2" select p).ToList();
                List<KeyValuePair<string, string>> kfc = (from p in oilTypeDc where p.Key == "KFC" select p).ToList();

                if (apis.Count() == 2) //由api1和api2求typ
                {
                    string typ = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, apis, 0, 70, i, out typ);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 13].Value = typ;
                        slOilType.Add(i + "|" + 13);
                        this.dgvOilType[i, 13].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                else if (d20s.Count() == 2)
                {
                    string typ = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, d20s, 0.5, 1.1, i, out typ);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 13].Value = typ;
                        slOilType.Add(i + "|" + 13);
                        this.dgvOilType[i, 13].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                else if (kfcs.Count() == 2)
                {
                    string typ = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, kfcs, 10.5, 12.6, i, out typ);
                    if (isFormatRight == 0)
                    {
                        // continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 13].Value = typ;
                        slOilType.Add(i + "|" + 13);
                        this.dgvOilType[i, 13].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                else if (kfc.Count == 1)
                {
                    string typ = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, kfc, 10.5, 12.6, i, out typ);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 13].Value = typ;
                        slOilType.Add(i + "|" + 13);
                        this.dgvOilType[i, 13].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                #endregion

                #region 求原油类型
                List<KeyValuePair<string, string>> api = (from p in oilTypeDc where p.Key == "API" select p).ToList();
                List<KeyValuePair<string, string>> d20 = (from p in oilTypeDc where p.Key == "D20" select p).ToList();
                if (api.Count() == 1) //由api求cla
                {
                    string cla = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, api, 0, 70, i, out cla);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 12].Value = cla;
                        slOilType.Add(i + "|" + 12);
                        this.dgvOilType[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                else if (d20.Count == 1)
                {
                    string cla = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, d20, 0.5, 1.1, i, out cla);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 12].Value = cla;
                        slOilType.Add(i + "|" + 12);
                        this.dgvOilType[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                #endregion

                #region 求硫水平
                List<KeyValuePair<string, string>> sul = (from p in oilTypeDc where p.Key == "SUL" select p).ToList();
                if (sul.Count() == 1) //由sul求scl
                {
                    string scl = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, sul, 0, 10, i, out scl);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 14].Value = scl;
                        slOilType.Add(i + "|" + 14);
                        this.dgvOilType[i, 14].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                #endregion

                #region 求酸水平
                List<KeyValuePair<string, string>> net = (from p in oilTypeDc where p.Key == "NET" select p).ToList();
                if (net.Count() == 1) //由net求acl
                {
                    string acl = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, net, 0, 10, i, out acl);
                    if (isFormatRight == 0)
                    {
                        // continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 15].Value = acl;
                        slOilType.Add(i + "|" + 15);
                        this.dgvOilType[i, 15].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                #endregion

                #region 求蜡水平
                List<KeyValuePair<string, string>> wax = (from p in oilTypeDc where p.Key == "WAX" select p).ToList();
                if (wax.Count() == 1) //由net求acl
                {
                    string wcl = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    isFormatRight = examTypFormat(this.lblOilTypeE, wax, 0, 50, i, out wcl);
                    if (isFormatRight == 0)
                    {
                        // continue;
                    }
                    else
                    {
                        this.dgvOilType[i, 16].Value = wcl;
                        slOilType.Add(i + "|" + 16);
                        this.dgvOilType[i, 16].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                #endregion

                #endregion

            }

            tooltipEContent(this.lblOilTypeE);
            #endregion
        }

        //清除所有单元格
        private void tsmiClearOilType_Click(object sender, EventArgs e)
        {
            OilTypeClear();
        }

        private void OilTypeClear()
        {
            this.dgvOilType.Rows.Clear();
            this.dgvOilType.Columns.Clear();
            slOilType.Clear();
            this.lblOilTypeE.Text = "";
            BindDataOilType();
        }

        //选项事件
        private void tsmiChoOilType_Click(object sender, EventArgs e)
        {
            OilTypeCho();
        }
        private void OilTypeCho()
        {
            this.contextMenuStrip2.Show(this.menuStripOilType, new Point(this.tsmiChoOilType.Width + intervalWidth, this.tsmiChoOilType.Height));
        }

        //单击编辑单元格
        private void dgvOilType_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvOilType.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvOilType.CurrentCell = this.dgvOilType.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvOilType.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        private void lblOilTypeE_MouseHover(object sender, EventArgs e)
        {
            if (lblOilTypeE.Text.Length > 35)
            {
                this.lblOilTypeE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //行标头添加序列
        private void dgvOilType_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
      e.RowBounds.Location.Y,
      this.dgvT.RowHeadersWidth - 4,
      e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--馏程转换

        List<string> slDistill = new List<string>();

        string[] point = new string[9] { "初切点", "5%点", "10%点", "30%点", "50%点", "70%点", "90%点", "95%点", "终切点" };

        /// <summary>
        /// 绑定原油类型列表
        /// </summary>
        private void BindDataDistill()
        {
            this.lblDistillE.Text = "";

            this.dgvDistil.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "馏出点" });
            this.dgvDistil.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "温度，℃" });
            this.dgvDistil.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "累计收率，v%" });
            this.dgvDistil.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "切割点，℃" });
            this.dgvDistil.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "馏出点" });
            this.dgvDistil.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "温度，℃" });

            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "初馏点", "", "", "", "初馏点");
            this.dgvDistil.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "5%点", "", "", "", "5%点");
            this.dgvDistil.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "10%点", "", "", "", "10%点");
            this.dgvDistil.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "30%点", "", "", "", "30%点");
            this.dgvDistil.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "50%点", "", "", "", "50%点");
            this.dgvDistil.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "70%点", "", "", "", "70%点");
            this.dgvDistil.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "90%点", "", "", "", "90%点");
            this.dgvDistil.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "95%点", "", "", "", "95%点");
            this.dgvDistil.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvDistil, "终馏点", "", "", "", "终馏点");
            this.dgvDistil.Rows.Add(rowT);

            for (int k = 0; k < 20; k++)
            {
                rowT = new DataGridViewRow();
                rowT.CreateCells(this.dgvDistil, "", "", "", "", "");
                this.dgvDistil.Rows.Add(rowT);
            }

            this.dgvDistil.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvDistil.Columns[4].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvDistil.Columns[0].ReadOnly = true;
            this.dgvDistil.Columns[4].ReadOnly = true;

            for (int j = 9; j < 28; j++)
            {
                this.dgvDistil[1, j].ReadOnly = true;
                this.dgvDistil[5, j].ReadOnly = true;
            }

            foreach (DataGridViewRow c in this.dgvDistil.Rows)
            {
                c.Height = 20;
            }

            foreach (DataGridViewColumn c in this.dgvDistil.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0 || c.Index == 4)
                {
                    c.Width = 50;
                    //  c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }

            this.AddSpanHeader(0, 2, "ASTM D86");
            this.AddSpanHeader(2, 2, "TBP");
            this.AddSpanHeader(4, 2, "ASTM D1160");
        }

        //计算
        Dictionary<string, string> inputDic = new Dictionary<string, string>();
        private void tsmiComDistill_Click(object sender, EventArgs e)
        {
            this.dgvDistil.EndEdit();
            DistillCom();
        }

        private void DistillCom()
        {
            ClearDgv(this.dgvDistil, slDistill);
            this.lblDistillE.Text = "";
            inputDic.Clear();

            int column6Count = 0;//用来存放第六列的输入参数(初馏点、10、30、50点)个数,这四个点必须有值

            #region 初始化/格式判断
            for (int i = 1; i < this.dgvDistil.Columns.Count; i++)//循环每列
            {
                if (i == 4 || i == 2 || i == 3)  //第5列为列标题，不做计算,第3、4列为任意多行，单独循环添加到inputDic中
                { continue; }
                
                for (int j = 0; j < 9; j++)//循环每行
                {
                    if (i == 5 && (j==0 ||j==2 ||j==3 ||j==4 ))//第六列的数据
                    {
                        string tempValue = this.dgvDistil[5, j].Value == null ? string.Empty : this.dgvDistil[5, j].Value.ToString();
                        if (tempValue != string.Empty)
                        {
                            column6Count++;
                        }
                    }

                    if (i!= 5 && (j == 1 || j == 7))//(除第六列的其他列)第二行和第八行可以为空，单独处理。当为空时，赋值为-1做标记
                    {
                        if (this.dgvDistil[i, j].Value != null && this.dgvDistil[i, j].Value.ToString() != "")
                        {
                            inputDic.Add(j + "|" + i.ToString() + i.ToString(), this.dgvDistil[i, j].Value.ToString());
                            this.dgvDistil[i, j].Style.ForeColor = BCellColor;
                        }
                        else
                        {
                            inputDic.Add(j + "|" + i.ToString() + i.ToString(), "-1");
                        }
                    }
                    else if (i == 5 && (j == 1 || j==4 || j == 5 || j == 6 || j == 7 || j == 8))//第六列5点、70点、90点、95点、终馏点都可以为空
                    {
                        if (this.dgvDistil[i, j].Value != null && this.dgvDistil[i, j].Value.ToString() != "")
                        {
                            inputDic.Add(j + "|" + i.ToString() + i.ToString(), this.dgvDistil[i, j].Value.ToString());
                            this.dgvDistil[i, j].Style.ForeColor = BCellColor;
                        }
                        else
                        {
                            inputDic.Add(j + "|" + i.ToString() + i.ToString(), "-1");
                        }
                    }
                    else if (this.dgvDistil[i, j].Value != null && this.dgvDistil[i, j].Value.ToString() != "")
                    {
                        inputDic.Add(j + "|" + i.ToString() + i.ToString(), this.dgvDistil[i, j].Value.ToString());
                        this.dgvDistil[i, j].Style.ForeColor = BCellColor;
                    }
                }
            }
            for (int i = 1; i < this.dgvDistil.Columns.Count; i++)//循环每列
            {
                if (i == 1 || i == 4 || i == 5)  //将第1、4、5列排除
                { continue; }

                for (int j = 0; j < this.dgvDistil.Rows.Count; j++)//循环每行
                {
                    if (j == 1 || j == 7)//第二行和第八行可以为空，单独处理。当为空时，赋值为-1做标记
                    {
                        if (this.dgvDistil[i, j].Value != null && this.dgvDistil[i, j].Value.ToString() != "")
                        {
                            inputDic.Add(j + "|" + i.ToString() + i.ToString(), this.dgvDistil[i, j].Value.ToString());
                            this.dgvDistil[i, j].Style.ForeColor = BCellColor;
                        }
                        else
                        {
                            inputDic.Add(j + "|" + i.ToString() + i.ToString(), "-1");
                        }
                    }
                    else if (this.dgvDistil[i, j].Value != null && this.dgvDistil[i, j].Value.ToString() != "")
                    {
                        inputDic.Add(j + "|" + i.ToString() + i.ToString(), this.dgvDistil[i, j].Value.ToString());
                        this.dgvDistil[i, j].Style.ForeColor = BCellColor;
                    }
                }
            }
            #endregion

            #region 计算
            List<KeyValuePair<string, string>> temperatures1 = (from p in inputDic where p.Key.Contains("|11") select p).ToList(); //存放第1列温度输入数据
            List<KeyValuePair<string, string>> ecp = (from p in inputDic where p.Key.Contains("|33") select p).ToList();  //存放第3列切割点输入数据
            List<KeyValuePair<string, string>> tvy = (from p in inputDic where p.Key.Contains("|22") select p).ToList();  //存放第4列累计收率输入数据
            List<KeyValuePair<string, string>> temperatures2 = (from p in inputDic where p.Key.Contains("|55") select p).ToList();  //存放第6列温度的输入数据

            if (temperatures1.Count == 9)//根据第一列数据计算
            {
                #region 根据第一列计算
                bool isIncrease = true;
                List<string> tList = new List<string>();
                double smallValue = 0.0;  //存放最小值，比较各值，判断是否递增
                foreach (var item in temperatures1)
                {
                    #region 格式判断
                    if (item.Value == "-1")//-1代表空值
                    {
                        tList.Add(item.Value);
                        continue;
                    }

                    string inputData = item.Value.Trim();
                    tList.Add(inputData);
                    string[] array = item.Key.Split('|');
                    int r = Convert.ToInt32(array[0]) + 1;

                    bool a = Regex.IsMatch(inputData, pattern);

                    if (a == false)  //如果非数字
                    {
                        this.lblDistillE.Text += "第1列第" + r + "行非数字；";
                        continue;
                    }
                    else if (Convert.ToDouble(inputData) < 15 || Convert.ToDouble(inputData) > 350)
                    {
                        this.lblDistillE.Text += "第1列第" + r + "行超限；";
                        continue;
                    }

                    if (smallValue == 0.0)
                    {
                        smallValue = Convert.ToDouble(inputData);
                    }
                    else if (smallValue >= Convert.ToDouble(inputData))
                    {
                        isIncrease = false;
                    }
                    else
                    {
                        smallValue = Convert.ToDouble(inputData);
                    }
                    #endregion
                }
                if (isIncrease == false)
                {
                    this.lblDistillE.Text += "第1列数据从上到下没有单调递增；";
                }
                else
                {
                    //调用函数
                    List<double?> resultList = BaseFunction.FunfromAIP_A05_A10_A30_A50_A70_A90_AEP_KFC(tList[0], tList[1], tList[2], tList[3], tList[4], tList[5], tList[6], tList[7], tList[8], "0");
                    for (int d = 0; d < resultList.Count; d++)//赋值
                    {
                        this.dgvDistil[3, d].Value = resultList[d];
                        this.dgvDistil[2, d].Value = point[d];
                        slDistill.Add(3 + "|" + d);
                        this.dgvDistil[3, d].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        slDistill.Add(2 + "|" + d);
                        this.dgvDistil[2, d].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                #endregion
            }
            else if (ecp.Count > 0 && tvy.Count > 2)  //由第3第4列输入求结果
            {
                #region 由第3第4列输入求结果
                string[] arr1 = ecp[0].Key.Split('|');
                string[] arr2 = tvy[0].Key.Split('|');
                string[] arr3 = ecp[ecp.Count - 1].Key.Split('|');
                string[] arr4 = tvy[tvy.Count - 1].Key.Split('|');

                int ecpCount = 0;//存放ECP实际个数
                int tvyCount = 0;//存放TVY实际个数
                for (int i = 0; i < ecp.Count; i++)
                {
                    if (ecp[i].Value != "-1")
                    {
                        ecpCount++;
                    }
                }

                for (int i = 0; i < tvy.Count; i++)
                {
                    if (tvy[i].Value != "-1")
                    {
                        tvyCount++;
                    }
                }

                if (ecpCount >= 5 && tvyCount >= 5 && ecpCount == tvyCount)
                {
                    bool ECP05_IsNull = false;
                    bool ECP95_IsNull = false;
                    bool TVY05_IsNull = false;
                    bool TVY95_IsNull = false;

                    bool ECP_IsIncrease = true;
                    bool TVY_IsIncrease = true;
                    bool flag = true;
                    if (Convert.ToInt32(arr1[0]) != 0 && Convert.ToInt32(arr2[0]) != 0 && Convert.ToInt32(arr3[0]) != Convert.ToInt32(arr4[0]))
                    {
                        this.lblDistillE.Text += "第3列第4列必须从首行开始输入且数据成对出现；";
                        return;
                    }

                    Dictionary<string, string>  TVY_ECPDic = new Dictionary<string, string>();

                    #region 输入条件判断
                    for (int r = 0; r < ecpCount; r++)
                    {
                        string[] array = ecp[r].Key.Split('|');
                        int row = Convert.ToInt32(array[0]) + 1;
                        double smallValueECP = 0.0;  //存放最小值，比较各值，判断是否递增
                        double smallValueTVY = 0.0;
                        bool a = Regex.IsMatch(ecp[r].Value.Trim(), pattern);
                        if (a == false)  //如果非数字
                        {
                            this.lblDistillE.Text += "第3列第" + row + "行非数字；";
                            flag = false;
                            break;
                        }
                        //else if (Convert.ToDouble(ecp[r].Value) < 0 || Convert.ToDouble(ecp[r].Value) > 400)
                        //{
                        //    this.lblDistillE.Text += "第3列第" + row + "行超限；";
                        //    flag = false;
                        //    break;
                        //}

                        bool b = Regex.IsMatch(tvy[r].Value.Trim(), pattern);
                        if (b == false)  //如果非数字
                        {
                            this.lblDistillE.Text += "第4列第" + row + "行非数字；";
                            flag = false;
                            break;
                        }
                        //else if (Convert.ToDouble(tvy[r].Value) < 15 || Convert.ToDouble(tvy[r].Value) > 600)
                        //{
                        //    this.lblDistillE.Text += "第4列第" + row + "行超限；";
                        //    flag = false;
                        //    break;
                        //}
                        //ECP递增判断
                        if (r > 0)
                        {
                            if (ecp[r].Value == "-1" &&r==1 )
                            {
                                ECP05_IsNull = true;
                                continue;
                            }
                            if (ecp[r].Value == "-1" && r == 7)
                            {
                                ECP95_IsNull = true;
                                continue;
                            }
                            smallValueECP = Convert.ToDouble(ecp[r].Value);
                            if (smallValueECP <= Convert.ToDouble(ecp[r-1].Value))
                            {
                                ECP_IsIncrease = false;
                                break;
                            }
                            
                            //TVY递增判断
                            if (tvy[r].Value == "-1" && r==1)
                            {
                                TVY05_IsNull = true;
                                continue;
                            }
                            if (tvy[r].Value == "-1" && r == 7)
                            {
                                TVY95_IsNull = true;
                                continue;
                            }

                            smallValueTVY = Convert.ToDouble(tvy[r].Value);
                            if (smallValueTVY <= Convert.ToDouble(tvy[r-1].Value))
                            {
                                TVY_IsIncrease = false;
                                break;
                            }
                        }
                        if (!TVY_ECPDic.Keys.Contains(ecp[r].Value))
                        {
                            TVY_ECPDic.Add(ecp[r].Value, tvy[r].Value);
                        }
                    }
                    #endregion

                    if (flag == false)
                    {
                        return;
                    }
                    if (ECP_IsIncrease == false)
                    {
                        this.lblDistillE.Text += "第4列数据从上到下没有单调递增；";
                    }
                    else if (TVY_IsIncrease == false)
                    {
                        this.lblDistillE.Text += "第3列数据从上到下没有单调递增；";
                    }
                    else
                    {
                        //调用函数
                        Dictionary<string, float?> resultDic = BaseFunction.FunAIP_A10_A30_A50_A70_A90_A95_AEPfromCurveEntityECP_TVYandICP_ECP_KFC_ForTool(TVY_ECPDic, ecp[0].Value, ecp[ecpCount - 1].Value,ECP05_IsNull,ECP95_IsNull,TVY05_IsNull,TVY95_IsNull, "0");
                        int r = 0;
                        if (Convert.ToDouble(ecp[ecpCount - 1].Value.Trim()) <= 400)
                        {
                            foreach (var item in resultDic)
                            {

                                this.dgvDistil[1, r].Value = item.Value;
                                slDistill.Add(1 + "|" + r);
                                this.dgvDistil[1, r].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                                r++;
                            }
                        }
                        else
                        {
                            foreach (var item in resultDic)
                            {
                                this.dgvDistil[5, r].Value = item.Value;
                                slDistill.Add(5 + "|" + r);
                                this.dgvDistil[5, r].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                                r++;
                            }
                        }
                    }
                }
                else
                {
                    this.lblDistillE.Text += "第3列第4列输入对数不对或对数小于5；";
                }
                #endregion
            }
            else if (ecp.Count > 8 && tvy.Count <= 2)//TVY为空，ECP至少为9个(TVY为空时，有两行自动补充为-1了）
            {
                #region TVY为空，ECP至少为9个(TVY为空时，有两行自动补充为-1了）
                bool dataFormat = true;
                string[] TVY = new string[9] { "0", "5", "10", "30", "50", "70", "90", "95", "100" };
                string[] ECP = new string[9];
                for (int i = 0; i < 9; i++)
                {
                    #region 格式判断

                    string tempECP = ecp[i].Value == string.Empty ? string.Empty : ecp[i].Value.Trim();

                    if (Regex.IsMatch(tempECP, pattern))
                    {
                        ECP[i] = tempECP;
                    }
                    else
                    {
                        this.lblDistillE.Text += "第4列第" + (i + 1) + "行非数字；";
                        dataFormat = false;
                        break;
                    }

                    if (i > 0)
                    {
                        if (tempECP == "-1")
                        {
                            continue;
                        }
                        double smallValueECP = Convert.ToDouble(tempECP);
                        if (smallValueECP < Convert.ToDouble(ecp[i - 1].Value.Trim()))
                        {
                            this.lblDistillE.Text += "第4列数据从上到下没有单调递增；";
                            dataFormat = false;
                            break;
                        }
                    }
                    #endregion
                }

                if (dataFormat == true)//数据格式正确，进行计算
                {
                    double?[] Y = BaseFunction.Fun_D86FromTBP(ECP);
                    
                    int r = 0;
                    if (Convert.ToDouble(ecp[ecp.Count - 1].Value.Trim()) <= 400)
                    {
                        foreach (var item in Y)
                        {
                            if (item == null)
                            {
                                r++;
                                continue;
                            }
                            this.dgvDistil[1, r].Value = item.Value;
                            slDistill.Add(1 + "|" + r);
                            this.dgvDistil[1, r].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            r++;
                        }
                    }
                    else
                    {
                        foreach (var item in Y)
                        {
                            if (item == null)
                            {
                                r++;
                                continue;
                            }
                            this.dgvDistil[5, r].Value = item.Value;
                            slDistill.Add(5 + "|" + r);
                            this.dgvDistil[5, r].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            r++;
                        }
                    }
                }
                #endregion
            }
            else if (temperatures2.Count > 2)         //由第6列输入温度求结果
            {
                #region 由第6列输入温度求结果
                if (temperatures2.Count >= 4)
                {
                    #region 格式判断
                    bool flag = true;

                    if (column6Count != 4)
                    {
                        this.lblDistillE.Text += "第6列初馏点、10点、30点、50点必须都有数据；";
                        flag = false;
                    }

                    bool isIncrease = true;
                    List<string> tList = new List<string>();
                    
                    foreach (var item in temperatures2)
                    {
                        if (item.Value == "-1")
                        {
                            tList.Add(item.Value);
                            continue;
                        }
                        string inputData = item.Value.Trim();
                        tList.Add(inputData);
                        string[] array = item.Key.Split('|');
                        int r = Convert.ToInt32(array[0]) + 1;
                        double smallValue = 0.0;  //存放最小值，比较各值，判断是否递增
                        bool a = Regex.IsMatch(inputData.Trim(), pattern);
                        if (a == false)  //如果非数字
                        {
                            this.lblDistillE.Text += "第6列第" + r + "行非数字；";
                            flag = false;
                        }
                        //else if (Convert.ToDouble(inputData) < 300)
                        //{
                        //    this.lblDistillE.Text += "第6列第" + r + "行超限；";
                        //    flag = false;
                        //}
                        if (smallValue == 0.0)
                        {
                            smallValue = Convert.ToDouble(inputData);
                        }
                        else if (smallValue > Convert.ToDouble(inputData))
                        {
                            isIncrease = false;
                        }
                        else
                        {
                            smallValue = Convert.ToDouble(inputData);
                        }
                    #endregion
                    }

                    if (flag == false)
                    {
                        return;
                    }

                    if (isIncrease == false)
                    {
                        this.lblDistillE.Text += "第6列数据从上到下没有单调递增；";
                    }
                    else
                    {
                        //调用函数
                        List<double?> resultList = BaseFunction.FunfromAIP_A05_A10_A30_A50_A70_A90_AEP_KFC(tList[0], tList[1], tList[2], tList[3], tList[4], tList[5], tList[6], tList[7], tList[8], "0");
                        for (int d = 0; d < resultList.Count; d++)
                        {
                            this.dgvDistil[3, d].Value = resultList[d];
                            this.dgvDistil[2, d].Value = point[d];
                            slDistill.Add(3 + "|" + d);
                            this.dgvDistil[3, d].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            slDistill.Add(2 + "|" + d);
                            this.dgvDistil[2, d].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        }
                    }
                }
                else
                {
                    this.lblDistillE.Text += "第6列输入个数不对；";
                }
                #endregion
            }

            #endregion

            //tooltipEContent(this.lblDistillE);
            if (lblDistillE.Text.Length > 17)
            {
                string lblContent = lblDistillE.Text.ToString();
                string tempContent = "";
                while (lblContent.Length >= 40)
                {
                    tempContent += lblContent.Substring(0, 40) + "\r\n";
                    lblContent = lblContent.Remove(0, 40);
                }
                if (lblContent.Length > 0)
                {
                    tempContent += lblContent;
                }
                this.toolTip1.Show(tempContent, lblDistillE);
                lblDistillE.Text = lblDistillE.Text.Substring(0, 17) + "....";
                this.toolTip1.Active = false;
            }
            else
            {
                lblDistillE.Cursor = Cursors.Default;
            } 
        }

        //清除所有单元格
        private void tsmiClearDistill_Click(object sender, EventArgs e)
        {
            DistillClear();
        }

        private void DistillClear()
        {
            this.dgvDistil.Rows.Clear();
            this.dgvDistil.Columns.Clear();
            slDistill.Clear();
            this.lblDistillE.Text = "";
            BindDataDistill();
        }

        //选择菜单
        private void tsmiChoDistill_Click(object sender, EventArgs e)
        {
            DistillCho();
        }
        private void DistillCho()
        {
            this.contextMenuStrip2.Show(this.menuStripDistill, new Point(this.tsmiChoDistill.Width + 80, this.tsmiChoDistill.Height));
        }

        private void lblDistillE_MouseHover(object sender, EventArgs e)
        {
            if (lblDistillE.Text.Length > 35)
            {
                this.lblDistillE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvDistil_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                // this.dgvDistil.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvDistil.CurrentCell = this.dgvDistil.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvDistil.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标头添加序列
        private void dgvDistil_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 4,
     e.RowBounds.Location.Y,
     this.dgvT.RowHeadersWidth - 4,
     e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #region 二维表头
        private Color _mergecolumnheaderbackcolor = Color.FromArgb(247, 248, 250);// System.Drawing.SystemColors.Control;
        private void dgvDistil_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex > -1)
                {

                }
                else
                {
                    //二维表头
                    if (e.RowIndex == -1)
                    {
                        if (SpanRows.ContainsKey(e.ColumnIndex)) //被合并的列
                        {
                            //画边框
                            Graphics g = e.Graphics;
                            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

                            int left = e.CellBounds.Left, top = e.CellBounds.Top + 2,
                            right = e.CellBounds.Right, bottom = e.CellBounds.Bottom;

                            switch (SpanRows[e.ColumnIndex].Position)
                            {
                                case 1:
                                    left += 2;
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    right -= 2;
                                    break;
                            }

                            //画上半部分底色
                            g.FillRectangle(new SolidBrush(this._mergecolumnheaderbackcolor), left, top,
                            right - left, (bottom - top) / 2);

                            //画中线
                            g.DrawLine(new Pen(this.dgvDistil.GridColor), left, (top + bottom) / 2,
                            right, (top + bottom) / 2);

                            //写小标题
                            StringFormat sf = new StringFormat();
                            sf.Alignment = StringAlignment.Center;
                            sf.LineAlignment = StringAlignment.Center;

                            g.DrawString(e.Value + "", e.CellStyle.Font, Brushes.Black,
                            new Rectangle(left, (top + bottom) / 2, right - left, (bottom - top) / 2), sf);
                            left = this.dgvDistil.GetColumnDisplayRectangle(SpanRows[e.ColumnIndex].Left, true).Left - 2;

                            if (left < 0) left = this.dgvDistil.GetCellDisplayRectangle(-1, -1, true).Width;
                            right = this.dgvDistil.GetColumnDisplayRectangle(SpanRows[e.ColumnIndex].Right, true).Right - 2;
                            if (right < 0) right = this.Width;

                            g.DrawString(SpanRows[e.ColumnIndex].Text, e.CellStyle.Font, Brushes.Black,
                            new Rectangle(left, top, right - left, (bottom - top) / 2), sf);
                            e.Handled = true;
                        }
                    }
                }
                //base.CellPainting(e); 

            }
            catch
            { }

        }
        #endregion

        #region 二维表头
        private struct SpanInfo //表头信息
        {
            public SpanInfo(string Text, int Position, int Left, int Right)
            {
                this.Text = Text;
                this.Position = Position;
                this.Left = Left;
                this.Right = Right;
            }

            public string Text; //列主标题
            public int Position; //位置，1:左，2中，3右
            public int Left; //对应左行
            public int Right; //对应右行
        }
        private Dictionary<int, SpanInfo> SpanRows = new Dictionary<int, SpanInfo>();//需要2维表头的列

        /// <summary>
        /// 合并列
        /// </summary>
        /// <param name="ColIndex">列的索引</param>
        /// <param name="ColCount">需要合并的列数</param>
        /// <param name="Text">合并列后的文本</param>
        public void AddSpanHeader(int ColIndex, int ColCount, string Text)
        {
            if (ColCount < 2)
            {
                throw new Exception("行宽应大于等于2，合并1列无意义。");
            }
            //将这些列加入列表
            int Right = ColIndex + ColCount - 1; //同一大标题下的最后一列的索引
            SpanRows[ColIndex] = new SpanInfo(Text, 1, ColIndex, Right); //添加标题下的最左列
            SpanRows[Right] = new SpanInfo(Text, 3, ColIndex, Right); //添加该标题下的最右列
            for (int i = ColIndex + 1; i < Right; i++) //中间的列
            {
                SpanRows[i] = new SpanInfo(Text, 2, ColIndex, Right);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            ReDrawHead();
        }

        /// <summary>
        /// 刷新显示表头
        /// </summary>
        public void ReDrawHead()
        {
            foreach (int si in SpanRows.Keys)
            {
                this.dgvDistil.Invalidate(this.dgvDistil.GetCellDisplayRectangle(si, -1, true));
            }
        }

        private void dgvDistil_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)// && e.Type == ScrollEventType.EndScroll)
            {
                //timer1.Enabled = false; 
                timer1.Enabled = true;
            }
        }
        #endregion

        #endregion

        #region 物性关联--密度--计算

        /// <summary>
        /// 物性关联--密度--计算
        /// </summary>
        /// <param name="i">列号</param>
        /// <param name="r">行号的开始位置</param>
        /// <param name="inputr">输入数据的行号</param>
        /// <param name="inputData">输入的数据</param>
        private void MiDuComputer(int i, int r, int inputr, double inputData, DataGridView dgv, bool isExistRightRow0, List<string> ls)
        {
            double rowinit = inputData;
            if (rowinit != 0.0)
            {
                if (inputr == 0)    //输入数据是第0行D20
                {
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    dgv[i, r].Value = rowinit;
                    slMiDu.Add(i + "|" + r);
                }

                if (inputr == 1)  //输入数据是第1行D15
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    //double row2 = Convert.ToDouble(FunD15fromD20(rowinit.ToString()));
                    string row2 = subDouble(BaseFunction.FunD15fromD20(rowinit.ToString()));// FunD15fromD20());
                    r++;
                    dgv[i, r].Value = subDouble(row2.ToString());
                    slMiDu.Add(i + "|" + r);
                }

                if (inputr == 2)  //输入数据是第2行D70
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    //double row3 = Convert.ToDouble(BaseFunction.FunD70fromD20(rowinit.ToString()));
                    string row3 = subDouble(BaseFunction.FunD70fromD20(rowinit.ToString()));
                    r++;
                    dgv[i, r].Value = subDouble(row3.ToString());
                    slMiDu.Add(i + "|" + r);
                }

                if (inputr == 3)  //输入数据是第3行D60
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    //double row4 = Convert.ToDouble(BaseFunction.FunD60fromD20(rowinit.ToString()));
                    string row4 = subDouble(BaseFunction.FunD60fromD20(rowinit.ToString()));
                    r++;
                    dgv[i, r].Value = subDouble(row4.ToString());
                    slMiDu.Add(i + "|" + r);
                }

                if (inputr == 4)  //输入数据是第4行SG
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    //double row4 = Convert.ToDouble(BaseFunction.FunSGfromD20(rowinit.ToString()));
                    string row4 = subDouble(BaseFunction.FunSGfromD20(rowinit.ToString()));
                    r++;
                    dgv[i, r].Value = subDouble(row4.ToString());
                    slMiDu.Add(i + "|" + r);
                }

                if (inputr == 5)  //输入数据是第5行API
                {
                    r++;
                    dgv[i, r].Style.ForeColor = BCellColor;
                }
                else
                {
                    //double row4 = Convert.ToDouble(BaseFunction.FunAPIfromD20(rowinit.ToString()));
                    string row5 = subDouble(BaseFunction.FunAPIfromD20(rowinit.ToString()));
                    r++;
                    dgv[i, r].Value = subDouble(row5.ToString());
                    slMiDu.Add(i + "|" + r);
                }
            }
        }

        /// <summary>
        /// 物性关联--密度--计算-逆计算
        /// </summary>
        /// <param name="r">行号</param>
        /// <param name="i">列号</param>
        /// <param name="inputData">输入数据</param>
        /// <param name="isExistRightRow0">是否存在数据正确的第一行</param>
        /// <returns></returns>
        private double ReMiDuComputer(int r, int i, string inputData, bool isExistRightRow0, DataGridView dgv)
        {
            double rowinit = 0.0;
            switch (r)
            {
                case 0:
                    rowinit = double.Parse(inputData);  //输入数据为第0行D20,只需转换下类型
                    break;
                case 1:
                    rowinit = -0.0137 * double.Parse(inputData) * double.Parse(inputData) + 1.0277 * double.Parse(inputData) - 0.0173;//输入数据为第1行D15
                    break;
                case 2:
                    rowinit = Convert.ToDouble(BaseFunction.FunD20fromD70(inputData));//输入数据为第2行D70
                    break;
                case 3:
                    rowinit = Convert.ToDouble(BaseFunction.FunD20fromD60(inputData));//输入数据为第3行D60
                    break;
                case 4:
                    rowinit = Convert.ToDouble(BaseFunction.FunD20fromSG(inputData));//输入数据为第4行SG
                    break;
                case 5:
                    rowinit = Convert.ToDouble(BaseFunction.FunD20fromAPI(inputData));//输入数据为第5行API
                    break;
            }
            return rowinit;
        }

        #endregion

        #region 物性关联--混合粘度--计算
        #region 判断输入参数个数
        /// <summary>
        /// 输入格式的判断
        /// </summary>
        /// <param name="dgv">datagridview对象</param>
        /// <param name="lbl">label对象</param>
        /// <param name="i">列号</param>
        /// <param name="isMutiValue"></param>
        /// <param name="tempData">输入数据临时存放变量</param>
        private bool examMutiInputCount(DataGridView dgv, Label lbl, int i, out int isMutiValue, out string tempData)
        {
            int _isMutiValue = 0;//判断输入对数是否大于1
            bool flag = false; // false--由组量求总量；true--由总量求组量
            string _tempData = "";  //临时存放每列的输入数据

            for (int j = 0; j < dgv.Rows.Count - 2; j += 2)
            {
                if (dgv[i, j].Value != null && dgv[i, j].Value.ToString() != "" && dgv[i, j + 1].Value != null && dgv[i, j + 1].Value.ToString() != "")
                {
                    dgv[i, j].Style.ForeColor = BCellColor;
                    dgv[i, j + 1].Style.ForeColor = BCellColor;
                    int c = j + 1;
                    _isMutiValue++;     //如果是一对，加1
                    _tempData += i.ToString() + "|" + j.ToString() + "|" + dgv[i, j].Value.ToString() + ";" + i.ToString() + "|" + c.ToString() + "|" + dgv[i, c].Value.ToString() + ";";
                }
            }
            int r = dgv.Rows.Count - 2; //混合油总量的行数
            if (dgv[i, r].Value != null && dgv[i, r].Value.ToString() != "" && dgv[i, r + 1].Value != null && dgv[i, r + 1].Value.ToString() != "")
            {
                dgv[i, r].Style.ForeColor = BCellColor;
                dgv[i, r + 1].Style.ForeColor = BCellColor;
                int c = r + 1; //当前行的下一行
                _isMutiValue++;
                _tempData += i.ToString() + "|" + r.ToString() + "|" + dgv[i, r].Value.ToString() + ";" + i.ToString() + "|" + c.ToString() + "|" + dgv[i, c].Value.ToString() + ";";
                flag = true;
            }
            isMutiValue = _isMutiValue;
            tempData = _tempData;
            return flag;
        }
        #endregion 判断输入参数个数

        #region 判断输入格式
        /// <summary>
        /// 输入格式的判断
        /// </summary>
        /// <param name="dgv">datagridview对象</param>
        /// <param name="lbl">label对象</param>
        /// <param name="i">列号</param>
        /// <param name="tempData">输入数据临时存放变量</param>
        /// <param name="isExistRightRow0"></param>
        /// <param name="inputRow">输入行</param>
        /// <param name="inputData">输入数据</param>
        /// <returns></returns>
        private int examMutiInputFormat(DataGridView dgv, Label lbl, int i, string tempData, out string inputData1, out string inputData2)
        {
            string[] inputDatas = tempData.Split(';');
            bool rowE = false;
            string _inputData1 = "";
            string _inputData2 = "";
            for (int k = 0; k < inputDatas.Length - 1; k += 2)
            {
                string[] data1 = inputDatas[k].Split('|');
                string[] data2 = inputDatas[k + 1].Split('|');

                int rowError1 = Convert.ToInt32(data1[1]) + 1;//该变量是表示原行数从0开始，现在从1开始
                int rowError2 = Convert.ToInt32(data2[1]) + 1;//该变量是表示原行数从0开始，现在从1开始

                _inputData1 += data1[2] + "|";   //输入的数据按照行号从小到大一次相加连接成字符串
                _inputData2 += data2[2] + "|";   //输入的数据按照行号从小到大一次相加连接成字符串

                bool a1 = Regex.IsMatch(data1[2].Trim(), pattern);  //组成混合量的判断
                if (a1 == false)  //如果非数字
                {
                    rowE = true;
                    lbl.Text += "第" + data1[0] + "列第" + rowError1 + "行非数字；";
                    continue;
                }

                bool a2 = Regex.IsMatch(data1[2].Trim(), pattern); //组成粘度的判断
                if (a2 == false)  //如果非数字
                {
                    rowE = true;
                    lbl.Text += "第" + data2[0] + "列第" + rowError2 + "行非数字；";
                    continue;
                }

                if (Convert.ToDouble(data1[2]) <= 0)  //组成混合量范围是否溢出
                {
                    rowE = true;
                    lbl.Text += "第" + data1[0] + "列第" + rowError1 + "行超限；";
                    continue;
                }

                if (Convert.ToDouble(data1[2]) <= 0 || Convert.ToDouble(data1[2]) >= 20000)  //组成粘度范围是否溢出
                {
                    rowE = true;
                    lbl.Text += "第" + data2[0] + "列第" + rowError2 + "行超限；";
                    continue;
                }
            }

            inputData1 = _inputData1;
            inputData2 = _inputData2;

            if (rowE == false)   //输入的数据正确，返回0
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        #endregion
        #endregion

        #region 物性关联--原油类型--计算

        #region 判断求typ原油基属的输入条件的格式
        private int examTypFormat(Label lbl, List<KeyValuePair<string, string>> oit, double downlimit, double uplimit, int i, out string typ)
        {
            string _typ = "";
            bool a = true;
            bool b = true;
            foreach (var item in oit)
            {
                int r = 0;
                foreach (int s in Enum.GetValues(typeof(OilType)))  //循环枚举类型求出行数
                {
                    if (((OilType)s).ToString() == item.Key)
                    { r = s + 1; }
                }

                a = Regex.IsMatch(item.Value.Trim(), pattern);
                if (a == false)
                {

                    this.lblOilTypeE.Text += "第" + i + "列第" + r + "行非数字；";
                }
                else if (Convert.ToDouble(item.Value) <= downlimit || Convert.ToDouble(item.Value) >= uplimit)
                {
                    b = false;
                    this.lblOilTypeE.Text += "第" + i + "列" + r + "行超限；";
                }
            }
            typ = _typ;
            if (a == false)
            {
                return 0;
            }
            else if (b == false)
            {
                return 0;
            }
            else
            {
                if (oit[0].Key == "API1" || oit[0].Key == "API2")
                    _typ = getTypbyapi(Convert.ToDouble(oit[0].Value), Convert.ToDouble(oit[1].Value));
                if (oit[0].Key == "D201" || oit[0].Key == "D202")
                {
                    string api1 = BaseFunction.FunAPIfromD20(oit[0].Value);
                    string api2 = BaseFunction.FunAPIfromD20(oit[1].Value);
                    _typ = getTypbyapi(Convert.ToDouble(api1), Convert.ToDouble(api2));
                }
                if (oit[0].Key == "KFC1" || oit[0].Key == "KFC2")
                    _typ = getTypbykfci(Convert.ToDouble(oit[0].Value), Convert.ToDouble(oit[1].Value));
                if (oit[0].Key == "KFC")
                    _typ = getTypbykfc(Convert.ToDouble(oit[0].Value));
                if (oit[0].Key == "API")
                    _typ = getCLA(Convert.ToDouble(oit[0].Value));
                if (oit[0].Key == "D20")
                {
                    string api = BaseFunction.FunAPIfromD20(oit[0].Value);
                    _typ = getCLA(Convert.ToDouble(api));
                }
                if (oit[0].Key == "SUL")
                    _typ = getSCL(Convert.ToDouble(oit[0].Value));
                if (oit[0].Key == "NET")
                    _typ = getACL(Convert.ToDouble(oit[0].Value));
                if (oit[0].Key == "WAX")
                    _typ = getWCL(Convert.ToDouble(oit[0].Value));
            }
            typ = _typ;
            return 1;
        }
        #endregion

        #region 计算

        //由api1和api2求原油基属
        private string getTypbyapi(double api1, double api2)
        {
            string typ = "";
            if (api1 >= 40 && api2 >= 30) typ = "石蜡基";
            else if (api1 >= 40 && api2 > 20 && api2 < 30) typ = "石蜡-中间基";
            else if (api1 > 33 && api1 < 40 && api2 > 30) typ = "中间-石蜡基";
            else if (api1 > 33 && api1 < 40 && api2 > 20 && api2 < 30) typ = "中间基";
            else if (api1 > 33 && api1 < 40 && api2 <= 20) typ = "中间-环烷基";
            else if (api1 <= 33 && api2 > 20 && api2 < 30) typ = "环烷-中间基";
            else if (api1 <= 33 && api2 <= 20) typ = "环烷基";
            return typ;
        }


        //由d201和d202求原油基属
        private string getTypbyd20i(double d201, double d202)
        {
            string typ = "";
            return typ;
        }

        //由kfc1和kfc2求原油基属
        private string getTypbykfci(double kfc1, double kfc2)
        {
            string typ = "";
            if (kfc1 > 11.9 && kfc2 > 12.2) typ = "石蜡基";
            else if (kfc1 > 11.9 && kfc2 >= 11.5 && kfc2 <= 12.2) typ = "石蜡-中间基";
            else if (kfc1 >= 11.5 && kfc1 <= 11.9 && kfc2 > 12.2) typ = "中间-石蜡基";
            else if (kfc1 >= 11.5 && kfc1 <= 11.9 && kfc2 >= 11.5 && kfc2 <= 12.2) typ = "中间基";
            else if (kfc1 >= 11.5 && kfc1 <= 11.9 && kfc2 < 11.5) typ = "中间-环烷基";
            else if (kfc1 < 11.5 && kfc2 >= 11.5 && kfc2 <= 12.2) typ = "环烷-中间基";
            else if (kfc1 < 11.5 && kfc2 < 11.5) typ = "环烷基";
            return typ;
        }

        //由kfc求原油基属
        private string getTypbykfc(double kfc)
        {
            string typ = "";
            if (kfc > 12.1) typ = "石蜡基";
            else if (kfc <= 12.1 && kfc >= 11.5) typ = "中间基";
            else if (kfc > 10.5 && kfc < 11.5) typ = "环烷基";
            return typ;
        }

        //原油类型
        private string getCLA(double api)
        {
            string cla = "";
            if (api <= 10) cla = "特重原油";
            if (api > 10 && api <= 20) cla = "重质原油";
            if (api > 20 && api <= 32) cla = "中质原油";
            if (api > 32) cla = "轻质原油";
            return cla;
        }

        //硫水平
        private string getSCL(double sul)
        {
            string scl = "";
            if (sul > 0 && sul < 100) scl = "";
            if (sul < 0.5) scl = "低硫";
            if (sul >= 0.5 && sul < 2) scl = "含硫";
            if (sul >= 2) scl = "高硫";
            return scl;
        }

        //酸水平
        private string getACL(double net)
        {
            string acl = "";
            if (net > 0 && net < 100) acl = "";
            if (net <= 0.5) acl = "低酸";
            if (net > 0.5 && net < 1) acl = "含酸";
            if (net > 1) acl = "高酸";
            return acl;
        }

        //蜡水平
        private string getWCL(double wax)
        {
            string wcl = "";
            if (wax > 0 && wax < 100) wcl = "";
            if (wax < 2.5) wcl = "低蜡";
            if (wax >= 2.5 && wax <= 10) wcl = "含蜡";
            if (wax >= 10) wcl = "高蜡";
            return wcl;
        }
        #endregion

        #endregion

        #region 枚举

        #region 质量--表行枚举
        enum Q
        { D20, Kg, T, P, L, B, AG, BG };
        #endregion

        #region 原油类型--表行枚举
        /// <summary>
        /// 原油类型--表行枚举
        /// </summary>
        enum OilType
        { API1, D201, KFC1, API2, D202, KFC2, KFC, API, D20, SUL, NET, WAX }
        #endregion

        #region 闪点、蒸气压--表行枚举
        /// <summary>
        /// 闪点、蒸气压--表行枚举
        /// </summary>
        enum Vapour
        { ICP, ECP, A10, A30, A50, A70, A90, FPO, RVP }
        #endregion

        #region 十六烷指数--表行枚举
        /// <summary>
        /// 十六烷指数--表行枚举
        /// </summary>
        enum Cetane
        { D20, ICP, ECP, MCP, A10, A30, A50, A70, A90, ANI, CI, CEN, DI }
        #endregion

        #region 冰点、烟点、芳烃--表行枚举
        /// <summary>
        /// 冰点、烟点、芳烃--表行枚举
        /// </summary>
        enum Freez
        { D20, ANI, ICP, ECP, A10, A30, A50, A70, A90, MCP, FRZ, SMK, ARV }
        #endregion

        #region 苯胺点--表行枚举
        /// <summary>
        /// 苯胺点--表行枚举
        /// </summary>
        enum Aniline
        { D20, ICP, ECP, A10, A30, A50, A70, A90, MCP, ANI }
        #endregion

        #region 分子量--表行枚举
        /// <summary>
        /// 分子量--表行枚举
        /// </summary>
        enum Mol
        { D20, ICP, ECP, A10, A30, A50, A70, A90, MCP, V04, V08, V10, MW }
        #endregion

        #region 碳氢比--表行枚举
        /// <summary>
        /// 碳氢比--表行枚举
        /// </summary>
        enum CH
        { D20, ICP, ECP, A10, A30, A50, A70, A90, MCP, SUL, CH, HC, H2 }
        #endregion

        #region 芳烃指数、芳烃潜含量--表行枚举
        /// <summary>
        /// 芳烃指数、芳烃潜含量--表行枚举
        /// </summary>
        enum BMCI
        { NAH, ARM, N06, A06, N07, A07, N08, A08, N2A, ARP }
        #endregion

        #region 四组分--表行枚举
        /// <summary>
        /// 四组分--表行枚举
        /// </summary>
        enum Four
        { ICP, ECP, D20, V10, CCR, SUL, SAH, ARS, RES, APH }
        #endregion

        #region 蜡油结构组成--表行枚举
        /// <summary>
        /// 蜡油结构组成--表行枚举
        /// </summary>
        enum Wax
        { D20, R20, D70, R70, MW, SUL, CPP, CNN, CAA, RTT, RNN, RAA }
        #endregion

        #region 渣油结构组成--表行枚举
        /// <summary>
        /// 渣油结构组成--表行枚举
        /// </summary>
        enum Residual
        { D20, CAR, H2, MW, FFA, CII, TCC, CA, RNN, RAA, RTT }
        #endregion

        #region 粘度参数--表行枚举
        /// <summary>
        /// 粘度参数--表行枚举
        /// </summary>
        enum VisPara
        { D20, V04, V10, VG4, V1G, VI }
        #endregion

        #region 酸值--表行枚举
        /// <summary>
        /// 酸值--表行枚举
        /// </summary>
        enum Acid
        { D20, ACD, NET }
        #endregion

        #region 三点粘度--表行枚举
        /// <summary>
        /// 三点粘度--表行枚举
        /// </summary>
        enum Viscosity
        { T1, V1, T2, V2, T3, V3 }
        #endregion

        #region 特性因数--表行枚举
        /// <summary>
        /// 特性因数--表行枚举
        /// </summary>
        enum Property
        { D20, V05, V10, ICP, ECP, A10, A30, A50, A70, A90, MCP, KFC, BMI }
        #endregion

        #region 馏程转换--表行枚举
        private string point1 = "10%点", point2 = "30%点", point3 = "50%点", point4 = "70%点", point5 = "90%点";
        /// <summary>
        /// 馏程转换--表行枚举
        /// </summary>
        enum Distill
        { 初切点, point1, point2, point3, point4, point5, 终切点 }
        #endregion

        #endregion

        #region 快捷菜单

        #region 单位换算类
        // bool flagT = false;
        private void 温度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.panel2.Visible = false;
            this.panel1.Visible = true;
            this.tbcUnit.SelectedIndex = 0;
            this.Text = "工具箱--" + this.tbcUnit.TabPages[0].Text;//温度
        }

        bool flagPressure = false;
        private void 压力ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagPressure == false)
            {
                BindDataPressure();//压力
                flagPressure = true;
            }
            //this.panel2.Visible = false;
            //this.panel1.Visible = true;
            this.tbcUnit.SelectedIndex = 1;
            this.Text = "工具箱--" + this.tbcUnit.TabPages[1].Text;//压力
        }

        bool flagDensity = false;
        private void 浓度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagDensity == false)
            {
                flagDensity = true;
                BindDataDensity();//浓度
            }
            this.panel2.Visible = false;
            this.panel1.Visible = true;
            this.tbcUnit.SelectedIndex = 2;
            this.Text = "工具箱--" + this.tbcUnit.TabPages[2].Text;//浓度
        }

        bool flagQ = false;
        private void 质量ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagQ == false)
            {
                flagQ = true;
                BindDataQ();
            }
            this.panel2.Visible = false;
            this.panel1.Visible = true;
            this.tbcUnit.SelectedIndex = 3;
            this.Text = "工具箱--" + this.tbcUnit.TabPages[3].Text;//质量
        }

        //bool flagDensity = false;
        private void 热量ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //this.panel2.Visible = false;
            //this.panel1.Visible = true;
            //this.tbcUnit.SelectedIndex = 4;
            //this.Text = "工具箱--" + this.tbcUnit.TabPages[4].Text;
        }
        #endregion

        #region 物性类
        bool flagMiDu = false;
        private void 密度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagMiDu == false)
            {
                flagMiDu = true;
                BindDataMiDu();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 0;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[0].Text;
        }

        bool flagAcid = false;
        private void 酸度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagAcid == false)
            {
                flagAcid = true;
                BindDataAcid();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 1;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[1].Text;
        }

        bool flagViscosity = false;
        private void 粘度ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagViscosity == false)
            {
                flagViscosity = true;
                BindDataViscosity();
                BindDataMixViscosity();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 2;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[2].Text;
        }

        bool flagProperty = false;
        private void 特性因数ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagProperty == false)
            {
                flagProperty = true;
                BindDataProperty();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 3;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[3].Text;
        }

        bool flagOilType = false;
        private void 原油类型ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagOilType == false)
            {
                flagOilType = true;
                BindDataOilType();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 4;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[4].Text;
        }

        bool flagDistill = false;
        private void 馏程转换ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagDistill == false)
            {
                flagDistill = true;
                BindDataDistill();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 5;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[5].Text;
        }

        bool flagVapour = false;
        private void 闪点蒸气压ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagVapour == false)
            {
                flagVapour = true;
                BindDataVapour();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 6;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[6].Text;
        }

        bool flagCetane = false;
        private void 十六烷指数ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagCetane == false)
            {
                flagCetane = true;
                BindDataCetane();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 7;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[7].Text;
        }

        bool flagFreez = false;
        private void 冰点烟点芳烃ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagFreez == false)
            {
                flagFreez = true;
                BindDataFreez();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 8;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[8].Text;
        }

        bool flagAniline = false;
        private void 苯胺点ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagAniline == false)
            {
                flagAniline = true;
                BindDataAniline();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 9;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[9].Text;
        }

        bool flagMol = false;
        private void 分子量ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagMol == false)
            {
                flagMol = true;
                BindDataMol();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 10;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[10].Text;
        }

        bool flagCH = false;
        private void 碳氢比ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagCH == false)
            {
                flagCH = true;
                BindDataCH();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 11;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[11].Text;
        }

        bool flagBMCI = false;
        private void 芳烃指数芳烃潜含量ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagBMCI == false)
            {
                flagBMCI = true;
                BindDataBMCI();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 12;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[12].Text;
        }

        bool flagFour = false;
        private void 四组分ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagFour == false)
            {
                flagFour = true;
                BindDataFour();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 13;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[13].Text;
        }

        bool flagWax = false;
        private void 蜡油结构组成ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagWax == false)
            {
                flagWax = true;
                BindDataWax();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 14;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[14].Text;
        }

        bool flagResidual = false;
        private void 渣油结构组成ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagResidual == false)
            {
                flagResidual = true;
                BindDataResidual();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 15;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[15].Text;
        }

        bool flagVisPara = false;
        private void 粘度参数ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (flagVisPara == false)
            {
                flagVisPara = true;
                BindDataVisPara();
            }
            this.panel1.Visible = false;
            this.panel2.Visible = true;
            this.tbcPhysic.SelectedIndex = 16;
            this.Text = "工具箱--" + this.tbcPhysic.TabPages[16].Text;
        }
        #endregion


        #region 操作-复制、剪切、粘贴、删除

        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            if (this.panel1.Visible == true)
            {
                int s = this.tbcUnit.SelectedIndex;
                foreach (System.Windows.Forms.Control control in this.tbcUnit.TabPages[s].Controls)
                {
                    if (control is System.Windows.Forms.DataGridView)
                    {
                        DataGridView dgv = (DataGridView)control;
                        GridOilDataEdit.CopyToClipboard(dgv);
                    }
                }
            }
            else
            {
                int s = this.tbcPhysic.SelectedIndex;
                foreach (System.Windows.Forms.Control control in this.tbcPhysic.TabPages[s].Controls)
                {

                    if (control is System.Windows.Forms.DataGridView)
                    {
                        DataGridView dgv = (DataGridView)control;

                        if (dgv == this.dgvViscosity || dgv == this.dgvMixViscosity)
                        {
                            if (dgvViscosity.SelectedCells.Count > dgvMixViscosity.SelectedCells.Count)
                            {
                                GridOilDataEdit.CopyToClipboard(this.dgvViscosity);
                                this.dgvViscosity.ClearSelection();
                            }
                            else
                            {
                                GridOilDataEdit.CopyToClipboard(this.dgvMixViscosity);
                                this.dgvMixViscosity.ClearSelection();
                            }
                        }
                        else
                        {
                            GridOilDataEdit.CopyToClipboard(dgv);
                        }
                        break;
                    }
                }
            }
        }

        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            if (this.panel1.Visible == true)
            {
                int s = this.tbcUnit.SelectedIndex;
                foreach (System.Windows.Forms.Control control in this.tbcUnit.TabPages[s].Controls)
                {
                    if (control is System.Windows.Forms.DataGridView)
                    {
                        DataGridView dgv = (DataGridView)control;
                        GridOilDataEdit.PasteClipboardValue(dgv);
                    }
                }
            }
            else
            {
                int s = this.tbcPhysic.SelectedIndex;
                foreach (System.Windows.Forms.Control control in this.tbcPhysic.TabPages[s].Controls)
                {
                    if (control is System.Windows.Forms.DataGridView)
                    {
                        DataGridView dgv = (DataGridView)control;
                        if (dgv == this.dgvViscosity || dgv == this.dgvMixViscosity)
                        {
                            if (dgvViscosity.SelectedCells.Count > dgvMixViscosity.SelectedCells.Count)
                            {
                                GridOilDataEdit.PasteClipboardValue(this.dgvViscosity);
                                this.dgvViscosity.ClearSelection();
                            }
                            else
                            {
                                GridOilDataEdit.PasteClipboardValue(this.dgvMixViscosity);
                                this.dgvMixViscosity.ClearSelection();
                            }
                        }
                        else
                        {
                            GridOilDataEdit.PasteClipboardValue(dgv);
                        }
                        break;
                    }
                }
            }
        }

        private void tsmiCut_Click(object sender, EventArgs e)
        {
            if (this.panel1.Visible == true)
            {
                int s = this.tbcUnit.SelectedIndex;
                foreach (System.Windows.Forms.Control control in this.tbcUnit.TabPages[s].Controls)
                {
                    if (control is System.Windows.Forms.DataGridView)
                    {
                        DataGridView dgv = (DataGridView)control;
                        GridOilDataEdit.CopyToClipboard(dgv);
                        GridOilDataEdit.DeleteValues(dgv);
                    }
                }
            }
            else
            {
                int s = this.tbcPhysic.SelectedIndex;
                foreach (System.Windows.Forms.Control control in this.tbcPhysic.TabPages[s].Controls)
                {
                    if (control is System.Windows.Forms.DataGridView)
                    {
                        DataGridView dgv = (DataGridView)control;
                        if (dgv == this.dgvViscosity || dgv == this.dgvMixViscosity)
                        {
                            if (dgvViscosity.SelectedCells.Count > dgvMixViscosity.SelectedCells.Count)
                            {
                                GridOilDataEdit.CopyToClipboard(this.dgvViscosity);
                                GridOilDataEdit.DeleteValues(dgvViscosity);
                            }
                            else
                            {
                                GridOilDataEdit.CopyToClipboard(this.dgvMixViscosity);
                                GridOilDataEdit.DeleteValues(dgvMixViscosity);
                            }
                        }
                        else
                        {
                            GridOilDataEdit.CopyToClipboard(dgv);
                            GridOilDataEdit.DeleteValues(dgv);
                        }
                        break;
                    }
                }
            }
        }

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if (this.panel1.Visible == true)
            {
                int s = this.tbcUnit.SelectedIndex;
                foreach (System.Windows.Forms.Control control in this.tbcUnit.TabPages[s].Controls)
                {
                    if (control is System.Windows.Forms.DataGridView)
                    {
                        DataGridView dgv = (DataGridView)control;
                        GridOilDataEdit.DeleteValues(dgv);
                    }
                }
            }
            else
            {
                int s = this.tbcPhysic.SelectedIndex;
                foreach (System.Windows.Forms.Control control in this.tbcPhysic.TabPages[s].Controls)
                {
                    if (control is System.Windows.Forms.DataGridView)
                    {
                        DataGridView dgv = (DataGridView)control;
                        if (dgv == this.dgvViscosity || dgv == this.dgvMixViscosity)
                        {
                            if (dgvViscosity.SelectedCells.Count > dgvMixViscosity.SelectedCells.Count)
                                GridOilDataEdit.DeleteValues(this.dgvViscosity);
                            else
                                GridOilDataEdit.DeleteValues(this.dgvMixViscosity);
                        }
                        else
                        {
                            GridOilDataEdit.DeleteValues(dgv);
                        }
                        break;
                    }
                }
            }
        }
        #endregion
        #endregion

        #region 物性关联--闪点、蒸气压

        List<string> slVapour = new List<string>();

        /// <summary>
        /// 闪点、蒸气压绑定
        /// </summary>
        private void BindDataVapour()
        {
            this.lblVapourE.Text = "";

            this.dgvVapour.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvVapour.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "初切点,℃");
            this.dgvVapour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "终切点,℃");
            this.dgvVapour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "馏程：10%点,℃");
            this.dgvVapour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "馏程：30%点,℃");
            this.dgvVapour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "馏程：50%点,℃");
            this.dgvVapour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "馏程：70%点,℃");
            this.dgvVapour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "馏程：90%点,℃");
            this.dgvVapour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "闪点，℃");
            this.dgvVapour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVapour, "蒸气压，kPa");
            this.dgvVapour.Rows.Add(rowT);

            this.dgvVapour.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvVapour.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvVapour.Rows)
            {
                c.Height = commonHeight;
            }

            foreach (DataGridViewColumn c in this.dgvVapour.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 120;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComVapour_Click(object sender, EventArgs e)
        {
            this.dgvVapour.EndEdit();
            VapourCom();
        }

        private void VapourCom()
        {
            ClearDgv(this.dgvVapour, slVapour);

            this.lblVapourE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvVapour.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvVapour.Rows.Count; j++)
                {
                    if (this.dgvVapour[i, j].Value != null && this.dgvVapour[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Vapour)j).ToString(), this.dgvVapour[i, j].Value.ToString());
                        this.dgvVapour[i, j].Style.ForeColor = BCellColor;
                    }
                }

                //List<KeyValuePair<string, string>> resultDataFormat = (from p in oilTypeDc where p.Key == "FPO" || p.Key=="RVP" select p).ToList();
                //if (resultDataFormat.Count > 0)//8、9行为计算结果行，必须为空
                //{
                //    this.lblVapourE.Text += "第" + i.ToString() + "列" + "第8、9行必须为空；";
                //    continue;
                //}

                #region 开始计算
                List<KeyValuePair<string, string>> ie = (from p in oilTypeDc where p.Key == "ICP" || p.Key == "ECP" select p).ToList(); //存放ICP和ECP的数据
                List<KeyValuePair<string, string>> a10 = (from p in oilTypeDc where p.Key == "A10" select p).ToList();             //存放A10
                List<KeyValuePair<string, string>> a10a30 = (from p in oilTypeDc where p.Key == "A10" || p.Key == "A30" select p).ToList(); //存放A10和A30的数据
                List<KeyValuePair<string, string>> a50 = (from p in oilTypeDc where p.Key == "A50" select p).ToList();

                int type = 0;  //0--由icp、ecp已知条件求fpo  1--由icp、ecp已知条件求rvp  2--由a10、a30已知条件求rvp 
                Type enumType = typeof(Vapour);
                if (ie.Count() == 2) //ICP,ECP->FPO   ICP, ECP->RVP
                {
                    string fpo = "";
                    string rvp = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 0;  //ICP,ECP->FPO
                    isFormatRight = examVapourFormat(this.lblVapourE, ie, 15, 560, i, out fpo, type, enumType);
                    type = 1; //ICP, ECP->RVP
                    isFormatRight = examVapourFormat(this.lblVapourE, ie, 15, 560, i, out rvp, type, enumType);
                    if (isFormatRight == 0)
                    {
                        if (lblVapourE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            //lblVapourE.Text = lblVapourE.Text.Substring(0, lblVapourE.Text.Length / 2);
                            lblVapourE.Text = subString(this.lblVapourE.Text);
                        }
                        // continue;
                    }
                    else
                    {
                        this.dgvVapour[i, 7].Value = fpo;
                        this.dgvVapour[i, 8].Value = rvp;
                        slVapour.Add(i + "|" + 7);
                        this.dgvVapour[i, 7].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        slVapour.Add(i + "|" + 8);
                        this.dgvVapour[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (a10.Count == 1)  //A10->FPO
                {
                    string fpo = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 3;  //A10->FPO
                    isFormatRight = examVapourFormat(this.lblVapourE, a10, 15, 560, i, out fpo, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (lblVapourE.Text.Length > 0)
                        {
                            lblVapourE.Text = subString(this.lblVapourE.Text);
                        }
                    }
                    else
                    {
                        this.dgvVapour[i, 7].Value = fpo;
                        slVapour.Add(i + "|" + 7);
                        this.dgvVapour[i, 7].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (a10a30.Count == 2)  //A10,A30->RVP
                {
                    string rvp = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 2;   //A10,A30->RVP
                    isFormatRight = examVapourFormat(this.lblVapourE, a10a30, 15, 560, i, out rvp, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (lblVapourE.Text.Length > 0)
                        {
                            lblVapourE.Text = subString(this.lblVapourE.Text);
                        }
                    }
                    else
                    {
                        this.dgvVapour[i, 8].Value = rvp;
                        slVapour.Add(i + "|" + 8);
                        this.dgvVapour[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                #endregion
            }

            #endregion

            tooltipEContent(this.lblVapourE);
        }

        //清除所有单元格
        private void tsmiClearVapour_Click(object sender, EventArgs e)
        {
            VapourClear();
        }

        private void VapourClear()
        {
            this.dgvVapour.Rows.Clear();
            this.dgvVapour.Columns.Clear();
            slVapour.Clear();
            this.lblVapourE.Text = "";
            BindDataVapour();
        }

        //选项事件
        private void tsmiChoVapour_Click(object sender, EventArgs e)
        {
            VapourCho();
        }
        private void VapourCho()
        {
            this.contextMenuStrip2.Show(this.menuStripVapour, new Point(this.tsmiChoVapour.Width + 80, this.tsmiChoVapour.Height));
        }

        //悬浮文字
        private void lblVapourE_MouseHover(object sender, EventArgs e)
        {
            if (lblVapourE.Text.Length > 35)
            {
                this.lblVapourE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //行标头添加序列
        private void dgvVapour_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
          e.RowBounds.Location.Y,
          this.dgvT.RowHeadersWidth - 4,
          e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        //单击编辑单元格
        private void dgvVapour_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvVapour.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvVapour.CurrentCell = this.dgvVapour.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvVapour.BeginEdit(true);//将单元格设为编辑状态
            }
        }
        #endregion

        #region 物性关联--十六烷指数

        List<string> slCetane = new List<string>();

        /// <summary>
        /// 十六烷指数绑定
        /// </summary>
        private void BindDataCetane()
        {
            this.lblCetaneE.Text = "";

            this.dgvCetane.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvCetane.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "20℃密度,g/cm3");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "初切点，℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "终切点,℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "中平均沸点,℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "馏程：10%点,℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "馏程：30%点,℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "馏程：50%点,℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "馏程：70%点,℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "馏程：90%点,℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "苯胺点,℃");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "十六烷指数(ASTM D976)");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "十六烷指数(ASTM D4737)");
            this.dgvCetane.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCetane, "柴油指数");
            this.dgvCetane.Rows.Add(rowT);

            this.dgvCetane.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvCetane.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvCetane.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvCetane.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 92;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComCetane_Click(object sender, EventArgs e)
        {
            this.dgvCetane.EndEdit();
            CetaneCom();
        }

        private void CetaneCom()
        {
            ClearDgv(this.dgvCetane, slCetane);

            this.lblCetaneE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvCetane.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvCetane.Rows.Count; j++)
                {
                    if (this.dgvCetane[i, j].Value != null && this.dgvCetane[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Cetane)j).ToString(), this.dgvCetane[i, j].Value.ToString());
                        this.dgvCetane[i, j].Style.ForeColor = BCellColor;
                    }
                }
                //List<KeyValuePair<string, string>> resultDataFormat = (from p in oilTypeDc where p.Key == "CI" || p.Key == "CEN" || p.Key == "DI" select p).ToList();
                //if (resultDataFormat.Count > 0)//计算结果11-13行必须为空
                //{
                //    this.lblCetaneE.Text += "第" + i.ToString() + "列" + "第11到13行必须为空；";
                //    continue;
                //}

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ICP" || p.Key == "ECP" select p).ToList(); //D20,ICP,ECP->CI
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "A10" || p.Key == "A30" || p.Key == "A50" || p.Key == "A70" || p.Key == "A90" select p).ToList();  //D20,A10,A30,A50,A70,A90->CI
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "A10" || p.Key == "A50" || p.Key == "A90" select p).ToList(); //D20,A10,A30,A50,A90->CEN
                List<KeyValuePair<string, string>> inputData4 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "MCP" select p).ToList(); //D20 ,MCP->CI 
                List<KeyValuePair<string, string>> inputData5 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ANI" select p).ToList();//D20,ANI->DI

                int type = 0;
                Type enumType = typeof(Cetane);
                if (inputData1.Count() == 3) //D20,ICP,ECP->CI 
                {
                    string ci = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 15;  //D20,ICP,ECP->CI 
                    isFormatRight = examFreezeFormat(this.lblCetaneE, inputData1, 0.7, 0.95, 100, 400, i, out ci, type, enumType);

                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvCetane[i, 10].Value = ci;
                        slCetane.Add(i + "|" + 10);
                        this.dgvCetane[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData4.Count() == 2) //D20 ,MCP->CI  13
                {
                    string ci = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 18;  //D20 ,MCP->CI  13
                    isFormatRight = examFreezeFormat(this.lblCetaneE, inputData4, 0.7, 0.95, 100, 400, i, out ci, type, enumType);

                    if (isFormatRight == 0)
                    {
                        if (this.lblCetaneE.Text.Length > 0)
                            lblCetaneE.Text = subString(this.lblCetaneE.Text);
                        // continue;
                    }
                    else
                    {
                        this.dgvCetane[i, 10].Value = ci;
                        slCetane.Add(i + "|" + 10);
                        this.dgvCetane[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData2.Count() == 6) //D20,A10,A30,A50,A70,A90->CI 
                {
                    string ci = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 16;  //D20,A10,A30,A50,A70,A90->CI 
                    isFormatRight = examFreezeFormat(this.lblCetaneE, inputData2, 0.7, 0.95, 100, 400, i, out ci, type, enumType);

                    if (isFormatRight == 0)
                    {
                        if (this.lblCetaneE.Text.Length > 0)
                            lblCetaneE.Text = subString(this.lblCetaneE.Text);
                        //continue;
                    }
                    else
                    {
                        this.dgvCetane[i, 10].Value = ci;
                        slCetane.Add(i + "|" + 10);
                        this.dgvCetane[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData3.Count() == 4) //D20,A10,A30,A50,A90->CEN 12
                {
                    string cen = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 17;  //D20,A10,A30,A50,A90->CEN
                    isFormatRight = examFreezeFormat(this.lblCetaneE, inputData3, 0.7, 0.95, 100, 400, i, out cen, type, enumType);

                    if (isFormatRight == 0)
                    {
                        if (this.lblCetaneE.Text.Length > 0)
                            lblCetaneE.Text = subString(this.lblCetaneE.Text);
                        //continue;
                    }
                    else
                    {
                        this.dgvCetane[i, 11].Value = cen;
                        slCetane.Add(i + "|" + 11);
                        this.dgvCetane[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData5.Count() == 2) //D20,ANI->DI 19
                {
                    string ci = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 19;  //D20,ANI->DI
                    isFormatRight = examFreezeFormat(this.lblCetaneE, inputData5, 0.7, 0.95, 50, 100, i, out ci, type, enumType);

                    if (isFormatRight == 0)
                    {
                        if (this.lblCetaneE.Text.Length > 0)
                            lblCetaneE.Text = subString(this.lblCetaneE.Text);
                        // continue;
                    }
                    else
                    {
                        this.dgvCetane[i, 12].Value = ci;
                        slCetane.Add(i + "|" + 12);
                        this.dgvCetane[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }
                #endregion
            }
            #endregion

            tooltipEContent(this.lblCetaneE);
        }

        //清除所有单元格
        private void tsmiClearCetane_Click(object sender, EventArgs e)
        {
            CetaneClear();
        }

        private void CetaneClear()
        {
            this.dgvCetane.Rows.Clear();
            this.dgvCetane.Columns.Clear();
            slCetane.Clear();
            this.lblCetaneE.Text = "";
            BindDataCetane();
        }

        //选项事件
        private void tsmiChoCetane_Click(object sender, EventArgs e)
        {
            CetaneCho();
        }
        private void CetaneCho()
        {
            this.contextMenuStrip2.Show(this.menuStripCetane, new Point(this.tsmiChoCetane.Width + intervalWidth, this.tsmiChoCetane.Height));
        }

        //悬浮文字
        private void lblCetaneE_MouseHover(object sender, EventArgs e)
        {
            if (lblCetaneE.Text.Length > 35)
            {
                this.lblCetaneE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvCetane_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvCetane.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvCetane.CurrentCell = this.dgvCetane.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvCetane.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标头添加序列
        private void dgvCetane_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
          e.RowBounds.Location.Y,
          this.dgvT.RowHeadersWidth - 4,
          e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--冰点、烟点、芳烃

        List<string> slFreez = new List<string>();

        /// <summary>
        /// 绑定冰点、烟点、芳烃
        /// </summary>
        private void BindDataFreez()
        {
            this.lblFreezE.Text = "";

            this.dgvFreez.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvFreez.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "20℃密度,g/cm3");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "苯胺点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "初切点，℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "终切点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "馏程：10%点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "馏程：30%点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "馏程：50%点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "馏程：70%点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "馏程：90%点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "中平均沸点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "冰点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "烟点,℃");
            this.dgvFreez.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFreez, "芳烃，v%");
            this.dgvFreez.Rows.Add(rowT);

            this.dgvFreez.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvFreez.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvFreez.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvFreez.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 63;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComFreeze_Click(object sender, EventArgs e)
        {
            this.dgvFreez.EndEdit();
            FreezCom();
        }

        private void FreezCom()
        {
            ClearDgv(this.dgvFreez, slFreez);

            this.lblFreezE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvFreez.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvFreez.Rows.Count; j++)
                {
                    if (this.dgvFreez[i, j].Value != null && this.dgvFreez[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Freez)j).ToString(), this.dgvFreez[i, j].Value.ToString());
                        this.dgvFreez[i, j].Style.ForeColor = BCellColor;
                    }
                }
                
                //List<KeyValuePair<string, string>> resultDataList = (from p in oilTypeDc where p.Key == "FRZ" || p.Key == "SMK" || p.Key == "ARV" select p).ToList();
                //if (resultDataList.Count > 0)//计算结果行必须为空
                //{
                //    this.lblFreezE.Text += "第" + i.ToString() + "列" + "第11到13行数据必须为空；";
                //    continue;
                //}

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ANI" select p).ToList(); //D20, ANI -> SMK    ------D20,ANI->SMK and D20->SG and SG,ANI->SMK
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ICP" || p.Key == "ECP" select p).ToList();  //D20,ICP,ECP ->SMK      D20,ICP,ECP->FRZ   D20, ICP, ECP ->SAV, ARV    D20->API and API,ICP,ECP->SMK
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "A10" || p.Key == "A30" || p.Key == "A50" || p.Key == "A70" || p.Key == "A90" select p).ToList(); //D20,A10,A30,A50,A70,A90 ->SMK     D20,A10,A30, A50, A70 ,A90->FRZ  D20,A10,A30,A50,A70,A90->SAV
                List<KeyValuePair<string, string>> inputData4 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "MCP" select p).ToList(); //D20,MCP->SMK  D20,MCP ->FRZ   D20,MCP ->SAV, ARV  -----D20,MCP->SMK and D20->API and API,MCP->SMK

                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Freez);

                if (inputData1.Count() == 2) //  D20, ANI -> SMK ---- D20,ANI->SMK and D20->SG and SG,ANI->SMK  SMK=-255.26+2.04*ANI-240.8*LN(SG)+7727*SG/ANI
                {
                    string smk = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 0;  //  D20, ANI -> SMK
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData1, 0.5, 1.1, 50, 100, i, out smk, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvFreez[i, 11].Value = smk;   //给smk单元格赋值
                        slFreez.Add(i + "|" + 11);
                        this.dgvFreez[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData2.Count() == 3) //D20,ICP,ECP ->SMK      D20,ICP,ECP->FRZ   D20, ICP, ECP ->SAV, ARV    D20->API and API,ICP,ECP->SMK
                {
                    string lblText = lblFreezE.Text;

                    string smk = "";
                    string frz = "";
                    string arv = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 1;  //D20,ICP,ECP ->SMK 
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData2, 0.5, 1.1, 15, 560, i, out smk, type, enumType);
                    type = 2; //D20,ICP,ECP->FRZ
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData2, 0.5, 1.1, 15, 560, i, out frz, type, enumType);
                    type = 3; //D20, ICP, ECP ->SAV, ARV
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData2, 0.5, 1.1, 15, 560, i, out arv, type, enumType);
                    if (isFormatRight == 0)
                    {
                        if (lblFreezE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            lblFreezE.Text = subString(this.lblFreezE.Text);
                            // lblFreezE.Text = lblFreezE.Text.Substring(0, lblFreezE.Text.Length / 3);
                        }
                        //continue;
                    }
                    else
                    {
                        this.dgvFreez[i, 11].Value = smk;   //给smk单元格赋值
                        this.dgvFreez[i, 10].Value = frz;   //给frz单元格赋值
                        this.dgvFreez[i, 12].Value = arv;   //给arv单元格赋值
                        slFreez.Add(i + "|" + 11);
                        this.dgvFreez[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        slFreez.Add(i + "|" + 10);
                        this.dgvFreez[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        slFreez.Add(i + "|" + 12);
                        this.dgvFreez[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }

                }

                if (inputData3.Count() == 6) //D20,A10,A30,A50,A70,A90 ->SMK     D20,A10,A30, A50, A70 ,A90->FRZ  D20,A10,A30,A50,A70,A90->SAV
                {
                    string smk = "";
                    string frz = "";
                    string arv = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 4; //D20,A10,A30,A50,A70,A90 ->SMK 
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData3, 0.5, 1.1, 15, 560, i, out smk, type, enumType);
                    type = 5; //D20,A10,A30, A50, A70 ,A90->FRZ
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData3, 0.5, 1.1, 15, 560, i, out frz, type, enumType);
                    type = 6; // D20,A10,A30,A50,A70,A90->SAV
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData3, 0.5, 1.1, 15, 560, i, out arv, type, enumType);
                    if (isFormatRight == 0)
                    {
                        if (lblFreezE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            // lblFreezE.Text = lblFreezE.Text.Substring(0, lblFreezE.Text.Length / 3);
                            lblFreezE.Text = subString(lblFreezE.Text);
                        }
                        //continue;
                    }
                    else
                    {
                        this.dgvFreez[i, 11].Value = smk;   //给smk单元格赋值
                        this.dgvFreez[i, 10].Value = frz;   //给frz单元格赋值
                        this.dgvFreez[i, 12].Value = arv;   //给arv单元格赋值
                        slFreez.Add(i + "|" + 11);
                        this.dgvFreez[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        slFreez.Add(i + "|" + 10);
                        this.dgvFreez[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        slFreez.Add(i + "|" + 12);
                        this.dgvFreez[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData4.Count() == 2) //D20,MCP->SMK  D20,MCP ->FRZ   D20,MCP ->SAV, ARV  
                {
                    string smk = "";
                    string frz = "";
                    string arv = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 7;//D20,MCP->SMK 
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData4, 0.5, 1.1, 15, 560, i, out smk, type, enumType);
                    type = 8;//D20,MCP ->FRZ 
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData4, 0.5, 1.1, 15, 560, i, out frz, type, enumType);
                    type = 9;//D20,MCP ->SAV, ARV  
                    isFormatRight = examFreezeFormat(this.lblFreezE, inputData4, 0.5, 1.1, 15, 560, i, out arv, type, enumType);
                    if (isFormatRight == 0)
                    {
                        if (lblFreezE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            //lblFreezE.Text = lblFreezE.Text.Substring(0, lblFreezE.Text.Length / 3);
                            lblFreezE.Text = subString(lblFreezE.Text);
                        }
                        //continue;
                    }
                    else
                    {
                        this.dgvFreez[i, 11].Value = smk;   //给smk单元格赋值
                        this.dgvFreez[i, 10].Value = frz;   //给frz单元格赋值
                        this.dgvFreez[i, 12].Value = arv;   //给arv单元格赋值
                        slFreez.Add(i + "|" + 11);
                        this.dgvFreez[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        slFreez.Add(i + "|" + 10);
                        this.dgvFreez[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        slFreez.Add(i + "|" + 12);
                        this.dgvFreez[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }

                }
                #endregion
            }
            #endregion

            tooltipEContent(this.lblFreezE);
        }

        //清除所有单元格

        private void tsmiClearFreeze_Click(object sender, EventArgs e)
        {
            FreezClear();
        }

        private void FreezClear()
        {
            this.dgvFreez.Rows.Clear();
            this.dgvFreez.Columns.Clear();
            slFreez.Clear();
            this.lblFreezE.Text = "";
            BindDataFreez();
        }

        //选项事件
        private void tsmiChoFreeze_Click(object sender, EventArgs e)
        {
            FreezCho();
        }
        private void FreezCho()
        {
            this.contextMenuStrip2.Show(this.menuStripFreeze, new Point(this.tsmiChoFreeze.Width + intervalWidth, this.tsmiChoFreeze.Height));
        }

        private void lblFreezE_MouseHover(object sender, EventArgs e)
        {
            if (lblFreezE.Text.Length > 35)
            {
                this.lblFreezE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvFreez_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvFreez.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvFreez.CurrentCell = this.dgvFreez.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvFreez.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        private void dgvFreez_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--苯胺点

        List<string> slAniline = new List<string>();

        /// <summary>
        /// 绑定苯胺点
        /// </summary>
        private void BindDataAniline()
        {
            this.lblAnilineE.Text = "";

            this.dgvAniline.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvAniline.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "20℃密度,g/cm3");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "初切点，℃");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "终切点,℃");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "馏程：10%点,℃");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "馏程：30%点,℃");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "馏程：50%点,℃");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "馏程：70%点,℃");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "馏程：90%点,℃");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "中平均沸点,℃");
            this.dgvAniline.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvAniline, "苯胺点,℃");
            this.dgvAniline.Rows.Add(rowT);

            this.dgvAniline.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvAniline.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvAniline.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvAniline.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 63;
                    c.Frozen = true;
                }
                else
                    c.Width = 50;
            }
        }

        //计算
        private void tsmiComAniline_Click(object sender, EventArgs e)
        {
            this.dgvAniline.EndEdit();
            AnilineCom();
        }

        private void AnilineCom()
        {
            ClearDgv(this.dgvAniline, slAniline);

            this.lblAnilineE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvAniline.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvAniline.Rows.Count; j++)
                {
                    if (this.dgvAniline[i, j].Value != null && this.dgvAniline[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Aniline)j).ToString(), this.dgvAniline[i, j].Value.ToString());
                        this.dgvAniline[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ICP" || p.Key == "ECP" select p).ToList(); //D20,ICP,ECP->ANI
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "A10" || p.Key == "A30" || p.Key == "A50" || p.Key == "A70" || p.Key == "A90" select p).ToList();  //D20,A10,A30,A50,A70,A90->ANI     
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "MCP" select p).ToList(); //D20,MCP->ANI

                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Aniline);
                if (inputData1.Count() == 3) // D20,ICP,ECP->ANI
                {
                    string ani = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 10;  //  D20,ICP,ECP->ANI
                    isFormatRight = examFreezeFormat(this.lblAnilineE, inputData1, 0.5, 1.1, 15, 560, i, out ani, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvAniline[i, 9].Value = ani;   //给ani单元格赋值
                        slAniline.Add(i + "|" + 9);
                        this.dgvAniline[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData2.Count() == 6) //D20,A10,A30,A50,A70,A90 ->ANI
                {
                    string ani = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 11;  //D20,ICP,ECP ->SMK 
                    isFormatRight = examFreezeFormat(this.lblAnilineE, inputData2, 0.5, 1.1, 100, 560, i, out ani, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (this.lblAnilineE.Text.Length > 0)
                        {
                            this.lblAnilineE.Text = subString(this.lblAnilineE.Text);
                        }
                    }
                    else
                    {
                        this.dgvAniline[i, 9].Value = ani;   //给ani单元格赋值
                        slAniline.Add(i + "|" + 9);
                        this.dgvAniline[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }

                }

                if (inputData3.Count() == 2) //D20,MCP->ANI
                {
                    string ani = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 12;
                    isFormatRight = examFreezeFormat(this.lblAnilineE, inputData3, 0.5, 1.1, 100, 560, i, out ani, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (this.lblAnilineE.Text.Length > 0)
                        {
                            this.lblAnilineE.Text = subString(this.lblAnilineE.Text);
                        }
                    }
                    else
                    {
                        this.dgvAniline[i, 9].Value = ani;   //给ani单元格赋值
                        slAniline.Add(i + "|" + 9);
                        this.dgvAniline[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }


                #endregion
            }
            #endregion

            tooltipEContent(lblAnilineE);
        }

        //清除
        private void tsmiClearAniline_Click(object sender, EventArgs e)
        {
            AnilineClear();
        }

        private void AnilineClear()
        {
            this.dgvAniline.Rows.Clear();
            this.dgvAniline.Columns.Clear();
            slAniline.Clear();
            this.lblAnilineE.Text = "";
            BindDataAniline();
        }

        //选项事件
        private void tsmiChoAniline_Click(object sender, EventArgs e)
        {
            AnilineCho();
        }
        private void AnilineCho()
        {
            this.contextMenuStrip2.Show(this.menuStripAniline, new Point(this.tsmiChoAniline.Width + intervalWidth, this.tsmiChoAniline.Height));
        }

        private void lblAnilineE_MouseHover(object sender, EventArgs e)
        {
            if (lblAnilineE.Text.Length > 35)
            {
                this.lblAnilineE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvAniline_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvAniline.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvAniline.CurrentCell = this.dgvAniline.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvAniline.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标头添加序列
        private void dgvAniline_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--分子量

        List<string> slMol = new List<string>();

        /// <summary>
        /// 绑定分子量
        /// </summary>
        private void BindDataMol()
        {
            this.lblMolE.Text = "";

            this.dgvMol.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvMol.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "20℃密度,g/cm3");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "初切点，℃");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "终切点,℃");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "馏程：10%点,℃");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "馏程：30%点,℃");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "馏程：50%点,℃");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "馏程：70%点,℃");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "馏程：90%点,℃");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "中平均沸点,℃");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "40℃运动粘度，mm2/s");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "80℃运动粘度，mm2/s");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "100℃运动粘度,mm2/s");
            this.dgvMol.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvMol, "分子量");
            this.dgvMol.Rows.Add(rowT);

            this.dgvMol.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvMol.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvMol.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvMol.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 76;
                    c.Frozen = true;
                }
                else
                    c.Width = 50;
            }
        }

        //计算
        private void tsmiComMol_Click(object sender, EventArgs e)
        {
            this.dgvMol.EndEdit();
            MolCom();
        }

        private void MolCom()
        {
            ClearDgv(this.dgvMol, slMol);

            this.lblMolE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvMol.Columns.Count; i++)
            {
                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvMol.Rows.Count; j++)
                {
                    if (this.dgvMol[i, j].Value != null && this.dgvMol[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Mol)j).ToString(), this.dgvMol[i, j].Value.ToString());
                        this.dgvMol[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ICP" || p.Key == "ECP" select p).ToList(); //D20,ICP,ECP->MW
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "A10" || p.Key == "A30" || p.Key == "A50" || p.Key == "A70" || p.Key == "A90" select p).ToList();  //D20,A10,A30,A50,A70,A90->MW     
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "MCP" select p).ToList(); //D20,MCP->MW
                List<KeyValuePair<string, string>> inputData4 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "V04" || p.Key == "V10" select p).ToList(); //D20,V04,V10->MW
                List<KeyValuePair<string, string>> inputData5 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "V08" || p.Key == "V10" select p).ToList(); //D20,V08,V10->MW

                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Mol);
                if (inputData1.Count() == 3) // D20,ICP,ECP->MW
                {
                    string mw = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 33;  //  D20,ICP,ECP->ANI
                    isFormatRight = examFreezeFormat(this.lblMolE, inputData1, 0.5, 1.1, 15, 560, i, out mw, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;

                    }
                    else
                    {
                        this.dgvMol[i, 12].Value = mw;   //给ani单元格赋值
                        slMol.Add(i + "|" + 12);
                        this.dgvMol[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData2.Count() == 6) //D20,A10,A30,A50,A70,A90 ->MW
                {
                    string mw = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 34;  //D20,ICP,ECP ->SMK 
                    isFormatRight = examFreezeFormat(this.lblMolE, inputData2, 0.5, 1.1, 15, 560, i, out mw, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (this.lblMolE.Text.Length > 0)
                            lblMolE.Text = subString(lblMolE.Text);
                    }
                    else
                    {
                        this.dgvMol[i, 12].Value = mw;   //给ani单元格赋值
                        slMol.Add(i + "|" + 12);
                        this.dgvMol[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }

                }

                if (inputData3.Count() == 2) //D20,MCP->MW
                {
                    string mw = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 35;
                    isFormatRight = examFreezeFormat(this.lblMolE, inputData3, 0.5, 1.1, 15, 560, i, out mw, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (this.lblMolE.Text.Length > 0)
                            lblMolE.Text = subString(lblMolE.Text);
                    }
                    else
                    {
                        this.dgvMol[i, 12].Value = mw;   //给ani单元格赋值
                        slMol.Add(i + "|" + 12);
                        this.dgvMol[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData4.Count() == 3) //D20,V04,V10->MW
                {
                    string mw = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 36;
                    isFormatRight = examFreezeFormat(this.lblMolE, inputData4, 0.5, 1.1, 0, 20000, i, out mw, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (this.lblMolE.Text.Length > 0)
                            lblMolE.Text = subString(lblMolE.Text);
                    }
                    else
                    {
                        this.dgvMol[i, 12].Value = mw;   //给ani单元格赋值
                        slMol.Add(i + "|" + 12);
                        this.dgvMol[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData5.Count() == 3) //D20,V08,V10->MW
                {
                    string mw = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 37;
                    isFormatRight = examFreezeFormat(this.lblMolE, inputData5, 0.5, 1.1, 0, 20000, i, out mw, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (this.lblMolE.Text.Length > 0)
                            lblMolE.Text = subString(lblMolE.Text);
                    }
                    else
                    {
                        this.dgvMol[i, 12].Value = mw;   //给ani单元格赋值
                        slMol.Add(i + "|" + 12);
                        this.dgvMol[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }


                #endregion
            }
            #endregion

            tooltipEContent(lblMolE);
        }

        //清除所有单元格
        private void tsmiClearMol_Click(object sender, EventArgs e)
        {
            MolClear();
        }

        private void MolClear()
        {
            this.dgvMol.Rows.Clear();
            this.dgvMol.Columns.Clear();
            slMol.Clear();
            this.lblMolE.Text = "";
            BindDataMol();
        }

        //选项事件
        private void tsmiChoMol_Click(object sender, EventArgs e)
        {
            MolCho();
        }
        private void MolCho()
        {
            this.contextMenuStrip2.Show(this.menuStripMol, new Point(this.tsmiChoMol.Width + intervalWidth, this.tsmiChoMol.Height));
        }

        private void lblMolE_MouseHover(object sender, EventArgs e)
        {
            if (lblMolE.Text.Length > 35)
            {
                this.lblMolE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //行标头添加序列
        private void dgvMol_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        //单击编辑单元格
        private void dgvMol_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvMol.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvMol.CurrentCell = this.dgvMol.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvMol.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        #endregion

        #region 物性关联--碳氢比

        List<string> slCH = new List<string>();
        /// <summary>
        /// 碳氢绑定
        /// </summary>
        private void BindDataCH()
        {
            this.lblCHE.Text = "";

            this.dgvCH.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvCH.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "20℃密度,g/cm3");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "初切点，℃");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "终切点,℃");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "馏程：10%点,℃");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "馏程：30%点,℃");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "馏程：50%点,℃");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "馏程：70%点,℃");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "馏程：90%点,℃");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "中平均沸点,℃");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "硫含量，%");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "碳氢比");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "氢碳原子比");
            this.dgvCH.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvCH, "氢含量");
            this.dgvCH.Rows.Add(rowT);

            this.dgvCH.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvCH.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvCH.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvCH.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 60;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
            //this.dgvCH.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }

        //计算
        private void tsmiComCH_Click(object sender, EventArgs e)
        {
            this.dgvCH.EndEdit();
            CHCom();
        }

        private void CHCom()
        {
            ClearDgv(this.dgvCH, slCH);

            this.lblCHE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvCH.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvCH.Rows.Count; j++)
                {
                    if (j == 2)
                    {
                        if (this.dgvCH[i, j].Value == null)
                        {
                            oilTypeDc.Add(((CH)j).ToString(), "2000");
                        }
                        else
                        {
                            oilTypeDc.Add(((CH)j).ToString(), this.dgvCH[i, j].Value.ToString());
                            this.dgvCH[i, j].Style.ForeColor = BCellColor;
                        }
                        //oilTypeDc.Add(((CH)j).ToString(), (this.dgvCH[i, j].Value == string.Empty || this.dgvCH[i, j].Value == null) ? "2000" : this.dgvCH[i, j].Value.ToString());//当ECP为空时，自动给赋值2000
                        //this.dgvCH[i, j].Style.ForeColor = BCellColor;
                    }
                    else if (this.dgvCH[i, j].Value != null && this.dgvCH[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((CH)j).ToString(), this.dgvCH[i, j].Value.ToString());
                        this.dgvCH[i, j].Style.ForeColor = BCellColor;
                    }

                    //if (this.dgvCH[i, j].Value != null && this.dgvCH[i, j].Value.ToString() != "")
                    //{
                    //    oilTypeDc.Add(((CH)j).ToString(), this.dgvCH[i, j].Value.ToString());
                    //    this.dgvCH[i, j].Style.ForeColor = BCellColor;
                    //}
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ICP" || p.Key == "ECP" select p).ToList(); //D20,ICP,ECP->C/H  -----D20, ICP,ECP->C/H  D20->SG MCP=(ICP+ECP)/2  SG,MCP->C/H
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "ECP" select p).ToList(); //IF ECP>=1000 THEN D20->C/H
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "A10" || p.Key == "A30" || p.Key == "A50" || p.Key == "A70" || p.Key == "A90" select p).ToList();  //D20,A10,A30,A50,A70,A90->C/H    
                List<KeyValuePair<string, string>> inputData4 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "MCP" select p).ToList(); //D20,MCP->C/H
                List<KeyValuePair<string, string>> inputData5 = (from p in oilTypeDc where p.Key == "CH" || p.Key == "HC" || p.Key == "H2" select p).ToList(); //H/C->C/H   C/H->H/C     C/H, SUL->H2; H2,SUL->C/H
                List<KeyValuePair<string, string>> inputData6 = (from p in oilTypeDc where p.Key == "HC" select p).ToList(); //H/C->C/H  
                List<KeyValuePair<string, string>> inputData7 = (from p in oilTypeDc where p.Key == "CH" select p).ToList(); // C/H->H/C  
                List<KeyValuePair<string, string>> inputData8 = (from p in oilTypeDc where p.Key == "SUL" || p.Key == "CH" select p).ToList(); //SUL, C/H ->H2;   44
                List<KeyValuePair<string, string>> inputData9 = (from p in oilTypeDc where p.Key == "SUL" || p.Key == "H2" select p).ToList(); // SUL,H2->C/H  45
                List<KeyValuePair<string, string>> inputData10 = (from p in oilTypeDc where p.Key == "SUL" select p).ToList(); // SUL,H2->C/H  45

                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(CH);
                if (inputData5.Count <= 1)//第11,12,13行最多只能有一行有数据
                {
                    if (inputData1.Count() == 3 && Regex.IsMatch(inputData1[2].Value.Trim(), pattern) && Convert.ToInt32( inputData1[2].Value)<=600) // D20,ICP,ECP->C/H
                    {
                        string ch = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 38;  //  D20,ICP,ECP->ANI
                        isFormatRight = examFreezeFormat(this.lblCHE, inputData1, 0.5, 1.1, 15, 560, 0, 600, i, out ch, type, enumType);

                        if (isFormatRight == 0)
                        {
                            //continue;
                        }
                        else
                        {
                            this.dgvCH[i, 10].Value = ch;   //给ch单元格赋值
                            slCH.Add(i + "|" + 10);
                            this.dgvCH[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;

                            //C/H->H/C 
                            Dictionary<string, string> chDic = new Dictionary<string, string>();
                            chDic.Add("CH", ch);
                            List<KeyValuePair<string, string>> chList = (from p in chDic where p.Key == "CH" select p).ToList(); //H/C->C/H  
                            HCfromCH(this.lblCHE, chList, i, "", type, enumType);

                            //C/H, SUL->H2
                            if (inputData10.Count == 1)
                            {
                                inputData10.Add(chList[0]);
                                H2fromSUL_CH(this.lblCHE, inputData10, i, "", type, enumType);
                                inputData10.Remove(chList[0]);
                            }
                        }
                    }

                    if (inputData2.Count() == 2) //IF ECP>=1000 THEN D20->C/H
                    {
                        string ch = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 39;
                        if ( Regex.IsMatch(inputData2[1].Value.Trim(), pattern) && Convert.ToInt32(inputData2[1].Value) > 600)
                        {
                            isFormatRight = examFreezeFormat(this.lblCHE, inputData2, 0.5, 1.1, 0, 999999999, 0.0, 0.0, i, out ch, type, enumType);
                            if (isFormatRight == 0)
                            {
                                //continue;
                                if (this.lblCHE.Text.Length > 0)
                                {
                                    this.lblCHE.Text = subString(this.lblCHE.Text);
                                }
                            }
                            else
                            {
                                this.lblCHE.Text = "";
                                this.dgvCH[i, 10].Value = ch;   //给C/H单元格赋值
                                slCH.Add(i + "|" + 10);
                                this.dgvCH[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;

                                //C/H->H/C 
                                Dictionary<string, string> chDic = new Dictionary<string, string>();
                                chDic.Add("CH", ch);
                                List<KeyValuePair<string, string>> chList = (from p in chDic where p.Key == "CH" select p).ToList(); //H/C->C/H  
                                HCfromCH(this.lblCHE, chList, i, "", type, enumType);

                                //C/H, SUL->H2
                                if (inputData10.Count == 1)
                                {
                                    inputData10.Add(chList[0]);
                                    H2fromSUL_CH(this.lblCHE, inputData10, i, "", type, enumType);
                                    inputData10.Remove(chList[0]);
                                }
                            }
                        }
                    }

                    if (inputData3.Count() == 6) //D20,A10,A30,A50,A70,A90 ->C/H
                    {
                        string ch = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 40;  //D20,A10,A30,A50,A70,A90 ->C/H
                        isFormatRight = examFreezeFormat(this.lblCHE, inputData3, 0.5, 1.1, 15, 560, i, out ch, type, enumType);
                        if (isFormatRight == 0)
                        {
                            //continue;
                            if (this.lblCHE.Text.Length > 0)
                            {
                                this.lblCHE.Text = subString(this.lblCHE.Text);
                            }
                        }
                        else
                        {
                            this.dgvCH[i, 10].Value = ch;   //给C/H单元格赋值
                            slCH.Add(i + "|" + 10);
                            this.dgvCH[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;

                            //C/H->H/C 
                            Dictionary<string, string> chDic = new Dictionary<string, string>();
                            chDic.Add("CH", ch);
                            List<KeyValuePair<string, string>> chList = (from p in chDic where p.Key == "CH" select p).ToList(); //H/C->C/H  
                            HCfromCH(this.lblCHE, chList, i, "", type, enumType);

                            //C/H, SUL->H2
                            if (inputData10.Count == 1)
                            {
                                inputData10.Add(chList[0]);
                                H2fromSUL_CH(this.lblCHE, inputData10, i, "", type, enumType);
                                inputData10.Remove(chList[0]);
                            }
                        }

                    }

                    if (inputData4.Count() == 2) //D20,MCP->C/H
                    {
                        string ch = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 41;
                        isFormatRight = examFreezeFormat(this.lblCHE, inputData4, 0.5, 1.1, 15, 560, i, out ch, type, enumType);
                        if (isFormatRight == 0)
                        {
                            //continue;
                            if (this.lblCHE.Text.Length > 0)
                            {
                                this.lblCHE.Text = subString(this.lblCHE.Text);
                            }
                        }
                        else
                        {
                            this.dgvCH[i, 10].Value = ch;   //给C/H单元格赋值
                            slCH.Add(i + "|" + 10);
                            this.dgvCH[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;

                            //C/H->H/C 
                            Dictionary<string, string> chDic = new Dictionary<string, string>();
                            chDic.Add("CH", ch);
                            List<KeyValuePair<string, string>> chList = (from p in chDic where p.Key == "CH" select p).ToList(); //H/C->C/H  
                            HCfromCH(this.lblCHE, chList, i, "", type, enumType);


                            //C/H, SUL->H2
                            if (inputData10.Count == 1)
                            {
                                inputData10.Add(chList[0]);
                                H2fromSUL_CH(this.lblCHE, inputData10, i, "", type, enumType);
                                inputData10.Remove(chList[0]);
                            }
                        }
                    }

                    if (inputData6.Count() == 1) //H/C->C/H   42
                    {
                        string ch = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 42;
                        isFormatRight = examFreezeFormat(this.lblCHE, inputData6, 0.0, 0.0, 0.0, 0.0, i, out ch, type, enumType);
                        if (isFormatRight == 0)
                        {
                            // continue;
                            if (this.lblCHE.Text.Length > 0)
                            {
                                this.lblCHE.Text = subString(this.lblCHE.Text);
                            }
                        }
                        else
                        {
                            if (this.dgvCH[i, 10].Value == null || this.dgvCH[i, 10].Value.ToString() == "")
                            {
                                this.dgvCH[i, 10].Value = ch;   //给C/H单元格赋值
                                slCH.Add(i + "|" + 10);
                                this.dgvCH[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            }
                        }
                    }

                    if (inputData7.Count() == 1) // C/H->H/C
                    {
                        string hc = "";
                        HCfromCH(this.lblCHE, inputData7, i, hc, type, enumType);

                    }
                    if (inputData8.Count() == 2) //SUL ，C/H->H2;  44
                    {
                        string h2 = "";
                        H2fromSUL_CH(this.lblCHE, inputData8, i, h2, type, enumType);

                    }
                    if (inputData9.Count() == 2) //SUL ，H2->C/H  45
                    {
                        string ch = "";
                        int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                        type = 45;
                        isFormatRight = examFreezeFormat(this.lblCHE, inputData9, 0.0, 100, 10, 20, i, out ch, type, enumType);
                        if (isFormatRight == 0)
                        {
                            //continue;
                            if (this.lblCHE.Text.Length > 0)
                            {
                                this.lblCHE.Text = subString(this.lblCHE.Text);
                            }
                        }
                        else
                        {
                            if (this.dgvCH[i, 10].Value == null || this.dgvCH[i, 10].Value.ToString() == "")
                            {
                                this.dgvCH[i, 10].Value = ch;   //给C/H单元格赋值
                                slCH.Add(i + "|" + 10);
                                this.dgvCH[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                            }

                            //C/H->H/C 
                            Dictionary<string, string> chDic = new Dictionary<string, string>();
                            chDic.Add("CH", ch);
                            List<KeyValuePair<string, string>> chList = (from p in chDic where p.Key == "CH" select p).ToList(); //H/C->C/H  
                            HCfromCH(this.lblCHE, chList, i, "", type, enumType);
                        }
                    }
                }
                else
                {
                    this.lblCHE.Text += "第" + i + "列11,12,13行最多只能有一行有数据;";
                }

                #endregion
            }
            #endregion

            tooltipEContent(lblCHE);
        }

        private void HCfromCH(Label lbl, List<KeyValuePair<string, string>> inputData7, int i, string hc, int type, Type enumType)
        {
            int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
            type = 43;
            isFormatRight = examFreezeFormat(lbl, inputData7, 4, 15, 0.0, 0.0, i, out hc, type, enumType);
            if (isFormatRight == 0)
            {
                // continue;
                if (this.lblCHE.Text.Length > 0)
                {
                    this.lblCHE.Text = subString(this.lblCHE.Text);
                }
            }
            else
            {
                //if (this.dgvCH[i, 11].Value == null || this.dgvCH[i, 11].Value.ToString() == "")
                //{
                //    this.dgvCH[i, 11].Value = hc;   //给C/H单元格赋值
                //    slCH.Add(i + "|" + 11);
                //    this.dgvCH[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                //}

                this.dgvCH[i, 11].Value = hc;   //给H/C单元格赋值
                slCH.Add(i + "|" + 11);
                this.dgvCH[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        private void H2fromSUL_CH(Label lbl, List<KeyValuePair<string, string>> inputData8, int i, string h2, int type, Type enumType)
        {
            int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
            type = 44;
            isFormatRight = examFreezeFormat(lbl, inputData8, 0.0, 100, 4, 15, i, out h2, type, enumType);
            if (isFormatRight == 0)
            {
                //continue;
                if (this.lblCHE.Text.Length > 0)
                {
                    this.lblCHE.Text = subString(this.lblCHE.Text);
                }
            }
            else
            {
                this.dgvCH[i, 12].Value = h2;   //给C/H单元格赋值
                slCH.Add(i + "|" + 12);
                this.dgvCH[i, 12].Style.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        //清除所有单元格

        private void tsmiClearCH_Click(object sender, EventArgs e)
        {
            CHClear();
        }

        private void CHClear()
        {
            this.dgvCH.Rows.Clear();
            this.dgvCH.Columns.Clear();
            slCH.Clear();
            this.lblCHE.Text = "";
            BindDataCH();
        }

        //选项事件     
        private void tsmiChoCH_Click(object sender, EventArgs e)
        {
            CHCho();
        }
        private void CHCho()
        {
            this.contextMenuStrip2.Show(this.menuStripCH, new Point(this.tsmiChoCH.Width + intervalWidth, this.tsmiChoCH.Height));
        }

        private void lblCHE_MouseHover(object sender, EventArgs e)
        {
            if (lblCHE.Text.Length > 35)
            {
                this.lblCHE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvCH_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvCH.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvCH.CurrentCell = this.dgvCH.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvCH.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标头添加列表
        private void dgvCH_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--芳烃指数、芳烃潜含量

        List<string> slBMCI = new List<string>();
        /// <summary>
        /// 芳烃指数、芳烃潜含量绑定
        /// </summary>
        private void BindDataBMCI()
        {
            this.lblBMCIE.Text = "";

            this.dgvBMCI.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvBMCI.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "环烷烃，%");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "芳烃，%");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "C6环烷烃，%");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "C6芳烃，%");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "C7环烷烃，%");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "C7芳烃，%");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "C8环烷烃，%");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "C8芳烃，%");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "N+2A");
            this.dgvBMCI.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvBMCI, "芳烃潜含量，%");
            this.dgvBMCI.Rows.Add(rowT);

            this.dgvBMCI.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvBMCI.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvBMCI.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvBMCI.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 60;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComBMCI_Click(object sender, EventArgs e)
        {
            this.dgvBMCI.EndEdit();
            BMCICom();
        }

        private void BMCICom()
        {
            ClearDgv(this.dgvBMCI, slBMCI);

            this.lblBMCIE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvBMCI.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvBMCI.Rows.Count; j++)
                {
                    if (this.dgvBMCI[i, j].Value != null && this.dgvBMCI[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((BMCI)j).ToString(), this.dgvBMCI[i, j].Value.ToString());
                        this.dgvBMCI[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "NAH" || p.Key == "ARM" select p).ToList(); //NAH,ARM->N2A  
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "N06" || p.Key == "A06" || p.Key == "N07" || p.Key == "A07" || p.Key == "N08" || p.Key == "A08" select p).ToList(); //N06,A06,N07,A07,N08,A08->ARP
                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(BMCI);
                if (inputData1.Count() == 2) //NAH,ARM->N2A 
                {
                    string n2a = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 1;  //NAH,ARM->N2A 
                    isFormatRight = examBMCIFormat(this.lblBMCIE, inputData1, 0, 100, i, out n2a, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;
                    }
                    else
                    {
                        this.dgvBMCI[i, 8].Value = n2a;   //给N2A单元格赋值
                        slBMCI.Add(i + "|" + 8);
                        this.dgvBMCI[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData2.Count() == 6) //N06,A06,N07,A07,N08,A08->ARP
                {
                    string arp = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 2;  //N06,A06,N07,A07,N08,A08->ARP
                    isFormatRight = examBMCIFormat(this.lblBMCIE, inputData2, 0, 100, i, out arp, type, enumType);
                    if (isFormatRight == 0)
                    {
                        //continue;

                    }
                    else
                    {
                        this.dgvBMCI[i, 9].Value = arp;   //给ARP单元格赋值
                        slBMCI.Add(i + "|" + 9);
                        this.dgvBMCI[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }


                #endregion
            }
            #endregion

            tooltipEContent(lblBMCIE);
        }

        //清除所有单元格
        private void tsmiClearBMCI_Click(object sender, EventArgs e)
        {
            BMCIClear();
        }

        private void BMCIClear()
        {
            this.dgvBMCI.Rows.Clear();
            this.dgvBMCI.Columns.Clear();
            slBMCI.Clear();
            this.lblBMCIE.Text = "";
            BindDataBMCI();
        }


        //选项事件
        private void tsmiChoBMCI_Click(object sender, EventArgs e)
        {
            BMCICho();
        }
        private void BMCICho()
        {
            this.contextMenuStrip2.Show(this.menuStripBMCI, new Point(this.tsmiChoBMCI.Width + intervalWidth, this.tsmiChoBMCI.Height));
        }

        private void lblBMCIE_MouseHover(object sender, EventArgs e)
        {
            if (lblBMCIE.Text.Length > 35)
            {
                this.lblBMCIE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvBMCI_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvBMCI.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvBMCI.CurrentCell = this.dgvBMCI.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvBMCI.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标头添加列表
        private void dgvBMCI_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region 物性关联--四组分

        List<string> slFour = new List<string>();
        /// <summary>
        /// 四组分绑定
        /// </summary>
        private void BindDataFour()
        {
            this.lblFourE.Text = "";

            this.dgvFour.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvFour.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "初切点，℃");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "终切点,℃");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "20℃密度,g/cm3");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "重油100℃粘度，mm2/s");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "残炭，%");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "硫含量，%");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "饱和分，%");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "芳香分，%");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "胶质，%");
            this.dgvFour.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvFour, "沥青质，%");
            this.dgvFour.Rows.Add(rowT);

            this.dgvFour.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvFour.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvFour.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvFour.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 60;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComFour_Click(object sender, EventArgs e)
        {
            this.dgvFour.EndEdit();
            FourCom();
        }

        private void FourCom()
        {
            ClearDgv(this.dgvFour, slFour);

            this.lblFourE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvFour.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvFour.Rows.Count; j++)
                {
                    if (j == 1)
                    {
                        oilTypeDc.Add(((Four)j).ToString(), (this.dgvFour[i, j].Value == string.Empty || this.dgvFour[i, j].Value == null) ? "2000" : this.dgvFour[i, j].Value.ToString());//当ECP为空时，自动给赋值2000
                        this.dgvFour[i, j].Style.ForeColor = BCellColor;
                    }
                    else if (this.dgvFour[i, j].Value != null && this.dgvFour[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Four)j).ToString(), this.dgvFour[i, j].Value.ToString());
                        this.dgvFour[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "ICP" || p.Key == "ECP" || p.Key == "D20" || p.Key == "V10" || p.Key == "CCR" || p.Key == "SUL" select p).ToList(); //ICP,ECP,D20,V10,CCR,SUL->SAH,ARS,RES,APH 
                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Four);
                if (inputData1.Count() == 6 ) //ICP,ECP,D20,SUL,V10,CCR->SAH,ARS,RES,APH 
                {
                    string sah = "";
                    string ars = "";
                    string res = "";
                    string aph = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 1;  //ICP,ECP,D20,V10,CCR,SUL->SAH
                    isFormatRight = examCommonFormat(this.lblFourE, inputData1, 300, 9999999, 400, 9999999, 0.5, 1.1, 0, 20000, 0, 100, 0, 100, i, out sah, type, enumType);
                    type = 2;  //ICP,ECP,D20,V10,CCR,SUL->ARS
                    isFormatRight = examCommonFormat(this.lblFourE, inputData1, 300, 9999999, 400, 9999999, 0.5, 1.1, 0, 20000, 0, 100, 0, 100, i, out ars, type, enumType);
                    type = 3;  //ICP,ECP,D20,V10,CCR,SUL->RES
                    isFormatRight = examCommonFormat(this.lblFourE, inputData1, 300, 9999999, 400, 9999999, 0.5, 1.1, 0, 20000, 0, 100, 0, 100, i, out res, type, enumType);
                    type = 4;  //ICP,ECP,D20,V10,CCR,SUL->APH 
                    isFormatRight = examCommonFormat(this.lblFourE, inputData1, 300, 9999999, 400, 9999999, 0.5, 1.1, 0, 20000, 0, 100, 0, 100, i, out aph, type, enumType);
                    if (isFormatRight == 0)
                    {
                        if (lblFourE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            lblFourE.Text = lblFourE.Text.Substring(0, lblFourE.Text.Length / 4);
                        }
                        //continue;
                    }
                    else
                    {
                        this.dgvFour[i, 6].Value = sah;   //给sah单元格赋值
                        this.dgvFour[i, 7].Value = ars;   //给ars单元格赋值
                        this.dgvFour[i, 8].Value = res;   //给res单元格赋值
                        this.dgvFour[i, 9].Value = aph;   //给aph单元格赋值
                        slFour.Add(i + "|" + 6);
                        slFour.Add(i + "|" + 7);
                        slFour.Add(i + "|" + 8);
                        slFour.Add(i + "|" + 9);
                        this.dgvFour[i, 6].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvFour[i, 7].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvFour[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvFour[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                #endregion
            }
            #endregion

            tooltipEContent(lblFourE);
        }

        //清除所有单元格
        private void tsmiClearFour_Click(object sender, EventArgs e)
        {
            FourClear();
        }

        private void FourClear()
        {
            this.dgvFour.Rows.Clear();
            this.dgvFour.Columns.Clear();
            slFour.Clear();
            this.lblFourE.Text = "";
            BindDataFour();
        }

        //选项事件
        private void tsmiChoFour_Click(object sender, EventArgs e)
        {
            FourCho();
        }
        private void FourCho()
        {
            this.contextMenuStrip2.Show(this.menuStripFour, new Point(this.tsmiChoFour.Width + intervalWidth, this.tsmiChoFour.Height));
        }

        private void lblFourE_MouseHover(object sender, EventArgs e)
        {
            if (lblFourE.Text.Length > 35)
            {
                this.lblFourE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvFour_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvFour.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvFour.CurrentCell = this.dgvFour.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvFour.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标题添加列表
        private void dgvFour_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--蜡油结构组成

        List<string> slWax = new List<string>();

        /// <summary>
        /// 绑定蜡油结构组成
        /// </summary>
        private void BindDataWax()
        {
            this.lblWaxE.Text = "";

            this.dgvWax.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvWax.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "20℃密度,g/cm3");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "20℃折光率");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "70℃密度,g/cm3");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "70℃折光率");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "平均分子量");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "硫含量，%");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "Cp%");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "Cn%");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "Ca%");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "Rt");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "Rn");
            this.dgvWax.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvWax, "Ra");
            this.dgvWax.Rows.Add(rowT);

            this.dgvWax.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvWax.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvWax.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvWax.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 60;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComWax_Click(object sender, EventArgs e)
        {
            this.dgvWax.EndEdit();
            WaxCom();
        }

        private void WaxCom()
        {
            ClearDgv(this.dgvWax, slWax);

            this.lblWaxE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvWax.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvWax.Rows.Count; j++)
                {
                    if (this.dgvWax[i, j].Value != null && this.dgvWax[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Wax)j).ToString(), this.dgvWax[i, j].Value.ToString());
                        this.dgvWax[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "R20" || p.Key == "MW" || p.Key == "SUL" select p).ToList(); //D20,R20,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA 
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "D70" || p.Key == "R70" || p.Key == "MW" || p.Key == "SUL" select p).ToList(); //D70,R70,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA
                List<KeyValuePair<string, string>> inputData3 = oilTypeDc.Where(o => o.Key == "D20" || o.Key == "R70" || o.Key == "MW" || o.Key == "SUL").ToList();//D20→D70;D70,R70,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA
                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Wax);
                if (inputData1.Count() == 4) //D20,R20,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA
                {
                    string cpp = "";
                    string cnn = "";
                    string caa = "";
                    string rtt = "";
                    string rnn = "";
                    string raa = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 5;  //D20,R20,MW,SUL->CPP
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData1, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out cpp, type, enumType);
                    type = 6;  //D20,R20,MW,SUL->CNN
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData1, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out cnn, type, enumType);
                    type = 7;  //D20,R20,MW,SUL->CAA
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData1, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out caa, type, enumType);
                    type = 8;  //D20,R20,MW,SUL->RTT
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData1, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out rtt, type, enumType);
                    type = 9;  //D20,R20,MW,SUL->RNN
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData1, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out rnn, type, enumType);
                    type = 10;  //D20,R20,MW,SUL->RAA
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData1, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out raa, type, enumType);
                    if (isFormatRight == 0)
                    {
                        if (lblWaxE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            //lblWaxE.Text = lblWaxE.Text.Substring(0, lblWaxE.Text.Length / 6);
                            lblWaxE.Text = subString(lblWaxE.Text);
                        }
                        // continue;
                    }
                    else
                    {
                        this.dgvWax[i, 6].Value = cpp;   //给cpp单元格赋值
                        this.dgvWax[i, 7].Value = cnn;   //给cnn单元格赋值
                        this.dgvWax[i, 8].Value = caa;   //给caa单元格赋值
                        this.dgvWax[i, 9].Value = rtt;   //给rtt单元格赋值
                        this.dgvWax[i, 10].Value = rnn;   //给rnn单元格赋值
                        this.dgvWax[i, 11].Value = raa;   //给raa单元格赋值
                        slWax.Add(i + "|" + 6);
                        slWax.Add(i + "|" + 7);
                        slWax.Add(i + "|" + 8);
                        slWax.Add(i + "|" + 9);
                        slWax.Add(i + "|" + 10);
                        slWax.Add(i + "|" + 11);
                        this.dgvWax[i, 6].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 7].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData2.Count() == 4) //D70,R70,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA
                {
                    string cpp = "";
                    string cnn = "";
                    string caa = "";
                    string rtt = "";
                    string rnn = "";
                    string raa = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 11;  //D70,R70,MW,SUL->CPP
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData2, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out cpp, type, enumType);
                    type = 12;  //D70,R70,MW,SUL->CNN
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData2, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out cnn, type, enumType);
                    type = 13;  //D70,R70,MW,SUL->CAA
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData2, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out caa, type, enumType);
                    type = 14;  //D70,R70,MW,SUL->RTT
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData2, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out rtt, type, enumType);
                    type = 15;  //D70,R70,MW,SUL->RNN
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData2, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out rnn, type, enumType);
                    type = 16;  //D70,R70,MW,SUL->RAA
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData2, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out raa, type, enumType);
                    if (isFormatRight == 0)
                    {
                        if (lblWaxE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            // lblWaxE.Text = lblWaxE.Text.Substring(0, lblWaxE.Text.Length / 6);
                            lblWaxE.Text = subString(lblWaxE.Text);
                        }
                        // continue;
                    }
                    else
                    {
                        this.dgvWax[i, 6].Value = cpp;   //给cpp单元格赋值
                        this.dgvWax[i, 7].Value = cnn;   //给cnn单元格赋值
                        this.dgvWax[i, 8].Value = caa;   //给caa单元格赋值
                        this.dgvWax[i, 9].Value = rtt;   //给rtt单元格赋值
                        this.dgvWax[i, 10].Value = rnn;   //给rnn单元格赋值
                        this.dgvWax[i, 11].Value = raa;   //给raa单元格赋值
                        slWax.Add(i + "|" + 6);
                        slWax.Add(i + "|" + 7);
                        slWax.Add(i + "|" + 8);
                        slWax.Add(i + "|" + 9);
                        slWax.Add(i + "|" + 10);
                        slWax.Add(i + "|" + 11);
                        this.dgvWax[i, 6].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 7].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData3.Count() == 4) //D20→D70; D70,R70,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA
                {
                    string cpp = "";
                    string cnn = "";
                    string caa = "";
                    string rtt = "";
                    string rnn = "";
                    string raa = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确

                    type = 33;  //D70,R70,MW,SUL->CPP
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData3, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out cpp, type, enumType);
                    type = 34;  //D70,R70,MW,SUL->CNN
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData3, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out cnn, type, enumType);
                    type = 35;  //D70,R70,MW,SUL->CAA
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData3, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out caa, type, enumType);
                    type = 36;  //D70,R70,MW,SUL->RTT
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData3, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out rtt, type, enumType);
                    type = 37;  //D70,R70,MW,SUL->RNN
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData3, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out rnn, type, enumType);
                    type = 38;  //D70,R70,MW,SUL->RAA
                    isFormatRight = examCommonFormat(this.lblWaxE, inputData3, 0.5, 1.1, 1.3, 1.6, 300, 1000, 0, 100, -1.0, -1.0, -1.0, -1.0, i, out raa, type, enumType);
                    if (isFormatRight == 0)
                    {
                        if (lblWaxE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            // lblWaxE.Text = lblWaxE.Text.Substring(0, lblWaxE.Text.Length / 6);
                            lblWaxE.Text = subString(lblWaxE.Text);
                        }
                        // continue;
                    }
                    else
                    {
                        this.dgvWax[i, 6].Value = cpp;   //给cpp单元格赋值
                        this.dgvWax[i, 7].Value = cnn;   //给cnn单元格赋值
                        this.dgvWax[i, 8].Value = caa;   //给caa单元格赋值
                        this.dgvWax[i, 9].Value = rtt;   //给rtt单元格赋值
                        this.dgvWax[i, 10].Value = rnn;   //给rnn单元格赋值
                        this.dgvWax[i, 11].Value = raa;   //给raa单元格赋值
                        slWax.Add(i + "|" + 6);
                        slWax.Add(i + "|" + 7);
                        slWax.Add(i + "|" + 8);
                        slWax.Add(i + "|" + 9);
                        slWax.Add(i + "|" + 10);
                        slWax.Add(i + "|" + 11);
                        this.dgvWax[i, 6].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 7].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                        this.dgvWax[i, 11].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                #endregion
            }
            #endregion

            tooltipEContent(lblWaxE);
        }

        //清除所有单元格
        private void tsmiClearWax_Click(object sender, EventArgs e)
        {
            WaxClear();
        }

        private void WaxClear()
        {
            this.dgvWax.Rows.Clear();
            this.dgvWax.Columns.Clear();
            slWax.Clear();
            this.lblWaxE.Text = "";
            BindDataWax();
        }

        //选项事件
        private void tsmiChoWax_Click(object sender, EventArgs e)
        {
            WaxCho();
        }
        private void WaxCho()
        {
            this.contextMenuStrip2.Show(this.menuStripWax, new Point(this.tsmiChoWax.Width + intervalWidth, this.tsmiChoWax.Height));
        }

        private void lblWaxE_MouseHover(object sender, EventArgs e)
        {
            if (lblWaxE.Text.Length > 35)
            {
                this.lblWaxE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvWax_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvWax.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvWax.CurrentCell = this.dgvWax.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvWax.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标题添加列表
        private void dgvWax_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--渣油结构组成

        List<string> slResidual = new List<string>();

        /// <summary>
        /// 绑定渣油结构组成
        /// </summary>
        private void BindDataResidual()
        {
            this.lblResidualE.Text = "";

            this.dgvResidual.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvResidual.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "20℃密度,g/cm3");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "碳，%");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "氢，%");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "平均分子量");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "芳烃分数，fa");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "缩合指数，CI");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "总碳数，#C");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "芳碳数，#Ca");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "环烷环数，Rn");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "芳环数，Ra");
            this.dgvResidual.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvResidual, "总环数，Rt");
            this.dgvResidual.Rows.Add(rowT);

            this.dgvResidual.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvResidual.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvResidual.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvResidual.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 60;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComResidual_Click(object sender, EventArgs e)
        {
            this.dgvResidual.EndEdit();
            ResidualCom();
        }

        private void ResidualCom()
        {
            ClearDgv(this.dgvResidual, slResidual);

            this.lblResidualE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvResidual.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvResidual.Rows.Count; j++)
                {
                    if (this.dgvResidual[i, j].Value != null && this.dgvResidual[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((Residual)j).ToString(), this.dgvResidual[i, j].Value.ToString());
                        this.dgvResidual[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "CAR" || p.Key == "H2" || p.Key == "MW" select p).ToList(); //D20,CAR,H2,MW->FFA,CII,TCC,RTT 
                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(Residual);
                if (inputData1.Count() == 4) //D20,CAR,H2,MW->FFA,CII,TCC,RTT 
                {
                    string ffa = "";
                    string cii = "";
                    string tcc = "";
                    string ca = "";
                    string rnn = "";
                    string raa = "";
                    string rtt = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 17;  //D20,CAR,H2,MW->FFA
                    isFormatRight = examCommonFormat(this.lblResidualE, inputData1, 0.5, 1.1, 80, 100, 10, 20, 0, 1500, -1.0, -1.0, -1.0, -1.0, i, out ffa, type, enumType);
                    type = 18;  //D20,CAR,H2,MW->CII
                    isFormatRight = examCommonFormat(this.lblResidualE, inputData1, 0.5, 1.1, 80, 100, 10, 20, 0, 1500, -1.0, -1.0, -1.0, -1.0, i, out cii, type, enumType);
                    type = 19;  //D20,CAR,H2,MW->TCC
                    isFormatRight = examCommonFormat(this.lblResidualE, inputData1, 0.5, 1.1, 80, 100, 10, 20, 0, 1500, -1.0, -1.0, -1.0, -1.0, i, out tcc, type, enumType);
                    type = 20;  //D20,CAR,H2,MW->CA
                    isFormatRight = examCommonFormat(this.lblResidualE, inputData1, 0.5, 1.1, 80, 100, 10, 20, 0, 1500, -1.0, -1.0, -1.0, -1.0, i, out ca, type, enumType);
                    type = 21;  //D20,CAR,H2,MW->RNN
                    isFormatRight = examCommonFormat(this.lblResidualE, inputData1, 0.5, 1.1, 80, 100, 10, 20, 0, 1500, -1.0, -1.0, -1.0, -1.0, i, out rnn, type, enumType);
                    type = 22;  //D20,CAR,H2,MW->RAA
                    isFormatRight = examCommonFormat(this.lblResidualE, inputData1, 0.5, 1.1, 80, 100, 10, 20, 0, 1500, -1.0, -1.0, -1.0, -1.0, i, out raa, type, enumType);
                    type = 26;  //D20,CAR,H2,MW->RTT
                    isFormatRight = examCommonFormat(this.lblResidualE, inputData1, 0.5, 1.1, 80, 100, 10, 20, 0, 1500, -1.0, -1.0, -1.0, -1.0, i, out rtt, type, enumType);

                    if (isFormatRight == 0)
                    {
                        if (lblResidualE.Text.Length > 0) //错误提示信息，由于重复赋值2次，需减去1次
                        {
                            lblResidualE.Text = lblResidualE.Text.Substring(0, lblResidualE.Text.Length / 7);
                        }
                        //continue;
                    }
                    this.dgvResidual[i, 4].Value = ffa;   //给ffa单元格赋值
                    this.dgvResidual[i, 5].Value = cii;   //给cii单元格赋值
                    this.dgvResidual[i, 7].Value = tcc;   //给tcc单元格赋值
                    this.dgvResidual[i, 6].Value = ca;   //给ca单元格赋值
                    this.dgvResidual[i, 9].Value = rnn;   //给rnn单元格赋值
                    this.dgvResidual[i, 8].Value = raa;   //给raa单元格赋值
                    this.dgvResidual[i, 10].Value = rtt;   //给rtt单元格赋值
                    slResidual.Add(i + "|" + 4);
                    slResidual.Add(i + "|" + 5);
                    slResidual.Add(i + "|" + 7);
                    slResidual.Add(i + "|" + 6);
                    slResidual.Add(i + "|" + 9);
                    slResidual.Add(i + "|" + 8);
                    slResidual.Add(i + "|" + 10);
                    this.dgvResidual[i, 4].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.dgvResidual[i, 5].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.dgvResidual[i, 6].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.dgvResidual[i, 7].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.dgvResidual[i, 8].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.dgvResidual[i, 9].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    this.dgvResidual[i, 10].Style.ForeColor = System.Drawing.SystemColors.ControlText;

                }



                #endregion
            }
            #endregion

            tooltipEContent(lblResidualE);
        }

        //清除所有单元格
        private void tsmiClearResidual_Click(object sender, EventArgs e)
        {
            ResidualClear();
        }

        private void ResidualClear()
        {
            this.dgvResidual.Rows.Clear();
            this.dgvResidual.Columns.Clear();
            slResidual.Clear();
            this.lblResidualE.Text = "";
            BindDataResidual();
        }

        //选项事件
        private void tsmiChoResidual_Click(object sender, EventArgs e)
        {
            ResidualCho();
        }
        private void ResidualCho()
        {
            this.contextMenuStrip2.Show(this.menuStripResidual, new Point(this.tsmiChoResidual.Width + intervalWidth, this.tsmiChoResidual.Height));
        }

        private void lblResidualE_MouseHover(object sender, EventArgs e)
        {
            if (lblResidualE.Text.Length > 35)
            {
                this.lblResidualE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvResidual_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvResidual.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvResidual.CurrentCell = this.dgvResidual.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvResidual.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标题添加列表
        private void dgvResidual_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region 物性关联--粘度参数

        List<string> slVisPara = new List<string>();
        /// <summary>
        /// 物性关联--粘度参数
        /// </summary>
        private void BindDataVisPara()
        {
            this.lblVisParaE.Text = "";

            this.dgvVisPara.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "项目" });
            for (int i = 1; i < 21; i++)
            {
                this.dgvVisPara.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = i.ToString() });
            }
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVisPara, "20℃密度,g/cm3");
            this.dgvVisPara.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVisPara, "40℃粘度，mm2/s");
            this.dgvVisPara.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVisPara, "100℃粘度，mm2/s");
            this.dgvVisPara.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVisPara, "粘重常数（40℃");
            this.dgvVisPara.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVisPara, "粘重常数（100℃");
            this.dgvVisPara.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvVisPara, "粘度指数");
            this.dgvVisPara.Rows.Add(rowT);

            this.dgvVisPara.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvVisPara.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvVisPara.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvVisPara.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (c.Index == 0)
                {
                    c.Width = 60;
                    c.Frozen = true;
                }
                else
                    c.Width = commonWidth;
            }
        }

        //计算
        private void tsmiComVispara_Click(object sender, EventArgs e)
        {
            this.dgvVisPara.EndEdit();
            VisParaCom();
        }

        private void VisParaCom()
        {
            ClearDgv(this.dgvVisPara, slVisPara);

            this.lblVisParaE.Text = "";

            #region 判断输入格式 和计算
            for (int i = 1; i < this.dgvVisPara.Columns.Count; i++)
            {

                oilTypeDc.Clear();
                for (int j = 0; j < this.dgvVisPara.Rows.Count; j++)
                {
                    if (this.dgvVisPara[i, j].Value != null && this.dgvVisPara[i, j].Value.ToString() != "")
                    {
                        oilTypeDc.Add(((VisPara)j).ToString(), this.dgvVisPara[i, j].Value.ToString());
                        this.dgvVisPara[i, j].Style.ForeColor = BCellColor;
                    }
                }

                #region 开始计算
                List<KeyValuePair<string, string>> inputData1 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "V04" select p).ToList(); //D20,V04->VG4
                List<KeyValuePair<string, string>> inputData2 = (from p in oilTypeDc where p.Key == "D20" || p.Key == "V10" select p).ToList(); //D20,V10->V1G
                List<KeyValuePair<string, string>> inputData3 = (from p in oilTypeDc where p.Key == "V04" || p.Key == "V10" select p).ToList(); //V04,V10->VI
                int type = 0;  //0--  1--  2--  3
                Type enumType = typeof(VisPara);
                if (inputData1.Count() == 2) //D20,V04->VG4
                {
                    string vg4 = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 30;  //D20,V04->VG4
                    isFormatRight = examCommonFormat(this.lblVisParaE, inputData1, 0.5, 1.1, 0, 20000, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, i, out vg4, type, enumType);


                    if (isFormatRight == 0)
                    {
                        // continue;
                    }
                    else
                    {
                        this.dgvVisPara[i, 3].Value = vg4;   //给vg4单元格赋值
                        slVisPara.Add(i + "|" + 3);
                        this.dgvVisPara[i, 3].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData2.Count() == 2)//D20,V10->V1G
                {
                    string v1g = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 31;  //D20,V10->V1G
                    isFormatRight = examCommonFormat(this.lblVisParaE, inputData2, 0.5, 1.1, 0, 20000, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, i, out v1g, type, enumType);


                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (lblVisParaE.Text.Length > 0)
                            lblVisParaE.Text = subString(lblVisParaE.Text);
                    }
                    else
                    {
                        this.dgvVisPara[i, 4].Value = v1g;   //给vg4单元格赋值
                        slVisPara.Add(i + "|" + 4);
                        this.dgvVisPara[i, 4].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }

                if (inputData3.Count() == 2) //V04,V10->VI
                {
                    string vi = "";
                    int isFormatRight = 1;  //返回0--格式错误，跳出本次循环  返回1--格式正确
                    type = 32;   //V04,V10->VI
                    isFormatRight = examCommonFormat(this.lblVisParaE, inputData3, 0, 20000, 0, 20000, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, i, out vi, type, enumType);

                    if (isFormatRight == 0)
                    {
                        //continue;
                        if (lblVisParaE.Text.Length > 0)
                            lblVisParaE.Text = subString(lblVisParaE.Text);
                    }
                    else
                    {
                        this.dgvVisPara[i, 5].Value = vi;   //给vi单元格赋值
                        slVisPara.Add(i + "|" + 5);
                        this.dgvVisPara[i, 5].Style.ForeColor = System.Drawing.SystemColors.ControlText;
                    }
                }



                #endregion
            }
            #endregion

            tooltipEContent(lblVisParaE);
        }

        //清除所有单元格

        private void tsmiClearVispara_Click(object sender, EventArgs e)
        {
            VisParaClear();
        }

        private void VisParaClear()
        {
            this.dgvVisPara.Rows.Clear();
            this.dgvVisPara.Columns.Clear();
            slVisPara.Clear();
            this.lblVisParaE.Text = "";
            BindDataVisPara();
        }

        //选项事件   
        private void tsmiChoVispara_Click(object sender, EventArgs e)
        {
            VisParaCho();
        }
        private void VisParaCho()
        {
            this.contextMenuStrip2.Show(this.menuStripVispara, new Point(this.tsmiChoVispara.Width + intervalWidth, this.tsmiChoVispara.Height));
        }

        private void lblVisParaE_MouseHover(object sender, EventArgs e)
        {
            if (lblVisParaE.Text.Length > 35)
            {
                this.lblVisParaE.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }

        //单击编辑单元格
        private void dgvVisPara_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                this.dgvVisPara.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvVisPara.CurrentCell = this.dgvVisPara.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvVisPara.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //行标题添加列表
        private void dgvVisPara_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X + 2,
        e.RowBounds.Location.Y,
        this.dgvT.RowHeadersWidth - 4,
        e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvT.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvT.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #endregion

        #region 物性关联--格式判断

        #region 判断求Vapour闪点、蒸汽压的输入条件的格式
        private int examVapourFormat(Label lbl, List<KeyValuePair<string, string>> oit, double downlimit, double uplimit, int i, out string vapour, int type, Type enumType)
        {
            string _vapour = "";
            bool a = true;
            double smallValue = 0.0;
            int r = 0; //行号
            int tempr = 0;//存放临时行号
            foreach (var item in oit)
            {
                foreach (int s in Enum.GetValues(enumType))  //循环枚举类型求出行数
                {
                    string enumName = Enum.GetValues(enumType).GetValue(s).ToString();
                    if (enumName == item.Key)
                    { r = s + 1; }
                }

                a = Regex.IsMatch(item.Value.Trim(), pattern);
                if (a == false)
                {

                    this.lblVapourE.Text += "第" + i + "列" + r + "行非数字；";
                }
                else if (Convert.ToDouble(item.Value) < downlimit || Convert.ToDouble(item.Value) > uplimit)
                {
                    a = false;
                    this.lblVapourE.Text += "第" + i + "列" + r + "行超限；";
                }
                else
                {
                    if (type == 0 || type == 1)  //0<ECP-ICP<=50的判断→ ECP-ICP>0
                    {
                        if (smallValue == 0.0)
                        {
                            smallValue = Convert.ToDouble(item.Value);
                            tempr = r;
                        }
                        else
                        {
                            if (Convert.ToDouble(item.Value) - smallValue <= 0)
                            {
                                a = false;
                                //this.lblVapourE.Text += "第" + i + "列第" + r + "行与第" + tempr + "行相差不在范围之内；";
                                this.lblVapourE.Text += "第" + i + "列" + "ECP小于ICP";
                            }
                        }
                    }
                    else if (type == 2)  //A10<A30<A50<A70<A90的判断
                    {
                        if (smallValue == 0.0)
                        {
                            smallValue = Convert.ToDouble(item.Value);
                            tempr = r;
                        }
                        else if (smallValue > Convert.ToDouble(item.Value))
                        {
                            a = false;
                            this.lblVapourE.Text += "第" + i + "列" + tempr + "行" + i + "列" + r + "行数据不合理；";
                        }
                        else
                        {
                            smallValue = Convert.ToDouble(item.Value);
                            tempr = r;
                        }
                    }
                }
            }
            vapour = _vapour;
            if (a == false)
            {
                return 0;
            }
            else
            {
                switch (type)
                {
                    case 0:
                        //ICP,ECP->FPO 0 闪点、蒸气压
                        _vapour = BaseFunction.FunFPO(oit[0].Value);
                        break;
                    case 1:
                        //ICP, ECP->RVP 1  闪点、蒸气压
                        _vapour = BaseFunction.FunRVPfromMCP(((Convert.ToDouble(oit[0].Value) + Convert.ToDouble(oit[1].Value)) / 2).ToString());
                        break;
                    case 2:
                        //A10,A30->RVP  2  闪点、蒸气压
                        _vapour = BaseFunction.FunRVPfromMCP(((Convert.ToDouble(oit[0].Value) + Convert.ToDouble(oit[1].Value)) / 2).ToString());
                        break;
                    case 3:
                        //A10->FPO  闪点、蒸气压
                        _vapour = BaseFunction.FunFPO(oit[0].Value);
                        break;
                }
            }
            vapour = _vapour;
            return 1;
        }
        #endregion

        #region 判断求Freeze冰点、烟点、芳烃的输入条件的格式

        /// <summary>
        /// 冰点、分子量、碳氢比公用函数
        /// </summary>
        /// <param name="lbl"></param>
        /// <param name="oit"></param>
        /// <param name="downlimit1"></param>
        /// <param name="uplimit1"></param>
        /// <param name="downlimit2"></param>
        /// <param name="uplimit2"></param>
        /// <param name="i"></param>
        /// <param name="freeze"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int examFreezeFormat(Label lbl, List<KeyValuePair<string, string>> oit, double downlimit1, double uplimit1, double downlimit2, double uplimit2, int i, out string freeze, int type, Type enumType)
        {
            string _freeze = "";
            bool a = true;
            double smallValue = 0.0;
            int r = 0; //行号
            int tempr = 0;//存放临时行号
            int h = 0;  //判断是否有两个范围 0--一个范围  1--两个范围
            bool b = true;
            foreach (var item in oit)
            {
                foreach (int s in Enum.GetValues(enumType))  //循环枚举类型求出行数
                {
                    string enumName = Enum.GetValues(enumType).GetValue(s).ToString();
                    if (enumName == item.Key)
                    { r = s + 1; }
                }

                b = Regex.IsMatch(item.Value.Trim(), pattern);
                if (b == false)
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行非数字；";
                }
                else if (downlimit1 != 0.0 && uplimit1 != 0.0 && h == 0 && (Convert.ToDouble(item.Value) < downlimit1 || Convert.ToDouble(item.Value) > uplimit1))
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行超限；";

                    if (downlimit2 != 0.0 && uplimit2 != 0.0)
                    {
                        h++;
                    }
                }
                else if (downlimit2 != 0.0 && uplimit2 != 0.0 && h == 1 && (Convert.ToDouble(item.Value) < downlimit2 || Convert.ToDouble(item.Value) > uplimit2))
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行超限；";
                }
                else
                {
                    if (type == 1 || type == 2 || type == 3 || type == 15 || type == 10)  //0<ECP-ICP<=50的判断
                    {
                        if (smallValue == 0.0)
                        {
                            if (type != 10 && type != 15)
                            {
                                smallValue = Convert.ToDouble(item.Value);
                            }
                            else //type==15或10 数组中第一行为d20,不作为判断
                            {
                                if (r != 1)
                                {
                                    smallValue = Convert.ToDouble(item.Value);
                                    tempr = r;
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble(item.Value) - smallValue <= 0 )
                            {
                                a = false;
                                //lbl.Text += "第" + i + "列第" + r + "行与第" + tempr + "行相差不在范围之内；";
                                lbl.Text += "第" + i + "列" + "ECP必须大于ICP；";

                            }
                        }
                    }
                    else if (type == 27 || type == 28)  //特性因素ICP<ECP 去掉 D20,ICP,ECP->KFC 中的d20判断
                    {
                        if (smallValue == 0.0)
                        {
                            if (r != 1)
                            {
                                smallValue = Convert.ToDouble(item.Value);
                                tempr = r;
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble(item.Value) - smallValue <= 0)
                            {
                                a = false;
                                lbl.Text += "第" + i + "列第" + r + "行必须大于第" + tempr + "行；";
                            }
                        }
                    }
                    else if (type == 4 || type == 5 || type == 6 || type == 29 || type == 30 || type == 16 || type == 17 || type == 11 || type == 40 || type == 34)  //A10<A30<A50<A70<A90的判断
                    {
                        if (smallValue == 0.0)
                        {
                            if (type != 29 && type != 30 && type != 16 && type != 17 && type != 11 && type != 40 && type != 34)
                            {
                                smallValue = Convert.ToDouble(item.Value);
                                tempr = r;
                            }
                            else  //type==29或30或11或34 去掉第一行的D20，余下的进行大小比较判断
                            {
                                if (r != 1)
                                {
                                    smallValue = Convert.ToDouble(item.Value);
                                    tempr = r;
                                }
                            }
                        }
                        else if (smallValue >= Convert.ToDouble(item.Value))
                        {
                            a = false;
                            lbl.Text += "第" + i + "列第" + tempr + "行第" + i + "列" + r + "行数据不合理；";
                            break;  //如果数据不按照递增形式，直接跳出
                        }
                        else
                        {
                            smallValue = Convert.ToDouble(item.Value);
                            tempr = r;
                        }
                    }
                }

                if (h == 0)
                { h++; }
            }
            freeze = _freeze;
            if (a == false)
            {
                return 0;
            }
            else
            {
                switch (type)
                {
                    case 0:
                        //D20, ANI->SMK 0    冰点、烟点、芳烃
                        double sg = Convert.ToDouble(BaseFunction.FunSGfromD20(oit[0].Value));
                        _freeze = (-255.26 + 2.04 * Convert.ToDouble(oit[1].Value) - 240.8 * Math.Log(sg, Math.E) + 7727 * sg / Convert.ToDouble(oit[1].Value)).ToString();
                        break;
                    case 1:
                        //D20,ICP,ECP->SMK 1  冰点、烟点、芳烃
                        string api = BaseFunction.FunAPIfromD20(oit[0].Value);
                        _freeze = BaseFunction.FunSMKfromAPI_ICP_ECP_For_Tool(api, oit[1].Value, oit[2].Value);
                        break;
                    case 2:
                        //D20,ICP,ECP->FRZ 2  冰点、烟点、芳烃
                        _freeze = BaseFunction.FunFRZfromD20_ICP_ECP_For_Tool(oit[0].Value, oit[1].Value, oit[2].Value);
                        break;
                    case 3:
                        //D20, ICP, ECP->SAV, ARV 3  冰点、烟点、芳烃
                        if (BaseFunction.FunSAV_ARVfromD20_ICP_ECP_For_Tool(oit[0].Value, oit[1].Value, oit[2].Value).Count > 0)
                        {
                            _freeze = BaseFunction.FunSAV_ARVfromD20_ICP_ECP_For_Tool(oit[0].Value, oit[1].Value, oit[2].Value).ToList()[0].Value.ToString();
                        }
                        //else
                        //{
                        //    _freeze = "无结果";
                        //}
                        break;
                    case 4:
                        //D20,A10,A30,A50,A70,A90->SMK  冰点、烟点、芳烃
                        _freeze = BaseFunction.FunSMKfromD20_A10_A30_A50_A70_A90(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value);
                        break;
                    case 5:
                        //D20,A10,A30, A50, A70 ,A90->FRZ 5  冰点、烟点、芳烃
                        _freeze = BaseFunction.FunFRZfromD20_A10_A30_A50_A70_A90(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value);
                        break;
                    case 6:
                        //D20,A10,A30,A50,A70,A90->SAV , ARV  冰点、烟点、芳烃
                        if (BaseFunction.FunSAV_ARVfromD20_A10_A30_A50_A70_A90(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value).Count > 0)
                        {
                            _freeze = BaseFunction.FunSAV_ARVfromD20_A10_A30_A50_A70_A90(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value).ToList()[0].Value.ToString();
                        }
                        //else
                        //{
                        //    _freeze = "无结果";
                        //}
                        break;
                    case 7:
                        //D20,MCP->SMK   冰点、烟点、芳烃
                        string api1 = BaseFunction.FunAPIfromD20(oit[0].Value);
                        _freeze = BaseFunction.FunSMKfromAPI_MCP(api1, oit[1].Value);
                        break;
                    case 8:
                        //D20,MCP->FRZ 8  冰点、烟点、芳烃
                        _freeze = BaseFunction.FunFRZfromD20_MCP(oit[0].Value, oit[1].Value);
                        break;
                    case 9:
                        //D20,MCP->SAV, ARV 9  冰点、烟点、芳烃
                        if (BaseFunction.FunSAV_ARVfromD20_MCP(oit[0].Value, oit[1].Value).Count > 0)
                        {
                            _freeze = BaseFunction.FunSAV_ARVfromD20_MCP(oit[0].Value, oit[1].Value).ToList()[0].Value.ToString();
                        }
                        else
                        {
                            _freeze = "无结果";
                        }
                        break;
                    case 10:
                        //D20,ICP,ECP->ANI 10 苯胺点
                        _freeze = BaseFunction.FunANIfromD20_ICP_ECP(oit[0].Value, oit[1].Value, oit[2].Value);
                        break;
                    case 11:
                        //D20,A10,A30,A50,A70,A90->ANI 苯胺点
                        _freeze = BaseFunction.FunANIfromD20_A10_A30_A50_A70_A90( oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value,oit[0].Value);
                        break;
                    case 12:
                        //D20,MCP->ANI 12  苯胺点
                        _freeze = BaseFunction.FunANIfromD20_MCP(oit[0].Value, oit[1].Value);
                        break;
                    case 15:
                        //D20,ICP,ECP->CI  十六烷指数 
                        _freeze = BaseFunction.FunCIfromICPECP_D20(oit[1].Value, oit[2].Value, oit[0].Value);
                        break;
                    case 16:
                        //D20,A10,A30,A50,A70,A90->CI 十六烷指数
                        _freeze = BaseFunction.FunCIfromA10A30A50A70A90_D20(oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value, oit[0].Value);
                        break;
                    case 17:
                        //D20,A10,A30,A50,A90->CEN 十六烷指数
                        _freeze = BaseFunction.FunCENfromA10A30A50A90_D20(oit[1].Value, "", oit[2].Value, oit[3].Value, oit[0].Value);
                        break;
                    case 18:
                        //D20 ,MCP->CI 13  十六烷指数
                        _freeze = BaseFunction.FunCIfromMCP_D20(oit[1].Value, oit[0].Value);
                        break;
                    case 19:
                        //D20,ANI->DI 14  十六烷指数
                        string api2 = BaseFunction.FunAPIfromD20(oit[0].Value);
                        _freeze = BaseFunction.FunDI(api2, oit[1].Value);
                        break;
                    case 22:
                        _freeze = BaseFunction.FunACD(oit[0].Value, oit[1].Value);  //  NET, D20->ACD
                        break;
                    case 23:
                        _freeze = BaseFunction.FunNET(oit[1].Value, oit[0].Value);//  ACD, D20->NET
                        break;
                    case 26:
                        //V10,D20->KFC 26
                        _freeze = BaseFunction.FunKFCfromV10_D20(oit[1].Value, oit[0].Value);
                        break;
                    case 27:
                        //ICP,ECP, D20->KFC 27
                        _freeze = BaseFunction.FunKFCfromICPECP_D20(oit[1].Value, oit[2].Value, oit[0].Value);
                        break;
                    case 28:
                        //ICP,ECP, D20->BMI 28
                        _freeze = BaseFunction.FunBMIfromICPECP_D20(oit[1].Value, oit[2].Value, oit[0].Value);
                        break;
                    case 29:
                        //A10,A30,A50,A70,A90, D20->KFC
                        _freeze = BaseFunction.FunKFCfromA10A30A50A70A90_D20(oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value, oit[0].Value);
                        break;
                    case 30:
                        //A10,A30,A50,A70,A90,D20->BMI 
                        _freeze = BaseFunction.FunBMIfromA10A30A50A70A90_D20(oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value, oit[0].Value);
                        break;
                    case 31:
                        //MCP, D20->KFC 31
                        _freeze = BaseFunction.FunKFCfromMCP_D20(oit[1].Value, oit[0].Value);
                        break;
                    case 32:
                        //MCP,D20->BMI 32
                        _freeze = BaseFunction.FunBMIfromMCP_D20(oit[1].Value, oit[0].Value);
                        break;
                    case 33:
                        //D20, ICP,ECP->MW 33  分子量
                        _freeze = BaseFunction.FunMWfromICP_ECP_D20(oit[1].Value, oit[2].Value, oit[0].Value);
                        break;
                    case 34:
                        //D20,A10,A30,A50,A70,A90->MW 34  分子量
                        _freeze = BaseFunction.FunMWfromA10_A30_A50_A70_A90_D20(oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value, oit[0].Value);
                        break;
                    case 35:
                        //D20,MCP->MW 35  分子量
                        //_freeze = BaseFunction.FunMWfromD20_V04_V10(oit[0].Value, oit[1].Value);
                        _freeze = BaseFunction.FunMWfromMCP_D20(oit[1].Value, oit[0].Value);
                        break;
                    case 36:
                        //D20,V04,V10->MW 36  分子量
                        _freeze = BaseFunction.FunMWfromD20_V04_V10(oit[0].Value, oit[1].Value, oit[2].Value);
                        break;
                    case 37:
                        //D20,V08,V10->MW 37  分子量
                        _freeze = BaseFunction.FunMWfromD20_V08_V10(oit[0].Value, oit[1].Value, oit[2].Value);
                        break;
                    case 40:
                        //D20,A10,A30,A50,A70,A90->C/H
                        _freeze = BaseFunction.FunC1HfromD20_A10_A30_A50_A70_A90(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value, oit[4].Value, oit[5].Value);
                        break;
                    case 41:
                        //D20,MCP->C/H 41  碳氢比
                        _freeze = BaseFunction.FunC1HfromD20_MCP(oit[0].Value, oit[1].Value);
                        break;
                    case 42:
                        //H/C->C/H  42  碳氢比
                        _freeze = BaseFunction.FunC1HfromH1C(oit[0].Value);
                        break;
                    case 43:
                        //C/H->H/C  43  碳氢比
                        _freeze = BaseFunction.FunH1CfromC1H(oit[0].Value);
                        break;
                    case 44:
                        //C/H, SUL->H2 44;   碳氢比
                        // _freeze = (100 - Convert.ToDouble(oit[1].Value) / (1 + Convert.ToDouble(oit[0].Value))).ToString();
                        _freeze = BaseFunction.FunH2fromC1H_SUL(oit[1].Value, oit[0].Value);
                        break;
                    case 45:
                        // H2,SUL->C/H  碳氢比
                        // _freeze = ((100 - Convert.ToDouble(oit[1].Value)) / Convert.ToDouble(oit[0].Value) - 1).ToString();
                        _freeze = BaseFunction.FunC1HfromH2_SUL(oit[1].Value, oit[0].Value);
                        break;
                    case 46:
                        //V05,D20->KFC 46
                        _freeze = BaseFunction.FunKFCfromV05_D20(oit[1].Value, oit[0].Value);
                        break;
                }
            }
            freeze = _freeze;
            return 1;
        }

        private int examFreezeFormat(Label lbl, List<KeyValuePair<string, string>> oit, double downlimit1, double uplimit1, double downlimit2, double uplimit2, double downlimit3, double uplimit3, int i, out string freeze, int type, Type enumType)
        {
            string _freeze = "";
            bool a = true;
            double smallValue = 0.0;
            int r = 0; //行号
            int tempr = 0;//存放临时行号
            int h = 0;  //判断是否有两个范围 0--一个范围  1--两个范围
            bool flag = false; //
            foreach (var item in oit)
            {
                foreach (int s in Enum.GetValues(enumType))  //循环枚举类型求出行数
                {
                    string enumName = Enum.GetValues(enumType).GetValue(s).ToString();
                    if (enumName == item.Key)
                    { r = s + 1; }
                }

                a = Regex.IsMatch(item.Value.Trim(), pattern);
                if (a == false)
                {

                    lbl.Text += "第" + i + "列" + r + "行非数字；";
                }
                else if (h == 0 && (Convert.ToDouble(item.Value) < downlimit1 || Convert.ToDouble(item.Value) > uplimit1))
                {
                    a = false;
                    lbl.Text += "第" + i + "列" + r + "行超限；";

                    if (downlimit2 != 0.0 && uplimit2 != 0.0)
                    {
                        h++;
                        flag = true;
                    }
                }
                else if (h == 1 && (Convert.ToDouble(item.Value) < downlimit2 || Convert.ToDouble(item.Value) > uplimit2))
                {
                    a = false;
                    lbl.Text += "第" + i + "列" + r + "行超限；";
                    if (downlimit3 != -1.0 && uplimit3 != -1.0)
                    {
                        h++;
                        flag = true;
                    }
                }
                else if (h == 2 && (Convert.ToDouble(item.Value) < downlimit3))
                {
                    if (type != 2)           // ECP大于0的范围判断
                    {
                        a = false;
                        lbl.Text += "第" + i + "列" + r + "行超限；";
                    }
                    else  //type==2  ECP大于等于1000的范围判断
                    {
                        if (Convert.ToDouble(item.Value) < 1000)
                        {
                            a = false;
                            lbl.Text += "第" + i + "列第" + r + "行超限；";
                        }
                    }
                    flag = true;
                }
                else
                {
                    if (type == 1 || type == 2 || type == 3 || type == 10 || type == 38)  //0<ECP-ICP<=50的判断(2012.11.23屏蔽)
                    {
                        if (smallValue == 0.0)
                        {
                            if (type != 38)
                            {
                                smallValue = Convert.ToDouble(item.Value);
                            }
                            else
                            {
                                if (r != 1)
                                    smallValue = Convert.ToDouble(item.Value);
                            }
                        }
                        else
                        {
                            if (Convert.ToDouble(item.Value) - smallValue <= 0)//ECP必须大于ICP
                            {
                                a = false;
                                //lbl.Text += "第" + i + "列第" + (r - 1) + "行与第" + r + "行相差不在范围之内";
                                lbl.Text += "第" + i + "列" + "ECP必须大于ICP；";

                            }
                        }
                    }
                    else if (type == 4 || type == 5 || type == 6)  //A10<A30<A50<A70<A90的判断
                    {
                        if (smallValue == 0.0)
                        {
                            smallValue = Convert.ToDouble(item.Value);
                            tempr = r;
                        }
                        else if (smallValue > Convert.ToDouble(item.Value))
                        {
                            a = false;
                            lbl.Text += "第" + i + "列" + tempr + "行" + i + "列" + r + "行数据不合理";
                            break;  //如果数据不按照递增形式，直接跳出
                        }
                        else
                        {
                            smallValue = Convert.ToDouble(item.Value);
                            tempr = r;
                        }
                    }
                }

                if (flag == false)
                { 
                    h++; 
                }
                else
                {
                    freeze = _freeze;
                    return 0;
                }
                flag = false;
            }
            freeze = _freeze;
            if (a == false)
            {
                return 0;
            }
            else
            {
                switch (type)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 38:
                        //D20, ICP,ECP->C/H 38 碳氢比
                        string sg = BaseFunction.FunSGfromD20(oit[0].Value);
                        string mcp = ((Convert.ToDouble(oit[1].Value) + Convert.ToDouble(oit[2].Value)) / 2).ToString();
                        _freeze = BaseFunction.FunC1HfromSG_MCP(sg, mcp);
                        break;
                    case 39:
                        //IF ECP>=1000 THEN D20->C/H 39  碳氢比
                        if (Convert.ToDouble(oit[1].Value) >= 1000)
                        {
                            string sg1 = BaseFunction.FunSGfromD20(oit[0].Value);
                            _freeze = BaseFunction.FunC1HfromSG(sg1);
                        }
                        else
                        {
                            return 0;
                        }
                        break;
                }
            }
            freeze = _freeze;
            return 1;
        }
        #endregion

        #region 判断求芳烃指数、芳烃潜含量的输入条件的格式
        private int examBMCIFormat(Label lbl, List<KeyValuePair<string, string>> oit, double downlimit1, double uplimit1, int i, out string bmci, int type, Type enumType)
        {
            string _bmci = "";
            bool a = true;
            double smallValue = 0.0;
            int r = 0; //行号
            int tempr = 0;//存放临时行号
            foreach (var item in oit)
            {
                foreach (int s in Enum.GetValues(enumType))  //循环枚举类型求出行数
                {
                    string enumName = Enum.GetValues(enumType).GetValue(s).ToString();
                    if (enumName == item.Key)
                    { r = s + 1; }
                }

                a = Regex.IsMatch(item.Value.Trim(), pattern);
                if (a == false)
                {

                    lbl.Text += i + "列" + r + "行非数字；";
                }
                else if ((Convert.ToDouble(item.Value) <= downlimit1 || Convert.ToDouble(item.Value) >= uplimit1))
                {
                    a = false;
                    lbl.Text += i + "列" + r + "行超限；";
                }
                else
                {
                    if (type == 1)  //NAH+RAM<100的判断
                    {
                        if (smallValue == 0.0)
                        {
                            smallValue = Convert.ToDouble(item.Value);
                        }
                        else
                        {
                            if (Convert.ToDouble(item.Value) + smallValue >= 100)
                            {
                                a = false;
                                lbl.Text += "NAH、RAM相加不在范围之内；";
                            }
                        }
                    }
                    else if (type == 2)  //N06+A06+N07+A07+N08+A08<100的判断
                    {
                        smallValue += Convert.ToDouble(item.Value);
                        tempr++;
                        if (tempr == 6 && smallValue >= 100)
                        {
                            a = false;
                            lbl.Text += "N06,A06,N07,A07,N08,A08相加不在范围之内；";
                        }
                    }
                }
            }
            bmci = _bmci;
            if (a == false)
            {
                return 0;
            }
            else
            {
                switch (type)
                {
                    case 1:
                        //NAH,ARM->N2A 1
                        _bmci = BaseFunction.FunN2A(oit[0].Value, oit[1].Value);
                        break;
                    case 2:
                        //N06,A06,N07,A07,N08,A08->ARP
                        _bmci = BaseFunction.FunARP(oit[0].Value, oit[2].Value, oit[4].Value, oit[1].Value, oit[3].Value, oit[5].Value);
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                }
            }
            bmci = _bmci;
            return 1;
        }
        #endregion

        #region 判断求四组分\蜡油结构组成\渣油结构组成\粘度参数 的输入条件的格式

        private int examCommonFormat(Label lbl, List<KeyValuePair<string, string>> oit, double downlimit1, double uplimit1, double downlimit2, double uplimit2, double downlimit3, double uplimit3, double downlimit4, double uplimit4, double downlimit5, double uplimit5, double downlimit6, double uplimit6, int i, out string output, int type, Type enumType)
        {
            string _output = "";
            bool a = true;
            bool b = true;
            double smallValue = 0.0;
            int r = 0; //行号
            int tempr = 0;//存放临时行号
            int h = 0;  //判断是否有两个范围 0--一个范围  1--两个范围
            bool flag = false;
            foreach (var item in oit)
            {
                foreach (int s in Enum.GetValues(enumType))  //循环枚举类型求出行数
                {
                    string enumName = Enum.GetValues(enumType).GetValue(s).ToString();
                    if (enumName == item.Key)
                    { r = s + 1; }
                }
                b = Regex.IsMatch(item.Value.Trim(), pattern);
                if (b == false)
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行非数字；";
                }
                else if (h == 0 && (Convert.ToDouble(item.Value) < downlimit1 || Convert.ToDouble(item.Value) > uplimit1))
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行超限；";

                    if (downlimit2 != -1.0 && uplimit2 != -1.0)
                    {
                        h++;
                        flag = true;
                    }
                }
                else if (h == 1 && item.Value != string.Empty && (Convert.ToDouble(item.Value) < downlimit2 || Convert.ToDouble(item.Value) > uplimit2))//ECP为空时不用比较
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行超限；";
                    if (downlimit3 != -1.0 && uplimit3 != -1.0)
                    {
                        h++;
                        flag = true;
                    }
                }
                else if (h == 2 && (Convert.ToDouble(item.Value) < downlimit3 || Convert.ToDouble(item.Value) > uplimit3))
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行超限；";
                    if (downlimit4 != -1.0 && uplimit4 != -1.0)
                    {
                        h++;
                        flag = true;
                    }
                }
                else if (h == 3 && (Convert.ToDouble(item.Value) < downlimit4 || Convert.ToDouble(item.Value) > uplimit4))
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行超限；";
                    if (downlimit5 != -1.0 && uplimit5 != -1.0)
                    {
                        h++;
                        flag = true;
                    }
                }
                else if (h == 4 && (Convert.ToDouble(item.Value) < downlimit5 || Convert.ToDouble(item.Value) > uplimit5))
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行超限；";
                    if (downlimit6 != -1.0 && uplimit6 != -1.0)
                    {
                        h++;
                        flag = true;
                    }
                }
                else if (h == 5 && (Convert.ToDouble(item.Value) < downlimit6 || Convert.ToDouble(item.Value) > uplimit6))
                {
                    a = false;
                    lbl.Text += "第" + i + "列第" + r + "行超限；";
                    flag = true;
                }
                else
                {
                    if (type == 23 || type == 24 || type == 25)
                    {
                        List<double> tvList = new List<double>();
                        tvList.Add(Convert.ToDouble(item.Value));
                        if (tvList.Count == 4)
                        {
                            if (tvList[0] < tvList[2] && tvList[1] > tvList[3])
                            {
                                a = false;
                                lbl.Text += "T1,T2,T3,V1,V2,V3数据不合理";
                            }
                        }
                    }
                    else if (type == 32)
                    {
                        if (smallValue == 0.0)
                        {
                            smallValue = Convert.ToDouble(item.Value);
                        }
                        else
                        {
                            if (smallValue < Convert.ToDouble(oit[1].Value))
                            {
                                a = false;
                                lbl.Text += "第" + i + "列第2行必须大于第3行";
                            }
                        }
                    }
                }

                if (flag == false)
                { h++; }
                flag = false;
            }
            output = _output;
            if (a == false)
            {
                return 0;
            }
            else
            {
                switch (type)
                {
                    case 1:
                        //ICP,ECP,D20,V10,CCR,SUL->SAH 四组分
                        if (BaseFunction.FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(null, oit[0].Value, oit[1].Value, oit[2].Value, oit[5].Value, oit[3].Value, oit[4].Value).Count > 0)
                            _output = (BaseFunction.FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(null, oit[0].Value, oit[1].Value, oit[2].Value, oit[5].Value, oit[3].Value, oit[4].Value)).ToList()[0].Value.ToString();
                        break;
                    case 2:
                        //ICP,ECP,D20,V10,CCR,SUL->ARS 四组分                    
                        if (BaseFunction.FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(null, oit[0].Value, oit[1].Value, oit[2].Value, oit[5].Value, oit[3].Value, oit[4].Value).Count > 0)
                            _output = (BaseFunction.FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(null, oit[0].Value, oit[1].Value, oit[2].Value, oit[5].Value, oit[3].Value, oit[4].Value)).ToList()[1].Value.ToString();
                        break;
                    case 3:
                        //ICP,ECP,D20,V10,CCR,SUL->RES  四组分
                        if (BaseFunction.FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(null, oit[0].Value, oit[1].Value, oit[2].Value, oit[5].Value, oit[3].Value, oit[4].Value).Count > 0)
                            _output = (BaseFunction.FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(null, oit[0].Value, oit[1].Value, oit[2].Value, oit[5].Value, oit[3].Value, oit[4].Value)).ToList()[2].Value.ToString();
                        break;
                    case 4:
                        //ICP,ECP,D20,V10,CCR,SUL->APH  四组分
                        if (BaseFunction.FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(null, oit[0].Value, oit[1].Value, oit[2].Value, oit[5].Value, oit[3].Value, oit[4].Value).Count > 0)
                            _output = (BaseFunction.FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(null, oit[0].Value, oit[1].Value, oit[2].Value, oit[5].Value, oit[3].Value, oit[4].Value)).ToList()[3].Value.ToString();
                        break;
                    case 5:
                        // D20,R20,MW,SUL->CPP  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[0].Value.ToString();
                        break;
                    case 6:
                        //D20,R20,MW,SUL->CNN  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[1].Value.ToString();
                        break;
                    case 7:
                        //D20,R20,MW,SUL->CAA 蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[2].Value.ToString();
                        break;
                    case 8:
                        //D20,R20,MW,SUL->RTT 蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[3].Value.ToString();
                        break;
                    case 9:
                        //D20,R20,MW,SUL->RNN  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[4].Value.ToString();
                        break;
                    case 10:
                        //D20,R20,MW,SUL->RAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[5].Value.ToString();
                        break;
                    case 11:
                        //D70,R70,MW,SUL->CPP 蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[0].Value.ToString();
                        break;
                    case 12:
                        //D70,R70,MW,SUL->CNN  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[1].Value.ToString();
                        break;
                    case 13:
                        //D70,R70,MW,SUL->CAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[2].Value.ToString();
                        break;
                    case 14:
                        //D70,R70,MW,SUL->RTT  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[3].Value.ToString();
                        break;
                    case 15:
                        //D70,R70,MW,SUL->RNN  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[4].Value.ToString();
                        break;
                    case 16:
                        //D70,R70,MW,SUL->RAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[5].Value.ToString();
                        break;
                    case 33:
                        //D20,R70,MW,SUL->RAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[0].Value.ToString();
                        break;
                    case 34:
                        //D20,R70,MW,SUL->RAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[1].Value.ToString();
                        break;
                    case 35:
                        //D20,R70,MW,SUL->RAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[2].Value.ToString();
                        break;
                    case 36:
                        //D20,R70,MW,SUL->RAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[3].Value.ToString();
                        break;
                    case 37:
                        //D20,R70,MW,SUL->RAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[4].Value.ToString();
                        break;
                    case 38:
                        //D20,R70,MW,SUL->RAA  蜡油结构组成
                        if (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value).Count > 0)
                            _output = (BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(oit[0].Value, oit[1].Value, oit[2].Value, oit[3].Value)).ToList()[5].Value.ToString();
                        break;
                    case 17:
                        //D20,CAR,H2,MW->FFA 渣油结构组成
                        if (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value).Count > 0)
                            _output = (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value)).ToList()[0].Value.ToString();
                        break;
                    case 18:
                        //D20,CAR,H2,MW->CII  渣油结构组成
                        if (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value).Count > 0)
                            _output = (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value)).ToList()[1].Value.ToString();
                        break;
                    case 19:
                        //D20,CAR,H2,MW->TCC  渣油结构组成
                        if (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value).Count > 0)
                            _output = (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value)).ToList()[2].Value.ToString();
                        break;
                    case 20:
                        //D20,CAR,H2,MW->CA  渣油结构组成
                        if (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value).Count > 0)
                            _output = (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value)).ToList()[3].Value.ToString();
                        break;
                    case 21:
                        //D20,CAR,H2,MW->RNN  渣油结构组成
                        if (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value).Count > 0)
                            _output = (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value)).ToList()[4].Value.ToString();
                        break;
                    case 22:
                        //D20,CAR,H2,MW->RAA  渣油结构组成
                        if (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value).Count > 0)
                            _output = (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value)).ToList()[5].Value.ToString();
                        break;
                    case 26:
                        //D20,CAR,H2,MW->RTT  渣油结构组成
                        if (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value).Count > 0)
                            _output = (BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(oit[0].Value, oit[3].Value, oit[1].Value, oit[2].Value)).ToList()[6].Value.ToString();
                        break;
                    case 23:
                        //T2,V2, T3, V3, T1->V1 三点粘度
                        _output = BaseFunction.FunV(oit[1].Value, oit[3].Value, oit[0].Value, oit[2].Value, oit[4].Value);
                        break;
                    case 24:
                        //T1,V1, T3, V3, T2->V2  三点粘度
                        _output = BaseFunction.FunV(oit[1].Value, oit[3].Value, oit[0].Value, oit[2].Value, oit[4].Value);
                        break;
                    case 25:
                        //T1,V1, T2, V2, T3->V3  三点粘度
                        _output = BaseFunction.FunV(oit[1].Value, oit[3].Value, oit[0].Value, oit[2].Value, oit[4].Value);
                        break;
                    case 30:
                        //D20,V04->VG4 30 粘度参数
                        string d15 = BaseFunction.FunD15fromD20(oit[0].Value);
                        _output = BaseFunction.FunVG4fromD15andV04(d15, oit[1].Value);
                        break;
                    case 31:
                        //D20,V10->V1G 31  粘度参数
                        string d151 = BaseFunction.FunD15fromD20(oit[0].Value);
                        _output = BaseFunction.FunV1GfromD15andV10(d151, oit[1].Value);
                        break;
                    case 32:
                        //V04,V10->VI  32 粘度参数
                        _output = BaseFunction.FunVIfromV04_V10(oit[0].Value, oit[1].Value);
                        break;
                }
            }
            output = _output;
            return 1;
        }

        #endregion

        #endregion

        #region 错误信息内容悬浮
        private void tooltipEContent(Label lbl)
        {
            if (lbl.Text.Length > 34)
            {
                string lblContent = lbl.Text.ToString();
                string tempContent = "";
                while (lblContent.Length >= 40)
                {
                    tempContent += lblContent.Substring(0, 40) + "\r\n";
                    lblContent = lblContent.Remove(0, 40);
                }
                if (lblContent.Length > 0)
                {
                    tempContent += lblContent;
                }
                this.toolTip1.Show(tempContent, lbl);
                lbl.Text = lbl.Text.Substring(0, 34) + "....";
                this.toolTip1.Active = false;
            }
            else
            {
                lbl.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 小数有效位数截取
        private string subDouble(string strData)
        {
            string _strData = strData;
            if (strData.Contains('E')) //double型的字符串以指数形式表示，则截取小数后6位
            {
                _strData = Convert.ToDouble(strData).ToString("F6"); //小数点后保留6位
            }
            return _strData;
        }
        #endregion

        #region 求二次方程 D20= -0.0137*D15^2 + 1.0277*D15 - 0.0173
        /// <summary>
        /// 求D15
        /// </summary>
        /// <param name="d20"></param>
        /// <returns></returns>
        private string FunD15fromD20(string d20)
        {
            double _d20 = Convert.ToDouble(d20);
            double result = 0.0;
            double d = 1.0277 * 1.0277 - 4 * -0.0137 * -0.0173;
            double x1, x2;
            if (d > 0)
            {
                x1 = (-1.0277 - Math.Sqrt(d)) / 2.0 / -0.0137;
                x2 = (-1.0277 + Math.Sqrt(d)) / 2.0 / -0.0137;
                if (x1 > 0.5 && x1 < 1.1)
                { result = -_d20 + x1; }
                if (x2 > 0.5 && x2 < 1.1)
                { result = -_d20 + x2; }
            }
            else if (d == 0)
            {
                x1 = x2 = (-1.0277) / 2.0 / -0.0137;
                if (x1 > 0.5 && x1 < 1.1)
                { result = -_d20 + x1; }
            }
            else
            {
                double i = Math.Sqrt(-d) / 2.0 / -0.0137;
                x1 = x2 = -1.0277 / 2.0 / -0.0137;
                if (x1 > 0.5 && x1 < 1.1)
                { result = -_d20 + x1; }
            }
            return result.ToString();
        }
        #endregion

        #region 字符串截取
        private string subString(string lblString)
        {
            string[] array = lblString.Split('；');
            string tempString = "";
            var item = array.Distinct();

            foreach (string d in item)
            {
                if (d != "")
                    tempString += d + "；";
            }
            return tempString;
        }
        #endregion

        #region 帮助
        private void tsmiHelp_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;
            string tsmiHelpName = tsmi.Name;

            switch (tsmiHelpName)
            {
                case "tsmiHelpT"://温度
                    formHelpShow(Resources.T, "温度");
                    break;

                case "tsmiHelpPressure"://压力
                    formHelpShow(Resources.Pressure, "压力");
                    break;

                case "tsmiHelpDensity"://浓度
                    formHelpShow(Resources.Density, "浓度");
                    break;

                case "tsmiHelpQ"://质量
                    formHelpShow(Resources.Q, "质量");
                    break;

                case "热量"://热量
                    //formHelpShow(Resources.ceshi, "热量");
                    break;

                case "tsmiHelpMiDu"://密度
                    formHelpShow(Resources.MiDu, "密度");
                    break;

                case "tsmiHelpAcid"://酸度
                    formHelpShow(Resources.Acid, "酸度");
                    break;

                case "tsmiHelpViscosity"://粘度
                    formHelpShow(Resources.Viscosity, "粘度");
                    break;

                case "tsmiHelpProperty"://特性因数
                    formHelpShow(Resources.Property, "特性因数");
                    break;

                case "tsmiHelpOilType"://原油类型s
                    formHelpShow(Resources.OilType, "原油类型");
                    break;

                case "tsmiHelpDistill"://馏程转换

                    formHelpShow(Resources.slDistill, "馏程转换");
                    break;

                case "tsmiHelpVapour"://闪点、蒸气压
                    formHelpShow(Resources.Vapour, "闪点、蒸气压");
                    break;

                case "tsmiHelpCetane"://十六烷指数
                    formHelpShow(Resources.Cetane, "十六烷指数");
                    break;

                case "tsmiHelpFreeze"://冰点 烟点 芳烃
                    formHelpShow(Resources.Freez, "冰点 烟点 芳烃");
                    break;

                case "tsmiHelpAniline"://苯胺点
                    formHelpShow(Resources.Aniline, "苯胺点");
                    break;

                case "tsmiHelpMol"://分子量
                    formHelpShow(Resources.Mol, "分子量");
                    break;

                case "tsmiHelpCH"://碳氢比
                    formHelpShow(Resources.CH, "碳氢比");
                    break;

                case "tsmiHelpBMCI"://芳烃指数 芳烃潜含量
                    formHelpShow(Resources.BMCI, "芳烃指数 芳烃潜含量");
                    break;

                case "tsmiHelpFour"://四组分
                    formHelpShow(Resources.Four, "四组分");
                    break;

                case "tsmiHelpWax"://蜡油结构组成
                    formHelpShow(Resources.Wax, "蜡油结构组成");
                    break;

                case "tsmiHelpResidual"://渣油结构组成
                    formHelpShow(Resources.Residual, "渣油结构组成");
                    break;

                case "tsmiHelpVispara"://粘度参数
                    formHelpShow(Resources.VisPara, "粘度参数");
                    break;

            }
        }

        /// <summary>
        /// 显示帮助窗体
        /// </summary>
        /// <param name="image"></param>
        private void formHelpShow(Image image, string text)
        {
            Form formHelp = new Form();
            Panel panle = new Panel();
            formHelp.Controls.Add(panle);
            panle.AutoScroll = true;
            panle.AutoSize = true;
            panle.BackgroundImage = image;
            panle.Size = image.Size;

            formHelp.AutoScroll = true;
            formHelp.Text = "图示-" + text;
            //formHelp.BackgroundImage = image;

            Label lbl=new Label();
            formHelp.Controls.Add(lbl);
            lbl.Text = "帮助说明:\r\n" + "1. 蓝色字体数据为用户需要输入的数据，黑色为计算数据;\r\n" + "2. 对应数据需要在输入范围内!";
            lbl.Width = image.Width;
            lbl.Height = 75;
            lbl.BackColor = Color.White;
            lbl.ForeColor = Color.Maroon;
            lbl.Font = new System.Drawing.Font("SimSun", 10);
            lbl.Dock = DockStyle.Bottom;

            formHelp.Height = image.Height + lbl.Height + 40;
            formHelp.Width = image.Width + 20;
            formHelp.FormBorderStyle = FormBorderStyle.FixedSingle;
            formHelp.Show();
        }

        #endregion

        private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //DataGridView dgv = (DataGridView)sender;
            //dgv[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Blue;
        }

        private void dgvViscosity_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.dgvViscosity.ClearSelection();
        }

        private void dgvMixViscosity_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.dgvMixViscosity.ClearSelection();
        }
    }
}
