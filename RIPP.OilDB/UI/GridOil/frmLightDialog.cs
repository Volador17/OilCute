using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.GCBLL;

namespace RIPP.OilDB.UI.GridOil
{
    public partial class frmLightDialog : Form
    {
        /// <summary>
        /// 窄馏分数据表
        /// </summary>
        private GridOilViewA _NarrowGridOil = null;


        public frmLightDialog()
        {
            InitializeComponent();
            this.button1.DialogResult = DialogResult.OK;
            this.button2.DialogResult = DialogResult.Cancel;

            this.label1.Visible = false;
            this.label2.Visible = false;
            this.textBox1.Visible = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NarrowGridOil"></param>
        public void init(GridOilViewA NarrowGridOil)
        {
            this._NarrowGridOil = NarrowGridOil;    
        }
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                GlobalLightDialog.YesNo = System.Windows.Forms.DialogResult.Yes;
                OilDataEntity WYOilData = this._NarrowGridOil.GetDataByRowItemCodeColumnIndex("WY", 0);
                string strWY = WYOilData == null ? string.Empty : WYOilData.calData;
                float tempWY = 0;
                if (strWY != string.Empty)
                {
                    if (float.TryParse(strWY, out tempWY))
                    {
                        GlobalLightDialog.LightWY = tempWY;
                    }
                }
            }
            else if (this.radioButton2.Checked)
            {
                GlobalLightDialog.YesNo = System.Windows.Forms.DialogResult.No;
                float tempWY = 0 ;
                if (this.textBox1.Text != string.Empty)
                {
                    if (float.TryParse(this.textBox1.Text, out tempWY))
                    {
                        if (tempWY > 100 || tempWY < 0)
                        {
                            MessageBox.Show("轻端的质量收率必须在0到100之间");
                            this.textBox1.Focus();
                            return;
                        }
                        else
                        {
                            GlobalLightDialog.LightWY = tempWY;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请输入轻端的质量收率");
                    this.textBox1.Focus();
                    return;
                }
            }
            //this.Close();           
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 是
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                this.label1.Visible = false;
                this.label2.Visible = false;
                this.textBox1.Visible = false;
            }
            else if (this.radioButton2.Checked)
            {
                this.label1.Visible = true;
                this.label2.Visible = true;
                this.textBox1.Visible = true;
            }
        }
        /// <summary>
        /// 否
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox1.Focus();

            if (this.radioButton1.Checked)
            {
                this.label1.Visible = false;
                this.label2.Visible = false;
                this.textBox1.Visible = false;
            }
            else if (this.radioButton2.Checked)
            {
                this.label1.Visible = true ;
                this.label2.Visible = true;
                this.textBox1.Visible = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
            {
                e.Handled = true;
            }
        } 
    }
}
