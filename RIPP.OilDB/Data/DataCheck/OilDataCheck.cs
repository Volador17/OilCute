using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using RIPP.Lib;

namespace RIPP.OilDB.Data.DataCheck
{
    public class returnError
    {
        private string _itemCode;
        private string _error;

        public returnError()
        {
            this._error = "";
            this._itemCode = "";
        }
        public returnError(string itemCode,string error)
        {
            this._error = error;
            this._itemCode = itemCode;
        }

        public string ItemCode
        {
            get { return this._itemCode; }
            set { this._itemCode = value; }
        }

        public string Error
        {
            get { return this._error; }
            set { this._error = value; }
        }
    }
    public class OilDataCheck
    {       
        #region "私有变量"
        /// <summary>
        /// 传递过来需要审查的窗体的列实体集合
        /// </summary>
        private IList<OilTableColEntity> _cols = null;
        /// <summary>
        /// 传递过来需要审查的窗体的行实体集合
        /// </summary>
        private IList<OilTableRowEntity> _rows = null;
        /// <summary>
        /// 传递过来窗体上的数据
        /// </summary>
        private IList<OilDataEntity> _datas = null;
        /// <summary>
        /// 传递过来需要审查的窗体
        /// </summary>
        private GridOilViewA _gridOil = null;
        /// <summary>
        /// 含有数据的最大列
        /// </summary>
        private int _maxCol = 0;
        /// <summary>
        /// 传递过来需要审查的原油性质窗体
        /// </summary>
        private GridOilViewA _wholeGridOil = null;
        /// <summary>
        /// 传递过来需要审查的轻端表窗体
        /// </summary>
        private GridOilViewA _lightGridOil = null;
        /// <summary>
        /// 传递过来需要审查的窄馏分窗体
        /// </summary>
        private GridOilViewA _narrowGridOil = null;
        /// <summary>
        /// 传递过来需要审查的宽馏分窗体
        /// </summary>
        private GridOilViewA _wideGridOil = null;
        /// <summary>
        /// 传递过来需要审查的渣馏分窗体
        /// </summary>
        private GridOilViewA _residueGridOil = null;
        #endregion 

        #region "审查的构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilDataCheck()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gridOil">需要检查的表</param>
        /// <param name="tableType">设置检查表的类型</param>
        //public OilDataCheck(GridOilViewA gridOil ,EnumTableType tableType)
        //{
        //    this._gridOil = gridOil;
        //    this._tableType = tableType;
        //}

        /// <summary>
        /// 范围的构造函数
        /// </summary>
        /// <param name="wholeGridOil">原油性质</param>
        /// <param name="lightGridOil">轻端表</param>
        /// <param name="narrowGridOil">窄馏分表</param>
        /// <param name="wideGridOil">宽馏分表</param>
        /// <param name="residueGridOil">渣油分表</param>
        public OilDataCheck(GridOilViewA wholeGridOil, GridOilViewA lightGridOil, 
            GridOilViewA narrowGridOil, GridOilViewA wideGridOil, GridOilViewA residueGridOil)
        {
            this._wholeGridOil = wholeGridOil;
            this._lightGridOil = lightGridOil;
            this._narrowGridOil = narrowGridOil;
            this._wideGridOil = wideGridOil;
            this._residueGridOil = residueGridOil;           
        }
        
        /// <summary>
        /// 找到填有数据的最大列找到填有数据的最大列
        /// </summary>
        private void getMaxCol()
        {
            this._maxCol = this._gridOil.GetMaxValidColumnIndex() + 1;
        }
        #endregion 

        /// <summary>
        /// 检查数据错误
        /// </summary>
        /// <param name="tableType"></param>
        public string CheckAllDataListError(EnumTableType tableType)
        {
            string  str  = string.Empty;
            List<returnError> errorList = new List<returnError>();
            if (tableType == EnumTableType.Whole)
            {
                this._gridOil = this._wholeGridOil;
                getMaxCol();//找到填有数据的最大列  
                errorList.AddRange(checkKeyErr(EnumTableType.Whole));  
                errorList.AddRange(WholeLinkCheckError());
                
            }
            if (tableType == EnumTableType.Narrow)
            {
                this._gridOil = this._narrowGridOil;
                getMaxCol();//找到填有数据的最大列
                errorList.AddRange(checkKeyErr(EnumTableType.Narrow));//严格审查：缺项，超限，格式错误
                errorList.AddRange(NarrowLinkCheck());//严格审查：关联错误
            }
            else if (tableType == EnumTableType.Wide)
            {
                this._gridOil = this._wideGridOil;
                getMaxCol();//找到填有数据的最大列 
                errorList.AddRange(checkKeyErr(EnumTableType.Wide));
                errorList.AddRange(WideLinkCheck());
            }
            else if (tableType == EnumTableType.Residue)
            {
                this._gridOil = this._residueGridOil;
                getMaxCol();//找到填有数据的最大列 
                errorList.AddRange(checkKeyErr(EnumTableType.Residue));
                errorList.AddRange(ResidueLinkCheck());
            }

            foreach (DataGridViewRow row in this._gridOil.Rows)
            {
                OilTableRowEntity rowEntity = this._gridOil.GetRowEntity(row.Index);
                string itemCode = rowEntity.itemCode;
                List<returnError> tempErrorList = errorList.Where(o => o.ItemCode == itemCode && o.Error != string.Empty).ToList();
                foreach (var item in tempErrorList)
                {
                    str += item.Error;
                }
            }
            if (errorList.Count <= 0)
                this._gridOil.IsValidated = true;
            return str;
        }


        #region "错误信息提示:严格检查"
        /// <summary>
        /// 数据检查检查关键项，数据格式及范围审查
        /// </summary>
        /// <returns>错误提示字符串</returns>
        public List<returnError> checkKeyErr(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            string itemCode;
            OilTableRowBll oilTableRowBll = new OilTableRowBll();
            List<OilTableRowEntity> rows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)tableType).ToList();

            OilDataCheck dataCheck = new OilDataCheck();
            string temp;
            this._gridOil.ClearRemarkFlat();//20161208新增,再次严格审查前先初始化表格中的背景颜色
            foreach (DataGridViewRow row in this._gridOil.Rows)
            {
                OilTableRowEntity rowEntity = this._gridOil.GetRowEntity(row.Index);
                itemCode = rowEntity.itemCode;

                for (int i = 0; i < this._maxCol; i++)
                {
                    OilTableColEntity colEntity = this._gridOil.GetColEntity(i);
                    string colName = colEntity == null ? string.Empty : colEntity.colName;
                    OilDataEntity oilData = this._gridOil.GetDataByRowItemCodeColumnIndex(itemCode, i);
                    string strCellValue = oilData == null ? string.Empty : oilData.calData;

                    if (rowEntity.isKey)//如果是关键字,如果为空，提示，如果不为空验证数据
                    {
                        #region "是关键值不为空验证"
                        if (string.IsNullOrWhiteSpace (strCellValue))//单元格的值为空
                        {
                            temp = dataCheck.CheckMetion(tableType, rowEntity.itemName, row.Index, colName, enumCheckErrType.Blank);//判断关键值是否有值
                            if (!string.IsNullOrWhiteSpace(temp))
                            {
                                if (tableType == EnumTableType.Narrow && itemCode == "ICP" && i == 0)//窄馏分的ICP(cut1)不做严格审查。
                                {

                                }
                                else
                                {
                                    sbAlert.Add(new returnError(itemCode, temp));
                                    this._gridOil.SetRemarkFlag(itemCode, i, true,GridOilColumnType.Calc);
                                }
                            }
                        }
                        else
                        {
                            temp = check(tableType, rowEntity.dataType, strCellValue, rowEntity.errDownLimit, rowEntity.errUpLimit, rowEntity.itemName, row.Index, colName);
                            if (!string.IsNullOrWhiteSpace(temp))
                            {
                                sbAlert.Add(new returnError(itemCode, temp));
                                this._gridOil.SetRemarkFlag(itemCode, i, true, GridOilColumnType.Calc);
                            }
                        }
                        #endregion 
                    }
                    else
                    {
                        #region "不是关键值"
                        if (!string.IsNullOrWhiteSpace (strCellValue)) //不空则验证
                        {
                            temp = check(tableType, rowEntity.dataType, strCellValue, rowEntity.errDownLimit, rowEntity.errUpLimit, rowEntity.itemName, row.Index, colName);
                            if (!string.IsNullOrWhiteSpace (temp))
                            {
                                sbAlert.Add(new returnError(itemCode, temp));
                                this._gridOil.SetRemarkFlag(itemCode, i, true, GridOilColumnType.Calc);
                            }
                        }
                        #endregion  
                    }
                }
            }
            return sbAlert ;
        }
        

        /// <summary>
        /// 检查是否日期
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>是True</returns>
        public bool checkDate(string value)
        {
           //string te =  string.Format("yyyy-MM-dd", value);
           
            DateTime tempValue  ;
            if (value.Count() < 7)
                return false;

            if (DateTime.TryParse(value, out tempValue)) //如果是日期
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 检查是浮点
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>是True</returns>
        public bool checkFloat(string value)
        {
            float tempValue;
            if (float.TryParse(value, out tempValue)) //如果是浮点
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 数据格式及数据范围审查（处于某个上限和下限之间）
        /// </summary>
        /// <param name="dataType">数据格式</param>
        /// <param name="value">要检查的值</param>
        /// <param name="downLimit">下限</param>
        /// <param name="upLimit">上限</param>
        /// <returns>是否正确</returns>
        private string check(EnumTableType tableType,string dataType, string value, float? downLimit, float? upLimit, string itemName, int row, string colName)
        {          
            returnError sbAlert = new returnError();
            if (value == "yukiu")
            {
                int c=0;
                c++;
            }
            float tempValue = 0;
            if (dataType.ToUpper() == "FLOAT")  //如果数据类型是浮点数
            {              
                if (float.TryParse(value, out tempValue)) //如果是浮点数
                {
                    if (downLimit != null && upLimit == null)
                    {
                        if (tempValue >= downLimit.Value) //若果在上限和下限之间
                            return "";
                        else
                            return CheckMetion(tableType,itemName, row, colName, enumCheckErrType.ErrLimit);
                    }
                    else if (downLimit == null && upLimit != null)
                    {
                        if (tempValue <= upLimit.Value) //若果在上限和下限之间
                            return "";
                        else
                            return CheckMetion(tableType, itemName, row, colName, enumCheckErrType.ErrLimit);
                    }
                    else if (downLimit != null && upLimit != null)
                    {
                        if (tempValue >= downLimit.Value && tempValue <= upLimit.Value) //若果在上限和下限之间
                            return "";
                        else
                            return CheckMetion(tableType, itemName, row, colName, enumCheckErrType.ErrLimit);
                    }
                    else if (downLimit == null && upLimit == null)
                    {
                    }
                    //if (downLimit == 0 && upLimit == 0)  //上下限相等表示没有限制
                    //    return "";
                    //if (tempValue >= downLimit && tempValue <= upLimit) //若果在上限和下限之间
                    //    return "";
                    //else
                    //    return CheckMetion(itemName, row, colName, enumCheckErrType.ErrLimit);
                }
                else
                {
                    return CheckMetion(tableType,itemName, row, colName, enumCheckErrType.TypeError);
                }
            }
            else if (dataType.ToUpper() == "STRING")  //如果数据类型是字符
            {
                return "";
            }
            return "";
        }


        public DateTime? GetDate(object value)
        {
            DateTime date;
            int temp;
            if (value != null)
                if (DateTime.TryParse(value.ToString(), out date)) //如果是日期
                    return date;
                else if (int.TryParse(value.ToString(), out temp))
                {
                    if(temp>1900& temp<9999)
                    {
                        return DateTime.Parse(temp.ToString() + "-1-1");
                    }
                }

            return  null;
        }
        #endregion 
    
        #region 错误提示：缺项错误, 类型错误,范围错误,关联错误

        /// <summary>
        /// 缺项错误, 类型错误,范围错误,关联错误
        /// </summary>
        /// <param name="itemName">项目名称</param>
        /// <param name="row">行号</param>
        /// <param name="errType">错误类别</param>
        /// <returns>错误提示</returns>
        public string CheckMetion(string itemName, int row, enumCheckErrType errType)
        {
            StringBuilder sbAlert = new StringBuilder();
            sbAlert.Append("第" + (row + 1).ToString());
            sbAlert.Append("行，");
            sbAlert.Append(itemName);
            switch (errType)
            {
                case enumCheckErrType.Blank: sbAlert.Append("缺项！"); break;
                case enumCheckErrType.TypeError: sbAlert.Append("校正值数据类型错误！"); break;
                case enumCheckErrType.ErrLimit: sbAlert.Append("校正值超限！"); break;
                case enumCheckErrType.Relative: sbAlert.Append("校正值关联错误！"); break;
            }
            sbAlert.Append("\n");
            return sbAlert.ToString();
        }



        /// <summary>
        /// 缺项错误, 数据类型错误,范围错误,关联错误
        /// </summary>
        /// <param name="itemName">项目名称</param>
        /// <param name="row">行号</param>\
        /// <param name="colName">列号</param>
        /// <param name="errType">错误类别</param>
        /// <returns>错误提示</returns>
        public string CheckMetion(EnumTableType tablType, string itemName, int row, string colName, enumCheckErrType errType)
        {
            StringBuilder sbAlert = new StringBuilder();
            sbAlert.Append(tablType.GetDescription());
            sbAlert.Append("第" + (row + 1).ToString());
            sbAlert.Append("行," + colName+ "列,");
            sbAlert.Append(itemName);
            switch (errType)
            {
                case enumCheckErrType.Blank: sbAlert.Append("缺项！"); break;
                case enumCheckErrType.TypeError: sbAlert.Append("数据类型错误！"); break;
                case enumCheckErrType.ErrLimit: sbAlert.Append("校正值错误超限！"); break;
                case enumCheckErrType.AlertLimit: sbAlert.Append("校正值警告超限！"); break;
                case enumCheckErrType.Relative: sbAlert.Append("校正值关联错误！"); break;
            }
            sbAlert.Append("\n");
            return sbAlert.ToString();
        }
        /// <summary>
        /// 缺项错误, 数据类型错误,范围错误,关联错误
        /// </summary>
        /// <param name="itemName">项目名称</param>
        /// <param name="row">行号</param>\
        /// <param name="colName">列号</param>
        /// <param name="errType">错误类别</param>
        /// <returns>错误提示</returns>
        public string CheckMetion(string itemName, int row, string colName, enumCheckErrType errType,EnumTableType tableName)
        {
            StringBuilder sbAlert = new StringBuilder();
            string strTableName = string.Empty;
            switch (tableName)
            {
                case EnumTableType.Whole:   strTableName = "原油表"; break;
                case EnumTableType.Narrow:  strTableName = "窄馏分表"; break;
                case EnumTableType.Wide:    strTableName = "宽馏分表"; break;
                case EnumTableType.Residue: strTableName = "渣油表"; break;
            }
            sbAlert.Append(strTableName);
            sbAlert.Append("第" + (row + 1).ToString());
            sbAlert.Append("行," + colName + "列,");
            sbAlert.Append(itemName);
            switch (errType)
            {
                case enumCheckErrType.Blank: sbAlert.Append("缺项！"); break;
                case enumCheckErrType.TypeError: sbAlert.Append("数据类型错误！"); break;
                case enumCheckErrType.ErrLimit: sbAlert.Append("校正值错误超限！"); break;
                case enumCheckErrType.AlertLimit: sbAlert.Append("校正值警告超限！"); break;
                case enumCheckErrType.Relative: sbAlert.Append("数据关联错误！"); break;
            }         
            sbAlert.Append("\n");
            return sbAlert.ToString();
        }

        /// <summary>
        /// 经验审查, 数据类型错误,范围错误,关联错误
        /// </summary>
        /// <param name="itemName">项目名称</param>
        /// <param name="row">行号</param>\
        /// <param name="colName">列号</param>
        /// <param name="errType">错误类别</param>
        /// <returns>错误提示</returns>
        public static  string ExperienceCheckMetion(EnumTableType tableName,OilTableRowEntity row, OilTableColEntity col, enumCheckExpericencType errType)
        {
            StringBuilder sbAlert = new StringBuilder();
            string strTableName = string.Empty;
            switch (tableName)
            {
                case EnumTableType.Narrow: strTableName = "窄馏分表"; break;
                case EnumTableType.Wide: strTableName = "宽馏分表"; break;
                case EnumTableType.Residue: strTableName = "渣油表"; break;
            }
            sbAlert.Append(strTableName);
            sbAlert.Append("第" + (row.RowIndex + 1).ToString());
            sbAlert.Append("行," + col.colName + "列,");
            //sbAlert.Append(itemName);
            switch (errType)
            {
                case enumCheckExpericencType.Tend: sbAlert.Append("数据不满足经验趋势要求,有疑问。"); break;
                case enumCheckExpericencType.Limit: sbAlert.Append("数据超出经验范围,有疑问。"); break;
            }
            sbAlert.Append("\n");
            
            return sbAlert.ToString();
        }
        /// <summary>
        /// 趋势审查错误字符提示
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="row">行</param>
        /// <param name="colLess">列</param>
        /// <param name="colMore">列</param>
        /// <returns>错误提示</returns>
        public static string ExperienceTrendCheckMetion(EnumTableType tableName, OilTableRowEntity row, OilTableColEntity colLess, OilTableColEntity colMore)
        {
            StringBuilder sbAlert = new StringBuilder();
            string strTableName = string.Empty;
            switch (tableName)
            {
                case EnumTableType.Narrow: strTableName = "窄馏分表"; break;
                case EnumTableType.Wide: strTableName = "宽馏分表"; break;
                case EnumTableType.Residue: strTableName = "渣油表"; break;
            }
            sbAlert.Append(strTableName);
            sbAlert.Append("第" + (row.RowIndex + 1).ToString());
            sbAlert.Append("行," + colLess.colName + "列到"+ colMore.colName +"列的数据变化趋势不满足经验审查趋势要求，有疑问");
            sbAlert.Append("\n");

            return sbAlert.ToString();
        }
        
        
        #endregion

 
        #region  关联审查      

        #region //四个表的公共关联审查函数
        /// <summary>
        /// 根据行的数据集合和列从中获取单元格的校正值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string getStrValuefromOilDataEntity(List<OilDataEntity> row, int column, bool BCalShowData = false)//为何要默认BCalShowData = false
        {
            string strResult = string.Empty;

            if (row == null || row.Count <= 0 || column < 0)
                return strResult;

            OilDataEntity OilData = row.Where(o => o.ColumnIndex == column).FirstOrDefault();
            if (!BCalShowData)
                strResult = OilData == null ? string.Empty : OilData.calData  ;
            else
                strResult = OilData == null ? string.Empty : OilData.calShowData;
            return strResult;
        }
        /// <summary>
        /// VY(i)=WY(i)/D20(i)*D20(CRU)
        /// </summary>
        /// <param name="strWY"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        private string FunVY(string strWY, string strD20)
        {
            if (strWY == string.Empty || strD20 == string.Empty)
                return null;

            float VY = 0;

            //var ds = this._parent.Oil.OilDatas.Where(d => d.OilTableTypeID == (int)EnumTableType.Whole).ToList();
            //List<OilDataEntity> WholeD20List = ds.Where(o => o.OilTableRow.itemCode == "D20").ToList();//选出窄馏分ICP行的数据

            //float WY = Convert.ToSingle(strWY);
            //float D20 = Convert.ToSingle(strD20);
            //float WholeD20 = WholeD20List[0].calData != string.Empty ? Convert.ToSingle(WholeD20List[0].calData) : 0;
            ///* VY(i)=WY(i)/D20(i)*D20(CRU)*/
            //if (WholeD20 != 0)
            //{
            //    VY = WY / D20 * WholeD20;
            //}
            return VY.ToString();
        }

        /// <summary>
        /// D15关联审查
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> D15LinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");

            if (D15OilDataList == null)
                return sbAlert;  
            if (D15OilDataList.Count <= 0)
                return sbAlert;  

            for (int i = 0; i < this._maxCol; i++)
            {
                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                string D15cal = getStrValuefromOilDataEntity(D15OilDataList, i);  

                #region
                float tempD20, tempD15;
                if (D15cal != string.Empty && D20cal != string.Empty
                    && float.TryParse(D20cal, out tempD20) && float.TryParse(D15cal, out tempD15))
                {
                    if (!(tempD15 > tempD20))
                    {
                        sbAlert.Add(new returnError("D15", CheckMetion(D15OilDataList[0].OilTableRow.itemName, D15OilDataList[0].OilTableRow.RowIndex, D15OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));                    
                        this._gridOil.SetRemarkFlag("D15", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }
            return sbAlert;
        }
        // <summary>
        /// V02、V04、V05、V08、V10   V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> V0_LinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");

            /*关联错误v02>v04>v05>v08>v10*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string V02cal = getStrValuefromOilDataEntity(V02OilDataList, i);
                string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                string V05cal = getStrValuefromOilDataEntity(V05OilDataList, i);
                string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);

                #region "粘度关联式子 "
                List<VT> VTList = new List<VT>();
                float fV02 = 0, fV04 = 0, fV05 = 0, fV08 = 0, fV10 = 0;

                if (V02cal != string.Empty && float.TryParse(V02cal, out fV02))
                {
                    VTList.Add(new VT(fV02, 20, V02OilDataList[i]));
                }

                if (V04cal != string.Empty && float.TryParse(V04cal, out fV04))
                {
                    VTList.Add(new VT(fV04, 40, V04OilDataList[i]));
                }

                if (V05cal != string.Empty && float.TryParse(V05cal, out fV05))
                {
                    VTList.Add(new VT(fV05, 50, V05OilDataList[i]));
                }

                if (V08cal != string.Empty && float.TryParse(V08cal, out fV08))
                {
                    VTList.Add(new VT(fV08, 80, V08OilDataList[i]));
                }


                if (V10cal != string.Empty && float.TryParse(V10cal, out fV10))
                {
                    VTList.Add(new VT(fV10, 100, V10OilDataList[i]));
                }
                #endregion

                List<VT> afterOrder = VTList.OrderBy(o => o.T).ToList();
                if (VTList.Count <= 1)
                    continue;

                for (int j = 1; j < VTList.Count; j ++)
                {
                    if (VTList[j - 1].fV < VTList[j].fV)
                    {
                        sbAlert.Add (new returnError(VTList[j - 1].OilData.OilTableRow.itemCode, CheckMetion(VTList[j - 1].OilData.OilTableRow.itemName, VTList[j - 1].OilData.OilTableRow.RowIndex, VTList[j - 1].OilData.OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag(VTList[j - 1].OilData.OilTableRow.itemCode, i, true, GridOilColumnType.Calc);
                    }
                }              
            }
            return sbAlert ;
        }
      
        /// <summary>
        /// H2关联审查 CAR>H2
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> H2LinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            if (H2OilDataList == null)
                return sbAlert;
            if (H2OilDataList.Count <= 0)
                return sbAlert;
            for (int i = 0; i < this._maxCol; i++)
            {
                string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);
                string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);
                #region
                float tempSUL, tempH2;
                if (SULcal != string.Empty && H2cal != string.Empty
                    && float.TryParse(SULcal, out tempSUL) && float.TryParse(H2cal, out tempH2))
                {
                    if (!(tempSUL > tempH2))
                    {
                        sbAlert.Add ( new returnError("H2", CheckMetion(H2OilDataList[0].OilTableRow.itemName, H2OilDataList[0].OilTableRow.RowIndex, H2OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("H2", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion                
            }
            return sbAlert ;
        }
        /// <summary>
        /// SUL关联审查 H2>SUL
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> SULLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");

            if (SULOilDataList == null)
                return sbAlert;
            if (SULOilDataList.Count <= 0)
                return sbAlert;
            for (int i = 0; i < this._maxCol; i++)
            {
                string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);
                string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);
                #region
                float tempSUL, tempH2;
                if (SULcal != string.Empty && H2cal != string.Empty
                    && float.TryParse(SULcal, out tempSUL) && float.TryParse(H2cal, out tempH2))
                {
                    if (!(tempH2 > tempSUL))
                    {
                        sbAlert.Add ( new returnError("SUL", CheckMetion(SULOilDataList[0].OilTableRow.itemName, SULOilDataList[0].OilTableRow.RowIndex, SULOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("SUL", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }
            return sbAlert;
        }
        /// <summary>
        /// H2S关联审查 H2S<SUL*10000
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> H2SLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> H2SOilDataList = this._gridOil.GetDataByRowItemCode("H2S");
            if (H2SOilDataList == null)
                return sbAlert;
            if (H2SOilDataList.Count <= 0)
                return sbAlert;
            for (int i = 0; i < this._maxCol; i++)
            {
                string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);
                string H2Scal = getStrValuefromOilDataEntity(H2SOilDataList, i);
                #region
                float tempSUL, tempH2S;
                if (SULcal != string.Empty && H2Scal != string.Empty
                    && float.TryParse(SULcal, out tempSUL) && float.TryParse(H2Scal, out tempH2S))
                {
                    if (!(tempH2S < tempSUL * 10000))
                    {
                        sbAlert.Add ( new returnError("H2S", CheckMetion(H2SOilDataList[0].OilTableRow.itemName, H2SOilDataList[0].OilTableRow.RowIndex, H2SOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("H2S", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }           
            return sbAlert ;
        }
        /// <summary>
        /// MEC关联审查MEC<SUL*10000
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> MECLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> MECOilDataList = this._gridOil.GetDataByRowItemCode("MEC");

            if (MECOilDataList == null)
                return sbAlert ;
            if (MECOilDataList.Count <= 0)
                return sbAlert;
 
            for (int i = 0; i < this._maxCol; i++)
            {
                string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);
                string MECcal = getStrValuefromOilDataEntity(MECOilDataList, i);
                #region
                float tempSUL, tempMEC;
                if (SULcal != string.Empty && MECcal != string.Empty
                    && float.TryParse(SULcal, out tempSUL) && float.TryParse(MECcal, out tempMEC))
                {
                    if (!(tempMEC < tempSUL * 10000))
                    {
                        sbAlert.Add (new returnError("MEC", CheckMetion(MECOilDataList[0].OilTableRow.itemName, MECOilDataList[0].OilTableRow.RowIndex, MECOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("MEC", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }                  
            return sbAlert ;
        }
        /// <summary>
        /// N2关联审查H2>N2
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> N2LinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> N2OilDataList = this._gridOil.GetDataByRowItemCode("N2");

            if (N2OilDataList == null)
                return sbAlert ;
            if (N2OilDataList.Count <= 0)
                return sbAlert ;

            for (int i = 0; i < this._maxCol; i++)
            {
                string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);
                string N2cal = getStrValuefromOilDataEntity(N2OilDataList, i);
                #region
                float tempH2, tempN2;
                if (H2cal != string.Empty && N2cal != string.Empty
                    && float.TryParse(H2cal, out tempH2) && float.TryParse(N2cal, out tempN2))
                {
                    if (!(tempH2 > tempN2/10000))
                    {
                        sbAlert.Add ( new returnError("N2", CheckMetion(N2OilDataList[0].OilTableRow.itemName, N2OilDataList[0].OilTableRow.RowIndex, N2OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("N2", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }           
 
            return sbAlert ;
        }
        /// <summary>
        /// BAN关联审查BAN<N2
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> BANLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> BANOilDataList = this._gridOil.GetDataByRowItemCode("BAN");
            List<OilDataEntity> N2OilDataList = this._gridOil.GetDataByRowItemCode("N2");

            if (BANOilDataList == null)
                return sbAlert ;
            if (BANOilDataList.Count <= 0)
                return sbAlert ;

            for (int i = 0; i < this._maxCol; i++)
            {
                string BANcal = getStrValuefromOilDataEntity(BANOilDataList, i);
                string N2cal = getStrValuefromOilDataEntity(N2OilDataList, i);
                #region
                float tempBAN, tempN2;
                if (BANcal != string.Empty && N2cal != string.Empty
                    && float.TryParse(BANcal, out tempBAN) && float.TryParse(N2cal, out tempN2))
                {
                    if (!(tempBAN < tempN2))
                    {
                        sbAlert.Add (new returnError("BAN", CheckMetion(BANOilDataList[0].OilTableRow.itemName, BANOilDataList[0].OilTableRow.RowIndex, BANOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("BAN", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            } 

            return sbAlert ;
        }
        /// <summary>
        /// MEI关联审查MEI<CCR
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> MEILinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MEIOilDataList = this._gridOil.GetDataByRowItemCode("MEI");
            List<OilDataEntity> CCROilDataList = this._gridOil.GetDataByRowItemCode("CCR");
            
            if (MEIOilDataList == null)
                return sbAlert ;
            if (MEIOilDataList.Count <= 0)
                return sbAlert ;

            for (int i = 0; i < this._maxCol; i++)
            {
                string MEIcal = getStrValuefromOilDataEntity(MEIOilDataList, i);
                string CCRcal = getStrValuefromOilDataEntity(CCROilDataList, i);
                #region
                float tempMEI, tempCCR;
                if (MEIcal != string.Empty && CCRcal != string.Empty
                    && float.TryParse(MEIcal, out tempMEI) && float.TryParse(CCRcal, out tempCCR))
                {
                    if (!(tempMEI < tempCCR))
                    {
                        sbAlert.Add ( new returnError("MEI", CheckMetion(MEIOilDataList[0].OilTableRow.itemName, MEIOilDataList[0].OilTableRow.RowIndex, MEIOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("MEI", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }            
            return sbAlert;
        }
        /// <summary>
        /// ASH关联审查ASH<CCR
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> ASHLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> ASHOilDataList = this._gridOil.GetDataByRowItemCode("ASH");
            List<OilDataEntity> CCROilDataList = this._gridOil.GetDataByRowItemCode("CCR");

            if (ASHOilDataList == null)
                return sbAlert ;
            if (ASHOilDataList.Count <= 0)
                return sbAlert ;

            for (int i = 0; i < this._maxCol; i++)
            {
                string ASHcal = getStrValuefromOilDataEntity(ASHOilDataList, i);
                string CCRcal = getStrValuefromOilDataEntity(CCROilDataList, i);
                #region
                float tempASH, tempCCR;
                if (ASHcal != string.Empty && CCRcal != string.Empty
                    && float.TryParse(ASHcal, out tempASH) && float.TryParse(CCRcal, out tempCCR))
                {
                    if (!(tempASH < tempCCR))
                    {
                        sbAlert.Add (new returnError("ASH", CheckMetion(ASHOilDataList[0].OilTableRow.itemName, ASHOilDataList[0].OilTableRow.RowIndex, ASHOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("ASH", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }          
 
            return sbAlert ;
        }
        /// <summary>
        ///NET关联审查ACD>NET
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> NETLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> ACDOilDataList = this._gridOil.GetDataByRowItemCode("ACD");
            List<OilDataEntity> NETOilDataList = this._gridOil.GetDataByRowItemCode("NET");

            if (NETOilDataList == null)
                return sbAlert ;
            if (NETOilDataList.Count <= 0)
                return sbAlert ;

            for (int i = 0; i < this._maxCol; i++)
            {
                string ACDcal = getStrValuefromOilDataEntity(ACDOilDataList, i);
                string NETcal = getStrValuefromOilDataEntity(NETOilDataList, i);
                #region
                float tempACD, tempNET;
                if (ACDcal != string.Empty && NETcal != string.Empty
                    && float.TryParse(ACDcal, out tempACD) && float.TryParse(NETcal, out tempNET))
                {
                    if (!(tempACD > tempNET))
                    {
                        sbAlert.Add (new returnError("NET", CheckMetion(NETOilDataList[0].OilTableRow.itemName, NETOilDataList[0].OilTableRow.RowIndex, NETOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("NET", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }   
 
            return sbAlert ;
        }
        /// <summary>
        /// R70关联审查R70<R20
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> R70LinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> R70OilDataList = this._gridOil.GetDataByRowItemCode("R70");
            List<OilDataEntity> R20OilDataList = this._gridOil.GetDataByRowItemCode("R20");

            if (R70OilDataList == null)
                return sbAlert ;
            if (R70OilDataList.Count <= 0)
                return sbAlert ;

            for (int i = 0; i < this._maxCol; i++)
            {
                string R70cal = getStrValuefromOilDataEntity(R70OilDataList, i);
                string R20cal = getStrValuefromOilDataEntity(R20OilDataList, i);
                #region "关联错误 R70<R20"
                float tempR70, tempR20;
                if (R70cal != string.Empty && R20cal != string.Empty
                    && float.TryParse(R70cal, out tempR70) && float.TryParse(R20cal, out tempR20))
                {
                    if (!(tempR70 < tempR20))
                    {
                        sbAlert.Add (new returnError("R70", CheckMetion(R70OilDataList[0].OilTableRow.itemName, R70OilDataList[0].OilTableRow.RowIndex, R70OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("R70", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
            }

            return sbAlert ;
        }
        /// <summary>
        /// AIP<A10<A30<A50<A70<A90<A95<AEP
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> AIP_AEPLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> AIPOilDataList = this._gridOil.GetDataByRowItemCode("AIP");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            List<OilDataEntity> A95OilDataList = this._gridOil.GetDataByRowItemCode("A95");
            List<OilDataEntity> AEPOilDataList = this._gridOil.GetDataByRowItemCode("AEP");

            for (int i = 0; i < this._maxCol; i++)
            {
                string AIPcal = getStrValuefromOilDataEntity(AIPOilDataList, i);
                string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);
                string A95cal = getStrValuefromOilDataEntity(A95OilDataList, i);
                string AEPcal = getStrValuefromOilDataEntity(AEPOilDataList, i);
 

                #region "关联错误AIP<A10<A30<A50<A70<A90<A95<AEP"
                #region "fAIP = 0, fA10 = 0, fA30 = 0, fA50 = 0, fA70 = 0, fA90 = 0, fA95 = 0, fAEP = 0"
                List<VT> VTList = new List<VT>();
                float fAIP = 0, fA10 = 0, fA30 = 0, fA50 = 0, fA70 = 0, fA90 = 0, fA95 = 0, fAEP = 0;
                if (AIPcal != string.Empty && float.TryParse(AIPcal, out fAIP))
                    VTList.Add(new VT(fAIP, 1, AIPOilDataList[i]));

                if (A10cal != string.Empty && float.TryParse(A10cal, out fA10))
                    VTList.Add(new VT(fA10, 2, A10OilDataList[i]));

                if (A30cal != string.Empty && float.TryParse(A30cal, out fA30))
                    VTList.Add(new VT(fA30, 3, A30OilDataList[i]));

                if (A50cal != string.Empty && float.TryParse(A50cal, out fA50))
                    VTList.Add(new VT(fA50, 4, A50OilDataList[i]));

                if (A70cal != string.Empty && float.TryParse(A70cal, out fA70))
                    VTList.Add(new VT(fA70, 5, A70OilDataList[i]));

                if (A90cal != string.Empty && float.TryParse(A90cal, out fA90))
                    VTList.Add(new VT(fA90, 6, A90OilDataList[i]));

                if (A95cal != string.Empty && float.TryParse(A95cal, out fA95))
                    VTList.Add(new VT(fA95, 7, A95OilDataList[i]));

                if (AEPcal != string.Empty && float.TryParse(AEPcal, out fAEP))
                    VTList.Add(new VT(fAEP, 8, AEPOilDataList[i]));
                #endregion

                List<VT> afterOrder = VTList.OrderBy(o => o.T).ToList();
                if (VTList.Count <= 1)
                    continue;

                for (int j = 1; j < afterOrder.Count; j++)
                {
                    if (afterOrder[j - 1].fV > afterOrder[j].fV)
                    {
                        //sbAlert= new returnError("","宽馏分表的镏程数据关联错误！\n");
                        sbAlert.Add (new returnError(afterOrder[j - 1].OilData.OilTableRow.itemCode, CheckMetion(afterOrder[j - 1].OilData.OilTableRow.itemName, afterOrder[j - 1].OilData.OilTableRow.RowIndex, afterOrder[j - 1].OilData.OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag(afterOrder[j - 1].OilData.OilTableRow.itemCode, i, true, GridOilColumnType.Calc);
                    }
                }
                #endregion                     
                 
            }
            return sbAlert ;
        }
        /// <summary>
        /// 宽馏分表ICP关联审查
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> WideICPLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> narrowECPOilDataList = this._narrowGridOil.GetDataByRowItemCode("ICP");
            if (ICPOilDataList == null)
                return sbAlert;
            if (ICPOilDataList.Count <= 0)
                return sbAlert;

            for (int i = 0; i < this._maxCol; i++)
            {
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i,true);

                if (ICPcal != string.Empty)
                {
                    //判断ICP是否在NCUTS.ICP中
                    OilDataEntity tempData = narrowECPOilDataList.Where(o => o.calShowData == ICPcal).FirstOrDefault();
                    if (tempData == null)
                    {
                        sbAlert .Add (new returnError("ICP", CheckMetion(ICPOilDataList[0].OilTableRow.itemName, ICPOilDataList[0].OilTableRow.RowIndex, ICPOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("ICP", i, true, GridOilColumnType.Calc);
                    }
                    else
                    {
                        float icp, ecp;
                        if (float.TryParse(tempData.calData, out ecp) && float.TryParse(ICPcal, out icp))
                        {
                            if (icp != ecp)
                            {
                                sbAlert.Add (new returnError("ICP", CheckMetion(ICPOilDataList[0].OilTableRow.itemName, ICPOilDataList[0].OilTableRow.RowIndex, ICPOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                                this._gridOil.SetRemarkFlag("ICP", i, true, GridOilColumnType.Calc);                          
                            }
                        }
                    
                    }
                }
            }
            return sbAlert;
        }              
        /// <summary>
        /// 宽馏分表ECP关联审查
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<returnError> WideECPLinkCheck(EnumTableType tableType)
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> narrowECPOilDataList = this._narrowGridOil.GetDataByRowItemCode("ECP");

            if (ECPOilDataList == null)
                return sbAlert ;
            if (ECPOilDataList.Count <= 0)
                return sbAlert ;

            for (int i = 0; i < this._maxCol; i++)
            {
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i,true);

                if (ECPcal != string.Empty)//判断ECP是否在NCUTS.ECP中
                {
                    OilDataEntity tempData = narrowECPOilDataList.Where(o => o.calShowData == ECPcal).FirstOrDefault();
                    if (tempData == null)
                    {
                        sbAlert.Add (new returnError("ECP", CheckMetion(ECPOilDataList[0].OilTableRow.itemName, ECPOilDataList[0].OilTableRow.RowIndex, ECPOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, tableType)));
                        this._gridOil.SetRemarkFlag("ECP", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert ;
        }

        #endregion

        #region //原油性质表关联审查
        /// <summary>
        /// 原油性质表关联检查
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeLinkCheckError()
        {
            List<returnError> sbAlerts = new List <returnError>();
            sbAlerts.AddRange(WholeD15LinkCheck());
            sbAlerts.AddRange(WholeV0LinkCheck());
            sbAlerts.AddRange(WholeH2LinkCheck());
            sbAlerts.AddRange(WholeSULLinkCheck());
            sbAlerts.AddRange(WholeH2SLinkCheck());
            sbAlerts.AddRange(WholeMECLinkCheck());
            sbAlerts.AddRange(WholeN2LinkCheck());
            sbAlerts.AddRange(WholeBANLinkCheck());
            sbAlerts.AddRange(WholeMEILinkCheck());
            sbAlerts.AddRange(WholeASHLinkCheck());
            return sbAlerts ;
        }

        /// <summary>
        /// 原油D15关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeD15LinkCheck()
        {
            return D15LinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// 原油V0关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeV0LinkCheck()
        {
            return V0_LinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// 原油H2关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeH2LinkCheck()
        {
            return H2LinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// 原油SUL关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeSULLinkCheck()
        {
            return SULLinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// 原油H2S关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeH2SLinkCheck()
        {
            return H2SLinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// MEC关联审查MEC<SUL*10000
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeMECLinkCheck()
        {
            return MECLinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// N2关联审查 
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeN2LinkCheck()
        {
            return N2LinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// BAN关联审查 
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeBANLinkCheck()
        {
            return BANLinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// MEI关联审查 
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeMEILinkCheck()
        {
            return MEILinkCheck(EnumTableType.Whole);
        }
        /// <summary>
        /// ASH关联审查 
        /// </summary>
        /// <returns></returns>
        private List<returnError> WholeASHLinkCheck()
        {
            return ASHLinkCheck(EnumTableType.Whole);
        }

        #endregion

        #region //窄馏分关联审查
        private List<returnError> NarrowLinkCheck()
        {
            List<returnError> sbAlerts = new List<returnError>();
            sbAlerts.AddRange(NarrowICPLinkCheck());
            sbAlerts.AddRange(NarrowECPLinkCheck());
            sbAlerts.AddRange(NarrowTWYLinkCheck());
            sbAlerts.AddRange(NarrowVYLinkCheck());
            sbAlerts.AddRange(NarrowTVYLinkCheck());
            sbAlerts.AddRange(NarrowD15LinkCheck());
            sbAlerts.AddRange(NarrowV0_LinkCheck());
            sbAlerts.AddRange(NarrowNETLinkCheck());
            sbAlerts.AddRange(NarrowMECLinkCheck());
            sbAlerts.AddRange(NarrowBANLinkCheck());
            sbAlerts.AddRange(NarrowR70LinkCheck());
            return sbAlerts;
        }
        /// <summary>
        /// 窄馏分表ICP关联审查
        /// </summary>
        private List<returnError> NarrowICPLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            if (ICPOilDataList == null)
                return sbAlert;
            if (ICPOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ICP(i)==ECP(i-1)*/
            for (int i = 1; i < this._maxCol; i++)
            {
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i-1);

                if (ICPcal != ECPcal)
                {
                    sbAlert.Add(new returnError("ICP",CheckMetion(ICPOilDataList[0].OilTableRow.itemName, ICPOilDataList[0].OilTableRow.RowIndex, ICPOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Narrow)));
                    this._gridOil.SetRemarkFlag("ICP", i, true, GridOilColumnType.Calc);
                }              
            }
            return sbAlert;
        }
        /// <summary>
        /// 窄馏分表ECP关联审查
        /// </summary>
        private List<returnError> NarrowECPLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            if (ECPOilDataList == null)
                return sbAlert ;
            if (ECPOilDataList.Count <= 0)
                return sbAlert ;
            /*关联错误ECP(i)>ICP(i)*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                #region
                float tempICP, tempECP;
                if (ICPcal != string.Empty && ECPcal != string.Empty
                    && float.TryParse(ICPcal, out tempICP) && float.TryParse(ECPcal, out tempECP))
                {
                    if (!(tempECP > tempICP))
                    {
                        sbAlert.Add(new returnError("ECP", CheckMetion(ECPOilDataList[0].OilTableRow.itemName, ECPOilDataList[0].OilTableRow.RowIndex, ECPOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Narrow)));
                        this._gridOil.SetRemarkFlag("ECP", i, true, GridOilColumnType.Calc);
                    }
                    else
                        continue;
                }
                else
                    continue;
                #endregion
                
            }
            return sbAlert;
        }
        /// <summary>
        /// 窄馏分表TWY关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowTWYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");

            if (TWYOilDataList == null)
                return sbAlert;
            if (TWYOilDataList.Count <= 0)
                return sbAlert;

            /*关联错误TWY(i)=TWY(i-1)+WY(i))*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i, true);//20161208修改，增加函数第3参数true,取出calShoeDate。 //string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);
                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i, true);//20161208修改，增加函数第3参数true,取出calShoeDate。  //string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i, true);
                float fTWY = 0;//TWY(i)
                float fWY = 0;//WY(i)
                if (TWYcal != string.Empty && WYcal != string.Empty 
                    &&float.TryParse(TWYcal, out fTWY) && float.TryParse(WYcal, out fWY))
                {                                   
                    if (i != 0)
                    {
                        string TWYcalbefore = getStrValuefromOilDataEntity(TWYOilDataList, i - 1);
                        float fTWYbefore = 0;//TWY(i-1)
                        if (TWYcalbefore != string.Empty && float.TryParse(TWYcalbefore, out fTWYbefore))//TWY(i-1)不为空
                        {
                            if ( Math.Abs(fTWYbefore + fWY - fTWY) > 0.001) /*关联错误TWY(i)=TWY(i-1)+WY(i))*/
                            {
                                sbAlert.Add( new returnError("TWY", CheckMetion(TWYOilDataList[0].OilTableRow.itemName, TWYOilDataList[i].OilTableRow.RowIndex, TWYOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Narrow)));
                                this._gridOil.SetRemarkFlag("TWY", i, true, GridOilColumnType.Calc);
                            }
                        }
                    }
                    else//i == 0 数据起始列
                    {
                        if (Math.Abs(fTWY - fWY) > 0.004)//20161208修改，0.001改为0.004
                        {
                            sbAlert.Add ( new returnError("TWY", CheckMetion(TWYOilDataList[0].OilTableRow.itemName, TWYOilDataList[0].OilTableRow.RowIndex, TWYOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Narrow)));
                            this._gridOil.SetRemarkFlag("TWY", i, true, GridOilColumnType.Calc);
                        }
                    }                     
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 窄馏分表VY关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowVYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            ///*关联错误VY(i)=WY(i)/D20(i)*D20(CRU)*/
            //for (int i = 0; i < this._maxCol; i++)
            //{
            //    string VYcal = this._gridOil[this._cols[i].ColumnIndex, VYRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, VYRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string D20cal = this._gridOil[this._cols[i].ColumnIndex, D20Row[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, D20Row[0].RowIndex].Value.ToString() : string.Empty;
            //    string WYcal = this._gridOil[this._cols[i].ColumnIndex, WYRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, WYRow[0].RowIndex].Value.ToString() : string.Empty;

            //    if (VYcal != string.Empty && D20cal != string.Empty && WYcal != string.Empty)
            //    {
            //        if (FunVY(WYcal, D20cal) != string.Empty)
            //        {
            //            float result = Convert.ToSingle(FunVY(WYcal, D20cal));

            //            if (Convert.ToSingle(VYcal) != result) /*关联错误VY(i)=WY(i)/D20(i)*D20(CRU)*/
            //            {
            //                sbAlert= new returnError("VY",CheckMetion("VY", VYRow[0].RowIndex, this._cols[i].colName, enumCheckErrType.Relative, EnumTableType.Narrow));
            //            }
            //        }
            //    }
            //}
            return sbAlert;
        }
        /// <summary>
        /// 窄馏分表TVY关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowTVYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");

            if (TVYOilDataList == null)
                return sbAlert;
            if (TVYOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误TVY(i)=TVY(i-1)+VY(i)*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);
                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                float fTVY = 0;//TVY(i)
                float fVY = 0;//VY(i)

                if (TVYcal != string.Empty && VYcal != string.Empty
                    && float.TryParse(TVYcal, out fTVY) && float.TryParse(VYcal, out fVY))
                {                  
                    if (i != 0)
                    {
                        float fTVYbefore = 0;//TVY(i-1)
                        string TVYcalbefore = getStrValuefromOilDataEntity(TVYOilDataList, i - 1);
                        if (TVYcalbefore != string.Empty)//TWY(i-1)不为空
                        {
                            fTVYbefore = Convert.ToSingle(TVYcalbefore);
                            if (Math.Abs( fTVYbefore + fVY-fTVY)> 0.01) /*关联错误TVY(i)=TVY(i-1)+VY(i))*/
                            {
                                sbAlert.Add ( new returnError("TVY", CheckMetion(TVYOilDataList[0].OilTableRow.itemName, TVYOilDataList[0].OilTableRow.RowIndex, TVYOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Narrow)));
                                this._gridOil.SetRemarkFlag("TVY", i, true, GridOilColumnType.Calc);
                            }
                        }
                    }
                    else//i == 0 数据起始列
                    {
                        if (Math.Abs(fTVY - fVY) > 0.01)
                        {
                            sbAlert.Add (new returnError("TVY", CheckMetion(TVYOilDataList[0].OilTableRow.itemName, TVYOilDataList[0].OilTableRow.RowIndex, TVYOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Narrow)));
                            this._gridOil.SetRemarkFlag("TVY", i, true, GridOilColumnType.Calc);
                        }
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 窄馏分表D15关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowD15LinkCheck()
        {
            return D15LinkCheck(EnumTableType.Narrow);
        }
        /// <summary>
        ///  窄馏分表V02关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowV0_LinkCheck()
        {
            return V0_LinkCheck(EnumTableType.Narrow);
        }
        /// <summary>
        /// 窄馏分表NET关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowNETLinkCheck()
        {
            return NETLinkCheck(EnumTableType.Narrow);
        }
        /// <summary>
        ///窄馏分表MEC关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowMECLinkCheck()
        {
            return MECLinkCheck(EnumTableType.Narrow); ;
        }
        /// <summary>
        /// 窄馏分表BAN关联审查
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowBANLinkCheck()
        {
            return BANLinkCheck(EnumTableType.Narrow);
        }
        /// <summary>
        /// 窄馏分表R70关联审查 R70<R20
        /// </summary>
        /// <returns></returns>
        private List<returnError> NarrowR70LinkCheck()
        {
            return R70LinkCheck(EnumTableType.Narrow);
        }


        #endregion

        #region //宽馏分关联审查
        private List<returnError> WideLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            sbAlert.AddRange(WideICPLinkCheck(EnumTableType.Wide));
            sbAlert.AddRange(WideECPLinkCheck(EnumTableType.Wide));
            sbAlert.AddRange(WideWYLinkCheck());
            sbAlert.AddRange(WideTWYLinkCheck());
            sbAlert.AddRange(WideVYLinkCheck());
            sbAlert.AddRange(WideTVYLinkCheck());
            sbAlert.AddRange(WideD15LinkCheck());
            sbAlert.AddRange(WideV0_LinkCheck());
            sbAlert.AddRange(WideR70LinkCheck());
            sbAlert.AddRange(WideH2LinkCheck());
            sbAlert.AddRange(WideN2LinkCheck());
            sbAlert.AddRange(WideC_HLinkCheck());
            sbAlert.AddRange(WideBANLinkCheck());
            sbAlert.AddRange(WideMECLinkCheck());
            sbAlert.AddRange(WideNETLinkCheck());
            sbAlert.AddRange(WideAIP_AEPLinkCheck());
            sbAlert.AddRange(WideGCTLinkCheck());
            sbAlert.AddRange(WideN2ALinkCheck());
            sbAlert.AddRange(WideIRTLinkCheck());
            sbAlert.AddRange(Wide_4CTLinkCheck());
            sbAlert.AddRange(WideM01LinkCheck());
            sbAlert.AddRange(WideMNALinkCheck());
            sbAlert.AddRange(WideMSPLinkCheck());
            sbAlert.AddRange(WideMA1LinkCheck());
            sbAlert.AddRange(WideMA2LinkCheck());
            sbAlert.AddRange(WideMA3LinkCheck());
            sbAlert.AddRange(WideMA4LinkCheck());
            sbAlert.AddRange(WideMA5LinkCheck());
            sbAlert.AddRange(WideMANLinkCheck());
            sbAlert.AddRange(WideMATLinkCheck());
            sbAlert.AddRange(WideMTALinkCheck());

            return sbAlert;
        }
 
        
        /// <summary>
        /// 宽馏分的WY关联审查 WY=SUM(WY(NCUTS(from i to j)))
        /// </summary>
        private List<returnError> WideWYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
 
            //#region
            //for (int i = 0; i < this._maxCol; i++)//宽馏分
            //{
            //    string ICPcal = this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string ECPcal = this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string WYcal = this._gridOil[this._cols[i].ColumnIndex, WYRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, WYRow[0].RowIndex].Value.ToString() : string.Empty;

            //    if (ICPcal != string.Empty && ECPcal != string.Empty && WYcal != string.Empty)
            //    {
            //        int startColumn = 0;
            //        int endColumn = 0;
            //        /*找相应的ICP和ECP值，找到ICP和ECP相等的窄馏分i和j*/
            //        for (int m = 0; m < NarrowICPList.Count; m++)
            //        {
            //            if (NarrowICPList[m].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowICPList[m].calData) == Convert.ToSingle(ICPcal))
            //                {
            //                    startColumn = NarrowICPList[m].ColumnIndex;//起始列                        
            //                }
            //            }
            //        }

            //        for (int n = 0; n < NarrowECPList.Count; n++)
            //        {
            //            if (NarrowECPList[n].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowECPList[n].calData) == Convert.ToSingle(ECPcal))
            //                {
            //                    endColumn = NarrowECPList[n].ColumnIndex;//结束列
            //                }
            //            }
            //        }
            //        float fWY = 0;//窄馏分的WY加和
            //        if (endColumn >= startColumn)
            //        {
            //            for (int j = 0; j < NarrowWYList.Count; j++)
            //            {
            //                if (NarrowWYList[j].ColumnIndex >= startColumn && NarrowWYList[j].ColumnIndex <= endColumn)
            //                {
            //                    if (NarrowWYList[j].calData != string.Empty)
            //                    {
            //                        fWY += Convert.ToSingle(NarrowWYList[j].calData);
            //                    }
            //                }
            //            }
            //        }
            //        if (Convert.ToSingle(WYcal) != fWY)
            //        {
            //            sbAlert.Append(CheckMetion("WY", WYRow[0].RowIndex, this._cols[i].colName, enumCheckErrType.Relative, EnumTableType.Wide));
            //        }

            //    }
            //}
            //#endregion
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的TWY关联审查 TWY=TWY(NCUTS(j))
        /// </summary>
        private List<returnError> WideTWYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            //#region
            //for (int i = 0; i < this._maxCol; i++)//宽馏分
            //{
            //    string ICPcal = this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string ECPcal = this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string TWYcal = this._gridOil[this._cols[i].ColumnIndex, TWYRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, TWYRow[0].RowIndex].Value.ToString() : string.Empty;

            //    if (ICPcal != string.Empty && ECPcal != string.Empty && TWYcal != string.Empty)
            //    {
            //        int startColumn = 0;
            //        int endColumn = 0;
            //        /*找相应的ICP和ECP值，找到ICP和ECP相等的窄馏分i和j*/
            //        for (int m = 0; m < NarrowICPList.Count; m++)
            //        {
            //            if (NarrowICPList[m].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowICPList[m].calData) == Convert.ToSingle(ICPcal))
            //                {
            //                    startColumn = NarrowICPList[m].ColumnIndex;//起始列                        
            //                }
            //            }
            //        }

            //        for (int n = 0; n < NarrowECPList.Count; n++)
            //        {
            //            if (NarrowECPList[n].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowECPList[n].calData) == Convert.ToSingle(ECPcal))
            //                {
            //                    endColumn = NarrowECPList[n].ColumnIndex;//结束列
            //                }
            //            }
            //        }
            //        string strTWYcal = string.Empty;
            //        if (endColumn >= startColumn)
            //        {
            //            for (int j = 0; j < NarrowTWYList.Count; j++)
            //            {
            //                if (NarrowTWYList[j].ColumnIndex == endColumn)
            //                {
            //                    if (NarrowTWYList[j].calData != string.Empty)
            //                    {
            //                        strTWYcal = NarrowTWYList[j].calData;
            //                    }
            //                }
            //            }
            //        }
            //        if (Convert.ToSingle(TWYcal) != Convert.ToSingle(strTWYcal))
            //        {
            //            sbAlert.Append(CheckMetion("TWY", TWYRow[0].RowIndex, this._cols[i].colName, enumCheckErrType.Relative, EnumTableType.Wide));
            //        }
            //    }
            //}
            //#endregion
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的VY关联审查 VY=SUM(VY(NCUTS(from i to j)))
        /// </summary>
        private List<returnError> WideVYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            //#region
            //for (int i = 0; i < this._maxCol; i++)//宽馏分
            //{
            //    string ICPcal = this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string ECPcal = this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string VYcal = this._gridOil[this._cols[i].ColumnIndex, VYRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, VYRow[0].RowIndex].Value.ToString() : string.Empty;

            //    if (ICPcal != string.Empty && ECPcal != string.Empty && VYcal != string.Empty)
            //    {
            //        int startColumn = 0;
            //        int endColumn = 0;
            //        /*找相应的ICP和ECP值，找到ICP和ECP相等的窄馏分i和j*/
            //        for (int m = 0; m < NarrowICPList.Count; m++)
            //        {
            //            if (NarrowICPList[m].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowICPList[m].calData) == Convert.ToSingle(ICPcal))
            //                {
            //                    startColumn = NarrowICPList[m].ColumnIndex;//起始列                        
            //                }
            //            }
            //        }

            //        for (int n = 0; n < NarrowECPList.Count; n++)
            //        {
            //            if (NarrowECPList[n].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowECPList[n].calData) == Convert.ToSingle(ECPcal))
            //                {
            //                    endColumn = NarrowECPList[n].ColumnIndex;//结束列
            //                }
            //            }
            //        }
            //        float fVY = 0;
            //        if (endColumn >= startColumn)
            //        {
            //            for (int j = 0; j < NarrowVYList.Count; j++)
            //            {
            //                if (NarrowVYList[j].ColumnIndex >= startColumn && NarrowVYList[j].ColumnIndex <= endColumn)
            //                {
            //                    if (NarrowVYList[j].calData != string.Empty)
            //                    {
            //                        fVY += Convert.ToSingle(NarrowVYList[j].calData);
            //                    }
            //                }
            //            }
            //        }
            //        if (Convert.ToSingle(VYcal) != fVY)
            //        {
            //            sbAlert.Append(CheckMetion("VY", VYRow[0].RowIndex, this._cols[i].colName, enumCheckErrType.Relative, EnumTableType.Wide));
            //        }
            //    }
            //}
            //#endregion
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的TVY关联审查 TVY=TVY(NCUTS(j))
        /// </summary>
        private List<returnError> WideTVYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            //#region
            //for (int i = 0; i < this._maxCol; i++)//宽馏分
            //{
            //    string ICPcal = this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string ECPcal = this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string TVYcal = this._gridOil[this._cols[i].ColumnIndex, TVYRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, TVYRow[0].RowIndex].Value.ToString() : string.Empty;

            //    if (ICPcal != string.Empty && ECPcal != string.Empty && TVYcal != string.Empty)
            //    {
            //        int startColumn = 0;
            //        int endColumn = 0;
            //        /*找相应的ICP和ECP值，找到ICP和ECP相等的窄馏分i和j*/
            //        for (int m = 0; m < NarrowICPList.Count; m++)
            //        {
            //            if (NarrowICPList[m].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowICPList[m].calData) == Convert.ToSingle(ICPcal))
            //                {
            //                    startColumn = NarrowICPList[m].ColumnIndex;//起始列                        
            //                }
            //            }
            //        }

            //        for (int n = 0; n < NarrowECPList.Count; n++)
            //        {
            //            if (NarrowECPList[n].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowECPList[n].calData) == Convert.ToSingle(ECPcal))
            //                {
            //                    endColumn = NarrowECPList[n].ColumnIndex;//结束列
            //                }
            //            }
            //        }
            //        string strTVYcal = string.Empty;
            //        if (endColumn >= startColumn)
            //        {
            //            for (int j = 0; j < NarrowTVYList.Count; j++)
            //            {
            //                if (NarrowTVYList[j].ColumnIndex == endColumn)
            //                {
            //                    if (NarrowTVYList[j].calData != string.Empty)
            //                    {
            //                        strTVYcal = NarrowTVYList[j].calData;
            //                    }
            //                }
            //            }
            //        }
            //        if (strTVYcal != string.Empty)
            //        {
            //            if (Convert.ToSingle(TVYcal) != Convert.ToSingle(strTVYcal))
            //            {
            //                sbAlert.Append(CheckMetion("TVY", TVYRow[0].RowIndex, this._cols[i].colName, enumCheckErrType.Relative, EnumTableType.Wide));
            //            }
            //        }
            //    }
            //}
            //#endregion
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的D15关联审查 D15>D20
        /// </summary>
        private List<returnError> WideD15LinkCheck()
        {
            return D15LinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的V02关联审查 v02>v04>v05>v08>v10
        /// </summary>
        private List<returnError> WideV0_LinkCheck()
        {
            return V0_LinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的R70关联审查  R70<R20
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideR70LinkCheck()
        {
            return R70LinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的H2关联审查  CAR>H2
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideH2LinkCheck()
        {
            return H2LinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的N2关联审查 H2>N2
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideN2LinkCheck()
        {
            return N2LinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的C/H关联审查  C/H>1
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideC_HLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            List<OilDataEntity> C_HOilDataList = this._gridOil.GetDataByRowItemCode("C/H");
            if (C_HOilDataList == null)
                return sbAlert;
            if (C_HOilDataList.Count <= 0)
                return sbAlert;

            /*关联错误CAR>H2*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string C_Hcal = getStrValuefromOilDataEntity(C_HOilDataList, i);
                float fC_H = 0;// 
                if (C_Hcal != string.Empty && float.TryParse(C_Hcal, out fC_H))
                {                 
                    if (fC_H <= 1)   /*关联错误CAR>H2*/
                    {
                        sbAlert.Add ( new returnError("C/H", CheckMetion(C_HOilDataList[0].OilTableRow.itemName, C_HOilDataList[0].OilTableRow.RowIndex, C_HOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("C/H", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的BAN关联审查  BAN<N2
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideBANLinkCheck()
        {
            return BANLinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的MEC关联审查  MEC<SUL
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMECLinkCheck()
        {
            return MECLinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的NET关联审查  ACD>NET
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideNETLinkCheck()
        {
            return NETLinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的AIP关联审查  AIP<A10<A30<A50<A70<A90<A95<AEP
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideAIP_AEPLinkCheck()
        {
            return AIP_AEPLinkCheck(EnumTableType.Wide);
        }
        /// <summary>
        /// 宽馏分的GCT关联审查 ABS(GCT-100)<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideGCTLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> GCTOilDataList = this._gridOil.GetDataByRowItemCode("GCT");
            if (GCTOilDataList == null)
                return sbAlert;
            if (GCTOilDataList.Count <= 0)
                return sbAlert;

            /*关联错误ABS(GCT-100)<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string GCTcal = getStrValuefromOilDataEntity(GCTOilDataList, i);
                float fGCT = 0;// 
                if (GCTcal != string.Empty && float.TryParse(GCTcal, out fGCT))
                {                 
                    if (Math.Abs(fGCT - 100) >= 0.02)   /*关联错误ABS(GCT-100)<0.02*/
                    {
                        sbAlert.Add(new returnError("GCT", CheckMetion(GCTOilDataList[0].OilTableRow.itemName, GCTOilDataList[0].OilTableRow.RowIndex, GCTOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("GCT", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的N2A关联审查ABS(N2A-NAH-2*ARM)<0.1
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideN2ALinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> N2AOilDataList = this._gridOil.GetDataByRowItemCode("N2A");
            List<OilDataEntity> NAHOilDataList = this._gridOil.GetDataByRowItemCode("NAH");
            List<OilDataEntity> ARMOilDataList = this._gridOil.GetDataByRowItemCode("ARM");
            if (N2AOilDataList == null)
                return sbAlert;
            if (N2AOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(N2A-NAH-2*ARM)<0.1*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string N2Acal = getStrValuefromOilDataEntity(N2AOilDataList, i);
                string NAHcal = getStrValuefromOilDataEntity(NAHOilDataList, i);
                string ARMcal = getStrValuefromOilDataEntity(ARMOilDataList, i);
                float fN2A = 0;// 
                float fNAH = 0;// 
                float fARM = 0;// 
                if (N2Acal != string.Empty && NAHcal != string.Empty && ARMcal != string.Empty
                   && float.TryParse(N2Acal, out fN2A) && float.TryParse(NAHcal, out fNAH) && float.TryParse(ARMcal, out fARM))                 
                {                  
                    /*关联错误ABS(N2A-NAH-2*ARM)<0.1*/
                    if (Math.Abs(fN2A - fNAH - 2 * fARM) >= 0.1)
                    {
                        sbAlert.Add (new returnError("N2A", CheckMetion(N2AOilDataList[0].OilTableRow.itemName, N2AOilDataList[0].OilTableRow.RowIndex, N2AOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("N2A", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的IRT关联审查ABS(IRT-100)<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideIRTLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> IRTOilDataList = this._gridOil.GetDataByRowItemCode("IRT");
            if (IRTOilDataList == null)
                return sbAlert;
            if (IRTOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(IRT-100)<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string IRTcal = getStrValuefromOilDataEntity(IRTOilDataList, i);
                float fIRT =0;// 
                if (IRTcal != string.Empty && float.TryParse(IRTcal, out fIRT)) 
                {                  
                    /*关联错误ABS(IRT-100)<0.02*/
                    if (Math.Abs(fIRT - 100) >= 0.02)
                    {
                        sbAlert.Add ( new returnError("IRT", CheckMetion(IRTOilDataList[0].OilTableRow.itemName, IRTOilDataList[0].OilTableRow.RowIndex, IRTOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("IRT", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的4CT关联审查ABS(4CT-100)<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> Wide_4CTLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> _4CTOilDataList = this._gridOil.GetDataByRowItemCode("4CT");
            if (_4CTOilDataList == null)
                return sbAlert;
            if (_4CTOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(4CT-100)<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string _4CTcal = getStrValuefromOilDataEntity(_4CTOilDataList, i);
                float f_4CT = 0;// 

                if (_4CTcal != string.Empty && float.TryParse(_4CTcal, out f_4CT)) 
                {                  
                    /*关联错误ABS(4CT-100)<0.02*/
                    if (Math.Abs(f_4CT - 100) >= 0.02)
                    {
                        sbAlert.Add ( new returnError("4CT", CheckMetion(_4CTOilDataList[0].OilTableRow.itemName, _4CTOilDataList[0].OilTableRow.RowIndex, _4CTOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("4CT", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的M01关联审查  M01<MSP
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideM01LinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> M01OilDataList = this._gridOil.GetDataByRowItemCode("M01");
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");
            if (M01OilDataList == null)
                return sbAlert;
            if (M01OilDataList.Count <= 0)
                return sbAlert;
            /*关联错误M01<MSP*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M01cal = getStrValuefromOilDataEntity(M01OilDataList, i);
                string MSPcal = getStrValuefromOilDataEntity(MSPOilDataList, i);

                float fM01 =0;// 
                float fMSP = 0;// 
                if (M01cal != string.Empty && MSPcal != string.Empty
                    && float.TryParse(M01cal, out fM01) && float.TryParse(MSPcal, out fMSP)
                    )
                {
                    if (fM01 >= fMSP)  /*关联错误M01<MSP*/
                    {
                        sbAlert.Add ( new returnError("M01", CheckMetion(M01OilDataList[0].OilTableRow.itemName, M01OilDataList[0].OilTableRow.RowIndex, M01OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("M01", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MNA关联审查ABS(MNA-SUM(M02:M07))<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMNALinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MNAOilDataList = this._gridOil.GetDataByRowItemCode("MNA");
            List<OilDataEntity> M02OilDataList = this._gridOil.GetDataByRowItemCode("M02");
            List<OilDataEntity> M03OilDataList = this._gridOil.GetDataByRowItemCode("M03");
            List<OilDataEntity> M04OilDataList = this._gridOil.GetDataByRowItemCode("M04");
            List<OilDataEntity> M05OilDataList = this._gridOil.GetDataByRowItemCode("M05");
            List<OilDataEntity> M06OilDataList = this._gridOil.GetDataByRowItemCode("M06");
            List<OilDataEntity> M07OilDataList = this._gridOil.GetDataByRowItemCode("M07");
            if (MNAOilDataList == null)
                return sbAlert;
            if (MNAOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MNA-SUM(M02:M07))<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M02cal = getStrValuefromOilDataEntity(M02OilDataList, i);
                string M03cal = getStrValuefromOilDataEntity(M03OilDataList, i);
                string M04cal = getStrValuefromOilDataEntity(M04OilDataList, i);
                string M05cal = getStrValuefromOilDataEntity(M05OilDataList, i);
                string M06cal = getStrValuefromOilDataEntity(M06OilDataList, i);
                string M07cal = getStrValuefromOilDataEntity(M07OilDataList, i);
                string MNAcal = getStrValuefromOilDataEntity(MNAOilDataList, i);

                float fM02 = 0;// 
                float fM03 = 0;// 
                float fM04 = 0;// 
                float fM05 = 0;// 
                float fM06 = 0;// 
                float fM07 = 0;// 
                float fMNA = 0;// 
                if (M02cal != string.Empty && M03cal != string.Empty && M04cal != string.Empty
                    && M05cal != string.Empty && M06cal != string.Empty && M07cal != string.Empty && MNAcal != string.Empty
                    && float.TryParse(M02cal, out fM02) && float.TryParse(M03cal, out fM03)
                     && float.TryParse(M04cal, out fM04) && float.TryParse(M05cal, out fM05)
                      && float.TryParse(M06cal, out fM06) && float.TryParse(M07cal, out fM07)
                         && float.TryParse(MNAcal, out fMNA))   
                {
                    float sum = fM02 + fM03 + fM04 + fM05 + fM06 + fM07;

                    /*关联错误ABS(MNA-SUM(M02:M07))<0.02*/
                    if (Math.Abs(fMNA - sum) >= 0.02)
                    {
                        sbAlert .Add (new returnError("MNA", CheckMetion(MNAOilDataList[0].OilTableRow.itemName, MNAOilDataList[0].OilTableRow.RowIndex, MNAOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MNA", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MSP关联审查ABS(MSP-MNA-M01)<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMSPLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");
            List<OilDataEntity> MNAOilDataList = this._gridOil.GetDataByRowItemCode("MNA");
            List<OilDataEntity> M01OilDataList = this._gridOil.GetDataByRowItemCode("M01");

            if (MSPOilDataList == null)
                return sbAlert;
            if (MSPOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MSP-MNA-M01)<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M01cal = getStrValuefromOilDataEntity(M01OilDataList, i);
                string MSPcal = getStrValuefromOilDataEntity(MSPOilDataList, i);              
                string MNAcal = getStrValuefromOilDataEntity(MNAOilDataList, i);

                float fMSP = 0;// 
                float fM01 = 0;// 
                float fMNA = 0;// 

                if (MSPcal != string.Empty && M01cal != string.Empty && MNAcal != string.Empty
                      && float.TryParse(MSPcal, out fMSP) && float.TryParse(M01cal, out fM01)
                         && float.TryParse(MNAcal, out fMNA))                  
                {
                    /*关联错误ABS(MSP-MNA-M01)<0.02*/
                    if (Math.Abs(fMSP - fMNA - fM01) >= 0.02)
                    {
                        sbAlert.Add (new returnError("MSP", CheckMetion(MSPOilDataList[0].OilTableRow.itemName, MSPOilDataList[0].OilTableRow.RowIndex, MSPOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MSP", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MA1关联审查ABS(MA1-SUM(M08:M12))<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMA1LinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MA1OilDataList = this._gridOil.GetDataByRowItemCode("MA1");
            List<OilDataEntity> M08OilDataList = this._gridOil.GetDataByRowItemCode("M08");
            List<OilDataEntity> M09OilDataList = this._gridOil.GetDataByRowItemCode("M09");
            List<OilDataEntity> M10OilDataList = this._gridOil.GetDataByRowItemCode("M10");
            List<OilDataEntity> M11OilDataList = this._gridOil.GetDataByRowItemCode("M11");
            List<OilDataEntity> M12OilDataList = this._gridOil.GetDataByRowItemCode("M12");

            if (MA1OilDataList == null)
                return sbAlert;
            if (MA1OilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MA1-SUM(M08:M12))<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M08cal = getStrValuefromOilDataEntity(M08OilDataList, i);
                string M09cal = getStrValuefromOilDataEntity(M09OilDataList, i);
                string M10cal = getStrValuefromOilDataEntity(M10OilDataList, i);
                string M11cal = getStrValuefromOilDataEntity(M11OilDataList, i);
                string M12cal = getStrValuefromOilDataEntity(M12OilDataList, i);
                string MA1cal = getStrValuefromOilDataEntity(MA1OilDataList, i);

                float fM08 = 0;// 
                float fM09 = 0;// 
                float fM10 = 0;// 
                float fM11 = 0;// 
                float fM12 = 0;// 
                float fMA1 = 0;// 
                if (M08cal != string.Empty && M09cal != string.Empty && M10cal != string.Empty 
                    && M11cal != string.Empty && M12cal != string.Empty && MA1cal != string.Empty
                     && float.TryParse(M08cal, out fM08) && float.TryParse(M09cal, out fM09)
                         && float.TryParse(M10cal, out fM10) && float.TryParse(M11cal, out fM11)
                     && float.TryParse(M12cal, out fM12) && float.TryParse(MA1cal, out fMA1)
                    )
                {
                    float sum = fM08 + fM09 + fM10 + fM11 + fM12;

                    /*关联错误ABS(MA1-SUM(M08:M12))<0.02*/
                    if (Math.Abs(fMA1 - sum) >= 0.02)
                    {
                        sbAlert.Add ( new returnError("MA1", CheckMetion(MA1OilDataList[0].OilTableRow.itemName, MA1OilDataList[0].OilTableRow.RowIndex, MA1OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MA1", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MA2关联审查ABS(MA2-SUM(M13:M18))<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMA2LinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MA2OilDataList = this._gridOil.GetDataByRowItemCode("MA2");
            List<OilDataEntity> M13OilDataList = this._gridOil.GetDataByRowItemCode("M13");
            List<OilDataEntity> M14OilDataList = this._gridOil.GetDataByRowItemCode("M14");
            List<OilDataEntity> M15OilDataList = this._gridOil.GetDataByRowItemCode("M15");
            List<OilDataEntity> M16OilDataList = this._gridOil.GetDataByRowItemCode("M16");
            List<OilDataEntity> M17OilDataList = this._gridOil.GetDataByRowItemCode("M17");
            List<OilDataEntity> M18OilDataList = this._gridOil.GetDataByRowItemCode("M18");
            if (MA2OilDataList == null)
                return sbAlert;
            if (MA2OilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MA2-SUM(M13:M18))<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M13cal = getStrValuefromOilDataEntity(M13OilDataList, i);
                string M14cal = getStrValuefromOilDataEntity(M14OilDataList, i);
                string M15cal = getStrValuefromOilDataEntity(M15OilDataList, i);
                string M16cal = getStrValuefromOilDataEntity(M16OilDataList, i);
                string M17cal = getStrValuefromOilDataEntity(M17OilDataList, i);
                string M18cal = getStrValuefromOilDataEntity(M18OilDataList, i);
                string MA2cal = getStrValuefromOilDataEntity(MA2OilDataList, i);

                float fM13 = 0;// 
                float fM14 = 0;// 
                float fM15 = 0;// 
                float fM16 = 0;// 
                float fM17 = 0;// 
                float fM18 = 0;// 
                float fMA2 = 0;// 
                if (M13cal != string.Empty && M14cal != string.Empty && M15cal != string.Empty 
                    && M16cal != string.Empty && M17cal != string.Empty && M18cal != string.Empty && MA2cal != string.Empty
                      && float.TryParse(M13cal, out fM13) && float.TryParse(M14cal, out fM14)
                         && float.TryParse(M15cal, out fM15) && float.TryParse(M16cal, out fM16)
                            && float.TryParse(M17cal, out fM17) && float.TryParse(M18cal, out fM18)
                    && float.TryParse(MA2cal, out fMA2))
                {
                    float sum = fM13 + fM14 + fM15 + fM16 + fM17 + fM18;

                    /*关联错误ABS(MA2-SUM(M13:M18))<0.02*/
                    if (Math.Abs(fMA2 - sum) >= 0.02)
                    {
                        sbAlert.Add (new returnError("MA2", CheckMetion(MA2OilDataList[0].OilTableRow.itemName, MA2OilDataList[0].OilTableRow.RowIndex, MA2OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MA2", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MA3关联审查ABS(MA3-SUM(M19:M20))<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMA3LinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MA3OilDataList = this._gridOil.GetDataByRowItemCode("MA3");
            List<OilDataEntity> M19OilDataList = this._gridOil.GetDataByRowItemCode("M19");
            List<OilDataEntity> M20OilDataList = this._gridOil.GetDataByRowItemCode("M20");

            if (MA3OilDataList == null)
                return sbAlert;
            if (MA3OilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MA3-SUM(M19:M20))<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M19cal = getStrValuefromOilDataEntity(M19OilDataList, i);
                string M20cal = getStrValuefromOilDataEntity(M20OilDataList, i);
                string MA3cal = getStrValuefromOilDataEntity(MA3OilDataList, i);
                float fM19 = 0;// 
                float fM20 = 0;//           
                float fMA3 = 0;// 
                if (M19cal != string.Empty && M20cal != string.Empty && MA3cal != string.Empty
                     && float.TryParse(M19cal, out fM19) && float.TryParse(M20cal, out fM20)
                    && float.TryParse(MA3cal, out fMA3))
                {
                    float sum = fM19 + fM20;

                    /*关联错误ABS(MA3-SUM(M19:M20))<0.02*/
                    if (Math.Abs(fMA3 - sum) >= 0.02)
                    {
                        sbAlert.Add (new returnError("MA3", CheckMetion(MA3OilDataList[0].OilTableRow.itemName, MA3OilDataList[0].OilTableRow.RowIndex, MA3OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MA3", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MA4关联审查ABS(MA4-SUM(M21:M22))<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMA4LinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MA4OilDataList = this._gridOil.GetDataByRowItemCode("MA4");
            List<OilDataEntity> M21OilDataList = this._gridOil.GetDataByRowItemCode("M21");
            List<OilDataEntity> M22OilDataList = this._gridOil.GetDataByRowItemCode("M22");

            if (MA4OilDataList == null)
                return sbAlert;
            if (MA4OilDataList.Count <= 0)
                return sbAlert;

            /*关联错误ABS(MA4-SUM(M21:M22))<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M21cal = getStrValuefromOilDataEntity(M21OilDataList, i);
                string M22cal = getStrValuefromOilDataEntity(M22OilDataList, i);
                string MA4cal = getStrValuefromOilDataEntity(MA4OilDataList, i);
                float fM21 = 0;// 
                float fM22 = 0;//                  
                float fMA4 = 0;// 
                if (M21cal != string.Empty && M22cal != string.Empty && MA4cal != string.Empty
                     && float.TryParse(M21cal, out fM21) && float.TryParse(M22cal, out fM22)
                    && float.TryParse(MA4cal, out fMA4))
                {
                  
                    float sum = fM21 + fM22;

                    /*关联错误ABS(MA4-SUM(M21:M22))<0.02*/
                    if (Math.Abs(fMA4 - sum) >= 0.02)
                    {
                        sbAlert.Add ( new returnError("MA4", CheckMetion(MA4OilDataList[0].OilTableRow.itemName, MA4OilDataList[0].OilTableRow.RowIndex, MA4OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MA4", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MA5关联审查ABS(MA5-SUM(M23:M24))<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMA5LinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MA5OilDataList = this._gridOil.GetDataByRowItemCode("MA5");
            List<OilDataEntity> M23OilDataList = this._gridOil.GetDataByRowItemCode("M23");
            List<OilDataEntity> M24OilDataList = this._gridOil.GetDataByRowItemCode("M24");

            if (MA5OilDataList == null)
                return sbAlert;
            if (MA5OilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MA5-SUM(M23:M24))<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M23cal = getStrValuefromOilDataEntity(M23OilDataList, i);
                string M24cal = getStrValuefromOilDataEntity(M24OilDataList, i);
                string MA5cal = getStrValuefromOilDataEntity(MA5OilDataList, i);
                float fM23 = 0;// 
                float fM24 = 0;//                  
                float fMA5 = 0;// 
                if (M23cal != string.Empty && M24cal != string.Empty && MA5cal != string.Empty
                   && float.TryParse(M23cal, out fM23) && float.TryParse(M24cal, out fM24)
                    && float.TryParse(MA5cal, out fMA5))
                {
                   
                    float sum = fM23 + fM24;

                    /*关联错误ABS(MA5-SUM(M23:M24))<0.02*/
                    if (Math.Abs(fMA5 - sum) >= 0.02)
                    {
                        sbAlert.Add ( new returnError("MA5", CheckMetion(MA5OilDataList[0].OilTableRow.itemName, MA5OilDataList[0].OilTableRow.RowIndex, MA5OilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MA5", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MAN关联审查ABS(MAN-SUM(M26:M28))<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMANLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MANOilDataList = this._gridOil.GetDataByRowItemCode("MAN");
            List<OilDataEntity> M26OilDataList = this._gridOil.GetDataByRowItemCode("M26");
            List<OilDataEntity> M27OilDataList = this._gridOil.GetDataByRowItemCode("M27");
            List<OilDataEntity> M28OilDataList = this._gridOil.GetDataByRowItemCode("M28");

            if (MANOilDataList == null)
                return sbAlert;
            if (MANOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MAN-SUM(M26:M28))<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string M26cal = getStrValuefromOilDataEntity(M26OilDataList, i);
                string M27cal = getStrValuefromOilDataEntity(M27OilDataList, i);
                string M28cal = getStrValuefromOilDataEntity(M28OilDataList, i);
                string MANcal = getStrValuefromOilDataEntity(MANOilDataList, i);

                float fM26 = 0;// 
                float fM27 = 0;// 
                float fM28 = 0;// 
                float fMAN = 0;// 
                if (M26cal != string.Empty && M27cal != string.Empty && M28cal != string.Empty && MANcal != string.Empty
                     && float.TryParse(M26cal, out fM26) && float.TryParse(M27cal, out fM27)
                    && float.TryParse(M28cal, out fM28) && float.TryParse(MANcal, out fMAN))
                {
                  
                    float sum = fM26 + fM27 + fM28;

                    /*关联错误ABS(MAN-SUM(M26:M28))<0.02*/
                    if (Math.Abs(fMAN - sum) >= 0.02)
                    {
                        sbAlert.Add ( new returnError("MAN", CheckMetion(MANOilDataList[0].OilTableRow.itemName, MANOilDataList[0].OilTableRow.RowIndex, MANOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MAN", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MAT关联审查ABS(MAT-(MA1+MA2+MA3+MA4+MA5))<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMATLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");
            List<OilDataEntity> MA1OilDataList = this._gridOil.GetDataByRowItemCode("MA1");
            List<OilDataEntity> MA2OilDataList = this._gridOil.GetDataByRowItemCode("MA2");
            List<OilDataEntity> MA3OilDataList = this._gridOil.GetDataByRowItemCode("MA3");
            List<OilDataEntity> MA4OilDataList = this._gridOil.GetDataByRowItemCode("MA4");
            List<OilDataEntity> MA5OilDataList = this._gridOil.GetDataByRowItemCode("MA5");
            List<OilDataEntity> MANOilDataList = this._gridOil.GetDataByRowItemCode("MAN");
            List<OilDataEntity> MAUOilDataList = this._gridOil.GetDataByRowItemCode("MAU");

            if (MATOilDataList == null)
                return sbAlert;
            if (MATOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MAT-(MA1+MA2+MA3+MA4+MA5))<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string MA1cal = getStrValuefromOilDataEntity(MA1OilDataList, i);
                string MA2cal = getStrValuefromOilDataEntity(MA2OilDataList, i);
                string MA3cal = getStrValuefromOilDataEntity(MA3OilDataList, i);
                string MA4cal = getStrValuefromOilDataEntity(MA4OilDataList, i);
                string MA5cal = getStrValuefromOilDataEntity(MA5OilDataList, i);
                string MATcal = getStrValuefromOilDataEntity(MATOilDataList, i);

                string MANcal = getStrValuefromOilDataEntity(MANOilDataList, i);
                string MAUcal = getStrValuefromOilDataEntity(MAUOilDataList, i);

                List<string> tempList = new List<string>();
                tempList.Add(MA1cal);
                tempList.Add(MA2cal);
                tempList.Add(MA3cal);
                tempList.Add(MA4cal);
                tempList.Add(MA5cal);
                tempList.Add(MANcal);
                tempList.Add(MAUcal);
                string tempMATcal = BaseFunction.FunSumAllowEmpty(tempList);

                float fMAT = 0, tempfMAT = 0;// 
                if (MA1cal != string.Empty && float.TryParse(MATcal, out fMAT)
                    && float.TryParse(tempMATcal, out tempfMAT)
                    )
                {                  
                    /*关联错误ABS(MAT-(MA1+MA2+MA3+MA4+MA5+MAU+MAN))<0.02*/
                    if (Math.Abs(fMAT - tempfMAT) >= 0.02)
                    {
                        sbAlert.Add (new returnError("MAT",CheckMetion(MATOilDataList[0].OilTableRow.itemName, MATOilDataList[0].OilTableRow.RowIndex, MATOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MAT", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分的MTA关联审查ABS(MTA-MAT-MRS-MSP)<0.02
        /// </summary>
        /// <returns></returns>
        private List<returnError> WideMTALinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> MTAOilDataList = this._gridOil.GetDataByRowItemCode("MTA");
            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");
            List<OilDataEntity> MRSOilDataList = this._gridOil.GetDataByRowItemCode("MRS");
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");
            if (MTAOilDataList == null)
                return sbAlert;
            if (MTAOilDataList.Count <= 0)
                return sbAlert;
            /*关联错误ABS(MTA-MAT-MRS-MSP)<0.02*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string MATcal = getStrValuefromOilDataEntity(MATOilDataList, i);
                string MRScal = getStrValuefromOilDataEntity(MRSOilDataList, i);
                string MSPcal = getStrValuefromOilDataEntity(MSPOilDataList, i);
                string MTAcal = getStrValuefromOilDataEntity(MTAOilDataList, i);

                float fMAT = 0;// 
                float fMRS = 0;// 
                float fMSP = 0;// 
                float fMTA = 0;//
                if (MATcal != string.Empty && MRScal != string.Empty && MSPcal != string.Empty && MTAcal != string.Empty
                   && float.TryParse(MATcal, out fMAT) && float.TryParse(MRScal, out fMRS)
                   && float.TryParse(MSPcal, out fMSP) && float.TryParse(MTAcal, out fMTA))
                {
                    float sum = fMSP + fMRS + fMAT;

                    /*关联错误ABS(MTA-MAT-MRS-MSP)<0.02*/
                    if (Math.Abs(fMTA - sum) >= 0.02)
                    {
                        sbAlert .Add (new returnError("MTA", CheckMetion(MTAOilDataList[0].OilTableRow.itemName, MTAOilDataList[0].OilTableRow.RowIndex, MTAOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Wide)));
                        this._gridOil.SetRemarkFlag("MTA", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert;
        }

        #endregion

        #region //渣油关联审查
        private List<returnError> ResidueLinkCheck()
        {
            List<returnError> sbAlerts = new List<returnError>();
            sbAlerts.AddRange(ResidueICPLinkCheck());
            sbAlerts.AddRange(ResidueWYLinkCheck());
            sbAlerts.AddRange(ResidueTWYLinkCheck());
            sbAlerts.AddRange(ResidueVYLinkCheck());
            sbAlerts.AddRange(ResidueTVYLinkCheck());
            sbAlerts.AddRange(ResidueD15LinkCheck());
            sbAlerts.AddRange(ResidueV0_LinkCheck());
            sbAlerts.AddRange(ResidueBANLinkCheck());
            sbAlerts.AddRange(ResidueH2LinkCheck());
            sbAlerts.AddRange(ResidueSULLinkCheck());
            sbAlerts.AddRange(ResidueN2LinkCheck());
            sbAlerts.AddRange(ResidueAPHLinkCheck());
            sbAlerts.AddRange(ResidueAIP_AEPLinkCheck());
            sbAlerts.AddRange(ResidueRTTLinkCheck());

            return sbAlerts;
        }

        /// <summary>
        /// 渣油ICP关联
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueICPLinkCheck()
        {
            return WideECPLinkCheck(EnumTableType.Residue);
        }
        /// <summary>
        /// 渣油WY关联WY<=100-TWY(NCUTS(NCUTS.ECP=RES.ICP))
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueWYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();

            //#region 
            ///*关联错误TVY>VY*/
            //for (int i = 0; i < this._maxCol; i++)
            //{
            //    string ICPcal = this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value.ToString() : string.Empty;
            //    string WYcal = this._gridOil[this._cols[i].ColumnIndex, WYRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, WYRow[0].RowIndex].Value.ToString() : string.Empty;

            //    if (WYcal != string.Empty && ICPcal != string.Empty)
            //    {
            //        int endColumn = 0;
            //        /*找相应的ICP和ECP值，找到ICP和ECP相等的窄馏分i和j*/

            //        for (int n = 0; n < NarrowECPList.Count; n++)
            //        {
            //            if (NarrowECPList[n].calData != string.Empty)
            //            {
            //                if (Convert.ToSingle(NarrowECPList[n].calData) == Convert.ToSingle(ICPcal))
            //                {
            //                    endColumn = NarrowECPList[n].ColumnIndex;//结束列
            //                }
            //            }
            //        }
            //        string strTWYcal = string.Empty;

            //        for (int j = 0; j < NarrowTWYList.Count; j++)
            //        {
            //            if (NarrowTWYList[j].ColumnIndex == endColumn)
            //            {
            //                if (NarrowTWYList[j].calData != string.Empty)
            //                {
            //                    strTWYcal = NarrowTWYList[j].calData;
            //                }
            //            }
            //        }

            //        float TWY = 0;// 
            //        float WY = 0;
            //        if (strTWYcal != string.Empty)
            //        {
            //            TWY = Convert.ToSingle(strTWYcal);// 
            //        }
            //        WY = Convert.ToSingle(WYcal);// 


            //        /*关联错误TVY>VY*/
            //        if (!(WY <= 100 - TWY))
            //        {
            //            sbAlert= new returnError("",CheckMetion("WY", WYRow[0].RowIndex, this._cols[i].colName, enumCheckErrType.Relative, EnumTableType.Wide));
            //        }
            //    }
            //}
            //#endregion 
            return sbAlert ;
        }
        /// <summary>
        /// 渣油TWY关联TWY>WY
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueTWYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            if (TWYOilDataList == null)
                return sbAlert ;
            if (TWYOilDataList.Count <= 0)
                return sbAlert ;

            /*关联错误 TWY>WY*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);
                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);

                float fTWY = 0;
                float fWY = 0;

                if (TWYcal != string.Empty && WYcal != string.Empty
                    && float.TryParse(TWYcal, out fTWY) && float.TryParse(WYcal, out fWY))
                {                 
                    /*关联错误ABS(MNA-SUM(M02:M07))<0.02*/
                    if (!(fTWY > fWY))
                    {
                        sbAlert.Add (new returnError("TWY", CheckMetion(TWYOilDataList[0].OilTableRow.itemName, TWYOilDataList[0].OilTableRow.RowIndex, TWYOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Residue)));
                        this._gridOil.SetRemarkFlag("TWY", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert ;
        }
        /// <summary>
        /// 渣油VY关联VY<WY
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueVYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");

            if (VYOilDataList == null)
                return sbAlert ;
            if (VYOilDataList.Count <= 0)
                return sbAlert ;

            /*关联错误 TWY>WY*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                float fVY = 0;
                float fWY = 0;
                if (VYcal != string.Empty && WYcal != string.Empty
                    && float.TryParse(VYcal, out fVY) && float.TryParse(WYcal, out fWY))
                {                
                    /*关联错误ABS(MNA-SUM(M02:M07))<0.02*/
                    if (!(fVY < fWY))
                    {
                        sbAlert.Add ( new returnError("VY", CheckMetion(VYOilDataList[0].OilTableRow.itemName, VYOilDataList[0].OilTableRow.RowIndex, VYOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Residue)));
                        this._gridOil.SetRemarkFlag("VY", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert ;
        }
        /// <summary>
        /// 渣油TVY关联TVY>VY
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueTVYLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            if (TVYOilDataList == null)
                return sbAlert ;
            if (TVYOilDataList.Count <= 0)
                return sbAlert ;

            /*关联错误TVY>VY*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);

                float fVY = 0;
                float fTVY = 0;

                if (VYcal != string.Empty && TVYcal != string.Empty
                  && float.TryParse(VYcal, out fVY) && float.TryParse(TVYcal, out fTVY))
                {            
                    /*关联错误TVY>VY*/
                    if (!(fTVY > fVY))
                    {
                        sbAlert.Add (new returnError("TVY", CheckMetion(TVYOilDataList[0].OilTableRow.itemName, TVYOilDataList[0].OilTableRow.RowIndex, TVYOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Residue)));
                        this._gridOil.SetRemarkFlag("TVY", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert ;
        }
        /// <summary>
        /// 渣油D15关联D15>D20
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueD15LinkCheck()
        {
            return D15LinkCheck(EnumTableType.Residue);
        }
        /// <summary>
        /// 渣油关联v02>v04>v05>v08>v10
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueV0_LinkCheck()
        {
            return V0_LinkCheck(EnumTableType.Residue);
        }
        /// <summary>
        /// 渣油BAN关联 BAN<N2
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueBANLinkCheck()
        {
            return BANLinkCheck(EnumTableType.Residue);
        }
        /// <summary>
        /// 渣油H2关联 CAR>H2
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueH2LinkCheck()
        {
            return H2LinkCheck(EnumTableType.Residue);
        }
        /// <summary>
        /// 渣油SUL关联 H2>SUL
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueSULLinkCheck()
        {
            return SULLinkCheck(EnumTableType.Residue);
        }
        /// <summary>
        /// 渣油N2关联 H2>N2
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueN2LinkCheck()
        {
            return N2LinkCheck(EnumTableType.Residue);
        }
        /// <summary>
        /// 渣油APH关联 ABS(SAH+RES+ARS+APH-100)<0.1
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueAPHLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> SAHOilDataList = this._gridOil.GetDataByRowItemCode("SAH");
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");
            List<OilDataEntity> ARSOilDataList = this._gridOil.GetDataByRowItemCode("ARS");
            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
            if (APHOilDataList == null)
                return sbAlert ;
            if (APHOilDataList.Count <= 0)
                return sbAlert ;

            /*关联错误ABS(SAH+RES+ARS+APH-100)<0.1*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string SAHcal = getStrValuefromOilDataEntity(SAHOilDataList, i);
                string REScal = getStrValuefromOilDataEntity(RESOilDataList, i);
                string ARScal = getStrValuefromOilDataEntity(ARSOilDataList, i);
                string APHcal = getStrValuefromOilDataEntity(APHOilDataList, i);

                float fSAH = 0;
                float fRES = 0;
                float fARS = 0;
                float fAPH = 0;
                if (SAHcal != string.Empty && REScal != string.Empty && ARScal != string.Empty && APHcal != string.Empty
                   && float.TryParse(SAHcal, out fSAH) && float.TryParse(REScal, out fRES)
                    && float.TryParse(ARScal, out fARS) && float.TryParse(APHcal, out fAPH))
                {                 
                    float sum = fSAH + fRES + fARS + fAPH;

                    /*关联错误ABS(MNA-SUM(M02:M07))<0.02*/
                    if (Math.Abs(sum - 100) >= 0.1)
                    {
                        sbAlert.Add (new returnError("APH", CheckMetion(APHOilDataList[0].OilTableRow.itemName, APHOilDataList[0].OilTableRow.RowIndex, APHOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Residue)));
                        this._gridOil.SetRemarkFlag("APH", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert ;
        }
        /// <summary>
        /// 渣油AIP<A10<A30<A50<A70<A90<A95<AEP关联   AIP<A10<A30<A50<A70<A90<A95<AEP
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueAIP_AEPLinkCheck()
        {
            return AIP_AEPLinkCheck(EnumTableType.Residue);
        }
        /// <summary>
        /// 渣油RTT关联 ABS(RTT-RAA-RNN)<0.1
        /// </summary>
        /// <returns></returns>
        private List<returnError> ResidueRTTLinkCheck()
        {
            List<returnError> sbAlert = new List<returnError>();
            List<OilDataEntity> RTTOilDataList = this._gridOil.GetDataByRowItemCode("RTT");
            List<OilDataEntity> RAAOilDataList = this._gridOil.GetDataByRowItemCode("RAA");
            List<OilDataEntity> RNNOilDataList = this._gridOil.GetDataByRowItemCode("RNN");
            if (RTTOilDataList == null)
                return sbAlert ;
            if (RTTOilDataList.Count <= 0)
                return sbAlert ;

            /*关联错误 ABS(RTT-RAA-RNN)<0.1*/
            for (int i = 0; i < this._maxCol; i++)
            {
                string RTTcal = getStrValuefromOilDataEntity(RTTOilDataList, i);
                string RAAcal = getStrValuefromOilDataEntity(RAAOilDataList, i);
                string RNNcal = getStrValuefromOilDataEntity(RNNOilDataList, i);

                float fRTT = 0;
                float fRAA = 0;
                float fRNN = 0;
                if (RTTcal != string.Empty && RAAcal != string.Empty && RNNcal != string.Empty
                   && float.TryParse(RTTcal, out fRTT) && float.TryParse(RAAcal, out fRAA) && float.TryParse(RNNcal, out fRNN))
                {              
                    float sum = fRTT - fRAA - fRNN;

                    /*关联错误ABS(MNA-SUM(M02:M07))<0.02*/
                    if (Math.Abs(sum) >= 0.1)
                    {
                        sbAlert .Add ( new returnError("RTT", CheckMetion(RTTOilDataList[0].OilTableRow.itemName, RTTOilDataList[0].OilTableRow.RowIndex, RTTOilDataList[i].OilTableCol.colName, enumCheckErrType.Relative, EnumTableType.Residue)));
                        this._gridOil.SetRemarkFlag("RTT", i, true, GridOilColumnType.Calc);
                    }
                }
            }
            return sbAlert ;
        }


        #endregion

        #endregion
    }
   public enum enumTrendType { up, down }

    /// <summary>
    /// 缺项错误,数据类型错误,错误上下限,警告上下限，评价上下限，关联错误
    /// </summary>
   public enum enumCheckErrType { Blank, TypeError, ErrLimit, AlertLimit, EvalLimit, Relative }
   /// <summary>
   /// 经验审查:Tend = 趋势审查 ,Limit = 范围审查,
   /// </summary>
   public enum enumCheckExpericencType { Tend , Limit}
   /// <summary>
   /// 范围审查表枚举 :Whole = 1 = 原油性质表,Naphtha = 石脑油表, AviationKerosene = 航煤表,DieselOil = 柴油表，VGO = VGO表,Residue = 渣油表
   /// </summary>
   public enum enumCheckTrendType 
   {
       [Description("原油性质表")]
       Whole = 1,

       [Description("石脑油表")]
       Naphtha,

       [Description("航煤表")]
       AviationKerosene,

       [Description("柴油表")]
       DieselOil,

       [Description("VGO表")]
       VGO,

       [Description("渣油表")]
       Residue } 

   
}
