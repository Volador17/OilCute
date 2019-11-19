using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Data
{
    public class MyComparable : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            double xDouble = double.Parse(x.Substring(0,x.IndexOf(",")).Trim());
            double yDouble = double.Parse(y.Substring(0,y.IndexOf(",")).Trim());
            if (xDouble > yDouble)
                return 1;
            else if (xDouble < yDouble)
                return -1;
            else
            return 0;
        }
    }
}
