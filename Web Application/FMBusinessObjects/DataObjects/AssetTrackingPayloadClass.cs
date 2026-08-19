namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class AssetTrackingPayloadClass
	{
		#region Private data members
		[DataMember] private Guid assetTrackingPayloadGuid;
		[DataMember] private Guid assetTrackingDetailGuid;
		[DataMember] private int byteNumber;
		[DataMember] private int payloadValue;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingPayloadClass()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid AssetTrackingPayloadGuid
		{
			get { return this.assetTrackingPayloadGuid; }
			set { this.assetTrackingPayloadGuid = value; }
		}

		public Guid AssetTrackingDetailGuid
		{ 
			get { return this.assetTrackingDetailGuid; }
			set { this.assetTrackingDetailGuid = value; }
		}

		public int ByteNumber
		{
			get { return this.byteNumber; }
			set { this.byteNumber = value; }
		}

		public int PayloadValue
		{
			get { return this.payloadValue; }
			set { this.payloadValue = value; }
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
			sqlCommand.CommandText = "INSERT INTO tblAssetTrackingPayload (" 
									 + " AssetTrackingPayloadGuid,"
			                         + " AssetTrackingDetailGuid," 
									 + " ByteNumber," 
									 + " PayloadValue," 
									 + " CreatedDate,"
			                         + " CreatedBy," 
									 + " UpdatedDate," 
									 + " UpdatedBy" 
									 + " ) VALUES ("
									 + " @AssetTrackingPayloadGuid,"
									 + " @AssetTrackingDetailGuid,"
									 + " @ByteNumber,"
									 + " @PayloadValue,"
									 + " @CreatedDate,"
									 + " @CreatedBy,"
									 + " @UpdatedDate,"
									 + " @UpdatedBy"
									 + ") ";

			this.assetTrackingPayloadGuid	= Guid.NewGuid();
			this.createdDate				= DateTimeOffset.Now;
			this.CreatedBy					= security.UserID;
			this.updatedDate				= this.createdDate;
			this.updatedBy					= this.createdBy;

			if (sqlCommand.Parameters.Contains("@AssetTrackingPayloadGuid"))
			{
				sqlCommand.Parameters["@AssetTrackingPayloadGuid"].Value	= this.AssetTrackingPayloadGuid;
				sqlCommand.Parameters["@AssetTrackingDetailGuid"].Value		= this.assetTrackingDetailGuid;
				sqlCommand.Parameters["@ByteNumber"].Value					= this.byteNumber;
				sqlCommand.Parameters["@PayloadValue"].Value				= this.payloadValue;
				sqlCommand.Parameters["@CreatedDate"].Value					= this.createdDate;
				sqlCommand.Parameters["@CreatedBy"].Value					= this.CreatedBy;
				sqlCommand.Parameters["@UpdatedDate"].Value					= this.updatedDate;
				sqlCommand.Parameters["@UpdatedBy"].Value					= this.updatedBy;

				return;
			}

			var parm = new SqlParameter("@AssetTrackingPayloadGuid", SqlDbType.UniqueIdentifier)  { Value = this.AssetTrackingPayloadGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = this.assetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ByteNumber", SqlDbType.Int) { Value = this.byteNumber };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@PayloadValue", SqlDbType.Int) { Value = this.payloadValue };
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
			sqlCommand.CommandText = "UPDATE tblAssetTrackingPayload SET " 
									 + " ByteNumber = @ByteNumber,"
			                         + " PayloadValue = @PayloadValue," 
									 + " UpdatedDate = @UpdatedDate,"
			                         + " UpdatedBy = @UpdatedBy" 
									 + " WHERE AssetTrackingPayloadGuid = @AssetTrackingPayloadGuid";

			var parm = new SqlParameter("@AssetTrackingPayloadGuid", SqlDbType.UniqueIdentifier) { Value = this.AssetTrackingPayloadGuid  };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ByteNumber", SqlDbType.Int) { Value = this.byteNumber };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@PayloadValue", SqlDbType.Int) { Value = this.payloadValue };
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
		/// <param name="inAssetTrackingDetailGuid">The payload to deleted based on this GUID.</param>
		public void DeleteByAssetTrackingDetail(SqlCommand sqlCommand, Guid inAssetTrackingDetailGuid)
		{
			sqlCommand.CommandText = "DELETE FROM tblAssetTrackingPayload "
									 + " WHERE AssetTrackingDetailGuid = @AssetTrackingDetailGuid";

			var parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the Delete by asset tracking payload SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="inAssetTrackingPayloadGuid">The payload to deleted based on this GUID.</param>
		public void DeleteByAssetTrackingPayload(SqlCommand sqlCommand, Guid inAssetTrackingPayloadGuid)
		{
			sqlCommand.CommandText = "DELETE FROM tblAssetTrackingPayload "
									 + " WHERE AssetTrackingPayloadGuid = @AssetTrackingPayloadGuid";

			var parm = new SqlParameter("@AssetTrackingPayloadGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingPayloadGuid };
			sqlCommand.Parameters.Add(parm);
			
		}

		/// <summary>
		/// This method will populate the Get SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="inAssetTrackingPayloadGuid">The asset tracking payload to retrieve.</param>
		public void GetSql(SqlCommand sqlCommand, Guid inAssetTrackingPayloadGuid)
		{
			sqlCommand.CommandText =
				"SELECT * FROM tblAssetTrackingPayload WHERE AssetTrackingPayloadGuid = @AssetTrackingPayloadGuid ";

			var parm = new SqlParameter("@AssetTrackingPayloadGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingPayloadGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the Get All Based On Asset Tracking Detail SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="inAssetTrackingDetailGuid">The asset tracking payload to retrieve using the detail GUID.</param>
		public void GetAllBasedOnAssetTrackingDetailSql(SqlCommand sqlCommand, Guid inAssetTrackingDetailGuid)
		{
			sqlCommand.CommandText =
				"SELECT * FROM tblAssetTrackingPayload WHERE AssetTrackingDetailGuid = @AssetTrackingDetailGuid "
				+ " ORDER BY ByteNumber ";

			var parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDetailGuid };
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

			this.assetTrackingPayloadGuid	= row.IsNull("AssetTrackingPayloadGuid") ? Guid.Empty : (Guid)row["AssetTrackingPayloadGuid"];
			this.assetTrackingDetailGuid	= row.IsNull("AssetTrackingDetailGuid") ? Guid.Empty : (Guid)row["AssetTrackingDetailGuid"];
			this.byteNumber					= row.IsNull("ByteNumber") ? 0 : (int)row["ByteNumber"];
			this.payloadValue				= row.IsNull("PayloadValue") ? 0 : (int)row["PayloadValue"];
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
			this.assetTrackingPayloadGuid	= Guid.Empty;
			this.assetTrackingDetailGuid	= Guid.Empty;
			this.byteNumber					= 0;
			this.payloadValue				= 0;
			this.createdDate				= null;
			this.updatedDate				= null;
			this.createdBy					= string.Empty;
			this.updatedBy					= string.Empty;
		}
		#endregion
	}
}
