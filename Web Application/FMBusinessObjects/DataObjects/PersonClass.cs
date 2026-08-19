using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
    [Serializable]
	[CollectionDataContract]
	[KnownType(typeof(PersonClass))]
	public class PersonCollectionClass : List<PersonClass> { }

	[KnownType(typeof(GregorianCalendar))]
	[Serializable]
	[DataContract]
	[EntityImportExportWorksheetAttribute("PERSONNEL")]
	[QueryWriterTopic(typeof(PersonClass), "Personnel")]
	[QueryWriterTopicSecurity(RIGHT.VIEW_PERSONNEL_DATA)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_PERSONNEL_DATA)]
	public class PersonClass : FMBaseDataObjectWithUserData, IComparable, IAlarmAndEventDiscovery
	{
		#region Public data members

        /// <summary>
        /// The default text to display in the PIN Number text box on the personnel form
        /// This value is checked when modifying a personnel record to see if the PIN Number was
        /// changed by the user.
        /// </summary>
        public const string MaskedPasswordText = "****";

		public const string ENTITY_TYPE_ID = "Personnel";
		public enum STATUS { In, Out, STB };

		[DataMember]
		public Guid MasterRecordGuid;

		[DataMember]
		public Guid AssignedToSiteGuid;
		[DataMember]
		public Guid AssignedFromSiteGuid;
		[DataMember]
		public string AssignedFromSiteId;

        [DataMember]
        public Guid MasterSiteGuid;

        [DataMember]
		public Guid UserGuid;

		[DataMember]
		public Guid SupervisorGuid;

		[DataMember]
		public Guid CompanyGuid;

		[DataMember]
		public Date _LockedOutDate;					// Excluded from PropertyMap

		[DataMember]
		public DateAndTime _LastActivityDate;		// Modified as DATA_TYPE.CONFIG, excluded from PropertyMap        

		[DataMember]
		public Guid AssignedEquipmentGuid;

		[DataMember]
		public QualificationCollectionClass QualificationExportPayload;

		[EntityImportExportWorksheetAttribute("PERSONNEL ROLES")]
		[EntityImportExportAttribute("ID*", 110, "ID")]
		[DataMember]
		public PersonRoleMapCollectionClass RoleCollection;

		[EntityImportExportWorksheetAttribute("QUALIFICATIONS")]
		[EntityImportExportAttribute("ID*", 110, "ID", 1)]
		[EntityImportExportAttribute("EXPIRATIONDATE", 110, "ExpirationDateString", 2)]
		[EntityImportExportAttribute("NUMBER", 110, "Number", 3)]
		[DataMember]
		public QualificationMapCollectionClass QualificationCollection;

		[EntityImportExportWorksheetAttribute("LICENSES")]
		[EntityImportExportAttribute("ID*", 110, "ID", 1)]
		[EntityImportExportAttribute("EXPIRATIONDATE", 110, "ExpirationDateString", 2)]
		[EntityImportExportAttribute("NUMBER", 110, "Number", 3)]
		[DataMember]
		public QualificationMapCollectionClass LicenseCollection;

		[EntityImportExportWorksheetAttribute("PERSON ACCESS SCHEDULE")]
		[EntityImportExportAttribute("TYPE", 100, "Type", 2)]
		[EntityImportExportAttribute("DAY", 110, "DayText", 3)]
		[EntityImportExportAttribute("ENABLED", 110, "Enabled", 4)]
		[EntityImportExportAttribute("OPENINGTIME", 110, "OpeningTimeString", 5)]
		[EntityImportExportAttribute("CLOSINGTIME", 110, "ClosingTimeString", 6)]
		[DataMember]
		public ScheduleCollectionClass AccessScheduleCollection;

		[EntityImportExportWorksheetAttribute("TRAINING")]
		[EntityImportExportAttribute("ID*", 110, "ID", 1)]
		[EntityImportExportAttribute("NUMBER", 110, "Number", 2)]
		[EntityImportExportAttribute("INSTRUCTOR", 110, "Instructor", 3)]
		[EntityImportExportAttribute("DATECOMPLETE", 110, "DateCompleted", 4)]
		[EntityImportExportAttribute("DATEDUE", 110, "DateDue", 5)]
		[EntityImportExportAttribute("EXPIRATIONDATE", 110, "ExpirationDateString", 6)]
		[EntityImportExportAttribute("RATING", 110, "Rating", 7)]
		[DataMember]
		public QualificationMapCollectionClass TrainingCollection;
        [DataMember]
        public CompanyMapCollectionClass AssignedCompaniesCollection;
		#endregion

		#region Protected data members
		protected DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;

		[DataMember]
		protected string _CardNumber;
		[DataMember]
		protected string _FirstName;
		[DataMember]
		protected string _MiddleName;
		[DataMember]
		protected string _LastName;
		[DataMember]
		protected string _Title;
		[DataMember]
		protected string _Department;
		[DataMember]
		protected string _Address1;
		[DataMember]
		protected string _Address2;
		[DataMember]
		protected string _City;
		[DataMember]
		protected string _State;
		[DataMember]
		protected string _Zip;
		[DataMember]
		protected string _Country;
		[DataMember]
		protected string _Phone1;
		[DataMember]
		protected string _Phone2;
		[DataMember]
		protected Date _AssignmentDate;
		[DataMember]
		protected Date _SupervisionDate;
		[DataMember]
		protected string _SSAN;
		[DataMember]
		protected Date _BirthDate;
		[DataMember]
		protected Decimal _PayRate;
		[DataMember]
		protected double _LaborRate1;
		[DataMember]
		protected double _LaborRate2;
		[DataMember]
		protected double _LaborRate3;
		[DataMember]
		protected double _LaborRate4;
		[DataMember]
		protected STATUS _Status;
		[DataMember]
		protected string _Email;
		[DataMember]
		protected bool _ResponsibleOfficer;
		[DataMember]
		protected short _Shift;
		[DataMember]
		protected string _PINNumber;
		[DataMember]
		protected bool _PINRequired;
		[DataMember]
		protected bool _LockedOut;
		[DataMember]
		protected string _LockedOutReason;
		[DataMember]
		protected bool _CardedIn;
		[DataMember]
		protected string _ShortCardNumber;
		[DataMember]
		protected byte[] _OnFileSignature;
		[DataMember]
		protected string _AssignedEquipmentID;        
        // Linked Items
        [DataMember]
		protected string _CompanyID;
		[DataMember]
		protected string _CompanyName;
		[DataMember]
		protected string _CompanyAddress;
		[DataMember]
		protected string _CompanyCity;
		[DataMember]
		protected string _CompanyState;
		[DataMember]
		protected string _UserID;
		[DataMember]
		protected string _SupervisorID;
        [DataMember]
        protected int _AssignedCompaniesCount;
        [DataMember]
        protected bool _InhibitInactivityLockout;
        #endregion

        #region Properties
        [EntityImportExportAttribute("SITE*", 105, "SITEGUID")]
		new public Guid SiteGuid { get { return this._SiteGuid; } set {
		    this._SiteGuid = value; } }

		[QueryWriterField("ID", "tblPersonnel.PersonID")]
		[EntityImportExportAttribute("PERSONID*", 110, "ID")]
		public override string ID
		{
			get { return this._ID; }
			set {
			    this.SetString("ID", 50, value, ref this._ID); }
		}

		[QueryWriterField("Card Number", "tblPersonnel.CardNumber")]
		[EntityImportExportAttribute("CARDNUMBER", 100, "CardNumber")]
		public string CardNumber
		{
			get { return this._CardNumber; }
			set {
			    this.SetString("Card Number", 30, value, ref this._CardNumber); }
		}

		[QueryWriterField("First Name", "tblPersonnel.FirstName")]
		[EntityImportExportAttribute("FIRSTNAME", 90, "FirstName")]
		public string FirstName
		{
			get { return this._FirstName; }
			set {
			    this.SetString("First Name", 20, value, ref this._FirstName); }
		}

		[QueryWriterField("Middle Name", "tblPersonnel.MiddleName")]
		[EntityImportExportAttribute("MIDDLENAME", 90, "MiddleName")]
		public string MiddleName
		{
			get { return this._MiddleName; }
			set {
			    this.SetString("Middle Name", 20, value, ref this._MiddleName); }
		}

		[QueryWriterField("Last Name", "tblPersonnel.LastName")]
		[EntityImportExportAttribute("LASTNAME", 95, "LastName")]
		public string LastName
		{
			get { return this._LastName; }
			set {
			    this.SetString("Last Name", 30, value, ref this._LastName); }
		}

		public string FullName => $"{this._LastName},{this._FirstName}";

	    public string FirstLastName => $"{this._FirstName} {this._LastName}";

	    [QueryWriterField("Title", "tblPersonnel.Title")]
		[EntityImportExportAttribute("TITLE", 85, "Title")]
		public string Title
		{
			get { return this._Title; }
			set {
			    this.SetString("Title", 50, value, ref this._Title); }
		}

		[QueryWriterField("Department", "tblPersonnel.Department")]
		[EntityImportExportAttribute("DEPARTMENT", 115, "Department")]
		public string Department
		{
			get { return this._Department; }
			set {
			    this.SetString("Department", 50, value, ref this._Department); }
		}

		[QueryWriterField("Address 1", "tblPersonnel.Address1")]
		[EntityImportExportAttribute("ADDRESS1", 120, "Address1")]
		public string Address1
		{
			get { return this._Address1; }
			set {
			    this.SetString("Address1", 50, value, ref this._Address1); }
		}

		[QueryWriterField("Address 2", "tblPersonnel.Address2")]
		[EntityImportExportAttribute("ADDRESS2", 120, "Address2")]
		public string Address2
		{
			get { return this._Address2; }
			set {
			    this.SetString("Address2", 50, value, ref this._Address2); }
		}

		[QueryWriterField("City", "tblPersonnel.City")]
		[EntityImportExportAttribute("CITY", 95, "City")]
		public string City
		{
			get { return this._City; }
			set {
			    this.SetString("City", 60, value, ref this._City); }
		}

		[QueryWriterField("State", "tblPersonnel.State")]
		[EntityImportExportAttribute("STATE", 60, "State")]
		public string State
		{
			get { return this._State; }
			set {
			    this.SetString("State", 20, value, ref this._State); }
		}

		[QueryWriterField("Zip", "tblPersonnel.Zip")]
		[EntityImportExportAttribute("ZIP", 70, "Zip")]
		public string Zip
		{
			get { return this._Zip; }
			set {
			    this.SetString("Zip", 10, value, ref this._Zip); }
		}

		[QueryWriterField("Country", "tblPersonnel.Country")]
		[EntityImportExportAttribute("COUNTRY", 70, "Country")]
		public string Country
		{
			get { return this._Country; }
			set {
			    this.SetString("Country", 20, value, ref this._Country); }
		}

		[QueryWriterField("Phone 1", "tblPersonnel.Phone1")]
		[EntityImportExportAttribute("PHONE1", 70, "Phone1")]
		public string Phone1
		{
			get { return this._Phone1; }
			set {
			    this.SetString("Phone1", 20, value, ref this._Phone1); }
		}

		[QueryWriterField("Phone 2", "tblPersonnel.Phone2")]
		[EntityImportExportAttribute("PHONE2", 70, "Phone2")]
		public string Phone2
		{
			get { return this._Phone2; }
			set {
			    this.SetString("Phone2", 50, value, ref this._Phone2); }
		}

		[QueryWriterField("Assignment Date", "tblPersonnel.AssignmentDate")]
		public Date AssignmentDateObject
		{
			get { return this._AssignmentDate; }
			set { this._AssignmentDate = value; }
		}

		[EntityImportExportAttribute("ASSIGNMENTDATE", 110, "AssignmentDate")]
		public string AssignmentDate
		{
			get { return this._AssignmentDate.ToString(); }
			set {
			    this.SetDate("Assignment Date", value, ref this._AssignmentDate); }
		}

		[QueryWriterField("Supervision Date", "tblPersonnel.SupervisionDate")]
		public Date SupervisionDateObject
		{
			get { return this._SupervisionDate; }
			set { this._SupervisionDate = value; }
		}

		[EntityImportExportAttribute("SUPERVISIONDATE", 115, "SupervisionDate")]
		public string SupervisionDate
		{
			get { return this._SupervisionDate.ToString(); }
			set {
			    this.SetDate("Supervision Date", value, ref this._SupervisionDate); }
		}

		// By design, we do not expose this field for Query Writer.  This is a Personal Identity issue and should never
		// be exposed in the Query Writer.
		[EntityImportExportAttribute("SSAN", 70, "SSAN")]
		public string SSAN
		{
			get { return this._SSAN; }
			set {
			    this.SetString("SSAN", 11, value, ref this._SSAN); }
		}

		[QueryWriterField("Birth Date", "tblPersonnel.BirthDate")]
		public Date BirthDateObject
		{
			get { return this._BirthDate; }
			set { this._BirthDate = value; }
		}

		[EntityImportExportAttribute("BIRTHDATE", 80, "BirthDate")]
		public string BirthDate
		{
			get { return this._BirthDate.ToString(); }
			set {
			    this.SetDate("Birth Date", value, ref this._BirthDate); }
		}

		[EntityImportExportAttribute("PAYRATE", 80, "PayRate")]
		public Decimal PayRate
		{
			get { return this._PayRate; }
			set { this._PayRate = value; }
		}

		[EntityImportExportAttribute("LABORRATE1", 90, "LABORRATE1")]
		public double LaborRate1
		{
			get { return this._LaborRate1; }
			set { this._LaborRate1 = value; }
		}

		[EntityImportExportAttribute("LABORRATE2", 90, "LABORRATE2")]
		public double LaborRate2
		{
			get { return this._LaborRate2; }
			set { this._LaborRate2 = value; }
		}

		[EntityImportExportAttribute("LABORRATE3", 90, "LABORRATE3")]
		public double LaborRate3
		{
			get { return this._LaborRate3; }
			set { this._LaborRate3 = value; }
		}

		[EntityImportExportAttribute("LABORRATE4", 90, "LABORRATE4")]
		public double LaborRate4
		{
			get { return this._LaborRate4; }
			set { this._LaborRate4 = value; }
		}

		[QueryWriterField("Status", "tblPersonnel.Status")]
		[EntityImportExportAttribute("STATUS", 80, "STATUS")]
		public STATUS Status
		{
			get { return this._Status; }
			set { this._Status = value; }
		}

		public string StatusText
		{
			get { return Enum.GetName(typeof(STATUS), this._Status); }
			set { }
		}

		[QueryWriterField("Email", "tblPersonnel.Email")]
		[EntityImportExportAttribute("EMAIL", 120, "Email")]
		public string Email
		{
			get { return this._Email; }
			set {
			    this.SetString("Email", 50, value, ref this._Email); }
		}

		[QueryWriterField("Responsible Officer", "tblPersonnel.ResponsibleOfficer")]
		[EntityImportExportAttribute("RESPONSIBLEOFFICER", 135, "ResponsibleOfficer")]
		public bool ResponsibleOfficer
		{
			get { return this._ResponsibleOfficer; }
			set { this._ResponsibleOfficer = value; }
		}

		[QueryWriterField("Shift", "tblPersonnel.Shift")]
		[EntityImportExportAttribute("SHIFT", 60, "Shift")]
		public short Shift
		{
			get { return this._Shift; }
			set { this._Shift = value; }
		}

		[EntityImportExportAttribute("PINNUMBER", 80, "PINNumber")]
		public string PINNumber
		{
			get { return this._PINNumber; }
			set {
			    this.SetString("PIN Number", 4, value, ref this._PINNumber); }
		}

		[QueryWriterField("PIN Required", "tblPersonnel.PINRequired")]
		[EntityImportExportAttribute("PINREQUIRED", 90, "PINREQUIRED")]
		public bool PINRequired
		{
			get { return this._PINRequired; }
			set { this._PINRequired = value; }
		}

		[QueryWriterField("Locked Out", "tblPersonnel.LockedOut")]
		[EntityImportExportAttribute("LOCKEDOUT", 80, "LockedOut")]
		public bool LockedOut
		{
			get { return this._LockedOut; }
			set {
			    this._LockedOut = value; }
		}

		[QueryWriterField("Locked Out Reason", "tblPersonnel.LockedOutReason")]
		[EntityImportExportAttribute("LOCKEDOUTREASON", 120, "LockedOutReason")]
		public string LockedOutReason
		{
			get { return this._LockedOutReason; }
			set {
			    this.SetString("Locked Out Reason", 80, value, ref this._LockedOutReason); }
		}

		[QueryWriterField("Locked Out Date", "tblPersonnel.LockedOutDate")]
		public Date LockedOutDateObject
		{
			get { return this._LockedOutDate; }
			set { this._LockedOutDate = value; }
		}

		[EntityImportExportAttribute("LOCKEDOUTDATE", 105, "LockedOutDate")]
		public string LockedOutDate
		{
			get { return this._LockedOutDate.ToString(); }
			set {
			    this.SetDate("Locked Out Date", value, ref this._LockedOutDate); }
		}

		[QueryWriterField("Last Activity Date", "tblPersonnel.LastActivityDate")]
		public DateAndTime LastActivityDateObject
		{
			get { return this._LastActivityDate; }
			set { this._LastActivityDate = value; }
		}

		[EntityImportExportAttribute("LASTACTIVITYDATE", 115, "LastActivityDate")]
		public string LastActivityDate
		{
			get { return this._LastActivityDate.ToString(); }
			set {
			    this.SetDateAndTime("Last Activity Date", value, ref this._LastActivityDate); }
		}

		[QueryWriterField("Carded In", "tblPersonnel.CardedIn")]
		[EntityImportExportAttribute("CARDEDIN", 75, "CardedIn")]
		public bool CardedIn
		{
			get { return this._CardedIn; }
			set { this._CardedIn = value; }
		}

		[QueryWriterField("Assigned Equipment ID", "tblEquipment.ID", false)]
		[EntityImportExportAttribute("ASSIGNEDEQUIPMENTID", 125, "ASSIGNEDEQUIPMENTID")]
		public string AssignedEquipmentID
		{
			get { return this._AssignedEquipmentID; }
			set { this._AssignedEquipmentID = value; }
		}

		[QueryWriterField("Company ID", "C.ID")]
		[EntityImportExportAttribute("COMPANYID", 90, "CompanyID")]
		public string CompanyID
		{
			get { return this._CompanyID; }
			set { this._CompanyID = value; }
		}

		[QueryWriterField("Company Name", "C.Name")]
		[EntityImportExportAttribute("COMPANYNAME", 110, "CompanyName")]
		public string CompanyName
		{
			get { return this._CompanyName; }
			set { this._CompanyName = value; }
		}

		[QueryWriterField("Company Address", "C.Address1")]
		[EntityImportExportAttribute("COMPANYADDRESS", 120, "CompanyAddress")]
		public string CompanyAddress
		{
			get { return this._CompanyAddress; }
			set { this._CompanyAddress = value; }
		}

		[QueryWriterField("Company City", "C.City")]
		[EntityImportExportAttribute("COMPANYCITY", 90, "CompanyCity")]
		public string CompanyCity
		{
			get { return this._CompanyCity; }
			set { this._CompanyCity = value; }
		}

		[QueryWriterField("Company State", "C.State")]
		[EntityImportExportAttribute("COMPANYSTATE", 100, "COMPANYSTATE")]
		public string CompanyState
		{
			get { return this._CompanyState; }
			set { this._CompanyState = value; }
		}

		[QueryWriterField("UserID", "tblUsers.UserID", false)]
		[EntityImportExportAttribute("USERID", 70, "UserID")]
		public string UserID
		{
			get { return this._UserID; }
			set { this._UserID = value; }
		}

		[QueryWriterField("Supervisor ID", "Supervisors.PersonID", false)]
		[EntityImportExportAttribute("SUPERVISORID", 120, "SupervisorID")]
		public string SupervisorID
		{
			get { return this._SupervisorID; }
			set { this._SupervisorID = value; }
		}
        [QueryWriterField("Assigned Companies Count", "Drivers.AssignedCompaniesCount", false)]
        [EntityImportExportAttribute("ASSIGNEDCOMPANIESCOUNT", 120, "AssignedCompaniesCount")]
        public int AssignedCompaniesCount => this._AssignedCompaniesCount;

	    [QueryWriterField("Short Card Number", "tblPersonnel.ShortCardNumber")]
		[EntityImportExportAttribute("SHORTCARDNUMBER", 120, "ShortCardNumber")]
		public string ShortCardNumber
		{
			get { return this._ShortCardNumber; }
			set {
			    this.SetString("Short Card Number", 30, value, ref this._ShortCardNumber); }
		}

        /// <summary>
        /// Represents the date + time that this personnel record was hidden
        /// A null value indicates the personnel record is not hidden.
        /// Although this field is stored as a datetime it is represented to users
        /// as a checkbox. 
        /// </summary>
        [DataMember]
        public DateTimeOffset? HiddenDate { get; set; }

        /// <summary>
        /// This property is here to support entity import + export of the hidden date.
        /// The Entity import + export functionality doesn't play nice with nullable DateTimeOffsets
        /// </summary>
        [EntityImportExportAttribute("HIDDENDATE", 70, "HIDDENDATE")]
        public string HiddenDateAsString
        {
            get
            {
                if (this.HiddenDate.HasValue)
                {
                    return this.HiddenDate.Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.HiddenDate = null;
                }
                else
                {
                    this.HiddenDate = DateTimeOffset.Parse(value);
                }
            }
        }

		[QueryWriterField("User Data 1", "tblPersonnel.UserData1")]
		public string UserData1 => this.UserData[0];

	    [QueryWriterField("User Data 2", "tblPersonnel.UserData2")]
		public string UserData2 => this.UserData[1];

	    [QueryWriterField("User Data 3", "tblPersonnel.UserData3")]
		public string UserData3 => this.UserData[2];

	    [QueryWriterField("User Data 4", "tblPersonnel.UserData4")]
		public string UserData4 => this.UserData[3];

	    [QueryWriterField("User Data 5", "tblPersonnel.UserData5")]
		public string UserData5 => this.UserData[4];

	    [QueryWriterField("User Data 6", "tblPersonnel.UserData6")]
		public string UserData6 => this.UserData[5];

	    [QueryWriterField("User Data 7", "tblPersonnel.UserData7")]
		public string UserData7 => this.UserData[6];

	    [QueryWriterField("User Data 8", "tblPersonnel.UserData8")]
		public string UserData8 => this.UserData[7];

	    [QueryWriterField("User Data 9", "tblPersonnel.UserData9")]
		public string UserData9 => this.UserData[8];

	    [QueryWriterField("User Data 10", "tblPersonnel.UserData10")]
		public string UserData10 => this.UserData[9];

	    [QueryWriterField("User Data 11", "tblPersonnel.UserData11")]
		public string UserData11 => this.UserData[10];

	    [QueryWriterField("User Data 12", "tblPersonnel.UserData12")]
		public string UserData12 => this.UserData[11];

	    [QueryWriterField("User Data 13", "tblPersonnel.UserData13")]
		public string UserData13 => this.UserData[12];

	    [QueryWriterField("User Data 14", "tblPersonnel.UserData14")]
		public string UserData14 => this.UserData[13];

	    [QueryWriterField("User Data 15", "tblPersonnel.UserData15")]
		public string UserData15 => this.UserData[14];

	    [QueryWriterField("User Data 16", "tblPersonnel.UserData16")]
		public string UserData16 => this.UserData[15];

	    [QueryWriterField("User Data 17", "tblPersonnel.UserData17")]
		public string UserData17 => this.UserData[16];

	    [QueryWriterField("User Data 18", "tblPersonnel.UserData18")]
		public string UserData18 => this.UserData[17];

	    [QueryWriterField("User Data 19", "tblPersonnel.UserData19")]
		public string UserData19 => this.UserData[18];

	    [QueryWriterField("User Data 20", "tblPersonnel.UserData20")]
		public string UserData20 => this.UserData[19];

	    [QueryWriterField("User Data 21", "tblPersonnel.UserData21")]
		public string UserData21 => this.UserData[20];

	    [QueryWriterField("User Data 22", "tblPersonnel.UserData22")]
		public string UserData22 => this.UserData[21];

	    [QueryWriterField("User Data 23", "tblPersonnel.UserData23")]
		public string UserData23 => this.UserData[22];

	    [QueryWriterField("User Data 24", "tblPersonnel.UserData24")]
		public string UserData24 => this.UserData[23];
        
		public byte[] OnFileSignature
		{
			get { return this._OnFileSignature; }
			set { this._OnFileSignature = value; }
		}

        public bool InhibitInactivityLockout { get { return _InhibitInactivityLockout; } set { _InhibitInactivityLockout = value; } }

        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{

				AlarmAndEventDescriptorClass[] descriptors ={	DriverAccessTimedOutAlarmDescriptor,
																		PersonnelLockOutEventDescriptor,
																		DriverLockedOutAlarmDescriptor,
																		CardInEventDescriptor,
																		CardUseSuccessfulEventDescriptor,
																		CardOutEventDescriptor,
																		MultipleCardInAlarmDescriptor,
																		DriverAccessScheduleAlarmDescriptor,
                                                                        CardPresentedEventDescriptor,
                                                                        DriverLoggedInDescriptor,
                                                                        HaveLoaderRoleDescriptor,
                                                                        HaveOffloaderRoleDescriptor,
                                                                        NotCardedInDescriptor,
                                                                        CardedInExceededAllowedCardInPeriodDescriptor,
                                                                        LoadTransAliasInvalidDescriptor,
                                                                        OffLoadTransAliasInvalidDescriptor,
                                                                        CardInWebAppEventDescriptor,
                                                                        CardOutWebAppEventDescriptor
                                                                    };
				return descriptors;
			}
		}

		public AlarmAndEventLogClass DriverAccessTimedOutAlarm
		{
			get
			{
			    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(DriverAccessTimedOutAlarmDescriptor)
			                                                 {
			                                                     AssociatedData = this.ID
			                                                 };
			    return alarmAndEventLog;
			}
		}


		public AlarmAndEventLogClass LockOutEvent
		{
			get
			{
			    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(PersonnelLockOutEventDescriptor)
			                                             {
			                                                 AssociatedData = this.ID
			                                             };
			    return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass DriverLockedOutAlarm(string CardOrDriveID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(DriverLockedOutAlarmDescriptor);
            alarmAndEventLog.AssociatedData = this.ID + " - " + CardOrDriveID;

            return alarmAndEventLog;
        }

		public AlarmAndEventLogClass MultipleCardInAlarm
		{
			get
			{
			    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(MultipleCardInAlarmDescriptor)
			                                             {
			                                                 AssociatedData = this.ID
			                                             };
			    return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass AccessScheduleAlarm
		{
			get
			{
			    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(DriverAccessScheduleAlarmDescriptor)
			                                             {
			                                                 AssociatedData = this.ID
			                                             };
			    return alarmAndEventLog;
			}
		}

        public AlarmAndEventLogClass CardPresentedEvent(string StationID)
        {
            AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(CardPresentedEventDescriptor);
            AlarmAndEventLog.AssociatedData = StationID + " - " + ID;
            return AlarmAndEventLog;
        }

        public AlarmAndEventLogClass DriverLoggedInEvent(string StationID)
        {
            AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(DriverLoggedInDescriptor);
            AlarmAndEventLog.AssociatedData = StationID + " - " + ID;
            return AlarmAndEventLog;
        }

        public AlarmAndEventLogClass HaveLoaderRoleEvent(string StationID)
        {
            AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(HaveLoaderRoleDescriptor);
            AlarmAndEventLog.AssociatedData = StationID + " - " + ID;
            return AlarmAndEventLog;
        }

        public AlarmAndEventLogClass HaveOffloaderRoleEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(HaveOffloaderRoleDescriptor)
            {
                AssociatedData =
                                               stationID + " - " + this.ID + " " + this.FirstLastName
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass NotCardedInEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(NotCardedInDescriptor)
            {
                AssociatedData =
                                               stationID + " - " + this.ID + " " + this.FirstLastName
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass CardedInExceededAllowedCardInPeriodEvent()
        {
            AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(CardedInExceededAllowedCardInPeriodDescriptor);
            AlarmAndEventLog.AssociatedData = ID;
            return AlarmAndEventLog;
        }

        public AlarmAndEventLogClass LoadTransAliasInvalidEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(LoadTransAliasInvalidDescriptor)
            {
                AssociatedData =
                                               stationID + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass OffLoadTransAliasInvalidEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OffLoadTransAliasInvalidDescriptor)
            {
                AssociatedData =
                                               stationID + " - "
                                               + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass CardInWebAppEvent(string UserID)
        {
            AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(CardInWebAppEventDescriptor);
            AlarmAndEventLog.AssociatedData = UserID + " - " + ID;
            return AlarmAndEventLog;
        }

        public AlarmAndEventLogClass CardOutWebAppEvent(string UserID)
        {
            AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(CardOutWebAppEventDescriptor);
            AlarmAndEventLog.AssociatedData = UserID + " - " + ID;
            return AlarmAndEventLog;
        }

        public override ENTITY_TYPE EntityType => ENTITY_TYPE.PERSONNEL;

	    public string PersonToolTip
		{
			get
			{
				string toolTip = "";
				if (this._FirstName != "")
					toolTip = this._FirstName;
				if (this._MiddleName != "")
					toolTip += " " + this._MiddleName;
				if (this._LastName != "")
					toolTip += " " + this._LastName;

				if (toolTip == "")
					toolTip = this._ID;

				return toolTip;
			}
		}

		public string CompanyToolTip
		{
			get
			{
				string toolTip;
				if (this._CompanyName != "")
					toolTip = this._CompanyName;
				else
					toolTip = this._CompanyID;
				if (this._CompanyAddress != "")
					toolTip += ", " + this._CompanyAddress;
				if (this._CompanyCity != "")
					toolTip += ", " + this._CompanyCity;
				if (this._CompanyState != "")
					toolTip += ", " + this._CompanyState;
				return toolTip;
			}
		}

		public SqlCommand InsertSqlCommand
		{
			get
			{
			    SqlCommand command = new SqlCommand();

				var sql = "INSERT INTO tblPersonnel " +
				             "(PersonnelGuid," +
				             "_MasterRecordGuid," +
				             "SiteGuid," +
				             "PersonID," +
				             "CardNumber," +
				             "UserGuid," +
				             "FirstName," +
				             "MiddleName," +
				             "LastName," +
				             "Title," +
				             "Department," +
				             "SupervisorPersonnelGuid," +
				             "Address1," +
				             "Address2," +
				             "City," +
				             "State," +
				             "Zip," +
				             "Country," +
				             "Phone1," +
				             "Phone2," +
				             "AssignmentDate," +
				             "SupervisionDate," +
				             "SSAN," +
				             "BirthDate," +
				             "PayRate," +
				             "LaborRate1," +
				             "LaborRate2," +
				             "LaborRate3," +
				             "LaborRate4," +
				             "Status," +
				             "Email," +
				             "ResponsibleOfficer," +
				             "Shift," +
				             "CompanyGuid," +
				             "PINNumber," +
				             "PINRequired," +
				             "LockedOut," +
				             "LockedOutReason," +
				             "LockedOutDate," +
				             "LastActivityDate," +
				             "CardedIn," +
				             "ShortCardNumber," +
				             "HiddenDate," + 
				             "AssignedEquipmentGuid," +
				             "CreatedDate," +
				             "CreatedBy," +
				             "UpdatedDate," +
				             "UpdatedBy," +
				             "OnFileSignature," +
				             "UserData1," +
				             "UserData2," +
				             "UserData3," +
				             "UserData4," +
				             "UserData5," +
				             "UserData6," +
				             "UserData7," +
				             "UserData8," +
				             "UserData9," +
				             "UserData10," +
				             "UserData11," +
				             "UserData12," +
				             "UserData13," +
				             "UserData14," +
				             "UserData15," +
				             "UserData16," +
				             "UserData17," +
				             "UserData18," +
				             "UserData19," +
				             "UserData20," +
				             "UserData21," +
				             "UserData22," +
				             "UserData23," +
                             "UserData24," +
                             "InhibitInactivityLockout" +
				             ") " +
				             "VALUES (" +
				             "@PersonnelGuid," +
				             "@MasterRecordGuid," +
				             "@SiteGuid, " +
				             "@PersonId, " +
				             "@CardNumber, " +
				             "@UserGuid, " +
				             "@FirstName, " +
				             "@MiddleName, " +
				             "@LastName, " +
				             "@Title, " +
				             "@Department, " +
				             "@SupervisorPersonnelGuid, " +
				             "@Address1, " +
				             "@Address2, " +
				             "@City, " +
				             "@State, " +
				             "@Zip, " +
				             "@Country, " +
				             "@Phone1, " +
				             "@Phone2, " +
				             "@AssignmentDate, " +
				             "@SupervisionDate, " +
				             "@SSAN, " +
				             "@Birthdate, " +
				             "@PayRate, " +
				             "@LaborRate1, " +
				             "@LaborRate2, " +
				             "@LaborRate3, " +
				             "@LaborRate4, " +
				             "@Status, " +
				             "@Email, " +
				             "@ResponsibleOfficer, " +
				             "@Shift, " +
				             "@CompanyGuid, " +
				             "@PINNumber, " +
				             "@PINRequired, " +
				             "@LockedOut, " +
				             "@LockedOutReason, " +
				             "@LockedOutDate, " +
				             "@LastActivityDate, " +
				             "@CardedIn, " +
				             "@ShortCardNumber, " +
				             "@HiddenDate," + 
				             "@AssignedEquipmentGuid, " +                      
				             "@CreatedDate, " +
				             "@CreatedBy, " +
				             "@UpdatedDate, " +
				             "@UpdatedBy, " +
				             "@OnFileSignature, " +
				             "@UserData1, " +
				             "@UserData2, " +
				             "@UserData3, " +
				             "@UserData4, " +
				             "@UserData5, " +
				             "@UserData6, " +
				             "@UserData7, " +
				             "@UserData8, " +
				             "@UserData9, " +
				             "@UserData10," +
				             "@UserData11," +
				             "@UserData12," +
				             "@UserData13," +
				             "@UserData14," +
				             "@UserData15," +
				             "@UserData16," +
				             "@UserData17," +
				             "@UserData18," +
				             "@UserData19," +
				             "@UserData20," +
				             "@UserData21," +
				             "@UserData22," +
				             "@UserData23," +
                             "@UserData24," +
                             "@InhibitInactivityLockout)";
				command.CommandText = sql;
				command.CommandType = CommandType.Text;

			    this._IdentityGuid = Guid.NewGuid();
				this.MasterRecordGuid = this._IdentityGuid;
				command.Parameters.Add(DataObject.NewGuidParameter("@PersonnelGuid", this._IdentityGuid));
				command.Parameters.Add(DataObject.NewGuidParameter("@MasterRecordGuid", this.MasterRecordGuid));
				command.Parameters.Add(DataObject.NewGuidParameter("@SiteGuid", this._SiteGuid));
				var newParameter = command.Parameters.Add("@PersonID", SqlDbType.NVarChar, 50);
				newParameter.Value = this._ID;
				newParameter = command.Parameters.Add("@CardNumber", SqlDbType.NVarChar, 30);
				if (string.IsNullOrEmpty(this._CardNumber))
				{
					newParameter.Value = DBNull.Value;
				}
				else
				{
					newParameter.Value = this._CardNumber;
				}
				command.Parameters.Add(DataObject.NewGuidParameter("@UserGuid", this.UserGuid, true)); // true means set to NULL if empty

				newParameter = command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 20);
				newParameter.Value = this._FirstName;
				newParameter = command.Parameters.Add("@MiddleName", SqlDbType.NVarChar, 20);
				newParameter.Value = this._MiddleName;
				newParameter = command.Parameters.Add("@LastName", SqlDbType.NVarChar, 30);
				newParameter.Value = this._LastName;
				newParameter = command.Parameters.Add("@Title", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Title;
				newParameter = command.Parameters.Add("@Department", SqlDbType.NVarChar, 20);
				newParameter.Value = this._Department;
				command.Parameters.Add(DataObject.NewGuidParameter("@SupervisorPersonnelGuid", this.SupervisorGuid, true)); // true means set to NULL if empty

				newParameter = command.Parameters.Add("@Address1", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Address1;
				newParameter = command.Parameters.Add("@Address2", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Address2;
				newParameter = command.Parameters.Add("@City", SqlDbType.NVarChar, 60);
				newParameter.Value = this._City;
				newParameter = command.Parameters.Add("@State", SqlDbType.NVarChar, 20);
				newParameter.Value = this._State;
				newParameter = command.Parameters.Add("@Zip", SqlDbType.NVarChar, 10);
				newParameter.Value = this._Zip;
				newParameter = command.Parameters.Add("@Country", SqlDbType.NVarChar, 20);
				newParameter.Value = this._Country;
				newParameter = command.Parameters.Add("@Phone1", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Phone1;
				newParameter = command.Parameters.Add("@Phone2", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Phone2;
				newParameter = command.Parameters.Add("@AssignmentDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._AssignmentDate.Value;
				newParameter = command.Parameters.Add("@SupervisionDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._SupervisionDate.Value;
				newParameter = command.Parameters.Add("@SSAN", SqlDbType.NVarChar, 11);
				newParameter.Value = this._SSAN;
				newParameter = command.Parameters.Add("@Birthdate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._BirthDate.Value;
				newParameter = command.Parameters.Add("@PayRate", SqlDbType.Money);
				newParameter.Value = this._PayRate;
				newParameter = command.Parameters.Add("@LaborRate1", SqlDbType.Float);
				newParameter.Value = this._LaborRate1;
				newParameter = command.Parameters.Add("@LaborRate2", SqlDbType.Float);
				newParameter.Value = this._LaborRate2;
				newParameter = command.Parameters.Add("@LaborRate3", SqlDbType.Float);
				newParameter.Value = this._LaborRate3;
				newParameter = command.Parameters.Add("@LaborRate4", SqlDbType.Float);
				newParameter.Value = this._LaborRate4;
				newParameter = command.Parameters.Add("@Status", SqlDbType.SmallInt, 1);
				newParameter.Value = this._Status;
				newParameter = command.Parameters.Add("@Email", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Email;
				newParameter = command.Parameters.Add("@ResponsibleOfficer", SqlDbType.Bit);
				newParameter.Value = this._ResponsibleOfficer;
				newParameter = command.Parameters.Add("@Shift", SqlDbType.SmallInt);
				newParameter.Value = this._Shift;
				command.Parameters.Add(DataObject.NewGuidParameter("@CompanyGuid", this.CompanyGuid, true)); // true means set to NULL if empty
                newParameter = command.Parameters.Add("@PINNumber", SqlDbType.VarBinary, 256);
                newParameter.Value = !string.IsNullOrEmpty(this._PINNumber)
                        ? (object)UserClass.encode(this._PINNumber, this._SiteGuid)
                        : DBNull.Value;

				newParameter = command.Parameters.Add("@PINRequired", SqlDbType.Bit);
				newParameter.Value = this._PINRequired;
				newParameter = command.Parameters.Add("@LockedOut", SqlDbType.Bit);
				newParameter.Value = this.LockedOut;
				newParameter = command.Parameters.Add("@LockedOutReason", SqlDbType.NVarChar, 80);
				newParameter.Value = this.LockedOutReason;
				newParameter = command.Parameters.Add("@LockedOutDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._LockedOutDate.Value;
				command.Parameters.Add(DataObject.NewGuidParameter("@AssignedEquipmentGuid", this.AssignedEquipmentGuid, true)); // true means set to NULL if empty
				newParameter = command.Parameters.Add("@LastActivityDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._LastActivityDate.Value;
				newParameter = command.Parameters.Add("@CardedIn", SqlDbType.Bit);
				newParameter.Value = this._CardedIn;
				newParameter = command.Parameters.Add("@ShortCardNumber", SqlDbType.NVarChar, 6);
				if (string.IsNullOrEmpty(this._ShortCardNumber))
				{
					newParameter.Value = DBNull.Value;
				}
				else
				{
					newParameter.Value = this._ShortCardNumber;
				}

                command.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset).Value = this.HiddenDate ?? (object)DBNull.Value;

				newParameter = command.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._CreatedDate;
				newParameter = command.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
				newParameter.Value = this._CreatedBy;
				newParameter = command.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._UpdatedDate;
				newParameter = command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
				newParameter.Value = this._UpdatedBy;
				newParameter = command.Parameters.Add("@OnFileSignature", SqlDbType.VarBinary);
				if (this._OnFileSignature == null || this._OnFileSignature.Length == 0)
				{
					newParameter.Size = 0;
					newParameter.Value = DBNull.Value;
				}
				else
				{
					newParameter.Size = this._OnFileSignature.Length;
					newParameter.Value = this._OnFileSignature;
				}
				for (int i = 1; i <= 24; i++)
				{
					newParameter = command.Parameters.Add("@UserData" + i.ToString(), SqlDbType.NVarChar, 60);
					newParameter.Value = this.UserData[i - 1];
				}
                newParameter = command.Parameters.Add("@InhibitInactivityLockout", SqlDbType.Bit);
                newParameter.Value = this._InhibitInactivityLockout;
                return command;
			}
		}

		public SqlCommand PurgeSQL
		{
			get
			{
				const string ParamNamePersonguid = "@PersonGuid";
				const SqlDbType ParamTypePersonguid = SqlDbType.UniqueIdentifier;

				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "DELETE FROM tblPersonnel WHERE " +
										DataObject.AddParameter(cmd, false, "PersonnelGuid", ParamNamePersonguid, ParamTypePersonguid, this._IdentityGuid);

				return cmd;
			}
		}

		#endregion

		#region Private and static strings
		static string DriverAccessTimedOutKey = "Driver Access Timed Out";
		static string PersonnelLockOutKey = "Personnel Lock Out";
		static string DriverLockedOutKey = "Driver Locked Out";
		static string CardInKey = "Card-in Successful";
		static string CardUseSuccessfulKey = "Successful Card Use at a Station";
		static string CardOutKey = "Card-out Successful";
		static string MultipleCardInKey = "Multiple Card-in";
		static string DriverAccessScheduleKey = "Driver Access Schedule";
        static string CardPresentedKey = "Card Presented";
        static string DriverLoggedIn = "Driver Found";
        static string HaveLoaderRole = "Must Have Driver Role";
        static string HaveOffloaderRole = "Must Have Offloader Role";
        static string NotCardedIn = "Not Carded In at Entry Gate";
        static string CardedInExceededAllowedCardInPeriodKey = "Driver Forcibly Carded Out";
        static string LoadTransAliasInvalid = "Load Transaction Alias Invalid";
        static string OffLoadTransAliasInvalid = "Offload Transaction Alias Invalid";
        static readonly AlarmAndEventDescriptorClass DriverAccessTimedOutAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, DriverAccessTimedOutKey);
		static readonly AlarmAndEventDescriptorClass DriverLockedOutAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, DriverLockedOutKey);
		static readonly AlarmAndEventDescriptorClass CardInEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, CardInKey);
		static readonly AlarmAndEventDescriptorClass CardUseSuccessfulEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, CardUseSuccessfulKey);
		static readonly AlarmAndEventDescriptorClass CardOutEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, CardOutKey);
		static readonly AlarmAndEventDescriptorClass MultipleCardInAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, MultipleCardInKey);
		static readonly AlarmAndEventDescriptorClass DriverAccessScheduleAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, DriverAccessScheduleKey);
        static readonly AlarmAndEventDescriptorClass CardPresentedEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, CardPresentedKey);
        static readonly AlarmAndEventDescriptorClass DriverLoggedInDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, DriverLoggedIn);
        static readonly AlarmAndEventDescriptorClass HaveLoaderRoleDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, HaveLoaderRole);
        static readonly AlarmAndEventDescriptorClass HaveOffloaderRoleDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, HaveOffloaderRole);
        static readonly AlarmAndEventDescriptorClass NotCardedInDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, NotCardedIn);
        static readonly AlarmAndEventDescriptorClass CardedInExceededAllowedCardInPeriodDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, CardedInExceededAllowedCardInPeriodKey);
        static readonly AlarmAndEventDescriptorClass LoadTransAliasInvalidDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, LoadTransAliasInvalid);
        static readonly AlarmAndEventDescriptorClass OffLoadTransAliasInvalidDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, OffLoadTransAliasInvalid);

		static readonly AlarmAndEventDescriptorClass PersonnelLockOutEventDescriptor = new AlarmAndEventDescriptorClass(false, SystemKey, PersonnelLockOutKey);
        static readonly AlarmAndEventDescriptorClass CardInWebAppEventDescriptor = new AlarmAndEventDescriptorClass(false, WebApplicationKey, CardInKey);
        static readonly AlarmAndEventDescriptorClass CardOutWebAppEventDescriptor = new AlarmAndEventDescriptorClass(false, WebApplicationKey, CardOutKey);
        #endregion

        #region Compare method
        int IComparable.CompareTo(object obj)
		{
			PersonClass person = obj as PersonClass;
			if (person == null)
				throw new Exception("Invalid Person");
			return string.Compare(this.ID, person.ID, StringComparison.Ordinal);
		}
		#endregion


		#region Constructors
		/// <summary>
		/// This is the default constructor the Person Class.
		/// </summary>
		public PersonClass()
		{
			this._AssignmentDate = new Date();
			this._SupervisionDate = new Date();
			this._BirthDate = new Date();
			this._LockedOutDate = new Date();
			this._LastActivityDate = new DateAndTime();
			this.UserData = new UserDataClass();

			this.Reset();
		}

		/// <summary>
		/// This constructor initializes the person class based
		/// on site information.
		/// </summary>
		/// <param name="site"></param>
		public PersonClass(SiteClass site)
		{
			this.dateTimeFormatInfo = site.GetDateTimeFormatInfo();
			this._AssignmentDate = new Date(site);
			this._SupervisionDate = new Date(site);
			this._BirthDate = new Date(site);
			this._LockedOutDate = new Date(site);
			this._LastActivityDate = new DateAndTime(site);
			this.UserData = new UserDataClass();

			this.Reset();
		}
		#endregion

		#region Public methods

		public AlarmAndEventLogClass CardInEvent(string stationID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(CardInEventDescriptor)
		                                             {
		                                                 AssociatedData = this.ID + ", " + stationID
		                                             };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass CardUseSuccessfulEventEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(CardUseSuccessfulEventDescriptor)
			{
				AssociatedData = this.ID + ", " + stationID
			};
			return alarmAndEventLog;
		}

		public AlarmAndEventLogClass CardOutEvent(string stationID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(CardOutEventDescriptor)
		                                             {
		                                                 AssociatedData = this.ID + ", " + stationID
		                                             };
		    return alarmAndEventLog;
		}

		public bool HasRole(PERSON_ROLE role)
		{
			foreach (PersonRoleMapClass availableRole in this.RoleCollection)
			{
				if (availableRole.Role == role)
					return true;
			}
			return false;
		}

		public override void Load(object o)
		{
		    this.Reset();

		    var set = o as DataSet;
		    if (set != null)
			{
				DataTable table = set.Tables[0];
				if (table.Rows.Count == 0)
					return;

				DataRow row = table.Rows[0];

			    this._IdentityGuid = DataObject.getValue<Guid>(row["PersonnelGuid"], Guid.Empty);
			    this.MasterRecordGuid = DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty);
			    this._SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
                this.MasterSiteGuid = DataObject.getValue<Guid>(row["MasterSiteGuid"], Guid.Empty);
                this._ID = DataObject.getValue<string>(row["PersonID"], "");
			    this._CardNumber = DataObject.getValue<string>(row["CardNumber"], "");
			    this.UserGuid = DataObject.getValue<Guid>(row["UserGuid"], Guid.Empty);
			    this._FirstName = DataObject.getValue<string>(row["FirstName"], "");
			    this._MiddleName = DataObject.getValue<string>(row["MiddleName"], "");
			    this._LastName = DataObject.getValue<string>(row["LastName"], "");
			    this._Title = DataObject.getValue<string>(row["Title"], "");
			    this._Department = DataObject.getValue<string>(row["Department"], "");
			    this.SupervisorGuid = DataObject.getValue<Guid>(row["SupervisorPersonnelGuid"], Guid.Empty);
			    this._Address1 = DataObject.getValue<string>(row["Address1"], "");
			    this._Address2 = DataObject.getValue<string>(row["Address2"], "");
			    this._City = DataObject.getValue<string>(row["City"], "");
			    this._State = DataObject.getValue<string>(row["State"], "");
			    this._Zip = DataObject.getValue<string>(row["Zip"], "");
			    this._Country = DataObject.getValue<string>(row["Country"], "");
			    this._Phone1 = DataObject.getValue<string>(row["Phone1"], "");
			    this._Phone2 = DataObject.getValue<string>(row["Phone2"], "");
			    this._AssignmentDate.Value = DataObject.getValue<DateTimeOffset>(row["AssignmentDate"], TimeConverter.Today(this._AssignmentDate.StandardName));
			    this._SupervisionDate.Value = DataObject.getValue<DateTimeOffset>(row["SupervisionDate"], TimeConverter.Today(this._SupervisionDate.StandardName));
			    this._SSAN = DataObject.getValue<string>(row["SSAN"], "");
			    this._BirthDate.Value = DataObject.getValue<DateTimeOffset>(row["BirthDate"], TimeConverter.Today(this._BirthDate.StandardName));
			    this._PayRate = DataObject.getValue<Decimal>(row["PayRate"], new Decimal(0.0));
			    this._LaborRate1 = DataObject.getValue<double>(row["LaborRate1"], 0.0);
			    this._LaborRate2 = DataObject.getValue<double>(row["LaborRate2"], 0.0);
			    this._LaborRate3 = DataObject.getValue<double>(row["LaborRate3"], 0.0);
			    this._LaborRate4 = DataObject.getValue<double>(row["LaborRate4"], 0.0);
			    this._Status = (STATUS)DataObject.getValue<short>(row["Status"], 0);
			    this._Email = DataObject.getValue<string>(row["Email"], "");
			    this._ResponsibleOfficer = DataObject.getValue<bool>(row["ResponsibleOfficer"], false);
			    this._Shift = DataObject.getValue<short>(row["Shift"], 0);
			    this.CompanyGuid = Guid.Empty; // CompanyGuid no longer saved in database; companies are stored in AssociatedCompaniesCollection
			    this._PINNumber = DataObject.getValue(row["PINNumber"] == DBNull.Value ? string.Empty : UserClass.decode((byte[])row["PINNumber"], this.MasterSiteGuid), string.Empty);
			    this._PINRequired = DataObject.getValue<bool>(row["PINRequired"], true);
			    this._LockedOut = DataObject.getValue<bool>(row["LockedOut"], false);
			    this._LockedOutReason = DataObject.getValue<string>(row["LockedOutReason"], "");
			    this._LockedOutDate.Value = DataObject.getValue<DateTimeOffset>(row["LockedOutDate"], TimeConverter.Today(this._LockedOutDate.StandardName));
			    this._LastActivityDate.Value = DataObject.getValue<DateTimeOffset>(row["LastActivityDate"], TimeConverter.Now(this._LastActivityDate.StandardName));
			    this._CardedIn = DataObject.getValue<bool>(row["CardedIn"], false);
			    this._ShortCardNumber = DataObject.getValue<string>(row["ShortCardNumber"], "");
                this.HiddenDate = DataObject.getValue<DateTimeOffset?>(row["HiddenDate"], null);
			    this.AssignedEquipmentGuid = DataObject.getValue<Guid>(row["AssignedEquipmentGuid"], Guid.Empty);
			    this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			    this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			    this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			    this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			    this._CompanyID = "{Unassigned}";
			    this._CompanyName = "";
			    this._CompanyAddress = "";
			    this._CompanyCity = "";
			    this._CompanyState = "";
			    this._UserID = DataObject.getValue<string>(row["UserID"], "");
			    this._SupervisorID = DataObject.getValue<string>(row["SupervisorID"], "");

				if (table.Columns.Contains("OnFileSignature"))
				{
				    this._OnFileSignature = DataObject.getValue<byte[]>(row["OnFileSignature"], null);
				}
				else
				{
				    this._OnFileSignature = null;
				}
			    this._AssignedEquipmentID = DataObject.getValue<string>(row["AssignedEquipmentID"], "{Unassigned}");
			    this._AssignedCompaniesCount = (row.IsNull("AssignedCompaniesCount")) ? 0 : (int)row["AssignedCompaniesCount"];
			    this.UserData[0] = DataObject.getValue<string>(row["UserData1"], "");
			    this.UserData[1] = DataObject.getValue<string>(row["UserData2"], "");
			    this.UserData[2] = DataObject.getValue<string>(row["UserData3"], "");
			    this.UserData[3] = DataObject.getValue<string>(row["UserData4"], "");
			    this.UserData[4] = DataObject.getValue<string>(row["UserData5"], "");
			    this.UserData[5] = DataObject.getValue<string>(row["UserData6"], "");
			    this.UserData[6] = DataObject.getValue<string>(row["UserData7"], "");
			    this.UserData[7] = DataObject.getValue<string>(row["UserData8"], "");
			    this.UserData[8] = DataObject.getValue<string>(row["UserData9"], "");
			    this.UserData[9] = DataObject.getValue<string>(row["UserData10"], "");
			    this.UserData[10] = DataObject.getValue<string>(row["UserData11"], "");
			    this.UserData[11] = DataObject.getValue<string>(row["UserData12"], "");
			    this.UserData[12] = DataObject.getValue<string>(row["UserData13"], "");
			    this.UserData[13] = DataObject.getValue<string>(row["UserData14"], "");
			    this.UserData[14] = DataObject.getValue<string>(row["UserData15"], "");
			    this.UserData[15] = DataObject.getValue<string>(row["UserData16"], "");
			    this.UserData[16] = DataObject.getValue<string>(row["UserData17"], "");
			    this.UserData[17] = DataObject.getValue<string>(row["UserData18"], "");
			    this.UserData[18] = DataObject.getValue<string>(row["UserData19"], "");
			    this.UserData[19] = DataObject.getValue<string>(row["UserData20"], "");
			    this.UserData[20] = DataObject.getValue<string>(row["UserData21"], "");
			    this.UserData[21] = DataObject.getValue<string>(row["UserData22"], "");
			    this.UserData[22] = DataObject.getValue<string>(row["UserData23"], "");
			    this.UserData[23] = DataObject.getValue<string>(row["UserData24"], "");
                if (table.Columns.Contains("InhibitInactivityLockout"))
                {
                    this._InhibitInactivityLockout = DataObject.getValue<bool>(row["InhibitInactivityLockout"], false);
                }
                else
                {
                    this._InhibitInactivityLockout = false;
                }
                this.RowVersion = DataObject.getValue<byte[]>(row["_RowVersion"], null);

				if (table.Columns.IndexOf("ASSIGNEDTOSITEGUID") >= 0) this.AssignedToSiteGuid = DataObject.getValue<Guid>(row["ASSIGNEDTOSITEGUID"], Guid.Empty);
				if (table.Columns.IndexOf("ASSIGNEDFROMSITEGUID") >= 0) this.AssignedFromSiteGuid = DataObject.getValue<Guid>(row["ASSIGNEDFROMSITEGUID"], Guid.Empty);
				if (table.Columns.IndexOf("ASSIGNEDFROMSITEID") >= 0) this.AssignedFromSiteId = DataObject.getValue<string>(row["ASSIGNEDFROMSITEID"], "");
			}
			else
			{
				base.Load(o);

			    var personNode = o as XmlNode;
			    if (personNode != null)
				{
					foreach (XmlNode node in personNode)
					{
						if (node.Name == "Roles")
						{
							foreach (XmlNode roleNode in node)
							{
								PersonRoleMapClass role = new PersonRoleMapClass();
								role.Load(roleNode);
							    this.RoleCollection.Add(role);
							}
						}
						else if (node.Name == "Licenses")
						{
							int sequence = 0;
							foreach (XmlNode licenseNode in node)
							{
								QualificationMapClass license = new QualificationMapClass();
								license.Load(licenseNode);
								license.Sequence = sequence++;
							    this.LicenseCollection.Add(license);
							}
						}
						else if (node.Name == "Training")
						{
							int sequence = 0;
							foreach (XmlNode trainingNode in node)
							{
								QualificationMapClass training = new QualificationMapClass();
								training.Load(trainingNode);
								training.Sequence = sequence++;
							    this.TrainingCollection.Add(training);
							}
						}
						else if (node.Name == "Qualifications")
						{
							int sequence = 0;
							foreach (XmlNode qualificationNode in node)
							{
								QualificationMapClass qualification = new QualificationMapClass();
								qualification.Load(qualificationNode);
								qualification.Sequence = sequence++;
							    this.QualificationCollection.Add(qualification);
							}
						}
						else if (node.Name == "AccessSchedule")
						{
						    this.AccessScheduleCollection.Clear();
							foreach (XmlNode scheduleEntry in node)
							{
								ScheduleClass schedule = new ScheduleClass();
								schedule.Load(scheduleEntry);
							    this.AccessScheduleCollection.Add(schedule);
							}
						}
						else if (node.Name == "OnFileSignature")
						{
						}
                        else if (node.Name == "AssignedCompanies")
                        {
                            foreach (XmlNode companyNode in node)
                            {
                                CompanyMapClass assignedCompany = CompanyMapClass.CreateCompanyMap(companyNode);
                                this.AssignedCompaniesCollection.Add(assignedCompany);
                            }
                        }
					}
				}
			}
		}

		public override void Store(object o)
		{
		    var personNode = o as XmlNode;
		    if (personNode != null)
			{
				base.Store(personNode);

				XmlNode personRolesNode = personNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Roles", null);
			    if (personRolesNode != null)
			    {
			        personNode.AppendChild(personRolesNode);
			        foreach (PersonRoleMapClass role in this.RoleCollection)
			        {
			            XmlNode personRoleNode = personRolesNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Role", null);
			            role.Store(personRoleNode);
			            if (personRoleNode != null)
			            {
			                personRolesNode.AppendChild(personRoleNode);
			            }
			        }
			    }

                XmlNode qualificationsNode = personNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Qualifications", null);
			    if (qualificationsNode != null)
			    {
			        personNode.AppendChild(qualificationsNode);
			        foreach (QualificationMapClass qualification in this.QualificationCollection)
			        {
			            XmlNode qualificationNode = qualificationsNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Qualification", null);
			            qualification.Store(qualificationNode);
			            if (qualificationNode != null)
			            {
			                qualificationsNode.AppendChild(qualificationNode);
			            }
			        }
			    }

                XmlNode licensesNode = personNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Licenses", null);
			    if (licensesNode != null)
			    {
			        personNode.AppendChild(licensesNode);
			        foreach (QualificationMapClass license in this.LicenseCollection)
			        {
			            XmlNode licenseNode = qualificationsNode?.OwnerDocument?.CreateNode(XmlNodeType.Element, "License", null);
			            license.Store(licenseNode);
			            if (licenseNode != null)
			            {
			                licensesNode.AppendChild(licenseNode);
			            }
			        }
			    }

                XmlNode trainingNodes = personNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Training", null);
			    if (trainingNodes != null)
			    {
			        personNode.AppendChild(trainingNodes);
			        foreach (QualificationMapClass training in this.TrainingCollection)
			        {
			            XmlNode trainingNode = qualificationsNode?.OwnerDocument?.CreateNode(XmlNodeType.Element, "Training", null);
			            training.Store(trainingNode);
			            if (trainingNode != null)
			            {
			                trainingNodes.AppendChild(trainingNode);
			            }
			        }
			    }

                XmlNode assignedCompaniesNode = personNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AssignedCompanies", null);
			    if (assignedCompaniesNode != null)
			    {
			        personNode.AppendChild(assignedCompaniesNode);
			        foreach (CompanyMapClass carrier in this.AssignedCompaniesCollection)
			        {
			            XmlNode assignedCompanyNode = assignedCompaniesNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AssignedCompanies", null);
			            carrier.Store(assignedCompanyNode);
			            if (assignedCompanyNode != null)
			            {
			                assignedCompaniesNode.AppendChild(assignedCompanyNode);
			            }
			        }
			    }

                XmlNode accessScheduleNode = personNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AccessSchedule", null);
			    if (accessScheduleNode != null)
			    {
			        personNode.AppendChild(accessScheduleNode);
			        foreach (ScheduleClass schedule in this.AccessScheduleCollection)
			        {
			            XmlNode scheduleNode = accessScheduleNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AccessScheduleEntry", null);
			            schedule.Store(scheduleNode);
			            if (scheduleNode != null)
			            {
			                accessScheduleNode.AppendChild(scheduleNode);
			            }
			        }
			    }
			}
		}

	    public SqlCommand UpdateSqlCommand(DATA_TYPE type)
		{
			string sql;
			SqlCommand command = new SqlCommand();
			SqlParameter newParameter;

			if (type == DATA_TYPE.CONFIG)
			{
				sql = "UPDATE tblPersonnel " +
					"SET SiteGuid = @SiteGuid, " +
					"PersonID = @PersonID, " +
					"CardNumber = @CardNumber, " +
					"UserGuid = @UserGuid, " +
					"FirstName = @FirstName, " +
					"MiddleName = @MiddleName, " +
					"LastName = @LastName, " +
					"Title = @Title, " +
					"Department = @Department, " +
					"SupervisorPersonnelGuid = @SupervisorPersonnelGuid, " +
					"Address1 = @Address1, " +
					"Address2 = @Address2, " +
					"City = @City, " +
					"State = @State, " +
					"Zip = @Zip, " +
					"Country = @Country, " +
					"Phone1 = @Phone1, " +
					"Phone2 = @Phone2, " +
					"AssignmentDate = @AssignmentDate, " +
					"SupervisionDate = @SupervisionDate, " +
					"SSAN = @SSAN, " +
					"BirthDate = @BirthDate, " +
					"PayRate = @PayRate, " +
					"LaborRate1 = @LaborRate1, " +
					"LaborRate2 = @LaborRate2, " +
					"LaborRate3 = @LaborRate3, " +
					"LaborRate4 = @LaborRate4, " +
					"Email = @Email, " +
					"ResponsibleOfficer = @ResponsibleOfficer, " +
					"Shift = @Shift, " +
					"CompanyGuid = @CompanyGuid, " +
					"PINNumber = @PINNumber, " +
					"PINRequired = @PINRequired, " +
					"LockedOut= @LockedOut, " +
					"LockedOutReason = @LockedOutReason, " +
					"LockedOutDate = @LockedOutDate, " +
					"UpdatedDate = @UpdatedDate, " +
					"ShortCardNumber = @ShortCardNumber, " +
                    "HiddenDate = @HiddenDate, " +
					"UpdatedBy = @UpdatedBy, " +
					"OnFileSignature = @OnFileSignature, " +
					"UserData1 = @UserData1, " +
					"UserData2 = @UserData2, " +
					"UserData3 = @UserData3, " +
					"UserData4 = @UserData4, " +
					"UserData5 = @UserData5, " +
					"UserData6 = @UserData6, " +
					"UserData7 = @UserData7, " +
					"UserData8 = @UserData8, " +
					"UserData9 = @UserData9, " +
					"UserData10 = @UserData10," +
					"UserData11 = @UserData11," +
					"UserData12 = @UserData12," +
					"UserData13 = @UserData13," +
					"UserData14 = @UserData14," +
					"UserData15 = @UserData15," +
					"UserData16 = @UserData16," +
					"UserData17 = @UserData17," +
					"UserData18 = @UserData18," +
					"UserData19 = @UserData19," +
					"UserData20 = @UserData20," +
					"UserData21 = @UserData21," +
					"UserData22 = @UserData22," +
					"UserData23 = @UserData23," +
               "UserData24 = @UserData24," +
               "InhibitInactivityLockout = @InhibitInactivityLockout " + "WHERE PersonnelGuid = @PersonnelGuid";
				command.CommandText = sql;
				command.CommandType = CommandType.Text;
				command.Parameters.Add(DataObject.NewGuidParameter("@SiteGuid", this._SiteGuid));
				newParameter = command.Parameters.Add("@PersonID", SqlDbType.NVarChar, 50);
				newParameter.Value = this._ID;
				newParameter = command.Parameters.Add("@CardNumber", SqlDbType.NVarChar, 30);
				if (string.IsNullOrEmpty(this._CardNumber))
				{
					newParameter.Value = DBNull.Value;
				}
				else
				{
					newParameter.Value = this._CardNumber;
				}
				command.Parameters.Add(DataObject.NewGuidParameter("@UserGuid", this.UserGuid, true)); // true means set to NULL if empty
				newParameter = command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 20);
				newParameter.Value = this._FirstName;
				newParameter = command.Parameters.Add("@MiddleName", SqlDbType.NVarChar, 20);
				newParameter.Value = this._MiddleName;
				newParameter = command.Parameters.Add("@LastName", SqlDbType.NVarChar, 30);
				newParameter.Value = this._LastName;
				newParameter = command.Parameters.Add("@Title", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Title;
				newParameter = command.Parameters.Add("@Department", SqlDbType.NVarChar, 20);
				newParameter.Value = this._Department;
				command.Parameters.Add(DataObject.NewGuidParameter("@SupervisorPersonnelGuid", this.SupervisorGuid, true)); // true means set to NULL if empty
				newParameter = command.Parameters.Add("@Address1", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Address1;
				newParameter = command.Parameters.Add("@Address2", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Address2;
				newParameter = command.Parameters.Add("@City", SqlDbType.NVarChar, 60);
				newParameter.Value = this._City;
				newParameter = command.Parameters.Add("@State", SqlDbType.NVarChar, 20);
				newParameter.Value = this._State;
				newParameter = command.Parameters.Add("@Zip", SqlDbType.NVarChar, 10);
				newParameter.Value = this._Zip;
				newParameter = command.Parameters.Add("@Country", SqlDbType.NVarChar, 20);
				newParameter.Value = this._Country;
				newParameter = command.Parameters.Add("@Phone1", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Phone1;
				newParameter = command.Parameters.Add("@Phone2", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Phone2;
				newParameter = command.Parameters.Add("@AssignmentDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._AssignmentDate.Value;
				newParameter = command.Parameters.Add("@SupervisionDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._SupervisionDate.Value;
				newParameter = command.Parameters.Add("@SSAN", SqlDbType.NVarChar, 11);
				newParameter.Value = this._SSAN;
				newParameter = command.Parameters.Add("@BirthDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._BirthDate.Value;
				newParameter = command.Parameters.Add("@PayRate", SqlDbType.Money);
				newParameter.Value = this._PayRate;
				newParameter = command.Parameters.Add("@LaborRate1", SqlDbType.Float);
				newParameter.Value = this._LaborRate1;
				newParameter = command.Parameters.Add("@LaborRate2", SqlDbType.Float);
				newParameter.Value = this._LaborRate2;
				newParameter = command.Parameters.Add("@LaborRate3", SqlDbType.Float);
				newParameter.Value = this._LaborRate3;
				newParameter = command.Parameters.Add("@LaborRate4", SqlDbType.Float);
				newParameter.Value = this._LaborRate4;
				newParameter = command.Parameters.Add("@Email", SqlDbType.NVarChar, 50);
				newParameter.Value = this._Email;
				newParameter = command.Parameters.Add("@ResponsibleOfficer", SqlDbType.Bit);
				newParameter.Value = this._ResponsibleOfficer;
				newParameter = command.Parameters.Add("@Shift", SqlDbType.SmallInt);
				newParameter.Value = this._Shift;
				command.Parameters.Add(DataObject.NewGuidParameter("@CompanyGuid", this.CompanyGuid, true));// true means set to NULL if empty

			    newParameter = command.Parameters.Add("@PINNumber", SqlDbType.VarBinary, 256);
                
                newParameter.Value = !string.IsNullOrEmpty(this._PINNumber)
						? (object)UserClass.encode(this._PINNumber, this.MasterSiteGuid)
			            : DBNull.Value;

				newParameter = command.Parameters.Add("@PINRequired", SqlDbType.Bit);
				newParameter.Value = this._PINRequired;
				newParameter = command.Parameters.Add("@LockedOut", SqlDbType.Bit);
				newParameter.Value = this._LockedOut;
				newParameter = command.Parameters.Add("@LockedOutReason", SqlDbType.NVarChar, 80);
				newParameter.Value = this._LockedOutReason;
				newParameter = command.Parameters.Add("@LockedOutDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._LockedOutDate.Value;
				newParameter = command.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._UpdatedDate;
				newParameter = command.Parameters.Add("@ShortCardNumber", SqlDbType.NVarChar, 6);
				if (string.IsNullOrEmpty(this._ShortCardNumber))
				{
					newParameter.Value = DBNull.Value;
				}
				else
				{
					newParameter.Value = this._ShortCardNumber;
				}

                command.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset).Value = this.HiddenDate ?? (object)DBNull.Value;

				newParameter = command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
				newParameter.Value = this._UpdatedBy;
				newParameter = command.Parameters.Add("@OnFileSignature", SqlDbType.VarBinary);
				if (this._OnFileSignature == null || this._OnFileSignature.Length == 0)
				{
					newParameter.Size = 0;
					newParameter.Value = DBNull.Value;
				}
				else
				{
					newParameter.Size = this._OnFileSignature.Length;
					newParameter.Value = this._OnFileSignature;
				}
				command.Parameters.Add(DataObject.NewGuidParameter("@PersonnelGuid", this._IdentityGuid));
				for (int i = 1; i <= 24; i++)
				{
					newParameter = command.Parameters.Add("@UserData" + i.ToString(), SqlDbType.NVarChar, 60);
					newParameter.Value = this.UserData[i - 1];
				}
                newParameter = command.Parameters.Add("@InhibitInactivityLockout", SqlDbType.Bit);
                newParameter.Value = this._InhibitInactivityLockout;
            }
			else
			{
				sql = "UPDATE tblPersonnel " +
						"SET LastActivityDate = @LastActivityDate, " +
						"CardedIn = @CardedIn, " +
						"Status = @Status, " +
						"AssignedEquipmentGuid = @AssignedEquipmentGuid, " +
						"UpdatedDate = @UpdatedDate, " +
						"UpdatedBy = @UpdatedBy ";

				if(this.LockedOut)
				{
					sql += ", LockedOut= @LockedOut, " +
							"LockedOutReason = @LockedOutReason, " +
							"LockedOutDate = @LockedOutDate ";
				}

				sql += "WHERE PersonnelGuid = @PersonnelGuid";

				command.CommandText = sql;
				command.CommandType = CommandType.Text;
				newParameter = command.Parameters.Add("@LastActivityDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._LastActivityDate.Value;
				newParameter = command.Parameters.Add("@CardedIn", SqlDbType.Bit);
				newParameter.Value = this._CardedIn;
				newParameter = command.Parameters.Add("@Status", SqlDbType.SmallInt, 1);
				newParameter.Value = this._Status;
				command.Parameters.Add(DataObject.NewGuidParameter("@AssignedEquipmentGuid", this.AssignedEquipmentGuid, true));
				newParameter = command.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
				newParameter.Value = this._UpdatedDate;
				newParameter = command.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
				newParameter.Value = this._UpdatedBy;
				command.Parameters.Add(DataObject.NewGuidParameter("@PersonnelGuid", this.IdentityGuid));

				if (this.LockedOut)
				{
					newParameter = command.Parameters.Add("@LockedOut", SqlDbType.Bit);
					newParameter.Value = this._LockedOut;
					newParameter = command.Parameters.Add("@LockedOutReason", SqlDbType.NVarChar, 80);
					newParameter.Value = this._LockedOutReason;
					newParameter = command.Parameters.Add("@LockedOutDate", SqlDbType.DateTimeOffset);
					newParameter.Value = this._LockedOutDate.Value;
				}
			}

			return command;
		}


		public void QueryWriterSQL(SqlCommand cmd, SecurityClass security, string selectClause)
		{
			// Construct the query writer sql. The WHERE 1 = 1 is required because the query writer functionality relies on a WHERE being present in the query already
			// If you don't have the WHERE it will just tack on ANDs for each field in the criteria
			cmd.CommandText = selectClause +
				" ,tblPersonnel.[PersonnelGuid] AS EntityGuid," +
				" tblUsers.UserID AS 'tblUsers.UserID'," +
				" Supervisors.PersonID AS 'Supervisors.PersonID'," +
				" tblEquipment.ID AS 'tblEquipment.ID', " +
                " (SELECT COUNT(*) FROM map.tblCompanyPersonnelAssignedToCompany CPA WHERE perA.PersonnelGuid = CPA.PersonnelGuid) AS 'Drivers.AssignedCompaniesCount'" +
                " FROM [erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) perA INNER JOIN tblPersonnel ON perA.PersonnelGuid = tblPersonnel.PersonnelGuid " + 
				" LEFT JOIN tblCompanies C ON tblPersonnel.CompanyGuid = C.CompanyGuid" +
				" LEFT JOIN tblUsers ON tblUsers.UserGuid = tblPersonnel.UserGuid" +
				" LEFT JOIN tblPersonnel Supervisors ON tblPersonnel.SupervisorPersonnelGuid = Supervisors.PersonnelGuid" +
				" LEFT JOIN tblEquipment ON tblEquipment.[EquipmentGuid] = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', tblPersonnel.AssignedEquipmentGuid, @TargetSiteGuid)" +
				" WHERE 1 = 1";

			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
		}

        /*
		public SqlCommand GetLatestRowVersionByRole(SecurityClass security, PERSON_ROLE role, bool bInTransaction)
		{
			return EnumerateByRoleSQL(security, role, bInTransaction, false);
		}
        */

		public string EnumerateNotificationSQL( SecurityClass security )
		{
			string siteFromJoinClause = " From [erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) perA INNER JOIN tblPersonnel perB ON perA.PersonnelGuid = perB.PersonnelGuid "; 

			string sql = "DECLARE @topRowVersion TIMESTAMP SET @topRowVersion = ( SELECT TOP 1 perB.[_RowVersion] " + siteFromJoinClause + " ORDER BY perB.[_RowVersion] DESC) ";

			sql += "DECLARE @topDate DATETIMEOFFSET SET @topDate = ( SELECT TOP 1 perB.[UpdatedDate] " + siteFromJoinClause + " ORDER BY perB.UpdatedDate DESC) ";

			sql += "SELECT COUNT(*) as 'Count',IsNull(@topRowVersion,0) as 'TopIndex',IsNull(@topDate,'1900-01-01') as 'TopDate' " + siteFromJoinClause;

			return sql;
		}

		public QualificationClass FindQualificationInPayload(QUALIFICATION_TYPE type, string qualificationID)
		{
			if (this.QualificationExportPayload != null)
			{
				foreach (QualificationClass qual in this.QualificationExportPayload)
				{
					if (qual.Type.Equals(type) && qual.ID.Equals(qualificationID))
					{
						return qual;
					}
				}
			}
			return null;
		}
		#endregion

		#region Internal methods
		public override void Reset()
		{
			base.Reset();
		    this._CardNumber = "";
		    this.UserGuid = Guid.Empty;
		    this._FirstName = "";
		    this._MiddleName = "";
		    this._LastName = "";
		    this._Title = "";
		    this._Department = "";
		    this.SupervisorGuid = Guid.Empty;
		    this._Address1 = "";
		    this._Address2 = "";
		    this._City = "";
		    this._State = "";
		    this._Zip = "";
		    this._Country = "";
		    this._Phone1 = "";
		    this._Phone2 = "";
		    this._SSAN = "";
		    this._PayRate = new Decimal(0.0);
		    this._LaborRate1 = 0.0;
		    this._LaborRate2 = 0.0;
		    this._LaborRate3 = 0.0;
		    this._LaborRate4 = 0.0;
		    this._Status = 0;
		    this._Email = "";
		    this._ResponsibleOfficer = false;
		    this._Shift = 0;
		    this.CompanyGuid = Guid.Empty;
		    this._PINNumber = "";
		    this._PINRequired = true;
		    this._LockedOut = false;
		    this._LockedOutReason = "";
		    this._CardedIn = false;
		    this._CompanyID = "{Unassigned}";
		    this._CompanyName = "";
		    this._CompanyAddress = "";
		    this._CompanyCity = "";
		    this._CompanyState = "";
		    this._UserID = "";
		    this._SupervisorID = "";
		    this._ShortCardNumber = "";
		    this.HiddenDate = null;
		    this.RoleCollection = new PersonRoleMapCollectionClass();
		    this.QualificationCollection = new QualificationMapCollectionClass();
		    this.LicenseCollection = new QualificationMapCollectionClass();
		    this.TrainingCollection = new QualificationMapCollectionClass();
		    this.AccessScheduleCollection = new ScheduleCollectionClass();
		    this._OnFileSignature = null;
		    this.AssignedCompaniesCollection = new CompanyMapCollectionClass();

		    this._AssignedEquipmentID = "{Unassigned}";
		    this._AssignedCompaniesCount = 0;
		    this.AssignedEquipmentGuid = Guid.Empty;
            this._InhibitInactivityLockout = false;

            DAY_OF_WEEK[] dayOfWeek = {DAY_OF_WEEK.SUNDAY,
													DAY_OF_WEEK.MONDAY,
													DAY_OF_WEEK.TUESDAY,
													DAY_OF_WEEK.WEDNESDAY,
													DAY_OF_WEEK.THURSDAY,
													DAY_OF_WEEK.FRIDAY,
													DAY_OF_WEEK.SATURDAY};

			for (int item = 0; item < 7; item++)
			{
			    ScheduleClass schedule = new ScheduleClass(this.dateTimeFormatInfo)
			                             {
			                                 Type = SCHEDULE_TYPE.PERSON_ACCESS_TYPE,
			                                 Day = (int)dayOfWeek[item],
			                                 Enabled = true,
			                                 OpeningTime =
			                                 {
			                                     Value = TimeConverter.MinFMTime
			                                 },
			                                 ClosingTime =
			                                 {
			                                     Value = TimeConverter.MaxFMTime
			                                 }
			                             };



			    this.AccessScheduleCollection.Add(schedule);
			}
		    this.UserData = new UserDataClass();

		    this.Status = STATUS.Out;

		    this._AssignmentDate.Value = TimeConverter.Today(this._AssignmentDate.StandardName);
		    this._SupervisionDate.Value = TimeConverter.Today(this._SupervisionDate.StandardName);
		    this._BirthDate.Value = TimeConverter.Today(this._BirthDate.StandardName);
		    this._LockedOutDate.Value = TimeConverter.Today(this._LockedOutDate.StandardName);
		    this._LastActivityDate.Value = TimeConverter.Now(this._LastActivityDate.StandardName);
		}
		#endregion

		public QueryWriterFieldCollection QueryAliasFields(SecurityClass security, QueryWriterFieldCollection fields)
		{
			var userDataFieldCollection =
				FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
					x => x.EnumerateByEntityType(security, ENTITY_TYPE.PERSONNEL, Guid.Empty, false, false));

			QueryWriterFieldCollection newCollection = new QueryWriterFieldCollection(fields);

			var userFields = from f in newCollection
								  where f.DisplayName.StartsWith("User Data")
								  select f;

			foreach (var userField in userFields)
			{
				if (this.UpdateFieldName(userField, userDataFieldCollection) == false)
				{
					userField.DisplayName = string.Empty;
				}
			}

			// Remove any blanked out fields.  Wish we could do it above but
			// it disrupts the enumeration.
			for (int index = newCollection.Count - 1; index >= 0; --index)
			{
				if (string.IsNullOrEmpty(newCollection[index].DisplayName))
				{
					newCollection.RemoveAt(index);
				}
			}

			QueryClass.ApplyDataDictionary(security, newCollection);

			return newCollection;
		}

		public string DetailPageReference()
		{
			return "FMWebApp\\PersonForm.aspx";
		}
	}
}
