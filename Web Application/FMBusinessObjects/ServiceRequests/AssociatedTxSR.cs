using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
	/// <summary>
	/// AssociatedTxSR will likely replace the other associated transaction SR
	/// classes in the future.
	/// </summary>
    [Serializable]
    [DataContract]
	[KnownType(typeof(AssociatedTxDO))]
	public class AssociatedTxSR : AccountingServiceRequest
	{
		#region Public data members
		public enum RequestTypes
		{
			GetAvailableTransactions,
			GetAssociatedTransactions,
			GetAssociatedTransactionDetails,
			GetAssociatedAndAvailableTransactions,
			GetAssociatedParentTransactions,
			None
		}

		public enum DateFilters { None, InventoryDate, TransactionDate }
		public enum ProjectTypes { ADF, BSME, NONE };
		#endregion

		#region Private data members

		[DataMember]
		private string product;
		[DataMember]
		private Guid transactionAliasGuid;
		[DataMember]
		private string transID;
		[DataMember]
		private Guid transactionLineItemGuid;
		[DataMember]
		private BaseCollections associatedIDs;
		[DataMember]
		private DateFilters dateFilter = DateFilters.None;
		[DataMember]
		private string startDateStr;
		[DataMember]
		private string endDateStr;
		[DataMember]
		private string manager;
		[DataMember]
		private string owner;
		[DataMember]
		private string supplier;
		[DataMember]
		private string poNumber;
		[DataMember]
		private string shipTo;
		[DataMember]
		private string billTo;
		[DataMember]
		private string documentNumber;
		[DataMember]
		private TransactionDO trans;
		[DataMember]
		private DateTimeOffset startDate;
		[DataMember]
		private DateTimeOffset endDate;
		[DataMember]
		private ProjectTypes projectType;
		[DataMember]
		private RequestTypes requestType;
		[DataMember]
		private Guid currencyGuid;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Associated Transaction Service Request class.
		/// </summary>
		public AssociatedTxSR()
		{
			associatedIDs = new BaseCollections();
			this.projectType = ProjectTypes.NONE;
			this.requestType = RequestTypes.None;
			this.TransTypeID = TransactionTypes.T_Maximum;
			this.currencyGuid = Guid.Empty;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will set or get the Currency Index. A value of zero or less
		/// indicates that the currency index was not set.
		/// </summary>
		public Guid CurrencyGuid
		{
			get { return this.currencyGuid; }
			set { this.currencyGuid = value; }
		}

		public RequestTypes RequestType
		{
			get { return this.requestType; }
			set { this.requestType = value; }
		}

		public TransactionDO Trans
		{
			get { return trans; }
			set { trans = value; }

		}

		public string Product
		{
			get { return product; }
			set { product = value; }
		}

		/// <summary>
		/// The Alias GUID of the parent transaction alias
		/// </summary>
		public Guid TransactionAliasGuid
		{
			get { return transactionAliasGuid; }
			set { transactionAliasGuid = value; }
		}

		/// <summary>
		/// The ID of the parent transaction
		/// </summary>
		public string TransID
		{
			get { return transID; }
			set { transID = value; }
		}

		/// <summary>
		/// The GUID of a line item that contains associated transactions
		/// </summary>
		public Guid TransactionLineItemGuid
		{
			get { return transactionLineItemGuid; }
			set { transactionLineItemGuid = value; }
		}

		/// <summary>
		/// A list of ID's associated with a transaction line item
		/// </summary>
		public BaseCollections AssociatedTransactionIDs
		{
			get { return this.associatedIDs; }
			set { associatedIDs = value; }
		}

		public DateFilters DateFilter
		{
			get { return dateFilter; }
			set { dateFilter = value; }
		}

		public DateTimeOffset StartDate
		{
			get { return this.startDate; }
			set { this.startDate = value; }
		}

		public string StartDateStr
		{
			get { return this.startDateStr; }
			set { this.startDateStr = value; }
		}

		public DateTimeOffset EndDate
		{
			get { return this.endDate; }
			set { this.endDate = value; }
		}

		public string EndDateStr
		{
			get { return this.endDateStr; }
			set { this.endDateStr = value; }
		}

		public string Manager
		{
			get { return manager; }
			set { manager = value; }
		}

		public string Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		public string Supplier
		{
			get { return supplier; }
			set { supplier = value; }
		}

		public string PONumber
		{
			get { return poNumber; }
			set { poNumber = value; }
		}

		public string ShipTo
		{
			get { return shipTo; }
			set { shipTo = value; }
		}

		public string BillTo
		{
			get { return billTo; }
			set { billTo = value; }
		}

		public string DocumentNumber
		{
			get { return documentNumber; }
			set { documentNumber = value; }
		}

		/// <summary>
		/// This property gets and sets the type of project such as ADF or BSME.
		/// </summary>
		public ProjectTypes ProjectType
		{
			get { return this.projectType; }
			set { this.projectType = value; }
		}

		[DataMember]
		public TransactionTypes TransTypeID
		{
			get;
			set;
		}

		#endregion
	}
}
