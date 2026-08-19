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
	public interface ILedgerAggregateColumnMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, LedgerAggregateColumnMapClass ledgerAggregateColumnMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, LedgerAggregateColumnMapClass columnMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifyCollection ( SecurityClass security, Guid columnGuid, LedgerAggregateColumnMapCollectionClass columnMaps );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void PurgeCollection ( SecurityClass security, Guid columnGuid );

		[OperationContract]
		LedgerAggregateColumnMapCollectionClass Enumerate(SecurityClass security, Guid ledgerAggregateColumnGuid);
	}
}
