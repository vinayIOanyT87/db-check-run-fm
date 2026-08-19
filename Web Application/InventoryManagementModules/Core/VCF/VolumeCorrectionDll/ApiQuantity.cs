using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public abstract class ApiQuantity
    {
        public double currentValue;                 /** The current value */
        public ApiUnit currentUnit;                /** The current unit */
        public double givenValue;                   /** The current value of */
        public ApiUnit givenUnit;                  /** The current unit */
        public bool isMutable;                      /** Is the quantity mutable */
        public bool wasCorrected;                   /** If the value can be corrected, was it done*/

        public Error Initialize(double value, ApiUnit unit, bool isMutable)
        {
            Error result = Error.NO_ERROR;
            this.isMutable = true;
            result = SetValue(value, unit);
            this.isMutable = isMutable;
            return result;
        }

        public void CleanUp()
        {
        }

        public Error IsAcceptableUnit(ApiUnit unit)
        {
            Error found = Error.INVALID_UNIT;
            ApiUnit[] allowedUnit;



            /** All is initialized now let's check the unit */
            allowedUnit = this.GetAcceptableUnits();
            if (allowedUnit == null) return found;
            for (int i = 0; i < allowedUnit.Length; i++)
            {
                if (allowedUnit[i] == null)
                {
                    return found;
                }
                if (unit.unitName == allowedUnit[i].unitName)
                {
                    return Error.NO_ERROR;
                }
            }
            return found;
        }

        public double GetValue(ApiUnit unit, bool rounded, out Error errorCode)
        {
            double result = 0;

            errorCode = Error.NO_ERROR;

            /** Check that the unit is acceptable  */
            errorCode = IsAcceptableUnit(unit);
            if (errorCode != Error.NO_ERROR)
            {
                return result;
            }

            result = _GetValue(unit, rounded, out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                return result;
            }


            /** Do the rounding if need be */
            if (rounded == true)
            {
                double delta;
                double dRounded, tmp, sign = 1;
                int trunc;

                /* Step 1: Get the rounding increment delta for the unit */
                delta = RoundingIncrement(unit, out errorCode);
                if (errorCode != Error.NO_ERROR) return result;

                /* Step 2: Normalize the input variable */
                if (result < 0) sign = -1;
                dRounded = Math.Abs(result) / delta;

                /* Step 3: Find the integer closes to the normalized variable
                     Remark: Decimal part is exactly 0.5 has been replaced
                         by |(decimal part) - 0.5| <   API_EPSILON
                             since floating point arithmetic may not result in
                             exactly  0.5 */
                trunc = (int)dRounded;
                tmp = dRounded - trunc;
                if (Math.Abs(tmp - 0.5) < Constants.API_EPSILON)
                {
                    int odd = trunc / 2;
                    if (odd * 2 != trunc)
                    {   /* odd  so  trunc is increase by 1 */
                        trunc += 1;
                    }
                }
                else
                {
                    trunc = (int)(dRounded + 0.5);
                }

                /* Step 4: Rescale the integer from Step 3 */
                dRounded = trunc * delta;
                dRounded *= sign;

                result = dRounded;
            }

            return result;
        }

        public Error SetValue(double value, ApiUnit unit)
        {
            Error result = Error.NO_ERROR;

            /** 
             * Checke that all is correctly initialized and 
             * that we can change the value  
             */
            if (unit == null) return Error.NULL_POINTER_EXCEPTION;
            if (isMutable == false) return Error.CHANGED_IMMUTABLE;


            /** Check that the unit is acceptable */
            result = IsAcceptableUnit(unit);
            if (result != 0)
            {
                return result;
            }

            /** Now set the value as unit is acceptable */
            currentValue = value;
            currentUnit = unit;
            givenValue = value;
            givenUnit = unit;

            wasCorrected = false;

            return result;
        }

        public Error SetValueFromParent(ApiQuantity parent)
        {
            Error errorCode = Error.NO_ERROR;
            ApiUnit u;
            double value;

            if (parent == null)
            {
                return Error.NULL_POINTER_EXCEPTION;
            }


            u = parent.GivenUnit(out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            value = parent.GetValue(u, false, out errorCode);
            if (errorCode != Error.NO_ERROR) return errorCode;
            errorCode = SetValue(value, u);
            return errorCode;
        }

        public ApiUnit StandardUnit()
        {
            ApiUnit unit = null;

            unit = GetStandardUnit();

            return unit;
        }

        public int CompareTo(ApiQuantity quant2, out Error errorCode)
        {
            int result = -1;
            double q1, q2;
            ApiUnit u1, u2;

            errorCode = Error.NO_ERROR;

            if (quant2 == null)
            {
                errorCode = Error.NULL_POINTER_EXCEPTION;
                return result;
            }

            if (this.GetQuantityName() == null || quant2.GetQuantityName() == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return result;
            }
            if (this.GetQuantityName() != quant2.GetQuantityName())
            {
                errorCode = Error.INVALID_COMPARISON;
                return result;
            }


            /* Next make sure they are equal */
            u1 = this.StandardUnit();
            u2 = quant2.StandardUnit();
            if (u1 == null || u2 == null)
            {
                errorCode = Error.INITIALIZE_FAILED;
                return result;
            }
            if (u1 != u2)
            {
                errorCode = Error.INVALID_COMPARISON;
                return result;
            }
            q1 = this.GetValue(u1, false, out errorCode);
            if (errorCode != 0)
            {
                return result;
            }
            q2 = quant2.GetValue(u2, false, out errorCode);
            if (errorCode != Error.NO_ERROR)
            {
                return result;
            }


            if (Math.Abs(q1 - q2) < Constants.API_EPSILON)
            {
                result = 0;
            }
            else if (q2 > q1)
            {
                result = -1;
            }
            else
            {
                result = 1;
            }

            return result;
        }

        public ApiUnit GivenUnit(out Error errorCode)
        {
            errorCode = Error.NO_ERROR;
            ApiUnit result = null;
            result = givenUnit;
            return result;
        }

        public Error GetName(string buf, int len)
        {
            Error errorCode = Error.NO_ERROR;
            buf += GetQuantityName();
            return errorCode;
        }

        public abstract double _GetValue(ApiUnit unit, bool rounded, out Error errorCode);

        public abstract double RoundingIncrement(ApiUnit unit, out Error errorCode);

        public abstract bool InAllowedRange(out Error errorCode);

        public abstract ApiUnit GetStandardUnit();

        public abstract ApiUnit[] GetAcceptableUnits();

        public abstract string GetQuantityName();

    }


}
