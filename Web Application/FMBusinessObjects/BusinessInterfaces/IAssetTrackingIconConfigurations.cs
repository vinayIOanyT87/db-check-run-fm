namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAssetTrackingIconConfigurations
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AssetTrackingIconConfigurationClass assetTrackingIconConfiguration);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AssetTrackingIconConfigurationClass assetTrackingIconConfiguration);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid assetTrackingIconConfigurationGuid);

		[OperationContract]
		AssetTrackingIconConfigurationClass Get(SecurityClass security, Guid assetTrackingIconConfigurationGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string iconConfigurationId);

		[OperationContract]
		List<AssetTrackingIconConfigurationClass> Enumerate(SecurityClass security);
	}
}
