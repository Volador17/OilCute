using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.Data
{
    public class S_ParmBll
    {      
        private S_ParmAccess _access = new S_ParmAccess();

        public S_ParmBll()
        {
        }

        #region 参数基本操作-获取，添加，删除，修改
        /// <summary>
        /// 根据参数类别ID，获取参数列表
        /// </summary>
        /// <param name="parmTypeID">参数类别ID</param>
        /// <returns>参数列表</returns>
        public List<S_ParmEntity> GetParms(int parmTypeID)
        {
            return _access.Get("parmTypeID=" + parmTypeID, 0, "parmOrder asc"); 
        }

        /// <summary>
        /// 根据参数类别编码，获取参数列表
        /// </summary>
        /// <param name="code">参数类别编码</param>
        /// <returns>参数列表</returns>
        public List<S_ParmEntity> GetParms(string code)
        {
            S_ParmTypeEntity parmType = GetParmType(code);
            if (parmType == null)
                return null;
            return _access.Get("parmTypeID=" + parmType.ID, 0, "parmOrder asc"); 
        }

        /// <summary>
        /// 根据ID获取参数
        /// </summary>
        /// <param name="id">参数ID</param>
        /// <returns>参数</returns>
        public S_ParmEntity GetParm(int id)
        {
            return _access.Get(id);
        }

        /// <summary>
        /// 插入数据并返回刚插入数据的id
        /// </summary>
        /// <param name="item">实体</param>
        /// <returns>自增的ID</returns>
        public int AddParm(S_ParmEntity item)
        {
            return this._access.Insert(item);
        }

        /// <summary>
        /// 根据id删除记录
        /// </summary>
        /// <param name="id">关键字id</param>
        /// <returns>受影响的行数</returns>
        public int DeleteParm(int id)
        {
            return this._access.Delete(id);
        }

        /// <summary>
        /// 根据关键字字段更新数据
        /// </summary>
        /// <param name="item">实体</param>
        /// <returns>受影响的记录条数</returns>
        public void UpdateParm(S_ParmEntity item)
        {
            this._access.Update(item, item.ID.ToString());
        }
        #endregion

        /// <summary>
        /// 获取指定ID在该类别参数中的序号
        /// </summary>
        /// <param name="parmTypeID">类别ID</param>
        /// <returns>序号</returns>
        public int getParmIndex(int id, int parmTypeID)
        {
            List<S_ParmEntity> parms = _access.Get("parmTypeID=" + parmTypeID, 0, "parmOrder asc");  //某类别的参数列表
            int index = 0;
            for (int i = 0; i < parms.Count; i++)
            {
                if (parms[i].ID == id)
                {
                    index = i;
                }
            }
            return index;
        }

        /// <summary>
        /// 获取指定值在该类别参数中的序号，用于在下拉列表中根据值找到列表中该值对应的索引，把当前索引设置为该索引
        /// </summary>
        /// <param name="code">类别代码</param>
        /// <returns>序号，找不到返回0</returns>
        public int getParmIndex(string parmValue, string code)
        {
            List<S_ParmEntity> parms = this.GetParms(code);  //某类别的参数列表
            int index = 0;
            for (int i = 0; i < parms.Count; i++)
            {
                if (parms[i].parmValue == parmValue)
                {
                    index = i;
                }
            }
            return index;
        }

        /// <summary>
        /// 根据类别和编号获取一项数据的ID
        /// </summary>
        /// <param name="parmValue">参数值</param>
        /// <returns>ID</returns>
        public int getParmID(string parmValue, int parmTypeID)
        {
            List<S_ParmEntity> parms = _access.Get("parmValue= '" + parmValue + "' and parmTypeID=" + parmTypeID);  //某类别的参数列表

            if (parms != null)
                return parms.First().ID;
            else
                return -1;
        }

        /// <summary>
        /// 根据ID获取一项参数数据的名称
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>参数名称</returns>
        public string getParmName(int id)
        {
            S_ParmEntity parm = _access.Get(id);
            if (parm != null)
                return parm.parmName;
            else
                return "";
        }

        /// <summary>
        /// 根据ID获取参数值
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>参数值</returns>
        public string getParmCode(int id)
        {
            S_ParmEntity parm = _access.Get(id);
            if (parm != null)
                return parm.parmValue;
            else
                return "";
        }


        #region 参数类别的基本操作-获取，添加，删除，修改
        /// <summary>
        /// 获取参数类别列表
        /// </summary>     
        /// <returns>参数类别列表</returns>
        public List<S_ParmTypeEntity> GetParmTypes()
        {
            S_ParmTypeAccess access = new S_ParmTypeAccess();
            return access.Get("1=1");
        }

        /// <summary>
        /// 根据ID获取参数类别
        /// </summary>
        /// <param name="id">参数类别ID</param>
        /// <returns>参数类别</returns>
        public S_ParmTypeEntity GetParmType(int id)
        {
            S_ParmTypeAccess access = new S_ParmTypeAccess();
            return access.Get(id);
        }

        /// <summary>
        /// 根据ID获取参数类别
        /// </summary>
        /// <param name="code">参数类别代码</param>
        /// <returns>参数类别</returns>
        public S_ParmTypeEntity GetParmType(string code)
        {
            S_ParmTypeAccess access = new S_ParmTypeAccess();
            List<S_ParmTypeEntity> ParmTypes = access.Get("code='" + code + "'");  //某类别的参数列表
            return ParmTypes.FirstOrDefault();
        }

        /// <summary>
        /// 插入参数类别并返回刚插入数据的id
        /// </summary>
        /// <param name="item">参数类别实体</param>
        /// <returns>自增的ID</returns>
        public int AddParmType(S_ParmTypeEntity item)
        {
            S_ParmTypeAccess access = new S_ParmTypeAccess();
            return access.Insert(item);
        }

        /// <summary>
        /// 根据id删除参数类别记录
        /// </summary>
        /// <param name="id">关键字id</param>
        /// <returns>受影响的行数</returns>
        public int DeleteParmType(int id)
        {
            S_ParmTypeAccess access = new S_ParmTypeAccess();
            return access.Delete(id);
        }

        /// <summary>
        /// 根据关键字字段更新参数类别数据
        /// </summary>
        /// <param name="item">参数类别实体</param>
        /// <returns>受影响的记录条数</returns>
        public void UpdateParmType(S_ParmTypeEntity item)
        {
            S_ParmTypeAccess access = new S_ParmTypeAccess();
            access.Update(item, item.ID.ToString());
        }
        #endregion


    }
}
