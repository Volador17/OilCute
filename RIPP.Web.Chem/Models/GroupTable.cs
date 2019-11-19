using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIPP.Web.Chem.Models
{
    public class GroupTable
    {
        public string Name { set; get; }

        public int ID { set; get; }

        public string Remark{set;get;}

        public string MasterName { set; get; }

        public int MasterUserID { set; get; }

    }
}