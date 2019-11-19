using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.OilDB.Data;
using System.Threading;
using System.IO;
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.OilDB.BLL;
namespace RIPP.App.OilDataManager.Forms.OilAudit
{
    /// <summary>
    /// 核算审查窗体
    /// </summary>
    public partial class FrmAccountAuditCopy : Form
    {
        #region "私有函数"
        //private OilInfoEntity _oilA = null;
        //private IList<OilDataEntity> _datas = new List<OilDataEntity>();
        private Dictionary<string, string> Name_ItemCodeDic = new Dictionary<string, string>();//名称和代码的对应字典
        
        private string bCmbText = string.Empty;
        private Dictionary <string,List<CutMothedEntity>> _Dic = new Dictionary<string,List<CutMothedEntity>> ();
        private string st = System.Windows.Forms.Application.StartupPath;
        
        private OilTools tool = new OilTools();

        /// <summary>
        /// 传递过来需要审查的原油性质窗体
        /// </summary>
        private GridOilViewA _wholeGridOil = null;
        /// <summary>
        /// 传递过来需要审查的窄馏分窗体
        /// </summary>
        private GridOilViewA _narrowGridOil = null;
        /// <summary>
        /// 传递过来需要审查的宽馏分窗体
        /// </summary>
        private GridOilViewA _wideGridOil = null;
        /// <summary>
        /// 传递过来需要审查的渣馏分窗体
        /// </summary>
        private GridOilViewA _residueGridOil = null;
        ConfigBll config = new ConfigBll();
        #endregion 

        #region"构造函数"
        /// <summary>
        /// 
        /// </summary>
        public FrmAccountAuditCopy()
        {
            InitializeComponent();
            //this.groupBox7.MouseDown += groupBox7_MouseDown;
            //this.groupBox7.MouseUp += groupBox7_MouseUp;
            ////initcmbTableName();
            //initAllCutMothed(st + "\\outFile\\default.aud");

            //this.tableLayoutPanel1.Visible = true;
            //this.palShow.Visible = false;
            //this.tableLayoutPanel1.Dock = DockStyle.Fill;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oil"></param>
        public FrmAccountAuditCopy(OilInfoEntity oil)
        {
            InitializeComponent();
            this.groupBox7.MouseDown += groupBox7_MouseDown;
            this.groupBox7.MouseUp += groupBox7_MouseUp;
            this.palShow.Visible = false;
            //this._datas = oil.OilDatas;
            initcmbTableName();
            initLeftShowListView();

            initAllCutMothed(config.getDir (enumModel.ManAud));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oil"></param>
        public FrmAccountAuditCopy(GridOilViewA wholeGridOil, GridOilViewA narrowGridOil, GridOilViewA wideGridOil, GridOilViewA residueGridOil)
        {
            InitializeComponent();
            this.groupBox7.MouseDown += groupBox7_MouseDown;
            this.groupBox7.MouseUp += groupBox7_MouseUp;
            this.palShow.Visible = false;
            this._wholeGridOil = wholeGridOil;
            this._narrowGridOil = narrowGridOil;
            this._wideGridOil = wideGridOil;
            this._residueGridOil = residueGridOil;

            initcmbTableName();
            initLeftShowListView();

            initAllCutMothed(config.getDir(enumModel.ManAud));
            
            this.tableLayoutPanel1.Visible = true;
            this.palShow.Visible = false;
            this.tableLayoutPanel1.Dock = DockStyle.Fill;
        }
        #endregion
       
        void groupBox7_MouseUp(object sender, MouseEventArgs e)
        {
             
        }

        void groupBox7_MouseDown(object sender, MouseEventArgs e)
        {
            bool isContain = this.groupBox7.Controls["tabControlEx1"].Bounds.Contains(e.X, e.Y);
            if (isContain && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.cMSAll.Show(this, e.X, e.Y);
            }              
        }
        
        /// <summary>
        /// 初始化显示列表
        /// </summary>
        private void initLeftShowListView()
        {
            Action ac =()=>
            {
                if (this.lvAccLeftShow.InvokeRequired)
                {
                    ThreadStart ss = () =>
                    {
                        AccountParmTableAccess accountParmTableAccess = new AccountParmTableAccess();
                        List<AccountParmTableEntity> AccountParmList = accountParmTableAccess.Get("1=1");
                        foreach (AccountParmTableEntity accParm in AccountParmList)
                        {
                            ListViewItem item = new ListViewItem();
                            for (int colIndex = 0; colIndex < this.lvAccLeftShow.Columns.Count; colIndex++)
                            {
                                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                                temp.Name = this.lvAccLeftShow.Columns[colIndex].Name;
                                item.SubItems.Add(temp);
                            }

                            item.SubItems[0].Text = accParm.itemName;
                            item.SubItems[0].Tag = accParm.itemCode;
                            this.lvAccLeftShow.Items.Add(item);
                        }      
                    };

                    this.lvAccLeftShow.Invoke(ss);
                }
                else
                {
                    AccountParmTableAccess accountParmTableAccess = new AccountParmTableAccess();
                    List<AccountParmTableEntity> AccountParmList = accountParmTableAccess.Get("1=1");
                    foreach (AccountParmTableEntity accParm in AccountParmList)
                    {
                        ListViewItem item = new ListViewItem();
                        for (int colIndex = 0; colIndex < this.lvAccLeftShow.Columns.Count; colIndex++)
                        {
                            ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                            temp.Name = this.lvAccLeftShow.Columns[colIndex].Name;
                            item.SubItems.Add(temp);
                        }

                        item.SubItems[0].Text = accParm.itemName;
                        item.SubItems[0].Tag = accParm.itemCode;
                        this.lvAccLeftShow.Items.Add(item);
                    }              
                }
            };
            ac.BeginInvoke(null, null);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void initcmbTableName()
        {
            this.cmbTableName.Items.Add(EnumTableType.Narrow.GetDescription());
            this.cmbTableName.Items.Add(EnumTableType.Wide.GetDescription());
            this.cmbTableName.Items.Add(EnumTableType.Residue.GetDescription());
            this.cmbTableName.SelectedIndex = 0;          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<AccountAuditEntity> getAccountAuditList(EnumTableType tableType)
        {
            List<AccountAuditEntity> list = new List<AccountAuditEntity>();

            if (tableType == EnumTableType.Wide)
            {
                List<OilDataEntity> ICPList = this._wideGridOil._datas.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableTypeID == (int)tableType && o.calData != string.Empty).ToList();
                List<OilDataEntity> ECPList = this._wideGridOil._datas.Where(o => o.OilTableRow.itemCode == "ECP" && o.OilTableTypeID == (int)tableType && o.calData != string.Empty).ToList();
                foreach (var ICPData in ICPList)
                {
                    if (ICPData == null)
                        continue;

                    OilDataEntity ECPData = ECPList.Where(o => o.OilTableCol.colCode == ICPData.OilTableCol.colCode).FirstOrDefault();
                    if (ECPData == null)
                        continue;

                    string text = ICPData.calShowData + " --- " + ECPData.calShowData;

                    list.Add(new AccountAuditEntity(text, ICPData.calShowData, ECPData.calShowData));
                }
            }
            else if (tableType == EnumTableType.Residue)
            {
                List<OilDataEntity> ICPList = this._residueGridOil._datas.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableTypeID == (int)tableType && o.calData != string.Empty).ToList();

                foreach (var ICPData in ICPList)
                {
                    if (ICPData == null)
                        continue;

                    string text = " > " + ICPData.calShowData;

                    list.Add(new AccountAuditEntity(text, ICPData.calShowData, ""));
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbTableName.Text == EnumTableType.Narrow.GetDescription())
            {
                this.cmbICPECP.Visible = false;
                this.txtICP.Visible = true;
                this.txtECP.Visible = true;
                this.labICP_ECP.Visible = true;              
            }
            else if (this.cmbTableName.Text == EnumTableType.Wide.GetDescription())
            {
                this.cmbICPECP.Visible = true;
                this.txtICP.Visible = false;
                this.txtECP.Visible = false;
                this.labICP_ECP.Visible = false; 

                this.cmbICPECP.DataSource = getAccountAuditList(EnumTableType.Wide);
                this.cmbICPECP.ValueMember = "Text";
                this.cmbICPECP.DisplayMember = "Text"; 
                this.cmbICPECP.SelectedIndex = 0;
            }
            else if (this.cmbTableName.Text == EnumTableType.Residue.GetDescription())
            {
                this.cmbICPECP.Visible = true;
                this.txtICP.Visible = false;
                this.txtECP.Visible = false;
                this.labICP_ECP.Visible = false; 

                this.cmbICPECP.DataSource = getAccountAuditList(EnumTableType.Residue);
                this.cmbICPECP.ValueMember = "Text";
                this.cmbICPECP.DisplayMember = "Text";
                this.cmbICPECP.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// 添加切割方案
        /// </summary>
        /// <param name="name"></param>
        TabPage addTabPage(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            string newName = name;
            int i = 0;
            while (this.tabControlEx1.TabPages.ContainsKey(newName))
            {
                newName = name + "(" + i + ")";
                i++;
            }
            name = newName;
            #region 
            // 
            // 馏分名称
            // 
            ColumnHeader columnHeaderName = new ColumnHeader
            {
                Text = "馏分名称",
                TextAlign = HorizontalAlignment.Center,
                Width = 80
            };

            // 
            // :
            // 
            ColumnHeader columnHeader = new ColumnHeader
            {
                Text = ":",
                TextAlign = HorizontalAlignment.Center,
                Width = 20
            };

            // 
            // ICP
            // 
            ColumnHeader columnHeaderICP = new ColumnHeader
            {
                Text = "ICP",
                TextAlign = HorizontalAlignment.Center,
                Width = 60
            };
            // 
            // -
            // 
            ColumnHeader columnHeader_ = new ColumnHeader
            {
                Text = "-",
                TextAlign = HorizontalAlignment.Center,
                Width = 20
            };

            // 
            // ECP
            // 
            ColumnHeader columnHeaderECP = new ColumnHeader
            {
                Text = "ECP",
                TextAlign = HorizontalAlignment.Center,
                Width = 60
            };
            #endregion 

            ListView newListView = new ListView()
            {
                //ContextMenuStrip = this.cMS,
                FullRowSelect = true,
                GridLines = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Location = new System.Drawing.Point(3, 3),
                Name = name,
                UseCompatibleStateImageBehavior = false,
                Dock = DockStyle.Fill,
                View = View.Details,

            };
            newListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeaderName,
            columnHeader,
            columnHeaderICP,
            columnHeader_,
            columnHeaderECP});


            TabPage tp = new TabPage()
            {
                Name = name,
                Text = name
            };          
            tp.Controls.Add(newListView);


            if (!this.tabControlEx1.TabPages.ContainsKey(tp.Name))
                this.tabControlEx1.TabPages.Add(tp);

            this.tabControlEx1.SelectedTab = tp;

            return tp;
        }

        /// <summary>
        /// 添加切割方案
        /// </summary>
        /// <param name="name"></param>
        void addTabPage(string name, List<CutMothedEntity> cutMothedList)
        {
            TabPage tp = addTabPage(name);

            var newListView = this.tabControlEx1.SelectedTab.Controls[this.tabControlEx1.SelectedTab.Text] as ListView;
            if (newListView == null)
                return;

            #region "将查询条件赋值"
            foreach (var cutMothed in cutMothedList)
            {
                ListViewItem Item = new ListViewItem();
                for (int colIndex = 0; colIndex < newListView.Columns.Count; colIndex++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    Item.SubItems.Add(temp);

                    #region
                    switch (colIndex)
                    {
                        case 0:
                            Item.SubItems[0].Name = "表名称";
                            break;
                        case 1:
                            Item.SubItems[1].Name = "表名称:ICP";
                            Item.SubItems[1].Text = ":";
                            break;
                        case 2:
                            Item.SubItems[2].Name = "ICP";
                            break;
                        case 3:
                            Item.SubItems[3].Name = "ICP-ECP";
                            Item.SubItems[3].Text = "-";
                            break;
                        case 4:
                            Item.SubItems[4].Name = "ECP";
                            break;
                    }
                    #endregion
                }

                Item.SubItems["表名称"].Text = cutMothed.Name;
                Item.SubItems["ICP"].Text = cutMothed.strICP;
                
                if (string.IsNullOrWhiteSpace(cutMothed.strICP))
                {
                    OilDataEntity ICPData = this._narrowGridOil.GetDataByRowItemCodeColumnIndex("ICP", 0);
                    Item.SubItems["ICP"].Tag = ICPData == null ? "-50" : ICPData.calShowData;
                }
               
                Item.SubItems["ECP"].Text = cutMothed.strECP;
                newListView.Items.Add(Item);
            }
            #endregion
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            #region "输入判断"
            if (this.tabControlEx1.TabPages.Count <= 0)
                return ;

            var listView = this.tabControlEx1.SelectedTab.Controls[this.tabControlEx1.SelectedTab.Text] as ListView;
            if (listView == null)
                return;
            
            string strICP = string.Empty;
            if (this.cmbTableName.Text == EnumTableType.Narrow.GetDescription())//非 
            {
                #region 
                if (string.IsNullOrEmpty(txtECP.Text))
                {
                    MessageBox.Show("馏分范围不能为空！", "提示信息");
                    return;
                }

                if (string.IsNullOrEmpty(txtICP.Text))
                {
                    OilDataEntity ICPData = this._narrowGridOil.GetDataByRowItemCodeColumnIndex("ICP", 0);
                    strICP = ICPData == null ? "-50" : ICPData.calShowData;
                }
                else
                {
                    strICP = this.txtICP.Text.Trim();
                }
                float tempICP = 0;
                if (float.TryParse(strICP, out tempICP))
                {
                }
                else
                {
                    MessageBox.Show("初切点必须为数字！", "提示信息");
                    txtICP.Focus();
                    return;
                }

                int tempECP = 0;
                if (Int32.TryParse(txtECP.Text, out tempECP))
                {
                }
                else
                {
                    MessageBox.Show("终切点必须为数字！", "提示信息");
                    txtECP.Focus();
                    return;
                }

                if (tempICP >= tempECP)
                {
                    MessageBox.Show("初切点必须小于终切点！", "提示信息");
                    return;
                }
                #endregion 
                
                foreach (ListViewItem item in listView.Items)
                {
                    if (item.SubItems["表名称"].Text.Equals(cmbTableName.Text)
                         && item.SubItems["ICP"].Text.Equals(txtICP.Text)
                         && item.SubItems["ECP"].Text.Equals(txtECP.Text))
                    {
                        MessageBox.Show("查询条件已经存在，请重新选择！", "提示信息");
                        return;
                    }
                }
            }
            else //if (this.cmbTableName.Text == EnumTableType.Wide.GetDescription())//非 
            {
                var temp = (AccountAuditEntity)this.cmbICPECP.SelectedItem;

                foreach (ListViewItem item in listView.Items)
                {
                    if (item.SubItems["表名称"].Text.Equals(cmbTableName.Text)
                         && item.SubItems["ICP"].Text.Equals(temp.strICP)
                         && item.SubItems["ECP"].Text.Equals(temp.strECP))
                    {
                        MessageBox.Show("查询条件已经存在，请重新选择！", "提示信息");
                        return;
                    }
                }
            }
           
            #endregion 

            ListViewItem Item = new ListViewItem();
            for (int colIndex = 0; colIndex < listView.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                Item.SubItems.Add(temp);

                #region
                switch (colIndex)
                {
                    case 0:
                        Item.SubItems[0].Name = "表名称";
                        break;
                    case 1:
                        Item.SubItems[1].Name = "表名称:ICP";
                        Item.SubItems[1].Text = ":";
                        break;
                    case 2:
                        Item.SubItems[2].Name = "ICP";
                        break;
                    case 3:
                        Item.SubItems[3].Name = "ICP-ECP";
                        Item.SubItems[3].Text = "-";
                        break;
                    case 4:
                        Item.SubItems[4].Name = "ECP";
                        break;
                }
                #endregion
            }

            if (this.cmbTableName.Text == EnumTableType.Narrow.GetDescription())//非 
            {
                Item.SubItems["表名称"].Text = this.cmbTableName.Text;
                Item.SubItems["ICP"].Text = this.txtICP.Text.Trim();
                Item.SubItems["ICP"].Tag = strICP;
                Item.SubItems["ECP"].Text = this.txtECP.Text.Trim();              
            }
            else //if (this.cmbTableName.Text == EnumTableType.Wide.GetDescription())//非 
            {
                var temp = (AccountAuditEntity)this.cmbICPECP.SelectedItem;

                Item.SubItems["表名称"].Text = this.cmbTableName.Text;
                Item.SubItems["ICP"].Text = temp.strICP; 
                Item.SubItems["ECP"].Text = temp.strECP; 
            }

            listView.Items.Add(Item);
            
        }
        /// <summary>
        /// 删除选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (this.tabControlEx1.TabPages.Count <= 0)
                return;

            var listView = this.tabControlEx1.SelectedTab.Controls[this.tabControlEx1.SelectedTab.Text] as ListView;
            if (listView == null)
                return;

            if (listView.Items.Count > 0)
                if (listView.SelectedItems["表名称"] != null)
            {
                listView.Items.Remove(listView.SelectedItems["表名称"]);
            }
        }

      
        private void butShowAddAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectItem in this.lvAccLeftShow.Items)
            {
                ListViewItem item = new ListViewItem();
                for (int colIndex = 0; colIndex < this.lvAccRightShow.Columns.Count; colIndex++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    temp.Name = this.lvAccRightShow.Columns[colIndex].Name;
                    item.SubItems.Add(temp);
                }
                if (selectItem != null)
                {
                    item.SubItems[0].Text = selectItem.SubItems[0].Text;
                    item.SubItems[0].Tag = selectItem.SubItems[0].Tag;

                    this.lvAccRightShow.Items.Add(item);
                    this.lvAccLeftShow.Items.Remove(selectItem);
                }
            }
        }

        private void butShowAdd_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            for (int colIndex = 0; colIndex < this.lvAccLeftShow.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                temp.Name = this.lvAccRightShow.Columns[colIndex].Name;
                item.SubItems.Add(temp);
            }
            if (this.lvAccLeftShow.SelectedItems.Count > 0)
            {
                item.SubItems[0].Tag = this.lvAccLeftShow.SelectedItems[0].SubItems[0].Tag;
                item.SubItems[0].Text = this.lvAccLeftShow.SelectedItems[0].SubItems[0].Text;
 

                this.lvAccRightShow.Items.Add(item);
                this.lvAccLeftShow.Items.Remove(this.lvAccLeftShow.SelectedItems[0]);
            }
        }

        private void butShowDel_Click(object sender, EventArgs e)
        {
            if (this.lvAccRightShow.SelectedItems.Count > 0)
            {
                if (this.lvAccRightShow.SelectedItems[0] != null)
                {
                    ListViewItem item = new ListViewItem();
                    for (int colIndex = 0; colIndex < this.lvAccLeftShow.Columns.Count; colIndex++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = this.lvAccLeftShow.Columns[colIndex].Name;
                        item.SubItems.Add(temp);
                    }
                    item.SubItems[0].Tag = this.lvAccRightShow.SelectedItems[0].SubItems[0].Tag;
                    item.SubItems[0].Text = this.lvAccRightShow.SelectedItems[0].SubItems[0].Text;

                    this.lvAccLeftShow.Items.Add(item);
                    this.lvAccRightShow.Items.Remove(this.lvAccRightShow.SelectedItems[0]);
                }
            }
        }

        private void butShowDelAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectItem in this.lvAccRightShow.Items)
            {
                ListViewItem item = new ListViewItem();
                for (int colIndex = 0; colIndex < this.lvAccLeftShow.Columns.Count; colIndex++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    temp.Name = this.lvAccLeftShow.Columns[colIndex].Name;
                    item.SubItems.Add(temp);
                }

                item.SubItems[0].Text = selectItem.SubItems[0].Text;
                item.SubItems[0].Tag = selectItem.SubItems[0].Tag;

                this.lvAccLeftShow.Items.Add(item);
                this.lvAccRightShow.Items.Remove(selectItem);
            }
        }

        
        /// <summary>
        /// 提取切割条件集合
        /// </summary>
        /// <param name="targetLv"></param>
        /// <returns></returns>
        private List<CutMothedEntity> getCutMothedList(ListView targetLv)
        {
            #region "提取切割实体"
            List<CutMothedEntity> CutMothedList = new List<CutMothedEntity>();
            for (int index = 0; index < targetLv.Items.Count; index++)
            {
                string strCutName = targetLv.Items[index].SubItems["表名称"].Text;
                string strICP = targetLv.Items[index].SubItems["ICP"].Text == string.Empty ? targetLv.Items[index].SubItems["ICP"].Tag.ToString() : targetLv.Items[index].SubItems["ICP"].Text;
                string strECP = targetLv.Items[index].SubItems["ECP"].Text;

                CutMothedEntity cutMethed = new CutMothedEntity();
                cutMethed.Name = strCutName;
                cutMethed.CutType = bCmbText;

                if (cutMethed.Name == EnumTableType.Residue.GetDescription())
                {
                    cutMethed.strICP = strICP.Trim();
                }
                else
                {
                    cutMethed.strICP = strICP.Trim();
                    cutMethed.strECP = strECP.Trim();
                }
                CutMothedList.Add(cutMethed);
            }
            #endregion

            return CutMothedList;
        }
        /// <summary>
        /// 审查计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAcc_Click(object sender, EventArgs e)
        {
            #region "判断条件是否符合"
           
            this._Dic.Clear();
            foreach (TabPage page in this.tabControlEx1.TabPages)
            {
                ListView listView = page.Controls[page.Text] as ListView;
                bCmbText = page.Text;
                var cutMothedList = getCutMothedList(listView);

                if (!this._Dic.Keys.Contains(bCmbText))
                    this._Dic.Add(bCmbText, cutMothedList);
            }

            if (this._Dic.Count <= 0)
            {
                MessageBox.Show("缺少核算方案！");
                return;
            }

            if (this.lvAccRightShow.Items.Count <= 0)
            {
                MessageBox.Show("缺少核算性质！");
                return;
            }

            #endregion

            #region 设置显示格式
            this.MaximizeBox = true;
            this.MaximumSize = new Size(0, 0);
            this.MinimumSize = new Size(0, 0);

            this.tableLayoutPanel1.Visible = false;
            this.palShow.Visible = true;
            this.palShow.Dock = DockStyle.Fill;
            #endregion
           
            setDgv(this.dgvShow, this.lvAccRightShow, this._Dic);
            foreach (var colName in this._Dic.Keys)
                fillDataToDgv(this.dgvShow, colName, this._Dic[colName]);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.palShow.Visible = false;
            this.tableLayoutPanel1.Visible = true;
            this.tableLayoutPanel1.Dock = DockStyle.Fill;
            this.MaximumSize = new Size(825, 475);
            this.MinimumSize = new Size(825, 475);
            this.Width = 825 ;
            this.Height = 475;
            this.MaximizeBox = false;
        }


        #region "显示数据"

        private void setDgv(DataGridView targetDgv,ListView lvRows, Dictionary<string, List<CutMothedEntity>> dicCols)
        {
            targetDgv.ReadOnly = true;
            targetDgv.Columns.Clear();
            targetDgv.Rows.Clear();

            targetDgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "itemName", HeaderText = "物性",Frozen = true , AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            foreach (var key in dicCols.Keys)
                targetDgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = key, HeaderText = key, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            foreach (ListViewItem row in lvRows.Items)
            {
                int rowIndex = targetDgv.Rows.Add();
                targetDgv.Rows[rowIndex].Cells["itemName"].Value = row.SubItems[0].Text;
                targetDgv.Rows[rowIndex].Cells["itemName"].Tag = row.SubItems[0].Tag;
            }
        }
        /// <summary>
        /// 计算数据
        /// </summary>
        /// <param name="targetDgv"></param>
        /// <param name="colName"></param>
        /// <param name="cutMothedList"></param>
        private void fillDataToDgv(DataGridView targetDgv,string colName, List<CutMothedEntity> cutMothedList)
        {
            #region "核算性质检查"
            if (targetDgv.Rows.Count == 0)
            {
                MessageBox.Show("缺少核算性质！");
                return;
            }
            #endregion

            List<OilDataEntity> NarrowICPList = this._narrowGridOil.GetDataByRowItemCode("ICP").Where(o => o.calData != string.Empty).ToList(); 
            List<OilDataEntity> NarrowECPList = this._narrowGridOil.GetDataByRowItemCode("ECP").Where(o => o.calData != string.Empty).ToList(); 

            List<OilDataEntity> WideICPList = this._wideGridOil.GetDataByRowItemCode("ICP").Where(o => o.calData != string.Empty).ToList(); 
            List<OilDataEntity> WideECPList = this._wideGridOil.GetDataByRowItemCode("ECP").Where(o => o.calData != string.Empty).ToList(); 

            List<OilDataEntity> ResidueICPList = this._residueGridOil.GetDataByRowItemCode("ICP").Where(o => o.calData != string.Empty).ToList(); 

            #region "数据判断"
            #region " "
            foreach (var cutMothed in cutMothedList)
            {
                if (cutMothed.Name == EnumTableType.Narrow.GetDescription())
                {
                    OilDataEntity oilDataICP = NarrowICPList.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == cutMothed.strICP).FirstOrDefault();
                    if (oilDataICP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show(colName + "方案找不到初切点为" + cutMothed.strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    OilDataEntity oilDataECP = NarrowECPList.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == cutMothed.strECP).FirstOrDefault();
                    if (oilDataECP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show(colName + "方案找不到终切点为" + cutMothed.strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else if (cutMothed.Name == EnumTableType.Wide.GetDescription())
                {
                    OilDataEntity oilDataICP = WideICPList.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == cutMothed.strICP).FirstOrDefault();
                    if (oilDataICP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show(colName + "方案找不到初切点为" + cutMothed.strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    OilDataEntity oilDataECP = WideECPList.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == cutMothed.strECP).FirstOrDefault();
                    if (oilDataECP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show(colName + "方案找不到终切点为" + cutMothed.strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else if (cutMothed.Name == EnumTableType.Residue.GetDescription())
                {
                    OilDataEntity oilDataICP = ResidueICPList.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == cutMothed.strICP).FirstOrDefault();
                    if (oilDataICP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show(colName + "方案找不到初切点为" + cutMothed.strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            #endregion

            #region "判断是否连续"
            bool _IsContinue = true;//判断切割馏分是否连接
            if (cutMothedList.Count > 1)
            {
                for (int cutIndex = 0; cutIndex < cutMothedList.Count - 1; cutIndex++)
                {
                    if (cutMothedList[cutIndex].strECP != cutMothedList[cutIndex + 1].strICP)
                    {
                        _IsContinue = false;
                    }
                }
            }

            if (!_IsContinue)
            {
                MessageBox.Show(colName+"方案选择的切割点不连续！");
                return;
            }
            #endregion

            #endregion           

            #region "计算每个物性的代码"
            if (!this.checkBox.Checked)
                getValueFromContinuousCol(cutMothedList, colName, NarrowICPList, NarrowECPList, WideICPList, WideECPList, ResidueICPList);
            else
                getValueFromDisContinuousCol(cutMothedList,colName , NarrowICPList, NarrowECPList, WideICPList, WideECPList, ResidueICPList);
            #endregion
        }

        #region "连续算法"
        /// <summary>
        /// 连续算法
        /// </summary>
        void getValueFromContinuousCol(List<CutMothedEntity> CutMothedEntityList, string colName, 
            List<OilDataEntity> NarrowICPList, List<OilDataEntity> NarrowECPList, List<OilDataEntity> WideICPList,
             List<OilDataEntity> WideECPList, List<OilDataEntity> ResidueICPList)
        {
            #region "计算每个物性的代码"
         
            for (int rowIndex = 0; rowIndex < this.dgvShow.Rows.Count; rowIndex++)
            {
                float SUM_POR = 0; float SUM_WY = 0; string _POR = string.Empty;//定义返回变量的两个值
                string itemCode = this.dgvShow.Rows[rowIndex].Cells["itemName"].Tag.ToString();

                bool continuous = true;
                if (itemCode == "WY" || itemCode == "VY")
                {
                    #region "CutMothedEntityList"
                    for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count; cutIndex++)
                    {
                        if (CutMothedEntityList[cutIndex].Name == EnumTableType.Narrow.GetDescription())
                        {
                            #region "窄馏分"
                            OilDataEntity oilDataICP = NarrowICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = NarrowECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)//如果查找的数据不存在则返回空
                            {
                                Dictionary<string, float> narrowResult = OilDataCumulationBll.getWYVYCumuationValueNotAllowEmptyFromNarrow(this._narrowGridOil ,CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains(itemCode))
                                    SUM_POR += narrowResult[itemCode];
                                else
                                    continuous = false;
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == EnumTableType.Wide.GetDescription())
                        {
                            #region "宽馏分"
                            OilDataEntity oilDataICP = WideICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = WideECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();
                            if (oilDataICP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到初切点为" + CutMothedEntityList[cutIndex].strICP + "的宽馏分");
                                break;
                            }
                            else if (oilDataECP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到终切点为" + CutMothedEntityList[cutIndex].strECP + "的宽馏分");
                                break;
                            }
                            else if (oilDataICP != null && oilDataECP != null)
                            {
                                Dictionary<string, float> wideResult =OilDataCumulationBll.getWYVYCumuationValueNotAllowEmptyFromWide(this._wideGridOil,CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);
                                if (wideResult.Count > 0 && wideResult.Keys.Contains(itemCode))
                                    SUM_POR += wideResult[itemCode];
                                else
                                    continuous = false;
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == EnumTableType.Residue.GetDescription())
                        {
                            #region "渣油"
                            OilDataEntity oilDataICP = ResidueICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            if (oilDataICP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到初切点为" + CutMothedEntityList[cutIndex].strICP + "的渣油");
                                break;
                            }
                            else
                            {
                                Dictionary<string, float> residueResult = OilDataCumulationBll.getWYVYCumuationValueNotAllowEmptyFromResidue (this._residueGridOil,CutMothedEntityList[cutIndex].strICP, itemCode);
                                if (residueResult.Count > 0 && residueResult.Keys.Contains(itemCode))
                                    SUM_POR += residueResult[itemCode];
                                else
                                    continuous = false;
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region "反函数"
                    if (continuous)
                        this.dgvShow.Rows[rowIndex].Cells[colName].Value = tool.calDataDecLimit(SUM_POR.ToString(), 4);
                    #endregion
                }
                else
                {
                    #region "CutMothedEntityList"
                    for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count; cutIndex++)
                    {
                        if (CutMothedEntityList[cutIndex].Name == EnumTableType.Narrow.GetDescription())
                        {
                            #region "窄馏分"
                            OilDataEntity oilDataICP = NarrowICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = NarrowECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)//如果查找的数据不存在则返回空
                            {
                                Dictionary<string, float> narrowResult = OilDataCumulationBll.getItemCodeCumuationValueNotAllowEmptyFromNarrow (this._narrowGridOil,CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains(itemCode) && narrowResult.Keys.Contains("WY"))
                                {
                                    SUM_POR += narrowResult[itemCode];
                                    SUM_WY += narrowResult["WY"];
                                }
                                else
                                    continuous = false;
                            }
                            else
                                continuous = false;
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == EnumTableType.Wide.GetDescription())
                        {
                            #region "宽馏分"
                            OilDataEntity oilDataICP = WideICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = WideECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)
                            {
                                Dictionary<string, float> wideResult = OilDataCumulationBll.getItemCodeCumuationValueNotAllowEmptyFromWide(this._wideGridOil,CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);
                                if (wideResult.Count > 0 && wideResult.Keys.Contains(itemCode) && wideResult.Keys.Contains("WY"))
                                {
                                    SUM_POR += wideResult[itemCode];
                                    SUM_WY += wideResult["WY"];
                                }
                                else
                                    continuous = false;
                            }
                            else
                                continuous = false;
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == EnumTableType.Residue.GetDescription())
                        {
                            #region "渣油"
                            OilDataEntity oilDataICP = ResidueICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();

                            if (oilDataICP != null)
                            {
                                Dictionary<string, float> residueResult = OilDataCumulationBll.getItemCodeCumuationValueNotAllowEmptyFromResidue(this._residueGridOil ,CutMothedEntityList[cutIndex].strICP, itemCode);
                                if (residueResult.Count > 0 && residueResult.Keys.Contains(itemCode) && residueResult.Keys.Contains("WY"))
                                {
                                    SUM_POR += residueResult[itemCode];
                                    SUM_WY += residueResult["WY"];
                                }
                                else
                                    continuous = false;
                            }
                            else
                                continuous = false;
                            #endregion
                        }
                    }
                    #endregion

                    #region "反函数"
                    if (SUM_WY == 0)
                        continue;

                    float temp = SUM_POR / SUM_WY;
                    _POR = BaseFunction.InverseIndexFunItemCode(temp.ToString(), itemCode);
                    if (continuous)
                        this.dgvShow.Rows[rowIndex].Cells[colName].Value = tool.calDataDecLimit(_POR, 4);
                    #endregion
                }
            }
            #endregion
        }
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">窄馏分的ICP</param>
        /// <param name="strECP">窄馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunNarrowStartEndTotal(string strICP, string strECP, string strItemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (oilDatasNarrow.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }


            List<OilDataEntity> WYDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null || ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (WYDatas.Count <= 0 || ItemCodeoDatas.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            bool Bbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                if (oilDataWY == null || oilDataItemCode == null)
                {
                    Bbreak = true;//计算过程不能为空，为空则跳出
                    break;
                }
                else
                {
                    float wyCal = 0; float itemCodeCal = 0;
                    if (float.TryParse(oilDataWY.calShowData, out wyCal) && float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                    {
                        string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calData, oilDataItemCode.OilTableRow.itemCode);
                        float fTtemp = 0;
                        if (float.TryParse(strTemp, out fTtemp) && strTemp != string.Empty)
                        {
                            SUM_POR = SUM_POR + wyCal * fTtemp;
                            SUM_WY = SUM_WY + wyCal;
                        }
                        else
                        {
                            Bbreak = true;
                            break;
                        }
                    }
                    else
                    {
                        Bbreak = true;
                        break;
                    }
                }
            }

            if (Bbreak)
                return ReturnDic;
            else
            {
                ReturnDic.Add("SUM_POR", SUM_POR);
                ReturnDic.Add("SUM_WY", SUM_WY);
            }

            return ReturnDic;
        }
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的WY/VY累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strItemCode">WY/VY</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        private Dictionary<string, float> FunNarrowWYVYStartEndTotal(string strICP, string strECP, string strItemCode)
        {
            float SUM_POR = 0; //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (oilDatasNarrow.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (ItemCodeoDatas.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            bool Bbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                if (oilDataItemCode == null)
                {
                    Bbreak = true;//计算过程不能为空，为空则跳出
                    break;
                }
                else
                {
                    float itemCodeCal = 0;
                    if (float.TryParse(oilDataItemCode.calData, out itemCodeCal))
                        SUM_POR += itemCodeCal;
                    else
                    {
                        Bbreak = true;
                        break;
                    }
                }
            }

            if (Bbreak)
                return ReturnDic;
            else
                ReturnDic.Add("SUM_POR", SUM_POR);

            return ReturnDic;
        }
              /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        private Dictionary<string, float> FunWideWYVYStartEndTotal(string strICP, string strECP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();
            float SUM_POR = 0;//定义返回变量的两个值
            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasWide = this._wideGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Wide).ToList();//找出窄馏分表数据

            if (oilDatasWide == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            List<OilDataEntity> oilDataECPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).ToList();

            if (oilDataICPList == null || oilDataECPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataICPList.Count <= 0 || oilDataECPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in oilDataICPList)
            {
                OilDataEntity ECPData = oilDataECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;
                else
                {
                    float wyCal = 0;
                    if (float.TryParse(oilDataItemCode.calShowData, out wyCal))
                    {
                        SUM_POR = wyCal;
                        ReturnDic.Add("SUM_POR", SUM_POR);
                        break;
                    }
                    else
                        continue;
                }
            }
            #endregion

            return ReturnDic;
        }

        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunResidueStartEndTotal(string strICP, string strItemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasResidue = this._residueGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Residue).ToList();//找出窄馏分表数据

            if (oilDatasResidue == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            if (oilDataICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (oilDataICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            List<OilDataEntity> WYDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null || ItemCodeoDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in oilDataICPList)
            {
                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                if (oilDataWY == null || oilDataItemCode == null)
                    continue;//计算过程不能为空，为空则跳出
                else
                {
                    float wyCal = 0;
                    if (float.TryParse(oilDataWY.calShowData, out wyCal))
                    {
                        string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                        float fTtemp = 0;
                        if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                        {
                            SUM_POR = wyCal * fTtemp;
                            SUM_WY = wyCal;
                            ReturnDic.Add("SUM_POR", SUM_POR);
                            ReturnDic.Add("SUM_WY", SUM_WY);
                            break;
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }
            }
            #endregion
            
            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunResidueVYWYStartEndTotal(string strICP, string strItemCode)
        {
            float SUM_POR = 0; //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasResidue = this._residueGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Residue).ToList();//找出窄馏分表数据

            if (oilDatasResidue == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            if (oilDataICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (oilDataICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in oilDataICPList)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;//计算过程不能为空，为空则跳出
                else
                {
                    float wyCal = 0;
                    if (float.TryParse(oilDataItemCode.calShowData, out wyCal))
                    {
                        SUM_POR = wyCal;
                        ReturnDic.Add("SUM_POR", SUM_POR);
                        break;
                    }
                    else
                        continue;
                }
            }
            #endregion

            return ReturnDic;
        }
        #endregion

        #region "不连续算法"
        /// <summary>
        /// 连续算法
        /// </summary>
        void getValueFromDisContinuousCol(List<CutMothedEntity> CutMothedEntityList,string colName,
            List<OilDataEntity> NarrowICPList, List<OilDataEntity> NarrowECPList, List<OilDataEntity> WideICPList,
             List<OilDataEntity> WideECPList, List<OilDataEntity> ResidueICPList)
        {
            #region "计算每个物性的代码"
            for (int rowIndex = 0; rowIndex < this.dgvShow.Rows.Count; rowIndex++)
            {
                float? SUM_POR = null; float? SUM_WY = null; string _POR = string.Empty;//定义返回变量的两个值
               
                string itemCode = this.dgvShow.Rows[rowIndex].Cells["itemName"].Tag.ToString();

                if (itemCode == "WY" || itemCode == "VY")
                {
                    #region "CutMothedEntityList"
                    for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count; cutIndex++)
                    {
                        if (CutMothedEntityList[cutIndex].Name == EnumTableType.Narrow.GetDescription().ToString())
                        {
                            #region "窄馏分"
                            OilDataEntity oilDataICP = NarrowICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = NarrowECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)//如果查找的数据不存在则返回空
                            {
                                Dictionary<string, float> narrowResult = OilDataCumulationBll.getWYVYCumuationValueAllowEmptyFromNarrow(this._narrowGridOil ,CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains(itemCode))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = narrowResult[itemCode];
                                    else
                                        SUM_POR = narrowResult[itemCode] + SUM_POR.Value;
                                }
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == EnumTableType.Wide.GetDescription().ToString())
                        {
                            #region "宽馏分"
                            OilDataEntity oilDataICP = WideICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = WideECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();
                            if (oilDataICP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到初切点为" + CutMothedEntityList[cutIndex].strICP + "的宽馏分");
                                break;
                            }
                            else if (oilDataECP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到终切点为" + CutMothedEntityList[cutIndex].strECP + "的宽馏分");
                                break;
                            }
                            else if (oilDataICP != null && oilDataECP != null)
                            {
                                Dictionary<string, float> wideResult = OilDataCumulationBll.getWYVYCumuationValueAllowEmptyFromWide(this._wideGridOil ,CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);
                                if (wideResult.Count > 0 && wideResult.Keys.Contains(itemCode))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = wideResult[itemCode];
                                    else
                                        SUM_POR = wideResult[itemCode] + SUM_POR.Value;
                                }
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == EnumTableType.Residue.GetDescription().ToString())
                        {
                            #region "渣油"
                            OilDataEntity oilDataICP = ResidueICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            if (oilDataICP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到初切点为" + CutMothedEntityList[cutIndex].strICP + "的渣油");
                                break;
                            }
                            else
                            {
                                Dictionary<string, float> residueResult = OilDataCumulationBll.getWYVYCumuationValueAllowEmptyFromResidue(this._residueGridOil ,CutMothedEntityList[cutIndex].strICP, itemCode);
                                if (residueResult.Count > 0 && residueResult.Keys.Contains(itemCode))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = residueResult[itemCode];
                                    else
                                        SUM_POR = residueResult[itemCode] + SUM_POR.Value;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region "反函数"
                    if (SUM_POR != null)
                    {
                        this.dgvShow.Rows[rowIndex].Cells[colName].Value =tool.calDataDecLimit(SUM_POR.ToString(), 4); ;
                    }
                    #endregion
                }
                else
                {
                    #region "CutMothedEntityList"
                    for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count; cutIndex++)
                    {
                        if (CutMothedEntityList[cutIndex].Name == EnumTableType.Narrow.GetDescription().ToString())
                        {
                            #region "窄馏分"
                            OilDataEntity oilDataICP = NarrowICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = NarrowECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)//如果查找的数据不存在则返回空
                            {
                                Dictionary<string, float> narrowResult = OilDataCumulationBll.getItemCodeCumuationValueAllowEmptyFromNarrow(this._narrowGridOil , CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains(itemCode))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = narrowResult[itemCode];
                                    else
                                        SUM_POR = narrowResult[itemCode] + SUM_POR.Value;
                                }

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains("WY"))
                                {
                                    if (SUM_WY == null)
                                        SUM_WY = narrowResult["WY"];
                                    else
                                        SUM_WY = narrowResult["WY"] + SUM_WY.Value;
                                }
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == EnumTableType.Wide.GetDescription().ToString())
                        {
                            #region "宽馏分"
                            OilDataEntity oilDataICP = WideICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = WideECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)
                            {
                                Dictionary<string, float> wideResult = OilDataCumulationBll.getItemCodeCumuationValueAllowEmptyFromWide (this._wideGridOil , CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);
                                if (wideResult.Count > 0 && wideResult.Keys.Contains(itemCode))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = wideResult[itemCode];
                                    else
                                        SUM_POR = wideResult[itemCode] + SUM_POR.Value;
                                }


                                if (wideResult.Count > 0 && wideResult.Keys.Contains("WY"))
                                {
                                    if (SUM_WY == null)
                                        SUM_WY = wideResult["WY"];
                                    else
                                        SUM_WY = wideResult["WY"] + SUM_WY.Value;
                                }

                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == EnumTableType.Residue.GetDescription().ToString())
                        {
                            #region "渣油"
                            OilDataEntity oilDataICP = ResidueICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();

                            if (oilDataICP != null)
                            {
                                Dictionary<string, float> residueResult = OilDataCumulationBll.getItemCodeCumuationValueAllowEmptyFromResidue(this._residueGridOil ,CutMothedEntityList[cutIndex].strICP, itemCode);
                                if (residueResult.Count > 0 && residueResult.Keys.Contains(itemCode))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = residueResult[itemCode];
                                    else
                                        SUM_POR = residueResult[itemCode] + SUM_POR.Value;
                                }

                                if (residueResult.Count > 0 && residueResult.Keys.Contains("WY"))
                                {
                                    if (SUM_WY == null)
                                        SUM_WY = residueResult["WY"];
                                    else
                                        SUM_WY = residueResult["WY"] + SUM_WY.Value;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region "反函数"
                    if (SUM_WY == 0)
                        continue;
                    if (SUM_POR != null && SUM_WY != null)
                    {
                        float temp = SUM_POR.Value / SUM_WY.Value;
                        _POR = BaseFunction.InverseIndexFunItemCode(temp.ToString(), itemCode);
                        this.dgvShow.Rows[rowIndex].Cells[colName].Value = tool.calDataDecLimit(_POR, 4);  
                        
                    }
                    #endregion
                }
            }
            #endregion
        }

        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的WY/VY累积和(允许存在空值)
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strItemCode">WY/VY</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        private Dictionary<string, float> FunNarrowWYVYStartEndTotalDiscontinuous(string strICP, string strECP, string strItemCode)
        {
            float SUM_POR = 0; //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (oilDatasNarrow.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (ItemCodeoDatas.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            bool Bbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                float itemCodeCal = 0;
                if (oilDataItemCode != null && float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                {
                    Bbreak = true;
                    SUM_POR += itemCodeCal;
                }
            }

            if (Bbreak)
                ReturnDic.Add("SUM_POR", SUM_POR);

            return ReturnDic;
        }
        /// <summary>
        /// 宽馏分中查找对应的两个ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        private Dictionary<string, float> FunWideWYVYStartEndTotalDiscontinuous(string strICP, string strECP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasWide = this._wideGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Wide).ToList();//找出窄馏分表数据

            if (oilDatasWide == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            List<OilDataEntity> oilDataECPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).ToList();

            if (oilDataICPList == null || oilDataECPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataICPList.Count <= 0 || oilDataECPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in oilDataICPList)
            {
                OilDataEntity ECPData = oilDataECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;

                float itemCodeCal = 0;
                if (oilDataItemCode.calShowData != string.Empty && float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                {
                    ReturnDic.Add("SUM_POR", itemCodeCal);
                    break;
                }
                else
                    continue;
            }
            #endregion

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunResidueVYWYStartEndTotalDiscontinuous(string strICP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasResidue = this._residueGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Residue).ToList();//找出窄馏分表数据

            if (oilDatasResidue == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            if (oilDataICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (oilDataICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in oilDataICPList)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                float itemCodeCal = 0;
                if (oilDataItemCode != null && float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                {
                    ReturnDic.Add("SUM_POR", itemCodeCal);
                    break;
                }
            }
            #endregion

            return ReturnDic;
        }
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(允许存在空值)
        /// </summary>
        /// <param name="strICP">窄馏分的ICP</param>
        /// <param name="strECP">窄馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunNarrowStartEndTotalDiscontinuous(string strICP, string strECP, string strItemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (oilDatasNarrow.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }


            List<OilDataEntity> WYDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (WYDatas.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;
            //if (WYDatas == null || ItemCodeoDatas == null)//如果查找的数据不存在则返回空
            //    return ReturnDic;
            //if (WYDatas.Count <= 0 || ItemCodeoDatas.Count <= 0)//如果查找的数据不存在则返回空
            //    return ReturnDic;
            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            #region "计算"
            bool WYbreak = false, SUMbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();

                float wyCal = 0;
                if (oilDataWY != null && float.TryParse(oilDataWY.calShowData, out wyCal))
                {
                    SUM_WY = SUM_WY + wyCal;
                    WYbreak = true;
                }

                if (ItemCodeoDatas == null)
                    continue;

                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                if (oilDataWY != null && oilDataItemCode != null && float.TryParse(oilDataWY.calShowData, out wyCal))
                {
                    string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = SUM_POR + wyCal * fTtemp;
                        SUMbreak = true;
                    }
                }
            }

            if (SUMbreak)
                ReturnDic.Add("SUM_POR", SUM_POR);
            if (WYbreak)
                ReturnDic.Add("SUM_WY", SUM_WY);
            #endregion

            return ReturnDic;
        }

        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunWideStartEndTotalDiscontinuous(string strICP, string strECP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();
            float SUM_POR = 0;  //定义返回变量的两个值
            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasWide = this._wideGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Wide).ToList();//找出窄馏分表数据

            if (oilDatasWide == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            List<OilDataEntity> oilDataECPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).ToList();

            if (oilDataICPList == null || oilDataECPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataICPList.Count <= 0 || oilDataECPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> WYDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in oilDataICPList)
            {
                OilDataEntity ECPData = oilDataECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                float wyCal = 0;
                if (oilDataWY != null && float.TryParse(oilDataWY.calShowData, out wyCal))
                {
                    ReturnDic.Add("SUM_WY", wyCal);
                }

                if (ItemCodeoDatas == null)
                    continue;
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();

                if (oilDataWY != null && oilDataItemCode != null && float.TryParse(oilDataWY.calShowData, out wyCal))
                {
                    string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = wyCal * fTtemp;
                        ReturnDic.Add("SUM_POR", SUM_POR);
                        break;
                    }
                }
            }
            #endregion

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunResidueStartEndTotalDiscontinuous(string strICP, string strItemCode)
        {
            float SUM_POR = 0;  //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasResidue = this._residueGridOil._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Residue).ToList();//找出窄馏分表数据

            if (oilDatasResidue == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            if (oilDataICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (oilDataICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            List<OilDataEntity> WYDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in oilDataICPList)
            {
                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();

                float wyCal = 0;
                if (oilDataWY != null && float.TryParse(oilDataWY.calShowData, out wyCal))
                {
                    ReturnDic.Add("SUM_WY", wyCal);
                }

                if (ItemCodeoDatas == null)
                    continue;
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();

                if (oilDataWY != null && oilDataItemCode != null && float.TryParse(oilDataWY.calShowData, out wyCal))
                {
                    string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = wyCal * fTtemp;
                        ReturnDic.Add("SUM_POR", SUM_POR);
                        break;
                    }
                }
            }
            #endregion

            return ReturnDic;
        }

        #endregion 

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion 

        private void butDel_Click(object sender, EventArgs e)
        {
            if (this.tabControlEx1.TabCount>0)
                this.tabControlEx1.TabPages.Remove(this.tabControlEx1.SelectedTab);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            setDgv(this.dgvShow, this.lvAccRightShow, this._Dic);
            foreach (var colName in this._Dic.Keys)
                fillDataToDgv(this.dgvShow, colName, this._Dic[colName]);
        }

        #region "快捷键"
        
        /// <summary>
        /// 保存当前切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabControlEx1.TabPages.Count <= 0)
                return;

            var listView = this.tabControlEx1.SelectedTab.Controls[this.tabControlEx1.SelectedTab.Text] as ListView;
            if (listView == null)
                return;

            if (listView.Items.Count <= 0)
                return;
            List<CutMothedEntity> cutMothedEntityOutList = new List<CutMothedEntity>();
            foreach (ListViewItem item in listView.Items)
            {
                string strCutName = item.SubItems["表名称"].Text;
                string strICP = item.SubItems["ICP"].Text;
                string strECP = item.SubItems["ECP"].Text;

                CutMothedEntity cutMothed = new CutMothedEntity();
                cutMothed.Name = strCutName;
                cutMothed.strICP = strICP.Trim();
                cutMothed.strECP = strECP.Trim();
               
                cutMothedEntityOutList.Add(cutMothed);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "切割方案文件 (*.audi)|*.audi";
            saveFileDialog1.RestoreDirectory = true;
            //saveFileDialog1.InitialDirectory = ConfigBll._startupPath+ ConfigBll._audFilePath;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outCut(saveFileDialog1.FileName, cutMothedEntityOutList);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        private void outCut(string fileName, List<CutMothedEntity> outCutMothedList)
        {
            CutMothedOutLib outLib = new CutMothedOutLib();
            outLib.CutMotheds = outCutMothedList;
            Serialize.Write<CutMothedOutLib>(outLib, fileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        private void outCut(string fileName, Dictionary<string, List<CutMothedEntity>> outCutMothedDic)
        {
            CutMothedDcOutLib outLib = new CutMothedDcOutLib();
            outLib.CutMothedDic = outCutMothedDic;
            Serialize.Write<CutMothedDcOutLib>(outLib, fileName);
        }
        private void 读取ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabControlEx1.TabPages.Count <= 0)
                return;

            var listView = this.tabControlEx1.SelectedTab.Controls[this.tabControlEx1.SelectedTab.Text] as ListView;
            if (listView == null)
                return;
           
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "切割方案文件 (*.audi)|*.audi";
            saveFileDialog.RestoreDirectory = true;
            //saveFileDialog.InitialDirectory = ConfigBll._startupPath + ConfigBll._audFilePath;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var outLib = Serialize.Read<CutMothedOutLib>(saveFileDialog.FileName);//将保存的条件赋值
                listView.Items.Clear();
                
                #region "将查询条件赋值"
                for (int i = 0; i < outLib.CutMotheds.Count; i++)
                {
                    ListViewItem Item = new ListViewItem();
                    for (int colIndex = 0; colIndex < listView.Columns.Count; colIndex++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        
                        Item.SubItems.Add(temp);

                        #region
                        switch (colIndex)
                        {
                            case 0:
                                Item.SubItems[0].Name = "表名称";
                                break;
                            case 1:
                                Item.SubItems[1].Name = "表名称:ICP";
                                Item.SubItems[1].Text = ":";
                                break;
                            case 2:
                                Item.SubItems[2].Name = "ICP";
                                break;
                            case 3:
                                Item.SubItems[3].Name = "ICP-ECP";
                                Item.SubItems[3].Text = "-";
                                break;
                            case 4:
                                Item.SubItems[4].Name = "ECP";
                                break;
                        }
                        #endregion
                    }

                    Item.SubItems["表名称"].Text = outLib.CutMotheds[i].Name;
                    if (string.IsNullOrWhiteSpace(outLib.CutMotheds[i].strICP))
                    {
                        OilDataEntity ICPData = this._narrowGridOil.GetDataByRowItemCodeColumnIndex("ICP", 0);
                        Item.SubItems["ICP"].Tag = ICPData == null ? "-50" : ICPData.calData;
                    }
                    Item.SubItems["ICP"].Text = outLib.CutMotheds[i].strICP;
                    Item.SubItems["ECP"].Text = outLib.CutMotheds[i].strECP;
                    listView.Items.Add(Item);
                }
                #endregion              
            }
            else
            {
                return;
            }    
        }

        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabControlEx1.TabPages.Count <= 0)
                return;

            var listView = this.tabControlEx1.SelectedTab.Controls[this.tabControlEx1.SelectedTab.Text] as ListView;
            if (listView == null)
                return;

            listView.Items.Clear();//清除显示列表信息
        }

       
        /// <summary>
        /// 保存所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (this.tabControlEx1.TabPages.Count <= 0)
                return;
            Dictionary<string, List<CutMothedEntity>> cutMothedDic = new Dictionary<string, List<CutMothedEntity>>();
            
            foreach (TabPage tabpage in this.tabControlEx1.TabPages)
            {
                var listView = tabpage.Controls[tabpage.Name] as ListView;
                if (listView == null || listView.Items.Count <= 0)
                    continue;
                List<CutMothedEntity> cutMothedList = new List<CutMothedEntity>();
                foreach (ListViewItem item in listView.Items)
                {
                    string strCutName = item.SubItems["表名称"].Text;
                    string strICP = item.SubItems["ICP"].Text;
                    string strECP = item.SubItems["ECP"].Text;

                    CutMothedEntity cutMothed = new CutMothedEntity();
                    cutMothed.Name = strCutName;
                    cutMothed.strICP = strICP.Trim();
                    cutMothed.strECP = strECP.Trim();

                    cutMothedList.Add(cutMothed);

                    if (!cutMothedDic.Keys.Contains(tabpage.Name))
                    { cutMothedDic.Add(tabpage.Name, cutMothedList); }
                }
            }
 
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "切割方案文件 (*.aud)|*.aud";

            saveFileDialog1.InitialDirectory = ConfigBll._startupPath + ConfigBll._audFilePath;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outCut(saveFileDialog1.FileName, cutMothedDic);              
            }
        }
        /// <summary>
        /// 读取所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {         
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "切割方案文件 (*.aud)|*.aud";
            saveFileDialog.RestoreDirectory = true;
            //saveFileDialog.InitialDirectory = ConfigBll._startupPath + ConfigBll._audFilePath;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string strDir = saveFileDialog.FileName;
                
                if (!string.IsNullOrWhiteSpace(strDir))
                {
                    ConfigBll confige = new ConfigBll();
                    confige.updateItem(strDir, enumModel.ManAud);
                    initAllCutMothed(saveFileDialog.FileName);   
                }    
            }
            else
            {
                return;
            }    
        }
        private void initAllCutMothed(string fileName)
        {
            if (File.Exists(fileName))
            {
                var newListView = this.tabControlEx1.SelectedTab.Controls[this.tabControlEx1.SelectedTab.Text] as ListView;
                if (newListView == null)
                    return;
                //newListView.Items.Clear();//清空所有的项

                var outLib = Serialize.Read<CutMothedDcOutLib>(fileName);//将保存的条件集合赋值
                if (outLib != null)
                {
                    foreach (var key in outLib.CutMothedDic.Keys)
                    {
                        addTabPage(key, outLib.CutMothedDic[key]);
                    }
                }
            }              
        }
        /// <summary>
        /// 清空所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (this.tabControlEx1.TabCount > 0)
                this.tabControlEx1.TabPages.Clear();
        }
        #endregion 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            FrmAccountAuditInput frmRemark = new FrmAccountAuditInput();
            frmRemark.StartPosition = FormStartPosition.CenterScreen;
            frmRemark.MdiParent = this.ParentForm ;
            frmRemark.ShowDialog();
            editAccountAuditName(frmRemark);
        }
        private void editAccountAuditName(FrmAccountAuditInput frmRemark)
        {
            if (GlobalAccountAuditInput.YesNo == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(GlobalAccountAuditInput.message))
                    return;
                if (!this.tabControlEx1.TabPages.ContainsKey(GlobalAccountAuditInput.message))
                {
                    this.tabControlEx1.SelectedTab.Controls[this.tabControlEx1.SelectedTab.Text].Name = GlobalAccountAuditInput.message;

                    this.tabControlEx1.SelectedTab.Text = GlobalAccountAuditInput.message;
                    this.tabControlEx1.SelectedTab.Name = GlobalAccountAuditInput.message;
                    frmRemark.Close();
                }
                else
                {
                    frmRemark.labError.Visible = true;
                    frmRemark.ShowDialog();
                    editAccountAuditName(frmRemark);
                }
            }
            else
            {
                frmRemark.Close();
            }

        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            FrmAccountAuditInput tempfrmRemark = (FrmAccountAuditInput)GetChildFrm("FrmAccountAuditInput");

            if (tempfrmRemark == null)
            {
                FrmAccountAuditInput frmRemark = new FrmAccountAuditInput();
                frmRemark.StartPosition = FormStartPosition.CenterScreen;
                frmRemark.MdiParent = this.ParentForm;
                frmRemark.ShowDialog();

                addAccountAuditName(frmRemark);
            }
        }


        private void addAccountAuditName(FrmAccountAuditInput frmRemark)
        {
            if (GlobalAccountAuditInput.YesNo == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(GlobalAccountAuditInput.message))
                    return;
                if (!this.tabControlEx1.TabPages.ContainsKey(GlobalAccountAuditInput.message))
                {
                    addTabPage(GlobalAccountAuditInput.message);
                    frmRemark.Close();
                }
                else
                {
                    frmRemark.labError.Visible = true;
                    frmRemark.ShowDialog();
                    addAccountAuditName(frmRemark);
                }
            }
            else
            {
                frmRemark.Close();
            }
        
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否删除？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
            {
                if (this.tabControlEx1.TabCount > 0)
                    this.tabControlEx1.TabPages.Remove(this.tabControlEx1.SelectedTab);
                else
                {
                    //不做任何事情
                }
            }
            else
            { 
                //不做任何事情
            }         
        }


        /// <summary>
        /// 根据窗体名称获取窗体
        /// </summary>
        /// <param name="childFrmName">窗体名称</param>
        /// <returns>存在返回1并不激活此窗口，不存在返回null</returns>
        public Form GetChildFrm(string childFrmName)
        {
            Form childFrm = null;
            foreach (Form frm in this.MdiChildren)
            {
                if (frm.Name == childFrmName)
                {
                    childFrm = frm;
                    break;
                }
            }
            return childFrm;
        }
        
    }
}
