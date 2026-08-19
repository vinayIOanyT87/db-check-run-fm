using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	public enum SCHEDULE_TYPE
	{
		TERMINAL_OPERATIONS_TYPE = 0,
		COMPANY_ACCESS_TYPE = 1,
		HOLIDAY_TYPE = 2,
		PERSON_ACCESS_TYPE = 3,
		MAX_SCHEDULE_TYPE = 4
	};

	public enum DAY_OF_WEEK
	{
		SUNDAY = 0,
		MONDAY = 1,
		TUESDAY = 2,
		WEDNESDAY = 3,
		THURSDAY = 4,
		FRIDAY = 5,
		SATURDAY = 6,
		HOLIDAY = 7
	};


	[CollectionDataContract]
	[Serializable]
	[KnownType(typeof(ScheduleClass))]
	public class ScheduleCollectionClass : List<ScheduleClass> { }

	[KnownType(typeof(GregorianCalendar))]
	[DataContract]
	[Serializable]
	public class ScheduleClass : BaseDataObject
	{
		[DataMember]
		private Guid entityGuid;
		[DataMember]
		private SCHEDULE_TYPE type;
		[DataMember]
		private int day;
		[DataMember]
		private DateTimeOffset? holidayDate;
		[DataMember]
		private bool enabled;
		[DataMember]
		private Time openingTime;
		[DataMember]
		private Time closingTime;
		[DataMember]
		private bool endOfDayEnabled;
		[DataMember]
		private Time endOfDayTime;

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Schedule class.
		/// </summary>
		public ScheduleClass()
		{
			this.openingTime = new Time();
			this.closingTime = new Time();
			this.endOfDayTime = new Time();
			this.Reset();
		}

		/// <summary>
		/// This constructor will initialize the schedule class based on the
		/// date time format information.
		/// </summary>
		/// <param name="dateTimeFormatInfo"></param>
		public ScheduleClass(DateTimeFormatInfo dateTimeFormatInfo)
		{
			this.openingTime = new Time(dateTimeFormatInfo);
			this.closingTime = new Time(dateTimeFormatInfo);
			this.endOfDayTime = new Time(dateTimeFormatInfo);
			this.Reset();
		}

		/// <summary>
		/// This constructor will initialize the schedule class based on the
		/// site information.
		/// </summary>
		/// <param name="site"></param>
		public ScheduleClass(SiteClass site)
		{
			this.openingTime = new Time(site);
			this.closingTime = new Time(site);
			this.endOfDayTime = new Time(site);
			this.Reset();
		}
		#endregion

		#region Properties
		public Time EndOfDayTime
		{
			get { return this.endOfDayTime; }
			set { this.endOfDayTime = value; }
		}

		[EntityImportExport("ENDOFDAYENABLED", 110, "EndOfDayEnabled")]
		public bool EndOfDayEnabled
		{
			get { return this.endOfDayEnabled; }
			set { this.endOfDayEnabled = value; }
		}

		[EntityImportExport("Type", 110, "Type")]
		public SCHEDULE_TYPE Type
		{
			get { return this.type; }
			set { this.type = value; }
		}

		public Time ClosingTime
		{
			get { return this.closingTime; }
			set { this.closingTime = value; }
		}

		public Time OpeningTime
		{
			get { return this.openingTime; }
			set { this.openingTime = value; }
		}

		[EntityImportExport("ENABLED", 110, "Enabled")]
		public bool Enabled
		{
			get { return this.enabled; }
			set { this.enabled = value; }
		}

		public Guid EntityGuid
		{
			get { return this.entityGuid; }
			set { this.entityGuid = value; }
		}

		public int Day
		{
			get { return this.day; }
			set { this.day = value; }
		}

		public DateTimeOffset? HolidayDate
		{
			get { return this.holidayDate; }
			set { this.holidayDate = value; }
		}

		[EntityImportExport("OPENINGTIME", 110, "OpeningTimeString")]
		public string OpeningTimeString
		{
			get { return this.openingTime.ToString(); }
			set
			{
				bool validTimeFound = DateTimeOffset.TryParse(TimeConverter.MinFMDate.ToString("d", this.openingTime.Format) + " " + value,
											this.openingTime.Format, DateTimeStyles.None, out DateTimeOffset tempDate);
				if (validTimeFound)
				{
					this.openingTime.Value = tempDate;
				}
				else
				{
					// May have been edited in Excel, which will write out the time as an ISO 8601 datetime with a date of 1899-12-31.
					// Accept it _only_ with that date
					validTimeFound = DateTimeOffset.TryParse(value, this.openingTime.Format, DateTimeStyles.RoundtripKind, out tempDate); // RoundtripKind is the "Sortable" ISO 8601 format
					if (validTimeFound)
					{
						DateTime timeOnlyDate = new DateTime(1899, 12, 31, 0, 0, 0);
						if (tempDate.Date == timeOnlyDate.Date)
						{
							this.openingTime.Value = tempDate;
						}
						else
						{
							// someone put a full date/time in for the schedule time, which is not valid.
							this.openingTime.Value = TimeConverter.DefaultFMStartTime;
						}
					}
					else
					{
						this.openingTime.Value = TimeConverter.DefaultFMStartTime;
					}
				}
			}
		}

		[EntityImportExport("CLOSINGTIME", 110, "ClosingTimeString")]
		public string ClosingTimeString
		{
			get { return this.closingTime.ToString(); }
			set
			{
				bool validTimeFound = DateTimeOffset.TryParse(TimeConverter.MinFMDate.ToString("d", this.closingTime.Format) + " " + value,
											this.closingTime.Format, DateTimeStyles.None, out DateTimeOffset tempDate);
				if (validTimeFound)
				{
					this.closingTime.Value = tempDate;
				}
				else
				{
					// May have been edited in Excel, which will write out the time as an ISO 8601 datetime with a date of 1899-12-31.
					// Accept it _only_ with that date
					validTimeFound = DateTimeOffset.TryParse(value, this.closingTime.Format, DateTimeStyles.RoundtripKind, out tempDate); // RoundtripKind is the "Sortable" ISO 8601 format
					if (validTimeFound)
					{
						DateTime timeOnlyDate = new DateTime(1899, 12, 31, 0, 0, 0);
						if (tempDate.Date == timeOnlyDate.Date)
						{
							this.closingTime.Value = tempDate;
						}
						else
						{
							// someone put a full date/time in for the schedule time, which is not valid.
							this.closingTime.Value = TimeConverter.DefaultFMStartTime;
						}
					}
					else
					{
						this.closingTime.Value = TimeConverter.DefaultFMStartTime;
					}
				}
			}
		}

		[EntityImportExport("ENDOFDAYTIME", 110, "EndOfDayTimeString")]
		public string EndOfDayTimeString
		{
			get { return this.endOfDayTime.ToString(); }
			set
			{
				bool validTimeFound = DateTimeOffset.TryParse(TimeConverter.MinFMDate.ToString("d", this.endOfDayTime.Format) + " " + value,
											this.endOfDayTime.Format, DateTimeStyles.None, out DateTimeOffset tempDate);
				if (validTimeFound)
				{
					this.endOfDayTime.Value = tempDate;
				}
				else
				{
					// May have been edited in Excel, which will write out the time as an ISO 8601 datetime with a date of 1899-12-31.
					// Accept it _only_ with that date
					validTimeFound = DateTimeOffset.TryParse(value, this.endOfDayTime.Format, DateTimeStyles.RoundtripKind, out tempDate); // RoundtripKind is the "Sortable" ISO 8601 format
					if (validTimeFound)
					{
						DateTime timeOnlyDate = new DateTime(1899, 12, 31, 0, 0, 0);
						if (tempDate.Date == timeOnlyDate.Date)
						{
							this.endOfDayTime.Value = tempDate;
						}
						else
						{
							// someone put a full date/time in for the schedule time, which is not valid.
							this.endOfDayTime.Value = TimeConverter.DefaultFMStartTime;
						}
					}
					else
					{
						this.endOfDayTime.Value = TimeConverter.DefaultFMStartTime;
					}
				}
			}
		}

		[EntityImportExport("ACCESSSCHEDULEID", 100, "ID")]
		public override string ID
		{
			get { return base.ID; }
			set { base.ID = value; }
		}

		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				switch (this.type)
				{
					case SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE:
						return ENTITY_TYPE.SCHEDULE_TERMINAL_OPERATIONS;
					case SCHEDULE_TYPE.COMPANY_ACCESS_TYPE:
						return ENTITY_TYPE.SCHEDULE_COMPANY_ACCESS;
					case SCHEDULE_TYPE.HOLIDAY_TYPE:
						return ENTITY_TYPE.SCHEDULE_HOLIDAY;
					case SCHEDULE_TYPE.PERSON_ACCESS_TYPE:
						return ENTITY_TYPE.SCHEDULE_PERSON_ACCESS;
					default:
						return ENTITY_TYPE.UNDEFINED;
				}
			}

			set
			{
				if (value == ENTITY_TYPE.SCHEDULE_TERMINAL_OPERATIONS)
				{
					this.type = SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE;
				}
				else if (value == ENTITY_TYPE.SCHEDULE_COMPANY_ACCESS)
				{
					this.type = SCHEDULE_TYPE.COMPANY_ACCESS_TYPE;
				}
				else if (value == ENTITY_TYPE.SCHEDULE_HOLIDAY)
				{
					this.type = SCHEDULE_TYPE.HOLIDAY_TYPE;
				}
				else if (value == ENTITY_TYPE.SCHEDULE_PERSON_ACCESS)
				{
					this.type = SCHEDULE_TYPE.PERSON_ACCESS_TYPE;
				}
			}
		}

		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				switch (this.Type)
				{
					case SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE:
						return ENTITY_TYPE.SITE;
					case SCHEDULE_TYPE.COMPANY_ACCESS_TYPE:
						return ENTITY_TYPE.COMPANY;
					case SCHEDULE_TYPE.HOLIDAY_TYPE:
						return ENTITY_TYPE.SITE;
					case SCHEDULE_TYPE.PERSON_ACCESS_TYPE:
						return ENTITY_TYPE.PERSONNEL;
					default:
						return ENTITY_TYPE.NONE;
				}
			}
		}

		public bool IsAHolidaySchedule
		{
			get
			{
				return this.Type == SCHEDULE_TYPE.HOLIDAY_TYPE;
			}
		}
		/// <summary>
		/// Day Column could be Day of Week or actual Holiday Date
		/// </summary>
		/// <param name="scheduleType"></param>
		/// <returns></returns>
		public static string GetDayColumnName(SCHEDULE_TYPE scheduleType)
		{
			string retValue = scheduleType == SCHEDULE_TYPE.HOLIDAY_TYPE ? "HolidayDate" : "LookupDayOfWeekIndex";
			return retValue;
		}

		public static string GetTableName(SCHEDULE_TYPE scheduleType)
		{
			switch (scheduleType)
			{
				case SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE:
					return "tblScheduleTerminalOperation";
				case SCHEDULE_TYPE.COMPANY_ACCESS_TYPE:
					return "tblScheduleCompanyAccess";
				case SCHEDULE_TYPE.HOLIDAY_TYPE:
					return "tblScheduleHoliday";
				case SCHEDULE_TYPE.PERSON_ACCESS_TYPE:
					return "tblSchedulePersonnelAccess";
				default:
					return "Unknown";
			}
		}

		public static string GetPrimaryKeyColumnName(SCHEDULE_TYPE scheduleType)
		{
			switch (scheduleType)
			{
				case SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE:
					return "ScheduleTerminalOperationGuid";
				case SCHEDULE_TYPE.COMPANY_ACCESS_TYPE:
					return "ScheduleCompanyAccessGuid";
				case SCHEDULE_TYPE.HOLIDAY_TYPE:
					return "ScheduleHolidayGuid";
				case SCHEDULE_TYPE.PERSON_ACCESS_TYPE:
					return "SchedulePersonnelAccessGuid";
				default:
					return "Unknown";
			}
		}

		public static string GetForeignKeyColumnName(SCHEDULE_TYPE scheduleType)
		{
			switch (scheduleType)
			{
				case SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE:
					return "SiteGuid";
				case SCHEDULE_TYPE.COMPANY_ACCESS_TYPE:
					return "CompanyGuid";
				case SCHEDULE_TYPE.HOLIDAY_TYPE:
					return "SiteGuid";
				case SCHEDULE_TYPE.PERSON_ACCESS_TYPE:
					return "PersonnelGuid";
				default:
					return "Unknown";
			}
		}

		[EntityImportExport("DAY", 110, "DayText")]
		public string DayText
		{
			get
			{
				if ((this.type == SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE) ||
					(this.type == SCHEDULE_TYPE.COMPANY_ACCESS_TYPE) ||
					(this.type == SCHEDULE_TYPE.PERSON_ACCESS_TYPE))
				{
					switch ((DAY_OF_WEEK)this.day)
					{
						case DAY_OF_WEEK.SUNDAY:
							return "Sunday";
						case DAY_OF_WEEK.MONDAY:
							return "Monday";
						case DAY_OF_WEEK.TUESDAY:
							return "Tuesday";
						case DAY_OF_WEEK.WEDNESDAY:
							return "Wednesday";
						case DAY_OF_WEEK.THURSDAY:
							return "Thursday";
						case DAY_OF_WEEK.FRIDAY:
							return "Friday";
						case DAY_OF_WEEK.SATURDAY:
							return "Saturday";
						case DAY_OF_WEEK.HOLIDAY:
							return "Holiday";
						default:
							return "Undefined";
					}
				}
				else
				{
					string retValue = string.Empty;
					if (this.holidayDate != null && this.holidayDate.HasValue)
					{
						retValue = this.holidayDate.Value.ToString("d", this.openingTime.Format);
					}
					return retValue;
				}
			}

			set
			{
				if ((this.type == SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE) ||
					(this.type == SCHEDULE_TYPE.COMPANY_ACCESS_TYPE) ||
					(this.type == SCHEDULE_TYPE.PERSON_ACCESS_TYPE) ||
					(this.type == SCHEDULE_TYPE.MAX_SCHEDULE_TYPE))
				{
					if (value == "Sunday")
						this.day = (int)DAY_OF_WEEK.SUNDAY;
					else if (value == "Monday")
						this.day = (int)DAY_OF_WEEK.MONDAY;
					else if (value == "Tuesday")
						this.day = (int)DAY_OF_WEEK.TUESDAY;
					else if (value == "Wednesday")
						this.day = (int)DAY_OF_WEEK.WEDNESDAY;
					else if (value == "Thursday")
						this.day = (int)DAY_OF_WEEK.THURSDAY;
					else if (value == "Friday")
						this.day = (int)DAY_OF_WEEK.FRIDAY;
					else if (value == "Saturday")
						this.day = (int)DAY_OF_WEEK.SATURDAY;
					else if (value == "Holiday")
						this.day = (int)DAY_OF_WEEK.HOLIDAY;
				}
				else
				{
					this.holidayDate = DateTimeOffset.TryParse(value, this.openingTime.Format, DateTimeStyles.None, out DateTimeOffset tempDate) ? tempDate : (DateTimeOffset?)null;
				}
			}
		}
		#endregion

		public override void Reset()
		{
			base.Reset();

			this.entityGuid = Guid.Empty;
			this.type = SCHEDULE_TYPE.MAX_SCHEDULE_TYPE;
			this.day = 0;
			this.holidayDate = null;
			this.enabled = true;
			this.openingTime.Value = TimeConverter.DefaultFMStartTime;
			this.closingTime.Value = TimeConverter.DefaultFMEndTime;
			this.endOfDayEnabled = true;
			this.endOfDayTime.Value = TimeConverter.DefaultFMEndTime;
		}

		public override void Load(Object o)
		{
			SCHEDULE_TYPE scheduleType = this.type;

			this.Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				this.type = scheduleType;

				_IdentityGuid = DataObject.getValue(Row[GetPrimaryKeyColumnName(this.Type)], Guid.Empty);
				this.entityGuid = DataObject.getValue(Row[GetForeignKeyColumnName(this.Type)], Guid.Empty);

				string dayColumnName = GetDayColumnName(this.Type);
				if (this.IsAHolidaySchedule)
				{
					this.holidayDate = DataObject.getValue<DateTimeOffset?>(Row[dayColumnName], null);
				}
				else
				{
					this.day = DataObject.getValue(Row[dayColumnName], 0);
				}
				this.enabled = DataObject.getValue(Row["Enabled"], true);
				this.openingTime.Value = DataObject.getValue(Row["OpeningTime"], TimeConverter.DefaultFMStartTime);
				this.closingTime.Value = DataObject.getValue(Row["ClosingTime"], TimeConverter.DefaultFMEndTime);
				this.endOfDayEnabled = DataObject.getValue(Row["EndOfDayEnabled"], true);
				this.endOfDayTime.Value = DataObject.getValue(Row["EndOfDayTime"], TimeConverter.DefaultFMEndTime);
				_CreatedDate = DataObject.getValue(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue(Row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue(Row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue(Row["UpdatedBy"], ADMIN);
			}

			else if (typeof(XmlNode).IsInstanceOfType(o))
			{
				XmlNode Node = (XmlNode)o;

				if (Node.ParentNode.ParentNode.Name == "Company")
				{
					this.type = SCHEDULE_TYPE.COMPANY_ACCESS_TYPE;
				}
				else if (Node.Name == "OperatingScheduleEntry")
				{
					this.type = SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE;
				}
				else if (Node.Name == "HolidayScheduleEntry")
				{
					this.type = SCHEDULE_TYPE.HOLIDAY_TYPE;
				}
				else if (Node.ParentNode.ParentNode.Name == "Person")
				{
					this.type = SCHEDULE_TYPE.PERSON_ACCESS_TYPE;
				}
				else
				{
					throw new Exception("Invalid Schedule Type");
				}

				string DayOne = TimeConverter.MinFMDate.ToString("d");

				DayText = Node.Attributes["ID"].Value;
				this.enabled = (bool)Convert.ChangeType(Node.Attributes["Enabled"].Value, typeof(bool));
				this.openingTime.Value = DateTimeOffset.Parse(DayOne + " " + Node.Attributes["OpeningTime"].Value, this.openingTime.Format);
				this.closingTime.Value = DateTimeOffset.Parse(DayOne + " " + Node.Attributes["ClosingTime"].Value, this.closingTime.Format);

				if (Node.Attributes["EndOfDayEnabled"] != null)
				{
					this.endOfDayEnabled = (bool)Convert.ChangeType(Node.Attributes["EndOfDayEnabled"].Value, typeof(bool));
				}
				if (Node.Attributes["EndOfDayTime"] != null)
				{
					this.endOfDayTime.Value = DateTimeOffset.Parse(DayOne + " " + Node.Attributes["EndOfDayTime"].Value, this.endOfDayTime.Format);
				}
			}
		}

		public override void Store(Object o)
		{
			if (o == null)
				throw new ArgumentNullException("Object");

			if (typeof(XmlNode).IsInstanceOfType(o))
			{
				XmlNode Node = (XmlNode)o;

				XmlAttribute Attribute;

				Attribute = Node.OwnerDocument.CreateAttribute("ID");
				Attribute.Value = ID;
				Node.Attributes.Append(Attribute);

				Attribute = Node.OwnerDocument.CreateAttribute("Enabled");
				Attribute.Value = this.enabled.ToString();
				Node.Attributes.Append(Attribute);

				Attribute = Node.OwnerDocument.CreateAttribute("OpeningTime");
				Attribute.Value = this.openingTime.ToString();
				Node.Attributes.Append(Attribute);

				Attribute = Node.OwnerDocument.CreateAttribute("ClosingTime");
				Attribute.Value = this.closingTime.ToString();
				Node.Attributes.Append(Attribute);

				if ((this.IsAHolidaySchedule) ||
					(this.type == SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE))
				{
					Attribute = Node.OwnerDocument.CreateAttribute("EndOfDayEnabled");
					Attribute.Value = this.endOfDayEnabled.ToString();
					Node.Attributes.Append(Attribute);

					Attribute = Node.OwnerDocument.CreateAttribute("EndOfDayTime");
					Attribute.Value = this.endOfDayTime.ToString();
					Node.Attributes.Append(Attribute);
				}
			}
			else
				throw new Exception("Store Error - Invalid Object Type : " + o.GetType().ToString());
		}

		private void AddDayColumnParameter(SqlCommand cmd)
		{
			object dayValue = null;
			if (this.IsAHolidaySchedule)
			{
				if (this.holidayDate != null && this.holidayDate.HasValue)
				{
					dayValue = this.holidayDate.Value;
				}
			}
			else
			{
				dayValue = this.day;
			}
			cmd.Parameters.AddWithValue("@Day", dayValue);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO " + GetTableName(this.Type) + " " +
					"(" + GetForeignKeyColumnName(this.Type) + "," +
					GetDayColumnName(Type) + "," +
					"Enabled," +
					"OpeningTime," +
					"ClosingTime," +
					"EndOfDayEnabled," +
					"EndOfDayTime," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy" +
					") VALUES (" +
					"@EntityGuid," +
					"@Day," +
					"@Enabled," +
					"@OpeningTime," +
					"@ClosingTime," +
					"@EndOfDayEnabled," +
					"@EndOfDayTime," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy)";

			cmd.Parameters.AddWithValue("@EntityGuid", this.EntityGuid);
			AddDayColumnParameter(cmd);
			cmd.Parameters.AddWithValue("@Enabled", ((this.enabled) ? 1 : 0));
			cmd.Parameters.AddWithValue("@OpeningTime", TimeConverter.ToFMTime(this.openingTime.Value));
			cmd.Parameters.AddWithValue("@ClosingTime", TimeConverter.ToFMTime(this.closingTime.Value));
			cmd.Parameters.AddWithValue("@EndOfDayEnabled", ((this.endOfDayEnabled) ? 1 : 0));
			cmd.Parameters.AddWithValue("@EndOfDayTime", TimeConverter.ToFMTime(this.endOfDayTime.Value));
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE " + GetTableName(this.Type) + " " +
					"SET " +
					GetDayColumnName(Type) + " = @Day, " +
					"Enabled = @Enabled, " +
					"OpeningTime = @OpeningTime, " +
					"ClosingTime = @ClosingTime, " +
					"EndOfDayEnabled = @EndOfDayEnabled, " +
					"EndOfDayTime = @EndOfDayTime, " +
					"UpdatedDate = @UpdatedDate, " +
					"UpdatedBy = @UpdatedBy " +
					"WHERE " + GetPrimaryKeyColumnName(this.Type) + " = @ScheduleGuid";

			AddDayColumnParameter(cmd);
			cmd.Parameters.AddWithValue("@Enabled", ((this.enabled) ? 1 : 0));
			cmd.Parameters.AddWithValue("@OpeningTime", TimeConverter.ToFMTime(this.openingTime.Value));
			cmd.Parameters.AddWithValue("@ClosingTime", TimeConverter.ToFMTime(this.closingTime.Value));
			cmd.Parameters.AddWithValue("@EndOfDayEnabled", ((this.endOfDayEnabled) ? 1 : 0));
			cmd.Parameters.AddWithValue("@EndOfDayTime", TimeConverter.ToFMTime(this.endOfDayTime.Value));
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@ScheduleGuid", _IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * " +
					" FROM " + GetTableName(this.Type) + " " + SQLUpdateLock(bInTransaction) + " WHERE " + GetPrimaryKeyColumnName(this.Type) + " =  @ScheduleGuid";

			cmd.Parameters.AddWithValue("@ScheduleGuid", _IdentityGuid);
		}

		public void SelectByEntityGuidTypeAndDaySQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * " +
					" FROM " + GetTableName(this.Type) + " " + SQLUpdateLock(bInTransaction) +
					" WHERE " + GetForeignKeyColumnName(this.Type) + " = @EntityGuid " +
					" AND " + GetDayColumnName(Type) + " = @Day";

			cmd.Parameters.AddWithValue("@EntityGuid", this.EntityGuid);
			AddDayColumnParameter(cmd);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM " + GetTableName(this.Type) + " WHERE " + GetPrimaryKeyColumnName(this.Type) + " = @ScheduleGuid";
			cmd.Parameters.AddWithValue("@ScheduleGuid", _IdentityGuid);
		}


		public void EnumerateByEntityGuidAndTypeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * " +
					" FROM " + GetTableName(this.Type) + " WHERE " + GetForeignKeyColumnName(this.Type) + " = @EntityGuid" +
					" ORDER BY " + GetDayColumnName(Type);

			cmd.Parameters.AddWithValue("@EntityGuid", this.EntityGuid);
		}
	}
}
