using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil;

namespace RIPP.App.OilDataManager.Forms.LibManage
{
    public partial class FrmLibManageA : Form
    {
        private OilInfoBll _oilInfoBll = new OilInfoBll();
        private string _sqlWhere = "1=1";
        private string _sqlLibrary = "";
        protected LibraryType _libraryType = LibraryType.LibraryA;

        public FrmLibManageA(LibraryType libraryType = LibraryType.LibraryA)
        {
            InitializeComponent();
            _libraryType = libraryType;
            if (libraryType == LibraryType.LibraryA)
            {
                _sqlLibrary = " and isLibraryA='true'";
                this.Text = "导出A库";
            }
            else if (libraryType == LibraryType.LibraryB)
            {
                _sqlLibrary = " and isLibraryB='true'";
                this.Text = "导出B库";
            }


            SetColHeader();
            InitStyle();
            GridListBind();
        }

        /// <summary>
        /// 表格控件绑定
        /// </summary>
        private void GridListBind()
        {          
            this.gridList.Rows.Clear();
            IList<OilInfoEntity> oilInfo = _oilInfoBll.dbGet(_sqlWhere + _sqlLibrary);

            //绑定数据
            for (int i = 0; i < oilInfo.Count; i++)
            {
                this.gridList.Rows.Add(false, oilInfo[i].ID, oilInfo[i].crudeName, oilInfo[i].englishName, oilInfo[i].crudeIndex,
                                 oilInfo[i].country, oilInfo[i].region, oilInfo[i].fieldBlock, oilInfo[i].sampleDate, oilInfo[i].receiveDate,
                                 oilInfo[i].sampleSite, oilInfo[i].assayDate, oilInfo[i].updataDate, oilInfo[i].sourceRef, oilInfo[i].assayLab,
                                 oilInfo[i].assayer, oilInfo[i].assayCustomer, oilInfo[i].reportIndex, oilInfo[i].summary, oilInfo[i].type,
                                 oilInfo[i].classification, oilInfo[i].sulfurLevel, oilInfo[i].acidLevel, oilInfo[i].corrosionLevel, oilInfo[i].processingIndex, oilInfo[i].isLibraryB ? "是" : "否");
            }          
        }

        private void SetColHeader()
        {
            //清除表的行和列
            this.gridList.Rows.Clear();
            this.gridList.Columns.Clear();

            #region 添加表头
            this.gridList.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "选择", Name = "select", Width=70 , ReadOnly=false});
         
            //this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "序号", HeaderText = "序号", Width = 70 });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称", ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "英文名称", HeaderText = "英文名称" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "产地国家", HeaderText = "产地国家" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "地理区域" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "油田区块" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "采样日期" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "到院日期" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "采样地点" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价日期" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "更新日期" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "数据来源" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价单位" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价人员" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价来源" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "报告号" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评论" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "类别" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "基属" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "硫水平" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "酸水平" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "腐蚀指数" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "加工指数" , ReadOnly = true });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "已生成B库" , ReadOnly = true });
            #endregion
        }

        private void InitStyle()
        {
            this.gridList.AllowUserToAddRows = false;
            this.gridList.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            this.gridList.DefaultCellStyle = myStyle.dgdViewCellStyle2();

            this.gridList.BorderStyle = BorderStyle.None;
            this.gridList.RowHeadersWidth = 30;
            this.gridList.MultiSelect = false;
            this.gridList.ReadOnly = false;

            this.gridList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

        }

        private void toolStripBtnAdd_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "原油数据文件 (*.lib)|*.lib";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                outLib(saveFileDialog.FileName);               
            }
        }

        private void outLib(string fileName)
        {
            this.gridList.EndEdit();
            string strWhere = "";
            string strWhere2 = "";
            foreach (DataGridViewRow row in this.gridList.Rows)
            {
                if (bool.Parse(row.Cells["select"].Value.ToString()) == true)
                {                
                    strWhere+= " or ID=" + int.Parse(row.Cells["ID"].Value.ToString());
                    strWhere2 += " or oilInfoID=" + int.Parse(row.Cells["ID"].Value.ToString());   
                }
            }
            strWhere = strWhere.Trim().Substring(2);
            strWhere2 = strWhere2.Trim().Substring(2);

            OilInfoOutAccess oilInfoOutAccess = new OilInfoOutAccess();
            List<OilInfoOut> oilInfoOuts = oilInfoOutAccess.Get(strWhere);

            OilDataOutAccess acess = new OilDataOutAccess();
            List<OilDataOut> oilDataAlls = acess.Get(strWhere2);

            OutLib outLib = new OutLib();
            OilTableRowOutAccess oilTableRowAccess = new OilTableRowOutAccess();
            OilTableColOutAccess oilTableColAccess = new OilTableColOutAccess();
            outLib.oilTableRows = oilTableRowAccess.Get("1=1");
            outLib.oilTableCols = oilTableColAccess.Get("1=1");

            foreach (OilInfoOut oilInfoOut in oilInfoOuts)
            {
                oilInfoOut.oilDatas = oilDataAlls.Where(c => c.oilInfoID == oilInfoOut.ID).ToList();
                outLib.oilInfoOuts.Add(oilInfoOut);
            }

            Serialize.Write<OutLib>(outLib, fileName);

        

        }
    }

  
}
