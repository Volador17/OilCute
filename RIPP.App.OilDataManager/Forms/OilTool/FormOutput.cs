/*
 * czw导出测试数据专用
 */


using System;
using System.Windows.Forms;
using RIPP.OilDB.BLL;

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FormOutput : Form
    {
        public FormOutput()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string itemCode = this.textBox1.Text.Trim();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择输出模版";
            openFileDialog.Filter = "原油数据模版文件 (*.xls)|*.xls";//Excel2010与2003不兼容
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int outResult = DataToExcelBll.outDataToExcel(openFileDialog.FileName, itemCode);
                if (outResult == 1)
                {
                    MessageBox.Show("数据导出成功!", "提示");
                }
                else if (outResult == -1)
                {
                    MessageBox.Show("当前系统尚未安装EXCEL软件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -2)
                {
                    MessageBox.Show("不能打开Excel进程,请关闭Excel后重试!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -3)
                {
                    MessageBox.Show("数据导出失败，请检查模版是否正确或者关闭正在运行的Excel重试!", "提示");
                }
                else if (outResult == -11)
                {
                    MessageBox.Show("切割数据不存在!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -99)
                {
                    MessageBox.Show("数据导出失败，找不到正确模板!", "提示");
                }
                else if (outResult == 0)//取消保存
                {
                    return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择输出模版";
            openFileDialog.Filter = "原油数据模版文件 (*.xls)|*.xls";//Excel2010与2003不兼容
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int outResult = DataToExcelBll.outGCDataToExcel(openFileDialog.FileName);
                if (outResult == 1)
                {
                    MessageBox.Show("数据导出成功!", "提示");
                }
                else if (outResult == -1)
                {
                    MessageBox.Show("当前系统尚未安装EXCEL软件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -2)
                {
                    MessageBox.Show("不能打开Excel进程,请关闭Excel后重试!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -3)
                {
                    MessageBox.Show("数据导出失败，请检查模版是否正确或者关闭正在运行的Excel重试!", "提示");
                }
                else if (outResult == -11)
                {
                    MessageBox.Show("切割数据不存在!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -99)
                {
                    MessageBox.Show("数据导出失败，找不到正确模板!", "提示");
                }
                else if (outResult == 0)//取消保存
                {
                    return;
                }
            }
        }
    }
}
