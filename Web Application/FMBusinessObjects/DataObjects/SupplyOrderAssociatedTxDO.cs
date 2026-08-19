using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	[KnownType(typeof(AssociatedTxDO))]
	[KnownType(typeof(SupplyOrderAssociatedTxLineItemDO))]
	public class SupplyOrderAssociatedTxDO : DataObject
	{
		#region Private data members
		[DataMember]
		private BaseCollections transactions = null;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the supply order associated
		/// transaction data object class.
		/// </summary>
		public SupplyOrderAssociatedTxDO ( )
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

		public override void GetSelectCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public override void GetInsertCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public override void GetDeleteCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public override void GetUpdateCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Public methods
		public void GetSelectCommand ( SqlCommand cmd, SecurityClass Security, Guid transactionLineItemGuid, string Sort )
		{
			string SQL = "SELECT";

			SQL += " b.TransID as [TransactionID]";
			SQL += ",b.AliasName as [TransactionAlias]";
			SQL += ",b.LookupTransactionStatusIndex";
			SQL += ",b.TransDateTime as [TransactionDate]";
			SQL += ",b.InventoryDate";
			SQL += ",b.DocumentNumber";
			SQL += ",b.PONumber";
			SQL += ",b.SupplierID";
			SQL += ",b.ManagerID";
			SQL += ",b.OwnerID";
			SQL += ",b.BillToID";
			SQL += ",b.ShipperID";
			SQL += ",b.ShipToID";
			SQL += ",b.CarrierID";
			SQL += ",b.Site";

			// Join
			SQL += " FROM tblTransactionLineItems a";
			SQL += " INNER JOIN tblTransactions b ON a.TransactionGuid = b.TransactionGuid";

			// Where clause
			SQL += " WHERE a.OrderReferenceTransactionLineItemGuid = @TransactionLineItemGuid";
			SQL += " AND (b.DeleteFlag = 0 OR b.DeleteFlag = NULL)";

			if (Sort != null && ( Sort.Length > 0 ))
			{
				SQL += " ORDER BY b." + Sort;
			}

			// Done
			cmd.CommandText = SQL;
			cmd.Parameters.AddWithValue("@TransactionLineItemGuid", transactionLineItemGuid);
		}
		#endregion
	}
}
