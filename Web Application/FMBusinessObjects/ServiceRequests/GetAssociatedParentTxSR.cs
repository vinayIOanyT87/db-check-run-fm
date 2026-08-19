using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class GetAssociatedParentTxSR : AccountingServiceRequest
	{
		#region Public data members
		public enum AssociatedParentTxRequest
		{
			GET_ASSOCIATED_PARENT_TX,
			GET_ASSOCIATED_PARENT_TX_LINE,
			GET_ASSOCIATED_PARENT_TX_LINE_PER_DOC,
			GET_ASSOCIATED_PARENT_TX_TRANSPORT_LINE,
			GET_ASSOCIATED_PARENT_TX_TRANSPORT_LINE_PER_DOC,
			GET_ASSOCIATED_TX_BASED_CONTRACT,
			NONE
		};
		#endregion

		#region Private data members
		[DataMember]
		private AssociatedParentTxRequest request;
		[DataMember]
		private TransactionTypes transTypeID;
		[DataMember]
		private string aliasName;
		[DataMember]
		private Guid transactionAliasGuid;
		[DataMember]
		private string transID;
		[DataMember]
		private string associatedDocNumber;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default construct for the Get Associated Parent Tx SR.
		/// </summary>
		public GetAssociatedParentTxSR()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the request subtype.
		/// </summary>
		public AssociatedParentTxRequest SubTypeRequest
		{
			get { return this.request; }
			set { this.request = value; }
		}

		/// <summary>
		/// This property will get and set the Alias Name.
		/// </summary>
		public string AliasName
		{
			get { return this.aliasName; }
			set { this.aliasName = value; }
		}

		/// <summary>
		/// This property will get and set the Alias Guid.
		/// </summary>
		public Guid TransactionAliasGuid
		{
			get { return this.transactionAliasGuid; }
			set { this.transactionAliasGuid = value; }
		}

		/// <summary>
		/// This property will get and set the transaction type.
		/// </summary>
		public TransactionTypes TransTypeID
		{
			get { return this.transTypeID; }
			set { this.transTypeID = value; }
		}

		/// <summary>
		/// This property will get and set the transaction ID.
		/// </summary>
		public string TransID
		{
			get { return this.transID; }
			set { this.transID = value; }
		}

		/// <summary>
		/// This property will get and set the associated document number data member.
		/// </summary>
		public string AssociatedDocNumber
		{
			get { return this.associatedDocNumber; }
			set { this.associatedDocNumber = value; }
		}

		#endregion
	}
}
