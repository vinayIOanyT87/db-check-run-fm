namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class AssetTrackingDetailClass
	{
		#region Public members
		public enum MarkerTypes { None, Marker, Crumb };
		public enum PayloadTypes { None, Tdu, Wrdcu };
		public enum MessageStates { None, Contaminated, Investigate, InvestigateCompletedFailed, InvestigateCompletedPassed }
		#endregion

		#region Private data members
		[DataMember] private Guid assetTrackingDetailGuid;
		[DataMember] private Guid siteGuid;
		[DataMember] private string equipmentId;
		[DataMember] private string productId;
		[DataMember] private string convoyId;
		[DataMember] private string assetTrackingDeviceId;
		[DataMember] private DateTime? assetSessionDateTime;
		[DataMember] private int? assetSessionStatus;
		[DataMember] private int momsn;
		[DataMember] private int mtmsn;
		[DataMember] private int? cdrReference;
		[DataMember] private double? latitude;
		[DataMember] private double? longitude;
		[DataMember] private int? cepRadius;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		[DataMember] private DateTime? completeInvestigationDate;
		[DataMember] private DateTime? startInvestigationDate;
		[DataMember] private string remarks;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		[DataMember] private List<AssetTrackingPayloadClass> payloadValues;
		[DataMember] private List<AssetTrackingTankClass> trackingTanks;
		[DataMember] private MarkerTypes markerType;
		[DataMember] private PayloadTypes payloadType;
		[DataMember] private MessageStates messageState;
		[DataMember] private bool checksumFlag;
		[DataMember] private bool contaminated;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default contructor.
		/// </summary>
		public AssetTrackingDetailClass()
		{
			this.Init();
		}
		#endregion

		#region Propeties
		public Guid AssetTrackingDetailGuid
		{
			get { return this.assetTrackingDetailGuid; }
			set { this.assetTrackingDetailGuid = value; }
		}

		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		public Guid SiteAdminGuid
		{
			get
			{
				const string SiteAdminGuidStr = "00000000-0000-0000-0000-000000000001";
				return Guid.Parse(SiteAdminGuidStr);
			}
		}

		public string EquipmentId
		{
			get { return this.equipmentId; }
			set { this.equipmentId = value; }
		}

		public string ProductId
		{
			get { return this.productId; }
			set { this.productId = value; }
		}

		public string ConvoyId
		{
			get { return this.convoyId; }
			set { this.convoyId = value; }
		}

		public string AssetTrackingDeviceId
		{
			get { return this.assetTrackingDeviceId; }
			set { this.assetTrackingDeviceId = value; }
		}

		public DateTime? AssetSessionDateTime
		{
			get { return this.assetSessionDateTime; }
			set { this.assetSessionDateTime = value; }
		}

		public int? AssetSessionStatus
		{
			get { return this.assetSessionStatus; }
			set { this.assetSessionStatus = value; }
		}

		public int Momsn
		{
			get { return this.momsn; }
			set { this.momsn = value; }
		}

		public int Mtmsn
		{
			get { return this.mtmsn; }
			set { this.mtmsn = value; }
		}

		public int? CdrReference
		{
			get { return this.cdrReference; }
			set { this.cdrReference = value; }
		}

		public double? Latitude
		{
			get { return this.latitude; }
			set { this.latitude = value; }
		}

		public double? Longitude
		{
			get { return this.longitude; }
			set { this.longitude = value; }
		}

		public int? CepRadius
		{
			get { return this.cepRadius; }
			set { this.cepRadius = value; }
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

		public List<AssetTrackingPayloadClass> PayloadValues
		{
			get { return this.payloadValues; }
			set { this.payloadValues = value; }
		}

		public List<AssetTrackingTankClass> TrackingTanks
		{
			get { return this.trackingTanks; }
			set { this.trackingTanks = value; }
		}

		public MarkerTypes MarkerType
		{
			get { return this.markerType; }
			set { this.markerType = value; }
		}

		public PayloadTypes PayloadType
		{
			get { return this.payloadType; }
			set { this.payloadType = value; }
		}

		public MessageStates MessageState
		{
			get { return this.messageState; }
			set { this.messageState = value; }
		}

		public bool ChecksumFlag
		{
			get { return this.checksumFlag; }
			set { this.checksumFlag = value; }
		}

		public bool Contaminated
		{
			get { return this.contaminated; }
			set { this.contaminated = value; }
		}

		public DateTime? CompleteInvestigationDate
		{
			get { return this.completeInvestigationDate; }
			set { this.completeInvestigationDate = value; }
		}

		public DateTime? StartInvestigationDate
		{
			get { return this.startInvestigationDate; }
			set { this.startInvestigationDate = value; }
		}

		public string Remarks
		{
			get { return this.remarks; }
			set { this.remarks = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will populate the insert SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="security">Security object.</param>
		public void InsertSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "INSERT INTO tblAssetTrackingDetail (" 
									 + " AssetTrackingDetailGuid," 
									 + " SiteGuid,"
			                         + " EquipmentID,"
									 + " ProductID,"
									 + " ConvoyID," 
									 + " AssetTrackingDeviceID," 
									 + " AssetSessionDateTime,"
			                         + " AssetSessionStatus," 
									 + " MOMSN," 
									 + " MTMSN," 
									 + " CDRReference," 
									 + " Latitude,"
			                         + " Longitude," 
									 + " CEPRadius,"
									 + " ChecksumFlag,"
									 + " Contaminated,"
									 + " CompleteInvestigationDate,"
									 + " StartInvestigationDate,"
									 + " Remarks,"
									 + " LookupAssetTrackingPayloadTypeIndex,"
									 + " LookupAssetTrackingMessageStateIndex,"
									 + " CreatedDate," 
									 + " CreatedBy," 
									 + " UpdatedDate,"
			                         + " UpdatedBy" 
									 + " ) VALUES (" 
									 + " @AssetTrackingDetailGuid,"
									 + " @SiteGuid,"
									 + " @EquipmentID,"
									 + " @ProductID,"
									 + " @ConvoyID,"
									 + " @AssetTrackingDeviceID,"
									 + " @AssetSessionDateTime,"
									 + " @AssetSessionStatus,"
									 + " @MOMSN,"
									 + " @MTMSN,"
									 + " @CDRReference,"
									 + " @Latitude,"
									 + " @Longitude,"
									 + " @CEPRadius,"
									 + " @ChecksumFlag,"
									 + " @Contaminated,"
									 + " @CompleteInvestigationDate,"
									 + " @StartInvestigationDate,"
									 + " @Remarks,"
									 + " @LookupAssetTrackingPayloadTypeIndex,"
									 + " @LookupAssetTrackingMessageStateIndex,"
									 + " @CreatedDate,"
									 + " @CreatedBy,"
									 + " @UpdatedDate,"
									 + " @UpdatedBy" 
									 + ") ";

			this.AssetTrackingDetailGuid	= Guid.NewGuid();
			this.siteGuid					= this.SiteAdminGuid;
			this.CreatedDate				= DateTimeOffset.Now;
			this.updatedDate				= this.createdDate;
			this.createdBy					= security.UserID;
			this.updatedBy					= security.UserID;

			var parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = this.assetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@EquipmentID", SqlDbType.NVarChar, 30) { Value = this.equipmentId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ProductID", SqlDbType.NVarChar, 30) { Value = this.productId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ConvoyID", SqlDbType.NVarChar, 50) { Value = this.convoyId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetTrackingDeviceID", SqlDbType.NVarChar, 30) { Value = this.AssetTrackingDeviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetSessionDateTime", SqlDbType.DateTime) { Value = this.assetSessionDateTime };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetSessionStatus", SqlDbType.Int) { Value = this.assetSessionStatus };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MOMSN", SqlDbType.Int) { Value = this.momsn };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MTMSN", SqlDbType.Int) { Value = this.mtmsn };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CDRReference", SqlDbType.Int) { Value = this.cdrReference };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Latitude", SqlDbType.Float) { Value = this.latitude };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Longitude", SqlDbType.Float) { Value = this.longitude };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CepRadius", SqlDbType.Int) { Value = this.cepRadius };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ChecksumFlag", SqlDbType.Bit) { Value = this.checksumFlag ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Contaminated", SqlDbType.Bit) { Value = this.contaminated ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupAssetTrackingPayloadTypeIndex", SqlDbType.Int) { Value = this.payloadType };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupAssetTrackingMessageStateIndex", SqlDbType.Int) { Value = this.messageState };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.createdDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.UpdatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 50) { Value = this.createdBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 50) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Remarks", SqlDbType.NVarChar, 4000) { Value = this.remarks };
			sqlCommand.Parameters.Add(parm);

			if (this.completeInvestigationDate == null)
			{
				parm = new SqlParameter("@CompleteInvestigationDate", SqlDbType.DateTime) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@CompleteInvestigationDate", SqlDbType.DateTime) { Value = this.completeInvestigationDate.Value };
				sqlCommand.Parameters.Add(parm);
			}

			if (this.startInvestigationDate == null)
			{
				parm = new SqlParameter("@StartInvestigationDate", SqlDbType.DateTime) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@StartInvestigationDate", SqlDbType.DateTime) { Value = this.startInvestigationDate.Value };
				sqlCommand.Parameters.Add(parm);
			}
		}

		/// <summary>
		/// This method will populate the update SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="security">Security object.</param>
		public void UpdateSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = "UPDATE tblAssetTrackingDetail SET "
									 + " EquipmentID = @EquipmentID,"
									 + " ProductID = @ProductID,"
									 + " ConvoyID = @ConvoyID," 
									 + " AssetTrackingDeviceID = @AssetTrackingDeviceID,"
			                         + " AssetSessionDateTime = @AssetSessionDateTime,"
			                         + " AssetSessionStatus = @AssetSessionStatus," 
									 + " MOMSN = @MOMSN," + " MTMSN = @MTMSN,"
									 + " CDRReference = @CDRReference," 
									 + " Latitude = @Latitude,"
			                         + " Longitude = @Longitude," 
									 + " CEPRadius = @CEPRadius," 
									 + " ChecksumFlag = @ChecksumFlag,"
									 + " Contaminated = @Contaminated,"
									 + " CompleteInvestigationDate = @CompleteInvestigationDate,"
									 + " StartInvestigationDate = @StartInvestigationDate,"
									 + " Remarks = @Remarks,"
									 + " LookupAssetTrackingPayloadTypeIndex = @LookupAssetTrackingPayloadTypeIndex,"
									 + " LookupAssetTrackingMessageStateIndex = @LookupAssetTrackingMessageStateIndex,"
									 + " UpdatedDate = @UpdatedDate,"
			                         + " UpdatedBy = @UpdatedBy" 
									 + " WHERE AssetTrackingDetailGuid = @AssetTrackingDetailGuid ";

			this.updatedDate = DateTimeOffset.Now;
			this.updatedBy = security.UserID;

			var parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = this.assetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@EquipmentID", SqlDbType.NVarChar, 30) { Value = this.equipmentId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ProductID", SqlDbType.NVarChar, 30) { Value = this.productId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ConvoyID", SqlDbType.NVarChar, 50) { Value = this.convoyId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetTrackingDeviceID", SqlDbType.NVarChar, 30) { Value = this.assetTrackingDeviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetSessionDateTime", SqlDbType.DateTime) { Value = this.assetSessionDateTime };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetSessionStatus", SqlDbType.Int) { Value = this.assetSessionStatus };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MOMSN", SqlDbType.Int) { Value = this.momsn };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MTMSN", SqlDbType.Int) { Value = this.mtmsn };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CDRReference", SqlDbType.Int) { Value = this.cdrReference };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Latitude", SqlDbType.Float) { Value = this.latitude };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Longitude", SqlDbType.Float) { Value = this.longitude };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CEPRadius", SqlDbType.Int) { Value = this.cepRadius };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ChecksumFlag", SqlDbType.Bit) { Value = this.checksumFlag ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Contaminated", SqlDbType.Bit) { Value = this.contaminated ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupAssetTrackingPayloadTypeIndex", SqlDbType.Int) { Value = this.payloadType };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupAssetTrackingMessageStateIndex", SqlDbType.Int) { Value = this.messageState };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 30) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Remarks", SqlDbType.NVarChar, 4000) { Value = this.remarks };
			sqlCommand.Parameters.Add(parm);

			if (this.completeInvestigationDate == null)
			{
				parm = new SqlParameter("@CompleteInvestigationDate", SqlDbType.DateTime) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@CompleteInvestigationDate", SqlDbType.DateTime) { Value = this.completeInvestigationDate.Value };
				sqlCommand.Parameters.Add(parm);
			}

			if (this.startInvestigationDate == null)
			{
				parm = new SqlParameter("@StartInvestigationDate", SqlDbType.DateTime) { Value = DBNull.Value };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				parm = new SqlParameter("@StartInvestigationDate", SqlDbType.DateTime) { Value = this.startInvestigationDate.Value };
				sqlCommand.Parameters.Add(parm);
			}
		}

		/// <summary>
		/// This method will populate the update the asset tracking detail records with an investigate state
		/// based on the asset tracking GUID list.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="security">Security object.</param>
		/// <param name="assetTrackingGuidList">The GUID list of the asset tracking records to update.</param>
		public void UpdateRecordsToInvestigateStateSql(SqlCommand sqlCommand, SecurityClass security, List<string> assetTrackingGuidList)
		{
			sqlCommand.CommandText = "UPDATE tblAssetTrackingDetail SET StartInvestigationDate = @StartInvestigationDate,"
						 + " UpdatedBy = @UpdatedBy,"
						 + " UpdatedDate = @UpdatedDate,"
						 + " LookupAssetTrackingMessageStateIndex = atms.AssetTrackingMessageStateIndex"
						 + " FROM lookup.tblAssetTrackingMessageState atms "
						 + " WHERE AssetTrackingDetailGuid IN (SELECT * FROM udf_SplitString(@AssetTrackingDetailGuidList, ',', 0)) "
						 + " AND atms.AssetTrackingMessageStateCode = 'INVESTIGATE' ";

			var startInvestigateDate = DateTime.Now;
			this.updatedDate = DateTimeOffset.Now;
			this.updatedBy = security.UserID;

			var parm = new SqlParameter("@StartInvestigationDate", SqlDbType.DateTime) { Value = startInvestigateDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 30) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);

			bool firstRecord = true;
			string parmValue = string.Empty;

			foreach (var assetTrackingGuid in assetTrackingGuidList)
			{
				if (firstRecord)
				{
					parmValue = parmValue + assetTrackingGuid;
					firstRecord = false;
				}
				else
				{
					parmValue = parmValue + "," + assetTrackingGuid;
				}
			}

			parm = new SqlParameter("@AssetTrackingDetailGuidList", SqlDbType.NVarChar, 200000) { Value = parmValue };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the update the asset tracking detail records with an investigate state
		/// based on the asset tracking GUID list.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="security">Security object.</param>
		/// <param name="startDate">This date is 60 days in the past.</param>
		/// <param name="deviceId">The device ID to filter on.</param>
		/// <param name="completeState">Complete state is either completed failed or completed passed.</param>
		/// <param name="completeInvestigateDate">The date time the investigate was completed.</param>
		public void UpdateRecordsToInvestigateCompleteStateSql(SqlCommand sqlCommand, 
																SecurityClass security,  
																DateTime startDate,
																string deviceId,
																MessageStates completeState,
																DateTime completeInvestigateDate)
		{
			sqlCommand.CommandText = "UPDATE tblAssetTrackingDetail SET CompleteInvestigationDate = @CompleteInvestigationDate,"
			                       + " UpdatedBy = @UpdatedBy,"
			                       + " UpdatedDate = @UpdatedDate,"
			                       + " LookupAssetTrackingMessageStateIndex = @LookupAssetTrackingMessageStateIndex"
			                       + " FROM lookup.tblAssetTrackingMessageState atms"
			                       + " WHERE StartInvestigationDate IS NOT NULL"
								   + " AND AssetSessionDateTime >= @StartDate"
								   + " AND AssetTrackingDeviceID = @DeviceId"
								   + " AND LookupAssetTrackingMessageStateIndex = atms.AssetTrackingMessageStateIndex"
			                       + " AND atms.AssetTrackingMessageStateCode = 'INVESTIGATE'";

			this.updatedDate = DateTimeOffset.Now;
			this.updatedBy = security.UserID;

			var parm = new SqlParameter("@CompleteInvestigationDate", SqlDbType.DateTime) { Value = completeInvestigateDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = startDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 30) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@DeviceId", SqlDbType.NVarChar, 30) { Value = deviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@LookupAssetTrackingMessageStateIndex", SqlDbType.Int) { Value = (int)completeState };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the update the asset tracking detail records with an investigate state
		/// based on the asset tracking GUID list.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="security">Security object.</param>
		/// <param name="startDate">This date is 60 days in the past.</param>
		/// <param name="deviceId">The device ID to filter on.</param>
		/// <param name="inRemarks">The investigation remarks.</param>
		/// <param name="completeInvestigateDate">The date time the investigate was completed.</param>
		public void UpdateRemarksOnInvestigateCompleteSql(SqlCommand sqlCommand,
																SecurityClass security,
																DateTime startDate,
																string deviceId,
																string inRemarks,
																DateTime completeInvestigateDate)
		{
			sqlCommand.CommandText = " UPDATE tblAssetTrackingDetail SET Remarks = @Remarks,"
								   + " UpdatedBy = @UpdatedBy,"
								   + " UpdatedDate = @UpdatedDate"
								   + " FROM lookup.tblAssetTrackingMessageState atms"
								   + " WHERE StartInvestigationDate IS NOT NULL"
								   + " AND CompleteInvestigationDate IS NOT NULL"
								   + " AND AssetSessionDateTime >= @StartDate"
								   + " AND AssetTrackingDeviceID = @DeviceId"
								   + " AND LookupAssetTrackingMessageStateIndex = atms.AssetTrackingMessageStateIndex"
								   + " AND (atms.AssetTrackingMessageStateCode = 'INVESTIGATION_COMPLETED_FAILED' OR atms.AssetTrackingMessageStateCode = 'INVESTIGATION_COMPLETED_PASSED')"
								   + " AND AssetTrackingDetailGuid = (SELECT TOP(1) AssetTrackingDetailGuid FROM tblAssetTrackingDetail"
								   + " WHERE Contaminated = 1 AND CompleteInvestigationDate = @CompleteInvestigationDate"
								   + " ORDER BY tblAssetTrackingDetail.AssetSessionDateTime ASC)";


			this.updatedDate = DateTimeOffset.Now;
			this.updatedBy = security.UserID;

			var parm = new SqlParameter("@CompleteInvestigationDate", SqlDbType.DateTime) { Value = completeInvestigateDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = startDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 30) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@DeviceId", SqlDbType.NVarChar, 30) { Value = deviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Remarks", SqlDbType.NVarChar, 4000) { Value = inRemarks };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the update remarks SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="security">Security object.</param>
		/// <param name="inAssetTrackingDetailGuid">The asset tracking detail GUID used to update the remarks.</param>
		/// <param name="inRemarks">The remarks to save.</param>
		public void UpdateRemarksSql(SqlCommand sqlCommand, SecurityClass security, Guid inAssetTrackingDetailGuid, string inRemarks)
		{
			sqlCommand.CommandText = "UPDATE tblAssetTrackingDetail SET Remarks = @Remarks,"
									 + " UpdatedBy = @UpdatedBy,"
									 + " UpdatedDate = @UpdatedDate"
									 + " WHERE AssetTrackingDetailGuid = @AssetTrackingDetailGuid ";

			this.updatedDate = DateTimeOffset.Now;
			this.updatedBy = security.UserID;

			var parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 30) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@Remarks", SqlDbType.NVarChar, 4000) { Value = inRemarks };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the Get SQL command.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="inAssetTrackingDetailGuid">The asset tracking detail to retrieve.</param>
		public void GetSql(SqlCommand sqlCommand, Guid inAssetTrackingDetailGuid)
		{
			sqlCommand.CommandText =
				"SELECT * FROM tblAssetTrackingDetail WHERE AssetTrackingDetailGuid = @AssetTrackingDetailGuid ";

			var parm = new SqlParameter("@AssetTrackingDetailGuid", SqlDbType.UniqueIdentifier) { Value = inAssetTrackingDetailGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the Get By Filter SQL.
		/// </summary>
		/// <param name="sqlCommand">SQL command to be populated.</param>
		/// <param name="inDeviceId">The asset tracking device filter</param>
		/// <param name="topOne">Whether to only get the top one record.</param>
		public void GetByFilterSql(SqlCommand sqlCommand, string inDeviceId, bool topOne)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingDetail";

			if (topOne)
			{
				sqlCommand.CommandText = "SELECT TOP 1 * FROM tblAssetTrackingDetail";
			}

			if (string.IsNullOrEmpty(inDeviceId) == false)
			{
				sqlCommand.CommandText = sqlCommand.CommandText + " WHERE AssetTrackingDeviceID = @AssetTrackingDeviceID";
				var parm = new SqlParameter("@AssetTrackingDeviceID", SqlDbType.NVarChar, 30) { Value = inDeviceId };
				sqlCommand.Parameters.Add(parm);
			}

			sqlCommand.CommandText = sqlCommand.CommandText + " ORDER BY AssetTrackingDeviceID, AssetSessionDateTime DESC ";
		}

		/// <summary>
		/// This method will populate the get by device and most current SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="deviceId">The device associated to the asset tracking detail record.</param>
		public void GetByDeviceAndMostCurrentSql(SqlCommand sqlCommand, string deviceId)
		{
			sqlCommand.CommandText = "SELECT TOP(1) * FROM tblAssetTrackingDetail"
									+ " WHERE AssetTrackingDeviceID = @AssetTrackingDeviceID"
									+ " ORDER BY AssetSessionDateTime DESC ";

			var parm = new SqlParameter("@AssetTrackingDeviceID", SqlDbType.NVarChar, 30) { Value = deviceId };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get by device list SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="devices">The list of devices to use in the where clause.</param>
		/// <param name="startDate">This is the start date to retrieve the data. Sixty days from current date.</param>
		public void GetByDeviceListSql(SqlCommand sqlCommand, List<AssetTrackingDeviceClass> devices, DateTime startDate)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingDetail";

			int parmCount = 0;
			sqlCommand.CommandText = sqlCommand.CommandText + " WHERE AssetSessionDateTime >= @AssetSessionDateTime AND AssetTrackingDeviceID IN (";

			var parm = new SqlParameter("@AssetSessionDateTime", SqlDbType.DateTime) { Value = startDate };
			sqlCommand.Parameters.Add(parm);

			foreach (AssetTrackingDeviceClass device in devices)
			{
				string parmVariable = "@DeviceID" + parmCount;

				if (parmCount == 0)
				{
					sqlCommand.CommandText = sqlCommand.CommandText + parmVariable;
				}
				else
				{
					sqlCommand.CommandText = sqlCommand.CommandText + "," + parmVariable;
				}

				parmCount++;

				parm = new SqlParameter(parmVariable, SqlDbType.NVarChar, 30) { Value = device.DeviceId };
				sqlCommand.Parameters.Add(parm);
			}

			sqlCommand.CommandText = sqlCommand.CommandText + ") ";
			sqlCommand.CommandText = sqlCommand.CommandText + " ORDER BY AssetTrackingDeviceID, AssetSessionDateTime ASC ";
		}

		/// <summary>
		/// This method will populate the get by device list SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="deviceId">The device ID to use in the where clause.</param>
		/// <param name="startDateTime">This is the start based on 60 days from the current time.</param>
		public void GetLast60DaysByDeviceListSql(SqlCommand sqlCommand, string deviceId, DateTime startDateTime)
		{
			sqlCommand.CommandText = "SELECT * FROM tblAssetTrackingDetail"
			                         + " WHERE AssetTrackingDeviceID = @AssetTrackingDeviceID AND AssetSessionDateTime >= @AssetSessionDateTime"
									 + " ORDER BY AssetSessionDateTime ASC";

			var parm = new SqlParameter("@AssetTrackingDeviceID", SqlDbType.NVarChar, 30) { Value = deviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetSessionDateTime", SqlDbType.DateTime) { Value = startDateTime };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the get the number of investigate states found list SQL.
		/// </summary>
		/// <param name="sqlCommand">The SQL command to populate.</param>
		/// <param name="deviceId">The device ID to use in the where clause.</param>
		/// <param name="startDateTime">This is the start based on 60 days from the current time.</param>
		public void FoundInvestigateStatesSql(SqlCommand sqlCommand, string deviceId, DateTime startDateTime)
		{
			sqlCommand.CommandText = "SELECT COUNT(*) AS NumberFound"
									 + " FROM tblAssetTrackingDetail atd LEFT OUTER JOIN "
									 + " lookup.tblAssetTrackingMessageState atms ON atd.LookupAssetTrackingMessageStateIndex = atms.AssetTrackingMessageStateIndex"
									 + " WHERE atd.AssetTrackingDeviceID = @AssetTrackingDeviceID "
									 + " AND atd.AssetSessionDateTime >= @AssetSessionDateTime"
									 + " AND atms.AssetTrackingMessageStateCode = 'INVESTIGATE'";

			var parm = new SqlParameter("@AssetTrackingDeviceID", SqlDbType.NVarChar, 30) { Value = deviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssetSessionDateTime", SqlDbType.DateTime) { Value = startDateTime };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will load the object.
		/// </summary>
		/// <param name="row">Row from the database.</param>
		public void Load(DataRow row)
		{
			if (row == null)
			{
				return;
			}

			this.assetTrackingDetailGuid	= row.IsNull("AssetTrackingDetailGuid") ? Guid.Empty : (Guid)row["AssetTrackingDetailGuid"];
			this.siteGuid					= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
			this.equipmentId				= row.IsNull("EquipmentID") ? string.Empty : (string)row["EquipmentID"];
			this.productId					= row.IsNull("ProductID") ? string.Empty : (string)row["ProductID"];
			this.convoyId					= row.IsNull("ConvoyID") ? string.Empty : (string)row["ConvoyID"];
			this.assetTrackingDeviceId		= row.IsNull("AssetTrackingDeviceId") ? string.Empty : (string)row["AssetTrackingDeviceId"];
			this.assetSessionStatus			= row.IsNull("AssetSessionStatus") ? null : (int?)row["AssetSessionStatus"];
			this.momsn						= row.IsNull("MOMSN") ? 0 : (int)row["MOMSN"];
			this.mtmsn						= row.IsNull("MTMSN") ? 0 : (int)row["MTMSN"];
			this.cdrReference				= row.IsNull("CDRReference") ? null : (int?)row["CDRReference"];
			this.latitude					= row.IsNull("Latitude") ? null : (double?)row["Latitude"];
			this.longitude					= row.IsNull("Longitude") ? null : (double?)row["Longitude"];
			this.cepRadius					= row.IsNull("CEPRadius") ? null : (int?)row["CEPRadius"];
			this.checksumFlag				= row.IsNull("ChecksumFlag") ? false : (bool)row["ChecksumFlag"];
			this.contaminated				= row.IsNull("Contaminated") ? false : (bool)row["Contaminated"];
			this.payloadType				= row.IsNull("LookupAssetTrackingPayloadTypeIndex") ? PayloadTypes.None : (PayloadTypes)row["LookupAssetTrackingPayloadTypeIndex"];
			this.messageState				= row.IsNull("LookupAssetTrackingMessageStateIndex") ? MessageStates.None : (MessageStates) row["LookupAssetTrackingMessageStateIndex"];
			this.createdBy					= row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
			this.updatedBy					= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];
			this.remarks					= row.IsNull("Remarks") ? string.Empty : (string) row["Remarks"];

			this.assetSessionDateTime = null;
			if (row.IsNull("AssetSessionDateTime") == false)
			{
				this.assetSessionDateTime = (DateTime)row["AssetSessionDateTime"];
			}

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

			this.completeInvestigationDate = null;
			if (row.IsNull("CompleteInvestigationDate") == false)
			{
				this.completeInvestigationDate = (DateTime) row["CompleteInvestigationDate"];
			}

			this.startInvestigationDate = null;
			if (row.IsNull("StartInvestigationDate") == false)
			{
				this.startInvestigationDate = (DateTime) row["StartInvestigationDate"];
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.assetTrackingDetailGuid		= Guid.Empty;
			this.siteGuid						= Guid.Empty;
			this.equipmentId					= string.Empty;
			this.productId						= string.Empty;
			this.convoyId						= string.Empty;
			this.assetTrackingDeviceId			= string.Empty;
			this.assetSessionDateTime			= null;
			this.assetSessionStatus				= null;
			this.momsn							= 0;
			this.mtmsn							= 0;
			this.cdrReference					= null;
			this.latitude						= null;
			this.longitude						= null;
			this.cepRadius						= null;
			this.createdDate					= null;
			this.updatedDate					= null;
			this.createdBy						= string.Empty;
			this.updatedBy						= string.Empty;
			this.markerType						= MarkerTypes.None;
			this.payloadType					= PayloadTypes.None;
			this.messageState					= MessageStates.None;
			this.checksumFlag					= false;
			this.contaminated					= false;
			this.completeInvestigationDate		= null;
			this.startInvestigationDate			= null;
			this.remarks						= string.Empty;
		}
		#endregion
	}
}
