using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using System.Drawing;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.DataCheck;
using RIPP.Lib;
using RIPP.Lib.Common;
using RIPP.OilDB.UI.GridOil.V2.Model;
using System.Data.SqlTypes;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public abstract partial class IGridOilInfo<TOilInfo> : DataGridView
        where TOilInfo : class,  IOilInfoEntity, new()
    {
        #region 私有变量
        protected TOilInfo _oilInfo = null;                    // 单元格
        /// <summary>
        /// 时间控件
        /// </summary>
        private DateCheckControl _SDADataCheckControl = new DateCheckControl();
        private DateCheckControl _RDADataCheckControl = new DateCheckControl();
        private DateCheckControl _ADADataCheckControl = new DateCheckControl();
        private DateCheckControl _UDDDataCheckControl = new DateCheckControl();
        /// <summary>
        /// 时间控件
        /// </summary>
        private DateTimePicker _timePicker = null;
        private ComboBox _cellCmb = new ComboBox();   //cmbox控件
        protected bool _isChanged = false; //是否有改动
        private OilTableTypeEntity _tableType = new OilTableTypeEntity(); //表类别
        private List<OilTableRowEntity> _oilTableRows = null;            // 行
        private const string dateFormat = "yyyy-MM-dd";
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        private OilDataCheck oilDataCheck = new OilDataCheck();
        private bool isValidated = false;//判断是否通过错误验证
        #endregion

        #region "公有变量"
        /// <summary>
        /// 是否有改动 
        /// </summary>
        public bool isChanged
        {
            set { this._isChanged = value; }
            get { return this._isChanged; }
        }
        /// <summary>
        /// 是否通过错误验证 
        /// </summary>
        public bool IsValidated
        {
            set { this.isValidated = value; }
            get { return this.isValidated; }
        }
        #endregion

        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public IGridOilInfo()
        {
            InitializeComponent();
            OilTableTypeBll tableCache = new OilTableTypeBll(); ;
            _tableType = tableCache.Where(t => t.ID == (int)EnumTableType.Info).FirstOrDefault();
            OilTableRowBll bll = new OilTableRowBll();
            _oilTableRows = bll.Where(c => c.oilTableTypeID == (int)EnumTableType.Info).ToList();
            InitStyle();
            //this.CellValueChanged += new DataGridViewCellEventHandler(cellValueChanged);
            this.SelectionChanged += GridOilInfo_SelectionChanged;
            this._SDADataCheckControl.Visible = false;
            this._RDADataCheckControl.Visible = false;
            this._ADADataCheckControl.Visible = false;
            this._UDDDataCheckControl.Visible = false;
            this._ADADataCheckControl.VisibleChanged+=_ADADataCheckControl_VisibleChanged;
            this._SDADataCheckControl.VisibleChanged +=_SDADataCheckControl_VisibleChanged;
            this._RDADataCheckControl.VisibleChanged +=_RDADataCheckControl_VisibleChanged;
            this.Controls.Add(this._SDADataCheckControl);
            this.Controls.Add(this._RDADataCheckControl);
            this.Controls.Add(this._ADADataCheckControl);
            this.Controls.Add(this._UDDDataCheckControl);
   
            this.CellClick += new DataGridViewCellEventHandler(dgdView_CellClick);
            this.Scroll += new ScrollEventHandler(dgdView_Scroll);
            this.ColumnWidthChanged += new DataGridViewColumnEventHandler(dgdView_ColumnWidthChanged);
            this.Controls.Add(this._timePicker);   // 将下拉列表框加入到DataGridView控件中


            this._cellCmb.Visible = false;                  // 设置下拉列表框不可见          
            this._cellCmb.SelectedIndexChanged += new EventHandler(cmb_Temp_SelectedIndexChanged);  // 添加下拉列表框事件         
            this.Controls.Add(this._cellCmb);   // 将下拉列表框加入到DataGridView控件中   

            this.RowPostPaint += new DataGridViewRowPostPaintEventHandler(GridOil_RowPostPaint);
            this._cellCmb.KeyPress += this._cellCmb_KeyPress;
        }

        #region 
        void _ADADataCheckControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this._ADADataCheckControl.Visible == true)
            {
                ADARowIndex = this.CurrentCell.RowIndex;
                if (this.CurrentCell.Value != null)
                {
                    this._ADADataCheckControl.ShortTime = oilDataCheck.GetDate(this.CurrentCell.Value);
                }
                else
                    this._ADADataCheckControl.ShortTime = DateTime.Now;

                Rectangle rect = this.GetCellDisplayRectangle(this.CurrentCell.ColumnIndex, this.CurrentCell.RowIndex, false);
                this._ADADataCheckControl.Visible = true;
                this._ADADataCheckControl.Left = rect.Left;
                this._ADADataCheckControl.Top = rect.Top;
                this._ADADataCheckControl.Width = rect.Width;
                this._ADADataCheckControl.Height = rect.Height;
                this._ADADataCheckControl.Focus();
            }
            else
            {
                if (ADARowIndex != -1)
                {
                    this.Rows[ADARowIndex].Cells["itemValue"].Value = this._ADADataCheckControl.ShortTime != null ? this._ADADataCheckControl.ShortTime.Value.ToString(dateFormat) : string.Empty;
                }
            }
        }

        void _SDADataCheckControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this._SDADataCheckControl.Visible == true)
            {
                SDARowIndex = this.CurrentCell.RowIndex;
                if (this.CurrentCell.Value != null)
                {
                    this._SDADataCheckControl.ShortTime = oilDataCheck.GetDate(this.CurrentCell.Value);
                }
                else
                    this._SDADataCheckControl.ShortTime = DateTime.Now;

                Rectangle rect = this.GetCellDisplayRectangle(this.CurrentCell.ColumnIndex, this.CurrentCell.RowIndex, false);
                this._SDADataCheckControl.Visible = true;
                this._SDADataCheckControl.Left = rect.Left;
                this._SDADataCheckControl.Top = rect.Top;
                this._SDADataCheckControl.Width = rect.Width;
                this._SDADataCheckControl.Height = rect.Height;
                this._SDADataCheckControl.Focus();
            }
            else
            {
                if (SDARowIndex != -1)
                {
                    this.Rows[SDARowIndex].Cells["itemValue"].Value = this._SDADataCheckControl.ShortTime != null ? this._SDADataCheckControl.ShortTime.Value.ToString(dateFormat) : string.Empty;
                    this._SDADataCheckControl.Visible = false;
                }

            }
        }

        void _RDADataCheckControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this._RDADataCheckControl.Visible == true)
            {
                   RDARowIndex = this.CurrentCell.RowIndex;
                    if (this.CurrentCell.Value != null)
                    {
                        this._RDADataCheckControl.ShortTime = oilDataCheck.GetDate(this.CurrentCell.Value);
                    }
                    else
                        this._RDADataCheckControl.ShortTime = DateTime.Now;

                    Rectangle rect = this.GetCellDisplayRectangle(this.CurrentCell.ColumnIndex, this.CurrentCell.RowIndex, false);
                    this._RDADataCheckControl.Visible = true;
                    this._RDADataCheckControl.Left = rect.Left;
                    this._RDADataCheckControl.Top = rect.Top;
                    this._RDADataCheckControl.Width = rect.Width;
                    this._RDADataCheckControl.Height = rect.Height;
                    this._RDADataCheckControl.Focus();               
            }
            else
            {                
                if (RDARowIndex != -1)                  
                {
                    this.Rows[RDARowIndex].Cells["itemValue"].Value = this._RDADataCheckControl.ShortTime != null ? this._RDADataCheckControl.ShortTime.Value.ToString(dateFormat) : string.Empty;
                    this._RDADataCheckControl.Visible = false;
                }
            }
        }

        #endregion
      
        void GridOilInfo_SelectionChanged(object sender, EventArgs e)
        {
            this._ADADataCheckControl.Visible = false;
            this._SDADataCheckControl.Visible = false;
            this._RDADataCheckControl.Visible = false;
            this._cellCmb.Visible = false;
        }

        /// <summary>
        /// 在下拉列表中，可以按字母键进行快速查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _cellCmb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar) == false)
                return;
            var ds = this._cellCmb.DataSource as List<S_ParmEntity>;
            if (ds == null)
                return;
            var index = this._cellCmb.SelectedIndex;
            var key = char.ToUpper(e.KeyChar);
            var t = ds.Skip(index + 1).FirstOrDefault(o => key == char.ToUpper(ChineseToPinYin.GetFirstChar(o.parmName)));
            if (t == null)
                t = ds.Take(index + 1).FirstOrDefault(o => key == char.ToUpper(ChineseToPinYin.GetFirstChar(o.parmName)));
            if (t == null)
                return;
            _cellCmb.SelectedIndex = ds.IndexOf(t);

        }

        private void GridOil_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y,
             this.RowHeadersWidth - 4,
             e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

        }

        /// <summary>
        /// 绑定下拉列表框,根据参数编码查询绑定
        /// </summary>
        private void cellCmbBind(string code)
        {
            S_ParmBll paraBll = new S_ParmBll();
            List<S_ParmEntity> parms = paraBll.GetParms(code);
            if (parms == null)
                return;

            this._cellCmb.ValueMember = "parmValue";
            this._cellCmb.DisplayMember = "parmName";
            this._cellCmb.DataSource = parms;
            this._cellCmb.DropDownStyle = ComboBoxStyle.DropDownList;

        }

        /// <summary>
        /// 当用户选择下拉列表框时改变DataGridView单元格的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
 
        private void cmb_Temp_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CurrentCell.Value = this._cellCmb.SelectedValue;
        }
        string itemCode = string.Empty;
       
        int SDARowIndex = -1, RDARowIndex = -1, ADARowIndex = -1, UDDRowIndex = -1;
        /// <summary>
        /// 单元格进入事件-在单元格中显示时间控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgdView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                itemCode = this.Rows[this.CurrentCell.RowIndex].Cells["itemCode"].Value.ToString().Trim();

                if (itemCode == "ADA")
                    this._ADADataCheckControl.Visible = true;
                else if (itemCode == "SDA")
                    this._SDADataCheckControl.Visible = true;
                else if (itemCode == "RDA")
                    this._RDADataCheckControl.Visible = true;

                if ((itemCode == "COU" || itemCode == "GRC" || itemCode == "AL") && this.CurrentCell.OwningColumn.Name.Equals("itemValue"))
                {
                    string value = this.CurrentCell.Value == null ? "" : this.CurrentCell.Value.ToString();
                    cellCmbBind(itemCode);
                    if (value != "")  //有值时，根据值找到该值对应的索引，显示
                    {
                        S_ParmBll paraBll = new S_ParmBll();
                        int selectIndex = paraBll.getParmIndex(value, itemCode);
                        this._cellCmb.SelectedIndex = selectIndex;
                    }
                    Rectangle rect = this.GetCellDisplayRectangle(this.CurrentCell.ColumnIndex, this.CurrentCell.RowIndex, false);
                    this._cellCmb.Left = rect.Left;
                    this._cellCmb.Top = rect.Top;
                    this._cellCmb.Width = rect.Width;
                    this._cellCmb.Height = rect.Height;
                    this._cellCmb.Visible = true;
                    this.CurrentCell.Value = _cellCmb.SelectedValue;
                    this._cellCmb.Focus();
                }
                else
                    this._cellCmb.Visible = false;
            }
            catch (Exception ex)
            {
                Log.Error("数据管理dgdView_CellEnter()" + ex);
            }
        }

        // 滚动DataGridView时将下拉列表框设为不可见
        private void dgdView_Scroll(object sender, ScrollEventArgs e)
        {
            this.EndEdit();
            this._ADADataCheckControl.Visible = false;
            this._SDADataCheckControl.Visible = false;
            this._RDADataCheckControl.Visible = false;
 
            this._cellCmb.Visible = false;
        }

        // 改变DataGridView列宽时将下拉列表框设为不可见
        private void dgdView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            this.EndEdit();
            this._ADADataCheckControl.Visible = false;
            this._SDADataCheckControl.Visible = false;
            this._RDADataCheckControl.Visible = false;
            //this._timePicker.Visible = false;
            this._cellCmb.Visible = false;
        }
 
        #endregion
        /// <summary>
        /// DataGridView单元格值改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void cellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this._isChanged = true;
            if (this.Columns[e.ColumnIndex].Name.Equals("itemValue"))//
            {
                var cell = this[e.ColumnIndex, e.RowIndex];
                this.CurrentCell.Style = myStyle.dgdViewCellStyleByRowIndex(e.RowIndex);
                string itemCode = this.Rows[e.RowIndex].Cells["itemCode"].Value.ToString().Trim();
                if ((itemCode == "SDA" || itemCode == "RDA" || itemCode == "ADA") && string.IsNullOrWhiteSpace(cell.Value as string) == false)
                {
                    OilDataCheck oilDataCheck = new OilDataCheck();
                    var date = oilDataCheck.GetDate((cell.Value as string).Trim());
                    cell.Value = date == null ? string.Empty : date.Value.ToString(dateFormat);
                }
                else if (itemCode == "UDD" && string.IsNullOrWhiteSpace(cell.Value as string) == false)
                {
                    OilDataCheck oilDataCheck = new OilDataCheck();
                    var date = oilDataCheck.GetDate((cell.Value as string).Trim());
                    cell.Value = date == null ? string.Empty : date.Value.ToString(LongDateFormat);
                }
            }
        }

        #region 公有函数

        /// <summary>
        ///  初始化表，给表头、行头和单元格赋值
        /// </summary>
        /// <param name="info"></param>
        public void FillOilInfo(TOilInfo info)
        {
            this._oilInfo = info;
            this._setColHeader();
            this._setRowHeader();
            this._setCellValues();
            _isChanged = false;
        }

        /// <summary>
        ///  初始化表，给表头、行头和单元格赋值
        /// </summary>
        /// <param name="info"></param>
        public void FillOilInfoA(TOilInfo info)
        {
            this._oilInfo = info;
            this._setColHeader();
            this._setRowHeader();
            this._setCellValues();
            _isChanged = false;
        }


        /// <summary>
        ///  初始化表，给表头、行头和单元格赋值
        /// </summary>
        /// <param name="info"></param>
        public void FillOilInfoB(TOilInfo info)
        {
            this._oilInfo = info;
            this._setColHeader();
            this._setRowHeaderB();
            this._setCellValues();
            _isChanged = false;
        }
        /// <summary>
        /// 从窗体中读出数据
        /// </summary>
        protected void ReadDataFromUI()
        {
            this.EndEdit();
            OilDataCheck dataCheck = new OilDataCheck();

            for (int i = 0; i < this.Rows.Count; i++)
            {
                DataGridViewRow row = this.Rows[i];

                #region "10"
                if (row.Tag.ToString() == "CNA")//原油名称
                    this._oilInfo.crudeName = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "ENA")//原油名称
                    this._oilInfo.englishName = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "IDC")//原油编号
                    this._oilInfo.crudeIndex = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "COU")//产地国家
                    this._oilInfo.country = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "GRC")//地理区域
                    this._oilInfo.region = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "FBC")//油田区块
                    this._oilInfo.fieldBlock = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "SS")//采样地点
                    this._oilInfo.sampleSite = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "ASC")//样品来源
                    this._oilInfo.assayCustomer = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "BLN")//样品信息
                    this._oilInfo.BlendingType = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "SDA")//采样日期
                    this._oilInfo.sampleDate = dataCheck.GetDate(this.Rows[i].Cells["itemValue"].Value);
                #endregion

                #region "10"
                else if (row.Tag.ToString() == "RDA")//到院日期
                    this._oilInfo.receiveDate = dataCheck.GetDate(this.Rows[i].Cells["itemValue"].Value);
                else if (row.Tag.ToString() == "ADA")//评价日期
                    this._oilInfo.assayDate = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "UDD")//入库日期
                    //this._oilInfo.updataDate = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                    this._oilInfo.updataDate = DateTime.Now.ToString(LongDateFormat);
                else if (row.Tag.ToString() == "ALA")//评价单位
                    this._oilInfo.assayLab = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "AER")//评价人员
                    this._oilInfo.assayer = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "RIN")//报告号
                    this._oilInfo.reportIndex = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "SRI")//数据资源
                    this._oilInfo.DataSource = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "SR")//数据详源
                    this._oilInfo.sourceRef = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "DQU")//数据质量
                    this._oilInfo.DataQuality = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "SUM")//评论
                    this._oilInfo.summary = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                #endregion

                #region "10"
                else if (row.Tag.ToString() == "CLA")//类别
                    this._oilInfo.type = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "TYP")//基数
                    this._oilInfo.classification = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "SCL")//硫水平
                    this._oilInfo.sulfurLevel = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "ACL")//酸水平
                    this._oilInfo.acidLevel = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "COL")//腐蚀指数
                    this._oilInfo.corrosionLevel = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "PRI")//加工指数
                    this._oilInfo.processingIndex = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "NIR")//NIR光谱
                    this._oilInfo.NIRSpectrum = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "REM")//NIR光谱
                    this._oilInfo.Remark = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "01R")//补充信息1
                    this._oilInfo.S_01R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "02R")//补充信息2
                    this._oilInfo.S_02R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                #endregion

                #region "10"
                else if (row.Tag.ToString() == "03R")//补充信息3
                    this._oilInfo.S_03R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "04R")//补充信息4
                    this._oilInfo.S_04R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "05R")//补充信息5
                    this._oilInfo.S_05R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "06R")//补充信息6
                    this._oilInfo.S_06R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "07R")//补充信息7
                    this._oilInfo.S_07R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "08R")//补充信息8
                    this._oilInfo.S_08R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "09R")//补充信息9
                    this._oilInfo.S_09R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                else if (row.Tag.ToString() == "10R")//补充信息10
                    this._oilInfo.S_10R = this.Rows[i].Cells["itemValue"].Value == null ? "" : this.Rows[i].Cells["itemValue"].Value.ToString();
                #endregion
            }              
        }
        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public abstract int Save();
        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public int Save(string ICP0)
        {
            this._oilInfo.ICP0 = ICP0;
            return Save();
        }
        /// <summary>
        /// 剪贴
        /// </summary>
        public void Cut(object sender, EventArgs e)
        {
            cCutMenu_Click(sender, e);
            this.isChanged = true;
        }
        /// <summary>
        /// 粘贴
        /// </summary>
        public void Copy(object sender, EventArgs e)
        {
            cCopyMenu_Click(sender, e); this.isChanged = true;
        }
        /// <summary>
        /// 粘贴
        /// </summary>
        public void Paste(object sender, EventArgs e)
        {
            this.isChanged = true;
            cPasteMenu_Click(sender, e);
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void Delete(object sender, EventArgs e)
        {
            cDeleteMenu_Click(sender, e); this.isChanged = true;
        }
        /// <summary>
        /// 保存
        /// </summary>
        public void Save(object sender, EventArgs e)
        {
            cSaveMenu_Click(sender, e); this.isChanged = false ;
        }    
        #endregion

        #region 私有函数
        /// <summary>
        /// 
        /// </summary>
        private void _setColHeader()
        {
            this.Columns.Clear();
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                HeaderText = "项目",
                Name = "itemName",
                Visible = this._tableType.itemNameShow,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                HeaderText = "英文名称",
                Name = "itemEnName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Visible = this._tableType.itemEnShow,
                SortMode = DataGridViewColumnSortMode.NotSortable,

            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                ReadOnly = true,
                HeaderText = "代码",
                Name = "itemCode",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                Visible = this._tableType.itemCodeShow,
                SortMode = DataGridViewColumnSortMode.NotSortable,

            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "值",
                Name = "itemValue",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 200,
                SortMode = DataGridViewColumnSortMode.NotSortable,

            });
        }
        
        /// <summary>
        /// 设置行头
        /// </summary>
        private void _setRowHeader()
        {
            this.Rows.Clear();

            if (this._oilTableRows == null)
                return;
            //this.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            foreach (var r in this._oilTableRows)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = r.itemCode;//代码绑定到行头
                row.CreateCells(this, r.itemName, r.itemEnName, r.itemCode);
                row.Visible = r.isDisplay;
                this.Rows.Add(row);
            }
        }
        /// <summary>
        /// 设置行头
        /// </summary>
        private void _setRowHeaderB()
        {
            this.Rows.Clear();

            if (this._oilTableRows == null)
                return;
            List<string> itemCodeList = new List<string>();
            itemCodeList.Add("CNA"); itemCodeList.Add("ENA"); itemCodeList.Add("IDC"); itemCodeList.Add("COU"); itemCodeList.Add("GRC");
            itemCodeList.Add("ADA"); itemCodeList.Add("UDD"); itemCodeList.Add("SR"); itemCodeList.Add("CLA"); itemCodeList.Add("TYP");
            itemCodeList.Add("SCL"); itemCodeList.Add("ACL"); itemCodeList.Add("DQU");
            //List<OilTableRowEntity> rowlist = this._oilTableRows.Where(o => o.itemCode == itemCodeList.Where(t=>t == o.itemCode)).ToList();
            var temp = from item in this._oilTableRows
                       join str  in itemCodeList 
                       on  item.itemCode  equals str
                       select item;

            List<OilTableRowEntity> rowlist = temp.ToList().OrderBy(o=>o.itemOrder).ToList();
            foreach (var r in rowlist)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = r.itemCode;//代码绑定到行头
                row.CreateCells(this, r.itemName, r.itemEnName, r.itemCode);
                row.Visible = r.isDisplay;
                this.Rows.Add(row);
            }
        }

        /// <summary>
        /// 设置单元格的值
        /// </summary>
        private void _setCellValues()
        {
            if (this._oilInfo == null)
                _oilInfo = new TOilInfo();
            List<OilTableRowEntity> rowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Info).ToList();
            for (int i = 0; i < this.Rows.Count; i++)
            {
                DataGridViewRow row = this.Rows[i];
                OilTableRowEntity rowEntity = rowList.Where(o => o.itemCode == row.Tag.ToString()).FirstOrDefault();
                #region "10"
                if (row.Tag.ToString() == "CNA")//原油名称
                {                   
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.crudeName;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "ENA")//原油名称
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.englishName;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "IDC")//原油编号
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.crudeIndex;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "COU")//产地国家
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.country;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "GRC")//地理区域
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.region;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "FBC")//油田区块
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.fieldBlock;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "SS")//采样地点
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.sampleSite;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "ASC")//样品来源
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.assayCustomer;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "BLN")//样品信息
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.BlendingType;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "SDA")//采样日期
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.sampleDate == null ? "" : _oilInfo.sampleDate.Value.ToString(dateFormat);
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                #endregion 

                #region "10"
                else if (row.Tag.ToString() == "RDA")//到院日期
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.receiveDate == null ? "" : _oilInfo.receiveDate.Value.ToString(dateFormat);
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "ADA")//评价日期
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.assayDate;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "UDD")//入库日期
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.updataDate ;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "ALA")//评价单位
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.assayLab;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "AER")//评价人员
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.assayer;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "RIN")//报告号
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.reportIndex;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "SRI")//数据资源
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.DataSource;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "SR")//数据来源
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.sourceRef;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "DQU")//数据质量
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.DataQuality;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "SUM")//评论
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.summary;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                #endregion 

                #region "10"
                else if (row.Tag.ToString() == "CLA")//类别
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.type;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "TYP")//基数
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.classification;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "SCL")//硫水平
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.sulfurLevel;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "ACL")//酸水平
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.acidLevel;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "COL")//腐蚀指数
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.corrosionLevel;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "PRI")//加工指数
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.processingIndex;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "NIR")//NIR光谱
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.NIRSpectrum;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "REM")//NIR光谱
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.Remark;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "01R")//补充信息1
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_01R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "02R")//补充信息2
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_02R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                #endregion 

                #region "10"
                else if (row.Tag.ToString() == "03R")//补充信息3
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_03R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "04R")//补充信息4
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_04R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "05R")//补充信息5
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_05R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "06R")//补充信息6
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_06R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "07R")//补充信息7
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_07R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "08R")//补充信息8
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_08R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "09R")//补充信息9
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_09R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                else if (row.Tag.ToString() == "10R")//补充信息10
                {
                    this.Rows[i].Cells["itemValue"].Value = _oilInfo.S_10R;
                    (this.Rows[i].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength = rowEntity.valDigital;
                }
                #endregion
            }       
        }

        #region "oldCode"
        /// <summary>
        /// 设置单元格值
        /// </summary>
        //private void _setCellValues()
        //{
        //    if (this._oilInfo == null)
        //        return;
        //    OilTableRowBll bll = new OilTableRowBll();
        //    List<OilTableRowEntity> oilTableRows = bll.Where(r => r.oilTableTypeID == (int)EnumTableType.Info).ToList();
        //    int i = 0;

        //    DataGridViewRow row = new DataGridViewRow();
        //    row.CreateCells(this, "原油名称", "Crude Name","CNA", this._oilInfo.crudeName);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "英文名称", "English Name", "ENA", this._oilInfo.englishName);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "原油编号", "Crude Index", "ID", this._oilInfo.crudeIndex);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "产地国家", "Original Country", "COU", this._oilInfo.country);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    row.ReadOnly = true;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "地理区域", "Geographical Region", "GRC", this._oilInfo.region);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    row.ReadOnly = true;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "油田区块", "Field Block", "FBC", this._oilInfo.fieldBlock);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "采样日期", "Sample Date", "SDA", this._oilInfo.sampleDate == DateTime.Parse("1900-1-1") ? "" : this._oilInfo.sampleDate.ToShortDateString());
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    row.ReadOnly = true;

        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "到院日期", "Receive Date", "RDA", this._oilInfo.receiveDate == DateTime.Parse("1900-1-1") ? "" : this._oilInfo.sampleDate.ToShortDateString());
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    row.ReadOnly = true;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "采样地点", "Sample Site", "SS", this._oilInfo.sampleSite);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "评价日期", "Assay Date", "ADA", this._oilInfo.assayDate == DateTime.Parse("1900-1-1") ? "" : this._oilInfo.sampleDate.ToShortDateString());
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    row.ReadOnly = true;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "更新日期", "Updata Date", "UDD", this._oilInfo.updataDate == DateTime.Parse("1900-1-1") ? "" : this._oilInfo.sampleDate.ToShortDateString());
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    row.ReadOnly = true;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "数据来源", "Source Reference", "SR", this._oilInfo.sourceRef);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "评价单位", "Assay Lab", "AL", this._oilInfo.assayLab);
        //    row.ReadOnly = true;
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "评价人员", "Assayer", "AER", this._oilInfo.assayer);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "评价甲方", "Assay Customer", "AC", this._oilInfo.assayCustomer);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "报告号", "Report Index", "RIN", this._oilInfo.reportIndex);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "评论", "Summary", "SUM", this._oilInfo.summary);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "类别", "Crude Oil Type", "COT", this._oilInfo.type);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "基属", "Classification", "TYP", this._oilInfo.classification);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "硫水平", "Sulfur Level", "SCL", this._oilInfo.sulfurLevel);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "酸水平", "Acid Level", "ACL", this._oilInfo.acidLevel);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "腐蚀指数", "Corrosion Level", "COI", this._oilInfo.corrosionLevel);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //    row = new DataGridViewRow();
        //    row.CreateCells(this, "加工指数", "Processing Index", "PRI", this._oilInfo.processingIndex);
        //    row.Visible = oilTableRows[i++].isDisplay;
        //    this.Rows.Add(row);

        //}
        #endregion 

        /// <summary>
        /// 表格样式
        /// </summary>
        private void InitStyle()
        {
            this.AllowUserToAddRows = false;
            this.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            this.DefaultCellStyle = myStyle.dgdViewCellStyle2();

            this.BorderStyle = BorderStyle.None;
            this.RowHeadersWidth = 30;
            this.MultiSelect = false;
            this.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }


        #endregion

        #region 编辑
        /// <summary>
        /// 复制数据到剪切板
        /// </summary>
        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = this.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);

        }
        /// <summary>
        /// 删除选中的单元格的值
        /// </summary>
        private void DeleteValue(string warning)
        {
            //Show Error if no cell is selected
            if (this.SelectedCells.Count == 0)
            {
                MessageBox.Show("请选择数据！", warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (this.CurrentCell == null || this.CurrentCell.Value == null)
            {
                return;
            }
            if (this.CurrentCell.ColumnIndex == 3)
            {
                if (this.CurrentCell != null && this.CurrentCell.Value != null)
                {
                    if (this.CurrentCell.RowIndex == 2 && this.CurrentCell.ColumnIndex == 3)
                    {
                        MessageBox.Show("原油编号不可" + warning + "！");
                        return;
                    }
                    this.CurrentCell.Value = null;
                }
            }
            else
            {
                MessageBox.Show("不可编辑！", "编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        /// <summary>
        /// 粘贴数据到剪贴板
        /// </summary>
        private void PasteClipboardValue()
        {
            //Show Error if no cell is selected
            if (this.SelectedCells.Count == 0)
            {
                MessageBox.Show("请选择数据！", "粘贴", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<double> abstra = new List<double>();

            string value = Clipboard.GetText();//从剪贴板上获取字符串
            if (this.CurrentCell.ColumnIndex == 3)
            {
                if (value != string.Empty)
                {
                    int length =  (this.Rows[this.CurrentCell.RowIndex].Cells["itemValue"] as DataGridViewTextBoxCell).MaxInputLength  ;
                    if (value.Length > length)
                        this.CurrentCell.Value = value.Substring(0, length);
                    else
                        this.CurrentCell.Value = value;
                }
                else if (value == string.Empty)
                {
                    this.CurrentCell.Value = null;
                }   
            }
            else
            {
                MessageBox.Show("不可编辑！", "编辑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        /// <summary>
        /// 剪切
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cCutMenu_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
            DeleteValue("剪贴");
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cCopyMenu_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }
        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cPasteMenu_Click(object sender, EventArgs e)
        {
            PasteClipboardValue();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cDeleteMenu_Click(object sender, EventArgs e)
        {
            DeleteValue("删除");
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cSaveMenu_Click(object sender, EventArgs e)
        {
            if (this.isChanged)
            {
                Save();
                this.isChanged = false;
            }            
        }
        /// <summary>
        /// 编辑快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridOilInfo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.X:
                            CopyToClipboard();
                            DeleteValue("剪贴");
                            break;
                        case Keys.C:
                            CopyToClipboard();
                            break;
                        case Keys.V:
                            PasteClipboardValue();
                            break;
                        case Keys.S:
                            cSaveMenu_Click(sender, e);
                            break;
                    }
                }
                if (e.KeyCode == Keys.Delete)
                {
                    DeleteValue("删除");
                }
            }
            catch (Exception ex)
            {
                Log.Error("原油信息表的编辑:"+ ex.ToString());
                //MessageBox.Show("Copy/paste operation failed. " + ex.Message, "Copy/Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        /// <summary>
        /// 数据检查检查关键项，数据格式及范围审查
        /// </summary>
        /// <returns></returns>
        public string DataCheck()
        {
            this.EndEdit();
            StringBuilder sbAlert = new StringBuilder();
            OilDataCheck dataCheck = new OilDataCheck();
            string itemCode;
            OilTableRowBll oilTableRowBll = new OilTableRowBll();

            foreach (DataGridViewRow row in this.Rows)
            {
                itemCode = row.Cells["itemCode"].Value.ToString().Trim();

                OilTableRowEntity oilTableRowEntity = oilTableRowBll[itemCode, EnumTableType.Info];

                if (oilTableRowEntity.isKey)
                {
                    if (string.IsNullOrEmpty(row.Cells["itemValue"].Value == null ? "" : row.Cells["itemValue"].Value.ToString().Trim()))
                    {
                        sbAlert.Append(dataCheck.CheckMetion(row.Cells["itemName"].Value.ToString(), row.Index, enumCheckErrType.Blank));
                        row.Cells["itemValue"].Style = myStyle.dgdViewCellStyleErr();
                    }
                }
                else if (oilTableRowEntity.dataType.ToUpper() == "DATETIME")
                {
                    if (row.Cells["itemValue"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["itemValue"].Value.ToString()) )
                    {
                        if (!dataCheck.checkDate(row.Cells["itemValue"].Value.ToString().Trim()))
                        {
                            sbAlert.Append(dataCheck.CheckMetion(row.Cells["itemName"].Value.ToString(), row.Index, enumCheckErrType.TypeError));
                            row.Cells["itemValue"].Style = myStyle.dgdViewCellStyleErr();
                        }
                    }
                }
            }
            return sbAlert.ToString();
        }
       

    }
}
