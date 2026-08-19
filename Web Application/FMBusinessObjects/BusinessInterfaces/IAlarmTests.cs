
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAlarmTests
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AlarmTest alarmTest);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AlarmTest alarmTest);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid alarmTestGuid);

		[OperationContract]
		AlarmTest Get(SecurityClass security, Guid alarmTestGuid);

		[OperationContract]
		Dictionary<Guid,AlarmTest> EnumerateByAlarmTestGuids(SecurityClass security, List<Guid> alarmTestGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, AlarmTest>> EnumerateByAlarmGuids(SecurityClass security, List<Guid> alarmGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, AlarmTest>> EnumerateByPointGuids(SecurityClass security, List<Guid> pointGuidList);

		[OperationContract]
		Dictionary<Guid,Dictionary<Guid, AlarmTest>> EnumerateByPointTagGuids(SecurityClass security, List<Guid> pointTagGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, AlarmTest>> EnumerateByPointLimitTagGuids(SecurityClass security, List<Guid> pointTagGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModifyAlarmTests(SecurityClass security, List<AlarmTest> alarmTestList, bool enableAdd, bool enableModify);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmTests(SecurityClass security, List<Guid> alarmTestGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmTestsByAlarmGuidList(SecurityClass security, List<Guid> alarmGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmTestsByPointGuidList(SecurityClass security, List<Guid> pointGuidList);
	}
}
