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
	public interface ILedgerAggregateColumns
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, LedgerAggregateColumnClass aggregateColumn );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, LedgerAggregateColumnClass aggregateColumn );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid columnGuid );

		[OperationContract]
		LedgerAggregateColumnClass GetByColumnID ( SecurityClass security, string columnID );

		[OperationContract]
		LedgerAggregateColumnClass GetByColumnGuid ( SecurityClass security, Guid columnGuid );

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string ID );

		[OperationContract]
		LedgerAggregateColumnCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		LedgerAggregateColumnCollectionClass EnumerateByFindText ( SecurityClass security, string findText );
	}
}
