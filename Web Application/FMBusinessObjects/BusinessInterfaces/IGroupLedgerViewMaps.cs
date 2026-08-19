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
	public interface IGroupLedgerViewMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, GroupLedgerViewMapClass groupLedgerViewMap);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid groupGuid, Guid listViewGuid);

		[OperationContract]
		GroupLedgerViewMapCollectionClass EnumerateByListViewGuid(SecurityClass security, Guid listViewGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyCollection(SecurityClass security,
									Guid listViewGuid,
									GroupLedgerViewMapCollectionClass newGroupLedgerViewMapCollection,
									GroupLedgerViewMapCollectionClass existingGroupLedgerViewMapCollection);
	}
}
