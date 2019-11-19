using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.AnalCenter.Busi;
using RIPP.Lib;
using System.Threading;
namespace RIPP.App.AnalCenter.Forms.Dlg
{
    public partial class FrmOilDetailcs : Form
    {
        private List<PropertyTable> _tbs;
        public FrmOilDetailcs()
        {
            InitializeComponent();
        }

        public void ShowData(List<PropertyTable> tbs)
        {
            if (tbs == null)
                return;
            this._tbs = tbs;
            foreach (var tb in tbs)
            {
                var page = new Ctrl.PropertyGrid(){ Dock= DockStyle.Fill};
                var tbp = new TabPage() { Text = tb.Table.GetDescription() };
                tbp.Controls.Add(page);
                this.tabControl1.TabPages.Add(tbp);
                page.LoadData(tb, false);
            }
            this.ShowDialog();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件|*.*|xls文件|*.xls|所有文件|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.FileName = @"Export.xls";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = (new Config()).FolderData;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = saveFileDialog.FileName.ToString();
                this.btnExport.Enabled = false;
                this.lblBusy.Text = "正在导出...";

                Action a = () =>
                {
                    if (!ExcelExportProvider.ExportToExcel(this._tbs, fName))
                        MessageBox.Show("导出失败");
                    if (this.toolStrip1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.btnExport.Enabled = true; };
                        this.toolStrip1.Invoke(s);
                    }
                    else
                        this.btnExport.Enabled = true;
                    if (this.toolStrip1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.lblBusy.Text = ""; };
                        this.toolStrip1.Invoke(s);
                    }
                    else
                        this.lblBusy.Text = "";
                };
                a.BeginInvoke(null, null);

            }
        }

        private void btn_LIMS_Click(object sender, EventArgs e)
        {
            var frm = new UploadLims();
            frm.ShowData(this._tbs);
        }
    }
}
