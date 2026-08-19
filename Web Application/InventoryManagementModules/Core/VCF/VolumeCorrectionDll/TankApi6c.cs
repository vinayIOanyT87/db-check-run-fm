using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi6c : TankBaseVcf
    {
        public TankApi6c()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6C;
            m_bUsesDensity = false;
            m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
        }


        public override bool TemperatureCorr(double dDensity,
                                              double dMeasTemp,
                                              double dStdTempInC,
                                              double dStdTemp,
                                              EngineeringUnit bDensityUnits,
                                              EngineeringUnit bTempUnits,
                                              double dDensityPress,          // density pressure for api 2004
                                              EngineeringUnit bDensityPressUnits, // density pressure units for api 2004
                                              double dAlternateTemperature,  // selected refined product sub catagory for api 2004
                                              double dBaseTemp,  // api 2004 alternate base temp reference
                                              double dAlternateBasePress,    // api 2004 alternate base pressure reference
                                              ref double[] dK,
                                              ref double pdVcfc,
                                              ref int Iflag,
												ref double CTLReturn,
												ref double CPLReturn,
											  bool RangeCk,
                                              bool bRound,
                                              bool bTable60,             //	Optional
                                              bool UseDensity)               //	Optional
        {
            double dDegF = 0;
            double dApi = 0;

            //FMTRACE(_T("CTankApi6c::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dApi, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegF, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                return false;

            APICorrection(dK[0], dDegF, ref pdVcfc, ref Iflag, bRound);

            return true;
        }

        public void APICorrection(double dAlf60,
                                                 double dDegF,
                                                 ref double pdVcfc,
                                                 ref int piFlag,
                                                 bool bRound)
        {
            const double dEp1 = 250.0,
                                            dEp2 = 200.0,
                                            dEp3 = 150.0,
                                            dNbp1 = 0.0002700,
                                            dNbp2 = 0.0009300,
                                            dBp1 = 0.0005100,
                                            dBp2 = 0.0005300,
                                            dTmp1 = 300.0,
                                            dTmp2 = 250.0,
                                            dTmp3 = 200.0,
                                            dBas = 60.0;

            double dDt;
            bool BelowApiTemp;

            pdVcfc = -1.0;
            piFlag = -1;
            BelowApiTemp = false;

            // round input parameters
            if (bRound)
            {
                RoundDouble(dAlf60, ref dAlf60, 7, false, true);
                RoundDouble(dDegF, ref dDegF, 1, false, false);
            }

            // Calculate delta temp

            dDt = dDegF - dBas;
            if (bRound)
            {
                RoundDouble(dDt, ref dDt, 1, false, false);
            }

            //  Check the input alpha and Temp ranges

            if (dAlf60 < dNbp1 || dAlf60 > dNbp2)
                return;

            if (dDegF < 0.0)
            {
                if (dDegF < LOWEST_FTEMP)
                    return;
                else
                    BelowApiTemp = true;
            }

            if (dAlf60 <= dBp1)
            {
                if (dDegF > dTmp1)
                    return;
            }
            else if (dAlf60 <= dBp2)
            {
                if (dDegF > dTmp2)
                    return;
            }
            else if (dDegF > dTmp3)
                return;

            // calc VCF

            VCF(dAlf60, dDt, ref pdVcfc, TAB54_DEF_REF_TEMP);
            piFlag = 0;

            // Check if in extrapolated region

            if (BelowApiTemp)
                piFlag = -1;
            else if (dAlf60 <= dBp1)
            {
                if (dDegF > dEp1)
                    piFlag = 1;
            }
            else if (dAlf60 <= dBp2)
            {
                if (dDegF > dEp2)
                    piFlag = 1;
            }
            else if (dDegF > dEp3)
                piFlag = 1;
        }
    }
}
