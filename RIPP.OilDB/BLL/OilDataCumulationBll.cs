using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.BLL
{
    public class OilDataCumulationBll
    {
        #region "WY/VY累积和(允许存在空值)"
     
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的WY/VY累积和(允许存在空值)
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="itemCode">WY/VY</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,itemCode</returns>
        public static Dictionary<string, float> getWYVYCumuationValueAllowEmptyFromNarrow(GridOilViewA narrowGridOil, string strICP, string strECP, string itemCode)
        {
            float SUM_ItemCode = 0; //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = narrowGridOil.GetDataByRowItemCode("ICP").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> ECPList = narrowGridOil.GetDataByRowItemCode("ECP").Where(o => o.calData != string.Empty).ToList();

            if (ICPList == null || ECPList == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (ICPList.Count <= 0 || ECPList.Count <= 0)//如果窄馏分数据表中ICP和ECP不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = ICPList.Where(o => o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = ECPList.Where(o =>o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeList = narrowGridOil.GetDataByRowItemCode(itemCode);

            if (ItemCodeList == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (ItemCodeList.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            bool Bbreak = false;//判断中间数据是否空（允许存在空值）
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity ItemCodeOilData = ItemCodeList.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                float itemCodeCal = 0;
                if (ItemCodeOilData != null && float.TryParse(ItemCodeOilData.calData, out itemCodeCal))
                {
                    Bbreak = true;
                    SUM_ItemCode += itemCodeCal;
                }
            }

            if (Bbreak)
                ReturnDic.Add(itemCode, SUM_ItemCode);

            return ReturnDic;
        }
 
        /// <summary>
        /// 宽馏分中查找对应ICP和ECP,并且找出指定物性的值(允许存在空值)
        /// </summary>
        /// <param name="wideGridOil">宽馏分表</param>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="itemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性值 ,itemCode</returns>
        public static Dictionary<string, float> getWYVYCumuationValueAllowEmptyFromWide(GridOilViewA wideGridOil, string strICP, string strECP, string itemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = wideGridOil.GetDataByRowItemCode("ICP").Where(o => o.calShowData == strICP).ToList();
            List<OilDataEntity> ECPList = wideGridOil.GetDataByRowItemCode("ECP").Where(o => o.calShowData == strECP).ToList();
            if (ICPList == null || ECPList == null)//如果ICP和ECP数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (ICPList.Count <= 0 || ECPList.Count <= 0)//如果ICP和ECP数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> itemCodeList = wideGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();

            if (itemCodeList == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion

            #region "ICP--ECP"
            foreach (OilDataEntity ICPData in ICPList)
            {
                OilDataEntity ECPData = ECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataItemCode = itemCodeList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;

                float itemCodeCal = 0;
                if (oilDataItemCode.calShowData != string.Empty && float.TryParse(oilDataItemCode.calData, out itemCodeCal))
                {
                    ReturnDic.Add(itemCode, itemCodeCal);
                    break;
                }
            }
            #endregion

            return ReturnDic;
        }
        
        /// <summary>
        /// 通过渣油表的ICP查找对应的itemCode列,并且找出指定物性值(允许存在空值)
        /// </summary>
        /// <param name="residueGridOil">渣油表</param>
        /// <param name="strICP">渣油表ICP的显示值</param>
        /// <param name="itemCode">渣油表中查找的物性</param>
        /// <returns>渣油表中查找的itemCode物性值</returns>
        public static Dictionary<string, float> getWYVYCumuationValueAllowEmptyFromResidue(GridOilViewA residueGridOil, string strICP, string itemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = residueGridOil.GetDataByRowItemCode("ICP").Where(o => o.calShowData == strICP).ToList();

            if (ICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (ICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = residueGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in ICPList)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                float itemCodeCal = 0;
                if (oilDataItemCode != null && float.TryParse(oilDataItemCode.calData, out itemCodeCal))
                {
                    ReturnDic.Add(itemCode, itemCodeCal);
                    break;
                }
            }
            #endregion

            return ReturnDic;
        }
        
        #endregion

        #region "除了WY/VY的其他物性的累积和(允许存在空值)"
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(允许存在空值)
        /// </summary>
        /// <param name="strICP">窄馏分的ICP</param>
        /// <param name="strECP">窄馏分的ECP</param>
        /// <param name="itemCode">计算的物性</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        public static Dictionary<string, float> getItemCodeCumuationValueAllowEmptyFromNarrow(GridOilViewA narrowGridOil, string strICP, string strECP, string itemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = narrowGridOil.GetDataByRowItemCode("ICP").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> ECPList = narrowGridOil.GetDataByRowItemCode("ECP").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> WYList = narrowGridOil.GetDataByRowItemCode("WY").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> itemCodeList = narrowGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();

            if (ICPList == null||ECPList == null||WYList == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (ICPList.Count <= 0||ECPList.Count <= 0||WYList.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = ICPList.Where(o => o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = ECPList.Where(o => o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
 
            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;


            #endregion

            #region "计算"
            bool WYbreak = false, SUMbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataWY = WYList.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                OilDataEntity ItemCodeOilData = itemCodeList.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();

                
                float wyCal = 0;
                if (oilDataWY != null && float.TryParse(oilDataWY.calData, out wyCal))
                {
                    SUM_WY += wyCal;
                    WYbreak = true;
                }

                if (ItemCodeOilData == null)
                    continue;


                if (oilDataWY != null && ItemCodeOilData != null && float.TryParse(oilDataWY.calData, out wyCal))
                {
                    string strTemp = BaseFunction.IndexFunItemCode(ItemCodeOilData.calData, itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = SUM_POR + wyCal * fTtemp;
                        SUMbreak = true;
                    }
                }
            }

            if (SUMbreak)
                ReturnDic.Add(itemCode, SUM_POR);
            if (WYbreak)
                ReturnDic.Add("WY", SUM_WY);
               
            #endregion

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        public static Dictionary<string, float> getItemCodeCumuationValueAllowEmptyFromWide(GridOilViewA wideGridOil, string strICP, string strECP, string itemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();
            float SUM_POR = 0;  //定义返回变量的两个值
            
            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = wideGridOil.GetDataByRowItemCode("ICP").Where(o =>o.calShowData == strICP).ToList();
            List<OilDataEntity> ECPList = wideGridOil.GetDataByRowItemCode("ECP").Where(o =>o.calShowData == strECP).ToList();
            List<OilDataEntity> WYList = wideGridOil.GetDataByRowItemCode("WY").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> itemCodeList = wideGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();
            if (ICPList == null || ECPList == null || WYList == null) 
                return ReturnDic;
            if (ICPList.Count <= 0 || ECPList.Count <= 0 || WYList.Count <= 0) 
                return ReturnDic;


            OilDataEntity oilDataICP = ICPList.Where(o => o.calShowData == strICP).FirstOrDefault();
            OilDataEntity oilDataECP = ECPList.Where(o => o.calShowData == strECP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
           
            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in ICPList)
            {
                OilDataEntity ECPData = ECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;
            
                OilDataEntity oilDataWY = WYList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                float wyCal = 0;
                if (oilDataWY != null && float.TryParse(oilDataWY.calData, out wyCal))
                {
                    ReturnDic.Add("WY", wyCal);
                }

                if (itemCodeList == null)
                    continue;

                OilDataEntity oilDataItemCode = itemCodeList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();

                if (oilDataWY != null && oilDataItemCode != null && float.TryParse(oilDataWY.calData, out wyCal))
                {
                    string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calData, itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = wyCal * fTtemp;
                        ReturnDic.Add(itemCode, SUM_POR);
                        break;
                    }
                }              
            }
            #endregion

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="itemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        public static Dictionary<string, float> getItemCodeCumuationValueAllowEmptyFromResidue(GridOilViewA residueGridOil, string strICP, string itemCode)
        {
            float SUM_POR = 0;  //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = residueGridOil.GetDataByRowItemCode("ICP").Where(o => o.calShowData == strICP).ToList();

            List<OilDataEntity> WYList = residueGridOil.GetDataByRowItemCode("WY").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> itemCodeList = residueGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();
            if (ICPList == null   || WYList == null)
                return ReturnDic;
            if (ICPList.Count <= 0 || WYList.Count <= 0)
                return ReturnDic;
 
            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in ICPList)
            {
                OilDataEntity oilDataWY = WYList.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();

                float wyCal = 0;
                if (oilDataWY != null && float.TryParse(oilDataWY.calData, out wyCal))
                {
                    ReturnDic.Add("WY", wyCal);
                }

                if (itemCodeList == null)
                    continue;
                OilDataEntity oilDataItemCode = itemCodeList.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();

                if (oilDataWY != null && oilDataItemCode != null && float.TryParse(oilDataWY.calData, out wyCal))
                {
                    string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calData, itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = wyCal * fTtemp;
                        ReturnDic.Add(itemCode, SUM_POR);
                        break;
                    }
                }
            }
            #endregion

            return ReturnDic;
        }
        #endregion


        #region "连续算法(不允许存在空值)"
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的WY/VY累积和(不允许存在空值)
        /// </summary>
        /// <param name="narrowGridOil">窄馏分表</param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="itemCode">定物性的WY/VY</param>
        /// <returns></returns>
        public static Dictionary<string, float> getWYVYCumuationValueNotAllowEmptyFromNarrow(GridOilViewA narrowGridOil, string strICP, string strECP, string itemCode)
        {
            float SUM_POR = 0; //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = narrowGridOil.GetDataByRowItemCode("ICP").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> ECPList = narrowGridOil.GetDataByRowItemCode("ECP").Where(o => o.calData != string.Empty).ToList();

            if (ICPList == null || ECPList == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (ICPList.Count <= 0 || ECPList.Count <= 0)//如果窄馏分数据表中ICP和ECP不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = ICPList.Where(o => o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = ECPList.Where(o => o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeList = narrowGridOil.GetDataByRowItemCode(itemCode);

            if (ItemCodeList == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (ItemCodeList.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            bool Bbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataItemCode = ItemCodeList.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                if (oilDataItemCode == null)
                {
                    Bbreak = true;//计算过程不能为空，为空则跳出
                    break;
                }
                else
                {
                    float itemCodeCal = 0;
                    if (float.TryParse(oilDataItemCode.calData, out itemCodeCal))
                        SUM_POR += itemCodeCal;
                    else
                    {
                        Bbreak = true;
                        break;
                    }
                }
            }

            if (Bbreak)
                return ReturnDic;
            else
                ReturnDic.Add(itemCode, SUM_POR);

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性WY/VY累积和(不允许存在空值)
        /// </summary>
        /// <param name="wideGridOil">宽馏分</param>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="itemCode">指定物性WY/VY</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        public static Dictionary<string, float> getWYVYCumuationValueNotAllowEmptyFromWide(GridOilViewA wideGridOil, string strICP, string strECP, string itemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = wideGridOil.GetDataByRowItemCode("ICP").Where(o => o.calShowData == strICP).ToList();
            List<OilDataEntity> ECPList = wideGridOil.GetDataByRowItemCode("ECP").Where(o => o.calShowData == strECP).ToList();
            if (ICPList == null || ECPList == null)//如果ICP和ECP数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (ICPList.Count <= 0 || ECPList.Count <= 0)//如果ICP和ECP数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> itemCodeList = wideGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();

            if (itemCodeList == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in ICPList)
            {
                OilDataEntity ECPData = ECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataItemCode = itemCodeList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;
                else
                {
                    float wyCal = 0;
                    if (float.TryParse(oilDataItemCode.calData, out wyCal))
                    {
                        ReturnDic.Add(itemCode, wyCal);
                        break;
                    }
                    else
                        continue;
                }
            }
            #endregion

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        public static Dictionary<string, float> getWYVYCumuationValueNotAllowEmptyFromResidue(GridOilViewA residueGridOil, string strICP, string itemCode)
        {           
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = residueGridOil.GetDataByRowItemCode("ICP").Where(o => o.calShowData == strICP).ToList();

            if (ICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (ICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = residueGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in ICPList)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;//计算过程不能为空，为空则跳出
                else
                {
                    float wyCal = 0;
                    if (float.TryParse(oilDataItemCode.calData, out wyCal))
                    {
                        ReturnDic.Add(itemCode, wyCal);
                        break;
                    }
                    else
                        continue;
                }
            }
            #endregion

            return ReturnDic;
        }
 
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        
        /// <param name="strICP">窄馏分的ICP</param>
        /// <param name="strECP">窄馏分的ECP</param>
        /// <param name="itemCode">计算的物性</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        public static Dictionary<string, float> getItemCodeCumuationValueNotAllowEmptyFromNarrow(GridOilViewA narrowGridOil, string strICP, string strECP, string itemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = narrowGridOil.GetDataByRowItemCode("ICP").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> ECPList = narrowGridOil.GetDataByRowItemCode("ECP").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> WYList = narrowGridOil.GetDataByRowItemCode("WY").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> itemCodeList = narrowGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();

            if (ICPList == null || ECPList == null || WYList == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (ICPList.Count <= 0 || ECPList.Count <= 0 || WYList.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = ICPList.Where(o => o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = ECPList.Where(o => o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            bool Bbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataWY = WYList.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                OilDataEntity oilDataItemCode = itemCodeList.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                if (oilDataWY == null || oilDataItemCode == null)
                {
                    Bbreak = true;//计算过程不能为空，为空则跳出
                    break;
                }
                else
                {
                    float wyCal = 0; float itemCodeCal = 0;
                    if (float.TryParse(oilDataWY.calData, out wyCal) && float.TryParse(oilDataItemCode.calData, out itemCodeCal))
                    {
                        string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calData,itemCode);
                        float fTtemp = 0;
                        if (float.TryParse(strTemp, out fTtemp) && strTemp != string.Empty)
                        {
                            SUM_POR = SUM_POR + wyCal * fTtemp;
                            SUM_WY = SUM_WY + wyCal;
                        }
                        else
                        {
                            Bbreak = true;
                            break;
                        }
                    }
                    else
                    {
                        Bbreak = true;
                        break;
                    }
                }
            }

            if (Bbreak)
                return ReturnDic;
            else
            {
                ReturnDic.Add(itemCode, SUM_POR);
                ReturnDic.Add("WY", SUM_WY);
            }

            return ReturnDic;
        }

        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="wideGridOil"></param>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        public static Dictionary<string, float> getItemCodeCumuationValueNotAllowEmptyFromWide(GridOilViewA wideGridOil, string strICP, string strECP, string itemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = wideGridOil.GetDataByRowItemCode("ICP").Where(o => o.calShowData == strICP).ToList();
            List<OilDataEntity> ECPList = wideGridOil.GetDataByRowItemCode("ECP").Where(o => o.calShowData == strECP).ToList();
            List<OilDataEntity> WYList = wideGridOil.GetDataByRowItemCode("WY").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> itemCodeList = wideGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();
            if (ICPList == null || ECPList == null || WYList == null)
                return ReturnDic;
            if (ICPList.Count <= 0 || ECPList.Count <= 0 || WYList.Count <= 0)
                return ReturnDic;


            OilDataEntity oilDataICP = ICPList.Where(o => o.calShowData == strICP).FirstOrDefault();
            OilDataEntity oilDataECP = ECPList.Where(o => o.calShowData == strECP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in ICPList)
            {
                OilDataEntity ECPData = ECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataWY = WYList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                OilDataEntity oilDataItemCode = itemCodeList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (oilDataWY == null || oilDataItemCode == null)
                    continue;
                else
                {
                    float wyCal = 0; float itemCodeCal = 0;
                    if (float.TryParse(oilDataWY.calData, out wyCal) && float.TryParse(oilDataItemCode.calData, out itemCodeCal))
                    {
                        string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calData, itemCode);
                        float fTtemp = 0;
                        if (float.TryParse(strTemp, out fTtemp) && strTemp != string.Empty)
                        {
                            SUM_POR = wyCal * fTtemp;
                            SUM_WY = wyCal;
                            ReturnDic.Add(itemCode, SUM_POR);
                            ReturnDic.Add("WY", SUM_WY);
                            break;
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }
            }
            #endregion

            return ReturnDic;
        }

        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="residueGridOil"></param>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        public static Dictionary<string, float> getItemCodeCumuationValueNotAllowEmptyFromResidue(GridOilViewA residueGridOil, string strICP, string itemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || itemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> ICPList = residueGridOil.GetDataByRowItemCode("ICP").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> WYList = residueGridOil.GetDataByRowItemCode("WY").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> itemCodeList = residueGridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();
            if (ICPList == null || WYList == null)
                return ReturnDic;
            if (ICPList.Count <= 0 || WYList.Count <= 0)
                return ReturnDic;

            OilDataEntity oilDataICP = ICPList.Where(o => o.calShowData == strICP).FirstOrDefault();

            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in ICPList)
            {
                OilDataEntity oilDataWY = WYList.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                OilDataEntity oilDataItemCode = itemCodeList.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                if (oilDataWY == null || oilDataItemCode == null)
                    continue;//计算过程不能为空，为空则跳出
                else
                {
                    float wyCal = 0;
                    if (float.TryParse(oilDataWY.calData, out wyCal))
                    {
                        string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calData, itemCode);
                        float fTtemp = 0;
                        if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                        {
                            SUM_POR = wyCal * fTtemp;
                            SUM_WY = wyCal;
                            ReturnDic.Add(itemCode, SUM_POR);
                            ReturnDic.Add("WY", SUM_WY);
                            break;
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }
            }
            #endregion

            return ReturnDic;
        }
        #endregion

    }
}
