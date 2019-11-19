using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public partial class IGridOilView<TOilInfo, TOilData>
    {
        private ToolStripMenuItem menuColumnAddOnLeft;
        private ToolStripMenuItem menuColumnAddOnRight;
        private ToolStripMenuItem menuColumnDelete;
        private System.ComponentModel.IContainer components;


        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuCellCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellEmpty = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellLabToCalc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuCellUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuColumnSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuColumnAddOnLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.menuColumnAddOnRight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuColumnDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuColumnSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuColumnSelectedLabShow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuColumnSelectedLabHide = new System.Windows.Forms.ToolStripMenuItem();
            this.menuColumnAllLabShow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuColumnAllLabHide = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellClearLinkTip = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCellCut,
            this.menuCellCopy,
            this.menuCellPaste,
            this.menuCellEmpty,
            this.menuCellLabToCalc,
            this.menuCellClearLinkTip,
            this.menuCellSave,
            this.menuCellSeparator,
            this.menuCellUndo,
            this.menuCellRedo,
            this.menuColumnSeparator,
            this.menuColumnAddOnLeft,
            this.menuColumnAddOnRight,
            this.menuColumnDelete,
            this.menuColumnSeparator2,
            this.menuColumnSelectedLabShow,
            this.menuColumnSelectedLabHide,
            this.menuColumnAllLabShow,
            this.menuColumnAllLabHide});
            this.contextMenu.Name = "contextMenuColumn";
            this.contextMenu.Size = new System.Drawing.Size(178, 374);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuColumn_Opening);
            // 
            // menuCellCut
            // 
            this.menuCellCut.Name = "menuCellCut";
            this.menuCellCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuCellCut.Size = new System.Drawing.Size(177, 22);
            this.menuCellCut.Text = "剪切(&T)";
            this.menuCellCut.Click += new System.EventHandler(this.menuCellCut_Click);
            // 
            // menuCellCopy
            // 
            this.menuCellCopy.Name = "menuCellCopy";
            this.menuCellCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuCellCopy.Size = new System.Drawing.Size(177, 22);
            this.menuCellCopy.Text = "复制(&C)";
            this.menuCellCopy.Click += new System.EventHandler(this.menuCellCopy_Click);
            // 
            // menuCellPaste
            // 
            this.menuCellPaste.Name = "menuCellPaste";
            this.menuCellPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.menuCellPaste.Size = new System.Drawing.Size(177, 22);
            this.menuCellPaste.Text = "粘贴(&P)";
            this.menuCellPaste.Click += new System.EventHandler(this.menuCellPaste_Click);
            // 
            // menuCellEmpty
            // 
            this.menuCellEmpty.Name = "menuCellEmpty";
            this.menuCellEmpty.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.menuCellEmpty.Size = new System.Drawing.Size(177, 22);
            this.menuCellEmpty.Text = "清空(&C)";
            this.menuCellEmpty.Click += new System.EventHandler(this.menuCellEmpty_Click);
            // 
            // menuCellLabToCalc
            // 
            this.menuCellLabToCalc.Name = "menuCellLabToCalc";
            this.menuCellLabToCalc.Size = new System.Drawing.Size(177, 22);
            this.menuCellLabToCalc.Text = "实测->校正";
            this.menuCellLabToCalc.Click += new System.EventHandler(this.menuCellLabToCalc_Click);
            // 
            // menuCellSave
            // 
            this.menuCellSave.Name = "menuCellSave";
            this.menuCellSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuCellSave.Size = new System.Drawing.Size(177, 22);
            this.menuCellSave.Text = "保存(&S)";
            this.menuCellSave.Click += new System.EventHandler(this.menuCellSave_Click);
            // 
            // menuCellSeparator
            // 
            this.menuCellSeparator.Name = "menuCellSeparator";
            this.menuCellSeparator.Size = new System.Drawing.Size(174, 6);
            // 
            // menuCellUndo
            // 
            this.menuCellUndo.Name = "menuCellUndo";
            this.menuCellUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.menuCellUndo.Size = new System.Drawing.Size(177, 22);
            this.menuCellUndo.Text = "撤销(&U)";
            this.menuCellUndo.Click += new System.EventHandler(this.menuCellUndo_Click);
            // 
            // menuCellRedo
            // 
            this.menuCellRedo.Name = "menuCellRedo";
            this.menuCellRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.menuCellRedo.Size = new System.Drawing.Size(177, 22);
            this.menuCellRedo.Text = "重做(&R)";
            this.menuCellRedo.Click += new System.EventHandler(this.menuCellRedo_Click);
            // 
            // menuColumnSeparator
            // 
            this.menuColumnSeparator.Name = "menuColumnSeparator";
            this.menuColumnSeparator.Size = new System.Drawing.Size(174, 6);
            // 
            // menuColumnAddOnLeft
            // 
            this.menuColumnAddOnLeft.Name = "menuColumnAddOnLeft";
            this.menuColumnAddOnLeft.Size = new System.Drawing.Size(177, 22);
            this.menuColumnAddOnLeft.Text = "在左侧添加(&L)";
            this.menuColumnAddOnLeft.Click += new System.EventHandler(this.menuColumnAdd_Click);
            // 
            // menuColumnAddOnRight
            // 
            this.menuColumnAddOnRight.Name = "menuColumnAddOnRight";
            this.menuColumnAddOnRight.Size = new System.Drawing.Size(177, 22);
            this.menuColumnAddOnRight.Text = "在右侧添加(&R)";
            this.menuColumnAddOnRight.Click += new System.EventHandler(this.menuColumnAdd_Click);
            // 
            // menuColumnDelete
            // 
            this.menuColumnDelete.Name = "menuColumnDelete";
            this.menuColumnDelete.Size = new System.Drawing.Size(177, 22);
            this.menuColumnDelete.Text = "删除已选列(&D)";
            this.menuColumnDelete.Click += new System.EventHandler(this.menuColumnDelete_Click);
            // 
            // menuColumnSeparator2
            // 
            this.menuColumnSeparator2.Name = "menuColumnSeparator2";
            this.menuColumnSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // menuColumnSelectedLabShow
            // 
            this.menuColumnSelectedLabShow.Name = "menuColumnSelectedLabShow";
            this.menuColumnSelectedLabShow.Size = new System.Drawing.Size(177, 22);
            this.menuColumnSelectedLabShow.Text = "显示已选实测列(&S)";
            this.menuColumnSelectedLabShow.Click += new System.EventHandler(this.menuColumnLabShowHide_Click);
            // 
            // menuColumnSelectedLabHide
            // 
            this.menuColumnSelectedLabHide.Name = "menuColumnSelectedLabHide";
            this.menuColumnSelectedLabHide.Size = new System.Drawing.Size(177, 22);
            this.menuColumnSelectedLabHide.Text = "隐藏已选实测列(&H)";
            this.menuColumnSelectedLabHide.Click += new System.EventHandler(this.menuColumnLabShowHide_Click);
            // 
            // menuColumnAllLabShow
            // 
            this.menuColumnAllLabShow.Name = "menuColumnAllLabShow";
            this.menuColumnAllLabShow.Size = new System.Drawing.Size(177, 22);
            this.menuColumnAllLabShow.Text = "显示全部实测列(&D)";
            this.menuColumnAllLabShow.Click += new System.EventHandler(this.menuColumnLabShowHide_Click);
            // 
            // menuColumnAllLabHide
            // 
            this.menuColumnAllLabHide.Name = "menuColumnAllLabHide";
            this.menuColumnAllLabHide.Size = new System.Drawing.Size(177, 22);
            this.menuColumnAllLabHide.Text = "隐藏全部实测列(&E)";
            this.menuColumnAllLabHide.Click += new System.EventHandler(this.menuColumnLabShowHide_Click);
            // 
            // menuCellClearLinkTip
            // 
            this.menuCellClearLinkTip.Name = "menuCellClearLinkTip";
            this.menuCellClearLinkTip.Size = new System.Drawing.Size(177, 22);
            this.menuCellClearLinkTip.Text = "取消关联提示";
            this.menuCellClearLinkTip.Click += new System.EventHandler(this.menuCellClearLinkTip_Click);
            // 
            // IGridOilView
            // 
            this.RowTemplate.Height = 23;
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        private ToolStripMenuItem menuCellEmpty;
        private ToolStripSeparator menuColumnSeparator;
        private ToolStripMenuItem menuCellCopy;
        private ToolStripMenuItem menuCellPaste;
        private ToolStripMenuItem menuCellCut;
        private ToolStripMenuItem menuCellUndo;
        private ToolStripMenuItem menuCellRedo;
        private ToolStripSeparator menuCellSeparator;
        private ToolStripMenuItem menuColumnSelectedLabShow;
        private ToolStripMenuItem menuColumnSelectedLabHide;
        private ToolStripSeparator menuColumnSeparator2;
        private ToolStripMenuItem menuColumnAllLabShow;
        private ToolStripMenuItem menuColumnAllLabHide;
        private ToolStripMenuItem menuCellSave;
        protected ContextMenuStrip contextMenu;
        protected ToolStripMenuItem menuCellLabToCalc;
        protected ToolStripMenuItem menuCellClearLinkTip;

    }
}
