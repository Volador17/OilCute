using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{

    public class AccountAudit
    { 
    
    }

    public class AccountAuditEntity
    {       
        private EnumTableType _tableType = EnumTableType.None;
        private string _FunName = string.Empty;
        private string _strICP = string.Empty;
        private string _strECP = string.Empty;
        private string _Text = string.Empty;

        public AccountAuditEntity()
        { 
        
        }
        public AccountAuditEntity(string text ,string strICP ,string strECP)
        {
            this._Text = text;
            this._strICP = strICP;
            this._strECP = strECP;
        }
        /// <summary>
        /// 
        /// </summary>
        public EnumTableType tableType
        {
            get { return this._tableType; }
            set { this._tableType = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FunName
        {
            get { return this._FunName; }
            set { this._FunName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return this._Text; }
            set { this._Text = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string strICP
        {
            get { return this._strICP; }
            set { this._strICP = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public string strECP
        {
            get { return this._strECP; }
            set { this._strECP = value; }
        }
        
    }
}
