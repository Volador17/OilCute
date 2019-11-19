using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using RIPP.Lib;
using System.Drawing;
using log4net;

namespace RIPP.NIR.Controls
{
    public class SpecGridView : DataGridView, RIPP.Lib.UI.Controls.IFlowNode
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
        #region 私有成员

        /// <summary>
        /// 光谱库基类
        /// </summary>
        private SpecBase _specList = new SpecBase();

       
        /// <summary>
        /// 表格是否初始化
        /// </summary>
        private bool _isInited = false;

        /// <summary>
        /// 是否被编辑过
        /// </summary>
        private bool _edited = false;

        /// <summary>
        /// 是否允许被编辑
        /// </summary>
        private bool _editEnbale = false;


        private bool _isShowComponent = false;

        /// <summary>
        /// 用于只显示一个性质
        /// </summary>
        private Component _SingleComponent;

        private int _dcolNum = 20;
        private int _drowNum = 30;


        #endregion

        #region 公有成员
        /// <summary>
        /// 选择的光谱发生变化
        /// </summary>
        public event EventHandler<SpecRowSelectedArgus> SelectChanged;

        /// <summary>
        /// 光谱列表发生变化
        /// </summary>
        public event EventHandler SpecListChanged;

        public event EventHandler EditedChanged;


        /// <summary>
        /// 所有光谱库
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpecBase Specs
        {
            get { return this._specList; }
            set
            {
                if (value != this._specList)
                {
                    this._specList = value;
                    this.Render();
                }
            }
        }

        /// <summary>
        /// 是否被编辑过
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Edited
        {
            get { return this._edited; }
            set { this._edited = value; }
        }
        /// <summary>
        /// 是否允许被编辑
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditEnable
        {
            get { return this._editEnbale; }
            set { this._editEnbale = value; }
        }

          [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ExportEnable { set; get; }

        /// <summary>
        /// 是否显示性质值
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsShowComponent
        {
            get { return this._isShowComponent; }
            set { 
             
                this._isShowComponent = value;
            }
        }
        /// <summary>
        /// 用于只显示一个性质
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Component SingleComponent
        {
            get { return this._SingleComponent; }
            set
            {
                if (this._SingleComponent != value)
                {
                    this._SingleComponent = value;
                    this.Render();
                }
            }
        }

        #endregion

        public SpecGridView()
        {
            this._editEnbale = false;
            this._isShowComponent = true;
        }

        public SpecGridView(bool editEnable = false, bool isShowComponent = false)
        {
            this._editEnbale = editEnable;
            this._isShowComponent = isShowComponent;
        }

        protected override void Dispose(bool disposing)
        {
            if (this._specList != null)
                this._specList.Dispose();
            
            base.Dispose(disposing);
        }


        #region Init
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIdx;
        // private System.Windows.Forms.DataGridViewImageColumn colColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescrip;
        private DataGridViewTextBoxColumn colOilName;
        private DlgComponent dlgComp = new DlgComponent();
        private void _init()
        {
            this.RowHeadersWidth = 30;
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;

            dlgComp.EventSave += new EventHandler<DlgComponentEventArgs>(dlgComp_EventSave);

            this.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(SpecGridView_ColumnHeaderMouseClick);
            this.RowHeaderMouseClick += new DataGridViewCellMouseEventHandler(SpecGridView_RowHeaderMouseClick);
         this.SortCompare+=new DataGridViewSortCompareEventHandler(SpecGridView_SortCompare);
            

            this.CellEndEdit += new DataGridViewCellEventHandler(SpecGridView_CellEndEdit);
            this.CellBeginEdit += new DataGridViewCellCancelEventHandler(SpecGridView_CellBeginEdit);

            this.SelectionChanged += new EventHandler(SpecGridView_SelectionChanged);

            this.KeyDown += new KeyEventHandler(SpecGridView_KeyDown);


            this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            this.AllowUserToAddRows = false;

            //  this.colColor = new System.Windows.Forms.DataGridViewImageColumn();
            this.colDescrip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOilName = new DataGridViewTextBoxColumn();
            this.colIdx = new DataGridViewTextBoxColumn();

            // colOilName

            this.colOilName.HeaderText = "样本名称";
            this.colOilName.Name = "colOilName";
            this.colOilName.ToolTipText = "样本名称";
            this.colOilName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.colOilName.Frozen = true;

            this.colIdx.HeaderText = "序号";
            this.colIdx.Name = "colIdx";
            this.colIdx.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colIdx.ToolTipText = "序号";
            this.colIdx.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.colIdx.Frozen = true;
            this.colIdx.Width = 40;
            this.colIdx.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // 
            // colDescrip
            // 
            this.colDescrip.HeaderText = "描述";
            this.colDescrip.MinimumWidth = 60;
            this.colDescrip.Name = "colDescrip";
            this.colDescrip.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDescrip.ToolTipText = "光谱描述信息";
            this.colDescrip.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDescrip.ReadOnly = !this._editEnbale;

            // 
            // colName
            // 
            this.colName.HeaderText = "光谱名称";
            this.colName.MinimumWidth = 80;
            this.colName.Name = "colName";
            this.colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.colName.ReadOnly = true;
            this.colName.Frozen = true;
            // 
            // colType
            // 
            this.colType.HeaderText = "类别";
            this.colType.Name = "colType";
            this.colType.Width = 60;
            this.colType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.colType.ReadOnly = true;
            this.colType.Frozen = true;

          




            this.ContextMenuStrip = new ContextMenuStrip();

            var menuitem1 = new ToolStripMenuItem()
            {
                Text = "全部设置为校正谱图",

                Visible = this._editEnbale
            };
            var menuitem2 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", UsageTypeEnum.Calibrate.GetDescription()),
                Visible = this._editEnbale
            };
            var menuitem3 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", UsageTypeEnum.Validate.GetDescription()),
                Visible = this._editEnbale
            };
            var menuitem4 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", UsageTypeEnum.Guide.GetDescription()),
                Visible = this._editEnbale
            };
            var menuitem5 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", UsageTypeEnum.Ignore.GetDescription()),
                Visible = this._editEnbale
            };
            var menuspec = new ToolStripSeparator() { Visible = this._editEnbale };
            var menuitemCopy = new ToolStripMenuItem()
            {
                Text = "复制所选(&C)"
            };
            var menuitemCopyAll = new ToolStripMenuItem()
            {
                Text = "复制全部(&A)"
            };
            var menuitemPaste = new ToolStripMenuItem()
            {
                Text = "粘贴(&V)",
                Visible = this._editEnbale
            };
            var menuitemDelete = new ToolStripMenuItem()
            {
                Text = "删除(&D)",
                Visible = this._editEnbale
            };
            var menuspec1 = new ToolStripSeparator() { Visible = this.ExportEnable };
            var menuitemExportSpec = new ToolStripMenuItem()
            {
                Text = "导出光谱(&S)",
                Visible = this.ExportEnable
            };
            var menuitemExportSpecSelect = new ToolStripMenuItem()
            {
                Text = "所选光谱(&S)",
                Visible = this.ExportEnable
            };

            var menuitemExportSpecCalibrate = new ToolStripMenuItem()
            {
                Text = string.Format("{0}光谱(&C)", UsageTypeEnum.Calibrate.GetDescription()),
                Tag = UsageTypeEnum.Calibrate,
                Visible = this.ExportEnable

            };
            var menuitemExportSpecValidate = new ToolStripMenuItem()
            {
                Text = string.Format("{0}光谱(&V)", UsageTypeEnum.Validate.GetDescription()),
                Tag = UsageTypeEnum.Validate,
                Visible = this.ExportEnable

            };
            var menuitemExportSpecIgnore = new ToolStripMenuItem()
            {
                Text = string.Format("{0}光谱(&I)", UsageTypeEnum.Ignore.GetDescription()),
                Tag = UsageTypeEnum.Ignore,
                Visible = this.ExportEnable
            };
            var menuitemExportSpecGuard = new ToolStripMenuItem()
            {
                Text = string.Format("{0}光谱(&G)", UsageTypeEnum.Guide.GetDescription()),
                Tag = UsageTypeEnum.Guide,
                Visible = this.ExportEnable
            };
            var menuitemExportSpecAll = new ToolStripMenuItem()
            {
                Text = "所有光谱(&A)",
                Tag = UsageTypeEnum.Node,
                Visible = this.ExportEnable
            };
            menuitemExportSpec.DropDownItems.AddRange(new ToolStripItem[] { menuitemExportSpecSelect, menuitemExportSpecAll, menuitemExportSpecCalibrate, menuitemExportSpecValidate, menuitemExportSpecIgnore, menuitemExportSpecGuard });

            var menuitemExportProperty = new ToolStripMenuItem()
            {
                Text = "导出性质(&P)",
                Visible = this.ExportEnable
            };
            var menuitemExportPropertySelect = new ToolStripMenuItem()
            {
                Text = "所选性质(&S)",
                Visible = this.ExportEnable
            };
            var menuitemExportPropertyCalibrate = new ToolStripMenuItem()
            {
                Text = string.Format("{0}性质(&C)", UsageTypeEnum.Calibrate.GetDescription()),
                Tag = UsageTypeEnum.Calibrate,
                Visible = this.ExportEnable

            };
            var menuitemExportPropertyValidate = new ToolStripMenuItem()
            {
                Text = string.Format("{0}性质(&V)", UsageTypeEnum.Validate.GetDescription()),
                Tag = UsageTypeEnum.Validate,
                Visible = this.ExportEnable
            };
            var menuitemExportPropertyIgnore = new ToolStripMenuItem()
            {
                Text = string.Format("{0}性质(&I)", UsageTypeEnum.Ignore.GetDescription()),
                Tag = UsageTypeEnum.Ignore,
                Visible = this.ExportEnable
            };
            var menuitemExportPropertyGuard = new ToolStripMenuItem()
            {
                Text = string.Format("{0}性质(&G)", UsageTypeEnum.Guide.GetDescription()),
                Tag = UsageTypeEnum.Guide,
                Visible = this.ExportEnable
            };
            var menuitemExportPropertyAll = new ToolStripMenuItem()
            {
                Text = "所有性质(&A)",
                Tag = UsageTypeEnum.Node,
                Visible = this.ExportEnable
            };
            menuitemExportProperty.DropDownItems.AddRange(new ToolStripItem[]{menuitemExportPropertySelect,menuitemExportPropertyAll,
                menuitemExportPropertyCalibrate,menuitemExportPropertyValidate,menuitemExportPropertyIgnore,menuitemExportPropertyGuard

            });

            menuitem1.Click += new EventHandler(menuitem1_Click);
            menuitem2.Click += new EventHandler(menuitem2_Click);
            menuitem3.Click += new EventHandler(menuitem2_Click);
            menuitem4.Click += new EventHandler(menuitem2_Click);
            menuitem5.Click += new EventHandler(menuitem2_Click);
            //复制 粘贴
            menuitemCopy.Click += new EventHandler(this.copyData);
            menuitemCopyAll.Click += new EventHandler(menuitemCopyAll_Click);
            menuitemPaste.Click += new EventHandler(this.pasteData);
            menuitemDelete.Click += new EventHandler(this.delData);
           

            //导出
            menuitemExportSpecSelect.Click += new EventHandler(menuitemExportSpec_Click);
            menuitemExportSpecAll.Click += new EventHandler(menuitemExportSpecAll_Click);
            menuitemExportSpecCalibrate.Click += new EventHandler(menuitemExportSpecAll_Click);
            menuitemExportSpecGuard.Click += new EventHandler(menuitemExportSpecAll_Click);
            menuitemExportSpecIgnore.Click += new EventHandler(menuitemExportSpecAll_Click);
            menuitemExportSpecValidate.Click += new EventHandler(menuitemExportSpecAll_Click);

            menuitemExportPropertySelect.Click += new EventHandler(menuitemExportProperty_Click);
            menuitemExportPropertyAll.Click += new EventHandler(menuitemExportPropertyAll_Click);
            menuitemExportPropertyCalibrate.Click += new EventHandler(menuitemExportPropertyAll_Click);
            menuitemExportPropertyGuard.Click += new EventHandler(menuitemExportPropertyAll_Click);
            menuitemExportPropertyIgnore.Click += new EventHandler(menuitemExportPropertyAll_Click);
            menuitemExportPropertyValidate.Click += new EventHandler(menuitemExportPropertyAll_Click);



            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { menuitem1, menuitem2, menuitem3, menuitem4, menuitem5, menuspec, menuitemCopy, menuitemCopyAll, menuitemPaste, menuitemDelete,menuspec1, menuitemExportSpec, menuitemExportProperty });
            //初始化右键菜单
            this._initColunms();

            this._isInited = true;
        }
       

        private void _initColunms()
        {
            this.Columns.Clear();
            this.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colIdx,
                this.colName,
                this.colType,
                this.colOilName,
                this.colDescrip
            });


            for (int i = 0; i < this._dcolNum; i++)
            {
                this.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = 60,
                    MinimumWidth = 60,
                    SortMode = DataGridViewColumnSortMode.Programmatic
                });
            }
            for (int i = 0; i < this._drowNum; i++)
            {
                var r = new DataGridViewRow();
r.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                r.HeaderCell.Value = (i + 1).ToString();
                this.Rows.Add(r);
            }
            this.renderRowNum();
        }

        #region 右键菜单
        void menuitem2_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null)
                return;

            // step 1、获取选中的光谱
            var rowidxs = this.getSelectRowIdxs();
            if (rowidxs == null || rowidxs.Length == 0)
                return;

            // step 2、获取值
            UsageTypeEnum utype = UsageTypeEnum.Calibrate;

            if (item.Text.Contains(RIPP.NIR.UsageTypeEnum.Guide.GetDescription()))
                utype = UsageTypeEnum.Guide;
            else if (item.Text.Contains(RIPP.NIR.UsageTypeEnum.Ignore.GetDescription()))
                utype = UsageTypeEnum.Ignore;
            else if (item.Text.Contains(RIPP.NIR.UsageTypeEnum.Validate.GetDescription()))
                utype = UsageTypeEnum.Validate;

            this._edited = this._edited || rowidxs.Length > 0;
            // step 3、设置值
            foreach (var r in rowidxs)
                this.changeCell(r, 2, utype);
            if (rowidxs.Length > 0)
                this.fireEditChange();

        }

        void menuitem1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.RowCount; i++)
                this.changeCell(i, 2, UsageTypeEnum.Calibrate);
            this._edited = true;
            this.fireEditChange();
        }

        /// <summary>
        /// 导出所有性质
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuitemExportPropertyAll_Click(object sender, EventArgs e)
        {
            var menu = (ToolStripMenuItem)sender;
            var type = (UsageTypeEnum)menu.Tag;
            int idx;
            string fl = this.openSaveDlg(menu.Text,SpecBase.GetSaveDlgFilter(), out idx);
            if (idx < 0)
                return;
            this._specList.ExportProperty(type, idx, fl);
        }

        /// <summary>
        /// 导出所选性质
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuitemExportProperty_Click(object sender, EventArgs e)
        {
            var rowidxs = this.getSelectRowIdxs();
            if (rowidxs == null || rowidxs.Length == 0)
                return;
            var lib = this._specList.SubLib(rowidxs);
            int idx;
            string fl = this.openSaveDlg("导出所选性质", SpecBase.GetSaveDlgFilter(), out idx);
            if (idx < 0)
                return;
            lib.ExportProperty(UsageTypeEnum.Node, idx, fl);

        }
        /// <summary>
        /// 导出所有光谱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuitemExportSpecAll_Click(object sender, EventArgs e)
        {
            var menu = (ToolStripMenuItem)sender;
            var type = (UsageTypeEnum)menu.Tag;
            int idx;
            string fl = this.openSaveDlg(menu.Text,SpecBase.GetSaveDlgFilter(), out idx);
            if (idx < 0)
                return;

            this._specList.ExportSpecs(type,idx,fl);
        }
        /// <summary>
        /// 导出所选光谱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuitemExportSpec_Click(object sender, EventArgs e)
        {
            var rowidxs = this.getSelectRowIdxs();
            if (rowidxs == null || rowidxs.Length == 0)
                return;
            var lib = this._specList.SubLib(rowidxs);
            int idx;
            string fl = this.openSaveDlg("导出所选光谱", SpecBase.GetSaveDlgFilter(), out idx);
            if (idx < 0)
                return;
            lib.ExportSpecs(UsageTypeEnum.Node,idx,fl);
        }
        /// <summary>
        /// 复制所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuitemCopyAll_Click(object sender, EventArgs e)
        {
            this.ClearSelection();
            this.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
            this.SelectAll();
            this.copyData(null,null);
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        #endregion


        private string openSaveDlg(string title,string filter,out int idx)
        {
            idx=-1;
            SaveFileDialog dlg = new SaveFileDialog()
            {
                Filter = filter,
                Title=title
            };
            if (dlg.ShowDialog() != DialogResult.OK)
                return null;
            idx = dlg.FilterIndex;
            return dlg.FileName;
        }

        void SpecGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        this.copyData(null, null);
                        break;
                    case Keys.V:
                        this.pasteData(null, null);
                        this.fireEditChange();
                        break;
                    default:
                        break;
                }
            }
            else if (e.KeyCode == Keys.Delete)
                this.delData(null, null);
        }
        #endregion

        #region Event

        void SpecGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = this.Rows[e.RowIndex] as SpecDataGridViewRow;

            if (row == null || row.Spec == null || this._editEnbale == false)
            {
                e.Cancel = true;
                return;
            }

            var idx = this._specList.Specs.Select((d, i) => new { d = d, idx = i }).Where(d => d.d.Name == row.Spec.Name).Select(d => d.idx).FirstOrDefault();
            if (idx >= 0)
            {
                var col = this.Columns[e.ColumnIndex] as ComponentColumn;
                if (col != null)
                {
                    var d = (double)this._specList.Y[idx + 1, col.CompIndex + 1];
                    this[e.ColumnIndex, e.RowIndex].Value = double.IsNaN(d) ? "" : d.ToString();
                }
                else if (e.ColumnIndex == 0)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        void SpecGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var col = this.Columns[e.ColumnIndex] as ComponentColumn;
            var row = this.Rows[e.RowIndex] as SpecDataGridViewRow;
            var str = Convert.ToString(this[e.ColumnIndex, e.RowIndex].Value);

            if (col != null)
            {
                if (string.IsNullOrWhiteSpace(str))
                    str = double.NaN.ToString();
                else if (!RIPP.Lib.Tool.IsDouble(str))
                {
                    MessageBox.Show("您输入的数据不是数字类型！");
                    return;
                }
            }
            
            if (!this.changeCell(e.RowIndex, e.ColumnIndex, str))
            {
                MessageBox.Show("修改失败，请重试！");
            }
            else
                this.fireEditChange();
        }






        private int[] getSelectRowIdxs()
        {

            List<int> rowidxs = new List<int>();
            for (int i = 0; i < this.SelectedCells.Count; i++)
            {
                var idx  = this.SelectedCells[i].RowIndex;
                var row = this.Rows[idx] as SpecDataGridViewRow;
                if (row == null)
                    continue;
                if (!rowidxs.Contains(idx))
                    rowidxs.Add(this.SelectedCells[i].RowIndex);
            }

            return rowidxs.ToArray();
        }

        private int[] getSelectColumnsIdxs()
        {
            List<int> colidxs = new List<int>();
            for (int i = 0; i < this.SelectedCells.Count; i++)
            {
                var idx = this.SelectedCells[i].ColumnIndex;
                if (!colidxs.Contains(idx))
                    colidxs.Add(idx);
            }
            return colidxs.ToArray();
        }



        /// <summary>
        /// 选择的行发生变化，通过父控件，选择的光谱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SpecGridView_SelectionChanged(object sender, EventArgs e)
        {
            var specs = new List<Spectrum>();
            if (this.SelectedRows.Count > 0)
                for (int i = 0; i < this.SelectedRows.Count; i++)
                {
                    var row = this.SelectedRows[i] as SpecDataGridViewRow;
                    if (row != null &&this._specList!=null&&this._specList.Specs!=null)
                        specs.Add(this._specList[this.SelectedRows[i].Index]);
                }
            if (this.SelectChanged != null)
                this.SelectChanged(this, new SpecRowSelectedArgus() { Specs = specs });
            
        }


       

        //void SpecGridView_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    if (e.ColumnIndex < 1)
        //        return;
          
        //    if (this.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
        //        this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
        //    if( this.Columns[e.ColumnIndex].SortMode!= DataGridViewColumnSortMode.Automatic)
        //    this.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.Automatic;


        //  //  this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
        //    SortOrder order = this.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection;
        //    switch (order)
        //    {
        //        case System.Windows.Forms.SortOrder.Ascending:
        //            this.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
        //            break;
        //        default:
        //            this.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
        //            break;
        //    }

        //}

        void SpecGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
            {
                this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                this.Rows[e.RowIndex].Selected = true;
                this.Refresh();
            }
        }

        void SpecGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;
            this.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.Programmatic;
            if (this.SelectionMode != DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                this.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                this.Columns[e.ColumnIndex].Selected = true;
               // this.Refresh();
            }

            //this.Sort
            DataGridViewColumn newColumn = this.Columns.GetColumnCount(DataGridViewElementStates.Selected) == 1 ?
            this.SelectedColumns[0] : null;

            DataGridViewColumn oldColumn = this.SortedColumn;
            ListSortDirection direction;

            // If oldColumn is null, then the DataGridView is not currently sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                    this.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            // If no column has been selected, display an error dialog  box.
            if (newColumn == null)
            {
                MessageBox.Show("Select a single column and try again.",
                    "Error: Invalid Selection", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                this.Sort(newColumn, direction);
                newColumn.HeaderCell.SortGlyphDirection =
                    direction == ListSortDirection.Ascending ?
                    SortOrder.Ascending : SortOrder.Descending;
                this.renderRowNum();
            }

        }

        void SpecGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            //最后一列是“描述”
            if (e.Column.Index == this.ColumnCount - 1)
                return;

            var col = e.Column as ComponentColumn;

            var sort = e.Column.HeaderCell.SortGlyphDirection;

            var row1 = this.Rows[e.RowIndex1] as SpecDataGridViewRow;
            var row2 = this.Rows[e.RowIndex2] as SpecDataGridViewRow;
            if (row1 == null && row2 == null)
            {
                e.SortResult = 0;
            }
            else if (row1 != null && row2 == null)
            {
                e.SortResult = sort == System.Windows.Forms.SortOrder.Ascending ? 1 : -1;
            }
            else if (row1 == null && row2 != null)
            {
                e.SortResult = sort == System.Windows.Forms.SortOrder.Ascending ? -1 : 1;
            }
            else
            {
                if (col != null || e.Column.Index == 0)
                {
                    var a = string.IsNullOrWhiteSpace(Convert.ToString(e.CellValue1)) ? double.MinValue : double.Parse(Convert.ToString(e.CellValue1));
                    var b = string.IsNullOrWhiteSpace(Convert.ToString(e.CellValue2)) ? double.MinValue : double.Parse(Convert.ToString(e.CellValue2));
                    if (a > b)
                        e.SortResult = 1;
                    else if (a.Equals(b))
                        e.SortResult = 0;
                    else
                        e.SortResult = -1;
                }
                else
                    e.SortResult = System.String.Compare(
               Convert.ToString(e.CellValue1), Convert.ToString(e.CellValue2));

            }
            e.Handled = true;
        }

        /// <summary>
        /// 保存性质
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dlgComp_EventSave(object sender, DlgComponentEventArgs e)
        {
            if (e.IsAdd)
            {
                if (this._specList.Comp_Contains(e.Comp))
                {
                    System.Windows.Forms.MessageBox.Show("性质名称重复，请重新输入。");
                    return;
                }
                this._specList.Comp_Add(e.Comp);
                this.Columns.Insert(this.colDescrip.Index, new ComponentColumn()
                {
                    Name = string.Format("col_{0}", e.Comp.Name),
                    HeaderText = string.Format("{0}", e.Comp.Name),
                    ToolTipText = string.Format("名称：{0}，单位：{1}，精度：{2}",
                        e.Comp.Name,
                        e.Comp.Units,
                        e.Comp.Eps),
                    Width = 60,
                    MinimumWidth = 60,
                    SortMode = DataGridViewColumnSortMode.Programmatic,
                    ReadOnly = !this._editEnbale,
                    Comp = e.Comp,
                    CompIndex = this._specList.Components.Count - 1
                });
                if (this.colDescrip.Index < this.Columns.Count - 1)
                    this.Columns.RemoveAt(this.Columns.Count - 1);
            }
            else
            {
                var col = this.Columns[e.ColIndex] as ComponentColumn;
                if (col == null)
                    return;
                var temp = Serialize.DeepClone<ComponentList>(this._specList.Components);
                temp.RemoveAt(col.CompIndex);
                if (temp.Contains(e.Comp))
                {
                    System.Windows.Forms.MessageBox.Show("性质名称重复，请重新输入。");
                    return;
                }

                col.HeaderText = string.Format("{0}", e.Comp.Name);
                col.Name = string.Format("col_{0}", e.Comp.Name);
                col.Comp = e.Comp;
                col.ToolTipText = string.Format("名称：{0}，单位：{1}，精度：{2}",
                        e.Comp.Name,
                        e.Comp.Units,
                        e.Comp.Eps);
                this._specList.Comp_EditProperty(col.CompIndex, e.Comp);
                //重新显示此列的数据

                for (int r = 0; r < this._specList.Count; r++)
                {
                    var row = this.Rows[r] as SpecDataGridViewRow;

                    if (row == null || r >= this._specList.Count)
                        return ;

                    var sidx = this._specList.GetIdxByName(row.Spec.Name);
                    var data = this._specList.Y[sidx + 1, col.CompIndex + 1].ToScalarDouble();
                    this[col.Index, r].Value = double.IsNaN(data) ? "" : data.ToString(this._specList.Components[col.CompIndex].EpsFormatString);


                  //  this[col.Index, r].Value = this._specList.Y[r + 1, col.CompIndex + 1].ToScalarDouble().ToString(this._specList.Components[col.CompIndex].EpsFormatString);
                }
                   

            }
            dlgComp.Close();
            this.fireEditChange();
            this._edited = true;
        }

        #endregion

        #region Public Method

        public bool Save()
        {
            return true;
        }

        public void SetVisible(bool tag)
        {
            this.Visible = tag;
        }

        public event EventHandler OnFinished;

        /// <summary>
        /// 添加光谱（弹出对话框）
        /// </summary>
        public void AddSpectrum(string initialDirectory)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.Multiselect = true;
            myOpenFileDialog.RestoreDirectory = true;
            myOpenFileDialog.InitialDirectory = initialDirectory;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.SelectionChanged -= new EventHandler(SpecGridView_SelectionChanged);

                try
                {
                    foreach (string p in myOpenFileDialog.FileNames)
                    {

                        Spectrum spec = new Spectrum(p);

                        if (spec != null)
                        {
                            this._specList.Add(spec);
                            this.Rows.Insert(this._specList.Count - 1, this._renderRow(this._specList[this._specList.Count - 1], this._specList.Count));
                            if (this._specList.Count < this.Rows.Count)
                                this.Rows.RemoveAt(this.Rows.Count - 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    MessageBox.Show(ex.Message);
                }
                this.SelectionChanged += new EventHandler(SpecGridView_SelectionChanged);

                //触发光谱列表变化整体
                if (this.SpecListChanged != null)
                    this.SpecListChanged(this, null);
                //触发光谱库被编辑过的事件
                this.fireEditChange();
                this._edited = true;

                this.renderRowNum();
            }
        }

        /// <summary>
        /// 删除选择中的光谱
        /// </summary>
        public void RemoveSpecturm()
        {
            var srows = this.getSelectRowIdxs();

            if (srows==null||srows.Length==0)
            {
                MessageBox.Show("您没有选中要删除的光谱。");
                return;
            }
            if (MessageBox.Show(string.Format("您将删除{0}条光谱，是否继续？", srows.Length), "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.SelectionChanged -= new EventHandler(SpecGridView_SelectionChanged);

                foreach(var i in srows.OrderByDescending(d=>d))
                {
                    this._specList.Remove(this[1, i].Value.ToString());
                    this.Rows.RemoveAt(i);
                }
                this.SelectionChanged += new EventHandler(SpecGridView_SelectionChanged);

                if (this.SpecListChanged != null)
                    this.SpecListChanged(this, null);
                this.fireEditChange();
                this.renderRowNum();
            }
        }

        /// <summary>
        /// 添加性质
        /// </summary>
        public void AddComponent()
        {
            if (!this._isInited)
                this._init();
            this.dlgComp.Dialog();
        }
        /// <summary>
        /// 删除选择中的性质
        /// </summary>
        public void RemoveComponent()
        {
            List<ComponentColumn> rowidx = new List<ComponentColumn>();
            var scols = this.getSelectColumnsIdxs();
            foreach (var i in scols)
            {
                var col = this.Columns[i] as ComponentColumn;
                if (col != null)
                    rowidx.Add(col);
            }
            if (rowidx.Count < 1)
            {
                MessageBox.Show("您没有选中要删除的性质。");
                return;
            }
            else
            {
                if (MessageBox.Show(string.Format("您将删除{0}个性质，是否继续？", rowidx.Count), "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    foreach (var i in rowidx.OrderByDescending(s => s.Index))
                    {
                        this._specList.Comp_Remove(i.Comp);
                        this.Columns.RemoveAt(i.Index);
                    }
                    //重新给序号
                    int count = 0;
                    for (int i = 0; i < this.Columns.Count; i++)
                    {
                        var col = this.Columns[i] as ComponentColumn;
                        if (col != null)
                        {
                            col.CompIndex = count;
                            count++;
                        }
                    }
                    this.fireEditChange();
                }
            }
        }

        /// <summary>
        /// 编辑选中的性质
        /// </summary>
        public void EditComponent()
        {
            var scols = this.getSelectColumnsIdxs();
            foreach(var i in scols)
            {
                var col = this.Columns[i] as ComponentColumn;
                if (col!=null)
                {
                   this.dlgComp.Dialog(col.Comp, false, this.Columns[i].Index);
                    break;
                }
                
            }
        }

        #endregion

        #region 私有方法

        public void Render()
        {
            if (this._isInited)
                this._initColunms();
            else
                this._init();


            if (this._specList == null)
                return;

            if (this._isShowComponent)
            {
                for (int i = 0; i < this._specList.Components.Count; i++)
                {
                    var c = this._specList.Components[i];
                    if (this._SingleComponent != null)
                    {
                        if (c.Name != this._SingleComponent.Name)
                            continue;
                    }

                    this.Columns.Insert(this.colDescrip.Index, new ComponentColumn()
                    {
                        Name = string.Format("col_{0}", c.Name),
                        HeaderText = string.Format("{0}", c.Name),
                        ToolTipText = string.Format("名称：{0}，单位：{1}，精度：{2}",
                            c.Name,
                            c.Units,
                            c.Eps),
                        Width = 60,
                        MinimumWidth = 60,
                        SortMode = DataGridViewColumnSortMode.Programmatic,
                        Comp = c,
                        CompIndex = i
                    });
                    if (this.colDescrip.Index < this.Columns.Count - 1)
                        this.Columns.RemoveAt(this.Columns.Count - 1);
                }
            }

            // Phase 3, Add spectrums rows
            int k = 0;
            foreach (var s in this._specList)
            {
                this.Rows.Insert(k, this._renderRow(s, k + 1));
                if (this._specList.Count < this.Rows.Count)
                    this.Rows.RemoveAt(this.Rows.Count - 1);
                k++;
            }

            this.renderRowNum();

        }
        private void fireEditChange()
        {
            if ( this.EditedChanged != null)
                this.EditedChanged(this, null);
        }

        private DataGridViewRow _renderRow(Spectrum s,int idx)
        {
            if (!this._isInited)
                this._init();
          
            var row = new SpecDataGridViewRow() { Spec = s };
            //Bitmap color = new Bitmap(20, 10);
            //Graphics myGraphics = Graphics.FromImage(color);
            //myGraphics.Clear(s.Color);

            List<object> cells = new List<object>();
            object[] objs = new object[]{
                idx,
                s.Name,
                s.Usage.GetDescription(),
                s.UUID
            };
            cells.AddRange(objs);
            if(s.Components!=null)
                foreach (var c in s.Components)
                {
                    if (this._SingleComponent != null)
                    {
                        if (c.Name != this._SingleComponent.Name)
                            continue;
                    }

                    string txt = double.IsNaN(c.ActualValue) ? "" : c.ActualValue.ToString(c.EpsFormatString);
                    cells.Add(txt);
                }
            cells.Add(s.Description);
            row.CreateCells(this, cells.ToArray());
            return row;
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        private void copyData(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.GetClipboardContent());
        }


        private void delData(object sender, EventArgs e)
        {
            if (this.SelectedRows.Count > 0)
                this.RemoveSpecturm();
            else
                for (int i = 0; i < this.SelectedCells.Count; i++)
                {
                    var c = this.SelectedCells[i];
                    var col = this.Columns[c.ColumnIndex] as ComponentColumn;
                    if (col != null)
                        this.changeCell(c.RowIndex, c.ColumnIndex, double.NaN.ToString());
                }
        }

        /// <summary>
        /// 粘贴数据
        /// </summary>
        private void pasteData(object sender, EventArgs e)
        {
            if(!this._editEnbale)
                return;

            //判断一下是否有复制内容
            string data = (string)Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(data))
                return;

            string[] grid;
            if (data.Contains("\r\n"))
            {
              //  grid = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Substring(data.Length - 2, 2) == "\r\n")
                    data = data.Substring(0, data.Length - 2);
                grid = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            else
            {
                grid = new string[1];
                grid[0] = data;
            }
            int errorCount = 0;

            if (this.SelectedCells.Count == 1)
            {
                //只有单一个单元格
                for (int i = 0; i < grid.Length; ++i)
                {
                    int r = i + this.CurrentCell.RowIndex;
                    if (r >= this.RowCount)
                    {
                        MessageBox.Show(String.Format("您复制的性质数据为{0}行，超过谱图数量{1}条，只有{1}行数据粘贴成功", grid.Length, i));
                        break;
                    }

                    data = grid[i];
                    if (!data.Contains("\t"))
                    {
                        try
                        {
                            if (!changeCell(r, this.CurrentCell.ColumnIndex, data))
                                errorCount++;
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                    }
                    else
                    {
                        string[] cur = data.Split('\t');
                        for (int j = 0; j < cur.Length; ++j)
                        {
                            int c = j + this.CurrentCell.ColumnIndex;
                            if (c >= this.ColumnCount) break;
                            try
                            {
                                if (!changeCell(r, c, cur[j]))
                                    errorCount++;
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
            }
            else
            {
                //有多个单元格
                Pair[] lst = new Pair[this.SelectedCells.Count];
                for (int i = 0; i < lst.Length; ++i)
                    lst[i] = new Pair(this.SelectedCells[i].RowIndex, this.SelectedCells[i].ColumnIndex);
                Array.Sort(lst);
                int cnt = 0;
                for (int i = 0; i < grid.Length; ++i)
                {
                    data = grid[i];
                    if (!data.Contains(","))
                    {
                        continue;
                    }
                    string[] cur = data.Split('\t');
                    for (int j = 0; j < cur.Length; ++j)
                    {
                        try
                        {
                            if (!changeCell(lst[cnt].Row, lst[cnt].Column, cur[j]))
                                errorCount++;
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                        }
                        ++cnt;
                        if (cnt == lst.Length)
                            return;

                    }
                }
            }
            this.EndEdit();
            if (errorCount > 0)
                MessageBox.Show(string.Format("有{0}项数据没有复制成功!", errorCount));
        }

        private bool changeCell(int rowIdx, int col, object value)
        {
            if (rowIdx < 0 || col < 0 || rowIdx >= this.Rows.Count || col > this.Columns.Count)
            {
                Log.Debug(string.Format("输入参数有输入，本Grid的行数为{0}，列数为{1}，参数为row={2}，col={3}", this.Rows.Count, this.Columns.Count, rowIdx, col));
                return false;
            }

            var r = this.Rows[rowIdx] as SpecDataGridViewRow;
          
            if (r == null || rowIdx>=this._specList.Count)
                return false;

            var sidx = this._specList.GetIdxByName(r.Spec.Name);
            Spectrum s = this._specList.Specs[sidx]; 
            if (col ==2)
            {
                s.Usage = (UsageTypeEnum)value;
                this[col, rowIdx].Value = s.Usage.GetDescription();
            }
            else if (col == 3)
            {
                var uuid =Convert.ToString(value);
                if (!string.IsNullOrWhiteSpace(uuid))
                {
                    s.UUID = uuid;
                    this[col, rowIdx].Value = value;
                }
            }
            else if (col == this._specList.Components.Count + 4)
            {
                var des = Convert.ToString(value);
                if (des!=null)
                {
                    s.Description = des;
                    this[col, rowIdx].Value = s.Description;
                }
            }
            else
            {
                var column = this.Columns[col] as ComponentColumn;
                if (column == null)
                    return false;
                var data = Convert.ToDouble(value);
                this._specList.Y[sidx + 1, column.CompIndex + 1] = data;
               // s.Components[column.CompIndex].ActualValue = Convert.ToDouble(value);
                this[col, rowIdx].Value = double.IsNaN(data) ? "" : data.ToString(this._specList.Components[column.CompIndex].EpsFormatString);
                   
            }
            this._edited = true;
            return true;
        }

        private void renderRowNum()
        {
            
            for (int i = 0; i < this.Rows.Count; i++)
            {
                this.Rows[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
              
        }

        #endregion

        #region Pair

        /// <summary>
        /// 自定义pair类,提供排序函数
        /// </summary>
        private class Pair : IComparable
        {
            /// <summary>
            /// 行,列
            /// </summary>
            public int Row, Column;

            /// <summary>
            /// 值
            /// </summary>
            public string value;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="r_"></param>
            /// <param name="c_"></param>
            public Pair(int r_, int c_)
            {
                Row = r_;
                Column = c_;
            }

            /// <summary>
            /// 比对
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {
                Pair p = (Pair)obj;
                if (Row != p.Row)
                {
                    if (Row < p.Row) return -1;
                    return 1;
                }
                else
                {
                    if (Column < p.Column) return -1;
                    else if (Column > p.Column) return 1;
                }
                return 0;
            }
        }

        #endregion


        public class ComponentColumn : DataGridViewTextBoxColumn
        {
            public RIPP.NIR.Component Comp { set; get; }

            public int CompIndex { set; get; }
        }

    }

    public class SpecRowSelectedArgus : EventArgs
    {
        public List<Spectrum> Specs { set; get; }
    }
}