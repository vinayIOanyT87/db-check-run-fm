using System;
using System.Data;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IEquipmentMaintenanceLogs
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, EquipmentMaintenanceLogClass equipmentMaintenanceLog);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, EquipmentMaintenanceLogClass equipmentMaintenanceLog);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid equipmentMaintenanceLogGuid);

		[OperationContract]
		DataSet GetDataSet(SecurityClass security,
								bool bHistorical,
								string sDateType,
								DateTimeOffset dateStart,
								DateTimeOffset dateEnd,
								Guid assetGuid);

		[OperationContract]
		EquipmentMaintenanceLogClass GetByEquipmentGuid(SecurityClass security, Guid equipmentGuid);

		[OperationContract]
		int GetHoursPassed(SecurityClass security, EquipmentMaintenanceLogClass equipmentMaintenanceLog);

		[OperationContract]
		EquipmentMaintenanceLogClass Get(SecurityClass security, Guid equipmentMaintenanceLogGuid);

		[OperationContract]
		EquipmentMaintenanceLogCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		bool IsMaintenanceReasonUsed(SecurityClass security, Guid maintenanceReasonGuid);

		[OperationContract]
		EquipmentMaintenanceLogCollectionClass EnumerateByMaintenanceReason(SecurityClass security, Guid maintenanceReasonGuid);
	}
}
