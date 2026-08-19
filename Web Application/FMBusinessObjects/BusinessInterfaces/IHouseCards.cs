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
	public interface IHouseCards
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, HouseCardClass HouseCard );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, HouseCardClass HouseCard );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		HouseCardClass Get ( SecurityClass security, Guid targetHouseCardGuid );

		[OperationContract]
		Guid GetIdentityGuidByDriverGuid( SecurityClass security, Guid targetDriverGuid );

		[OperationContract]
		Guid GetIdentityGuidByNumber( SecurityClass security, string Number );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid targetHouseCardGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		HouseCardCollectionClass Enumerate ( SecurityClass security );
	}
}
