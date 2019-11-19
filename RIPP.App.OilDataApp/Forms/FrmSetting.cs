using RIPP.OilDB.BLL;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace RIPP.App.OilDataApp.Forms
{
    public partial class FrmSetting : Form
    {
         
        public FrmSetting()
        {
            InitializeComponent();
            ConfigBll config = new ConfigBll();
            this.textBox1.Text = config.getDir(enumModel.AppCut);
            this.textBox2.Text = config.getDir(enumModel.AppXls);
        }

        private void btnCutMothed_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "切割方案文件 (*.cut)|*.cut";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog.FileName;//得到文件名
                //this.textBox1.Tag = System.IO.Path.GetFileName(openFileDialog.FileName);//得到文件名
            }          
        }
        private void btnOutExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择输出模版";
            openFileDialog.Filter = "原油数据模版文件 (*.xls)|*.xls";//Excel2010与2003不兼容
            openFileDialog.RestoreDirectory = true;
             
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = openFileDialog.FileName;//得到文件名
               // this.textBox2.Tag = System.IO.Path.GetFileName(openFileDialog.FileName);//得到文件名
            }   
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            string xmlFilePath = ConfigBll._startupPath;//路径
            ConfigBll confige = new ConfigBll();

            if (!string.IsNullOrWhiteSpace (this.textBox1.Text))
                confige.updateItem( this.textBox1.Text, enumModel.AppCut);

            if (!string.IsNullOrWhiteSpace(this.textBox2.Text))
                confige.updateItem(this.textBox2.Text,   enumModel.AppXls);

            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }       
    }
}
