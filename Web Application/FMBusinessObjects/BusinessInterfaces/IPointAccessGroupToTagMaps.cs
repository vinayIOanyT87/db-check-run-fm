namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using DataObjects;

	public interface IPointAccessGroupToTagMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<PointAccessGroupToTagMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);
	}
}
