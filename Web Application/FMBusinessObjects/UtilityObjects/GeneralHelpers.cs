using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.UtilityObjects
{
    using System;
    public class GeneralHelpers
    {
        public static bool IsStringAGuid(string strValue)
        {
            Guid guidValue;
            return Guid.TryParse(strValue, out guidValue);
        }

        public static bool IsStringAnNonEmptyGuid(string strValue)
        {
            Guid guidValue;
            return (Guid.TryParse(strValue, out guidValue) && !guidValue.Equals(Guid.Empty));
        }
    }
}
