using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data.DataCheck;
using System.IO;
using System.Reflection;
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.OilDB.UI.GridOil;
using System.Windows.Forms;
using System.Threading;
using RIPP.Lib;
using System.Text.RegularExpressions;
using System.Collections;
using RIPP.OilDB.Model.Excel;
using RIPP.OilDB.BLL;

namespace RIPP.OilDB.Data
{
    public class OilBll
    {
        private static OilTableColBll _colCache = new OilTableColBll();
        private static OilTableRowBll _rowCache = new OilTableRowBll();
        private static OilTableTypeBll _tableCache = new OilTableTypeBll();
        private OilDataSearchBll _oilDataSearch = new OilDataSearchBll();
        /// <summary>
        /// 根据ID返回一条A库原油
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static OilInfoEntity GetOilById(int id)
        {
            return new OilInfoAccess().Get(id);
        }

        /// <summary>
        /// 根据CrudeIndex返回一条A库原油
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static OilInfoEntity GetOilInfoByCrudeIndex(string index)
        {
            return new OilInfoAccess().GetOilInfoByCrudex(index);
        }

        /// <summary>
        /// 根据ID返回一条B库原油
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static OilInfoBEntity GetOilBByID(int id)
        {
            return new OilInfoBAccess().Get(id);
        }

        /// <summary>
        /// 根据物性的itemCode在C库获取相应的行ID
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public static int GetOilTableRowIDFromOilTableRowByItemCode(string itemCode, EnumTableType tableType)
        {
            int result = -1;

            OilTableRowEntity row = OilTableRowBll._OilTableRow.Where(o => o.itemCode == itemCode && o.oilTableTypeID == (int)tableType).FirstOrDefault();
            if (row != null)
                result = row.ID;

            return result;
        }

        #region "查找原油"

        #region "相似查找"
        /// <summary>
        /// 相似查找返回原油ID列表
        /// </summary>
        /// <param name="oilProperty">相似查找的条件</param>
        /// <returns></returns>
        public IDictionary<string, double> GetOilSimInfoCrudeIndex(IList<OilSimilarSearchEntity> OilSimilarSearchEntityList)
        {
            return _oilDataSearch.GetOilSimInfoCrudeIndex(OilSimilarSearchEntityList);
        }
        #endregion

        /// <summary>
        /// 相似查找的所有原油信息的ID列表
        /// </summary>
        /// <returns></returns>
        public static List<int> OilDataSearchInfoBIDlist()
        {
            string sqlWhere = "select distinct(oilInfoID) from OilDataSearch";

            OilDataSearchAccess acess = new OilDataSearchAccess();
            List<int> InfoBIDlist = acess.getId(sqlWhere);//获取OilData表中的oilInfoID数据,不包含重复选项    

            return InfoBIDlist;
        }
        /// <summary>
        /// 根据原油编号查找一条原油
        /// </summary>
        /// <param name="crudeIndex">原油编号</param>
        /// <returns></returns>
        public static OilInfoEntity GetOilById(string crudeIndex)
        {
            OilInfoAccess access = new OilInfoAccess();
            OilInfoEntity oilInfo = access.Get("crudeIndex='" + crudeIndex + "'").FirstOrDefault();
            return oilInfo;
        }
        /// <summary>
        /// 根据原油编号查找一条原油B
        /// </summary>
        /// <param name="crudeIndex">原油编号</param>
        /// <returns></returns>
        public static OilInfoBEntity GetOilByCrudeIndex(string crudeIndex)
        {
            var t = new OilInfoBAccess().Get("crudeIndex='" + crudeIndex + "'").FirstOrDefault();
            if (t != null && t.curves != null && t.curves.Count > 0)
            {
                //家族全部原油曲线数据
                t.curves.Where(o => o.curveDatas != null).Count();
            }
            return t;
        }
        /// <summary>
        /// 根据原油属性查询找到满足条件的原油的ID
        /// </summary>
        /// <param name="cutPropertys">要查询的属性（包括范围）列表</param>
        /// <returns>key为原油数据中的InfoID,值为该ID满足条件的次数或null</returns>
        public Dictionary<int, int> GetInfoID(IList<OilRangeSearchEntity> cutPropertys)
        {
            return _oilDataSearch.GetInfoID(cutPropertys);
        }

        /// <summary>
        /// 从C库获取满足范围查询条件的原油编号
        /// </summary>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        public IDictionary<string, double> GetRangOilInfoCrudeIndex(IList<OilRangeSearchEntity> searchEntity)
        {
            return OilDataSearchBll.GetRangOilInfoCrudeIndex(searchEntity);
        }
        #endregion

        #region "保存原油"
        /// <summary>
        /// 保存一条原油
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码</returns>
        public static int save(OilInfoEntity info)
        {
            int infoID = saveInfo(info); //保存原油信息表，并返回ID
            info.ID = infoID;
            saveTables(info);
            return infoID;
        }

        /// <summary>
        /// 保存一条原油
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码</returns>
        public static int save(OilInfoBEntity info)
        {
            int infoID = saveInfo(info); //保存原油信息表，并返回ID
            info.ID = infoID;
            saveTables(info);
            return infoID;
        }
        /// <summary>
        /// C库的切割方案
        /// </summary>
        /// <returns></returns>
        private static List<CutMothedEntity> OilSearchCutMothed()
        {
            List<CutMothedEntity> cutMothedEntityList = new List<CutMothedEntity>();
            #region

            CutMothedEntity cutMethedEntity1 = new CutMothedEntity()
            {
                ICP = 15,
                ECP = 140,
                Name = "15-140馏分（石脑油）"
            };
            CutMothedEntity cutMethedEntity2 = new CutMothedEntity()
            {
                ICP = 15,
                ECP = 180,
                Name = "15-180馏分（石脑油）"
            };

            CutMothedEntity cutMethedEntity3 = new CutMothedEntity()
            {
                ICP = 140,
                ECP = 240,
                Name = "140-240馏分（航煤）"
            };

            CutMothedEntity cutMethedEntity4 = new CutMothedEntity()
            {
                ICP = 180,
                ECP = 350,
                Name = "180-350馏分（柴油）"
            };

            CutMothedEntity cutMethedEntity5 = new CutMothedEntity()
            {
                ICP = 240,
                ECP = 350,
                Name = "240-350馏分（柴油）"
            };

            CutMothedEntity cutMethedEntity6 = new CutMothedEntity()
            {
                ICP = 350,
                ECP = 540,
                Name = "350-540馏分（VGO）"
            };

            CutMothedEntity cutMethedEntity7 = new CutMothedEntity()
            {
                ICP = 350,
                ECP = 2000,
                Name = ">350（常渣）"
            };

            CutMothedEntity cutMethedEntity8 = new CutMothedEntity()
            {
                ICP = 540,
                ECP = 2000,
                Name = ">540（减渣）"
            };

            cutMothedEntityList.Add(cutMethedEntity1);
            cutMothedEntityList.Add(cutMethedEntity2);
            cutMothedEntityList.Add(cutMethedEntity3);
            cutMothedEntityList.Add(cutMethedEntity4);
            cutMothedEntityList.Add(cutMethedEntity5);
            cutMothedEntityList.Add(cutMethedEntity6);
            cutMothedEntityList.Add(cutMethedEntity7);
            cutMothedEntityList.Add(cutMethedEntity8);
            #endregion
            return cutMothedEntityList;
        }

        /// <summary>
        /// 保存到C库:临时快速查询数据库
        /// </summary>
        public static void SaveC(OilInfoEntity oilA, OilInfoBEntity oilB)
        {
            if (oilA == null || oilB == null)
                return;
            try
            {
                OilApply.OilCApplyBll oilCApplyBll = new OilApply.OilCApplyBll(oilA, oilB);
                List<OilDataSearchEntity> dataSearchList = oilCApplyBll.GetCutResult();

                OilDataSearchAccess access = new OilDataSearchAccess();
                access.Delete("oilInfoID = " + oilB.ID);
                OilBll.saveSearchTable(dataSearchList);
            }
            catch (Exception ex)
            {
                Log.Error("向快速查询库插入数据错误:" + ex);
            }
        }
        /// <summary>
        /// 保存到C库:临时快速查询数据库
        /// </summary>
        public static void SaveC(OilInfoBEntity oilB)
        {
            if (oilB == null)
                return;
            try
            {
                OilApply.OilCApplyBll oilCApplyBll = new OilApply.OilCApplyBll(null, oilB);
                List<OilDataSearchEntity> dataSearchList = oilCApplyBll.GetCutResult();

                OilDataSearchAccess access = new OilDataSearchAccess();
                access.Delete("oilInfoID = " + oilB.ID);
                OilBll.saveSearchTable(dataSearchList);
            }
            catch (Exception ex)
            {
                Log.Error("向查询库插入数据错误:" + ex);
            }
        }
        /// <summary>
        /// A库的infoYes 转换为A库的infoNo
        /// </summary>
        /// <param name="infoYes">A库的infoYes</param>
        /// <param name="infoNo">A库的infoNo</param>
        public static void InfoToInfo(OilInfoEntity infoYes, OilInfoEntity infoNo)
        {
            #region "A库的infoYes 转换为B库的infoNo"

            #region "10"
            infoNo.crudeName = infoYes.crudeName;
            infoNo.englishName = infoYes.englishName;
            infoNo.crudeIndex = infoYes.crudeIndex;
            infoNo.country = infoYes.country;
            infoNo.region = infoYes.region;

            infoNo.fieldBlock = infoYes.fieldBlock;
            infoNo.sampleDate = infoYes.sampleDate;
            infoNo.receiveDate = infoYes.receiveDate;
            infoNo.sampleSite = infoYes.sampleSite;
            infoNo.assayDate = infoYes.assayDate;
            #endregion

            #region "10"
            infoNo.updataDate = infoYes.updataDate;
            infoNo.sourceRef = infoYes.sourceRef;
            infoNo.assayLab = infoYes.assayLab;
            infoNo.assayer = infoYes.assayer;
            infoNo.assayCustomer = infoYes.assayCustomer;

            infoNo.reportIndex = infoYes.reportIndex;
            infoNo.summary = infoYes.summary;
            infoNo.type = infoYes.type;
            infoNo.classification = infoYes.classification;
            infoNo.sulfurLevel = infoYes.sulfurLevel;
            #endregion

            #region "10"
            infoNo.acidLevel = infoYes.acidLevel;
            infoNo.corrosionLevel = infoYes.corrosionLevel;
            infoNo.processingIndex = infoYes.processingIndex;
            infoNo.NIRSpectrum = infoYes.NIRSpectrum;
            infoNo.BlendingType = infoYes.BlendingType;

            infoNo.DataQuality = infoYes.DataQuality;
            infoNo.DataSource = infoYes.DataSource;
            infoNo.ICP0 = infoYes.ICP0;
            infoNo.S_01R = infoYes.S_01R;
            infoNo.S_02R = infoYes.S_02R;
            #endregion

            #region "10"
            infoNo.S_03R = infoYes.S_03R;
            infoNo.S_04R = infoYes.S_04R;
            infoNo.S_05R = infoYes.S_05R;
            infoNo.S_06R = infoYes.S_06R;
            infoNo.S_07R = infoYes.S_07R;

            infoNo.S_08R = infoYes.S_08R;
            infoNo.S_09R = infoYes.S_09R;
            infoNo.S_10R = infoYes.S_10R;
            #endregion

            #endregion
        }
        /// <summary>
        /// A库的info 转换为B库的InfoB
        /// </summary>
        /// <param name="infoYes">A库的info </param>
        /// <param name="infoB">B库的InfoB</param>
        public static void InfoToInfoB(OilInfoEntity infoA, OilInfoBEntity infoB)
        {
            #region "A库的info 转换为B库的InfoB"
            infoB.crudeName = infoA.crudeName;
            infoB.englishName = infoA.englishName;
            infoB.crudeIndex = infoA.crudeIndex;
            infoB.country = infoA.country;
            infoB.region = infoA.region;
            //infoB.fieldBlock = infoA.fieldBlock;
            //infoB.sampleDate = infoA.sampleDate;
            //infoB.receiveDate = infoA.receiveDate;
            //infoB.sampleSite = infoA.sampleSite;
            infoB.assayDate = infoA.assayDate;
            infoB.updataDate = infoA.updataDate;
            infoB.sourceRef = infoA.sourceRef;
            //infoB.assayLab = infoA.assayLab;
            //infoB.assayer = infoA.assayer;
            //infoB.assayCustomer = infoA.assayCustomer;
            //infoB.reportIndex = infoA.reportIndex;
            //infoB.summary = infoA.summary;
            infoB.type = infoA.type;
            infoB.classification = infoA.classification;
            infoB.sulfurLevel = infoA.sulfurLevel;
            infoB.acidLevel = infoA.acidLevel;
            //infoB.corrosionLevel = infoA.corrosionLevel;
            //infoB.processingIndex = infoA.processingIndex;
            //infoB.NIRSpectrum = infoA.NIRSpectrum;
            //infoB.BlendingType = infoA.BlendingType;
            infoB.DataQuality = infoA.DataQuality;
            //infoB.DataSource = infoA.DataSource;
            infoB.ICP0 = infoA.ICP0;
            #endregion
        }
        /// <summary>
        /// 保存一条原油
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码</returns>
        public static bool updateOilInfoBAndOilDataB(OilInfoBEntity info)
        {
            int oilInfoBID = info.ID;

            if (oilInfoBID < 0)
                return false;
            
            OilInfoBAccess oilInfoBAccess = new OilInfoBAccess();
            oilInfoBID =  oilInfoBAccess.Update(info, info.ID.ToString());
            saveTables(info);
            return true;
        }
        /// <summary>
        /// 保存一条原油
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码</returns>
        public static bool updateOilInfoB(OilInfoBEntity info)
        {
            int oilInfoBID = info.ID;

            if (oilInfoBID < 0)
                return false;

            OilInfoBAccess oilInfoBAccess = new OilInfoBAccess();
            oilInfoBID = oilInfoBAccess.Update(info, info.ID.ToString());
        
            return true;
        }
        
  
        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public static int saveInfo(OilInfoEntity info)
        {
            if (info.crudeIndex == "")  //ID<时如果原油编号为空，返回-1不能插入原油，否则插入           
                return -1;
            OilInfoAccess access = new OilInfoAccess();
            if (info.ID > 0)             //如果是打开编辑的原油(info.ID > 0),则更新，是新建的原油则判断新原油的代码是否存在，如果存在不能插入
            {
                access.Update(info, info.ID.ToString());
                OilDataAccess oilDataAccess = new OilDataAccess();
                oilDataAccess.Delete("labData='' and calData='' and oilInfoID=" + info.ID); //删除空的数据            
            }
            else
            {
                string sqlWhere = "crudeIndex='" + info.crudeIndex + "'";
                if (access.Get(sqlWhere).Count != 0)
                {
                    return -1;
                }
                info.ID = access.Insert(info);
            }
            return info.ID;
        }
        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info"></param>
        /// <param name="change"></param>
        /// <returns> -1 表示原油有重复</returns>
        public static int saveInfo(OilInfoBEntity info)
        {
            if (info.crudeIndex == "")  //ID<时如果原油编号为空，返回-1不能插入原油，否则插入           
                return -1;
            OilInfoBAccess access = new OilInfoBAccess();
            if (info.ID > 0)             //如果是打开编辑的原油(info.ID > 0),则更新，是新建的原油则判断新原油的代码是否存在，如果存在不能插入
            {
                access.Update(info, info.ID.ToString());
                OilDataBAccess oilDataAccess = new OilDataBAccess();
                oilDataAccess.Delete("labData='' and calData='' and oilInfoID=" + info.ID); //删除空的数据               
            }
            else
            {
                string sqlWhere = "crudeIndex='" + info.crudeIndex + "'";
                List<OilInfoBEntity> oilInfoBList = access.Get(sqlWhere).ToList();
                if (oilInfoBList.Count != 0)
                {
                    return -1;
                }
                else
                    info.ID = access.Insert(info);
            }
            return info.ID;
        }
        /// <summary>
        ///  批量插入快速查询表的数据 
        /// </summary>
        /// <param name="oilDatas">要插入的数据</param>
        public static void saveSearchTable(List<OilDataSearchEntity> oilDatas)
        {
            var dt = getTable("OilDataSearch");
            foreach (OilDataSearchEntity item in oilDatas)
            {
                var r = dt.NewRow();
                r["oilInfoID"] = item.oilInfoID;
                r["oilTableColID"] = item.oilTableColID;
                r["oilTableRowID"] = item.oilTableRowID;
                r["labData"] = item.labData;
                r["calData"] = item.calData;
                dt.Rows.Add(r);
            }
            BulkToDB(dt, "OilDataSearch");
        }

        /// <summary>
        ///  批量插入快速查询表的数据 
        /// </summary>
        /// <param name="oilDatas">要插入的数据</param>
        public static void saveSearchTable(List<OilDataEntity> oilDatas)
        {
            var dt = getTable("OilDataSearch");
            foreach (OilDataEntity item in oilDatas)
            {
                var r = dt.NewRow();
                r["oilInfoID"] = item.oilInfoID;
                r["oilTableColID"] = item.oilTableColID;
                r["oilTableRowID"] = item.oilTableRowID;
                r["labData"] = item.labData;
                r["calData"] = item.calData;
                dt.Rows.Add(r);
            }
            BulkToDB(dt, "OilDataSearch");
        }
        /// <summary>
        /// 批量插入快速查询表的数据 
        /// </summary>
        /// <param name="info">OilInfoBEntity</param>
        public static void saveSearchTable(OilInfoBEntity info)
        {
            var dt = getTable("OilDataSearch");

            foreach (var d in info.OilDataSearchs)
            {
                d.oilInfoID = info.ID;

                if (d.oilInfoID > 0 && d.oilTableColID > 0 && d.oilTableRowID > 0)
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = d.oilInfoID;
                    r["oilTableColID"] = d.oilTableColID;
                    r["oilTableRowID"] = d.oilTableRowID;
                    r["labData"] = d.labData;
                    r["calData"] = d.calData;
                    dt.Rows.Add(r);
                }
            }

            BulkToDB(dt, "OilDataSearch");
        }
        /// <summary>
        /// 批量插入原油信息的数据部分
        /// </summary>
        /// <param name="info">原油信息</param>
        public static void saveTables(List<OilDataEntity> OilDatas)
        {
            var dt = getTable("OilData");
            #region //批量插入其他表数据
            foreach (OilDataEntity d in OilDatas)
            {
                if (d.oilInfoID > 0 && d.oilTableColID > 0 && d.oilTableRowID > 0)
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = d.oilInfoID;
                    r["oilTableColID"] = d.oilTableColID;
                    r["oilTableRowID"] = d.oilTableRowID;
                    r["labData"] = d.labData;
                    r["calData"] = RIPP.Lib.Security.SecurityTool.MyEncrypt(d.calData);
                    dt.Rows.Add(r);
                }
            }
            /*向数据库中批量插入数据*/
            BulkToDB(dt, "OilData");
            #endregion
        }

        /// <summary>
        /// 更新原油信息
        /// </summary>
        /// <param name="info">原油信息</param>
        public static void updateTables2<T>(List<T> OilDatas)
            where T : RIPP.OilDB.UI.GridOil.V2.Model.IOilDataEntity
        {
            var tableName = "OilData";
            var type = typeof(T);
            if (type == typeof(OilDataBEntity) || type.IsSubclassOf(typeof(OilDataBEntity)))
                tableName = "OilDataB";
            else if (type == typeof(OilDataEntity) || type.IsSubclassOf(typeof(OilDataEntity)))
                tableName = "OilData";
            else
                throw new NotSupportedException(string.Format("{0} 未处理。", typeof(T)));

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"DELETE FROM {0} where oilInfoID={1} and oilTableColID in({2})", tableName, OilDatas.First().oilInfoID, string.Join(",", OilDatas.Select(o => o.oilTableColID).Distinct()));
            sql.AppendLine();
            foreach (var d in OilDatas)
            {
                if (string.IsNullOrWhiteSpace(d.labData) && string.IsNullOrWhiteSpace(d.calData))
                    continue;
                const string f = @"INSERT INTO {0} (oilInfoID,oilTableColID,oilTableRowID,labData,calData) VALUES ('{1}','{2}','{3}','{4}','{5}')";
                sql.AppendFormat(f, tableName,
                       d.oilInfoID, d.oilTableColID, d.oilTableRowID, string.IsNullOrWhiteSpace(d.labData) ? null : d.labData.Replace("'", "''"), RIPP.Lib.Security.SecurityTool.MyEncrypt(d.calData));
                sql.AppendLine();
            }
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql.ToString(), null);
        }
        /// <summary>
        /// 更新原油信息
        /// </summary>
        /// <param name="info">原油信息</param>
        public static void updateTables(List<OilDataEntity> OilDatas)
        {
            if (OilDatas == null || OilDatas.Count == 0)
                return;
            var tableName = "OilData";

            StringBuilder sql = new StringBuilder();
            int count = 0;
            foreach (var d in OilDatas)
            {
                var f = @"update {0} set labData='{4}',calData='{5}' where oilInfoID='{1}' and oilTableColID='{2}' and oilTableRowID='{3}'
IF @@ROWCOUNT = 0
   INSERT INTO {0} (oilInfoID,oilTableColID,oilTableRowID,labData,calData) VALUES ('{1}','{2}','{3}','{4}','{5}')
";
                sql.AppendFormat(f, tableName,
                       d.oilInfoID, d.oilTableColID, d.oilTableRowID, string.IsNullOrWhiteSpace(d.labData) ? null : d.labData.Replace("'", "''"), RIPP.Lib.Security.SecurityTool.MyEncrypt(d.calData));
                if (++count % 100000 == 0)
                {
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql.ToString(), null);
                    sql.Clear();
                    count = 0;
                }
            }
            if (count > 0)
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql.ToString(), null);

        }

        private static Regex regexChinese = new Regex(@"[\u4e00-\u9fa5]", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        /// <summary>
        /// 批量插入原油的数据部分
        /// </summary>
        /// <param name="info">原油信息</param>
        public static void saveTables(OilInfoEntity info)
        {
            var dt = getTable("OilData");
            #region //批量插入其他表数据
            foreach (var d in info.OilDatas)
            {
                d.oilInfoID = info.ID;

                #region
                //if (d.oilTableColID < 1 && d.OilTableCol != null)//添加Table ID
                //{
                //    if (!_colCache.Contains(d.OilTableCol))
                //        _colCache.Add(d.OilTableCol);

                //    if (d.OilTableCol.oilTableTypeID > 0)
                //        d.oilTableColID = _colCache[d.OilTableCol.colName, d.OilTableCol.oilTableTypeID].ID;
                //    else if (d.OilTableCol.OilTableType != null)
                //        d.oilTableColID = _colCache[d.OilTableCol.colName, (EnumTableType)(d.OilTableCol.OilTableType.ID)].ID;
                //}
                //if (d.oilTableRowID < 1 && d.OilTableRow != null) //添加 Row
                //{
                //    if (!_rowCache.Contains(d.OilTableRow))
                //        _rowCache.Add(d.OilTableRow);

                //    if (d.OilTableRow.oilTableTypeID > 0)
                //        d.oilTableRowID = _colCache[d.OilTableRow.itemCode, d.OilTableRow.oilTableTypeID].ID;
                //    else if (d.OilTableRow.OilTableType != null)
                //        d.oilTableRowID = _colCache[d.OilTableRow.itemCode, (EnumTableType)d.OilTableRow.OilTableType.ID].ID;
                //}
                #endregion

                if (d.oilInfoID > 0 && d.oilTableColID > 0 && d.oilTableRowID > 0)
                {
                    try
                    {
                        var r = dt.NewRow();
                        r["oilInfoID"] = d.oilInfoID;
                        r["oilTableColID"] = d.oilTableColID;
                        r["oilTableRowID"] = d.oilTableRowID;
                        r["labData"] = d.labData;
                        //汉字截断到5个字符
                        if (string.IsNullOrWhiteSpace(d.labData) == false)
                        {
                            d.labData = d.labData.Trim();
                            if (regexChinese.IsMatch(d.labData) && d.labData.Length > 5)
                                d.labData = d.labData.Substring(0, 5);
                        }
                        //汉字截断到5个字符
                        if (string.IsNullOrWhiteSpace(d.calData) == false)
                        {
                            d.calData = d.calData.Trim();
                            if (regexChinese.IsMatch(d.calData) && d.calData.Length > 5)
                                d.calData = d.calData.Substring(0, 5);
                        }
                        r["calData"] = RIPP.Lib.Security.SecurityTool.MyEncrypt(d.calData);
                        dt.Rows.Add(r);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("保存原油元素出错：" + ex);
                    }
                }

            }
            /*向数据库中批量插入数据*/
            BulkToDB(dt, "OilData");
            #endregion
        }

        /// <summary>
        /// 批量插入原油信息的数据部分
        /// </summary>
        /// <param name="info">原油信息</param>
        public static void saveTables(OilInfoBEntity info)
        {
            var dt = getTable("OilDataB");
            #region //批量插入其他表数据
            foreach (var d in info.OilDatas)
            {
                d.oilInfoID = info.ID;

                if (d.oilInfoID > 0 && d.oilTableColID > 0 && d.oilTableRowID > 0)
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = d.oilInfoID;
                    r["oilTableColID"] = d.oilTableColID;
                    r["oilTableRowID"] = d.oilTableRowID;
                    r["labData"] = d.labData;
                    r["calData"] = RIPP.Lib.Security.SecurityTool.MyEncrypt(d.calData);
                    dt.Rows.Add(r);
                }
            }

            BulkToDB(dt, "OilDataB");
            #endregion
        }
        /// <summary>
        /// 将A库的数据初始化到B库包括(原油性质表、GC标准表)
        /// </summary>
        /// <param name="oilInfoB"></param>
        /// <param name="oilDatasAll"></param>
        public static void ConvertToDatasB(ref OilInfoBEntity oilInfoB, List<OilDataEntity> oilDatasAll)
        {
            List<OilDataEntity> oilDatas = oilDatasAll.Where(c => c.OilTableTypeID == (int)EnumTableType.GCLevel || c.OilTableTypeID == (int)EnumTableType.Whole).ToList();
            List<OilDataBEntity> dataBlist = new List<OilDataBEntity>();
            foreach (OilDataEntity oilData in oilDatas)
            {
                float tempCal = 0;
                if (!string.IsNullOrWhiteSpace(oilData.calData) && float.TryParse(oilData.calData, out tempCal) && oilData.calData != "非数字")
                {
                    OilDataBEntity oilDataB = new OilDataBEntity();
                    oilDataB.oilInfoID = oilInfoB.ID;
                    oilDataB.oilTableColID = oilData.oilTableColID;
                    oilDataB.oilTableRowID = oilData.oilTableRowID;
                    oilDataB.labData = oilData.calData;
                    oilDataB.calData = oilData.calData;
                    dataBlist.Add(oilDataB);
                }
            }
            oilInfoB.OilDatas.Clear();
            oilInfoB.OilDatas.AddRange(dataBlist);
            OilDataBAccess oilDataBAccess = new Data.OilDataBAccess();
            oilDataBAccess.Delete("oilInfoID =" + oilInfoB.ID);
            saveTables(oilInfoB);
        }

        /// <summary>
        /// 批量插入B库曲线的数据部分
        /// </summary>
        /// <param name="info">B库原油信息</param>
        public static void saveCurves(List<CurveEntity> curveList)
        {
            var dt = getTable("CurveData");
            #region //批量插入其他表数据

            foreach (CurveEntity curve in curveList)
            {
                foreach (var data in curve.curveDatas)
                {
                    var r = dt.NewRow();
                    r["curveID"] = curve.ID;
                    r["xValue"] = data.xValue;
                    r["yValue"] = data.yValue;

                    dt.Rows.Add(r);
                }
            }

            BulkToDB(dt, "CurveData");
            #endregion
        }

        /// <summary>
        /// 批量插入B库曲线的数据部分
        /// </summary>
        /// <param name="info">B库原油信息</param>
        public static void saveCurves(OilInfoBEntity info)
        {
            var dt = getTable("CurveData");
            #region //批量插入其他表数据

            foreach (CurveEntity curve in info.curves)
            {
                foreach (var data in curve.curveDatas)
                {
                    var r = dt.NewRow();
                    r["curveID"] = data.curveID;
                    r["xValue"] = data.xValue;
                    r["yValue"] = data.yValue;

                    dt.Rows.Add(r);
                }
            }

            BulkToDB(dt, "CurveData");
            #endregion
        }
        /// <summary>
        /// 批量插入C库原油性质的数据部分
        /// </summary>
        /// <param name="infoB">B库原油信息</param>
        public static void saveCOilInfo(OilInfoBEntity infoB)
        {
            if (infoB == null)
                return;

            var dt = getTable("OilDataSearch");
            #region //批量插入其他表数据
            int oilInfoID = infoB.ID;
            OilDataSearchColAccess searchColAccess = new OilDataSearchColAccess();
            List<OilDataSearchColEntity> dataSearchCols = searchColAccess.Get("1=1").Where(o => o.OilTableName == "原油信息").ToList();
            int oilTableColID = dataSearchCols[0].OilTableColID;

            OilDataSearchRowAccess oilDataRowAccess = new OilDataSearchRowAccess();
            List<OilDataSearchRowEntity> oilDataRowEntityList = oilDataRowAccess.Get("1=1").Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Info).ToList();

            foreach (OilDataSearchRowEntity e in oilDataRowEntityList)
            {
                #region
                if (e.OilTableRow.itemCode == "CNA")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.crudeName;
                    r["calData"] = infoB.crudeName;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "ENA")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.englishName;
                    r["calData"] = infoB.englishName;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "IDC")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.crudeIndex;
                    r["calData"] = infoB.crudeIndex;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "COU")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.country;
                    r["calData"] = infoB.country;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "GRC")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.region;
                    r["calData"] = infoB.region;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "ADA")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.receiveDate;
                    r["calData"] = infoB.receiveDate;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "ALA")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.assayLab;
                    r["calData"] = infoB.assayLab;
                    dt.Rows.Add(r);
                }
                #endregion

                #region
                if (e.OilTableRow.itemCode == "AER")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.assayer;
                    r["calData"] = infoB.assayer;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "SR")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.sourceRef;
                    r["calData"] = infoB.sourceRef;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "ASC")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.assayCustomer;
                    r["calData"] = infoB.assayCustomer;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "RIN")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.reportIndex;
                    r["calData"] = infoB.reportIndex;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "CLA")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.type;
                    r["calData"] = infoB.type;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "TYP")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.classification;
                    r["calData"] = infoB.classification;
                    dt.Rows.Add(r);
                }
                else if (e.OilTableRow.itemCode == "SCL")
                {
                    var r = dt.NewRow();
                    r["oilInfoID"] = oilInfoID;
                    r["oilTableColID"] = oilTableColID;
                    r["oilTableRowID"] = e.OilTableRowID;
                    r["labData"] = infoB.sulfurLevel;
                    r["calData"] = infoB.sulfurLevel;
                    dt.Rows.Add(r);
                }
                #endregion
            }
            BulkToDB(dt, "OilDataSearch");
            #endregion
        }
        /// <summary>
        /// 批量插入C库原油性质的数据部分
        /// </summary>
        /// <param name="info">B库原油信息</param>
        public static void saveCWhole(OilInfoBEntity info)
        {
            var dt = getTable("OilDataSearch");
            #region //批量插入其他表数据
            OilDataSearchRowAccess oilDataRowAccess = new OilDataSearchRowAccess();
            List<OilDataSearchRowEntity> oilDataRowEntityList = oilDataRowAccess.Get("1=1").Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Whole).ToList();

            List<OilDataBEntity> wholeDataList = info.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();//选出原油性质的数据

            foreach (OilDataBEntity d in wholeDataList)
            {
                foreach (OilDataSearchRowEntity e in oilDataRowEntityList)
                {
                    if (e.OilTableRow.itemCode == d.OilTableRow.itemCode)
                    {
                        var r = dt.NewRow();
                        r["oilInfoID"] = d.oilInfoID;
                        r["oilTableColID"] = d.oilTableColID;
                        r["oilTableRowID"] = d.oilTableRowID;
                        r["labData"] = d.labData;
                        r["calData"] = d.calData;
                        dt.Rows.Add(r);
                    }
                }
            }


            BulkToDB(dt, "OilDataSearch");
            #endregion
        }
        /// <summary>
        /// 批量插入C库切割后的曲线的数据部分
        /// </summary>
        /// <param name="info">B库原油信息</param>
        public static void saveCCutResult(OilInfoBEntity info, List<CutMothedEntity> cutMothedEntityList)
        {
            var dt = getTable("OilDataSearch");
            #region //批量插入其他表数据
            /*获取针对切割名称的行代码*/
            OilDataSearchColAccess oilDataColAccess = new OilDataSearchColAccess();
            List<OilDataSearchColEntity> oilDataColData = oilDataColAccess.Get("1=1");
            //List<OilDataTableBEntity> cutResutlt = info.OilDataTableBEntityList;//切割结果
            try
            {
                foreach (CutMothedEntity c in cutMothedEntityList)//8个固定的切割方案
                {
                    OilDataSearchColEntity oilDataColEntity = oilDataColData.Where(o => o.OilTableName == c.Name).FirstOrDefault();
                    List<OilDataSearchRowEntity> oilDataRowEntityName = oilDataColEntity.OilDataRowList;//需要显示的物性

                    // List<OilDataTableBEntity> cutResutltTemp = cutResutlt.Where(o => o.OilTableName == c.name).ToList();//计算出的物性

                    foreach (OilDataSearchRowEntity od in oilDataRowEntityName)
                    {
                        //foreach (OilDataTableBEntity ot in cutResutltTemp)
                        //{
                        //    if (ot.ItemCode == od.OilTableRow.itemCode)
                        //    {
                        //        var r = dt.NewRow();
                        //        r["oilInfoID"] = info.ID;
                        //        r["oilTableColID"] = oilDataColEntity.OilTableColID;
                        //        r["oilTableRowID"] = od.OilTableRowID;
                        //        r["labData"] = ot.CalData;
                        //        r["calData"] = ot.CalData;
                        //        dt.Rows.Add(r);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            BulkToDB(dt, "OilDataSearch");
            #endregion
        }
        /// <summary>
        /// 批量插入C库切割后的曲线的数据部分
        /// </summary>
        /// <param name="info">B库原油信息</param>
        public static void saveCurveDatasB(OilInfoBEntity info)
        {
            var dt = getTable("CurveDataB");
            #region //批量插入其他表数据

            foreach (CurveEntity curve in info.curves)
            {
                foreach (var data in curve.curveDatas)
                {
                    var r = dt.NewRow();
                    r["curveID"] = data.curveID;
                    r["xValue"] = data.xValue;
                    r["yValue"] = data.yValue;

                    dt.Rows.Add(r);
                }
            }

            BulkToDB(dt, "CurveDataB");
            #endregion
        }
        /// <summary>
        /// 批量插入C库切割后的曲线的数据部分
        /// </summary>
        /// <param name="info">B库原油信息</param>
        public static void saveTargetedValue(List<TargetedValueEntity> list)
        {
            var dt = getTable("TargetedValue");
            #region //批量插入其他表数据

            foreach (TargetedValueEntity data in list)
            {
                var r = dt.NewRow();
                r["S_UserID"] = data.S_UserID;
                r["OilTableTypeComparisonTableID"] = data.OilTableTypeComparisonTableID;
                r["TargetedValueColID"] = data.TargetedValueColID;
                r["TargetedValueRowID"] = data.TargetedValueRowID;
                //r["Value"] = data.fValue.ToString();
                r["Value"] = data.strValue;
                dt.Rows.Add(r);
            }

            BulkToDB(dt, "TargetedValue");
            #endregion
        }
        /// <summary>
        /// 根据数据库表名获取表的格式
        /// </summary>
        /// <param name="tableName">数据库中数据表名</param>
        /// <returns>DataTable</returns>
        private static DataTable getTable(string tableName)
        {           
            //SqlConnection sqlconnection = new SqlConnection(
            //    System.Configuration.ConfigurationManager.ConnectionStrings["SQLConnString"].ConnectionString);
            SqlDataAdapter sqldataadapter = new SqlDataAdapter("select  * from " + tableName + " where 1=2", SqlHelper.GetConnection());
            DataSet dataset = new DataSet();
            sqldataadapter.Fill(dataset, tableName);
            var dt = dataset.Tables[0];
            dt.Rows.Clear();
            return dt;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="dt">表格数据</param>
        private static void BulkToDB(DataTable dt, string destinTableName)
        {
            SqlConnection sqlConn = new SqlConnection(SqlHelper .connectionString );
               // System.Configuration.ConfigurationManager.ConnectionStrings["SQLConnString"].ConnectionString
            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn);
            bulkCopy.DestinationTableName = destinTableName;
            bulkCopy.BatchSize = dt.Rows.Count;

            try
            {
                sqlConn.Open();
                if (dt.Rows != null && dt.Rows.Count != 0)
                    bulkCopy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                Log.Error("数据管理,批量插入BulkToDB()" + ex);
            }
            finally
            {
                sqlConn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }
        }

        #endregion

        #region "删除原油"
        /// <summary>
        /// 根据ID删除一条原油
        /// </summary>
        /// <param name="id">原油ID</param>
        public static void delete(int id, LibraryType libraryType)
        {

            if (libraryType == LibraryType.LibraryA)
            {
                OilInfoAccess access = new OilInfoAccess();
                access.Delete(id);
            }
            else
            {
                OilInfoBAccess access = new OilInfoBAccess();
                access.Delete(id);
            }
        }
        #endregion

        #region "读入Excel以及输出Excel"


        public static OilInfoEntity importExcel(string fileName)
        {
            int oilInfoID = 0;
            OilInfoEntity oilInfo = new OilInfoEntity();
            try
            {
                DataSet ds = ExcelTool.ExcelToDataSet(fileName);

                string itemCode = "";

                #region 原油基本信息
                DataRow dr = ds.Tables["原油信息"].NewRow();  //读取的第一行DataTable当成表头
                dr[1] = ds.Tables["原油信息"].Columns[1];
                dr[2] = ds.Tables["原油信息"].Columns[2];
                ds.Tables["原油信息"].Rows.Add(dr);
                for (int i = 0; i < ds.Tables["原油信息"].Rows.Count; i++)
                {
                    itemCode = ds.Tables["原油信息"].Rows[i][1].ToString().Trim();
                    string value = ds.Tables["原油信息"].Rows[i][2].ToString().Trim();
                    oilInfoAddItem(ref oilInfo, itemCode, value);
                }
                #endregion
                oilInfoID = saveInfo(oilInfo);
                oilInfo.ID = oilInfoID;
                if (oilInfoID == -1)
                {
                    return oilInfo;
                }

                OilTableRowBll rowBll = new OilTableRowBll();
                OilTableColBll colBll = new OilTableColBll();
                try
                {
                    #region 原油性质表

                    getSheetData(ds.Tables["原油性质"], ref oilInfo, 1, 2, 3, EnumTableType.Whole);

                    #endregion
                }
                catch (Exception ex)
                {
                    Log.Error("数据管理,导入Excel到库A 原油性质表出问题" + ex);
                }
                int colID = 0;//
                OilTools oilTools = new OilTools();
                try
                {
                    Application.DoEvents();
                    #region 轻端-气体
                    List<string> itemCodelist = new List<string>();//用来判断是哪一个表

                    int colID1 = colBll["Cut1", EnumTableType.Light].ID;
                    int colID2 = colBll["Cut2", EnumTableType.Light].ID;


                    int rowCount = ds.Tables["气体"] == null ? 0 : ds.Tables["气体"].Rows.Count;

                    for (int i = 0; i < rowCount; i++)
                    {
                        itemCode = ds.Tables["气体"].Rows[i][1].ToString().Trim(); //在行中获取代码
                        if (!itemCodelist.Contains(itemCode))
                        {
                            itemCodelist.Add(itemCode);
                            colID = colID1;
                        }
                        else
                        {
                            colID = colID2;
                        }

                        OilTableRowEntity row = _rowCache[itemCode, EnumTableType.Light]; //获取代码的ID
                        if (row != null)
                        {
                            string labData = ds.Tables["气体"].Rows[i][2].ToString().Trim();
                            if (row.decNumber != null)
                                labData = oilTools.calDataDecLimit(labData, row.decNumber + 2, row.valDigital);//输入Execl表的过程中转换数据精度
                            else
                                labData = oilTools.calDataDecLimit(labData, row.decNumber, row.valDigital);//输入Execl表的过程中转换数据精度                         
                            //labData = oilTools.calDataDecLimit(labData, row.decNumber);//输入Execl表的过程中转换数据精度   

                            OilDataEntity oilData = new OilDataEntity();
                            oilData.oilInfoID = oilInfoID;
                            oilData.oilTableColID = colID;
                            if (i <= 17)
                                oilData.oilTableColID = colID;
                            else
                                oilData.oilTableColID = colID2;
                            oilData.oilTableRowID = row.ID;
                            oilData.labData = labData;
                            oilData.calData = labData;
                            oilInfo.OilDatas.Add(oilData);
                        }
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    Log.Error("数据管理,导入Excel到库A 轻端表出问题" + ex);
                }
                try
                {
                    #region 窄馏分-实沸点

                    #region step1 把表中代码对应的ID存在列表中(第3行是代码 )
                    ds.Tables["实沸点"].Rows.RemoveAt(0);
                    List<int> arrRow = new List<int>();  //存行ID
                    int colSul = -1, colN2 = -1; // colSul记录硫含量%的列，氮含量%的列
                    int colSulValue = -1, colN2Value = -1; // colSul记录硫含量值的列，氮含量值的列
                    //ds.Tables.Contains("实沸点")==false
                    var columnCount = ds.Tables["实沸点"] == null || ds.Tables["实沸点"].Rows.Count <= 3 ? 0 : ds.Tables["实沸点"].Columns.Count;
                    for (int i = 0; i < columnCount; i++)
                    {
                        itemCode = ds.Tables["实沸点"].Rows[3][i].ToString().Trim();
                        if (itemCode != "")
                        {
                            OilTableRowEntity row = rowBll[itemCode, EnumTableType.Narrow];
                            if (row != null)
                            {
                                var prvRowValue = ds.Tables["实沸点"].Rows[1][i].ToString().Trim();
                                if (string.IsNullOrWhiteSpace(prvRowValue))
                                    prvRowValue = ds.Tables["实沸点"].Rows[2][i].ToString().Trim();
                                bool isPercentage = prvRowValue.Contains('%') || prvRowValue.Contains('％');
                                if (itemCode == "SUL")
                                {
                                    if (isPercentage)
                                        colSul = i;
                                    else
                                        colSulValue = i;
                                }
                                else if (itemCode == "N2")
                                {
                                    if (isPercentage)
                                        colN2 = i;
                                    else
                                        colN2Value = i;
                                }
                                arrRow.Add(row.ID);
                            }
                            else
                                arrRow.Add(-1); //可能包含其他符号，不是itemCode
                        }
                        else
                        {
                            arrRow.Add(-1);
                        }
                    }
                    #endregion

                    int rowsCount = 0;

                    
                    #region "判断有多少行"
                    
                    int colCount = ds.Tables["实沸点"] == null ? 0 : ds.Tables["实沸点"].Columns.Count;
                    for (int i = 0; i < colCount; i++)
                    {
                        string strItemCode = ds.Tables["实沸点"].Rows[3][i].ToString().Trim();

                        if (strItemCode == "ICP")
                        {
                            for (int j = 4; j < ds.Tables["实沸点"].Rows.Count; j++)
                            {
                                string dataICP = ds.Tables["实沸点"].Rows[j][i].ToString().Trim();
                                string dataNext = ds.Tables["实沸点"].Rows[j][i + 1].ToString().Trim();
                                string dataECP = ds.Tables["实沸点"].Rows[j][i + 2].ToString().Trim();
                                if (dataICP == string.Empty && dataNext == ">")
                                {
                                    rowsCount = j - 1;
                                    break;
                                }
                                else if (dataICP == string.Empty && dataECP == string.Empty)
                                {
                                    rowsCount = j - 1;
                                    break;
                                }
                                else if (dataECP == string.Empty)
                                {
                                    rowsCount = j - 1;
                                    break;
                                }
                                if (j == ds.Tables["实沸点"].Rows.Count - 1 && dataICP != string.Empty && dataECP != string.Empty)
                                {
                                    rowsCount = j;
                                }

                            }
                        }
                    }



                    #endregion

                    #region step2: 遍历每行每列，如果单元格不空则添加数据
                    for (int i = 4; i <= rowsCount; i++)
                    {
                        colID = colBll["Cut" + (i - 3).ToString(), EnumTableType.Narrow].ID; //真实表格中的一行是一个Cut
                        for (int x = 0; x < arrRow.Count; x++)
                        {
                            if (arrRow[x] > 0)
                            {
                                var t = ds.Tables["实沸点"].Rows[i][x];
                                string labData = ds.Tables["实沸点"].Rows[i][x].ToString().Trim(); //取一个单元格值   

                                if (string.IsNullOrWhiteSpace(labData))
                                    continue;
                                labData = labData.Trim();

                                OilTableRowEntity row = rowBll[arrRow[x], (int)EnumTableType.Narrow];

                                #region 如果上一列μg/g为单位的SUL存在，找到其oilData，转化为%单位


                                if (colSul == x)//找到列为单位%的SUL列
                                {
                                    OilDataEntity oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Narrow && c.OilTableRow.itemCode == "SUL" && c.oilTableColID == colID).FirstOrDefault();
                                    if (oilDataTemp != null) //如果前一个单元格不空，进行单位转化
                                        oilInfo.OilDatas.Remove(oilDataTemp);
                                }
                                else if (colSulValue == x)
                                {
                                    OilDataEntity oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Narrow && c.OilTableRow.itemCode == "SUL" && c.oilTableColID == colID).FirstOrDefault();
                                    if (oilDataTemp != null)
                                        continue;
                                    int decNum = row.decNumber == null ? row.valDigital : row.decNumber.Value;
                                    if (!oilTools.unitUgTo(ref labData, decNum)) //转化数据单位,如果不能转化，修改值为空
                                        continue;
                                }
                                #endregion

                                //if (labData.Length > 12) //可能是一些备注，暂时这样做
                                //    continue;

                                #region 如果当前单元格%为单位的N2,转化为μg/g单位，否则跳过该单元格
                                if (colN2 == x)//为μg/g单位 %
                                {
                                    OilDataEntity oilDataTemp;
                                    oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Narrow && c.OilTableRow.itemCode == "N2" && c.oilTableColID == colID).FirstOrDefault();
                                    if (oilDataTemp != null) //如果前一个单元格不空，则跳过
                                        continue;
                                    int decNum = row.decNumber == null ? row.valDigital : row.decNumber.Value;
                                    if (!oilTools.unitToUg(ref labData, decNum)) //转化数据单位,如果不能转化，修改值为空
                                        continue;
                                }
                                else if (colN2Value == x)
                                {
                                    var oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Narrow && c.OilTableRow.itemCode == "N2" && c.oilTableColID == colID).FirstOrDefault();
                                    if (oilDataTemp != null) //如果前一个单元格不空，则跳过
                                        oilInfo.OilDatas.Remove(oilDataTemp);
                                }
                                #endregion
                                if (row.decNumber != null)
                                    labData = oilTools.calDataDecLimit(labData, row.decNumber , row.valDigital);//labData = oilTools.calDataDecLimit(labData, row.decNumber + 2, row.valDigital);输入Execl表的过程中转换数据精度
                                //不明白为何原编码中，显示的小数倍数要+2
                                else
                                    labData = oilTools.calDataDecLimit(labData, row.decNumber, row.valDigital);//输入Execl表的过程中转换数据精度                         

                                //labData = oilTools.calDataDecLimit(labData, row.decNumber, row.valDigital);//输入Execl表的过程中转换数据精度        

                                OilDataEntity oilData = new OilDataEntity();
                                oilData.oilInfoID = oilInfoID;
                                oilData.oilTableColID = colID;
                                oilData.oilTableRowID = arrRow[x];
                                oilData.labData = labData;
                                oilData.calData = labData;
                                oilInfo.OilDatas.Add(oilData);
                            }
                        }
                    }
                    #endregion

                    #endregion
                }
                catch (Exception ex)
                {
                    Log.Error("数据管理,导入Excel到库A 窄馏分表出问题" + ex);
                }

                try
                {
                    #region 渣油
                    getSheetData(ds.Tables["渣油"], ref oilInfo, 1, 2, 3, EnumTableType.Residue);
                    #endregion
                }
                catch (Exception ex)
                {
                    Log.Error("数据管理,导入Excel到库A 渣油表出问题" + ex);
                }
                try
                {
                    #region 宽馏分

                    //WCutTypeAccess acess = new WCutTypeAccess();
                    // List<WCutTypeEntity> wCutTypes = acess.Get("1=1").ToList();  //宽馏分cut的名称
                    S_ParmBll s_ParmBll = new S_ParmBll();
                    List<S_ParmEntity> wCutTypes = s_ParmBll.GetParms("WCT");

                    int cutCount = 0; //宽馏分cut数目
                    int oldCount = 0;

                    getSheetData(ds.Tables["重整料"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "重整料").FirstOrDefault().parmValue);
                    getSheetData(ds.Tables["石脑油"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "石脑油").FirstOrDefault().parmValue);
                    getSheetData(ds.Tables["航煤"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "航煤").FirstOrDefault().parmValue);
                    oldCount = cutCount;
                    getSheetData(ds.Tables["柴油"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "柴油").FirstOrDefault().parmValue);
                    getSheetData(ds.Tables["柴油质谱"], ref oldCount, ref oilInfo, 1, 1, 2, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "柴油").FirstOrDefault().parmValue);
                    getSheetData(ds.Tables["重油质谱"], ref oldCount, ref oilInfo, 1, 1, 2, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "柴油").FirstOrDefault().parmValue);

                    getSheetData(ds.Tables["溶剂油"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "溶剂油").FirstOrDefault().parmValue);
                    oldCount = cutCount;
                    //getSheetData(ds.Tables["VGO"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "VGO").FirstOrDefault().parmValue);
                    getSheetData(ds.Tables["VGO"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "蜡油").FirstOrDefault().parmValue);
                    #endregion
                }
                catch (Exception ex)
                {
                    Log.Error("数据管理,导入Excel到原始库 宽馏分表出问题" + ex);
                }

                //saveTables(oilInfo);

                //OilInfoEntity retOilInfoA = OilBll.GetOilById(oilInfo.ID);
                //return retOilInfoA;
                return oilInfo;//考虑到导入Excel的数据不稳定
            }
            catch (Exception ex)
            {
                Log.Error("数据管理,导入Excel到库原始库:" + ex);
                return null;
            }
        }

        /// <summary>
        /// 导入Excel到库A，并返回原油的oilInfoID
        /// </summary>
        /// <param name="path">excel的路径</param>
        /// <returns></returns>
        public static OilInfoEntity importExcel2(string fileName)
        {
            int oilInfoID = 0;
            OilInfoEntity oilInfo = new OilInfoEntity();
            try
            {
                DataSet ds = ExcelTool.ExcelToDataSet(fileName);

                string itemCode = "";
                //得到所有的oildTableRow Select(d => new { d.itemName, d.itemUnit, d.itemCode })
                //List<OilTableRowEntity> allOilTableRow = OilTableRowBll._OilTableRow.Where(d => new int[] { 2, 6, 7, 8 }.Contains(d.oilTableTypeID)).Distinct().ToList();

                //原油信息表的itemcode
                List<string> oilInfoItemCode = OilTableRowBll._OilTableRow.Where(d => d.oilTableTypeID == (int)EnumTableType.Info).Select(d => d.itemCode).Distinct().ToList();
                ////2 6 7 8的itemcode
                List<string> allItemCode = OilTableRowBll._OilTableRow.Where(d => new int[] { (int)EnumTableType.Whole, (int)EnumTableType.Light, (int)EnumTableType.Narrow, (int)EnumTableType.Wide, (int)EnumTableType.Residue }.Contains(d.oilTableTypeID)).Select(d => d.itemCode).Distinct().ToList();
                //首先要找到原油信息表，没有原油信息表 则没必要继续下去
                #region 找到原油信息表
                foreach (DataTable table in ds.Tables)
                {
                    int[] firstCode = new int[2] { -1, -1 };
                    for (int r_num = 0; r_num < table.Rows.Count; r_num++)
                    {
                        for (int c_num = 0; c_num < table.Columns.Count; c_num++)
                        {
                            string cellContent = table.Rows[r_num][c_num].ToString();
                            cellContent = Units.ToDBC(cellContent);//ask:usage???

                            if (cellContent.Equals("CNA") && oilInfoItemCode.Contains(cellContent))//ask:mean???///if (!cellContent.Equals("CLA")&&oilInfoItemCode.Contains(cellContent))
                            {
                                //原油信息表 第一个代码
                                firstCode[0] = r_num;
                                firstCode[1] = c_num;

                                for (int i = firstCode[0]; i < table.Rows.Count; i++)
                                {
                                    itemCode = table.Rows[i][firstCode[1]].ToString().Trim();
                                    itemCode = Units.ToDBC(itemCode);
                                    string value = table.Rows[i][firstCode[1] + 1].ToString().Trim();
                                    value = Units.ToDBC(value);
                                    oilInfoAddItem(ref oilInfo, itemCode, value);
                                }
                                oilInfoID = saveInfo(oilInfo);
                                oilInfo.ID = oilInfoID;
                                if (oilInfoID == -1)
                                {
                                    return oilInfo;
                                }
                            }
                            if (firstCode[0] != -1)
                                break;
                        }
                        if (firstCode[0] != -1)
                            break;
                    }
                    if (firstCode[0] != -1)
                        break;
                }
                #endregion

                //记录所有窄馏分、宽馏分、渣油 数据
                List<WCol> oilTablesData = new List<WCol>();

                //找到原油信息表后，遍历每个表
                foreach (DataTable table in ds.Tables)
                {
                    string name = table.TableName;
                    int[] firstCode = new int[2] { -1, -1 };
                    //遍历每个单元格找到第一个代码
                    #region 找到表格中第一个代码
                    for (int r_num = 0; r_num < table.Rows.Count; r_num++)
                    {
                        for (int c_num = 0; c_num < table.Columns.Count; c_num++)
                        {
                            string cellContent = table.Rows[r_num][c_num].ToString();
                            cellContent = Units.ToDBC(cellContent);
                            //包含ICP，或在allItemCode中
                            if (cellContent.Contains("IEP") || cellContent.Contains("ICP") || allItemCode.Contains(cellContent))
                            {
                                //找到第一个代码
                                firstCode[0] = r_num;
                                firstCode[1] = c_num;
                            }
                            if (firstCode[0] != -1)
                                break;
                        }
                        if (firstCode[0] != -1)
                            break;
                    }
                    #endregion

                    if (firstCode[0] == -1)
                    {
                        //此表格没有ItemCode，继续下一个表格
                        continue;
                    }

                    #region 判断表格是窄馏分or宽馏分
                    //判断第一个code右边两个中有一个在allitemcode中就判断是窄馏分
                    else//找到itemcode
                    {
                        EnumTableType tableType = EnumTableType.None;
                        DataTable oilTable = new DataTable();
                        oilTable = table;
                        if (allItemCode.Contains(Units.ToDBC(table.Rows[firstCode[0]][firstCode[1] + 1].ToString())) || allItemCode.Contains(Units.ToDBC(table.Rows[firstCode[0]][firstCode[1] + 2].ToString())))
                        {
                            tableType = EnumTableType.Narrow;
                            //窄馏分 则进行行列转置
                            #region 转置
                            DataTable dtNew = new DataTable();
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                dtNew.Columns.Add("Column" + (i + 1).ToString(), typeof(string));
                            }
                            foreach (DataColumn dc in table.Columns)
                            {
                                DataRow drNew = dtNew.NewRow();
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    drNew[i] = table.Rows[i][dc].ToString();
                                }
                                dtNew.Rows.Add(drNew);
                            }
                            #endregion
                            oilTable = dtNew;
                            int temp = firstCode[0];
                            firstCode[0] = firstCode[1];
                            firstCode[1] = temp;
                        }

                        //存放此table中的列项
                        //List<OilTableColEntity> tableCol = new List<OilTableColEntity>();
                        //List<OilTableRowEntity> tableRow = new List<OilTableRowEntity>();
                        Dictionary<int, int> tableColToColID = new Dictionary<int, int>();
                        //先找到iep或icp所在列，得到行、列 的顺序列表

                        //记录IEP的行
                        int IEPRow = -1;
                        int ICPRow = -1;
                        int ECPRow = -1;

                        Dictionary<string, string[]> codeUnitConvert = new Dictionary<string, string[]>();
                        List<string> itemCodeList = new List<string>();
                        

                        //SortedList<double, int> sortMCPToCol = new SortedList<double, int>();
                        //记录单个的温度的列
                        //SortedList<double, int> sortECPOrICPToCol = new SortedList<double, int>();

                        //行
                        for (int row = firstCode[0]; row < oilTable.Rows.Count; row++)
                        {
                            string rowContent = oilTable.Rows[row][firstCode[1]].ToString().Trim();
                            rowContent = Units.ToDBC(rowContent);
                            if (rowContent.Equals("G00"))
                            {
                                tableType = EnumTableType.Light;
                            }
                            //补充前面空行，使itemcodelist顺序与行号一致
                            while (itemCodeList.Count < row)
                                itemCodeList.Add("");

                            //记录下此表的行的itemcode顺序
                            itemCodeList.Add(rowContent);

                            //记录下温度行
                            if (rowContent.Contains("IEP"))
                            {
                                IEPRow = row;
                            }
                            if (rowContent.Contains("ICP"))
                            {
                                ICPRow = row;
                            }
                            if (rowContent.Contains("ECP"))
                            {
                                ECPRow = row;
                            }
                            if (tableType != EnumTableType.Light)
                            {
                                #region 单位转换
                                //先读取行前面的 单位，确定哪一行如何单位转换
                                int colIndex = firstCode[1];
                                string frontContent = "";
                                #region 获得前面的值
                                if (colIndex >= 1)
                                {
                                    frontContent = oilTable.Rows[row][colIndex - 1].ToString().Trim();
                                    if (colIndex >= 2)
                                    {
                                        frontContent += oilTable.Rows[row][colIndex - 2].ToString().Trim();
                                    }
                                    if (colIndex >= 3)
                                    {
                                        frontContent += oilTable.Rows[row][colIndex - 3].ToString().Trim();
                                    }
                                    //D20单位转换要先记录下所有需要转换的行，等wcol填满后，再计算需要转换的行
                                }
                                else
                                {
                                    //throw
                                }
                                #endregion
                                frontContent = Units.ToDBC(frontContent);

                                string matchCode = "";
                                foreach (var item in Units.UnitList)
                                {
                                    if (frontContent.Contains(item))
                                    {
                                        //有可能多个匹配、部分匹配，记录下最长的
                                        if (item.Length > matchCode.Length)
                                            matchCode = item;
                                    }
                                }
                                //包含可能转换的单位，matchCode是excel中的单位,为空则表示无单位，去查看这个代码在程序中的单位
                                //除去IEP
                                string tUnit = "";
                                if (!rowContent.Contains("IEP"))
                                {
                                    try
                                    {
                                        var rowEntity = OilTableRowBll._OilTableRow.Where(d => new int[] { 2, 6, 7, 8 }.Contains(d.oilTableTypeID) && d.itemCode == rowContent).FirstOrDefault();
                                        if (rowEntity != null)
                                            tUnit = rowEntity.itemUnit;
                                    }
                                    catch
                                    {
                                        MessageBox.Show("未找到itemunit" + rowContent);
                                    }
                                }
                                //tUnit = _rowCache[rowContent, EnumTableType.Wide].itemUnit;
                                if (!string.IsNullOrWhiteSpace(matchCode) && !string.IsNullOrWhiteSpace(tUnit) && !tUnit.Equals(matchCode))
                                    codeUnitConvert.Add(row.ToString(), new string[] { matchCode, tUnit });
                                #endregion
                            }

                        }


                        //列
                        for (int col = firstCode[1] + 1; col < oilTable.Columns.Count; col++)
                        {
                            WCol colData = new WCol();
                            if (tableType != EnumTableType.Light)
                            {
                                #region 读取IEP行
                                if (IEPRow != -1)
                                {
                                    string colContent = oilTable.Rows[IEPRow][col].ToString().Trim();
                                    colContent = Units.ToDBC(colContent);
                                    if (colContent.Contains("原油") || colContent.Contains("分析结果"))
                                    {
                                        colData.TableType = EnumTableType.Whole;
                                    }
                                    else
                                    {
                                        //
                                        List<int> IEPNum = GetIEPNum(colContent);
                                        //有数，个数小于两个
                                        if (IEPNum.Count <= 2 && IEPNum.Count > 0)
                                        {
                                            if (IEPNum.Count == 2)
                                            {
                                                if (IEPNum[0] < IEPNum[1])
                                                {
                                                    //这一行是
                                                    if (tableType != EnumTableType.Narrow)
                                                    {
                                                        colData.TableType = EnumTableType.Wide;
                                                    }
                                                    else
                                                    {
                                                        colData.TableType = EnumTableType.Narrow;
                                                    }
                                                    colData.ICP = IEPNum[0];
                                                    colData.ECP = IEPNum[1];
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                //只得到一个值，有可能是
                                                bool isICP = true;
                                                if (colContent.Contains(">"))
                                                {//这个值是ECP
                                                    isICP = false;
                                                }
                                                else if (colContent.Contains("<"))
                                                {
                                                    isICP = true;
                                                }
                                                else if (colContent.Contains("~"))
                                                {
                                                    int signIndex = colContent.IndexOf("~");
                                                    //--------------------------获得一个 为数字的值
                                                    Regex regex = new Regex(@"\d");
                                                    int numIndex = regex.Match(colContent).Index;
                                                    if (signIndex < numIndex)
                                                    {
                                                        isICP = false;
                                                    }
                                                    else
                                                    {
                                                        isICP = true;
                                                    }
                                                }
                                                else if (colContent.Contains("-"))
                                                {
                                                    int signIndex = colContent.IndexOf("-");
                                                    //--------------------------获得一个 为数字的值
                                                    Regex regex = new Regex(@"\d");
                                                    int numIndex = regex.Match(colContent).Index;
                                                    if (signIndex < numIndex)
                                                    {
                                                        isICP = false;
                                                    }
                                                    else
                                                    {
                                                        isICP = true;
                                                    }
                                                }
                                                if (tableType != EnumTableType.Narrow)
                                                {
                                                    if (colContent.Contains("+") || colContent.Contains(">"))
                                                    {
                                                        //渣油
                                                        colData.TableType = EnumTableType.Residue;
                                                        colData.ICP = IEPNum[0];
                                                    }
                                                    else
                                                    {
                                                        colData.TableType = EnumTableType.Wide;
                                                        if (isICP == true)
                                                        {
                                                            colData.ICP = IEPNum[0];
                                                        }
                                                        else
                                                        {
                                                            colData.ECP = IEPNum[0];
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    colData.TableType = EnumTableType.Narrow;
                                                    if (isICP == true)
                                                    {
                                                        colData.ICP = IEPNum[0];
                                                    }
                                                    else
                                                    {
                                                        colData.ECP = IEPNum[0];
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                #endregion

                                #region 查ICP 和 ECP 的数据
                                if (ICPRow != -1 && colData.TableType == EnumTableType.None)
                                {
                                    string ICPContent = oilTable.Rows[ICPRow][col].ToString().Trim();
                                    string ECPContent = "";
                                    if(ECPRow!=-1)
                                        ECPContent = oilTable.Rows[ECPRow][col].ToString().Trim();
                                    ICPContent = Units.ToDBC(ICPContent);
                                    ECPContent = Units.ToDBC(ECPContent);


                                    if (IsInt(ICPContent) && IsInt(ECPContent))
                                    {
                                        if (int.Parse(ICPContent) < int.Parse(ECPContent))
                                        {
                                            if (tableType != EnumTableType.Narrow)
                                            {
                                                colData.TableType = EnumTableType.Wide;
                                            }
                                            else
                                            {
                                                colData.TableType = EnumTableType.Narrow;
                                            }
                                            colData.ICP = int.Parse(ICPContent);
                                            colData.ECP = int.Parse(ECPContent);
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else if (IsInt(ICPContent))
                                    {
                                        if (tableType != EnumTableType.Narrow)//渣油
                                        {
                                            colData.TableType = EnumTableType.Residue;
                                        }
                                        else
                                        {
                                            colData.TableType = EnumTableType.Narrow;
                                        }
                                        colData.ICP = int.Parse(ICPContent);
                                    }
                                    else if (IsInt(ECPContent))
                                    {
                                        if (tableType != EnumTableType.Narrow)
                                        {
                                            colData.TableType = EnumTableType.Wide;
                                        }
                                        else
                                        {
                                            colData.TableType = EnumTableType.Narrow;
                                        }
                                        colData.ECP = int.Parse(ECPContent);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                colData.TableType = EnumTableType.Light;
                            }

                            //记录轻端itemcode
                            List<string> lightItemtCode = new List<string>();
                            //for(int row = ) 一列一列循环单元格，放入List<wcol>中
                            for (int row = firstCode[0]; row < oilTable.Rows.Count; row++)
                            {
                                //if()
                                if (row == IEPRow||row==ECPRow||row==ICPRow)
                                {
                                    continue;
                                }
                                string content = oilTable.Rows[row][col].ToString().Trim();
                                content = Units.ToDBC(content);
                                if (tableType == EnumTableType.Light)
                                {
                                    if(!string.IsNullOrWhiteSpace(itemCodeList[row]))
                                    {
                                        if (lightItemtCode.Contains(itemCodeList[row]))
                                        {
                                            if (colData.Cells.Count() != 0)
                                                oilTablesData.Add(colData);
                                            lightItemtCode.Clear();
                                            lightItemtCode.Add(itemCodeList[row]);
                                            colData = new WCol();
                                            colData.TableType = EnumTableType.Light;
                                        }
                                        else
                                        {
                                            lightItemtCode.Add(itemCodeList[row]);
                                        }
                                    }

                                }
                                if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(itemCodeList[row]))
                                {
                                    continue;
                                }
                                WCell cell = new WCell();
                                cell.ItemCode = itemCodeList[row];//itemcodelist中存有空值，根据content不为空，则存入coldata中的都不为空

                                //需要单位转换的
                                if (codeUnitConvert.ContainsKey(row.ToString()))
                                {
                                    string[] tempstr = codeUnitConvert[row.ToString()];
                                    double dContent = double.Parse(content);
                                    Units.UnitConvert(ref dContent, tempstr[0], tempstr[1]);
                                    content = dContent.ToString();
                                }

                                cell.LabData = content;
                                colData.Add(cell);
                            }
                            if(colData.Cells.Count()!=0)
                                oilTablesData.Add(colData);
                        }
                        #region 注释
                        //宽馏分处理 oiltable，下边是代码列
                        //for (int col = firstCode[1]; col < oilTable.Columns.Count; col++)
                        //{
                        //    OilTableColEntity colEntity = new OilTableColEntity();
                        //    EnumTableType tableType2 = EnumTableType.None;
                        //    for (int row = firstCode[0]; row < oilTable.Rows.Count; row++)
                        //    {
                        //        string labData = oilTable.Rows[row][col].ToString().Trim();
                        //        if (string.IsNullOrWhiteSpace(labData))
                        //            continue;
                        //        //新一列的开始，要判断是什么油。
                        //        if (row == firstCode[0])
                        //        {
                        //            if (labData.Contains("原油"))
                        //            {
                        //                //原油性质表
                        //                colEntity = _colCache["Cut1", EnumTableType.Whole];
                        //                tableType2 = EnumTableType.Whole;
                        //            }
                        //            //todo 解析温度，此处根据温度判断表类型，并得到列实体
                        //            if (false)
                        //            {

                        //            }
                        //            continue;//此行不是数据，转到下一行
                        //        }
                        //        OilTableRowEntity rowEntity = _rowCache[itemCode, tableType2];
                        //        string unit = rowEntity.itemUnit;//得到程序中的单位
                        //        #region todo 判断单位是否需要转换，并进行转换
                        //        #endregion


                        //        OilTools tools = new OilTools();
                        //        if (rowEntity.decNumber != null) labData = tools.calDataDecLimit(labData, rowEntity.decNumber + 2, rowEntity.valDigital); //输入Execl表的过程中转换数据精度
                        //        else labData = tools.calDataDecLimit(labData, rowEntity.decNumber, rowEntity.valDigital); //输入Execl表的过程中转换数据精度

                        //        OilDataEntity oilData = new OilDataEntity();
                        //        oilData.oilInfoID = oilInfo.ID;
                        //        oilData.oilTableColID = colEntity.ID;
                        //        oilData.oilTableRowID = rowEntity.ID;
                        //        oilData.labData = labData;
                        //        oilData.calData = labData;
                        //        oilInfo.OilDatas.Add(oilData);
                        //    }
                        //}
                        #endregion
                    }
                    #endregion
                }//end foreach table

                //ICP ECP一样的合并 只对宽馏分进行
                //foreach (var col in oilTablesData.Where(d => d.TableType == EnumTableType.Wide))
                //{
                    
                //}
                var items = oilTablesData.Where(d => d.TableType == EnumTableType.Wide).GroupBy(x => new { x.ICP, x.ECP }).Select(d => new {keys = d.Key,Count = d.Count()});

                foreach (var i in items)
                {
                    if (i.Count > 1)
                    {
                        List<WCol> mergeList = oilTablesData.Where(d => d.TableType == EnumTableType.Wide && d.ICP == i.keys.ICP && d.ECP == i.keys.ECP).ToList();
                        WCol newCol = new WCol();
                        newCol.ECP = i.keys.ECP;
                        newCol.ICP = i.keys.ICP;
                        newCol.TableType = EnumTableType.Wide;
                        foreach (var mergeCol in mergeList)
                        {
                            newCol.Cells.AddRange(mergeCol.Cells);
                            oilTablesData.Remove(mergeCol);
                        }
                        oilTablesData.Add(newCol);
                    }
                }


                //排序，找出宽馏分的，根据iep排序，渣油也排序
                List<WCol> wideData = oilTablesData.Where(d => d.TableType == EnumTableType.Wide).OrderBy(d => d.MCP).ToList();
                WriteToOilA(ref oilInfo, wideData, EnumTableType.Wide);

                wideData = oilTablesData.Where(d => d.TableType == EnumTableType.Whole).ToList();
                WriteToOilA(ref oilInfo, wideData, EnumTableType.Whole);

                wideData = oilTablesData.Where(d => d.TableType == EnumTableType.Narrow).ToList();
                WriteToOilA(ref oilInfo, wideData, EnumTableType.Narrow);

                wideData = oilTablesData.Where(d => d.TableType == EnumTableType.Residue).OrderBy(d => d.MCP).ToList();
                WriteToOilA(ref oilInfo, wideData, EnumTableType.Residue);

                wideData = oilTablesData.Where(d => d.TableType == EnumTableType.Light).ToList();
                WriteToOilA(ref oilInfo, wideData, EnumTableType.Light);


                #region 注释
                //    #region 原油基本信息
                //    DataRow dr = ds.Tables["原油信息"].NewRow();  //读取的第一行DataTable当成表头
                //    dr[1] = ds.Tables["原油信息"].Columns[1];
                //    dr[2] = ds.Tables["原油信息"].Columns[2];
                //    ds.Tables["原油信息"].Rows.Add(dr);
                //    for (int i = 0; i < ds.Tables["原油信息"].Rows.Count; i++)
                //    {
                //        itemCode = ds.Tables["原油信息"].Rows[i][1].ToString().Trim();
                //        string value = ds.Tables["原油信息"].Rows[i][2].ToString().Trim();
                //        oilInfoAddItem(ref oilInfo, itemCode, value);
                //    }
                //    #endregion
                //    oilInfoID = saveInfo(oilInfo);
                //    oilInfo.ID = oilInfoID;
                //    if (oilInfoID == -1)
                //    {
                //        return oilInfo;
                //    }

                //    OilTableRowBll rowBll = new OilTableRowBll();
                //    OilTableColBll colBll = new OilTableColBll();
                //    try
                //    {
                //        #region 原油性质表

                //        getSheetData(ds.Tables["原油性质"], ref oilInfo, 1, 2, 3, EnumTableType.Whole);

                //        #endregion
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.Error("数据管理,导入Excel到库A 原油性质表出问题" + ex);
                //    }
                //    int colID = 0;//
                //    OilTools oilTools = new OilTools();
                //    try
                //    {
                //        Application.DoEvents();
                //        #region 轻端-气体
                //        List<string> itemCodelist = new List<string>();//用来判断是哪一个表

                //        //??两列 对应的id
                //        int colID1 = colBll["Cut1", EnumTableType.Light].ID;
                //        int colID2 = colBll["Cut2", EnumTableType.Light].ID;


                //        int rowCount = ds.Tables["气体"] == null ? 0 : ds.Tables["气体"].Rows.Count;

                //        for (int i = 0; i < rowCount; i++)
                //        {
                //            itemCode = ds.Tables["气体"].Rows[i][1].ToString().Trim(); //在行中获取代码
                //            if (!itemCodelist.Contains(itemCode))//判断itemcode是否出现过
                //            {
                //                itemCodelist.Add(itemCode);
                //                colID = colID1;
                //            }
                //            else
                //            {
                //                colID = colID2;
                //            }

                //            OilTableRowEntity row = _rowCache[itemCode, EnumTableType.Light]; //获取代码的ID
                //            if (row != null)
                //            {
                //                string labData = ds.Tables["气体"].Rows[i][2].ToString().Trim();
                //                if (row.decNumber != null)
                //                    labData = oilTools.calDataDecLimit(labData, row.decNumber + 2, row.valDigital);//输入Execl表的过程中转换数据精度
                //                else
                //                    labData = oilTools.calDataDecLimit(labData, row.decNumber, row.valDigital);//输入Execl表的过程中转换数据精度                         
                //                //labData = oilTools.calDataDecLimit(labData, row.decNumber);//输入Execl表的过程中转换数据精度   

                //                OilDataEntity oilData = new OilDataEntity();
                //                oilData.oilInfoID = oilInfoID;
                //                oilData.oilTableColID = colID;
                //                if (i <= 17)//??
                //                    oilData.oilTableColID = colID;
                //                else
                //                    oilData.oilTableColID = colID2;
                //                oilData.oilTableRowID = row.ID;
                //                oilData.labData = labData;
                //                oilData.calData = labData;
                //                oilInfo.OilDatas.Add(oilData);
                //            }
                //        }

                //        #endregion
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.Error("数据管理,导入Excel到库A 轻端表出问题" + ex);
                //    }
                //    try
                //    {
                //        #region 窄馏分-实沸点

                //        #region step1 把表中代码对应的ID存在列表中(第3行是代码 )
                //        List<int> arrRow = new List<int>();  //存行ID
                //        int colSul = -1, colN2 = -1; // colSul记录硫含量%的列，氮含量%的列
                //        int colSulValue = -1, colN2Value = -1; // colSul记录硫含量值的列，氮含量值的列
                //        //ds.Tables.Contains("实沸点")==false
                //        var columnCount = ds.Tables["实沸点"] == null || ds.Tables["实沸点"].Rows.Count <= 3 ? 0 : ds.Tables["实沸点"].Columns.Count;
                //        for (int i = 0; i < columnCount; i++)
                //        {
                //            itemCode = ds.Tables["实沸点"].Rows[3][i].ToString().Trim();
                //            if (itemCode != "")
                //            {
                //                OilTableRowEntity row = rowBll[itemCode, EnumTableType.Narrow];
                //                if (row != null)
                //                {
                //                    var prvRowValue = ds.Tables["实沸点"].Rows[1][i].ToString().Trim();
                //                    if (string.IsNullOrWhiteSpace(prvRowValue))
                //                        prvRowValue = ds.Tables["实沸点"].Rows[2][i].ToString().Trim();
                //                    bool isPercentage = prvRowValue.Contains('%') || prvRowValue.Contains('％');
                //                    if (itemCode == "SUL")
                //                    {
                //                        if (isPercentage)
                //                            colSul = i;
                //                        else
                //                            colSulValue = i;
                //                    }
                //                    else if (itemCode == "N2")
                //                    {
                //                        if (isPercentage)
                //                            colN2 = i;
                //                        else
                //                            colN2Value = i;
                //                    }
                //                    arrRow.Add(row.ID);
                //                }
                //                else
                //                    arrRow.Add(-1); //可能包含其他符号，不是itemCode
                //            }
                //            else
                //            {
                //                arrRow.Add(-1);
                //            }
                //        }
                //        #endregion

                //        int rowsCount = 0;

                //        #region "判断有多少行"
                //        int colCount = ds.Tables["实沸点"] == null ? 0 : ds.Tables["实沸点"].Columns.Count;
                //        for (int i = 0; i < colCount; i++)
                //        {
                //            string strItemCode = ds.Tables["实沸点"].Rows[3][i].ToString().Trim();

                //            if (strItemCode == "ICP")
                //            {
                //                for (int j = 4; j < ds.Tables["实沸点"].Rows.Count; j++)
                //                {
                //                    string dataICP = ds.Tables["实沸点"].Rows[j][i].ToString().Trim();
                //                    string dataNext = ds.Tables["实沸点"].Rows[j][i + 1].ToString().Trim();
                //                    string dataECP = ds.Tables["实沸点"].Rows[j][i + 2].ToString().Trim();
                //                    if (dataICP == string.Empty && dataNext == ">")
                //                    {
                //                        rowsCount = j - 1;
                //                        break;
                //                    }
                //                    else if (dataICP == string.Empty && dataECP == string.Empty)
                //                    {
                //                        rowsCount = j - 1;
                //                        break;
                //                    }
                //                    else if (dataECP == string.Empty)
                //                    {
                //                        rowsCount = j - 1;
                //                        break;
                //                    }
                //                    if (j == ds.Tables["实沸点"].Rows.Count - 1 && dataICP != string.Empty && dataECP != string.Empty)
                //                    {
                //                        rowsCount = j;
                //                    }

                //                }
                //            }
                //        }



                //        #endregion

                //        #region step2: 遍历每行每列，如果单元格不空则添加数据
                //        for (int i = 4; i <= rowsCount; i++)
                //        {
                //            colID = colBll["Cut" + (i - 3).ToString(), EnumTableType.Narrow].ID; //真实表格中的一行是一个Cut
                //            for (int x = 0; x < arrRow.Count; x++)
                //            {
                //                if (arrRow[x] > 0)
                //                {
                //                    var t = ds.Tables["实沸点"].Rows[i][x];
                //                    string labData = ds.Tables["实沸点"].Rows[i][x].ToString().Trim(); //取一个单元格值   

                //                    if (string.IsNullOrWhiteSpace(labData))
                //                        continue;
                //                    labData = labData.Trim();

                //                    OilTableRowEntity row = rowBll[arrRow[x], (int)EnumTableType.Narrow];

                //                    #region 如果上一列μg/g为单位的SUL存在，找到其oilData，转化为%单位


                //                    if (colSul == x)//找到列为单位%的SUL列
                //                    {
                //                        OilDataEntity oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Narrow && c.OilTableRow.itemCode == "SUL" && c.oilTableColID == colID).FirstOrDefault();
                //                        if (oilDataTemp != null) //如果前一个单元格不空，进行单位转化
                //                            oilInfo.OilDatas.Remove(oilDataTemp);
                //                    }
                //                    else if (colSulValue == x)
                //                    {
                //                        OilDataEntity oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Narrow && c.OilTableRow.itemCode == "SUL" && c.oilTableColID == colID).FirstOrDefault();
                //                        if (oilDataTemp != null)
                //                            continue;
                //                        int decNum = row.decNumber == null ? row.valDigital : row.decNumber.Value;
                //                        if (!oilTools.unitUgTo(ref labData, decNum)) //转化数据单位,如果不能转化，修改值为空
                //                            continue;
                //                    }
                //                    #endregion

                //                    //if (labData.Length > 12) //可能是一些备注，暂时这样做
                //                    //    continue;

                //                    #region 如果当前单元格%为单位的N2,转化为μg/g单位，否则跳过该单元格
                //                    if (colN2 == x)//为μg/g单位 %
                //                    {
                //                        OilDataEntity oilDataTemp;
                //                        oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Narrow && c.OilTableRow.itemCode == "N2" && c.oilTableColID == colID).FirstOrDefault();
                //                        if (oilDataTemp != null) //如果前一个单元格不空，则跳过
                //                            continue;
                //                        int decNum = row.decNumber == null ? row.valDigital : row.decNumber.Value;
                //                        if (!oilTools.unitToUg(ref labData, decNum)) //转化数据单位,如果不能转化，修改值为空
                //                            continue;
                //                    }
                //                    else if (colN2Value == x)
                //                    {
                //                        var oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Narrow && c.OilTableRow.itemCode == "N2" && c.oilTableColID == colID).FirstOrDefault();
                //                        if (oilDataTemp != null) //如果前一个单元格不空，则跳过
                //                            oilInfo.OilDatas.Remove(oilDataTemp);
                //                    }
                //                    #endregion
                //                    if (row.decNumber != null)
                //                        labData = oilTools.calDataDecLimit(labData, row.decNumber + 2, row.valDigital);//输入Execl表的过程中转换数据精度
                //                    else
                //                        labData = oilTools.calDataDecLimit(labData, row.decNumber, row.valDigital);//输入Execl表的过程中转换数据精度                         

                //                    //labData = oilTools.calDataDecLimit(labData, row.decNumber, row.valDigital);//输入Execl表的过程中转换数据精度        

                //                    OilDataEntity oilData = new OilDataEntity();
                //                    oilData.oilInfoID = oilInfoID;
                //                    oilData.oilTableColID = colID;
                //                    oilData.oilTableRowID = arrRow[x];
                //                    oilData.labData = labData;
                //                    oilData.calData = labData;
                //                    oilInfo.OilDatas.Add(oilData);
                //                }
                //            }
                //        }
                //        #endregion

                //        #endregion
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.Error("数据管理,导入Excel到库A 窄馏分表出问题" + ex);
                //    }

                //    try
                //    {
                //        #region 渣油
                //        getSheetData(ds.Tables["渣油"], ref oilInfo, 1, 2, 3, EnumTableType.Residue);
                //        #endregion
                //    }
                //    catch(Exception ex)
                //    {
                //        Log.Error("数据管理,导入Excel到库A 渣油表出问题" + ex);
                //    }
                //    try
                //    {
                //        #region 宽馏分

                //    //WCutTypeAccess acess = new WCutTypeAccess();
                //    // List<WCutTypeEntity> wCutTypes = acess.Get("1=1").ToList();  //宽馏分cut的名称
                //    S_ParmBll s_ParmBll = new S_ParmBll();
                //    List<S_ParmEntity> wCutTypes = s_ParmBll.GetParms("WCT");

                //    int cutCount = 0; //宽馏分cut数目
                //    int oldCount = 0;

                //    getSheetData(ds.Tables["重整料"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "重整料").FirstOrDefault().parmValue);
                //    getSheetData(ds.Tables["石脑油"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "石脑油").FirstOrDefault().parmValue);
                //    getSheetData(ds.Tables["航煤"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "航煤").FirstOrDefault().parmValue);
                //    oldCount = cutCount;
                //    getSheetData(ds.Tables["柴油"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "柴油").FirstOrDefault().parmValue);
                //    getSheetData(ds.Tables["柴油质谱"], ref oldCount, ref oilInfo, 1, 1, 2, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "柴油").FirstOrDefault().parmValue);
                //    getSheetData(ds.Tables["重油质谱"], ref oldCount, ref oilInfo, 1, 1, 2, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "柴油").FirstOrDefault().parmValue);

                //    getSheetData(ds.Tables["溶剂油"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "溶剂油").FirstOrDefault().parmValue);
                //    oldCount = cutCount;
                //    //getSheetData(ds.Tables["VGO"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "VGO").FirstOrDefault().parmValue);
                //    getSheetData(ds.Tables["VGO"], ref cutCount, ref oilInfo, 1, 2, 3, EnumTableType.Wide, wCutTypes.Where(c => c.parmName == "蜡油").FirstOrDefault().parmValue);
                //    #endregion
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.Error("数据管理,导入Excel到原始库 宽馏分表出问题" + ex);
                //    }

                //    //saveTables(oilInfo);

                //    //OilInfoEntity retOilInfoA = OilBll.GetOilById(oilInfo.ID);
                //    //return retOilInfoA;
                //    return oilInfo;//考虑到导入Excel的数据不稳定
#endregion
            }
            catch (Exception ex)
            {
                Log.Error("数据管理,镇海导入Excel到库原始库:" + ex);
                return null;
            }

            return oilInfo;
        }

        private static void WriteToOilA(ref OilInfoEntity oilInfo,List<WCol> Data,EnumTableType tableType)
        {
            try
            {
                int colNum = 0;
                foreach (var col in Data)
                {
                    //得到此列的列号
                    colNum++;
                    int colID = 0;
                    //如果是轻端 则变为“质量” “体积”
                    //if (tableType == EnumTableType.Light)
                    //{
                    //    if (colNum == 1)
                    //        colID = _colCache["质量含量", tableType].ID;
                    //    else
                    //        colID = _colCache["体积含量", tableType].ID;
                    //}
                    //else
                    //{
                    try
                    {
                        colID = _colCache["Cut" + colNum.ToString(), tableType].ID;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("获取不到colID" + colNum.ToString());
                        Log.Error("导入EXCEl："+ex.ToString());
                    }
                    //}

                    //添加icp ecp
                    if (tableType == EnumTableType.Narrow || tableType == EnumTableType.Wide || tableType == EnumTableType.Residue)
                    {
                        if (tableType == EnumTableType.Narrow || tableType == EnumTableType.Wide)
                        {
                            var ICProw = _rowCache["ICP", tableType];
                            OilDataEntity oilDataICP = new OilDataEntity();
                            oilDataICP.oilInfoID = oilInfo.ID;
                            oilDataICP.oilTableColID = colID;
                            oilDataICP.oilTableRowID = ICProw.ID;
                            string data = col.ICP.ToString();
                            oilDataICP.labData = data;
                            oilDataICP.calData = data;
                            oilInfo.OilDatas.Add(oilDataICP);
                            var ECProw = _rowCache["ECP", tableType];
                            OilDataEntity oilDataECP = new OilDataEntity();
                            oilDataECP.oilInfoID = oilInfo.ID;
                            oilDataECP.oilTableColID = colID;
                            oilDataECP.oilTableRowID = ECProw.ID;
                            if (col.ECP == 0)
                                data = "";
                            else
                                data = col.ECP.ToString();
                            oilDataECP.labData = data;
                            oilDataECP.calData = data;
                            oilInfo.OilDatas.Add(oilDataECP);
                        }
                        if (tableType == EnumTableType.Residue)
                        {
                            var ICProw = _rowCache["ICP", tableType];
                            OilDataEntity oilDataICP = new OilDataEntity();
                            oilDataICP.oilInfoID = oilInfo.ID;
                            oilDataICP.oilTableColID = colID;
                            oilDataICP.oilTableRowID = ICProw.ID;
                            string data = col.ICP.ToString();
                            oilDataICP.labData = data;
                            oilDataICP.calData = data;
                            oilInfo.OilDatas.Add(oilDataICP);
                        }
                    }

                    //如果是“宽馏分”和“渣油” 温度判断是什么油{    宽 和 渣 才有 WCT
                    if (tableType == EnumTableType.Wide || tableType == EnumTableType.Residue)
                    {
                        string oilType = GetOilType(col.ICP, col.ECP);
                        //得到wct的种类
                        S_ParmBll s_ParmBll = new S_ParmBll();
                        //todo 如果是“渣油”变为rct
                        List<S_ParmEntity> wCutTypes;
                        if (tableType == EnumTableType.Residue)
                            wCutTypes = s_ParmBll.GetParms("RCT");
                        else
                        {
                            wCutTypes = s_ParmBll.GetParms("WCT");
                        }
                        string WCT = "";
                        try
                        {
                            WCT = wCutTypes.Where(c => c.parmName == oilType).FirstOrDefault().parmValue;
                        }
                        catch
                        {
                            MessageBox.Show("ICP:"+col.ICP+"ECP:"+col.ECP+"未找到对应原油类型");
                        }
                        //宽馏分的wct行
                        var rowWCT = _rowCache["WCT", tableType];
                        OilDataEntity oilDataWCT = new OilDataEntity();
                        oilDataWCT.oilInfoID = oilInfo.ID;
                        oilDataWCT.oilTableColID = colID;
                        oilDataWCT.oilTableRowID = rowWCT.ID;
                        string data = WCT;
                        if (data.Length > 12)
                            data = data.Substring(0, 12);
                        oilDataWCT.labData = data;
                        oilDataWCT.calData = data;
                        oilInfo.OilDatas.Add(oilDataWCT);
                        //}
                    }
                    foreach (var item in col.Cells)
                    {
                        OilTools tools = new OilTools();
                        //根据itemcode得到行id

                        OilTableRowEntity rowEntity = new OilTableRowEntity();
                        try
                        {
                            if (item.ItemCode == "TYP")
                            {
                                //rowEntity = _rowCache["TYP", EnumTableType.Info];
                                oilInfo.type = item.LabData;
                                //更新原油信息
                                saveInfo(oilInfo);
                                continue;
                            }
                            else
                            {
                                rowEntity = _rowCache[item.ItemCode, tableType];
                            }
                            if (rowEntity == null)
                            {
                                continue;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("rowEntity获取失败");
                        }
                        OilDataEntity oilData = new OilDataEntity();
                        oilData.oilInfoID = oilInfo.ID;
                        oilData.oilTableColID = colID;
                        oilData.oilTableRowID = rowEntity.ID;
                        string labData;
                        try
                        {
                            if (rowEntity.decNumber != null)
                                labData = tools.calDataDecLimit(item.LabData, rowEntity.decNumber + 2, rowEntity.valDigital);//输入Execl表的过程中转换数据精度
                            else
                                labData = tools.calDataDecLimit(item.LabData, rowEntity.decNumber, rowEntity.valDigital);//输入Execl表的过程中转换数据精度
                        }
                        catch
                        {
                            MessageBox.Show("lab:" + item.LabData);
                        }
                        oilData.labData = item.LabData;
                        oilData.calData = item.LabData;
                        oilInfo.OilDatas.Add(oilData);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("erro:" + ex);
            }
        }
        /// <summary>
        /// 根据温度获取输入宽馏分的列的油种类，主要用于镇海格式。
        /// </summary>
        /// <param name="ICP"></param>
        /// <param name="ECP"></param>
        /// <returns></returns>
        private static string GetOilType(int ICP,int ECP)
        {
            //等于0时是渣油
            if (ECP != 0)
            {
                if ((ICP == 0 && ECP <= 200) || (ECP <= 200 && (ECP + ICP) / 2 < 150))
                {
                    return "石脑油";
                }
                else if (ECP <= 260 && 150 <= (ECP + ICP) / 2 && (ECP + ICP) / 2 <= 200)
                {
                    return "航煤";
                }
                else if (ECP <= 360 && 200 < (ECP + ICP) / 2 && (ECP + ICP) / 2 <= 350)
                {
                    return "柴油";
                }
                else if (ECP <= 560 && 350 < (ECP + ICP) / 2 && (ECP + ICP) / 2 <= 560)
                {
                    return "蜡油";
                }
            }
            else if (ICP <= 400 && ECP == 0)
            {
                return "常渣";
            }
            else if (ICP >= 400 && ECP == 0)
            {
                return "减渣";
            }
            return "";
        }

        public static bool IsInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        private static List<int> GetIEPNum(string content)
        {
            List<int> result = new List<int>();
            try
            { 
                int t = 0;
                string str = content;
                for (int i = 0; i < str.Length; i++)
                {
                    int tag = 0;
                    while (char.IsDigit(str, i))
                    {
                        tag = 1;
                        t = t * 10 + Convert.ToInt32(str[i].ToString());
                        i++;
                    }
                    if (tag == 1)
                    {
                        result.Add(t);
                        t = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return result;
        }
        /// <summary>
        /// 找到表格中的第一个itemcode的位置
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        //private static int[] FindFirstItemCode(List<string> oilInfoItemCode, List<string> allItemCode, DataTable table)
        //{

        //    int[] firstCode = new int[2]{-1,-1};
        //    for (int r_num = 0; r_num < table.Rows.Count; r_num++)
        //    {
        //        for (int c_num = 0; c_num < table.Columns.Count; c_num++)
        //        {
        //            string cellContent = table.Rows[r_num][c_num].ToString();

        //            if (oilInfoItemCode.Contains(cellContent))
        //            {
        //                //原油信息表 第一个代码
        //                firstCode[0] = r_num;
        //                firstCode[1] = c_num;
        //                return firstCode;
        //            }

        //            //包含ICP，或在allItemCode中
        //            if (cellContent.Contains("IEP") || cellContent.Contains("ICP") || allItemCode.Contains(cellContent))
        //            {
        //                //找到第一个代码
        //                firstCode[0] = r_num;
        //                firstCode[1] = c_num;
        //                return firstCode;
        //            }
        //        }
        //    }

        //    return firstCode;
        //}


        /// <summary>
        /// 把导入的EXCEl 的数据转换成可以插入的原油信息元素
        /// </summary>
        /// <param name="oilInfo"></param>
        /// <param name="itemCode"></param>
        /// <param name="value"></param>
        public static void oilInfoAddItem(ref OilInfoEntity oilInfo, string itemCode, string value)
        {
            List<OilTableRowEntity> rowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Info).ToList();
            OilTableRowEntity rowEntity = rowList.Where(o => o.itemCode == itemCode).FirstOrDefault();
            OilDataCheck dataCheck = new OilDataCheck();
            switch (itemCode)
            {
                #region "10"
                case "CNA":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 50);
                    oilInfo.crudeName = value;
                    break;
                case "ENA":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 50);
                    oilInfo.englishName = value;
                    break;
                case "IDC":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.crudeIndex = value;
                    break;
                case "COU":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.country = value;
                    break;
                case "GRC":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.region = value;
                    break;


                case "FBC":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 50);
                    oilInfo.fieldBlock = value;
                    break;
                case "SDA":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 20);
                    oilInfo.sampleDate = dataCheck.GetDate(value);
                    break;
                case "RDA":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 20);
                    oilInfo.receiveDate = dataCheck.GetDate(value);
                    break;
                case "SS":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 50);
                    oilInfo.sampleSite = value;
                    break;
                case "ADA":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 20);
                    oilInfo.assayDate = value;
                    break;

                #endregion

                #region "10"
                case "UDD":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 20);
                    oilInfo.updataDate = value;
                    break;
                case "SR":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 50);
                    oilInfo.sourceRef = value;
                    break;
                case "ALA":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.assayLab = value;
                    break;
                case "AER":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.assayer = value;
                    break;
                case "ASC":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.assayCustomer = value;
                    break;


                case "RIN":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 50);
                    oilInfo.reportIndex = value;
                    break;
                case "SUM":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 1000);
                    oilInfo.summary = value;
                    break;
                case "CLA":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.type = value;
                    break;
                case "TYP":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.classification = value;
                    break;
                case "SCL":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.sulfurLevel = value;
                    break;

                #endregion

                #region "10"
                case "ACL":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.acidLevel = value;
                    break;
                case "COL":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.corrosionLevel = value;
                    break;
                case "NIR":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 50);
                    oilInfo.NIRSpectrum = value;
                    break;
                case "BLN":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 100);
                    oilInfo.BlendingType = value;
                    break;
                case "DQU":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 2);
                    oilInfo.DataQuality = value;
                    break;


                case "SRI":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 10);
                    oilInfo.DataSource = value;
                    break;

                case "01R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_01R = value;
                    break;
                case "02R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_02R = value;
                    break;
                case "03R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_03R = value;
                    break;
                case "04R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_04R = value;
                    break;

                case "05R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_05R = value;
                    break;
                case "06R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_06R = value;
                    break;
                case "07R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_07R = value;
                    break;
                case "08R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_08R = value;
                    break;
                case "09R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);
                    oilInfo.S_09R = value;
                    break;
                case "10R":
                    if (value.Length > rowEntity.decNumber && rowEntity != null)
                        value = value.Substring(0, rowEntity.valDigital);
                    else if (value.Length > rowEntity.decNumber && rowEntity == null)
                        value = value.Substring(0, 200);

                    oilInfo.S_10R = value;
                    break;

                #endregion
            }
        }

        private static void oilInfoAddItem(ref OilInfoBEntity oilInfo, string itemCode, string value)
        {
            OilDataCheck dataCheck = new OilDataCheck();
            #region
            if (itemCode == "CNA")
            {
                oilInfo.crudeName = value;
                oilInfo.crudeIndex = value;
            }
            else if (itemCode == "ENA")
            {
                oilInfo.englishName = value;
            }
            if (itemCode == "IDC")
            {
                oilInfo.crudeIndex = value;
            }
            else if (itemCode == "COU")
            {
                oilInfo.country = value;
            }
            if (itemCode == "GRC")
            {
                oilInfo.region = value;
            }
            else if (itemCode == "FBC")
            {
                oilInfo.fieldBlock = value;
            }
            if (itemCode == "SDA")
            {
                oilInfo.sampleDate = dataCheck.GetDate(value);
            }
            else if (itemCode == "RDA")
            {
                oilInfo.receiveDate = dataCheck.GetDate(value);
            }
            if (itemCode == "SS")
            {
                oilInfo.sampleSite = value;
            }
            else if (itemCode == "ADA")
            {
                oilInfo.assayDate = value;
            }
            if (itemCode == "UDD")
            {
                oilInfo.updataDate = value;
            }
            else if (itemCode == "SR")
            {
                oilInfo.sourceRef = value;
            }
            else if (itemCode == "ALA")
            {
                oilInfo.assayLab = value;
            }
            else if (itemCode == "AER")
            {
                oilInfo.assayer = value;
            }
            if (itemCode == "ASC")
            {
                oilInfo.assayCustomer = value;
            }
            else if (itemCode == "RIN")
            {
                oilInfo.reportIndex = value;
            }
            else if (itemCode == "SUM")
            {
                oilInfo.summary = value;
            }
            else if (itemCode == "CLA")
            {
                oilInfo.type = value;
            }
            else if (itemCode == "TYP")
            {
                oilInfo.classification = value;
            }
            else if (itemCode == "SCL")
            {
                oilInfo.sulfurLevel = value;
            }
            else if (itemCode == "ACL")
            {
                oilInfo.acidLevel = value;
            }
            else if (itemCode == "COL")
            {
                oilInfo.corrosionLevel = value;
            }
            else if (itemCode == "PRI")
            {
                oilInfo.processingIndex = value;
            }
            #endregion
        }

        /// <summary>
        /// 针对宽馏分表，根据Excel的sheet生成的DataTable提取数据。
        /// </summary>
        /// <param name="dt">根据Excel的sheet生成的DataTable</param>
        ///<param name="cutCount">列数</param>
        /// <param name="oilInfo">原油</param>
        /// <param name="rowStart">数据开始行</param>
        /// <param name="codeCol">代码列</param>
        /// <param name="colStart">数据开始列</param>
        /// <param name="tableType">表类别</param>
        private static void getSheetData(DataTable dt, ref int cutCount, ref OilInfoEntity oilInfo, int rowStart, int codeCol, int colStart, EnumTableType tableType, string wctType)
        {
            string itemCode;

            if (dt == null)
                return;

            #region step1：计算出该表有多少有数据的列

            List<int> arrColID = new List<int>();  //选把列Id算出 
            for (int i = rowStart; i < dt.Rows.Count; i++) //找到ICP行，根据ICP或ECP不为空判断该列有数据，列的计数加1
            {
                itemCode = dt.Rows[i][codeCol].ToString().Trim(); //在行中，通过代码列获取代码
                if (itemCode == "ICP")
                {
                    for (int x = colStart; x < dt.Columns.Count; x++)
                    {
                        if (dt.Rows[i][x].ToString().Trim() != "" || dt.Rows[i + 1][x].ToString().Trim() != "")
                            arrColID.Add(_colCache["Cut" + (++cutCount).ToString(), EnumTableType.Wide].ID);
                    }
                    break;
                }
            }

            #endregion

            #region step2: 添加WCT的单元格，WCT与Sheet相关
            //此行是馏分类型wct是类型
            var rowWCT = _rowCache["WCT", EnumTableType.Wide];
            if (rowWCT == null)
                return;
            for (int x = 0; x < arrColID.Count; x++)
            {
                OilDataEntity oilData = new OilDataEntity();
                oilData.oilInfoID = oilInfo.ID;
                oilData.oilTableColID = arrColID[x];
                oilData.oilTableRowID = rowWCT.ID;     //馏分类型
                string data = wctType;
                if (data.Length > 12)
                    data = data.Substring(0, 12);
                //data中存什么油？？
                oilData.labData = data;
                oilData.calData = data;
                oilInfo.OilDatas.Add(oilData);
            }
            #endregion

            #region  step3: 遍历每行每列，如果单元格不空则添加数据

            List<string> itemCodeList = new List<string>();

            for (int i = rowStart; i < dt.Rows.Count; i++) //遍历每行
            {
                itemCode = dt.Rows[i][codeCol].ToString().Trim(); //在行中，通过代码列获取代码

                OilTableRowEntity row = _rowCache[itemCode, tableType]; //获取代码的ID
                if (row != null)
                {
                    for (int x = colStart; x < colStart + arrColID.Count; x++) //遍历每列
                    {
                        string labData = dt.Rows[i][x].ToString().Trim();   //获取当前行，列的单元格值
                        if (labData.Length > 10)  //暂时这样，可能读到一些批注，字符串很长。
                            labData = labData.Substring(0, 10);

                        if (labData == "")  //如果该单元格为空则跳过
                            continue;

                        #region "SUL和N2的单位转换"

                        string itemName = dt.Rows[i][0].ToString().Trim();  //行头名
                        OilTools tools = new OilTools();

                        #region "SUL的单位转换"

                        //如果是μg/g为单位的SUL,如果该列的上一行%表示的SUL的单元格不空，则跳过该单元格，否则转化为%单位                  
                        if (itemName.Contains("μg/g") && itemCode == "SUL")
                        {
                            OilDataEntity oilDataTemp;
                            oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)tableType && c.OilTableRow.itemCode == "SUL" && c.oilTableColID == arrColID[x - colStart]).FirstOrDefault();

                            //如果当前列的上一行SUL含量不为空，已经有%表示的SUL,则跳过该单元格。
                            if (oilDataTemp != null)
                                continue;
                            int decNum = row.decNumber == null ? row.valDigital : row.decNumber.Value;
                            if (!tools.unitUgTo(ref labData, decNum)) //转化数据单位,如果不能转化，修改值为空
                            {
                                labData = "";
                            }
                        }
                        #endregion

                        #region "N2的单位转换"

                        if (itemName.Contains("%") && itemCode == "N2")//如果是%为单位的N2,如果该列的下一行μg/g表示的N2的单元格为空，转化为μg/g单位，否则跳过该单元格
                        {
                            OilDataEntity oilDataTemp;
                            oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)tableType && c.OilTableRow.itemCode == "N2" && c.oilTableColID == arrColID[x - colStart]).FirstOrDefault();

                            //如果当前列的上一行N2含量不为空，已经有%表示的N2,则跳过该单元格。
                            if (oilDataTemp != null)
                                continue;
                            int decNum = row.decNumber == null ? row.valDigital : row.decNumber.Value;
                            if (!tools.unitToUg(ref labData, decNum)) //转化数据单位,如果不能转化，修改值为空
                            {
                                labData = "";
                            }
                        }
                        #endregion

                        #endregion
                        if (row.decNumber != null)
                            labData = tools.calDataDecLimit(labData, row.decNumber + 2, row.valDigital);//输入Execl表的过程中转换数据精度
                        else
                            labData = tools.calDataDecLimit(labData, row.decNumber, row.valDigital);//输入Execl表的过程中转换数据精度

                        //labData = tools.calDataDecLimit(labData, row.decNumber,row.valDigital);//输入Execl表的过程中转换数据精度
                        OilDataEntity oilData = new OilDataEntity();
                        oilData.oilTableColID = arrColID[x - colStart];
                        oilData.oilTableRowID = row.ID;
                        oilData.labData = labData;
                        oilData.calData = labData;
                        oilInfo.OilDatas.Add(oilData);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 针对渣油表,根据Excel的sheet生成的DataTable提取数据。
        /// </summary>
        /// <param name="dt">根据Excel的sheet生成的DataTable</param>      
        /// <param name="oilInfo">原油</param>
        /// <param name="rowStart">数据开始行</param>
        /// <param name="codeCol">代码列</param>
        /// <param name="colStart">数据开始列</param>
        /// <param name="tableType">表类别</param>
        private static void getSheetData(DataTable dt, ref OilInfoEntity oilInfo, int rowStart, int codeCol, int colStart, EnumTableType tableType)
        {
            string itemCode;

            //step1：计算出该表有多少有数据的列           
            List<int> arrColID = new List<int>();  //选把列Id算出   
            int colCount = dt.Columns.Count;
            OilTableColBll bll = new OilTableColBll();
            int tableColcount = colStart + bll.Where(c => c.OilTableType.ID == (int)tableType).ToList().Count(); //起始列加上该表类型的列
            if (colCount > tableColcount) //循环控制的最大列取两者之小
                colCount = tableColcount;
            for (int i = colStart; i < colCount; i++)
            {
                //按顺序cut1、2、3。。
                var col = _colCache["Cut" + (i - colStart + 1).ToString(), tableType];
                if (col != null)
                    arrColID.Add(col.ID);
            }

            #region  step3: 遍历每行每列，如果单元格不空则添加数据
            for (int i = rowStart; i < dt.Rows.Count; i++)
            {
                itemCode = dt.Rows[i][codeCol].ToString().Trim(); //在行中，通过代码列获取代码
                OilTableRowEntity row = _rowCache[itemCode, tableType]; //获取代码的ID
                if (row != null)//这一行有数据，下面遍历这一行
                {
                    for (int x = colStart; x < colCount; x++)
                    {
                        string labData = dt.Rows[i][x].ToString().Trim();   //获取当前行，列的单元格值
                        //if (labData.Length > 10)  //暂时这样，可能读到一些批注，字符串很长。
                        //    labData = labData.Substring(0, 10);

                        if (labData == "")  //如果该单元格为空则跳过
                            continue;

                        #region "SUL和N2的单位转换"

                        string itemName = dt.Rows[i][0].ToString().Trim();  //行头名
                        OilTools tools = new OilTools();

                        #region "SUL的单位转换"

                        //如果是μg/g为单位的SUL,如果该列的上一行%表示的SUL的单元格不空，则跳过该单元格，否则转化为%单位                  
                        if (itemName.Contains("μg/g") && itemCode == "SUL")
                        {
                            OilDataEntity oilDataTemp;
                            oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)tableType && c.OilTableRow.itemCode == "SUL" && c.oilTableColID == arrColID[x - colStart]).FirstOrDefault();

                            //如果当前列的上一行SUL含量不为空，已经有%表示的SUL,则跳过该单元格。
                            if (oilDataTemp != null)
                                continue;
                            int decNum = row.decNumber == null ? row.valDigital : row.decNumber.Value;
                            if (!tools.unitUgTo(ref labData, decNum)) //转化数据单位,如果不能转化，修改值为空
                            {
                                labData = "";
                            }
                        }
                        #endregion

                        #region "N2的单位转换"

                        if (itemName.Contains("%") && itemCode == "N2")//如果是%为单位的N2,如果该列的下一行μg/g表示的N2的单元格为空，转化为μg/g单位，否则跳过该单元格
                        {
                            OilDataEntity oilDataTemp;
                            oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)tableType && c.OilTableRow.itemCode == "N2" && c.oilTableColID == arrColID[x - colStart]).FirstOrDefault();

                            //如果当前列的上一行N2含量不为空，已经有%表示的N2,则跳过该单元格。
                            if (oilDataTemp != null)
                                continue;
                            int decNum = row.decNumber == null ? row.valDigital : row.decNumber.Value;
                            if (!tools.unitToUg(ref labData, decNum)) //转化数据单位,如果不能转化，修改值为空
                            {
                                labData = "";
                            }
                        }
                        #endregion

                        #endregion

                        #region "下面代码已经注释"
                        //如果是μg/g为单位的SUL,如果该列的上一行%表示的SUL的单元格不空，则跳过该单元格，否则转化为%单位
                        //string itemName = dt.Rows[i][0].ToString().Trim();  //行头名
                        //OilTools tools = new OilTools();
                        //if (itemName.Contains("μg/g") && itemCode == "SUL" && dt.Rows[i - 1][codeCol].ToString().Trim() == "SUL") //本行,上一行为SUL
                        //{
                        //    OilDataEntity oilDataTemp;
                        //    oilDataTemp = oilInfo.OilDatas.Where(c => c.OilTableTypeID == (int)tableType && c.OilTableRow.itemCode == "SUL" && c.oilTableColID == arrColID[x - colStart]).FirstOrDefault();


                        //    //如果当前列的上一行SUL含量不为空，已经有%表示的SUL,则跳过该单元格。
                        //    if (oilDataTemp != null)
                        //        continue;
                        //    if (!tools.unitUgTo(ref labData)) //转化数据单位,如果不能转化，修改值为空
                        //    {
                        //        labData = "";
                        //    }
                        //}
                        ////如果是%为单位的N2,如果该列的下一行μg/g表示的N2的单元格为空，转化为μg/g单位，否则跳过该单元格
                        //if (itemName.Contains("%") && itemCode == "N2" && dt.Rows[i + 1][codeCol].ToString().Trim() == "N2")
                        //{
                        //    string labDataNext = dt.Rows[i + 1][x].ToString().Trim();   //获取下一行，当前列的单元格值
                        //    if (labDataNext != "")
                        //        continue;
                        //    if (!tools.unitToUg(ref labData)) //转化数据单位,如果不能转化，修改值为空
                        //    {
                        //        labData = "";
                        //    }
                        //}
                        #endregion
                        if (row.decNumber != null)
                            labData = tools.calDataDecLimit(labData, row.decNumber + 2, row.valDigital);//输入Execl表的过程中转换数据精度
                        else
                            labData = tools.calDataDecLimit(labData, row.decNumber, row.valDigital);//输入Execl表的过程中转换数据精度
                        OilDataEntity oilData = new OilDataEntity();
                        //需要行列id 此id是数据库中的id
                        oilData.oilTableColID = arrColID[x - colStart];
                        oilData.oilTableRowID = row.ID;
                        oilData.labData = labData;
                        oilData.calData = labData;
                        oilInfo.OilDatas.Add(oilData);
                    }
                }
            }
            #endregion
        }

        #endregion

        #region "读入Cru文件"

        /// <summary>
        /// 导入cru文件到库B，并返回原油的oilInfoID
        /// </summary>
        /// <param name="fileName">cru文件的路径</param>
        /// <returns></returns>
        public static OilInfoBEntity importCru(string fileName)
        {
            int oilInfoID = 0;
            OilInfoBEntity oilInfoB = new OilInfoBEntity();
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            try
            {
                string[] tmpCrudeOilData = sr.ReadToEnd().Split('$');
                if (tmpCrudeOilData.Length < 1)
                    return oilInfoB;

                #region 获取表信息
                OilTableColBll colBll = new OilTableColBll();
                OilTools oilTools = new OilTools();
                CruCodeMapBll cruCodeMapBll = new CruCodeMapBll();

                List<CruCodeMapEntity> cruCodeMapListAll = cruCodeMapBll.dbGets("1 = 1");  //获取整个映射表
                int oilTableRowID = 0;
                List<CruCodeMapEntity> cruCodeMapList = null;
                string cru_D20Value = ""; //用于GC表的计算
                oilInfoB.curves = new List<CurveEntity>(); //防止自动添加一个空的曲线，oilInfoB.curves会自动添加一条曲线

                foreach (string str in tmpCrudeOilData)
                {
                    string[] tmpDetailData = str.Replace("\r\n", "\r").Trim().Split('\r');
                    int index = 1;

                    if (tmpDetailData.Length > 0)
                    {
                        if (tmpDetailData[0].Trim() == "GENERAL")
                        {
                            #region 基本信息-$GENERAL
                            cruCodeMapList = cruCodeMapListAll.Where(o => o.oilTableTypeID == (int)EnumTableType.Info).ToList();
                            string itemCode = "";
                            for (int j = index; j < tmpDetailData.Length; j++)
                            {
                                string[] tmpStr = { "", "", "" };
                                if (tmpDetailData[j].Length > 8)
                                    tmpStr[0] = tmpDetailData[j].Substring(0, 8).Trim();
                                if (tmpDetailData[j].Length >= 32)
                                    tmpStr[1] = tmpDetailData[j].Substring(9, 32).Trim().Replace("'", " "); //去掉',否则导致sql语句出错

                                for (int x = 0; x < cruCodeMapList.Count; x++)    //在原油信息表和cru文件GENERAL的映射结构中
                                {
                                    if (cruCodeMapList[x].cruCode == tmpStr[0]) //如果=原油信息表和cru文件GENERAL的映射结构中的cruCode==cru文件的代码
                                    {
                                        itemCode = cruCodeMapList[x].oilTableRow.itemCode;     //得到代码
                                        oilInfoAddItem(ref oilInfoB, itemCode, tmpStr[1]);
                                    }
                                }

                            }

                            oilInfoID = saveInfo(oilInfoB);
                            oilInfoB.ID = oilInfoID;
                            if (oilInfoID == -1)
                            {
                                return oilInfoB;
                            }
                            #endregion
                        }
                        else if (tmpDetailData[0].Trim() == "WHOLE CRUDE")
                        {
                            #region 原油性质表-$WHOLE CRUDE
                            cruCodeMapList = cruCodeMapListAll.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                            for (int j = index; j < tmpDetailData.Length; j++)
                            {
                                string[] tmpStr = { "", "", "", "", "" };
                                if (tmpDetailData[j].Length >= 12)
                                    tmpStr[0] = tmpDetailData[j].Substring(0, 12).Trim();     //CRu代码
                                if (tmpDetailData[j].Length >= 21)
                                    tmpStr[1] = tmpDetailData[j].Substring(13, 8).Trim();     //值
                                if (tmpDetailData[j].Length > 26)
                                    tmpStr[2] = tmpDetailData[j].Substring(22, 4).Trim();     //单位
                                //if (tmpDetailData[j].Length > 28)
                                //    tmpStr[3] = tmpDetailData[j].Substring(27, 1).Trim();
                                //if (tmpDetailData[j].Length > 29)
                                //    tmpStr[4] = tmpDetailData[j].Substring(29).Trim();

                                #region
                                for (int x = 0; x < cruCodeMapList.Count; x++)                //在原油其他表和cru文件的映射结构
                                {
                                    if (cruCodeMapList[x].cruCode == tmpStr[0])                //如果=原油表和cru文件映射结构中的cruCode==cru文件的代码
                                    {
                                        OilDataBEntity oilData = new OilDataBEntity();
                                        OilTableRowEntity oilTableRow = cruCodeMapList[x].oilTableRow;
                                        if (oilTableRow == null)
                                            continue;
                                        string calData = tmpStr[1];
                                        if (oilTableRow.itemUnit.ToUpper() == "KPA" || oilTableRow.itemUnit.ToUpper() == "℃")
                                        {
                                            int decNum = oilTableRow.decNumber == null ? oilTableRow.valDigital : oilTableRow.decNumber.Value;
                                            calData = oilTools.cruYConvert(oilTableRow.itemUnit, tmpStr[1], decNum);
                                            //calData = oilTools.cruYConvert(oilTableRow.itemUnit, tmpStr[1], oilTableRow.decNumber);
                                        }
                                        else if (oilTableRow.itemCode == "H2O")
                                        {
                                            oilTools.unitUgTo(ref calData);
                                        }
                                        if (calData == "")
                                            continue;

                                        oilData.oilInfoID = oilInfoID;
                                        oilData.oilTableColID = colBll["Cut1", EnumTableType.Whole].ID;
                                        oilData.oilTableRowID = oilTableRow.ID;
                                        oilData.labData = calData;
                                        oilData.calData = calData;
                                        oilInfoB.OilDatas.Add(oilData);
                                    }
                                }
                                #endregion
                            }

                            #region 补充D20
                            int posD20 = -1, posAPI = -1;

                            for (int i = 0; i < oilInfoB.OilDatas.Count; i++)     //遍历，找到D20和API的序号
                            {
                                if (oilInfoB.OilDatas[i].OilTableRow.itemCode.ToUpper() == "D20")
                                {
                                    posD20 = i;
                                    cru_D20Value = oilInfoB.OilDatas[i].calData;
                                }
                                else if (oilInfoB.OilDatas[i].OilTableRow.itemCode.ToUpper() == "API")
                                    posAPI = i;
                            }
                            if (posD20 == -1 && posAPI > -1)  //在*.cru中找不到DENSITY20,但找到API GRAVITY，且有值，then,D60=141.5/(API+131.5)*DH2O60F,D20=-0.00546*D60^2+1.013143*D60-0.010325
                            {
                                OilTableRowBll rowBll = new OilTableRowBll();
                                OilTableRowEntity d20Row = rowBll["D20", EnumTableType.Whole];

                                string calData = BaseFunction.FunD20fromAPI(oilInfoB.OilDatas[posAPI].calData); //oilTools.apiToD20(oilInfoB.OilDatas[posAPI].calData, d20Row.decNumber);
                                OilDataBEntity oilData = new OilDataBEntity();
                                CruCodeMapEntity cruCodeMap = cruCodeMapList.Where(o => o.oilTableRow.itemCode.ToUpper() == "D20").FirstOrDefault();
                                if (cruCodeMap != null)
                                {
                                    oilData.oilInfoID = oilInfoID;
                                    oilData.oilTableColID = colBll["Cut1", EnumTableType.Whole].ID;
                                    oilData.oilTableRowID = cruCodeMap.oilTableRow.ID;
                                    cru_D20Value = calData;
                                    oilData.labData = calData;
                                    oilData.calData = calData;
                                    oilInfoB.OilDatas.Add(oilData);
                                }
                            }
                            #endregion
                            #endregion
                        }
                        else if (tmpDetailData[0].Trim() == "GAS CHROMATOGRAPHY")
                        {
                            #region GC标准表-$GAS CHROMATOGRAPHY
                            cruCodeMapList = cruCodeMapListAll.Where(o => o.oilTableRow.OilTableType.ID == (int)EnumTableType.GCLevel).ToList();
                            string GV00Value = "";   //用于GC转化
                            if (tmpDetailData.Length > 3)    //GV表要使用GV00计算，GV00在第三行
                            {
                                index = 3;                                          //第三开始数据，GV00在第三行
                                GV00Value = tmpDetailData[2].Substring(5, 9).Trim();  //得到GV00 
                                float wy = 0;
                                for (int j = index; j < tmpDetailData.Length; j++)
                                {
                                    string[] tmpStr = { "", "", "" };
                                    if (tmpDetailData[j].Length >= 5)
                                        tmpStr[0] = tmpDetailData[j].Substring(0, 5).Trim();
                                    if (tmpDetailData[j].Length >= 14)
                                        tmpStr[1] = tmpDetailData[j].Substring(5, 9).Trim();
                                    //tmpStr[2] = tmpDetailData[j].Substring(14).Trim();

                                    foreach (var cruCodeItem in cruCodeMapList.Where(o => o.cruCode == tmpStr[0]))
                                    {
                                        OilDataBEntity oilData = new OilDataBEntity();
                                        OilTableRowEntity oilTableRow = cruCodeItem.oilTableRow;
                                        if (oilTableRow == null)
                                            continue;
                                        int decNum = oilTableRow.decNumber == null ? oilTableRow.valDigital : oilTableRow.decNumber.Value;
                                        string calData = oilTools.GASToGV(tmpStr[1], GV00Value, cru_D20Value, cruCodeItem.cParam, decNum);
                                        //string calData = oilTools.GASToGV(tmpStr[1], GV00Value, cru_D20Value, cruCodeItem.cParam, oilTableRow.decNumber);
                                        if (calData == "")
                                            continue;
                                        wy += float.Parse(calData);
                                        oilTableRowID = cruCodeItem.oilTableRow.ID;     //得到代码
                                        oilData.oilInfoID = oilInfoID;
                                        oilData.oilTableColID = colBll["Cut1", EnumTableType.GCLevel].ID;
                                        oilData.oilTableRowID = oilTableRowID;

                                        oilData.labData = calData;
                                        oilData.calData = calData;
                                        oilInfoB.OilDatas.Add(oilData);
                                    }
                                    #region "old code"
                                    /*
                                    for (int x = 0; x < cruCodeMapList.Count; x++) //在原油其他表和cru文件的映射结构
                                    {
                                        if (cruCodeMapList[x].cruCode == tmpStr[0]) //如果=原油表和cru文件映射结构中的cruCode==cru文件的代码
                                        {
                                            OilDataBEntity oilData = new OilDataBEntity();
                                            OilTableRowEntity oilTableRow = cruCodeMapList[x].oilTableRow;
                                            if (oilTableRow == null)
                                                continue;
                                            string calData = oilTools.GASToGV(tmpStr[1], GV00Value, oilTableRow.decNumber);
                                            if (calData == "")
                                                continue;
                                            oilTableRowID = cruCodeMapList[x].oilTableRow.ID;     //得到代码
                                            oilData.oilInfoID = oilInfoID;
                                            oilData.oilTableColID = colBll["Cut1", EnumTableType.GCLevel].ID;
                                            oilData.oilTableRowID = oilTableRowID;

                                            oilData.labData = calData;
                                            oilData.calData = calData;
                                            oilInfoB.OilDatas.Add(oilData);
                                        }
                                    }*/
                                    #endregion
                                }
                                var cruCodeMap = cruCodeMapListAll.Where(o => o.oilTableRow.OilTableType.ID == (int)EnumTableType.GCLevel && o.oilTableRow.itemCode.ToUpper() == "WY").FirstOrDefault();
                                if (cruCodeMap != null)
                                {
                                    OilDataBEntity oilData = new OilDataBEntity();
                                    oilData.oilInfoID = oilInfoID;
                                    oilData.oilTableColID = colBll["Cut1", EnumTableType.GCLevel].ID;
                                    oilData.oilTableRowID = cruCodeMap.oilTableRow.ID;

                                    oilData.labData = wy.ToString();
                                    oilData.calData = wy.ToString();
                                    oilInfoB.OilDatas.Add(oilData);
                                }

                                #region "ECP赋值"
                                OilTableRowEntity ECPRow = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel && o.itemCode == "ECP").FirstOrDefault();
                                if (ECPRow != null)
                                {
                                    OilDataBEntity oilData = new OilDataBEntity();
                                    oilData.oilInfoID = oilInfoID;
                                    oilData.oilTableColID = colBll["Cut1", EnumTableType.GCLevel].ID;
                                    oilData.oilTableRowID = ECPRow.ID;
                                    oilData.labData = "150";
                                    oilData.calData = "150";
                                    oilInfoB.OilDatas.Add(oilData);
                                }
                                #endregion

                                #region "ICP赋值"
                                var t = tmpCrudeOilData.Where(o => o.Contains("\nPROPERTY WYIELD")).FirstOrDefault();
                                var icpValue = "-100";
                                if (t != null)
                                {
                                    var arr = t.Split("\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                                    var title = arr.FirstOrDefault(o => o.StartsWith("PROPERTY WYIELD"));
                                    icpValue = arr[arr.IndexOf(title) + 1].Split(" \t".ToArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                                }
                                icpValue = (5 / 9.0 * (double.Parse(icpValue) - 32)).ToString();

                                OilTableRowEntity ICPRow = OilTableRowBll._OilTableRow.Where(o => o.itemCode == "ICP" && o.oilTableTypeID == (int)EnumTableType.GCLevel).FirstOrDefault();
                                string strICPValue = oilTools.calDataDecLimit(icpValue, ICPRow.decNumber, ICPRow.valDigital);
                                //string strICPValue = oilTools.calDataDecLimit(icpValue, ICPRow.decNumber);
                                oilInfoB.ICP0 = strICPValue;
                                if (ICPRow != null)
                                {
                                    //PROPERTY WYIELD
                                    OilDataBEntity oilData = new OilDataBEntity();
                                    oilData.oilInfoID = oilInfoID;
                                    oilData.oilTableColID = colBll["Cut1", EnumTableType.GCLevel].ID;
                                    oilData.oilTableRowID = ICPRow.ID;

                                    oilData.labData = strICPValue;
                                    oilData.calData = strICPValue;
                                    oilInfoB.OilDatas.Add(oilData);

                                }
                                #endregion
                            }
                            #endregion
                        }
                        else if (tmpDetailData[0].Trim() == "RESIDUE" || tmpDetailData[0].Trim() == "DISTILLATE")
                        {
                            cruCurve(ref oilInfoB, str, cruCodeMapListAll);
                        }
                    }
                }
                #endregion

                sr.Close();
                saveTables(oilInfoB);
                saveCurves(oilInfoB);
                return oilInfoB;
            }
            catch (Exception ex)
            {
                Log.Error("数据管理,导入CRu到库importCru()" + ex);
                return null;
            }
            finally
            {
                sr.Close();
            }
        }

        /// <summary>
        /// 读Cru文件的曲线
        /// </summary>
        /// <param name="oilInfoB">原油</param>
        /// <param name="strCuever">曲线字符串</param>
        /// <param name="cruCodeMapListAll">映射表</param>
        private static void cruCurve(ref OilInfoBEntity oilInfoB, string strCuever, List<CruCodeMapEntity> cruCodeMapListAll)
        {
            #region 读曲线

            OilTools oilTools = new OilTools();
            CurveTypeAccess curveTypeAccess = new CurveTypeAccess();
            string[] tmpDetailData = strCuever.Replace("PROPERTY", "|").Trim().Split('|');
            string typeCode = tmpDetailData[0].Trim();         //数组第一项为曲线类别代码 -馏分曲线,渣油曲线               

            CurveTypeEntity curveType = curveTypeAccess.Get("typeCode='" + typeCode + "'").FirstOrDefault();
            List<CruCodeMapEntity> cruCodeMapList = null;
            if (typeCode == "RESIDUE")
                cruCodeMapList = cruCodeMapListAll.Where(o => o.oilTableRow.OilTableType.ID == (int)EnumTableType.Residue).ToList(); //获取该类别曲线的映射
            else
                cruCodeMapList = cruCodeMapListAll.Where(o => o.oilTableRow.OilTableType.ID == (int)EnumTableType.Narrow || o.oilTableRow.OilTableType.ID == (int)EnumTableType.Wide).ToList(); //获取该类别曲线的映射


            if (curveType == null)
                return;

            for (int j = 1; j < tmpDetailData.Length; j++)
            {
                CurveEntity curve = new CurveEntity();                      //构建一条曲线
                string[] tmpCurver = tmpDetailData[j].Replace("\r\n", "\r").Trim().Split('\r');

                curve.oilInfoID = oilInfoB.ID;
                curve.curveTypeID = curveType.ID;

                CruCodeMapEntity cruCodeMap = new CruCodeMapEntity();      //记录该曲线的映射，用有代码和单位以及曲线值的转化
                if (tmpCurver[0].Length >= 11)
                {
                    string cruCode = tmpCurver[0].Substring(0, 12).Trim();   //Cru文件的代码类,两种格式Cru文件代码宽度一样
                    int x;
                    for (x = 0; x < cruCodeMapList.Count; x++)
                    {
                        cruCodeMap = cruCodeMapList[x];
                        if (cruCodeMap.cruCode.Trim() == cruCode)            //如果=原油表和cru文件映射结构中的cruCode==cru文件的代码
                        {
                            break;
                        }
                    }
                    if (x == cruCodeMapList.Count) //没有找到则跳过
                    {
                        continue;
                    }
                    curve.propertyY = cruCodeMap.oilTableRow.itemCode;  //暂时存放代码，考虑是否存放ID和oilTableRow表关联

                    if ((cruCode == "YIELD" || cruCode == "WYIELD") && typeCode == "DISTILLATE")     //Cru文件中该曲线是收率曲线
                    {
                        CurveTypeEntity curveTypetmp = curveTypeAccess.Get("typeCode='YIELD'").FirstOrDefault();
                        curve.curveTypeID = curveTypetmp.ID;
                    }

                }

                if (typeCode == "RESIDUE")
                    curve.propertyX = "WY";
                curve.unit = cruCodeMap.oilTableRow.itemUnit;
                curve.decNumber = cruCodeMap.oilTableRow.decNumber == null ? cruCodeMap.oilTableRow.valDigital : cruCodeMap.oilTableRow.decNumber.Value;
                //curve.decNumber = cruCodeMap.oilTableRow.decNumber;
                curve.descript = cruCodeMap.oilTableRow.itemName;

                CurveAccess acess = new CurveAccess();
                int curveID = acess.Insert(curve);   //保存曲线，并获取ID
                curve.ID = curveID;

                #region  添加曲线上的点
                for (int k = 1; k < tmpCurver.Length; k++)
                {
                    CurveDataEntity CurveData = new CurveDataEntity();
                    string[] strLine = tmpCurver[k].Trim().Split(' ');     //获取x,y，先裁掉两端空格，然后按中间的空格拆分
                    string[] tmpStr = { "", "" };
                    tmpStr[0] = strLine[0];                            //与88. 这种数据
                    tmpStr[1] = strLine[strLine.Length - 1];

                    CurveData.curveID = curveID;
                    string xValue = tmpStr[0];             //渣油不转化 x为累计收率twy
                    if (typeCode != "RESIDUE")
                        xValue = oilTools.fToC(tmpStr[0], curve.decNumber);  //X轴 华氏温度转化为摄氏温度值

                    string yValue = tmpStr[1];
                    if (cruCodeMap.oilTableRow.itemUnit.ToUpper() == "KPA" || cruCodeMap.oilTableRow.itemUnit.ToUpper() == "℃")
                    {
                        int decNum = cruCodeMap.oilTableRow.decNumber == null ? cruCodeMap.oilTableRow.valDigital : cruCodeMap.oilTableRow.decNumber.Value;
                        yValue = oilTools.cruYConvert(cruCodeMap.oilTableRow.itemUnit, tmpStr[1], decNum); //Y轴单位转化
                        //yValue = oilTools.cruYConvert(cruCodeMap.oilTableRow.itemUnit, tmpStr[1], cruCodeMap.oilTableRow.decNumber); //Y轴单位转化
                    }

                    if (xValue != "" && yValue != "")
                    {
                        CurveData.xValue = Convert.ToSingle(xValue);
                        CurveData.yValue = Convert.ToSingle(yValue);
                        curve.curveDatas.Add(CurveData);
                    }
                }
                #endregion
                curve.curveDatas = curve.curveDatas.OrderBy(o => o.xValue).ToList();
                oilInfoB.curves.Add(curve);
            }

            #region "补充D20"

            int posD20 = -1, posAPI = -1;

            for (int j = 0; j < oilInfoB.curves.Count; j++)
            {
                if (oilInfoB.curves[j].propertyY == "D20" && oilInfoB.curves[j].curveTypeID == curveType.ID) //D20曲线并且同类别
                {
                    posD20 = j; break;           //找到D20，表明已经有D20曲线了
                }
                else if (oilInfoB.curves[j].propertyY == "API" && oilInfoB.curves[j].curveTypeID == curveType.ID)
                    posAPI = j;
            }

            if (posD20 == -1 && posAPI > -1)  // 在*.cru中找不到DENSITY20,但找到API GRAVITY，且有值，then
            {
                CurveEntity curve = new CurveEntity();
                curve.oilInfoID = oilInfoB.ID;
                curve.curveTypeID = curveType.ID;
                curve.unit = "g/cm3";
                curve.decNumber = oilInfoB.curves[posAPI].decNumber;
                curve.descript = "";
                CruCodeMapEntity cruCodeMap = cruCodeMapList.Where(o => o.oilTableRow.itemCode.ToUpper() == "D20").FirstOrDefault();
                curve.propertyY = cruCodeMap.oilTableRow.itemCode;
                if (typeCode == "RESIDUE")
                    curve.propertyX = "WY";
                CurveAccess acess = new CurveAccess();
                int curveID = acess.Insert(curve);   //保存曲线，并获取ID
                curve.ID = curveID;

                for (int k = 1; k < oilInfoB.curves[posAPI].curveDatas.Count; k++) //添加曲线上的点
                {
                    CurveDataEntity CurveData = new CurveDataEntity();
                    CurveData.curveID = curveID;
                    int decNum = cruCodeMap.oilTableRow.decNumber == null ? cruCodeMap.oilTableRow.valDigital : cruCodeMap.oilTableRow.decNumber.Value;
                    string yValue = oilTools.apiToD20(oilInfoB.curves[posAPI].curveDatas[k].yValue.ToString(), decNum); //Y轴转化
                    //string yValue = oilTools.apiToD20(oilInfoB.curves[posAPI].curveDatas[k].yValue.ToString(), cruCodeMap.oilTableRow.decNumber); //Y轴转化
                    if (yValue != "")
                    {
                        CurveData.xValue = oilInfoB.curves[posAPI].curveDatas[k].xValue;
                        CurveData.yValue = Convert.ToSingle(yValue);
                        curve.curveDatas.Add(CurveData);
                    }
                }
                oilInfoB.curves.Add(curve);
            }
            #endregion

            #endregion
        }

        #endregion

        #region "插入批注"
        /// <summary>
        /// 保存一条批注，并返回ID
        /// </summary>
        /// <param name="remarkData">一条批注信息</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public static int editRemark(RemarkEntity remarkData)
        {
            RemarkAccess remarkAccess = new RemarkAccess();
            if (remarkData.ID > 0 && string.IsNullOrWhiteSpace(remarkData.LabRemark) && string.IsNullOrWhiteSpace(remarkData.CalRemark))
            {
                remarkAccess.Delete(remarkData.ID);
            }

            if (string.IsNullOrWhiteSpace(remarkData.LabRemark) && string.IsNullOrWhiteSpace(remarkData.CalRemark))  //如果批注信息为空，返回-1不能插入原油，否则插入           
                return -1;

            if (remarkData.oilInfoID < 0 || remarkData.oilTableColID < 0 || remarkData.oilTableRowID < 0)
                return -1;


            if (remarkData.ID > 0)             //如果是打开编辑的原油(info.ID > 0),则更新，是新建的原油则判断新原油的代码是否存在，如果存在不能插入
            {
                remarkAccess.Update(remarkData, remarkData.ID.ToString());
                remarkAccess.Delete("labremark = '" + string.Empty + "' and calremark = '" + string.Empty + "'");
            }
            else
            {
                string sqlWhere = "oilInfoID = " + remarkData.oilInfoID + "and oilTableColID = " + remarkData.oilTableColID + "and oilTableRowID = " + remarkData.oilTableRowID;
                List<RemarkEntity> remarkList = remarkAccess.Get(sqlWhere).ToList();
                if (remarkList.Count != 0)
                {
                    RemarkEntity tempRemak = remarkList[0];
                    remarkAccess.Update(tempRemak, tempRemak.ID.ToString());
                    remarkAccess.Delete("labremark = '" + string.Empty + "' and calremark = '" + string.Empty + "'");
                }
                remarkData.ID = remarkAccess.Insert(remarkData);
            }
            return remarkData.ID;
        }

        #endregion
        //临时导入数据-Cru导入B库_转换对应_发 出_201220211.xls
        public static void importExcelTemp(string fileName)
        {
            #region 原油 WHOLE CRUDE
            DataSet ds = ExcelTool.ExcelToDataSet(fileName);
            string itemCode = "";

            var dt = getTable("CruCodeMap");
            for (int i = 0; i < ds.Tables[4].Rows.Count; i++)
            {
                itemCode = ds.Tables[4].Rows[i][4].ToString().Trim();  //在行中获取代码
                OilTableRowEntity row = _rowCache[itemCode, EnumTableType.Whole];    //获取代码的ID

                if (row != null)
                {
                    string cruCode = ds.Tables[4].Rows[i][0].ToString().Trim(); //在行中获取代码

                    var r = dt.NewRow();
                    r["oilTableTypeID"] = (int)EnumTableType.Whole;
                    r["cruCode"] = cruCode;
                    r["itemCode"] = row.itemCode;
                    dt.Rows.Add(r);
                }
            }

            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                itemCode = ds.Tables[1].Rows[i][3].ToString().Trim();  //在行中获取代码
                OilTableRowEntity row = _rowCache[itemCode, EnumTableType.GCLevel];    //获取代码的ID

                if (row != null)
                {
                    string cruCode = ds.Tables[1].Rows[i][0].ToString().Trim(); //在行中获取代码
                    string strParam = ds.Tables[1].Rows[i][5].ToString().Trim(); //在行中获取参数
                    float cParam;
                    if (!float.TryParse(strParam, out cParam))
                        cParam = -1;
                    var r = dt.NewRow();
                    r["oilTableTypeID"] = (int)EnumTableType.GCLevel;
                    r["cruCode"] = cruCode;
                    r["itemCode"] = row.itemCode;
                    dt.Rows.Add(r);
                }
            }

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                itemCode = ds.Tables[0].Rows[i][4].ToString().Trim();  //在行中获取代码
                OilTableRowEntity row = _rowCache[itemCode, EnumTableType.Narrow];    //获取代码的ID,先找窄馏分，如果没有则找宽馏分
                if (row == null)
                    row = _rowCache[itemCode, EnumTableType.Wide];
                if (row != null)
                {
                    string cruCode = ds.Tables[0].Rows[i][0].ToString().Trim(); //在行中获取代码

                    var r = dt.NewRow();
                    r["oilTableTypeID"] = row.oilTableTypeID;
                    r["cruCode"] = cruCode;
                    r["itemCode"] = row.itemCode;
                    dt.Rows.Add(r);
                }
            }

            for (int i = 0; i < ds.Tables[3].Rows.Count; i++)
            {
                itemCode = ds.Tables[3].Rows[i][4].ToString().Trim();  //在行中获取代码
                OilTableRowEntity row = _rowCache[itemCode, EnumTableType.Residue];    //获取代码的ID

                if (row != null)
                {
                    string cruCode = ds.Tables[3].Rows[i][0].ToString().Trim(); //在行中获取代码

                    var r = dt.NewRow();
                    r["oilTableTypeID"] = (int)EnumTableType.Residue;
                    r["cruCode"] = cruCode;
                    r["itemCode"] = row.itemCode;
                    dt.Rows.Add(r);
                }
            }


            BulkToDB(dt, "CruCodeMap");

            #endregion
        }
    }
}


