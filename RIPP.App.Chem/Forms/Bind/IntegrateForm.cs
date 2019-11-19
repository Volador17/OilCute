using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.NIR.Models;
using RIPP.Lib;
using System.IO;
using log4net;


namespace RIPP.App.Chem.Forms.Bind
{
    public partial class IntegrateForm : Form, IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }

        private IntegrateModel _model = new IntegrateModel();

        public IntegrateForm()
        {
            InitializeComponent();



            this.Load += new EventHandler(IntegrateForm_Load);

            //设置编辑事件
            this.gridModel.CellBeginEdit += new DataGridViewCellCancelEventHandler(gridModel_CellBeginEdit);
            this.gridModel.CellEndEdit += new DataGridViewCellEventHandler(gridModel_CellEndEdit);

            this.gridWeigth.CellBeginEdit += new DataGridViewCellCancelEventHandler(gridWeigth_CellBeginEdit);
            this.gridWeigth.CellValueChanged += new DataGridViewCellEventHandler(gridWeigth_CellValueChanged);

        }

        void gridWeigth_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.ColumnIndex < 1 || e.ColumnIndex > 4)
                return;
            double d = 0;
            if (this.gridWeigth[e.ColumnIndex, e.RowIndex].Value != null && !string.IsNullOrEmpty(this.gridWeigth[e.ColumnIndex, e.RowIndex].Value.ToString()))
            {
                if (!double.TryParse(this.gridWeigth[e.ColumnIndex, e.RowIndex].Value.ToString(), out d))
                {

                    MessageBox.Show("必须输入数字");

                    //改为回来
                    this.gridWeigth.CellValueChanged -= new DataGridViewCellEventHandler(gridWeigth_CellValueChanged);
                    if (this._model.Weights != null)
                        this.gridWeigth[e.ColumnIndex, e.RowIndex].Value = this._model.Weights[e.RowIndex, e.ColumnIndex - 1];
                    else
                        this.gridWeigth[e.ColumnIndex, e.RowIndex].Value = 0;
                    this.gridWeigth.CellValueChanged += new DataGridViewCellEventHandler(gridWeigth_CellValueChanged);
                    this.gridWeigth[e.ColumnIndex, e.ColumnIndex].Selected = true;
                    this.gridWeigth.BeginEdit(false);
                    return;
                }
            }
            else
                d = 0;
            //算总和
            double sum = 0;
            for (int c = 1; c < 5; c++)
                sum += this.gridWeigth[c, e.RowIndex].Value == null ? 0 : Convert.ToDouble(this.gridWeigth[c, e.RowIndex].Value);
            this.gridWeigth[5, e.RowIndex].Value = sum.ToString("F2");

            if (this._model.Weights != null)
                this._model.Weights[e.RowIndex, e.ColumnIndex - 1] = d;

            this.checkWeights();

        }

        void gridWeigth_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.ColumnIndex < 1 || e.ColumnIndex > 4)
            {
                e.Cancel = true;
                return;
            }
            var cname = this.gridWeigth.Rows[e.RowIndex].Tag.ToString();
            //判断识别列是否可编辑
            switch (e.ColumnIndex)
            {
                case 1://识别
                    if (this._model.IDSelectMatrix == null || this._model.IDSelectMatrix.Length < e.RowIndex)
                        e.Cancel = true;
                    else if (this._model.IDSelectMatrix[e.RowIndex] == 0)
                        e.Cancel = true;
                    break;
                case 2://拟合
                    if (this._model.FitSelectMatrix == null || this._model.FitSelectMatrix.Length < e.RowIndex)
                        e.Cancel = true;
                    else if (this._model.FitSelectMatrix[e.RowIndex] == 0)
                        e.Cancel = true;
                    break;
                case 3:
                    e.Cancel = !this.plsHas(cname, false);
                    break;
                case 4:
                    e.Cancel = !this.plsHas(cname, true);
                    break;
                default:
                    break;
            }
        }

        void gridModel_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var c = this.gridModel[e.ColumnIndex, e.RowIndex] as DataGridViewCheckBoxCell;
            if (c == null)
                e.Cancel = true;
        }

        void gridModel_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.getDataFromControls();
            //修改系数
            this.setWightWithModel();

        }

        void IntegrateForm_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridModel);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridWeigth);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridMethods);
        }

        private void btnAddtmpl_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.LibTmp, FileExtensionEnum.LibTmp.GetDescription()),
                InitialDirectory = Busi.Common.Configuration.FolderSpecTemp
            };

            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            try
            {
                var s = new SpecBase(myOpenFileDialog.FileName);
                this._model = new IntegrateModel();
                this._model.Comps = s.Components.Clone();
                this.initGrid();
            }
            catch
            {

            }
            this.btnAddModel.Enabled = true;
            this.btnSave.Enabled = true;
        }
        /// <summary>
        /// 根据性质初始化行
        /// </summary>
        /// <param name="clst"></param>
        private void initGrid()
        {
            var clst = this._model.Comps;
            this.gridModel.Columns.Clear();
            this.gridWeigth.Columns.Clear();
            this.gridMethods.Columns.Clear();

            this.gridWeigth.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "性质",
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.gridWeigth.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "识别库%",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.gridWeigth.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "拟合库%",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.gridWeigth.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "PLS1%",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.gridWeigth.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "PLS-ANN%",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.gridWeigth.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "总和%",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });


            this.gridModel.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "性质",
                SortMode = DataGridViewColumnSortMode.NotSortable
            });


            this.gridMethods.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "类型",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.gridMethods.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "名称",
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.gridMethods.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "路径",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });


            int k = 1;
            foreach (var c in clst)
            {
                var row1 = new DataGridViewRow() { Tag = c.Name };
                row1.CreateCells(this.gridModel, c.Name);
                row1.HeaderCell.Value = k.ToString();
                this.gridModel.Rows.Add(row1);

                var row2 = new DataGridViewRow() { Tag = c.Name };
                row2.CreateCells(this.gridWeigth, c.Name);
                row2.HeaderCell.Value = k.ToString();
                this.gridWeigth.Rows.Add(row2);

                k++;
            }
        }

        private void btnAddModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("所有文件|*.{0};*.{2};*.{4};*.{6}|{1} (*.{0})|*.{0}|{3} (*.{2})|*.{2}|{5} (*.{4})|*.{4}|{7} (*.{6})|*.{6}",
                FileExtensionEnum.IdLib, FileExtensionEnum.IdLib.GetDescription(),
                FileExtensionEnum.FitLib, FileExtensionEnum.FitLib.GetDescription(),
                FileExtensionEnum.PLS1, FileExtensionEnum.PLS1.GetDescription(),
                FileExtensionEnum.PLSANN, FileExtensionEnum.PLSANN.GetDescription()
                );
            myOpenFileDialog.Title = "选择方法";
            myOpenFileDialog.Multiselect = true;
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.DefaultDirectory;
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            foreach (var f in myOpenFileDialog.FileNames)
                this.addModel(f);
        }

        private void addModel(string fullpath)
        {
            FileInfo f = new FileInfo(fullpath);
            string fexten = f.Extension.ToUpper().Replace(".", "");

            try
            {
                if (fexten == FileExtensionEnum.IdLib.ToString().ToUpper())
                {
                    var model = BindModel.ReadModel<IdentifyModel>(fullpath);
                    if (!this._model.AddID(model))
                        MessageBox.Show("该方法已经存在！");
                    else
                        this.showModel();
                }
                else if (fexten == FileExtensionEnum.FitLib.ToString().ToUpper())
                {
                    var model = BindModel.ReadModel<FittingModel>(fullpath);
                    if (!this._model.AddFit(model))
                        MessageBox.Show("该方法已经存在，或者该方法的参数与与有拟合库参数不一致！");
                    else
                        this.showModel();
                }
                else if (fexten == FileExtensionEnum.PLS1.ToString().ToUpper() || fexten == FileExtensionEnum.PLSANN.ToString().ToUpper())
                {
                    var model = BindModel.ReadModel<PLSSubModel>(fullpath);
                    var tag = false;
                    if (model.AnnType != PLSAnnEnum.None)
                        tag = this._model.CheckPLSIsExist(model, false);
                    else
                        tag = this._model.CheckPLSIsExist(model, true);
                    if (tag)
                    {
                        if (MessageBox.Show("信息提示！", "该模型已经存在，是否覆盖原有模型？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                            tag = true;
                    }
                    if (!tag)
                    {
                        if (model.AnnType != PLSAnnEnum.None)
                            this._model.AddAnn(model);
                        else
                            this._model.AddPLS1(model);
                        this.showModel();
                    }
                }
            }
            catch
            {
                MessageBox.Show("读取方法出错，请检查文件！");
            }
            this.getDataFromControls();
            this.setWightWithModel();
        }



        private void addIdModel(IdentifyModel model, int idx)
        {
            if (model == null)
                return;
            this.gridModel.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = string.Format("识别_{0}", model.Name),
                ToolTipText = model.FullPath,
                Tag = model.FullPath,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            int colIdx = this.gridModel.Columns.Count - 1;

            for (int i = 0; i < this.gridModel.Rows.Count; i++)
            {
                if (this.gridModel.Rows[i].Tag == null)
                    continue;
                var cname = this.gridModel.Rows[i].Tag.ToString();
                if (model.SpecLib.Components.Contains(cname))
                {
                    this.gridModel[colIdx, i] = new DataGridViewCheckBoxCell();
                    if (this._model.IDSelectMatrix == null)
                        this.gridModel[colIdx, i].Value = true;
                    else
                    {
                        var ddd = (this._model.IDSelectMatrix[this._model.Comps.GetIndex(cname)] & ((int)Math.Pow(2, idx))) == (int)Math.Pow(2, idx);
                        this.gridModel[colIdx, i].Value = ddd;
                    }
                }
                else
                {
                    this.gridModel[colIdx, i] = new DataGridViewTextBoxCell();
                    this.gridModel[colIdx, i].Value = null;
                }

            }

            //给
        }

        private void addFitModel(FittingModel model, int idx)
        {
            if (model == null)
                return;
            this.gridModel.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = string.Format("拟合_{0}", model.Name),
                Tag = model.FullPath,
                ToolTipText = model.FullPath,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            int colIdx = this.gridModel.Columns.Count - 1;
            for (int i = 0; i < this.gridModel.Rows.Count; i++)
            {
                if (this.gridModel.Rows[i].Tag == null)
                    continue;
                var cname = this.gridModel.Rows[i].Tag.ToString();
                if (model.SpecLib.Components.Contains(cname))
                {
                    this.gridModel[colIdx, i] = new DataGridViewCheckBoxCell();
                    if (this._model.FitSelectMatrix == null)
                        this.gridModel[colIdx, i].Value = true;
                    else
                    {
                        var ddd = (this._model.FitSelectMatrix[this._model.Comps.GetIndex(cname)] & ((int)Math.Pow(2, idx))) == (int)Math.Pow(2, idx);
                        this.gridModel[colIdx, i].Value = ddd;
                    }

                }
                else
                {
                    this.gridModel[colIdx, i] = new DataGridViewTextBoxCell();
                    this.gridModel[colIdx, i].Value = null;
                }
            }
        }

        private void addPLS1Model(List<PLSSubModel> models)
        {
            this.gridModel.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "PLS1",
                Tag = "PLS1",
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            int colIdx = this.gridModel.Columns.Count - 1;
            for (int i = 0; i < this.gridModel.Rows.Count; i++)
            {
                if (this.gridModel.Rows[i].Tag == null)
                    continue;
                var cname = this.gridModel.Rows[i].Tag.ToString();
                var m = models.Where(d => d.Comp.Name == cname).FirstOrDefault();
                if (m != null)
                {
                    this.gridModel[colIdx, i] = new DataGridViewCheckBoxCell();
                    this.gridModel[colIdx, i].Value = true;
                    this.gridModel[colIdx, i].Tag = m.FullPath;

                }
                else
                {
                    this.gridModel[colIdx, i] = new DataGridViewTextBoxCell();
                    this.gridModel[colIdx, i].Value = null;
                }
            }
        }

        private void addPLSAnnModel(List<PLSSubModel> models)
        {

            this.gridModel.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "PLS-ANN",
                Tag = "PLS-ANN",
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            int colIdx = this.gridModel.Columns.Count - 1;
            for (int i = 0; i < this.gridModel.Rows.Count; i++)
            {
                if (this.gridModel.Rows[i].Tag == null)
                    continue;
                var cname = this.gridModel.Rows[i].Tag.ToString();
                var m = models.Where(d => d.Comp.Name == cname).FirstOrDefault();
                if (m != null)
                {
                    this.gridModel[colIdx, i] = new DataGridViewCheckBoxCell();
                    this.gridModel[colIdx, i].Value = true;
                    this.gridModel[colIdx, i].Tag = m.FullPath;

                }
                else
                {
                    this.gridModel[colIdx, i] = new DataGridViewTextBoxCell();
                    this.gridModel[colIdx, i].Value = null;
                }
            }
        }
        /// <summary>
        /// 显示模型信息
        /// </summary>
        private void showModel()
        {
            //清除其它列
            for (int i = this.gridModel.Columns.Count - 1; i > 0; i--)
                if (i > 0)
                    this.gridModel.Columns.RemoveAt(i);

            //添加识别相关的列
            for (int i = 0; i < this._model.IdModels.Count; i++)
                this.addIdModel(this._model.IdModels[i], i);


            //添加拟合相关列
            for (int i = 0; i < this._model.FitModels.Count; i++)
                this.addFitModel(this._model.FitModels[i], i);


            //添加PLS列
            this.addPLS1Model(this._model.Pls1Models);
            this.addPLSAnnModel(this._model.PlsANNModels);

            //gridMethods
            this.gridMethods.Rows.Clear();
            if (this._model.IdModels != null)
                foreach (var m in this._model.IdModels)
                    this.gridMethods.Rows.Add("识别", m.Name, m.FullPath);
            if (this._model.FitModels != null)
                foreach (var m in this._model.FitModels)
                    this.gridMethods.Rows.Add("拟合", m.Name, m.FullPath);
            if (this._model.Pls1Models != null)
                foreach (var m in this._model.Pls1Models)
                    this.gridMethods.Rows.Add("PLS1", m.Comp.Name, m.FullPath);
            if (this._model.PlsANNModels != null)
                foreach (var m in this._model.PlsANNModels)
                    this.gridMethods.Rows.Add("PLS-ANN", m.Comp.Name, m.FullPath);


        }

        /// <summary>
        /// 获取模型列表中的选中的数据
        /// </summary>
        private void getDataFromControls()
        {

            //构建两个矩阵
            if (this._model.IdPath.Count > 0)
            {
                this._model.IDSelectMatrix = new int[this._model.Comps.Count];
                //根据完整路径来关联吧
                for (int col = 0; col < this._model.IdPath.Count; col++)
                {
                    var idx = this.findColumnIdx(this.gridModel, this._model.IdPath[col]);
                    if (idx < 0)
                        continue;
                    for (int r = 0; r < this.gridModel.Rows.Count; r++)
                    {
                        var c = this.gridModel[idx, r] as DataGridViewCheckBoxCell;
                        if (c != null && Convert.ToBoolean(c.Value))
                            this._model.IDSelectMatrix[r] += (int)Math.Pow(2, col);
                    }
                }
            }

            if (this._model.FitPath.Count > 0)
            {
                this._model.FitSelectMatrix = new int[this._model.Comps.Count];
                //根据完整路径来关联吧
                for (int col = 0; col < this._model.FitPath.Count; col++)
                {
                    var idx = this.findColumnIdx(this.gridModel, this._model.FitPath[col]);
                    if (idx < 0)
                        continue;
                    for (int r = 0; r < this.gridModel.Rows.Count; r++)
                    {
                        var c = this.gridModel[idx, r] as DataGridViewCheckBoxCell;
                        if (c != null && Convert.ToBoolean(c.Value))
                            this._model.FitSelectMatrix[r] += (int)Math.Pow(2, col);
                    }
                }
            }


            // To do list
            // 1、剔除没有用到的PLS模型



            if (this._model.PLS1Path.Count > 0)
            {

            }

            //构建权重矩阵
            this._model.Weights = new double[this._model.Comps.Count, 4];
            for (int r = 0; r < this.gridWeigth.Rows.Count; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    this._model.Weights[r, c] = Convert.ToDouble(this.gridWeigth[c + 1, r].Value);
                }
            }


        }
        /// <summary>
        /// 根据方法清除其权重
        /// </summary>
        private void setWightWithModel()
        {
            for (int r = 0; r < this.gridWeigth.Rows.Count; r++)
                for (int c = 0; c < this.gridWeigth.Columns.Count; c++)
                    this.gridWeigth[c, r].Tag = null;


            //识别
            if (this._model.IDSelectMatrix == null)
            {
                for (int i = 0; i < this.gridWeigth.Rows.Count; i++)
                {
                    this.gridWeigth[1, i].Value = null;
                    this.gridWeigth[1, i].Tag = "none";
                }
            }
            else
                for (int i = 0; i < this._model.IDSelectMatrix.Length; i++)
                    if (this._model.IDSelectMatrix[i] == 0)
                    {
                        this.gridWeigth[1, i].Value = null;
                        this.gridWeigth[1, i].Tag = "none";
                    }
            //拟合
            if (this._model.FitSelectMatrix == null)
                for (int i = 0; i < this.gridWeigth.Rows.Count; i++)
                {
                    this.gridWeigth[2, i].Value = null;
                    this.gridWeigth[2, i].Tag = "none";
                }
            else
                for (int i = 0; i < this._model.FitSelectMatrix.Length; i++)
                    if (this._model.FitSelectMatrix[i] == 0)
                    {
                        this.gridWeigth[2, i].Value = null;
                        this.gridWeigth[2, i].Tag = "none";
                    }
            //PLS1
            for (int i = 0; i < this.gridWeigth.Rows.Count; i++)
                if (!this.plsHas(this.gridWeigth.Rows[i].Tag.ToString(), false))
                {
                    this.gridWeigth[3, i].Value = null;
                    this.gridWeigth[3, i].Tag = "none";
                }


            //PLS-ANN
            for (int i = 0; i < this.gridWeigth.Rows.Count; i++)
                if (!this.plsHas(this.gridWeigth.Rows[i].Tag.ToString(), true))
                {
                    this.gridWeigth[4, i].Value = null;
                    this.gridWeigth[4, i].Tag = "none";
                }

            for (int r = 0; r < this.gridWeigth.Rows.Count; r++)
                for (int c = 1; c < 5; c++)
                    if (this.gridWeigth[c, r].Tag != null)
                        this.gridWeigth[c, r].Style.BackColor = Color.FromArgb(230, 230, 230);
                    else
                    {
                        this.gridWeigth[c, r].Style.BackColor = Color.FromArgb(255, 255, 255);
                        this.gridWeigth[c, r].Value = this._model.Weights[r, c - 1].ToString("F1");
                    }


        }
        /// <summary>
        /// 根据tag找出行号
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private int findColumnIdx(DataGridView grid, string tag)
        {
            int idx = -1;
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                if (grid.Columns[i].Tag == null)
                    continue;
                if (grid.Columns[i].Tag.ToString() == tag)
                    return i;
            }
            return idx;
        }

        private int findRowIdx(DataGridView grid, string tag)
        {
            int idx = -1;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (grid.Rows[i].Tag == null)
                    continue;
                if (grid.Rows[i].Tag.ToString() == tag)
                    return i;
            }
            return idx;
        }

        private bool checkWeights()
        {
            if (this._model.Comps == null || this._model.Comps.Count == 0)
                return false;
            if (this._model.Weights == null)
                this.getDataFromControls();
            int cols = this._model.Weights.GetLength(1);
            var tag = true;
            for (int r = 0; r < this._model.Weights.GetLength(0); r++)
            {
                double sum = 0;
                for (int c = 0; c < cols; c++)
                    sum += double.IsNaN(this._model.Weights[r, c]) ? 0 : this._model.Weights[r, c];
                if (Math.Abs(sum - 100) > 0.01)
                {
                    tag = false;
                    this.gridWeigth[5, r].Style.BackColor = Color.Red;
                }
                else
                    this.gridWeigth[5, r].Style.BackColor = Color.White;
            }


            return tag;

        }

        private bool plsHas(string cname, bool isann)
        {
            var models = isann ? this._model.PlsANNModels : this._model.Pls1Models;
            var colidx = isann ? this.gridModel.Columns.Count - 1 : this.gridModel.Columns.Count - 2;

            if (models == null)
                return false;
            if (models.Where(d => d.Comp.Name == cname).Count() == 0)
                return false;
            var cell = this.gridModel[colidx, this.findRowIdx(this.gridModel, cname)] as DataGridViewCheckBoxCell;
            if (cell == null || !Convert.ToBoolean(cell.Value))
                return false;
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.getDataFromControls();
            if (!checkWeights())
            {
                MessageBox.Show("权重系数有误，你仔细检查！");
                return;
            }
            this.gridModel.EndEdit();

            //对话框
            if (String.IsNullOrWhiteSpace(this._model.FullPath))
            {
                SaveFileDialog mySaveFileDialog = new SaveFileDialog();
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.ItgBind, FileExtensionEnum.ItgBind.GetDescription());
                mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMInteg;
                if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                this._model.FullPath = mySaveFileDialog.FileName;
                var finfo = new FileInfo(mySaveFileDialog.FileName);
                this._model.CreateTime = DateTime.Now;
                this._model.Name = finfo.Name.Replace(finfo.Extension, "");
            }

            if (this._model.Save())
            {
                MessageBox.Show("集成包保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Text = string.Format("集成包管理_{0}", this._model.FullPath);
            }
            else
                MessageBox.Show("集成包保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.showModel();
            this.setTitle();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var s = new Spectrum(@"D:\3506\03chemometrics\RIPP_DEMO\src\RIPP.App.Chem\bin\Debug\Spec\DHUNG317.csv");
            var d = this._model.Predict(s);
        }

        private void btnDelMethod_Click(object sender, EventArgs e)
        {
            if (this.gridMethods.SelectedRows.Count == 0)
                return;
            if (MessageBox.Show("删除确认", "是否删除选择的方法?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;
            for (int i = this.gridMethods.SelectedRows.Count - 1; i >= 0; i--)
            {
                var type = this.gridMethods.SelectedRows[i].Cells[0].Value.ToString();
                var fullpath = this.gridMethods.SelectedRows[i].Cells[2].Value.ToString();
                var name = (new FileInfo(fullpath)).Name;


                switch (type)
                {
                    case "识别":
                        this._model.IdPath.Remove(fullpath);
                        this._model.IdModels = null;
                        var dddd = this._model.IdModels;
                        this._model.IDSelectMatrix = null;
                        break;
                    case "拟合":
                        this._model.FitPath.Remove(fullpath);
                        this._model.FitModels = null;
                        var dddd2 = this._model.FitModels;
                        this._model.FitSelectMatrix = null;
                        break;
                    case "PLS1":
                        this._model.PLS1Path.Remove(fullpath);
                        this._model.Pls1Models = null;
                        var ccc = this._model.Pls1Models;
                        break;
                    case "PLS-ANN":
                        this._model.PLSANNPath.Remove(fullpath);
                        this._model.PlsANNModels = null;
                        var ccc1 = this._model.PlsANNModels;
                        break;
                    default:
                        break;
                }
                this.gridMethods.Rows.RemoveAt(this.gridMethods.SelectedRows[i].Index);
            }

            this.showModel();
            this.getDataFromControls();
            this.setWightWithModel();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.ItgBind, FileExtensionEnum.ItgBind.GetDescription());
            myOpenFileDialog.Title = "选择方法";
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMInteg;
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this._model = BindModel.ReadModel<IntegrateModel>(myOpenFileDialog.FileName);



            //检查子方法是否存在
            List<string> notExitFiles = this._model.CheckModelNotExist();
            if (notExitFiles.Count > 0)
            {
                MessageBox.Show(string.Format("以下方法不存在，未被加载：\n{0}", string.Join("\n", notExitFiles)));
                return;
            }
            this.initGrid();
            this.showModel();
            this.setWightWithModel();

            this.btnAddModel.Enabled = true;
            this.btnSave.Enabled = true;
            this.setTitle();

        }

        private void setTitle()
        {
            this.Text = this._model.FullPath;
        }

        private void btnSetDefault_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("本操作将清除原有的系数，是否继续？", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;
            for (int r = 0; r < this.gridWeigth.Rows.Count; r++)
            {
                var id = this.gridWeigth[1, r].Value != null;
                var fit = this.gridWeigth[2, r].Value != null;
                var pls1 = this.gridWeigth[3, r].Value != null;
                var ann = this.gridWeigth[4, r].Value != null;
                var idv = double.NaN;
                var fitv = double.NaN;
                var pls1v = double.NaN;
                var annv = double.NaN;
                if (id)
                {
                    if(( fit&&(!pls1&&!ann) )||(!fit&& (ann || pls1)))
                        idv = 65;
                    else if (fit && (ann || pls1))
                        idv = 55;
                    else
                        idv = 100;
                }
                if (fit)
                {
                    if (id && (!pls1 && !ann))
                        fitv = 35;
                    else if (id && (pls1 || ann))
                        fitv = 30;
                    else if (pls1 || ann)
                        fitv = 65;
                    else
                        fitv = 100;
                }
                if (pls1)
                {
                    if (id && !fit)
                        pls1v = 35;
                    else if (id && fit)
                        pls1v = 15;
                    else if (!id && fit)
                        pls1v = 35;
                    else
                        pls1v = 100;
                    if (ann)
                        pls1v = pls1v / 2;
                }
                if (ann)
                {
                    if (id && !fit)
                        annv = 35;
                    else if (id && fit)
                        annv = 15;
                    else if (!id && fit)
                        annv = 35;
                    else
                        pls1v = 100;
                    if (pls1)
                        annv = annv / 2;
                }

                if (double.IsNaN(idv))
                {
                    this.gridWeigth[1, r].Value = null;
                    this.gridWeigth[1, r].Tag = "none";
                }
                else
                {
                    this.gridWeigth[1, r].Value = idv;
                    this.gridWeigth[1, r].Tag = null;
                }

                if (double.IsNaN(fitv))
                {
                    this.gridWeigth[2, r].Value = null;
                    this.gridWeigth[2, r].Tag = "none";
                }
                else
                {
                    this.gridWeigth[2, r].Value = fitv;
                    this.gridWeigth[2, r].Tag = null;
                }

                if (double.IsNaN(pls1v))
                {
                    this.gridWeigth[3, r].Value = null;
                    this.gridWeigth[3, r].Tag = "none";
                }
                else
                {
                    this.gridWeigth[3, r].Value = pls1v;
                    this.gridWeigth[3, r].Tag = null;
                }

                if (double.IsNaN(annv))
                {
                    this.gridWeigth[4, r].Value = null;
                    this.gridWeigth[1, r].Tag = "none";
                }
                else
                {
                    this.gridWeigth[4, r].Value = annv;
                    this.gridWeigth[4, r].Tag = null;
                }
            }

        }
    }
}
