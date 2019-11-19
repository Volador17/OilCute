using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Interpolation.Algorithms;
using MathNet.Numerics.Interpolation;
using System.Windows.Forms;

namespace RIPP.OilDB.Data
{
    public class BaseFunction
    {
        #region  "似有函数"

        private static readonly float DH2O04C = 0.99997f;// DH2O04C=0.99997; 
        private static readonly float DH2O60F = 0.999006f;// DH2O60F=0.999006

        #endregion 

        #region "构造函数"

        public BaseFunction()
        {

        }

        #endregion 


        #region "关联补充数据算法函数"
        /// <summary>
        /// TYP的函数，用来判断API
        /// </summary>
        /// <param name="fAPICut1">API(250-275)</param>
        /// <param name="fAPICut2">API(395-425)</param>
        /// <returns>strTYP</returns>
        public static  string FunTYPfromAPI1_API2(float fAPICut1, float fAPICut2)
        {
            string strResult = string.Empty;

            #region

            if (fAPICut1 >= 40 && fAPICut2 >= 30)
            { //API(250-275)>=40   & API(395-425)>=30      ,则TYP＝“石蜡基”
                strResult = "石蜡基";
            }
            else if (fAPICut1 >= 40 && fAPICut2 < 30 && fAPICut2 > 20)
            { //API(250-275)>=40   &   20<API(395-425)< 30 , 则TYP＝“石蜡-中间基”
                strResult = "石蜡-中间基";
            }
            else if (fAPICut1 < 40 && fAPICut1 > 33 && fAPICut2 > 30)
            { //33<API(250-275)< 40   &  30<API(395-425)  ,则TYP＝“中间-石蜡基”
                strResult = "中间-石蜡基";
            }
            else if (fAPICut1 < 40 && fAPICut1 > 33 && fAPICut2 < 30 && fAPICut2 > 20)
            { //33<API(250-275)<40   & 20 <API(395-425)< 30  ,则TYP＝“中间基”
                strResult = "中间基";
            }
            else if (fAPICut1 < 40 && fAPICut1 > 33 && fAPICut2 <= 20)
            {//33<API(250-275)< 40   & API(395-425)<=20     ,则TYP＝“中间-环烷基”
                strResult = "中间-环烷基";
            }
            else if (fAPICut1 <= 33 && fAPICut2 < 30 && fAPICut2 > 20)
            {//API(250-275)<＝33    &  20<API(395-425)<30  ,则TYP＝“环烷-中间基”
                strResult = "环烷-中间基";
            }
            else if (fAPICut1 <= 33 && fAPICut2 <= 20)
            { //API(250-275)<＝33    &  API(395-425)<=20  ,则TYP＝“环烷-中间基”
                strResult = "环烷基";
            }

            #endregion

            return strResult;
        }

        /// <summary>
        /// 工具箱的四组分算法和原油补充的不一样
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strD20"></param>
        /// <param name="strSUL"></param>
        /// <param name="strV10"></param>
        /// <param name="strCCR"></param>
        /// <returns>字典的键中输入SAH,ARS,RES ,APH的结果</returns>
        public static Dictionary<string, double> FunSAH_ARS_RES_APHfromICP_ECP_D20_SUL_V10_CCR(OilInfoBEntity oilB, string strICP, string strECP, string strD20, string strSUL, string strV10, string strCCR)
        {
            Dictionary<string, double> returnDic = new Dictionary<string, double>();

            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || strD20 == string.Empty || strSUL == string.Empty || strV10 == string.Empty || strCCR == string.Empty)//有一个条件为空则不计算
                return returnDic;

            double ICP = 0, ECP = 0, D20 = 0, SUL = 0, V10 = 0, CCR = 0;

            if (double.TryParse(strICP, out ICP) && double.TryParse(strECP, out ECP) && double.TryParse(strD20, out D20)
                && double.TryParse(strSUL, out SUL) && double.TryParse(strV10, out V10) && double.TryParse(strCCR, out CCR))
            {
                if (ICP >= ECP)
                    return returnDic;
            }
            else
            {
                return returnDic;
            }

            #endregion
             
            #region "根据条件判断得到结果"

            if (ICP > 300 && ECP <= 600)//STEP2
            {
                if (oilB == null)
                {
                    double SAH = (71.206 - 444.106 * D20 + 12.252 * V10 - 0.964 * V10 * SUL) * 100;
                    double ARS = (24.254 + 2.766 * SUL + 389.363 * D20 - 10.545 * V10) * 100;
                    double RES = (2.787 + (272.611 * D20 - 3.706 * SUL - 8.894) * CCR) * 100;
                    double APH = 100 - SAH - ARS - RES;
                    if (SAH < 0 || ARS < 0 || RES < 0)//如果有一个小于0，则不继续计算
                    {
                        return returnDic;
                    }
                    else
                    {
                        if (APH < -5)//如果APH小于-5，则不继续计算
                        {
                            return returnDic;
                        }
                        else if (APH < 0 && APH >= -5)//如果APH在0~-5之间，则置0
                        {
                            APH = 0;
                        }
                    }

                    double SUM = SAH + ARS + RES + APH;
                    if (SUM == 0)
                        return returnDic;

                    returnDic.Add("SAH", SAH / SUM * 100);//SAH=SAH/SUM*100
                    returnDic.Add("ARS", ARS / SUM * 100);//ARS=ARS/SUM*100
                    returnDic.Add("RES", RES / SUM * 100);//RES=RES/SUM*100
                    returnDic.Add("APH", APH / SUM * 100);//APH=APH/SUM*100   
                }
                else
                {
                    #region "添加切割方案"
                    CutMothedEntity newCutMothedEntity = new CutMothedEntity
                    {
                        ICP = (float)ICP,
                        ECP = (float)ECP,
                        Name = "1"
                    };
                    List<CutMothedEntity> CutMothedEntityList = new List<CutMothedEntity>();
                    CutMothedEntityList.Add(newCutMothedEntity);
                    #endregion

                    CurveEntity curveEntityECP_TWY = oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TWY").FirstOrDefault();
                    CurveEntity result_CurveEntityECP_TWY = OilApplyBll.ResidueECP_WY_VYCurveCut(curveEntityECP_TWY, CutMothedEntityList);//切割TWY

                    CurveEntity curveEntityWY_SAH = oilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "SAH").FirstOrDefault();
                    CurveEntity curveEntityWY_ARS = oilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "ARS").FirstOrDefault();
                    CurveEntity curveEntityWY_RES = oilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "RES").FirstOrDefault();
                    CurveEntity curveEntityWY_APH = oilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "APH").FirstOrDefault();

                    CurveEntity result_curveEntityWY_SAH = OilApplyBll.ResidueCurveCut(curveEntityWY_SAH, result_CurveEntityECP_TWY);
                    CurveEntity result_curveEntityWY_ARS = OilApplyBll.ResidueCurveCut(curveEntityWY_ARS, result_CurveEntityECP_TWY);
                    CurveEntity result_curveEntityWY_RES = OilApplyBll.ResidueCurveCut(curveEntityWY_RES, result_CurveEntityECP_TWY);
                    CurveEntity result_curveEntityWY_APH = OilApplyBll.ResidueCurveCut(curveEntityWY_APH, result_CurveEntityECP_TWY);
                }
     
            }
            else if ((ICP >= 300 && ICP <= 400) && ECP > 1600)//STEP4
            {
                double SAH = 81.637 - 377.33 * D20 - 1.379 * SUL;
                double ARS = 12.761 + (10.913 - 2.128 * V10) * SUL + 198.73 * D20;
                double RES = 5.73 + (0.664 * V10 - 0.513 * SUL) * V10 + 657.218 * Math.Pow(D20, 2);
                double APH = 1.285 + 0.05233 * Math.Pow(CCR, 2) - 0.198 * Math.Pow(V10, 2);

                if (SAH < 0 || ARS < 0 || RES < 0)//如果有一个小于0，则不继续计算
                {
                    return returnDic;
                }
                else
                {
                    if (APH < -5)//如果APH小于-5，则不继续计算
                    {
                        return returnDic;
                    }
                    else if (APH < 0 && APH >= -5)//如果APH在0~-5之间，则置0
                    {
                        APH = 0;
                    }
                }

                double SUM = SAH + ARS + RES + APH;

                if (SUM == 0)
                    return returnDic;

                returnDic.Add("SAH", SAH / SUM * 100);//SAH=SAH/SUM*100
                returnDic.Add("ARS", ARS / SUM * 100);//ARS=ARS/SUM*100
                returnDic.Add("RES", RES / SUM * 100);//RES=RES/SUM*100
                returnDic.Add("APH", APH / SUM * 100);//APH=APH/SUM*100            
            }
            else if (ICP > 400 && ECP > 1600)//STEP5
            {
                double SAH = 104.098 - 858.579 * D20 + 1824.206 * Math.Pow(D20, 2);
                double ARS = 31.787 + 10.286 * SUL - 1.13 * Math.Pow(SUL, 2);
                double RES = -47.915 + 15.132 * V10 + CCR * (2.156 - 0.515 * V10);
                double APH = -0.856 + 0.021 * Math.Pow(CCR, 2);

                if (SAH < 0 || ARS < 0 || RES < 0)//如果有一个小于0，则不继续计算
                {
                    return returnDic;
                }
                else
                {
                    if (APH < -5)//如果APH小于-5，则不继续计算
                    {
                        return returnDic;
                    }
                    else if (APH < 0 && APH >= -5)//如果APH在0~-5之间，则置0
                    {
                        APH = 0;
                    }
                }

                double SUM = SAH + ARS + RES + APH;
                if (SUM == 0)
                    return returnDic;

                returnDic.Add("SAH", SAH / SUM * 100);//SAH=SAH/SUM*100
                returnDic.Add("ARS", ARS / SUM * 100);//ARS=ARS/SUM*100
                returnDic.Add("RES", RES / SUM * 100);//RES=RES/SUM*100
                returnDic.Add("APH", APH / SUM * 100);//APH=APH/SUM*100   
            }

            #endregion

            return returnDic;
        }
        /// <summary>
        /// 输入数值的累积加和,如果存在空值则继续加和
        /// </summary>
        /// <param name="tempList"></param>
        /// <returns>加和结果</returns>
        public static string FunSumAllowEmpty(List<string> tempList)
        {
            string strResult = string.Empty;

            if (tempList == null)
                return strResult;
            
            float sum = 0;
            foreach (string str in tempList)
            {
                float temp = 0;
                if (str != "非数字"&& float.TryParse(str, out temp))
                {
                    sum += temp;
                }
            }

            strResult = sum == 0 ? string.Empty : sum.ToString();

            return strResult;
        }
        /// <summary>
        /// 输入数值的累积加和。如果存在空值不能加和，返回空值。
        /// </summary>
        /// <param name="tempList"></param>
        /// <returns>加和结果</returns>
        public static string FunSumNotAllowEmpty(List<string> tempList)
        {
            string strResult = string.Empty;

            if (tempList == null)
                return strResult;

            float sum = 0;
            foreach (string str in tempList)
            {
                float temp = 0;
                if (!str.Equals (string.Empty) && float.TryParse(str, out temp))
                {
                    sum += temp;
                }
                else
                {
                    sum = 0;
                    break;
                }
            }

            strResult = sum == 0 ? string.Empty : sum.ToString();

            return strResult;
        }
        /// <summary>
        /// MCP, SG=>MW
        /// T=MCP+273.15
        /// MW==42.965*EXP(2.097/10000*T-7.78712*SG+2.08476/1000*T*SG)*T^1.26007*SG^4.98308
        /// </summary>
        /// <param name="strMCP"></param>
        /// <param name="srSG"></param>
        /// <returns></returns>
        public static string FunMWfromMCP_SG(string strMCP , string srSG)
        {
            string strResult = string.Empty;

            if (strMCP == string.Empty || srSG == string.Empty)
                return strResult;
            float T = 0, MCP = 0;
            if (float.TryParse(strMCP, out MCP))
            {
                T = MCP + 273.15f;
            }
            float SG = 0;
            if (float.TryParse(srSG, out SG))
            {
                double MW =42.965 * Math .Exp(2.097/10000*T-7.78712*SG+2.08476/1000*T*SG)*Math .Pow (T,1.26007) * Math .Pow(SG,4.98308);
                strResult = MW.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// ICP, ECP ,D20=>MW
	    /// MCP=(ICP+ECP)/2
        /// D20=>SG
        /// MCP, SG=>MW
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunMWfromICP_ECP_D20(string strICP, string strECP, string strD20)
        {
            string strResult = string.Empty;

            if (strICP == string.Empty || strECP == string.Empty || strD20 == string.Empty)
                return strResult;

            float MCP = 0, ICP = 0, ECP = 0;
            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                MCP = (ICP + ECP) / 2;               
            }

            string strSG = FunSG(strD20);
            strResult = FunMWfromMCP_SG(MCP.ToString(), strSG);

            return strResult;
        }

        /// <summary>
        /// MCP ,D20=>MW
        /// D20=>SG
        /// MCP, SG=>MW
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunMWfromMCP_D20(string strMCP, string strD20)
        {
            string strResult = string.Empty;

            if (strMCP == string.Empty || strD20 == string.Empty)
                return strResult;            

            string strSG = FunSG(strD20);
            strResult = FunMWfromMCP_SG(strMCP, strSG);

            return strResult;
        }



        /// <summary>
        /// D20, A10, A30, A50, A70,A90 =>MW
        /// MCP=(A10+A30+A50+A70+A90)/5
        /// D20=>SG
        /// MCP,SG=>MW
        /// </summary>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunMWfromA10_A30_A50_A70_A90_D20(string strA10, string strA30, string strA50, string strA70, string strA90, string strD20)
        {
            string strResult = string.Empty;

            if (strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty || strD20 == string.Empty)
                return strResult;

            float MCP = 0, A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0;
            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50) && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                MCP = (A10 + A30 + A50 + A70 + A90) / 5;
            }

            string strSG = FunSGfromD20(strD20);
            strResult = FunMWfromMCP_SG(MCP.ToString(), strSG);

            return strResult;
        }
       /// <summary>
       /// D20, V04, V10=>MW
       /// D20=>SG
       ///40, V04, 100, V10, 38 =>V38
       ///40, V04, 100, V10, 99 =>V99
       ///SG, V38, V99 =>MW
       /// <summary>
       /// <param name="strD20"></param>
       /// <param name="strV04"></param>
       /// <param name="strV10"></param>
       /// <returns></returns>
        public static string FunMWfromD20_V04_V10(string strD20 , string strV04,string strV10)
        {
            string strResult = string.Empty;

            if (strV04 == string.Empty || strV10 == string.Empty || strD20 == string.Empty)
                return strResult;

            string strV38 = FunV(strV04, strV10, "40", "100", "38");
            string strV99 = FunV(strV04, strV10, "40", "100", "99");
            string strSG = FunSG(strD20);

            strResult = FunMWfromSG_V38_V99(strSG, strV38, strV99);
              
            return strResult;
        }
        /// <summary>
        /// D20, V08, V10=>MW
        /// D20=>SG
        /// 80,V08, 100, V10, 38 =>V38
        /// 80,V08, 100, V10, 99 =>V99
        /// SG, V38, V99 =>MW
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strV08"></param>
        /// <param name="strV10"></param>
        /// <returns></returns>
        public static string FunMWfromD20_V08_V10(string strD20, string strV08, string strV10)
        {
            string strResult = string.Empty;

            if (strV08 == string.Empty || strV10 == string.Empty || strD20 == string.Empty)
                return strResult;
           
            string strV38 = FunV(strV08, strV10, "80", "100", "38");
            string strV99 = FunV(strV08, strV10, "80", "100", "99");
            string strSG = FunSG(strD20);

            strResult = FunMWfromSG_V38_V99(strSG, strV38, strV99);

            return strResult;
        }
        /// <summary>
        /// SG, V38, V99=>MW
        /// MW=223.56*(V(38)^(-1.2435+1.1228*SG)* V(99)^ (3.4758 -3.038 * SG ) ) * SG^(-0.6665)
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunMWfromSG_V38_V99(string srSG, string strV38, string strV99)
        {
            string strResult = string.Empty;

            if (strV38 == string.Empty || srSG == string.Empty || strV99 == string.Empty)
                return strResult;


            float SG = 0, V38 = 0, V99 = 0;
            if (float.TryParse(srSG, out SG) && float.TryParse(strV38, out V38) && float.TryParse(strV99, out V99))
            {
                double MW = 223.56 * Math.Pow(V38, (-1.2435 + 1.1228 * SG)) * Math.Pow(V99, (3.4758 - 3.038 * SG)) * Math.Pow(SG, -0.6665);
                if (!MW.Equals (double.NaN))
                    strResult = MW.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// API, ANI, SUL =>LHV
        /// IF API<30 OR ANI<50 OR SUL<0.01 THEN 退出
        /// LHV = (41.6796 + 0.00025407 * ((ANI * 1.8 + 32) * API)) * (1 - 0.01 * SUL) + 0.1016 * SUL
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunLHVfromAPI_ANI_SUL(string srAPI, string strANI, string strSUL)
        {
            string strResult = string.Empty;

            if (srAPI == string.Empty || strANI == string.Empty || strSUL == string.Empty)
                return strResult;

            float API = 0, ANI = 0, SUL = 0;
            if (float.TryParse(srAPI, out API) && float.TryParse(strANI, out ANI) && float.TryParse(strSUL, out SUL))
            {
                //应李工要求去掉限制条件
                //if (API < 30 || ANI < 50 || SUL < 0.01)
                //{
                //    return strResult;
                //}
                //else
                //{
                    double LHV = (41.6796 + 0.00025407 * ((ANI * 1.8 + 32) * API)) * (1 - 0.01 * SUL) + 0.1016 * SUL;
                    strResult = LHV.ToString();
                //}              
            }

            return strResult;
        }

        /// <summary>
        /// D20, ANI, SUL=>LHV
        /// D20=>API
        ///	API, ANI ,SUL=>LHV
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunLHVfromD20_ANI_SUL(string strD20, string strANI, string strSUL)
        {
            string strResult = string.Empty;

            if (strD20 == string.Empty || strANI == string.Empty || strSUL == string.Empty)
                return strResult;

            string strAPI = FunAPIfromD20(strD20);
            strResult = FunLHVfromAPI_ANI_SUL(strAPI, strANI, strSUL);

            return strResult;
        }

        /// <summary>
        /// D60=>API  API=141.5/D60*DH2O60F-131.5
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunAPIfromD60(string strD60)
        {
            string strResult = string.Empty;

            if (strD60 == string.Empty)
                return strResult;

            float D60 = 0;

            if (float.TryParse(strD60, out D60))
            {
                if (D60 == 0)
                    return strResult;

                float API = 141.5f / D60 * DH2O60F - 131.5f;//API=141.5/D60*DH2O60F-131.5
                strResult = API.ToString();
            }
            return strResult;
        }
        /// <summary>
        /// D15 =0.013574*D20^2+0.972625*D20+0.017101
        /// </summary>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunD15fromD20(string strD20)
        {
            string strResult = string.Empty;

            if (strD20 == string.Empty)
                return strResult;

            float D20 = 0;

            if (float.TryParse(strD20, out D20))
            {
                float D15 = (float)(0.013574 * Math.Pow(D20, 2) + 0.972625 * D20 + 0.017101);//D15 =0.013574*D20^2+0.972625*D20+0.017101
                strResult = D15.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// API=>D60 D60=141.5/(API+131.5)*DH2O60F
        /// </summary>
        /// <param name="strAPI"></param>
        /// <returns></returns>
        public static string FunD60fromAPI(string strAPI)
        {
            string strResult = string.Empty;

            float D60 = 0;

            if (strAPI == string.Empty)
                return strResult;

            float API = 0;

            if (float.TryParse(strAPI, out API))
            {
                D60 = 141.5f / (API + 131.5f) * DH2O60F;//D60=141.5/(API+131.5)*DH2O60F
                if (D60 != 0)
                {
                    strResult = D60.ToString();
                }
            }
            return strResult;
        }
        /// <summary>
        /// SG=>D60	D60=SG*DH2O60F
        /// </summary>
        /// <param name="strAPI"></param>
        /// <returns></returns>
        public static string FunD60fromSG(string strSG)
        {
            string strResult = string.Empty;
            float D60 = 0;

            if (strSG == string.Empty)
                return strResult;

            float SG = 0;

            if (float.TryParse(strSG, out SG))
            {
                D60 = SG * DH2O60F;//D60=SG*DH2O60F
                if (D60 != 0)
                {
                    strResult = D60.ToString();
                }
            }

            return strResult;
        }
        /// <summary>
        /// SG=>D60	D60=SG*DH2O60F
        /// </summary>
        /// <param name="strAPI"></param>
        /// <returns></returns>
        public static string FunD20fromSG(string strSG)
        {
            if (strSG == string.Empty)
                return string.Empty;

            string strD60 = FunD60fromSG(strSG);
            string strD20 = FunD20fromD60(strD60);

            return strD20;
        }
        /// <summary>
        /// D60=>D20   D20=-0.00546*D60^2+1.013143*D60-0.010325
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunD20fromD60(string strD60)
        {
            string strResult = string.Empty;

            float D20 = 0;

            if (strD60 == string.Empty)
                return strResult;

            float D60 = 0;

            if (float.TryParse(strD60, out D60))
            {
                D20 = (float)(-0.00546f * Math.Pow(D60, 2) + 1.013143 * D60 - 0.010325);//D20=-0.00546*D60^2+1.013143*D60-0.010325
                strResult = D20.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// D20=>D60  D60=0.005483*D20^2+0.987013*D20+0.010226
        /// </summary>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunD60fromD20(string strD20)
        {
            string strResult = string.Empty;

            float D60 = 0;

            if (strD20 == string.Empty)
                return strResult;

            float D20 = 0;
            if (float.TryParse(strD20, out D20))
            {
                D60 = (float)(0.005483 * Math.Pow(D20, 2) + 0.987013 * D20 + 0.010226);//D60=0.005483*D20^2+0.987013*D20+0.010226
                if (D60 != 0)
                {
                    strResult = D60.ToString();
                }
            }

            return strResult;
        }
        /// <summary>
        ///  D20=>D70  D70=-0.04769*D20^2+1.09984*D20-0.08628
        /// </summary>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunD70fromD20(string strD20)
        {
            string strResult = string.Empty;

            float D70 = 0;

            if (strD20 == string.Empty)
                return strResult;

            float D20 = 0;
            if (float.TryParse(strD20, out D20))
            {
                D70 = (float)(-0.04769 * Math.Pow(D20, 2) + 1.09984 * D20 - 0.08628);//D70=-0.04769*D20^2+1.09984*D20-0.08628
                strResult = D70.ToString();
            }
            return strResult;
        }
        /// <summary>
        /// D70=>D20  //D20=0.04599*D70^2+0.90665*D70+0.08138
        /// </summary>
        /// <param name="strD70"></param>
        /// <returns></returns>
        public static string FunD20fromD70(string strD70)
        {
            string strResult = string.Empty;

            float D20 = 0;

            if (strD70 == string.Empty)
                return strResult;

            float D70 = 0;
            if (float.TryParse(strD70, out D70))
            {
                D20 = (float)(0.04599 * Math.Pow(D70, 2) + 0.90665 * D70 + 0.08138);//D20=0.04599*D70^2+0.90665*D70+0.08138
                strResult = D20.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// API=>D20   API=>D60=>D20
        /// </summary>
        /// <param name="strD70"></param>
        /// <returns></returns>
        public static string FunD20fromAPI(string strAPI)
        {
            if (strAPI == string.Empty)
                return string.Empty;

            string strD60 = FunD60fromAPI(strAPI);

            string strD20 = FunD20fromD60(strD60);

            return strD20;
        }
        /// <summary>
        /// API=>D20   D20=>D60=>API
        /// </summary>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunAPIfromD20(string strD20)
        {
            if (strD20 == string.Empty)
                return string.Empty;

            string strD60 = FunD60fromD20(strD20);
             
            string strAPI = FunAPIfromD60(strD60);

            return strAPI;
        }
        /// <summary>
        ///X=(4.8448-SG)*(1.3185-4.6593/(5+LN(V10)))+8.24
        ///KFC=X/SG
        /// </summary>
        /// <param name="strV10"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunKFCfromV10_SG(string strV10, string strSG)
        {
            string strResult = string.Empty;
            
            if (strV10 == string.Empty || strSG == string.Empty)
                return strResult;

            float V10 = 0, SG = 0, KFC = 0;

            if (float.TryParse(strV10, out V10) && float.TryParse(strSG, out SG))
            {
                double temp = Math.Log(V10);
                double X = (4.8448 - SG) * (1.3185 - 4.6593 / (5.0 + temp)) + 8.24;
                
                if (SG == 0)
                    return strResult;

                KFC = (float)(X / SG);
                strResult = KFC.ToString();
            }
            return strResult;
        }
        /// <summary>
        ///NIV=NI+V
        /// </summary>
        /// <param name="strNI"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunNIVfromNI_V(string strNI, string strV)
        {
            string strResult = string.Empty;

            if (strNI == string.Empty || strV == string.Empty)
                return strResult;

            float V = 0, NI = 0, NIV = 0;

            if (float.TryParse(strNI, out NI) && float.TryParse(strV, out V))
            {
                NIV = NI + V;
                strResult = NIV.ToString();
            }
            return strResult;
        }
        /// <summary>
        /// V10,D20=>KFC
        /// </summary>
        /// <param name="strV10"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunKFCfromV10_D20(string strV10, string strD20)
        {
            if (strV10 == string.Empty || strD20 == string.Empty)
                return string.Empty;
             
            string strSG = FunSGfromD20(strD20);

            string strKFC = FunKFCfromV10_SG(strV10, strSG);

            return strKFC;
        }
        /// <summary>
        /// KFC==(1.216*((MCP+273.15)^(1/3)))/D60*DH2O60F
        /// </summary>
        /// <param name="strMCP"></param>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunKFCfromMCP_D60(string strMCP, string strD60)
        {
            string strResult = string.Empty;

            if (strMCP == string.Empty || strD60 == string.Empty)
                return strResult;

            float MCP = 0, D60 = 0, KFC = 0;

            if (float.TryParse(strMCP, out MCP) && float.TryParse(strD60, out D60))
            {
                KFC = (float)((1.216 * Math.Pow((MCP + 273.15), 1 / 3.0)) / D60 * DH2O60F);
                strResult = KFC.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// A10,A30,A50,A70,A90,D20=>KFC
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunKFCfromA10A30A50A70A90_D20(string strA10, string strA30, string strA50, string strA70, string strA90, string strD20)
        {//A10,A30,A50,A70,A90,D20
            if (strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty || strD20 == string.Empty)
                return string.Empty;

            float A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, MCP = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50) && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                MCP = (A10 + A30 + A50 + A70 + A90) / 5;
            }

            string strD60 = FunD60fromD20(strD20);

            string strKFC = FunKFCfromMCP_D60(MCP.ToString(), strD60);

            return strKFC;
        }
        /// <summary>
        /// ICP,ECP, D20=>KFC
        /// </summary>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunKFCfromICPECP_D20(string strICP, string strECP, string strD20)
        {    //ICP,ECP, D20
            if (strICP == string.Empty || strECP == string.Empty || strD20 == string.Empty)
                return string.Empty;

            float ICP = 0, ECP = 0, MCP = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                if (ECP - ICP <= 50)
                {
                    MCP = (ICP + ECP) / 2;
                }
            }

            string strD60 = FunD60fromD20(strD20);

            string strKFC = FunKFCfromMCP_D60(MCP.ToString(), strD60);

            return strKFC;
        }
        /// <summary>
        /// WYD(i)=WY(i)/(ECP(i)-ICP(i))
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strWY"></param>
        /// <returns></returns>
        public static string FunWYDfromICP_ECP_WY(string strICP, string strECP, string strWY)
        {    //WYD(i)=WY(i)/(ECP(i)-ICP(i))
            string strResult = string.Empty;

            if (strICP == string.Empty || strECP == string.Empty || strWY == string.Empty)
                return strResult;

            float ICP = 0, ECP = 0, WY = 0, WYD = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP) && float.TryParse(strWY, out WY))
            {
                if (ECP > ICP)
                {
                    WYD = WY / (ECP - ICP);
                    strResult = WYD.ToString();
                }
            }

            return strResult;
        }
        /// <summary>
        /// MWY(i)=TWY(i-1)+WY(i)/2
        /// </summary>
        /// <param name="strTWY"></param>
        /// <param name="strWY"></param>
        /// <returns></returns>
        public static string FunMWYfromTWY_WY(string strTWY, string strWY)
        {    //MWY(i)=TWY(i-1)+WY(i)/2
            string strResult = string.Empty;

            float TWY = 0, WY = 0, MWY = 0;

            if (strTWY != string.Empty && strWY != string.Empty)
            {
                if (float.TryParse(strTWY, out TWY) && float.TryParse(strWY, out WY))
                {
                    MWY = TWY + WY / 2;
                    strResult = MWY.ToString();
                }
            }
            else
            {
                return strResult;
            }
            return strResult;
        }
        /// <summary>
        /// MCP(i)=(ICP(i)+ECP(i))/2
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <returns></returns>
        public static string FunMCPfromICP_ECP(string strICP, string strECP)
        {    //MCP(i)=(ICP(i)+ECP(i))/2
            string strResult = string.Empty;

            if (strICP == string.Empty || strECP == string.Empty)
                return strResult;

            float ICP = 0, ECP = 0, MCP = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                MCP = (ICP + ECP) / 2;
                strResult = MCP.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// MCP, D20=>KFC
        /// </summary>
        /// <param name="strMCP"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunKFCfromMCP_D20(string strMCP, string strD20)
        {    //MCP, D20=>KFC
            if (strMCP == string.Empty || strD20 == string.Empty)
                return string.Empty;

            string strD60 = FunD60fromD20(strD20);

            string strKFC = FunKFCfromMCP_D60(strMCP, strD60);

            return strKFC;
        }
        /// <summary>
        ///对原油表，渣油表:  KFC=X/SG
        ///X=(4-SG)*(1-4.6593/(5+LN(V10)))+8.24
        ///SG=D60/DH2O60F
        /// </summary>
        /// <param name="strD60"></param>
        /// <param name="strV10cal">轻端表的V10值</param>
        /// <returns></returns>
        public static string FunKFC_WholeandResidue(string strD60, string strV10cal)
        {
            string strResult = string.Empty;           

            if (strD60 == string.Empty || strV10cal == string.Empty)
                return strResult;

            float KFC = 0, D60=0 ,V10 = 0;
            if (float.TryParse(strD60, out D60))
            {
                float SG =  D60 / DH2O60F;
                float X = (4 - SG) * (1f - 4.6593f / (5f + (float)Math.Log(V10))) + 8.24f;
                if (SG  == 0)
                    return strResult;

                KFC = X / SG;
                strResult = KFC.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// MCP,D60=>BMI
        /// </summary>
        /// <param name="strMCP"></param>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunBMIfromMCP_D60(string strMCP, string strD60)
        {//BMI=(48640/(MCP+273.15))+473.7*(D60/DH2O60F)-456.8
            if (strMCP == string.Empty || strD60 == string.Empty)
                return string.Empty;

            float MCP = 0, D60 = 0, BMI = 0;

            if (float.TryParse(strMCP, out MCP) && float.TryParse(strD60, out D60))
            {
                BMI = (float)((48640 / (MCP + 273.15)) + 473.7 * (D60 / DH2O60F) - 456.8);
            }

            return BMI.ToString();
        }
        /// <summary>
        /// A10,A30,A50,A70,A90,D200=>BMI
        /// </summary>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunBMIfromA10A30A50A70A90_D20(string strA10, string strA30, string strA50, string strA70, string strA90, string strD20)
        {//A10,A30,A50,A70,A90,D20
            if (strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty || strD20 == string.Empty)
                return string.Empty;
           
            float A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, MCP = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50) && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                MCP = (A10 + A30 + A50 + A70 + A90) / 5;
            }

            string strD60 = FunD60fromD20(strD20);

            string strBMI = FunBMIfromMCP_D60(MCP.ToString(), strD60);

            return strBMI;
        }
        /// <summary>
        /// ICP,ECP, D20=>BMI
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunBMIfromICPECP_D20(string strICP, string strECP, string strD20)
        {    //ICP,ECP, D20
            if (strICP == string.Empty || strECP == string.Empty || strD20 == string.Empty)
                return string.Empty;

            float ICP = 0, ECP = 0, MCP = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                if (ECP - ICP <= 50)
                {
                    MCP = (ICP + ECP) / 2;
                }
            }

            string strD60 = FunD60fromD20(strD20);

            string strBMI = FunBMIfromMCP_D60(MCP.ToString(), strD60);

            return strBMI;
        }
        /// <summary>
        /// MCP,D20=>BMI
        /// </summary>
        /// <param name="strMCP"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunBMIfromMCP_D20(string strMCP, string strD20)
        {    //MCP, D20=>KFC
            if (strMCP == string.Empty || strD20 == string.Empty)
                return string.Empty;

            string strD60 = FunD60fromD20(strD20);

            string strBMI = FunBMIfromMCP_D60(strMCP, strD60);

            return strBMI;
        }
        /// <summary>
        /// VG4=(D15-0.0664-0.1154*lg(V04-5.5))/(0.94-0.109*lg(V04-5.5))
        /// VG4=(D15-0.0664-0.1154*lg(V04-5.5))/(0.94-0.109*lg(V04-5.5))
        /// </summary>
        /// <param name="strD15"></param>
        /// <param name="strV04"></param>
        /// <returns></returns>
        public static string FunVG4fromD15andV04(string strD15, string strV04)
        {
            string strResult = string.Empty;

            if (strD15 == string.Empty || strV04 == string.Empty)
                return strResult;

            float D15 = 0, V04 = 0, VG4;

            if (float.TryParse(strD15, out D15) && float.TryParse(strV04, out V04))
            {
                float temp = V04 - 5.5f;

                if (temp > 0)
                {
                    double i = 0.1154 * Math.Log10(temp);
                    double j = 0.109 * Math.Log10(temp);
                    VG4 = (float)((D15 - 0.0664 - 0.1154 * Math.Log10(temp)) / (0.94 - 0.109 * Math.Log10(temp)));
                    strResult = VG4.ToString();
                }
            }

            return strResult;
        }

        /// <summary>
        /// VG4=(D15-0.0664-0.1154*lg(V04-5.5))/(0.94-0.109*lg(V04-5.5))
        /// VG4=(D15-0.0664-0.1154*lg(V04-5.5))/(0.94-0.109*lg(V04-5.5))
        /// </summary>
        /// <param name="strD15"></param>
        /// <param name="strV04"></param>
        /// <returns></returns>
        public static string FunVG4fromD20andV04(string strD20, string strV04)
        {
            string strResult = string.Empty;

            if (strD20 == string.Empty || strV04 == string.Empty)
                return strResult;

            string strD15 = FunD15fromD20(strD20);
            strResult = FunVG4fromD15andV04(strD15, strV04);
            
            return strResult;
        }


        /// <summary>
        /// V1G==(D15-0.108-0.1255*lg(V10-0.8))/(0.9-0.097*lg(V10-0.8))
        /// </summary>
        /// <param name="strD15"></param>
        /// <param name="strV10"></param>
        /// <returns></returns>
        public static string FunV1GfromD15andV10(string strD15, string strV10)
        {//V1G==(D15-0.108-0.1255*lg(V10-0.8))  /   (0.9-0.097*lg(V10-0.8))

            string strResult = string.Empty;

            if (strD15 == string.Empty || strV10 == string.Empty)
                return strResult;

            float D15 = 0, V10 = 0, V1G;

            if (float.TryParse(strD15, out D15) && float.TryParse(strV10, out V10))
            {
                float temp = V10 - 0.8f;

                if (temp > 0)
                {
                    V1G = (float)((D15 - 0.108 - 0.1255 * Math.Log10(temp)) / (0.9 - 0.097 * Math.Log10(temp)));
                    strResult = V1G.ToString();
                }
            }
            return strResult;
        }

        public static string FunV1GfromD20andV10(string strD20, string strV10)
        { 
            string strResult = string.Empty;

            if (strD20 == string.Empty || strV10 == string.Empty)
                return strResult;

            string strD15 = FunD15fromD20(strD20);
            strResult = FunV1GfromD15andV10(strD15, strV10);
            return strResult;
        }

        /// <summary>
        /// POR=SOP+3
        /// </summary>
        /// <param name="strSOP"></param>
        /// <returns></returns>
        public static string FunPOR(string strSOP)
        {
            string strResult = string.Empty;
           
            if (strSOP == string.Empty)
                return strResult;

            float POR = 0, SOP = 0;
            if (float.TryParse(strSOP, out SOP))
            {
                POR = SOP + 3;
                strResult = POR.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// SOP=POR-3
        /// </summary>
        /// <param name="strPOR"></param>
        /// <returns></returns>
        public static string FunSOP(string strPOR)
        {
            string strResult = string.Empty;           
            if (strPOR == string.Empty)
                return strResult;

            float POR = 0, SOP = 0;
            if (float.TryParse(strPOR, out POR))
            {
                SOP = POR - 3;
                strResult = SOP.ToString();
            }
            return strResult;
        }
        /// <summary>
        ///NET= ACD/D20/100
        /// </summary>
        /// <param name="strACD"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunNET(string strACD, string strD20)
        {
            string strResult = string.Empty;     

            if (strACD == string.Empty || strD20 == string.Empty)
                return strResult;

            float ACD = 0, D20 = 0, NET = 0;

            if (float.TryParse(strACD, out ACD) && float.TryParse(strD20, out D20))
            {
                NET = ACD / D20 / 100;
                strResult = NET.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// ACD= NET*D20*100;
        /// </summary>
        /// <param name="strNET"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunACD(string strNET, string strD20)
        {
            string strResult = string.Empty;             
            if (strNET == string.Empty || strD20 == string.Empty)
                return strResult;

            float ACD = 0, D20 = 0, NET = 0;
            if (float.TryParse(strNET, out NET) && float.TryParse(strD20, out D20))
            {
                ACD = NET * D20 * 100;
                strResult = ACD.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// //D20,MCP=>CI
        /// </summary>
        /// <param name="strMCP"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunCIfromMCP_D20(string strMCP, string strD20)
        {   //D20,MCP=>CI
            //CI=431.29- 1586.88 * D20+  730.97*     ((D20)^2)   +12.392*  ((D20)^3)  +0.0515*((D20)^4)-0.554*MCP+97.803*(LOG(MCP))^2
            string strResult = string .Empty ;

            float CI = 0, D20 = 0, MCP = 0;

            if (strMCP == string.Empty || strD20 == string.Empty)
                return strResult;
            //strMCP = "262.5"; strD20 = "0.855";
            if (float.TryParse(strMCP, out MCP) && float.TryParse(strD20, out D20))
            {
                if (D20 < 0.7 || D20 > 0.95 || MCP > 400 || MCP < 140)
                {
                    return strResult;
                }
                else
                {
                    CI = (float)(431.29 - 1586.88 * D20 + 730.97 * Math.Pow(D20, 2) + 12.392 * Math.Pow(D20, 3) + 0.0515 * Math.Pow(D20, 4) - 0.554 * MCP + 97.803 * Math.Pow(Math.Log10(MCP), 2));
                    strResult = CI.ToString();
                }
            }

            return strResult;
        }
        /// <summary>
        /// D20, A10, A30, A50, A70, A90 =>CI 
        /// </summary>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunCIfromA10A30A50A70A90_D20(string strA10, string strA30, string strA50, string strA70, string strA90, string strD20)
        {   //D20, A10, A30, A50, A70, A90 =>CI 
            if (strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty || strD20 == string.Empty)
                return string.Empty;

            float A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, MCP = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50) && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                MCP = (A10 + A30 + A50 + A70 + A90) / 5;
            }

            string strCI = FunCIfromMCP_D20(MCP.ToString(), strD20);

            return strCI;
        }
        /// <summary>
        /// ICP,ECP, D20=>CI  
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunCIfromICPECP_D20(string strICP, string strECP, string strD20)
        {    //ICP,ECP, D20=>CI            
            if (strICP == string.Empty || strECP == string.Empty || strD20 == string.Empty)
                return string.Empty;

            float ICP = 0, ECP = 0, MCP = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                MCP = (ICP + ECP) / 2;
            }

            string strCI = FunCIfromMCP_D20(MCP.ToString(), strD20);

            return strCI;
        }
        /// <summary>
        /// CEN=45.2+0.0892*(A10-215)+(0.901*  (    EXP((-3.5)*(D15-0.85))-1)+0.131)*(A50-260)+(-0.42*(EXP((-3.5)*(D15-0.85))-1)+0.0523)*(A90-310)+0.00049*((A10-215)^2-(A90-310)^2)+107*(EXP((-3.5)*(D15-0.85))-1)+60*(EXP((-3.5)*(D15-0.85))-1)^2
        /// </summary>
        /// <param name="strA10"></param>
        /// <param name="strA50"></param>
        /// <param name="strA90"></param>
        /// <param name="strD15"></param>
        /// <returns></returns>
        public static string FunCENfromA10A50A90_D15(string strA10, string strA50, string strA90, string strD15)
        {
            if (strA10 == string.Empty  || strA50 == string.Empty || strA90 == string.Empty || strD15 == string.Empty)
                return string.Empty;

            float CEN = 0, A10 = 0, A50 = 0, A90 = 0, D15 = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA50, out A50) && float.TryParse(strA90, out A90) && float.TryParse(strD15, out D15))
            {
                double temp = Math.Exp((-3.5) * (D15 - 0.85)) - 1;

                CEN = (float)(45.2 + 0.0892 * (A10 - 215) + (0.901 * temp + 0.131) * (A50 - 260) + (-0.42 * temp + 0.0523) * (A90 - 310) + 0.00049 * (Math.Pow((A10 - 215), 2) - Math.Pow((A90 - 310), 2)) + 107 * temp + 60 * Math.Pow(temp, 2));
            }

            return CEN.ToString();
        }
        /// <summary>
        /// D20,A10, A30, A50, A90=>CEN
        /// </summary>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA90"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunCENfromA10A30A50A90_D20(string strA10, string strA30, string strA50, string strA90, string strD20)
        {
            if (strA10 == string.Empty || strA50 == string.Empty || strA90 == string.Empty || strD20 == string.Empty)
                return string.Empty;

            string strD15 = FunD15fromD20(strD20);

            string strCEN = FunCENfromA10A50A90_D15(strA10,strA50, strA90, strD15);

            return strCEN;
        }
        /// <summary>
        /// D20,ANI->DI
        /// </summary>
        /// <param name="strAPI"></param>
        /// <param name="strANI"></param>
        /// <returns></returns>
        public static string FunDIfromD20_ANI(string strD20, string strANI)
        {
            string strResult = string.Empty;

            if (strD20 == string.Empty || strANI == string.Empty)
                return strResult;

            string strAPI = FunAPIfromD20(strD20);
            strResult = FunDI(strAPI, strANI);

            return strResult ;
        }
        /// <summary>
        /// DI=API*(9/5*ANI+32)/100
        /// </summary>
        /// <param name="strAPI"></param>
        /// <param name="strANI"></param>
        /// <returns></returns>
        public static string FunDI(string strAPI, string strANI)
        {
            string strResult = string.Empty;

            if (strAPI == string.Empty || strANI == string.Empty)
                return strResult;

            float DI = 0, API = 0, ANI = 0;

            if (float.TryParse(strAPI, out API) && float.TryParse(strANI, out ANI))
            {
                if (API < 15 || API > 65)
                {
                    return strResult;
                }
                else
                {
                    DI = (float)(API * (9.0 / 5.0 * ANI + 32) / 100);
                    strResult = DI.ToString();
                }
            }

            return strResult;
        }
        /// <summary>
        /// GCT=PAN+PAO+NAH+ARM+OLE+UNK
        /// </summary>
        /// <param name="strPAN"></param>
        /// <param name="strPAO"></param>
        /// <param name="strNAH"></param>
        /// <param name="strARM"></param>
        /// <param name="strOLE"></param>
        /// <param name="strUNK"></param>
        /// <returns></returns>
        public static string FunGCT(string strPAN, string strPAO, string strNAH, string strARM, string strOLE, string strUNK)
        {   //GCT=PAN+PAO+NAH+ARM+OLE+UNK
            string strResult = string.Empty;
            if (strPAN == string.Empty || strPAO == string.Empty || strNAH == string.Empty || strARM == string.Empty || strOLE == string.Empty || strUNK == string.Empty)
                return strResult;

            float PAN = 0, PAO = 0, NAH = 0, ARM = 0, OLE = 0, UNK = 0, GCT = 0;

            if (float.TryParse(strPAN, out PAN) && float.TryParse(strPAO, out PAO) && float.TryParse(strNAH, out NAH)
                && float.TryParse(strARM, out ARM) && float.TryParse(strOLE, out OLE) && float.TryParse(strUNK, out UNK))
            {
                GCT = PAN + PAO + NAH + ARM + OLE + UNK;
                strResult = GCT.ToString();
            }

            return strResult;
        }

        /// <summary>
        /// MIV=NI+V
        /// </summary>
        /// <param name="strNI"></param>
        /// <param name="strV"></param>
        /// <returns></returns>
        public static string FunMIVfromNI_V(string strNI, string strV)
        {
            string strResult = string.Empty;

            if (strNI == string.Empty || strV == string.Empty)
            {
                return strResult;
            }
            else
            {
                float NI = 0, V = 0,MIV=0;
                if (float.TryParse(strNI, out NI) && float.TryParse(strV, out V))
                {
                    MIV = NI + V;
                    if (MIV != 0)
                    {
                        strResult = MIV.ToString();
                    }
                }
            }

            return strResult;         
        }

        /// <summary>
        /// PAT=PAN+PAO
        /// </summary>
        /// <param name="strPAN"></param>
        /// <param name="strPAO"></param>
        /// <returns></returns>
        public static string FunPATfromPAN_PAO(string strPAN, string strPAO)
        {
            string strResult = string.Empty;

            if (strPAN == string.Empty || strPAO == string.Empty)
                return strResult;

            float PAT = 0, PAN = 0, PAO = 0;

            if (float.TryParse(strPAN, out PAN) && float.TryParse(strPAO, out PAO))
            {
                PAT = PAN + PAO;
                if (PAT != 0)
                {
                    strResult = PAT.ToString();
                }
            }
            return strResult;
        }

        /// <summary>
        /// CAR=100-H2-SUL-N2
        /// </summary>
        /// <param name="strPAN"></param>
        /// <param name="strPAO"></param>
        /// <returns></returns>
        public static string FunCARfromH2_SUL_N2(string strH2, string strSUL, string strN2)
        {
            string strResult = string.Empty;

            if (strH2 == string.Empty || strSUL == string.Empty || strN2 == string.Empty)
                return strResult;

            float H2 = 0, SUL = 0, N2 = 0 , CAR = 0 ;

            if (float.TryParse(strH2, out H2) && float.TryParse(strSUL, out SUL) && float.TryParse(strN2, out N2))
            {
                CAR = (float)(100 - H2 - SUL - N2/10000.0);
                strResult = CAR.ToString();
            }

            return strResult;
        }

        /// <summary>
        /// N2A=NAH+2*ARM
        /// </summary>
        /// <param name="strNAH"></param>
        /// <param name="strARM"></param>
        /// <returns></returns>
        public static string FunN2A(string strNAH, string strARM)
        {//N2A=NAH+2*ARM
            string strResult = string.Empty;
            if (strNAH == string.Empty || strARM == string.Empty)
                return strResult;

            float N2A = 0, NAH = 0, ARM = 0;

            if (float.TryParse(strNAH, out NAH) && float.TryParse(strARM, out ARM))
            {
                N2A = NAH + 2 * ARM;
                strResult = N2A.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// 4CT=SAH+ARS+RES+APH
        /// </summary>
        /// <param name="strSAH"></param>
        /// <param name="strARS"></param>
        /// <param name="strRES"></param>
        /// <param name="strAPH"></param>
        /// <returns></returns>
        public static string Fun4CT(string strSAH, string strARS, string strRES, string strAPH)
        {   //4CT=SAH+ARS+RES+APH
            string strResult = string.Empty;
            if (strSAH == string.Empty || strARS == string.Empty || strRES == string.Empty || strAPH == string.Empty)
                return strResult;

            float SAH = 0, ARS = 0, RES = 0, APH = 0, _4CT = 0;

            if (float.TryParse(strSAH, out SAH) && float.TryParse(strARS, out ARS) && float.TryParse(strRES, out RES) && float.TryParse(strAPH, out APH))
            {
                _4CT = SAH + ARS + RES + APH;
                strResult = _4CT.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// MNA=M02+M03+M04+M05+M06+M07
        /// </summary>
        /// <param name="strM02"></param>
        /// <param name="strM03"></param>
        /// <param name="strM04"></param>
        /// <param name="strM05"></param>
        /// <param name="strM06"></param>
        /// <param name="strM07"></param>
        /// <returns></returns>
        public static string FunMNA(string strM02, string strM03, string strM04, string strM05, string strM06, string strM07)
        {   //MNA=M02+M03+M04+M05+M06+M07
            string strResult = string.Empty;
            if (strM02 == string.Empty || strM03 == string.Empty || strM04 == string.Empty || strM05 == string.Empty || strM06 == string.Empty || strM07 == string.Empty)
                return strResult;

            float M02 = 0, M03 = 0, M04 = 0, M05 = 0, M06 = 0, M07 = 0, MNA = 0;

            if (float.TryParse(strM02, out M02) && float.TryParse(strM03, out M03) && float.TryParse(strM04, out M04)
                && float.TryParse(strM05, out M05) && float.TryParse(strM06, out M06) && float.TryParse(strM07, out M07))
            {
                MNA = M02 + M03 + M04 + M05 + M06 + M07;
                strResult = MNA.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// MSP=M01+MNA
        /// </summary>
        /// <param name="strM01"></param>
        /// <param name="strMNA"></param>
        /// <returns></returns>
        public static string FunMSP(string strM01, string strMNA)
        {//MSP=M01+MNA
            if (strM01 == string.Empty || strMNA == string.Empty)
                return string.Empty;

            float MSP = 0, M01 = 0, MNA = 0;

            if (float.TryParse(strM01, out M01) && float.TryParse(strMNA, out MNA))
            {
                MSP = M01 + 2 * MNA;
            }

            return MSP.ToString();
        }
        /// <summary>
        /// MA1=M08+M09+M10+M11+M12
        /// </summary>
        /// <param name="strM08"></param>
        /// <param name="strM09"></param>
        /// <param name="strM10"></param>
        /// <param name="strM11"></param>
        /// <param name="strM12"></param>
        /// <returns></returns>
        public static string FunMA1(string strM08, string strM09, string strM10, string strM11, string strM12)
        {   //MA1=M08+M09+M10+M11+M12
            if (strM08 == string.Empty || strM09 == string.Empty || strM10 == string.Empty || strM11 == string.Empty || strM12 == string.Empty)
                return string.Empty;

            float M08 = 0, M09 = 0, M10 = 0, M11 = 0, M12 = 0, MA1 = 0;

            if (float.TryParse(strM08, out M08) && float.TryParse(strM09, out M09) && float.TryParse(strM10, out M10)
                && float.TryParse(strM11, out M11) && float.TryParse(strM12, out M12))
            {
                MA1 = M08 + M09 + M10 + M11 + M12;
            }

            return MA1.ToString();
        }
        /// <summary>
        /// MA2=M12+M14+M15+M16+M17+M18
        /// </summary>
        /// <param name="strM12"></param>
        /// <param name="strM14"></param>
        /// <param name="strM15"></param>
        /// <param name="strM16"></param>
        /// <param name="strM17"></param>
        /// <param name="strM18"></param>
        /// <returns></returns>
        public static string FunMN2(string strM12, string strM14, string strM15, string strM16, string strM17, string strM18)
        {   //MA2=M12+M14+M15+M16+M17+M18
            if (strM12 == string.Empty || strM14 == string.Empty || strM15 == string.Empty || strM16 == string.Empty || strM17 == string.Empty || strM18 == string.Empty)
                return string.Empty;

            float M12 = 0, M14 = 0, M15 = 0, M16 = 0, M17 = 0, M18 = 0, MA2 = 0;

            if (float.TryParse(strM12, out M12) && float.TryParse(strM14, out M14) && float.TryParse(strM15, out M15)
                && float.TryParse(strM16, out M16) && float.TryParse(strM17, out M17) && float.TryParse(strM18, out M18))
            {
                MA2 = M12 + M14 + M15 + M16 + M17 + M18;
            }

            return MA2.ToString();
        }
        /// <summary>
        /// MA3=M19+M20
        /// </summary>
        /// <param name="strM19"></param>
        /// <param name="strM20"></param>
        /// <returns></returns>
        public static string FunMA3(string strM19, string strM20)
        {//MA3=M19+M20
            if (strM19 == string.Empty || strM20 == string.Empty)
                return string.Empty;

            float MA3 = 0, M19 = 0, M20 = 0;

            if (float.TryParse(strM19, out M19) && float.TryParse(strM20, out M20))
            {
                MA3 = M19 + M20;
            }

            return MA3.ToString();
        }
        /// <summary>
        /// MA4=M21+M22
        /// </summary>
        /// <param name="strM21"></param>
        /// <param name="strM22"></param>
        /// <returns></returns>
        public static string FunMA4(string strM21, string strM22)
        {//MA4=M21+M22
            if (strM21 == string.Empty || strM22 == string.Empty)
                return string.Empty;

            float MA4 = 0, M21 = 0, M22 = 0;

            if (float.TryParse(strM21, out M21) && float.TryParse(strM22, out M22))
            {
                MA4 = M21 + M22;
            }

            return MA4.ToString();
        }
        /// <summary>
        /// MA5=M23+M24
        /// </summary>
        /// <param name="strM23"></param>
        /// <param name="strM24"></param>
        /// <returns></returns>
        public static string FunMA5(string strM23, string strM24)
        {//MA5=M23+M24
            if (strM23 == string.Empty || strM24 == string.Empty)
                return string.Empty;

            float MA5 = 0, M23 = 0, M24 = 0;

            if (float.TryParse(strM23, out M23) && float.TryParse(strM24, out M24))
            {
                MA5 = M23 + M24;
            }

            return MA5.ToString();
        }
        /// <summary>
        /// MA5=M23+M24
        /// </summary>
        /// <param name="strM26"></param>
        /// <param name="strM27"></param>
        /// <param name="strM28"></param>
        /// <returns></returns>
        public static string FunMAN(string strM26, string strM27, string strM28)
        {//MA5=M23+M24
            if (strM26 == string.Empty || strM27 == string.Empty || strM28 == string.Empty)
                return string.Empty;

            float MAN = 0, M26 = 0, M27 = 0, M28 = 0;

            if (float.TryParse(strM26, out M26) && float.TryParse(strM27, out M27) && float.TryParse(strM28, out M28))
            {
                MAN = M26 + M27 + M28;
            }

            return MAN.ToString();
        }
        /// <summary>
        /// MAT=MA1+MA2+MA3+MA4+MA5
        /// </summary>
        /// <param name="strMA1"></param>
        /// <param name="strMA2"></param>
        /// <param name="strMA3"></param>
        /// <param name="strMA4"></param>
        /// <param name="strMA5"></param>
        /// <returns></returns>
        public static string FunMAT(string strMA1, string strMA2, string strMA3, string strMA4, string strMA5)
        {   //MAT=MA1+MA2+MA3+MA4+MA5
            if (strMA1 == string.Empty || strMA2 == string.Empty || strMA3 == string.Empty || strMA4 == string.Empty || strMA5 == string.Empty)
                return string.Empty;

            float MAT = 0, MA1 = 0, MA2 = 0, MA3 = 0, MA4 = 0, MA5 = 0;

            if (float.TryParse(strMA1, out MA1) && float.TryParse(strMA2, out MA2) && float.TryParse(strMA3, out MA3) && float.TryParse(strMA4, out MA4) && float.TryParse(strMA5, out MA5))
            {
                MAT = MA1 + MA2 + MA3 + MA4 + MA5;
            }

            return MAT.ToString();
        }
        public static string FunMTA(string strMAT, string strMRS, string strMSP)
        {   //MTA=MAT+MRS+MSP
            if (strMAT == string.Empty || strMRS == string.Empty || strMSP == string.Empty)
                return string.Empty;

            float MTA = 0, MAT = 0, MRS = 0, MSP = 0;

            if (float.TryParse(strMAT, out MAT) && float.TryParse(strMRS, out MRS) && float.TryParse(strMSP, out MSP))
            {
                MTA = MAT + MRS + MSP;
            }

            return MTA.ToString();
        }

        /// <summary>
        /// ARP=N06*78/84+A06+N07*92/98+A07+N08*106/112+A08(芳潜)
        /// ARP=N06*78/84+A06+N07*92/98+A07+N08*106/112+A08(芳潜)
        /// </summary>
        /// <param name="strN06"></param>
        /// <param name="strN07"></param>
        /// <param name="strN08"></param>
        /// <param name="strA06"></param>
        /// <param name="strA07"></param>
        /// <param name="strA08"></param>
        /// <returns></returns>
        public static string FunARP(string strN06, string strN07, string strN08, string strA06, string strA07, string strA08)
        {
            string strResult = string.Empty;

            if (strA06 == string.Empty || strA07 == string.Empty || strA08 == string.Empty || strN06 == string.Empty || strN07 == string.Empty || strN08 == string.Empty)
                return string.Empty;

            float N06 = 0, N07 = 0, N08 = 0, A06 = 0, A07 = 0, A08 = 0, ARP = 0;

            if (float.TryParse(strN06, out N06) && float.TryParse(strN07, out N07) && float.TryParse(strN08, out N08)
                && float.TryParse(strA06, out A06) && float.TryParse(strA07, out A07) && float.TryParse(strA08, out A08))
            {
                ARP = N06 * 78 / 84 + A06 + N07 * 92 / 98 + A07 + N08 * 106 / 112 + A08;
                strResult = ARP.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// V04,V10=>VI
        /// </summary>
        /// <param name="strV04"></param>
        /// <param name="strV10"></param>
        /// <returns></returns>
        public static string FunVIfromV04_V10(string strV04, string strV10)
        {
            string strResult = string.Empty;
            if (strV04 == string.Empty || strV10 == string.Empty)
                return strResult;

            LinkSupplementParmAccess LinkSupplementAccess = new LinkSupplementParmAccess();
            List<LinkSupplementParmEntity> parms = LinkSupplementAccess.Get("1=1");

            float L = 0;
            float H = 0;

            float V10 = 0;
            float V04 = 0;


            if (!(float.TryParse(strV10, out V10) && float.TryParse(strV04, out V04)))//判断是否可以转换为数字
                return string.Empty;


            if (V10 <= 70)
            {
                for (int i = 0; i < parms.Count - 1; i++)
                {
                    if (parms[i].a < V10 && parms[i + 1].a > V10)
                    {
                        L = (V10 - parms[i].a) / (parms[i + 1].a - parms[i].a) * (parms[i + 1].b - parms[i].b) + parms[i].b;
                        H = (V10 - parms[i].a) / (parms[i + 1].a - parms[i].a) * (parms[i + 1].c - parms[i].c) + parms[i].c;
                    }
                    else if (parms[i].a == V10)
                    {
                        L = parms[i].b;
                        H = parms[i].c;
                    }
                }
            }
            else
            {
                L = (float)(0.8353 * V10 * V10 + 14.67 * V10 - 216);
                H = (float)(0.6669 * V10 * V10 + 2.82 * V10 - 119);
            }


            if (L == H)
            {
                //throw new Exception("输入参数有误!");  
                return string.Empty;
            }

            int VI = (int)((L - V04) / (L - H) * 100);

            if (VI < 0 || VI > 100)
            {
                if (V10 > 70)
                {
                    H = 0.1684f * V10 * V10 + 11.85f * V10 - 97;
                }
                if (V10 > 0)
                {
                    //float N = (float)((Math.Log10(H) / Math.Log10(10) - Math.Log10(V04) / Math.Log10(10)) / (Math.Log10(V10) / Math.Log10(10)));
                    float N = (float)((Math.Log10(H) - Math.Log10(V04)) / Math.Log10(V10));
                    double temp =Math.Pow(10.0, N) ;

                    VI = (int)(( temp - 1) / 0.00715 + 100);
                }
                else
                {
                    //throw new Exception("输入参数有误!");
                    return string.Empty;
                }
            }

            return VI.ToString();
        }
        /// <summary>
        /// V08,V10=>VI
        /// </summary>
        /// <param name="strV04"></param>
        /// <param name="strV08"></param>
        /// <returns></returns>
        public static string FunVIfromV08_V10(string strV08, string strV10)
        {
            string strResult = string.Empty;

            if (strV10 == string.Empty || strV08 == string.Empty)
                return strResult;

            float V08 = 0, V10 = 0;

            if (float.TryParse(strV08, out V08) && float.TryParse(strV10, out V10))//判断是否可以转换为数字
            {
                //strResult =          
            }

            return strResult;
        }
        /// <summary>
        /// D20=WY/VY*D20(原油)
        /// </summary>
        /// <param name="strWY"></param>
        /// <param name="strVY"></param>
        /// <param name="strWholeD20"></param>
        /// <returns></returns>
        public static string FunD20fromWY_VY_WholeD20(string strWY, string strVY, string strWholeD20)
        {
            string strResult = string.Empty;
                    
            if (strVY == string.Empty || strWY == string.Empty || strWholeD20 == string.Empty)
                return string.Empty;

            float  VY = 0, WY = 0, WholeD20 = 0;

            if (float.TryParse(strVY, out VY) && float.TryParse(strWY, out WY) && float.TryParse(strWholeD20, out WholeD20))
            {
                if (VY != 0)
                {
                    WY = WY / VY * WholeD20;
                    strResult = WY.ToString();
                }
            }

            return strResult;
        }
        /// <summary>
        /// WY(i)=VY(i)*D20(i)/D20(CRU) 
        /// </summary>
        /// <param name="strVY"></param>
        /// <param name="strD20"></param>
        /// <param name="strWholeD20">原油性质的D20</param>
        /// <returns></returns>
        public static string FunWY(string strVY, string strD20,string strWholeD20)
        {
            string strResult = string.Empty;

            //WY(i)=VY(i)*D20(i)/D20(CRU)           
            if (strVY == string.Empty || strD20 == string.Empty || strWholeD20 == string.Empty)
                return string.Empty;

            float D20 = 0, VY = 0, WY = 0, WholeD20 = 0;

            if (float.TryParse(strVY, out VY) && float.TryParse(strD20, out D20) && float.TryParse(strWholeD20, out WholeD20))
            {
                WY = VY * D20 / WholeD20;
                strResult = WY.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// TWY(i)=TWY(i-1)+WY(i)  
        /// </summary>
        /// <param name="strTWYbefore"></param>
        /// <param name="strWY"></param>
        /// <returns></returns>
        public static string FunTWY(string strNarrowTWY, string strWY)
        {
            string strResult = string.Empty;
            if (strNarrowTWY == string.Empty || strWY == string.Empty)
                return strResult;

            float TWYbefore = 0;
            float WY = 0;
            float TWY = 0;

            if (float.TryParse(strNarrowTWY, out TWYbefore) && float.TryParse(strWY, out WY))
            {
                TWY = TWYbefore + WY;
                strResult = TWY.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// VY(i)=WY(i)/D20(i)*D20(CRU)
        /// </summary>
        /// <param name="strWY"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        //public static string FunVY(string strWY, string strD20)
        //{//VY(i)=WY(i)/D20(i)*D20(CRU)
        //    if (strWY == string.Empty || strD20 == string.Empty)
        //        return null;

        //    var ds = this._parent.Oil.OilDatas.Where(d => d.OilTableTypeID == (int)EnumTableType.Whole).ToList();//D20(CRU)
        //    OilDataEntity oilDataD20Whole = ds.Where(o => o.OilTableRow.itemCode == "D20").FirstOrDefault();//选出原油性质的D20校正值 

        //    if (oilDataD20Whole == null)
        //        return string.Empty;

        //    float D20 = 0, VY = 0, WY = 0, WholeD20 = 0;
        //    if (oilDataD20Whole != null)
        //    {
        //        if (float.TryParse(strWY, out WY) && float.TryParse(strD20, out D20) && float.TryParse(oilDataD20Whole.calData, out WholeD20))
        //        {
        //            VY = WY * D20 / WholeD20;
        //        }
        //    }

        //    return VY.ToString();
        //}
        public static string FunVY(string strWY, string strD20, string strWholeD20)
        {
            string strResult = string.Empty;

            //VY(i)=WY(i)/D20(i)*D20(CRU)
            if (strWY == string.Empty || strD20 == string.Empty || strWholeD20 == string.Empty)
                return strResult;
            
            float D20 = 0, VY = 0, WY = 0, WholeD20 = 0;

            if (float.TryParse(strWY, out WY) && float.TryParse(strD20, out D20) && float.TryParse(strWholeD20, out WholeD20))
            {
                VY = WY / D20 * WholeD20;
                strResult = VY.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// TVY(i)=[NCUTS(ECP)=WCUTS(ICP)].TVY+VY(i)  
        /// </summary>
        /// <param name="strTVYbefore"></param>
        /// <param name="strVY"></param>
        /// <returns></returns>
        public static string FunTVY(string strNarrowTVY, string strVY)
        {   /*TVY(i)=TVY(i-1)+VY(i)*/
            string strResult = string.Empty;
            if (strNarrowTVY == string.Empty || strVY == string.Empty)
                return strResult;

            float TVYbefore = 0;
            float VY = 0;
            float TVY = 0;

            if (float.TryParse(strNarrowTVY, out TVYbefore) && float.TryParse(strVY, out VY))
            {
                TVY = TVYbefore + VY;
                strResult = TVY.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// C/H=CAR/H2
        /// </summary>
        /// <param name="strCAR"></param>
        /// <param name="strH2"></param>
        /// <returns></returns>
        public static string FunC_H(string strCAR, string strH2)
        {
            string strRestult = string.Empty;

            if (strCAR == string.Empty || strH2 == string.Empty)
                return strRestult;
            float C_H = 0, CAR = 0, H2 = 0;

            if (float.TryParse(strCAR, out CAR) && float.TryParse(strH2, out H2))
            {
                if (H2 != 0)
                {
                    C_H = CAR / H2;
                }

                strRestult = C_H.ToString();
            }

            return strRestult;
        }

        /// <summary>
        /// //API,MCP=>SMK
        ///IF 100<=MCP<=280。否则，退出算法。
        ///SMK=0.839*API+0.0182634*(MCP+273.15)-22.97
        /// </summary>
        /// <param name="strAPI"></param>
        /// <param name="strMCP"></param>
        /// <returns></returns>
        public static string FunSMKfromAPI_MCP(string strAPI, string strMCP)
        {   //API,MCP=>SMK
            //IF 100<=MCP<=280。否则，退出算法。
            //SMK=0.839*API+0.0182634*(MCP+273.15)-22.97
            
            string strRestult = string.Empty;

            if (strAPI == string.Empty || strMCP == string.Empty)
                return strRestult;

            float SMK = 0, API = 0, MCP = 0;

            if (float.TryParse(strAPI, out API) && float.TryParse(strMCP, out MCP))
            {
                if (MCP >= 100 && MCP <= 280)
                {
                    SMK = (float)(0.839 * API + 0.0182634 * (MCP + 273.15) - 22.97);
                    strRestult = SMK.ToString();
                }
            }

            return strRestult;
        }

        /// <summary>
        /// ANI,SG => SMK
        /// </summary>
        /// <param name="strANI"></param>
        /// <param name="strSG"></param>
        /// <returns></returns>
        public static string FunSMKfromANI_SG(string strANI, string strSG)
        {
            string strRestult = string.Empty;

            return strRestult;
        }

        /// <summary>
        /// API,ICP, ECP =>SMK
        /// </summary>
        /// <param name="strAPI"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <returns></returns>
        public static string FunSMKfromAPI_ICP_ECP(string strAPI, string strICP, string strECP)
        {   // API,ICP, ECP =>SMK
            // IF ECP-ICP<=50。 否则，退出。
            //MCP=(ICP+ECP)/2
            //API,MCPSMK

            string strRestult = string.Empty;

            if (strAPI == string.Empty || strICP == string.Empty || strECP == string.Empty)
                return strRestult;

            float API = 0, MCP = 0, ICP = 0, ECP = 0;

            if (float.TryParse(strAPI, out API) && float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                if ((ECP - ICP) <= 50)
                {
                    MCP = (ICP + ECP) / 2;
                    strRestult = FunSMKfromAPI_MCP(strAPI, MCP.ToString());
                }
            }

            return strRestult;
        }

        /// <summary>
        /// 工具箱用
        /// </summary>
        /// <param name="strAPI"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <returns></returns>
        public static string FunSMKfromAPI_ICP_ECP_For_Tool(string strAPI, string strICP, string strECP)
        {   // API,ICP, ECP =>SMK
            // IF ECP-ICP<=50。 否则，退出。
            //MCP=(ICP+ECP)/2
            //API,MCPSMK

            string strRestult = string.Empty;

            if (strAPI == string.Empty || strICP == string.Empty || strECP == string.Empty)
                return strRestult;

            float API = 0, MCP = 0, ICP = 0, ECP = 0;

            if (float.TryParse(strAPI, out API) && float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                MCP = (ICP + ECP) / 2;
                strRestult = FunSMKfromAPI_MCP(strAPI, MCP.ToString());
            }

            return strRestult;
        }

        /// <summary>
        /// D20,A10, A30, A50, A70 ,A90 =>SMK
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <returns></returns>
        public static string FunSMKfromD20_A10_A30_A50_A70_A90(string strD20, string strA10, string strA30, string strA50, string strA70, string strA90)
        {
            //D20,A10, A30, A50, A70 ,A90 =>SMK
            //IF 100<=A10,& A90<=280。否则，退出算法。
            //MCP=(A10+A30+A50+A70+A90)/5
            //  D20 =>API	
            //API,MCP =>SMK

            string strRestult = string.Empty;

            if (strD20 == string.Empty || strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty)
                return strRestult;

            float A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, MCP = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50)
                && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                if (A10 >= 100 && A90 <= 280)
                {
                    MCP = (A10 + A30 + A50 + A70 + A90) / 5;
                }
            }

            string strAPI = FunAPIfromD20(strD20);
            strRestult = FunSMKfromAPI_MCP(strAPI, MCP.ToString());

            return strRestult;
        }

        /// <summary>
        /// H/C->C/H 
        /// </summary>
        /// <param name="strC1H"></param>
        /// <returns></returns>
        public static string FunC1HfromH1C(string strH1C)
        {
            string strRestult = string.Empty;

            if (strH1C == string.Empty)
                return strRestult;

            float H1C = 0, C1H = 0;

            if (float.TryParse(strH1C, out H1C))
            {
                C1H = (float)(11.9147 / H1C);
                strRestult = C1H.ToString();
            }

            return strRestult;
        }

        /// <summary>
        /// C/H-> H/C
        /// </summary>
        /// <param name="strC1H"></param>
        /// <returns></returns>
        public static string FunH1CfromC1H(string strC1H)
        {
            string strRestult = string.Empty;

            if (strC1H == string.Empty)
                return strRestult;

            float H1C = 0, C1H = 0;

            if (float.TryParse(strC1H, out C1H))
            {
                H1C = (float)(11.9147 / C1H);
                strRestult = H1C.ToString();
            }

            return strRestult;
        }

        /// <summary>
        /// C/H, SUL->H2
        /// </summary>
        /// <param name="strC1H"></param>
        /// <returns></returns>
        public static string FunH2fromC1H_SUL(string strC1H,string strSUL)
        {
            string strRestult = string.Empty;

            if (strC1H == string.Empty || strSUL == string.Empty)
                return strRestult;

            float H2 = 0, C1H = 0,SUL=0;

            if (float.TryParse(strC1H, out C1H) && float.TryParse(strSUL, out SUL))
            {
                H2 = (float)((100 - SUL) / (1 + C1H));
                strRestult = H2.ToString();
            }

            return strRestult;
        }


        /// <summary>
        /// H2,SUL->C/H  碳氢比
        /// </summary>
        /// <param name="strC1H"></param>
        /// <returns></returns>
        public static string FunC1HfromH2_SUL(string strH2, string strSUL)
        {
            string strRestult = string.Empty;

            if (strH2 == string.Empty || strSUL == string.Empty)
                return strRestult;

            float H2 = 0, C1H = 0, SUL = 0;

            if (float.TryParse(strH2, out H2) && float.TryParse(strSUL, out SUL))
            {
                C1H = (float)((100 - SUL) / H2 - 1);
                strRestult = C1H.ToString();
            }

            return strRestult;
        }

        /// <summary>
        /// SG=>C/H
        /// </summary>
        /// <param name="strSG"></param>
        /// <returns></returns>
        public static string FunC1HfromSG(string strSG)
        {
            string strRestult = string.Empty;

            if (strSG == string.Empty)
                return strRestult;

            float C1H = 0, SG = 0;

            if (float.TryParse(strSG, out SG))
            {
                C1H = (float)(32.37 - 21.63 * SG);
                strRestult = C1H.ToString();
            }

            return strRestult;
        }
        /// <summary>
        ///  SG, MCP=>C/H
        /// </summary>
        /// <param name="strSG"></param>
        /// <param name="strMCP"></param>
        /// <returns></returns>
        public static string FunC1HfromSG_MCP(string strSG, string strMCP)
        {
            // SG, MCP=>C/H
            // Tb=MCP+273.15           
            //CH=3.4707*EXP(1.485/100*Tb+16.94*SG-1.2492/100*Tb*SG)*Tb^(-2.725)*SG^(-6.798)

            string strRestult = string.Empty;

            if (strSG == string.Empty || strMCP == string.Empty)
                return strRestult;

            float C1H = 0, SG = 0, MCP = 0;

            if (float.TryParse(strSG, out SG) && float.TryParse(strMCP, out MCP))
            {
                float Tb = MCP + 273.15f;
                if (Tb != 0 && SG != 0)
                {
                    C1H = (float)(3.4707 * Math.Exp(1.485 / 100 * Tb + 16.94 * SG - 1.2492 / 100 * Tb * SG) * Math.Pow(Tb, -2.725) * Math.Pow(SG, -6.798));
                    strRestult = C1H.ToString();
                }
            }

            return strRestult;
        }
        /// <summary>
        /// D20,MCP=>C/H
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strMCP"></param>
        /// <returns></returns>
        public static string FunC1HfromD20_MCP(string strD20, string strMCP)
        {   //D20,MCP=>C/H
            string strRestult = string.Empty;

            if (strD20 == string.Empty || strMCP == string.Empty)
                return strRestult;

            string strSG = FunSGfromD20(strD20);
            strRestult = FunC1HfromSG_MCP(strSG, strMCP);

            return strRestult;
        }
        /// <summary>
        /// D20, A10, A30, A50, A70,A90=>C/H
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <returns></returns>
        public static string FunC1HfromD20_A10_A30_A50_A70_A90(string strD20, string strA10, string strA30, string strA50, string strA70, string strA90)
        {
            string strRestult = string.Empty;

            if (strD20 == string.Empty || strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty)
                return strRestult;

            float A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, MCP = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50)
                && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                MCP = (A10 + A30 + A50 + A70 + A90) / 5;
                strRestult = FunC1HfromD20_MCP(strD20, MCP.ToString());
            }



            return strRestult;
        }

        /// <summary>
        /// D20,MCP=>FRZ
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strMCP"></param>
        /// <returns></returns>
        public static string FunFRZfromD20_MCP(string strD20, string strMCP)
        {   //D20,MCP=>FRZ
            string strRestult = string.Empty;

            if (strD20 == string.Empty || strMCP == string.Empty)
                return strRestult;

            float SG = 0, MCP = 0, T = 0;

            if (float.TryParse(strMCP, out MCP))
            {
                if (MCP >= 15 && MCP <= 560)
                {
                    T = MCP + 273.15f;
                }
                else
                {
                    return strRestult;
                }
            }

            string strSG = FunSGfromD20(strD20);

            if (float.TryParse(strSG, out SG))
            {
                double MW = 1.6607 * 0.0001 * Math.Pow(T, 2.1962) * Math.Pow(SG, -1.0164);
                double I = 0.3773 * Math.Pow(T, -0.02269) * Math.Pow(SG, 0.9182);
                double n = Math.Pow((1 + 2 * I) / (1 - I), 1 / 2.0);
                double m = MW * (n - 1.475);

                double Xp = 3.7387 - 4.0829 * SG + 0.014772 * m;
                double Xn = -1.5027 + 2.10152 * SG - 0.02388 * m;
                double Xa = 1 - Xp - Xn;
                //double i = Math.Log(1070 - T);
                //double j = 1 / 0.02013 * (6.98291 - i);
                //double k = Math.Pow(j, 3.0 / 2);

                double Mp = Math.Pow(1 / 0.02013 * (6.98291 - Math.Log(1070 - T)), 3.0 / 2);
                double Mn = Math.Pow(1 / 0.02239 * (6.95649 - Math.Log(1028 - T)), 3.0 / 2);
                double Ma = Math.Pow(1 / 0.02247 * (6.91062 - Math.Log(1015 - T)), 3.0 / 2);

                double TmP = 397 - Math.Exp(6.5096 - 0.14187 * Math.Pow(Mp, 0.47));
                double TmN5 = 370 - Math.Exp(6.52504 - 0.04945 * Math.Pow(Mn, 2.0 / 3));

                //double TmN6 = 360 - Math.Exp(6.55942 - 0.04681 * Math.Pow(Mn, 0.7));
                double TmA = 375 - Math.Exp(6.53599 - 0.04912 * Math.Pow(Ma, 2.0 / 3));
                double TmN = TmN5;// *0.5 + TmN6 * 0.5;

                double FRZ = Xp * TmP + Xn * TmN + (1 - Xp - Xn) * TmA - 273.15;
                if (FRZ.ToString() != "非数字")
                strRestult = FRZ.ToString();
            }
            return strRestult;
        }
        /// <summary>
        /// D20,ICP, ECP=>FRZ
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <returns></returns>
        public static string FunFRZfromD20_ICP_ECP(string strD20, string strICP, string strECP)
        {
            // IF 100<=ECP<=280。否则，退出算法。
            //IF ECP-ICP<=50。 否则，退出。
            //MCP=(ICP+ECP)/2
            //D20,MCP FRZ

            string strRestult = string.Empty;

            if (strD20 == string.Empty || strICP == string.Empty || strECP == string.Empty)
                return strRestult;

            float MCP = 0, ICP = 0, ECP = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                if (ECP <= 560 && ICP >= 15)
                {
                    if ((ECP - ICP) <= 50)
                    {
                        MCP = (ICP + ECP) / 2;
                        strRestult = FunFRZfromD20_MCP(strD20, MCP.ToString());
                    }
                }
            }

            return strRestult;
        }
        /// <summary>
        /// 工具箱用
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <returns></returns>
        public static string FunFRZfromD20_ICP_ECP_For_Tool(string strD20, string strICP, string strECP)
        {
            // IF 100<=ECP<=280。否则，退出算法。
            //IF ECP-ICP<=50。 否则，退出。
            //MCP=(ICP+ECP)/2
            //D20,MCP FRZ

            string strRestult = string.Empty;

            if (strD20 == string.Empty || strICP == string.Empty || strECP == string.Empty)
                return strRestult;

            float MCP = 0, ICP = 0, ECP = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                MCP = (ICP + ECP) / 2;
                strRestult = FunFRZfromD20_MCP(strD20, MCP.ToString());
            }

            return strRestult;
        }
        /// <summary>
        /// D20,A10, A30, A50, A70 ,A90=>FRZ
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <returns></returns>
        public static string FunFRZfromD20_A10_A30_A50_A70_A90(string strD20, string strA10, string strA30, string strA50, string strA70, string strA90)
        {
            //D20,A10, A30, A50, A70 ,A90=>FRZ
            //IF 100<=A10,& A90<=280。否则，退出算法。
            //MCP=(A10+A30+A50+A70+A90)/5
            //D20,MCP FRZ

            string strRestult = string.Empty;

            if (strD20 == string.Empty || strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty)
                return strRestult;

            float A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, MCP = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50)
                && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                if (A10 >= 15 && A90 <= 560)
                {
                    MCP = (A10 + A30 + A50 + A70 + A90) / 5;
                    strRestult = FunFRZfromD20_MCP(strD20, MCP.ToString());
                }
            }

            return strRestult;
        }
        /// <summary>
        /// V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度
        /// </summary>
        /// <param name="strV1">V1</param>
        /// <param name="strV2">V2</param>
        /// <param name="strT1">t1</param>
        /// <param name="strT2">t2</param>
        /// <param name="strT"></param>
        /// <returns></returns>
        public static string FunV(string strV1, string strV2, string strT1, string strT2, string strT)
        {
            string strResult = string.Empty;

            if (strV1 == string.Empty || strV2 == string.Empty || strT1 == string.Empty || strT2 == string.Empty || strT == string.Empty)
                return strResult;

            float V1 = 0, V2 = 0, T1 = 0, T2 = 0, T = 0, V = 0;

            if (float.TryParse(strV1, out V1) && float.TryParse(strV2, out V2)
             && float.TryParse(strT1, out T1) && float.TryParse(strT2, out T2) && float.TryParse(strT, out T))
            {

                double X1 = Math.Log10 (T1 + 273.15f);
                double X2 = Math.Log10 (T2 + 273.15f);
                double X = Math.Log10(T + 273.15f);

                double tempY1 = Math.Log10(V1 + 0.6);
                double tempY2 = Math.Log10(V2 + 0.6);
                if (tempY1 > 0 && tempY2 > 0)
                {
                    double Y1 = Math.Log10(tempY1);
                    double Y2 = Math.Log10(tempY2);
                    double Y3 = (Y2 - Y1) / (X2 - X1) * (X - X1) + Y1;
                    V = (float)Math.Pow(10, Math.Pow(10, Y3)) - 0.6f;
                }
                strResult = V.ToString();
            }
            else
                return strResult;

            return strResult;
        }
        /// <summary>
        /// D20,MCP =>SAV, ARV
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strMCP"></param>
        /// <returns></returns>
        public static Dictionary<string, float> FunSAV_ARVfromD20_MCP(string strD20, string strMCP)
        {
            //D20,MCP =>SAV, ARV
            //IF 100<=MCP<=280。否则，退出算法。
            //T=MCP+273.15
            //D20=>SG

            Dictionary<string, float> returnList = new Dictionary<string, float>();

            if (strD20 == string.Empty || strMCP == string.Empty)
                return returnList;

            float D20 = 0, MCP = 0, SG = 0, T = 0;

            if (float.TryParse(strD20, out D20) && float.TryParse(strMCP, out MCP))
            {
                if (MCP >= 100 && MCP <= 280)
                {
                    T = MCP + 273.15f;
                }
                else
                {
                    return returnList;
                }
            }
            else
            {
                return returnList;
            }

            string strSG = FunSGfromD20(strD20);

            if (float.TryParse(strSG, out SG))
            {
                double MW = 1.6607 * 0.0001 * Math.Pow(T, 2.1962) * Math.Pow(SG, -1.0164);
                double I = 0.3773 * Math.Pow(T, -0.02269) * Math.Pow(SG, 0.9182);
                double n = Math.Pow((1 + 2 * I) / (1 - I), 1 / 2.0);
                double m = MW * (n - 1.475);

                double Xp = 3.7387 - 4.0829 * SG + 0.014772 * m;
                double Xn = -1.5027 + 2.10152 * SG - 0.02388 * m;

                float ARV = (float)((1 - Xp - Xn) / 2 * 100);
                float SAV = (float)(100 - ARV);

                returnList.Add("ARV", ARV);
                returnList.Add("SAV", SAV);
            }


            return returnList;
        }
        /// <summary>
        /// D20, ICP, ECP =>SAV, ARV
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <returns>字典键中输入SAV, ARV的到对应的值</returns>
        public static Dictionary<string, float> FunSAV_ARVfromD20_ICP_ECP(string strD20, string strICP, string strECP)
        {
            Dictionary<string, float> returnList = new Dictionary<string, float>();

            if (strD20 == string.Empty || strICP == string.Empty || strECP == string.Empty)
                return returnList;

            float MCP = 0, ICP = 0, ECP = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                if (ECP <= 280 && ECP >= 100)
                {
                    if ((ECP - ICP) <= 50)
                    {
                        MCP = (ICP + ECP) / 2;
                        returnList = FunSAV_ARVfromD20_MCP(strD20, MCP.ToString());
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// 工具箱用
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <returns></returns>
        public static Dictionary<string, float> FunSAV_ARVfromD20_ICP_ECP_For_Tool(string strD20, string strICP, string strECP)
        {
            Dictionary<string, float> returnList = new Dictionary<string, float>();

            if (strD20 == string.Empty || strICP == string.Empty || strECP == string.Empty)
                return returnList;

            float MCP = 0, ICP = 0, ECP = 0;

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {

                MCP = (ICP + ECP) / 2;
                returnList = FunSAV_ARVfromD20_MCP(strD20, MCP.ToString());
            }
            return returnList;
        }

        /// <summary>
        /// D20, A10, A30, A50,A70 ,A90 =>SAV,ARV
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <returns>字典键中输入SAV,ARV的到对应的值</returns>
        public static Dictionary<string, float> FunSAV_ARVfromD20_A10_A30_A50_A70_A90(string strD20, string strA10, string strA30, string strA50, string strA70, string strA90)
        {
            Dictionary<string, float> returnList = new Dictionary<string, float>();

            if (strD20 == string.Empty || strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty)
                return returnList;

            float A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, MCP = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50)
                && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                if (A10 >= 100 && A90 <= 280)
                {
                    MCP = (A10 + A30 + A50 + A70 + A90) / 5;
                    returnList = FunSAV_ARVfromD20_MCP(strD20, MCP.ToString());
                }
            }
            return returnList;
        }
        /// <summary>
        /// //D20,R20,MW,SUL=>CPP,CNN,CAA,RTT,RNN,RAA
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strR20"></param>
        /// <param name="strMW"></param>
        /// <param name="strSUL"></param>
        /// <returns>字典键中输入CPP,CNN,CAA,RTT,RNN,RAA的到对应的值</returns>
        public static  Dictionary<string, float> FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(string strD20, string strR20, string strMW, string strSUL)
        {
            Dictionary<string, float> returnList = new Dictionary<string, float>();

            //D20,R20,MW,SUL=>CPP,CNN,CAA,RTT,RNN,RAA
            if (strD20 == string.Empty || strR20 == string.Empty || strMW == string.Empty || strSUL == string.Empty)
                return returnList;

            float D20 = 0, R20 = 0, MW = 0, SUL = 0;
            float CPP, CNN, CAA, RTT, RNN, RAA;

            if (float.TryParse(strD20, out D20) && float.TryParse(strR20, out R20) && float.TryParse(strMW, out MW) && float.TryParse(strSUL, out SUL))
            {
                float x = (float)(2.51 * (R20 - 1.475) - D20 + 0.851);
                float y = (float)(D20 - 0.851 - 1.11 * (R20 - 1.475));

                CAA = x > 0 ? 430 * x + 3660 / MW : 670 * x + 3600 / MW;
                float CR = y > 0 ? 820 * y - 3 * SUL + 10000 / MW : 1400 * y - 3 * SUL + 10600 / MW;
                CNN = CR - CAA;
                CPP = 100 - CR;
                RAA = x >= 0 ? (float)(0.44 + 0.055 * MW * x) : (float)(0.44 + 0.08 * MW * x);
                RTT = y > 0 ? (float)(1.33 + 0.146 * MW * (y - 0.005 * SUL)) : (float)(1.33 + 0.18 * MW * (y - 0.005 * SUL));
                RNN = RTT - RAA;

                if (CPP >= 0 && CPP <= 100
                    && CNN >= 0 && CNN <= 100
                    && CAA >= 0 && CAA <= 100
                    && RTT >= 0 && RTT <= 100
                    && RNN >= 0 && RNN <= 100
                    && RAA >= 0 && RAA <= 100)
                {
                    returnList.Add("CPP", CPP);
                    returnList.Add("CNN", CNN);
                    returnList.Add("CAA", CAA);
                    returnList.Add("RTT", RTT);
                    returnList.Add("RNN", RNN);
                    returnList.Add("RAA", RAA);
                }  
            }

            return returnList;
        }
        /// <summary>
        /// D70,R70,MW,SUL=>CPP,CNN,CAA,RTT,RNN,RAA
        /// </summary>
        /// <param name="strD70"></param>
        /// <param name="strR70"></param>
        /// <param name="strMW"></param>
        /// <param name="strSUL"></param>
        /// <returns>字典键中输入CPP,CNN,CAA,RTT,RNN,RAA的到对应的值</returns>
        public static Dictionary<string, float> FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(string strD70, string strR70, string strMW, string strSUL)
        {
            Dictionary<string, float> returnList = new Dictionary<string, float>();

            //D70,R70,MW,SUL=>CPP,CNN,CAA,RTT,RNN,RAA
            if (strD70 == string.Empty || strR70 == string.Empty || strMW == string.Empty || strSUL == string.Empty)
                return returnList;

            float D70 = 0, R70 = 0, MW = 0, SUL = 0;
            float CPP, CNN, CAA, RTT, RNN, RAA;

            if (float.TryParse(strD70, out D70) && float.TryParse(strR70, out R70) && float.TryParse(strMW, out MW) && float.TryParse(strSUL, out SUL))
            {
                float x = (float)(2.42 * (R70 - 1.46) - D70 + 0.828);
                float y = (float)(D70 - 0.828 - 1.11 * (R70 - 1.46));

                CAA = x >= 0 ? 410 * x + 3660 / MW : 720 * x + 3600 / MW;
                float CR = y >= 0 ? 775 * y - 3 * SUL + 11500 / MW : 1400 * y - 3 * SUL + 12100 / MW;
                CNN = CR - CAA;
                CPP = 100 - CR;
                RAA = x >= 0 ? (float)(0.41 + 0.055 * MW * x) : (float)(0.41 + 0.08 * MW * x);
                RTT = y > 0 ? (float)(1.55 + 0.146 * MW * (y - 0.005 * SUL)) : (float)(1.55 + 0.18 * MW * (y - 0.005 * SUL));
                RNN = RTT - RAA;
                if (CPP >= 0 && CPP <= 100
                    && CNN >= 0 && CNN <= 100
                    && CAA >= 0 && CAA <= 100
                    && RTT >= 0 && RTT <= 100
                    && RNN >= 0 && RNN <= 100
                    && RAA >= 0 && RAA <= 100)
                {
                    returnList.Add("CPP", CPP);
                    returnList.Add("CNN", CNN);
                    returnList.Add("CAA", CAA);
                    returnList.Add("RTT", RTT);
                    returnList.Add("RNN", RNN);
                    returnList.Add("RAA", RAA);
                }              
            }
            return returnList;
        }

        /// <summary>
        /// D20,R70,MW,SUL=>CPP,CNN,CAA,RTT,RNN,RAA（工具箱用）
        /// </summary>
        /// <param name="strD70"></param>
        /// <param name="strR70"></param>
        /// <param name="strMW"></param>
        /// <param name="strSUL"></param>
        /// <returns>字典键中输入CPP,CNN,CAA,RTT,RNN,RAA的到对应的值</returns>
        public static Dictionary<string, float> FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(string strD20, string strR70, string strMW, string strSUL)
        {
            Dictionary<string, float> returnList = new Dictionary<string, float>();

            //D20,R70,MW,SUL=>CPP,CNN,CAA,RTT,RNN,RAA
            if (strD20 == string.Empty || strR70 == string.Empty || strMW == string.Empty || strSUL == string.Empty)
                return returnList;

            string strD70 = FunD70fromD20(strD20);

            returnList = FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(strD70,strR70, strMW, strSUL);
            
            return returnList;
        }

        /// <summary>
        ///  D20,MW,CAR,H2-->FFA,CII,TCC,RTT,RNN, CA,RAA
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strMW"></param>
        /// <param name="strCAR"></param>
        /// <param name="strH2"></param>
        /// <returns></returns>
        public static  Dictionary<string, float> FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(string strD20, string strMW, string strCAR, string strH2)
        { // D20,MW,CAR,H2-->FFA,CII,TCC,RTT,RNN, CA,RAA
            Dictionary<string, float> returnList = new Dictionary<string, float>();

            //D20,R20,MW,SUL=>CPP,CNN,CAA,RTT,RNN,RAA
            if (strD20 == string.Empty || strCAR == string.Empty || strMW == string.Empty || strH2 == string.Empty)
                return returnList;

            float D20 = 0, CAR = 0, MW = 0, H2 = 0;//输入条件
            float FFA, CII, TCC, RTT, RNN, CA, RAA;//输出条件

            if (float.TryParse(strD20, out D20) && float.TryParse(strCAR, out CAR) && float.TryParse(strMW, out MW) && float.TryParse(strH2, out H2))
            {
                float H2CAR = 11.92f * H2 / CAR;
                float MWCARD20 = 1201f / D20 / CAR;
                float MWCARD20CAR = MWCARD20 - 6 * (100 - CAR - H2) / CAR;

                FFA = 0.09f * MWCARD20CAR - 1.15f * H2CAR + 0.77f;
                CII = 2 - FFA - H2CAR;
                TCC = CAR * MW / 1201;
                RTT = TCC * CII / 2 + 1;
                CA = FFA * TCC;
                RAA = (CA - 2) / 4;
                RNN = RTT - RAA;

                returnList.Add("FFA", FFA);
                returnList.Add("CII", CII);
                returnList.Add("CA#", CA);
                returnList.Add("TCC", TCC);
                returnList.Add("RAA", RAA);
                returnList.Add("RNN", RNN);
                returnList.Add("RTT", RTT);

            }

            return returnList;
        }


        /// <summary>
        /// D20, ICP, ECP=>ANI
        ///IF 100<=ECP<=560。否则，退出算法。
        ///MCP=(ICP+ECP)/2
	    ///D20,MCP=>ANI
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <returns></returns>
        public static string FunANIfromD20_ICP_ECP(string strD20, string strICP, string strECP)
        {  
            string strRestult = string.Empty;

            if (strD20 == string.Empty || strICP == string.Empty || strECP == string.Empty)
                return strRestult;

            float MCP = 0,ICP = 0 ,ECP = 0;
            if (float.TryParse(strECP, out ECP) &&float.TryParse(strICP, out ICP))
            {
                if (ECP <= 560 && ICP >= 15)
                    MCP = (ICP + ECP) / 2;
            }

            strRestult = FunANIfromD20_MCP(strD20, MCP.ToString());

            return strRestult;
        }
        /// <summary>
        /// D20,MCP=>ANI
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strMCP"></param>
        /// <returns></returns>
        public static string FunANIfromD20_MCP(string strD20, string strMCP)
        {   //D20,MCP=>ANI
            //ANI= 0.163677 /10000*T^2.29383*D20^(-4.4013)
            string strRestult = string.Empty;

            if (strD20 == string.Empty || strMCP == string.Empty)
                return strRestult;

            float D20 = 0, MCP = 0, T = 0, ANI = 0;

            if (float.TryParse(strMCP, out MCP) && float.TryParse(strD20, out D20))
            {
                if (MCP > 15 && MCP <= 560)
                {
                    T = MCP + 273.15f;
                    ANI = (float)(0.163677 / 10000 * Math.Pow(T, 2.29383) * Math.Pow(D20, -4.4013));
                    strRestult = ANI.ToString();
                }
            }

            return strRestult;
        }
        /// <summary>
        /// D20,A10,A30,A50,A70,A90=>ANI
        /// </summary>
        /// <param name="strD20"></param>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <returns></returns>
        public static string FunANIfromD20_A10_A30_A50_A70_A90(string strA10, string strA30, string strA50, string strA70, string strA90,string strD20)
        {
            string strRestult = string.Empty;

            if (strD20 == string.Empty || strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty)
                return strRestult;

            float A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, MCP = 0;

            if (float.TryParse(strA10, out A10) && float.TryParse(strA30, out A30) && float.TryParse(strA50, out A50)
                && float.TryParse(strA70, out A70) && float.TryParse(strA90, out A90))
            {
                if (A10 >= 15 && A90 <= 560)
                {
                    MCP = (A10 + A30 + A50 + A70 + A90) / 5;
                    strRestult = FunANIfromD20_MCP(strD20, MCP.ToString());
                }
            }

            return strRestult;
        }
        /// <summary>
        /// 原油信息表的CLA的补充
        /// API<=10 ，则CLA=“特重原油”
        /// 10<API<=20, 则CLA=“重质原油”
        /// 20<API<=32, 则CLA=“中质原油”
        /// 32<API, 则CLA=“轻质原油”
        /// </summary>
        /// <param name="strAPI"></param>
        /// <returns></returns>
        public static string FunCLA(string strAPI)
        {
            string strResult = string.Empty;//返回值          

            if (strAPI == string.Empty)
                return strResult;

            float API = 0;

            if (float.TryParse(strAPI, out API))
            {
                if (API <= 10)
                    strResult = "特重原油";
                else if ((API > 10) && (API <= 20))
                    strResult = "重质原油";
                else if ((API > 20) && (API <= 32))
                    strResult = "中质原油";
                else if (API > 32)
                    strResult = "轻质原油";
            }

            return strResult;
        }
        /// <summary>
        /// A10->FPO
        ///FPO=1/(-0.024209+2.84947/(A10+273.15)+3.4254*0.001*LN((A10+273.15)))-273.15
        /// </summary>
        /// <param name="strAPI"></param>
        /// <param name="strANI"></param>
        /// <returns></returns>
        public static string FunFPO(string strA10)
        {
            float FPO = 0;

            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strA10 == string.Empty)
                return string.Empty;

            float A10 = 0;

            if (float.TryParse(strA10, out A10))
            {
                float temp = (float)Math.Log((A10 + 273.15));

                FPO = 1 / (float)(-0.024209 + 2.84947 / (A10 + 273.15) + 3.4254 * 0.001 * temp) - 273.15f;

                result = FPO.ToString();
            }

            return result;
        }



        /// <summary>
        /// 窄馏分表补充函数
        /// RVP＝Bi=7.641/(EXP(0.03402*MCP+0.6048))*100
        /// </summary>
        /// <param name="strMCP"></param>
        /// <returns></returns>
        public static string FunRVPfromMCP(string strMCP)
        {
            float RVP = 0;//结果

            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strMCP == string.Empty)
                return string.Empty;

            float MCP = 0;//条件

            if (float.TryParse(strMCP, out MCP))
            {
                RVP = (float)(7.641 / (Math.Exp(0.03402 * MCP + 0.6048)) * 100);
                if (!RVP.Equals(float.NaN))
                    result = RVP.ToString();
            }
            return result;
        }
        /// <summary>
        /// SG=D60/0.999006
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunSG(string strD60)
        {
            float SG = 0;//结果

            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strD60 == string.Empty)
                return result;

            float D60 = 0;//条件

            if (float.TryParse(strD60, out D60))
            {
                SG = (float)(D60 / DH2O60F);
                if (SG != 0)
                {
                    result = SG.ToString();
                }
            }

            return result;

        }
        /// <summary>
        /// D20=>D60=>SG
        /// </summary>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunSGfromD20(string strD20)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strD20 == string.Empty)
                return result;

            string strD60 = FunD60fromD20(strD20);

            result = FunSG(strD60);

            return result;
        }
        /// <summary>
        ///MW = 1.6607 * 0.0001 * T ^ 2.1962 * SG ^( -1.0164)
        /// </summary>
        /// <param name="strD60"></param>
        /// <returns></returns>
        public static string FunMW(string strSG, float T)
        {
            float MW = 0;//结果

            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strSG == string.Empty)
                return result;

            float SG = 0;//条件

            if (float.TryParse(strSG, out SG))
            {
                MW = (float)(1.6607 * 0.0001 * Math.Pow(T, 2.1962) * Math.Pow(SG, -1.0164));

                result = MW.ToString();
            }

            return result;
        }

 
        /// <summary>
        /// 馏程算法 
        /// 已知：B库中TVY曲线（TVY---ECP），馏分的ICP(0),ECP(0)，体积收率VY,KFC。
        /// </summary>
        /// <param name="Dic">ECP_TVY组成的词典</param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strKFC">KFC值</param>
        /// <returns>AIP,A10,A30,A50,A70,A90,A95,AEP七个数字组成的数据词典</returns>
        public static Dictionary<string, float?> FunAIP_A10_A30_A50_A70_A90_A95_AEPfromCurveEntityECP_TVYandICP_ECP_KFC(Dictionary<string, string> Dic, string strICP, string strECP, string strKFC)
        {
            Dictionary<string, float?> returnDic = new Dictionary<string, float?>();//声明返回对象    
            Dictionary<string, float?> tempResult = new Dictionary<string, float?>();

            #region "输入判断"

            if (Dic == null)
                return returnDic;
            if (Dic.Count <= 0)
                return returnDic;
            if (strICP == string.Empty || strECP == string.Empty)
                return returnDic;

            #endregion

            float ICP = 0, ECP = 0, KFC = 0;
            List<float> ECPList = new List<float>();
            List<float> TVYList = new List<float>();

            #region "数据转换"

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                if (ICP >= ECP)
                    return returnDic;
            }

            if (!float.TryParse(strKFC, out KFC))
            {
                return returnDic;
            }
            #endregion

            tempResult = FunAIP_A10_A30_A50_A70_A90_A95_AEPfromCurveEntityECP_TVYandICP_ECP_KFC_ForTool(Dic, strICP, strECP, true, false, true, false, strKFC);
            if (tempResult.Count != 0)
            {
                returnDic.Add("AIP", tempResult["AIP"]);
                returnDic.Add("A10", tempResult["A10"]);
                returnDic.Add("A30", tempResult["A30"]);
                returnDic.Add("A50", tempResult["A50"]);
                returnDic.Add("A70", tempResult["A70"]);
                returnDic.Add("A90", tempResult["A90"]);
                returnDic.Add("A95", tempResult["A95"]);
                returnDic.Add("AEP", tempResult["AEP"]);
            }

            #region 数据补充-之前的算法（针对ECP>400情况下用)
            //#region "输入判断"

            //if (Dic == null)
            //    return returnDic;
            //if (Dic.Count <= 0)
            //    return returnDic;
            //if (strICP == string.Empty || strECP == string.Empty || strKFC == string.Empty)
            //    return returnDic;

            //#endregion

            //#region "数据转换"

            //if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            //{
            //    if (ICP >= ECP)
            //        return returnDic;
            //}

            //if (!float.TryParse(strKFC, out KFC))
            //{
            //    return returnDic;
            //}

            //Dictionary<float, float> tempDIC = new Dictionary<float, float>();

            //foreach (string temp in Dic.Keys)
            //{
            //    float left = 0, right = 0;
            //    if (temp == string.Empty)//数据转换
            //        continue;

            //    if (Dic[temp] == string.Empty)//数据转换
            //        continue;

            //    if (float.TryParse(temp, out left) && float.TryParse(Dic[temp], out right))
            //    {
            //        if (!tempDIC.Keys.Contains(left))
            //        {
            //            tempDIC.Add(left, right);
            //        }
            //    }
            //}
            //tempDIC.OrderBy(o => o.Key);
            //ECPList = tempDIC.Keys.ToList();
            //TVYList = tempDIC.Values.ToList();
            //#endregion


            //int ArrayLength = TVYList.Count;//获取数组的长度
            //int XYZlength = 7;//设置数组的长度

            //List<float> InputX = new List<float> { ICP, ECP };

            //List<float> OutputY = SplineLineInterpolate.spline(ECPList, TVYList, InputX);

            //float VY = OutputY[1] - OutputY[0];

            ////step2
            //List<float> X = new List<float>();
            //#region "计算X的值"

            //X.Add(OutputY[0]);
            //X.Add(OutputY[0] + VY * 0.1f);
            //X.Add(OutputY[0] + VY * 0.3f);
            //X.Add(OutputY[0] + VY * 0.5f);
            //X.Add(OutputY[0] + VY * 0.7f);
            //X.Add(OutputY[0] + VY * 0.9f);
            //X.Add(OutputY[1]);

            //#endregion
            //List<float> Y = SplineLineInterpolate.spline(TVYList, ECPList, X);//到推出ECP列表

            //float[] Z = new float[XYZlength];

            //if (ICP <= 400)
            //{
            //    //step3              
            //    #region "计算Z的值"

            //    Z[1] = (float)Math.Pow((1 / 0.1403 * (Y[6] - Y[5])), (1 / 1.6606));
            //    Z[2] = (float)Math.Pow((1 / 2.6339 * (Y[5] - Y[4])), (1 / 0.755));
            //    Z[3] = (float)Math.Pow((1 / 2.2744 * (Y[4] - Y[3])), (1 / 0.82));
            //    Z[4] = (float)Math.Pow((1 / 2.6956 * (Y[3] - Y[2])), (1 / 0.8008));
            //    Z[5] = (float)Math.Pow((1 / 4.1481 * (Y[2] - Y[1])), (1 / 0.7164));
            //    Z[6] = (float)Math.Pow((1 / 5.8589 * (Y[1] - Y[0])), (1 / 0.6024));
            //    #endregion

            //    //step4
            //    #region
            //    X[3] = (float)(1 / 0.8851 * Math.Pow((Y[3] + 273.15 - 255.4), (1 / 1.0258)) + 255.4 - 273.15);
            //    X[0] = X[3] - Z[4] - Z[5] - Z[6];
            //    X[1] = X[3] - Z[4] - Z[5];
            //    X[2] = X[3] - Z[4];
            //    X[4] = X[3] + Z[3];
            //    X[5] = X[3] + Z[3] + Z[2];
            //    X[6] = X[3] + Z[3] + Z[2] + Z[1];
            //    X[0] = (float)(X[0] * 0.8 + ICP * 0.2);
            //    #endregion

            //    returnDic.Add("AIP", X[0]);
            //    returnDic.Add("A10", X[1]);
            //    returnDic.Add("A30", X[2]);
            //    returnDic.Add("A50", X[3]);
            //    returnDic.Add("A70", X[4]);
            //    returnDic.Add("A90", X[5]);
            //    returnDic.Add("A95", (float)((X[5] + X[6]) / 2.0));
            //    returnDic.Add("AEP", X[6]);
            //}
            //else if (ECP > 400)
            //{
            //    //step6             

            //    for (int i = 0; i < XYZlength; i++)//XYZlength = 7
            //    {
            //        double F = 0;
            //        if (KFC != 0)
            //            F = -3.2985 + 0.009 * (Y[i] + 273.15);
            //        else
            //            F = 0;
            //        Z[i] = (float)(Y[i] - 1.3889 * F * (KFC - 12) * Math.Log10(10.0 / 760.0));
            //        Z[i] = (float)((Z[i] + 273.15) * 0.683398 / (1 - 1.63434 / 10000 * (Z[i] + 273.15)));
            //    }


            //    double[] W = new double[XYZlength];

            //    #region

            //    W[3] = Z[3];
            //    W[4] = Z[4];
            //    W[5] = Z[5];
            //    W[6] = Z[6];
            //    W[2] = W[3] - 0.9 * (Z[3] - Z[2]);
            //    W[1] = W[2] - 0.9 * (Z[2] - Z[1]);
            //    W[0] = W[1] - 0.8 * (Z[1] - Z[0]);

            //    #endregion

            //    for (int i = 0; i < XYZlength; i++)//XYZlength = 7
            //    {
            //        Z[i] = (float)(W[i] / (W[i] * 1.63434 / 10000 + 0.683398));
            //        double F = 0;
            //        if (KFC != 0)
            //            F = -3.2985 + 0.009 * Z[i];
            //        else
            //            F = 0;
            //        Z[i] = (float)(Z[i] + 1.3889 * F * (KFC - 12) * Math.Log10(10.0 / 760.0) - 273.15);
            //    }

            //    returnDic.Add("AIP", Z[0]);
            //    returnDic.Add("A10", Z[1]);
            //    returnDic.Add("A30", Z[2]);
            //    returnDic.Add("A50", Z[3]);
            //    returnDic.Add("A70", Z[4]);
            //    returnDic.Add("A90", Z[5]);
            //    returnDic.Add("A95", (float)((Z[5] + Z[6]) / 2.0));
            //    returnDic.Add("AEP", Z[6]);
            //}
            #endregion
            return returnDic;
        }

        /// <summary>
        /// 馏程算法(工具箱用)
        /// </summary>
        /// <param name="Dic"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strKFC"></param>
        /// <returns></returns>
        public static Dictionary<string, float?> FunAIP_A10_A30_A50_A70_A90_A95_AEPfromCurveEntityECP_TVYandICP_ECP_KFC_ForTool(Dictionary<string, string> Dic, string strICP, string strECP, bool ECP05_IsNull, bool ECP95_IsNull, bool TVY05_IsNull, bool TVY95_IsNull, string strKFC)
        {
            Dictionary<string, float?> returnDic = new Dictionary<string, float?>();//声明返回对象          

            #region "输入判断"

            if (Dic == null)
                return returnDic;
            if (Dic.Count <= 0)
                return returnDic;
            if (strICP == string.Empty || strECP == string.Empty )
                return returnDic;

            #endregion

            float ICP = 0, ECP = 0, KFC = 0;
            List<float> ECPList = new List<float>();
            List<float> TVYList = new List<float>();

            #region "数据转换"

            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                if (ICP >= ECP)
                    return returnDic;
            }

            if (!float.TryParse(strKFC, out KFC))
            {
                return returnDic;
            }

            Dictionary<float, float> tempDIC = new Dictionary<float, float>();

            foreach (string temp in Dic.Keys)
            {
                float left = 0, right = 0;
                if (temp == string.Empty)//数据转换
                    continue;

                if (Dic[temp] == string.Empty)//数据转换
                    continue;

                if (float.TryParse(temp, out left) && float.TryParse(Dic[temp], out right))
                {
                    if (!tempDIC.Keys.Contains(left))
                    {
                        tempDIC.Add(left, right);
                    }
                }
            }
            tempDIC.OrderBy(o => o.Key);
            ECPList = tempDIC.Keys.ToList();
            TVYList = tempDIC.Values.ToList();
            #endregion

            int ArrayLength = TVYList.Count;//获取数组的长度

            float[] X1 = new float[ArrayLength];
            X1[0] = ICP;
            X1[ArrayLength - 1] = ECP;

            List<float> YList = SplineLineInterpolate.spline(ECPList, TVYList, X1.ToList());
            float VY = YList[YList.Count - 1] - YList[0];

            float[] X = new float[9];
            X[0] = YList[0];
            X[1] = X[0] + VY * 0.05f;
            X[2] = X[0] + VY * 0.1f;
            X[3] = X[0] + VY * 0.3f;
            X[4] = X[0] + VY * 0.5f;
            X[5] = X[0] + VY * 0.7f;
            X[6] = X[0] + VY * 0.9f;
            X[7] = X[0] + VY * 0.95f;
            X[8] = YList[YList.Count - 1];

            List<float> Y = SplineLineInterpolate.spline(TVYList, ECPList, X.ToList());//求出Y[9]      

            double?[] Z = new double?[8];
            if (ECP <= 400)
            {
                //step3              
                #region "计算Z的值"
                Z[0] = (float)Math.Pow((1 / 0.174 * (Y[8] - Y[6])), (1 / 1.6606));
                Z[1] = (float)Math.Pow((1 / 2.6339 * (Y[6] - Y[5])), (1 / 0.755));
                Z[2] = (float)Math.Pow((1 / 2.2744 * (Y[5] - Y[4])), (1 / 0.82));
                Z[3] = (float)Math.Pow((1 / 2.6956 * (Y[4] - Y[3])), (1 / 0.8008));
                Z[4] = (float)Math.Pow((1 / 4.1481 * (Y[3] - Y[2])), (1 / 0.7164));
                Z[5] = (float)Math.Pow((1 / 5.8589 * (Y[2] - Y[0])), (1 / 0.6024));
                Z[6] = (float)Math.Pow((1 / 5.8589 * (Y[2] - Y[1])), (1 / 0.6024));
                Z[7] = (float)Math.Pow((1 / 0.174 * (Y[7] - Y[6])), (1 / 1.6606));

                #endregion

                //step4
                #region 计算X
                X[4] = (float)(1 / 0.8851 * Math.Pow((Y[4] + 273.15 - 255.4), (1 / 1.0258)) + 255.4 - 273.15);
                X[0] = (float)(X[4] - Z[3] - Z[4] - Z[5]);
                X[1] = (float)(X[4] - Z[3] - Z[4] - Z[6]);
                X[2] = (float)(X[4] - Z[3] - Z[4]);
                X[3] = (float)(X[4] - Z[3]);
                X[5] = (float)(X[4] + Z[2]);
                X[6] = (float)(X[4] + Z[2] + Z[1]);
                X[7] = (float)(X[4] + Z[2] + Z[1] + Z[7]);
                X[8] = (float)(X[4] + Z[2] + Z[1] + Z[0]);
                #endregion

                returnDic.Add("AIP", X[0]);
                returnDic.Add("A05", X[1]);
                returnDic.Add("A10", X[2]);
                returnDic.Add("A30", X[3]);
                returnDic.Add("A50", X[4]);
                returnDic.Add("A70", X[5]);
                returnDic.Add("A90", X[6]);
                returnDic.Add("A95", X[7]);
                returnDic.Add("AEP", X[8]);
            }
            else if (ECP > 400)
            {
                double?[] result = new double?[9];
                //step6 
                if (ECPList.Count == 9)
                {
                    if (ECP05_IsNull && ECP95_IsNull)
                    {
                        result = FUN_TBP_2_D1160(Y, true, true, -1, -1, KFC);
                    }
                    else if (ECP05_IsNull && !ECP95_IsNull)
                    {
                        result = FUN_TBP_2_D1160(Y, true, false, -1, ECPList[6], KFC);
                    }
                    else if (!ECP05_IsNull && ECP95_IsNull)
                    {
                        result = FUN_TBP_2_D1160(Y, false, true, ECPList[1], -1, KFC);
                    }
                    else
                    {
                        result = FUN_TBP_2_D1160(Y, false, false, ECPList[1], ECPList[7], KFC);
                    }
                }
                else if (ECPList.Count < 9)
                {
                    result = FUN_TBP_2_D1160(Y, false, false, -1, -1, KFC);
                }
                else//数据补充用
                {
                    result = FUN_TBP_2_D1160(Y, false, false, -1, -1, KFC);
                }

                returnDic.Add("AIP", (float?)result[0]);
                returnDic.Add("A05", (float?)result[1]);
                returnDic.Add("A10", (float?)result[2]);
                returnDic.Add("A30", (float?)result[3]);
                returnDic.Add("A50", (float?)result[4]);
                returnDic.Add("A70", (float?)result[5]);
                returnDic.Add("A90", (float?)result[6]);
                returnDic.Add("A95", (float?)result[7]);
                returnDic.Add("AEP", (float?)result[8]);
            }
            else//如果ECP<400，退出
            {
                return returnDic;
            }
            return returnDic;
        }

        /// <summary>
        /// 工具箱馏程转换用（求解三次方程）
        /// </summary>
        /// <param name="k"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double?[] Fun_Quad_Equl(double?[] k, double? y, double?[] x)
        {
            double?[] result = new double?[3];//用于结果判断

            if (k.Count() != 4 || x.Count() != 3)
            {
                return result;
            }

            double? XX = 0 , YY = 0 , YYY = 0;
            k[3] = k[3] - y;
            for (int i = 0; i < 3; i++)
            {
                int count = 1;
                result[i] = x[i];
                do
                {

                    XX = result[i];
                    YY = k[0] * Math.Pow((double) XX, 3) + k[1] * Math.Pow((double) XX, 2) + k[2] * XX + k[3];
                    if (YY == 0)
                    {
                        return result;
                    }
                    YYY = 3 * k[0] * Math.Pow((double) XX, 2) + 2 * k[1] * XX + k[2];
                    if (YYY != 0)
                    {
                        result[i] = XX - YY / YYY;
                    }
                    count++;
                }
                while ((Math.Abs((double)(result[i] - XX)) > 0.00001) || count >= 100);
            }
            return result;
        }

        /// <summary>
        /// 工具箱-馏程算法中用ECP计算温度值用
        /// </summary>
        /// <param name="ECP"></param>
        /// <returns></returns>
        public static double?[] Fun_D86FromTBP(string[] ECP)
        {
            double?[] result = new double?[9];

            if (ECP.Count() != 9)
            {
                return result;
            }
            float ECP0 = 0, ECP1 = 0, ECP2 = 0, ECP3 = 0, ECP4 = 0, ECP5 = 0, ECP6 = 0, ECP7 = 0, ECP8 = 0;

            if (float.TryParse(ECP[0], out ECP0) && float.TryParse(ECP[1], out ECP1) && float.TryParse(ECP[2], out ECP2) && float.TryParse(ECP[3], out ECP3) && float.TryParse(ECP[4], out ECP4) && float.TryParse(ECP[5], out ECP5) && float.TryParse(ECP[6], out ECP6) && float.TryParse(ECP[7], out ECP7) && float.TryParse(ECP[8], out ECP8))
            {
                double[] Da = new double[6] { 0.174, 2.6339, 2.2744, 2.6956, 4.1481, 5.8589 };
                double[] Db = new double[6] { 1.6606, 0.755, 0.82, 0.8008, 0.7164, 0.6024 };

                float[] tempECP = new float[9] { (float)ECP0, (float)ECP1, (float)ECP2, (float)ECP3, (float)ECP4, (float)ECP5, (float)ECP6, (float)ECP7, (float)ECP8 };
                List<float> Y = tempECP.ToList();

                if (ECP8 <= 400)//AEP<=400
                {
                    result = FUN_TBP_2_D86(Da, Db, ECP0, ECP1, ECP2, ECP3, ECP4, ECP5, ECP6, ECP7, ECP8);
                }
                else//AEP>400
                {
                    result = FUN_TBP_2_D1160(Y, false, false, ECP1, ECP7, 0);
                }
            }
            return result;
        }



        /// <summary>
        /// 工具箱-馏程计算用
        /// </summary>
        /// <param name="Da"></param>
        /// <param name="Db"></param>
        /// <param name="ECP"></param>
        /// <returns></returns>
        private static double?[] FUN_TBP_2_D86(double[] Da, double[] Db, double ECP0,double ECP1,double ECP2,double ECP3,double ECP4,double ECP5,double ECP6,double ECP7,double ECP8)
        {
            double?[] result = new double?[9];

            result[4] = 255.4 + 1 / 0.8851 * Math.Pow((ECP4 - 255.4 + 273.15), 1 / 1.0258) - 273.15;

            double[] y = new double[8];
            y[0] = Math.Pow((1 / Da[0] * (ECP8 - ECP6)), 1 / Db[0]);
            y[1] = Math.Pow((1 / Da[1] * (ECP6 - ECP5)), 1 / Db[1]);
            y[2] = Math.Pow((1 / Da[2] * (ECP5 - ECP4)), 1 / Db[2]);
            y[3] = Math.Pow((1 / Da[3] * (ECP4 - ECP3)), 1 / Db[3]);
            y[4] = Math.Pow((1 / Da[4] * (ECP3 - ECP2)), 1 / Db[4]);
            y[5] = Math.Pow((1 / Da[5] * (ECP2 - ECP0)), 1 / Db[5]);

            result[0] = result[4] - y[3] - y[4] - y[5];
            result[2] = result[4] - y[3] - y[4];
            result[3] = result[4] - y[3];
            result[5] = result[4] + y[2];
            result[6] = result[4] + y[2] + y[1];
            result[8] = result[4] + y[2] + y[1] + y[0];
            if (ECP1 != -1)
            {
                y[6] = Math.Pow((1 / Da[5] * (ECP2 - ECP1)), 1 / Db[5]);
                result[1] = result[4] - y[3] - y[4] - y[6];
            }
            else
            {
                result[1] = null;
            }

            if (ECP7 != -1)
            {
                y[7] = Math.Pow((1 / Da[0] * (ECP8 - ECP7)), 1 / Db[0]);
                result[7] = result[4] + y[2] + y[1] + y[7];
            }
            else
            {
                result[7] = null;
            }

            return result;
        }

        /// <summary>
        /// TBP求D1160，馏程算法第六步
        /// </summary>
        /// <param name="Z"></param>
        /// <param name="Y"></param>
        /// <param name="KFC"></param>
        /// <returns></returns>
        private static double?[] FUN_TBP_2_D1160(List<float> Y, bool ECP05_IsNull, bool ECP95_IsNull, float ECP1, float ECP7, double? KFC)
        {
            double?[] result = new double?[9];

            double?[] Z = new double?[7];

            Y.RemoveAt(1);//移去05点
            Y.RemoveAt(6);//移去95点

            for (int i = 0; i < Y.Count; i++)
            {
                double? F = 0;
                if (KFC != 0)
                    F = -3.2985 + 0.009 * (Y[i] + 273.15);
                else
                    F = 0;
                Z[i] = (float)(Y[i] - 1.3889 * F * (KFC - 12) * Math.Log10(10.0 / 760.0) / Math.Log10(10));
                Z[i] = (float)((Z[i] + 273.15) * 0.683398 / (1 - 1.63434 / 10000 * (Z[i] + 273.15)));
            }

            double? DT = 0;
            double? min = 0;
            int xmin = 0;
            double?[] k = new double?[4];
            double?[] x = new double?[3];

            double? F0 = Z[3] - Z[2];
            DT = (F0 - 0.3) / 1.265;

            //解三次方程求DT
            min = DT * 10; xmin = -1;
            k[0] = 2.749 / 100000;
            k[1] = -5.539 / 1000;
            k[2] = 1.265;
            k[3] = 0.3;
            
            x[0] = DT;
            x[1] = 10 * DT;
            x[2] = DT / 10;

            x = Fun_Quad_Equl(k, F0, x);
            if (x[0] == null && x[1] == null && x[2] == null)
            {
                return result;
            }

            for (int i = 0; i < 3; i++)
            {
                if (x[i] > 0)
                {
                    if (Math.Abs( (double)( x[i] - DT)) < min)
                    {
                        min = Math.Abs((double)( x[i] - DT));
                        xmin = i;
                    }
                }
            }

            if (xmin > -1)
            {
                DT = x[xmin];
            }

            double? Dx0 = DT;
            Z[2] = Z[3] - (float)Dx0;
            double? F1 = Z[2] - Z[1];
            DT = (F1 - 0.3) / 1.265;
            min = DT * 10; xmin = -1;
            k[0] = 2.7486 / 100000; k[1] = -5.539 / 1000; k[2] = 1.2775; k[3] = 3.9;
            x[0] = DT; x[1] = 10 * DT; x[2] = DT / 10;
            x = Fun_Quad_Equl(k, F1, x);
            if (x[0] == null && x[1] == null && x[2] == null)
            {
                return result;
            }
            for (int i = 0; i < 3; i++)
            {
                if (x[i] > 0)
                {
                    if (Math.Abs((double)( x[i] - DT) )< min)
                    {
                        min = Math.Abs((double)( x[i] - DT));
                        xmin = i;
                    }
                }
            }
            if (xmin > -1)
            {
                DT = x[xmin];
            }

            double? Dx1 = DT;
            Z[1] = Z[2] - (float)Dx1;
            double? F2 = Z[1] - Z[0];

            DT = F2 / 2.26;
            min = DT * 10; xmin = -1;
            k[0] = 1.4093 / 10000; k[1] = -266.2 / 10000; k[2] = 2.205; k[3] = 9.01;
            x[0] = DT; x[1] = 10 * DT; x[2] = DT / 10;
            x = Fun_Quad_Equl(k, F2, x);
            if (x[0] == null && x[1] == null && x[2] == null)
            {
                return result;
            }
            for (int i = 0; i < 3; i++)
            {
                if (x[i] > 0)
                {
                    if (Math.Abs((double)( x[i] - DT)) < min)
                    {
                        min = Math.Abs((double)( x[i] - DT));
                        xmin = i;
                    }
                }
            }
            if (xmin > -1)
            {
                DT = x[xmin];
            }

            double? Dx2 = DT;
            Z[0] = Z[1] - (float)Dx2;
            for (int i = 0; i < Y.Count; i++)
            {
                double F = 0;
                Z[i] = (float)(Z[i] / (Z[i] * 1.63434 / 10000 + 0.683398));
                if (KFC != 0)
                {
                    F = (double)(-3.2985f + 0.009f * Z[i]);
                }
                else
                {
                    F = 0;
                }
                Z[i] = (float)(Z[i] + 1.3889 * F * (KFC - 12) * Math.Log10(10.0 / 760) - 273.15);
            }

            result[0] = Z[0];
            if (ECP05_IsNull && ECP1==-1)
            {
                result[1] = null;
            }
            else if (ECP1 != -1)
            {
                result[1] = ECP1;
            }
            result[2] = Z[1];
            result[3] = Z[2];
            result[4] = Z[3];
            result[5] = Z[4];
            result[6] = Z[5];
            if (ECP95_IsNull && ECP7==-1)
            {
                result[7] = null;
            }
            else if (ECP7 != -1)
            {
                result[7] = ECP7;
            }
            result[8] = Z[6];
            return result;
        }

        /// <summary>
        /// 反镏程算法(工具箱用)
        /// </summary>
        /// <param name="strAIP"></param>
        /// <param name="strA10"></param>
        /// <param name="strA30"></param>
        /// <param name="strA50"></param>
        /// <param name="strA70"></param>
        /// <param name="strA90"></param>
        /// <param name="strAEP"></param>
        /// <param name="strKFC"></param>
        /// <returns></returns>
        public static List<double?> FunfromAIP_A05_A10_A30_A50_A70_A90_AEP_KFC(string strAIP, string strA05, string strA10, string strA30, string strA50, string strA70, string strA90, string strA95,string strAEP, string strKFC)
        {
            List<double?> returnList = new List<double?>();//声明返回对象          

            #region "输入判断"    
      
            if (strAIP == string.Empty || strA05 == string.Empty || strA10 == string.Empty || strA30 == string.Empty || strA50 == string.Empty || strA70 == string.Empty || strA90 == string.Empty || strAEP == string.Empty || strKFC == string.Empty)
                return returnList;

            #endregion                

            #region "数据转换"

            List<double?> X = new List<double?>();

            double AIP = 0, A05 = 0, A10 = 0, A30 = 0, A50 = 0, A70 = 0, A90 = 0, A95 = 0, AEP = 0, KFC = 0;
            if (double.TryParse(strAIP, out AIP) && double.TryParse(strA05, out A05) && double.TryParse(strA10, out A10) && double.TryParse(strA30, out A30) && double.TryParse(strA50, out A50) && double.TryParse(strA70, out A70) && double.TryParse(strA90, out A90) && double.TryParse(strA95, out A95) && double.TryParse(strAEP, out AEP) && double.TryParse(strKFC, out KFC))               
            {
                X.Add(AIP);
                X.Add(A05);
                X.Add(A10);
                X.Add(A30);
                X.Add(A50);
                X.Add(A70);
                X.Add(A90);
                X.Add(A95);
                X.Add(AEP);
                if (AEP <= 400 && AEP != -1)//AEP<=400
                {
                    double?[] Y = new double?[9];
                    double?[] Z = new double?[9];
                    Z[0] = 0.174 * Math.Pow( (double)( X[8] - X[6]), 1.6606);
                    Z[1] = 2.6339 * Math.Pow((double)( X[6] - X[5]), 0.755);
                    Z[2] = 2.2744 * Math.Pow((double)( X[5] - X[4]), 0.82);
                    Z[3] = 2.6956 * Math.Pow((double)( X[4] - X[3]), 0.8008);
                    Z[4] = 4.1481 * Math.Pow((double)( X[3] - X[2]), 0.7164);
                    Z[5] = 5.8589 * Math.Pow((double)( X[2] - X[0]), 0.6024);

                    Y[4] = 255.4 + 0.8851 * Math.Pow((double)( (X[4] + 273.15 - 255.4)), 1.0258) - 273.15;
                    Y[0] = Y[4] - Z[3] - Z[4] - Z[5];
                    Y[2] = Y[4] - Z[3] - Z[4];
                    Y[3] = Y[4] - Z[3];
                    Y[5] = Y[4] + Z[2];
                    Y[6] = Y[4] + Z[2] + Z[1];
                    Y[8] = Y[4] + Z[2] + Z[1] + Z[0];

                    if (X[1] != -1)
                    {
                        Z[6] = 5.8589 * Math.Pow((double)((X[2] - X[1])), 0.6024);
                        Y[1]=Y[4]-Z[3]-Z[4]-Z[6];
                    }
                    else
                    {
                        Y[1] = null;
                    }

                    if (X[7] != -1)
                    {
                        Z[7] = 0.174 * Math.Pow((double)((X[7] - X[6])), 1.6606);
                        Y[7] = Y[4] - Z[3] - Z[4] - Z[7];
                    }
                    else 
                    {
                        Y[7] = null;
                    }

                    returnList = Y.ToList();
                }
                else//AEP>400
                {
                    #region ""

                    //X.RemoveAt(1);//移除5%点
                    //X.RemoveAt(6);//移除95%点

                    double?[] Y = new double?[9];
                    double?[] Z = new double?[9];

                    for (int i = 0; i < X.Count; i++)
                    {
                        if (X[i] == -1)
                        {
                            continue;
                        }
                        double? F = 0;
                        if (KFC != 0)
                        {
                            F = -3.2985 + 0.009 * (X[i] + 273.15);
                        }
                        else
                        {
                            F = 0;
                        }

                        //Z(i) = X(i) - 1.3889 * F * (KFC - 12) * Log(10 / 760) / Log(10)
                        //z(i) = (X(i) + 273.15) * 0.683398 / (1 - 1.63434 / 10000 * (X(i) + 273.15))

                        Z[i] = X[i] - 1.3889 * F * (KFC - 12) * Math.Log10(10.0 / 760)/Math.Log10(10);
                        Z[i] = (Z[i] + 273.15) * 0.683398 / (1 - 1.63434 / 10000 * (Z[i] + 273.15));
                    }

                    double? DX_0 = Z[4] - Z[3];
                    double? DX_1 = Z[3] - Z[2];
                    double? DX_2 = Z[2] - Z[0];

                    double? F_0 = 0.3 + 1.265 * DX_0 - 5.539 / 1000 * Math.Pow((double)DX_0, 2) + 2.749 / 100000 * Math.Pow((double) DX_0, 3);
                    double? F_1 = 0.3 + 1.265 * DX_1 - 5.539 / 1000 * Math.Pow((double) DX_1, 2) + 2.749 / 100000 * Math.Pow((double) DX_1, 3);
                    double? F_2 = 2.26 * DX_2 - 266.5 / 10000 * Math.Pow((double) DX_2, 2) + 1.37 / 10000 * Math.Pow((double) DX_2, 3);
                    Z[3] = Z[4] - F_0;
                    Z[2] = Z[3] - F_1;
                    Z[0] = Z[2] - F_2;


                    //For i = 0 To 6
                    //    Y(i) = Z(i) / (Z(i) * 1.63434 / 10000 + 0.683398)
                    //        IF KFC<>0 THEN
                    //    F = -3.2985 + 0.009 * Z(i)
                    //        ELSE 
                    //    F=0
                    //        ENDIF
                    //        Z(i) = Z(i) + 1.3889 * F * (KFC - 12) * Log(10 / 760) / Log(10) - 273.15
                    //Next i

                    for (int i = 0; i < X.Count; i++)
                    {
                        Z[i] = Z[i] / (Z[i] * 1.63434 / 10000 + 0.683398);
                        double? F = 0;
                        if (KFC != 0)
                        {
                            F = -3.2985 + 0.009 * Z[i];
                        }
                        else
                        {
                            F = 0;
                        }
                        Z[i] = Z[i] - 1.3889 * F * (KFC - 12) * Math.Log10(10.0 / 760)/Math.Log10(10) - 273.15;
                    }

                    Y[0] = Z[0];
                    if (A05 == -1)
                    {
                        Y[1] = null;
                    }
                    else
                    {
                        Y[1] = A05;
                    }
                    Y[2] = Z[2];
                    Y[3] = Z[3];
                    Y[4] = Z[4];
                    Y[5] = Z[5];
                    Y[6] = Z[6];
                    if (A95 == -1)
                    {
                        Y[7] = null;
                    }
                    else
                    {
                        Y[7] = A95;
                    }
                    Y[8] = Z[8];

                    returnList = Y.ToList();
                    #endregion 
                }
            }
            else
            {
                return returnList;
            }

            #endregion
            return returnList;
        }

        /// <summary>
        /// 根据V05和D20求KFC
        /// </summary>
        /// <param name="strV05"></param>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string FunKFCfromV05_D20(string strV05, string strD20)
        {
            string strResult = string.Empty;

            if (strV05 == string.Empty || strD20 == string.Empty )
                return strResult;

            double V05 = 0, D20 = 0,API=0;

            if (double.TryParse(strV05, out V05) && double.TryParse(strD20, out D20))
            {
                if (double.TryParse(FunAPIfromD20(strD20), out API))
                {
                    double lnV05 = Math.Log(V05);
                    double apik0 = FunAPIk(-0.0021, 0.0409, 0.3407, 3.4935, lnV05);
                    double apik1 = FunAPIk(-0.0024, 0.0408, 0.2889, 3.6997, lnV05);
                    double apik2 = FunAPIk(-0.0023, 0.0378, 0.2566, 3.8739, lnV05);
                    double apik3 = FunAPIk(-0.0021, 0.0342, 0.2315, 4.0129, lnV05);
                    double apik4 = FunAPIk(-0.0021, 0.0332, 0.2168, 4.137, lnV05);
                    double apik5 = FunAPIk(-0.002, 0.0307, 0.1967, 4.2246, lnV05);
                    double apik6 = FunAPIk(-0.002, 0.0326, 0.2074, 4.3349, lnV05);

                    double KFC = 0;

                    if (API < apik0)
                    {
                        //MessageBox.Show("K值小于9.5!", "信息提示");
                        strResult = "<9.5";
                    }
                    else if (API >= apik0 && API < apik1)
                    {
                        KFC = 9.5 + 0.5 / (apik1 - apik0) * (API - apik0);
                    }
                    else if (API >= apik1 && API < apik2)
                    {
                        KFC = 10 + 0.5 / (apik2 - apik1) * (API - apik1);
                    }
                    else if (API >= apik2 && API < apik3)
                    {
                        KFC = 10.5 + 0.5 / (apik3 - apik2) * (API - apik2);
                    }
                    else if (API >= apik3 && API < apik4)
                    {
                        KFC = 11 + 0.5 / (apik4 - apik3) * (API - apik3);
                    }
                    else if(API >= apik4 && API < apik5)
                    {
                        KFC = 11.5 + 0.5 / (apik5 - apik4) * (API - apik4);
                    }
                    else if (API >= apik5 && API < apik6)
                    {
                        KFC = 12 + 0.5 / (apik6 - apik5) * (API - apik5);
                    }
                    else
                    {
                        KFC = 12.5 + 0.5 / (apik6 - apik5) * (API - apik6);
                    }

                    //if (KFC > 13)
                    //{
                    //    //MessageBox.Show("计算K值超限，输入数据有疑问", "信息提示");
                    //    //strResult = ">13";
                    //}
                    //else

                    if (strResult != "<9.5")
                    {
                        strResult = KFC.ToString();
                    }
                }
            }
            return strResult;
        }

        /// <summary>
        /// 根据V05、D20求KFC专用函数
        /// </summary>
        /// <param name="douX1"></param>
        /// <param name="douX2"></param>
        /// <param name="douX3"></param>
        /// <param name="douX4"></param>
        /// <param name="lnV05"></param>
        /// <returns></returns>
        private static double FunAPIk(double douX1, double douX2, double douX3, double douX4,double lnV05)
        {
            double douResult = 0;
            douResult = -20 + Math.Exp(douX1 * Math.Pow(lnV05,3) +douX2 * Math.Pow(lnV05, 2 ) - douX3 * lnV05 + douX4);
            return douResult;
        }



        #endregion

        #region "GC关联补充"
        /// <summary>
        /// N06=G14+G15
        /// </summary>
        /// <param name="strG27"></param>
        /// <param name="strG15"></param>
        /// <returns></returns>
        public static string FunN06fromG14_G15(string strG14, string strG15)
        {
            string strResult = string.Empty;

            if (strG14 == string.Empty || strG15 == string.Empty)
                return strResult;

            float G14 = 0, G15 = 0;
            if (float.TryParse(strG14, out G14) && float.TryParse(strG15, out G15))
            {
                float N06 = G14 + G15;
                strResult = N06.ToString();
            }            
            return strResult;
        }


       /// <summary>
        /// N07=G27+G28+G29+G30+G31+G32+G34
       /// </summary>
       /// <param name="strG27"></param>
       /// <param name="strG28"></param>
       /// <param name="strG29"></param>
       /// <param name="strG30"></param>
       /// <param name="strG31"></param>
       /// <param name="strG32"></param>
       /// <param name="strG34"></param>
       /// <returns></returns>
        public static string FunN07fromG27_G28_G29_G30_G31_G32_G34(string strG27, string strG28, string strG29, string strG30, string strG31, string strG32, string strG34)
        {
            string strResult = string.Empty;

            if (strG27 == string.Empty || strG28 == string.Empty || strG29 == string.Empty 
                || strG30 == string.Empty || strG31 == string.Empty || strG32 == string.Empty || strG34 == string.Empty)
                return strResult;

            float G27 = 0, G28 = 0, G29 = 0, G30 = 0, G31 = 0, G32 = 0, G34 = 0;
            if (float.TryParse(strG27, out G27) && float.TryParse(strG28, out G28)
                && float.TryParse(strG29, out G29) && float.TryParse(strG30, out G30)
                && float.TryParse(strG31, out G31) && float.TryParse(strG32, out G32)
                && float.TryParse(strG34, out G34))            
            {
                float N07 = G27 + G28 + G29 + G30 + G31 + G32 + G34;
                strResult = N07.ToString();
            }
            return strResult;
        }

        /// <summary>
        /// N08=G38+G39
        /// </summary>
        /// <param name="strG38"></param>
        /// <param name="strG39"></param>
        /// <returns></returns>
        public static string FunN08fromG38_G39(string strG38, string strG39)
        {
            string strResult = string.Empty;

            if (strG38 == string.Empty || strG39 == string.Empty)
                return strResult;

            float G38 = 0, G39 = 0;
            if (float.TryParse(strG38, out G38) && float.TryParse(strG39, out G39))
            {
                float N08 = G38 + G39;
                strResult = N08.ToString();
            }
            return strResult;
        }
        /// <summary>
        /// A08= G40+G41+G42+G43
        /// </summary>
        /// <param name="strG40"></param>
        /// <param name="strG41"></param>
        /// <param name="strG42"></param>
        /// <param name="strG43"></param>
        /// <returns></returns>
        public static string FunA08fromG40_G41_G42_G43(string strG40, string strG41, string strG42, string strG43)
        {
            string strResult = string.Empty;

            if (strG40 == string.Empty || strG41 == string.Empty || strG42 == string.Empty || strG43 == string.Empty)              
                return strResult;

            float G40 = 0, G41 = 0, G42 = 0, G43 = 0;
            if (float.TryParse(strG40, out G40) && float.TryParse(strG41, out G41)
                && float.TryParse(strG42, out G42) && float.TryParse(strG43, out G43))
            {
                float A08 = G40 + G41 + G42 + G43;
                strResult = A08.ToString();
            }
            return strResult;
        }

        


        #endregion 


        #region "指数函数和反指数函数"

        /// <summary>
        /// VIS的指数函数INDEX(VIS)= 14.534*ln(ln(VIS+0.8))+10.975	
        /// </summary>
        /// <param name="strVIS"></param>
        /// <param name="high">默认大于0.8用两个嵌套</param>
        /// <returns></returns>
        public static string IndexFunVIS(string strVIS,bool high = true)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strVIS == string.Empty)
                return result;

            double VIS = 0;//条件

            if (high)
            {
                if (double.TryParse(strVIS, out VIS))
                {
                    double temp = Math.Log(VIS + 0.8);
                    double tempL = Math.Log(temp);
                    VIS = 14.534 * tempL + 10.975;
                    result = VIS.ToString();
                }
            }
            else
            {
                if (double.TryParse(strVIS, out VIS))
                {
                    double temp = Math.Log(VIS + 0.8);
                    //double tempL = Math.Log(temp);
                    VIS = 14.534 * temp + 10.975;
                    result = VIS.ToString();
                }
            }

            return result;
        }
        /// <summary>
        /// VIS的反指数函数VIS=EXP(EXP((INDEX-10.975)/14.534))-0.8
        /// </summary>
        /// <param name="strVIS"></param>
        /// <returns></returns>
        public static string InverseFunIndexVIS(string strVIS, bool high = true)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strVIS == string.Empty)
                return result;

            double VIS = 0;//条件
            if (high)
            {
                if (double.TryParse(strVIS, out VIS))
                {
                    VIS = Math.Exp(Math.Exp((VIS - 10.975) / 14.534)) - 0.8;
                    result = VIS.ToString();
                }
            }
            else
            {
                if (double.TryParse(strVIS, out VIS))
                {
                    VIS = Math.Exp((VIS - 10.975) / 14.534) - 0.8;
                    result = VIS.ToString();
                }
            }

            return result;
        }
        /// <summary>
        /// INDEX(FRZ)	=EXP(0.0654*FRZ+3.51)
        /// </summary>
        /// <param name="strPOR"></param>
        /// <returns></returns>
        public static string IndexFunFRZ(string strFRZ)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strFRZ == string.Empty)
                return result;

            double FRZ = 0;//条件

            if (double.TryParse(strFRZ, out FRZ))
            {
                FRZ = Math.Exp(0.065484 * FRZ + 3.514);
                result = FRZ.ToString();
            }

            return result;
        }
        /// <summary>
        /// FRZ=(LN(INDEX)-3.51)/0.00654
        /// </summary>
        /// <param name="strFRZ"></param>
        /// <returns></returns>
        public static string InverseFunIndexFRZ(string strFRZ)
        {
            string result = string.Empty;

            if (strFRZ == string.Empty)
                return result;

            double FRZ = 0;//条件

            if (double.TryParse(strFRZ, out FRZ))
            {
                FRZ = (Math.Log(FRZ) - 3.514) / 0.065484;
                result = FRZ.ToString();
            }

            return result;
        }
        /// <summary>
        /// INDEX(POR)= (POR+273.15)^(1/0.08) 
        /// </summary>
        /// <param name="strPOR"></param>
        /// <returns></returns>
        public static string IndexFunPOR(string strPOR)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strPOR == string.Empty)
                return result;

            double POR = 0;//条件

            if (double.TryParse(strPOR, out POR))
            {
                POR = Math.Pow(POR + 273.15, 1 / 0.8);
                result = POR.ToString();
            }

            return result;
        }
        /// <summary>
        /// POR=INDEX^(0.08)-273.15
        /// </summary>
        /// <param name="strPOR"></param>
        /// <returns></returns>
        public static string InverseFunIndexPOR(string strPOR)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strPOR == string.Empty)
                return result;

            double POR = 0;//条件

            if (double.TryParse(strPOR, out POR))
            {
                POR = Math.Pow(POR, 0.8) - 273.15;
                result = POR.ToString();
            }

            return result;
        }
        /// <summary>
        /// INDEX(SMK)=1000/SMK 
        /// </summary>
        /// <param name="strSMK"></param>
        /// <returns></returns>
        public static string IndexFunSMK(string strSMK)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strSMK == string.Empty)
                return result;

            double SMK = 0;//条件

            if (double.TryParse(strSMK, out SMK))
            {
                SMK = 1000 / SMK;
                result = SMK.ToString();
            }

            return result;
        }
        /// <summary>
        /// SMK=1000/INDEX
        /// </summary>
        /// <param name="strSMK"></param>
        /// <returns></returns>
        public static string InverseFunIndexSMK(string strSMK)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strSMK == string.Empty)
                return result;

            double SMK = 0;//条件

            if (double.TryParse(strSMK, out SMK))
            {
                SMK = 1000 / SMK;
                result = SMK.ToString();
            }

            return result;
        }
        /// <summary>
        ///  INDEX(ANI)=1.124*exp(0.00657*ANI) 
        /// </summary>
        /// <param name="strANI"></param>
        /// <returns></returns>
        public static string IndexFunANI(string strANI)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strANI == string.Empty)
                return result;

            double ANI = 0;//条件

            if (double.TryParse(strANI, out ANI))
            {
                ANI = 1.124 * Math.Exp(0.00657 * ANI);
                result = ANI.ToString();
            }

            return result;
        }
        /// <summary>
        /// ANI=LN(INDEX/1.123) /0.00657
        /// </summary>
        /// <param name="strANI"></param>
        /// <returns></returns>
        public static string InverseFunIndexANI(string strANI)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strANI == string.Empty)
                return result;

            double ANI = 0;//条件

            if (double.TryParse(strANI, out ANI))
            {
                ANI = Math.Log(ANI / 1.123) / 0.00657;
                result = ANI.ToString();
            }

            return result;
        }
        /// <summary>
        /// INDEX(RVP)=RVP^1.25 
        /// </summary>
        /// <param name="strRVP"></param>
        /// <returns></returns>
        public static string IndexFunRVP(string strRVP)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strRVP == string.Empty)
                return result;

            double RVP = 0;//条件

            if (double.TryParse(strRVP, out RVP))
            {
                RVP = Math.Pow(RVP, 1.25);
                result = RVP.ToString();
            }

            return result;
        }
        /// <summary>
        /// RVP=INDEX^0.8
        /// </summary>
        /// <param name="strRVP"></param>
        /// <returns></returns>
        public static string InverseFunIndexRVP(string strRVP)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strRVP == string.Empty)
                return result;

            double RVP = 0;//条件

            if (double.TryParse(strRVP, out RVP))
            {
                RVP = Math.Pow(RVP, 0.8);
                result = RVP.ToString();
            }

            return result;
        }
        /// <summary>
        /// INDEX(FPO)=10^(-6.1188+4345.2/(1.8*FPO+415))
        /// </summary>
        /// <param name="strFPO"></param>
        /// <returns></returns>
        public static string IndexFunFPO(string strFPO)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strFPO == string.Empty)
                return result;

            double FPO = 0;//条件

            if (double.TryParse(strFPO, out FPO))
            {
                FPO = Math.Pow(10, -6.1188 + 4345.2 / (1.8 * FPO + 415));
                result = FPO.ToString();
            }

            return result;
        }
        /// <summary>
        /// FPO=(4345.2/(Lg(INDEX)+6.1188) -415)/1.8
        /// </summary>
        /// <param name="strFPO"></param>
        /// <returns></returns>
        public static string InverseFunIndexFPO(string strFPO)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strFPO == string.Empty)
                return result;

            double FPO = 0;//条件

            if (double.TryParse(strFPO, out FPO))
            {
                FPO = (4345.2 / (Math.Log10(FPO) + 6.1188) - 415) / 1.8;
                result = FPO.ToString();
            }

            return result;
        }
        /// <summary>
        /// INDEX(D20)=1/D20
        /// </summary>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string IndexFunD20(string strD20)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strD20 == string.Empty)
                return result;

            double D20 = 0;//条件

            if (double.TryParse(strD20, out D20))
            {
                if (D20 != 0)
                    D20 = 1 / D20;
                else
                    D20 = 0;
                result = D20.ToString();
            }

            return result;
        }

        /// <summary>
        /// D20=1/INDEX
        /// </summary>
        /// <param name="strD20"></param>
        /// <returns></returns>
        public static string InverseFunIndexD20(string strD20)
        {
            string result = string.Empty;//如果没有经过计算则返回空值而不是0;

            if (strD20 == string.Empty)
                return result;

            double D20 = 0;//条件

            if (double.TryParse(strD20, out D20))
            {
                if (D20 != 0)
                    D20 = 1 / D20;
                else
                    D20 = 0;
                result = D20.ToString();
            }

            return result;
        }
        /// <summary>
        /// 指数函数在此更改;
        /// </summary>
        /// <param name="strValue">一般情况下是物性值</param>
        /// <param name="strItemCode">不同的物性</param>
        public static string IndexFunItemCode(string strValue, string strItemCode,bool high = true)
        {
            string strResult = string.Empty;

            if (strValue == string.Empty || strValue == "非数字" || strItemCode == string.Empty)
                return strResult;

            float temp = 0;

            if (float.TryParse(strValue, out temp))
            {
                switch (strItemCode)
                {
                    case "V02":
                        strResult = BaseFunction.IndexFunVIS(strValue, high);
                        break;
                    case "V04":
                        strResult = BaseFunction.IndexFunVIS(strValue, high);
                        break;
                    case "V05":
                        strResult = BaseFunction.IndexFunVIS(strValue, high);
                        break;
                    case "V08":
                        strResult = BaseFunction.IndexFunVIS(strValue, high);
                        break;
                    case "V10":
                        strResult = BaseFunction.IndexFunVIS(strValue, high);
                        break;
                    case "ANI":
                        strResult = BaseFunction.IndexFunANI(strValue);
                        break;
                    case "D20":
                        strResult = BaseFunction.IndexFunD20(strValue);
                        break;
                    case "FPO":
                        strResult = BaseFunction.IndexFunFPO(strValue);
                        break;
                    case "FRZ":
                        strResult = BaseFunction.IndexFunFRZ(strValue);
                        break;
                    case "POR":
                    case "SOP":
                        strResult = BaseFunction.IndexFunPOR(strValue);
                        break;
                    case "RVP":
                        strResult = BaseFunction.IndexFunRVP(strValue);
                        break;
                    case "SMK":
                        strResult = BaseFunction.IndexFunSMK(strValue);
                        break;
                    default:
                        strResult = strValue;
                        break;
                }
            }

            return strResult;
        }
        /// <summary>
        /// 反指数函数
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="strItemCode"></param>
        /// <returns></returns>
        public static string InverseIndexFunItemCode(string strValue, string strItemCode,bool high = true)
        {
            string strResult = string.Empty;

            if (strValue == string.Empty || strItemCode == string.Empty)
                return strResult;

            float temp = 0;

            if (float.TryParse(strValue, out temp))
            {
                switch (strItemCode)
                {
                    case "V02":
                        strResult = BaseFunction.InverseFunIndexVIS(strValue, high);
                        break;
                    case "V04":
                        strResult = BaseFunction.InverseFunIndexVIS(strValue, high);
                        break;
                    case "V05":
                        strResult = BaseFunction.InverseFunIndexVIS(strValue, high);
                        break;
                    case "V08":
                        strResult = BaseFunction.InverseFunIndexVIS(strValue, high);
                        break;
                    case "V10":
                        strResult = BaseFunction.InverseFunIndexVIS(strValue, high);
                        break;
                    case "ANI":
                        strResult = BaseFunction.InverseFunIndexANI(strValue);
                        break;
                    case "D20":
                        strResult = BaseFunction.InverseFunIndexD20(strValue);
                        break;
                    case "FPO":
                        strResult = BaseFunction.InverseFunIndexFPO(strValue);
                        break;
                    case "FRZ":
                        strResult = BaseFunction.InverseFunIndexFRZ(strValue);
                        break;
                    case "POR":
                    case "SOP":
                        strResult = BaseFunction.InverseFunIndexPOR(strValue);
                        break;
                    case "RVP":
                        strResult = BaseFunction.InverseFunIndexRVP(strValue);
                        break;
                    case "SMK":
                        strResult = BaseFunction.InverseFunIndexSMK(strValue);
                        break;
                    default:
                        strResult = strValue;
                        break;
                }
            }

            return strResult;
        }


        #endregion "指数函数和反指数函数"

        #region "插值函数"
        /// <summary>
        /// 数据插值函数
        /// </summary>
        /// <param name="ECPList"></param>
        /// <param name="oilDataList"></param>
        /// <param name="ECP"></param>
        /// <param name="oilData"></param>
        public static string SplineCal(List<OilDataEntity> XList, List<OilDataEntity> YList, string input)
        {
            string strResult = string.Empty;

            if (XList.Count <= 0 || YList.Count <= 0 || input == string.Empty)
                return strResult;

            Dictionary<float, float> X_YDic = new Dictionary<float, float>();        
            foreach (OilDataEntity x in XList)
            {             
                var y = YList.Where(o => o.OilTableCol.colCode == x.OilTableCol.colCode).FirstOrDefault();

                if (y != null)
                {
                    float tempx = 0, tempy = 0;
                    if (float.TryParse(x.calData, out tempx) && float.TryParse(y.calData, out tempy))
                    {
                        string stry = BaseFunction.IndexFunItemCode(y.calData, y.OilTableRow.itemCode);//物性指数函数
                        float dy = 0;
                        if (float.TryParse(stry, out dy))
                        {
                            if (!X_YDic.Keys.Contains(tempx))
                            {
                                X_YDic.Add(tempx, dy);
                            }
                        }                       
                    }
                }                
            }
            var tempX = from item in X_YDic
                        orderby item.Key
                        select item.Key;
            var tempY = from item in X_YDic
                        orderby item.Key
                        select item.Value;
            List<float> X = tempX.ToList();
            List<float> Y = tempY.ToList();

           
            float fInput = 0;

            if (float.TryParse(input, out fInput))
            {
                strResult = SplineLineInterpolate.spline(X, Y, fInput);//样条插值

                if (strResult == "非数字")
                    strResult = string.Empty;

                string str = InverseIndexFunItemCode(strResult, YList[0].OilTableRow.itemCode);
                strResult = str;

            }

            return strResult;
        }
        /// <summary>
        /// 数据插值函数
        /// </summary>
        /// <param name="XList"></param>
        /// <param name="YList"></param>
        /// <param name="input"></param>
        /// <returns>输入的input作为键值key</returns>
        public static Dictionary <string ,string> SplineCal(List<OilDataEntity> XList, List<OilDataEntity> YList,   List<string> input)
        {
            Dictionary<string, string> DicResult = new Dictionary<string, string>();//返回的结果，输入的input作为键值key

            if (XList.Count <= 0 || YList.Count <= 0 || input.Count <= 0)
                return DicResult;

            Dictionary<float, float> Dic = new Dictionary<float, float>();//找出X轴和Y轴对应的点
            foreach (OilDataEntity x in XList)
            {
                var y = YList.Where(o => o.OilTableCol.colCode == x.OilTableCol.colCode).FirstOrDefault();

                if (y != null)
                {
                    float tempx = 0, tempy = 0;
                    if (float.TryParse(x.calData, out tempx) && float.TryParse(y.calData, out tempy))
                    {
                        string stry = BaseFunction.IndexFunItemCode(y.calData, y.OilTableRow.itemCode);
                        float dy = 0;
                        if (float.TryParse(stry, out dy))
                        {
                            if (!Dic.Keys.Contains(tempx))
                            {
                                Dic.Add(tempx, dy);
                            }
                        }
                    }
                }
            }

            Dic.OrderBy(o => o.Key);//升序排序
            List<float> X = Dic.Keys.ToList();
            List<float> Y = Dic.Values.ToList();

            if (X.Count != Y.Count || X.Count < 5)
                return DicResult;           

            for (int i = 0; i < input.Count; i++)
            {
                float dInput = 0;
                if (float.TryParse(input[i], out dInput))
                {
                    if (dInput > X[X.Count - 1] || dInput < X[0])
                    {
                        if (DicResult.Keys.Contains(input[i]))
                            DicResult.Add(input[i], string .Empty);
                    }
                    else
                    {
                        string  strResult = SplineLineInterpolate.spline(X, Y, dInput);

                        if (strResult == "非数字")
                            strResult = string.Empty;

                        string str = InverseIndexFunItemCode(strResult, YList[0].OilTableRow.itemCode);
                        strResult = str;

                        if (DicResult.Keys.Contains(input[i]))
                            DicResult.Add(input[i], strResult);
                    }
                }
            }

            return DicResult;
        }
        
     
         
        #endregion 

        #region "切割检查函数"
        /// <summary>
        /// 切割方案检查
        /// </summary>
        /// <param name="cutMotheds"></param>
        /// <returns>正常返回true , 异常返回false</returns>
        public static bool checkCutMotheds(IList<CutMothedEntity> cutMotheds)
        {
            bool bResult = true;//默认返回条件

            #region"检查切割方案是否符合条件"
            List<string> NameList = new List<string>();
            for (int i = 0; i < cutMotheds.Count; i++)
            {
                if (cutMotheds[i].ICP >= cutMotheds[i].ECP)
                {
                    bResult = false;
                    return bResult;
                }

                if (!NameList.Contains(cutMotheds[i].Name))
                {
                    NameList.Add(cutMotheds[i].Name);
                }
                else
                {
                    bResult = false;
                    return bResult;
                }
            }
            #endregion

            return bResult;
        }
        /// <summary>
        /// 把切割方案转换成list列表用于样条插值函数的计算
        /// </summary>
        /// <param name="cutMotheds"></param>
        /// <returns></returns>
        public static List<float> getX_Input(IList<CutMothedEntity> cutMotheds)
        {
            List<float> X_Input = new List<float>();      //输入即将切割的横坐数组

            if (cutMotheds == null)
                return X_Input;

            for (int i = 0; i < cutMotheds.Count; i++)    //输入的横坐标值
            {
                X_Input.Add(cutMotheds[i].ICP);
                X_Input.Add(cutMotheds[i].ECP);
            }

            return X_Input;
        }
        /// <summary>
        /// 在进行样条插值计算前整理出X和Y输入列表
        /// </summary>
        /// <param name="curveDataEntityList"></param>
        /// <param name="strICP0"></param>
        /// <returns>key = X , value = Y</returns>
        public static void DicX_Y(out List<float> X, out List<float> Y, List<CurveDataEntity> curveDataEntityList, string strICP0)
        {
            List<float> temp = new List<float>();
            if (curveDataEntityList.Count <= 0)
            {
                X = temp;
                Y = temp;
                return;
            }
                
            #region "切割的输入参数"
            float startECP = curveDataEntityList[0].xValue;//TVY/TWY起始点温度

            SortedDictionary<float, float> DIC = new SortedDictionary<float, float>();
            if (startECP == 15 && strICP0 != string.Empty)
            {
                float ICP0 = 0;
                if (float.TryParse(strICP0, out ICP0))
                {
                    DIC.Add(ICP0, 0);
                    foreach (CurveDataEntity curveData in curveDataEntityList)
                    {
                        if (!DIC.Keys.Contains(curveData.xValue) && !curveData.xValue.Equals(float.NaN) && !curveData.yValue.Equals(float.NaN))
                            DIC.Add(curveData.xValue, curveData.yValue);
                    }
                }
                else
                {
                    foreach (CurveDataEntity curveData in curveDataEntityList)
                    {
                        if (!DIC.Keys.Contains(curveData.xValue) && !curveData.xValue.Equals(float.NaN) && !curveData.yValue.Equals(float.NaN))
                            DIC.Add(curveData.xValue, curveData.yValue);
                    }
                }
            }
            else
            {
                foreach (CurveDataEntity curveData in curveDataEntityList)
                {
                    if (!DIC.Keys.Contains(curveData.xValue) && !curveData.xValue.Equals(float.NaN) && !curveData.yValue.Equals(float.NaN))
                        DIC.Add(curveData.xValue, curveData.yValue);
                }
            }
            //X = DIC.Keys.ToList();
            //Y = DIC.Values.ToList();
            var D_Keys = from item in DIC
                         orderby item.Key
                         select item.Key;
            var D_Values = from item in DIC
                           orderby item.Key
                           select item.Value;

            X = D_Keys.ToList();//已知的X数组
            Y = D_Values.ToList();//已知的Y数组
            #endregion           
        }
        #endregion 

        /// <summary>
        /// 相似查找相似度计算公式
        /// </summary>
        /// <param name="tempfValue"></param>
        /// <param name="foundtionValue"></param>
        /// <param name="weight"></param>
        /// <param name="diff"></param>
        /// <returns></returns>
        public static double FunToolSimilarity(float tempfValue, float foundtionValue, float weight, float diff)
        {
            double result = 0;
             
            if (diff != 0)
            {
                result = weight * ((double)(1 - Math.Pow((Math.Abs(tempfValue - foundtionValue) / diff), 1.0 / 3)));
            }
            else 
            {
                result = 1;
            }
            return result;
        }
        /// <summary>
        /// 三元一次方程的计算
        /// </summary>
        /// <param name="X">三个点</param>
        /// <param name="Y">三个点</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static  float TOEquation(IList<float> X, IList<float> Y, float input)
        {
            float Yi = float.NaN;

            if (X.Count != Y.Count || X.Count != 3)
                return Yi;
            //X[0] = 1; X[1] = 2; X[2] = 3;
            //Y[0] = 6; Y[1] = 17; Y[2] = 34;
            float[,] arrayA = new float[3, 3] { { X[0] * X[0], X[0], 1 }, { X[1] * X[1], X[1], 1 }, { X[2] * X[2], X[2], 1 } };
            float[,] arrayY = new float[3, 1] { { Y[0] }, { Y[1] }, { Y[2] } };
            DenseMatrix A_Matrix = new DenseMatrix(arrayA);
            DenseMatrix Y_Matrix = new DenseMatrix(arrayY);
            var LU = A_Matrix.LU();
            DenseMatrix X_Matrix = (DenseMatrix)LU.Solve(Y_Matrix);// A_Matrix *X_Matrix =Y_Matrix
          
            Yi = X_Matrix[0, 0] * input * input + X_Matrix[1, 0] * input + X_Matrix[2, 0];

            return Yi;
        }

        /// <summary>
        /// 两个参数的指数函数求解
        /// </summary>
        /// <param name="X">已知的两个点x值</param>
        /// <param name="Y">已知的两个点y值</param>
        /// <param name="x">输入x</param>
        /// <returns>待求点y</returns>
        public static float FunIndexTwoPoints(List<float> X, List<float> Y, float x)
        {
            float y = float.NaN;

            if (X.Count != Y.Count || X.Count != 2 || X[0] == X[1] || X[0] == x || X[1] == x || Y[1] == 0)//如果不是两个点或者X与Y点数不同，或者输入的两个X相同，或者输入的Y中有0，不能计算
            {
                return y;
            }
            else
            {
                double temp = (X[0] - x) / (X[0] - X[1]);
                y = (float)((Math.Pow(Y[1], temp)) / Math.Pow(Y[0], temp - 1));
            }
            return y;
        }

        /// <summary>
        /// 指数函数的最小二乘拟合
        /// </summary>
        /// <param name="X">输入的X点</param>
        /// <param name="YY">输入的Y点</param>
        /// <param name="x">输入点x</param>
        /// <returns>待求点y</returns>
        public static float FunLeastSquaresOfIndex(Dictionary<float ,float> DIC ,  float input)
        {
            List<float> X = DIC.Keys.ToList();
            List<float> YY = DIC.Values.ToList(); 

            /* y=ae^(bx) 转变为 ln(y)=ln(a)+bx  转换为求线性的最小二乘 */
            float output = float.NaN;
            if (X.Count != YY.Count || input < 0 || X.Count <= 1)
                return output;
            if (DIC.Keys.Contains(input))
                return DIC[input];

            int n = X.Count;
            List<float> XX = new List<float>();
            List<float> XY = new List<float>();

            List<float> Y = new List<float>();
            foreach(float yy  in YY)
            {
                if (yy < 0)
                {
                    return output;
                }
                else
                {
                    Y.Add((float)Math.Log((double)yy)); //y=ln(y)
                }
            }

            foreach(float tempX in X)
            {
                XX.Add(tempX * tempX);
            }

            for (int i = 0; i < X.Count; i++)
            {
                XY.Add(X[i] * Y[i]);
            }

            double A = 0;//中间结果，求a
            double b = 0;

            float sumY = Y.Sum();//Y点加和
            float sumX = X.Sum();//X点加和
            float sumXX = XX.Sum();//X²加和
            float sumXY = XY.Sum();//XY乘积加和

            A = (sumY * sumXX - sumX * sumXY) / (n * sumXX - sumX * sumX);//A=ln(a) 为了转变为线性最小二乘
            b = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);

            double a = Math.Exp(A);//求出系数a

            output = (float)(a * Math.Exp(b * input));//求出y

            return output;
        }
        /// <summary>
        /// 指数函数的最小二乘拟合
        /// </summary>
        /// <param name="X">输入的X点</param>
        /// <param name="YY">输入的Y点</param>
        /// <param name="x">输入点集合x</param>
        /// <returns>待求点集合y</returns>
        public static List<float> FunLeastSquaresOfIndex(Dictionary<float, float> DIC, List<float> input)
        {
            /* y=ae^(bx) 转变为 ln(y)=ln(a)+bx  转换为求线性的最小二乘 */
            List<float> output = new List<float>();//输出


            List<float> X = DIC.Keys.ToList();
            List<float> YY = DIC.Values.ToList();
            
            if (X.Count != YY.Count || X.Count <= 1)
                return output;
          
            int n = X.Count;
            List<float> XX = new List<float>();
            List<float> XY = new List<float>();

            List<float> Y = new List<float>();
            foreach (float yy in YY)
            {
                if (yy < 0)
                {
                    return output;
                }
                else
                {
                    Y.Add((float)Math.Log((double)yy)); //y=ln(y)
                }
            }

            foreach (float tempX in X)
            {
                XX.Add(tempX * tempX);
            }

            for (int i = 0; i < X.Count; i++)
            {
                XY.Add(X[i] * Y[i]);
            }

            double A = 0;//中间结果，求a
            double b = 0;

            float sumY = Y.Sum();//Y点加和
            float sumX = X.Sum();//X点加和
            float sumXX = XX.Sum();//X²加和
            float sumXY = XY.Sum();//XY乘积加和

            A = (sumY * sumXX - sumX * sumXY) / (n * sumXX - sumX * sumX);//A=ln(a) 为了转变为线性最小二乘
            b = (n * sumXY - sumX * sumY) / (n * sumXX - sumX * sumX);

            double a = Math.Exp(A);//求出系数a
            foreach (float x in input)
            {
                //if (!DIC.Keys.Contains(x))
                //{
                //    float temp = (float)(a * Math.Exp(b * x));//求出y
                //    output.Add(temp);
                //}
                //else if (DIC.Keys.Contains(x))
                //{
                //    output.Add(DIC[x]);
                //}
                float temp = (float)(a * Math.Exp(b * x));//求出y
                output.Add(temp);
            }
          
            return output;
        }

 
        /// <summary>
        /// 一元二次方程的系数矩阵
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DenseMatrix TOEquationCoffMartix(IList<float> X, IList<float> Y)
        {
            if (X.Count != Y.Count || X.Count != 3)
                return null;

            float[,] arrayA = new float[3, 3] { { X[0] * X[0], X[0], 1 }, { X[1] * X[1], X[1], 1 }, { X[2] * X[2], X[2], 1 } };
            float[,] arrayY = new float[3, 1] { { Y[0] }, { Y[1] }, { Y[2] } };
            DenseMatrix A_Matrix = new DenseMatrix(arrayA);
            DenseMatrix Y_Matrix = new DenseMatrix(arrayY);
            var LU = A_Matrix.LU();
            DenseMatrix X_Matrix = (DenseMatrix)LU.Solve(Y_Matrix);// A_Matrix *X_Matrix =Y_Matrix

           // Yi = X_Matrix[0, 0] * input * input + X_Matrix[1, 0] * input + X_Matrix[2, 0];

            return X_Matrix;
        }

    }
}
