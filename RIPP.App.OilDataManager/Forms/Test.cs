using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.App.OilDataManager.Forms
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
            this.pager1.PageCurrent = 1;
            this.pager1.Bind();

        }

        private int dgvBind()
        {
            OilDB.UI.Pager.PageData pageData = new OilDB.UI.Pager.PageData();
            pageData.TableName = "OilInfo";
            pageData.PrimaryKey = "ID";
            pageData.OrderStr = "ID desc";
            pageData.PageIndex = this.pager1.PageCurrent;
            pageData.PageSize = this.pager1.PageSize;
            pageData.QueryCondition = "";
            pageData.QueryFieldName = "*";

            this.pager1.bindingSource.DataSource = pageData.QueryDataTable().Tables[0];
            this.pager1.bindingNavigator.BindingSource = pager1.bindingSource;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = this.pager1.bindingSource;
            return pageData.TotalCount;
        }

        private int pager1_EventPaging(OilDB.UI.Pager.EventPagingArg e)
        {
            return dgvBind();

        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }
    }
}
