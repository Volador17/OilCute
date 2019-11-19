namespace RIPP.App.AnalCenter.Forms.Dlg
{
    partial class FrmPropertyEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pNIR = new RIPP.App.AnalCenter.Forms.Ctrl.PropertyEdit();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pShiNao = new RIPP.App.AnalCenter.Forms.Ctrl.PropertyEdit();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.pPenQi = new RIPP.App.AnalCenter.Forms.Ctrl.PropertyEdit();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.pChaiYou = new RIPP.App.AnalCenter.Forms.Ctrl.PropertyEdit();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.pLaYou = new RIPP.App.AnalCenter.Forms.Ctrl.PropertyEdit();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.pZhaYou = new RIPP.App.AnalCenter.Forms.Ctrl.PropertyEdit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnOk, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(642, 434);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(336, 392);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 39);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOk.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.Location = new System.Drawing.Point(231, 392);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 39);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tabControl1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabControl1, 2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(636, 383);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pNIR);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(628, 357);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "基本性质";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pNIR
            // 
            this.pNIR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pNIR.Location = new System.Drawing.Point(3, 3);
            this.pNIR.Name = "pNIR";
            this.pNIR.Size = new System.Drawing.Size(622, 351);
            this.pNIR.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pShiNao);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(628, 357);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "石脑油";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pShiNao
            // 
            this.pShiNao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pShiNao.Location = new System.Drawing.Point(3, 3);
            this.pShiNao.Name = "pShiNao";
            this.pShiNao.Size = new System.Drawing.Size(622, 351);
            this.pShiNao.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.pPenQi);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(628, 357);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "喷气燃料";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // pPenQi
            // 
            this.pPenQi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pPenQi.Location = new System.Drawing.Point(0, 0);
            this.pPenQi.Name = "pPenQi";
            this.pPenQi.Size = new System.Drawing.Size(628, 357);
            this.pPenQi.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.pChaiYou);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(628, 357);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "柴油";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // pChaiYou
            // 
            this.pChaiYou.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pChaiYou.Location = new System.Drawing.Point(0, 0);
            this.pChaiYou.Name = "pChaiYou";
            this.pChaiYou.Size = new System.Drawing.Size(628, 357);
            this.pChaiYou.TabIndex = 1;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.pLaYou);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(628, 357);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "蜡油";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // pLaYou
            // 
            this.pLaYou.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pLaYou.Location = new System.Drawing.Point(0, 0);
            this.pLaYou.Name = "pLaYou";
            this.pLaYou.Size = new System.Drawing.Size(628, 357);
            this.pLaYou.TabIndex = 1;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.pZhaYou);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(628, 357);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "渣油";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // pZhaYou
            // 
            this.pZhaYou.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pZhaYou.Location = new System.Drawing.Point(0, 0);
            this.pZhaYou.Name = "pZhaYou";
            this.pZhaYou.Size = new System.Drawing.Size(628, 357);
            this.pZhaYou.TabIndex = 1;
            // 
            // FrmPropertyEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 434);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmPropertyEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配置原油性质项";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private Ctrl.PropertyEdit pNIR;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private Ctrl.PropertyEdit pShiNao;
        private Ctrl.PropertyEdit pPenQi;
        private Ctrl.PropertyEdit pChaiYou;
        private Ctrl.PropertyEdit pLaYou;
        private Ctrl.PropertyEdit pZhaYou;
    }
}