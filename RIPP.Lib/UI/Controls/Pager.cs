using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.Lib.UI.Controls
{
    public class Pager : ToolStrip
    {
        private System.Windows.Forms.ToolStripButton btnFirst;
        private System.Windows.Forms.ToolStripButton btnPre;
        private System.Windows.Forms.ToolStripTextBox txbPage;
        private System.Windows.Forms.ToolStripLabel lblPage;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripButton btnLast;
        private System.Windows.Forms.ToolStripLabel lblInfo;


        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public event EventHandler PageChange;


        public Pager()
        {
            this.init();
        }

        public void ShowPage(int pageIndex, int pageSize, int totalcount)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalcount;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            this.btnFirst.Enabled = this.HasFirst;
            this.btnLast.Enabled = this.HasLast;
            this.btnPre.Enabled = this.HasPreviousPage;
            this.btnNext.Enabled = this.HasNextPage;
            this.txbPage.Text = this.PageIndex.ToString();
            this.lblPage.Text = string.Format("/{0}", TotalPages);
            this.lblInfo.Text = string.Format("共{0}条记录，当前是第{1}到{2}条。",
                TotalCount,
                (PageIndex - 1) * PageSize + 1,
                PageIndex < TotalPages ? PageIndex * PageSize : TotalCount
                );
        }

        public bool HasFirst
        {
            get
            {
                return PageIndex > 1;
            }
        }
        public bool HasLast
        {
            get
            {
                return PageIndex < TotalPages;
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }




        private void init()
        {
            this.txbPage = new System.Windows.Forms.ToolStripTextBox();
            this.lblPage = new System.Windows.Forms.ToolStripLabel();
            this.lblInfo = new System.Windows.Forms.ToolStripLabel();
            this.btnFirst = new System.Windows.Forms.ToolStripButton();
            this.btnPre = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnLast = new System.Windows.Forms.ToolStripButton();

            // 
            // toolStrip1
            // 
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFirst,
            this.btnPre,
            this.txbPage,
            this.lblPage,
            this.btnNext,
            this.btnLast,
            this.lblInfo});

            // 
            // txbPage
            // 
            this.txbPage.AutoSize = false;
            this.txbPage.Name = "txbPage";
            this.txbPage.Size = new System.Drawing.Size(30, 23);
            // 
            // lblPage
            // 
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(27, 23);
            this.lblPage.Text = "/20";
            // 
            // lblInfo
            // 
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(12, 23);
            this.lblInfo.Text = " ";
            // 
            // btnFirst
            // 
            this.btnFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFirst.Image = global::RIPP.Lib.Properties.Resources.resultset_first;
            this.btnFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(23, 23);
            this.btnFirst.ToolTipText = "首页";
            this.btnFirst.Click += new EventHandler(btnFirst_Click);
            // 
            // btnPre
            // 
            this.btnPre.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPre.Image = global::RIPP.Lib.Properties.Resources.resultset_previous;
            this.btnPre.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPre.Name = "btnPre";
            this.btnPre.Size = new System.Drawing.Size(23, 23);
            this.btnPre.ToolTipText = "上一页";
            this.btnPre.Click += new EventHandler(btnPre_Click);
            // 
            // btnNext
            // 
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.Image = global::RIPP.Lib.Properties.Resources.resultset_next;
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 23);
            this.btnNext.Text = "下页";
            this.btnNext.Click += new EventHandler(btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLast.Image = global::RIPP.Lib.Properties.Resources.resultset_last;
            this.btnLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(23, 23);
            this.btnLast.ToolTipText = "最后一页";
            this.btnLast.Click += new EventHandler(btnLast_Click);
        }

        void btnLast_Click(object sender, EventArgs e)
        {
            this.PageIndex = this.TotalPages;
            this.fireChange();
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            if (this.HasNextPage)
            {
                this.PageIndex++;
                this.fireChange();
            }
        }

        void btnPre_Click(object sender, EventArgs e)
        {
            if(this.HasPreviousPage)
            {
                this.PageIndex--;
                this.fireChange();
            }
            
        }

        private void fireChange()
        {
            if (this.PageChange != null)
                this.PageChange(this, null);
        }

        void btnFirst_Click(object sender, EventArgs e)
        {
            this.PageIndex = 1;
            this.fireChange();
        }
    }
}
