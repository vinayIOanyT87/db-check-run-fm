using System;
using System.Collections.Generic;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface ITransactionAliasFieldsProxy
    {
        Guid Add(TransactionAliasFieldClass transactionAliasField);
        TransactionAliasFieldCollectionClass Enumerate(Guid transactionAliasGuid, TransactionFieldType type, bool dispatchFields, bool byUser);
        TransactionAliasFieldCollectionClass EnumerateByAliasGuid(Guid aliasGuid, bool byUser);
        List<string> EnumerateFields(TransactionFieldType fieldType, TransactionTypes transType);
        TransactionAliasFieldClass Get(Guid identityGuid);
        IEnumerable<TransactionAliasFieldExtendedAttributes> GetColumnDefinitionsForTransactions();
        void Modify(TransactionAliasFieldClass transactionAliasField);
        void ModifyCollection(Guid transactionAliasGuid, string transactionAliasName, TransactionAliasFieldCollectionClass newFieldCollection, TransactionAliasFieldCollectionClass oldFieldCollection);
        void Purge(Guid identityGuid);
    }
}