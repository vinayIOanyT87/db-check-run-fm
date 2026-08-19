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
	public interface IListViews
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, ListViewClass listView);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, ListViewClass listView);

		[OperationContract]
		ListViewClass Get(SecurityClass security, LISTVIEW_TYPE listViewType, Guid listViewGuid);

		[OperationContract]
		ListViewClass GetWithProductsAndGroups(SecurityClass security, LISTVIEW_TYPE listViewType, Guid listViewGuid, bool includeProductsAndGroups);

		[OperationContract]
		Guid GetIdentityGuidByID(SecurityClass security, LISTVIEW_TYPE listViewType, Guid typeGuid, string id);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, LISTVIEW_TYPE listViewType, Guid typeGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, LISTVIEW_TYPE listViewType, Guid listViewGuid);

		[OperationContract]
		ListViewCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		ListViewCollectionClass EnumerateByTypeAndTypeGuid(SecurityClass security, LISTVIEW_TYPE listViewType, Guid typeGuid);

		[OperationContract]
		ListViewCollectionClass EnumerateLedgerViewsByProductAndUser(SecurityClass security, Guid productGuid);

		[OperationContract]
		string CreateDefaultListViews(SecurityClass security);

		[OperationContract]
		string CreateDefaultLedgerView(SecurityClass security);
	}
}
