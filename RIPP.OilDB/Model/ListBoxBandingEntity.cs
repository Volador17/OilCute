using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 范围查询以及相似查询还有应用查询的ListBox中绑定构造的类
    /// </summary>
    [Serializable]
    public class RangeSearchListBoxBandingEntity : ICloneable
    {
        #region "私有变量"

        private string _leftParenthesis = " ( ";//左括号   
        private string _tableName = "";//绑定的表的名称
        private string _firColon = " : ";//第一个冒号
        private string _itemName = "";//绑定的物性名称
        private string _secColon = " : ";//第二个冒号
        private string _data = "";//查找的数据
        private string _rihtParenthesis = " ) ";//右括号
        private string _andor = "";//存储该选项的or或者and属性

        //private string _replace = "##########################################################################";//等待被替换的字符串
        private string _replace = "############################################################################################################";//等待被替换的字符串
        private string _key = string.Empty;//ListBox中显示给客户端，绑定DisplayMember属性
        private string _value = "";//ListBox中对应的值，绑定ValueMember属性     

        private int numLeftParenthesis = 3;
        private int numTableName = 24;
        private int numItemName = 24;
        private int numData = 30;

        private OilRangeSearchEntity _oilRangeSearchEntity = null;//范围查找的非信息查找部分
        private string _oilInfoRangeSearch = string.Empty;//范围查找的信息查找     
        #endregion

        #region "构造函数"

        public RangeSearchListBoxBandingEntity()
        {

        }

        #endregion

        #region "公有变量"
        /// <summary>
        /// 左括号
        ///3个字符
        /// </summary>
        public string LeftParenthesis
        {
            get { return this._leftParenthesis; }
            set
            {
                this._leftParenthesis = value;

                //if (this._leftParenthesis != string.Empty)
                //{
                //    Replace(ref   _replace, 0, this._leftParenthesis);
                //}
                //else
                //{
                Replace(ref   _replace, 0, this._leftParenthesis);
                //}
            }
        }
        /// <summary>
        /// 绑定的表的名称
        /// 14个字符
        /// </summary>
        public string TableName
        {
            get { return this._tableName; }
            set
            {
                this._tableName = value;

                #region
                //byte [] tempLength = System.Text.Encoding.Default.GetBytes(this._tableName);

                //string temp = this._tableName;

                //for (int i = 0; i < (this.numTableName - tempLength.Length); i++)
                //{
                //    temp += " ";//如果长度不够则补充。
                //}                   

                //this._key += temp;
                #endregion

                //if (this._tableName != string.Empty)
                //{
                Replace(ref _replace, this.numLeftParenthesis, this._tableName);
                // }
                //else
                //{
                //    this._key = this._replace;
                //} 
            }
        }
        /// <summary>
        /// 第一个冒号
        ///  3个字符
        /// </summary>
        public string FirColon
        {
            get { return this._firColon; }

            set
            {
                this._firColon = value;

                //if (this._firColon != string.Empty)
                //{
                Replace(ref _replace, this.numLeftParenthesis + this.numTableName, this._firColon);
                // }
                //else
                //{
                //    this._key = this._replace;
                //} 
            }
        }

        /// <summary>
        /// 绑定的物性名称
        /// 18个字符
        /// </summary>
        public string ItemName
        {
            get { return this._itemName; }
            set
            {
                this._itemName = value;

                #region
                //byte[] tempLength = System.Text.Encoding.Default.GetBytes(this._itemName);

                //string temp = this._itemName;

                //for (int i = 0; i < (this.numItemName - tempLength.Length); i++)
                //{
                //    temp += " ";//如果长度不够则补充。
                //}          

                //this._key += temp;
                #endregion

                //if (this._itemName != string.Empty)
                //{
                Replace(ref _replace, this.numLeftParenthesis * 2 + this.numTableName, this._itemName);
                //}
                //else
                //{
                //    this._key = this._replace;
                //}   
            }
        }

        /// <summary>
        /// 第二个冒号
        ///3个字符
        /// </summary>
        public string SecColon
        {
            get { return this._secColon; }
            set
            {
                this._secColon = value;

                //if (this._secColon != string.Empty)
                //{
                Replace(ref _replace, this.numLeftParenthesis * 2 + this.numTableName + this.numItemName, this._secColon);
                //}
                //else
                //{
                //    this._key = this._replace;
                //}   
            }
        }

        /// <summary>
        /// 查找的数据分两种情况：1.原油信息的查找 2.非原油信息的查找1-100
        /// 16个字符
        /// </summary>
        public string Data
        {
            get { return this._data; }
            set
            {
                this._data = value;

                #region
                //byte[] tempLength = System.Text.Encoding.Default.GetBytes(this._data);

                //string temp = this._data;

                //for (int i = 0; i < (this.numData  - tempLength.Length); i++)
                //{
                //    temp += " ";//如果长度不够则补充。
                //}

                //this._key += temp;          
                #endregion

                //if (this._data != string.Empty)
                //{
                Replace(ref _replace, this.numLeftParenthesis * 3 + this.numTableName + this.numItemName, this._data);
                //}
                //else
                //{
                //    this._key = this._replace;
                //}  
            }
        }

        /// <summary>
        ///右括号
        ///4个字符
        /// </summary>
        public string RightParenthesis
        {
            get { return this._rihtParenthesis; }
            set
            {
                this._rihtParenthesis = value;

                //if (this._rihtParenthesis != string.Empty)
                //{
                Replace(ref _replace, this.numLeftParenthesis * 4 + this.numTableName + this.numItemName + this.numData, this._rihtParenthesis);
                //}
                //else
                //{
                //    this._key = this._replace;
                //}   
            }
        }
        /// <summary>
        /// 逻辑关系
        /// 3个字符
        /// </summary>
        public string AndOr
        {
            get { return this._andor; }
            set
            {
                this._andor = value;

                //if (this._andor != string.Empty)
                //{
                Replace(ref _replace, this.numLeftParenthesis * 5 + this.numTableName + this.numItemName + this.numData, this._andor);
                //}
                //else
                //{
                //    this._key = this._replace;
                //}  
            }
        }
        /// <summary>
        /// 代码
        /// </summary>
        public OilRangeSearchEntity OilRangeSearchEntity
        {
            get { return this._oilRangeSearchEntity; }
            set { this._oilRangeSearchEntity = value; }
        }
        /// <summary>
        /// 代码
        /// </summary>
        public string OilInfoRangeSearch
        {
            get { return this._oilInfoRangeSearch; }
            set { this._oilInfoRangeSearch = value; }
        }
        /// <summary>
        /// 用于显示值
        /// </summary>
        public string Key
        {
            get { return this._key; }

            set
            {
                //this._replace = value;
                this._key = this._replace.Replace("#", " ");

                //_key = this._replace;
            }
        }
        /// <summary>
        /// 用于查询的值
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion
        /// <summary>
        /// 字符串的指定位置的替换
        /// </summary>
        /// <param name="_replace"></param>
        /// <param name="start"></param>
        /// <param name="newStr"></param>
        private void Replace(ref string _replace, int start, string newStr)
        {
            byte[] tempoldStrLength = System.Text.Encoding.Default.GetBytes(_replace);

            if (start > (tempoldStrLength.Length - 1))
            {
                return;
            }

            byte[] tempnewStrLength = System.Text.Encoding.Default.GetBytes(newStr);

            if ((start + tempnewStrLength.Length - 1) >= tempoldStrLength.Length)
            {
                for (int i = start; i < tempoldStrLength.Length; i++)
                {
                    tempoldStrLength[start] = tempnewStrLength[i - start];
                }
            }
            else if ((start + tempnewStrLength.Length - 1) < tempoldStrLength.Length)
            {
                for (int i = 0; i < tempnewStrLength.Length; i++)
                {
                    tempoldStrLength[start + i] = tempnewStrLength[i];
                }
            }

            string temp = System.Text.Encoding.Default.GetString(tempoldStrLength);

            _replace = temp;
        }
        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 范围查询以及相似查询还有应用查询的ListBox中绑定构造的类
    /// </summary>
    [Serializable]
    public class SimilarSearchListBoxBandingEntity : ICloneable
    {
        #region "私有变量"
        private string _leftParenthesis = " ( ";//左括号   
        private string _tableName = "";//绑定的表的名称
        private string _firColon = " : ";//第一个冒号

        private string _itemName = "";//绑定的物性名称
        private string _secColon = " : ";//第二个冒号

        private string _data = "";//查找的数据
        private string _thiColon = " : ";//第三个冒号

        private string _weight = "";//查找的权重

        private string _rihtParenthesis = " ) ";//右括号
        private string _andor = "";//存储该选项的or或者and属性

        private string _key = string.Empty;//ListBox中显示给客户端，绑定DisplayMember属性
        private string _value = "";//ListBox中对应的值，绑定ValueMember属性     
        private string _replace = "################################################################################################";//等待被替换的字符串

        private int numLeftParenthesis = 3;
        private int numTableName = 24;//表的名称的字符数
        private int numItemName = 16;//物性名称的字符数
        private int numData = 12;//数据的字符数
        private int numWeight = 12;//权重的字符数

        private OilSimilarSearchEntity _oilSimilarSearchEntity = null;
        #endregion

        #region "构造函数"
        public SimilarSearchListBoxBandingEntity()
        {

        }
        #endregion

        #region "公有变量"
        /// <summary>
        /// 左括号
        ///3个字符
        /// </summary>
        public string LeftParenthesis
        {
            get { return this._leftParenthesis; }
            set
            {
                this._leftParenthesis = value;

                Replace(ref   _replace, 0, this._leftParenthesis);
            }
        }
        /// <summary>
        /// 绑定的表的名称
        /// </summary>
        public string TableName
        {
            get { return this._tableName; }
            set
            {
                this._tableName = value;

                Replace(ref _replace, this.numLeftParenthesis, this._tableName);
            }
        }
        /// <summary>
        /// 第一个冒号
        ///  3个字符
        /// </summary>
        public string FirColon
        {
            get { return this._firColon; }

            set
            {
                this._firColon = value;

                Replace(ref _replace, this.numLeftParenthesis + this.numTableName, this._firColon);
            }
        }
        /// <summary>
        /// 绑定的物性名称
        /// 18个字符
        /// </summary>
        public string ItemName
        {
            get { return this._itemName; }
            set
            {
                this._itemName = value;

                Replace(ref _replace, this.numLeftParenthesis * 2 + this.numTableName, this._itemName);
            }
        }
        /// <summary>
        /// 第二个冒号
        ///3个字符
        /// </summary>
        public string SecColon
        {
            get { return this._secColon; }
            set
            {
                this._secColon = value;

                Replace(ref _replace, this.numLeftParenthesis * 2 + this.numTableName + this.numItemName, this._secColon);
            }
        }

        /// <summary>
        /// 查找的数据分两种情况：1.原油信息的查找 2.非原油信息的查找1-100
        /// 16个字符
        /// </summary>
        public string Data
        {
            get { return this._data; }
            set
            {
                this._data = value;

                Replace(ref _replace, this.numLeftParenthesis * 3 + this.numTableName + this.numItemName, this._data);
            }
        }

        /// <summary>
        /// 第三个冒号
        /// </summary>
        public string ThiColon
        {
            get { return this._thiColon; }
            set
            {
                this._thiColon = value;

                Replace(ref _replace, this.numLeftParenthesis * 3 + this.numTableName + this.numItemName + this.numData, this._thiColon);
            }
        }

        /// <summary>
        /// 查找的数据分两种情况：1.原油信息的查找 2.非原油信息的查找1-100
        /// 16个字符
        /// </summary>
        public string Weight
        {
            get { return this._weight; }
            set
            {
                this._weight = value;

                Replace(ref _replace, this.numLeftParenthesis * 4 + this.numTableName + this.numItemName + this.numData, this._weight);
            }
        }
        /// <summary>
        ///右括号
        ///4个字符
        /// </summary>
        public string RightParenthesis
        {
            get { return this._rihtParenthesis; }
            set
            {
                this._rihtParenthesis = value;

                Replace(ref _replace, this.numLeftParenthesis * 4 + this.numTableName + this.numItemName + this.numData + this.numWeight, this._rihtParenthesis);
            }
        }
        /// <summary>
        /// 逻辑关系
        /// 3个字符
        /// </summary>
        public string AndOr
        {
            get { return this._andor; }
            set
            {
                this._andor = value;

                Replace(ref _replace, this.numLeftParenthesis * 5 + this.numTableName + this.numItemName + this.numData + this.numWeight, this._andor);
            }
        }

        /// <summary>
        /// 用于显示值
        /// </summary>
        public string Key
        {
            get { return _key; }
            set
            {
                this._key = this._replace.Replace("#", " ");
            }
        }

        /// <summary>
        /// 代码
        /// </summary>
        public OilSimilarSearchEntity OilSimilarSearchEntity
        {
            get { return this._oilSimilarSearchEntity; }
            set { this._oilSimilarSearchEntity = value; }
        }

        /// <summary>
        /// 用于查询的值
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        /// <summary>
        /// 字符串的指定位置的替换
        /// </summary>
        /// <param name="_replace"></param>
        /// <param name="start"></param>
        /// <param name="newStr"></param>
        private void Replace(ref string _replace, int start, string newStr)
        {
            byte[] tempoldStrLength = System.Text.Encoding.Default.GetBytes(_replace);

            if (start > (tempoldStrLength.Length - 1))
            {
                return;
            }

            byte[] tempnewStrLength = System.Text.Encoding.Default.GetBytes(newStr);

            if ((start + tempnewStrLength.Length - 1) >= tempoldStrLength.Length)
            {
                for (int i = start; i < tempoldStrLength.Length; i++)
                {
                    tempoldStrLength[start] = tempnewStrLength[i - start];
                }
            }
            else if ((start + tempnewStrLength.Length - 1) < tempoldStrLength.Length)
            {
                for (int i = 0; i < tempnewStrLength.Length; i++)
                {
                    tempoldStrLength[start + i] = tempnewStrLength[i];
                }
            }

            string temp = System.Text.Encoding.Default.GetString(tempoldStrLength);

            _replace = temp;
        }

        object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }
    }


}
