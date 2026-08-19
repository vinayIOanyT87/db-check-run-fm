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
	public interface ITankMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, TankMapClass tankMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid assignedToTankGroupGuid, Guid tankGuid );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifyCollection ( SecurityClass security,
										Guid identityGuid,
										string ID,
										TankMapCollectionClass newTankMapCollection,
										TankMapCollectionClass existingTankMapCollection );

		[OperationContract]
		TankMapCollectionClass EnumerateByAssignedToTankGroupGuid ( SecurityClass security, Guid assignedToTankGroupGuid );

		[OperationContract]
		TankMapCollectionClass EnumerateByTankGuid ( SecurityClass security, Guid tankGuid );
	}
}
