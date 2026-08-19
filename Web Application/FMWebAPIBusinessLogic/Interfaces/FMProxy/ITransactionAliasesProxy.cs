using System;
using System.Collections.Generic;
using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface ITransactionAliasesProxy
    {
        Guid Add(TransactionAliasClass transactionAlias);
        TransactionAliasCollectionClass Enumerate();
        TransactionAliasCollectionClass EnumerateByGroupMapsOnly();
        TransactionAliasCollectionClass EnumerateByTransTypeID(TransactionTypes transTypeID);
        TransactionAliasNameCollectionClass EnumerateDispatchAliasNames();
        List<string> EnumerateDispatchStatusCodes();
        TransactionAliasNameCollectionClass EnumerateNamesOnly(bool byUser);
        TransactionAliasCollectionClass EnumerateUndelegated();
        TransactionAliasClass Get(Guid identityGuid, bool byUser);
        TransactionAliasClass GetBasicInfo(Guid transactionAliasClassGuid, Guid siteGuid);
        Guid GetIdentityGuid(string ID);
        Guid GetMasterRecordGuid(string id);
        TransactionAliasClass GetWithoutAliasFields(Guid aliasGuid);
        void Import(TransactionAliasClass alias);
        void Modify(TransactionAliasClass transactionAlias);
        void Purge(Guid transactionAliasGuid);
        bool UserHasModifyPermissions(Guid aliasGuid);
    }
}