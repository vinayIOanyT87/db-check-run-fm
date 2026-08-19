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
	public interface IAlarmPriorities
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, AlarmPriorityClass alarmPriority );

      [OperationContract]
      [TransactionFlow(TransactionFlowOption.Allowed)]
      Guid Import(SecurityClass security, AlarmPriorityClass alarmPriority);

      [OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, AlarmPriorityClass alarmPriority );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid alarmPriorityGuid );

		[OperationContract]
		AlarmPriorityClass Get(SecurityClass security, Guid guid);

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string alarmPriorityID );

		[OperationContract]
		AlarmPriorityCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		AlarmPriorityCollectionClass EnumerateByEmailGroup ( SecurityClass security, Guid groupGuid );
	}
}
