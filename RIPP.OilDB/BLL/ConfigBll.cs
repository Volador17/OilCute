using RIPP.Lib;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace RIPP.OilDB.BLL
{
   
    public class ConfigBll
    {
        public static string _XMLFileName = "config.xml";
        public static string _audFileName = "manage.default.aud";
        public static string _cutFileName = "app.default.cut";
        public static string _xlsFileName = "app.default.xls";
        public static string _audFilePath = "\\config\\Appcut";
        public static string _cutFilePath = "\\config\\Appxls";
        public static string _xlsFilePath = "\\config\\ManageAud";
        //public static string _XMLFilePath = "\\config\\";
        public static string _startupPath = System.Windows.Forms.Application.StartupPath;
        public static string xmlFilePath = ConfigBll._startupPath + "\\" + ConfigBll._XMLFileName;//路径

        public ConfigBll ()
        {
            if (!File.Exists(xmlFilePath))
            {
                createXml(xmlFilePath);
            }
        }
 
        private  void createXml(string filePath)
        {
            XmlDocument myXmlDoc = new XmlDocument();
            XmlElement rootElement = myXmlDoc.CreateElement("Configure");
            myXmlDoc.AppendChild(rootElement);

            myXmlDoc.Save(xmlFilePath);
 
        }

        private bool findItem(XmlNodeList nodeList, enumModel model)
        {
             bool result = false;
             foreach (XmlNode xn in nodeList)//遍历所有子节点
             {
                 XmlElement xe = (XmlElement)xn;//将子节点类型转换为XmlElement类型
                 if (xe.Name == model.ToString())
                 {
                     result= true;
                     break;
                 }
             }
             return result;
        }

        public void updateItem(string filePath, enumModel model)
        {          
            #region "添加"
            if (!File.Exists(xmlFilePath))
            {
                return ;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            XmlNode root = xmlDoc.SelectSingleNode("Configure");//查找<PersonalResume>
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("Configure").ChildNodes;//获取PersonalResume节点的所有子节点

            if (findItem(nodeList, model))
            {
                foreach (XmlNode xn in nodeList)//遍历所有子节点
                {
                    XmlElement xe = (XmlElement)xn;//将子节点类型转换为XmlElement类型
                    if (xe.Name == model.ToString())
                    {
                        xe.InnerText = filePath;
                        break;
                    }
                }
            }
            else
            {              
                try
                {
                    XmlElement Element = xmlDoc.CreateElement(model.ToString());//创建一个<Resume>节点
                    Element.InnerText = filePath;
                    root.AppendChild(Element);                   
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }              
            }
            xmlDoc.Save(xmlFilePath);
           
            #endregion
        }

        public string getDir( enumModel model)
        {
            #region "添加"
            string str = string.Empty;
           
            if (!File.Exists(xmlFilePath))
            {
                return str;
            }
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(xmlFilePath);

            XmlNode root = xmlDoc.SelectSingleNode("Configure");//查找<PersonalResume>
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("Configure").ChildNodes;//获取PersonalResume节点的所有子节点

            if (findItem(nodeList, model))
            {
                foreach (XmlNode xn in nodeList)//遍历所有子节点
                {
                    XmlElement xe = (XmlElement)xn;//将子节点类型转换为XmlElement类型
                    if (xe.Name == model.ToString())
                    {
                        str = xe.InnerText;  
                        break;
                    }
                }
            }
            return str;
            #endregion
        }
        public static void copy(string sourceFileName,string filName,enumModel model)
        {
            if (File.Exists(sourceFileName))
            {
                switch (model)
                {
                    case enumModel.AppCut:
                        newFloder(_startupPath + _cutFilePath);
                        File.Copy(sourceFileName, _startupPath + _cutFilePath + filName, true);
                        break;
                    case enumModel.AppXls:
                        newFloder(_startupPath + _cutFilePath);
                        File.Copy(sourceFileName, _startupPath + _xlsFilePath + filName, true);
                        break;
                }
            }                      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        public static void newFloder(string dir)
        {
            if (!Directory.Exists(dir))//判断是否存在
            {
                Directory.CreateDirectory(dir);//创建新路径
            }             
        }



    } 
}
