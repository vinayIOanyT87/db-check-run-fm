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
    public interface ISyncTableToScopeMapColumns
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass pSecurity, SyncTableToScopeMapColumnDO pSyncTableToScopeMapColumn);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass pSecurity, SyncTableToScopeMapColumnDO pSyncTableToScopeMapColumn);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass pSecurity, Guid pSyncTableToScopeMapColumnGuid);

        [OperationContract]
        EquipmentClass Get(SecurityClass pSecurity, Guid pSyncTableToScopeMapColumnGuid);

        [OperationContract]
        SyncTableToScopeMapColumnDO GetById(SecurityClass pSecurity, string pID);

        [OperationContract]
        Guid GetIdentityGuid(SecurityClass pSecurity, string ID);

        [OperationContract]
        SyncTableToScopeMapColumnCollection Enumerate(SecurityClass pSecurity, SyncTableToScopeMapDO pSyncTableToScopeMap);

        [OperationContract]
        SyncTableToScopeMapColumnCollection EnumerateExt(SecurityClass pSecurity, SyncTableToScopeMapDO pSyncTableToScopeMap, int limit = 0);
    }
}
