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
	public interface ISchedules
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, ScheduleClass Schedule );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, ScheduleClass Schedule );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifyCollection ( SecurityClass security, Guid guid, ScheduleCollectionClass NewScheduleCollection, ScheduleCollectionClass ExistingScheduleCollection );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, ScheduleClass Schedule );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void PurgeByIdentityGuid ( SecurityClass security, Guid targetGuid, SCHEDULE_TYPE scheduleType );
	}
}
