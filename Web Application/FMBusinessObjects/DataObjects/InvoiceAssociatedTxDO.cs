using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class InvoiceAssociatedTxDO : BaseAssociatedTxDO
	{
		#region Public enumerations
		/// <summary>
		/// Defines the types of date filters that can be used to search
		/// Invoice Associated Transactions
		/// </summary>
		public enum DateFilters
		{
			None,
			TransactionDate,
			InventoryDate
		}
		#endregion

		#region Private data members
		[DataMember]
		private string poNumber;
		[DataMember]
		private string voucherNumber;
		[DataMember]
		private string invoiceNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default construct for the invoice associated transaction
		/// data object class.
		/// </summary>
		public InvoiceAssociatedTxDO()
		{
		}
		#endregion

		#region Properties

		public string InvoiceNumber
		{
			set { this.invoiceNumber = value; }
			get { return this.invoiceNumber; }
		}

		public string PONumber
		{
			set { this.poNumber = value; }
			get { return this.poNumber; }
		}

		public string VoucherNumber
		{
			set { this.voucherNumber = value; }
			get { return this.voucherNumber; }
		}	
		#endregion

		#region Public override methods
		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getSelectCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion

		#region Public methods
		public string getSearchCommand(InvoiceAssociatedTxDO.SearchCriteria criteria)
		{
			string transTypeFilter;
			if (criteria.transType == TransactionTypes.T21_AccountPayableInvoice)
			{
				transTypeFilter = "AND t.LookupTransTypeIndex = 8 ";
			}
			else
			{
				transTypeFilter = "AND t.LookupTransTypeIndex = 5 ";
			}

			string dateFilterSql;
			if (criteria.dateFilter == DateFilters.InventoryDate)
			{
				dateFilterSql = "AND t.InventoryDate BETWEEN @startDate AND @endDate ";
			}
			else if (criteria.dateFilter == DateFilters.TransactionDate)
			{
				dateFilterSql = "AND t.TransDateTime BETWEEN @startDate AND @endDate ";
			}
			else
			{
				dateFilterSql = "";
			}

			string sql = 
				// Get the unassigned transactions
				"SELECT " +
					"t.TransID, t.TransDateTime, t.InventoryDate, t.AliasName, t.PONumber, t.DocumentNumber, " +
					"t.SupplierID, t.OwnerID, t.ManagerID, t.BillToID, t.ShipToID, li.Product, li.InvoiceNumber " +
				"FROM " +
					"tblTransactions t JOIN tblTransactionLineItems li ON t.TransactionGuid = li.TransactionGuid " + 
					"AND li.Product = @product " +
				"WHERE " +
					"((t.ManagerID = @managerID) OR (@managerID IS NULL)) " +
					transTypeFilter + dateFilterSql +
					"AND ((t.OwnerID = @ownerID) OR (@ownerID IS NULL)) " +
					"AND ((t.ShipToID = @shipTo) OR (@shipTo IS NULL)) " +
					"AND ((t.BillToID = @billTo) OR (@billTo IS NULL)) " +
					"AND ((t.PONumber = @poNumber) OR (@poNumber IS NULL)) " +
					"AND ((t.DocumentNumber = @voucher) OR (@voucher IS NULL)) " +
					"AND ((t.SupplierID = @vendor) OR (@vendor IS NULL)) " +
					"AND ((t.SiteGuid = @SiteGuid) OR (@SiteGuid IS NULL)) " +
					"AND t.TransID NOT IN " +
						"(SELECT LinkedTransID FROM tblTransactionLinks)";
			
			return sql;
		}

		public string getSelectAssociatedTxCommand()
		{
			string sql = 
				"SELECT " +
					"l.LinkedTransID AS TransID, t.TransDateTime, t.InventoryDate, t.AliasName, t.PONumber, t.DocumentNumber, " +
					"t.SupplierID, t.OwnerID, t.ManagerID, t.BillToID, t.ShipToID, " +
					"(SELECT Product FROM tblTransactionLineItems WHERE TransactionLineItemGuid = @TransactionLineItemGuid) AS Product, " +
					"(SELECT InvoiceNumber FROM tblTransactionLineItems WHERE TransactionLineItemGuid = @TransactionLineItemGuid) AS InvoiceNumber " +
				"FROM " +
					"tblTransactionLinks l JOIN tblTransactions t ON l.LinkedTransID = t.TransID " +
				"WHERE " +
					"l.OriginalTransID = @transID AND l.TransactionLineItemGuid = @TransactionLineItemGuid";

			return sql;
		}
		#endregion

		#region Search Criteria class
		/// <summary>
		/// Used to filter a list of Invoice Associated Transactions
		/// </summary>
		[DataContract]
      [Serializable]
		public class SearchCriteria
		{
			#region Constructors
			/// <summary>
			/// This is the default constructor for the search criteria class.
			/// </summary>
			public SearchCriteria() 
			{
			}
			#endregion

			#region Public data members
			[DataMember] public DateTimeOffset startDate;
			[DataMember] public DateTimeOffset endDate;
			[DataMember] public DateFilters dateFilter;
			[DataMember] public string manager;
			[DataMember] public string owner;
			[DataMember] public string vendor;
			[DataMember] public string poNumber;
			[DataMember] public string shipTo;
			[DataMember] public string billTo;
			[DataMember] public string voucher;
			[DataMember] public Guid transactionLineItemGuid;
			[DataMember] public TransactionTypes transType;
			[DataMember] public Guid siteGuid;
			[DataMember] public string product;
			[DataMember] public string transID;
			#endregion
		}
		#endregion
	}
}
