
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
	public class PointTemplateTagAlarmStatus : BaseDataObject, ICloneable
	{

      [EntityImportExportAttribute("ALARMTEMPLATEGUID", 200, "AlarmTemplateGuid")]
      [DataMember]
		[FMPersistedField("AlarmTemplateGuid", ReadOnly = true)]
		public Guid AlarmTemplateGuid { get; private set; }

      [EntityImportExportAttribute("POINTTEMPLATEGUID", 200, "PointTemplateGuid")]
      [DataMember]
		[FMPersistedField("PointTemplateGuid", ReadOnly = true)]
		public Guid PointTemplateGuid { get; private set; }

      [EntityImportExportAttribute("POINTTEMPLATETAGGUID", 200, "PointTemplateTagGuid")]
      [DataMember]
		[FMPersistedField("PointTemplateTagGuid", ReadOnly = true)]
		public Guid PointTemplateTagGuid { get; private set; }

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

      [EntityImportExportAttribute("ALARMSTATUSTEMPLATEGUID*", 200, "AlarmStatusTemplateGuid")]
      [FMPersistedField]
		public Guid PointTemplateTagAlarmStatusGuid
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

      [EntityImportExportAttribute("ALARMTESTTEMPLATEGUID", 200, "AlarmTestTemplateGuid")]    
      [DataMember]
		[FMPersistedField]
		public Guid AlarmTestTemplateGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Acknowledged { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset? AcknowledgedTimestamp { get; set; }

		[DataMember]
		[FMPersistedField]
		public string AcknowledgedBy { get; set; }

		[DataMember]
		[FMPersistedField]
		public string AcknowledgedComment { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Silenced { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset? SilencedTimestamp { get; set; }

		[DataMember]
		[FMPersistedField]
		public string SilencedBy { get; set; }
		[DataMember]
		[FMPersistedField]
		public bool AlarmTestFailed { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset? AlarmTestFailedTimestamp { get; set; }


		public PointTemplateTagAlarmStatus()
		{
			this.PointTemplateTagAlarmStatusGuid = Guid.NewGuid();
			this.Acknowledged = true;
			this.AcknowledgedTimestamp = null;
			this.AcknowledgedBy = null;
			this.AcknowledgedComment = null;
			this.Silenced = true;
			this.SilencedTimestamp = null;
			this.SilencedBy = null;
			this.AlarmTestFailed = false;
			this.AlarmTestFailedTimestamp = null;
		}

		public object Clone()
		{
			var a = (PointTemplateTagAlarmStatus)this.MemberwiseClone();
			this.BaseClone(a);
			return a;
		}


		//Do sql below

		public static void EnumerateByPointTemplateTagGuidListSQL(SqlCommand cmd, List<Guid> pointTemplateTagGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.AlarmTemplateGuid AS AlarmTemplateGuid, ptas.* from tblPointTemplateTagAlarmStatus ptas"
								 + " INNER JOIN tblAlarmTestTemplate at"
								 + " ON ptas.AlarmTestTemplateGuid = at.AlarmTestTemplateGuid"
								 + " INNER JOIN tblAlarmTemplate a"
								 + " ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
								 + " INNER JOIN tblPointTemplateTag t"
								 + " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.InputTemplateTagGuid"
								 + " ORDER BY PointTemplateTagGuid";
			GenerateGuidListTable(cmd, pointTemplateTagGuidList);
		}

		public static void EnumerateByAlarmTemplateGuidListSQL(SqlCommand cmd, List<Guid> alarmTemplateGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.AlarmTemplateGuid AS AlarmTemplateGuid, ptas.* from tblPointTemplateTagAlarmStatus ptas"
								 + " INNER JOIN tblAlarmTestTemplate at"
								 + " ON ptas.AlarmTestTemplateGuid = at.AlarmTestTemplateGuid"
								 + " INNER JOIN tblAlarmTemplate a"
								 + " ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
								 + " INNER JOIN tblPointTemplateTag t"
								 + " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmTemplateGuid"
								 + " ORDER BY AlarmTemplateGuid";
			GenerateGuidListTable(cmd, alarmTemplateGuidList);
		}

		public static void EnumerateByAlarmTestTemplateGuidListSQL(SqlCommand cmd, List<Guid> alarmTestTemplateGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.AlarmTemplateGuid AS AlarmTemplateGuid, ptas.* from tblPointTemplateTagAlarmStatus ptas"
								 + " INNER JOIN tblAlarmTestTemplate at"
								 + " ON ptas.AlarmTestTemplateGuid = at.AlarmTestTemplateGuid"
								 + " INNER JOIN tblAlarmTemplate a"
								 + " ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
								 + " INNER JOIN tblPointTemplateTag t"
								 + " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = ptas.AlarmTestTemplateGuid"
								 + " ORDER BY AlarmTestTemplateGuid";
			GenerateGuidListTable(cmd, alarmTestTemplateGuidList);
		}

		public static void EnumerateByPointTemplateTagAlarmStatusGuidListSQL(SqlCommand cmd, List<Guid> pointTemplateTagAlarmStatusGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.AlarmTemplateGuid AS AlarmTemplateGuid, ptas.* from tblPointTemplateTagAlarmStatus ptas"
								 + " INNER JOIN tblAlarmTestTemplate at"
								 + " ON ptas.AlarmTestTemplateGuid = at.AlarmTestTemplateGuid"
								 + " INNER JOIN tblAlarmTemplate a"
								 + " ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
								 + " INNER JOIN tblPointTemplateTag t"
								 + " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.PointTemplateTagAlarmStatusGuid";
			GenerateGuidListTable(cmd, pointTemplateTagAlarmStatusGuidList);
		}

		public static void EnumerateByPointTemplateListSQL(SqlCommand cmd, List<Guid> pointTemplateGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.AlarmTemplateGuid AS AlarmTemplateGuid, ptas.* from tblPointTemplateTagAlarmStatus ptas"
								 + " INNER JOIN tblAlarmTestTemplate at"
								 + " ON ptas.AlarmTestTemplateGuid = at.AlarmTestTemplateGuid"
								 + " INNER JOIN tblAlarmTemplate a"
								 + " ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
								 + " INNER JOIN tblPointTemplateTag t"
								 + " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointTemplateGuid"
								 + " ORDER BY PointTemplateGuid";
			GenerateGuidListTable(cmd, pointTemplateGuidList);
		}

		public static void AddModifyStoredProcedure(SqlCommand cmd, List<PointTemplateTagAlarmStatus> alarmStatusTemplateList, SecurityClass security, bool enableAdd, bool enableModify)
		{
			if (alarmStatusTemplateList == null || alarmStatusTemplateList.Count < 1)
			{
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_PointTemplateTagAlarmStatusAddModify";

			var alarmStatusTable = new DataTable();
			alarmStatusTable.Columns.Add("PointTemplateTagAlarmStatusGuid", typeof(Guid));
			alarmStatusTable.Columns.Add("AlarmTestTemplateGuid", typeof(Guid));
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
			foreach (var alarmStatus in alarmStatusTemplateList)
			{
				var row = alarmStatusTable.NewRow();
				row["PointTemplateTagAlarmStatusGuid"] = alarmStatus.PointTemplateTagAlarmStatusGuid;
				row["AlarmTestTemplateGuid"] = alarmStatus.AlarmTestTemplateGuid;
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

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmStatusTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = alarmStatusTable;
			tableValuedParameter.TypeName = "dbo.PointTemplateTagAlarmStatusDataType";
			cmd.Parameters.AddWithValue("@EnableAdd", enableAdd);
			cmd.Parameters.AddWithValue("@EnableModify", enableModify);
		}

		public static void DeleteListSQL(SqlCommand cmd, List<Guid> pointTemplateTagAlarmStatusGuidList)
		{
			cmd.CommandText = "DELETE ptas FROM dbo.tblPointTemplateTagAlarmStatus ptas"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = ptas.PointTemplateTagAlarmStatusGuid";
			GenerateGuidListTable(cmd, pointTemplateTagAlarmStatusGuidList);
		}

		public static void DeleteByAlarmTemplateGuidListSQL(SqlCommand cmd, List<Guid> alarmTemplateGuidList)
		{
			cmd.CommandText = "DELETE ptas FROM dbo.tblPointTemplateTagAlarmStatus ptas"
									+ " INNER JOIN tblAlarmTestTemplate at"
									+ " ON ptas.AlarmTestTemplateGuid = at.AlarmTestTemplateGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmTemplateGuid";
			GenerateGuidListTable(cmd, alarmTemplateGuidList);
		}

		public static void DeleteByPointTemplateGuidSQL(SqlCommand cmd, Guid pointTemplateGuid)
		{
			cmd.CommandText = "DELETE ptas FROM dbo.tblPointTemplateTagAlarmStatus ptas"
									+ " INNER JOIN tblAlarmTestTemplate at ON ptas.AlarmTestTemplateGuid = at.AlarmTestTemplateGuid"
									+ " INNER JOIN tblAlarmTemplate a ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
									+ " INNER JOIN tblPointTemplateTag t ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
									+ " WHERE t.PointTemplateGuid = @PointTemplateGuid";
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}
	}
}
