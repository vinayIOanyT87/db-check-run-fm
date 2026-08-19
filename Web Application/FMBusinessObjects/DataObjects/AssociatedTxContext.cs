using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   public class AssociatedTxContext
	{
		[DataMember] public string TransactionLineItemGuid			= "";
		[DataMember] public string ReturnURL			= "";
		[DataMember] public string OrderNumber			= "";
		[DataMember] public string CustomerOrderNumber	= "";
		[DataMember] public string LineNumber			= "";
		[DataMember] public string Product				= "";
		[DataMember] public string TransDate			= "";
		[DataMember] public string mode					= "View";
		[DataMember] public string EditItemIndex		= "-1";

		[DataMember] public TransactionDO transaction										= null;
		[DataMember] public BaseCollections allAssociatedTransactionsBeforeTransactionEdit	= null;
		[DataMember] public BaseCollections associatedTransactionsBeforeEdit				= null;

		[DataMember] public TransactionDetailList DetailList				= null;
		[DataMember] public AssociatedTxContext previousAssociatedTxContext	= null;
	}
}
