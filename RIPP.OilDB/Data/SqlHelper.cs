using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Reflection;

namespace RIPP.OilDB.Data
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class SqlHelper
    {
        #region 私有变量

        //连接数据库连接字符串
        private static string _connectionString = string.Empty;
        //缓存参数哈希表
        private static Hashtable _parmCache = Hashtable.Synchronized(new Hashtable());

        #endregion

        #region 属性

        //连接数据库连接字符串
        public static string connectionString
        {
            get
            {
                if (_connectionString == string.Empty)
                {
                    try
                    {
                        _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLConnString"].ToString();
                    }
                    catch
                    {
                        _connectionString = "server=.;database = OilDataManage;Trusted_Connection=True;";
                    }
                }
                return _connectionString;
            }
        }


        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlHelper()
        {
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 执行数据库查询返回记录集(DataSet)
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>记录集(DataSet)</returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// 执行数据库查询返回记录集(DataSet)
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>记录集(DataSet)</returns>
        public static DataSet ExecuteDataSet(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行数据库操作：添加、删除、更新不返回结果
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>整数(命令所影响的行数)或-1</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);

                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行数据库操作：添加、删除、更新不返回结果
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>整数(命令所影响的行数)或-1</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行数据库事务操作不返回结果
        /// </summary>
        /// <param name="trans">数据库事务</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>整数(命令所影响的行数)或-1</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="ds">要更新的数据</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(DataSet ds, string cmdText)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmdText, connection);
                SqlCommandBuilder cmb = new SqlCommandBuilder(da);
                da.UpdateCommand = cmb.GetUpdateCommand();
                da.DeleteCommand = cmb.GetDeleteCommand();
                da.InsertCommand = cmb.GetInsertCommand();
                return da.Update(ds.Tables[0]);
            }
        }

        /// <summary>
        /// 执行数据库查询返回记录集(DataReader)
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>记录集(DataReader)</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行数据库查询返回记录集(DataReader)
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>记录集(DataReader)</returns>
        public static SqlDataReader ExecuteReader(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();

                //conn.Close();
                return rdr;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 执行ExecuteScalar并返回查询所返回的结果集中第一行的第一列
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">具体命令文本</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>返回查询结果集中第一行的第一列</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                // conn.Close();
                return val;
            }
        }

        /// <summary>
        /// 数据库操作(SqlCommand)命令预处理
        /// </summary>
        /// <param name="cmd">数据库操作(SqlCommand)对象</param>
        /// <param name="conn">数据库连接(SqlConnection)对象</param>
        /// <param name="trans">数据库事务(SqlTransaction)对象</param>
        /// <param name="cmdType">命令类型(CommandType:存储过程、Sql语句)</param>
        /// <param name="cmdText">命令文本</param>
        /// <param name="commandParameters">Parameters for the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            if (trans != null)
                cmd.Transaction = trans;

            if (commandParameters != null)
            {
                foreach (SqlParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        ///参数缓存
        /// </summary>
        /// <param name="cacheKey">参数缓存关键字</param>
        /// <param name="commandParameters">缓存的参数</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            _parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 从缓存中取参数
        /// </summary>
        /// <param name="cacheKey">参数缓存关键字</param>
        /// <returns></returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])_parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            return conn;
        }

        #endregion

        //#region 公共查询数据函数Sql存储过程版

        ///// <summary>
        ///// 公共查询数据函数Sql存储过程版
        ///// </summary>    
        ///// <param name="qp">查询字符串</param>
        ///// <param name="totalRecords">返回记录总数</param>
        ///// <returns>返回ds</returns>
        //public static DataSet GetQuryPage(QueryParam qp, out int totalRecords)
        //{
        //    totalRecords = 0;
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("QuryPage", connection);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        // 设置参数
        //        cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 500).Value = qp.TableName;
        //        cmd.Parameters.Add("@ReturnFields", SqlDbType.NVarChar, 500).Value = qp.ReturnFields;
        //        cmd.Parameters.Add("@Where", SqlDbType.NVarChar, 500).Value = qp.StrWhere;
        //        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = qp.CurPage;
        //        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = qp.PageSize;
        //        cmd.Parameters.Add("@Orderfld", SqlDbType.NVarChar, 200).Value = qp.OrderColumn;
        //        cmd.Parameters.Add("@OrderType", SqlDbType.Int).Value = qp.OrderType;
        //        // 执行
        //        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //        {
        //            DataSet ds = new DataSet();
        //            try
        //            {
        //                da.Fill(ds, "ds");
        //                totalRecords = int.Parse(ds.Tables[1].Rows[0][1].ToString());
        //                cmd.Parameters.Clear();
        //            }
        //            catch (System.Data.SqlClient.SqlException ex)
        //            {
        //                throw new Exception(ex.Message);
        //            }
        //            finally
        //            {
        //                if (connection.State == ConnectionState.Open)
        //                {
        //                    connection.Close();
        //                }
        //            }
        //            return ds;
        //        }

        //    }
        //}


        ///// <summary>
        ///// 公共查询数据函数Sql存储过程版
        ///// </summary>    
        ///// <param name="qp">查询字符串</param>
        ///// <param name="totalRecords">返回记录总数</param>
        ///// <returns>以返回查询结果DataReader数据集</returns>
        //public static SqlDataReader ExecuteReader(QueryParam qp, out int totalRecords)
        //{          
        //    totalRecords = 0;
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("QuryPage", connection);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        // 设置参数
        //        cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 500).Value = qp.TableName;
        //        cmd.Parameters.Add("@ReturnFields", SqlDbType.NVarChar, 500).Value = qp.ReturnFields;
        //        cmd.Parameters.Add("@Where", SqlDbType.NVarChar, 500).Value = qp.StrWhere;
        //        cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = qp.CurPage;
        //        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = qp.PageSize;
        //        cmd.Parameters.Add("@Orderfld", SqlDbType.NVarChar, 200).Value = qp.OrderColumn;
        //        cmd.Parameters.Add("@OrderType", SqlDbType.Int).Value = qp.OrderType;

        //        // 执行
        //        try
        //        {                  
        //            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        //            cmd.Parameters.Clear();
        //            return rdr;
        //        }
        //        catch
        //        {
        //            connection.Close();
        //            throw;
        //        }
        //    }
        //}

        //#endregion



    }



}
