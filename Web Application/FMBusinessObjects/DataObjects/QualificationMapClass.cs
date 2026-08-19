namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
	using System.Xml;

	using FMBusinessObjects.UtilityObjects;

	public enum QUALIFICATION_MAP_TYPE
	{
		COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY = 0,
		EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT = 1,
		EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT = 2,
		PERSON_QUALIFICATION_TO_PERSON = 3,
		PERSON_LICENSE_TO_PERSON = 4,
		PERSON_TRAINING_TO_PERSON = 5,
		PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE = 6,
		PERSON_TRAINING_TO_EQUIPMENT_TYPE = 7,
		PERSON_QUALIFICATION_TO_STATION = 8,
		PERSON_TRAINING_TO_STATION = 9,
		EQUIPMENT_TEST_AND_INSPECTION_TO_STATION = 10,
        PERSON_LICENSE_TO_STATION = 11,
        EQUIPMENT_TAG_AND_LICENSE_TO_STATION = 12,
        MAX_QUALIFICATION_MAP_TYPE = 13
	};


   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(QualificationMapClass))]
	public class QualificationMapCollectionClass : List<QualificationMapClass> { }


   [Serializable]
   [DataContract]
	public class QualificationMapClass : BaseDataObject, IAlarmAndEventDiscovery
	{
		#region Protected data members
		[DataMember]
		protected string _Number;
		[DataMember]
		protected int _Reoccurrence;
		[DataMember]
		protected string _Rating;
		[DataMember]
		protected bool _HistoricalRecord;
		[DataMember]
		protected string _Instructor;
		[DataMember]
		protected Date _ExpirationDate;
		#endregion

		#region Properties

		[DataMember]
		public Guid AssigneeGuid { get; set; }

		[DataMember]
		public Guid AssignedGuid { get; set; }

		[EntityImportExportAttribute("TYPE", 110, "Type")]
		[DataMember]
		public QUALIFICATION_MAP_TYPE Type { get; set; }

		[DataMember]
		public int Sequence { get; set; }

		[EntityImportExportAttribute("DATECOMPLETE", 110, "DateCompleted")]
		[DataMember]
		public Date DateCompleted { get; set; }

		[EntityImportExportAttribute("DATEDUE", 110, "DateDue")]
		[DataMember]
		public Date DateDue { get; set; }

		public Date ExpirationDate
		{
			get { return _ExpirationDate; }
			set { _ExpirationDate = value; }
		}

		[EntityImportExportAttribute("EXPIRATIONDATE", 110, "ExpirationDateString")]
		public string ExpirationDateString
		{
			get { return ExpirationDate.ToString(); }
			set { ExpirationDate.Value = DateTimeOffset.Parse(value, ExpirationDate.Format); }
		}

		[EntityImportExportAttribute("NUMBER", 110, "Number")]
		public string Number
		{
			get { return _Number; }
			set { SetString("Number", 50, value, ref _Number); }
		}

		[EntityImportExportAttribute("INSTRUCTOR", 110, "Instructor")]
		public string Instructor
		{
			get { return _Instructor; }
			set { SetString("Instructor", 50, value, ref _Instructor); }
		}

		[EntityImportExportAttribute("RATING", 110, "Rating")]
		public string Rating
		{
			get { return _Rating; }
			set { SetString("Rating", 20, value, ref _Rating); }
		}

		[EntityImportExportAttribute("ID*", 105, "ID")]
		new public string ID { get { return _ID; } set { _ID = value; } }

		public bool HistoricalRecord
		{
			get { return _HistoricalRecord; }
			set { _HistoricalRecord = value; }
		}

		public int Reoccurrence
		{
			get { return this._Reoccurrence; }
			set { this._Reoccurrence = value; }
		}

		public override ENTITY_TYPE EntityType
		{
			get
			{
				switch (Type)
				{
					case QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY:
						return ENTITY_TYPE.QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT;
					case QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT:
						return ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION;
					case QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT:
						return ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE;
					case QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON:
						return ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION;
					case QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON:
						return ENTITY_TYPE.QUALIFICATION_PERSON_LICENSE;
					case QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON:
						return ENTITY_TYPE.QUALIFICATION_PERSON_TRAINING;
					case QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE:
						return ENTITY_TYPE.QUALIFICATION_MAP_PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE;
					case QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_EQUIPMENT_TYPE:
						return ENTITY_TYPE.QUALIFICATION_MAP_PERSON_TRAINING_TO_EQUIPMENT_TYPE;
					case QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_STATION:
						return ENTITY_TYPE.QUALIFICATION_MAP_PERSON_QUALIFICATION_TO_STATION;
					case QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_STATION:
						return ENTITY_TYPE.QUALIFICATION_MAP_PERSON_TRAINING_TO_STATION;
					case QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_STATION:
						return ENTITY_TYPE.QUALIFICATION_MAP_EQUIPMENT_TEST_AND_INSPECTION_TO_STATION;
                    case QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_STATION:
                        return ENTITY_TYPE.QUALIFICATION_MAP_EQUIPMENT_TAG_AND_LICENSE_TO_STATION;
                    default:
						return ENTITY_TYPE.QUALIFICATION_MAP;
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

		#region SQL Command constants
		const string PARAM_NAME_SEQUENCE = "@Sequence";
		const SqlDbType PARAM_TYPE_SEQUENCE = SqlDbType.Int;
		const string PARAM_NAME_INSTRUCTOR = "@Instructor";
		const SqlDbType PARAM_TYPE_INSTRUCTOR = SqlDbType.NVarChar;
		const int PARAM_SIZE_INSTRUCTOR = 50;
		const string PARAM_NAME_DATECOMPLETED = "@DateCompleted";
		const SqlDbType PARAM_TYPE_DATECOMPLETED = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_DATEDUE = "@DateDue";
		const SqlDbType PARAM_TYPE_DATEDUE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_EXPIRATIONDATE = "@ExpirationDate";
		const SqlDbType PARAM_TYPE_EXPIRATIONDATE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_ID = "@ID";
		const SqlDbType PARAM_TYPE_ID = SqlDbType.VarChar;
		const int PARAM_SIZE_ID = 50;
		const string PARAM_NAME_UPDATEDDATE = "@UpdatedDate";
		const SqlDbType PARAM_TYPE_UPDATEDDATE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_UPDATEDBY = "@UpdatedBy";
		const SqlDbType PARAM_TYPE_UPDATEDBY = SqlDbType.NVarChar;
		const int PARAM_SIZE_UPDATEDBY = 100;
		const string PARAM_NAME_RATING = "@Rating";
		const SqlDbType PARAM_TYPE_RATING = SqlDbType.NVarChar;
		const int PARAM_SIZE_RATING = 20;
		const string PARAM_NAME_HISTORICALRECORD = "@HistoricalRecord";
		const SqlDbType PARAM_TYPE_HISTORICALRECORD = SqlDbType.Bit;
		const string PARAM_NAME_PRIMARYKEYGUID = "@PrimaryKeyGuid";
		const SqlDbType PARAM_TYPE_PRIMARYKEYGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_ASSIGNEEGUID = "@AssigneeGuid";
		const SqlDbType PARAM_TYPE_ASSIGNEEGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_ASSIGNEDGUID = "@AssignedGuid";
		const SqlDbType PARAM_TYPE_ASSIGNEDGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_CREATEDDATE = "@CreatedDate";
		const SqlDbType PARAM_TYPE_CREATEDDATE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_CREATEDBY = "@CreatedBy";
		const SqlDbType PARAM_TYPE_CREATEDBY = SqlDbType.NVarChar;
		const string PARAM_NAME_SITEGUID = "@SiteGuid";
		const SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;
		const int PARAM_SIZE_CREATEDBY = 100;
		#endregion

		/// <summary>
		/// Return true if this is a type of qualification map that supports retention of historical records
		/// </summary>
		/// <param name="qualificationMapType">The type of qualification map to check</param>
		/// <returns>True if the type provided is a type that supports historical records</returns>
		public static bool IsHistoricalRecordType(QUALIFICATION_MAP_TYPE qualificationMapType)
		{
			return qualificationMapType == QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON
				   || qualificationMapType == QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;
		}

		public SqlCommand InsertSQL_
		{
			get
			{
				var cmd = new SqlCommand();



				cmd.CommandText =
					"INSERT INTO " + MapTableName(Type) +
						"(" +
						MapTablePrimaryKeyColumnName(Type) + "," +
						AssigneeGuidColumnName(Type) + "," +
						AssignedGuidColumnName(Type) + "," +
						(SupportsSiteGuid(Type) ? "SiteGuid," : "") +
						"Sequence," +
						"Instructor," +
						"DateCompleted," +
						"DateDue," +
						"ExpirationDate," +
						"ID," +
						"CreatedDate," +
						"CreatedBy," +
						"UpdatedDate," +
						"UpdatedBy," +
						"Rating," +
						"HistoricalRecord" +
					") VALUES (" +
					DataObject.AddParameter(cmd, string.Empty, PARAM_NAME_PRIMARYKEYGUID, PARAM_TYPE_PRIMARYKEYGUID, IdentityGuid) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_ASSIGNEEGUID, PARAM_TYPE_ASSIGNEEGUID, AssigneeGuid) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_ASSIGNEDGUID, PARAM_TYPE_ASSIGNEDGUID, AssignedGuid) +
					(SupportsSiteGuid(Type) ? DataObject.AddParameter(cmd, ",", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, SiteGuid) : "") +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_SEQUENCE, PARAM_TYPE_SEQUENCE, Sequence) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_INSTRUCTOR, PARAM_TYPE_INSTRUCTOR, PARAM_SIZE_INSTRUCTOR, _Instructor) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_DATECOMPLETED, PARAM_TYPE_DATECOMPLETED, DateCompleted.Value) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_DATEDUE, PARAM_TYPE_DATEDUE, DateDue.Value) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_EXPIRATIONDATE, PARAM_TYPE_EXPIRATIONDATE, ExpirationDate.Value) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_ID, PARAM_TYPE_ID, PARAM_SIZE_ID, Number) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_CREATEDDATE, PARAM_TYPE_CREATEDDATE, _CreatedDate) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_CREATEDBY, PARAM_TYPE_CREATEDBY, PARAM_SIZE_CREATEDBY, _CreatedBy) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_UPDATEDDATE, PARAM_TYPE_UPDATEDDATE, _UpdatedDate) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_UPDATEDBY, PARAM_TYPE_UPDATEDBY, PARAM_SIZE_UPDATEDBY, _UpdatedBy) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_RATING, PARAM_TYPE_RATING, PARAM_SIZE_RATING, _Rating) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_HISTORICALRECORD, PARAM_TYPE_HISTORICALRECORD, _HistoricalRecord) +
					")";

				return cmd;
			}
		}

		public SqlCommand UpdateSQL
		{
			// we only update non historical records so check the value at false or null
			get
			{
				var cmd = new SqlCommand();

				cmd.CommandText = "UPDATE " + MapTableName(Type) +
						" SET " +
						DataObject.AddParameter(cmd, false, AssigneeGuidColumnName(Type), PARAM_NAME_ASSIGNEEGUID, PARAM_TYPE_ASSIGNEEGUID, AssigneeGuid) + "," +
						DataObject.AddParameter(cmd, false, AssignedGuidColumnName(Type), PARAM_NAME_ASSIGNEDGUID, PARAM_TYPE_ASSIGNEDGUID, AssignedGuid) + "," +
						(SupportsSiteGuid(Type) ? (DataObject.AddParameter(cmd, false, "SiteGuid", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, SiteGuid) + ",") : "") +
						DataObject.AddParameter(cmd, false, "Sequence", PARAM_NAME_SEQUENCE, PARAM_TYPE_SEQUENCE, Sequence) + "," +
						DataObject.AddParameter(cmd, false, "Instructor", PARAM_NAME_INSTRUCTOR, PARAM_TYPE_INSTRUCTOR, PARAM_SIZE_INSTRUCTOR, _Instructor) + "," +
						DataObject.AddParameter(cmd, false, "DateCompleted", PARAM_NAME_DATECOMPLETED, PARAM_TYPE_DATECOMPLETED, DateCompleted.Value) + "," +
						DataObject.AddParameter(cmd, false, "DateDue", PARAM_NAME_DATEDUE, PARAM_TYPE_DATEDUE, DateDue.Value) + "," +
						DataObject.AddParameter(cmd, false, "ExpirationDate", PARAM_NAME_EXPIRATIONDATE, PARAM_TYPE_EXPIRATIONDATE, ExpirationDate.Value) + "," +
						DataObject.AddParameter(cmd, false, "ID", PARAM_NAME_ID, PARAM_TYPE_ID, PARAM_SIZE_ID, Number) + "," +
						DataObject.AddParameter(cmd, false, "UpdatedDate", PARAM_NAME_UPDATEDDATE, PARAM_TYPE_UPDATEDDATE, _UpdatedDate) + "," +
						DataObject.AddParameter(cmd, false, "UpdatedBy", PARAM_NAME_UPDATEDBY, PARAM_TYPE_UPDATEDBY, PARAM_SIZE_UPDATEDBY, _UpdatedBy) + "," +
						DataObject.AddParameter(cmd, false, "Rating", PARAM_NAME_RATING, PARAM_TYPE_RATING, PARAM_SIZE_RATING, _Rating) + "," +
						DataObject.AddParameter(cmd, false, "HistoricalRecord", PARAM_NAME_HISTORICALRECORD, PARAM_TYPE_HISTORICALRECORD, _HistoricalRecord) +
						" WHERE " +
						this.AddWherePrimaryKeyGuid(cmd, false);
						 //AddWhereAssigneeGuid(cmd, false) +
						 //AddWhereAssignedGuid(cmd, true) +
						 //AddWhereHistoricalRecord(cmd, false);

				return cmd;
			}
		}

		public SqlCommand PurgeSQL
		{
			get
			{
				var cmd = new SqlCommand();

				string sql = "DELETE FROM " + MapTableName(Type) + " WHERE " + this.AddWherePrimaryKeyGuid(cmd, false);
				//AddWhereAssigneeGuid(cmd, false) +
				//AddWhereAssignedGuid(cmd, true) +
				//AddWhereHistoricalRecord(cmd, includeHistoricalRecords);

				cmd.CommandText = sql;

				return cmd;
			}
		}


		public SqlCommand EnumerateByGuidAndTypeSQL
		{
			get
			{
				var cmd = new SqlCommand();

				cmd.CommandText = SQLSelectClause +
						GetFromClause(Type, string.Empty) + "WHERE " +
						AddWhereAssigneeGuid(cmd, false) +
						AddWhereSiteGuid(cmd, true, "MapTable.") +
						AddWhereHistoricalRecord(cmd, false) +
						SQLOrderByClause;

				return cmd;
			}
		}

		public SqlCommand EnumerateWhereQualificationTrainingIsUsedSQL
		{
			get
			{
				var qmTypes = new[] 
					{
						QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON,
						QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE,
						QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_STATION,
						QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON,
						QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_EQUIPMENT_TYPE,
						QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_STATION
					};

				var cmd = new SqlCommand();
				string sql = string.Empty;

				for (int idx = 0; idx < qmTypes.Length; idx++)
				{
					QUALIFICATION_MAP_TYPE currentType = qmTypes[idx];
					string tableAlias = "Tbl" + idx;
					sql += (idx == 0 ? string.Empty : " UNION ") +
						string.Format("(SELECT {0}.Sequence, {0}.Instructor, {0}.DateCompleted,{0}.DateDue, {0}.ExpirationDate, {0}.ID, " +
											"{0}.CreatedDate, {0}.CreatedBy, {0}.UpdatedDate, {0}.UpdatedBy, {0}.Rating, {0}.HistoricalRecord, " +
											"{0}.{2} AS AssignedGuid, {0}.{3} AS AssigneeGuid " +
											"FROM  {1} {0} WHERE {0}.{2} = @AssignedGuid AND (HistoricalRecord = @HistoricalRecord OR HistoricalRecord IS NULL))",
											tableAlias, MapTableName(currentType), AssignedGuidColumnName(currentType), AssigneeGuidColumnName(currentType));
				}
				DataObject.AddParameter(cmd, "@AssignedGuid", SqlDbType.UniqueIdentifier, AssignedGuid);
				DataObject.AddParameter(cmd, "@HistoricalRecord", SqlDbType.Bit, false);

				cmd.CommandText = "SELECT *, " + SQLSelectClause2 + " FROM (" + sql + ") AllTables INNER JOIN tblQualifications QTable ON QTable.QualificationGuid = AllTables.AssignedGuid" +
						SQLOrderByClause;
				return cmd;
			}
		}

		public SqlCommand EnumerateHistoricalRecordsByIndexAndTypeSQL
		{
			get
			{
				var cmd = new SqlCommand();

				cmd.CommandText = SQLSelectClause +
						GetFromClause(Type, string.Empty) + "WHERE " +
						AddWhereAssigneeGuid(cmd, false, "MapTable.") +
						AddWhereSiteGuid(cmd, true, "MapTable.") +
						AddWhereHistoricalRecord(cmd, true) +
						SQLOrderByClause;

				return cmd;
			}
		}

		public SqlCommand EnumerateByAssignedGuidSQL
		{
			get
			{
				var cmd = new SqlCommand();

				cmd.CommandText = SQLSelectClause +
						GetFromClause(Type, string.Empty) + "WHERE " +
						AddWhereAssignedGuid(cmd, false, "MapTable.") +
						AddWhereSiteGuid(cmd, true, "MapTable.") +
						AddWhereHistoricalRecord(cmd, false) +
						SQLOrderByClause;

				return cmd;
			}
		}
		#endregion

		#region Private static members
		// Three kinds of keys.
	   private const string PersonnelQualificationWarningKey = "Personnel Qualification Impending Expiration";
	   private const string PersonnelLicenseWarningKey = "Personnel License Impending Expiration";
	   private const string EquipmentTagOrLicenseWarningKey = "Equipment Tag/License Impending Expiration";
	   private const string EquipmentTestOrInspectionWarningKey = "Equipment Test/Inspection Impending Expiration";
	   private const string CompanyCertificateOrPermitWarningKey = "Company Certificate/Permit Impending Expiration";
	   private const string PersonnelQualificationExpiredKey = "Personnel Qualification Expired";
	   private const string PersonnelLicenseExpiredKey = "Personnel License Expired";
	   private const string EquipmentTagOrLicenseExpiredKey = "Equipment Tag/License Expired";
	   private const string EquipmentTestOrInspectionExpiredKey = "Equipment Test/Inspection Expired";
	   private const string CompanyCertificateOrPermitExpiredKey = "Company Certificate/Permit Expired";
	   private const string PersonnelQualificationMissingKey = "Personnel Qualification Missing";
	   private const string PersonnelTrainingExpiredKey = "Personnel Training Expired";
	   private const string PersonnelTrainingWarningKey = "Personnel Training Impending Expiration";
	   private const string StationTestOrInspectionExpiredKey = "Station Test/Inspection Expired";
	   private const string StationEquipmentNotAuthorizedKey = "Station Equipment Not Authorized";
	   private const string StationDriverNotAuthorizedKey = "Station Driver Not Authorized";

	   const string SQLOrderByClause = " ORDER BY Sequence";
		const string SQLSelectClause = "SELECT MapTable.*, " + SQLSelectClause2;
		const string SQLSelectClause2 = " QTable.ID AS QualificationID, QTable.Reoccurrence as ReoccurrenceID ";

		static readonly Dictionary<QUALIFICATION_MAP_TYPE, MapDBInfoClass> QualificationDbInfo = GetQualificationDBInfo();
		#endregion

		#region Public Static Members
		// Three kinds of descriptors.
		public static AlarmAndEventDescriptorClass PersonnelQualificationWarningEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, PersonnelQualificationWarningKey);
		public static AlarmAndEventDescriptorClass PersonnelLicenseWarningEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, PersonnelLicenseWarningKey);
		public static AlarmAndEventDescriptorClass EquipmentTagOrLicenseWarningEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EquipmentTagOrLicenseWarningKey);
		public static AlarmAndEventDescriptorClass EquipmentTestOrInspectionWarningEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EquipmentTestOrInspectionWarningKey);
		public static AlarmAndEventDescriptorClass CompanyCertificatOrPermitWarningEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, CompanyCertificateOrPermitWarningKey);

		public static AlarmAndEventDescriptorClass PersonnelQualificationExpiredAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, PersonnelQualificationExpiredKey);
		public static AlarmAndEventDescriptorClass PersonnelLicenseExpiredAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, PersonnelLicenseExpiredKey);
		public static AlarmAndEventDescriptorClass EquipmentTagOrLicenseExpiredAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, EquipmentTagOrLicenseExpiredKey);
		public static AlarmAndEventDescriptorClass EquipmentTestOrInspectionExpiredAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, EquipmentTestOrInspectionExpiredKey);
		public static AlarmAndEventDescriptorClass CompanyCertificatOrPermitExpiredAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, CompanyCertificateOrPermitExpiredKey);

		public static AlarmAndEventDescriptorClass PersonnelQualificationMissingAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, PersonnelQualificationMissingKey);
		public static AlarmAndEventDescriptorClass PersonnelTrainingExpiredAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, PersonnelTrainingExpiredKey);
		public static AlarmAndEventDescriptorClass PersonnelTrainingWarningEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, PersonnelTrainingWarningKey);

		public static AlarmAndEventDescriptorClass StationTestOrInspectionExpiredAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, StationTestOrInspectionExpiredKey);
		public static AlarmAndEventDescriptorClass StationEquipmentNotAuthorizedAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, StationEquipmentNotAuthorizedKey);
		public static AlarmAndEventDescriptorClass StationDriverNotAuthorizedAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, StationDriverNotAuthorizedKey);
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Qualification Map class.
		/// </summary>
		public QualificationMapClass()
		{
			ExpirationDate = new Date();
			DateCompleted = new Date();
			DateDue = new Date();
			Reset();
		}

		/// <summary>
		/// This constructor will initialize the qualification map class based 
		/// on site information.
		/// </summary>
		/// <param name="Site"></param>
		public QualificationMapClass(SiteClass Site)
		{
			ExpirationDate = new Date(Site);
			DateCompleted = new Date(Site);
			DateDue = new Date(Site);
			Reset();
		}
		#endregion

		#region Alarm and event descriptors.
		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] Descriptors ={	PersonnelQualificationWarningEventDescriptor,
																		PersonnelLicenseWarningEventDescriptor,
																		EquipmentTagOrLicenseWarningEventDescriptor,
																		EquipmentTestOrInspectionWarningEventDescriptor,
																		CompanyCertificatOrPermitWarningEventDescriptor,
																		PersonnelQualificationExpiredAlarmDescriptor,
																		PersonnelLicenseExpiredAlarmDescriptor,
																		EquipmentTagOrLicenseExpiredAlarmDescriptor,
																		EquipmentTestOrInspectionExpiredAlarmDescriptor,
																		CompanyCertificatOrPermitExpiredAlarmDescriptor,
																		PersonnelQualificationMissingAlarmDescriptor,
																		PersonnelTrainingExpiredAlarmDescriptor,
																		PersonnelTrainingWarningEventDescriptor,
																		StationTestOrInspectionExpiredAlarmDescriptor,
																		StationEquipmentNotAuthorizedAlarmDescriptor,
																		StationDriverNotAuthorizedAlarmDescriptor,
																	};
				return Descriptors;
			}
		}
		#endregion


		#region Public Methods
		// Warning events.
		public AlarmAndEventLogClass PersonnelQualificationWarningEvent(string PersonID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PersonnelQualificationWarningEventDescriptor);
			AlarmAndEventLog.AssociatedData = PersonID + ", " + ID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass PersonnelLicenseWarningEvent(string PersonID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PersonnelLicenseWarningEventDescriptor);
			AlarmAndEventLog.AssociatedData = PersonID + ", " + ID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass EquipmentTagOrLicenseWarningEvent(string equipmentID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(EquipmentTagOrLicenseWarningEventDescriptor)
		                                                 {
		                                                     AssociatedData = equipmentID + ", " + this.ID
		                                                 };
		    return alarmAndEventLog;
		}

        /// <summary>
        /// Creates an event object for recording an about-to-expire equipment tag or license.  This overload allows for driver and
        /// reporting station to be recorded
        /// </summary>
        /// <param name="equipmentId">
        /// The equipment id.
        /// </param>
        /// <param name="driverId">
        /// The driver id.
        /// </param>
        /// <param name="stationId">
        /// The station id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containg the event data.
        /// </returns>
        public AlarmAndEventLogClass EquipmentTagOrLicenseWarningEvent(string equipmentId, string driverId, string stationId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentTagOrLicenseWarningEventDescriptor)
            {
                AssociatedData = equipmentId + ", " + this.ID + " - Driver " + driverId + " - " + stationId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EquipmentTestOrInspectionWarningEvent(string equipmentID)
		{
            AlarmAndEventLogClass alarmAndEventLog =
                new AlarmAndEventLogClass(EquipmentTestOrInspectionWarningEventDescriptor)
                    {
                        AssociatedData = equipmentID + ", " + this.ID
                    };
            return alarmAndEventLog;
		}

        /// <summary>
        /// The equipment test or inspection warning event.
        /// </summary>
        /// <param name="equipmentId">
        /// The equipment id.
        /// </param>
        /// <param name="driverId">
        /// The driver id.
        /// </param>
        /// <param name="stationId">
        /// The station id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containg the event data.
        /// </returns>
        public AlarmAndEventLogClass EquipmentTestOrInspectionWarningEvent(string equipmentId, string driverId, string stationId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentTestOrInspectionWarningEventDescriptor)
            {
                AssociatedData = equipmentId + ", " + this.ID + " - Driver " + driverId + " - " + stationId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass CompanyCertificateOrPermitWarningEvent(string companyID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(CompanyCertificatOrPermitWarningEventDescriptor)
		                                                 {
		                                                     AssociatedData = companyID + ", " + this.ID
		                                                 };
		    return alarmAndEventLog;
		}

		// Expired alarms.
		public AlarmAndEventLogClass PersonnelQualificationExpiredAlarm(string personID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(PersonnelQualificationExpiredAlarmDescriptor)
		                                                 {
		                                                     AssociatedData = personID + ", " + this.ID
		                                                 };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass PersonnelLicenseExpiredAlarm(string PersonID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PersonnelLicenseExpiredAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = PersonID + ", " + ID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass EquipmentTagOrLicenseExpiredAlarm(string equipmentID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(EquipmentTagOrLicenseExpiredAlarmDescriptor)
		                                                 {
		                                                     AssociatedData = equipmentID + ", " + this.ID
		                                                 };
		    return alarmAndEventLog;
		}

        /// <summary>
        /// Creates an alarm object for recording an expired equipment tag or license.  This overload allows for driver and
        /// reporting station to be recorded
        /// </summary>
        /// <param name="equipmentID">
        /// The equipment id.
        /// </param>
        /// <param name="driverId">
        /// The driver id.
        /// </param>
        /// <param name="stationId">
        /// The station id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the alarm data.
        /// </returns>
        public AlarmAndEventLogClass EquipmentTagOrLicenseExpiredAlarm(string equipmentID, string driverId, string stationId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentTagOrLicenseExpiredAlarmDescriptor)
            {
                AssociatedData
                                               =
                                               equipmentID
                                               + ", "
                                               + this.ID + " - Driver "
                                               + driverId + " - "
                                               + stationId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EquipmentTestOrInspectionExpiredAlarm(string equipmentID)
		{
            AlarmAndEventLogClass alarmAndEventLog =
                new AlarmAndEventLogClass(EquipmentTestOrInspectionExpiredAlarmDescriptor)
                    {
                        AssociatedData = equipmentID + ", " + this.ID
                    };
            return alarmAndEventLog;
		}

        /// <summary>
        /// Creates an alarm object for recording an expired equipment test or inspection.  This overload allows for driver and
        /// reporting station to be recorded
        /// </summary>
        /// <param name="equipmentId">
        /// The equipment id.
        /// </param>
        /// <param name="driverId">
        /// The driver id.
        /// </param>
        /// <param name="stationId">
        /// The station id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the alarm data.
        /// </returns>
        public AlarmAndEventLogClass EquipmentTestOrInspectionExpiredAlarm(string equipmentId, string driverId, string stationId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentTestOrInspectionExpiredAlarmDescriptor)
            {
                AssociatedData = equipmentId + ", " + this.ID + " - Driver " + driverId + " - " + stationId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass CompanyCertificateOrPermitExpiredAlarm(string CompanyID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(CompanyCertificatOrPermitExpiredAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = CompanyID + ", " + ID;
			return AlarmAndEventLog;
		}

		// Missing alarms.
		public AlarmAndEventLogClass PersonnelQualificationMissingAlarm(string PersonID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PersonnelQualificationMissingAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = PersonID + ", " + ID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass PersonnelTrainingExpiredAlarm(string PersonID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PersonnelTrainingExpiredAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = PersonID + ", " + ID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass PersonnelTrainingWarningEvent(string PersonID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PersonnelTrainingWarningEventDescriptor);
			AlarmAndEventLog.AssociatedData = PersonID + ", " + ID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass StationTestOrInspectionExpiredAlarm(string StationID, string EquipmentID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(StationTestOrInspectionExpiredAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = StationID + ", " + EquipmentID + ", " + ID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass StationEquipmentNotAuthorizedAlarm(string StationID, string EquipmentID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(StationEquipmentNotAuthorizedAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = StationID + ", " + EquipmentID + ", " + ID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass StationDriverNotAuthorizedAlarm(string StationID, string DriverID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(StationDriverNotAuthorizedAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = StationID + ", " + DriverID + ", " + ID;
			return AlarmAndEventLog;
		}

		private static void CheckDBInfoExists(QUALIFICATION_MAP_TYPE mapType)
		{
			if (!QualificationDbInfo.ContainsKey(mapType))
			{
				throw new NotImplementedException(" Please update _QualificationDBInfo by updating the GetQualificationDBInfo method in QualificationMapClass");
			}
		}

		public static string MapTableName(QUALIFICATION_MAP_TYPE mapType)
		{
			CheckDBInfoExists(mapType);
			return QualificationDbInfo[mapType].MapTableName;
		}

		public static string AssignedGuidColumnName(QUALIFICATION_MAP_TYPE mapType)
		{
			CheckDBInfoExists(mapType);
			return QualificationDbInfo[mapType].AssignedGuidColumnName;
		}

		public static string AssigneeGuidColumnName(QUALIFICATION_MAP_TYPE mapType)
		{
			CheckDBInfoExists(mapType);
			return QualificationDbInfo[mapType].AssigneeGuidColumnName;
		}

		public static string AssigneeTableName(QUALIFICATION_MAP_TYPE mapType)
		{
			CheckDBInfoExists(mapType);
			return QualificationDbInfo[mapType].AssigneeTableName;
		}

		public static string MapTablePrimaryKeyColumnName(QUALIFICATION_MAP_TYPE mapType)
		{
			CheckDBInfoExists(mapType);
			return QualificationDbInfo[mapType].MapTablePrimaryKeyColumnName;
		}

		public static bool SupportsSiteGuid(QUALIFICATION_MAP_TYPE mapType)
		{
			CheckDBInfoExists(mapType);
			return QualificationDbInfo[mapType].SupportsSiteGuid;
		}

		public override void Load(Object o)
		{
			this.Reset();

			if (o is DataSet)
			{
				var set = (DataSet)o;
				DataTable table = set.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				_IdentityGuid = DataObject.getValue(row[MapTablePrimaryKeyColumnName(Type)], Guid.Empty);
				AssigneeGuid = DataObject.getValue(row[AssigneeGuidColumnName(Type)], Guid.Empty);
				AssignedGuid = DataObject.getValue(row[AssignedGuidColumnName(Type)], Guid.Empty);
				Sequence = DataObject.getValue(row["Sequence"], 0);
				_Instructor = DataObject.getValue(row["Instructor"], "");
				DateCompleted.Value = DataObject.getValue(row["DateCompleted"], TimeConverter.Today(DateCompleted.StandardName));
				DateDue.Value = DataObject.getValue(row["DateDue"], TimeConverter.Today(DateDue.StandardName));
				ExpirationDate.Value = DataObject.getValue(row["ExpirationDate"], TimeConverter.Today(ExpirationDate.StandardName));
				Number = DataObject.getValue(row["ID"], "");
				_CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue(row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue(row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue(row["UpdatedBy"], ADMIN);
				_Rating = DataObject.getValue(row["Rating"], "");
				_HistoricalRecord = DataObject.getValue(row["HistoricalRecord"], false);
				_ID = DataObject.getValue(row["QualificationID"], "");
				_Reoccurrence = DataObject.getValue(row["ReoccurrenceID"], 0);
				
				if (SupportsSiteGuid(Type))
				{
					_SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
				}
			}

			else if (typeof(XmlNode).IsInstanceOfType(o))
			{
				XmlNode Node = (XmlNode)o;

				if (Node.Name == "CertificateAndPermit")
					Type = QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY;

				else if (Node.Name == "TagAndLicense")
					Type = QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT;

				else if (Node.Name == "TestAndInspection")
				{
					if (Node.ParentNode.ParentNode.Name == "Equipment")
						Type = QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT;
					else if (Node.ParentNode.ParentNode.Name == "Station")
						Type = QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_STATION;
					else
						throw new Exception("Invalid Qualification Type");
				}

				else if (Node.Name == "License")
					Type = QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON;

				else if (Node.Name == "Qualification")
				{
					if (Node.ParentNode.ParentNode.Name == "Person")
						Type = QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON;
					else if (Node.ParentNode.ParentNode.Name == "Equipment Type")
						Type = QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE;
					else
						throw new Exception("Invalid Qualification Type");
				}
				else if (Node.Name == "Training")
				{
					if (Node.ParentNode.ParentNode.Name == "Person")
						Type = QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;
					else if (Node.ParentNode.ParentNode.Name == "Equipment Type")
						Type = QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_EQUIPMENT_TYPE;
					else if (Node.ParentNode.ParentNode.Name == "Station")
						Type = QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_STATION;
					else
						throw new Exception("Invalid Qualification Type");
				}
				else
				{
					throw new Exception("Invalid Qualification Type");
				}

				_ID = Node.Attributes["ID"].Value;

				if (Node.Attributes["ExpirationDate"] != null)
				{
					SetDate("Expiration Date", Node.Attributes["ExpirationDate"].Value, ref _ExpirationDate);
				}
				if (Node.Attributes["Number"] != null)
				{
					Number = Node.Attributes["Number"].Value;
				}
			}
			else
			{
				throw new Exception("Load Error - Invalid Object Type : " + o.GetType().ToString());
			}
		}

		public override void Store(Object O)
		{
			if (O == null)
			{
				throw new ArgumentNullException("Object");
			}

			var node = O as XmlNode;
			if (node != null)
			{
				XmlAttribute attribute = node.OwnerDocument.CreateAttribute("ID");
				attribute.Value = ID;
				node.Attributes.Append(attribute);

				attribute = node.OwnerDocument.CreateAttribute("ExpirationDate");
				attribute.Value = ExpirationDate.ToString();
				node.Attributes.Append(attribute);

				attribute = node.OwnerDocument.CreateAttribute("Number");
				attribute.Value = Number;
				node.Attributes.Append(attribute);
			}
			else
			{
				throw new Exception("Store Error - Invalid Object Type : " + O.GetType().ToString());
			}
		}

		public SqlCommand SelectSQL(bool bInTransaction)
		{
			var cmd = new SqlCommand();

			cmd.CommandText = SQLSelectClause +
									GetFromClause(Type, SQLUpdateLock(bInTransaction)) +
									" WHERE " +
									AddWhereAssigneeGuid(cmd, false, "MapTable.") +
									AddWhereAssignedGuid(cmd, true, "MapTable.") +
									AddWhereSiteGuid(cmd, true, "MapTable.") +
									AddWhereHistoricalRecord(cmd, false);

			return cmd;
		}


		#endregion

		#region Internal methods
		public override void Reset()
		{
			base.Reset();
			AssignedGuid = Guid.Empty;
			AssigneeGuid = Guid.Empty;
			Sequence = 0;
			ExpirationDate.Value = TimeConverter.Today(ExpirationDate.StandardName);
			_Instructor = "";
			DateCompleted.Value = TimeConverter.Today(DateCompleted.StandardName);
			DateDue.Value = TimeConverter.Today(DateDue.StandardName);
			Number = "";
			_Rating = "";
			_HistoricalRecord = false;
		}

		private static Dictionary<QUALIFICATION_MAP_TYPE, MapDBInfoClass> GetQualificationDBInfo()
		{
			var dbList = new Dictionary<QUALIFICATION_MAP_TYPE, MapDBInfoClass>();
			dbList.Add(QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY,
							new MapDBInfoClass("Company Certificate & Permits", "tblQualificationCompanyCertificateAndPermitToCompany", "CompanyGuid", "tblCompanies"));
			dbList.Add(QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT,
							new MapDBInfoClass("Equipment Test & Inspections", "tblQualificationEquipmentTestAndInspectionToEquipment", "EquipmentGuid", "tblEquipment"));
			dbList.Add(QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT,
							new MapDBInfoClass("Equipment Tag & Licenses", "tblQualificationEquipmentTagAndLicenseToEquipment", "EquipmentGuid", "tblEquipment"));
			dbList.Add(QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON,
							new MapDBInfoClass("Personnel Qualification", "tblQualificationPersonQualificationToPerson", "PersonnelGuid", "tblPersonnel"));
			dbList.Add(QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON,
							new MapDBInfoClass("Personnel Licenses", "tblQualificationPersonLicenseToPerson", "PersonnelGuid", "tblPersonnel"));
			dbList.Add(QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON,
							new MapDBInfoClass("Personnel Training", "tblQualificationPersonTrainingToPerson", "PersonnelGuid", "tblPersonnel"));
			dbList.Add(QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE,
							new MapDBInfoClass("Equipment Type Personnel Qualification", "tblQualificationPersonQualificationToEquipmentType", "EquipmentTypeGuid", "tblEquipmentTypes", true));
			dbList.Add(QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_EQUIPMENT_TYPE,
							new MapDBInfoClass("Equipment Type Personnel Training", "tblQualificationPersonTrainingToEquipmentType", "EquipmentTypeGuid", "tblEquipmentTypes", true));
            dbList.Add(QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_STATION,
                            new MapDBInfoClass("Station Personnel Qualification", "tblQualificationPersonQualificationToStation", "StationGuid", "tblStations"));
            dbList.Add(QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_STATION,
                            new MapDBInfoClass("Station Personnel Training", "tblQualificationPersonTrainingToStation", "StationGuid", "tblStations"));
            dbList.Add(QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_STATION,
                            new MapDBInfoClass("Station Personnel Licenses", "tblQualificationPersonLicenseToStation", "StationGuid", "tblStations"));
            dbList.Add(QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_STATION,
							new MapDBInfoClass("Station Equipmet Test & Inspection", "tblQualificationEquipmentTestAndInspectionToStation", "StationGuid", "tblStations"));
            dbList.Add(QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_STATION,
                            new MapDBInfoClass("Station Equipmet Licenses", "tblQualificationEquipmentTagAndLicenseToStation", "StationGuid", "tblStations"));

            foreach (MapDBInfoClass mapInfo in dbList.Values)
			{
				mapInfo.AssignedGuidColumnName = "QualificationGuid";
			}
			return dbList;
		}

		private string AddWhereAssigneeGuid(SqlCommand cmd, bool prefixWithAnd, string mapTableAlias = "")
		{
			return DataObject.AddParameter(cmd, prefixWithAnd, mapTableAlias + AssigneeGuidColumnName(Type), "@WhereAssigneeGuid", SqlDbType.UniqueIdentifier, AssigneeGuid);
		}

		private string AddWhereAssignedGuid(SqlCommand cmd, bool prefixWithAnd, string mapTableAlias = "")
		{
			return DataObject.AddParameter(cmd, prefixWithAnd, mapTableAlias + AssignedGuidColumnName(Type), "@WhereAssignedGuid", SqlDbType.UniqueIdentifier, AssignedGuid);
		}

		private string AddWherePrimaryKeyGuid(SqlCommand cmd, bool prefixWithAnd, string mapTableAlias = "")
		{
			return DataObject.AddParameter(cmd, prefixWithAnd, mapTableAlias + MapTablePrimaryKeyColumnName(Type), "@PrimaryKeyGuid", SqlDbType.UniqueIdentifier, IdentityGuid);
		}

		private string AddWhereSiteGuid(SqlCommand cmd, bool prefixWithAnd, string mapTableAlias = "")
		{
			if (SupportsSiteGuid(Type))
			{
				return DataObject.AddParameter(cmd, prefixWithAnd, mapTableAlias + "SiteGuid", "@SiteGUid", SqlDbType.UniqueIdentifier, SiteGuid);
			}
			else
			{
				string returnStr = " 1 = 1";
				if (prefixWithAnd)
				{
					returnStr = " AND " + returnStr;
				}

				return returnStr;
			}

		}


		private string AddWhereHistoricalRecord(SqlCommand cmd, bool isHistoricalRecord)
		{
			const string PARAM_NAME_HISTORICALRECORD_WHERE = "@WhereHistoricalRecord";
			const SqlDbType PARAM_TYPE_HISTORICALRECORD = SqlDbType.Bit;


			string sql;

			if (isHistoricalRecord)
			{
				sql = DataObject.AddParameter(cmd, true, "HistoricalRecord", PARAM_NAME_HISTORICALRECORD_WHERE, PARAM_TYPE_HISTORICALRECORD, true);
			}
			else
			{
				sql = " AND (" +
							DataObject.AddParameter(cmd, false, "HistoricalRecord", PARAM_NAME_HISTORICALRECORD_WHERE, PARAM_TYPE_HISTORICALRECORD, false) +
						"OR HistoricalRecord IS NULL)";
			}
			return sql;
		}

		private string GetFromClause(QUALIFICATION_MAP_TYPE qmType, string lockHint)
		{
			return string.Format(
					 " FROM {0} MapTable " + lockHint +
					 " INNER JOIN tblQualifications QTable " + lockHint +
					 " ON MapTable.{1} = QTable.QualificationGuid ",
					 MapTableName(qmType), AssignedGuidColumnName(qmType)
					 );

		}
		#endregion
	}
}
