using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface IErrorTransactionSubmissionProxy 
    {
        Guid Add(ErrorTransactionSubmissionClass errorTransactionSubmissionClass);
        IEnumerable<ErrorTransactionSubmissionClass> GetByCustomer(Guid customerGuid);
    }
}
