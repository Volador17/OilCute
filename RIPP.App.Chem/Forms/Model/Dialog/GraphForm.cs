using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.App.Chem.Forms.Model.Dialog
{
    public partial class GraphForm : Form
    {
        public GraphForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(GraphForm_Load);
        }

        void GraphForm_Load(object sender, EventArgs e)
        {
            var myPane = this.zedGraphControl1.GraphPane;

            //字体
            myPane.Border.Width = 0;
            myPane.Border.Color = Color.White;
            myPane.Title.FontSpec.IsBold = false;
            myPane.XAxis.Title.FontSpec.IsBold = false;
            myPane.YAxis.Title.FontSpec.IsBold = false;

            myPane.XAxis.Scale.MaxGrace = 0;
            myPane.XAxis.Scale.MinGrace = 0;
        }

        public void Drawline(double[] x, string title,string xlable,string ylable)
        {
            if (x == null)
                return;
            
            this.Text = title;

            var myPane = this.zedGraphControl1.GraphPane;
            myPane.Title.Text = title;
            myPane.XAxis.Title.Text = xlable;
            myPane.YAxis.Title.Text = ylable;
            double[] idx = new double[x.Length];
            for(int i=1;i<=idx.Length;i++)
                idx[i-1]=i;
            myPane.AddCurve("", idx, x, Color.Blue ,ZedGraph.SymbolType.None);
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();
            this.ShowDialog();

        }
    }
}
