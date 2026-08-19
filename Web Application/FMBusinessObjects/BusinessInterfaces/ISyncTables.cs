using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
    [ServiceContract]
    public interface ISyncTables
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass pSecurity, SyncTableDO pSyncTable);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass pSecurity, SyncTableDO pSyncTable);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass pSecurity, Guid pSyncTableGuid);

        [OperationContract]
        EquipmentClass Get(SecurityClass pSecurity, Guid pSyncTableGuid);

        [OperationContract]
        SyncTableDO GetById(SecurityClass pSecurity, string pID);

        [OperationContract]
        Guid GetIdentityGuid(SecurityClass pSecurity, string ID);

        [OperationContract]
        SyncTableCollection Enumerate(SecurityClass pSecurity);

        [OperationContract]
        SyncTableCollection EnumerateExt(SecurityClass pSecurity, int limit = 0);

        [OperationContract]
        SyncTableCollection EnumerateForDependencyGroup(SecurityClass pSecurity, SyncDependencyGroupDO pSyncDependencyGroup);
    }
}
