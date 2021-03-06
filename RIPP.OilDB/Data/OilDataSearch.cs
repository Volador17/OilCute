using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using System.Data.SqlClient;
using System.Windows.Forms ;

namespace RIPP.OilDB.Data
{
    public partial class TempsimliarEntity
    {
        private double _Match = 0;//相似度
        private double _Weight = 0;//权重

        public TempsimliarEntity()
        {

        }

        /// <summary>
        /// 相似度
        /// </summary>
        public double Match
        {
            set { this._Match = value; }
            get { return this._Match; }
        }

        /// <summary>
        /// 权重
        /// </summary>
        public double Weight
        {
            set { this._Weight = value; }
            get { return this._Weight; }
        }
    }

    public partial class OilDataSearch
    {
        /// <summary>
        /// 存储暂时的相似查找的原有编号和相识度。
        /// </summary>
        Dictionary<string, List<TempsimliarEntity>> tempSimliarDic = new Dictionary<string, List<TempsimliarEntity>>();//存储暂时的相似查找的原有编号和相识度。

        #region "范围查询函数"
                /// <summary>
        /// 范围查询函数
        /// 根据原油属性查询找到满足条件的原油的ID
        /// </summary>
        /// <param name="cutPropertys">要查询的属性（包括范围）列表</param>
        /// <returns>key为原油数据中的InfoID,值为该ID满足条件的次数或null</returns>
        public Dictionary<int, int> GetInfoID(IList<OilRangeSearchEntity> cutPropertys)
        {
            if (cutPropertys.Count == 0)
                return null;

            Dictionary<int, int> infoIDAnd = new Dictionary<int, int>();  //范围查询满足and条件的oilInfoId
            Dictionary<int, int> infoIDOr = new Dictionary<int, int>();  //范围查询满足or条件的oilInfoId
            string sqlWhereAnd = "";   //按属性或查询原油数据表，既把查询属性的所有原油数据全查出来，只查关于And的选项
            string sqlWhereOr = "";   //按属性或查询原油数据表，只查关于or选项的选项。
            Dictionary<int, int> resutlDictory = new Dictionary<int, int>();//返回最后查询的合适ID的结果

            OilDataSearchAccess access = new OilDataSearchAccess();
            foreach (var item in cutPropertys)
            {
                if (item.IsAnd)
                    sqlWhereAnd += "or  oilTableRowID= '" + item.OilTableRowID + "' ";
                else
                    sqlWhereOr += "or  oilTableRowID= '" + item.OilTableRowID + "' ";
            }
            /*获取And条件下适合的数据*/
            if ("" != sqlWhereAnd)
            {
                infoIDAnd = GetAndOilInfoId(cutPropertys, sqlWhereAnd, access);
            }

            /*获取Or条件下适合的数据,返回or条件下只要满足一个条件的id*/
            if ("" != sqlWhereOr)
            {
                infoIDOr = GetOrOilInfoID(cutPropertys, sqlWhereOr, access);
            }
            /*求And条件下满足的ID和or条件下满足条件的ID的交集即为此次查询的结果
             *求出and条件和or条件的个数，如果如果cutProperties只有一个条件则返回其中不为空的数据
             *如果and的个数为0则返回or条件查询出的数据,如果or为0则返回and查询条件的结果，
             *如果and 和or的个数都不为0的话则求两个条件的交集即为查询的结果
             */
            #region
            int andCount = cutPropertys.Where(s => s.IsAnd == true).Count();
            int orCount = cutPropertys.Count() - andCount;
            if (cutPropertys.Count() == 1)
            {
                return cutPropertys[0].IsAnd == true ? infoIDAnd : infoIDOr;
            }
            else if (orCount == 0)
            {
                return infoIDAnd;//or为0则只返回and的查询结果集
            }
            else if (andCount == 0)
            {
                return infoIDOr;//and为0则只返回or的查询结果集
            }
            else
            {
                foreach (var item in infoIDOr)
                {
                    if (infoIDAnd.ContainsKey(item.Key))
                    {
                        resutlDictory[item.Key] = item.Value;
                    }
                }
                return resutlDictory;//返回or查询数据和and查询数据的交集
            }
            #endregion
        }
        /// <summary>
        /// 获取or条件下的满足条件的oilInfoID
        /// </summary>
        /// <param name="cutPropertys"></param>
        /// <param name="sqlWhereOr"></param>
        /// <param name="access"></param>
        /// <returns>满足条件的OilInfoId</returns>
        private Dictionary<int, int> GetOrOilInfoID(IList<OilRangeSearchEntity> cutPropertys, string sqlWhereOr, OilDataSearchAccess access)
        {
            sqlWhereOr = " (" + sqlWhereOr.Substring(2) + ") and calData!='' ";
            //OilDataAccess acessOr = new OilDataAccess();
            IList<OilDataSearchEntity> oilDatasOr = access.Get(sqlWhereOr);
            Dictionary<int, int> infoID = new Dictionary<int, int>();
            foreach (OilDataSearchEntity oilData in oilDatasOr)
            {
                foreach (var cutProperty in cutPropertys.Where(s => s.IsAnd == false))
                {
                    float calData = 0;
                    if (float.TryParse(oilData.calData, out calData))
                    {//判断字符串是否能够转换为float类型
                    }
                    else
                    {
                        calData = -9999999;
                    }
                    if (oilData.OilTableRow.itemCode == cutProperty.itemCode && oilData.oilTableColID == cutProperty.OilTableColID && float.Parse(oilData.calData) >= float.Parse(cutProperty.downLimit) && float.Parse(cutProperty.upLimit) >= float.Parse(oilData.calData))
                    {
                        if (infoID.Keys.Contains(oilData.oilInfoID))
                        {
                            infoID[oilData.oilInfoID] = infoID[oilData.oilInfoID] + 1;
                        }
                        else
                        {
                            infoID[oilData.oilInfoID] = 1;
                        }
                    }
                }
            }
            return infoID;
        }

        /// <summary>
        /// 返回And条件下的OilInfoID
        /// </summary>
        /// <param name="cutPropertys"></param>
        /// <param name="sqlWhereAnd"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        private Dictionary<int, int> GetAndOilInfoId(IList<OilRangeSearchEntity> cutPropertys, string sqlWhereAnd, OilDataSearchAccess access)
        {
            sqlWhereAnd = " (" + sqlWhereAnd.Substring(2) + ") and calData!='' ";
            IList<OilDataSearchEntity> oilDatas = access.Get(sqlWhereAnd);
            Dictionary<int, int> infoID = new Dictionary<int, int>();
            //对每条原油数据，遍历属性列表，如果原油属性=属性列表中的某个属性，并且在查询范围内。
            //如果字典键值中存在该原油的OilInfoID则该键对应的值加1，如果不存在则该键值为1.
            foreach (OilDataSearchEntity oilData in oilDatas)
            {
                foreach (var cutProperty in cutPropertys.Where(s => s.IsAnd == true))
                {
                    float calData = 0;
                    if (float.TryParse(oilData.calData, out calData))
                    {//判断字符串是否能够转换为float类型
                    }
                    else
                    {
                        calData = -9999999;
                    }
                    if (oilData.OilTableRow.itemCode == cutProperty.itemCode && oilData.oilTableColID == cutProperty.OilTableColID && calData > float.Parse(cutProperty.downLimit) && float.Parse(cutProperty.upLimit) > calData)
                    {
                        if (infoID.Keys.Contains(oilData.oilInfoID))
                        {
                            infoID[oilData.oilInfoID] = infoID[oilData.oilInfoID] + 1;
                        }
                        else
                        {
                            infoID[oilData.oilInfoID] = 1;
                        }
                    }
                }
            }
            //键的值=属性个数值符合要求
            Dictionary<int, int> infoID2 = new Dictionary<int, int>();
            foreach (var item in infoID)
            {
                if (item.Value == cutPropertys.Where(s => s.IsAnd == true).Count())
                {
                    infoID2[item.Key] = item.Value;
                }
            }
            return infoID2;
        }
        #endregion

        #region "范围查询"
        /// <summary>
        /// 从一条原油中判断或条件集合是否可用
        /// </summary>
        /// <param name="OrOilRangeSearchList"></param>
        /// <param name="oilInfoID"></param>
        /// <returns> 1 = 可用，0 = 不可用</returns>
        private static int getDataFromOrRangeSearchList(IList<OilRangeSearchEntity> OrOilRangeSearchList, int oilInfoID)
        {
            int result = 0;//返回结果

            #region "输入条件处理"
            if (OrOilRangeSearchList == null)
                return result;

            if (OrOilRangeSearchList.Count <= 0)
                return result;
            foreach (OilRangeSearchEntity currrentOilRangeSearchEntity in OrOilRangeSearchList)//循环每一个物性
            {
                if (currrentOilRangeSearchEntity.IsAnd)
                    return result;
            }
            if (OrOilRangeSearchList[0].LeftParenthesis != "(")//队列的头部为右括号
                return result; 

            if (OrOilRangeSearchList[OrOilRangeSearchList.Count -1].RightParenthesis != ")")//队列的尾部为右括号
                return result;
            #endregion 

            #region "Or条件判断"
            foreach (OilRangeSearchEntity currentOilRangeSearchEntity in OrOilRangeSearchList)//循环每一个Or条件
            {
                if (currentOilRangeSearchEntity.OilTableTypeID != (int)EnumTableType.Info)
                {
                    string strCalData = getCalValueFromOilDataSearch(currentOilRangeSearchEntity, oilInfoID, (EnumTableType)currentOilRangeSearchEntity.OilTableTypeID);
                    if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                        continue;

                    double dataDown = dataUpDown(currentOilRangeSearchEntity).Min();//范围下限
                    double dataUp = dataUpDown(currentOilRangeSearchEntity).Max();//范围上限

                    float calData = 0;
                    if (!float.TryParse(strCalData, out calData))   //判断字符串是否能够转换为float类型
                        continue;

                    if (calData >= dataDown && calData <= dataUp)
                    {
                        result = 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                        break;
                    }
                }
                else if (currentOilRangeSearchEntity.OilTableTypeID == (int)EnumTableType.Info)
                {
                    string strCalData = getCalValueFromOilDataSearch(currentOilRangeSearchEntity, oilInfoID, EnumTableType.Info);

                    if (strCalData != string.Empty && strCalData != null)
                    {
                        List<string> upDown = oilInfoDataUpDown(currentOilRangeSearchEntity);
                        string dataDown = upDown.Min();//范围下限
                        string dataUp = upDown.Max();//范围上限
                        if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                            continue;

                        if (dataDown == dataUp && strCalData == dataUp)//原油信息表的精确查找
                        {
                            result  = 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                            break;
                        }
                        else if (string.Compare(strCalData, dataDown) >= 0 && string.Compare(strCalData, dataUp) <= 0)//原油信息表的范围查找
                        {
                            result = 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                            break;
                        }                       
                    }
                }
            }
            #endregion 

            return result;
        }
        /// <summary>
        /// 从一条原油中判断和条件集合是否可用
        /// </summary>
        /// <param name="AndOilRangeSearchList"></param>
        /// <param name="oilInfoID"></param>
        /// <returns></returns>
        private static int getDataFromAndRangeSearchList(IList<OilRangeSearchEntity> AndOilRangeSearchList, int oilInfoID)
        {
            int result = 0;//返回结果

            #region "输入条件处理"
            if (AndOilRangeSearchList == null)
                return result;

            if (AndOilRangeSearchList.Count <= 0)
                return result;

            foreach (OilRangeSearchEntity currrentOilRangeSearchEntity in AndOilRangeSearchList)//循环每一个物性
            {
                if (!currrentOilRangeSearchEntity.IsAnd)
                    return result;
            }
            #endregion

            #region "And条件判断"
            foreach (OilRangeSearchEntity currrentOilRangeSearchEntity in AndOilRangeSearchList)//循环每一个物性
            {               
                if (currrentOilRangeSearchEntity.OilTableTypeID != (int)EnumTableType.Info)
                {
                    string strCalData = getCalValueFromOilDataSearch(currrentOilRangeSearchEntity, oilInfoID, (EnumTableType)currrentOilRangeSearchEntity.OilTableTypeID);

                    if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                        continue;

                    double dataDown = dataUpDown(currrentOilRangeSearchEntity).Min();//范围下限
                    double dataUp = dataUpDown(currrentOilRangeSearchEntity).Max();//范围上限

                     
                    float calData = 0;
                    if (!float.TryParse(strCalData, out calData))   //判断字符串是否能够转换为float类型
                        continue ;
                    if (calData >= dataDown && calData <= dataUp)
                        result += 1;
                }
                else if (currrentOilRangeSearchEntity.OilTableTypeID == (int)EnumTableType.Info)
                {
                    string strCalData = getCalValueFromOilDataSearch(currrentOilRangeSearchEntity, oilInfoID, EnumTableType.Info);
                    
                    if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                        continue;

                    List<string> upDown = oilInfoDataUpDown(currrentOilRangeSearchEntity);
                    string dataDown = upDown.Min();//范围下限
                    string dataUp = upDown.Max();//范围上限
                    if (dataDown == dataUp && strCalData == dataUp)//原油信息表的精确查找
                    {
                        result += 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                        continue;
                    }
                    else if (string.Compare(strCalData, dataDown) >= 0 && string.Compare(strCalData, dataUp) <= 0)//原油信息表的范围查找
                    {
                        result += 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                        continue;
                    }    
                }
            }
            #endregion

            return result;
        }
        /// <summary>
        /// 范围查询-从C库查找满足条件的原油编号
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, double> GetRangOilInfoCrudeIndex(IList<OilRangeSearchEntity> rangeSearchEntityList)
        {
            int count = 0; //输入条件的个数
            IDictionary<string, double> crudeIndexRanDic = new Dictionary<string, double>();
            //IList<string> resultCrudeIndexList = new List<string>();//存放满足条件的原油编号
            
            #region "输入条件判断"           
            if (rangeSearchEntityList == null)//查找条件为空
                return crudeIndexRanDic;
            if (rangeSearchEntityList.Count == 0)//查找条件为空
                return crudeIndexRanDic;
            #endregion 

            #region "标志条件"
            Dictionary<string, int> oilInfoCrudeIndex_And_Result = new Dictionary<string, int>();//满足条件的原油对应值为1，不满足为0            
            string sqlWhere = "select distinct(oilInfoID) from OilDataSearch";
            OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
            List<int> oilDataSearchOilInfoBIDList = oilDataSearchAccess.getId(sqlWhere);//获取OilDataSearch表中的所有oilInfoID

            foreach (int oilInfoBID in oilDataSearchOilInfoBIDList)//从C库中查找到的原油编号
            {
                OilInfoBEntity bEntity = OilBll.GetOilBByID(oilInfoBID);
                if (bEntity != null)
                    oilInfoCrudeIndex_And_Result[bEntity.crudeIndex] = 0;
            }
            #endregion 

            #region "范围查找中And和or两种类型的判断的归类"
            IList<OilRangeSearchEntity> searchAnd = new List<OilRangeSearchEntity>();//范围查找用（and条件）
            IList<OilRangeSearchEntity> searchOr = new List<OilRangeSearchEntity>();//范围查找用（or条件）
            foreach (OilRangeSearchEntity currentOilRangeSearchEntity in rangeSearchEntityList)
            {
                if (currentOilRangeSearchEntity.IsAnd && currentOilRangeSearchEntity.RightParenthesis.Trim() == "")//如果该条件是And，添加到searchAnd中去
                {
                    searchAnd.Add(currentOilRangeSearchEntity);
                    count++;
                    continue;
                }
                else if (!currentOilRangeSearchEntity.IsAnd && currentOilRangeSearchEntity.RightParenthesis.Trim() != ")")//如果该条件是Or，但不是最后一个Or，则添加到searchOr中去，暂时不进行计算
                {
                    searchOr.Add(currentOilRangeSearchEntity);
                    continue;
                }
                else if (currentOilRangeSearchEntity.RightParenthesis.Trim() == ")")////如果该条件是Or，且是括号中的最后一个Or，则添加到searchOr中去，并进行或的计算
                {
                    #region "Or的计算"
                    searchOr.Add(currentOilRangeSearchEntity);

                    foreach (int oilInfoBID in oilDataSearchOilInfoBIDList)//从C库中查找到的原油编号
                    {
                        OilInfoBEntity infoB = OilBll.GetOilBByID(oilInfoBID);
                        int temp = getDataFromOrRangeSearchList(searchOr, oilInfoBID);
                        oilInfoCrudeIndex_And_Result[infoB.crudeIndex] += temp;
                    }
                    count++;
                    searchOr.Clear();//该Or括号计算完后，情况OrList，用于后面的Or括号计算
                    continue;
                    #endregion
                }
            }
            #endregion 

            #region "处理And条件（或计算的结果和And条件取交集"
            foreach (int oilInfoBID in oilDataSearchOilInfoBIDList)//从C库中查找到的原油编号
            {
                OilInfoBEntity infoB = OilBll.GetOilBByID(oilInfoBID);
                oilInfoCrudeIndex_And_Result[infoB.crudeIndex] += getDataFromAndRangeSearchList(searchAnd, oilInfoBID);
            }               
            #endregion

            foreach (string key in oilInfoCrudeIndex_And_Result.Keys)
            {
                if (oilInfoCrudeIndex_And_Result[key] == count)
                {
                    crudeIndexRanDic.Add(key,0);
                }
            }
            searchAnd.Clear();
            searchOr.Clear();
            return crudeIndexRanDic;
        }

        /// <summary>
        /// 获取C库的校正值
        /// </summary>
        /// <param name="rangeSearchEntity"></param>
        /// <param name="oilInfoID">oilInfoID</param>
        /// <param name="value">查询的物性是否为原油信息表（true代表是）</param>
        /// <returns></returns>
        private static string getCalValueFromOilDataSearch(OilRangeSearchEntity rangeSearchEntity, int oilInfoID,EnumTableType tableType =  EnumTableType.Info)
        {
            string result = string.Empty;

            OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
            string sqlWhere = " oilInfoId ='" + oilInfoID.ToString() + "'" + "and oilTableRowID='" + rangeSearchEntity.OilTableRowID.ToString() + "'" + " and oilTableColId='" + rangeSearchEntity.OilTableColID + "'" + " and calData!=''";
            List<OilDataSearchEntity> oilDataSearchEntityList = oilDataSearchAccess.Get(sqlWhere);//获取对应物性的校正值

            if (oilDataSearchEntityList.Count != 0)
                result = oilDataSearchEntityList.FirstOrDefault().calData;

            //List<OilDataSearchEntity> calList = new List<OilDataSearchEntity>();
            //if (tableType == EnumTableType.Info)
            //{
            //    string itemCode = rangeSearchEntity.itemCode;
            //    string oilTableRowID = OilBll.GetOilTableRowIDFromOilTableRowByItemCode(itemCode, tableType).ToString();
            //    string sqlWhere = " oilInfoId ='" + oilInfoID.ToString() + "'" + "and oilTableRowID='" + oilTableRowID + "'" + " and oilTableColId='" + rangeSearchEntity.OilTableColID + "'" + " and calData!=''";
            //    calList = oilDataSearchAccess.Get(sqlWhere);//获取对应物性的校正值
            //}
            //else
            //{
            //    string sqlWhere = " oilInfoId ='" + oilInfoID.ToString() + "'" + "and oilTableRowID='" + rangeSearchEntity.OilTableRowID.ToString() + "'" + " and oilTableColId='" + rangeSearchEntity.OilTableColID + "'" + " and calData!=''";
            //    calList = oilDataSearchAccess.Get(sqlWhere);//获取对应物性的校正值
            //}
            
            return result;
        }

        /// <summary>
        /// 获取除原油信息表外的相应物性上下限值
        /// </summary>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        private static List<double> dataUpDown(OilRangeSearchEntity searchEntity)
        {
            List<double> Result = new List<double>();
            double down =0 , up = 0;
            if (double.TryParse(searchEntity.downLimit, out down) && double.TryParse(searchEntity.upLimit, out up))
            {
                Result.Add(up);
                Result.Add(down);
            }
            else
            {
                Result.Add(0);
                Result.Add(0);
            }
            return Result;
        }

        /// <summary>
        /// 获取原油信息表的相应物性上下限值
        /// </summary>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        private static List<string> oilInfoDataUpDown(OilRangeSearchEntity searchEntity)
        {
            List<string> result = new List<string>();
            result.Add(searchEntity.downLimit);
            result.Add(searchEntity.upLimit);
            return result;
        }

        #endregion

        #region  "相似查找"
        /// <summary>
        ///从一条原油中获取相似度
        /// </summary>
        /// <param name="AndOilSimilarSearchList"></param>
        /// <param name="oilInfoID"></param>
        /// <returns></returns>
        private static TempsimliarEntity getDataFromAndSimilarSearchList(IList<OilSimilarSearchEntity> AndOilSimilarSearchList, int oilInfoID)
        {
            TempsimliarEntity tempsimliarEntity = new  TempsimliarEntity();//key = SUM , value = sumWeight
            double SUM = 0;//返回结果
            double sumWeight = 0;//权重加和
            #region "输入条件处理"
            if (AndOilSimilarSearchList == null)
                return tempsimliarEntity;

            if (AndOilSimilarSearchList.Count <= 0)
                return tempsimliarEntity;

            foreach (OilSimilarSearchEntity currrentOilSimilarSearchEntity in AndOilSimilarSearchList)//循环每一个物性
            {
                if (!currrentOilSimilarSearchEntity.IsAnd)
                    return tempsimliarEntity;
            }
            #endregion

            #region "And条件判断"
            foreach (OilSimilarSearchEntity currrentOilSimilarSearchEntity in AndOilSimilarSearchList)//循环每一个物性
            {                
                string strCalData = getCalValueFromOilDataSearch(currrentOilSimilarSearchEntity, oilInfoID);

                if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                {
                    tempsimliarEntity.Match = double.NaN;
                    tempsimliarEntity.Weight = sumWeight;
                    return tempsimliarEntity;
                }

                float tempfValue = 0;
                if (!float.TryParse(strCalData, out tempfValue))
                {
                    tempsimliarEntity.Match = double.NaN;
                    tempsimliarEntity.Weight = sumWeight;
                    return tempsimliarEntity;
                }

                float fValue = currrentOilSimilarSearchEntity.Fvalue;
                float weight = currrentOilSimilarSearchEntity.Weight;
                float Diff = currrentOilSimilarSearchEntity.Diff;
                if (Diff > 0)
                {
                    //SUM += weight * ((double)(1 - Math.Pow(((tempfValue - fValue) / Diff), 2)));//得到的运算符左边的相似度值 
                    //SUM += weight * (1 - Math.Abs((tempfValue - fValue) / Diff));  //得到的运算符左边的相似度值
                    //SUM += weight * (1 - Math.Sqrt(Math.Abs((tempfValue - fValue) / Diff)));  //得到的运算符左边的相似度值
                    //SUM += weight * (1 - Math.Sqrt(Math.Sqrt(Math.Abs((tempfValue - fValue) / Diff))));  //得到的运算符左边的相似度值
                    SUM += weight * (1 - Math.Pow(Math.Abs((tempfValue - fValue) / Diff), (double)1.0/3));  //得到的运算符左边的相似度值

                    sumWeight += weight;
                }
                else
                {
                    sumWeight += weight;
                    SUM += weight;
                }
            }
            #endregion

            tempsimliarEntity.Match = SUM;
            tempsimliarEntity.Weight = sumWeight;
            return tempsimliarEntity;
        }

        /// <summary>
        /// 从一条原油中判断或条件集合是否可用
        /// </summary>
        /// <param name="OrOilSimilarSearchList"></param>
        /// <param name="oilInfoID"></param>
        /// <returns></returns>
        private static TempsimliarEntity getDataFromOrSimilarSearchList(IList<OilSimilarSearchEntity> OrOilSimilarSearchList, int oilInfoID)
        {
            TempsimliarEntity tempsimliarEntity = new TempsimliarEntity();//key = SUM , value = sumWeight
            double SUM = 0;//返回结果
            double sumWeight = 0;//权重加和

            #region "输入条件处理"
            if (OrOilSimilarSearchList == null)
                return tempsimliarEntity;

            if (OrOilSimilarSearchList.Count <= 0)
                return tempsimliarEntity;

            if (OrOilSimilarSearchList[0].LeftParenthesis != "(")//队列的头部不为左括号
                return tempsimliarEntity;

            if (OrOilSimilarSearchList[OrOilSimilarSearchList.Count - 1].RightParenthesis != ")")//队列的尾部为右括号
                return tempsimliarEntity;
            #endregion

            #region "Or条件判断"
            double temp = 0,tempWeight = 0; 
            foreach (OilSimilarSearchEntity currentOilSimilarSearchEntity in OrOilSimilarSearchList)//循环每一个Or条件
            {               
                string strCalData = getCalValueFromOilDataSearch(currentOilSimilarSearchEntity, oilInfoID);

                if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                {
                    tempsimliarEntity.Match = double.NaN;
                    tempsimliarEntity.Weight = sumWeight;
                    return tempsimliarEntity;
                }

                 float tempfValue = 0;
                 if (!float.TryParse(strCalData, out tempfValue))
                 {
                     tempsimliarEntity.Match = double.NaN;
                     tempsimliarEntity.Weight = sumWeight;
                     return tempsimliarEntity;
                 }

                 float fValue = currentOilSimilarSearchEntity.Fvalue;
                 float weight = currentOilSimilarSearchEntity.Weight;
                 float Diff = currentOilSimilarSearchEntity.Diff;
                 if (Diff > 0)
                 {
                     //temp = weight * ((double)(1 - Math.Pow(((tempfValue - fValue) / Diff), 2)));  //得到的运算符左边的相似度值
                     //temp = weight * (1 - Math.Abs((tempfValue - fValue) / Diff));  //得到的运算符左边的相似度值
                     //temp = weight * (1 - Math.Sqrt(Math.Abs((tempfValue - fValue) / Diff)));  //得到的运算符左边的相似度值
                     temp = weight * ((double)(1 - Math.Pow(((tempfValue - fValue) / Diff), (double)1.0 / 3)));  //得到的运算符左边的相似度值

                     tempWeight = weight;

                     if (temp > SUM)
                     {
                         SUM = temp;
                         sumWeight = tempWeight;                        
                     }
                 }
                 else
                 {
                     if (1 > SUM)
                     {
                         SUM = 1;
                         sumWeight = weight;
                     }
                 }
            }
            #endregion

            tempsimliarEntity.Match = SUM;
            tempsimliarEntity.Weight = sumWeight;
            return tempsimliarEntity;
        }
        /// <summary>
        /// 获取C库的校正值
        /// </summary>
        /// <param name="rangeSimilarEntity"></param>
        /// <param name="oilInfoID">oilInfoID</param>
        /// <param name="value">查询的物性是否为原油信息表（true代表是）</param>
        /// <returns></returns>
        private static string getCalValueFromOilDataSearch(OilSimilarSearchEntity rangeSimilarEntity, int oilInfoID)
        {
            string result = string.Empty;

            OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
            string sqlWhere = " oilInfoId ='" + oilInfoID.ToString() + "'" + "and oilTableRowID='" + rangeSimilarEntity.OilTableRowID.ToString() + "'" + " and oilTableColId='" + rangeSimilarEntity.OilTableColID + "'" + " and calData!=''";
            List<OilDataSearchEntity> oilDataSearchEntityList = oilDataSearchAccess.Get(sqlWhere);//获取对应物性的校正值

            if (oilDataSearchEntityList.Count != 0)
                result = oilDataSearchEntityList.FirstOrDefault().calData;
            return result;
        }


        /// <summary>
        /// 相似查找,返回对应的原油ID
        /// </summary>
        /// <param name="oilProperty">相似查找条件的实体集合OilSimilarSearchEntityList</param>
        /// <returns></returns>
        public IDictionary<string, double> GetOilSimInfoCrudeIndex(IList<OilSimilarSearchEntity> similarSearchEntityList)
        {
            IDictionary<string, double> crudeIndexSimDic = new Dictionary<string, double>();
            #region "输入条件判断"
            IList<string> resultCrudeIndexList = new List<string>();//存放满足条件的原油编号
            if (similarSearchEntityList == null)//查找条件为空
                return crudeIndexSimDic;
            if (similarSearchEntityList.Count == 0)//查找条件为空
                return crudeIndexSimDic;
            #endregion 

            #region "初始化判断条件"
            string sqlWhere = "select distinct(oilInfoID) from OilDataSearch";
            OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
            List<int> oilDataSearchOilInfoIDList = oilDataSearchAccess.getId(sqlWhere);//获取OilDataSearch表中的所有oilInfoID

            foreach (int oilInfoBID in oilDataSearchOilInfoIDList)//从C库中查找到的原油编号
            {
                OilInfoBEntity bEntity = OilBll.GetOilBByID(oilInfoBID);
                List<TempsimliarEntity> tempList = new List<TempsimliarEntity>();
                if (bEntity != null)
                    tempSimliarDic[bEntity.crudeIndex] = tempList;
            }

            #endregion 

            #region "范围查找中And和or两种类型的判断的归类"
            IList<OilSimilarSearchEntity> searchAnd = new List<OilSimilarSearchEntity>();//范围查找用（and条件）
            IList<OilSimilarSearchEntity> searchOr = new List<OilSimilarSearchEntity>();//范围查找用（or条件）
            foreach (OilSimilarSearchEntity currentOilSimilarSearchEntity in similarSearchEntityList)
            {
                if (currentOilSimilarSearchEntity.IsAnd && currentOilSimilarSearchEntity.RightParenthesis.Trim() == "")//如果该条件是And，添加到searchAnd中去
                {
                    searchAnd.Add(currentOilSimilarSearchEntity);
                    continue;
                }
                else if (!currentOilSimilarSearchEntity.IsAnd && currentOilSimilarSearchEntity.LeftParenthesis.Trim() == "(" && currentOilSimilarSearchEntity.RightParenthesis.Trim() == "")//如果该条件是Or，但不是最后一个Or，则添加到searchOr中去，暂时不进行计算
                {
                    searchOr.Add(currentOilSimilarSearchEntity);
                    continue;
                }
                else if (!currentOilSimilarSearchEntity.IsAnd && currentOilSimilarSearchEntity.LeftParenthesis.Trim() == "" && currentOilSimilarSearchEntity.RightParenthesis.Trim() == "")//如果该条件是Or，但不是最后一个Or，则添加到searchOr中去，暂时不进行计算
                {
                    searchOr.Add(currentOilSimilarSearchEntity);
                    continue;
                }
                else if (currentOilSimilarSearchEntity.RightParenthesis.Trim() == ")")////如果该条件是Or，且是括号中的最后一个Or，则添加到searchOr中去，并进行或的计算
                {
                    #region "Or的计算"
                    searchOr.Add(currentOilSimilarSearchEntity);

                    foreach (int ID in oilDataSearchOilInfoIDList)//根据ID循环每一条原油
                    {
                        OilInfoBEntity infoB = OilBll.GetOilBByID(ID);
                        if (!tempSimliarDic.Keys.Contains(infoB.crudeIndex))
                            continue;

                        if (searchOr.Count <= 0)
                            continue;

                        TempsimliarEntity  tempsimliarEntity = getDataFromOrSimilarSearchList(searchOr, ID);
                        if (tempSimliarDic.Keys.Contains(infoB.crudeIndex))
                            tempSimliarDic[infoB.crudeIndex].Add(tempsimliarEntity);
                    }
                    searchOr.Clear();//该Or括号计算完后，情况OrList，用于后面的Or括号计算
                    continue;

                    #endregion
                }
            }
            #endregion 

            #region "处理And条件或计算的结果和And条件取交集"
            foreach (int oilInfoID in oilDataSearchOilInfoIDList)//根据ID循环每一条原油
            {
                OilInfoBEntity infoB = OilBll.GetOilBByID(oilInfoID);
                if (!tempSimliarDic.Keys.Contains(infoB.crudeIndex))
                    continue;
                if (searchAnd.Count <= 0)
                    continue;

                TempsimliarEntity  tempsimliarEntity  = getDataFromAndSimilarSearchList(searchAnd, oilInfoID);
                if (tempSimliarDic.Keys.Contains(infoB.crudeIndex))
                    tempSimliarDic[infoB.crudeIndex].Add(tempsimliarEntity);
            }
            #endregion

            foreach (string key in tempSimliarDic.Keys)
            {           
                double sum = 0 , sumWeight = 0;
                foreach (TempsimliarEntity tempsimliarEntity in tempSimliarDic[key])
                {
                    if (tempsimliarEntity.Match == 0 || tempsimliarEntity.Match.Equals(double.NaN))
                    {
                        sum = double.NaN;
                        sumWeight = tempsimliarEntity.Weight;
                    }
                    else
                    {
                        sum += tempsimliarEntity.Match;
                        sumWeight += tempsimliarEntity.Weight;
                    }
                }

                if (sumWeight != 0 && !sum.Equals(double.NaN))
                    crudeIndexSimDic.Add(key, sum / sumWeight);
            }

            return crudeIndexSimDic;
        }
        #endregion

        //相似查询函数
        #region
        ///// <summary>
        ///// 将原油ID转换成原油编号
        ///// </summary>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //private string getCrudeIndexByID(int ID)
        //{
        //    string result = string.Empty;
        //    OilInfoBAccess access = new OilInfoBAccess();
        //    result = access.Get(ID).crudeIndex;
        //    return result;
        //}
        #endregion
    }
}
