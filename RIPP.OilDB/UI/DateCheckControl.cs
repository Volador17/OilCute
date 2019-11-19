using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data.DataCheck;
using RIPP.Lib;
namespace RIPP.OilDB.UI
{
    public partial class DateCheckControl : UserControl
    {
        private bool bYvalid = false ;
        private bool bMvalid = false;
        private bool bDvalid = false;
        private DateTime? _Time = DateTime.Now;
        /// <summary>
        /// 当前短日期
        /// </summary>
        public DateTime? ShortTime
        {       
            get 
            {
                string temp = this.txBYear.Text.Trim() + this.txBMonth.Text.Trim() + this.txBDay.Text.Trim();

                if (!temp.Equals(string.Empty))
                {
                    StringBuilder strTime = new StringBuilder();
                    strTime.Append(this.txBYear.Text.Trim());
                    if (this.txBMonth.Text.Trim().Length == 1)
                        strTime.Append("-0" + this.txBMonth.Text.Trim());
                    else
                        strTime.Append("-" + this.txBMonth.Text.Trim());

                    if (this.txBDay.Text.Trim().Length == 1)
                        strTime.Append("-0" + this.txBDay.Text.Trim());
                    else
                        strTime.Append("-" + this.txBDay.Text.Trim());

                    if (!dataCheck.checkDate(strTime.ToString()))
                    {
                        MessageBox.Show("日期不正确!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this._Time = null;
                        this.txBYear.Text = string.Empty;
                        this.txBMonth.Text = string.Empty;
                        this.txBDay.Text = string.Empty;
                    }
                    else
                    {
                        this._Time = dataCheck.GetDate(strTime);
                    }
                }
                else
                    this._Time = null;
                return  this._Time;           
            }

            set
            {
                this._Time = value;  
                this.txBYear.Text = this._Time != null ? this._Time.Value.Year.ToString(): string.Empty;
                this.txBMonth.Text = this._Time != null ? this._Time.Value.Month.ToString(): string.Empty;
                this.txBDay.Text = this._Time!= null ? this._Time.Value.Day.ToString() : string.Empty;
                //this.bYvalid = true;
                //this.bDvalid = true;
                //this.bMvalid = true;
            }
        }

        private OilDataCheck dataCheck = new OilDataCheck();
        public DateCheckControl()
        {
            InitializeComponent();
        }

        private void teBYear_Validating(object sender, CancelEventArgs e)
        {
            
        }

        private void teBYear_KeyDown(object sender, KeyEventArgs e)
        {
            string strTemp = this.txBYear.Text.Trim();
                      
        }

       
        /// <summary>
     
  
        private void teBYear_Validated(object sender, EventArgs e)
        {
            string strTemp = this.txBYear.Text.Trim();
            if (strTemp.Length != 4)
                return;
            int iTemp = 0;
            if (!strTemp.Equals(string.Empty) && int.TryParse(strTemp, out iTemp))
            {
                if (iTemp < 1900)
                {
                    this.bYvalid = false;
                    MessageBox.Show("日期超出范围!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.txBYear.Focus();
                    this.txBYear.Text = strTemp;
                    this.Visible = true;
                }
                else
                    this.bYvalid = true;
            }
            else
            {
                this.bYvalid = true;
                //MessageBox.Show("日期不能为空或非数字!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.txBYear.Focus();
            }
        }
        private void txBMonth_Validating(object sender, CancelEventArgs e)
        {
            string strTemp = this.txBMonth.Text.Trim();
            int iTemp = 0;
            if (!strTemp.Equals(string.Empty) && int.TryParse(strTemp, out iTemp))
            {
                if (iTemp < 1 || iTemp > 12)
                {
                    bMvalid = false;
                    MessageBox.Show("日期超出范围!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.txBMonth.Focus();
                    this.txBMonth.Text = strTemp;
                    this.Visible = true;
                }
                else
                    bMvalid = true;
            }
            else
            {
                bMvalid = true;
                //MessageBox.Show("日期不能为空或非数字!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.txBMonth.Focus();
            }
        }

        private void txBDay_Validated(object sender, EventArgs e)
        {
            string strTemp = this.txBDay.Text.Trim();
            int iTemp = 0;
            if (!strTemp.Equals(string.Empty) && int.TryParse(strTemp, out iTemp))
            {
                if (iTemp < 1 || iTemp > 31)
                {
                    bDvalid = false;
                    MessageBox.Show("日期超出范围!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.txBDay.Focus();
                    this.txBDay.Text = strTemp;
                    this.Visible = true;
                }
                else
                    bDvalid = true;
            }
            else
            {
                bDvalid = true;
                //MessageBox.Show("日期不能为空或非数字!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.txBDay.Focus();
            }
        }
        /// <summary>
        /// enter结束编辑
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                string strYTemp = this.txBYear.Text.Trim();
                if (!strYTemp.Equals(string.Empty))
                {
                    if (strYTemp.Length != 4)
                    {
                        MessageBox.Show("日期超出范围!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.txBYear.Focus();
                        this.txBYear.Text = strYTemp;
                    }
                    else
                    {
                        int iYTemp = 0;
                        if (int.TryParse(strYTemp, out iYTemp))
                        {
                            if (iYTemp < 1900)
                            {
                                this.bYvalid = false;
                                MessageBox.Show("日期超出范围!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.txBYear.Focus();
                                this.txBYear.Text = strYTemp;
                                this.Visible = true;
                            }
                            else
                                this.bYvalid = true;
                        }
                    }
                }
               
                string strMTemp = this.txBMonth.Text.Trim();
                int iMTemp = 0;
                if (!strYTemp.Equals(string.Empty) && int.TryParse(strMTemp, out iMTemp))
                {
                    if (iMTemp < 1 || iMTemp > 12)
                    {
                        bMvalid = false;
                        MessageBox.Show("日期超出范围!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.txBMonth.Focus();
                        this.txBMonth.Text = strYTemp;
                        this.Visible = true;
                    }
                    else
                        bMvalid = true;
                }


                string strDTemp = this.txBDay.Text.Trim();
                int iDTemp = 0;
                if (!strDTemp.Equals(string.Empty) && int.TryParse(strDTemp, out iDTemp))
                {
                    if (iDTemp < 1 || iDTemp > 31)
                    {
                        bDvalid = false;
                        MessageBox.Show("日期超出范围!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.txBDay.Focus();
                        this.txBDay.Text = strDTemp;
                        this.Visible = true;
                    }
                    else
                        bDvalid = true;
                }

                string temp = this.txBYear.Text.Trim() + this.txBMonth.Text.Trim() + this.txBDay.Text.Trim();

                if (!temp.Equals(string.Empty))
                {
                    StringBuilder strTime = new StringBuilder();
                    strTime.Append(this.txBYear.Text.Trim());
                    if (this.txBMonth.Text.Trim().Length == 1)
                        strTime.Append("-0" + this.txBMonth.Text.Trim());
                    else
                        strTime.Append("-" + this.txBMonth.Text.Trim());

                    if (this.txBDay.Text.Trim().Length == 1)
                        strTime.Append("-0" + this.txBDay.Text.Trim());
                    else
                        strTime.Append("-" + this.txBDay.Text.Trim());

                    if (!dataCheck.checkDate(strTime.ToString()))
                    {
                        MessageBox.Show("日期不正确!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Visible = false;
                    }
                    else
                    {
                        this.Visible = false;
                    }
                }
                else
                    this.Visible = false;

                return true;
            }

            return base.ProcessDialogKey(keyData);

        }
        private void DateCheckControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string str = "";
            }
        }
        
        private void txBDay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData  == Keys.Enter)
            {
                string str = "";
            }
        }
    }
}
