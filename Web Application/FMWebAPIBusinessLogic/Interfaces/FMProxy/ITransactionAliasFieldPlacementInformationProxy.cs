using System;
using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface ITransactionAliasFieldPlacementInformationProxy
    {
        Guid AddOrUpdate(TransactionAliasFieldPlacementInformationClass fieldPlacement);
        TransactionAliasFieldPlacementInformationClass GetByTransactionAlias(Guid transactionAliasGuid);
    }
}