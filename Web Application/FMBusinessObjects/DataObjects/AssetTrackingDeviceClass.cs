namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    [DataContract]
	[Serializable]
	public class AssetTrackingDeviceClass : BaseDataObject
	{
		#region Public
		/// <summary>
		/// Indicates the asset tracking device types.  Must be in the same
		/// order as in lookup.tblAssetTrackingDeviceType table.
		/// </summary>
		public enum AssetTrackingDeviceTypes { Tdu, Wrdcu, Standard };
		#endregion

		#region Private data members
		[DataMember] private Guid assetTrackingDeviceGuid;
		[DataMember] private string deviceId;
		[DataMember] private string modelNumber;
		[DataMember] private string serialNumber;
		[DataMember] private bool active;
		[DataMember] private string description;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		[DataMember] private string equipmentId;
		[DataMember] private Guid equipmentGuid;
		[DataMember] private Guid equipmentSiteGuid;
		[DataMember] private double? equipmentVolume;
		[DataMember] private int? equipmentDensityUnitIndex;
		[DataMember] private int? equipmentVolumeUnitIndex;
		[DataMember] private string productId;
		[DataMember] private double? productDensity;
		[DataMember] private double? productDielectricTolerance;
		[DataMember] private int? productDensityUnitIndex;
		[DataMember] private int? productVolumeUnitIndex;
		[DataMember] private AssetTrackingDeviceTypes assetTrackingDeviceType;
		[DataMember] private EngineeringUnit sourceUnit;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingDeviceClass()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid AssetTrackingDeviceGuid
		{
			get { return this.assetTrackingDeviceGuid; }
			set
			{
				this.assetTrackingDeviceGuid = value;
				this.IdentityGuid = this.assetTrackingDeviceGuid;
			}
		}

		public string AssetTrackingDeviceGuidStr
		{
			get { return this.assetTrackingDeviceGuid.ToString(); }
			set { this.assetTrackingDeviceGuid = Guid.Parse(value); }
		}

		public string DeviceId
		{
			get { return this.deviceId; }
			set
			{
				this.deviceId = value;
				this.ID = this.deviceId;
			}
		}

		public string ModelNumber
		{
			get { return this.modelNumber; }
			set { this.modelNumber = value; }
		}

		public string SerialNumber
		{
			get { return this.serialNumber; }
			set { this.serialNumber = value; }
		}

		public bool Active
		{
			get { return this.active; }
			set { this.active = value; }
		}

		public string Description
		{
			get { return this.description; }
			set { this.description = value; }
		}

		public string AssetTrackingDeviceToolTip
		{
			get
			{
				string toolTip = this.DeviceId;

				if (string.IsNullOrEmpty(this.description) == false)
				{
					toolTip = toolTip + ", " + this.description;
				}

				return toolTip;
			}
		}

		public string EquipmentGuidStr
		{
			get { return this.equipmentGuid.ToString(); }
			set { this.equipmentGuid = Guid.Parse(value); }
		}

		public Guid EquipmentGuid
		{
			get { return this.equipmentGuid; }
			set { this.equipmentGuid = value; }
		}

		public Guid EquipmentSiteGuid
		{
			get { return this.equipmentSiteGuid; }
			set { this.equipmentSiteGuid = value; }
		}

		public string EquipmentId
		{
			get { return this.equipmentId; }
			set { this.equipmentId = value; }
		}

		public double? EquipmentVolume
		{
			get { return this.equipmentVolume; }
			set { this.equipmentVolume = value; }
		}
		public int? EquipmentDensityUnitIndex
		{
			get { return this.equipmentDensityUnitIndex; }
			set { this.equipmentDensityUnitIndex = value; }
		}

		public int? EquipmentVolumeUnitIndex
		{
			get { return this.equipmentVolumeUnitIndex; }
			set { this.equipmentVolumeUnitIndex = value; }
		}

		public AssetTrackingDeviceTypes AssetTrackingDeviceType
		{
			get { return this.assetTrackingDeviceType; }
			set { this.assetTrackingDeviceType = value; }
		}

		public string ProductId
		{
			get { return this.productId; }
			set { this.productId = value; }
		}

		public double? ProductDensity
		{
			get { return this.productDensity; }
			set { this.productDensity = value; }
		}

		public double? ProductDielectricTolerance
		{
			get { return this.productDielectricTolerance; }
			set { this.productDielectricTolerance = value; }
		}

		public int? ProductDensityUnitIndex
		{
			get { return this.productDensityUnitIndex; }
			set { this.productDensityUnitIndex = value; }
		}

		public int? ProductVolumeUnitIndex
		{
			get { return this.productVolumeUnitIndex; }
			set { this.productVolumeUnitIndex = value; }
		}

		public EngineeringUnit SourceUnit
		{
			get { return this.sourceUnit; }
			set { this.sourceUnit = value; }
		}

		public override ENTITY_TYPE EntityType => ENTITY_TYPE.ASSET_TRACKING_DEVICE;

		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;
		#endregion

		#region Public methods
		/// <summary>
		/// This methods populates the insert SQL command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void InsertSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "INSERT INTO tblAssetTrackingDevice (" 
									 + " AssetTrackingDeviceGuid," 
									 + " SiteGuid,"
									 + " DeviceID,"
			                         + " ModelNumber," 
									 + " SerialNumber," 
									 + " Active," 
									 + " Description, "
									 + " LookupAssetTrackingDeviceTypeIndex,"
									 + " LookupEngineeringUnitIndex,"
									 + " CreatedDate," 
									 + " CreatedBy,"
			                         + " UpdatedDate," 
									 + " UpdatedBy" 
									 + ") VALUES (" 
									 + " @AssetTrackingDeviceGuid,"
									 + " @SiteGuid,"
			                         + " @DeviceID," 
									 + " @ModelNumber," 
									 + " @SerialNumber," 
									 + " @Active," 
									 + " @Description,"
									 + " @LookupAssetTrackingDeviceTypeIndex,"
									 + " @LookupEngineeringUnitIndex,"
									 + " @CreatedDate,"
			                         + " @CreatedBy," 
									 + " @UpdatedDate," 
									 + " @UpdatedBy" 
									 + " ) ";

			this.AssetTrackingDeviceGuid = Guid.NewGuid();
			this.createdDate			= DateTimeOffset.Now;
			this.updatedDate			= this.createdDate;
			this.CreatedBy				= security.UserID;
			this.UpdatedBy				= security.UserID;
			this.SiteGuid				= security.SiteGuid;

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.SiteGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@DeviceID", SqlDbType.NVarChar, 30) { Value = this.DeviceId };
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.modelNumber))
			{
				parm = new SqlParameter("@ModelNumber", SqlDbType.NVarChar, 50) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@ModelNumber", SqlDbType.NVarChar, 50) { Value = this.modelNumber };
				sqlCommand.Parameters.Add(parm);
			}

			if (string.IsNullOrEmpty(this.serialNumber))
			{
				parm = new SqlParameter("@SerialNumber", SqlDbType.NVarChar, 50) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@SerialNumber", SqlDbType.NVarChar, 50) { Value = this.serialNumber };
				sqlCommand.Parameters.Add(parm);
			}

			parm = new SqlParameter("@Active", SqlDbType.Bit) { Value = this.active };
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.description))
			{
				parm = new SqlParameter("@Description", SqlDbType.NVarChar, 50) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@Description", SqlDbType.NVarChar, 50) { Value = this.description };
				sqlCommand.Parameters.Add(parm);
			}

			parm = new SqlParameter("@LookupAssetTrackingDeviceTypeIndex", SqlDbType.Int) { Value = this.assetTrackingDeviceType };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupEngineeringUnitIndex", SqlDbType.Int) { Value = (int) this.sourceUnit };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.createdDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 50) { Value = this.CreatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 50) { Value = this.UpdatedBy };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This methods populates the update SQL command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void UpdateSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "UPDATE tblAssetTrackingDevice SET"
									 + " DeviceID = @DeviceID,"
									 + " ModelNumber = @ModelNumber,"
									 + " SerialNumber = @SerialNumber,"
									 + " Active = @Active,"
									 + " Description = @Description,"
									 + " LookupAssetTrackingDeviceTypeIndex = @LookupAssetTrackingDeviceTypeIndex,"
									 + " LookupEngineeringUnitIndex = @LookupEngineeringUnitIndex,"
									 + " UpdatedDate = @UpdatedDate,"
									 + " UpdatedBy = @UpdatedBy"
									 + " WHERE AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid ";

			this.UpdatedBy = security.UserID;
			this.updatedDate = DateTimeOffset.Now;

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@DeviceID", SqlDbType.NVarChar, 30) { Value = this.DeviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Active", SqlDbType.Bit) { Value = this.active };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 50) { Value = this.UpdatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupAssetTrackingDeviceTypeIndex", SqlDbType.Int) { Value = this.assetTrackingDeviceType };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupEngineeringUnitIndex", SqlDbType.Int) { Value = (int) this.sourceUnit };
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.description))
			{
				parm = new SqlParameter("@Description", SqlDbType.NVarChar, 50) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@Description", SqlDbType.NVarChar, 50) { Value = this.description };
				sqlCommand.Parameters.Add(parm);				
			}

			if (string.IsNullOrEmpty(this.modelNumber))
			{
				parm = new SqlParameter("@ModelNumber", SqlDbType.NVarChar, 50) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);			
			}
			else
			{
				parm = new SqlParameter("@ModelNumber", SqlDbType.NVarChar, 50) { Value = this.modelNumber };
				sqlCommand.Parameters.Add(parm);					
			}

			if (string.IsNullOrEmpty(this.serialNumber))
			{
				parm = new SqlParameter("@SerialNumber", SqlDbType.NVarChar, 50) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@SerialNumber", SqlDbType.NVarChar, 50) { Value = this.serialNumber };
				sqlCommand.Parameters.Add(parm);
			}
		}

		/// <summary>
		/// This method will populate the Enumerate SQL command that is used for entity
		/// assignment.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void EnumerateSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.*" 
					+ " FROM tblAssetTrackingDevice " + SQLUpdateLock(false)
					+ " WHERE " + this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid") 
					+ " ORDER BY DeviceID";
		}

		/// <summary>
		/// This method will populate the enumerate active devices SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void EnumerateActiveSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.* FROM tblAssetTrackingDevice " + SQLUpdateLock(false)
									+ " WHERE " + this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid")
									+ " AND tblAssetTrackingDevice.Active = 1 ";
		}

		/// <summary>
		/// This method will populate the enumerate all active unassigned devices.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void EnumerateAllUnassignedActiveDevicesSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.* "
									+ " FROM tblAssetTrackingDevice " + SQLUpdateLock(false)
									+ " LEFT OUTER JOIN tblEquipment ON tblAssetTrackingDevice.AssetTrackingDeviceGuid = tblEquipment.AssetTrackingDeviceGuid"
									+ " LEFT OUTER JOIN lookup.tblAssetTrackingDeviceType ON tblAssetTrackingDevice.LookupAssetTrackingDeviceTypeIndex = lookup.tblAssetTrackingDeviceType.AssetTrackingDeviceTypeIndex"
									+ " LEFT OUTER JOIN lookup.tblEngineeringUnit ON tblAssetTrackingDevice.LookupEngineeringUnitIndex = lookup.tblEngineeringUnit.EngineeringUnitIndex"
									+ " WHERE " + this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid")
									+ " AND tblAssetTrackingDevice.Active = 1 "
									+ " AND lookup.tblAssetTrackingDeviceType.AssetTrackingDeviceTypeCode <> 'TDU'"
									+ " AND tblEquipment.AssetTrackingDeviceGuid IS NULL ";
		}

		/// <summary>
		/// This method will populate the enumerate all devices SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void EnumerateAllSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.* "
									+ " FROM tblAssetTrackingDevice "
									+ " LEFT OUTER JOIN lookup.tblAssetTrackingDeviceType atdt ON tblAssetTrackingDevice.LookupAssetTrackingDeviceTypeIndex = atdt.AssetTrackingDeviceTypeIndex "
									+ " LEFT OUTER JOIN lookup.tblEngineeringUnit ON tblAssetTrackingDevice.LookupEngineeringUnitIndex = lookup.tblEngineeringUnit.EngineeringUnitIndex"
									+ " WHERE (atdt.AssetTrackingDeviceTypeCode = 'STANDARD' OR atdt.AssetTrackingDeviceTypeCode = 'WRDCU')"
									+ " AND " + this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid");
		}

		/// <summary>
		/// This method will populate the enumerate all devices with filter SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="filter">Filter on device ID field.</param>
		/// <param name="security">The security object.</param>
		public void EnumerateAllWithFilterSql(SqlCommand sqlCommand, string filter, SecurityClass security)
		{
			if (string.IsNullOrEmpty(filter))
			{
				sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.* FROM tblAssetTrackingDevice " + SQLUpdateLock(false)
										 + " WHERE " 
										 + this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid");

				return;
			}

			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.* FROM tblAssetTrackingDevice " + SQLUpdateLock(false)
									+ " WHERE tblAssetTrackingDevice.DeviceID LIKE UPPER(@Filter) "
									+ " AND " + this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid");

			var parm = new SqlParameter("@Filter", SqlDbType.NVarChar, 30) { Value = "%" + filter.ToUpper() + "%" };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the enumerate only devices that are linked
		/// to equipment and are active SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void EnumerateOnlyDevicesLinkedToEquipmentSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.*, "
									+ " e.EquipmentGuid, "
									+ " e.ID, "
									+ " e.VolumeUnitIndex AS EquipmentVolumeUnitIndex," 
									+ " e.DensityUnitIndex AS EquipmentDensityUnitIndex, "
									+ " e.Volume, "
									+ " p.ProductID, "
									+ " p.StandardDensity AS Density,"
									+ " p.VolumeUnitIndex AS ProductVolumeUnitIndex,"
									+ " p. DensityUnitIndex AS ProductDensityUnitIndex, "
									+ " p.DielectricTolerance "
									+ " FROM (tblAssetTrackingDevice "
									+ " INNER JOIN tblEquipment e ON tblAssetTrackingDevice.AssetTrackingDeviceGuid = e.AssetTrackingDeviceGuid) "
									+ " LEFT OUTER JOIN tblProducts p ON e.ProductGuid = p.ProductGuid "
									+ " WHERE tblAssetTrackingDevice.Active = 1 AND "
									+ this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid");
		}

		/// <summary>
		/// This method will populate the enumerate only equipment that are not linked
		/// to devices SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inSiteGuid">The site GUID of the device.</param>
		public void EnumerateAllEquipmentNotAssociateToDevicesSql(SqlCommand sqlCommand, Guid inSiteGuid)
		{
			sqlCommand.CommandText = "SELECT e.EquipmentGuid, e.ID, e.Volume "
									+ " FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a"
									+ " INNER JOIN tblEquipment e ON e.EquipmentGuid = a.EquipmentGuid"
									+ " WHERE e.AssetTrackingDeviceGuid IS NULL ";

			var parm = new SqlParameter("@TargetSiteGuid", SqlDbType.UniqueIdentifier) { Value = inSiteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the enumerate all satellite 
		/// devices (i.e. TDU). The device must be active.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void EnumerateAllSatelliateDevicesSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.* "
									 + " FROM tblAssetTrackingDevice LEFT OUTER JOIN "
									 + " lookup.tblAssetTrackingDeviceType atdt ON tblAssetTrackingDevice.LookupAssetTrackingDeviceTypeIndex = atdt.AssetTrackingDeviceTypeIndex "
									 + " LEFT OUTER JOIN lookup.tblEngineeringUnit ON tblAssetTrackingDevice.LookupEngineeringUnitIndex = lookup.tblEngineeringUnit.EngineeringUnitIndex"
									 + " WHERE tblAssetTrackingDevice.Active = 1 AND atdt.AssetTrackingDeviceTypeCode = 'TDU' "
									 + " AND " + this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid");
		}

		/// <summary>
		/// This method will populate the enumerate associated tanks SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDeviceGuid">The deivce GUID that the tanks are associated.</param>
		public void EnumerateAssociatedTanksSql(SqlCommand sqlCommand, Guid inAssetTrackingDeviceGuid)
		{
			sqlCommand.CommandText = "SELECT TankID FROM tblTanks WHERE AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid ";

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the enumerate all associated tanks SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inSiteGuid">The stie GUID that the tanks are associated.</param>
		public void EnumerateAllAssociatedTanksSql(SqlCommand sqlCommand, Guid inSiteGuid)
		{
            //sqlCommand.CommandText = "SELECT * FROM tblTanks"
            //                         + " WHERE SiteGuid = @SiteGuid AND AssetTrackingDeviceGuid IS NOT NULL ";
            sqlCommand.CommandText = "SELECT DISTINCT t.TankID AS TankIDMain,"
                + " (SELECT TOP(1) AssetSessionDateTime FROM tblAssetTrackingDetail"
                + " WHERE AssetTrackingDeviceID = atd.DeviceID"
                + " ORDER BY AssetSessionDateTime DESC) AS AssetSessionDateTime, t.*"
                + " FROM tblTanks t LEFT JOIN tblAssetTrackingDevice atd ON t.AssetTrackingDeviceGuid = atd.AssetTrackingDeviceGuid"
                + " LEFT JOIN tblAssetTrackingDetail d ON atd.DeviceID = d.AssetTrackingDeviceID"
                + " WHERE t.SiteGuid = @SiteGuid AND t.AssetTrackingDeviceGuid IS NOT NULL ";

			var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = inSiteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get identity GUID by device ID SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDeviceId">The device ID to get the GUID.</param>
		/// <param name="security">The security object.</param>
		public void GetIdentityGuidSql(SqlCommand sqlCommand, string inAssetTrackingDeviceId, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.AssetTrackingDeviceGuid FROM tblAssetTrackingDevice " + SQLUpdateLock(false)
									+ " WHERE tblAssetTrackingDevice.DeviceID = @DeviceID AND "
									+ this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingDevice", "AssetTrackingDeviceGuid");

			var parm = new SqlParameter("@DeviceID", SqlDbType.NVarChar, 30) { Value = inAssetTrackingDeviceId };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get identity GUID by device ID SQL without using site.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDeviceId">The device ID to get the GUID.</param>
		/// <param name="security">The security object.</param>
		public void GetIdentityGuidWithoutSiteSql(SqlCommand sqlCommand, string inAssetTrackingDeviceId, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingDevice.AssetTrackingDeviceGuid FROM tblAssetTrackingDevice " + SQLUpdateLock(false)
									+ " WHERE tblAssetTrackingDevice.DeviceID = @DeviceID ";

			var parm = new SqlParameter("@DeviceID", SqlDbType.NVarChar, 30) { Value = inAssetTrackingDeviceId };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get by identity GUID SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDeviceGuid">The device GUID to get the asset tracking device.</param>
		public void GetByIdentityGuidSql(SqlCommand sqlCommand, Guid inAssetTrackingDeviceGuid)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingDevice "
									+ " WHERE AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid ";

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get associated equipment and product ID SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDeviceGuid">The device GUID to get the equipment GUID.</param>
		public void GetAssociatedEquipmentIdAndProductSql(SqlCommand sqlCommand, Guid inAssetTrackingDeviceGuid)
		{
			sqlCommand.CommandText = "SELECT e.ID AS EquipmentID, p.ProductID, p.StandardDensity, p.DielectricTolerance, e.SiteGuid AS EquipmentSiteGuid "
									+ " FROM tblEquipment e LEFT OUTER JOIN tblProducts p ON e.ProductGuid = p.ProductGuid "
									+ " WHERE e.AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid";

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get equipment site Guid SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDeviceGuid">The device GUID to get the equipment site GUID.</param>
		public void GetEquipmentSiteGuidSql(SqlCommand sqlCommand, Guid inAssetTrackingDeviceGuid)
		{
			sqlCommand.CommandText = "SELECT e.SiteGuid AS EquipmentSiteGuid "
									+ " FROM tblEquipment e "
									+ " WHERE e.AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid";

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get associated equipment GUID SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDeviceGuid">The device GUID to get the equipment GUID.</param>
		public void GetAssociatedEquipmentGuidSql(SqlCommand sqlCommand, Guid inAssetTrackingDeviceGuid)
		{
			sqlCommand.CommandText = "SELECT EquipmentGuid FROM tblEquipment WHERE AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid ";

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);			
		}

		/// <summary>
		/// This method will populate the Purge SQL command.
		/// </summary>
		/// <param name="sqlCommand">The SQL Command to populate</param>
		/// <param name="inAssetTrackingDeviceGuid">The asset tracking device GUID to purge.</param>
		public void PurgeSql(SqlCommand sqlCommand, Guid inAssetTrackingDeviceGuid)
		{
			sqlCommand.CommandText = "DELETE FROM tblAssetTrackingDevice WHERE AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid ";

			var parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier)  { Value = inAssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the equipment device reference SQL command.
		/// </summary>
		/// <param name="sqlCommand">The SQL Command to populate</param>
		/// <param name="security">The security object.</param>
		public void UpdateEquipmentDeviceReference(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "UPDATE tblEquipment SET " 
									 + " AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid,"
			                         + " UpdatedBy = @UpdatedBy,"
									 + " UpdatedDate = @UpdatedDate "
									 + " WHERE EquipmentGuid = @EquipmentGuid ";

			var parm = new SqlParameter("@EquipmentGuid", SqlDbType.UniqueIdentifier) { Value = this.EquipmentGuid };
			sqlCommand.Parameters.Add(parm);

			if (this.AssetTrackingDeviceGuid.Equals(Guid.Empty))
			{
				parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);				
			}
			else
			{
				parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingDeviceGuid };
				sqlCommand.Parameters.Add(parm);
			}

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = security.UserID };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = DateTimeOffset.Now };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the remove Tank device reference SQL command.
		/// </summary>
		/// <param name="sqlCommand">The SQL Command to populate</param>
		/// <param name="security">The security object.</param>
		/// <param name="currentAssetTrackingDeviceGuid">The current device GUID associated to the tanks.</param>
		public void RemoveTankDeviceReference(SqlCommand sqlCommand, SecurityClass security, Guid currentAssetTrackingDeviceGuid)
		{
			sqlCommand.CommandText = "UPDATE tblTanks SET "
									 + " AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid,"
									 + " UpdatedBy = @UpdatedBy,"
									 + " UpdatedDate = @UpdatedDate "
									 + " WHERE AssetTrackingDeviceGuid = @CurrentAssetTrackingDeviceGuid ";

			var parm = new SqlParameter("@CurrentAssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = currentAssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetTrackingDeviceGuid", SqlDbType.UniqueIdentifier) { Value = DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = security.UserID };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = DateTimeOffset.Now };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the remove equipment device reference SQL command.
		/// </summary>
		/// <param name="sqlCommand">The SQL Command to populate</param>
		/// <param name="security">The security object.</param>
		public void RemoveEquipmentDeviceReference(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "UPDATE tblEquipment SET "
									 + " AssetTrackingDeviceGuid = @AssetTrackingDeviceRemoveGuid,"
									 + " UpdatedBy = @UpdatedBy,"
									 + " UpdatedDate = @UpdatedDate "
									 + " WHERE AssetTrackingDeviceGuid = @AssetTrackingDeviceWhereGuid ";

			var parm = new SqlParameter("@AssetTrackingDeviceRemoveGuid", SqlDbType.UniqueIdentifier) { Value = DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetTrackingDeviceWhereGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = security.UserID };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = DateTimeOffset.Now };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This emthod will load the object from a database record. It is used when getting
		/// the associated equipment.
		/// </summary>
		/// <param name="row">The record to load.</param>
		public void LoadWithEquipment(DataRow row)
		{
			if (row == null)
			{
				return;
			}

			this.Load(row);

			this.equipmentId				= row.IsNull("ID") ? string.Empty : (string)row["ID"];
			this.equipmentGuid				= row.IsNull("EquipmentGuid") ? Guid.Empty : (Guid)row["EquipmentGuid"];
			this.equipmentVolume			= row.IsNull("Volume") ? null : (double?)row["Volume"];
			this.equipmentDensityUnitIndex	= row.IsNull("EquipmentDensityUnitIndex") ? null : (int?)row["EquipmentDensityUnitIndex"];
			this.equipmentVolumeUnitIndex	= row.IsNull("EquipmentVolumeUnitIndex") ? null : (int?)row["EquipmentVolumeUnitIndex"];
			this.productId					= row.IsNull("ProductID") ? string.Empty : (string)row["ProductID"];
			this.productDensity				= row.IsNull("Density") ? null : (double?)row["Density"];
			this.productDensityUnitIndex	= row.IsNull("ProductDensityUnitIndex") ? null : (int?)row["ProductDensityUnitIndex"];
			this.productVolumeUnitIndex		= row.IsNull("ProductVolumeUnitIndex") ? null : (int?)row["ProductVolumeUnitIndex"];
			this.productDielectricTolerance = row.IsNull("DielectricTolerance") ? null : (double?)row["DielectricTolerance"];
		}

		/// <summary>
		/// This method will load the object from a database record.
		/// </summary>
		/// <param name="row">The record to load.</param>
		public void Load(DataRow row)
		{
			if (row == null)
			{
				return;
			}

			this.AssetTrackingDeviceGuid	= row.IsNull("AssetTrackingDeviceGuid") ? Guid.Empty : (Guid)row["AssetTrackingDeviceGuid"];
			this.SiteGuid					= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
			this.DeviceId					= row.IsNull("DeviceID") ? string.Empty : (string)row["DeviceID"];
			this.modelNumber				= row.IsNull("ModelNumber") ? string.Empty : (string)row["ModelNumber"];
			this.serialNumber				= row.IsNull("SerialNumber") ? string.Empty : (string)row["SerialNumber"];
			this.active						= !row.IsNull("Active") && (bool)row["Active"];
			this.assetTrackingDeviceType	= row.IsNull("LookupAssetTrackingDeviceTypeIndex") ? AssetTrackingDeviceTypes.Standard : (AssetTrackingDeviceTypes)row["LookupAssetTrackingDeviceTypeIndex"];
			this.sourceUnit					= row.IsNull("LookupEngineeringUnitIndex") ? EngineeringUnit.FmvMeter3 : (EngineeringUnit)row["LookupEngineeringUnitIndex"];
			this.description				= row.IsNull("Description") ? string.Empty : (string)row["Description"];
			this.CreatedBy					= row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
			this.UpdatedBy					= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];

			this.createdDate = null;
			if (row.IsNull("CreatedDate") == false)
			{
				this.createdDate = (DateTimeOffset)row["CreatedDate"];
			}

			this.updatedDate = null;
			if (row.IsNull("UpdatedDate") == false)
			{
				this.updatedDate = (DateTimeOffset)row["UpdatedDate"];
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will set the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Reset();

			this.AssetTrackingDeviceGuid	= Guid.Empty;
			this.SiteGuid					= Guid.Empty;
			this.DeviceId					= string.Empty;
			this.modelNumber				= string.Empty;
			this.serialNumber				= string.Empty;
			this.active						= false;
			this.description				= string.Empty;
			this.createdDate				= null;
			this.CreatedBy					= string.Empty;
			this.updatedDate				= null;
			this.UpdatedBy					= string.Empty;
			this.equipmentId				= string.Empty;
			this.equipmentGuid				= Guid.Empty;
			this.assetTrackingDeviceType	= AssetTrackingDeviceTypes.Standard;
			this.sourceUnit					= EngineeringUnit.FmvMeter3;
		}
		#endregion
	}
}
