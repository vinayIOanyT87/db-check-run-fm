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
    public interface IPostToEnterpriseProcessor
    {
        [OperationContract]
        int TransactionCountToUpdate(
            SecurityClass security,
            Guid productGuid,
            Guid managerGuid,
            DateTime end,
            DateTime? start = null);

        [OperationContract]
        int PostTransactionsToEnterprise(
            SecurityClass security,
            Guid productGuid,
            Guid managerGuid,
            DateTime end,
            DateTime? start = null,
            bool doneInPeaceMeal = false);
    }
}
