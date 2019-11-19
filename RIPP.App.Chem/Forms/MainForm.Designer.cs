namespace RIPP.App.Chem.Forms
{
    partial class MainForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuSpec = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSpecNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSpecOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuModel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuModelNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuModelOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuId = new System.Windows.Forms.ToolStripMenuItem();
            this.menuIdNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuIdOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuF = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuB = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBB = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBI = new System.Windows.Forms.ToolStripMenuItem();
            this.menuP = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMix = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMixS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMixP = new System.Windows.Forms.ToolStripMenuItem();
            this.menuM = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMI = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMF = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMP = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arrangeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuC = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCRole = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCU = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCR = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUserinfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowsMenu,
            this.menuSpec,
            this.menuModel,
            this.menuId,
            this.menuF,
            this.menuB,
            this.menuP,
            this.menuMix,
            this.menuM,
            this.helpMenu,
            this.menuC,
            this.btnUserinfo});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.MdiWindowListItem = this.windowsMenu;
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(963, 25);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "MenuStrip";
            // 
            // menuSpec
            // 
            this.menuSpec.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSpecNew,
            this.menuSpecOpen,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
            this.menuSpec.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.menuSpec.Name = "menuSpec";
            this.menuSpec.Size = new System.Drawing.Size(71, 21);
            this.menuSpec.Text = "光谱库(&S)";
            this.menuSpec.Visible = false;
            // 
            // menuSpecNew
            // 
            this.menuSpecNew.Image = ((System.Drawing.Image)(resources.GetObject("menuSpecNew.Image")));
            this.menuSpecNew.ImageTransparentColor = System.Drawing.Color.Black;
            this.menuSpecNew.Name = "menuSpecNew";
            this.menuSpecNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuSpecNew.Size = new System.Drawing.Size(165, 22);
            this.menuSpecNew.Text = "新建(&N)";
            this.menuSpecNew.Click += new System.EventHandler(this.menuSpecNew_Click);
            // 
            // menuSpecOpen
            // 
            this.menuSpecOpen.Image = ((System.Drawing.Image)(resources.GetObject("menuSpecOpen.Image")));
            this.menuSpecOpen.ImageTransparentColor = System.Drawing.Color.Black;
            this.menuSpecOpen.Name = "menuSpecOpen";
            this.menuSpecOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuSpecOpen.Size = new System.Drawing.Size(165, 22);
            this.menuSpecOpen.Text = "打开(&O)";
            this.menuSpecOpen.Click += new System.EventHandler(this.menuSpecOpen_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(162, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.exitToolStripMenuItem.Text = "退出(&X)";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolsStripMenuItem_Click);
            // 
            // menuModel
            // 
            this.menuModel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuModelNew,
            this.menuModelOpen});
            this.menuModel.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.menuModel.Name = "menuModel";
            this.menuModel.Size = new System.Drawing.Size(76, 21);
            this.menuModel.Text = "模型库(&M)";
            this.menuModel.Visible = false;
            // 
            // menuModelNew
            // 
            this.menuModelNew.Image = ((System.Drawing.Image)(resources.GetObject("menuModelNew.Image")));
            this.menuModelNew.ImageTransparentColor = System.Drawing.Color.Black;
            this.menuModelNew.Name = "menuModelNew";
            this.menuModelNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuModelNew.Size = new System.Drawing.Size(165, 22);
            this.menuModelNew.Text = "新建(&N)";
            this.menuModelNew.Click += new System.EventHandler(this.menuModelNew_Click);
            // 
            // menuModelOpen
            // 
            this.menuModelOpen.Image = ((System.Drawing.Image)(resources.GetObject("menuModelOpen.Image")));
            this.menuModelOpen.ImageTransparentColor = System.Drawing.Color.Black;
            this.menuModelOpen.Name = "menuModelOpen";
            this.menuModelOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuModelOpen.Size = new System.Drawing.Size(165, 22);
            this.menuModelOpen.Text = "打开(&O)";
            this.menuModelOpen.Click += new System.EventHandler(this.menuModelOpen_Click);
            // 
            // menuId
            // 
            this.menuId.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuIdNew,
            this.menuIdOpen});
            this.menuId.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.menuId.Name = "menuId";
            this.menuId.Size = new System.Drawing.Size(68, 21);
            this.menuId.Text = "识别库(&I)";
            this.menuId.Visible = false;
            // 
            // menuIdNew
            // 
            this.menuIdNew.Image = ((System.Drawing.Image)(resources.GetObject("menuIdNew.Image")));
            this.menuIdNew.ImageTransparentColor = System.Drawing.Color.Black;
            this.menuIdNew.Name = "menuIdNew";
            this.menuIdNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuIdNew.Size = new System.Drawing.Size(165, 22);
            this.menuIdNew.Text = "新建(&N)";
            this.menuIdNew.Click += new System.EventHandler(this.menuIdNew_Click);
            // 
            // menuIdOpen
            // 
            this.menuIdOpen.Image = ((System.Drawing.Image)(resources.GetObject("menuIdOpen.Image")));
            this.menuIdOpen.ImageTransparentColor = System.Drawing.Color.Black;
            this.menuIdOpen.Name = "menuIdOpen";
            this.menuIdOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuIdOpen.Size = new System.Drawing.Size(165, 22);
            this.menuIdOpen.Text = "打开(&O)";
            this.menuIdOpen.Click += new System.EventHandler(this.menuIdOpen_Click);
            // 
            // menuF
            // 
            this.menuF.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFNew,
            this.menuFOpen});
            this.menuF.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.menuF.Name = "menuF";
            this.menuF.Size = new System.Drawing.Size(70, 21);
            this.menuF.Text = "拟合库(&F)";
            this.menuF.Visible = false;
            // 
            // menuFNew
            // 
            this.menuFNew.Image = ((System.Drawing.Image)(resources.GetObject("menuFNew.Image")));
            this.menuFNew.ImageTransparentColor = System.Drawing.Color.Black;
            this.menuFNew.Name = "menuFNew";
            this.menuFNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuFNew.Size = new System.Drawing.Size(165, 22);
            this.menuFNew.Text = "新建(&N)";
            this.menuFNew.Click += new System.EventHandler(this.menuINew_Click);
            // 
            // menuFOpen
            // 
            this.menuFOpen.Image = ((System.Drawing.Image)(resources.GetObject("menuFOpen.Image")));
            this.menuFOpen.ImageTransparentColor = System.Drawing.Color.Black;
            this.menuFOpen.Name = "menuFOpen";
            this.menuFOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuFOpen.Size = new System.Drawing.Size(165, 22);
            this.menuFOpen.Text = "打开(&O)";
            this.menuFOpen.Click += new System.EventHandler(this.menuIOpen_Click);
            // 
            // menuB
            // 
            this.menuB.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBB,
            this.menuBI});
            this.menuB.Name = "menuB";
            this.menuB.Size = new System.Drawing.Size(60, 21);
            this.menuB.Text = "打包(&B)";
            this.menuB.Visible = false;
            // 
            // menuBB
            // 
            this.menuBB.Name = "menuBB";
            this.menuBB.Size = new System.Drawing.Size(136, 22);
            this.menuBB.Text = "方法打包";
            this.menuBB.Click += new System.EventHandler(this.menuB_Click);
            // 
            // menuBI
            // 
            this.menuBI.Name = "menuBI";
            this.menuBI.Size = new System.Drawing.Size(136, 22);
            this.menuBI.Text = "集成包打包";
            this.menuBI.Click += new System.EventHandler(this.bToolStripMenuItem_Click);
            // 
            // menuP
            // 
            this.menuP.Name = "menuP";
            this.menuP.Size = new System.Drawing.Size(59, 21);
            this.menuP.Text = "预测(&P)";
            this.menuP.Visible = false;
            this.menuP.Click += new System.EventHandler(this.menuP_Click);
            // 
            // menuMix
            // 
            this.menuMix.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMixS,
            this.menuMixP});
            this.menuMix.Name = "menuMix";
            this.menuMix.Size = new System.Drawing.Size(68, 21);
            this.menuMix.Text = "混兑比例";
            this.menuMix.Visible = false;
            // 
            // menuMixS
            // 
            this.menuMixS.Name = "menuMixS";
            this.menuMixS.Size = new System.Drawing.Size(124, 22);
            this.menuMixS.Text = "混兑光谱";
            this.menuMixS.Click += new System.EventHandler(this.menuMixS_Click);
            // 
            // menuMixP
            // 
            this.menuMixP.Name = "menuMixP";
            this.menuMixP.Size = new System.Drawing.Size(124, 22);
            this.menuMixP.Text = "混兑预测";
            this.menuMixP.Click += new System.EventHandler(this.menuMixP_Click);
            // 
            // menuM
            // 
            this.menuM.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMI,
            this.menuMF,
            this.menuMP});
            this.menuM.Name = "menuM";
            this.menuM.Size = new System.Drawing.Size(88, 21);
            this.menuM.Text = "方法维护(&M)";
            this.menuM.Visible = false;
            // 
            // menuMI
            // 
            this.menuMI.Name = "menuMI";
            this.menuMI.Size = new System.Drawing.Size(175, 22);
            this.menuMI.Text = "识别库(&I)";
            this.menuMI.Click += new System.EventHandler(this.menuMI_Click);
            // 
            // menuMF
            // 
            this.menuMF.Name = "menuMF";
            this.menuMF.Size = new System.Drawing.Size(175, 22);
            this.menuMF.Text = "拟合库(&F)";
            this.menuMF.Click += new System.EventHandler(this.menuMF_Click);
            // 
            // menuMP
            // 
            this.menuMP.Name = "menuMP";
            this.menuMP.Size = new System.Drawing.Size(175, 22);
            this.menuMP.Text = "PLS1 PLS-ANN(&P)";
            this.menuMP.Click += new System.EventHandler(this.menuMP_Click);
            // 
            // windowsMenu
            // 
            this.windowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newWindowToolStripMenuItem,
            this.cascadeToolStripMenuItem,
            this.tileVerticalToolStripMenuItem,
            this.tileHorizontalToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.arrangeIconsToolStripMenuItem});
            this.windowsMenu.Name = "windowsMenu";
            this.windowsMenu.Size = new System.Drawing.Size(64, 21);
            this.windowsMenu.Text = "窗口(&W)";
            // 
            // newWindowToolStripMenuItem
            // 
            this.newWindowToolStripMenuItem.Name = "newWindowToolStripMenuItem";
            this.newWindowToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newWindowToolStripMenuItem.Text = "新建窗口(&N)";
            // 
            // cascadeToolStripMenuItem
            // 
            this.cascadeToolStripMenuItem.Name = "cascadeToolStripMenuItem";
            this.cascadeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cascadeToolStripMenuItem.Text = "层叠(&C)";
            this.cascadeToolStripMenuItem.Click += new System.EventHandler(this.CascadeToolStripMenuItem_Click);
            // 
            // tileVerticalToolStripMenuItem
            // 
            this.tileVerticalToolStripMenuItem.Name = "tileVerticalToolStripMenuItem";
            this.tileVerticalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tileVerticalToolStripMenuItem.Text = "垂直平铺(&V)";
            this.tileVerticalToolStripMenuItem.Click += new System.EventHandler(this.TileVerticalToolStripMenuItem_Click);
            // 
            // tileHorizontalToolStripMenuItem
            // 
            this.tileHorizontalToolStripMenuItem.Name = "tileHorizontalToolStripMenuItem";
            this.tileHorizontalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tileHorizontalToolStripMenuItem.Text = "水平平铺(&H)";
            this.tileHorizontalToolStripMenuItem.Click += new System.EventHandler(this.TileHorizontalToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeAllToolStripMenuItem.Text = "全部关闭(&L)";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItem_Click);
            // 
            // arrangeIconsToolStripMenuItem
            // 
            this.arrangeIconsToolStripMenuItem.Name = "arrangeIconsToolStripMenuItem";
            this.arrangeIconsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.arrangeIconsToolStripMenuItem.Text = "排列图标(&A)";
            this.arrangeIconsToolStripMenuItem.Click += new System.EventHandler(this.ArrangeIconsToolStripMenuItem_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator8,
            this.aboutToolStripMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(61, 21);
            this.helpMenu.Text = "帮助(&H)";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.contentsToolStripMenuItem.Text = "目录(&C)";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("indexToolStripMenuItem.Image")));
            this.indexToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.indexToolStripMenuItem.Text = "索引(&I)";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("searchToolStripMenuItem.Image")));
            this.searchToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.searchToolStripMenuItem.Text = "搜索(&S)";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(163, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.aboutToolStripMenuItem.Text = "关于(&A) ... ...";
            // 
            // menuC
            // 
            this.menuC.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCS,
            this.menuCRole,
            this.menuCU,
            this.menuCR});
            this.menuC.Name = "menuC";
            this.menuC.Size = new System.Drawing.Size(60, 21);
            this.menuC.Text = "配置(&C)";
            // 
            // menuCS
            // 
            this.menuCS.Name = "menuCS";
            this.menuCS.Size = new System.Drawing.Size(141, 22);
            this.menuCS.Text = "系统设置(&S)";
            this.menuCS.Click += new System.EventHandler(this.menuCS_Click);
            // 
            // menuCRole
            // 
            this.menuCRole.Name = "menuCRole";
            this.menuCRole.Size = new System.Drawing.Size(141, 22);
            this.menuCRole.Text = "权限管理";
            this.menuCRole.Click += new System.EventHandler(this.menuCRole_Click);
            // 
            // menuCU
            // 
            this.menuCU.Name = "menuCU";
            this.menuCU.Size = new System.Drawing.Size(141, 22);
            this.menuCU.Text = "用户管理(&U)";
            this.menuCU.Click += new System.EventHandler(this.menuCU_Click);
            // 
            // menuCR
            // 
            this.menuCR.Name = "menuCR";
            this.menuCR.Size = new System.Drawing.Size(141, 22);
            this.menuCR.Text = "软件注册(&R)";
            this.menuCR.Click += new System.EventHandler(this.menuCR_Click);
            // 
            // btnUserinfo
            // 
            this.btnUserinfo.Name = "btnUserinfo";
            this.btnUserinfo.Size = new System.Drawing.Size(68, 21);
            this.btnUserinfo.Text = "个人资料";
            this.btnUserinfo.ToolTipText = "点击修改个人资料";
            this.btnUserinfo.Click += new System.EventHandler(this.btnUserinfo_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(963, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 396);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(963, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 418);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RIPP化学计量学软件";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MdiChildActivate += new System.EventHandler(this.MainForm_MdiChildActivate);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileHorizontalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuSpec;
        private System.Windows.Forms.ToolStripMenuItem menuSpecNew;
        private System.Windows.Forms.ToolStripMenuItem menuSpecOpen;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsMenu;
        private System.Windows.Forms.ToolStripMenuItem newWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cascadeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileVerticalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arrangeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem menuModel;
        private System.Windows.Forms.ToolStripMenuItem menuModelNew;
        private System.Windows.Forms.ToolStripMenuItem menuModelOpen;
        private System.Windows.Forms.ToolStripMenuItem menuId;
        private System.Windows.Forms.ToolStripMenuItem menuIdNew;
        private System.Windows.Forms.ToolStripMenuItem menuIdOpen;
        private System.Windows.Forms.ToolStripMenuItem menuF;
        private System.Windows.Forms.ToolStripMenuItem menuFNew;
        private System.Windows.Forms.ToolStripMenuItem menuFOpen;
        private System.Windows.Forms.ToolStripMenuItem menuB;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuP;
        private System.Windows.Forms.ToolStripMenuItem menuMix;
        private System.Windows.Forms.ToolStripMenuItem menuM;
        private System.Windows.Forms.ToolStripMenuItem menuMI;
        private System.Windows.Forms.ToolStripMenuItem menuMF;
        private System.Windows.Forms.ToolStripMenuItem menuC;
        private System.Windows.Forms.ToolStripMenuItem menuCS;
        private System.Windows.Forms.ToolStripMenuItem menuCU;
        private System.Windows.Forms.ToolStripMenuItem menuCR;
        private System.Windows.Forms.ToolStripMenuItem menuCRole;
        private System.Windows.Forms.ToolStripMenuItem menuBB;
        private System.Windows.Forms.ToolStripMenuItem menuBI;
        private System.Windows.Forms.ToolStripMenuItem menuMP;
        private System.Windows.Forms.ToolStripMenuItem btnUserinfo;
        private System.Windows.Forms.ToolStripMenuItem menuMixS;
        private System.Windows.Forms.ToolStripMenuItem menuMixP;
    }
}


