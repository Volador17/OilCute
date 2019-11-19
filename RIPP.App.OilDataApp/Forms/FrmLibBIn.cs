using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.Lib;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.DataCheck;
using RIPP.OilDB.UI.GridOil;
namespace RIPP.App.OilDataApp.Forms
{
    public partial class FrmLibBIn : Form
    {
        #region "私有变量"

        private OutLib _outLib = null;//导入的库文件
        private Form _MainForm = null;//主窗体
        /// <summary>
        /// 日期格式
        /// </summary>
        private const string dateFormat = "yyyy-MM-dd";
        /// <summary>
        /// 日期格式
        /// </summary>
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        private OilDataCheck oilDataCheck = new OilDataCheck();
        private DgvHeader dgvHeader = new DgvHeader();
        #endregion 


        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmLibBIn()
        {
            InitializeComponent();
            this.Text = "导入应用库";
           
        }
        
        public void Init(OutLib outLib)
        {
            this._outLib = outLib;
           // init();
            GridListBind();
        }
        /// <summary>
        /// 主窗体的传递
        /// </summary>
        /// <param name="MainForm"></param>
        public void InputMainForm(Form MainForm)
        {
            this._MainForm = MainForm;        
        }
        /// <summary>
        /// 初始化导入数据
        /// </summary>
        private void init()
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "原油数据文件 (*.libB)|*.libB";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _outLib = Serialize.Read<OutLib>(saveFileDialog.FileName);
                this.Visible = true;
            }
            else
                this.Visible = false;
        }

        /// <summary>
        /// 表格控件绑定
        /// </summary>
        private void GridListBind()
        {
            dgvHeader.SetMangerDataBaseBColHeader(this.dgvLibBIn, false, true);
            this.dgvLibBIn.Rows.Clear();
            if (_outLib == null)
                return;
            IList<OilInfoOut> oilInfo = _outLib.oilInfoOuts;

            //绑定数据
            for (int i = 0; i < oilInfo.Count; i++)
            {
                string receiveDate = oilInfo[i].receiveDate == null ? string.Empty : oilInfo[i].receiveDate.Value.ToString(dateFormat);
                string updataDate = string.Empty;
                if (oilInfo[i].updataDate != string.Empty)
                {
                    var updataDateTime = oilDataCheck.GetDate(oilInfo[i].updataDate);
                    updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                }
                this.dgvLibBIn.Rows.Add
                   (false, i, oilInfo[i].ID, 0,
                   oilInfo[i].crudeName,
                   oilInfo[i].englishName,
                   oilInfo[i].crudeIndex,
                   oilInfo[i].country,
                   oilInfo[i].region,
                   receiveDate, updataDate,
                   oilInfo[i].sourceRef,
                   oilInfo[i].type,
                   oilInfo[i].classification,
                   oilInfo[i].sulfurLevel,
                   oilInfo[i].acidLevel
                   );
               }
        }
        /// <summary>
        /// B库数据导入功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripBtnIn_Click(object sender, EventArgs e)
        {
            this.dgvLibBIn.EndEdit();
            OilInfoBAccess oilInfoBccess = new OilInfoBAccess();//从B库进行原油查找
            LibManageBll libManageBll = new LibManageBll();//导入B库管理
            string alert = "未导入的原油：";
            foreach (DataGridViewRow row in this.dgvLibBIn.Rows)
            {
                if (bool.Parse(row.Cells["select"].Value.ToString()) == true)
                {
                    int oilInfoId = int.Parse(row.Cells["ID"].Value.ToString());
                    OilInfoOut oilInfoOut = this._outLib.oilInfoOuts.Where(c => c.ID == oilInfoId).FirstOrDefault();//从库文件中获取数据

                    OilInfoBEntity oilInfoBEntity = new OilInfoBEntity();
                    libManageBll.toOilInfoEntity(ref  oilInfoBEntity, oilInfoOut);  //转换为OilInfoBEntity
                    oilInfoBEntity.ID = OilBll.saveInfo(oilInfoBEntity);//将数据保存到原油应用模块

                    if (oilInfoBEntity.ID == -1)//原油已存在，提示是否更新数据
                    {
                        DialogResult r = MessageBox.Show(oilInfoBEntity.crudeIndex + "原油已存在！是否要更新", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (r == DialogResult.Yes)
                        {
                            oilInfoBccess.Delete("crudeIndex='" + oilInfoBEntity.crudeIndex + "'");  //删除原油信息数据
                            oilInfoBEntity.ID = OilBll.save(oilInfoBEntity);    //重新插入原油信息

                            libManageBll.toOilDatas(ref oilInfoBEntity, oilInfoOut, this._outLib.oilTableRows, this._outLib.oilTableCols);
                            OilBll.saveTables(oilInfoBEntity);

                            libManageBll.toOilDataSearchs(ref oilInfoBEntity, oilInfoOut);
                            OilBll.saveSearchTable(oilInfoBEntity);

                            libManageBll.toCurve(ref oilInfoBEntity, oilInfoOut.curves, _outLib.curveTypes);
                            OilBll.saveCurves(oilInfoBEntity);


                            if (this.ActiveMdiChild != null)
                            {
                                if (this.ActiveMdiChild.GetType().Name == "FrmMain")//如果打开窗体存在.
                                {
                                    FrmMain frmMain = (FrmMain)this.ActiveMdiChild;
                                    if (frmMain != null)  //如果打开原油库A的窗口存在，则更新
                                    {
                                        frmMain.refreshGridList();
                                    }
                                }
                            }
                            MessageBox.Show(oilInfoBEntity.crudeName + "原油导入成功！");
                        }
                        else
                        {
                            alert += oilInfoBEntity.crudeIndex + "  ";
                            MessageBox.Show(oilInfoBEntity.crudeName + "原油导入不成功！");
                        }
                    }
                    else//原油不存在 
                    {
                        try
                        {
                            libManageBll.toOilDatas(ref oilInfoBEntity, oilInfoOut, this._outLib.oilTableRows, this._outLib.oilTableCols);
                            OilBll.saveTables(oilInfoBEntity);

                            libManageBll.toOilDataSearchs(ref oilInfoBEntity, oilInfoOut);
                            OilBll.saveSearchTable(oilInfoBEntity);

                            libManageBll.toCurve(ref oilInfoBEntity, oilInfoOut.curves, _outLib.curveTypes);
                            OilBll.saveCurves(oilInfoBEntity);


                            if (this.ActiveMdiChild != null)
                            {
                                if (this.ActiveMdiChild.GetType().Name == "FrmMain")//如果打开窗体存在.
                                {
                                    FrmMain frmMain = (FrmMain)this.ActiveMdiChild;
                                    if (frmMain != null)  //如果打开原油库A的窗口存在，则更新
                                    {
                                        frmMain.refreshGridList();
                                    }                   
                                }
                            }
                            MessageBox.Show(oilInfoBEntity.crudeName + "原油导入成功！");
                        }
                        catch (Exception ex)
                        {
                            Log.Error("原油导入错误！" + ex.ToString());
                            MessageBox.Show(oilInfoBEntity.crudeName + "原油导入不成功！");
                            return;
                        }                       
                    }

                }
            }
        }
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripCeckBox1_Click(object sender, EventArgs e)
        {
            if (!this.toolStripCeckBox1.Checked)
            {
                for (int i = 0; i < this.dgvLibBIn.Rows.Count; i++)//
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)this.dgvLibBIn.Rows[i].Cells["select"];
                    checkCell.Value = false;
                }
            }
            else
            {
                for (int i = 0; i < this.dgvLibBIn.Rows.Count; i++)//
                {
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)this.dgvLibBIn.Rows[i].Cells["select"];
                    checkCell.Value = true;
                }
            }
            this.dgvLibBIn.EndEdit();
            this.dgvLibBIn.Refresh();
        }
        /// <summary>
        /// 添加行头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLibBIn_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {

        }

    }
}