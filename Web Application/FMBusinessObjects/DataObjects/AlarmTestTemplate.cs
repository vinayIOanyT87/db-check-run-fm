
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
	public class AlarmTestTemplate : BaseDataObject, ICloneable
	{
		[EntityImportExportAttribute("ALARMTESTTEMPLATESID*", 200, "ID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get { return base.ID; } set { if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9 ]*$")) { base.ID = value; } else throw new Exception("Alarm Test ID must be Alphanumeric"); } }

		[DataMember]
		[FMPersistedField("PointTemplateGuid")]
		public Guid PointTemplateGuid { get; set; }

		[DataMember]
		[FMPersistedField("PointTemplateTagGuid")]
		public Guid PointTemplateTagGuid { get; set; }

		[EntityImportExportAttribute("ALARMTESTTEMPLATEGUID", 200, "AlarmTestTemplateGuid")]
		[FMPersistedField]
		public Guid AlarmTestTemplateGuid
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

		[EntityImportExportAttribute("ALARMTEMPLATEGUID", 200, "AlarmTemplateGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmTemplateGuid { get; set; }

		[EntityImportExportAttribute("LIMITTEMPLATETAGGUID", 200, "LimitTemplateTagGUID")]
		[DataMember]
		[FMPersistedField]
		public Guid LimitTemplateTagGuid { get; set; }

		public enum TagFieldEnum { Value = 0, Status = 1, OpcStatusSubCode = 2 }

		[EntityImportExportAttribute("TAGFIELD", 100, "TagField")]
		[DataMember]
		[FMPersistedField]
		public TagFieldEnum TagField { get; set; }

		[EntityImportExportAttribute("ALARMPRIORITYGUID", 200, "AlarmPriorityGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid AlarmPriorityGuid { get; set; }

		[EntityImportExportAttribute("NORMALUNACKNOWLEDGEDPRIORITYGUID", 200, "NormalUnacknowledgedAlarmPriorityGUID")]
		[DataMember]
        [FMPersistedField]
        public Guid NormalUnacknowledgedAlarmPriorityGuid { get; set; }

        public enum TestTypeEnum { GreaterThan = 0, GreaterThanOrEqual = 1, LessThan = 2, LessThanOrEqual = 3, Equals = 4, NotEquals = 5 }

		[EntityImportExportAttribute("TESTTYPE", 100, "TestType")]
		[DataMember]
		[FMPersistedField]
		public TestTypeEnum TestType { get; set; }

		public enum BitwiseOperatorEnum { And = 0, Or = 1, Nor = 3, Xor = 4, Nand = 5, Nxor = 6 }

		[DataMember]
		[FMPersistedField]
		public BitwiseOperatorEnum BitwiseOperator { get; set; }

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


		public AlarmTestTemplate()
		{
			this.AlarmTestTemplateGuid = Guid.NewGuid();
			this.TagField = TagFieldEnum.Value;
			this.BitMask = -1;
			this.Enabled = true;
			this.Order = 1;
			this.AlarmState = "Alarm";
			this.Holdoff = 0.00;
			this.AlarmText = null;
			this.HelpFile = null;
			this.DrawingGuid = null;
			this.TimedHoldOffInSeconds = 0;
			this.BitwiseOperator = BitwiseOperatorEnum.And;
		}

		public object Clone()
		{
			var a = (AlarmTestTemplate)this.MemberwiseClone();
			this.BaseClone(a);
			return a;
		}

		public static void EnumerateByPointTemplateTagGuidListSQL(SqlCommand cmd, List<Guid> pointTemplateTagGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.* from tblAlarmTestTemplate at"
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
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.* from tblAlarmTestTemplate at"
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
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.* from tblAlarmTestTemplate at"
								 + " INNER JOIN tblAlarmTemplate a"
								 + " ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
								 + " INNER JOIN tblPointTemplateTag t"
								 + " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmTestTemplateGuid"
								 + " ORDER BY AlarmTestTemplateGuid";
			GenerateGuidListTable(cmd, alarmTestTemplateGuidList);
		}

		public static void EnumerateByPointTemplateListSQL(SqlCommand cmd, List<Guid> pointTemplateGuidList)
		{
			cmd.CommandText = "SELECT t.PointTemplateGuid AS PointTemplateGuid, t.PointTemplateTagGuid AS PointTemplateTagGuid, at.* from tblAlarmTestTemplate at"
								 + " INNER JOIN tblAlarmTemplate a"
								 + " ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
								 + " INNER JOIN tblPointTemplateTag t"
								 + " ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
								 + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = t.PointTemplateGuid"
								 + " ORDER BY PointTemplateGuid";

			GenerateGuidListTable(cmd, pointTemplateGuidList);
		}

		public static void DeleteListSQL(SqlCommand cmd, List<Guid> alarmTestTemplateGuidList)
		{
			cmd.CommandText = "DELETE at FROM dbo.tblAlarmTestTemplate at"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = at.AlarmTestTemplateGuid";
			GenerateGuidListTable(cmd, alarmTestTemplateGuidList);
		}

		public static void DeleteByAlarmTemplateGuidListSQL(SqlCommand cmd, List<Guid> alarmTemplateGuidList)
		{
			cmd.CommandText = "DELETE at FROM dbo.tblAlarmTestTemplate at"
									+ " INNER JOIN tblAlarmTemplate a"
									+ " ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
									+ " INNER JOIN @GuidTable gtbl ON gtbl.Guid = a.AlarmTemplateGuid";
			GenerateGuidListTable(cmd, alarmTemplateGuidList);
		}

		public static void DeleteByPointTemplateGuid(SqlCommand cmd, Guid pointTemplateGuid)
		{
			cmd.CommandText = "SET NOCOUNT ON"
									+ " DELETE pagtat FROM map.tblPointAccessGroupToAlarmTest pagtat"
									+ " INNER JOIN dbo.tblAlarmTestTemplate at ON at.AlarmTestTemplateGuid = pagtat.AlarmTestGuid"
									+ " INNER JOIN tblAlarmTemplate a ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
									+ " INNER JOIN tblPointTemplateTag t ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
									+ " WHERE t.PointTemplateGuid = @PointTemplateGuid"
									+ ""
									+ " DELETE at FROM dbo.tblAlarmTestTemplate at"
									+ " INNER JOIN tblAlarmTemplate a ON at.AlarmTemplateGuid = a.AlarmTemplateGuid"
									+ " INNER JOIN tblPointTemplateTag t ON a.InputTemplateTagGuid = t.PointTemplateTagGuid"
									+ " WHERE t.PointTemplateGuid = @PointTemplateGuid";

         cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}


		public static void DeleteAlarmTestTemplatesNotInList(SqlCommand cmd, Guid alarmTemplateGuid, List<Guid> alarmTestTemplateGuidList)
		{
			if (alarmTestTemplateGuidList == null || alarmTestTemplateGuidList.Count < 1)
			{
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_AlarmTestTemplateDeleteByNotInListAlarmTestTemplateGuid";

			var table = new DataTable();
			table.Columns.Add("Guid", typeof(Guid));

			foreach (var alarmTestTemplate in alarmTestTemplateGuidList)
			{
				var row = table.NewRow();
				row["Guid"] = alarmTestTemplate;
				table.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmTestList", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.GuidListType";
			cmd.Parameters.AddWithValue("@AlarmTemplateGuid", alarmTemplateGuid);
		}

		public static void AddModifyStoredProcedure(SqlCommand cmd, List<AlarmTestTemplate> alarmTestTemplateList, SecurityClass security, bool enableAdd, bool enableModify)
		{
			if (alarmTestTemplateList == null || alarmTestTemplateList.Count < 1)
			{
				return;
			}

			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_AlarmTestTemplateAddModify";

			var table = new DataTable();
			table.Columns.Add("AlarmTestTemplateGuid", typeof(Guid));
			table.Columns.Add("AlarmTemplateGuid", typeof(Guid));
			table.Columns.Add("ID", typeof(string));
			table.Columns.Add("LimitTemplateTagGuid", typeof(Guid));
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
			foreach (var alarmTestTemplate in alarmTestTemplateList)
			{
				var row = table.NewRow();
				row["AlarmTestTemplateGuid"] = alarmTestTemplate.AlarmTestTemplateGuid;
				row["AlarmTemplateGuid"] = alarmTestTemplate.AlarmTemplateGuid;
				row["ID"] = alarmTestTemplate.ID;
				row["LimitTemplateTagGuid"] = alarmTestTemplate.LimitTemplateTagGuid;
				row["TagField"] = alarmTestTemplate.TagField;
				row["AlarmPriorityGuid"] = alarmTestTemplate.AlarmPriorityGuid;
			    row["NormalUnacknowledgedAlarmPriorityGuid"] = alarmTestTemplate.NormalUnacknowledgedAlarmPriorityGuid;
				row["TestType"] = alarmTestTemplate.TestType;
				row["BitMask"] = alarmTestTemplate.BitMask;
				row["Enabled"] = alarmTestTemplate.Enabled;
				row["Order"] = alarmTestTemplate.Order;
				row["AlarmState"] = alarmTestTemplate.AlarmState;
				row["Holdoff"] = alarmTestTemplate.Holdoff;
				row["AlarmText"] = alarmTestTemplate.AlarmText;
				row["HelpFile"] = alarmTestTemplate.HelpFile;
				row["DrawingGuid"] = alarmTestTemplate.DrawingGuid ?? (object) DBNull.Value;
				row["UpdatedBy"] = security.UserID;
				row["BitwiseOperator"] = alarmTestTemplate.BitwiseOperator;
				row["TimedHoldOffInSeconds"] = alarmTestTemplate.TimedHoldOffInSeconds;
				table.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@AlarmTestTempTable", SqlDbType.Structured);
			tableValuedParameter.Value = table;
			tableValuedParameter.TypeName = "dbo.AlarmTestTemplateDataType";
			cmd.Parameters.AddWithValue("@EnableAdd", enableAdd);
			cmd.Parameters.AddWithValue("@EnableModify", enableModify);
		}

	}
}
