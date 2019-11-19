using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RIPP.App.Chem.Roles;

namespace RIPP.App.Chem.Busi
{
    [Serializable]
    public class Config
    {

        
        /// <summary>
        /// 保存路径
        /// </summary>
        private string _filepath = "ChemConfig.db";
        private List<UserEntity> _users = new List<UserEntity>();

        #region 默认参数
        private int _plsMaxFactor = 15;
        private int _IdWinNum = 11;
        private double _IdTQ = 0.9;
        private double _IdSQ = 0.9;
        private int _FitWinNum = 11;
        private double _FitTQ = 0.9;
        private double _FitSQ = 0.9;
        private int _numOfId = 5;


        private string _folderSpectrum;
        private string _folderSpecLib;
        private string _folderSpecTemp;
        private string _folderMModel;
        private string _folderMModelLib;


        private string _folderMId;
        private string _folderMFit;
        private string _folderMInteg;
        private string _folderMMethod;

        private string _folderParameter;
        private string _folderBlendLib;
        private string _folderBlendMod;





        /// <summary>
        /// PLS最大主因子个数
        /// </summary>
        public int PLSMaxFoctor
        {
            set { this._plsMaxFactor = value; }
            get { return this._plsMaxFactor; }
        }

        public int IdWinNum
        {
            set { this._IdWinNum = value; }
            get { return this._IdWinNum; }
        }

        public double IdTQ
        {
            set { this._IdTQ = value; }
            get { return this._IdTQ; }
        }

        public double IdSQ
        {
            get { return this._IdSQ; }
            set { this._IdSQ = value; }
        }

        public int FitWinNum
        {
            set { this._FitWinNum = value; }
            get { return this._FitWinNum; }
        }

        public double FitTQ
        {
            set { this._FitTQ = value; }
            get { return this._FitTQ; }
        }

        public double FitSQ
        {
            get { return this._FitSQ; }
            set { this._FitSQ = value; }

        }

        public int IdNumOfId
        {
            set { this._numOfId = value; }
            get { return this._numOfId; }
        }

        public string DefaultDirectory
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory; }
        }


        /// <summary>
        /// 打开光谱默认文件夹：原油快评软件\Spectra
        /// </summary>
        public string FolderSpectrum
        {
            set { this._folderSpectrum = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderSpectrum) || !Directory.Exists(this._folderSpectrum))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Spectra");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderSpectrum;
            }
        }
        /// <summary>
        /// 光谱库默认文件夹：原油快评软件\Spectra Lib
        /// </summary>
        public string FolderSpecLib
        {
            set { this._folderSpecLib = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderSpecLib) || !Directory.Exists(this._folderSpecLib))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Spectra Lib");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderSpecLib;
            }
        }
        /// <summary>
        /// 模板默认文件夹：原油快评软件\ Lib temp
        /// </summary>
        public string FolderSpecTemp
        {
            set { this._folderSpecTemp = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderSpecTemp) || !Directory.Exists(this._folderSpecTemp))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Lib temp");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderSpecTemp;
            }
        }
        /// <summary>
        /// 模型库默认文件夹：原油快评软件\Model bin
        /// </summary>
        public string FolderMModel
        {
            set { this._folderMModel = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderMModel) || !Directory.Exists(this._folderMModel))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Model bin");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderMModel;
            }
        }

        public string FolderMModelLib
        {
            set { this._folderMModelLib = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderMModelLib) || !Directory.Exists(this._folderMModelLib))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Model lib");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderMModelLib;
            }
        }
        
        /// <summary>
        /// 识别库默认文件夹：原油快评软件\ Ident Lib
        /// </summary>
        public string FolderMId
        {
            set { this._folderMId = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderMId) || !Directory.Exists(this._folderMId))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Ident Lib");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderMId;
            }
        }
        /// <summary>
        /// 拟合库默认文件夹：原油快评软件\ Fit Lib
        /// </summary>
        public string FolderMFit
        {
            set { this._folderMFit = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderMFit) || !Directory.Exists(this._folderMFit))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Fit Lib");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderMFit;
            }
        }
        /// <summary>
        /// 集成方法包保存默认文件夹：原油快评软件\ Integ bin
        /// </summary>
        public string FolderMInteg
        {
            set { this._folderMInteg = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderMInteg) || !Directory.Exists(this._folderMInteg))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Integ Lib");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderMInteg;
            }
        }
        /// <summary>
        /// 方法打包保存默认文件夹：原油快评软件\ Method bin
        /// </summary>
        public string FolderMMethod
        {
            set { this._folderMMethod = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderMMethod) || !Directory.Exists(this._folderMMethod))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Method bin");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderMMethod;
            }
        }

        /// <summary>
        /// 预处理方法默认文件夹：原油快评软件\Parameter
        /// </summary>
        public string FolderParameter
        {
            set { this._folderParameter = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderParameter) || !Directory.Exists(this._folderParameter))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Parameter");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderParameter;
            }
        }
        /// <summary>
        /// 保存光谱库：原油快评软件\Blend Lib
        /// </summary>
        public string FolderBlendLib
        {
            set { this._folderBlendLib = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderBlendLib) || !Directory.Exists(this._folderBlendLib))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Blend Lib");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderBlendLib;
            }
        }
        /// <summary>
        /// 捆绑模型：默认文件夹：原油快评软件\Blend Mod
        /// </summary>
        public string FolderBlendMod
        {
            set { this._folderBlendMod = value; }
            get
            {
                if (string.IsNullOrEmpty(this._folderBlendMod) || !Directory.Exists(this._folderBlendMod))
                {
                    string p = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Blend Mod");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    return p;
                }
                return this._folderBlendMod;
            }
        }

        


       

        #endregion




        /// <summary>
        /// 用户列表
        /// </summary>
        public List<UserEntity> Users
        {
            set { this._users = value; }
            get { return this._users; }
        }

        /// <summary>
        /// 权限
        /// </summary>
        public List<RoleDetail> Roles { set; get; }

        public Config()
        {
            if (!File.Exists(_filepath))
            {
                this.Save();
            }
            else
            {
                var d = RIPP.Lib.Serialize.Read<Config>(this._filepath);
                this.FitSQ = d.FitSQ;
                this.FitTQ = d.FitTQ;
                this.FitWinNum = d.FitWinNum;
                this.IdSQ = d.IdSQ;
                this.IdTQ = d.IdTQ;
                this.IdWinNum = d.IdWinNum;
                this.PLSMaxFoctor = d.PLSMaxFoctor;
                this.Roles = d.Roles;
                this.Users = d.Users;

                this.FolderBlendLib = d.FolderBlendLib;
                this.FolderBlendMod = d.FolderBlendMod;
                this.FolderMFit = d.FolderMFit;
                this.FolderMId = d.FolderMId;

                this.FolderMInteg = d.FolderMInteg;
                this.FolderMMethod = d.FolderMMethod;
                this.FolderMModel = d.FolderMModel;
                this.FolderMModelLib = d.FolderMModelLib;
                this.FolderParameter = d.FolderParameter;

                this.FolderSpecLib = d.FolderSpecLib;
                this.FolderSpecTemp = d.FolderSpecTemp;
                this.FolderSpectrum = d.FolderSpectrum;

            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void Save()
        {
            if (this.Roles == null || this.Roles.Count == 0)
                this.Roles = RoleDetail.Load();
            RIPP.Lib.Serialize.Write<Config>(this, this._filepath);
            Common.Reload();
        }

    }
}
