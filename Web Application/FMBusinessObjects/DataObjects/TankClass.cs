namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using FMCore;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public enum VESSEL_TYPE
	{
		UNDEFINED_VESSEL = 0,
		SPHERICAL_VESSEL = 1,
		CYLINDRICAL_VESSEL = 2,
		BULLET_VESSEL = 3,
		PROPANE_VESSEL = 4,
		UNDERGROUND_VESSEL = 5,
		TANKER_VESSEL = 6,
		PIPELINE_VESSEL = 7,
		COLLAPSIBLE_STORAGE_TANK = 8,
		OTHER_VESSEL = 9,
		MAX_VESSEL = 10
	};

	public enum DeviceTankTypes { Opc, Satellite }

	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(TankClass))]
	public class TankCollectionClass : List<TankClass>
	{
	}

	[XmlRoot("Tanks")]
	[XmlType("Tank")]
	[Serializable]
	[DataContract]
	[QueryWriterTopic(typeof(TankClass), "Tanks")]
	[QueryWriterTopicSecurity(RIGHT.VIEW_TANK_DATA)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_TANK_DATA)]
	public class TankClass : BaseDataObject
	{
		#region Public data members
		[DataMember]
		public ProcessVariableCollectionClass ProcessVariableCollection;
		[DataMember]
		public QualityTagClass QualityTag;

		[DataMember]
		public List<MeterClass> Meters = new List<MeterClass>();
		#endregion

		#region Protected data members
		[DataMember] protected Guid _ProductGuid;
		[DataMember] protected Guid _ManagerGuid;
		[DataMember] protected VESSEL_TYPE _VesselType;

		// From SubQuery
		[DataMember] protected string _ProductID;
		[DataMember] protected string _ProductCode;
		[DataMember] protected string _ManagerID;
		[DataMember] protected string _ManagerCode;

		[DataMember] protected string _MaintenanceNote;
		[DataMember] protected string _QCNote;
		[DataMember] protected bool _InServiceFlag;
		#endregion

		#region Private data members
		[DataMember] private Guid assetTrackingDeviceGuid;
		[DataMember] private string assetTrackingDeviceId;
		[DataMember] private double? latitude;
		[DataMember] private double? longitude;
		[DataMember] private int? zoom;
		[DataMember] private DeviceTankTypes deviceTankType;
		[DataMember] private int? tankConfigurationNumber;
		#endregion

		#region Constructors
		public TankClass()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		[XmlIgnore]
		public Guid AssetTrackingDeviceGuid
		{
			get { return this.assetTrackingDeviceGuid; }
			set { this.assetTrackingDeviceGuid = value; }
		}

		[QueryWriterField("Asset Tracking Device ID", "TankAssetTrackingDevice.DeviceID", false)]
		public string TrackingDeviceId
		{
			get { return this.assetTrackingDeviceId; }
			set
			{
				this.assetTrackingDeviceId = value;
			}
		}

		[QueryWriterField("Latitude", "tblTanks.Latitude")]
		public double? Latitude
		{
			get { return this.latitude; }
			set { this.latitude = value; }
		}

		[QueryWriterField("Longitude", "tblTanks.Longitude")]
		public double? Longitude
		{
			get { return this.longitude; }
			set { this.longitude = value; }
		}

		[QueryWriterField("Zoom", "tblTanks.Zoom")]
		public int? Zoom
		{
			get { return this.zoom; }
			set { this.zoom = value; }
		}

		[QueryWriterField("Device Tank Type", "tblTanks.LookupDeviceTankTypeIndex")]
		public DeviceTankTypes DeviceTankType
		{
			get { return this.deviceTankType; }
			set { this.deviceTankType = value; }
		}

		[QueryWriterField("Tank Configuration Number", "tblTanks.TankConfigurationNumber")]
		public int? TankConfigurationNumber
		{
			get { return this.tankConfigurationNumber; }
			set { this.tankConfigurationNumber = value; }
		}

		[QueryWriterField("ID", "tblTanks.TankID")]
		public override string ID { get { return this._ID; } set {
			this.SetString("ID", 50, value, ref this._ID); } }

		[XmlIgnore]
		public Guid ProductGuid { get { return this._ProductGuid; } set {
			this._ProductGuid = value; } }

		[QueryWriterField("Product ID", "TankProduct.ProductID", false)]
		public string ProductID { get { return this._ProductID; } set {
			this._ProductID = value; } }

		[QueryWriterField("Product Code", "TankProduct.ProductCode", false)]
		public string ProductCode { get { return this._ProductCode; } set {
			this._ProductCode = value; } }

		[XmlIgnore]
		public Guid ManagerGuid { get { return this._ManagerGuid; } set {
			this._ManagerGuid = value; } }

		[QueryWriterField("Manager", "TankCompany.ID", false)]
		public string ManagerID { get { return this._ManagerID; } set {
			this._ManagerID = value; } }

		[QueryWriterField("Manager Code", "TankCompany.Code", false)]
		public string ManagerCode { get { return this._ManagerCode; } set {
			this._ManagerCode = value; } }

        [XmlIgnore]
        [DataMember]
        public Guid OwnerGuid { get; set; }

        [DataMember]
        [QueryWriterField("Owner", "TankOwner.ID", false)]
        public string OwnerID { get; set; }

        [DataMember]
        [QueryWriterField("Owner Code", "TankOwner.Code", false)]
        public string OwnerCode { get; set; }

        [QueryWriterField("Vessel Type", "tblTanks.LookupVesselTypeIndex")]
		public VESSEL_TYPE VesselType { get { return this._VesselType; } set {
			this._VesselType = value; } }

		[QueryWriterField("Maintenance Note", "E.Memo")]
		public string MaintenanceNote { get { return this._MaintenanceNote; } set {
			this._MaintenanceNote = value; } }

		[QueryWriterField("QC Note", "F.Memo")]
		public string QCNote { get { return this._QCNote; } set {
			this._QCNote = value; } }

		[QueryWriterField("In Service Flag", "E.InServiceFlag")]
		public bool InServiceFlag
		{
			get { return this._InServiceFlag; }
			set {
				this._InServiceFlag = value; }
		}

		/// <summary>
		/// Represents the date + time that this tank was hidden
		/// A null value indicates the tank is not hidden.
		/// Although this field is stored as a datetime it is represented to users
		/// as a checkbox. 
		/// </summary>
		[DataMember]
		public DateTimeOffset? HiddenDate { get; set; }

		[XmlIgnore]
		public override ENTITY_TYPE EntityType => ENTITY_TYPE.TANK;

	    [XmlIgnore]
		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

	    public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTanks " +
				"(SiteGuid," +
				"TankID," +
				"ProductGuid," +
				"LookupVesselTypeIndex," +
				"ManagerCompanyGuid," +
				"HiddenDate," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"TankGuid," +
				"Latitude," +
				"Longitude," +
				"Zoom," +
				"LookupDeviceTankTypeIndex, " +
				"AssetTrackingDeviceGuid, " +
				"TankConfigurationNumber," +
                "OwnerCompanyGuid" +
				") VALUES (" +
				"@SiteGuid," +
				"@TankID," +
				"@ProductGuid," +
				"@LookupVesselTypeIndex," +
				"@ManagerCompanyGuid," +
				"@HiddenDate," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@TankGuid," +
				"@Latitude," +
				"@Longitude," +
				"@Zoom," +
				"@LookupDeviceTankTypeIndex, " +
				"@AssetTrackingDeviceGuid, " +
				"@TankConfigurationNumber," +
                "@OwnerCompanyGuid" +
				") ";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@TankID", this._ID);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = this._ProductGuid == Guid.Empty ? DBNull.Value : (object)this._ProductGuid;
			cmd.Parameters.AddWithValue("@LookupVesselTypeIndex", ((int)this._VesselType));
			cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier).Value = this._ManagerGuid == Guid.Empty ? DBNull.Value : (object)this._ManagerGuid;
			cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset).Value = this.HiddenDate ?? (object)DBNull.Value;
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@TankGuid", this._IdentityGuid);
			cmd.Parameters.Add("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier).Value = this.assetTrackingDeviceGuid == Guid.Empty ? DBNull.Value : (object)this.assetTrackingDeviceGuid;
			cmd.Parameters.AddWithValue("@LookupDeviceTankTypeIndex", ((int)this.deviceTankType));
			cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = this.Latitude ?? (object)DBNull.Value;
			cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = this.Longitude ?? (object)DBNull.Value;
			cmd.Parameters.Add("@Zoom", SqlDbType.Int).Value = this.Zoom ?? (object)DBNull.Value;
			cmd.Parameters.Add("@TankConfigurationNumber", SqlDbType.Int).Value = this.TankConfigurationNumber ?? (object)DBNull.Value;
            cmd.Parameters.Add("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier).Value = this.OwnerGuid == Guid.Empty ? DBNull.Value : (object)this.OwnerGuid;
        }

        public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText =	"UPDATE tblTanks " +
								"SET SiteGuid = @SiteGuid, " +
								"TankID = @TankID, " +
								"ProductGuid = @ProductGuid, " +
								"LookupVesselTypeIndex = @LookupVesselTypeIndex, " +
								"ManagerCompanyGuid = @ManagerCompanyGuid, " +
								"Latitude = @Latitude, " +
								"Longitude = @Longitude, " +
								"Zoom = @Zoom, " +
								"LookupDeviceTankTypeIndex = @LookupDeviceTankTypeIndex, " +
								"AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid, " +
								"TankConfigurationNumber = @TankConfigurationNumber, " +
								"HiddenDate = @HiddenDate, " +
								"UpdatedDate = @UpdatedDate, " +
								"UpdatedBy = @UpdatedBy, " +
                                "OwnerCompanyGuid = @OwnerCompanyGuid " +
								"WHERE TankGuid = @TankGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@TankID", this._ID);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = this._ProductGuid == Guid.Empty ? DBNull.Value : (object)this._ProductGuid;
			cmd.Parameters.AddWithValue("@LookupVesselTypeIndex", ((int)this._VesselType));
			cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier).Value = this._ManagerGuid == Guid.Empty ? DBNull.Value : (object)this._ManagerGuid;
			cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset).Value = this.HiddenDate ?? (object)DBNull.Value;
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@TankGuid", this._IdentityGuid);
			cmd.Parameters.Add("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier).Value = this.assetTrackingDeviceGuid == Guid.Empty ? DBNull.Value : (object)this.assetTrackingDeviceGuid;
			cmd.Parameters.AddWithValue("@LookupDeviceTankTypeIndex", ((int)this.deviceTankType));
			cmd.Parameters.Add("@Latitude", SqlDbType.Float).Value = this.Latitude ?? (object)DBNull.Value;
			cmd.Parameters.Add("@Longitude", SqlDbType.Float).Value = this.Longitude ?? (object)DBNull.Value;
			cmd.Parameters.Add("@Zoom", SqlDbType.Int).Value = this.Zoom ?? (object)DBNull.Value;
			cmd.Parameters.Add("@TankConfigurationNumber", SqlDbType.Int).Value = this.TankConfigurationNumber ?? (object)DBNull.Value;
            cmd.Parameters.Add("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier).Value = this.OwnerGuid == Guid.Empty ? DBNull.Value : (object)this.OwnerGuid;
        }

        public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTanks WHERE TankGuid = @TankGuid";
			cmd.Parameters.AddWithValue("@TankGuid", this._IdentityGuid);
		}

		public void EnumerateByManagerSQL(SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblTanks" +
			" LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid 			 = tblTanks.[TankGuid] " +
			" LEFT JOIN tblTestSetTankResults F ON F.TankGuid 			 = tblTanks.[TankGuid] " +
			" LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo FROM tblTankQualityTagLog HH " +
			"           LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
			"           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  " +
			"           WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G ON G.TankGuid = tblTanks.[TankGuid] " +
				" WHERE ManagerCompanyGuid = @ManagerCompanyGuid" +
			" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " +
			" AND (F.ResultTimeStamp        IS NULL OR F.ResultTimeStamp        = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) " +
			" ORDER BY tblTanks.TankID";

			cmd.Parameters.AddWithValue("@ManagerCompanyGuid", this._ManagerGuid);
			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, bool hideHiddenTanks = false)
		{
			cmd.CommandText = this.SelectClause +
					" FROM tblTanks " +
				" LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblTestSetTankResults F ON F.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblAssetTrackingDevice atd ON atd.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid] " +
				" LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo FROM tblTankQualityTagLog HH " +
				"           LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
				"           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  " +
				"           WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G ON G.TankGuid = tblTanks.[TankGuid] " +
					" WHERE tblTanks.SiteGuid = @SiteGuid" +
				" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " +
				" AND (F.ResultTimeStamp IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) " +
				(hideHiddenTanks ? " AND tblTanks.HiddenDate IS NULL " : string.Empty) +
				" ORDER BY tblTanks.TankID";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
		}
		#endregion

		#region Select clause
		protected string SelectClause = "SELECT *," +
												 " E.EstReturnToServiceDate AS ReturnToServiceDate, E.MaintenanceReason AS StatusDescription, E.InServiceFlag, E.Memo AS MaintenanceNote, " +
												 " ISNULL('QC Tag Memo: ' + G.Memo + CHAR(0x0d) + CHAR(0x0d), '') + ISNULL( 'Test Result Memo: ' + F.Memo, '') as QCNote, " +
												 " G.QualityTagGuid, G.SiteGuid AS QualitySiteGuid, G.Name, G.Severity, G.Active, " +
												 "(SELECT p.ProductID FROM tblProducts p INNER JOIN [erv].[udf_GetProductRecordVersions](tblTanks.SiteGuid) rp ON p.ProductGuid = rp.ProductGuid WHERE p._MasterRecordGuid = tblTanks.ProductGuid) AS ProductID," +
												 "(SELECT p.ProductCode FROM tblProducts p INNER JOIN [erv].[udf_GetProductRecordVersions](tblTanks.SiteGuid) rp ON p.ProductGuid = rp.ProductGuid WHERE p._MasterRecordGuid = tblTanks.ProductGuid) AS ProductCode," +
												 "(SELECT ID FROM tblCompanies c INNER JOIN [erv].[udf_GetCompanyRecordVersions](tblTanks.SiteGuid) rc ON c.CompanyGuid= rc.CompanyGuid WHERE c._MasterRecordGuid = tblTanks.ManagerCompanyGuid) AS ManagerID," +
												 "(SELECT Code FROM tblCompanies c INNER JOIN [erv].[udf_GetCompanyRecordVersions](tblTanks.SiteGuid) rc ON c.CompanyGuid= rc.CompanyGuid WHERE c._MasterRecordGuid = tblTanks.ManagerCompanyGuid) AS ManagerCode," +
												 "(SELECT ID FROM tblCompanies c INNER JOIN [erv].[udf_GetCompanyRecordVersions](tblTanks.SiteGuid) rc ON c.CompanyGuid= rc.CompanyGuid WHERE c._MasterRecordGuid = tblTanks.OwnerCompanyGuid) AS OwnerID," +
												 "(SELECT Code FROM tblCompanies c INNER JOIN [erv].[udf_GetCompanyRecordVersions](tblTanks.SiteGuid) rc ON c.CompanyGuid= rc.CompanyGuid WHERE c._MasterRecordGuid = tblTanks.OwnerCompanyGuid) AS OwnerCode";
		#endregion


		public override void Reset()
		{
			base.Reset();

			this._ProductGuid				= Guid.Empty;
			this._VesselType				= VESSEL_TYPE.MAX_VESSEL;
			this._ManagerGuid				= Guid.Empty;
			this._ProductID					= string.Empty;
			this._ProductCode				= string.Empty;
			this._ManagerID					= string.Empty;
			this._ManagerCode				= string.Empty;
			this.HiddenDate					= null;
			this._InServiceFlag				= true;
			this._MaintenanceNote			= string.Empty;
			this._QCNote					= string.Empty;
			this.assetTrackingDeviceGuid	= Guid.Empty;
			this.assetTrackingDeviceId		= string.Empty;
			this.deviceTankType				= DeviceTankTypes.Opc;
			this.Latitude					= null;
			this.Longitude					= null;
			this.Zoom						= null;
			this.TankConfigurationNumber	= null;
		    this.OwnerGuid                  = Guid.Empty;
		    this.OwnerID = string.Empty;
		    this.OwnerCode = string.Empty;

			this.QualityTag = new QualityTagClass();
			this.ProcessVariableCollection = new ProcessVariableCollectionClass();

			PROCESS_VARIABLE_TYPE[] pvType ={PROCESS_VARIABLE_TYPE.LEVEL_PV,
														PROCESS_VARIABLE_TYPE.TEMPERATURE_PV,
														PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV,
														PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV,
														PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV,
														PROCESS_VARIABLE_TYPE.NET_VOLUME_PV,
														PROCESS_VARIABLE_TYPE.DENSITY_PV,
														PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV,
														PROCESS_VARIABLE_TYPE.MASS_PV,
														PROCESS_VARIABLE_TYPE.VCF_PV,
														PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV,
														PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV,
														PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV,
														PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV,
														PROCESS_VARIABLE_TYPE.TANK_STATUS_PV,
														PROCESS_VARIABLE_TYPE.UNDEFINED_PV};

			EngineeringUnit[] unit ={  EngineeringUnit.FmlFtIn16Th,
									EngineeringUnit.FmtDegF,
									EngineeringUnit.FmvUsGal,
									EngineeringUnit.FmvUsGal,
									EngineeringUnit.FmvUsGal,
									EngineeringUnit.FmvUsGal,
									EngineeringUnit.FmdDegApi,
									EngineeringUnit.FmdDegApi,
									EngineeringUnit.FmmLb,
									0,
									0,
									EngineeringUnit.FmpPsi,
									EngineeringUnit.FmvUsGal,
									EngineeringUnit.FmvUsGal,
									0,
									0};


			int pv = 0;

			while (pvType[pv] != PROCESS_VARIABLE_TYPE.UNDEFINED_PV)
			{
				ProcessVariableClass processVariable = new ProcessVariableClass
				                                       {
					                                       ServerUnits = unit[pv],
					                                       UnitType = UNIT_TYPE.TANK_UNIT,
					                                       ProcessVariableType = pvType[pv]
				                                       };


				if (pvType[pv] == PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV || pvType[pv] == PROCESS_VARIABLE_TYPE.TANK_STATUS_PV)
				{
					processVariable.DataType = VarEnum.VT_BSTR;
					processVariable.SetMaximum(string.Empty, 0);
					processVariable.SetMinimum(string.Empty, 0);
				}
				else
				{
					processVariable.DataType = VarEnum.VT_R8;

					if (pvType[pv] == PROCESS_VARIABLE_TYPE.VCF_PV)
						processVariable.SetMaximum(2.0, unit[pv]);
					else if (pvType[pv] == PROCESS_VARIABLE_TYPE.DENSITY_PV || pvType[pv] == PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV)
						processVariable.SetMaximum(50.0, unit[pv]);
					else if (pvType[pv] == PROCESS_VARIABLE_TYPE.LEVEL_PV)
						processVariable.SetMaximum(30.0, unit[pv]);
					else if (pvType[pv] == PROCESS_VARIABLE_TYPE.TEMPERATURE_PV)
						processVariable.SetMaximum(200.0, unit[pv]);
					else if (pvType[pv] == PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV)
						processVariable.SetMaximum(20, unit[pv]);
					else
						processVariable.SetMaximum(1000000.0, unit[pv]);

					processVariable.SetMinimum(0.0, unit[pv]);
				}

				processVariable.DataTypeEnabled = false;
				processVariable.Input = true;
				processVariable.InputEnabled = false;
				this.ProcessVariableCollection.Add(processVariable);
				pv++;
			}
		}

		public static string VesselTypeID(VESSEL_TYPE vesselType)
		{
			switch (vesselType)
			{
				case VESSEL_TYPE.SPHERICAL_VESSEL:
					return "Spherical";
				case VESSEL_TYPE.CYLINDRICAL_VESSEL:
					return "Cylindrical";
				case VESSEL_TYPE.BULLET_VESSEL:
					return "Bullet";
				case VESSEL_TYPE.PROPANE_VESSEL:
					return "Propane";
				case VESSEL_TYPE.UNDERGROUND_VESSEL:
					return "Underground";
				case VESSEL_TYPE.TANKER_VESSEL:
					return "Tanker";
				case VESSEL_TYPE.PIPELINE_VESSEL:
					return "Pipeline";
				case VESSEL_TYPE.COLLAPSIBLE_STORAGE_TANK:
					return "Collapsible Tank";
				case VESSEL_TYPE.OTHER_VESSEL:
					return "Other";
				default:
					return "Undefined";
			}
		}

		public static string DeviceTankTypeName(DeviceTankTypes deviceTankType)
		{
			switch (deviceTankType)
			{
				case DeviceTankTypes.Opc:
					return "OPC";
				case DeviceTankTypes.Satellite:
					return "Satellite";
				default:
					return "Satellite";
			}
		}

		/// <summary>
		/// Load basic tank information from a DataRow
		/// </summary>
		/// <param name="row">The DataRow to load tank information for</param>
		public void LoadBasicInformation(DataRow row)
		{
			if (row.Equals(null))
			{
				return;
			}

			this._IdentityGuid				= DataObject.getValue(row["TankGuid"], Guid.Empty);
			this._ID						= DataObject.getValue(row["TankID"], string.Empty);
			this.SiteGuid					= DataObject.getValue(row["SiteGuid"], Guid.Empty);
			this.TankConfigurationNumber	= DataObject.getValue(row["TankConfigurationNumber"], 0);
		}

		// A whole rowset is sent, but we can only process the zeroth row.
		// The caller manages the rowset.
		public override void Load(Object inObj)
		{
			this.Reset();
			var obj = inObj as DataSet;

			if (obj != null)
			{
				DataSet set = obj;
				DataTable table = set.Tables[0];

				if (table.Rows.Count == 0 || table.Columns.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				this._IdentityGuid = DataObject.getValue(row["TankGuid"], Guid.Empty);
				this._SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
				this._ID = DataObject.getValue(row["TankID"], "");
				this._ProductGuid = DataObject.getValue(row["ProductGuid"], Guid.Empty);
				this._VesselType = DataObject.getValue(row["LookupVesselTypeIndex"], VESSEL_TYPE.UNDEFINED_VESSEL);
				this._ManagerGuid = DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty);
				this.HiddenDate = DataObject.getValue<DateTimeOffset?>(row["HiddenDate"], null);
				this._CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
				this._CreatedBy = DataObject.getValue(row["CreatedBy"], ADMIN);
				this._UpdatedDate = DataObject.getValue(row["UpdatedDate"], this._CreatedDate);
				this._UpdatedBy = DataObject.getValue(row["UpdatedBy"], ADMIN);
                this.OwnerGuid = DataObject.getValue(row["OwnerCompanyGuid"], Guid.Empty);

                this._InServiceFlag = DataObject.getValue<byte>(row["InServiceFlag"], 0) != 0;
				this._MaintenanceNote = DataObject.getValue(row["MaintenanceNote"], "");
				this._QCNote = DataObject.getValue(row["QCNote"], "");

				this.QualityTag.IdentityGuid = DataObject.getValue(row["QualityTagGuid"], Guid.Empty);
				this.QualityTag.SiteGuid = DataObject.getValue(row["QualitySiteGuid"], Guid.Empty);
				this.QualityTag.ID = DataObject.getValue(row["Name"], "");
				this.QualityTag.Severity = (QUALITY_SEVERITY_LEVELS)DataObject.getValue(row["Severity"], (short)QUALITY_SEVERITY_LEVELS.CAUTION);
				this.QualityTag.Active = DataObject.getValue(row["Active"], false);

				this.latitude					= row.IsNull("Latitude") ? null : (double?)row["Latitude"];
				this.longitude					= row.IsNull("Longitude") ? null : (double?)row["Longitude"];
				this.zoom						= row.IsNull("Zoom") ? null : (int?)row["Zoom"];
				this.deviceTankType				= row.IsNull("LookupDeviceTankTypeIndex") ? DeviceTankTypes.Opc : (DeviceTankTypes)row["LookupDeviceTankTypeIndex"];
				this.assetTrackingDeviceGuid	= row.IsNull("AssetTrackingDeviceGuid") ? Guid.Empty : (Guid)row["AssetTrackingDeviceGuid"];
				this.assetTrackingDeviceId		= row.IsNull("DeviceID") ? string.Empty : (string)row["DeviceID"];
				this.tankConfigurationNumber	= row.IsNull("TankConfigurationNumber") ? null : (int?)row["TankConfigurationNumber"];


				// From other tables.
				this._ProductID = DataObject.getValue(row["ProductID"], "{None}");
				this._ProductCode = DataObject.getValue(row["ProductCode"], "");
				this._ManagerID = DataObject.getValue(row["ManagerID"], "{None}");
				this._ManagerCode = DataObject.getValue(row["ManagerCode"], "");
                this.OwnerID = DataObject.getValue(row["OwnerID"], "{None}");
                this.OwnerCode = DataObject.getValue(row["OwnerCode"], "");
            }
            else
			{
				var tnkClass = inObj as TankClass;

				if (tnkClass != null)
				{
					TankClass tank					= tnkClass;
					this._IdentityGuid				= tank.IdentityGuid;
					this._SiteGuid					= tank.SiteGuid;
					this._ID						= tank.ID;
					this._ProductGuid				= tank.ProductGuid;
					this._VesselType				= tank.VesselType;
					this._ManagerGuid				= tank.ManagerGuid;
					this._CreatedDate				= tank.CreatedDate;
					this._CreatedBy					= tank.CreatedBy;
					this._UpdatedDate				= tank.UpdatedDate;
					this._UpdatedBy					= tank.UpdatedBy;
					this._ProductID					= tank.ProductID;
					this._ProductCode				= tank.ProductCode;
					this._ManagerID					= tank.ManagerID;
					this._ManagerCode				= tank.ManagerCode;
					this.latitude					= tank.latitude;
					this.longitude					= tank.longitude;
					this.zoom						= tank.zoom;
					this.AssetTrackingDeviceGuid	= tank.assetTrackingDeviceGuid;
					this.deviceTankType				= tank.deviceTankType;
					this.assetTrackingDeviceId		= tank.assetTrackingDeviceId;
					this.tankConfigurationNumber	= tank.tankConfigurationNumber;
				    this.OwnerGuid = tank.OwnerGuid;
				    this.OwnerID = tank.OwnerID;
				    this.OwnerCode = tank.OwnerCode;

					this.ProcessVariableCollection.Clear();

					foreach (ProcessVariableClass existingProcessVariable in tank.ProcessVariableCollection)
					{
						var newProcessVariable = new ProcessVariableClass();
						newProcessVariable.Load(existingProcessVariable);
						this.ProcessVariableCollection.Add(newProcessVariable);
					}
				}
				else
				{
					base.Load(inObj);
				}
			}
		}

		/// <summary>
		/// This method will populate the enumerate where coordinates exists SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command</param>
		/// <param name="siteGuid">The site Guid to retrieve from.</param>
		public void EnumerateWhereCoordinatesExistSql(SqlCommand sqlCommand, Guid siteGuid)
		{
			sqlCommand.CommandText = this.SelectClause
				+ " FROM tblTanks "
				+ " LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid = tblTanks.[TankGuid] " 
				+ " LEFT JOIN tblTestSetTankResults F ON F.TankGuid = tblTanks.[TankGuid] " 
				+ " LEFT JOIN tblAssetTrackingDevice atd ON atd.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid] " 
				+ " LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo FROM tblTankQualityTagLog HH " 
				+ "           LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " 
				+ "           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  " 
				+ "           WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G ON G.TankGuid = tblTanks.[TankGuid] " 
				+ " WHERE (tblTanks.Latitude IS NOT NULL OR tblTanks.Latitude <> '') AND (tblTanks.Longitude IS NOT NULL OR tblTanks.Longitude <> '') "
				+ " AND tblTanks.AssetTrackingDeviceGuid IS NOT NULL "
				+ " AND tblTanks.SiteGuid = @SiteGuid"
				+ " AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " 
				+ " AND (F.ResultTimeStamp IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) " 
				+ " ORDER BY tblTanks.TankID";

			sqlCommand.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblTanks " + SQLUpdateLock(bInTransaction) +
				" LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblTestSetTankResults F ON F.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblAssetTrackingDevice atd ON atd.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid] " +
				" LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo FROM tblTankQualityTagLog HH " +
				"           LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
				"           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  " +
				"           WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G ON G.TankGuid = tblTanks.[TankGuid] " +
				" WHERE tblTanks.TankGuid = @TankGuid " +
				" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " +
				" AND (F.ResultTimeStamp IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) " +
				" ORDER BY tblTanks.TankID";

			cmd.Parameters.AddWithValue("@TankGuid", this._IdentityGuid);
			cmd.Parameters.AddWithValue("@SiteGuid", this._IdentityGuid);
		}

		public void SelectByIDSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblTanks " + SQLUpdateLock(bInTransaction) +
				" LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblTestSetTankResults F ON F.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblAssetTrackingDevice atd ON atd.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid] " +
				" LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo FROM tblTankQualityTagLog HH " +
				"           LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
				"           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  " +
				"           WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G ON G.TankGuid = tblTanks.[TankGuid] " +
				" WHERE tblTanks.SiteGuid = @SiteGuid " + " AND " +
				" tblTanks.TankID = @TankID " +
				" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " +
				" AND (F.ResultTimeStamp  IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) ";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@TankID", this._ID);
		}

		public void EnumerateTanksWithoutQualityTagSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblTanks " +
				" LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblTestSetTankResults F ON F.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblAssetTrackingDevice atd ON atd.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid] " +
				" LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo FROM tblTankQualityTagLog HH " +
				"           LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
				"           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  " +
				"           WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G ON G.TankGuid = tblTanks.[TankGuid] " +
				" WHERE tblTanks.SiteGuid = @SiteGuid " +
				" AND NOT EXISTS(SELECT * FROM tblTankQualityTagLog WHERE TankGuid = tblTanks.TankGuid AND RemovedDate IS NULL AND TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog WHERE TankGuid = tblTanks.TankGuid)) " +
				" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " +
				" AND (F.ResultTimeStamp        IS NULL OR F.ResultTimeStamp        = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) " +
			  " ORDER BY tblTanks.TankID";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
		}

		public void QueryWriterSQL(SqlCommand cmd, SecurityClass security, string selectClause)
		{
			cmd.CommandText = selectClause + "," +
				" TankProduct.ProductID AS 'TankProduct.ProductID'," +
				" TankProduct.ProductCode AS 'TankProduct.ProductCode'," +
                " TankCompany.ID AS 'TankCompany.ID'," +
                " TankCompany.Code AS 'TankCompany.Code'," +
                " TankOwner.ID AS 'TankOwner.ID'," +
                " TankOwner.Code AS 'TankOwner.Code'," +
                " TankAssetTrackingDevice.DeviceID AS 'TankAssetTrackingDevice.DeviceID', " +
				" tblTanks.TankGuid AS EntityGuid" +
				" FROM tblTanks " +
				" LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid = tblTanks.[TankGuid] " +
				" LEFT JOIN tblTestSetTankResults F ON F.TankGuid = tblTanks.[TankGuid] " +
				" LEFT JOIN tblAssetTrackingDevice TankAssetTrackingDevice ON TankAssetTrackingDevice.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid] " +
				" LEFT JOIN (SELECT ProductID, ProductCode, _MasterRecordGuid FROM tblProducts INNER JOIN [erv].[udf_GetProductRecordVersions](@SiteGuid) rp ON tblProducts.ProductGuid = rp.ProductGuid) TankProduct ON TankProduct._MasterRecordGuid = tblTanks.ProductGuid" +
                " LEFT JOIN (SELECT ID, Code, _MasterRecordGuid FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid))) TankCompany ON TankCompany._MasterRecordGuid = tblTanks.ManagerCompanyGuid" +
                " LEFT JOIN (SELECT ID, Code, _MasterRecordGuid FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid))) TankOwner ON TankOwner._MasterRecordGuid = tblTanks.OwnerCompanyGuid" +
                " WHERE tblTanks.SiteGuid = @SiteGuid " +
				" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " +
				" AND (F.ResultTimeStamp IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) ";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
		}

		/// <summary>
		/// This method will return a SQL statement to retrieve the tanks by product
		/// and by the filter.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="filter"></param>
		/// <param name="hideHiddenTanks">If true, only tanks not marked as hidden will be returned</param>
		/// <param name="cmd">SQL command</param>
		/// <returns></returns>
		public void EnumerateByProductAndFilterSQL(SqlCommand cmd, SecurityClass security, string filter, bool hideHiddenTanks = false)
		{
			const string OrderBy = "ORDER BY tblTanks.TankID";
			string sql = this.SelectClause +
								  " FROM tblTanks" +
				" LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblTestSetTankResults F ON F.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblAssetTrackingDevice atd ON atd.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid] " +
				" LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo FROM tblTankQualityTagLog HH " +
				"           LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
				"           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  " +
				"           WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G ON G.TankGuid = tblTanks.[TankGuid] " +
				" WHERE ProductGuid = @ProductGuid " +
				" AND tblTanks.SiteGuid = @SiteGuid " +
				" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " +
				" AND (F.ResultTimeStamp        IS NULL OR F.ResultTimeStamp        = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) ";

			string whereFilter = string.Empty;

			if ((filter != null) && (filter.Trim().Length > 0))
			{
				string searchFilter = "'%" + FuelsManagerExtensions.EscapeLikeClauseCharacters(filter.Trim()) + "%'";
				searchFilter = searchFilter.ToUpper();
				whereFilter = " AND tblTanks.TankID LIKE(UPPER(" + searchFilter + ")) ";
			}

			if (hideHiddenTanks)
			{
				whereFilter += " AND tblTanks.HiddenDate IS NULL ";
			}

			cmd.CommandText = sql + whereFilter + OrderBy;

			cmd.Parameters.AddWithValue("@ProductGuid", this._ProductGuid);
			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}

		/// <summary>
		/// This method will return a SQL statement to retrieve the tanks by the filter.
		/// </summary>
		/// <param name="cmd">SQL Command</param>
		/// <param name="filter"></param>
		/// <param name="hideHiddenTanks">If true, only tanks that are not marked as hidden will be returned</param>
		/// <returns></returns>
		public void EnumerateByFilterSQL(SqlCommand cmd, string filter, bool hideHiddenTanks = false)
		{
			const string OrderBy = "ORDER BY tblTanks.TankID";
			string sql = this.SelectClause +
								  " FROM tblTanks " +
				" LEFT JOIN tblTankMaintenanceLog E ON E.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblTestSetTankResults F ON F.TankGuid 			 = tblTanks.[TankGuid] " +
				" LEFT JOIN tblAssetTrackingDevice atd ON atd.AssetTrackingDeviceGuid = tblTanks.[AssetTrackingDeviceGuid] " +
				" LEFT JOIN (SELECT GG.*, TankGuid, HH.Memo FROM tblTankQualityTagLog HH " +
				"           LEFT JOIN tblQualityTags GG  ON GG.QualityTagGuid = HH.QualityTagGuid WHERE RemovedDate IS NULL " +
				"           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  " +
				"           WHERE tblTankQualityTagLog.TankGuid = HH.TankGuid )) G ON G.TankGuid = tblTanks.[TankGuid] " +
			  " WHERE tblTanks.SiteGuid = @SiteGuid " +
				" AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankGuid = E.TankGuid)) " +
				" AND (F.ResultTimeStamp        IS NULL OR F.ResultTimeStamp        = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankGuid = F.TankGuid)) ";

			string whereFilter = string.Empty;

			if ((filter != null) && (filter.Trim().Length > 0))
			{
				string searchFilter = "'%" + FuelsManagerExtensions.EscapeLikeClauseCharacters(filter.Trim()) + "%'";
				searchFilter = searchFilter.ToUpper();
				whereFilter = " AND tblTanks.TankID LIKE(UPPER(" + searchFilter + ")) ";
			}

			if (hideHiddenTanks)
			{
				whereFilter += " AND tblTanks.HiddenDate IS NULL ";
			}

			cmd.CommandText = sql + whereFilter + OrderBy;

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
		}

		/// <summary>
		/// Get only basic information like the ID and TankGuid for all tanks for a specified site
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="cmd">A SqlCommand to populate</param>
		public void EnumerateBasicInformation(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandText = "SELECT TankGuid, TankID, SiteGuid, TankConfigurationNumber FROM tblTanks WHERE SiteGuid = @SiteGuid";
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
		}

		/// <summary>
		/// This method will populate the get basic tank info for tank associated to an
		/// Asset Tracking Device ID.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDeviceId">The Asset Tracking Device ID.</param>
		public void EnumerateBasicInfoLinkedToAssetTrackingDevicesSQL(SqlCommand sqlCommand, string inAssetTrackingDeviceId)
		{
			sqlCommand.CommandText = "SELECT t.TankGuid, t.TankID, t.SiteGuid, t.TankConfigurationNumber"
									+ " FROM tblTanks t INNER JOIN tblAssetTrackingDevice atd ON t.AssetTrackingDeviceGuid = atd.AssetTrackingDeviceGuid"
									+ " WHERE atd.DeviceID = @DeviceID ";

			var parm = new SqlParameter("@DeviceID", SqlDbType.NVarChar, 30) { Value = inAssetTrackingDeviceId };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the tank configuration number being user SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inTankGuid">The current tank GUID.</param>
		/// <param name="inAssetTrackingDeviceGuid">The asset tracking device Guid to check.</param>
		/// <param name="inTankConfigurationNumber">The tank configuration number to check.</param>
		public void TankConfigurationNumberBeingUsedSQL(SqlCommand sqlCommand, Guid inTankGuid, Guid inAssetTrackingDeviceGuid, int inTankConfigurationNumber)
		{
			sqlCommand.CommandText = "SELECT COUNT(*) UseCount FROM tblTanks"
									+ " WHERE AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid AND TankConfigurationNumber = @TankConfigurationNumber"
									+ " AND TankGuid <> @CurrentTankGuid";

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@TankConfigurationNumber", SqlDbType.Int) { Value = inTankConfigurationNumber };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CurrentTankGuid", SqlDbType.UniqueIdentifier) { Value = inTankGuid };
			sqlCommand.Parameters.Add(parm);
		}

		public SqlCommand EnumerateAuthorizedSqlCmd(SecurityClass security, bool hideHiddenTanks = false)
        {
            throw new NotImplementedException();
            //SqlCommand cmd = new SqlCommand
            //                 {
            //                     CommandText =
            //                         "IF EXISTS (SELECT(1) FROM udf_AuthorizedCompaniesGuid(@SiteGuid, @UserGuid) ac WHERE ac.CompanyIndex <> 0) "
            //                         + "BEGIN " + this.SelectClause + " FROM tblTanks "
            //                         + " LEFT JOIN tblTankMaintenanceLog E ON E.TankIndex 			 = tblTanks.[TankIndex] "
            //                         + " LEFT JOIN tblTestSetTankResults F ON F.TankIndex 			 = tblTanks.[TankIndex] "
            //                         + " LEFT JOIN (SELECT GG.*, TankIndex, HH.Memo FROM tblTankQualityTagLog HH "
            //                         + "           LEFT JOIN tblQualityTags GG  ON GG.QualityTagIndex = HH.QualityTagIndex WHERE RemovedDate IS NULL "
            //                         + "           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  "
            //                         + "           WHERE tblTankQualityTagLog.TankIndex = HH.TankIndex )) G ON G.TankIndex = tblTanks.[TankIndex] "
            //                         + " WHERE tblTanks.SiteIndex = @SiteIndex AND "
            //                         + "(ManagerIndex IN (SELECT CompanyIndex FROM dbo.AuthorizedCompaniesIndex(@LoginSiteIndex, @SiteIndex, @UserIndex)) OR "
            //                         + "OwnerIndex IN (SELECT CompanyIndex FROM dbo.AuthorizedCompaniesIndex(@LoginSiteIndex, @SiteIndex, @UserIndex)))"
            //                         + " AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankIndex = E.TankIndex)) "
            //                         + " AND (F.ResultTimeStamp IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankIndex = F.TankIndex)) "
            //                         + " ORDER BY tblTanks.TankID " + " END " + "ELSE " + "BEGIN "
            //                         + this.SelectClause + " FROM tblTanks "
            //                         + " LEFT JOIN tblTankMaintenanceLog E ON E.TankIndex 			 = tblTanks.[TankIndex] "
            //                         + " LEFT JOIN tblTestSetTankResults F ON F.TankIndex 			 = tblTanks.[TankIndex] "
            //                         + " LEFT JOIN (SELECT GG.*, TankIndex, HH.Memo FROM tblTankQualityTagLog HH "
            //                         + "           LEFT JOIN tblQualityTags GG  ON GG.QualityTagIndex = HH.QualityTagIndex WHERE RemovedDate IS NULL "
            //                         + "           AND  HH.TaggedDate = (SELECT MAX(TaggedDate) FROM tblTankQualityTagLog  "
            //                         + "           WHERE tblTankQualityTagLog.TankIndex = HH.TankIndex )) G ON G.TankIndex = tblTanks.[TankIndex] "
            //                         + " WHERE tblTanks.SiteIndex = @SiteIndex "
            //                         + " AND (E.ChangeDate IS NULL OR E.ChangeDate = (SELECT MAX(ChangeDate) FROM tblTankMaintenanceLog WHERE tblTankMaintenanceLog.TankIndex = E.TankIndex)) "
            //                         + " AND (F.ResultTimeStamp IS NULL OR F.ResultTimeStamp = (SELECT MAX(ResultTimeStamp) FROM tblTestSetTankResults WHERE tblTestSetTankResults.TankIndex = F.TankIndex)) "
            //                         + " ORDER BY tblTanks.TankID " + " END "
            //                 };


            //cmd.Parameters.AddWithValue("@SiteIndex", security.SiteIndex);
            //cmd.Parameters.AddWithValue("@LoginSiteIndex", security.LoginSiteIndex);
            //cmd.Parameters.AddWithValue("@UserIndex", security.UserIndex);

            //return cmd;
        }

        public QueryWriterFieldCollection QueryAliasFields(SecurityClass security, QueryWriterFieldCollection fields)
		{
			QueryClass.ApplyDataDictionary(security, fields);
			return fields;
		}

		/// <summary>
		/// This method is used when the edit button is clicked on the query writer results form
		/// </summary>
		/// <returns>The page corresponding to this entity</returns>
		public string DetailPageReference()
		{
			return "/FMWebApp/TankForm.aspx";
		}
	}
}
