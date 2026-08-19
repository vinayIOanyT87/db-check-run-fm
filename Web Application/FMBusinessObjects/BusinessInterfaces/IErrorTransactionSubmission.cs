using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.BusinessInterfaces
{
    [ServiceContract]
    public interface IErrorTransactionSubmission
    {

        [OperationContract]
        Guid Add(SecurityClass security, ErrorTransactionSubmissionClass errorTransactionSubmissionClass);

        [OperationContract]
        IEnumerable<ErrorTransactionSubmissionClass> GetByCustomer(SecurityClass security, Guid customerGuid);
    }
}
