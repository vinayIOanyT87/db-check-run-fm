using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;
namespace VCF
{
    public class ApiAlpha60 : ApiQuantity
    {
        /** The  range of allowed temperatures */
        protected static ApiAlpha60[] AllowedAlpha60;

        public override double _GetValue(
                       ApiUnit unit,
                       bool rounded,
                       out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            /** Retrieve the temperature in 1/F - convert if needed */
            if (unit == ApiUnit.ApiUnit_ThermalExp_REV_F())
            {
                if (currentUnit == ApiUnit.ApiUnit_ThermalExp_REV_F())
                {
                    result = currentValue;
                }
                else
                {
                    result = currentValue / Constants.API_THERMAL_EXPANSION_REVF_TO_REVC;
                }
            }

            /** Retrieve the temperature in C - convert if needed */
            else if (unit == ApiUnit.ApiUnit_ThermalExp_REV_C())
            {
                if (currentUnit == ApiUnit.ApiUnit_ThermalExp_REV_C())
                {
                    result = currentValue;
                }
                else
                {
                    result = currentValue * Constants.API_THERMAL_EXPANSION_REVF_TO_REVC;
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

        public override double RoundingIncrement(ApiUnit unit,  out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            /** Retrieve the  Thermal Expansion Factor  increment for 1/F */
            if (unit == ApiUnit.ApiUnit_ThermalExp_REV_F())
            {
                result = Constants.API_THERMAL_EXPANSION_F_ROUNDING_INCREMENT;
            }

            /** Retrieve the  Thermal Expansion Factor  increment for 1/C */
            else if (unit == ApiUnit.ApiUnit_ThermalExp_REV_C())
            {
                result = Constants.API_THERMAL_EXPANSION_C_ROUNDING_INCREMENT;
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
            errorCode = Error.NO_ERROR;

            /** Initialize AllowedAlpha60 if necessary */
            if (AllowedAlpha60 == null)
            {
                double [] range = Constants.API_ALLOWED_THERMAL_EXPANSION_RANGE;
                int i;

                errorCode = Error.NO_ERROR;
                AllowedAlpha60 = new ApiAlpha60[2];
                if (AllowedAlpha60 == null)
                {
                    errorCode = Error.INITIALIZE_FAILED;
                    return result;
                }
                for (i = 0; i < 2; i++)
                {
                    AllowedAlpha60[i] = Init(range[i],
                                     ApiUnit.ApiUnit_ThermalExp_REV_F(),
                                     false,
                                     out errorCode);
                    if (errorCode != Error.NO_ERROR || AllowedAlpha60[i] == null)
                    {
                        errorCode = Error.INITIALIZE_FAILED;
                        return result;
                    }
                }
            }

            /** now compare the data */
            compare = CompareTo(AllowedAlpha60[0], out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                return result;
            }
            if (compare == -1)
            {
                errorCode = Error.QUANTITY_OUT_OFF_RANGE;
                return result;
            }
            compare = CompareTo(AllowedAlpha60[1], out errorCode);
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

        public static ApiAlpha60 Init(double value,
                         ApiUnit unit,
                         bool isMutable,
                         out Error errorCode)
        {
            ApiAlpha60 temp = new ApiAlpha60();

            errorCode = Error.NO_ERROR;

            /** Create the object itself */
            if (temp == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return temp;
            }

            /** And finally let the quant object initialize itself */
            errorCode = temp.Initialize(value,
                             unit,
                             isMutable);

            /** And return the new object*/
            return temp;
        }

        public override ApiUnit GetStandardUnit()
        {
            return ApiUnit.ApiUnit_ThermalExp_REV_F();
        }

        public override string GetQuantityName()
        {
            return "Thermal Expansion Factor";
        }

        public override ApiUnit[] GetAcceptableUnits()
        {
            ApiUnit[] acceptableUnits = new ApiUnit[3];
            acceptableUnits[0] = ApiUnit.ApiUnit_ThermalExp_REV_F();
            acceptableUnits[1] = ApiUnit.ApiUnit_ThermalExp_REV_C();
            acceptableUnits[2] = null;
            return acceptableUnits;
        }

    }
}
