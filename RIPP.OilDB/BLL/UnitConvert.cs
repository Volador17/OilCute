using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.BLL
{
    public class Units
    {
        ///可能需要转换的单位
        public static List<string> UnitList = new List<string>()
        {
            "F","kg?m","kg?cm","kg?l","kg?ml","g?l","g?ml","g?cm","ppm","μg?g","ppb","mg?kg","μg?kg","ng?g","%","mg?g","mm2?s","mgKOH?100mL","mgKOH?g","psi","mpa","mg?L","mg?ml","gBr?100g","mgBr?100g","mg?100mL","MJ?kg","μg/g"
        };
        public static bool UnitConvert(ref double ConvertData,string PrimaryUnit,string TargetUnit)
        {
            if (PrimaryUnit.Replace('?', '/').Equals(TargetUnit))
            {
                return true;
            }

            if (TargetUnit.Equals("C"))
            {
                if (PrimaryUnit.Equals("F"))
                {
                    ConvertData = (ConvertData - 32) * 5.0 / 9;
                    return true;
                }
            }
            #region g/cm3
            if (TargetUnit.Equals("g/cm3"))
            {
                if (PrimaryUnit.Equals("kg?m"))
                {
                    ConvertData = ConvertData / 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("kg?cm"))
                {
                    ConvertData = ConvertData * 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("kg?l"))
                {
                    return true;
                }
                if (PrimaryUnit.Equals("kg?ml"))
                {
                    ConvertData = ConvertData * 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("g?l"))
                {
                    ConvertData = ConvertData / 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("g?ml"))
                {
                    return true;
                }
                if (PrimaryUnit.Equals("g?cm"))
                {
                    return true;
                }
            }
            #endregion

            #region %
            if (TargetUnit.Equals("%"))
            {
                if (PrimaryUnit.Equals("ppm"))
                {
                    ConvertData = ConvertData / 10000;
                    return true;
                }
                if (PrimaryUnit.Equals("μg?g"))
                {
                    ConvertData = ConvertData / 10000;
                    return true;
                }
                if (PrimaryUnit.Equals("μg/g"))
                {
                    ConvertData = ConvertData / 10000;
                    return true;
                }
                if (PrimaryUnit.Equals("ppb"))
                {
                    ConvertData = ConvertData / 1000000;
                    return true;
                }
                if (PrimaryUnit.Equals("mg?kg"))
                {
                    return true;
                }
                if (PrimaryUnit.Equals("μg?kg"))
                {
                    ConvertData = ConvertData / 1000000;
                    return true;
                }
                if (PrimaryUnit.Equals("ng?g"))
                {
                    ConvertData = ConvertData / 1000000;
                    return true;
                }
            }
            #endregion

            #region ug/g
            if (TargetUnit.Equals("μg/g"))
            {
                if (PrimaryUnit.Equals("μg?g"))
                {
                    return true;
                }
                if (PrimaryUnit.Equals("ppm"))
                {
                    return true;
                }
                if (PrimaryUnit.Equals("%"))
                {
                    ConvertData = ConvertData * 10000;
                    return true;
                }
                if (PrimaryUnit.Equals("mg?kg"))
                {
                    return true;
                }
                if (PrimaryUnit.Equals("mg?g"))
                {
                    ConvertData = ConvertData * 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("ppb"))
                {
                    ConvertData = ConvertData / 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("μg?kg"))
                {
                    ConvertData = ConvertData / 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("ng?g"))
                {
                    ConvertData = ConvertData / 1000;
                    return true;
                }
            }
            #endregion

            #region ng/g
            if (TargetUnit.Equals("ng/g"))
            {
                if (PrimaryUnit.Equals("μg?g"))
                {
                    ConvertData = ConvertData * 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("ppm"))
                {
                    ConvertData = ConvertData * 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("mg?kg"))
                {
                    ConvertData = ConvertData * 1000;
                    return true;
                }
                if (PrimaryUnit.Equals("mg?g"))
                {
                    ConvertData = ConvertData * 1000000;
                    return true;
                }
                if (PrimaryUnit.Equals("ppb"))
                {
                    return true;
                }
                if (PrimaryUnit.Equals("μg?kg"))
                {
                    return true;
                }
                if (PrimaryUnit.Equals("ng?g"))
                {
                    return true;
                }
            }
            #endregion

            if (TargetUnit.Equals("mm2/s"))
            {
                if (PrimaryUnit.Equals("mm2?s"))
                {
                    return true;
                }
            }

            if (TargetUnit.Equals("mgKOH/100mL"))
            {
                if (PrimaryUnit.Equals("mgKOH?100mL"))
                {
                    return true;
                }
            }

            if (TargetUnit.Equals("mgKOH/g"))
            {
                if (PrimaryUnit.Equals("mgKOH?g"))
                {
                    return true;
                }
            }

            if (TargetUnit.Equals("kPa"))
            {
                if (PrimaryUnit.Equals("psi"))
                {
                    ConvertData = ConvertData * 6895;
                    return true;
                }
                if (PrimaryUnit.Equals("mpa"))
                {
                    ConvertData = ConvertData * 1000;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String ToDBC(String input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }
    }
}
