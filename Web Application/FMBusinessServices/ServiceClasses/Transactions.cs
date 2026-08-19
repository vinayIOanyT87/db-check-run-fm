namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class Transactions : FMServiceBase, ITransactions
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void SaveFastEntryTransaction(SecurityClass security, TransactionDO transaction)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (transaction.TransactionGuid == Guid.Empty)
			{
				this.AddTransaction(security, transaction);
			}
			else
			{
				this.ModifyTransaction(security, transaction);
			}
		}

		private void AddTransaction(SecurityClass security, TransactionDO transaction)
		{
			transaction.SiteGuid = security.SiteGuid;
			transaction.CreatedDate = DateTimeOffset.Now;
			transaction.CreatedBy = security.UserID;
			transaction.UpdatedDate = transaction.CreatedDate;
			transaction.UpdatedBy = security.UserID;
			transaction.TransactionGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				transaction.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);

				foreach (var lineItem in transaction.LineItems)
				{
					lineItem.TransactionLineItemGuid = Guid.NewGuid();
					this.AddLineItem(security, transaction.TransactionGuid, lineItem);
				}
			}
		}

		private void AddLineItem(SecurityClass security, Guid transactionGuid, LineItemDO lineItem)
		{
			using (var cmd = new SqlCommand())
			{
				lineItem.InsertSQL(cmd, transactionGuid);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		private void ModifyTransaction(SecurityClass security, TransactionDO transaction)
		{
			throw new NotImplementedException("ModifyTransaction");
		}
	}

	public static class TransactionHelpers
	{
		public static void InsertSQL(this TransactionDO trans, SqlCommand cmd)
		{
			// TODO: Add full transaction record save
			// TODO: This is only prototype code to prove out saving to the database.  It is by no means production worthy.
			var sql = "insert into dbo.tblTransactions ";
			sql += "(TransactionGuid," + "FuelAdditiveFlag," + "TransID," + "AliasName," + "SiteGuid," + "Site,"
			       + "InventoryDate," + "DeleteFlag," + "CreatedDate) ";

			sql += " VALUES (@TransactionGuid," + "@FuelAdditiveFlag," + "@TransID," + "@AliasName," + "@SiteGuid," + "@Site,"
			       + "@InventoryDate," + "@DeleteFlag," + "@CreatedDate)";

			cmd.Parameters.AddWithValue("@TransactionGuid", trans.TransactionGuid);
			cmd.Parameters.AddWithValue("@FuelAdditiveFlag", trans.FuelAdditiveFlag);
			cmd.Parameters.AddWithValue("@TransID", trans.TransID);
			cmd.Parameters.AddWithValue("@AliasName", trans.Alias);
			cmd.Parameters.AddWithValue("@SiteGuid", trans.SiteGuid);
			cmd.Parameters.AddWithValue("@Site", trans.Site);
			cmd.Parameters.AddWithValue("@InventoryDate", trans.InventoryDate);
			cmd.Parameters.AddWithValue("@DeleteFlag", trans.DeleteFlag);
			cmd.Parameters.AddWithValue("@CreatedDate", trans.CreatedDate);

			cmd.CommandText = sql;
		}

		public static void InsertSQL(this LineItemDO line, SqlCommand cmd, Guid transactionGuid)
		{
			// TODO: Add full transaction record save
			var sql = "insert into dbo.tblTransactionLineItems ";
			sql += "(TransactionGuid," + "TransactionLineItemGuid," + "GrossQuantity," + "NetQuantity" + ") " + "VALUES ("
			       + "@TransactionGuid, " + "@TransactionLineItemGuid," + "@GrossQuantity," + "@NetQuantity" + ")";

			cmd.Parameters.AddWithValue("@TransactionGuid", transactionGuid);
			cmd.Parameters.AddWithValue("@TransactionLineItemGuid", line.TransactionLineItemGuid);
			cmd.Parameters.AddWithValue("@GrossQuantity", line.GrossInventoryChange);
			cmd.Parameters.AddWithValue("@NetQuantity", line.NetInventoryChange);

			cmd.CommandText = sql;
		}
	}
}
