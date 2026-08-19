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

	public class PointAccessGroupToExposedSettingMaps : IPointAccessGroupToExposedSettingMaps, IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToExposedSettingMap.PurgeByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeBySiteGuidAndPointTemplateGuid(SecurityClass security, Guid siteGuid, Guid pointTemplateGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToExposedSettingMap.PurgeBySiteGuidAndPointTemplateGuidSQL(cmd, siteGuid, pointTemplateGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeByPointTemplatePropertyGuid(SecurityClass security, Guid pointTemplatePropertyGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToExposedSettingMap.PurgeByPointTemplatePropertyGuidSQL(cmd, pointTemplatePropertyGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeByPointTemplateGuidAndNotInList(SecurityClass security, Guid pointTemplateGuid, List<Guid> propertyGuidList)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			//Add Security Rights
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = " DELETE pagteps FROM map.tblPointAccessGroupToExposedPropertySetting pagteps"
										+ " INNER JOIN dbo.tblPointTemplateProperty ptp ON ptp.PointTemplatePropertyGuid = pagteps.PointSettingGuid AND ptp.PointTemplateGuid = @PointTemplateGuid"
										+ " WHERE pagteps.PointSettingGuid NOT IN (SELECT * from @PointTemplatePropertyGuidTable)";

				using (var pointTemplatePropertyGuidTable = new DataTable())
				{
					pointTemplatePropertyGuidTable.Columns.Add("PointTemplatePropertyGuid", typeof(Guid));

					foreach (var propertyGuid in propertyGuidList)
					{
						pointTemplatePropertyGuidTable.Rows.Add(propertyGuid);
					}

					var pList = new SqlParameter("@PointTemplatePropertyGuidTable", SqlDbType.Structured);
					pList.TypeName = "dbo.GuidListType";
					pList.Value = pointTemplatePropertyGuidTable;

					cmd.Parameters.Add(pList);
					cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);

					ConsolidatedDa.ExecuteQuery(security, cmd);
				}
			}
		}



		public List<PointAccessGroupToExposedSettingMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToExposedSettingMap.EnumerateByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupToExposedSettingMapList = new List<PointAccessGroupToExposedSettingMap>();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroupToExposedSettingMap = new PointAccessGroupToExposedSettingMap();

				pointAccessGroupToExposedSettingMap.AutoLoad(row);

				pointAccessGroupToExposedSettingMapList.Add(pointAccessGroupToExposedSettingMap);
			}

			return pointAccessGroupToExposedSettingMapList;
		}


		public void Modify(SecurityClass security, Guid pointAccessGroupGuid, List<PointAccessGroupToExposedSettingMap> pointAccessGroupToExposedSettingMapList)
		{
			security.ThrowIfNull("security");

			using (SqlCommand cmd = new SqlCommand())
			{
				PointAccessGroupToExposedSettingMaps.AddDeleteUpdateStoredProcedure(cmd, pointAccessGroupGuid, pointAccessGroupToExposedSettingMapList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public static void AddDeleteUpdateStoredProcedure(SqlCommand cmd, Guid pointAccessGroupGuid, List<PointAccessGroupToExposedSettingMap> pointAccessGroupToExposedSettingMapList, SecurityClass security)
		{
			if (pointAccessGroupToExposedSettingMapList == null)
			{
				pointAccessGroupToExposedSettingMapList = new List<PointAccessGroupToExposedSettingMap>();
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointAccessGroupToExposedSettingFullUpdate";

			var table = new DataTable();
			table.Columns.Add("PointAccessGroupToExposedSettingGuid", typeof(Guid));
			table.Columns.Add("ExposedSettingGuid", typeof(Guid));
			table.Columns.Add("PropertyID", typeof(string));
			table.Columns.Add("PointAccessGroupGuid", typeof(Guid));
			table.Columns.Add("ValueType", typeof(int));
			table.Columns.Add("View", typeof(Boolean));
			table.Columns.Add("Modify", typeof(Boolean));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var pointAccessGroupToSettingMap in pointAccessGroupToExposedSettingMapList)
			{

				var row = table.NewRow();
				row["PointAccessGroupToExposedSettingGuid"] = pointAccessGroupToSettingMap.PointAccessGroupToExposedSettingGuid;
				row["ExposedSettingGuid"] = pointAccessGroupToSettingMap.ExposedSettingGuid;
				row["PropertyID"] = pointAccessGroupToSettingMap.PropertyID;
				row["PointAccessGroupGuid"] = pointAccessGroupToSettingMap.PointAccessGroupGuid;
				row["ValueType"] = (int)pointAccessGroupToSettingMap.ValueType;
				row["View"] = pointAccessGroupToSettingMap.View;
				row["Modify"] = pointAccessGroupToSettingMap.Modify;
				row["UpdatedBy"] = security.UserID;

				table.Rows.Add(row);

			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointAccessGroupToExposedSettingTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.PointAccessGroupToExposedSettingDataType";

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

			else if(typeof(PointTemplateProperty).IsInstanceOfType(Object))
			{
				var pointTemplateProperty = Object as PointTemplateProperty;
				this.PurgeByPointTemplatePropertyGuid(security, pointTemplateProperty.PointTemplatePropertyGuid);
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