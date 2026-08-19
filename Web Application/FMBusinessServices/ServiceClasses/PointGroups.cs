// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointGroups.cs" company="Varec, Inc.">
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
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Service providing access to point group configuration data.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointGroups : FMServiceBase, IPointGroups
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public PointGroups()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PointGroup pointGroup)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroup == null)
			{
				throw new ArgumentNullException("pointGroup");
			}

			if (pointGroup.PointGroupGuid == Guid.Empty)
			{
				pointGroup.PointGroupGuid = Guid.NewGuid();
			}

			if (pointGroup.PointGroupColumn.PointGroupColumnsGuid == Guid.Empty)
			{
				pointGroup.PointGroupColumn.PointGroupColumnsGuid = Guid.NewGuid();
			}

			if (pointGroup.PointGroupRow.PointGroupRowsGuid == Guid.Empty)
			{
				pointGroup.PointGroupRow.PointGroupRowsGuid = Guid.NewGuid();
			}

			using (var cmd = new SqlCommand())
			{
				pointGroup.PointGroupColumn.PointGroupGuid = pointGroup.PointGroupGuid;
				pointGroup.PointGroupRow.PointGroupGuid = pointGroup.PointGroupGuid;

				pointGroup.SetCreationStamp(security);
				pointGroup.AutoGenerateInsertProcSQL(cmd, "usp_PointGroupInsert");

				this.consolidatedDA.ExecuteQuery(security, cmd);

				new PointGroupColumns().Add(security, pointGroup.PointGroupColumn);
				new PointGroupRows().Add(security, pointGroup.PointGroupRow);


			}
			return pointGroup.PointGroupGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pointGroupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			var pointGroup = this.Get(security, pointGroupGuid, security.UserGuid, security.SiteGuid);
			if (pointGroup.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Point Group not found.");
			}

			// Delete Rows
			var rows = new PointGroupRows();
			rows.Purge(security, pointGroup.PointGroupRow.PointGroupRowsGuid);

			// Delete Columns
			var columns = new PointGroupColumns();
			columns.Purge(security, pointGroup.PointGroupColumn.PointGroupColumnsGuid);


			// Delete point
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "gsp_PointGroupDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointGroupGuid", pointGroupGuid);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PointGroup pointGroup)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pointGroup == null)
			{
				throw new ArgumentNullException("pointGroup");
			}

			using (var cmd = new SqlCommand())
			{
				pointGroup.SetCreationStamp(security);
				pointGroup.AutoGenerateModifyProcSQL(cmd, "usp_PointGroupUpdateByPK");

				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			new PointGroupColumns().Modify(security, pointGroup.PointGroupColumn);
			new PointGroupRows().Modify(security, pointGroup.PointGroupRow);

		}

		public PointGroup Get(SecurityClass security, Guid pointGroupGuid, Guid userGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGuid == null)
			{
				throw new ArgumentNullException("userGuid");
			}

			if (siteGuid == null)
			{
				throw new ArgumentNullException("siteGuid");
			}

			var pointGroup = new PointGroup();
			DataSet set;
			// get the main PointGroup data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointGroupGetByPK";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@pointGroupGuid", pointGroupGuid);
//				cmd.Parameters.AddWithValue("@userGuid", userGuid);
//				cmd.Parameters.AddWithValue("@siteGuid", siteGuid);

				this.consolidatedDA.ExecuteQuery(security, cmd);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointGroup.AutoLoad(table.Rows[0]);
			}

			pointGroup.PointGroupColumn = new PointGroupColumns().GetByPointGroupGuid(security, pointGroupGuid);
			pointGroup.PointGroupRow = new PointGroupRows().GetByPointGroupGuid(security, pointGroupGuid);

			return pointGroup;

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid? GetDuplicate(SecurityClass security, string id, int pointGroupType, Guid ownerUserGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}


			if (id == null)
			{
				throw new ArgumentNullException("id");
			}

			if (ownerUserGuid == null)
			{
				throw new ArgumentNullException("ownerUserGuid");
			}

			if (siteGuid == null)
			{
				throw new ArgumentNullException("siteGuid");
			}

			var pointGroup = new PointGroup();
			DataSet set;
			// get the main PointGroup data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointGroupGetDuplicate";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ID", id);
				cmd.Parameters.AddWithValue("@PointGroupType", pointGroupType);
				cmd.Parameters.AddWithValue("@OwnerUserGuid", ownerUserGuid);
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointGroup.AutoLoad(table.Rows[0]);
			}
			return pointGroup.PointGroupGuid;
		}

		public PointGroupCollection EnumerateByUserSite(SecurityClass security, Guid userGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGuid == null)
			{
				throw new ArgumentNullException("userGuid");
			}

			if (siteGuid == null)
			{
				throw new ArgumentNullException("siteGuid");
			}

			var pointGroupList = new PointGroupCollection();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointGroupEnumerateByUserSite";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@userGuid", userGuid);
				cmd.Parameters.AddWithValue("@siteGuid", siteGuid);

				this.consolidatedDA.ExecuteQuery(security, cmd);

				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var pointGroup = new PointGroup();

				pointGroup.AutoLoad(row);
				pointGroupList.Add(pointGroup);

			}

			return pointGroupList;
		}

	}
}