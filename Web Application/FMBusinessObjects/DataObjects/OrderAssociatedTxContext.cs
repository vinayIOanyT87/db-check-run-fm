using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class OrderAssociatedTxContext
	{
		[DataMember] public Guid TransactionLineItemGuid = Guid.Empty;
		[DataMember] public string ReturnURL = "";
		[DataMember] public string OrderNumber = "";
		[DataMember] public string CustomerOrderNumber = "";
		[DataMember] public string LineNumber = "";
		[DataMember] public string Product = "";
		[DataMember] public string TransDate = "";

		[DataMember] public TransactionDO transaction = null;
		[DataMember] public TransactionDetailList DetailList = null;
	}
}
