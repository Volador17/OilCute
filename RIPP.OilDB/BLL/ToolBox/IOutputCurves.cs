using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using System.Windows.Forms;

namespace RIPP.OilDB.BLL.ToolBox
{
    public interface IOutputCurves
    {     
        /// <summary>
        /// 初始化坐标轴表格
        /// </summary>
        void initAxisDgv();
        /// <summary>
        /// 初始化曲线数据表格
        /// </summary>
        void initCurveDataDgv();
        /// <summary>
        /// 初始化绘图曲线
        /// </summary>
        void initZedGraph();

        void drawCurves();
    }
}
