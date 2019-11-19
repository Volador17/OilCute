using RIPP.Lib;
using RIPP.NIR;
using RIPP.NIR.Models;
using RIPP.Web.Chem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace RIPP.Web.Chem.Datas
{
    public partial class S_Group
    {
        private S_User _masterUser;
        /// <summary>
        /// 负责人
        /// </summary>
        public S_User MasterUser
        {
            get
            {
                if (this.MasterUserID > 0 && this._masterUser == null)
                {
                    using (var db = new RIPPWebEntities())
                    {
                        this._masterUser = db.S_User.Where(d => d.ID == this.MasterUserID).FirstOrDefault();
                    }
                }
                return this._masterUser;
            }
        }
    }


    public partial class S_User
    {
        private S_Group _group;

        /// <summary>
        /// 所在部门
        /// </summary>
        public S_Group Group
        {
            get
            {
                if (this.GroupID > 0 && this._group == null)
                {
                    using (var db = new RIPPWebEntities())
                    {
                        this._group = db.S_Group.Where(d => d.ID == this.GroupID).FirstOrDefault();
                    }
                }
                return this._group;
            }
        }


        private LoginStatu _statu;
        public LoginStatu statu
        {
            get
            {
                if (this._statu == null)
                {
                    this._statu = new LoginStatu()
                    {
                        islogin = true,
                        uid = ID,
                        mid = modelid.HasValue ? modelid.Value : 0,
                        mname = Model != null ? Model.name : ""
                    };
                }
                return this._statu;
            }
        }

        private model _model;
        public model Model
        {
            get
            {
                if (this.modelid.HasValue&&this.modelid.Value> 0 && this._model == null)
                {
                    using (var db = new RIPPWebEntities())
                    {
                        this._model = db.model.Where(d => d.id == this.modelid.Value).FirstOrDefault();
                    }
                }
                return this._model;
            }
        }

        public bool HasRole(RoleEnum role)
        {
            using (var db = new RIPPWebEntities())
            {
                if (this.Roles == null)
                    return false;
                else
                    return this.Roles.Contains(role);
            }
        }

        private List<RoleEnum> _Roles;
        public List<RoleEnum> Roles
        {
            get
            {
                using (var db = new RIPPWebEntities())
                {
                    this._Roles = null;
                    var lst = db.S_UserInRole.Where(i => i.UserID == this.ID).Select(i => i.RoleID);
                    if (lst.Count() > 0)
                        this._Roles = new List<RoleEnum>();
                    //系统管理员
                    foreach (var r in lst)
                        this._Roles.Add((RoleEnum)r);

                }
                return this._Roles == null ? new List<RoleEnum>() : this._Roles;
            }
        }

        public string RolesString
        {
            get
            {
                var lst = new List<string>();
                if (this.Roles != null && this.Roles.Count > 0)
                {
                    lst = this.Roles.Select(r => r.GetDescription()).ToList();
                }



                return string.Join(" - ", lst);
            }
        }
    }



    public partial class model
    {
        private S_User _addUser;
        /// <summary>
        /// 负责人
        /// </summary>
        public S_User AddUser
        {
            get
            {
                if (this.uid > 0 && this._addUser == null)
                {
                    using (var db = new RIPPWebEntities())
                    {
                        this._addUser = db.S_User.Where(d => d.ID == this.uid).FirstOrDefault();
                    }
                }
                return this._addUser;
            }
        }

        public ComponentList Components
        {
            get
            {
                //根据类型加载模型
                loadModel();
                return this._components;
            }
        }

        private BindModel _mBind;
        private PLSModel _mPLS;
        private IdentifyModel _mId;
        private FittingModel _mFitting;
        private PLSSubModel _mPLS1;
        private IntegrateModel _itgSub;
        private ComponentList _components;
        private void loadModel()
        {
            var ftype = (FileExtensionEnum)this.type;
            var tempfullName = Path.Combine(HttpContext.Current.Server.MapPath("~/"), this.path);
            switch (ftype)
            {
                case FileExtensionEnum.Allmethods:
                    if (this._mBind == null)
                    {
                        this._mBind = BindModel.ReadModel<BindModel>(tempfullName);
                        this._components = this._mBind.GetComponents();
                    }
                    break;
                case FileExtensionEnum.PLSBind:
                    if (this._mPLS == null)
                    {
                        this._mPLS = BindModel.ReadModel<PLSModel>(tempfullName);
                        this._components = this._mPLS.GetComponents();
                    }
                    break;
                case FileExtensionEnum.IdLib:
                    if (this._mId == null)
                    {
                        this._mId = BindModel.ReadModel<IdentifyModel>(tempfullName);
                        this._components = this._mId.GetComponents();
                    }
                    break;
                case FileExtensionEnum.FitLib:
                    if (this._mFitting == null)
                    {
                        this._mFitting = BindModel.ReadModel<FittingModel>(tempfullName);
                        this._components = this._mFitting.GetComponents();
                    }
                    break;
                case FileExtensionEnum.PLS1:
                case FileExtensionEnum.PLSANN:
                    if (this._mPLS1 == null)
                    {
                        this._mPLS1 = BindModel.ReadModel<PLSSubModel>(tempfullName);
                        this._components = this._mPLS1.GetComponents();
                    }
                    break;
                case FileExtensionEnum.ItgBind:
                    if (this._itgSub == null)
                    {
                        this._itgSub = BindModel.ReadModel<IntegrateModel>(tempfullName);
                        this._components = this._itgSub.GetComponents();
                    }
                    break;
                default:
                    break;
            }
        }
        public object Predict(spec s,out ComponentList c)
        {
            loadModel();
            c = null;
            var ftype = (FileExtensionEnum)this.type;
            var tempfullName = Path.Combine(HttpContext.Current.Server.MapPath("~/"), this.path);
            switch (ftype)
            {
                case FileExtensionEnum.Allmethods:
                    var r1 = this._mBind.Predict(s.Spec);
                    c = r1.GetPredictComp();
                    return r1;
                case FileExtensionEnum.PLSBind:
                    var r2= this._mPLS.Predict(s.Spec);
                    c = new ComponentList();
                   var clst= r2.Select(d => d.Comp);
                   foreach (var csub in clst)
                       c.Add(csub);
                   return r2;
                case FileExtensionEnum.IdLib:
                    var r3= this._mId.Predict(s.Spec);
                    c = r3.Components;
                    return r3;
                case FileExtensionEnum.FitLib:
                    var r4 = this._mFitting.Predict(s.Spec);
                    c = r4.FitSpec.Components;
                    return r4;
                case FileExtensionEnum.PLS1:
                case FileExtensionEnum.PLSANN:
                    var r5= this._mPLS1.Predict(s.Spec);
                    c = new ComponentList();
                    c.Add(r5.Comp);
                    return r5;
                case FileExtensionEnum.ItgBind:
                    var r6= this._itgSub.Predict(s.Spec);
                    c = new ComponentList();
                    foreach (var csub in r6)
                        c.Add(csub.Comp);
                    return r6;
                default:
                    break;
            }
            return null;
        }
    }


    public partial class spec
    {
        private Spectrum _spec;

        public Spectrum Spec
        {
            get
            {
                if (this._spec == null && this.contents != null)
                {
                    this._spec = Serialize.ByteToObject<Spectrum>(this.contents);
                }
                return this._spec;
            }
            set
            {
                this._spec = value;
                this.contents = Serialize.ObjectToByte(value);
            }
        }

        private S_User _addUser;
        /// <summary>
        /// 负责人
        /// </summary>
        public S_User AddUser
        {
            get
            {
                if (this.uid > 0 && this._addUser == null)
                {
                    using (var db = new RIPPWebEntities())
                    {
                        this._addUser = db.S_User.Where(d => d.ID == this.uid).FirstOrDefault();
                    }
                }
                return this._addUser;
            }
        }
    }


    public partial class results
    {
        private S_User _addUser;
        /// <summary>
        /// 负责人
        /// </summary>
        public S_User AddUser
        {
            get
            {
                if (this.uid > 0 && this._addUser == null)
                {
                    using (var db = new RIPPWebEntities())
                    {
                        this._addUser = db.S_User.Where(d => d.ID == this.uid).FirstOrDefault();
                    }
                }
                return this._addUser;
            }
        }

        private spec _spec;

        public spec Spec
        {
            get
            {
                if (this.sid > 0 && this._spec == null)
                {
                    using (var db = new RIPPWebEntities())
                    {
                        this._spec = db.spec.Where(d => d.id == this.sid).FirstOrDefault();
                    }
                }
                return this._spec;
            }
        }

        private model _model;
        public model Model
        {
            get
            {
                if (this.mid > 0 && this._model == null)
                {
                    using (var db = new RIPPWebEntities())
                    {
                        this._model = db.model.Where(d => d.id == this.mid).FirstOrDefault();
                    }
                }
                return this._model;
            }
        }


        public ComponentList Components
        {
            get
            {
                //根据类型加载模型
                if (this.componts == null)
                    return null;
                return Serialize.ByteToObject<ComponentList>(this.componts);
            }
        }
    }

}