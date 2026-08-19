using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class DemandAssociatedTxDO : BaseAssociatedTxDO
	{
		#region Private data members
		[DataMember]
		private string shipmentNumber;
		[DataMember]
		private string poNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Demand Associated Transaction data object class.
		/// </summary>
		public DemandAssociatedTxDO()
		{
		}
		#endregion

		#region Properties

		public string ShipmentNumber
		{
			get { return shipmentNumber; }
			set { shipmentNumber = value; }
		}

		public string PONumber
		{
			get { return poNumber; }
			set { poNumber = value; }
		}
		#endregion

		#region Public methods
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

		public void GetSelectAvailableTxCommand(SqlCommand cmd, string product)
		{
			cmd.CommandText = "SELECT " +
					"t.TransID, t.TransDateTime, t.InventoryDate, t.AliasName, t.PONumber, t.ShipmentNumber, " +
					"t.OwnerID, t.ManagerID, t.BillToID, t.ShipToID, li.Product " +
				"FROM " +
					"tblTransactions t JOIN tblTransactionLineItems li ON t.TransactionGuid = li.TransactionGuid " +
					"AND li.Product = @product " +
				"WHERE " +
					"t.LookupTransTypeIndex = 8" +	// Receipt
					"AND t.TransID NOT IN " +
						"(SELECT LinkedTransID FROM tblTransactionLinks)";

			cmd.Parameters.Add("@product", SqlDbType.NVarChar, 30);
			cmd.Parameters["product"].Value = product;
		}

		public void GetSelectAssociatedTxCommand(SqlCommand cmd, Guid transactionLineItemGuid, string transID)
		{
			cmd.CommandText = "SELECT " +
				"l.LinkedTransID AS TransID, t.TransDateTime, t.InventoryDate, t.AliasName, t.PONumber, t.ShipmentNumber, " +
				"t.OwnerID, t.ManagerID, t.BillToID, t.ShipToID, " +
				"(SELECT Product FROM tblTransactionLineItems WHERE TransactionLineItemGuid = @TransactionLineItemGuid) AS Product, " +
				"FROM " +
					"tblTransactionLinks l JOIN tblTransactions t ON l.LinkedTransID = t.TransID " +
				"WHERE " +
					"l.OriginalTransID = @transID AND l.TransactionLineItemGuid = @TransactionLineItemGuid";


			cmd.Parameters.Add("@TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TransactionLineItemGuid"].Value = transactionLineItemGuid;

			cmd.Parameters.Add("@transID", SqlDbType.NVarChar, 64);
			cmd.Parameters["@transID"].Value = transID;

		}
		#endregion

		#region Search Criteria Class
		[DataContract]
      [Serializable]
		public class SearchCriteria
		{
			[DataMember]
			public Guid transactionLineItemGuid;
			[DataMember]
			public TransactionTypes transType = TransactionTypes.T8_Receipt;
			[DataMember]
			public string product;
			[DataMember]
			public string transID;
			[DataMember]
			public Guid siteGuid;

			public SearchCriteria()
			{
			}
		}
		#endregion
	}
}
