using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiVolume : ApiQuantity
    {
        public override double _GetValue(ApiUnit unit, bool rounded, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            /** Only the current unit is allowed */
            if (unit != currentUnit)
            {
                errorCode = Error.INVALID_UNIT;
                return result;
            }
            result = currentValue;

            return result;
        }

        public override double RoundingIncrement(ApiUnit unit, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.UNSUPPORTED_FUNCTION;

            return result;
        }


        public override bool InAllowedRange(out Error errorCode)
        {
            errorCode = Error.NO_ERROR;
            return true;
        }
        public override ApiUnit GetStandardUnit()
        {
            if(givenUnit == null)
            {
                return ApiUnit.ApiUnit_Volume_BARREL();
            }
            return givenUnit;
        }

        public static ApiVolume Init(double value,
                         ApiUnit unit,
                         bool isMutable,
                         out Error errorCode)
        {
            ApiVolume vol = new ApiVolume();

            errorCode = Error.NO_ERROR;

            if (vol == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return vol;
            }



            /** And finally let the quant object initialize itself */
            errorCode = vol.Initialize(value,unit,isMutable);

            return vol;
        }

        public override string GetQuantityName()
        {
            return "Volume";
        }

        public override ApiUnit[] GetAcceptableUnits()
        {
            ApiUnit[] acceptableUnits = new ApiUnit[4];
            acceptableUnits[0] = ApiUnit.ApiUnit_Volume_BARREL();
            acceptableUnits[1] = ApiUnit.ApiUnit_Volume_LITER();
            acceptableUnits[2] = ApiUnit.ApiUnit_Volume_M3();
            acceptableUnits[3] = null;
            return acceptableUnits;
        }
    }
}
