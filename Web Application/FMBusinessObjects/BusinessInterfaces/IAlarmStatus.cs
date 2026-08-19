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
	public interface IAlarmStatus
	{

		[OperationContract]
		List<AlarmStatusClass2> GetActiveAlarms(SecurityClass security, bool unacknowledged, bool unsilenced, bool notify);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AcknowledgeAlarms(SecurityClass security, string comment, List<Guid> alarmGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void SilenceAlarms(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void OpcUaAcknowledgeAlarm(SecurityClass security, Guid alarmStatusIdentityGuid, string alarmStatus);
	}
}
