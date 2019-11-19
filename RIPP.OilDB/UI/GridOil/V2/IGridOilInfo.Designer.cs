using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2.Model;
namespace RIPP.OilDB.UI.GridOil.V2
{
    partial class IGridOilInfo<TOilInfo>
        where TOilInfo : class, IOilInfoEntity, new()
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cCutMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.cCopyMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.cPasteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.cDeleteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.cSaveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.cMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // cMenu
            // 
            this.cMenu.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cCutMenu,
            this.cCopyMenu,
            this.cPasteMenu,
            this.cDeleteMenu,
            this.cSaveMenu});
            this.cMenu.Name = "cMenu";
            this.cMenu.Size = new System.Drawing.Size(146, 114);
            // 
            // cCutMenu
            // 
            this.cCutMenu.Name = "cCutMenu";
            this.cCutMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cCutMenu.Size = new System.Drawing.Size(145, 22);
            this.cCutMenu.Text = "剪切";
            this.cCutMenu.Click += new System.EventHandler(this.cCutMenu_Click);
            // 
            // cCopyMenu
            // 
            this.cCopyMenu.Name = "cCopyMenu";
            this.cCopyMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.cCopyMenu.Size = new System.Drawing.Size(145, 22);
            this.cCopyMenu.Text = "复制";
            this.cCopyMenu.Click += new System.EventHandler(this.cCopyMenu_Click);
            // 
            // cPasteMenu
            // 
            this.cPasteMenu.Name = "cPasteMenu";
            this.cPasteMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.cPasteMenu.Size = new System.Drawing.Size(145, 22);
            this.cPasteMenu.Text = "粘贴";
            this.cPasteMenu.Click += new System.EventHandler(this.cPasteMenu_Click);
            // 
            // cDeleteMenu
            // 
            this.cDeleteMenu.Name = "cDeleteMenu";
            this.cDeleteMenu.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.cDeleteMenu.Size = new System.Drawing.Size(145, 22);
            this.cDeleteMenu.Text = "删除";
            this.cDeleteMenu.Click += new System.EventHandler(this.cDeleteMenu_Click);
            // 
            // cSaveMenu
            // 
            this.cSaveMenu.Name = "cSaveMenu";
            this.cSaveMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.cSaveMenu.Size = new System.Drawing.Size(145, 22);
            this.cSaveMenu.Text = "保存";
            this.cSaveMenu.Click += new System.EventHandler(this.cSaveMenu_Click);
            // 
            // IGridOilInfo
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ContextMenuStrip = this.cMenu;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.RowTemplate.Height = 23;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GridOilInfo_KeyDown);
            this.cMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cMenu;
        private System.Windows.Forms.ToolStripMenuItem cCopyMenu;
        private System.Windows.Forms.ToolStripMenuItem cCutMenu;
        private System.Windows.Forms.ToolStripMenuItem cPasteMenu;
        private System.Windows.Forms.ToolStripMenuItem cDeleteMenu;
        private System.Windows.Forms.ToolStripMenuItem cSaveMenu;
    }
}
