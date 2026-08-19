using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IGates
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, GateClass gate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, GateClass gate);

		[OperationContract]
		GateClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string ID);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid identityGuid);

		[OperationContract]
		GateCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		GateCollectionClass EnumerateBySite(SecurityClass security, Guid siteGuid);
	}
}
