using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RIPP.Lib;
namespace RIPP.App.AnalCenter.Busi
{
    [Serializable]
    public class Config
    {
        private string _filepath = "config.db";
        private int _NumOfReuslt = 5;
        private int _NumOfId = 5;
        private int _MaxSpecNum = 10;
        private List<string> _AvailableModelNames = new List<string>();

        private string _folderSpec;
        private string _folderLIMS;
        private string _folderData;
        private string _folderModel;
        private int _topK = 1;

        public List<PropertyTable> Properties { set; get; }


        /// <summary>
        /// 光谱默认所在文件夹子
        /// </summary>
        public string FolderSpec
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._folderSpec) || !Directory.Exists(this._folderSpec))
                {
                    this._folderSpec = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Spectra");
                    if (!Directory.Exists(this._folderSpec))
                        Directory.CreateDirectory(this._folderSpec);
                }
                return this._folderSpec;
            }
            set { this._folderSpec = value; }
        }
        /// <summary>
        /// LIMS结果存放路径
        /// </summary>
        public string FolderLIMS
        {
             get
            {
                if (string.IsNullOrWhiteSpace(this._folderLIMS) || !Directory.Exists(this._folderLIMS))
                {
                    this._folderLIMS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LIMS");
                    if (!Directory.Exists(this._folderLIMS))
                        Directory.CreateDirectory(this._folderLIMS);
                }
                return this._folderLIMS;
            }
            set { this._folderLIMS = value; }
        }
        /// <summary>
        /// 导出所选数据路径
        /// </summary>
        public string FolderData
        {
            
             get
            {
                if (string.IsNullOrWhiteSpace(this._folderData) || !Directory.Exists(this._folderData))
                {
                    this._folderData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                    if (!Directory.Exists(this._folderData))
                        Directory.CreateDirectory(this._folderData);
                }
                return this._folderData;
            }
            set { this._folderData = value; }
        }

        /// <summary>
        /// 模型存放文件夹子
        /// </summary>
        public string FolderModel
        {
             get
            {
                if (string.IsNullOrWhiteSpace(this._folderModel) || !Directory.Exists(this._folderModel))
                {
                    this._folderModel = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Method");
                    if (!Directory.Exists(this._folderModel))
                        Directory.CreateDirectory(this._folderModel);
                }
                return this._folderModel;
            }
            set { this._folderModel = value; }
        }
        /// <summary>
        /// 默认方法包存放路径
        /// </summary>
        public string ModelDefaultPath
        {
            set;
            get;
        }
        /// <summary>
        /// 默认方法包在数据库中的ID
        /// </summary>
        public int ModelDefaultID
        {
            set;
            get;
        }
        /// <summary>
        /// 识别结果个数
        /// </summary>
        public int NumOfReuslt
        {
            set { this._NumOfReuslt = value; }
            get { return this._NumOfReuslt > 1 ? this._NumOfReuslt : 1; }
        }

        public int NumOfId
        {
            set { this._NumOfId = value; }
            get { return this._NumOfId > 1 ? this._NumOfId : 1; }
        }

        /// <summary>
        /// 新来的光谱先从这里去找值
        /// </summary>
        public int MaxSpecNum
        {
            set { this._MaxSpecNum = value; }
            get { return this._MaxSpecNum > 0? this._MaxSpecNum : 0; }
        }

        /// <summary>
        /// 是否自动执行
        /// </summary>
        public bool IsAutoEstimate { set; get; }


        public string LIMSDataDescription { set; get; }


        public int TopK
        {
            get { return this._topK > 1 ? this._topK : 1; }
            set { this._topK = value > 0 ? value : 1; }
        }
        /// <summary>
        /// 可用模型
        /// </summary>
        public List<string> AvailableModelNames { set { this._AvailableModelNames = value; } get { return this._AvailableModelNames??new List<string>(); } }


        public Config()
        {
            if (!File.Exists(_filepath))
            {
               // this._initProperty();
                this.Save();
            }
            else
            {
                var d = RIPP.Lib.Serialize.Read<Config>(this._filepath);
                this.AvailableModelNames = d.AvailableModelNames;
                this.FolderData = d.FolderData;
                this.FolderLIMS = d.FolderLIMS;
                this.FolderModel = d.FolderModel;
                this.FolderSpec = d.FolderSpec;
                this.IsAutoEstimate = d.IsAutoEstimate;
                this.LIMSDataDescription = d.LIMSDataDescription;
                this.MaxSpecNum = d.MaxSpecNum;
                this.ModelDefaultID = d.ModelDefaultID;
                this.ModelDefaultPath = d.ModelDefaultPath;
                this.NumOfId = d.NumOfId;
                this.NumOfReuslt = d.NumOfReuslt;
                this.Properties = d.Properties;
                this.TopK = d.TopK;


            }
        }

        public void Save()
        {
            if (this.Properties == null || this.Properties.Count == 0)
                this.Properties = OilEnity.Load();
            RIPP.Lib.Serialize.Write<Config>(this, this._filepath);
        }

    

    }


    [Serializable]
    public class CutSetting
    {
        private string _filepath = "point.db";

        public List<int[]> Point { set; get; }

        public CutSetting()
        {
            if (!File.Exists(_filepath))
            {
               // this._initProperty();
                this.Save();
            }
            else
            {
                var d = RIPP.Lib.Serialize.Read<CutSetting>(this._filepath);
                this.Point = d.Point;

            }

         }

        public void Save()
        {
            if (this.Point == null)
                this.Point = new List<int[]>();

            RIPP.Lib.Serialize.Write<CutSetting>(this, this._filepath);
        }

    }
    
}
