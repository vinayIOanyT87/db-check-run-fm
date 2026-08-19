using System;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace FMBusinessServices.DataAccessLayer
{
	internal static class PointTagsDao
	{

		internal static void DeleteTags(this PointTag tag, SecurityClass security, Guid pointGuid)
		{
			var consolidatedDa = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				cmd.CommandText = "[dbo].[usp_PointTagDeleteByPointGuid]";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@PointGuid", pointGuid );
				consolidatedDa.ExecuteQuery( security, cmd );
			}
		}

		internal static void ModifyTag(this PointTag tag, SecurityClass security)
		{
			var consolidatedDa = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				tag.SetModifyStamp( security );
				tag.AutoGenerateModifyProcSQL( cmd, "usp_PointTagUpdateByPK" );
				if ( ( tag.OpcUaServerGuid == Guid.Empty ) && cmd.Parameters.Contains( "@OpcUaServerGuid" ) )
				{
					cmd.Parameters["@OpcUaServerGuid"].Value = DBNull.Value;
					cmd.Parameters.AddWithValue("@NullOverrideOpcUaServerGuid", 1);
				}

				if( tag.Value == null && cmd.Parameters.Contains("@Value"))
				{
					cmd.Parameters["@Value"].Value = DBNull.Value;
					cmd.Parameters.AddWithValue("@NullOverrideValue", 1);
				}

				if (tag.OpcUaServerDataType == null)
				{
					cmd.Parameters.AddWithValue("@NullOverrideOpcUaServerDataType", 1);
				}

				if (tag.OpcUaWriteHoldoffTime == null)
				{
					cmd.Parameters.AddWithValue("@NullOverrideOpcUaWriteHoldoffTime", 1);
				}

				if (tag.OpcUaWritePeriodicUpdateInterval == null)
				{
					cmd.Parameters.AddWithValue("@NullOverrideOpcUaWritePeriodicUpdateInterval", 1);
				}

				if (tag.PointTemplateTagGuid == Guid.Empty)
				{
					cmd.Parameters["@PointTemplateTagGuid"].Value = DBNull.Value;
					cmd.Parameters.AddWithValue("@NullOverridePointTemplateTagGuid", 1);
				}


				consolidatedDa.ExecuteQuery( security, cmd );
			}
		}

		internal static PointTag Get( this PointTag tag, SecurityClass security, Guid tagGuid )
		{
			var consolidatedDa = new ConsolidatedDAClass();

			DataSet dataSet;

			using ( var cmd = new SqlCommand() )
			{
				tag.SelectSql( cmd, tagGuid );
				dataSet = consolidatedDa.GetDataSet( cmd, security );
			}

			DataTable table = dataSet.Tables[0];

			if ( table.Rows.Count > 0 )
			{
				tag.AutoLoad( table.Rows[0] );
			}

			return tag;
		}

		internal static void SelectSql( this PointTag tag, SqlCommand cmd, Guid tagGuid )
		{
			cmd.CommandText = "SELECT T.*, OUAS.ServerEndPoint, OUAS.SecurityMode, OUAS.SecurityPolicy, OUAS.MessageEncoding, OUAS.UserIdentityMethod, OUAS.UserId, OUAS.UserPassword, OUAS.UserCertificatePath, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType, P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID FROM tblPointTag T"
												+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
												+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
												+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
												+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
												+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
												+ " WHERE PointTagGuid = @PointTagGuid";

			cmd.Parameters.AddWithValue( "@PointTagGuid", tagGuid );
		}

		internal static void EnumerateByPointSql( this PointTag tag, SqlCommand cmd, Guid pointGuid )
		{
			cmd.CommandText = "SELECT T.*, OUAS.ServerEndPoint, OUAS.SecurityMode, OUAS.SecurityPolicy, OUAS.MessageEncoding, OUAS.UserIdentityMethod, OUAS.UserId, OUAS.UserPassword, OUAS.UserCertificatePath, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType, P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID, ptt.WellKnownIdentityGuid"
 								+ " FROM tblPointTag T"
								+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
								+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
								+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
								+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
								+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
								+ " LEFT JOIN tblPointTemplateTag ptt ON T.PointTemplateTagGuid = ptt.PointTemplateTagGuid"
								+ " WHERE T.PointGuid = @PointGuid ORDER BY T.ID";

			cmd.Parameters.AddWithValue( "@PointGuid", pointGuid );
		}


		internal static void EnumerateGuidAndIdByPointGuidSql(this PointTag tag, SqlCommand cmd, Guid siteGuid, Guid userGuid, Guid pointGuid) 
		{
			cmd.CommandText += "SET NOCOUNT ON";

			// Select Point Access Groups that map to User Groups to which the User is mapped for the current site
			cmd.CommandText += " DECLARE @PointAccessGroupGuidTable TABLE(PointAccessGroupGuid UniqueIdentifier)";

			cmd.CommandText += " INSERT INTO @PointAccessGroupGuidTable"
									+ " SELECT DISTINCT pagtug.PointAccessGroupGuid FROM map.tblUserToGroup utg"
									+ " INNER JOIN map.tblPointAccessGroupToUserGroup pagtug ON pagtug.UserGroupGuid = utg.GroupGuid"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON pag.PointAccessGroupGuid = pagtug.PointAccessGroupGuid AND pag.SiteGuid = utg.SiteGuid"
									+ " WHERE utg.SiteGuid = @SiteGuid AND utg.UserGuid = @UserGuid";

			// Select Point, PointAccessGroup for which the Point Template is mapped or the Point is mapped
			cmd.CommandText += " IF OBJECT_ID('tempdb.#PointTable') IS NOT NULL"
									+ " DROP TABLE tempdb.#PointTable"
									+ " CREATE TABLE tempdb.#PointTable"
									+ " ("
									+ "		PointGuid UniqueIdentifier,"
									+ "		PointAccessGroupGuid UniqueIdentifier"
									+ " )";
		
			cmd.CommandText	+= " INSERT INTO #PointTable SELECT DISTINCT PointGuid, PointAccessGroupGuid FROM"
									+ " (SELECT p.PointGuid, pagtpt.PointAccessGroupGuid FROM dbo.tblPoint p"
									+ " INNER JOIN map.tblPointAccessGroupToPointTemplate pagtpt ON pagtpt.PointTemplateGuid = p.PointTemplateGuid"
									+ " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtpt.PointAccessGroupGuid"
									+ " WHERE p.SiteGuid = @SiteGuid"
									+ " UNION"
									+ " SELECT p.PointGuid, pagtp.PointAccessGroupGuid FROM dbo.tblPoint p"
									+ " INNER JOIN map.tblPointAccessGroupToPoint pagtp ON pagtp.PointGuid = p.PointGuid"
									+ " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtp.PointAccessGroupGuid"
									+ " WHERE p.SiteGuid = @SiteGuid AND p.PointGuid = @PointGuid) s";

			// Select Tag related information
			cmd.CommandText	+= " IF OBJECT_ID('tempdb.#TagTable') IS NOT NULL"
									+ " DROP TABLE tempdb.#TagTable"
									+ " CREATE TABLE tempdb.#TagTable"
									+ " ("
									+ "		ID nvarchar(50),"
									+ "		PointTagGuid UniqueIdentifier,"
									+ "		PointTemplateTagGuid UniqueIdentifier,"
									+ "		PointGuid UniqueIdentifier,"
									+ "		PointTemplateGuid UniqueIdentifier,"
									+ "		ValueType nvarchar(MAX)"
									+ " )";


			cmd.CommandText += " INSERT INTO #TagTable"
									+ " SELECT pt.ID, pt.PointTagGuid, pt.PointTemplateTagGuid, p.PointGuid, p.PointTemplateGuid, pt.ValueType FROM dbo.tblPointTag pt"
									+ " INNER JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid"
									+ " WHERE pt.PointGuid = @PointGuid";

			// Select Tags that are accessible to Point Access groups either with no mapping or a mapping that has View or Modify = 1
			cmd.CommandText += " SELECT DISTINCT PointTagGuid, ID, ValueType FROM #TagTable tt"
									+ " INNER JOIN #PointTable pt ON pt.PointGuid = tt.PointGuid"
									+ " LEFT JOIN map.tblPointAccessGroupToTag pagt ON pagt.TagGuid = tt.PointTemplateTagGuid AND pagt.PointAccessGroupGuid = pt.PointAccessGroupGuid"
									+ " LEFT JOIN map.tblPointAccessGroupToPointTag pagpt ON pagpt.TagGuid = tt.PointTagGuid AND pagpt.PointAccessGroupGuid = pt.PointAccessGroupGuid"
									+ " WHERE (tt.PointTemplateTagGuid IS NOT NULL AND tt.ValueType <> 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND (pagt.[View] = CAST(1 as BIT) OR pagt.[View] IS NULL OR pagt.Modify = CAST(1 as BIT) OR pagt.Modify IS NULL))"
									+ " OR (tt.PointTemplateTagGuid IS NULL OR tt.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND (pagpt.[View] = CAST(1 as BIT) OR pagpt.[View] IS NULL OR pagpt.Modify = CAST(1 as BIT) OR pagpt.Modify IS NULL))"
									+ " ORDER BY ID";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
		}



		internal static void EnumerateBySiteSql( this PointTag tag, SqlCommand cmd, Guid siteGuid )
		{
			cmd.CommandText = "SELECT T.*, OUAS.ServerEndPoint, OUAS.SecurityMode, OUAS.SecurityPolicy, OUAS.MessageEncoding, OUAS.UserIdentityMethod, OUAS.UserId, OUAS.UserPassword, OUAS.UserCertificatePath, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType, P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID"
									+ " FROM tblPointTag T"
									+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
									+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
									+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
									+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
									+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
									+ " WHERE P.SiteGuid = @SiteGuid ORDER BY P.PointGuid,T.ID";

			cmd.Parameters.AddWithValue( "@SiteGuid", siteGuid );
		}

		internal static void EnumerateTagsAssociatedWithDeviceAlarmMapBySiteGuidSql(this PointTag tag, SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "SELECT T.*, OUAS.ServerEndPoint, OUAS.SecurityMode, OUAS.SecurityPolicy, OUAS.MessageEncoding, OUAS.UserIdentityMethod, OUAS.UserId, OUAS.UserPassword, OUAS.UserCertificatePath, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType, P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID"
									+ " FROM tblPointTag T"
									+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
									+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
									+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
									+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
									+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
									+ " WHERE P.SiteGuid = @SiteGuid AND (T.PointTemplateTagGuid IS NULL OR T.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference')"
									+ " ORDER BY P.ID,T.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}



		internal static void EnumerateByPointListSql(SqlCommand cmd, List<Guid> pointGuidList, List<string> tagIDFilter)
		{
			cmd.CommandType = CommandType.Text;

			cmd.CommandText = "SELECT T.*, OUAS.ServerEndPoint, OUAS.SecurityMode, OUAS.SecurityPolicy, OUAS.MessageEncoding, OUAS.UserIdentityMethod, OUAS.UserId, OUAS.UserPassword, OUAS.UserCertificatePath, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType, P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID, AI.AlarmPriorityGuid, AI.AlarmState, AI.Acknowledged, ptt.WellKnownIdentityGuid"
									+ " FROM tblPointTag T"
									+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
									+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
									+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
									+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
									+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
									+ " OUTER APPLY"
									+ " (SELECT top(1)"
									+ " CASE WHEN ptas.AlarmTestFailed = 1 AND a.ShelvedOneShot = 0 AND a.Suppressed = 0 AND (a.ShelvedEndTimeStamp is null OR a.ShelvedEndTimeStamp < SYSDATETIMEOFFSET()) THEN at.AlarmPriorityGuid ELSE"
									+ " CASE WHEN a.ShelvedOneShot = 1 OR (a.ShelvedEndTimeStamp is not null AND a.ShelvedEndTimeStamp > SYSDATETIMEOFFSET()) THEN '00000000-0000-0000-0000-000000000000' ELSE"
									+ " CASE WHEN ptas.Acknowledged = 0 THEN at.NormalUnacknowledgedAlarmPriorityGuid ELSE '00000000-0000-0000-0000-000000000000' END END END AS AlarmPriorityGuid,"
									+ " CASE WHEN ptas.AlarmTestFailed = 1 THEN at.AlarmState ELSE a.NotAlarmState END AS AlarmState,"
									+ " ptas.Acknowledged,"
									+ " CASE WHEN ptas.AlarmTestFailed = 1 THEN ap.Priority * 100 + a.[Order] + at.[Order] ELSE CASE WHEN ptas.Acknowledged = 0 then 10000 + ap.Priority * 100 + a.[Order] + at.[Order] else 100000 END END AS SortIndex"
									+ " FROM tblPointTag ipt"
									+ " LEFT	JOIN tblAlarm a ON a.InputTagGuid = ipt.PointTagGuid AND a.Enabled = CAST(1 AS Bit)"
									+ " LEFT JOIN tblAlarmTest at ON at.AlarmGuid = a.AlarmGuid AND at.Enabled = CAST(1 AS Bit)"
									+ " LEFT JOIN tblPointTagAlarmStatus ptas ON ptas.AlarmTestGuid = at.AlarmTestGuid"
									+ " LEFT JOIN tblAlarmPriorities ap ON ap.AlarmPriorityGuid = at.AlarmPriorityGuid"
									+ " WHERE T.PointTagGuid = ipt.PointTagGuid"
									+ " ORDER by SortIndex, ptas.UpdatedDate DESC) AI"
									+ " INNER JOIN @PointTable ptable ON ptable.Guid = t.PointGuid"
									+ " LEFT JOIN tblPointTemplateTag ptt ON T.PointTemplateTagGuid = ptt.PointTemplateTagGuid";

			if (tagIDFilter != null && tagIDFilter.Count > 0 )
			{
				cmd.CommandText += " JOIN @PointTagFilter ptf ON ptf.ID = T.ID ";
				var PointTagFilterTable = new DataTable();
				PointTagFilterTable.Columns.Add("ID", typeof(string));
				foreach (var tagID in tagIDFilter)
				{
					var row = PointTagFilterTable.NewRow();
					row[0] = tagID;

					PointTagFilterTable.Rows.Add(row);
				}

				SqlParameter tablePointTagFilterParameter = cmd.Parameters.Add("@PointTagFilter", SqlDbType.Structured);
				tablePointTagFilterParameter.Value = PointTagFilterTable;
				tablePointTagFilterParameter.TypeName = "dbo.PointTagIDListType";

			}

			cmd.CommandText += " ORDER BY P.PointGuid,T.ID";

			var pointTable = new DataTable();
			pointTable.Columns.Add("Guid", typeof(Guid));
			foreach (var pointGuid in pointGuidList)
			{
				var row = pointTable.NewRow();
				row[0] = pointGuid;

				pointTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTable", SqlDbType.Structured);
			tableValuedParameter.Value = pointTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}

		internal static void EnumerateTagListByPointAccessSql(SqlCommand cmd, List<Guid> tagGuidList, Guid siteGuid, Guid userGuid)
		{
			cmd.CommandText += "SET NOCOUNT ON";

			// Select Point Access Groups that map to User Groups to which the User is mapped for the current site
			cmd.CommandText	+= " DECLARE @PointAccessGroupGuidTable TABLE(PointAccessGroupGuid UniqueIdentifier)";

			cmd.CommandText	+= " INSERT INTO @PointAccessGroupGuidTable"
									+ " SELECT DISTINCT pagtug.PointAccessGroupGuid FROM map.tblUserToGroup utg"
									+ " INNER JOIN map.tblPointAccessGroupToUserGroup pagtug ON pagtug.UserGroupGuid = utg.GroupGuid"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON pag.PointAccessGroupGuid = pagtug.PointAccessGroupGuid AND pag.SiteGuid = utg.SiteGuid"
									+ " WHERE utg.SiteGuid = @SiteGuid AND utg.UserGuid = @UserGuid";

			// Select Point, PointAccessGroup for which the Point Template is mapped or the Point is mapped
			cmd.CommandText += " IF OBJECT_ID('tempdb.#PointTable') IS NOT NULL"
									+ " DROP TABLE tempdb.#PointTable"
									+ " CREATE TABLE tempdb.#PointTable"
									+ " ("
									+ "		PointGuid UniqueIdentifier,"
									+ "		PointAccessGroupGuid UniqueIdentifier"
									+ " )";

			cmd.CommandText += " INSERT INTO #PointTable SELECT DISTINCT PointGuid, PointAccessGroupGuid FROM"
									+ " (SELECT p.PointGuid, pagtpt.PointAccessGroupGuid FROM dbo.tblPoint p"
									+ " INNER JOIN map.tblPointAccessGroupToPointTemplate pagtpt ON pagtpt.PointTemplateGuid = p.PointTemplateGuid"
									+ " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtpt.PointAccessGroupGuid"
									+ " WHERE p.SiteGuid = @SiteGuid"
									+ " UNION"
									+ " SELECT p.PointGuid, pagtp.PointAccessGroupGuid FROM dbo.tblPoint p"
									+ " INNER JOIN map.tblPointAccessGroupToPoint pagtp ON pagtp.PointGuid = p.PointGuid"
									+ " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtp.PointAccessGroupGuid"
									+ " WHERE p.SiteGuid = @SiteGuid) s";


			// Select Tag related information
			cmd.CommandText += " IF OBJECT_ID('tempdb.#TagTable') IS NOT NULL"
									+ " DROP TABLE tempdb.#TagTable"
									+ " CREATE TABLE tempdb.#TagTable"
									+ " ("
									+ "		PointTagGuid UniqueIdentifier,"
									+ "		PointTemplateTagGuid UniqueIdentifier,"
									+ "		PointGuid UniqueIdentifier,"
									+ "		PointTemplateGuid UniqueIdentifier"
									+ " )";


			cmd.CommandText	+= " INSERT INTO #TagTable"
									+ " SELECT ptgt.Guid as PointTagGuid, pt.PointTemplateTagGuid, p.PointGuid, p.PointTemplateGuid FROM @PointTagGuidTable ptgt"
									+ " INNER JOIN dbo.tblPointTag pt ON pt.PointTagGuid = ptgt.Guid"
									+ " INNER JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid";

			// Select Tags that are accessible to Point Access groups either with no mapping or a mapping that has View or Modify = 1
			cmd.CommandText	+= " SELECT DISTINCT PointTagGuid FROM #TagTable tt"
									+	" INNER JOIN #PointTable pt ON pt.PointGuid = tt.PointGuid"
									+	" LEFT JOIN map.tblPointAccessGroupToTag pagt ON pagt.TagGuid = tt.PointTemplateTagGuid AND pagt.PointAccessGroupGuid = pt.PointAccessGroupGuid"
									+	" WHERE pagt.[View] = CAST(1 as BIT) OR pagt.[View] IS NULL OR pagt.Modify = CAST(1 as BIT) OR pagt.Modify IS NULL";




			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);

			var tagTable = new DataTable();
			tagTable.Columns.Add("Guid", typeof(Guid));
			foreach (var tagGuid in tagGuidList)
			{
				var row = tagTable.NewRow();
				row[0] = tagGuid;

				tagTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTagGuidTable", SqlDbType.Structured);
			tableValuedParameter.Value = tagTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}
      internal static void EnumerateTagListByOpcUaNodeIdSql(SqlCommand cmd, List<string> OpcUaNodeIds)
      {
         cmd.CommandType = CommandType.Text;

				cmd.CommandText += "SELECT DISTINCT pt.PointTagGuid,pt.OpcUaNodeId FROM dbo.tblPointTag pt"
             + " INNER JOIN @OpcUaNodeIdsTable opcids ON pt.OpcUaNodeId = opcids.value";

            var tagTable = new DataTable();
            tagTable.Columns.Add("value", typeof(string));
            foreach (var OpcUaNodeId in OpcUaNodeIds)
            {
                var row = tagTable.NewRow();
                row[0] = OpcUaNodeId;

                tagTable.Rows.Add(row);
            }

            SqlParameter tableValuedParameter = cmd.Parameters.Add("@OpcUaNodeIdsTable", SqlDbType.Structured);
            tableValuedParameter.Value = tagTable;
            tableValuedParameter.TypeName = "dbo.StringListType";
        }
        internal static void EnumerateByTagListSql(SqlCommand cmd, List<Guid> tagGuidList)
		{
			cmd.CommandType = CommandType.Text;

			cmd.CommandText = "SELECT T.ID, T.EngineeringUnitsType, T.EngineeringUnitsIndex,"
							+ " T.DecimalPlaces, T.ServerEngineeringUnitsIndex, T.ValueType,"
							+ " T.Status, T.Value,"
							+ " CASE"
							+ " WHEN (T.Status & 0x80000000) = 0x80000000 THEN T.ServerTimeStamp"
							+ " WHEN T.ServerTimeStamp > ISNULL(AI.PTAS_UpdDate, T.ServerTimeStamp) THEN T.ServerTimeStamp"
							+ " WHEN T.ServerTimeStamp < ISNULL(AI.PTAS_UpdDate, T.ServerTimeStamp) THEN ISNULL(AI.PTAS_UpdDate, T.ServerTimeStamp)"
							+ " ELSE DATEADD(microsecond,1,ISNULL(AI.PTAS_UpdDate, T.ServerTimeStamp))"
							+ " END AS ServerTimeStamp,"
							+ " T.SourceTimeStamp, T.Maximum, T.Minimum,"
							+ " T.PointTagInputOutputTypeIndex, T.LastPointTagInputOutputTypeIndex,"
							+ " T.Input, T.AlarmStatus, T.ApplyPointEngineeringUnits,"
							+ " T.ApplyPointDecimalPlaces, T.ApplyPointMaximum, T.ApplyPointMinimum,"
							+ " T.OpcUaServerGuid, T.OpcUaBrowsePath, T.OpcUaNamespaceUri,"
							+ " T.OpcUaPublishingInterval, T.OpcUaNodeId, T.OpcUaIsReadable,"
							+ " T.OpcUaServerDataType, T.OpcUaWriteHoldoffTime,"
							+ " T.OpcUaWritePeriodicUpdateInterval, T.CreatedDate,"
							+ " T.CreatedBy, T.UpdatedDate, T.UpdatedBy,"
							+ " T._RowVersion, T.PointTagGuid, T.PointGuid,"
							+ " T.PointTemplateTagGuid, T.AlarmsEnabled,"
							+ " T.InhibitInputOutputTypeConfiguration, T.InhibitOverride,"
							+ " T.Deadband, T.Holdoff, T.Archived, T._ClusterIdx,"
							+ " OUAS.ServerEndPoint, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType,"
							+ " P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID,"
							+ " ptt.WellKnownIdentityGuid, AI.AlarmPriorityGuid, AI.AlarmState, AI.Acknowledged "
							+ " FROM tblPointTag T"
							+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
							+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
							+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
							+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
							+ " LEFT JOIN tblPointTemplateTag ptt ON T.PointTemplateTagGuid = ptt.PointTemplateTagGuid"
							+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
							+ " OUTER APPLY"
							+ " (SELECT top(1)"
							+ " CASE WHEN ptas.AlarmTestFailed = 1 AND a.ShelvedOneShot = 0 AND a.Suppressed = 0 AND (a.ShelvedEndTimeStamp is null OR a.ShelvedEndTimeStamp < SYSDATETIMEOFFSET()) THEN at.AlarmPriorityGuid ELSE"
							+ " CASE WHEN a.ShelvedOneShot = 1 OR (a.ShelvedEndTimeStamp is not null AND a.ShelvedEndTimeStamp > SYSDATETIMEOFFSET()) THEN '00000000-0000-0000-0000-000000000000' ELSE"
							+ " CASE WHEN ptas.Acknowledged = 0 THEN at.NormalUnacknowledgedAlarmPriorityGuid ELSE '00000000-0000-0000-0000-000000000000' END END END AS AlarmPriorityGuid,"
							+ " CASE WHEN ptas.AlarmTestFailed = 1 THEN at.AlarmState ELSE a.NotAlarmState END AS AlarmState,"
							+ " ptas.Acknowledged,"
							+ " SWITCHOFFSET(ptas.UpdatedDate, 0) AS PTAS_UpdDate,"
							+ " CASE WHEN ptas.AlarmTestFailed = 1 THEN ap.Priority * 100 + a.[Order] + at.[Order] ELSE CASE WHEN ptas.Acknowledged = 0 then 10000 + ap.Priority * 100 + a.[Order] + at.[Order] else 100000 END END AS SortIndex"
							+ " FROM tblPointTag ipt"
							+ " LEFT	JOIN tblAlarm a ON a.InputTagGuid = ipt.PointTagGuid AND a.Enabled = CAST(1 AS Bit)"
							+ " LEFT JOIN tblAlarmTest at ON at.AlarmGuid = a.AlarmGuid AND at.Enabled = CAST(1 AS Bit)"
							+ " LEFT JOIN tblPointTagAlarmStatus ptas ON ptas.AlarmTestGuid = at.AlarmTestGuid"
							+ " LEFT JOIN tblAlarmPriorities ap ON ap.AlarmPriorityGuid = at.AlarmPriorityGuid"
							+ " WHERE T.PointTagGuid = ipt.PointTagGuid"
							+ " ORDER by SortIndex, ptas.UpdatedDate DESC) AI"
							+ " INNER JOIN @TagTable ttable ON ttable.Guid = t.PointTagGuid"
							+ " ORDER BY P.ID,T.ID";

			var tagTable = new DataTable();
			tagTable.Columns.Add("Guid", typeof(Guid));
			foreach (var tagGuid in tagGuidList)
			{
				var row = tagTable.NewRow();
				row[0] = tagGuid;

				tagTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@TagTable", SqlDbType.Structured);
			tableValuedParameter.Value = tagTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}

      internal static void EnumerateSql( this PointTag tag, SqlCommand cmd )
		{
			cmd.CommandText = "SELECT T.*, OUAS.ServerEndPoint, OUAS.SecurityMode, OUAS.SecurityPolicy, OUAS.MessageEncoding, OUAS.UserIdentityMethod, OUAS.UserId, OUAS.UserPassword, OUAS.UserCertificatePath, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType, P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID"
									+ " FROM tblPointTag T"
									+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
									+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
									+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
									+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
									+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
									+ " ORDER BY P.PointGuid,T.ID";
		}

		internal static void EnumerateForSimulatorSql(this PointTag tag, SqlCommand cmd, string opcUaEndPoint)
		{
			cmd.CommandText = "SELECT T.*, OUAS.ServerEndPoint, OUAS.SecurityMode, OUAS.SecurityPolicy, OUAS.MessageEncoding, OUAS.UserIdentityMethod, OUAS.UserId, OUAS.UserPassword, OUAS.UserCertificatePath, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType, P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID"
											+ " FROM tblPointTag T"
											+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
											+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
											+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
											+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
											+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
											+ " WHERE OUAS.ServerEndPoint = @OpcUaEndPoint"
											+ " ORDER BY P.PointGuid,T.ID";

			cmd.Parameters.AddWithValue("@OpcUaEndPoint", opcUaEndPoint);
		}

		internal static void EnumerateForHostNameSql(this PointTag tag, SqlCommand cmd, string hostname, int startIndex, int count)
		{
			cmd.CommandText = "SELECT T.*, OUAS.ServerEndPoint, P.SiteGuid, P.ID AS PointID, aps.ID AS PointType, P.Description AS PointDescription, P.Enabled AS Enabled, S.ID AS SiteID"
											+ " FROM tblPointTag T"
											+ " LEFT OUTER JOIN tblPoint P ON P.PointGuid = T.PointGuid"
											+ " LEFT JOIN tblSites S ON S.SiteGuid = P.SiteGuid"
											+ " LEFT JOIN tblOpcUaServer OUAS ON OUAS.OpcUaServerGuid = T.OpcUaServerGuid"
											+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
											+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
											+ " WHERE P.PointGuid IN"
											+ " ("
											+ " SELECT P.PointGuid"
											+ " FROM dbo.tblPoint P"
											+ " INNER JOIN map.tblPointToPointService m"
											+ " ON m.PointGuid = P.PointGuid"
											+ " INNER JOIN dbo.tblPointService s"
											+ " ON s.PointServiceGuid = m.PointServiceGuid"
											+ " WHERE s.Hostname = @Hostname AND P.Enabled = 1"
											+ " ORDER BY P.CreatedDate ASC OFFSET @StartingRec ROWS FETCH NEXT @NumRecs ROWS ONLY"
											+ " )"
											+ " ORDER BY P.PointGuid,T.ID";

			cmd.Parameters.AddWithValue("@Hostname", hostname);
			cmd.Parameters.AddWithValue("@StartingRec", startIndex);
			cmd.Parameters.AddWithValue("@NumRecs", count);
		}
	}
}