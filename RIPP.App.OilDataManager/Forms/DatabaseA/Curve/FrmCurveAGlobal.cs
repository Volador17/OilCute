using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.App.OilDataManager.Forms.DatabaseA.Curve
{
    public class FrmCurveAGlobal
    {
        /// <summary>
        /// 表格中A库的X行号_ARowXIndex = 0
        /// </summary>
        public static readonly int _ARowXIndex = 0;
        /// <summary>
        /// 表格中A库的Y行号_BRowYIndex = 1
        /// </summary>
        public static readonly int _ARowYIndex = 1;
        /// <summary>
        /// 表格的ECP行号_ECPRowIndex = 2;
        /// </summary>
        public static readonly int _ECPRowIndex = 2;
        /// <summary>
        /// 表格中要做曲线X行号_curveRowXIndex = 3
        /// </summary>
        public static readonly int _curveRowXIndex = 3;//表格中要做曲线X行号
        /// <summary>
        /// 表格中要做曲线Y行号 _curveRowYIndex = 4
        /// </summary>
        public static readonly int _curveRowYIndex = 4; //表格中要做曲线Y列的行号
        /// <summary>
        /// 表格中数据起始列 _dataColStart = 3
        /// </summary>
        public static readonly int _dataColStart = 3; //数据起始列
 
        /// <summary>
        /// 收率曲线和馏分曲线B库的固定数据点个数 _dataBcol = 19
        /// </summary>
        public static readonly int _dataBcol = 19; //收率曲线和馏分曲线B库的固定数据点个数
        /// <summary>
        /// 渣油曲线B库的固定数据点个数 _dataBcolResidu = 8
        /// </summary>
        public static readonly int _dataBcolResidu = 8; //渣油曲线B库的固定数据点个数
        /// <summary>
        /// 馏分性质曲线和馏分曲线的固定18个点(float):15, 60, 100, 140, 160, 180, 200, 220, 240, 280, 320, 350, 400, 425, 450, 500, 540, 560 
        /// </summary>
        public static readonly List<float> B_XList = new List<float>() { 15, 60, 80, 100, 140, 160, 180, 200, 220, 240, 280, 320, 350, 400, 425, 450, 500, 540, 560 };//固定x轴点
        /// <summary>
        /// 渣油馏分性质曲线的8个固定点设置: 320, 350, 400, 425, 450, 500, 540, 560
        /// </summary>
        public static readonly List<float> ECPList = new List<float> { 320, 350, 400, 425, 450, 500, 540, 560 };//渣油曲线固定的点

    }
}
