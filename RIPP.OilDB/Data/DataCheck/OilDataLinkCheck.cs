using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using RIPP.Lib;
namespace RIPP.OilDB.Data.DataCheck
{
    public class OilDataLinkCheck
    {
        #region "私有变量"
        /// <summary>
        /// 传递过来的表格的类型实体
        /// </summary>
        private EnumTableType _tableType = EnumTableType.Whole;
        /// <summary>
        /// 传递过来需要审查的窗体
        /// </summary>
        private GridOilViewA _gridOil = null;
        /// <summary>
        /// 传递过来需要审查的原油性质窗体
        /// </summary>
        private GridOilViewA _wholeGridOil = null;
        /// <summary>
        /// 传递过来需要审查的轻端表窗体
        /// </summary>
        private GridOilViewA _lightGridOil = null;
        /// <summary>
        /// 传递过来需要审查的窄馏分窗体
        /// </summary>
        private GridOilViewA _narrowGridOil = null;
        /// <summary>
        /// 传递过来需要审查的宽馏分窗体
        /// </summary>
        private GridOilViewA _wideGridOil = null;
        /// <summary>
        /// 传递过来需要审查的渣馏分窗体
        /// </summary>
        private GridOilViewA _residueGridOil = null;
        /// <summary>
        /// 含有数据的最大列
        /// </summary>
        private int _maxCol = 0;
        private bool _showTip = false;
        /// <summary>
        /// 设置是否显示核算值
        /// </summary>
        public bool ShowTip
        {
            set { this._showTip = value; }
            get { return this._showTip; }
        }
        #endregion 

        #region "范围的构造函数"
        /// <summary>
        /// 范围的构造函数
        /// </summary>
        public OilDataLinkCheck()
        { 
        
        }

        /// <summary>
        /// 范围的构造函数
        /// </summary>
        /// <param name="gridOil">需要检查的表</param>
        /// <param name="tableType">设置检查表的类型</param>
        public OilDataLinkCheck(GridOilViewA wholeGridOil, GridOilViewA lightGridOil, GridOilViewA narrowGridOil, GridOilViewA wideGridOil, GridOilViewA residueGridOil)
        {
            this._wholeGridOil = wholeGridOil;
            this._narrowGridOil = narrowGridOil;
            this._wideGridOil = wideGridOil;
            this._residueGridOil = residueGridOil;
            this._lightGridOil = lightGridOil;
        }
        #endregion 

        #region "公共函数"
        /// <summary>
        /// 关联审查
        /// </summary>
        /// <param name="tableType"></param>
        public void AllDatasLinkCheck(EnumTableType tableType, bool showToolTip)
        {
             string str = string.Empty;
             this._tableType = tableType;
             if (tableType == EnumTableType.Whole)
             {
                 this._gridOil = this._wholeGridOil;
                 getMaxCol();//找到填有数据的最大列 
                 WholeLinkCheck(showToolTip);
             }
             if (tableType == EnumTableType.Narrow)
             {
                 this._gridOil = this._narrowGridOil;
                 getMaxCol();//找到填有数据的最大列 
                 NarrowGridOilLinkCheck(showToolTip);
             }
             else if (tableType == EnumTableType.Wide)
             {
                 this._gridOil = this._wideGridOil;
                 getMaxCol();//找到填有数据的最大列 
                 WideGridOilLinkCheck(showToolTip);
             }
             else if (tableType == EnumTableType.Residue)
             {
                 this._gridOil = this._residueGridOil;
                 getMaxCol();//找到填有数据的最大列 
                 ResidueGridOilLinkCheck(showToolTip);
             }                              
        }
        /// <summary>
        /// 找到填有数据的最大列找到填有数据的最大列
        /// </summary>
        private void getMaxCol()
        {
            this._maxCol = this._gridOil.GetMaxValidColumnIndex() + 1;
        }
        /// <summary>
        /// 原油性质表关联审查
        /// </summary>
        public void WholeLinkCheck(bool showToolTip)
        {
            try
            {
                if (showToolTip)
                {
                    WholeGridOilAPILinkCheck();
                    WholeGridOilD20LinkCheck();
                    WholeGridOilD15LinkCheck();
                    WholeGridOilD60LinkCheck();
                    WholeGridOilSGLinkCheck();
                    WholeGridOilV02LinkCheck();
                    WholeGridOilV04LinkCheck();
                    WholeGridOilV05LinkCheck();
                    WholeGridOilV08LinkCheck();
                    WholeGridOilV10LinkCheck();
                    WholeGridOilKFCLinkCheck();
                }
                else
                {
                    for (int j = 0; j < this._maxCol; j++)
                    {
                        this._gridOil.CancelTips("API", j);
                        this._gridOil.CancelTips("D20", j);
                        this._gridOil.CancelTips("D15", j);
                        this._gridOil.CancelTips("D60", j);
                        this._gridOil.CancelTips("SG", j);
                        this._gridOil.CancelTips("V02", j);
                        this._gridOil.CancelTips("V04", j);
                        this._gridOil.CancelTips("V05", j);
                        this._gridOil.CancelTips("V08", j);
                        this._gridOil.CancelTips("V10", j);
                        this._gridOil.CancelTips("KFC", j);
                    }
                }                            
            }
            catch (Exception ex)
            {
                Log.Error("原油性质表的关联审查" + ex);
            }
        }
        #region "原油性质关联审查"
        /// <summary>
        /// D20->API
        /// D60->API
        /// </summary>
        private void WholeGridOilAPILinkCheck()
        {
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");

            if (D60OilDataList == null && D20OilDataList == null && D60OilDataList.Count <= 0 && D20OilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (APIOilData == null)
                    continue;
                if (APIOilData.calData == APIOilData.labData && APIOilData.calData != string.Empty)
                {
                    string APIcal = string.Empty;

                    #region
                    if (APIcal == string.Empty)//存在D20实测值
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        APIcal = BaseFunction.FunAPIfromD20(D20cal);
                    }

                    if (APIcal == string.Empty)//存在D60实测值
                    {
                        string D60cal = getStrValuefromOilDataEntity(D60OilDataList, i);
                        APIcal = BaseFunction.FunAPIfromD60(D60cal);
                    }

                    if (APIcal != string.Empty && APIcal != "非数字")
                    {

                        this._gridOil.SetTips("API", i, APIcal);
                    }
                    #endregion
                }
            }
        }
        /// <summary>
        /// D60->D20
        /// API->D20
        /// </summary>
        private void WholeGridOilD20LinkCheck()
        {
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            if (D60OilDataList == null && SGOilDataList == null && APIOilDataList == null && D60OilDataList.Count <= 0 && APIOilDataList.Count <= 0 && SGOilDataList == null)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D20OilData == null)
                    continue;
                if (D20OilData.calData == D20OilData.labData && D20OilData.calData != string.Empty)
                {
                    string D20cal = string.Empty;

                    #region
                    if (D20cal == string.Empty)
                    {
                        string D60cal = getStrValuefromOilDataEntity(D60OilDataList, i);
                        D20cal = BaseFunction.FunD20fromD60(D60cal);
                    }
                    if (D20cal == string.Empty)
                    {
                        string APIcal = getStrValuefromOilDataEntity(APIOilDataList, i);
                        D20cal = BaseFunction.FunD20fromAPI(APIcal);
                    }

                    if (D20cal == string.Empty)
                    {
                        string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);
                        D20cal = BaseFunction.FunD20fromSG(SGcal);
                    }

                    if (D20cal != string.Empty && D20cal != "非数字")
                    {
                        this._gridOil.SetTips("D20", i, D20cal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// D20->D15
        /// </summary>
        private void WholeGridOilD15LinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");
            if (D20OilDataList == null && D20OilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D15OilData = D15OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D15OilData == null)
                    continue;
                if (D15OilData.calData == D15OilData.labData && D15OilData.calData != string.Empty)
                {
                    string D15cal = string.Empty;

                    #region
                    if (D15cal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        D15cal = BaseFunction.FunD15fromD20(D20cal);
                    }
                    if (D15cal != string.Empty && D15cal != "非数字")
                    {
                        OilTools oilTool = new OilTools();
                        this._gridOil.SetTips("D15", i, D15cal);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        ///D20->D60
        ///API->D60
        /// </summary>
        private void WholeGridOilD60LinkCheck()
        {
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");

            if (D20OilDataList == null && APIOilDataList == null && D20OilDataList.Count <= 0 && APIOilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D60OilData == null)
                    continue;
                if (D60OilData.calData == D60OilData.labData && D60OilData.calData != string.Empty)
                {
                    string D60cal = string.Empty;

                    #region
                    if (D60cal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        D60cal = BaseFunction.FunD60fromD20(D20cal);
                    }
                    if (D60cal == string.Empty)
                    {
                        string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);
                        D60cal = BaseFunction.FunD60fromSG(SGcal);
                    }
                    if (D60cal == string.Empty)
                    {
                        string APIcal = getStrValuefromOilDataEntity(APIOilDataList, i);
                        D60cal = BaseFunction.FunD60fromAPI(APIcal);
                    }

                    if (D60cal != string.Empty && D60cal != "非数字")
                        this._gridOil.SetTips("D60", i, D60cal);

                    #endregion
                }
            }
        }

        /// <summary>
        /// 原油性质的SG关联审查
        /// </summary>
        private void WholeGridOilSGLinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (SGOilData == null)
                    continue;
                if (SGOilData.calData == SGOilData.labData && SGOilData.calData != string.Empty)
                {
                    string SGcal = string.Empty;

                    #region
                    if (SGcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        SGcal = BaseFunction.FunSGfromD20(D20cal);
                    }

                    if (SGcal == string.Empty)
                    {
                        string D60cal = getStrValuefromOilDataEntity(D60OilDataList, i);
                        SGcal = BaseFunction.FunSG(D60cal);
                    }

                    if (SGcal != string.Empty && SGcal != "非数字")
                        this._gridOil.SetTips("SG", i, SGcal);
                }
                #endregion
            }
        }

        /// <summary>
        /// T1,V1, T2, V2, T3->V3
        /// </summary>
        private void WholeGridOilV02LinkCheck()
        {
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity V02OilData = V02OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (V02OilData == null)
                    continue;
                if (V02OilData.calData == V02OilData.labData && V02OilData.calData != string.Empty)
                {
                    string V02cal = string.Empty;

                    #region "VTList"
                    List<VT> VTList = new List<VT>();

                    string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                    string V05cal = getStrValuefromOilDataEntity(V05OilDataList, i);
                    string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                    if (!string.IsNullOrWhiteSpace(V04cal))
                    {
                        VT newVT = new VT(V04cal, 40);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V05cal))
                    {
                        VT newVT = new VT(V05cal, 50);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V08cal))
                    {
                        VT newVT = new VT(V08cal, 80);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V10cal))
                    {
                        VT newVT = new VT(V10cal, 100);
                        VTList.Add(newVT);
                    }

                    #endregion

                    if (V02cal == string.Empty && VTList.Count >= 2)
                    {
                        V02cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "20");
                    }

                    if (V02cal != string.Empty && V02cal != "非数字")
                        this._gridOil.SetTips("V02", i, V02cal);
                }                
            }
        }
        /// <summary>
        /// T1,V1, T2, V2, T3->V3
        /// </summary>
        private void WholeGridOilV04LinkCheck()
        {
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity V04OilData = V04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (V04OilData == null)
                    continue;
                if (V04OilData.calData == V04OilData.labData && V04OilData.calData != string.Empty)
                {
                    string V04cal = string.Empty;

                    #region "VTList"
                    List<VT> VTList = new List<VT>();
 
                    string V02cal = getStrValuefromOilDataEntity(V02OilDataList, i);
                    string V05cal = getStrValuefromOilDataEntity(V05OilDataList, i);
                    string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                    if (!string.IsNullOrWhiteSpace(V02cal))
                    {
                        VT newVT = new VT(V02cal, 20);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V05cal))
                    {
                        VT newVT = new VT(V05cal, 50);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V08cal))
                    {
                        VT newVT = new VT(V08cal, 80);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V10cal))
                    {
                        VT newVT = new VT(V10cal, 100);
                        VTList.Add(newVT);
                    }

                    #endregion

                    if (V04cal == string.Empty && VTList.Count >= 2)
                    {
                        V04cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "40");
                    }

                    if (V04cal != string.Empty && V04cal != "非数字")
                        this._gridOil.SetTips("V04", i, V04cal);
                }
            }
        }
        /// <summary>
        /// T1,V1, T2, V2, T3->V3
        /// </summary>
        private void WholeGridOilV05LinkCheck()
        {
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity V05OilData = V05OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (V05OilData == null)
                    continue;
                if (V05OilData.calData == V05OilData.labData && V05OilData.calData != string.Empty)
                {
                    string V05cal = string.Empty;

                    #region "VTList"
                    List<VT> VTList = new List<VT>();

                    string V02cal = getStrValuefromOilDataEntity(V02OilDataList, i);
                    string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                    string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                    if (!string.IsNullOrWhiteSpace(V02cal))
                    {
                        VT newVT = new VT(V02cal, 20);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V04cal))
                    {
                        VT newVT = new VT(V04cal, 40);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V08cal))
                    {
                        VT newVT = new VT(V08cal, 80);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V10cal))
                    {
                        VT newVT = new VT(V10cal, 100);
                        VTList.Add(newVT);
                    }

                    #endregion

                    if (V05cal == string.Empty && VTList.Count >= 2)
                    {
                        V05cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "50");
                    }

                    if (V05cal != string.Empty && V05cal != "非数字")
                        this._gridOil.SetTips("V05", i, V05cal);
                }
            }
        }
        /// <summary>
        /// T1,V1, T2, V2, T3->V3
        /// </summary>
        private void WholeGridOilV08LinkCheck()
        {
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity V08OilData = V08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (V08OilData == null)
                    continue;
                if (V08OilData.calData == V08OilData.labData && V08OilData.calData != string.Empty)
                {
                    string V08cal = string.Empty;

                    #region "VTList"
                    List<VT> VTList = new List<VT>();

                    string V02cal = getStrValuefromOilDataEntity(V02OilDataList, i);
                    string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                    string V05cal = getStrValuefromOilDataEntity(V05OilDataList, i);
                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                    if (!string.IsNullOrWhiteSpace(V02cal))
                    {
                        VT newVT = new VT(V02cal, 20);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V04cal))
                    {
                        VT newVT = new VT(V04cal, 40);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V05cal))
                    {
                        VT newVT = new VT(V05cal, 50);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V10cal))
                    {
                        VT newVT = new VT(V10cal, 100);
                        VTList.Add(newVT);
                    }

                    #endregion

                    if (V08cal == string.Empty && VTList.Count >= 2)
                    {
                        V08cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "80");
                    }

                    if (V08cal != string.Empty && V08cal != "非数字")
                        this._gridOil.SetTips("V08", i, V08cal);
                }
            }
        }
        /// <summary>
        /// T1,V1, T2, V2, T3->V3
        /// </summary>
        private void WholeGridOilV10LinkCheck()
        {
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (V10OilData == null)
                    continue;
                if (V10OilData.calData == V10OilData.labData && V10OilData.calData != string.Empty)
                {
                    string V10cal = string.Empty;

                    #region "VTList"
                    List<VT> VTList = new List<VT>();

                    string V02cal = getStrValuefromOilDataEntity(V02OilDataList, i);
                    string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                    string V05cal = getStrValuefromOilDataEntity(V05OilDataList, i);
                    string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                    if (!string.IsNullOrWhiteSpace(V02cal))
                    {
                        VT newVT = new VT(V02cal, 20);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V04cal))
                    {
                        VT newVT = new VT(V04cal, 40);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V05cal))
                    {
                        VT newVT = new VT(V05cal, 50);
                        VTList.Add(newVT);
                    }
                    if (!string.IsNullOrWhiteSpace(V08cal))
                    {
                        VT newVT = new VT(V08cal, 80);
                        VTList.Add(newVT);
                    }

                    #endregion

                    if (V10cal == string.Empty && VTList.Count >= 2)
                    {
                        V10cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "100");
                    }

                    if (V10cal != string.Empty && V10cal != "非数字")
                        this._gridOil.SetTips("V10", i, V10cal);
                }
            }
        }
        /// <summary>
        ///KFC=X/SG
        ///X=(4-SG)*(1-4.6593/(5+LN(V10)))+8.24
        ///SG=D60/DH2O60F
        /// </summary>
        private void WholeGridOilKFCLinkCheck()
        {
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> KFCOilDataList = this._gridOil.GetDataByRowItemCode("KFC");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");

            if (SGOilDataList == null && V10OilDataList == null && SGOilDataList.Count <= 0 && V10OilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity KFCOilData = KFCOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (KFCOilData == null)
                    continue;
                if (KFCOilData.calData == KFCOilData.labData && KFCOilData.calData != string.Empty)
                {
                    string KFCcal = string.Empty;

                    #region
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                    if (KFCcal == string.Empty)
                    {
                        string V05cal = getStrValuefromOilDataEntity(V05OilDataList, i);
                        KFCcal = BaseFunction.FunKFCfromV05_D20(V05cal, D20cal);
                    }
                    if (KFCcal == string.Empty)
                    {
                        string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                        KFCcal = BaseFunction.FunKFCfromV10_D20(V10cal, D20cal);
                    }
                    if (KFCcal == string.Empty)
                    {
                        string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                        string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);
                        KFCcal = BaseFunction.FunKFCfromV10_SG(V10cal, SGcal);
                    }
                    if (KFCcal != string.Empty && KFCcal != "非数字")
                        this._gridOil.SetTips("KFC", i, KFCcal);

                    #endregion
                }
            }
        }
        #endregion 
        /// <summary>
        /// 窄馏分表关联审查
        /// </summary>
        public void NarrowGridOilLinkCheck(bool showToolTip)
        {
            try
            {
                if (showToolTip)
                {
                    //NarrowGridOilICPLinkCheck();
                     NarrowGridOilWYLinkCheck();
                     NarrowGridOilTWYLinkCheck();
                     NarrowGridOilVYLinkCheck();
                     NarrowGridOilTVYLinkCheck();
                     NarrowGridOilD20LinkCheck();
                     NarrowGridOilD15LinkCheck();
                     NarrowGridOilD60LinkCheck();
                     NarrowGridOilSGLinkCheck();
                     NarrowGridOilAPILinkCheck();
                     NarrowGridOilWYDLinkCheck();
                     NarrowGridOilMWYLinkCheck();
                     NarrowGridOilMCPLinkCheck();
                     NarrowGridOilV0LinkCheck();
                     NarrowGridOilNETLinkCheck();
                     //NarrowGridOilSULLinkCheck();;//内插
                     //NarrowGridOilN2LinkCheck();;//内插
                     //NarrowGridOilCHRLinkCheck();//内插
                     //NarrowGridOilMECLinkCheck();;//内插
                     NarrowGridOilRVPLinkCheck();
                     NarrowGridOilBANLinkCheck();
                     NarrowGridOilRVPLinkCheck();
                     NarrowGridOilSMKLinkCheck();
                     NarrowGridOilFRZLinkCheck();
                     NarrowGridOilPORLinkCheck();
                     NarrowGridOilSOPLinkCheck();
                     NarrowGridOilCFPLinkCheck();
                     NarrowGridOilSAVLinkCheck();
                     NarrowGridOilARVLinkCheck();
                     NarrowGridOilANILinkCheck();
                     NarrowGridOilR20LinkCheck();
                     NarrowGridOilR70LinkCheck();
                     NarrowGridOilKFCLinkCheck();
                     NarrowGridOilBMILinkCheck();
                     NarrowGridOilDILinkCheck();
                     NarrowGridOilCILinkCheck();
                     NarrowGridOilVG4LinkCheck();
                     NarrowGridOilV1GLinkCheck();
                     NarrowGridOilMWLinkCheck();
                }
                 else
                 {
                     for (int j = 0; j < this._maxCol; j++)
                     {
                         this._gridOil.CancelTips("WY", j);
                         this._gridOil.CancelTips("TWY", j);
                         this._gridOil.CancelTips("VY", j);
                         this._gridOil.CancelTips("TVY", j);
                        
                         this._gridOil.CancelTips("API", j);
                         this._gridOil.CancelTips("D20", j);
                         this._gridOil.CancelTips("D15", j);
                         this._gridOil.CancelTips("D60", j);
                         this._gridOil.CancelTips("SG", j);
                         this._gridOil.CancelTips("V02", j);
                         this._gridOil.CancelTips("V04", j);
                         this._gridOil.CancelTips("V05", j);
                         this._gridOil.CancelTips("V08", j);
                         this._gridOil.CancelTips("V10", j);
                         this._gridOil.CancelTips("KFC", j);
                         this._gridOil.CancelTips("WYD", j);
                         this._gridOil.CancelTips("MWY", j);
                         this._gridOil.CancelTips("MCP", j);
                         this._gridOil.CancelTips("NET", j);
                         this._gridOil.CancelTips("RVP", j);
                         this._gridOil.CancelTips("BAN", j);
                         this._gridOil.CancelTips("RVP", j);
                         this._gridOil.CancelTips("SMK", j);
                         this._gridOil.CancelTips("FRZ", j);
                         this._gridOil.CancelTips("POR", j);
                         this._gridOil.CancelTips("SOP", j);
                         this._gridOil.CancelTips("CFP", j);
                         this._gridOil.CancelTips("SAV", j);
                         this._gridOil.CancelTips("ARV", j);
                         this._gridOil.CancelTips("ANI", j);
                         this._gridOil.CancelTips("R20", j);
                         this._gridOil.CancelTips("R70", j);

                         this._gridOil.CancelTips("BMI", j);
                         this._gridOil.CancelTips("DI", j);
                         this._gridOil.CancelTips("CI", j);
                         this._gridOil.CancelTips("VG4", j);
                         this._gridOil.CancelTips("V1G", j);
                         this._gridOil.CancelTips("MW", j);
                     }
                 }        
            }
            catch (Exception ex)
            {
                Log.Error("窄馏分表的数据审查" + ex);
            }
            //NarrowGridOilECPLinkCheck();
            //NarrowGridOilFPOLinkCheck();
            //
        }

        #region "窄馏分"
        private void NarrowGridOilTVYLinkCheck()
        {
            TVYLinkSupplement();
        }
        private void NarrowGridOilTWYLinkCheck()
        {
            TWYLinkSupplement();
        }
        private void NarrowGridOilWYLinkCheck()
        {
            WYLinkSupplement ();
        }

        private void NarrowGridOilVYLinkCheck()
        {
            VYLinkSupplement();
        }
        private void NarrowGridOilD20LinkCheck()
        {
            D20LinkSupplement();
        }
        private void NarrowGridOilD15LinkCheck()
        {
            D15LinkSupplement();
        }
        private void NarrowGridOilD60LinkCheck()
        {
            D60LinkSupplement();
        }

        private void NarrowGridOilSGLinkCheck()
        {
            SGLinkSupplement();
        }
        private void NarrowGridOilAPILinkCheck()
        {
            APILinkSupplement();
        }
        private void NarrowGridOilWYDLinkCheck()
        {
            WYDLinkSupplement();
        }
        private void NarrowGridOilMWYLinkCheck()
        {
            MWYLinkSupplement();
        }
        private void NarrowGridOilMCPLinkCheck()
        {
            MCPLinkSupplement();
        }
        private void NarrowGridOilV0LinkCheck()
        {
            V0LinkSupplement();
        }

        private void NarrowGridOilNETLinkCheck()
        {
            NETLinkSupplement();
        }
        private void NarrowGridOilSULLinkCheck()
        {
            SULLinkSupplement();
        }

        private void NarrowGridOilN2LinkCheck()
        {
            N2LinkSupplement();
        }

        private void NarrowGridOilRVPLinkCheck()
        {
            RVPLinkSupplement();
        }
        private void NarrowGridOilBANLinkCheck()
        {
            BANLinkSupplement();
        }

        private void NarrowGridOilSMKLinkCheck()
        {
            SMKLinkSupplement();
        }
        private void NarrowGridOilFRZLinkCheck()
        {
            FRZLinkSupplement();
        }
        private void NarrowGridOilPORLinkCheck()
        {
            PORLinkSupplement();
        }

        private void NarrowGridOilSOPLinkCheck()
        {
            SOPLinkSupplement();
        }

        private void NarrowGridOilCFPLinkCheck()
        {
            CFPLinkSupplement();
        }
        private void NarrowGridOilSAVLinkCheck()
        {
            SAVLinkSupplement();
        }
        private void NarrowGridOilARVLinkCheck()
        {
            ARVLinkSupplement();
        }
        private void NarrowGridOilANILinkCheck()
        {
            ANILinkSupplement();
        }
        private void NarrowGridOilR20LinkCheck()
        {
            R20LinkSupplement();
        }

        private void NarrowGridOilR70LinkCheck()
        {
            R70LinkSupplement();
        }

        private void NarrowGridOilKFCLinkCheck()
        {
            KFCLinkSupplement();
        }
        private void NarrowGridOilBMILinkCheck()
        {
            BMILinkSupplement();
        }
        private void NarrowGridOilDILinkCheck()
        {
            DILinkSupplement();
        }
        private void NarrowGridOilCILinkCheck()
        {
            CILinkSupplement();
        }
        private void NarrowGridOilVG4LinkCheck()
        {
            VG4LinkSupplement();
        }
        private void NarrowGridOilV1GLinkCheck()
        {
            V1GLinkSupplement();
        }
        private void NarrowGridOilMWLinkCheck()
        {
            MWLinkSupplement();
        }
        #endregion 
        /// <summary>
        /// 宽馏分表关联审查
        /// </summary>
        public void WideGridOilLinkCheck(bool showToolTip)
        {
            try
            {
                if (showToolTip)
                {
                    WideGridOilWYLinkCheck();
                    WideGridOilTWYLinkCheck();
                    WideGridOilVYLinkCheck();
                    WideGridOilTVYLinkCheck();
                    WideGridOilAPILinkCheck();
                    WideGridOilD20LinkCheck();
                    WideGridOilD60LinkCheck();
                    WideGridOilD15LinkCheck();
                    WideGridOilD70LinkCheck();
                    WideGridOilSGLinkCheck();
                    WideGridOilWYDLinkCheck();
                    WideGridOilMWYLinkCheck();
                    WideGridOilMCPLinkCheck();
                    WideGridOilV0LinkCheck();
                    WideGridOilVILinkCheck();
                    WideGridOilVG4LinkCheck();
                    WideGridOilV1GLinkCheck();
                    WideGridOilR20LinkCheck();
                    WideGridOilR70LinkCheck();
                    WideGridOilC_HLinkCheck();
                    WideGridOilSULLinkCheck();
                    WideGridOilN2LinkCheck();
                    WideGridOilBANLinkCheck();
                    WideGridOilMECLinkCheck();
                    WideGridOilNETLinkCheck();
                    WideGridOilACDLinkCheck();
                    WideGridOilMWLinkCheck();
                    //WideGridOilA_PLinkCheck();//馏程算法,TVY-ECP曲线=>AIP, A10, A30, A50,A70,A90,AEP
                    WideGridOilKFCLinkCheck();
                    WideGridOilBMILinkCheck();
                    WideGridOilANILinkCheck();
                    WideGridOilPANLinkCheck();
                    WideGridOilPAOLinkCheck();
                    WideGridOilNAHLinkCheck();
                    WideGridOilARMLinkCheck();
                    WideGridOilGCTLinkCheck();
                    WideGridOilARPLinkCheck();
                    WideGridOilN2ALinkCheck();
                    WideGridOilCHRLinkCheck();
                    WideGridOilRVPLinkCheck();
                    WideGridOilFRZLinkCheck();
                    WideGridOilSMKLinkCheck();
                    WideGridOilSAVLinkCheck();
                    WideGridOilARVLinkCheck();
                    WideGridOilLHVLinkCheck();
                    WideGridOilIRTLinkCheck();
                    WideGridOilPORLinkCheck();
                    WideGridOilSOPLinkCheck();
                    WideGridOilCLPLinkCheck();
                    WideGridOilCILinkCheck();
                    WideGridOilCENLinkCheck();
                    WideGridOilDILinkCheck();
                    WideGridOilSAHLinkCheck();
                    WideGridOilARSLinkCheck();
                    WideGridOilRESLinkCheck();
                    WideGridOilAPHLinkCheck();
                    WideGridOil4CTLinkCheck();
                    WideGridOilCPPLinkCheck();
                    WideGridOilCNNLinkCheck();
                    WideGridOilCAALinkCheck();
                    WideGridOilRTTLinkCheck();
                    WideGridOilRNNLinkCheck();
                    WideGridOilRAALinkCheck();
                    WideGridOilPATLinkCheck();
                    WideGridOilMNALinkCheck();
                    WideGridOilMSPLinkCheck();
                    WideGridOilMA1LinkCheck();
                    WideGridOilMA2LinkCheck();
                    WideGridOilMA3LinkCheck();
                    WideGridOilMA4LinkCheck();
                    WideGridOilMA5LinkCheck();
                    WideGridOilMANLinkCheck();
                    WideGridOilMATLinkCheck();
                    WideGridOilMTALinkCheck();
                }
                else
                {
                    for (int j = 0; j < this._maxCol; j++)
                    {
                        this._gridOil.CancelTips("WY", j);
                        this._gridOil.CancelTips("TWY", j);
                        this._gridOil.CancelTips("VY", j);
                        this._gridOil.CancelTips("TVY", j);
                        this._gridOil.CancelTips("API", j);
                        this._gridOil.CancelTips("D20", j);
                        this._gridOil.CancelTips("D60", j);
                        this._gridOil.CancelTips("D15", j);
                        this._gridOil.CancelTips("D70", j);
                        this._gridOil.CancelTips("SG", j);
                        this._gridOil.CancelTips("WYD", j);
                        this._gridOil.CancelTips("MWY", j);
                        this._gridOil.CancelTips("MCP", j);
                        this._gridOil.CancelTips("V02", j);
                        this._gridOil.CancelTips("V04", j);
                        this._gridOil.CancelTips("V05", j);
                        this._gridOil.CancelTips("V08", j);
                        this._gridOil.CancelTips("V10", j);
                        this._gridOil.CancelTips("VI", j);
                        this._gridOil.CancelTips("VG4", j);
                        this._gridOil.CancelTips("V1G", j);
                        this._gridOil.CancelTips("R20", j);
                        this._gridOil.CancelTips("R70", j);
                        this._gridOil.CancelTips("MW", j);
                        this._gridOil.CancelTips("C/H", j);
                        this._gridOil.CancelTips("SUL", j); 
                        this._gridOil.CancelTips("N2", j); 
                        this._gridOil.CancelTips("BAN", j);
                        this._gridOil.CancelTips("MEC", j);
                        this._gridOil.CancelTips("NET", j);
                        this._gridOil.CancelTips("ACD", j);
                        this._gridOil.CancelTips("KFC", j);
                        this._gridOil.CancelTips("BMI", j);
                        this._gridOil.CancelTips("ANI", j);
                        this._gridOil.CancelTips("PAN", j);
                        this._gridOil.CancelTips("PAO", j);
                        this._gridOil.CancelTips("NAH", j);
                        this._gridOil.CancelTips("ARM", j);
                        this._gridOil.CancelTips("GCT", j);
                        this._gridOil.CancelTips("ARP", j);
                        this._gridOil.CancelTips("N2A", j);
                        this._gridOil.CancelTips("CHR", j);
                        this._gridOil.CancelTips("RVP", j);
                        this._gridOil.CancelTips("FRZ", j);
                        this._gridOil.CancelTips("SMK", j);
                        this._gridOil.CancelTips("SAV", j);
                        this._gridOil.CancelTips("ARV", j);
                        this._gridOil.CancelTips("LHV", j);
                        this._gridOil.CancelTips("IRT", j);
                        this._gridOil.CancelTips("POR", j);
                        this._gridOil.CancelTips("SOP", j); 
                        this._gridOil.CancelTips("CLP", j);
                        this._gridOil.CancelTips("DI", j);
                        this._gridOil.CancelTips("CI", j);
                        this._gridOil.CancelTips("CEN", j);
                        this._gridOil.CancelTips("SAH", j);
                        this._gridOil.CancelTips("ARS", j);
                        this._gridOil.CancelTips("RES", j);
                        this._gridOil.CancelTips("APH", j);
                        this._gridOil.CancelTips("4CT", j);
                        this._gridOil.CancelTips("CPP", j);
                        this._gridOil.CancelTips("CNN", j);
                        this._gridOil.CancelTips("CAA", j);
                        this._gridOil.CancelTips("RTT", j);
                        this._gridOil.CancelTips("RNN", j);
                        this._gridOil.CancelTips("RAA", j);
                        this._gridOil.CancelTips("PAT", j);
                        this._gridOil.CancelTips("MNA", j);
                        this._gridOil.CancelTips("MSP", j);
                        this._gridOil.CancelTips("MA1", j);
                        this._gridOil.CancelTips("MA2", j);
                        this._gridOil.CancelTips("MA3", j);
                        this._gridOil.CancelTips("MA4", j);
                        this._gridOil.CancelTips("MA5", j);
                        this._gridOil.CancelTips("MAN", j);
                        this._gridOil.CancelTips("MAT", j);
                        this._gridOil.CancelTips("MTA", j);
                    }
                }        
            }
            catch (Exception ex)
            {
                Log.Error("宽馏分表的数据审查" + ex);
            }
            // WideGridOilFPOLinkCheck();
            //WideGridOilPATLinkCheck();
            //
        }

        #region "宽馏分"
        private void WideGridOilWYLinkCheck()
        {
            WYLinkSupplement();
        }
        private void WideGridOilTWYLinkCheck()
        {
            TWYLinkSupplement();
        }
        private void WideGridOilVYLinkCheck()
        {
            VYLinkSupplement();
        }
        private void WideGridOilTVYLinkCheck()
        {
            TVYLinkSupplement();
        }

        private void WideGridOilAPILinkCheck()
        {
            APILinkSupplement();
        }
        private void WideGridOilD20LinkCheck()
        {
            D20LinkSupplement();
        }
        private void WideGridOilD60LinkCheck()
        {
            D60LinkSupplement();
        }
        private void WideGridOilD15LinkCheck()
        {
            D15LinkSupplement();
        }
        private void WideGridOilD70LinkCheck()
        {
            D70LinkSupplement();
        }
        private void WideGridOilSGLinkCheck()
        {
            SGLinkSupplement();
        }
        private void WideGridOilWYDLinkCheck()
        {
            WYDLinkSupplement();
        }
        private void WideGridOilMWYLinkCheck()
        {
            //MWYLinkSupplement();
        }
        private void WideGridOilMCPLinkCheck()
        {
            //MCPLinkSupplement();
        }
        private void WideGridOilV0LinkCheck()
        {
            V0LinkSupplement();
        }
        private void WideGridOilVILinkCheck()
        {
            List<OilDataEntity> VIOilDataList = this._gridOil.GetDataByRowItemCode("VI");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity VIOilData = VIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (VIOilData == null)
                    continue;

                if (VIOilData.calData == VIOilData.labData && VIOilData.calData != string.Empty)
                {
                    string VIcal = string.Empty;
                    if (VIcal == string.Empty)
                    {
                        OilDataEntity V04OilData = V04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (V04OilData != null && V10OilData != null)
                            VIcal = BaseFunction.FunVIfromV04_V10(V04OilData.calData, V10OilData.calData);
                    }
                    if (VIcal != string.Empty && VIcal != "非数字")
                    {
                        this._gridOil.SetTips("VI", i, VIcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilVG4LinkCheck()
        {
            VG4LinkSupplement();
        }
        private void WideGridOilV1GLinkCheck()
        {
            V1GLinkSupplement();
        }
        private void WideGridOilR20LinkCheck()
        {
            R20LinkSupplement();
        }
        private void WideGridOilR70LinkCheck()
        {
            R70LinkSupplement();
        }
        private void WideGridOilC_HLinkCheck()
        {
            List<OilDataEntity> C_HOilDataList = this._gridOil.GetDataByRowItemCode("C/H");

            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");

            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");

            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity C_HOilData = C_HOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (C_HOilData == null)
                    continue;

                if (C_HOilData.calData == C_HOilData.labData && C_HOilData.calData != string.Empty)
                {
                    string C_Hcal = string.Empty;
                    if (C_Hcal == string.Empty)
                    {
                        OilDataEntity CAROilData = CAROilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity H2OilData = H2OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (CAROilData != null && H2OilData != null)
                            C_Hcal = BaseFunction.FunC_H(CAROilData.calData, H2OilData.calData);
                    }
                    if (C_Hcal == string.Empty)
                    {
                        OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (SGOilData != null && MCPOilData != null)
                            C_Hcal = BaseFunction.FunC1HfromSG_MCP(SGOilData.calData, MCPOilData.calData);
                    }

                    if (C_Hcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D20OilData != null && MCPOilData != null)
                            C_Hcal = BaseFunction.FunC1HfromD20_MCP(D20OilData.calData, MCPOilData.calData);
                    }

                    if (C_Hcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                            C_Hcal = BaseFunction.FunC1HfromD20_A10_A30_A50_A70_A90(D20OilData.calData, A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData);
                    }
                    if (C_Hcal != string.Empty && C_Hcal != "非数字")
                    {
                        this._gridOil.SetTips("C/H", i, C_Hcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilSULLinkCheck()
        {
            SULLinkSupplement();
        }
        private void WideGridOilN2LinkCheck()
        {
            N2LinkSupplement();
        }
        private void WideGridOilBANLinkCheck()
        {
            BANLinkSupplement();
        }
        private void WideGridOilMECLinkCheck()
        {
            //MECLinkSupplement();
        }
        private void WideGridOilNETLinkCheck()
        {
            NETLinkSupplement();
        }
        private void WideGridOilACDLinkCheck()
        {
            //ACDLinkSupplement();
        }
        private void WideGridOilMWLinkCheck()
        {
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
          
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
           
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity MWOilData = MWOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MWOilData == null)
                    continue;

                if (MWOilData.calData == MWOilData.labData && MWOilData.calData != string.Empty)
                {
                    string MWcal = string.Empty;

                    if (MWcal == string.Empty)
                    {
                        OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (SGOilData != null && MCPOilData != null)
                            MWcal = BaseFunction.FunMWfromMCP_SG(MCPOilData.calData, SGOilData.calData);
                    }
                    if (MWcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                            MWcal = BaseFunction.FunMWfromA10_A30_A50_A70_A90_D20( A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData,D20OilData.calData);
                    }

                    if (MWcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V04OilData = V04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (V04OilData != null && V10OilData != null && D20OilData != null)
                            MWcal = BaseFunction.FunMWfromD20_V04_V10(D20OilData.calData, V04OilData.calData, V10OilData.calData);
                    }
                    if (MWcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V08OilData = V08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (V08OilData != null && V10OilData != null && D20OilData != null)
                            MWcal = BaseFunction.FunMWfromD20_V08_V10(D20OilData.calData, V08OilData.calData, V10OilData.calData);
                    }
                   
                    if (MWcal != string.Empty && MWcal != "非数字")
                    {
                        this._gridOil.SetTips("MW", i, MWcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilKFCLinkCheck()
        {
            List<OilDataEntity> KFCOilDataList = this._gridOil.GetDataByRowItemCode("KFC");

            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");

            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity KFCOilData = KFCOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (KFCOilData == null)
                    continue;

                if (KFCOilData.calData == KFCOilData.labData && KFCOilData.calData != string.Empty)
                {
                    string KFCcal = string.Empty;

                    if (KFCcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D20OilData != null && MCPOilData != null)
                            KFCcal = BaseFunction.FunKFCfromMCP_D20(MCPOilData.calData, D20OilData.calData);
                    }
                    if (KFCcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                            KFCcal = BaseFunction.FunKFCfromA10A30A50A70A90_D20(A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData, D20OilData.calData);
                    }
 
                    if (KFCcal != string.Empty && KFCcal != "非数字")
                    {
                        this._gridOil.SetTips("KFC", i, KFCcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilBMILinkCheck()
        {
            List<OilDataEntity> BMIOilDataList = this._gridOil.GetDataByRowItemCode("BMI");

            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");

            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity BMIOilData = BMIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (BMIOilData == null)
                    continue;

                if (BMIOilData.calData == BMIOilData.labData && BMIOilData.calData != string.Empty)
                {
                    string BMIcal = string.Empty;

                    if (BMIcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D20OilData != null && MCPOilData != null)
                            BMIcal = BaseFunction.FunBMIfromMCP_D20(MCPOilData.calData, D20OilData.calData);
                    }
                    if (BMIcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                            BMIcal = BaseFunction.FunBMIfromA10A30A50A70A90_D20(A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData, D20OilData.calData);
                    }

                    if (BMIcal != string.Empty && BMIcal != "非数字")
                    {
                        this._gridOil.SetTips("BMI", i, BMIcal);
                    }
                }
                #endregion
            } 
        }
        private void WideGridOilANILinkCheck()
        {
            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");

            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");

            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity ANIOilData = ANIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (ANIOilData == null)
                    continue;

                if (ANIOilData.calData == ANIOilData.labData && ANIOilData.calData != string.Empty)
                {
                    string ANIcal = string.Empty;

                    if (ANIcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D20OilData != null && MCPOilData != null)
                            ANIcal = BaseFunction.FunANIfromD20_MCP(D20OilData.calData, MCPOilData.calData);
                    }
                    if (ANIcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                            ANIcal = BaseFunction.FunANIfromD20_A10_A30_A50_A70_A90(A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData, D20OilData.calData);
                    }

                    if (ANIcal != string.Empty && ANIcal != "非数字")
                    {
                        this._gridOil.SetTips("ANI", i, ANIcal);
                    }
                }
                #endregion
            } 
        }
        private void WideGridOilPANLinkCheck()
        {
            List<OilDataEntity> PANOilDataList = this._gridOil.GetDataByRowItemCode("PAN");

            List<OilDataEntity> P03OilDataList = this._gridOil.GetDataByRowItemCode("P03");
            List<OilDataEntity> P04OilDataList = this._gridOil.GetDataByRowItemCode("P04");
            List<OilDataEntity> P05OilDataList = this._gridOil.GetDataByRowItemCode("P05");
            List<OilDataEntity> P06OilDataList = this._gridOil.GetDataByRowItemCode("P06");
            List<OilDataEntity> P07OilDataList = this._gridOil.GetDataByRowItemCode("P07");
            List<OilDataEntity> P08OilDataList = this._gridOil.GetDataByRowItemCode("P08");
            List<OilDataEntity> P09OilDataList = this._gridOil.GetDataByRowItemCode("P09");
            List<OilDataEntity> P10OilDataList = this._gridOil.GetDataByRowItemCode("P10");
            List<OilDataEntity> P11OilDataList = this._gridOil.GetDataByRowItemCode("P11");
            List<OilDataEntity> P12OilDataList = this._gridOil.GetDataByRowItemCode("P12");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity PANOilData = PANOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (PANOilData == null)
                    continue;

                if (PANOilData.calData == PANOilData.labData && PANOilData.calData != string.Empty)
                {
                    string PANcal = string.Empty;

                    if (PANcal == string.Empty)
                    {
                        OilDataEntity P03OilData = P03OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P04OilData = P04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P05OilData = P05OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P06OilData = P06OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P07OilData = P07OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P08OilData = P08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P09OilData = P09OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P10OilData = P10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P11OilData = P11OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity P12OilData = P12OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (P03OilData != null && P04OilData != null && P05OilData != null && P06OilData != null
                            && P07OilData != null && P08OilData != null && P09OilData != null
                            && P10OilData != null && P11OilData != null && P12OilData != null)
                        {
                            List<string> tempList = new List<string>();
                            tempList.Add(P03OilData.calData);
                            tempList.Add(P04OilData.calData);
                            tempList.Add(P05OilData.calData);
                            tempList.Add(P06OilData.calData);
                            tempList.Add(P07OilData.calData);
                            tempList.Add(P08OilData.calData);
                            tempList.Add(P09OilData.calData);
                            tempList.Add(P10OilData.calData);
                            tempList.Add(P11OilData.calData);
                            tempList.Add(P12OilData.calData);
                            PANcal = BaseFunction.FunSumAllowEmpty(tempList);
                        }
                    }
                    
                    if (PANcal != string.Empty && PANcal != "非数字")
                    {
                        this._gridOil.SetTips("PAN", i, PANcal);
                    }
                }
                #endregion
            } 
        }
        private void WideGridOilPAOLinkCheck()
        {
            List<OilDataEntity> PAOOilDataList = this._gridOil.GetDataByRowItemCode("PAO");

            List<OilDataEntity> I03OilDataList = this._gridOil.GetDataByRowItemCode("I03");
            List<OilDataEntity> I04OilDataList = this._gridOil.GetDataByRowItemCode("I04");
            List<OilDataEntity> I05OilDataList = this._gridOil.GetDataByRowItemCode("I05");
            List<OilDataEntity> I06OilDataList = this._gridOil.GetDataByRowItemCode("I06");
            List<OilDataEntity> I07OilDataList = this._gridOil.GetDataByRowItemCode("I07");
            List<OilDataEntity> I08OilDataList = this._gridOil.GetDataByRowItemCode("I08");
            List<OilDataEntity> I09OilDataList = this._gridOil.GetDataByRowItemCode("I09");
            List<OilDataEntity> I10OilDataList = this._gridOil.GetDataByRowItemCode("I10");
            List<OilDataEntity> I11OilDataList = this._gridOil.GetDataByRowItemCode("I11");
            List<OilDataEntity> I12OilDataList = this._gridOil.GetDataByRowItemCode("I12");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity PAOOilData = PAOOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (PAOOilData == null)
                    continue;

                if (PAOOilData.calData == PAOOilData.labData && PAOOilData.calData != string.Empty)
                {
                    string PAOcal = string.Empty;

                    if (PAOcal == string.Empty)
                    {
                        OilDataEntity I03OilData = I03OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I04OilData = I04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I05OilData = I05OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I06OilData = I06OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I07OilData = I07OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I08OilData = I08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I09OilData = I09OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I10OilData = I10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I11OilData = I11OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity I12OilData = I12OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (I03OilData != null && I04OilData != null && I05OilData != null && I06OilData != null
                            && I07OilData != null && I08OilData != null && I09OilData != null
                            && I10OilData != null && I11OilData != null && I12OilData != null)
                        {
                            List<string> tempList = new List<string>();
                            tempList.Add(I03OilData.calData);
                            tempList.Add(I04OilData.calData);
                            tempList.Add(I05OilData.calData);
                            tempList.Add(I06OilData.calData);
                            tempList.Add(I07OilData.calData);
                            tempList.Add(I08OilData.calData);
                            tempList.Add(I09OilData.calData);
                            tempList.Add(I10OilData.calData);
                            tempList.Add(I11OilData.calData);
                            tempList.Add(I12OilData.calData);
                            PAOcal = BaseFunction.FunSumAllowEmpty(tempList);
                        }
                    }

                    if (PAOcal != string.Empty && PAOcal != "非数字")
                    {
                        this._gridOil.SetTips("PAO", i, PAOcal);
                    }
                }
                #endregion
            } 
        }
        private void WideGridOilNAHLinkCheck()
        {
            List<OilDataEntity> NAHOilDataList = this._gridOil.GetDataByRowItemCode("NAH");

            List<OilDataEntity> N03OilDataList = this._gridOil.GetDataByRowItemCode("N03");
            List<OilDataEntity> N04OilDataList = this._gridOil.GetDataByRowItemCode("N04");
            List<OilDataEntity> N05OilDataList = this._gridOil.GetDataByRowItemCode("N05");
            List<OilDataEntity> N06OilDataList = this._gridOil.GetDataByRowItemCode("N06");
            List<OilDataEntity> N07OilDataList = this._gridOil.GetDataByRowItemCode("N07");
            List<OilDataEntity> N08OilDataList = this._gridOil.GetDataByRowItemCode("N08");
            List<OilDataEntity> N09OilDataList = this._gridOil.GetDataByRowItemCode("N09");
            List<OilDataEntity> N10OilDataList = this._gridOil.GetDataByRowItemCode("N10");
            List<OilDataEntity> N11OilDataList = this._gridOil.GetDataByRowItemCode("N11");
            List<OilDataEntity> N12OilDataList = this._gridOil.GetDataByRowItemCode("N12");

            List<OilDataEntity> MNAOilDataList = this._gridOil.GetDataByRowItemCode("MNA");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity NAHOilData = NAHOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (NAHOilData == null)
                    continue;

                if (NAHOilData.calData == NAHOilData.labData && NAHOilData.calData != string.Empty)
                {
                    string NAHcal = string.Empty;

                    if (NAHcal == string.Empty)
                    {
                        OilDataEntity N03OilData = N03OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N04OilData = N04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N05OilData = N05OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N06OilData = N06OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N07OilData = N07OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N08OilData = N08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N09OilData = N09OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N10OilData = N10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N11OilData = N11OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N12OilData = N12OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (N03OilData != null && N04OilData != null && N05OilData != null && N06OilData != null
                            && N07OilData != null && N08OilData != null && N09OilData != null
                            && N10OilData != null && N11OilData != null && N12OilData != null)
                        {
                            List<string> tempList = new List<string>();
                            tempList.Add(N03OilData.calData);
                            tempList.Add(N04OilData.calData);
                            tempList.Add(N05OilData.calData);
                            tempList.Add(N06OilData.calData);
                            tempList.Add(N07OilData.calData);
                            tempList.Add(N08OilData.calData);
                            tempList.Add(N09OilData.calData);
                            tempList.Add(N10OilData.calData);
                            tempList.Add(N11OilData.calData);
                            tempList.Add(N12OilData.calData);
                            NAHcal = BaseFunction.FunSumAllowEmpty(tempList);
                        }
                    }
                    if (NAHcal == string.Empty)
                    {
                        OilDataEntity MNAOilData = MNAOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (MNAOilData != null)
                            NAHcal = MNAOilData.calData;
                    }

                    if (NAHcal != string.Empty && NAHcal != "非数字")
                    {
                        this._gridOil.SetTips("NAH", i, NAHcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilARMLinkCheck()
        {
            List<OilDataEntity> ARMOilDataList = this._gridOil.GetDataByRowItemCode("ARM");

            List<OilDataEntity> A03OilDataList = this._gridOil.GetDataByRowItemCode("A03");
            List<OilDataEntity> A04OilDataList = this._gridOil.GetDataByRowItemCode("A04");
            List<OilDataEntity> A05OilDataList = this._gridOil.GetDataByRowItemCode("A05");
            List<OilDataEntity> A06OilDataList = this._gridOil.GetDataByRowItemCode("A06");
            List<OilDataEntity> A07OilDataList = this._gridOil.GetDataByRowItemCode("A07");
            List<OilDataEntity> A08OilDataList = this._gridOil.GetDataByRowItemCode("A08");
            List<OilDataEntity> A09OilDataList = this._gridOil.GetDataByRowItemCode("A09");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("10A");
            List<OilDataEntity> A11OilDataList = this._gridOil.GetDataByRowItemCode("A11");
            List<OilDataEntity> A12OilDataList = this._gridOil.GetDataByRowItemCode("A12");

            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity ARMOilData = ARMOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (ARMOilData == null)
                    continue;

                if (ARMOilData.calData == ARMOilData.labData && ARMOilData.calData != string.Empty)
                {
                    string ARMcal = string.Empty;

                    if (ARMcal == string.Empty)
                    {
                        OilDataEntity A03OilData = A03OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A04OilData = A04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A05OilData = A05OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A06OilData = A06OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A07OilData = A07OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A08OilData = A08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A09OilData = A09OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A11OilData = A11OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A12OilData = A12OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (A03OilData != null && A04OilData != null && A05OilData != null && A06OilData != null
                            && A07OilData != null && A08OilData != null && A09OilData != null
                            && A10OilData != null && A11OilData != null && A12OilData != null)
                        {
                            List<string> tempList = new List<string>();
                            tempList.Add(A03OilData.calData);
                            tempList.Add(A04OilData.calData);
                            tempList.Add(A05OilData.calData);
                            tempList.Add(A06OilData.calData);
                            tempList.Add(A07OilData.calData);
                            tempList.Add(A08OilData.calData);
                            tempList.Add(A09OilData.calData);
                            tempList.Add(A10OilData.calData);
                            tempList.Add(A11OilData.calData);
                            tempList.Add(A12OilData.calData);
                            ARMcal = BaseFunction.FunSumAllowEmpty(tempList);
                        }
                    }
                    if (ARMcal == string.Empty)
                    {
                        OilDataEntity MATOilData = MATOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (MATOilData != null)
                            ARMcal = MATOilData.calData;
                    }

                    if (ARMcal != string.Empty && ARMcal != "非数字")
                    {
                        this._gridOil.SetTips("ARM", i, ARMcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilGCTLinkCheck()
        {
            List<OilDataEntity> GCTOilDataList = this._gridOil.GetDataByRowItemCode("GCT");
            //:::::
            List<OilDataEntity> PANOilDataList = this._gridOil.GetDataByRowItemCode("PAN");
            List<OilDataEntity> PAOOilDataList = this._gridOil.GetDataByRowItemCode("PAO");
            List<OilDataEntity> NAHOilDataList = this._gridOil.GetDataByRowItemCode("NAH");
            List<OilDataEntity> ARMOilDataList = this._gridOil.GetDataByRowItemCode("ARM");
            List<OilDataEntity> OLEOilDataList = this._gridOil.GetDataByRowItemCode("OLE");
            List<OilDataEntity> UNKOilDataList = this._gridOil.GetDataByRowItemCode("UNK");
            
            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity GCTOilData = GCTOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (GCTOilData == null)
                    continue;

                if (GCTOilData.calData == GCTOilData.labData && GCTOilData.calData != string.Empty)
                {
                    string GCTcal = string.Empty;

                    if (GCTcal == string.Empty)
                    {
                        OilDataEntity PANOilData = PANOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity PAOOilData = PAOOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity NAHOilData = NAHOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ARMOilData = ARMOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity OLEOilData = OLEOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity UNKOilData = UNKOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                         
                        if (PANOilData != null && PAOOilData != null && NAHOilData != null
                            && ARMOilData != null && OLEOilData != null && UNKOilData != null)                          
                        {
                            List<string> tempList = new List<string>();
                            tempList.Add(PANOilData.calData);
                            tempList.Add(PAOOilData.calData);
                            tempList.Add(NAHOilData.calData);
                            tempList.Add(ARMOilData.calData);
                            tempList.Add(OLEOilData.calData);
                            tempList.Add(UNKOilData.calData);
 
                            GCTcal = BaseFunction.FunSumAllowEmpty(tempList);
                        }
                    }

                    if (GCTcal != string.Empty && GCTcal != "非数字")
                    {
                        this._gridOil.SetTips("GCT", i, GCTcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilARPLinkCheck()
        {
            List<OilDataEntity> ARPOilDataList = this._gridOil.GetDataByRowItemCode("ARP");
            //:::::
            List<OilDataEntity> N06OilDataList = this._gridOil.GetDataByRowItemCode("N06");
            List<OilDataEntity> N07OilDataList = this._gridOil.GetDataByRowItemCode("N07");
            List<OilDataEntity> N08OilDataList = this._gridOil.GetDataByRowItemCode("N08");
            List<OilDataEntity> A06OilDataList = this._gridOil.GetDataByRowItemCode("A06");
            List<OilDataEntity> A07OilDataList = this._gridOil.GetDataByRowItemCode("A07");
            List<OilDataEntity> A08OilDataList = this._gridOil.GetDataByRowItemCode("A08");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity ARPOilData = ARPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (ARPOilData == null)
                    continue;

                if (ARPOilData.calData == ARPOilData.labData && ARPOilData.calData != string.Empty)
                {
                    string ARPcal = string.Empty;

                    if (ARPcal == string.Empty)
                    {
                        OilDataEntity N06OilData = N06OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N07OilData = N07OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity N08OilData = N08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A06OilData = A06OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A07OilData = A07OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A08OilData = A08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (N06OilData != null && N07OilData != null && N08OilData != null
                            && A06OilData != null && A07OilData != null && A08OilData != null)
                        {
                            ARPcal = BaseFunction.FunARP(N06OilData.calData, N07OilData.calData, N08OilData.calData, A06OilData.calData, A07OilData.calData, A08OilData.calData);
                        }
                    }

                    if (ARPcal != string.Empty && ARPcal != "非数字")
                    {
                        this._gridOil.SetTips("ARP", i, ARPcal);
                    }
                }
                #endregion
            }
        }    
        private void WideGridOilN2ALinkCheck()
        {
            List<OilDataEntity> N2AOilDataList = this._gridOil.GetDataByRowItemCode("N2A");
            //:::::N2A=+2*ARM
            List<OilDataEntity> NAHOilDataList = this._gridOil.GetDataByRowItemCode("NAH");
            List<OilDataEntity> ARMOilDataList = this._gridOil.GetDataByRowItemCode("ARM");
          
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity N2AOilData = N2AOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (N2AOilData == null)
                    continue;

                if (N2AOilData.calData == N2AOilData.labData && N2AOilData.calData != string.Empty)
                {
                    string N2Acal = string.Empty;

                    if (N2Acal == string.Empty)
                    {
                        OilDataEntity NAHOilData = NAHOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ARMOilData = ARMOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                       
                        if (NAHOilData != null && ARMOilData != null  )
                        {
                            float ARM = 0; float fN2A = 0; float NAH = 0;
                            if (float.TryParse(ARMOilData.calData, out ARM))
                            {
                                fN2A += ARM * 2;
                            }
                            if (float.TryParse(NAHOilData.calData, out NAH))
                            {
                                fN2A += NAH;
                            }

                            N2Acal = fN2A == 0 ? string.Empty : fN2A.ToString();
                        }
                    }

                    if (N2Acal != string.Empty && N2Acal != "非数字")
                    {
                        this._gridOil.SetTips("N2A", i, N2Acal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilCHRLinkCheck()
        { 
            //混合
        }
        private void WideGridOilRVPLinkCheck()
        {
            //查看文档
        }
        private void WideGridOilFRZLinkCheck()
        {
            List<OilDataEntity> FRZOilDataList = this._gridOil.GetDataByRowItemCode("FRZ");

            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity FRZOilData = FRZOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (FRZOilData == null)
                    continue;

                if (FRZOilData.calData == FRZOilData.labData && FRZOilData.calData != string.Empty)
                {
                    string FRZcal = string.Empty;

                    if (FRZcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D20OilData != null && MCPOilData != null)
                            FRZcal = BaseFunction.FunFRZfromD20_MCP(D20OilData.calData, MCPOilData.calData);
                    }
                    if (FRZcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                            FRZcal = BaseFunction.FunFRZfromD20_A10_A30_A50_A70_A90(D20OilData.calData,A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData);
                    }
 
                    if (FRZcal != string.Empty && FRZcal != "非数字")
                    {
                        this._gridOil.SetTips("FRZ", i, FRZcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilSMKLinkCheck()
        {
            List<OilDataEntity> SMKOilDataList = this._gridOil.GetDataByRowItemCode("SMK");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity SMKOilData = SMKOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (SMKOilData == null)
                    continue;

                if (SMKOilData.calData == SMKOilData.labData && SMKOilData.calData != string.Empty)
                {
                    string SMKcal = string.Empty;

                    if (SMKcal == string.Empty)
                    {
                        OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (APIOilData != null && MCPOilData != null)
                            SMKcal = BaseFunction.FunSMKfromAPI_MCP(APIOilData.calData, MCPOilData.calData);
                    }
                    if (SMKcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                            SMKcal = BaseFunction.FunFRZfromD20_A10_A30_A50_A70_A90(D20OilData.calData, A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData);
                    }

                    if (SMKcal != string.Empty && SMKcal != "非数字")
                    {
                        this._gridOil.SetTips("SMK", i, SMKcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilSAVLinkCheck()
        {
            List<OilDataEntity> SAVOilDataList = this._gridOil.GetDataByRowItemCode("SAV");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity SAVOilData = SAVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (SAVOilData == null)
                    continue;

                if (SAVOilData.calData == SAVOilData.labData && SAVOilData.calData != string.Empty)
                {
                    string SAVcal = string.Empty;                   
                    
                    if (SAVcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D20OilData != null && MCPOilData != null)
                        {
                            Dictionary<string, float> SAV_ARVfromD20_MCP = BaseFunction.FunSAV_ARVfromD20_MCP(D20OilData.calData, MCPOilData.calData);
                            if (SAV_ARVfromD20_MCP.Keys.Contains("SAV"))
                                SAVcal = SAV_ARVfromD20_MCP["SAV"].ToString();
                        }
                    }
                    if (SAVcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                        {
                            Dictionary<string, float> SAV_ARVfromD20_A10_A30_A50_A70_A90 = BaseFunction.FunSAV_ARVfromD20_A10_A30_A50_A70_A90(D20OilData.calData, A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData);
                            if (SAV_ARVfromD20_A10_A30_A50_A70_A90.Keys.Contains("SAV"))
                                SAVcal = SAV_ARVfromD20_A10_A30_A50_A70_A90["SAV"].ToString();
                        }              
                    }

                    if (SAVcal != string.Empty && SAVcal != "非数字")
                    {
                        this._gridOil.SetTips("SAV", i, SAVcal);
                    }
                }
                #endregion
            } 
        }
        private void WideGridOilARVLinkCheck()
        {
            List<OilDataEntity> ARVOilDataList = this._gridOil.GetDataByRowItemCode("ARV");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity ARVOilData = ARVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (ARVOilData == null)
                    continue;

                if (ARVOilData.calData == ARVOilData.labData && ARVOilData.calData != string.Empty)
                {
                    string ARVcal = string.Empty;

                    if (ARVcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D20OilData != null && MCPOilData != null)
                        {
                            Dictionary<string, float> SAV_ARVfromD20_MCP = BaseFunction.FunSAV_ARVfromD20_MCP(D20OilData.calData, MCPOilData.calData);
                            if (SAV_ARVfromD20_MCP.Keys.Contains("ARV"))
                                ARVcal = SAV_ARVfromD20_MCP["ARV"].ToString();
                        }
                    }
                    if (ARVcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                        {
                            Dictionary<string, float> SAV_ARVfromD20_A10_A30_A50_A70_A90 = BaseFunction.FunSAV_ARVfromD20_A10_A30_A50_A70_A90(D20OilData.calData, A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData);
                            if (SAV_ARVfromD20_A10_A30_A50_A70_A90.Keys.Contains("ARV"))
                                ARVcal = SAV_ARVfromD20_A10_A30_A50_A70_A90["ARV"].ToString();
                        }
                    }

                    if (ARVcal != string.Empty && ARVcal != "非数字")
                    {
                        this._gridOil.SetTips("ARV", i, ARVcal);
                    }
                }
                #endregion
            } 
        }
        private void WideGridOilLHVLinkCheck()
        {
            List<OilDataEntity> LHVOilDataList = this._gridOil.GetDataByRowItemCode("LHV");

            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");

          
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity LHVOilData = LHVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (LHVOilData == null)
                    continue;

                if (LHVOilData.calData == LHVOilData.labData && LHVOilData.calData != string.Empty)
                {
                    string LHVcal = string.Empty;

                    if (LHVcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ANIOilData = ANIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity SULOilData = SULOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && ANIOilData != null && SULOilData != null)
                            LHVcal = BaseFunction.FunLHVfromD20_ANI_SUL(D20OilData.calData, ANIOilData.calData, SULOilData.calData);
                    }
                    if (LHVcal == string.Empty)
                    {
                        OilDataEntity ANIOilData = ANIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity SULOilData = SULOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (APIOilData != null && SULOilData != null && ANIOilData != null)
                            LHVcal = BaseFunction.FunLHVfromAPI_ANI_SUL(APIOilData.calData, ANIOilData.calData, SULOilData.calData);
                    }

                    if (LHVcal != string.Empty && LHVcal != "非数字")
                    {
                        this._gridOil.SetTips("LHV", i, LHVcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilIRTLinkCheck()
        {
            List<OilDataEntity> IRTOilDataList = this._gridOil.GetDataByRowItemCode("IRT");

            List<OilDataEntity> SAVOilDataList = this._gridOil.GetDataByRowItemCode("SAV");
            List<OilDataEntity> ARVOilDataList = this._gridOil.GetDataByRowItemCode("ARV");
            List<OilDataEntity> OLVOilDataList = this._gridOil.GetDataByRowItemCode("OLV");
 
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity IRTOilData = IRTOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (IRTOilData == null)
                    continue;
                // IRT=SUM(SAV:OLV)  SAV:ARV:OLV
                if (IRTOilData.calData == IRTOilData.labData && IRTOilData.calData != string.Empty)
                {
                    string IRTcal = string.Empty;

                    if (IRTcal == string.Empty)
                    {
                        OilDataEntity SAVOilData = SAVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ARVOilData = ARVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity OLVOilData = OLVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                      
                        if (SAVOilData != null && ARVOilData != null && OLVOilData != null)
                        {
                            List<string> tempList = new List<string>();
                            tempList.Add(SAVOilData.calData);
                            tempList.Add(ARVOilData.calData);
                            tempList.Add(OLVOilData.calData);
                    
                            IRTcal = BaseFunction.FunSumAllowEmpty(tempList);
                        }
                    }
                    if (IRTcal != string.Empty && IRTcal != "非数字")
                    {
                        this._gridOil.SetTips("IRT", i, IRTcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilPORLinkCheck()
        {
            PORLinkSupplement();
        }
        private void WideGridOilSOPLinkCheck()
        {
            SOPLinkSupplement();
        }
        private void WideGridOilCLPLinkCheck()
        {
            //混合
        }
        private void WideGridOilCILinkCheck()
        {
            List<OilDataEntity> CIOilDataList = this._gridOil.GetDataByRowItemCode("CI");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity CIOilData = CIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (CIOilData == null)
                    continue;

                if (CIOilData.calData == CIOilData.labData && CIOilData.calData != string.Empty)
                {
                    string CIcal = string.Empty;

                    if (CIcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D20OilData != null && MCPOilData != null)
                        {
                            CIcal = BaseFunction.FunCIfromMCP_D20(MCPOilData.calData, D20OilData.calData); 
                        }
                    }
                    if (CIcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                        {
                            CIcal = BaseFunction.FunCIfromA10A30A50A70A90_D20(A10OilData.calData, A30OilData.calData, A50OilData.calData, A70OilData.calData, A90OilData.calData, D20OilData.calData);
                        }
                    }

                    if (CIcal != string.Empty && CIcal != "非数字")
                    {
                        this._gridOil.SetTips("CI", i, CIcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilCENLinkCheck()
        {
            List<OilDataEntity> CENOilDataList = this._gridOil.GetDataByRowItemCode("CEN");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity CENOilData = CENOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (CENOilData == null)
                    continue;

                if (CENOilData.calData == CENOilData.labData && CENOilData.calData != string.Empty)
                {
                    string CENcal = string.Empty;                 
                    if (CENcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A10OilData = A10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A30OilData = A30OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A50OilData = A50OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A70OilData = A70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity A90OilData = A90OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && A10OilData != null && A30OilData != null && A50OilData != null && A70OilData != null && A90OilData != null)
                        {
                            CENcal = BaseFunction.FunCENfromA10A30A50A90_D20(A10OilData.calData, A30OilData.calData, A50OilData.calData,  A90OilData.calData, D20OilData.calData);
                        }
                    }

                    if (CENcal != string.Empty && CENcal != "非数字")
                    {
                        this._gridOil.SetTips("CEN", i, CENcal);
                    }
                }
                #endregion
            }
        }
        private void WideGridOilDILinkCheck()
        {
            DILinkSupplement();
        }
        private void WideGridOilSAHLinkCheck()
        {
            List<OilDataEntity> SAHOilDataList = this._gridOil.GetDataByRowItemCode("SAH");
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");
            List<OilDataEntity> M01OilDataList = this._gridOil.GetDataByRowItemCode("M01");
            List<OilDataEntity> MNAOilDataList = this._gridOil.GetDataByRowItemCode("MNA");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity SAHOilData = SAHOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (SAHOilData == null)
                    continue;
                if (SAHOilData.calData == SAHOilData.labData && SAHOilData.calData != string.Empty)
                {
                    string SAHcal = string.Empty;

                    if (SAHcal == string.Empty)
                    {
                        string MSPcal = getStrValuefromOilDataEntity(MSPOilDataList, i);
                        SAHcal = MSPcal;
                    }

                    if (SAHcal == string.Empty)
                    {
                        string M01cal = getStrValuefromOilDataEntity(M01OilDataList, i);
                        string MNAcal = getStrValuefromOilDataEntity(MNAOilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(M01cal);
                        tempList.Add(MNAcal);

                        SAHcal = BaseFunction.FunSumNotAllowEmpty(tempList);
                    }
                    if (SAHcal != string.Empty && SAHcal != "非数字")
                        this._gridOil.SetTips("SAH", i, SAHcal);
                }
            }
        }
        private void WideGridOilARSLinkCheck()
        {
            List<OilDataEntity> ARSOilDataList = this._gridOil.GetDataByRowItemCode("ARS");
            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");
            List<OilDataEntity> MA1OilDataList = this._gridOil.GetDataByRowItemCode("MA1");
            List<OilDataEntity> MA2OilDataList = this._gridOil.GetDataByRowItemCode("MA2");
            List<OilDataEntity> MA3OilDataList = this._gridOil.GetDataByRowItemCode("MA3");
            List<OilDataEntity> MA4OilDataList = this._gridOil.GetDataByRowItemCode("MA4");
            List<OilDataEntity> MA5OilDataList = this._gridOil.GetDataByRowItemCode("MA5");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity ARSOilData = ARSOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (ARSOilData == null)
                    continue;
                if (ARSOilData.calData == ARSOilData.labData && ARSOilData.calData != string.Empty)
                {
                    string ARScal = string.Empty;
                    if (ARScal == string.Empty)
                    {
                        string MATcal = getStrValuefromOilDataEntity(MATOilDataList, i);
                        ARScal = MATcal;
                    }

                    if (ARScal == string.Empty)
                    {
                        string MA1cal = getStrValuefromOilDataEntity(MA1OilDataList, i);
                        string MA2cal = getStrValuefromOilDataEntity(MA2OilDataList, i);
                        string MA3cal = getStrValuefromOilDataEntity(MA3OilDataList, i);
                        string MA4cal = getStrValuefromOilDataEntity(MA4OilDataList, i);
                        string MA5cal = getStrValuefromOilDataEntity(MA5OilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(MA1cal);
                        tempList.Add(MA2cal);
                        tempList.Add(MA3cal);
                        tempList.Add(MA4cal);
                        tempList.Add(MA5cal);

                        ARScal = BaseFunction.FunSumNotAllowEmpty(tempList);
                    }
                    if (ARScal != string.Empty && ARScal != "非数字")
                        this._gridOil.SetTips("ARS", i, ARScal);
                }
            }
        }
        private void WideGridOilRESLinkCheck()
        {
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");
            List<OilDataEntity> MRSOilDataList = this._gridOil.GetDataByRowItemCode("MRS");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity RESOilData = RESOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (RESOilData == null)
                    continue;
                if (RESOilData.calData == RESOilData.labData && RESOilData.calData != string.Empty)
                {
                    string REScal = string.Empty;
                    if (REScal == string.Empty)
                    {
                        string MRScal = getStrValuefromOilDataEntity(MRSOilDataList, i);
                        REScal = MRScal;
                    }

                    if (REScal != string.Empty && REScal != "非数字")
                        this._gridOil.SetTips("RES", i, REScal);
                }
            }
        }
        private void WideGridOilAPHLinkCheck()
        {
            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
            List<OilDataEntity> SAHOilDataList = this._gridOil.GetDataByRowItemCode("SAH");
            List<OilDataEntity> ARSOilDataList = this._gridOil.GetDataByRowItemCode("ARS");
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity APHOilData = APHOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (APHOilData == null)
                    continue;
                if (APHOilData.calData == APHOilData.labData && APHOilData.calData != string.Empty)
                {
                    string APHcal = string.Empty;
                    if (APHcal == string.Empty)
                    {
                        string ARScal = getStrValuefromOilDataEntity(ARSOilDataList, i);
                        string REScal = getStrValuefromOilDataEntity(RESOilDataList, i);
                        string SAHcal = getStrValuefromOilDataEntity(SAHOilDataList, i);

                        if (ARScal == string.Empty || REScal == string.Empty || SAHcal == string.Empty)
                            continue;

                        List<string> tempList = new List<string>();
                        tempList.Add(ARScal);
                        tempList.Add(REScal);
                        tempList.Add(SAHcal);

                        string strTemp = BaseFunction.FunSumAllowEmpty(tempList);
                        float sum = 100, temp = 0;
                        if (float.TryParse(strTemp, out temp))
                        {
                            sum = 100 - temp;
                        }

                        APHcal = sum.ToString();
                    }
                    if (APHcal != string.Empty && APHcal != "非数字")
                        this._gridOil.SetTips("APH", i, APHcal);
                }
            }
        }
        private void WideGridOil4CTLinkCheck()
        {
            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
            List<OilDataEntity> SAHOilDataList = this._gridOil.GetDataByRowItemCode("SAH");
            List<OilDataEntity> ARSOilDataList = this._gridOil.GetDataByRowItemCode("ARS");
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");
            List<OilDataEntity> _4CTOilDataList = this._gridOil.GetDataByRowItemCode("4CT");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity _4CTOilData = _4CTOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (_4CTOilData == null)
                    continue;
                if (_4CTOilData.calData == _4CTOilData.labData && _4CTOilData.calData != string.Empty)
                {
                    string _4CTcal = string.Empty;
                    ///*4CT关联补充  SUM(SAH:APH)    SAH:ARS:RES:APH*/
                    if (_4CTcal == string.Empty)
                    {
                        string SAHcal = getStrValuefromOilDataEntity(SAHOilDataList, i);
                        string ARScal = getStrValuefromOilDataEntity(ARSOilDataList, i);
                        string REScal = getStrValuefromOilDataEntity(RESOilDataList, i);
                        string APHcal = getStrValuefromOilDataEntity(APHOilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(SAHcal);
                        tempList.Add(ARScal);
                        tempList.Add(REScal);
                        tempList.Add(APHcal);

                        _4CTcal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (_4CTcal != string.Empty && _4CTcal != "非数字")
                        this._gridOil.SetTips("4CT", i, _4CTcal);
                }
            }
        }
        private void WideGridOilCPPLinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> R20OilDataList = this._gridOil.GetDataByRowItemCode("R20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            List<OilDataEntity> R70OilDataList = this._gridOil.GetDataByRowItemCode("R70");

            List<OilDataEntity> CPPOilDataList = this._gridOil.GetDataByRowItemCode("CPP");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity CPPOilData = CPPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (CPPOilData == null)
                    continue;
                if (CPPOilData.calData == CPPOilData.labData && CPPOilData.calData != string.Empty)
                {
                    string CPPcal = string.Empty;

                    if (CPPcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string R20cal = getStrValuefromOilDataEntity(R20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(D20cal, R20cal, MWcal, SULcal);
                        if (DIC.Keys.Contains("CPP"))
                            CPPcal = DIC["CPP"].ToString();
                    }

                    if (CPPcal == string.Empty)
                    {
                        string D70cal = getStrValuefromOilDataEntity(D70OilDataList, i);
                        string R70cal = getStrValuefromOilDataEntity(R70OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(D70cal, R70cal, MWcal, SULcal);

                        if (DIC.Keys.Contains("CPP"))
                            CPPcal = DIC["CPP"].ToString();
                    }
                    if (CPPcal != string.Empty && CPPcal != "非数字")
                        this._gridOil.SetTips("CPP", i, CPPcal);                                      
                }
            }
        }
        private void WideGridOilCNNLinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> R20OilDataList = this._gridOil.GetDataByRowItemCode("R20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            List<OilDataEntity> R70OilDataList = this._gridOil.GetDataByRowItemCode("R70");

            List<OilDataEntity> CNNOilDataList = this._gridOil.GetDataByRowItemCode("CNN");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity CNNOilData = CNNOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (CNNOilData == null)
                    continue;
                if (CNNOilData.calData == CNNOilData.labData && CNNOilData.calData != string.Empty)
                {
                    string CNNcal = string.Empty;

                    if (CNNcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string R20cal = getStrValuefromOilDataEntity(R20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(D20cal, R20cal, MWcal, SULcal);
                        if (DIC.Keys.Contains("CNN"))
                            CNNcal = DIC["CNN"].ToString();
                    }

                    if (CNNcal == string.Empty)
                    {
                        string D70cal = getStrValuefromOilDataEntity(D70OilDataList, i);
                        string R70cal = getStrValuefromOilDataEntity(R70OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(D70cal, R70cal, MWcal, SULcal);

                        if (DIC.Keys.Contains("CNN"))
                            CNNcal = DIC["CNN"].ToString();
                    }
                    if (CNNcal != string.Empty && CNNcal != "非数字")
                        this._gridOil.SetTips("CNN", i, CNNcal);
                }
            }
        }
        private void WideGridOilCAALinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> R20OilDataList = this._gridOil.GetDataByRowItemCode("R20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            List<OilDataEntity> R70OilDataList = this._gridOil.GetDataByRowItemCode("R70");

            List<OilDataEntity> CAAOilDataList = this._gridOil.GetDataByRowItemCode("CAA");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity CAAOilData = CAAOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (CAAOilData == null)
                    continue;
                if (CAAOilData.calData == CAAOilData.labData && CAAOilData.calData != string.Empty)
                {
                    string CAAcal = string.Empty;

                    if (CAAcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string R20cal = getStrValuefromOilDataEntity(R20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(D20cal, R20cal, MWcal, SULcal);
                        if (DIC.Keys.Contains("CAA"))
                            CAAcal = DIC["CAA"].ToString();
                    }

                    if (CAAcal == string.Empty)
                    {
                        string D70cal = getStrValuefromOilDataEntity(D70OilDataList, i);
                        string R70cal = getStrValuefromOilDataEntity(R70OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(D70cal, R70cal, MWcal, SULcal);

                        if (DIC.Keys.Contains("CAA"))
                            CAAcal = DIC["CAA"].ToString();
                    }
                    if (CAAcal != string.Empty && CAAcal != "非数字")
                        this._gridOil.SetTips("CAA", i, CAAcal);
                }
            }
        }
        private void WideGridOilRTTLinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> R20OilDataList = this._gridOil.GetDataByRowItemCode("R20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            List<OilDataEntity> R70OilDataList = this._gridOil.GetDataByRowItemCode("R70");

            List<OilDataEntity> RTTOilDataList = this._gridOil.GetDataByRowItemCode("RTT");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity RTTOilData = RTTOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (RTTOilData == null)
                    continue;
                if (RTTOilData.calData == RTTOilData.labData && RTTOilData.calData != string.Empty)
                {
                    string RTTcal = string.Empty;

                    if (RTTcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string R20cal = getStrValuefromOilDataEntity(R20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(D20cal, R20cal, MWcal, SULcal);
                        if (DIC.Keys.Contains("RTT"))
                            RTTcal = DIC["RTT"].ToString();
                    }

                    if (RTTcal == string.Empty)
                    {
                        string D70cal = getStrValuefromOilDataEntity(D70OilDataList, i);
                        string R70cal = getStrValuefromOilDataEntity(R70OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(D70cal, R70cal, MWcal, SULcal);

                        if (DIC.Keys.Contains("RTT"))
                            RTTcal = DIC["RTT"].ToString();
                    }
                    if (RTTcal != string.Empty && RTTcal != "非数字")
                        this._gridOil.SetTips("RTT", i, RTTcal);
                }
            }
        }
        private void WideGridOilRNNLinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> R20OilDataList = this._gridOil.GetDataByRowItemCode("R20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            List<OilDataEntity> R70OilDataList = this._gridOil.GetDataByRowItemCode("R70");

            List<OilDataEntity> RNNOilDataList = this._gridOil.GetDataByRowItemCode("RNN");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity RNNOilData = RNNOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (RNNOilData == null)
                    continue;
                if (RNNOilData.calData == RNNOilData.labData && RNNOilData.calData != string.Empty)
                {
                    string RNNcal = string.Empty;

                    if (RNNcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string R20cal = getStrValuefromOilDataEntity(R20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(D20cal, R20cal, MWcal, SULcal);
                        if (DIC.Keys.Contains("RNN"))
                            RNNcal = DIC["RNN"].ToString();
                    }

                    if (RNNcal == string.Empty)
                    {
                        string D70cal = getStrValuefromOilDataEntity(D70OilDataList, i);
                        string R70cal = getStrValuefromOilDataEntity(R70OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(D70cal, R70cal, MWcal, SULcal);

                        if (DIC.Keys.Contains("RNN"))
                            RNNcal = DIC["RNN"].ToString();
                    }
                    if (RNNcal != string.Empty && RNNcal != "非数字")
                        this._gridOil.SetTips("RNN", i, RNNcal);
                }
            }
        }
        private void WideGridOilRAALinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> R20OilDataList = this._gridOil.GetDataByRowItemCode("R20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            List<OilDataEntity> R70OilDataList = this._gridOil.GetDataByRowItemCode("R70");

            List<OilDataEntity> RAAOilDataList = this._gridOil.GetDataByRowItemCode("RAA");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity RAAOilData = RAAOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (RAAOilData == null)
                    continue;
                if (RAAOilData.calData == RAAOilData.labData && RAAOilData.calData != string.Empty)
                {
                    string RAAcal = string.Empty;

                    if (RAAcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string R20cal = getStrValuefromOilDataEntity(R20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(D20cal, R20cal, MWcal, SULcal);
                        if (DIC.Keys.Contains("RAA"))
                            RAAcal = DIC["RAA"].ToString();
                    }

                    if (RAAcal == string.Empty)
                    {
                        string D70cal = getStrValuefromOilDataEntity(D70OilDataList, i);
                        string R70cal = getStrValuefromOilDataEntity(R70OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(D70cal, R70cal, MWcal, SULcal);

                        if (DIC.Keys.Contains("RAA"))
                            RAAcal = DIC["RAA"].ToString();
                    }
                    if (RAAcal != string.Empty && RAAcal != "非数字")
                        this._gridOil.SetTips("RAA", i, RAAcal);
                }
            }
        }       
        private void WideGridOilPATLinkCheck()
        {
            List<OilDataEntity> PATOilDataList = this._gridOil.GetDataByRowItemCode("PAT");
            List<OilDataEntity> M01OilDataList = this._gridOil.GetDataByRowItemCode("M01");

            List<OilDataEntity> PANOilDataList = this._gridOil.GetDataByRowItemCode("PAN");
            List<OilDataEntity> PAOOilDataList = this._gridOil.GetDataByRowItemCode("PAO");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity PATOilData = PATOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (PATOilData == null)
                    continue;
                if (PATOilData.calData == PATOilData.labData && PATOilData.calData != string.Empty)
                {
                    string PATcal = string.Empty;

                    if (PATcal == string.Empty)
                    {
                        string PANcal = getStrValuefromOilDataEntity(PANOilDataList, i);
                        string PAOcal = getStrValuefromOilDataEntity(PAOOilDataList, i);

                        PATcal = BaseFunction.FunPATfromPAN_PAO(PANcal, PAOcal);
                    }
                    if (PATcal == string.Empty)
                    {
                        string M01cal = getStrValuefromOilDataEntity(M01OilDataList, i);
                        PATcal = M01cal;
                    }

                    if (PATcal != string.Empty && PATcal != "非数字")
                        this._gridOil.SetTips("PAT", i, PATcal);
                }
            }
        }
        private void WideGridOilMNALinkCheck()
        {
            List<OilDataEntity> MNAOilDataList = this._gridOil.GetDataByRowItemCode("MNA");

            List<OilDataEntity> M02OilDataList = this._gridOil.GetDataByRowItemCode("M02");
            List<OilDataEntity> M03OilDataList = this._gridOil.GetDataByRowItemCode("M03");
            List<OilDataEntity> M04OilDataList = this._gridOil.GetDataByRowItemCode("M04");
            List<OilDataEntity> M05OilDataList = this._gridOil.GetDataByRowItemCode("M05");
            List<OilDataEntity> M06OilDataList = this._gridOil.GetDataByRowItemCode("M06");
            List<OilDataEntity> M07OilDataList = this._gridOil.GetDataByRowItemCode("M07");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MNAOilData = MNAOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MNAOilData == null)
                    continue;
                if (MNAOilData.calData == MNAOilData.labData && MNAOilData.calData != string.Empty)
                {
                    string MNAcal =  string.Empty;
                    if (MNAcal == string.Empty)
                    {
                        string M02cal = getStrValuefromOilDataEntity(M02OilDataList, i);
                        string M03cal = getStrValuefromOilDataEntity(M03OilDataList, i);
                        string M04cal = getStrValuefromOilDataEntity(M04OilDataList, i);
                        string M05cal = getStrValuefromOilDataEntity(M05OilDataList, i);
                        string M06cal = getStrValuefromOilDataEntity(M06OilDataList, i);
                        string M07cal = getStrValuefromOilDataEntity(M07OilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(M02cal);
                        tempList.Add(M03cal);
                        tempList.Add(M04cal);
                        tempList.Add(M05cal);
                        tempList.Add(M06cal);
                        tempList.Add(M07cal);

                        MNAcal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MNAcal != string.Empty && MNAcal != "非数字")
                        this._gridOil.SetTips("MNA", i, MNAcal);
                }
            }
        }
        private void WideGridOilMSPLinkCheck()
        {
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");

            List<OilDataEntity> M01OilDataList = this._gridOil.GetDataByRowItemCode("M01");
            List<OilDataEntity> MNAOilDataList = this._gridOil.GetDataByRowItemCode("MNA");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MSPOilData = MSPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MSPOilData == null)
                    continue;
                if (MSPOilData.calData == MSPOilData.labData && MSPOilData.calData != string.Empty)
                {
                    string MSPcal = string.Empty;
                    if (MSPcal == string.Empty)
                    {
                        string MNAcal = getStrValuefromOilDataEntity(MNAOilDataList, i);
                        string M01cal = getStrValuefromOilDataEntity(M01OilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(MNAcal);
                        tempList.Add(M01cal);

                        MSPcal = BaseFunction.FunSumAllowEmpty(tempList);
                    }

                    if (MSPcal != string.Empty && MSPcal != "非数字")
                        this._gridOil.SetTips("MSP", i, MSPcal);
                }
            }
        }
        private void WideGridOilMA1LinkCheck()
        {
            List<OilDataEntity> MA1OilDataList = this._gridOil.GetDataByRowItemCode("MA1");

            List<OilDataEntity> M08OilDataList = this._gridOil.GetDataByRowItemCode("M08");
            List<OilDataEntity> M09OilDataList = this._gridOil.GetDataByRowItemCode("M09");
            List<OilDataEntity> M10OilDataList = this._gridOil.GetDataByRowItemCode("M10");
            List<OilDataEntity> M11OilDataList = this._gridOil.GetDataByRowItemCode("M11");
            List<OilDataEntity> M12OilDataList = this._gridOil.GetDataByRowItemCode("M12");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MA1OilData = MA1OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MA1OilData == null)
                    continue;
                if (MA1OilData.calData == MA1OilData.labData && MA1OilData.calData != string.Empty)
                {
                    string MA1cal = string.Empty;
                    if (MA1cal == string.Empty)
                    {
                        string M08cal = getStrValuefromOilDataEntity(M08OilDataList, i);
                        string M09cal = getStrValuefromOilDataEntity(M09OilDataList, i);
                        string M10cal = getStrValuefromOilDataEntity(M10OilDataList, i);
                        string M11cal = getStrValuefromOilDataEntity(M11OilDataList, i);
                        string M12cal = getStrValuefromOilDataEntity(M12OilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(M08cal);
                        tempList.Add(M09cal);
                        tempList.Add(M10cal);
                        tempList.Add(M11cal);
                        tempList.Add(M12cal);

                        MA1cal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MA1cal != string.Empty && MA1cal != "非数字")
                        this._gridOil.SetTips("MA1", i, MA1cal);
                }
            }
        }
        private void WideGridOilMA2LinkCheck()
        {
            List<OilDataEntity> MA2OilDataList = this._gridOil.GetDataByRowItemCode("MA2");

            List<OilDataEntity> M13OilDataList = this._gridOil.GetDataByRowItemCode("M13");
            List<OilDataEntity> M14OilDataList = this._gridOil.GetDataByRowItemCode("M14");
            List<OilDataEntity> M15OilDataList = this._gridOil.GetDataByRowItemCode("M15");
            List<OilDataEntity> M16OilDataList = this._gridOil.GetDataByRowItemCode("M16");
            List<OilDataEntity> M17OilDataList = this._gridOil.GetDataByRowItemCode("M17");
            List<OilDataEntity> M18OilDataList = this._gridOil.GetDataByRowItemCode("M18");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MA2OilData = MA2OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MA2OilData == null)
                    continue;
                if (MA2OilData.calData == MA2OilData.labData && MA2OilData.calData != string.Empty)
                {
                    string MA2cal = string.Empty;
                    if (MA2cal == string.Empty)
                    {
                        string M13cal = getStrValuefromOilDataEntity(M13OilDataList, i);
                        string M14cal = getStrValuefromOilDataEntity(M14OilDataList, i);
                        string M15cal = getStrValuefromOilDataEntity(M15OilDataList, i);
                        string M16cal = getStrValuefromOilDataEntity(M16OilDataList, i);
                        string M17cal = getStrValuefromOilDataEntity(M17OilDataList, i);
                        string M18cal = getStrValuefromOilDataEntity(M18OilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(M13cal);
                        tempList.Add(M14cal);
                        tempList.Add(M15cal);
                        tempList.Add(M16cal);
                        tempList.Add(M17cal);
                        tempList.Add(M18cal);

                        MA2cal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MA2cal != string.Empty && MA2cal != "非数字")
                        this._gridOil.SetTips("MA2", i, MA2cal);
                }
            }
        }
        private void WideGridOilMA3LinkCheck()
        {
            List<OilDataEntity> MA3OilDataList = this._gridOil.GetDataByRowItemCode("MA3");

            List<OilDataEntity> M19OilDataList = this._gridOil.GetDataByRowItemCode("M19");
            List<OilDataEntity> M20OilDataList = this._gridOil.GetDataByRowItemCode("M20");

            for (int i = 0; i < this._maxCol; i++)
            {   
                OilDataEntity MA3OilData = MA3OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MA3OilData == null)
                    continue;
                if (MA3OilData.calData == MA3OilData.labData && MA3OilData.calData != string.Empty)
                {
                    string MA3cal = string.Empty;
                    if (MA3cal == string.Empty)
                    {
                        string M19cal = getStrValuefromOilDataEntity(M19OilDataList, i);
                        string M20cal = getStrValuefromOilDataEntity(M20OilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(M19cal);
                        tempList.Add(M20cal);
                        MA3cal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MA3cal != string.Empty && MA3cal != "非数字")
                        this._gridOil.SetTips("MA3", i, MA3cal);
                }
            }
        }
        private void WideGridOilMA4LinkCheck()
        {
            List<OilDataEntity> MA4OilDataList = this._gridOil.GetDataByRowItemCode("MA4");

            List<OilDataEntity> M21OilDataList = this._gridOil.GetDataByRowItemCode("M21");
            List<OilDataEntity> M22OilDataList = this._gridOil.GetDataByRowItemCode("M22");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MA4OilData = MA4OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MA4OilData == null)
                    continue;
                if (MA4OilData.calData == MA4OilData.labData && MA4OilData.calData != string.Empty)
                {
                    string MA4cal = string.Empty;
                    /*MA4关联补充  SUM(M21:M22)*/
                    if (MA4cal == string.Empty)
                    {
                        string M21cal = getStrValuefromOilDataEntity(M21OilDataList, i);
                        string M22cal = getStrValuefromOilDataEntity(M22OilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(M21cal);
                        tempList.Add(M22cal);

                        MA4cal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MA4cal != string.Empty && MA4cal != "非数字")
                        this._gridOil.SetTips("MA4", i, MA4cal);
                }
            }
        }
        private void WideGridOilMA5LinkCheck()
        {
            List<OilDataEntity> MA5OilDataList = this._gridOil.GetDataByRowItemCode("MA5");

            List<OilDataEntity> M23OilDataList = this._gridOil.GetDataByRowItemCode("M23");
            List<OilDataEntity> M24OilDataList = this._gridOil.GetDataByRowItemCode("M24");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MA5OilData = MA5OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MA5OilData == null)
                    continue;
                if (MA5OilData.calData == MA5OilData.labData && MA5OilData.calData != string.Empty)
                {
                    string MA5cal = string.Empty;

                    /*MA5关联补充  SUM(M23:M24)*/
                    if (MA5cal == string.Empty)
                    {
                        string M23cal = getStrValuefromOilDataEntity(M23OilDataList, i);
                        string M24cal = getStrValuefromOilDataEntity(M24OilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(M23cal);
                        tempList.Add(M24cal);

                        MA5cal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MA5cal != string.Empty && MA5cal != "非数字")
                        this._gridOil.SetTips("MA5", i, MA5cal);
                }
            }
        }
        private void WideGridOilMANLinkCheck()
        {
            List<OilDataEntity> MANOilDataList = this._gridOil.GetDataByRowItemCode("MAN");

            List<OilDataEntity> M26OilDataList = this._gridOil.GetDataByRowItemCode("M26");
            List<OilDataEntity> M27OilDataList = this._gridOil.GetDataByRowItemCode("M27");
            List<OilDataEntity> M28OilDataList = this._gridOil.GetDataByRowItemCode("M28");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MANOilData = MANOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MANOilData == null)
                    continue;
                if (MANOilData.calData == MANOilData.labData && MANOilData.calData != string.Empty)
                {
                    string MANcal = string.Empty;
                    /*MA3关联补充  SUM(M19:M20)*/
                    if (MANcal == string.Empty)
                    {
                        string M26cal = getStrValuefromOilDataEntity(M26OilDataList, i);
                        string M27cal = getStrValuefromOilDataEntity(M27OilDataList, i);
                        string M28cal = getStrValuefromOilDataEntity(M28OilDataList, i);
                        List<string> tempList = new List<string>();
                        tempList.Add(M26cal);
                        tempList.Add(M27cal);
                        tempList.Add(M28cal);
                        MANcal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MANcal != string.Empty && MANcal != "非数字")
                        this._gridOil.SetTips("MAN", i, MANcal);
                }
            }
        }
        private void WideGridOilMATLinkCheck()
        {
            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");

            List<OilDataEntity> MA1OilDataList = this._gridOil.GetDataByRowItemCode("MA1");
            List<OilDataEntity> MA2OilDataList = this._gridOil.GetDataByRowItemCode("MA2");
            List<OilDataEntity> MA3OilDataList = this._gridOil.GetDataByRowItemCode("MA3");
            List<OilDataEntity> MA4OilDataList = this._gridOil.GetDataByRowItemCode("MA4");
            List<OilDataEntity> MA5OilDataList = this._gridOil.GetDataByRowItemCode("MA5");
            List<OilDataEntity> MANOilDataList = this._gridOil.GetDataByRowItemCode("MAN");
            List<OilDataEntity> MAUOilDataList = this._gridOil.GetDataByRowItemCode("MAU");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MATOilData = MATOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MATOilData == null)
                    continue;
                if (MATOilData.calData == MATOilData.labData && MATOilData.calData != string.Empty)
                {
                    string MATcal = string.Empty;
                    /*MAT关联补充  SUM(MA1+MA2+MA3+MA4+MA5 ) */
                    if (MATcal == string.Empty)
                    {
                        string MA1cal = getStrValuefromOilDataEntity(MA1OilDataList, i);
                        string MA2cal = getStrValuefromOilDataEntity(MA2OilDataList, i);
                        string MA3cal = getStrValuefromOilDataEntity(MA3OilDataList, i);
                        string MA4cal = getStrValuefromOilDataEntity(MA4OilDataList, i);
                        string MA5cal = getStrValuefromOilDataEntity(MA5OilDataList, i);
                        string MANcal = getStrValuefromOilDataEntity(MANOilDataList, i);
                        string MAUcal = getStrValuefromOilDataEntity(MAUOilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(MA1cal);
                        tempList.Add(MA2cal);
                        tempList.Add(MA3cal);
                        tempList.Add(MA4cal);
                        tempList.Add(MA5cal);
                        tempList.Add(MANcal);
                        tempList.Add(MAUcal);
                        MATcal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MATcal != string.Empty && MATcal != "非数字")
                        this._gridOil.SetTips("MAT", i, MATcal);
                }
            }
        }
        private void WideGridOilMTALinkCheck()
        {
            List<OilDataEntity> MTAOilDataList = this._gridOil.GetDataByRowItemCode("MTA");

            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");
            List<OilDataEntity> MRSOilDataList = this._gridOil.GetDataByRowItemCode("MRS");
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MTAOilData = MTAOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MTAOilData == null)
                    continue;
                if (MTAOilData.calData == MTAOilData.labData && MTAOilData.calData != string.Empty)
                {
                    string MTAcal = string.Empty;
                    /*MTA关联补充  SUM(MAT:MRS:MSP)*/
                    if (MTAcal == string.Empty)
                    {
                        string MATcal = getStrValuefromOilDataEntity(MATOilDataList, i);
                        string MRScal = getStrValuefromOilDataEntity(MRSOilDataList, i);
                        string MSPcal = getStrValuefromOilDataEntity(MSPOilDataList, i);

                        List<string> tempList = new List<string>();
                        tempList.Add(MATcal);
                        tempList.Add(MRScal);
                        tempList.Add(MSPcal);

                        MTAcal = BaseFunction.FunSumAllowEmpty(tempList);
                    }
                    if (MTAcal != string.Empty && MTAcal != "非数字")
                        this._gridOil.SetTips("MTA", i, MTAcal);
                }
            }
        }
        #endregion
        /// <summary>
        /// 渣油表关联审查
        /// </summary>
        public void ResidueGridOilLinkCheck(bool showToolTip)
        {
            if (showToolTip)
            {
                ResidueGridOilWYLinkCheck();
                ResidueGridOilTWYLinkCheck();/////
                ResidueGridOilVYLinkCheck();
                ResidueGridOilTVYLinkCheck();////
                ResidueGridOilAPILinkCheck();
                ResidueGridOilD20LinkCheck();
                ResidueGridOilD15LinkCheck();
                ResidueGridOilD60LinkCheck();
                WideGridOilSGLinkCheck();
                ResidueGridOilV0LinkCheck();
                ResidueGridOilVILinkCheck();            
                ResidueGridOilVG4LinkCheck();
                ResidueGridOilV1GLinkCheck();
                ResidueGridOilPORLinkCheck();
                ResidueGridOilSOPLinkCheck();
                //ResidueGridOilMixCheckAccumulate("CCR");
                //ResidueGridOilMixCheckAccumulate("ASH");
                ResidueGridOilMWLinkCheck();
                //ResidueGridOilMixCheckAccumulate("CAR");
                //ResidueGridOilMixCheckAccumulate("H2");
                ResidueGridOilC_HLinkCheck();
                //ResidueGridOilMixCheckAccumulate("SUL");
                //ResidueGridOilMixCheckAccumulate("N2");
                //ResidueGridOilMixCheckAccumulate("FE");
                //ResidueGridOilMixCheckAccumulate("NI");
                //ResidueGridOilMixCheckAccumulate("V");
                //ResidueGridOilMixCheckAccumulate("CA");
                //ResidueGridOilMixCheckAccumulate("NA");
                //ResidueGridOilMixCheckAccumulate("CU");
                //ResidueGridOilMixCheckAccumulate("PB");
                //ResidueGridOilMixCheckAccumulate("MG");
                ResidueGridOilNIVLinkCheck();
                //ResidueGridOilMixCheckAccumulate("SAH");
                //ResidueGridOilMixCheckAccumulate("ARS");
                //ResidueGridOilMixCheckAccumulate("RES");
                //ResidueGridOilMixCheckAccumulate("APH");
                ResidueGridOilFFALinkCheck();
                ResidueGridOilCIILinkCheck();
                ResidueGridOilTCCLinkCheck();
                ResidueGridOilCALinkCheck();
                ResidueGridOilRNNLinkCheck();
                ResidueGridOilRAALinkCheck();
                ResidueGridOilRTTLinkCheck();
                ResidueGridOilKFCLinkCheck();
            }
            else
            {
                for (int j = 0; j < this._maxCol; j++)
                {
                    this._gridOil.CancelTips("WY", j);
                    this._gridOil.CancelTips("TWY", j);
                    this._gridOil.CancelTips("VY", j);
                    this._gridOil.CancelTips("TVY", j);

                    this._gridOil.CancelTips("API", j);
                    this._gridOil.CancelTips("D20", j);
                    this._gridOil.CancelTips("D60", j);
                    this._gridOil.CancelTips("D15", j);
                    this._gridOil.CancelTips("SG", j);
                    this._gridOil.CancelTips("V02", j);
                    this._gridOil.CancelTips("V04", j);
                    this._gridOil.CancelTips("V05", j);
                    this._gridOil.CancelTips("V08", j);
                    this._gridOil.CancelTips("V10", j);
                    this._gridOil.CancelTips("VI", j);
                    this._gridOil.CancelTips("VG4", j);
                    this._gridOil.CancelTips("V1G", j);
                    this._gridOil.CancelTips("POR", j);
                    this._gridOil.CancelTips("SOP", j);
 
                    this._gridOil.CancelTips("MW", j);
                    this._gridOil.CancelTips("C/H", j);
                    this._gridOil.CancelTips("NIV", j);
                    this._gridOil.CancelTips("KFC", j);

                    this._gridOil.CancelTips("FFA", j);
                    this._gridOil.CancelTips("CII", j);
                    this._gridOil.CancelTips("TCC", j);
                    this._gridOil.CancelTips("CAL", j);
                    this._gridOil.CancelTips("RNN", j);
                    this._gridOil.CancelTips("RAA", j);
                    this._gridOil.CancelTips("RTT", j);           
                }
            }  
        }

        #region "渣油表"
        private void ResidueGridOilWYLinkCheck()
        {
            WYLinkSupplement();
        }
        private void ResidueGridOilTWYLinkCheck()
        {
            //TWYLinkSupplement();
        }
        private void ResidueGridOilVYLinkCheck()
        {
            VYLinkSupplement();
        }
        private void ResidueGridOilTVYLinkCheck()
        {
            //TVYLinkSupplement();
        }
        private void ResidueGridOilAPILinkCheck()
        {
            APILinkSupplement();
        }
        private void ResidueGridOilD20LinkCheck()
        {
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            if (D60OilDataList == null && SGOilDataList == null && APIOilDataList == null && D60OilDataList.Count <= 0 && APIOilDataList.Count <= 0 && SGOilDataList == null)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D20OilData == null)
                    continue;
                if (D20OilData.calData == D20OilData.labData && D20OilData.calData != string.Empty)
                {
                    string D20cal = string.Empty;
                    if (D20cal == string.Empty)
                    {
                        OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        string D60cal = D60OilData == null ? string.Empty : D60OilData.calData;
                        D20cal = BaseFunction.FunD20fromD60(D60cal);
                    }
                    if (D20cal == string.Empty)
                    {
                        OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        string APIcal = APIOilData == null ? string.Empty : APIOilData.calData;
                        D20cal = BaseFunction.FunD20fromAPI(APIcal);
                    }

                    if (D20cal == string.Empty)
                    {
                        OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        string SGcal = SGOilData == null ? string.Empty : SGOilData.calData;
                        D20cal = BaseFunction.FunD20fromSG(SGcal);
                    }

                    if (D20cal != string.Empty && D20cal != "非数字")
                    {
                        this._gridOil.SetTips("D20", i, D20cal);
                    }
                }
            }
        }
        private void ResidueGridOilD60LinkCheck()
        {
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D60OilData == null)
                    continue;
                if (D60OilData.calData == D60OilData.labData && D60OilData.calData != string.Empty)
                {
                    string D60cal = string.Empty;
                    if (D60cal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;
                        D60cal = BaseFunction.FunD60fromD20(D20cal);
                    }
                    if (D60cal == string.Empty)
                    {
                        OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        string SGcal = SGOilData == null ? string.Empty : SGOilData.calData;
                        D60cal = BaseFunction.FunD60fromSG(SGcal);
                    }
                    if (D60cal == string.Empty)
                    {
                        OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        string APIcal = APIOilData == null ? string.Empty : APIOilData.calData;
                        D60cal = BaseFunction.FunD60fromAPI(APIcal);
                    }

                    if (D60cal != string.Empty && D60cal != "非数字")
                    {
                        this._gridOil.SetTips("D20", i, D60cal);
                    }
                }
            }
        }
        private void ResidueGridOilD15LinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");
            if (D20OilDataList == null && D20OilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D15OilData = D15OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D15OilData == null)
                    continue;
                if (D15OilData.calData == D15OilData.labData && D15OilData.calData != string.Empty)
                {
                    string D15cal = string.Empty;

                     if (D15cal == string.Empty)
                     {
                         OilDataEntity D20OilData = this._gridOil.GetDataByRowItemCodeColumnIndex("D20", i);
                         string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;
                         D15cal = BaseFunction.FunD15fromD20(D20cal);
                     }
                     if (D15cal != string.Empty && D15cal != "非数字")
                     {
                         this._gridOil.SetTips("D15", i, D15cal);
                     }
                 }
            }
        }
        private void ResidueGridOilV0LinkCheck()
        { 
            //查看
        }
        private void ResidueGridOilVILinkCheck()
        {
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> VIOilDataList = this._gridOil.GetDataByRowItemCode("VI");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity VIOilData = VIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (VIOilData == null)
                    continue;
                if (VIOilData.calData == VIOilData.labData && VIOilData.calData != string.Empty)
                {
                    string VIcal = string.Empty; 

                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);

                    if (VIcal == string.Empty)
                    {
                        string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                        VIcal = BaseFunction.FunVIfromV04_V10(V04cal, V10cal);
                    }
                    if (VIcal == string.Empty)
                    {
                        string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                        VIcal = BaseFunction.FunVIfromV08_V10(V08cal, V10cal);
                    }
                    if (VIcal != string.Empty && VIcal != "非数字")
                        this._gridOil.SetTips("VI", i, VIcal);
                }
            }
        }
        private void ResidueGridOilNIVLinkCheck()
        {
            List<OilDataEntity> NIVOilDataList = this._gridOil.GetDataByRowItemCode("NIV");

            List<OilDataEntity> NIOilDataList = this._gridOil.GetDataByRowItemCode("NI");
            List<OilDataEntity> VOilDataList = this._gridOil.GetDataByRowItemCode("V");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity NIVOilData = NIVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (NIVOilData == null)
                    continue;
                if (NIVOilData.calData == NIVOilData.labData && NIVOilData.calData != string.Empty)
                {
                    string NIVcal = string.Empty;

                    if (NIVcal == string.Empty)
                    {
                        string NIcal = getStrValuefromOilDataEntity(NIOilDataList, i);
                        string Vcal = getStrValuefromOilDataEntity(VOilDataList, i);

                        NIVcal = BaseFunction.FunNIVfromNI_V(NIcal, Vcal);
                    }

                    if (NIVcal != string.Empty)
                        this._gridOil.SetTips("NIV", i, NIVcal);
                }
            }
        }
        private void ResidueGridOilVG4LinkCheck()
        {
            VG4LinkSupplement();
        }
        private void ResidueGridOilV1GLinkCheck()
        {
            V1GLinkSupplement();
        }
        private void ResidueGridOilPORLinkCheck()
        {
            PORLinkSupplement();
        }
        private void ResidueGridOilSOPLinkCheck()
        {
            SOPLinkSupplement();
        }
        private void ResidueGridOilMWLinkCheck()
        {
            List<OilDataEntity> MWVOilDataList = this._gridOil.GetDataByRowItemCode("MW");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity MWOilData = MWVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MWOilData == null)
                    continue;
                if (MWOilData.calData == MWOilData.labData && MWOilData.calData != string.Empty)
                {
                    string MWcal = string.Empty;

                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);

                    if (MWcal == string.Empty)
                    {
                        string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                        MWcal = BaseFunction.FunMWfromD20_V08_V10(D20cal, V08cal, V10cal);
                    }

                    if (MWcal == string.Empty)
                    {
                        string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                        MWcal = BaseFunction.FunMWfromD20_V04_V10(D20cal, V04cal, V10cal);
                    }
                    
                    if (MWcal != string.Empty)
                        this._gridOil.SetTips("MW", i, MWcal);
                }                
            }
        }
        private void ResidueGridOilC_HLinkCheck()
        {
            List<OilDataEntity> C_HOilDataList = this._gridOil.GetDataByRowItemCode("C/H");

            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity C_HOilData = C_HOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (C_HOilData == null)
                    continue;
                if (C_HOilData.calData == C_HOilData.labData && C_HOilData.calData != string.Empty)
                {
                    string C_Hcal = string.Empty;
                    if (C_Hcal == string.Empty)
                    {
                        string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);
                        string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                        C_Hcal = BaseFunction.FunC_H(CARcal, H2cal);
                    }
                    if (C_Hcal == string.Empty)
                    {
                        string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);
                        C_Hcal = BaseFunction.FunC1HfromSG(SGcal);
                    }
                    if (C_Hcal != string.Empty && C_Hcal != "非数字")
                        this._gridOil.SetTips("C/H", i, C_Hcal);
                }
            }
        }
        private void ResidueGridOilFFALinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");

            List<OilDataEntity> FFAOilDataList = this._gridOil.GetDataByRowItemCode("FFA");
         
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity FFAOilData = FFAOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (FFAOilData == null)
                    continue;
                if (FFAOilData.calData == FFAOilData.labData && FFAOilData.calData != string.Empty)
                {
                    string FFAcal = string.Empty;

                    if (FFAcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                        string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(D20cal, MWcal, CARcal, H2cal);

                        if (DIC.Keys.Contains("FFA"))
                            FFAcal = DIC["FFA"].ToString();

                        if (FFAcal != string.Empty && FFAcal != "非数字")
                            this._gridOil.SetTips("FFA", i, FFAcal);
                    }
                }
            }
        }
        private void ResidueGridOilCIILinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> CIIOilDataList = this._gridOil.GetDataByRowItemCode("CII");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity CIIOilData = CIIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (CIIOilData == null)
                    continue;
                if (CIIOilData.calData == CIIOilData.labData && CIIOilData.calData != string.Empty)
                {
                    string CIIcal = string.Empty;

                    if (CIIcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                        string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(D20cal, MWcal, CARcal, H2cal);

                        if (DIC.Keys.Contains("CII"))
                            CIIcal = DIC["CII"].ToString();

                        if (CIIcal != string.Empty && CIIcal != "非数字")
                            this._gridOil.SetTips("CII", i, CIIcal);
                    }
                }
            }
        }
        private void ResidueGridOilTCCLinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> TCCOilDataList = this._gridOil.GetDataByRowItemCode("TCC");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity TCCOilData = TCCOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (TCCOilData == null)
                    continue;
                if (TCCOilData.calData == TCCOilData.labData && TCCOilData.calData != string.Empty)
                {
                    string TCCcal = string.Empty;

                    if (TCCcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                        string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(D20cal, MWcal, CARcal, H2cal);

                        if (DIC.Keys.Contains("TCC"))
                            TCCcal = DIC["TCC"].ToString();

                        if (TCCcal != string.Empty && TCCcal != "非数字")
                            this._gridOil.SetTips("TCC", i, TCCcal);
                    }
                }
            }
        }
        private void ResidueGridOilCALinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> CA_OilDataList = this._gridOil.GetDataByRowItemCode("CA#");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity CA_OilData = CA_OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (CA_OilData == null)
                    continue;
                if (CA_OilData.calData == CA_OilData.labData && CA_OilData.calData != string.Empty)
                {
                    string CA_cal = string.Empty;

                    if (CA_cal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                        string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(D20cal, MWcal, CARcal, H2cal);

                        if (DIC.Keys.Contains("CA#"))
                            CA_cal = DIC["CA#"].ToString();

                        if (CA_cal != string.Empty && CA_cal != "非数字")
                            this._gridOil.SetTips("CA#", i, CA_cal);
                    }
                }
            }
        }
        private void ResidueGridOilRNNLinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> RNNOilDataList = this._gridOil.GetDataByRowItemCode("RNN");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity RNNOilData = RNNOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (RNNOilData == null)
                    continue;
                if (RNNOilData.calData == RNNOilData.labData && RNNOilData.calData != string.Empty)
                {
                    string RNNcal = string.Empty;

                    if (RNNcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                        string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(D20cal, MWcal, CARcal, H2cal);

                        if (DIC.Keys.Contains("RNN"))
                            RNNcal = DIC["RNN"].ToString();

                        if (RNNcal != string.Empty && RNNcal != "非数字")
                            this._gridOil.SetTips("RNN", i, RNNcal);
                    }
                }
            }
        }
        private void ResidueGridOilRAALinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> RAAOilDataList = this._gridOil.GetDataByRowItemCode("RAA");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity RAAOilData = RAAOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (RAAOilData == null)
                    continue;
                if (RAAOilData.calData == RAAOilData.labData && RAAOilData.calData != string.Empty)
                {
                    string RAAcal = string.Empty;

                    if (RAAcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                        string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(D20cal, MWcal, CARcal, H2cal);

                        if (DIC.Keys.Contains("RAA"))
                            RAAcal = DIC["RAA"].ToString();

                        if (RAAcal != string.Empty && RAAcal != "非数字")
                            this._gridOil.SetTips("RAA", i, RAAcal);
                    }
                }
            }
        }
        private void ResidueGridOilRTTLinkCheck()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> RTTOilDataList = this._gridOil.GetDataByRowItemCode("RTT");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity RTTOilData = RTTOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (RTTOilData == null)
                    continue;
                if (RTTOilData.calData == RTTOilData.labData && RTTOilData.calData != string.Empty)
                {
                    string RTTcal = string.Empty;

                    if (RTTcal == string.Empty)
                    {
                        string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                        string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                        string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                        string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);

                        Dictionary<string, float> DIC = BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(D20cal, MWcal, CARcal, H2cal);

                        if (DIC.Keys.Contains("RTT"))
                            RTTcal = DIC["RTT"].ToString();

                        if (RTTcal != string.Empty && RTTcal != "非数字")
                            this._gridOil.SetTips("RTT", i, RTTcal);
                    }
                }
            }
        }
        private void ResidueGridOilKFCLinkCheck()
        {
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> KFCOilDataList = this._gridOil.GetDataByRowItemCode("KFC");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity KFCOilData = KFCOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (KFCOilData == null)
                    continue;
                if (KFCOilData.calData == KFCOilData.labData && KFCOilData.calData != string.Empty)
                {
                    string KFCcal = string.Empty;
                    if (KFCcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;

                        OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        string V10cal = V10OilData == null ? string.Empty : V10OilData.calData;

                        KFCcal = BaseFunction.FunKFCfromV10_D20(V10cal, D20cal);
                    }
                    if (KFCcal != string.Empty && KFCcal != "非数字")
                        this._gridOil.SetTips("KFC", i, KFCcal);
                }
            }
        }
        #endregion 
        #endregion

        #region "补充函数"

        /// <summary>
        /// 根据行的数据集合和列从中获取单元格的校正值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string getStrValuefromOilDataEntity(List<OilDataEntity> row, int column)
        {
            string strResult = string.Empty;

            if (row == null || row.Count <= 0 || column < 0)
                return strResult;

            OilDataEntity OilData = row.Where(o => o.ColumnIndex == column).FirstOrDefault();
            strResult = OilData == null ? string.Empty : OilData.calData;

            return strResult;
        }
        /// <summary>
        /// 根据行的数据集合和列从中获取单元格的校正值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string getStrValuefromOilDataEntity(OilDataEntity oilData, int column)
        {
            string strResult = string.Empty;

            if (oilData == null || column < 0)
                return strResult;

            strResult = oilData == null ? string.Empty : oilData.calData;

            return strResult;
        }

        /// <summary>
        /// WY(i)=VY(i)*D20(i)/D20(原油)
        /// </summary>
        private void WYLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            var ds = this._wholeGridOil.GetAllData();
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity WYOilData = WYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (WYOilData == null)
                    continue;
                if (WYOilData.calData == WYOilData.labData && WYOilData.calData != string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);

                   
                    OilDataEntity oilDataD20Whole = ds.Where(o => o.OilTableRow.itemCode == "D20").FirstOrDefault();//选出原油性质的D20校正值 

                    if (oilDataD20Whole == null)
                        continue;

                    string WYcal = BaseFunction.FunWY(VYcal, D20cal, oilDataD20Whole.calData);
                    if (WYcal != string.Empty && WYcal != "非数字")
                    {
                        this._gridOil.SetTips("WY", i, WYcal);
                    }
                }
            }
        }

        /// <summary>
        /// VY(i)=WY(i)/D20(i)*D20(原油)
        /// </summary>
        private void VYLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            var ds = this._wholeGridOil.GetAllData();
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity VYOilData = VYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (VYOilData == null)
                    continue;

                if (VYOilData.calData == VYOilData.labData && VYOilData.calData != string.Empty)
                {
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                  
                    OilDataEntity oilDataD20Whole = ds.Where(o => o.OilTableRow.itemCode == "D20").FirstOrDefault();//选出原油性质的D20校正值 

                    if (oilDataD20Whole == null)
                    continue;

                    string VYcal = BaseFunction.FunVY(WYcal, D20cal, oilDataD20Whole.calData);
                    if (VYcal != string.Empty && VYcal != "非数字")
                    {
                        this._gridOil.SetTips("VY", i, VYcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void TWYLinkSupplement()
        {
            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity TWYOilData = TWYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (TWYOilData == null)
                    continue;
                if (TWYOilData.calData == TWYOilData.labData && TWYOilData.calData != string.Empty)
                {
                    string TWYcal = string.Empty;

                    if (i == 0)
                    {
                        string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                        TWYcal = WYcal;
                    }
                    else
                    {
                        OilDataEntity beforTWYOilData = TWYOilDataList.Where(o => o.ColumnIndex == i - 1).FirstOrDefault();
                        OilDataEntity WYOilData = WYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (beforTWYOilData != null && WYOilData != null)
                            TWYcal = BaseFunction.FunTWY(beforTWYOilData.calData, WYOilData.calData); ;
                    }

                    if (TWYcal != string.Empty && TWYcal != "非数字")
                    {
                        this._gridOil.SetTips("TWY", i, TWYcal);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void TVYLinkSupplement()
        {
            List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity TVYOilData = TVYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (TVYOilData == null)
                    continue;
                if (TVYOilData.calData == TVYOilData.labData && TVYOilData.calData != string.Empty)
                {
                    string TVYcal = string.Empty;

                    if (i == 0)
                    {
                        string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                        TVYcal = VYcal;
                    }
                    else
                    {
                        OilDataEntity beforTVYOilData = TVYOilDataList.Where(o => o.ColumnIndex == i - 1).FirstOrDefault();
                        OilDataEntity VYOilData = VYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (beforTVYOilData != null && VYOilData != null)
                            TVYcal = BaseFunction.FunTWY(beforTVYOilData.calData, VYOilData.calData); ;
                    }

                    if (TVYcal != string.Empty && TVYcal != "非数字")
                    {
                        this._gridOil.SetTips("TVY", i, TVYcal);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void D20LinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D20OilData == null)
                    continue;
                if (D20OilData.calData == D20OilData.labData && D20OilData.calData != string.Empty)
                {
                    string D20cal = string.Empty;
                    if (i == 0)
                    {
                        #region "i == 0"
                        string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                         
                        float ECP = 0;
                        if (float.TryParse(ECPcal, out ECP))
                        {
                            if (ECP <= 15)
                            {
                                List<OilDataEntity> Datas = this._lightGridOil._datas.Where(o => o.OilTableTypeID == (int)EnumTableType.Light).ToList();
                                List<OilDataEntity> lightDatas = Datas.Where(o => o.OilTableCol.colCode == "Cut1").ToList();

                                if (lightDatas != null)
                                {
                                    bool Havelight = (lightDatas.Count > 0) ? true : false;//表示轻端不为空
                                    if (Havelight)
                                    {
                                        LightCurveParmTableAccess lightCurveParmTableAccess = new LightCurveParmTableAccess();
                                        List<LightCurveParmTableEntity> LightCurveParmList = lightCurveParmTableAccess.Get("1=1");
                                        float WT_SUM = 0, D20_SUM = 0;
                                        for (int k = 0; k < lightDatas.Count; k++)
                                        {
                                            var LightD20 = LightCurveParmList.Where(o => o.ItemCode == lightDatas[k].OilTableRow.itemCode).FirstOrDefault();
                                            float D20 = 0, lightWT = 0;
                                            if (float.TryParse(LightD20.D20, out D20) && float.TryParse(lightDatas[k].calData, out lightWT))
                                            {
                                                WT_SUM += lightWT;
                                                D20_SUM += lightWT / D20;
                                            }
                                        }

                                        D20cal = (WT_SUM / D20_SUM).ToString();
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region "i != 0"
                        if (D20cal == string.Empty)
                        {
                            OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                            if (D60OilData != null)
                                D20cal = BaseFunction.FunD20fromD60(D60OilData.calData);
                        }
                        if (D20cal == string.Empty)
                        {
                            OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                            if (APIOilData != null)
                                D20cal = BaseFunction.FunD20fromAPI(APIOilData.calData);
                        }

                        if (D20cal == string.Empty)
                        {
                            OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                            if (SGOilData != null)
                                D20cal = BaseFunction.FunD20fromSG(SGOilData.calData);
                        }
                        #endregion
                    }

                    if (D20cal != string.Empty && D20cal != "非数字")
                    {
                        this._gridOil.SetTips("D20", i, D20cal);
                    }
                }
            }       
        }
        /// <summary>
        /// 
        /// </summary>
        private void D15LinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D15OilData = D15OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D15OilData == null)
                    continue;
                if (D15OilData.calData == D15OilData.labData && D15OilData.calData != string.Empty)
                {
                    string D15cal = string.Empty;

                    if (D15cal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null)
                            D15cal = BaseFunction.FunD15fromD20(D20OilData.calData);
                    }

                    if (D15cal != string.Empty && D15cal != "非数字")
                    {
                        this._gridOil.SetTips("D15", i, D15cal);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void D60LinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D60OilData == null)
                    continue;
                if (D60OilData.calData == D60OilData.labData && D60OilData.calData != string.Empty)
                {
                    string D60cal = string.Empty;

                    if (D60cal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null)
                            D60cal = BaseFunction.FunD60fromD20(D20OilData.calData);
                    }
                    if (D60cal == string.Empty)
                    {
                        OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (SGOilData != null)
                            D60cal = BaseFunction.FunD60fromSG(SGOilData.calData);
                    }
                    if (D60cal == string.Empty)
                    {
                        OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (APIOilData != null)
                            D60cal = BaseFunction.FunD60fromAPI(APIOilData.calData);
                    }
                    if (D60cal != string.Empty && D60cal != "非数字")
                    {
                        this._gridOil.SetTips("D60", i, D60cal);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void D70LinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region 
                OilDataEntity D70OilData = D70OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (D70OilData == null)
                    continue;
                if (D70OilData.calData == D70OilData.labData && D70OilData.calData != string.Empty)
                {
                    string D70cal = string.Empty;

                    if (D70cal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null)
                            D70cal = BaseFunction.FunD70fromD20(D20OilData.calData);
                    }
                    
                    if (D70cal != string.Empty && D70cal != "非数字")
                    {
                        this._gridOil.SetTips("D70", i, D70cal);
                    }
                }
                #endregion 
            }
        }
        /// <summary>
        /// D20->SG;D60->S
        /// </summary>
        private void SGLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (SGOilData == null)
                    continue;

                if (SGOilData.calData == SGOilData.labData && SGOilData.calData != string.Empty)
                {
                    string SGcal = string.Empty;
                    if (SGcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null)
                            SGcal = BaseFunction.FunSGfromD20(D20OilData.calData);
                    }

                    if (SGcal == string.Empty)
                    {
                        OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D60OilData != null)
                            SGcal = BaseFunction.FunSG(D60OilData.calData);
                    }
                    if (SGcal != string.Empty && SGcal != "非数字")
                    {
                        this._gridOil.SetTips("SG", i, SGcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void APILinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (APIOilData == null)
                    continue;

                if (APIOilData.calData == APIOilData.labData && APIOilData.calData != string.Empty)
                {
                    string APIcal = string.Empty;
                    if (APIcal == string.Empty)//存在D20实测值
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null)
                            APIcal = BaseFunction.FunAPIfromD20(D20OilData.calData);
                    }

                    if (APIcal == string.Empty)//存在D60实测值
                    {
                        OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D60OilData != null)
                            APIcal = BaseFunction.FunAPIfromD60(D60OilData.calData);
                    }

                    if (APIcal != string.Empty && APIcal != "非数字")
                    {
                        this._gridOil.SetTips("API", i, APIcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void WYDLinkSupplement()
        {
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");

            List<OilDataEntity> WYDOilDataList = this._gridOil.GetDataByRowItemCode("WYD");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity WYDOilData = WYDOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (WYDOilData == null)
                    continue;

                if (WYDOilData.calData == WYDOilData.labData && WYDOilData.calData != string.Empty)
                {
                    string WYDcal = string.Empty;
                    if (WYDcal == string.Empty)
                    {
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity WYOilData = WYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (ICPOilData != null && ECPOilData != null && WYOilData != null)
                            WYDcal = BaseFunction.FunWYDfromICP_ECP_WY(ICPOilData.calData, ECPOilData.calData, WYOilData.calData);
                    }
                    if (WYDcal != string.Empty && WYDcal != "非数字")
                    {
                        this._gridOil.SetTips("WYD", i, WYDcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void MWYLinkSupplement()
        {
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
            List<OilDataEntity> MWYOilDataList = this._gridOil.GetDataByRowItemCode("MWY");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity MWYOilData = MWYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MWYOilData == null)
                    continue;

                if (MWYOilData.calData == MWYOilData.labData && MWYOilData.calData != string.Empty)
                {
                    string MWYcal = string.Empty;
                    string TWYcal = string.Empty;
                    string WYcal = string.Empty;
                    if (i == 0)
                    {
                        OilDataEntity WYOilData = WYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (WYOilData != null)
                        {
                            WYcal = WYOilData.calData;
                            TWYcal = "0";
                        }
                    }
                    else
                    {
                        OilDataEntity WYOilData = WYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity TWYOilData = TWYOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (WYOilData != null && TWYOilData != null)
                        {
                            WYcal = WYOilData.calData;
                            TWYcal = TWYOilData.calData;
                        }
                    }

                    MWYcal = BaseFunction.FunMWYfromTWY_WY(TWYcal, WYcal);
                    if (MWYcal != string.Empty && MWYcal != "非数字")
                    {
                        this._gridOil.SetTips("MWY", i, MWYcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void MCPLinkSupplement()
        {
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
 
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MCPOilData == null)
                    continue;

                if (MCPOilData.calData == MCPOilData.labData && MCPOilData.calData != string.Empty)
                {
                    string MCPcal = string.Empty;
                    if (MCPcal == string.Empty)
                    {
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                     
                        if (ICPOilData != null && ECPOilData != null )
                            MCPcal = BaseFunction.FunMCPfromICP_ECP(ICPOilData.calData, ECPOilData.calData);
                    }
                    if (MCPcal != string.Empty && MCPcal != "非数字")
                    {
                        this._gridOil.SetTips("MCP", i, MCPcal);
                    }
                }
                #endregion
            }
        }
        private void V0LinkSupplement()
        {
            WholeGridOilV02LinkCheck();
            WholeGridOilV04LinkCheck();
            WholeGridOilV05LinkCheck();
            WholeGridOilV08LinkCheck();
            WholeGridOilV10LinkCheck();
        }
        /// <summary>
        ///  
        /// </summary>
        private void NETLinkSupplement()
        {
            List<OilDataEntity> ACDOilDataList = this._gridOil.GetDataByRowItemCode("ACD");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> NETOilDataList = this._gridOil.GetDataByRowItemCode("NET");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity NETOilData = NETOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (NETOilData == null)
                    continue;

                if (NETOilData.calData == NETOilData.labData && NETOilData.calData != string.Empty)
                {
                    string NETcal = string.Empty;
                    if (NETcal == string.Empty)
                    {
                        OilDataEntity ACDOilData = ACDOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (ACDOilData != null && D20OilData != null)
                            NETcal = BaseFunction.FunNET(ACDOilData.calData, D20OilData.calData);
                    }
                    if (NETcal != string.Empty && NETcal != "非数字")
                    {
                        this._gridOil.SetTips("NET", i, NETcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void SULLinkSupplement()
        {
            //内插
        }
        /// <summary>
        ///  
        /// </summary>
        private void N2LinkSupplement()
        {
            //内插
        }

        /// <summary>
        ///  
        /// </summary>
        private void RVPLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> RVPOilDataList = this._gridOil.GetDataByRowItemCode("RVP");
          
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity RVPOilData = RVPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (RVPOilData == null)
                    continue;

                if (RVPOilData.calData == RVPOilData.labData && RVPOilData.calData != string.Empty)
                {
                    string RVPcal = string.Empty;
                    if (RVPcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (MCPOilData != null)
                            RVPcal = BaseFunction.FunRVPfromMCP(MCPOilData.calData);
                    }
                    if (RVPcal != string.Empty && RVPcal != "非数字")
                    {
                        this._gridOil.SetTips("RVP", i, RVPcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void BANLinkSupplement()
        {
             //内插
        }
        /// <summary>
        ///  SMK
        /// </summary>
        private void SMKLinkSupplement()
        {
            List<OilDataEntity> SMKOilDataList = this._gridOil.GetDataByRowItemCode("SMK");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity SMKOilData = SMKOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (SMKOilData == null)
                    continue;

                if (SMKOilData.calData == SMKOilData.labData && SMKOilData.calData != string.Empty)
                {
                    string SMKcal = string.Empty;
                    
                    if (SMKcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        SMKcal = BaseFunction.FunSMKfromAPI_MCP(APIOilData.calData, MCPOilData.calData);
                    }
                    if (SMKcal == string.Empty)
                    {
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        SMKcal = BaseFunction.FunSMKfromAPI_ICP_ECP(APIOilData.calData, ICPOilData.calData, ECPOilData.calData);
                    }
                    if (SMKcal != string.Empty && SMKcal != "非数字")
                    {
                        this._gridOil.SetTips("SMK", i, SMKcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void FRZLinkSupplement()
        {
            List<OilDataEntity> FRZOilDataList = this._gridOil.GetDataByRowItemCode("FRZ");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity FRZOilData = FRZOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (FRZOilData == null)
                    continue;

                if (FRZOilData.calData == FRZOilData.labData && FRZOilData.calData != string.Empty)
                {
                    string FRZcal = string.Empty;

                    if (FRZcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        FRZcal = BaseFunction.FunFRZfromD20_MCP(D20OilData.calData, MCPOilData.calData);
                    }
                    if (FRZcal == string.Empty)
                    {
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        FRZcal = BaseFunction.FunFRZfromD20_ICP_ECP(D20OilData.calData, ICPOilData.calData, ECPOilData.calData);
                    }
                    if (FRZcal != string.Empty && FRZcal != "非数字")
                    {
                        this._gridOil.SetTips("FRZ", i, FRZcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void PORLinkSupplement()
        {
            List<OilDataEntity> POROilDataList = this._gridOil.GetDataByRowItemCode("POR");
            List<OilDataEntity> SOPOilDataList = this._gridOil.GetDataByRowItemCode("SOP");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity POROilData = POROilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (POROilData == null)
                    continue;

                if (POROilData.calData == POROilData.labData && POROilData.calData != string.Empty)
                {
                    string PORcal = string.Empty;

                    if (PORcal == string.Empty)
                    {
                        OilDataEntity SOPOilData = SOPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (SOPOilData != null)
                            PORcal = BaseFunction.FunPOR(SOPOilData.calData);
                    }
                    
                    if (PORcal != string.Empty && PORcal != "非数字")
                    {
                        this._gridOil.SetTips("POR", i, PORcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void SOPLinkSupplement()
        {
            List<OilDataEntity> POROilDataList = this._gridOil.GetDataByRowItemCode("POR");
            List<OilDataEntity> SOPOilDataList = this._gridOil.GetDataByRowItemCode("SOP");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity SOPOilData = SOPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (SOPOilData == null)
                    continue;

                if (SOPOilData.calData == SOPOilData.labData && SOPOilData.calData != string.Empty)
                {
                    string SOPcal = string.Empty;

                    if (SOPcal == string.Empty)
                    {
                        OilDataEntity POROilData = POROilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (SOPOilData != null)
                            SOPcal = BaseFunction.FunSOP(POROilData.calData);
                    }

                    if (SOPcal != string.Empty && SOPcal != "非数字")
                    {
                        this._gridOil.SetTips("SOP", i, SOPcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        ///  
        /// </summary>
        private void CFPLinkSupplement()
        {
            //内插
        }
        /// <summary>
        /// SAV  
        /// </summary>
        private void SAVLinkSupplement()
        {
            List<OilDataEntity> SAVOilDataList = this._gridOil.GetDataByRowItemCode("SAV");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity SAVOilData = SAVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (SAVOilData == null)
                    continue;

                if (SAVOilData.calData == SAVOilData.labData && SAVOilData.calData != string.Empty)
                {
                    string SAVcal = string.Empty;

                    if (SAVcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        Dictionary<string, float> SAV_ARVfromD20_MCP = BaseFunction.FunSAV_ARVfromD20_MCP(D20OilData.calData, MCPOilData.calData);
                        if (SAV_ARVfromD20_MCP.Keys.Contains("SAV"))
                            SAVcal = SAV_ARVfromD20_MCP["SAV"].ToString();
                    }
                    if (SAVcal == string.Empty)
                    {
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        Dictionary<string, float> SAV_ARVfromD20_ICP_ECP = BaseFunction.FunSAV_ARVfromD20_ICP_ECP(D20OilData.calData, ICPOilData.calData, ECPOilData.calData);
                        if (SAV_ARVfromD20_ICP_ECP.Keys.Contains("SAV"))
                            SAVcal = SAV_ARVfromD20_ICP_ECP["SAV"].ToString();
                    }
                    if (SAVcal != string.Empty && SAVcal != "非数字")
                    {
                        this._gridOil.SetTips("SAV", i, SAVcal);
                    }
                }
                #endregion
            }      
        }
        /// <summary>
        /// ARV 
        /// </summary>
        private void ARVLinkSupplement()
        {
            List<OilDataEntity> ARVOilDataList = this._gridOil.GetDataByRowItemCode("ARV");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity ARVOilData = ARVOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (ARVOilData == null)
                    continue;

                if (ARVOilData.calData == ARVOilData.labData && ARVOilData.calData != string.Empty)
                {
                    string ARVcal = string.Empty;

                    if (ARVcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        Dictionary<string, float> SAV_ARVfromD20_MCP = BaseFunction.FunSAV_ARVfromD20_MCP(D20OilData.calData, MCPOilData.calData);
                        if (SAV_ARVfromD20_MCP.Keys.Contains("ARV"))
                            ARVcal = SAV_ARVfromD20_MCP["ARV"].ToString();
                    }
                    if (ARVcal == string.Empty)
                    {
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        Dictionary<string, float> SAV_ARVfromD20_ICP_ECP = BaseFunction.FunSAV_ARVfromD20_ICP_ECP(D20OilData.calData, ICPOilData.calData, ECPOilData.calData);
                        if (SAV_ARVfromD20_ICP_ECP.Keys.Contains("ARV"))
                            ARVcal = SAV_ARVfromD20_ICP_ECP["ARV"].ToString();
                    }
                    if (ARVcal != string.Empty && ARVcal != "非数字")
                    {
                        this._gridOil.SetTips("ARV", i, ARVcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// ANI 
        /// </summary>
        private void ANILinkSupplement()
        {
            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            //List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            //List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity ANIOilData = ANIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (ANIOilData == null)
                    continue;

                if (ANIOilData.calData == ANIOilData.labData && ANIOilData.calData != string.Empty)
                {
                    string ANIcal = string.Empty;

                    if (ANIcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (MCPOilData != null && D20OilData != null)
                            ANIcal = BaseFunction.FunANIfromD20_MCP(D20OilData.calData , MCPOilData.calData);
                    }
 
                    if (ANIcal != string.Empty && ANIcal != "非数字")
                    {
                        this._gridOil.SetTips("ANI", i, ANIcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// R20 
        /// </summary>
        private void R20LinkSupplement()
        { 
            //内插
        }
        /// <summary>
        /// R70 
        /// </summary>
        private void R70LinkSupplement()
        {
            //内插
        }
        /// <summary>
        ///  
        /// </summary>
        private void KFCLinkSupplement()
        {
            List<OilDataEntity> KFCOilDataList = this._gridOil.GetDataByRowItemCode("KFC");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity KFCOilData = KFCOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (KFCOilData == null)
                    continue;

                if (KFCOilData.calData == KFCOilData.labData && KFCOilData.calData != string.Empty)
                {
                    string KFCcal = string.Empty;

                    if (KFCcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (MCPOilData != null && D20OilData != null)
                            KFCcal = BaseFunction.FunKFCfromMCP_D20(MCPOilData.calData, D20OilData.calData);
                    }
                    if (KFCcal == string.Empty)
                    {
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (ICPOilData != null && ECPOilData != null && D20OilData != null)
                            KFCcal = BaseFunction.FunKFCfromICPECP_D20(ICPOilData.calData, ECPOilData.calData, D20OilData.calData);
                    }
                    if (KFCcal != string.Empty && KFCcal != "非数字")
                    {
                        this._gridOil.SetTips("KFC", i, KFCcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void BMILinkSupplement()
        {
            List<OilDataEntity> BMIOilDataList = this._gridOil.GetDataByRowItemCode("BMI");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity BMIOilData = BMIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (BMIOilData == null)
                    continue;

                if (BMIOilData.calData == BMIOilData.labData && BMIOilData.calData != string.Empty)
                {
                    string BMIcal = string.Empty;

                    if (BMIcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (MCPOilData != null && D20OilData != null)
                            BMIcal = BaseFunction.FunBMIfromMCP_D20(MCPOilData.calData, D20OilData.calData);
                    }
                    if (BMIcal == string.Empty)
                    {
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (ICPOilData != null && ECPOilData != null && D20OilData != null)
                            BMIcal = BaseFunction.FunBMIfromICPECP_D20(ICPOilData.calData, ECPOilData.calData, D20OilData.calData);
                    }
                    if (BMIcal != string.Empty && BMIcal != "非数字")
                    {
                        this._gridOil.SetTips("BMI", i, BMIcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void DILinkSupplement()
        {
            List<OilDataEntity> DIOilDataList = this._gridOil.GetDataByRowItemCode("DI");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity DIOilData = DIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (DIOilData == null)
                    continue;

                if (DIOilData.calData == DIOilData.labData && DIOilData.calData != string.Empty)
                {
                    string DIcal = string.Empty;

                    if (DIcal == string.Empty)
                    {
                        OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ANIOilData = ANIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (APIOilData != null && ANIOilData != null)
                            DIcal = BaseFunction.FunDI(APIOilData.calData, ANIOilData.calData);
                    }
 
                    if (DIcal != string.Empty && DIcal != "非数字")
                    {
                        this._gridOil.SetTips("DI", i, DIcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void CILinkSupplement() 
        {
            List<OilDataEntity> CIOilDataList = this._gridOil.GetDataByRowItemCode("CI");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity CIOilData = CIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (CIOilData == null)
                    continue;

                if (CIOilData.calData == CIOilData.labData && CIOilData.calData != string.Empty)
                {
                    string CIcal = string.Empty;

                    if (CIcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (MCPOilData != null && D20OilData != null)
                            CIcal = BaseFunction.FunCIfromMCP_D20(MCPOilData.calData, D20OilData.calData);
                    }
                    if (CIcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        if (D20OilData != null && ICPOilData != null && ECPOilData != null)
                            CIcal = BaseFunction.FunCIfromICPECP_D20(ICPOilData.calData, ECPOilData.calData, D20OilData.calData);
                    }
                    if (CIcal != string.Empty && CIcal != "非数字")
                    {
                        this._gridOil.SetTips("CI", i, CIcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void VG4LinkSupplement()
        {
            List<OilDataEntity> VG4OilDataList = this._gridOil.GetDataByRowItemCode("VG4");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity VG4OilData = VG4OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (VG4OilData == null)
                    continue;

                if (VG4OilData.calData == VG4OilData.labData && VG4OilData.calData != string.Empty)
                {
                    string VG4cal = string.Empty;

                    if (VG4cal == string.Empty)
                    {
                        OilDataEntity D15OilData = D15OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V04OilData = V04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D15OilData != null && V04OilData != null)
                            VG4cal = BaseFunction.FunVG4fromD15andV04(D15OilData.calData, V04OilData.calData);
                    }
                    
                    if (VG4cal != string.Empty && VG4cal != "非数字")
                    {
                        this._gridOil.SetTips("VG4", i, VG4cal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void V1GLinkSupplement()
        {
            List<OilDataEntity> V1GOilDataList = this._gridOil.GetDataByRowItemCode("V1G");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity V1GOilData = V1GOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (V1GOilData == null)
                    continue;

                if (V1GOilData.calData == V1GOilData.labData && V1GOilData.calData != string.Empty)
                {
                    string V1Gcal = string.Empty;

                    if (V1Gcal == string.Empty)
                    {
                        OilDataEntity D15OilData = D15OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (D15OilData != null && V10OilData != null)
                            V1Gcal = BaseFunction.FunV1GfromD15andV10(D15OilData.calData, V10OilData.calData);
                    }

                    if (V1Gcal != string.Empty && V1Gcal != "非数字")
                    {
                        this._gridOil.SetTips("V1G", i, V1Gcal);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void MWLinkSupplement()
        {
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
 
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                OilDataEntity MWOilData = MWOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                if (MWOilData == null)
                    continue;

                if (MWOilData.calData == MWOilData.labData && MWOilData.calData != string.Empty)
                {
                    string MWcal = string.Empty;

                    if (MWcal == string.Empty)
                    {
                        OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (MCPOilData != null && SGOilData != null)
                            MWcal = BaseFunction.FunMWfromMCP_SG(MCPOilData.calData, SGOilData.calData);
                    }

                    if (MWcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (ICPOilData != null && ECPOilData != null && D20OilData != null)
                            MWcal = BaseFunction.FunMWfromICP_ECP_D20(ICPOilData.calData, ECPOilData.calData, D20OilData.calData);
                    }

                    if (MWcal == string.Empty)
                    {
                        OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V04OilData = V04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                        OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                        if (V04OilData != null && V10OilData != null && D20OilData != null)
                            MWcal = BaseFunction.FunMWfromD20_V04_V10(D20OilData.calData, V04OilData.calData, V10OilData.calData);
                    }
                    if (MWcal != string.Empty && MWcal != "非数字")
                    {
                        this._gridOil.SetTips("MW", i, MWcal);
                    }
                }
                #endregion
            }
        }
        #endregion 
    }
}
