using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.DTO.TransactionDTO;
using System;
using System.Collections.Generic;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    public interface ITransactionActionsProcessorsService
    {
        TransactionDO SubmitNewTransactionInDictionaryFormat(Dictionary<string, string> newTransactionUserValues, Guid transactionAliasGuid);

        TransactionDO UpdateExistingTransactionInDictionaryFormat(Dictionary<string, string> newTransactionUserValues, Guid transactionAliasGuid, Guid transactionGuid);
        void ReverseTransaction(Guid transactionGuid);
        void ReverseUpdateTransactionInDictionaryFormat(Guid originalTransactionGuid, Dictionary<string, string> updatedTransactionUserValues);
        void DeleteTransaction(Guid transactionGuid);
    }
}