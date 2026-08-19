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
	public interface ISyncServerConfigurations
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		Guid Add(SecurityClass pSecurity, SyncServerConfigurationDO pSyncServerConfiguration);

        [OperationContract]
        void Modify(SecurityClass pSecurity, SyncServerConfigurationDO pSyncServerConfiguration);

        [OperationContract]
        void Purge(SecurityClass pSecurity, Guid pSyncServerConfigurationGuid);

        [OperationContract]
        SyncServerConfigurationDO Get(SecurityClass pSecurity);
    }
}
