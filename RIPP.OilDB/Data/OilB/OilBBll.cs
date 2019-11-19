using RIPP.OilDB.Model;
using RIPP.OilDB.Model.OilB;
using RIPP.OilDB.Model.Query.RangeQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
namespace RIPP.OilDB.Data.OilB
{
    public class OilBBll
    {
        /// <summary>
        /// 从切割结果获取满足范围查询条件的原油编号
        /// </summary>
        /// <param name="rangeSearchEntityList"></param>
        /// <param name="oilBList"></param>
        /// <returns></returns>
        public IDictionary<string, double> GetRangOilBCrudeIndex(IList<ToolCusFraRanQueListItemEntity> rangeSearchEntityList,List<OilInfoBEntity> oilBList)
        {
            int count = 0; //输入条件的个数
            IDictionary<string, double> crudeIndexRanDic = new Dictionary<string, double>();
         
            #region "输入条件判断"
            if (rangeSearchEntityList == null)//查找条件为空
                return crudeIndexRanDic;
            if (rangeSearchEntityList.Count == 0)//查找条件为空
                return crudeIndexRanDic;
            #endregion

            #region "标志条件"
            Dictionary<string, int> oilBCrudeIndex_And_Result = new Dictionary<string, int>();//满足条件的原油对应值为1，不满足为0            
           
            foreach (var oilB in oilBList)//从C库中查找到的原油编号
            {
                if (oilB != null)
                    oilBCrudeIndex_And_Result[oilB.crudeIndex] = 0;
            }
            #endregion

            #region "范围查找中And和or两种类型的判断的归类"
            IList<ToolCusFraRanQueListItemEntity> searchAnd = new List<ToolCusFraRanQueListItemEntity>();//范围查找用（and条件）
            IList<ToolCusFraRanQueListItemEntity> searchOr = new List<ToolCusFraRanQueListItemEntity>();//范围查找用（or条件）
            foreach (ToolCusFraRanQueListItemEntity currentOilRangeSearchEntity in rangeSearchEntityList)
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
                    #region "一条原油中Or条件的处理"
                    searchOr.Add(currentOilRangeSearchEntity);

                    foreach (var oilB in oilBList)//从C库中查找到的原油编号
                    {
                        oilBCrudeIndex_And_Result[oilB.crudeIndex] += getDataFromOrRangeSearchList(searchOr, oilB);
                    }
                    count++;
                    searchOr.Clear();//该Or括号计算完后，情况OrList，用于后面的Or括号计算
                    continue;
                    #endregion
                }
            }
            #endregion

            #region "一条原油中And条件处理（或计算的结果和And条件取交集"
            foreach (var oilB in oilBList)//从C库中查找到的原油编号
            {
                oilBCrudeIndex_And_Result[oilB.crudeIndex] += getDataFromAndRangeSearchList(searchAnd, oilB);
            }
            #endregion

            foreach (string key in oilBCrudeIndex_And_Result.Keys)
            {
                if (oilBCrudeIndex_And_Result[key] == count)
                {
                    crudeIndexRanDic.Add(key, 0);
                }
            }
            searchAnd.Clear();
            searchOr.Clear();
            return crudeIndexRanDic;
        }

        

        /// <summary>
        /// 获取C库的校正值
        /// </summary>
        /// <param name="OilBrangeSearchEntity"></param>
        /// <param name="oilInfoID">oilInfoID</param>
        /// <param name="value">查询的物性是否为原油信息表（true代表是）</param>
        /// <returns></returns>
        private static string getCalValueFromOilDataSearch(ToolCusFraRanQueListItemEntity OilBRangeSearchEntity, OilInfoBEntity oilB, enumToolQueryDataBTableName tableType = enumToolQueryDataBTableName.WhoTable)
        {
            string result = string.Empty;

            if (tableType != enumToolQueryDataBTableName.WhoTable)
            {
                List<CutDataEntity> cutDataList = oilB.CutDataEntityList.Where(o => o.CutName == OilBRangeSearchEntity.CutName).ToList();
                if (cutDataList.Count != 0)
                    result = cutDataList.FirstOrDefault().ShowCutData;
            }
            else
            {
                List<OilDataBEntity> cutDataList = oilB.OilDatas.Where(o => o.OilTableTypeID == (int) EnumTableType.Whole).ToList();
                if (cutDataList.Count != 0)
                    result = cutDataList.FirstOrDefault().calShowData;
            }
            return result;
        }
        /// <summary>
        /// 获取除原油信息表外的相应物性上下限值
        /// </summary>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        private static List<double> dataUpDown(ToolCusFraRanQueListItemEntity searchEntity)
        {
            List<double> Result = new List<double>();
            double down = 0, up = 0;
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
        /// 从一条原油中判断和条件集合是否可用
        /// </summary>
        /// <param name="andOilBRangeSearchList"></param>
        /// <param name="oilInfoID"></param>
        /// <returns></returns>
        private static int getDataFromAndRangeSearchList(IList<ToolCusFraRanQueListItemEntity> andOilBRangeSearchList,OilInfoBEntity oilB)
        {
            int result = 0;//返回结果

            #region "输入条件处理"
            if (andOilBRangeSearchList == null)
                return result;

            if (andOilBRangeSearchList.Count <= 0)
                return result;

            foreach (ToolCusFraRanQueListItemEntity currrentOilRangeSearchEntity in andOilBRangeSearchList)//循环每一个物性
            {
                if (!currrentOilRangeSearchEntity.IsAnd)
                    return result;
            }
            #endregion

            #region "And条件判断"
            foreach (ToolCusFraRanQueListItemEntity currrentOilRangeSearchEntity in andOilBRangeSearchList)//循环每一个物性
            {
                //if (currrentOilRangeSearchEntity.TableName != enumToolQueryDataBTableName.WhoTable)
                //{
                    string strCalData = getCalValueFromOilDataSearch(currrentOilRangeSearchEntity, oilB, currrentOilRangeSearchEntity.TableName);

                    if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                        continue;

                    double dataDown = dataUpDown(currrentOilRangeSearchEntity).Min();//范围下限
                    double dataUp = dataUpDown(currrentOilRangeSearchEntity).Max();//范围上限

                    float calData = 0;
                    if (!float.TryParse(strCalData, out calData))   //判断字符串是否能够转换为float类型
                        continue;
                    if (calData >= dataDown && calData <= dataUp)
                        result += 1;
                //}
                //else if (currrentOilRangeSearchEntity.TableName == enumToolQueryDataBTableName.WhoTable)
                //{
                    //string strCalData = getCalValueFromOilDataSearch(currrentOilRangeSearchEntity, oilInfoID, EnumTableType.Info);

                    //if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                    //    continue;

                    //List<string> upDown = oilInfoDataUpDown(currrentOilRangeSearchEntity);
                    //string dataDown = upDown.Min();//范围下限
                    //string dataUp = upDown.Max();//范围上限
                    //if (dataDown == dataUp && strCalData == dataUp)//原油信息表的精确查找
                    //{
                    //    result += 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                    //    continue;
                    //}
                    //else if (string.Compare(strCalData, dataDown) >= 0 && string.Compare(strCalData, dataUp) <= 0)//原油信息表的范围查找
                    //{
                    //    result += 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                    //    continue;
                    //}
                //}
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 从一条原油中判断或条件集合是否可用
        /// </summary>
        /// <param name="OrOilBRangeSearchList"></param>
        /// <param name="oilInfoID"></param>
        /// <returns> 1 = 可用，0 = 不可用</returns>
        private static int getDataFromOrRangeSearchList(IList<ToolCusFraRanQueListItemEntity> OrOilBRangeSearchList, OilInfoBEntity oilB)
        {
            int result = 0;//返回结果

            #region "输入条件处理"
            if (OrOilBRangeSearchList == null)
                return result;

            if (OrOilBRangeSearchList.Count <= 0)
                return result;
            foreach (var  currrentOilRangeSearchEntity in OrOilBRangeSearchList)//循环每一个物性
            {
                if (currrentOilRangeSearchEntity.IsAnd)
                    return result;
            }
            if (OrOilBRangeSearchList[0].LeftParenthesis != "(")//队列的头部为右括号
                return result;

            if (OrOilBRangeSearchList[OrOilBRangeSearchList.Count - 1].RightParenthesis != ")")//队列的尾部为右括号
                return result;
            #endregion

            #region "Or条件判断"
            foreach (ToolCusFraRanQueListItemEntity currentOilRangeSearchEntity in OrOilBRangeSearchList)//循环每一个Or条件
            {
                //if (currentOilRangeSearchEntity.TableName != enumToolQueryDataBTableName.WhoTable)
                //{
                    string strCalData = getCalValueFromOilDataSearch(currentOilRangeSearchEntity, oilB, currentOilRangeSearchEntity.TableName);
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
                //}
                //else if (currentOilRangeSearchEntity.OilTableTypeID == (int)EnumTableType.Info)
                //{
                //    string strCalData = getCalValueFromOilDataSearch(currentOilRangeSearchEntity, oilInfoID, EnumTableType.Info);

                //    if (strCalData != string.Empty && strCalData != null)
                //    {
                //        List<string> upDown = oilInfoDataUpDown(currentOilRangeSearchEntity);
                //        string dataDown = upDown.Min();//范围下限
                //        string dataUp = upDown.Max();//范围上限
                //        if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                //            continue;

                //        if (dataDown == dataUp && strCalData == dataUp)//原油信息表的精确查找
                //        {
                //            result = 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                //            break;
                //        }
                //        else if (string.Compare(strCalData, dataDown) >= 0 && string.Compare(strCalData, dataUp) <= 0)//原油信息表的范围查找
                //        {
                //            result = 1;//这一项Or中只要有一个Or条件满足，则置1进入下一条原油的计算
                //            break;
                //        }
                //    }
                //}
            }
            #endregion

            return result;
        }
    }
}
