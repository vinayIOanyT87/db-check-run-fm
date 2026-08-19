using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiExpansionFactor : ApiQuantity
    {

        public override double _GetValue(ApiUnit unit, bool rounded, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            result = currentValue;

            return result;
        }

        public override double RoundingIncrement(ApiUnit unit, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            result = Constants.API_EXPANSION_FACTOR_ROUNDING_INCREMENT;

            return result;
        }
 
        public static ApiExpansionFactor Init(double value,
                                 ApiUnit unit,
                                 bool isMutable,
                                 out Error errorCode)
        {
            ApiExpansionFactor vol = new ApiExpansionFactor();

            errorCode = Error.NO_ERROR;

            if (vol == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return vol;
            }

            /** And finally let the quant object initialize itself */
            errorCode = vol.Initialize(value,
                             unit,
                             isMutable);

            /** And return the new object*/
            return vol;
        }

        public override ApiUnit GetStandardUnit()
        {
            return ApiUnit.ApiUnit_Expansion_DIMLESS();
        }

        public override string GetQuantityName()
        {
            return "Expansion Factor";
        }

        public override ApiUnit[] GetAcceptableUnits()
        {
            ApiUnit[] acceptableUnits = new ApiUnit[2];
            acceptableUnits[0] = ApiUnit.ApiUnit_Expansion_DIMLESS();
            acceptableUnits[1] = null;
            return acceptableUnits;
        }

        public override bool InAllowedRange(out Error errorCode)
        {
            errorCode = Error.NO_ERROR;
            return true;
        }
    }
}
