using System.Collections.Generic;
using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.DTO.TransactionDTO;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    public interface ITransactionObjectTranslationService
    {
        TransactionDO ApplyDictionaryToTransaction(TransactionDO transactionToBeAppliedTo, Dictionary<string, string> newTransactionUserValues, TransactionAliasClass transactionAlias);
        Dictionary<string, string> CreateTransactionFromDataObject(TransactionDO transaction);
    }
}