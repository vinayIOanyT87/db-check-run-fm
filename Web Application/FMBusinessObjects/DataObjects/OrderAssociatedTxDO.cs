using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	[KnownType(typeof(AssociatedTxDO))]
	[KnownType(typeof(OrderAssociatedTxLineItemDO))]
	public class OrderAssociatedTxDO : DataObject
	{
		#region Private methods
		[DataMember]
		private BaseCollections transactions = null;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the order associated transaction
		/// data object class.
		/// </summary>
		public OrderAssociatedTxDO ( )
		{
			this.transactions = new BaseCollections ( );
		}
		#endregion

		#region Properties

		public BaseCollections Transactions
		{
			get { return this.transactions; }
			set { this.transactions = value; }
		}
		#endregion

		#region Public methods
		public SqlCommand getSelectCommand(SecurityClass Security, Guid transactionLineItemGuid, string Sort)
		{
			const string PARAM_NAME_ORDERLINEREFERENCE = "@OrderReferenceTransactionLineItemGuid";
			const SqlDbType PARAM_TYPE_ORDERLINEREFERENCE = SqlDbType.UniqueIdentifier;
			const string PARAM_NAME_DELETEFLAG = "@DeleteFlag";
			const SqlDbType PARAM_TYPE_DELETEFLAG = SqlDbType.Bit;



			SqlCommand cmd = new SqlCommand();
			string SQL = "SELECT";

			SQL += " b.TransID as [TransactionID]";
			SQL += ",b.AliasName as [TransactionAlias]";
			SQL += ",b.LookupTransactionStatusIndex";
			SQL += ",b.TransDateTime as [TransactionDate]";
			SQL += ",b.InventoryDate";
			SQL += ",b.SupplierID";
			SQL += ",b.ManagerID";
			SQL += ",b.OwnerID";
			SQL += ",b.BillToID";
			SQL += ",b.ShipperID";
			SQL += ",b.ShipToID";
			SQL += ",b.CarrierID";
			SQL += ",b.DocumentNumber";
			SQL += ",b.Site";

			// Join
			SQL += " FROM tblTransactionLineItems a";
			SQL += " INNER JOIN tblTransactions b ON a.TransactionGuid = b.TransactionGuid";

			// Where clause
			SQL += AddParameter(cmd, " WHERE", "a.OrderReferenceTransactionLineItemGuid", "=", PARAM_NAME_ORDERLINEREFERENCE, PARAM_TYPE_ORDERLINEREFERENCE, transactionLineItemGuid) +
					" AND (" +
					AddParameter(cmd, false, "b.DeleteFlag", PARAM_NAME_DELETEFLAG, PARAM_TYPE_DELETEFLAG, 0) +
		 		   "OR b.DeleteFlag IS NULL)";
			

			if (!string.IsNullOrEmpty(Sort))
			{
				SQL += " ORDER BY b." + Sort;
			}

			cmd.CommandText = SQL;
			// Done
			return cmd;
		}
		#endregion

		#region Override methods
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
		#endregion
	}
}
