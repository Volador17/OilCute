using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.UI.GraphOil
{
    public partial class PropertyGraphA : UserControl
    {
        private CurveAEntity _curve; //一条性质曲线
        private int _index;//保存选中点的下标
        private double _y;//保存选中点的下标
        public int index
        {
            get { return _index; }
            set { _index = value; }
        }
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }
        public PropertyGraphA()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 一条性质曲线
        /// </summary>
        public CurveAEntity curve
        {
            set
            {
                this._curve = value;
            }
            get { return this._curve; }
        }

        /// <summary>
        /// 绘制一条性质曲线
        /// </summary>
        /// <param name="curve">一条性质曲线</param>
        public void DrawCurve(CurveAEntity curve = null)
        {
            if (curve == null)
                return;
            this._curve = curve;

            if (this._curve.X == null)
                return;
            //this.zedGraph1.GraphPane.AddCurve(this._curve.descript, this._curve.X.ToArray(), this._curve.Y.ToArray(), this._curve.Color, SymbolType.None);

            PointPairList list = zedGraph1.ZedExpand.PointList;
            list.Clear();
            for (int i = 0; i < this._curve.X.Count(); i++)
            {
                list.Add(this._curve.X[i], this._curve.Y[i]);
            }
            this.zedGraph1.GraphPane.CurveList.Clear();

            this.zedGraph1.GraphPane.AddCurve(this._curve.descript, list, this._curve.Color, SymbolType.Square);
            //this.zedGraph1.GraphPane.AddCurve(this._curve.descript, this._curve.X.ToArray(), this._curve.Y.ToArray(), this._curve.Color, SymbolType.Square);
            this.setTitle();
            this.zedGraph1.AxisChange();
            this.zedGraph1.Refresh();



        }

        /// <summary>
        /// 设置绘图的样式
        /// </summary>
        private void setTitle()
        {
            if (this._curve == null)
                return;
            GraphPane myPane = this.zedGraph1.GraphPane;

            this.zedGraph1.IsEnableVEdit = true;

            myPane.Legend.IsVisible = false;
            //字体
            myPane.Border.Width = 0;
            myPane.Border.Color = Color.White;
            myPane.Title.FontSpec.IsBold = false;
            myPane.XAxis.Title.FontSpec.IsBold = false;//设置x轴的文字粗体
            myPane.YAxis.Title.FontSpec.IsBold = false;

            myPane.XAxis.Scale.MaxGrace = 0;
            myPane.XAxis.Scale.MinGrace = 0;

            myPane.Title.Text = this.curve.descript;
            myPane.XAxis.Title.Text = this.curve.propertyX;
            //myPane.YAxis.Title.Text = this.curve.propertyY;           
        }

        private void zedGraph1_MouseMove(object sender, MouseEventArgs e)
        {
            index = this.zedGraph1.ZedExpand.Index;
            y = this.zedGraph1.ZedExpand.PointList[index].Y;
        }
    }
}
