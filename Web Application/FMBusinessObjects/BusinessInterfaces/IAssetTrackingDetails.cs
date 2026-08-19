namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Parsers;

	[ServiceContract]
	public interface IAssetTrackingDetails
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AssetTrackingDetailClass assetTrackingDetail);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Update(SecurityClass security, AssetTrackingDetailClass assetTrackingDetail);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateRecordsToInvestigateState(SecurityClass security, List<string> assetTrackingGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateRecordsToInvestigateCompleteState( SecurityClass security, 
														string deviceId,
														AssetTrackingDetailClass.MessageStates completeState,
														string remarks);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateRemarks(SecurityClass security, Guid assetTrackingDetailGuid, string remarks);

		[OperationContract]
		AssetTrackingDetailClass Get(SecurityClass security, Guid assetTrackingDetailGuid);

		[OperationContract]
		List<AssetTrackingDetailClass> GetByFilters(SecurityClass security, DateTime startDate, DateTime endDate, string deviceId, bool topOne);

		[OperationContract]
		List<AssetTrackingDetailClass> GetByDeviceList(	SecurityClass security, 
														List<AssetTrackingDeviceClass> devices, 
														DateTime startDate,
														DateTime endDate,
														bool topOne);

		[OperationContract]
		List<AssetTrackingDetailClass> GetByDeviceAndMostCurrent(SecurityClass security, AssetTrackingDeviceClass device);

		[OperationContract]
		List<AssetTrackingTankClass> GetWrdcuTanks(Guid assetTrackingDetailGuid, SecurityClass security);

		[OperationContract]
		List<AssetTrackingTankClass> GetPreviousDetailTanks(string assetTrackingDeviceId, SecurityClass security);

		[OperationContract]
		List<AssetTrackingDetailClass> GetLast60DaysByDevice(SecurityClass security,
															string deviceId,
															DateTime startDate,
															DateTime filterStartingDate,
															DateTime filterEndingDate,
															bool topOne);

		[OperationContract]
		bool FoundInvestigateStates(SecurityClass security, string deviceId, DateTime startDate);
	}
}
