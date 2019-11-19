using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace RIPP.Web.Chem.Models
{
    public enum InfoPage
    {
        error = 0,

        highlight = 1
    }

    public class InfoModel
    {
        public InfoPage Icon { set; get; }

        public string Info { set; get; }

        public KeyValuePair<string, string>[] Urls { set; get; }

        public bool ParentReload { set; get; }
    }
}