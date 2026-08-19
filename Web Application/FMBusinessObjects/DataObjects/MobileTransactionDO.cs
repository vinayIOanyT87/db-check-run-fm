using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
	[Serializable]
	public class MobileTransactionDO
	{
		[DataMember]
		public TransactionSelectionDO transaction = new TransactionSelectionDO();
		[DataMember]
		public TransactionLineItemSelectionCollectionDO transactionLineItems = new TransactionLineItemSelectionCollectionDO();
	}

	[Serializable]
	[CollectionDataContract]
	public class MobileTransactionCollectionDO : List<MobileTransactionDO>
	{
	}
}
