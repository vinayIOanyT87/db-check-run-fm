// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointGroupRows.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Service providing access to point group data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;
	using System.Web;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Service providing access to point group configuration data.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointGroupRows : FMServiceBase, IPointGroupRows
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroupRows()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PointGroupRow pointGroupRow)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroupRow == null)
			{
				throw new ArgumentNullException("pointGroupColumn");
			}

			using (var cmd = new SqlCommand())
			{
				pointGroupRow.SetCreationStamp(security);
				pointGroupRow.AutoGenerateInsertProcSQL(cmd, "usp_PointGroupRowsInsert");

				this.consolidatedDA.ExecuteQuery(security, cmd);

			}
			return pointGroupRow.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pointGroupRowGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// Delete point Group Row
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "gsp_PointGroupRowsDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointGroupRowsGuid", pointGroupRowGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointGroupRow pointGroupRow)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroupRow == null)
			{
				throw new ArgumentNullException("pointGroupColumn");
			}

			using (var cmd = new SqlCommand())
			{
				pointGroupRow.SetCreationStamp(security);
				pointGroupRow.AutoGenerateModifyProcSQL(cmd, "usp_PointGroupRowsUpdateByPointGroupGuid");

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public PointGroupRow GetByPointGroupGuid(SecurityClass security, Guid pointGroupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroupGuid == null)
			{
				throw new ArgumentNullException("pointGroupGuid");
			}


			var pointGroupRow = new PointGroupRow();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointGroupRowGetBypointGroupGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@pointGroupGuid", pointGroupGuid);

				this.consolidatedDA.ExecuteQuery(security, cmd);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointGroupRow.AutoLoad(table.Rows[0]);
			}

			return pointGroupRow;
		}

	}
}