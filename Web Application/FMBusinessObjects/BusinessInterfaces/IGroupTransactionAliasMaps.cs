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
	public interface IGroupTransactionAliasMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, GroupTransactionAliasMapClass GroupTransactionAliasMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, GroupTransactionAliasMapClass GroupTransactionAliasMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifyCollection ( SecurityClass security, GroupTransactionAliasMapCollectionClass NewGroupTransactionAliasMapCollection, GroupTransactionAliasMapCollectionClass ExistingGroupTransactionAliasMapCollection );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid groupGuid, Guid transactionAliasGuid );
	}
}
