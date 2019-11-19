using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;


namespace RIPP.OilDB.Data
{
    /// <summary>
    /// 数据库操作抽象类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class abstractDB<T> where T : class,new ()
    {
        protected string tableName { set; get; } //数据库的表名
        protected string keyField { set; get; }  //数据库的表的关键字        

        protected abstract Dictionary<string, string> getProperty(T item);
        protected abstract List<T> dataReaderToEntity(IDataReader reader);

        /// <summary>
        /// 根据实体得到数据插入语句
        /// </summary>
        /// <param name="item">实体</param>      
        /// <returns>数据插入语句</returns>
        protected virtual string getInsertSQL(T item)
        {
            if (item == null)
                return null;
            Dictionary<string, string> dic = this.getProperty(item);
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            foreach (string k in dic.Keys)
            {
                if (k.ToUpper() != this.keyField.ToUpper())
                {
                    keys.Add(k);
                    values.Add(string.Format("'{0}'", dic[k]));
                }
            }

            if (keys.Count > 0)
                return string.Format("Insert into {0} ({1}) values ({2})",
                    this.tableName,
                    string.Join(",", keys),
                    string.Join(",", values)
                    );

            return null;
        }

        /// <summary>
        /// 根据实体和更新的关键字得到数据更新语句
        /// </summary>
        /// <param name="item">实体</param>
        /// <param name="keyvalue">关键字</param>
        /// <returns>数据更新语句</returns>
        protected virtual string getUpdateSQL(T item, string keyvalue)
        {
            if (item == null)
                return null;
            if (string.IsNullOrWhiteSpace(keyvalue) )
                return null;
            Dictionary<string, string> dic = this.getProperty(item);
            List<string> va = new List<string>();
            foreach (string k in dic.Keys)
            {
                if (k.ToUpper() != this.keyField.ToUpper())
                {
                    va.Add(string.Format("{0} = '{1}'", k, dic[k]));
                }
            }

            if (va.Count > 0)
                return string.Format("update {0} set {1} where {2} ='{3}'", this.tableName, string.Join(",", va), this.keyField, keyvalue);
                                                                                             
            return null;
        }

      
        /// <summary>
        /// 根据ID获取数据记录并转化为实体
        /// </summary>
        /// <param name="id">关键字id</param>
        /// <returns>记录的实体</returns>
        public virtual T Get(int id)
        {
            string sqlwhere = string.Format("{0}='{1}'", this.keyField, id);
            return this.Get(sqlwhere).FirstOrDefault();
        }

        /// <summary>
        /// 根据原有编号获取记录并转化为实体
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual T GetOilInfoByCrudex(string index)
        {
            string sqlwhere = string.Format("{0}='{1}'", "CrudeIndex", index);
            return this.Get(sqlwhere).FirstOrDefault();
        }

        /// <summary>
        /// 根据SQL 的where条件获取数据记录并转化为List
        /// </summary>
        /// <param name="sqlWhere">条件</param>
        /// <returns></returns>
        public virtual List<T> Get(string sqlWhere,int Count=0,string orderStr=null)
        {
            string sql ;
            if (Count < 1)
                sql = string.Format("Select * from {0} where {1}", this.tableName, sqlWhere);
            else
                sql = string.Format("Select top {2} * from {0} where {1}", this.tableName, sqlWhere, Count);

            if (!string.IsNullOrWhiteSpace(orderStr))
                sql = string.Format("{0} order by {1}", sql, orderStr);
            var reader = SqlHelper.ExecuteReader(SqlHelper.connectionString, CommandType.Text, sql, null);
            var lst= this.dataReaderToEntity(reader);
            reader.Close();
            
            return lst;
        }

        

        /// <summary>
        /// 插入数据并返回刚插入数据的id
        /// </summary>
        /// <param name="item">实体</param>
        /// <returns>自增的ID</returns>
        public virtual int Insert(T item)
        {
            string sql = this.getInsertSQL(item);
            sql = sql + "; select SCOPE_IDENTITY();";
            int reslut =  Convert.ToInt32 ( SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text ,sql,null));
            return reslut;
        }

        /// <summary>
        /// 根据id删除记录
        /// </summary>
        /// <param name="id">关键字id</param>
        /// <returns>受影响的行数</returns>
        public virtual int Delete(int id)
        {
            string sqlwhere = string.Format(" {0}='{1}'", this.keyField, id);
            return this.Delete(sqlwhere);
        }

        /// <summary>
        /// 根据条件删除记录
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>受影响的行数</returns>
        public virtual int Delete(string where)
        {
            string sql = string.Format("Delete from {0} where {1}", this.tableName, where);
            return SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, null);
        }

        /// <summary>
        /// 根据关键字字段更新数据
        /// </summary>
        /// <param name="item">实体</param>
        /// <returns>受影响的记录条数</returns>
        public virtual int Update(T item,string keyValue)
        {
            string sql = this.getUpdateSQL(item, keyValue);
            int result =  Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, sql, null));
            return result;              
        }



        /// <summary>
        /// 根据关键字字段更新数据
        /// </summary>
        /// <param name="item">实体</param>
        /// <returns>受影响的记录条数</returns>
        public virtual int Update(T item, string keyValue,string oldStr,string newStr)
        {
            string sql = this.getUpdateSQL(item, keyValue);
            string newSql = sql.Replace(oldStr, newStr);
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, newSql, null));
        }
    }
}