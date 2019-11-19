using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using RIPP.OilDB.Data;

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FrmPreCalculation : Form
    {
        #region 基础变量
        private int commonWidth = 170;
        private int commonHeight = 26;
        protected Color RowTitleColor = Color.SaddleBrown;
        protected Color BCellColor = Color.RoyalBlue;
       // protected string pattern = @"^[+-]?(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$";
        protected string pattern = @"^[+-]?((\.[0-9]*[1-9][0-9]*)|([0-9]+\.[0-9]*[0-9][0-9]*)|([0-9]*[0-9][0-9]*\.[0-9]+)|([0-9]*[0-9][0-9]*))$";
        private int stepNum=0;  //用来判断当前在第几步骤
        Dictionary<string, double> step1Dic = new Dictionary<string, double>(); //存放步骤1表中内容
        List<double> ctList = new List<double>();   //存放终切点，℃列
        List<double> cwList = new List<double>();   //存放样品量，g列
        List<double> wyList = new List<double>();     //存放收率，w%列
        List<double> twyList = new List<double>();    //存放累计收率，w%列
        List<double> icpList = new List<double>();   //存放初馏点，℃
        List<double> ecpList = new List<double>();   //终馏点，℃列
        List<double> wwList = new List<double>();   //配样量，g列
        Dictionary<int, Dictionary<int, double>> ccwDic = new Dictionary<int, Dictionary<int, double>>();  //存放中间变量
        bool isDealData = false; //第二步是否通过数据处理
        bool isError = false; //判断数据是否有错误
        bool isShowMess = true; //是否弹出提示对话框
        Size s = new Size(730, 315);
        #endregion

        public FrmPreCalculation()
        {
            InitializeComponent();
        }

        private void FrmPreCalculation_Load(object sender, EventArgs e)
        {
            BindDataStep1();
            BindDataStep2();
            BindDataStep3();
            DataBindDisplay();

            this.dgvStep1.Visible = true;
            this.dgvStep2.Visible = false;
            this.dgvStep3.Visible = false;
            this.dgvDisplay.Visible = false;

            this.tsbDataDeal.Visible = false;
            this.tsbComputer.Visible = false;
            this.tsbLastStep.Visible = false;

            //this.tsblabLimit.Visible = false;
            //this.tsblabOilName.Visible = false;
            //this.tsbtxtLimit.Visible = false;
            //this.tsbtxtOilName.Visible = false;

            this.toolStrip2.Visible = false;
            this.tsbExcel.Visible = false;

            
        }


        #region step1
        private void BindDataStep1()
        { 
            this.dgvStep1.Size = s;
            this.lblStep1E.Text = "";

            this.dgvStep1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "标题" });

            this.dgvStep1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "内容" });
            DataGridViewRow rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvStep1, "常压釜装油量，g");
            this.dgvStep1.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvStep1, "常压釜残油+柱残油量，g");
            this.dgvStep1.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvStep1, "减压釜装油量，g");
            this.dgvStep1.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvStep1, "减压釜残油量，g");
            this.dgvStep1.Rows.Add(rowT);
            rowT = new DataGridViewRow();
            rowT.CreateCells(this.dgvStep1, "蒸馏切换点，℃");
            this.dgvStep1.Rows.Add(rowT);

            this.dgvStep1.Columns[0].DefaultCellStyle.ForeColor = RowTitleColor;
            this.dgvStep1.Columns[0].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvStep1.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvStep1.Columns)
            {
                if (c.Index == 0)
                {
                    c.Width = 150;
                }
                else
                    c.Width = 235;
            }

            sortUnable(this.dgvStep1);//列标题禁用排序
        }

        private void dgvSetp1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
          e.RowBounds.Location.Y,
          this.dgvStep1.RowHeadersWidth - 4,
          e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvStep1.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvStep1.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void dgvSetp1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0&&e.ColumnIndex!=-1&&e.RowIndex!=-1)
            {
                this.dgvStep1.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvStep1.CurrentCell = this.dgvStep1.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvStep1.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        #region 步骤1数据格式处理
        /// <summary>
        /// 步骤1数据格式处理
        /// </summary>
        private void step1DealFormat()
        {
            step1Dic.Clear();
            this.lblStep1E.Text = "";
            double tempData = 9999999999;

            int rCount = 0;//记录第3、4、5行有值的个数

            for (int c = 1; c < dgvStep1.Columns.Count; c++)
            {
                for (int r = 0; r < dgvStep1.Rows.Count; r++)
                {
                    string crValue = dgvStep1[c, r].Value == null ? string.Empty : dgvStep1[c, r].Value.ToString().Trim();//第C列第R行的值

                    if (crValue != "")
                    {

                        if (r == 2 || r == 3 || r == 4)
                        {
                            rCount++;
                        }

                        bool a = Regex.IsMatch(dgvStep1[c, r].Value.ToString().Trim(), pattern);
                        if (a == false)  //如果非数字
                        {

                            this.lblStep1E.Text += "第" + (c+1) + "列第" + (r + 1) + "行非数字；";
                        }
                        else if ((r != 3 && r != 4) && Convert.ToDouble(dgvStep1[c, r].Value) <= 0)
                        {
                            this.lblStep1E.Text += "第" + (c + 1) + "列第" + (r + 1) + "行超限；";
                        }
                        else if (r == 3 && dgvStep1[c, r - 1].Value != null && dgvStep1[c, r - 1].Value.ToString() != "" && Convert.ToDouble(dgvStep1[c, r].Value) <= 0)
                        {
                            //double lastRow=Convert.ToDouble(dgvStep1[c, r-1].Value);
                            this.lblStep1E.Text += "第" + (c + 1) + "列第" + (r + 1) + "行超限；";
                        }
                        else if (r == 3 && Convert.ToDouble(dgvStep1[c, r].Value) >= Convert.ToDouble(dgvStep1[c, r - 1].Value))
                        {
                            this.lblStep1E.Text += "第3行必须大于第四行；";
                        }
                        else if (r == 4 && (Convert.ToDouble(dgvStep1[c, r].Value) < 100 || Convert.ToDouble(dgvStep1[c, r].Value) > 500))
                        {
                            this.lblStep1E.Text += "第" + (c + 1) + "列第" + (r + 1) + "行超限；";
                        }
                        else
                        {
                            step1Dic.Add(((step1Enum)r).ToString(), Convert.ToDouble(dgvStep1[c, r].Value));
                            if (tempData == 9999999999 && r == 0)
                            {
                                tempData = Convert.ToDouble(dgvStep1[c, r].Value);
                            }
                            else if (r == 1 && tempData <= Convert.ToDouble(dgvStep1[c, r].Value))
                            {
                                this.lblStep1E.Text += "第1行必须大于第2行；";
                            }
                        }
                    }
                    else if (r != 2 && r != 3 && r != 4)//第一行、第二行不能为空
                    {
                        this.lblStep1E.Text += "第" + c + "列第" + (r + 1) + "行不能为空；";
                    }
                }
            }

            if (rCount != 3  && rCount!=0)
            {
                this.lblStep1E.Text += "第" + 2 + "列第3、4、5行必须同时为空或同时有值；";
            }

            //错误信息悬浮方法
            tooltipEContent(this.lblStep1E);
        }
        #endregion

        #endregion

        #region step2
        private void BindDataStep2()
        {
            this.dgvStep2.Size = s;

            this.lblStep1E.Text = "";

            this.dgvStep2.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "终切点，℃" });
            this.dgvStep2.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "样品量，g" });
            this.dgvStep2.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "收率，w%" });
            this.dgvStep2.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "累计收率，w%" });
            for (int r = 0; r < 50; r++)
            {
                DataGridViewRow rowT = new DataGridViewRow();
                rowT.CreateCells(this.dgvStep2, "");
                this.dgvStep2.Rows.Add(rowT);
            }

            this.dgvStep2.Columns[2].ReadOnly = true;
            this.dgvStep2.Columns[3].ReadOnly = true;

            foreach (DataGridViewRow c in this.dgvStep2.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvStep2.Columns)
            {
               
                    c.Width = 92;
            }
            sortUnable(this.dgvStep2);//列标题禁用排序
        }


        private void dgvStep2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.ColumnIndex != 2 && e.ColumnIndex != 3)
            {
                this.dgvStep2.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvStep2.CurrentCell = this.dgvStep2.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvStep2.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        //第二步数据处理
        private void tsbDataDeal_Click(object sender, EventArgs e)
        {
            isShowMess = true;

            step2DealFormat();

            step2Compute();

            if (this.lblStep1E.Text.Length == 0) //没有出错信息，则转入下一步
            {
                isDealData = true;
            }
        }

        private void dgvStep2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
       e.RowBounds.Location.Y,
       this.dgvStep1.RowHeadersWidth - 4,
       e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvStep1.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvStep1.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #region 步骤2算法
        private void step2Compute()
        {
            wyList.Clear();
            twyList.Clear();

            #region 步骤2算法
            if (this.lblStep1E.Text.Length == 0)
            {
                var w1 = (from d in step1Dic where d.Key == "W1" select d).ToList();
                var w2 = (from d in step1Dic where d.Key == "W2" select d).ToList();
                var w3 = (from d in step1Dic where d.Key == "W3" select d).ToList();
                var w4 = (from d in step1Dic where d.Key == "W4" select d).ToList();
                var t = (from d in step1Dic where d.Key == "T" select d).ToList();

                if (w4.Count!=0 && cwList.LastOrDefault() == w4[0].Value)
                {
                    ctList.RemoveAt(ctList.Count - 1);
                    //cwList.RemoveAt(cwList.Count - 1);
                }

                for (int i = 0; i < ctList.Count; i++)
                {
                    if (ctList[i] <= t[0].Value)   //TWY(0)=0
                    {
                        wyList.Add(cwList[i] / w1[0].Value * 100);
                    }
                    else
                    {
                        wyList.Add(cwList[i] / w3[0].Value * w2[0].Value / w1[0].Value * 100);                       
                    }
                    if (i == 0)
                    {
                        twyList.Add(wyList[i]);
                    }
                    else
                    {
                        twyList.Add(twyList[i - 1] + wyList[i]);
                    }
                }

                double wyN1 = 0;
                if (w3.Count > 0)
                {
                    wyN1 = w4[0].Value / w3[0].Value * (w2[0].Value / w1[0].Value) * 100;
                    wyList.Add(wyN1);
                }
                else if (w3.Count == 0)
                {
                    wyN1 = (w2[0].Value / w1[0].Value) * 100;
                    wyList.Add(wyN1);
                }
                double twyN1 = twyList.LastOrDefault() + wyN1;
                twyList.Add(twyN1);

                //double loss = (w1[0].Value - w2[0].Value) / w1[0].Value * 100;
                double loss = 100 - twyN1;

                if (isShowMess == true)
                {
                    if (loss > 0.5)
                    {
                        // 提示用户“蒸馏损失>0.5%，是否退出重新校正数据？”
                        //是，退出算法；否，继续算法。
                        MessageBoxButtons messButton = MessageBoxButtons.YesNo;
                        DialogResult dr = MessageBox.Show("蒸馏损失>0.5%，是否退出重新校正数据？", "退出系统", messButton);
                        if (dr == DialogResult.Yes)//如果点击”是”按钮
                        {
                            this.lblStep1E.Text += "蒸馏损失>0.5%，请重新校正数据；";
                            return;
                        }
                    }
                }

                for (int r = 0; r < wyList.Count; r++)   //计算的WY结果显示出来
                {
                    this.dgvStep2[2, r].Value = Convert.ToDouble(wyList[r]).ToString("F2");
                }

                for (int r = 0; r < twyList.Count; r++)  //计算的TWY结果显示出来
                {
                    this.dgvStep2[3, r].Value = Convert.ToDouble(twyList[r]).ToString("F2");

                    if (r == twyList.Count - 1)   //最后一行将默认值2000和W4放上去
                    {
                        if (w4.Count != 0)
                        {
                            this.dgvStep2[0, r].Value = 2000;
                            this.dgvStep2[1, r].Value = w4[0].Value;
                        }
                    }
                }
            }
            #endregion
        }
        #endregion

        #region 步骤2数据格式判断
        private void step2DealFormat()
        {
            this.dgvStep2.EndEdit();

            ctList.Clear();
            cwList.Clear();

            isError = true;

            this.lblStep1E.Text = "";
            double tempData = 0.0;
            bool isIncrease = false;
            for (int c = 0; c < dgvStep2.Columns.Count - 3; c++)
            {
                for (int r = 0; r < dgvStep2.Rows.Count; r++)
                {
                    if (dgvStep2[c, r].Value != null && dgvStep2[c, r].Value.ToString() != "" && dgvStep2[c + 1, r].Value != null && dgvStep2[c + 1, r].Value.ToString() != "")
                    {
                        
                        bool a = Regex.IsMatch(dgvStep2[c, r].Value.ToString().Trim(), pattern);
                        if (a == false)  //如果非数字
                        {
                            this.lblStep1E.Text += "第" + (c+1) + "列第" + (r + 1) + "行非数字；";
                            isError = false;
                        }
                        else if (Convert.ToDouble(dgvStep2[c, r].Value) <= 0)
                        {
                            this.lblStep1E.Text += "第" + (c + 1) + "列第" + (r + 1) + "行超限；";
                            isError = false;
                        }
                        else
                        {
                            ctList.Add(Convert.ToDouble(dgvStep2[c, r].Value));
                           

                            if (tempData == 0.0)
                            {
                                tempData = Convert.ToDouble(dgvStep2[c, r].Value);
                            }
                            else if (isIncrease==false && tempData > Convert.ToDouble(dgvStep2[c, r].Value) )
                            {
                                this.lblStep1E.Text += "第"+(c+1)+"列数据必须单调递增；";
                                isError = false;
                                isIncrease = true;
                            }
                            else
                            {
                                tempData = Convert.ToDouble(dgvStep2[c, r].Value);
                            }
                        }

                        bool b = Regex.IsMatch(dgvStep2[c + 1, r].Value.ToString().Trim(), pattern);
                        if (b == false)  //如果非数字
                        {
                            this.lblStep1E.Text += "第" + (c + 2) + "列第" + (r + 1) + "行非数字；";
                            isError = false;
                        }
                        else if (Convert.ToDouble(dgvStep2[c + 1, r].Value) <= 0)
                        {
                            this.lblStep1E.Text += "第" + (c + 2) + "列第" + (r + 1) + "行超限；";
                            isError = false;
                        }
                        else
                        {
                            cwList.Add(Convert.ToDouble(dgvStep2[c + 1, r].Value));
                        }
                    }
                    else
                    {
                        if ((dgvStep2[c, r].Value != null && dgvStep2[c, r].Value.ToString() != "") || (dgvStep2[c + 1, r].Value != null && dgvStep2[c + 1, r].Value.ToString() != ""))
                        {
                            this.lblStep1E.Text += "第" + (r+1) + "行数据没有成对出现；";
                            isError = false;
                        }
                        else
                        {
                            bool isEmpty = false;  //用来判断是否有空行
                            for (int a = 0; a < dgvStep2.Columns.Count; a++)  //循环判断某空行下面剩余行是否为空行
                            {
                                for (int b = r + 1; b < dgvStep2.Rows.Count; b++)
                                {
                                    if (dgvStep2[a, b].Value != null && dgvStep2[a, b].Value.ToString() != "")
                                        isEmpty = true;
                                }
                            }

                            if (isEmpty == true)
                            {
                                this.lblStep1E.Text += "第" + (r + 1) + "行不允许空值；";
                                isError = false;
                            }
                        }
                    }
                }
            }

            if (ctList.Count == 0&&this.lblStep1E.Text.Length==0)
            {
                this.lblStep1E.Text += "请填写数据；";
                isError = false;
            }

            tooltipEContent(this.lblStep1E);
        }
        #endregion
        #endregion

        #region step3
        private void BindDataStep3()
        {
            this.dgvStep3.Size = s;
            this.lblStep1E.Text = "";

            this.dgvStep3.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "初馏点，℃" });
            this.dgvStep3.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "终馏点，℃" });
            this.dgvStep3.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "配样量，g" });
          
            for (int r = 0; r < 50; r++)
            {
                DataGridViewRow rowT = new DataGridViewRow();
                rowT.CreateCells(this.dgvStep3, "");
                this.dgvStep3.Rows.Add(rowT);
            }

            foreach (DataGridViewRow c in this.dgvStep3.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvStep3.Columns)
            {

                c.Width = 122;
            }
            sortUnable(this.dgvStep3);//列标题禁用排序
        }

        private void dgvStep3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 )
            {
                this.dgvStep3.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;//将当前单元格设为可读
                this.dgvStep3.CurrentCell = this.dgvStep3.Rows[e.RowIndex].Cells[e.ColumnIndex];//获取当前单元格
                this.dgvStep3.BeginEdit(true);//将单元格设为编辑状态
            }
        }

        private void dgvStep3_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
      e.RowBounds.Location.Y,
      this.dgvStep1.RowHeadersWidth - 4,
      e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvStep1.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvStep1.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }


        #region 步骤3数据格式判断
        /// <summary>
        /// 步骤3数据格式判断
        /// </summary>
        private void step3DealFormat()
        {
            icpList.Clear();
            ecpList.Clear();
            wwList.Clear();

            this.lblStep1E.Text = "";

            for (int r = 0; r < dgvStep3.Rows.Count; r++)
            {
                string cell1 = dgvStep3.Rows[r].Cells[0].Value == null ? string.Empty : dgvStep3.Rows[r].Cells[0].Value.ToString().Trim();//第一列数据
                string cell2 = dgvStep3.Rows[r].Cells[1].Value == null ? string.Empty : dgvStep3.Rows[r].Cells[1].Value.ToString().Trim();//第二列数据
                string cell3 = dgvStep3.Rows[r].Cells[2].Value == null ? string.Empty : dgvStep3.Rows[r].Cells[2].Value.ToString().Trim();//第三列数据

                bool a = Regex.IsMatch(cell1, pattern);
                bool b = Regex.IsMatch(cell2, pattern);
                bool c = Regex.IsMatch(cell3, pattern);

                if (cell1 == string.Empty && cell2 == string.Empty && cell3 == string.Empty)//三列全部为空
                {
                    continue;
                }
                else
                {
                    if (cell1 == string.Empty && cell2 == string.Empty)
                    {
                        this.lblStep1E.Text += "第" + (r + 1) + "行前2列不能同时为空；";
                    }
                    else if (cell1 != string.Empty && cell2 != string.Empty && cell3 == string.Empty)//第1、2列都有数据，第三列没数据
                    {
                        this.lblStep1E.Text += "第" + (r + 1) + "行第3列为空；";
                    }
                    else if (cell1 == string.Empty && cell2 != string.Empty && cell3 != string.Empty)//第一列无数据，第二列有数据，自动补-50
                    {
                        
                        if (b && c)
                        {
                            cell1 = (-50).ToString();//当第一列为空时，默认给-50

                            icpList.Add(Convert.ToDouble(cell1));
                            ecpList.Add(Convert.ToDouble(cell2));
                            wwList.Add(Convert.ToDouble(cell3));

                        }
                        else if(!b)
                        {
                            this.lblStep1E.Text += "第" + (r + 1) + "行第2列为非数字；";
                        }
                        else if (!c)
                        {
                            this.lblStep1E.Text += "第" + (r + 1) + "行第3列为非数字；";
                        }
                    }
                    else if (cell1 == string.Empty && cell2 != string.Empty && cell3 == string.Empty)
                    {
                        this.lblStep1E.Text += "第" + (r + 1) + "行第3列为空；";
                    }
                    else if (cell1 != string.Empty && cell2 == string.Empty && cell3 != string.Empty)//第一列有数据，第二列没数据，自动补2000
                    {
                        if (a && c)
                        {
                            cell2 = (2000).ToString();//当第二列为空时，默认给2000
                            icpList.Add(Convert.ToDouble(cell1));
                            ecpList.Add(Convert.ToDouble(cell2));
                            wwList.Add(Convert.ToDouble(cell3));
                        }
                        else if (!a)
                        {
                            this.lblStep1E.Text += "第" + (r + 1) + "行第1列为非数字；";
                        }
                        else if (!c)
                        {
                            this.lblStep1E.Text += "第" + (r + 1) + "行第3列为非数字；";
                        }
                    }
                    else if (cell1 != string.Empty && cell2 == string.Empty && cell3 == string.Empty)
                    {
                        this.lblStep1E.Text += "第" + (r + 1) + "行第3列为空；";
                    }
                    else if (cell1 != string.Empty && cell2 != string.Empty && cell3 != string.Empty)//三列都有数据
                    {
                        icpList.Add(Convert.ToDouble(cell1));
                        ecpList.Add(Convert.ToDouble(cell2));
                        wwList.Add(Convert.ToDouble(cell3));
                    }
                }
            }


            //for (int c = 0; c < dgvStep3.Columns.Count - 2; c++)//每一列
            //{
            //    for (int r = 0; r < dgvStep3.Rows.Count; r++)//每一行
            //    {
            //        if ((dgvStep3.Rows[r].Cells[0].Value == null || dgvStep3.Rows[r].Cells[0].Value.ToString() ==string.Empty) && dgvStep3.Rows[r].Cells[1].Value != null)
            //        {
            //            dgvStep3[0, r].Value = -50;
            //        }
            //        else if (dgvStep3.Rows[r].Cells[0].Value != null && dgvStep3.Rows[r].Cells[0].Value.ToString() != string.Empty && (dgvStep3.Rows[r].Cells[1].Value == null || dgvStep3.Rows[r].Cells[1].Value.ToString() == string.Empty))
            //        {
            //            dgvStep3[1, r].Value = 2000;
            //        }

            //        if (dgvStep3[c, r].Value != null && dgvStep3[c, r].Value.ToString() != "" && dgvStep3[c + 1, r].Value != null && dgvStep3[c + 1, r].Value.ToString() != "" && dgvStep3[c + 2, r].Value != null && dgvStep3[c + 2, r].Value.ToString() != "")
            //        {
                       
            //            bool a = Regex.IsMatch(dgvStep3[c, r].Value.ToString().Trim(), pattern);
            //            bool b = Regex.IsMatch(dgvStep3[c + 1, r].Value.ToString().Trim(), pattern);
            //            bool d = Regex.IsMatch(dgvStep3[c + 2, r].Value.ToString().Trim(), pattern);
            //            if (a == false)  //如果非数字
            //            {
            //                this.lblStep1E.Text += "第" + c + "列第" + (r + 1) + "行非数字；";
            //            }
            //            else if (b == false)  //如果非数字
            //            {
            //                this.lblStep1E.Text += "第" + (c + 1) + "列第" + (r + 1) + "行非数字；";
            //            }
            //            else if (d == false)  //如果非数字
            //            {
            //                this.lblStep1E.Text += "第" + (c + 2) + "列第" + (r + 1) + "行非数字；";
            //            }
            //            //else if (Convert.ToDouble(dgvStep3[c, r].Value) <= 0)
            //            //{
            //            //    this.lblStep1E.Text += "第" + c + "列第" + (r + 1) + "行超限；";
            //            //}
            //            else if (Convert.ToDouble(dgvStep3[c + 1, r].Value) <= 0)
            //            {
            //                this.lblStep1E.Text += "第" + c + "列第" + (r + 1) + "行超限；";
            //            }
            //            else if (Convert.ToDouble(dgvStep3[c + 2, r].Value) <= 0)
            //            {
            //                this.lblStep1E.Text += "第" + c + "列第" + (r + 1) + "行超限；";
            //            }
            //            else if (Convert.ToDouble(dgvStep3[c, r].Value) >= Convert.ToDouble(dgvStep3[c + 1, r].Value))
            //            {
            //                this.lblStep1E.Text += "第" + (c + 1) + "列第" + (r + 1) + "行数据必须小于第" + (c + 2) + "列第" + (r + 1) + "行数据；";
            //            }
            //            else
            //            {
            //                icpList.Add(Convert.ToDouble(dgvStep3[c, r].Value));
            //                ecpList.Add(Convert.ToDouble(dgvStep3[c + 1, r].Value));
            //                wwList.Add(Convert.ToDouble(dgvStep3[c + 2, r].Value));
            //            }

            //        }
            //        else
            //        {
            //            if ((dgvStep3[c, r].Value != null && dgvStep3[c, r].Value.ToString() != "") || (dgvStep3[c + 1, r].Value != null && dgvStep3[c + 1, r].Value.ToString() != ""))
            //            {
            //                this.lblStep1E.Text += "第" + (r + 1) + "行数据没有成对出现；";
            //            }
            //            else
            //            {
            //                bool isEmpty = false;  //用来判断是否有空行
            //                for (int a = 0; a < dgvStep3.Columns.Count; a++)  //循环判断某空行下面剩余行是否为空行
            //                {
            //                    for (int b = r + 1; b < dgvStep3.Rows.Count; b++)
            //                    {
            //                        if (dgvStep3[a, b].Value != null && dgvStep3[a, b].Value.ToString() != "")
            //                            isEmpty = true;
            //                    }
            //                }
            //                if (isEmpty == true)
            //                    this.lblStep1E.Text += "第1列第2列第3列数据没有成对出现或中间出现空值单元格；";
            //            }
            //        }
            //    }
            //}

            if (this.tsbtxtOilName1.Text.ToString() == "")
            {
                this.lblStep1E.Text += "原油名称或编号不能为空；";
            }

            if (this.tsbtxtLimit1.Text.ToString() == "")
            {
                this.lblStep1E.Text += "窄馏分余量限制不能为空；";
            }
            else
            {
                bool a = Regex.IsMatch(this.tsbtxtLimit1.Text.ToString(), pattern);
                if (a == false)  //如果非数字
                {
                    this.lblStep1E.Text += "窄馏分余量限制必须输入数字；";
                }
                else if (Convert.ToDouble(this.tsbtxtLimit1.Text) <= 0)
                {
                    this.lblStep1E.Text += "窄馏分余量限制必须大于0；";
                }
            }

            tooltipEContent(this.lblStep1E);
        }
        #endregion

        #region 步骤3算法
        bool isExitStep3Com = false;
        /// <summary>
        /// 步骤3算法
        /// </summary>
        private void step3Compute()
        {
            isExitStep3Com = false;
            ccwDic.Clear();
            for (int i = 0; i < icpList.Count; i++)
            {
                int start = -1, end = -1;
                if (icpList[i] == -50)
                {
                    start = 0;
                }
                if (ecpList[i] == 2000)
                {
                    end = ctList.Count;
                }
                for (int j = 0; j < ctList.Count; j++)
                {
                    if (icpList[i] == ctList[j])   //判断第三步的ICP与第二步的ICP是否对应
                        start = j;
                    if (ecpList[i] == ctList[j])
                        end = j;
                    if (start != -1 && end != -1)
                    {
                        break;
                    }
                }
                if (start == -1 || end == -1)
                {
                    //警告“第 i 馏分的切割点不为实测点！重新输入“，退出算法。
                    this.lblStep1E.Text += "第"+ (i+1) +"行馏分的切割点不为实测点,请重新输入；";
                    isExitStep3Com = true;
                    break;
                }

                Dictionary<int, double> valueDic = new Dictionary<int, double>();

                if (icpList[i] == -50)
                {
                    for (int j = start; j <= end; j++)
                    {

                        valueDic.Add(j, wwList[i] * wyList[j] / twyList[end]);

                        cwList[j] = cwList[j] - valueDic[j]; //余量计算
                    }
                    ccwDic.Add(i, valueDic);
                }
                else if (ecpList[i] == 2000)
                {
                    for (int j = start; j < end; j++)
                    {

                        valueDic.Add(j, wwList[i] * wyList[j + 1] / (twyList[end] - twyList[start]));

                        cwList[j + 1] = cwList[j + 1] - valueDic[j]; //余量计算(15点余量没变，不参与计算)
                    }
                    ccwDic.Add(i, valueDic);

                }
                else
                {
                    for (int j = start; j < end; j++)
                    {

                        valueDic.Add(j, wwList[i] * wyList[j + 1] / (twyList[end] - twyList[start]));

                        cwList[j + 1] = cwList[j + 1] - valueDic[j]; //余量计算(15点余量没变，不参与计算)
                    }
                    ccwDic.Add(i, valueDic);
                }
            }                        
        }
        #endregion
        #endregion


        #region 上一步 下一步 操作
        /// <summary>
        /// 上一步操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbLastStep_Click(object sender, EventArgs e)
        {
          
            if (stepNum == 1)  //第二步的上一步操作
            {
                this.dgvStep1.Visible = true;
                this.dgvStep2.Visible = false;
                this.dgvStep3.Visible = false;
                this.dgvDisplay.Visible = false;

                this.tsbDataDeal.Visible = false;
                this.tsbComputer.Visible = false;
                this.tsbLastStep.Visible = false;
                this.tsbNextStep.Visible = true;
                this.tsbClear.Visible = true;

                this.toolStrip2.Visible = false;
                this.tsbExcel.Visible = false; 

                this.lblStep1E.Text = "";

                stepNum--;
            }
            else if (stepNum == 2)  //第三步的上一步操作
            {
                this.dgvStep2.Visible = true;
                this.dgvStep1.Visible = false;
                this.dgvStep3.Visible = false;
                this.dgvDisplay.Visible = false;

                this.tsbDataDeal.Visible = true;
                this.tsbComputer.Visible = false;
                this.tsbNextStep.Visible = true;
                this.tsbClear.Visible = true;


                //this.tsblabLimit.Visible = false;
                //this.tsblabOilName.Visible = false;
                //this.tsbtxtLimit.Visible = false;
                //this.tsbtxtOilName.Visible = false;

                this.toolStrip2.Visible = false;
                this.tsbExcel.Visible = false;

                this.lblStep1E.Text = "";

                stepNum--;
            }
            else if (stepNum == 3)//完成计算的上一步操作
            {
                this.dgvStep1.Visible = false;
                this.dgvStep3.Visible = true;
                this.dgvStep2.Visible = false;
                this.dgvDisplay.Visible = false;

                this.tsbDataDeal.Visible = false;
                this.tsbComputer.Visible = true;
                this.tsbNextStep.Visible = false;
                this.tsbClear.Visible = true;
                this.tsbLastStep.Visible = true;

                //this.tsblabLimit.Visible = true;
                //this.tsblabOilName.Visible = true;
                //this.tsbtxtLimit.Visible = true;
                //this.tsbtxtOilName.Visible = true;

                this.toolStrip2.Visible = true;
                this.tsbExcel.Visible = false;


                stepNum--;

                //点击上一步后，cwList重新赋初值，避免余量叠加相减
                cwList.Clear();
                for (int c = 0; c < dgvStep2.Columns.Count - 3; c++)
                {
                    for (int r = 0; r < dgvStep2.Rows.Count; r++)
                    {
                        if (dgvStep2[c, r].Value != null && dgvStep2[c, r].Value.ToString() != "" && dgvStep2[c + 1, r].Value != null && dgvStep2[c + 1, r].Value.ToString() != "")
                        {
                            cwList.Add(Convert.ToDouble(dgvStep2[c + 1, r].Value));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbNextStep_Click(object sender, EventArgs e)
        {
            if (stepNum == 0)
            {
                this.dgvStep1.EndEdit();
                step1DealFormat(); //步骤1输入数据的格式检查
                if (this.lblStep1E.Text.Length == 0)  //没有出错信息，则转入下一步
                {
                    stepNum++;
                    // BindDataStep2();
                    this.dgvStep1.Visible = false;
                    this.dgvStep2.Visible = true;
                    this.dgvStep3.Visible = false;
                    this.dgvDisplay.Visible = false;

                    this.tsbDataDeal.Visible = true;
                    this.tsbComputer.Visible = false;
                    this.tsbLastStep.Visible = true;

                    this.toolStrip2.Visible = false;
                    this.tsbExcel.Visible = false;
                    //return;
                }
            }
            else if (stepNum == 1)  //点击步骤2的下一步按钮，则检查步骤2单元格数据时候有空行，不成对出现
            {
                if (this.lblStep1E.Text.Length == 0) //没有出错信息，则转入下一步
                {
                    if (isDealData == false)
                    {
                        this.lblStep1E.Text = "请数据处理后点击下一步；";
                    }
                    else
                    {
                        this.dgvStep2.EndEdit();

                        isShowMess = false;
                        step2DealFormat();
                        step2Compute();

                        for (int c = 0; c < dgvStep2.Columns.Count; c++)
                        {
                            for (int r = 0; r < ctList.Count; r++)
                            {
                                if (dgvStep2[c, r].Value == null || dgvStep2[c, r].Value.ToString() == "")
                                {

                                    this.lblStep1E.Text += "第" + c + "列第" + (r + 1) + "行不能为空；";
                                    isError = false;
                                }
                            }
                        }

                        if (isError == false)
                        {
                            return;
                        }

                        stepNum++;

                        this.dgvStep1.Visible = false;
                        this.dgvStep3.Visible = true;
                        this.dgvStep2.Visible = false;
                        this.dgvDisplay.Visible = false;

                        this.tsbDataDeal.Visible = false;
                        this.tsbComputer.Visible = true;
                        this.tsbNextStep.Visible = false;

                        //this.tsblabLimit.Visible = true;
                        //this.tsblabOilName.Visible = true;
                        //this.tsbtxtLimit.Visible = true;
                        //this.tsbtxtOilName.Visible = true;

                        this.toolStrip2.Visible = true;
                        this.tsbExcel.Visible = false;                       
                    }
                    return;
                }
            }
            else if (stepNum == 2) //点击步骤3的完成按钮，则检查步骤2单元格数据时候有空行，不成对出现
            {

            }
        }
        #endregion

        #region enum
        enum step1Enum { W1, W2, W3,W4, T }
        #endregion

        #region 完成按钮
        private void tsbComputer_Click(object sender, EventArgs e)
        {
            step3DealFormat();
            if (this.lblStep1E.Text.Length == 0)
            {
                step3Compute();

                if (isExitStep3Com == false)
                {
                    this.dgvDisplay.Visible = true;
                    this.dgvDisplay.Rows.Clear();
                    this.dgvDisplay.Columns.Clear();
                    DataBindDisplay(); //将最终结果显示表中

                    this.tsbLastStep.Visible = true;
                    this.tsbNextStep.Visible = false;
                    this.dgvStep3.Visible = false;
                    //this.tsblabOilName.Visible = false;
                    //this.tsblabLimit.Visible = false;
                    //this.tsbtxtOilName.Visible = false;
                    //this.tsbtxtLimit.Visible = false;

                    this.tsbClear.Visible = false;
                    this.tsbComputer.Visible = false;

                    this.toolStrip2.Visible = false;
                    this.toolStrip1.Visible = true;

                    this.tsbExcel.Visible = false;

                   // this.lblStep1E.Visible = false;

                    stepNum++;
                }
            }  
        }

        #endregion

        string titleContent = "";

        #region 绑定最终显示结果
        /// <summary>
        /// 绑定最终显示结果
        /// </summary>
        private void DataBindDisplay()
        {
            Size sr = new Size(850, 315);
            this.dgvDisplay.Size = sr;
            int countNW = 1;
            int countR = 1;

            string oilName = this.tsbtxtOilName1.Text;

            this.lblStep1E.Text = "";

            this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "样品编号" });
            this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "初馏点,℃" });
            this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "终馏点,℃" });
            this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "收率,%" });
            this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "余量,g" });
             titleContent = "\t样品编号\t初馏点,℃\t终馏点,℃\t收率,%\t余量,g";
            for (int i = 1; i < this.icpList.Count + 1; i++)  //循环，动态添加XXX-W&I(ICP(i)-ECP(i)) 表头
            {
                if (icpList[i - 1] == -50)//针对ICP为空的
                {
                    this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = oilName + "-W" + countNW + "\n<" + ecpList[i - 1] + "/" + wwList[i - 1] + "g" });
                    titleContent += "\t" + oilName + "-W" + countNW + "<" + ecpList[i - 1] + "/" + wwList[i - 1] + "g";
                    countNW++;
                }
                else if (ecpList[i - 1] == 2000)//针对ECP为空的
                {
                    //this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = oilName + "-W" + i + "\n>" + icpList[i - 1] + "/" + wwList[i - 1] + "g" });
                    //titleContent += "\t" + oilName + "-W" + i + ">" + icpList[i - 1] + "/" + wwList[i - 1] + "g";
                    //k=i+1;
                    this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = oilName + "-R" + countR + "\n>" + icpList[i - 1] + "/" + wwList[i - 1] + "g" });
                    titleContent += "\t" + oilName + "-R" + countR + ">" + icpList[i - 1] + "/" + wwList[i - 1] + "g";
                    countR++;
                }
                else
                {
                    this.dgvDisplay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = oilName + "-W" + countNW + "\n(" + icpList[i - 1] + "-" + ecpList[i - 1] + ")" + "/" + wwList[i - 1] + "g" });
                    titleContent += "\t" + oilName + "-W" + countNW + "(" + icpList[i - 1] + "-" + ecpList[i - 1] + ")" + "/" + wwList[i - 1] + "g";
                    countNW++;
                }
            }

            for (int j = 0; j < this.ctList.Count; j++) //循环，依次将各数组中数据填入第一列到第5列的单元格中
            {
                DataGridViewRow rowT = new DataGridViewRow();
                if (j == 0)
                {
                    rowT.CreateCells(this.dgvDisplay, oilName + "-N" + j, "", Convert.ToDouble(ctList[j]).ToString("F2"), Convert.ToDouble(wyList[j]).ToString("F2"), Convert.ToDouble(cwList[j]).ToString("F2"));
                    this.dgvDisplay.Rows.Add(rowT);
                }
                else if (j < this.ctList.Count)
                {
                    if (ctList[j] != 2000)
                    {
                        rowT.CreateCells(this.dgvDisplay, oilName + "-N" + j, ctList[j - 1], ctList[j], Convert.ToDouble(wyList[j]).ToString("F2"), Convert.ToDouble(cwList[j]).ToString("F2"));
                        this.dgvDisplay.Rows.Add(rowT);
                    }
                    if (j == this.ctList.Count - 1)//最后再多增加一行
                    {
                        if (this.cwList.Count - this.ctList.Count == 1)
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            //row.CreateCells(this.dgvDisplay, oilName + "-W" + k, ctList[j], "", Convert.ToDouble(wyList[j + 1]).ToString("F2"), Convert.ToDouble(cwList[j + 1]).ToString("F2"));
                            row.CreateCells(this.dgvDisplay, oilName + "-R", ctList[j], "", Convert.ToDouble(wyList[j + 1]).ToString("F2"), Convert.ToDouble(cwList[j + 1]).ToString("F2"));
                            this.dgvDisplay.Rows.Add(row);
                        }
                    }
                }
                //this.dgvDisplay.Rows.Add(rowT);
            }

            foreach (int i in ccwDic.Keys)  //遍历存放CCW数据的数组，依次填入表中相应单元格内
            {
                foreach (int j in ccwDic[i].Keys)
                {
                    if (icpList[i] == -50)
                    {
                        this.dgvDisplay[i + 5, j].Value = Convert.ToDouble(ccwDic[i][j]).ToString("F2");
                    }
                    else
                    {
                        this.dgvDisplay[i + 5, j + 1].Value = Convert.ToDouble(ccwDic[i][j]).ToString("F2");
                    }
                }
            }

            for (int r = 0; r < dgvDisplay.Rows.Count; r++)
            {
                if (Convert.ToDouble(this.dgvDisplay[4, r].Value) < Convert.ToDouble(this.tsbtxtLimit1.Text))
                {
                    this.lblStep1E.Text += "第5列第" + (r + 1) + "行窄馏分的余量不足!";
                    this.dgvDisplay[4, r].Style.ForeColor = Color.Red;
                }
            }

            foreach (DataGridViewRow c in this.dgvDisplay.Rows)
            { c.Height = commonHeight; }

            foreach (DataGridViewColumn c in this.dgvDisplay.Columns)
            {
                if (c.Index == 0)
                {
                    c.Width = 96;
                }
                else
                    c.Width = 96;
            }

            tooltipEContent(this.lblStep1E);
            sortUnable(this.dgvDisplay);//禁用列标题排序
        }
        #endregion

        private void lblStep1E_MouseHover(object sender, EventArgs e)
        {
            if (this.lblStep1E.Text.Length > 34)
            {
                this.lblStep1E.Cursor = Cursors.Hand;
                this.toolTip1.Active = true;
            }
            else
            {
                this.toolTip1.Active = false;
            }
        }


        #region 错误信息内容悬浮
        /// <summary>
        /// 错误信息内容悬浮
        /// </summary>
        /// <param name="lbl"></param>
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


        private void tsbClear_Click(object sender, EventArgs e)
        {
            if (stepNum == 0)
            {
                this.dgvStep1.Rows.Clear();
                this.dgvStep1.Columns.Clear();
                BindDataStep1();
            }
            else if (stepNum == 1)
            {
                this.dgvStep2.Rows.Clear();
                this.dgvStep2.Columns.Clear();
                BindDataStep2();
            }
            else if (stepNum == 2)
            {
                this.dgvStep3.Rows.Clear();
                this.dgvStep3.Columns.Clear();
                BindDataStep3();
            }
            else
            {
                //this.dgvDisplay.Rows.Clear();
                //this.dgvDisplay.Columns.Clear();
                //DataBindDisplay();
            }
        }

        private void dgvDisplay_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
     e.RowBounds.Location.Y,
     this.dgvStep1.RowHeadersWidth - 4,
     e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dgvStep1.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dgvStep1.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        #region 快捷菜单  复制 粘贴 删除
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.Control control in this.panel1.Controls)
            {
                if (control is System.Windows.Forms.DataGridView)
                {
                    DataGridView dgv = (DataGridView)control;
                    if(dgv.Visible==true)
                        CopyToClipboard(dgv);
                }
            }
        }

        /// <summary>
        /// 普通表格的复制
        /// </summary>
        /// <param name="dataGridView1"></param>
        public  void CopyToClipboard(DataGridView dataGridView1)
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
            {
                if (this.dgvDisplay.Visible == true && this.dgvDisplay.SelectedCells.Count == dgvDisplay.Rows.Count * dgvDisplay.Columns.Count)//只有全部选择时才复制表头
                {
                    Clipboard.SetDataObject(titleContent.Substring(1) + "\r\n" + dataObj.GetText());
                }
                else
                {
                    Clipboard.SetDataObject(dataObj.GetText());
                }
            }
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.Control control in this.panel1.Controls)
            {
                if (control is System.Windows.Forms.DataGridView)
                {
                    DataGridView dgv = (DataGridView)control;
                    //string name = dgv.Name;
                    if (dgv.Visible == true)
                    {
                        GridOilDataEdit.CopyToClipboard(dgv);
                        GridOilDataEdit.DeleteValues(dgv);
                    }
                }
            }
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.Control control in this.panel1.Controls)
            {
                if (control is System.Windows.Forms.DataGridView)
                {
                    DataGridView dgv = (DataGridView)control;
                    if (dgv.Visible == true)
                        GridOilDataEdit.PasteClipboardValue(dgv);
                }
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.Control control in this.panel1.Controls)
            {
                if (control is System.Windows.Forms.DataGridView)
                {
                    DataGridView dgv = (DataGridView)control;
                    if (dgv.Visible == true)
                        GridOilDataEdit.DeleteValues(dgv);
                }
            }
        }
        #endregion

        #region 导出当前页到Excel中

        //按下导出按钮

        //public void print(DataGridView dataGridView1)
        //{
        //    //导出到execl
        //    try
        //    {
        //        //没有数据的话就不往下执行
        //        if (dataGridView1.Rows.Count == 0)
        //            return;
        //        //实例化一个Excel.Application对象
        //        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

        //        //让后台执行设置为不可见，为true的话会看到打开一个Excel，然后数据在往里写
        //        excel.Visible = false;

        //        //新增加一个工作簿，Workbook是直接保存，不会弹出保存对话框，加上Application会弹出保存对话框，值为false会报错
        //        excel.Application.Workbooks.Add(true);
        //        //生成Excel中列头名称
        //        for (int i = 0; i < dataGridView1.Columns.Count; i++)
        //        {
        //            excel.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
        //        }
        //        //把DataGridView当前页的数据保存在Excel中
        //        for (int i = 0; i < dataGridView1.Rows.Count; i++)
        //        {
        //            for (int j = 0; j < dataGridView1.Columns.Count; j++)
        //            {
        //                if (dataGridView1[j, i].ValueType == typeof(string))
        //                {
        //                    excel.Cells[i + 2, j + 1] = "'" + dataGridView1[j, i].Value.ToString();
        //                }
        //                else
        //                {
        //                    excel.Cells[i + 2, j + 1] = dataGridView1[j, i].Value == null ? "" : dataGridView1[j, i].Value.ToString();
        //                }
        //            }
        //        }

        //        //设置禁止弹出保存和覆盖的询问提示框
        //        excel.DisplayAlerts = false;
        //        excel.AlertBeforeOverwriting = false;

        //        //保存工作簿
        //        excel.Application.Workbooks.Add(true).Save();
        //        //保存excel文件
        //      //  excel.Save("D:" + "\\KKHMD.xls");

        //        //确保Excel进程关闭
        //        excel.Quit();
        //        excel = null;

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "错误提示");
        //    }
        //}

        /// <summary>
        /// 另存新档按钮
        /// </summary>
        private void SaveAs(DataGridView dgvAgeWeekSex)//另存新档按钮   导出成Excel
        {
            //SaveFileDialog saveFileDialog = new SaveFileDialog();

            //saveFileDialog.Filter = "Execl files (*.xls)|*.xls";

            //saveFileDialog.FilterIndex = 0;

            //saveFileDialog.RestoreDirectory = true;

            //saveFileDialog.CreatePrompt = true;

            //saveFileDialog.Title = "Export Excel File To";

            //saveFileDialog.ShowDialog();

            //Stream myStream;

            //myStream = saveFileDialog.OpenFile();

            ////StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));

            //StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));

            //string str = "";

            //try
            //{

            //    //写标题

            //    for (int i = 0; i < dgvAgeWeekSex.ColumnCount; i++)
            //    {

            //        if (i > 0)
            //        {
            //            str += "\t";
            //        }
            //        str += dgvAgeWeekSex.Columns[i].HeaderText;
            //    }
            //    str = str.Replace("\n", ",");
            //    sw.WriteLine(str);

            //    //写内容
            //    for (int j = 0; j < dgvAgeWeekSex.Rows.Count; j++)
            //    {
            //        string tempStr = "";
            //        for (int k = 0; k < dgvAgeWeekSex.Columns.Count; k++)
            //        {
            //            if (k > 0)
            //            {
            //                tempStr += "\t";
            //            }
            //            tempStr += dgvAgeWeekSex.Rows[j].Cells[k].Value==null?"":dgvAgeWeekSex.Rows[j].Cells[k].Value.ToString();
            //        }

            //        sw.WriteLine(tempStr);
            //    }
            //    sw.Close();
            //    myStream.Close();
            //}
            //catch (Exception e)
            //{

            //    MessageBox.Show(e.ToString());

            //}

            //finally
            //{

            //    sw.Close();

            //    myStream.Close();

            //}

        }

        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SaveAs(dgvDisplay);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            foreach (System.Windows.Forms.Control control in this.panel1.Controls)
            {
                if (control is System.Windows.Forms.DataGridView)
                {
                    DataGridView dgv = (DataGridView)control;
                    if (dgv.Visible == true)
                    {
                        if (dgv.Name == "dgvDisplay")
                        {
                            //this.contextMenuStrip1.Items[1].Enabled = false;
                            //this.contextMenuStrip1.Items[2].Enabled = false;
                            //this.contextMenuStrip1.Items[3].Enabled = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 列标题禁用排序
        /// </summary>
        /// <param name="dgv"></param>
        private void sortUnable(DataGridView dgv)
        {
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            //dgv.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }
    }
}
