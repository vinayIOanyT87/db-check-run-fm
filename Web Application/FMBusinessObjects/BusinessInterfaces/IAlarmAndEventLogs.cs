using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel.Dispatcher;

	using FMBusinessObjects.Exceptions;

	[ServiceContract]
	public interface IAlarmAndEventLogs
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		[FaultContractAttribute(typeof(FMAlarmAndEventLogException))]
		void Add(SecurityClass security, AlarmAndEventLogClass alarmAndEventLog);

        /// <summary>
        /// Add multiple alarm and event log records at once
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="alarmAndEventLogs">The alarm and event log records to add</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
		[FaultContractAttribute(typeof(FMAlarmAndEventLogException))]
        void AddList(SecurityClass security, List<AlarmAndEventLogClass> alarmAndEventLogs);

		[OperationContract]
		[FaultContractAttribute(typeof(FMAlarmAndEventLogException))]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AlarmAndEventLogClass alarmAndEventLog);

		[OperationContract]
		[FaultContractAttribute(typeof(FMAlarmAndEventLogException))]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeOldRecords(SecurityClass security);

		[OperationContract]
		[FaultContractAttribute(typeof(FMAlarmAndEventLogException))]
		AlarmAndEventLogCollectionClass Enumerate(SecurityClass security,
																DateTimeOffset beginning,
																DateTimeOffset ending,
																string source,
																string type,
																string id,
																string categoryID,
																string priorityID,
																bool includeMemberSites,
                                                                bool queryArchiveDb,
                                                                bool includeGlobalSites);

		[OperationContract]
		[FaultContractAttribute(typeof(FMAlarmAndEventLogException))]
		AlarmAndEventLogCollectionClass EnumerateBySequenceNumber(SecurityClass security, long sequenceNumber);

        [OperationContract]
		[FaultContractAttribute(typeof(FMRowCountThresholdException))]
		[FaultContractAttribute(typeof(FMAlarmAndEventLogException))]
        void CheckLogSize(SecurityClass security, int capacityLimitInRows, int thresholdPercentage);
	}
}
