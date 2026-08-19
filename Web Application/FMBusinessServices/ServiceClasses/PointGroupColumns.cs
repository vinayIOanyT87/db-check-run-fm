// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointGroupColumns.cs" company="Varec, Inc.">
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
	public class PointGroupColumns : FMServiceBase, IPointGroupColumns
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroupColumns()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PointGroupColumn pointGroupColumn)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroupColumn == null)
			{
				throw new ArgumentNullException("pointGroupColumn");
			}

			using (var cmd = new SqlCommand())
			{
				pointGroupColumn.SetCreationStamp(security);
				pointGroupColumn.AutoGenerateInsertProcSQL(cmd, "usp_PointGroupColumnsInsert");

				this.consolidatedDA.ExecuteQuery(security, cmd);

			}
			return pointGroupColumn.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pointGroupColumnGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// Delete point Group Row
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "gsp_PointGroupColumnDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointGroupColumnsGuid", pointGroupColumnGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointGroupColumn pointGroupColumn)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroupColumn == null)
			{
				throw new ArgumentNullException("pointGroupColumn");
			}

			using (var cmd = new SqlCommand())
			{
				pointGroupColumn.SetCreationStamp(security);
				pointGroupColumn.AutoGenerateModifyProcSQL(cmd, "usp_PointGroupColumnsUpdateByPointGroupGuid");

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public PointGroupColumn GetByPointGroupGuid(SecurityClass security, Guid pointGroupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroupGuid == null)
			{
				throw new ArgumentNullException("pointGroupGuid");
			}


			var pointGroupColumn = new PointGroupColumn();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointGroupColumnGetByPointGroupGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@pointGroupGuid", pointGroupGuid);

				this.consolidatedDA.ExecuteQuery(security, cmd);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointGroupColumn.AutoLoad(table.Rows[0]);
			}

			return pointGroupColumn;
		}
	}
}