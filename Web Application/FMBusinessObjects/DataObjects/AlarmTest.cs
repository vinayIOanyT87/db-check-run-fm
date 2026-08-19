
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Data;

	[DataContract]
	[Serializable]
	public class AlarmTest : BaseDataObject, ICloneable
	{
		[EntityImportExportAttribute("ALARMTESTID*", 200, "ID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get { return base.ID; } set { base.ID = value; } }


		[EntityImportExportAttribute("ALARMTESTGUID", 200, "AlarmTestGuid")]
		[FMPersistedField]
		public Guid AlarmTestGuid
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
		[FMPersistedField()]
		public Guid AlarmTestTemplateGuid { get; set; }


		[DataMember]
		[FMPersistedField("PointGuid", ReadOnly = true)]
		public Guid PointGuid { get; private set; }


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

		[EntityImportExportAttribute("ALARMGUID", 200, "AlarmGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmGuid { get; set; }

		[EntityImportExportAttribute("LIMITTAGID", 200, "LimitTagID")]
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string LimitTagID { get; set; }

		[EntityImportExportAttribute("LIMITTAGGUID", 200, "LimitTagGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid LimitTagGuid { get; set; }

		[EntityImportExportAttribute("TAGFIELD", 100, "TagField")]
		[DataMember]
		[FMPersistedField]
		public AlarmTestTemplate.TagFieldEnum TagField { get; set; }

		[EntityImportExportAttribute("BITWISEOPERATOR", 100, "BitwiseOperator")]
		[DataMember]
		[FMPersistedField]
		public AlarmTestTemplate.BitwiseOperatorEnum BitwiseOperator { get; set; }

		[EntityImportExportAttribute("ALARMPRIORITYID", 100, "AlarmPriorityID")]
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string AlarmPriorityID { get; set; }

		[EntityImportExportAttribute("ALARMPRIORITYGUID", 200, "AlarmPriorityGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmPriorityGuid { get; set; }

		[EntityImportExportAttribute("NORMALUNACKNOWLEDGEDPRIORITYID", 200, "NormalUnacknowledgedAlarmPriorityID")]
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string NormalUnacknowledgedAlarmPriorityID { get; set; }

		[EntityImportExportAttribute("NORMALUNACKNOWLEDGEDPRIORITYGUID", 200, "NormalUnacknowledgedAlarmPriorityGuid")]
		[DataMember]
      [FMPersistedField]
      public Guid NormalUnacknowledgedAlarmPriorityGuid { get; set; }

		[EntityImportExportAttribute("TESTTYPE", 100, "TestType")]
		[DataMember]
		[FMPersistedField]
		public AlarmTestTemplate.TestTypeEnum TestType { get; set; }

		[EntityImportExportAttribute("BITMASK", 100, "BitMask")]
		[DataMember]
		[FMPersistedField]
		public long BitMask { get; set; }

		[EntityImportExportAttribute("ENABLED", 100, "Enabled")]
		[DataMember]
		[FMPersistedField]
		public bool Enabled { get; set; }

		[EntityImportExportAttribute("ORDER", 100, "Order")]
		[DataMember]
		[FMPersistedField]
		public int Order { get; set; }

		[EntityImportExportAttribute("ALARMSTATE", 100, "AlarmState")]
		[DataMember]
		[FMPersistedField]
		public string AlarmState { get; set; }

		[EntityImportExportAttribute("DEADBAND", 100, "Holdoff")]
		[DataMember]
		[FMPersistedField]
		public double Holdoff { get; set; }

		[EntityImportExportAttribute("HOLDOFF", 100, "TimedHoldOffInSeconds")]
		[DataMember]
		[FMPersistedField]
		public int TimedHoldOffInSeconds { get; set; }

		[EntityImportExportAttribute("ALARMTEXT", 100, "AlarmText")]
		[DataMember]
		[FMPersistedField]
		public string AlarmText { get; set; }

		[DataMember]
		[FMPersistedField]
		public string HelpFile { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid? DrawingGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public byte? AlarmPriority { get; set; }

		public AlarmTest()
		{
			this.AlarmTestGuid = Guid.NewGuid();
			this.TagField = AlarmTestTemplate.TagFieldEnum.Value;
			this.BitMask = -1;
			this.Enabled = true;
			this.Order = 1;
			this.AlarmState = "Alarm";
			this.Holdoff = 0.00;
			this.AlarmText = null;
			this.HelpFile = null;
			this.DrawingGuid = null;
			this.TimedHoldOffInSeconds = 0;
			this.BitwiseOperator = AlarmTestTemplate.BitwiseOperatorEnum.And;
		}

		public AlarmTest(Type inputValueType, AlarmTestTemplate alarmTestTemplate, Guid alarmGuid, Dictionary<Guid, Guid> templateTagGuidToTagGuidMap)
		{
			this.AlarmTestGuid = Guid.NewGuid();
			this.ID = alarmTestTemplate.ID;
			this.LimitTagGuid = templateTagGuidToTagGuidMap[alarmTestTemplate.LimitTemplateTagGuid];
			this.TagField = alarmTestTemplate.TagField;
			this.AlarmPriorityGuid = alarmTestTemplate.AlarmPriorityGuid;
		    this.NormalUnacknowledgedAlarmPriorityGuid = alarmTestTemplate.NormalUnacknowledgedAlarmPriorityGuid;
			this.TestType = alarmTestTemplate.TestType;
			this.BitMask = alarmTestTemplate.BitMask;
			this.Enabled = alarmTestTemplate.Enabled;
			this.Order = alarmTestTemplate.Order;
			this.AlarmState = alarmTestTemplate.AlarmState;
			this.Holdoff = alarmTestTemplate.Holdoff;
			this.AlarmText = alarmTestTemplate.AlarmText;
			this.HelpFile = alarmTestTemplate.HelpFile;
			this.DrawingGuid = alarmTestTemplate.DrawingGuid;
			this.AlarmGuid = alarmGuid;
			this.TimedHoldOffInSeconds = alarmTestTemplate.TimedHoldOffInSeconds;
			this.BitwiseOperator = alarmTestTemplate.BitwiseOperator;
			this.AlarmTestTemplateGuid = (inputValueType == typeof(DeviceAlarmMapReference)) ? Guid.Empty : alarmTestTemplate.AlarmTestTemplateGuid;
		}

		public object Clone()
		{
			var a = (AlarmTest)this.MemberwiseClone();
			this.BaseClone(a);
			return a;
		}

		public static void EnumerateByTagGuidListSQL(SqlCommand cmd, List<Guid> tagGuidList)
		{
			cmd.CommandText = "SELECT t.PointGuid AS PointGuid, t.ID AS LimitTagID,  ap1.ID AS AlarmPriorityID, ap1.Priority as AlarmPriority, ap2.ID as NormalUnacknowledgedAlarmPriorityID, at.* from dbo.tblAlarmTest at"
								 + " LEFT JOIN dbo.tblPointTag t ON at.LimitTagGuid = t.PointTagGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap1 ON ap1.AlarmPriorityGuid = at.AlarmPriorityGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap2 ON ap2.AlarmPriorityGuid = at.NormalUnacknowledgedAlarmPriorityGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.InputTagGuid"
								 + " ORDER BY gtbl.Guid";
			GenerateGuidListTable(cmd, tagGuidList);
		}

		public static void EnumerateByLimitTagGuidListSQL(SqlCommand cmd, List<Guid> tagGuidList)
		{
			cmd.CommandText = "SELECT t.PointGuid AS PointGuid,  t.ID AS LimitTagID, ap1.ID AS AlarmPriorityID, ap1.Priority as AlarmPriority, ap2.ID as NormalUnacknowledgedAlarmPriorityID, at.* from dbo.tblAlarmTest at"
								 + " LEFT JOIN dbo.tblPointTag t ON at.LimitTagGuid = t.PointTagGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap1 ON ap1.AlarmPriorityGuid = at.AlarmPriorityGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap2 ON ap2.AlarmPriorityGuid = at.NormalUnacknowledgedAlarmPriorityGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.LimitTagGuid"
								 + " ORDER BY gtbl.Guid";
			GenerateGuidListTable(cmd, tagGuidList);
		}



		public static void EnumerateByAlarmGuidListSQL(SqlCommand cmd, List<Guid> alarmGuidList)
		{
			cmd.CommandText = "SELECT t.PointGuid AS PointGuid,  t.ID AS LimitTagID, ap1.ID AS AlarmPriorityID, ap1.Priority as AlarmPriority, ap2.ID as NormalUnacknowledgedAlarmPriorityID, at.* from dbo.tblAlarmTest at"
								 + " LEFT JOIN dbo.tblPointTag t ON at.LimitTagGuid = t.PointTagGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap1 ON ap1.AlarmPriorityGuid = at.AlarmPriorityGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap2 ON ap2.AlarmPriorityGuid = at.NormalUnacknowledgedAlarmPriorityGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmGuid"
								 + " ORDER BY AlarmGuid";
			GenerateGuidListTable(cmd, alarmGuidList);
		}

		public static void EnumerateByAlarmTestGuidListSQL(SqlCommand cmd, List<Guid> alarmTestGuidList)
		{
			cmd.CommandText = "SELECT t.PointGuid AS PointGuid,  t.ID AS LimitTagID, ap1.ID AS AlarmPriorityID, ap1.Priority as AlarmPriority, ap2.ID as NormalUnacknowledgedAlarmPriorityID, at.* from dbo.tblAlarmTest at"
								 + " LEFT JOIN dbo.tblPointTag t ON at.LimitTagGuid = t.PointTagGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap1 ON ap1.AlarmPriorityGuid = at.AlarmPriorityGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap2 ON ap2.AlarmPriorityGuid = at.NormalUnacknowledgedAlarmPriorityGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmTestGuid"
								 + " ORDER BY AlarmTestGuid";
			GenerateGuidListTable(cmd, alarmTestGuidList);
		}

		public static void EnumerateByPointListSQL(SqlCommand cmd, List<Guid> pointGuidList)
		{
			cmd.CommandText = "SELECT t.PointGuid AS PointGuid,  t.ID AS LimitTagID, ap1.ID AS AlarmPriorityID, ap1.Priority as AlarmPriority, ap2.ID as NormalUnacknowledgedAlarmPriorityID, at.* from dbo.tblAlarmTest at"
								 + " LEFT JOIN dbo.tblPointTag t ON at.LimitTagGuid = t.PointTagGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap1 ON ap1.AlarmPriorityGuid = at.AlarmPriorityGuid"
								 + " LEFT JOIN dbo.tblAlarmPriorities ap2 ON ap2.AlarmPriorityGuid = at.NormalUnacknowledgedAlarmPriorityGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointGuid"
								 + " ORDER BY PointGuid";

			GenerateGuidListTable(cmd, pointGuidList);
		}

		public static void DeleteListSQL(SqlCommand cmd, List<Guid> alarmTestGuidList)
		{
			cmd.CommandText = "DELETE at FROM dbo.tblAlarmTest at"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmTestGuid";
			GenerateGuidListTable(cmd, alarmTestGuidList);
		}

		public static void DeleteByAlarmGuidListSQL(SqlCommand cmd, List<Guid> alarmGuidList)
		{
			cmd.CommandText = "DELETE at FROM dbo.tblAlarmTest at"
									+ " INNER JOIN dbo.tblAlarm a ON at.AlarmGuid = a.AlarmGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AlarmGuid";
			GenerateGuidListTable(cmd, alarmGuidList);
		}

		public static void DeleteByPointGuidList(SqlCommand cmd, List<Guid> pointGuidList)
		{
			cmd.CommandText = "SET NOCOUNT ON"
									+ " DELETE pagtpat FROM map.tblPointAccessGroupToPointAlarmTest pagtpat"
									+ " INNER JOIN dbo.tblAlarmTest at ON at.AlarmTestGuid = pagtpat.AlarmTestGuid"
									+ " INNER JOIN dbo.tblAlarm a ON at.AlarmGuid = a.AlarmGuid"
									+ " INNER JOIN dbo.tblPointTag t ON a.InputTagGuid = t.PointTagGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointGuid"
									+ " "
									+ " DELETE at FROM dbo.tblAlarmTest at"
									+ " INNER JOIN dbo.tblAlarm a ON at.AlarmGuid = a.AlarmGuid"
									+ " INNER JOIN dbo.tblPointTag t ON a.InputTagGuid = t.PointTagGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointGuid";
			GenerateGuidListTable(cmd, pointGuidList);
		}

		public static void AddModifyStoredProcedure(SqlCommand cmd, List<AlarmTest> alarmTestList, SecurityClass security, bool enableAdd, bool enableModify)
		{
			if (alarmTestList == null || alarmTestList.Count < 1)
			{
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_AlarmTestAddModify";

			var table = new DataTable();
			table.Columns.Add("AlarmTestGuid", typeof(Guid));
			table.Columns.Add("AlarmGuid", typeof(Guid));
			table.Columns.Add("ID", typeof(string));
			table.Columns.Add("LimitTagGuid", typeof(Guid));
			table.Columns.Add("TagField", typeof(int));
			table.Columns.Add("AlarmPriorityGuid", typeof(Guid));
		    table.Columns.Add("NormalUnacknowledgedAlarmPriorityGuid", typeof(Guid));
			table.Columns.Add("TestType", typeof(int));
			table.Columns.Add("BitMask", typeof(long));
			table.Columns.Add("Enabled", typeof(bool));
			table.Columns.Add("Order", typeof(int));
			table.Columns.Add("AlarmState", typeof(string));
			table.Columns.Add("Holdoff", typeof(double));
			table.Columns.Add("AlarmText", typeof(string));
			table.Columns.Add("HelpFile", typeof(string));
			table.Columns.Add("DrawingGuid", typeof(Guid));
			table.Columns.Add("UpdatedBy", typeof(string));
			table.Columns.Add("BitwiseOperator", typeof(int));
			table.Columns.Add("TimedHoldOffInSeconds", typeof(int));
			table.Columns.Add("AlarmTestTemplateGuid", typeof(Guid));
			foreach (var alarmTest in alarmTestList)
			{
				var row = table.NewRow();
				row["AlarmTestGuid"] = alarmTest.AlarmTestGuid;
				row["AlarmGuid"] = alarmTest.AlarmGuid;
				row["ID"] = alarmTest.ID;
				row["LimitTagGuid"] = alarmTest.LimitTagGuid;
				row["TagField"] = alarmTest.TagField;
				row["AlarmPriorityGuid"] = alarmTest.AlarmPriorityGuid;
			    row["NormalUnacknowledgedAlarmPriorityGuid"] = alarmTest.NormalUnacknowledgedAlarmPriorityGuid;
				row["TestType"] = alarmTest.TestType;
				row["BitMask"] = alarmTest.BitMask;
				row["Enabled"] = alarmTest.Enabled;
				row["Order"] = alarmTest.Order; 
				row["AlarmState"] = alarmTest.AlarmState;
				row["Holdoff"] = alarmTest.Holdoff;
				row["AlarmText"] = alarmTest.AlarmText;
				row["HelpFile"] = alarmTest.HelpFile;
				row["DrawingGuid"] = alarmTest.DrawingGuid ?? (object)DBNull.Value;
				row["UpdatedBy"] = security.UserID;
				row["BitwiseOperator"] = alarmTest.BitwiseOperator;
				row["TimedHoldOffInSeconds"] = alarmTest.TimedHoldOffInSeconds;
				if (alarmTest.AlarmTestTemplateGuid != Guid.Empty)
				{
					row["AlarmTestTemplateGuid"] = alarmTest.AlarmTestTemplateGuid;
				}
				table.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmTestTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.AlarmTestDataType";
			cmd.Parameters.AddWithValue("@EnableAdd", enableAdd);
			cmd.Parameters.AddWithValue("@EnableModify", enableModify);
		}

		public static void EnumerateRestrictedAccessByAlarmTestGuidList(SqlCommand cmd, SecurityClass security, List<Guid> alarmTestGuidList)
		{
			cmd.CommandText = "[dbo].[usp_EnumerateRestrictedAccessByAlarmTestGuids]";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", security.UserGuid);

			var alarmTestGuidTable = new DataTable();
			alarmTestGuidTable.Columns.Add("Guid", typeof(Guid));

			foreach (var alarmTestGuid in alarmTestGuidList)
			{
				var row = alarmTestGuidTable.NewRow();
				row[0] = alarmTestGuid;
				alarmTestGuidTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmTestGuids", SqlDbType.Structured);
			tableValuedParameter.Value = alarmTestGuidTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}
	}
}
