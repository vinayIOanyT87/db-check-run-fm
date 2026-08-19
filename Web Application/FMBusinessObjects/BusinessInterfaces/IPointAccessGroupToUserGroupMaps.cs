using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using DataObjects;

	public interface IPointAccessGroupToUserGroupMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<PointAccessGroupToUserGroupMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security,Guid pointAccessGroupGuid,List<PointAccessGroupToUserGroupMap> pointAccessGroupToUserGroupMapList);

	}
}
