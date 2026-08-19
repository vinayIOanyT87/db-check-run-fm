using System;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IAlarmPriorityEmailGroupMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, AlarmPriorityEmailGroupMapClass alarmPriorityEmailGroupMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, string id, Guid emailGroupGuid, Guid alarmPriorityGuid);
	}
}
