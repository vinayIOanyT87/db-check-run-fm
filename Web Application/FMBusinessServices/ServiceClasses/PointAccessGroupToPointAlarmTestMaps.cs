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

	public class PointAccessGroupToPointAlarmTestMaps : IPointAccessGroupToPointAlarmTestMaps, IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		public void PurgeByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointAlarmTestMap.PurgeByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public void PurgeBySiteGuidAndPointTemplateGuid(SecurityClass security, Guid siteGuid, Guid pointTemplateGuid)
		{
			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointAlarmTestMap.PurgeBySiteGuidAndPointTemplateGuidSQL(cmd, siteGuid, pointTemplateGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}


		//public void PurgeAlarmTestTemplatesByAlarmTemplateGuidList(SecurityClass security, List<Guid> alarmTemplateGuidList)
		//{
		//	if (security == null)
		//	{
		//		throw new ArgumentNullException("Security");
		//	}

		//	//Add Security Rights

		//	if (alarmTemplateGuidList == null || alarmTemplateGuidList.Count < 1)
		//	{
		//		return;
		//	}

		//	using (SqlCommand cmd = new SqlCommand())
		//	{
		//		cmd.CommandText = " DELETE pagtat FROM map.tblPointAccessGroupToAlarmTest pagtat"
		//								+ " INNER JOIN dbo.tblAlarmTestTemplate att ON att.AlarmTestTemplateGuid = pagtat.AlarmTestGuid"
		//								+ " WHERE att.AlarmTemplateGuid IN (SELECT * from @AlarmTemplateGuidTable)";

		//		using (var alarmTemplateGuidTable = new DataTable())
		//		{
		//			alarmTemplateGuidTable.Columns.Add("AlarmTemplateGuid", typeof(Guid));

		//			foreach (var alarmTemplateGuid in alarmTemplateGuidList)
		//			{
		//				alarmTemplateGuidTable.Rows.Add(alarmTemplateGuid);
		//			}

		//			var pList = new SqlParameter("@AlarmTemplateGuidTable", SqlDbType.Structured);
		//			pList.TypeName = "dbo.GuidListType";
		//			pList.Value = alarmTemplateGuidTable;

		//			cmd.Parameters.Add(pList);

		//			ConsolidatedDa.ExecuteQuery(security, cmd);
		//		}
		//	}
		//}

		//public void PurgeAlarmTestTemplatesNotInList(SecurityClass security, Guid alarmTemplateGuid, List<Guid> alarmTestTemplateGuidList)
		//{
		//	security.ThrowIfNull("security");

		//	//Add Security Rights

		//	if (alarmTestTemplateGuidList == null || alarmTestTemplateGuidList.Count < 1)
		//	{
		//		return;
		//	}

		//	using (SqlCommand cmd = new SqlCommand())
		//	{
		//		cmd.CommandText = " DELETE pagtat FROM map.tblPointAccessGroupToAlarmTest pagtat"
		//								+ " INNER JOIN dbo.tblAlarmTestTemplate att ON att.AlarmTestTemplateGuid = pagtat.AlarmTestGuid"
		//								+ " WHERE att.AlarmTemplateGuid = @AlarmTemplateGuid AND att.AlarmTestTemplateGuid NOT IN (SELECT AlarmTestTemplateGuid from @AlarmTestTemplateGuidTable)";

		//		using (var alarmTestTemplateGuidTable = new DataTable())
		//		{
		//			alarmTestTemplateGuidTable.Columns.Add("AlarmTestTemplateGuid", typeof(Guid));

		//			foreach (var alarmTestemplateGuid in alarmTestTemplateGuidList)
		//			{
		//				alarmTestTemplateGuidTable.Rows.Add(alarmTestemplateGuid);
		//			}

		//			var pList = new SqlParameter("@AlarmTestTemplateGuidTable", SqlDbType.Structured);
		//			pList.TypeName = "dbo.GuidListType";
		//			pList.Value = alarmTestTemplateGuidTable;

		//			cmd.Parameters.Add(pList);
		//			cmd.Parameters.AddWithValue("@AlarmTemplateGuid", alarmTemplateGuid);

		//			ConsolidatedDa.ExecuteQuery(security, cmd);
		//		}
		//	}
		//}


		public List<PointAccessGroupToPointAlarmTestMap> EnumerateByPointAccessGroupGuid(SecurityClass security, Guid pointAccessGroupGuid)
		{
			security.ThrowIfNull("security");

			// TODO: Check security rights

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointAccessGroupToPointAlarmTestMap.EnumerateByPointAccessGroupGuidSQL(cmd, pointAccessGroupGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			var pointAccessGroupToPointAlarmTestMapList = new List<PointAccessGroupToPointAlarmTestMap>();

			foreach (DataRow row in table.Rows)
			{
				var pointAccessGroupToPointAlarmTestMap = new PointAccessGroupToPointAlarmTestMap();

                pointAccessGroupToPointAlarmTestMap.AutoLoad(row);

                pointAccessGroupToPointAlarmTestMapList.Add(pointAccessGroupToPointAlarmTestMap);

			}

			return pointAccessGroupToPointAlarmTestMapList;
		}


		public void Modify(SecurityClass security, Guid pointAccessGroupGuid, List<PointAccessGroupToPointAlarmTestMap> pointAccessGroupToPointAlarmTestMapList)
		{
			security.ThrowIfNull("security");

			using (SqlCommand cmd = new SqlCommand())
			{
				PointAccessGroupToPointAlarmTestMaps.AddDeleteUpdateStoredProcedure(cmd, pointAccessGroupGuid, pointAccessGroupToPointAlarmTestMapList, security);
				this.ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public static void AddDeleteUpdateStoredProcedure(SqlCommand cmd, Guid pointAccessGroupGuid, List<PointAccessGroupToPointAlarmTestMap> pointAccessGroupToPointAlarmTestMapList, SecurityClass security)
		{
			if (pointAccessGroupToPointAlarmTestMapList == null)
			{
				pointAccessGroupToPointAlarmTestMapList = new List<PointAccessGroupToPointAlarmTestMap>();
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointAccessGroupToPointAlarmTestFullUpdate";

			var table = new DataTable();
			table.Columns.Add("PointAccessGroupToAlarmTestGuid", typeof(Guid));
			table.Columns.Add("AlarmTestTemplateGuid", typeof(Guid));
			table.Columns.Add("PointAccessGroupGuid", typeof(Guid));
			table.Columns.Add("View", typeof(Boolean));
			table.Columns.Add("Acknowledge", typeof(Boolean));
			table.Columns.Add("UpdatedBy", typeof(string));

			foreach (var pointAccessGroupToPointAlarmTestMap in pointAccessGroupToPointAlarmTestMapList)
			{

				var row = table.NewRow();
				row["PointAccessGroupToAlarmTestGuid"] = pointAccessGroupToPointAlarmTestMap.PointAccessGroupToPointAlarmTestGuid;
				row["AlarmTestTemplateGuid"] = pointAccessGroupToPointAlarmTestMap.AlarmTestGuid;
				row["PointAccessGroupGuid"] = pointAccessGroupToPointAlarmTestMap.PointAccessGroupGuid;
				row["View"] = pointAccessGroupToPointAlarmTestMap.View;
				row["Acknowledge"] = pointAccessGroupToPointAlarmTestMap.Acknowledge;
				row["UpdatedBy"] = security.UserID;

				table.Rows.Add(row);

			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointAccessGroupToPointAlarmTestTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.PointAccessGroupToPointAlarmTestDataType";

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