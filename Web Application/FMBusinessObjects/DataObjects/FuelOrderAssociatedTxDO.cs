using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class FuelOrderAssociatedTxDO : BaseAssociatedTxDO
	{
		#region Private data members
		private DateTimeOffset effectiveDate;
		private DateTimeOffset expirationDate;
		private string originStation;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the fuel order associated transaction data
		/// object class.
		/// </summary>
		public FuelOrderAssociatedTxDO ( )
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public DateTimeOffset EffectiveDate
		{
			get { return effectiveDate; }
			set { effectiveDate = value; }
		}

		[DataMember]
		public DateTimeOffset ExpirationDate
		{
			get { return expirationDate; }
			set { expirationDate = value; }
		}

		[DataMember]
		public string OriginStation
		{
			get { return originStation; }
			set { originStation = value; }
		}
		#endregion

		#region Public override methods
		public override string getDeleteCommand ( )
		{
			return null;
		}

		public override string getInsertCommand ( )
		{
			return null;
		}

		public override string getSelectCommand ( )
		{
			return null;
		}

		public override string getUpdateCommand ( )
		{
			return null;
		}

		public string getSelectAssociatedTxCommand ( )
		{
			string sql =
				"SELECT " +
					"l.LinkedTransID AS TransID, t.TransDateTime, t.InventoryDate, t.AliasName, t.EffectiveDate, t.ExpirationDate, " +
					"t.SupplierID, t.OwnerID, t.ManagerID, t.BillToID, t.OriginStationIATAID, " +
					"(SELECT Product FROM tblTransactionLineItems WHERE TransactionLineItemGuid = @TransactionLineItemGuid) AS Product, " +
				"FROM " +
					"tblTransactionLinks l JOIN tblTransactions t ON l.LinkedTransID = t.TransID " +
				"WHERE " +
					"l.OriginalTransID = @transID AND l.TransactionLineItemGuid = @TransactionLineItemGuid";

			return sql;
		}

		public string getSelectAvailableTxCommand ( )
		{
			string sql =
				"SELECT " +
					"t.TransID, t.TransDateTime, t.InventoryDate, t.AliasName, t.EffectiveDate, t.ExpirationDate, " +
					"t.SupplierID, t.OwnerID, t.ManagerID, t.BillToID, t.OriginStationIATAID, li.Product " +
				"FROM " +
					"tblTransactions t JOIN tblTransactionLineItems li ON t.TransactionGuid = li.TransactionGuid " +
					"AND li.Product = @product " +
				"WHERE " +
					"t.LookupTransTypeIndex = 9" +	// Request/Demand
					"AND t.TransID NOT IN " +
						"(SELECT LinkedTransID FROM tblTransactionLinks)";

			return sql;
		}
		#endregion

		#region Search Criteria class
		[Serializable]
		public class SearchCriteria
		{
			public Guid transactionLineItemGuid;
			public string product;
			public string transID;
			public Guid  siteGuid;

			public SearchCriteria ( ) 
			{
			}
		}
		#endregion
	}
}
