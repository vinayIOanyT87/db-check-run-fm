
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAlarmTestTemplates
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AlarmTestTemplate alarmTestTemplate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AlarmTestTemplate alarmTestTemplate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid alarmTestTemplateGuid);

		[OperationContract]
		AlarmTestTemplate Get(SecurityClass security, Guid alarmTestTemplateGuid);

		[OperationContract]
		Dictionary<Guid, AlarmTestTemplate> EnumerateByAlarmTestTemplateGuids(SecurityClass security, List<Guid> alarmTestTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>> EnumerateByAlarmTemplateGuids(SecurityClass security, List<Guid> alarmTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>> EnumerateByPointTemplateGuids(SecurityClass security, List<Guid> pointTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, AlarmTestTemplate>> EnumerateByPointTemplateTagGuids(SecurityClass security, List<Guid> pointTemplateTagGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModifyAlarmTestTemplates(SecurityClass security, List<AlarmTestTemplate> alarmTestTemplateList, bool enableAdd, bool enableModify);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmTestTemplates(SecurityClass security, List<Guid> alarmTestTemplateGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmTestTemplatesByAlarmTemplateGuidList(SecurityClass security, List<Guid> alarmTemplateGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmTestTemplatesByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid);
	}
}
