

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAlarms
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, Alarm alarm);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, Alarm alarm);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid alarmGuid);

		[OperationContract]
		Alarm Get(SecurityClass security, Guid alarmGuid);

		[OperationContract]
		Dictionary<Guid, Alarm> EnumerateByAlarmGuids(SecurityClass security, List<Guid> alarmGuidList );

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, Alarm>> EnumerateByPointGuids(SecurityClass security, List<Guid> pointGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, Alarm>> EnumerateByPointTagGuids(SecurityClass security, List<Guid> pointTagGuidList);

		[OperationContract]
		Dictionary<Guid, Alarm> EnumerateActiveAlarmsBySiteGuid(SecurityClass security, Guid siteGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateShelved(SecurityClass security, List<Alarm> alarmList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateShelvedOneShot(SecurityClass security, List<Alarm> alarmList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModifyAlarms(SecurityClass security, List<Alarm> alarmList, bool enableAdd, bool enableModify);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarms(SecurityClass security, List<Guid> alarmGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmsByPointGuidList(SecurityClass security, List<Guid> pointGuidList);
	}
}
