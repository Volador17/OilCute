using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace RIPP.App.AnalCenter.Busi
{

    public class OilEnity
    {
        private static string fileName = "config.xml";
        public static List<PropertyTable> Load()
        {
            string fullname = "";
            if (!File.Exists(fullname))
            {
                Save(init());
            }
            XmlSerializer xs = new XmlSerializer(typeof(List<PropertyTable>));
            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var lst = (List<PropertyTable>)xs.Deserialize(fs);
            fs.Close();
            return lst;
        }

        public static void Save(List<PropertyTable> lst)
        {
            if (lst == null)
                return;
            XmlSerializer xs = new XmlSerializer(typeof(List<PropertyTable>));
            var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            xs.Serialize(fs, lst);
            fs.Close();
        }

        
        private static List<PropertyTable> init()
        {
            using (var db = new NIRCeneterEntities())
            {
                var gt = db.Properties.GroupBy(d => d.TableID);
                var tb = new List<PropertyTable>();
                foreach (var g in gt)
                {
                    

                    var tbl = (PropertyType)g.Key;
                    double bs, be;
                    switch (tbl)
                    {
                        case PropertyType.ChaiYou:
                            bs=180;
                            be=350;
                            break;
                        case PropertyType.LaYou:
                            bs=350;
                            be=540;
                            break;
                        case PropertyType.PenQi:
                            bs=140;
                            be=240;
                            break;
                        case PropertyType.ShiNao:
                            bs=15;
                            be=180;
                            break;
                        case PropertyType.ZhaYou:
                            bs=540;
                            be=1600;
                            break;
                        default:
                            bs=15;
                            be=180;
                            break;
                    }

                    tb.Add(new PropertyTable()
                    {
                        BoilingEnd = be,
                        BoilingStart = bs,
                        Datas = g.Select(d => new PropertyEntity() { Code = d.Code, ColumnIdx = d.ColumnIdx, Index = d.Idx, Name = d.Name, Name2 = d.Name1, Unit = d.Units, Eps = d.Eps, ShowEngineer = true, ShowRIPP = true }).ToList(),
                        Table = (PropertyType)g.Key
                    });
                }
                return tb;
            }
        }
    }


    
    [Serializable]
    public class PropertyTable
    {
        public OilInfo OilInfoDetail { set; get; }
        
        /// <summary>
        /// 沸点起点
        /// </summary>
        public double BoilingStart { set; get; }

        /// <summary>
        /// 沸点终点
        /// </summary>
        public double BoilingEnd { set; get; }

        /// <summary>
        /// 表名称
        /// </summary>
        public PropertyType Table { set; get; }

        /// <summary>
        /// 表下的数据
        /// </summary>
        public List<PropertyEntity> Datas { set; get; }
    }


    [Serializable]
    public class PropertyEntity
    {
        private string _name2;
        private bool _showRIPP = true;
        private bool _showEng = true;
        private string _code;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 第二列名称
        /// </summary>
        public string Name2
        {
            set { this._name2 = value; }
            get { return string.IsNullOrWhiteSpace(this._name2) ? this.Name : this._name2; }
        }
        /// <summary>
        /// 列编号
        /// </summary>
        public int ColumnIdx { set; get; }

        public string Code
        {
            set { this._code = value; }
            get
            {
                var dd = this._code ?? "";
                return dd.ToUpper();
            }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int Index { set; get; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { set; get; }

        //精度
        public int Eps { set; get; }

        /// <summary>
        /// 值
        /// </summary>
        public double Value { set; get; }

        /// <summary>
        /// 值信度
        /// </summary>
        public double Confidence { set; get; }


        /// <summary>
        /// RIPP是否显示
        /// </summary>
        public bool ShowRIPP
        {
            set { this._showRIPP = value; }
            get { return this._showRIPP; }
        }

        public bool ShowEngineer
        {
            set { this._showEng = value; }
            get { return this._showEng; }
        }
    }
    [Serializable]
    public class OilInfo
    {
        public string CrudeIndex { set; get; }

        public string CrudeName { set; get; }
    }
}
