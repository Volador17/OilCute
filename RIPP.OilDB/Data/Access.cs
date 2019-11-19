using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data.DataCheck;
using RIPP.OilDB.UI.GridOil;
using System.Data.SqlClient;
using RIPP.Lib;
using RIPP.OilDB.Model.OilB;
namespace RIPP.OilDB.Data
{
    public sealed class OilDataAccess : abstractDB<OilDataEntity>
    {
        public OilDataAccess()
        {
            this.tableName = "OilData";
            this.keyField = "ID";
        }

        protected override Dictionary<string, string> getProperty(OilDataEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("calData", item.calData);
            dic.Add("labData", item.labData);
            dic.Add("oilInfoID", item.oilInfoID.ToString());
            dic.Add("oilTableColID", item.oilTableColID.ToString());
            dic.Add("oilTableRowID", item.oilTableRowID.ToString());
            dic.Add("ID", item.ID.ToString());
            return dic;
        }

        protected override List<OilDataEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilDataEntity>();
            while (reader.Read())
            {
                var item = new OilDataEntity()
                {
                    calData = RIPP.Lib.Security.SecurityTool.MyDecrypt(reader["calData"].ToString()),
                    //calData = reader["calData"].ToString(),
                    labData = reader["labData"].ToString(),
                    oilInfoID = Convert.ToInt32(reader["oilInfoID"]),
                    oilTableColID = Convert.ToInt32(reader["oilTableColID"]),
                    oilTableRowID = Convert.ToInt32(reader["oilTableRowID"]),
                    ID = Convert.ToInt32(reader["ID"])
                };
                lst.Add(item);
            }
            return lst;
        }

        public override int Insert(OilDataEntity item)
        {
            OilDataEntity newItem = new OilDataEntity();
            newItem.oilInfoID = item.oilInfoID;
            newItem.oilTableColID = item.oilTableColID;
            newItem.oilTableRowID = item.oilTableRowID;
            newItem.labData = item.labData;

            OilTableRowAccess acess = new OilTableRowAccess();
            OilTableRowEntity row = acess.Get(item.oilTableRowID);
            OilTools tools = new OilTools();
            item.calData = tools.calDataDecLimit(item.calData, row.decNumber, row.valDigital);
            //item.calData = tools.calDataDecNumber(item.calData, row.decNumber, row.valDigital);
            newItem.calData = RIPP.Lib.Security.SecurityTool.MyEncrypt(item.calData);
            return base.Insert(newItem);
        }

        public override int Update(OilDataEntity item, string keyValue)
        {
            OilDataEntity newItem = new OilDataEntity();

            newItem.oilInfoID = item.oilInfoID;
            newItem.oilTableColID = item.oilTableColID;
            newItem.oilTableRowID = item.oilTableRowID;
            newItem.labData = item.labData;

            OilTableRowAccess acess = new OilTableRowAccess();
            OilTableRowEntity row = acess.Get(item.oilTableRowID);
            OilTools tools = new OilTools();
            item.calData = tools.calDataDecLimit(item.calData, row.decNumber, row.valDigital);
            //item.calData = tools.calDataDecNumber(item.calData, row.decNumber, row.valDigital);
            newItem.calData = RIPP.Lib.Security.SecurityTool.MyEncrypt(item.calData);
            return base.Update(newItem, keyValue);
        }

        /// <summary>
        /// 根据原油ID以及行号获取对应物性所有值(工具箱范围查找用)
        /// </summary>
        /// <param name="oilID"></param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        public List<OilDataEntity> getOilDataBy_OilID_And_RowID(int oilID, int rowID)
        {
            List<OilDataEntity> oilDatas = new List<OilDataEntity>();

            string strSQL = string.Format("select * from OilData where oilInfoID='{0}' and oilTableRowID='{1}'", oilID, rowID);
            oilDatas = getOilData(strSQL);
            return oilDatas;
        }

        public List<OilDataEntity> getOilData(string sql)
        {
            List<OilDataEntity> oilDatas = new List<OilDataEntity>();
            var reader = SqlHelper.ExecuteReader(SqlHelper.connectionString, CommandType.Text, sql, null);
            while (reader.Read())
            {
                var item = new OilDataEntity()
                {
                    calData = RIPP.Lib.Security.SecurityTool.MyDecrypt(reader["calData"].ToString()),
                    //calData = reader["calData"].ToString(),
                    labData = reader["labData"].ToString(),
                    oilInfoID = Convert.ToInt32(reader["oilInfoID"]),
                    oilTableColID = Convert.ToInt32(reader["oilTableColID"]),
                    oilTableRowID = Convert.ToInt32(reader["oilTableRowID"]),
                    //ID = Convert.ToInt32(reader["ID"])
                };
                oilDatas.Add(item);
            }
            return oilDatas;
        }
    }

    public sealed class OilDataBAccess : abstractDB<OilDataBEntity>
    {
        public OilDataBAccess()
        {
            this.tableName = "OilDataB";
            this.keyField = "ID";
        }

        protected override Dictionary<string, string> getProperty(OilDataBEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("calData", item.calData);
            dic.Add("labData", item.labData);
            dic.Add("oilInfoID", item.oilInfoID.ToString());
            dic.Add("oilTableColID", item.oilTableColID.ToString());
            dic.Add("oilTableRowID", item.oilTableRowID.ToString());
            dic.Add("ID", item.ID.ToString());
            return dic;
        }

        protected override List<OilDataBEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilDataBEntity>();
            while (reader.Read())
            {
                var item = new OilDataBEntity()
                {
                    calData = RIPP.Lib.Security.SecurityTool.MyDecrypt(reader["calData"].ToString()),
                    //calData = reader["calData"].ToString(),
                    labData = reader["labData"].ToString(),
                    oilInfoID = Convert.ToInt32(reader["oilInfoID"]),
                    oilTableColID = Convert.ToInt32(reader["oilTableColID"]),
                    oilTableRowID = Convert.ToInt32(reader["oilTableRowID"]),
                    ID = Convert.ToInt32(reader["ID"])
                };
                lst.Add(item);
            }
            return lst;
        }

        public override int Insert(OilDataBEntity item)
        {
            OilDataBEntity newItem = new OilDataBEntity();
            newItem.oilInfoID = item.oilInfoID;
            newItem.oilTableColID = item.oilTableColID;
            newItem.oilTableRowID = item.oilTableRowID;
            newItem.labData = item.labData;
            newItem.calData = RIPP.Lib.Security.SecurityTool.MyEncrypt(item.calData);
            return base.Insert(newItem);
        }

        public override int Update(OilDataBEntity item, string keyValue)
        {
            OilDataBEntity newItem = new OilDataBEntity();

            newItem.oilInfoID = item.oilInfoID;
            newItem.oilTableColID = item.oilTableColID;
            newItem.oilTableRowID = item.oilTableRowID;
            newItem.labData = item.labData;
            newItem.calData = RIPP.Lib.Security.SecurityTool.MyEncrypt(item.calData);
            return base.Update(newItem, keyValue);
        }
    }

    public class OilInfoAccess : abstractDB<OilInfoEntity>
    {
        public OilInfoAccess()
        {
            this.tableName = "OilInfo";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(OilInfoEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("crudeName", item.crudeName);
            dic.Add("englishName", item.englishName);
            dic.Add("crudeIndex", item.crudeIndex);
            dic.Add("country", item.country);
            dic.Add("region", item.region);
            dic.Add("fieldBlock", item.fieldBlock);
            dic.Add("sampleDate", item.sampleDate.ToString());
            dic.Add("receiveDate", item.receiveDate.ToString());
            dic.Add("sampleSite", item.sampleSite);
            dic.Add("assayDate", item.assayDate.ToString());
            dic.Add("updataDate", item.updataDate.ToString());
            dic.Add("sourceRef", item.sourceRef);
            dic.Add("assayLab", item.assayLab);
            dic.Add("assayer", item.assayer);
            dic.Add("reportIndex", item.reportIndex);
            dic.Add("assayCustomer", item.assayCustomer);
            dic.Add("summary", item.summary);
            dic.Add("type", item.type);
            dic.Add("classification", item.classification);
            dic.Add("sulfurLevel", item.sulfurLevel);
            dic.Add("acidLevel", item.acidLevel);
            dic.Add("corrosionLevel", item.corrosionLevel);
            dic.Add("processingIndex", item.processingIndex);
            dic.Add("NIRSpectrum", item.NIRSpectrum);
            dic.Add("BlendingType", item.BlendingType);
            dic.Add("ICP0", item.ICP0);

            dic.Add("DataQuality", item.DataQuality);
            dic.Add("Remark", item.Remark);
            dic.Add("S_01R", item.S_01R);
            dic.Add("S_02R", item.S_02R);
            dic.Add("S_03R", item.S_03R);
            dic.Add("S_04R", item.S_04R);
            dic.Add("S_05R", item.S_05R);
            dic.Add("S_06R", item.S_06R);
            dic.Add("S_07R", item.S_07R);
            dic.Add("S_08R", item.S_08R);
            dic.Add("S_09R", item.S_09R);
            dic.Add("S_10R", item.S_10R);
            dic.Add("DataSource", item.DataSource);

            return dic;
        }
        protected override List<OilInfoEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilInfoEntity>();
            while (reader.Read())
            {
                OilInfoEntity item = new OilInfoEntity();
                item.ID = Convert.ToInt32(reader["ID"]);
                item.crudeName = reader["crudeName"].ToString();
                item.crudeIndex = reader["crudeIndex"].ToString();
                item.englishName = reader["englishName"].ToString();
                item.country = reader["country"].ToString();                                     
                item.region = reader["region"].ToString();
                item.fieldBlock = reader["fieldBlock"].ToString();
                item.sampleDate = DateTime.Parse(reader["sampleDate"].ToString());
                item.receiveDate = DateTime.Parse(reader["receiveDate"].ToString());
                item.sampleSite = reader["sampleSite"].ToString();
                item.assayDate = reader["assayDate"].ToString();
                item.updataDate = reader["updataDate"].ToString();
                item.sourceRef = reader[12].ToString();
                item.assayLab = reader[13].ToString();
                item.assayer = reader[14].ToString();
                item.assayCustomer = reader[15].ToString();
                item.reportIndex = reader[16].ToString();
                item.summary = reader[17].ToString();
                item.type = reader[18].ToString();
                item.classification = reader[19].ToString();
                item.sulfurLevel = reader[20].ToString();
                item.acidLevel = reader[21].ToString();
                item.corrosionLevel = reader[22].ToString();
                item.processingIndex = reader[23].ToString();
                item.NIRSpectrum = reader[24].ToString();
                item.BlendingType = reader[25].ToString();
                item.ICP0 = reader["ICP0"].ToString();

                item.DataQuality = reader["DataQuality"].ToString();
                item.Remark = reader["Remark"].ToString();
                item.S_01R = reader["S_01R"].ToString();
                item.S_02R = reader["S_02R"].ToString();
                item.S_03R = reader["S_03R"].ToString();
                item.S_04R = reader["S_04R"].ToString();
                item.S_05R = reader["S_05R"].ToString();
                item.S_06R = reader["S_06R"].ToString();
                item.S_07R = reader["S_07R"].ToString();
                item.S_08R = reader["S_08R"].ToString();
                item.S_09R = reader["S_09R"].ToString();
                item.S_10R = reader["S_10R"].ToString();
                item.DataSource = reader["DataSource"].ToString();
                lst.Add(item);
                if (item.sampleDate.HasValue && item.sampleDate.Value.Year <= 1900)
                    item.sampleDate = null;
                if (item.receiveDate.HasValue && item.receiveDate.Value.Year <= 1900)
                    item.receiveDate = null;
            }
            return lst;
        }

        /// <summary>
        /// 获取不重复的Id
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<string> getCrudeIndex(string sql)
        {
            List<string> crudeIndexList = new List<string>();
            var reader = SqlHelper.ExecuteReader(SqlHelper.connectionString, CommandType.Text, sql, null);
            while (reader.Read())
            {
                string crudeIndex = reader["crudeIndex"] == null ? string.Empty : reader["crudeIndex"].ToString();
                if (crudeIndex!=string.Empty)
                    crudeIndexList.Add(crudeIndex);
            }
            return crudeIndexList;
        }
    }
    /// <summary>
    /// 一条原油B的获取
    /// </summary>
    public class OilBAccess  
    {
        private OilBEntitiy _oilB = new OilBEntitiy();//获取的原油数据
        private OilInfoBAccess oilInfoBAccess = new OilInfoBAccess();//
        private OilDataBAccess oilDataBAccess = new OilDataBAccess();//
        private CurveAccess curveAccess = new CurveAccess();//

        public OilBAccess()
        {      
       
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iOilID"></param>
        /// <returns></returns>
        public OilBEntitiy getOilB(int iOilID)
        {
            if (this._oilB.OilDataBs != null)
                this._oilB.OilDataBs.Clear();
            this._oilB.OilDataBs = new OilDataBAccess().Get(string.Format(" oilInfoID = {0}", iOilID));

            if (this._oilB.curves != null)
                this._oilB.curves.Clear();
            this._oilB.curves = curveAccess.Get("oilInfoID ={0}", iOilID);

            return _oilB;
        }

        /// <summary>
        /// 获取所有B库数据
        /// </summary>
        /// <returns></returns>
        public List<OilBEntitiy> getAllOilB()
        {
            List<OilBEntitiy> oilBList = new List<OilBEntitiy>();
            List<OilInfoBEntity> oilInfoBList = new OilInfoBAccess().Get("1=1");

            foreach (var temp in oilInfoBList)
            {
                if (temp.ID > 0)
                    oilBList.Add(getOilB(temp.ID));
            }
            return oilBList;
        }
    }
    public class OilInfoACrudeIndexIDAccess : abstractDB<CrudeIndexIDAEntity>
    {
        public OilInfoACrudeIndexIDAccess()
        {
            this.tableName = "OilInfo";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(CrudeIndexIDAEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("crudeName", item.crudeName);
            dic.Add("englishName", item.englishName);
            dic.Add("crudeIndex", item.crudeIndex);
            dic.Add("country", item.country);
            dic.Add("region", item.region);
            dic.Add("fieldBlock", item.fieldBlock);
            dic.Add("sampleDate", item.sampleDate.ToString());
            dic.Add("receiveDate", item.receiveDate.ToString());
            dic.Add("sampleSite", item.sampleSite);
            dic.Add("assayDate", item.assayDate.ToString());
            dic.Add("updataDate", item.updataDate.ToString());
            dic.Add("sourceRef", item.sourceRef);
            dic.Add("assayLab", item.assayLab);
            dic.Add("assayer", item.assayer);
            dic.Add("reportIndex", item.reportIndex);
            //dic.Add("assayCustomer", item.assayCustomer);
            dic.Add("summary", item.summary);
            dic.Add("type", item.type);
            dic.Add("classification", item.classification);
            dic.Add("sulfurLevel", item.sulfurLevel);
            dic.Add("acidLevel", item.acidLevel);
            dic.Add("corrosionLevel", item.corrosionLevel);
            dic.Add("processingIndex", item.processingIndex);
            dic.Add("NIRSpectrum", item.NIRSpectrum);
            dic.Add("BlendingType", item.BlendingType);
            return dic;
        }
        protected override List<CrudeIndexIDAEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<CrudeIndexIDAEntity>();
            while (reader.Read())
            {
                CrudeIndexIDAEntity item = new CrudeIndexIDAEntity();
                item.ID = Convert.ToInt32(reader["ID"]);
                item.crudeName = reader["crudeName"].ToString();
                item.crudeIndex = reader["crudeIndex"].ToString();
                item.englishName = reader["englishName"].ToString();
                item.country = reader["country"].ToString();
                item.region = reader["region"].ToString();
                item.fieldBlock = reader["fieldBlock"].ToString();
                item.sampleDate = DateTime.Parse(reader["sampleDate"].ToString());
                item.receiveDate = DateTime.Parse(reader["receiveDate"].ToString());
                item.sampleSite = reader["sampleSite"].ToString();
                item.assayDate = reader["assayDate"].ToString();
                item.updataDate = reader["updataDate"].ToString();
                item.sourceRef = reader["sourceRef"].ToString();
                item.assayLab = reader["assayLab"].ToString();
                item.assayer = reader["assayer"].ToString();
                //item.assayCustomer = reader[15].ToString();
                item.reportIndex = reader["reportIndex"].ToString();
                item.summary = reader["summary"].ToString();
                item.type = reader["type"].ToString();
                item.classification = reader["classification"].ToString();
                item.sulfurLevel = reader["sulfurLevel"].ToString();
                item.acidLevel = reader["acidLevel"].ToString();
                item.corrosionLevel = reader["corrosionLevel"].ToString();
                item.processingIndex = reader["processingIndex"].ToString();
                item.NIRSpectrum = reader["NIRSpectrum"].ToString();
                item.BlendingType = reader["BlendingType"].ToString();
                if (item.sampleDate.HasValue && item.sampleDate.Value.Year <= 1900)
                    item.sampleDate = null;
                if (item.receiveDate.HasValue && item.receiveDate.Value.Year <= 1900)
                    item.receiveDate = null;
                lst.Add(item);
            }
            return lst;
        }
    }

    public class OilInfoBCrudeIndexIDAccess : abstractDB<CrudeIndexIDBEntity>
    {
        public OilInfoBCrudeIndexIDAccess()
        {
            this.tableName = "OilInfoB";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(CrudeIndexIDBEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("crudeName", item.crudeName);
            dic.Add("englishName", item.englishName);
            dic.Add("crudeIndex", item.crudeIndex);
            dic.Add("country", item.country);
            dic.Add("region", item.region);
            dic.Add("fieldBlock", item.fieldBlock);
            dic.Add("sampleDate", item.sampleDate.ToString());
            dic.Add("receiveDate", item.receiveDate.ToString());
            dic.Add("sampleSite", item.sampleSite);
            dic.Add("assayDate", item.assayDate.ToString());
            dic.Add("updataDate", item.updataDate.ToString());
            dic.Add("sourceRef", item.sourceRef);
            dic.Add("assayLab", item.assayLab);
            dic.Add("assayer", item.assayer);
            dic.Add("reportIndex", item.reportIndex);
            //dic.Add("assayCustomer", item.assayCustomer);
            dic.Add("summary", item.summary);
            dic.Add("type", item.type);
            dic.Add("classification", item.classification);
            dic.Add("sulfurLevel", item.sulfurLevel);
            dic.Add("acidLevel", item.acidLevel);
            dic.Add("corrosionLevel", item.corrosionLevel);
            dic.Add("processingIndex", item.processingIndex);
            dic.Add("NIRSpectrum", item.NIRSpectrum);
            dic.Add("BlendingType", item.BlendingType);
         
            return dic;
        }
        protected override List<CrudeIndexIDBEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<CrudeIndexIDBEntity>();
            while (reader.Read())
            {
                CrudeIndexIDBEntity item = new CrudeIndexIDBEntity();
                item.ID = Convert.ToInt32(reader["ID"]);
                item.crudeName = reader["crudeName"].ToString();
                item.crudeIndex = reader["crudeIndex"].ToString();
                item.englishName = reader["englishName"].ToString();
                item.country = reader["country"].ToString();
                item.region = reader["region"].ToString();
                item.fieldBlock = reader["fieldBlock"].ToString();
                item.sampleDate = DateTime.Parse(reader["sampleDate"].ToString());
                item.receiveDate = DateTime.Parse(reader["receiveDate"].ToString());
                item.sampleSite = reader["sampleSite"].ToString();
                item.assayDate = reader["assayDate"].ToString();
                item.updataDate = reader["updataDate"].ToString();
                item.sourceRef = reader["sourceRef"].ToString();
                item.assayLab = reader["assayLab"].ToString();
                item.assayer = reader["assayer"].ToString();
                //item.assayCustomer = reader[15].ToString();
                item.reportIndex = reader["reportIndex"].ToString();
                item.summary = reader["summary"].ToString();
                item.type = reader["type"].ToString();
                item.classification = reader["classification"].ToString();
                item.sulfurLevel = reader["sulfurLevel"].ToString();
                item.acidLevel = reader["acidLevel"].ToString();
                item.corrosionLevel = reader["corrosionLevel"].ToString();
                item.processingIndex = reader["processingIndex"].ToString();
                item.NIRSpectrum = reader["NIRSpectrum"].ToString();
                item.BlendingType = reader["BlendingType"].ToString();

                if (item.sampleDate.HasValue && item.sampleDate.Value.Year <= 1900)
                    item.sampleDate = null;
                if (item.receiveDate.HasValue && item.receiveDate.Value.Year <= 1900)
                    item.receiveDate = null;
                lst.Add(item);
            }
            return lst;
        }
    }

    public class OilInfoBAccess : abstractDB<OilInfoBEntity>
    {
        public OilInfoBAccess()
        {
            this.tableName = "OilInfoB";
            this.keyField = "ID";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(OilInfoBEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("crudeName", item.crudeName);
            dic.Add("englishName", item.englishName);
            dic.Add("crudeIndex", item.crudeIndex);
            dic.Add("country", item.country);
            dic.Add("region", item.region);
            dic.Add("fieldBlock", item.fieldBlock);
            dic.Add("sampleDate", item.sampleDate == null ? string.Empty : item.sampleDate.ToString());
            dic.Add("receiveDate", item.receiveDate == null ? string.Empty : item.receiveDate.ToString());
            dic.Add("sampleSite", item.sampleSite);
            dic.Add("assayDate", item.assayDate.ToString());
            dic.Add("updataDate", item.updataDate.ToString());
            dic.Add("sourceRef", item.sourceRef);
            dic.Add("assayLab", item.assayLab);
            dic.Add("assayer", item.assayer);
            dic.Add("reportIndex", item.reportIndex);
            dic.Add("assayCustomer", item.assayCustomer);
            dic.Add("summary", item.summary);
            dic.Add("type", item.type);
            dic.Add("classification", item.classification);
            dic.Add("sulfurLevel", item.sulfurLevel);
            dic.Add("acidLevel", item.acidLevel);
            dic.Add("corrosionLevel", item.corrosionLevel);
            dic.Add("processingIndex", item.processingIndex);
            dic.Add("NIRSpectrum", item.NIRSpectrum);
            dic.Add("BlendingType", item.BlendingType);
            dic.Add("ICP0", item.ICP0);

            dic.Add("DataQuality", item.DataQuality);
            dic.Add("Remark", item.Remark);
            dic.Add("S_01R", item.S_01R);
            dic.Add("S_02R", item.S_02R);
            dic.Add("S_03R", item.S_03R);
            dic.Add("S_04R", item.S_04R);
            dic.Add("S_05R", item.S_05R);
            dic.Add("S_06R", item.S_06R);
            dic.Add("S_07R", item.S_07R);
            dic.Add("S_08R", item.S_08R);
            dic.Add("S_09R", item.S_09R);
            dic.Add("S_10R", item.S_10R);
            dic.Add("DataSource", item.DataSource);

            if (item.sampleDate.HasValue && item.sampleDate.Value.Year <= 1900)
                item.sampleDate = null;
            if (item.receiveDate.HasValue && item.receiveDate.Value.Year <= 1900)
                item.receiveDate = null;
            return dic;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<OilInfoBEntity> dataReaderToEntity(IDataReader reader)
        {
            OilDataCheck dataCheck = new OilDataCheck();
            var lst = new List<OilInfoBEntity>();
            while (reader.Read())
            {
                OilInfoBEntity item = new OilInfoBEntity();               
                item.ID = Convert.ToInt32(reader["ID"]);
                item.crudeName = reader["crudeName"].ToString();
                item.englishName = reader["englishName"].ToString();
                item.crudeIndex = reader["crudeIndex"].ToString();
                item.country = reader["country"].ToString();
                item.region = reader["region"].ToString();
                item.fieldBlock = reader["fieldBlock"].ToString();
                item.sampleDate = dataCheck.GetDate(reader["sampleDate"]);
                item.receiveDate = dataCheck.GetDate(reader["receiveDate"]);
                item.sampleSite = reader["sampleSite"].ToString();
                item.assayDate = reader["assayDate"].ToString();
                item.updataDate = reader["updataDate"].ToString();
                item.sourceRef = reader[12].ToString();
                item.assayLab = reader[13].ToString();
                item.assayer = reader[14].ToString();
                item.assayCustomer = reader[15].ToString();
                item.reportIndex = reader[16].ToString();
                item.summary = reader[17].ToString();
                item.type = reader[18].ToString();
                item.classification = reader[19].ToString();
                item.sulfurLevel = reader[20].ToString();
                item.acidLevel = reader[21].ToString();
                item.corrosionLevel = reader[22].ToString();
                item.processingIndex = reader[23].ToString();
                item.NIRSpectrum = reader[24].ToString();
                item.BlendingType = reader[25].ToString();
                item.ICP0 = reader["ICP0"].ToString();

                item.DataQuality = reader["DataQuality"].ToString();
                item.Remark = reader["Remark"].ToString();
                item.S_01R = reader["S_01R"].ToString();
                item.S_02R = reader["S_02R"].ToString();
                item.S_03R = reader["S_03R"].ToString();
                item.S_04R = reader["S_04R"].ToString();
                item.S_05R = reader["S_05R"].ToString();
                item.S_06R = reader["S_06R"].ToString();
                item.S_07R = reader["S_07R"].ToString();
                item.S_08R = reader["S_08R"].ToString();
                item.S_09R = reader["S_09R"].ToString();
                item.S_10R = reader["S_10R"].ToString();
                item.DataSource = reader["DataSource"].ToString();
                lst.Add(item);
                if (item.sampleDate.HasValue && item.sampleDate.Value.Year <= 1900)
                    item.sampleDate = null;
                if (item.receiveDate.HasValue && item.receiveDate.Value.Year <= 1900)
                    item.receiveDate = null;
            }
            return lst;
        }
    }

    #region outLib
    public class OilInfoOutAccess : abstractDB<OilInfoOut>
    {
        public OilInfoOutAccess(string tableName = "OilInfo")
        {
            this.tableName = tableName;
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(OilInfoOut item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("crudeName", item.crudeName);
            dic.Add("englishName", item.englishName);
            dic.Add("crudeIndex", item.crudeIndex);
            dic.Add("country", item.country);
            dic.Add("region", item.region);
            dic.Add("fieldBlock", item.fieldBlock);
            dic.Add("sampleDate", item.sampleDate.ToString());
            dic.Add("receiveDate", item.receiveDate.ToString());
            dic.Add("sampleSite", item.sampleSite);
            dic.Add("assayDate", item.assayDate.ToString());
            dic.Add("updataDate", item.updataDate.ToString());
            dic.Add("sourceRef", item.sourceRef);
            dic.Add("assayLab", item.assayLab);
            dic.Add("assayer", item.assayer);
            dic.Add("reportIndex", item.reportIndex);
            dic.Add("assayCustomer", item.assayCustomer);
            dic.Add("summary", item.summary);
            dic.Add("type", item.type);
            dic.Add("classification", item.classification);
            dic.Add("sulfurLevel", item.sulfurLevel);
            dic.Add("acidLevel", item.acidLevel);
            dic.Add("corrosionLevel", item.corrosionLevel);
            dic.Add("processingIndex", item.processingIndex);
            dic.Add("NIRSpectrum", item.NIRSpectrum);
            dic.Add("BlendingType", item.BlendingType);
            dic.Add("ICP0", item.ICP0);


            dic.Add("DataQuality", item.DataQuality);
            dic.Add("Remark", item.Remark);
            dic.Add("S_01R", item.S_01R);
            dic.Add("S_02R", item.S_02R);
            dic.Add("S_03R", item.S_03R);
            dic.Add("S_04R", item.S_04R);
            dic.Add("S_05R", item.S_05R);
            dic.Add("S_06R", item.S_06R);
            dic.Add("S_07R", item.S_07R);
            dic.Add("S_08R", item.S_08R);
            dic.Add("S_09R", item.S_09R);
            dic.Add("S_10R", item.S_10R);
            dic.Add("DataSource", item.DataSource);
            return dic;
        }
        protected override List<OilInfoOut> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilInfoOut>();
            while (reader.Read())
            {
                OilInfoOut item = new OilInfoOut();
                item.ID = Convert.ToInt32(reader["ID"]);
                item.crudeName = reader["crudeName"].ToString();
                item.englishName = reader["englishName"].ToString();
                item.crudeIndex = reader["crudeIndex"].ToString() ;
                item.country = reader["country"].ToString();
                item.region = reader["region"].ToString();
                item.fieldBlock = reader["fieldBlock"].ToString();
                item.sampleDate = DateTime.Parse(reader["sampleDate"].ToString()) ;
                item.receiveDate = DateTime.Parse(reader["receiveDate"].ToString()); 
                item.sampleSite = reader["sampleSite"].ToString(); 
                item.assayDate = reader["assayDate"].ToString(); 
                item.updataDate = reader["updataDate"].ToString();
                item.sourceRef = reader["sourceRef"].ToString();
                item.assayLab = reader["assayLab"].ToString() ;
                item.assayer = reader["assayer"].ToString() ;
                item.assayCustomer = reader["assayCustomer"].ToString() ;
                item.reportIndex = reader["reportIndex"].ToString() ;
                item.summary = reader["summary"].ToString() ;
                item.type = reader["type"].ToString() ;
                item.classification = reader["classification"].ToString() ;
                item.sulfurLevel = reader["sulfurLevel"].ToString() ;
                item.acidLevel = reader["acidLevel"].ToString() ;
                item.corrosionLevel = reader["corrosionLevel"].ToString() ;
                item.processingIndex = reader["processingIndex"].ToString() ;  
                item.NIRSpectrum = reader["NIRSpectrum"].ToString() ;  
                item.BlendingType = reader["BlendingType"].ToString() ;  
                item.ICP0 = reader["ICP0"].ToString() ;  
                item.DataQuality = reader["DataQuality"].ToString() ;  
                item.Remark = reader["Remark"].ToString() ;  
                item.S_01R = reader["S_01R"].ToString() ;  
                item.S_02R = reader["S_02R"].ToString() ;  
                item.S_03R = reader["S_03R"].ToString() ;    
                item.S_04R = reader["S_04R"].ToString(); 
                item.S_05R = reader["S_05R"].ToString(); 
                item.S_06R = reader["S_06R"].ToString(); 
                item.S_07R = reader["S_07R"].ToString(); 
                item.S_08R = reader["S_08R"].ToString(); 
                item.S_09R = reader["S_09R"].ToString(); 
                item.S_10R = reader["S_10R"].ToString(); 
                item.DataSource = reader["DataSource"].ToString();            
                lst.Add(item);
            }
            return lst;
        }
    }

    public sealed class OilDataOutAccess : abstractDB<OilDataOut>
    {
        public OilDataOutAccess(string tableName = "OilData")
        {
            this.tableName = tableName;
            this.keyField = "ID";
        }

        protected override Dictionary<string, string> getProperty(OilDataOut item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("calData", item.calData);
            dic.Add("labData", item.labData);
            dic.Add("oilInfoID", item.oilInfoID.ToString());
            dic.Add("oilTableColID", item.oilTableColID.ToString());
            dic.Add("oilTableRowID", item.oilTableRowID.ToString());
            dic.Add("ID", item.ID.ToString());
            return dic;
        }


        protected override List<OilDataOut> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilDataOut>();
            while (reader.Read())
            {
                var item = new OilDataOut()
                {
                    calData = RIPP.Lib.Security.SecurityTool.MyDecrypt(reader["calData"].ToString()),
                    //calData = reader["calData"].ToString(),
                    labData = reader["labData"].ToString(),
                    oilInfoID = Convert.ToInt32(reader["oilInfoID"]),
                    oilTableColID = Convert.ToInt32(reader["oilTableColID"]),
                    oilTableRowID = Convert.ToInt32(reader["oilTableRowID"]),
                    ID = Convert.ToInt32(reader["ID"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class OilTableColOutAccess : abstractDB<OilTableColOut>
    {
        public OilTableColOutAccess()
        {
            this.tableName = "OilTableCol";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(OilTableColOut item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("oilTableTypeID", item.oilTableTypeID.ToString());
            dic.Add("colName", item.colName);
            dic.Add("colOrder", item.colOrder.ToString());
            dic.Add("isDisplay", item.isDisplay.ToString());
            dic.Add("descript", item.descript);
            dic.Add("isSystem", item.isSystem.ToString());
            dic.Add("colCode", item.colCode.ToString());
            dic.Add("isDisplayLab", item.isDisplayLab.ToString());
            return dic;
        }

        protected override List<OilTableColOut> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilTableColOut>();
            while (reader.Read())
            {
                var item = new OilTableColOut()
                {

                    ID = Convert.ToInt32(reader["ID"]),
                    oilTableTypeID = Convert.ToInt32(reader["oilTableTypeID"]),
                    colName = Convert.ToString(reader["colName"]),
                    colOrder = Convert.ToInt32(reader["colOrder"]),
                    isDisplay = Convert.ToBoolean(reader["isDisplay"]),
                    descript = Convert.ToString(reader["descript"]),
                    isSystem = Convert.ToBoolean(reader["isSystem"]),
                    colCode = Convert.ToString(reader["colCode"]),
                    isDisplayLab = Convert.ToBoolean(reader["isDisplayLab"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class OilTableRowOutAccess : abstractDB<OilTableRowOut>
    {
        public OilTableRowOutAccess()
        {
            this.tableName = "OilTableRow";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(OilTableRowOut item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.ID.ToString());
            dic.Add("oilTableTypeID", item.oilTableTypeID.ToString());
            dic.Add("itemOrder", item.itemOrder.ToString());
            dic.Add("itemName", item.itemName);
            dic.Add("itemEnName", item.itemEnName);
            dic.Add("itemUnit", item.itemUnit);
            dic.Add("itemCode", item.itemCode);
            dic.Add("dataType", item.dataType);
            dic.Add("decNumber", item.decNumber.ToString());
            dic.Add("valDigital", item.valDigital.ToString());
            dic.Add("isKey", item.isKey.ToString());
            dic.Add("isDisplay", item.isDisplay.ToString());
            dic.Add("trend", item.trend.ToString());
            dic.Add("errDownLimit", item.errDownLimit.ToString());
            dic.Add("errUpLimit", item.errUpLimit.ToString());
            dic.Add("alertDownLimit", item.alertDownLimit.ToString());
            dic.Add("alertUpLimit", item.alertUpLimit.ToString());
            dic.Add("evalDownLimit", item.evalDownLimit.ToString());
            dic.Add("evalUpLimit", item.evalUpLimit.ToString());
            dic.Add("descript", item.descript);
            dic.Add("subItemName", item.subItemName);
            dic.Add("isSystem", item.isSystem.ToString());

            return dic;
        }

        protected override List<OilTableRowOut> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilTableRowOut>();
            while (reader.Read())
            {
                var aa = Convert.IsDBNull(reader["itemEnName"]) ? "" : Convert.ToString(reader["itemEnName"]);
                var item = new OilTableRowOut()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    oilTableTypeID = Convert.ToInt32(reader["oilTableTypeID"]),
                    itemOrder = Convert.ToInt32(reader["itemOrder"]),
                    itemName = Convert.ToString(reader["itemName"]),
                    itemEnName = Convert.IsDBNull(reader["itemEnName"]) ? "" : Convert.ToString(reader["itemEnName"]),
                    itemUnit = Convert.IsDBNull(reader["itemUnit"]) ? "" : Convert.ToString(reader["itemUnit"]),
                    itemCode = Convert.ToString(reader["itemCode"]),
                    dataType = Convert.IsDBNull(reader["dataType"]) ? "" : Convert.ToString(reader["dataType"]),
                    decNumber = reader["decNumber"] == DBNull.Value ? 4 : Convert.ToInt32(reader["decNumber"]),
                    valDigital = reader["valDigital"] == DBNull.Value ? 4 : Convert.ToInt32(reader["valDigital"]),
                    isKey = reader["isKey"] == DBNull.Value ? false : Convert.ToBoolean(reader["isKey"]),
                    isDisplay = reader["isDisplay"] == DBNull.Value ? true : Convert.ToBoolean(reader["isDisplay"]),
                    trend = Convert.IsDBNull(reader["trend"]) ? "" : Convert.ToString(reader["trend"]),
                    errDownLimit = Convert.IsDBNull(reader["errDownLimit"]) ? 0 : Convert.ToSingle(reader["errDownLimit"]),
                    errUpLimit = Convert.IsDBNull(reader["errUpLimit"]) ? 0 : Convert.ToSingle(reader["errUpLimit"]),
                    alertDownLimit = Convert.IsDBNull(reader["alertDownLimit"]) ? float.NaN : Convert.ToSingle(reader["alertDownLimit"]),
                    alertUpLimit = Convert.IsDBNull(reader["alertUpLimit"]) ? float.NaN : Convert.ToSingle(reader["alertUpLimit"]),
                    evalDownLimit = Convert.IsDBNull(reader["evalDownLimit"]) ? 0 : Convert.ToSingle(reader["evalDownLimit"]),
                    evalUpLimit = Convert.IsDBNull(reader["evalUpLimit"]) ? 0 : Convert.ToSingle(reader["evalUpLimit"]),
                    descript = Convert.IsDBNull(reader["descript"]) ? "" : Convert.ToString(reader["descript"]),
                    subItemName = Convert.IsDBNull(reader["subItemName"]) ? "" : Convert.ToString(reader["subItemName"]),
                    isSystem = reader["isSystem"] == DBNull.Value ? true : Convert.ToBoolean(reader["isSystem"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    #endregion
    /// <summary>
    /// 原油的信息提示
    /// </summary>
    public class OilTipTableAccess : abstractDB<OilTipTableEntity>
    {
        public OilTipTableAccess()
        {
            this.tableName = "OilTipTable";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(OilTipTableEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("oilTableTypeID", item.oilTableTypeID.ToString());
            dic.Add("tip", item.Tip);
           
            return dic;
        }

        protected override List<OilTipTableEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilTipTableEntity>();
            while (reader.Read())
            {
                var item = new OilTipTableEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    oilTableTypeID = Convert.ToInt32(reader["oilTableTypeID"]),
                    Tip = Convert.ToString(reader["tip"])                     
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    [Serializable]
    public class OilTableColAccess : abstractDB<OilTableColEntity>
    {
        public OilTableColAccess()
        {
            this.tableName = "OilTableCol";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(OilTableColEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("oilTableTypeID", item.oilTableTypeID.ToString());
            dic.Add("colName", item.colName);
            dic.Add("colOrder", item.colOrder.ToString());
            dic.Add("isDisplay", item.isDisplay.ToString());
            dic.Add("descript", item.descript);
            dic.Add("isSystem", item.isSystem.ToString());
            dic.Add("colCode", item.colCode.ToString());
            dic.Add("isDisplayLab", item.isDisplayLab.ToString());
            return dic;
        }

        protected override List<OilTableColEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilTableColEntity>();
            while (reader.Read())
            {
                var item = new OilTableColEntity()
                {

                    ID = Convert.ToInt32(reader["ID"]),
                    oilTableTypeID = Convert.ToInt32(reader["oilTableTypeID"]),
                    colName = Convert.ToString(reader["colName"]),
                    colOrder = Convert.ToInt32(reader["colOrder"]),
                    isDisplay = Convert.ToBoolean(reader["isDisplay"]),
                    descript = Convert.ToString(reader["descript"]),
                    isSystem = Convert.ToBoolean(reader["isSystem"]),
                    colCode = Convert.ToString(reader["colCode"]),
                    isDisplayLab = Convert.ToBoolean(reader["isDisplayLab"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    [Serializable]
    public class OilTableRowAccess : abstractDB<OilTableRowEntity>
    {
        public OilTableRowAccess()
        {
            this.tableName = "OilTableRow";
            this.keyField = "ID";
        }
        /// <summary>
        /// 向数据库写入数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(OilTableRowEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.ID.ToString());
            dic.Add("oilTableTypeID", item.oilTableTypeID.ToString());
            dic.Add("itemOrder", item.itemOrder.ToString());
            dic.Add("itemName", item.itemName);
            dic.Add("itemEnName", item.itemEnName);
            dic.Add("itemUnit", item.itemUnit);
            dic.Add("itemCode", item.itemCode);
            dic.Add("dataType", item.dataType);
            dic.Add("decNumber", item.decNumber.ToString());
            dic.Add("valDigital", item.valDigital.ToString());
            dic.Add("isKey", item.isKey.ToString());
            dic.Add("isDisplay", item.isDisplay.ToString());
            dic.Add("trend", item.trend.ToString());
            dic.Add("errDownLimit", item.errDownLimit.ToString());
            dic.Add("errUpLimit", item.errUpLimit.ToString());
            dic.Add("alertDownLimit", item.alertDownLimit.ToString());
            dic.Add("alertUpLimit", item.alertUpLimit.ToString());
            dic.Add("evalDownLimit", item.evalDownLimit.ToString());
            dic.Add("evalUpLimit", item.evalUpLimit.ToString());
            dic.Add("outExcel",  ((int)item.OutExcel).ToString());
            dic.Add("descript", item.descript);
            dic.Add("subItemName", item.subItemName);
            dic.Add("isSystem", item.isSystem.ToString());

            return dic;
        }

        /// <summary>
        /// 获取不重复的itemCode
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<string> getAllItemCode(string sql)
        {
            List<string> itemCodeList = new List<string>();
            var reader = SqlHelper.ExecuteReader(SqlHelper.connectionString, CommandType.Text, sql, null);
            while (reader.Read())
            {
                string itemCode = reader["itemCode"].ToString();
                if (!itemCodeList.Contains(itemCode))
                    itemCodeList.Add(itemCode);
            }
            return itemCodeList;
        }

        /// <summary>
        /// 根据物性代码和表类型获取物性行ID
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="oilTableTypeID"></param>
        /// <returns></returns>
        public int getRowIDBy_ItemCode_And_oilTableTypeID(string itemCode, int oilTableTypeID)
        {
            int result = 0;

            string strSQL = string.Format("select ID from OilTableRow where itemCode='{0}' and oilTableTypeID='{1}'", itemCode, oilTableTypeID);
            var id = SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, strSQL, null);
            if (id != null)
            {
                result = Convert.ToInt32(id);
            }
            return result;
        }
        /// <summary>
        /// 从数据库读出数据
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<OilTableRowEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilTableRowEntity>();
            while (reader.Read())
            {
                var aa = Convert.IsDBNull(reader["itemEnName"]) ? "" : Convert.ToString(reader["itemEnName"]);
                OilTableRowEntity item = new OilTableRowEntity();
                lst.Add(item);
                item.ID = Convert.ToInt32(reader["ID"]);
                item.oilTableTypeID = Convert.ToInt32(reader["oilTableTypeID"]);
                item.itemOrder = Convert.ToInt32(reader["itemOrder"]);
                item.itemName = Convert.ToString(reader["itemName"]);
                item.itemEnName = Convert.IsDBNull(reader["itemEnName"]) ? "" : Convert.ToString(reader["itemEnName"]);
                item.itemUnit = Convert.IsDBNull(reader["itemUnit"]) ? "" : Convert.ToString(reader["itemUnit"]);
                item.itemCode = Convert.ToString(reader["itemCode"]);
                item.dataType = Convert.IsDBNull(reader["dataType"]) ? "" : Convert.ToString(reader["dataType"]);
                item.decNumber = reader["decNumber"] == DBNull.Value ? null : Convert.ToInt32(reader["decNumber"]) as Int32?;
                item.valDigital = reader["valDigital"] == DBNull.Value ? 1 : Convert.ToInt32(reader["valDigital"]);
                item.isKey = reader["isKey"] == DBNull.Value ? false : Convert.ToBoolean(reader["isKey"]);
                item.isDisplay = reader["isDisplay"] == DBNull.Value ? true : Convert.ToBoolean(reader["isDisplay"]);
                item.trend = Convert.IsDBNull(reader["trend"]) ? "" : Convert.ToString(reader["trend"]);
                
                if (Convert.IsDBNull(reader["errDownLimit"]))
                    item.errDownLimit = null;
                else
                    item.errDownLimit = Convert.ToSingle(reader["errDownLimit"]);

                if (Convert.IsDBNull(reader["errUpLimit"]))
                    item.errUpLimit = null;
                else
                    item.errUpLimit = Convert.ToSingle(reader["errUpLimit"]);

                item.alertDownLimit = Convert.IsDBNull(reader["alertDownLimit"]) ? 0 : Convert.ToSingle(reader["alertDownLimit"]);
                item.alertUpLimit = Convert.IsDBNull(reader["alertUpLimit"]) ? 0 : Convert.ToSingle(reader["alertUpLimit"]);
                item.evalDownLimit = Convert.IsDBNull(reader["evalDownLimit"]) ? 0 : Convert.ToSingle(reader["evalDownLimit"]);
                item.evalUpLimit = Convert.IsDBNull(reader["evalUpLimit"]) ? 0 : Convert.ToSingle(reader["evalUpLimit"]);

                item.OutExcel = Convert.IsDBNull(reader["outExcel"]) ? enumOutExcelMode.None : (enumOutExcelMode )Convert.ToInt32(reader["outExcel"]);

                item.descript = Convert.IsDBNull(reader["descript"]) ? "" : Convert.ToString(reader["descript"]);
                item.subItemName = Convert.IsDBNull(reader["subItemName"]) ? "" : Convert.ToString(reader["subItemName"]);
                item.isSystem = reader["isSystem"] == DBNull.Value ? true : Convert.ToBoolean(reader["isSystem"]);                              
            }
            return lst;
        }
    }
    /// <summary>
    /// 表类别存取
    /// </summary>
    public class OilTableTypeAccess : abstractDB<OilTableTypeEntity>
    {
        public OilTableTypeAccess()
        {
            this.tableName = "OilTableType";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(OilTableTypeEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("tableName", item.tableName);
            dic.Add("dataStoreTable", item.dataStoreTable);
            dic.Add("tableOrder", item.tableOrder.ToString());
            dic.Add("descript", item.descript);
            dic.Add("libraryA", item.libraryA.ToString());
            dic.Add("libraryB", item.libraryB.ToString());
            dic.Add("libraryC", item.libraryC.ToString());
            dic.Add("itemNameShow", item.itemNameShow.ToString());
            dic.Add("itemEnShow", item.itemEnShow.ToString());
            dic.Add("itemUnitShow", item.itemUnitShow.ToString());
            dic.Add("itemCodeShow", item.itemCodeShow.ToString());

            return dic;
        }

        protected override List<OilTableTypeEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilTableTypeEntity>();
            while (reader.Read())
            {
                var item = new OilTableTypeEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    tableName = Convert.ToString(reader["tableName"]),
                    dataStoreTable = Convert.ToString(reader["dataStoreTable"]),
                    tableOrder = Convert.ToInt32(reader["tableOrder"]),
                    descript = Convert.ToString(reader["descript"]),
                    libraryA = Convert.ToBoolean(reader["libraryA"]),
                    libraryB = Convert.ToBoolean(reader["libraryB"]),
                    libraryC = Convert.ToBoolean(reader["libraryC"]),
                    itemNameShow = Convert.ToBoolean(reader["itemNameShow"]),
                    itemEnShow = Convert.ToBoolean(reader["itemEnShow"]),
                    itemUnitShow = Convert.ToBoolean(reader["itemUnitShow"]),
                    itemCodeShow = Convert.ToBoolean(reader["itemCodeShow"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    /// <summary>
    /// 原油表类型的转换对比表
    /// </summary>
    public class OilTableTypeComparisonTableAccess : abstractDB<OilTableTypeComparisonTableEntity>
    {
        public OilTableTypeComparisonTableAccess()
        {
            this.tableName = "OilTableTypeComparisonTable";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(OilTableTypeComparisonTableEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("tableName", item.tableName);
            dic.Add("oilTableTypeID", item.descript);
            dic.Add("belongToLevelTable", item.belongToLevelTable.ToString());
            dic.Add("belongToTargedValueTable", item.belongToTargedValueTable.ToString());
            dic.Add("belongToRangeTable", item.belongToRangeTable.ToString());
            dic.Add("descript", item.descript);
            return dic;
        }

        protected override List<OilTableTypeComparisonTableEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilTableTypeComparisonTableEntity>();
            while (reader.Read())
            {
                var item = new OilTableTypeComparisonTableEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    tableName = Convert.ToString(reader["tableName"]),
                    oilTableTypeID = Convert.ToInt32(reader["oilTableTypeID"]),
                    belongToLevelTable = Convert.ToBoolean(reader["belongToLevelTable"]),
                    belongToTargedValueTable = Convert.ToBoolean(reader["belongToTargedValueTable"]),
                    belongToRangeTable = Convert.ToBoolean(reader["belongToRangeTable"]),
                    descript = Convert.ToString(reader["descript"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    /// <summary>
    /// 经验审查中趋势审查表的上下限数据范围
    /// </summary>
    public class RangeParmTableAccess : abstractDB<RangeParmTableEntity>
    {
        public RangeParmTableAccess()
        {
            this.tableName = "TrendParmTable";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(RangeParmTableEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.ID.ToString());
            dic.Add("OilTableTypeComparisonTableID", item.OilTableTypeComparisonTableID.ToString());
            dic.Add("itemCode", item.itemCode);
            dic.Add("alertDownLimit", item.alertDownLimit.ToString());
            dic.Add("alertUpLimit", item.alertUpLimit.ToString());
            dic.Add("descript", item.descript);

            return dic;
        }

        protected override List<RangeParmTableEntity> dataReaderToEntity(IDataReader reader)
        {
            List<RangeParmTableEntity> lst = new List<RangeParmTableEntity>();
            while (reader.Read())
            {
                RangeParmTableEntity item = new RangeParmTableEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    OilTableTypeComparisonTableID = Convert.ToInt32(reader["OilTableTypeComparisonTableID"]),
                    itemCode = Convert.ToString(reader["itemCode"]),
                    alertDownLimit = Convert.IsDBNull(reader["alertDownLimit"]) ? null : Convert.ToString(reader["alertDownLimit"]),
                    alertUpLimit = Convert.IsDBNull(reader["alertUpLimit"]) ? null : Convert.ToString(reader["alertUpLimit"]),
                    descript = Convert.IsDBNull(reader["descript"]) ? "" : Convert.ToString(reader["descript"]),
                };
                lst.Add(item);
            }
            return lst;
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="accountParmTableEntity"></param>
        /// <param name="ID"></param>
        /// <returns> -1修改不成功 ， 0没修改，1修改成功</returns>
        public override int Update(RangeParmTableEntity rangeParmTableEntity, string ID)
        {
            int returnValue = 0;
            if (rangeParmTableEntity == null || ID == null)
                return returnValue;

            SqlCommand cmd = new SqlCommand();
            try
            {               
                cmd.CommandText = "UPDATE TrendParmTable SET alertDownLimit =@alertDownLimit , alertUpLimit =@alertUpLimit WHERE ID = @ID";
                cmd.Connection = new SqlConnection(SqlHelper.connectionString);
                cmd.Parameters.Add("@alertDownLimit", SqlDbType.VarChar).Value = rangeParmTableEntity.alertDownLimit == null ? DBNull.Value.ToString() : rangeParmTableEntity.alertDownLimit;
                cmd.Parameters.Add("@alertUpLimit", SqlDbType.VarChar).Value = rangeParmTableEntity.alertUpLimit == null ? DBNull.Value.ToString() : rangeParmTableEntity.alertUpLimit; ;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();            
            }
            catch(Exception ex)
            {
                Log.Error("TrendParmTable表格修改失败!\n\r" + ex.ToString());
                return returnValue =-1;
            }
            finally
            {
                cmd.Connection.Close();              
            }
            return returnValue = 1;
        }
    }




    /// <summary>
    /// 核算审查 
    /// </summary>
    public class AccountParmTableAccess : abstractDB<AccountParmTableEntity>
    {
        public AccountParmTableAccess()
        {
            this.tableName = "AccountParmTable";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(AccountParmTableEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.ID.ToString());
            dic.Add("itemCode", item.itemCode);
            dic.Add("itemName", item.itemName);

            return dic;
        }

        protected override List<AccountParmTableEntity> dataReaderToEntity(IDataReader reader)
        {
            List<AccountParmTableEntity> lst = new List<AccountParmTableEntity>();
            while (reader.Read())
            {
                AccountParmTableEntity item = new AccountParmTableEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    itemCode = Convert.ToString(reader["itemCode"]),
                    itemName = Convert.ToString(reader["itemName"])
                };
                lst.Add(item);
            }
            return lst;
        }
           }

    /// <summary>
    /// 水平值表数据存储
    /// </summary>
    public class LevelValueAccess : abstractDB<LevelValueEntity>
    {
        public LevelValueAccess()
        {
            this.tableName = "LevelValue";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(LevelValueEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.ID.ToString());
            dic.Add("OilTableTypeComparisonTableID", item.OilTableTypeComparisonTableID.ToString());
            dic.Add("itemCode", item.itemCode);
            dic.Add("itemName", item.itemName);
            dic.Add("<Less", item.belowLess);
            dic.Add("Less", item.Less.ToString());
            dic.Add("Less-More", item.More_Less);
            dic.Add("More", item.More.ToString());
            dic.Add(">More", item.aboveMore);

            return dic;
        }

        protected override List<LevelValueEntity> dataReaderToEntity(IDataReader reader)
        {
            List<LevelValueEntity> lst = new List<LevelValueEntity>();
            while (reader.Read())
            {
                LevelValueEntity item = new LevelValueEntity();
                lst.Add(item);
                item.ID = Convert.ToInt32(reader["ID"]);
                item.OilTableTypeComparisonTableID = Convert.ToInt32(reader["OilTableTypeComparisonTableID"]);
                item.itemCode = Convert.ToString(reader["itemCode"]);
                item.itemName = Convert.ToString(reader["itemName"]);
                item.belowLess = Convert.ToString(reader["<Less"]);
                item.strLess = Convert.ToString(reader["Less"]);
                item.strMore = Convert.ToString(reader["More"]);              
                item.More_Less = Convert.ToString(reader["Less-More"]);                
                item.aboveMore = Convert.ToString(reader[">More"]);
                #region"More"
                if (Convert.IsDBNull(reader["More"]) || string.IsNullOrWhiteSpace(reader["More"].ToString()))
                    item.More = null;
                else
                {
                    float temp;
                    if (float.TryParse(reader["More"].ToString(), out temp))
                        item.More = temp;
                    else
                        item.More = null;
                }
                #endregion

                #region"Less"
                if (Convert.IsDBNull(reader["Less"]) || string.IsNullOrWhiteSpace(reader["Less"].ToString()))
                    item.Less = null;
                else
                {
                    float temp;
                    if (float.TryParse(reader["Less"].ToString(), out temp))
                        item.Less = temp;
                    else
                        item.Less = null;
                }
                #endregion 
            }
            return lst;
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="accountParmTableEntity"></param>
        /// <param name="ID"></param>
        /// <returns> -1修改不成功 ， 0没修改，1修改成功</returns>
        public override int Update(LevelValueEntity levelValue, string ID)
        {
            int returnValue = 0;
            if (levelValue == null || ID == null)
                return returnValue;

            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.CommandText = "UPDATE LevelValue SET Less =@Less, More =@More WHERE ID = @ID";
                cmd.Connection = new SqlConnection(SqlHelper.connectionString);
                cmd.Parameters.Add("@Less", SqlDbType.VarChar).Value = levelValue.strLess == string.Empty ? DBNull.Value : levelValue.strLess as object;//levelValue.strLess == null ? DBNull.Value : levelValue.Less as object;
                cmd.Parameters.Add("@More", SqlDbType.VarChar).Value = levelValue.strMore == string.Empty ? DBNull.Value : levelValue.strMore as object;// levelValue.strMore == null ? DBNull.Value : levelValue.More as object;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error("LevelValue表格修改失败!\n\r" + ex.ToString());
                return returnValue = -1;
            }
            finally
            {
                cmd.Connection.Close();
            }
            return returnValue = 1;
        }
    }
    /// <summary>
    /// 指标值表的列存储
    /// </summary>
    public class TargetedValueColEntityAccess : abstractDB<TargetedValueColEntity>
    {
        public TargetedValueColEntityAccess()
        {
            this.tableName = "TargetedValueCol";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(TargetedValueColEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.ID.ToString());
            dic.Add("OilTableTypeComparisonTableID", item.OilTableTypeComparisonTableID.ToString());
            dic.Add("colName", item.colName);
            dic.Add("colCode", item.colCode);

            return dic;
        }

        protected override List<TargetedValueColEntity> dataReaderToEntity(IDataReader reader)
        {
            List<TargetedValueColEntity> lst = new List<TargetedValueColEntity>();
            while (reader.Read())
            {
                TargetedValueColEntity item = new TargetedValueColEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    OilTableTypeComparisonTableID = Convert.ToInt32(reader["OilTableTypeComparisonTableID"]),
                    colName = Convert.ToString(reader["colName"]),                  
                    colCode = Convert.ToString(reader["colCode"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    /// <summary>
    /// 指标值表的行存储
    /// </summary>
    public class TargetedValueRowEntityAccess : abstractDB<TargetedValueRowEntity>
    {
        public TargetedValueRowEntityAccess()
        {
            this.tableName = "TargetedValueRow";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(TargetedValueRowEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.ID.ToString());
            dic.Add("OilTableTypeComparisonTableID", item.OilTableTypeComparisonTableID.ToString());
            dic.Add("itemName", item.itemName);
            dic.Add("itemCode", item.itemCode);
            dic.Add("unit",item.unit);
            dic.Add("descript", item.descript);

            return dic;
        }

        protected override List<TargetedValueRowEntity> dataReaderToEntity(IDataReader reader)
        {
            List<TargetedValueRowEntity> lst = new List<TargetedValueRowEntity>();
            while (reader.Read())
            {
                TargetedValueRowEntity item = new TargetedValueRowEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    OilTableTypeComparisonTableID = Convert.ToInt32(reader["OilTableTypeComparisonTableID"]),
                    itemName = reader["itemName"] == null ? string.Empty : Convert.ToString(reader["itemName"]),  
                    itemCode = reader["itemCode"] == null ? string.Empty : Convert.ToString(reader["itemCode"]),                   
                    unit = reader["unit"] == null ? string.Empty : Convert.ToString(reader["unit"]),
                    descript = reader["descript"] == null ? string.Empty : Convert.ToString(reader["descript"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    /// <summary>
    /// 指标值表的值存储
    /// </summary>
    public class TargetedValueEntityAccess : abstractDB<TargetedValueEntity>
    {
        public TargetedValueEntityAccess()
        {
            this.tableName = "TargetedValue";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(TargetedValueEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.ID.ToString());
            dic.Add("S_UserID", item.S_UserID.ToString());
            dic.Add("OilTableTypeComparisonTableID", item.OilTableTypeComparisonTableID.ToString());
            dic.Add("TargetedValueColID", item.TargetedValueColID.ToString());
            dic.Add("TargetedValueRowID", item.TargetedValueRowID.ToString());
            dic.Add("Value", item.fValue.ToString());
            return dic;
        }

        protected override List<TargetedValueEntity> dataReaderToEntity(IDataReader reader)
        {
            List<TargetedValueEntity> lst = new List<TargetedValueEntity>();
            while (reader.Read())
            {
                TargetedValueEntity item = new TargetedValueEntity();
                lst.Add(item);
                item.ID = Convert.ToInt32(reader["ID"]);
                item.S_UserID =  reader["S_UserID"] == DBNull.Value ? 0: Convert.ToInt32(reader["S_UserID"].ToString());
                item.OilTableTypeComparisonTableID = Convert.ToInt32(reader["OilTableTypeComparisonTableID"]);
                item.TargetedValueColID = Convert.ToInt32(reader["TargetedValueColID"]);
                item.TargetedValueRowID = Convert.ToInt32(reader["TargetedValueRowID"]);
                item.strValue = Convert.ToString(reader["Value"]);
                #region"Value"
                if (Convert.IsDBNull(reader["Value"]) || string.IsNullOrWhiteSpace(reader["Value"].ToString()))
                    item.fValue = null;
                else
                {
                    float temp;
                    if (float.TryParse(reader["Value"].ToString(), out temp))
                        item.fValue = temp;
                    else
                        item.fValue = null;
                }
                #endregion                        
            }
            return lst;
        }    
    }

    /// <summary>
    /// 用户表
    /// </summary>
    public class S_UserAccess : abstractDB<S_UserEntity>
    {
        public S_UserAccess()
        {
            this.tableName = "S_User";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(S_UserEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("loginName", item.loginName);
            dic.Add("password", item.password);
            dic.Add("realName", item.realName);
            dic.Add("sex", item.sex.ToString());
            dic.Add("tel", item.tel);
            dic.Add("email", item.email);
            dic.Add("addTime", item.addTime.ToString());
            dic.Add("role", item.role);
            return dic;
        }

        protected override List<S_UserEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<S_UserEntity>();
            while (reader.Read())
            {
                var item = new S_UserEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    loginName = Convert.ToString(reader["loginName"]),
                    password = Convert.ToString(reader["password"]),
                    realName = Convert.ToString(reader["realName"]),
                    sex = Convert.ToBoolean(reader["sex"]),
                    tel = Convert.ToString(reader["tel"]),
                    email = Convert.ToString(reader["email"]),
                    addTime = Convert.ToDateTime(reader["addTime"]),
                    role = Convert.ToString(reader["role"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    /// <summary>
    /// Cru文件的编码映射
    /// </summary>
    public class CruCodeMapAccess : abstractDB<CruCodeMapEntity>
    {
        public CruCodeMapAccess()
        {
            this.tableName = "CruCodeMap";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(CruCodeMapEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("oilTableTypeID", item.oilTableTypeID.ToString());
            dic.Add("cruCode", item.cruCode);
            dic.Add("cParam", item.cParam.ToString());
            dic.Add("itemCode", item.itemCode);
            return dic;
        }

        protected override List<CruCodeMapEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<CruCodeMapEntity>();
            while (reader.Read())
            {
                var item = new CruCodeMapEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    oilTableTypeID = Convert.ToInt32(reader["oilTableTypeID"]),
                    cruCode = Convert.ToString(reader["cruCode"]),
                    cParam = DBNull.Value == reader["cParam"] ? -1 : Convert.ToSingle(reader["cParam"]),
                    itemCode = Convert.ToString(reader["itemCode"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class WCutTypeAccess : abstractDB<WCutTypeEntity>
    {
        public WCutTypeAccess()
        {
            this.tableName = "WCutType";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(WCutTypeEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("code", item.code.ToString());
            dic.Add("name", item.name);
            return dic;
        }

        protected override List<WCutTypeEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<WCutTypeEntity>();
            while (reader.Read())
            {
                var item = new WCutTypeEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    code = Convert.ToInt32(reader["code"]),
                    name = Convert.ToString(reader["name"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class CurveDataAccess : abstractDB<CurveDataEntity>
    {
        public CurveDataAccess()
        {
            this.tableName = "CurveData";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(CurveDataEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("curveID", item.curveID.ToString());
            dic.Add("xValue", item.xValue.ToString());
            dic.Add("yValue", item.yValue.ToString());
            return dic;
        }

        protected override List<CurveDataEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<CurveDataEntity>();
            while (reader.Read())
            {
                var item = new CurveDataEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    curveID = Convert.ToInt32(reader["curveID"]),
                    xValue = Convert.ToSingle(reader["xValue"]),
                    yValue = Convert.ToSingle(reader["yValue"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class CurveAccess : abstractDB<CurveEntity>
    {
        public CurveAccess()
        {
            this.tableName = "Curve";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(CurveEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("oilInfoID", item.oilInfoID.ToString());
            dic.Add("curveTypeID", item.curveTypeID.ToString());
            dic.Add("propertyX", item.propertyX.ToString());
            dic.Add("propertyY", item.propertyY.ToString());
            dic.Add("unit", item.unit.ToString());
            dic.Add("decNumber", item.decNumber.ToString());
            dic.Add("descript", item.descript);
            return dic;
        }

        protected override List<CurveEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<CurveEntity>();
            while (reader.Read())
            {
                var item = new CurveEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    oilInfoID = Convert.ToInt32(reader["oilInfoID"]),
                    curveTypeID = Convert.ToInt32(reader["curveTypeID"]),
                    propertyX = Convert.ToString(reader["propertyX"]),
                    propertyY = Convert.ToString(reader["propertyY"]),
                    unit = Convert.ToString(reader["unit"]),
                    decNumber = Convert.ToInt32(reader["decNumber"]),
                    descript = Convert.ToString(reader["descript"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class CurveSubTypeAccess : abstractDB<CurveSubTypeEntity>
    {
        public CurveSubTypeAccess()
        {
            this.tableName = "CurveSubType";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(CurveSubTypeEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("typeCode", item.typeCode.ToString());
            dic.Add("propertyX", item.propertyX.ToString());
            dic.Add("propertyY", item.propertyY.ToString());
            dic.Add("splineLine", item.splineLine.ToString());
            dic.Add("descript", item.descript.ToString());
            return dic;
        }

        protected override List<CurveSubTypeEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<CurveSubTypeEntity>();
            while (reader.Read())
            {
                var item = new CurveSubTypeEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    typeCode = Convert.ToString(reader["typeCode"]),
                    propertyX = Convert.ToString(reader["propertyX"]),
                    propertyY = Convert.ToString(reader["propertyY"]),
                    splineLine = reader["splineLine"] == DBNull.Value ? 0 : Convert.ToInt32(reader["splineLine"].ToString()),
                    descript = Convert.ToString(reader["descript"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class CurveTypeAccess : abstractDB<CurveTypeEntity>
    {
        public CurveTypeAccess()
        {
            this.tableName = "CurveType";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(CurveTypeEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("typeName", item.typeName.ToString());
            dic.Add("typeCode", item.typeCode.ToString());
            dic.Add("descript", item.descript.ToString());
            return dic;
        }

        protected override List<CurveTypeEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<CurveTypeEntity>();
            while (reader.Read())
            {
                var item = new CurveTypeEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    typeName = Convert.ToString(reader["typeName"]),
                    typeCode = Convert.ToString(reader["typeCode"]),
                    descript = Convert.ToString(reader["descript"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    /// <summary>
    /// CurveParmType表的底层访问代码
    /// </summary>
    public class CurveParmTypeAccess : abstractDB<CurveParmTypeEntity>
    {
        #region "构造函数"
        public CurveParmTypeAccess()
        {
            this.tableName = "CurveParmType";//访问表的名字
            this.keyField = "Id";//表的主键名称
        }
        #endregion

        /// <summary>
        /// 通过CurveParmTypeEntity来获取属性字典
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(CurveParmTypeEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ID", item.Id.ToString());
            dic.Add("OilTableTypeID", item.OilTableTypeID.ToString());
            dic.Add("TypeCode", item.TypeCode.ToString());
            dic.Add("ItemCode", item.ItemCode.ToString());
            dic.Add("IsX", item.IsX.ToString());
            dic.Add("Show", item.Show.ToString());
            dic.Add("SaveB", item.SaveB.ToString());
            dic.Add("GCCal", item.GCCal.ToString());
            dic.Add("Descript", item.Descript.ToString());

            return dic;
        }

        /// <summary>
        /// 从IDataReader中提出出CurveParmTypeEntity的集合
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<CurveParmTypeEntity> dataReaderToEntity(IDataReader reader)
        {
            var list = new List<CurveParmTypeEntity>();
            while (reader.Read())
            {
                var item = new CurveParmTypeEntity()
                {
                    Id = Convert.ToInt32(reader["ID"]),
                    OilTableTypeID = Convert.ToInt32(reader["oilTableTypeID"]),
                    TypeCode = Convert.ToString(reader["typeCode"]),
                    ItemCode = Convert.ToString(reader["itemCode"]),
                    IsX = Convert.ToInt32(reader["isX"]),
                    Show = Convert.ToInt32(reader["show"]),
                    SaveB = Convert.ToInt32(reader["saveB"]),
                    GCCal = Convert.ToInt32(reader["GCCal"]),
                    Descript = Convert.ToString(reader["descript"])
                };
                list.Add(item);
            }

            return list;
        }
    }

    public class LightCurveParmTableAccess : abstractDB<LightCurveParmTableEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public LightCurveParmTableAccess()
        {
            this.tableName = "LightCurveParmTable";         //表名称
            this.keyField = "ID";                                         //表主键
        }
        /// <summary>
        /// 返回数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(LightCurveParmTableEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("ItemCode", item.ItemCode);
            dic.Add("Tb", item.Tb);
            dic.Add("D20", item.Tb);
            dic.Add("SG15", item.Tb);
            return dic;
        }
        /// <summary>
        /// 从数据库中读取数据
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<LightCurveParmTableEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<LightCurveParmTableEntity>();
            while (reader.Read())
            {
                var item = new LightCurveParmTableEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    ItemCode = reader["itemCode"] == DBNull.Value ? "" : reader["itemCode"].ToString(),
                    Tb = reader["Tb"] == DBNull.Value ? "" : reader["Tb"].ToString(),
                    D20 = reader["D20"] == DBNull.Value ? "" : reader["D20"].ToString(),
                    SG15 = reader["SG15"] == DBNull.Value ? "" : reader["SG15"].ToString()
                };
                lst.Add(item);
            }
            return lst;
        }
    }  

    public class GCMatch1Access : abstractDB<GCMatch1Entity>
    {
        public GCMatch1Access()
        {
            this.tableName = "GCMatch1";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(GCMatch1Entity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("itemName", item.itemName.ToString());
            dic.Add("itemEnName", item.itemEnName.ToString());
            dic.Add("itemValue", item.itemValue.ToString());
            dic.Add("itemCode", item.itemCode.ToString());
            dic.Add("descript", item.descript.ToString());

            return dic;
        }

        protected override List<GCMatch1Entity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<GCMatch1Entity>();
            while (reader.Read())
            {
                var item = new GCMatch1Entity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    itemName = reader["itemName"].ToString(),
                    itemEnName = reader["itemEnName"].ToString(),
                    itemValue = Convert.ToSingle(reader["itemValue"]),
                    itemCode = reader["itemCode"].ToString(),
                    descript = reader["descript"].ToString(),
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class GCMatch2Access : abstractDB<GCMatch2Entity>
    {
        public GCMatch2Access()
        {
            this.tableName = "GCMatch2";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(GCMatch2Entity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("itemCode", item.itemCode.ToString());
            dic.Add("descript", item.descript.ToString());
            dic.Add("colIntC", item.colIntC.ToString());
            dic.Add("colStrD", item.colStrD.ToString());
            dic.Add("colFloatE", item.colFloatE.ToString());
            dic.Add("colFloatF", item.colFloatF.ToString());
            dic.Add("colIntG", item.colIntG.ToString());
            dic.Add("colFloatH", item.colFloatH.ToString());
            dic.Add("colFloatI", item.colFloatI.ToString());
            dic.Add("colFloatJ", item.colFloatJ.ToString());
            dic.Add("colFloatK", item.colFloatK.ToString());
            dic.Add("colIntL", item.colIntL.ToString());
            dic.Add("colIntM", item.colIntM.ToString());
            dic.Add("colFloatN", item.colFloatN.ToString());
            return dic;
        }

        protected override List<GCMatch2Entity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<GCMatch2Entity>();
            while (reader.Read())
            {
                var item = new GCMatch2Entity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    itemCode = reader["itemCode"].ToString(),
                    descript = reader["descript"].ToString(),
                    colIntC = Convert.ToInt32(reader["colIntC"]),
                    colStrD = reader["colStrD"].ToString(),
                    colFloatE = Convert.ToSingle(reader["colFloatE"]),
                    colFloatF = Convert.ToSingle(reader["colFloatF"]),
                    colIntG = Convert.ToInt32(reader["colIntG"]),
                    colFloatH = Convert.ToSingle(reader["colFloatH"]),
                    colFloatI = Convert.ToSingle(reader["colFloatI"]),
                    colFloatJ = Convert.ToSingle(reader["colFloatJ"]),
                    colFloatK = Convert.ToSingle(reader["colFloatK"]),
                    colIntL = Convert.ToInt32(reader["colIntL"]),
                    colIntM = Convert.ToInt32(reader["colIntM"]),
                    colFloatN = Convert.ToSingle(reader["colFloatN"]),
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class S_ParmTypeAccess : abstractDB<S_ParmTypeEntity>
    {
        public S_ParmTypeAccess()
        {
            this.tableName = "S_ParmType";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(S_ParmTypeEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("parmTypeName", item.parmTypeName.ToString());
            dic.Add("code", item.parmTypeName.ToString());
            dic.Add("isSystem", item.isSystem.ToString());
            dic.Add("descript", item.descript.ToString());

            return dic;
        }

        protected override List<S_ParmTypeEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<S_ParmTypeEntity>();
            while (reader.Read())
            {
                var item = new S_ParmTypeEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    parmTypeName = reader["parmTypeName"].ToString(),
                    code = reader["code"].ToString(),
                    isSystem = Convert.ToBoolean(reader["isSystem"].ToString()),
                    descript = reader["descript"] == DBNull.Value ? "" : reader["descript"].ToString()
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class S_ParmAccess : abstractDB<S_ParmEntity>
    {
        public S_ParmAccess()
        {
            this.tableName = "S_Parm";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(S_ParmEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("parmTypeID", item.parmTypeID.ToString());
            dic.Add("parmName", item.parmName.ToString());
            dic.Add("parmValue", item.parmValue.ToString());
            dic.Add("parmOrder", item.parmOrder.ToString());

            return dic;
        }

        protected override List<S_ParmEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<S_ParmEntity>();
            while (reader.Read())
            {
                var item = new S_ParmEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    parmTypeID = Convert.ToInt32(reader["parmTypeID"]),
                    parmName = reader["parmName"].ToString(),
                    parmValue = reader["parmValue"].ToString(),
                    parmOrder = Convert.ToInt32(reader["parmOrder"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class S_MoudleAccess : abstractDB<S_MoudleEntity>
    {
        public S_MoudleAccess()
        {
            this.tableName = "S_Moudle";
            this.keyField = "ID";
        }
        protected override Dictionary<string, string> getProperty(S_MoudleEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("text", item.text.ToString());
            dic.Add("pID", item.pID.ToString());
            dic.Add("name", item.name.ToString());
            dic.Add("role1", item.role1.ToString());
            dic.Add("role2", item.role2.ToString());
            return dic;
        }

        protected override List<S_MoudleEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<S_MoudleEntity>();
            while (reader.Read())
            {
                var item = new S_MoudleEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    text = reader["text"].ToString(),
                    pID = Convert.ToInt32(reader["pID"]),
                    name = reader["name"].ToString(),
                    role1 = Convert.ToBoolean(reader["role1"].ToString()),
                    role2 = Convert.ToBoolean(reader["role2"].ToString())
                };
                lst.Add(item);
            }
            return lst;
        }
    }

    public class LinkSupplementParmAccess : abstractDB<LinkSupplementParmEntity>
    {
        #region "构造函数"
        public LinkSupplementParmAccess()
        {
            this.tableName = "LinkSupplementParm";
            this.keyField = "ID";
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(LinkSupplementParmEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("a", item.a.ToString());
            dic.Add("b", item.b.ToString());
            dic.Add("c", item.c.ToString());


            return dic;
        }

        protected override List<LinkSupplementParmEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<LinkSupplementParmEntity>();
            while (reader.Read())
            {
                var item = new LinkSupplementParmEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    a = Convert.ToSingle(reader["a"]),
                    b = Convert.ToSingle(reader["b"]),
                    c = Convert.ToSingle(reader["c"])
                };
                lst.Add(item);
            }
            return lst;
        }
    }
    /// <summary>
    /// OilDataSearchCol从数据库中访问数据的底层代码
    /// </summary>
    public class OilDataSearchColAccess : abstractDB<OilDataSearchColEntity>
    {
        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public OilDataSearchColAccess()
        {
            this.tableName = "OilDataSearchCol";
            this.keyField = "Id";
        }
        #endregion
        /// <summary>
        /// 通过OilDataColEntity来获得实体属性字典
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(OilDataSearchColEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("itemOrder", item.itemOrder.ToString());
            dic.Add("oilTableName", item.OilTableName);
            dic.Add("oilTableColID", item.OilTableColID.ToString());
            dic.Add("BelongsToRan", item.BelongsToRan.ToString());
            dic.Add("BelongsToSim", item.BelongsToSim.ToString());
            dic.Add("ICP", item.ICP.ToString());
            dic.Add("ECP", item.ECP.ToString());
            return dic;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<OilDataSearchColEntity> dataReaderToEntity(IDataReader reader)
        {
            var list = new List<OilDataSearchColEntity>();
            while (reader.Read())
            {
                var item = new OilDataSearchColEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    itemOrder = reader["itemOrder"] != DBNull.Value ? Convert.ToInt32(reader["itemOrder"]) : 0,
                    OilTableName = reader["oilTableName"].ToString(),
                    OilTableColID = Convert.ToInt32(reader["oilTableColID"]),
                    BelongsToRan = Convert.ToBoolean(reader["BelongsToRan"]),
                    BelongsToSim = Convert.ToBoolean(reader["BelongsToSim"]),
                    ICP = reader["ICP"] != DBNull.Value ? Convert.ToSingle(reader["ICP"]) : 0,
                    ECP = reader["ECP"] != DBNull.Value ? Convert.ToSingle(reader["ECP"]) : 0
                };
                list.Add(item);
            }
            return list;
        }
    }
    /// <summary>
    /// OilDataSearchRow访问数据库底层代码
    /// </summary>
    public class OilDataSearchRowAccess : abstractDB<OilDataSearchRowEntity>
    {
        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public OilDataSearchRowAccess()
        {
            this.tableName = "OilDataSearchRow";//访问表的名字OilDataRow
            this.keyField = "Id";//表的主键名称
        }
        #endregion

        /// <summary>
        /// 通过OilDataRowEntity来获得实体属性字典
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(OilDataSearchRowEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Id", item.ID.ToString());
            dic.Add("OilTableRowID", item.OilTableRowID.ToString());
            dic.Add("OilDataColID", item.OilDataColID.ToString());
            dic.Add("BelongsToA", item.BelongsToA.ToString());
            dic.Add("BelongsToB", item.BelongsToB.ToString());
            dic.Add("BelongsToApp", item.BelongsToApp.ToString());
            return dic;
        }
        /// <summary>
        /// 根据IDataReader从中提取数据返回OilDataRowEntity实体集合
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<OilDataSearchRowEntity> dataReaderToEntity(IDataReader reader)
        {
            var list = new List<OilDataSearchRowEntity>();
            while (reader.Read())
            {
                var item = new OilDataSearchRowEntity()
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    OilTableRowID = Convert.ToInt32(reader["OilTableRowID"]),
                    OilDataColID = Convert.ToInt32(reader["OilDataColID"]),
                    BelongsToA = Convert.ToBoolean(reader["BelongsToA"]),
                    BelongsToB = Convert.ToBoolean(reader["BelongsToB"]),
                    BelongsToApp = Convert.ToBoolean(reader["BelongsToApp"])
                };
                list.Add(item);
            }
            return list;
        }
    }
    /// <summary>
    /// 密封快速查找的数据表OilDataSearchAccess的类
    /// </summary>
    public sealed class OilDataSearchAccess : abstractDB<OilDataSearchEntity>
    {
        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public OilDataSearchAccess()
        {
            this.tableName = "OilDataSearch";
            this.keyField = "ID";
        }
        #endregion

        /// <summary>
        /// 通过OilDataSearchEntity来获得实体属性字典
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(OilDataSearchEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("oilInfoID", item.oilInfoID.ToString());
            dic.Add("oilTableColID", item.oilTableColID.ToString());
            dic.Add("oilTableRowID", item.oilTableRowID.ToString());
            dic.Add("calData", item.calData);
            dic.Add("labData", item.labData);
            return dic;
        }
        /// <summary>
        /// 根据IDataReader从中提取数据返回OilDataSearchEntity实体集合
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<OilDataSearchEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<OilDataSearchEntity>();
            while (reader.Read())
            {
                var item = new OilDataSearchEntity()
                {
                    //calData = RIPP.Lib.Security.SecurityTool.MyDecrypt(reader["calData"].ToString()),
                    calData = reader["calData"].ToString(),
                    labData = reader["labData"].ToString(),
                    oilInfoID = Convert.ToInt32(reader["oilInfoID"]),
                    oilTableColID = Convert.ToInt32(reader["oilTableColID"]),
                    oilTableRowID = Convert.ToInt32(reader["oilTableRowID"]),
                    ID = Convert.ToInt32(reader["ID"])
                };
                lst.Add(item);
            }
            return lst;
        }
        /// <summary>
        /// 向数据库中插入一条OilDataSearchEntity数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override int Insert(OilDataSearchEntity item)
        {
            OilDataSearchEntity newItem = new OilDataSearchEntity();
            newItem.oilInfoID = item.oilInfoID;
            newItem.oilTableColID = item.oilTableColID;
            newItem.oilTableRowID = item.oilTableRowID;
            newItem.labData = item.labData;
            newItem.calData = item.calData;
            return base.Insert(newItem);
        }
        /// <summary>
        /// 向数据库中修改一条OilDataSearchEntity数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public override int Update(OilDataSearchEntity item, string keyValue)
        {
            OilDataSearchEntity newItem = new OilDataSearchEntity();

            newItem.oilInfoID = item.oilInfoID;
            newItem.oilTableColID = item.oilTableColID;
            newItem.oilTableRowID = item.oilTableRowID;
            newItem.labData = item.labData;
            newItem.calData = item.calData;
            //newItem.calData = RIPP.Lib.Security.SecurityTool.MyEncrypt(item.calData);
            return base.Update(newItem, keyValue);
        }
        /// <summary>
        /// 获取不重复的Id
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<int> getId(string sql)
        {
            List<int> idList = new List<int>();
            var reader = SqlHelper.ExecuteReader(SqlHelper.connectionString, CommandType.Text, sql, null);       
            while (reader.Read())
            {
                int ID = Convert.ToInt32(reader["oilInfoID"]);
                if (!idList.Contains(ID))
                    idList.Add(ID);
            }
            return idList;
        }
        /*更新数据前先删除原来表中的数据*/
        public int deleteData(string sql)
        {
            int i = SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, null);
            return i;
        }

        public override List<OilDataSearchEntity> Get(string sqlWhere, int Count = 0, string orderStr = null)
        {
            return base.Get(sqlWhere, Count, orderStr);
        }
    }
    /// <summary>
    /// 密封批注实体类
    /// </summary>
    public sealed class RemarkAccess : abstractDB<RemarkEntity>
    {
        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public RemarkAccess()
        {
            this.tableName = "Remark";
            this.keyField = "ID";
        }
        #endregion

        /// <summary>
        /// 通过OilDataSearchEntity来获得实体属性字典
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override Dictionary<string, string> getProperty(RemarkEntity item)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("ID", item.ID.ToString());
            dic.Add("oilInfoID", item.oilInfoID.ToString());
            dic.Add("oilTableColID", item.oilTableColID.ToString());
            dic.Add("oilTableRowID", item.oilTableRowID.ToString());
            dic.Add("calremark", item.CalRemark);
            dic.Add("labremark", item.LabRemark);
            return dic;
        }
        /// <summary>
        /// 根据IDataReader从中提取数据返回OilDataSearchEntity实体集合
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override List<RemarkEntity> dataReaderToEntity(IDataReader reader)
        {
            var lst = new List<RemarkEntity>();
            while (reader.Read())
            {
                RemarkEntity item = new RemarkEntity()
                {
                    LabRemark = reader["labremark"].ToString(),
                    CalRemark = reader["calremark"].ToString(),
                    oilInfoID = Convert.ToInt32(reader["oilInfoID"]),
                    oilTableColID = Convert.ToInt32(reader["oilTableColID"]),
                    oilTableRowID = Convert.ToInt32(reader["oilTableRowID"]),
                    ID = Convert.ToInt32(reader["ID"])
                };
                lst.Add(item);
            }
            return lst;
        }
        /// <summary>
        /// 向数据库中插入一条RemarkEntity数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override int Insert(RemarkEntity item)
        {
            RemarkEntity newItem = new RemarkEntity();
            newItem.oilInfoID = item.oilInfoID;
            newItem.oilTableColID = item.oilTableColID;
            newItem.oilTableRowID = item.oilTableRowID;
            newItem.LabRemark  = item.LabRemark;
            newItem.CalRemark = item.CalRemark;
            return base.Insert(newItem);
        }
        /// <summary>
        /// 向数据库中修改一条RemarkEntity数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public override int Update(RemarkEntity item, string keyValue)
        {
            RemarkEntity newItem = new RemarkEntity();

            newItem.oilInfoID = item.oilInfoID;
            newItem.oilTableColID = item.oilTableColID;
            newItem.oilTableRowID = item.oilTableRowID;
            newItem.LabRemark = item.LabRemark;
            newItem.CalRemark = item.CalRemark;
 
            return base.Update(newItem, keyValue);
        }
        /*获取不重复的Id*/
        public List<int> getId(string sql)
        {
            List<int> idList = new List<int>();
            var reader = SqlHelper.ExecuteReader(SqlHelper.connectionString, CommandType.Text, sql, null);
            //List<OilDataEntity> oilData = new List<OilDataEntity>();
            while (reader.Read())
            {
                idList.Add(Convert.ToInt32(reader["oilInfoID"]));
            }
            return idList;
        }
        /*更新数据前先删除原来表中的数据*/
        public int deleteData(string sql)
        {
            int i = SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, null);
            return i;
        }

        public List<RemarkEntity> getRemakList(string sql)
        {
            List<RemarkEntity> remarkEntityList = new List<RemarkEntity>();
            var reader = SqlHelper.ExecuteReader(SqlHelper.connectionString, CommandType.Text, sql, null);
            while (reader.Read())
            {
                RemarkEntity tempRemark = new RemarkEntity();
                tempRemark.CalRemark = reader["calremark"].ToString();
                tempRemark.LabRemark = reader["labremark"].ToString();
                tempRemark.oilInfoID = Convert.ToInt32(reader["oilInfoID"].ToString());
                tempRemark.oilTableColID = Convert.ToInt32(reader["oilTableColID"].ToString());
                tempRemark.oilTableRowID = Convert.ToInt32(reader["oilTableRowID"].ToString());
                tempRemark.OilTableRow = OilTableRowBll._OilTableRow.Where(o=>o.ID == tempRemark.oilTableRowID).FirstOrDefault();
                tempRemark.OilTableCol = OilTableColBll._OilTableCol.Where(o => o.ID == tempRemark.oilTableColID).FirstOrDefault();
                if (tempRemark.OilTableRow != null)
                tempRemark.TableName = tempRemark.OilTableRow.OilTableType.tableName;
                remarkEntityList.Add(tempRemark);
            }
            return remarkEntityList;
        }

        /// <summary>
        /// 根据原油ID和批注类型(实测Or校正)来获取批注实体
        /// </summary>
        /// <param name="oilInfoID"></param>
        /// <param name="markType"></param>
        /// <returns></returns>
        public List<RemarkEntity> getRemarkData(int oilInfoID)
        {
            List<RemarkEntity> result = new List<RemarkEntity>();

            string sql = string.Format("select * from Remark where oilInfoID='{0}'",oilInfoID);

            result = getRemakList(sql);

            return result;
        }
    }
    
    
    public sealed class oilName
    {
        public oilName()
        { }
        /*返回根据物性名称查询的数据*/
        public string GetOilName(IList<OilSimilarSearchEntity> _oilPropery)
        {
            if (0 == _oilPropery.Count)
            {
                return "";
            }
            /*获取查询ID*/
            OilTableRowAccess rowAccess = new OilTableRowAccess();
            for (int i = 0; i < _oilPropery.Count; i++)
            {
                IList<OilTableRowEntity> oilList = rowAccess.Get(" itemName='" + _oilPropery[i].ItemName + "'" + "and oilTableTypeID = 2");
                if (0 != oilList.Count)
                {
                    _oilPropery[i].OilTableRowID = oilList.First().ID;
                }
            }
            string value = "";
            List<string> sortList = new List<string>();

            string sqlWhere = "select distinct(oilInfoID) from OilDataB";
            //OilDataAccess acess = new OilDataAccess();

            OilDataBAccess acess = new OilDataBAccess();
            List<int> oilDatas = getDistinctId(sqlWhere);//获取OilData表中的oilId数据,不包含重复选项


            for (int i = 0; i < oilDatas.Count; i++)
            {
                string sql = " oilInfoId ='" + oilDatas[i].ToString() + "' ";
                float[] calDatas = new float[5];//最多5行数据，在传入之前已经限定过了
                double Sum = 0f;//
                int k = 0;

                foreach (var item in _oilPropery)
                {
                    string sqlData = sql + "and oilTableRowID='" + item.OilTableRowID.ToString() + "'";
                    List<OilDataBEntity> list = acess.Get(sqlData);
                    if (0 != list.Count)
                    {
                        calDatas[k] = float.Parse(list.Single().calData);//将临时的calData填入
                        Sum += ((float)(calDatas[k] - item.Fvalue) / item.Fvalue) * ((float)(calDatas[k] - item.Fvalue) / item.Fvalue);//计算公式
                        k++;
                    }
                    else
                    {
                        Sum = 0;//将数据清零
                        break;
                    }
                }
                if (0 != Sum)
                {
                    string crudeName = "";
                    OilInfoBAccess oilData = new OilInfoBAccess();
                    crudeName = oilData.Get("id=" + oilDatas[i]).First().crudeName;
                    sortList.Add(Sum.ToString() + "," + crudeName);
                }
            }
            sortList.Sort(new MyComparable());//对其进行自定义排序
            if (sortList.Count > 0)
            {
                value = sortList[0].Substring(sortList[0].IndexOf(",") + 1);
            }
            return value;
        }
        private List<int> getDistinctId(string sql)
        {
            List<int> idList = new List<int>();
            var reader = SqlHelper.ExecuteReader(SqlHelper.connectionString, CommandType.Text, sql, null);
            while (reader.Read())
            {
                idList.Add(Convert.ToInt32(reader["oilInfoID"]));
            }
            return idList;
        }
    }
}
