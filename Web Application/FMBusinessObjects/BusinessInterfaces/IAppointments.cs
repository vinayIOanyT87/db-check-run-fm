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
	public interface IAppointments
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AppointmentClass appointment);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AppointmentClass appointment);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid appointmentGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByAssetID(SecurityClass security, Guid assetID, string appointmentType);

		[OperationContract]
		AppointmentClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		AppointmentClass GetByIncludeTests(SecurityClass security, Guid identityGuid, bool includeTests);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string id);

		[OperationContract]
		AppointmentCollectionClass EnumerateScheduledAndOverdue(SecurityClass security, DateTimeOffset startDate, string appointmentType);

		[OperationContract]
		AppointmentCollectionClass EnumerateByAssetGuid(SecurityClass security, string appointmentType, Guid entityGuid);

		[OperationContract]
		AppointmentCollectionClass EnumerateByStartStopTime(SecurityClass security, string appointmentType, DateTimeOffset startDate, DateTimeOffset endDate);

		[OperationContract]
		AppointmentClass EnumerateAppointmentByIdentityGuid(SecurityClass security, Guid appointmentGuid);

		[OperationContract]
		AppointmentCollectionClass EnumerateBasedOnTestSetAndEntity(SecurityClass security, Guid testSetDefinitionGuid, bool equipment, Guid equipmentGuid);

		[OperationContract]
		DateTimeOffset GetNextQCDate(SecurityClass security, Guid equipmentGuid, Guid testSetDefinitionGuid, string assetType, DateTimeOffset qcDate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateAppointmentBasedOnDueDate(SecurityClass security, Guid testSetDefinitionGuid, Guid entityGuid, bool equipment, DateTimeOffset dueDate);

		[OperationContract]
		DateTimeOffset GetQCDateForTestSet(SecurityClass security, Guid equipmentGuid, Guid testSetDefinitionGuid, string assetType, DateTimeOffset startDate, DateTimeOffset endDate);

		[OperationContract]
		DateTimeOffset GetNextQCDateForAsset(SecurityClass security, Guid equipmentGuid, string assetType, DateTimeOffset startDate);
	}
}
