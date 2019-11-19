using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIPP.Web.Chem.Models
{
    public class LeftNavEntity
    {
        public LeftNavPanel Panel { set; get; }
        public string LiStr { set; get; }
    }

    public enum LeftNavPanel
    {
        Home,
        User,
        System,
        Group
    }
}