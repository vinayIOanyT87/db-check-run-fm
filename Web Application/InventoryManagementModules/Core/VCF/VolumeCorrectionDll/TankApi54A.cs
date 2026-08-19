using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class TankApi54a : TankBaseVcf
    {
        protected double m_dTable54ReferenceTemperature;

        public TankApi54a()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A;
            m_bUsesDensity = false;
            m_dTable54ReferenceTemperature = TAB54_DEF_REF_TEMP;
            m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
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

            if (!ConvertEngUnits.ConvEngrUnits(ref dKgPerM3, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, dStdTempInC) || !ConvertEngUnits.ConvEngrUnits(ref dDegC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 0))
            {
                return false;
            }

            APICorrection(dKgPerM3, dDegC, ref pdVcfc, ref Iflag, true, bRound, m_dTable54ReferenceTemperature);

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
                                            dBp2 = 824.5f,
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

            int BelowApiTemp;

            pdVcfc = -1.0;
            piFlag = -1;
            BelowApiTemp = 0;

            //Round Input parameters
            if (bRound)
            {
                RoundDouble(dDen15, ref dDen15, 1, false, true);
                RoundDouble(dDegC, ref dDegC, 2, false, true);
            }

            /*
            * Calculate delta temp
            */
            dDt = dDegC - dBas;
            RoundDouble(dDt, ref dDt, 2, false, false);

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
                    if (dDegC <= dTmp1) { }
                    else
                        return;
                else if (dDen15 <= dBp2)
                    if (dDegC <= dTmp2) { }
                    else
                        return;
                else if (dDegC <= dTmp3) { }

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
            VCF(dAlpha, dDt, ref pdVcfc, dRefDegC);
            piFlag = 0;

            /*
            * Check if in extrapolated region
            */
            if (BelowApiTemp != 0)
                piFlag = -1;
            else if (dDen15 < dEden)
                piFlag = 1;
            else if (dDen15 >= dBp1)
                if (dDegC <= dEp1) { }
                else
                    piFlag = 1;
            else if (dDen15 >= dBp2)
                if (dDegC <= dEp2) { }
                else
                    piFlag = 1;
            else if (dDegC <= dEp3) { }
            else
                piFlag = 1;
        }
    }
}
