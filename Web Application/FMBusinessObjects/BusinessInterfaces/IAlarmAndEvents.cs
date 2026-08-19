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
	public interface IAlarmAndEvents
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AlarmAndEventClass alarmAndEvent);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string Source, string alarmAndEventID);

		[OperationContract]
		AlarmAndEventClass Get(SecurityClass security, Guid alarmAndEventGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AlarmAndEventClass alarmAndEvent);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid alarmAndEventGuid);

		[OperationContract]
		AlarmAndEventCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		AlarmAndEventCollectionClass EnumerateBySourceAndType(SecurityClass security, string source, string type);

		[OperationContract]
		string[] EnumerateSources(SecurityClass security);

		[OperationContract]
		void CheckLogSize(SecurityClass security, int capacityLimitInRows, int thresholdPercentage);
	}
}
