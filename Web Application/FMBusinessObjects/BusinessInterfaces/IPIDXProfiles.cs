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
	public interface IPIDXProfiles
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, PIDXProfileClass PIDXProfile );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, PIDXProfileClass PIDXProfile );

		[OperationContract]
		PIDXProfileClass Get ( SecurityClass security, Guid identityGuid, bool getMaps );

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string ID);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid pidxProfileGuid);

		[OperationContract]
		PIDXProfileCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Import ( SecurityClass security, PIDXProfileClass pidxProfile );
	}
}
