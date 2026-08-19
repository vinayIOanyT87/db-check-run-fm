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

	public class PointAccessGroupToTagMaps : IPointAccessGroupToTagMaps, IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToTagMap.PurgeByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeBySiteGuidAndPointTemplateGuid(SecurityClass security, Guid siteGuid, Guid pointTemplateGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToTagMap.PurgeBySiteGuidAndPointTemplateGuidSQL(cmd, siteGuid, pointTemplateGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeByPointTemplateGuidAndNotInList(SecurityClass security, Guid pointTemplateGuid, List<Guid> tagList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = " DELETE pagtt FROM map.tblPointAccessGroupToTag pagtt"
										+ " INNER JOIN dbo.tblPointTemplateTag ptt ON ptt.PointTemplateGuid = @PointTemplateGuid AND ptt.PointTemplateTagGuid = pagtt.TagGuid"
										+ " WHERE pagtt.TagGuid NOT IN (SELECT * from @PointTemplateTagGuidList)";

				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("PointTemplateTagGuid", typeof(Guid));

					foreach (var pointTemplateTagGuid in tagList)
					{
						parameterTempTable.Rows.Add(pointTemplateTagGuid);
					}

					var pList = new SqlParameter("@PointTemplateTagGuidList", SqlDbType.Structured);
					pList.TypeName = "dbo.GuidListType";
					pList.Value = parameterTempTable;

					cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
					cmd.Parameters.Add(pList);

					ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}
		}


		public void PurgeByPointTemplateTagGuid(SecurityClass security, Guid pointTemplateTagGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToTagMap.PurgeByPointTemplateTagGuidSQL(cmd, pointTemplateTagGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


		public List<PointAccessGroupToTagMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToTagMap.EnumerateByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupToTagMapList = new List<PointAccessGroupToTagMap>();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroupToTagMap = new PointAccessGroupToTagMap();

				pointAccessGroupToTagMap.AutoLoad(row);

				pointAccessGroupToTagMapList.Add(pointAccessGroupToTagMap);

			}

			return pointAccessGroupToTagMapList;
		}


		public void Modify(SecurityClass security, Guid pointAccessGroupGuid, List<PointAccessGroupToTagMap> pointAccessGroupToTagMapList)
		{
			security.ThrowIfNull("security");

			using (SqlCommand cmd = new SqlCommand())
			{
				PointAccessGroupToTagMaps.AddDeleteUpdateStoredProcedure(cmd, pointAccessGroupGuid, pointAccessGroupToTagMapList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public static void AddDeleteUpdateStoredProcedure(SqlCommand cmd, Guid pointAccessGroupGuid, List<PointAccessGroupToTagMap> pointAccessGroupToTagMapList, SecurityClass security)
		{
			if (pointAccessGroupToTagMapList == null)
			{
				pointAccessGroupToTagMapList = new List<PointAccessGroupToTagMap>();
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointAccessGroupToTagFullUpdate";

			var table = new DataTable();
			table.Columns.Add("PointAccessGroupToTagGuid", typeof(Guid));
			table.Columns.Add("TagGuid", typeof(Guid));
			table.Columns.Add("PointAccessGroupGuid", typeof(Guid));
			table.Columns.Add("View", typeof(Boolean));
			table.Columns.Add("Modify", typeof(Boolean));
			table.Columns.Add("ExceedRange", typeof(Boolean));
			table.Columns.Add("Override", typeof(Boolean));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var pointAccessGroupToTagMap in pointAccessGroupToTagMapList)
			{

				var row = table.NewRow();
				row["PointAccessGroupToTagGuid"] = pointAccessGroupToTagMap.PointAccessGroupToTagGuid;
				row["TagGuid"] = pointAccessGroupToTagMap.PointTemplateTagGuid;
				row["PointAccessGroupGuid"] = pointAccessGroupToTagMap.PointAccessGroupGuid;
				row["View"] = pointAccessGroupToTagMap.View;
				row["Modify"] = pointAccessGroupToTagMap.Modify;
				row["ExceedRange"] = pointAccessGroupToTagMap.ExceedRange;
				row["Override"] = pointAccessGroupToTagMap.Override;
				row["UpdatedBy"] = security.UserID;

				table.Rows.Add(row);

			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointAccessGroupToTagTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.PointAccessGroupToTagDataType";

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

			else if (typeof(PointTemplateTag).IsInstanceOfType(Object))
			{
				var pointTemplateTag = Object as PointTemplateTag;
				this.PurgeByPointTemplateTagGuid(security, pointTemplateTag.PointTemplateTagGuid);
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