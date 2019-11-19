using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using System.Data;
using System.Windows.Forms;
using RIPP.OilDB.UI.GridOil;

namespace RIPP.OilDB.Data
{
    public class GcTableFuntion
    {
        #region "私有参数"
        private int inputDataColStart = 2;//输入表数据数据的起始列数 
        private int inputDataRowStart = 8;//输入表头的行数（包括组成列名和列值）
        private int inputRowICP = 0;//输入的ICP行
        private int inputRowECP = 1;//输入的ECP行
        private int inputRowWY = 2;//输入的WY行
        private int inputRowTWY = 3;//输入的TWY行
        private int inputRowVY = 4;//输入的VY行      
        private int inputRowAPI = 5;//输入的API行
        private int inputRowD20 = 6;//输入的D20行
        private int inputDataColNum = 5;//输入表的数据列的个数--5个/////////////////////////////////////
 

        private int StaDataColStart = 4;//统计表的数据起始列
        private int StaCodeCol = 3;//统计表的代码列
        private int StaDataColNum = 5;//统计表的数据列//////////////////////////////////////////
        private int StaDataRowStart = 2;//统计表的起始行
        private int StaDataRowLast = 4;//统计表的后4行
        private int StaDataRowICP = 0;//统计表的ICP行
        private int StaDataRowECP = 1;//统计表的ECP行
        private int normalOutputDataColStart = 5;//标准GC输出表的起始Cut列的位置(校正数据列的起始列)
        private int normalOutputWTCol = 9;//标准GC输出表的WT%的列的位置
        private int normalOutputRowStart = 7;//标准GC输出表的数据列G01-G68开时的行的位置

        private IList<OilTableColEntity> _cols = null;
        private IList<OilTableRowEntity> _rows = null;
        private IList<OilDataEntity> _datas = null;
       
        List<OilTableRowEntity> _normRows = null;//标准的行集合
        List<OilTableColEntity> _normCols = null;//标准的列集合
        List<GCMatch1Entity> MatchTable1 = null;//读出配置表1
        List<GCMatch2Entity> MatchTable2 = null;//读出配置表2
 

        Dictionary<string, float[]> _gcCodeandValue = null;//存储统计表数据
        Dictionary<string, float[]> _gcNormOutput = null;//存储标准表数据
        List<GCInputEntity> _gcInputDatas = new List<GCInputEntity>();//存储输入表数据
        #endregion

        #region "构造函数"
       
        /// <summary>
        /// 
        /// </summary>
        public GcTableFuntion()
        {

        }

        /// <summary>
        /// 加载两个配置表
        /// </summary>
        private void _getMatch()
        {
            /*计算前需判断Input表中的数据列为数据*/
            GCMatch1Access gcMatch1Access = new GCMatch1Access();
            MatchTable1 = gcMatch1Access.Get("1=1");
        

            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            MatchTable2 = gcMatch2Access.Get("1=1");

        
        }
        #endregion 

        #region "公有函数"
        /// <summary>
        /// 将GC输入表数据输入表数据
        /// </summary>
        public void InputData(DataGridView dataGridView)
        {
            this._gcInputDatas.Clear();//清空数据

            for (int i = 0; i < this._cols.Count; i++)//输入表的列循环
            {
                #region "上侧数据"

                string strICP = dataGridView[this.inputDataColStart + 2 * i, this.inputRowICP].Value != null ? dataGridView[this.inputDataColStart + 2 * i, this.inputRowICP].Value.ToString() : string.Empty;
                string strECP = dataGridView[this.inputDataColStart + 2 * i, this.inputRowECP].Value != null ? dataGridView[this.inputDataColStart + 2 * i, this.inputRowECP].Value.ToString() : string.Empty;
                string strWY = dataGridView[this.inputDataColStart + 2 * i, this.inputRowWY].Value != null ? dataGridView[this.inputDataColStart + 2 * i, this.inputRowWY].Value.ToString() : string.Empty;
                string strTWY = dataGridView[this.inputDataColStart + 2 * i, this.inputRowTWY].Value != null ? dataGridView[this.inputDataColStart + 2 * i, this.inputRowTWY].Value.ToString() : string.Empty;
                string strVY = dataGridView[this.inputDataColStart + 2 * i, this.inputRowVY].Value != null ? dataGridView[this.inputDataColStart + 2 * i, this.inputRowVY].Value.ToString() : string.Empty;
                string strAPI = dataGridView[this.inputDataColStart + 2 * i, this.inputRowAPI].Value != null ? dataGridView[this.inputDataColStart + 2 * i, this.inputRowAPI].Value.ToString() : string.Empty;
                string strD20 = dataGridView[this.inputDataColStart + 2 * i, this.inputRowD20].Value != null ? dataGridView[this.inputDataColStart + 2 * i, this.inputRowD20].Value.ToString() : string.Empty;
              
                if (strICP != string.Empty)
                {
                    GCInputEntity inputICP = new GCInputEntity(i, this.inputRowICP, "ICP", strICP, this._cols[i].colCode,this._parent.Oil.ID, true);
                    this._gcInputDatas.Add(inputICP);
                }
                if (strECP != string.Empty)
                {
                    GCInputEntity inputECP = new GCInputEntity(i, this.inputRowECP, "ECP", strECP, this._cols[i].colCode, this._parent.Oil.ID, true);
                    this._gcInputDatas.Add(inputECP);
                }
                if (strWY != string.Empty)
                {
                    GCInputEntity inputWY = new GCInputEntity(i, this.inputRowWY, "WY", strWY, this._cols[i].colCode, this._parent.Oil.ID, true);
                    this._gcInputDatas.Add(inputWY);
                }

                if (strTWY != string.Empty)
                {
                    GCInputEntity inputTWY = new GCInputEntity(i, this.inputRowTWY, "TWY", strTWY, this._cols[i].colCode, this._parent.Oil.ID, true);
                    this._gcInputDatas.Add(inputTWY);
                }

                if (strVY != string.Empty)
                {
                    GCInputEntity inputVY = new GCInputEntity(i, this.inputRowVY, "VY", strVY, this._cols[i].colCode, this._parent.Oil.ID, true);
                    this._gcInputDatas.Add(inputVY);
                }
                if (strD20 != string.Empty)
                {
                    GCInputEntity inputD20 = new GCInputEntity(i, this.inputRowD20, "D20", strD20, this._cols[i].colCode, this._parent.Oil.ID, true);
                    this._gcInputDatas.Add(inputD20);
                }
                if (strAPI != string.Empty)
                {
                    GCInputEntity inputAPI = new GCInputEntity(i, this.inputRowAPI, "API", strAPI, this._cols[i].colCode, this._parent.Oil.ID, true);
                    this._gcInputDatas.Add(inputAPI);
                }
                #endregion

                #region "下侧数据"

                for (int j = this.inputDataRowStart; j < dataGridView.Rows.Count; j++)
                {
                    string strX = dataGridView[this.inputDataColStart + i * 2, j].Value != null ? dataGridView[this.inputDataColStart + i * 2, j].Value.ToString() : string.Empty;
                    string strY = dataGridView[this.inputDataColStart + 1 + i * 2, j].Value != null ? dataGridView[this.inputDataColStart + 1 + i * 2, j].Value.ToString() : string.Empty;

                    if (strX == string.Empty || strY == string.Empty)
                        continue;
                    GCInputEntity newInput = new GCInputEntity(i, j, strX, strY, this._cols[i].colCode, this._parent.Oil.ID, false);
                    this._gcInputDatas.Add(newInput);
                }

                #endregion
            }
        }
        /// <summary>
        /// 判断输入表的头部是否含有值
        /// </summary>
        /// <returns></returns>
        public bool InputHeadCheck()
        {
            bool bRetrun = false;//返回值用来判断输入表头是否用来正确。
            if (this._gcInputDatas.Count <= 0)//当前表格不存在值
                return bRetrun;


            List<GCInputEntity> gcHeadDatas = this._gcInputDatas.Where(o => o.IsHeadValue == true).ToList();//取出头部数据

            if (gcHeadDatas == null)
                bRetrun = false;//头部不存在数据
            if (gcHeadDatas.Count <= 0)
                bRetrun = false;//头部不存在数据
             
            int count = 0;//判断含有多少个头部完整的馏分段
            foreach (var temp in this._cols)
            {
                List<GCInputEntity> headDatas = gcHeadDatas.Where(o => o.wCut == temp.colCode).ToList();
                if (headDatas == null)
                    continue;
                if (headDatas.Count == 7)
                    count++;
            }          
            
            if (count == 0)//头部不存在完整的馏分段
            {
                bRetrun = false;
            }
            else
            {
                bRetrun = true;
            }
            return bRetrun;
        }          
        /// <summary>
        /// 输入数据归一
        /// </summary>
        /// <returns></returns>
        public void InputDataGuiYi(ref DataGridView dataGridView)
        {
            #region "输入判断"

            if (this._gcInputDatas.Count <= 0)//当前表格不存在值
                return;

            #endregion 

            for (int k = 0; k < this._cols.Count; k++)
            {
                #region "每一列的数据判断"

                List<GCInputEntity> InputColDatas = this._gcInputDatas.Where(o => o.wCut == this._cols[k].colCode).ToList();//找出当列数据(包括列头)
                if (InputColDatas == null)
                    continue;
                if (InputColDatas.Count <= 0)
                    continue;

                List<GCInputEntity> colsHead = InputColDatas.Where(o => o.IsHeadValue == true ).ToList();//找出上侧数据部分
                if (colsHead == null)
                    continue;
                if (colsHead.Count == 0)//输入表的上侧数据为空
                    continue;


                List<GCInputEntity> colsDown = InputColDatas.Where(o => o.IsHeadValue == false).ToList();//找出下侧数据部分
                if (colsDown == null)
                    continue;
               
                #endregion 

                #region "每一列的数据归一"

                float sum = 0;//每列数据加和
                for (int i = 0; i < colsDown.Count; i++)//输入行
                {
                    float fvalue = 0;
                    if (float.TryParse(colsDown[i].itemValue, out fvalue))
                    {
                        colsDown[i].fValue = fvalue;
                        sum += fvalue;//每列数据加和
                    }
                }

                if (Math.Abs(100 - sum) >= 1)//数据严格缺失，删除数据
                {
                    if (MessageBox.Show(this._cols[k].colCode + "馏分的GC数据严重不归一！建议审查数据或删除此列。", "归一检测", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }                 
                }
                else if (Math.Abs(100 - sum) < 1 && Math.Abs(100 - sum) >= 0.1)//归一数据
                {
                    if (MessageBox.Show(this._cols[k].colCode + "馏分的GC数据不归一！是否做归一校正?", "归一检测", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        for (int j = 0; j < colsDown.Count; j++)
                        {
                            colsDown[j].fValue = colsDown[j].fValue / sum * 100;
                            colsDown[j].itemValue = colsDown[j].fValue.ToString();
                            dataGridView[this.inputDataColStart + 1 + colsDown[j].Col * 2 , colsDown[j].Row].Value = colsDown[j].fValue.ToString();
                        }
                    }
                    else
                    {
                        continue;
                    }
                }          
                #endregion 
            }                          
        }
        /// <summary>
        /// 输入表保存
        /// </summary>
        public void InputSave(DataGridView dataGridView)
        {
            InputData(dataGridView);//归一后的数据
            this._gcInputDatas = ItemValueTofVlaue(this._gcInputDatas);
            var access = new GCInputAccess();
            access.Delete("oilInfoID=" + this._parent.Oil.ID);
            OilBll.saveTables(this._gcInputDatas);
        }

        /// <summary>
        /// 将字符数据抓换成浮点数据
        /// </summary>
        /// <param name="InputDatas"></param>
        /// <returns></returns>
        private List<GCInputEntity> ItemValueTofVlaue(List<GCInputEntity> InputDatas)
        {
            if (InputDatas == null)
                return null;
            if (InputDatas.Count <= 0)
                return null;

            for (int i = 0; i < InputDatas.Count; i++)
            {
                float fvalue = 0;
                if (float.TryParse(InputDatas[i].itemValue, out fvalue))
                {
                    InputDatas[i].fValue = fvalue;
                }
            }
            return InputDatas;
        }      

        #endregion

        #region "GC统计表"
        /// <summary>
        /// 统计表的代码和5个CUT空列
        /// </summary>
        private Dictionary<string, float[]> GCCodeAndValue()
        {
            List<OilTableRowEntity> rows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GC).ToList();
            Dictionary<string, float[]> GcClassifi = new Dictionary<string, float[]>();
            for (int i = 0; i < rows.Count; i++)
            {
                GcClassifi.Add(rows[i].itemCode, new float[5]);
            }
            return GcClassifi;
        }
        /// <summary>
        /// GC数据的分类统计表
        /// </summary>
        /// <param name="MatchTable1"></param>
        /// <returns></returns>
        public void GcClaAndStaTable()
        {
            DeleteAllClaAndStaGridValue();//删除统计表的数据重新填写和显示

            _gcCodeandValue = GCCodeAndValue();//输出表的格式

            for (int k = 0; k < this._cols.Count; k++)//输入表的列循环
            {
                #region "每一列的数据判断"

                List<GCInputEntity> InputColDatas = this._gcInputDatas.Where(o => o.wCut == this._cols[k].colCode).ToList();//找出当列数据(包括列头)
                if (InputColDatas == null)
                    continue;
                if (InputColDatas.Count <= 0)
                    continue;

                List<GCInputEntity> colsHead = InputColDatas.Where(o => o.IsHeadValue == true).ToList();//找出上侧数据部分
                if (colsHead == null)
                    continue;
                if (colsHead.Count == 0)//输入表的上侧数据为空
                    continue;


                List<GCInputEntity> colsDown = InputColDatas.Where(o => o.IsHeadValue == false).ToList();//找出下侧数据部分
                if (colsDown == null)
                    continue;
                if (colsDown.Count == 0)//输入表的下侧数据为空
                    continue;

                #endregion

                #region "输入表和统计表数据转换算法"

                if (colsHead.Count == 7)//表头不为空
                {
                    for (int i = 0; i < colsDown.Count; i++)//循环每一数据行
                    {
                        string GcName = colsDown[i].itemName;////输入表的输入的物性名称

                        #region "查找strGcCode"

                        string strGcCode = string.Empty;//

                        if (GcName == string.Empty)
                        {
                            strGcCode = "G66";//Unknow
                        }
                        else if (GcName != string.Empty)//找到名字输入物性不为空的名字
                        {
                            for (int j = 0; j < MatchTable1.Count; j++)//对匹配表一进行行循环
                            {
                                if (MatchTable1[j].itemName.Equals(GcName))
                                {
                                    int a = Convert.ToInt32(MatchTable1[j].itemValue);
                                    if (a < 10)
                                        strGcCode = MatchTable1[j].itemCode.Substring(0, 1) + "0" + MatchTable1[j].itemValue;
                                    else
                                        strGcCode = MatchTable1[j].itemCode.Substring(0, 1) + MatchTable1[j].itemValue;
                                }
                            }
                        }

                        //表示在输入配置表中找不到输入表的物性
                        if (strGcCode == string.Empty)
                            strGcCode = "G67";//UnFound

                        #endregion

                        this._gcCodeandValue[strGcCode][k] += colsDown[i].fValue;//输入表的输入的数据加和

                        if (strGcCode != "G66" && strGcCode != "G67" && strGcCode != "G68")//计算Found的值
                            this._gcCodeandValue["G65"][k] += colsDown[i].fValue; ;//Found
                    }
                    this._gcCodeandValue["ICP"][k] = colsHead[0].fValue;
                    this._gcCodeandValue["ECP"][k] = colsHead[1].fValue;
                    this._gcCodeandValue["G68"][k] = this._gcCodeandValue["G65"][k] + this._gcCodeandValue["G66"][k] + this._gcCodeandValue["G67"][k];
                }

                #endregion

                #region "将转换后的数据存入数据库并在统计表中显示"

                GridOilDataEdit gridOilDataEdit = new GridOilDataEdit(this._gridOilRight, this._parent);

                int iColIndex = this.StaDataColStart + k * 2;//统计表的数据起始列

                var col = this._gridOilRight.Columns[iColIndex];
                iColIndex = col.Visible ? iColIndex : iColIndex + 1;//列隐藏                 

               
                for (int j = 0; j < this._gridOilRight.Rows.Count; j++)//循环行保存和填入数据
                {
                    if (this._gcCodeandValue[this._gridOilRight[this.StaCodeCol, j].Value.ToString()][k] != 0)
                    {
                        gridOilDataEdit.OilDataSupplementPaste(this._gcCodeandValue[this._gridOilRight[this.StaCodeCol, j].Value.ToString()][k].ToString(), iColIndex, j);
                    }
                }

                #endregion
            }//输入表的列循环      
        }
        #endregion

        #region "向宽馏分表中插入数据"
      
        /// <summary>
        /// 向宽馏分表中插入数据
        /// </summary>
        public void UpdataWide(DataGridView dataGridView)
        {
            this.DeleteAllWideValues(dataGridView);//删除宽馏分数据

            List<string> strList = new List<string>();
            strList.Add("ICP");
            strList.Add("ECP");
            strList.Add("G65");
            strList.Add("G66");
            strList.Add("G67");
            strList.Add("G68");

            
            IList<OilTableColEntity> _WideCols = OilTableColBll._OilTableCol.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            IList<OilTableRowEntity> _WideRows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            IList<OilTableRowEntity> _StacRows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GC).ToList();
            
            List<OilTableRowEntity> stacRows = new List<OilTableRowEntity>();
            foreach (OilTableRowEntity temp in _StacRows)//排除统计表的部分物性行
            {
                if (!strList.Contains(temp.itemCode))
                {
                    stacRows.Add(temp);
                }
            }

            List<OilTableRowEntity> wideRows = new List<OilTableRowEntity>();
            foreach (OilTableRowEntity tempStac in stacRows)//找出宽馏分和统计表都有的物性
            {
                var temp = _WideRows.Where(o => o.itemCode == tempStac.itemCode).FirstOrDefault();
                if (temp == null)
                    continue ;
                else
                    wideRows.Add(temp);
            }


            if (wideRows.Count <= 0)
                return;

            
            for (int k = 0; k < this._cols.Count; k++)//输入表的列循环
            {
                float icp = this._gcCodeandValue["ICP"][k];
                float ecp = this._gcCodeandValue["ECP"][k];

                if (icp != 0 && ecp != 0)
                {
                    string wcut = dataGridView.Columns[this.inputDataColStart + k * 2].Tag.ToString();//查找宽馏分对应的的列
                 
                    int iColumnIndex = 0;
                    for (int i = 0; i < _WideCols.Count; i++)//计算列ID
                    {
                        if (_WideCols[i].colCode == wcut)
                        {
                            iColumnIndex = _WideCols[i].ColumnIndex;
                            break;
                        }
                    }

                    #region "保存数据"

                    for (int i = 0; i < wideRows.Count; i++)
                    {
                        if (this._gcCodeandValue.Keys.Contains(wideRows[i].itemCode))
                        {
                            float temp = this._gcCodeandValue[wideRows[i].itemCode][k];
                            if (temp != 0)
                            {
                                GridOilDataEdit WideGridOilDataEdit = new GridOilDataEdit(this._parent.WideGridOil, this._parent);
                                WideGridOilDataEdit.OilDataSupplementPaste(temp.ToString(), iColumnIndex, wideRows[i].RowIndex);
                            }
                        }                    
                    }
                    #endregion
                }
            }  
        }

        #endregion 

        #region "GCNorm标准表"
        /// <summary>
        /// 标准表的代码和6个CUT空列
        /// </summary>
        private Dictionary<string, float[]> NormTable()
        {
            List<OilTableRowEntity> rows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            Dictionary<string, float[]> NormTable = new Dictionary<string, float[]>();
            for (int i = 0; i < rows.Count; i++)
            {
                NormTable.Add(rows[i].itemCode, new float[6]);
            }
            return NormTable;
        }
        /// <summary>
        /// 判断馏分段是否连续，并找出温差最大的馏分段。
        /// 返回的第一个是ECP实体，第二个是ICP实体
        /// </summary>
        /// <returns></returns>
        private List<GCInputEntity> IsMaxContinuseCol()
        {
            #region "条件判断"
            List<GCInputEntity> returnList = new List<GCInputEntity>();

            List<GCInputEntity> DIC = new List<GCInputEntity>();
            //List<GCInputEntity> InputDatas = ItemValueTofVlaue(_InputDatas);

            List<GCInputEntity> ICPGCInputDatas = this._gcInputDatas.Where(o => o.itemName == "ICP").ToList();
            List<GCInputEntity> ECPGCInputDatas = this._gcInputDatas.Where(o => o.itemName == "ECP").ToList();

            if (ICPGCInputDatas == null || ECPGCInputDatas == null)
                return null;

            if (ICPGCInputDatas.Count <= 0 || ECPGCInputDatas.Count <= 0)
                return null;

            var group = this._gcInputDatas.GroupBy(o => o.Col);

            if (group.Count() == 1)
            {
                var ICPGCInput = this._gcInputDatas.Where(o => o.itemName == "ICP").FirstOrDefault();
                var ECPGCInput = this._gcInputDatas.Where(o => o.itemName == "ECP").FirstOrDefault();
                returnList.Add(ICPGCInput);
                returnList.Add(ECPGCInput);
                return returnList;
            }
            #endregion

            #region "馏分段温差判断"

            ECPGCInputDatas.OrderBy(o => o.Col);//升序排序
            ICPGCInputDatas.OrderBy(o => o.Col);//升序排序
            foreach (GCInputEntity ECPData in ECPGCInputDatas)
            {
                var ICPData = ICPGCInputDatas.Where(o => o.Col == ECPData.Col + 1).FirstOrDefault();//
                if (ICPData == null)
                    continue;
                if (ICPData.itemValue == ECPData.itemValue)
                {
                    continue;
                }
                else
                {
                    DIC.Add(ECPData);
                    DIC.Add(ICPData);
                }
            }

            if (DIC.Count % 2 != 0)
                return null;

            if (DIC.Count == 0)
                return null;

            #endregion

            #region "返回温差最大的联系馏分段或单独馏分段"
            float maxECP_ICP = 0;
            List<GCInputEntity> ECP_ICPlist = new List<GCInputEntity>();//存储温差最大的列的ICP和ECP.
            for (int i = 0; i < ICPGCInputDatas.Count; i++)
            {
                if (ECPGCInputDatas[i].fValue - ICPGCInputDatas[i].fValue > maxECP_ICP)
                {
                    maxECP_ICP = ECPGCInputDatas[i].fValue - ICPGCInputDatas[i].fValue;
                    ECP_ICPlist.Clear();
                    ECP_ICPlist.Add(ECPGCInputDatas[i]);
                    ECP_ICPlist.Add(ICPGCInputDatas[i]);
                }
            }


            if (DIC.Count == 2)
            {
                if (DIC[0].fValue - DIC[1].fValue > maxECP_ICP)
                {
                    returnList = DIC;
                }
                else
                {
                    returnList = ECP_ICPlist;
                }
            }
            else if (DIC.Count == 4)
            {
                if ((DIC[0].fValue - DIC[1].fValue) > (DIC[2].fValue - DIC[3].fValue))
                {
                    if ((DIC[0].fValue - DIC[1].fValue) > maxECP_ICP)
                    {
                        returnList.Add(DIC[0]);
                        returnList.Add(DIC[1]);
                    }
                    else
                    {
                        returnList = ECP_ICPlist;
                    }
                }
                else
                {
                    if ((DIC[2].fValue - DIC[3].fValue) > maxECP_ICP)
                    {
                        returnList.Add(DIC[2]);
                        returnList.Add(DIC[3]);
                    }
                    else
                    {
                        returnList = ECP_ICPlist;
                    }
                }
            }
            #endregion

            return returnList;
        }

        /// <summary>
        /// GC数据转换为标准表
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="MatchTable1"></param>
        /// <param name="MatchTable2"></param>
        /// <returns></returns>
        public void GcNormTable()
        {
            #region "判断初始化条件"
            if (MatchTable1 == null || MatchTable2 == null)//判断输入的表格不能为空
                return;

            if (MatchTable1.Count <= 0 || MatchTable2.Count <= 0)
                return;

            if (this._gcInputDatas == null)
                return;
            if (this._gcInputDatas.Count <= 0)
                return;

            DeleteAllNormGridValues();//删除标准表数据

            _gcNormOutput = NormTable();//初始化表格

            List<OilTableRowEntity> normTalbeRow = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            #endregion

            #region "中间数据的处理"

            for (int i = 0; i < this._cols.Count; i++)//输入表的列循环
            {
                List<GCInputEntity> headRowGroup = this._gcInputDatas.Where(o => o.IsHeadValue == true && o.Col == i).ToList();
                var objICP = headRowGroup.Where(o => o.itemName == "ICP").FirstOrDefault();
                var objECP = headRowGroup.Where(o => o.itemName == "ECP").FirstOrDefault();

                if (objICP != null && objECP != null)//判断输入表列的ICP和ECP是否为空
                {
                    #region "头部数据处理"
                    var ICPData = headRowGroup.Where(o => o.itemName == "ICP").FirstOrDefault();
                    if (ICPData != null)
                        this._gcNormOutput["ICP"][i] = ICPData.fValue;
                    //NormGridOilDataEdit.OilDataSupplementPaste(ICPData.itemValue, this.normalOutputDataColStart + i * 2, this.inputRowICP);

                    var ECPData = headRowGroup.Where(o => o.itemName == "ECP").FirstOrDefault();
                    if (ECPData != null)
                        this._gcNormOutput["ECP"][i] = ECPData.fValue;
                    //NormGridOilDataEdit.OilDataSupplementPaste(ECPData.itemValue, this.normalOutputDataColStart + i * 2, this.inputRowECP);

                    var WYData = headRowGroup.Where(o => o.itemName == "WY").FirstOrDefault();
                    if (WYData != null)
                        this._gcNormOutput["WY"][i] = WYData.fValue;
                    //NormGridOilDataEdit.OilDataSupplementPaste(WYData.itemValue, this.normalOutputDataColStart + i * 2, this.inputRowWY);

                    var TWYData = headRowGroup.Where(o => o.itemName == "TWY").FirstOrDefault();
                    if (TWYData != null)
                        this._gcNormOutput["TWY"][i] = TWYData.fValue;
                    //NormGridOilDataEdit.OilDataSupplementPaste(TWYData.itemValue, this.normalOutputDataColStart + i * 2, this.inputRowWY);

                    var VYData = headRowGroup.Where(o => o.itemName == "VY").FirstOrDefault();
                    if (VYData != null)
                        this._gcNormOutput["VY"][i] = VYData.fValue;
                    //NormGridOilDataEdit.OilDataSupplementPaste(VYData.itemValue, this.normalOutputDataColStart + i * 2, this.inputRowWY);

                    var APIData = headRowGroup.Where(o => o.itemName == "API").FirstOrDefault();
                    if (APIData != null)
                        this._gcNormOutput["API"][i] = APIData.fValue;
                    //NormGridOilDataEdit.OilDataSupplementPaste(APIData.itemValue, this.normalOutputDataColStart + i * 2, this.inputRowWY);

                    var D20Data = headRowGroup.Where(o => o.itemName == "D20").FirstOrDefault();
                    if (APIData != null)
                        this._gcNormOutput["D20"][i] = D20Data.fValue;
                    //NormGridOilDataEdit.OilDataSupplementPaste(D20Data.itemValue, this.normalOutputDataColStart + i * 2, this.inputRowWY);
                    #endregion

                    List<GCInputEntity> bodyRowGroup = this._gcInputDatas.Where(o => o.IsHeadValue == false && o.Col == i).ToList();

                    GcNormTableCol(bodyRowGroup, i);//GC数据转换为标准表的半成品表即列WT%无数据
                }
            }

            #endregion

            #region "最后一列的数据处理,判断馏分段是否连续，如果连续找出那个馏分段连续；本表一共有5个Cut列所有最多有2个连续的馏分段"

            List<GCInputEntity> temp = IsMaxContinuseCol();

            if (temp == null)
                return;

            if (temp[0].Col == temp[1].Col)
            {
                GcNormFunctionInterval(temp[0].Col);
            }
            else
            {
                GcNormFunctionContinuse(temp[0].Col, temp[1].Col);
            }

            #endregion

            #region "保存数据"

            GridOilDataEdit NormGridOilDataEdit = new GridOilDataEdit(this._parent.GCLEVEGridOil, this._parent);

            for (int i = 0; i < this._parent.GCLEVEGridOil.RowCount; i++)//行循环
            {
                var row = this._parent.GCLEVEGridOil.Rows[i];

                while (!row.Visible)
                {
                    i = i + 1;//行隐藏
                    row = this._parent.GCLEVEGridOil.Rows[i];
                }

                for (int j = 0; j < 6; j++)//列循环
                {
                    int selcol = j * 2 + this.normalOutputDataColStart - 1;
                    var col = this._parent.GCLEVEGridOil.Columns[selcol];

                    if (col.Visible == false)
                    {
                        selcol = selcol + 1;
                    }
                    if (this._gcNormOutput[normTalbeRow[i].itemCode][j].ToString() == "0")
                        continue;
                   
                    if (j == 5 && i == 0)
                    {
                        List<OilDataEntity> lightDatas = this._parent.Oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Light).ToList();
                        bool Havelight = (lightDatas.Count > 0) ? true : false;//表示轻端不为空
                        if (Havelight)
                        {
                            NormGridOilDataEdit.OilDataSupplementPaste("IBP", selcol, i);
                        }
                        else
                        {
                            NormGridOilDataEdit.OilDataSupplementPaste(this._gcNormOutput[normTalbeRow[i].itemCode][j].ToString(), selcol, i);
                        }
                    }
                    else
                    {
                        NormGridOilDataEdit.OilDataSupplementPaste(this._gcNormOutput[normTalbeRow[i].itemCode][j].ToString(), selcol, i);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 因为在GC转换时输入的列不确定那个列是空，所以添加列定位功能这样可以定位计算那个列。
        /// 输出的是GC标准表的列.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="MatchTable1"></param>
        /// <param name="MatchTable2"></param>
        /// <param name="Output"></param>
        /// <returns></returns>
        private void GcNormTableCol(List<GCInputEntity> bodyRowGroup, int InputDatacol)
        {
            #region "输入判断"

            if (MatchTable1 == null || MatchTable2 == null || bodyRowGroup == null)//判断输入的表格不能为空
                return;//运行不成功

            if (MatchTable1.Count <= 0 || MatchTable2.Count <= 0 || bodyRowGroup.Count <= 0)
                return;//运行不成功

            if (InputDatacol >= this.inputDataColNum || InputDatacol < 0)//输入的数据表最大为5列
                return;//运行不成功

            #endregion

            for (int i = 0; i < MatchTable2.Count; i++)//GC标准表的配置表
            {
                //GCX[i] = MatchTable2[i].itemCode; //取MatchTable2的第一列数据作为标准GC表的X的值,代码列的值
                #region "匹配项目"

                if (MatchTable2[i].colStrD.Contains("T"))//在MatchTable2中的第四列找含T的字符
                { /* 找含T的字符，不进行任何操作 */}
                else if (MatchTable2[i].colStrD.Contains("S"))//在MatchTable2中的第四列找含S的字符/* 找含S的字符*/
                {
                    int strLength = MatchTable2[i].colStrD.Length;
                    string TEMP = MatchTable2[i].colStrD.Substring(0, strLength - 1);//取出除S字符后的字符(即取出S字符前面的字符后复制给TEMP)

                    float SUM = 0;//              
                    for (int j = 0; j < MatchTable1.Count; j++)//在Matchtable1表中的第四列找与TEMP相同字符的行                      
                    {   //在Matchtable1表中的第四列找与TEMP相同字符的行并且在Matchtable1表中的第三列和MatchTable2中的第三列中的数字相同
                        //找到相同字符的行后，取出同行的第1列中名称，放入GCName中                  
                        if ((MatchTable1[j].itemCode == TEMP) && (MatchTable1[j].itemValue == MatchTable2[i].colIntC))
                        {
                            string GCName = MatchTable1[j].itemName;
                            for (int k = 0; k < bodyRowGroup.Count; k++)//有可能找到多个条件相同的数值
                            {                                              //在GC实测数据中找与GCName同名的组分，将对应的y值累计加和
                                if (bodyRowGroup[k].itemName == GCName)     //再输入表中找到对应的物性名称
                                {
                                    SUM += bodyRowGroup[k].fValue;
                                }
                            }
                        }
                    }
                    //GCY[i] = SUM;
                    this._gcNormOutput[MatchTable2[i].itemCode][InputDatacol] = SUM;//将数据列添加到output数据集
                    // NormalInsert(SUM.ToString(), this.normalOutputDataColStart + InputDatacol * 2, i + normalOutputRowStart);
                }
                else /*其它情况(不含S和T的情况)，先在MatchTable1中找英文与GCX[i]相同的行，
                    取出同行的中文名称，在实测数据找名称相同的x[j]，找到后将值赋给标准GC数据的GCY[i]*/
                {
                    string Ename = MatchTable2[i].descript;//取出英文名称
                    string Cname = null;
                    for (int j = 0; j < MatchTable1.Count; j++)
                    {
                        if (MatchTable1[j].descript == Ename)  //MatchTable1的第2列中条件相同时
                        {
                            Cname = MatchTable1[j].itemName;   //获取中文名字
                        }
                    }
                    for (int j = 0; j < bodyRowGroup.Count; j++)//输入表的循环赋值过程
                    {
                        if (Cname == bodyRowGroup[j].itemName)
                        {
                            this._gcNormOutput[MatchTable2[i].itemCode][InputDatacol] = bodyRowGroup[j].fValue;//将数据列添加到output数据集                        
                            // NormalInsert(fValue.ToString(), this.normalOutputDataColStart + InputDatacol * 2, i + normalOutputRowStart);
                        }
                    }

                }

                #endregion
            }

            #region "找出未鉴定的组分Unknown和无对应关系Unfound的组分，统计各含量"

            float found = 0, unfound = 0, unknown = 0; ;

            for (int i = 0; i < bodyRowGroup.Count; i++)//
            {
                if (bodyRowGroup[i].itemName == string.Empty)
                {
                    unknown += bodyRowGroup[i].fValue;//名字不知道的物性
                }
                else if (bodyRowGroup[i].itemName != string.Empty)
                {
                    for (int j = 0; j < MatchTable1.Count; j++)//从MatchTable1中查找匹配的物性
                    {
                        if (bodyRowGroup[i].itemName == MatchTable1[j].itemName)
                        {
                            found += bodyRowGroup[i].fValue;
                        }
                        else if (bodyRowGroup[i].itemName != MatchTable1[j].itemName)
                        {
                            unfound += bodyRowGroup[i].fValue;
                        }
                    }
                }
            }

            float sum = found + unknown + unfound;          //标准吧表输出的总物性值
            this._gcNormOutput["G65"][InputDatacol] = found;   //将数据列添加到output数据集  
            this._gcNormOutput["G66"][InputDatacol] = unknown; //将数据列添加到output数据集 
            this._gcNormOutput["G67"][InputDatacol] = unfound; //将数据列添加到output数据集 
            this._gcNormOutput["G68"][InputDatacol] = sum;     //将数据列添加到output数据集 

            #endregion
        }


        /// <summary>
        /// 方案一
        ///  Start 为连续的起始列号，End为连续的结束列号
        /// </summary>
         /// <param name="startECP"></param>
         /// <param name="endICP"></param>
        private void GcNormFunctionContinuse(int startECP, int endICP)
        {   /*Start 为连续的起始列号，End为连续的结束列号*/

            List <OilDataEntity > lightDatas = this._parent.Oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Light).ToList();
            bool Havelight = (lightDatas.Count > 0) ? true : false;//表示轻端不为空

            if (Havelight)
            {             
                List<OilDataEntity> lightWT = lightDatas.Where(o => o.OilTableCol.colCode == "Cut1" && o.calData != string.Empty).ToList();
                
                #region "轻端表存在"
                if (lightWT.Count > 0)
                {
                    //frmLightDialog lightDialog = new frmLightDialog();
                    //lightDialog.TopMost = true;
                    //lightDialog.Text = "第一窄馏分是否为轻端？";
                    //lightDialog.Show();                                     

                    //if (MessageBox.Show("是否调用轻端数据？", "GC表数据转换", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //{
                    //    OilDataEntity WYOilDataEntity = this._parent.Oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow && o.OilTableCol.colOrder == 1 && o.OilTableRow.itemCode == "WY").FirstOrDefault();
                    //    if (WYOilDataEntity.calData != string.Empty && WYOilDataEntity != null)
                    //    {
                    //        string WYcal = WYOilDataEntity.calData;
                    //        string ICPcal = OilDataSupplement.ICP0Supplement(lightWT, WYcal);
                    //    }
                    //}
                    //else
                    //{

                    //}
                }
                else
                { 
                
                }
                

                float LENDTOTAL = 0;//轻端中除CO2外，所有组成质量的加和为LEND.TOTAL。
                #region "轻端中除CO2外，所有组成质量的加和为LEND.TOTAL"
                if (lightWT != null && lightWT.Count > 0)
                {
                    foreach (var temp in lightWT)
                    {
                        float fvalue = 0;
                        if (float .TryParse ( temp .calData ,out fvalue))
                        {
                            if (temp.OilTableRow.itemCode != "CO2")
                            {
                                LENDTOTAL += fvalue;
                            }
                        }                  
                    }
                }
                #endregion 

                //ICP的值
                this._gcNormOutput["ICP"][5] = this._gcNormOutput["ECP"][startECP];
                //ECP的值
                this._gcNormOutput["ECP"][5] = this._gcNormOutput["ECP"][startECP];

                IList<OilDataEntity> _dsNarrow = this._parent.Oil.OilDatas.Where(d => d.OilTableTypeID == (int)EnumTableType.Narrow).ToList(); //查出窄馏分数据               
                if (_dsNarrow == null)
                    return;
                if (_dsNarrow.Count <= 0)
                    return;

                OilDataEntity oilDataECP = _dsNarrow.Where(o => o.calData == "15" && o.OilTableRow.itemCode == "ECP").ToList().FirstOrDefault();
                if (oilDataECP == null)
                    return;

                OilDataEntity oilDataTWY = _dsNarrow.Where(o => o.OilTableRow.itemCode == "TWY" && o.OilTableCol.colCode == oilDataECP.OilTableCol.colCode).ToList().FirstOrDefault();
                if (oilDataTWY == null)
                    return;

                float LENDWY = oilDataTWY.calData == string.Empty ? 0 : Convert.ToSingle(oilDataTWY.calData);

                for (int i = this.normalOutputRowStart; i < (this._gcNormOutput.Count - 4); i++)//处理G01到G64的数据
                {
                    #region //特殊情况
                    if (this._gcNormOutput["G06"][5].ToString() == "G06")//特殊情况//轻端表的两个列相加 
                    {   //轻端表的两个列相加                        
                        OilDataEntity oilDataG06 = lightDatas.Where(o => o.OilTableRow.itemCode == "G06").ToList().FirstOrDefault();
                        string G06 = (oilDataG06 == null) ? "0" : oilDataG06.labData;//输出轻端表G06行的值
                        OilDataEntity oilDataGX1 = lightDatas.Where(o => o.OilTableRow.itemCode == "GX1").ToList().FirstOrDefault();
                        string GX1 = (oilDataGX1 == null) ? "0" : oilDataGX1.labData;//输出轻端表GX1行的值
                        /*修改轻端表代码为G06行的值并存入轻端表*/
                        G06 = (Convert.ToSingle(G06) + Convert.ToSingle(GX1)).ToString();
                        oilDataG06.calData = G06;//修改检测值
                        var access = new OilDataAccess();
                        if (oilDataG06.ID > 0)
                            access.Update(oilDataG06, oilDataG06.ID.ToString());
                        else
                            oilDataG06.ID = access.Insert(oilDataG06);
                    }
                    #endregion

                    OilDataEntity oilData = lightDatas.Where(o => o.OilTableRow.itemCode == this._normRows[i].itemCode).ToList().FirstOrDefault();
                    float LENDContent = (oilData == null) ? 0 : Convert.ToSingle(oilData.calData);//轻端表中存在的标准表中的物性的值

                    float A = 0;//GC(头).Content(COD) * GC(头).WY   + GC(中).Content(COD) * GC(中).WY + GC(尾).Content(COD) * GC(尾).WY
                    float B = 0;//GC(头).Content(65) * GC(头).WY + GC(中).Content(65) * GC(中).WY + GC(尾).Content(65) * GC(尾).WY 
                    float C = 0;//GC(尾).TWY
                    float result = 0;
                    for (int j = startECP; j <= endICP; j++)
                    {
                        if (this._gcNormOutput[this._normRows[i].itemCode][j].ToString() != string.Empty && this._gcNormOutput["WY"][j].ToString() != string.Empty)
                        {
                            A += this._gcNormOutput[this._normRows[i].itemCode][j] * this._gcNormOutput["WY"][j];
                        }
                        if (this._gcNormOutput["G65"][j].ToString() != string.Empty)
                        {
                            B += this._gcNormOutput["G65"][j] * this._gcNormOutput["WY"][j];
                        }                       
                    }

                    C = this._gcNormOutput["TWY"][endICP];

                    if (this._gcNormOutput[this._normRows[i].itemCode][endICP].ToString() != string.Empty)
                    {
                        result = (LENDWY * LENDContent + A) / (LENDTOTAL + B) *  C;
                    }
                    this._gcNormOutput[this._normRows[i].itemCode][5] = result;
                }

                #endregion
            }
            else //轻端表数据不存在
            {
                #region "轻端表数据不存在"
                //ICP的值
                this._gcNormOutput["ICP"][5] = this._gcNormOutput["ICP"][startECP];
                //ECP的值
                this._gcNormOutput["ECP"][5] = this._gcNormOutput["ECP"][endICP];

                for (int i = normalOutputRowStart; i < (_gcNormOutput.Count - 4); i++)//处理G01到G64的数据
                {
                    float A = 0;
                    float B = 0;
                    float result = 0;
                    for (int j = startECP; j <= endICP; j++)
                    {
                        if (this._gcNormOutput[this._normRows[i].itemCode][j] != null && this._gcNormOutput["WY"][j] != null)
                        {
                            A += this._gcNormOutput[this._normRows[i].itemCode][j] * this._gcNormOutput["WY"][j];
                        }
                        if (this._gcNormOutput["G65"][j] != null)
                        {
                            B += this._gcNormOutput["G65"][j] * this._gcNormOutput["WY"][j];
                        }
                    }

                    if (this._gcNormOutput["TWY"][endICP] != null)
                    {
                        result = A / B * this._gcNormOutput[this._normRows[i].itemCode][endICP];
                    }
                    this._gcNormOutput[this._normRows[i].itemCode][5] = result;

                }
                #endregion
            }
        }
        /// <summary>
        /// 方案二
        /// GC标准表,此函数的输入表为GC标准表的半成品输出表
        /// 找到的ECP-ICP最大的宽馏分GCMAX。
        ///COD: From G01 to G64
        ///TotalGC.Content(COD)=GCMAX.Content(COD)/GCMAX.Content(G65)*GCMAX.TWY
        ///End
        /// </summary>
        /// <param name="Input">半成品输出表</param>
        /// <param name="SelCol">选择的最大的ECP-ICP的列</param>
        /// <param name="Output">成品输出表</param>
        /// <returns></returns>
        private void GcNormFunctionInterval(int SelCol)
        {
            if (SelCol > 5 || SelCol < 0)//输入的数据表最大为5列
                return;

            #region "头部的赋值"
            //ICP的值
            this._gcNormOutput["ICP"][5] = this._gcNormOutput["ICP"][SelCol];
            //ECP的值
            this._gcNormOutput["ECP"][5] = this._gcNormOutput["ECP"][SelCol];
            //WY的值
            this._gcNormOutput["WY"][5] = this._gcNormOutput["WY"][SelCol];
            //TWY的值
            this._gcNormOutput["TWY"][5] = this._gcNormOutput["TWY"][SelCol];
            //VY的值
            this._gcNormOutput["VY"][5] = this._gcNormOutput["VY"][SelCol];
            //D20的值
            this._gcNormOutput["D20"][5] = this._gcNormOutput["D20"][SelCol];
            //API的值
            this._gcNormOutput["API"][5] = this._gcNormOutput["API"][SelCol];
            #endregion

            #region "身体部分的赋值"

            float TWY = this._gcNormOutput["TWY"][SelCol];//必填项            
            float selectG65 = this._gcNormOutput["G65"][SelCol];//必填项

            if (selectG65 != 0)//判断除数不为零
            {
                for (int i = normalOutputRowStart; i < (_gcNormOutput.Count - 4); i++)
                {
                    if (this._gcNormOutput[this._normRows[i].itemCode][SelCol].ToString() != string.Empty)
                    {
                        this._gcNormOutput[this._normRows[i].itemCode][5] = this._gcNormOutput[this._normRows[i].itemCode][SelCol] / selectG65 * TWY;
                    }
                }
            }

            #endregion
        }


        #endregion    

        #region "删除四个表的数据"
        /// <summary>
        /// 删除输入表数据
        /// </summary>
        public void DeleteInputTable(ref DataGridView dataGridView)
        {
            for (int i = 0; i < this._cols.Count; i++)//输入表的列循环
            {
                #region "上侧数据清空"

                for (int j = 0; j < this.inputDataRowStart - 1; j++)
                {
                    dataGridView[this.inputDataColStart + 2 * i, this.inputRowICP].Value = string.Empty;
                }
                #endregion

                #region "下侧数据清空"

                for (int j = this.inputDataRowStart; j < dataGridView.Rows.Count; j++)
                {
                    dataGridView[this.inputDataColStart + i * 2, j].Value = string.Empty;
                    dataGridView[this.inputDataColStart + 1 + i * 2, j].Value = string.Empty;
                }

                #endregion
            }

            GCInputAccess gcInputAccess = new GCInputAccess();
            gcInputAccess.Delete("oilInfoID= " + this._parent .Oil .ID);

        }
        /// <summary>
        /// 删除统计表数据
        /// </summary>
        public void DeleteAllClaAndStaGridValue()
        {
            List<OilTableColEntity> gcCols = this._parent.Oil.OilTableCols.Where(o => o.oilTableTypeID == (int)EnumTableType.GC).ToList();//取出统计表的列           
            List<OilTableRowEntity> gcRows = this._parent.Oil.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.GC).ToList();//取出统计表的行                   
        
            GridOilDataEdit gridOilDataEdit = new GridOilDataEdit(this._gridOilRight, this._parent);

            for (int k = 0; k < gcCols.Count; k++)//输入表的列循环
            {              
                for (int i = 0; i < gcRows.Count; i++)
                {
                    gridOilDataEdit.Delete(gcCols[k].ColumnIndex, gcRows[i].RowIndex);
                }
            }
        }
       
        /// <summary>
        /// 删除标准表数据
        /// </summary>
        public void DeleteAllNormGridValues()
        {
            IList<OilTableRowEntity> _gcLevelRows = this._parent.Oil.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            IList<OilTableColEntity> _gcLevelCols = this._parent.Oil.OilTableCols.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();

            GridOilDataEdit gcLevelOilDataEdit = new GridOilDataEdit(this._parent.GCLEVEGridOil, this._parent);

            for (int i = 0; i < _gcLevelRows.Count; i++)//删除显示的GC标准表数据
            {
                for (int j = 0; j < _gcLevelCols.Count; j++)
                {
                    gcLevelOilDataEdit.Delete(_gcLevelCols[j].ColumnIndex, _gcLevelRows[i].RowIndex);
                }
            }
        }
        /// <summary>
        /// 删除宽馏分表的数据 
        /// </summary>
        private void DeleteAllWideValues(DataGridView dataGridView)//删除宽馏分表的数据 
        {
            #region "查找出宽馏分中对应的行"

            List<string> strList = new List<string>();
            strList.Add("ICP");
            strList.Add("ECP");
            strList.Add("G65");
            strList.Add("G66");
            strList.Add("G67");
            strList.Add("G68");


            IList<OilTableRowEntity> _WideRows = this._parent.Oil.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            IList<OilTableColEntity> _WideCols = this._parent.Oil.OilTableCols.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            IList<OilTableRowEntity> _StacRows = this._parent.Oil.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.GC).ToList();

            List<OilTableRowEntity> stacRows = new List<OilTableRowEntity>();
            foreach (OilTableRowEntity temp in _StacRows)//排除统计表的部分物性行
            {
                if (!strList.Contains(temp.itemCode))
                {
                    stacRows.Add(temp);
                }
            }

            List<OilTableRowEntity> wideRows = new List<OilTableRowEntity>();
            foreach (OilTableRowEntity tempWide in _WideRows)//找出宽馏分和统计表都有的物性
            {
                foreach (OilTableRowEntity tempStac in stacRows)
                {
                    if (tempWide.itemCode == tempStac.itemCode)
                        wideRows.Add(tempWide);
                }
            }

            if (wideRows.Count <= 0)
                return;

            #endregion

            GridOilDataEdit gridOilDataEdit = new GridOilDataEdit(this._parent.WideGridOil, this._parent);

            #region "删除数据"

            for (int k = 0; k < this._cols.Count; k++)//输入表的列循环
            {
                float icp = this._gcCodeandValue["ICP"][k];
                float ecp = this._gcCodeandValue["ECP"][k];

                if (icp != 0 && ecp != 0)
                {
                    string wcut = dataGridView.Columns[this.inputDataColStart + k * 2].Tag.ToString();//查找宽馏分对应的的列              

                    int iColumnIndex = 0;
                    for (int i = 0; i < _WideCols.Count; i++)//计算列ID
                    {
                        if (_WideCols[i].colCode == wcut)
                        {
                            iColumnIndex = _WideCols[i].ColumnIndex;
                            break;
                        }
                    }

                    for (int i = 0; i < wideRows.Count; i++)
                    {
                        gridOilDataEdit.Delete(iColumnIndex, wideRows[i].RowIndex);
                    }
                }
            }

            #endregion
        }
       
        #endregion 
    }
   
}
