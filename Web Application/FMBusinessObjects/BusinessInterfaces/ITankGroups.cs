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
	public interface ITankGroups
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, TankGroupClass TankGroup );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, TankGroupClass TankGroup );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid identityGuid);

		[OperationContract]
		TankGroupClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string ID );

		[OperationContract]
		TankGroupCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		TankGroupCollectionClass EnumerateByProduct(SecurityClass security, Guid productGuid);
	}
}
