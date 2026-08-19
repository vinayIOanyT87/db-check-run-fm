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
	public interface ISyncScopes
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		Guid Add(SecurityClass pSecurity, SyncScopeDO pSyncScope);

        [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass pSecurity, SyncScopeDO pSyncScope);

        [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass pSecurity, Guid pSyncScopeGuid);

        [OperationContract]
        SyncScopeDO Get(SecurityClass pSecurity, Guid pSyncScopeGuid);

        [OperationContract]
        SyncScopeDO GetById(SecurityClass pSecurity, Guid pSyncProfileGuid, string pID);

		[OperationContract]
        Guid GetIdentityGuid(SecurityClass pSecurity, Guid pSyncProfileGuid, string pID);

		[OperationContract]
        SyncScopeCollection Enumerate(SecurityClass pSecurity, SyncProfileDO pSyncProfile);

		[OperationContract]
        SyncScopeCollection EnumerateExt(SecurityClass pSecurity, SyncProfileDO pSyncProfile, int limit = 0);
    }
}
