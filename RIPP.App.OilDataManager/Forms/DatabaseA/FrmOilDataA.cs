using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.GCBLL;
using RIPP.OilDB.Data.DataCheck;
using RIPP.OilDB.Data.DataSupplement;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.UI.GridOil.V2;
using System.Threading;
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil.V2.Model;
using RIPP.OilDB.BLL;

namespace RIPP.App.OilDataManager.Forms.DatabaseA
{
    public partial class FrmOilDataA : Form, IGridOilEditor
    {
        #region "私有变量"
        private OilInfoEntity _oilA = null;     // 一条原油  
        private const int CLOSE_SIZE = 15;      // 导航的关闭按钮高宽
        
        private bool _errValidate = false;     //是否验证通过 
        /// <summary>
        /// 判读是否通过错误验证 
        /// </summary>
        public bool IsValidated
        {
            get
            {
                if (this.gridOilInfoA1.IsValidated || this.dgvWhole.IsValidated
                    || this.dgvNarrow.IsValidated || this.dgvWide.IsValidated
                    || this.dgvResidue.IsValidated )
                    this._errValidate = false;
                else
                    this._errValidate = true;

                return _errValidate;
            }
            set
            {
                this._errValidate = value;
            }       
        }



        #region 等待线程
        /// <summary>
        /// 等待窗口
        /// </summary>
        private FrmWaiting myFrmWaiting;

        /// <summary>
        /// 等待线程
        /// </summary>
        private Thread waitingThread;

        /// <summary>
        /// 等待线程
        /// </summary>
        public void Waiting()
        {
            this.myFrmWaiting = new FrmWaiting();
            this.myFrmWaiting.ShowDialog();
        }
        /// <summary>
        /// 开始等待线程
        /// </summary>
        public void StartWaiting()
        {
            this.waitingThread = new Thread(new ThreadStart(this.Waiting));
            this.waitingThread.Start();
        }

        /// <summary>
        /// 结束等待线程
        /// </summary>
        private void StopWaiting()
        {
            if (this.waitingThread != null)
            {
                if (myFrmWaiting != null)
                {
                    Action ac = () => myFrmWaiting.Close();
                    myFrmWaiting.Invoke(ac);
                }
                this.waitingThread.Abort();
            }
        }

        #endregion

        /// <summary>
        /// 曲线类型
        /// </summary>
        public OilInfoEntity oil
        {
            set { this._oilA = value; }
            get { return this._oilA; }
        }
        /// <summary>
        /// 判断表格中的数据是否有改动
        /// </summary>
        private bool _isChanged = false; //是否有改动
        private bool _isInforChanged = false; //是否有改动
        /// <summary>
        /// 是否有改动 
        /// </summary>
        public bool isChanged
        {
            get
            {
                
                if (this.gridOilInfoA1.isChanged || this.dgvWhole.NeedSave || this.dgvLight.NeedSave
                    || this.dgvGCInput.NeedSave || this.dgvGC.NeedSave || this.dgvGCLeve.NeedSave
                    || this.dgvNarrow.NeedSave || this.dgvWide.NeedSave || this.dgvResidue.NeedSave || this.dgvSmilatedDistalltion.NeedSave)
                    this._isChanged = true;
                else
                    this._isChanged = false;
               
                return _isChanged ;
            }
            set
            {
                this._isChanged = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool isInfoChanged
        {
            get
            {
                if (this.dgvWhole.needSaveInfo || this.dgvLight.needSaveInfo
                    || this.dgvGCInput.needSaveInfo || this.dgvGC.needSaveInfo || this.dgvGCLeve.needSaveInfo
                    || this.dgvNarrow.needSaveInfo || this.dgvWide.needSaveInfo || this.dgvResidue.needSaveInfo || this.dgvSmilatedDistalltion.needSaveInfo)
                    this._isInforChanged = true;
                else
                    this._isInforChanged = false;

                return _isInforChanged;
            }
            set
            {
                this._isInforChanged = value;
            }
        }

        /// <summary>
        /// 上一个选项卡的名称。
        /// </summary>
        private string previousSelectTabText = string.Empty;
        /// <summary>
        /// 原油信息提示
        /// </summary>
        private List<OilTipTableEntity> _OilTipTableEntityList = new List<OilTipTableEntity>();
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oil">一条原油</param>
        public FrmOilDataA(OilInfoEntity oil,bool bZH = true)
        {
            this.KeyPreview = true;
            InitializeComponent();
            this.splitContainer.Panel1.MouseClick += new MouseEventHandler(splitContainer_Panel1_MouseClick);
            this.splitContainer.Panel1.Paint += new PaintEventHandler(splitContainer_Panel1_Paint);
            initTab(oil );
            //this.oilTab1.Undo.OnChanged += new EventHandler(Undo_OnChanged);
            OilTipTableAccess oilTipTableAccess = new OilTipTableAccess();
            this._OilTipTableEntityList = oilTipTableAccess.Get("1=1");
            this.Text = "原始库原油数据-" + this._oilA.crudeIndex;
            this.Name = this._oilA.crudeIndex + "A";   //在打开操作时要根据该名称判断窗口是否存在
            splitLeftClose();
            addRemarkToForm();
            isVisible(bZH);
        }

        /// <summary>
        /// 演示用
        /// </summary>
        /// <param name="value"></param>
        private void isVisible(bool value)
        {
            if (value == false)
            {
                this.tabControl1.TabPages.Remove(tabPage6);
                this.tabControl1.TabPages.Remove(tabPage10);
                tabControlNavigate.TabPages.Remove(tabPageWarning);
                tabControlNavigate.TabPages.Remove(tabPageRemark);
            }
        }
        #endregion

        /// <summary>
        /// 选择选项卡
        /// </summary>
        /// <param name="tabIndex"></param>
        public void setTabIndex(string tabPageText = "原油信息")
        {
            foreach(TabPage currtentTab in this.tabControl1.TabPages  )
            {
                if (currtentTab.Text == tabPageText)
                {
                    this.tabControl1.SelectedTab = currtentTab;
                    break;
                }
            }
        }

        #region 选项卡和表格数据绑定

        /// <summary>
        /// 选项卡数据绑定
        /// </summary>
        private void initTab(OilInfoEntity oil,bool bZH = false)
        {
            if (oil != null)
            {
                this._oilA = oil;
                this.gridOilInfoA1.FillOilInfo(this._oilA);
                this.dgvWhole.InitTable(this._oilA, EnumTableType.Whole);
                this.dgvWhole.AllowEditColumn = false;

                this.dgvLight.InitTable(this._oilA, EnumTableType.Light);
                this.dgvLight.AllowEditColumn = false;

                this.dgvGCInput.InitTable(this._oilA, dgvWide);
                this.dgvGCInput.AllowEditColumn = false;

                this.dgvGC.InitTable(this._oilA, EnumTableType.GC);
                this.dgvGC.HiddenColumnType = GridOilColumnType.Lab;
                this.dgvGC.AllowEditColumn = false;
                
                this.dgvGCLeve.InitTable(this._oilA, EnumTableType.GCLevel);
                this.dgvGCLeve.HiddenColumnType = GridOilColumnType.Lab;
                this.dgvGCLeve.AllowEditColumn = false;
 

                this.dgvNarrow.InitTable(this._oilA, EnumTableType.Narrow);
                this.dgvNarrow.AllowEditColumn = true;

                this.dgvWide.InitTable(this._oilA, EnumTableType.Wide, "WCT");
                this.dgvWide.AllowEditColumn = true;

                this.dgvResidue.InitTable(this._oilA, EnumTableType.Residue, "RCT");
                this.dgvResidue.AllowEditColumn = true;

                
                this.dgvSmilatedDistalltion.InitTable(this._oilA, EnumTableType.SimulatedDistillation);
                this.dgvSmilatedDistalltion.AllowEditColumn = false;
                               
            }
        }

        /// <summary>
        /// splitContainer左边折叠
        /// </summary>
        public void splitLeftClose()
        {
            this.splitContainer.Panel1Collapsed = true;
        }

        /// <summary>
        /// splitContainer左边打开
        /// </summary>
        public void splitLeftOpen()
        {
            this.splitContainer.Panel1Collapsed = false;
        }

        #endregion

        #region 保存数据

        #region "保存到A库"
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void changeSave()
        {       
            this.dgvWhole.NeedSave = true;
            this.dgvLight.NeedSave = true;
            this.dgvGCInput.NeedSave = true;
            this.dgvGC.NeedSave = true;
            this.dgvGCLeve.NeedSave = true;
            this.dgvNarrow.NeedSave = true;
            this.dgvWide.NeedSave = true;
            this.dgvResidue.NeedSave = true;
            this.dgvSmilatedDistalltion.NeedSave = true;
        }
        private void saveInfo()
        {
            this.gridOilInfoA1.Save(this._oilA.ICP0);
            this.Text = "原始库原油数据-" + this._oilA.crudeIndex;
            this.Name = this._oilA.crudeIndex + "A";
            this.gridOilInfoA1.isChanged = false;

            FrmMain frmMain = (FrmMain)this.MdiParent;
            DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)frmMain.GetChildFrm("frmOpenA");
            if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
            {
                frmOpenA.refreshGridList(false);
            }
        }
        /// <summary>
        /// 保存A库
        /// </summary>
        public bool SaveA()
        {
            stopEdit();

            //if (this.gridOilInfoA1.isChanged)//原油信息表
                saveInfo();
 
            if (this.dgvWhole.NeedSave)//原油性质
                this.dgvWhole.Save();
            if (this.dgvLight.NeedSave)//轻端
                this.dgvLight.Save();
            if (this.dgvGCInput.NeedSave)//GC输入表
                this.dgvGCInput.Save();
            if (this.dgvGC.NeedSave)//GC统计
                this.dgvGC.Save();
            if (this.dgvGCLeve.NeedSave)//GC标准
                this.dgvGCLeve.Save();
            if (this.dgvNarrow.NeedSave)//窄馏分
                this.dgvNarrow.Save();
            if (this.dgvWide.NeedSave)//宽馏分
                this.dgvWide.Save();
            if (this.dgvResidue.NeedSave)//渣油
                this.dgvResidue.Save();
            if (this.dgvSmilatedDistalltion.NeedSave)//模拟馏程表
                this.dgvSmilatedDistalltion.Save();

            return true;
        }

        /// <summary>
        /// 点击保存到A库按钮时，停止编辑
        /// </summary>
        private void stopEdit()
        {
            this.gridOilInfoA1.EndEdit();
            this.dgvGC.EndEdit();
            this.dgvGCInput.EndEdit();
            this.dgvGCLeve.EndEdit();
            this.dgvLight.EndEdit();
            this.dgvNarrow.EndEdit();
            this.dgvResidue.EndEdit();
            this.dgvSmilatedDistalltion.EndEdit();
            this.dgvWhole.EndEdit();
            this.dgvWide.EndEdit();
        }

        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public int SaveOilInfo()
        {
            var gridOilInfo = this.gridOilInfoA1;
            if (gridOilInfo == null)
                return -1;
            return gridOilInfo.Save(this._oilA.ICP0);
        }

        #endregion

        #region "保存到B库"
        #region "保存到B库老版本"
        /// <summary>
        /// 保存到B库
        /// </summary>
        //public void SaveB()
        //{
        //    OilInfoBEntity infoB = new OilInfoBEntity();//读出原油信息B库的数据
        //    string result = this.oilTab1.DataCheckAll();//用来检查A库数据是否检查和审查完毕。
        //    if (result == "")
        //    {
        //        infoB.ID = this.oilTab1.SaveInfoB(ref infoB); //保存信息表
        //        CurveAccess curveAccess = new CurveAccess();
        //        if (infoB.ID > 0)
        //        {
        //            this.StartWaiting();
        //            OilBll.ConvertToDatasB(ref infoB, this._oil.OilDatas); //保存数据                   
        //            this.oilTab1.SaveCurves(infoB.ID);   //保存曲线
        //            this.StopWaiting();
        //            //SaveC(infoB);//保存到C库                    
        //        }
        //        else
        //        {
        //            DialogResult r = MessageBox.Show(this._oil.crudeIndex + "原油已存在！是否要更新", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        //            if (r == DialogResult.Yes)
        //            {
        //                this.StartWaiting();
        //                OilInfoBAccess oilInfoBAccess = new OilInfoBAccess();
        //                oilInfoBAccess.Delete("crudeIndex='" + this._oil.crudeIndex + "'");

        //                infoB.ID = this.oilTab1.SaveInfoB(ref infoB); //保存信息表
        //                OilBll.ConvertToDatasB(ref infoB, this._oil.OilDatas); //保存数据                   
        //                this.oilTab1.SaveCurves(infoB.ID);   //保存曲线
        //                this.StopWaiting();

        //                //SaveC(infoB);//保存到C库
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show(result + "表,数据检验出错!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }
        //    this.Text = "A库原油数据-" + this._oil.crudeIndex;
        //    this.Name = this._oil.crudeIndex + "A";

        //}
        #endregion
        /// <summary>
        /// 保存到B库
        /// </summary>
        public void SaveB()
        {
            OilInfoBEntity infoB = new OilInfoBEntity();//读出原油信息B库的数据
            //string result = this.oilDataCheck();//用来检查A库数据是否检查和审查完毕。
            string result = string.Empty;
            if (result == "")
            {
                infoB.ID = this.SaveInfoB(ref infoB); //保存信息表

                if (infoB.ID > 0)
                {
                    List<OilDataEntity> wholeDataList = this.dgvWhole.GetAllData();
                    List<OilDataEntity> gcLevelDataList = this.dgvGCLeve.GetAllData();
                    List<OilDataEntity> oilDataList = new List<OilDataEntity>();
                    oilDataList.AddRange(wholeDataList);
                    oilDataList.AddRange(gcLevelDataList);
                    OilBll.ConvertToDatasB(ref infoB, oilDataList); //只保存表的数据    


                     
                    //CurveParmTypeAccess curveParmTypeAccess = new CurveParmTypeAccess();// 提取数据库中的XY参数
                    //List<CurveParmTypeEntity> curveParmTypeList = curveParmTypeAccess.Get("1=1").ToList();

                    //var curveX = curveParmTypeList.Where(o=>o.ItemCode == "ECP").FirstOrDefault();
                    ////下面的函数获取当前原油的曲线函数必须放在最前面，不能挪位置。
                    //DatabaseA.Curve.DataBaseACurve.getCurrentCurveFromDataGridViewDataBaseB(ref this._oilB, this.dataGridView, this._typeCode, this._curveX, this._curveY);
            
                }              
            }
            else
            {
                MessageBox.Show(result + "表,数据检验出错!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.Text = "原始库原油数据-" + this._oilA.crudeIndex;
            this.Name = this._oilA.crudeIndex + "A";
        }

        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public int SaveInfoB(ref OilInfoBEntity oilInfoB)
        {
            var gridOilInfo = this.gridOilInfoA1;
            if (gridOilInfo == null)
                return -1;
 
            return gridOilInfo.SaveInfoB(ref oilInfoB); //保存信息表;
        }         
        #endregion

        #region "输出EXCEL"       
        /// <summary>
        /// 输出详评文件
        /// </summary>
        public int outDetailExcel()
        {
            int result = 0;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择输出模版";
            openFileDialog.Filter = "原油数据文件 (*.xls)|*.xls";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._oilA = getData();
                result = DataToExcelBll.DataToExcel(this._oilA,null, openFileDialog.FileName,null,"A");//下层调用函数
            }
            else
            {
                return 0;
            }
            return result;
        }
        /// <summary>
        /// 数据严格检查
        /// </summary>
        public void addRemarkToForm()
        {
            this.remarkRichBox.Text = string.Empty;
            this.remarkRichBox.Text = getRemakfromDataBase();
            if (this.remarkRichBox.Text != "")
                splitLeftOpen();
            this.remarkRichBox.ReadOnly = true;
        }
        /// <summary>
        /// 从数据库中获取批注信息
        /// </summary>
        /// <returns></returns>
        private string getRemakfromDataBase()
        {            
            RemarkAccess remarkAccess = new RemarkAccess();
            List<RemarkEntity> remarkEntityList = remarkAccess.getRemakList("select * from remark where  oilInfoID = " + this._oilA.ID).ToList();
            StringBuilder strBuild = new StringBuilder();
            foreach (RemarkEntity remark in remarkEntityList)
            {
                if (!string.IsNullOrWhiteSpace(remark.LabRemark))
                    strBuild.Append(remark.TableName + (remark.OilTableRow.RowIndex + 1) + "行" + remark.OilTableCol.colName + "列 实测值:" + remark.LabRemark + "\n");
                if (!string.IsNullOrWhiteSpace(remark.CalRemark))
                    strBuild.Append(remark.TableName + (remark.OilTableRow.RowIndex + 1) + "行" + remark.OilTableCol.colName + "列 校正值:" + remark.CalRemark + "\n");
            }
         
            return strBuild.ToString();
        }
        /// <summary>
        /// 添加评论
        /// </summary>
        /// <returns></returns>
        public RemarkEntity addRemark()
        {
            RemarkEntity remarkEntity = new RemarkEntity();
            RemarkAccess remarkAccess = new RemarkAccess();   
            if (this.tabControl1.SelectedTab.Text == EnumTableType.Info.GetDescription())
            {
                return null;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
            {
                DataGridViewCell currentCell = this.dgvWhole.CurrentCell;
                if (currentCell == null)
                    return null;
                if (currentCell.ColumnIndex <  4 || currentCell.RowIndex < 0)
                    return null;
                 int columnIndex = (currentCell.ColumnIndex - 4) /2;
                
                 OilDataEntity oilData = this.dgvWhole.GetDataByRowIndexColumnIndex(currentCell.RowIndex, columnIndex);
                 RemarkEntity tempRemarkEntity = remarkAccess.Get("oilInfoID = " + oilData.oilInfoID + "and oilTableColID = " + oilData.oilTableColID + "and oilTableRowID = " + oilData.oilTableRowID).FirstOrDefault();

                 if (tempRemarkEntity == null)
                 {
                     remarkEntity.oilTableRowID = oilData.oilTableRowID;
                     remarkEntity.oilTableColID = oilData.oilTableColID;
                     remarkEntity.oilInfoID = oilData.oilInfoID;

                     if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                         remarkEntity.LaborCal = 0;
                     else
                         remarkEntity.LaborCal = 1;
                 }
                 else
                 {
                     if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                         tempRemarkEntity.LaborCal = 0;
                     else
                         tempRemarkEntity.LaborCal = 1;

                     return tempRemarkEntity;
                 }
                
                return remarkEntity;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Light.GetDescription())
            {
                DataGridViewCell currentCell = this.dgvLight.CurrentCell;
                if (currentCell == null)
                    return null;
                if (currentCell.ColumnIndex <  4 || currentCell.RowIndex < 0)
                    return null;
                int columnIndex = (currentCell.ColumnIndex - 4) / 2;
                OilDataEntity oilData = this.dgvWhole.GetDataByRowIndexColumnIndex(currentCell.RowIndex, columnIndex);
                RemarkEntity tempRemarkEntity = remarkAccess.Get("oilInfoID = " + oilData.oilInfoID + "and oilTableColID = " + oilData.oilTableColID + "and oilTableRowID = " + oilData.oilTableRowID).FirstOrDefault();
                
                if (tempRemarkEntity == null)
                {
                    remarkEntity.oilTableRowID = oilData.oilTableRowID;
                    remarkEntity.oilTableColID = oilData.oilTableColID;
                    remarkEntity.oilInfoID = oilData.oilInfoID;

                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        remarkEntity.LaborCal = 0;
                    else
                        remarkEntity.LaborCal = 1;
                }
                else
                {
                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        tempRemarkEntity.LaborCal = 0;
                    else
                        tempRemarkEntity.LaborCal = 1;

                    return tempRemarkEntity;
                }
                return remarkEntity;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.GCInput.GetDescription())
            {
                DataGridViewCell currentCell = this.dgvGCInput.CurrentCell;
                if (currentCell == null)
                    return null;
                if (currentCell.ColumnIndex <  4 || currentCell.RowIndex < 0)
                    return null;
                int columnIndex = (currentCell.ColumnIndex - 4) / 2;
                OilDataEntity oilData = this.dgvGCInput.GetDataByRowIndexColumnIndex(currentCell.RowIndex, columnIndex);
                RemarkEntity tempRemarkEntity = remarkAccess.Get("oilInfoID = " + oilData.oilInfoID + "and oilTableColID = " + oilData.oilTableColID + "and oilTableRowID = " + oilData.oilTableRowID).FirstOrDefault();
                
                if (tempRemarkEntity == null)
                {
                    remarkEntity.oilTableRowID = oilData.oilTableRowID;
                    remarkEntity.oilTableColID = oilData.oilTableColID;
                    remarkEntity.oilInfoID = oilData.oilInfoID;

                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        remarkEntity.LaborCal = 0;
                    else
                        remarkEntity.LaborCal = 1;
                }
                else
                {
                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        tempRemarkEntity.LaborCal = 0;
                    else
                        tempRemarkEntity.LaborCal = 1;

                    return tempRemarkEntity;
                }
                return remarkEntity;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.GC.GetDescription())
            {
                DataGridViewCell currentCell = this.dgvGC.CurrentCell;
                if (currentCell == null)
                    return null;
                if (currentCell.ColumnIndex <  4 || currentCell.RowIndex < 0)
                    return null;
                int columnIndex = (currentCell.ColumnIndex - 4) / 2;
                OilDataEntity oilData = this.dgvGC.GetDataByRowIndexColumnIndex(currentCell.RowIndex, columnIndex);
                RemarkEntity tempRemarkEntity = remarkAccess.Get("oilInfoID = " + oilData.oilInfoID + "and oilTableColID = " + oilData.oilTableColID + "and oilTableRowID = " + oilData.oilTableRowID).FirstOrDefault();
                 
                if (tempRemarkEntity == null)
                {
                    remarkEntity.oilTableRowID = oilData.oilTableRowID;
                    remarkEntity.oilTableColID = oilData.oilTableColID;
                    remarkEntity.oilInfoID = oilData.oilInfoID;

                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        remarkEntity.LaborCal = 0;
                    else
                        remarkEntity.LaborCal = 1;
                }
                else
                {
                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        tempRemarkEntity.LaborCal = 0;
                    else
                        tempRemarkEntity.LaborCal = 1;

                    return tempRemarkEntity;
                }
                return remarkEntity;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.GCLevel.GetDescription())
            {
                DataGridViewCell currentCell = this.dgvGCLeve.CurrentCell;
                if (currentCell == null)
                    return null;
                if (currentCell.ColumnIndex <  4 || currentCell.RowIndex < 0)
                    return null;
                int columnIndex = (currentCell.ColumnIndex - 4) / 2;
                OilDataEntity oilData = this.dgvGCLeve.GetDataByRowIndexColumnIndex(currentCell.RowIndex, columnIndex);
                RemarkEntity tempRemarkEntity = remarkAccess.Get("oilInfoID = " + oilData.oilInfoID + "and oilTableColID = " + oilData.oilTableColID + "and oilTableRowID = " + oilData.oilTableRowID).FirstOrDefault();
                
                if (tempRemarkEntity == null)
                {
                    remarkEntity.oilTableRowID = oilData.oilTableRowID;
                    remarkEntity.oilTableColID = oilData.oilTableColID;
                    remarkEntity.oilInfoID = oilData.oilInfoID;

                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        remarkEntity.LaborCal = 0;
                    else
                        remarkEntity.LaborCal = 1;
                }
                else
                {
                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        tempRemarkEntity.LaborCal = 0;
                    else
                        tempRemarkEntity.LaborCal = 1;

                    return tempRemarkEntity;
                }
                return remarkEntity;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
            {
                DataGridViewCell currentCell = this.dgvNarrow.CurrentCell;
                if (currentCell == null)
                    return null;
                if (currentCell.ColumnIndex < 4 || currentCell.RowIndex < 0)
                    return null;
                int columnIndex = (currentCell.ColumnIndex - 4) / 2;
                OilDataEntity oilData = this.dgvNarrow.GetDataByRowIndexColumnIndex(currentCell.RowIndex, columnIndex);
                RemarkEntity tempRemarkEntity = remarkAccess.Get("oilInfoID = " + oilData.oilInfoID + "and oilTableColID = " + oilData.oilTableColID + "and oilTableRowID = " + oilData.oilTableRowID).FirstOrDefault();
                
                if (tempRemarkEntity == null)
                {
                    remarkEntity.oilTableRowID = oilData.oilTableRowID;
                    remarkEntity.oilTableColID = oilData.oilTableColID;
                    remarkEntity.oilInfoID = oilData.oilInfoID;

                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        remarkEntity.LaborCal = 0;
                    else
                        remarkEntity.LaborCal = 1;
                }
                else
                {
                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        tempRemarkEntity.LaborCal = 0;
                    else
                        tempRemarkEntity.LaborCal = 1;

                    return tempRemarkEntity;
                }
                return remarkEntity;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
            {
                DataGridViewCell currentCell = this.dgvWide.CurrentCell;
                if (currentCell == null)
                    return null;
                if (currentCell.ColumnIndex < 4 || currentCell.RowIndex < 0)
                    return null;
                int columnIndex = (currentCell.ColumnIndex - 4) / 2;
                OilDataEntity oilData = this.dgvWide.GetDataByRowIndexColumnIndex(currentCell.RowIndex, columnIndex);
                RemarkEntity tempRemarkEntity = remarkAccess.Get("oilInfoID = " + oilData.oilInfoID + "and oilTableColID = " + oilData.oilTableColID + "and oilTableRowID = " + oilData.oilTableRowID).FirstOrDefault();
               
                if (tempRemarkEntity == null)
                {
                    remarkEntity.oilTableRowID = oilData.oilTableRowID;
                    remarkEntity.oilTableColID = oilData.oilTableColID;
                    remarkEntity.oilInfoID = oilData.oilInfoID;

                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        remarkEntity.LaborCal = 0;
                    else
                        remarkEntity.LaborCal = 1;
                }
                else
                {
                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        tempRemarkEntity.LaborCal = 0;
                    else
                        tempRemarkEntity.LaborCal = 1;

                    return tempRemarkEntity;
                }
                return remarkEntity;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
            {
                DataGridViewCell currentCell = this.dgvResidue.CurrentCell;
                if (currentCell == null)
                    return null;
                if (currentCell.ColumnIndex < 4 || currentCell.RowIndex < 0)
                    return null;
                int columnIndex = (currentCell.ColumnIndex - 4) / 2;
                OilDataEntity oilData = this.dgvResidue.GetDataByRowIndexColumnIndex(currentCell.RowIndex, columnIndex);
                RemarkEntity tempRemarkEntity = remarkAccess.Get("oilInfoID = " + oilData.oilInfoID + "and oilTableColID = " + oilData.oilTableColID + "and oilTableRowID = " + oilData.oilTableRowID).FirstOrDefault();
                 
                if (tempRemarkEntity == null)
                {
                    remarkEntity.oilTableRowID = oilData.oilTableRowID;
                    remarkEntity.oilTableColID = oilData.oilTableColID;
                    remarkEntity.oilInfoID = oilData.oilInfoID;

                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        remarkEntity.LaborCal = 0;
                    else
                        remarkEntity.LaborCal = 1;
                }
                else
                {
                    if ((currentCell.ColumnIndex - 4) % 2 == 0)//实测列
                        tempRemarkEntity.LaborCal = 0;
                    else
                        tempRemarkEntity.LaborCal = 1;

                    return tempRemarkEntity;
                }
                return remarkEntity;
            }

            return remarkEntity;
        }

        #endregion

        #endregion
      
        #region "数据检查"
        /// <summary>
        /// 数据检查检查关键项，数据格式及范围审查
        /// </summary>
        /// <returns>错误提示字符串</returns>
        private string oilDataCheck()
        {
            //this._gridOil.ClearRemarkFlat();
            string str = string.Empty; 
            OilDataCheck dataCheck = new OilDataCheck(this.dgvWhole,this.dgvLight ,this.dgvNarrow ,this.dgvWide ,this.dgvResidue);//严格审查核心，构造函数新建变量
            if (this.tabControl1.SelectedTab.Text == EnumTableType.Info.GetDescription())//原油信息表审查
            {
                str = this.gridOilInfoA1.DataCheck();
                if (str.Equals(string.Empty))
                    this.gridOilInfoA1.IsValidated = true;
            }               
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())//原油性质表审查
                str = dataCheck.CheckAllDataListError(EnumTableType.Whole);//审查核心：CheckAllDataListError()
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Light.GetDescription()) { }//轻端表暂不做审查
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.GCInput.GetDescription()) { }//GC输入表暂不做审查
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.GC.GetDescription()) { }      
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.GCLevel.GetDescription()) { }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())//窄馏分表审查
                str = dataCheck.CheckAllDataListError(EnumTableType.Narrow);
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
                str = dataCheck.CheckAllDataListError(EnumTableType.Wide);
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
                str = dataCheck.CheckAllDataListError(EnumTableType.Residue);

            return str;
        }
        /// <summary>
        /// 数据严格检查
        /// </summary>
        public void DataCheck()
        {
            this.txtAlert.Text = oilDataCheck();//严格审查核心
            if (this.txtAlert.Text != "")
            {
                splitLeftOpen();
                this.tabControlNavigate.SelectedIndex = 0;
            }
            //this._errValidate = this.txtAlert.Text == "" ? true : false;
        }
        #endregion

        #region "数据补充"
        /// <summary>
        /// 数据补充
        /// </summary>
        public void DataSupplement()
        {
            if (this.tabControl1.SelectedTab.Text == EnumTableType.Info.GetDescription())
            {
                RIPP.OilDB.Data.DataSupplement.OilDataSupplement oildatasupplment = new OilDB.Data.DataSupplement.OilDataSupplement(this.gridOilInfoA1, this._oilA, this.dgvWhole, this.dgvNarrow);
                oildatasupplment.OilInfoLinkSupplement();//信息表数据补充
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
            {
                RIPP.OilDB.Data.DataSupplement.OilDataSupplement oildatasupplment = new OilDB.Data.DataSupplement.OilDataSupplement(this.dgvWhole, this.dgvLight,this.dgvGC, this.dgvNarrow, this.dgvWide, this.dgvResidue, EnumTableType.Whole);
                oildatasupplment.WholeLinkSupplement();//原油表数据补充
            }           
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
            {
                RIPP.OilDB.Data.DataSupplement.OilDataSupplement oildatasupplment = new OilDB.Data.DataSupplement.OilDataSupplement(this.dgvWhole, this.dgvLight, this.dgvGC, this.dgvNarrow, this.dgvWide, this.dgvResidue, EnumTableType.Narrow);
                oildatasupplment.NarrowGridOilLinkSupplement();//窄馏分表数据补充
                OilDataEntity ICPData = this.dgvNarrow.GetDataByRowItemCodeColumnIndex("ICP", 0);
                string strICP0 = ICPData == null ? string.Empty : ICPData.calData;

                if (this._oilA.ICP0 != strICP0)
                {
                    this._oilA.ICP0 = strICP0;
                    this._isChanged = true;
                }
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())///宽馏分补充
            {
                RIPP.OilDB.Data.DataSupplement.OilDataSupplement oildatasupplment = new OilDB.Data.DataSupplement.OilDataSupplement(this.dgvWhole, this.dgvLight, this.dgvGC, this.dgvNarrow, this.dgvWide, this.dgvResidue, EnumTableType.Wide);
                oildatasupplment.WideGridOilLinkSupplement();//宽馏分表数据补充
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
            {
                RIPP.OilDB.Data.DataSupplement.OilDataSupplement oildatasupplment = new OilDB.Data.DataSupplement.OilDataSupplement(this.dgvWhole, this.dgvLight, this.dgvGC, this.dgvNarrow, this.dgvWide, this.dgvResidue, EnumTableType.Residue);
                oildatasupplment.ResidueLinkSupplement();//渣油表数据补充
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.SimulatedDistillation.GetDescription())
            {

            }
            //}

        }

        /// <summary>
        /// 渣油表强制补充
        /// </summary>
        public void DataCorrectionSupplement()
        {           
            //if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
            //{
                RIPP.OilDB.Data.DataSupplement.OilDataSupplement oildatasupplment = new OilDB.Data.DataSupplement.OilDataSupplement(this.dgvWhole, this.dgvLight, this.dgvGC, this.dgvNarrow, this.dgvWide, this.dgvResidue, EnumTableType.Residue);
                oildatasupplment.DataCorrectionSupplement();
            //}
        }
        #endregion

        #region  "数据审查"
        /// <summary>
        /// 趋势审查
        /// </summary>
        /// <returns></returns>
        public void TrendCheck()
        {
            string str = string.Empty;
            try
            {
                
                OilDataTrendCheck trendCheck = new OilDataTrendCheck(this.dgvNarrow, this.dgvWide, this.dgvResidue);
                if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
                {
                    this.StartWaiting();
                    str = trendCheck.trendCheck(EnumTableType.Narrow);
                }                   
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
                {
                    this.StartWaiting();
                    str = trendCheck.trendCheck(EnumTableType.Wide);
                }
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
                {
                    this.StartWaiting();
                    str = trendCheck.trendCheck(EnumTableType.Residue);
                }
                   
            }
            catch (Exception ex)
            {
                Log.Error("原油趋势审查错误：" + ex.ToString());
                return;
            }
            finally
            {
                this.StopWaiting();
            }

            this.richTextBoxWarning.Text = str;
            if (this.richTextBoxWarning.Text != "")
            {
                splitLeftOpen();
                this.tabControlNavigate.SelectedIndex = 1; 
            }
        }
        /// <summary>
        /// 范围审查
        /// </summary>
        /// <returns></returns>
        public void rangeCheck()
        {
            string str = string.Empty;
            try
            {
                this.StartWaiting();
                OilDataRangeCheck rangeCheck = new OilDataRangeCheck(this.dgvWhole, this.dgvNarrow, this.dgvWide, this.dgvResidue);
                if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
                    str = rangeCheck.rangeCheck(EnumTableType.Whole);
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
                    str = rangeCheck.rangeCheck(EnumTableType.Narrow);
                if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
                    str = rangeCheck.rangeCheck(EnumTableType.Wide);
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
                    str = rangeCheck.rangeCheck(EnumTableType.Residue);
            }
            catch (Exception ex)
            {
                Log.Error("原油范围审查错误：" + ex.ToString());
                return;
            }
            finally
            {
                this.StopWaiting();
            }
            this.richTextBoxWarning.Text = str;
            if (this.richTextBoxWarning.Text != "")
            {
                splitLeftOpen();
                this.tabControlNavigate.SelectedIndex = 1;
            }
        }     
        /// <summary>
        /// 核算审查
        /// </summary>
        /// <returns></returns>
        public void accountCheck()
        {
            //OilAudit.FrmAccountAudit temp = new OilAudit.FrmAccountAudit(this._oilA);
            OilAudit.FrmAccountAudit temp = new OilAudit.FrmAccountAudit(this.dgvWhole, this.dgvNarrow, this.dgvWide, this.dgvResidue);
            temp.Show();
            //FrmExperienceCalCheck tempForm = (FrmExperienceCalCheck)GetChildFrm("FrmExperienceCalCheck");
            //if (tempForm == null)
            //{
            //    FrmExperienceCalCheck frmCalCheck = new FrmExperienceCalCheck(this._oilA);
            //    frmCalCheck.Name = "FrmExperienceCalCheck";
            //    frmCalCheck.Text = "核算审查";
            //    frmCalCheck.Show();
            //}
            //else
            //    tempForm.Activate();

            //if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
            //{
          
            //}
        }
        /// <summary>
        /// 关联审查
        /// </summary>
        /// <returns></returns>
        public void LinkCheck(bool showToolTip)
        {
            OilDataLinkCheck linkCheck = new OilDataLinkCheck(this.dgvWhole,this.dgvLight, this.dgvNarrow, this.dgvWide, this.dgvResidue);
            linkCheck.ShowTip = true;
            try
            {
                this.StartWaiting();
                if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
                {
                    linkCheck.AllDatasLinkCheck(EnumTableType.Whole, showToolTip);
                }
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
                {
                    linkCheck.AllDatasLinkCheck(EnumTableType.Narrow, showToolTip);
                }
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
                {
                    linkCheck.AllDatasLinkCheck(EnumTableType.Wide, showToolTip);
                }
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
                {
                    linkCheck.AllDatasLinkCheck(EnumTableType.Residue, showToolTip);
                }
            }
            catch (Exception ex)
            {
                Log.Error("原油关联审查错误:" + ex.ToString());
                return;
            }
            finally
            {
                this.StopWaiting();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void showToolTip()
        {
            if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
            {
                //this.dgvWhole
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
            {
                //linkCheck.AllDatasLinkCheck(EnumTableType.Narrow);
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
            {
                //linkCheck.AllDatasLinkCheck(EnumTableType.Wide);
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
            {
               // linkCheck.AllDatasLinkCheck(EnumTableType.Residue);
            }
        }
        #endregion

        #region 导航的关闭按钮

        /// <summary>
        /// 重绘时添加一个关闭按钮
        /// </summary>   
        private void splitContainer_Panel1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Rectangle myTabRect = this.splitContainer.Panel1.ClientRectangle;
                //画一个矩形框   
                using (Pen p = new Pen(Color.White))
                {
                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                    myTabRect.Width = CLOSE_SIZE;
                    myTabRect.Height = CLOSE_SIZE;
                    e.Graphics.DrawRectangle(p, myTabRect);
                }

                //填充矩形框   
                Color recColor = Color.White;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTabRect);
                }

                //画关闭符号   
                using (Pen objpen = new Pen(Color.Black))
                {
                    //"\"线   
                    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                    e.Graphics.DrawLine(objpen, p1, p2);

                    //"/"线   
                    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                    e.Graphics.DrawLine(objpen, p3, p4);

                    // 图形转化为位图
                    Bitmap bt = new Bitmap(20, 20);
                    Point p5 = new Point(myTabRect.X - 50, 4);
                    e.Graphics.DrawImage(bt, p5);
                }
                e.Graphics.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// 鼠标点击导航关闭
        /// </summary>   
        private void splitContainer_Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;

                //计算关闭区域      
                Rectangle myTabRect = this.splitContainer.Panel1.ClientRectangle;

                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;

                //如果鼠标在区域内就关闭选项卡      
                bool isClose = x > myTabRect.X && x < myTabRect.Right && y > myTabRect.Y && y < myTabRect.Bottom;
                if (isClose == true)
                {
                    this.splitLeftClose();
                }
            }
        }

        #endregion

        #region "显示和隐藏列"

        /// <summary>
        /// 显示和隐藏所有校正列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void showHideLabCol(bool isShow)
        {
            if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
            {
                if (this.dgvGC.HiddenColumnType == GridOilColumnType.Lab)
                    this.dgvGC.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvGC.HiddenColumnType == GridOilColumnType.None)
                    this.dgvGC.HiddenColumnType = GridOilColumnType.Lab;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Light.GetDescription())
            {
                if (this.dgvLight.HiddenColumnType == GridOilColumnType.Lab)
                    this.dgvLight.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvLight.HiddenColumnType == GridOilColumnType.None)
                    this.dgvLight.HiddenColumnType = GridOilColumnType.Lab;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
            {
                if (this.dgvNarrow.HiddenColumnType == GridOilColumnType.Lab)
                    this.dgvNarrow.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvNarrow.HiddenColumnType == GridOilColumnType.None)
                    this.dgvNarrow.HiddenColumnType = GridOilColumnType.Lab;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
            {
                if (this.dgvWide.HiddenColumnType == GridOilColumnType.Lab)
                    this.dgvWide.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvWide.HiddenColumnType == GridOilColumnType.None)
                    this.dgvWide.HiddenColumnType = GridOilColumnType.Lab;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
            {
                if (this.dgvResidue.HiddenColumnType == GridOilColumnType.Lab)
                    this.dgvResidue.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvResidue.HiddenColumnType == GridOilColumnType.None)
                    this.dgvResidue.HiddenColumnType = GridOilColumnType.Lab;
            }
        }

        /// <summary>
        /// 显示和隐藏所有校正列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void showHideCalCol(bool isShow)
        {
            if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
            {
                if (this.dgvGC.HiddenColumnType == GridOilColumnType.Calc)
                    this.dgvGC.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvGC.HiddenColumnType == GridOilColumnType.None)
                    this.dgvGC.HiddenColumnType = GridOilColumnType.Calc;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Light.GetDescription())
            {
                if (this.dgvLight.HiddenColumnType == GridOilColumnType.Calc)
                    this.dgvLight.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvLight.HiddenColumnType == GridOilColumnType.None)
                    this.dgvLight.HiddenColumnType = GridOilColumnType.Calc;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
            {
                if (this.dgvNarrow.HiddenColumnType == GridOilColumnType.Calc)
                    this.dgvNarrow.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvNarrow.HiddenColumnType == GridOilColumnType.None)
                    this.dgvNarrow.HiddenColumnType = GridOilColumnType.Calc;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
            {
                if (this.dgvWide.HiddenColumnType == GridOilColumnType.Calc)
                    this.dgvWide.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvWide.HiddenColumnType == GridOilColumnType.None)
                    this.dgvWide.HiddenColumnType = GridOilColumnType.Calc;
            }
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
            {
                if (this.dgvResidue.HiddenColumnType == GridOilColumnType.Calc)
                    this.dgvResidue.HiddenColumnType = GridOilColumnType.None;
                else if (this.dgvResidue.HiddenColumnType == GridOilColumnType.None)
                    this.dgvResidue.HiddenColumnType = GridOilColumnType.Calc;
            }
        }


        #endregion

        #region "old Code"
        ///// <summary>
        ///// 关闭前提示保存
        ///// </summary>      
        //private void FrmOilDataA_FormClosing(object sender, FormClosingEventArgs e)
        //{         
        //    if (this.isChanged)
        //    {
        //        DialogResult r = MessageBox.Show("是否保存数据！", "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
        //        if (r == DialogResult.Yes)
        //        {
        //            this.SaveA();
        //            FrmMain frmMain = (FrmMain)this.MdiParent;
        //            DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)frmMain.GetChildFrm("frmOpenA");
        //            if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
        //            {
        //                frmOpenA.refreshGridList();
        //            }

        //            this.isChanged = false;
        //        }
        //        else if (r == DialogResult.No)
        //        {
        //            FrmMain frmMain = (FrmMain)this.MdiParent;
        //            DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)frmMain.GetChildFrm("frmOpenA");
        //            if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
        //            {
        //                frmOpenA.refreshGridList();
        //            }

        //            this.isChanged = false;
        //        }
        //        else if (r == DialogResult.Cancel)
        //        {
        //            e.Cancel = true;
        //        }
        //    }
        //}
        #endregion 

        /// <summary>
        /// 关闭前提示保存
        /// </summary>      
        private void FrmOilDataA_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭录入数据表窗口前先确定作图窗口是否处于打开状态
            foreach (Form frm in this.ParentForm.MdiChildren)
            {
                if (frm.Text.Contains(this._oilA.crudeIndex) && frm.Text.Contains(this._oilA.crudeName) && this._oilA.crudeIndex != "" && this._oilA.crudeName != "")
                {
                    DialogResult dr = MessageBox.Show("当前录入数据表的作图窗口处于打开状态，请先关闭对应的作图窗口!", "提示");
                    e.Cancel = true;
                }
            }

            if (this.isChanged)
            {
                DialogResult r = MessageBox.Show("是否保存" + this._oilA.crudeIndex + "数据到原始库？", "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    this.SaveA();                 
                    this.isChanged = false;
                }
                else if (r == DialogResult.No)
                {
                    #region "涉及到导入数据库的保存和修改后的保存"
                    //OilBll.delete(this._oilA.ID, LibraryType.LibraryA);
                    //FrmMain frmMain = (FrmMain)this.MdiParent;
                    //DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)frmMain.GetChildFrm("frmOpenA");
                    //if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
                    //{
                    //    frmOpenA.refreshGridList(false);
                    //}
                    #endregion
                    
                    this.isChanged = false;
                }
                else if (r == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            if (this.isInfoChanged)
            {
                this.saveInfo();
                this.isInfoChanged = false;
            }
        }
        

        #region "得到表格上的数据"
      
        /// <summary>
        /// 获取当前的所有数据
        /// </summary>
        /// <returns></returns>
        public OilInfoEntity getData()
        {
            this._oilA.OilDatas.Clear();
            this.gridOilInfoA1.getOilInfo(ref this._oilA);
            List<OilDataEntity> dataList = new List<OilDataEntity>();
            dataList.AddRange(this.dgvLight.GetAllData());
            dataList.AddRange(this.dgvWhole.GetAllData());
            dataList.AddRange(this.dgvGCLeve.GetAllData());           
            dataList.AddRange(this.dgvNarrow.GetAllData ());
            dataList.AddRange(this.dgvWide.GetAllData());
            dataList.AddRange(this.dgvResidue.GetAllData());
            this._oilA.OilDatas.AddRange(dataList);
            return this._oilA;   
        }

        #endregion
        
        #region "快捷键"
        /// <summary>
        /// GC输入表的统计功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnStatus_Click(object sender, EventArgs e)
        {
            GCInput gcInput = new GCInput(this.dgvLight, this.dgvGCInput, this.dgvGC, this.dgvGCLeve, this.dgvNarrow);
            gcInput.Calculation();
        }

        public void Redo()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.Redo();
        }

        public void Undo()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.Undo();
        }

        public void Cut()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.Cut();
        }

        public void Copy()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.Copy();
        }

        public void Paste()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.Paste();
        }

        public void Empty()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.Empty();
        }

        public void AddColumn(bool isLeft)
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.AddColumn(isLeft);
        }

        public void DeleteColumn()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.DeleteColumn();
        }

        public void Save()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.Save();
        }
        public void Lab2Cal()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.Lab2Cal();
        }

        public void ClearLinkTips()
        {
            var t = GetCurrentGridOilEditor();
            if (t != null)
                t.ClearLinkTips();
        }
        public GridOilEditorCommandType CheckCommandState()
        {
            var t = GetCurrentGridOilEditor();
            if (t == null)
                return GridOilEditorCommandType.None;
            return t.CheckCommandState();
        }

        public IGridOilEditor GetCurrentGridOilEditor()
        {
            if (this.ActiveControl is IGridOilEditor)
                return this.ActiveControl as IGridOilEditor;
            var currentTab = tabControl1.SelectedTab;
            foreach (Control v in currentTab.Controls)
                if (v.Visible && v is IGridOilEditor)
                    return v as IGridOilEditor;
            return null;
        }
        #endregion 

        /// <summary>
        /// tabPage切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            previousSelectTabText = this.tabControl1.SelectedTab.Text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {    
            if (previousSelectTabText == EnumTableType.Info.GetDescription())
            {
                if (!this.gridOilInfoA1.isChanged && this._oilA.ID > 0)
                    return;
                if (this.gridOilInfoA1.Save() < 0)
                {
                    e.Cancel = true;
                    MessageBox.Show("原油信息表中的原油编号为空或重复!必须先填写。", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {                         
                    if (this.Name != this._oilA.crudeIndex + "A")
                    {
                        this.Text = "原始库原油数据-" + this._oilA.crudeIndex;
                        this.Name = this._oilA.crudeIndex + "A";   //在打开操作时要根据该名称判断窗口是否存在
                    }
                    DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)GetChildFrm("frmOpenA");
                    if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
                    {
                        frmOpenA.refreshGridList(false);
                    }                                
                }
            }       
        }
        /// <summary>
        /// tabPage切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedTab.Text == EnumTableType.Info.GetDescription())
                this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Info).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
                this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Light.GetDescription())
                this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Light).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.GCInput.GetDescription())
                this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.GCInput).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.GC.GetDescription())
                this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.GC).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
                this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
                this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
            else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
                this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");

        } 
        /// <summary>
        /// 根据窗体名称获取窗体
        /// </summary>
        /// <param name="childFrmName">窗体名称</param>
        /// <returns>存在返回1并不激活此窗口，不存在返回null</returns>
        public Form GetChildFrm(string childFrmName)
        {
            Form childFrm = null;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == childFrmName)
                {
                    childFrm = frm;
                    break;
                }
            }
            return childFrm;
        }
        /// <summary>
        /// 左侧信息栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlNavigate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControlNavigate.SelectedTab.Text == "帮助")
            {
                if (this.tabControl1.SelectedTab.Text == EnumTableType.Info.GetDescription())
                    this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Info).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Whole.GetDescription())
                    this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Light.GetDescription())
                    this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Light).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.GCInput.GetDescription())
                    this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.GCInput).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.GC.GetDescription())
                    this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.GC).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Narrow.GetDescription())
                    this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Wide.GetDescription())
                    this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
                else if (this.tabControl1.SelectedTab.Text == EnumTableType.Residue.GetDescription())
                    this.richTextBoxHelp.Text = this._OilTipTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).FirstOrDefault().Tip.Replace("\\r", "\r\n\n");
            }

        }

            
    }
}
