using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil.V2;
namespace RIPP.App.OilDataApp.Forms
{
    public partial class CopyFrmMain : Form  
    {
        #region 私有变量
        private List<CutOilRateEntity> _cutOilRates = new List<CutOilRateEntity>();
        private List<CutMothedEntity> _cutMotheds = new List<CutMothedEntity>();       
        private OilInfoBEntity _oil = new OilInfoBEntity();     //一条原油
        private OilApplyBll oilApplyBll = new OilApplyBll();
        #endregion

        #region 属性

        /// <summary>
        /// 原油ID和混合比例  
        /// </summary>
        public List<CutOilRateEntity> CutOilRates
        {
            set { this._cutOilRates = value; }
            get { return this._cutOilRates; }
        }

        /// <summary>
        /// 切割方案 
        /// </summary>
        public List<CutMothedEntity> CutMotheds
        {
            set { this._cutMotheds = value; }
            get { return this._cutMotheds; }
        }

        /// <summary>
        /// 切割计算结果
        /// </summary>
        public OilInfoBEntity Oil
        {
            set { this._oil = value; }
            get { return this._oil; }
        }
        

        #endregion

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

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public CopyFrmMain()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized; //窗口最大化
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = true;
            this.button5.Enabled = false;
        }

        #endregion

        #region 私有函数      

        #endregion

        #region 公有函数

        /// <summary>
        /// 是否存在该类型的子窗体
        /// </summary>
        /// <param name="childFrmType">窗体类型</param>
        /// <returns>存在返回1并激活此窗口，不存在返回0</returns>
        public bool IsExistChildFrm(string childFrmType)
        {
            bool flag = false;
            foreach (Control frm in this.splitContainer1.Panel2.Controls)
            {
                if (frm.GetType().Name == childFrmType)
                {
                    frm.BringToFront();
                    flag = true;
                    break;
                }
            }
            return flag;
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

        #region 菜单

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
             
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

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

        #endregion    

        #region

        /// <summary>
        /// 选择原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this._cutMotheds.Clear();
            this._cutOilRates.Clear();
            this._oil.OilDatas.Clear();

            //this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;

            if (!this.IsExistChildFrm("FrmStep1"))  //打开原始数据库B，只能存在一个窗口
            {
                FrmStep1 frmStep1 = new FrmStep1(this);
                frmStep1.Name = "FrmStep1";
                frmStep1.Text = "选择原油";
                frmStep1.MdiParent = this;
                this.splitContainer1.Panel2.Controls.Add(frmStep1);
                frmStep1.Show();
                this.toolStripStatusLabel.Text = "选择原油";
                this.button2.Enabled = true;
            }         
        }

        /// <summary>
        /// 混合方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmStep2"))  //打开原始数据库B，只能存在一个窗口
            {
                FrmStep2 frmStep2 = new FrmStep2(this);
                frmStep2.Name = "FrmStep2";
                frmStep2.Text = "混合方案";
                frmStep2.MdiParent = this;
                this.splitContainer1.Panel2.Controls.Add(frmStep2);
                frmStep2.BringToFront();
                frmStep2.Show();
                this.toolStripStatusLabel.Text = "混合方案";
                this.button3.Enabled = true;
            }
        }

        /// <summary>
        /// 设置切割方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmStep2"))  //打开原始数据库B，只能存在一个窗口
            {
                //this._cutOilRates.Add(cutRate);
                //this._cutOilRates[0].rate = 100;
                FrmStep3 frmStep3 = new FrmStep3(this);
                frmStep3.Name = "FrmStep3";
                frmStep3.Text = "切割方案";
                frmStep3.MdiParent = this;
                this.splitContainer1.Panel2.Controls.Add(frmStep3);
                frmStep3.BringToFront();
                this.toolStripStatusLabel.Text = "切割方案";
                frmStep3.Show();
                this.button4.Enabled = true;
            }
        }

        /// <summary>
        /// 计算并输出计算结果到界面表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (this._cutMotheds.Count <= 0 || this._cutOilRates.Count <= 0)
                return;

            this.StartWaiting();          
            this._oil = oilApplyBll.GetCutResult(this._cutOilRates, this._cutMotheds);
            ShowCutData newForm = new ShowCutData(this._oil, this._cutMotheds);
            newForm.Show();
            #region 
            //OilPropertyAPIEntity temp = new OilPropertyAPIEntity()
            //{
            //    D20 = 0.8612f,
            //    CCR = 5.52f,
            //    N2 = 1300f,
            //    WAX = 3f,
            //    SUL = 1.8f,
            //    TWY140 = 15.44981f,
            //    TWY180 = 22.57231f,
            //    TWY240 = 32.58333f,
            //    TWY350 = 51.37775f
            //};
            //OilApplyAPIBll oilApplyAPIBll = new OilApplyAPIBll();

            //CutMothedAPIEntity a = new CutMothedAPIEntity()
            //{
            //    ICP = 15,
            //    ECP = 180,
            //    Name = CutTableName.ShiNaoYou
            //};
            //CutMothedAPIEntity b = new CutMothedAPIEntity()
            //{
            //    ICP = 140,
            //    ECP = 240,
            //    Name = CutTableName.MeiYou
            //};
            //CutMothedAPIEntity c = new CutMothedAPIEntity()
            //{
            //    ICP = 180,
            //    ECP = 350,
            //    Name = CutTableName.ChaiYou
            //};
            //CutMothedAPIEntity d = new CutMothedAPIEntity()
            //{
            //    ICP = 350,
            //    ECP = 540,
            //    Name = CutTableName.LaYou
            //};
            ////CutMothedAPIEntity f = new CutMothedAPIEntity()
            ////{
            ////    ICP = 350,
            ////    ECP = 1600,
            ////    Name = CutTableName.YuanYouXingZhi
            ////};
            //CutMothedAPIEntity g = new CutMothedAPIEntity()
            //{
            //    ICP = 540,
            //    ECP = 1600,
            //    Name = CutTableName.ZhaYou
            //};
            //List<CutMothedAPIEntity> l = new List<CutMothedAPIEntity>();
            //l.Add(a);
            //l.Add(b);
            //l.Add(c);
            //l.Add(d);
            ////l.Add(f);
            //l.Add(g);
                      
            //List<CutMothedEntity> cutMothedList = new List<CutMothedEntity>();
            //for (int index = 0; index < l.Count; index++)
            //{
            //    CutMothedEntity cutMothed = new CutMothedEntity();
            //    cutMothed.ICP = l[index].ICP;
            //    cutMothed.ECP = l[index].ECP;
            //    cutMothed.Name = l[index].Name.GetDescription();
            //    cutMothedList.Add(cutMothed);
            //}
            //List<string> crudeIndexList = new List<string>() { "RIPP0337", "RIPP0044", "RIPP0277", "RIPP0113", "RIPP0206", "RIPP0405" };
            //List<float> cutRateList = new List<float>() { 81.56557f, 11.75237f, 2.040833f, 2.034374f, 1.848026f, 0.7588176f };
            //List<CutOilRateEntity> rateList = new List<CutOilRateEntity>();
            //for (int i = 0; i < crudeIndexList.Count; i++)
            //{
            //    CutOilRateEntity cutRateEntity = new CutOilRateEntity();
            //    cutRateEntity.crudeIndex = crudeIndexList[i];
            //    cutRateEntity.rate = cutRateList[i];
            //    rateList.Add(cutRateEntity);
            //}
 
            //this._oil = oilApplyAPIBll.GetCutResultAPI("RIPP0305 ", l);
            ////this._oil = oilApplyAPIBll.GetCutResultAPI(rateList, l);
            ////this._oil = oilApplyAPIBll.GetCutResultAPI(temp, l);
            ////ShowCutData newForm = new ShowCutData(this._oil, this._cutMotheds);
            //ShowCutData newForm = new ShowCutData(this._oil, cutMothedList);
            //newForm.Show();
            #endregion
            this.toolStripStatusLabel.Text = "切割计算";
            this.StopWaiting();
            this.button5.Enabled = true;
        }

        /// <summary>
        /// 计算结果输出为Excel
        /// </summary>   
        private void button5_Click(object sender, EventArgs e)
        {
            this.toolStripStatusLabel.Text = "导出EXCEL";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择输出模版";
            openFileDialog.Filter = "原油数据模版文件 (*.xls)|*.xls";//Excel2010与2003不兼容
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int outResult = ExcelTool.outDataToExcel(null, this._oil, openFileDialog.FileName);
                if (outResult == 1)
                {
                    MessageBox.Show("数据导出成功!","提示");
                }
                else if (outResult == -1)
                {
                    MessageBox.Show("当前系统尚未安装EXCEL软件!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -2)
                {
                    MessageBox.Show("不能打开Excel进程!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (outResult == -3)
                {
                    MessageBox.Show("数据导出失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if(outResult==-11)
                {
                    return;
                }
            }
        }

        #endregion

        #region "文件"

        /// <summary>
        /// 重新开始
        /// </summary>    
        private void menuItemStart_Click(object sender, EventArgs e)
        {
            this._cutMotheds.Clear();
            this._cutOilRates.Clear();
            this._oil.OilDatas.Clear();

            //this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
            this.button5.Enabled = false;
        }

        #endregion

        /// <summary>
        /// 多B库合并
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuLibBMerge_Click(object sender, EventArgs e)
        {
            if (!this.IsExistChildFrm("FrmLibBIn"))   //导入B库，只能存在一个窗口
            {
                FrmLibBIn frmLibBIn = new FrmLibBIn();
                //frmLibBIn.InputMainForm(this);
                frmLibBIn.Name = "FrmLibBIn";
                if (frmLibBIn.Visible)
                {
                    frmLibBIn.MdiParent = this;
                    frmLibBIn.Show();
                    this.splitContainer1.Panel2.Controls.Add(frmLibBIn);
                }
                else
                    frmLibBIn.Close();
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
    }
}
