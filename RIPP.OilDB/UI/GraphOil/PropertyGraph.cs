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
    public partial class PropertyGraph : UserControl
    {
        private CurveEntity _curve; //一条性质曲线
      
        public PropertyGraph()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 一条性质曲线
        /// </summary>
        public CurveEntity curve
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
        public void DrawCurve(CurveEntity curve = null)
        {
            this._curve = curve;
            if (curve == null)
            {              
                this.zedGraph1.GraphPane.CurveList.Clear();
                this.setTitle();
                this.zedGraph1.AxisChange();
                this.zedGraph1.Refresh();
            }
            else
            {
                //this._curve = curve;
                if (this._curve.X == null)
                {
                    this.zedGraph1.GraphPane.CurveList.Clear();
                    this.setTitle();
                    this.zedGraph1.AxisChange();
                    this.zedGraph1.Refresh();
                }

                //去掉Y值为Float.MaxValue的点
                PointPairList list = new PointPairList();
                for (int i = 0; i < _curve.Y.Count(); i++)
                {
                    if (float.MaxValue != _curve.Y[i])
                    {
                        list.Add(_curve.X[i], _curve.Y[i]);
                    }
                }

                //this.zedGraph1.GraphPane.AddCurve(this._curve.descript, this._curve.X.ToArray(), this._curve.Y.ToArray(), this._curve.Color, SymbolType.None);
                this.zedGraph1.GraphPane.CurveList.Clear();
                this.zedGraph1.GraphPane.AddCurve(this._curve.descript, list, this._curve.Color, SymbolType.None);
                this.setTitle();
                this.zedGraph1.AxisChange();
                this.zedGraph1.Refresh();
            }          
        }     

        /// <summary>
        /// 设置绘图的样式
        /// </summary>
        private void setTitle()
        {
            GraphPane myPane = this.zedGraph1.GraphPane;
            if (this._curve == null)
            {
                myPane.Legend.IsVisible = false;
                //字体
                myPane.Border.Width = 0;
                myPane.Border.Color = Color.White;
                myPane.Title.FontSpec.IsBold = false;
                myPane.XAxis.Title.FontSpec.IsBold = false;
                myPane.YAxis.Title.FontSpec.IsBold = false;

                myPane.XAxis.Scale.MaxGrace = 0;
                myPane.XAxis.Scale.MinGrace = 0;

                myPane.Title.Text = "";
                myPane.YAxis.Title.Text = "";
                myPane.XAxis.Title.Text = "";
            }
            else
            {              
                myPane.Legend.IsVisible = false;
                //字体
                myPane.Border.Width = 0;
                myPane.Border.Color = Color.White;
                myPane.Title.FontSpec.IsBold = false;
                myPane.XAxis.Title.FontSpec.IsBold = false;
                myPane.YAxis.Title.FontSpec.IsBold = false;

                myPane.XAxis.Scale.MaxGrace = 0;
                myPane.XAxis.Scale.MinGrace = 0;


                if (this.curve.propertyX == "ECP")
                {
                    myPane.YAxis.Title.Text = this.curve.descript;
                    myPane.XAxis.Title.Text = "初切点";
                    myPane.Title.Text = "初切点 - " + this.curve.descript;
                }
                else if (this.curve.propertyX == "WY")
                {
                    myPane.YAxis.Title.Text = this.curve.descript;
                    myPane.XAxis.Title.Text = "质量单收率";
                    myPane.Title.Text = "质量单收率 - " + this.curve.descript;                
                }
            }
        }

    }
}
