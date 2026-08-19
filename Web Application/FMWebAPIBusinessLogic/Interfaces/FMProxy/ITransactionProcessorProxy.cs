using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface ITransactionProcessorProxy
    {
        TransactionDO Process(TransactionSR sr);
    }
}
