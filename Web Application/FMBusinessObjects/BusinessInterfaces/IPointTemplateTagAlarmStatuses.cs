
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPointTemplateTagAlarmStatuses
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, PointTemplateTagAlarmStatus alarmStatusTemplate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, PointTemplateTagAlarmStatus alarmStatusTemplate);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid alarmStatusTemplateGuid);

		[OperationContract]
		PointTemplateTagAlarmStatus Get(SecurityClass security, Guid alarmStatusTemplateGuid);

		[OperationContract]
		Dictionary<Guid, PointTemplateTagAlarmStatus> EnumerateByAlarmStatusTemplateGuids(SecurityClass security, List<Guid> alarmStatusTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>> EnumerateByAlarmTestTemplateGuids(SecurityClass security, List<Guid> alarmTestTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>> EnumerateByAlarmTemplateGuids(SecurityClass security, List<Guid> alarmTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>> EnumerateByPointTemplateGuids(SecurityClass security, List<Guid> pointTemplateGuidList);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTemplateTagAlarmStatus>> EnumerateByPointTemplateTagGuids(SecurityClass security, List<Guid> pointTemplateTagGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModifyAlarmStatusTemplates(SecurityClass security, List<PointTemplateTagAlarmStatus> alarmStatusTemplateList, bool enableAdd, bool enableModify);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmStatusTemplates(SecurityClass security, List<Guid> alarmStatusTemplateGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmStatusTemplatesByAlarmTemplateGuidList(SecurityClass security, List<Guid> alarmTemplateGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAlarmStatusTemplatesByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid);
	}
}
