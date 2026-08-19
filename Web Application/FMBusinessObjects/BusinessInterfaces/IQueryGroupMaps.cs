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
	public interface IQueryGroupMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, QueryGroupMapClass QueryGroupMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid queryGuid, Guid groupGuid);
			    
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		GroupCollectionClass EnumerateAssignedGroups(SecurityClass security, Guid queryStorageGuid);


	}
}
