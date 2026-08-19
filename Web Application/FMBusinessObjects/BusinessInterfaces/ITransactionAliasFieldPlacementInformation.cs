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
    public interface ITransactionAliasFieldPlacementInformation
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid AddOrUpdate(SecurityClass security, TransactionAliasFieldPlacementInformationClass fieldPlacement);

        [OperationContract]
        TransactionAliasFieldPlacementInformationClass GetByTransactionAlias(SecurityClass security, Guid transactionAliasGuid);
    }
}
