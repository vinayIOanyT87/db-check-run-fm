using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public class ApiPressure : ApiQuantity
    {
        static ApiPressure[] AllowedPres;

        public override double _GetValue(ApiUnit unit, bool rounded, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            /** Retrieve the pressure in psi - convert if needed */
            if (unit == ApiUnit.ApiUnit_Pressure_PSI())
            {
                if (currentUnit == ApiUnit.ApiUnit_Pressure_PSI())
                {
                    result = currentValue;
                }
                else if (currentUnit == ApiUnit.ApiUnit_Pressure_KPA())
                {
                    result = currentValue / Constants.API_PRESSURE_KPA_TO_PSI_FACTOR;
                }
                else if (currentUnit == ApiUnit.ApiUnit_Pressure_BAR())
                {
                    result = currentValue / Constants.API_PRESSURE_BAR_TO_PSI_FACTOR;
                }
            }

            /** Retrieve the pressure in kPa - convert if needed */
            else if (unit == ApiUnit.ApiUnit_Pressure_KPA())
            {
                if (currentUnit == ApiUnit.ApiUnit_Pressure_KPA())
                {
                    result = currentValue;
                }
                else if (currentUnit == ApiUnit.ApiUnit_Pressure_PSI())
                {
                    result = currentValue * Constants.API_PRESSURE_KPA_TO_PSI_FACTOR;
                }
                else if (currentUnit == ApiUnit.ApiUnit_Pressure_BAR())
                {
                    result = currentValue * Constants.API_PRESSURE_KPA_TO_PSI_FACTOR
                              / Constants.API_PRESSURE_BAR_TO_PSI_FACTOR;
                }
            }

            /** Retrieve the pressure in bar - convert if needed */
            else if (unit == ApiUnit.ApiUnit_Pressure_BAR())
            {
                if (currentUnit == ApiUnit.ApiUnit_Pressure_BAR())
                {
                    result = currentValue;
                }
                else if (currentUnit == ApiUnit.ApiUnit_Pressure_PSI())
                {
                    result = currentValue * Constants.API_PRESSURE_BAR_TO_PSI_FACTOR;
                }
                else if (currentUnit == ApiUnit.ApiUnit_Pressure_KPA())
                {
                    result = currentValue * Constants.API_PRESSURE_BAR_TO_PSI_FACTOR
                              / Constants.API_PRESSURE_KPA_TO_PSI_FACTOR;
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

            /** Retrieve the pressure increment for psi */
            if (unit == ApiUnit.ApiUnit_Pressure_PSI())
            {
                result = Constants.API_RESSURE_PSI_ROUNDING_INCREMENT;
            }

            /** Retrieve the pressure increment for kPa */
            else if (unit == ApiUnit.ApiUnit_Pressure_KPA())
            {
                result = Constants.API_PRESSURE_KPA_ROUNDING_INCREMENT;
            }

            /** Retrieve the pressure increment for bar */
            else if (unit == ApiUnit.ApiUnit_Pressure_BAR())
            {
                result = Constants.API_PRESSURE_BAR_ROUNDING_INCREMENT;
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
            double[] range = Constants.API_ALLOWED_PRESSURE_RANGE;

            /** Initialize AllowedPres if necessary */
            if (AllowedPres == null)
            {
                int i;

                errorCode = Error.NO_ERROR;
                AllowedPres = new ApiPressure[2];
                for (i = 0; i < 2; i++)
                {
                    AllowedPres[i] = Init(range[i],
                                    ApiUnit.ApiUnit_Pressure_PSI(),
                                    false,
                                    out errorCode);
                    if (errorCode != Error.NO_ERROR || AllowedPres[i] == null)
                    {
                        errorCode = Error.INITIALIZE_FAILED;
                        return result;
                    }
                }
            }

            /** now compare the data */
            compare = CompareTo(AllowedPres[0], out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                return result;
            }
            if (compare == -1)
            {
                errorCode = SetValue(range[0], ApiUnit.ApiUnit_Pressure_PSI());
                if (errorCode != Error.NO_ERROR)
                {   /** We tried to change an immutable value */
                    errorCode = Error.QUANTITY_OUT_OFF_RANGE;
                    return result;
                }
            }
            compare = CompareTo(AllowedPres[1],out errorCode);
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


        public static ApiPressure Init(double value,
                           ApiUnit unit,
                           bool isMutable,
                           out Error errorCode)
        {
            ApiPressure temp = new ApiPressure();

            if (temp == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return temp;
            }

            errorCode = Error.NO_ERROR;

 
            errorCode = temp.Initialize(
                             value,
                             unit,
                             isMutable);

            /** And return the new object*/
            return temp;
        }

        public override ApiUnit GetStandardUnit()
        {
            return ApiUnit.ApiUnit_Pressure_PSI();
        }

        public override string GetQuantityName()
        {
            return "Pressure";
        }

        public override ApiUnit[] GetAcceptableUnits()
        {
            ApiUnit[] acceptableUnits = new ApiUnit[4];
            acceptableUnits[0] = ApiUnit.ApiUnit_Pressure_PSI();
            acceptableUnits[1] = ApiUnit.ApiUnit_Pressure_KPA();
            acceptableUnits[2] = ApiUnit.ApiUnit_Pressure_BAR();
            acceptableUnits[3] = null;
            return acceptableUnits;
        }
    }
}
