using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiDensity : ApiQuantity
    {
        /** The  range of allowed Pressures */

        public override double _GetValue( ApiUnit unit, bool rounded,  out Error errorCode)
        {
            double result = 0, tmp;

            errorCode = Error.NO_ERROR;

            /** Retrieve the density in kg/m^3 - convert if needed */
            if (unit == ApiUnit.ApiUnit_Density_KGM3())
            {
                if (currentUnit == ApiUnit.ApiUnit_Density_KGM3())
                {
                    result = currentValue;
                }
                else
                {
                    result = ConvertToKGM3(currentValue, currentUnit);
                }
            }

            /** Retrieve the density in API - convert if needed */
            else if (unit == ApiUnit.ApiUnit_Density_API())
            {
                if (currentUnit == ApiUnit.ApiUnit_Density_API())
                {
                    result = currentValue;
                }
                else
                {
                    /** first convert to kg/m^3 */
                    result = ConvertToKGM3(currentValue, currentUnit);

                    /** and then convert to API */
                    tmp = Constants.API_DENSITY_RELATIVE_TO_KGM3_FACTOR *
                         Constants.API_DENSITY_API_TO_KGM3_UPPER;
                    result = tmp / result;
                    result -= Constants.API_DENSITY_API_TO_KGM3_LOWER;
                }
            }

            /** Retrieve the density in Relative Density - convert if needed */
            else if (unit == ApiUnit.ApiUnit_Density_RELATIVE())
            {
                if (currentUnit == ApiUnit.ApiUnit_Density_RELATIVE())
                {
                    result = currentValue;
                }
                else
                {
                    result = ConvertToKGM3(currentValue, currentUnit);
                    result /= Constants.API_DENSITY_RELATIVE_TO_KGM3_FACTOR;
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

            /** Retrieve the density increment for API */
            if (unit == ApiUnit.ApiUnit_Density_API())
            {
                result = Constants.API_DENSITY_API_ROUNDING_INCREMENT;
            }

            /** Retrieve the density increment for kg/m^3 */
            else if (unit == ApiUnit.ApiUnit_Density_KGM3())
            {
                result = Constants.API_DENSITY_KGM3_ROUNDING_INCREMENT;
            }

            /** Retrieve the density increment for Relative Density */
            else if (unit == ApiUnit.ApiUnit_Density_RELATIVE())
            {
                result = Constants.API_DENSITY_RELATIVE_ROUNDING_INCREMENT;
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
            errorCode = Error.UNSUPPORTED_FUNCTION;
            return false;
        }

        public static ApiDensity Init(double value, ApiUnit unit, bool isMutable, out Error errorCode)
        {
            ApiDensity temp = new ApiDensity();

            errorCode = Error.NO_ERROR;

            /** Create the object itself */
            if (temp == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return temp;
            }

            /** And finally let the quant object initialize itself */
            errorCode = temp.Initialize(value, unit, isMutable);

            /** And return the new object*/
            return temp;
        }

        public double ConvertToKGM3(double value,
                   ApiUnit unit)
        {
            double result = value;

            if (unit == ApiUnit.ApiUnit_Density_API())
            {
                result += Constants.API_DENSITY_API_TO_KGM3_LOWER;
                result = Constants.API_DENSITY_API_TO_KGM3_UPPER / result;
                result *= Constants.API_DENSITY_RELATIVE_TO_KGM3_FACTOR;
            }
            else if (unit == ApiUnit.ApiUnit_Density_RELATIVE())
            {
                result *= Constants.API_DENSITY_RELATIVE_TO_KGM3_FACTOR;
            }

            return result;
        }


        public bool InRhoRange(ApiOilProduct product, bool type2, out Error errorCode)
        {
            bool result = false;

            if (product == null)
            {
                errorCode = Error.NULL_POINTER_EXCEPTION;
                return result;
            }

            errorCode = 0;

            if (type2 == false)
            {    /* use strict limits */
                result = product.IsInRange(this, out errorCode);
                if (errorCode != Error.NO_ERROR)
                {
                    result = false;
                    return result;
                }
            }

            else
            {             /* lesser limits for Type II calculation */
                result = product.IsInRhoRange( this, out errorCode);
                if (errorCode != Error.NO_ERROR)
                {
                    result = false;
                    return result;
                }
            }

            if (result == false) errorCode = Error.QUANTITY_OUT_OFF_RANGE;

            return result;
        }

        public override ApiUnit GetStandardUnit()
        {
            return ApiUnit.ApiUnit_Density_KGM3();
        }

        public override string GetQuantityName()
        {
            return "Density";
        }

        public override ApiUnit[] GetAcceptableUnits()
        {
            ApiUnit[] acceptableUnits = new ApiUnit[4];
            acceptableUnits[0] = ApiUnit.ApiUnit_Density_KGM3();
            acceptableUnits[1] = ApiUnit.ApiUnit_Density_RELATIVE();
            acceptableUnits[2] = ApiUnit.ApiUnit_Density_API();
            acceptableUnits[3] = null;
            return acceptableUnits;
        }
    }
}
