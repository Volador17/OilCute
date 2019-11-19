using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 数据库中保存的曲线的参数类型
    /// </summary>
    public class CurveParmTypeEntity
    {
        #region 私有变量
        private int      _id = -1;                              //Id
        private int      _oilTableTypeid = -1;          //6表示窄馏分，7表示宽馏分，8表示渣油
        private string   _typeCode = "";               //物性code
        private string   _itemCode = "";            //代码名称
        private int      _isX = -1;                  //是否作图，1表示可以做X轴，0表不作图
        private int      _show = -1;                  //是否作图，1表作图，0表不作图
        private int      _saveB = -1;                //1表示保存到B库，0表示不保存到B库
        private int      _GCCal = -1;               //1表示曲线进行GC计算，0表示曲线不进行GC计算
        private string   _descript = "";          //物性描述
        #endregion

        #region 构造函数
        public CurveParmTypeEntity()
        { 
        
        }
        #endregion

        #region 公有属性

        /// <summary>
        /// Id
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 6表示窄馏分，7表示宽馏分，8表示渣油
        /// </summary>
        public int OilTableTypeID
        {
            get { return _oilTableTypeid; }
            set { _oilTableTypeid = value; }
        }

        /// <summary>
        /// 物性code
        /// </summary>
        public string TypeCode
        {
            get { return _typeCode; }
            set { _typeCode = value; }
        }

        /// <summary>
        /// 代码名称
        /// </summary>
        public string ItemCode
        {          
            get 
            {
                if (this._itemCode == "MCP")
                    return "ECP";
                else
                    return this._itemCode;
            } 
            set { this._itemCode = value; }   
        }
        ///// <summary>
        ///// 代码名称
        ///// </summary>
        //public string GetDataItemCode
        //{
        //    get
        //    {
        //        if (this.ItemCode == "ECP")
        //            return "MCP";
        //        else
        //            return this.ItemCode;
        //    }  
        //}
        ///// <summary>
        ///// 物性描述
        ///// </summary>
        //public string GetDataDescript
        //{
        //    get
        //    {
        //        if (this.ItemCode == "ECP")
        //            return "中平均沸点";
        //        else
        //            return this.Descript;
        //    }        
        //}
        /// <summary>
        ///  
        /// </summary>
        public int IsX
        {
            get { return this._isX; }
            set { this._isX = value; }
        }

        /// <summary>
        /// 是否作图，1表作图，0表不作图
        /// </summary>
        public int Show
        {
            get { return _show; }
            set { _show = value; }
        }

        /// <summary>
        /// 1表示保存到B库，0表示不保存到B库
        /// </summary>
        public int SaveB
        {
            get { return _saveB; }
            set { _saveB = value; }
        }
        /// <summary>
        /// 1表示曲线进行GC计算，0表示曲线不进行GC计算
        /// </summary>
        public int GCCal
        {
            get { return this._GCCal; }
            set { this._GCCal = value; }
        }
        /// <summary>
        /// 物性描述
        /// </summary>
        public string Descript
        {
            get
            {
                if (this.ItemCode == "ECP")
                    return "终切点";
                else
                    return this._descript;
            }

             //get { return _descript; }
            set { _descript = value; } 
        }

        #endregion

    }
}
