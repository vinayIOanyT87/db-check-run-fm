using System;
using System.Collections.Generic;
using FMWebAPIBusinessLogic.DTO.TransactionDTO;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    using FMBusinessObjects.DataObjects;

    public interface ITransactionFieldsService
    {
        IEnumerable<TransactionAliasFieldClassWithColumn> GeTransactionFieldDefinitionsForUI(TransactionAliasClass currentAlias);
        IEnumerable<TransactionAliasFieldClassWithColumn> GeTransactionFieldDefinitionsForUI(Guid transactionAliasGuid);
    }
}