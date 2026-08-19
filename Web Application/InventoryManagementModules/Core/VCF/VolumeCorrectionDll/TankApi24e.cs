using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi24e : TankBaseVcf
    {
        #region structures
        public struct TABLE24E
        {
            public double dRelativeDensity;
            public double dCompressibility;
            public double dCriticalTempInK;
            public double dCriticalDensity;
            public double[] dSaturationDensity;
        } ;
        #endregion //defines

        #region members
        protected static TABLE24E[] TABLE24E_LOOKUP = new TABLE24E[]
        {
        //                          Crit.
        // Density   Comp.   Temp   Dens.      K1            K2               K3               K4
        //--------------------------------------------------------------------------------------------
	        new TABLE24E(){dRelativeDensity = 0.325022,dCompressibility = 0.27998,dCriticalTempInK = 298.11,dCriticalDensity = 6.250,dSaturationDensity = new double[4]{2.54616855327,-0.058244177754,0.803398090807,-0.745720314137}},	// 0  - EE (68/32) (1)
	        new TABLE24E(){dRelativeDensity = 0.355994,dCompressibility = 0.28220,dCriticalTempInK = 305.33,dCriticalDensity = 6.870,dSaturationDensity = new double[4]{1.89113042610,-0.370305782347,-0.544867288720,0.337876634952}},	// 1  - Ethane
	        new TABLE24E(){dRelativeDensity = 0.429277,dCompressibility = 0.28060,dCriticalTempInK = 333.67,dCriticalDensity = 5.615,dSaturationDensity = new double[4]{2.20970078464,-0.294253708172,-0.405754420098,0.319443433421}},	// 2  - EP (65/35) (2)
	        new TABLE24E(){dRelativeDensity = 0.470381,dCompressibility = 0.27930,dCriticalTempInK = 352.46,dCriticalDensity = 5.110,dSaturationDensity = new double[4]{2.25341981320,-0.266542138024,-0.372756711655,0.384734185665}},	// 3  - EP (65/35) (3)
	        new TABLE24E(){dRelativeDensity = 0.507025,dCompressibility = 0.27626,dCriticalTempInK = 369.78,dCriticalDensity = 5.000,dSaturationDensity = new double[4]{1.96568366933,-0.327662435541,-0.417979702538,0.303271602831}},	// 4  - Propane
	        new TABLE24E(){dRelativeDensity = 0.562827,dCompressibility = 0.28326,dCriticalTempInK = 407.85,dCriticalDensity = 3.860,dSaturationDensity = new double[4]{2.04748034410,-0.289734363425,-0.330345036434,0.291757103132}},	// 5  - i-Butane
	        new TABLE24E(){dRelativeDensity = 0.584127,dCompressibility = 0.27536,dCriticalTempInK = 425.16,dCriticalDensity = 3.920,dSaturationDensity = new double[4]{2.03734743118,-0.299059145695,-0.418883095671,0.380367738748}},	// 6  - n-Butane
	        new TABLE24E(){dRelativeDensity = 0.624285,dCompressibility = 0.27026,dCriticalTempInK = 460.44,dCriticalDensity = 3.247,dSaturationDensity = new double[4]{2.06541640707,-0.238366208840,-0.161440492247,0.258681568613}},	// 7  - i-Pentane
	        new TABLE24E(){dRelativeDensity = 0.631054,dCompressibility = 0.27235,dCriticalTempInK = 469.65,dCriticalDensity = 3.200,dSaturationDensity = new double[4]{2.11263474494,-0.261269413560,-0.291923445075,0.308344290017}},	// 8  - n-Pentane
	        new TABLE24E(){dRelativeDensity = 0.657167,dCompressibility = 0.26706,dCriticalTempInK = 498.05,dCriticalDensity = 2.727,dSaturationDensity = new double[4]{2.02382197871,-0.423550090067,-1.152810982570,0.950139001678}},	// 9  - i-Hexane
	        new TABLE24E(){dRelativeDensity = 0.664064,dCompressibility = 0.26762,dCriticalTempInK = 507.35,dCriticalDensity = 2.704,dSaturationDensity = new double[4]{2.17134547773,-0.232997313405,-0.267019794036,0.378629524102}},	// 10 - n-Hexane
	        new TABLE24E(){dRelativeDensity = 0.688039,dCompressibility = 0.26312,dCriticalTempInK = 540.15,dCriticalDensity = 2.315,dSaturationDensity = new double[4]{2.19773533433,-0.275056764147,-0.447144095029,0.493770995799}}	// 11 - n-Heptane
        };
        #endregion

        public TankApi24e()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API24E;
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
            double dSpecificGravity = 0;
            double dDegF = 0;

            if (!ConvertEngUnits.ConvEngrUnits(ref dSpecificGravity, dDensity, EngineeringUnit.FmdSpGrav, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegF, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                return false;

            APICorrection(dSpecificGravity, dDegF, ref pdVcfc, ref piFlag, bRound);

            return true;

        } // End TemperatureCorr()

        protected void CalcSaturationDensity(ref TABLE24E pFluid,
                                                             ref double dReducedCriticalTemp,
                                                             ref double pdSaturationDensity)
        {
            double dt = 1 - dReducedCriticalTemp;

            double dSaturationDens = pFluid.dCriticalDensity * (1 + (pFluid.dSaturationDensity[0] * Math.Pow(dt, 0.35) +
                                                  pFluid.dSaturationDensity[2] * Math.Pow(dt, 2) + pFluid.dSaturationDensity[3] * Math.Pow(dt, 3))
                                                  / (1 + pFluid.dSaturationDensity[1] * Math.Pow(dt, 0.65)));
            pdSaturationDensity = dSaturationDens;

        } // End of CalcSaturationDensity()

        protected void CalculateCTL(ref TABLE24E pFluid2,
                                                 ref TABLE24E pFluid1,
                                                 ref double dRoundedDensity,
                                                 ref double dTempInK,
                                                 ref int piFlag,
                                                 ref double pdVcf)
        {
            // STEP 5
            double dInterpolatingVar = (dRoundedDensity - pFluid1.dRelativeDensity) / (pFluid2.dRelativeDensity - pFluid1.dRelativeDensity);

            // STEP 6
            double dCriticalTemp = pFluid1.dCriticalTempInK + dInterpolatingVar * (pFluid2.dCriticalTempInK - pFluid1.dCriticalTempInK);

            // STEP 7
            double dReducedObservedTemp = dTempInK / dCriticalTemp;
            if (dReducedObservedTemp > 1.0)
            {
                piFlag = -1;
                return;
            }

            // STEP 8
            double dReducedCriticalTemp = 519.67 / (1.8 * dCriticalTemp);

            // STEP 9
            double dScalingFactor = (pFluid1.dCompressibility * pFluid1.dCriticalDensity)
                                            / (pFluid2.dCompressibility * pFluid2.dCriticalDensity);

            // STEP 10
            double dSaturationDens60_2 = 0.0;
            CalcSaturationDensity(ref pFluid2, ref dReducedCriticalTemp, ref dSaturationDens60_2);

            double dSaturationDens60_1 = 0.0;
            CalcSaturationDensity(ref pFluid1, ref dReducedCriticalTemp, ref dSaturationDens60_1);

            // STEP 11
            double dInterpolatingFactor = dSaturationDens60_1 / (1 + dInterpolatingVar *
                                                    ((dSaturationDens60_1 / (dScalingFactor * dSaturationDens60_2) - 1)));

            // STEP 12

            double dSaturationDensX_2 = 0.0;
            CalcSaturationDensity(ref pFluid2, ref dReducedObservedTemp, ref dSaturationDensX_2);

            double dSaturationDensX_1 = 0.0;
            CalcSaturationDensity(ref pFluid1, ref dReducedObservedTemp, ref dSaturationDensX_1);

            // STEP 13
            pdVcf = dSaturationDensX_1 / (dInterpolatingFactor * (1 + (dInterpolatingVar * ((dSaturationDensX_1 / (dScalingFactor * dSaturationDensX_2)) - 1))));

        } // End of CalculateCTL()

        public void APICorrection(double dSpecificGravity,
                                                    double dDegF,
                                                    ref double pdVcfc,
                                            ref int piFlag,
                                            bool bRound)
        {
            // STEP 1

            double dRoundedTempInF = 0.0;
            double dRoundedDensity = 0.0;

            if (bRound)
            {
                RoundDouble(dDegF, ref dRoundedTempInF, 1, false, false);
                RoundDouble(dSpecificGravity, ref dRoundedDensity, 4, false, false);
            }
            else
            {
                dRoundedTempInF = dDegF;
                dRoundedDensity = dSpecificGravity;
            }

            // STEP 2

            if (dRoundedTempInF < -50.0 || dRoundedTempInF > 200.0
                || dRoundedDensity < 0.35 || dRoundedDensity > 0.688)
            {
                piFlag = -1;
                return;
            }

            // STEP 3

            double dTemperatureInK = 0.0;
            if (!ConvertEngUnits.ConvEngrUnits(ref dTemperatureInK, dRoundedTempInF, EngineeringUnit.FmtDegK, EngineeringUnit.FmtDegF, 0))
            {
                piFlag = -1;
                return;
            }

            // STEP 4

            // Determine the reference fluids from the table
            TABLE24E pFluid2 = new TABLE24E();
            TABLE24E pFluid1 = new TABLE24E();

            bool bFluidFound = false;
            int iFluids = TABLE24E_LOOKUP.Length;
            for (int iLoop = 1; iLoop < iFluids; iLoop++)
            {
                if (dRoundedDensity <= TABLE24E_LOOKUP[iLoop].dRelativeDensity)
                {
                    pFluid2 = TABLE24E_LOOKUP[iLoop];
                    pFluid1 = TABLE24E_LOOKUP[iLoop - 1];
                    bFluidFound = true;
                    break;
                } // End if (dRoundedDensity <= TABLE24E_LOOKUP.dRelativeDensity)

            } // End of iLoop

            if (!bFluidFound)
            {
                piFlag = -1;
                return;

            } // End if (!bFluidFound)

            CalculateCTL(ref pFluid2, ref pFluid1, ref dRoundedDensity, ref dTemperatureInK, ref piFlag, ref pdVcfc);

            if (bRound)
            {
                RoundDouble(pdVcfc, ref pdVcfc, 5, false, false);
            }

        } // End of APICorrection()
    }
}
