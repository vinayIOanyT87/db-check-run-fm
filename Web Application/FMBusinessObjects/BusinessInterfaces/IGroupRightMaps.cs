using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IGroupRightMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, GroupRightMapClass GroupRightMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid GroupGuid, RIGHT Right );

		/// <summary>
		/// This method determines if a group has a specific security right assigned to it.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="bInTransaction">A bool indicating if this call is wrapped in a transaction</param>
		/// <param name="groupGuid">A Guid representing the unique id of the group</param>
		/// <param name="right">The security right</param>
		/// <returns>A bool indicating whether or not this group has this right assigned</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		bool GroupHasRight(SecurityClass security, bool bInTransaction, Guid groupGuid, RIGHT right);
	}

}
