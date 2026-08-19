using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi54c : TankBaseVcf
    {
        protected double m_dTable54ReferenceTemperature;

        public TankApi54c()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54C;
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
                                            ref int piFlag,
												ref double CTLReturn,
												ref double CPLReturn,
											bool RangeCk,
                                            bool bRound,
                                            bool bTable60,				//	Optional
                                            bool UseDensity)				//	Optional
        {
            double dDegC = 0;

            //FMTRACE(_T("CTankApi54c::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dDegC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 0))
                return false;

            APICorrection(dK[0], dDegC, ref pdVcfc, ref piFlag, bRound, m_dTable54ReferenceTemperature);

            return true;
        }

        public void APICorrection(double dAlf15,
                                                  double dDegC,
                                                  ref double pdVcfc,
                                                  ref int piFlag,
                                                  bool bRound,
                                                  double dRefDegC)
        {
            const double dEp1 = 120.00,
                                        dEp2 = 90.00,
                                        dEp3 = 60.00,
                                        dNbp1 = 0.0004860,
                                        dNbp2 = 0.0016740,
                                        dBp1 = 0.0009180,
                                        dBp2 = 0.0009540,
                                        dTmp1 = 150.00,
                                        dTmp2 = 125.00,
                                        dTmp3 = 95.00,
                                        dBas = 15.00,
                                        dBt1 = -18.00;

            double dDt = 0.00;

            bool BelowApiTemp;

            pdVcfc = -1.0;
            piFlag = -1;
            BelowApiTemp = false;

            /*
           * Round Input parameters 
           */
            if (bRound)
            {
                RoundDouble(dAlf15, ref dAlf15, 6, false, false);
                RoundDouble(dDegC, ref dDegC, 2, false, true);
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
            *  Check the input alpha and Temp ranges
            */
            if (dAlf15 < dNbp1)
                return;
            else if (dAlf15 > dNbp2)
                return;
            else if (dDegC < dBt1)
            {
                if (dDegC < LOWEST_CTEMP) return;
                else BelowApiTemp = true;
            }

            if (dAlf15 <= dBp1)
                if (dDegC <= dTmp1)
				{ }
                else
                    return;
            else if (dAlf15 <= dBp2)
                if (dDegC <= dTmp2)
				{ }
                else
                    return;
            else if (dDegC <= dTmp3)
			{ }
            else
                return;

            /*
            * calc VCF
            */
            VCF(dAlf15, dDt, ref pdVcfc, dRefDegC);
            piFlag = 0;

            /*
            * Check if in extrapolated region
            */
            if (BelowApiTemp)
                piFlag = -1;
            else if (dAlf15 <= dBp1)
                if (dDegC <= dEp1)
				{ }
                else
                    piFlag = 1;
            else if (dAlf15 <= dBp2)
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
