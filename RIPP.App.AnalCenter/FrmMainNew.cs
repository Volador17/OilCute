using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.NIR.Models;
using RIPP.NIR;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RIPP.App.AnalCenter.Busi;
using ZedGraph;
using log4net;
using System.Text.RegularExpressions;

namespace RIPP.App.AnalCenter
{
    public partial class FrmMainNew : Form
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Busi.NIRMoniter _moniter;
        private Busi.Config _config;
        private BindModel _predictor;
        private S_User _user;
        private int _pageCount = 20;
        private SpecCache _specCache;

       // private int _workerCount = 0;

        public FrmMainNew(S_User u)
        {
            this._user = u;
            InitializeComponent();

            this.Load += new EventHandler(FrmMainNew_Load);
            this.propertyGrid1.SelectionChanged += new EventHandler(propertyGrid1_SelectionChanged);
            this.pager1.PageChange += new EventHandler(pager1_PageChange);

            this.propertyGrid1.RowHeaderMouseClick += new DataGridViewCellMouseEventHandler(propertyGrid1_RowHeaderMouseClick);
            this.propertyGrid1.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(propertyGrid1_ColumnHeaderMouseClick);

            this.FormClosing += new FormClosingEventHandler(FrmMainNew_FormClosing);
            
           
        }

        void FrmMainNew_FormClosing(object sender, FormClosingEventArgs e)
        {
             //提示是否退出
            if (MessageBox.Show("是否退出本软件！", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                e.Cancel = true;
        }
        #region 初始化事件

        void propertyGrid1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.propertyGrid1.SelectionMode != DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                this.propertyGrid1.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                this.propertyGrid1.Columns[e.ColumnIndex].Selected = true;
                this.propertyGrid1.Refresh();
            }
        }

        void propertyGrid1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.propertyGrid1.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
            {
                this.propertyGrid1.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                this.propertyGrid1.Rows[e.RowIndex].Selected = true;
                this.propertyGrid1.Refresh();
            }
        }

        void pager1_PageChange(object sender, EventArgs e)
        {
            this.pager1.Enabled = false;
            this.search();
        }

        

       

        void FrmMainNew_Load(object sender, EventArgs e)
        {
           

            this._config = new Busi.Config();
            this._specCache = new SpecCache(this._config.MaxSpecNum);
            this._moniter = new Busi.NIRMoniter(this._config.FolderSpec);
            this._moniter.OnFoundSpecs += new EventHandler<NIRMonitorHandler>(_moniter_OnFoundSpecs);




            this.btnUserinfo.Text = string.Format("欢迎 {0} !", this._user.realName);
            this.btnUserinfo.ToolTipText = "点击修改个人资料";
            try
            {
                this._predictor = BindModel.LoadModel(this._config.ModelDefaultPath);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                if (MessageBox.Show("加载方法包出错，是否重新配置方法？", "加载方法包出错", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.menuMethods.PerformClick();
                }
            }
            

            if (this._config.IsAutoEstimate)
                this._moniter.Start();
            if (this._predictor != null)
            {
                this.btnMethodName.Text = this._predictor.Name;
            }
            this.lblAutoRun.Text = this._config.IsAutoEstimate ? "自动" : "手动";
            

            //权限
            if (this._user.Role != RoleEnum.Operater)
            {
                this.menuManger.Enabled = true;
                this.menuSetting.Enabled = true;
                this.lblAutoRun.Enabled = true;
                this.menuPropertyConfig.Visible = this._user.Role == RoleEnum.RIPP;
            }

            this.initGrid();
            this.initChart();
            this.initSearchPanel();
        }

        private void initSearchPanel()
        {
            //txbSPredictorName
            this.txbSPredictorName.Items.Add("全部");
            this.txbSPredictorName.Items.Add(PredictMethod.Identify.GetDescription());
            this.txbSPredictorName.Items.Add(PredictMethod.Fitting.GetDescription());
            this.txbSPredictorName.Items.Add(PredictMethod.Integrate.GetDescription());
            this.txbSPredictorName.Items.Add(PredictMethod.PLSBind.GetDescription());
            this.txbSPredictorName.Items.Add(PredictMethod.None.GetDescription());
            this.txbSPredictorName.SelectedIndex = 0;

            //txbSPredictorName

            using (var db = new NIRCeneterEntities())
            {
                var lst = db.S_User.ToList();
                this.txbSUserName.Items.Add("全部");
                this.txbSUserName.SelectedIndex = 0;
                for (int i = 0; i < lst.Count; i++)
                {
                    this.txbSUserName.Items.Add(string.Format("{0}({1})", lst[i].realName, lst[i].loginName));
                    if (this._user.loginName == lst[i].loginName)
                        this.txbSUserName.SelectedIndex = i + 1;
                }
                this.txbSUserName.Enabled = this._user.Role!= RoleEnum.Operater;
            }
        }

        void propertyGrid1_SelectionChanged(object sender, EventArgs e)
        {
            var cols = new List<MyDataCol>();
            if (this.propertyGrid1.SelectedCells.Count > 0)
            {
                showdetail(this.propertyGrid1.Columns[this.propertyGrid1.SelectedCells[0].ColumnIndex]);
                if (this.propertyGrid1.SelectedColumns.Count == 0)
                {
                    var col = this.propertyGrid1.Columns[this.propertyGrid1.SelectedCells[0].ColumnIndex] as MyDataCol;
                    if (col != null)
                    {
                        cols.Add(col);
                        this.drawSelectLines(cols);
                    }
                }
            }
            if (this.propertyGrid1.SelectedColumns.Count > 0)
            {
                cols.Clear();
                for (int i = 0; i < this.propertyGrid1.SelectedColumns.Count; i++)
                {
                    var col = this.propertyGrid1.SelectedColumns[i] as MyDataCol;
                    if (col != null)
                        cols.Add(col);
                }
                this.drawSelectLines(cols);
            }
        }

        void _moniter_OnFoundSpecs(object sender, NIRMonitorHandler e)
        {
            this.addSpectrum(e.NewFile.Select(d => d.FullName).ToArray());
        }
        #endregion

        private void showdetail(DataGridViewColumn col)
        {
            var dcol = col as MyDataCol;
            if (dcol == null)
                return;


            #region panel
            if (this.lblSpecName.InvokeRequired)
            {
                ThreadStart s = () => { this.lblSpecName.Text = dcol.Spec.Spec.Name; };
                this.lblSpecName.Invoke(s);
            }
            else
                this.lblSpecName.Text = dcol.Spec.Spec.Name;

            if (this.lblPredictMethod.InvokeRequired)
            {
                ThreadStart s = () => { this.lblPredictMethod.Text = dcol.Spec.Result != null ? dcol.Spec.ResultObj.MethodType.GetDescription() : "正在预测"; };
                this.lblPredictMethod.Invoke(s);
            }
            else
                this.lblPredictMethod.Text = dcol.Spec.Result != null ? dcol.Spec.ResultObj.MethodType.GetDescription() : "正在预测";

            if (this.txbOilName.InvokeRequired)
            {
                ThreadStart s = () => { this.txbOilName.Text = dcol.Spec.OilName; };
                this.txbOilName.Invoke(s);
            }
            else
                this.txbOilName.Text = dcol.Spec.OilName;

            if (this.txbSamplePlace.InvokeRequired)
            {
                ThreadStart s = () => { this.txbSamplePlace.Text = dcol.Spec.SamplePlace; };
                this.txbSamplePlace.Invoke(s);
            }
            else
                this.txbSamplePlace.Text = dcol.Spec.SamplePlace;

            if (this.txbSampleTime.InvokeRequired)
            {
                ThreadStart s = () => { this.txbSampleTime.Value = dcol.Spec.SampleTime.HasValue ? dcol.Spec.SampleTime.Value : DateTime.Now; };
                this.txbSampleTime.Invoke(s);
            }
            else
                this.txbSampleTime.Value = dcol.Spec.SampleTime.HasValue ? dcol.Spec.SampleTime.Value : DateTime.Now;

            if (this.txbAnalyTime.InvokeRequired)
            {
                ThreadStart s = () => { this.txbAnalyTime.Value = dcol.Spec.AddTime; };
                this.txbAnalyTime.Invoke(s);
            }
            else
                this.txbAnalyTime.Value = dcol.Spec.AddTime;

            if (this.btnGetDetail.InvokeRequired)
            {
                ThreadStart s = () => { this.btnGetDetail.Enabled = dcol.Spec.ResultObj!=null; };
                this.btnGetDetail.Invoke(s);
            }
            else
                this.btnGetDetail.Enabled = dcol.Spec.ResultObj != null;

            #endregion

            if (dcol.Spec != null && dcol.Spec.ResultObj != null)
            {
                
                  if (this.resultDetail1.InvokeRequired)
                  {
                      ThreadStart s = () => { this.resultDetail1.ShowGrid(dcol.Spec.ResultObj, this._config.NumOfReuslt,dcol.Spec.Spec,this._config.NumOfId); };
                      this.resultDetail1.Invoke(s);
                  }
                  else
                      this.resultDetail1.ShowGrid(dcol.Spec.ResultObj, this._config.NumOfReuslt, dcol.Spec.Spec,this._config.NumOfId);
                

            }
           
        }

       

        private void drawSelectLines(IList<MyDataCol> cols)
        {
            if (cols.Count == 0)
                return;
            List<Spectrum> slst = new List<Spectrum>();
            List<Spectrum> lslst = new List<Spectrum>();
            List<double[]> xlst = new List<double[]>();
            List<double[]> ylst = new List<double[]>();

            foreach (var col in cols)
            {
                if (col.Spec != null && col.Spec.Spec != null)
                {
                    slst.Add(col.Spec.Spec);
                }
                if (col.Spec.Spec.Components != null)
                {
                    double[] x, y;
                    this.getLinedata(col.Spec.Spec.Components, out x, out y);
                    lslst.Add(col.Spec.Spec);
                    xlst.Add(x);
                    ylst.Add(y);
                }
            }

            if (this.specGraph1.InvokeRequired)
            {
                ThreadStart s = () => { this.specGraph1.DrawSpec(slst); };
                this.specGraph1.Invoke(s);
            }
            else
                this.specGraph1.DrawSpec(slst);

            //if (this.zedGraphControl1.InvokeRequired)
            //{
            //    ThreadStart s = () => { this.drawline(lslst, xlst, ylst); };
            //    this.zedGraphControl1.Invoke(s);
            //}
            //else
            //    this.drawline(lslst, xlst, ylst);
        }


       

        private void getLinedata(ComponentList clst,  out double[] x, out double[] y)
        {

            List<KeyValuePair<int, double>> ds = new List<KeyValuePair<int, double>>();
            foreach (var c in clst)
            {
                int dd;
                if (int.TryParse(c.Name.ToUpper().Replace("TBP", ""), out dd))
                    ds.Add(new KeyValuePair<int, double>(dd, c.PredictedValue));
                else if (c.Name.Contains("收率"))
                {
                     var reg = new Regex(@"-\d+", RegexOptions.Singleline);
                     var m = reg.Match(c.Name);
                     if (m.Success)
                     {
                         ds.Add(new KeyValuePair<int, double>(Convert.ToInt32( m.Value.Replace("-", "")), c.PredictedValue));
                     }
                }
            }

            var xl = new List<double>();
            var yl = new List<double>();
            ds = ds.OrderBy(d => d.Key).ToList();
            if (clst.Where(d => d.Name.Contains("收率")).Count() > 0)
            {
                double sum = 0;
                foreach (var p in ds)
                {
                    xl.Add(p.Key);
                    yl.Add( p.Value);
                    //sum += p.Value;
                }
            }
            else
            {
                xl = ds.Select(d =>(double) d.Key).ToList();
                yl = ds.Select(d => d.Value).ToList();
            }
            x = xl.ToArray();
            y = yl.ToArray();
        }



        private void drawline(List<Spectrum> s,List<double[]> xlst,List< double[]> ylst)
        {
            //this.zedGraphControl1.GraphPane.CurveList.Clear();
            //for (int i = 0; i < s.Count; i++)
            //    this.zedGraphControl1.GraphPane.AddCurve(s[i].Name, xlst[i], ylst[i], s[i].Color, SymbolType.Triangle);
            //this.zedGraphControl1.AxisChange();
            //this.zedGraphControl1.Refresh();
        }

        private void initChart()
        {
            //GraphPane myPane = this.zedGraphControl1.GraphPane;
            ////字体
            //myPane.Border.Width = 0;
            //myPane.Border.Color = Color.White;
            //myPane.Title.FontSpec.IsBold = false;
            //myPane.Title.FontSpec.Size = 14;
            //myPane.XAxis.Title.FontSpec.IsBold = false;
            //myPane.YAxis.Title.FontSpec.IsBold = false;

            //myPane.XAxis.Scale.MaxGrace = 0;
            //myPane.XAxis.Scale.MinGrace = 0;
            //myPane.Legend.IsVisible = false;
            //myPane.Title.Text = "蒸馏曲线";
            //myPane.XAxis.Title.Text = "温度（℃）";
            //myPane.XAxis.Title.FontSpec.Size = 14;
            //myPane.YAxis.Title.Text = "累计收率";
            //myPane.YAxis.Title.FontSpec.Size = 14;
        }

        private void initGrid()
        {
            this.propertyGrid1.Columns.Clear();

            for (int i = 0; i < this._pageCount; i++)
            {
                this.propertyGrid1.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Width = 60
                });
            }

            string[] slst = new string[] { "光谱名", "预测方法", "预测时间", "原油名称", "采样点", "采样时间" };
            foreach (var s in slst)
            {
                var row = new DataGridViewRow() { Frozen = true };
                row.HeaderCell.Value = s;
                this.propertyGrid1.Rows.Add(row);
            }


            if (this._predictor != null)
            {
                var configlst = this._config.Properties.Where(d => d.Table == PropertyType.NIR).FirstOrDefault().Datas;

                var clst = this._predictor.GetComponents();
                foreach (var c in clst)
                {
                    bool tag = true;
                    if (this._user.Role != RoleEnum.RIPP)
                    {
                        tag = configlst.Where(d => d.Name == c.Name && (d.ShowEngineer && d.ShowRIPP)).Count() > 0;
                    }
                    if (tag)
                    {
                        var r = new DataGridViewRow();
                        r.Tag = c.Name;
                        r.HeaderCell.Value = c.Name;
                        this.propertyGrid1.Rows.Add(r);
                    }
                }
            }

        }

        private void updateColumn(Specs s)
        {
            if (s == null || s.Spec == null || s.ID < 1)
                return;

            if (this.propertyGrid1.Columns.Count < 1)
                this.initGrid();

            lock (this.propertyGrid1)
            {
                //先找是否有，如有，则更新内容
                MyDataCol col = null;
                for (int i = 0; i < this.propertyGrid1.Columns.Count; i++)
                {
                    var tc = this.propertyGrid1.Columns[i] as MyDataCol;
                    if (tc != null && tc.DbIndex == s.ID)
                    {
                        col = tc;
                        break;
                    }
                }

                if (col == null)
                {
                    col = new MyDataCol()
                        {
                            HeaderText = s.ID.ToString("D5"),
                            ReadOnly = true,
                            SortMode = DataGridViewColumnSortMode.NotSortable,
                            ToolTipText = s.Spec.Name,
                            Spec = s,
                            DbIndex = s.ID,
                            Width = 60,
                            Resizable= DataGridViewTriState.True,
                            MinimumWidth = 50
                        };
                    this.propertyGrid1.Columns.Insert(0, col);
                    if (this.propertyGrid1.Columns.Count > this._pageCount)
                        this.propertyGrid1.Columns.RemoveAt(this.propertyGrid1.Columns.Count - 1);

                }
                else
                {
                    col.Spec = s;
                }




                //添加前面几行信息
                this.propertyGrid1[col.Index, 0].Value = s.Spec.Name;
                if (s.ResultObj != null)
                {
                    var methodStr= s.ResultObj.MethodType.GetDescription();
                    this.propertyGrid1[col.Index, 1].Value = s.PredictByA ?
                        string.Format("{0}{1}", methodStr, "*") : methodStr;
                    this.propertyGrid1[col.Index, 2].Value = s.AddTime.ToString("MM-dd HH:mm");
                }
                this.propertyGrid1[col.Index, 3].Value = s.OilName;
                this.propertyGrid1[col.Index, 4].Value = s.SamplePlace;
                this.propertyGrid1[col.Index, 5].Value = s.SampleTime.HasValue ? s.SampleTime.Value.ToString("MM-dd HH:mm") : "";

                if (s.Spec.Components != null)
                {
                    foreach (var c in s.Spec.Components)
                    {
                        int idx = this.findRowIndex(c.Name);
                        if (idx < 0)
                        {
                            //var row = new DataGridViewRow();
                            //row.Tag = c.Name;
                            //row.CreateCells(this.propertyGrid1, c.Name);
                            //this.propertyGrid1.Rows.Add(row);
                            //idx = row.Index;
                            continue;
                        }
                        this.propertyGrid1[col.Index, idx].Value =
                               double.IsNaN(c.PredictedValue) ? "NaN" : c.PredictedValue.ToString(string.Format("F{0}", c.Eps));
                        if (c.State != ComponentStatu.Pass)
                            this.propertyGrid1[col.Index, idx].Style = c.Style;
                    }
                }

                //this.propertyGrid1.ClearSelection();
                //col.Selected = true;
            }
           

        }

        private void addSpectrum(string[] filePathes)
        {
            var lst = new List<Specs>();
            foreach (var f in filePathes)
            {
                var spec = new Spectrum(f);
                if (spec == null)
                    continue;
                //添加一行
                var item = new Specs()
                {
                    AddTime = DateTime.Now,
                    MethodID = this._config.ModelDefaultID,
                    UserID = this._user.ID
                };
                item = this.findOilName(item, spec.Name);
                item.Spec = spec;

                //先保存数据库
                using (var db1 = new NIRCeneterEntities())
                {
                    try
                    {
                        db1.Specs.AddObject(item);
                        db1.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                    }
                }
                if (this.propertyGrid1.InvokeRequired)
                {
                    ThreadStart s = () => { this.updateColumn(item); };
                    this.propertyGrid1.Invoke(s);
                }
                else
                {
                    this.updateColumn(item);
                }

                lst.Add(item);
            }

            Action start = () =>
            {
                this.predict(lst);
                if (this.toolStrip1.InvokeRequired)
                {
                    ThreadStart s = () => { this.btnSpecOpen.Enabled = true; };
                    this.toolStrip1.Invoke(s);
                }
                else
                    this.btnSpecOpen.Enabled = true;
                if (this.statusStrip1.InvokeRequired)
                {
                    ThreadStart s = () =>
                        {
                            this.lblBusy.Text = "";
                            this.toolProgressbar.Visible = false;
                        };
                    this.statusStrip1.Invoke(s);
                }
                else
                {
                    this.lblBusy.Text = "";
                    this.toolProgressbar.Visible = false;
                }
            };

            start.BeginInvoke(null, null);
        }

        private void predict(List<Specs> lst)
        {
            try
            {
               // foreach (var item in lst)
               for(int i=0;i<lst.Count;i++)
                {
                    var item = lst[i];
                    if (this._predictor != null)
                    {
                        if (this._config.MaxSpecNum<1||  !this._specCache.Predict(ref item))
                        {
                            item.ResultObj = this._predictor.Predict(item.Spec, true, this._config.NumOfId,this._config.TopK);
                            item.ResultType = (int)item.ResultObj.MethodType;
                            item.Spec.Components = item.ResultObj.GetPredictComp(this._config.NumOfReuslt);
                            this._specCache.AddSpec(item);
                            item.PredictByA = false;
                        }

                        //保存数据
                        using (var db = new NIRCeneterEntities())
                        {
                            var item2 = db.Specs.Where(d => d.ID == item.ID).FirstOrDefault();
                            if (item2 != null)
                            {
                                item2.Result = Serialize.ObjectToByte(item.ResultObj);
                                item2.Contents = Serialize.ObjectToByte(item.Spec);
                                item2.ResultType = item.ResultType;
                                item.Result = item2.Result;
                                item.Contents = item2.Contents;
                                db.SaveChanges();
                            }
                        }
                        if (this.propertyGrid1.InvokeRequired)
                        {
                            ThreadStart s = () => { this.updateColumn(item); };
                            this.propertyGrid1.Invoke(s);
                        }
                        else
                        {
                            this.updateColumn(item);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show("预测出错，请检查模型文件是否正确。", "预测出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #region 内部函数
        /// <summary>
        /// 根据光谱名获取原油信息，光谱名为：码头-XX原油-201204180930
        /// </summary>
        /// <param name="s"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private Specs findOilName(Specs s, string filename)
        {
            if (filename.IndexOf("-") > 0)
            {
                string[] lst = filename.Split('-');
                if (lst.Length == 4)
                {
                    s.LIMSID = lst[0];
                    s.SamplePlace = lst[1];
                    s.OilName = lst[2];
                    DateTime dt;
                    if (DateTime.TryParseExact(lst[3], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        s.SampleTime = dt;
                    }
                    else if (DateTime.TryParseExact(lst[3], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dt))
                    {
                        s.SampleTime = dt;
                    }

                    //s.SampleTime
                    // DateTime.TryParse(lst[1],
                }
            }
            return s;
        }

        /// <summary>
        /// 修改单元格值
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        private void changeCell(int col, int row, string value)
        {
            if (this.propertyGrid1.InvokeRequired)
            {
                ThreadStart s = () => { this.propertyGrid1[col, row].Value = value; };
                this.propertyGrid1.Invoke(s);
            }
            else
            {
                this.propertyGrid1[col, row].Value = value;
            }
        }

        private int findRowIndex(string name)
        {
            int idx = -1;
            for (int i = 0; i < this.propertyGrid1.Rows.Count; i++)
            {
                if (this.propertyGrid1.Rows[i].Tag == null)
                    continue;
                if (this.propertyGrid1.Rows[i].Tag.ToString() == name)
                    return i;
            }
            return idx;
        }

        private void search()
        {
            Action a = () =>
            {
                using (var db = new NIRCeneterEntities())
                {

                    var qlst = db.Specs.Where(d => 1 == 1);
                    //选择用户
                    string username = "";
                    if (this.txbSUserName.InvokeRequired)
                    {
                        ThreadStart s = () => { username = this.txbSUserName.Text.Trim(); };
                        this.txbSUserName.Invoke(s);
                    }
                    else
                        username = this.txbSUserName.Text.Trim();
                    if (username != "全部")
                    {
                        var reg = new Regex(@"\(\w+", RegexOptions.Singleline);
                        var m = reg.Match(username);
                        if (m.Success)
                        {
                            var logname = m.Value.Replace("(", "");
                            var u = db.S_User.Where(d => d.loginName == logname).FirstOrDefault();
                            if (u != null)
                            {
                                qlst = qlst.Where(d => d.UserID == u.ID);
                            }
                        }
                    }

                   
                    //预测方法
                    string methodstr = "";
                    if (this.txbSPredictorName.InvokeRequired)
                    {
                        ThreadStart s = () => { methodstr = this.txbSPredictorName.Text.Trim(); };
                        this.txbSPredictorName.Invoke(s);
                    }
                    else
                        methodstr = this.txbSPredictorName.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(methodstr))
                    {
                        if (methodstr != "全部")
                        {
                            PredictMethod pm = PredictMethod.None;
                            if (methodstr == PredictMethod.Fitting.GetDescription())
                                pm = PredictMethod.Fitting;
                            else if (methodstr == PredictMethod.Identify.GetDescription())
                                pm = PredictMethod.Identify;
                            else if (methodstr == PredictMethod.Integrate.GetDescription())
                                pm = PredictMethod.Integrate;
                            else if (methodstr == PredictMethod.PLSBind.GetDescription())
                                pm = PredictMethod.PLSBind;
                            qlst = qlst.Where(d => d.ResultType == (int)pm);
                        }
                    }
                   
                    //预测时间
                    if (!string.IsNullOrWhiteSpace(this.txbSPtimeS.Text.Trim()))
                        qlst = qlst.Where(d => d.AddTime > this.txbSPtimeS.Value);
                    if(!string.IsNullOrWhiteSpace(this.txbSPtimeE.Text.Trim()))
                        qlst = qlst.Where(d => d.AddTime < this.txbSPtimeE.Value);
                   
                    //采样时间
                    if(!string.IsNullOrWhiteSpace(this.txbSTimeStart.Text.Trim()))
                        qlst = qlst.Where(d => d.SampleTime> this.txbSTimeStart.Value);
                    if (!string.IsNullOrWhiteSpace(this.txbSTimeEnd.Text.Trim()))
                        qlst = qlst.Where(d => d.SampleTime < this.txbSTimeEnd.Value);


                    //原油名
                    if(!string.IsNullOrWhiteSpace(this.txbSOilname.Text))
                        qlst=qlst.Where(d=>d.OilName.Contains(this.txbSOilname.Text.Trim()));
                    //取样点
                    if (!string.IsNullOrWhiteSpace(this.txbSSamplePlace.Text))
                        qlst = qlst.Where(d => d.SamplePlace.Contains(this.txbSSamplePlace.Text.Trim()));
                    var olst = qlst.OrderByDescending(d => d.ID);



                    var plst = new PaginatedList<Specs>(olst, this.pager1.PageIndex < 1 ? 1 : this.pager1.PageIndex, this._pageCount);
                    if (this.propertyGrid1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.initGrid(); };
                        this.propertyGrid1.Invoke(s);
                    }
                    else
                        this.initGrid();

                    foreach (var d in plst)
                    {
                        if (this.propertyGrid1.InvokeRequired)
                        {
                            ThreadStart s = () => { this.updateColumn(d); };
                            this.propertyGrid1.Invoke(s);
                        }
                        else
                            this.updateColumn(d);
                    }


                    if (pager1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.pager1.ShowPage(plst.PageIndex, this._pageCount, plst.TotalCount); this.pager1.Enabled = true; };
                        this.pager1.Invoke(s);
                    }
                    else
                    {
                        this.pager1.ShowPage(plst.PageIndex, this._pageCount, plst.TotalCount);
                        this.pager1.Enabled = true;
                    }
                }
            };
            a.BeginInvoke(null, null);
        }
        #endregion

        #region private Class



        private class MyDataCol : DataGridViewTextBoxColumn
        {
            public Specs Spec { set; get; }

            public int DbIndex { set; get; }
        }

        #endregion

        #region 按钮事件

        private void btnSpecOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Multiselect = true;
            myOpenFileDialog.InitialDirectory = this._config.FolderSpec;
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.addSpectrum(myOpenFileDialog.FileNames);
                this.btnSpecOpen.Enabled = false;
                this.lblBusy.Text = "正在预测...";
                this.toolProgressbar.Visible = true;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.panelSearch.Visible = !panelSearch.Visible;
        }

        private void menuUsers_Click(object sender, EventArgs e)
        {
            new RIPP.App.AnalCenter.Forms.Dlg.FrmUsers(this._user).ShowDialog();
            //new RIPP.App.AnalCenter.Forms.Dlg.FrmUserSet().ShowDialog();
        }

        private void menuMethods_Click(object sender, EventArgs e)
        {
            var frm = new RIPP.App.AnalCenter.Forms.Dlg.FrmModelSet(this._config, this._user);
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                this._config = new Config();
                this._predictor = BindModel.LoadModel(this._config.ModelDefaultPath);
                if (this._predictor != null)
                {
                    this.btnMethodName.Text = this._predictor.Name;
                    this.initGrid();
                }
            }
        }

        private void menuSetting_Click(object sender, EventArgs e)
        {
            if (new Forms.FrmOptions(this._config, this._user.Role == RoleEnum.RIPP).ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._config = new Config();
                this._specCache = new SpecCache(this._config.MaxSpecNum);
                if (this._config.IsAutoEstimate)
                {
                    this._moniter.Start();
                    this.lblAutoRun.Text = "自动";
                }
                else
                {
                    this._moniter.Stop();
                    this.lblAutoRun.Text = "手动";
                }
                this.initGrid();
            }

        }

        private void btnMethodName_Click(object sender, EventArgs e)
        {
            if (this._predictor == null)
            {
                MessageBox.Show("您还没有设置默认分析方法，请先设置", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                new Forms.Dlg.FrmModelDetail(this._predictor).ShowDialog();
            }
        }

        private void btnModelSet_Click(object sender, EventArgs e)
        {
            this.menuMethods.PerformClick();
        }

        private void btnAutorunTrue_Click(object sender, EventArgs e)
        {
            this._config.IsAutoEstimate = true;
            this._config.Save();
            this._moniter.Start();
            this.lblAutoRun.Text = "自动";
        }

        private void btnAutorunFalse_Click(object sender, EventArgs e)
        {
            this._config.IsAutoEstimate = false;
            this._config.Save();
            this._moniter.Stop();
            this.lblAutoRun.Text = "手动";
        }

        private void txbOilName_Leave(object sender, EventArgs e)
        {
            var idx = -1;
            if( this.propertyGrid1.SelectedCells.Count==0)
                return;
            idx = this.propertyGrid1.SelectedCells[0].ColumnIndex;
           
            using (var db = new NIRCeneterEntities())
            {
                var col = this.propertyGrid1.Columns[idx] as MyDataCol;
                if (col == null || col.Spec == null)
                    return;
                var item = db.Specs.Where(d => d.ID == col.Spec.ID).FirstOrDefault();
                if (item != null)
                {
                    item.OilName = this.txbOilName.Text;
                    db.SaveChanges();
                    this.changeCell(col.Index, 3, item.OilName);
                }
            }
        }

        private void txbSamplePlace_Leave(object sender, EventArgs e)
        {
            var idx = -1;
            if (this.propertyGrid1.SelectedCells.Count == 0)
                return;
            idx = this.propertyGrid1.SelectedCells[0].ColumnIndex;
           
            using (var db = new NIRCeneterEntities())
            {
                var col = this.propertyGrid1.Columns[idx] as MyDataCol;
                if (col == null || col.Spec == null)
                    return;
                var item = db.Specs.Where(d => d.ID == col.Spec.ID).FirstOrDefault();
                if (item != null)
                {
                    item.SamplePlace = this.txbSamplePlace.Text;
                    db.SaveChanges();
                    this.changeCell(col.Index, 4, item.SamplePlace);
                }
            }
        }

        private void txbSampleTime_Leave(object sender, EventArgs e)
        {
            var idx = -1;
            if (this.propertyGrid1.SelectedCells.Count == 0)
                return;
            idx = this.propertyGrid1.SelectedCells[0].ColumnIndex;
          
            using (var db = new NIRCeneterEntities())
            {
                var col = this.propertyGrid1.Columns[idx] as MyDataCol;
                if (col == null || col.Spec == null)
                    return;
                var item = db.Specs.Where(d => d.ID == col.Spec.ID).FirstOrDefault();
                if (item != null)
                {
                    item.SampleTime = this.txbSampleTime.Value;
                    db.SaveChanges();
                    this.changeCell(col.Index, 5, item.SampleTime.Value.ToString("MM-dd HH:mm"));
                }
            }
        }

        private void txbAnalyTime_Leave(object sender, EventArgs e)
        {
            var idx = -1;
            if (this.propertyGrid1.SelectedCells.Count == 0)
                return;
            idx = this.propertyGrid1.SelectedCells[0].ColumnIndex;
           
            using (var db = new NIRCeneterEntities())
            {
                var col = this.propertyGrid1.Columns[idx] as MyDataCol;
                if (col == null || col.Spec == null)
                    return;
                var item = db.Specs.Where(d => d.ID == col.Spec.ID).FirstOrDefault();
                if (item != null)
                {
                    item.AnalyTime = this.txbAnalyTime.Value;
                    db.SaveChanges();
                    this.changeCell(col.Index, 6, item.AnalyTime.Value.ToString("MM-dd HH:mm"));
                }
            }
        }

        private void btnSpecDel_Click(object sender, EventArgs e)
        {
            if (this.propertyGrid1.SelectedColumns.Count == 0)
                return;
            for (int i = this.propertyGrid1.SelectedColumns.Count - 1; i >= 0; i--)
            {
                if (this.propertyGrid1.Columns.Count > 1)
                    this.propertyGrid1.Columns.Remove(this.propertyGrid1.SelectedColumns[i]);
            }
        }

        #endregion

        private void txbSTimeStart_ValueChanged(object sender, EventArgs e)
        {
            this.txbSTimeStart.CustomFormat = "MM-dd HH:mm";
            this.txbSTimeEnd.MinDate = this.txbSTimeStart.Value;
        }
        private void txbSPtimeS_ValueChanged(object sender, EventArgs e)
        {
            this.txbSPtimeS.CustomFormat = "MM-dd HH:mm";
            this.txbSPtimeE.MinDate = this.txbSPtimeS.Value;
        }
        private void txbSPtimeE_ValueChanged(object sender, EventArgs e)
        {
            this.txbSPtimeE.CustomFormat = "MM-dd HH:mm";
        }
        private void txbSTimeEnd_ValueChanged(object sender, EventArgs e)
        {
            this.txbSTimeEnd.CustomFormat = "MM-dd HH:mm";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.search();
        }

        private void btnChartSpec_Click(object sender, EventArgs e)
        {
            var lst = new List<MyDataCol>();
            for (int i = 0; i < this.propertyGrid1.Columns.Count; i++)
            {
                var col = this.propertyGrid1.Columns[i] as MyDataCol;
                if (col != null)
                    lst.Add(col);
            }
            this.drawSelectLines(lst);
        }

        private void btnChartCol_Click(object sender, EventArgs e)
        {
            this.btnChartSpec.PerformClick();
        }

        private void menuSpecs_Click(object sender, EventArgs e)
        {
            this.btnSearch.PerformClick();
        }

        private void btnSpecNew_Click(object sender, EventArgs e)
        {
            this.initGrid();
        }

        private void btnGetDetail_Click(object sender, EventArgs e)
        {
            if (this.propertyGrid1.SelectedCells.Count > 0)
            {
                var dcol = this.propertyGrid1.Columns[this.propertyGrid1.SelectedCells[0].ColumnIndex] as MyDataCol;
                if (dcol == null)
                    return;
                this.btnGetDetail.Enabled = false;
                this.btnGetDetail.Text = "正在获取";
                var dlg = new Forms.Dlg.FrmOilDetailcs();
                Action a = () =>
                {
                    var api = new OilAPI(this._config.Properties);
                    var spec = dcol.Spec;
                    if (!api.APIGetData(this._predictor, ref spec))
                    {
                        MessageBox.Show("没有数据！");
                        dcol.Spec.OilData = null;
                    }
                     else
                     {
                         ThreadStart s = () => { dlg.ShowData(dcol.Spec.OilData); };
                         this.Invoke(s);
                     }
                    if (this.btnGetDetail.InvokeRequired)
                    {
                        ThreadStart s = () =>
                        {
                            this.btnGetDetail.Enabled = true;
                            this.btnGetDetail.Text = "快评数据";
                        };
                        this.btnGetDetail.Invoke(s);
                    }
                    else
                    {
                        this.btnGetDetail.Enabled = true;
                        this.btnGetDetail.Text = "快评数据";
                    }
                };
                a.BeginInvoke(null, null);

            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            List<Specs> lst = new List<Specs>();
            for (int i = 0; i < this.propertyGrid1.SelectedColumns.Count; i++)
            {
                var col = this.propertyGrid1.SelectedColumns[i] as MyDataCol;
                if (col != null && col.Spec != null)
                    lst.Add(col.Spec);
            }
            if (lst.Count == 0)
            {

                MessageBox.Show("请选择要导出的样本");
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件|*.*|xls文件|*.xls|所有文件|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.FileName = @"Export.xls";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = this._config.FolderData;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = saveFileDialog.FileName.ToString();
                this.btnExport.Enabled = false;
                this.lblBusy.Text = "正在导出...";

                Action a = () =>
               {
                   foreach (var s in lst)
                   {
                       if (s.OilData == null)
                       {
                           var api = new OilAPI(this._config.Properties);
                           s.OilData = api.GetData(s, this._predictor);
                       }
                   }

                   if (!ExcelExportProvider.ExportToExcel(lst, fName))
                       MessageBox.Show("导出失败");
                   if (this.toolStrip1.InvokeRequired)
                   {
                       ThreadStart s = () => { this.btnExport.Enabled = true; };
                       this.toolStrip1.Invoke(s);
                   }
                   else
                       this.btnExport.Enabled = true;
                   if (this.statusStrip1.InvokeRequired)
                   {
                       ThreadStart s = () => { this.lblBusy.Text = ""; };
                       this.statusStrip1.Invoke(s);
                   }
                   else
                       this.lblBusy.Text = "";
               };
                a.BeginInvoke(null, null);
            }


            //导出 TO DO LIST
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new Forms.Dlg.FrmPropertyEdit(this._config).ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._config = new Config();
                this.initGrid();
            }
        }

        private void btnLIMS_Click(object sender, EventArgs e)
        {
            List<Specs> lst = new List<Specs>();
            for (int i = 0; i < this.propertyGrid1.SelectedColumns.Count; i++)
            {
                var col = this.propertyGrid1.SelectedColumns[i] as MyDataCol;
                if (col != null && col.Spec != null)
                    lst.Add(col.Spec);
            }
            if (lst.Count == 0)
            {

                MessageBox.Show("请选择要上传的样本");
                return;
            }

            FolderBrowserDialog saveFileDialog = new FolderBrowserDialog();
            saveFileDialog.SelectedPath = this._config.FolderLIMS;
            var points = new CutSetting().Point;
            var cutpoints = new List<int[]>();
            foreach (var p in points)
                cutpoints.Add(new int[] { p[0], p[2], p[4], p[6] });

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fPath = saveFileDialog.SelectedPath;
                this.btnLIMS.Enabled = false;
                this.lblBusy.Text = "正在上传...";
                Action a = () =>
                {
                    foreach (var s in lst)
                    {
                        if (s.OilData == null)
                        {
                            var api = new OilAPI(this._config.Properties);
                            s.OilData = api.GetData(s, this._predictor);
                        }


                        //先获取所有TBP
                        double[] x, y;
                        this.getLinedata(s.Spec.Components, out x, out y);
                        if (y != null && y.Length == 23 && cutpoints != null && cutpoints.Count > 0)
                        {
                            try
                            {
                                var cutresult = RIPP.NIR.Data.Tools.ydcaculate(y, cutpoints);
                                if (cutresult != null && cutresult.Count > 0)
                                {
                                    using (var dbi = new NIRCeneterEntities())
                                    {
                                        var str = string.Format("TBP:\n[{0}] \n\n tempoint:\n[{1}] \n\n yd:\n[{2}]",
                                            string.Join(",", y),
                                          string.Join(",", cutpoints.Select(d => string.Format("[{0}]", string.Join(",", d)))),
                                          string.Join(",", cutresult.Select(d => string.Format("[{0}]", string.Join(",", d)))));
                                        dbi.CutResult.AddObject(new CutResult()
                                        {
                                            AddTime = DateTime.Now,
                                            Contents = str,
                                            SpecID = s.ID
                                        });
                                        dbi.SaveChanges();


                                        //var sql = "insert YDResult";
                                        var keys = new List<string>();
                                        var vals = new List<string>();
                                        keys.Add("DESCRIPTION");
                                        vals.Add("'原油近红外快速评价'");
                                        keys.Add("SAMPLEDATE");
                                        vals.Add(string.Format("'{0}'", s.SampleTime.HasValue?s.SampleTime.Value.ToString("yyyy-MM-dd HH:mm"):""));
                                        keys.Add("SAMPLING_POINT");
                                        vals.Add(string.Format("'{0}'",s.SamplePlace));
                                        keys.Add("SAMPLEID");
                                        vals.Add(s.ID.ToString());
                                        keys.Add("LMISID");
                                        vals.Add(string.Format("'{0}'", s.LIMSID));
                                        int i = 1;
                                        foreach (var arry in cutresult)
                                        {
                                            double sum = 0;
                                            foreach (var ad in arry)
                                            {
                                                sum += ad;
                                                keys.Add(string.Format("YD_VALUE{0}", i));
                                                vals.Add(ad.ToString());
                                                i++;
                                            }
                                            keys.Add(string.Format("YD_VALUE{0}", i));
                                            vals.Add((99.75 - sum).ToString());
                                            i++;
                                        }

                                        var sql = string.Format("insert YDResult ({0}) values ({1})",
                                            string.Join(",", keys),
                                            string.Join(",", vals));
                                        dbi.ExecuteStoreCommand(sql);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }

                        }
                    }
                    List<string> errorName = new List<string>();
                    foreach (var s in lst)
                    {
                        if (!LIMS.WriteToFile(s,this._config.Properties, Path.Combine(fPath, string.Format("{1}_{0}.txt", s.Spec.Name, s.ID)), this._config.LIMSDataDescription))
                            errorName.Add(s.Spec.Name);
                    }
                    if (errorName.Count > 0)
                        MessageBox.Show(string.Format("上传LIMS失败，以下文件未上传:\n{0}", string.Join("\n", errorName)));
                    if (this.toolStrip1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.btnLIMS.Enabled = true; };
                        this.toolStrip1.Invoke(s);
                    }
                    else
                        this.btnLIMS.Enabled = true;
                    if (this.statusStrip1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.lblBusy.Text = ""; };
                        this.statusStrip1.Invoke(s);
                    }
                    else
                        this.lblBusy.Text = "";

                    
                };
                a.BeginInvoke(null, null);
            }
        }

        private void btnUserinfo_Click(object sender, EventArgs e)
        {
            if (this._user.Role == RoleEnum.RIPP)
            {
                MessageBox.Show("RIPP用户不用修改个人资料");
                return;
            }
            new Forms.Dlg.frmUserDetail().ShowUser(this._user);
        }

        private void menuPredictLib_Click(object sender, EventArgs e)
        {
            if (new Forms.Dlg.FrmBaseIdManage().ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._specCache = new SpecCache(this._config.MaxSpecNum);
            }
        }

        private void menuCutsetting_Click(object sender, EventArgs e)
        {
            new Forms.FrmCutSetting().ShowDialog();
        }

        private void mTUpload_Click(object sender, EventArgs e)
        {

        }

       
       

       
    }
}