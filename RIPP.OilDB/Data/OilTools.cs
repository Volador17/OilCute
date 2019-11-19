using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Data
{
    public class OilTools
    {
        /// <summary>
        /// 包括：单位转换
        /// </summary>
        public OilTools()
        {
        }

        /// <summary>
        /// 将插入的数据小数位数精简
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="decNumber">小数位数</param>
        /// <param name="valDigital">有效位数</param>
        /// <returns></returns>
        public string calDataDecLimit(string strValue, int? decNumber, int valDigital)
        {
            string str = string.Empty;//返回值；
            if (string.IsNullOrWhiteSpace(strValue))
                return str;

            if (strValue.Contains("E"))
                return strValue;

            if (strValue.Contains("e"))
                return strValue;
            float fValue;
            if (float.TryParse(strValue, out fValue)) //如果是浮点
            {
                if (fValue.Equals(float.NaN))
                    return str;

                if (decNumber == null)
                {
                    if (fValue == 0)
                        str = fValue.ToString();
                    else
                        str = getValDigital(strValue, valDigital);
                }
                else if (decNumber != null)
                {
                    if (fValue == 0)
                        str = fValue.ToString();
                    else
                        str = getDecNumberValue(strValue, decNumber.Value);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(strValue))
                    str = strValue;
                else
                {
                    if (strValue.Length > 20)//数据库中设置的OilData数据表中的数据格式最大是20个字符
                        str = strValue.Remove(20);
                    else
                        str = strValue;
                }
            }

            return str;
        }
        /// <summary>
        /// 有效数据
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="fValue"></param>
        /// <param name="valDigital"></param>
        /// <returns></returns>
        private string getValDigital(string strValue, int valDigital)//strValue为显示的小数位数，valDigital为实际的小数位数
        {
            string str = string.Empty;//返回值；
            if (string.IsNullOrWhiteSpace(strValue))
                return str;
            if (strValue.Contains("E"))
                return strValue;

            if (strValue.Contains("e"))
                return strValue;
            float fValue;
            if (float.TryParse(strValue, out fValue)) //如果是浮点
            {
                if (fValue > 0)
                {
                    #region "fValue > 0"
                    if (fValue < 1)
                    {
                        #region "fValue < 1"
                        int decNumOfStrValue = strValue.Length - strValue.IndexOf('.') - 1;
                        string partOfInteger = strValue.Substring(0, strValue.IndexOf("."));
                        string partOfDec = strValue.Substring(strValue.IndexOf(".") + 1);
                        string g = "G" + valDigital;
                        if (partOfDec.Length < valDigital)
                        {
                            string temp0 = string.Empty;
                            for (int i = 0; i < valDigital - partOfDec.Length; i++)
                                temp0 += "0";
                            str = partOfInteger + "." + partOfDec + temp0;
                        }
                        else
                        {
                            str = fValue.ToString(g);
                        }
                        #endregion
                    }
                    else //fValue>= 1
                    {
                        #region "fValue>= 1"
                        if (strValue.Contains("."))
                        {
                            int decNumOfStrValue = strValue.Length - strValue.IndexOf('.') - 1;
                            string partOfInteger = strValue.Substring(0, strValue.IndexOf("."));
                            string partOfDec = strValue.Substring(strValue.IndexOf(".") + 1);

                            if (!partOfDec.Contains("e") && !partOfDec.Contains("E"))
                            {
                                #region"小数"

                                if (partOfInteger.Length >= valDigital)
                                    str = partOfInteger;
                                else
                                {
                                    int decNum = valDigital - partOfInteger.Length;//需要从小数中获取数字补充数据。
                                    if (decNum <= partOfDec.Length)
                                    {
                                        string strtemp = "0." + partOfDec;
                                        decimal fDec = Convert.ToDecimal(strtemp);
                                        decimal fDecAfterRound = Math.Round(fDec, decNum);

                                        string strDecAfterRound = fDecAfterRound.ToString().Substring(fDecAfterRound.ToString().IndexOf("."));
                                        str = partOfInteger + strDecAfterRound;
                                    }
                                    else if (decNum > partOfDec.Length)
                                    {
                                        string temp0 = string.Empty;

                                        for (int i = 0; i < decNum - partOfDec.Length; i++)
                                            temp0 += "0";

                                        str = partOfInteger + "." + partOfDec + temp0;
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                str = strValue;
                            }
                        }
                        else//整数
                        {
                            #region "整数"
                            if (strValue.Length < valDigital)
                            {
                                string temp0 = string.Empty;

                                for (int i = 0; i < valDigital - strValue.Length; i++)
                                    temp0 += "0";

                                if (temp0 != string.Empty)
                                    str = strValue + "." + temp0;
                                else
                                    str = strValue;
                            }
                            else
                                str = strValue;
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region "fValue < 0"
                    if (fValue > -1)
                    {
                        #region "fValue > -1 && fValue < 0 "
                        int decNumOfStrValue = strValue.Length - strValue.IndexOf('.') - 1;
                        string partOfInteger = strValue.Substring(0, strValue.IndexOf("."));
                        string partOfDec = strValue.Substring(strValue.IndexOf(".") + 1);
                        string g = "G" + valDigital;
                        if (partOfDec.Length < valDigital)
                        {
                            string temp0 = string.Empty;
                            for (int i = 0; i < valDigital - partOfDec.Length; i++)
                                temp0 += "0";
                            str = partOfInteger + "." + partOfDec + temp0;
                        }
                        else
                        {
                            str = fValue.ToString(g);
                        }
                        #endregion
                    }
                    else
                    {
                        #region "fValue< -1"
                        if (strValue.Contains("."))
                        {
                            #region"小数"
                            int decNumOfStrValue = strValue.Length - strValue.IndexOf('.') - 1;
                            string partOfInteger = strValue.Substring(0, strValue.IndexOf("."));
                            string partOfDec = strValue.Substring(strValue.IndexOf(".") + 1);

                            if (partOfInteger.Length - 1 >= valDigital)
                                str = partOfInteger;
                            else
                            {
                                int decNum = valDigital - partOfInteger.Length + 1;//需要从小数中获取数字补充数据。

                                if (decNum <= partOfDec.Length)
                                {
                                    string strtemp = "0." + partOfDec;
                                    decimal fDec = Convert.ToDecimal(strtemp);
                                    decimal fDecAfterRound = Math.Round(fDec, decNum);

                                    string strDecAfterRound = fDecAfterRound.ToString().Substring(fDecAfterRound.ToString().IndexOf("."));
                                    str = partOfInteger + strDecAfterRound;
                                }
                                else if (decNum > partOfDec.Length)
                                {
                                    string temp0 = string.Empty;

                                    for (int i = 0; i < decNum - partOfDec.Length; i++)
                                        temp0 += "0";

                                    str = partOfInteger + "." + partOfDec + temp0;
                                }
                            }
                            #endregion
                        }
                        else//整数
                        {
                            #region "整数"
                            if (strValue.Length - 1 < valDigital)
                            {
                                string temp0 = string.Empty;

                                for (int i = 0; i < valDigital - strValue.Length + 1; i++)
                                    temp0 += "0";

                                str = strValue + "." + temp0;
                            }
                            else
                                str = strValue;
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            else
            {
                str = strValue;
            }

            return str;
        }
        /// <summary>
        /// 小数位数 
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="valDigital"></param>
        /// <returns></returns>
        private string getDecNumberValue(string strValue, int decNumber)
        {
            string str = string.Empty;//返回值；
            if (string.IsNullOrWhiteSpace(strValue))
                return str;
            float fValue;
            if (float.TryParse(strValue, out fValue)) //如果是浮点
            {
                string strTemp = Math.Round(fValue, decNumber).ToString();
                if (strTemp.Contains("."))//是一个小数
                {
                    int decNumOfStrValue = strTemp.Length - strTemp.IndexOf('.') - 1;
                    string partOfInteger = strTemp.Substring(0, strTemp.IndexOf("."));
                    string partOfDec = strTemp.Substring(strTemp.IndexOf(".") + 1);//小数部分长度

                    if (partOfDec.Length < decNumber)
                    {
                        string temp0 = string.Empty;
                        for (int i = 0; i < decNumber - partOfDec.Length; i++)
                            temp0 += "0";
                        str = partOfInteger + "." + partOfDec + temp0;
                    }
                    else
                    {
                        str = strTemp;
                    }
                }
                else//是一个整数
                {
                    string temp0 = string.Empty;

                    for (int i = 0; i < decNumber; i++)
                        temp0 += "0";

                    if (temp0 != string.Empty)
                        str = strTemp + "." + temp0;
                    else
                        str = strTemp;
                }
            }
            else
            {
                str = strValue;
            }

            return str;
        }
        /// <summary>
        /// 实测试值精度
        /// </summary>
        /// <param name="calData"></param>
        /// <param name="decNumber"></param>
        /// <returns></returns>
        public string calDataDecNumber(string calData, int decNumber, int valDigital)
        {
            if (string.IsNullOrWhiteSpace(calData))
                return string.Empty;

            float tempValue;
            if (float.TryParse(calData, out tempValue)) //如果是浮点
            {
                if (valDigital == 0) //如果没有有效数字就用小数位数
                    return Math.Round(tempValue, decNumber).ToString();
                else
                    return tempValue.ToString("G" + valDigital);

            }
            else
            {
                return calData;
            }
        }
        /// <summary>
        /// 将插入的数据小数位数精简
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decNumber"></param>
        /// <returns></returns>
        public string calDataDecLimit(string value, int decNumber)
        {
            string str = "";

            float tempValue;
            if (float.TryParse(value, out tempValue)) //如果是浮点
            {
                if (value.Contains("."))//是一个小数
                {
                    int dec = value.Length - value.IndexOf('.') - 1;

                    if (dec <= decNumber)
                    {
                        str = tempValue.ToString();
                    }
                    else
                    {
                        str = (Math.Round(tempValue, decNumber)).ToString();
                    }
                }
                else//是一个整数
                {
                    str = tempValue.ToString();
                }
            }
            else
            {
                str = value;
            }
            return str;
        }
        /// <summary>
        /// 单位转换ug/g=>%
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <returns>转化成功true不成功false</returns>
        public bool unitUgTo(ref string value)
        {
            bool result = false;
            float temp;

            if (float.TryParse(value, out temp))
            {
                value = (Math.Round(temp / 10000, 4)).ToString();
                result = true;
            }
            else
            {
                value = null;
            }

            return result;
        }
        /// <summary>
        /// 单位转换ug/g=>%
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="valDigital">转换后的小数位数</param>
        /// <returns>转化成功true不成功false</returns>
        public bool unitUgTo(ref string value, int valDigital)
        {
            bool result = false;
            float temp;

            if (float.TryParse(value, out temp))
            {
                value = (Math.Round(temp / 10000, valDigital)).ToString();
                result = true;
            }
            else
            {
                value = null;
            }

            return result;
        }

        #region
        ///// <summary>
        ///// 单位转换%=>ug/g
        ///// </summary>
        ///// <param name="value">要转化的值</param>
        ///// <returns>转化成功true不成功false</returns>
        //public bool unitToUg(ref string value)
        //{
        //    bool result = false;
        //    float temp;
        //    if (float.TryParse(value, out temp))
        //    {
        //        if (value.Contains("."))//是一个小数
        //        {
        //            int dec = value .Length - value.IndexOf('.') - 1;

        //            if (dec > 4)//小数位数太多需转换
        //            {
        //                value = (Math.Round(temp * 10000, 4)).ToString();        
        //            }

        //            result = true; //Math.Round(D20, decNumber).ToString();
        //        }
        //        else//是一个整数
        //        { 

        //        }             
        //    }
        //    else
        //    {
        //        value = null;
        //    }

        //    return result;
        //}
        #endregion

        /// <summary>
        /// 单位转换%=>ug/g
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <returns>转化成功true不成功false</returns>
        public bool unitToUg(ref string value)
        {
            bool result = false;
            float temp;
            if (float.TryParse(value, out temp))
            {
                if (value.Contains("."))//是一个小数
                {
                    int dec = value.Length - value.IndexOf('.') - 1;//小数点位数

                    if (dec <= 4)
                    {
                        value = (temp * 10000).ToString();
                    }
                    else
                    {
                        value = (Math.Round(temp * 10000, 4)).ToString();
                    }
                }
                else//是一个整数
                {
                    value = (temp * 10000).ToString();
                }

                result = true; //Math.Round(D20, decNumber).ToString();
            }
            else
            {
                value = null;
            }

            return result;
        }
        /// <summary>
        /// 单位转换%=>ug/g
        /// </summary>
        /// <param name="value">要转化的值</param>
        /// <param name="valDigital">转换后的小数位数</param>
        /// <returns>转化成功true不成功false</returns>
        public bool unitToUg(ref string value, int valDigital)
        {
            bool result = false;
            float temp;
            if (float.TryParse(value, out temp))
            {
                if (value.Contains("."))//是一个小数
                {
                    int dec = value.Length - value.IndexOf('.') - 1;//小数点位数

                    if (dec <= valDigital)
                    {
                        value = (temp * 10000).ToString();
                    }
                    else
                    {
                        value = (Math.Round(temp * 10000, valDigital)).ToString();
                    }
                }
                else//是一个整数
                {
                    value = (temp * 10000).ToString();
                }

                result = true; //Math.Round(D20, decNumber).ToString();
            }
            else
            {
                value = null;
            }

            return result;
        }

        /// <summary>
        ///  Cru导入,华氏温度转化为摄氏温度值
        /// </summary>
        /// <param name="value">华氏温度值</param>
        /// <param name="decNumber">精度</param>
        /// <returns>成功：摄氏温度值；不成功：""</returns>
        public string fToC(string value, int decNumber)
        {
            string result = "";
            double temp;
            if (double.TryParse(value, out temp))
            {
                result = Math.Round((5.0 / 9.0 * (temp - 32)), decNumber).ToString();
            }
            return result;
        }

        /// <summary>
        ///  Cru导入,RVP化为KPA
        /// </summary>
        /// <param name="value">RVP值</param>
        /// <returns>成功：KPA值；不成功：""</returns>
        public string toKpa(string value, int decNumber)
        {
            string result = "";
            double temp;
            if (double.TryParse(value, out temp))
            {
                result = Math.Round((temp / 6.895), decNumber).ToString();
            }
            return result;
        }

        /// <summary>
        ///  Cru导入,API计算出D20 (D60=141.5/(API+131.5)*DH2O60F,D20=-0.00546*D60^2+1.013143*D60-0.010325)
        /// </summary>
        /// <param name="value">API值</param>
        /// <returns>D20值或空</returns>
        public string apiToD20(string value, int decNumber)
        {
            string result = "";
            double DH2O60F = 0.99;
            double temp;
            if (double.TryParse(value, out temp))
            {
                double D60 = 141.5 / (temp + 131.5) * DH2O60F;
                double D20 = D20 = -0.00546 * D60 * D60 + 1.013143 * D60 - 0.010325;
                result = Math.Round(D20, decNumber).ToString();
            }
            return result;
        }

        /// <summary>
        ///  Cru导入单位转换，根据目的单位对值进行转换
        /// </summary>
        /// <param name="desUnit">目的单位</param>
        /// <param name="value">要转换的值</param>
        /// <returns>转换结果值或空</returns>
        public string cruYConvert(string desUnit, string value, int decNumber)
        {
            string result = "";
            if (desUnit.ToUpper() == "KPA")
            {
                result = toKpa(value, decNumber);
            }
            else if (desUnit.ToUpper() == "℃")
            {
                result = fToC(value, decNumber);
            }

            return result;
        }

        /// <summary>
        /// Cru导入时GAS转化到标准GV表，=Y(GV01)*D20_GC(i)*Y(GV00)/CRU_D20/100  ,=Y(GV01)*Y(GV00)/100
        /// </summary>
        /// <param name="gv">gv值</param>
        /// <param name="gv00">gv00值</param>
        /// <param name="cruD20"></param>
        /// <param name="D20_GCI">D20值</param>
        /// <returns>GV值</returns>
        public string GASToGV(string gv, string gv00, string cruD20, float D20_GCI, int decNumber)
        {
            double tempGV, tempGV00, tempCruD20;

            if (D20_GCI < 0) //GV00不用计算
                return gv;
            string result = "";
            if (double.TryParse(gv, out tempGV) && double.TryParse(gv00, out tempGV00) && double.TryParse(cruD20, out tempCruD20))
            {
                result = Math.Round((tempGV * D20_GCI * tempGV00 / tempCruD20 / 100), decNumber).ToString();
            }
            return result;
        }

        /// <summary>
        /// Cru导入时GAS转化到标准GV表，=Y(GV01)*Y(GV00)/100
        /// </summary>
        /// <param name="gv">gv值</param>
        /// <param name="gv00">gv00值</param>
        /// <param name="cruD20"></param>
        /// <param name="D20_GCI">D20值</param>
        /// <returns>GV值</returns>
        public string GASToGV(string gv, string gv00, int decNumber)
        {
            double tempGV, tempGV00;
            string result = "";
            if (double.TryParse(gv, out tempGV) && double.TryParse(gv00, out tempGV00))
            {
                result = Math.Round((tempGV * tempGV00 / 100), decNumber).ToString();
            }
            return result;
        }

    }
}
