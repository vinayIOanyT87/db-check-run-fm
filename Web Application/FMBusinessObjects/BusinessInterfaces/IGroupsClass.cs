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
	public interface IGroups
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, GroupClass group );

		[OperationContract]
		GroupClass Get ( SecurityClass security, Guid groupGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, GroupClass Group );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PropagateCompanyMappings(SecurityClass security, Guid groupGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid groupGuid );

		[OperationContract]
		GroupCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string groupID );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Import ( SecurityClass security, GroupClass group );

		[OperationContract]
		GroupCollectionClass EnumerateByUserDuringLogOn ( SecurityClass security, Guid userGuid );

		[OperationContract]
		GroupCollectionClass EnumerateByUser(SecurityClass security, Guid userGuid);

		[OperationContract]
		GroupCollectionClass EnumerateByUserByGroup(SecurityClass security, Guid userGuid, Guid groupGuid);

		[OperationContract]
		GroupCollectionClass EnumerateAllForGrid(SecurityClass security);

        [OperationContract]
        GroupCollectionClass EnumerateByUserForSiteHierarchy(SecurityClass security, Guid userGuid, Guid siteGuid);


    }
}
