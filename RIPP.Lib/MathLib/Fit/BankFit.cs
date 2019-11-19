using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using RIPP.Lib.MathLib;

namespace RIPP.Lib.MathLib.Fit
{
    public class BankFit
    {
        public static KeyValuePair<int, double>[] Fit(Matrix x, Vector y)
        {
            if (x == null || y == null)
                throw new ArgumentNullException("");
            if (x.RowCount != y.Count)
                throw new ArgumentException("");

            var spFit = SpectraFit.Fit(x, y);
            int[] idx;
            var sorted = spFit.SortDesc(out idx);
            var lst = new List<KeyValuePair<int, double>>();
            double c = 0;
            for (int i = 0; i < idx.Length; i++)
            {
                if (sorted[i] > 0)
                {
                    lst.Add(new KeyValuePair<int, double>(idx[i], sorted[i]));
                    c += sorted[i];
                }
                else
                    break;
            }
            if (lst.Count > 0)
            {
                var result = new KeyValuePair<int, double>[lst.Count];
                for (int i = 0; i < lst.Count; i++)
                {
                    result[i] = new KeyValuePair<int, double>(lst[i].Key, lst[i].Value / c);
                }
                return result;
            }
            return null;

        }
    }
}
