using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiCalculationRequest
    {
        public enum EParameterName
        {
            API_BASE_TEMP_NAME = 0,
            API_BASE_PRES_NAME = 1,
            API_COMMODITY_NAME = 2,
            API_ALPHA_60_NAME = 3,
            API_OB_TEMP_NAME = 4,
            API_OB_PRES_NAME = 5,
            API_OB_DENS_NAME = 6,
            API_ALT_TEMP_NAME = 7,
            API_ALT_PRES_NAME = 8,
            API_OB_VOL_NAME = 9,
            API_ALT_VOL_NAME = 10,
            API_BASE_VOL_NAME = 11,
            API_ALT_DENS_NAME = 12,
            API_SCALED_COMP_ALT_NAME = 13,
            API_BASE_DENS_NAME = 14,
            API_CTL_OB_TO_BASE_NAME = 15,
            API_CPL_OB_TO_BASE_NAME = 16,
            API_CTPL_OB_TO_BASE_NAME = 17,
            API_SCALED_COMP_OB_NAME = 18,
            API_CTL_BASE_TO_ALT_NAME = 19,
            API_CPL_BASE_TO_ALT_NAME = 20,
            API_CTPL_BASE_TO_ALT_NAME = 21,
            API_PARAMETERS_MAX_NUMBER = 22
        }


        protected static string[] ApiParameterNames = new string[]
                   {"Base Temp.",
            "Base Pres.",
            "Commodity",
            "Alpha_60",
            "Ob. Temp",
            "Ob. Pres",
            "Ob. Dens",
            "Alt.Temp",
            "Alt. Pres.",
            "Ob. Vol",
            "Alt. Vol.",
            "Base Vol.",
            "Alt. Dens",
            "Fp",
            "Base Dens.",
            "ctl (obs. to base)",
            "cpl (obs. to base)",
            "ctpl (obs. to base)",
            "Fp (obs. to base)",
            "ctl (base to alt.)",
            "cpl (base to alt.)",
            "ctpl (base to alt.)"};

        protected static string[] ApiOilProductNames = new string[]
                    {"Crude Oil",
             "Lubricating Oil",
             "Refined Products",
             "Fuel Oil",
             "Jet Fuel",
             "Transition Zone",
             "Gasoline"};

        protected static string[] ApiErrorValues = new string[]
     { "Initialization failed",
       "Nullpointer Exception",
       "Invalid unit for quantity",
       "Attempted to change an immuatable quantity",
       "Buffer overflow for text base char array",
       "Attempt to compare two quantites of different type",
       "Value for quantity is out of range",
       "Attempt to call an unsupported function",
       "K-Values are not defined for the given commodity",
       "rho limits are not defined for the given commodity",
       "Undefined commodity type",
       "Commodity and alpha_60 are both supplied, please give only one",
       "Commodity and alpha_60 are both null. Please supply at least one",
       "Alpha_60 value is out of range",
       "Observed density value is missing",
       "Observed density is out of range for Type I calculation",
       "Observed density is out of range for Type II calculation",
       "Observed pressure value is missing",
       "Observed pressure is out of range",
       "Alternate pressure value is missing",
       "Alternate pressure is out of range",
       "Observed temperature value is missing",
       "Observed temperature is out of range",
       "Alternate temperature value is missing",
       "Alternate temperature is out of range",
       "Please supply only one of these: Observered Volume, Base Volume or Alternate Volume",
       "Density out of range during iteration",
       "Convergence was not reached",
       "Array index out of range"};


        public ApiTemperature baseTemp;
        public ApiPressure basePres;
        public ApiOilProduct.EProductNumber commodityName;
        public ApiAlpha60 alpha60;
        public ApiTemperature obTemp;
        public ApiPressure obPres;
        public ApiDensity obDens;
        public ApiTemperature altTemp;
        public ApiPressure altPres;
        public ApiVolume obVol;
        public ApiVolume altVol;
        public ApiVolume baseVol;
        public ApiExpansionFactor ctlAlt;
        public ApiExpansionFactor cplAlt;
        public ApiExpansionFactor ctplAlt;
        public ApiDensity altDens;
        public ApiScaledFactor scaledCompAlt;
        public ApiDensity baseDens;
        public ApiExpansionFactor ctlObToBase;
        public ApiExpansionFactor cplObToBase;
        public ApiExpansionFactor ctplObToBase;
        public ApiScaledFactor scaledCompOb;
        public ApiExpansionFactor ctlBaseToAlt;
        public ApiExpansionFactor cplBaseToAlt;
        public ApiExpansionFactor ctplBaseToAlt;
        public string comments;
        public string intermediate;
        public bool reportIntermediate;


        /**********************************************************************************************************/

        public ApiOilProduct CALC_REQ_COMMODITY = null;
        public ApiOilProduct CALC_REQ_DET_COM = null;
        public ApiCalculationRequest CALC_REQ_INTER = null;
        public ApiTemperature CALC_REQ_BASE_TEMP = null;
        public ApiPressure CALC_REQ_BASE_PRES = null;
        public Error Initialize()
        {
            Error errorCode = Error.NO_ERROR;

            errorCode = ApiUnit.ApiUnit_initalize();
            if (errorCode != 0)
            {
                return errorCode;
            }

            return errorCode;
        }

        public static ApiCalculationRequest Init(out Error errorCode)
        {
            ApiCalculationRequest request = new ApiCalculationRequest();

            errorCode = Error.NO_ERROR;

            if (request == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return request;
            }

            /** Initialize the base parameters */
			request.baseTemp = ApiTemperature.Init(Constants.API_BASE_TEMP,
								 ApiUnit.ApiUnit_Temperature_F(),
								 true,
								 out errorCode);
			if (errorCode != Error.NO_ERROR)
            {
                request.baseTemp = null;
                return request;
            }

            request.basePres = ApiPressure.Init(Constants.API_BASE_PRES,
                              ApiUnit.ApiUnit_Pressure_PSI(),
                              true,
                              out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                request.baseTemp = null;
                request.basePres = null;
                return request;
            }

            /** By default do not report intermediate results */
            request.reportIntermediate = false;

            /** And return the new object*/
            return request;
        }

        public void SetErrorStatus()
        {
            ctlAlt = null;
            cplAlt = null;
            ctplAlt = null;
            altDens = null;
            scaledCompAlt = null;
            baseDens = null;
            ctlObToBase = null;
            cplObToBase = null;
            ctplObToBase = null;
            scaledCompOb = null;
            ctlBaseToAlt = null;
            cplBaseToAlt = null;
            ctplBaseToAlt = null;
        }

        public static Error AppendBuffer(ref string buffer, string result)
        {
            Error errorCode = Error.NO_ERROR;

            if (buffer != null && result != null)
            {
                buffer += result;
            }
            else errorCode = Error.BUFFER_OVERFLOW;
            return errorCode;
        }

        public static Error AppendDouble(ref string buffer, double value)
        {
            Error errorCode = Error.NO_ERROR;

            if (buffer != null)
            {
                buffer += value.ToString();
            }
            else errorCode = Error.BUFFER_OVERFLOW;
            return errorCode;
        }

        public Error InitializeRequest()
        {
            Error errorCode = Error.NO_ERROR;


            if (obTemp == null)
            {
                obTemp = ApiTemperature.Init(Constants.API_BASE_TEMP,
                                    ApiUnit.ApiUnit_Temperature_F(),
                                    true,
                                    out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (obPres == null)
            {
                obPres = ApiPressure.Init(Constants.API_BASE_PRES,
                               ApiUnit.ApiUnit_Pressure_PSI(),
                               true,
                               out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (obDens == null)
            {
                obDens = ApiDensity.Init(0,
                              ApiUnit.ApiUnit_Density_KGM3(),
                              true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (altTemp == null)
            {
                altTemp = ApiTemperature.Init(Constants.API_BASE_TEMP,
                                ApiUnit.ApiUnit_Temperature_F(),
                              true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (altPres == null)
            {
                altPres = ApiPressure.Init(Constants.API_BASE_PRES,
                               ApiUnit.ApiUnit_Pressure_PSI(),
                              true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (ctlAlt == null)
            {
                ctlAlt = ApiExpansionFactor.Init(1.0,
                                  ApiUnit.ApiUnit_Expansion_DIMLESS(),
                               true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (cplAlt == null)
            {
                cplAlt = ApiExpansionFactor.Init(1.0,
                                  ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                 true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (ctplAlt == null)
            {
                ctplAlt = ApiExpansionFactor.Init(1.0,
                                  ApiUnit.ApiUnit_Expansion_DIMLESS(),
                              true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (altDens == null)
            {
                altDens = ApiDensity.Init(0,
                            ApiUnit.ApiUnit_Density_KGM3(),
                               true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (scaledCompAlt == null)
            {
                scaledCompAlt = ApiScaledFactor.Init(1.0,
                                       ApiUnit.ApiUnit_ScaledComp_REV_PSI(),
                               true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }


            if (baseDens == null)
            {
                baseDens = ApiDensity.Init(0,
                                ApiUnit.ApiUnit_Density_KGM3(),
                               true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (ctlObToBase == null)
            {
                ctlObToBase = ApiExpansionFactor.Init(1.0,
                                   ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (cplObToBase == null)
            {
                cplObToBase = ApiExpansionFactor.Init(1.0,
                                   ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                    true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (ctplObToBase == null)
            {
                ctplObToBase = ApiExpansionFactor.Init(1.0,
                                   ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                       true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (scaledCompOb == null)
            {
                scaledCompOb = ApiScaledFactor.Init(1.0,
                                     ApiUnit.ApiUnit_ScaledComp_REV_PSI(),
                                  true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (cplBaseToAlt == null)
            {
                cplBaseToAlt = ApiExpansionFactor.Init(1.0,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                         true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (ctlBaseToAlt == null)
            {
                ctlBaseToAlt = ApiExpansionFactor.Init(1.0,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                  true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (ctplBaseToAlt == null)
            {
                ctplBaseToAlt = ApiExpansionFactor.Init(1.0,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                    true,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            return errorCode;
        }

        public Error GetVolumeData()
        {

            Error errorCode = Error.NO_ERROR;
            ApiUnit vu;
            double rounded, value;

            /**  the observed volume is supplied */
            if (obVol != null)
            {
                vu = obVol.givenUnit;

                /** Get the observed volume */
                value = obVol.GetValue(
                              vu,
                              false,
                             out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Calculate and set the base volume */
                rounded = ctplObToBase.GetValue(
                               ApiUnit.ApiUnit_Expansion_DIMLESS(),
                               true,
                               out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                value *= rounded;
                baseVol = ApiVolume.Init(value, vu, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Calculate and set the alternate volume */
                rounded = ctplBaseToAlt.GetValue(
                               ApiUnit.ApiUnit_Expansion_DIMLESS(),
                               true,
                               out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                value /= rounded;
                altVol = ApiVolume.Init(value, vu, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }


            /**  The base volume is supplied */
            else if (baseVol != null)
            {
                vu = baseVol.givenUnit;

                /** Get the base volume */
                value = baseVol.GetValue(
                              vu,
                              false,
                             out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Calculate and set the alternate volume */
                rounded = ctplBaseToAlt.GetValue(
                               ApiUnit.ApiUnit_Expansion_DIMLESS(),
                               true,
                               out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                value /= rounded;
                altVol = ApiVolume.Init(value, vu, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Calculate and set the observed volume */
                rounded = ctplObToBase.GetValue(
                               ApiUnit.ApiUnit_Expansion_DIMLESS(),
                               true,
                               out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                value /= rounded;
                obVol = ApiVolume.Init(value, vu, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            /** The alternate volume is supplied */
            else if (altVol != null)
            {
                vu = altVol.givenUnit;
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Get the alternate volume */
                value = altVol.GetValue(
                              vu,
                              false,
                             out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Calculate and set the base volume */
                rounded = ctplBaseToAlt.GetValue(
                               ApiUnit.ApiUnit_Expansion_DIMLESS(),
                               true,
                               out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                value *= rounded;
                baseVol = ApiVolume.Init(value, vu, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Calculate and set the observed volume */
                rounded = ctplObToBase.GetValue(
                               ApiUnit.ApiUnit_Expansion_DIMLESS(),
                               true,
                               out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                value /= rounded;
                obVol = ApiVolume.Init(value, vu, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            return errorCode;
        }


        public void ReportQuantityValue(
                           bool isIntermediate,
                           string result,
                           double value)
        {

            if (isIntermediate == true && reportIntermediate == false) return;


            ApiCalculationRequest.AppendBuffer(ref intermediate, result);
            ApiCalculationRequest.AppendBuffer(ref intermediate, ".");
            ApiCalculationRequest.AppendDouble(ref intermediate, value);
            ApiCalculationRequest.AppendBuffer(ref intermediate, Constants.API_EOL);

        }

        public Error GetParameterName(EParameterName name, ref string buffer)
        {
            Error errorCode = Error.NO_ERROR;

            if (name >= 0 && name < EParameterName.API_PARAMETERS_MAX_NUMBER)
            {
                errorCode = AppendBuffer(ref buffer, ApiParameterNames[(int)name]);
            }
            else errorCode = Error.VCFARRAY_INDEX_OUT_OF_RANGE;
            return errorCode;
        }

        public Error GetOilProductName(ApiOilProduct.EProductNumber name, ref string buffer)
        {
            Error errorCode = Error.NO_ERROR;

            if (name >= 0 && name < ApiOilProduct.EProductNumber.API_OIL_PRODUCT_MAX_NUMBER)
            {
                errorCode = AppendBuffer(ref buffer, ApiOilProductNames[(int)name]);
            }
            else errorCode = Error.VCFARRAY_INDEX_OUT_OF_RANGE;
            return errorCode;
        }

        public Error ApiCalculation_getErrorDescription(Error name, ref string buffer)
        {
            Error errorCode = Error.NO_ERROR;

            if (name > Error.NO_ERROR && name <= Error.VCFMAX_ERROR_NUMBER)
            {
                int index = (int)name;
                index = index - 1;
                errorCode = AppendBuffer(ref buffer,
                                       ApiErrorValues[index]);
            }
            else errorCode = Error.VCFARRAY_INDEX_OUT_OF_RANGE;
            return errorCode;
        }


        public void ReportQuantity(
                      bool isIntermediate,
                      EParameterName name,
                      ApiQuantity value,
                      bool rounded,
                      ApiUnit unit)
        {
            Error errorCode = Error.NO_ERROR;
            double result;
            ApiUnit u = unit;

            //SRM
            if(unit == null)
            {
                u = ApiUnit.ApiUnit_Expansion_DIMLESS();
            }
            string paramName = string.Empty;
            GetParameterName(name, ref paramName);
            result = value.GetValue(u, rounded, out errorCode);
            string roundedString = rounded == true ? " (rounded) " : "";
            //Console.WriteLine(paramName + roundedString + " " + result);
            //SRM


            if (intermediate == null || value == null) return;
            if (isIntermediate == true && reportIntermediate == false) return;

            if (u == null)
            {
                u = value.givenUnit;
            }
            result = value.GetValue(u, rounded, out errorCode);
            if (errorCode != Error.NO_ERROR) return;

            GetParameterName(name, ref intermediate);

            if (rounded)
                AppendBuffer(ref intermediate, " (rounded)");
            AppendBuffer(ref intermediate, ".");
            AppendDouble(ref intermediate, result);
            ApiUnit.ApiUnit_unitName(u, ref intermediate);
            AppendBuffer(ref intermediate, Constants.API_EOL);
        }

        public void ReportInputData()
        {
            if (intermediate == null) return;

            AppendBuffer(ref intermediate, "Input Data");
            AppendBuffer(ref intermediate, Constants.API_EOL);


            if (commodityName != ApiOilProduct.EProductNumber.API_COMMODITY_NOT_GIVEN)
            {
                if (GetParameterName(EParameterName.API_COMMODITY_NAME, ref intermediate) == Error.NO_ERROR)
                {
                    AppendBuffer(ref intermediate, " : ");
                }
                if (GetOilProductName(commodityName, ref intermediate) != Error.NO_ERROR)
                {
                    if (intermediate != null)
                    {
                        AppendDouble(ref intermediate, (double)commodityName);
                    }
                }
                AppendBuffer(ref intermediate, Constants.API_EOL);
            }
            if (alpha60 != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_ALPHA_60_NAME,
                                 alpha60,
                                 false,
                                 null);
            }
            if (obTemp != null)
            {
                ReportQuantity(
                         false,
                         EParameterName.API_OB_TEMP_NAME,
                         obTemp,
                         false,
                         null);
            }
            if (obPres != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_OB_PRES_NAME,
                                 obPres,
                                 false,
                                 null);
            }
            if (obDens != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_OB_DENS_NAME,
                                 obDens,
                                 false,
                                 null);
            }
            if (altTemp != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_ALT_TEMP_NAME,
                                 altTemp,
                                 false,
                                 null);
            }
            if (altPres != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_ALT_PRES_NAME,
                                 altPres,
                                 false,
                                 null);
            }

            if (altVol != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_ALT_VOL_NAME,
                                 altVol,
                                 false,
                                 null);
            }
            if (baseVol != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_BASE_VOL_NAME,
                                 baseVol,
                                 false,
                                 null);
            }
            if (obVol != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_OB_VOL_NAME,
                                 obVol,
                                 false,
                                 null);
            }
        }

        public void ReportOutputData()
        {
            ApiUnit u;
            Error errorCode = Error.NO_ERROR;

            if (intermediate == null) return;

            AppendBuffer(ref intermediate, "Output Data");
            AppendBuffer(ref intermediate, Constants.API_EOL);

            AppendBuffer(ref intermediate, "Base Values");
            AppendBuffer(ref intermediate, Constants.API_EOL);
            if (baseDens != null)
            {
                u = obDens.GivenUnit(out errorCode);
                if (errorCode != Error.NO_ERROR) u = null;
                ReportQuantity(
                                 false,
                                 EParameterName.API_BASE_DENS_NAME,
                                 baseDens,
                                 false,
                                 u);
            }
            if (ctlObToBase != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_CTL_OB_TO_BASE_NAME,
                                 ctlObToBase,
                                 false,
                                 null);

            }
            if (scaledCompOb != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_SCALED_COMP_OB_NAME,
                                 scaledCompOb,
                                 false,
                                 null);
            }
            if (cplObToBase != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_CPL_OB_TO_BASE_NAME,
                                 cplObToBase,
                                 false,
                                 null);

            }
            if (ctplObToBase != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_CTPL_OB_TO_BASE_NAME,
                                 ctplObToBase,
                                 false,
                                 null);
                ReportQuantity(
                                 false,
                                 EParameterName.API_CTPL_OB_TO_BASE_NAME,
                                 ctplObToBase,
                                 true,
                                 null);

            }
            if (baseVol != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_BASE_VOL_NAME,
                                 baseVol,
                                 false,
                                 null);
            }



            AppendBuffer(ref intermediate, "Alternate Values");
            AppendBuffer(ref intermediate, Constants.API_EOL);
            if (altDens != null)
            {
                u = obDens.GivenUnit(out errorCode);
                if (errorCode != Error.NO_ERROR) u = null;
                ReportQuantity(
                                 false,
                                 EParameterName.API_ALT_DENS_NAME,
                                 altDens,
                                 false,
                                 u);
            }
            if (ctlBaseToAlt != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_CTL_BASE_TO_ALT_NAME,
                                 ctlBaseToAlt,
                                 false,
                                 null);

            }
            if (scaledCompAlt != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_SCALED_COMP_ALT_NAME,
                                 scaledCompAlt,
                                 false,
                                 null);
            }
            if (cplBaseToAlt != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_CPL_BASE_TO_ALT_NAME,
                                 cplBaseToAlt,
                                 false,
                                 null);

            }
            if (ctplBaseToAlt != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_CTPL_BASE_TO_ALT_NAME,
                                 ctplBaseToAlt,
                                 false,
                                 null);
                ReportQuantity(
                                 false,
                                 EParameterName.API_CTPL_BASE_TO_ALT_NAME,
                                 ctplBaseToAlt,
                                 true,
                                 null);

            }
            if (altVol != null)
            {
                ReportQuantity(
                                 false,
                                EParameterName.API_ALT_VOL_NAME,
                                altVol,
                                false,
                                null);
            }
            if (obVol != null)
            {
                ReportQuantity(
                                 false,
                                 EParameterName.API_OB_VOL_NAME,
                                 obVol,
                                 false,
                                 null);
            }
        }

        public bool CheckLimits(out Error errorCode)
        {
            bool tmp;
            int c1, c2;

            errorCode = Error.NO_ERROR;

            /** Check that either commodity or alpha60 is supplied **/
            /** Check that they are not both non-null  */
            if (commodityName != ApiOilProduct.EProductNumber.API_COMMODITY_NOT_GIVEN &&
                alpha60 != null)
            {
                errorCode = Error.COMMODITY_AND_ALPHA60_SUPPLIED;
                return false;
            }
            /**  At least one is supplied */
            if (commodityName == ApiOilProduct.EProductNumber.API_COMMODITY_NOT_GIVEN &&
                alpha60 == null)
            {
                errorCode = Error.COMMODITY_AND_ALPHA60_NULL;
                return false;
            }
            /**  If alpha60 is supplied, make sure it is in range */
            if (alpha60 != null)
            {
                tmp = alpha60.InAllowedRange(out errorCode);
                if (tmp == false || errorCode != Error.NO_ERROR)
                {
                    errorCode = Error.ALPHA60_OUT_OF_RANGE;
                    return false;
                }
            }


            /**  Ensure that an observed density is given */
            if (obDens == null)
            {
                errorCode = Error.VCFOBSERVED_DENSITY_VALUE_MISSING;
                return false;
            }
            /**  Check that the  commodity values are correct */
            if (CALC_REQ_COMMODITY != null)
            {
                c1 = CALC_REQ_BASE_TEMP.CompareTo(
                            obTemp,
                            out errorCode);
                if (errorCode != Error.NO_ERROR) return false;
                c2 = CALC_REQ_BASE_PRES.CompareTo(
                            obPres,
                            out errorCode);
                if (errorCode != Error.NO_ERROR) return false;
                if (c1 == 0 && c2 == 0)
                {     /* Type I calculation */
                    tmp = obDens.InRhoRange(CALC_REQ_COMMODITY, false, out errorCode);
                    if (tmp == false || errorCode != Error.NO_ERROR)
                    {
                        errorCode = Error.VCFOBSERVED_DENSITY_OUT_OF_RANGE_TYPE_I;
                        return false;
                    }
                }
                else
                {
                    tmp = obDens.InRhoRange(CALC_REQ_COMMODITY, true, out errorCode);
                    if (tmp == false || errorCode != Error.NO_ERROR)
                    {
                        errorCode = Error.VCFOBSERVED_DENSITY_OUT_OF_RANGE_TYPE_II;
                        return false;
                    }
                }
            }

            /** Check that temperature and pressure are in range */
            /**  Observed and alternate pressure are in range */
            if (obPres == null)
            {
                errorCode = Error.VCFOBSERVED_PRESSURE_VALUE_MISSING;
                return false;
            }
            else
            {
                tmp = obPres.InAllowedRange(out errorCode);
                if (tmp == false || errorCode != Error.NO_ERROR)
                {
                    errorCode = Error.VCFOBSERVED_PRESSURE_OUT_OF_RANGE;
                    return false;
                }
            }
            if (altPres == null)
            {
                errorCode = Error.VCFALTERNATE_PRESSURE_VALUE_MISSING;
                return false;
            }
            else
            {
                tmp = altPres.InAllowedRange(out errorCode);
                if (tmp == false || errorCode != Error.NO_ERROR)
                {
                    errorCode = Error.VCFALTERNATE_PRESSURE_OUT_OF_RANGE;
                    return false;
                }
            }



            /**  Observed and alternate temperature are in range */
            if (obTemp == null)
            {
                errorCode = Error.VCFOBSERVED_TEMPERATURE_VALUE_MISSING;
                return false;
            }
            else
            {
                tmp = obTemp.InAllowedRange(out errorCode);
                if (tmp == false || errorCode != Error.NO_ERROR)
                {
                    errorCode = Error.VCFOBSERVED_TEMPERATURE_OUT_OF_RANGE; ;
                    return false;
                }
            }
            if (altTemp == null)
            {
                errorCode = Error.VCFALTERNATE_TEMPERATURE_VALUE_MISSING;
                return false;
            }
            else
            {
                tmp = altTemp.InAllowedRange(out errorCode);
                if (tmp == false || errorCode != Error.NO_ERROR)
                {
                    errorCode = Error.VCFALTERNATE_TEMPERATURE_OUT_OF_RANGE;
                    return false;
                }
            }

            /*  Make sure only one of the volumes is supplied */
            c1 = 0;
            if (obVol != null) c1++;
            if (altVol != null) c1++;
            if (baseVol != null) c1++;
            if (c1 > 1)
            {
                errorCode = Error.VCFMORE_THAN_ONE_VOLUME_SUPPLIED;
                return false;
            }

            return true;
        }

        public Error TransformBaseToDesired(bool substep, ref double CTLReturn, ref double CPLReturn)
        {
            Error errorCode = Error.NO_ERROR;
            int c1, c2;
            double rhostar = 0;
            double dAlpha60 = 0;
            double rho60;
            double[] k = new double[3];
            double tstar = 0;
            double deltat, ctl, cpl, ctpl, fp, density;
            double[] fpStep6 = Constants.API_FP_STEP_6_FACTOR;


            if (substep == false)
            {    /** Set the result values that are identical to the 
                                 input values */
                c1 = CALC_REQ_BASE_TEMP.CompareTo(
                             altTemp,
                             out errorCode);
                if (errorCode != 0) return errorCode;
                c2 = CALC_REQ_BASE_PRES.CompareTo(
                            altPres,
                             out errorCode);
                if (errorCode != 0) return errorCode;
                if (c1 == 0 && c2 == 0)
                {
                    /** Set the alternate density to the base density */
                    errorCode = altDens.SetValueFromParent(
                                       baseDens);
                    if (errorCode != 0) return errorCode;

                    /** Set base to alternate expansion factors to 1.0 */
                    errorCode = ctlBaseToAlt.SetValue(
                                 1.0,
                                 ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != 0) return errorCode;
                    errorCode = cplBaseToAlt.SetValue(
                                 1.0,
                                 ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != 0) return errorCode;
                    errorCode = ctplBaseToAlt.SetValue(
                                 1.0,
                                 ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != 0) return errorCode;
                    errorCode = scaledCompAlt.SetValueFromParent(
                                        scaledCompOb);
                    return errorCode;
                }
            }

            /** Get the base density value and determine the current commodity to use */
            rho60 = baseDens.GetValue(
                          ApiUnit.ApiUnit_Density_KGM3(),
                          false,
                          out errorCode);
            if (errorCode != 0) return errorCode;
            if (CALC_REQ_COMMODITY != null && CALC_REQ_DET_COM == null)
            {
                CALC_REQ_DET_COM = ApiOilProduct.GetProductByDensity(commodityName,
                                          obDens,
                                          out errorCode);
                if (errorCode != 0) return errorCode;
            }

            /**  Step 2:  shift temperature to IPTS-68 basis */
            if (substep == true)
            {     /** Only shift the temperature if not already shifted */
                bool result;
                result = obTemp.WasIPTS68Corrected(out errorCode);
                if (errorCode != 0) return errorCode;
                if (result == false)
                {
                    obTemp.ConvertToIPTS68(out errorCode);
                    if (errorCode != 0) return errorCode;
                }
                tstar = obTemp.GetValue(
                              ApiUnit.ApiUnit_Temperature_F(),
                              false,
                              out errorCode);
                if (errorCode != 0) return errorCode;
            }
            else
            {
                bool result;
                result = altTemp.WasIPTS68Corrected(out errorCode);
                if (errorCode != 0) return errorCode;
                if (result == false)
                {
                    altTemp.ConvertToIPTS68(out errorCode);
                    if (errorCode != 0) return errorCode;
                }
                tstar = altTemp.GetValue(
                              ApiUnit.ApiUnit_Temperature_F(),
                              false,
                              out errorCode);
                if (errorCode != 0) return errorCode;
                ReportQuantityValue(
                                  true,
                                  "t corrected to IPTS-68 (in F)",
                                  tstar);
            }

            /**  Step 3: Shift input rho-60 to IPTS-68 basis */
            if (this.alpha60 != null)
            {
                dAlpha60 = this.alpha60.GetValue(
                               ApiUnit.ApiUnit_ThermalExp_REV_F(),
                               false,
                               out errorCode);
                if (errorCode != 0) return errorCode;
                rhostar = 1.0 + 0.4 * dAlpha60 * Constants.API_DELTA_60;
                rhostar *= 0.5 * dAlpha60 * Constants.API_DELTA_60;
                rhostar = rho60 * Math.Exp(rhostar);
                ReportQuantityValue(true, "Rho60*", rhostar);
            }
            else
            {
                double a, b;

                errorCode = CALC_REQ_DET_COM.GetKValues(out k);
                if (errorCode != 0) return errorCode;

                ReportQuantityValue(true, "K0", k[0]);
                ReportQuantityValue(true, "K1", k[1]);
                ReportQuantityValue(true, "K2", k[2]);
                a = k[1] + k[0] / rho60;
                a /= rho60;
                a += k[2];
                a *= Constants.API_DELTA_60 / 2.0;
                ReportQuantityValue(true, "A", a);

                b = k[0] + (k[1] + k[2] * rho60) * rho60;
                b = (2.0 * k[0] + k[1] * rho60) / b;
                ReportQuantityValue(true, "B", b);

                rhostar = a * (1.0 + 0.8 * a);
                rhostar = Math.Exp(rhostar) - 1.0;
                rhostar /= 1.0 + a * (1.0 + 1.6 * a) * b;
                rhostar = rho60 * (1.0 + rhostar);
                ReportQuantityValue(true, "Rho60*", rhostar);
            }

            /** Step 4: Calculate alpha-60 if not supplied */
            if (this.alpha60 == null)
            {
                dAlpha60 = k[0] / rhostar + k[1];
                dAlpha60 /= rhostar;
                dAlpha60 += k[2];
                if (alpha60 != null)
                {
                    errorCode = alpha60.SetValue(
                                       dAlpha60,
                                     ApiUnit.ApiUnit_ThermalExp_REV_F());
                    if (errorCode != 0) return errorCode;
                }
                else
                {
                    alpha60 = ApiAlpha60.Init(dAlpha60,
                                   ApiUnit.ApiUnit_ThermalExp_REV_F(),
                                   true,
                                   out errorCode);
                    if (errorCode != 0) return errorCode;
                }
                ReportQuantity(
                                 true,
                                 EParameterName.API_ALPHA_60_NAME,
                                 alpha60,
                                 false,
                                 ApiUnit.ApiUnit_ThermalExp_REV_F());
            }

            /**  Step 5: Calculcate the CTL */
            deltat = tstar - Constants.API_IPTS_68_BASE;
            ReportQuantityValue(true, "delta t", deltat);
            ctl = deltat + Constants.API_DELTA_60;
            ctl = 1.0 + 0.8 * dAlpha60 * ctl;
            ctl = dAlpha60 * deltat * ctl;
            ctl = Math.Exp(-ctl);
            errorCode = ctlBaseToAlt.SetValue(
                             ctl,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != 0) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_CTL_BASE_TO_ALT_NAME,
                             ctlBaseToAlt,
                             false,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());

            /** Step 6: Calculate the scale compressibility factor */
            fp = fpStep6[2] + fpStep6[3] * tstar;
            fp /= rhostar * rhostar;
            fp += fpStep6[1] * tstar;
            fp += fpStep6[0];
            fp = Math.Exp(fp);
            errorCode = scaledCompAlt.SetValue(
                              fp,
                              ApiUnit.ApiUnit_ScaledComp_REV_PSI());
            if (errorCode != 0) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_SCALED_COMP_ALT_NAME,
                             scaledCompAlt,
                             false,
                             ApiUnit.ApiUnit_ScaledComp_REV_PSI());


            /**  Step 7: Calculate the CPL factor */
            if (substep == false)
            {
                cpl = altPres.GetValue(
                            ApiUnit.ApiUnit_Pressure_PSI(),
                            false,
                            out errorCode);
            }
            else
            {
                cpl = obPres.GetValue(
                            ApiUnit.ApiUnit_Pressure_PSI(),
                            false,
                            out errorCode);
            }
            if (errorCode != 0) return errorCode;
            cpl = 1.0 - 1e-5 * fp * cpl;
            cpl = 1.0 / cpl;
            errorCode = cplBaseToAlt.SetValue(
                              cpl,
                              ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != 0) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_CPL_BASE_TO_ALT_NAME,
                             cplBaseToAlt,
                             false,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());

			/** Step 8: Calculate CTPL */
			CTLReturn = ctl;
			CPLReturn = cpl;
			ctpl = cpl * ctl;
            errorCode = ctplBaseToAlt.SetValue(
                              ctpl,
                              ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != 0) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_CTPL_BASE_TO_ALT_NAME,
                             ctplBaseToAlt,
                             false,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());

            ////SRM -- This was a hack to force vcf to rounded
            //ReportQuantity(
            //     true,
            //     EParameterName.API_CTPL_BASE_TO_ALT_NAME,
            //     ctplBaseToAlt,
            //     true,
            //     ApiUnit.ApiUnit_Expansion_DIMLESS());
            //ctpl = ctplBaseToAlt.GetValue(ApiUnit.ApiUnit_Expansion_DIMLESS(), true, out errorCode);
            //if (errorCode != 0) return errorCode;
            //errorCode = ctplBaseToAlt.SetValue(
            //      ctpl,
            //      ApiUnit.ApiUnit_Expansion_DIMLESS());
            //if (errorCode != 0) return errorCode;
            ////SRM -- This was a hack to force vcf to rounded

            /**  Step 9: Calculate density and volume */
            density = ctl * cpl * rho60;
            errorCode = altDens.SetValue(
                              density,
                              ApiUnit.ApiUnit_Density_KGM3());
            if (errorCode != 0) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_ALT_DENS_NAME,
                             altDens,
                             false,
                             ApiUnit.ApiUnit_Density_KGM3());

            ////SRM -- this was just for visibility into the alt density
            //ReportQuantity(
            //      true,
            //      EParameterName.API_ALT_DENS_NAME,
            //      altDens,
            //      false,
            //      ApiUnit.ApiUnit_Density_API());
            ////SRM -- this was just for visibility into the alt density

            return errorCode;
        }

        public Error CheckConversion(Error desiredError)
        {
            Error errorCode = desiredError;

            /* Check whether we are calculation for Refined products */
            if (commodityName == ApiOilProduct.EProductNumber.API_REFINED_PRODUCTS_NAME)
            {
                double[] limits = Constants.API_TRANSITION_ZONE_DENSITY_LIMITS;
                double value;
                Error error;

                /* Get the current base value */
                value = baseDens.GetValue(
                              ApiUnit.ApiUnit_Density_KGM3(),
                             false,
                             out error);
                if (error != Error.NO_ERROR) return errorCode;

                /* Compare whether we are close to the limits of the Transition Zone */
                if (Math.Abs(value - limits[0]) < Constants.API_CONVERSION_NOTE_11161_CUTOFF ||
                    Math.Abs(value - limits[1]) < Constants.API_CONVERSION_NOTE_11161_CUTOFF)
                {

                    comments = Constants.API_CONVERGENCE_NOTE_11162;

                    /* Return 0 to indicate that we have an exception to convergence failure */
                    errorCode = Error.NO_ERROR;
                }

            }

            /* No exception, convergence failed, return the desired errorCode */
            return errorCode;
        }

        public Error TransformToBaseCondition()
        {
            int c1, c2;
            Error errorCode = Error.NO_ERROR;
            int m = 0;
            double em, dtm, dpm, deltaT, deltaR;
            double rho60;
            double unshiftedT0;
            bool alphaSupplied = false;
            ApiDensity[] limits;
            double value;
            bool tmp;
            double[] dpStepfactor = Constants.API_DP_STEP_5_FACTOR;
			double CTLReturn = 0.0, CPLReturn = 0.0;	// bds new vars


            c1 = CALC_REQ_BASE_TEMP.CompareTo(
                            obTemp,
                            out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            c2 = CALC_REQ_BASE_PRES.CompareTo(
                            obPres,
                            out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            if (c1 == 0 && c2 == 0)
            {
                /** Set the base  density to the observed density */
                errorCode = baseDens.SetValueFromParent(
                                   obDens);
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Set observered to base expansion factors to 1.0 */
                errorCode = ctlObToBase.SetValue(1.0,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = cplObToBase.SetValue(
                             1.0,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = ctplObToBase.SetValue(
                             1.0,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
                if (errorCode != Error.NO_ERROR) return errorCode;
                return errorCode;
            }

            /** 
            * Check whether alpha60 is supplied. 
            * We need to persist this between calls to 
            * transformBaseToDesired() as that method calculates 
            * and intermediate alpha60. 
            */
            if (alpha60 != null) alphaSupplied = true;

            /**
            * The temperature uncorrected for IPTS-68 needs to be
            * persisted, as a call to transformBaseToDesired()  will
            * shift the obTemp value. However, The 11.1.6.2 calculations 
            * need the uncorrected value.
            */
            unshiftedT0 = obTemp.GetValue(
                               ApiUnit.ApiUnit_Temperature_F(),
                               false,
                               out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;

            /**
            *   Step 2: Initialize the value for rho_60(m), i.e. calcReq.baseDens.
            *           The observed density is used as the starting value.
            *           If the commodity and not alpha60 is supplied, we need to
            *           restrict the initial rho60(m) so it stays within allowed 
            *           limits.
            */
            errorCode = baseDens.SetValueFromParent(
                                   obDens);
            if (errorCode != Error.NO_ERROR) return errorCode;

            if (alphaSupplied == false)
            {
                tmp = CALC_REQ_COMMODITY.IsInRange(
                               baseDens,
                               out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                if (tmp == false)
                {
                    limits = CALC_REQ_COMMODITY.GetLimits(out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    c1 = limits[0].CompareTo(
                                   baseDens,
                                   out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;

                    if (c1 > 0)
                    {
                        errorCode = baseDens.SetValueFromParent(
                                           limits[0]);
                        if (errorCode != Error.NO_ERROR) return errorCode;
                    }
                    c1 = limits[1].CompareTo(
                                   baseDens,
                                   out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    if (c1 < 0)
                    {
                        errorCode = baseDens.SetValueFromParent(
                                           limits[1]);
                        if (errorCode != Error.NO_ERROR) return errorCode;
                    }
                }
            }

            /**  Start the iteration */
            while (true)
            {
                if (reportIntermediate)
                {
                    AppendBuffer(ref intermediate,
                                   "Step ");
                    AppendDouble(ref intermediate,
                                       m + 1);
                    AppendBuffer(ref intermediate,
                                       Constants.API_EOL);
                }
                value = baseDens.GetValue(
                                ApiUnit.ApiUnit_Density_KGM3(),
                                false,
                                out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                ReportQuantityValue(true, "Rho60(m) in kg/m^3", value);

                /**
                *  Step 3: Perform a 11.1.6.1 calculation
                *          If the commodity is "Refined Products", the 
                *           commodity may change during iteration.
                *           We determine it here once(!) per iteration
                *           and save it as detCommodity
                */
                if (alphaSupplied == false)
                {
                    ApiOilProduct.EProductNumber c1Prime = CALC_REQ_COMMODITY.ProductName(out errorCode);
                    c1 = (int)c1Prime;
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    CALC_REQ_DET_COM = ApiOilProduct.GetProductByDensity(c1Prime,
                                              baseDens,
                                              out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    alpha60 = null;
                }
                errorCode = TransformBaseToDesired(true,ref CTLReturn,ref CPLReturn);
                if (errorCode != Error.NO_ERROR) return errorCode;

                /**  Step 4: Check whether we need to terminate the iteration */
                value = obDens.GetValue(
                    ApiUnit.ApiUnit_Density_KGM3(),
                                 false,
                                 out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                deltaR = altDens.GetValue(ApiUnit.ApiUnit_Density_KGM3(),
                                  false,
                                  out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                deltaR = value - deltaR;
                ReportQuantityValue(
                                      true,
                                      "delta Rho60 in kg/m^3",
                                      deltaR);
                if (Math.Abs(deltaR) < Constants.API_CONVERSION_CRITERIA) break;

                /**  Step 5: Calculate the new value for rho_60(m) */
                /* determine em  */
                em = obDens.GetValue(
                              ApiUnit.ApiUnit_Density_KGM3(),
                              false,
                              out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                value = ctplBaseToAlt.GetValue(
                                 ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                 false,
                                 out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                em /= value;
                value = baseDens.GetValue(
                                 ApiUnit.ApiUnit_Density_KGM3(),
                                 false,
                                 out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                em -= value;
                ReportQuantityValue(true, "E(m)", em);

                /**  determine dpm */
                dpm = dpStepfactor[0] + dpStepfactor[1] * unshiftedT0;
                value = cplBaseToAlt.GetValue(
                                 ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                 false,
                                 out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                dpm *= 2.0 * value;
                value = obPres.GetValue(
                                 ApiUnit.ApiUnit_Pressure_PSI(),
                                 false,
                                 out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                dpm *= value;
                value = scaledCompAlt.GetValue(
                                 ApiUnit.ApiUnit_ScaledComp_REV_PSI(),
                                 false,
                                 out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                dpm *= value;
                value = baseDens.GetValue(
                                 ApiUnit.ApiUnit_Density_KGM3(),
                                 false,
                                 out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                dpm /= value * value;
                dpm *= -1.0;
                ReportQuantityValue(true, "Dp(m)", dpm);

                /**  determine dtm - but only if alpha_60 was not supplied */
                if (alphaSupplied == true) dtm = 0;
                else
                {
                    deltaT = unshiftedT0 - 60.0;
                    value = alpha60.GetValue(
                                     ApiUnit.ApiUnit_ThermalExp_REV_F(),
                                     false,
                                     out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    dtm = 1.0 + 1.6 * deltaT * value;
                    dtm *= deltaT * value;
                    dtm *= CALC_REQ_DET_COM.GetDalpha(out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
                ReportQuantityValue(true, "Dt(m)", dtm);

                /**  determine deltaR and the new value for rho(m) */
                deltaR = em / (1.0 + dtm + dpm);
                ReportQuantityValue(true, "delta Rho(m)", deltaR);
                value = baseDens.GetValue(
                                  ApiUnit.ApiUnit_Density_KGM3(),
                                  false,
                                  out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                rho60 = value + deltaR;

                /**  check for special cases in Newton's method */
                if (alphaSupplied == false)
                {
                    limits = CALC_REQ_COMMODITY.GetLimits(out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    value = limits[0].GetValue(
                                     ApiUnit.ApiUnit_Density_KGM3(),
                                     false,
                                     out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    if (rho60 < value)
                    {
                        rho60 = value;
                        if (m >= Constants.API_ITERATION_STEPS - 1)
                        {
                            errorCode = Error.VCFITERATION_DENSITY_VALUE_OUT_OF_RANGE;
                            return errorCode;
                        }
                    }
                    value = limits[1].GetValue(
                                     ApiUnit.ApiUnit_Density_KGM3(),
                                     false,
                                     out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    if (rho60 > value)
                    {
                        rho60 = value;
                        if (m >= Constants.API_ITERATION_STEPS - 1)
                        {
                            errorCode = CheckConversion(
                                 Error.VCFITERATION_DENSITY_VALUE_OUT_OF_RANGE);
                            if (errorCode == 0) break;
                            else return errorCode;
                        }
                    }
                }

                /**  initialize for the next iteration */
                errorCode = baseDens.SetValue(
                              rho60,
                              ApiUnit.ApiUnit_Density_KGM3());
                if (errorCode != Error.NO_ERROR) return errorCode;

                /**  Step 6: Increment the counter */
                m++;
                if (m >= Constants.API_ITERATION_STEPS - 1)
                {   /* Procedure is not converging */
                    errorCode = CheckConversion(Error.VCFCONVERGENCE_NOT_REACHED);
                    if (errorCode == 0) break;
                    else return errorCode;
                }
            }
            if (reportIntermediate)
            {
                AppendBuffer(ref intermediate,
                               "Convergence reached");
                AppendBuffer(ref intermediate,
                               Constants.API_EOL);
            }
            ReportQuantity(
                             true,
                             EParameterName.API_BASE_DENS_NAME,
                             baseDens,
                             false,
                             ApiUnit.ApiUnit_Density_KGM3());

            /**  
             * Set the intermediate values to the values
             * calculated by the 11.1.6.2 procedure
             */
            value = ctlBaseToAlt.GetValue(
                         ApiUnit.ApiUnit_Expansion_DIMLESS(),
                         false,
                         out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            errorCode = ctlObToBase.SetValue(
                             value,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != Error.NO_ERROR) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_CTL_OB_TO_BASE_NAME,
                             ctlObToBase,
                             false,
                             null);


            value = cplBaseToAlt.GetValue(
                         ApiUnit.ApiUnit_Expansion_DIMLESS(),
                         false,
                         out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            errorCode = cplObToBase.SetValue(
                             value,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != Error.NO_ERROR) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_CPL_OB_TO_BASE_NAME,
                             cplObToBase,
                             false,
                             null);

            value = ctplBaseToAlt.GetValue(
                         ApiUnit.ApiUnit_Expansion_DIMLESS(),
                         false,
                         out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            errorCode = ctplObToBase.SetValue(
                             value,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != Error.NO_ERROR) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_CTPL_OB_TO_BASE_NAME,
                             ctplObToBase,
                             false,
                             null);


            value = scaledCompAlt.GetValue(
                         ApiUnit.ApiUnit_ScaledComp_REV_PSI(),
                         false,
                         out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            errorCode = scaledCompOb.SetValue(
                             value,
                             ApiUnit.ApiUnit_ScaledComp_REV_PSI());
            if (errorCode != Error.NO_ERROR) return errorCode;
            ReportQuantity(
                             true,
                             EParameterName.API_SCALED_COMP_OB_NAME,
                             scaledCompOb,
                             false,
                             null);

            if (reportIntermediate)
            {
                AppendBuffer(ref intermediate,
                               "Correct to 60 F and 0 psi");
                AppendBuffer(ref intermediate,
                               Constants.API_EOL);
            }
            return errorCode;
        }

        public Error DoCalculation(bool transformToBase, ref double CTLReturn, ref double CPLReturn)
        {
            Error errorCode = 0;
            double v1, v2;
            bool tmp;

			CTLReturn = 0.0;
			CPLReturn = 0.0;

			CALC_REQ_COMMODITY = null;
            CALC_REQ_DET_COM = null;

            /** Determine the commodity if calcReq.alpha60 is not given */
            if (alpha60 == null &&
                commodityName != ApiOilProduct.EProductNumber.API_COMMODITY_NOT_GIVEN)
            {
                CALC_REQ_COMMODITY = ApiOilProduct.GetProductByName(commodityName,
                                        out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (transformToBase)
            {
                /**  first check that all input parameters are in range */
                tmp = CheckLimits(out errorCode);
                if (tmp == false || errorCode != Error.NO_ERROR)
                {
                    return errorCode;
                }
                if (reportIntermediate)
                {
                    AppendBuffer(ref intermediate,
                                   "All values are in range ");
                    AppendBuffer(ref intermediate,
                                   Constants.API_EOL);
                }

                errorCode = InitializeRequest();
                if (errorCode != Error.NO_ERROR) return errorCode;

                /* 
            *  First we need to calculate all values to base condition.
                *  The procedure 11.1.6.2 is used for this 
                */
                errorCode = TransformToBaseCondition();
                if (errorCode != Error.NO_ERROR) return errorCode;
            }


            /* 
            * Now we need to run procedure 11.1.6.1 to transform to
            * alternate conditions 
            */
            errorCode = TransformBaseToDesired(false, ref CTLReturn, ref CPLReturn);
			if (errorCode != Error.NO_ERROR) return errorCode;


            /**  Calculate the final correction factors */
            /** ctlAlt  = ctlBaseToAlt / ctlObToBase */
            v1 = ctlBaseToAlt.GetValue(
                           ApiUnit.ApiUnit_Expansion_DIMLESS(),
                       false,
                           out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            v2 = ctlObToBase.GetValue(
                           ApiUnit.ApiUnit_Expansion_DIMLESS(),
                           false,
                           out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            v1 = v1 / v2;
            errorCode = ctlAlt.SetValue(
                             v1,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != Error.NO_ERROR) return errorCode;

            /** cplAlt  = cplBaseToAlt / cplObToBase */
            v1 = cplBaseToAlt.GetValue(
                           ApiUnit.ApiUnit_Expansion_DIMLESS(),
                           false,
                           out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            v2 = cplObToBase.GetValue(
                           ApiUnit.ApiUnit_Expansion_DIMLESS(),
                           false,
                           out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            v1 = v1 / v2;
            errorCode = cplAlt.SetValue(
                             v1,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != Error.NO_ERROR) return errorCode;

            /** ctplAlt = cplAlt * ctlAlt */
            v1 = ctlAlt.GetValue(
                           ApiUnit.ApiUnit_Expansion_DIMLESS(),
                           false,
                           out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            v2 = cplAlt.GetValue(
                           ApiUnit.ApiUnit_Expansion_DIMLESS(),
                           false,
                           out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            v1 = v1 * v2;
            errorCode = ctplAlt.SetValue(
                             v1,
                             ApiUnit.ApiUnit_Expansion_DIMLESS());
            if (errorCode != Error.NO_ERROR) return errorCode;

            return errorCode;
        }

        public Error CalculationResult(ref double CTLReturn, ref double CPLReturn)
        {
            ApiUnit u;
            Error errorCode = 0;
            int c1, c2;
            double value;

            ReportInputData();


            /** 
             * Check that the base values are set 
             */
            if (CALC_REQ_BASE_TEMP == null)
            {
				CALC_REQ_BASE_TEMP = ApiTemperature.Init(Constants.API_BASE_TEMP,
                                 ApiUnit.ApiUnit_Temperature_F(),
                                 false,
                                 out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            if (CALC_REQ_BASE_PRES == null)
            {
                CALC_REQ_BASE_PRES = ApiPressure.Init(Constants.API_BASE_PRES,
                                  ApiUnit.ApiUnit_Pressure_PSI(),
                                  false,
                                  out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }


            /**  
             *  If the base conditions are 60 F and 0 psi, 
             *  no extra calculation is needed
             */
            c1 = baseTemp.CompareTo(
                           CALC_REQ_BASE_TEMP,
                           out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            c2 = basePres.CompareTo(
                           CALC_REQ_BASE_PRES,
                           out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            if (c1 == 0 && c2 == 0)
            {
                errorCode = DoCalculation(true, ref CTLReturn, ref CPLReturn);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }


            else
            {   /**  We need to transform to the desired base condition */
                /**  intialize the intermediate  CalculationRequest object */
                if (reportIntermediate)
                {
                    AppendBuffer(ref intermediate,
                                   "Transform observed to alternate");
                    AppendBuffer(ref intermediate,
                                   Constants.API_EOL);
                }

                if (CALC_REQ_INTER == null)
                {
                    CALC_REQ_INTER = Init(out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }

                /** Transfer the values for the intermediate reporting buffer */
                CALC_REQ_INTER.intermediate = intermediate;
                CALC_REQ_INTER.reportIntermediate = reportIntermediate;

                /** Initialize all relevant quantities */
                errorCode = InitializeRequest();
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = CALC_REQ_INTER.InitializeRequest();
                if (errorCode != Error.NO_ERROR) return errorCode;

                /** Transfer the commodity or alpha60 information */
                CALC_REQ_INTER.commodityName = commodityName;
                if (alpha60 != null)
                {
                    if (CALC_REQ_INTER.alpha60 != null)
                    {
                        errorCode = CALC_REQ_INTER.alpha60.SetValueFromParent(
                                               alpha60);
                    }
                    else
                    {
                        u = alpha60.GivenUnit(out errorCode);
                        if (errorCode != Error.NO_ERROR) return errorCode;
                        value = alpha60.GetValue(
                                 u,
                                 false, out errorCode);
                        if (errorCode != Error.NO_ERROR) return errorCode;
                        CALC_REQ_INTER.alpha60 = ApiAlpha60.Init(value, u, true, out errorCode);
                    }
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
                else
                {
                    CALC_REQ_INTER.alpha60 = null;
                }


                /** Transfer the relevant values from calcReq */
                errorCode = CALC_REQ_INTER.obTemp.SetValueFromParent(
                                   obTemp);
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = CALC_REQ_INTER.obPres.SetValueFromParent(
                                   obPres);
                if (errorCode != Error.NO_ERROR) return errorCode;

                errorCode = CALC_REQ_INTER.altTemp.SetValueFromParent(
                                   altTemp);
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = CALC_REQ_INTER.altPres.SetValueFromParent(
                                   altPres);
                if (errorCode != Error.NO_ERROR) return errorCode;

                errorCode = CALC_REQ_INTER.obDens.SetValueFromParent(
                                   obDens);
                if (errorCode != Error.NO_ERROR) return errorCode;

				/**  do the calculation for the final values */
				CALC_REQ_INTER.CALC_REQ_BASE_TEMP = CALC_REQ_BASE_TEMP;
				CALC_REQ_INTER.CALC_REQ_BASE_PRES = CALC_REQ_BASE_PRES;
				errorCode = CALC_REQ_INTER.DoCalculation(true, ref CTLReturn, ref CPLReturn);

				if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = altDens.SetValueFromParent(
                                   CALC_REQ_INTER.altDens);
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = ctlAlt.SetValueFromParent(
                                   CALC_REQ_INTER.ctlAlt);
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = cplAlt.SetValueFromParent(
                                   CALC_REQ_INTER.cplAlt);
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = ctplAlt.SetValueFromParent(
                                   CALC_REQ_INTER.ctplAlt);
                if (errorCode != Error.NO_ERROR) return errorCode;
                errorCode = scaledCompAlt.SetValueFromParent(
                                   CALC_REQ_INTER.scaledCompAlt);
                if (errorCode != Error.NO_ERROR) return errorCode;


                /** now initialize for base conditions */
                c1 = baseTemp.CompareTo(
                           obTemp,
                           out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                c2 = basePres.CompareTo(
                           obPres,
                           out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;

                if (c1 != 0 || c2 != 0)
                {
                    /** do the calculation for the base conditions */
                    if (reportIntermediate)
                    {
                        AppendBuffer(ref intermediate,
                                           "Transform 60 F, 0 psi to base");
                        AppendBuffer(ref intermediate,
                                           Constants.API_EOL);
                    }
                    errorCode = CALC_REQ_INTER.altTemp.SetValueFromParent(
                                       baseTemp);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = CALC_REQ_INTER.altPres.SetValueFromParent(
                                       basePres);
                    if (errorCode != Error.NO_ERROR) return errorCode;

                    errorCode = CALC_REQ_INTER.DoCalculation(false, ref CTLReturn, ref CPLReturn);
                    if (errorCode != Error.NO_ERROR) return errorCode;

                    /** and set the parameters */
                    errorCode = baseDens.SetValueFromParent(
                                       CALC_REQ_INTER.altDens);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = scaledCompOb.SetValueFromParent(
                                                       CALC_REQ_INTER.scaledCompOb);
                    if (errorCode != Error.NO_ERROR) return errorCode;

                    value = CALC_REQ_INTER.ctlAlt.GetValue(
                                     ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                     false,
                                     out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    value = 1.0 / value;
                    errorCode = ctlObToBase.SetValue(
                                     value,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;

                    value = CALC_REQ_INTER.cplAlt.GetValue(
                                     ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                     false,
                                     out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    value = 1.0 / value;
                    errorCode = cplObToBase.SetValue(
                                     value,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;

                    value = CALC_REQ_INTER.ctplAlt.GetValue(
                                     ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                     false,
                                     out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    value = 1.0 / value;
                    errorCode = ctplObToBase.SetValue(
                                     value,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
                else
                {    /**   no calculation needed */
                    errorCode = baseDens.SetValueFromParent(
                                       obDens);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = ctlObToBase.SetValue(
                                     1.0,
                                    ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = cplObToBase.SetValue(
                                     1.0,
                                    ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = ctplObToBase.SetValue(
                                     1.0,
                                    ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = scaledCompOb.SetValue(
                                     1.0,
                                     ApiUnit.ApiUnit_ScaledComp_REV_PSI());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }


                /** Initialize for alternate conditions */
                c1 = altTemp.CompareTo(
                           baseTemp,
                           out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                c2 = altPres.CompareTo(
                           basePres,
                           out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
                if (c1 != 0 || c2 != 0)
                {
                    double f1, f2;

                    if (reportIntermediate)
                    {
                        AppendBuffer(ref intermediate,
                                           "Transform 60 F, 0 psi to alternate");
                        AppendBuffer(ref intermediate,
                                           Constants.API_EOL);
                    }
                    f1 = ctlObToBase.GetValue(
                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                    false,
                                    out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    f2 = ctlAlt.GetValue(
                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                    false,
                                    out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = ctlBaseToAlt.SetValue(
                                     f1 * f2,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    ReportQuantity(
                                         true,
                                         EParameterName.API_CTL_BASE_TO_ALT_NAME,
                                         ctlBaseToAlt,
                                         false,
                                         ApiUnit.ApiUnit_Expansion_DIMLESS());

                    f1 = cplObToBase.GetValue(
                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                    false,
                                    out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    f2 = cplAlt.GetValue(
                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                    false,
                                    out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = cplBaseToAlt.SetValue(
                                     f1 * f2,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    ReportQuantity(
                                         true,
                                         EParameterName.API_CPL_BASE_TO_ALT_NAME,
                                         cplBaseToAlt,
                                         false,
                                         ApiUnit.ApiUnit_Expansion_DIMLESS());

                    f1 = ctplObToBase.GetValue(
                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                    false,
                                    out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    f2 = ctplAlt.GetValue(
                                    ApiUnit.ApiUnit_Expansion_DIMLESS(),
                                    false,
                                    out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = ctplBaseToAlt.SetValue(
                                     f1 * f2,
                                     ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    ReportQuantity(
                                         true,
                                         EParameterName.API_CTPL_BASE_TO_ALT_NAME,
                                         ctplBaseToAlt,
                                         false,
                                         ApiUnit.ApiUnit_Expansion_DIMLESS());
                }
                else
                {
                    errorCode = ctlBaseToAlt.SetValue(
                                 1.0,
                                 ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = cplBaseToAlt.SetValue(
                                 1.0,
                                 ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                    errorCode = ctplBaseToAlt.SetValue(
                                 1.0,
                                 ApiUnit.ApiUnit_Expansion_DIMLESS());
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
            }

            /** Set the volume data and report the output data  */
            errorCode = GetVolumeData();
            if (errorCode != Error.NO_ERROR) return errorCode;
            ReportOutputData();

            return errorCode;
        }

        public Error PerformCalculation(ref double CTLReturn, ref double CPLReturn)
		{
            Error errorCode = CalculationResult(ref CTLReturn, ref CPLReturn);

            if (errorCode != 0) SetErrorStatus();
            return errorCode;
        }

        public Error SetParameters(
                         ApiOilProduct.EProductNumber commodity,
                         double dAlpha60, ApiUnit au,
                         double dObTemp, ApiUnit tuo,
                         double dAltTemp, ApiUnit tua,
                         double dObPres, ApiUnit puo,
                         double dAltPres, ApiUnit pua,
                         double dObDens, ApiUnit duo,
                         double dBaseTemp, ApiUnit tub,
                         double dBasePres, ApiUnit pub,

                         double dBaseVol, ApiUnit vub,
                         double dAltVol, ApiUnit vua,
                         double dObVol, ApiUnit vuo)
        {
            Error errorCode;


            commodityName = commodity;
            if (au == null)
            {
                alpha60 = null;
            }
            else
            {
                if (alpha60 != null)
                {
                    errorCode = alpha60.SetValue(dAlpha60, au);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
                else
                {
                    alpha60 = ApiAlpha60.Init(dAlpha60, au, true, out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
            }


            if (obTemp != null)
            {
                errorCode = obTemp.SetValue( dObTemp, tuo);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            else
            {
                obTemp = ApiTemperature.Init(dObTemp, tuo, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (altTemp != null)
            {
                errorCode = altTemp.SetValue( dAltTemp, tua);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            else
            {
                altTemp = ApiTemperature.Init(dAltTemp, tua, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (obPres != null)
            {
                errorCode = obPres.SetValue( dObPres, puo);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            else
            {
                obPres = ApiPressure.Init(dObPres, puo, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (altPres != null)
            {
                errorCode = altPres.SetValue( dAltPres, pua);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            else
            {
                altPres = ApiPressure.Init(dAltPres, pua, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (obDens != null)
            {
                errorCode = obDens.SetValue( dObDens, duo);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            else
            {
                obDens = ApiDensity.Init(dObDens, duo, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (baseTemp != null)
            {
                errorCode = baseTemp.SetValue( dBaseTemp, tub);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            else
            {
                baseTemp = ApiTemperature.Init(dBaseTemp, tub, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (basePres != null)
            {
                errorCode = basePres.SetValue( dBasePres, pub);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }
            else
            {
                basePres = ApiPressure.Init(dBasePres, pub, true, out errorCode);
                if (errorCode != Error.NO_ERROR) return errorCode;
            }

            if (vub == null)
            {
                baseVol = null;
            }
            else
            {
                if (baseVol != null)
                {
                    errorCode = baseVol.SetValue( dBaseVol, vub);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
                else
                {
                    baseVol = ApiVolume.Init(dBaseVol, vub, true, out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
            }

            if (vua == null)
            {
                altVol = null;
            }
            else
            {
                if (altVol != null)
                {
                    errorCode = altVol.SetValue( dAltVol, vua);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
                else
                {
                    altVol = ApiVolume.Init(dAltVol, vua, true, out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
            }

            if (vuo == null)
            {
                obVol = null;
            }
            else
            {
                if (obVol != null)
                {
                    errorCode = obVol.SetValue( dObVol, vuo);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
                else
                {
                    obVol = ApiVolume.Init(dObVol, vuo, true, out errorCode);
                    if (errorCode != Error.NO_ERROR) return errorCode;
                }
            }
            return errorCode;
        }

        public Error Set11_1_6_1Parameters(
                             ApiOilProduct.EProductNumber commodity,
                             double alpha60, ApiUnit au,
                             double altTemp, ApiUnit tua,
                             double altPres, ApiUnit pua,
                             double baseDens, ApiUnit dub,

                               double baseVol, ApiUnit vub)
        {
            Error errorCode = Error.NO_ERROR;

            errorCode = SetParameters(
                                 commodity,
                                 alpha60, au,
                                 Constants.API_BASE_TEMP, ApiUnit.ApiUnit_Temperature_F(),
                                 altTemp, tua,
                                 Constants.API_BASE_PRES, ApiUnit.ApiUnit_Pressure_PSI(),
                                 altPres, pua,
                                 baseDens, dub,
                                 Constants.API_BASE_TEMP, ApiUnit.ApiUnit_Temperature_F(),
                                 Constants.API_BASE_PRES, ApiUnit.ApiUnit_Pressure_PSI(),
                                 baseVol, vub,
                                 0, null,
                                 0, null);
            return errorCode;
        }

        public Error Set11_1_6_2Parameters(
                             ApiOilProduct.EProductNumber commodity,
                             double alpha60, ApiUnit au,
                             double obTemp, ApiUnit tuo,
                             double obPres, ApiUnit puo,
                             double obDens, ApiUnit duo,

                             double obVol, ApiUnit vuo)
        {
            Error errorCode = Error.NO_ERROR;

            errorCode = this.SetParameters(
                                  commodity,
                                  alpha60, au,
                                  obTemp, tuo,
                                  Constants.API_BASE_TEMP, ApiUnit.ApiUnit_Temperature_F(),
                                  obPres, puo,
                                  Constants.API_BASE_PRES, ApiUnit.ApiUnit_Pressure_PSI(),
                                  obDens, duo,
                                                      Constants.API_BASE_TEMP, ApiUnit.ApiUnit_Temperature_F(),
                                  Constants.API_BASE_PRES, ApiUnit.ApiUnit_Pressure_PSI(),
                                  0, null,
                                  0, null,
                                  obVol, vuo);
            return errorCode;
        }

 
        public Error Set11_1_6_3Parameters(
                             ApiOilProduct.EProductNumber commodity,
                             double alpha60, ApiUnit au,
                             double obTemp, ApiUnit tuo,
                             double obPres, ApiUnit puo,
                             double obDens, ApiUnit duo,

                             double altTemp, ApiUnit tua,
                             double altPres, ApiUnit pua,

                             double obVol, ApiUnit vuo)
        {
            Error errorCode = Error.NO_ERROR;

            errorCode = SetParameters(
                                  commodity,
                                  alpha60, au,
                                  obTemp, tuo,
                                  altTemp, tua,
                                  obPres, puo,
                                  altPres, pua,
                                  obDens, duo,
                                                      Constants.API_BASE_TEMP, ApiUnit.ApiUnit_Temperature_F(),
                                  Constants.API_BASE_PRES, ApiUnit.ApiUnit_Pressure_PSI(),
                                  0, null,
                                  0, null,
                                  obVol, vuo);
            return errorCode;
        }

        /**
        * A convienence function to set parameters for a 11.1.7.1 type calculation.
*/
        public Error Set11_1_7_1Parameters(
                             ApiOilProduct.EProductNumber commodity,
                             double alpha60, ApiUnit au,
                             double altTemp, ApiUnit tua,
                             double altPres, ApiUnit pua,
                             double baseDens, ApiUnit dub,

                             double baseTemp, ApiUnit tub,

                             double baseVol, ApiUnit vub)
        {
            Error errorCode = Error.NO_ERROR;

            errorCode = SetParameters(
                                 commodity,
                                 alpha60, au,
                                 baseTemp, tub,
                                 altTemp, tua,
                                 0, ApiUnit.ApiUnit_Pressure_KPA(),
                                 altPres, pua,
                                 baseDens, dub,
                                 baseTemp, tub,
                                 0, ApiUnit.ApiUnit_Pressure_KPA(),
                                 baseVol, vub,
                                 0, null,
                                 0, null);
            return errorCode;
        }

        /**
        * A convienence function to set parameters for a 11.1.7.2  type calculation.
        * If the volume unit is null, the volume is not calculated
*/
        public Error Set11_1_7_2Parameters(
                             ApiOilProduct.EProductNumber commodity,
                             double alpha60, ApiUnit au,
                             double obTemp, ApiUnit tuo,
                             double obPres, ApiUnit puo,
                             double obDens, ApiUnit duo,

                             double baseTemp, ApiUnit tub,

                             double obVol, ApiUnit vuo)
        {
            Error errorCode = Error.NO_ERROR;

            errorCode = SetParameters(
                                  commodity,
                                  alpha60, au,
                                  obTemp, tuo,
                                  baseTemp, tub,
                                  obPres, puo,
                                  0, ApiUnit.ApiUnit_Pressure_KPA(),
                                  obDens, duo,
                                                      baseTemp, tub,
                                  0, ApiUnit.ApiUnit_Pressure_KPA(),
                                  0, null,
                                  0, null,
                                  obVol, vuo);
            return errorCode;
        }

    }
}
