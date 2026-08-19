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
	public interface ISyncTableToScopeMappings
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		Guid Add(SecurityClass security, SyncTableToScopeMapDO syncTableToScopeMap);

        [OperationContract]
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        void Modify(SecurityClass security, SyncTableToScopeMapDO syncTableToScopeMap);

        [OperationContract]
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        void Purge(SecurityClass security, Guid syncTableToScopeMapGuid);

        [OperationContract]
        SyncTableToScopeMapDO Get(SecurityClass security, Guid syncTableToScopeMapGuid);

        [OperationContract]
        SyncTableToScopeMapDO GetById(SecurityClass security, string id);

		[OperationContract]
        Guid GetIdentityGuid(SecurityClass security, string id);

		[OperationContract]
        SyncTableToScopeMapCollection Enumerate(SecurityClass security, SyncScopeDO syncScope);

		[OperationContract]
        SyncTableToScopeMapCollection EnumerateExt(SecurityClass security, SyncScopeDO syncScope, int limit = 0);

        [OperationContract]
        SyncTableToScopeMapCollection EnumerateForTable(SecurityClass security, SyncTableDO syncTable);
    }
}
