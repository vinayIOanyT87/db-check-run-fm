using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi6A : TankBaseVcf
    {
        public TankApi6A()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6A;
            m_bUsesDensity = false;
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
                                            ref int piFlag,
												ref double CTLReturn,
												ref double CPLReturn,
											bool RangeCk,
                                            bool bRound,
                                            bool bTable60,				//	Optional
                                            bool UseDensity)				//	Optional
        {
            double dApi = 0;
            double dDegF = 0;

            //FMTRACE(_T("CTankApi6a::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dApi, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegF, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                return false;

            APICorrection(dApi, dDegF, ref pdVcfc, ref piFlag, true, bRound);

            return true;
        }

        public void APICorrection(double dApi60,
                                                    double dDegF,
                                                    ref double pdVcfc,
                                                    ref int piFlag,
                                                    bool RangeCk,
                                                    bool bRound)
        {
            const double dBas = 60.0,
                                           dBp1 = 40.0,
                                           dBp2 = 50.0,
                                           dTmp1 = 300.0,
                                           dTmp2 = 250.0,
                                           dTmp3 = 200.0,
                                           dEp1 = 250.0,
                                           dEp2 = 200.0,
                                           dEp3 = 150.0,
                                           dEapi = 55.0,
                                           dK0 = 341.0957,
                                           dK1 = 0.0;

            double dDt, dAlpha = 0, dRho = 0;
            bool BelowApiTemp = false;

            pdVcfc = -1.0;
            piFlag = -1;

            // round input parameters
            if (bRound)
            {
                RoundDouble(dApi60, ref dApi60, 1, false, false);
                RoundDouble(dDegF, ref dDegF, 1, false, false);
            }

            // Calculate delta temp
            dDt = dDegF - dBas;
            if (bRound)
            {
                RoundDouble(dDt, ref dDt, 1, false, false);
            }


            // Check the input API and Temp ranges

            if (RangeCk)
            {
                if (dApi60 < 0 || dApi60 > 100.0)
                    return;

                if (dDegF < 0.0)
                {
                    if (dDegF < LOWEST_FTEMP)
                        return;
                    else
                        BelowApiTemp = true;
                }

                if (dApi60 <= dBp1)
                {
                    if (dDegF > dTmp1)
                        return;
                }
                else if (dApi60 <= dBp2)
                {
                    if (dDegF > dTmp2)
                        return;
                }
                else if (dDegF > dTmp3)
                    return;
            }

            // calculate rho

            RHO(dApi60, ref dRho);

            // calc alpha
            ALPHA(dRho, dK0, dK1, ref dAlpha);
            RoundDouble(dAlpha, ref dAlpha, 7, false, false);

            // calc VCF
            VCF(dAlpha, dDt, ref pdVcfc, TAB54_DEF_REF_TEMP);
            piFlag = 0;

            // Check if in extrapolated region

            if (BelowApiTemp)
                piFlag = -1;
            else if (dApi60 > dEapi)
                piFlag = 1;
            else if (dApi60 <= dBp1)
            {
                if (dDegF > dEp1)
                    piFlag = 1;
            }
            else if (dDegF <= dBp2)
            {
                if (dDegF > dEp2)
                    piFlag = 1;
            }
            else if (dDegF > dEp3)
                piFlag = 1;

        } // End of APICorrection()
    }
}
