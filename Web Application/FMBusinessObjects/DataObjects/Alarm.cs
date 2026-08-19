
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
	public class Alarm : BaseDataObject, ICloneable
	{
		[EntityImportExportAttribute("ALARMID*", 200, "ID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get { return base.ID; } set { base.ID = value; } }


		[EntityImportExportAttribute("ALARMGUID", 200, "AlarmGuid")]
		[FMPersistedField]
		public Guid AlarmGuid {	get {	return base.IdentityGuid; } set { base.IdentityGuid = value; }	}

		[DataMember]
		[FMPersistedField("PointID", ReadOnly = true)]
		public string PointID { get; set; }

		[DataMember]
		[FMPersistedField("InputTagID", ReadOnly = true)]
		public string InputTagID { get; set; }

		[EntityImportExportAttribute("INPUTTAGGUID", 200, "InputTagGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid InputTagGuid { get; set; }

		[EntityImportExportWorksheet("ALARMTESTS", "ALARMTESTID*")]
		[DataMember]
		public Dictionary<Guid,AlarmTest> AlarmTests = new Dictionary<Guid, AlarmTest>();

		[EntityImportExportWorksheet("POINTTAGALARMSTATUSES", "POINTTAGALARMSTATUSGUID*")]
		[DataMember]
		public Dictionary<Guid, PointTagAlarmStatus> AlarmStatus = new Dictionary<Guid, PointTagAlarmStatus>();

		[DataMember]
		[FMPersistedField("PointGuid", ReadOnly = true)]
		public Guid PointGuid { get; set; }

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
		public bool Notify { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool ExclusiveAlarm { get; set; }

		[EntityImportExportAttribute("ALARMCATEGORYID", 100, "AlarmCategoryID")]
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string AlarmCategoryID { get; set; }

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

		[EntityImportExportAttribute("ALARMSTATETAGID", 200, "AlarmStateTagID")]
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string AlarmStateTagID { get; set; }

		[EntityImportExportAttribute("ALARMSTATETAGGUID", 200, "AlarmStateTagGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmStateTagGuid { get; set; }

		[EntityImportExportAttribute("ALARMTEMPLATEGUID", 200, "AlarmTemplateGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmTemplateGuid { get; set; }


		public Alarm()
		{
			this.AlarmGuid = Guid.NewGuid();
			this.Comment = string.Empty;
			this.ShelvedStartTimeStamp = null;
			this.ShelvedEndTimeStamp = null;
			this.Enabled = true;
			this.ShelvedOneShot = false;
			this.Suppressed = false;
			this.NotAlarmState = string.Empty;
			this.ExclusiveAlarm = true;
			this.AlarmTemplateGuid = Guid.Empty;
			this.Notify = false;
		}

		public Alarm(Type inputValueType, AlarmTemplate alarmTemplate, Dictionary<Guid,Guid> templateTagGuidToTagGuidMap )
		{
			this.ID = alarmTemplate.ID;
			this.AlarmGuid = Guid.NewGuid();
			this.Comment = alarmTemplate.Comment;
			this.ShelvedStartTimeStamp = alarmTemplate.ShelvedStartTimeStamp;
			this.ShelvedEndTimeStamp = alarmTemplate.ShelvedEndTimeStamp;
			this.Enabled = alarmTemplate.Enabled;
			this.ShelvedOneShot = alarmTemplate.ShelvedOneShot;
			this.Suppressed = alarmTemplate.Suppressed;
			this.Order = alarmTemplate.Order;
			this.NotAlarmState = alarmTemplate.NotAlarmState;
			this.AlarmCategoryApplicationStringGuid = alarmTemplate.AlarmCategoryApplicationStringGuid;
			this.InputTagGuid = templateTagGuidToTagGuidMap[alarmTemplate.InputTemplateTagGuid];
			this.AlarmStateTagGuid = templateTagGuidToTagGuidMap[alarmTemplate.AlarmStateTemplateTagGuid];
			this.ExclusiveAlarm = alarmTemplate.ExclusiveAlarm;
			this.AlarmTemplateGuid = (inputValueType == typeof(DeviceAlarmMapReference)) ? Guid.Empty : alarmTemplate.AlarmTemplateGuid;

			Dictionary<Guid,Guid> alarmTestTemplateGuidToAlarmTestGuidDictionary = new Dictionary<Guid, Guid>();
         foreach (var alarmTestTemplate in alarmTemplate.AlarmTestTemplates.Values)
			{
				var alarmTest = new AlarmTest(inputValueType, alarmTestTemplate, this.AlarmGuid,templateTagGuidToTagGuidMap);
				alarmTestTemplateGuidToAlarmTestGuidDictionary.Add(alarmTestTemplate.IdentityGuid,alarmTest.IdentityGuid);
            this.AlarmTests.Add(alarmTest.IdentityGuid, alarmTest);
			}
			foreach (var alarmStatusTemplate in alarmTemplate.AlarmStatusTemplates.Values)
			{
				var alarmStatus = new PointTagAlarmStatus(alarmStatusTemplate, alarmTestTemplateGuidToAlarmTestGuidDictionary);
				this.AlarmStatus.Add(alarmStatus.IdentityGuid, alarmStatus);
			}
			this.Notify = false;
		}

		public string GetActiveAlarmState(bool useAck = true,bool alwaysGetAlarmText = false)
		{
			// if the alarm is disabled, clear out the alarm state
			if (!this.Enabled)
			{
				return null;
			}
			if (this.ExclusiveAlarm)
			{
				if (!useAck)
				{
					var alarmTest = this.GetActiveAlarmTest(false, alwaysGetAlarmText);
					if (alarmTest == null)
					{
						return this.NotAlarmState;
					}
					return alarmTest.AlarmState;
				}
				else
				{
					var alarmTest = this.GetActiveAlarmTest(false);
					if (alarmTest == null)
					{
						alarmTest = this.GetActiveAlarmTest();
					}

					if (alarmTest == null)
					{
						return this.NotAlarmState;
					}
					return alarmTest.AlarmState;
				}
			}
			else
			{
				return this.GetNonExclusiveActiveAlarmState(useAck, alwaysGetAlarmText);
			}
		}

		protected string GetNonExclusiveActiveAlarmState(bool useAck = true, bool alwaysGetAlarmText = false)
		{
			var alarmStatusList = this.GetAllActiveAlarmStatus(useAck, alwaysGetAlarmText);
			var alrmState = this.NotAlarmState;
			bool firstTimeThrough = true;
			if (alarmStatusList.Count > 0)
			{
				foreach (var alarmStatus in alarmStatusList)
				{
					if (!firstTimeThrough)
					{
						alrmState += " : ";
					}
					else
					{
						firstTimeThrough = false;
					}
					alrmState += this.AlarmTests[alarmStatus.AlarmTestGuid].AlarmState;
				}
			}
			return alrmState;
		}

		public AlarmTest GetActiveAlarmTest(bool useAck = true, bool alwaysGetAlarmText = false)
		{
			var activeAlarmStatus = this.GetActiveAlarmStatus(useAck, alwaysGetAlarmText);
			if (activeAlarmStatus == null)
			{
				return null;
			}
			return this.AlarmTests[activeAlarmStatus.AlarmTestGuid];
		}

		public AlarmTest GetAlarmTestByAlarmState(string state)
		{
			if (this.AlarmTests.Any())
			{
				foreach (var alarmTest in this.AlarmTests.Values)
				{
					if (state == alarmTest.AlarmState)
					{
						return alarmTest;
					}
				}
			}
			return null;
		}

		public PointTagAlarmStatus GetAlarmStatusByAlarmTestGuid(Guid alarmTestGuid)
		{
			if (this.AlarmStatus.Any())
			{
				foreach (var alarmStatus in this.AlarmStatus.Values)
				{
					if (alarmStatus.AlarmTestGuid == alarmTestGuid)
					{
						return alarmStatus;
					}
				}
			}
			return null;
		}

		public PointTagAlarmStatus GetAlarmStatusByAlarmState(string state)
		{
			var alarmTest = this.GetAlarmTestByAlarmState(state);
			if (alarmTest == null)
			{
				return null;
			}
			return this.GetAlarmStatusByAlarmTestGuid(alarmTest.AlarmTestGuid);
		}

		public PointTagAlarmStatus GetActiveAlarmStatus(bool useAck = true, bool alwaysGetAlarmText = false)
		{
			PointTagAlarmStatus lowestAlarmStatus = null;
			AlarmTest lowestAlarmStatusAlarmTest = null;

			foreach (var alarmStatus in this.AlarmStatus.Values)
			{
				var alarmTest = this.AlarmTests[alarmStatus.AlarmTestGuid];
				bool ack = (useAck) ? alarmStatus.Acknowledged : true;

				if (alarmTest.Enabled && ((alarmStatus.AlarmTestFailed || !ack) || alwaysGetAlarmText))
				{
					if (lowestAlarmStatus == null)
					{
						lowestAlarmStatus = alarmStatus;
						lowestAlarmStatusAlarmTest = alarmTest;
					}
					else
					{
						if (alarmTest.Order < lowestAlarmStatusAlarmTest.Order)
						{
							lowestAlarmStatus = alarmStatus;
							lowestAlarmStatusAlarmTest = alarmTest;
						}
					}
				}
			}
			return lowestAlarmStatus;
		}

		public List<PointTagAlarmStatus> GetAllActiveAlarmStatus(bool useAck = true, bool alwaysGetAlarmText = false)
		{
			List<PointTagAlarmStatus> activeAlarmStatuses = new List<PointTagAlarmStatus>();

			foreach (var alarmStatus in this.AlarmStatus.Values)
			{
				var alarmTest = this.AlarmTests[alarmStatus.AlarmTestGuid];
				bool ack = (useAck) ? alarmStatus.Acknowledged : true;

				if (alarmTest.Enabled && ((alarmStatus.AlarmTestFailed || !ack) || alwaysGetAlarmText))
				{
					activeAlarmStatuses.Add(alarmStatus);
				}
			}
			return activeAlarmStatuses;
		}



		public bool IsActiveAlarm()
		{
			if (this.Enabled && this.Suppressed == false && this.ShelvedOneShot == false
			    && (this.ShelvedEndTimeStamp == null || this.ShelvedEndTimeStamp < DateTimeOffset.UtcNow))
			{
				foreach (var alarmStatus in this.AlarmStatus.Values)
				{
					if (this.AlarmTests[alarmStatus.AlarmTestGuid].Enabled && alarmStatus.IsActiveAlarm())
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void EnumerateByTagGuidListSQL(SqlCommand cmd, List<Guid> tagGuidList)
		{
			cmd.CommandText = "SELECT p.PointGuid AS PointGuid, p.ID AS PointID, t1.ID AS InputTagID, t2.ID AS AlarmStateTagID, [as].ID AS AlarmCategoryID, a.* from dbo.tblAlarm a"
									+ " LEFT JOIN tblPointTag t1 ON a.InputTagGuid = t1.PointTagGuid"
									+ " LEFT JOIN tblPointTag t2 ON a.AlarmStateTagGuid = t2.PointTagGuid"
									+ " LEFT JOIN tblApplicationString [as] ON a.AlarmCategoryApplicationStringGuid = [as].ApplicationStringGuid"
									+ " LEFT JOIN tblPoint p ON p.PointGuid = t1.PointGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.InputTagGuid"
			                  + " ORDER BY InputTagGuid";
         GenerateGuidListTable(cmd, tagGuidList);
		}

		public static void EnumerateByAlarmGuidListSQL(SqlCommand cmd, List<Guid> alarmGuidList)
		{
			cmd.CommandText = "SELECT p.PointGuid AS PointGuid, p.ID AS PointID, t1.ID AS InputTagID, t2.ID AS AlarmStateTagID, [as].ID AS AlarmCategoryID, a.* from dbo.tblAlarm a"
									+ " LEFT JOIN tblPointTag t1 ON a.InputTagGuid = t1.PointTagGuid"
									+ " LEFT JOIN tblPointTag t2 ON a.AlarmStateTagGuid = t2.PointTagGuid"
									+ " LEFT JOIN tblApplicationString [as] ON a.AlarmCategoryApplicationStringGuid = [as].ApplicationStringGuid"
									+ " LEFT JOIN tblPoint p ON p.PointGuid = t1.PointGuid"
								   + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AlarmGuid"
								   + " ORDER BY AlarmGuid";
			GenerateGuidListTable(cmd, alarmGuidList);
		}

      public static void EnumerateByPointListSQL(SqlCommand cmd, List<Guid> pointGuidList)
		{
			cmd.CommandText = "SELECT p.PointGuid AS PointGuid, p.ID AS PointID, t1.ID AS InputTagID, t2.ID AS AlarmStateTagID, [as].ID AS AlarmCategoryID,  a.* from dbo.tblAlarm a"
									+ " LEFT JOIN tblPointTag t1 ON a.InputTagGuid = t1.PointTagGuid"
									+ " LEFT JOIN tblPointTag t2 ON a.AlarmStateTagGuid = t2.PointTagGuid"
									+ " LEFT JOIN tblApplicationString [as] ON a.AlarmCategoryApplicationStringGuid = [as].ApplicationStringGuid"
									+ " LEFT JOIN tblPoint p ON p.PointGuid = t1.PointGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = p.PointGuid"
								 + " ORDER BY PointGuid";
			GenerateGuidListTable(cmd, pointGuidList);
		}

		public static void EnumerateActiveAlarmsBySiteGuidSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "SELECT p.PointGuid AS PointGuid, p.ID AS PointID, t1.ID AS InputTagID, t2.ID AS AlarmStateTagID, [as].ID AS AlarmCategoryID, a.* from dbo.tblAlarm a"
									+ " LEFT JOIN tblPointTag t1 ON a.InputTagGuid = t1.PointTagGuid"
									+ " LEFT JOIN tblPointTag t2 ON a.AlarmStateTagGuid = t2.PointTagGuid"
									+ " LEFT JOIN tblApplicationString [as] ON a.AlarmCategoryApplicationStringGuid = [as].ApplicationStringGuid"
									+ " LEFT JOIN tblPoint p ON p.PointGuid = t1.PointGuid"
							 + " INNER JOIN"
								 + " ("
								 + " SELECT Distinct(a1.AlarmGuid) from tblPoint p1"
								 + "    INNER JOIN tblPointTag t1 ON t1.PointGuid = p1.PointGuid AND p1.SiteGuid = @SiteGuid"
								 + "    INNER JOIN tblAlarm a1 ON a1.InputTagGuid = t1.PointTagGuid"
								 + "    INNER JOIN tblAlarmTest at1 ON a1.AlarmGuid = at1.AlarmGuid"
								 + "    INNER JOIN tblPointTagAlarmStatus ptas1"
								 + "    ON ptas1.AlarmTestGuid = at1.AlarmTestGuid AND(ptas1.Acknowledged = 0 OR(ptas1.Acknowledged = 1 AND ptas1.AlarmTestFailed = 1))"
								 + " ) v"
								 + " ON v.AlarmGuid = a.AlarmGuid"
								 + " ORDER BY AlarmGuid";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public static void DeleteListSQL(SqlCommand cmd, List<Guid> alarmGuidList)
		{
			cmd.CommandText = "DELETE a FROM dbo.tblAlarm a"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AlarmGuid";
			GenerateGuidListTable(cmd, alarmGuidList);
		}

		public static void DeleteByPointGuidListSQL(SqlCommand cmd, List<Guid> pointGuidList)
		{
			cmd.CommandText = "DELETE a FROM dbo.tblAlarm a"
									 + " INNER JOIN dbo.tblPointTag t"
									 + " ON a.InputTagGuid = t.PointTagGuid"
									 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointGuid";
			GenerateGuidListTable(cmd, pointGuidList);
		}

		protected static DataTable CreateAlarmListDataTable(List<Alarm> alarmList, SecurityClass security)
		{

			var table = new DataTable();
			table.Columns.Add("AlarmGuid", typeof(Guid));
			table.Columns.Add("InputTagGuid", typeof(Guid));
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
			table.Columns.Add("AlarmStateTagGuid", typeof(Guid));
			table.Columns.Add("ExclusiveAlarm", typeof(bool));
			table.Columns.Add("AlarmTemplateGuid", typeof(Guid));
			table.Columns.Add("Notify", typeof(bool));

			foreach (var alarm in alarmList)
			{
				var row = table.NewRow();
				row["AlarmGuid"] = alarm.AlarmGuid;
				row["InputTagGuid"] = alarm.InputTagGuid;
				row["ID"] = alarm.ID;
                row["Enabled"] = alarm.Enabled;
				row["Notify"] = alarm.Notify;
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
				row["AlarmStateTagGuid"] = alarm.AlarmStateTagGuid;
				row["ExclusiveAlarm"] = alarm.ExclusiveAlarm;
				if (alarm.AlarmTemplateGuid != Guid.Empty)
				{
					row["AlarmTemplateGuid"] = alarm.AlarmTemplateGuid;
				}
				table.Rows.Add(row);
			}

			return table;
		}

		public static void UpdateShelvedOneShot(SqlCommand cmd, List<Alarm> alarmList, SecurityClass security)
		{
			if (alarmList == null || alarmList.Count < 1)
			{
				return;
			}
			var table = CreateAlarmListDataTable(alarmList, security);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_AlarmUpdateShelvedOneShot";

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.AlarmDataType";
		}

		public static void UpdateShelved(SqlCommand cmd, List<Alarm> alarmList, SecurityClass security)
		{
			if (alarmList == null || alarmList.Count < 1)
			{
				return;
			}
			var table = CreateAlarmListDataTable(alarmList, security);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_AlarmUpdateShelved";

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.AlarmDataType";
		}

		public static void AddModifyStoredProcedure(SqlCommand cmd, List<Alarm> alarmList, SecurityClass security, bool enableAdd, bool enableModify)
		{
			if (alarmList == null || alarmList.Count < 1)
			{
				return;
			}
			var table = CreateAlarmListDataTable(alarmList, security);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_AlarmAddModify";

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.AlarmDataType";
			cmd.Parameters.AddWithValue("@EnableAdd", enableAdd);
			cmd.Parameters.AddWithValue("@EnableModify", enableModify);
		}

		public object Clone()
		{
			var a = (Alarm)this.MemberwiseClone();
			this.BaseClone(a);
			if (this.AlarmStatus != null)
			{
				a.AlarmStatus = new Dictionary<Guid, PointTagAlarmStatus>();
				foreach (var alarmStatus in this.AlarmStatus)
				{
					a.AlarmStatus.Add(alarmStatus.Key, (PointTagAlarmStatus)alarmStatus.Value.Clone());
				}
			}
			if (this.AlarmTests != null)
			{
				a.AlarmTests = new Dictionary<Guid, AlarmTest>();
				foreach (var alarmTest in this.AlarmTests)
				{
					a.AlarmTests.Add(alarmTest.Key, (AlarmTest)alarmTest.Value.Clone());
				}
			}
			return a;
		}

		public static void DeleteAlarmForTagNotInList(SqlCommand cmd, Guid inputTagGuid, List<Guid> alarms)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "[usp_AlarmDeleteAlarmsForTagNotInList]";

			var table = new DataTable();
			table.Columns.Add("Guid", typeof(Guid));

			foreach (var alarm in alarms)
			{
				var row = table.NewRow();
				row["Guid"] = alarm;
				table.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmList", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.GuidListType";
			cmd.Parameters.AddWithValue("@inputTagGuid", inputTagGuid);
		}

		public static void DeleteAlarmsFromTagsNotInList(SqlCommand cmd, Guid pointGuid, List<Guid> tagsWithAlarms)
		{

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_AlarmDeleteNotInTagList";

			var table = new DataTable();
			table.Columns.Add("Guid", typeof(Guid));

			foreach (var tagGuid in tagsWithAlarms)
			{
				var row = table.NewRow();
				row["Guid"] = tagGuid;
				table.Rows.Add(row);
			}

			cmd.Parameters.AddWithValue("@PointGuid", pointGuid);
			SqlParameter tableValuedParameter = cmd.Parameters.Add("@TagGuids", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}

	}
}
