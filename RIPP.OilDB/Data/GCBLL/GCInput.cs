using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.GCBLL;
using RIPP.OilDB.Data.DataCheck;
using RIPP.OilDB.Data.DataSupplement;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.UI.GridOil.V2;
using System.Threading;
using RIPP.Lib;
using System.Windows.Forms;

namespace RIPP.OilDB.Data.GCBLL
{
    public static class GlobalLightDialog
    {
        public static DialogResult YesNo = DialogResult.Yes;
        /// <summary>
        /// 轻端的质量收率
        /// </summary>
        public static float LightWY = 0;
    }

    public class GCInput
    {
        #region "私有变量"
        /// <summary>
        /// GC输入表
        /// </summary>
        private GCGridOilView _gridOil = null;
        /// <summary>
        /// GC统计表
        /// </summary>
        private GridOilViewA _GCGridOil = null;
        /// <summary>
        /// GC标准表
        /// </summary>
        private GridOilViewA _GCLevelGridOil = null;
        /// <summary>
        /// 轻端表
        /// </summary>
        private GridOilViewA _LightGridOil = null;
        /// <summary>
        /// 窄馏分表
        /// </summary>
        private GridOilViewA _NarrowGridOil = null;
        /// <summary>
        /// GC输入表的列
        /// </summary>
        List<OilTableColEntity> _cols = null;
        /// <summary>
        /// GC表的数据
        /// </summary>
        List<OilDataEntity> _datas = new List<OilDataEntity> ();
        /// <summary>
        /// GC表的头部数据
        /// </summary>
        List<OilDataEntity> _headDatas = new List<OilDataEntity>();
        /// <summary>
        /// GC表的输入数据
        /// </summary>
        List<OilDataEntity> _inputDatas = new List<OilDataEntity> ();
        
        /// <summary>
        /// GC统计表的代码和数据
        /// </summary>
        private Dictionary<string, float []> _gcCodeandValue = new Dictionary<string, float []>();
        /// <summary>
        /// GC标准表的代码和数据
        /// </summary>
        private Dictionary<string, float[]> _gcLevelOutput = new Dictionary<string, float[]>();
        /// <summary>
        /// 轻端数据含量
        /// </summary>
        private Dictionary<string, float> _LightContent = new Dictionary<string, float>();
        /// <summary>
        /// GC输入表的最大列
        /// </summary>
        private int _Max = 0;
        /// <summary>
        /// 头部的数据行数 _headDataNum = 8 
        /// </summary>
        private int _headDataNum = 8;
        /// <summary>
        /// GC标准表有多少列 _COlNUM = 11
        /// </summary>
        private int _COlNUM = 11;
        /// <summary>
        /// GCLevel的G01数据列的起始行
        /// </summary>
        private int _GCLevelG01Row = 7;//标准GC输出表的数据列G01-G68开时的行的位置
        /// <summary>
        /// GCLevel的G68数据列的起始行
        /// </summary>
        private int _GCLevelG64Row = 73;//标准GC输出表的数据列G01-G68开时的行的位置
        /// <summary>
        /// GCLeve的行集合
        /// </summary>
        List<OilTableRowEntity> _GCLevelG01_G64Rows = null;//标准GC输出表的数据列G01-G64标准的行集合
        /// <summary>
        /// 读出配置表1
        /// </summary>
        List<GCMatch1Entity> _MatchTable1 = null;//读出配置表1
        /// <summary>
        /// 读出配置表2
        /// </summary>
        List<GCMatch2Entity> _MatchTable2 = null;//读出配置表2
        /// <summary>
        /// 代码列G00-G64
        /// </summary>
        private List<string> _G00_G64List = new List<string>();
        #endregion

        #region "GC构造函数"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GCInputGridOil"></param>
        /// <param name="GCGridOil"></param>
        /// <param name="GCLevelGridOil"></param>
        public GCInput(GridOilViewA LightGridOil, GCGridOilView GCInputGridOil, GridOilViewA GCGridOil, GridOilViewA GCLevelGridOil, GridOilViewA NarrowGridOil)
        {
            this._gridOil = GCInputGridOil;
            this._GCGridOil = GCGridOil;
            this._GCLevelGridOil = GCLevelGridOil;
            this._LightGridOil = LightGridOil;
            this._NarrowGridOil = NarrowGridOil;
            
            this._cols = OilTableColBll._OilTableCol.Where(o => o.oilTableTypeID == (int)EnumTableType.GCInput).ToList();
            InputHeadCheck();
            GCMatch1Access MatchTable1Access = new GCMatch1Access();
            this._MatchTable1 = MatchTable1Access.Get("1=1").ToList();

            GCMatch2Access MatchTable2Access = new GCMatch2Access();
            this._MatchTable2 = MatchTable2Access.Get("1=1").ToList();
            List<OilTableColEntity> cols = OilTableColBll._OilTableCol.Where(o=>o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            

            this._GCLevelG01_G64Rows = OilTableRowBll._OilTableRow.Where(s => s.oilTableTypeID == (int)EnumTableType.GCLevel && s.itemCode.Contains("G")).ToList();

            InitLight();
        }
        /// <summary>
        /// 初始化轻端数据
        /// </summary>
        private void InitLight()
        {
            List<OilDataEntity> LightDatalist = this._LightGridOil.GetAllData();
            if (LightDatalist != null)
            {
                List<OilDataEntity> LightWYDatalist = LightDatalist.Where(o=>o.ColumnIndex == 0 && o.calData != string.Empty).ToList();
                for (int index = 0; index < LightWYDatalist.Count; index++)
                {
                    OilDataEntity oilData = LightWYDatalist[index];                   
                    string strCal = oilData == null ? string.Empty : oilData.calData;
                    string itemCode = oilData.OilTableRow.itemCode;
                    float tempCal = 0;
                    if (strCal != string.Empty && float.TryParse (strCal ,out tempCal))
                    {
                        if (!this._LightContent.Keys.Contains (itemCode))
                            this._LightContent.Add(itemCode, tempCal);
                    }
                }
            }             
        }
        #endregion 

        #region "私有函数"
        /// <summary>
        /// 判断输入表的头部是否含有值
        /// </summary>
        /// <returns></returns>
        private bool InputHeadCheck()
        {
            bool bRetrun = false;//返回值用来判断输入表头是否用来正确。
            List<OilDataEntity> allDatalist = this._gridOil.GetAllData();

            #region "表格是否含有数据的整体判断"
            if (allDatalist.Count <= 0)//当前表格不存在值
                return bRetrun;


            var tempallDataEnum = from item in allDatalist
                                  where !string.IsNullOrWhiteSpace(item.calData) || !string.IsNullOrWhiteSpace(item.labData)
                                  select item;
            this._datas = tempallDataEnum.ToList().Where (o=>o.RowIndex != this._headDataNum).ToList();
            if (this._datas.Count <= 0)//当前表格不存在值
                return bRetrun;

            this._inputDatas = this._datas.Where (o=>o.RowIndex > 8).ToList ();
            #endregion 

            #region "头部是否含有数据判断"
            List<OilDataEntity> gcInputHeadDataList = allDatalist.Where(o => o.RowIndex < 8).ToList();//取出头部数据

            if (gcInputHeadDataList == null)
                bRetrun = false;//头部不存在数据
            if (gcInputHeadDataList.Count <= 0)
                bRetrun = false;//头部不存在数据
            this._headDatas = this._datas.Where(o => o.RowIndex < 8).ToList();//取出头部数据
            #endregion 
            
            int count = 0;//判断含有多少个头部完整的馏分段
            foreach (OilTableColEntity col in this._cols)
            {
                List<OilDataEntity> headDatas = this._headDatas.Where(o => o.OilTableCol.colCode == col.colCode).ToList();
                if (headDatas == null)
                    continue;
                if (headDatas.Count == 8)
                    count++;
            }
            this._Max = count;
            if (count == 0)//头部不存在完整的馏分段          
                bRetrun = false;
            else
                bRetrun = true;

            return bRetrun;
        }
        /// <summary>
        /// 输入数据归一
        /// </summary>
        /// <returns></returns>
        private bool  InputDataGuiYi()
        {
            bool ready = true ;

            for (int colIndex = 0; colIndex < this._Max; colIndex++)
            {
                #region "每一列的数据判断"

                List<OilDataEntity> InputColDatas = this._datas.Where(o => o.OilTableCol.colCode == this._cols[colIndex].colCode).ToList();//找出当列数据(包括列头)
                if (InputColDatas == null)
                    continue;
                if (InputColDatas.Count <= 0)
                    continue;

                List<OilDataEntity> colsHead = InputColDatas.Where(o => o.RowIndex < 8).ToList();//找出上侧数据部分
                if (colsHead == null)
                    continue;
                if (colsHead.Count == 0)//输入表的上侧数据为空
                    continue;


                List<OilDataEntity> colsDown = InputColDatas.Where(o => o.RowIndex > 8).ToList();//找出下侧数据部分
                if (colsDown == null)
                    continue;

                #endregion

                #region "每一列的数据归一"
                Dictionary<string, float> DICDown = new Dictionary<string, float>();//暂存每一列的输入数据
                float sum = 0;//每列数据加和
                for (int i = 0; i < colsDown.Count; i++)//输入行
                {
                    float fvalue = 0;
                    if (float.TryParse(colsDown[i].calData, out fvalue))
                    {
                        DICDown[colsDown[i].OilTableRow.itemCode]= fvalue;
                        sum += fvalue;//每列数据加和
                    }
                }

                if (Math.Abs(100 - sum) >= 1)//数据严格缺失 
                {
                    if (MessageBox.Show(this._cols[colIndex].colName + "馏分的GC数据严重不归一！建议审查数据或删除此列。", "归一检测", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ready = false;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (Math.Abs(100 - sum) < 1 && Math.Abs(100 - sum) >= 0.1)//归一数据
                {
                    if (MessageBox.Show(this._cols[colIndex].colName + "馏分的GC数据不归一！是否做归一校正?", "归一检测", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        List<string> Keys = DICDown.Keys.ToList();
                        foreach (string key in Keys)
                        {
                            float tempCell = DICDown[key] / sum * 100;
                            DICDown[key] = tempCell;
                           this._gridOil.SetData(key, colIndex, tempCell.ToString());
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                #endregion
            }

            return ready;
        }       

        #region "GC统计表"
        /// <summary>
        /// 统计表的代码和10个CUT空列
        /// </summary>
        private Dictionary<string, float []> GCCodeAndValue()
        {
            List<OilTableRowEntity> rows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GC).ToList();
            Dictionary<string, float[]> GcClassifi = new Dictionary<string, float[]>();
            for (int i = 0; i < rows.Count; i++)
            {
                GcClassifi.Add(rows[i].itemCode, new float[10] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN});
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
            this._gcCodeandValue = GCCodeAndValue();//输出表的格式
           
            for (int colIndex = 0; colIndex < this._Max; colIndex++)//输入表的列循环
            {
                #region "每一列的数据判断"
                List<OilDataEntity> InputColDatas = this._datas.Where(o => o.ColumnIndex == colIndex).ToList();//找出当列数据(包括列头)
                List<OilDataEntity> colsHead = InputColDatas.Where(o => o.RowIndex < 8).ToList();//找出上侧数据部分
                List<OilDataEntity> colsDown = InputColDatas.Where(o => o.RowIndex > 8).ToList();//找出下侧数据部分
                #endregion

                #region "输入表和统计表数据转换算法"
                if (colsHead.Count == this._headDataNum)//表头不为空
                {
                    for (int rowIndex = 0; rowIndex < colsDown.Count; rowIndex++)//循环每一数据行
                    {
                        string GcName = colsDown[rowIndex].labData;////输入表的输入的物性名称

                        #region "查找strGcCode"

                        string strGcCode = string.Empty;//

                        if (GcName == string.Empty)
                        {
                            strGcCode = "G66";//Unknow
                        }
                        else if (GcName != string.Empty)//找到名字输入物性不为空的名字
                        {
                            for (int j = 0; j < this._MatchTable1.Count; j++)//对匹配表一进行行循环
                            {
                                if (this._MatchTable1[j].itemName.Equals(GcName))
                                {
                                    int a = Convert.ToInt32(_MatchTable1[j].itemValue);
                                    if (a < 10)
                                        strGcCode = this._MatchTable1[j].itemCode.Substring(0, 1) + "0" + this._MatchTable1[j].itemValue;
                                    else
                                        strGcCode = this._MatchTable1[j].itemCode.Substring(0, 1) + this._MatchTable1[j].itemValue;
                                }
                            }
                        }

                        //表示在输入配置表中找不到输入表的物性
                        if (strGcCode == string.Empty)
                            strGcCode = "G67";//UnFound

                        #endregion

                        float tempCellData = 0;
                        if (float.TryParse(colsDown[rowIndex].calData, out tempCellData))
                        {
                            if (strGcCode == "A10")
                                strGcCode = "10A";
                            try
                            {
                                if (!this._gcCodeandValue[strGcCode][colIndex].Equals(float.NaN) )
                                    this._gcCodeandValue[strGcCode][colIndex] += tempCellData;//输入表的输入的数据加和
                                else if (this._gcCodeandValue[strGcCode][colIndex].Equals(float.NaN))
                                {
                                    this._gcCodeandValue[strGcCode][colIndex] = 0;
                                    this._gcCodeandValue[strGcCode][colIndex] += tempCellData;//输入表的输入的数据加和
                                }                               
                            }
                            catch (Exception ex)
                            {
                                Log.Error("统计表转换:" +ex.ToString () );
                            }
                            if (strGcCode != "G66" && strGcCode != "G67" && strGcCode != "G68")//计算Found的值
                            {
                                if (this._gcCodeandValue["G65"][colIndex].Equals(float.NaN))
                                {
                                    this._gcCodeandValue["G65"][colIndex] = 0;
                                    this._gcCodeandValue["G65"][colIndex] += tempCellData; ;//Found
                                }
                                else if (!this._gcCodeandValue["G65"][colIndex].Equals(float.NaN))
                                    this._gcCodeandValue["G65"][colIndex] += tempCellData; ;//Found
                            }
                        }                      
                    }
                    this._gcCodeandValue["ICP"][colIndex] = Convert.ToSingle(colsHead[0].labData);
                    this._gcCodeandValue["ECP"][colIndex] = Convert.ToSingle(colsHead[1].labData);
                    float SUM = 0;
                    if (!this._gcCodeandValue["G65"][colIndex].Equals(float.NaN))
                        SUM += this._gcCodeandValue["G65"][colIndex];
                    if (!this._gcCodeandValue["G66"][colIndex].Equals(float.NaN))
                        SUM += this._gcCodeandValue["G66"][colIndex];
                    if (!this._gcCodeandValue["G67"][colIndex].Equals(float.NaN))
                        SUM += this._gcCodeandValue["G67"][colIndex];

                    this._gcCodeandValue["G68"][colIndex] = SUM;
                }

                #endregion

                #region "将转换后的数据存入数据库并在统计表中显示"
                foreach (string key in this._gcCodeandValue.Keys)
                {                   
                    float [] row = this._gcCodeandValue[key] ;
                    for (int index = 0; index < row.Length; index++)
                    {
                        if (row[index].Equals(float.NaN))
                            this._GCGridOil.SetData(key, index, "");
                        else
                            this._GCGridOil.SetData(key, index, row[index].ToString());
                    }                   
                }
                #endregion
            }    
        }
        #endregion

        #region "GC数据转换为标准表"
        /// <summary>
        /// 标准表的代码和6个CUT空列
        /// </summary>
        private Dictionary<string, float[]> NormTable()
        {
            List<OilTableRowEntity> rows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            Dictionary<string, float[]> NormTable = new Dictionary<string, float[]>();
            for (int i = 0; i < rows.Count; i++)
            {
                NormTable.Add(rows[i].itemCode, new float[11] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN ,float.NaN});
            }
            return NormTable;
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
        private void GcNormTableCol(List<OilDataEntity> bodyRowGroup, int InputDatacol)
        {
            #region "输入判断"

            if (this._MatchTable1 == null || this._MatchTable2 == null || bodyRowGroup == null)//判断输入的表格不能为空
                return;//运行不成功

            if (this._MatchTable1.Count <= 0 || this._MatchTable2.Count <= 0 || bodyRowGroup.Count <= 0)
                return;//运行不成功

            if (InputDatacol >= this._Max  || InputDatacol < 0)//输入的数据表最大为5列
                return;//运行不成功

            #endregion

            for (int i = 0; i < this._MatchTable2.Count; i++)//GC标准表的配置表
            {
                string strG = this._MatchTable2[i].itemCode;//取MatchTable2的第一列数据作为标准GC表的X的值,代码列的值
                
                #region "匹配项目"
                if (this._MatchTable2[i].colStrD.Contains("T"))//在MatchTable2中的第四列找含T的字符
                { /* 找含T的字符，不进行任何操作 */}
                else if (_MatchTable2[i].colStrD.Contains("S"))//在MatchTable2中的第四列找含S的字符/* 找含S的字符*/
                {
                    int strLength = this._MatchTable2[i].colStrD.Length;
                    string TEMP = this._MatchTable2[i].colStrD.Substring(0, strLength - 1);//取出除S字符后的字符(即取出S字符前面的字符后复制给TEMP)

                    float SUM = 0;//              
                    for (int j = 0; j < this._MatchTable1.Count; j++)//在Matchtable1表中的第四列找与TEMP相同字符的行                      
                    {   //在Matchtable1表中的第四列找与TEMP相同字符的行并且在Matchtable1表中的第三列和MatchTable2中的第三列中的数字相同
                        //找到相同字符的行后，取出同行的第1列中名称，放入GCName中                  
                        if ((this._MatchTable1[j].itemCode == TEMP) && (this._MatchTable1[j].itemValue == this._MatchTable2[i].colIntC))
                        {
                            string GCName = this._MatchTable1[j].itemName;
                            for (int k = 0; k < bodyRowGroup.Count; k++)//有可能找到多个条件相同的数值
                            {                                              //在GC实测数据中找与GCName同名的组分，将对应的y值累计加和
                                if (bodyRowGroup[k].labData  == GCName)     //再输入表中找到对应的物性名称
                                {
                                    string strCal = bodyRowGroup[k].calData;
                                    float fCal = 0;
                                    if (float.TryParse(strCal , out fCal))
                                        SUM += fCal;
                                }
                            }
                        }
                    }
                    //GCY[i] = SUM;
                    this._gcLevelOutput[strG][InputDatacol] = SUM;//将数据列添加到output数据集                  
                }
                else /*其它情况(不含S和T的情况)，先在MatchTable1中找英文与GCX[i]相同的行，
                    取出同行的中文名称，在实测数据找名称相同的x[j]，找到后将值赋给标准GC数据的GCY[i]*/
                {
                    string Ename = this._MatchTable2[i].descript;//取出英文名称
                    string Cname = null;
                    for (int j = 0; j < this._MatchTable1.Count; j++)
                    {
                        if (this._MatchTable1[j].descript == Ename)  //MatchTable1的第2列中条件相同时
                        {
                            Cname = this._MatchTable1[j].itemName;   //获取中文名字
                        }
                    }
                    for (int j = 0; j < bodyRowGroup.Count; j++)//输入表的循环赋值过程
                    {
                        if (Cname == bodyRowGroup[j].labData)
                        {
                            string strCal = bodyRowGroup[j].calData ;
                            float fCal = 0;
                            if(float.TryParse (strCal , out fCal))
                            {
                                this._gcLevelOutput[strG][InputDatacol] = fCal;//将数据列添加到output数据集    
                            }                                                
                        }
                    }
                }
                #endregion
            }

            #region "找出未鉴定的组分Unknown和无对应关系Unfound的组分，统计各含量"
            float found = 0, unfound = 0, unknown = 0; ;

            foreach (string str in this._G00_G64List)
            {
                if (this._gcLevelOutput.Keys.Contains(str))
                {
                    float temp = this._gcLevelOutput[str][InputDatacol];
                    if (!temp.Equals(float.NaN))
                        found += this._gcLevelOutput[str][InputDatacol];
                }
            }
         
            float sum = found + unknown + unfound;          //标准吧表输出的总物性值
            this._gcLevelOutput["G65"][InputDatacol] = found;   //将数据列添加到output数据集  
            this._gcLevelOutput["G66"][InputDatacol] = unknown; //将数据列添加到output数据集 
            this._gcLevelOutput["G67"][InputDatacol] = unfound; //将数据列添加到output数据集 
            this._gcLevelOutput["G68"][InputDatacol] = sum;     //将数据列添加到output数据集 

            #endregion
        }
        /// <summary>
        /// 判断馏分段是否连续，并找出温差最大的馏分段。
        /// 返回的第一个是ICP实体列表 
        /// </summary>
        /// <returns></returns>
        private List<OilDataEntity> IsMaxContinuseCol()
        {
            Dictionary<float, List<OilDataEntity>> DIC = new Dictionary<float, List<OilDataEntity>>();
            List<OilDataEntity> returnList = new List<OilDataEntity>();
            #region "条件判断"                  
            List<OilDataEntity> ICPGCInputDatas = this._datas.Where(o => o.OilTableRow.itemCode == "ICP").ToList();
            List<OilDataEntity> ECPGCInputDatas = this._datas.Where(o => o.OilTableRow.itemCode == "ECP").ToList();

            if (ICPGCInputDatas == null || ECPGCInputDatas == null)
                return returnList;

            if (ICPGCInputDatas.Count <= 0 || ECPGCInputDatas.Count <= 0)
                return returnList;
            #endregion         
            
            for (int colIndex = 0; colIndex < this._Max; colIndex++)
            {
                #region "取出已经存入字典的数据"
                List<OilDataEntity> dataList = new List<OilDataEntity>();
                foreach (float key in DIC.Keys)
                {
                    List<OilDataEntity> list = DIC[key];
                    dataList.AddRange(list);
                }
                #endregion 

                List<OilDataEntity> tempList = new List<OilDataEntity>();
              
                OilDataEntity ICPData = ICPGCInputDatas.Where(o => o.ColumnIndex == colIndex).FirstOrDefault();
                OilDataEntity ECPData = ECPGCInputDatas.Where(o => o.ColumnIndex == colIndex).FirstOrDefault();
                if (dataList.Contains(ICPData) && dataList.Contains(ECPData))
                    continue;

                float MaxTemp = 0; //馏分段的温差

                if (ICPData != null && ECPData != null)
                {
                    float fICP = 0, fECP = 0;
                    if (float.TryParse(ICPData.calData, out fICP) && float.TryParse(ECPData.calData, out fECP))
                    {
                        MaxTemp += fECP - fICP;
                        tempList.Add(ICPData);//添加第一个点
                        tempList.Add(ECPData);//添加第一个点
                    }
                }      

                OilDataEntity tempICPData = null;
                OilDataEntity tempECPData = null;
               
                for (int Index = 0; Index < this._Max; Index++)
                {
                    tempICPData = ICPGCInputDatas.Where(o => o.calData == ECPData.calData).FirstOrDefault();
                    if (tempICPData != null)
                        tempECPData = ECPGCInputDatas.Where(o => o.ColumnIndex == tempICPData.ColumnIndex).FirstOrDefault();

                    if (tempECPData != null && tempICPData != null)
                    {
                        float fICP = 0, fECP = 0;
                        if (float.TryParse(tempICPData.calData, out fICP) && float.TryParse(tempECPData.calData, out fECP))
                        {
                            MaxTemp += fECP - fICP;
                            tempList.Add(tempICPData);//添加第一个点
                            tempList.Add(tempECPData);//添加第一个点
                            ECPData = tempECPData;
                        }
                    }                                   
                }
                if (!DIC.Keys.Contains(MaxTemp))
                    DIC.Add(MaxTemp, tempList);
                else
                {
                    List<OilDataEntity> tempValueList = DIC[MaxTemp];//判断列的多少
                    if (tempValueList.Count > tempList.Count)
                    {
                        DIC.Remove(MaxTemp);
                        DIC.Add(MaxTemp, tempList);
                    }
                }
            }

            Dictionary<float, List<OilDataEntity>> tempDIC = new Dictionary<float, List<OilDataEntity>>();
            float tempKey = 0;
            foreach (float key in DIC.Keys)
            {
                if (key > tempKey)
                    tempKey = key;
            }

            returnList = DIC[tempKey].Where(o=>o.OilTableRow.itemCode == "ICP").ToList();

            return returnList;
        }

        /// <summary>
        /// 方案一
        ///  Start 为连续的起始列号，End为连续的结束列号
        /// </summary>
        /// <param name="startECP"></param>
        /// <param name="endICP"></param>
        private void GcNormFunctionContinuse(List<OilDataEntity> ICPList)
        {   /*Start 为连续的起始列号，End为连续的结束列号*/

            Dictionary<float, int> Col_ICPList = new Dictionary<float, int>();
            foreach (OilDataEntity item in ICPList)
            {
                float ICP = 0;
                if (float.TryParse(item.calData, out ICP))
                {
                    Col_ICPList.Add(ICP,item.ColumnIndex);
                }          
            }
            Dictionary<float, int> Col_ICPDIC = Col_ICPList.OrderBy(o => o.Key).ToDictionary(o=>o.Key,o=>o.Value);
            List<int> colList = Col_ICPDIC.Values.ToList();
            
            if (MessageBox.Show("是否调用轻端数据？", "GC表数据转换", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (this._LightContent.Count > 0)
                {
                    #region "轻端表存在"
                    float fLightWY = 0; //轻端表的WY
                    float LightTotal = 0;//轻端表的加和

                    #region "轻端表的数据处理"
                    frmLightDialog lightDialog = new frmLightDialog();
                    lightDialog.init(this._NarrowGridOil);
                    lightDialog.TopMost = true;
                    lightDialog.Text = "第一窄馏分是否为轻端？";
                    lightDialog.ShowDialog();

                    if (lightDialog.DialogResult == DialogResult.OK)
                        fLightWY = GlobalLightDialog.LightWY;
                    lightDialog.Close();

                    string strICP0 = DataSupplement.OilDataSupplement.ICP0Supplement(this._LightGridOil, fLightWY);

                    foreach (string key in this._LightContent.Keys)
                    {
                        if (key != "CO2")
                            LightTotal += this._LightContent[key];//轻端中除CO2外，所有组成质量的加和为LEND.TOTAL。
                    }
                    #endregion

                    #region"头部处理"
                    float ICP0 = 0;
                    if (strICP0 != string.Empty && float.TryParse(strICP0, out ICP0))
                        this._gcLevelOutput["ICP"][this._COlNUM - 1] = ICP0;//ICP的值
                    this._gcLevelOutput["ECP"][this._COlNUM - 1] = this._gcLevelOutput["ECP"][colList[colList.Count - 1]];//ECP的值
                    this._gcLevelOutput["WY"][this._COlNUM - 1] = this._gcLevelOutput["TWY"][colList[colList.Count - 1]];//WY的值 
                    #endregion

                    #region "身体部分的赋值"
                    float SUMFOUND = 0;
                    foreach (string strG in this._G00_G64List)
                    {
                        if (strG == "G06")
                        {
                            if (this._LightContent.Keys.Contains("GX1") && this._LightContent.Keys.Contains(strG))
                                this._LightContent[strG] += this._LightContent["GX1"];
                        }

                        float A = 0;//GC(头).Content(COD) * GC(头).WY   + GC(中).Content(COD) * GC(中).WY + GC(尾).Content(COD) * GC(尾).WY
                        float B = 0;//GC(头).Content(65) * GC(头).WY + GC(中).Content(65) * GC(中).WY + GC(尾).Content(65) * GC(尾).WY 
                        float C = 0;//GC(尾).TWY
                        foreach (int j in colList)
                        {
                            if (!this._gcLevelOutput[strG][j].Equals(float.NaN) && !this._gcLevelOutput["WY"][j].Equals(float.NaN))
                            {
                                A += this._gcLevelOutput[strG][j] * this._gcLevelOutput["WY"][j];
                            }
                            if (!this._gcLevelOutput["G65"][j].ToString().Equals(float.NaN) && !this._gcLevelOutput["WY"][j].Equals(float.NaN))
                            {
                                B += this._gcLevelOutput["G65"][j] * this._gcLevelOutput["WY"][j];
                            }
                        }

                        if (!this._gcLevelOutput["TWY"][colList[colList.Count - 1]].Equals(float.NaN))
                            C = this._gcLevelOutput["TWY"][colList[colList.Count - 1]];

                        if (this._LightContent.Keys.Contains(strG))
                            this._gcLevelOutput[strG][this._COlNUM -1] = (this._LightContent[strG] * fLightWY + A) / (fLightWY * LightTotal + B) * C;
                        else
                            this._gcLevelOutput[strG][this._COlNUM -1] = (A) / (fLightWY * LightTotal + B) * C;

                        if (!this._gcLevelOutput[strG][this._COlNUM - 1].Equals(float.NaN))
                            SUMFOUND += this._gcLevelOutput[strG][this._COlNUM - 1];
                    }
                    this._gcLevelOutput["G65"][this._COlNUM - 1] = SUMFOUND;
                    #endregion

                    #endregion
                }
                else
                {
                    #region"头部处理"
                    //this._gcLevelOutput["ICP"][this._COlNUM - 1] = this._gcLevelOutput["ICP"][colList[0]];//ICP的值
                    this._gcLevelOutput["ICP"][this._COlNUM - 1] = -50;//ICP的值
                    this._gcLevelOutput["ECP"][this._COlNUM - 1] = this._gcLevelOutput["ECP"][colList[colList.Count - 1]]; //ECP的值
                    this._gcLevelOutput["WY"][this._COlNUM - 1] = this._gcLevelOutput["TWY"][colList[colList.Count - 1]];  //WY的值             
                    #endregion

                    #region "身体部分的赋值"
                    float SUMFOUND = 0;
                    foreach (string strG in this._G00_G64List)
                    {
                        float A = 0;//GC(头).Content(COD) * GC(头).WY   + GC(中).Content(COD) * GC(中).WY + GC(尾).Content(COD) * GC(尾).WY
                        float B = 0;//GC(头).Content(65) * GC(头).WY + GC(中).Content(65) * GC(中).WY + GC(尾).Content(65) * GC(尾).WY 
                        float C = 0;//GC(尾).TWY

                        foreach (int j in colList)
                        {
                            A += this._gcLevelOutput[strG][j] * this._gcLevelOutput["WY"][j];
                            B += this._gcLevelOutput["G65"][j] * this._gcLevelOutput["WY"][j];
                        }

                        C = this._gcLevelOutput["TWY"][colList[colList.Count - 1]];

                        this._gcLevelOutput[strG][this._COlNUM - 1] = A / B * C;
                        if (!this._gcLevelOutput[strG][this._COlNUM - 1].Equals(float.NaN))
                            SUMFOUND += this._gcLevelOutput[strG][this._COlNUM - 1];
                    }

                    this._gcLevelOutput["G65"][this._COlNUM - 1] = SUMFOUND;
                    #endregion                            
                }                
            }
            else//不调用轻端表数据 
            {
                #region"头部处理"
                this._gcLevelOutput["ICP"][this._COlNUM - 1] = this._gcLevelOutput["ICP"][colList[0]];//ICP的值
                this._gcLevelOutput["ECP"][this._COlNUM - 1] = this._gcLevelOutput["ECP"][colList[colList.Count -1]]; //ECP的值
                this._gcLevelOutput["WY"][this._COlNUM - 1] = this._gcLevelOutput["TWY"][colList[colList.Count - 1]];  //WY的值             
                #endregion 

                #region "身体部分的赋值"
                float SUMFOUND = 0;               
                foreach (string strG in this._G00_G64List)
                {
                    float A = 0;//GC(头).Content(COD) * GC(头).WY   + GC(中).Content(COD) * GC(中).WY + GC(尾).Content(COD) * GC(尾).WY
                    float B = 0;//GC(头).Content(65) * GC(头).WY + GC(中).Content(65) * GC(中).WY + GC(尾).Content(65) * GC(尾).WY 
                    float C = 0;//GC(尾).TWY

                    foreach (int j in colList)
                    {
                        A += this._gcLevelOutput[strG][j] * this._gcLevelOutput["WY"][j];
                        B += this._gcLevelOutput["G65"][j] * this._gcLevelOutput["WY"][j];                           
                    }

                    C = this._gcLevelOutput["TWY"][colList[colList.Count - 1]];

                    this._gcLevelOutput[strG][this._COlNUM - 1] = A / B * C;
                    if (!this._gcLevelOutput[strG][this._COlNUM - 1].Equals(float.NaN))
                        SUMFOUND += this._gcLevelOutput[strG][this._COlNUM - 1];                  
                }

                this._gcLevelOutput["G65"][this._COlNUM - 1] = SUMFOUND;
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
            #region "确定循环的行"
            if (SelCol > this._COlNUM || SelCol < 0)//输入的数据表最大为11列
                return;
            #endregion 

            if (MessageBox.Show("是否调用轻端数据？", "GC表数据转换", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {               
                if (this._LightContent.Count > 0)
                {
                    float LightWY = 0;
                    #region "对话框"                  
                    frmLightDialog lightDialog = new frmLightDialog();
                    lightDialog.init(this._NarrowGridOil);
                    lightDialog.TopMost = true;
                    lightDialog.Text = "第一窄馏分是否为轻端？";
                    lightDialog.ShowDialog();

                    if (lightDialog.DialogResult == DialogResult.OK)
                        LightWY = GlobalLightDialog.LightWY;
                    lightDialog.Close();
                    #endregion 

                    #region "头部的赋值"
                    OilDataEntity ICPOilData = this._NarrowGridOil.GetDataByRowItemCodeColumnIndex("ICP", 0);
                    string strICP = ICPOilData == null ? string.Empty : ICPOilData.calData;
                    float ICP = 0;
                    if (strICP != string.Empty && float.TryParse(strICP, out ICP))
                        this._gcLevelOutput["ICP"][this._COlNUM - 1] = ICP;//ICP的值                           
                    this._gcLevelOutput["ECP"][this._COlNUM - 1] = this._gcLevelOutput["ECP"][SelCol];//ECP的值
                    this._gcLevelOutput["WY"][this._COlNUM - 1] = this._gcLevelOutput["TWY"][SelCol];//WY的值
                    #endregion

                    #region "身体部分的赋值"
                    float TWY = this._gcLevelOutput["TWY"][SelCol];//必填项            
                    float selectG65 = this._gcLevelOutput["G65"][SelCol];//必填项
                    float GCMaxWY = this._gcLevelOutput["WY"][SelCol];

                    if (selectG65 != 0)//判断除数不为零
                    {
                        float LightTotal = 0;//
                        foreach (string key in this._LightContent.Keys)
                        {
                            if (key != "CO2")//H2S和O2和N2
                                LightTotal += this._LightContent[key];//轻端中除CO2外，所有组成质量的加和为LEND.TOTAL。
                        }

                        float SUMFOUND = 0;
                        foreach (string strG in this._G00_G64List)
                        {
                            try
                            {
                                if (strG == "G06")
                                {
                                    if (this._LightContent.Keys.Contains("GX1") && this._LightContent.Keys.Contains(strG))
                                        this._LightContent[strG] += this._LightContent["GX1"];
                                }

                                if (this._LightContent.Keys.Contains(strG))
                                {
                                    if (this._gcLevelOutput[strG][SelCol].Equals(float.NaN))
                                        this._gcLevelOutput[strG][SelCol] = 0;
                                    this._gcLevelOutput[strG][this._COlNUM - 1] = (this._LightContent[strG] * LightWY + this._gcLevelOutput[strG][SelCol] * GCMaxWY) / (LightTotal * LightWY + selectG65 * GCMaxWY) * TWY;
                                }
                                else
                                {
                                    if (this._gcLevelOutput[strG][SelCol].Equals(float.NaN))
                                        this._gcLevelOutput[strG][SelCol] = 0;
                                    this._gcLevelOutput[strG][this._COlNUM - 1] = (this._gcLevelOutput[strG][SelCol] * GCMaxWY) / (LightTotal * LightWY + selectG65 * GCMaxWY) * TWY;
                                }
                                if (!this._gcLevelOutput[strG][this._COlNUM - 1].Equals(float.NaN))
                                    SUMFOUND += this._gcLevelOutput[strG][this._COlNUM - 1];
                            }
                            catch (Exception ex)
                            {
                                Log.Error("" + ex.ToString());
                            }
                        }

                        this._gcLevelOutput["G65"][this._COlNUM - 1] = SUMFOUND;
                    }

                    #endregion
                }
                else
                {
                    #region "头部的赋值"
                    //this._gcLevelOutput["ICP"][this._COlNUM - 1] = this._gcLevelOutput["ICP"][SelCol];//ICP的值
                    this._gcLevelOutput["ICP"][this._COlNUM - 1] =  -50;//ICP的值
                    this._gcLevelOutput["ECP"][this._COlNUM - 1] = this._gcLevelOutput["ECP"][SelCol];//ECP的值
                    this._gcLevelOutput["WY"][this._COlNUM - 1] = this._gcLevelOutput["TWY"][SelCol];//WY的值
                    #endregion

                    #region "身体部分的赋值"

                    float TWY = this._gcLevelOutput["TWY"][SelCol];//必填项            
                    float selectG65 = this._gcLevelOutput["G65"][SelCol];//必填项

                    if (selectG65 != 0 && TWY != 0)//判断除数不为零
                    {
                        float SUMFOUND = 0;
                        foreach (string strG in this._G00_G64List)
                        {
                            this._gcLevelOutput[strG][this._COlNUM - 1] = this._gcLevelOutput[strG][SelCol] / selectG65 * TWY;
                            if (!this._gcLevelOutput[strG][this._COlNUM - 1].Equals(float.NaN))
                                SUMFOUND += this._gcLevelOutput[strG][this._COlNUM - 1];
                        }
                        this._gcLevelOutput["G65"][this._COlNUM - 1] = SUMFOUND;
                    }

                    #endregion                           
                }
            }
            else
            {
                #region "头部的赋值"
                this._gcLevelOutput["ICP"][this._COlNUM - 1] = this._gcLevelOutput["ICP"][SelCol];//ICP的值
                this._gcLevelOutput["ECP"][this._COlNUM - 1] = this._gcLevelOutput["ECP"][SelCol];//ECP的值
                this._gcLevelOutput["WY"][this._COlNUM - 1] = this._gcLevelOutput["TWY"][SelCol];//WY的值
                #endregion

                #region "身体部分的赋值"

                float TWY = this._gcLevelOutput["TWY"][SelCol];//必填项            
                float selectG65 = this._gcLevelOutput["G65"][SelCol];//必填项

                if (selectG65 != 0 && TWY != 0)//判断除数不为零
                {
                    float SUMFOUND = 0;
                    foreach (string strG in this._G00_G64List)
                    {
                        this._gcLevelOutput[strG][this._COlNUM - 1] = this._gcLevelOutput[strG][SelCol] / selectG65 * TWY;
                        if (!this._gcLevelOutput[strG][this._COlNUM - 1].Equals(float.NaN))
                            SUMFOUND += this._gcLevelOutput[strG][this._COlNUM - 1];
                    }                    
                    this._gcLevelOutput["G65"][this._COlNUM - 1] = SUMFOUND;
                }

                #endregion            
            }          
        }

        /// <summary>
        ///  GC数据转换为标准表
        /// </summary>
        public void GcNormTable()
        {
            #region "判断初始化条件"
            if (this._MatchTable1 == null || this._MatchTable2 == null)//判断输入的表格不能为空
                return;

            if (this._MatchTable1.Count <= 0 || this._MatchTable2.Count <= 0)
                return;

            if (this._datas == null)
                return;
            if (this._datas.Count <= 0)
                return;
            this._G00_G64List.Add("G00");
            foreach (var item in this._MatchTable2)
                this._G00_G64List.Add(item.itemCode);
            //DeleteAllNormGridValues();//删除标准表数据
            this._gcLevelOutput = NormTable();//初始化表格

            List<OilTableRowEntity> normTalbeRow = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            #endregion

            #region "中间数据的处理"

            for (int colIndex = 0; colIndex < this._Max; colIndex++)//输入表的列循环
            {
                List<OilDataEntity> headRowGroup = this._datas.Where(o =>o.ColumnIndex == colIndex && o.RowIndex < this._headDataNum).ToList();

                for (int headRowIndex = 0; headRowIndex < headRowGroup.Count; headRowIndex++)
                {
                    string strCal = headRowGroup[headRowIndex].labData;
                    string itemCode = headRowGroup[headRowIndex].OilTableRow.itemCode;
                    float fCal = 0;
                    if (float.TryParse(strCal, out fCal))
                    {
                        if (this._gcLevelOutput.Keys.Contains(itemCode))
                            this._gcLevelOutput[itemCode][colIndex] = fCal;
                    }
                }

                List<OilDataEntity> bodyRowGroup = this._datas.Where(o => o.ColumnIndex == colIndex && o.RowIndex > this._headDataNum).ToList();

                GcNormTableCol(bodyRowGroup, colIndex);//GC数据转换为标准表的半成品表即列WT%无数据                 
            }

            #endregion

            #region "最后一列的数据处理,判断馏分段是否连续，如果连续找出那个馏分段连续；本表一共有5个Cut列所有最多有2个连续的馏分段"           
            List<OilDataEntity> tempList = IsMaxContinuseCol();

            if (tempList.Count == 0)
            {
                return;
            }
            else if (tempList.Count == 1)
            {
                GcNormFunctionInterval(tempList[0].ColumnIndex);
            }
            else if (tempList.Count > 1)//存在最大的连续馏分(方案一)
            {
                GcNormFunctionContinuse(tempList);
            }       
            #endregion

            #region "保存数据"
            foreach (string key in this._gcLevelOutput.Keys)
            {
                float value = this._gcLevelOutput[key][this._COlNUM - 1];//
                //for (int colIndex = 0; colIndex < col.Length ; colIndex ++)
                //{
                    //this._GCLevelGridOil.SetData(key, 0, "");
                    if (!value.Equals(float.NaN))
                        this._GCLevelGridOil.SetData(key, 0, value.ToString());
                    else
                        this._GCLevelGridOil.SetData(key, 0, ""); 
                //}
            }
            #endregion                      
        }
        #endregion
        #endregion 

        #region "统计计算"
        /// <summary>
        /// 统计
        /// </summary>
        public void Calculation()
        {
            try
            {
                bool headNull = InputHeadCheck();
                if (!headNull)
                    MessageBox.Show("馏分的初切点/终切点/WY/TWY/VY/D20/API为空，请在宽馏分中输入数据！", "输入检测", MessageBoxButtons.OK);
                else
                {
                    bool ready = InputDataGuiYi();//数据归一检查
                    if (ready)
                    {
                        GcClaAndStaTable();//数据统计
                        GcNormTable();//数据标准转换
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GC表的数据转换:" + ex);
            }
        }
        #endregion 
    }
}
