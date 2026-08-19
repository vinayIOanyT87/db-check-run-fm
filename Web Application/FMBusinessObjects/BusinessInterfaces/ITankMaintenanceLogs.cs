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
	public interface ITankMaintenanceLogs
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, TankMaintenanceLogClass tankMaintenanceLog);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, TankMaintenanceLogClass tankMaintenanceLog);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid tankMaintenanceLogGuid);

		[OperationContract]
		TankMaintenanceLogClass Get(SecurityClass security, Guid tankMaintenanceLogGuid);

		[OperationContract]
		TankMaintenanceLogClass GetByTankGuid(SecurityClass security, Guid tankGuid);

		[OperationContract]
		int GetHoursPassed(SecurityClass security, TankMaintenanceLogClass tankMaintenanceLog);

		[OperationContract]
		bool IsMaintenanceReasonUsed(SecurityClass security, Guid maintenanceReasonGuid);

		[OperationContract]
		TankMaintenanceLogCollectionClass EnumerateByMaintenanceReason(SecurityClass security, Guid maintenanceReasonGuid);
	}
}
