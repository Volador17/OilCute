using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using RIPP.Lib;
using RIPP.NIR;
using RIPP.NIR.Models;

namespace RIPP.App.AnalCenter.Busi
{
    public enum RoleEnum
    {
        /// <summary>
        /// RIPP用户
        /// </summary>
        [Description("RIPP用户")]
        RIPP = -1,

        /// <summary>
        /// 操作员
        /// </summary>
        [Description("分析操作员")]
        Operater = 0,

        /// <summary>
        /// 用户工程师
        /// </summary>
        [Description("用户工程师")]
        Engineer = 1
    }
    /// <summary>
    /// 数据库
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// 基本性质
        /// </summary>
        [Description("基本性质")]
        NIR = 0,

        /// <summary>
        /// 石脑油馏分
        /// </summary>
        [Description("石脑油")]
        ShiNao = 1,

        /// <summary>
        /// 喷气燃料
        /// </summary>
        [Description("喷气燃料")]
        PenQi = 2,

        /// <summary>
        /// 柴油馏分
        /// </summary>
        [Description("柴油")]
        ChaiYou = 3,

        /// <summary>
        /// 蜡油馏分
        /// </summary>
        [Description("蜡油")]
        LaYou = 4,

        /// <summary>
        /// 渣油馏分
        /// </summary>
        [Description("渣油")]
        ZhaYou = 5

    }


    public partial class S_User
    {
        public RoleEnum Role
        {
            get
            {
                return (RoleEnum)this._roleID;
            }
            set
            {
                this._roleID = (int)value;
            }
        }
    }
    
    public partial class Specs
    {
        private Spectrum _spec;

        public Spectrum Spec
        {
            get
            {
                if (this._spec == null&& this._contents!=null)
                {
                    this._spec = Serialize.ByteToObject<Spectrum>(this._contents);
                }
                return this._spec;
            }
            set
            {
                this._spec = value;
            }
        }

        private List<PropertyTable> _oildata;
        public List<PropertyTable> OilData
        {
            get
            {
                if (this._oildata == null)
                {
                    using (var db = new NIRCeneterEntities())
                    {
                        var item = db.OilData.Where(d => d.SID == this._iD).FirstOrDefault();
                        if (item != null)
                            this._oildata = Serialize.ByteToObject<List<PropertyTable>>(item.Data);
                        if (this._oildata == null)
                        {


                        }
                    }
                }
                return _oildata;
            }
            set
            {
                this._oildata = value;
            }
        }

        private BindResult _resultobj;

        public BindResult ResultObj
        {
            get
            {
                if (this._resultobj == null && this._result!=null)
                {
                    this._resultobj = Serialize.ByteToObject<BindResult>(this._result);
                }
                return this._resultobj;
            }
            set
            {
                this._resultobj = value;
            }
        }


        private S_User _user;
        public S_User User
        {
            get
            {
                if (this._user == null)
                {
                    using (var db = new NIRCeneterEntities())
                    {
                        this._user = db.S_User.Where(d => d.ID == this._userID).FirstOrDefault();
                    }
                }
                return _user;

            }
        }
        /// <summary>
        /// 是预测方法预测出结果
        /// </summary>
        public bool PredictByA { set; get; }
    }

    public partial class Properties
    {
        public PropertyType TableType
        {
            set { this._tableID = (int)value; }
            get { return (PropertyType)this._tableID; }
        }
    }
}
