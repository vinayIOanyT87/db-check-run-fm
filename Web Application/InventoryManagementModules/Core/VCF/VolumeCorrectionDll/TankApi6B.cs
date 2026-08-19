using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Varec.CommonComponents.EngineeringUnitsLibrary;


namespace VCF
{
    public class TankApi6b : TankBaseVcf
    {
        public TankApi6b()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6B;
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
                                            ref int Iflag,
												ref double CTLReturn,
												ref double CPLReturn,
											bool RangeCk,
                                            bool bRound,
                                            bool bTable60,				//	Optional
                                            bool UseDensity)				//	Optional
        {
            double dApi = 0;
            double dDegF = 0;

            //FMTRACE(_T("CTankApi6b::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dApi, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegF, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                return false;

            APICorrection(dApi, dDegF, ref pdVcfc, ref Iflag, RangeCk, bRound);

            return true;
        }

        public void APICorrection(double dApi60,
                                                 double dDegF,
                                                 ref double pdVcfc,
                                                 ref int piFlag,
                                                 bool RangeCk,
                                                 bool bRound)
        {
            const double dNbp1 = 37.0,
                                            dNbp2 = 48.0,
                                            dNbp3 = 52.0,
                                            dNbp4 = 85.0,
                                            dBp1 = 40.0,
                                            dBp2 = 50.0,
                                            dTmp1 = 300.0,
                                            dTmp2 = 250.0,
                                            dTmp3 = 200.0,
                                            dBas = 60.0,
                                            dEp1 = 250.0,
                                            dEp2 = 200.0,
                                            dEp3 = 150.0,
                                            dK0f = 103.8720,
                                            dK1f = 0.2701,			// diesel , heating oils, fuel oils
                                            dK0j = 330.3010,
                                            dK1j = 0.0,				// jet fuels, kerosenes, solvents
                                            dK0t = 1489.0670,
                                            dK1t = -0.00186840,	// transition between jets and gasolines
                                            dK0g = 192.4571,
                                            dK1g = 0.2438;			// gasolines, naphthenes

            double dDt = 0.00, dK0 = 0.00, dK1 = 0.00, dRho = 0.00, dAlpha = 0.00;
            int BelowApiTemp = 0;

            pdVcfc = -1.0;
            piFlag = -1;
            BelowApiTemp = 0;

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

            //  Check the input API and Temp ranges

            if (dApi60 < 0.0)
                return;
            else if (dApi60 <= dNbp1)
            {
                dK0 = dK0f;
                dK1 = dK1f;
            }
            else if (dApi60 < dNbp2)
            {
                dK0 = dK0j;
                dK1 = dK1j;
            }
            else if (dApi60 <= dNbp3)
            {
                dK0 = dK0t;
                dK1 = dK1t;
            }
            else if (dApi60 <= dNbp4)
            {
                dK0 = dK0g;
                dK1 = dK1g;
            }
            else
            {
                return;
            }
            if (RangeCk)
            {
                if (dDegF < 0.0)
                {
                    if (dDegF < LOWEST_FTEMP)
                        return;
                    else
                        BelowApiTemp = 1;
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

            // ck if in transition region

            if (dK0 == dK0t)
            {
                double dTerm1, dTerm2;

                dTerm1 = dK0 / dRho;
                RoundDouble(dTerm1, ref dTerm1, 6, false, false);

                dTerm2 = dTerm1 / dRho;
                RoundDouble(dTerm2, ref dTerm2, 8, true, false);

                dAlpha = dK1 + dTerm2;

            }
            else
            {
                ALPHA(dRho, dK0, dK1, ref dAlpha);
            }

            RoundDouble(dAlpha, ref dAlpha, 8, false, false);

            /*
            *  calculate VCF
            */
            VCF(dAlpha, dDt, ref pdVcfc, TAB54_DEF_REF_TEMP);
            piFlag = 0;

            // Check if in extrapolated region

            if (BelowApiTemp != 0)
                piFlag = -1;
            else if (dApi60 <= dBp1)
            {
                if (dDegF > dEp1)
                    piFlag = 1;
            }
            else if (dApi60 <= dBp2)
            {
                if (dDegF > dEp2)
                    piFlag = 1;
            }
            else if (dDegF > dEp3)
                piFlag = 1;
        }


    }
}
