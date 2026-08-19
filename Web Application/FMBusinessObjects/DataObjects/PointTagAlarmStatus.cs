
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Data;
	using System.Runtime.CompilerServices;

	[DataContract]
	[Serializable]
	public class PointTagAlarmStatus : BaseDataObject, ICloneable
	{

		[DataMember]
		[FMPersistedField("AlarmGuid", ReadOnly = true)]
		public Guid AlarmGuid { get; private set; }

		[DataMember]
		[FMPersistedField("PointGuid", ReadOnly = true)]
		public Guid PointGuid { get; private set; }

		[DataMember]
		[FMPersistedField("TagGuid", ReadOnly = true)]
		public Guid TagGuid { get; private set; }


		[DataMember]
		[FMPersistedField("PointID", ReadOnly = true)]
		public string PointID { get; private set; }

		[DataMember]
		[FMPersistedField("TagID", ReadOnly = true)]
		public string TagID { get; private set; }

		[DataMember]
		[FMPersistedField("AlarmID", ReadOnly = true)]
		public string AlarmID { get; private set; }

		[DataMember]
		[FMPersistedField("AlarmTestID", ReadOnly = true)]
		public string AlarmTestID { get; private set; }

		[DataMember]
		public new Guid SiteGuid
		{
			get
			{
				return base.SiteGuid;
			}
			set
			{
				base.SiteGuid = value;
			}
		}

		[DataMember]
		public new string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
			}
		}

		[EntityImportExportAttribute("POINTTAGALARMSTATUSGUID*", 200, "PointTagAlarmStatusGuid")]
		[FMPersistedField]
		public Guid PointTagAlarmStatusGuid
		{
			get
			{
				return base.IdentityGuid;
			}

			set
			{
				base.IdentityGuid = value;
			}
		}

		[EntityImportExportAttribute("ALARMTESTGUID", 200, "AlarmTestGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmTestGuid { get; set; }

		[EntityImportExportAttribute("ACKNOWLEDGED", 100, "Acknowledged")]
		[DataMember]
		[FMPersistedField]
		public bool Acknowledged { get; set; }

		[EntityImportExport("ACKNOWLEDGEDTIMESTAMP", 100, "AcknowledgedTimestamp")]
		[DataMember]
		[FMPersistedField]
		public DateTimeOffset? AcknowledgedTimestamp { get; set; }

		[EntityImportExport("ACKNOWLEDGEDBY", 100, "AcknowledgedBy")]
		[DataMember]
		[FMPersistedField]
		public string AcknowledgedBy { get; set; }

		[EntityImportExport("ACKNOWLEDGEDCOMMENT", 100, "AcknowledgedComment")]
		[DataMember]
		[FMPersistedField]
		public string AcknowledgedComment { get; set; }

		[EntityImportExportAttribute("SILENCED", 100, "Silenced")]
		[DataMember]
		[FMPersistedField]
		public bool Silenced { get; set; }

		[EntityImportExport("SILENCEDTIMESTAMP", 100, "SilencedTimestamp")]
		[DataMember]
		[FMPersistedField]
		public DateTimeOffset? SilencedTimestamp { get; set; }

		[EntityImportExport("SILENCEDBY", 100, "SilencedBy")]
		[DataMember]
		[FMPersistedField]
		public string SilencedBy { get; set; }

		[EntityImportExportAttribute("ALARMTESTFAILED", 100, "AlarmTestFailed")]
		[DataMember]
		[FMPersistedField]
		public bool AlarmTestFailed { get; set; }

		[EntityImportExport("ALARMTESTFAILEDTIMESTAMP", 100, "AlarmTestFailedTimestamp")]
		[DataMember]
		[FMPersistedField]
		public DateTimeOffset? AlarmTestFailedTimestamp { get; set; }

		public bool AlarmTestInTimedHoldOff { get; set; }

		public DateTimeOffset? AlarmTestInTimedHoldOffTimestamp { get; set; }

		public bool ReAlarm { get; set; }

		public bool ReAlarmInProgress { get; set; }


		public bool ReAlarmDone { get; set; }

		public Boolean WrittenToEnterprise { get; set; }


		public PointTagAlarmStatus()
		{
			this.PointTagAlarmStatusGuid = Guid.NewGuid();
			this.Acknowledged = true;
			this.AcknowledgedTimestamp = null;
			this.AcknowledgedBy = null;
			this.AcknowledgedComment = null;
			this.Silenced = true;
			this.SilencedTimestamp = null;
			this.SilencedBy = null;
			this.AlarmTestFailed = false;
			this.AlarmTestFailedTimestamp = DateTimeOffset.UtcNow;
			this.AlarmTestInTimedHoldOff = false;
			this.AlarmTestInTimedHoldOffTimestamp = null;
			this.ReAlarm = false;
			this.ReAlarmInProgress = false;
			this.ReAlarmDone = false;
			this.WrittenToEnterprise = false;
		}

		public PointTagAlarmStatus(PointTemplateTagAlarmStatus alarmStatusTemplate, Dictionary<Guid, Guid> alarmTestTemplateGuidToAlarmTestGuidDictionary)
		{
			this.PointTagAlarmStatusGuid = Guid.NewGuid();
			this.AlarmTestGuid = alarmTestTemplateGuidToAlarmTestGuidDictionary[alarmStatusTemplate.AlarmTestTemplateGuid];
			this.Acknowledged = alarmStatusTemplate.Acknowledged;
			this.AcknowledgedTimestamp = alarmStatusTemplate.AcknowledgedTimestamp;
			this.AcknowledgedBy = alarmStatusTemplate.AcknowledgedBy;
			this.AcknowledgedComment = alarmStatusTemplate.AcknowledgedComment;
			this.Silenced = alarmStatusTemplate.Silenced;
			this.SilencedTimestamp = alarmStatusTemplate.SilencedTimestamp;
			this.SilencedBy = alarmStatusTemplate.SilencedBy;
			this.AlarmTestFailed = alarmStatusTemplate.AlarmTestFailed;
			this.AlarmTestFailedTimestamp = alarmStatusTemplate.AlarmTestFailedTimestamp;
			this.AlarmTestInTimedHoldOff = false;
			this.AlarmTestInTimedHoldOffTimestamp = null;
			this.ReAlarm = false;
			this.ReAlarmInProgress = false;
			this.ReAlarmDone = false;
		}

		public object Clone()
		{
			var a = (PointTagAlarmStatus)this.MemberwiseClone();
			this.BaseClone(a);
			return a;
		}

		public bool IsActiveAlarm()
		{
			return (this.Acknowledged == false || (this.Acknowledged == true && this.AlarmTestFailed == true));
		}

		public bool IsUnAcknowledgedAndNormal()
		{
			return (this.Acknowledged == false && this.AlarmTestFailed == false);
		}

		//Do sql below

		public static void EnumerateByTagGuidListSQL(SqlCommand cmd, List<Guid> tagGuidList)
		{
			cmd.CommandText = "SELECT p.ID AS PointID, "
										 + " t.PointGuid AS PointGuid, "
										 + " t.PointTagGuid AS TagGuid, "
										 + " at.AlarmGuid AS AlarmGuid, "
										 + " a.ID AS AlarmID, " + " t.ID AS TagID, "
										 + " at.ID AS AlarmTestID,"
										 + " ptas.* "
										 + " from dbo.tblPointTagAlarmStatus ptas"
										 + " INNER JOIN dbo.tblAlarmTest at"
										 + " ON ptas.AlarmTestGuid = at.AlarmTestGuid"
										 + " INNER JOIN dbo.tblAlarm a"
										 + " ON at.AlarmGuid = a.AlarmGuid"
										 + " INNER JOIN dbo.tblPointTag t"
										 + " ON a.InputTagGuid = t.PointTagGuid"
										 + " INNER JOIN dbo.tblPoint p"
										 + " ON p.PointGuid = t.PointGuid"
										 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.InputTagGuid"
										 + " ORDER BY TagGuid";
			GenerateGuidListTable(cmd, tagGuidList);
		}

		public static void EnumerateByAlarmGuidListSQL(SqlCommand cmd, List<Guid> alarmGuidList)
		{
			cmd.CommandText = "SELECT p.ID AS PointID, "
									+ " t.PointGuid AS PointGuid, "
									+ " t.PointTagGuid AS TagGuid, "
									+ " at.AlarmGuid AS AlarmGuid, "
									+ " a.ID AS AlarmID, " + " t.ID AS TagID, "
									+ " at.ID AS AlarmTestID,"
									+ " ptas.* "
									+ " from dbo.tblPointTagAlarmStatus ptas"
									+ " INNER JOIN dbo.tblAlarmTest at"
									+ " ON ptas.AlarmTestGuid = at.AlarmTestGuid"
									+ " INNER JOIN dbo.tblAlarm a"
									+ " ON at.AlarmGuid = a.AlarmGuid"
									+ " INNER JOIN dbo.tblPointTag t"
									+ " ON a.InputTagGuid = t.PointTagGuid"
									+ " INNER JOIN dbo.tblPoint p"
								   + " ON p.PointGuid = t.PointGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmGuid"
								   + " ORDER BY AlarmGuid";
			GenerateGuidListTable(cmd, alarmGuidList);
		}

		public static void EnumerateByAlarmTestGuidListSQL(SqlCommand cmd, List<Guid> alarmTestGuidList)
		{
			cmd.CommandText = "SELECT p.ID AS PointID, "
																									  + " t.PointGuid AS PointGuid, "
																									  + " t.PointTagGuid AS TagGuid, "
																									  + " at.AlarmGuid AS AlarmGuid, "
																									  + " a.ID AS AlarmID, " + " t.ID AS TagID, "
																									  + " at.ID AS AlarmTestID,"
																									  + " ptas.* "
																									  + " from dbo.tblPointTagAlarmStatus ptas"
																									  + " INNER JOIN dbo.tblAlarmTest at"
																									  + " ON ptas.AlarmTestGuid = at.AlarmTestGuid"
																									  + " INNER JOIN dbo.tblAlarm a"
																									  + " ON at.AlarmGuid = a.AlarmGuid"
																									  + " INNER JOIN dbo.tblPointTag t"
																									  + " ON a.InputTagGuid = t.PointTagGuid"
																									  + " INNER JOIN dbo.tblPoint p"
																									  + " ON p.PointGuid = t.PointGuid"

											+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = ptas.AlarmTestGuid"
								 + " ORDER BY AlarmTestGuid";
			GenerateGuidListTable(cmd, alarmTestGuidList);
		}

		public static void EnumerateByPointTagAlarmStatusGuidListSQL(SqlCommand cmd, List<Guid> pointTagAlarmStatusGuidList)
		{
			cmd.CommandText = "SELECT p.ID AS PointID, "
																									  + " t.PointGuid AS PointGuid, "
																									  + " t.PointTagGuid AS TagGuid, "
																									  + " at.AlarmGuid AS AlarmGuid, "
																									  + " a.ID AS AlarmID, " + " t.ID AS TagID, "
																									  + " at.ID AS AlarmTestID,"
																									  + " ptas.* "
																									  + " from dbo.tblPointTagAlarmStatus ptas"
																									  + " INNER JOIN dbo.tblAlarmTest at"
																									  + " ON ptas.AlarmTestGuid = at.AlarmTestGuid"
																									  + " INNER JOIN dbo.tblAlarm a"
																									  + " ON at.AlarmGuid = a.AlarmGuid"
																									  + " INNER JOIN dbo.tblPointTag t"
																									  + " ON a.InputTagGuid = t.PointTagGuid"
																									  + " INNER JOIN dbo.tblPoint p"
																									  + " ON p.PointGuid = t.PointGuid"

											+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.PointTagAlarmStatusGuid";
			GenerateGuidListTable(cmd, pointTagAlarmStatusGuidList);
		}

		public static void EnumerateByPointListSQL(SqlCommand cmd, List<Guid> pointGuidList)
		{
			cmd.CommandText = "SELECT p.ID AS PointID, "
							  + " t.PointGuid AS PointGuid, "
							  + " t.PointTagGuid AS TagGuid, "
							  + " at.AlarmGuid AS AlarmGuid, "
							  + " a.ID AS AlarmID, " + " t.ID AS TagID, "
							  + " at.ID AS AlarmTestID,"
							  + " ptas.* "
							  + " from dbo.tblPointTagAlarmStatus ptas"
							  + " INNER JOIN dbo.tblAlarmTest at"
							  + " ON ptas.AlarmTestGuid = at.AlarmTestGuid"
							  + " INNER JOIN dbo.tblAlarm a"
							  + " ON at.AlarmGuid = a.AlarmGuid"
							  + " INNER JOIN dbo.tblPointTag t"
							  + " ON a.InputTagGuid = t.PointTagGuid"
							  + " INNER JOIN dbo.tblPoint p"
							  + " ON p.PointGuid = t.PointGuid"										
							  + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointGuid"
							  + " ORDER BY PointGuid";
			GenerateGuidListTable(cmd, pointGuidList);
		}

		protected static DataTable CreateAlarmStatusListDataTable(List<PointTagAlarmStatus> alarmStatusList, SecurityClass security)
		{

			var alarmStatusTable = new DataTable();
			alarmStatusTable.Columns.Add("PointTagAlarmStatusGuid", typeof(Guid));
			alarmStatusTable.Columns.Add("AlarmTestGuid", typeof(Guid));
			alarmStatusTable.Columns.Add("Acknowledged", typeof(bool));
			alarmStatusTable.Columns.Add("AcknowledgedTimestamp", typeof(DateTimeOffset));
			alarmStatusTable.Columns.Add("AcknowledgedBy", typeof(string));
			alarmStatusTable.Columns.Add("AcknowledgedComment", typeof(string));
			alarmStatusTable.Columns.Add("Silenced", typeof(bool));
			alarmStatusTable.Columns.Add("SilencedTimestamp", typeof(DateTimeOffset));
			alarmStatusTable.Columns.Add("SilencedBy", typeof(string));
			alarmStatusTable.Columns.Add("AlarmTestFailed", typeof(bool));
			alarmStatusTable.Columns.Add("AlarmTestFailedTimestamp", typeof(DateTimeOffset));
			alarmStatusTable.Columns.Add("UpdatedBy", typeof(string));
			foreach (var alarmStatus in alarmStatusList)
			{
				var row = alarmStatusTable.NewRow();
				row["PointTagAlarmStatusGuid"] = alarmStatus.PointTagAlarmStatusGuid;
				row["AlarmTestGuid"] = alarmStatus.AlarmTestGuid;
				row["Acknowledged"] = alarmStatus.Acknowledged;
				row["AcknowledgedTimestamp"] = alarmStatus.AcknowledgedTimestamp ?? (object)DBNull.Value;
				row["AcknowledgedBy"] = alarmStatus.AcknowledgedBy;
				row["AcknowledgedComment"] = alarmStatus.AcknowledgedComment;
				row["Silenced"] = alarmStatus.Silenced;
				row["SilencedTimestamp"] = alarmStatus.SilencedTimestamp ?? (object)DBNull.Value;
				row["SilencedBy"] = alarmStatus.SilencedBy;
				row["AlarmTestFailed"] = alarmStatus.AlarmTestFailed;
				row["AlarmTestFailedTimestamp"] = alarmStatus.AlarmTestFailedTimestamp ?? (object)DBNull.Value;
				row["UpdatedBy"] = security.UserID;
				alarmStatusTable.Rows.Add(row);
			}
			return alarmStatusTable;
		}

		public static void AddModifyStoredProcedure(SqlCommand cmd, List<PointTagAlarmStatus> alarmStatusList, SecurityClass security, bool enableAdd, bool enableModify)
		{
			if (alarmStatusList == null || alarmStatusList.Count < 1)
			{
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_PointTagAlarmStatusAddModify";

			var alarmStatusTable = CreateAlarmStatusListDataTable(alarmStatusList, security);

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmStatusTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = alarmStatusTable;
			tableValuedParameter.TypeName = "dbo.PointTagAlarmStatusDataType";
			cmd.Parameters.AddWithValue("@EnableAdd", enableAdd);
			cmd.Parameters.AddWithValue("@EnableModify", enableModify);
		}

		public static void UpdateTestFailedStoredProcedure(SqlCommand cmd, List<PointTagAlarmStatus> alarmStatusList, SecurityClass security)
		{
			if (alarmStatusList == null || alarmStatusList.Count < 1)
			{
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_PointTagAlarmStatusUpdateTestFailed";

			var alarmStatusTable = CreateAlarmStatusListDataTable(alarmStatusList, security);

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmStatusTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = alarmStatusTable;
			tableValuedParameter.TypeName = "dbo.PointTagAlarmStatusDataType";
		}

		public static void UpdateAcknowledgeAndSilenceStoredProcedure(SqlCommand cmd, List<PointTagAlarmStatus> alarmStatusList, SecurityClass security)
		{
			if (alarmStatusList == null || alarmStatusList.Count < 1)
			{
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_PointTagAlarmStatusUpdateAcknowledgeAndSilence";

			var alarmStatusTable = CreateAlarmStatusListDataTable(alarmStatusList, security);

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmStatusTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = alarmStatusTable;
			tableValuedParameter.TypeName = "dbo.PointTagAlarmStatusDataType";
		}

		public static void UpdateSilenceStoredProcedure(SqlCommand cmd, List<PointTagAlarmStatus> alarmStatusList, SecurityClass security)
		{
			if (alarmStatusList == null || alarmStatusList.Count < 1)
			{
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_PointTagAlarmStatusUpdateSilence";

			var alarmStatusTable = CreateAlarmStatusListDataTable(alarmStatusList, security);

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmStatusTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = alarmStatusTable;
			tableValuedParameter.TypeName = "dbo.PointTagAlarmStatusDataType";
		}


		public static void DeleteListSQL(SqlCommand cmd, List<Guid> pointTagAlarmStatusGuidList)
		{
			cmd.CommandText = "DELETE ptas FROM dbo.tblPointTagAlarmStatus ptas"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = ptas.PointTagAlarmStatusGuid";
			GenerateGuidListTable(cmd, pointTagAlarmStatusGuidList);
		}

		public static void DeleteByAlarmGuidListSQL(SqlCommand cmd, List<Guid> alarmGuidList)
		{
			cmd.CommandText = "DELETE ptas FROM dbo.tblPointTagAlarmStatus ptas"
									+ " INNER JOIN tblAlarmTest at"
									+ " ON ptas.AlarmTestGuid = at.AlarmTestGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmGuid";
			GenerateGuidListTable(cmd, alarmGuidList);
		}

		public static void DeleteByPointGuidListSQL(SqlCommand cmd, List<Guid> pointGuidList)
		{
			cmd.CommandText = "DELETE ptas FROM dbo.tblPointTagAlarmStatus ptas"
									+ " INNER JOIN tblAlarmTest at"
									+ " ON ptas.AlarmTestGuid = at.AlarmTestGuid"
									+ " INNER JOIN tblAlarm a"
									+ " ON at.AlarmGuid = a.AlarmGuid"
									+ " INNER JOIN tblPointTag t"
									+ " ON a.InputTagGuid = t.PointTagGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointGuid";
			GenerateGuidListTable(cmd, pointGuidList);
		}
	}

}
