namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	public class AssetTrackingIconConfigurationClass : BaseDataObject
	{
		#region Private data members
		[DataMember] private Guid assetTrackingIconConfigurationGuid;
		[DataMember] private DateTimeOffset? createdDateN;
		[DataMember] private DateTimeOffset? updatedDateN;
		[DataMember] private string iconConfigurationId;
		[DataMember] private string equipmentIconName;
		[DataMember] private string equipmentVarianceIconName;
		[DataMember] private string equipmentInvestigationIconName;
		[DataMember] private string equipmentCompleteInvestigationFailedIconName;
		[DataMember] private string equipmentCompleteInvestigationPassedIconName;
		[DataMember] private string facilityIconName;
		[DataMember] private string deliveryLocationIconName;
		[DataMember] private string tankIconName;
		[DataMember] private string breadcrumbIconName;
		[DataMember] private string breadcrumbVarianceIconName;
		[DataMember] private string breadcrumbInvestigationIconName;
		[DataMember] private string breadcrumbCompleteInvestigationFailedIconName;
		[DataMember] private string breadcrumbCompleteInvestigationPassedIconName;
		[DataMember] private string mapPinIconName;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingIconConfigurationClass()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid AssetTrackingIconConfigurationGuid
		{
			get { return this.assetTrackingIconConfigurationGuid; }
			set
			{
				this.assetTrackingIconConfigurationGuid = value;
				this.IdentityGuid = this.assetTrackingIconConfigurationGuid;
			}
		}
		public DateTimeOffset? CreatedDateN
		{
			get { return this.createdDateN; }
			set { this.createdDateN = value; }
		}

		public DateTimeOffset? UpdatedDateN
		{
			get { return this.updatedDateN; }
			set { this.updatedDateN = value; }
		}

		public string IconConfigurationId
		{
			get { return this.iconConfigurationId; }
			set
			{
				this.iconConfigurationId = value;
				this.ID = value;
			}
		}

		public string EquipmentIconName
		{
			get { return this.equipmentIconName; }
			set { this.equipmentIconName = value; }
		}

		public string EquipmentVarianceIconName
		{
			get { return this.equipmentVarianceIconName; }
			set { this.equipmentVarianceIconName = value; }
		}

		public string EquipmentInvestigationIconName
		{
			get { return this.equipmentInvestigationIconName; }
			set { this.equipmentInvestigationIconName = value; }
		}

		public string EquipmentCompleteInvestigationFailedIconName
		{
			get { return this.equipmentCompleteInvestigationFailedIconName; }
			set { this.equipmentCompleteInvestigationFailedIconName = value; }
		}

		public string EquipmentCompleteInvestigationPassedIconName
		{
			get { return this.equipmentCompleteInvestigationPassedIconName; }
			set { this.equipmentCompleteInvestigationPassedIconName = value; }
		}

		public string FacilityIconName
		{
			get { return this.facilityIconName; }
			set { this.facilityIconName = value; }
		}

		public string DeliveryLocationIconName
		{
			get { return this.deliveryLocationIconName; }
			set { this.deliveryLocationIconName = value; }
		}

		public string TankIconName
		{
			get { return this.tankIconName; }
			set { this.tankIconName = value; }
		}

		public string BreadcrumbIconName
		{
			get { return this.breadcrumbIconName; }
			set { this.breadcrumbIconName = value; }
		}

		public string BreadcrumbVarianceIconName
		{
			get { return this.breadcrumbVarianceIconName; }
			set { this.breadcrumbVarianceIconName = value; }
		}

		public string BreadcrumbInvestigationIconName
		{
			get { return this.breadcrumbInvestigationIconName; }
			set { this.breadcrumbInvestigationIconName = value; }
		}

		public string BreadcrumbCompleteInvestigationFailedIconName
		{
			get { return this.breadcrumbCompleteInvestigationFailedIconName; }
			set { this.breadcrumbCompleteInvestigationFailedIconName = value; }
		}

		public string BreadcrumbCompleteInvestigationPassedIconName
		{
			get { return this.breadcrumbCompleteInvestigationPassedIconName; }
			set { this.breadcrumbCompleteInvestigationPassedIconName = value; }
		}

		public string MapPinIconName
		{
			get { return this.mapPinIconName; }
			set { this.mapPinIconName = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will populate the SQL Command with an insert command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void InsertSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "INSERT INTO tblAssetTrackingIconConfiguration ("
									 + " AssetTrackingIconConfigurationGuid,"
									 + " SiteGuid,"
									 + " IconConfigurationID,"
									 + " EquipmentIconName,"
									 + " EquipmentVarianceIconName,"
									 + " EquipmentInvestigationIconName,"
									 + " EquipmentCompleteInvestigationFailedIconName,"
									 + " EquipmentCompleteInvestigationPassedIconName,"
									 + " TankIconName,"
									 + " FacilityIconName,"
									 + " DeliveryLocationIconName,"
									 + " BreadcrumbIconName,"
									 + " BreadcrumbVarianceIconName,"
									 + " BreadcrumbInvestigationIconName,"
									 + " BreadcrumbCompleteInvestigationFailedIconName,"
									 + " BreadcrumbCompleteInvestigationPassedIconName,"
									 + " MapPinIconName,"
									 + " CreatedDate,"
									 + " CreatedBy,"
									 + " UpdatedDate,"
									 + " UpdatedBy"
									 + ") VALUES ("
									 + " @AssetTrackingIconConfigurationGuid,"
									 + " @SiteGuid,"
									 + " @IconConfigurationID,"
									 + " @EquipmentIconName,"
									 + " @EquipmentVarianceIconName,"
									 + " @EquipmentInvestigationIconName,"
									 + " @EquipmentCompleteInvestigationFailedIconName,"
									 + " @EquipmentCompleteInvestigationPassedIconName,"
									 + " @TankIconName,"
									 + " @FacilityIconName,"
									 + " @DeliveryLocationIconName,"
									 + " @BreadcrumbIconName,"
									 + " @BreadcrumbVarianceIconName,"
									 + " @BreadcrumbInvestigationIconName,"
									 + " @BreadcrumbCompleteInvestigationFailedIconName,"
									 + " @BreadcrumbCompleteInvestigationPassedIconName,"
									 + " @MapPinIconName,"
									 + " @CreatedDate,"
									 + " @CreatedBy,"
									 + " @UpdatedDate,"
									 + " @UpdatedBy"
									 + " ) ";

			this.AssetTrackingIconConfigurationGuid = Guid.NewGuid();
			this.createdDateN = DateTimeOffset.Now;
			this.updatedDateN = this.createdDateN;
			this.CreatedBy = security.UserID;
			this.UpdatedBy = security.UserID;
			this.SiteGuid = security.SiteGuid;

			var parm = new SqlParameter("@AssetTrackingIconConfigurationGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingIconConfigurationGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.SiteGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@IconConfigurationID", SqlDbType.NVarChar, 20) { Value = this.IconConfigurationId };
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.equipmentIconName))
			{
				parm = new SqlParameter("@EquipmentIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentIconName", SqlDbType.NVarChar, (50)) { Value = this.equipmentIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.equipmentVarianceIconName))
			{
				parm = new SqlParameter("@EquipmentVarianceIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentVarianceIconName", SqlDbType.NVarChar, (50)) { Value = this.EquipmentVarianceIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.EquipmentInvestigationIconName))
			{
				parm = new SqlParameter("@EquipmentInvestigationIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentInvestigationIconName", SqlDbType.NVarChar, (50)) { Value = this.EquipmentInvestigationIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.EquipmentCompleteInvestigationFailedIconName))
			{
				parm = new SqlParameter("@EquipmentCompleteInvestigationFailedIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentCompleteInvestigationFailedIconName", SqlDbType.NVarChar, (50)) { Value = this.EquipmentCompleteInvestigationFailedIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.EquipmentCompleteInvestigationPassedIconName))
			{
				parm = new SqlParameter("@EquipmentCompleteInvestigationPassedIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentCompleteInvestigationPassedIconName", SqlDbType.NVarChar, (50)) { Value = this.EquipmentCompleteInvestigationPassedIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.tankIconName))
			{
				parm = new SqlParameter("@TankIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@TankIconName", SqlDbType.NVarChar, (50)) { Value = this.tankIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.facilityIconName))
			{
				parm = new SqlParameter("@FacilityIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@FacilityIconName", SqlDbType.NVarChar, (50)) { Value = this.facilityIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.deliveryLocationIconName))
			{
				parm = new SqlParameter("@DeliveryLocationIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@DeliveryLocationIconName", SqlDbType.NVarChar, (50)) { Value = this.deliveryLocationIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.breadcrumbIconName))
			{
				parm = new SqlParameter("@BreadcrumbIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbIconName", SqlDbType.NVarChar, (50)) { Value = this.breadcrumbIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.breadcrumbVarianceIconName))
			{
				parm = new SqlParameter("@BreadcrumbVarianceIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbVarianceIconName", SqlDbType.NVarChar, (50)) { Value = this.breadcrumbVarianceIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.breadcrumbInvestigationIconName))
			{
				parm = new SqlParameter("@BreadcrumbInvestigationIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbInvestigationIconName", SqlDbType.NVarChar, (50)) { Value = this.breadcrumbInvestigationIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.breadcrumbCompleteInvestigationFailedIconName))
			{
				parm = new SqlParameter("@BreadcrumbCompleteInvestigationFailedIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbCompleteInvestigationFailedIconName", SqlDbType.NVarChar, (50)) { Value = this.breadcrumbCompleteInvestigationFailedIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.breadcrumbCompleteInvestigationPassedIconName))
			{
				parm = new SqlParameter("@BreadcrumbCompleteInvestigationPassedIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbCompleteInvestigationPassedIconName", SqlDbType.NVarChar, (50)) { Value = this.breadcrumbCompleteInvestigationPassedIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.mapPinIconName))
			{
				parm = new SqlParameter("@MapPinIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@MapPinIconName", SqlDbType.NVarChar, (50)) { Value = this.mapPinIconName };
			}
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.createdDateN };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 50) { Value = this.CreatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDateN };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 50) { Value = this.UpdatedBy };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with an update command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void UpdateSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "UPDATE tblAssetTrackingIconConfiguration SET"
									 + " IconConfigurationID = @IconConfigurationID,"
									 + " EquipmentIconName = @EquipmentIconName,"
									 + " EquipmentVarianceIconName = @EquipmentVarianceIconName,"
									 + " EquipmentInvestigationIconName = @EquipmentInvestigationIconName,"
									 + " EquipmentCompleteInvestigationFailedIconName = @EquipmentCompleteInvestigationFailedIconName,"
									 + " EquipmentCompleteInvestigationPassedIconName = @EquipmentCompleteInvestigationPassedIconName,"
									 + " TankIconName = @TankIconName,"
									 + " FacilityIconName = @FacilityIconName,"
									 + " DeliveryLocationIconName = @DeliveryLocationIconName,"
									 + " BreadcrumbIconName = @BreadcrumbIconName,"
									 + " BreadcrumbVarianceIconName = @BreadcrumbVarianceIconName,"
									 + " BreadcrumbInvestigationIconName = @BreadcrumbInvestigationIconName,"
									 + " BreadcrumbCompleteInvestigationFailedIconName = @BreadcrumbCompleteInvestigationFailedIconName,"
									 + " BreadcrumbCompleteInvestigationPassedIconName = @BreadcrumbCompleteInvestigationPassedIconName,"
									 + " MapPinIconName = @MapPinIconName,"
									 + " UpdatedBy = @UpdatedBy,"
									 + " UpdatedDate = @UpdatedDate"
									 + " WHERE AssetTrackingIconConfigurationGuid = @AssetTrackingIconConfigurationGuid";

			this.updatedDateN = DateTimeOffset.Now;
			this.UpdatedBy = security.UserID;
			this.SiteGuid = security.SiteGuid;

			var parm = new SqlParameter("@AssetTrackingIconConfigurationGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingIconConfigurationGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@IconConfigurationID", SqlDbType.NVarChar, 20) { Value = this.IconConfigurationId };
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.equipmentIconName))
			{
				parm = new SqlParameter("@EquipmentIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentIconName", SqlDbType.NVarChar, (50)) { Value = this.equipmentIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.EquipmentVarianceIconName))
			{
				parm = new SqlParameter("@EquipmentVarianceIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentVarianceIconName", SqlDbType.NVarChar, (50)) { Value = this.EquipmentVarianceIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.equipmentInvestigationIconName))
			{
				parm = new SqlParameter("@EquipmentInvestigationIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentInvestigationIconName", SqlDbType.NVarChar, (50)) { Value = this.equipmentInvestigationIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.EquipmentCompleteInvestigationFailedIconName))
			{
				parm = new SqlParameter("@EquipmentCompleteInvestigationFailedIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentCompleteInvestigationFailedIconName", SqlDbType.NVarChar, (50)) { Value = this.EquipmentCompleteInvestigationFailedIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.EquipmentCompleteInvestigationPassedIconName))
			{
				parm = new SqlParameter("@EquipmentCompleteInvestigationPassedIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@EquipmentCompleteInvestigationPassedIconName", SqlDbType.NVarChar, (50)) { Value = this.EquipmentCompleteInvestigationPassedIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.tankIconName))
			{
				parm = new SqlParameter("@TankIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@TankIconName", SqlDbType.NVarChar, (50)) { Value = this.tankIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.facilityIconName))
			{
				parm = new SqlParameter("@FacilityIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@FacilityIconName", SqlDbType.NVarChar, (50)) { Value = this.facilityIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.deliveryLocationIconName))
			{
				parm = new SqlParameter("@DeliveryLocationIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@DeliveryLocationIconName", SqlDbType.NVarChar, (50)) { Value = this.deliveryLocationIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.breadcrumbIconName))
			{
				parm = new SqlParameter("@BreadcrumbIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbIconName", SqlDbType.NVarChar, (50)) { Value = this.breadcrumbIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.breadcrumbVarianceIconName))
			{
				parm = new SqlParameter("@BreadcrumbVarianceIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbVarianceIconName", SqlDbType.NVarChar, (50)) { Value = this.breadcrumbVarianceIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.breadcrumbInvestigationIconName))
			{
				parm = new SqlParameter("@BreadcrumbInvestigationIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbInvestigationIconName", SqlDbType.NVarChar, (50)) { Value = this.breadcrumbInvestigationIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.BreadcrumbCompleteInvestigationFailedIconName))
			{
				parm = new SqlParameter("@BreadcrumbCompleteInvestigationFailedIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbCompleteInvestigationFailedIconName", SqlDbType.NVarChar, (50)) { Value = this.BreadcrumbCompleteInvestigationFailedIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.BreadcrumbCompleteInvestigationPassedIconName))
			{
				parm = new SqlParameter("@BreadcrumbCompleteInvestigationPassedIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@BreadcrumbCompleteInvestigationPassedIconName", SqlDbType.NVarChar, (50)) { Value = this.BreadcrumbCompleteInvestigationPassedIconName };
			}
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.mapPinIconName))
			{
				parm = new SqlParameter("@MapPinIconName", SqlDbType.NVarChar, (50)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@MapPinIconName", SqlDbType.NVarChar, (50)) { Value = this.mapPinIconName };
			}
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = this.UpdatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDateN };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with a delete command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingIconConfigurationGuid">The GUID to the record to delete.</param>
		public void DeleteSql(SqlCommand sqlCommand, Guid inAssetTrackingIconConfigurationGuid)
		{
			sqlCommand.CommandText = "DELETE FROM tblAssetTrackingIconConfiguration"
									 + " WHERE AssetTrackingIconConfigurationGuid = @AssetTrackingIconConfigurationGuid";

			var parm = new SqlParameter("@AssetTrackingIconConfigurationGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingIconConfigurationGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with an ennumerate command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The Security object.</param>
		public void EnumerateSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingIconConfiguration ";
		}

		/// <summary>
		/// This method will populate the SQL Command with a get command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingIconConfigurationGuid">The GUID to the record to get.</param>
		public void GetSql(SqlCommand sqlCommand, Guid inAssetTrackingIconConfigurationGuid)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingIconConfiguration " + SQLUpdateLock(false)
									 + " WHERE AssetTrackingIconConfigurationGuid = @AssetTrackingIconConfigurationGuid";

			var parm = new SqlParameter("@AssetTrackingIconConfigurationGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingIconConfigurationGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get identity GUID by map name SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inIconConfigurationId">The Icon Configuration to get the GUID.</param>
		/// <param name="security">The security object.</param>
		public void GetIdentityGuidSql(SqlCommand sqlCommand, string inIconConfigurationId, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT AssetTrackingIconConfigurationGuid FROM tblAssetTrackingIconConfiguration "
									+ " WHERE IconConfigurationID = @IconConfigurationID ";

			var parm = new SqlParameter("@IconConfigurationID", SqlDbType.NVarChar, 20) { Value = inIconConfigurationId };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will load the object with the record from the database.
		/// </summary>
		/// <param name="row">The row to load.</param>
		public void Load(DataRow row)
		{
			if (row == null)
			{
				return;
			}

			this.AssetTrackingIconConfigurationGuid = row.IsNull("AssetTrackingIconConfigurationGuid") ? Guid.Empty : (Guid)row["AssetTrackingIconConfigurationGuid"];
			this.SiteGuid							= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
			this.IconConfigurationId				= row.IsNull("IconConfigurationID") ? string.Empty : (string)row["IconConfigurationID"];
			this.CreatedBy							= row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
			this.UpdatedBy							= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];
			this.equipmentIconName					= row.IsNull("EquipmentIconName") ? string.Empty : (string)row["EquipmentIconName"];
			this.equipmentVarianceIconName			= row.IsNull("EquipmentVarianceIconName") ? string.Empty : (string) row["EquipmentVarianceIconName"];
			this.equipmentInvestigationIconName		= row.IsNull("EquipmentInvestigationIconName") ? string.Empty : (string) row["EquipmentInvestigationIconName"];
			this.tankIconName						= row.IsNull("TankIconName") ? string.Empty : (string)row["TankIconName"];
			this.facilityIconName					= row.IsNull("FacilityIconName") ? string.Empty : (string)row["FacilityIconName"];
			this.deliveryLocationIconName			= row.IsNull("DeliveryLocationIconName") ? string.Empty : (string)row["DeliveryLocationIconName"];
			this.breadcrumbIconName					= row.IsNull("BreadcrumbIconName") ? string.Empty : (string)row["BreadcrumbIconName"];
			this.breadcrumbVarianceIconName			= row.IsNull("BreadcrumbVarianceIconName") ? string.Empty : (string) row["BreadcrumbVarianceIconName"];
			this.breadcrumbInvestigationIconName	= row.IsNull("BreadcrumbInvestigationIconName") ? string.Empty : (string) row["BreadcrumbInvestigationIconName"];
			this.mapPinIconName						= row.IsNull("MapPinIconName") ? string.Empty : (string)row["MapPinIconName"];

			this.equipmentCompleteInvestigationFailedIconName	= row.IsNull("EquipmentCompleteInvestigationFailedIconName") ? string.Empty : (string) row["EquipmentCompleteInvestigationFailedIconName"];
			this.equipmentCompleteInvestigationPassedIconName	= row.IsNull("EquipmentCompleteInvestigationPassedIconName") ? string.Empty : (string) row["EquipmentCompleteInvestigationPassedIconName"];
			this.breadcrumbCompleteInvestigationFailedIconName	= row.IsNull("BreadcrumbCompleteInvestigationFailedIconName") ? string.Empty : (string) row["BreadcrumbCompleteInvestigationFailedIconName"];
			this.breadcrumbCompleteInvestigationPassedIconName	= row.IsNull("BreadcrumbCompleteInvestigationPassedIconName") ? string.Empty : (string) row["BreadcrumbCompleteInvestigationPassedIconName"];

			this.createdDateN = null;
			if (row.IsNull("CreatedDate") == false)
			{
				this.CreatedDateN = (DateTimeOffset)row["CreatedDate"];
			}

			this.updatedDateN = null;
			if (row.IsNull("UpdatedDate") == false)
			{
				this.CreatedDateN = (DateTimeOffset)row["UpdatedDate"];
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.assetTrackingIconConfigurationGuid = Guid.Empty;
			this.createdDateN						= null;
			this.updatedDateN						= null;
			this.equipmentIconName					= string.Empty;
			this.equipmentVarianceIconName			= string.Empty;
			this.equipmentInvestigationIconName		= string.Empty;
			this.facilityIconName					= string.Empty;
			this.deliveryLocationIconName			= string.Empty;
			this.tankIconName						= string.Empty;
			this.breadcrumbIconName					= string.Empty;
			this.breadcrumbVarianceIconName			= string.Empty;
			this.breadcrumbInvestigationIconName	= string.Empty;
			this.mapPinIconName						= string.Empty;
			this.iconConfigurationId				= string.Empty;

			this.EquipmentCompleteInvestigationFailedIconName = string.Empty;
			this.EquipmentCompleteInvestigationPassedIconName = string.Empty;
			this.breadcrumbCompleteInvestigationFailedIconName = string.Empty;
			this.breadcrumbCompleteInvestigationPassedIconName = string.Empty;
		}
		#endregion
	}
}
