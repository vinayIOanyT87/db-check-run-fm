
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Data;
	using System.Linq;

	[DataContract]
	[Serializable]
	public class AlarmTemplate : BaseDataObject, ICloneable
	{
		[EntityImportExportAttribute("ALARMTEMPLATEID*", 200, "ID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get { return base.ID; } set { if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9 ]*$")) { base.ID = value; } else throw new Exception("Alarm ID must be Alphanumeric"); } }

		[EntityImportExportAttribute("ALARMTEMPLATEGUID", 200, "AlarmTemplateGuid")]
		[FMPersistedField]
		public Guid AlarmTemplateGuid
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

		[DataMember]
		[FMPersistedField("PointTemplateTagGuid")]
		public Guid PointTemplateTagGuid { get; set; }

		[DataMember]
		[FMPersistedField("PointTemplateID", ReadOnly = true)]
		public string PointTemplateID { get; private set; }

		[DataMember]
		[FMPersistedField("PointTemplateTagID", ReadOnly = true)]
		public string PointTemplateTagID { get; private set; }

		[EntityImportExportWorksheet("ALARMTESTTEMPLATES", "ALARMTESTTEMPLATESID*")]
		[DataMember]
		public Dictionary<Guid, AlarmTestTemplate> AlarmTestTemplates = new Dictionary<Guid, AlarmTestTemplate>();

      [EntityImportExportWorksheet("ALARMSTATUSTEMPLATES", "ALARMSTATUSTEMPLATEGUID*")]
      [DataMember]
		public Dictionary<Guid, PointTemplateTagAlarmStatus> AlarmStatusTemplates = new Dictionary<Guid, PointTemplateTagAlarmStatus>();

		[DataMember]
		[FMPersistedField("PointTemplateGuid", ReadOnly = true)]
		public Guid PointTemplateGuid { get; set; }

		[EntityImportExportAttribute("INPUTTEMPLATETAGGUID", 200, "InputTemplateTagGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid InputTemplateTagGuid { get; set; }

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

		[EntityImportExportAttribute("ENABLED", 100, "Enabled")]
		[DataMember]
		[FMPersistedField]
		public bool Enabled { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool ExclusiveAlarm { get; set; }

		[EntityImportExportAttribute("ALARMCATEGORYGUID", 100, "AlarmCategoryGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmCategoryApplicationStringGuid { get; set; }

		[EntityImportExportAttribute("ORDER", 100, "Order")]
		[DataMember]
		[FMPersistedField]
		public int Order { get; set; }

		[EntityImportExportAttribute("NOTALARMTEXT", 100, "NotAlarmState")]
		[DataMember]
		[FMPersistedField]
		public string NotAlarmState { get; set; }

		[EntityImportExportAttribute("COMMENT", 100, "Comment")]
		[DataMember]
		[FMPersistedField]
		public string Comment { get; set; }  //Need to initialize to empty string;

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset? ShelvedStartTimeStamp { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset? ShelvedEndTimeStamp { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool ShelvedOneShot { get; set; }

		[DataMember]
		[FMPersistedField]
		public string ShelvedBy { get; set; }


		[DataMember]
		[FMPersistedField]
		public bool Suppressed { get; set; }

		[EntityImportExportAttribute("ALARMSTATETEMPLATETAGGUID", 200, "AlarmStateTemplateTagGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmStateTemplateTagGuid { get; set; }

		public AlarmTemplate()
		{
			this.AlarmTemplateGuid = Guid.NewGuid();
			this.Comment = string.Empty;
			this.ShelvedStartTimeStamp = null;
			this.ShelvedEndTimeStamp = null;
			this.Enabled = true;
			this.ShelvedOneShot = false;
			this.Suppressed = false;
			this.NotAlarmState = string.Empty;
			this.ExclusiveAlarm = true;
		}

		public static void EnumerateByPointTemplateTagGuidListSQL(SqlCommand cmd, List<Guid> pointTemplateTagGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, p.ID AS PointTemplateID, t.ID AS PointTemplateTagID, t.PointTemplateTagGuid AS PointTemplateTagGuid,  a.* from tblAlarmTemplate a"
									+ " INNER JOIN tblPointTemplateTag t"
									+ " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
									+ " INNER JOIN tblPointTemplate p"
									+ " ON p.PointTemplateGuid = t.PointTemplateGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.InputTemplateTagGuid"
									+ " ORDER BY InputTemplateTagGuid";
			GenerateGuidListTable(cmd, pointTemplateTagGuidList);
		}

		public static void EnumerateByAlarmTemplateGuidListSQL(SqlCommand cmd, List<Guid> alarmTemplateGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, p.ID AS PointTemplateID, t.ID AS PointTemplateTagID, t.PointTemplateTagGuid AS PointTemplateTagGuid, a.* from tblAlarmTemplate a"
									+ " INNER JOIN tblPointTemplateTag t"
									+ " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
									+ " INNER JOIN tblPointTemplate p"
									+ " ON p.PointTemplateGuid = t.PointTemplateGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AlarmTemplateGuid"
								 + " ORDER BY AlarmTemplateGuid";
			GenerateGuidListTable(cmd, alarmTemplateGuidList);
		}

		public static void EnumerateByPointTemplateListSQL(SqlCommand cmd, List<Guid> pointTemplateGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, p.ID AS PointTemplateID, t.ID AS PointTemplateTagID, t.PointTemplateTagGuid AS PointTemplateTagGuid, a.* from tblAlarmTemplate a"
									+ " INNER JOIN tblPointTemplateTag t"
									+ " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
									+ " INNER JOIN tblPointTemplate p"
									+ " ON p.PointTemplateGuid = t.PointTemplateGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointTemplateGuid"
								 + " ORDER BY PointTemplateGuid";
			GenerateGuidListTable(cmd, pointTemplateGuidList);
		}

		public static void DeleteListSQL(SqlCommand cmd, List<Guid> alarmTemplateGuidList)
		{
			cmd.CommandText = "DELETE a FROM dbo.tblAlarmTemplate a"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AlarmTemplateGuid";
			GenerateGuidListTable(cmd, alarmTemplateGuidList);
		}

		public static void DeleteByPointTemplateGuidSQL(SqlCommand cmd, Guid pointTemplateGuid)
		{
			cmd.CommandText = "DELETE a FROM dbo.tblAlarmTemplate a"
									 + " INNER JOIN tblPointTemplateTag t ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
									 + " WHERE t.PointTemplateGuid = @PointTemplateGuid";

			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}


		public static void AddModifyStoredProcedure(SqlCommand cmd, List<AlarmTemplate> alarmTemplateList, SecurityClass security, bool enableAdd, bool enableModify)
		{

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_AlarmTemplateAddModify";

			var table = new DataTable();
			table.Columns.Add("AlarmTemplateGuid", typeof(Guid));
			table.Columns.Add("InputTemplateTagGuid", typeof(Guid));
			table.Columns.Add("ID", typeof(string));
			table.Columns.Add("Enabled", typeof(bool));
			table.Columns.Add("AlarmCategoryApplicationStringGuid", typeof(Guid));
			table.Columns.Add("Order", typeof(int));
			table.Columns.Add("NotAlarmState", typeof(string));
			table.Columns.Add("Comment", typeof(string));
			table.Columns.Add("ShelvedStartTimeStamp", typeof(DateTimeOffset));
			table.Columns.Add("ShelvedEndTimeStamp", typeof(DateTimeOffset));
			table.Columns.Add("ShelvedOneShot", typeof(bool));
			table.Columns.Add("ShelvedBy", typeof(string));
			table.Columns.Add("Suppressed", typeof(bool));
			table.Columns.Add("UpdatedBy", typeof(string));
			table.Columns.Add("AlarmStateTemplateTagGuid", typeof(Guid));
			table.Columns.Add("ExclusiveAlarm", typeof(bool));
			foreach (var alarm in alarmTemplateList)
			{
				var row = table.NewRow();
				row["AlarmTemplateGuid"] = alarm.AlarmTemplateGuid;
				row["InputTemplateTagGuid"] = alarm.InputTemplateTagGuid;
				row["ID"] = alarm.ID;
				row["Enabled"] = alarm.Enabled;
				row["AlarmCategoryApplicationStringGuid"] = alarm.AlarmCategoryApplicationStringGuid;
				row["Order"] = alarm.Order;
				row["NotAlarmState"] = alarm.NotAlarmState;
				row["Comment"] = alarm.Comment;
				row["ShelvedStartTimeStamp"] = alarm.ShelvedStartTimeStamp ?? (object)DBNull.Value;
				row["ShelvedEndTimeStamp"] = alarm.ShelvedEndTimeStamp ?? (object)DBNull.Value;
				row["ShelvedOneShot"] = alarm.ShelvedOneShot;
				row["ShelvedBy"] = alarm.ShelvedBy;
				row["Suppressed"] = alarm.Suppressed;
				row["UpdatedBy"] = security.UserID;
				row["AlarmStateTemplateTagGuid"] = alarm.AlarmStateTemplateTagGuid;
				row["ExclusiveAlarm"] = alarm.ExclusiveAlarm;
				table.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.AlarmTemplateDataType";
			cmd.Parameters.AddWithValue("@EnableAdd", enableAdd);
			cmd.Parameters.AddWithValue("@EnableModify", enableModify);
		}

		public object Clone()
		{
			var a = (AlarmTemplate)this.MemberwiseClone();
			this.BaseClone(a);
			if (this.AlarmStatusTemplates != null)
			{
				a.AlarmStatusTemplates = new Dictionary<Guid, PointTemplateTagAlarmStatus>();
				foreach (var alarmStatusTemplate in this.AlarmStatusTemplates)
				{
					a.AlarmStatusTemplates.Add(alarmStatusTemplate.Key, (PointTemplateTagAlarmStatus)alarmStatusTemplate.Value.Clone());
				}
			}
			if (this.AlarmTestTemplates != null)
			{
				a.AlarmTestTemplates = new Dictionary<Guid, AlarmTestTemplate>();
				foreach (var alarmTestTemplate in this.AlarmTestTemplates)
				{
					a.AlarmTestTemplates.Add(alarmTestTemplate.Key, (AlarmTestTemplate)alarmTestTemplate.Value.Clone());
				}
			}
			return a;
		}

		public static void DeleteAlarmTemplatesFromTagsNotInList(SqlCommand cmd, Guid pointTemplateGuid, List<Guid> tagsWithAlarms)
		{

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_AlarmTemplateDeleteNotInTagList";

			var table = new DataTable();
			table.Columns.Add("Guid", typeof(Guid));

			foreach (var tagGuid in tagsWithAlarms)
			{
				var row = table.NewRow();
				row["Guid"] = tagGuid;
				table.Rows.Add(row);
			}

			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
			SqlParameter tableValuedParameter = cmd.Parameters.Add("@TemplateTagGuids", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}

		public static void DeleteAlarmTemplatesForTagNotInList(SqlCommand cmd, Guid inputTagTemplateGuid, List<Guid> alarmTemplates)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_AlarmTemplateDeleteAlarmTemplatesForTagNotInList";

			var table = new DataTable();
			table.Columns.Add("Guid", typeof(Guid));

			foreach (var alarmTemplate in alarmTemplates)
			{
				var row = table.NewRow();
				row["Guid"] = alarmTemplate;
				table.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmList", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.GuidListType";
			cmd.Parameters.AddWithValue("@inputTagTemplateGuid", inputTagTemplateGuid);
		}
	}
}
