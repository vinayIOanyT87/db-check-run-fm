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
    public interface ISyncTableToScopeMapCommands
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass pSecurity, SyncTableToScopeMapCommandDO pSyncTableToScopeMapCommand);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass pSecurity, SyncTableToScopeMapCommandDO pSyncTableToScopeMapCommand);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass pSecurity, Guid pSyncTableToScopeMapCommandGuid);

        [OperationContract]
        EquipmentClass Get(SecurityClass pSecurity, Guid pSyncTableToScopeMapCommandGuid);

        [OperationContract]
        SyncTableToScopeMapCommandDO GetById(SecurityClass pSecurity, string pID);

        [OperationContract]
        Guid GetIdentityGuid(SecurityClass pSecurity, string ID);

        [OperationContract]
        SyncTableToScopeMapCommandCollection Enumerate(SecurityClass pSecurity, SyncTableToScopeMapDO pSyncTableToScopeMap);

        [OperationContract]
        SyncTableToScopeMapCommandCollection EnumerateExt(SecurityClass pSecurity, SyncTableToScopeMapDO pSyncTableToScopeMap, int limit = 0);
    }
}
