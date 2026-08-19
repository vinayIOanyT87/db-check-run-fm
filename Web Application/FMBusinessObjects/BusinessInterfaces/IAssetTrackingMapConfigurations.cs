namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAssetTrackingMapConfigurations
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AssetTrackingMapConfigurationClass assetTrackingMapConfigurationClass);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AssetTrackingMapConfigurationClass assetTrackingMapConfigurationClass);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid assetTrackingMapConfigurationGuid);

		[OperationContract]
		AssetTrackingMapConfigurationClass Get(SecurityClass security, Guid assetTrackingMapConfigurationGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string mapName);

		[OperationContract]
		AssetTrackingMapConfigurationClass GetByMapName(SecurityClass security, string mapName);

		[OperationContract]
		List<AssetTrackingMapConfigurationClass> Enumerate(SecurityClass security);

		[OperationContract]
		List<AssetTrackingMapConfigurationClass> EnumerateByFilter(SecurityClass security, string filter);
	}
}
