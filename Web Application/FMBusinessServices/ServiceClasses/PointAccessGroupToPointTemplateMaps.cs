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

	public class PointAccessGroupToPointTemplateMaps : IPointAccessGroupToPointTemplateMaps, IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointTemplateMap.PurgeByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeBySiteGuidAndPointTemplateGuid(SecurityClass security, Guid siteGuid, Guid pointTemplateGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointTemplateMap.PurgeBySiteGuidAndPointTemplateGuidSQL(cmd, siteGuid, pointTemplateGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}



		public List<PointAccessGroupToPointTemplateMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointTemplateMap.EnumerateByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupToPointTemplateMapList = new List<PointAccessGroupToPointTemplateMap>();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroupToPointTemplateMap = new PointAccessGroupToPointTemplateMap();

				pointAccessGroupToPointTemplateMap.AutoLoad(row);

				pointAccessGroupToPointTemplateMapList.Add(pointAccessGroupToPointTemplateMap);

			}

			return pointAccessGroupToPointTemplateMapList;
		}

		public void Modify(SecurityClass security, Guid pointAccessGroupGuid, List<PointAccessGroupToPointTemplateMap> pointAccessGroupToPointTemplateMapList)
		{
			security.ThrowIfNull("security");

			using (SqlCommand cmd = new SqlCommand())
			{
				PointAccessGroupToPointTemplateMaps.AddDeleteStoredProcedure(cmd, pointAccessGroupGuid, pointAccessGroupToPointTemplateMapList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public static void AddDeleteStoredProcedure(SqlCommand cmd, Guid pointAccessGroupGuid, List<PointAccessGroupToPointTemplateMap> pointAccessGroupToPointTemplateMapList, SecurityClass security)
		{
			if (pointAccessGroupToPointTemplateMapList == null )
			{
				pointAccessGroupToPointTemplateMapList = new List<PointAccessGroupToPointTemplateMap>();
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointAccessGroupToPointTemplateFullUpdate";

			var table = new DataTable();
			table.Columns.Add("PointAccessGroupToPointTemplateGuid", typeof(Guid));
			table.Columns.Add("PointAccessGroupGuid", typeof(Guid));
			table.Columns.Add("PointTemplateGuid", typeof(Guid));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var pointAccessGroupToPointTemplateMap in pointAccessGroupToPointTemplateMapList)
			{
				if (pointAccessGroupToPointTemplateMap.Assigned)
				{
					var row = table.NewRow();
					row["PointAccessGroupToPointTemplateGuid"] = pointAccessGroupToPointTemplateMap.PointAccessGroupToPointTemplateGuid;
					row["PointAccessGroupGuid"] = pointAccessGroupToPointTemplateMap.PointAccessGroupGuid;
					row["PointTemplateGuid"] = pointAccessGroupToPointTemplateMap.PointTemplateGuid;
					row["UpdatedBy"] = security.UserID;

					table.Rows.Add(row);
				}
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointAccessGroupToPointTemplateTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.PointAccessGroupToPointTemplateDataType";

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
				if (entityToSiteMap.TypeID == ENTITY_TYPE.POINT_TEMPLATE)
				{
					this.PurgeBySiteGuidAndPointTemplateGuid(security, entityToSiteMap.SiteGuid, entityToSiteMap.IdentityGuid);
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