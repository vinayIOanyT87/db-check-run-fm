using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{

    public abstract class TankBaseVcf
    {

        #region enums
 

        protected enum EMassCalc
        {
            NETWEIGHT_MASS = 0,// Weight in a vacuum
            NETWEIGHT_WEIGHT = 1,// Weight in air
            NETWEIGHT_PRESSURE = 2,// Weight in a pressurized tank
            NETWEIGHT_MOLAR_METHOD = 3,// Special weight calculation for Japan
            NETWEIGHT_FOODOIL = 4,// Special weight calculation for the japanese food oil industry
            NETWEIGHT_TABLE52 = 5,// Mass calculation using table 52
            NETWEIGHT_TABLE56 = 6,// Mass calculation using table 56
            NETWEIGHT_TABLE57 = 7,// Mass calculation using table 57
            GBT_MASSCALC_1 = 8,// Mass calculation chinese 1
            GBT_MASSCALC_2 = 9// Mass calculation chinese 9
        }

        protected enum ERoofType
        {
            // Define Tank Roof Types
            ROOF_NONE = 0,			// No Roof Defined
            ROOF_FIXED = 1,			// Fixed Roof Tank
            ROOF_IN_STRAP = 2,			// Floating Roof Included In Strap Table
            ROOF_NOT_IN_STRAP = 3,	// Floating Roof Not Included In Strap
            ROOF_KAIJI_KYYOKAI = 4,
            ROOF_KENTEI_KYYOKAI1 = 5,
            ROOF_KENTEI_KYYOKAI2 = 6,
            ROOF_KENTEI_KYYOKAI3 = 7
        }

        protected enum EVolSubtract
        {
            VOL_SUBTRACT_NONE = 0,
            VOL_SUBTRACT_GROSS = 1,
            VOL_SUBTRACT_NET = 2
        }

        public enum ETempRounding
        {
            // Temperature Rounding definitions
            TEMP_ROUNDING_NONE = 0,
            TEMP_ROUNDING_025 = 1,
            TEMP_ROUNDING_050 = 2
        }

        protected enum ELevelRounding
        {
            LEVEL_ROUNDING_NONE = 0,
            LEVEL_ROUNDING_INTEGER = 1
        }

        public enum EVcfRounding
        {
            VCF_ROUNDING_NONE = 0,
            VCF_ROUNDING_0001 = 1,
            VCF_ROUNDING_000001 = 2,
            VCF_ROUNDING_00001 = 3
        }

        public enum EApiCalc
        {
            // define the calculation types
            API_CALC_STANDARD = 0,
            API_CALC_JAPANEESE = 1,
            API_CALC_GBT = 2
        }

        #endregion

        #region consts

        //// Table 54 reference temperatures implemented
        protected const double TAB54_DEF_REF_TEMP = 15.0f;	// default reference temp is 15 DegC
        protected const double TAB54_30_REF_TEMP = 30.0f;	// 30 DegC ref temp
        protected const double TAB59_DEF_REF_TEMP = 20.0f;

        protected const int MAX_ITERATIONS = 10;
        protected const double MAX_DENSITY_DIFF = 0.05;

        protected const double LOWEST_FTEMP = -60.0f;
        protected const double LOWEST_CTEMP = -60.00f;

        public const int SET_APICORR_ALARM = -1;

        #endregion

        #region members
        public bool m_bUsesDensity;
        protected bool m_bUseApi1980 = false;
        protected EApiCalc m_bStandardCalculationType;
        protected Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor m_byCorrectionTypeMajor;
        protected Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor m_byCorrectionTypeMinor;
        protected bool m_dwHydro = true;
        protected bool m_dwFrenchWM;
        protected bool m_dwJapanWM;
        protected bool dwForcetoFourDigits = false;
        #endregion

        #region attributes

        public Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor CorrectionTypeMajor
        {
            get { return m_byCorrectionTypeMajor; }
        }

        public Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor CorrectionTypeMinor
        {
            get { return m_byCorrectionTypeMinor; }
        }

        public bool FrenchWM
        {
            get { return m_dwFrenchWM; }
            set { m_dwFrenchWM = value; }
        }

        public bool JapanWM
        {
            get { return m_dwJapanWM; }
            set { m_dwJapanWM = value; }
        }

        public bool UseApi1980
        {
            get { return m_bUseApi1980; }
        }

        public bool DoHydroCorrection
        {
            get { return m_dwHydro; }
            set { m_dwHydro = value; }
        }

        public bool ForcetoFourDigits
        {
            get
            {
                return dwForcetoFourDigits;
            }
            set
            {
                dwForcetoFourDigits = value;
            }
        }

        #endregion

        #region protected methods

        protected bool RoundDouble(double dInputVariable,
                            ref double dOutputVariable,
                            byte bPrecision,
                            bool bTruncate,
                            bool bRndToFive)
        {
            // dInputVariable		- double to round
            // dOutputVariable	- Rounded double
            // bPrecision			- number of Decimal points to round to
            // there are two ways we could accomplish this, one is to convert to a string
            // and then convert back. however string manipulation is slow so we will do this
            // with number manipulation
            byte bDecPlacesFactor = 0;
            double dRndFactor;
            double dRnd;
            double dDivFactor;

            if (dOutputVariable == 0.00 ||
                (bTruncate && bRndToFive))
                return (false);

            if (true == bTruncate)
            {
                dRndFactor = 0.0;
                dDivFactor = 1.0;
            }
            else
            {
                if (true == bRndToFive)
                {
                    dRndFactor = 25.0;
                    dDivFactor = 50.0;
                    bDecPlacesFactor = 1;
                }
                else
                {
                    dRndFactor = 0.5;
                    dDivFactor = 1.0;
                }
            }

            if (dInputVariable >= 0)
            {
                dRnd = Math.Floor((dInputVariable * (Math.Pow(10.0, (bPrecision + bDecPlacesFactor))) + dRndFactor) / dDivFactor);
                dOutputVariable = (dRnd * dDivFactor) / Math.Pow(10.0, (bPrecision + bDecPlacesFactor));
            }
            else
            {
                dRnd = Math.Ceiling((dInputVariable * (Math.Pow(10.0, (bPrecision + bDecPlacesFactor))) - dRndFactor) / dDivFactor);
                dOutputVariable = (dRnd * dDivFactor) / Math.Pow(10.0, (bPrecision + bDecPlacesFactor));
            }

            return (true);
        }

        protected void ALPHA(double dRho,			// input ---- Density at 15 C 
                         double dK0,			// input ---- K0 parameter
                                 double dK1,			// input ---- K1 parameter
                                 ref double pdAlf)		// output --- ALPHA factor
        {

            double dTerm1 = 0.00, dTerm2 = 0.00, dTerm3 = 0.00;

            dTerm1 = dK0 / dRho;
            RoundDouble(dTerm1, ref dTerm1, 8, true, false);

            dTerm2 = dTerm1 / dRho;
            RoundDouble(dTerm2, ref dTerm2, 10, true, false);

            dTerm3 = dK1 / dRho;
            RoundDouble(dTerm3, ref dTerm3, 10, true, false);

            pdAlf = dTerm2 + dTerm3;
            RoundDouble(pdAlf, ref pdAlf, 7, false, false);

        }

        protected void VCF(
            double dAlf, // input ---- ALPHA factor
            double dDt, // input ---- DELTA factor ( temperature differential )
            ref double pdVcf, // output --- calculated VCF
            double dDegC // input ---- Reference temp to correct to , in DegC
            )
        {
            VCF(dAlf,dDt,ref pdVcf,dDegC,true);
        }

        protected void VCF(double dAlf,	// input ---- ALPHA factor
                        double dDt,		// input ---- DELTA factor ( temperature differential )
                        ref double pdVcf,			// output --- calculated VCF
                        double dDegC,			// input ---- Reference temp to correct to , in DegC
                        bool bRndValue)
        {
            double dTerm1, dTerm2, dTerm3, dTerm4;
            bool APIF = false;

            if (m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F ||
                m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F_1980)
                APIF = true;
            /*
             * calculate alpha * dt
            */
            dTerm1 = dAlf * dDt;
            if (APIF)
                RoundDouble(dTerm1, ref dTerm1, 8, true, false);	// 8dps - truncate to nearest 0.1
            else
                RoundDouble(dTerm1, ref dTerm1, 9, true, false);	// 9dps - truncate to nearest 0.1
            /*
            * calculate 0.8 * alpha * dt
            */
            dTerm2 = dTerm1 / 5 * 4;
            if (APIF)
                RoundDouble(dTerm2, ref dTerm2, 8, true, false);	// 8dps - truncate to nearest 0.1
            else
                RoundDouble(dTerm2, ref dTerm2, 9, true, false);	// 9dps - truncate to nearest 0.1

            /*
            * calculate 0.8 * alpha *alpha * dt *dt
            */
            dTerm3 = dTerm1 * dTerm2;
            if (APIF)
                RoundDouble(dTerm3, ref dTerm3, 8, false, false);	// 8dps - rounding to nearest 0.1
            else
                RoundDouble(dTerm3, ref dTerm3, 9, false, false);	// 9dps - rounding to nearest 0.1

            /*
            * calculate full term
            */
            dTerm4 = -(dTerm1 + dTerm3);
            RoundDouble(dTerm4, ref dTerm4, 8, true, false);	// 8dps - truncate to nearest 0.1

            /*
            * add calculation for temperature other than 15
            */
            if (TAB54_DEF_REF_TEMP != dDegC)
            {
                double dTerm1prime, dTerm2prime, dTerm3prime, dTerm4prime;
                /*
                 * calculate alpha * dt
                */
                dTerm1prime = dAlf * (dDegC - TAB54_DEF_REF_TEMP);
                if (APIF)
                    RoundDouble(dTerm1prime, ref dTerm1prime, 8, true, false);	// 8dps - truncate to nearest 0.1
                else
                    RoundDouble(dTerm1prime, ref dTerm1prime, 9, true, false);	// 9dps - truncate to nearest 0.1

                /*
                * calculate 0.8 * alpha * dt
                */
                dTerm2prime = dTerm1prime / 5 * 4;
                if (APIF)
                    RoundDouble(dTerm2prime, ref dTerm2prime, 8, true, false);	// 8dps - truncate to nearest 0.1
                else
                    RoundDouble(dTerm2prime, ref dTerm2prime, 9, true, false);	// 9dps - truncate to nearest 0.1
                /*
                * calculate 0.8 * alpha *alpha * dt *dt
                */
                dTerm3prime = dTerm1prime * dTerm2prime;
                if (APIF)
                    RoundDouble(dTerm3prime, ref dTerm3prime, 8, true, true);	// 8dps - rounding to nearest 0.1
                else
                    RoundDouble(dTerm3prime, ref dTerm3prime, 9, true, true);	// 9dps - rounding to nearest 0.1
                /*
                * calculate full term
                */
                dTerm4prime = dTerm1prime + dTerm3prime;
                RoundDouble(dTerm4prime, ref dTerm4prime, 8, true, false);	// 8dps - truncate to nearest 0.1

                dTerm4 += dTerm4prime;
            }

            /*
            * calc exponential 
            */
            pdVcf = Math.Exp(dTerm4);
            double dVCFTemp = 0, dTerm4Temp;
            long lTerm1, lTerm2, lTerm3;

            lTerm1 = (long)(dTerm1 * 100000000);
            lTerm2 = (long)(dTerm2 * 100000000);
            lTerm3 = (long)(dTerm3 * 100000000);

            MPY(lTerm1, lTerm2, ref lTerm3);

            dTerm4Temp = (double)((double)(lTerm1 + lTerm3) / 100000000.0);

            dTerm4Temp *= -1;

            dVCFTemp = CalcVCFExp(dTerm4Temp);
            if (bRndValue)
                RoundDouble(pdVcf, ref pdVcf, 8, false, false);
        }

        protected double CalcVCFExp(double dIX)
        {
            long dSum1 = 0, dSum2 = 0, dSum3 = 0, dSum4 = 0, dSum5 = 0, dSum6 = 0, lIX = 0;
            double dVcf = 0;
            lIX = (long)(dIX * 100000000);

            dSum1 = (long)(100000000 + lIX);

            MPY(lIX, lIX, ref dSum2);
            dSum2 = dSum2 / 2;

            MPY(lIX, dSum2, ref dSum3);
            dSum3 = dSum3 / 3;

            MPY(lIX, dSum3, ref dSum4);
            dSum4 = dSum4 / 4;

            MPY(lIX, dSum4, ref dSum5);
            dSum5 = dSum5 / 5;

            MPY(lIX, dSum5, ref dSum6);
            dSum6 = dSum6 / 6;

            dVcf = (dSum1 + dSum2 + dSum3 + dSum4 + dSum5 + dSum6) / 100000000.0;

            return (dVcf);
        }

        protected void MPY(long dIX, long dIY, ref long dIZ)
        {
            double dIU1, dK1, dK2, dIV2, dK3, dIV1, dIU2;

            dIU1 = dIX / 10000;
            dK1 = 10000 * dIU1;
            dIV1 = dIX - dK1;
            dIU2 = dIY / 10000;
            dK2 = 10000 * dIU2;
            dIV2 = dIY - dK2;
            dK3 = (dIU1 * dIV2) + (dIU2 * dIV1) + (dIV1 * (dIV2 / 10000));


            dIZ = (long)((dK3 + 5000) / 10000 + (dIU1 * dIU2));
        }

        protected void CheckStdDenAgstTable(double dStdDensity,
                                                        ref double dHiStdDensity,
                                                        ref double dLoStdDensity,
                                                        ref bool lpbBetweenTable,
                                                        ref bool lpbExactMidpoint)
        {
            int iStdDenisy = (int)(dStdDensity * 10);
            int iLast2Digits = 0;

            lpbBetweenTable = true;
            lpbExactMidpoint = false;

            //Check between table value or not
            if (iStdDenisy % 20 == 0)
                lpbBetweenTable = false;
            else
            {
                iLast2Digits = iStdDenisy - (iStdDenisy / 100 * 100);

                // Check exact mid point of table values
                if ((iLast2Digits % 10 == 0) && ((iLast2Digits / 10) % 2 != 0))
                    lpbExactMidpoint = true;

                if (0 < iLast2Digits && iLast2Digits < 20)
                {
                    dLoStdDensity = ((double)(iStdDenisy - iLast2Digits)) / 10.0;
                    dHiStdDensity = ((double)(iStdDenisy - iLast2Digits + 20)) / 10.0;
                }
                else if (20 < iLast2Digits && iLast2Digits < 40)
                {
                    dLoStdDensity = ((double)(iStdDenisy - iLast2Digits + 20)) / 10.0;
                    dHiStdDensity = ((double)(iStdDenisy - iLast2Digits + 40)) / 10.0;
                }
                else if (40 < iLast2Digits && iLast2Digits < 60)
                {
                    dLoStdDensity = ((double)(iStdDenisy - iLast2Digits + 40)) / 10.0;
                    dHiStdDensity = ((double)(iStdDenisy - iLast2Digits + 60)) / 10.0;
                }
                else if (60 < iLast2Digits && iLast2Digits < 80)
                {
                    dLoStdDensity = ((double)(iStdDenisy - iLast2Digits + 60)) / 10.0;
                    dHiStdDensity = ((double)(iStdDenisy - iLast2Digits + 80)) / 10.0;
                }
                else if (80 < iLast2Digits && iLast2Digits < 100)
                {
                    dLoStdDensity = ((double)(iStdDenisy - iLast2Digits + 80)) / 10.0;
                    dHiStdDensity = ((double)(iStdDenisy - iLast2Digits + 100)) / 10.0;
                }
            }
            return;
        }

        protected bool TemperatureRound_Japan(ETempRounding byMethod, double dTemp, ref double pdRoundedTemp)
        {
            double dIntegerPart = 0.0;
            double dFractionalPart = 0.0;
            bool hr = true;

            if (m_bStandardCalculationType == EApiCalc.API_CALC_STANDARD ||
                m_bStandardCalculationType == EApiCalc.API_CALC_GBT)
            {
                pdRoundedTemp = dTemp;
            }
            else
            {
                switch (byMethod)
                {
                    case ETempRounding.TEMP_ROUNDING_NONE:
                        pdRoundedTemp = dTemp;
                        break;
                    case ETempRounding.TEMP_ROUNDING_025:
                        RoundDouble(dTemp, ref dIntegerPart, 0, true, false);
                        dFractionalPart = dTemp - dIntegerPart;
                        if (dFractionalPart <= -0.875)
                            pdRoundedTemp = dIntegerPart - 1.00;
                        else if (dFractionalPart <= -0.625)
                            pdRoundedTemp = dIntegerPart - 0.75;
                        else if (dFractionalPart <= -0.375)
                            pdRoundedTemp = dIntegerPart - 0.5;
                        else if (dFractionalPart <= -0.125)
                            pdRoundedTemp = dIntegerPart - 0.25;
                        else if (dFractionalPart < 0.125)
                            pdRoundedTemp = dIntegerPart;
                        else if (dFractionalPart < 0.375)
                            pdRoundedTemp = dIntegerPart + 0.25;
                        else if (dFractionalPart < 0.625)
                            pdRoundedTemp = dIntegerPart + 0.5;
                        else if (dFractionalPart < 0.875)
                            pdRoundedTemp = dIntegerPart + 0.75;
                        else if (dFractionalPart < 1.000)
                            pdRoundedTemp = dIntegerPart + 1.00;
                        else
                            hr = false;
                        break;
                    case ETempRounding.TEMP_ROUNDING_050:
                        RoundDouble(dTemp, ref dIntegerPart, 0, true, false);
                        dFractionalPart = dTemp - dIntegerPart;
                        if (dFractionalPart <= -0.800)
                            pdRoundedTemp = dIntegerPart - 1.0;
                        else if (dFractionalPart <= -0.300)
                            pdRoundedTemp = dIntegerPart - 0.5;
                        else if (dFractionalPart < 0.300)
                            pdRoundedTemp = dIntegerPart;
                        else if (dFractionalPart < 0.800)
                            pdRoundedTemp = dIntegerPart + 0.5;
                        else if (dFractionalPart < 1.000)
                            pdRoundedTemp = dIntegerPart + 1.0;
                        else
                            hr = false;
                        break;
                    default:
                        hr = false;
                        break;
                } // End of switch(byMethod)

            } // End else

            return hr;

        } // End of TemperatureRound_Japan()

        protected void ApplyHydroCorrection(ref double pdDens,		// input/output --- density in kg/m3
                                                      double dTemp,			// input - current tempertaure
                                                      Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor byAPITableMajor,	// input - correction table in use
                                                      Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor byAPITableMinor,	// input - correction table in use
                                                      bool bRemoveCorrection,
                                                      ref double dHydrometer)
        {
            double dDeltaT = 0.00, dTerm1 = 0.00, dTerm2 = 0.00, dHY = 1.00;

            // apply hydro correction if value = 1
            dHydrometer = 1.0;
            if (DoHydroCorrection)
            {
                switch (byAPITableMajor)
                {
                    case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F:
                    case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F_1980:
                        {
                            switch (byAPITableMinor)
                            {
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6A:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6B:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6D:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API24E:

                                    // Compute Hydrometer correction factors
                                    dDeltaT = dTemp - 60.0;
                                    dTerm1 = 0.00001278 * dDeltaT;
                                    dTerm2 = 0.0000000062 * (dDeltaT * dDeltaT);

                                    RoundDouble(dTerm1, ref dTerm1, 9, false, false);
                                    RoundDouble(dTerm2, ref dTerm2, 9, false, false);

                                    dHY = 1.0 - dTerm1 - dTerm2;
                                    RoundDouble(dHY, ref dHY, 9, false, false);
                                    break;

                                default:		// default case - DO NOT APPLY HYDRO CORR.
                                    dHY = 1.0;
                                    break;
                            } // End switch (byAPITableMinor)
                            break;

                        } // End case CORR_60_F:

                    case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C:
                    case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C_1980:
                    case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_JIS_2249:
                    case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_ASTM_D1250:
                        {
                            switch (byAPITableMinor)
                            {
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D:
                                    // Compute Hydrometer correction factors
                                    dDeltaT = dTemp - 15.0;
                                    dTerm1 = 0.000023 * dDeltaT;
                                    dTerm2 = 0.00000002 * (dDeltaT * dDeltaT);

                                    RoundDouble(dTerm1, ref dTerm1, 9, false, false);
                                    RoundDouble(dTerm2, ref dTerm2, 9, false, false);

                                    dHY = 1.0 - dTerm1 - dTerm2;
                                    RoundDouble(dHY, ref dHY, 9, false, false);
                                    break;

                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60A:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60B:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60D:

                                    // Compute Hydrometer correction factors
                                    dDeltaT = dTemp - 20.0;
                                    dTerm1 = 0.000023 * dDeltaT;
                                    dTerm2 = 0.00000002 * (dDeltaT * dDeltaT);

                                    RoundDouble(dTerm1, ref dTerm1, 9, false, false);
                                    RoundDouble(dTerm2, ref dTerm2, 9, false, false);

                                    dHY = 1.0 - dTerm1 - dTerm2;
                                    RoundDouble(dHY, ref dHY, 9, false, false);
                                    break;

                                default:		// default case - DO NOT APPLY HYDRO CORR.
                                    dHY = 1.0;
                                    break;
                            } // End switch (byAPITableMinor)
                            break;
                        } // End case CORR_15_C:
                    case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_JAPAN_JIS_2249_TABLE:
                        {
                            switch (byAPITableMinor)
                            {
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_TABLE:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B_TABLE:
                                case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_TABLE:
                                    // Compute Hydrometer correction factors
                                    dDeltaT = dTemp - 15.0;
                                    dTerm1 = 0.000023 * dDeltaT;
                                    dTerm2 = 0.00000002 * (dDeltaT * dDeltaT);

                                    RoundDouble(dTerm1, ref dTerm1, 9, false, false);
                                    RoundDouble(dTerm2, ref dTerm2, 9, false, false);

                                    dHY = 1.0 - dTerm1 - dTerm2;
                                    RoundDouble(dHY, ref dHY, 9, false, false);
                                    break;
                            }
                            break;
                        }
                    default:
                        dHY = 1.0;
                        break;

                } // End switch (byAPITableMajor)

                // Apply hydrometer correction
                if (bRemoveCorrection)
                    pdDens = pdDens / dHY;
                else
                    pdDens = pdDens * dHY;

                dHydrometer = dHY;

            } // End if (m_dwHydro == 1)

        } // End ApplyHydroCorrection() function

        protected void RHO(double dApi, ref double pdRho)
        {
            double dDenom;

            dDenom = dApi + 131.5;
            RoundDouble(dDenom, ref dDenom, 1, true, false);

            pdRho = 141360.1980 / dDenom;
            RoundDouble(pdRho, ref pdRho, 2, false, false);

        }

        protected bool VcfRound_Japan(EVcfRounding byMethod, double dVcf, ref double pdVcf)
        {
            bool hr = true;

            if (m_bStandardCalculationType == EApiCalc.API_CALC_STANDARD ||
                m_bStandardCalculationType == EApiCalc.API_CALC_GBT)
            {
                pdVcf = dVcf;
            }
            else
            {

                switch (byMethod)
                {
                    case EVcfRounding.VCF_ROUNDING_NONE:
                        pdVcf = dVcf;
                        break;

                    case EVcfRounding.VCF_ROUNDING_0001:
                        RoundDouble(dVcf, ref pdVcf, 4, false, false);
                        break;

                    case EVcfRounding.VCF_ROUNDING_00001:
                        RoundDouble(dVcf, ref pdVcf, 5, false, false);
                        break;

                    case EVcfRounding.VCF_ROUNDING_000001:
                        RoundDouble(dVcf, ref pdVcf, 6, false, false);
                        break;
                    default:
                        hr = false;
                        break;
                } // End switch(byMethod)
            }
            return hr;
        } // End of VcfRound_Japan()


        #endregion

        #region private methods

        private void GrossVolume(bool bOutageTank,
                                         double dLevel,
                                         double dPinHeight,
                                         double dStrapVolInM3,
                                         double dCorrectionVolInM3,
                                         ERoofType byRoofType,
                                         EVolSubtract byWaterSubtractMethod,
                                         EVolSubtract byVSWSubtractMethod,
                                         double dRoofVolInM3,
                                         double dBottomsVolInM3,
                                         double dPercentBsw,
                                         double dShellCorr,
                                         double dHydrostaticCorr,
                                         double dWaterVolume,
                                         ref double pdGrossVol)
        {
            double dBswVolume = 0.0;

            if (m_bStandardCalculationType == EApiCalc.API_CALC_JAPANEESE)
            {
                if (EVolSubtract.VOL_SUBTRACT_NONE == byWaterSubtractMethod
                    || EVolSubtract.VOL_SUBTRACT_NET == byWaterSubtractMethod)
                {
                    dBottomsVolInM3 = 0.0;
                }

                if (EVolSubtract.VOL_SUBTRACT_NONE == byVSWSubtractMethod
                    || EVolSubtract.VOL_SUBTRACT_NET == byVSWSubtractMethod)
                {
                    dPercentBsw = 0.0;
                }

                if (dBottomsVolInM3 != 0.0) // Don't bother to truncate if 0.0
                {
                    // Truncate the bottoms volume in m3
                    // to 3 Decimal places (equals 0 decimal places in Liters)
                    RoundDouble(dBottomsVolInM3, ref dBottomsVolInM3, 3, true, false);
                }

                // If roof correction applied in net volume, set to zero for this calculation
                if (ERoofType.ROOF_KAIJI_KYYOKAI == byRoofType
                    || ERoofType.ROOF_KENTEI_KYYOKAI1 == byRoofType
                    || ERoofType.ROOF_KENTEI_KYYOKAI2 == byRoofType)
                {
                    dRoofVolInM3 = 0.0;
                }

                dShellCorr = 1.0;

            } // End of else if (m_bStandardCalculationType)
            if (m_bUseApi1980)
            {
                // Neither Bottoms Volume, Shell Correction, and Bsw are used
                dBottomsVolInM3 = 0.0;
                dShellCorr = 1.0;
                dPercentBsw = 0.0;
            }
            else // Use the current API standard
            {
                // Shell Correction and Bottoms Volume remain unchanged
                dPercentBsw = 0.0; // We do not use Bsw in Gross
                if (dShellCorr <= 0)
                    dShellCorr = 1.0;
            }


            // Calculate VSW (Volume of BSW)
            dBswVolume = (dStrapVolInM3 - dBottomsVolInM3) * dPercentBsw / 100;

            if (m_bStandardCalculationType == EApiCalc.API_CALC_GBT)
            {
                pdGrossVol = (dStrapVolInM3 + dHydrostaticCorr) - dWaterVolume;
            }
            else if ((!bOutageTank) && (dLevel <= dPinHeight))
            {
                pdGrossVol = ((dStrapVolInM3 - dBottomsVolInM3) * dShellCorr) - dBswVolume + dCorrectionVolInM3;
            }
            else if ((bOutageTank) && (dLevel >= dPinHeight))
            {
                pdGrossVol = ((dStrapVolInM3 - dBottomsVolInM3) * dShellCorr) - dBswVolume + dCorrectionVolInM3;
            }
            // Otherwise Compute Volume Due to Floating Roof
            else
            {
                pdGrossVol = ((dStrapVolInM3 - dBottomsVolInM3) * dShellCorr) - dRoofVolInM3 - dBswVolume + dCorrectionVolInM3;
            }

            if ((m_dwFrenchWM && m_bStandardCalculationType != 0) == (EApiCalc.API_CALC_STANDARD != 0))	// if french W&M option selected in registry
            {
                RoundDouble(pdGrossVol, ref pdGrossVol, 5, false, false);	// round GrossVolinM3 
                RoundDouble(pdGrossVol, ref pdGrossVol, 3, true, false);	// truncate GrossVolinM3 to nearest liter 
            }

        }

        private void NetVolume(EMassCalc byCalcMethod,
                                        double dGrossVolume,
                                        double dVcf,
                                        double dRoofCorrection,
                                        double dShellCorr,
                                        ERoofType byRoofType,
                                        EVolSubtract byWaterSubtractMethod,
                                        EVolSubtract byVSWSubtractMethod,
                                        double dBottomsVolume,
                                        double dPercentBsw,
                                        ref double pdNetVolume)
        {
            if (m_bStandardCalculationType == EApiCalc.API_CALC_JAPANEESE)
            {
                if (EVolSubtract.VOL_SUBTRACT_NONE == byWaterSubtractMethod
                    || EVolSubtract.VOL_SUBTRACT_GROSS == byWaterSubtractMethod)
                {
                    dBottomsVolume = 0.0;
                }

                if (EVolSubtract.VOL_SUBTRACT_NONE == byVSWSubtractMethod
                    || EVolSubtract.VOL_SUBTRACT_GROSS == byVSWSubtractMethod)
                {
                    dPercentBsw = 0.0;
                }

                if (ERoofType.ROOF_KAIJI_KYYOKAI == byRoofType
                    || ERoofType.ROOF_KENTEI_KYYOKAI2 == byRoofType)
                {
                    pdNetVolume = (((dGrossVolume - dBottomsVolume)
                                        * (1.0 - dPercentBsw / 100.0))
                                        * dVcf * dShellCorr) - dRoofCorrection;
                }
                else if (ERoofType.ROOF_KENTEI_KYYOKAI1 == byRoofType)
                {
                    pdNetVolume = ((((dGrossVolume - dBottomsVolume) * dShellCorr - dRoofCorrection)
                                        * (1.0 - dPercentBsw / 100.0))
                                        * dVcf);
                }
                else if (ERoofType.ROOF_KENTEI_KYYOKAI3 == byRoofType)
                {
                    pdNetVolume = (((dGrossVolume - dBottomsVolume)
                                        * (1.0 - dPercentBsw / 100.0))
                                        * dVcf * dShellCorr);
                }
                else
                {
                    pdNetVolume = (((dGrossVolume - dBottomsVolume)
                                        * (1.0 - dPercentBsw / 100.0))
                                        * dVcf * dShellCorr);
                }
            } // End of if (m_bStandardCalculationType)
            else if (m_bStandardCalculationType == EApiCalc.API_CALC_GBT)
            {
                switch (byCalcMethod)
                {
                    case EMassCalc.GBT_MASSCALC_2:
                        pdNetVolume = dGrossVolume * dVcf * dShellCorr - dRoofCorrection;
                        break;
                    default:
                        pdNetVolume = dGrossVolume * dVcf * dShellCorr;
                        break;
                }
                return;
            }
            else if (m_bUseApi1980)
            {
                pdNetVolume = (((dGrossVolume - dBottomsVolume)			// Calculate Net Volume
                                    * (1.0 - dPercentBsw / 100.0))						// Correction for BS & W
                                    * dVcf);												// Correct to Std Conditions
            }
            else
            {
                pdNetVolume = ((dGrossVolume										// Calculate Net Volume
                                    * (1.0 - dPercentBsw / 100.0))						// Correction for BS & W
                                    * dVcf);												// Correct to Std Conditions
            }

            if ((m_dwFrenchWM && m_bStandardCalculationType != 0) == (EApiCalc.API_CALC_STANDARD != 0))	// if french W&M option selected in registry ted in registry 
            {
                long iGrossVolume, iNetVolume, iBSW, iVCF;

                iGrossVolume = (long)dGrossVolume;		// perform netvolume calculation using inetger math for French
                iBSW = (long)(1 - dPercentBsw / 100) * 100;
                iVCF = (long)(dVcf * 100000);

                if ((double)iVCF - (dVcf * 100000) < -.010)	// fix up VCF value to compensate for double floating storage.
                    iVCF++;

                iNetVolume = iGrossVolume * iBSW * iVCF;
                iNetVolume /= 10000000;
                pdNetVolume = (double)iNetVolume;

                RoundDouble(pdNetVolume, ref pdNetVolume, 0, true, false);	// truncate NetVolinLiters to nearest liter       
            }

            if (m_dwJapanWM && m_dwFrenchWM)
            {
                long iGrossVolume, iNetVolume, iBSW, iVCF;

                iGrossVolume = (long)(dGrossVolume * 1000);
                if ((double)(iGrossVolume - (dGrossVolume * 1000)) < -.010)
                    iGrossVolume++;

                iBSW = (long)(1 - dPercentBsw / 100) * 100;
                iVCF = (long)(dVcf * 100000);

                if ((double)iVCF - (dVcf * 100000) < -.010)	// fix up VCF value to compensate for double floating storage.
                    iVCF++;

                iNetVolume = iGrossVolume * iBSW * iVCF;
                iNetVolume /= 10000000;
                pdNetVolume = (double)iNetVolume;
                RoundDouble(pdNetVolume, ref pdNetVolume, 0, true, false);	// truncate NetVolinLiters to nearest liter       
                pdNetVolume /= 1000;
            }

        } // End of NetVolume()

        private void GrossVolumeToStrapVolume(bool bOutageTank,
                                                            double dBsw,
                                                            double dPinHeightStrapVolInM3,
                                                            double dGrossVolInM3,
                                                            double dCorrectionVolInM3,
                                                            ERoofType byRoofType,
                                                            EVolSubtract byWaterSubtractMethod,
                                                            EVolSubtract byVSWSubtractMethod,
                                                            double dRoofVolInM3,
                                                            double dBottomsVolInM3,
                                                            double dShellCorr,
                                                            ref double pdStrapVolInM3)
        {
            double dBswVolume = 0.0;
            double dPercentBsw = 0.0;

            dPercentBsw = dBsw / 100.0;

            if (m_bStandardCalculationType == EApiCalc.API_CALC_JAPANEESE)
            {
                if (EVolSubtract.VOL_SUBTRACT_NONE == byWaterSubtractMethod
                    || EVolSubtract.VOL_SUBTRACT_NET == byWaterSubtractMethod)
                {
                    dBottomsVolInM3 = 0.0;
                }

                if (EVolSubtract.VOL_SUBTRACT_NONE == byVSWSubtractMethod
                    || EVolSubtract.VOL_SUBTRACT_NET == byVSWSubtractMethod)
                {
                    dPercentBsw = 0.0;
                }

                if (dBottomsVolInM3 != 0.0) // Don't bother to truncate if 0.0
                {
                    // Truncate the bottoms volume
                    RoundDouble(dBottomsVolInM3, ref dBottomsVolInM3, 0, true, false);
                }

                // If roof correction applied in net volume, set to zero for this calculation
                if (ERoofType.ROOF_KAIJI_KYYOKAI == byRoofType
                    || ERoofType.ROOF_KENTEI_KYYOKAI1 == byRoofType
                    || ERoofType.ROOF_KENTEI_KYYOKAI2 == byRoofType)
                {
                    dRoofVolInM3 = 0.0;
                }

                dShellCorr = 1.0;

            } // End of else if (m_bStandardCalculationType)
            else if (m_bUseApi1980)
            {
                // Neither Bottoms Volume, Shell Correction, and Bsw are used
                dBottomsVolInM3 = 0.0;
                dShellCorr = 1.0;
                dPercentBsw = 0.0;
            }
            else // Use the current API standard
            {
                // Shell Correction and Bottoms Volume remain unchanged
                dPercentBsw = 0.0; // We do not use Bsw in Gross
                if (dShellCorr <= 0)
                    dShellCorr = 1.0;
            }

            // Calculate VSW (Volume of BSW)
            dBswVolume = dGrossVolInM3 * dPercentBsw / 100;

            if ((!bOutageTank) && (dGrossVolInM3 <= dPinHeightStrapVolInM3))
            {
                pdStrapVolInM3 = ((dGrossVolInM3 + dBottomsVolInM3) / dShellCorr) + dBswVolume - dCorrectionVolInM3;
            }
            else if ((bOutageTank) && (dGrossVolInM3 >= dPinHeightStrapVolInM3))
            {
                pdStrapVolInM3 = ((dGrossVolInM3 + dBottomsVolInM3) / dShellCorr) + dBswVolume - dCorrectionVolInM3;
            }
            // Otherwise Compute Volume Due to Floating Roof
            else
            {
                pdStrapVolInM3 = ((dGrossVolInM3 + dBottomsVolInM3) / dShellCorr) + dRoofVolInM3 + dBswVolume - dCorrectionVolInM3;
            }

        } // End of GrossVolumeToStrapVolume()
        #endregion

        #region public methods

        public abstract bool TemperatureCorr(double dDensity,
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
                                    bool UseDensity);//	UseDensity

        public bool CalcTankProdVcf(double dMeasTemp,				// Measured Temperature
                                    double dStdTemp,				// Standard Temperature
                                    EngineeringUnit bStdTempUnits, 		// Standard Temp. Engineering Units
                                    EngineeringUnit bTempUnits, 			// Temp. Engineering Units
                                    ETempRounding byTempRoundingMethod,
                                    EVcfRounding byVcfRoundingMethod,
                                    double dMeasDensity,			// Measured Density
                                    EngineeringUnit bDensUnits, 			// Density Engineering Units
                                    double dStdDensity,			// Standard Product Density
                                    EngineeringUnit bStdDensUnits,			// Std. density Engineering Units
                                    bool UseDensity, 			// Use Measured Density in Calc
                                    double dDensityPress,			// density pressure for api 2004
                                    EngineeringUnit bDensityPressUnits,	// density pressure units for api 2004
                                    double dAlternateTemperature,	// alternate temp for api 2004
                                    double dBaseTemp,	// api 2004 alternate base temp reference
                                    double dAlternateBasePress, // api 2004 alternate base pressure reference
									ref double CTLReturn,
									ref double CPLReturn,
									ref double[] pdKfactors, 			// Pointer to K Factors Array
									ref double pdVcfCalc,               // Pnt to VCF for calculation purposes
									ref double dUnRoundedVcfCalc,
									ref double pdVcf)					// Pnt to Volume Correction Factor (rounded)
        {
            double dVcfc = 0.00, dStdTempInC = 0.00, dDensity = 0.00;
            double[] dK;
            int Iflag = 0;
            EngineeringUnit bDensityUnits = EngineeringUnit.FmdKgM3;
            bool hRes = false;

			CTLReturn = 0.0;
			CPLReturn = 0.0;

			hRes = TemperatureRound_Japan(byTempRoundingMethod, dMeasTemp, ref dMeasTemp);

            // when using alternate base parameters only calculation using observed density is valid
            if (m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004)
            {
                if (bTempUnits == EngineeringUnit.FmtDegF &&
                    (dBaseTemp != 60.0 ||
                    dAlternateBasePress != 0.0))
                    UseDensity = true;
                else if (bTempUnits == EngineeringUnit.FmtDegC &&
                    ((dBaseTemp != 15.0 &&
                    dBaseTemp != 20.0) ||
                    dAlternateBasePress != 0.0))
                    UseDensity = true;
            }
            // Select Density and Engr Units From Either Standard or Measured Density
            if (!UseDensity || m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1250_1952) // always use standard density for calculation
            {
                bDensityUnits = bStdDensUnits;
                dDensity = dStdDensity;
            }
            else
            {
                bDensityUnits = bDensUnits;
                dDensity = dMeasDensity;
            }

            dK = pdKfactors;

            // Convert Input Units to Standard Units Based on Correction Method

            if (!ConvertEngUnits.ConvEngrUnits(ref dStdTempInC, dStdTemp, EngineeringUnit.FmtDegC, bStdTempUnits, 0))
                return false;

            dVcfc = 1;
            Iflag = 0;

            // Call correction calculation using an UNROUNDED temperature for pdVcfCalc.
            // This unrounded VCF should only be used for leak detection -- it does not
            // meet the API definition because it uses higher precisions than the API specifies.
            TemperatureCorr(dDensity,
                            dMeasTemp,
                            dStdTempInC,
                            dStdTemp,
                            bDensityUnits,
                            bTempUnits,
                            dDensityPress,			// density pressure for api 2004
                            bDensityPressUnits,	// density pressure units for api 2004
                            dAlternateTemperature,	// alternate temp for api 2004
                            dBaseTemp,	// api 2004 alternate base temp reference
                            dAlternateBasePress,	// api 2004 alternate base pressure reference
                            ref dK,
                            ref dVcfc,
                            ref Iflag,
							ref CTLReturn,
							ref CPLReturn,
							true,
                            false,
                            false,
                            UseDensity);
            dUnRoundedVcfCalc = dVcfc;
            if (Iflag == -1)
            {
                if (dVcfc <= 0 || dVcfc > 3)
                    dVcfc = 1;
                pdVcf = dVcfc;
                pdVcfCalc = dVcfc;
                return (false);
            }

            // Now calculate the VCF as defined by API standards, applying all rounding
            dVcfc = 1;
            Iflag = 0;
            TemperatureCorr(dDensity,
                                    dMeasTemp,
                                    dStdTempInC,
                                    dStdTemp,
                                    bDensityUnits,
                                    bTempUnits,
                                    dDensityPress,			// density pressure for api 2004
                                    bDensityPressUnits,	// density pressure units for api 2004
                                    dAlternateTemperature,	// alternate temp for api 2004
                                    dBaseTemp,	// api 2004 alternate base temp reference
                                    dAlternateBasePress,	// api 2004 alternate base pressure reference
                                    ref dK,
                                    ref dVcfc,
                                    ref Iflag,
									ref CTLReturn,
									ref CPLReturn,
									true,
                                    true,
                                    false,
                                    UseDensity);

            // Call Conversion Method Based on Correction Type and Density Source
            // if Density or Alpha is outside the API routine's range, return the
            // API error status.
            // If VCF is negative, zero, or > 3, force volume correction factor to 1

            if (Iflag == -1)
            {
                if (dVcfc <= 0 || dVcfc > 3)
                    dVcfc = 1;
                pdVcf = dVcfc;
                pdVcfCalc = dVcfc;
                return (false);
            }
            else
                hRes = true;

            //	Round VCF to Fixed Decimal Places
            if (m_bStandardCalculationType == EApiCalc.API_CALC_JAPANEESE)
            {
                hRes = VcfRound_Japan(byVcfRoundingMethod, dVcfc, ref dVcfc);

            }
            else if ((m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_LPG_C &&	// 5 places for LPG
                m_byCorrectionTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_LPG) ||
                (m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F &&	// 5 places for table 23/24
                m_byCorrectionTypeMinor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API24E) ||
                m_bStandardCalculationType == EApiCalc.API_CALC_GBT ||
                m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004 ||
                m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_D1555_F_2009)
            {
                RoundDouble(dVcfc, ref dVcfc, 5, false, false);
            }
            else if (m_byCorrectionTypeMajor == Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004)
            {
            }
            else										// 5 places if greater than equal 1
            {											// 4 otherwise fo all other API
                if (ForcetoFourDigits)
                    RoundDouble(dVcfc, ref dVcfc, 4, false, false);
                else if (dVcfc >= 1.0)
                    RoundDouble(dVcfc, ref dVcfc, 4, false, false);
                else
                    RoundDouble(dVcfc, ref dVcfc, 5, false, false);
            }
            pdVcf = dVcfc;
            pdVcfCalc = dVcfc;

            return hRes;
        } // End of CalcTankProdVcf()
        #endregion

        public void VCF60(double dAlf,			// input ---- ALPHA factor
                         double dDt,            // input ---- DELTA factor ( temperature differential )
                                 ref double pdVcf)     // output --- calculated VCF
        {
            double dTerm1, dTerm2, dTerm3, dTerm4, dTerm5, dTerm6;

            /*
           * calculate alpha * dt
           */
            dTerm1 = dAlf * dDt;
            RoundDouble(dTerm1, ref dTerm1, 9, true, false);

            /*
            * calculate alpha * alpha * dt
            */
            dTerm2 = dTerm1 * dAlf;
            RoundDouble(dTerm2, ref dTerm2, 9, true, false);

            /*
            * calculate 8 * term2
            */
            dTerm3 = 8.0f * dTerm2;
            RoundDouble(dTerm3, ref dTerm3, 9, false, false);

            /*
            * calculate 0.8 * alpha * dt
            */
            dTerm4 = dTerm1 / 5 * 4;
            RoundDouble(dTerm4, ref dTerm4, 9, true, false);

            /*
            * calculate terms 5 and 6
            */
            dTerm5 = dTerm1 * dTerm4;
            RoundDouble(dTerm5, ref dTerm5, 9, false, false);

            dTerm6 = -(dTerm1 + dTerm3 + dTerm5);
            RoundDouble(dTerm6, ref dTerm6, 9, false, false);

            /*
            * calc exponential 
             */
            pdVcf = Math.Exp(dTerm6);
            RoundDouble(pdVcf, ref pdVcf, 6, false, false);
        }

		public virtual bool CalcTankStdDensity(EngineeringUnit bStdDensityUnits, // Std Density Engr Units
																double dTemp,               // Measured Temperature
																EngineeringUnit bTempUnits,        // Temperature Engr Units
																ETempRounding byTempRoundingMethod,
																EVcfRounding byVcfRoundingMethod,
																double dDensity,            // Measured Density
																EngineeringUnit bDensityUnits,     // Density Engr Units
																double dVolCorrFactor,  // Volume Correction Factor
																double dDensityPress,           // density pressure for api 2004
																EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
																double dAlternateTemperature,   // selected refined product sub catagory for api 2004
																double dBaseTemp,   // api 2004 alternate base temp reference
																double dAlternateBasePress, // api 2004 alternate base pressure reference
																ref double CTLReturn,
																ref double CPLReturn,
																ref double[] dK,
																ref double pdStdDensity,       // Standard Density Variable
																ref double dHydrometer)
		{
			double dStdDensity = 0.0, dTempInC = 0.0, dTempInF = 0.0;

			// First apply VCF to density and then convert to StdDensity units
			if (dVolCorrFactor <= 0)
				dVolCorrFactor = 1;

			bool hRes = TemperatureRound_Japan(byTempRoundingMethod, dTemp, ref dTemp);

			// Convert Temperature to Degress Centigrade
			ConvertEngUnits.ConvEngrUnits(ref dTempInC, dTemp, EngineeringUnit.FmtDegC, bTempUnits, 0);

			// Convert Temperature to Degress F
			ConvertEngUnits.ConvEngrUnits(ref dTempInF, dTemp, EngineeringUnit.FmtDegF, bTempUnits, 0);

			switch (m_byCorrectionTypeMajor)
			{
				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F:
				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F_1980:
					{
						switch (m_byCorrectionTypeMinor)
						{
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6A:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6B:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6D:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API24E:       // Added case for table 24E. (IGO 19-Aug-2004)

								// convert any denisty unit to API
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, 60.0);

								// Round to nearest 0.1 only if we are not calculating STD Density from Gauge Density
								if (EVcfRounding.VCF_ROUNDING_NONE != byVcfRoundingMethod)
								{
									RoundDouble(dDensity, ref dDensity, 1, false, false);
								}

								// convert any denisty unit to kg/m3
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdKgM3, EngineeringUnit.FmdDegApi, 60.0);

								// Round to nearest 0.01 only if we are not calculating STD Density from Gauge Density
								if (EVcfRounding.VCF_ROUNDING_NONE != byVcfRoundingMethod)
								{
									RoundDouble(dDensity, ref dDensity, 2, false, false);
								}

								// NOTE: ApplyHyrdoCorrection function checks the registry entry
								// ApplyHydroCorrection and will only apply if = 1
								ApplyHydroCorrection(ref dDensity, dTempInF, m_byCorrectionTypeMajor, m_byCorrectionTypeMinor, false, ref dHydrometer);

								break;

							default:
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, 60.0);
								break;
						}
						break;
					}

				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C:
				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C_1980:
					{
						switch (m_byCorrectionTypeMinor)
						{
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D:
								// convert any denisty unit to kg/m3
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, 60.0);

								// round to nearest 0.1 only if we are not calculating STD Density from Gauge Density
								if (EVcfRounding.VCF_ROUNDING_NONE != byVcfRoundingMethod)
								{
									RoundDouble(dDensity, ref dDensity, 1, false, false);
								}

								// apply hydrometer correction
								// NOTE: ApplyHyrdoCorrection function checks the registry entry
								// ApplyHydroCorrection and will only apply if = 1
								ApplyHydroCorrection(ref dDensity, dTempInC, m_byCorrectionTypeMajor, m_byCorrectionTypeMinor, false, ref dHydrometer);
								break;

							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60A:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60B:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60D:

								// convert any denisty unit to kg/m3
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, 60.0);

								// round to nearest 0.1 only if we are not calculating STD Density from Gauge Density
								if (EVcfRounding.VCF_ROUNDING_NONE != byVcfRoundingMethod)
								{
									RoundDouble(dDensity, ref dDensity, 1, false, true);
								}

								// apply hydrometer correction
								// NOTE: ApplyHyrdoCorrection function checks the registry entry
								// ApplyHydroCorrection and will only apply if = 1
								ApplyHydroCorrection(ref dDensity, dTempInC, m_byCorrectionTypeMajor, m_byCorrectionTypeMinor, false, ref dHydrometer);
								break;

							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_30:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B_30:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_30:

								// convert any denisty unit to kg/m3
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, 60.0);

								// round to nearest 0.1 only if we are not calculating STD Density from Gauge Density
								if (EVcfRounding.VCF_ROUNDING_NONE != byVcfRoundingMethod)
								{
									RoundDouble(dDensity, ref dDensity, 1, false, true);
								}

								// apply hydrometer correction
								// NOTE: ApplyHyrdoCorrection function checks the registry entry
								// ApplyHydroCorrection and will only apply if = 1
								ApplyHydroCorrection(ref dDensity, dTempInC, m_byCorrectionTypeMajor, m_byCorrectionTypeMinor, false, ref dHydrometer);
								break;

							default:
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, 60.0);
								break;
						}
						break;
					}

				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_GOST:
					{
						switch (m_byCorrectionTypeMinor)
						{
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_3900_85_20C:
								{
									//if (!StdDensityLookup(dDensity, dTemp, bDensityUnits, bTempUnits,
									//							 pdStdDensity))
										return false;
									//else
										//return true;

									//break;
								}
						}
						break;
					}

				default:

					ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdKgM3, bDensityUnits, 60.0);
					// round to nearest 0.1
					// RoundDouble(dDensity, &dDensity,1,FALSE,TRUE);

					break;
			}

			// round density to nearest 0.01 only if we are not calculating STD Density from Gauge Density
			if (EVcfRounding.VCF_ROUNDING_NONE != byVcfRoundingMethod)
			{
				RoundDouble(dDensity, ref dDensity, 2, false, false);
			}

			// Convert Density to Standard Conditions
			dStdDensity = dDensity / dVolCorrFactor;

			// round denisty result to nearest 0.1 only if we are not calculating STD Density from Gauge Density
			if (EVcfRounding.VCF_ROUNDING_NONE != byVcfRoundingMethod)
			{
				RoundDouble(dStdDensity, ref dStdDensity, 1, false, false);
			}

			// Convert from kg/m3 to Standard Density Units
			ConvertEngUnits.ConvEngrUnits(ref dStdDensity, dStdDensity, bStdDensityUnits,EngineeringUnit.FmdKgM3, 60.0);

			// Return Result;
			pdStdDensity = dStdDensity;

			return true;

		} // End of CalcTankStdDensity()

		public virtual bool CalcTankDensity(EngineeringUnit bDensityUnits,       // Density Engineering Units
									double dTemp,                   // Current Temperature
									EngineeringUnit bTempUnits,            // Temperature Engineering Units
									ETempRounding byTempRoundingMethod,
									EVcfRounding byVcfRoundingMethod,
									double dStdDensity,         // Product Standard Density
									EngineeringUnit bStdDensityUnits,  // Standard Density Engr Units
									double dVolCorrFactor,      // Volume Correction Factor
									double dDensityPress,           // density pressure for api 2004
									EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
									double dBaseTemp,   // api 2004 alternate base temp reference
									double dAlternateBasePress, // api 2004 alternate base pressure reference
									ref double CTLReturn,
									ref double CPLReturn,
									ref double[] dK,
									ref double pdDensity,              // Pointer to Density Variable
									ref double dHydrometer)
		{
			double dDensity = 0.0;
			double dTempInC = 0.0;
			double dTempInF = 0.0;


			bool hRes = TemperatureRound_Japan(byTempRoundingMethod, dTemp, ref dTemp);

			// First convert Std Density to Density units
			// Then calculate

			// Convert Temperature to Degrees Centigrade - Required by Units Conversion
			ConvertEngUnits.ConvEngrUnits(ref dTempInC, dTemp, EngineeringUnit.FmtDegC, bTempUnits, 0);

			// Convert Temperature to Degress F
			ConvertEngUnits.ConvEngrUnits(ref dTempInF, dTemp, EngineeringUnit.FmtDegF, bTempUnits, 0);

			switch (m_byCorrectionTypeMajor)
			{
				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F:
				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_F_1980:
					{
						switch (m_byCorrectionTypeMinor)
						{
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6A:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6B:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API6D:

								// convert any denisty unit to API
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dStdDensity, EngineeringUnit.FmdDegApi, bStdDensityUnits, dTempInC);

								// Round to nearest 0.1
								RoundDouble(dDensity, ref dDensity, 1, false, false);

								// convert any denisty unit to kg/m3
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dDensity, EngineeringUnit.FmdKgM3, EngineeringUnit.FmdDegApi, 60.0);

								// ApplyHydroCorrection and will only apply if = 1
								ApplyHydroCorrection(ref dDensity, dTempInF, m_byCorrectionTypeMajor, m_byCorrectionTypeMinor, true, ref dHydrometer);

								break;

							default:
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dStdDensity, EngineeringUnit.FmdKgM3, bStdDensityUnits, 60.0);
								break;
						}
						break;
					}

				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C:
				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_API_C_1980:
					{
						switch (m_byCorrectionTypeMinor)
						{
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D:
								// convert any denisty unit to kg/m3
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dStdDensity, EngineeringUnit.FmdKgM3, bStdDensityUnits, 60.0);

								// ApplyHydroCorrection and will only apply if = 1
								ApplyHydroCorrection(ref dDensity, dTempInC, m_byCorrectionTypeMajor, m_byCorrectionTypeMinor, true, ref dHydrometer);
								break;

							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60A:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60B:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API60D:

								// convert any denisty unit to kg/m3
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dStdDensity, EngineeringUnit.FmdKgM3, bStdDensityUnits, 60.0);

								// ApplyHydroCorrection and will only apply if = 1
								ApplyHydroCorrection(ref dDensity, dTempInC, m_byCorrectionTypeMajor, m_byCorrectionTypeMinor, true, ref dHydrometer);
								break;

							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54A_30:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54B_30:
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_API54D_30:

								// convert any denisty unit to kg/m3
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dStdDensity, EngineeringUnit.FmdKgM3, bStdDensityUnits, 60.0);

								// ApplyHydroCorrection and will only apply if = 1
								ApplyHydroCorrection(ref dDensity, dTempInC, m_byCorrectionTypeMajor, m_byCorrectionTypeMinor, true, ref dHydrometer);
								break;

							default:
								ConvertEngUnits.ConvEngrUnits(ref dDensity, dStdDensity, EngineeringUnit.FmdKgM3, bStdDensityUnits, 60.0);
								break;
						}
						break;
					}

				case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_GOST:
					{
						switch (m_byCorrectionTypeMinor)
						{
							case Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_3900_85_20C:
								{
									//if (!DensityLookup(dStdDensity, dTemp, bStdDensityUnits, bTempUnits,pdDensity))
									//	return (S_FALSE);
									//else
									//	return (S_OK);
									return false;
								}
						}
						break;
					}

				default:
					ConvertEngUnits.ConvEngrUnits(ref dDensity, dStdDensity, EngineeringUnit.FmdKgM3, bStdDensityUnits, 60.0);
					break;
			}

			// Convert Density to Standard Conditions
			dStdDensity = dDensity * dVolCorrFactor;

			// round density result to nearest 0.1
			RoundDouble(dStdDensity, ref dStdDensity, 1, false, false);

			// Convert from kg/m3 to Density Units
			ConvertEngUnits.ConvEngrUnits(ref dDensity, dStdDensity, bDensityUnits, EngineeringUnit.FmdKgM3, 60.0);
			pdDensity = dDensity;

			return (true);
		} // End of CalcTankDensity()

	}
}
