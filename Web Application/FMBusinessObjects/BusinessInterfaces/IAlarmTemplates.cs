

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAlarmTemplates
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AlarmTemplate alarmTemplate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AlarmTemplate alarmTemplate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid alarmTemplateGuid);

		[OperationContract]
		AlarmTemplate Get(SecurityClass security, Guid alarmTemplateGuid);

		[OperationContract]
		Dictionary<Guid, AlarmTemplate> EnumerateByAlarmTemplateGuids(SecurityClass security, List<Guid> alarmTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, AlarmTemplate>> EnumerateByPointTemplateGuids(SecurityClass security, List<Guid> pointTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, AlarmTemplate>> EnumerateByPointTemplateTagGuids(SecurityClass security, List<Guid> pointTemplateTagGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModifyAlarmTemplates(SecurityClass security, List<AlarmTemplate> alarmTemplateList, bool enableAdd, bool enableModify);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmTemplates(SecurityClass security, List<Guid> alarmTemplateGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmTemplatesByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid);
	}
}
