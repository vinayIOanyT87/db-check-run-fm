using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class TankApi54aTable : TankBaseVcf
    {
        protected double m_dTable54ReferenceTemperature;

        public TankApi54aTable()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_TABLE;
            m_bUsesDensity = false;
            m_dTable54ReferenceTemperature = TAB54_DEF_REF_TEMP;
            m_bStandardCalculationType = EApiCalc.API_CALC_JAPANEESE;
        }

        public override bool TemperatureCorr(double dDensity,
                                                        double dMeasTemp,
                                                        double dStdTempInC,
                                                        double dStdTemp,
                                                        EngineeringUnit bDensityUnits,
                                                        EngineeringUnit bTempUnits,
                                                        double dDensityPress,			// density pressure for api 2004
                                                        EngineeringUnit bDensityPressUnits,	// density pressure units for api 2004
                                                        double dAlternateTemperature,	// selected refined product sub catagory for api 2004
                                                        double dBaseTemp,	// api 2004 alternate base temp reference
                                                        double dAlternateBasePress,	// api 2004 alternate base pressure reference
                                                        ref double[] dK,
                                                        ref double pdVcfc,
                                                        ref int Iflag,
												ref double CTLReturn,
												ref double CPLReturn,
														bool RangeCk,
                                                        bool bRound,
                                                        bool bTable60,				//	Optional
                                                        bool UseDensity)				//	Optional
        {
            double dKgPerM3 = 0;
            double dDegC = 0;
            double dVcfcHi = 0.0;
            double dVcfcLo = 0.0;
            bool lpbBetweenTable = false;
            bool lpbExactMidpoint = false;
            bool bMidTableVcf = false;
            double dHiStdDensity = 0.00, dLoStdDensity = 0.00, dDiffInTableVcf = 0.00;

            //FMTRACE(_T("CTankApi54a::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dKgPerM3, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 0))
                return false;

            //Check Middle of table for standard density 
            CheckStdDenAgstTable(dKgPerM3, ref dHiStdDensity, ref dLoStdDensity, ref lpbBetweenTable, ref lpbExactMidpoint);

            // if StdDensity is between table values, find Hi and Low of table value.  
            if (lpbBetweenTable == true)
            {
                //Find High Vcf in the table 
                APICorrection(dHiStdDensity, dDegC, ref pdVcfc, ref Iflag, true, bRound, m_dTable54ReferenceTemperature);
                dVcfcHi = pdVcfc;
                if (bRound)
                {
                    RoundDouble(dVcfcHi, ref dVcfcHi, 4, false, false);
                }
                //Find Low Vcf in the table  
                APICorrection(dLoStdDensity, dDegC, ref pdVcfc, ref Iflag, true, bRound, m_dTable54ReferenceTemperature);
                dVcfcLo = pdVcfc;
                if (bRound)
                {
                    RoundDouble(dVcfcLo, ref dVcfcLo, 4, false, false);
                }

                //Calculate Vcf using 
                pdVcfc = dVcfcLo - (dKgPerM3 - dLoStdDensity) * (dVcfcLo - dVcfcHi) / (dHiStdDensity - dLoStdDensity);
                dDiffInTableVcf = dVcfcHi - dVcfcLo;

                //Check distance between calculated vcf and VcfLo
                int iDis = (int)((pdVcfc - dVcfcLo) * 100000);
                //Find mid point of VcfHi and VcfLo 
                int iMid = (int)(dDiffInTableVcf * 100000 / 2);
                //Check Middle of table for vcfs 
                if ((iMid == iDis) && (dDiffInTableVcf != 0.0))
                {
                    bMidTableVcf = true;
                }

                if (bRound)
                {
                    //Only both standard density and vcf are middle of table  
                    if (lpbExactMidpoint && bMidTableVcf)
                    {
                        RoundDouble(pdVcfc, ref pdVcfc, 5, false, false);
                        double dValue = pdVcfc * 100000.0;
                        int iValue = 0;
                        iValue = (int)dValue;
                        if (iValue % 5 != 0)
                        {
                            RoundDouble(pdVcfc, ref pdVcfc, 4, false, false);
                        }
                        else
                        {
                            iValue /= 10;
                            if (iValue % 2 != 0)
                            {
                                RoundDouble(pdVcfc, ref pdVcfc, 4, true, false);
                                pdVcfc += 0.0001;
                            }
                            else
                                RoundDouble(pdVcfc, ref pdVcfc, 4, true, false);
                        }
                    }
                    else
                        RoundDouble(pdVcfc, ref pdVcfc, 4, false, false);
                }
            }
            else
            {
                APICorrection(dKgPerM3, dDegC, ref pdVcfc, ref Iflag, true, bRound, m_dTable54ReferenceTemperature);
            }

            return true;
        }

        public void APICorrection(double dDen15,		// input: std density in Kg/M3 at 15 C
                                                            double dDegC,		// input: observed temp in C
                                                            ref double pdVcfc,		// output: volume corec factor for calculations
                                                            ref int piFlag,		// output: 0=>normal,1=>extrap region, -1=>out of range
                                                            bool RangeCk,		// input: if true check to make sure Den15 at DegC are within Range
                                                            bool bRound,		// input: if = 1 then function should perform rounding on input vars
                                                            double dRefDegC)	// input: reference temperature , normally 15
        {
            const double dBas = 15.00f,
                                            dBp1 = 778.5f,
                                            dBp2 = 824.0f,
                                            dNbp1 = 1075.0f,
                                            dTmp1 = 95.00f,
                                            dTmp2 = 125.00f,
                                            dTmp3 = 150.00f,
                                            dEp1 = 60.00f,
                                            dEp2 = 90.00f,
                                            dEp3 = 120.00f,
                                            dEden = 758.0f,
                                            dFden = 610.5f,
                                            dBt1 = -18.00f,
                                            dK0 = 613.9723f,
                                            dK1 = 0.0f;

            double dDt = 0.00, dAlpha = 0.00;

            int BelowApiTemp = 0;

            pdVcfc = -1.0;
            piFlag = -1;
            BelowApiTemp = 0;


            //Round Input parameters
            if (bRound)
            {
                RoundDouble(dDen15, ref dDen15, 1, false, true);
                TemperatureRound_Japan(ETempRounding.TEMP_ROUNDING_025, dDegC, ref dDegC);
            }

            /*
           * Calculate delta temp
           */
            dDt = dDegC - dBas;
            if (bRound)
            {
                RoundDouble(dDt, ref dDt, 2, false, false);
            }

            /*
            *  Check the input den and Temp ranges
            */
            if (RangeCk)
            {
                if (dDen15 < dFden)
                    return;
                else if (dDen15 <= dNbp1)
                {
                }
                else
                    return;
                if (dDegC < dBt1)
                {
                    if (dDegC < LOWEST_CTEMP) return;
                    else BelowApiTemp = 1;
                }
                else if (dDen15 <= dBp1)
                    if (dDegC <= dTmp1)
                    { }
                    else
                        return;
                else if (dDen15 <= dBp2)
                    if (dDegC <= dTmp2)
                    { }
                    else
                        return;
                else if (dDegC <= dTmp3)
                { }
                else
                    return;
            }

            /*
           * calc alpha
           */
            ALPHA(dDen15, dK0, dK1, ref dAlpha);
            RoundDouble(dAlpha, ref dAlpha, 7, false, false);

            /*
            * calc VCF
            */
            VCF(dAlpha, dDt, ref pdVcfc, dRefDegC, false);
            piFlag = 0;

            /*
            * Check if in extrapolated region
            */
            if (BelowApiTemp != 0)
                piFlag = -1;
            else if (dDen15 < dEden)
                piFlag = 1;
            else if (dDen15 >= dBp1)
                if (dDegC <= dEp1)
                { }
                else
                    piFlag = 1;
            else if (dDen15 >= dBp2)
                if (dDegC <= dEp2)
                { }
                else
                    piFlag = 1;
            else if (dDegC <= dEp3)
            { }
            else
                piFlag = 1;
        }

    }
}
