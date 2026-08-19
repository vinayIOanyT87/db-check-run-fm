using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi60d : TankApi53d
    {
        public TankApi60d()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60D;
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

            //FMTRACE(_T("CTankApi60d::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dKgPerM3, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 0))
                return false;

            APICorrection(dKgPerM3, dDegC, ref pdVcfc, ref Iflag, RangeCk, UseDensity, bRound);

            return true;
        }


        public void APICorrection(double dDen20,
                                                  double dDegC,
                                                  ref double pdVcfc,
                                                  ref int piFlag,
                                                  bool RangeCk,
                                                  bool UseDensity,
                                                  bool bRound)
        {
            const double dBas = 20.00,
                                       dBp1 = 824.0,
                                       dNbp1 = 1164.0,
                                       dTmp1 = 125.00,
                                       dTmp2 = 150.00,
                                       dFden = 800.0,
                                       dBt1 = -18.00,
                                       dK0 = 0.0,
                                       dK1 = 0.6278;

            double dVcfc = 0.00, dTemp = 0.00, dDen15 = 0.00;
            double dDt = 0.00, dAlf = 0.00;

            int iFlag = 0;

            pdVcfc = -1.0;
            piFlag = -1;

            /*
             * convert density to 15 C
             */
            if (true == UseDensity)
                dTemp = dDegC;              // uncorrected density , so use product temperature
            else
                dTemp = 20.0f;              // density corrected to 20C , so use 20.0 C as temperature

            //Round Input parameters
            if (true == UseDensity)
            {
                if (bRound)
                {
                    RoundDouble(dDen20, ref dDen20, 1, false, true);
                }
            }

            if (bRound)
            {
                RoundDouble(dTemp, ref dTemp, 2, false, true);
            }

            // apply hydro correction here
            double dHydrometer = 0.00;
            ApplyHydroCorrection(ref dDen20, dTemp, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60D, false, ref dHydrometer);

            // round density result value to nearest 0.01
            if (bRound)
            {
                RoundDouble(dDen20, ref dDen20, 2, false, false);
            }

            base.APICorrection(dDen20, dTemp, ref dVcfc, ref iFlag, TAB54_DEF_REF_TEMP, true, bRound);

            if (iFlag == -1)
                dDen15 = -1;
            else
                dDen15 = dDen20 / dVcfc;

            // round density result value to nearest 0.01
            if (bRound)
            {
                RoundDouble(dDen15, ref dDen15, 2, false, false);
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
                if ((dDen15 < dFden) || (dDen15 > dNbp1) || (dDegC < dBt1))
                    return;

                if ((dDen15 <= dBp1) && (dDegC > dTmp1))
                    return;

                if (dDegC > dTmp2)
                    return;

            }
            /*
            * calc alpha
            */
            ALPHA(dDen15, dK0, dK1, ref dAlf);

            /*
            * calc VCF
            */
            VCF60(dAlf, dDt, ref pdVcfc);
            piFlag = 0;
        }
    }
}
