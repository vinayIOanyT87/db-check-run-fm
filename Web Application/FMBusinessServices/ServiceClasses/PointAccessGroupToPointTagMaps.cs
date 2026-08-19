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

	public class PointAccessGroupToPointTagMaps : IPointAccessGroupToPointTagMaps, IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointTagMap.PurgeByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeBySiteGuidAndPointGuid(SecurityClass security, Guid siteGuid, Guid pointGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointTagMap.PurgeBySiteGuidAndPointGuidSQL(cmd, siteGuid, pointGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeByPointGuidAndNotInList(SecurityClass security, Guid pointGuid, List<Guid> tagList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = " DELETE pagtpt FROM map.tblPointAccessGroupToPointTag pagtpt"
										+ " INNER JOIN dbo.tblPointTag pt ON ptt.PointTemplateGuid = @PointGuid AND pt.PointTemplateTagGuid = pagtpt.TagGuid"
										+ " WHERE pagtpt.TagGuid NOT IN (SELECT * from @PointTagGuidList)";

				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("PointTagGuid", typeof(Guid));

					foreach (var pointTagGuid in tagList)
					{
						parameterTempTable.Rows.Add(pointTagGuid);
					}

					var pList = new SqlParameter("@PointTemplateTagGuidList", SqlDbType.Structured);
					pList.TypeName = "dbo.GuidListType";
					pList.Value = parameterTempTable;

					cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
					cmd.Parameters.Add(pList);

					ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}
		}


		public void PurgeByPointTagGuid(SecurityClass security, Guid pointTagGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointTagMap.PurgeByPointTagGuidSQL(cmd, pointTagGuid);
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
				cmd.CommandText = " DELETE pagtpt FROM map.tblPointAccessGroupToPointTag pagtpt"
										+ " INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = pagtpt.TagGuid"
										+ " INNER JOIN dbo.tblPointTemplateTag ptt ON ptt.PointTemplateGuid = @PointTemplateGuid AND ptt.PointTemplateTagGuid = pt.PointTemplateTagGuid"
										+ " WHERE pt.PointTemplateTagGuid IS NOT NULL AND pt.PointTemplateTagGuid NOT IN (SELECT * from @PointTemplateTagGuidList)";

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



		public List<PointAccessGroupToPointTagMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointTagMap.EnumerateByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupToPointTagMapList = new List<PointAccessGroupToPointTagMap>();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroupToPointTagMap = new PointAccessGroupToPointTagMap();

				pointAccessGroupToPointTagMap.AutoLoad(row);

				pointAccessGroupToPointTagMapList.Add(pointAccessGroupToPointTagMap);
			}

			return pointAccessGroupToPointTagMapList;
		}


		public void Modify(SecurityClass security, Guid pointAccessGroupGuid, List<PointAccessGroupToPointTagMap> pointAccessGroupToPointTagMapList)
		{
			security.ThrowIfNull("security");

			using (SqlCommand cmd = new SqlCommand())
			{
				PointAccessGroupToPointTagMaps.AddDeleteUpdateStoredProcedure(cmd, pointAccessGroupGuid, pointAccessGroupToPointTagMapList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public static void AddDeleteUpdateStoredProcedure(SqlCommand cmd, Guid pointAccessGroupGuid, List<PointAccessGroupToPointTagMap> pointAccessGroupToPointTagMapList, SecurityClass security)
		{
			if (pointAccessGroupToPointTagMapList == null)
			{
				pointAccessGroupToPointTagMapList = new List<PointAccessGroupToPointTagMap>();
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointAccessGroupToPointTagFullUpdate";

			var table = new DataTable();
			table.Columns.Add("PointAccessGroupToPointTagGuid", typeof(Guid));
			table.Columns.Add("TagGuid", typeof(Guid));
			table.Columns.Add("PointAccessGroupGuid", typeof(Guid));
			table.Columns.Add("View", typeof(Boolean));
			table.Columns.Add("Modify", typeof(Boolean));
			table.Columns.Add("ExceedRange", typeof(Boolean));
			table.Columns.Add("Override", typeof(Boolean));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var pointAccessGroupToPointTagMap in pointAccessGroupToPointTagMapList)
			{

				var row = table.NewRow();
				row["PointAccessGroupToPointTagGuid"] = pointAccessGroupToPointTagMap.PointAccessGroupToPointTagGuid;
				row["TagGuid"] = pointAccessGroupToPointTagMap.PointTagGuid;
				row["PointAccessGroupGuid"] = pointAccessGroupToPointTagMap.PointAccessGroupGuid;
				row["View"] = pointAccessGroupToPointTagMap.View;
				row["Modify"] = pointAccessGroupToPointTagMap.Modify;
				row["ExceedRange"] = pointAccessGroupToPointTagMap.ExceedRange;
				row["Override"] = pointAccessGroupToPointTagMap.Override;
				row["UpdatedBy"] = security.UserID;

				table.Rows.Add(row);

			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointAccessGroupToPointTagTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.PointAccessGroupToPointTagDataType";

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

			if (typeof(PointTag).IsInstanceOfType(Object))
			{
				var pointTag = Object as PointTag;
				this.PurgeByPointTagGuid(security, pointTag.PointTagGuid);
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