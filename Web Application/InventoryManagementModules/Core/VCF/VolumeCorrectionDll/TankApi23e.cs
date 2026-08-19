using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class TankApi23e : TankApi24e
    {
        public TankApi23e()
        {
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API24E;
            m_bUsesDensity = true;
            m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;

        } // End of constructor

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
            double dSpecificGravity = 0;
            double dDegF = 0;

            if (!ConvertEngUnits.ConvEngrUnits(ref dSpecificGravity, dDensity, EngineeringUnit.FmdSpGrav, bDensityUnits, dStdTempInC)
                || !ConvertEngUnits.ConvEngrUnits(ref dDegF, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                return false;

#if _TEST // Run the standard API tests during debug and ASSERT if errors found
	
	struct TEST_STRUCT
	{
		double	dDens;
		double	dTemp;
		BOOL		bDisableHydroCorr;
		double	dVcf;
		int		iFlag;
	} TestData[] = 
	{
		0.67432,-23.333,TRUE,1.068531730459,0,	// Ex.1
		0.24573,189.98,TRUE,0.502607454450,0,		// Ex.2
		0.50004,190.04,TRUE,0.845011991820,0,		// Ex.3
		0.22238,87.28,TRUE,0.635428359299,0,		// Ex.4
		0.21028,187.94,FALSE,0.0,-1,					// Ex.5
		0.73950,-28.48,FALSE,0.0,-1,					// Ex.6
		0.34006,64.63,FALSE,0.0,-1,					// Ex.7
		0.72776,-27.53,FALSE,0.0,-1,					// Ex.8
		0.24087,-24.76,FALSE,0.0,-1,					// Ex.9
		0.25776,179.28,FALSE,0.539131012769,0,		// Ex.10
		0.39548,59.78,FALSE,1.000681086591,0,		// Ex.11
		0.21056,87.46,FALSE,0.601090815523,0,		// Ex.12
		0.45003,199.73,FALSE,0.794711736658,0,		// Ex.13
		0.601332,177.17,FALSE,0.906953294554,0,	// Ex.14
		0.73498,-44.13,FALSE,1.069654976925,0		// Ex.15
	};

	double	dTest					= 0.0;
	int		iFlagTest			= 0;
	double	dRoundedActualVcf = 0.0;
	int iTests = sizeof (TestData) / sizeof (TEST_STRUCT);
	for (int iTest=0; iTest < iTests; iTest++)
	{
		dRoundedActualVcf = 0.0;
		dTest = 0.0;
		iFlagTest = 0;
		TEST_STRUCT* pTest = &TestData[iTest];
		APICorrection(pTest->dDens,pTest->dTemp,&dTest,&iFlagTest,pTest->bDisableHydroCorr);	
		RoundDouble(pTest->dVcf,&dRoundedActualVcf,5,FALSE,FALSE);
		ASSERT(dRoundedActualVcf==dTest && pTest->iFlag==iFlagTest);
	}

#endif // #if _DEBUG

            APICorrection(dSpecificGravity, dDegF, ref pdVcfc, ref Iflag, false, bRound);

            return true;
        }



        protected bool CalcRelativeDensityAtObservedTemp(TABLE24E pFluid,
                                                                     double dTempInK,
                                                                     ref double pdObservedDensity)
        {
            // Calculate the reduced observed temperature for this fluid
            double dFluidReducedTemp = dTempInK / pFluid.dCriticalTempInK;
            if (dFluidReducedTemp > 1.0)
            {
                pdObservedDensity = 0.0;
                return false;
            } // End of if (dFluidReducedTemp <= 1.0)

            // Calculate the saturation density for this fluid at the reduced temperature
            double dFluidSaturationDens = 0.0;
            CalcSaturationDensity(ref pFluid, ref dFluidReducedTemp, ref dFluidSaturationDens);
            // Calculate the saturation density for this fluid at 60F
            double dFluidStdSaturationDens = 0.0;
            double dFluidReducedStdTemp = 519.67 / (1.8 * pFluid.dCriticalTempInK);
            CalcSaturationDensity(ref pFluid, ref dFluidReducedStdTemp, ref dFluidStdSaturationDens);
            // Calculate the relative density at the observation temperature
            pdObservedDensity = pFluid.dRelativeDensity * (dFluidSaturationDens / dFluidStdSaturationDens);

            RoundDouble(pdObservedDensity, ref pdObservedDensity, 12, false, false);

            return true;

        } // End CalcRelativeDensityAtObservedTemp()

        protected struct tagBOUNDSVAR
        {
            public double dDen60High;
            public double dDenXHigh;
            public double dDen60Low;
            public double dDenXLow;
            public double dDen60Mid;
            public double dDenXMid;
            public double dDen60Trial;
            public double dDenXTrial;
        };

        public void APICorrection(double dSpecificGravity,
                                                    double dDegF,
                                                    ref double pdVcfc,
                                                    ref int piFlag,
                                                    bool bDisableHydroCorr,
                                                    bool bRound)
        {
            double dVcfc = 0.0;
            double dDensity = 0.0;
            double dDifference = 0.0;

            const double MAX_DIFFERENCE = 1.0e-8;

            double dRoundedTempInF = 0.0;
            double dRoundedDensity = 0.0;

            // STEP 1

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

            double dTempInK = 0.0;
            if (!ConvertEngUnits.ConvEngrUnits(ref dTempInK, dRoundedTempInF, EngineeringUnit.FmtDegK, EngineeringUnit.FmtDegF, 0))
            {
                piFlag = -1;
                return;
            }

            // STEP 3

            double dCorrectedRelativeDensity = dRoundedDensity;
            if (!bDisableHydroCorr)
            {
                double dHydrometer = 0.00;
                ApplyHydroCorrection(ref dCorrectedRelativeDensity, dRoundedTempInF, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F, Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API24E, false, ref dHydrometer);
            }

            // STEP 4

            if (dRoundedTempInF < -50.0 || dRoundedTempInF > 200.0
                || dCorrectedRelativeDensity < 0.21 || dCorrectedRelativeDensity > 0.74)
            {
                piFlag = -1;
                return;
            }

            int iFluids = TABLE24E_LOOKUP.Count();


            tagBOUNDSVAR Bounds = new tagBOUNDSVAR()
            {
                dDen60High = 0.00,
                dDenXHigh = 0.00,
                dDen60Low = 0.00,
                dDenXLow = 0.00,
                dDen60Mid = 0.00,
                dDenXMid = 0.00,
                dDen60Trial = 0.00,
                dDenXTrial = 0.00
            };



            // Determine the reference fluids from the table
            TABLE24E pFluid2 = new TABLE24E();
            TABLE24E pFluid1 = new TABLE24E();

            // STEP 6 - With Step 5 embedded

            bool bFluidFound = false;
            int iLoop;
            for (iLoop = 1; iLoop < iFluids; iLoop++)
            {
                double dCurrentFluidDenX = 0.0;
                CalcRelativeDensityAtObservedTemp(TABLE24E_LOOKUP[iLoop], dTempInK, ref dCurrentFluidDenX);

                // If this Fluid has the lowest density reference
                if (dCorrectedRelativeDensity <= dCurrentFluidDenX)
                {
                    pFluid2 = TABLE24E_LOOKUP[iLoop];
                    pFluid1 = TABLE24E_LOOKUP[iLoop - 1];

                    Bounds.dDenXHigh = dCurrentFluidDenX;
                    CalcRelativeDensityAtObservedTemp(pFluid1, dTempInK, ref Bounds.dDenXLow);
                    Bounds.dDen60High = pFluid2.dRelativeDensity;
                    Bounds.dDen60Low = pFluid1.dRelativeDensity;

                    bFluidFound = true;
                    break;
                } // if (dCorrectedRelativeDensity <= TABLE24E_LOOKUP[iLoop].dRelativeDensity)

            } // End of iLoop

            if (!bFluidFound)
            {
                piFlag = -1;
                return;

            } // End if (!bFluidFound)

            // STEP 7

            // Initialize the upper bound for the observed fluids 60 degree relative density

            // Bounds.dDen60High is the observed fluids 60 degree relative density High
            // Bounds.dDenXHigh is the relative density at the observed temperature High

            // If the relative density is greater than the reference fluid "2" relative density
            // ...at the observed temperature
            if (dCorrectedRelativeDensity > Bounds.dDenXHigh)
            {
                piFlag = -1;
                return;
            } // End if (dCorrectedRelativeDensity > dObservedDen[1])

            // Bounds.dDen60Low is the observed fluids 60 degree relative density Low
            // Bounds.dDenXLow is the relative density at the observed temperature Low

            if (0.0 == Bounds.dDenXLow)
            {
                double dFluidReducedTempLow = 0.0;
                dFluidReducedTempLow = ((dTempInK - pFluid1.dCriticalTempInK) / (pFluid2.dCriticalTempInK - pFluid1.dCriticalTempInK))
                                                        * (Bounds.dDen60High - Bounds.dDen60Low) + Bounds.dDen60Low;

                if (0.35 > dFluidReducedTempLow)
                {
                    dFluidReducedTempLow = 0.35;

                } // End if (0.35 > dFluidReducedTempLow)

                Bounds.dDen60Low = dFluidReducedTempLow;

                double dVcf = 0.0;
                int iTempFlag = 0;

                base.CalculateCTL(ref pFluid2, ref pFluid1, ref Bounds.dDen60Low, ref dTempInK, ref iTempFlag, ref dVcf);

                Bounds.dDenXLow = dVcf * Bounds.dDen60Low;

            } // End if (dFluidReducedTempLow > 1.0)

            // Loop to determine the relative density
            for (iLoop = 0; iLoop < MAX_ITERATIONS; iLoop++)
            {
                // STEP 8

                // If Bounds.dDenXLow is valid
                if (dCorrectedRelativeDensity >= Bounds.dDenXLow)
                {
                    double dInterpolatingVar = (dCorrectedRelativeDensity - Bounds.dDenXLow) / (Bounds.dDenXHigh - Bounds.dDenXLow);
                    if (0.001 > dInterpolatingVar)
                    {
                        dInterpolatingVar = 0.001;

                    } // End if (0.001 > dInterpolatingVar)
                    else if (0.999 < dInterpolatingVar)
                    {
                        dInterpolatingVar = 0.999;

                    } // End if (0.999 < dInterpolatingVar)

                    Bounds.dDen60Mid = Bounds.dDen60Low + dInterpolatingVar
                                 * (Bounds.dDen60High - Bounds.dDen60Low);

                } // End if (dCorrectedRelativeDensity >= Bounds.dDenXLow)
                else // If Bounds.dDenXLow is not valid
                {
                    Bounds.dDen60Mid = (Bounds.dDen60High + Bounds.dDen60Low) / 2.0;

                } // End else If Bounds.dDenXLow is not valid

                double dVcf = 0.0;
                int iTempFlag = 0;

                base.CalculateCTL(ref pFluid2, ref pFluid1, ref Bounds.dDen60Mid, ref dTempInK, ref iTempFlag, ref dVcf);

                Bounds.dDenXMid = dVcf * Bounds.dDen60Mid;

                // STEP 9

                if (dCorrectedRelativeDensity >= Bounds.dDenXLow
                    && dCorrectedRelativeDensity >= Bounds.dDenXMid)
                {
                    dDifference = Math.Abs(Bounds.dDen60Low - Bounds.dDen60Mid);
                    if (MAX_DIFFERENCE > dDifference)
                    {
                        dVcfc = dVcf;
                        dDensity = Bounds.dDen60Mid;
                        break;

                    } // End if (MAX_DIFFERENCE > dDifference)

                    dDifference = Math.Abs(Bounds.dDen60Mid - Bounds.dDen60High);
                    if (MAX_DIFFERENCE > dDifference)
                    {
                        dVcfc = dVcf;
                        dDensity = Bounds.dDen60Mid;
                        break;

                    } // End if (MAX_DIFFERENCE > dDifference)

                } // End if dCorrectedRelativeDensity

                // STEP 10

                double dAlpha = Bounds.dDen60High - Bounds.dDen60Low;

                double dBeta = Math.Pow(Bounds.dDenXHigh, 2) - Math.Pow(Bounds.dDenXLow, 2);

                /*		for (int i=1; i<15; i++)
                        {
                            double dHigh = 0.674300900334;
                            double dMid = 0.674295574123;
                            double dLow = 0.668992076725;
                            RoundDouble(dHigh,&dHigh,i,FALSE,FALSE);
                            RoundDouble(dMid,&dMid,i,FALSE,FALSE);
                            RoundDouble(dLow,&dLow,i,FALSE,FALSE);
                            double dPhiTest = (dHigh - dLow) / (dMid - dLow);
                            TCHAR szMessage[50];
                            szMessage[0] = NULL;
                            swprintf(szMessage,_T("Phi with rounding to %d digits = %e \n"),i,dPhiTest);
                            TRACE(szMessage);
                        }
                */
                double dPhi = (Bounds.dDenXHigh - Bounds.dDenXLow) / (Bounds.dDenXMid - Bounds.dDenXLow);

                double dA = (dAlpha - dPhi * (Bounds.dDen60Mid - Bounds.dDen60Low))
                             / (dBeta - dPhi * (Math.Pow(Bounds.dDenXMid, 2) - Math.Pow(Bounds.dDenXLow, 2)));

                double dB = (dAlpha - dA * dBeta) / (Bounds.dDenXHigh - Bounds.dDenXLow);

                double dC = Bounds.dDen60Low - dB * Bounds.dDenXLow - dA * Math.Pow(Bounds.dDenXLow, 2);

                Bounds.dDen60Trial = dA * Math.Pow(dCorrectedRelativeDensity, 2) + dB * dCorrectedRelativeDensity + dC;

                if (Bounds.dDen60Trial < Bounds.dDen60Low)
                {
                    Bounds.dDen60Trial = Bounds.dDen60Low + ((Bounds.dDen60Mid - Bounds.dDen60Low)
                                                * (dCorrectedRelativeDensity - Bounds.dDenXLow))
                                                / (Bounds.dDenXMid - Bounds.dDenXLow);
                } // End if (Bounds.dDen60Trial < Bounds.dDen60Low)
                else if (Bounds.dDen60Trial > Bounds.dDen60High)
                {
                    Bounds.dDen60Trial = Bounds.dDen60Mid + ((Bounds.dDen60High - Bounds.dDen60Mid)
                                      * (dCorrectedRelativeDensity - Bounds.dDen60Mid)) / (Bounds.dDenXHigh - Bounds.dDen60Mid);
                } // End else if (Bounds.dDen60Trial > Bounds.dDen60High)

                base.CalculateCTL(ref pFluid2, ref pFluid1, ref Bounds.dDen60Trial, ref dTempInK, ref iTempFlag, ref dVcf);

                Bounds.dDenXTrial = Bounds.dDen60Trial * dVcf;

                // STEP 11

                dDifference = Math.Abs(dCorrectedRelativeDensity - Bounds.dDenXTrial);
                if (MAX_DIFFERENCE > dDifference)
                {
                    dVcfc = dVcf;
                    dDensity = Bounds.dDen60Mid;
                    break;

                } // End if (MAX_DIFFERENCE > dDifference)

                // STEP 12

                if (Bounds.dDenXTrial > dCorrectedRelativeDensity)
                {
                    Bounds.dDenXHigh = Bounds.dDenXTrial;
                    Bounds.dDen60High = Bounds.dDen60Trial;

                    if (Bounds.dDenXMid < dCorrectedRelativeDensity)
                    {
                        Bounds.dDenXLow = Bounds.dDenXMid;
                        Bounds.dDen60Low = Bounds.dDen60Mid;

                    } // End if (Bounds.dDenXMid < dCorrectedRelativeDensity)

                } // End if (Bounds.dDenXTrial > dCorrectedRelativeDensity)
                else // We already know it is not equal from STEP 11
                {
                    Bounds.dDenXLow = Bounds.dDenXTrial;
                    Bounds.dDen60Low = Bounds.dDen60Trial;

                    if (Bounds.dDenXMid > dCorrectedRelativeDensity)
                    {
                        Bounds.dDenXHigh = Bounds.dDenXMid;
                        Bounds.dDen60High = Bounds.dDen60Mid;

                    } // End if (Bounds.dDenXMid > dCorrectedRelativeDensity)

                } // End else We already know it is not equal from STEP 11

            } // End of iLoop

            // STEP 13

            if (bRound)
            {
                RoundDouble(dDensity, ref dDensity, 4, false, false);
                RoundDouble(dVcfc, ref dVcfc, 5, false, false);
            }

            if (dDensity < 0.35
                || dDensity > 0.688)
            {
                pdVcfc = 0.0;
                piFlag = -1;
            }
            else
            {
                pdVcfc = dVcfc;
                piFlag = 0;
            }

        } // End of APICorrection()

    }
}
