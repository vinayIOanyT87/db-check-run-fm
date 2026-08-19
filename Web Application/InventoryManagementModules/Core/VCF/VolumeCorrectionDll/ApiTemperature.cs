using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiTemperature : ApiQuantity
    {
        protected static ApiTemperature[] AllowedTemp = null;

        public override double _GetValue(ApiUnit unit, bool rounded, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            /** Retrieve the temperature in F - convert if needed */
            if (unit == ApiUnit.ApiUnit_Temperature_F())
            {
                if (currentUnit == ApiUnit.ApiUnit_Temperature_F())
                {
                    result = currentValue;
                }
                else
                {
                    result = Constants.API_TEMP_C_TO_F_SLOPE * currentValue +
                              Constants.API_TEMP_C_TO_F_INTERCEPT;
                }
            }

            /** Retrieve the temperature in C - convert if needed */
            else if (unit == ApiUnit.ApiUnit_Temperature_C())
            {
                if (currentUnit == ApiUnit.ApiUnit_Temperature_C())
                {
                    result = currentValue;
                }
                else
                {
                    result = currentValue - Constants.API_TEMP_C_TO_F_INTERCEPT;
                    result /= Constants.API_TEMP_C_TO_F_SLOPE;
                }
            }

            /** Otherwise the unit is not valid */
            else
            {
                errorCode = Error.INVALID_UNIT;
            }

            /** The rounding is done in the ApiQuantity function */
            return result;
        }

        public override double RoundingIncrement(ApiUnit unit, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            /** Retrieve the temperature increment for F */
            if (unit == ApiUnit.ApiUnit_Temperature_F())
            {
                result = Constants.API_TEMP_F_ROUNDING_INCREMENT;
            }

            /** Retrieve the temperature  increment for C */
            else if (unit == ApiUnit.ApiUnit_Temperature_C())
            {
                result = Constants.API_TEMP_C_ROUNDING_INCREMENT;
            }

            /** Otherwise the unit is not valid */
            else
            {
                errorCode = Error.INVALID_UNIT;
            }

            return result;
        }

        public override bool InAllowedRange(out Error errorCode)
        {
            bool result = false;
            int compare;

            /** Initialize AllowedTemp if necessary */
            if (AllowedTemp == null)
            {
                double[] range = Constants.API_ALLOWED_TEMPERATURE_RANGE;
                int i;

                errorCode = Error.NO_ERROR;
                AllowedTemp = new ApiTemperature[2];

                for (i = 0; i < 2; i++)
                {
                    AllowedTemp[i] = Init(range[i],
                                       ApiUnit.ApiUnit_Temperature_F(),
                                       false,
                                       out errorCode);
                    if (errorCode != Error.NO_ERROR || AllowedTemp[i] == null)
                    {
                        errorCode = Error.INITIALIZE_FAILED;
                        return result;
                    }
                }
            }

            /** now compare the data */
            compare = CompareTo(
                            (ApiQuantity)AllowedTemp[0],
                            out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                return result;
            }
            if (compare == -1)
            {
                errorCode = Error.QUANTITY_OUT_OFF_RANGE;
                return result;
            }
            compare = CompareTo(
                            AllowedTemp[1],
                            out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                return result;
            }
            if (compare == +1)
            {
                errorCode = Error.QUANTITY_OUT_OFF_RANGE;
                return result;
            }

            /** Quantity is in range - return TRUE */
            result = true;
            return result;
        }

        public override ApiUnit GetStandardUnit()
        {
            return ApiUnit.ApiUnit_Temperature_F();
        }


        public static ApiTemperature Init(double value,
                              ApiUnit unit,
                              bool isMutable,
                              out Error errorCode)
        {
            ApiTemperature temp = null;

            errorCode = Error.NO_ERROR;

            /** Create the object itself */
            temp = new ApiTemperature();

            if (temp == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return temp;
            }

            /** And finally let the quant object initialize itself */
            errorCode = temp.Initialize(
                             value,
                             unit,
                             isMutable);

            /** And return the new object*/
            return temp;
        }

        public void ConvertToIPTS68(out Error errorCode)
        {
            double value, scaled, delta = 0, factor;
            double[] consts = Constants.API_TEMP_IPTS68_CONSTANTS;
            int i;

            if (wasCorrected != false || isMutable == false)
            {
                errorCode = Error.CHANGED_IMMUTABLE;
                return;
            }

            errorCode = Error.NO_ERROR;

            /* Step 1: Get temperature in C */
            value = GetValue(ApiUnit.ApiUnit_Temperature_C(), false, out errorCode);
            if (errorCode != Error.NO_ERROR) return;

            /* Step 3: Get the scaled value */
            scaled = value / Constants.API_TEMP_IPTS68_SCALER;

            /* Step 3 cont.: Caluclate correction factor Delta_t */
            factor = scaled;
            delta = 0;
            for (i = 0; i < 8; i++)
            {
                delta += consts[i] * factor;
                factor *= scaled;
            }

            /* Step 4: Correct the temperature value */
            value -= delta;

            /* Finish: Set the currentValue and currentUnit to the
           corrected values */
            currentValue = value;
            currentUnit = ApiUnit.ApiUnit_Temperature_C();
            wasCorrected = true;
        }


        public bool WasIPTS68Corrected(out Error errorCode)
        {
            bool result = false;

            errorCode = Error.NO_ERROR;

            result = wasCorrected;
            return result;
        }

        public override ApiUnit[] GetAcceptableUnits()
        {
            ApiUnit[] acceptableUnits = new ApiUnit[3];
            acceptableUnits[0] = ApiUnit.ApiUnit_Temperature_F();
            acceptableUnits[1] = ApiUnit.ApiUnit_Temperature_C();
            acceptableUnits[2] = null;
            return acceptableUnits;
        }

        public override string GetQuantityName()
        {
            return "Temperature";            
        }

    }
}
