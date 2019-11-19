using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.OilDataManager.Forms.DatabaseA;
using RIPP.App.OilDataManager.Forms.DatabaseB;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System.Threading;
using RIPP.Lib;
using RIPP.App.OilDataManager.Forms.SystemM;
using RIPP.App.OilDataManager.Forms.LibManage;
using RIPP.App.OilDataManager.Forms.FrmBase;
using RIPP.App.OilDataManager.Forms.OilTool;
using RIPP.OilDB.UI.GridOil.V2.Model;
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.App.OilDataManager.Forms.DatabaseA.Curve;

namespace RIPP.App.OilDataManager.Forms
{
    public partial class FrmMain : Form
    {
        #region 私有属性

        private int _childFormNumber = 0;
        private bool _showHideLabCo = true;
        private string _role = "role2";  //权限简单内定两个角色
        private WaitingPanel waitingPanel; //全局声明
        private bool splitLeftVisible = true;//用于左侧的批注等信息栏是否显示

        /// <summary>
        /// 等待窗口
        /// </summary>
        private FrmWaiting myFrmWaiting;

        /// <summary>
        /// 等待线程
        /// </summary>
        private Thread waitingThread;
        /// <summary>
        /// 
        /// </summary>
        public string role
        {
            get { return _role; }
            set { _role = value; }
        }
        /// <summary>
        /// 是否在繁忙状态
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return waitingPanel.IsBusy;
            }
            set
            {
                waitingPanel.IsBusy = value;
            }
        }
        private bool bZH = true;
        /// <summary>
        /// 镇海功能
        /// </summary>
        public bool BZH
        {
            get { return bZH; }
            set { bZH = value; }
        }
        #endregion

        #region 构造函数

        public FrmMain()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized; //窗口最大化
            waitingPanel = new WaitingPanel(this);

        }
        public FrmMain(string role)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized; //窗口最大化
            waitingPanel = new WaitingPanel(this);
            this._role = role;
        }
        public void initMenu(bool bZH)
        {
            //S_MoudleAccess access = new S_MoudleAccess();
            //List<S_MoudleEntity> s_MoudleEntity = access.Get("1=1"); //选择当前表类型的所有列数据
            //bool flag = false;
            //foreach (ToolStripMenuItem MenuItem2 in menuStrip.Items)  //遍历一级菜单                
            //{
            //    flag = false;                                        //该一级菜单下如果有一个子菜单显示则该一级菜单要显示
            //    for (int i = 0; i < MenuItem2.DropDownItems.Count; i++)//遍历menustrip遍历二级菜单
            //    {
            //        for (int k = 0; k < s_MoudleEntity.Count; k++)  //对每一个二级菜单在数据库中查找到对应项
            //        {
            //            if (MenuItem2.DropDownItems[i].Name == s_MoudleEntity[k].name)
            //            {
            //                if (_role == "role2")
            //                {
            //                    if (s_MoudleEntity[k].role2 == false)
            //                        MenuItem2.DropDownItems[i].Visible = false;
            //                    else
            //                        flag = true;
            //                }
            //                else
            //                {
            //                    if (s_MoudleEntity[k].role1 == false)
            //                        MenuItem2.DropDownItems[i].Visible = false;
            //                    else
            //                        flag = true;
            //                }
            //            }
            //        }
            //    }
            //    MenuItem2.Visible = flag;
            //}
            this.BZH = bZH;
            isVisible(this.BZH);
        }
        /// <summary>
        /// 演示用（隐藏部分按钮)
        /// </summary>
        /// <param name="value"></param>
        private void isVisible(bool value)
        {
            value = !value;
           
            this.menuFileOpenC.Visible = value;//打开C库按钮
            this.menuFileInCru.Visible = value;//导入Crul文件
            this.menuFileInExcel.Visible = value;//导入Excel文件
            this.menuFileOutDetail.Visible = value;//输出到Excel
            this.toolStripSeparator3.Visible = value;
            this.toolStripSeparator13.Visible = value;

            this.menuDataCorrecion.Visible = value;

            this.menuCheck.Visible = value;
            this.MenuApply.Visible = value;
            this.menuSystem.Visible = value;

            this.toolStripButton7.Visible = value;//左移按钮
            this.toolStripButton8.Visible = value;//右移按钮

            this.toolBtnAddRemark.Visible = value;//批注

            this.toolStripButton11.Visible = value;//趋势审查
            this.toolStripButton12.Visible = value;//范围审查
            this.toolStripButton13.Visible = value;//核算审查
            this.toolStripButton14.Visible = value;//关联审查

            this.menuEditAddLeftCol.Visible = value;//添加左侧列
            this.menuEditAddRightCol.Visible = value;//添加右侧列

            this.toolStripButton15.Visible = value;//取消关联经验审查数据（最右边的小灯泡图标）

            this.toolStripButton1.Visible = value;//导入Excel文件
            this.toolStripButton2.Visible = value;//导入Cru文件

            this.toolStripMenuItem3.Visible = value;//录入菜单中的 GC标准表
            this.menuInputGC.Visible = value;//录入菜单中的 模拟馏程表

            toolStripSeparator11.Visible = value;
            menuFileOpenB.Visible = value;//打开B库
            menuFileSaveB.Visible = value;//保存到数据库
            menuLib1.Visible = value;//导入原始库
            menuLib2.Visible = value;//导出原始库
            menuLib3.Visible = value;//导入应用库
            menuHelp.Visible = value;//帮助
        }

        #endregion

        #region 主窗体公有函数

        /// <summary>
        /// 是否存在该类型的子窗体
        /// </summary>
        /// <param name="childFrmType">窗体类型</param>
        /// <returns>存在返回1并激活此窗口，不存在返回0</returns>
        public bool IsExistChildFrm(string childFrmType)
        {
            bool flag = false;
            foreach (Form frm in this.MdiChildren)
            {
                string str = frm.GetType().Name;
                if (frm.GetType().Name == childFrmType)
                {
                    frm.Activate();
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        /// <summary>
        /// 是否存在该类型的窗体
        /// </summary>
        /// <param name="childFrmType">窗体类型</param>
        /// <returns>存在返回1并激活此窗口，不存在返回0</returns>
        public bool IsExistFrm(string childFrmType)
        {
            bool flag = false;
            foreach (Form frm in Application.OpenForms)
            {
                string str = frm.GetType().Name;
                if (frm.GetType().Name == childFrmType)
                {
                    frm.Activate();
                    frm.WindowState = FormWindowState.Normal;
                    flag = true;
                    break;
                }
            }
            return flag;
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
        /// <summary>
        /// 用来判断是否存在当前数据表的作图窗口
        /// </summary>
        /// <returns></returns>
        private FrmCurveA frmCurveAIsExist()
        {
            FrmCurveA result = null;

            foreach (Form frm in this.MdiChildren)
            {
                if (((DatabaseA.FrmOilDataA)this.ActiveMdiChild).oil != null)
                {
                    if (frm.Text.Contains(((DatabaseA.FrmOilDataA)this.ActiveMdiChild).oil.crudeIndex) && frm.Text.Contains(((DatabaseA.FrmOilDataA)this.ActiveMdiChild).oil.crudeName))
                    {
                        result = (DatabaseA.FrmCurveA)frm;
                    }
                }
                else
                {

                }
            }
            return result;
        }
        #endregion

        #region 主窗体控件事件

        /// <summary>
        /// 取消子窗口左上角的图标
        /// </summary>    
        private void menuStrip_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (e.Item.ToString() == "System.Windows.Forms.MdiControlStrip+SystemMenuItem")
            {
                e.Item.Visible = false;
            }
        }

        #endregion

        #region 文件菜单

        /// <summary>
        /// 文件-新建，新建
        /// </summary>   
        private void menuFileNew_Click(object sender, EventArgs e)
        {
            OilInfoEntity oil = new OilInfoEntity();
            //oil.crudeIndex = "原油编号";
            DatabaseA.FrmOilDataA frmOilData = new DatabaseA.FrmOilDataA(oil);
            frmOilData.MdiParent = this;
            frmOilData.Text = "新建原始库文档 " + _childFormNumber++;
            frmOilData.Show();
        }

        /// <summary>
        /// 文件-打开，打开原始数据库A
        /// </summary>    
        private void menuFileOpenA_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmOpenA"))   //打开原始数据库A，只能存在一个窗口
            {
                DatabaseA.FrmOpenA frmOpen = new DatabaseA.FrmOpenA(this.BZH);
                frmOpen.MdiParent = this;
                frmOpen.Show();
            }
            else
            {
                foreach (Form frm in this.MdiChildren)
                {
                    if (frm.Name == "frmOpenA")
                    {
                        frm.WindowState = FormWindowState.Normal;
                    }
                }
            }
        }

        private string FrmOpenA()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 文件-打开，打开原始数据库B
        /// </summary>     
        private void menuFileOpenB_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmOpenB"))  //打开原始数据库B，只能存在一个窗口
            {
                DatabaseB.FrmOpenB frmOpen = new DatabaseB.FrmOpenB();
                frmOpen.MdiParent = this;
                frmOpen.Show();
            }
            else
            {
                foreach (Form frm in this.MdiChildren)
                {
                    if (frm.Name == "frmOpenB")
                    {
                        frm.WindowState = FormWindowState.Normal;
                    }
                }
            }
        }
        /// <summary>
        /// 打开C库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuFileOpenC_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmOpenC"))  //打开原始数据库C，只能存在一个窗口
            {
                DatabaseC.FrmOpenC frmOpen = new DatabaseC.FrmOpenC();
                frmOpen.MdiParent = this;
                frmOpen.Show();
            }
            else
            {
                foreach (Form frm in this.MdiChildren)
                {
                    if (frm.Name == "frmOpenC")
                    {
                        frm.WindowState = FormWindowState.Normal;//将最小化的窗体正常显示
                    }
                }
            }
        }
        /// <summary>
        /// 文件-关闭，关闭当前文件
        /// </summary>    
        private void menuFileClose_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)      // 可关闭当前窗口，不分窗口类别
            {
                this.ActiveMdiChild.Close();
            }
        }

        /// <summary>
        /// 文件- 导入cru文件
        /// </summary>    
        private void menuFileInCru_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "原油数据文件 (*.cru)|*.cru";
            myOpenFileDialog.RestoreDirectory = true;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //if (!this.IsExistChildFrm("FrmOilDataB"))
                    //{
                    this.IsBusy = true;
                    //this.StartWaiting();
                    OilInfoBEntity oil = OilBll.importCru(myOpenFileDialog.FileName);
                    //this.StopWaiting();
                    this.IsBusy = false;
                    if (oil.ID > 0)
                    {
                        DatabaseB.FrmOilDataB frmOilDataB = new DatabaseB.FrmOilDataB(oil);
                        frmOilDataB.MdiParent = this;
                        frmOilDataB.Text = "应用库原油数据： " + myOpenFileDialog.FileName;
                        frmOilDataB.Show();

                        DatabaseB.FrmOpenB frmOpenB = (DatabaseB.FrmOpenB)GetChildFrm("frmOpenB");
                        if (frmOpenB != null)  //如果打开原油库A的窗口存在，则更新
                        {
                            frmOpenB.refreshGridList(false);
                        }
                    }
                    else
                    {
                        MessageBox.Show("原油编号为空或已经存在!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导入的文件格式不正确!", "导入失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error("导入Cru文件" + ex);
                }
            }
        }


        /// <summary>
        /// 是否显示关联审查数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonToolTip_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).LinkCheck(false);
                }
            }
        }

        /// <summary>
        /// 文件-导入EXCEL文件
        /// </summary>      
        private void menuFileInExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "原油数据文件 (*.xls)|*.xls";
            myOpenFileDialog.RestoreDirectory = true;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //if (!this.IsExistChildFrm("FrmOilDataA"))
                    //{
                    this.IsBusy = true;
                    //this.StartWaiting();
                    OilInfoEntity oil = OilBll.importExcel2(myOpenFileDialog.FileName);
                    this.IsBusy = false;
                    //this.StopWaiting();
                    if (oil.ID > 0)
                    {
                        DatabaseA.FrmOilDataA frmOilDataA = new DatabaseA.FrmOilDataA(oil);
                        frmOilDataA.changeSave();//让窗体中的表格处于编辑状态
                        frmOilDataA.MdiParent = this;
                        frmOilDataA.Text = "原始库原油数据： " + myOpenFileDialog.FileName;
                        frmOilDataA.Show();

                        DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)GetChildFrm("frmOpenA");
                        if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
                            frmOpenA.refreshGridList(false);
                    }
                    else
                    {
                        MessageBox.Show("原油编号为空或已经存在!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导入的文件格式不正确!", "导入失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error("文件-导入EXCEL文件" + ex);
                }
            }
        }

        /// <summary>
        /// 文件-保存到A库
        /// </summary>   
        private void menuFileSaveA_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    if (((DatabaseA.FrmOilDataA)this.ActiveMdiChild).SaveA())
                    {
                        DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)GetChildFrm("frmOpenA");
                        if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
                        {
                            frmOpenA.refreshGridList(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 后台操作-保存到A库
        /// </summary>    
        private void bgwLogin_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        /// <summary>
        /// 后台操作-保存到A库完成
        /// </summary>    
        private void bgwLogin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /// <summary>
        /// 文件-保存到B库
        /// </summary>     
        private void menuFileSaveB_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmCurveA")//把处于选定状态的窗体保存.
                {
                    DatabaseA.FrmCurveA frmCurveA = (DatabaseA.FrmCurveA)this.ActiveMdiChild;
                    DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)GetChildFrm(frmCurveA.Oil.crudeIndex + "A");
                    if (frmOilDataA != null)
                    {
                        if (frmOilDataA.isChanged)
                            frmCurveA.HaveSave = false;
                    }
                    frmCurveA.SaveB();

                    DatabaseB.FrmOpenB frmOpenB = (DatabaseB.FrmOpenB)GetChildFrm("frmOpenB");
                    if (frmOpenB != null)  //如果打开原油库A的窗口存在，则更新
                    {
                        frmOpenB.refreshGridList(false);
                    }
                }
                else if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")//如果录入窗体A存在.
                {
                    DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;
                    this.IsBusy = true;
                    frmOilDataA.SaveB();
                    this.IsBusy = false;
                    DatabaseB.FrmOpenB frmOpenB = (DatabaseB.FrmOpenB)GetChildFrm("frmOpenB");
                    if (frmOpenB != null)  //如果打开原油库A的窗口存在，则更新
                    {
                        frmOpenB.refreshGridList(false);
                    }
                }
                else if (this.ActiveMdiChild.GetType().Name == "FrmOilDataB")//如果录入窗体B存在.
                {
                    DatabaseB.FrmOilDataB frmOilDataB = (DatabaseB.FrmOilDataB)this.ActiveMdiChild;
                    this.IsBusy = true;
                    frmOilDataB.SaveB();
                    this.IsBusy = false;
                    DatabaseB.FrmOpenB frmOpenB = (DatabaseB.FrmOpenB)GetChildFrm("frmOpenB");
                    if (frmOpenB != null)  //如果打开原油库A的窗口存在，则更新
                    {
                        frmOpenB.refreshGridList(false);
                    }
                }
            }
        }

        /// <summary>
        /// 文件-输出详评文件到Excel
        /// </summary>    
        private void menuFileOutDetail_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    int outResult = ((FrmOilDataA)this.ActiveMdiChild).outDetailExcel();//下层调用函数
                    if (outResult == 1)
                    {
                        MessageBox.Show("数据导出成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (outResult == 0)
                    {
                        return;//取消导出
                    }
                    else if (outResult == -1)
                    {
                        MessageBox.Show("当前系统尚未安装EXCEL软件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (outResult == -2)
                    {
                        MessageBox.Show("不能打开Excel进程,请关闭Excel后重试!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (outResult == -11)
                    {
                        MessageBox.Show("没有原油数据!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (outResult == -99)//找不到正确模版
                    {
                        MessageBox.Show("数据导出失败，找不到正确模板!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("数据导出失败,错误信息：\r\n" + outResult, "提示");
                    }
                }
            }
        }

        /// <summary>
        /// 文件-输出Excel文件
        /// </summary>      
        private void menuFileOutExcel_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 文件-退出，退出系统
        /// </summary>     
        private void menuFileQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 编辑

        public void editUnDoEnable(bool enabled)
        {
            this.tsBtnEditUnDo.Enabled = enabled;
        }
        public void editReDoEnable(bool enabled)
        {
            this.tsBtnEditReDo.Enabled = enabled;
        }

        /// <summary>
        /// 撤销
        /// </summary>      
        private void menuEditUnDo_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).Undo();
        }

        /// <summary>
        /// 重做
        /// </summary>   
        private void menuEditReDo_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).Redo();
        }
        /// <summary>
        /// 剪切
        /// </summary> 
        private void menuEditCut_Click(object sender, EventArgs e)
        {
            //if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            //{
            //    DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)GetChildFrm("FrmOilDataA");
            //    if (frmOilDataA.tabControl1.SelectedIndex == 0)
            //    {
            //        //frmOilDataA.tabControl1.
            //    }
            //    else
            //    {

            //    }
            //}

            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).Cut();
        }
        /// <summary>
        /// 复制
        /// </summary> 
        private void menuEditCopy_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).Copy();
        }
        /// <summary>
        /// 粘贴
        /// </summary> 
        private void menuEditPaste_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).Paste();
        }
        /// <summary>
        /// 删除
        /// </summary> 
        private void menuEditDelete_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).Empty();
        }
        /// <summary>
        /// 添加左侧列
        /// </summary> 
        private void menuEditAddLeftCol_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).AddColumn(true);
        }
        /// <summary>
        /// 添加右侧列
        /// </summary> 
        private void menuEditAddRightCol_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).AddColumn(false);
        }
        /// <summary>
        /// 删除列
        /// </summary> 
        private void menuEditDelCol_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).DeleteColumn();
        }

        #endregion

        #region "录入"

        /// <summary>
        /// 录入-原油信息
        /// </summary>     
        private void menuInputOilInfo_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex();
            }
        }

        /// <summary>
        /// 录入-原油性质
        /// </summary>   
        private void menuInputWhole_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("原油性质");
            }
        }

        /// <summary>
        /// 录入-轻端组成
        /// </summary>  
        private void menuInputLight_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("轻端表");
            }
        }


        /// <summary>
        /// GC输入表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripGCInput_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("GC输入表");
            }
        }
        /// <summary>
        /// GC统计表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripGCSta_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("GC统计表");
            }
        }
        /// <summary>
        /// GC标准表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripGCNormal_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("GC标准表");
            }
        }
        /// <summary>
        /// 录入-窄馏分性质
        /// </summary>  
        private void menuInputNarrow_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("窄馏分");
            }
        }
        /// <summary>
        /// 录入-宽馏分性质
        /// </summary>    
        private void menuInputWide_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("宽馏分");
            }
        }



        /// <summary>
        /// 录入-渣油数据
        /// </summary> 
        private void menuInputResident_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("渣油");
            }
        }
        /// <summary>
        /// 录入-模拟镏程表 
        /// </summary>  
        private void menuInputSmilatedDistalltion_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).setTabIndex("模拟镏程表");
            }
        }
        /// <summary>
        /// 录入-数据检查
        /// </summary>  
        private void menuInputCkeck_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).DataCheck();//校正值审查
            }
        }

        /// <summary>
        /// 录入-数据补充
        /// </summary>  
        private void menuInputFill_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).DataSupplement();//数据补充
            }
        }
        /// <summary>
        /// 数据校正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuDataCorrecion_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).DataCorrectionSupplement();
            }
        }

        /// <summary>
        /// 录入行和列设置
        /// </summary>  
        private void menuInputOption_Click(object sender, EventArgs e)
        {
            //打开子窗口
            if (!this.IsExistChildFrm("FrmInputRowOption"))
            {
                var frmInputOption = new RIPP.App.OilDataManager.Forms.DatabaseA.FrmInputRowOption(this.BZH);
                frmInputOption.Text = "录入表设置";
                frmInputOption.MdiParent = this;
                frmInputOption.Show();
            }
        }


        #endregion

        #region "审查"

        /// <summary>
        /// 趋势审查
        /// </summary>    
        private void menuCheckTrend_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    ((FrmOilDataA)this.ActiveMdiChild).TrendCheck();
                }
            }
        }

        /// <summary>
        /// 范围审查
        /// </summary>    
        private void menuCheckRange_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).rangeCheck();
                }
            }
        }
        /// <summary>
        /// 核算审查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCheckAccount_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).accountCheck();
                }
            }
        }
        /// <summary>
        /// 关联审查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCheckLink_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).LinkCheck(true);
                }
            }
        }
        /// <summary>
        /// 审查配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 审查配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmExperienceRangeCheck"))
            {
                FrmExperienceRangeCheck frm = new FrmExperienceRangeCheck();
                frm.MdiParent = this;
                frm.Name = "范围审查配置表";
                frm.Show();
            }
        }
        /// <summary>
        /// 趋势审查配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 趋势审查配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmExperienceTrendCheck"))
            {
                FrmExperienceTrendCheck frm = new FrmExperienceTrendCheck();
                frm.MdiParent = this;
                frm.Name = "趋势审查配置";
                frm.Show();
            }
        }
        #endregion

        #region "性质曲线"
        ///// <summary>
        ///// 收率曲线
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void menuCurveItem1_Click(object sender, EventArgs e)
        //{
        //    if (this.ActiveMdiChild != null)
        //    {
        //        if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
        //        {                 
        //            DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;     // 一条原油 
        //            if (frmOilDataA != null)
        //            {
        //                DatabaseA.FrmCurveA frmCurveA = new DatabaseA.FrmCurveA(frmOilDataA, CurveTypeCode.YIELD);
        //                //frmCurveA.Text = frmOilDataA.oil.crudeIndex + "收率曲线";
        //                frmCurveA.MdiParent = this;
        //                frmCurveA.Show();
        //            }
        //        }
        //        else if (this.ActiveMdiChild.GetType().Name == "FrmCurveA")
        //        {
        //            ((DatabaseA.FrmCurveA)this.ActiveMdiChild).init(CurveTypeCode.YIELD);
        //        }
        //    }
        //}

        /// <summary>
        /// 绘制曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolBtnCurve_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;     // 一条原油 
                    if (frmOilDataA != null)
                    {
                        bool bResult = true;
                        //关闭录入数据表窗口前先确定作图窗口是否处于打开状态
                        foreach (Form frm in this.MdiChildren)
                        {
                            if (frm is DatabaseA.FrmCurveA)
                            {
                                if (frm.Text.Contains(frmOilDataA.oil.crudeIndex) || frm.Text.Contains(frmOilDataA.oil.crudeName))                                 
                                {
                                    DialogResult dr = MessageBox.Show("当前录入数据表的作图窗口处于打开状态，请先关闭对应的作图窗口!", "提示");
                                    if (dr == System.Windows.Forms.DialogResult.OK)
                                    {
                                        bResult = false;
                                    }                                    
                                }                                   
                            }
                        }

                        if (bResult)
                            FrmCurveAFunction.saveOilBFromOilA(frmOilDataA.getData());//调用核心函数：frmOilDataA.getData()---取出A库数据;saveOilBFromOilA()-----自动曲线作图，保存B库数据
                    }
                }
            }
        }


        /// <summary>
        /// 收率曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCurveItem1_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;     // 一条原油 

                    if (frmOilDataA != null)
                    {
                        //if (!frmOilDataA.IsValidated)
                        //    MessageBox.Show("需要对原始库进行错误检查，是否退出？", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //else
                        //{
                        #region
                        //先判断作图窗口是否存在
                        FrmCurveA frm = frmCurveAIsExist();
                        if (frm != null)
                        {
                            frm.init(CurveTypeCode.YIELD);
                        }
                        else
                        {
                            try
                            {
                                this.StartWaiting();
                                DatabaseA.FrmCurveA frmCurveA = new DatabaseA.FrmCurveA(frmOilDataA, CurveTypeCode.YIELD,this.BZH);
                                frmCurveA.MdiParent = this;
                                frmCurveA.Show();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("原油打开收率曲线错误:" + ex.ToString());
                                return;
                            }
                            finally
                            {
                                this.StopWaiting();
                            }
                        }

                        #endregion
                        //}                       
                    }
                }
                else if (this.ActiveMdiChild.GetType().Name == "FrmCurveA")
                {
                    ((DatabaseA.FrmCurveA)this.ActiveMdiChild).init(CurveTypeCode.YIELD);
                }
            }
        }

        ///// <summary>
        ///// 馏分的性质曲线
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void menuCurveItem2_Click(object sender, EventArgs e)
        //{
        //    if (this.ActiveMdiChild != null)
        //    {
        //        if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
        //        {
        //            DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;     // 一条原油 
        //            if (frmOilDataA != null)
        //            {
        //                DatabaseA.FrmCurveA frmCurveA = new DatabaseA.FrmCurveA(frmOilDataA, CurveTypeCode.DISTILLATE);
        //                //frmCurveA.Text = frmOilDataA.oil.crudeIndex + "收率曲线";
        //                frmCurveA.MdiParent = this;
        //                frmCurveA.Show();
        //            }
        //        }
        //        else if (this.ActiveMdiChild.GetType().Name == "FrmCurveA")
        //        {
        //            ((DatabaseA.FrmCurveA)this.ActiveMdiChild).init(CurveTypeCode.DISTILLATE);
        //        }

        //    }
        //}

        /// <summary>
        /// 馏分的性质曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCurveItem2_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;     // 一条原油 
                    if (frmOilDataA != null)
                    {
                        FrmCurveA frm = frmCurveAIsExist();
                        if (frm != null)
                        {
                            frm.init(CurveTypeCode.DISTILLATE);
                        }
                        else
                        {
                            try
                            {
                                this.StartWaiting();
                                DatabaseA.FrmCurveA frmCurveA = new DatabaseA.FrmCurveA(frmOilDataA, CurveTypeCode.DISTILLATE,this.BZH );
                                frmCurveA.MdiParent = this;
                                frmCurveA.Show();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("原油打开收率曲线错误:" + ex.ToString());
                                return;
                            }
                            finally
                            {
                                this.StopWaiting();
                            }
                        }
                    }
                }
                else if (this.ActiveMdiChild.GetType().Name == "FrmCurveA")
                {
                    ((DatabaseA.FrmCurveA)this.ActiveMdiChild).init(CurveTypeCode.DISTILLATE);
                }

            }
        }
        ///// <summary>
        ///// 渣油性质曲线
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void menuCurveItem3_Click(object sender, EventArgs e)
        //{
        //    if (this.ActiveMdiChild != null)
        //    {
        //        if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
        //        {
        //            DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;     // 一条原油 
        //            if (frmOilDataA != null)
        //            {
        //                DatabaseA.FrmCurveA frmCurveA = new DatabaseA.FrmCurveA(frmOilDataA, CurveTypeCode.RESIDUE);

        //                if (!(frmCurveA.canOpen()))//新加提示，收率曲线不存在时，不弹出渣油作图窗口
        //                {
        //                    MessageBox.Show(this.ActiveMdiChild.Text + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                    return;
        //                }
        //                //frmCurveA.Text = frmOilDataA.oil.crudeIndex + "收率曲线";
        //                frmCurveA.MdiParent = this;
        //                frmCurveA.Show();
        //            }
        //        }
        //        else if (this.ActiveMdiChild.GetType().Name == "FrmCurveA")
        //        {
        //            bool ok = ((DatabaseA.FrmCurveA)this.ActiveMdiChild).canOpen();
        //            if (ok)
        //            {
        //                ((DatabaseA.FrmCurveA)this.ActiveMdiChild).init(CurveTypeCode.RESIDUE);
        //            }
        //            else
        //            {
        //                MessageBox.Show(this.ActiveMdiChild.Text + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 渣油性质曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCurveItem3_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;     // 一条原油 
                    if (frmOilDataA != null)
                    {
                        FrmCurveA frm = frmCurveAIsExist();
                        if (frm != null)
                        {
                            bool ok = frm.canOpen();
                            if (ok)
                            {
                                frm.init(CurveTypeCode.RESIDUE);
                            }
                            else
                            {
                                MessageBox.Show(this.ActiveMdiChild.Text + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            try
                            {
                                this.StartWaiting();
                                DatabaseA.FrmCurveA frmCurveA = new DatabaseA.FrmCurveA(frmOilDataA, CurveTypeCode.RESIDUE, this.BZH);//调用曲线界面窗口的构造函数FrmCurveA（），初始化过程ComboBox的DataSource值变化，从而激发cmbCurve_SelectedIndexChanged事件，调用处理函数，完成曲线显示和曲线下表格中A、B库数据的填充和显示

                                if (!(frmCurveA.canOpen()))//新加提示，收率曲线不存在时，不弹出渣油作图窗口
                                {
                                    MessageBox.Show(this.ActiveMdiChild.Text + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                                //frmCurveA.Text = frmOilDataA.oil.crudeIndex + "收率曲线";
                                frmCurveA.MdiParent = this;
                                frmCurveA.Show();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("原油打开收率曲线错误:" + ex.ToString());
                                return;
                            }
                            finally
                            {
                                this.StopWaiting();
                            }
                        }
                    }
                }
                else if (this.ActiveMdiChild.GetType().Name == "FrmCurveA")
                {
                    bool ok = ((DatabaseA.FrmCurveA)this.ActiveMdiChild).canOpen();
                    if (ok)
                    {
                        ((DatabaseA.FrmCurveA)this.ActiveMdiChild).init(CurveTypeCode.RESIDUE);//渣油曲线窗口初始化函数
                    }
                    else
                    {
                        MessageBox.Show(this.ActiveMdiChild.Text + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }


        /// <summary>
        /// 曲线设置选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCurveOption_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmCurveOption"))   //打开原始数据库A，只能存在一个窗口
            {
                FrmCurveOption frm = new FrmCurveOption();
                frm.MdiParent = this;
                frm.Text = "曲线设置选项";
                frm.Show();
            }
        }


        #endregion

        #region "应用"

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuApply1_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FormQuery"))   //打开查询窗口，只能存在一个查询页面
            {
                FormQuery formQuery = new FormQuery();
                formQuery.InitFormQuery();//初始化窗体
                formQuery.MdiParent = this;
                formQuery.Show();
                this.toolStripStatusLabel.Text = "工具箱数据查询";
            }
        }

        /// <summary>
        /// 计算工具箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuApply2_Click(object sender, EventArgs e)
        {
            if (!this.IsExistFrm("FormToolBox"))   //打开查询窗口，只能存在一个查询页面
            {
                FormToolBox formToolBox = new FormToolBox();
                formToolBox.Name = "FormToolBox";
                formToolBox.Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 配样计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsExistFrm("FrmPreCalculation"))   //打开查询窗口，只能存在一个查询页面
            {
                FrmPreCalculation frmPreCalculation = new FrmPreCalculation();
                frmPreCalculation.Name = "FrmPreCalculation";
                frmPreCalculation.Show();
            }
        }
        /// <summary>
        /// 小结生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 小结生成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    DatabaseA.FrmOilDataA frmOilDataA = (DatabaseA.FrmOilDataA)this.ActiveMdiChild;     // 一条原油 
                    if (frmOilDataA != null)
                    {
                        if (!this.IsExistFrm("FrmSummary"))   //打开查询窗口，只能存在一个结论生成窗体
                        {
                            FrmSummary frmSummary = new FrmSummary(frmOilDataA);
                            frmSummary.Show();
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    MessageBox.Show("请激活录入表！", "提示信息!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("请打开一条原油的录入表！", "提示信息!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        /// <summary>
        /// 水平值配置表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 水平值配置表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsExistFrm("FrmSetLevelValue"))   //打开查询窗口，只能存在一个查询页面
            {
                FrmSetLevelValue frm = new FrmSetLevelValue();
                frm.Name = "FrmSetLevelValue";
                frm.Show();
            }
        }
        /// <summary>
        /// 指标值配置表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 指标值配置表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsExistFrm("FrmSetTargetedValue"))   //打开查询窗口，只能存在一个查询页面
            {
                FrmSetTargetedValue frm = new FrmSetTargetedValue();
                frm.Name = "FrmSetTargetedValue";
                frm.Show();
            }

        }
        /// <summary>
        /// 插值算法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 插值算法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.IsExistFrm("FormSpline"))   //打开查询窗口，只能存在一个查询页面
            {
                FormSpline formSpline = new FormSpline();
                formSpline.Name = "FormSpline";
                formSpline.Show();
            }
        }

        #endregion

        #region 库管理

        #endregion

        #region 系统管理菜单

        /// <summary>
        /// 账号管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSystemUser_Click(object sender, EventArgs e)
        {
            //打开子窗口
            if (!this.IsExistChildFrm("FrmUser"))
            {
                FrmUser frm = new FrmUser();
                frm.MdiParent = this;
                frm.Show();
            }
        }
        /// <summary>
        /// 角色管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 角色管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //打开子窗口
            if (!this.IsExistChildFrm("FrmRole"))
            {
                FrmRole frm = new FrmRole();
                frm.MdiParent = this;
                frm.Show();
            }
        }


        /// <summary>
        /// 原油信息表管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSystemOilInfo_Click(object sender, EventArgs e)
        {
            //if (!this.IsExistChildFrm("FrmOilTableType"))
            //{
            //    FrmOilTableType frmLogin = new FrmOilTableType();
            //    frmLogin.MdiParent = this;
            //    frmLogin.Show();
            //}
        }

        /// <summary>
        /// 日志管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSystemLog_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 视图菜单

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = menuViewTools.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = menuViewStatus.Checked;
        }

        #endregion

        #region 窗口菜单

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        #endregion

        #region 按钮
        private void toolBtnNavigate_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                if (splitLeftVisible == true)
                {
                    ((FrmOilDataA)this.ActiveMdiChild).splitLeftOpen();
                    splitLeftVisible = false;
                }
                else
                {
                    ((FrmOilDataA)this.ActiveMdiChild).splitLeftClose();
                    splitLeftVisible = true;
                }
            }

        }
        /// <summary>
        /// 隐藏实测值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolBtnShowHideCalCol_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                if (_showHideLabCo == true)
                {
                    ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).showHideCalCol(false);
                    _showHideLabCo = false;
                }
                else
                {
                    ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).showHideCalCol(true);
                    _showHideLabCo = true;
                }
            }

        }
        /// <summary>
        /// 隐藏校正值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolBtnShowHideLabCol_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;
            if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
            {
                if (this._showHideLabCo == true)
                {
                    ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).showHideLabCol(false);
                    this._showHideLabCo = false;
                }
                else
                {
                    ((DatabaseA.FrmOilDataA)this.ActiveMdiChild).showHideLabCol(true);
                    this._showHideLabCo = true;
                }
            }
        }

        #endregion

        #region 帮助菜单

        #endregion


        #region 库管理

        //多A库合并
        private void menuLib1_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmLibAIn"))   //打开原始数据库A，只能存在一个窗口
            {
                FrmLibAIn frmLibAIn = new FrmLibAIn();
                frmLibAIn.MdiParent = this;
                if (!frmLibAIn.Visible)
                    frmLibAIn.Close();
                else
                    frmLibAIn.Show();
            }
        }

        //导出A库
        private void menuLib2_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmLibAOut"))   //打开原始数据库A，只能存在一个窗口
            {
                FrmLibAOut frmLibManageA = new FrmLibAOut();
                frmLibManageA.MdiParent = this;
                frmLibManageA.Show();
            }
        }

        //多B库合并
        private void menuLib3_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmLibBIn"))   //打开原始数据库A，只能存在一个窗口
            {
                FrmLibBIn frmLibBIn = new FrmLibBIn();
                frmLibBIn.MdiParent = this;
                if (!frmLibBIn.Visible)
                    frmLibBIn.Close();
                else
                    frmLibBIn.Show();
            }
        }

        //导出B库
        private void menuLib4_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmLibBOut"))   //打开原始数据库A，只能存在一个窗口
            {
                FrmLibBOut frmLibBOut = new FrmLibBOut();
                frmLibBOut.MdiParent = this;
                frmLibBOut.Show();
            }
        }

        //发布数据升级包
        private void menuLib5_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region 等待线程

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
        /// 批注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                if (this.ActiveMdiChild.GetType().Name == "FrmOilDataA")
                {
                    RemarkEntity remarkData = ((FrmOilDataA)this.ActiveMdiChild).addRemark();
                    if (remarkData == null)
                        return;

                    DatabaseA.FrmRemark tempfrmRemark = (DatabaseA.FrmRemark)GetChildFrm("frmRemark");
                    if (tempfrmRemark == null)
                    {
                        DatabaseA.FrmRemark frmRemark = new DatabaseA.FrmRemark(remarkData);
                        //frmRemark.TopMost = true;
                        frmRemark.StartPosition = FormStartPosition.CenterScreen;
                        frmRemark.ShowDialog();
                        if (GlobalRemark.YesNo == System.Windows.Forms.DialogResult.OK)
                        {
                            if (remarkData.LaborCal == 0)
                                remarkData.LabRemark = GlobalRemark.message;
                            else if (remarkData.LaborCal == 1)
                                remarkData.CalRemark = GlobalRemark.message;
                        }
                        frmRemark.Close();
                    }
                    else
                        tempfrmRemark.Activate();

                    int ID = OilBll.editRemark(remarkData);
                    ((FrmOilDataA)this.ActiveMdiChild).addRemarkToForm();

                }
            }
        }

        /// <summary>
        /// 下拉菜单选择窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuWindows_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem as ToolStripMenuItem;
            if (item == null || item.IsMdiWindowListEntry == false)
                return;

            foreach (Form frm in this.MdiChildren)
            {
                if (frm.Text == e.ClickedItem.Text.Substring(3))
                {
                    frm.Activate();
                    this.ActiveMdiChild.WindowState = FormWindowState.Normal;
                    break;
                }
            }
        }
        /// <summary>
        /// 实测值->校正值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is IGridOilEditor)
                (this.ActiveMdiChild as IGridOilEditor).Lab2Cal();
        }
        /// <summary>
        /// B库的定制馏分查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FormQueryA"))   //打开查询窗口，只能存在一个查询页面
            {
                FormQueryDataB formQuery = new FormQueryDataB();
                //formQuery.InitFormQuery();//初始化窗体
                //formQuery.MdiParent = this;
                formQuery.Show();
                this.toolStripStatusLabel.Text = "定制馏分查询";
            }
        }

        private void 输出曲线Menu_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FormOutputCurve"))   //打开查询窗口，只能存在一个查询页面
            {
                FormOutputCurve outputCurve = new FormOutputCurve();
                outputCurve.Name = "FormOutputCurve";
                outputCurve.Show();
            }
        }

        private void 数据输出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOutput output = new FormOutput();
            output.Show();
        }

        private void 导入ExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "原油数据文件 (*.xls)|*.xls";
            myOpenFileDialog.RestoreDirectory = true;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //if (!this.IsExistChildFrm("FrmOilDataA"))
                    //{
                    this.IsBusy = true;
                    //this.StartWaiting();
                    OilInfoEntity oil = OilBll.importExcel(myOpenFileDialog.FileName);
                    this.IsBusy = false;
                    //this.StopWaiting();
                    if (oil.ID > 0)
                    {
                        DatabaseA.FrmOilDataA frmOilDataA = new DatabaseA.FrmOilDataA(oil);
                        frmOilDataA.changeSave();//让窗体中的表格处于编辑状态
                        frmOilDataA.MdiParent = this;
                        frmOilDataA.Text = "原始库原油数据： " + myOpenFileDialog.FileName;
                        frmOilDataA.Show();

                        DatabaseA.FrmOpenA frmOpenA = (DatabaseA.FrmOpenA)GetChildFrm("frmOpenA");
                        if (frmOpenA != null)  //如果打开原油库A的窗口存在，则更新
                            frmOpenA.refreshGridList(false);
                    }
                    else
                    {
                        MessageBox.Show("原油编号为空或已经存在!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导入的文件格式不正确!", "导入失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error("文件-导入EXCEL文件" + ex);
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }

    }
}
