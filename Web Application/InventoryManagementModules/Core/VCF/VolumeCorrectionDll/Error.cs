using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace VCF
{
    public enum Error
    {
        NO_ERROR = 0,
        /**
        * Error signal if an object cannot be  initialized.
*/
        INITIALIZE_FAILED = 1,

        /**
        * Error signaled if an unexpected  null pointer was encountered
*/
        NULL_POINTER_EXCEPTION = 2,

        /** 
        *  Error signaled if an unexpected  unit was encountered.
        *  This happens if   
        *  ApiQuantity_getValue(ApiQuantity * quant,  ApiUnit *unit,  bool rounded, int * errorCode)
        *  or
        *  ApiQuantity_setValue(ApiQuantity * quant, double value, ApiUnit *unit)
        *  are called with incompatible units.
        *
*/
        INVALID_UNIT = 3,

        /**
        *  The user tried to set an immutable quantity.
        * This happens if  
        * ApiQuantity_setValue(ApiQuantity * quant, double value, ApiUnit *unit)
        * or ApiQuantity_setValueFromParent(ApiQuantity * quant, ApiQuantity * parent)
        * are called on a quantity that has been declared as immutable.
*/
        CHANGED_IMMUTABLE = 4,

        /** 
        * Attempt to write a value to a string buffer that is to large.
        * In this case an error is signaled and nothing is written to the buffer.
*/
        BUFFER_OVERFLOW = 5,

        /** 
        * Attempt to compare two  different quantities.
        * This is signaled if for example an attempt is made to compare
        * an ApiDensity with an ApiTemperature object using the function
        * ApiQuantity_compareTo(ApiQuantity * quant1, ApiQuantity * quant2, int *errorCode)
*/
        INVALID_COMPARISON = 6,

        /**
        * A quantity is outside the range of the allowed values.
        * Before an actual calculation a check is performed to ensure that 
        * all data are in range. The check might return a more specific 
        * out of range message. This error signals a general out of range conditions.
*/
        QUANTITY_OUT_OFF_RANGE = 7,

        /** 
        * Attempt to call an  unsupported function.
        * Some functions might not be defined for all quantities, for example
        * ApiQuantity_inAllowedRange(ApiQuantity *quant, int * errorCode) 
        * is not defined for ApiDensity since the allowed density range depends on
        * the commodity used and the type of calculation performed.
*/
        UNSUPPORTED_FUNCTION = 8,

        /**
         K-Values are not defined  */
        UNDEFINED_K_VALUES = 9,

        /**Rho density limits not defined are not defined  */
        UNDEFINED_RHO_LIMITS = 10,

        /** Undefined oil product  */
        UNDEFINED_OIL_PRODUCT = 11,

        /** Commidity and Alpha_60 values are supplied for calculation */
        COMMODITY_AND_ALPHA60_SUPPLIED = 12,

        /** Commidity and Alpha_60 values are both null */
        COMMODITY_AND_ALPHA60_NULL = 13,

        /** Alpha_60 value  is out of range */
        ALPHA60_OUT_OF_RANGE = 14,

        /** Observed density value is missing */
        VCFOBSERVED_DENSITY_VALUE_MISSING = 15,

        /** Observed density not in range of given commodity for Type I calculation */
        VCFOBSERVED_DENSITY_OUT_OF_RANGE_TYPE_I = 16,

        /** Observed density not in range of given commodity for Type II calculation */
        VCFOBSERVED_DENSITY_OUT_OF_RANGE_TYPE_II = 17,

        /** Observed pressure value is missing */
        VCFOBSERVED_PRESSURE_VALUE_MISSING = 18,

        /** Observed pressure value is out of range */
        VCFOBSERVED_PRESSURE_OUT_OF_RANGE = 19,

        /** Alternate pressure value is missing */
        VCFALTERNATE_PRESSURE_VALUE_MISSING = 20,

        /** Alternate pressure value is out of range */
        VCFALTERNATE_PRESSURE_OUT_OF_RANGE = 21,

        /** Observed temperature value is missing */
        VCFOBSERVED_TEMPERATURE_VALUE_MISSING = 22,

        /** Observed temperature value is out of range */
        VCFOBSERVED_TEMPERATURE_OUT_OF_RANGE = 23,

        /** Alternate temperature value is missing */
        VCFALTERNATE_TEMPERATURE_VALUE_MISSING = 24,

        /** Alternate temperature value is out of range */
        VCFALTERNATE_TEMPERATURE_OUT_OF_RANGE = 25,

        /** More than one volume value was supplied */
        VCFMORE_THAN_ONE_VOLUME_SUPPLIED = 26,

        /** In the iteration step the density value is out of range */
        VCFITERATION_DENSITY_VALUE_OUT_OF_RANGE = 27,

        /** Convergence was not reached */
        VCFCONVERGENCE_NOT_REACHED = 28,

        /**  Array index out of range */
        VCFARRAY_INDEX_OUT_OF_RANGE = 29,

        VCFMAX_ERROR_NUMBER = 29
    };
}
