using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi60b : TankApi53b
    {
        public TankApi60b()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60B;
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

            //FMTRACE(_T("CTankApi60b::TemperatureCorr"));
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
            const double dNbp1 = 770.5,
                                       dNbp2 = 787.5,
                                       dNbp3 = 839.0,
                                       dNbp4 = 1075.0,
                                       dBp1 = 778.5,
                                       dBp2 = 824.0,
                                       dBp3 = 1075.0,
                                       dTmp1 = 95.00,
                                       dTmp2 = 125.00,
                                       dTmp3 = 150.00,
                                       dBas = 20.00,
                                       dBt1 = -18.00,
                                       dFden = 653.0,
                                       dK0f = 186.9696,
                                       dK1f = 0.4862,          // diesel , heating oils, fuel oils
                                       dK0j = 594.5418,
                                       dK1j = 0.0,             // jet fuels, kerosenes, solvents
                                       dK0t = 2680.3206,
                                       dK1t = -0.00336312, // transition between jets and gasolines
                                       dK0g = 346.4228,
                                       dK1g = 0.4388;          // gasolines, naphthenes

            double dVcfc = 0.00, dTemp = 0.00, dDen15 = 0.00, dDt = 0.00;
            double dK0 = 0.00, dK1 = 0.00, dAlf = 0.00;

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
            ApplyHydroCorrection(ref dDen20, dTemp, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60B, false, ref dHydrometer);

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
            * Calculate delta temp , use 20 C as base
            */
            dDt = dDegC - dBas;
            if (bRound)
            {
                RoundDouble(dDt, ref dDt, 2, false, false);
            }

            /*
            *  Check the input density and Temp ranges
            */
            if (dDen15 < dFden)
                return;
            else if (dDen15 < dNbp1)
            {
                dK0 = dK0g; dK1 = dK1g;
            }
            else if (dDen15 <= dNbp2)
            {
                dK0 = dK0t; dK1 = dK1t;
            }
            else if (dDen15 < dNbp3)
            {
                dK0 = dK0j; dK1 = dK1j;
            }
            else if (dDen15 <= dNbp4)
            {
                dK0 = dK0f; dK1 = dK1f;
            }
            else
            {
                return;
            }

            if (RangeCk)
            {

                if ((dDen15 < dFden) || (dDen15 > dBp3) || (dDegC < dBt1))
                    return;

                if ((dDen15 <= dBp1) && (dDegC > dTmp1))
                    return;

                if ((dDen15 <= dBp2) && (dDegC > dTmp2))
                    return;

                if (dDegC > dTmp3)
                    return;
            }

            /*
            * ck if in transition region , calculate ALPHA
            */
            if (dK0 == dK0t)
            {
                double dTerm1, dTerm2;

                dTerm1 = dK0 / dDen15;
                RoundDouble(dTerm1, ref dTerm1, 6, true, false);

                dTerm2 = dTerm1 / dDen15;
                RoundDouble(dTerm2, ref dTerm2, 8, false, false);

                dAlf = dK1 + dTerm2;
            }
            else
            {
                ALPHA(dDen15, dK0, dK1, ref dAlf);
            }

            RoundDouble(dAlf, ref dAlf, 7, false, false);

            /*
            * calc vcf
            */
            VCF60(dAlf, dDt, ref pdVcfc);
            piFlag = 0;
        }
    }
}
