namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Xml;
	using System.Xml.Serialization;

	using BusinessInterfaces;
	using ChannelFactories;

	using Opc;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	 public enum FUELING_TYPES { NONE, REFUELER, DEFUELER };

	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(EquipmentClass))]
	public class EquipmentCollectionClass : List<EquipmentClass>
	{
	}

	[Serializable]
	public struct EquipmentInfo
	{
		public string ID;
		public string Xref;
		public Guid siteGuid;
		public Guid identityGuid;
		  public Guid masterRecordGuid;
		  public Guid AssignedToSiteGuid;
		  public Guid AssignedFromSiteGuid;
		  public string AssignedFromSiteId;
	}


	[XmlInclude(typeof(GregorianCalendar))]
	[DebuggerDisplay("ID={ID},IdentityGuid={IdentityGuid}")]
	[EntityImportExportWorksheetAttribute("EQUIPMENT")]
	[KnownType(typeof(GregorianCalendar))]
	[KnownType(typeof(QualityTagClass))]
	[KnownType(typeof(ProcessVariableClass))]
	[QueryWriterTopic(typeof(EquipmentClass), "Equipment", PostQueryAliasName = "tblCombinedTable")]
	[QueryWriterTopicSecurity(RIGHT.VIEW_EQUIPMENT_DATA)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_EQUIPMENT_DATA)]
	[DataContract]
	[Serializable]
	public class EquipmentClass : FMBaseDataObjectWithUserData, IAlarmAndEventDiscovery, IComparable
	{
		#region Public Data Members
		[DataMember]
		public const string ENTITY_TYPE_ID = "Equipment";
		[XmlIgnore]
		[DataMember]
		public Guid EquipmentTypeGuid;
		[DataMember]
		public QualityTagClass QualityTag;
		[DataMember]
		public Guid CompanyGuid;

		[XmlIgnore]
		[DataMember]
		public Guid ProductGuid;

		[XmlIgnore]
		[DataMember]
		public Guid FuelCardGuid;

		[XmlIgnore]
		[DataMember]
		public Guid AssetTrackingDeviceGuid;

		[DataMember]
		public SIDouble _LowStockWarning;
		[DataMember]
		public SIDouble _Volume;

		[DataMember]
		public Guid ParentEquipmentGuid;	// When the equipment is a compartment this represents the containing equipment Guid

		[DataMember]
		public Date _LockedOutDate;				// Excluded from PropertyMap
		[DataMember]
		public Date _ReturnToServiceDate;
		[DataMember]
		public Date _ManufactureDate;
		[DataMember]
		public Date _InstallationDate;
		[DataMember]
		public Date _InspectionDate;
		[DataMember]
		public Date _CalibrationDate;
		[DataMember]
		public Date _QCDate = new Date();
		[DataMember]
		public Date _MaintenanceChangeDate;
		[DataMember]
		public Date _MaintenanceCreatedDate;
		[DataMember]
		public Date _MaintenanceUpdatedDate;
		[DataMember]
		public Date _QualityCreatedDate;
		[DataMember]
		public Date _QualityUpdatedDate;
		[DataMember]
		public string _QualityUpdatedBy;

		//	protected bool _MultiCompartment;
		
		[EntityImportExportAttribute("ASSIGNEDTOMETERGUID", 200, "AssignedToMeterGuid")]
		[DataMember]
		public Guid AssignedToMeterGuid;

		[EntityImportExportWorksheetAttribute("EQUIPMENT METERS")]
		[EntityImportExportAttribute("METERID*", 30, "ID", 1)]
		[EntityImportExportAttribute("NUMBEROFDIGITS", 2, "NumberOfDigits", 2)]
		[EntityImportExportAttribute("ROTATESBACKWARDS", 10, "RotatesBackwardsFlag", 3)]
		[EntityImportExportAttribute("RECEIPTMETER", 10, "ReceiptMeterFlag", 4)]
		[EntityImportExportAttribute("METERFACTOR", 8, "MeterFactor", 5)]
		[EntityImportExportAttribute("FUELCOMPRESSIONFACTOR", 8, "FuelCompressionFactor", 6)]
		[DataMember]
		public List<MeterClass> Meter = new List<MeterClass>();

		[EntityImportExportWorksheetAttribute("EQUIPMENT COMPARTMENTS")]
		[EntityImportExportAttribute("NUMBER", 150, "EquipmentSequence", 1)]
		[EntityImportExportAttribute("CAPACITY", 80, "CAPACITY", 2)]
		[EntityImportExportAttribute("SAFEFILL", 80, "SAFEFILL", 3)]
		[DataMember]
		public EquipmentCollectionClass CompartmentCollection;

		[EntityImportExportWorksheetAttribute("EQUIPMENT TEST AND INSPECTIONS")]
		[EntityImportExportAttribute("ID*", 120, "ID", 1)]
		[EntityImportExportAttribute("TYPE", 110, "Type", 2)]
		[EntityImportExportAttribute("NUMBER", 80, "NUMBER", 4)]
		[EntityImportExportAttribute("EXPIRATIONDATE", 100, "ExpirationDateString", 3)]
		[DataMember]
		public QualificationMapCollectionClass TestAndInspectionCollection;

		[EntityImportExportWorksheetAttribute("EQUIPMENT TAGS AND LICENSES")]
		[EntityImportExportAttribute("ID*", 120, "ID", 1)]
		[EntityImportExportAttribute("TYPE", 110, "Type", 2)]
		[EntityImportExportAttribute("NUMBER", 80, "NUMBER", 4)]
		[EntityImportExportAttribute("EXPIRATIONDATE", 100, "ExpirationDateString", 3)]
		[DataMember]
		public QualificationMapCollectionClass TagAndLicenseCollection;
		#endregion

		#region Protected Data Members
		[DataMember]
		protected string _Description;
		[DataMember]
		protected string _Make;
		[DataMember]
		protected string _Model;
		[DataMember]
		protected int _Year;
		[DataMember]
		protected string _IssPt;
		[DataMember]
		protected string _IssPtNum;

		[XmlIgnore]
		[DataMember]
		protected bool _Fixed;

		[DataMember]
		protected string _StorageType;
		[DataMember]
		protected bool _InUse;
		[DataMember]
		protected bool _FixedVolume;
		[DataMember]
		protected bool _IntoPlane;
		[DataMember]
		protected bool _Mobile;
		[DataMember]
		protected string _AttachedTo;
		[DataMember]
		protected string _MediaType;
		[DataMember]
		protected bool _DefuelMeterForwards;
		[DataMember]
		protected int _Meters;
		[DataMember]
		protected double _PulseRatio;
		[DataMember]
		protected bool _Round;

		[DataMember]
		protected string _Xref;
		[DataMember]
		protected bool _StockTrack;
		[DataMember]
		protected string _Totalisor1;
		[DataMember]
		protected string _Totalisor2;
		[DataMember]
		protected string _FuelingState;
		[DataMember]
		protected double _MeterReading;
		[DataMember]
		protected int _Consecutive_OOS_Variance;
		[DataMember]
		protected string _Notes;
		[DataMember]
		protected SIDouble _Capacity;
		[DataMember]
		protected SIDouble _SafeFill;
		[DataMember]
		protected EngineeringUnit _VolumeUnits;
		[DataMember]
		protected EngineeringUnit _TemperatureUnits;
		[DataMember]
		protected EngineeringUnit _DensityUnits;
		[DataMember]
		protected EngineeringUnit _MassUnits;
		[DataMember]
		protected short _VolumeDecimalPlaces;
		[DataMember]
		protected short _TemperatureDecimalPlaces;
		[DataMember]
		protected short _DensityDecimalPlaces;
		[DataMember]
		protected short _MassDecimalPlaces;

		[DataMember]
		protected string _EquipmentSequence;
		[DataMember]
		protected bool _LockedOut;
		[DataMember]
		protected string _LockedOutReason;
		[DataMember]
		protected string _SerialNumber;
		[DataMember]
		protected string _TruckCardNumber;
		  [DataMember]
		  protected bool _ScullyRequired;
		  [DataMember]
		protected string _CompanyEquipmentID;

		[DataMember]
		protected double _RatedGPM;
		[DataMember]
		protected double _ActualGPM;
		[DataMember]
		protected bool _FuelAdditiveFlag;
		[DataMember]
		protected bool _SecondaryStorageFlag;
		[DataMember]
		protected string _StatusDescription;
		[DataMember]
		protected string _CompanyID;

		[XmlIgnore]
		[DataMember]
		protected string _CompanyName;

		[XmlIgnore]
		[DataMember]
		protected string _CompanyAddress;

		[XmlIgnore]
		[DataMember]
		protected string _CompanyCity;

		[XmlIgnore]
		[DataMember]
		protected string _CompanyState;

		[DataMember]
		protected string _ProductID;
		[DataMember]
		protected string _FuelCardID;
		[DataMember]
		protected string assetTrackingDeviceId;


		[DataMember]
		protected bool _ManagedEquipmentFlag;
		[DataMember]
		protected FUELING_TYPES _FuelingType;
		[DataMember]
		protected string _MaintenanceNote;
		[DataMember]
		protected string _QCNote;
		[DataMember]
		protected bool _InServiceFlag;

		[DataMember]
		protected string _MaintenanceOperatorID;
		[DataMember]
		protected string _MaintenanceWorkOrder;
		[DataMember]
		protected string _MaintenanceCreatedBy;
		[DataMember]
		protected string _MaintenanceUpdatedBy;
		[DataMember]
		protected string _QualityCreatedBy;

		[DataMember]
		protected ProcessVariableClass volumeProcessVariable;

		[DataMember]
		protected Guid _MasterRecordGuid;

		// EquipmentType Composite Attributes to avoid holding a reference to an EquipmentType
		[DataMember]
		protected string _EquipmentTypeID = null;
		[DataMember]
		protected EQUIPMENT_TYPE _EquipmentType = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;
		[DataMember]
		protected bool _IsMultiCompartment;
		[DataMember]
		protected COMPANY_ROLE _CompanyRoleAssignmentConstraint = COMPANY_ROLE.MAX_COMPANY_ROLE;

		#endregion

		#region Static members
		static string EquipmentLockOutKey = "Equipment Lock Out";
		static string EquipmentLockedOutKey = "Equipment Locked Out";
		static AlarmAndEventDescriptorClass EquipmentLockOutEventDescriptor = new AlarmAndEventDescriptorClass(false, SystemKey, EquipmentLockOutKey);
		static AlarmAndEventDescriptorClass EquipmentLockedOutAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, EquipmentLockedOutKey);

		string SelectClause = 
								@"SELECT tblEquipment.*, 
								C.ID AS CompanyID, 
								C.Name,C.Address1, 
								C.City,
								C.State, 
								D.EqTypeName, 
								D.LookupEquipmentTypeIndex, 
								D.Capacity, 
								D.SafeFill, 
								D.MultiCompartment, 
								D.Isspt, 
								D.LookupCompanyRoleIndex, 
								E.EstReturnToServiceDate AS ReturnToServiceDate, 
								E.MaintenanceReason AS StatusDescription, 
								E.InServiceFlag, 
								E.Memo AS MaintenanceNote, 
								E.ChangeDate, 
								E.OperatorID as MaintenanceOperatorID, 
								E.WorkOrder as MaintenanceWorkOrder, 
								E.CreatedDate as MaintenanceCreatedDate, 
								E.CreatedBy as MaintenenaceCreatedBy, 
								E.UpdatedDate as MaintenenaceUpdatedDate, 
								E.UpdatedBy as MaintenanceUpdatedBy, 
								CASE WHEN ISNULL(LTRIM(RTRIM(G.Memo)), '') = '' THEN '' ELSE 'QC Tag Memo: ' + G.Memo + CHAR(0x0d) + CHAR(0x0d) END + 
								CASE WHEN ISNULL(LTRIM(RTRIM(F.Memo)), '') = '' THEN '' ELSE 'Test Result Memo: ' + F.Memo END as QCNote, 
								G.QualityCreatedDate, 
								G.QualityCreatedBy, 
								G.QualityUpdatedDate, 
								G.QualityUpdatedBy, 
								G.QualityTagGuid, 
								G.SiteGuid AS QualityTagSiteGuid, 
								G.Name AS QualityTagName, 
								G.Severity, 
								G.Active, 
								(SELECT ProductID FROM tblProducts WHERE tblProducts.ProductGuid = tblEquipment.ProductGuid) AS ProductID, 
								(SELECT ID FROM tblFuelCards fc WHERE fc.FuelCardGuid = tblEquipment.FuelCardGuid) AS FuelCardID, 
								(SELECT DeviceID FROM tblAssetTrackingDevice atd WHERE atd.AssetTrackingDeviceGuid = tblEquipment.AssetTrackingDeviceGuid) AS AssetTrackingDeviceID ";
		#endregion

		#region Properties
		[DataMember]
		public Guid StatusDescriptionGuid { get; set; }

		[DataMember]
		public FuelCardClass ExportUseFuelCard { get; set; }

		[DataMember]
		public bool IsAssignedToPersonnel { get; set; }

		[EntityImportExportAttribute("SITE*", 105, "SITEGUID")]
		new public Guid SiteGuid { get { return this._SiteGuid; } set {
			this._SiteGuid = value; } }

		[QueryWriterField("ID", "tblEquipment.ID")]
		[EntityImportExportAttribute("EQUIPMENTID*", 100, "ID")]
		public override string ID { get { return this._ID; } set {
			this.SetString("ID", 30, value, ref this._ID); } }

		[QueryWriterField("Description", "tblEquipment.Description")]
		[EntityImportExportAttribute("DESCRIPTION", 100, "Description")]
		public string Description { get { return this._Description; } set {
			this.SetString("Description", 50, value, ref this._Description); } }

		[QueryWriterField("Company Role Constraint", "CompanyRoleConstraint", false)]
		[EntityImportExportAttribute("COMPANYROLECONSTRAINT", 100, "CompanyRoleConstraint")]
		public COMPANY_ROLE CompanyRoleAssignmentConstraint { get{ return this._CompanyRoleAssignmentConstraint;} }

		[QueryWriterField("Type", false)]
		[EntityImportExportAttribute("EQUIPMENTTYPE", 100, "EquipmentType")]
		public EQUIPMENT_TYPE Type { get { return this._EquipmentType; } set{
			this._EquipmentType = value;}}

		[EntityImportExportAttribute("TYPECLASS", 100, "TypeClass")]
		public string TypeClass { get { return this._EquipmentTypeID; } set {
			this.SetString("TypeClass", 50, value, ref this._EquipmentTypeID); } }

		[QueryWriterField("Make", "tblEquipment.Make")]
		[EntityImportExportAttribute("MAKE", 80, "Make")]
		public string Make { get { return this._Make; } set {
			this.SetString("Make", 20, value, ref this._Make); } }

		[QueryWriterField("Model", "tblEquipment.Model")]
		[EntityImportExportAttribute("MODEL", 80, "Model")]
		public string Model { get { return this._Model; } set {
			this.SetString("Model", 50, value, ref this._Model); } }

		[QueryWriterField("Year", "tblEquipment.Year")]
		[EntityImportExportAttribute("YEAR", 80, "Year")]
		public int Year { get { return this._Year; } set {
			this._Year = value; } }

		[QueryWriterField("Fixed")]
		[EntityImportExportAttribute("FIXED", 70, "Fixed")]
		public bool Fixed { get { return this._Fixed; } set {
			this._Fixed = value; } }

		[QueryWriterField("Issue Point Number")]
		[EntityImportExportAttribute("ISSPTNUM", 120, "IssPtNum")]
		public string IssPtNum { get { return this._IssPtNum; } set {
			this.SetString("IssPtNum", 50, value, ref this._IssPtNum); } }

		[QueryWriterField("Issue Point")]
		[EntityImportExportAttribute("ISSPT", 50, "ISSPT")]
		public string IssPt { get { return this._IssPt; } set {
			this.SetString("IssPt", 20, value, ref this._IssPt); } }

		[QueryWriterField("Storage Type")]
		[EntityImportExportAttribute("STORAGETYPE", 100, "StorageType")]
		public string StorageType { get { return this._StorageType; } set {
			this.SetString("Storage Type", 2, value, ref this._StorageType); } }

		[QueryWriterField("In Use")]
		[EntityImportExportAttribute("INUSE", 70, "InUse")]
		public bool InUse { get { return this._InUse; } set {
			this._InUse = value; } }

		[QueryWriterField("Fixed Volume")]
		[EntityImportExportAttribute("FIXEDVOLUME", 100, "FixedVolume")]
		public bool FixedVolume { get { return this._FixedVolume; } set {
			this._FixedVolume = value; } }

		[EntityImportExportAttribute("INTOPLANE", 80, "IntoPlane")]
		public bool IntoPlane { get { return this._IntoPlane; } set {
			this._IntoPlane = value; } }

		[QueryWriterField("Mobile")]
		[EntityImportExportAttribute("MOBILE", 70, "Mobile")]
		public bool Mobile { get { return this._Mobile; } set {
			this._Mobile = value; } }

		[QueryWriterField("Attached To")]
		[EntityImportExportAttribute("ATTACHEDTO", 100, "AttachedTo")]
		public string AttachedTo { get { return this._AttachedTo; } set {
			this.SetString("Attached To", 6, value, ref this._AttachedTo); } }

		[QueryWriterField("Media Type")]
		[EntityImportExportAttribute("MEDIATYPE", 95, "MediaType")]
		public string MediaType { get { return this._MediaType; } set {
			this.SetString("Media Type", 1, value, ref this._MediaType); } }

		[EntityImportExportAttribute("DEFUELMETERFORWARDS", 150, "DefuelMeterForwards")]
		public bool DefuelMeterForwards { get { return this._DefuelMeterForwards; } set {
			this._DefuelMeterForwards = value; } }

		[QueryWriterField("Meters")]
		[EntityImportExportAttribute("METERS", 70, "Meters")]
		public int Meters { get { return this._Meters; } set {
			this._Meters = value; } }

		[QueryWriterField("Pulse Ratio")]
		[EntityImportExportAttribute("PULSERATIO", 100, "PulseRatio")]
		public double PulseRatio { get { return this._PulseRatio; } set {
			this._PulseRatio = value; } }

		[QueryWriterField("Round")]
		[EntityImportExportAttribute("ROUND", 70, "Round")]
		public bool Round { get { return this._Round; } set {
			this._Round = value; } }

		[QueryWriterField("Xref")]
		[EntityImportExportAttribute("XREF", 70, "Xref")]
		public string Xref { get { return this._Xref; } set {
			this.SetString("Xref", 10, value, ref this._Xref); } }

		[QueryWriterField("Low Stock Warning", "tblEquipment.LowStockWarning")]
		[EntityImportExportAttribute("LOWSTOCKWARNING", 130, "LowStockWarning")]
		public string LowStockWarning { get { return this._LowStockWarning.ToString(); } set {
			this.SetSIDouble("Low Stock Warning", value, ref this._LowStockWarning); } }

		[QueryWriterField("Stock Track", "tblEquipment.StockTrack")]
		[EntityImportExportAttribute("STOCKTRACK", 100, "StockTrack")]
		public bool StockTrack { get { return this._StockTrack; } set {
			this._StockTrack = value; } }

		[QueryWriterField("Totalisor1")]
		[EntityImportExportAttribute("TOTALISOR1", 100, "TOTALISOR1")]
		public string Totalisor1 { get { return this._Totalisor1; } set {
			this.SetString("Totalisor1", 10, value, ref this._Totalisor1); } }

		[QueryWriterField("Totalisor2")]
		[EntityImportExportAttribute("TOTALISOR2", 100, "Totalisor2")]
		public string Totalisor2 { get { return this._Totalisor2; } set {
			this.SetString("Totalisor2", 10, value, ref this._Totalisor2); } }

		[QueryWriterField("Fueling State")]
		[EntityImportExportAttribute("FUELINGSTATE", 100, "FuelingState")]
		public string FuelingState { get { return this._FuelingState; } set {
			this.SetString("Fueling State", 30, value, ref this._FuelingState); } }

		[QueryWriterField("Volume", "tblEquipment.Volume", false)]
		[EntityImportExportAttribute("VOLUME", 80, "Volume")]
		public string Volume { get { return this._Volume.ToString(); } set {
			this.SetSIDouble("Volume", value, ref this._Volume); } }

		  /// <summary>
		  /// Represents the date + time that this equipment record was hidden
		  /// A null value indicates the equipment record is not hidden.
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

		[QueryWriterField("User Data 1", "tblEquipment.UserData1")]
		[EntityImportExportAttribute("USERDATA1", 150, "UserData1")]
		public string UserData1 { get { return this.UserData[0]; } set {
			this.UserData[0] = value; } }

		[QueryWriterField("User Data 2", "tblEquipment.UserData2")]
		[EntityImportExportAttribute("USERDATA2", 150, "UserData2")]
		public string UserData2 { get { return this.UserData[1]; } set {
			this.UserData[1] = value; } }

		[QueryWriterField("User Data 3", "tblEquipment.UserData3")]
		[EntityImportExportAttribute("USERDATA3", 150, "UserData3")]
		public string UserData3 { get { return this.UserData[2]; } set {
			this.UserData[2] = value; } }

		[QueryWriterField("User Data 4", "tblEquipment.UserData4")]
		[EntityImportExportAttribute("USERDATA4", 150, "UserData4")]
		public string UserData4 { get { return this.UserData[3]; } set {
			this.UserData[3] = value; } }

		[QueryWriterField("User Data 5", "tblEquipment.UserData5")]
		[EntityImportExportAttribute("USERDATA5", 150, "UserData5")]
		public string UserData5 { get { return this.UserData[4]; } set {
			this.UserData[4] = value; } }

		[QueryWriterField("User Data 6", "tblEquipment.UserData6")]
		[EntityImportExportAttribute("USERDATA6", 150, "UserData6")]
		public string UserData6 { get { return this.UserData[5]; } set {
			this.UserData[5] = value; } }

		[QueryWriterField("User Data 7", "tblEquipment.UserData7")]
		[EntityImportExportAttribute("USERDATA7", 150, "UserData7")]
		public string UserData7 { get { return this.UserData[6]; } set {
			this.UserData[6] = value; } }

		[QueryWriterField("User Data 8", "tblEquipment.UserData8")]
		[EntityImportExportAttribute("USERDATA8", 150, "UserData8")]
		public string UserData8 { get { return this.UserData[7]; } set {
			this.UserData[7] = value; } }

		[QueryWriterField("User Data 9", "tblEquipment.UserData9")]
		[EntityImportExportAttribute("USERDATA9", 150, "UserData9")]
		public string UserData9 { get { return this.UserData[8]; } set {
			this.UserData[8] = value; } }

		[QueryWriterField("User Data 10", "tblEquipment.UserData10")]
		[EntityImportExportAttribute("USERDATA10", 150, "UserData10")]
		public string UserData10 { get { return this.UserData[9]; } set {
			this.UserData[9] = value; } }

		[QueryWriterField("User Data 11", "tblEquipment.UserData11")]
		[EntityImportExportAttribute("USERDATA11", 150, "UserData11")]
		public string UserData11 { get { return this.UserData[10]; } set {
			this.UserData[10] = value; } }

		[QueryWriterField("User Data 12", "tblEquipment.UserData12")]
		[EntityImportExportAttribute("USERDATA12", 150, "UserData12")]
		public string UserData12 { get { return this.UserData[11]; } set {
			this.UserData[11] = value; } }

		[QueryWriterField("User Data 13", "tblEquipment.UserData13")]
		[EntityImportExportAttribute("USERDATA13", 150, "UserData13")]
		public string UserData13 { get { return this.UserData[12]; } set {
			this.UserData[12] = value; } }

		[QueryWriterField("User Data 14", "tblEquipment.UserData14")]
		[EntityImportExportAttribute("USERDATA14", 150, "UserData14")]
		public string UserData14 { get { return this.UserData[13]; } set {
			this.UserData[13] = value; } }

		[QueryWriterField("User Data 15", "tblEquipment.UserData15")]
		[EntityImportExportAttribute("USERDATA15", 150, "UserData15")]
		public string UserData15 { get { return this.UserData[14]; } set {
			this.UserData[14] = value; } }

		[QueryWriterField("User Data 16", "tblEquipment.UserData16")]
		[EntityImportExportAttribute("USERDATA16", 150, "UserData16")]
		public string UserData16 { get { return this.UserData[15]; } set {
			this.UserData[15] = value; } }

		[QueryWriterField("User Data 17", "tblEquipment.UserData17")]
		[EntityImportExportAttribute("USERDATA17", 150, "UserData17")]
		public string UserData17 { get { return this.UserData[16]; } set {
			this.UserData[16] = value; } }

		[QueryWriterField("User Data 18", "tblEquipment.UserData18")]
		[EntityImportExportAttribute("USERDATA18", 150, "UserData18")]
		public string UserData18 { get { return this.UserData[17]; } set {
			this.UserData[17] = value; } }

		[QueryWriterField("User Data 19", "tblEquipment.UserData19")]
		[EntityImportExportAttribute("USERDATA19", 150, "UserData19")]
		public string UserData19 { get { return this.UserData[18]; } set {
			this.UserData[18] = value; } }

		[QueryWriterField("User Data 20", "tblEquipment.UserData20")]
		[EntityImportExportAttribute("USERDATA20", 150, "UserData20")]
		public string UserData20 { get { return this.UserData[19]; } set {
			this.UserData[19] = value; } }

		[QueryWriterField("User Data 21", "tblEquipment.UserData21")]
		[EntityImportExportAttribute("USERDATA21", 150, "UserData21")]
		public string UserData21 { get { return this.UserData[20]; } set {
			this.UserData[20] = value; } }

		[QueryWriterField("User Data 22", "tblEquipment.UserData22")]
		[EntityImportExportAttribute("USERDATA22", 150, "UserData22")]
		public string UserData22 { get { return this.UserData[21]; } set {
			this.UserData[21] = value; } }

		[QueryWriterField("User Data 23", "tblEquipment.UserData23")]
		[EntityImportExportAttribute("USERDATA23", 150, "UserData23")]
		public string UserData23 { get { return this.UserData[22]; } set {
			this.UserData[22] = value; } }

		[QueryWriterField("User Data 24", "tblEquipment.UserData24")]
		[EntityImportExportAttribute("USERDATA24", 150, "UserData241")]
		public string UserData24 { get { return this.UserData[23]; } set {
			this.UserData[23] = value; } }

		[EntityImportExportAttribute("METERREADING", 100, "MeterReading")]
		public double MeterReading { get { return this._MeterReading; } set {
			this._MeterReading = value; } }

		[EntityImportExportAttribute("CONSECUTIVE_OOS_VARIANCE", 180, "Consecutive_OOS_Variance")]
		[XmlIgnore]
		public int Consecutive_OOS_Variance { get { return this._Consecutive_OOS_Variance; } private set { ; } }

		[QueryWriterField("Notes")]
		[EntityImportExportAttribute("NOTES", 100, "Notes")]
		public string Notes { get { return this._Notes; } set {
			this.SetString("Notes", 1000, value, ref this._Notes); } }

		[QueryWriterField("Capacity", "tblEquipment.Capacity", false)]
		[EntityImportExportAttribute("CAPACITY", 80, "Capacity")]
		public string Capacity
		{
			get
			{
				if (this._Capacity != null)
				{
					return this._Capacity.ToString();
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.SetSIDouble("Capacity", value, ref this._Capacity);
			}
		}

		[QueryWriterField("Safe Fill", "tblEquipment.SafeFill", false)]
		[EntityImportExportAttribute("SAFEFILL", 80, "SafeFill")]
		public string SafeFill
		{
			get
			{
				if (this._SafeFill == null)
				{
					return null;
				}
				else
				{
					return this._SafeFill.ToString();
				}
			}
			set
			{
				this.SetSIDouble("Safe Fill", value, ref this._SafeFill);
			}
		}

		public SIDouble SICapacity
		{
			get
			{
				return (this._Capacity);

			}
		}

		public SIDouble SISafeFill
		{
			get
			{
				return (this._SafeFill);

			}
		}

		[EntityImportExportAttribute("VOLUMEUNITS", 100, "VolumeUnits")]
		public EngineeringUnit VolumeUnits
		{
			get
			{
				return this._VolumeUnits;
			}
			set
			{
				this._VolumeUnits = value;
				this._LowStockWarning.Units = value;
				this._Volume.Units = value;
				this._Capacity.Units = value;
				this._SafeFill.Units = value;

				if (this.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					foreach (EquipmentClass Compartment in this.CompartmentCollection)
					{
						Compartment.VolumeUnits = this._VolumeUnits;
					}
				}
			}
		}

		[EntityImportExportAttribute("TEMPERATUREUNITS", 120, "TemperatureUnits")]
		public EngineeringUnit TemperatureUnits
		{
			get
			{
				return this._TemperatureUnits;
			}
			set
			{
				this._TemperatureUnits = value;

				if (this.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					foreach (EquipmentClass Compartment in this.CompartmentCollection)
					{
						Compartment.TemperatureUnits = this._TemperatureUnits;
					}
				}
			}
		}


		[EntityImportExportAttribute("DENSITYUNITS", 100, "DensityUnits")]
		public EngineeringUnit DensityUnits
		{
			get
			{
				return this._DensityUnits;
			}
			set
			{
				this._DensityUnits = value;

				if (this.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					foreach (EquipmentClass Compartment in this.CompartmentCollection)
					{
						Compartment.DensityUnits = this._DensityUnits;
					}
				}
			}
		}

		[EntityImportExportAttribute("MASSUNITS", 100, "MassUnits")]
		public EngineeringUnit MassUnits
		{
			get
			{
				return this._MassUnits;
			}
			set
			{
				this._MassUnits = value;

				if (this.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					foreach (EquipmentClass Compartment in this.CompartmentCollection)
					{
						Compartment.MassUnits = this._MassUnits;
					}
				}
			}
		}

		[QueryWriterField("Volume Decimal Places", "tblEquipment.VolumeDecimalPlaces")]
		[EntityImportExportAttribute("VOLUMEDECIMALPLACES", 150, "VolumeDecimalPlaces")]
		public short VolumeDecimalPlaces
		{
			get
			{
				return this._VolumeDecimalPlaces;
			}
			set
			{
				this._VolumeDecimalPlaces = value;

				if (!this._LowStockWarning.Format.IsReadOnly)
				{
					this._LowStockWarning.Format.NumberDecimalDigits = value;
				}
				if (!this._Volume.Format.IsReadOnly)
				{
					this._Volume.Format.NumberDecimalDigits = value;
				}
				if (!this._Capacity.Format.IsReadOnly)
				{
					this._Capacity.Format.NumberDecimalDigits = value;
				}
				if (!this._SafeFill.Format.IsReadOnly)
				{
					this._SafeFill.Format.NumberDecimalDigits = value;
				}

				if (this.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					foreach (EquipmentClass Compartment in this.CompartmentCollection)
					{
						Compartment.VolumeDecimalPlaces = this._VolumeDecimalPlaces;
					}
				}
			}
		}

		[QueryWriterField("Temperature Decimal Places", "tblEquipment.TemperatureDecimalPlaces")]
		[EntityImportExportAttribute("TEMPERATUREDECIMALPLACES", 180, "TemperatureDecimalPlaces")]
		public short TemperatureDecimalPlaces
		{
			get
			{
				return this._TemperatureDecimalPlaces;
			}
			set
			{
				this._TemperatureDecimalPlaces = value;

				if (this.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					foreach (EquipmentClass Compartment in this.CompartmentCollection)
					{
						Compartment.TemperatureDecimalPlaces = this._TemperatureDecimalPlaces;
					}
				}
			}
		}

		[QueryWriterField("Density Decimal Places", "tblEquipment.DensityDecimalPlaces")]
		[EntityImportExportAttribute("DENSITYDECIMALPLACES", 150, "DensityDecimalPlaces")]
		public short DensityDecimalPlaces
		{
			get
			{
				return this._DensityDecimalPlaces;
			}
			set
			{
				this._DensityDecimalPlaces = value;

				if (this.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					foreach (EquipmentClass Compartment in this.CompartmentCollection)
					{
						Compartment.DensityDecimalPlaces = this._DensityDecimalPlaces;
					}
				}
			}
		}

		[QueryWriterField("Mass Decimal Places", "tblEquipment.MassDecimalPlaces")]
		[EntityImportExportAttribute("MASSDECIMALPLACES", 150, "MassDecimalPlaces")]
		public short MassDecimalPlaces
		{
			get
			{
				return this._MassDecimalPlaces;
			}
			set
			{
				this._MassDecimalPlaces = value;

				if (this.Type != EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					foreach (EquipmentClass Compartment in this.CompartmentCollection)
					{
						Compartment.MassDecimalPlaces = this._MassDecimalPlaces;
					}
				}
			}
		}

		[QueryWriterField("Equipment Sequence")]
		[EntityImportExportAttribute("EQUIPMENTSEQUENCE", 130, "EquipmentSequence")]
		public string EquipmentSequence { get { return this._EquipmentSequence; } set {
			this._EquipmentSequence = value; } }

		  [QueryWriterField("ScullyRequired", "tblEquipment.ScullyRequired")]
		  [EntityImportExportAttribute("SCULLYREQUIRED", 80, "SCULLYREQUIRED")]
		  public bool ScullyRequired
		  {
				get { return this._ScullyRequired; }
				set
				{
					 this._ScullyRequired = value;
				}
		  }
		  [QueryWriterField("Locked Out", "tblEquipment.LockedOut")]
		[EntityImportExportAttribute("LOCKEDOUT", 80, "LockedOut")]
		public bool LockedOut { get { return this._LockedOut; } set {
			this._LockedOut = value; } }

		[QueryWriterField("Locked Out Reason", "tblEquipment.LockedOutReason")]
		[EntityImportExportAttribute("LOCKEDOUTREASON", 150, "LockedOutReason")]
		public string LockedOutReason { get { return this._LockedOutReason; } set {
			this.SetString("Locked Out Reason", 80, value, ref this._LockedOutReason); } }

		[QueryWriterField("Locked Out Date", "tblEquipment.LockedOutDate")]
		[EntityImportExportAttribute("LOCKEDOUTDATE", 100, "LockedOutDate")]
		public Date LockedOutDateTime { get { return this._LockedOutDate; } set {
			this._LockedOutDate = value; } }

		[XmlIgnore]
		public string LockedOutDate { get { return this._LockedOutDate.ToString(); } set {
			this.SetDate("Locked Out Date", value, ref this._LockedOutDate); } }

		[QueryWriterField("Serial Number")]
		[EntityImportExportAttribute("SERIALNUMBER", 120, "SerialNumber")]
		public string SerialNumber { get { return this._SerialNumber; } set {
			this.SetString("Serial Number", 30, value, ref this._SerialNumber); } }

		[QueryWriterField("Company Equipment ID")]
		[EntityImportExportAttribute("COMPANYEQUIPMENTID", 150, "CompanyEquipmentID")]
		public string CompanyEquipmentID { get { return this._CompanyEquipmentID; } set {
			this.SetString("Company Equipment ID", 30, value, ref this._CompanyEquipmentID); } }

		[QueryWriterField("Company ID", "EquipmentCompany.ID")]
		[EntityImportExportAttribute("COMPANYID", 100, "CompanyID")]
		public string CompanyID { get { return this._CompanyID; } set {
			this._CompanyID = value; } }

		[QueryWriterField("Product ID", false)]
		[EntityImportExportAttribute("PRODUCTID", 100, "ProductID")]
		public string ProductID { get { return this._ProductID; } set {
			this._ProductID = value; } }

		[QueryWriterField("Fuel Card ID", false)]
		[EntityImportExportAttribute("FUELCARDID", 180, "FuelCardID")]
		public string FuelCardID { get { return this._FuelCardID; } set {
			this._FuelCardID = value; } }

		[QueryWriterField("Asset Tracking Device ID", false)]
		[EntityImportExportAttribute("ASSETTRACKINGDEVICEID", 180, "AssetTrackingDeviceID")]
		public string AssetTrackingDeviceID
		{
			get { return this.assetTrackingDeviceId; }
			set
			{
				this.assetTrackingDeviceId = value;
			}
		}

		[QueryWriterField("Truck Card Number")]
		[EntityImportExportAttribute("TRUCKCARDNUMBER", 120, "TruckCardNumber")]
		public string TruckCardNumber { get { return this._TruckCardNumber; } set {
			this.SetString("Truck Card Number", 30, value, ref this._TruckCardNumber); } }

		[QueryWriterField("Company Name", "EquipmentCompany.Name")]
		[EntityImportExportAttribute("COMPANYNAME", 150, "CompanyName")]
		public string CompanyName { get { return this._CompanyName; } set {
			this._CompanyName = value; } }

		[QueryWriterField("Equipment Type Name", "D.EqTypeName")]
		public string EqTypeName { get { return this._EquipmentTypeID; } set {
			this._EquipmentTypeID = value; } }

		[QueryWriterField("Company Address", "EquipmentCompany.Address1")]
		[EntityImportExportAttribute("COMPANYADDRESS", 150, "COMPANYADDRESS")]
		public string CompanyAddress { get { return this._CompanyAddress; } set {
			this._CompanyAddress = value; } }

		[QueryWriterField("Company City", "EquipmentCompany.City")]
		[EntityImportExportAttribute("COMPANYCITY", 100, "CompanyCity")]
		public string CompanyCity { get { return this._CompanyCity; } set {
			this._CompanyCity = value; } }

		[QueryWriterField("Company State", "EquipmentCompany.State")]
		[EntityImportExportAttribute("COMPANYSTATE", 100, "CompanyState")]
		public string CompanyState { get { return this._CompanyState; } set {
			this._CompanyState = value; } }

		[QueryWriterField("Rated GPM")]
		[EntityImportExportAttribute("RATEDGPM", 80, "RatedGPM")]
		public double RatedGPM { get { return this._RatedGPM; } set {
			this._RatedGPM = value; } }

		[QueryWriterField("Actual GPM")]
		[EntityImportExportAttribute("ACTUALGPM", 80, "ActualGPM")]
		public double ActualGPM { get { return this._ActualGPM; } set {
			this._ActualGPM = value; } }

		[QueryWriterField("Fuel Additive")]
		[EntityImportExportAttribute("FUELADDITIVEFLAG", 50, "FuelAdditiveFlag")]
		public bool FuelAdditiveFlag { get { return this._FuelAdditiveFlag; } set {
			this._FuelAdditiveFlag = value; } }

		[QueryWriterField("Source Equipment")]
		[EntityImportExportAttribute("SECONDARYSTORAGE", 50, "SecondaryStorageFlag")]
		public bool SecondaryStorageFlag { get { return this._SecondaryStorageFlag; } set {
			this._SecondaryStorageFlag = value; } }

		[QueryWriterField("Status Description", "MaintenanceReason")]
		public string StatusDescription { get { return this._StatusDescription; } set {
			this._StatusDescription = value; } }

		[QueryWriterField("Return To Service Date", "EstReturnToServiceDate")]
		[XmlIgnore]
		public string ReturnToServiceDate { get { return (this._ReturnToServiceDate.Value == DateTimeOffset.MinValue) ? null : this._ReturnToServiceDate.ToString(); } set {
			this.SetDate("Return To Service Date", value, ref this._ReturnToServiceDate); } }

		public Date ReturnToServiceDateObject { get { return this._ReturnToServiceDate; } set {
			this._ReturnToServiceDate = value; } }

		[QueryWriterField("Manufacture Date")]
		[EntityImportExportAttribute("MANUFACTUREDATE", 100, "ManufactureDate")]
		[XmlIgnore]
		public string ManufactureDate { get { return (this._ManufactureDate.Value == DateTimeOffset.MinValue) ? null : this._ManufactureDate.ToString(); } set {
			this.SetDate("Manufacture Date", value, ref this._ManufactureDate); } }

		[QueryWriterField("Installation Date")]
		[EntityImportExportAttribute("INSTALLATIONDATE", 100, "InstallationDate")]
		[XmlIgnore]
		public string InstallationDate { get { return (this._InstallationDate.Value == DateTimeOffset.MinValue) ? null : this._InstallationDate.ToString(); } set {
			this.SetDate("Installation Date", value, ref this._InstallationDate); } }

		[QueryWriterField("Inspection Date")]
		[EntityImportExportAttribute("INSPECTIONDATE", 100, "InspectionDate")]
		[XmlIgnore]
		public string InspectionDate { get { return (this._InspectionDate.Value == DateTimeOffset.MinValue) ? null : this._InspectionDate.ToString(); } set {
			this.SetDate("Inspection Date", value, ref this._InspectionDate); } }

		[QueryWriterField("Calibration Date")]
		[EntityImportExportAttribute("CALIBRATIONDATE", 100, "CalibrationDate")]
		[XmlIgnore]
		public string CalibrationDate { get { return (this._CalibrationDate.Value == DateTimeOffset.MinValue) ? null : this._CalibrationDate.ToString(); } set {
			this.SetDate("Calibration Date", value, ref this._CalibrationDate); } }

		[QueryWriterField("QC Date")]
		[XmlIgnore]
		public string QCDate { get { return (this._QCDate.Value == DateTimeOffset.MinValue) ? null : this._QCDate.ToString(); } set {
			this.SetDate("QC Date", value, ref this._QCDate); } }

		[QueryWriterField("Maintenance Note", "E.Memo")]
		public string MaintenanceNote { get { return this._MaintenanceNote; } set {
			this._MaintenanceNote = value; } }

		[QueryWriterField("QC Note", "F.Memo")]
		public string QCNote { get { return this._QCNote; } set {
			this._QCNote = value; } }

		[QueryWriterField("In Service Flag")]
		public bool InServiceFlag { get { return this._InServiceFlag; } set {
			this._InServiceFlag = value; } }

		[QueryWriterField("Managed Equipment Flag")]
		[EntityImportExportAttribute("MANAGEMENTEQUIPMENT", 50, "ManagedEquipmentFlag")]
		public bool ManagedEquipmentFlag { get { return this._ManagedEquipmentFlag; } set {
			this._ManagedEquipmentFlag = value; } }

		[QueryWriterField("Fueling Type")]
		[EntityImportExportAttribute("FUELINGTYPE", 20, "FuelingType")]
		public FUELING_TYPES FuelingType { get { return this._FuelingType; } set {
			this._FuelingType = value; } }

		[QueryWriterField("Maintenance Change Date", "E.ChangeDate")]
		[XmlIgnore]
		public string MaintenanceChangeDate { get { return this._MaintenanceChangeDate.ToString(); } private set { ; } }

		[XmlIgnore]
		public Date MaintenanceChangeDateObject { get { return this._MaintenanceChangeDate; } private set { ; } }

		[QueryWriterField("Maintenance Operator ID", "E.OperatorID")]
		[XmlIgnore]
		public string MaintenanceOperatorID { get { return this._MaintenanceOperatorID; } private set { ; } }

		[QueryWriterField("Maintenance Work Order", "E.WorkOrder")]
		[XmlIgnore]
		public string MaintenanceWorkOrder { get { return this._MaintenanceWorkOrder; } private set { ; } }

		[QueryWriterField("Maintenance Created Date", "E.CreatedDate")]
		[XmlIgnore]
		public string MaintenanceCreatedDate { get { return this._MaintenanceCreatedDate.ToString(); } private set { ; } }

		[XmlIgnore]
		public Date MaintenanceCreatedDateObject { get { return this._MaintenanceCreatedDate; } private set { ; } }

		[QueryWriterField("Maintenance Created By", "E.CreatedBy")]
		[XmlIgnore]
		public string MaintenanceCreatedBy { get { return this._MaintenanceCreatedBy; } private set { ; } }

		[QueryWriterField("Maintenance Updated Date", "E.UpdatedDate")]
		[XmlIgnore]
		public string MaintenanceUpdatedDate { get { return this._MaintenanceUpdatedDate.ToString(); } private set { ; } }

		[XmlIgnore]
		public Date MaintenanceUpdatedDateObject { get { return this._MaintenanceUpdatedDate; } private set { ;} }

		[QueryWriterField("Maintenance Updated By", "E.UpdatedBy")]
		[XmlIgnore]
		public string MaintenanceUpdatedBy { get { return this._MaintenanceUpdatedBy; } private set { ; } }

		[EntityImportExportAttribute("QCDUEDATE", 100, "QualityCreatedDate")]
		[XmlIgnore]
		public string QualityCreatedDate { get { return this._QualityCreatedDate.ToString(); } private set { ; } }

		[XmlIgnore]
		public Date QualityCreatedDateObject { get { return this._QualityCreatedDate; } private set { ; } }

		[QueryWriterField("Quality Created By", "QualityCreatedBy")]
		[XmlIgnore]
		public string QualityCreatedBy { get { return this._QualityCreatedBy; } private set { ; } }

		[QueryWriterField("Quality Updated Date", "QualityUpdatedDate")]
		[XmlIgnore]
		public string QualityUpdatedDate { get { return this._QualityUpdatedDate.ToString(); } private set { ; } }

		[XmlIgnore]
		public Date QualityUpdatedDateObject { get { return this._QualityUpdatedDate; } private set { ; } }

		[QueryWriterField("Quality Updated By", "QualityUpdatedBy")]
		[XmlIgnore]
		public string QualityUpdatedBy { get { return this._QualityUpdatedBy; } private set { ; } }

		public string VolumeHostName
		{
			get
			{
				URL Url = new URL(this.volumeProcessVariable.URL);
				return Url.HostName;
			}
			set
			{
				URL Url = new URL(this.volumeProcessVariable.URL);
				Url.HostName = value;
				this.volumeProcessVariable.URL = Url.ToString();
			}
		}
		[EntityImportExportAttribute("VOLUMEURL", 150, "VolumeURL")]
		public string VolumeURL
		{
			get { return this.volumeProcessVariable.URL; }
			set
			{
				this.volumeProcessVariable.URL = value;
				URL Url = new URL(this.VolumeProcessVariable.URL);
				string[] pathStrings = Url.Path.Split(new char[] { '/' });

				if (pathStrings.Length > 1)
				{
					this.volumeProcessVariable.ProgID = pathStrings[0];
				}
				else
				{
					this.volumeProcessVariable.ProgID = "";
				}
			}
		}

		[XmlIgnoreAttribute]
		public string VolumeProgID
		{
			get { return this.volumeProcessVariable.ProgID; }
			set {
				this.volumeProcessVariable.ProgID = value; }
		}

		[EntityImportExportAttribute("VOLUMEITEM", 80, "VolumeItemID")]
		public string VolumeItemID
		{
			get { return this.volumeProcessVariable.OPCItemID; }
			set {
				this.volumeProcessVariable.OPCItemID = value; }
		}

		[XmlIgnoreAttribute]
		public ProcessVariableClass VolumeProcessVariable
		{
			get { return this.volumeProcessVariable; }
			set {
				this.volumeProcessVariable = value; }
		}

		  //Need to update how meters are exposed for import/export now that equipment 
		  //can support multiple meters
		  /*
		/// <summary>
		/// Exposes the MeterID contained within the Equipment.Meter object
		/// so that it can be imported or exported.
		/// </summary>
		[EntityImportExportAttribute("METERID", 150, "ImportExportMeterID")]
		public string ImportExportMeterID
		{
			get
			{
				if (this.Meter != null)
				{
					return this.Meter.ID;
				}

				return string.Empty;
			}

			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (this.Meter == null)
					{
						this.Meter = new MeterClass();
					}

					this.Meter.ID = value;
				}
			}
		}

		/// <summary>
		/// Exposes the NumberOfDigits contained within the Equipment.Meter object
		/// so that it can be imported or exported.
		/// </summary>
		[EntityImportExportAttribute("NUMBEROFDIGITS", 100, "ImportExportMeterNumberOfDigits")]
		public int ImportExportMeterNumberOfDigits
		{
			get
			{
				if (this.Meter != null)
				{
					return this.Meter.NumberOfDigits;
				}

				return default(int);
			}

			set
			{
				if (value != default(int))
				{
					if (this.Meter == null)
					{
						this.Meter = new MeterClass();
					}

					this.Meter.NumberOfDigits = value;
				}
			}
		}

		/// <summary>
		/// Exposes the RotatesBackwardsFlag contained within the Equipment.Meter object
		/// so that it can be imported or exported.
		/// </summary>
		[EntityImportExportAttribute("ROTATESBACKWARDSFLAG", 140, "ImportExportMeterRotatesBackwardsFlag")]
		public bool ImportExportMeterRotatesBackwardsFlag
		{
			get
			{
				if (this.Meter != null)
				{
					return this.Meter.RotatesBackwardsFlag;
				}

				return default(bool);
			}

			set
			{
				if (value != default(bool))
				{
					if (this.Meter == null)
					{
						this.Meter = new MeterClass();
					}

					this.Meter.RotatesBackwardsFlag = value;
				}
			}
		}

		/// <summary>
		/// Exposes the ReceiptMeterFlag contained within the Equipment.Meter object
		/// so that it can be imported or exported.
		/// </summary>
		[EntityImportExportAttribute("RECEIPTMETERFLAG", 110, "ImportExportMeterReceiptMeterFlag")]
		public bool ImportExportMeterReceiptMeterFlag
		{
			get
			{
				if (this.Meter != null)
				{
					return this.Meter.ReceiptMeterFlag;
				}

				return default(bool);
			}
			set
			{
				if (value != default(bool))
				{
					if (this.Meter == null)
					{
						this.Meter = new MeterClass();
					}

					this.Meter.ReceiptMeterFlag = value;
				}
			}
		}

		/// <summary>
		/// Exposes the MeterGuid (IdentityGuid) contained within the Equipment.Meter object
		/// so that it can be imported or exported.
		/// </summary>
		[EntityImportExportAttribute("METERGUID", 200, "ImportExportMeterGuid")]
		public Guid ImportExportMeterGuid
		{
			get
			{
				if (this.Meter != null)
				{
					return this.Meter.IdentityGuid;
				}

				return default(Guid);
			}
			set
			{
				if (value != default(Guid))
				{
					if (this.Meter == null)
					{
						this.Meter = new MeterClass();
					}

					this.Meter.IdentityGuid = value;
				}
			}
		}

		/// <summary>
		/// Exposes the SiteGuid contained within the Equipment.Meter object
		/// so that it can be imported or exported.
		/// </summary>
		[EntityImportExportAttribute("METERSITEGUID", 200, "ImportExportMeterSiteGuid")]
		public Guid ImportExportMeterSiteGuid
		{
			get
			{
				if (this.Meter != null)
				{
					return this.Meter.SiteGuid;
				}

				return default(Guid);
			}
			set
			{
				if (value != default(Guid))
				{
					if (this.Meter == null)
					{
						this.Meter = new MeterClass();
					}

					this.Meter.SiteGuid = value;
				}
			}
		}
		  */


		[XmlIgnoreAttribute]
		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] Descriptors ={	EquipmentLockOutEventDescriptor,
																		EquipmentLockedOutAlarmDescriptor
																	};
				return Descriptors;
			}
		}

		public AlarmAndEventLogClass LockOutEvent
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(EquipmentLockOutEventDescriptor);
				AlarmAndEventLog.AssociatedData = this.ID;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass LockedOutAlarm
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(EquipmentLockedOutAlarmDescriptor);
				AlarmAndEventLog.AssociatedData = this.ID;
				return AlarmAndEventLog;
			}
		}

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.EQUIPMENT; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public string EquipmentToolTip
		{
			get
			{
				string ToolTip = "";

				if (this.Type == EQUIPMENT_TYPE.COMPARTMENT_TYPE)
				{
					ToolTip = "Capacity " + this.Capacity + " Safe Fill" + this.SafeFill;
				}

				else
				{
					if (this._Description != "")
					{
						ToolTip = this._Description;
					}
					else
					{
						ToolTip = this._ID;
					}

					if (this._Make != "")
					{
						ToolTip += ", " + this._Make;
					}
					if (this._Model != "")
					{
						ToolTip += ", " + this._Model;
					}
					if (this._SerialNumber != "")
					{
						ToolTip += ", " + this._SerialNumber;
					}
					if (this._TruckCardNumber != "")
					{
						ToolTip += ", " + this._TruckCardNumber;
					}
				}

				return ToolTip;
			}
		}

		public string CompanyToolTip
		{
			get
			{
				string ToolTip = "";

				if (this._CompanyName != "")
				{
					ToolTip = this._CompanyName;
				}
				else
				{
					ToolTip = this._CompanyID;
				}

				if (this._CompanyAddress != "")
				{
					ToolTip += ", " + this._CompanyAddress;
				}
				if (this._CompanyCity != "")
				{
					ToolTip += ", " + this._CompanyCity;
				}
				if (this._CompanyState != "")
				{
					ToolTip += ", " + this._CompanyState;
				}

				return ToolTip;
			}
		}

		public Guid MasterRecordGuid { get { return this._MasterRecordGuid; } set {
			this._MasterRecordGuid = value; } }

		// EquipmentType Composite Properties to avoid holding a reference to an EquipmentType
		[EntityImportExportAttribute("MULTICOMPARTMENT", 120, "D.MultiCompartment")]
		public bool IsMultiCompartment { get { return this._IsMultiCompartment; } set {
			this._IsMultiCompartment = value; } }

		public void InsertSQL(SqlCommand cmd)
		{
				cmd.CommandText = "INSERT INTO tblEquipment " +
								"(EquipmentGuid," +
								"SiteGuid," +
								"_MasterRecordGuid," +
								"ID," +
								"Description," +
								"EquipmentTypeGuid," +
								"Make," +
								"Model," +
								"Year," +
								"IssPtNum," +
								"CompanyGuid," +
								"Fixed," +
								"StorageType," +
								"InUse," +
								"ProductGuid," +
								"FuelCardGuid," +
						"AssetTrackingDeviceGuid, " +
								"FixedVolume," +
								"IntoPlane," +
								"Mobile," +
								"AttachedTo," +
								"MediaType," +
								"Meters," +
								"DefuelMeterForwards," +
								"PulseRatio," +
								"Round," +
								"Xref," +
								"LowStockWarning," +
								"StockTrack," +
								"Totalisor1," +
								"Totalisor2," +
								"FuelingState," +
								"Volume," +
								"MeterReading," +
								"Notes," +
								"Capacity," +
								"SafeFill," +
								"VolumeUnitIndex," +
								"TemperatureUnitIndex," +
								"DensityUnitIndex," +
								"MassUnitIndex," +
								"VolumeDecimalPlaces," +
								"TemperatureDecimalPlaces," +
								"DensityDecimalPlaces," +
								"MassDecimalPlaces," +
								"ParentEquipmentGuid," +
								"EquipmentSequence," +
								"LockedOut," +
								"LockedOutReason," +
								"LockedOutDate," +
								"SerialNumber," +
								"CompanyEquipmentID," +
								"TruckCardNumber," +
								"HiddenDate," + 
								"CreatedDate," +
								"CreatedBy," +
								"UpdatedDate," +
								"UpdatedBy," +
								"RatedGPM ," +
								"ActualGPM," +
								"FuelAdditiveFlag," +
								"ManufactureDate ," +
								"InstallationDate ," +
								"InspectionDate," +
								"CalibrationDate," +
								"QCDate," +
								"SecondaryStorageFlag," +
								"ManagedEquipmentFlag," +
								"FuelingType, " +
								"AssignedToMeterGuid, "+
								"ScullyRequired";

			for (int nextItem = 1; nextItem <= 24; nextItem++)
			{
				cmd.CommandText += ", UserData" + nextItem.ToString();
			}

			cmd.CommandText += ") VALUES (" +
						  "@EquipmentGuid," +
						  "@SiteGuid," +
						  "@MasterRecordGuid," +
					"@ID," +
					"@Description," +
					"@EquipmentTypeGuid," +
					"@Make," +
					"@Model," +
					"@Year," +
					"@IssPtNum," +
					"@CompanyGuid," +
					"@Fixed," +
					"@StorageType," +
					"@InUse," +
					"@ProductGuid," +
					"@FuelCardGuid," +
					"@AssetTrackingDeviceGuid, " +
					"@FixedVolume," +
					"@IntoPlane," +
					"@Mobile," +
					"@AttachedTo," +
					"@MediaType," +
					"@Meters," +
					"@DefuelMeterForwards," +
					"@PulseRatio," +
					"@Round," +
					"@Xref," +
					"@LowStockWarning," +
					"@StockTrack," +
					"@Totalisor1," +
					"@Totalisor2," +
					"@FuelingState," +
					"@Volume," +
					"@MeterReading," +
					"@Notes," +
					"@Capacity," +
					"@SafeFill," +
					"@VolumeUnits," +
					"@TemperatureUnits," +
					"@DensityUnits," +
					"@MassUnits," +
					"@VolumeDecimalPlaces," +
					"@TemperatureDecimalPlaces," +
					"@DensityDecimalPlaces," +
					"@MassDecimalPlaces," +
					"@ParentEquipmentGuid," +
					"@EquipmentSequence, " +
					"@LockedOut," +
					"@LockedOutReason," +
					"@LockedOutDate," +
					"@SerialNumber," +
					"@CompanyEquipmentID," +
					"@TruckCardNumber," +
						  "@HiddenDate," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@RatedGPM," +
					"@ActualGPM," +
					"@FuelAdditiveFlag," +
					"@ManufactureDate," +
					"@InstallationDate," +
					"@InspectionDate," +
					"@CalibrationDate," +
					"@QCDate," +
					"@SecondaryStorageFlag," +
					"@ManagedEquipmentFlag," +
					"@FuelingType," +
					"@AssignedToMeterGuid, "+
						  "@ScullyRequired";

			for (int nextItem = 1; nextItem <= 24; nextItem++)
			{
				cmd.CommandText += ", @UserData" + nextItem.ToString();
			}

			cmd.CommandText += ")";
				
				cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@MasterRecordGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@EquipmentTypeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Make", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@Model", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Year", SqlDbType.Int);
			cmd.Parameters.Add("@IssPtNum", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Fixed", SqlDbType.Bit);
			cmd.Parameters.Add("@StorageType", SqlDbType.NVarChar, 2);
			cmd.Parameters.Add("@InUse", SqlDbType.Bit);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@FixedVolume", SqlDbType.Bit);
			cmd.Parameters.Add("@IntoPlane", SqlDbType.Bit);
			cmd.Parameters.Add("@Mobile", SqlDbType.Bit);
			cmd.Parameters.Add("@AttachedTo", SqlDbType.NVarChar, 6);
			cmd.Parameters.Add("@MediaType", SqlDbType.Char, 1);
			cmd.Parameters.Add("@Meters", SqlDbType.Int);
			cmd.Parameters.Add("@DefuelMeterForwards", SqlDbType.Bit);
			cmd.Parameters.Add("@PulseRatio", SqlDbType.Float);
			cmd.Parameters.Add("@Round", SqlDbType.Bit);
			cmd.Parameters.Add("@Xref", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@LowStockWarning", SqlDbType.Float);
			cmd.Parameters.Add("@StockTrack", SqlDbType.Bit);
			cmd.Parameters.Add("@Totalisor1", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@Totalisor2", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@FuelingState", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@Volume", SqlDbType.Float);
			cmd.Parameters.Add("@MeterReading", SqlDbType.Float);
			cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 1000);
			cmd.Parameters.Add("@Capacity", SqlDbType.Float);
			cmd.Parameters.Add("@SafeFill", SqlDbType.Float);
			cmd.Parameters.Add("@VolumeUnits", SqlDbType.Int);
			cmd.Parameters.Add("@TemperatureUnits", SqlDbType.Int);
			cmd.Parameters.Add("@DensityUnits", SqlDbType.Int);
			cmd.Parameters.Add("@MassUnits", SqlDbType.Int);
			cmd.Parameters.Add("@VolumeDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@TemperatureDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@DensityDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@MassDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@ParentEquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EquipmentSequence", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@LockedOut", SqlDbType.Bit);
			cmd.Parameters.Add("@LockedOutReason", SqlDbType.NVarChar, 80);
			cmd.Parameters.Add("@LockedOutDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@CompanyEquipmentID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@TruckCardNumber", SqlDbType.NVarChar, 32);
			 cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@RatedGPM", SqlDbType.Float);
			cmd.Parameters.Add("@ActualGPM", SqlDbType.Float);
			cmd.Parameters.Add("@FuelAdditiveFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@ManufactureDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@InstallationDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@InspectionDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CalibrationDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@QCDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SecondaryStorageFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@ManagedEquipmentFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@FuelingType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@AssignedToMeterGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ScullyRequired", SqlDbType.Bit);

				for (int nextItem = 1; nextItem <= 24; nextItem++)
			{
				cmd.Parameters.Add("@UserData" + nextItem.ToString(), SqlDbType.NVarChar, 60);
			}

			this.IdentityGuid = Guid.NewGuid();
			cmd.Parameters["@EquipmentGuid"].Value = this.IdentityGuid;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;

				//This query can only be used to create master record versions.
			this.MasterRecordGuid = this.IdentityGuid;

			cmd.Parameters["@MasterRecordGuid"].Value = this.MasterRecordGuid;
			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@Description"].Value = this.Description;

			if (this.EquipmentTypeGuid != Guid.Empty)
			{
				cmd.Parameters["@EquipmentTypeGuid"].Value = this.EquipmentTypeGuid;
			}
			else
			{
				cmd.Parameters["@EquipmentTypeGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@Make"].Value = this.Make;
			cmd.Parameters["@Model"].Value = this.Model;
			cmd.Parameters["@Year"].Value = this.Year;
			cmd.Parameters["@IssPtNum"].Value = this.IssPtNum;

			if (this.CompanyGuid != Guid.Empty)
			{
				cmd.Parameters["@CompanyGuid"].Value = this.CompanyGuid;
			}
			else
			{
				cmd.Parameters["@CompanyGuid"].Value = DBNull.Value;
			}

			if (this.Fixed)
			{
				cmd.Parameters["@Fixed"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Fixed"].Value = 0;
			}

			cmd.Parameters["@StorageType"].Value = this.StorageType;

			if (this.InUse)
			{
				cmd.Parameters["@InUse"].Value = 1;
			}
			else
			{
				cmd.Parameters["@InUse"].Value = 0;
			}

			if (this.ProductGuid != Guid.Empty)
			{
				cmd.Parameters["@ProductGuid"].Value = this.ProductGuid;
			}
			else
			{
				cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@FuelCardGuid"].Value = DBNull.Value;
			if (this.FuelCardGuid != Guid.Empty)
			{
				cmd.Parameters["@FuelCardGuid"].Value = this.FuelCardGuid;
			}

			cmd.Parameters["@AssetTrackingDeviceGuid"].Value = DBNull.Value;
			if (this.AssetTrackingDeviceGuid != Guid.Empty)
			{
				cmd.Parameters["@AssetTrackingDeviceGuid"].Value = this.AssetTrackingDeviceGuid;
			}

			if (this.FixedVolume)
			{
				cmd.Parameters["@FixedVolume"].Value = 1;
			}
			else
			{
				cmd.Parameters["@FixedVolume"].Value = 0;
			}

			if (this.IntoPlane)
			{
				cmd.Parameters["@IntoPlane"].Value = 1;
			}
			else
			{
				cmd.Parameters["@IntoPlane"].Value = 0;
			}

			if (this.Mobile)
			{
				cmd.Parameters["@Mobile"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Mobile"].Value = 0;
			}

			cmd.Parameters["@AttachedTo"].Value = this.AttachedTo;
			cmd.Parameters["@MediaType"].Value = this.MediaType;
			cmd.Parameters["@Meters"].Value = this.Meters;

			if (this.DefuelMeterForwards)
			{
				cmd.Parameters["@DefuelMeterForwards"].Value = 1;
			}
			else
			{
				cmd.Parameters["@DefuelMeterForwards"].Value = 0;
			}

			cmd.Parameters["@PulseRatio"].Value = this.PulseRatio;

			if (this.Round)
			{
				cmd.Parameters["@Round"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Round"].Value = 0;
			}

			cmd.Parameters["@Xref"].Value = this.Xref;
			cmd.Parameters["@LowStockWarning"].Value = this._LowStockWarning.SIValue;

			if (this.StockTrack)
			{
				cmd.Parameters["@StockTrack"].Value = 1;
			}
			else
			{
				cmd.Parameters["@StockTrack"].Value = 0;
			}

			cmd.Parameters["@Totalisor1"].Value = this.Totalisor1;
			cmd.Parameters["@Totalisor2"].Value = this.Totalisor2;
			cmd.Parameters["@FuelingState"].Value = this.FuelingState;
			cmd.Parameters["@Volume"].Value = this._Volume.SIValue;
			cmd.Parameters["@MeterReading"].Value = this.MeterReading;
			cmd.Parameters["@Notes"].Value = this.Notes;
			cmd.Parameters["@Capacity"].Value = this._Capacity.SIValue;
			cmd.Parameters["@SafeFill"].Value = this._SafeFill.SIValue;
			cmd.Parameters["@VolumeUnits"].Value = (int)this.VolumeUnits;
			cmd.Parameters["@TemperatureUnits"].Value = (int)this.TemperatureUnits;
			cmd.Parameters["@DensityUnits"].Value = (int)this.DensityUnits;
			cmd.Parameters["@MassUnits"].Value = (int)this.MassUnits;
			cmd.Parameters["@VolumeDecimalPlaces"].Value = this.VolumeDecimalPlaces;
			cmd.Parameters["@TemperatureDecimalPlaces"].Value = this.TemperatureDecimalPlaces;
			cmd.Parameters["@DensityDecimalPlaces"].Value = this.DensityDecimalPlaces;
			cmd.Parameters["@MassDecimalPlaces"].Value = this.MassDecimalPlaces;

			if (this.ParentEquipmentGuid != Guid.Empty)
			{
				cmd.Parameters["@ParentEquipmentGuid"].Value = this.ParentEquipmentGuid;
			}
			else
			{
				cmd.Parameters["@ParentEquipmentGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@EquipmentSequence"].Value = this.EquipmentSequence;

			if (this.LockedOut)
			{
				cmd.Parameters["@LockedOut"].Value = 1;
			}
			else
			{
				cmd.Parameters["@LockedOut"].Value = 0;
			}

			cmd.Parameters["@LockedOutReason"].Value = this.LockedOutReason;

			if (this._LockedOutDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@LockedOutDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@LockedOutDate"].Value = this._LockedOutDate.Value;
			}

			cmd.Parameters["@SerialNumber"].Value = this.SerialNumber;

			if (!string.IsNullOrEmpty(this.CompanyEquipmentID))
			{
				cmd.Parameters["@CompanyEquipmentID"].Value = this.CompanyEquipmentID;
			}
			else
			{
				cmd.Parameters["@CompanyEquipmentID"].Value = DBNull.Value;
			}

			cmd.Parameters["@TruckCardNumber"].Value = this.TruckCardNumber;
				cmd.Parameters["@HiddenDate"].Value = this.HiddenDate ?? (object)DBNull.Value;
			cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;

			cmd.Parameters["@RatedGPM"].Value = this.RatedGPM;
			cmd.Parameters["@ActualGPM"].Value = this.ActualGPM;

			if (this.FuelAdditiveFlag)
			{
				cmd.Parameters["@FuelAdditiveFlag"].Value = 1;
			}
			else
			{
				cmd.Parameters["@FuelAdditiveFlag"].Value = 0;
			}

			if (this._ManufactureDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@ManufactureDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ManufactureDate"].Value = this._ManufactureDate.Value;
			}

			if (this._InstallationDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@InstallationDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@InstallationDate"].Value = this._InstallationDate.Value;
			}

			if (this._InspectionDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@InspectionDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@InspectionDate"].Value = this._InspectionDate.Value;
			}

			if (this._CalibrationDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@CalibrationDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@CalibrationDate"].Value = this._CalibrationDate.Value;
			}

			if (this._QCDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@QCDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@QCDate"].Value = this._QCDate.Value;
			}

			if (this.SecondaryStorageFlag)
			{
				cmd.Parameters["@SecondaryStorageFlag"].Value = 1;
			}
			else
			{
				cmd.Parameters["@SecondaryStorageFlag"].Value = 0;
			}

			if (this.ManagedEquipmentFlag)
			{
				cmd.Parameters["@ManagedEquipmentFlag"].Value = 1;
			}
			else
			{
				cmd.Parameters["@ManagedEquipmentFlag"].Value = 0;
			}

			cmd.Parameters["@FuelingType"].Value = (int)this.FuelingType;

			for (int nextItem = 1; nextItem <= 24; nextItem++)
			{
				cmd.Parameters["@UserData" + nextItem.ToString()].Value = this.UserData[nextItem - 1];
			}

			if(this.AssignedToMeterGuid != Guid.Empty)
			{
				cmd.Parameters["@AssignedToMeterGuid"].Value = this.AssignedToMeterGuid;
			}
			else
			{
				cmd.Parameters["@AssignedToMeterGuid"].Value = DBNull.Value;
			}

				if (this.ScullyRequired)
				{
					 cmd.Parameters["@ScullyRequired"].Value = 1;
				}
				else
				{
					 cmd.Parameters["@ScullyRequired"].Value = 0;
				}
		  }

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE tblEquipment " +
						"SET SiteGuid = @SiteGuid," +
						"ID = @ID," +
						"Description = @Description," +
						"EquipmentTypeGuid = @EquipmentTypeGuid," +
						"Make = @Make," +
						"Model = @Model," +
						"Year = @Year," +
						"IssPtNum = @IssPtNum," +
						"CompanyGuid = @CompanyGuid," +
						"Fixed = @Fixed," +
						"StorageType = @StorageType," +
						"InUse = @InUse," +
						"ProductGuid = @ProductGuid," +
						"FuelCardGuid = @FuelCardGuid," +
						"AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid, " +
						"FixedVolume = @FixedVolume," +
						"IntoPlane = @IntoPlane," +
						"Mobile = @Mobile," +
						"AttachedTo = @AttachedTo," +
						"MediaType = @MediaType," +
						"Meters = @Meters," +
						"DefuelMeterForwards = @DefuelMeterForwards," +
						"PulseRatio = @PulseRatio," +
						"Round = @Round," +
						"Xref = @Xref," +
						"LowStockWarning = @LowStockWarning," +
						"StockTrack = @StockTrack," +
						"Totalisor1 = @Totalisor1," +
						"Totalisor2 = @Totalisor2," +
						"FuelingState = @FuelingState," +
						"Volume = @Volume," +
						"MeterReading = @MeterReading," +
						"Notes = @Notes," +
						"Capacity = @Capacity," +
						"SafeFill = @SafeFill," +
						"VolumeUnitIndex = @VolumeUnits," +
						"TemperatureUnitIndex = @TemperatureUnits," +
						"DensityUnitIndex = @DensityUnits," +
						"MassUnitIndex = @MassUnits," +
						"VolumeDecimalPlaces = @VolumeDecimalPlaces," +
						"TemperatureDecimalPlaces = @TemperatureDecimalPlaces," +
						"DensityDecimalPlaces = @DensityDecimalPlaces," +
						"MassDecimalPlaces = @MassDecimalPlaces," +
						"ParentEquipmentGuid = @ParentEquipmentGuid," +
						"EquipmentSequence = @EquipmentSequence, " +
						"LockedOut = @LockedOut," +
						"LockedOutReason = @LockedOutReason," +
						"LockedOutDate = @LockedOutDate," +
						"SerialNumber = @SerialNumber," +
						"CompanyEquipmentID = @CompanyEquipmentID," +
						"TruckCardNumber = @TruckCardNumber," +
								"HiddenDate = @HiddenDate," + 
						"UpdatedDate = @UpdatedDate," +
						"UpdatedBy = @UpdatedBy, " +
						"RatedGPM = @RatedGPM," +
						"ActualGPM = @ActualGPM," +
						"FuelAdditiveFlag = @FuelAdditiveFlag," +
						"ManufactureDate = @ManufactureDate," +
						"InstallationDate = @InstallationDate," +
						"InspectionDate = @InspectionDate," +
						"CalibrationDate = @CalibrationDate," +
						"QCDate = @QCDate," +
						"SecondaryStorageFlag = @SecondaryStorageFlag," +
						"ManagedEquipmentFlag = @ManagedEquipmentFlag," +
						"FuelingType = @FuelingType," + 
						"AssignedToMeterGuid = @AssignedToMeterGuid, "+
								"ScullyRequired = @ScullyRequired ";

			for (int i = 1; i <= 24; i++)
			{
				cmd.CommandText += ", UserData" + i.ToString() + " = @UserData" + i.ToString();
			}

			cmd.CommandText += " WHERE EquipmentGuid = @EquipmentGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@EquipmentTypeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Make", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@Model", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@Year", SqlDbType.Int);
			cmd.Parameters.Add("@IssPtNum", SqlDbType.NVarChar, 20);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Fixed", SqlDbType.Bit);
			cmd.Parameters.Add("@StorageType", SqlDbType.NVarChar, 2);
			cmd.Parameters.Add("@InUse", SqlDbType.Bit);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@FixedVolume", SqlDbType.Bit);
			cmd.Parameters.Add("@IntoPlane", SqlDbType.Bit);
			cmd.Parameters.Add("@Mobile", SqlDbType.Bit);
			cmd.Parameters.Add("@AttachedTo", SqlDbType.NVarChar, 6);
			cmd.Parameters.Add("@MediaType", SqlDbType.Char, 1);
			cmd.Parameters.Add("@Meters", SqlDbType.Int);
			cmd.Parameters.Add("@DefuelMeterForwards", SqlDbType.Bit);
			cmd.Parameters.Add("@PulseRatio", SqlDbType.Float);
			cmd.Parameters.Add("@Round", SqlDbType.Bit);
			cmd.Parameters.Add("@Xref", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@LowStockWarning", SqlDbType.Float);
			cmd.Parameters.Add("@StockTrack", SqlDbType.Bit);
			cmd.Parameters.Add("@Totalisor1", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@Totalisor2", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@FuelingState", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@Volume", SqlDbType.Float);
			cmd.Parameters.Add("@MeterReading", SqlDbType.Float);
			cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 1000);
			cmd.Parameters.Add("@Capacity", SqlDbType.Float);
			cmd.Parameters.Add("@SafeFill", SqlDbType.Float);
			cmd.Parameters.Add("@VolumeUnits", SqlDbType.Int);
			cmd.Parameters.Add("@TemperatureUnits", SqlDbType.Int);
			cmd.Parameters.Add("@DensityUnits", SqlDbType.Int);
			cmd.Parameters.Add("@MassUnits", SqlDbType.Int);
			cmd.Parameters.Add("@VolumeDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@TemperatureDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@DensityDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@MassDecimalPlaces", SqlDbType.TinyInt);
			cmd.Parameters.Add("@ParentEquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EquipmentSequence", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@LockedOut", SqlDbType.Bit);
			cmd.Parameters.Add("@LockedOutReason", SqlDbType.NVarChar, 80);
			cmd.Parameters.Add("@LockedOutDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SerialNumber", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@CompanyEquipmentID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@TruckCardNumber", SqlDbType.NVarChar, 32);
				cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@RatedGPM", SqlDbType.Float);
			cmd.Parameters.Add("@ActualGPM", SqlDbType.Float);
			cmd.Parameters.Add("@FuelAdditiveFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@ManufactureDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@InstallationDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@InspectionDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CalibrationDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@QCDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SecondaryStorageFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@ManagedEquipmentFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@FuelingType", SqlDbType.SmallInt);
			cmd.Parameters.Add("@AssignedToMeterGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ScullyRequired", SqlDbType.Bit);

				for (int nextItem = 1; nextItem <= 24; nextItem++)
			{
				cmd.Parameters.Add("@UserData" + nextItem.ToString(), SqlDbType.NVarChar, 60);
			}

			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@Description"].Value = this.Description;

			if (this.EquipmentTypeGuid != Guid.Empty)
			{
				cmd.Parameters["@EquipmentTypeGuid"].Value = this.EquipmentTypeGuid;
			}
			else
			{
				cmd.Parameters["@EquipmentTypeGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@Make"].Value = this.Make;
			cmd.Parameters["@Model"].Value = this.Model;
			cmd.Parameters["@Year"].Value = this.Year;
			cmd.Parameters["@IssPtNum"].Value = this.IssPtNum;

			if (this.CompanyGuid != Guid.Empty)
			{
				cmd.Parameters["@CompanyGuid"].Value = this.CompanyGuid;
			}
			else
			{
				cmd.Parameters["@CompanyGuid"].Value = DBNull.Value;
			}

			if (this.Fixed)
			{
				cmd.Parameters["@Fixed"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Fixed"].Value = 0;
			}

			cmd.Parameters["@StorageType"].Value = this.StorageType;

			if (this.InUse)
			{
				cmd.Parameters["@InUse"].Value = 1;
			}
			else
			{
				cmd.Parameters["@InUse"].Value = 0;
			}

			if (this.ProductGuid != Guid.Empty)
			{
				cmd.Parameters["@ProductGuid"].Value = this.ProductGuid;
			}
			else
			{
				cmd.Parameters["@ProductGuid"].Value = DBNull.Value;
			}

			if (this.FuelCardGuid != Guid.Empty)
			{
				cmd.Parameters["@FuelCardGuid"].Value = this.FuelCardGuid;
			}
			else
			{
				cmd.Parameters["@FuelCardGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@AssetTrackingDeviceGuid"].Value = DBNull.Value;
			if (this.AssetTrackingDeviceGuid != Guid.Empty)
			{
				cmd.Parameters["@AssetTrackingDeviceGuid"].Value = this.AssetTrackingDeviceGuid;
			}

			if (this.FixedVolume)
			{
				cmd.Parameters["@FixedVolume"].Value = 1;
			}
			else
			{
				cmd.Parameters["@FixedVolume"].Value = 0;
			}

			if (this.IntoPlane)
			{
				cmd.Parameters["@IntoPlane"].Value = 1;
			}
			else
			{
				cmd.Parameters["@IntoPlane"].Value = 0;
			}

			if (this.Mobile)
			{
				cmd.Parameters["@Mobile"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Mobile"].Value = 0;
			}

			cmd.Parameters["@AttachedTo"].Value = this.AttachedTo;
			cmd.Parameters["@MediaType"].Value = this.MediaType;
			cmd.Parameters["@Meters"].Value = this.Meters;

			if (this.DefuelMeterForwards)
			{
				cmd.Parameters["@DefuelMeterForwards"].Value = 1;
			}
			else
			{
				cmd.Parameters["@DefuelMeterForwards"].Value = 0;
			}

			cmd.Parameters["@PulseRatio"].Value = this.PulseRatio;

			if (this.Round)
			{
				cmd.Parameters["@Round"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Round"].Value = 0;
			}

			cmd.Parameters["@Xref"].Value = this.Xref;
			cmd.Parameters["@LowStockWarning"].Value = this._LowStockWarning.SIValue;

			if (this.StockTrack)
			{
				cmd.Parameters["@StockTrack"].Value = 1;
			}
			else
			{
				cmd.Parameters["@StockTrack"].Value = 0;
			}

			cmd.Parameters["@Totalisor1"].Value = this.Totalisor1;
			cmd.Parameters["@Totalisor2"].Value = this.Totalisor2;
			cmd.Parameters["@FuelingState"].Value = this.FuelingState;
			cmd.Parameters["@Volume"].Value = this._Volume.SIValue;
			cmd.Parameters["@MeterReading"].Value = this.MeterReading;
			cmd.Parameters["@Notes"].Value = this.Notes;
			cmd.Parameters["@Capacity"].Value = this._Capacity.SIValue;
			cmd.Parameters["@SafeFill"].Value = this._SafeFill.SIValue;
			cmd.Parameters["@VolumeUnits"].Value = (int)this.VolumeUnits;
			cmd.Parameters["@TemperatureUnits"].Value = (int)this.TemperatureUnits;
			cmd.Parameters["@DensityUnits"].Value = (int)this.DensityUnits;
			cmd.Parameters["@MassUnits"].Value = (int)this.MassUnits;
			cmd.Parameters["@VolumeDecimalPlaces"].Value = this.VolumeDecimalPlaces;
			cmd.Parameters["@TemperatureDecimalPlaces"].Value = this.TemperatureDecimalPlaces;
			cmd.Parameters["@DensityDecimalPlaces"].Value = this.DensityDecimalPlaces;
			cmd.Parameters["@MassDecimalPlaces"].Value = this.MassDecimalPlaces;

			if (this.ParentEquipmentGuid != Guid.Empty)
			{
				cmd.Parameters["@ParentEquipmentGuid"].Value = this.ParentEquipmentGuid;
			}
			else
			{
				cmd.Parameters["@ParentEquipmentGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@EquipmentSequence"].Value = this.EquipmentSequence;

			if (this.LockedOut)
			{
				cmd.Parameters["@LockedOut"].Value = 1;
			}
			else
			{
				cmd.Parameters["@LockedOut"].Value = 0;
			}

			cmd.Parameters["@LockedOutReason"].Value = this.LockedOutReason;

			if (this._LockedOutDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@LockedOutDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@LockedOutDate"].Value = this._LockedOutDate.Value;
			}

				if (this.ScullyRequired)
				{
					 cmd.Parameters["@ScullyRequired"].Value = 1;
				}
				else
				{
					 cmd.Parameters["@ScullyRequired"].Value = 0;
				}

				cmd.Parameters["@SerialNumber"].Value = this.SerialNumber;

			if (!string.IsNullOrEmpty(this.CompanyEquipmentID))
			{
				cmd.Parameters["@CompanyEquipmentID"].Value = this.CompanyEquipmentID;
			}
			else
			{
				cmd.Parameters["@CompanyEquipmentID"].Value = DBNull.Value; ;
			}

			cmd.Parameters["@TruckCardNumber"].Value = this.TruckCardNumber;
				cmd.Parameters["@HiddenDate"].Value = this.HiddenDate ?? (object)DBNull.Value;

			cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;

			cmd.Parameters["@RatedGPM"].Value = this.RatedGPM;
			cmd.Parameters["@ActualGPM"].Value = this.ActualGPM;

			if (this.FuelAdditiveFlag)
			{
				cmd.Parameters["@FuelAdditiveFlag"].Value = 1;
			}
			else
			{
				cmd.Parameters["@FuelAdditiveFlag"].Value = 0;
			}

			if (this._ManufactureDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@ManufactureDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ManufactureDate"].Value = this._ManufactureDate.Value;
			}

			if (this._InstallationDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@InstallationDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@InstallationDate"].Value = this._InstallationDate.Value;
			}

			if (this._InspectionDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@InspectionDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@InspectionDate"].Value = this._InspectionDate.Value;
			}

			if (this._CalibrationDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@CalibrationDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@CalibrationDate"].Value = this._CalibrationDate.Value;
			}

			if (this._QCDate.Value == DateTimeOffset.MinValue)
			{
				cmd.Parameters["@QCDate"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@QCDate"].Value = this._QCDate.Value;
			}

			if (this.SecondaryStorageFlag)
			{
				cmd.Parameters["@SecondaryStorageFlag"].Value = 1;
			}
			else
			{
				cmd.Parameters["@SecondaryStorageFlag"].Value = 0;
			}

			if (this.ManagedEquipmentFlag)
			{
				cmd.Parameters["@ManagedEquipmentFlag"].Value = 1;
			}
			else
			{
				cmd.Parameters["@ManagedEquipmentFlag"].Value = 0;
			}

			cmd.Parameters["@FuelingType"].Value = (int)this.FuelingType;

			if (this.AssignedToMeterGuid != Guid.Empty)
			{
				cmd.Parameters["@AssignedToMeterGuid"].Value = this.AssignedToMeterGuid;
			}
			else
			{
				cmd.Parameters["@AssignedToMeterGuid"].Value = DBNull.Value;
			}

			for (int nextItem = 1; nextItem <= 24; nextItem++)
			{
				cmd.Parameters["@UserData" + nextItem.ToString()].Value = this.UserData[nextItem - 1];
			}

			cmd.Parameters["@EquipmentGuid"].Value = this.IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblEquipment WHERE EquipmentGuid = @EquipmentGuid";

			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@EquipmentGuid"].Value = this.IdentityGuid;
		}

		public void EnumerateByEquipmentSQL(SqlCommand cmd)
		{
				cmd.CommandText = this.SelectClause +
								" FROM (tblEquipment LEFT JOIN (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid))) C ON tblEquipment.CompanyGuid = C.CompanyGuid)" +
								" LEFT JOIN tblEquipmentTypes D  ON D.EquipmentTypeGuid = tblEquipment.EquipmentTypeGuid " +
								" LEFT JOIN tblEquipmentMaintenanceLog E ON E.EquipmentGuid = tblEquipment._MasterRecordGuid " +
								" LEFT JOIN tblTestSetEquipmentResults F ON F.EquipmentGuid = tblEquipment._MasterRecordGuid " +
								" LEFT JOIN (SELECT GG.*, HH.EquipmentGuid, HH.Memo, HH.CreatedDate as QualityCreatedDate, HH.CreatedBy as QualityCreatedBy, HH.UpdatedDate as QualityUpdatedDate, HH.UpdatedBy as QualityUpdatedBy FROM tblEquipmentQualityTagLog HH " +
								"			LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
								"			AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblEquipmentQualityTagLog  " +
								"			WHERE tblEquipmentQualityTagLog.EquipmentGuid = HH.EquipmentGuid )) G ON G.EquipmentGuid = tblEquipment._MasterRecordGuid " +
								" WHERE tblEquipment.ParentEquipmentGuid = @EquipmentGuid" +
								" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblEquipmentMaintenanceLog WHERE tblEquipmentMaintenanceLog.EquipmentGuid = E.EquipmentGuid)) " +
								" AND (F.ResultTimeStamp IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetEquipmentResults WHERE tblTestSetEquipmentResults.EquipmentGuid = F.EquipmentGuid)) " +
								" ORDER BY tblEquipment.EquipmentSequence";

				cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@EquipmentGuid"].Value = this.IdentityGuid;
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
		}

		/// <summary>
		/// This method will populate the SQL Command to retrieve all the associated compartment Guids for
		/// a given equipment.
		/// </summary>
		/// <param name="cmd">The SQL Command object.</param>
		/// <param name="parentEquipmentGuid">The parent equipment Guid to search on.</param>
		public void GetEquipmentCompartmentGuidSql(SqlCommand cmd, Guid parentEquipmentGuid)
		  {
			cmd.CommandText = "SELECT E.EquipmentGuid, E.ID FROM tblEquipment E WHERE E.ParentEquipmentGuid = @ParentEquipmentGuid";

			cmd.Parameters.Add("@ParentEquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ParentEquipmentGuid"].Value = parentEquipmentGuid;
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Equipment Class.
		/// </summary>
		public EquipmentClass()
		{
			this.Initialize(null);
		}

		/// <summary>
		/// This is the default constructor for the Equipment Class.
		/// </summary>
		public EquipmentClass(EquipmentTypeClass EquipmentType)
		{
			this.Initialize(null);

			if (null != EquipmentType)
			{
				this.SetEquipmentType(EquipmentType);
			}
		}

		/// <summary>
		/// This constructor initializes the class based on the site.
		/// </summary>
		/// <param name="Site"></param>
		public EquipmentClass(SiteClass Site)
		{
			this.Initialize(Site);
		}

		/// <summary>
		/// This is the default constructor for the Equipment Class.
		/// </summary>
		public EquipmentClass(SiteClass Site, EquipmentTypeClass EquipmentType)
		{
			this.Initialize(Site);

			if (null != EquipmentType)
			{
				this.SetEquipmentType(EquipmentType);
			}
		}

		private void Initialize(SiteClass site)
		{
			//this.EquipmentType = new EquipmentTypeClass(Site);
			EngineeringUnit units = EngineeringUnit.FmvUsGal;

			NumberFormatInfo numFormatInfo = (site == null ? NumberFormatInfo.CurrentInfo :
				site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

			this._LowStockWarning = new SIDouble(units, numFormatInfo, 0.0);
			this._Volume = new SIDouble(units, numFormatInfo, 0.0);
			this._Capacity = new SIDouble(units, numFormatInfo, 0.0);
			this._SafeFill = new SIDouble(units, numFormatInfo, 0.0);

			this._LockedOutDate = (site == null) ? new Date() : new Date(site);
			this._ReturnToServiceDate = (site == null) ? new Date() : new Date(site);
			this._ManufactureDate = (site == null) ? new Date() : new Date(site);
			this._InstallationDate = (site == null) ? new Date() : new Date(site);
			this._InspectionDate = (site == null) ? new Date() : new Date(site);
			this._CalibrationDate = (site == null) ? new Date() : new Date(site);
			this._QCDate = (site == null) ? new Date() : new Date(site);
			this._MaintenanceUpdatedDate = (site == null) ? new Date() : new Date(site);
			this._MaintenanceCreatedDate = (site == null) ? new Date() : new Date(site);
			this._MaintenanceChangeDate = (site == null) ? new Date() : new Date(site);
			this._QualityCreatedDate = (site == null) ? new Date() : new Date(site);
			this._QualityUpdatedDate = (site == null) ? new Date() : new Date(site);

			this.Reset();
		}
		#endregion

		#region Comparison
		int IComparable.CompareTo(object obj)
		{
			EquipmentClass Equipment = obj as EquipmentClass;

			if (Equipment == null)
			{
				throw new Exception("Invalid Equipment");
			}

			return this.ID.CompareTo(Equipment.ID);
		}
		#endregion


		#region Internal methods
		public override void Reset()
		{
			base.Reset();
			this._Make = "";
			this._Model = "";
			this._Year = 0;
			this._IssPt = "";
			this._IssPtNum = "";
			this.CompanyGuid = Guid.Empty;
			this._Description = "";
			this._Fixed = false;
			this._StorageType = "";
			this._InUse = false;
			this.ProductGuid = Guid.Empty;
			this.FuelCardGuid = Guid.Empty;
			this.AssetTrackingDeviceGuid = Guid.Empty;
			this._FixedVolume = false;
			this._IntoPlane = false;
			this._Mobile = false;
			this._AttachedTo = "";
			this._MediaType = "";
			this._DefuelMeterForwards = false;
			this._Meters = 0;
			this._PulseRatio = 1.0;
			this._Round = false;
			this._Xref = "";
			this._StockTrack = false;
			this._Totalisor1 = "";
			this._Totalisor2 = "";
			this._FuelingState = "";
			this._MeterReading = 0.0;
			this._Consecutive_OOS_Variance = 0;
			this._Notes = "";
			this._VolumeUnits = EngineeringUnit.FmvUsGal;
			this._TemperatureUnits = EngineeringUnit.FmtDegF;
			this._DensityUnits = EngineeringUnit.FmdDegApi;
			this._MassUnits = EngineeringUnit.FmmLb;
			this._VolumeDecimalPlaces = 0;
			this._TemperatureDecimalPlaces = 0;
			this._DensityDecimalPlaces = 0;
			this._MassDecimalPlaces = 0;
			this.ParentEquipmentGuid = Guid.Empty;
			this._EquipmentSequence = "0";
			this._LockedOut = false;
			this._LockedOutReason = "";
				this._ScullyRequired = false;
				this._SerialNumber = "";
			this._CompanyEquipmentID = "";
			this._TruckCardNumber = "";
			 this.HiddenDate = null;
			this._CompanyID = "{Unassigned}";
			this._CompanyName = "";
			this._CompanyAddress = "";
			this._CompanyCity = "";
			this._CompanyState = "";
			this._ProductID = "{Unassigned}";
			this._FuelCardID = "{Unassigned}";
			this.assetTrackingDeviceId = "{Unassigned}";
			this._RatedGPM = 0;
			this._ActualGPM = 0;
			this._FuelAdditiveFlag = false;
			this._SecondaryStorageFlag = false;
			this._StatusDescription = "";
			this._LockedOutDate.Value = DateTimeOffset.MinValue;
			this._ReturnToServiceDate.Value = DateTimeOffset.MinValue;
			this._ManufactureDate.Value = DateTimeOffset.MinValue;
			this._InstallationDate.Value = DateTimeOffset.MinValue;
			this._InspectionDate.Value = DateTimeOffset.MinValue;
			this._CalibrationDate.Value = DateTimeOffset.MinValue;
			this._QCDate.Value = DateTimeOffset.MinValue;
			this.AssignedToMeterGuid = Guid.Empty;
			this.IsAssignedToPersonnel = false;

			this._InServiceFlag = true;
			this._MaintenanceNote = "";
			this._MaintenanceOperatorID = "";
			this._MaintenanceWorkOrder = "";
			this._MaintenanceCreatedBy = "";
			this._MaintenanceUpdatedBy = "";

			this._MaintenanceUpdatedDate.Value = DateTimeOffset.MinValue;
			this._MaintenanceCreatedDate.Value = DateTimeOffset.MinValue;
			this._MaintenanceChangeDate.Value = DateTimeOffset.MinValue;
			this._QCNote = "";

			this._QualityCreatedDate.Value = DateTimeOffset.MinValue;
			this._QualityUpdatedDate.Value = DateTimeOffset.MinValue;
			this._ManagedEquipmentFlag = false; ;
			this._FuelingType = FUELING_TYPES.NONE;

			this.CompartmentCollection = new EquipmentCollectionClass();
			this.TestAndInspectionCollection = new QualificationMapCollectionClass();
			this.TagAndLicenseCollection = new QualificationMapCollectionClass();
			this.TestAndInspectionCollection = new QualificationMapCollectionClass();
			this.Meter = null;
			this.UserData = new UserDataClass();
			this.QualityTag = new QualityTagClass();

			this.volumeProcessVariable = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV,
				UNIT_TYPE.EQUIPMENT_UNIT,
				VarEnum.VT_R8,
				false,
				"",
				"",
				"");

			this.volumeProcessVariable.SetMaximum(1000000.0, this._VolumeUnits);
			this.volumeProcessVariable.SetMinimum(0, this._VolumeUnits);

				this.Meter = new List<MeterClass>();
		}
		#endregion

		#region Public methods
		public void SetSelectLimit(int a_limit)
		{
			if (a_limit > 0)
			{
				// remove the SELECT
				this.SelectClause = this.SelectClause.Remove(0, 6);

				// add custom select
				this.SelectClause = "SELECT TOP " + a_limit + " " + this.SelectClause;
			}
		}

		public void LoadIDType(Object O)
		{
			this.Reset();

			if (typeof(DataSet).IsInstanceOfType(O))
			{
				DataSet Set = (DataSet)O;

				DataTable Table = Set.Tables[0];
				if (Table.Rows.Count == 0)
					return;

				DataRow Row = Table.Rows[0];
				this._IdentityGuid = DataObject.getValue<Guid>(Row["EquipmentGuid"], Guid.Empty);
				this._ID = DataObject.getValue<string>(Row["ID"], "");
				this._EquipmentType = DataObject.getValue<EQUIPMENT_TYPE>(Row["LookupEquipmentTypeIndex"], EQUIPMENT_TYPE.COMPARTMENT_TYPE);
			}

		}

		public override void Load(Object o)
		{
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

				this._IdentityGuid = DataObject.getValue<Guid>(Row["EquipmentGuid"], Guid.Empty);
				this._MasterRecordGuid = DataObject.getValue<Guid>(Row["_MasterRecordGuid"], Guid.Empty);
				this.SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				this._ID = DataObject.getValue<string>(Row["ID"], "");
				this._Description = DataObject.getValue<string>(Row["Description"], "");
				this.EquipmentTypeGuid = DataObject.getValue<Guid>(Row["EquipmentTypeGuid"], Guid.Empty);
				this._Make = DataObject.getValue<string>(Row["Make"], "");
				this._Model = DataObject.getValue<string>(Row["Model"], "");
				this._Year = DataObject.getValue<int>(Row["Year"], 0);
				this._IssPtNum = DataObject.getValue<string>(Row["IssPtNum"], "");
				this.CompanyGuid = DataObject.getValue<Guid>(Row["CompanyGuid"], Guid.Empty);
				this._Fixed = DataObject.getValue<bool>(Row["Fixed"], false);
				this._StorageType = DataObject.getValue<string>(Row["StorageType"], "");
				this._InUse = DataObject.getValue<bool>(Row["InUse"], false);
				this.ProductGuid = DataObject.getValue<Guid>(Row["ProductGuid"], Guid.Empty);
				this.FuelCardGuid = DataObject.getValue<Guid>(Row["FuelCardGuid"], Guid.Empty);
				this.AssetTrackingDeviceGuid = DataObject.getValue<Guid>(Row["AssetTrackingDeviceGuid"], Guid.Empty);
				this._FixedVolume = DataObject.getValue<bool>(Row["FixedVolume"], false);
				this._IntoPlane = DataObject.getValue<bool>(Row["IntoPlane"], false);
				this._Mobile = DataObject.getValue<bool>(Row["Mobile"], false);
				this._AttachedTo = DataObject.getValue<string>(Row["AttachedTo"], "");
				this._MediaType = DataObject.getValue<string>(Row["MediaType"], "");
				this._Meters = DataObject.getValue<int>(Row["Meters"], 0);
				this._DefuelMeterForwards = DataObject.getValue<bool>(Row["DefuelMeterForwards"], false);
				this._PulseRatio = DataObject.getValue<double>(Row["PulseRatio"], 0.0);
				this._Round = DataObject.getValue<bool>(Row["Round"], false);
				this._Xref = DataObject.getValue<string>(Row["Xref"], "");
				this._LowStockWarning.SIValue = DataObject.getValue<double>(Row["LowStockWarning"], 0.0);
				this._StockTrack = DataObject.getValue<bool>(Row["StockTrack"], false);
				this._Totalisor1 = DataObject.getValue<string>(Row["Totalisor1"], "");
				this._Totalisor2 = DataObject.getValue<string>(Row["Totalisor2"], "");
				this._FuelingState = DataObject.getValue<string>(Row["FuelingState"], "");
				this._Volume.SIValue = DataObject.getValue<double>(Row["Volume"], 0.0);
				this._MeterReading = DataObject.getValue<double>(Row["MeterReading"], 0.0);
				this._Consecutive_OOS_Variance = DataObject.getValue<int>(Row["Consecutive_OOS_Variance"], 0);
				this._Notes = DataObject.getValue<string>(Row["Notes"], "");
				this._Capacity.SIValue = DataObject.getValue<double>(Row["Capacity"], 0.0);
				this._SafeFill.SIValue = DataObject.getValue<double>(Row["SafeFill"], 0.0);
				this._VolumeUnits = DataObject.getValue<EngineeringUnit>(Row["VolumeUnitIndex"], EngineeringUnit.FmvUsGal);
				this._TemperatureUnits = DataObject.getValue<EngineeringUnit>(Row["TemperatureUnitIndex"], EngineeringUnit.FmtDegF);
				this._DensityUnits = DataObject.getValue<EngineeringUnit>(Row["DensityUnitIndex"], EngineeringUnit.FmdDegApi);
				this._MassUnits = DataObject.getValue<EngineeringUnit>(Row["MassUnitIndex"], EngineeringUnit.FmmLb);
				this._VolumeDecimalPlaces = DataObject.getValue<byte>(Row["VolumeDecimalPlaces"], 0);
				this._TemperatureDecimalPlaces = DataObject.getValue<byte>(Row["TemperatureDecimalPlaces"], 0);
				this._DensityDecimalPlaces = DataObject.getValue<byte>(Row["DensityDecimalPlaces"], 0);
				this._MassDecimalPlaces = DataObject.getValue<byte>(Row["MassDecimalPlaces"], 0);
				this.ParentEquipmentGuid = DataObject.getValue<Guid>(Row["ParentEquipmentGuid"], Guid.Empty);
				this._EquipmentSequence = DataObject.getValue<string>(Row["EquipmentSequence"], "0");
				this._LockedOut = DataObject.getValue<bool>(Row["LockedOut"], false);
				this._LockedOutReason = DataObject.getValue<string>(Row["LockedOutReason"], "");
				this._LockedOutDate.Value = DataObject.getValue<DateTimeOffset>(Row["LockedOutDate"], DateTimeOffset.MinValue);
					 this._ScullyRequired = DataObject.getValue<bool>(Row["ScullyRequired"], false);
					 this._SerialNumber = DataObject.getValue<string>(Row["SerialNumber"], "");
				this._CompanyEquipmentID = DataObject.getValue<string>(Row["CompanyEquipmentID"], "");
				this._TruckCardNumber = DataObject.getValue<string>(Row["TruckCardNumber"], "");
					 this.HiddenDate = DataObject.getValue<DateTimeOffset?>(Row["HiddenDate"], null);
				this._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				this._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				this._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this._CreatedDate);
				this._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				this._RatedGPM = DataObject.getValue<double>(Row["RatedGPM"], 0.0);
				this._ActualGPM = DataObject.getValue<double>(Row["ActualGPM"], 0.0);
				this._FuelAdditiveFlag = DataObject.getValue<bool>(Row["FuelAdditiveFlag"], false);
				this._ManufactureDate.Value = DataObject.getValue<DateTimeOffset>(Row["ManufactureDate"], DateTimeOffset.MinValue);
				this._InstallationDate.Value = DataObject.getValue<DateTimeOffset>(Row["InstallationDate"], DateTimeOffset.MinValue);
				this._InspectionDate.Value = DataObject.getValue<DateTimeOffset>(Row["InspectionDate"], DateTimeOffset.MinValue);
				this._CalibrationDate.Value = DataObject.getValue<DateTimeOffset>(Row["CalibrationDate"], DateTimeOffset.MinValue);
				this._QCDate.Value = DataObject.getValue<DateTimeOffset>(Row["QCDate"], DateTimeOffset.MinValue);
				this._SecondaryStorageFlag = DataObject.getValue<bool>(Row["SecondaryStorageFlag"], false);
				this._ManagedEquipmentFlag = DataObject.getValue<bool>(Row["ManagedEquipmentFlag"], false);
				this._FuelingType = (FUELING_TYPES)DataObject.getValue<short>(Row["FuelingType"], (short)FUELING_TYPES.NONE);
				this.AssignedToMeterGuid = DataObject.getValue<Guid>(Row["AssignedToMeterGuid"], Guid.Empty);
				DataColumnCollection columns = Table.Columns;
				if (columns.Contains("IsAssignedToPersonnel"))
				{
					this.IsAssignedToPersonnel = DataObject.getValue<bool>(Row["IsAssignedToPersonnel"], false);
				}

				this._CompanyID = DataObject.getValue<string>(Row["CompanyID"], "{Unassigned}");
				this._CompanyName = DataObject.getValue<string>(Row["Name"], "");
				this._CompanyAddress = DataObject.getValue<string>(Row["Address1"], "");
				this._CompanyCity = DataObject.getValue<string>(Row["City"], "");
				this._CompanyState = DataObject.getValue<string>(Row["State"], "");

				this._EquipmentTypeID = DataObject.getValue<string>(Row["EqTypeName"], "");
				this._EquipmentType = DataObject.getValue<EQUIPMENT_TYPE>(Row["LookupEquipmentTypeIndex"], EQUIPMENT_TYPE.COMPARTMENT_TYPE);
				this._IsMultiCompartment = DataObject.getValue<bool>(Row["MultiCompartment"], false);
				this._IssPt = DataObject.getValue<string>(Row["Isspt"], "");
				this._CompanyRoleAssignmentConstraint = DataObject.getValue<COMPANY_ROLE>(Row["LookupCompanyRoleIndex"], COMPANY_ROLE.MAX_COMPANY_ROLE);

				//EquipmentType.ID = DataObject.getValue<string>(Row["EqTypeName"], "");
				//EquipmentType.Attribute = DataObject.getValue<EQUIPMENT_TYPE>(Row["LookupEquipmentTypeIndex"], EQUIPMENT_TYPE.COMPARTMENT_TYPE);
				//EquipmentType.SICapacity.SIValue = DataObject.getValue<double>(Row["Capacity"], 0.0);
				//EquipmentType.SISafeFill.SIValue = DataObject.getValue<double>(Row["SafeFill"], 0.0);
				//EquipmentType.IsMultiCompartment = DataObject.getValue<bool>(Row["MultiCompartment"], false);
				//EquipmentType.Isspt = DataObject.getValue<string>(Row["Isspt"], "");
				this._ReturnToServiceDate.Value = DataObject.getValue<DateTimeOffset>(Row["ReturnToServiceDate"], DateTimeOffset.MinValue);
				this._StatusDescription = DataObject.getValue<string>(Row["StatusDescription"], "");

				this._InServiceFlag = DataObject.getValue<byte>(Row["InServiceFlag"], 1) != 0;
				this._MaintenanceNote = DataObject.getValue<string>(Row["MaintenanceNote"], "");

				this._MaintenanceChangeDate.Value = DataObject.getValue<DateTimeOffset>(Row["ChangeDate"], DateTimeOffset.MinValue);
				this._MaintenanceOperatorID = DataObject.getValue<string>(Row["MaintenanceOperatorID"], "");
				this._MaintenanceWorkOrder = DataObject.getValue<string>(Row["MaintenanceWorkOrder"], "");
				this._MaintenanceCreatedDate.Value = DataObject.getValue<DateTimeOffset>(Row["MaintenanceCreatedDate"], DateTimeOffset.MinValue);
				this._MaintenanceCreatedBy = DataObject.getValue<string>(Row["MaintenenaceCreatedBy"], "");
				this._MaintenanceUpdatedDate.Value = DataObject.getValue<DateTimeOffset>(Row["MaintenenaceUpdatedDate"], DateTimeOffset.MinValue);
				this._MaintenanceUpdatedBy = DataObject.getValue<string>(Row["MaintenanceUpdatedBy"], "");

				this._QCNote = DataObject.getValue<string>(Row["QCNote"], "");
				this._QualityCreatedDate.Value = DataObject.getValue<DateTimeOffset>(Row["QualityCreatedDate"], DateTimeOffset.MinValue);
				this._QualityCreatedBy = DataObject.getValue<string>(Row["QualityCreatedBy"], "");
				this._QualityUpdatedDate.Value = DataObject.getValue<DateTimeOffset>(Row["QualityUpdatedDate"], DateTimeOffset.MinValue);
				this._QualityUpdatedBy = DataObject.getValue<string>(Row["QualityUpdatedBy"], "");

				this.QualityTag.IdentityGuid = DataObject.getValue<Guid>(Row["QualityTagGuid"], Guid.Empty);
				this.QualityTag.SiteGuid = DataObject.getValue<Guid>(Row["QualityTagSiteGuid"], Guid.Empty);
				this.QualityTag.ID = DataObject.getValue<string>(Row["QualityTagName"], "");
				this.QualityTag.Severity = (QUALITY_SEVERITY_LEVELS)DataObject.getValue<short>(Row["Severity"], (short)QUALITY_SEVERITY_LEVELS.CAUTION);
				this.QualityTag.Active = DataObject.getValue<bool>(Row["Active"], false);

				this._ProductID = DataObject.getValue<string>(Row["ProductID"], "{Unassigned}");
				this._FuelCardID = DataObject.getValue<string>(Row["FuelCardID"], "{Unassigned}");
				this.assetTrackingDeviceId = DataObject.getValue<string>(Row["AssetTrackingDeviceID"], "{Unassigned}");

				this.UserData[0] = DataObject.getValue<string>(Row["UserData1"], "");
				this.UserData[1] = DataObject.getValue<string>(Row["UserData2"], "");
				this.UserData[2] = DataObject.getValue<string>(Row["UserData3"], "");
				this.UserData[3] = DataObject.getValue<string>(Row["UserData4"], "");
				this.UserData[4] = DataObject.getValue<string>(Row["UserData5"], "");
				this.UserData[5] = DataObject.getValue<string>(Row["UserData6"], "");
				this.UserData[6] = DataObject.getValue<string>(Row["UserData7"], "");
				this.UserData[7] = DataObject.getValue<string>(Row["UserData8"], "");
				this.UserData[8] = DataObject.getValue<string>(Row["UserData9"], "");
				this.UserData[9] = DataObject.getValue<string>(Row["UserData10"], "");
				this.UserData[10] = DataObject.getValue<string>(Row["UserData11"], "");
				this.UserData[11] = DataObject.getValue<string>(Row["UserData12"], "");
				this.UserData[12] = DataObject.getValue<string>(Row["UserData13"], "");
				this.UserData[13] = DataObject.getValue<string>(Row["UserData14"], "");
				this.UserData[14] = DataObject.getValue<string>(Row["UserData15"], "");
				this.UserData[15] = DataObject.getValue<string>(Row["UserData16"], "");
				this.UserData[16] = DataObject.getValue<string>(Row["UserData17"], "");
				this.UserData[17] = DataObject.getValue<string>(Row["UserData18"], "");
				this.UserData[18] = DataObject.getValue<string>(Row["UserData19"], "");
				this.UserData[19] = DataObject.getValue<string>(Row["UserData20"], "");
				this.UserData[20] = DataObject.getValue<string>(Row["UserData21"], "");
				this.UserData[21] = DataObject.getValue<string>(Row["UserData22"], "");
				this.UserData[22] = DataObject.getValue<string>(Row["UserData23"], "");
				this.UserData[23] = DataObject.getValue<string>(Row["UserData24"], "");

				this.RowVersion = DataObject.getValue<Byte[]>(Row["_RowVersion"], null);

				this._LowStockWarning.Units = this._VolumeUnits;
				this._Volume.Units = this._VolumeUnits;
				this._Capacity.Units = this._VolumeUnits;
				this._SafeFill.Units = this._VolumeUnits;

				if (!this._LowStockWarning.Format.IsReadOnly)
				{
					this._LowStockWarning.Format.NumberDecimalDigits = this._VolumeDecimalPlaces;
				}
				if (!this._Volume.Format.IsReadOnly)
				{
					this._Volume.Format.NumberDecimalDigits = this._VolumeDecimalPlaces;
				}
				if (!this._Capacity.Format.IsReadOnly)
				{
					this._Capacity.Format.NumberDecimalDigits = this._VolumeDecimalPlaces;
				}
				if (!this._SafeFill.Format.IsReadOnly)
				{
					this._SafeFill.Format.NumberDecimalDigits = this._VolumeDecimalPlaces;
				}
			}
			else
			{
				base.Load(o);

				if (typeof(XmlNode).IsInstanceOfType(o))
				{
					XmlNode parentNode = (XmlNode)o;

					foreach (XmlNode Node in parentNode)
					{
						if (Node.Name == "Compartments")
						{
							int Sequence = 0;
							foreach (XmlNode CompartmentNode in Node)
							{
								EquipmentClass Compartment = new EquipmentClass();
								Compartment.Load(CompartmentNode);
								this.CompartmentCollection.Add(Compartment);

								// See if the value for equipment sequence is a number.
								// if so, increment it.  Otherwise ?
								int equipSequence;
								if (Int32.TryParse(Compartment.EquipmentSequence, out equipSequence))
								{
									equipSequence = ++Sequence;
									Compartment.EquipmentSequence = equipSequence.ToString();
								}
								else
								{
									// Set the value to an empty string?
									Compartment.EquipmentSequence = "";
								}

								Compartment.Type = EQUIPMENT_TYPE.COMPARTMENT_TYPE;
							}
						}
						else if (Node.Name == "TagsAndLicense")
						{
							int Sequence = 0;
							foreach (XmlNode TagAndLicenseNode in Node)
							{
								QualificationMapClass TagAndLicense = new QualificationMapClass();
								TagAndLicense.Load(TagAndLicenseNode);
								TagAndLicense.Sequence = Sequence++;
								this.TagAndLicenseCollection.Add(TagAndLicense);
							}
						}

					}

				}
			}
		}

		public void LoadForDispatch(Object dataObj)
		{
			this.Reset();

			var set = dataObj as DataSet;

			if (set != null)
			{
				DataTable table = set.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				_IdentityGuid			= DataObject.getValue<Guid>(row["EquipmentGuid"], Guid.Empty);
				_MasterRecordGuid		= DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty);
				_ID						= DataObject.getValue<string>(row["ID"], "");
				_IssPtNum				= row.IsNull("IssPtNum") ? "" : (string) row["IssPtNum"];
				ProductGuid				= DataObject.getValue<Guid>(row["ProductGuid"], Guid.Empty);
				_Xref					= row.IsNull("Xref") ? "" : (string) row["Xref"];
				_Volume.SIValue			= row.IsNull("Volume") ? 0.0 : (double) row["Volume"];
				_VolumeUnits			= DataObject.getValue<EngineeringUnit>(row["VolumeUnitIndex"], EngineeringUnit.FmvUsGal);
				_VolumeDecimalPlaces	= row.IsNull("VolumeDecimalPlaces") ? (byte) 0 : (byte) row["VolumeDecimalPlaces"];
				_LockedOut				= row.IsNull("LockedOut") ? false : (bool) row["LockedOut"];
					 _ScullyRequired         = row.IsNull("ScullyRequired") ? false : (bool)row["ScullyRequired"];
					 _QCDate.Value			= (row.IsNull("QCDate")) ? DateTime.MinValue : (DateTime) row["QCDate"];
				_SecondaryStorageFlag	= row.IsNull("SecondaryStorageFlag") ? false : (bool) row["SecondaryStorageFlag"];
				_ManagedEquipmentFlag	= row.IsNull("ManagedEquipmentFlag") ? false : (bool) row["ManagedEquipmentFlag"];
				_FuelingType			= row.IsNull("FuelingType") ? FUELING_TYPES.NONE : (FUELING_TYPES) (short) row["FuelingType"];
				_FuelAdditiveFlag		= row.IsNull("FuelAdditiveFlag") ? false : (bool) row["FuelAdditiveFlag"];
				_EquipmentTypeID		= DataObject.getValue<string>(row["EqTypeName"], "");
				_InServiceFlag			= row.IsNull("InServiceFlag") ? true : (((byte) row["InServiceFlag"]) != 0);
				_ProductID				= row.IsNull("ProductID") ? "{Unassigned}" : (string) row["ProductID"];
				_Volume.Units			= _VolumeUnits;
				//EquipmentType.Attribute = row.IsNull("EqTypeAttribute") ? EQUIPMENT_TYPE.COMPARTMENT_TYPE : (EQUIPMENT_TYPE) row["EqTypeAttribute"];

				if (!_Volume.Format.IsReadOnly)
				{
					_Volume.Format.NumberDecimalDigits = _VolumeDecimalPlaces;
				}
			}
		}

		public override void Store(Object o)
		{
			if (typeof(XmlNode).IsInstanceOfType(o))
			{
				XmlNode EquipmentNode = (XmlNode)o;
				if (EquipmentNode.Name == "Compartment")
				{
					XmlAttribute Attribute;

					Attribute = EquipmentNode.OwnerDocument.CreateAttribute("ID");
					Attribute.Value = this.ID;
					EquipmentNode.Attributes.Append(Attribute);

					Attribute = EquipmentNode.OwnerDocument.CreateAttribute("Capacity");
					Attribute.Value = this.Capacity;
					EquipmentNode.Attributes.Append(Attribute);

					Attribute = EquipmentNode.OwnerDocument.CreateAttribute("SafeFill");
					Attribute.Value = this.SafeFill;
					EquipmentNode.Attributes.Append(Attribute);
				}

				else
				{
					base.Store(o);

					if (this._IsMultiCompartment)
					{
						XmlNode CompartmentsNode = (XmlNode)EquipmentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Compartments", null);
						EquipmentNode.AppendChild(CompartmentsNode);
						foreach (EquipmentClass Compartment in this.CompartmentCollection)
						{
							XmlNode CompartmentNode = (XmlNode)CompartmentsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Compartment", null);
							Compartment.Store(CompartmentNode);
							CompartmentsNode.AppendChild(CompartmentNode);
						}
					}



					XmlNode TagsAndLicensesNode = (XmlNode)EquipmentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "TagsAndLicenses", null);
					EquipmentNode.AppendChild(TagsAndLicensesNode);
					foreach (QualificationMapClass TagAndLicense in this.TagAndLicenseCollection)
					{
						XmlNode TagAndLicenseNode = (XmlNode)TagsAndLicensesNode.OwnerDocument.CreateNode(XmlNodeType.Element, "TagAndLicense", null);
						TagAndLicense.Store(TagAndLicenseNode);
						TagsAndLicensesNode.AppendChild(TagAndLicenseNode);
					}
				}
			}
		}

		 public static void UpdateFuelCardAssignments(SecurityClass security, Guid equipmentGuid, Guid fuelCardGuid)
		 {
			  
		 }

		public override bool Equals(object obj)
		{
			EquipmentClass testObject = (EquipmentClass)obj;
			if (testObject != null)
			{
				return testObject.IdentityGuid == this.IdentityGuid;
			}

			return base.Equals(obj);

		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		  
		  public void QueryWriterSQL(SqlCommand cmd, SecurityClass Security, string selectClause)
		  {
				cmd.CommandText = "SELECT * FROM (" +
								selectClause +
								" ,tblEquipment.EquipmentGuid as EntityGuid," +
						" EquipmentCompany.ID,EquipmentCompany.Name,EquipmentCompany.Address1,EquipmentCompany.City,EquipmentCompany.State," +
								" EquipmentProduct.ProductID as 'ProductID'," +
								" dbo.udf_ConvertFromSIUnits(tblEquipment.Capacity,tblEquipment.VolumeUnitIndex, tblEquipment.VolumeDecimalPlaces) as 'tblEquipment.Capacity'," +
								" dbo.udf_ConvertFromSIUnits(tblEquipment.Volume, tblEquipment.VolumeUnitIndex, tblEquipment.VolumeDecimalPlaces) as 'tblEquipment.Volume'," +
								" dbo.udf_ConvertFromSIUnits(tblEquipment.SafeFill, tblEquipment.VolumeUnitIndex, tblEquipment.VolumeDecimalPlaces) as 'tblEquipment.SafeFill'," +
								" (SELECT ID FROM tblFuelCards WHERE tblFuelCards.FuelCardGuid = tblEquipment.FuelCardGuid) AS FuelCardID," +
						" (SELECT DeviceID FROM tblAssetTrackingDevice WHERE tblAssetTrackingDevice.AssetTrackingDeviceGuid = tblEquipment.AssetTrackingDeviceGuid) AS AssetTrackingDeviceID," +
						" (SELECT LookupEquipmentTypeIndex FROM tblEquipmentTypes WHERE tblEquipment.EquipmentTypeGuid = EquipmentTypeGuid) as [Type], " +
						" (SELECT LookupCompanyRoleIndex FROM tblEquipmentTypes WHERE tblEquipment.EquipmentTypeGuid = EquipmentTypeGuid) as [CompanyRoleConstraint] " +
						" FROM tblEquipment " +
							" LEFT JOIN (SELECT rc.MasterRecordGuid, C.ID, C.Name, C.Address1, C.City, C.State FROM [erv].[udf_GetCompanyRecordVersions]('" + Security.SiteGuid.ToString() + "') rc INNER JOIN tblCompanies C ON C.CompanyGuid = rc.CompanyGuid ) EquipmentCompany" +
							" ON tblEquipment.CompanyGuid = EquipmentCompany.MasterRecordGuid " +
							" LEFT JOIN (SELECT rp.MasterRecordGuid, P.ProductID FROM [erv].[udf_GetProductRecordVersions]('" + Security.SiteGuid.ToString() + "') rp INNER JOIN tblProducts P ON P.ProductGuid = rp.ProductGuid) EquipmentProduct" +
							" ON tblEquipment.ProductGuid = EquipmentProduct.MasterRecordGuid " +
							" LEFT JOIN tblEquipmentTypes D  ON D.EquipmentTypeGuid = tblEquipment.EquipmentTypeGuid " +
							" LEFT JOIN tblEquipmentMaintenanceLog E ON E.EquipmentGuid = tblEquipment._MasterRecordGuid " +
							" LEFT JOIN tblTestSetEquipmentResults F ON F.EquipmentGuid = tblEquipment._MasterRecordGuid " +
							" LEFT JOIN (SELECT GG.*, EquipmentGuid, HH.Memo, HH.CreatedDate as QualityCreatedDate, HH.CreatedBy as QualityCreatedBy, HH.UpdatedDate as QualityUpdatedDate, HH.UpdatedBy as QualityUpdatedBy FROM tblEquipmentQualityTagLog HH " +
							" LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
								" AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblEquipmentQualityTagLog  " +
								" WHERE tblEquipmentQualityTagLog.EquipmentGuid = HH.EquipmentGuid )) G ON G.EquipmentGuid = tblEquipment._MasterRecordGuid " +
								" WHERE tblEquipment.EquipmentGuid IN (SELECT EquipmentGuid FROM [erv].[udf_GetEquipmentRecordVersions] ('" + Security.SiteGuid.ToString() + "'))" +
								" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblEquipmentMaintenanceLog WHERE tblEquipmentMaintenanceLog.EquipmentGuid = E.EquipmentGuid)) " +
								" AND (F.ResultTimeStamp IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetEquipmentResults WHERE tblTestSetEquipmentResults.EquipmentGuid = F.EquipmentGuid)) " +
								") tblCombinedTable WHERE 1=1 ";
		  }

		// Gets the ID and Type columns for all equipment.
		public void EnumerateGetIDTypeOnlySQL(SqlCommand cmd)
		{
				cmd.CommandText = ""
						  + "SELECT IdentityGuid, _MasterRecordGuid, "
						  + "		ID, "
						  + "		D.LookupEquipmentTypeIndex "
						  + "  FROM dbo.tblEquipment LEFT JOIN tblEquipmentTypes D  ON D.EquipmentTypeGuid = tblEquipment.EquipmentTypeGuid "
						  + " ORDER BY ID ";
		}

		public string EnumerateNotificationSQL( SecurityClass Security )
		{
			var siteFromJoinClause = this.SiteFromJoinClause( Security, "dbo.tblEquipment", "[EquipmentGuid]" );

			string sql = "DECLARE @topRowVersion TIMESTAMP SET @topRowVersion = ( SELECT TOP 1 B.[_RowVersion] " + siteFromJoinClause + " ORDER BY B.[_RowVersion] DESC) ";

			sql += "DECLARE @topDate DATETIMEOFFSET SET @topDate = ( SELECT TOP 1 A.[UpdatedDate] " + siteFromJoinClause + " ORDER BY A.UpdatedDate DESC) ";

			sql += "SELECT COUNT(*) as 'Count',IsNull(@topRowVersion,0) as 'TopIndex',IsNull(@topDate,'1900-01-01') as 'TopDate' " + siteFromJoinClause;

			return sql;
		}

		public void EnumerateNotificationForOpcSQL(SecurityClass security, SqlCommand sqlCommand)
		{
			string sql =
				"Select COUNT(*) as 'Count', ISNULL(MAX(b._RowVersion), 0) as 'TopIndex', ISNULL(MAX(b.UpdatedDate), '1900-01-01') as 'TopDate' "
				+ " FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a "
				+ " INNER JOIN tblEquipment b ON b.EquipmentGuid = a.EquipmentGuid " + " WHERE b.SecondaryStorageFlag = 1";

			sqlCommand.CommandText = sql;
			sqlCommand.Parameters.AddWithValue("@TargetSiteGuid", security.SiteGuid);

		}

		/// <summary>
		/// This method returns an associated transaction count SQL command.
		/// </summary>
		/// <param name="equipmentGuid"></param>
		/// <returns>Returns the SQL command for count association.</returns>
		public static SqlCommand CountAssociatedTrxSQL(Guid equipmentGuid)
		{
			string sql = string.Format("SELECT COUNT(*) FROM tblTransactions WHERE " +
										"Source1EquipmentGuid = '{0}' OR " +
										"Source2EquipmentGuid = '{0}' OR " +
										"Source3EquipmentGuid = '{0}' OR " +
										"Destination1EquipmentGuid = '{0}' OR " +
										"Destination2EquipmentGuid = '{0}' OR " +
										"Destination3EquipmentGuid = '{0}'",
				equipmentGuid);

			var command = new SqlCommand(sql);

			return command;
		}

		public void SetEquipmentType(EquipmentTypeClass EquipmentType)
		{
			Guid previousEquipmentTypeGuid = this.EquipmentTypeGuid;

			if (null != EquipmentType)
			{
				this.EquipmentTypeGuid = EquipmentType.IdentityGuid;
				this._EquipmentTypeID = EquipmentType.ID;
				this._EquipmentType = EquipmentType.Attribute;
				this._CompanyRoleAssignmentConstraint = EquipmentType.CompanyRoleAssignmentConstraint;

				this._IsMultiCompartment = EquipmentType.IsMultiCompartment;
				this._IssPt = EquipmentType.Isspt;

				// Only use the Capacity and SafeFill values from the EquipmentType 
				// if we're initializing or changing the EquipmentType.
				if (previousEquipmentTypeGuid != EquipmentType.IdentityGuid)
				{
					if (EquipmentType.Attribute == EQUIPMENT_TYPE.COMPARTMENT_TYPE || EquipmentType.IsMultiCompartment)
					{
						this._Capacity = EquipmentType.SICapacity;
						this._SafeFill = EquipmentType.SISafeFill;
					}
				}
			}
		}

		  public QueryWriterFieldCollection QueryAliasFields(SecurityClass security, QueryWriterFieldCollection fields)
		  {
				var userDataFieldCollection =
					 FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
						  x => x.EnumerateByEntityType(security, ENTITY_TYPE.EQUIPMENT, Guid.Empty, false, false));

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
			return "FMWebApp\\EquipmentForm.aspx";
		}

		  /// <summary>
		  /// The locked out station alarm.
		  /// </summary>
		  /// <param name="driverId">
		  /// Id of the driver to be recorded as using the locked out equipment.
		  /// </param>
		  /// <param name="stationId">
		  /// Id of the station to be recorded as the location where the locked out equipment was entered.
		  /// </param>
		  /// <returns>
		  /// The <see cref="AlarmAndEventLogClass"/> containing the alarm data.
		  /// </returns>
		  public AlarmAndEventLogClass LockedOutStationAlarm(string driverId, string stationId)
		  {
				var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentLockedOutAlarmDescriptor)
				{
					 AssociatedData =
															  this.ID + " - Driver " +
															  driverId + " - " +
															  stationId
				};
				return alarmAndEventLog;
		  }
		  #endregion Public Methods
	 }
}
