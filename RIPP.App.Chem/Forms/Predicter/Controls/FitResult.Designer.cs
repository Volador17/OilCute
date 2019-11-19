namespace RIPP.App.Chem.Forms.Predicter.Controls
{
    partial class FitResult
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
            this.treeGridView1 = new RIPP.Lib.UI.Expander.TreeGridView();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).BeginInit();
            this.SuspendLayout();
            this.treeGridView1.AllowUserToAddRows = false;
            this.treeGridView1.AllowUserToDeleteRows = false;
            this.treeGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.treeGridView1.ImageList = null;
            this.treeGridView1.Location = new System.Drawing.Point(0, 42);
            this.treeGridView1.Name = "treeGridView1";
            this.treeGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.treeGridView1.ShowLines = false;
            this.treeGridView1.Size = new System.Drawing.Size(699, 308);
            this.treeGridView1.TabIndex = 1;
            this.ResumeLayout(false);

            this.Controls.Add(this.treeGridView1);

        }

        #endregion

        private RIPP.Lib.UI.Expander.TreeGridView treeGridView1;
    }
}
