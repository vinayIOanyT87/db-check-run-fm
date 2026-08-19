using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class OrderAssociatedTxSR : AccountingServiceRequest
	{
		#region Public enumeration
		public enum RequestTypes { GET_ASSOCIATED_TRANSACTIONS, NONE };
		#endregion

		#region Private data members

		[DataMember]
		private RequestTypes subRequest;
		[DataMember]
		private Guid transactionLineItemGuid = Guid.Empty;
		[DataMember]
		private string sortExpression = "";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the order associated transaction
		/// service request class.
		/// </summary>
		public OrderAssociatedTxSR ( )
		{
			this.subRequest = RequestTypes.NONE;
		}
		#endregion

		#region Properties

		public RequestTypes SubRequest
		{
			get { return this.subRequest; }
			set { this.subRequest = value; }
		}

		public Guid TransactionLineItemGuid
		{
			get { return this.transactionLineItemGuid; }
			set { this.transactionLineItemGuid = value; }
		}

		public string SortExpression
		{
			get { return this.sortExpression; }
			set { this.sortExpression = value; }
		}
		#endregion
	}
}
