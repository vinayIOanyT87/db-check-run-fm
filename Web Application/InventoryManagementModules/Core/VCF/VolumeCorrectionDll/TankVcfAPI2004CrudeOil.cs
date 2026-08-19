using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class TankVcfAPI2004CrudeOil : TankBaseVcf
    {
        protected double m_dTable54ReferenceTemperature;
        protected ApiCalculationRequest APIRequestStruct;
        public TankVcfAPI2004CrudeOil()
        {
            Error ReturnCode;
            m_byCorrectionTypeMajor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMajor.CORR_ASTM_COMM_2004;
            m_byCorrectionTypeMinor = Varec.CommonComponents.VolumeCorrection.ECorrectionTypeMinor.CORR_CRUDE_OIL;
            m_bUsesDensity = false;
            m_dTable54ReferenceTemperature = TAB54_DEF_REF_TEMP;
            m_bStandardCalculationType = EApiCalc.API_CALC_STANDARD;
            APIRequestStruct = ApiCalculationRequest.Init(out ReturnCode);
        }

		/*        protected bool CalcTankStdDensity(EngineeringUnit bStdDensityUnits, // Std Density Engr Units
																		 double dTemp,               // Measured Temperature
																		  EngineeringUnit bTempUnits,        // Temperature Engr Units
																		 EngineeringUnit byTempRoundingMethod,
																		 byte byVcfRoundingMethod,
																		 double dDensity,            // Measured Density
																		 EngineeringUnit bDensityUnits,     // Density Engr Units
																		 double dVolCorrFactor,  // Volume Correction Factor
																		 double dDensityPress,           // density pressure for api 2004
																		 EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
																		 double dAlternateTemperature,   // selected refined product sub catagory for api 2004
																		 double dBaseTemp,   // api 2004 alternate base temp reference
																		 double dAlternateBasePress, // api 2004 alternate base pressure reference
																		 ref double[] dK,
																		 ref double pdStdDensity,       // Standard Density Variable
																		 ref double dHydrometer)
		*/
		public override bool CalcTankStdDensity(EngineeringUnit bStdDensityUnits, // Std Density Engr Units
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
			double dDensityInKGM3 = 0.00;
            double ProdDensity = 0.00;
            double dVcfc = 0.00;
            bool bUseAlternate = false;

            if (bTempUnits == EngineeringUnit.FmtDegF &&
                (dBaseTemp != 60.0 ||
                dAlternateBasePress != 0.0))
            {   // correct to a non standard temp/press configuration
                bUseAlternate = true;
            }
            else if (bTempUnits == EngineeringUnit.FmtDegC &&
                ((dAlternateTemperature != 15.0 &&
                dAlternateTemperature != 20.0) ||
                dAlternateBasePress != 0.0))
                bUseAlternate = true;

            if (bUseAlternate)
            {

                if (!CalcAlternateParameters(dDensity,
                                                    dTemp,
                                                    bDensityUnits,
                                                    bTempUnits,
                                                    dDensityPress,
                                                    bDensityPressUnits,
                                                    dAlternateTemperature,
                                                    dBaseTemp,
                                                    dAlternateBasePress,
													ref CTLReturn,
													ref CPLReturn,
													ref dVcfc,
                                                    ref dDensityInKGM3))
                {
                    return false;
                }
            }
            else
            {
                if (!CalcDensityParameters(dDensity,
                                                    dTemp,
                                                    bDensityUnits,
                                                    bTempUnits,
                                                    dDensityPress,
                                                    bDensityPressUnits,
                                                    dBaseTemp,
                                                    dAlternateBasePress,
													ref CTLReturn,
													ref CPLReturn,
													ref dVcfc,
                                                    ref dDensityInKGM3))
                {
                    return false;
                }
            }

            if (!ConvertEngUnits.ConvEngrUnits(ref ProdDensity, dDensityInKGM3, bStdDensityUnits, EngineeringUnit.FmdKgM3, 15.55555))
                return false;

            pdStdDensity = ProdDensity;
            return true;
        }

        public override bool CalcTankDensity(EngineeringUnit bDensityUnits,         // Density Engineering Units
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
			double ProdDensity = 0.00;
            double dVcfc = 0.00;
            double dDensityInKGM3 = 0.00;

            if (!CalcStdDensityParameters(dStdDensity,
                                                dTemp,
                                                bStdDensityUnits,
                                                bTempUnits,
                                                dDensityPress,
                                                bDensityPressUnits,
                                                dBaseTemp,
                                                dAlternateBasePress,
													ref CTLReturn,
													ref CPLReturn,
												ref dVcfc,
                                                ref dDensityInKGM3))
            {
                return false;
            }

            if (!ConvertEngUnits.ConvEngrUnits(ref ProdDensity, dDensityInKGM3, bDensityUnits, EngineeringUnit.FmdKgM3, 15.55555))
                return false;

            pdDensity = ProdDensity;
            return true;
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
                               ref int piFlag,
												ref double CTLReturn,
												ref double CPLReturn,
							   bool RangeCk,
                               bool bRound,
                               bool bTable60,             //	Optional
                               bool UseDensity)              //	Optional
        {
            double dCalcDensity = 0.0;
            bool bUseAltCalcMethod = false;

            if (bTempUnits == EngineeringUnit.FmtDegF &&
                ((dBaseTemp != 60.0 ||
                    dAlternateBasePress != 0.0) &&
                    UseDensity))
            {   // correct to a non standard temp/press configuration
                bUseAltCalcMethod = true;
            }
            else if (bTempUnits == EngineeringUnit.FmtDegC &&
                ((dAlternateTemperature != 15.0 &&
                dAlternateTemperature != 20.0) ||
                dAlternateBasePress != 0.0) &&
                    UseDensity)
                bUseAltCalcMethod = true;

            if (bUseAltCalcMethod)
            {
                if (!CalcAlternateParameters(dDensity,
                                                    dMeasTemp,
                                                    bDensityUnits,
                                                    bTempUnits,
                                                    dDensityPress,
                                                    bDensityPressUnits,
                                                    dAlternateTemperature,
                                                    dBaseTemp,
                                                    dAlternateBasePress,
													ref CTLReturn,
													ref CPLReturn,
													ref pdVcfc,
                                                    ref dCalcDensity))
                {
                    piFlag = -1;
                    return false;
                }
                else
                    piFlag = 0;
            }
            else
            {
                if (!UseDensity)
                {
                    if (!CalcStdDensityParameters(dDensity,
                                                        dMeasTemp,
                                                        bDensityUnits,
                                                        bTempUnits,
                                                        dDensityPress,
                                                        bDensityPressUnits,
                                                        dBaseTemp,
                                                        dAlternateBasePress,
													ref CTLReturn,
													ref CPLReturn,
														ref pdVcfc,
                                                        ref dCalcDensity))
                    {
                        piFlag = -1;
                        return false;
                    }
                    else
                        piFlag = 0;

                }
                else
                {
                    if (!CalcDensityParameters(dDensity,
                                                        dMeasTemp,
                                                        bDensityUnits,
                                                        bTempUnits,
                                                        dDensityPress,
                                                        bDensityPressUnits,
                                                        dBaseTemp,
                                                        dAlternateBasePress,
													ref CTLReturn,
													ref CPLReturn,
														ref pdVcfc,
                                                        ref dCalcDensity))
                    {
                        piFlag = -1;
                        return false;
                    }
                    else
                        piFlag = 0;

                }
            }

            return true;
        }


        protected bool CalcAlternateParameters(double dDensity,
                                                double dMeasTemp,
                                                EngineeringUnit bDensityUnits,
                                                EngineeringUnit bTempUnits,
                                                double dDensityPress,           // density pressure for api 2004
                                                EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
                                                double dAlternateTemperature,
                                                double dBaseTemp,   // api 2004 alternate base temp reference
                                                double dAlternateBasePress, // api 2004 alternate base pressure reference
												ref double CTLReturn,
												ref double CPLReturn,
												ref double pdVcfc,
                                                ref double dCalcDensity)
        {
            double dTempLocal;
            double dPressureLocal;
            double dAltTempLocal;
            double dBaseTempLocal;
            double dAltPressureLocal;
            double dDensityLocal;
            Error ReturnCode;
            double VCFvalue;
            double roundedVCF;
            double dDensityInKGM3;
            ApiUnit APITemperatureUnits;
            ApiUnit APIPressureUnits;
            ApiUnit APIDensityUnits;
            EngineeringUnit bCalcMethodToUse = EngineeringUnit.FmtDegF;

            dTempLocal = dMeasTemp;
            dAltTempLocal = dAlternateTemperature;
            dBaseTempLocal = dBaseTemp;
            switch (bTempUnits)
            {
                case EngineeringUnit.FmtDegF:
                    APITemperatureUnits = ApiUnit.ApiUnit_Temperature_F();
                    bCalcMethodToUse = EngineeringUnit.FmtDegF;
                    break;
                case EngineeringUnit.FmtDegC:
                    APITemperatureUnits = ApiUnit.ApiUnit_Temperature_C();
                    bCalcMethodToUse = EngineeringUnit.FmtDegC;
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dTempLocal, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0)
                            || !ConvertEngUnits.ConvEngrUnits(ref dAltTempLocal, dAlternateTemperature, EngineeringUnit.FmtDegF, bTempUnits, 0)
                            || !ConvertEngUnits.ConvEngrUnits(ref dBaseTempLocal, dBaseTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                            return false;
                        APITemperatureUnits = ApiUnit.ApiUnit_Temperature_F();
                        bCalcMethodToUse = EngineeringUnit.FmtDegF;
                    }
                    break;
            }

            dDensityLocal = dDensity;
            switch (bDensityUnits)
            {
                case EngineeringUnit.FmdDegApi:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_API();
                    break;
                case EngineeringUnit.FmdSpGrav:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_RELATIVE();
                    break;
                case EngineeringUnit.FmdKgM3:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_KGM3();
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dDensityLocal, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, 15.55555))
                            return false;
                        APIDensityUnits = ApiUnit.ApiUnit_Density_API();
                    }
                    break;
            }

            dPressureLocal = dDensityPress;
            dAltPressureLocal = dAlternateBasePress;
            switch (bDensityPressUnits)
            {
                case EngineeringUnit.FmpPsi:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_PSI();
                    break;
                case EngineeringUnit.FmpKPa:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_KPA();
                    break;
                case EngineeringUnit.FmpBar:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_BAR();
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dPressureLocal, dDensityPress, EngineeringUnit.FmpPsi, bDensityPressUnits, 0)
                            || !ConvertEngUnits.ConvEngrUnits(ref dAltPressureLocal, dAlternateBasePress, EngineeringUnit.FmpPsi, bDensityPressUnits, 0))
                            return false;
                        APIPressureUnits = ApiUnit.ApiUnit_Pressure_PSI();
                    }
                    break;
            }

            if (bCalcMethodToUse == EngineeringUnit.FmtDegC)
            {
                ReturnCode = APIRequestStruct.SetParameters(
                                    ApiOilProduct.EProductNumber.API_CRUDE_OIL_NAME,
                                    0, null,
                                    dTempLocal, APITemperatureUnits,
                                    dAltTempLocal, APITemperatureUnits,
                                    dPressureLocal, APIPressureUnits,
                                    dAltPressureLocal, APIPressureUnits,
                                    dDensityLocal, APIDensityUnits,
                                    dBaseTemp, APITemperatureUnits,
                                    0, ApiUnit.ApiUnit_Pressure_KPA(),
                                    0, null,
                                    0, null,
                                    0, null);
            }
            else
            {
                ReturnCode = APIRequestStruct.Set11_1_6_3Parameters(
                                    ApiOilProduct.EProductNumber.API_CRUDE_OIL_NAME,
                                    0, null,
                                    dTempLocal, APITemperatureUnits,
                                    dPressureLocal, APIPressureUnits,
                                    dDensityLocal, APIDensityUnits,
                                    dAltTempLocal, APITemperatureUnits,
                                    dAltPressureLocal, APIPressureUnits,
                                    0, null);
            }
            if (ReturnCode != Error.NO_ERROR)
                return false;

            ReturnCode = APIRequestStruct.PerformCalculation(ref CTLReturn, ref CPLReturn);
			if (ReturnCode != Error.NO_ERROR)
                return false;

            VCFvalue = APIRequestStruct.ctplBaseToAlt.GetValue(
                                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                                      false,
                                                    out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            roundedVCF = APIRequestStruct.ctplBaseToAlt.GetValue(
                                                        ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                                           true,
                                                        out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            dDensityInKGM3 = APIRequestStruct.baseDens.GetValue(
                                                             ApiUnit.ApiUnit_Density_KGM3(),
                                                             false,
                                                             out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            dCalcDensity = dDensityInKGM3;
            pdVcfc = VCFvalue;


            return true;
        }

        protected bool CalcStdDensityParameters(double dDensity,
                                                double dMeasTemp,
                                                EngineeringUnit bDensityUnits,
                                                EngineeringUnit bTempUnits,
                                                double dDensityPress,           // density pressure for api 2004
                                                EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
                                                double dBaseTemp,
                                                double dBasePressure,
												ref double CTLReturn,
												ref double CPLReturn,
												ref double pdVcfc,
                                                ref double dCalcDensity)
        {
            double dTempLocal;
            double dPressureLocal;
            double dDensityLocal;
            Error ReturnCode;
            double VCFvalue;
            double roundedVCF;
            double dDensityInKGM3;
            ApiUnit APITemperatureUnits;
            ApiUnit APIPressureUnits;
            ApiUnit APIDensityUnits;
            EngineeringUnit bCalcMethodToUse = EngineeringUnit.FmtDegF;

            dTempLocal = dMeasTemp;
            switch (bTempUnits)
            {
                case EngineeringUnit.FmtDegF:
                    APITemperatureUnits = ApiUnit.ApiUnit_Temperature_F();
                    bCalcMethodToUse = EngineeringUnit.FmtDegF;
                    break;
                case EngineeringUnit.FmtDegC:
                    APITemperatureUnits = ApiUnit.ApiUnit_Temperature_C();
                    bCalcMethodToUse = EngineeringUnit.FmtDegC;
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dTempLocal, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                            return false;
                        APITemperatureUnits = ApiUnit.ApiUnit_Temperature_F();
                        bCalcMethodToUse = EngineeringUnit.FmtDegF;
                    }
                    break;
            }

            dDensityLocal = dDensity;
            switch (bDensityUnits)
            {
                case EngineeringUnit.FmdDegApi:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_API();
                    break;
                case EngineeringUnit.FmdSpGrav:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_RELATIVE();
                    break;
                case EngineeringUnit.FmdKgM3:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_KGM3();
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dDensityLocal, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, 15.55555))
                            return false;
                        APIDensityUnits = ApiUnit.ApiUnit_Density_API();
                    }
                    break;
            }

            dPressureLocal = dDensityPress;
            switch (bDensityPressUnits)
            {
                case EngineeringUnit.FmpPsi:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_PSI();
                    break;
                case EngineeringUnit.FmpKPa:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_KPA();
                    break;
                case EngineeringUnit.FmpBar:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_BAR();
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dPressureLocal, dDensityPress, EngineeringUnit.FmpPsi, bDensityPressUnits, 0))
                            return false;
                        APIPressureUnits = ApiUnit.ApiUnit_Pressure_PSI();
                    }
                    break;
            }

            if (bCalcMethodToUse == EngineeringUnit.FmtDegC)
            {
                ReturnCode = APIRequestStruct.Set11_1_7_1Parameters(
                                    ApiOilProduct.EProductNumber.API_CRUDE_OIL_NAME,
                                    0, null,
                                    dTempLocal, APITemperatureUnits,
                                    dPressureLocal, APIPressureUnits,
                                    dDensityLocal, APIDensityUnits,
                                    dBaseTemp, ApiUnit.ApiUnit_Temperature_C(),
                                    0, null);
            }
            else
            {
                ReturnCode = APIRequestStruct.Set11_1_6_1Parameters(
                                    ApiOilProduct.EProductNumber.API_CRUDE_OIL_NAME,
                                    0, null,
                                    dTempLocal, APITemperatureUnits,
                                    dPressureLocal, APIPressureUnits,
                                    dDensityLocal, APIDensityUnits,
                                    0, null);
            }
            if (ReturnCode != Error.NO_ERROR)
                return false;

            ReturnCode = APIRequestStruct.PerformCalculation(ref CTLReturn, ref CPLReturn);
			if (ReturnCode != Error.NO_ERROR)
                return false;

            VCFvalue = APIRequestStruct.ctplBaseToAlt.GetValue(
                                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                                      false,
                                                    out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            roundedVCF = APIRequestStruct.ctplBaseToAlt.GetValue(
                                                        ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                                           true,
                                                        out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            dDensityInKGM3 = APIRequestStruct.altDens.GetValue(
                                                             ApiUnit.ApiUnit_Density_KGM3(),
                                                             false,
                                                             out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            dCalcDensity = dDensityInKGM3;
            pdVcfc = VCFvalue;


            return true;
        }

        protected bool CalcDensityParameters(double dDensity,
                                            double dMeasTemp,
                                            EngineeringUnit bDensityUnits,
                                            EngineeringUnit bTempUnits,
                                            double dDensityPress,           // density pressure for api 2004
                                            EngineeringUnit bDensityPressUnits,    // density pressure units for api 2004
                                            double dBaseTemp,
                                            double dBasePressure,
											ref double CTLReturn,
											ref double CPLReturn,
											ref double pdVcfc,
                                            ref double pdStdDensity)
        {
            double dTempLocal;
            double dPressureLocal;
            double dDensityLocal;
            Error ReturnCode;
            double VCFvalue;
            double roundedVCF;
            double dDensityInKGM3;
            ApiUnit APITemperatureUnits;
            ApiUnit APIPressureUnits;
            ApiUnit APIDensityUnits;
            EngineeringUnit bCalcMethodToUse = EngineeringUnit.FmtDegF;

            dTempLocal = dMeasTemp;
            switch (bTempUnits)
            {
                case EngineeringUnit.FmtDegF:
                    APITemperatureUnits = ApiUnit.ApiUnit_Temperature_F();
                    bCalcMethodToUse = EngineeringUnit.FmtDegF;
                    break;
                case EngineeringUnit.FmtDegC:
                    APITemperatureUnits = ApiUnit.ApiUnit_Temperature_C();
                    bCalcMethodToUse = EngineeringUnit.FmtDegC;
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dTempLocal, dMeasTemp, EngineeringUnit.FmtDegF, bTempUnits, 0))
                            return false;
                        APITemperatureUnits = ApiUnit.ApiUnit_Temperature_F();
                        bCalcMethodToUse = EngineeringUnit.FmtDegF;
                    }
                    break;
            }

            dDensityLocal = dDensity;
            switch (bDensityUnits)
            {
                case EngineeringUnit.FmdDegApi:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_API();
                    break;
                case EngineeringUnit.FmdSpGrav:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_RELATIVE();
                    break;
                case EngineeringUnit.FmdKgM3:
                    APIDensityUnits = ApiUnit.ApiUnit_Density_KGM3();
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dDensityLocal, dDensity, EngineeringUnit.FmdDegApi, bDensityUnits, 15.55555))
                            return false;
                        APIDensityUnits = ApiUnit.ApiUnit_Density_API();
                    }
                    break;
            }

            dPressureLocal = dDensityPress;
            switch (bDensityPressUnits)
            {
                case EngineeringUnit.FmpPsi:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_PSI();
                    break;
                case EngineeringUnit.FmpKPa:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_KPA();
                    break;
                case EngineeringUnit.FmpBar:
                    APIPressureUnits = ApiUnit.ApiUnit_Pressure_BAR();
                    break;
                default:
                    {
                        if (!ConvertEngUnits.ConvEngrUnits(ref dPressureLocal, dDensityPress, EngineeringUnit.FmpPsi, bDensityPressUnits, 0))
                            return false;
                        APIPressureUnits = ApiUnit.ApiUnit_Pressure_PSI();
                    }
                    break;
            }

            if (bCalcMethodToUse == EngineeringUnit.FmtDegC)
            {
                ReturnCode = APIRequestStruct.Set11_1_7_2Parameters(
                                    ApiOilProduct.EProductNumber.API_CRUDE_OIL_NAME,
                                    0, null,
                                    dTempLocal, APITemperatureUnits,
                                    dPressureLocal, APIPressureUnits,
                                    dDensityLocal, APIDensityUnits,
                                    dBaseTemp, ApiUnit.ApiUnit_Temperature_C(),
                                    0, null);
            }
            else
            {
                ReturnCode = APIRequestStruct.Set11_1_6_2Parameters(
                                    ApiOilProduct.EProductNumber.API_CRUDE_OIL_NAME,
                                    0, null,
                                    dTempLocal, APITemperatureUnits,
                                    dPressureLocal, APIPressureUnits,
                                    dDensityLocal, APIDensityUnits,
                                    0, null);
            }
            if (ReturnCode != Error.NO_ERROR)
                return false;

            ReturnCode = APIRequestStruct.PerformCalculation(ref CTLReturn, ref CPLReturn);
			if (ReturnCode != Error.NO_ERROR)
                return false;

            VCFvalue = APIRequestStruct.ctlObToBase.GetValue(
                                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                                      false,
                                                    out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            roundedVCF = APIRequestStruct.ctlObToBase.GetValue(
                                                        ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                                           true,
                                                        out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            dDensityInKGM3 = APIRequestStruct.baseDens.GetValue(
                                                             ApiUnit.ApiUnit_Density_KGM3(),
                                                             false,
                                                             out ReturnCode);
            if (ReturnCode != Error.NO_ERROR)
                return false;

            pdStdDensity = dDensityInKGM3;

            pdVcfc = VCFvalue;


            return true;
        }
    }
}
