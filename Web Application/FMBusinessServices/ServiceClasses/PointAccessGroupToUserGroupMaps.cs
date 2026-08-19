namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	using InternalClasses;

	public class PointAccessGroupToUserGroupMaps : IPointAccessGroupToUserGroupMaps , IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToUserGroupMap.PurgeByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeBySiteGuidAndUserGroupGuid(SecurityClass security, Guid siteGuid, Guid userGroupGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToUserGroupMap.PurgeBySiteGuidAndUserGroupGuidSQL(cmd, siteGuid, userGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


		public List<PointAccessGroupToUserGroupMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToUserGroupMap.EnumerateByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupToUserGroupMapList = new List<PointAccessGroupToUserGroupMap>();

			var users = new UsersClass();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroupToUserGroupMap = new PointAccessGroupToUserGroupMap();

				pointAccessGroupToUserGroupMap.AutoLoad(row);

				pointAccessGroupToUserGroupMapList.Add(pointAccessGroupToUserGroupMap);
			}

			return pointAccessGroupToUserGroupMapList;
		}


		public void Modify(SecurityClass security, Guid pointAccessGroupGuid, List<PointAccessGroupToUserGroupMap> pointAccessGroupToUserGroupMapList)
		{
			security.ThrowIfNull("security");

			using (SqlCommand cmd = new SqlCommand())
			{
				PointAccessGroupToUserGroupMaps.AddDeleteStoredProcedure(cmd, pointAccessGroupGuid, pointAccessGroupToUserGroupMapList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public static void AddDeleteStoredProcedure(SqlCommand cmd, Guid pointAccessGroupGuid, List<PointAccessGroupToUserGroupMap> pointAccessGroupToUserGroupMapList, SecurityClass security)
		{
			if (pointAccessGroupToUserGroupMapList == null)
			{
				pointAccessGroupToUserGroupMapList = new List<PointAccessGroupToUserGroupMap>();
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointAccessGroupToUserGroupFullUpdate";

			var table = new DataTable();
			table.Columns.Add("PointAccessGroupToUserGroupGuid", typeof(Guid));
			table.Columns.Add("PointAccessGroupGuid", typeof(Guid));
			table.Columns.Add("UserGroupGuid", typeof(Guid));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var pointAccessGroupToPointMap in pointAccessGroupToUserGroupMapList)
			{
				if (pointAccessGroupToPointMap.Assigned)
				{
					var row = table.NewRow();
					row["PointAccessGroupToUserGroupGuid"] = pointAccessGroupToPointMap.PointAccessGroupToUserGroupGuid;
					row["PointAccessGroupGuid"] = pointAccessGroupToPointMap.PointAccessGroupGuid;
					row["UserGroupGuid"] = pointAccessGroupToPointMap.UserGroupGuid;
					row["UpdatedBy"] = security.UserID;

					table.Rows.Add(row);
				}
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointAccessGroupToUserGroupTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.PointAccessGroupToUserGroupDataType";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);

		}

		#region Explicit Interface Methods
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
			{
				var entityToSiteMap = Object as EntityToSiteMapClass;
				if (entityToSiteMap.TypeID == ENTITY_TYPE.GROUP)
				{
					this.PurgeBySiteGuidAndUserGroupGuid(security, entityToSiteMap.SiteGuid, entityToSiteMap.IdentityGuid);
				}
			}
		}
		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}
		#endregion
	}
}