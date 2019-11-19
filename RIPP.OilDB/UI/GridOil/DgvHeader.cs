using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil
{
    public class DgvHeader
    {
        /// <summary>
        /// 设置表头
        /// </summary>
        public void SetAppDataBaseBColHeader(DataGridView dgv)
        {
            //清除表的行和列
            dgv.Columns.Clear();

            #region 添加表头

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "英文名称", HeaderText = "英文名称", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "产地国家", HeaderText = "产地国家", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "地理区域", HeaderText = "地理区域", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评价日期", HeaderText = "评价日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "入库日期", HeaderText = "入库日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "数据来源", HeaderText = "数据来源", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "类别", HeaderText = "类别", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "基属", HeaderText = "基属", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "硫水平", HeaderText = "硫水平", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "酸水平", HeaderText = "酸水平", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            #endregion
        }
        /// <summary>
        /// 打开A库的表头设置
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="Visible"></param>
        /// <param name="checkBoxShow"></param>
        public void SetMangerDataBaseAColHeader(DataGridView dgv, bool Visible,  bool checkBoxShow = false)
        {
            dgv.Columns.Clear(); //清除表的行和列
            #region 添加表头
            
            dgv.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "选择", Name = "select", Width = 70, ReadOnly = false, Visible = checkBoxShow });           //添加CheckBox的列
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "序号", HeaderText = "序号", Width = 70, Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "相似度总和", HeaderText = "相似度总和", Visible = Visible, ReadOnly = true });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "英文名称", HeaderText = "英文名称", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "产地国家", HeaderText = "产地国家", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "地理区域", HeaderText = "地理区域", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "油田区块", HeaderText = "油田区块", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "采样日期", HeaderText = "采样日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "到院日期", HeaderText = "到院日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "采样地点", HeaderText = "采样地点", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评价日期", HeaderText = "评价日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "入库日期", HeaderText = "入库日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "数据来源", HeaderText = "数据来源", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评价单位", HeaderText = "评价单位", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评价人员", HeaderText = "评价人员", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "报告号", HeaderText = "报告号", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
           
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "类别", HeaderText = "类别", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "基属", HeaderText = "基属", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "硫水平", HeaderText = "硫水平", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "酸水平", HeaderText = "酸水平", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "腐蚀指数", HeaderText = "腐蚀指数", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "加工指数", HeaderText = "加工指数", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            
            #endregion
        }


        /// <summary>
        /// 打开B库的表头设置
        /// </summary>
        /// <param name="Visible"></param>
        /// <param name="checkBoxShow">是否显示选择空间，默认选择</param>
        public void SetMangerDataBaseBColHeader(DataGridView dgv, bool Visible,  bool checkBoxShow = false)
        {
            dgv.Columns.Clear();//清除表的行和列
            #region 添加表头 
            dgv.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "选择", Name = "select", Width = 70, ReadOnly = false, Visible = checkBoxShow });           //添加CheckBox的列
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "序号", HeaderText = "序号", Width = 70, Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "相似度总和", HeaderText = "相似度总和", Visible = Visible, ReadOnly = true });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "英文名称", HeaderText = "英文名称", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "产地国家", HeaderText = "产地国家", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "地理区域", HeaderText = "地理区域", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评价日期", HeaderText = "评价日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "入库日期", HeaderText = "入库日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "数据来源", HeaderText = "数据来源", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
             
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "类别", HeaderText = "类别", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "基属", HeaderText = "基属", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "硫水平", HeaderText = "硫水平", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = "酸水平", HeaderText = "酸水平", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });                             
            #endregion
        }


    }
}
