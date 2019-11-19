using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;
using System.Windows.Forms;
using System.ComponentModel;
using RIPP.Lib;

namespace RIPP.OilDB.Data
{    
    public class SummaryBll
    {
        #region "私有变量"
        /// <summary>
        /// 当前原油
        /// </summary>
        private OilInfoEntity _OilA = null;
        /// <summary>
        /// 指标值集合
        /// </summary>
        private List<TargetedValueEntity> _targetedValueList = new List<TargetedValueEntity>();
        /// <summary>
        /// 指标值列集合
        /// </summary>
        private List<TargetedValueColEntity> _targetedValueColList = new List<TargetedValueColEntity>();
        /// <summary>
        /// 指标值行集合
        /// </summary>
        private List<TargetedValueRowEntity> _targetedValueRowlList = new List<TargetedValueRowEntity>();
        /// <summary>
        /// 
        /// </summary>
        private List<OilTableTypeComparisonTableEntity> _oilTableTypeComparisonTableEntityList = new List<OilTableTypeComparisonTableEntity>();
        /// <summary>
        /// 水平值
        /// </summary>
        private List<LevelValueEntity> _LevelValueEntityList = new List<LevelValueEntity>();

        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public SummaryBll()
        {         
        
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oilInfo"></param>
        public SummaryBll(OilInfoEntity oilInfo)
        {
            this._OilA = oilInfo;

            TargetedValueEntityAccess targetedValueAccess = new TargetedValueEntityAccess();
            this._targetedValueList = targetedValueAccess.Get("1=1");

            TargetedValueColEntityAccess targetedValueColAccess = new TargetedValueColEntityAccess();
            this._targetedValueColList = targetedValueColAccess.Get("1=1");

            TargetedValueRowEntityAccess targetedValueRowAccess = new TargetedValueRowEntityAccess();
            this._targetedValueRowlList = targetedValueRowAccess.Get("1=1");

            OilTableTypeComparisonTableAccess oilTableTypeComparisonTableAccess = new OilTableTypeComparisonTableAccess();
            this._oilTableTypeComparisonTableEntityList = oilTableTypeComparisonTableAccess.Get("1=1").ToList();

            LevelValueAccess levelValueAccess = new LevelValueAccess();
            this._LevelValueEntityList = levelValueAccess.Get("1=1").ToList();
        }
        #endregion 

        #region "简评"       
        /// <summary>
        /// 石脑油简评
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="dataList"></param>
        /// <param name="rowList"></param>
        /// <param name="comparsionTable"></param>
        private void getSimSummaryFromNaphtha(ref string summary, List<OilDataEntity> dataList, List<OilTableRowEntity> rowList,string cutName)
        {
            try
            {
                OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.ID == (int)enumOilTableTypeComparisonTable.Naphtha).FirstOrDefault();

                string strERROR = string.Empty;
                SummaryEntity ICPSummary = getTextandValue(ref strERROR, "ICP", dataList, rowList, EnumTableType.Wide,true,cutName);
                SummaryEntity ECPSummary = getTextandValue(ref strERROR, "ECP", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity WYSummary = getTextandValue(ref strERROR, "WY", dataList, rowList, EnumTableType.Wide, true, cutName);
                //SummaryEntity SULSummary = getTextandValue(ref strERROR, "SUL", dataList, rowList, EnumTableType.Wide);
                SummaryEntity SULSummary = getTextandValueBeforeCondenseDec(ref strERROR, "SUL", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity PATSummary = getTextandValue(ref strERROR, "PAT", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity ARMSummary = getTextandValue(ref strERROR, "ARM", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity N2ASummary = getTextandValue(ref strERROR, "N2A", dataList, rowList, EnumTableType.Wide, true, cutName);
                bool work = false;
                if (!string.IsNullOrWhiteSpace(strERROR))
                {
                    DialogResult r = MessageBox.Show(strERROR + "是否继续？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (r == DialogResult.Yes)
                        work = true;
                    else
                        return;
                }
                else
                    work = true;

                if (work)
                {
                    summary += "\r\n";

                    #region "硫含量"
                    string strSULTEXT = string.Empty;
                    OilTableRowEntity SULRow = rowList.Where(o => o.itemCode == "SUL" && o.oilTableTypeID == (int)EnumTableType.Wide).FirstOrDefault();
                    if (SULSummary != null && SULSummary.fTEXT < 0.1)
                        strSULTEXT = oilTool.calDataDecLimit((SULSummary.fTEXT * 10000).ToString(), 1, SULRow.valDigital) + "μg/g";
                    else if (SULSummary != null && SULSummary.fTEXT >= 0.1)
                        strSULTEXT = oilTool.calDataDecLimit(SULSummary.TEXT, 2, SULRow.valDigital) + "%";
                    #endregion

                    summary += ICPSummary.TEXT + "～" + ECPSummary.TEXT + "℃石脑油馏分的收率为" + WYSummary.TEXT + "%。硫含量为" + strSULTEXT;

                    #region "PATARP"
                    string PATARPTEMP = "。链烷烃含量为" + PATSummary.TEXT + "%，芳香烃含量为" + ARMSummary.TEXT + "%";
                    LevelValueEntity PATLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "PAT" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity ARMLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "ARM" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity N2ALevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "N2A" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();



                    if (PATLevelValue != null && PATLevelValue.More != null && PATSummary.fVALUE != null && PATSummary.fVALUE.Value > PATLevelValue.More.Value)
                    {
                        //if (PATSummary.fVALUE.Value > PATLevelValue.More.Value)
                        //{
                            PATARPTEMP = "。链烷烃含量较高，为" + PATSummary.TEXT + "%";
                            if (ARMLevelValue != null && ARMSummary.fVALUE != null && ARMLevelValue.Less != null)
                            {
                                if (ARMSummary.fVALUE.Value < ARMLevelValue.Less.Value)
                                {
                                    PATARPTEMP += "，芳香烃含量为" + ARMSummary.TEXT + "%";
                                }
                            }
                            PATARPTEMP += "，是较好的乙烯裂解原料。";
                        //}
                    }
                    else if (PATSummary.fVALUE != null && PATLevelValue != null && PATLevelValue.Less != null && PATSummary.fVALUE.Value > PATLevelValue.Less.Value)
                    {
                        //if (PATSummary.fVALUE.Value < PATLevelValue.Less.Value)
                        //{
                            PATARPTEMP = "。链烷烃含量为" + PATSummary.TEXT + "%";
                            if (ARMLevelValue != null && ARMSummary.fVALUE != null && ARMLevelValue.Less != null)
                            {
                                if (ARMSummary.fVALUE.Value < ARMLevelValue.Less.Value)
                                {
                                    PATARPTEMP += "，芳香烃含量为" + ARMSummary.TEXT + "%";
                                }
                            }
                            PATARPTEMP += "，可做为乙烯裂解原料。";
                        //}
                    }
                    else if (N2ALevelValue != null && N2ALevelValue.More != null && N2ASummary.fVALUE != null && N2ASummary.fVALUE.Value > N2ALevelValue.More.Value)
                    {
                       // if (N2ASummary.fVALUE.Value > N2ALevelValue.More.Value)
                        PATARPTEMP = "。链烷烃含量为" + PATSummary.TEXT + "%，芳烃收率指数（N＋2A）为" + N2ASummary.TEXT + "，是较好的重整原料。";
                    }
                    else if (N2ALevelValue != null && N2ALevelValue.Less != null && N2ASummary.fVALUE != null && N2ASummary.fVALUE.Value > N2ALevelValue.Less.Value)
                    {
                        //if (N2ASummary.fVALUE.Value < N2ALevelValue.Less.Value)
                        PATARPTEMP = "。链烷烃含量为" + PATSummary.TEXT + "%，芳烃收率指数（N＋2A）为" + N2ASummary.TEXT + "，可作为重整原料。";
                    }
                    if (!PATARPTEMP.EndsWith("。"))
                        PATARPTEMP += "。";
                    summary += PATARPTEMP;
                    #endregion

                    #region "石脑油N2A"
                    //string N2ATEMP = "";
                    //LevelValueEntity N2ALevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "N2A" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    //if (N2ALevelValue != null && N2ALevelValue.More != null && N2ASummary.fVALUE != null)
                    //{
                    //    if (N2ASummary.fVALUE.Value > N2ALevelValue.More.Value)
                    //        N2ATEMP = "芳烃收率指数（N＋2A）为" + N2ASummary.TEXT + "，是较好的重整原料。";
                    //}
                    //else if (N2ALevelValue != null && N2ALevelValue.Less != null && N2ASummary.fVALUE != null)
                    //{
                    //    if (N2ASummary.fVALUE.Value < N2ALevelValue.Less.Value)
                    //        N2ATEMP = "芳烃收率指数（N＋2A）为" + N2ASummary.TEXT + "，可作为重整原料。";
                    //}
                    //summary += N2ATEMP;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Error("石脑油的简评错误：" + ex.ToString());
            }
        }
        /// <summary>
        ///  航煤简评
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="dataList"></param>
        /// <param name="rowList"></param>
        private void getSimSummaryFromAviationKerosene(ref string summary, List<OilDataEntity> dataList, List<OilTableRowEntity> rowList, string cutName)
        {
            try
            {
                OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.ID == (int)enumOilTableTypeComparisonTable.AviationKerosene).FirstOrDefault();

                string strERROR = string.Empty;
                SummaryEntity ICPSummary = getTextandValue(ref strERROR, "ICP", dataList, rowList, EnumTableType.Wide, true ,cutName);
                SummaryEntity ECPSummary = getTextandValue(ref strERROR, "ECP", dataList, rowList, EnumTableType.Wide,  true, cutName);
                SummaryEntity WYSummary = getTextandValue(ref strERROR, "WY", dataList, rowList, EnumTableType.Wide, true, cutName);
                //SummaryEntity SULSummary = getTextandValue(ref strERROR, "SUL", dataList, rowList, EnumTableType.Wide);
                SummaryEntity SULSummary = getTextandValueBeforeCondenseDec(ref strERROR, "SUL", dataList, rowList, EnumTableType.Wide, false, cutName);
                SummaryEntity D20Summary = getTextandValue(ref strERROR, "D20", dataList, rowList, EnumTableType.Wide, true, cutName);

                SummaryEntity NETSummary = getTextandValue(ref strERROR, "NET", dataList, rowList, EnumTableType.Wide, false, cutName);
                SummaryEntity FRZSummary = getTextandValue(ref strERROR, "FRZ", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity SMKSummary = getTextandValue(ref strERROR, "SMK", dataList, rowList, EnumTableType.Wide,  true, cutName);
                SummaryEntity ARVSummary = getTextandValue(ref strERROR, "ARV", dataList, rowList, EnumTableType.Wide,  true, cutName);
                SummaryEntity MECSummary = getTextandValue(ref strERROR, "MEC", dataList, rowList, EnumTableType.Wide,  false , cutName);
                SummaryEntity CC2Summary = getTextandValue(ref strERROR, "CC2", dataList, rowList, EnumTableType.Wide,  false , cutName);
                bool work = false;
                if (!string.IsNullOrWhiteSpace(strERROR))
                {
                    DialogResult r = MessageBox.Show(strERROR + "是否继续？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (r == DialogResult.Yes)
                        work = true;
                    else
                        return;
                }
                else
                    work = true;
                if (work)
                {
                    summary += "\r\n";

                    #region "硫含量单位转换"
                    string strSULTEXT = string.Empty;
                    OilTableRowEntity SULRow = rowList.Where(o => o.itemCode == "SUL" && o.oilTableTypeID == (int)EnumTableType.Wide).FirstOrDefault();
                    if (SULSummary != null && SULSummary.fTEXT < 0.1)
                        strSULTEXT = oilTool.calDataDecLimit((SULSummary.fTEXT * 10000).ToString(), 1, SULRow.valDigital) + "μg/g";
                    else if (SULSummary != null && SULSummary.fTEXT >= 0.1)
                        strSULTEXT = oilTool.calDataDecLimit(SULSummary.TEXT, 2, SULRow.valDigital) + "%";
                    #endregion


                    summary += ICPSummary.TEXT + "～" + ECPSummary.TEXT + "℃喷气燃料馏分的收率为" + WYSummary.TEXT + "%。";
                        
                       
                    int sign = 0;
                    string temp="";
                    int sign1=0;

                    #region "FRZARVD20SMK取出指标值"
                    LevelValueEntity FRZLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "FRZ" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity ARVLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "ARV" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity D20LevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "D20" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity SMKLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "SMK" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    #endregion

                    temp = "密度为" + D20Summary.TEXT + "g/cm3。";
                    if( D20LevelValue.More != null && D20Summary.fVALUE != null)
                    {
                        if(D20Summary.fVALUE.Value  > D20LevelValue.More.Value )
                        {
                            temp = "密度偏大，为" + D20Summary.TEXT + "g/cm3。";
                            sign1=1;
                        }
                    }
                    if(D20LevelValue.Less != null && D20Summary.fVALUE != null)
                    {
                        if (D20Summary.fVALUE.Value < D20LevelValue.Less.Value)
                        {
                            temp="密度偏小，为" + D20Summary.TEXT + "g/cm3。";
                            sign1=1;
                        }
                    }
                    summary += temp + "硫含量为" + strSULTEXT + "，酸值为" + NETSummary.TEXT + "mgKOH/g。";
                    temp="";

                    if(FRZLevelValue.More != null )
                    {
                        if (FRZSummary.fVALUE.Value > FRZLevelValue.More.Value)
                        {
                            temp = "冰点偏高，为" + FRZSummary.TEXT + "℃，";
                            sign1=1;
                        }
                        else if (FRZSummary.fVALUE >= FRZLevelValue.More.Value - 0.5)
                        {
                            temp = "冰点卡边，为" + FRZSummary.TEXT + "℃，";
                        }
                        else
                            temp = "冰点为" + FRZSummary.TEXT + "℃，";
                    }

                    summary += temp ;
                    temp = "";

                    if(ARVLevelValue.More !=null)
                    {
                        if (ARVSummary.fVALUE.Value > ARVLevelValue.More.Value)
                        {
                            temp = "芳香烃体积含量偏高，为" + ARVSummary.TEXT + "%，";
                            sign1=1;
                        }
                        else if (ARVSummary.fVALUE.Value >= ARVLevelValue.More.Value - 0.5)
                        {
                            temp = "芳香烃体积含量卡边，为" + ARVSummary.TEXT + "%，";
                        }
                        else
                            temp = "芳香烃体积含量为" + ARVSummary.TEXT + "%，";
                    }

                    summary += temp ;
                    temp = "";
                    
                    if(SMKLevelValue.More !=null)
                    {
                        if (SMKSummary.fVALUE.Value > SMKLevelValue.More.Value + 0.5)
                        {
                            temp = "烟点为" + SMKSummary.TEXT + "mm";
                        }
                        else if (SMKSummary.fVALUE.Value >= SMKLevelValue.More.Value)
                        {
                            temp = "烟点卡边，为" + SMKSummary.TEXT + "mm";
                        }
                        else if (SMKLevelValue.Less != null && SMKSummary.fVALUE.Value > SMKLevelValue.Less.Value)
                        {
                            temp = "烟点为" + SMKSummary.TEXT + "mm";
                            sign1 += 2;
                        }
                        else
                        {
                            temp = "烟点偏低，为" + SMKSummary.TEXT + "mm";
                            sign1 = 1;
                        }
                    }

                    summary += temp ;


                    #region "石脑油SUL"
                    LevelValueEntity SULLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "SUL" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (SULLevelValue != null && SULLevelValue.More  != null && SULSummary.fVALUE != null)
                    {
                        if (SULSummary.fVALUE.Value > SULLevelValue.More.Value)
                            sign++;
                    }
                    #endregion

                    #region "石脑油NET"
                    LevelValueEntity NETLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "NET" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (NETLevelValue != null && NETLevelValue.More != null && NETSummary.fVALUE != null)
                    {
                        if (NETSummary.fVALUE.Value > NETLevelValue.More.Value)
                            sign++;
                    }
                    #endregion

                    #region "石脑油CC2"
                    LevelValueEntity CC2LevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "CC2" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (CC2LevelValue != null && CC2LevelValue.More != null && CC2Summary.fVALUE != null)
                    {

                        //if (CC2Summary.VALUE != string.Empty)
                        //{ 
                        //    if (CC2Summary.VALUE.Length ==1)
                        //    {
                        //       string str = CC2Summary.VALUE.Substring(0, 1);
                               
                        //       int a 

                        //    }
                        //    else if (CC2Summary.VALUE.Length >= 1)
                        //    {
                            
                        //    }
                        
                        
                        //}
                        if (CC2Summary.fVALUE.Value > CC2LevelValue.Less.Value)
                            sign++;
                    }
                    #endregion

                    #region "石脑油MEC"
                    LevelValueEntity MECLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "MEC" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (MECLevelValue != null && MECLevelValue.More != null && MECSummary.fVALUE != null)
                    {
                        if (MECSummary.fVALUE.Value > MECLevelValue.More.Value)
                            sign++;
                    }
                    #endregion

                    #region "FRZARVD20SMK"  "修改，此部分弃用"
                    //LevelValueEntity FRZLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "FRZ" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    //LevelValueEntity ARVLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "ARV" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    //LevelValueEntity D20LevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "D20" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    //LevelValueEntity SMKLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "SMK" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    //if (FRZLevelValue != null && ARVLevelValue != null && D20LevelValue != null && SMKLevelValue != null
                    //    && ARVLevelValue.Less != null && ARVSummary.fVALUE != null && FRZLevelValue.Less != null && FRZSummary.fVALUE != null
                    //    && D20LevelValue.Less != null && D20LevelValue.More != null && D20Summary.fVALUE != null
                    //    && SMKLevelValue.Less != null && SMKLevelValue.More != null && SMKSummary.fVALUE != null)
                    //{
                    //    if (FRZSummary.fVALUE.Value <= FRZLevelValue.Less.Value && ARVSummary.fVALUE.Value <= ARVLevelValue.Less.Value &&
                    //        D20LevelValue.Less.Value <= D20Summary.fVALUE.Value && D20Summary.fVALUE.Value <= D20LevelValue.More.Value)
                    //    {
                    //        if (SMKSummary.fVALUE.Value > SMKLevelValue.More.Value + 0.1)
                    //        {
                    //            if (sign == 0)
                    //                summary += "，可生产3#喷气燃料";
                    //            else
                    //                summary += "，精制后可生产3#喷气燃料";
                    //        }
                    //        else if (SMKSummary.fVALUE.Value <= SMKLevelValue.More.Value && SMKSummary.fVALUE.Value >= SMKLevelValue.Less.Value)
                    //            summary += "，建议补充辉光值或萘系烃含量后判断可否做3#喷气燃料";
                    //    }
                    //}
                    #endregion
                    
                    #region"结论"
                    if( sign1==0)
                    {
                        if(sign==0)
                            summary += "，可生产3#喷气燃料" ;
                        else
                            summary += "，精制后可生产3#喷气燃料" ;
                    }
                    else if(sign1 > 1)
                        summary += "，建议补充辉光值或萘系烃含量后判断可否做3#喷气燃料";
                    #endregion

                    summary += "。";
                }
            }
            catch (Exception ex)
            {
                Log.Error("航煤的简评错误：" + ex.ToString());
            }
        }

       

        private void getSimSummaryFromDieselOil(ref string summary, List<OilDataEntity> dataList, List<OilTableRowEntity> rowList, string cutName)
        {
            try
            {
                OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.ID == (int)enumOilTableTypeComparisonTable.DieselOil).FirstOrDefault();

                string str = string.Empty;
                SummaryEntity ICPSummary = getTextandValue(ref str, "ICP", dataList, rowList, EnumTableType.Wide,true , cutName);
                SummaryEntity ECPSummary = getTextandValue(ref str, "ECP", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity WYSummary = getTextandValue(ref str, "WY", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity SOPSummary = getTextandValue(ref str, "SOP", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity CISummary = getTextandValue(ref str, "CI", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity CFPSummary = getTextandValue(ref str, "CFP", dataList, rowList, EnumTableType.Wide, false , cutName);

                SummaryEntity SULSummary = getTextandValueBeforeCondenseDec(ref str, "SUL", dataList, rowList, EnumTableType.Wide, false, cutName);
                SummaryEntity D20Summary = getTextandValue(ref str, "D20", dataList, rowList, EnumTableType.Wide,false , cutName);
                SummaryEntity ACDSummary = getTextandValue(ref str, "ACD", dataList, rowList, EnumTableType.Wide,false , cutName);
                SummaryEntity CC3Summary = getTextandValue(ref str, "CC3", dataList, rowList, EnumTableType.Wide, false, cutName);
                #region "硫含量"
                string strSULTEXT = string.Empty;
                OilTableRowEntity SULRow = rowList.Where(o => o.itemCode == "SUL" && o.oilTableTypeID == (int)EnumTableType.Wide).FirstOrDefault();
                if (SULSummary != null && SULSummary.fTEXT < 0.1)
                    strSULTEXT = oilTool.calDataDecLimit((SULSummary.fTEXT * 10000).ToString(), 1, SULRow.valDigital) + "μg/g";
                else if (SULSummary != null && SULSummary.fTEXT >= 0.1)
                    strSULTEXT = oilTool.calDataDecLimit(SULSummary.TEXT, 2, SULRow.valDigital) + "%";
                #endregion
                bool work = false;
                if (!string.IsNullOrWhiteSpace(str))
                {
                    DialogResult r = MessageBox.Show(str + "是否继续？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (r == DialogResult.Yes)
                        work = true;
                    else
                        return;
                }
                else
                    work = true;
                if (work)
                {
                    bool critical = false;//新定义一个判断变量，卡边指示。 lh

                    #region "柴油第一次循环"
                    List<int> colList = new List<int>() { -50, -35, -20, -10, 0, 5, 10 };
                   
                    int FLAG = 0; int iBreak = -100;//跳出循环时的标列
                    foreach (int colMIN in colList)
                    {
                        #region
                        FLAG = 0;
                        string strQMIN = colMIN.ToString() + "号QMAX";
                        TargetedValueEntity SOPQMAXValue = getTargetedValue(strQMIN, "SOP", enumTargetedValueTableType.DieselOil);

                        if (SOPQMAXValue != null && SOPQMAXValue.fValue != null)
                        {
                            if (SOPSummary != null && SOPSummary.fVALUE != null)
                            {
                                if (SOPSummary.fVALUE.Value < SOPQMAXValue.fValue.Value-1)
                                {
                                    FLAG = 1;
                                    iBreak = colMIN;
                                    break;
                                }
                                else if (SOPSummary.fVALUE.Value <= SOPQMAXValue.fValue.Value)
                                {
                                    FLAG = 1;
                                    iBreak = colMIN;
                                    critical = true;
                                    break;
                                }
                            }
                            else 
                            {
                                FLAG = -1;
                                iBreak = colMIN;
                                break;
                            }                              
                        }
                        else//无指标值，也认为满足条件指标
                        {
                            FLAG = -2;
                            iBreak = colMIN;
                            break;
                        }                                               
                        #endregion
                    }
                    #endregion

                    summary += "\r\n";
                    summary += ICPSummary.TEXT + "～" + ECPSummary.TEXT + "℃柴油馏分的收率为" + WYSummary.TEXT + "%。";

                    TargetedValueEntity D20QMAXValue = getTargetedValue(iBreak.ToString()+"号QMAX", "D20", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity V02QMAXValue = getTargetedValue(iBreak.ToString()+"号QMAX", "V02", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity SULQMAXValue = getTargetedValue(iBreak.ToString()+"号QMAX", "SUL", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity CIQMAXValue = getTargetedValue(iBreak.ToString()+"号QMAX", "CI", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity ACDQMAXValue = getTargetedValue(iBreak.ToString()+"号QMAX", "ACD", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity CC3QMAXValue = getTargetedValue(iBreak.ToString()+"号QMAX", "CC3", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity CFPQMAXValue = getTargetedValue(iBreak.ToString()+"号QMAX", "CFP", enumTargetedValueTableType.DieselOil);


                    TargetedValueEntity D20QMINValue = getTargetedValue(iBreak.ToString()+"号QMIN", "D20", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity V02QMINValue = getTargetedValue(iBreak.ToString()+"号QMIN", "V02", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity SULQMINValue = getTargetedValue(iBreak.ToString()+"号QMIN", "SUL", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity CIQMINValue = getTargetedValue(iBreak.ToString()+"号QMIN", "CI", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity ACDQMINValue = getTargetedValue(iBreak.ToString()+"号QMIN", "ACD", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity CFPQMINValue = getTargetedValue(iBreak.ToString()+"号QMIN", "CFP", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity CC3QMINValue = getTargetedValue(iBreak.ToString() + "号QMIN", "CC3", enumTargetedValueTableType.DieselOil);


                    if (FLAG == 1)
                    {
                        #region ""
                        string temp = "密度为" +D20Summary.TEXT + "g/cm3，";
                        int sFir = 0, sSec = 0;
                        if (D20QMAXValue != null && D20QMAXValue.fValue != null)
                        {
                            if (D20Summary != null && D20Summary.fVALUE != null && D20Summary.fVALUE.Value > D20QMAXValue.fValue.Value)
                            {
                                temp = "密度偏大，为" +D20Summary.TEXT + "g/cm3，";
                                sSec = -1;
                            }                               
                        }
                        if (D20QMINValue != null && D20QMINValue.fValue != null)
                        {
                            if (D20Summary != null && D20Summary.fVALUE != null && D20Summary.fVALUE.Value <= D20QMINValue.fValue.Value)
                            {
                                temp = "密度偏小，为" +D20Summary.TEXT + "g/cm3，";
                                sSec = -1;
                            }                               
                        }
                        summary += temp;
                        if(critical == false)
                            summary += "硫含量为" + strSULTEXT + "，凝点为" + SOPSummary.TEXT + "℃";
                        else
                            summary += "硫含量为" + strSULTEXT + "，凝点卡边，为" + SOPSummary.TEXT + "℃";

                        temp = string .Empty ;

                        if (CFPQMAXValue != null && CFPQMAXValue.fValue != null)
                        {
                            if (CFPSummary != null && CFPSummary.fVALUE != null && CFPSummary.fVALUE.Value > CFPQMAXValue.fValue.Value)
                            {
                                temp = "，冷滤点偏高，为" +CFPSummary.TEXT + "℃";
                                sSec = -1;
                            }  
                            else if(CFPSummary != null && CFPSummary.fVALUE != null && CFPSummary.fVALUE.Value >= CFPQMAXValue.fValue.Value - 1) 
                                temp = "，冷滤点卡边，为" +CFPSummary.TEXT + "℃";
                        }
                        //if (CFPQMINValue != null && CFPQMINValue.fValue != null)用不到此情况
                        //{
                        //    if (CFPSummary != null && CFPSummary.fVALUE != null && CFPSummary.fVALUE.Value <= CFPQMINValue.fValue.Value)
                        //    {
                        //        sSec = -1;
                        //    }
                        //}
                        summary += temp;


                        temp = "，十六烷指数为" + CISummary.TEXT;
                        if (CIQMAXValue != null && CIQMAXValue.fValue != null)
                        {
                            if (CISummary != null && CISummary.fVALUE != null && CISummary.fVALUE.Value > CIQMAXValue.fValue.Value)
                            {
                                temp = "，十六烷指数偏高，为" +CISummary.TEXT ;
                                sSec = -1;
                            }                               
                        }
                        if (CIQMINValue != null && CIQMINValue.fValue != null)
                        {
                            if (CISummary != null && CISummary.fVALUE != null && CISummary.fVALUE.Value <= CIQMINValue.fValue.Value)
                            {
                                sSec = -1;
                                temp = "，十六烷指数偏低，为" +CISummary.TEXT ;
                            }
                        }
                        summary += temp;
 
                        if (SULQMAXValue != null && SULQMAXValue.fValue != null)
                        {
                            if (SULSummary != null && SULSummary.fVALUE != null && SULSummary.fVALUE.Value > SULQMAXValue.fValue.Value)
                            {
                                sFir = 1;
                            }                               
                        }
                        if (SULQMINValue != null && SULQMINValue.fValue != null)
                        {
                            if (SULSummary != null && SULSummary.fVALUE != null && SULSummary.fVALUE.Value <= SULQMINValue.fValue.Value)
                            {
                                sFir = 1;
                            }
                        }

                        if (ACDQMAXValue != null && ACDQMAXValue.fValue != null)
                        {
                            if (ACDSummary != null && ACDSummary.fVALUE != null && ACDSummary.fVALUE.Value > ACDQMAXValue.fValue.Value)
                            {
                                sFir = 1;
                            }
                        }
                        if (ACDQMINValue != null && ACDQMINValue.fValue != null)
                        {
                            if (ACDSummary != null && ACDSummary.fVALUE != null && ACDSummary.fVALUE.Value <= ACDQMINValue.fValue.Value)
                            {
                                sFir = 1;
                            }
                        }

                        if (CC3QMAXValue != null && CC3QMAXValue.fValue != null)
                        {
                            if (CC3Summary != null && CC3Summary.fVALUE != null && CC3Summary.fVALUE.Value > CC3QMAXValue.fValue.Value)
                            {
                                sFir = 1;
                            }
                        }
                        if (CC3QMINValue != null && CC3QMINValue.fValue != null)
                        {
                            if (CC3Summary != null && CC3Summary.fVALUE != null && CC3Summary.fVALUE.Value <= CC3QMINValue.fValue.Value)
                            {
                                sFir = 1;
                            }
                        }

                        if (sSec == 0)
                        {
                             if (sFir == 0)
                                 summary += "，可生产" + iBreak.ToString() + "号柴油。";
                             else 
                                 summary += "，精制后可生产" + iBreak.ToString() + "号柴油。";
                        }                       
                        else
                            summary += "。";
                        #endregion
                    }
                    else if (FLAG == 0)//找不到相应的产品指标
                    { 
                        summary += "密度为" + D20Summary.TEXT + "g/cm3，硫含量为" + strSULTEXT + "，凝点偏高，为" + SOPSummary.TEXT + "℃，十六烷指数为"+
                            CISummary.TEXT +"。";
                    }
                    else if (FLAG == -1)
                    {
                        summary += "密度为" + D20Summary.TEXT + "g/cm3，硫含量为" + strSULTEXT + "，凝点为" + SOPSummary.TEXT + "℃，十六烷指数为" +
                                CISummary.TEXT + "。";
                    }
                    else
                    {
                        MessageBox.Show("结论指标值配置表中，柴油凝点无指标值!", "提示信息！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        summary += "密度为" + D20Summary.TEXT + "g/cm3，硫含量为" + strSULTEXT + "，凝点为" + SOPSummary.TEXT + "℃，十六烷指数为" +
                           CISummary.TEXT + "。";
                    }            
                }
            }
            catch (Exception ex)
            {
                Log.Error("柴油的简评错误：" + ex.ToString());
            }
        }
        
        
        /// <summary>
        /// 蜡油简评
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="dataList"></param>
        /// <param name="rowList"></param>
        private void getSimSummaryFromVGO(ref string summary, List<OilDataEntity> dataList, List<OilTableRowEntity> rowList, string cutName)
        {
            try 
            {
                OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.ID == (int)enumOilTableTypeComparisonTable.VGO).FirstOrDefault();
 
                string str = string.Empty;    
                SummaryEntity ICPSummary = getTextandValue(ref str ,"ICP", dataList, rowList, EnumTableType.Wide,true ,cutName);
                SummaryEntity ECPSummary = getTextandValue(ref str, "ECP", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity WYSummary = getTextandValue(ref str, "WY", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity D20Summary = getTextandValue(ref str, "D20", dataList, rowList, EnumTableType.Wide, true, cutName);
                //SummaryEntity SULSummary = getTextandValue(ref str, "SUL", dataList, rowList, EnumTableType.Wide);
                SummaryEntity SULSummary = getTextandValueBeforeCondenseDec(ref str, "SUL", dataList, rowList, EnumTableType.Wide, true, cutName);

                SummaryEntity SAHSummary = getTextandValue(ref str, "SAH", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity KFCSummary = getTextandValue(ref str, "KFC", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity CPPSummary = getTextandValue(ref str, "CPP", dataList, rowList, EnumTableType.Wide, true, cutName);
                bool work = false;
                if (!string.IsNullOrWhiteSpace(str))
                {
                    DialogResult r = MessageBox.Show(str + " 是否继续？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (r == DialogResult.Yes)
                        work = true;
                    else
                        return;
                }
                else
                    work = true;
                if (work)
                {
                    summary += "\r\n";

                    #region "硫含量"
                    string strSULTEXT = string.Empty;
                    OilTableRowEntity SULRow = rowList.Where(o => o.itemCode == "SUL" && o.oilTableTypeID == (int)EnumTableType.Wide).FirstOrDefault();
                    if (SULSummary != null && SULSummary.fTEXT < 0.1)
                        strSULTEXT = oilTool.calDataDecLimit((SULSummary.fTEXT * 10000).ToString(), 1, SULRow.valDigital) + "μg/g";
                    else if (SULSummary != null && SULSummary.fTEXT >= 0.1)
                        strSULTEXT = oilTool.calDataDecLimit(SULSummary.TEXT, 2, SULRow.valDigital) + "%";
                    #endregion 

                    summary += ICPSummary.TEXT + "～" + ECPSummary.TEXT + "℃蜡油馏分的收率为" + WYSummary.TEXT +
                               "%。密度为" + D20Summary.TEXT + "g/cm3，硫含量为" + strSULTEXT;

                    #region "石脑油SAH"
                    string TEMP = "，饱和分含量为" + SAHSummary.TEXT + "%，K值为" + KFCSummary.TEXT+"。";
                    //LevelValueEntity SAHLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "SAH" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity CPPLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "CPP" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity KFCLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "KFC" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (CPPLevelValue != null && CPPLevelValue.More != null && CPPSummary.fVALUE != null)
                    {
                        if (CPPSummary.fVALUE.Value > CPPLevelValue.More.Value)
                        {
                            TEMP = "，饱和分含量较高，为" + SAHSummary.TEXT + "%";

                            if (KFCLevelValue.More != null && KFCSummary.fVALUE != null)
                            {
                                if (KFCSummary.fVALUE.Value > KFCLevelValue.More.Value)
                                {
                                    TEMP += "，K值为" + KFCSummary.TEXT;
                                }
                            }
                            TEMP += "，裂化性能较好。";
                        }
                    }
                    if (CPPLevelValue != null && CPPLevelValue.Less != null && CPPSummary.fVALUE != null)
                    {
                        if (CPPSummary.fVALUE.Value < CPPLevelValue.Less.Value)
                        {
                            TEMP = "，饱和分含量较低，为" + SAHSummary.TEXT + "%";
                       
                            if (KFCLevelValue.Less != null && KFCSummary.fVALUE != null)
                            {
                                if (KFCSummary.fVALUE.Value < KFCLevelValue.Less.Value)
                                {
                                    TEMP += "，K值为" + KFCSummary.TEXT;
                                }
                            }
                            TEMP += "，裂化性能较差。";
                        }
                    }
                    summary += TEMP;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Error("蜡油的简评错误：" + ex.ToString());
            }
        }
        /// <summary>
        /// 宽馏分表简评
        /// </summary>
        /// <param name="summary"></param>
        private void getSimSummaryFromWide(ref string summary)
        {
            List<OilDataEntity> wideDataList = this._OilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Wide).Where(o=>o.labData != string.Empty || o.calData != string.Empty).ToList();
            List<OilDataEntity> wctDataList = wideDataList.Where(o => o.OilTableRow.itemCode == "WCT").ToList();
            List<OilTableRowEntity> rowList = this._OilA.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide && o.isDisplay == true).ToList();
            foreach (OilDataEntity col in wctDataList)
            {
                List<OilDataEntity> dataList = wideDataList.Where(o => o.ColumnIndex  == col.ColumnIndex).ToList();

                switch (col.calData)
                {
                    case "石脑油":
                    case "乙烯料": 
                    case "重整料":
                    case "溶剂油": getSimSummaryFromNaphtha(ref summary, dataList, rowList, col.calData+"馏分"+ col.OilTableCol.colName); break;
                    case "航煤": getSimSummaryFromAviationKerosene(ref summary, dataList, rowList, col.calData +"馏分"+  col.OilTableCol.colName); break;
                    case "柴油": getSimSummaryFromDieselOil(ref summary, dataList, rowList, col.calData + "馏分" + col.OilTableCol.colName); break;
                    case "蜡油": getSimSummaryFromVGO(ref summary, dataList, rowList, col.calData + "馏分" + col.OilTableCol.colName); break;                                                
                }
            }        
        }
        /// <summary>
        /// 渣油简评
        /// </summary>
        /// <param name="summary"></param>
        private void getSimSummaryFromResidue(ref string summary)
        {
            OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.ID == (int)enumOilTableTypeComparisonTable.Residue).FirstOrDefault();

            List<OilDataEntity> residueDataList = this._OilA.OilDatas.Where (o=>o.OilTableTypeID == (int)EnumTableType.Residue).Where(o=>o.labData != string.Empty || o.calData != string.Empty).ToList();
            List<OilTableColEntity> colList = this._OilA.OilTableCols.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();
            List<OilTableRowEntity> rowList = this._OilA.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue && o.isDisplay == true).ToList();
            foreach (OilTableColEntity col in colList)
            {
                List<OilDataEntity> dataList = residueDataList.Where(o => o.oilTableColID == col.ID).ToList();
                if (dataList.Count == 0)
                    continue;
                string ERROR = string.Empty;
                string cutName = "馏分" + col.colName;
                SummaryEntity ICPSummary = getTextandValue(ref ERROR, "ICP", dataList, rowList, EnumTableType.Residue, true, cutName);
                SummaryEntity WYSummary = getTextandValue(ref ERROR, "WY", dataList, rowList, EnumTableType.Residue, true, cutName);
                //SummaryEntity SULSummary = getTextandValue(ref ERROR, "SUL", dataList, rowList, EnumTableType.Residue);
                SummaryEntity SULSummary = getTextandValueBeforeCondenseDec(ref ERROR, "SUL", dataList, rowList, EnumTableType.Residue, true, cutName);
                SummaryEntity CCRSummary = getTextandValue(ref ERROR, "CCR", dataList, rowList, EnumTableType.Residue, true, cutName);
                //SummaryEntity N2Summary = getTextandValue(ref ERROR, "N2", dataList, rowList, EnumTableType.Residue);
                SummaryEntity N2Summary = getTextandValueBeforeCondenseDec(ref ERROR, "N2", dataList, rowList, EnumTableType.Residue, true, cutName);
                SummaryEntity NIVSummary = getTextandValue(ref ERROR, "NIV", dataList, rowList, EnumTableType.Residue, true, cutName);
                bool work = false;
                if (!string.IsNullOrWhiteSpace(ERROR))
                {
                    if (MessageBox.Show(ERROR + "是否继续？", "提示信息！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        work = true;
                    else
                        return;
                }
                else
                    work = true;

                if (work)
                {
                    summary += "\r\n";

                    #region "硫含量"
                    string strSULTEXT = string.Empty;
                    OilTableRowEntity SULRow = rowList.Where(o => o.itemCode == "SUL" && o.oilTableTypeID == (int)EnumTableType.Residue).FirstOrDefault();             
                    if (SULSummary !=null &&  SULSummary.fTEXT < 0.1)
                        strSULTEXT = oilTool.calDataDecLimit((SULSummary.fTEXT * 10000).ToString(),1, SULRow.valDigital) + "μg/g";
                    else if (SULSummary != null && SULSummary.fTEXT >= 0.1)
                        strSULTEXT = oilTool.calDataDecLimit(SULSummary.TEXT, 2, SULRow.valDigital) + "%";
                    #endregion 

                    #region "氮含量"
                    string strN2TEXT = string.Empty;
                    OilTableRowEntity N2Row = rowList.Where(o => o.itemCode == "N2" && o.oilTableTypeID == (int)EnumTableType.Residue).FirstOrDefault();             
                    if (N2Summary != null && N2Summary.fTEXT >=1000)
                        strN2TEXT = oilTool.calDataDecLimit((N2Summary.fTEXT / 10000).ToString(), 2, SULRow.valDigital) + "%";
                    else if (N2Summary != null && N2Summary.fTEXT < 1000)
                        strN2TEXT = oilTool.calDataDecLimit((N2Summary.TEXT).ToString(), 1, SULRow.valDigital) + "μg/g";
                    #endregion 

                    summary += ">" + ICPSummary.TEXT + "℃渣油馏分的收率为" + WYSummary.TEXT + "%。硫含量为" + strSULTEXT + "，氮含量为";

                    summary += strN2TEXT + "。残炭值为" + CCRSummary.TEXT + "%" + "，金属镍、钒含量加和为" + NIVSummary.TEXT + "μg/g。";
                }
            }
        }
        /// <summary>
        /// 从原油表中获取简评数据
        /// </summary>
        /// <param name="summary"></param>
        private void getSimSummaryFromWhole(ref string summary)
        {
            try
            {
                List<OilDataEntity> dataList = this._OilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).Where(o => o.labData != string.Empty || o.calData != string.Empty).ToList();
                List<OilTableRowEntity> rowList = this._OilA.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole && o.isDisplay == true).ToList();
                OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();

                string strERROR = string.Empty;
                SummaryEntity D20Summary = getTextandValue(ref strERROR, "D20", dataList, rowList, EnumTableType.Whole,true );
                SummaryEntity APISummary = getTextandValue(ref strERROR, "API", dataList, rowList, EnumTableType.Whole, true);
                //SummaryEntity SULSummary = getTextandValue(ref strERROR, "SUL", dataList, rowList, EnumTableType.Whole);
                //SummaryEntity N2Summary = getTextandValue(ref strERROR, "N2", dataList, rowList, EnumTableType.Whole);
                SummaryEntity SULSummary = getTextandValueBeforeCondenseDec(ref strERROR, "SUL", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity N2Summary = getTextandValueBeforeCondenseDec(ref strERROR, "N2", dataList, rowList, EnumTableType.Whole, true);

                SummaryEntity SOPSummary = getTextandValue(ref strERROR, "SOP", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity NETSummary = getTextandValue(ref strERROR, "NET", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity NISummary = getTextandValue(ref strERROR, "NI", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity VSummary = getTextandValue(ref strERROR, "V", dataList, rowList, EnumTableType.Whole, true);
                bool work = false;
                if (!string.IsNullOrWhiteSpace(strERROR))
                {
                    DialogResult r = MessageBox.Show(strERROR+"是否继续？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (r == DialogResult.Yes)
                        work = true;
                    else
                        return;
                }
                else
                    work = true;
                if (work)
                {
                    summary += getCNASummaryFromOilInfo().TEXT + "原油属" + getSCLSummaryFromOilInfo().TEXT + getTYPSummaryFromOilInfo().TEXT +
                        "原油。其API值为" + APISummary.TEXT + "，20℃密度为" + D20Summary.TEXT + "g/cm3。";


                    #region "原油性质硫含量"
                    LevelValueEntity SULLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "SUL" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    #region "硫含量"
                    string strSULTEXT = string.Empty;
                    OilTableRowEntity SULRow = rowList.Where(o => o.itemCode == "SUL" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                    if (SULSummary != null && SULSummary.fTEXT < 0.1)
                        strSULTEXT = oilTool.calDataDecLimit((SULSummary.fTEXT * 10000).ToString(), 1, SULRow.valDigital) + "μg/g";
                    else if (SULSummary != null && SULSummary.fTEXT >= 0.1)
                        strSULTEXT = oilTool.calDataDecLimit(SULSummary.TEXT, 2, SULRow.valDigital) + "%";
                    #endregion

                    string SULTEMP = "硫含量为" + strSULTEXT + "，";
                    if (SULLevelValue != null && SULLevelValue.More != null && SULSummary.fVALUE != null)
                    {
                        if (SULSummary.fVALUE.Value > SULLevelValue.More.Value)
                            SULTEMP = "硫含量较高，为" + strSULTEXT + "，";
                    }

                    if (SULLevelValue != null && SULLevelValue.Less != null && SULSummary.fVALUE != null)
                    {
                        if (SULSummary.fVALUE.Value < SULLevelValue.Less.Value)
                            SULTEMP = "硫含量较低，为" + strSULTEXT + "，";
                    }
                    summary += SULTEMP;
                    #endregion

                    #region "原油性质氮含量"
                    int sign = 0;//结论的依据标志。

                    #region "氮含量"
                    string strN2TEXT = string.Empty;
                    OilTableRowEntity N2Row = rowList.Where(o => o.itemCode == "N2" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                    if (N2Summary != null && N2Summary.fTEXT >= 1000)
                        strN2TEXT = oilTool.calDataDecLimit((N2Summary.fTEXT / 10000).ToString(), 2, SULRow.valDigital) + "%";
                    else if (N2Summary != null && N2Summary.fTEXT < 1000)
                        strN2TEXT = oilTool.calDataDecLimit((N2Summary.TEXT).ToString(), 1, SULRow.valDigital) + "μg/g";
                    #endregion

                    string N2TEMP = "氮含量为" + strN2TEXT + "。";
                    LevelValueEntity N2LevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "N2" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (N2LevelValue != null && N2LevelValue.More != null && N2Summary.fVALUE != null)
                    {
                        if (N2Summary.fVALUE.Value > N2LevelValue.More.Value)
                            N2TEMP = "氮含量较高，为" + strN2TEXT + "。";
                    }
                    if (N2LevelValue != null && N2LevelValue.Less != null && N2Summary.fVALUE != null)
                    {
                        if (N2Summary.fVALUE.Value < N2LevelValue.Less.Value)
                        {
                            N2TEMP = "";
                            sign++;
                        }
                    }
                    summary += N2TEMP;
                    #endregion

                    #region "原油性质凝点"
                    string SOPTEMP = "凝点为" + SOPSummary.TEXT + "℃。";
                    LevelValueEntity SOPLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "SOP" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (SOPLevelValue != null && SOPLevelValue.More != null && SOPSummary.fVALUE != null)
                    {
                        if (SOPSummary.fVALUE.Value >= SOPLevelValue.More.Value)
                            SOPTEMP = "凝点较高，为" + SOPSummary.TEXT + "℃。";
                    }
                    if (SOPLevelValue != null && SOPLevelValue.Less != null && SOPSummary.fVALUE != null)
                    {
                        if (SOPSummary.fVALUE.Value < SOPLevelValue.Less.Value)
                        {
                            SOPTEMP = "";
                            sign += 2;
                        }
                    }
                    summary += SOPTEMP;
                    #endregion

                    #region "原油性质酸值"
                    string NETTEMP = "酸值为" + NETSummary.TEXT + "mgKOH/g，";
                    LevelValueEntity NETLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "NET" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (NETLevelValue != null && NETLevelValue.More != null && NETSummary.fVALUE != null)
                    {
                        if (NETSummary.fVALUE.Value >= NETLevelValue.More.Value)
                            NETTEMP = "酸值较高，为" + NETSummary.TEXT + "mgKOH/g，";
                    }
                    if (NETLevelValue != null && NETLevelValue.Less != null && NETSummary.fVALUE != null)
                    {
                        if (NETSummary.fVALUE.Value < NETLevelValue.Less.Value)
                        {
                            NETTEMP = "";
                            sign += 4;
                        }
                    }
                    summary += NETTEMP;
                    #endregion

                    #region "sign"
                    switch (sign)
                    {
                        case 0:
                            summary.Remove(summary.Length - 1, 1);
                            break;
                        case 1:
                            summary += "氮含量较低。";
                            break;
                        case 2:
                            summary += "凝点较低。";
                            break;
                        case 3:
                            summary += "氮含量、凝点较低。";
                            break;
                        case 4:
                            summary += "酸值较低。";
                            break;
                        case 5:
                            summary += "氮含量、酸值较低。";
                            break;
                        case 6:
                            summary += "凝点、酸值较低。";
                            break;
                        case 7:
                            summary += "氮含量、凝点和酸值较低。";
                            break;
                    }
                    #endregion

                    sign = 0;
                    #region "原油性质镍"
                    string NITEMP = string.Empty;
                    LevelValueEntity NILevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "NI" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (NILevelValue != null && NISummary != null && NETLevelValue.Less != null && NISummary.fVALUE != null)
                    {
                        if (NISummary.fVALUE.Value < NILevelValue.Less.Value)
                        {
                            //NITEMP = "";
                            sign += 1;
                        }
                        else if (NISummary.fVALUE.Value > NILevelValue.More.Value)
                        {
                            NITEMP += "金属镍含量较高，为" + NISummary.TEXT + "μg/g，";
                        }
                        else
                        {
                            NITEMP += "金属镍含量为" + NISummary.TEXT + "μg/g，";
                        }
                    }
                    //summary += NITEMP;
                    #endregion

                    #region "原油性质钒"
                    LevelValueEntity VLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "V" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    string tempVS = string.Empty;
                    if (VLevelValue != null && VLevelValue.Less != null && VSummary != null && VSummary.fVALUE != null)
                    {
                        if (VSummary.fVALUE.Value < VLevelValue.Less.Value)
                        {
                            sign += 2;
                        }
                        else if (VSummary.fVALUE.Value > VLevelValue.More.Value)
                        {
                            tempVS += "钒含量较高，为" + VSummary.TEXT + "μg/g。";
                        }
                        else
                        {
                            tempVS += "钒含量为" + VSummary.TEXT + "μg/g。";
                        }
                    }
                    #endregion

                    #region "sign"
                    switch (sign)
                    {
                        case 0:
                            summary += "金属镍、钒含量较高，镍钒加和为" + (NISummary.fVALUE.Value  + VSummary.fVALUE.Value)  + "μg/g。";
                            break;
                        case 1:
                            summary += "金属镍含量较低," + tempVS;
                            break;
                        case 2:
                            summary += NITEMP+"钒含量较低。";
                            break;
                        case 3:
                            summary += "金属镍、钒含量较低。";
                            break;
                        default:
                            summary = summary.Remove(summary.Length - 1);
                            //summary += "钒含量为" + VSummary.TEXT + "μg/g。";
                            summary += "。";
                            break;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Error("原油的简评错误："+ex.ToString());
            }
        }
        /// <summary>
        /// 获取简评数据
        /// </summary>
        public string getSimSummary()
        {
            string summary = string.Empty;
            try
            {
                getSimSummaryFromWhole(ref summary);
                getSimSummaryFromWide(ref summary);
                getSimSummaryFromResidue(ref summary);
            }
            catch (Exception ex)
            {
                Log.Error("简评结果："+ex.ToString());
            }
            return summary;
        }
        OilTools oilTool = new OilTools();
        /// <summary>
        /// 从数据库中获取数据
        /// </summary>
        /// <param name="ERROE"></param>
        /// <param name="itemCode"></param>
        /// <param name="dataList"></param>
        /// <param name="rowList"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private SummaryEntity getTextandValue(ref string ERROE,  string itemCode, List<OilDataEntity> dataList, List<OilTableRowEntity> rowList, EnumTableType tableType,bool Check,string colName = "")
        {
            SummaryEntity simSum = new SummaryEntity();

            OilDataEntity oilData = dataList.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();
            OilTableRowEntity row = rowList.Where(o => o.itemCode == itemCode && o.oilTableTypeID == (int)tableType).FirstOrDefault();
           
            if (oilData == null && Check)
            {
                #region "输入判断"
                if (row != null)
                {
                    switch (tableType)
                    {
                        case EnumTableType.Whole:
                            ERROE += "原性油质表" + colName +"缺少"+ row.itemName + "的数据，请补充！";
                            break;
                        case EnumTableType.GC:
                            ERROE += "原油GC表缺少" + colName + "缺少" + row.itemName + "的数据，请补充！";
                            break;
                        case EnumTableType.Narrow:
                            ERROE += "原油窄馏分表" + colName + "缺少" + row.itemName + "的数据，请补充！";
                            break;
                        case EnumTableType.Wide:
                            ERROE += "原油宽馏分表" + colName + "缺少" + row.itemName + "的数据，请补充！";
                            break;
                        case EnumTableType.Residue:
                            ERROE += "原油渣油表" + colName + "缺少" + row.itemName + "的数据，请补充！";
                            break;
                    }
                }
                else
                {
                    switch (tableType)
                    {
                        case EnumTableType.Whole:
                            ERROE += "原油性质表" + colName + "缺少" + itemCode + "的数据,请检查！";
                            break;
                        case EnumTableType.GC:
                            ERROE += "原油GC表" + colName + "缺少" + itemCode + "的数据,请检查！";
                            break;
                        case EnumTableType.Narrow:
                            ERROE += "原油窄馏分表" + colName + "缺少" + itemCode + "的数据,请检查！";
                            break;
                        case EnumTableType.Wide:
                            ERROE += "原油宽馏分表" + colName + "缺少" + itemCode + "的数据，请补充！";
                            break;
                        case EnumTableType.Residue:
                            ERROE += "原油渣油表" + colName + "缺少" + itemCode + "的数据,请检查！";
                            break;
                    }
                }
                return simSum;

                #endregion
            }
            else if(oilData !=null)
            {
                #region "主体判断"
                string cal = oilData.calShowData;
                string lab = oilData.labShowData;
                if (oilData.calShowData == oilData.labShowData && oilData.calShowData != string.Empty && oilData.labShowData != string.Empty)
                {
                    simSum.TEXT = lab;
                    simSum.VALUE = lab;

                    if (itemCode != "CC2" && itemCode != "CC3")
                    {
                        if (simSum.fVALUE == null && Check)
                        {
                            switch (tableType)
                            {
                                case EnumTableType.Whole:
                                    ERROE += "原油性质表" + colName + oilData.OilTableRow.itemName + "的校正值非数字，请修改！";
                                    break;
                                case EnumTableType.GC:
                                    ERROE += "原油GC表" + colName + oilData.OilTableRow.itemName + "的校正值非数字，请修改！";
                                    break;
                                case EnumTableType.Narrow:
                                    ERROE += "原油窄馏分表" + colName + oilData.OilTableRow.itemName + "的校正值非数字，请修改！";
                                    break;
                                case EnumTableType.Wide:
                                    ERROE += "原油宽馏分表" + colName + oilData.OilTableRow.itemName + "的校正值非数字，请修改！";
                                    break;
                                case EnumTableType.Residue:
                                    ERROE += "原油渣油表" + colName + oilData.OilTableRow.itemName + "的校正值非数字，请修改！";
                                    break;
                            }
                        }
                    }
                    
                }
                else if (oilData.calShowData != oilData.labShowData && oilData.calShowData != string.Empty && oilData.labShowData != string.Empty)
                {
                    simSum.TEXT = lab;
                    simSum.VALUE = cal;
                }
                else if (oilData.labShowData == string.Empty && oilData.calShowData != string.Empty)
                {
                    simSum.TEXT = cal;
                    simSum.VALUE = cal;
                }
                else if (oilData.labShowData != string.Empty && oilData.calShowData == string.Empty)
                {
                    simSum.TEXT = oilData.labData;
                    if (Check)
                    {
                        switch (tableType)
                        {
                            case EnumTableType.Whole:
                                ERROE += "原油性质表" + colName + "缺少" + oilData.OilTableRow.itemName + "的校正值数据，请补充！";
                                break;
                            case EnumTableType.GC:
                                ERROE += "原油GC表" + colName + "缺少" + oilData.OilTableRow.itemName + "的校正值数据，请补充！";
                                break;
                            case EnumTableType.Narrow:
                                ERROE += "原油窄馏分表" + colName + "缺少" + oilData.OilTableRow.itemName + "的校正值数据，请补充！";
                                break;
                            case EnumTableType.Wide:
                                ERROE += "原油宽馏分表" + colName + "缺少" + oilData.OilTableRow.itemName + "的校正值数据，请补充！";
                                break;
                            case EnumTableType.Residue:
                                ERROE += "原油渣油表" + colName + "缺少" + oilData.OilTableRow.itemName + "的校正值数据，请补充！";
                                break;
                        }
                    }
                }
                else if (oilData.labShowData == string.Empty && oilData.calShowData == string.Empty)
                {
                    if (Check)
                    {
                        switch (tableType)
                        {
                            case EnumTableType.Whole:
                                ERROE += "原油性质表" + colName + "缺少" + oilData.OilTableRow.itemName + "的数据，请补充！";
                                break;
                            case EnumTableType.GC:
                                ERROE += "原油GC表" + colName + "缺少" + oilData.OilTableRow.itemName + "的数据，请补充！";
                                break;
                            case EnumTableType.Narrow:
                                ERROE += "原油窄馏分表" + colName + "缺少" + oilData.OilTableRow.itemName + "的数据，请补充！";
                                break;
                            case EnumTableType.Wide:
                                ERROE += "原油宽馏分表" + colName + "缺少" + oilData.OilTableRow.itemName + "的数据，请补充！";
                                break;
                            case EnumTableType.Residue:
                                ERROE += "原油渣油表" + colName + "缺少" + oilData.OilTableRow.itemName + "的数据，请补充！";
                                break;
                        }
                    }
                }
                #endregion 
            }
            return simSum; 
        }
       
        /// <summary>
        /// 小数位数精简之前的数据
        /// </summary>
        /// <param name="ERROE"></param>
        /// <param name="itemCode"></param>
        /// <param name="dataList"></param>
        /// <param name="rowList"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private SummaryEntity getTextandValueBeforeCondenseDec(ref string ERROE, string itemCode, List<OilDataEntity> dataList, List<OilTableRowEntity> rowList, EnumTableType tableType, bool Check, string colName = "")
        {
            SummaryEntity simSum = new SummaryEntity();

            OilDataEntity oilData = dataList.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();
            OilTableRowEntity row = rowList.Where(o => o.itemCode == itemCode && o.oilTableTypeID == (int)tableType).FirstOrDefault();

            if (oilData == null)
            {
                #region "输入判断"
                if (row != null)
                {
                    switch (tableType)
                    {
                        case EnumTableType.Whole:
                            ERROE += "原油性质表缺少性质名称为" + row.itemName + "的性质数据请补充！";
                            break;
                        case EnumTableType.GC:
                            ERROE += "原油GC表缺少性质名称为" + row.itemName + "的性质数据请补充！";
                            break;
                        case EnumTableType.Narrow:
                            ERROE += "原油窄表缺少性质名称为" + row.itemName + "的性质数据请补充！";
                            break;
                        case EnumTableType.Residue:
                            ERROE += "渣油表缺少性质名称为" + row.itemName + "的性质数据请补充！";
                            break;
                    }
                }
                else
                {
                    switch (tableType)
                    {
                        case EnumTableType.Whole:
                            ERROE += "原油性质表不存在性质代码为" + itemCode + "的性质数据,请检查！";
                            break;
                        case EnumTableType.GC:
                            ERROE += "原油GC表不存在性质代码为" + itemCode + "的性质数据,请检查！";
                            break;
                        case EnumTableType.Narrow:
                            ERROE += "原油窄表不存在性质代码为" + itemCode + "的性质数据,请检查！";
                            break;
                        case EnumTableType.Residue:
                            ERROE += "渣油表不存在性质代码为" + itemCode + "的性质数据,请检查！";
                            break;
                    }
                }
                return simSum;

                #endregion
            }
            else
            {
                #region "主体判断"

                if (oilData.calData == oilData.labData && oilData.calData != string.Empty && oilData.labData != string.Empty)
                {
                    simSum.TEXT = oilData.labData;
                    simSum.VALUE = oilData.labData;
                }
                else if (oilData.calData != oilData.labData && oilData.calData != string.Empty && oilData.labData != string.Empty)
                {
                    simSum.TEXT = oilData.labData;
                    simSum.VALUE = oilData.calData;
                }
                else if (oilData.labData == string.Empty && oilData.calData != string.Empty)
                {
                    simSum.TEXT = oilData.calData;
                    simSum.VALUE = oilData.calData;
                }
                else if (oilData.labData == string.Empty && oilData.calData == string.Empty)
                {
                    switch (tableType)
                    {
                        case EnumTableType.Whole:
                            ERROE += "原油性质表缺少性质名称为" + oilData.OilTableRow.itemName + "的性质数据请补充！";
                            break;
                        case EnumTableType.GC:
                            ERROE += "原油GC表缺少性质名称为" + oilData.OilTableRow.itemName + "的性质数据请补充！";
                            break;
                        case EnumTableType.Narrow:
                            ERROE += "原油窄表缺少性质名称为" + oilData.OilTableRow.itemName + "的性质数据请补充！";
                            break;
                        case EnumTableType.Residue:
                            ERROE += "渣油表缺少性质名称为" + oilData.OilTableRow.itemName + "的性质数据请补充！";
                            break;
                    }
                }
                #endregion
            }
            return simSum;
        }
        /// <summary>
        /// 原油信息CNA
        /// </summary>
        /// <returns></returns>
        private SummaryEntity getCNASummaryFromOilInfo()
        {
            SummaryEntity CNASummary = new SummaryEntity();
              
            if (string.IsNullOrWhiteSpace (this._OilA.crudeName))
            {
                CNASummary.ERROE = "原油信息表缺少性质名称为" + "原油名称"+ "的性质数据请补充！";
                return CNASummary;
            }
            else 
            {
                CNASummary.TEXT = this._OilA.crudeName;
                CNASummary.VALUE = this._OilA.crudeName;
            }
              
            return CNASummary;
        }
        /// <summary>
        /// 原油信息SCL
        /// </summary>
        /// <returns></returns>
        private SummaryEntity getSCLSummaryFromOilInfo()
        {
            SummaryEntity SCLSummary = new SummaryEntity();

            if (string.IsNullOrWhiteSpace(this._OilA.sulfurLevel))
            {
                SCLSummary.ERROE = "原油信息表缺少性质名称为" + "硫水平" + "的性质数据请补充！";
                return SCLSummary;
            }
            else
            {
                SCLSummary.TEXT = this._OilA.sulfurLevel;
                SCLSummary.VALUE = this._OilA.sulfurLevel;
            }

            return SCLSummary;
        }
        /// <summary>
        /// 原油信息TYP
        /// </summary>
        /// <returns></returns>
        private SummaryEntity getTYPSummaryFromOilInfo()
        {
            SummaryEntity TYPSummary = new SummaryEntity();

            if (string.IsNullOrWhiteSpace(this._OilA.classification))
            {
                TYPSummary.ERROE = "原油信息表缺少性质名称为" + "基属" + "的性质数据请补充！";
                return TYPSummary;
            }
            else
            {
                TYPSummary.TEXT = this._OilA.classification;
                TYPSummary.VALUE = this._OilA.classification;
            }

            return TYPSummary;
        }
        /// <summary>
        /// 原油信息ACL
        /// </summary>
        /// <returns></returns>
        private SummaryEntity getACLSummaryFromOilInfo()
        {
            SummaryEntity ACLSummary = new SummaryEntity();

            if (string.IsNullOrWhiteSpace(this._OilA.acidLevel))
            {
                ACLSummary.ERROE = "原油信息表缺少性质名称为" + "酸水平" + "的性质数据请补充！";
                return ACLSummary;
            }
            else
            {
                ACLSummary.TEXT = this._OilA.acidLevel;
                ACLSummary.VALUE = this._OilA.acidLevel;
            }

            return ACLSummary;
        }

        #endregion 

        #region "详评"
        /// <summary>
        /// 获取详评数据
        /// </summary>
        public string  getDetailSummary()
        {
            string summary = string.Empty;

            getDetailSummaryFromWhole(ref summary);
            getDetailSummaryFromWide(ref summary);
            getDetialSummaryFromResidue(ref summary);

            return summary;
        }
        /// <summary>
        /// 获取原油性质详评数据
        /// </summary>
        private void getDetailSummaryFromWhole(ref string summary)
        {
            try 
            {
                List<OilDataEntity> dataList = this._OilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).Where(o => o.labData != string.Empty || o.calData != string.Empty).ToList();
                List<OilTableRowEntity> rowList = this._OilA.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole && o.isDisplay == true).ToList();
    
                OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.ID == (int)enumOilTableTypeComparisonTable.Whole).FirstOrDefault();
                string strTemp = string.Empty;
                SummaryEntity WAXSummary = getTextandValue(ref strTemp, "WAX", dataList, rowList, EnumTableType.Whole,true);
                SummaryEntity D20Summary = getTextandValue(ref strTemp, "D20", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity APISummary = getTextandValue(ref strTemp, "API", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity SULSummary = getTextandValue(ref strTemp, "SUL", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity N2Summary = getTextandValue(ref strTemp, "N2", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity SOPSummary = getTextandValue(ref strTemp, "SOP", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity CCRSummary = getTextandValue(ref strTemp, "CCR", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity NETSummary = getTextandValue(ref strTemp, "NET", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity NISummary = getTextandValue(ref strTemp, "NI", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity VSummary = getTextandValue(ref strTemp, "V", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity APHSummary = getTextandValue(ref strTemp, "APH", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity RESSummary = getTextandValue(ref strTemp, "RES", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity FPOSummary = getTextandValue(ref strTemp, "FPO", dataList, rowList, EnumTableType.Whole, true);
                SummaryEntity NIVSummary = getTextandValue(ref strTemp, "NIV", dataList, rowList, EnumTableType.Whole, true);


                bool work = false;
                if (!string.IsNullOrWhiteSpace(strTemp))
                {
                    if (MessageBox.Show(strTemp + "是否继续？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        work = true;
                    else
                        return;
                }
                else 
                    work = true;
                if (work)
                {
                    #region "summary"
                    summary += getCNASummaryFromOilInfo().TEXT; 

                    #region "原油性质D20"
                    LevelValueEntity D20LevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "D20" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    string D20TEMP = "原油20℃的密度为" + D20Summary.TEXT + "g/cm3，";
                            
                    if (D20LevelValue!= null && D20LevelValue.Less != null && D20Summary.fVALUE != null)
                    {
                        if (D20Summary.fVALUE < D20LevelValue.Less.Value)
                            D20TEMP = "原油20℃的密度较轻，为" + D20Summary.TEXT + "g/cm3，";
                    }
                    else if (D20LevelValue != null && D20LevelValue.More != null && D20Summary.fVALUE != null)
                    {
                        if (D20Summary.fVALUE > D20LevelValue.More.Value)
                            D20TEMP = "原油20℃的密度较重，为" + D20Summary.TEXT + "g/cm3，";
                    }                
                    summary += D20TEMP;
                    #endregion

                    #region "原油性质WAX 、SOP、FPO"
                    LevelValueEntity WAXLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "WAX" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity SOPLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "SOP" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    
                    bool sign = false;
                    if (WAXLevelValue != null && WAXLevelValue.More != null && WAXSummary.fVALUE != null)
                    {
                        if (WAXSummary.fVALUE > WAXLevelValue.More.Value)
                        {
                            summary += "蜡含量较高，为" + WAXSummary.TEXT + "%,";
                            sign = true;
                        }
                    }
                    string SOPTEMP = "凝点较低，为" + SOPSummary.TEXT + "℃，";
                    if (SOPLevelValue != null && SOPLevelValue.More != null && SOPSummary.fVALUE != null)
                    {
                        if (SOPSummary.fVALUE.Value > SOPLevelValue.More.Value)
                            if (sign)
                                SOPTEMP = "相应地，凝点较高，为" + SOPSummary.TEXT + "℃，储运中要注意保温。";
                            else
                                SOPTEMP = "凝点较高，为" + SOPSummary.TEXT + "℃，";
                    }
                    if (SOPLevelValue != null && SOPLevelValue.Less != null && SOPSummary.fVALUE != null)
                    {
                        if (SOPSummary.fVALUE.Value < SOPLevelValue.Less.Value)
                        {
                            SOPTEMP = "凝点较低，为" + SOPSummary.TEXT + "℃，";
                        }
                    }
                    summary += SOPTEMP;

                    #endregion

                    #region "原油性质FPO"
                    LevelValueEntity FPOLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "FPO" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    
                    if (FPOLevelValue != null && FPOLevelValue.Less != null && FPOSummary.fVALUE != null)
                    {
                        if (FPOSummary.fVALUE.Value < FPOLevelValue.Less.Value)
                            summary += "原油的开口闪点较低，为" + FPOSummary.TEXT + "℃，应在储存和运输过程中注意安全问题。";
                    }

                    #endregion

                    #region "原油性质酸值"
                    string NETTEMP = "酸值为" + NETSummary.TEXT + "mgKOH/g，属含酸原油。";
                    LevelValueEntity NETLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "NET" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    if (NETLevelValue != null && NETLevelValue.More != null && NETSummary.fVALUE != null)
                    {
                        if (NETSummary.fVALUE.Value >  NETLevelValue.More.Value)
                            NETTEMP = "酸值较高，为" + NETSummary.TEXT + "mgKOH/g，加工过程中应注意设备防腐。";
                    }
                    if (NETLevelValue != null && NETLevelValue.Less != null && NETSummary.fVALUE != null)
                    {
                        if (NETSummary.fVALUE.Value < NETLevelValue.Less.Value)
                        {
                            NETTEMP = "酸值较低，为" + NETSummary.TEXT + "mgKOH/g。";
                        }
                    }
                    summary += NETTEMP;
                    #endregion

                    #region "硫含量"
                    string strSULTEXT = string.Empty;
                    OilTableRowEntity SULRow = rowList.Where(o => o.itemCode == "SUL" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                    if (SULSummary != null && SULSummary.fTEXT < 0.1)
                        strSULTEXT = oilTool.calDataDecLimit((SULSummary.fTEXT * 10000).ToString(), 1, SULRow.valDigital) + "μg/g";
                    else if (SULSummary != null && SULSummary.fTEXT >= 0.1)
                        strSULTEXT = oilTool.calDataDecLimit(SULSummary.TEXT, 2, SULRow.valDigital) + "%";
                    #endregion 

                    #region "原油性质硫含量"
                    LevelValueEntity SULLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "SUL" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    string SULTEMP = "原油的硫含量为" + strSULTEXT + ", 属含硫原油。";
                    if (SULLevelValue != null && SULLevelValue.More != null && SULSummary.fVALUE != null)
                    {
                        if (SULSummary.fVALUE.Value > SULLevelValue.More.Value)
                            SULTEMP = "原油的硫含量较高，为" + strSULTEXT + ", 属高硫原油。";
                    }

                    if (SULLevelValue != null && SULLevelValue.Less != null && SULSummary.fVALUE != null)
                    {
                        if (SULSummary.fVALUE.Value < SULLevelValue.Less.Value)
                            SULTEMP = "原油的硫含量较低，为" + strSULTEXT + ",为低硫原油。";
                    }              
                    summary += SULTEMP;
                    #endregion

                    #region "原油性质CCR"
                    LevelValueEntity CCRLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "CCR" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    string  CCRTEMP = string.Empty  ;
                    if (CCRLevelValue != null && CCRLevelValue.More != null && CCRSummary.fVALUE != null)
                    {
                        if (CCRSummary.fVALUE.Value > CCRLevelValue.More.Value)
                            CCRTEMP = ("残炭值较高，为" + CCRSummary.TEXT + "%,");
                    }
                    summary += CCRTEMP;
                    #endregion

                    #region "原油性质APH、RES" 
                    LevelValueEntity APHLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "APH" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity RESLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "RES" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    int APH_SIGN = 0,RES_SIGN = 0;
                    if (APHLevelValue != null && APHLevelValue.More != null && APHSummary.fVALUE != null)
                    {
                        if (APHSummary.fVALUE.Value > APHLevelValue.More.Value)
                            APH_SIGN = 1;
                    }
                    if (APHLevelValue != null && APHLevelValue.Less != null && APHSummary.fVALUE != null)
                    {
                        if (APHSummary.fVALUE.Value < APHLevelValue.Less.Value)
                            APH_SIGN = -1;
                    }

                    if (RESLevelValue != null && RESLevelValue.More != null && RESSummary.fVALUE != null)
                    {
                        if (RESSummary.fVALUE.Value > RESLevelValue.More.Value)
                            RES_SIGN = 1;
                    }
                    if (RESLevelValue != null && RESLevelValue.Less != null && RESSummary.fVALUE != null)
                    {
                        if (RESSummary.fVALUE.Value < RESLevelValue.Less.Value)
                            RES_SIGN = -1;
                    }
                    string APHRESEMP = string.Empty;
                    if (APH_SIGN == 1 && RES_SIGN == 1)
                        APHRESEMP = "胶质、沥青质含量较高，分别为" + RESSummary.TEXT + "%," + APHSummary.TEXT + "%,";
                    else if (APH_SIGN == 1 && RES_SIGN == 0)
                        APHRESEMP = "沥青质含量较高，为" + APHSummary.TEXT + "%,";
                    else if (APH_SIGN == 1 && RES_SIGN == -1)
                        APHRESEMP = "胶质含量低，沥青质含量较高，为" + APHSummary.TEXT + "%,";
                    else if (APH_SIGN == 0 && RES_SIGN == 1)
                        APHRESEMP = "胶质含量较高，为" + RESSummary.TEXT + "%,";
                    else if (APH_SIGN == -1 && RES_SIGN == -1)
                        APHRESEMP = "胶质、沥青质含量较低，";
                    else if (APH_SIGN == -1 && RES_SIGN == 0)
                        APHRESEMP = "沥青质含量较低，";
                    summary += APHRESEMP;
                    #endregion

                    #region "原油性质NI、V、NIV"
                    LevelValueEntity NILevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "NI" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity VLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "V" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
                    LevelValueEntity NIVLevelValue = this._LevelValueEntityList.Where(o => o.itemCode == "NIV" && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

                    int NI_SIGN = 0, V_SIGN = 0, NIV_SIGN = 0;
                    if (NILevelValue != null && NILevelValue.More != null && NISummary.fVALUE != null)
                    {
                        if (NISummary.fVALUE.Value > NILevelValue.More.Value)
                            NI_SIGN = 1;
                    }
                    if (NILevelValue != null && NILevelValue.Less != null && NISummary.fVALUE != null)
                    {
                        if (NISummary.fVALUE.Value < NILevelValue.Less.Value)
                            NI_SIGN = -1;
                    }

                    if (VLevelValue != null && VLevelValue.More != null && VSummary.fVALUE != null)
                    {
                        if (VSummary.fVALUE.Value > VLevelValue.More.Value)
                            V_SIGN = 1;
                    }
                    if (VLevelValue != null && VLevelValue.Less != null && VSummary.fVALUE != null)
                    {
                        if (VSummary.fVALUE.Value < VLevelValue.Less.Value)
                            V_SIGN = -1;
                    }

                    if (NIVLevelValue != null && NIVLevelValue.More != null && NIVSummary.fVALUE != null)
                    {
                        if (NIVSummary.fVALUE.Value > NIVLevelValue.More.Value)
                            NIV_SIGN = 1;
                    }
                    if (NIVLevelValue != null && NIVLevelValue.Less != null && NIVSummary.fVALUE != null)
                    {
                        if (NIVSummary.fVALUE.Value < NIVLevelValue.Less.Value)
                            NIV_SIGN = -1;
                    }

                    string NIVNIVTEMP = string.Empty;
                    if (NI_SIGN == 1 && V_SIGN == 1)
                        NIVNIVTEMP = "金属镍、钒含量较高，加和为"+ NIVSummary.TEXT + "μg/g，不利于原油的二次加工。";
                    else if (NI_SIGN == 1 && V_SIGN <1  )
                        NIVNIVTEMP = "金属镍含量较高，为" + NISummary.TEXT + "μg/g，不利于原油的二次加工。";
                    else if (NI_SIGN < 1 && V_SIGN == 1 )
                        NIVNIVTEMP = "金属钒含量高，为" + VSummary.TEXT + "μg/g，不利于原油的二次加工。";
                    else if (NI_SIGN < 1 && V_SIGN < 1&& NIV_SIGN == 1)
                        NIVNIVTEMP = "金属镍、钒含量加和较高，为" + VSummary.TEXT + "μg/g，不利于原油的二次加工。";
                    else if (NI_SIGN == 0 && V_SIGN == 0 && NIV_SIGN == 0)
                        NIVNIVTEMP = "金属镍、钒含量加和为"+ NIVSummary.TEXT + "μg/g。";
                    else if (NI_SIGN == 0 && V_SIGN == -1 )
                        NIVNIVTEMP = "金属镍含量为" + NISummary.TEXT + "μg/g,钒含量较低。";
                    else if (NI_SIGN == -1 && V_SIGN == 0)
                        NIVNIVTEMP = "金属钒含量为" + VSummary.TEXT + "μg/g,镍含量较低。";
                    else if (NI_SIGN == 0 && V_SIGN == 0)
                        NIVNIVTEMP = "金属镍含量为" + NISummary.TEXT + "μg/g,金属钒含量为" + VSummary.TEXT + "μg/g。";
                    else if (NI_SIGN == -1 && V_SIGN == -1)
                        NIVNIVTEMP = "金属镍钒含量较低。";

                    summary += NIVNIVTEMP;
                    #endregion

                    #region "原油性质TYP"  
                    summary += "按照原油的硫含量和关键组分分类，该原油属" + getSCLSummaryFromOilInfo().TEXT+getTYPSummaryFromOilInfo().TEXT + "原油。";
                    #endregion

                    #endregion 
                }
             }
            catch (Exception ex)
            {
                Log.Error("原油性质详评错误："+ex.ToString());
            }
        }
        /// <summary>
        /// 获取宽馏分数据
        /// </summary>
        /// <param name="summary">详评信息</param>
        private void getDetailSummaryFromWide(ref string summary)
        { 
            List<OilDataEntity> wideDataList = this._OilA.OilDatas.Where (o=>o.OilTableTypeID == (int)EnumTableType.Wide).Where(o=>o.labData != string.Empty ||o.calData != string.Empty).ToList();
            List<OilDataEntity> wctDataList = wideDataList.Where(o => o.OilTableRow.itemCode == "WCT").ToList();
            List<OilTableRowEntity> rowList = this._OilA.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide && o.isDisplay == true).ToList();
            foreach (OilDataEntity col in wctDataList)
            {
                List<OilDataEntity> dataList = wideDataList.Where(o => o.oilTableColID == col.oilTableColID).ToList();
                string cutName = col.calData +"馏分"+col.OilTableCol.colName ;
                switch (col.calData)
                {                     
                    case "石脑油":
                        getNaphthaDetialSummary(ref summary, dataList, rowList,cutName);
                        break;
                    case "汽油":
                        getDetialSummaryByColData(ref summary, col.calData, enumTargetedValueTableType.Naphtha, dataList, rowList, "汽油QMIN", "汽油QMAX",cutName);
                        break;
                    case "乙烯料":
                        getDetialSummaryByColData(ref summary, col.calData, enumTargetedValueTableType.Naphtha, dataList, rowList, "乙烯料QMIN", "乙烯料QMAX",cutName);
                        break;
                    case "重整料":
                        getDetialSummaryByColData(ref summary, col.calData, enumTargetedValueTableType.Naphtha, dataList, rowList, "重整料QMIN", "重整料QMAX",cutName);
                        break;
                    case "溶剂油":
                        getDetialSummaryByColData(ref summary, col.calData, enumTargetedValueTableType.Naphtha, dataList, rowList, "溶剂油QMIN", "溶剂油QMAX",cutName);
                        break ;
                    case "航煤":
                        getDetialSummaryByColData(ref summary,col.calData, enumTargetedValueTableType.AviationKerosene ,dataList, rowList, "航煤QMIN", "航煤QMAX",cutName);
                        break;
                    case "煤油":
                        getDetialSummaryByColData(ref summary, col.calData, enumTargetedValueTableType.AviationKerosene, dataList, rowList, "煤油QMIN", "煤油QMAX",cutName);
                        break;
                    case "柴油":
                        getDieselOilDetialSummary(ref summary, col.calData, dataList, rowList,cutName);
                        break;
                    case "蜡油": 
                        getWAXDetialSummary(ref summary, col.calData, dataList, rowList,cutName); 
                        break;
                }             
            }
        }
        /// <summary>
        /// 石脑油详评数据
        /// </summary>
        private void getNaphthaDetialSummary(ref string summary, List<OilDataEntity> dataList, List<OilTableRowEntity> rowList,string cutName)
        {         
            int SIGN = 0;
            try
            {
                string strError = string.Empty;//
                SummaryEntity ICPSummary = getTextandValue(ref strError, "ICP", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity ECPSummary = getTextandValue(ref strError, "ECP", dataList, rowList, EnumTableType.Wide, true, cutName);
                SummaryEntity WYSummary = getTextandValue(ref strError, "WY", dataList, rowList, EnumTableType.Wide, true, cutName);
                summary += "\r\n" + ICPSummary.TEXT + "～" + ECPSummary.TEXT + "℃石脑油";

                #region "WY_TEMP"
                string WY_TEMP = "馏分的收率为" + WYSummary.TEXT + "%。";
                //TargetedValueEntity ULPWYQMINValue = getTargetedValue(strQMIN, "WY", tableType);
                //TargetedValueEntity ULPWYQMAXValue = getTargetedValue(strQMAX, "WY", tableType);

                //if (ULPWYQMINValue != null && ULPWYQMINValue.fValue != null && WYSummary.fVALUE != null)
                //{
                //    if (WYSummary.fVALUE.Value < ULPWYQMINValue.fValue.Value)
                //        WY_TEMP = "馏分的收率较低，为" + WYSummary.TEXT + "%。";
                //}
                //if (ULPWYQMAXValue != null && ULPWYQMAXValue.fValue != null && WYSummary.fVALUE != null)
                //{
                //    if (WYSummary.fVALUE.Value > ULPWYQMAXValue.fValue.Value)
                //        WY_TEMP = "，馏分的收率较高，为" + WYSummary.TEXT + "%。";
                //}
                summary += WY_TEMP;
                #endregion
                bool work = false;
                if (!string.IsNullOrWhiteSpace(strError))
                {
                    if (MessageBox.Show(strError + "是否继续？", "提示信息!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        work = true;
                    else
                        work = false;
                }
                else 
                    work =true ;

                if (work)
                {
                    #region "汽油"
                    summary += "\r\n如果生产汽油，";
                    string strULP = string.Empty;
                    Dictionary<string, string> ULPDIC = getDetialSummaryByColData(ref strULP, "汽油", enumTargetedValueTableType.Naphtha, dataList, rowList, "汽油QMIN", "汽油QMAX", cutName);
                    if (ULPDIC.Count > 0)
                    {
                        string ULPGOOD_SUM = ULPDIC["GOOD_SUM"];
                        string ULPBAD_SUM = ULPDIC["BAD_SUM"];

                        if (!string.IsNullOrWhiteSpace(ULPGOOD_SUM) && !string.IsNullOrWhiteSpace(ULPBAD_SUM))
                        {
                            summary += ULPGOOD_SUM.Remove(ULPGOOD_SUM.Length - 1) + "满足指标要求。";
                            summary += ULPBAD_SUM.Remove(ULPBAD_SUM.Length - 1) + "不满足指标要求。";
                            SIGN += 1;
                        }
                        else if (string.IsNullOrWhiteSpace(ULPGOOD_SUM) && !string.IsNullOrWhiteSpace(ULPBAD_SUM))
                        {
                            summary += ULPBAD_SUM.Remove(ULPBAD_SUM.Length - 1) + "不满足指标要求。";
                            SIGN += 1;
                        }
                        else if (!string.IsNullOrWhiteSpace(ULPGOOD_SUM) && string.IsNullOrWhiteSpace(ULPBAD_SUM))
                        {
                            summary += ULPGOOD_SUM.Remove(ULPGOOD_SUM.Length - 1) + "满足指标要求。";
                            SIGN += 1;
                        }
                    }
                    #endregion

                    #region "乙烯料"
                    summary += "\r\n如果生产乙烯料，";
                    string strEthylene = string.Empty;
                    Dictionary<string, string> EthyleneDIC = getDetialSummaryByColData(ref strEthylene, "乙烯料", enumTargetedValueTableType.Naphtha, dataList, rowList, "乙烯料QMIN", "乙烯料QMAX", cutName);
                    if (EthyleneDIC.Count > 0)
                    {
                        string EthyleneGOOD_SUM = EthyleneDIC["GOOD_SUM"];
                        string EthyleneBAD_SUM = EthyleneDIC["BAD_SUM"];

                        if (!string.IsNullOrWhiteSpace(EthyleneGOOD_SUM) && !string.IsNullOrWhiteSpace(EthyleneBAD_SUM))
                        {
                            //summary += TEMP + EthyleneWY_TEMP;
                            summary += EthyleneGOOD_SUM.Remove(EthyleneGOOD_SUM.Length - 1) + "满足指标要求。";
                            summary += EthyleneBAD_SUM.Remove(EthyleneBAD_SUM.Length - 1) + "不满足指标要求。";
                            SIGN += 1;
                        }
                        else if (string.IsNullOrWhiteSpace(EthyleneGOOD_SUM) && !string.IsNullOrWhiteSpace(EthyleneBAD_SUM))
                        {
                            //summary += TEMP + EthyleneWY_TEMP;
                            summary += EthyleneBAD_SUM.Remove(EthyleneBAD_SUM.Length - 1) + "不满足指标要求。";
                            SIGN += 1;
                        }
                        else if (!string.IsNullOrWhiteSpace(EthyleneGOOD_SUM) && string.IsNullOrWhiteSpace(EthyleneBAD_SUM))
                        {
                            //summary += TEMP + EthyleneWY_TEMP;
                            summary += EthyleneGOOD_SUM.Remove(EthyleneGOOD_SUM.Length - 1) + "满足指标要求。";
                            SIGN += 1;
                        }
                    }
                    #endregion

                    #region "重整料"
                    summary += "\r\n如果生产重整料，";
                    string strReformate = string.Empty;
                    Dictionary<string, string> ReformateDIC = getDetialSummaryByColData(ref strReformate, "重整料", enumTargetedValueTableType.Naphtha, dataList, rowList, "重整料QMIN", "重整料QMAX",cutName);
                    if (ReformateDIC.Count > 0)
                    {
                        //string ReformateWY_TEMP = ReformateDIC["WY_TEMP"];
                        string ReformateGOOD_SUM = ReformateDIC["GOOD_SUM"];
                        string ReformateBAD_SUM = ReformateDIC["BAD_SUM"];

                        if (!string.IsNullOrWhiteSpace(ReformateGOOD_SUM) && !string.IsNullOrWhiteSpace(ReformateBAD_SUM))
                        {
                            //summary += TEMP + ReformateWY_TEMP;
                            summary += ReformateGOOD_SUM.Remove(ReformateGOOD_SUM.Length - 1)+ "满足指标要求。";
                            summary += ReformateBAD_SUM.Remove(ReformateBAD_SUM.Length - 1) + "不满足指标要求。";
                            SIGN += 1;
                        }
                        else if (string.IsNullOrWhiteSpace(ReformateGOOD_SUM) && !string.IsNullOrWhiteSpace(ReformateBAD_SUM))
                        {
                            //summary += TEMP + ReformateWY_TEMP;
                            summary += ReformateBAD_SUM.Remove(ReformateBAD_SUM.Length - 1)+ "不满足指标要求。";
                            SIGN += 1;
                        }
                        else if (!string.IsNullOrWhiteSpace(ReformateGOOD_SUM) && string.IsNullOrWhiteSpace(ReformateBAD_SUM))
                        {
                            //summary += TEMP + ReformateWY_TEMP;
                            summary += ReformateGOOD_SUM.Remove(ReformateGOOD_SUM.Length - 1) + "满足指标要求。";
                            SIGN += 1;
                        }
                    }
                    #endregion

                    #region "溶剂油"
                    summary += "\r\n如果生产溶剂油，";
                    string strSolventOil = string.Empty;
                    Dictionary<string, string> SolventOilDIC = getDetialSummaryByColData(ref strSolventOil, "溶剂油", enumTargetedValueTableType.Naphtha, dataList, rowList, "溶剂油QMIN", "溶剂油QMAX", cutName);
                    if (SolventOilDIC.Count > 0)
                    {
                        //string SolventOilWY_TEMP = SolventOilDIC["WY_TEMP"];
                        string SolventOilGOOD_SUM = SolventOilDIC["GOOD_SUM"];
                        string SolventOilBAD_SUM = SolventOilDIC["BAD_SUM"];

                        if (!string.IsNullOrWhiteSpace(SolventOilGOOD_SUM) && !string.IsNullOrWhiteSpace(SolventOilBAD_SUM))
                        {
                            //summary += TEMP + SolventOilWY_TEMP;
                            summary += SolventOilGOOD_SUM.Remove(SolventOilGOOD_SUM.Length - 1) + "满足指标要求。";
                            summary += SolventOilBAD_SUM.Remove(SolventOilBAD_SUM.Length - 1) + "不满足指标要求。";
                            SIGN += 1;
                        }
                        else if (string.IsNullOrWhiteSpace(SolventOilGOOD_SUM) && !string.IsNullOrWhiteSpace(SolventOilBAD_SUM))
                        {
                            //summary += TEMP + SolventOilWY_TEMP;
                            summary += SolventOilBAD_SUM.Remove(SolventOilBAD_SUM.Length - 1) + "不满足指标要求。";
                            SIGN += 1;
                        }
                        else if (!string.IsNullOrWhiteSpace(SolventOilGOOD_SUM) && string.IsNullOrWhiteSpace(SolventOilBAD_SUM))
                        {
                            //summary += TEMP + SolventOilWY_TEMP;
                            summary += SolventOilGOOD_SUM.Remove(SolventOilGOOD_SUM.Length - 1) + "满足指标要求。";
                            SIGN += 1;
                        }
                    }
                    #endregion
                }
               
            }
            catch(Exception ex)
            {
                Log.Error("石脑油详评："+ex.ToString());
            }
            if (SIGN == 0)
                MessageBox.Show("缺少石脑油馏分的详评配置，WCUT宽馏分不能生成结论！", "提示信息!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// 获取指标值
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="itemCode"></param>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        private TargetedValueEntity getTargetedValue(string colName , string itemCode , enumTargetedValueTableType tableType)
        {
            if (colName == string.Empty || itemCode == string.Empty  )
                return null;

            OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.tableName == tableType.GetDescription()).FirstOrDefault();
            TargetedValueColEntity QMINCOL = this._targetedValueColList.Where(o => o.colCode == colName && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
            TargetedValueRowEntity QMINROW = this._targetedValueRowlList.Where(o => o.itemCode == itemCode && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
            TargetedValueEntity ULPQMINValue = null;
            if (oilTableTypeComTableEntity != null && QMINCOL != null && QMINROW != null)
                ULPQMINValue = this._targetedValueList.Where(o => o.TargetedValueColID == QMINCOL.ID && o.TargetedValueRowID == QMINROW.ID && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();

            return ULPQMINValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private List<TargetedValueEntity> getTargetedValueList (string colName, enumTargetedValueTableType tableType)
        {
            List<TargetedValueEntity> ULPQMINValue = new List<TargetedValueEntity>();
            if (colName == string.Empty )
                return ULPQMINValue;

            OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.tableName == tableType.GetDescription()).FirstOrDefault();
            TargetedValueColEntity ULPQMINCOL = this._targetedValueColList.Where(o => o.colCode == colName && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).FirstOrDefault();
           
            if (oilTableTypeComTableEntity != null && ULPQMINCOL != null )
                ULPQMINValue = this._targetedValueList.Where(o => o.TargetedValueColID == ULPQMINCOL.ID && o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).ToList();

            return ULPQMINValue;
        }
        /// <summary>
        /// 返回行集合
        /// </summary>
        /// <param name="tableType">判断是那一个表</param>
        /// <returns></returns>
        private List<TargetedValueRowEntity> getTargetedRowList(enumTargetedValueTableType tableType)
        {
            List<TargetedValueRowEntity> rowList = new List<TargetedValueRowEntity>();
            
            OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = this._oilTableTypeComparisonTableEntityList.Where(o => o.tableName == tableType.GetDescription()).FirstOrDefault();
            
            if (oilTableTypeComTableEntity != null  )
                rowList = this._targetedValueRowlList.Where(o => o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID && o.itemCode != string.Empty).ToList();

            return rowList;
        }
        /// <summary>
        /// 每一列的 详评数据
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        private Dictionary<string, string> getDetialSummaryByColData(ref string summary, string WCT,enumTargetedValueTableType tableType, List<OilDataEntity> colDataList, List<OilTableRowEntity> rowList,string strQMIN,string strQMAX,string cutName)
        {
            Dictionary<string, string> DIC = new Dictionary<string, string>();

            string str = string.Empty;
            SummaryEntity ICPSummary = getTextandValue(ref str, "ICP", colDataList, rowList, EnumTableType.Wide,true ,cutName);
            SummaryEntity ECPSummary = getTextandValue(ref str, "ECP", colDataList, rowList, EnumTableType.Wide, true, cutName);
            SummaryEntity WYSummary = getTextandValue(ref str, "WY", colDataList, rowList, EnumTableType.Wide, true, cutName);
            bool work = false;
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (MessageBox.Show(str + "是否继续？", "提示信息!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    work = true;
                else
                    return DIC;
            }
            else 
                work =true ;

            if (work)
            {
                summary += "\r\n"+ICPSummary.TEXT + "～" + ECPSummary.TEXT + "℃" + WCT;

                #region "WY_TEMP"
                string WY_TEMP = "馏分的收率为" + WYSummary.TEXT + "%。";
                TargetedValueEntity ULPWYQMINValue = getTargetedValue(strQMIN, "WY", tableType);
                TargetedValueEntity ULPWYQMAXValue = getTargetedValue(strQMAX, "WY", tableType);

                if (ULPWYQMINValue != null && ULPWYQMINValue.fValue != null && WYSummary.fVALUE != null)
                {
                    if (WYSummary.fVALUE.Value < ULPWYQMINValue.fValue.Value)
                        WY_TEMP = "馏分的收率较低，为" + WYSummary.TEXT + "%。";
                }
                if (ULPWYQMAXValue != null && ULPWYQMAXValue.fValue != null && WYSummary.fVALUE != null)
                {
                    if (WYSummary.fVALUE.Value > ULPWYQMAXValue.fValue.Value)
                        WY_TEMP = "，馏分的收率较高，为" + WYSummary.TEXT + "%。";
                }
                summary += WY_TEMP;
                #endregion
                string GOOD_SUM = string.Empty, BAD_SUM = string.Empty;

                #region "循环"
                List<TargetedValueRowEntity> targetedRowList = getTargetedRowList(tableType).Where(o => o.itemCode != "WY").ToList();

                foreach (TargetedValueRowEntity row in targetedRowList)
                {
                    TargetedValueEntity QMINValue = getTargetedValue(strQMIN, row.itemCode, tableType);
                    TargetedValueEntity QMAXValue = getTargetedValue(strQMAX, row.itemCode, tableType);
                    if (QMINValue == null && QMAXValue == null)
                        continue;
                    if (row.itemCode != "CC2" && row.itemCode != "CC3")
                    {
                        SummaryEntity tempSummary = getTextandValue(ref str, row.itemCode, colDataList, rowList, EnumTableType.Wide, false, cutName);

                        if (QMINValue != null && QMAXValue != null && QMINValue.fValue != null && QMAXValue.fValue != null && tempSummary.fVALUE != null)
                        {
                            if (QMINValue.fValue.Value <= tempSummary.fVALUE.Value && tempSummary.fVALUE.Value <= QMAXValue.fValue.Value)
                                GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            else
                                BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        }
                        else if (QMAXValue != null && ((QMINValue != null && QMINValue.fValue == null) || QMINValue == null) && QMAXValue.fValue != null && tempSummary.fVALUE != null)
                        {//不存在最小值
                            if (tempSummary.fVALUE.Value <= QMAXValue.fValue.Value)
                                GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            else
                                BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        }
                        else if (QMINValue != null && QMINValue.fValue != null && ((QMAXValue != null && QMAXValue.fValue == null) || QMAXValue == null) && tempSummary.fVALUE != null)
                        {//不存在最大值
                            if (tempSummary.fVALUE.Value >= QMINValue.fValue.Value)
                                GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            else
                                BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        }
                    }
                    else
                    {
                        SummaryEntity tempSummary = getTextandValue(ref str, row.itemCode, colDataList, rowList, EnumTableType.Wide, true , cutName);
                        if (string.IsNullOrWhiteSpace(tempSummary.TEXT))
                            continue;

                        string strTemp = tempSummary.TEXT.ToCharArray()[0].ToString(); 
                        float CC2CC3 =0 ;
                        if (float.TryParse(strTemp, out CC2CC3))
                        {
                            if (QMINValue != null && QMAXValue != null && QMINValue.fValue != null && QMAXValue.fValue != null)
                            {
                                if (QMINValue.fValue.Value <  CC2CC3 && CC2CC3 <  QMAXValue.fValue.Value)
                                    GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                                else
                                    BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            }
                            else if (QMAXValue != null && ((QMINValue != null && QMINValue.fValue == null) || QMINValue == null) && QMAXValue.fValue != null)
                            {//不存在最小值
                                if (CC2CC3 <QMAXValue.fValue.Value)
                                    GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                                else
                                    BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            }
                            else if (QMINValue != null && QMINValue.fValue != null && ((QMAXValue != null && QMAXValue.fValue == null) || QMAXValue == null))
                            {//不存在最大值
                                if (CC2CC3 > QMINValue.fValue.Value)
                                    GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                                else
                                    BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            }
                        }
                        else
                        {
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        }                                                                
                    }
                }
                #region 
                summary += "\r\n";
                if (!string.IsNullOrWhiteSpace(GOOD_SUM) && !string.IsNullOrWhiteSpace(BAD_SUM))
                {
                    summary += GOOD_SUM.Remove(GOOD_SUM.Length - 1) + "满足指标要求。";
                    summary += BAD_SUM.Remove(BAD_SUM.Length - 1) + "不满足指标要求。";
                }
                else if (string.IsNullOrWhiteSpace(GOOD_SUM) && !string.IsNullOrWhiteSpace(BAD_SUM))
                    summary += BAD_SUM.Remove(BAD_SUM.Length - 1) + "不满足指标要求。";
                if (!string.IsNullOrWhiteSpace(GOOD_SUM) && string.IsNullOrWhiteSpace(BAD_SUM))
                    summary += GOOD_SUM.Remove(GOOD_SUM.Length - 1) + "满足指标要求。";
                #endregion 

                DIC.Add("WY_TEMP", WY_TEMP);
                DIC.Add("GOOD_SUM", GOOD_SUM);
                DIC.Add("BAD_SUM", BAD_SUM);
                #endregion
            }
            return DIC;
        }     

        #region "柴油详评数据"
        /// <summary>
        /// 柴油详评数据
        /// </summary>
        private void getDieselOilDetialSummary(ref string summary, string WCT, List<OilDataEntity> colDataList, List<OilTableRowEntity> rowList, string cutName)
        { 
            Dictionary<string, string> DIC = new Dictionary<string, string>();
 
            string str = string.Empty;
            SummaryEntity ICPSummary = getTextandValue(ref str, "ICP", colDataList, rowList, EnumTableType.Wide, true, cutName);
            SummaryEntity ECPSummary = getTextandValue(ref str, "ECP", colDataList, rowList, EnumTableType.Wide, true, cutName);
            SummaryEntity WYSummary = getTextandValue(ref str, "WY", colDataList, rowList, EnumTableType.Wide, true, cutName);
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (MessageBox.Show(str + "是否继续？", "提示信息!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    return; //转到宽馏分
                else
                    return ;
            }
            else
            {
                string temp = "\r\n"+ICPSummary.TEXT + "～" + ECPSummary.TEXT + "℃" + WCT;
                summary += temp;
                #region "WY_TEMP"
                string WY_TEMP = "馏分的收率为" + WYSummary.TEXT + "%。";
                //TargetedValueEntity DieselOilWYQMINValue = getTargetedValue(strQMIN, "WY", enumTargetedValueTableType.DieselOil);
                //TargetedValueEntity DieselOilWYQMAXValue = getTargetedValue(strQMAX, "WY", enumTargetedValueTableType.DieselOil);

                //if (DieselOilWYQMINValue != null && DieselOilWYQMINValue.fValue != null && WYSummary.fVALUE != null)
                //{
                //    if (WYSummary.fVALUE.Value < DieselOilWYQMINValue.fValue.Value)
                //        WY_TEMP = "馏分的收率较低，为" + WYSummary.TEXT + "%。";
                //}
                //if (DieselOilWYQMAXValue != null && DieselOilWYQMAXValue.fValue != null && WYSummary.fVALUE != null)
                //{
                //    if (WYSummary.fVALUE.Value > DieselOilWYQMAXValue.fValue.Value)
                //        WY_TEMP = "，馏分的收率较高，为" + WYSummary.TEXT + "%。";
                //}
                summary += WY_TEMP;
                #endregion

                SummaryEntity SOPSummary = getTextandValue(ref str, "SOP", colDataList, rowList, EnumTableType.Wide, true, cutName);
                if (SOPSummary == null || SOPSummary.TEXT == string.Empty)
                {
                    if (DialogResult.OK == MessageBox.Show(temp + "凝点校正值空缺，不能生成相应结论!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning))
                    {
                        return;
                    }
                }

                SummaryEntity CFPSummary = getTextandValue(ref str, "CFP", colDataList, rowList, EnumTableType.Wide, true, cutName);
                List<TargetedValueColEntity> colQMINList = this._targetedValueColList.Where(o => o.OilTableTypeComparisonTableID ==(int) enumTargetedValueTableType.DieselOil && o.colCode.Contains ("QMIN")).ToList();
                
                #region "柴油第一次循环"
                List<int> colList = new List<int>() { -50,-35,-20, -10,0,5,10};
                string TEMP = string.Empty;
                string breakNum = "-50";//跳出的油号
                int SIGN = 0;
                bool isBorken = true  ;
                foreach (int colMIN in colList)
                {
                    #region "colMIN"
                    SIGN = 0;
                    string strTempQMAX = colMIN.ToString() + "号QMAX";
                    TargetedValueEntity SOPQMAXValue = getTargetedValue(strTempQMAX, "SOP", enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity CFPQMAXValue = getTargetedValue(strTempQMAX, "CFP", enumTargetedValueTableType.DieselOil);

                    if (SOPQMAXValue != null && SOPQMAXValue.fValue != null)
                    {
                        if (SOPSummary != null && SOPSummary.fVALUE != null && SOPSummary.fVALUE.Value <= SOPQMAXValue.fValue.Value)
                            SIGN++;
                    }
                    else
                        SIGN++;


                    if (CFPQMAXValue != null && CFPQMAXValue.fValue != null)
                    {
                        if (CFPSummary != null && CFPSummary.fVALUE != null && CFPSummary.fVALUE.Value <= CFPQMAXValue.fValue.Value)
                            SIGN+=2;
                        else if (CFPSummary == null || CFPSummary.TEXT == string.Empty)
                            SIGN += 2;
                    }
                    else
                        SIGN += 2;

                    breakNum = colMIN.ToString();
                    if (SIGN == 3)
                    {
                        if (CFPSummary == null || CFPSummary.TEXT == string.Empty)
                            summary += "\r\n凝点"+SOPSummary.TEXT+"℃，可以用来生产" + colMIN.ToString() + "号柴油。";
                        else if (CFPSummary != null &&  CFPSummary.TEXT != string.Empty)
                            summary += "\r\n凝点" + SOPSummary.TEXT + "℃，冷滤点" + CFPSummary.TEXT + "℃，可以用来生产" + colMIN.ToString() + "号柴油。";
                        breakNum = colMIN.ToString();
                        isBorken = false ;
                        break;
                    }
                    #endregion 
                }

                if (isBorken)
                {
                    if (SIGN == 0)
                        summary += "凝点偏高，不能用来直接生产"+breakNum+"号柴油。";
                    else if (SIGN == 1)
                        summary += "凝点偏高，不能用来直接生产" + breakNum + "号柴油。";
                    else if (SIGN == 2)
                        summary += "冷滤点偏高，不能用来直接生产" + breakNum + "号柴油。";
                    return;
                }
                string strQMIN = breakNum + "号QMIN";
                string strQMAX = breakNum + "号QMAX";
                
                #endregion 

                #region "柴油第二次循环"
                List<TargetedValueRowEntity> targetedRowList = getTargetedRowList(enumTargetedValueTableType.DieselOil).Where(o => o.itemCode != "WY" && o.itemCode != "CFP" && o.itemCode != "SOP").ToList();
                string GOOD_SUM = string.Empty, BAD_SUM = string.Empty;
                                    
                #region "GOOD_SUMBAD_SUM循环"                    
                foreach (TargetedValueRowEntity row in targetedRowList)
                {
                    TargetedValueEntity QMINValue = getTargetedValue(strQMIN, row.itemCode, enumTargetedValueTableType.DieselOil);
                    TargetedValueEntity QMAXValue = getTargetedValue(strQMAX, row.itemCode, enumTargetedValueTableType.DieselOil);
                    if (QMINValue == null && QMAXValue == null)
                        continue;

                    SummaryEntity tempSummary = getTextandValue(ref str, row.itemCode, colDataList, rowList, EnumTableType.Wide, true, cutName);
                    if (row.itemCode != "CC2" && row.itemCode != "CC3")
                    {
                        if (QMINValue != null && QMAXValue != null && QMINValue.fValue != null && QMAXValue.fValue != null && tempSummary.fVALUE != null)
                        {
                            if (QMINValue.fValue.Value <= tempSummary.fVALUE.Value && tempSummary.fVALUE.Value <= QMAXValue.fValue.Value)
                                GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            else
                                BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        }
                        else if (QMAXValue != null && ((QMINValue != null && QMINValue.fValue == null) || QMINValue == null) && QMAXValue.fValue != null && tempSummary.fVALUE != null)
                        {
                            if (tempSummary.fVALUE.Value <= QMAXValue.fValue.Value)
                                GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            else
                                BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        }
                        else if (QMINValue != null && QMINValue.fValue != null && ((QMAXValue != null && QMAXValue.fValue == null) || QMAXValue == null) && tempSummary.fVALUE != null)
                        {
                            if (tempSummary.fVALUE.Value >= QMINValue.fValue.Value)
                                GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            else
                                BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        }
                    }
                    else
                    {
                        //SummaryEntity tempSummary = getTextandValue(ref str, row.itemCode, colDataList, rowList, EnumTableType.Wide);
                        if (string.IsNullOrWhiteSpace(tempSummary.TEXT))
                            continue;

                        string strTemp = tempSummary.TEXT.ToCharArray()[0].ToString();
                        float CC2CC3 = 0;
                        if (float.TryParse(strTemp, out CC2CC3))
                        {
                            if (QMINValue != null && QMAXValue != null && QMINValue.fValue != null && QMAXValue.fValue != null)
                            {
                                if (QMINValue.fValue.Value < CC2CC3 && CC2CC3 < QMAXValue.fValue.Value)
                                    GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                                else
                                    BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            }
                            else if (QMAXValue != null && ((QMINValue != null && QMINValue.fValue == null) || QMINValue == null) && QMAXValue.fValue != null)
                            {//不存在最小值
                                if (CC2CC3 < QMAXValue.fValue.Value)
                                    GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                                else
                                    BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            }
                            else if (QMINValue != null && QMINValue.fValue != null && ((QMAXValue != null && QMAXValue.fValue == null) || QMAXValue == null))
                            {//不存在最大值
                                if (CC2CC3 > QMINValue.fValue.Value)
                                    GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                                else
                                    BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                            }
                        }
                        else
                        {
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        }
                    }
                }                  
                #endregion                   
                
                if (!string.IsNullOrWhiteSpace(GOOD_SUM) && !string.IsNullOrWhiteSpace(BAD_SUM))
                {
                    summary += GOOD_SUM.Remove(GOOD_SUM.Length - 1) + "满足指标要求。";
                    summary += BAD_SUM.Remove(BAD_SUM.Length - 1) + "不满足指标要求。";
                }
                else if (string.IsNullOrWhiteSpace(GOOD_SUM) && !string.IsNullOrWhiteSpace(BAD_SUM))
                    summary += BAD_SUM.Remove(BAD_SUM.Length - 1) + "不满足指标要求。";
                if (!string.IsNullOrWhiteSpace(GOOD_SUM) && string.IsNullOrWhiteSpace(BAD_SUM))
                    summary += GOOD_SUM.Remove(GOOD_SUM.Length - 1) + "满足指标要求。";
                #endregion 
            }
        }
        #endregion 

        #region "蜡油详评数据"
        /// <summary>
        /// 航煤详评数据
        /// </summary>
        private void getWAXDetialSummary(ref string summary, string WCT, List<OilDataEntity> colDataList, List<OilTableRowEntity> rowList,string cutName)
        {
            string strERROE = string.Empty;

            SummaryEntity ICPSummary = getTextandValue(ref strERROE, "ICP", colDataList, rowList, EnumTableType.Wide, true, cutName);
            SummaryEntity ECPSummary = getTextandValue(ref strERROE, "ECP", colDataList, rowList, EnumTableType.Wide, true, cutName);
            SummaryEntity WYSummary = getTextandValue(ref strERROE, "WY", colDataList, rowList, EnumTableType.Wide, true, cutName);
            if (!string.IsNullOrWhiteSpace(strERROE))
            {
                if (MessageBox.Show(strERROE + "是否继续？", "提示信息!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    //转到宽馏分
                    return;
                }
                else
                    return  ;
            }
            else
            {
                summary += "\r\n"+ICPSummary.TEXT + "～" + ECPSummary.TEXT + "℃" + WCT;

                #region "WY_TEMP"
                string WY_TEMP = "馏分的收率为" + WYSummary.TEXT + "%。";
                TargetedValueEntity WYQMINValue = getTargetedValue("催化裂化原料QMIN", "WY", enumTargetedValueTableType.MassSpectrometry);
                TargetedValueEntity WYQMAXValue = getTargetedValue("催化裂化原料QMAX", "WY", enumTargetedValueTableType.MassSpectrometry);

                if (WYQMINValue != null && WYQMINValue.fValue != null && WYSummary.fVALUE != null)
                {
                    if (WYSummary.fVALUE.Value < WYQMINValue.fValue.Value)
                        WY_TEMP = "馏分的收率较低，为" + WYSummary.TEXT + "%。";
                }
                if (WYQMAXValue != null && WYQMAXValue.fValue != null && WYSummary.fVALUE != null)
                {
                    if (WYSummary.fVALUE.Value > WYQMAXValue.fValue.Value)
                        WY_TEMP = "，馏分的收率较高，为" + WYSummary.TEXT + "%。";
                }
                summary += WY_TEMP;
                #endregion
                string GOOD_SUM = string.Empty, BAD_SUM = string.Empty;

                #region "循环"
                List<TargetedValueRowEntity> targetedRowList = getTargetedRowList(enumTargetedValueTableType.MassSpectrometry).Where(o => o.itemCode != "WY").ToList();

                foreach (TargetedValueRowEntity row in targetedRowList)
                {
                    TargetedValueEntity QMINValue = getTargetedValue("催化裂化原料QMIN", row.itemCode, enumTargetedValueTableType.MassSpectrometry);
                    TargetedValueEntity QMAXValue = getTargetedValue("催化裂化原料QMAX", row.itemCode, enumTargetedValueTableType.MassSpectrometry);
                    if (QMINValue == null && QMAXValue == null)
                        continue;

                    SummaryEntity tempSummary = getTextandValue(ref strERROE, row.itemCode, colDataList, rowList, EnumTableType.Wide, true, cutName);

                    if (QMINValue != null && QMAXValue != null && QMINValue.fValue != null && QMAXValue.fValue != null && tempSummary.fVALUE != null)
                    {
                        if (QMINValue.fValue.Value <= tempSummary.fVALUE.Value && tempSummary.fVALUE.Value <= QMAXValue.fValue.Value)
                            GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        else if (QMINValue.fValue.Value > tempSummary.fVALUE.Value)
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")较低、";
                        else if (QMAXValue.fValue.Value > tempSummary.fVALUE.Value)
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")较高、";
                    }
                    else if (QMAXValue != null && ((QMINValue != null && QMINValue.fValue == null) || QMINValue == null) && QMAXValue.fValue != null && tempSummary.fVALUE != null)
                    {
                        if (tempSummary.fVALUE.Value <= QMAXValue.fValue.Value)
                            GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        else
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")较高、";
                    }
                    else if (QMINValue != null && QMINValue.fValue != null && ((QMAXValue != null && QMAXValue.fValue == null) || QMAXValue == null) && tempSummary.fVALUE != null)
                    {
                        if (tempSummary.fVALUE.Value >= QMINValue.fValue.Value)
                            GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        else
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")较低、";
                    }
                }

                summary += "\r\n";
                if (!string.IsNullOrWhiteSpace(GOOD_SUM) && !string.IsNullOrWhiteSpace(BAD_SUM))
                {
                    summary += "如果作为催化裂化原料，则" + GOOD_SUM.Remove(GOOD_SUM.Length - 1) + "满足指标要求。";
                    summary += BAD_SUM.Remove(BAD_SUM.Length - 1) + "，不适合作催化裂化原料。";
                }
                else if (string.IsNullOrWhiteSpace(GOOD_SUM) && !string.IsNullOrWhiteSpace(BAD_SUM))
                {
                    summary += "如果作为催化裂化原料，则" + BAD_SUM.Remove(BAD_SUM.Length - 1) + "，不适合作催化裂化原料。";
                }
                if (!string.IsNullOrWhiteSpace(GOOD_SUM) && string.IsNullOrWhiteSpace(BAD_SUM))
                {
                    summary += "如果作为催化裂化原料，则" + GOOD_SUM.Remove(GOOD_SUM.Length - 1) + "满足指标要求。";
                }
               
                #endregion
            }          
        }
        #endregion 

        #region "渣油详评数据"
        /// <summary>
        /// 渣油详评数据
        /// </summary>
        private void getDetialSummaryFromResidue(ref string summary)
        {
            string WCT = "渣油";

            List<OilDataEntity> residueDataList = this._OilA.OilDatas.Where (o=>o.OilTableTypeID == (int)EnumTableType.Residue).Where(o=>o.calData != string.Empty).ToList();
            List<OilTableColEntity> colList = this._OilA.OilTableCols.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();
            List<OilTableRowEntity> rowList = this._OilA.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue && o.isDisplay == true).ToList();
            foreach (OilTableColEntity col in colList)
            {
                List<OilDataEntity> dataList = residueDataList.Where(o => o.oilTableColID == col.ID).ToList();
                if (dataList.Count == 0)
                    continue;
                string str = string.Empty;
                string cutName = "馏分"+ col.colName;
                SummaryEntity ICPSummary = getTextandValue(ref str, "ICP", dataList, rowList, EnumTableType.Residue,true , cutName);
                SummaryEntity WYSummary = getTextandValue(ref str, "WY", dataList, rowList, EnumTableType.Residue,true, cutName);
                string ERROR = ICPSummary.ERROE + WYSummary.ERROE;
                bool work = false;
                if (!string.IsNullOrWhiteSpace(ERROR))
                {
                    if (MessageBox.Show(ERROR + "是否继续？", "提示信息！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        work = true;
                    else
                        return;
                }
                else
                    work = true;
                if (work)
                {                                    
                    try
                    {
                        summary += "\r\n>" + ICPSummary.TEXT + "℃" + WCT;
                        string WY_TEMP = "收率为" + WYSummary.TEXT + "%。";

                        #region "WY_TEMP"
                        //TargetedValueEntity WYQMINValue = getTargetedValue(strQMIN, "WY", enumTargetedValueTableType.Residue);
                        //TargetedValueEntity WYQMAXValue = getTargetedValue(strQMAX, "WY", enumTargetedValueTableType.Residue);

                        //if (WYQMINValue != null && WYQMINValue.fValue != null && WYSummary.fVALUE != null)
                        //{
                        //    if (WYSummary.fVALUE.Value < WYQMINValue.fValue.Value)
                        //        WY_TEMP = "馏分的收率较低，为" + WYSummary.TEXT + "%。";
                        //}
                        //if (WYQMAXValue != null && WYQMAXValue.fValue != null && WYSummary.fVALUE != null)
                        //{
                        //    if (WYSummary.fVALUE.Value > WYQMAXValue.fValue.Value)
                        //        WY_TEMP = "馏分的收率较高，为" + WYSummary.TEXT + "%。";
                        //}
                        #endregion
                        summary += WY_TEMP;
                        summary += getResidueDetialSummaryFromColDataList("催化裂化原料", dataList, rowList, "催化裂化原料QMIN", "催化裂化原料QMAX", cutName);
                        summary += getResidueDetialSummaryFromColDataList("加氢裂化原料", dataList, rowList, "加氢裂化原料QMIN", "加氢裂化原料QMAX", cutName);
                        summary += getResidueDetialSummaryFromColDataList("延迟焦化原料", dataList, rowList, "延迟焦化原料QMIN", "延迟焦化原料QMAX", cutName);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("详细文件:"+ex.ToString());
                    }
                }  
            }              
        }

        /// <summary>
        /// 从每一列中获取渣油详评数据
        /// </summary>
        /// <param name="dataList">渣油录入表中的一列数据</param>
        /// <param name="rowList"></param>
        /// <param name="strQMIN">指标值表的列名称</param>
        /// <param name="strQMAX">指标值表的列名称</param>
        /// <returns></returns>
        private string  getResidueDetialSummaryFromColDataList(string strFun, List<OilDataEntity> dataList, List<OilTableRowEntity> rowList, string strQMIN, string strQMAX,string cutName)
        {
            string summary = string.Empty;//返回的字符串集合
            string str = string.Empty;   

            //SummaryEntity ICPSummary = getTextandValue(ref str, "ICP", dataList, rowList, EnumTableType.Residue);
            //SummaryEntity WYSummary = getTextandValue(ref str, "WY", dataList, rowList, EnumTableType.Residue);
                        
            summary += "\r\n如用作" + strFun+"，";
            //string WY_TEMP = "馏分的收率为" + WYSummary.TEXT + "%。";
            
            #region "WY_TEMP" 
            //TargetedValueEntity WYQMINValue = getTargetedValue(strQMIN, "WY", enumTargetedValueTableType.Residue);
            //TargetedValueEntity WYQMAXValue = getTargetedValue(strQMAX, "WY", enumTargetedValueTableType.Residue);

            //if (WYQMINValue != null && WYQMINValue.fValue != null && WYSummary.fVALUE != null)
            //{
            //    if (WYSummary.fVALUE.Value < WYQMINValue.fValue.Value)
            //        WY_TEMP = "馏分的收率较低，为" + WYSummary.TEXT + "%。";
            //}
            //if (WYQMAXValue != null && WYQMAXValue.fValue != null && WYSummary.fVALUE != null)
            //{
            //    if (WYSummary.fVALUE.Value > WYQMAXValue.fValue.Value)
            //        WY_TEMP = "馏分的收率较高，为" + WYSummary.TEXT + "%。";
            //}
            #endregion
            //summary += WY_TEMP;
            string GOOD_SUM = string.Empty, BAD_SUM = string.Empty;

            #region "循环"

            List<TargetedValueRowEntity> targetedRowList = getTargetedRowList(enumTargetedValueTableType.MassSpectrometry).Where(o => o.itemCode != "WY").ToList();

            List<TargetedValueEntity> QMINValueList = getTargetedValueList(strQMIN, enumTargetedValueTableType.MassSpectrometry).Where(o => o.TargetedValueRow.itemCode != "WY").ToList();
            List<TargetedValueEntity> QMAXValueList = getTargetedValueList(strQMAX, enumTargetedValueTableType.MassSpectrometry).Where(o => o.TargetedValueRow.itemCode != "WY").ToList();
            foreach (TargetedValueRowEntity row in targetedRowList)
            {
                TargetedValueEntity QMINValue = getTargetedValue(strQMIN, row.itemCode, enumTargetedValueTableType.MassSpectrometry);
                TargetedValueEntity QMAXValue = getTargetedValue(strQMAX, row.itemCode, enumTargetedValueTableType.MassSpectrometry);
                SummaryEntity tempSummary = getTextandValue(ref str, row.itemCode, dataList, rowList, EnumTableType.Residue, true, cutName);
                if (!string.IsNullOrWhiteSpace(tempSummary.TEXT))
                {
                    if (QMINValue == null && QMAXValue == null)
                        continue;
                    //summary += row.itemName + "为" + tempSummary.TEXT + row.unit + ",";

                    if (QMINValue != null && QMAXValue != null && QMINValue.fValue != null && QMAXValue.fValue != null && tempSummary.fVALUE != null)
                    {
                        if (QMINValue.fValue.Value <= tempSummary.fVALUE.Value && tempSummary.fVALUE.Value <= QMAXValue.fValue.Value)
                            GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        else if (QMINValue.fValue.Value > tempSummary.fVALUE.Value)
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")较低、";
                        else if (QMAXValue.fValue.Value > tempSummary.fVALUE.Value)
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")较高、";
                    }
                    else if (QMAXValue != null && ((QMINValue != null && QMINValue.fValue == null) || QMINValue == null) && QMAXValue.fValue != null && tempSummary.fVALUE != null)
                    {
                        if (tempSummary.fVALUE.Value <= QMAXValue.fValue.Value)
                            GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        else
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")较高、";
                    }
                    else if (QMINValue != null && QMINValue.fValue != null && ((QMAXValue != null && QMAXValue.fValue == null) || QMAXValue == null) && tempSummary.fVALUE != null)
                    {
                        if (tempSummary.fVALUE.Value >= QMINValue.fValue.Value)
                            GOOD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")、";
                        else
                            BAD_SUM += row.itemName + "(" + tempSummary.TEXT + row.unit + ")较低、";
                    }
                }
            }           
            #endregion 

            if (!string.IsNullOrWhiteSpace(GOOD_SUM) && !string.IsNullOrWhiteSpace(BAD_SUM))
            {
                summary += GOOD_SUM.Remove(GOOD_SUM.Length - 1) + "满足指标要求。";
                summary += BAD_SUM.Remove(BAD_SUM.Length - 1) + "，不适合作"+strFun+"。";
            }
            else if (string.IsNullOrWhiteSpace(GOOD_SUM) && !string.IsNullOrWhiteSpace(BAD_SUM))
                summary += BAD_SUM.Remove(BAD_SUM.Length - 1) + "，不适合作" + strFun + "。";
            if (!string.IsNullOrWhiteSpace(GOOD_SUM) && string.IsNullOrWhiteSpace(BAD_SUM))
                summary += GOOD_SUM.Remove(GOOD_SUM.Length - 1) + "满足指标要求。";
            return summary;
        }

        #endregion 
        #endregion
    }
}
 