
namespace FMBusinessObjects.BusinessInterfaces
{
	using DataObjects;
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	[ServiceContract]
	public interface IPointAccessGroups
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, PointAccessGroup pointAccessGroup);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, PointAccessGroup pointAccessGroup);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid pointAccessGroupGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<Guid, PointAccessGroup> Enumerate(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<PointAccessGroup> EnumerateByUserGroup(SecurityClass security, Guid userGroupGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid? GetDuplicate(SecurityClass security, string id, Guid siteGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		PointAccessGroup Get(SecurityClass security, Guid pointAccessGroupGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyByList(SecurityClass security, List<PointAccessGroup> pointAccessGroupList);


	}
}
