using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.OilDB.Data;
using RIPP.Lib;
using System.Windows.Forms;
using System.Drawing;

namespace RIPP.OilDB.Data.DataSupplement
{
    public static class GlobalResiduDataSupplementDialog
    {
        public static DialogResult YesNo = DialogResult.Yes;
        /// <summary>
        /// 残炭
        /// </summary>
        public static string  _CCR = "0";
        /// <summary>
        /// 残炭下拉菜单
        /// </summary>
        public static int _CCRDrop = -1;
        /// <summary>
        /// 金属
        /// </summary>
        public static string _MET = "0";
        /// <summary>
        /// 金属下拉菜单
        /// </summary>
        public static int _METDrop = -1;
        /// <summary>
        ///胶质
        /// </summary>
        public static string _APH = "0";
        /// <summary>
        /// 胶质下拉菜单
        /// </summary>
        public static int _APHDrop = -1;
    }


    public class OilDataSupplement
    {
        #region "私有变量"
        private IList<OilTableColEntity> _cols = null;
        private IList<OilTableRowEntity> _rows = null;
        private IList<OilDataEntity> _datas = null;
        private GridOilViewA _gridOil = null;
        private GridOilViewA _wholeGridOil = null;
        private GridOilViewA _lightGridOil = null;
        private GridOilViewA _gcGridOil = null;
        private GridOilViewA _narrowGridOil = null;
        private GridOilViewA _wideGridOil = null;
        private GridOilViewA _residueGridOil = null;
        private DataGridView _grdiOilInfo = null;//原油信息表关联补充使用
        private OilInfoEntity _oilInfo = null;//获取一条原油数据
        /// <summary>
        /// 含有数据的最大列
        /// </summary>
        private int _maxCol = 0;
         
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造空函数
        /// </summary>
        public OilDataSupplement()
        {
            
        }
        /// <summary>
        /// 原油信息表关联补充
        /// </summary>
        /// <param name="grdiOilInfo"></param>
        /// <param name="oilInfo"></param>
        /// <param name="wholeGridOil">原油性质</param>
        /// <param name="narrowGridOil"></param>
        public OilDataSupplement(DataGridView grdiOilInfo, OilInfoEntity oilInfo, GridOilViewA wholeGridOil, GridOilViewA narrowGridOil)
        {
            this._grdiOilInfo = grdiOilInfo;
            this._oilInfo = oilInfo;
            this._narrowGridOil = narrowGridOil;
            this._wholeGridOil = wholeGridOil;
        }       
        /// <summary>
        /// 原油性质表补充
        /// </summary>
        /// <param name="gridOil"></param>
        /// <param name="unImportanceGridOil"></param>
        /// <param name="tableType"></param>
        public OilDataSupplement(GridOilViewA wholeGridOil, GridOilViewA lightGridOil, GridOilViewA GCGridOil, GridOilViewA narrowGridOil, GridOilViewA wideGridOil, GridOilViewA residueGridOil, EnumTableType tableType)
        {
            if (tableType == EnumTableType.Whole)
            {
                this._gridOil = wholeGridOil;
                this._wholeGridOil = wholeGridOil;
                this._lightGridOil = lightGridOil;
                this._gcGridOil = GCGridOil;
                this._narrowGridOil = narrowGridOil;
                this._wideGridOil = wideGridOil;
                this._residueGridOil = residueGridOil;
            }
            else if (tableType == EnumTableType.Narrow)
            {
                this._gridOil = narrowGridOil;
                this._wholeGridOil = wholeGridOil;
                this._lightGridOil = lightGridOil;
                this._gcGridOil = GCGridOil;
                this._narrowGridOil = narrowGridOil;
                this._wideGridOil = wideGridOil;
                this._residueGridOil = residueGridOil;
            }
            else if (tableType == EnumTableType.Wide)
            {
                this._gridOil = wideGridOil;
                this._wholeGridOil = wholeGridOil;
                this._lightGridOil = lightGridOil;
                this._gcGridOil = GCGridOil;
                this._narrowGridOil = narrowGridOil;
                this._wideGridOil = wideGridOil;
                this._residueGridOil = residueGridOil;
            }
            else if (tableType == EnumTableType.Residue)
            {
                this._gridOil = residueGridOil;
                this._wholeGridOil = wholeGridOil;
                this._gcGridOil = GCGridOil;
                this._lightGridOil = lightGridOil;
                this._narrowGridOil = narrowGridOil;
                this._wideGridOil = wideGridOil;
                this._residueGridOil = residueGridOil;
            }
            _MaxCol();//找到填有数据的最大列
        }
        
        /// <summary>
        /// 找到填有数据的最大列找到填有数据的最大列
        /// </summary>
        private void _MaxCol()
        {
            this._maxCol = this._gridOil.GetMaxValidColumnIndex() +1;          
        }
        #endregion

        #region "数据补充"

        #region 原油信息表
        /// <summary>
        /// 原油信息表关联补充
        /// </summary>
        public void OilInfoLinkSupplement()
        {
            OilInfoGridOilCLALinkSupplement();//原油信息表的CLA的补充    
            OilInfoGridOilTYPLinkSupplement();//原油信息表的TYP的补充
            OilInfoGridOilSCLLinkSupplement();
            OilInfoGridOilACLLinkSupplement();
        }
        #endregion

        #region 原油性质表
        /// <summary>
        /// 原油性质表关联补充
        /// </summary>
        public void WholeLinkSupplement()
        {
            try 
            {
                this._wholeGridOil.IsBusy = true;
                WholeGridOilAPILinkSupplement();
                WholeGridOilD20LinkSupplement();
                WholeGridOilD15LinkSupplement();
                WholeGridOilD60LinkSupplement();
                WholeGridOilSGLinkSupplement();
                WholeGridOilV0LinkSupplement();
                NIVLinkSupplement();
                WholeGridOilKFCLinkSupplement();
                this._wholeGridOil.IsBusy = false;
             }
            catch (Exception ex)
            {
                this._wholeGridOil.IsBusy = false;
                Log.Error("原油性质表的数据补充" + ex);
            }
        }
        #endregion

        #region 窄馏分表
        /// <summary>
        /// 窄馏分表关联补充
        /// </summary>
        public void NarrowGridOilLinkSupplement()
        {
            try
            {
                //LabtoCal();
                this._narrowGridOil.IsBusy = true;
                NarrowGridOilICPLinkSupplement();
                NarrowGridOilECPLinkSupplement();
                NarrowGridOilWYLinkSupplement();
                NarrowGridOilTWYLinkSupplement();
                NarrowGridOilVYLinkSupplement();
                NarrowGridOilTVYLinkSupplement();
                NarrowGridOilD20LinkSupplement();
                NarrowGridOilD15LinkSupplement();
                NarrowGridOilD60LinkSupplement();
                NarrowGridOilSGLinkSupplement();
                NarrowGridOilAPILinkSupplement();
                NarrowGridOilWYDLinkSupplement();
                NarrowGridOilMWYLinkSupplement();
                NarrowGridOilMCPLinkSupplement();
                NarrowGridOilV0LinkSupplement();
                NarrowGridOilACDLinkSupplement();
                NarrowGridOilNETLinkSupplement();
                NarrowGridOilSULLinkSupplement();
                NarrowGridOilN2LinkSupplement();
                NarrowGridOilCHRLinkSupplement();
                NarrowGridOilMECLinkSupplement();
                NarrowGridOilBANLinkSupplement();
                NarrowGridOilRVPLinkSupplement();
                NarrowGridOilSMKLinkSupplement();
                NarrowGridOilFRZLinkSupplement();
                NarrowGridOilPORLinkSupplement();
                NarrowGridOilSOPLinkSupplement();
                NarrowGridOilCFPLinkSupplement();
                NarrowGridOilSAV_ARVLinkSupplement();
                NarrowGridOilANILinkSupplement();
                NarrowGridOilR20LinkSupplement();
                NarrowGridOilR70LinkSupplement();
                NarrowGridOilKFCLinkSupplement();
                NarrowGridOilBMILinkSupplement();
                NarrowGridOilDILinkSupplement();
                NarrowGridOilCILinkSupplement();
                NarrowGridOilVG4LinkSupplement();
                NarrowGridOilV1GLinkSupplement();
                NarrowGridOilMWLinkSupplement();
                this._narrowGridOil.IsBusy = false;
            }
            catch (Exception ex)
            {
                this._narrowGridOil.IsBusy = false;
                Log.Error("窄馏分表的数据补充" + ex);
            }
            //NarrowGridOilECPLinkSupplement();
            //NarrowGridOilFPOLinkSupplement();
            //
        }
        #endregion

        #region 宽馏分表
        /// <summary>
        /// 宽馏分表关联补充
        /// </summary>
        public void WideGridOilLinkSupplement()
        {
            try
            {
                //LabtoCal();
                this._wideGridOil.IsBusy = true;
                WideGridOilICPinkSupplement();//宽馏分的第一列ICP没有值时，取窄馏分第一列的ICP
                WideGridOilWYLinkSupplement();
                WideGridOilTWYLinkSupplement();
                WideGridOilVYLinkSupplement();
                WideGridOilTVYLinkSupplement();
                WideGridOilAPILinkSupplement();
                WideGridOilD20LinkSupplement();
                WideGridOilD60LinkSupplement();
                WideGridOilD15LinkSupplement();
                WideGridOilD70LinkSupplement();
                WideGridOilSGLinkSupplement();
                WideGridOilWYDLinkSupplement();
                WideGridOilMWYLinkSupplement();
                WideGridOilMCPLinkSupplement();
                WideGridOilV0LinkSupplement();
                WideGridOilVILinkSupplement();
                WideGridOilVG4LinkSupplement();
                WideGridOilV1GLinkSupplement();
                WideGridOilR20LinkSupplement();
                WideGridOilR70LinkSupplement();
                WideGridOilC_HLinkSupplement();
                WideGridOilSULLinkSupplement();
                WideGridOilN2LinkSupplement();
                WideGridOilBANLinkSupplement();
                WideGridOilMECLinkSupplement();
                WideGridOilNETLinkSupplement();
                WideGridOilACDLinkSupplement();
                WideGridOilMWLinkSupplement();
                WideGridOilA_PLinkSupplement();//馏程算法,TVY-ECP曲线=>?AIP, A10, A30, A50,A70,A90,AEP
                WideGridOilKFCLinkSupplement();
                WideGridOilBMILinkSupplement();
                WideGridOilANILinkSupplement();
                WideGridOilPANLinkSupplement();
                WideGridOilPAOLinkSupplement();
                WideGridOilNAHLinkSupplement();
                WideGridOilARMLinkSupplement();
                WideGridOilGCTLinkSupplement();
                WideGridOilARPLinkSupplement();
                WideGridOilN2ALinkSupplement();
                WideGridOilCHRLinkSupplement();
                WideGridOilRVPLinkSupplement();
                WideGridOilFRZLinkSupplement();
                WideGridOilSMKLinkSupplement();
                WideGridOilSAVLinkSupplement();
                WideGridOilARVLinkSupplement();
                WideGridOilOLVLinkSupplement();
                WideGridOilLHVLinkSupplement();
                WideGridOilIRTLinkSupplement();
                WideGridOilPORLinkSupplement();
                WideGridOilSOPLinkSupplement();
                WideGridOilCLPLinkSupplement();
                NIVLinkSupplement();
                WideGridOilCILinkSupplement();
                WideGridOilCENLinkSupplement();
                WideGridOilDILinkSupplement();
                WideGridOilSAHLinkSupplement();
                WideGridOilARSLinkSupplement();
                WideGridOilRESLinkSupplement();
                WideGridOilAPHLinkSupplement();
                WideGridOil4CTLinkSupplement();               
                WideGridOilCPP_RAALinkSupplement();
                WideGridOilPATLinkSupplement();
                WideGridOilMNALinkSupplement();
                WideGridOilMSPLinkSupplement();
                WideGridOilMA1LinkSupplement();
                WideGridOilMA2LinkSupplement();
                WideGridOilMA3LinkSupplement();
                WideGridOilMA4LinkSupplement();
                WideGridOilMA5LinkSupplement();
                WideGridOilMANLinkSupplement();
                WideGridOilMATLinkSupplement();
                WideGridOilMTALinkSupplement();

                getGCCalToWideSupplement();

                this._wideGridOil.IsBusy = false;
            }
            catch (Exception ex)
            {
                this._wideGridOil.IsBusy = false;
                Log.Error("宽馏分表的数据补充" + ex);
            }
            // WideGridOilFPOLinkSupplement();
            //WideGridOilPATLinkSupplement();
            //
        }
        #endregion

        #region 渣油
        #region 渣油表的实测值补充校正值
        /// <summary>
        /// 渣油表的实测值补充校正值
        /// </summary>
        public void DataCorrectionSupplement()
        {
            try 
            {          
                FrmResiduDataSupplementDialog frmResiduDataSupplementDialog = new FrmResiduDataSupplementDialog();
                frmResiduDataSupplementDialog.Init(this._wholeGridOil, this._lightGridOil, this._gcGridOil, this._narrowGridOil, this._wideGridOil, this._residueGridOil);
                frmResiduDataSupplementDialog.TopMost = true;
                frmResiduDataSupplementDialog.ShowDialog();

                this._residueGridOil.IsBusy = true;
                ResidueWYDataCorrectionSupplement();
                ResidueTWYDataCorrectionSupplement();
                VYTVYDataCorrectionSupplement();
                wideVYTVYDataCorrectionSupplement();
                resVYTVYDataCorrectionSupplement();
                //ResidueTVYDataCorrectionSupplement();

                if (frmResiduDataSupplementDialog.DialogResult == DialogResult.OK)
                {
                    frmResiduDataSupplementDialog.Close();                    
                    List<OilDataEntity> residueOilDataList = this._residueGridOil.GetAllData();
                    if (GlobalResiduDataSupplementDialog._CCR == "1")//原油校正渣油
                    {
                        ResidueCCRDataCorrectionSupplement();
                    }
                    else if (GlobalResiduDataSupplementDialog._CCR == "2")//渣油校正原油
                    {
                        if (GlobalResiduDataSupplementDialog._CCRDrop != -1)
                        {
                            suppleWholefromResidue(residueOilDataList, "CCR", GlobalResiduDataSupplementDialog._CCRDrop);
                        }
                    }

                    ResidueFEDataCorrectionSupplement();//Fe不管在那种情况下都是原油矫正渣油
                    if (GlobalResiduDataSupplementDialog._MET == "1")//原油校正渣油
                    {
                        //ResidueFEDataCorrectionSupplement();
                        ResidueNIDataCorrectionSupplement();
                        ResidueVDataCorrectionSupplement();
                        ResidueCADataCorrectionSupplement();
                        ResidueNADataCorrectionSupplement();
                    }
                    else if (GlobalResiduDataSupplementDialog._MET == "2")//渣油校正原油
                    {
                        if (GlobalResiduDataSupplementDialog._METDrop != -1)
                        {
                            //suppleWholefromResidue(residueOilDataList, "FE", GlobalResiduDataSupplementDialog._METDrop);
                            suppleWholefromResidue(residueOilDataList, "NI", GlobalResiduDataSupplementDialog._METDrop);
                            suppleWholefromResidue(residueOilDataList, "V", GlobalResiduDataSupplementDialog._METDrop);
                            suppleWholefromResidue(residueOilDataList, "CA", GlobalResiduDataSupplementDialog._METDrop);
                            suppleWholefromResidue(residueOilDataList, "NA", GlobalResiduDataSupplementDialog._METDrop);
                        }
                    }

                    if (GlobalResiduDataSupplementDialog._APH == "1")//原油校正渣油
                    {
                        ResidueAPHDataCorrectionSupplement();
                        ResidueRESDataCorrectionSupplement();

                        ResiduDataSAHARSCorr();
                      
                    }
                    else if (GlobalResiduDataSupplementDialog._APH == "2")//渣油校正原油
                    {
                        if (GlobalResiduDataSupplementDialog._APHDrop != -1)
                        {
                            suppleWholefromResidue(residueOilDataList, "APH", GlobalResiduDataSupplementDialog._APHDrop);
                            suppleWholefromResidue(residueOilDataList, "RES", GlobalResiduDataSupplementDialog._APHDrop);
                            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
                            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");

                            ResiduDataSAHARSCorr();
                        }
                    }
                }
                //ResidueD20DataCorrectionSupplement();
                //ResidueD15DataCorrectionSupplement();
                //ResidueD60DataCorrectionSupplement();
                //ResidueAPIDataCorrectionSupplement();
                this._residueGridOil.IsBusy = false;
            }
            catch (Exception ex)
            {
                this._residueGridOil.IsBusy = false;
                Log.Error("渣油表的数据强制补充" + ex);
            }       
        }
        /// <summary>
        /// SAHARS强制校正补充
        /// </summary>
        private void ResiduDataSAHARSCorr()
        {
            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");
            List<OilDataEntity> SAHOilDataList = this._gridOil.GetDataByRowItemCode("SAH");
            List<OilDataEntity> ARSOilDataList = this._gridOil.GetDataByRowItemCode("ARS");

            if (APHOilDataList == null || APHOilDataList == null
                || SAHOilDataList == null || ARSOilDataList == null)
                return;
            if (APHOilDataList.Count <= 0 || RESOilDataList.Count <= 0
                || SAHOilDataList.Count <= 0 || ARSOilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                #region " "
                string APHcal = getStrValuefromOilDataEntity(APHOilDataList, i);
                string REScal = getStrValuefromOilDataEntity(RESOilDataList, i);
                string SAHcal = getStrValuefromOilDataEntity(SAHOilDataList, i);
                string ARScal = getStrValuefromOilDataEntity(ARSOilDataList, i);

                float APH = 0, RES = 0, SAH = 0, ARS = 0;
                if (float.TryParse(APHcal, out APH) && float.TryParse(REScal, out RES)
                    && float.TryParse(SAHcal, out SAH) && float.TryParse(ARScal, out ARS))
                {
                    SAH = SAH / (SAH + ARS) * (100 - RES - APH);
                    ARS = (100 - RES - APH - SAH);

                    if (SAH.ToString() != string.Empty && SAH.ToString() != "非数字")
                        this._gridOil.SetData("SAH", i, SAH.ToString());

                    if (ARS.ToString() != string.Empty && ARS.ToString() != "非数字")
                        this._gridOil.SetData("ARS", i, ARS.ToString());
                }
                #endregion
            }
        
        }
        #endregion

        #region 油渣强制校正原油
        /// <summary>
        /// 油渣强制校正原油
        /// </summary>
        /// <param name="residueOilDataList"></param>
        /// <param name="itemCode"></param>
        /// <param name="colIndex"></param>
        private void suppleWholefromResidue(List<OilDataEntity> residueOilDataList , string itemCode , int colIndex)
        {
            if (residueOilDataList.Count > 0)
            {
                List<OilDataEntity> oilDataList = residueOilDataList.Where(o => o.calData != string.Empty).ToList();
                if (residueOilDataList.Count > 0)
                {
                    OilDataEntity itemCodeOilData = residueOilDataList.Where(o => o.OilTableRow.itemCode == itemCode && o.ColumnIndex == colIndex).FirstOrDefault();
                    OilDataEntity WYOilData = residueOilDataList.Where(o => o.OilTableRow.itemCode == "WY" && o.ColumnIndex == colIndex).FirstOrDefault();

                    
                    if (itemCodeOilData != null && itemCodeOilData.calData != string.Empty
                        && WYOilData != null && WYOilData.calData != string.Empty)
                    {
                        float itemCodeValue = 0; float WY = 0;
                        if (float.TryParse(itemCodeOilData.calData, out itemCodeValue) && float.TryParse(WYOilData.calData, out WY))
                        {
                            #region "补充原油表"
                            float wholeitemCodeValue = itemCodeValue * WY / 100;
                            if (!wholeitemCodeValue.Equals(float.NaN))
                            {
                                this._wholeGridOil.SetData(itemCode, 0, wholeitemCodeValue.ToString());
                            }
                            #endregion

                            #region "补充渣油表"
                            int maxCol = this._residueGridOil.GetMaxValidColumnIndex() + 1;
                            for (int i = 0; i < maxCol; i++)
                            {
                                List<OilDataEntity> WYOilDataList = residueOilDataList.Where(o=>o.OilTableRow.itemCode == "WY").ToList();
                                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                                float tempWY = 0;
                                if (WYcal != string.Empty  && float.TryParse(WYcal, out tempWY))
                                {
                                    float tempData = itemCodeValue * WY / tempWY;

                                    if (!tempData.Equals(float.NaN))
                                    {
                                        this._residueGridOil.SetData(itemCode, i, tempData.ToString());
                                    }
                                }                            
                            }
                            #endregion
                        }
                    }                                  
                }
            }
        }
        #endregion

        #region 渣油关联补充
        /// <summary>
        /// 渣油表关联补充
        /// </summary>
        public void ResidueLinkSupplement()
        {
            try
            {
                //LabtoCal();
                this._residueGridOil.IsBusy = true;
                ResidueGridOilWYLinkSupplement();
                ResidueGridOilTWYLinkSupplement();/////
                ResidueGridOilVYLinkSupplement();
                ResidueGridOilTVYLinkSupplement();////
                ResidueGridOilAPILinkSupplement();
                ResidueGridOilD20LinkSupplement();
                ResidueGridOilD15LinkSupplement();
                ResidueGridOilD60LinkSupplement();
                WideGridOilSGLinkSupplement();
                ResidueGridOilV0LinkSupplement();
                ResidueGridOilVILinkSupplement();
                //ResidueGridOilNIVLinkSupplement();
                ResidueGridOilVG4LinkSupplement();
                ResidueGridOilV1GLinkSupplement();
                ResidueGridOilPORLinkSupplement();
                ResidueGridOilSOPLinkSupplement();
                ResidueGridOilMixSupplementAccumulate("CCR");
                ResidueGridOilMixSupplementAccumulate("ASH");
                ResidueGridOilMWLinkSupplement();
                ResidueGridOilMixSupplementAccumulate("CAR");
                ResidueGridOilMixSupplementAccumulate("H2");
                ResidueGridOilC_HLinkSupplement();
                ResidueGridOilMixSupplementAccumulate("SUL");
                ResidueGridOilMixSupplementAccumulate("N2");
                ResidueGridOilMixSupplementAccumulate("FE");
                ResidueGridOilMixSupplementAccumulate("NI");
                ResidueGridOilMixSupplementAccumulate("V");
                ResidueGridOilMixSupplementAccumulate("CA");
                ResidueGridOilMixSupplementAccumulate("NA");
                ResidueGridOilMixSupplementAccumulate("CU");
                ResidueGridOilMixSupplementAccumulate("PB");
                ResidueGridOilMixSupplementAccumulate("MG");
                ResidueGridOilNIVLinkSupplement();
                ResidueGridOilMixSupplementAccumulate("SAH");
                ResidueGridOilMixSupplementAccumulate("ARS");
                ResidueGridOilMixSupplementAccumulate("RES");
                ResidueGridOilMixSupplementAccumulate("APH");
                ResidueGridOilFFA_CII_TCC_CA_RNN_RAA_RTTLinkSupplement();
                ResidueGridOilKFCLinkSupplement();
                this._residueGridOil.IsBusy = false ;
            }
            catch (Exception ex)
            {
                this._residueGridOil.IsBusy = false;
                Log.Error("渣油表的数据补充" + ex);
            }
        }
        #endregion

        #endregion

        #endregion

        #region "从五个关联补充函数中补充函数"
        #region "原油信息"
        /// <summary>
        /// API->CLA
        /// </summary>
        private void OilInfoGridOilCLALinkSupplement()
        {
            CLA_WholeLinkSupplement();
        }
        /// <summary>
        /// 原油信息表的TYP补充
        /// </summary>
        private void OilInfoGridOilTYPLinkSupplement()
        {
            string strResult = string.Empty;
            strResult = strTYP();//前两步

            #region "第三步"
            if (strResult == string.Empty)//数据仍然没有补充完整
            {
                #region "通过KFC计算"
                //K>12.1  ,则TYP＝“石蜡基”
                //12.1＝>K＝>11.5  ,则TYP＝“中间基”
                //11.5>K=>10.5 ,则TYP＝“环烷基”
                var KFCOilDataEntiy = this._wholeGridOil.GetDataByRowItemCodeColumnIndex("KFC",0);

                if (KFCOilDataEntiy != null)
                {
                    float K = 0;
                    if (float.TryParse(KFCOilDataEntiy.calData, out K))
                    {
                        if (K > 12.1)
                            strResult = "石蜡基";
                        else if (K >= 11.5 && K < 12.1)
                            strResult = "中间基";
                        else if (K >= 10.5 && K < 11.5)
                            strResult = "环烷基";
                    }
                }

                #endregion
            }
            #endregion

            if (strResult != string.Empty)
            {
                for (int i = 0; i < this._grdiOilInfo.Rows.Count; i++)
                {
                    DataGridViewRow row = this._grdiOilInfo.Rows[i];
                    if (row.Tag.ToString() == "TYP")
                    {
                        this._grdiOilInfo.Rows[i].Cells["itemValue"].Value = strResult;
                        this._oilInfo.classification = strResult;
                    }
                }
            }
        }
        /// <summary>
        /// TYP的前两步骤补充
        /// </summary>
        /// <returns></returns>
        private string strTYP()
        {
            string strResult = string.Empty;

            var NarrowICPList = this._narrowGridOil.GetDataByRowItemCode("ICP").ToList();//选出窄馏分ICP行的数据
            var NarrowECPList = this._narrowGridOil.GetDataByRowItemCode("ECP").ToList();//选出窄馏分ECP行的数据
            var NarrowAPIList = this._narrowGridOil.GetDataByRowItemCode("API").ToList();//选出窄馏分API行的数据
            var NarrowTWYList = this._narrowGridOil.GetDataByRowItemCode("TWY").ToList();//选出窄馏分WY行的数据

            if (NarrowICPList == null || NarrowECPList == null || NarrowAPIList == null || NarrowTWYList == null)
                return strResult;

            if (NarrowICPList.Count <= 0 || NarrowECPList.Count <= 0 || NarrowAPIList.Count <= 0)
                return strResult;
            try
            {

                #region "第一步"
                #region "查找到ICP和ICP馏分"
                // 在窄馏分表完成数据补充后，在其中找250-275, 395-425两个窄馏分。找到，读取相应的API(250-275), API(395-425)。
                #region "提取250-275馏分段"
                var ICP250s = NarrowICPList.Where(o => o.calData == "250").ToList();
                var ECP275s = NarrowECPList.Where(o => o.calData == "275").ToList();
                if (ICP250s.Count <= 0 || ECP275s.Count <= 0)
                    strResult = string.Empty;

                OilDataEntity ICP250 = null; OilDataEntity ECP275 = null;
                for (int ICPIndex = 0; ICPIndex < ICP250s.Count; ICPIndex++)
                {
                    ICP250 = ICP250s[ICPIndex];
                    ECP275 = ECP275s.Where(o => o.calData == "275" && o.OilTableCol.colCode == ICP250.OilTableCol.colCode).FirstOrDefault();

                    if (ECP275 == null)
                        continue;
                    else
                        break;
                }
                #endregion

                #region "提取395-425馏分段"
                var ICP395s = NarrowICPList.Where(o => o.calData == "395").ToList();
                var ECP425s = NarrowECPList.Where(o => o.calData == "425").ToList();
                if (ICP395s.Count <= 0 || ECP425s.Count <= 0)
                    strResult = string.Empty;

                OilDataEntity ICP395 = null; OilDataEntity ECP425 = null;
                for (int ICPIndex = 0; ICPIndex < ICP395s.Count; ICPIndex++)
                {
                    ICP395 = ICP395s[ICPIndex];
                    ECP425 = ECP425s.Where(o => o.calData == "425" && o.OilTableCol.colCode == ICP395.OilTableCol.colCode).FirstOrDefault();

                    if (ECP425 == null)
                        continue;
                    else
                        break;
                }
                #endregion

                if (ICP250 != null && ECP275 != null && ICP395 != null && ECP425 != null)
                {
                    OilDataEntity API250_275 = NarrowAPIList.Where(o => o.OilTableCol.colCode == ICP250.OilTableCol.colCode).FirstOrDefault();
                    OilDataEntity API395_425 = NarrowAPIList.Where(o => o.OilTableCol.colCode == ICP395.OilTableCol.colCode).FirstOrDefault();

                    if (API250_275 != null && API395_425 != null)
                    {
                        string strAPICut1 = API250_275.calData;
                        string strAPICut2 = API395_425.calData;
                        float fAPICut1 = 0, fAPICut2 = 0;
                        if (float.TryParse(strAPICut1, out fAPICut1) && float.TryParse(strAPICut2, out fAPICut2))
                        {
                            strResult = BaseFunction.FunTYPfromAPI1_API2(fAPICut1, fAPICut2);
                        }
                    }
                }
                #endregion
                #endregion

                #region "第二步"
                if (strResult == string.Empty || strResult == "非数字")
                {
                    IList<CutMothedEntity> cutMotheds = new List<CutMothedEntity>();
                    CutMothedEntity cutMothed1 = new CutMothedEntity
                    {
                        Name = "1",
                        ICP = 395,
                        ECP = 425
                    }; cutMotheds.Add(cutMothed1);

                    CutMothedEntity cutMothed2 = new CutMothedEntity
                    {
                        Name = "1",
                        ICP = 250,
                        ECP = 275
                    }; cutMotheds.Add(cutMothed2);

                    CurveEntity ECP_TWYCurve = OilApplyBll.oilDatasToCurve(NarrowECPList, NarrowTWYList);
                    CurveEntity ECP_APICurve = OilApplyBll.oilDatasToCurve(NarrowECPList, NarrowAPIList);
                    if (ECP_TWYCurve != null && ECP_APICurve != null)
                    {
                        CurveEntity curveECPTWY = OilApplyBll.oilCurveCut(ECP_TWYCurve, cutMotheds, "");
                        CurveEntity curveECPAPI = OilApplyBll.oilCurveCut(ECP_APICurve, cutMotheds, ECP_TWYCurve, "");
                        if (curveECPTWY != null && curveECPAPI != null && curveECPTWY.curveDatas.Count >= 4 && curveECPAPI.curveDatas.Count >= 4)
                        {
                            List<float> ICPECP = new List<float>(); ICPECP.Add(250); ICPECP.Add(275); ICPECP.Add(395); ICPECP.Add(425);
                            CurveDataEntity TWY0 = curveECPTWY.curveDatas.Where(o => o.xValue == ICPECP[0]).FirstOrDefault();
                            CurveDataEntity API0 = curveECPAPI.curveDatas.Where(o => o.xValue == ICPECP[0]).FirstOrDefault();
                            CurveDataEntity TWY1 = curveECPTWY.curveDatas.Where(o => o.xValue == ICPECP[1]).FirstOrDefault();
                            CurveDataEntity API1 = curveECPAPI.curveDatas.Where(o => o.xValue == ICPECP[1]).FirstOrDefault();
                            CurveDataEntity TWY2 = curveECPTWY.curveDatas.Where(o => o.xValue == ICPECP[2]).FirstOrDefault();
                            CurveDataEntity API2 = curveECPAPI.curveDatas.Where(o => o.xValue == ICPECP[2]).FirstOrDefault();
                            CurveDataEntity TWY3 = curveECPTWY.curveDatas.Where(o => o.xValue == ICPECP[3]).FirstOrDefault();
                            CurveDataEntity API3 = curveECPAPI.curveDatas.Where(o => o.xValue == ICPECP[3]).FirstOrDefault();

                            float fAPICut1 = (API1.yValue - API0.yValue) / (TWY1.yValue - TWY0.yValue);
                            float fAPICut2 = (API3.yValue - API2.yValue) / (TWY3.yValue - TWY2.yValue);
                            strResult = BaseFunction.FunTYPfromAPI1_API2(fAPICut1, fAPICut2);
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                Log.Error("TYP关联补充错误：" + ex.ToString());
                return strResult;
            }

            return strResult;
        }
        /// <summary>
        /// 原油信息表的SCL补充
        /// </summary>
        private void OilInfoGridOilSCLLinkSupplement()
        {
            string strResult = string.Empty;

            #region "通过KFC计算"
            //原油表中SUL有值，则判断：
            //SUL<0.5 ,则SCL=“低硫”
            //SUL<2, & SUL＝>0.5 ,则SCL=“含硫”
            //SUL＝>2 ,则SCL=“高硫”
            var datas = this._oilInfo.OilDatas.Where(d => d.OilTableTypeID == (int)EnumTableType.Whole).ToList();

            var SULOilDataEntiy = datas.Where(o => o.OilTableRow.itemCode == "SUL").FirstOrDefault();//选出窄馏分ICP行的数据

            if (SULOilDataEntiy != null)
            {
                float SUL = 0;
                if (float.TryParse(SULOilDataEntiy.calData, out SUL))
                {
                    if (SUL < 0.5)
                        strResult = "低硫";
                    else if (SUL >= 0.5 && SUL < 2)
                        strResult = "含硫";
                    else if (SUL >= 2)
                        strResult = "高硫";
                }
            }

            #endregion

            if (strResult != string.Empty)
            {
                for (int i = 0; i < this._grdiOilInfo.Rows.Count; i++)
                {
                    DataGridViewRow row = this._grdiOilInfo.Rows[i];
                    if (row.Tag.ToString() == "SCL")
                    {
                        this._grdiOilInfo.Rows[i].Cells["itemValue"].Value = strResult;
                        this._oilInfo.sulfurLevel = strResult;
                    }
                }
            }
        }

        /// <summary>
        /// 原油信息表的ACL补充
        /// </summary>
        private void OilInfoGridOilACLLinkSupplement()
        {
            string strResult = string.Empty;

            #region "通过NET计算"
            //原油表中NET有值，则判断：
            //NET<=0.5 ,则ACL=“低酸”
            //NET>0.5 & NET<1 ,则ACL=“含酸”
            //NET>1 ,则ACL=“高酸”

            var NETOilDataEntiy = this._wholeGridOil.GetDataByRowItemCodeColumnIndex ("NET",0); 

            if (NETOilDataEntiy != null)
            {
                float NET = 0;
                if (float.TryParse(NETOilDataEntiy.calData, out NET))
                {
                    if (NET < 0.5)
                        strResult = "低酸";
                    else if (NET >= 0.5 && NET < 1)
                        strResult = "含酸";
                    else if (NET >= 1)
                        strResult = "高酸";
                }
            }

            #endregion


            if (strResult != string.Empty)
            {
                for (int i = 0; i < this._grdiOilInfo.Rows.Count; i++)
                {
                    DataGridViewRow row = this._grdiOilInfo.Rows[i];
                    if (row.Tag.ToString() == "ACL")
                    {
                        this._grdiOilInfo.Rows[i].Cells["itemValue"].Value = strResult;
                        this._oilInfo.acidLevel = strResult;
                    }
                }
            }
        }

        #endregion           

        #region "原油性质表关联补充"
        /// <summary>
        /// WY(i)=VY(i)*D20(i)/D20(原油)
        /// </summary>
        private void WYLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");

            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");

            for (int i = 0; i < this._maxCol; i++)
            {
                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);

                /*WY(i)=VY(i)*D20(i)/D20(原油)*/
                #region

                if (WYcal != string.Empty)
                    continue;

                if (WYcal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);

                    var ds = this._wideGridOil.GetAllData();
                    OilDataEntity oilDataD20Whole = ds.Where(o => o.OilTableRow.itemCode == "D20").FirstOrDefault();//选出原油性质的D20校正值 

                    if (oilDataD20Whole == null)
                        continue;

                    WYcal = BaseFunction.FunWY(VYcal, D20cal, oilDataD20Whole.calData);
                }

                if (WYcal != string.Empty && WYcal != "非数字")
                {
                    this._gridOil.SetData("WY", i, WYcal);
                }
                #endregion
            }        
        }
        /// <summary>
        /// D20->API
        /// D60->API
        /// </summary>
        private void WholeGridOilAPILinkSupplement()
        {
            APILinkSupplement();
        }

        /// <summary>
        /// D60->D20
        /// API->D20
        /// </summary>
        private void WholeGridOilD20LinkSupplement()
        {
            ResidueD20LinkSupplement();
        }

        /// <summary>
        /// D20->D15
        /// </summary>
        private void WholeGridOilD15LinkSupplement()
        {
            D15LinkSupplement();
        }

        /// <summary>
        ///D20->D60
        ///API->D60
        /// </summary>
        private void WholeGridOilD60LinkSupplement()
        {
            D60LinkSupplement();
        }

        /// <summary>
        /// 
        /// </summary>
        private void WholeGridOilSGLinkSupplement()
        {
            SGLinkSupplement();
        }

        /// <summary>
        /// T1,V1, T2, V2, T3->V3
        /// </summary>
        private void WholeGridOilV0LinkSupplement()
        {
            V0_LinkSupplement();

            V0_Condition();
        }

        /// <summary>
        ///KFC=X/SG
        ///X=(4-SG)*(1-4.6593/(5+LN(V10)))+8.24
        ///SG=D60/DH2O60F
        /// </summary>
        private void WholeGridOilKFCLinkSupplement()
        {
            KFCWholeLinkSupplement();
        }

        ///// <summary>
        ///// 原油性质 FPO关联补充
        ///// </summary>
        //private void WholeGridOilFPOLinkSupplement()
        //{
        //    FPOLinkSupplement();
        //}

        ///// <summary>
        ///// 原油性质 CLA关联补充
        ///// </summary>
        //private void WholeGridOilCLALinkSupplement()
        //{
        //    CLA_WholeLinkSupplement();
        //}
        #endregion 

        #region "窄馏分"

        #region "ICP ECP"
        #region "窄馏分ICP的原始补充"
        ///// <summary>
        ///// 由ECP补充ICP，第一馏分列为特殊馏分列
        ///// </summary>
        //private void NarrowGridOilICPLinkSupplement()
        //{
        //    #region
        //    for (int i = 0; i < this._maxCol; i++)
        //    {
        //        string ICPlab = this._gridOil[this._cols[i].ColumnIndex - 1, ICPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex - 1, ICPRow[0].RowIndex].Value.ToString() : string.Empty;
        //        string ICPcal = this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ICPRow[0].RowIndex].Value.ToString() : string.Empty;

        //        if (ICPcal != string.Empty)
        //            continue;

        //        if (ICPcal == string.Empty && ICPlab != string.Empty)
        //        {
        //            ICPcal = ICPlab;
        //        }

        //        #region 
        //        if (ICPcal == string.Empty)
        //        {
        //            if (i == 0)
        //            {
        //                #region "i == 0"

        //                if (MessageBox.Show("第一窄馏分是否为轻端？", "窄馏分数据补充", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //                {
        //                    List<OilDataEntity> lightDatas = this._parent.Oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Light).ToList();
        //                    bool Havelight = (lightDatas.Count > 0) ? true : false;//表示轻端不为空
        //                    if (Havelight)
        //                    {
        //                        List<string> itemCodes = new List<string>();
        //                        for (int k = 0; k < lightDatas.Count; k++)
        //                        {
        //                            float temp = 0;
        //                            if (lightDatas[k].calData != string.Empty && float.TryParse(lightDatas[k].calData, out temp))
        //                                itemCodes.Add(lightDatas[k].OilTableRow.itemCode);
        //                        }

        //                        LightCurveParmTableAccess LIGHTCURVEPARM = new LightCurveParmTableAccess();
        //                        List<LightCurveParmTableEntity> LightCurveParmList = LIGHTCURVEPARM.Get("1=1");
        //                        List<LightCurveParmTableEntity> tempList = LightCurveParmList.Where(o => o.Tb != string.Empty && itemCodes.Contains (o.ItemCode)).ToList();
        //                        tempList.Sort(new LightCurveParmTableEntityComparable());//Tb升序排序

        //                        string ECPcal = this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, ECPRow[0].RowIndex].Value.ToString() : string.Empty;

        //                        for (int j = 0; j < tempList.Count - 2; j++)
        //                        {
        //                            var lightData = lightDatas.Where(o => o.OilTableRow.itemCode == tempList[j].ItemCode).FirstOrDefault();
        //                            var lightDataNext = lightDatas.Where(o => o.OilTableRow.itemCode == tempList[j + 1].ItemCode).FirstOrDefault();

        //                            if (lightData != null)
        //                            {
        //                                float fCal = 0;
        //                                if (float.TryParse(lightData.calData, out fCal))
        //                                {
        //                                    if (fCal > 1)
        //                                    {
        //                                        ICPcal = tempList[j].Tb;
        //                                        float ECP =0 ,ICP = 0;
        //                                        if (float.TryParse(ECPcal, out ECP) && float.TryParse(ICPcal, out ICP))
        //                                        {
        //                                            if (ICP >= ECP)
        //                                            {
        //                                                ICPcal = string.Empty;
        //                                                MessageBox.Show("轻端表有问题！", "窄馏分数据补充", MessageBoxButtons.OKCancel);
        //                                            }
        //                                        }
        //                                        break;
        //                                    }
        //                                    else if (fCal < 1 )
        //                                    {
        //                                        if (lightDataNext != null)
        //                                        {
        //                                            float fNextCal = 0;
        //                                            if (float.TryParse(lightDataNext.calData, out fNextCal))
        //                                            {
        //                                                if ((fCal + fNextCal) >= 1)//ICP＝X（i）* Tb（i） + (1- X（i）) * Tb (i+1)。
        //                                                {
        //                                                    float ICP = fCal * Convert.ToSingle(tempList[j].Tb) + (1 - fCal) * Convert.ToSingle(tempList[j + 1].Tb);
        //                                                    ICPcal = ICP.ToString();

        //                                                    float ECP = 0;
        //                                                    if (float.TryParse(ECPcal, out ECP))
        //                                                    {
        //                                                        if (ICP >= ECP)
        //                                                        {
        //                                                            ICPcal = string.Empty;
        //                                                            MessageBox.Show("轻端表有问题！", "窄馏分数据补充", MessageBoxButtons.OKCancel);
        //                                                        }
        //                                                    }
        //                                                    break;
        //                                                }
        //                                                else//ICP＝X（i）* Tb（i） + X（i+1）* Tb (i+1) + (1-X(i) – X(i+1) ) * Tb(i+2)。
        //                                                {
        //                                                    float ICP = fCal * Convert.ToSingle(tempList[j].Tb) + fNextCal * Convert.ToSingle(tempList[j + 1].Tb) + (1 - fCal - fNextCal) * Convert.ToSingle(tempList[j + 2].Tb);
        //                                                    ICPcal = ICP.ToString();
        //                                                    float ECP = 0;
        //                                                    if (float.TryParse(ECPcal, out ECP))
        //                                                    {
        //                                                        if (ICP >= ECP)
        //                                                        {
        //                                                            ICPcal = string.Empty;
        //                                                            MessageBox.Show("轻端表有问题！", "窄馏分数据补充", MessageBoxButtons.OKCancel);
        //                                                        }
        //                                                    }
        //                                                    break;
        //                                                }
        //                                            }
        //                                        }
        //                                    }

        //                                }

        //                            }
        //                        }

        //                    }
        //                    else
        //                    {
        //                        ICPcal = "-100";
        //                    }   
        //                }
        //                else
        //                {
        //                    ICPcal = "-100";
        //                }                                       
        //                #endregion
        //            }
        //            else
        //            {
        //                #region "i != 0"

        //                string ECPcal = this._gridOil[this._cols[i - 1].ColumnIndex, ECPRow[0].RowIndex].Value != null ? this._gridOil[this._cols[i - 1].ColumnIndex, ECPRow[0].RowIndex].Value.ToString() : string.Empty;

        //                if (ECPcal != string.Empty)
        //                {
        //                    ICPcal = ECPcal;
        //                }

        //                #endregion
        //            }
        //        }
        //        #endregion 

        //        if (ICPcal != string.Empty)
        //        {

        //            oilDataEdit.OilDataSupplementPaste(ICPcal, this._cols[i].ColumnIndex, ICPRow[0].RowIndex);
        //        }
        //    }
        //    #endregion
        //}
        #endregion

        /// <summary>
        /// IPC0的轻端表补充
        /// </summary>
        /// <param name="lightDatas"></param>
        /// <param name="WYcal"></param>
        /// <returns></returns>
        public static string ICP0Supplement(GridOilViewA lightGridOil, float fLightWY)
        {
            string ICP0cal = string.Empty;

            if (lightGridOil == null )
                return ICP0cal;

            List<OilDataEntity> allLightDatas = lightGridOil.GetAllData();
            List<OilDataEntity> LightDataList = allLightDatas.Where(o => o.OilTableCol.colCode == "Cut1" && o.calData != string.Empty).ToList();

            #region "第一窄馏分是否为轻端？"
            if (LightDataList.Count >0)
            {
                if (fLightWY < 0.03)
                {
                    ICP0cal = "-10";
                    return ICP0cal;
                }

                #region "用于轻端数据归一的加和数据"
                Dictionary<string, float> originDIC = new Dictionary<string, float>();
                float light_sum = 0;//用于轻端数据归一的加和数据
                for (int k = 0; k < LightDataList.Count; k++)
                {
                    float tempLightData = 0;
                    if (LightDataList[k].calData != string.Empty && float.TryParse(LightDataList[k].calData, out tempLightData))
                    {
                        if (LightDataList[k].OilTableRow.itemCode != "CO2" && LightDataList[k].OilTableRow.itemCode != "N2" && LightDataList[k].OilTableRow.itemCode != "O2" && LightDataList[k].OilTableRow.itemCode != "H2S")
                        {
                            if (!originDIC.Keys.Contains(LightDataList[k].OilTableRow.itemCode))
                            {
                                originDIC.Add(LightDataList[k].OilTableRow.itemCode, tempLightData);
                                light_sum += tempLightData;
                            }
                        }
                    }
                }
                #endregion 

                #region "归一化后的轻端的组成含量"
                Dictionary<string, float> calDIC = new Dictionary<string, float>();                                
                foreach (string strKey in originDIC.Keys)
                {
                    if (!calDIC.Keys.Contains(strKey))
                        calDIC.Add(strKey, originDIC[strKey] / light_sum * fLightWY);
                }
                #endregion 

                List<string> itemCodes = originDIC.Keys.ToList();

                LightCurveParmTableAccess LIGHTCURVEPARM = new LightCurveParmTableAccess();
                List<LightCurveParmTableEntity> LightCurveParmList = LIGHTCURVEPARM.Get("1=1");
                List<LightCurveParmTableEntity> tempList = LightCurveParmList.Where(o => o.Tb != string.Empty && itemCodes.Contains(o.ItemCode)).ToList();
                tempList.Sort(new LightCurveParmTableEntityComparable());//Tb升序排序

                #region "补充ICP"
                float TOTAL = 0, TOTAL1 = 0;//加和
                foreach (var lightItem in tempList)
                {
                    TOTAL += calDIC[lightItem.ItemCode];
                    if (TOTAL > 0.03)
                    {
                        TOTAL1 = TOTAL1 + (0.03f - TOTAL + calDIC[lightItem.ItemCode]) * Convert.ToSingle(lightItem.Tb);
                        ICP0cal = (TOTAL1 / 0.03).ToString();
                        break;
                    }
                    else //TOTAL1=TOTAL1+WX(j)* j.Tb
                        TOTAL1 = TOTAL1 + calDIC[lightItem.ItemCode] * Convert.ToSingle(lightItem.Tb);               
                }

                if (ICP0cal == string.Empty)
                    ICP0cal = "-50";
                
                #endregion
            }
            else//没有轻端数据
            {
                ICP0cal = "-50";
            }
            #endregion

            return ICP0cal;
        }
        /// <summary>
        /// 由ECP补充ICP，第一馏分列为特殊馏分列
        /// </summary>
        private void NarrowGridOilICPLinkSupplement()
        {
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");

            #region
            for (int i = 0; i < this._maxCol; i++)
            {
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                
                if (ICPcal != string.Empty)
                    continue;

                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                #region "ICP补充"
                if (ICPcal == string.Empty)
                {
                    if (i == 0)
                    {
                        #region "i == 0"

                        if (MessageBox.Show("第一窄馏分是否为轻端？", "窄馏分数据补充", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            #region "第一窄馏分是否为轻端？"
                            float fWY = 0;
                            if (float.TryParse(WYcal, out fWY))
                            {
                                ICPcal = ICP0Supplement(this._lightGridOil, fWY);
                            }
                            else
                                ICPcal = "-50";
                            #endregion
                        }
                        else
                        {
                            ICPcal = "-50";
                        }

                        #endregion
                    }
                    else
                    {
                        #region "i != 0"

                        string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i -1);

                        if (ECPcal != string.Empty)
                            ICPcal = ECPcal;

                        #endregion
                    }
                }
                #endregion

                if (ICPcal != string.Empty && ICPcal != "非数字")
                    this._gridOil.SetData("ICP", i, ICPcal);
            }
            #endregion
        }
        /// <summary>
        /// 由ICP补充ECP，尾馏分列为特殊馏分列，需要用户填写
        /// </summary>
        private void NarrowGridOilECPLinkSupplement()
        {
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");

            #region
            for (int i = 0; i < this._maxCol; i++)
            {
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                if (ECPcal == string.Empty)
                {
                    if (i == this._maxCol - 1)
                    {
                        #region i ==this._maxCol -1
                        //ECPlab = "";需手动填写
                        //ECPcal = "";
                        #endregion
                    }
                    else
                    {
                        #region i != this._maxCol -1
                       string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i +1);
                       if (ICPcal != string.Empty)
                        {
                            //ECPlab = ICPlab;
                            ECPcal = ICPcal;
                        }
                        #endregion
                    }
                }
                if (ECPcal != string.Empty)
                    this._gridOil.SetData("ECP", i, ECPcal);
            }
            #endregion
        }
        #endregion
        /// <summary>
        /// 由TWY、VY、TVY补充WY.
        /// </summary>
        private void NarrowGridOilWYLinkSupplement()
        {
            NarrowWYLinkSupplement();            
        }
        /// <summary>
        /// 窄馏分表TWY补充
        /// </summary>
        private void NarrowGridOilTWYLinkSupplement()
        {
            narrowTWYLinkSupplement();
        }
        /// <summary>
        /// 窄馏分表VY补充   VY(i)=WY(i)/D20(i)*D20(CRU)
        /// </summary>
        private void NarrowGridOilVYLinkSupplement()
        {
            VYLinkSupplement();
        }
        /// <summary>
        /// 窄馏分表TVY补充 TVY(i)=TVY(i-1)+VY(i)
        /// </summary>
        private void NarrowGridOilTVYLinkSupplement()
        {
            narrowTVYLinkSupplement();
        }
        /// <summary>
        /// 由D15或API补充D20函数
        /// </summary>
        private void NarrowGridOilD20LinkSupplement()
        {
            D20LinkSupplement();
        }
        /// <summary>
        /// 窄馏分补充D15:D15=f1(D20) 
        /// </summary>
        private void NarrowGridOilD15LinkSupplement()
        {
            D15LinkSupplement();
        }
        /// <summary>
        /// 窄馏分补充D60:D60=f1(D20) API=f2(D20)
        /// </summary>
        private void NarrowGridOilD60LinkSupplement()
        {
            D60LinkSupplement();
        }
        /// <summary>
        /// 窄馏分补充SG
        /// </summary>
        private void NarrowGridOilSGLinkSupplement()
        {
            SGLinkSupplement();
        }
        /// <summary>
        /// 窄馏分补充API: API=f2(D20)
        /// </summary>
        private void NarrowGridOilAPILinkSupplement()
        {
            APILinkSupplement();
        }
        /// <summary>
        /// 窄馏分补充MYD:WYD(i)=WY(i)/(ECP(i)-ICP(i)
        /// </summary>
        private void NarrowGridOilWYDLinkSupplement()
        {
            WYDLinkSupplement();
        }
        /// <summary>
        /// 窄馏分补充MWY:MWY(i)=TWY(i-1)+WY(i)/2
        /// </summary>
        private void NarrowGridOilMWYLinkSupplement()
        {
            MWYLinkSupplement();
        }
        /// <summary>
        /// MCP(i)=(ICP(i)+ECP(i))/2
        /// </summary>
        private void NarrowGridOilMCPLinkSupplement()
        {
            MCPLinkSupplement();
        }
        /// <summary>
        /// 窄馏分补充V02、V04、V05、V08、V10   V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度
        /// </summary>
        private void NarrowGridOilV0LinkSupplement()
        {
            V0_SplineCalSupplement();
            V0_LinkSupplement();

            V0_Condition();
        }

        /// <summary>
        /// V0补充判断条件
        /// </summary>
        private void V0_Condition()
        {
            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempV02 = getCalByItemCode_Column("V02", i);
                float? tempV04 = getCalByItemCode_Column("V04", i);
                float? tempV05 = getCalByItemCode_Column("V05", i);
                float? tempV08 = getCalByItemCode_Column("V08", i);
                float? tempV10 = getCalByItemCode_Column("V10", i);

                float? tempSOP = getCalByItemCode_Column("SOP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);

                if (tempV02 != null && tempSOP != null && tempSOP > 20)
                {
                    this._gridOil.SetData("V02", i, string.Empty);
                }
                if (tempV02 != null && tempSOP == null && tempECP != null && tempECP >= 400)
                {
                    this._gridOil.SetData("V02", i, string.Empty);
                }

                if (tempV04 != null && tempSOP != null && tempSOP > 40)
                {
                    this._gridOil.SetData("V04", i, string.Empty);
                }

                if (tempV05 != null && tempSOP != null && tempSOP > 50)
                {
                    this._gridOil.SetData("V05", i, string.Empty);
                }

                if (tempV08 != null && tempSOP != null && tempSOP > 80)
                {
                    this._gridOil.SetData("V08", i, string.Empty);
                }
                if (tempV08 != null && tempSOP == null && tempICP != null && tempICP < 300)
                {
                    this._gridOil.SetData("V08", i, string.Empty);
                }

                if (tempV10 != null && tempSOP != null && tempSOP > 100)
                {
                    this._gridOil.SetData("V10", i, string.Empty);
                }
                if (tempV10 != null && tempSOP == null && tempICP != null && tempICP < 300)
                {
                    this._gridOil.SetData("V10", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// ACD补充
        /// </summary>
        private void NarrowGridOilACDLinkSupplement()
        {
           GridOilACDLinkSupplement();

        }
        /// <summary>
        /// ACD补充
        /// </summary>
        private void WideGridOilACDLinkSupplement()
        {
            GridOilACDLinkSupplement();

        }
        /// <summary>
        /// 窄馏分补充ACD ::IF ACD.CLA!=Null THEN NET=FUN(ACD)
        /// </summary>
        private void NarrowGridOilNETLinkSupplement()
        {
            NETLinkSupplement();
            //NETSplineCalSupplement();
            narrowSplineCalSupplement("NET");
            #region 补充判断条件//20161208新增，解决插值补充数据为负值的问题
            for (int i = 1; i < this._maxCol - 1; i++)
            {
                float? medValue = getCalByItemCode_Column("NET", i);
                float? beforeValue = getCalByItemCode_Column("NET", i - 1);
                float? afterValue = getCalByItemCode_Column("NET", i + 1);

                if (medValue != null && beforeValue != null && afterValue != null && medValue < 0)
                {
                    float? value = (beforeValue + afterValue) / 2;
                    if (value > 0)
                        this._gridOil.SetData("NET", i, value.ToString());
                    else
                        this._gridOil.SetData("NET", i, string.Empty);

                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分的SUL插值补充
        /// </summary>
        private void NarrowGridOilSULLinkSupplement()
        {
            //SULSplineCalSupplement(); 
            narrowSplineCalSupplement("SUL");

            #region 补充判断条件//20161208新增，解决插值补充数据为负值的问题
            for (int i = 1; i < this._maxCol - 1; i++)
            {
                float? medValue = getCalByItemCode_Column("SUL", i);
                float? beforeValue = getCalByItemCode_Column("SUL", i - 1);
                float? afterValue = getCalByItemCode_Column("SUL", i + 1);

                if (medValue != null && beforeValue != null && afterValue != null && medValue < 0)
                {
                    float? value = (beforeValue + afterValue) / 2;
                    if (value > 0)
                        this._gridOil.SetData("SUL", i, value.ToString());
                    else
                        this._gridOil.SetData("SUL", i, string.Empty);

                }
            }
            #endregion
        }

        /// <summary>
        /// 窄馏分的N2插值补充
        /// </summary>
        private void NarrowGridOilN2LinkSupplement()
        {
            //N2SplineCalSupplement();
            narrowSplineCalSupplement("N2");

            #region 补充判断条件//20161208新增，解决插值补充数据为负值的问题
            for (int i = 1; i < this._maxCol-1; i++)
            {
                float? medValue = getCalByItemCode_Column("N2", i);
                float? beforeValue = getCalByItemCode_Column("N2", i-1);
                float? afterValue = getCalByItemCode_Column("N2", i+1);

                if (medValue != null && beforeValue != null && afterValue != null && medValue < 0)
                {
                    float? value=(beforeValue+afterValue )/2;
                    if(value>0)
                        this._gridOil.SetData("N2", i, value.ToString() );
                    else
                        this._gridOil.SetData("N2", i, string.Empty);

                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分的CHR插值补充
        /// </summary>
        private void NarrowGridOilCHRLinkSupplement()
        {
           // CHRSplineCalSupplement();
            narrowSplineCalSupplement("CHR");

            #region 补充判断条件//20161208新增，解决插值补充数据为负值的问题
            for (int i = 1; i < this._maxCol - 1; i++)
            {
                float? medValue = getCalByItemCode_Column("CHR", i);
                float? beforeValue = getCalByItemCode_Column("CHR", i - 1);
                float? afterValue = getCalByItemCode_Column("CHR", i + 1);

                if (medValue != null && beforeValue != null && afterValue != null && medValue < 0)
                {
                    float? value = (beforeValue + afterValue) / 2;
                    if (value > 0)
                        this._gridOil.SetData("CHR", i, value.ToString());
                    else
                        this._gridOil.SetData("CHR", i, string.Empty);

                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分的MEC插值补充
        /// </summary>
        private void NarrowGridOilMECLinkSupplement()
        {
            //MECSplineCalSupplement();
            narrowSplineCalSupplement("MEC");

            #region 补充判断条件//20161208新增，解决插值补充数据为负值的问题
            for (int i = 1; i < this._maxCol - 1; i++)
            {
                float? medValue = getCalByItemCode_Column("MEC", i);
                float? beforeValue = getCalByItemCode_Column("MEC", i - 1);
                float? afterValue = getCalByItemCode_Column("MEC", i + 1);

                if (medValue != null && beforeValue != null && afterValue != null && medValue < 0)
                {
                    float? value = (beforeValue + afterValue) / 2;
                    if (value > 0)
                        this._gridOil.SetData("MEC", i, value.ToString());
                    else
                        this._gridOil.SetData("MEC", i, string.Empty);

                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分的BAN插值补充
        /// </summary>
        private void NarrowGridOilBANLinkSupplement()
        {
            //BANSplineCalSupplement();
            narrowSplineCalSupplement("BAN");

            #region 补充判断条件//20161208新增，解决插值补充数据为负值的问题
            for (int i = 1; i < this._maxCol - 1; i++)
            {
                float? medValue = getCalByItemCode_Column("BAN", i);
                float? beforeValue = getCalByItemCode_Column("BAN", i - 1);
                float? afterValue = getCalByItemCode_Column("BAN", i + 1);

                if (medValue != null && beforeValue != null && afterValue != null && medValue < 0)
                {
                    float? value = (beforeValue + afterValue) / 2;
                    if (value > 0)
                        this._gridOil.SetData("BAN", i, value.ToString());
                    else
                        this._gridOil.SetData("BAN", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分的RVP插值补充
        /// </summary>
        private void NarrowGridOilRVPLinkSupplement()
        {
            //RVPSplineCalSupplement();
            narrowSplineCalSupplement("RVP");
            RVP_NarrowLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempRVP = getCalByItemCode_Column("RVP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempRVP != null && tempECP != null && tempECP > 200)
                {
                    this._gridOil.SetData("RVP", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分的SMK插值补充
        /// </summary>
        private void NarrowGridOilSMKLinkSupplement()
        {
            narrowSplineCalSupplement("SMK");
            //SMKSplineCalSupplement();
            SMKLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempSMK = getCalByItemCode_Column("SMK", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);


                if (tempSMK != null && tempICP != null && tempICP < 120)
                {
                    this._gridOil.SetData("SMK", i, string.Empty);
                    tempSMK = null;
                }
                if (tempSMK != null && tempECP != null && tempECP > 250)
                {
                    this._gridOil.SetData("SMK", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分的FRZ插值补充
        /// </summary>
        private void NarrowGridOilFRZLinkSupplement()
        {
            narrowSplineCalSupplement("FRZ");
            //FRZSplineCalSupplement();
            FRZLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempFRZ = getCalByItemCode_Column("FRZ", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);


                if (tempFRZ != null && tempICP != null && tempICP < 120)
                {
                    this._gridOil.SetData("FRZ", i, string.Empty);
                    tempFRZ = null;
                }
                if (tempFRZ != null && tempECP != null && tempECP > 250)
                {
                    this._gridOil.SetData("FRZ", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充POR=SOP+3
        /// </summary>
        private void NarrowGridOilPORLinkSupplement()
        {
            narrowSplineCalSupplement("POR");
            //PORSplineCalSupplement();
            PORLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempPOR = getCalByItemCode_Column("POR", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);


                if (tempPOR != null && tempICP != null && tempICP < 160)
                {
                    this._gridOil.SetData("POR", i, string.Empty);
                    tempPOR = null;
                }
                if (tempPOR != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("POR", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充SOP 
        /// </summary>
        private void NarrowGridOilSOPLinkSupplement()
        {
            narrowSplineCalSupplement("SOP");
            //SOPSplineCalSupplement();
            SOPLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempSOP = getCalByItemCode_Column("SOP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);


                if (tempSOP != null && tempICP != null && tempICP < 160)
                {
                    this._gridOil.SetData("SOP", i, string.Empty);
                    tempSOP = null;
                }
                if (tempSOP != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("SOP", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充CFP
        /// </summary>
        private void NarrowGridOilCFPLinkSupplement()
        {
            narrowSplineCalSupplement("CFP");
            //CFPSplineCalSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempCFP = getCalByItemCode_Column("CFP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);

                if (tempCFP != null && tempICP != null && tempICP < 160)
                {
                    this._gridOil.SetData("CFP", i, string.Empty);
                    tempCFP = null;
                }
                if (tempCFP != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("CFP", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充SAV_ARV
        /// </summary>
        private void NarrowGridOilSAV_ARVLinkSupplement()
        {
            narrowSplineCalSupplement("SAV");
            narrowSplineCalSupplement("ARV");
            //SAVSplineCalSupplement();
            //ARVSplineCalSupplement();
            SAV_ARVLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempSAV = getCalByItemCode_Column("SAV", i);
                float? tempARV = getCalByItemCode_Column("ARV", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);

                if (tempSAV != null && tempICP != null && tempICP < 120)
                {
                    this._gridOil.SetData("SAV", i, string.Empty);
                    tempSAV = null;
                }
                if (tempSAV != null && tempECP != null && tempECP > 250)
                {
                    this._gridOil.SetData("SAV", i, string.Empty);
                }

                if (tempARV != null && tempICP != null && tempICP < 120)
                {
                    this._gridOil.SetData("ARV", i, string.Empty);
                    tempARV = null;
                }
                if (tempARV != null && tempECP != null && tempECP > 250)
                {
                    this._gridOil.SetData("ARV", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充ANI
        /// </summary>
        private void NarrowGridOilANILinkSupplement()
        {
            narrowSplineCalSupplement("ANI");
            //ANISplineCalSupplement();
            ANILinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempANI = getCalByItemCode_Column("ANI", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);

                if (tempANI != null && tempICP != null && tempICP < 120)
                {
                    this._gridOil.SetData("ANI", i, string.Empty);
                    tempANI = null;
                }
                if (tempANI != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("ANI", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充R20:: R20=f6(D70)
        /// </summary>
        private void NarrowGridOilR20LinkSupplement()
        {
            //R20LinkSupplement();
            //R20SplineCalSupplement();
            narrowSplineCalSupplement("R20");

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempR20 = getCalByItemCode_Column("R20", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempSOP = getCalByItemCode_Column("SOP", i);

                if (tempR20 != null && tempSOP != null && tempSOP > 20)
                {
                    this._gridOil.SetData("R20", i, string.Empty);
                }
                if (tempR20 != null && tempSOP == null && tempECP != null)
                {
                    if (tempECP > 350)
                    {
                        this._gridOil.SetData("R20", i, string.Empty);
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充R70:: R70=f7(D20)
        /// </summary>
        private void NarrowGridOilR70LinkSupplement()
        {
            //R70LinkSupplement();
            //R70SplineCalSupplement();
            narrowSplineCalSupplement("R70");

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempR70 = getCalByItemCode_Column("R70", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempSOP = getCalByItemCode_Column("SOP", i);

                if (tempR70 != null )

                {
                    if (tempECP <=350)
                    {
                        this._gridOil.SetData("R70", i, string.Empty);
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充KRC::KRC=f8(API,Tmed)) Tmed=(ICP+ECP)/2
        /// </summary>
        private void NarrowGridOilKFCLinkSupplement()
        {
            KFCLinkSupplement();
        }
        /// <summary>
        /// 窄馏分补充BMI
        /// </summary>
        private void NarrowGridOilBMILinkSupplement()
        {
            BMI_NarrowLinkSupplement();

            //#region 补充判断条件
            //for (int i = 0; i < this._maxCol; i++)
            //{
            //    float? tempBMI = getCalByItemCode_Column("BMI", i);
            //    float? tempECP = getCalByItemCode_Column("ECP", i);

            //    if (tempBMI != null && tempECP != null && tempECP > 200)
            //    {
            //        this._gridOil.SetData("BMI", i, string.Empty);
            //    }
            //}
            //#endregion
        }
        /// <summary>
        /// 窄馏分补充DI
        /// </summary>
        private void NarrowGridOilDILinkSupplement()
        {
            DILinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempDI = getCalByItemCode_Column("DI", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);

                if (tempDI != null && tempICP != null && tempICP < 140)
                {
                    this._gridOil.SetData("DI", i, string.Empty);
                }
                if (tempDI != null && tempECP != null && tempECP > 400)
                {
                    this._gridOil.SetData("DI", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充CI
        /// </summary>
        private void NarrowGridOilCILinkSupplement()
        {
            CI_NarrowLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempCI = getCalByItemCode_Column("CI", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);

                if (tempCI != null && tempICP != null && tempICP < 140)
                {
                    this._gridOil.SetData("CI", i, string.Empty);
                }
                if (tempCI != null && tempECP != null && tempECP > 400)
                {
                    this._gridOil.SetData("CI", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充VG4
        /// </summary>
        private void NarrowGridOilVG4LinkSupplement()
        {
            VG4LinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempVG4 = getCalByItemCode_Column("VG4", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);

                if (tempVG4 != null && tempICP != null && tempICP < 140)
                {
                    this._gridOil.SetData("VG4", i, string.Empty);
                }
                if (tempVG4 != null && tempECP != null && tempECP > 400)
                {
                    this._gridOil.SetData("VG4", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 窄馏分补充V1G
        /// </summary>
        private void NarrowGridOilV1GLinkSupplement()
        {
            V1GLinkSupplement();
            Comm_Judge_Condition("V1G");
        }
        /// <summary>
        /// 窄馏分补充MW
        /// </summary>
        private void NarrowGridOilMWLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");

            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");

            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");

            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MWcal = getStrValuefromOilDataEntity(MWOilDataList,i);
             
                /*D15,V10 ->V1G*/
                #region
                if (MWcal != string.Empty)
                    continue;

                if (MWcal == string.Empty)
                {
                    string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    MWcal = BaseFunction.FunMWfromMCP_SG(MCPcal, SGcal);
                }
                if (MWcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    MWcal = BaseFunction.FunMWfromICP_ECP_D20(ICPcal, ECPcal, D20cal);
                }
                if (MWcal == string.Empty)
                {
                    string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    MWcal = BaseFunction.FunMWfromD20_V04_V10(D20cal, V04cal, V10cal);
                }
                if (MWcal != string.Empty && MWcal != "非数字")
                    this._gridOil.SetData("MW", i, MWcal);
                #endregion
            }
        }

        #endregion

        #region "宽馏分"

       
        //private void WideGridOilICPinkSupplement()
        //{
        //    List<OilDataEntity> wideICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
        //    List<OilDataEntity> narrowICPOilDataList = this._narrowGridOil.GetDataByRowItemCode("ICP");

        //    string wideICPcal = getStrValuefromOilDataEntity(wideICPOilDataList, 0);
        //    string narrowICPcal = getStrValuefromOilDataEntity(narrowICPOilDataList, 0);

        //    if (wideICPcal == string.Empty || wideICPcal == null)
        //    {
        //        wideICPcal = narrowICPcal;
        //    }

        //    if (wideICPcal != string.Empty && wideICPcal != "非数字")
        //        this._gridOil.SetData("ICP", 0, wideICPcal);
        //}
        /// <summary>
        ///  /// <summary>
        /// 宽馏分的ICP为空时，从窄馏分的第一列取值(2012.10.19增加)
        /// </summary>
        /// </summary>
        private void WideGridOilICPinkSupplement()
        {
            for (int i = 0; i < this._maxCol; i++)
            {
                List<OilDataEntity> wideICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
                List<OilDataEntity> narrowICPOilDataList = this._narrowGridOil.GetDataByRowItemCode("ICP");

                string wideICPcal = getStrValuefromOilDataEntity(wideICPOilDataList, i);
                string narrowICPcal = getStrValuefromOilDataEntity(narrowICPOilDataList, 0);

                if (wideICPcal == string.Empty || wideICPcal == null)
                {
                    wideICPcal = narrowICPcal;
                }

                if (wideICPcal != string.Empty && wideICPcal != "非数字")
                    this._gridOil.SetData("ICP", i, wideICPcal);
            }
        }

        /// <summary>
        /// 宽馏分的WY关联补充
        /// </summary>
        private void WideGridOilWYLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY");//累积补充           
            WYLinkSupplement();//关联补充             
        }
        ///// <summary>
        ///// 宽馏分的TWY关联补充
        ///// </summary>
        //private void WideGridOilTWYLinkSupplement()
        //{
        //    #region "混合补充"
        //    List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");

        //    List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
        //    List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
        //    List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
        //    for (int i = 0; i < this._maxCol; i++)//宽馏分
        //    {
        //        string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);
        //        if (TWYcal != string.Empty)
        //            continue;

        //         string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
        //         string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

        //         if (TWYcal == string.Empty)
        //             TWYcal = FunNarrowStartEndReturnEndValue(ICPcal, ECPcal, "TWY");

        //        if (TWYcal == string.Empty)
        //        {
        //            TWYcal = FunNarrowStartEndReturnStartValue(ICPcal, "TWY");
        //            string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
        //            List<string> list = new List<string> ();list.Add (TWYcal);list.Add (WYcal);
        //            TWYcal = BaseFunction.FunSumAllowEmpty(list);
        //        }
        //        if (TWYcal != string.Empty && TWYcal != "非数字")
        //            this._gridOil.SetData("TWY", i, TWYcal);
        //    }
        //    #endregion
        //}
        
        /// <summary>
        /// 宽馏分的TWY关联补充
        /// </summary>
        private void WideGridOilTWYLinkSupplement()
        {
            #region "混合补充"
            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);
                if (TWYcal != string.Empty)
                    continue;

                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                if (TWYcal == string.Empty)
                    TWYcal = FunWideValueFromNarrowECPValue(ECPcal, "TWY");

                if (TWYcal != string.Empty && TWYcal != "非数字")
                    this._gridOil.SetData("TWY", i, TWYcal);
            }
            #endregion

            wideTWYLinkSupplement();
        }
        /// <summary>
        /// 通过宽馏分的ECP在窄馏分中查找对应的ECP列,并且返回指定ECP列的物性值
        /// </summary>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定ECP列的物性值</returns>
        public string FunWideValueFromNarrowECPValue(string strECP, string strItemCode)
        {
            string strResult = string.Empty;

            if (strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return strResult;
            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData();

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return strResult;

            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calData == strECP).FirstOrDefault();
            List<OilDataEntity> oilDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode && o.calShowData != string.Empty).ToList();

            if (oilDataECP == null || oilDatas == null)//如果查找的数据不存在则返回空
                return strResult;

            var temp = oilDatas.Where(o => o.OilTableCol.colOrder == oilDataECP.OilTableCol.colOrder).FirstOrDefault();
            if (temp != null)
                strResult = temp.calData;

            return strResult;
        }

        /// <summary>
        /// 宽馏分TWY补充—*TWY(i)=NCUTS(TWY(ECP))+WY(i)*/
        /// </summary>
        private void wideTWYLinkSupplement()
        {
            for (int i = 0; i < this._maxCol; i++)
            {
                List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
                List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
                List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
                string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);

                if (TWYcal != string.Empty)
                    continue;

                /*TWY(i)=NCUTS(TWY(ECP))+WY(i)*/
                #region "数据补充"

                if (i == 0)
                {
                    if (TWYcal == string.Empty)
                    {
                        string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                        if (WYcal != "0")
                        {
                            TWYcal = WYcal;
                        }
                    }
                }
                else
                {
                    if (TWYcal == string.Empty)
                    {
                        string strNarrowTWY = string.Empty;//存放[NCUTS（ECP）=WCUTS（ICP）].Value
                        string strICP = getStrValuefromOilDataEntity(ICPOilDataList, i);//获取宽馏分中对应的ICP值
                        string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);//获取宽馏分中对应的WY值

                        List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData();//获取窄馏分中的数据

                        if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                            strNarrowTWY = string.Empty;
                        else
                        {

                            //var narrowOilDataECPlist = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP").ToList();

                            OilDataEntity narrowOilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strICP).FirstOrDefault();
                            List<OilDataEntity> narrowOilDataTWYList = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "TWY").ToList();
                            if (narrowOilDataECP != null)
                            {
                                var temp = narrowOilDataTWYList.Where(o => o.OilTableCol.colOrder == narrowOilDataECP.OilTableCol.colOrder).FirstOrDefault();
                                if (temp != null)
                                    strNarrowTWY = temp.calData;
                            }
                        }

                        TWYcal = BaseFunction.FunTWY(strNarrowTWY, WYcal); ;
                    }
                }

                #endregion

                if (TWYcal != string.Empty && TWYcal != "非数字")
                    this._gridOil.SetData("TWY", i, TWYcal);
            }
        }


        /// <summary>
        /// 宽馏分的VY关联补充
        /// </summary>
        private void WideGridOilVYLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("VY");//累积补充  
            VYLinkSupplement();
        }
        ///// <summary>
        ///// 宽馏分的TVY关联补充
        ///// </summary>
        //private void WideGridOilTVYLinkSupplement()
        //{
        //    List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");
            
        //    List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
        //    List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
        //    List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

        //    #region
        //    for (int i = 0; i < this._maxCol; i++)//宽馏分
        //    {
        //        string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);

        //        if (TVYcal != string.Empty)
        //            continue;
        //        string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
        //        string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

        //        if (TVYcal == string.Empty)                    
        //            TVYcal = FunNarrowStartEndReturnEndValue(ICPcal, ECPcal, "TVY");

        //        if (TVYcal == string.Empty)
        //        {
        //            TVYcal = FunNarrowStartEndReturnStartValue(ICPcal, "TVY");
        //            string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
        //            List<string> list = new List<string>(); list.Add(TVYcal); list.Add(VYcal);
        //            TVYcal = BaseFunction.FunSumAllowEmpty(list);
        //        }
        //        if (TVYcal != string.Empty && TVYcal != "非数字")
        //            this._gridOil.SetData("TVY", i, TVYcal);
        //    }
        //    #endregion

        //    TVYLinkSupplement();
        //}


        // <summary>
        /// 宽馏分的TVY补充
        /// </summary>
        private void WideGridOilTVYLinkSupplement()
        {
            List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            #region
            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);

                if (TVYcal != string.Empty)
                    continue;
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                if (TVYcal == string.Empty)
                    TVYcal = FunWideValueFromNarrowECPValue(ECPcal, "TVY");

                if (TVYcal != string.Empty && TVYcal != "非数字")
                    this._gridOil.SetData("TVY", i, TVYcal);
            }
            #endregion

            wideTVYLinkSupplement();
        }
        /// <summary>
        /// 宽馏分TVY补充—TVY(i)=[NCUTS(ECP)=WCUTS(ICP)].TVY+VY(i)
        /// </summary>
        private void wideTVYLinkSupplement()
        {
            for (int i = 0; i < this._maxCol; i++)
            {
                List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");
                List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
                List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
                string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);

                if (TVYcal != string.Empty)
                    continue;

                /*TVY(i)=[NCUTS(ECP)=WCUTS(ICP)].TVY+VY(i)*/
                #region "数据补充"
                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                //if (i == 0)
                //{
                //    if (TVYcal == string.Empty && VYcal != string.Empty)
                //    {
                //        TVYcal = VYcal;
                //    }
                //}
                //else
                //{
                    string strICP = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string strNarrowTVY = string.Empty;
                    List<OilDataEntity> narrowOilData = this._narrowGridOil.GetAllData();
                    if (narrowOilData == null)
                    {
                        strNarrowTVY = string.Empty;
                    }
                    else
                    {
                        OilDataEntity narrowOilDataECP = narrowOilData.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strICP).FirstOrDefault();
                        if (narrowOilDataECP == null)
                            narrowOilDataECP = narrowOilData.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();

                        List<OilDataEntity> narrowOilDataTVYList = narrowOilData.Where(o => o.OilTableRow.itemCode == "TVY").ToList();
                        if (narrowOilDataECP == null || narrowOilDataTVYList == null)
                            continue;
                        var temp = narrowOilDataTVYList.Where(o => o.OilTableCol.colOrder == narrowOilDataECP.OilTableCol.colOrder).FirstOrDefault();
                        if (temp != null)
                        {
                            strNarrowTVY = temp.calData;
                            TVYcal = BaseFunction.FunTVY(strNarrowTVY, VYcal);
                        }
                    }
                //}
                #endregion

                if (TVYcal != string.Empty && TVYcal != "非数字")
                    this._gridOil.SetData("TVY", i, TVYcal);
            }
        }

        /// <summary>
        /// 宽馏分的API关联补充
        /// </summary>
        private void WideGridOilAPILinkSupplement()
        {
            APILinkSupplement();
        }
        /// <summary>
        /// 宽馏分的D20关联补充
        /// </summary>
        private void WideGridOilD20LinkSupplement()
        {
            WholeGridOilD20LinkSupplement();
            WideGridOilMixSupplementAccumulate("WY", "D20", "D20");           
            //D20LinkSupplement();
        }
        /// <summary>
        /// 宽馏分的D60关联补充
        /// </summary>
        private void WideGridOilD60LinkSupplement()
        {
            D60LinkSupplement();
            WideGridOilMixSupplementAccumulate("WY", "D60", "D60");           
        }
        /// <summary>
        /// 宽馏分的D15关联补充
        /// </summary>
        private void WideGridOilD15LinkSupplement()
        {
            D15LinkSupplement();
            WideGridOilMixSupplementAccumulate("WY", "D15", "D15");          
        }
        /// <summary>
        /// 宽馏分的D70关联补充
        /// </summary>
        private void WideGridOilD70LinkSupplement()
        {
            D70_WideLinkSupplement();
            WideGridOilMixSupplementAccumulate("WY", "D70", "D70");

        }
        /// <summary>
        /// 宽馏分的SG关联补充
        /// </summary>
        private void WideGridOilSGLinkSupplement()
        {
            SGLinkSupplement();
        }
        /// <summary>
        /// 宽馏分的WYD关联补充
        /// </summary>
        private void WideGridOilWYDLinkSupplement()
        {
            WYDLinkSupplement();
        }
        /// <summary>
        ///  宽馏分的MWY关联补充
        /// </summary>
        private void WideGridOilMWYLinkSupplement()
        {
            #region "混合补充"
            List<OilDataEntity> MWYOilDataList = this._gridOil.GetDataByRowItemCode("MWY");

            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");

            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string MWYcal = getStrValuefromOilDataEntity(MWYOilDataList, i);

                if (MWYcal != string.Empty)
                    continue;

                if (MWYcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                    string TWYcal = FunNarrowStartEndReturnStartValue(ICPcal, "TWY");
                    
                    if (TWYcal == string.Empty)
                        continue;    

                    string WYal = getStrValuefromOilDataEntity(WYOilDataList, i);                  
                     
                    if (WYal == string.Empty )
                        continue;
                   

                    float WY = 0, TWY = 0;

                    if (float.TryParse(WYal, out WY) && float.TryParse(TWYcal, out TWY))
                    {
                        MWYcal = (WY / 2.0 + TWY).ToString();
                    }
                }

                if (MWYcal != string.Empty && MWYcal != "非数字")
                    this._gridOil.SetData("MWY", i, MWYcal);
            }

            #endregion
        }
        /// <summary>
        ///  宽馏分的MCP关联补充
        /// </summary>
        private void WideGridOilMCPLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");

            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> MWYOilDataList = this._gridOil.GetDataByRowItemCode("MWY");

            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData ();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return;

            List<OilDataEntity> oilDataECPList = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP").ToList();
            List<OilDataEntity> oilDataTWYList = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "TWY").ToList();

            if (oilDataECPList == null || oilDataTWYList == null)//如果不存在则返回空
                return;

            if (oilDataECPList.Count <= 0 || oilDataTWYList.Count <= 0)//如果不存在则返回空
                return;

            #region "混合补充"

            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                if (MCPcal != string.Empty)
                    continue;

                if (MCPcal == string.Empty)
                {
                    string MWYcal = getStrValuefromOilDataEntity(MWYOilDataList, i);
                    MCPcal = BaseFunction.SplineCal(oilDataTWYList, oilDataECPList, MWYcal);
                }

                if (MCPcal != string.Empty && MCPcal != "非数字")
                    this._gridOil.SetData("MCP", i, MCPcal);
            }

            #endregion
        }
        /// <summary>
        /// 宽馏分补充V-2、 V02、V04、V05、V08、V10   V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度
        /// </summary>
        private void WideGridOilV0LinkSupplement()
        {
            V0_LinkSupplement();

            WideGridOilMixSupplementAccumulate("WY", "V02", "V02");
            WideGridOilMixSupplementAccumulate("WY", "V04", "V04");
            WideGridOilMixSupplementAccumulate("WY", "V05", "V05");
            WideGridOilMixSupplementAccumulate("WY", "V08", "V08");
            WideGridOilMixSupplementAccumulate("WY", "V10", "V10");

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempV02 = getCalByItemCode_Column("V02", i);
                float? tempV04 = getCalByItemCode_Column("V04", i);
                float? tempV05 = getCalByItemCode_Column("V05", i);
                float? tempV08 = getCalByItemCode_Column("V08", i);
                float? tempV10 = getCalByItemCode_Column("V10", i);

                float? tempSOP = getCalByItemCode_Column("SOP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);
                float? tempICP = getCalByItemCode_Column("ICP", i);

                if (tempV02 != null && tempSOP != null && tempSOP > 20)
                {
                    this._gridOil.SetData("V02", i, string.Empty);
                }
                if (tempV02 != null && tempSOP == null && tempECP !=null && tempECP >= 400)
                {
                    this._gridOil.SetData("V02", i, string.Empty);
                }

                if (tempV04 != null && tempSOP != null && tempSOP > 50)
                {
                    this._gridOil.SetData("V04", i, string.Empty);
                }

                if (tempV05 != null && tempSOP != null && tempSOP > 80)
                {
                    this._gridOil.SetData("V05", i, string.Empty);
                }
                if (tempV05 != null && tempSOP == null && tempICP !=null && tempICP < 300)
                {
                    this._gridOil.SetData("V05", i, string.Empty);
                }

                if (tempV08 != null && tempSOP != null && tempSOP > 80)
                {
                    this._gridOil.SetData("V08", i, string.Empty);
                }
                if (tempV08 != null && tempSOP == null && tempICP !=null && tempICP < 300)
                {
                    this._gridOil.SetData("V08", i, string.Empty);
                }

                if (tempV10 != null && tempSOP != null && tempSOP > 100)
                {
                    this._gridOil.SetData("V10", i, string.Empty);
                }
                if (tempV10 != null && tempSOP == null && tempICP !=null && tempICP < 300)
                {
                    this._gridOil.SetData("V10", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的VI关联补充V04,V10->VI
        /// </summary>
        private void WideGridOilVILinkSupplement()
        {
            VI_WideLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempVI = getCalByItemCode_Column("VI", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);


                if (tempVI != null && tempICP != null && tempICP < 300)
                {
                    this._gridOil.SetData("VI", i, string.Empty);
                    tempVI = null;
                }
                if (tempVI != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("VI", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的VG4关联补充D15,V04->VG4
        /// </summary>
        private void WideGridOilVG4LinkSupplement()
        {
            VG4LinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempVG4 = getCalByItemCode_Column("VG4", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);


                if (tempVG4 != null && tempICP != null && tempICP < 140)
                {
                    this._gridOil.SetData("VG4", i, string.Empty);
                    tempVG4 = null;
                }
                if (tempVG4 != null && tempECP != null && tempECP > 400)
                {
                    this._gridOil.SetData("VG4", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的V1G关联补充D15,V10->V1G
        /// </summary>
        private void WideGridOilV1GLinkSupplement()
        {
            V1GLinkSupplement();

            Comm_Judge_Condition("V1G");
        }
        /// <summary>
        /// 宽馏分的R20关联补充
        /// </summary>
        private void WideGridOilR20LinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "R20", "R20");

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempR20 = getCalByItemCode_Column("R20", i);

                float? tempSOP = getCalByItemCode_Column("SOP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempR20 != null && tempSOP != null && tempSOP > 20)
                {
                    this._gridOil.SetData("R20", i, string.Empty);
                }
                if (tempR20 != null && tempSOP == null && tempECP !=null && tempECP > 350)
                {
                    this._gridOil.SetData("R20", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的R70关联补充
        /// </summary>
        private void WideGridOilR70LinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "R70", "R70");

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempR70 = getCalByItemCode_Column("R70", i);

                float? tempSOP = getCalByItemCode_Column("SOP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempR70 != null && tempSOP != null && tempSOP > 70)
                {
                    this._gridOil.SetData("R70", i, string.Empty);
                }
                if (tempR70 != null && tempSOP == null && tempECP !=null && tempECP < 300)
                {
                    this._gridOil.SetData("R70", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        ///宽馏分的C/H关联补充C/H=CAR/H2
        /// </summary>
        private void WideGridOilC_HLinkSupplement()
        {
            C_H_WideLinkSupplement();
        }
        /// <summary>
        ///宽馏分的SUL=FUN(WY(from i to j), SUL(from i to j))
        /// </summary>
        private void WideGridOilSULLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "SUL", "SUL");
        }
        /// <summary>
        ///宽馏分的N2=FUN(WY(from i to j), N2(from i to j))
        /// </summary>
        private void WideGridOilN2LinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "N2", "N2");
        }
        /// <summary>
        ///宽馏分的BAN=FUN(WY(from i to j),BAN(from i to j))
        /// </summary>
        private void WideGridOilBANLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "BAN", "BAN");
        }
        /// <summary>
        ///宽馏分的MEC=FUN(WY(from i to j), MEC(from i to j))
        /// </summary>
        private void WideGridOilMECLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "MEC", "MEC");
        }
        /// <summary>
        ///宽馏分的ACD,D20=>NET
        /// </summary>
        private void WideGridOilNETLinkSupplement()
        {
            NETLinkSupplement();
            WideGridOilMixSupplementAccumulate("WY", "NET", "NET");
        }
        /// <summary>
        ///宽馏分的 NET,D20=>ACD
        /// </summary>
        private void GridOilACDLinkSupplement()
        {
            List<OilDataEntity> ACDOilDataList = this._gridOil.GetDataByRowItemCode("ACD");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> NETOilDataList = this._gridOil.GetDataByRowItemCode("NET");

            #region "混合补充"

            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string ACDcal = getStrValuefromOilDataEntity(ACDOilDataList, i);
                if (ACDcal != string.Empty)
                    continue;

                if (ACDcal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    string NETcal = getStrValuefromOilDataEntity(NETOilDataList, i);

                    ACDcal = BaseFunction.FunACD(NETcal, D20cal);
                }

                if (ACDcal != string.Empty && ACDcal != "非数字")
                    this._gridOil.SetData("ACD", i, ACDcal);
            }
            #endregion
        }



        /// <summary>
        /// 宽馏分的MW
        /// </summary>
        private void WideGridOilMWLinkSupplement()
        {
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");

            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);

                #region "MW赋值"
                if (MWcal != string.Empty)
                    continue;
                
                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                if (MWcal == string.Empty)
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);
                    MWcal = BaseFunction.FunMWfromA10_A30_A50_A70_A90_D20(A10cal, A30cal, A50cal, A70cal, A90cal, D20cal);
                }

                if (MWcal == string.Empty)
                {
                    string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    MWcal = BaseFunction.FunMWfromMCP_SG(MCPcal, SGcal);
                }

                string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);

                if (MWcal == string.Empty)
                {
                    string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                    MWcal = BaseFunction.FunMWfromD20_V04_V10(D20cal, V04cal, V10cal);
                }
                if (MWcal == string.Empty)
                {
                    string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                    MWcal = BaseFunction.FunMWfromD20_V08_V10(D20cal, V08cal, V10cal);
                }
                if (MWcal != string.Empty && MWcal!="非数字")
                    this._gridOil.SetData("MW", i, MWcal);
                #endregion
            }
        }
        /// <summary>
        /// 馏程算法,TVY-ECP曲线=>AIP, A10, A30, A50,A70,A90,AEP
        /// </summary>
        private void WideGridOilA_PLinkSupplement()
        {
            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData().Where(o => o.calData != string.Empty).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return;

            List<OilDataEntity> oilDataECPList = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP").ToList();
            List<OilDataEntity> oilDataTVYList = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "TVY").ToList();

            if (oilDataECPList == null || oilDataTVYList == null)//如果不存在则返回空
                return;

            if (oilDataECPList.Count <= 0 || oilDataTVYList.Count <= 0)//如果不存在则返回空
                return;
           
            Dictionary<string,string> ECP_TVYDIC = new Dictionary<string,string> ();
            foreach (OilDataEntity oilDataECP in oilDataECPList)
            {
                var oilDataTVY = oilDataTVYList.Where(o => o.OilTableCol.colCode == oilDataECP.OilTableCol.colCode).FirstOrDefault();

                if (oilDataTVY == null)
                    continue;
                else
                {
                    if (!ECP_TVYDIC.Keys.Contains (oilDataECP.calData))
                        ECP_TVYDIC.Add (oilDataECP.calData ,oilDataTVY.calData);                  
                }
            }
            List<OilDataEntity> AIPOilDataList = this._gridOil.GetDataByRowItemCode("AIP");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            List<OilDataEntity> A95OilDataList = this._gridOil.GetDataByRowItemCode("A95");
            List<OilDataEntity> AEPOilDataList = this._gridOil.GetDataByRowItemCode("AEP");

            List<OilDataEntity> KFCOilDataList = this._gridOil.GetDataByRowItemCode("KFC");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            #region "混合补充"
            Dictionary<string, float?> Dic = new Dictionary<string, float?>();
            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string KFCcal = getStrValuefromOilDataEntity(KFCOilDataList, i);
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                float ICP = 0, ECP = 0;
                if (float.TryParse(ICPcal, out ICP) && float.TryParse(ECPcal, out ECP))
                {
                    Dic = BaseFunction.FunAIP_A10_A30_A50_A70_A90_A95_AEPfromCurveEntityECP_TVYandICP_ECP_KFC(ECP_TVYDIC, ICPcal, ECPcal, KFCcal);
                }

                string AIPcal = getStrValuefromOilDataEntity(AIPOilDataList, i);
                string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);
                string A95cal = getStrValuefromOilDataEntity(A95OilDataList, i);
                string AEPcal = getStrValuefromOilDataEntity(AEPOilDataList, i);
               
                #region "计算赋值"

                if (Dic != null)
                {
                    if (AIPcal == string.Empty && Dic.Keys.Contains("AIP"))
                    {
                        AIPcal = Dic["AIP"].ToString();
                    }
                    if (A10cal == string.Empty && Dic.Keys.Contains("A10"))
                    {
                        A10cal = Dic["A10"].ToString();
                    }
                    if (A30cal == string.Empty && Dic.Keys.Contains("A30"))
                    {
                        A30cal = Dic["A30"].ToString();
                    }
                    if (A50cal == string.Empty && Dic.Keys.Contains("A50"))
                    {
                        A50cal = Dic["A50"].ToString();
                    }
                    if (A70cal == string.Empty && Dic.Keys.Contains("A70"))
                    {
                        A70cal = Dic["A70"].ToString();
                    }
                    if (A90cal == string.Empty && Dic.Keys.Contains("A90"))
                    {
                        A90cal = Dic["A90"].ToString();
                    }
                    if (A95cal == string.Empty && Dic.Keys.Contains("A95"))
                    {
                        A95cal = Dic["A95"].ToString();
                    }
                    if (AEPcal == string.Empty && Dic.Keys.Contains("AEP"))
                    {
                        AEPcal = Dic["AEP"].ToString();
                    }
                }
                #endregion

                #region "表格赋值"
                if (AIPcal != string.Empty && AIPcal != "非数字")
                    this._gridOil.SetData("AIP", i, AIPcal);
                if (A10cal != string.Empty && A10cal != "非数字")
                    this._gridOil.SetData("A10", i, A10cal);
                if (A30cal != string.Empty && A30cal != "非数字")
                    this._gridOil.SetData("A30", i, A30cal);
                if (A50cal != string.Empty && A50cal != "非数字")
                    this._gridOil.SetData("A50", i, A50cal);
                if (A70cal != string.Empty && A70cal != "非数字")
                    this._gridOil.SetData("A70", i, A70cal);
                if (A90cal != string.Empty && A90cal != "非数字")
                    this._gridOil.SetData("A90", i, A90cal);
                if (A95cal != string.Empty && A95cal != "非数字")
                    this._gridOil.SetData("A95", i, A95cal);
                if (AEPcal != string.Empty && AEPcal != "非数字")
                    this._gridOil.SetData("AEP", i, AEPcal);
                #endregion
            }

            #endregion

        }
        /// <summary>
        /// 宽馏分的KFC关联补充 
        /// </summary>
        private void WideGridOilKFCLinkSupplement()
        {
            KFC_WideLinkSupplement();
        }
        /// <summary> 
        /// 宽馏分的BMI关联补充    
        /// </summary>
        private void WideGridOilBMILinkSupplement()
        {
            BMI_WideLinkSupplement();

            //#region 补充判断条件
            //for (int i = 0; i < this._maxCol; i++)
            //{
            //    float? tempBMI = getCalByItemCode_Column("BMI", i);

            //    float? tempECP = getCalByItemCode_Column("ECP", i);

            //    if (tempBMI != null && tempECP != null && tempECP > 200)
            //    {
            //        this._gridOil.SetData("BMI", i, string.Empty);
            //    }
            //}
            //#endregion
        }
        /// <summary> 
        /// 宽馏分的ANI关联补充    
        /// </summary>
        private void WideGridOilANILinkSupplement()
        {
            ANI_WideLinkSupplement();
            WideGridOilMixSupplementAccumulate("WY", "ANI", "ANI");          

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempANI = getCalByItemCode_Column("ANI", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempANI != null && tempICP != null && tempICP < 120)
                {
                    this._gridOil.SetData("ANI", i, string.Empty);
                    tempANI = null;
                }
                if (tempANI != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("ANI", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        ///  宽馏分的PAN关联补充    
        /// </summary>
        private void WideGridOilPANLinkSupplement()
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

            #region "补充"

            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string PANcal = getStrValuefromOilDataEntity(PANOilDataList, i);

                if (PANcal != string.Empty)
                    continue;

                if (PANcal == string.Empty)
                {
                    #region
                    string P03cal = getStrValuefromOilDataEntity(P03OilDataList, i);
                    string P04cal = getStrValuefromOilDataEntity(P04OilDataList, i);
                    string P05cal = getStrValuefromOilDataEntity(P05OilDataList, i);
                    string P06cal = getStrValuefromOilDataEntity(P06OilDataList, i);
                    string P07cal = getStrValuefromOilDataEntity(P07OilDataList, i);
                    string P08cal = getStrValuefromOilDataEntity(P08OilDataList, i);
                    string P09cal = getStrValuefromOilDataEntity(P09OilDataList, i);
                    string P10cal = getStrValuefromOilDataEntity(P10OilDataList, i);
                    string P11cal = getStrValuefromOilDataEntity(P11OilDataList, i);
                    string P12cal = getStrValuefromOilDataEntity(P12OilDataList, i);

                    List<string> tempList = new List<string>();
                    tempList.Add(P03cal);
                    tempList.Add(P04cal);
                    tempList.Add(P05cal);
                    tempList.Add(P06cal);
                    tempList.Add(P07cal);
                    tempList.Add(P08cal);
                    tempList.Add(P09cal);
                    tempList.Add(P10cal);
                    tempList.Add(P11cal);
                    tempList.Add(P12cal);
                    #endregion

                    PANcal = BaseFunction.FunSumAllowEmpty(tempList);
                }

                if (PANcal != string.Empty && PANcal!= "非数字")
                    this._gridOil.SetData("PAN", i, PANcal);
            }
            #endregion
        }

        /// <summary>
        /// 宽馏分的PAO关联补充 
        /// </summary>
        private void WideGridOilPAOLinkSupplement()
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

            #region "补充"

            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string PAOcal = getStrValuefromOilDataEntity(PAOOilDataList, i);

                if (PAOcal != string.Empty)
                    continue;

                if (PAOcal == string.Empty)
                {
                    #region
                    string I03cal = getStrValuefromOilDataEntity(I03OilDataList, i);
                    string I04cal = getStrValuefromOilDataEntity(I04OilDataList, i);
                    string I05cal = getStrValuefromOilDataEntity(I05OilDataList, i);
                    string I06cal = getStrValuefromOilDataEntity(I06OilDataList, i);
                    string I07cal = getStrValuefromOilDataEntity(I07OilDataList, i);
                    string I08cal = getStrValuefromOilDataEntity(I08OilDataList, i);
                    string I09cal = getStrValuefromOilDataEntity(I09OilDataList, i);
                    string I10cal = getStrValuefromOilDataEntity(I10OilDataList, i);
                    string I11cal = getStrValuefromOilDataEntity(I11OilDataList, i);
                    string I12cal = getStrValuefromOilDataEntity(I12OilDataList, i);

                    List<string> tempList = new List<string>();
                    tempList.Add(I03cal);
                    tempList.Add(I04cal);
                    tempList.Add(I05cal);
                    tempList.Add(I06cal);
                    tempList.Add(I07cal);
                    tempList.Add(I08cal);
                    tempList.Add(I09cal);
                    tempList.Add(I10cal);
                    tempList.Add(I11cal);
                    tempList.Add(I12cal);
                    #endregion

                    PAOcal = BaseFunction.FunSumAllowEmpty(tempList);
                }
                if (PAOcal != string.Empty && PAOcal != "非数字")
                    this._gridOil.SetData("PAO", i, PAOcal);
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的NAH关联补充 
        /// </summary>
        private void WideGridOilNAHLinkSupplement()
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


            #region "补充"
            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string NAHcal = getStrValuefromOilDataEntity(NAHOilDataList, i);

                if (NAHcal != string.Empty)
                    continue;

                if (NAHcal == string.Empty)
                {
                    #region
                    string N03cal = getStrValuefromOilDataEntity(N03OilDataList, i);
                    string N04cal = getStrValuefromOilDataEntity(N04OilDataList, i);
                    string N05cal = getStrValuefromOilDataEntity(N05OilDataList, i);
                    string N06cal = getStrValuefromOilDataEntity(N06OilDataList, i);
                    string N07cal = getStrValuefromOilDataEntity(N07OilDataList, i);
                    string N08cal = getStrValuefromOilDataEntity(N08OilDataList, i);
                    string N09cal = getStrValuefromOilDataEntity(N09OilDataList, i);
                    string N10cal = getStrValuefromOilDataEntity(N10OilDataList, i);
                    string N11cal = getStrValuefromOilDataEntity(N11OilDataList, i);
                    string N12cal = getStrValuefromOilDataEntity(N12OilDataList, i);

                    List<string> tempList = new List<string>();
                    tempList.Add(N03cal);
                    tempList.Add(N04cal);
                    tempList.Add(N05cal);
                    tempList.Add(N06cal);
                    tempList.Add(N07cal);
                    tempList.Add(N08cal);
                    tempList.Add(N09cal);
                    tempList.Add(N10cal);
                    tempList.Add(N11cal);
                    tempList.Add(N12cal);
                    #endregion

                    NAHcal = BaseFunction.FunSumAllowEmpty(tempList);
                }
                if (NAHcal == string.Empty)
                {
                    string MNAcal = getStrValuefromOilDataEntity(MNAOilDataList, i);
                    NAHcal = MNAcal;
                }
                if (NAHcal != string.Empty && NAHcal != "非数字")
                    this._gridOil.SetData("NAH", i, NAHcal);
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的ARM关联补充 
        /// </summary>
        private void WideGridOilARMLinkSupplement()
        {
            List<OilDataEntity> ARMOilDataList = this._gridOil.GetDataByRowItemCode("ARM");

            List<OilDataEntity> A03OilDataList = this._gridOil.GetDataByRowItemCode("A03");
            List<OilDataEntity> A04OilDataList = this._gridOil.GetDataByRowItemCode("A04");
            List<OilDataEntity> A05OilDataList = this._gridOil.GetDataByRowItemCode("A05");
            List<OilDataEntity> A06OilDataList = this._gridOil.GetDataByRowItemCode("A06");
            List<OilDataEntity> A07OilDataList = this._gridOil.GetDataByRowItemCode("A07");
            List<OilDataEntity> A08OilDataList = this._gridOil.GetDataByRowItemCode("A08");
            List<OilDataEntity> A09OilDataList = this._gridOil.GetDataByRowItemCode("A09");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("10A");//此处用的是10A
            List<OilDataEntity> A11OilDataList = this._gridOil.GetDataByRowItemCode("A11");
            List<OilDataEntity> A12OilDataList = this._gridOil.GetDataByRowItemCode("A12");
            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");

            #region "补充"
            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string ARMcal = getStrValuefromOilDataEntity(ARMOilDataList, i);

                if (ARMcal != string.Empty)
                    continue;

                if (ARMcal == string.Empty)
                {
                    #region
                    string A03cal = getStrValuefromOilDataEntity(A03OilDataList, i);
                    string A04cal = getStrValuefromOilDataEntity(A04OilDataList, i);
                    string A05cal = getStrValuefromOilDataEntity(A05OilDataList, i);
                    string A06cal = getStrValuefromOilDataEntity(A06OilDataList, i);
                    string A07cal = getStrValuefromOilDataEntity(A07OilDataList, i);
                    string A08cal = getStrValuefromOilDataEntity(A08OilDataList, i);
                    string A09cal = getStrValuefromOilDataEntity(A09OilDataList, i);
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A11cal = getStrValuefromOilDataEntity(A11OilDataList, i);
                    string A12cal = getStrValuefromOilDataEntity(A12OilDataList, i);

                    List<string> tempList = new List<string>();
                    tempList.Add(A03cal);
                    tempList.Add(A04cal);
                    tempList.Add(A05cal);
                    tempList.Add(A06cal);
                    tempList.Add(A07cal);
                    tempList.Add(A08cal);
                    tempList.Add(A09cal);
                    tempList.Add(A10cal);
                    tempList.Add(A11cal);
                    tempList.Add(A12cal);
                    #endregion

                    ARMcal = BaseFunction.FunSumAllowEmpty(tempList);
                }
                if (ARMcal == string.Empty)
                {
                    string MATcal = getStrValuefromOilDataEntity(MATOilDataList, i);
                    ARMcal = MATcal;
                }
                if (ARMcal != string.Empty && ARMcal != "非数字")
                    this._gridOil.SetData("ARM", i, ARMcal);
            }
            #endregion
        }
        /// <summary> 
        /// 宽馏分的GCT关联补充  GCT=SUM(PAN:UNK)
        /// PAN:PAO:NAH:ARM:OLE:UNK
        /// </summary>
        private void WideGridOilGCTLinkSupplement()
        {
            List<OilDataEntity> GCTOilDataList = this._gridOil.GetDataByRowItemCode("GCT");

            List<OilDataEntity> PANOilDataList = this._gridOil.GetDataByRowItemCode("PAN");
            List<OilDataEntity> PAOOilDataList = this._gridOil.GetDataByRowItemCode("PAO");
            List<OilDataEntity> NAHOilDataList = this._gridOil.GetDataByRowItemCode("NAH");
            List<OilDataEntity> ARMOilDataList = this._gridOil.GetDataByRowItemCode("ARM");
            List<OilDataEntity> OLEOilDataList = this._gridOil.GetDataByRowItemCode("OLE");
            List<OilDataEntity> UNKOilDataList = this._gridOil.GetDataByRowItemCode("UNK");

            List<OilDataEntity> PATOilDataList = this._gridOil.GetDataByRowItemCode("PAT");
            for (int i = 0; i < this._maxCol; i++)
            {
                string GCTcal = getStrValuefromOilDataEntity(GCTOilDataList, i);

                if (GCTcal != string.Empty)
                    continue;

                ///*GCT=SUM(PAN:UNK)    PAN:PAO:NAH:ARM:OLE:UNK*/
                if (GCTcal == string.Empty)
                {
                    string PANcal = getStrValuefromOilDataEntity(PANOilDataList, i);
                    string PAOcal = getStrValuefromOilDataEntity(PAOOilDataList, i);
                    string NAHcal = getStrValuefromOilDataEntity(NAHOilDataList, i);
                    string ARMcal = getStrValuefromOilDataEntity(ARMOilDataList, i);

                    string OLEcal = getStrValuefromOilDataEntity(OLEOilDataList, i);
                    string UNKcal = getStrValuefromOilDataEntity(UNKOilDataList, i);
                    string PATcal = getStrValuefromOilDataEntity(PATOilDataList, i);

                    List<string> tempList = new List<string>();
                    if (PATcal != string.Empty &&(PANcal == string.Empty || PAOcal == string.Empty) )
                    {
                        tempList.Add(PATcal);
                    }
                    else
                    {
                        tempList.Add(PANcal);
                        tempList.Add(PAOcal);
                    }
                    tempList.Add(NAHcal);
                    tempList.Add(ARMcal);
                    tempList.Add(OLEcal);
                    tempList.Add(UNKcal);
                    GCTcal = BaseFunction.FunSumNotAllowEmpty(tempList);

                }
                if (GCTcal != string.Empty && GCTcal != "非数字")
                    this._gridOil.SetData("GCT", i, GCTcal);
            }
        }
        /// <summary> 
        /// 宽馏分的N2A关联补充   N2A=NAH+2*ARM
        /// </summary>
        private void WideGridOilN2ALinkSupplement()
        {
            List<OilDataEntity> N2AOilDataList = this._gridOil.GetDataByRowItemCode("N2A");

            List<OilDataEntity> NAHOilDataList = this._gridOil.GetDataByRowItemCode("NAH");
            List<OilDataEntity> ARMOilDataList = this._gridOil.GetDataByRowItemCode("ARM");

            for (int i = 0; i < this._maxCol; i++)
            {
                string N2Acal = getStrValuefromOilDataEntity(N2AOilDataList, i);

                if (N2Acal != string.Empty)
                    continue;
                /*N2A关联补充   N2A=NAH+2*ARM*/

                if (N2Acal == string.Empty)
                {
                    string ARMcal = getStrValuefromOilDataEntity(ARMOilDataList, i);
                    string NAHcal = getStrValuefromOilDataEntity(NAHOilDataList, i);

                    float ARM = 0; float fN2A = 0; float NAH = 0;
                    if (float.TryParse(ARMcal, out ARM))
                    {
                        fN2A += ARM * 2;
                    }
                    if (float.TryParse(NAHcal, out NAH))
                    {
                        fN2A += NAH;
                    }

                    N2Acal = fN2A == 0 ? string.Empty : fN2A.ToString();
                }
                if (N2Acal != string.Empty && N2Acal != "非数字")
                    this._gridOil.SetData("N2A", i, N2Acal);
            }
        }
        /// <summary>
        /// 宽馏分的ARP关联补充N06,A06,N07,A07,N08,A08->ARP
        /// </summary>
        private void WideGridOilARPLinkSupplement()
        {
            List<OilDataEntity> ARPOilDataList = this._gridOil.GetDataByRowItemCode("ARP");

            List<OilDataEntity> N06OilDataList = this._gridOil.GetDataByRowItemCode("N06");
            List<OilDataEntity> A06OilDataList = this._gridOil.GetDataByRowItemCode("A06");
            List<OilDataEntity> N07OilDataList = this._gridOil.GetDataByRowItemCode("N07");
            List<OilDataEntity> A07OilDataList = this._gridOil.GetDataByRowItemCode("A07");
            List<OilDataEntity> N08OilDataList = this._gridOil.GetDataByRowItemCode("N08");
            List<OilDataEntity> A08OilDataList = this._gridOil.GetDataByRowItemCode("A08");

            for (int i = 0; i < this._maxCol; i++)
            {
                string ARPcal = getStrValuefromOilDataEntity(ARPOilDataList, i);

                if (ARPcal != string.Empty)
                    continue;

                if (ARPcal == string.Empty)
                {
                    #region
                    string N06cal = getStrValuefromOilDataEntity(N06OilDataList, i);
                    string N07cal = getStrValuefromOilDataEntity(N07OilDataList, i);
                    string N08cal = getStrValuefromOilDataEntity(N08OilDataList, i);
                    string A06cal = getStrValuefromOilDataEntity(A06OilDataList, i);
                    string A07cal = getStrValuefromOilDataEntity(A07OilDataList, i);
                    string A08cal = getStrValuefromOilDataEntity(A08OilDataList, i);

                    ARPcal = BaseFunction.FunARP(N06cal, N07cal, N08cal, A06cal, A07cal, A08cal);
                    #endregion
                }
                if (ARPcal != string.Empty && ARPcal != "非数字")
                    this._gridOil.SetData("ARP", i, ARPcal);
            }
        }
        /// <summary>
        /// CHR=FUN(WY(from i to j),CHR(from i to j))
        /// </summary>
        private void WideGridOilCHRLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "CHR", "CHR");
        }
        /// <summary>
        /// 宽馏分的RVP
        /// </summary>
        private void WideGridOilRVPLinkSupplement()
        {
            RVP_WideLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempRVP = getCalByItemCode_Column("RVP", i);

                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempRVP != null && tempECP != null && tempECP > 200)
                {
                    this._gridOil.SetData("RVP", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的FRZ
        /// </summary>
        private void WideGridOilFRZLinkSupplement()
        {
            FRZWideLinkSupplement();
            WideGridOilMixSupplementAccumulate("WY", "FRZ", "FRZ");         

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempFRZ = getCalByItemCode_Column("FRZ", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempFRZ != null && tempICP != null && tempICP < 120)
                {
                    this._gridOil.SetData("FRZ", i, string.Empty);
                    tempFRZ = null;
                }
                if (tempFRZ != null && tempECP != null && tempECP > 250)
                {
                    this._gridOil.SetData("FRZ", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的SMK
        /// </summary>
        private void WideGridOilSMKLinkSupplement()
        {
            SMK_WideLinkSupplement();

            WideGridOilMixSupplementAccumulate("WY", "SMK", "SMK");
           
            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempSMK = getCalByItemCode_Column("SMK", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempSMK != null && tempICP != null && tempICP < 120)
                {
                    this._gridOil.SetData("SMK", i, string.Empty);
                    tempSMK = null;
                }
                if (tempSMK != null && tempECP != null && tempECP > 250)
                {
                    this._gridOil.SetData("SMK", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的SAV
        /// </summary>
        private void WideGridOilSAVLinkSupplement()
        {
            SAV_ARV_WideLinkSupplement();

            WideGridOilMixSupplementAccumulate("WY", "SAV", "SAV");
          
            judgeConditionByICP120_ECP250("SAV");
        }
        /// <summary>
        /// 宽馏分的ARV
        /// </summary>
        private void WideGridOilARVLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "ARV", "ARV");
            //SAV_ARV_WideLinkSupplement();

            judgeConditionByICP120_ECP250("SAV");
            judgeConditionByICP120_ECP250("ARV");
        }

        /// <summary>
        /// 宽馏分的OLV
        /// </summary>
        private void WideGridOilOLVLinkSupplement()
        {
            judgeConditionByICP120_ECP250("OLV");
        }

        /// <summary>
        /// 补充判断条件：ICP小于120，或者ECP大于250，清空
        /// </summary>
        /// <param name="itemCode"></param>
        private void judgeConditionByICP120_ECP250(string itemCode)
        {
            for (int i = 0; i < this._maxCol; i++)
            {
                float? temp = getCalByItemCode_Column(itemCode, i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (temp != null && tempICP != null && tempICP < 120 )
                {
                    this._gridOil.SetData(itemCode, i, string.Empty);
                    temp = null;
                }
                if (temp != null && tempECP != null && tempECP > 250)
                {
                    this._gridOil.SetData(itemCode, i, string.Empty);
                }
            }
        }
        /// <summary>
        /// 宽馏分的ＬＨＶ
        /// </summary>
        private void WideGridOilLHVLinkSupplement()
        {
            List<OilDataEntity> LHVOilDataList = this._gridOil.GetDataByRowItemCode("LHV");

            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");

            for (int i = 0; i < this._maxCol; i++)
            {
                string LHVcal = getStrValuefromOilDataEntity(LHVOilDataList, i);

                if (LHVcal != string.Empty)
                    continue;

                string ANIcal = getStrValuefromOilDataEntity(ANIOilDataList, i);
                string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                if (LHVcal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    LHVcal = BaseFunction.FunLHVfromD20_ANI_SUL(D20cal, ANIcal, SULcal);
                }
                if (LHVcal == string.Empty)
                {
                    string APIcal = getStrValuefromOilDataEntity(APIOilDataList, i);
                    LHVcal = BaseFunction.FunLHVfromAPI_ANI_SUL(APIcal, ANIcal, SULcal);
                }
                if (LHVcal != string.Empty && LHVcal != "非数字")
                    this._gridOil.SetData("LHV", i, LHVcal);
            }
        }

        /// <summary> 
        /// 宽馏分的IRT关联补充   IRT=SUM(SAV:OLV)  SAV:ARV:OLV
        /// </summary>
        private void WideGridOilIRTLinkSupplement()
        {
            List<OilDataEntity> IRTOilDataList = this._gridOil.GetDataByRowItemCode("IRT");

            List<OilDataEntity> SAVOilDataList = this._gridOil.GetDataByRowItemCode("SAV");
            List<OilDataEntity> OLVOilDataList = this._gridOil.GetDataByRowItemCode("OLV");
            List<OilDataEntity> ARVOilDataList = this._gridOil.GetDataByRowItemCode("ARV");

            for (int i = 0; i < this._maxCol; i++)
            {
                string SAVcal = getStrValuefromOilDataEntity(SAVOilDataList, i);
                string ARVcal = getStrValuefromOilDataEntity(ARVOilDataList, i);
                string OLVcal = getStrValuefromOilDataEntity(OLVOilDataList, i);

                string IRTcal = getStrValuefromOilDataEntity(IRTOilDataList, i);

                if (IRTcal != string.Empty)
                {
                    continue;
                }

                if (IRTcal == string.Empty)
                {
                    List<string> tempList = new List<string>();
                    tempList.Add(SAVcal);
                    tempList.Add(ARVcal);
                    tempList.Add(OLVcal);
                    IRTcal = BaseFunction.FunSumAllowEmpty(tempList);
                }
                if (IRTcal != string.Empty && IRTcal != "非数字")
                    this._gridOil.SetData("IRT", i, IRTcal);
            }
            judgeConditionByICP120_ECP250("IRT");
        }
        /// <summary>
        /// 宽馏分的POR关联补充   POR=fn(SOP)
        /// </summary>
        private void WideGridOilPORLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "POR", "POR");
            PORLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempPOR = getCalByItemCode_Column("POR", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempPOR != null && tempICP != null && tempICP < 160)
                {
                    this._gridOil.SetData("POR", i, string.Empty);
                    tempPOR = null;
                }
                if (tempPOR != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("POR", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的SOP关联补充   SOP=f5(POR)
        /// </summary>
        private void WideGridOilSOPLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "SOP", "SOP");
            SOPLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempSOP = getCalByItemCode_Column("SOP", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempSOP != null && tempICP != null && tempICP < 160)
                {
                    this._gridOil.SetData("SOP", i, string.Empty);
                    tempSOP = null;
                }
                if (tempSOP != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("SOP", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的CLP
        /// </summary>
        private void WideGridOilCLPLinkSupplement()
        {
            WideGridOilMixSupplementAccumulate("WY", "CLP", "CLP");

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempCLP = getCalByItemCode_Column("CLP", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempCLP != null && tempICP != null && tempICP < 160)
                {
                    this._gridOil.SetData("CLP", i, string.Empty);
                    tempCLP = null;
                }
                if (tempCLP != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData("CLP", i, string.Empty);
                }
            }
            #endregion
        }

        /// <summary>
        /// 宽馏分的CI关联补充  
        /// </summary>
        private void WideGridOilCILinkSupplement()
        {
            CI_WideLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempCI = getCalByItemCode_Column("CI", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempCI != null && tempICP != null && tempICP < 140)
                {
                    this._gridOil.SetData("CI", i, string.Empty);
                    tempCI = null;
                }
                if (tempCI != null && tempECP != null && tempECP > 400)
                {
                    this._gridOil.SetData("CI", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的CEN关联补充 D15,…->CEN
        /// </summary>
        private void WideGridOilCENLinkSupplement()
        {
            CEN_WideLinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempCEN = getCalByItemCode_Column("CEN", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempCEN != null && tempICP != null && tempICP < 140)
                {
                    this._gridOil.SetData("CEN", i, string.Empty);
                    tempCEN = null;
                }
                if (tempCEN != null && tempECP != null && tempECP > 400)
                {
                    this._gridOil.SetData("CEN", i, string.Empty);
                }
            }
            #endregion
        }
        /// <summary>
        /// 宽馏分的DI关联补充  
        /// </summary>
        private void WideGridOilDILinkSupplement()
        {
            DILinkSupplement();

            #region 补充判断条件
            for (int i = 0; i < this._maxCol; i++)
            {
                float? tempDI = getCalByItemCode_Column("DI", i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (tempDI != null && tempICP != null && tempICP < 140)
                {
                    this._gridOil.SetData("DI", i, string.Empty);
                    tempDI = null;
                }
                if (tempDI != null && tempECP != null && tempECP > 400)
                {
                    this._gridOil.SetData("DI", i, string.Empty);
                }
            }
            #endregion
        }

        /// <summary>
        /// 宽馏分的SAH关联补充 
        /// SAH=MSP;
        /// SAH=M01+MNA
        /// </summary>
        private void WideGridOilSAHLinkSupplement()
        {
            List<OilDataEntity> SAHOilDataList = this._gridOil.GetDataByRowItemCode("SAH");
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");
            List<OilDataEntity> M01OilDataList = this._gridOil.GetDataByRowItemCode("M01");
            List<OilDataEntity> MNAOilDataList = this._gridOil.GetDataByRowItemCode("MNA");
            for (int i = 0; i < this._maxCol; i++)
            {
                string SAHcal = getStrValuefromOilDataEntity(SAHOilDataList, i);

                if (SAHcal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("SAH", i, SAHcal);
            }

            Comm_Judge_Condition("SAH");
        }
        /// <summary>
        /// 宽馏分的ARS关联补充  
        /// ARS=MAT; 
        /// ARS= MA1+MA2+MA3+MA4+MA5(充许空值）
        /// </summary>
        private void WideGridOilARSLinkSupplement()
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
                string ARScal = getStrValuefromOilDataEntity(ARSOilDataList, i);

                if (ARScal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("ARS", i, ARScal);
            }

            Comm_Judge_Condition("ARS");

        }
        /// <summary>
        /// 宽馏分的RES关联补充
        /// RES=MRS
        /// </summary>
        private void WideGridOilRESLinkSupplement()
        {
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");
            List<OilDataEntity> MRSOilDataList = this._gridOil.GetDataByRowItemCode("MRS");

            for (int i = 0; i < this._maxCol; i++)
            {
                string REScal = getStrValuefromOilDataEntity(RESOilDataList, i);

                if (REScal != string.Empty)
                    continue;

                if (REScal == string.Empty)
                {
                    string MRScal = getStrValuefromOilDataEntity(MRSOilDataList, i);
                    REScal = MRScal;
                }

                if (REScal != string.Empty && REScal != "非数字")
                    this._gridOil.SetData("RES", i, REScal);
            }

            Comm_Judge_Condition("RES");
        }
        /// <summary>
        /// 宽馏分的APH关联补充  
        /// APH=100-SAH-ARS-RES
        /// </summary>
        private void WideGridOilAPHLinkSupplement()
        {
            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
            List<OilDataEntity> SAHOilDataList = this._gridOil.GetDataByRowItemCode("SAH");
            List<OilDataEntity> ARSOilDataList = this._gridOil.GetDataByRowItemCode("ARS");
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");

            for (int i = 0; i < this._maxCol; i++)
            {
                string APHcal = getStrValuefromOilDataEntity(APHOilDataList, i);

                if (APHcal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("APH", i, APHcal);
            }

            Comm_Judge_Condition("APH");
        }
        /// <summary> 
        /// 宽馏分的4CT关联补充  SUM(SAH:APH)容许空值
        /// SAH:ARS:RES:APH
        /// </summary>
        private void WideGridOil4CTLinkSupplement()
        {
            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
            List<OilDataEntity> SAHOilDataList = this._gridOil.GetDataByRowItemCode("SAH");
            List<OilDataEntity> ARSOilDataList = this._gridOil.GetDataByRowItemCode("ARS");
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");
            List<OilDataEntity> _4CTOilDataList = this._gridOil.GetDataByRowItemCode("4CT");
            for (int i = 0; i < this._maxCol; i++)
            {
                string _4CTcal = getStrValuefromOilDataEntity(_4CTOilDataList, i);
                if (_4CTcal != string.Empty)
                    continue;
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
                    this._gridOil.SetData("4CT", i, _4CTcal);
            }

            Comm_Judge_Condition("4CT");
        }

      
        /// <summary> 
        /// 宽馏分的PAT关联补充  
        /// PAT=M01
        /// </summary>
        private void WideGridOilPATLinkSupplement()
        {
            List<OilDataEntity> PATOilDataList = this._gridOil.GetDataByRowItemCode("PAT");
            List<OilDataEntity> M01OilDataList = this._gridOil.GetDataByRowItemCode("M01");

            List<OilDataEntity> PANOilDataList = this._gridOil.GetDataByRowItemCode("PAN");
            List<OilDataEntity> PAOOilDataList = this._gridOil.GetDataByRowItemCode("PAO");
            for (int i = 0; i < this._maxCol; i++)
            {
                string PATcal = getStrValuefromOilDataEntity(PATOilDataList, i);
                if (PATcal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("PAT", i, PATcal);
            }
        }

        /// <summary>
        /// 宽馏分的CPP_RAA 关联补充  
        /// </summary>
        private void WideGridOilCPP_RAALinkSupplement()
        {
            CPP_RAA_WideLinkSupplement1();
            CPP_RAA_WideLinkSupplement2();

            Comm_Judge_Condition("CPP");
            Comm_Judge_Condition("CNN");
            Comm_Judge_Condition("CAA");
            Comm_Judge_Condition("RTT");
            Comm_Judge_Condition("RNN");
            Comm_Judge_Condition("RAA");
        }

        /// <summary>
        /// 公用补充判断条件(ICP小于300或者ECP大于600,清空)
        /// </summary>
        /// <param name="itemCode"></param>
        private void Comm_Judge_Condition(string itemCode)
        {
            for (int i = 0; i < this._maxCol; i++)
            {
                float? temp = getCalByItemCode_Column(itemCode, i);

                float? tempICP = getCalByItemCode_Column("ICP", i);
                float? tempECP = getCalByItemCode_Column("ECP", i);

                if (temp != null && tempICP != null && tempICP < 300)
                {
                    this._gridOil.SetData(itemCode, i, string.Empty);
                    temp = null;
                }
                if (temp != null && tempECP != null && tempECP > 600)
                {
                    this._gridOil.SetData(itemCode, i, string.Empty);
                }
            }
        }

        /// <summary> 
        /// 宽馏分的MNA关联补充  MNA=SUM(M02:M07)
        /// M02:M03:M04:M05:M06:M07
        /// </summary>
        private void WideGridOilMNALinkSupplement()
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
                string MNAcal = getStrValuefromOilDataEntity(MNAOilDataList, i);

                if (MNAcal != string.Empty)
                    continue;
                /*GCT=SUM(PAN:UNK)    PAN:PAO:NAH:ARM:OLE:UNK*/
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
                    this._gridOil.SetData("MNA", i, MNAcal);
            }
        }
        /// <summary> 
        /// 宽馏分的MSP关联补充  MSP=M01+MNA容许空值
        /// </summary>
        private void WideGridOilMSPLinkSupplement()
        {
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");

            List<OilDataEntity> M01OilDataList = this._gridOil.GetDataByRowItemCode("M01");
            List<OilDataEntity> MNAOilDataList = this._gridOil.GetDataByRowItemCode("MNA");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MSPcal = getStrValuefromOilDataEntity(MSPOilDataList, i);

                if (MSPcal != string.Empty)
                    continue;
                /*MSP关联补充  MSP=M01+MNA*/
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
                    this._gridOil.SetData("MSP", i, MSPcal);
            }
        }
        /// <summary> 
        /// 宽馏分的MA1关联补充  SUM(M08:M12)
        /// M08:M09:M10:M11:M12
        /// </summary>
        private void WideGridOilMA1LinkSupplement()
        {
            List<OilDataEntity> MA1OilDataList = this._gridOil.GetDataByRowItemCode("MA1");

            List<OilDataEntity> M08OilDataList = this._gridOil.GetDataByRowItemCode("M08");
            List<OilDataEntity> M09OilDataList = this._gridOil.GetDataByRowItemCode("M09");
            List<OilDataEntity> M10OilDataList = this._gridOil.GetDataByRowItemCode("M10");
            List<OilDataEntity> M11OilDataList = this._gridOil.GetDataByRowItemCode("M11");
            List<OilDataEntity> M12OilDataList = this._gridOil.GetDataByRowItemCode("M12");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MA1cal = getStrValuefromOilDataEntity(MA1OilDataList, i);

                if (MA1cal != string.Empty)
                    continue;
                /*GCT=SUM(PAN:UNK)    PAN:PAO:NAH:ARM:OLE:UNK*/

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
                    this._gridOil.SetData("MA1", i, MA1cal);
            }
        }
        /// <summary> 
        /// 宽馏分的MA2关联补充  SUM(M13:M18)
        /// M13:M14:M15:M16:M17:M18 
        /// </summary>
        private void WideGridOilMA2LinkSupplement()
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
                string MA2cal = getStrValuefromOilDataEntity(MA2OilDataList, i);

                if (MA2cal != string.Empty)
                    continue;
                /*MA2关联补充  SUM(M13:M18)  M13:M14:M15:M16:M17:M18 */
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
                    this._gridOil.SetData("MA2", i, MA2cal);
            }
        }
        /// <summary> 
        /// 宽馏分的MA3关联补充  SUM(M19:M20)
        /// M19:M20 
        /// </summary>
        private void WideGridOilMA3LinkSupplement()
        {
            List<OilDataEntity> MA3OilDataList = this._gridOil.GetDataByRowItemCode("MA3");

            List<OilDataEntity> M19OilDataList = this._gridOil.GetDataByRowItemCode("M19");
            List<OilDataEntity> M20OilDataList = this._gridOil.GetDataByRowItemCode("M20");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MA3cal = getStrValuefromOilDataEntity(MA3OilDataList, i);

                if (MA3cal != string.Empty)
                    continue;
                /*MA3关联补充  SUM(M19:M20)*/
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
                    this._gridOil.SetData("MA3", i, MA3cal);
            }
        }
        /// <summary> 
        /// 宽馏分的MA4关联补充  SUM(M21:M22)
        /// M21:M22 
        /// </summary>
        private void WideGridOilMA4LinkSupplement()
        {
            List<OilDataEntity> MA4OilDataList = this._gridOil.GetDataByRowItemCode("MA4");

            List<OilDataEntity> M21OilDataList = this._gridOil.GetDataByRowItemCode("M21");
            List<OilDataEntity> M22OilDataList = this._gridOil.GetDataByRowItemCode("M22");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MA4cal = getStrValuefromOilDataEntity(MA4OilDataList, i);

                if (MA4cal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("MA4", i, MA4cal);
            }
        }
        /// <summary> 
        /// 宽馏分的MA5关联补充 SUM(M23:M24)
        /// M23:M24 
        /// </summary>
        private void WideGridOilMA5LinkSupplement()
        {
            List<OilDataEntity> MA5OilDataList = this._gridOil.GetDataByRowItemCode("MA5");

            List<OilDataEntity> M23OilDataList = this._gridOil.GetDataByRowItemCode("M23");
            List<OilDataEntity> M24OilDataList = this._gridOil.GetDataByRowItemCode("M24");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MA5cal = getStrValuefromOilDataEntity(MA5OilDataList, i);

                if (MA5cal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("MA5", i, MA5cal);
            }
        }
        /// <summary> 
        /// 宽馏分的MAN关联补充 SUM(M26:M28)
        /// M26:M27 :M28 
        /// </summary>
        private void WideGridOilMANLinkSupplement()
        {
            List<OilDataEntity> MANOilDataList = this._gridOil.GetDataByRowItemCode("MAN");

            List<OilDataEntity> M26OilDataList = this._gridOil.GetDataByRowItemCode("M26");
            List<OilDataEntity> M27OilDataList = this._gridOil.GetDataByRowItemCode("M27");
            List<OilDataEntity> M28OilDataList = this._gridOil.GetDataByRowItemCode("M28");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MANcal = getStrValuefromOilDataEntity(MANOilDataList, i);

                if (MANcal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("MAN", i, MANcal);
            }
        }
        /// <summary> 
        /// 宽馏分的MAT关联补充  SUM(MA1+MA2+MA3+MA4+MA5 )
        /// MA1+MA2+MA3+MA4+MA5 
        /// </summary>
        private void WideGridOilMATLinkSupplement()
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
                string MATcal = getStrValuefromOilDataEntity(MATOilDataList, i);

                if (MATcal != string.Empty)
                    continue;
                /*MAT关联补充  SUM(MA1+MA2+MA3+MA4+MA5 +MAN+MAU ) */
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
                    this._gridOil.SetData("MAT", i, MATcal);
            }
        }
        /// <summary> 
        /// 宽馏分的MTA关联补充 SUM(MAT+MRS+MSP)
        /// MAT+MRS+MSP 
        /// </summary>
        private void WideGridOilMTALinkSupplement()
        {
            List<OilDataEntity> MTAOilDataList = this._gridOil.GetDataByRowItemCode("MTA");

            List<OilDataEntity> MATOilDataList = this._gridOil.GetDataByRowItemCode("MAT");
            List<OilDataEntity> MRSOilDataList = this._gridOil.GetDataByRowItemCode("MRS");
            List<OilDataEntity> MSPOilDataList = this._gridOil.GetDataByRowItemCode("MSP");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MTAcal = getStrValuefromOilDataEntity(MTAOilDataList, i);

                if (MTAcal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("MTA", i, MTAcal);
            }
        }
        /// <summary>
        /// 宽馏分的FPO关联补充  
        /// </summary>
        private void WideGridOilFPOLinkSupplement()
        {
            FPOLinkSupplement();
        }
        /// <summary>
        /// GC表数据转给宽馏分
        /// </summary>
        private void getGCCalToWideSupplement()
        {
            List<OilDataEntity> AllGCDataList = this._gcGridOil.GetAllData();
            List<OilDataEntity> GCDataList = AllGCDataList.Where (o=>o.calData != string.Empty).ToList();
            List<OilDataEntity> GCICPList = GCDataList.Where(o => o.OilTableRow.itemCode == "ICP").ToList();
            List<OilDataEntity> GCECPList = GCDataList.Where(o => o.OilTableRow.itemCode == "ECP").ToList();
            foreach (var ICP in GCICPList)
            {
                OilDataEntity ECP = GCECPList.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                if (ECP != null)
                {
                    List<OilDataEntity> AllWideICPList = this._gridOil.GetDataByRowItemCode("ICP");
                    List<OilDataEntity> AllWideECPList = this._gridOil.GetDataByRowItemCode("ECP");
                    List<OilDataEntity> WideICPList = AllWideICPList.Where(o=>o.calData == ICP.calData).ToList();
                    List<OilDataEntity> WideECPList = AllWideECPList.Where(o =>o.calData == ECP.calData).ToList();
                    foreach (var wideICP in WideICPList)
                    {
                        OilDataEntity wideECP = WideECPList.Where(o => o.OilTableCol.colCode == wideICP.OilTableCol.colCode).FirstOrDefault();
                        if (wideECP != null)
                        {
                            List<OilDataEntity> GCColList = GCDataList.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).ToList();
                            foreach (var data in GCColList)
                            {
                                this._gridOil.SetData(data.OilTableRow.itemCode, wideECP.ColumnIndex, data.calData);                            
                            }
                        }
                    }               
                }           
            }
        
        }


        #endregion 

        #region "原油性质的强制补充"
        /// <summary>
        /// 渣油表的CCR联补充
        /// </summary>
        private void WholeCCRDataCorrectionSupplement()
        {
            List<OilDataEntity> CCROilDataList = this._gridOil.GetDataByRowItemCode("CCR");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeCCROilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "CCR").FirstOrDefault();
            if (wholeCCROilData == null)
                return;
            if (wholeCCROilData.calData == string.Empty)
                return;

            float WholeCCR = 0;
            if (float.TryParse(wholeCCROilData.calData, out WholeCCR))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string CCRcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        CCRcal = (WholeCCR * 100 / WY).ToString();
                    }
                    #endregion

                    if (CCRcal != string.Empty && CCRcal != "非数字")
                        this._gridOil.SetData("CCR", i, CCRcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的FE联补充
        /// </summary>
        private void WholeFEDataCorrectionSupplement()
        {
            List<OilDataEntity> FEOilDataList = this._gridOil.GetDataByRowItemCode("FE");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeFEOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "FE").FirstOrDefault();
            if (wholeFEOilData == null)
                return;
            if (wholeFEOilData.calData == string.Empty)
                return;

            float WholeFE = 0;
            if (float.TryParse(wholeFEOilData.calData, out WholeFE))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string FEcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        FEcal = (WholeFE * 100 / WY).ToString();
                    }
                    #endregion

                    if (FEcal != string.Empty && FEcal != "非数字")
                        this._gridOil.SetData("FE", i, FEcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的NI联补充
        /// </summary>
        private void WholeNIDataCorrectionSupplement()
        {
            List<OilDataEntity> NIOilDataList = this._gridOil.GetDataByRowItemCode("NI");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeNIOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "NI").FirstOrDefault();
            if (wholeNIOilData == null)
                return;
            if (wholeNIOilData.calData == string.Empty)
                return;

            float WholeNI = 0;
            if (float.TryParse(wholeNIOilData.calData, out WholeNI))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string NIcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        NIcal = (WholeNI * 100 / WY).ToString();
                    }
                    #endregion

                    if (NIcal != string.Empty && NIcal != "非数字")
                        this._gridOil.SetData("NI", i, NIcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的V联补充
        /// </summary>
        private void WholeVDataCorrectionSupplement()
        {
            List<OilDataEntity> NIOilDataList = this._gridOil.GetDataByRowItemCode("V");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeVOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "V").FirstOrDefault();
            if (wholeVOilData == null)
                return;
            if (wholeVOilData.calData == string.Empty)
                return;

            float WholeV = 0;
            if (float.TryParse(wholeVOilData.calData, out WholeV))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string Vcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        Vcal = (WholeV * 100 / WY).ToString();
                    }
                    #endregion

                    if (Vcal != string.Empty && Vcal != "非数字")
                        this._gridOil.SetData("V", i, Vcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的CA联补充
        /// </summary>
        private void WholeCADataCorrectionSupplement()
        {
            List<OilDataEntity> CAOilDataList = this._gridOil.GetDataByRowItemCode("CA");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeCAOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "CA").FirstOrDefault();
            if (wholeCAOilData == null)
                return;
            if (wholeCAOilData.calData == string.Empty)
                return;

            float WholeCA = 0;
            if (float.TryParse(wholeCAOilData.calData, out WholeCA))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string CAcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        CAcal = (WholeCA * 100 / WY).ToString();
                    }
                    #endregion

                    if (CAcal != string.Empty && CAcal != "非数字")
                        this._gridOil.SetData("CA", i, CAcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的NA联补充
        /// </summary>
        private void WholeNADataCorrectionSupplement()
        {
            List<OilDataEntity> NAOilDataList = this._gridOil.GetDataByRowItemCode("NA");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeNAOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "NA").FirstOrDefault();
            if (wholeNAOilData == null)
                return;
            if (wholeNAOilData.calData == string.Empty)
                return;

            float WholeNA = 0;
            if (float.TryParse(wholeNAOilData.calData, out WholeNA))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string NAcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        NAcal = (WholeNA * 100 / WY).ToString();
                    }
                    #endregion

                    if (NAcal != string.Empty && NAcal != "非数字")
                        this._gridOil.SetData("NA", i, NAcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的RES联补充
        /// </summary>
        private void WholeRESDataCorrectionSupplement()
        {
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeRESOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "RES").FirstOrDefault();
            if (wholeRESOilData == null)
                return;
            if (wholeRESOilData.calData == string.Empty)
                return;

            float WholeRES = 0;
            if (float.TryParse(wholeRESOilData.calData, out WholeRES))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string REScal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        REScal = (WholeRES * 100 / WY).ToString();
                    }
                    #endregion

                    if (REScal != string.Empty && REScal != "非数字")
                        this._gridOil.SetData("RES", i, REScal);
                }
            }
        }
        /// <summary>
        /// 渣油表的APH联补充
        /// </summary>
        private void WholeAPHDataCorrectionSupplement()
        {
            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeAPHOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "RES").FirstOrDefault();
            if (wholeAPHOilData == null)
                return;
            if (wholeAPHOilData.calData == string.Empty)
                return;

            float WholeAPH = 0;
            if (float.TryParse(wholeAPHOilData.calData, out WholeAPH))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string APHcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        APHcal = (WholeAPH * 100 / WY).ToString();
                    }
                    #endregion

                    if (APHcal != string.Empty && APHcal != "非数字")
                        this._gridOil.SetData("APH", i, APHcal);
                }
            }
        }

        #endregion 
        #region "渣油表强制补充"
        /// <summary>
        /// 累计值补充"WY=100-TWY(NCUTS(NCUTS.ECP=RES.ICP))"
        /// </summary>
        private void ResidueWYDataCorrectionSupplement()
        {
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");

            for (int i = 0; i < this._maxCol; i++)
            {
                string WYcal = string.Empty;

                #region "WY=100-TWY(NCUTS(NCUTS.ECP=RES.ICP))"

                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string strTWY = FunNarrowStartReturnItemCodeValue(ICPcal, "ECP", "TWY");

                float TWY = 0;
                if (float.TryParse(strTWY, out TWY))
                {
                    WYcal = (100 - TWY).ToString();
                }

                #endregion

                if (WYcal != string.Empty && WYcal != "非数字")
                {
                    this._gridOil.SetData("WY", i, WYcal);                
                }
            }
        }

        /// <summary>
        /// 渣油表的TWY联补充
        /// </summary>
        private void ResidueTWYDataCorrectionSupplement()
        {
            #region "TWY=WY+TWY(NCUTS (NCUTS.ECP=RES.ICP))"

            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            for (int i = 0; i < this._maxCol; i++)
            {
                string TWYcal = string.Empty;

                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string strTWY = FunNarrowStartReturnItemCodeValue(ICPcal, "ECP", "TWY");

                float TWY = 0, WY = 0;

                if (float.TryParse(strTWY, out TWY) && float.TryParse(WYcal, out WY))
                {
                    TWYcal = (WY + TWY).ToString();
                }

                if (TWYcal != string.Empty && TWYcal != "非数字")
                {
                    this._gridOil.SetData("TWY", i, TWYcal);

                }
            }

            #endregion
        }
        /// <summary>
        /// 渣油表的VY联补充
        /// </summary>
        private void VYTVYDataCorrectionSupplement()
        {
            #region 

            
            //List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            //for (int i = 0; i < this._maxCol; i++)
            //{
            //    string VYcal = string.Empty;

            //    #region "VY=100-TVY(NCUTS(NCUTS.ECP=RES.ICP))"

            //    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
            //    string strTVY = FunNarrowStartReturnItemCodeValue(ICPcal, "ECP", "TVY");

            //    float TVY = 0;

            //    if (float.TryParse(strTVY, out TVY))
            //    {
            //        VYcal = (100 - TVY).ToString();
            //    }
            //    #endregion

            //    if (VYcal != string.Empty && VYcal != "非数字")
            //        this._gridOil.SetData("VY", i, VYcal);
            //}
            #endregion

            int narrMaxCol = this._narrowGridOil.GetMaxValidColumnIndex() ;

            OilDataEntity narrLastECP = this._narrowGridOil.GetDataByRowItemCode("ECP").Where (o=>o.ColumnIndex == narrMaxCol ).FirstOrDefault ();//的ECP

            if (narrLastECP == null)
                return;
            if (narrLastECP.calShowData == string.Empty)
                return;

            OilDataEntity resICP = this._gridOil.GetDataByRowItemCode("ICP").Where (o=>o.calShowData == narrLastECP.calShowData).FirstOrDefault();//渣油表的ICP
            if (resICP == null)
                return;
            if (resICP.calShowData == string.Empty)
                return;


            OilDataEntity resTVY = this._gridOil.GetDataByRowItemCode("TVY").Where(o => o.OilTableCol.colCode == resICP.OilTableCol.colCode).FirstOrDefault();//渣油表的ICP

            if (resTVY == null)
                return;
            if (resTVY.calShowData == string.Empty)
                return;

            float TVY = 0, DIF = 0;
            if (float.TryParse(resTVY.calShowData, out TVY))
            {
                DIF = 100 - TVY ;
            }

            if (DIF != 0)
            {
                NarrowVYDataCorrectionSupplement(DIF);
                narrowTVYDataCorrectionSupplement();
                //wideVYTVYDataCorrectionSupplement();
                //resVYTVYDataCorrectionSupplement();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void narrowTVYDataCorrectionSupplement()
        {
            int narrMaxCol = this._narrowGridOil.GetMaxValidColumnIndex() + 1;

            for (int i = 0; i < narrMaxCol; i++)
            {             
                string TVYcal = string.Empty;             

                /*TVY(i)=TVY(i-1)+VY(i)*/
                #region "数据补充"
                string VYcal = getStrValuefromOilDataEntity(this._narrowGridOil.GetDataByRowItemCode("VY"), i);
                if (i == 0)
                {
                    if (TVYcal == string.Empty && VYcal != string.Empty)
                    {
                        TVYcal = VYcal;
                    }
                }
                else
                {
                    if (TVYcal == string.Empty)
                    {
                        string TVYbefore = getStrValuefromOilDataEntity(this._narrowGridOil.GetDataByRowItemCode("TVY"), i - 1);
                        TVYcal = BaseFunction.FunTWY(TVYbefore, VYcal); ;
                    }
                }
                #endregion

                if (TVYcal != string.Empty && TVYcal != "非数字")
                    this._narrowGridOil.SetData("TVY", i, TVYcal);
            }
        
        }
        /// <summary>
        /// 强制校正的窄馏分校正
        /// </summary>
        private void NarrowVYDataCorrectionSupplement(float DIF)
        {
            int narrMaxCol = this._narrowGridOil.GetMaxValidColumnIndex() +1;
             
            #region 伪代码

            //在窄馏分表中找出ECP<150的馏分对应的序号index
            //IF index>0
            //    取出从1到index的窄馏分VY，并求其和SUM；
            //    重新计算1--index窄馏分的VY’=VY+DIF* VY/SUM
            //elseIF index=0(没找到ECP<150的窄馏分)
            //    重新计算第1个窄馏分的VY=VY+DIF
            //endif
            #endregion

            List<OilDataEntity> narrowECPOilDataList = this._narrowGridOil.GetDataByRowItemCode("ECP");//的ICP

            int colIndex = 0;
            for (int i = 0; i < narrMaxCol; i++)
            {               
                OilDataEntity ECPData = this._narrowGridOil.GetDataByRowItemCodeColumnIndex("ECP", i);
                float ECP = 0;
                if (float.TryParse(ECPData.calShowData, out ECP))
                {
                    if (ECP < 150)
                    {
                        colIndex = ECPData.ColumnIndex;
                    }
                }
            }

            float SUM = narrowTotalTVY(colIndex);

            for (int i = 0; i < narrMaxCol; i++)
            {
               
                if (i <= colIndex)
                {
                    #region
                    string strVY = string.Empty;

                    string strTempVY = this._narrowGridOil.GetDataByRowItemCodeColumnIndex("VY", i).calShowData;
                    float VY = 0;
                    if (float.TryParse(strTempVY, out VY))
                    {
                        VY = VY + DIF * VY / SUM;
                    }
                    strVY = VY.ToString();
                    this._narrowGridOil.SetData("VY", i, strVY);
                    #endregion               
                }
                else
                {
                    break;
                }
            }          
        }
      
        /// <summary>
        /// 强制校正的宽馏分校正
        /// </summary>
        private void wideVYTVYDataCorrectionSupplement()
        {
            #region "VY"
           
            List<OilDataEntity> ItemCodeOilDataList = this._wideGridOil.GetDataByRowItemCode("VY");

            List<OilDataEntity> ICPOilDataList = this._wideGridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._wideGridOil.GetDataByRowItemCode("ECP");

            int maxCol = this._wideGridOil.GetMaxValidColumnIndex() + 1;    
            #region "数据赋值"

            for (int i = 0; i < maxCol; i++)//宽馏分
            {               
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                string strCal = FunNarrowStartEndTotal(ICPcal, ECPcal, "VY");

                if (strCal != string.Empty && strCal != "非数字")
                    this._wideGridOil.SetData("VY", i, strCal);
            }

            #endregion        
            #endregion

            
            #region "TVY"
            List<OilDataEntity> TVYOilDataList = this._wideGridOil.GetDataByRowItemCode("TVY");
            
            for (int i = 0; i < maxCol; i++)//宽馏分
            {               
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                string TVYcal = FunWideValueFromNarrowECPValue(ECPcal, "TVY");

                if (TVYcal != string.Empty && TVYcal != "非数字")
                    this._wideGridOil.SetData("TVY", i, TVYcal);
            }
            #endregion
        }

        /// <summary>
        /// 强制校正的渣油馏分校正
        /// </summary>
        private void resVYTVYDataCorrectionSupplement()
        {        
            List<OilDataEntity> ICPOilDataList = this._residueGridOil.GetDataByRowItemCode("ICP");

            int resMaxCol = this._residueGridOil.GetMaxValidColumnIndex() + 1;

            #region
            for (int i = 0; i < resMaxCol; i++)//宽馏分
            {        
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string TVYcal = FunWideValueFromNarrowECPValue(ICPcal, "TVY");
             
                string VYcal = string.Empty;

                float   TVY = 0;
                if ( float.TryParse(TVYcal, out TVY))
                {
                    float VY = 100 - TVY;
                    VYcal = VY.ToString();
                }

                this._residueGridOil.SetData("TVY", i, "100");

                if (VYcal != string.Empty && VYcal != "非数字")
                    this._residueGridOil.SetData("VY", i, VYcal);
            }
            #endregion
        }

        private float narrowTotalTVY( int colIndex)
        {
            List<OilDataEntity> narrowVYOilDataList = this._narrowGridOil.GetDataByRowItemCode("VY");//的ICP
            float SUM = 0, VY = 0;
            foreach (var VYData in narrowVYOilDataList)
            {
                if (VYData.ColumnIndex <= colIndex && float.TryParse(VYData.calShowData, out VY))
                {
                    SUM += VY;
                }
            }

            return SUM;
        }
        /// <summary>
        /// 渣油表的TVY联补充
        /// </summary>
        private void ResidueTVYDataCorrectionSupplement()
        {
            #region "TVY=VY+TVY(NCUTS(NCUTS.ECP=RES.ICP))"
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            for (int i = 0; i < this._maxCol; i++)
            {
                string TVYcal = string.Empty;

                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string strTVY = FunNarrowStartReturnItemCodeValue(ICPcal, "ECP", "TVY");

                float TVY = 0, VY = 0;

                if (float.TryParse(strTVY, out TVY) && float.TryParse(VYcal, out VY))
                {
                    TVYcal = (VY + TVY).ToString();
                }

                if (TVYcal != string.Empty && VYcal != "非数字")
                {
                    this._gridOil.SetData("TVY", i, TVYcal);
                }
            }

            #endregion
        }
        /// <summary>
        /// 渣油表的CCR联补充
        /// </summary>
        private void ResidueCCRDataCorrectionSupplement()
        {
            List<OilDataEntity> CCROilDataList = this._gridOil.GetDataByRowItemCode("CCR");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();          
            if (wholeOilDataList== null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeCCROilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "CCR").FirstOrDefault();
            if (wholeCCROilData == null)
                return;
            if (wholeCCROilData.calData == string.Empty)
                return;

            float WholeCCR = 0;
            if (float.TryParse(wholeCCROilData.calData, out WholeCCR))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string CCRcal = string.Empty;

                    #region " "                  
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        CCRcal = (WholeCCR * 100 / WY).ToString();
                    }                  
                    #endregion

                    if (CCRcal != string.Empty && CCRcal != "非数字")
                        this._gridOil.SetData("CCR", i, CCRcal);
                }
            }           
        }
        /// <summary>
        /// 渣油表的FE联补充
        /// </summary>
        private void ResidueFEDataCorrectionSupplement()
        {
            List<OilDataEntity> FEOilDataList = this._gridOil.GetDataByRowItemCode("FE");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeFEOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "FE").FirstOrDefault();
            if (wholeFEOilData == null)
                return;
            if (wholeFEOilData.calData == string.Empty)
                return;

            float WholeFE = 0;
            if (float.TryParse(wholeFEOilData.calData, out WholeFE))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string FEcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        FEcal = (WholeFE * 100 / WY).ToString();
                    }
                    #endregion

                    if (FEcal != string.Empty && FEcal != "非数字")
                        this._gridOil.SetData("FE", i, FEcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的NI联补充
        /// </summary>
        private void ResidueNIDataCorrectionSupplement()
        {
            List<OilDataEntity> NIOilDataList = this._gridOil.GetDataByRowItemCode("NI");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeNIOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "NI").FirstOrDefault();
            if (wholeNIOilData == null)
                return;
            if (wholeNIOilData.calData == string.Empty)
                return;

            float WholeNI = 0;
            if (float.TryParse(wholeNIOilData.calData, out WholeNI))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string NIcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        NIcal = (WholeNI * 100 / WY).ToString();
                    }
                    #endregion

                    if (NIcal != string.Empty && NIcal != "非数字")
                        this._gridOil.SetData("NI", i, NIcal);
                }
            }
        }    
        /// <summary>
        /// 渣油表的V联补充
        /// </summary>
        private void ResidueVDataCorrectionSupplement()
        {
            List<OilDataEntity> NIOilDataList = this._gridOil.GetDataByRowItemCode("V");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeVOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "V").FirstOrDefault();
            if (wholeVOilData == null)
                return;
            if (wholeVOilData.calData == string.Empty)
                return;

            float WholeV = 0;
            if (float.TryParse(wholeVOilData.calData, out WholeV))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string Vcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        Vcal = (WholeV * 100 / WY).ToString();
                    }
                    #endregion

                    if (Vcal != string.Empty && Vcal != "非数字")
                        this._gridOil.SetData("V", i, Vcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的CA联补充
        /// </summary>
        private void ResidueCADataCorrectionSupplement()
        {
            List<OilDataEntity> CAOilDataList = this._gridOil.GetDataByRowItemCode("CA");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeCAOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "CA").FirstOrDefault();
            if (wholeCAOilData == null)
                return;
            if (wholeCAOilData.calData == string.Empty)
                return;

            float WholeCA = 0;
            if (float.TryParse(wholeCAOilData.calData, out WholeCA))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string CAcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        CAcal = (WholeCA * 100 / WY).ToString();
                    }
                    #endregion

                    if (CAcal != string.Empty && CAcal != "非数字")
                        this._gridOil.SetData("CA", i, CAcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的NA联补充
        /// </summary>
        private void ResidueNADataCorrectionSupplement()
        {
            List<OilDataEntity> NAOilDataList = this._gridOil.GetDataByRowItemCode("NA");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeNAOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "NA").FirstOrDefault();
            if (wholeNAOilData == null)
                return;
            if (wholeNAOilData.calData == string.Empty)
                return;

            float WholeNA = 0;
            if (float.TryParse(wholeNAOilData.calData, out WholeNA))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string NAcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        NAcal = (WholeNA * 100 / WY).ToString();
                    }
                    #endregion

                    if (NAcal != string.Empty && NAcal != "非数字")
                        this._gridOil.SetData("NA", i, NAcal);
                }
            }
        }
        /// <summary>
        /// 渣油表的RES联补充
        /// </summary>
        private void ResidueRESDataCorrectionSupplement()
        {
            List<OilDataEntity> RESOilDataList = this._gridOil.GetDataByRowItemCode("RES");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeRESOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "RES").FirstOrDefault();
            if (wholeRESOilData == null)
                return;
            if (wholeRESOilData.calData == string.Empty)
                return;

            float WholeRES = 0;
            if (float.TryParse(wholeRESOilData.calData, out WholeRES))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string REScal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        REScal = (WholeRES * 100 / WY).ToString();
                    }
                    #endregion

                    if (REScal != string.Empty && REScal != "非数字")
                        this._gridOil.SetData("RES", i, REScal);
                }
            }
        }
        /// <summary>
        /// 渣油表的APH联补充
        /// </summary>
        private void ResidueAPHDataCorrectionSupplement()
        {
            List<OilDataEntity> APHOilDataList = this._gridOil.GetDataByRowItemCode("APH");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> wholeOilDataList = this._wholeGridOil.GetAllData();
            if (wholeOilDataList == null)
                return;
            if (wholeOilDataList.Count <= 0)
                return;

            OilDataEntity wholeAPHOilData = wholeOilDataList.Where(o => o.OilTableRow.itemCode == "APH").FirstOrDefault();
            if (wholeAPHOilData == null)
                return;
            if (wholeAPHOilData.calData == string.Empty)
                return;

            float WholeAPH = 0;
            if (float.TryParse(wholeAPHOilData.calData, out WholeAPH))
            {
                for (int i = 0; i < this._maxCol; i++)
                {
                    string APHcal = string.Empty;

                    #region " "
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    float WY = 0;
                    if (float.TryParse(WYcal, out WY))
                    {
                        APHcal = (WholeAPH * 100 / WY).ToString();
                    }
                    #endregion

                    if (APHcal != string.Empty && APHcal != "非数字")
                        this._gridOil.SetData("APH", i, APHcal);
                }
            }
        }

        /// <summary>
        /// 渣油表的D20联补充
        /// </summary>
        private void ResidueD20DataCorrectionSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");

            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            for (int i = 0; i < this._maxCol; i++)
            {
                string D20cal = string.Empty;

                #region "D20=WY/VY*D20(原油)"

                if (D20cal == string.Empty)
                {
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);

                    var ds = this._wholeGridOil.GetAllData();//D20(CRU)
                    OilDataEntity oilDataD20Whole = ds.Where(o => o.OilTableRow.itemCode == "D20").FirstOrDefault();//选出原油性质的D20校正值 

                    if (oilDataD20Whole == null)
                        continue;

                    D20cal = BaseFunction.FunD20fromWY_VY_WholeD20(WYcal, VYcal, oilDataD20Whole.calData);
                }

                #endregion

                if (D20cal != string.Empty && D20cal != "非数字")
                    this._gridOil.SetData("D20", i, D20cal);
            }
        }

        /// <summary>
        /// 渣油表的D15联补充
        /// </summary>
        private void ResidueD15DataCorrectionSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            #region "数据赋值"

            for (int i = 0; i < this._maxCol; i++)
            {
                string D15cal = string.Empty;

                #region "D20->D15"

                if (D15cal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                    D15cal = BaseFunction.FunD15fromD20(D20cal);
                }

                #endregion

                if (D15cal != string.Empty && D15cal != "非数字")
                {
                    this._gridOil.SetData("D15", i, D15cal);
                }
            }

            #endregion
        }
        /// <summary>
        /// 渣油表的D60联补充
        /// </summary>
        private void ResidueD60DataCorrectionSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            #region "数据赋值"

            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string D60cal = string.Empty;

                #region "D20->D60"

                if (D60cal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                    D60cal = BaseFunction.FunD60fromD20(D20cal);
                }

                #endregion

                if (D60cal != string.Empty && D60cal != "非数字")
                    this._gridOil.SetData("D60", i, D60cal);
            }//END for

            #endregion
        }
        /// <summary>
        /// 渣油表的API联补充
        /// </summary>
        private void ResidueAPIDataCorrectionSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");

            for (int i = 0; i < this._maxCol; i++)
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
                    this._gridOil.SetData("API", i, APIcal);

                #endregion
            }
        }
        #endregion

        #region "渣油表关联补充"
        /// <summary>
        /// 渣油表的WY联补充
        /// </summary>
        private void ResidueGridOilWYLinkSupplement()
        {
            WYLinkSupplement();//
            ResidueGridOilWYAccumulateSupplement();
        }
        /// <summary>
        /// 累计值补充"WY=100-TWY(NCUTS(NCUTS.ECP=RES.ICP))"
        /// </summary>
        private void ResidueGridOilWYAccumulateSupplement()
        {
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");

            #region "WY=100-TWY(NCUTS(NCUTS.ECP=RES.ICP))"
            for (int i = 0; i < this._maxCol; i++)
            {
                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                if (WYcal != string.Empty)
                    continue;

                if (WYcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string strTWY = FunNarrowStartReturnItemCodeValue(ICPcal, "ECP", "TWY");

                    float TWY = 0;

                    if (float.TryParse(strTWY, out TWY))
                    {
                        WYcal = (100 - TWY).ToString();
                    }
                }
                if (WYcal != string.Empty && WYcal != "非数字")
                    this._gridOil.SetData("WY", i, WYcal);
            }

            #endregion
        }
        /// <summary>
        /// 渣油表的TWY联补充
        /// </summary>
        private void ResidueGridOilTWYLinkSupplement()
        {
            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");

            #region "TWY=WY+TWY(NCUTS (NCUTS.ECP=RES.ICP))"
            for (int i = 0; i < this._maxCol; i++)
            {
                string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);
                if (TWYcal != string.Empty)
                    continue;

                if (TWYcal == string.Empty)
                {
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string strTWY = FunNarrowStartReturnItemCodeValue(ICPcal, "ECP", "TWY");

                    float TWY = 0, WY = 0;

                    if (float.TryParse(strTWY, out TWY) && float.TryParse(WYcal, out WY))
                    {
                        TWYcal = (WY + TWY).ToString();
                    }
                }
                if (TWYcal != string.Empty && TWYcal != "非数字")
                    this._gridOil.SetData("TWY", i, TWYcal);
            }

            #endregion
        }
        /// <summary>
        /// 渣油表的VY联补充
        /// </summary>
        private void ResidueGridOilVYLinkSupplement()
        {
            VYLinkSupplement();
            ResidueGridOilVYAccumulateSupplement();           
        }

        /// <summary>
        /// 累计值补充  "VY=100-TVY(NCUTS(NCUTS.ECP=RES.ICP))"
        /// </summary>
        private void ResidueGridOilVYAccumulateSupplement()
        {
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            #region "VY=100-TVY(NCUTS(NCUTS.ECP=RES.ICP))"
            for (int i = 0; i < this._maxCol; i++)
            {
                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);

                if (VYcal != string.Empty)
                    continue;
                if (VYcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string strTVY = FunNarrowStartReturnItemCodeValue(ICPcal, "ECP", "TVY");

                    float TVY = 0;

                    if (float.TryParse(strTVY, out TVY))
                    {
                        VYcal = (100 - TVY).ToString();
                    }
                }
                if (VYcal != string.Empty && VYcal != "非数字")
                    this._gridOil.SetData("VY", i, VYcal);
            }

            #endregion
        }
        /// <summary>
        /// 渣油表的TVY联补充
        /// </summary>
        private void ResidueGridOilTVYLinkSupplement()
        {
            List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            #region "TVY=VY+TVY(NCUTS(NCUTS.ECP=RES.ICP))"
            for (int i = 0; i < this._maxCol; i++)
            {
                string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);
                if (TVYcal != string.Empty)
                    continue;

                if (TVYcal == string.Empty)
                {
                    string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string strTVY = FunNarrowStartReturnItemCodeValue(ICPcal, "ECP", "TVY");

                    float TVY = 0, VY = 0;

                    if (float.TryParse(strTVY, out TVY) && float.TryParse(VYcal, out VY))
                    {
                        TVYcal = (VY + TVY).ToString();
                    }
                }
                if (TVYcal != string.Empty && TVYcal != "非数字")
                    this._gridOil.SetData("TVY", i, TVYcal);
            }

            #endregion
        }

        /// <summary>
        /// 渣油表的API关联补充
        /// </summary>
        private void ResidueGridOilAPILinkSupplement()
        {
            APILinkSupplement();
        }
        /// <summary>
        /// 渣油表的D20关联补充
        /// </summary>
        private void ResidueGridOilD20LinkSupplement()
        {
            ResidueD20LinkSupplement();
            ResidueGridOilMixSupplementAccumulate("D20");
            
        }
        /// <summary>
        /// 渣油表的D15关联补充
        /// </summary>
        private void ResidueGridOilD15LinkSupplement()
        {
            D15LinkSupplement();
            ResidueGridOilMixSupplementAccumulate("D15");
            
        }
        /// <summary>
        /// 渣油表的D60关联补充
        /// </summary>
        private void ResidueGridOilD60LinkSupplement()
        {
            D60LinkSupplement();
            ResidueGridOilMixSupplementAccumulate("D60");
            
        }
        /// <summary>
        /// 渣油表的V02、V04、V05、V08、V10关联补充
        /// </summary>
        private void ResidueGridOilV0LinkSupplement()
        {
            V0_LinkSupplement();
            ResidueGridOilMixSupplementAccumulate("V02");
            ResidueGridOilMixSupplementAccumulate("V04");
            ResidueGridOilMixSupplementAccumulate("V05");
            ResidueGridOilMixSupplementAccumulate("V08");
            ResidueGridOilMixSupplementAccumulate("V10");
           

            V0_Condition();
        }
        /// <summary>
        /// 渣油表的VI联补充
        /// </summary>
        private void ResidueGridOilVILinkSupplement()
        {
            VI_ResidueLinkSupplement();
        }
        /// <summary>
        /// 渣油表的VG4联补充
        /// </summary>
        private void ResidueGridOilVG4LinkSupplement()
        {
            VG4LinkSupplement();
        }
        /// <summary>
        /// 渣油表的V1G联补充
        /// </summary>
        private void ResidueGridOilV1GLinkSupplement()
        {
            V1GLinkSupplement();
        }
        
        /// <summary>
        /// 渣油表的POR联补充
        /// </summary>
        private void ResidueGridOilPORLinkSupplement()
        {
            PORLinkSupplement();
            //ResidueGridOilMixSupplementAccumulate("POR");//2012.10.21和SOP一起屏蔽
        }
        
        /// <summary>
        /// 渣油表的SOP联补充
        /// </summary>
        private void ResidueGridOilSOPLinkSupplement()
        {
            SOPLinkSupplement();
            //ResidueGridOilMixSupplementAccumulate("SOP");//2012.10.21和POR一起屏蔽
        }

        /// <summary>
        /// 渣油表的NIV联补充
        /// </summary>
        private void ResidueGridOilNIVLinkSupplement()
        {
            #region "关联补充"
            List<OilDataEntity> NIVOilDataList = this._gridOil.GetDataByRowItemCode("NIV");

            List<OilDataEntity> NIOilDataList = this._gridOil.GetDataByRowItemCode("NI");
            List<OilDataEntity> VOilDataList = this._gridOil.GetDataByRowItemCode("V");


            for (int i = 0; i < this._maxCol; i++)
            {
                string NIVcal = getStrValuefromOilDataEntity(NIVOilDataList, i);                
                #region "NIV赋值"
                if (NIVcal != string.Empty)
                    continue;

                if (NIVcal == string.Empty)
                {
                    string NIcal = getStrValuefromOilDataEntity(NIOilDataList, i);
                    string Vcal = getStrValuefromOilDataEntity(VOilDataList, i);

                    NIVcal = BaseFunction.FunNIVfromNI_V(NIcal, Vcal);
                }

                if (NIVcal != string.Empty)
                    this._gridOil.SetData("NIV", i, NIVcal);
                #endregion
            }
            #endregion

        }
        /// <summary>
        /// 渣油表的MW联补充
        /// </summary>
        private void ResidueGridOilMWLinkSupplement()
        {
            #region "关联补充"
            List<OilDataEntity> MWVOilDataList = this._gridOil.GetDataByRowItemCode("MW");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            for (int i = 0; i < this._maxCol; i++)
            {
                string MWcal = getStrValuefromOilDataEntity(MWVOilDataList, i);
                #region "MW赋值"
                if (MWcal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("MW", i, MWcal);
                #endregion
            }
            #endregion

            ResidueGridOilMixSupplementAccumulate("MW");
        }
        /// <summary>
        /// 渣油表的C/H联补充
        /// </summary>
        private void ResidueGridOilC_HLinkSupplement()
        {
            List<OilDataEntity> C_HOilDataList = this._gridOil.GetDataByRowItemCode("C/H");

            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            for (int i = 0; i < this._maxCol; i++)
            {
                string C_Hcal = getStrValuefromOilDataEntity(C_HOilDataList, i);
                if (C_Hcal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("C/H", i, C_Hcal);
            }
        }
        /// <summary>
        /// 渣油表的FFA,CII,TCC,RTT,RNN, CA#,RAA联补充
        /// </summary>
        private void ResidueGridOilFFA_CII_TCC_CA_RNN_RAA_RTTLinkSupplement()
        {
            FFA_CII_TCC_CA_RNN_RAA_RTTLinkSupplement();
        }
        /// <summary>
        /// 渣油表的KFC联补充
        /// </summary>
        private void ResidueGridOilKFCLinkSupplement()
        {
            KFCResidueLinkSupplement();
        }

        #endregion 

        #endregion         

        #region "五个表的关联补充公共函数"
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
            strResult = OilData == null ? string.Empty : OilData.calShowData;

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

            strResult = oilData == null ? string.Empty : oilData.calShowData;

            return strResult;
        }
        /// <summary>
        /// D20->API
        /// D60->API
        /// </summary>
        private void APILinkSupplement()
        {           
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");

            if (D60OilDataList == null && D20OilDataList == null && D60OilDataList.Count <= 0 && D20OilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity APIOilData = APIOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string APIcal = APIOilData == null ? string.Empty : APIOilData.calData;
              
                #region

                if (APIcal != string.Empty)
                    continue;

                if (APIcal == string.Empty)//存在D20实测值
                {
                    OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;
                    APIcal = BaseFunction.FunAPIfromD20(D20cal);
                }

                if (APIcal == string.Empty )//存在D60实测值
                {
                    OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string D60cal = D60OilData == null ? string.Empty : D60OilData.calData;
                    APIcal = BaseFunction.FunAPIfromD60(D60cal);
                }

                if (APIcal != string.Empty&&APIcal!="非数字")
                {
                    this._gridOil.SetData("API", i, APIcal);
                }
                #endregion
            }
        }

        /// <summary>
        ///D60->D20
        ///API->D20
        ///SG->D20
        /// </summary>
        private void D20LinkSupplement()
        {
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> ECPOilDataList = this._narrowGridOil.GetDataByRowItemCode("ECP");

            if (D60OilDataList == null && SGOilDataList == null && APIOilDataList == null && D60OilDataList.Count <= 0 && APIOilDataList.Count <= 0 && SGOilDataList == null)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                float? fD20 = getCalByItemCode_Column("D20",i);
                string D20cal = fD20 == null ? string.Empty : fD20.ToString();
                #region

                if (D20cal != string.Empty)
                    continue;

                if (i == 0)
                {
                    #region "i == 0"
                    OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string ECPcal = ECPOilData == null ? string.Empty : ECPOilData.calData;

                    float ECP = 0;
                    if (float.TryParse(ECPcal, out ECP))
                    {
                        if (ECP <= 15)
                        {
                            List<OilDataEntity> Datas = this._lightGridOil.GetAllData();
                            List<OilDataEntity> lightDatas = Datas.Where(o => o.OilTableCol.colCode == "Cut1" && o.calData != string.Empty).ToList();

                            if (lightDatas != null)
                            {
                                if (lightDatas.Count > 0)//表示轻端不为空
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
                    #endregion
                }

                if (D20cal != string.Empty && D20cal != "非数字")
                {
                    this._gridOil.SetData("D20", i, D20cal);
                }

                #endregion
            }
        }

        /// <summary>
        /// D60->D20
        ///API->D20
        ///SG->D20
        /// </summary>
        private void ResidueD20LinkSupplement()
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
                string D20cal  = D20OilData == null ? string.Empty : D20OilData.calData;
               // D20cal = D20cal == null ? string.Empty : D20cal.ToString();
                #region
                
                if (D20cal != string.Empty)
                    continue;                

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
                    this._gridOil.SetData("D20", i, D20cal);
                }

                #endregion
            }
        }
        /// <summary>
        /// D20->D15
        /// </summary>
        private void D15LinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");
            if (D20OilDataList == null && D20OilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D15OilData = D15OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string D15cal = D15OilData == null ? string.Empty : D15OilData.calData;

                #region
                if (D15cal != string.Empty)
                    continue;
                 
                if (D15cal == string.Empty)
                {
                    OilDataEntity D20OilData = this._gridOil.GetDataByRowItemCodeColumnIndex("D20", i);
                    string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;
                    D15cal = BaseFunction.FunD15fromD20(D20cal);
                }
                if (D15cal != string.Empty && D15cal != "非数字")
                {
                    this._gridOil.SetData("D15", i, D15cal);
                }
                #endregion
            }
        }
        /// <summary>
        ///D20->D60
        ///API->D60
        /// </summary>
        private void D60LinkSupplement()
        {
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");

            if (D20OilDataList == null&& APIOilDataList == null && D20OilDataList.Count <= 0 && APIOilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity D60OilData = D60OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string D60cal = D60OilData == null ? string.Empty : D60OilData.calData;
                #region
                if (D60cal != string.Empty)
                    continue;

              
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

                if (D60cal != string.Empty && D60cal!="非数字")
                    this._gridOil.SetData("D60", i, D60cal);

                #endregion
            }
        }
        /// <summary>
        /// D20->SG;D60->S
        /// </summary>
        private void SGLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");
            List<OilDataEntity> D60OilDataList = this._gridOil.GetDataByRowItemCode("D60");
            for (int i = 0; i < this._maxCol; i++)
            {
                string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);

                #region
                if (SGcal != string.Empty)
                    continue;
 
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

                if (SGcal != string.Empty && SGcal!="非数字")
                    this._gridOil.SetData("SG", i, SGcal);

                #endregion
            }
        }

        /// <summary>
        /// 宽馏分补充V-2、 V02、V04、V05、V08、V10   V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度
        /// </summary>
        private void V0_LinkSupplement()
        {
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode ("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode ("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode ("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode ("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode ("V10");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity V02OilData = V02OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                OilDataEntity V04OilData = V04OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                OilDataEntity V05OilData = V05OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                OilDataEntity V08OilData = V08OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();

                string V02cal = V02OilData == null ? string.Empty : V02OilData.calData;
                string V04cal = V04OilData == null ? string.Empty : V04OilData.calData;
                string V05cal = V05OilData == null ? string.Empty : V05OilData.calData;
                string V08cal = V08OilData == null ? string.Empty : V08OilData.calData;
                string V10cal = V10OilData == null ? string.Empty : V10OilData.calData; 
                List<VT> VTList = new List<VT>();

                /* V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度*/
                #region

                #region "VTList"

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
                if (!string.IsNullOrWhiteSpace(V10cal))
                {
                    VT newVT = new VT(V10cal, 100);
                    VTList.Add(newVT);
                }

                #endregion

                #region "V02cal"
                if (V02cal == string.Empty)
                {
                    if (V02cal == string.Empty && VTList.Count >= 2)
                    {
                        V02cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "20");
                    }

                    if (V02cal != string.Empty && V02cal!="非数字")
                        this._gridOil.SetData("V02", i, V02cal);
                }

                #endregion

                #region  "V04cal"

                if (V04cal == string.Empty)
                {                     
                    if (V04cal == string.Empty && VTList.Count >= 2)
                    {
                        V04cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "40");
                    }
                    if (V04cal != string.Empty && V04cal != "非数字")
                        this._gridOil.SetData("V04", i, V04cal);
                }
                #endregion

                #region  "V05cal"

                if (V05cal == string.Empty)
                {
                    if (V05cal == string.Empty && VTList.Count >= 2)
                    {
                        V05cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "50");
                    }

                    if (V05cal != string.Empty && V05cal != "非数字")
                        this._gridOil.SetData("V05", i, V05cal);
                }

                #endregion

                #region  "V08cal"

                if (V08cal == string.Empty)
                {
                    if (V08cal == string.Empty && VTList.Count >= 2)
                    {
                        V08cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "80");
                    }

                    if (V08cal != string.Empty && V08cal != "非数字")
                        this._gridOil.SetData("V08", i, V08cal);
                }

                #endregion

                #region  "V10cal"

                if (V10cal == string.Empty)
                {
                    if (V10cal == string.Empty && VTList.Count >= 2)
                    {
                        V10cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "100");
                    }

                    if (V10cal != string.Empty && V10cal != "非数字")
                        this._gridOil.SetData("V10", i, V10cal);
                }

                #endregion

                #endregion
            }
        }
        /// <summary>
        /// V10,SG=>KF
        /// </summary>
        private void KFCWholeLinkSupplement()
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
                string KFCcal = KFCOilData == null ? string.Empty : KFCOilData.calData;  
            
                #region

                if (KFCcal != string.Empty)
                    continue;

                OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;  

                if (KFCcal == string.Empty)
                {
                    OilDataEntity V05OilData = V05OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string V05cal = V05OilData == null ? string.Empty : V05OilData.calData;  

                    KFCcal = BaseFunction.FunKFCfromV05_D20(V05cal, D20cal);
                }
                if (KFCcal == string.Empty)
                {
                    OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string V10cal = V10OilData == null ? string.Empty : V10OilData.calData;  
                    KFCcal = BaseFunction.FunKFCfromV10_D20(V10cal, D20cal);
                }
                if (KFCcal == string.Empty)
                {
                    OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string V10cal = V10OilData == null ? string.Empty : V10OilData.calData;

                    OilDataEntity SGOilData = SGOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string SGcal = SGOilData == null ? string.Empty : SGOilData.calData;  
                    KFCcal = BaseFunction.FunKFCfromV10_SG(V10cal, SGcal);
                }
                if (KFCcal != string.Empty && KFCcal != "非数字")
                    this._gridOil.SetData("KFC", i, KFCcal);
                #endregion
            }
        }
        /// <summary>
        /// NI + V=>NIV
        /// </summary>
        private void NIVLinkSupplement()
        {
            List<OilDataEntity> NIOilDataList = this._gridOil.GetDataByRowItemCode("NI");
            List<OilDataEntity> VOilDataList = this._gridOil.GetDataByRowItemCode("V");
            List<OilDataEntity> NIVOilDataList = this._gridOil.GetDataByRowItemCode("NIV");

            if (NIOilDataList == null && VOilDataList == null && NIOilDataList.Count <= 0 && VOilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                string NIVcal = getStrValuefromOilDataEntity(NIVOilDataList, i);
                #region

                if (NIVcal != string.Empty)
                    continue;             

                if (NIVcal == string.Empty)
                {
                    string NIcal = getStrValuefromOilDataEntity(NIOilDataList, i);
                    string Vcal = getStrValuefromOilDataEntity(VOilDataList, i);
                    NIVcal = BaseFunction.FunNIVfromNI_V(NIcal, Vcal);
                }

                if (NIVcal != string.Empty && NIVcal != "非数字")
                    this._gridOil.SetData("NIV", i, NIVcal);
                #endregion
            }
        }
        /// <summary>
        ///V10,D20 =>KFC
        /// </summary>
        private void KFCResidueLinkSupplement()
        {
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> KFCOilDataList = this._gridOil.GetDataByRowItemCode("KFC");

            if (D20OilDataList == null && V10OilDataList == null && D20OilDataList.Count <= 0 && V10OilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity KFCOilData = KFCOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string KFCcal = KFCOilData == null ? string.Empty : KFCOilData.calData;  

                #region

                if (KFCcal != string.Empty)
                    continue;
                
                if (KFCcal == string.Empty)
                {
                    OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;

                    OilDataEntity V10OilData = V10OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string V10cal = V10OilData == null ? string.Empty : V10OilData.calData;

                    KFCcal = BaseFunction.FunKFCfromV10_D20(V10cal, D20cal);
                }
                if (KFCcal != string.Empty && KFCcal != "非数字")
                    this._gridOil.SetData("KFC", i, KFCcal);
                #endregion
            }
        }
        /// <summary>
        /// MCP ,D20 =>KFC
        /// ICP,ECP ,D20 =>KFC
        /// </summary>
        private void KFCLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> KFCOilDataList = this._gridOil.GetDataByRowItemCode("KFC");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity KFCOilData = KFCOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string KFCcal = KFCOilData == null ? string.Empty : KFCOilData.calData; 


                OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;

                /* V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度*/
                #region " KFC赋值"
 
                if (KFCcal == string.Empty)
                {
                    OilDataEntity MCPOilData = MCPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string MCPcal = MCPOilData == null ? string.Empty : MCPOilData.calData; 

                    KFCcal = BaseFunction.FunKFCfromMCP_D20(MCPcal, D20cal);
                }
                if (KFCcal == string.Empty)
                {
                    OilDataEntity ICPOilData = ICPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string ICPcal = ICPOilData == null ? string.Empty : ICPOilData.calData;

                    OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string ECPcal = ECPOilData == null ? string.Empty : ECPOilData.calData; 

                    KFCcal = BaseFunction.FunKFCfromICPECP_D20(ICPcal, ECPcal, D20cal);
                }

                if (KFCcal != string.Empty && KFCcal != "非数字")
                    this._gridOil.SetData("KFC", i, KFCcal);

                #endregion
            }
        }
        /// <summary>
        /// MCP ,D20 =>KFC
        /// A10, A30,A50, A70, A90, D20 =>KFC
        /// </summary>
        private void KFC_WideLinkSupplement()
        {
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> KFCOilDataList = this._gridOil.GetDataByRowItemCode("KFC");

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity KFCOilData = KFCOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string KFCcal = KFCOilData == null ? string.Empty : KFCOilData.calData;


                OilDataEntity D20OilData = D20OilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string D20cal = D20OilData == null ? string.Empty : D20OilData.calData;
                /* V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度*/
                #region

                if (KFCcal != string.Empty)
                    continue;               

                if (KFCcal == string.Empty)
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);

                    KFCcal = BaseFunction.FunKFCfromA10A30A50A70A90_D20(A10cal, A30cal, A50cal, A70cal, A90cal, D20cal);
                }

                if (KFCcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                    KFCcal = BaseFunction.FunKFCfromMCP_D20(MCPcal, D20cal);
                }

                if (KFCcal != string.Empty && KFCcal != "非数字")
                    this._gridOil.SetData("KFC", i, KFCcal);
                #endregion
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
            List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");


            for (int i = 0; i < this._maxCol; i++)
            {
                string VYcal = getStrValuefromOilDataEntity(VYOilDataList,i);
                 
                if (VYcal != string.Empty)
                    continue;

                /*VY(i)=WY(i)/D20(i)*D20(原油)*/
                #region
              
                if (VYcal == string.Empty)
                {
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);                   
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                    var ds = this._wholeGridOil.GetAllData();
                    OilDataEntity oilDataD20Whole = ds.Where(o => o.OilTableRow.itemCode == "D20").FirstOrDefault();//选出原油性质的D20校正值 

                    if (oilDataD20Whole == null)
                        continue;

                    VYcal = BaseFunction.FunVY(WYcal, D20cal, oilDataD20Whole.calData);
                }

                if (VYcal != string.Empty && VYcal != "非数字")
                    this._gridOil.SetData("VY", i, VYcal);

                #endregion
            }

            for (int i = 0; i < this._maxCol; i++)
            {
                /*VY(i)=TVY(i)-TVY(i-1) */
                #region "数据补充"

                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                if (VYcal != string.Empty)
                    continue;

                if (i == 0)
                {
                    if (VYcal == string.Empty)
                    {
                        string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);
                        if (TVYcal != "0")
                        {
                            VYcal = TVYcal;
                        }
                    }
                }
                else
                {
                    if (VYcal == string.Empty)
                    {
                        string beforTWYcal = getStrValuefromOilDataEntity(TVYOilDataList, i - 1);
                        string TWYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);

                        if (beforTWYcal != string.Empty && TWYcal != string.Empty)
                        {
                            float TWYbefore = 0;
                            float WY = 0;
                            float TWY = 0;

                            if (float.TryParse(beforTWYcal, out TWYbefore) && float.TryParse(TWYcal, out TWY))
                            {
                                WY = TWY - TWYbefore;
                                if (WY != 0)
                                {
                                    VYcal = WY.ToString();
                                }
                            }
                        }
                    }
                }

                if (VYcal != string.Empty && VYcal != "非数字")
                    this._gridOil.SetData("VY", i, VYcal);
                #endregion
            }
        }
        /// <summary>
        /// D15,V04->VG4
        /// </summary>
        private void VG4LinkSupplement()
        {
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> VG4OilDataList = this._gridOil.GetDataByRowItemCode("VG4");

            for (int i = 0; i < this._maxCol; i++)
            {
                string VG4cal = getStrValuefromOilDataEntity(VG4OilDataList, i);   
                
                /*D15,V04->VG4*/
                #region
                if (VG4cal != string.Empty)
                    continue;
                
                if (VG4cal == string.Empty)
                {
                    string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                    string D15cal = getStrValuefromOilDataEntity(D15OilDataList, i);
                    VG4cal = BaseFunction.FunVG4fromD15andV04(D15cal, V04cal);
                }
                if (VG4cal != string.Empty && VG4cal != "非数字")
                    this._gridOil.SetData("VG4", i, VG4cal);
                #endregion
            }
        }
        /// <summary>
        /// D15,V10 ->V1G
        /// </summary>
        private void V1GLinkSupplement()
        {
            List<OilDataEntity> D15OilDataList = this._gridOil.GetDataByRowItemCode("D15");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> V1GOilDataList = this._gridOil.GetDataByRowItemCode("V1G");

            for (int i = 0; i < this._maxCol; i++)
            {
                string V1Gcal = getStrValuefromOilDataEntity(V1GOilDataList, i);
                /*D15,V10 ->V1G*/
                #region
                if (V1Gcal != string.Empty)
                    continue;
 
                if (V1Gcal == string.Empty)
                {
                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                    string D15cal = getStrValuefromOilDataEntity(D15OilDataList, i);

                    V1Gcal = BaseFunction.FunV1GfromD15andV10(D15cal, V10cal);
                }
                if (V1Gcal != string.Empty && V1Gcal != "非数字")
                    this._gridOil.SetData("V1G", i, V1Gcal);
                #endregion
            }
        }
        /// <summary>
        /// POR=SOP+3
        /// </summary>
        private void PORLinkSupplement()
        {
            List<OilDataEntity> SOPOilDataList = this._gridOil.GetDataByRowItemCode("SOP");
            List<OilDataEntity> POROilDataList = this._gridOil.GetDataByRowItemCode("POR");


            for (int i = 0; i < this._maxCol; i++)
            {
                string PORcal = getStrValuefromOilDataEntity(POROilDataList, i);
                /*POR=SOP+3*/
                if (PORcal != string.Empty)
                    continue;
                
                if (PORcal == string.Empty)
                {
                    string SOPcal = getStrValuefromOilDataEntity(SOPOilDataList, i);
                    PORcal = BaseFunction.FunPOR(SOPcal);
                }

                if (PORcal != string.Empty && PORcal != "非数字")
                    this._gridOil.SetData("POR", i, PORcal);
            }
        }
        /// <summary>
        /// SOP=POR-3
        /// </summary>
        private void SOPLinkSupplement()
        {
            List<OilDataEntity> SOPOilDataList = this._gridOil.GetDataByRowItemCode("SOP");
            List<OilDataEntity> POROilDataList = this._gridOil.GetDataByRowItemCode("POR");

            for (int i = 0; i < this._maxCol; i++)
            {
                string SOPcal = getStrValuefromOilDataEntity(SOPOilDataList, i);
                /*SOP=POR-3*/
                if (SOPcal != string.Empty)
                    continue;
                
                if (SOPcal == string.Empty)
                {
                    string PORcal = getStrValuefromOilDataEntity(POROilDataList, i);
                    SOPcal = BaseFunction.FunSOP(PORcal);
                }

                if (SOPcal != string.Empty && SOPcal != "非数字")
                    this._gridOil.SetData("SOP", i, SOPcal);
            }
        }
        /// <summary>
        /// D20,MW,CAR,H2-->FFA,CII,TCC,RTT,RNN, CA,RAA
        /// </summary>
        private void FFA_CII_TCC_CA_RNN_RAA_RTTLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");

            List<OilDataEntity> FFAOilDataList = this._gridOil.GetDataByRowItemCode("FFA");
            List<OilDataEntity> CIIOilDataList = this._gridOil.GetDataByRowItemCode("CII");
            List<OilDataEntity> TCCOilDataList = this._gridOil.GetDataByRowItemCode("TCC");
            List<OilDataEntity> RTTOilDataList = this._gridOil.GetDataByRowItemCode("RTT");
            List<OilDataEntity> RNNOilDataList = this._gridOil.GetDataByRowItemCode("RNN");
            List<OilDataEntity> CA_OilDataList = this._gridOil.GetDataByRowItemCode("CA#");
            List<OilDataEntity> RAAOilDataList = this._gridOil.GetDataByRowItemCode("RAA");

            for (int i = 0; i < this._maxCol; i++)
            {
                #region
                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);

                string FFAcal = getStrValuefromOilDataEntity(FFAOilDataList, i);
                string CIIcal = getStrValuefromOilDataEntity(CIIOilDataList, i);
                string TCCcal = getStrValuefromOilDataEntity(TCCOilDataList, i);
                string CA_cal = getStrValuefromOilDataEntity(CA_OilDataList, i);
                string RNNcal = getStrValuefromOilDataEntity(RNNOilDataList, i);
                string RAAcal = getStrValuefromOilDataEntity(RAAOilDataList, i);
                string RTTcal = getStrValuefromOilDataEntity(RTTOilDataList, i);    
                #endregion

                Dictionary <string ,float> DIC = BaseFunction.FunFFA_CII_TCC_CA_RNN_RAA_RTTfromD20_MW_CAR_H2(D20cal, MWcal, CARcal, H2cal);
                
                #region  
                if (DIC.Keys.Contains("FFA") && FFAcal == string.Empty)                  
                    FFAcal = DIC["FFA"].ToString();
                if (DIC.Keys.Contains("CII") && CIIcal == string.Empty)
                    CIIcal = DIC["CII"].ToString();
                if (DIC.Keys.Contains("TCC") && TCCcal == string.Empty)
                    TCCcal = DIC["TCC"].ToString();
                if (DIC.Keys.Contains("RTT") && RTTcal == string.Empty)
                    RTTcal = DIC["RTT"].ToString();
                if (DIC.Keys.Contains("CA#") && CA_cal == string.Empty)
                    CA_cal = DIC["CA#"].ToString();
                if (DIC.Keys.Contains("RAA") && RAAcal == string.Empty)
                    RAAcal = DIC["RAA"].ToString();
                if (DIC.Keys.Contains("RNN") && RNNcal == string.Empty)
                    RNNcal = DIC["RNN"].ToString();

                if (FFAcal != string.Empty && FFAcal != "非数字")
                    this._gridOil.SetData("FFA", i, FFAcal);
                if (CIIcal != string.Empty && CIIcal != "非数字")
                    this._gridOil.SetData("CII", i, CIIcal);
                if (TCCcal != string.Empty && TCCcal != "非数字")
                    this._gridOil.SetData("TCC", i, TCCcal);
                if (RTTcal != string.Empty && RTTcal != "非数字")
                    this._gridOil.SetData("RTT", i, RTTcal);
                if (CA_cal != string.Empty && CA_cal != "非数字")
                    this._gridOil.SetData("CA#", i, CA_cal);
                if (RAAcal != string.Empty && RAAcal != "非数字")
                    this._gridOil.SetData("RAA", i, RAAcal);
                if (RNNcal != string.Empty && RNNcal != "非数字")
                    this._gridOil.SetData("RNN", i, RNNcal);
                #endregion
            }
        }
        /// <summary>
        /// WY(i)=VY(i)*D20(i)/D20(原油)
        /// </summary>
        private void NarrowWYLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");

            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");

            for (int i = 0; i < this._maxCol; i++)
            {
                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                
                /*WY(i)=VY(i)*D20(i)/D20(原油)*/
                #region

                if (WYcal != string.Empty)
                    continue;
 
                if (WYcal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);

                    var ds = this._wideGridOil.GetAllData();
                    OilDataEntity oilDataD20Whole = ds.Where(o => o.OilTableRow.itemCode == "D20").FirstOrDefault();//选出原油性质的D20校正值 

                    if (oilDataD20Whole == null)
                        continue;

                    WYcal = BaseFunction.FunWY(VYcal, D20cal, oilDataD20Whole.calData);
                }

                if (WYcal != string.Empty && WYcal != "非数字")
                    this._gridOil.SetData("WY", i, WYcal);

                #endregion
            }

            for (int i = 0; i < this._maxCol; i++)
            {
                /*WY(i)=TWY(i)-TWY(i-1) */
                #region "数据补充"

                string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                if (WYcal != string.Empty)
                    continue;         

                if (i == 0)
                {
                    if (WYcal == string.Empty)
                    {
                        string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);
                        if (TWYcal != "0")
                        {
                            WYcal = TWYcal;
                        }
                    }
                }
                else
                {
                    if (WYcal == string.Empty)
                    {
                        string beforTWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i - 1);
                        string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);

                        if (beforTWYcal != string.Empty && TWYcal != string.Empty)
                        {
                            float TWYbefore = 0;
                            float WY = 0;
                            float TWY = 0;

                            if (float.TryParse(beforTWYcal, out TWYbefore) && float.TryParse(TWYcal, out TWY))
                            {
                                WY = TWY - TWYbefore;
                                if (WY != 0)
                                {
                                    WYcal = WY.ToString();
                                }
                            }
                        }
                    }
                }

                if (WYcal != string.Empty && WYcal != "非数字")
                    this._gridOil.SetData("WY", i, WYcal);
                #endregion
            }
        }

        /// <summary>
        /// TWY(i)=TWY(i-1)+WY(i)
        /// </summary>
        private void TWYLinkSupplement()
        {          
            for (int i = 0; i < this._maxCol; i++)
            {
                List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
                List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
                string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);

                if (TWYcal != string.Empty)
                    continue;

                /*TWY(i)=TWY(i-1)+WY(i)*/
                #region "数据补充"

                if (i == 0)
                {
                    if (TWYcal == string.Empty)
                    {
                        string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                        if (WYcal != "0")
                        {
                            TWYcal = WYcal;
                        }
                    }
                }
                else
                {
                    if (TWYcal == string.Empty)
                    {
                        string beforTWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i-1);
                        string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);

                        TWYcal = BaseFunction.FunTWY(beforTWYcal, WYcal); ;
                    }
                }

                #endregion

                if (TWYcal != string.Empty && TWYcal != "非数字")
                    this._gridOil.SetData("TWY", i, TWYcal);
            }
        }
        /// <summary>
        /// 窄馏分TWY补充 TWY(i)=TWY(i-1)+WY(i)
        /// </summary>
        private void narrowTWYLinkSupplement()
        {
            for (int i = 0; i < this._maxCol; i++)
            {
                List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
                List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
                string TWYcal = getStrValuefromOilDataEntity(TWYOilDataList, i);

                if (TWYcal != string.Empty)
                    continue;

                /*TWY(i)=NCUTS(TWY(ECP))+WY(i)*/
                #region "数据补充"

                if (i == 0)
                {
                    if (TWYcal == string.Empty)
                    {
                        string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                        if (WYcal != "0")
                        {
                            TWYcal = WYcal;
                        }
                    }
                }
                else
                {
                    if (TWYcal == string.Empty)
                    {
                        string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);//获取宽馏分中对应的WY值
                        string TWYbefore = getStrValuefromOilDataEntity(TWYOilDataList, i - 1);
                        TWYcal = BaseFunction.FunTWY(TWYbefore, WYcal); ;
                    }
                }

                #endregion
                if (TWYcal != string.Empty && TWYcal != "非数字")
                    this._gridOil.SetData("TWY", i, TWYcal);
            }
        }
        /// <summary>
        /// 窄馏分TVY补充—TVY=TVY(i-1)+VY(i)
        /// </summary>
        private void narrowTVYLinkSupplement()
        {
            for (int i = 0; i < this._maxCol; i++)
            {
                List<OilDataEntity> TVYOilDataList = this._narrowGridOil.GetDataByRowItemCode("TVY");
                List<OilDataEntity> VYOilDataList = this._narrowGridOil.GetDataByRowItemCode("VY");
                string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);
             
                if (TVYcal != string.Empty)
                    continue;

                /*TVY(i)=TVY(i-1)+VY(i)*/
                #region "数据补充"
                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                if (i == 0)
                {
                    if (TVYcal == string.Empty && VYcal != string.Empty)
                    {
                        TVYcal = VYcal;
                    }
                }
                else
                {
                    if (TVYcal == string.Empty)
                    {
                        string TVYbefore = getStrValuefromOilDataEntity(TVYOilDataList, i - 1);
                        TVYcal = BaseFunction.FunTWY(TVYbefore, VYcal); ;
                    }
                }
                #endregion

                if (TVYcal != string.Empty && TVYcal != "非数字")
                    this._gridOil.SetData("TVY", i, TVYcal);
            }
        }

        /// <summary>
        /// TVY(i)=TVY(ICP)+VY(i)
        /// </summary>
        private void TVYLinkSupplement()
        {
            for (int i = 0; i < this._maxCol; i++)
            {
                List<OilDataEntity> TVYOilDataList = this._gridOil.GetDataByRowItemCode("TVY");
                List<OilDataEntity> VYOilDataList = this._gridOil.GetDataByRowItemCode("VY");
                string TVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i);

                if (TVYcal != string.Empty)
                    continue;

                /*TVY(i)=TVY(i-1)+VY(i)*/
                #region "数据补充"
                string VYcal = getStrValuefromOilDataEntity(VYOilDataList, i);
                if (i == 0)
                {
                    if (TVYcal == string.Empty && VYcal != string.Empty)
                    {
                        TVYcal = VYcal;
                    }
                }
                else
                {
                    if (TVYcal == string.Empty && VYcal != string.Empty)
                    {
                        string beforTVYcal = getStrValuefromOilDataEntity(TVYOilDataList, i-1);
                        TVYcal = BaseFunction.FunTVY(beforTVYcal, VYcal);
                    }
                }
                #endregion

                if (TVYcal != string.Empty && TVYcal != "非数字")
                    this._gridOil.SetData("TVY", i, TVYcal);
            }
        }
        /// <summary>
        /// ACD,D20->NET NET= ACD/D20/100
        /// </summary>
        private void NETLinkSupplement()
        {
            List<OilDataEntity> ACDOilDataList = this._gridOil.GetDataByRowItemCode("ACD");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> NETOilDataList = this._gridOil.GetDataByRowItemCode("NET");

            for (int i = 0; i < this._maxCol; i++)
            {
                string NETcal = getStrValuefromOilDataEntity(NETOilDataList, i);
                if (NETcal != string.Empty)
                    continue;

                if (NETcal == string.Empty)
                {
                    string ACDcal = getStrValuefromOilDataEntity(ACDOilDataList, i);
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                    NETcal = BaseFunction.FunNET(ACDcal, D20cal);
                }
                if (NETcal != string.Empty && NETcal != "非数字")
                    this._gridOil.SetData("NET", i, NETcal);
            }
        }
        /// <summary>
        /// 窄馏分补充R70->R20
        /// </summary>
        //private void R20LinkSupplement()
        //{
        //    if (R20Row == null)
        //        return;

        //    for (int i = 0; i < this._maxCol; i++)
        //    {
        //        string R20lab = this._gridOil[this._cols[i].ColumnIndex - 1, R20Row[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex - 1, R20Row[0].RowIndex].Value.ToString() : string.Empty;
        //        string R20cal = this._gridOil[this._cols[i].ColumnIndex, R20Row[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, R20Row[0].RowIndex].Value.ToString() : string.Empty;
        //        string R70cal = this._gridOil[this._cols[i].ColumnIndex, R70Row[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, R70Row[0].RowIndex].Value.ToString() : string.Empty;
        //        if (R20cal == string.Empty && R20lab != string.Empty)
        //        {
        //            R20cal = R20lab;
        //        }
        //        if (R20cal == string.Empty && R20lab == string.Empty && R70cal != string.Empty)
        //        {
        //            R20cal = BaseFunction.
        //        }
        //        if (R20cal != string.Empty)
        //        {
        //            oilDataEdit.OilDataSupplementPaste(R20cal, this._cols[i].ColumnIndex, R20Row[0].RowIndex);

        //        }
        //    }
        //}
        /// <summary>
        /// 窄馏分补充R20->R70
        /// </summary>
        //private void R70LinkSupplement()
        //{
        //    for (int i = 0; i < this._maxCol; i++)
        //    {
        //        string R70lab = this._gridOil[this._cols[i].ColumnIndex - 1, R70Row[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex - 1, R70Row[0].RowIndex].Value.ToString() : string.Empty;
        //        string R70cal = this._gridOil[this._cols[i].ColumnIndex, R70Row[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, R70Row[0].RowIndex].Value.ToString() : string.Empty;
        //        string R20cal = this._gridOil[this._cols[i].ColumnIndex, R20Row[0].RowIndex].Value != null ? this._gridOil[this._cols[i].ColumnIndex, R20Row[0].RowIndex].Value.ToString() : string.Empty;
        //        if (R70cal == string.Empty && R70lab != string.Empty)
        //        {
        //            R70cal = R70lab;
        //        }
        //        if (R70cal == string.Empty && R70lab == string.Empty && R20cal != string.Empty)
        //        {
        //            R70cal = R20cal;
        //        }
        //        if (R70cal != string.Empty)
        //        {
        //            oilDataEdit.OilDataSupplementPaste(R70cal, this._cols[i].ColumnIndex, R70Row[0].RowIndex);                    
        //        }
        //    }
        //}
        /// <summary>
        /// BMI=(48640/(Tx+273))+473.7*(D60/DH2O60F)-456.8
        /// NCUTS:  Tx=(ICP+ECP)/2
        /// </summary>
        private void BMI_NarrowLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> BMIOilDataList = this._gridOil.GetDataByRowItemCode("BMI");

            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            for (int i = 0; i < this._maxCol; i++)
            {              
                string BMIcal = getStrValuefromOilDataEntity(BMIOilDataList, i);

                if (BMIcal != string.Empty)
                    continue;
                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                if (BMIcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    BMIcal = BaseFunction.FunBMIfromMCP_D20(MCPcal, D20cal);
                }

                if (BMIcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                    BMIcal = BaseFunction.FunBMIfromICPECP_D20(ICPcal, ECPcal, D20cal);
                }
                if (BMIcal != string.Empty && BMIcal != "非数字")
                    this._gridOil.SetData("BMI", i, BMIcal);
            }
        }
        /// <summary>
        /// BMI=(48640/(Tx+273))+473.7*(D60/DH2O60F)-456.8
        /// Tx=(A10+A30+A50+A70+A90)/5
        /// </summary>
        private void BMI_WideLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> BMIOilDataList = this._gridOil.GetDataByRowItemCode("BMI");

            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            for (int i = 0; i < this._maxCol; i++)
            {
                string BMIcal = getStrValuefromOilDataEntity(BMIOilDataList, i);

                if (BMIcal != string.Empty)
                    continue;
                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                ///Tx=(A10+A30+A50+A70+A90)/5 //宽馏分

                if (BMIcal == string.Empty)
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);
                    BMIcal = BaseFunction.FunBMIfromA10A30A50A70A90_D20(A10cal, A30cal, A50cal, A70cal, A90cal, D20cal);
                }

                if (BMIcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    BMIcal = BaseFunction.FunBMIfromMCP_D20(MCPcal, D20cal);
                }

                if (BMIcal != string.Empty && BMIcal != "非数字")
                    this._gridOil.SetData("BMI", i, BMIcal);
            }
        }
        /// <summary>
        /// ANI->DI 
        /// DI==API*(9/5*ANI+32)/100
        /// </summary>
        private void DILinkSupplement()
        {
            List<OilDataEntity> DIOilDataList = this._gridOil.GetDataByRowItemCode("DI");
            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            for (int i = 0; i < this._maxCol; i++)
            {
                string DIcal = getStrValuefromOilDataEntity(DIOilDataList, i);

                if (DIcal != string.Empty)
                    continue;

                /// DI==API*(9/5*ANI+32)/100
                if (DIcal == string.Empty)
                {
                    string ANIcal = getStrValuefromOilDataEntity(ANIOilDataList, i);
                    string APIcal = getStrValuefromOilDataEntity(APIOilDataList, i);
                    DIcal = BaseFunction.FunDI(APIcal, ANIcal);
                }
                if (DIcal != string.Empty && DIcal!="非数字")
                    this._gridOil.SetData("DI", i, DIcal);
            }
        }
        /// <summary>
        /// D20,Tx->CI
        /// </summary>
        private void CI_NarrowLinkSupplement()
        {
            List<OilDataEntity> CIOilDataList = this._gridOil.GetDataByRowItemCode("CI");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");

            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                string CIcal = getStrValuefromOilDataEntity(CIOilDataList, i);

                if (CIcal != string.Empty)
                    continue;

                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                if (CIcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    CIcal = BaseFunction.FunCIfromMCP_D20(MCPcal, D20cal);
                }

                if (CIcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                    CIcal = BaseFunction.FunCIfromICPECP_D20(ICPcal, ECPcal, D20cal);
                }

                if (CIcal != string.Empty && CIcal != "非数字")
                    this._gridOil.SetData("CI", i, CIcal);
            }
        }
        /// <summary>
        /// D20,Tx->CI
        /// </summary>
        private void CI_WideLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> CIOilDataList = this._gridOil.GetDataByRowItemCode("CI");

            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");

            for (int i = 0; i < this._maxCol; i++)
            {
                string CIcal = getStrValuefromOilDataEntity(CIOilDataList, i);

                if (CIcal != string.Empty)
                    continue;

                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                ///Tx=(A10+A30+A50+A70+A90)/5 //宽馏分
                if (CIcal == string.Empty)
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);
                    CIcal = BaseFunction.FunCIfromA10A30A50A70A90_D20(A10cal, A30cal, A50cal, A70cal, A90cal, D20cal);
                }
                if (CIcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    CIcal = BaseFunction.FunCIfromMCP_D20(MCPcal, D20cal);
                }

                if (CIcal != string.Empty && CIcal != "非数字")
                    this._gridOil.SetData("CI", i, CIcal);
            }
        }
        /// <summary>
        /// D20->D70
        /// </summary>
        private void D70_WideLinkSupplement()
        {
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");

            for (int i = 0; i < this._maxCol; i++)
            {
                string D70cal = getStrValuefromOilDataEntity(D70OilDataList, i);

                if (D70cal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    D70cal = BaseFunction.FunD70fromD20(D20cal);
                }
                if (D70cal != string.Empty && D70cal != "非数字")
                    this._gridOil.SetData("D70", i, D70cal);
            }
        }
        /// <summary>
        /// 宽馏分补充V-2、 V02、V04、V05、V08、V10   V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度
        /// </summary>
        private void V0_WideLinkSupplement()
        {
            List<OilDataEntity> V_2OilDataList = this._gridOil.GetDataByRowItemCode("V-2");
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");


            for (int i = 0; i < this._maxCol; i++)
            {
                string V_2cal = getStrValuefromOilDataEntity(V_2OilDataList, i);
                string V02cal = getStrValuefromOilDataEntity(V02OilDataList, i);
                string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                string V05cal = getStrValuefromOilDataEntity(V05OilDataList, i);
                string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                List<VT> VTList = new List<VT>();

                /* V3=f3(V1,V2,t1,t2,t)已知任意两温度点下的粘度，求第三温度点的粘度*/
                #region
                #region//VTList
                if (V_2cal != string.Empty)
                {
                    VT newVT = new VT(V_2cal, -20);
                    VTList.Add(newVT);
                }
                if (V02cal != string.Empty)
                {
                    VT newVT = new VT(V02cal, 20);
                    VTList.Add(newVT);
                }
                if (V04cal != string.Empty)
                {
                    VT newVT = new VT(V04cal, 40);
                    VTList.Add(newVT);
                }
                if (V05cal != string.Empty)
                {
                    VT newVT = new VT(V05cal, 50);
                    VTList.Add(newVT);
                }
                if (V08cal != string.Empty)
                {
                    VT newVT = new VT(V08cal, 80);
                    VTList.Add(newVT);
                }
                if (V10cal != string.Empty)
                {
                    VT newVT = new VT(V10cal, 100);
                    VTList.Add(newVT);
                }
                #endregion
                #region//V_2cal

                if (V_2cal == string.Empty && VTList.Count >= 2)
                {
                    V_2cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "-20");
                }
                if (V_2cal != string.Empty)
                    this._gridOil.SetData("V-2", i, V_2cal);
                #endregion
                #region //V02cal
                if (V02cal == string.Empty && VTList.Count >= 2)
                {
                    V02cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "20");
                }
                if (V02cal != string.Empty)
                    this._gridOil.SetData("V02", i, V02cal);
                #endregion
                #region  //V04cal

                if (V04cal == string.Empty && VTList.Count >= 2)
                {
                     V04cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "40");                   
                }
                if (V04cal != string.Empty)
                    this._gridOil.SetData("V04", i, V04cal);
                #endregion
                #region  //V05cal

                if (V05cal == string.Empty && VTList.Count >= 2)
                {
                     V05cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "50");
                }
                if (V05cal != string.Empty)
                    this._gridOil.SetData("V05", i, V05cal);

                #endregion
                #region  //V08cal
                if (V08cal == string.Empty && VTList.Count >= 2)
                {
                     V08cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "80");
                }
                if (V08cal != string.Empty)
                    this._gridOil.SetData("V08", i, V08cal);
                #endregion
                #region  //V10cal
                if (V10cal == string.Empty && VTList.Count >= 2)
                {
                    V10cal = BaseFunction.FunV(VTList[0].V.ToString(), VTList[1].V.ToString(), VTList[0].T.ToString(), VTList[1].T.ToString(), "100");
                }
                if (V10cal != string.Empty)
                    this._gridOil.SetData("V10", i, V10cal);
                #endregion
                #endregion
            }
        }
        /// <summary>
        ///VI关联补充V04,V10->VI
        /// </summary>
        private void VI_WideLinkSupplement()
        {
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> VIOilDataList = this._gridOil.GetDataByRowItemCode("VI");

            for (int i = 0; i < this._maxCol; i++)
            {
                string VIcal = getStrValuefromOilDataEntity(VIOilDataList, i);

                if (VIcal != string.Empty)
                    continue;

                if (VIcal == string.Empty)
                {
                    string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);
                    string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);

                    VIcal = BaseFunction.FunVIfromV04_V10(V04cal, V10cal);
                }
                if (VIcal != string.Empty && VIcal != "非数字")
                    this._gridOil.SetData("VI", i, VIcal);
            }
        }
        /// <summary>
        ///VI关联补充V04,V10->VI
        ///V08,V10->VI
        /// </summary>
        private void VI_ResidueLinkSupplement()
        {
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> VIOilDataList = this._gridOil.GetDataByRowItemCode("VI");

            for (int i = 0; i < this._maxCol; i++)
            {
                string VIcal = getStrValuefromOilDataEntity(VIOilDataList, i);

                if (VIcal != string.Empty)
                    continue;

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
                    this._gridOil.SetData("VI", i, VIcal);
            }
        }
        /// <summary>
        ///C/H关联补充
        ///CAR/H2=>C/H
        ///SG, MCP?C/H; D20,MCP=>C/H; 
        ///D20, A10, A30, A50, A70,A90=>C/H?
        /// </summary>
        private void C_H_WideLinkSupplement()
        {
            List<OilDataEntity> C_HOilDataList = this._gridOil.GetDataByRowItemCode("C/H");
            List<OilDataEntity> CAROilDataList = this._gridOil.GetDataByRowItemCode("CAR");
            List<OilDataEntity> H2OilDataList = this._gridOil.GetDataByRowItemCode("H2");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");


            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            for (int i = 0; i < this._maxCol; i++)
            {
                string C_Hcal = getStrValuefromOilDataEntity(C_HOilDataList, i);
              
                if (C_Hcal != string.Empty)
                    continue;
                               
                if (C_Hcal == string.Empty)
                {
                    string CARcal = getStrValuefromOilDataEntity(CAROilDataList, i);
                    string H2cal = getStrValuefromOilDataEntity(H2OilDataList, i);
                    C_Hcal = BaseFunction.FunC_H(CARcal, H2cal);
                }

                if (C_Hcal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);

                    C_Hcal = BaseFunction.FunC1HfromD20_A10_A30_A50_A70_A90(D20cal, A10cal, A30cal, A50cal, A70cal, A90cal);
                }

                if (C_Hcal == string.Empty)
                {
                    string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    C_Hcal = BaseFunction.FunC1HfromSG_MCP(SGcal, MCPcal);
                }

                if (C_Hcal == string.Empty)
                {
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    C_Hcal = BaseFunction.FunC1HfromD20_MCP(D20cal, MCPcal);
                }


                if (C_Hcal != string.Empty && C_Hcal != "非数字")
                    this._gridOil.SetData("C/H", i, C_Hcal);
            }
        }
        /// <summary>
        /// D20,A10, A30, A50, A90->CEN
        /// </summary>
        private void CEN_WideLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            //List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            List<OilDataEntity> CENOilDataList = this._gridOil.GetDataByRowItemCode("CEN");

            for (int i = 0; i < this._maxCol; i++)
            {
                string CENcal = getStrValuefromOilDataEntity(CENOilDataList, i);

                if (CENcal != string.Empty)
                    continue;

                if (CENcal == string.Empty)
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    //string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);
                    string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                    CENcal = BaseFunction.FunCENfromA10A30A50A90_D20(A10cal, "", A50cal, A90cal, D20cal);
                }
                if (CENcal != string.Empty && CENcal != "非数字")
                    this._gridOil.SetData("CEN", i, CENcal);
            }
        }
        /// <summary>
        /// 宽馏分的CPP_RAA 关联补充 
        /// D20,R20,MW,SUL-->?CPP,CNN,CAA,RTT,RNN,RAA
        /// </summary>
        private void CPP_RAA_WideLinkSupplement1()
        {
          
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> R20OilDataList = this._gridOil.GetDataByRowItemCode("R20");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");

            List<OilDataEntity> CPPOilDataList = this._gridOil.GetDataByRowItemCode("CPP");
            List<OilDataEntity> CNNOilDataList = this._gridOil.GetDataByRowItemCode("CNN");
            List<OilDataEntity> CAAOilDataList = this._gridOil.GetDataByRowItemCode("CAA");
            List<OilDataEntity> RTTOilDataList = this._gridOil.GetDataByRowItemCode("RTT");
            List<OilDataEntity> RNNOilDataList = this._gridOil.GetDataByRowItemCode("RNN");
            List<OilDataEntity> RAAOilDataList = this._gridOil.GetDataByRowItemCode("RAA");
            for (int i = 0; i < this._maxCol; i++)
            {
               
                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                string R20cal = getStrValuefromOilDataEntity(R20OilDataList, i);
                string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                string CPPcal = getStrValuefromOilDataEntity(CPPOilDataList, i);
                string CNNcal = getStrValuefromOilDataEntity(CNNOilDataList, i);
                string CAAcal = getStrValuefromOilDataEntity(CAAOilDataList, i);
                string RTTcal = getStrValuefromOilDataEntity(RTTOilDataList, i);
                string RNNcal = getStrValuefromOilDataEntity(RNNOilDataList, i);
                string RAAcal = getStrValuefromOilDataEntity(RAAOilDataList, i);

                #region " 计算 "
                string strRAAcal = string.Empty;
                string strRNNcal = string.Empty;
                string strRTTcal = string.Empty;
                string strCAAcal = string.Empty;
                string strCNNcal = string.Empty;
                string strCPPcal = string.Empty;
                Dictionary<string,float>DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(D20cal, R20cal, MWcal, SULcal);
                 
                if (DIC.Keys.Contains ("RAA"))
                    strRAAcal =DIC["RAA"].ToString();
                if (DIC.Keys.Contains ("RNN"))
                    strRNNcal = DIC["RNN"].ToString();
                if (DIC.Keys.Contains ("RTT"))
                    strRTTcal = DIC["RTT"].ToString();
                if (DIC.Keys.Contains ("CAA"))
                    strCAAcal = DIC["CAA"].ToString();
                if (DIC.Keys.Contains ("CNN"))
                    strCNNcal = DIC["CNN"].ToString();
                if (DIC.Keys.Contains ("CPP"))
                    strCPPcal = DIC["CPP"].ToString();                  
                 
                #endregion

                #region "赋值"
                if (CPPcal == string.Empty)
                    CPPcal = strCPPcal;

                if (CPPcal != string.Empty && CPPcal != "非数字")
                    this._gridOil.SetData("CPP", i, CPPcal);

                if (CNNcal == string.Empty)
                    CNNcal = strCNNcal;

                if (CNNcal != string.Empty && CNNcal != "非数字")
                    this._gridOil.SetData("CNN", i, CNNcal);

                if (CAAcal == string.Empty)
                    CAAcal = strCAAcal;
                if (CAAcal != string.Empty && CAAcal != "非数字")
                    this._gridOil.SetData("CAA", i, CAAcal);

                if (RTTcal == string.Empty)
                    RTTcal = strRTTcal;

                if (RTTcal != string.Empty && RTTcal != "非数字")
                    this._gridOil.SetData("RTT", i, RTTcal);

                if (RNNcal == string.Empty)
                    RNNcal = strRNNcal;
                if (RNNcal != string.Empty && RNNcal != "非数字")
                    this._gridOil.SetData("RNN", i, RNNcal);


                if (RAAcal == string.Empty)
                    RAAcal = strRAAcal;
                if (RAAcal != string.Empty && RAAcal != "非数字")
                    this._gridOil.SetData("RAA", i, RAAcal);
                #endregion
            }
        }
        /// <summary>
        /// 宽馏分的CPP_RAA 关联补充 
        /// D70,R70,MW,SUL-->CPP,CNN,CAA,RTT,RNN,RAA
        /// </summary>
        private void CPP_RAA_WideLinkSupplement2()
        {
            List<OilDataEntity> D70OilDataList = this._gridOil.GetDataByRowItemCode("D70");
            List<OilDataEntity> R70OilDataList = this._gridOil.GetDataByRowItemCode("R70");
            List<OilDataEntity> MWOilDataList = this._gridOil.GetDataByRowItemCode("MW");
            List<OilDataEntity> SULOilDataList = this._gridOil.GetDataByRowItemCode("SUL");

            List<OilDataEntity> CPPOilDataList = this._gridOil.GetDataByRowItemCode("CPP");
            List<OilDataEntity> CNNOilDataList = this._gridOil.GetDataByRowItemCode("CNN");
            List<OilDataEntity> CAAOilDataList = this._gridOil.GetDataByRowItemCode("CAA");
            List<OilDataEntity> RTTOilDataList = this._gridOil.GetDataByRowItemCode("RTT");
            List<OilDataEntity> RNNOilDataList = this._gridOil.GetDataByRowItemCode("RNN");
            List<OilDataEntity> RAAOilDataList = this._gridOil.GetDataByRowItemCode("RAA");
            for (int i = 0; i < this._maxCol; i++)
            {
                string D70cal = getStrValuefromOilDataEntity(D70OilDataList, i);
                string R70cal = getStrValuefromOilDataEntity(R70OilDataList, i);
                string MWcal = getStrValuefromOilDataEntity(MWOilDataList, i);
                string SULcal = getStrValuefromOilDataEntity(SULOilDataList, i);

                string CPPcal = getStrValuefromOilDataEntity(CPPOilDataList, i);
                string CNNcal = getStrValuefromOilDataEntity(CNNOilDataList, i);
                string CAAcal = getStrValuefromOilDataEntity(CAAOilDataList, i);
                string RTTcal = getStrValuefromOilDataEntity(RTTOilDataList, i);
                string RNNcal = getStrValuefromOilDataEntity(RNNOilDataList, i);
                string RAAcal = getStrValuefromOilDataEntity(RAAOilDataList, i);

                #region " 计算 "
                string strRAAcal = string.Empty;
                string strRNNcal = string.Empty;
                string strRTTcal = string.Empty;
                string strCAAcal = string.Empty;
                string strCNNcal = string.Empty;
                string strCPPcal = string.Empty;
                Dictionary<string, float> DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(D70cal, R70cal, MWcal, SULcal);

                if (DIC.Keys.Contains("RAA"))
                    strRAAcal = DIC["RAA"].ToString();
                if (DIC.Keys.Contains("RNN"))
                    strRNNcal = DIC["RNN"].ToString();
                if (DIC.Keys.Contains("RTT"))
                    strRTTcal = DIC["RTT"].ToString();
                if (DIC.Keys.Contains("CAA"))
                    strCAAcal = DIC["CAA"].ToString();
                if (DIC.Keys.Contains("CNN"))
                    strCNNcal = DIC["CNN"].ToString();
                if (DIC.Keys.Contains("CPP"))
                    strCPPcal = DIC["CPP"].ToString();

                #endregion

                #region "赋值"
                if (CPPcal == string.Empty)
                    CPPcal = strCPPcal;

                if (CPPcal != string.Empty && CPPcal != "非数字")
                    this._gridOil.SetData("CPP", i, CPPcal);

                if (CNNcal == string.Empty)
                    CNNcal = strCNNcal;

                if (CNNcal != string.Empty && CNNcal != "非数字")
                    this._gridOil.SetData("CNN", i, CNNcal);

                if (CAAcal == string.Empty)
                    CAAcal = strCAAcal;
                if (CAAcal != string.Empty && CAAcal != "非数字")
                    this._gridOil.SetData("CAA", i, CAAcal);

                if (RTTcal == string.Empty)
                    RTTcal = strRTTcal;

                if (RTTcal != string.Empty && RTTcal != "非数字")
                    this._gridOil.SetData("RTT", i, RTTcal);

                if (RNNcal == string.Empty)
                    RNNcal = strRNNcal;
                if (RNNcal != string.Empty && RNNcal != "非数字")
                    this._gridOil.SetData("RNN", i, RNNcal);


                if (RAAcal == string.Empty)
                    RAAcal = strRAAcal;
                if (RAAcal != string.Empty && RAAcal != "非数字")
                    this._gridOil.SetData("RAA", i, RAAcal);
                #endregion
            }
        }
        /// <summary>
        /// 窄馏分补充R70->R20
        /// 补充A10->FPO
        /// </summary>
        private void FPOLinkSupplement()
        {
            List<OilDataEntity> FPOOilDataList = this._gridOil.GetDataByRowItemCode("FPO");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");

            for (int i = 0; i < this._maxCol; i++)
            {
                string FPOcal = getStrValuefromOilDataEntity(FPOOilDataList, i);
               
                if (FPOcal != string.Empty)
                    continue;
                //如果存在实验值和校正值为空，推测值的校正值不为空则通过公式计算
                if (FPOcal == string.Empty )
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    FPOcal = BaseFunction.FunFPO(A10cal);
                }

                //判断计算结果是否为空
                if (FPOcal != string.Empty)
                    this._gridOil.SetData("FPO", i, FPOcal);
            }
        }
        /// <summary>
        /// 窄馏分补充MCP->RVP
        /// </summary>
        private void RVP_NarrowLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> RVPOilDataList = this._gridOil.GetDataByRowItemCode("RVP");

            for (int i = 0; i < this._maxCol; i++)
            {
                string RVPcal = getStrValuefromOilDataEntity(RVPOilDataList, i);

                if (RVPcal != string.Empty)
                    continue;

                //如果存在实验值和校正值为空，推测值的校正值不为空则通过公式计算
                if (RVPcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                    RVPcal = BaseFunction.FunRVPfromMCP(MCPcal);
                }

                if (RVPcal != string.Empty && RVPcal != "非数字")
                    this._gridOil.SetData("RVP", i, RVPcal);
            }
        }
        /// <summary>
        /// 宽馏分表RVP关联补充
        /// </summary>
        private void RVP_WideLinkSupplement()
        {
            List<OilDataEntity> oilDataNarrow = this._narrowGridOil.GetAllData();

            if (oilDataNarrow == null)
                return;

            List<OilDataEntity> oilDataICPList = oilDataNarrow.Where(o => o.OilTableRow.itemCode == "ICP").ToList();//窄馏分数据
            List<OilDataEntity> oilDataECPList = oilDataNarrow.Where(o => o.OilTableRow.itemCode == "ECP").ToList();//窄馏分数据
            List<OilDataEntity> oilDataMCPList = oilDataNarrow.Where(o => o.OilTableRow.itemCode == "MCP").ToList();//窄馏分数据
            List<OilDataEntity> oilDataWYList = oilDataNarrow.Where(o => o.OilTableRow.itemCode == "WY").ToList();//窄馏分数据

            if (oilDataICPList == null || oilDataECPList == null || oilDataMCPList == null || oilDataWYList == null)
                return;

            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> RVPOilDataList = this._gridOil.GetDataByRowItemCode("RVP");

            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            for (int i = 0; i < this._maxCol; i++)
            {
                string RVPcal = getStrValuefromOilDataEntity(RVPOilDataList, i);

                if (RVPcal != string.Empty)
                    continue;

                if (RVPcal == string.Empty)
                {
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                    OilDataEntity ICPOilData = oilDataICPList.Where(o => o.calData == ICPcal).FirstOrDefault();
                    OilDataEntity ECPOilData = oilDataECPList.Where(o => o.calData == ECPcal).FirstOrDefault();

                    if (ICPOilData == null || ECPOilData == null)//在窄馏分中找不对应的ICP和ECP
                        continue;

                    if (ICPOilData.ColumnIndex > ECPOilData.ColumnIndex)
                        continue;

                    float sum = 0;
                    for (int j = ICPOilData.OilTableCol.colOrder; j <= ECPOilData.OilTableCol.colOrder; j++)
                    {
                        var MCPOilData = oilDataMCPList.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();//取出窄馏分中MCP的值
                        var WYOilData = oilDataWYList.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();//取出窄馏分中WY的值

                        if (MCPOilData == null || WYOilData == null)
                            break;

                        string strBI = BaseFunction.FunRVPfromMCP(MCPOilData.calData);

                        float BI = 0, WY = 0;
                        if (float.TryParse(strBI, out BI) && float.TryParse(WYOilData.calData, out WY))
                        {
                            sum += BI * WY;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (sum == 0)
                        continue;

                    float MCP = 0;
                    float WYWide = 0;
                    float fRVP = 0;
                    if (float.TryParse(MCPcal, out MCP) && float.TryParse(WYcal, out WYWide))
                    {
                        float F = (float)(1.0 + 0.003744 * (MCP - 93.3));
                        if (WYWide != 0)
                        {
                            fRVP = F * sum / WYWide;
                        }
                    }
                    RVPcal = fRVP != 0 ? fRVP.ToString() : string.Empty;
                }


                if (RVPcal != string.Empty && RVPcal != "非数字")
                    this._gridOil.SetData("RVP", i, RVPcal);
            }
        }
        /// <summary>
        /// 原油信息表CLA关联补充
        /// </summary>
        private void CLA_WholeLinkSupplement()
        {
            var APIData = this._wholeGridOil.GetDataByRowItemCodeColumnIndex("API", 0);
            string strAPI = APIData == null ? string.Empty : APIData.calData.ToString ();

            string strCLAcal = BaseFunction.FunCLA(strAPI);//计算出关联补充的值  

            if (strCLAcal != string.Empty)
            {            
                for (int i = 0; i < this._grdiOilInfo.Rows.Count; i++)
                {
                    DataGridViewRow row = this._grdiOilInfo.Rows[i];
                    if (row.Tag.ToString() == "CLA")
                    {
                        this._grdiOilInfo.Rows[i].Cells["itemValue"].Value = strCLAcal;
                        this._oilInfo.type = strCLAcal;
                    }
                }
            }
        }
        /// <summary>
        /// WYD(i)=WY(i)/(ECP(i)-ICP(i))
        /// </summary>
        private void WYDLinkSupplement()
        {
            List<OilDataEntity> WYDOilDataList = this._gridOil.GetDataByRowItemCode("WYD");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");

            for (int i = 0; i < this._maxCol; i++)
            {
                string WYDcal = getStrValuefromOilDataEntity(WYDOilDataList, i);

                if (WYDcal != string.Empty)
                    continue;

                if (WYDcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                    string WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);

                    WYDcal = BaseFunction.FunWYDfromICP_ECP_WY(ICPcal, ECPcal, WYcal);
                    if (WYDcal == "0")
                    {
                        WYDcal = string.Empty;
                    }
                }

                if (WYDcal != string.Empty && WYDcal != "非数字")
                    this._gridOil.SetData("WYD", i, WYDcal);
            }
        }
        /// <summary>
        /// MWY(i)=TWY(i-1)+WY(i)/2
        /// </summary>
        private void MWYLinkSupplement()
        {
            //MWY(i)=TWY(i-1)+WY(i)/2
            List<OilDataEntity> MWYOilDataList = this._gridOil.GetDataByRowItemCode("MWY");
            List<OilDataEntity> TWYOilDataList = this._gridOil.GetDataByRowItemCode("TWY");
            List<OilDataEntity> WYOilDataList = this._gridOil.GetDataByRowItemCode("WY");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MWYcal = getStrValuefromOilDataEntity(MWYOilDataList, i);

                if (MWYcal != string.Empty)
                    continue;

                if (MWYcal == string.Empty)
                {
                    string TWYcal = string.Empty;
                    string WYcal = string.Empty;
                    if (i == 0)
                    {
                        WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                        TWYcal = "0";
                    }
                    else
                    {
                        WYcal = getStrValuefromOilDataEntity(WYOilDataList, i);
                        TWYcal =  getStrValuefromOilDataEntity(TWYOilDataList, i-1);
                    }

                    MWYcal = BaseFunction.FunMWYfromTWY_WY(TWYcal, WYcal);                   
                }

                if (MWYcal != string.Empty && MWYcal != "非数字")
                    this._gridOil.SetData("MWY", i, MWYcal);
            }
        }
        /// <summary>
        /// MCP(i)=(ICP(i)+ECP(i))/2
        /// </summary>
        private void MCPLinkSupplement()
        {
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            for (int i = 0; i < this._maxCol; i++)
            {
                string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                if (MCPcal != string.Empty)
                    continue;

                if (MCPcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                    MCPcal = BaseFunction.FunMCPfromICP_ECP(ICPcal, ECPcal);
                }

                if (MCPcal != string.Empty && MCPcal != "非数字")
                    this._gridOil.SetData("MCP", i, MCPcal);
            }
        }
        /// <summary>
        ///  API,MCP=>SMK;
        ///  API,ICP, ECP =>SMK
        /// </summary>
        private void SMKLinkSupplement()
        {
            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> SMKOilDataList = this._gridOil.GetDataByRowItemCode("SMK");

            for (int i = 0; i < this._maxCol; i++)
            {

                string SMKcal = getStrValuefromOilDataEntity(SMKOilDataList, i);

                if (SMKcal != string.Empty)
                    continue;

                string APIcal = getStrValuefromOilDataEntity(APIOilDataList, i);

                if (SMKcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                    SMKcal = BaseFunction.FunSMKfromAPI_MCP(APIcal, MCPcal);
                }
                if (SMKcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                    SMKcal = BaseFunction.FunSMKfromAPI_ICP_ECP(APIcal, ICPcal, ECPcal);
                }

                if (SMKcal != string.Empty && SMKcal != "非数字")
                    this._gridOil.SetData("SMK", i, SMKcal);
            }
        }
        /// <summary>
        ///  API,MCP=>SMK;
        ///  D20,A10, A30, A50, A70 ,A90  =>SMK
        /// </summary>
        private void SMK_WideLinkSupplement()
        {

            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");
            List<OilDataEntity> SGOilDataList = this._gridOil.GetDataByRowItemCode("SG");

            List<OilDataEntity> APIOilDataList = this._gridOil.GetDataByRowItemCode("API");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            List<OilDataEntity> SMKOilDataList = this._gridOil.GetDataByRowItemCode("SMK");

            for (int i = 0; i < this._maxCol; i++)
            {

                string SMKcal = getStrValuefromOilDataEntity(SMKOilDataList, i);

                if (SMKcal != string.Empty)
                    continue;

                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                if (SMKcal == string.Empty)
                {
                    string ANIcal = getStrValuefromOilDataEntity(ANIOilDataList, i);
                    string SGcal = getStrValuefromOilDataEntity(SGOilDataList, i);
                    SMKcal = BaseFunction.FunSMKfromANI_SG(ANIcal, SGcal);
                }

                if (SMKcal == string.Empty)
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);
                    SMKcal = BaseFunction.FunSMKfromD20_A10_A30_A50_A70_A90(D20cal, A10cal, A30cal, A50cal, A70cal, A90cal);
                }

                if (SMKcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    string strAPI = BaseFunction.FunAPIfromD20(D20cal);
                    SMKcal = BaseFunction.FunSMKfromAPI_MCP(strAPI, MCPcal);
                }

                if (SMKcal != string.Empty && SMKcal != "非数字")
                    this._gridOil.SetData("SMK", i, SMKcal);
            }
        }

        /// <summary>
        ///  D20,MCP =>FRZ; 
        ///  D20,ICP, ECP =>FRZ
        /// </summary>
        private void FRZLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> FRZOilDataList = this._gridOil.GetDataByRowItemCode("FRZ");

            for (int i = 0; i < this._maxCol; i++)
            {
                string FRZcal = getStrValuefromOilDataEntity(FRZOilDataList, i);

                if (FRZcal != string.Empty)
                    continue;

                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                if (FRZcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                    FRZcal = BaseFunction.FunFRZfromD20_MCP(D20cal, MCPcal);
                }
                if (FRZcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                    FRZcal = BaseFunction.FunFRZfromD20_ICP_ECP(D20cal, ICPcal, ECPcal);
                }

                if (FRZcal != string.Empty && FRZcal != "非数字")
                    this._gridOil.SetData("FRZ", i, FRZcal);
            }
        }
        /// <summary>
        ///  D20,MCP =>FRZ; 
        ///  D20,A10, A30, A50, A70 ,A90 =>FRZ
        /// </summary>
        private void FRZWideLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            List<OilDataEntity> FRZOilDataList = this._gridOil.GetDataByRowItemCode("FRZ");


            for (int i = 0; i < this._maxCol; i++)
            {
                string FRZcal = getStrValuefromOilDataEntity(FRZOilDataList, i);

                if (FRZcal != string.Empty)
                    continue;

                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                if (FRZcal == string.Empty)
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);

                    FRZcal = BaseFunction.FunFRZfromD20_A10_A30_A50_A70_A90(D20cal, A10cal, A30cal, A50cal, A70cal, A90cal);
                }

                if (FRZcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);
                    FRZcal = BaseFunction.FunFRZfromD20_MCP(D20cal, MCPcal);
                }

                if (FRZcal != string.Empty && FRZcal != "非数字")
                    this._gridOil.SetData("FRZ", i, FRZcal);
            }
        }

        /// <summary>
        /// D20,MCP =>SAV, ARV
        ///D20, ICP, ECP  =>SAV, ARV
        /// </summary>
        private void SAV_ARVLinkSupplement()
        {
            List<OilDataEntity> SAVOilDataList = this._gridOil.GetDataByRowItemCode("SAV");
            List<OilDataEntity> ARVOilDataList = this._gridOil.GetDataByRowItemCode("ARV");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            for (int i = 0; i < this._maxCol; i++)
            {
                string ARVcal = getStrValuefromOilDataEntity(ARVOilDataList, i);
                string SAVcal = getStrValuefromOilDataEntity(SAVOilDataList, i);

                string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                Dictionary<string, float> SAV_ARVfromD20_MCP = BaseFunction.FunSAV_ARVfromD20_MCP(D20cal, MCPcal);
                Dictionary<string, float> SAV_ARVfromD20_ICP_ECP = BaseFunction.FunSAV_ARVfromD20_ICP_ECP(D20cal, ICPcal, ECPcal);

                #region " 补充ARV"

                if (ARVcal == string.Empty)
                {
                    if (ARVcal == string.Empty)
                    {
                        if (SAV_ARVfromD20_MCP.Keys.Contains("ARV"))
                            ARVcal = SAV_ARVfromD20_MCP["ARV"].ToString();
                    }
                    if (ARVcal == string.Empty)
                    {
                        if (SAV_ARVfromD20_MCP.Keys.Contains("ARV"))
                            ARVcal = SAV_ARVfromD20_ICP_ECP["ARV"].ToString();
                    }

                    if (ARVcal != string.Empty && ARVcal != "非数字")
                        this._gridOil.SetData("ARV", i, ARVcal);
                }

                #endregion

                #region " 补充SAV"

                if (SAVcal == string.Empty)
                {
                    if (SAVcal == string.Empty)
                    {
                        if (SAV_ARVfromD20_MCP.Keys.Contains("SAV"))
                            SAVcal = SAV_ARVfromD20_MCP["SAV"].ToString();
                    }
                    if (ARVcal == string.Empty)
                    {
                        if (SAV_ARVfromD20_MCP.Keys.Contains("SAV"))
                            SAVcal = SAV_ARVfromD20_ICP_ECP["SAV"].ToString();
                    }

                    if (SAVcal != string.Empty && SAVcal != "非数字")
                        this._gridOil.SetData("SAV", i, SAVcal);
                }
                #endregion
           }
        }
        /// <summary>
        /// D20,MCP =>SAV, ARV
        ///D20, ICP, ECP  =>SAV, ARV
        /// </summary>
        private void SAV_ARV_WideLinkSupplement()
        {
            List<OilDataEntity> SAVOilDataList = this._gridOil.GetDataByRowItemCode("SAV");
            List<OilDataEntity> ARVOilDataList = this._gridOil.GetDataByRowItemCode("ARV");

            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            for (int i = 0; i < this._maxCol; i++)
            {
                string ARVcal = getStrValuefromOilDataEntity(ARVOilDataList, i);
                string SAVcal = getStrValuefromOilDataEntity(SAVOilDataList, i);

                string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);

                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);
                string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                Dictionary<string, float> SAV_ARVfromD20_MCP = BaseFunction.FunSAV_ARVfromD20_MCP(D20cal, MCPcal);
                Dictionary<string, float> SAV_ARVfromD20_A10_A30_A50_A70_A90 = BaseFunction.FunSAV_ARVfromD20_A10_A30_A50_A70_A90(D20cal, A10cal, A30cal, A50cal, A70cal, A90cal);


                #region " 补充ARV"

                if (ARVcal == string.Empty)
                {                   
                    if (ARVcal == string.Empty)
                    {
                        if (SAV_ARVfromD20_A10_A30_A50_A70_A90.Keys.Contains("ARV"))
                            ARVcal = SAV_ARVfromD20_A10_A30_A50_A70_A90["ARV"].ToString();
                    }
                    if (ARVcal == string.Empty)
                    {
                        if (SAV_ARVfromD20_MCP.Keys.Contains("ARV"))
                            ARVcal = SAV_ARVfromD20_MCP["ARV"].ToString();
                    }

                    if (ARVcal != string.Empty && ARVcal != "非数字")
                        this._gridOil.SetData("ARV", i, ARVcal);
                }

                #endregion

                #region " 补充SAV"

                if (SAVcal == string.Empty)
                {
                    if (SAVcal == string.Empty)
                    {
                        if (SAV_ARVfromD20_A10_A30_A50_A70_A90.Keys.Contains("SAV"))
                            SAVcal = SAV_ARVfromD20_A10_A30_A50_A70_A90["SAV"].ToString();
                    }
                    if (SAVcal == string.Empty)
                    {
                        if (SAV_ARVfromD20_MCP.Keys.Contains("SAV"))
                            SAVcal = SAV_ARVfromD20_MCP["SAV"].ToString();
                    }

                    if (SAVcal != string.Empty && SAVcal != "非数字")
                        this._gridOil.SetData("SAV", i, SAVcal);
                }

                #endregion
            }
        }

        /// <summary>
        /// D20,MCP=>ANI
        /// D20, ICP, ECP=>ANI 
        /// </summary>
        private void ANILinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            for (int i = 0; i < this._maxCol; i++)
            {
                string ANIcal = getStrValuefromOilDataEntity(ANIOilDataList, i);

                if (ANIcal != string.Empty)
                    continue;

                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                if (ANIcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                    ANIcal = BaseFunction.FunANIfromD20_MCP(D20cal, MCPcal);
                }
                if (ANIcal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                    ANIcal = BaseFunction.FunANIfromD20_ICP_ECP(D20cal, ICPcal, ECPcal);
                }

                if (ANIcal != string.Empty && ANIcal!="非数字")
                    this._gridOil.SetData("ANI", i, ANIcal);
            }
        }
        /// <summary>
        /// D20,MCP=>ANI
        /// D20,A10,A30,A50,A70,A90=>ANI
        /// </summary>
        private void ANI_WideLinkSupplement()
        {
            List<OilDataEntity> D20OilDataList = this._gridOil.GetDataByRowItemCode("D20");
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> A10OilDataList = this._gridOil.GetDataByRowItemCode("A10");
            List<OilDataEntity> A30OilDataList = this._gridOil.GetDataByRowItemCode("A30");
            List<OilDataEntity> A50OilDataList = this._gridOil.GetDataByRowItemCode("A50");
            List<OilDataEntity> A70OilDataList = this._gridOil.GetDataByRowItemCode("A70");
            List<OilDataEntity> A90OilDataList = this._gridOil.GetDataByRowItemCode("A90");
            List<OilDataEntity> ANIOilDataList = this._gridOil.GetDataByRowItemCode("ANI");

            for (int i = 0; i < this._maxCol; i++)
            {
                string ANIcal = getStrValuefromOilDataEntity(ANIOilDataList, i);

                if (ANIcal != string.Empty)
                    continue;

                string D20cal = getStrValuefromOilDataEntity(D20OilDataList, i);

                if (ANIcal == string.Empty)
                {
                    string A10cal = getStrValuefromOilDataEntity(A10OilDataList, i);
                    string A30cal = getStrValuefromOilDataEntity(A30OilDataList, i);
                    string A50cal = getStrValuefromOilDataEntity(A50OilDataList, i);
                    string A70cal = getStrValuefromOilDataEntity(A70OilDataList, i);
                    string A90cal = getStrValuefromOilDataEntity(A90OilDataList, i);

                    ANIcal = BaseFunction.FunANIfromD20_A10_A30_A50_A70_A90(A10cal, A30cal, A50cal, A70cal, A90cal, D20cal);
                }

                if (ANIcal == string.Empty)
                {
                    string MCPcal = getStrValuefromOilDataEntity(MCPOilDataList, i);

                    ANIcal = BaseFunction.FunANIfromD20_MCP(D20cal, MCPcal);
                }

                if (ANIcal != string.Empty && ANIcal!="非数字")
                    this._gridOil.SetData("ANI", i, ANIcal);
            }
        }


        #region "窄馏分表插值补充"
        /// <summary>
        /// 窄馏分表插值补充
        /// V02
        /// V04
        /// V05
        /// V08
        /// V10
        /// </summary>
        private void V0_SplineCalSupplement()
        {
            List<OilDataEntity> V02OilDataList = this._gridOil.GetDataByRowItemCode("V02");
            List<OilDataEntity> V04OilDataList = this._gridOil.GetDataByRowItemCode("V04");
            List<OilDataEntity> V05OilDataList = this._gridOil.GetDataByRowItemCode("V05");
            List<OilDataEntity> V08OilDataList = this._gridOil.GetDataByRowItemCode("V08");
            List<OilDataEntity> V10OilDataList = this._gridOil.GetDataByRowItemCode("V10");

            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");

            #region  "插值补充"
            var V02datas = this._narrowGridOil.GetDataByRowItemCode("V02").ToList();//
            var V04datas = this._narrowGridOil.GetDataByRowItemCode("V04").ToList();//
            var V05datas = this._narrowGridOil.GetDataByRowItemCode("V05").ToList();//
            var V08datas = this._narrowGridOil.GetDataByRowItemCode("V08").ToList();//
            var V10datas = this._narrowGridOil.GetDataByRowItemCode("V10").ToList();//

            var ECPdatas = this._narrowGridOil.GetDataByRowItemCode("ECP").ToList();//

            if (ECPdatas == null)
                return;

            if (ECPdatas.Count <= 5)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                string V02cal = getStrValuefromOilDataEntity(V02OilDataList, i);
                string V04cal = getStrValuefromOilDataEntity(V04OilDataList, i);
                string V05cal = getStrValuefromOilDataEntity(V05OilDataList, i);
                string V08cal = getStrValuefromOilDataEntity(V08OilDataList, i);
                string V10cal = getStrValuefromOilDataEntity(V10OilDataList, i);

                string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);

                #region "V02赋值"

                if (V02cal == string.Empty)
                {
                    if (V02cal == string.Empty)
                        V02cal = BaseFunction.SplineCal(ECPdatas, V02datas, ECPcal); ;

                    if (V02cal != string.Empty && V02cal != "非数字")
                        this._gridOil.SetData("V02", i, V02cal);                   
                }

                #endregion

                #region  "V04cal"

                if (V04cal == string.Empty)
                {
                    if (V04cal == string.Empty)
                        V04cal = BaseFunction.SplineCal(ECPdatas, V04datas, ECPcal);

                    if (V04cal != string.Empty && V04cal != "非数字")
                        this._gridOil.SetData("V04", i, V04cal);
                }

                #endregion

                #region  "V05cal"

                if (V05cal == string.Empty)
                {
                    if (V05cal == string.Empty)
                        V05cal = BaseFunction.SplineCal(ECPdatas, V05datas, ECPcal);

                    if (V05cal != string.Empty && V05cal != "非数字")
                        this._gridOil.SetData("V05", i, V05cal);
                }

                #endregion

                #region  "V08cal"

                if (V08cal == string.Empty)
                {
                    if (V08cal == string.Empty)
                        V08cal = BaseFunction.SplineCal(ECPdatas, V08datas, ECPcal);

                    if (V08cal != string.Empty && V08cal != "非数字")
                        this._gridOil.SetData("V08", i, V08cal);
                }

                #endregion

                #region  "V10cal"

                if (V10cal == string.Empty)
                {
                    if (V10cal == string.Empty)
                        V10cal = BaseFunction.SplineCal(ECPdatas, V10datas, ECPcal);

                    if (V10cal != string.Empty && V10cal != "非数字")
                        this._gridOil.SetData("V10", i, V10cal);
                }
                #endregion
            }
            #endregion
        }
        /// <summary>
        /// 根据代码和列获取校正值
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private float? getCalByItemCode_Column(string itemCode , int columnIndex)
        {
            float? returnData = null;

            OilDataEntity OilData = this._gridOil.GetDataByRowItemCodeColumnIndex(itemCode, columnIndex);
            if (OilData == null)
                return returnData;

            if (OilData.calData != string.Empty)
            {
                string strResult = OilData.calData;
                float tempData = 0;
                if (float.TryParse(strResult, out tempData))
                    returnData = tempData;
            }
            else
                return returnData;

            return returnData;
        }
        /// <summary>
        /// 窄馏分表插值补充
        /// </summary>
        private void narrowSplineCalSupplement(string itemCode)
        {
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> itemCodeOilDataList = this._gridOil.GetDataByRowItemCode(itemCode);

            if (ECPOilDataList == null && ECPOilDataList.Count <= 0 && itemCodeOilDataList == null && itemCodeOilDataList.Count <= 0)
                return;

            for (int i = 0; i < this._maxCol; i++)
            {
                OilDataEntity OilData = itemCodeOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                string cal = OilData == null ? string.Empty : OilData.calData;
                
                if (cal != string.Empty)
                    continue;

                if (cal == string.Empty)
                {
                    OilDataEntity ECPOilData = ECPOilDataList.Where(o => o.ColumnIndex == i).FirstOrDefault();
                    string ECPcal = ECPOilData == null ? string.Empty : ECPOilData.calData;

                    cal = BaseFunction.SplineCal(ECPOilDataList, itemCodeOilDataList, ECPcal);
                }
                if (cal != string.Empty && cal != "非数字")
                    this._gridOil.SetData(itemCode, i, cal);
            }
        }

        #endregion 

        #endregion

        #region "窄馏分数据补充函数"
        /// <summary>
        /// 通过宽馏分的ICP在窄馏分中查找对应的物性列的值
        /// </summary>
        /// <param name="strWideICP">宽馏分的ICP</param>
        /// <param name="strItemCode">希望窄馏分返回的对应的ECP的列的物性行</param>
        /// <returns>窄馏分中对应的ICP的列的物性行的值/returns>
        public string FunNarrowStartReturnItemCodeValue(string strWideICP, string strNarrowItemCode, string strReturnItemCode)
        {
            string returnList = string.Empty;

            if (strWideICP == string.Empty || strReturnItemCode == string.Empty)//输入条件不能为空
                return returnList;
            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData();
            //List<OilDataEntity> oilDatasNarrow = this._parent.Oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)
                return returnList;

            OilDataEntity oilDataTemp = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strNarrowItemCode && o.calShowData== strWideICP).FirstOrDefault();//20151204修改.原码:calData == strWideICP).FirstOrDefault();

            if (oilDataTemp == null)
                return returnList;

            OilDataEntity oilDataItemCode = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strReturnItemCode && o.OilTableCol.colCode == oilDataTemp.OilTableCol.colCode).FirstOrDefault();

            if (oilDataItemCode == null)
                return returnList;

            returnList = oilDataItemCode.calShowData;  //20151204修改,原码:calData;

            return returnList;
        }

        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列</returns>
        public Dictionary<string, OilDataEntity> FunNarrowStartEnd(string strICP, string strECP)
        {
            Dictionary<string, OilDataEntity> returnList = new Dictionary<string, OilDataEntity>();

            if (strICP == string.Empty || strECP == string.Empty)
                return returnList;
            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData();
            //List<OilDataEntity> oilDatasNarrow = this._parent.Oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)
                return returnList;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();

            if (oilDataICP == null || oilDataECP == null)
                return returnList;

            if (oilDataICP.OilTableCol.colOrder < oilDataECP.OilTableCol.colOrder)
            {
                returnList.Add("ICP", oilDataICP);
                returnList.Add("ECP", oilDataECP);
            }

            return returnList;
        }

        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和</returns>
        public string FunNarrowStartEndTotal(string strICP, string strECP, string strItemCode)
        {
            string strResult = string.Empty;

            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return strResult;
            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData();
            //List<OilDataEntity> oilDatasNarrow = this._parent.Oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return strResult;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            List<OilDataEntity> oilDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (oilDataICP == null || oilDataECP == null || oilDatas == null)//如果查找的数据不存在则返回空
                return strResult;

            if (oilDataICP.OilTableCol.colOrder <= oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
            {
                float fSUM = 0;

                for (int j = 0; j < oilDatas.Count; j++)
                {
                    if (oilDatas[j].OilTableCol.colOrder >= oilDataICP.OilTableCol.colOrder && oilDatas[j].OilTableCol.colOrder <= oilDataECP.OilTableCol.colOrder)
                    {
                        float temp = 0;
                        if (float.TryParse(oilDatas[j].calData, out temp))
                        {
                            fSUM += temp;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (fSUM != 0)
                    strResult = fSUM.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且返回指定ECP列的物性值
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定ECP列的物性值</returns>
        public string FunNarrowStartEndReturnEndValue(string strICP, string strECP, string strItemCode)
        {
            string strResult = string.Empty;

            //if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
            //    return strResult;
            //List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData();
            
            //if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
            //    return strResult;

            //OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            //OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            //List<OilDataEntity> oilDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            //if ( oilDataECP == null || oilDatas == null)//如果查找的数据不存在则返回空
            //    return strResult;
          
            //var temp = oilDatas.Where(o => o.OilTableCol.colOrder == oilDataECP.OilTableCol.colOrder).FirstOrDefault();
            //if (temp != null)
            //    strResult = temp.calData;
 
            return strResult;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且返回指定ECP列的物性值
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定ECP列的物性值</returns>
        public string FunNarrowStartEndReturnStartValue(string strICP, string strItemCode)
        {
            string strResult = string.Empty;

            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return strResult;
            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData();
            //List<OilDataEntity> oilDatasNarrow = this._parent.Oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return strResult;

            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strICP).FirstOrDefault();
            List<OilDataEntity> oilDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (oilDataECP == null || oilDatas == null)//如果查找的数据不存在则返回空
                return strResult;

            var temp = oilDatas.Where(o => o.OilTableCol.colOrder == oilDataECP.OilTableCol.colOrder).FirstOrDefault();
            if (temp != null)
                strResult = temp.calData;

            return strResult;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且返回指定ECP列的物性值
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <returns>窄馏分中查找对应的两个ICP列,并且返回指定ECP列的物性值</returns>
        public string FunNarrowStartReturnStartValue(string strICP, string strItemCode)
        {
            string strResult = string.Empty;

            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return strResult;
            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData();
           
            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return strResult;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            
           
            if (oilDataICP == null)//如果查找的数据不存在则返回空
                return strResult;
            else
                return "0";

            return strResult;
        }

        #endregion

        #region "宽留分数据补充函数"

        /// <summary>
        /// 累加后把值返回给指定的物性, 宽留分表混合补充 WY=SUM(WY(NCUTS(from i to j)))
        /// </summary>
        private void WideGridOilMixSupplementAccumulate(string strItemCode)
        {
            List<OilDataEntity> ItemCodeOilDataList = this._gridOil.GetDataByRowItemCode(strItemCode);

            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            #region "数据赋值"

            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string strCal = getStrValuefromOilDataEntity(ItemCodeOilDataList, i);//从数据表中取得要补充的校正值
                 
                if (strCal != string.Empty)//如校正值不空，则不用补充，继续下一个馏分
                    continue;

                if (strCal == string.Empty)//如果 校正值空，则采用下面方式补充
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i);
                    strCal = FunNarrowStartEndTotal(ICPcal, ECPcal, strItemCode);
                }

                if (strCal != string.Empty && strCal != "非数字")
                    this._gridOil.SetData(strItemCode, i, strCal);
            }

            #endregion
        }
        /// <summary>
        /// itemCode=FUN(strItemCode1(from i to j), strItemCode2(from i to j))
        /// 举例： D20=FUN(WY(from i to j), D20(from i to j))
        /// </summary>
        /// <param name="strItemCode1">累积参数一</param>
        /// <param name="strItemCode2">累积参数一</param>
        /// <param name="itemCode">等待补充的参数</param>
        private void WideGridOilMixSupplementAccumulate(string strItemCode1, string strItemCode2, string itemCode)
        {
            List<OilDataEntity> itemCodeOilDataList = this._gridOil.GetDataByRowItemCode(itemCode);
            
            List<OilDataEntity> MCPOilDataList = this._gridOil.GetDataByRowItemCode("MCP");
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode("ICP");
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode("ECP");
            List<OilDataEntity> FRZOilDataList = this._gridOil.GetDataByRowItemCode("FRZ");
            
            #region "数据赋值"
            for (int i = 0; i < this._maxCol; i++)//宽馏分
            {
                string strCal = getStrValuefromOilDataEntity(itemCodeOilDataList, i);

                if (strCal != string.Empty)
                    continue;

                if (strCal == string.Empty)
                {
                    string ICPcal = getStrValuefromOilDataEntity(ICPOilDataList, i);
                    string ECPcal = getStrValuefromOilDataEntity(ECPOilDataList, i); 
                    strCal = WideFunNarrowStartEnd_ReturnItemCodeValue(ICPcal, ECPcal, strItemCode1, strItemCode2, itemCode);//计算对应的值
                }

                if (strCal != string.Empty && strCal != "非数字")
                    this._gridOil.SetData(itemCode, i, strCal);
            }

            #endregion
        }
        /// <summary>
        /// 用作宽馏分,返回物性的指数的累计乘积
        /// </summary>
        /// <param name="strICP">确定开始列</param>
        /// <param name="strECP">确定结束列</param>
        /// <param name="ItemCodeWY">确定第一个物性参数</param>
        /// <param name="ItemCode2">确定第二个物性参数</param>
        /// <param name="strItemCode"></param>
        /// <returns></returns>
        private string WideFunNarrowStartEnd_ReturnItemCodeValue(string strICP, string strECP, string ItemCodeWY, string ItemCode2, string strItemCode)
        {
            string strResult = string.Empty;

            if (strICP == string.Empty || strECP == string.Empty || ItemCodeWY == string.Empty || ItemCode2 == string.Empty)//不存在此行则返回空
                return strResult;

            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData ();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return strResult;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            List<OilDataEntity> oilDatas_ItemCodeWY = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == ItemCodeWY).ToList();
            List<OilDataEntity> oilDatas_ItemCode2 = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == ItemCode2).ToList();

            if (oilDataICP == null || oilDataECP == null || oilDatas_ItemCodeWY == null || oilDatas_ItemCode2 == null)//如果查找的数据不存在则返回空
                return strResult;

            if (oilDataICP.OilTableCol.colOrder <= oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
            {
                float fSUM = 0, fSUMWY = 0;

                for (int j = oilDataICP.OilTableCol.colOrder; j <= oilDataECP.OilTableCol.colOrder; j++)
                {
                    var oilData_ItemCodeWY = oilDatas_ItemCodeWY.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();//取出对应的WY数值
                    var oilData_ItemCode2 = oilDatas_ItemCode2.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();//取出对应的数值

                    if (oilData_ItemCodeWY == null || oilData_ItemCode2 == null)//如果馏分段内存在空值则返回空
                        return strResult;

                    string strTemp = BaseFunction.IndexFunItemCode(oilData_ItemCode2.calData, ItemCode2);//将每个数值指数转换

                    float tempWY = 0, temp2 = 0;
                    if (float.TryParse(oilData_ItemCodeWY.calData, out tempWY) && float.TryParse(strTemp, out temp2))
                    {
                        fSUM += tempWY * temp2;//将数据累积加和
                        fSUMWY += tempWY;//WY的累计加和
                    }
                    else
                    {
                        // break;
                        return strResult;
                    }
                }

                if (fSUMWY != 0 && fSUM != 0)//被除数不能为零
                {
                    float tempResult = fSUM / fSUMWY;
                    strResult = BaseFunction.InverseIndexFunItemCode(tempResult.ToString(), oilDatas_ItemCode2[0].OilTableRow.itemCode);

                    if (strResult == "非数字")
                        return strResult;
                }
                else
                {
                    return strResult;
                }
            }

            return strResult;
        }

        #endregion

        #region "渣油表补充"
        /// <summary>
        /// itemCode = FUN(strItemCode1(from i to j), strItemCode2(from i to j))
        /// 举例： D20=fun(NCUTS.D20,RES.D20)
        /// </summary>
        /// <param name="strItemCode1">累积参数一,用于乘第二个参数的指数函数</param>
        /// <param name="strItemCode2">累积参数二，用于乘第一个参数</param>
        /// <param name="itemCode">等待补充的参数</param>
        private void ResidueGridOilMixSupplementAccumulate(string itemCode)
        {
            #region "输入条件判断"
            List<OilDataEntity> ICPOilDatas = this._gridOil.GetDataByRowItemCode("ICP").Where(o=>o.calData != string.Empty).ToList();
            List<OilDataEntity> WYOilDatas = this._gridOil.GetDataByRowItemCode("WY").Where(o => o.calData != string.Empty).ToList();
            List<OilDataEntity> ItemCodeOilDatas = this._gridOil.GetDataByRowItemCode(itemCode).Where(o => o.calData != string.Empty).ToList();

            if (ICPOilDatas == null || ItemCodeOilDatas == null)
                return;

            //if (ICPOilDatas.Count <= 0 || ItemCodeOilDatas.Count <= 0)//不存在数据则无法计算
            //    return;
            if (ICPOilDatas.Count <= 0 )//不存在数据则无法计算
                return;
            if (this._maxCol == 1)//只有一列数据无法补充
                return;

            #endregion

            #region "数据赋值"

            for (int i = 0; i < this._maxCol; i++)//宽馏分的列循环
            {
                string ICPcal = getStrValuefromOilDataEntity(ICPOilDatas,i);   
                string ItemCodecal = getStrValuefromOilDataEntity(ItemCodeOilDatas, i);  
                
                if (ItemCodecal != string.Empty)//如果校正值不为空，跳出当前循环
                    continue;

                float ICP = 0;
                if (!float.TryParse(ICPcal, out ICP))//判断对应的ICP是否可以转换成数字。
                    continue;

                if (ItemCodecal == string.Empty)
                {
                    if (i < this._maxCol - 1)//不是最后一列的补充，前面值的补充依靠后面列             
                    {
                        #region "累计和赋值"

                        string tempICPcal = string.Empty, tempItemCodecal = string.Empty, strWYcalUN = string.Empty;
                        double  tempICP = 0, tempItemCode = 0, WYUN = 0;

                        for (int j = i + 1; j <= this._maxCol; j++)//寻找下一个馏分段
                        {
                            strWYcalUN = getStrValuefromOilDataEntity(WYOilDatas, j);
                            tempICPcal = getStrValuefromOilDataEntity(ICPOilDatas, j);   
                            tempItemCodecal = getStrValuefromOilDataEntity(ItemCodeOilDatas,j);

                            if (double.TryParse(tempICPcal, out tempICP) && double.TryParse(tempItemCodecal, out tempItemCode) && double.TryParse(strWYcalUN, out WYUN))
                            {
                                tempItemCodecal = BaseFunction.IndexFunItemCode(tempItemCodecal, itemCode);//指数转换
                                break;
                            }
                        }

                        string WYcal = getStrValuefromOilDataEntity(WYOilDatas, i);  

                        if (ICP < tempICP) //前一馏分段包括后面馏分段
                        {
                            string strAccumulate = ResidueFunStartEnd_ReturnItemCodeValue(ICPcal, tempICPcal, itemCode);//获取积和

                            double Accumulate = 0, WY = 0;
                            // string  strIndex = BaseFunction .IndexFunItemCode(str
                            if (double.TryParse(strAccumulate, out Accumulate) && double.TryParse(WYcal, out WY) && double.TryParse(tempItemCodecal, out tempItemCode))
                            {

                                double fResult = (tempItemCode * WYUN + Accumulate) / WY;//最终的加和SUM/WY(UN)
                                ItemCodecal = BaseFunction.InverseIndexFunItemCode(fResult.ToString(), itemCode);
                                //ItemCodecal = (tempItemCode * WY + Accumulate).ToString();
                            }
                        }
                        else if (ICP > tempICP) //后面馏分段包括前一馏分段
                        {
                            string strAccumulate = ResidueFunStartEnd_ReturnItemCodeValue(tempICPcal, ICPcal, itemCode);//获取积和

                            double Accumulate = 0, WY = 0;
                            if (double.TryParse(strAccumulate, out Accumulate) && double.TryParse(WYcal, out WY) && double.TryParse(tempItemCodecal, out tempItemCode))
                            {
                                double fResult = (tempItemCode * WY - Accumulate) / WYUN;//最终的加和SUM/WY(UN)
                                ItemCodecal = BaseFunction.InverseIndexFunItemCode(fResult.ToString(), itemCode);
                                //ItemCodecal = (tempItemCode * WY - Accumulate).ToString();
                            }
                        }
                        #endregion
                    }
                }

                if (ItemCodecal == string.Empty)
                {
                    if (i >= 1)//不是第一列的值的补充，后一列的值的补充依靠前面列  
                    {
                        #region "累计和赋值"

                        string tempICPcal = string.Empty, tempItemCodecal = string.Empty, strWYcal = string.Empty;
                        double tempICP = 0, tempItemCode = 0, WY = 0;

                        for (int j = 0; j <= i - 1; j++)//寻找下一个馏分段
                        {
                            strWYcal = getStrValuefromOilDataEntity(WYOilDatas, j);   
                            tempICPcal = getStrValuefromOilDataEntity(ICPOilDatas, j); 
                            tempItemCodecal = getStrValuefromOilDataEntity(ItemCodeOilDatas, j);
                            if (double.TryParse(tempICPcal, out tempICP) && double.TryParse(tempItemCodecal, out tempItemCode) && double.TryParse(strWYcal, out WY))
                            {
                                tempItemCodecal = BaseFunction.IndexFunItemCode(tempItemCodecal, itemCode);//指数转换
                                break;
                            }
                        }

                        string strWYcalUN = getStrValuefromOilDataEntity(WYOilDatas, i);     

                        if (ICP < tempICP) //前一馏分段包括后面馏分段
                        {
                            string strAccumulate = ResidueFunStartEnd_ReturnItemCodeValue(ICPcal, tempICPcal, itemCode);//获取积和

                            double Accumulate = 0, WYUN = 0;
                            if (double.TryParse(strAccumulate, out Accumulate) && double.TryParse(strWYcalUN, out WYUN) && double.TryParse(tempItemCodecal, out tempItemCode))
                            {
                                double fResult = (tempItemCode * WY + Accumulate) / WYUN;//最终的加和SUM/WY(UN)
                                ItemCodecal = BaseFunction.InverseIndexFunItemCode(fResult.ToString(), itemCode);
                            }
                        }
                        else if (ICP > tempICP) //后面馏分段包括前一馏分段
                        {
                            string strAccumulate = ResidueFunStartEnd_ReturnItemCodeValue(tempICPcal, ICPcal, itemCode);//获取积和

                            double Accumulate = 0, WYUN = 0;
                            if (double.TryParse(strAccumulate, out Accumulate) && double.TryParse(strWYcalUN, out WYUN) && double.TryParse(tempItemCodecal, out tempItemCode))
                            {
                                double fResult = (tempItemCode * WY - Accumulate) / WYUN;//最终的加和SUM/WY(UN)
                                ItemCodecal = BaseFunction.InverseIndexFunItemCode(fResult.ToString(), itemCode);
                            }
                        }
                        #endregion
                    }

                }

                if (ItemCodecal != string.Empty && ItemCodecal != "非数字")
                    this._gridOil.SetData(itemCode, i, ItemCodecal);
            }//END for

            #endregion
        }

        /// <summary>
        /// 用作渣油表,返回窄馏分表和宽馏分表中物性的指数的累计乘积
        /// </summary>
        /// <param name="strICP">确定窄馏分表中ICP开始列的值</param>
        /// <param name="strECP">确定窄馏分表中ECP结束列的值</param>
        /// <param name="ItemCode">确定第一个物性参数，用于乘第二个参数的指数函数</param>
        /// <returns>用作渣油表,返回窄馏分表和宽馏分表中物性的指数的累计乘积</returns>
        private string ResidueFunStartEnd_ReturnItemCodeValue(string strICP, string strECP, string ItemCode)
        {
            string strResult = string.Empty;

            strResult = ResidueFunNarrowStartEnd_ReturnItemCodeValue(strICP, strECP, ItemCode);

            if (strResult == string.Empty)
                strResult = ResidueFunWideStartEnd_ReturnItemCodeValue(strICP, strECP, ItemCode);

            return strResult;
        }
        /// <summary>
        /// 用作渣油表,返回窄馏分表中物性的指数的累计乘积
        /// </summary>
        /// <param name="strICP">确定窄馏分表中ICP开始列的值</param>
        /// <param name="strECP">确定窄馏分表中ECP结束列的值</param>
        /// <param name="ItemCode">确定第一个物性参数，用于乘第二个参数的指数函数</param>
        /// <returns>用作渣油表,返回窄馏分表中物性的指数的累计乘积</returns>
        private string ResidueFunNarrowStartEnd_ReturnItemCodeValue(string strICP, string strECP, string ItemCode)
        {
            string strResult = string.Empty;

            #region "输入条件判断"
            List<OilDataEntity> ItemCodeOilDataList = this._gridOil.GetDataByRowItemCode(ItemCode);
            List<OilDataEntity> ICPOilDataList = this._gridOil.GetDataByRowItemCode(strICP);
            List<OilDataEntity> ECPOilDataList = this._gridOil.GetDataByRowItemCode(strECP);

            List<OilDataEntity> oilDatasNarrow = this._narrowGridOil.GetAllData ();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return strResult;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calData == strICP).FirstOrDefault();
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calData == strECP).FirstOrDefault();
            List<OilDataEntity> oilDatas_WY = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> oilDatas_ItemCode = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == ItemCode).ToList();

            if (oilDataICP == null || oilDataECP == null || oilDatas_WY == null || oilDatas_ItemCode == null)//如果查找的数据不存在则返回空
                return strResult;

            #endregion

            #region "判断选择的馏分段中ICP和ECP是否对应"

            bool ICP_ECP = true;

            #region "初始化列表"
            List<string> WCTList = new List<string>();
            WCTList.Add("脱蜡油");
            WCTList.Add("精制油1");
            WCTList.Add("精制油2");
            WCTList.Add("精制油3");
            WCTList.Add("P+N");
            WCTList.Add("P+N+A1");
            WCTList.Add("P+N+A2");
            #endregion

            if (oilDataICP.OilTableCol.colOrder != oilDataECP.OilTableCol.colOrder)
            {
                for (int j = oilDataICP.OilTableCol.colOrder; j < oilDataECP.OilTableCol.colOrder; j++)
                {
                    var tempICPOilData = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableCol.colOrder == j + 1).FirstOrDefault();
                    var tempECPOilData = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.OilTableCol.colOrder == j).FirstOrDefault();

                    if (tempICPOilData.calData != tempECPOilData.calData)
                        ICP_ECP = false;

                }
            }

            if (!ICP_ECP)//ICP和ECP不连续
                return strResult;

            #endregion

            if (oilDataICP.OilTableCol.colOrder <= oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
            {
                float fSUM = 0;

                for (int j = oilDataICP.OilTableCol.colOrder; j <= oilDataECP.OilTableCol.colOrder; j++)
                {
                    var oilData_WY = oilDatas_WY.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();//取出对应的数值                    
                    var oilData_ItemCode = oilDatas_ItemCode.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();//取出对应的数值

                    if (oilData_WY == null || oilData_ItemCode == null)//馏分段内不存在值，则返回空
                        return strResult;

                    string strTemp = BaseFunction.IndexFunItemCode(oilData_ItemCode.calData, ItemCode);//将每个数值指数转换

                    float WY = 0, temp = 0;
                    if (float.TryParse(oilData_WY.calData, out WY) && float.TryParse(strTemp, out temp))
                    {
                        fSUM += WY * temp;//将数据累积加和
                    }
                    else
                    {
                        return strResult;//数据乘积的累积加和过程中不能存在空值。
                    }
                }

                strResult = fSUM.ToString();
            }

            return strResult;
        }
        /// <summary>
        /// 用作渣油表,返回宽馏分表中物性的指数的累计乘积
        /// </summary>
        /// <param name="strICP">确定宽馏分表中ICP开始列的值</param>
        /// <param name="strECP">确定宽馏分表中ECP结束列的值</param>
        /// <param name="ItemCode">确定第一个物性参数，用于乘第二个参数的指数函数</param>
        /// <returns>用作渣油表,返回宽馏分表中物性的指数的累计乘积</returns>
        private string ResidueFunWideStartEnd_ReturnItemCodeValue(string strICP, string strECP, string ItemCode)
        {
            string strResult = string.Empty;

            #region "输入条件判断"
            if (strICP == string.Empty || strECP == string.Empty || ItemCode == string.Empty)//不存在此行则返回空
                return strResult;

            List<OilDataEntity> oilDatasWide = this._wideGridOil.GetAllData ();//找出窄馏分表数据

            if (oilDatasWide == null)//如果宽馏分数据表不存在则返回空
                return strResult;

            OilDataEntity oilDataICP = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.calData == strICP).FirstOrDefault();
            OilDataEntity oilDataECP = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.calData == strECP).FirstOrDefault();
            List<OilDataEntity> oilDatas_WY = oilDatasWide.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> oilDatas_ItemCode = oilDatasWide.Where(o => o.OilTableRow.itemCode == ItemCode).ToList();
            List<OilDataEntity> oilDatas_WCT = oilDatasWide.Where(o => o.OilTableRow.itemCode == "WCT").ToList();//宽馏分中排除类型为脱蜡油、P+N、P+N+A1、P+N+A2类型;

            if (oilDataICP == null || oilDataECP == null || oilDatas_WY == null || oilDatas_ItemCode == null || oilDatas_WCT == null)//如果查找的数据不存在则返回空
                return strResult;

            #endregion

            if (oilDataICP.OilTableCol.colOrder <= oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
            {
                float fSUM = 0;

                #region "判断选择的馏分段中ICP和ECP是否对应"

                #region "初始化列表"
                List<string> WCTList = new List<string>();
                WCTList.Add("脱蜡油");
                WCTList.Add("精制油1");
                WCTList.Add("精制油2");
                WCTList.Add("精制油3");
                WCTList.Add("P+N");
                WCTList.Add("P+N+A1");
                WCTList.Add("P+N+A2");
                #endregion

                bool ICP_ECP = true;

                if (oilDataICP.OilTableCol.colOrder <= oilDataECP.OilTableCol.colOrder)
                {
                    for (int j = oilDataICP.OilTableCol.colOrder; j < oilDataECP.OilTableCol.colOrder; j++)
                    {
                        var tempICPOilData = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableCol.colOrder == j + 1).FirstOrDefault();
                        var tempECPOilData = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.OilTableCol.colOrder == j).FirstOrDefault();
                        var tempWCTOilData = oilDatas_WCT.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();

                        //if (tempICPOilData == null || tempECPOilData == null)
                        //    ICP_ECP = false;
                        if (tempICPOilData.calData != tempECPOilData.calData || WCTList.Contains(tempWCTOilData.calData))
                            ICP_ECP = false;
                    }
                }

                if (!ICP_ECP)//ICP和ECP不连续
                    return strResult;

                #endregion

                for (int j = oilDataICP.OilTableCol.colOrder; j <= oilDataECP.OilTableCol.colOrder; j++)
                {
                    var oilData_WY = oilDatas_WY.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();//取出对应的数值                    
                    var oilData_ItemCode = oilDatas_ItemCode.Where(o => o.OilTableCol.colOrder == j).FirstOrDefault();//取出对应的数值

                    if (oilData_WY == null || oilData_ItemCode == null)//馏分段内不存在值，则返回空
                        return strResult;

                    string strTemp = BaseFunction.IndexFunItemCode(oilData_ItemCode.calData, ItemCode);//将每个数值指数转换

                    float WY = 0, temp = 0;
                    if (float.TryParse(oilData_WY.calData, out WY) && float.TryParse(strTemp, out temp))
                    {
                        fSUM += WY * temp;//将数据累积加和
                    }
                    else
                    {
                        return strResult;//数据乘积的累积加和过程中不能存在空值。
                    }
                }

                strResult = fSUM.ToString();
            }

            return strResult;
        }

        #endregion       
    }

    #region 排序比较类
    /// <summary>
    /// 轻端参数Tb比较器
    /// </summary>
    public class LightCurveParmTableEntityComparable : IComparer<LightCurveParmTableEntity> 
    {
        public int Compare(LightCurveParmTableEntity one, LightCurveParmTableEntity two)
        {
            int result = 0;
            float x =0,y =0;
            if (float.TryParse(one.Tb, out x) && float.TryParse(two.Tb, out y))
            {
                if (x > y)
                    result = 1;
                else if (x < y)
                    result = -1;
                else
                    result = 0;
            }
            return result;
        }
    }
    #endregion

    /// <summary>
    /// 做关联补充时窄馏分表中运动粘度补充设置的对象
    /// </summary>
    public class VT
    {
        private string _v = string.Empty;
        private int _t = 0;

        public VT()
        {

        }
        public VT(string v, int t)
        {
            this._v = v;
            this._t = t;
        }

        public string V
        {
            set { this._v = value; }
            get { return this._v; }
        }
        public int T
        {
            set { this._t = value; }
            get { return this._t; }
        }

    }


   
}
