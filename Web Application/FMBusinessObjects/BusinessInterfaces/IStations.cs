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
	public interface IStations
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		[ReferencePreservingDataContractFormat]
		Guid Add ( SecurityClass security, StationClass Station );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		[ReferencePreservingDataContractFormat]
		void Modify ( SecurityClass security, StationClass Station );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		[ReferencePreservingDataContractFormat]
		void Purge ( SecurityClass security, Guid stationGuid );

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		StationClass Get ( SecurityClass security, Guid stationGuid );

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		Guid GetIdentityGuid ( SecurityClass security, string id );

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		StationCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		StationCollectionClass EnumerateByType ( SecurityClass security, STATION_TYPE type );

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		int GetTheNextPresetNumber(SecurityClass security, Guid stationGuid);

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		bool IsDynamicRecipesEnabled(SecurityClass security, Guid stationGuid, STATION_TYPE type);

		[OperationContract]
		[ReferencePreservingDataContractFormat]
		List<bool> IsDynamicRecipesEnabledOnPartnerStations(SecurityClass security, Guid stationGuid, List<Guid> partnerStationGuids, STATION_TYPE type);
	}
}
