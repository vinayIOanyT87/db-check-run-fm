using System;
using System.Data;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface ITanks
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, TankClass tank );

		[OperationContract]
		DataSet EnumerateForPhysicalInventory(SecurityClass security, bool hideHiddenTanks = false);

		[OperationContract]
		TankCollectionClass Enumerate ( SecurityClass security, bool hideHiddenTanks = false );

        [OperationContract]
        TankCollectionClass EnumerateAuthorized(SecurityClass security, bool hideHiddenTanks = false);

        [OperationContract]
		TankCollectionClass EnumerateWhereCoordinatesExist(SecurityClass security);

		[OperationContract]
		TankCollectionClass EnumerateTanksWithoutQualityTag ( SecurityClass security );

		[OperationContract]
		TankCollectionClass EnumerateByFilter ( SecurityClass security, string filter, bool hideHiddenTanks = false);

		[OperationContract]
        TankCollectionClass EnumerateByProduct(SecurityClass security, Guid productGuid, bool hideHiddenTanks = false);

		[OperationContract]
        TankCollectionClass EnumerateByProductAndFilter(SecurityClass security, Guid productGuid, string filter, bool hideHiddenTanks = false);

		[OperationContract]
		TankCollectionClass EnumerateByManager(SecurityClass security, Guid managerGuid);

		[OperationContract]
        TankCollectionClass EnumerateBasicInformation(SecurityClass security);

		[OperationContract]
		TankCollectionClass EnumerateBasicInfoLinkedToAssetTrackingDevices(SecurityClass security, string assetTrackingDeviceId);

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string id );

		[OperationContract]
		TankClass Get ( SecurityClass security, Guid tankGuid );

		[OperationContract]
		ProcessVariableCollectionClass GetProcessVariables(SecurityClass security, Guid tankGuid);

		[OperationContract]
		int TankConfigurationNumberBeingUsed(SecurityClass security,Guid tankGuid, Guid assetTrackingDeviceGuid, int tankConfigurationNumber);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, TankClass tank );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid tankGuid);
	}
}
