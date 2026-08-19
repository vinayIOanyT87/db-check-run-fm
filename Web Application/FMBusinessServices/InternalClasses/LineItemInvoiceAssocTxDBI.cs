// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineItemInvoiceAssocTxDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	public class LineItemInvoiceAssocTxDBI : BaseDBI
	{
		#region Constructors and Destructors

		public LineItemInvoiceAssocTxDBI(string user, DateTimeOffset saveTime)
			: base(user, saveTime)
		{
		}

		#endregion

		#region Public Methods and Operators

		public void Save(LineItemDO lineItem, string transID, SecurityClass security)
		{
			// Get the associations that already exist in the system.
			DataTable dt = this.GetCurrentAssociations(security, lineItem, transID, security.SiteGuid);

			// Get the list of associations that need to be added to the DB
			ArrayList toAdd = this.GetToBeAdded(lineItem, dt);

			// Get the list of associations that need to be removed from the DB
			ArrayList toRemove = this.GetToBeRemoved(lineItem, dt);

			// Add any new associations
			this.Insert(toAdd, transID, lineItem, security);

			// Delete any removed transactions
			this.Delete(toRemove, transID, lineItem, security);
		}

		#endregion

		#region Methods

		protected override void PrepareDeleteRemainingStatement()
		{
			return;
		}

		protected override void PrepareDeleteStatement()
		{
			this.deleteCmd.CommandText = "DELETE " + "tblTransactionLinks " + "WHERE " + "SiteGuid = @SiteGuid "
			                             + "AND OriginalTransID = @original " + "AND LinkedTransID = @linked "
			                             + "AND TransactionLineItemGuid = @TransactionLineItemGuid";

			this.deleteCmd.Parameters.Clear();

			this.deleteCmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			this.deleteCmd.Parameters.Add("@original", SqlDbType.NVarChar, 64);
			this.deleteCmd.Parameters.Add("@linked", SqlDbType.NVarChar, 64);
			this.deleteCmd.Parameters.Add("@TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
		}

		protected override void PrepareInsertStatement()
		{
			this.insertCmd.CommandText = "INSERT INTO tblTransactionLinks ("
			                             + "SiteGuid, OriginalTransID, LinkedTransID, Level, TransactionLineItemGuid, "
			                             + "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) " + "VALUES ("
			                             + "@SiteGuid, @originalTransID, @linkedTransID, @level, @TransactionLineItemGuid, "
			                             + "@createdBy, @createdDate, @updatedBy, @updatedDate)";

			this.insertCmd.Parameters.Clear();

			this.insertCmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			this.insertCmd.Parameters.Add("@originalTransID", SqlDbType.NVarChar, 64);
			this.insertCmd.Parameters.Add("@linkedTransID", SqlDbType.NVarChar, 64);
			this.insertCmd.Parameters.Add("@level", SqlDbType.Int);
			this.insertCmd.Parameters.Add("@TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
			this.insertCmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 100);
			this.insertCmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			this.insertCmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 100);
			this.insertCmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
		}

		protected override void PrepareSelectStatement()
		{
			return;
		}

		protected override void PrepareUpdateStatement()
		{
			return;
		}

		private void Delete(ArrayList toRemove, string transID, LineItemDO lineItem, SecurityClass security)
		{
			foreach (object obj in toRemove)
			{
				string txId = obj.ToString();

				this.PrepareDeleteStatement();
				SqlParameterCollection parms = this.deleteCmd.Parameters;

				parms["@SiteGuid"].Value = security.SiteGuid;
				parms["@original"].Value = transID;
				parms["@linked"].Value = txId;
				parms["@TransactionLineItemGuid"].Value = lineItem.TransactionLineItemGuid;

				this.ConsolidatedDA.ExecuteQuery(security, this.deleteCmd);
			}
		}

		/// <summary>
		/// Returns a list of associated transaction IDs
		/// </summary>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		/// <param name="lineItem">
		/// The line item the transactions are associated with
		/// </param>
		/// <param name="transID">
		/// The transaction the transactions are associated with
		/// </param>
		/// <param name="siteGuid">
		/// </param>
		/// <returns>
		/// A single column DataTable (LinkedTransID) containing a list of
		///     associated transactions
		/// </returns>
		private DataTable GetCurrentAssociations(SecurityClass security, LineItemDO lineItem, string transID, Guid siteGuid)
		{
			// Get the saved associates for the passed line item
			string sql = "SELECT " + "LinkedTransID " + "FROM " + "tblTransactionLinks " + "WHERE "
			             + "OriginalTransID = @transID " + "AND TransactionLineItemGuid = @TransactionLineItemGuid "
			             + "AND SiteGuid = @SiteGuid";

			this.selectCmd.CommandText = sql;
			this.selectCmd.Parameters.Clear();
			this.selectCmd.Parameters.Add("@transID", SqlDbType.NVarChar, 64);
			this.selectCmd.Parameters.Add("@TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
			this.selectCmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			this.selectCmd.Parameters[0].Value = transID;
			this.selectCmd.Parameters[1].Value = lineItem.TransactionLineItemGuid;
			this.selectCmd.Parameters[2].Value = siteGuid;

			DataSet ds = this.ConsolidatedDA.GetDataSet(this.selectCmd, security);

			return ds.Tables[0];
		}

		/// <summary>
		/// Returns a list of Invoice Associated Transactions that need to be added
		///     to the database.
		/// </summary>
		/// <param name="lineItem">
		/// The line item for which associations will be added
		/// </param>
		/// <param name="existing">
		/// Contains the list of existing associations
		/// </param>
		/// <returns>
		/// A collection of Invoice Associated Transactions
		/// </returns>
		private ArrayList GetToBeAdded(LineItemDO lineItem, DataTable existing)
		{
			var toBeAdded = new ArrayList();
			foreach (object obj in lineItem.AssociatedInvoiceTx)
			{
				var tx = (InvoiceAssociatedTxDO)obj;
				bool found = false;

				foreach (DataRow dr in existing.Rows)
				{
					if (dr["LinkedTransID"].ToString() == tx.TransactionID)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					toBeAdded.Add(tx);
				}
			}

			return toBeAdded;
		}

		/// <summary>
		/// Returns a list of Transaction ID's representing Invoice Associated
		///     transactions that need to be removed from the DB
		/// </summary>
		/// <param name="lineItem">
		/// The line item containing the associations to be removed
		/// </param>
		/// <param name="existing">
		/// A list of existing associations
		/// </param>
		/// <returns>
		/// A list of strings representing transaction id's
		/// </returns>
		private ArrayList GetToBeRemoved(LineItemDO lineItem, DataTable existing)
		{
			var toBeRemoved = new ArrayList();

			foreach (DataRow dr in existing.Rows)
			{
				bool found = false;
				string transID = dr["LinkedTransID"].ToString();
				foreach (object obj in lineItem.AssociatedInvoiceTx)
				{
					var tx = (InvoiceAssociatedTxDO)obj;
					if (tx.TransactionID == transID)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					toBeRemoved.Add(transID);
				}
			}

			return toBeRemoved;
		}

		/// <summary>
		/// Inserts a list of Invoice transaction associations into the DB
		/// </summary>
		/// <param name="toAdd">
		/// Contains the list of Invoice Associated Transactions
		/// </param>
		/// <param name="transID">
		/// The parent transaction's ID
		/// </param>
		/// <param name="lineItem">
		/// The parent line item
		/// </param>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		private void Insert(ArrayList toAdd, string transID, LineItemDO lineItem, SecurityClass security)
		{
			foreach (object obj in toAdd)
			{
				var tx = (InvoiceAssociatedTxDO)obj;
				this.PrepareInsertStatement();
				SqlParameterCollection parms = this.insertCmd.Parameters;

				parms["@SiteGuid"].Value = security.SiteGuid;
				parms["@originalTransID"].Value = transID;
				parms["@linkedTransID"].Value = tx.TransactionID;
				parms["@level"].Value = 0; // 0=line item, 1=header
				parms["@TransactionLineItemGuid"].Value = lineItem.TransactionLineItemGuid;
				parms["@createdBy"].Value = security.UserID;
				parms["@createdDate"].Value = DateTimeOffset.Now;
				parms["@updatedBy"].Value = security.UserID;
				parms["@updatedDate"].Value = DateTimeOffset.Now;

				this.ConsolidatedDA.ExecuteQuery(security, this.insertCmd);
			}
		}

		#endregion
	}
}