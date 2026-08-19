namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using DataObjects;

	public interface IPointAccessGroupToPointMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<PointAccessGroupToPointMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify( SecurityClass security, Guid pointAccessGroupGuid, List<PointAccessGroupToPointMap> pointAccessGroupToPointMapList);

	}
}
