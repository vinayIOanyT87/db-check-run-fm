using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi54b : TankBaseVcf
    {
        protected double m_dTable54ReferenceTemperature;

        public TankApi54b()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B;
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
            int iJettoGas = 1;
            double dKgPerM3 = 0;
            double dDegC = 0;

            //FMTRACE(_T("CTankApi54b::TemperatureCorr"));
            if (!ConvertEngUnits.ConvEngrUnits(ref dKgPerM3, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegC, dMeasTemp, EngineeringUnit.FmtDegC, bTempUnits, 0))
                return false;

            APICorrection(dKgPerM3, dDegC, ref pdVcfc, ref Iflag, true, bRound, ref iJettoGas, m_dTable54ReferenceTemperature);

            return true;
        }

        public void APICorrection(double dDen15,
                                                  double dDegC,
                                                  ref double pdVcfc,
                                                  ref int piFlag,
                                                  bool RangeCk,
                                                  bool bRound,
                                                  ref int iJettoGas,
                                                  double dRefDegC)
        {
            const double dNbp1 = 770.5,
                                        dNbp2 = 787.5,
                                        dNbp3 = 839.0,
                                        dNbp4 = 1075.0,
                                        dBp1 = 779.0,
                                        dBp2 = 824.5,
                                        dBp3 = 1075.0,
                                        dTmp1 = 95.00,
                                        dTmp2 = 125.00,
                                        dTmp3 = 150.00,
                                        dBas = 15.00,
                                        dEp1 = 60.00,
                                        dEp2 = 90.00,
                                        dEp3 = 120.00,
                                        dBt1 = -18.00,
                                        dFden = 653.0,
                                        dK0f = 186.9696,
                                        dK1f = 0.4862,			// diesel , heating oils, fuel oils
                                        dK0j = 594.5418,
                                        dK1j = 0.0,				// jet fuels, kerosenes, solvents
                                        dK0t = 2680.3206,
                                        dK1t = -.00336312,	// transition between jets and gasolines
                                        dK0g = 346.4228,
                                        dK1g = 0.4388;			// gasolines, naphthenes

            double dK0 = 0.00, dK1 = 0.00, dDt = 0.00, dAlpha = 0.00;

            int BelowApiTemp;

            pdVcfc = -1.0;
            piFlag = -1;
            BelowApiTemp = 0;

            // round input vars
            if (bRound)
            {
                RoundDouble(dDen15, ref dDen15, 1, false, true);
                RoundDouble(dDegC, ref dDegC, 2, false, true);
            }

            /*
            * Calculate delta temp
            */
            dDt = dDegC - dBas;
            RoundDouble(dDt, ref dDt, 2, false, true);

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
                if (dDegC < dBt1)
                {
                    if (dDegC < LOWEST_CTEMP) return;
                    else BelowApiTemp = 1;
                }
                if (dDen15 <= dBp1)
                    if (dDegC <= dTmp1)
                        if (dDegC <= dTmp3)
                        { }
                        else
                            return;
                    else
                        return;
                else if (dDen15 <= dBp2)
                    if (dDegC <= dTmp2)
                        if (dDegC <= dTmp3)
                        { }
                        else
                            return;
                    else
                        return;
                else if (dDen15 > dBp3)
                    return;
                else if (dDegC <= dTmp3)
                { }
                else
                    return;
            }

            /*
            * ck if in transition region
            */
            if (dK0 == dK0t)
            {

                double dTerm1, dTerm2;

                if (iJettoGas == 0)
                {
                    iJettoGas = 1;
                    // intialize dDen15 = 778.84
                    dDen15 = 778.84;
                }

                dTerm1 = dK0 / dDen15;
                RoundDouble(dTerm1, ref dTerm1, 6, true, false);

                dTerm2 = dTerm1 / dDen15;
                RoundDouble(dTerm2, ref dTerm2, 8, false, false);

                dAlpha = dK1 + dTerm2;

            }
            else
            {
                ALPHA(dDen15, dK0, dK1, ref dAlpha);
            }

            RoundDouble(dAlpha, ref dAlpha, 7, false, false);

            /*
            * calc vcf
            */
            VCF(dAlpha, dDt, ref pdVcfc, dRefDegC);
            piFlag = 0;

            /*
            * Check if in extrapolated region
            */
            if (BelowApiTemp != 0)
                piFlag = -1;
            else if (dDen15 > dBp1)
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
