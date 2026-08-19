namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAssetTrackingDevices
	{
		[OperationContract]
		List<AssetTrackingDeviceClass> Enumerate(SecurityClass security);

		[OperationContract]
		List<AssetTrackingDeviceClass> EnumerateActiveDevices(SecurityClass security);

		[OperationContract]
		List<AssetTrackingDeviceClass> EnumerateAllDevices(SecurityClass security);

		[OperationContract]
		List<AssetTrackingDeviceClass> EnumerateAllUnassignedActiveDevices(SecurityClass security);

		[OperationContract]
		DataSet EnumerateAllDeviceInDataSet(SecurityClass security, string filter);

		[OperationContract]
		List<AssetTrackingDeviceClass> EnumerateAllDevicesLinkedToEquipment(SecurityClass security);

		[OperationContract]
		DataSet EnumerateAllEquipmentNotAssociateToDevices(SecurityClass security);

		[OperationContract]
		List<AssetTrackingDeviceClass> EnumerateAllSatelliteDevices(SecurityClass security);

		[OperationContract]
		List<string> EnumerateAssociatedTanks(SecurityClass security, Guid assetTrackingDeviceGuid);

		[OperationContract]
		DataSet EnumerateAllAssociatedTanks(SecurityClass security);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string assetTrackingDeviceId);

		[OperationContract]
		Guid GetIdentityGuidWithoutSite(SecurityClass security, string assetTrackingDeviceId);

		[OperationContract]
		AssetTrackingDeviceClass GetByIdentityGuid(SecurityClass security, Guid assetTrackingDeviceGuid);

		[OperationContract]
		AssetTrackingDeviceClass GetAssociatedEquipmentIdAndProduct(SecurityClass security, Guid assetTrackingDeviceGuid);

		[OperationContract]
		Guid GetEquipmentSiteGuid(SecurityClass security, Guid assetTrackingDeviceGuid);

		[OperationContract]
		Guid GetAssociatedEquipmentGuid(SecurityClass security, Guid assetTrackingDeviceGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid assetTrackingDeviceGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AssetTrackingDeviceClass assetTrackingDevice);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AssetTrackingDeviceClass assetTrackingDevice);
	}
}
