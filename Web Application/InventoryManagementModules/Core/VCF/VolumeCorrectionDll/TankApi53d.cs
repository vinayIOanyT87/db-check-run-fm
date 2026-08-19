using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi53d : TankApi54d
    {
        public TankApi53d()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D;
            m_bUsesDensity = true;
            m_dTable54ReferenceTemperature = TAB54_DEF_REF_TEMP;
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
                                                 bool UseDensity)				//	Optional
        {
            double dKgPerM3 = 0;
            double dDegC = 0;

            //FMTRACE(_T("CTankApi53d::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dKgPerM3, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 0))
                return false;

            if (UseDensity)
                APICorrection(dKgPerM3, dDegC, ref pdVcfc, ref Iflag, m_dTable54ReferenceTemperature, bTable60, bRound);
            else
            {
                //		CTankApi54d::APICorrection(dKgPerM3, dDegC, pdVcfc, Iflag, TRUE, TRUE, m_dTable54ReferenceTemperature);
                base.APICorrection(dKgPerM3, dDegC, ref pdVcfc, ref Iflag, true, bRound, m_dTable54ReferenceTemperature);
            }


            return true;
        }


        public void APICorrection(double dDen,
                                                  double dDegC,
                                                  ref double pdVcfc,
                                                  ref int piFlag,
                                                  double dRefDegC,
                                                  bool bTable60,
                                                  bool bRound)
        {
            double dVcfc = 0.00, dStdDen1 = 0.00, dStdDen2 = 0.00;
            int iFlag = 0;
            bool RangeCk = false;
            int i;

            //Round Temp Input parameter
            if (bRound)
            {
                RoundDouble(dDegC, ref dDegC, 2, false, true);
            }

            // apply hydrometer correction
            // NOTE:- hydro correction for Table 60D performed in function TnkApi60d
            if (!bTable60)
            {
                //Round Input parameters
                if (bRound)
                {
                    RoundDouble(dDen, ref dDen, 1, false, true);
                }
                double dHydrometer = 0.00;
                ApplyHydroCorrection(ref dDen, dDegC, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D, false, ref dHydrometer);
            }

            // round density result value to nearest 0.01
            if (bRound)
            {
                RoundDouble(dDen, ref dDen, 2, false, false);
            }

            for (i = MAX_ITERATIONS, RangeCk = true, iFlag = 0, dStdDen1 = dDen; --i != 0 && iFlag != -1; dStdDen1 = dStdDen2, RangeCk = false)
            {
                base.APICorrection(dStdDen1, dDegC, ref dVcfc, ref iFlag, RangeCk, false, dRefDegC);  // call API54D - no rounding
                dStdDen2 = dDen / dVcfc;
                if (Math.Abs(dStdDen2 - dStdDen1) <= MAX_DENSITY_DIFF)
                    break;
            }
            piFlag = iFlag;
            pdVcfc = dVcfc;
        }
    }
}
