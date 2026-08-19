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
	public interface IListViewFields
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, ListViewFieldClass listViewField);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, ListViewFieldClass listViewField);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, LISTVIEW_FIELD_TYPE listViewFieldType, Guid listViewFieldGuid);

		[OperationContract]
		ListViewFieldClass Get(SecurityClass security, LISTVIEW_FIELD_TYPE listViewFieldType, Guid listViewFieldGuid);

		[OperationContract]
		ListViewFieldCollectionClass Enumerate(SecurityClass security, Guid listViewGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyCollection(SecurityClass security,
							Guid listViewGuid,
							string listViewID,
							ListViewFieldCollectionClass newListViewFieldCollection,
							ListViewFieldCollectionClass oldListViewFieldCollection);
	}
}
