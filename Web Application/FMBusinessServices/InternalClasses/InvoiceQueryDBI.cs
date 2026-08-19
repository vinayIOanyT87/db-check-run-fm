// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvoiceQueryDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Data;

	using FMBusinessObjects.DataObjects;

	public class InvoiceQueryDBI : BaseDBI
	{
		#region Constants and Fields

		protected InvoiceQueryDO invoiceQueryDO;

		protected SecurityClass security = null;

		private static readonly object singleton = new Object();

		#endregion

		#region Constructors and Destructors

		public InvoiceQueryDBI(SecurityClass inSecurity, string user, DateTimeOffset saveTime)
			: base(user, saveTime)
		{
			this.security = inSecurity;
		}

		#endregion

		#region Public Methods and Operators

		public Guid Save(InvoiceQueryDO queryDO)
		{
			this.invoiceQueryDO = queryDO;

			this.invoiceQueryDO.UpdatedBy = base.user;
			this.invoiceQueryDO.UpdatedDate = DateTimeOffset.Now;

			if (this.invoiceQueryDO.InvoiceQueryGuid != Guid.Empty)
			{
				this.invoiceQueryDO.CreatedBy = this.invoiceQueryDO.UpdatedBy;
				this.invoiceQueryDO.CreatedDate = this.invoiceQueryDO.UpdatedDate;

				this.Insert();
			}
			else
			{
				this.Update();
			}

			return this.invoiceQueryDO.InvoiceQueryGuid;
		}

		#endregion

		#region Methods

		protected void Insert()
		{
			int i = -1;
			this.insertCmd.Parameters[++i].Value = this.invoiceQueryDO.Description;
			this.insertCmd.Parameters[++i].Value = this.invoiceQueryDO.CreatedBy;
			this.insertCmd.Parameters[++i].Value = this.invoiceQueryDO.CreatedDate;
			this.insertCmd.Parameters[++i].Value = this.invoiceQueryDO.UpdatedBy;
			this.insertCmd.Parameters[++i].Value = this.invoiceQueryDO.UpdatedDate;

			lock (singleton)
			{
				this.ConsolidatedDA.ExecuteQuery(this.security, this.insertCmd);
			}
		}

		protected override void PrepareDeleteRemainingStatement()
		{
			// not needed
		}

		protected override void PrepareDeleteStatement()
		{
			// requirement: queries cannot be deleted
		}

		protected override void PrepareInsertStatement()
		{
			this.insertCmd.CommandText =
				"INSERT INTO tblInvoiceQueries VALUES ( @Description, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate )";
			this.insertCmd.Parameters.Add("@Description", SqlDbType.NVarChar, 511);
			this.insertCmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			this.insertCmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			this.insertCmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			this.insertCmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
		}

		protected override void PrepareSelectStatement()
		{
			// not needed
		}

		protected override void PrepareUpdateStatement()
		{
			// requirement: can update descriptions of existing queries
			this.updateCmd.CommandText =
				"UPDATE tblInvoiceQueries SET description = @Description WHERE InvoiceQueryGuid = @InvoiceQueryGuid";

			this.updateCmd.Parameters.Add("@Description", SqlDbType.NVarChar, 511);
			this.updateCmd.Parameters.Add("@InvoiceQueryGuid", SqlDbType.UniqueIdentifier);
			this.updateCmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			this.updateCmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			this.updateCmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			this.updateCmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
		}

		protected void Update()
		{
			int i = -1;
			this.updateCmd.Parameters[++i].Value = this.invoiceQueryDO.Description;
			this.updateCmd.Parameters[++i].Value = this.invoiceQueryDO.InvoiceQueryGuid;
			this.updateCmd.Parameters[++i].Value = this.invoiceQueryDO.CreatedBy;
			this.updateCmd.Parameters[++i].Value = this.invoiceQueryDO.CreatedDate;
			this.updateCmd.Parameters[++i].Value = this.invoiceQueryDO.UpdatedBy;
			this.updateCmd.Parameters[++i].Value = this.invoiceQueryDO.UpdatedDate;

			lock (singleton)
			{
				this.ConsolidatedDA.ExecuteQuery(this.security, this.updateCmd);
			}
		}

		#endregion
	}
}