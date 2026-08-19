namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IMaintenanceReasons
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, MaintenanceReasonClass maintenanceReason);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, MaintenanceReasonClass maintenanceReason);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid maintenanceReasonGuid);

		[OperationContract]
		MaintenanceReasonClass Get(SecurityClass security, Guid maintenanceReasonGuid);

		[OperationContract]
		MaintenanceReasonClass GetBySite(SecurityClass security, Guid maintenanceReasonGuid, SiteClass site);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string id);

		[OperationContract]
		MaintenanceReasonCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		MaintenanceReasonCollectionClass EnumerateBySite(SecurityClass security);
	}
}
