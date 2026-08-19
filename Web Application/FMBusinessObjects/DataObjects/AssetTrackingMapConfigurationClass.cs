namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class AssetTrackingMapConfigurationClass : BaseDataObject
	{
		#region Public
		/// <summary>
		/// Indicates which map server to use.
		/// </summary>
		public enum MapSources { OpenStreetMap, MapServer, GoogleMap, BingMap };
		#endregion

		#region Private data members
		[DataMember] private Guid assetTrackingMapConfigurationGuid;
		[DataMember] private string mapName;
		[DataMember] private int zoom;
		[DataMember] private double latitude;
		[DataMember] private double longitude;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		[DataMember] private MapSources mapSource;
		[DataMember] private string description;
		[DataMember] private bool active;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingMapConfigurationClass()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid AssetTrackingMapConfigurationGuid
		{
			get { return this.assetTrackingMapConfigurationGuid; }
			set
			{
				this.assetTrackingMapConfigurationGuid = value;
				this.IdentityGuid = this.assetTrackingMapConfigurationGuid;
			}
		}

		public string MapName
		{
			get { return this.mapName; }
			set
			{
				this.mapName = value;
				this.ID = this.mapName;
			}
		}

		public int Zoom
		{
			get { return this.zoom; }
			set { this.zoom = value; }
		}

		public double Latitude
		{
			get { return this.latitude; }
			set { this.latitude = value; }
		}

		public double Longitude
		{
			get { return this.longitude; }
			set { this.longitude = value; }
		}

		public MapSources MapSource
		{
			get { return this.mapSource; }
			set { this.mapSource = value; }
		}

		public string Description
		{
			get { return this.description; }
			set { this.description = value; }
		}

		public bool Active
		{
			get { return this.active; }
			set { this.active = value; }
		}

		public override ENTITY_TYPE EntityType => ENTITY_TYPE.ASSET_TRACKING_MAP_CONFIGURATION;

		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

		#endregion

		#region Public methods
		/// <summary>
		/// This method will populate the SQL Command with an insert command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void InsertSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "INSERT INTO tblAssetTrackingMapConfiguration ("
									 + " AssetTrackingMapConfigurationGuid,"
									 + " SiteGuid,"
									 + " MapName,"
									 + " Zoom,"
									 + " Latitude,"
									 + " Longitude,"
									 + " LookupMapSourceIndex,"
									 + " Description,"
									 + " Active,"
									 + " CreatedDate,"
									 + " CreatedBy,"
									 + " UpdatedDate,"
									 + " UpdatedBy"
									 + ") VALUES ("
									 + " @AssetTrackingMapConfigurationGuid,"
									 + " @SiteGuid,"
									 + " @MapName,"
									 + " @Zoom,"
									 + " @Latitude,"
									 + " @Longitude,"
									 + " @LookupMapSourceIndex,"
									 + " @Description,"
									 + " @Active,"
									 + " @CreatedDate,"
									 + " @CreatedBy,"
									 + " @UpdatedDate,"
									 + " @UpdatedBy"
									 + " ) ";

			this.AssetTrackingMapConfigurationGuid = Guid.NewGuid();
			this.createdDate						= DateTimeOffset.Now;
			this.updatedDate						= this.createdDate;
			this.CreatedBy							= security.UserID;
			this.UpdatedBy							= security.UserID;
			this.SiteGuid							= security.SiteGuid;

			var parm = new SqlParameter("@AssetTrackingMapConfigurationGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingMapConfigurationGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.SiteGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MapName", SqlDbType.NVarChar, 20) { Value = this.MapName };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Zoom", SqlDbType.Int) { Value = this.zoom };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Latitude", SqlDbType.Float) { Value = this.latitude };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Longitude", SqlDbType.Float) { Value = this.longitude };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupMapSourceIndex", SqlDbType.Int) { Value = this.mapSource };
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.description))
			{
				parm = new SqlParameter("@Description", SqlDbType.NVarChar, (200)) { Value = DBNull.Value };
			}
			else
			{
				parm = new SqlParameter("@Description", SqlDbType.NVarChar, (200)) { Value = this.description };
			}
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Active", SqlDbType.Bit) { Value = this.active ? 1 : 0 };
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
		/// This method will populate the SQL Command with an update command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		public void UpdateSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "UPDATE tblAssetTrackingMapConfiguration SET" 
									 + " MapName = @MapName," 
									 + " Zoom = @Zoom,"
			                         + " Latitude = @Latitude," 
									 + " Longitude = @Longitude,"
									 + " LookupMapSourceIndex = @LookupMapSourceIndex,"
 									 + " Description = @Description,"
									 + " Active = @Active,"
									 + " UpdatedBy = @UpdatedBy,"
			                         + " UpdatedDate = @UpdatedDate,"
									 + " SiteGuid = @SiteGuid"
			                         + " WHERE AssetTrackingMapConfigurationGuid = @AssetTrackingMapConfigurationGuid";

			this.updatedDate = DateTimeOffset.Now;
			this.UpdatedBy = security.UserID;
			this.SiteGuid = security.SiteGuid;

			var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.SiteGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetTrackingMapConfigurationGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingMapConfigurationGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MapName", SqlDbType.NVarChar, 20) { Value = this.MapName };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Zoom", SqlDbType.Int) { Value = this.zoom };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Latitude", SqlDbType.Float) { Value = this.latitude };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Longitude", SqlDbType.Float) { Value = this.longitude };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupMapSourceIndex", SqlDbType.Int) { Value = this.mapSource };
			sqlCommand.Parameters.Add(parm);

			if (string.IsNullOrEmpty(this.description))
			{
				parm = new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = DBNull.Value  };
			}
			else
			{
				parm = new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = this.description };
			}
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Active", SqlDbType.Bit) { Value = this.active ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = this.UpdatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with a delete command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingMapConfigurationGuid">The GUID to the record to delete.</param>
		public void DeleteSql(SqlCommand sqlCommand, Guid inAssetTrackingMapConfigurationGuid)
		{
			sqlCommand.CommandText = "DELETE FROM tblAssetTrackingMapConfiguration"
			                         + " WHERE AssetTrackingMapConfigurationGuid = @AssetTrackingMapConfigurationGuid";

			var parm = new SqlParameter("@AssetTrackingMapConfigurationGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingMapConfigurationGuid };
			sqlCommand.Parameters.Add(parm);

		}

		/// <summary>
		/// This method will populate the SQL Command with an ennumerate command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The Security object.</param>
		public void EnumerateSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingMapConfiguration " + SQLUpdateLock(false) + " WHERE " 
									+ this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingMapConfiguration", "AssetTrackingMapConfigurationGuid");
		}

		/// <summary>
		/// This method will populate the SQL Command with an ennumerate by filter command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The Security object.</param>
		/// <param name="filter">The filter value to add to the criterion.</param>
		public void EnumerateByFilterSql(SqlCommand sqlCommand, SecurityClass security, string filter)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingMapConfiguration " + SQLUpdateLock(false)
									+ " WHERE MapName LIKE @MapNameFilter AND "
									+ this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingMapConfiguration", "AssetTrackingMapConfigurationGuid");

			var parm = new SqlParameter("@MapNameFilter", SqlDbType.NVarChar, 20) { Value = "%" + filter + "%" };
			sqlCommand.Parameters.Add(parm);
		}


		/// <summary>
		/// This method will populate the get identity GUID by map name SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inMapName">The map name to get the GUID.</param>
		/// <param name="security">The security object.</param>
		public void GetIdentityGuidSql(SqlCommand sqlCommand, string inMapName, SecurityClass security)
		{
			sqlCommand.CommandText = "SELECT AssetTrackingMapConfigurationGuid FROM tblAssetTrackingMapConfiguration " + SQLUpdateLock(false)
									+ " WHERE MapName = @MapName AND "
									+ this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingMapConfiguration", "AssetTrackingMapConfigurationGuid");

			var parm = new SqlParameter("@MapName", SqlDbType.NVarChar, 20) { Value = inMapName };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with a get command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingMapConfigurationGuid">The GUID to the record to get.</param>
		public void GetSql(SqlCommand sqlCommand, Guid inAssetTrackingMapConfigurationGuid)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingMapConfiguration " + SQLUpdateLock(false)
									 + " WHERE AssetTrackingMapConfigurationGuid = @AssetTrackingMapConfigurationGuid";

			var parm = new SqlParameter("@AssetTrackingMapConfigurationGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingMapConfigurationGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with a get by map name command.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="security">The security object</param>
		/// <param name="inMapName">The map name to the record to get.</param>
		public void GetByMapNameSql(SqlCommand sqlCommand, SecurityClass security, string inMapName)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingMapConfiguration " + SQLUpdateLock(false)
									 + " WHERE MapName = @MapName AND "
									 + this.AppendSiteWhereClause(sqlCommand, security, "tblAssetTrackingMapConfiguration", "AssetTrackingMapConfigurationGuid");

			var parm = new SqlParameter("@MapName", SqlDbType.NVarChar, 20) { Value = inMapName };
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

			this.AssetTrackingMapConfigurationGuid	= row.IsNull("AssetTrackingMapConfigurationGuid") ? Guid.Empty : (Guid)row["AssetTrackingMapConfigurationGuid"];
			this.SiteGuid							= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
			this.MapName							= row.IsNull("MapName") ? string.Empty : (string)row["MapName"];
			this.CreatedBy							= row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
			this.UpdatedBy							= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];
			this.zoom								= row.IsNull("Zoom") ? 7 : (int)row["Zoom"];
			this.latitude							= row.IsNull("Latitude") ? 0 : (double)row["Latitude"];
			this.longitude							= row.IsNull("Longitude") ? 0 : (double)row["Longitude"];
			this.mapSource							= row.IsNull("LookupMapSourceIndex") ? MapSources.OpenStreetMap : (MapSources)row["LookupMapSourceIndex"];
			this.description						= row.IsNull("Description") ? string.Empty : (string)row["Description"];
			this.active								= row.IsNull("Active") ? false : (bool)row["Active"];

			this.createdDate = null;
			if (row.IsNull("CreatedDate") == false)
			{
				this.CreatedDate = (DateTimeOffset)row["CreatedDate"];
			}

			this.updatedDate = null;
			if (row.IsNull("UpdatedDate") == false)
			{
				this.CreatedDate = (DateTimeOffset)row["UpdatedDate"];
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Reset();

			this.AssetTrackingMapConfigurationGuid  = Guid.Empty;
			this.SiteGuid							= Guid.Empty;
			this.MapName							= string.Empty;
			this.zoom								= 7;
			this.latitude							= 0;
			this.longitude							= 0;
			this.CreatedBy							= string.Empty;
			this.createdDate						= null;
			this.UpdatedBy							= string.Empty;
			this.updatedDate						= null;
			this.mapSource							= MapSources.OpenStreetMap;
			this.active								= false;
			this.description						= string.Empty;
		}
		#endregion
	}
}
