//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace RIPP.Web.Chem.Datas
{
    using System;
    using System.Collections.Generic;
    
    public partial class S_User
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public string Pwd { get; set; }
        public string RealName { get; set; }
        public bool SexIsMan { get; set; }
        public string Email { get; set; }
        public string PhoneCell { get; set; }
        public string PhoneOffice { get; set; }
        public string Address { get; set; }
        public Nullable<int> QQ { get; set; }
        public bool ApplyState { get; set; }
        public string Remark { get; set; }
        public bool Deleted { get; set; }
        public System.DateTime LastLoginTime { get; set; }
        public System.DateTime AddTime { get; set; }
        public Nullable<int> modelid { get; set; }
        public string watchFolder { get; set; }
    }
}
