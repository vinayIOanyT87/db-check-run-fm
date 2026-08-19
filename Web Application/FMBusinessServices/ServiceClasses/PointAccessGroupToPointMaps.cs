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

	public class PointAccessGroupToPointMaps : IPointAccessGroupToPointMaps, IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointMap.PurgeByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeByPointGuid(SecurityClass security, Guid pointGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointMap.PurgeByPointGuidSQL(cmd, pointGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


		public List<PointAccessGroupToPointMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointMap.EnumerateByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupToPointMapList = new List<PointAccessGroupToPointMap>();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroupToPointMap = new PointAccessGroupToPointMap();

				pointAccessGroupToPointMap.AutoLoad(row);

				pointAccessGroupToPointMapList.Add(pointAccessGroupToPointMap);

			}

			return pointAccessGroupToPointMapList;
		}

		public void Modify(SecurityClass security, Guid pointAccessGroupGuid, List<PointAccessGroupToPointMap> pointAccessGroupToPointMapList)
		{
			security.ThrowIfNull("security");

			using (SqlCommand cmd = new SqlCommand())
			{
				PointAccessGroupToPointMaps.AddDeleteStoredProcedure(cmd, pointAccessGroupGuid, pointAccessGroupToPointMapList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public static void AddDeleteStoredProcedure(SqlCommand cmd, Guid pointAccessGroupGuid, List<PointAccessGroupToPointMap> pointAccessGroupToPointMapList, SecurityClass security)
		{
			if (pointAccessGroupToPointMapList == null)
			{
				pointAccessGroupToPointMapList = new List<PointAccessGroupToPointMap>();
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointAccessGroupToPointFullUpdate";

			var table = new DataTable();
			table.Columns.Add("PointAccessGroupToPointGuid", typeof(Guid));
			table.Columns.Add("PointAccessGroupGuid", typeof(Guid));
			table.Columns.Add("PointGuid", typeof(Guid));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var pointAccessGroupToPointMap in pointAccessGroupToPointMapList)
			{
				if (pointAccessGroupToPointMap.Assigned)
				{
					var row = table.NewRow();
					row["PointAccessGroupToPointGuid"] = pointAccessGroupToPointMap.PointAccessGroupToPointGuid;
					row["PointAccessGroupGuid"] = pointAccessGroupToPointMap.PointAccessGroupGuid;
					row["PointGuid"] = pointAccessGroupToPointMap.PointGuid;
					row["UpdatedBy"] = security.UserID;

					table.Rows.Add(row);
				}
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointAccessGroupToPointTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.PointAccessGroupToPointDataType";

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

			if (typeof(Point).IsInstanceOfType(Object))
			{
				var point = Object as Point;
				this.PurgeByPointGuid(security, point.PointGuid);
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
