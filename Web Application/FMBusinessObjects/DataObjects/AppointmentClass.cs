using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	public class AppointmentCollectionClass : List<AppointmentClass> { }

	[DataContract]
	[Serializable]
	public class AppointmentClass : BaseDataObject
	{
		[DataMember]
		public bool AuditLog = false;

		[DataMember]
		public string _Description;

		[DataMember]
		public string _AssociatedType;

		[DataMember]
		public Guid _AssociatedTypeGuid;

		[DataMember]
		public string _AppointmentCategory;

		[DataMember]
		public int _Duration;

		[DataMember]
		public string _AppointmentPeriodText;

		[DataMember]
		public int _AppointmentPeriod;

		[DataMember]
		public bool _AppointmentIsSingle;

		[DataMember]
		public bool _ScheduleOnWeekends;

		[DataMember]
		public bool _ScheduleOnHolidays;

		[DataMember]
		public DateAndTime _StartDate = new DateAndTime();

		[DataMember]
		public int _AppointmentTimeInterval;

		[DataMember]
		public string _AppointmentDayOfTheWeekText;

		[DataMember]
		public int _AppointmentDayOfTheWeek;

		[DataMember]
		public int _AppointmentReoccuranceInterval;

		[DataMember]
		public bool _AppointmentOption2Selected;

		[DataMember]
		public string _AppointmentTimeOptionSelectionText;

		[DataMember]
		public int _AppointmentTimeOptionSelection;

		[DataMember]
		public string _AppointmentMonthSelectionText;

		[DataMember]
		public int _AppointmentMonthSelection;

		[DataMember]
		public int _AppointmentDayOfTheMonth;

		[DataMember]
		public DateTimeOffset DueDate;

		[DataMember]
		public string _AssetText;

		[DataMember]
		public Guid _TestSetDefinitionGuid;

		[DataMember]
		public FUELING_TYPES EquipmentFuelingType { get; set; }

		[QueryWriterField("Description")]
		public string Description { get { return _Description; } set { SetString("Description", 50, value, ref _Description); } }

		[QueryWriterField("Associated Type")]
		public string AssociatedType { get { return _AssociatedType; } set { SetString("AssociatedType", 50, value, ref _AssociatedType); } }

		[QueryWriterField("Associated Type Guid")]
		public Guid AssociatedTypeGuid { get { return _AssociatedTypeGuid; } set { _AssociatedTypeGuid = value; } }

		[QueryWriterField("Appointment Category")]
		public string AppointmentCategory { get { return _AppointmentCategory; } set { SetString("AppointmentCategory", 50, value, ref _AppointmentCategory); } }

		[QueryWriterField("Duration")]
		public int Duration { get { return _Duration; } set { _Duration = value; } }

		[QueryWriterField("Appointment Period")]
		public string AppointmentPeriodText { get { return _AppointmentPeriodText; } set { SetString("AppointmentPeriodText", 50, value, ref _AppointmentPeriodText); } }

		[QueryWriterField("Appointment Is Single")]
		public bool AppointmentIsSingle { get { return _AppointmentIsSingle; } set { _AppointmentIsSingle = value; } }

		[QueryWriterField("Schedule On Weekends")]
		public bool ScheduleOnWeekends { get { return _ScheduleOnWeekends; } set { _ScheduleOnWeekends = value; } }

		[QueryWriterField("Schedule On Holidays")]
		public bool ScheduleOnHolidays { get { return _ScheduleOnHolidays; } set { _ScheduleOnHolidays = value; } }

		[QueryWriterField("Start Date", "StartDate")]
		public DateAndTime StartDateObject { get { return _StartDate; } }

		public string StartDate { get { return _StartDate.ToString(); } set { SetDateAndTime("Start Date", value, ref _StartDate); } }

		[QueryWriterField("Time Interval", "AppointmentTimeInterval")]
		public int AppointmentTimeInterval { get { return _AppointmentTimeInterval; } set { _AppointmentTimeInterval = value; } }

		[QueryWriterField("Day of the Week", "AppointmentDayOfTheWeekText")]
		public string AppointmentDayOfTheWeekText { get { return _AppointmentDayOfTheWeekText; } set { SetString("AppointmentDayOfTheWeekText", 20, value, ref _AppointmentDayOfTheWeekText); } }

		public int AppointmentDayOfTheWeek { get { return _AppointmentDayOfTheWeek; } set { _AppointmentDayOfTheWeek = value; } }

		[QueryWriterField("Recurrance Interval", "AppointmentReoccuranceInterval")]
		public int AppointmentReoccuranceInterval { get { return _AppointmentReoccuranceInterval; } set { _AppointmentReoccuranceInterval = value; } }

		[QueryWriterField("Option 2 Selected", "AppointmentOption2Selected")]
		public bool AppointmentOption2Selected { get { return _AppointmentOption2Selected; } set { _AppointmentOption2Selected = value; } }

		[QueryWriterField("Time Option Selection", "AppointmentTimeOptionSelectionText")]
		public string AppointmentTimeOptionSelectionText { get { return _AppointmentTimeOptionSelectionText; } set { SetString("AppointmentTimeOptionSelectionText", 20, value, ref _AppointmentTimeOptionSelectionText); } }

		public int AppointmentTimeOptionSelection { get { return _AppointmentTimeOptionSelection; } set { _AppointmentTimeOptionSelection = value; } }

		[QueryWriterField("Month Selection", "AppointmentMonthSelection")]
		public string AppointmentMonthSelectionText { get { return _AppointmentMonthSelectionText; } set { SetString("AppointmentMonthSelectionText", 20, value, ref _AppointmentMonthSelectionText); } }

		public int AppointmentMonthSelection { get { return _AppointmentMonthSelection; } set { _AppointmentMonthSelection = value; } }

		[QueryWriterField("Day of the Month", "AppointmentDayOfTheMonth")]
		public int AppointmentDayOfTheMonth { get { return _AppointmentDayOfTheMonth; } set { _AppointmentDayOfTheMonth = value; } }

		public int AppointmentPeriod { get { return _AppointmentPeriod; } set { _AppointmentPeriod = value; } }

		public string AssetText { get { return _AssetText; } set { SetString("AssetText", 100, value, ref _AssetText); } }

		public Guid TestSetDefinitionGuid { get { return _TestSetDefinitionGuid; } set { _TestSetDefinitionGuid = value; } }

		public AppointmentClass()
		{
			Initialize();
		}

		private void Initialize()
		{
			_Description = "";
			_AssociatedType = "";
			_AssociatedTypeGuid = Guid.Empty;
			_AppointmentCategory = "";
			_Duration = 0;
			_AppointmentPeriodText = "";
			_AppointmentPeriod = 0;
			_AppointmentIsSingle = true;
			_ScheduleOnWeekends = false;
			_ScheduleOnHolidays = false;
			_StartDate.Value = DateTimeOffset.Now;
			_AppointmentTimeInterval = 1;
			_AppointmentDayOfTheWeekText = "";
			_AppointmentDayOfTheWeek = 1;
			_AppointmentReoccuranceInterval = 1;
			_AppointmentOption2Selected = false;
			_AppointmentTimeOptionSelectionText = "";
			_AppointmentTimeOptionSelection = 1;
			_AppointmentMonthSelectionText = "";
			_AppointmentMonthSelection = 1;
			_AppointmentDayOfTheMonth = 1;
			_TestSetDefinitionGuid = Guid.Empty;
			DueDate = DateTimeOffset.Now;
			AssetText = "";
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
		}

		public override ENTITY_TYPE EntityType
		{
			get
			{
				switch (AssociatedType)
				{
					case "Tanks":
						return ENTITY_TYPE.APPOINTMENT_TANK;
					case "Personnel":
						return ENTITY_TYPE.APPOINTMENT_PERSONNEL;
					case "Equipment":
						return ENTITY_TYPE.APPOINTMENT_EQUIPMENT;
					default:
						return ENTITY_TYPE.UNKNOWN;
				}
			}
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		public static string GetAssociatedGuidColumnName(string associatedType)
		{
			switch (associatedType)
			{
				case "Tanks":
					return "TankGuid";
				case "Personnel":
					return "PersonnelGuid";
				case "Equipment":
					return "EquipmentGuid";
				default:
					return "Unknown";
			}
		}

		public static string GetPrimaryKeyColumnName(string associatedType)
		{
			switch (associatedType)
			{
				case "Tanks":
					return "AppointmentTankGuid";
				case "Personnel":
					return "AppointmentPersonnelGuid";
				case "Equipment":
					return "AppointmentEquipmentGuid";
				default:
					return "Unknown";
			}
		}

		public static string GetTableName(string associatedType)
		{
			switch (associatedType)
			{
				case "Tanks":
					return "tblAppointmentTank";
				case "Personnel":
					return "tblAppointmentPersonnel";
				case "Equipment":
					return "tblAppointmentEquipment";
				default:
					return "Unknown";
			}
		}

		private string SelectClause(string associatedType)
		{
			return " SELECT " + GetPrimaryKeyColumnName(associatedType) + " AS IdentityGuid, " +
				GetTableName(associatedType) + "." + GetAssociatedGuidColumnName(associatedType) + " AS AssociatedTypeGuid " +
				",TestSetDefinitionGuid" +
				"," + GetTableName(associatedType) + ".SiteGuid" +
				",AssetText" +
				",AppointmentCategory" +
				",AppointmentIsSingle" +
				",ScheduleOnWeekends" +
				",ScheduleOnHolidays" +
				",StartDate" +
				",Duration" +
				",AppointmentPeriod" +
				",AppointmentPeriodText" +
				"," + GetTableName(associatedType) + ".Description" +
				",AppointmentTimeInterval" +
				",AppointmentDayOfTheWeekText" +
				",AppointmentDayOfTheWeek" +
				",AppointmentReoccuranceInterval" +
				",AppointmentOption2Selected" +
				",AppointmentTimeOptionSelectionText" +
				",AppointmentTimeOptionSelection" +
				",AppointmentMonthSelectionText" +
				",AppointmentMonthSelection" +
				",AppointmentDayOfTheMonth" +
				"," + GetTableName(associatedType) + ".CreatedDate" +
				"," + GetTableName(associatedType) + ".CreatedBy" +
				"," + GetTableName(associatedType) + ".UpdatedDate" +
				"," + GetTableName(associatedType) + ".UpdatedBy" +
				",'" + associatedType + "' AS AssociatedType";
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException("Set is Null");
			}

			string associatedType = AssociatedType;

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

			AssociatedType = DataObject.getValue<string>(Row["AssociatedType"], "Unknown");
			IdentityGuid = DataObject.getValue<Guid>(Row["IdentityGuid"], Guid.Empty);
			AssociatedTypeGuid = DataObject.getValue<Guid>(Row["AssociatedTypeGuid"], Guid.Empty);

			SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			Description = DataObject.getValue<string>(Row["Description"], "");
			CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], CreatedDate);
			UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);


			AssetText = DataObject.getValue<string>(Row["AssetText"], "");
			AppointmentCategory = DataObject.getValue<string>(Row["AppointmentCategory"], "");
			AppointmentIsSingle = DataObject.getValue<bool>(Row["AppointmentIsSingle"], true);

			ScheduleOnWeekends = DataObject.getValue<bool>(Row["ScheduleOnWeekends"], false);
			ScheduleOnHolidays = DataObject.getValue<bool>(Row["ScheduleOnHolidays"], false);
			_StartDate.Value = DataObject.getValue<DateTimeOffset>(Row["StartDate"], DateTimeOffset.Now);
			Duration = DataObject.getValue<int>(Row["Duration"], 0);
			AppointmentPeriod = DataObject.getValue<int>(Row["AppointmentPeriod"], 0);
			AppointmentPeriodText = DataObject.getValue<string>(Row["AppointmentPeriodText"], "");
			AppointmentTimeInterval = DataObject.getValue<int>(Row["AppointmentTimeInterval"], 1);
			AppointmentDayOfTheWeekText = DataObject.getValue<string>(Row["AppointmentDayOfTheWeekText"], "");
			AppointmentDayOfTheWeek = DataObject.getValue<int>(Row["AppointmentDayOfTheWeek"], 1);
			AppointmentReoccuranceInterval = DataObject.getValue<int>(Row["AppointmentReoccuranceInterval"], 1);
			AppointmentOption2Selected = DataObject.getValue<bool>(Row["AppointmentOption2Selected"], false);
			AppointmentTimeOptionSelectionText = DataObject.getValue<string>(Row["AppointmentTimeOptionSelectionText"], "");
			AppointmentTimeOptionSelection = DataObject.getValue<int>(Row["AppointmentTimeOptionSelection"], 1);
			AppointmentMonthSelectionText = DataObject.getValue<string>(Row["AppointmentMonthSelectionText"], "");
			AppointmentMonthSelection = DataObject.getValue<int>(Row["AppointmentMonthSelection"], 1);
			AppointmentDayOfTheMonth = DataObject.getValue<int>(Row["AppointmentDayOfTheMonth"], 1);
			TestSetDefinitionGuid = DataObject.getValue<Guid>(Row["TestSetDefinitionGuid"], Guid.Empty);

			if (Table.Columns.Contains("FuelingType"))
			{
				EquipmentFuelingType = (FUELING_TYPES)DataObject.getValue<short>(Row["FuelingType"], (short)FUELING_TYPES.NONE);
			}
			else
			{
				EquipmentFuelingType = FUELING_TYPES.NONE;
			}

			DueDate = DateTimeOffset.Now;
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO " + GetTableName(AssociatedType) + " (" +
				"SiteGuid," +
				GetAssociatedGuidColumnName(AssociatedType) + "," +
				"AssetText," +
				"AppointmentCategory," +
				"AppointmentIsSingle," +
				"ScheduleOnWeekends," +
				"ScheduleOnHolidays," +
				"StartDate," +
				"Duration," +
				"AppointmentPeriod," +
				"AppointmentPeriodText," +
				"Description," +
				"AppointmentTimeInterval," +
				"AppointmentDayOfTheWeekText," +
				"AppointmentDayOfTheWeek," +
				"AppointmentReoccuranceInterval," +
				"AppointmentOption2Selected," +
				"AppointmentTimeOptionSelectionText," +
				"AppointmentTimeOptionSelection," +
				"AppointmentMonthSelectionText," +
				"AppointmentMonthSelection," +
				"AppointmentDayOfTheMonth," +
				"TestSetDefinitionGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				GetPrimaryKeyColumnName(AssociatedType) +
				") VALUES (" +
				"@SiteGuid," +
				"@AssociatedTypeGuid," +
				"@AssetText," +
				"@AppointmentCategory," +
				"@AppointmentIsSingle," +
				"@ScheduleOnWeekends," +
				"@ScheduleOnHolidays," +
				"@StartDate," +
				"@Duration," +
				"@AppointmentPeriod," +
				"@AppointmentPeriodText," +
				"@Description," +
				"@AppointmentTimeInterval," +
				"@AppointmentDayOfTheWeekText," +
				"@AppointmentDayOfTheWeek," +
				"@AppointmentReoccuranceInterval," +
				"@AppointmentOption2Selected," +
				"@AppointmentTimeOptionSelectionText," +
				"@AppointmentTimeOptionSelection," +
				"@AppointmentMonthSelectionText," +
				"@AppointmentMonthSelection," +
				"@AppointmentDayOfTheMonth," +
				"@TestSetDefinitionGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@" + GetPrimaryKeyColumnName(AssociatedType) +
				")";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AssociatedTypeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AssetText", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@AppointmentCategory", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@AppointmentIsSingle", SqlDbType.Bit);
			cmd.Parameters.Add("@ScheduleOnWeekends", SqlDbType.Bit);
			cmd.Parameters.Add("@ScheduleOnHolidays", SqlDbType.Bit);
			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Duration", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentPeriod", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentPeriodText", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@AppointmentTimeInterval", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentDayOfTheWeekText", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@AppointmentDayOfTheWeek", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentReoccuranceInterval", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentOption2Selected", SqlDbType.Bit);
			cmd.Parameters.Add("@AppointmentTimeOptionSelectionText", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@AppointmentTimeOptionSelection", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentMonthSelectionText", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@AppointmentMonthSelection", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentDayOfTheMonth", SqlDbType.Int);
			cmd.Parameters.Add("@TestSetDefinitionGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@" + GetPrimaryKeyColumnName(AssociatedType), SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@AssociatedTypeGuid"].Value = AssociatedTypeGuid;
			cmd.Parameters["@AssetText"].Value = AssetText;
			cmd.Parameters["@AppointmentCategory"].Value = AppointmentCategory;

			if (AppointmentIsSingle)
			{
				cmd.Parameters["@AppointmentIsSingle"].Value = 1;
			}
			else
			{
				cmd.Parameters["@AppointmentIsSingle"].Value = 0;
			}

			if (ScheduleOnWeekends)
			{
				cmd.Parameters["@ScheduleOnWeekends"].Value = 1;
			}
			else
			{
				cmd.Parameters["@ScheduleOnWeekends"].Value = 0;
			}

			if (ScheduleOnHolidays)
			{
				cmd.Parameters["@ScheduleOnHolidays"].Value = 1;
			}
			else
			{
				cmd.Parameters["@ScheduleOnHolidays"].Value = 0;
			}

			cmd.Parameters["@StartDate"].Value = StartDate;
			cmd.Parameters["@Duration"].Value = Duration;
			cmd.Parameters["@AppointmentPeriod"].Value = AppointmentPeriod;
			cmd.Parameters["@AppointmentPeriodText"].Value = AppointmentPeriodText;
			cmd.Parameters["@Description"].Value = Description;
			cmd.Parameters["@AppointmentTimeInterval"].Value = AppointmentTimeInterval;
			cmd.Parameters["@AppointmentDayOfTheWeekText"].Value = AppointmentDayOfTheWeekText;
			cmd.Parameters["@AppointmentDayOfTheWeek"].Value = AppointmentDayOfTheWeek;
			cmd.Parameters["@AppointmentReoccuranceInterval"].Value = AppointmentReoccuranceInterval;

			if (AppointmentOption2Selected)
			{
				cmd.Parameters["@AppointmentOption2Selected"].Value = 1;
			}
			else
			{
				cmd.Parameters["@AppointmentOption2Selected"].Value = 0;
			}

			cmd.Parameters["@AppointmentTimeOptionSelectionText"].Value = AppointmentTimeOptionSelectionText;
			cmd.Parameters["@AppointmentTimeOptionSelection"].Value = AppointmentTimeOptionSelection;
			cmd.Parameters["@AppointmentMonthSelectionText"].Value = AppointmentMonthSelectionText;
			cmd.Parameters["@AppointmentMonthSelection"].Value = AppointmentMonthSelection;
			cmd.Parameters["@AppointmentDayOfTheMonth"].Value = AppointmentDayOfTheMonth;

			if (_TestSetDefinitionGuid == Guid.Empty)
			{
				cmd.Parameters["@TestSetDefinitionGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@TestSetDefinitionGuid"].Value = _TestSetDefinitionGuid;
			}

			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@" + GetPrimaryKeyColumnName(AssociatedType)].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE " + GetTableName(AssociatedType) + " SET " +
			  "SiteGuid = @SiteGuid ," +
			  GetAssociatedGuidColumnName(AssociatedType) + " = @AssociatedTypeGuid," +
			  "AssetText = @AssetText," +
			  "AppointmentCategory = @AppointmentCategory," +
			  "AppointmentIsSingle = @AppointmentIsSingle," +
			  "ScheduleOnWeekends = @ScheduleOnWeekends," +
			  "ScheduleOnHolidays = @ScheduleOnHolidays," +
			  "StartDate = @StartDate," +
			  "Duration = @Duration," +
			  "AppointmentPeriod = @AppointmentPeriod," +
			  "AppointmentPeriodText = @AppointmentPeriodText," +
			  "Description = @Description," +
			  "AppointmentTimeInterval = @AppointmentTimeInterval," +
			  "AppointmentDayOfTheWeekText = @AppointmentDayOfTheWeekText," +
			  "AppointmentDayOfTheWeek = @AppointmentDayOfTheWeek," +
			  "AppointmentReoccuranceInterval = @AppointmentReoccuranceInterval," +
			  "AppointmentOption2Selected = @AppointmentOption2Selected," +
			  "AppointmentTimeOptionSelectionText = @AppointmentTimeOptionSelectionText," +
			  "AppointmentTimeOptionSelection = @AppointmentTimeOptionSelection," +
			  "AppointmentMonthSelectionText = @AppointmentMonthSelectionText," +
			  "AppointmentMonthSelection = @AppointmentMonthSelection," +
			  "AppointmentDayOfTheMonth = @AppointmentDayOfTheMonth," +
			  "TestSetDefinitionGuid = @TestSetDefinitionGuid," +
			  "CreatedDate = @CreatedDate," +
			  "CreatedBy = @CreatedBy," +
			  "UpdatedDate = @UpdatedDate," +
			  "UpdatedBy = @UpdatedBy " +
			  " WHERE " + GetPrimaryKeyColumnName(AssociatedType) + " = @AppointmentGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AssociatedTypeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AssetText", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@AppointmentCategory", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@AppointmentIsSingle", SqlDbType.Bit);
			cmd.Parameters.Add("@ScheduleOnWeekends", SqlDbType.Bit);
			cmd.Parameters.Add("@ScheduleOnHolidays", SqlDbType.Bit);
			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@Duration", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentPeriod", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentPeriodText", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@AppointmentTimeInterval", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentDayOfTheWeekText", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@AppointmentDayOfTheWeek", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentReoccuranceInterval", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentOption2Selected", SqlDbType.Bit);
			cmd.Parameters.Add("@AppointmentTimeOptionSelectionText", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@AppointmentTimeOptionSelection", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentMonthSelectionText", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@AppointmentMonthSelection", SqlDbType.Int);
			cmd.Parameters.Add("@AppointmentDayOfTheMonth", SqlDbType.Int);
			cmd.Parameters.Add("@TestSetDefinitionGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@AppointmentGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@AssociatedTypeGuid"].Value = AssociatedTypeGuid;
			cmd.Parameters["@AssetText"].Value = AssetText;
			cmd.Parameters["@AppointmentCategory"].Value = AppointmentCategory;

			if (AppointmentIsSingle)
			{
				cmd.Parameters["@AppointmentIsSingle"].Value = 1;
			}
			else
			{
				cmd.Parameters["@AppointmentIsSingle"].Value = 0;
			}

			if (ScheduleOnWeekends)
			{
				cmd.Parameters["@ScheduleOnWeekends"].Value = 1;
			}
			else
			{
				cmd.Parameters["@ScheduleOnWeekends"].Value = 0;
			}

			if (ScheduleOnHolidays)
			{
				cmd.Parameters["@ScheduleOnHolidays"].Value = 1;
			}
			else
			{
				cmd.Parameters["@ScheduleOnHolidays"].Value = 0;
			}

			cmd.Parameters["@StartDate"].Value = StartDate;
			cmd.Parameters["@Duration"].Value = Duration;
			cmd.Parameters["@AppointmentPeriod"].Value = AppointmentPeriod;
			cmd.Parameters["@AppointmentPeriodText"].Value = AppointmentPeriodText;
			cmd.Parameters["@Description"].Value = Description;
			cmd.Parameters["@AppointmentTimeInterval"].Value = AppointmentTimeInterval;
			cmd.Parameters["@AppointmentDayOfTheWeekText"].Value = AppointmentDayOfTheWeekText;
			cmd.Parameters["@AppointmentDayOfTheWeek"].Value = AppointmentDayOfTheWeek;
			cmd.Parameters["@AppointmentReoccuranceInterval"].Value = AppointmentReoccuranceInterval;

			if (AppointmentOption2Selected)
			{
				cmd.Parameters["@AppointmentOption2Selected"].Value = 1;
			}
			else
			{
				cmd.Parameters["@AppointmentOption2Selected"].Value = 0;
			}

			cmd.Parameters["@AppointmentTimeOptionSelectionText"].Value = AppointmentTimeOptionSelectionText;
			cmd.Parameters["@AppointmentTimeOptionSelection"].Value = AppointmentTimeOptionSelection;
			cmd.Parameters["@AppointmentMonthSelectionText"].Value = AppointmentMonthSelectionText;
			cmd.Parameters["@AppointmentMonthSelection"].Value = AppointmentMonthSelection;
			cmd.Parameters["@AppointmentDayOfTheMonth"].Value = AppointmentDayOfTheMonth;

			if (_TestSetDefinitionGuid == Guid.Empty)
			{
				cmd.Parameters["@TestSetDefinitionGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@TestSetDefinitionGuid"].Value = _TestSetDefinitionGuid;
			}

			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@AppointmentGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM " + GetTableName(AssociatedType) + " WHERE " + GetPrimaryKeyColumnName(AssociatedType) + " = @AppointmentGuid";

			cmd.Parameters.Add("@AppointmentGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AppointmentGuid"].Value = IdentityGuid;
		}

		public void PurgeByAssetIDSQL(SqlCommand cmd, Guid assetID)
		{
			cmd.CommandText = "DELETE FROM " + GetTableName(AssociatedType) + " WHERE " + GetAssociatedGuidColumnName(AssociatedType) + " = @AssetID";

			cmd.Parameters.Add("@AssetID", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AssetID"].Value = assetID;
		}

		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = SelectClause("Equipment") +
				" FROM tblAppointmentEquipment " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid AND Description = @Description";

			cmd.CommandText += " UNION ";

			cmd.CommandText += SelectClause("Tanks") +
				" FROM tblAppointmentTank " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid AND Description = @Description";

			cmd.CommandText += " UNION ";

			cmd.CommandText += SelectClause("Personnel") +
				" FROM tblAppointmentPersonnel " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid AND Description = @Description";


			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
			cmd.Parameters["@Description"].Value = Description;
		}

		public void EnumerateScheduledAndOverdueSQL(SqlCommand cmd, SecurityClass security, DateTimeOffset startDate, string appointmentType, bool bInTransaction)
		{

			if (appointmentType.Equals("ALL", StringComparison.OrdinalIgnoreCase))
			{
				//despite the use of "ALL", this SQL should not enumerate personnel appointments 		
				cmd.CommandText = SelectClause("Equipment") + ", tblEquipment.FuelingType FROM tblAppointmentEquipment" + SQLUpdateLock(bInTransaction) +
                " LEFT OUTER JOIN tblEquipment ON tblEquipment.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', tblAppointmentEquipment.EquipmentGuid, @SiteGuid)" +                   					
                " WHERE tblAppointmentEquipment.SiteGuid = @SiteGuid AND AppointmentCategory = 'Quality Control'" +
					" AND StartDate < @StartDate";

				cmd.CommandText += " UNION ";

				cmd.CommandText += SelectClause("Tanks") + ", NULL AS FuelingType FROM tblAppointmentTank" + SQLUpdateLock(bInTransaction) +
					" WHERE tblAppointmentTank.SiteGuid = @SiteGuid AND AppointmentCategory = 'Quality Control'" +
					" AND StartDate < @StartDate";
			}
			else
			{
				cmd.CommandText = SelectClause(appointmentType) + ",";

				if (appointmentType.Equals("Equipment", StringComparison.OrdinalIgnoreCase))
				{
					cmd.CommandText += " tblEquipment.FuelingType ";
				}
				else
				{
					cmd.CommandText += " NULL AS FuelingType ";
				}

				cmd.CommandText += " FROM " + GetTableName(appointmentType) + " " + SQLUpdateLock(bInTransaction);

				if (appointmentType.Equals("Equipment", StringComparison.OrdinalIgnoreCase))
				{
					cmd.CommandText += " LEFT OUTER JOIN tblEquipment ON tblEquipment.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', tblAppointmentEquipment.EquipmentGuid, @SiteGuid) ";
				}

				cmd.CommandText += " WHERE " + GetTableName(appointmentType) + ".SiteGuid = @SiteGuid AND AppointmentCategory = 'Quality Control'" +
					" AND StartDate < @StartDate";
			}

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
			cmd.Parameters["@StartDate"].Value = startDate;
		}

		public void EnumerateBasedOnTestSetAndEquipmentSQL(SqlCommand cmd, SecurityClass security, Guid testSetDefinitionGuid, Guid equipmentGuid, bool bInTransaction)
		{
			cmd.CommandText = SelectClause("Equipment") + " FROM tblAppointmentEquipment " + SQLUpdateLock(bInTransaction)
				+ " WHERE SiteGuid = @SiteGuid"
				+ " AND EquipmentGuid = @EquipmentGuid"
				+ " AND TestSetDefinitionGuid = @TestSetDefinitionGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@TestSetDefinitionGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
			cmd.Parameters["@EquipmentGuid"].Value = equipmentGuid;
			cmd.Parameters["@TestSetDefinitionGuid"].Value = testSetDefinitionGuid;
		}

		public void EnumerateBasedOnTestSetAndTankSQL(SqlCommand cmd, SecurityClass security, Guid testSetDefinitionGuid, Guid tankGuid, bool bInTransaction)
		{
			cmd.CommandText = SelectClause("Tanks") + " FROM tblAppointmentTank " + SQLUpdateLock(bInTransaction)
				+ " WHERE SiteGuid = @SiteGuid"
				+ " AND TankGuid = @TankGuid"
				+ " AND TestSetDefinitionGuid = @TestSetDefinitionGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@TestSetDefinitionGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
			cmd.Parameters["@TankGuid"].Value = tankGuid;
			cmd.Parameters["@TestSetDefinitionGuid"].Value = testSetDefinitionGuid;
		}

		public void EnumerateByAssetGuidSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction, string appointmentType, Guid entityGuid)
		{
			cmd.CommandText = SelectClause(appointmentType) + " FROM " + GetTableName(appointmentType) + " WHERE " + GetAssociatedGuidColumnName(appointmentType) + " = @EntityGuid";

			cmd.Parameters.Add("@EntityGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@EntityGuid"].Value = entityGuid;
		}

		public void EnumerateAll(SqlCommand cmd, SecurityClass security, bool bInTransaction, string appointmentType)
		{

			if (appointmentType == "All")
			{
				cmd.CommandText = SelectClause("Equipment") +
					" FROM tblAppointmentEquipment " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid";

				cmd.CommandText += " UNION ";

				cmd.CommandText += SelectClause("Tanks") +
					" FROM tblAppointmentTank " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid";

				cmd.CommandText += " UNION ";

				cmd.CommandText += SelectClause("Personnel") +
					" FROM tblAppointmentPersonnel " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid";
			}
			else
			{
				cmd.CommandText = SelectClause(appointmentType) +
					" FROM " + GetTableName(appointmentType) + " " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid ";
			}

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
		}

		public void EnumerateByIdentityGuid(SqlCommand cmd, SecurityClass security, bool bInTransaction, Guid appointmentGuid)
		{
			cmd.CommandText = SelectClause("Equipment") +
				" FROM tblAppointmentEquipment " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid AND " + GetPrimaryKeyColumnName("Equipment") + " = @AppointmentGuid";

			cmd.CommandText += " UNION ";

			cmd.CommandText += SelectClause("Tanks") +
				" FROM tblAppointmentTank " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid AND " + GetPrimaryKeyColumnName("Tanks") + " = @AppointmentGuid";

			cmd.CommandText += " UNION ";

			cmd.CommandText += SelectClause("Personnel") +
				" FROM tblAppointmentPersonnel " + SQLUpdateLock(bInTransaction) + " WHERE SiteGuid = @SiteGuid AND " + GetPrimaryKeyColumnName("Personnel") + " = @AppointmentGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AppointmentGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
			cmd.Parameters["@AppointmentGuid"].Value = appointmentGuid;
		}

		public void EnumerateEquipmentQCItems(SqlCommand cmd, SecurityClass security, bool bInTransaction, Guid typeGuid, string associatedType)
		{
			if (associatedType != "Tanks" && associatedType != "Equipment")
			{
				cmd.CommandText = SelectClause("Equipment") +
					" FROM " + GetTableName("Equipment") + SQLUpdateLock(bInTransaction) +
					" WHERE SiteGuid = @SiteGuid " +
					" AND " + GetAssociatedGuidColumnName("Equipment") + " = @TypeGuid " +
					" AND [AppointmentCategory] IN ('Quality Control', 'Maintenance')" + //only want quality or maintenance items to affect qc
					" UNION " +
					SelectClause("Tanks") +
					" FROM " + GetTableName("Tanks") + SQLUpdateLock(bInTransaction) +
					" WHERE SiteGuid = @SiteGuid " +
					" AND " + GetAssociatedGuidColumnName("Tanks") + " = @TypeGuid " +
					" AND [AppointmentCategory] IN ('Quality Control', 'Maintenance')"; //only want quality or maintenance items to affect qc

			}
			else
			{
				cmd.CommandText = SelectClause(associatedType) +
					" FROM " + GetTableName(associatedType) + SQLUpdateLock(bInTransaction) +
					" WHERE SiteGuid = @SiteGuid " +
					" AND " + GetAssociatedGuidColumnName(associatedType) + " = @TypeGuid " +
					" AND [AppointmentCategory] IN ('Quality Control', 'Maintenance')"; //only want quality or maintenance items to affect qc
			}

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@TypeGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
			cmd.Parameters["@TypeGuid"].Value = typeGuid;
		}

	}

}
