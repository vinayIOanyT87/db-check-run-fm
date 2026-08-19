
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPointTagAlarmStatuses
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, PointTagAlarmStatus alarmStatus);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, PointTagAlarmStatus alarmStatus);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid alarmStatusGuid);

		[OperationContract]
		PointTagAlarmStatus Get(SecurityClass security, Guid alarmStatusGuid);

		[OperationContract]
		Dictionary<Guid,PointTagAlarmStatus> EnumerateByAlarmStatusGuids(SecurityClass security, List<Guid> alarmStatusGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>> EnumerateByAlarmTestGuids(SecurityClass security, List<Guid> alarmTestGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>> EnumerateByAlarmGuids(SecurityClass security, List<Guid> alarmGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>> EnumerateByPointGuids(SecurityClass security, List<Guid> pointGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTagAlarmStatus>> EnumerateByPointTagGuids(SecurityClass security, List<Guid> pointTagGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModifyAlarmStatuses(SecurityClass security, List<PointTagAlarmStatus> alarmStatusList, bool enableAdd, bool enableModify);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateTestFailed(SecurityClass security, List<PointTagAlarmStatus> alarmStatusList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Acknowledge(SecurityClass security, List<PointTagAlarmStatus> alarmStatusList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Silence(SecurityClass security, List<PointTagAlarmStatus> alarmStatusList);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmStatuses(SecurityClass security, List<Guid> alarmStatusGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmStatusesByAlarmGuidList(SecurityClass security, List<Guid> alarmGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmStatusesByPointGuidList(SecurityClass security, List<Guid> pointGuidList);
	}
}
