namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using DataObjects;

	public interface IPointAccessGroupToPointAlarmTestMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<PointAccessGroupToPointAlarmTestMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);
	}
}
