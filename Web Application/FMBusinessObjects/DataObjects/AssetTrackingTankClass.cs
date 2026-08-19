namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class AssetTrackingTankClass
	{
		#region Private data members
		[DataMember] private Guid assetTrackingTankGuid;
		[DataMember] private Guid assetTrackingDetailGuid;
		[DataMember] private string tankId;
		[DataMember] private string productId;
		[DataMember] private double? volume;
		[DataMember] private double? temperature;
		[DataMember] private double? density;
		[DataMember] private double? dielectric;
		[DataMember] private bool contaminated;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingTankClass()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid AssetTrackingTankGuid
		{
			get { return this.assetTrackingTankGuid; }
			set { this.assetTrackingTankGuid = value; }
		}
		public Guid AssetTrackingDetailGuid
		{
			get { return this.assetTrackingDetailGuid; }
			set { this.assetTrackingDetailGuid = value; }
		}
		public string TankId
		{
			get { return this.tankId; }
			set { this.tankId = value; }
		}
		public string ProductId
		{
			get { return this.productId; }
			set { this.productId = value; }
		}
		public double? Volume
		{
			get { return this.volume; }
			set { this.volume = value; }
		}
		public double? Temperature
		{
			get { return this.temperature; }
			set { this.temperature = value; }
		}
		public double? Density
		{
			get { return this.density; }
			set { this.density = value; }
		}
		public double? Dielectric
		{
			get { return this.dielectric; }
			set { this.dielectric = value; }
		}
		public bool Contaminated
		{
			get { return this.contaminated; }
			set { this.contaminated = value; }
		}
		public DateTimeOffset? CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		public DateTimeOffset? UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will populate the insert SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="security">Security object.</param>
		public void InsertSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "INSERT INTO tblAssetTrackingTank ("
									 + " AssetTrackingTankGuid,"
									 + " AssetTrackingDetailGuid,"
									 + " TankID,"
									 + " ProductID,"
									 + " Volume,"
									 + " Temperature,"
									 + " Density,"
									 + " Dielectric,"
									 + " Contaminated,"
									 + " CreatedDate,"
									 + " CreatedBy,"
									 + " UpdatedDate,"
									 + " UpdatedBy"
									 + " ) VALUES ("
									 + " @AssetTrackingTankGuid,"
									 + " @AssetTrackingDetailGuid,"
									 + " @TankID,"
									 + " @ProductID,"
									 + " @Volume,"
									 + " @Temperature,"
									 + " @Density,"
									 + " @Dielectric,"
									 + " @Contaminated,"
									 + " @CreatedDate,"
									 + " @CreatedBy,"
									 + " @UpdatedDate,"
									 + " @UpdatedBy"
									 + ") ";

			this.AssetTrackingTankGuid	= Guid.NewGuid();
			this.createdDate			= DateTimeOffset.Now;
			this.CreatedBy				= security.UserID;
			this.updatedDate			= this.createdDate;
			this.updatedBy				= this.createdBy;

			if (sqlCommand.Parameters.Contains("@AssetTrackingTankGuid"))
			{
				sqlCommand.Parameters["@AssetTrackingTankGuid"].Value	= this.AssetTrackingTankGuid;
				sqlCommand.Parameters["@AssetTrackingDetailGuid"].Value = this.assetTrackingDetailGuid;
				sqlCommand.Parameters["@TankID"].Value					= this.tankId;
				sqlCommand.Parameters["@ProductID"].Value				= this.productId;
				sqlCommand.Parameters["@Volume"].Value					= this.volume ?? (object)DBNull.Value;
				sqlCommand.Parameters["@Temperature"].Value				= this.temperature ?? (object)DBNull.Value;
				sqlCommand.Parameters["@Density"].Value					= this.density ?? (object)DBNull.Value;
				sqlCommand.Parameters["@Dielectric"].Value				= this.dielectric ?? (object)DBNull.Value;
				sqlCommand.Parameters["@Contaminated"].Value			= this.Contaminated ? 1 : 0;
				sqlCommand.Parameters["@CreatedDate"].Value				= this.createdDate;
				sqlCommand.Parameters["@CreatedBy"].Value				= this.CreatedBy;
				sqlCommand.Parameters["@UpdatedDate"].Value				= this.updatedDate;
				sqlCommand.Parameters["@UpdatedBy"].Value				= this.updatedBy;

				return;
			}

			var parm = new SqlParameter("@AssetTrackingTankGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingTankGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = this.assetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@TankID", SqlDbType.NVarChar, 30) { Value = this.tankId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ProductID", SqlDbType.NVarChar, 30) { Value = this.productId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Volume", SqlDbType.Float) { Value = this.volume ?? (object)DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Temperature", SqlDbType.Float) { Value = this.temperature ?? (object)DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Density", SqlDbType.Float) { Value = this.density ?? (object)DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Dielectric", SqlDbType.Float) { Value = this.dielectric ?? (object)DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Contaminated", SqlDbType.Bit) { Value = this.Contaminated ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.createdDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 50) { Value = this.CreatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 50) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the update SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="security">Security object.</param>
		public void UpdateSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "UPDATE tblAssetTrackingTank SET "
									 + " TankID = @TankID,"
									 + " ProductID = @ProductID,"
									 + " Volume = @Volume,"
									 + " Temperature = @Temperature,"
									 + " Density = @Density,"
									 + " Dielectric = @Dielectric,"
									 + " Contaminated = @Contaminated,"
									 + " UpdatedDate = @UpdatedDate,"
									 + " UpdatedBy = @UpdatedBy"
									 + " WHERE AssetTrackingTankGuid = @AssetTrackingTankGuid";

			var parm = new SqlParameter("@AssetTrackingTankGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingTankGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@TankID", SqlDbType.NVarChar, 30) { Value = this.tankId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ProductID", SqlDbType.NVarChar, 30) { Value = this.productId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Volume", SqlDbType.Float) { Value = this.volume ?? (object)DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Temperature", SqlDbType.Float) { Value = this.temperature ?? (object)DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Density", SqlDbType.Float) { Value = this.density ?? (object)DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Dielectric", SqlDbType.Float) { Value = this.dielectric ?? (object)DBNull.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Contaminated", SqlDbType.Bit) { Value = this.Contaminated ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			this.updatedDate = DateTimeOffset.Now;
			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			this.updatedBy = security.UserID;
			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 50) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the Delete by asset tracking detail SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingDetailGuid">The tank to deleted based on this GUID.</param>
		public void DeleteByAssetTrackingDetail(SqlCommand sqlCommand, Guid inAssetTrackingDetailGuid)
		{
			sqlCommand.CommandText = "DELETE FROM tblAssetTrackingTank "
									 + " WHERE AssetTrackingDetailGuid = @AssetTrackingDetailGuid";

			var parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the Delete by asset tracking tank SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingTankGuid">The tank to deleted based on this GUID.</param>
		public void DeleteByAssetTrackingTank(SqlCommand sqlCommand, Guid inAssetTrackingTankGuid)
		{
			sqlCommand.CommandText = "DELETE FROM tblAssetTrackingTank "
									 + " WHERE AssetTrackingTankGuid = @AssetTrackingTankGuid";

			var parm = new SqlParameter("@AssetTrackingTankGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingTankGuid };
			sqlCommand.Parameters.Add(parm);

		}

		/// <summary>
		/// This method will populate the Get SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="inAssetTrackingTankGuid">The asset tracking tank to retrieve.</param>
		public void GetSql(SqlCommand sqlCommand, Guid inAssetTrackingTankGuid)
		{
			sqlCommand.CommandText =
				"SELECT * FROM tblAssetTrackingTank WHERE AssetTrackingTankGuid = @AssetTrackingTankGuid ";

			var parm = new SqlParameter("@AssetTrackingTankGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingTankGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the Get all tanks SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="detailList">The asset tracking tanks to retrieve.</param>
		public void GetAssociatedWrdcuTanksSql(SqlCommand sqlCommand, List<AssetTrackingDetailClass> detailList)
		{
			bool firstRecord = true;
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingTank "
									+ "WHERE AssetTrackingDetailGuid IN (SELECT * FROM udf_SplitString(@AssetTrackingDetailGuidList, ',', 0)) ";
			string parmValue = string.Empty;

			foreach (var detail in detailList)
			{
				if (firstRecord)
				{
					parmValue = parmValue + detail.AssetTrackingDetailGuid;
					firstRecord = false;
				}
				else
				{
					parmValue = parmValue + "," + detail.AssetTrackingDetailGuid;
				}
			}

			var parm = new SqlParameter("@AssetTrackingDetailGuidList", SqlDbType.NVarChar, 200000) { Value = parmValue };
			sqlCommand.Parameters.Add(parm);

			sqlCommand.CommandText = sqlCommand.CommandText + " ORDER BY AssetTrackingDetailGuid,  TankID ";
		}

		/// <summary>
		/// This method will populate the Get all tanks SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="inAssetTrackingDetailGuid">The asset tracking tanks to retrieve.</param>
		public void GetAllTanksByAssetTrackingDetailSql(SqlCommand sqlCommand, Guid inAssetTrackingDetailGuid)
		{
			sqlCommand.CommandText =
				"SELECT * FROM tblAssetTrackingTank WHERE AssetTrackingDetailGuid = @AssetTrackingDetailGuid "
				+ " ORDER BY TankID ";

			var parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get previous asset tracking detail tanks based on the 
		/// asset tracking device ID and getting the most resent tanks based on the asset session
		/// date time.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="inAssetTrackingDeviceId">The asset tracking device ID related to the tanks.</param>
		public void GetPreviousDetailTanksSql(SqlCommand sqlCommand, string inAssetTrackingDeviceId)
		{
			sqlCommand.CommandText = "SELECT tblAssetTrackingTank.* "
			                         + " FROM tblAssetTrackingTank LEFT OUTER JOIN tblAssetTrackingDetail ON "
			                         + " tblAssetTrackingTank.AssetTrackingDetailGuid = tblAssetTrackingDetail.AssetTrackingDetailGuid"
			                         + " WHERE tblAssetTrackingTank.AssetTrackingDetailGuid = "
									 + " (SELECT TOP(1) AssetTrackingDetailGuid FROM tblAssetTrackingDetail WHERE AssetTrackingDeviceID = @AssetTrackindDeviceID"
									 + " ORDER BY AssetSessionDateTime DESC) ";

			var parm = new SqlParameter("@AssetTrackindDeviceID", SqlDbType.NVarChar, 30) { Value = inAssetTrackingDeviceId };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will load the object from a database row.
		/// </summary>
		/// <param name="row">Row from the database.</param>
		public void Load(DataRow row)
		{
			if (row == null)
			{
				return;
			}

			this.AssetTrackingTankGuid		= row.IsNull("AssetTrackingTankGuid") ? Guid.Empty : (Guid)row["AssetTrackingTankGuid"];
			this.assetTrackingDetailGuid	= row.IsNull("AssetTrackingDetailGuid") ? Guid.Empty : (Guid)row["AssetTrackingDetailGuid"];
			this.tankId						= row.IsNull("TankID") ? string.Empty : (string)row["TankID"];
			this.productId					= row.IsNull("ProductID") ? string.Empty : (string)row["ProductID"];
			this.volume						= row.IsNull("Volume") ? (double?)null : (double)row["Volume"];
			this.temperature				= row.IsNull("Temperature") ? (double?)null : (double)row["Temperature"];
			this.density					= row.IsNull("Density") ? (double?)null : (double)row["Density"];
			this.dielectric					= row.IsNull("Dielectric") ? (double?)null : (double)row["Dielectric"];
			this.Contaminated				= row.IsNull("Contaminated") ? false : (bool)row["Contaminated"];
			this.createdBy					= row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
			this.updatedBy					= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];

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
		/// This method will initial the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.assetTrackingTankGuid		= Guid.Empty;
			this.assetTrackingDetailGuid	= Guid.Empty;
			this.tankId						= string.Empty;
			this.productId					= string.Empty;
			this.volume						= null;
			this.temperature				= null;
			this.density					= null;
			this.dielectric					= null;
			this.Contaminated				= false;
			this.createdDate				= null;
			this.updatedDate				= null;
			this.createdBy					= string.Empty;
			this.updatedBy					= string.Empty;
		}
		#endregion
	}
}
