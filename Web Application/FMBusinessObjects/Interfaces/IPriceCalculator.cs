using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.Interfaces
{
    public interface IPriceCalculator
    {
        bool Calculate( SecurityClass oSecurity, TransactionDO trans, ArrayList origLineItems, bool bForceRecalculation );
    }
}
