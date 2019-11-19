using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;

namespace RIPP.App.OilDataManager.Forms.DatabaseA
{  
    public partial class FrmRemark : Form
    {
        /// <summary>
        /// 一条记录
        /// </summary>
        private RemarkEntity _remarkData = null;


        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmRemark()
        {
            InitializeComponent();           
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remarkData"></param>
        public FrmRemark(RemarkEntity remarkData)
        {
            InitializeComponent();
            this.Name = "frmRemark";
            this._remarkData = remarkData;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            if (this._remarkData != null && this._remarkData.LaborCal == 0)
            {
                this.textBox1.Text = this._remarkData.LabRemark;
            }
            else if (this._remarkData != null && this._remarkData.LaborCal == 1)
            {
                this.textBox1.Text = this._remarkData.CalRemark;
            }
            
        }
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrWhiteSpace(this.textBox1.Text))
            //{
                GlobalRemark.YesNo = System.Windows.Forms.DialogResult.OK;
                GlobalRemark.message = this.textBox1.Text;
            //}
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 手动修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox1.Text += "手动修改 = ";
        }
        /// <summary>
        /// 问题按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.textBox1.Text += "问题 : ";
        }
        /// <summary>
        /// 清空按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = string.Empty;
        }
    }

    public static class GlobalRemark
    {
        /// <summary>
        /// 对话形式
        /// </summary>
        public static DialogResult YesNo = DialogResult.Yes;
        /// <summary>
        /// 批注
        /// </summary>
        public static string message = string.Empty;
    }
}
