using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiScaledFactor : ApiQuantity
    {

        public override double _GetValue(ApiUnit unit,  bool rounded, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            /** Retrieve the compression factor  in 1/psi - convert if needed */
            if (unit == ApiUnit.ApiUnit_ScaledComp_REV_PSI())
            {
                if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_PSI())
                {
                    result = currentValue;
                }
                else if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_KPA())
                {
                    result = currentValue * Constants.API_PRESSURE_KPA_TO_PSI_FACTOR;
                }
                else if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_BAR())
                {
                    result = currentValue * Constants.API_PRESSURE_BAR_TO_PSI_FACTOR;
                }
            }

            /** Retrieve the pressure in 1/kPa - convert if needed */
            else if (unit == ApiUnit.ApiUnit_ScaledComp_REV_KPA())
            {
                if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_KPA())
                {
                    result = currentValue;
                }
                else if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_PSI())
                {
                    result = currentValue / Constants.API_PRESSURE_KPA_TO_PSI_FACTOR;
                }
                else if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_BAR())
                {
                    result = currentValue * Constants.API_PRESSURE_BAR_TO_PSI_FACTOR
                          / Constants.API_PRESSURE_KPA_TO_PSI_FACTOR;
                }
            }

            /** Retrieve the pressure in 1/bar - convert if needed */
            else if (unit == ApiUnit.ApiUnit_ScaledComp_REV_BAR())
            {
                if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_BAR())
                {
                    result = currentValue;
                }
                else if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_PSI())
                {
                    result = currentValue / Constants.API_PRESSURE_BAR_TO_PSI_FACTOR;
                }
                else if (currentUnit == ApiUnit.ApiUnit_ScaledComp_REV_KPA())
                {
                    result = currentValue * Constants.API_PRESSURE_KPA_TO_PSI_FACTOR
                                 / Constants.API_PRESSURE_BAR_TO_PSI_FACTOR;
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

            /** Retrieve the pressure increment for 1/psi */
            if (unit == ApiUnit.ApiUnit_ScaledComp_REV_PSI())
            {
                result = Constants.API_SCALED_COMP_REV_PSI_ROUNDING_INCREMENT;
            }

            /** Retrieve the pressure increment for 1/kPa */
            else if (unit == ApiUnit.ApiUnit_ScaledComp_REV_KPA())
            {
                result = Constants.API_SCALED_COMP_REV_KPA_ROUNDING_INCREMENT; ;
            }

            /** Retrieve the pressure increment for 1/bar */
            else if (unit == ApiUnit.ApiUnit_ScaledComp_REV_BAR())
            {
                result = Constants.API_SCALED_COMP_REV_BAR_ROUNDING_INCREMENT;
            }

            /** Otherwise the unit is not valid */
            else
            {
                errorCode = Error.INVALID_UNIT;
            }

            return result;
        }

        public static ApiScaledFactor Init(double value,
                               ApiUnit unit,
                               bool isMutable,
                               out Error errorCode)
        {
            ApiScaledFactor temp = new ApiScaledFactor() ;

            errorCode = Error.NO_ERROR;

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

        public override ApiUnit GetStandardUnit()
        {
            return ApiUnit.ApiUnit_ScaledComp_REV_PSI();
        }

        public override string GetQuantityName()
        {
            return "Scaled Compression Factor";
        }

        public override ApiUnit[] GetAcceptableUnits()
        {
            ApiUnit[] acceptableUnits = new ApiUnit[4];
            acceptableUnits[0] = ApiUnit.ApiUnit_ScaledComp_REV_PSI();
            acceptableUnits[1] = ApiUnit.ApiUnit_ScaledComp_REV_KPA();
            acceptableUnits[2] = ApiUnit.ApiUnit_ScaledComp_REV_BAR();
            acceptableUnits[3] = null;
            return acceptableUnits;
        }

        public override bool InAllowedRange(out Error errorCode)
        {
            errorCode = Error.NO_ERROR;
            return true;
        }
    }
}
