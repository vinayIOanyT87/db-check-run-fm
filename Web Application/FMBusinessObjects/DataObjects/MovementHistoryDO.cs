namespace FMBusinessObjects.DataObjects
{
    using Microsoft.SqlServer.Server;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;


	[Serializable]
	[DataContract]
	public class MovementHistoryDO : BaseDataObject
	{
		#region Public data members
		public enum MovementRecordTypes { Movement, Node, Handgauge, Final, None };
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementHistoryDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		[DataMember] public Guid MovementHistoryGuid { get; set; }
		[DataMember] public Guid PointGuid { get; set; }
		[DataMember] public string Name { get; set; }
		[DataMember] public string Node { get; set; }
		[DataMember] public long? InitiationCount { get; set; }
		[DataMember] public MovementRecordTypes RecordType { get; set; }
		[DataMember] public DateTime? TimeStamp { get; set; }
		[DataMember] public Guid ParentGuid { get; set; }
		[DataMember] public bool AutoStart { get; set; }
		[DataMember] public DateTimeOffset? AutoStartTime { get; set; }
		[DataMember] public bool AutoStop { get; set; }
		[DataMember] public DateTimeOffset? AutoStopTime { get; set; }
		[DataMember] public string CloseoutDataModifiedBy { get; set; }
		[DataMember] public double? CloseoutDensityProductInAir { get; set; }
		[DataMember] public double? CloseoutDensityProductObserved { get; set; }
		[DataMember] public DateTimeOffset? CloseoutDensityProductObservedTime { get; set; }
		[DataMember] public double? CloseoutDensityProductStandard { get; set; }
		[DataMember] public DateTimeOffset? CloseoutDensityProductStandardTime { get; set; }
		[DataMember] public double? CloseoutDensityProductStandardInAir { get; set; }
		[DataMember] public double? CloseoutLevelProduct { get; set; }
		[DataMember] public DateTimeOffset? CloseoutLevelProductTime { get; set; }
		[DataMember] public double? CloseoutLevelWater { get; set; }
		[DataMember] public DateTimeOffset? CloseoutLevelWaterTime { get; set; } // TODO new
		[DataMember] public double? CloseoutMassLiquid { get; set; }
		[DataMember] public double? CloseoutPercentBsw { get; set; }
		[DataMember] public double? CloseoutRoofMass { get; set; }
		[DataMember] public double? CloseoutTankShellCorrection { get; set; }
		[DataMember] public double? CloseoutTemperatureAmbient { get; set; }
		[DataMember] public DateTimeOffset? CloseoutTemperatureAmbientTime { get; set; }
		[DataMember] public double? CloseoutTemperatureDensity { get; set; }
		[DataMember] public DateTimeOffset? CloseoutTemperatureDensityTime { get; set; } //TODO new
		[DataMember] public double? CloseoutTemperatureProduct { get; set; }
		[DataMember] public DateTimeOffset? CloseoutTemperatureProductTime { get; set; }   //TODO new
		[DataMember] public DateTimeOffset? CloseoutTime { get; set; }
		[DataMember] public double? CloseoutTransferGov { get; set; }
		[DataMember] public double? CloseoutTransferNsv { get; set; }
		[DataMember] public double? CloseoutTransferMassLiquid { get; set; }
		[DataMember] public double? CloseoutTransferVolumeWater { get; set; }
		[DataMember] public double? CloseoutVolumeBsw { get; set; }
		[DataMember] public double? CloseoutVolumeCorrectionFactor { get; set; }
		[DataMember] public double? CloseoutVolumeGrossObserved { get; set; }
		[DataMember] public double? CloseoutVolumeGrossStandard { get; set; }
		[DataMember] public double? CloseoutVolumeNetStandard { get; set; }
		[DataMember] public double? CloseoutVolumeRoofCorrection { get; set; }
		[DataMember] public double? CloseoutVolumeTotalObserved { get; set; }
		[DataMember] public double? CloseoutVolumeWater { get; set; }
		[DataMember] public string Comment { get; set; }
		[DataMember] public string Type { get; set; }
		[DataMember] public string OrderNumber { get; set; }
		[DataMember] public DateTimeOffset? PlannedStartTime { get; set; }
		[DataMember] public string Product { get; set; }
		[DataMember] public string ProductDescription { get; set; }
		[DataMember] public DateTimeOffset? StartTime { get; set; }
		[DataMember] public DateTimeOffset? StopTime { get; set; }
		[DataMember] public double? StartDensityProductObserved { get; set; }
		[DataMember] public DateTimeOffset? StartDensityProductObservedTime { get; set; }
		[DataMember] public double? StartDensityProductObservedInAir { get; set; }
		[DataMember] public double? StartDensityProductStandard { get; set; }
		[DataMember] public double? StartDensityProductStandardInAir { get; set; }
		[DataMember] public DateTimeOffset? StartDensityProductStandardTime { get; set; }
		[DataMember] public string StartUserID { get; set; }
		[DataMember] public double? StartLevelProduct { get; set; }
		[DataMember] public DateTimeOffset? StartLevelProductTime { get; set; }
		[DataMember] public double? StartLevelWater { get; set; }
		[DataMember] public DateTimeOffset? StartLevelWaterTime { get; set; }
		[DataMember] public double? StartMassLiquid { get; set; }
        [DataMember] public double? StartPercentBsw { get; set; }
        [DataMember] public double? StartTankShellCorrection { get; set; }
		[DataMember] public double? StartTemperatureAmbient { get; set; }
		[DataMember] public DateTimeOffset? StartTemperatureAmbientTime { get; set; }
		[DataMember] public double? StartTemperatureProduct { get; set; }
		[DataMember] public DateTimeOffset? StartTemperatureProductTime { get; set; }
		[DataMember] public double? StartTemperatureDensity { get; set; }
		[DataMember] public DateTimeOffset? StartTemperatureDensityTime { get; set; }
        [DataMember] public double? StartVolume { get; set; }
        [DataMember] public double? StartVolumeBsw { get; set; }
        [DataMember] public double? StartVolumeCorrectionFactor { get; set; }
		[DataMember] public double? StartVolumeGrossObserved { get; set; }
		[DataMember] public double? StartVolumeGrossStandard { get; set; }
		[DataMember] public double? StartVolumeNetStandard { get; set; }
		[DataMember] public double? StartVolumeRoofCorrection { get; set; }
		[DataMember] public double? StartVolumeTotalObserved { get; set; }
		[DataMember] public double? StartVolumeWater { get; set; }
		[DataMember] public int? UnitsLevelProductIndex { get; set; }
		[DataMember] public int? UnitsTemperatureAmbientIndex { get; set; }
		[DataMember] public int? UnitsTemperatureDensityIndex { get; set; }
		[DataMember] public int? UnitsTemperatureProductIndex { get; set; }
		[DataMember] public int? UnitsDensityProductObservedIndex { get; set; }
		[DataMember] public int? UnitsDensityProductStandardIndex { get; set; }
		[DataMember] public int? UnitsVolumeIndex { get; set; }
		[DataMember] public int? DecimalPlacesVolume { get; set; }
		[DataMember] public int? DecimalPlacesLevel { get; set; }
		[DataMember] public int? DecimalPlacesDensity { get; set; }
		[DataMember] public int? DecimalPlacesTemperature { get; set; }
        [DataMember] public int? DecimalPlacesPercent { get; set; }
        [DataMember] public int? UnitsMassIndex { get; set; }
		[DataMember] public string UserData01 { get; set; }
		[DataMember] public string UserData02 { get; set; }
		[DataMember] public string UserData03 { get; set; }
		[DataMember] public string UserData04 { get; set; }
		[DataMember] public string UserData05 { get; set; }
		[DataMember] public string UserData06 { get; set; }
		[DataMember] public string UserData07 { get; set; }
		[DataMember] public string UserData08 { get; set; }
		[DataMember] public string UserData09 { get; set; }
		[DataMember] public string UserData10 { get; set; }
		[DataMember] public double? TransferDeviation { get; set; }
      [DataMember] public double? TransferPercentDeviation { get; set; }
      [DataMember] public int? TransferMode { get; set; }
		[DataMember] public int? TransferStatus { get; set; }
		[DataMember] public double? TransferTarget { get; set; }
		[DataMember] public int? TransferTargetUnitsIndex { get; set; }
		[DataMember] public double? TransferLevelTarget { get; set; }
		[DataMember] public double? TransferVolumeTarget { get; set; }
		[DataMember] public long? TransferTimeRemaining { get; set; }
		[DataMember] public string TransferDirection { get; set; }
		[DataMember] public DateTime? CommentDateTime { get; set; }
		[DataMember] public string CommentUserId { get; set; }
		[DataMember] public int? Status { get; set; }
		[DataMember] public double? VolumeWater { get; set; }
		[DataMember] public double? LevelProduct { get; set; }
		[DataMember] public double? TransferredVolumeWater { get; set; }
		[DataMember] public double? TransferredVolume { get; set; }
		[DataMember] public bool MidnightRecord { get; set; }
		[DataMember] public Guid RootParentGuid { get; set; }
		[DataMember] public int RecordSeq { get; set; }
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will populate the SQL command to save a movement.
		/// </summary>
		/// <param name="cmd">SQL command to populate.</param>
		public void SaveMovementHistorySql(SqlCommand cmd)
        {
			// Call the stored procedure, passing in the movement history table.
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementHistoryAddUpdate";

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@MovementHistoryParmTable", SqlDbType.Structured);
			tableValuedParameter.Value = this.CreateSqlDataRecords();
			tableValuedParameter.TypeName = "dbo.MovementHistoryType";
		}

		/// <summary>
		/// This method will populate the SQL command to delete a movement from the 
		/// movement history.
		/// </summary>
		/// <param name="cmd">SQL command to populate.</param>
		/// <param name="movementHistoryGuid">The movement history Guid to delete.</param>
		/// <param name="movementName">The movement name to delete.</param>
		/// <param name="siteGuid">The movement site Guid to delete.</param>
		public void DeleteMovementHistoryByMovementNameSql(SqlCommand cmd, Guid movementHistoryGuid, string movementName, Guid siteGuid)
        {
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementHistoryDeleteMovement";

            var parm = new SqlParameter("@MovementHistoryGuid", SqlDbType.UniqueIdentifier) { Value = movementHistoryGuid };
            cmd.Parameters.Add(parm);

			parm = new SqlParameter("@MovementName", SqlDbType.UniqueIdentifier) { Value = movementName };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = siteGuid };
			cmd.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command to delete a movement from the 
		/// movement history.
		/// </summary>
		/// <param name="cmd">SQL command to populate.</param>
		/// <param name="movementHistoryGuid">The movement history Guid to update the comment.</param>
		/// <param name="commentUserId">The comment user ID that is updating the comment.</param>
		/// <param name="commentDateTime">The comment date time that the comment is updated.</param>
		public void UpdateMovementHistoryCommentSql(SqlCommand cmd, Guid movementHistoryGuid, string comment, string commentUserId, DateTime commentDateTime)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementHistoryUpdateComment";

			var parm = new SqlParameter("@MovementHistoryGuid", SqlDbType.UniqueIdentifier) { Value = movementHistoryGuid };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@Comment", SqlDbType.NVarChar, 1000) { Value = comment };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@CommentUserID", SqlDbType.NVarChar, 50) { Value = commentUserId };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@CommentDateTime", SqlDbType.DateTime) { Value = commentDateTime };
			cmd.Parameters.Add(parm);
		}

		/// <summary>
		/// This method populates the SQL command to retrieve the final record assocated to a 
		/// hand gauge.
		/// </summary>
		/// <param name="cmd">The SQL command object.</param>
		/// <param name="rootParentGuid">The root parent Guid to associate</param>
		/// <param name="parentGuid">The parent Guid to associate</param>
		public void GetFinalRecordInfoAssociatedToHandgaugeSql(SqlCommand cmd, Guid rootParentGuid, Guid parentGuid)
        {
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementHistoryGetFinalRecordInfo";

			var parm = new SqlParameter("@RootParentGuid", SqlDbType.UniqueIdentifier) { Value = rootParentGuid };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@ParentGuid", SqlDbType.UniqueIdentifier) { Value = parentGuid };
			cmd.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command to get a movement from the 
		/// movement history.
		/// </summary>
		/// <param name="cmd">SQL command to populate.</param>
		/// <param name="movementName">The movement name to retrieve.</param>
		/// <param name="siteGuid">The movement site Guid to retrieve.</param>
		public void GetMovementByMovementNameSql(SqlCommand cmd, string movementName, Guid siteGuid)
        {
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementHistoryGetMovementByMovementGuid";

			var parm = new SqlParameter("@MovementName", SqlDbType.UniqueIdentifier) { Value = movementName };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = siteGuid };
			cmd.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command to get a movement record from the 
		/// movement history.
		/// </summary>
		/// <param name="cmd">SQL command to populate.</param>
		/// <param name="movementHistoryGuid">The movement history Guid to retrieve.</param>
		/// <param name="siteGuid">The movement site Guid to retrieve.</param>
		public void GetMovementRecordByGuidSql(SqlCommand cmd, Guid movementHistoryGuid, Guid siteGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementHistoryGetRecordByGuid";

			var parm = new SqlParameter("@MovementHistoryGuid", SqlDbType.UniqueIdentifier) { Value = movementHistoryGuid };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = siteGuid };
			cmd.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command to get all movements from the 
		/// movement history based on site.
		/// </summary>
		/// <param name="cmd">SQL command to populate.</param>
		/// <param name="siteGuid">The movement site Guid to retrieve.</param>
		/// <param name="startTime">The movement start Date/Time to retrieve.</param>
		/// <param name="endTime">The movement end Date/Time to retrieve.</param>
		/// <param name="orderColumnName">The column name to order by.</param>
		/// <param name="orderDirection">The order direction.</param>
		public void GetAllMovementsBySiteGuidSql(SqlCommand cmd
												, Guid siteGuid
												, DateTime startTime
												, DateTime endTime
												, bool autoGauge
												, bool handGauge
												, bool midnightRecord
												, string orderColumnName
												, string orderDirection)
        {
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementHistoryGetAllMovementsBySite";

			var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = siteGuid };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = startTime };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = endTime };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@OrderColumnName", SqlDbType.NVarChar, 100) { Value = orderColumnName };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@OrderDirection", SqlDbType.NVarChar, 10) { Value = orderDirection };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@AutoGauge", SqlDbType.Bit) { Value = autoGauge ? 1 : 0 };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@HandGauge", SqlDbType.Bit) { Value = handGauge ? 1 : 0 };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@MidnightRecord", SqlDbType.Bit) { Value = midnightRecord ? 1 : 0 };
			cmd.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command to get all movements from the 
		/// movement history based on site and the load request number.
		/// </summary>
		/// <param name="cmd">SQL command to populate.</param>
		/// <param name="siteGuid">The movement site Guid to retrieve.</param>
		/// <param name="initialLoadCount">The initial load count to retrieve.</param>
		public void GetMovementsByInitialLoadRequestSql(SqlCommand cmd, Guid siteGuid, int initialLoadCount)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MovementHistoryGetMovementsByInitialLoadRequest";

			var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = siteGuid };
			cmd.Parameters.Add(parm);

			parm = new SqlParameter("@InitialLoadCount", SqlDbType.Int) { Value = initialLoadCount };
			cmd.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will load a movement history record from the database.
		/// </summary>
		/// <param name="row">The row to load.</param>
		public void Load(DataRow row)
        {
			this.MovementHistoryGuid					= row.IsNull("MovementHistoryGuid") ? Guid.Empty : (Guid)row["MovementHistoryGuid"];
			this.SiteGuid								= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
			this.Name									= row.IsNull("Name") ? string.Empty : (string)row["Name"];
			this.Node									= row.IsNull("Node") ? string.Empty : (string)row["Node"];
			this.InitiationCount						= this.IsNullLong(row, "InitiationCount");
			this.RecordType								= this.IsNullRecordType(row, "RecordType");
			this.TimeStamp								= this.IsNullDateTimeUtc(row, "TimeStamp");
			this.ParentGuid								= row.IsNull("ParentGuid") ? Guid.Empty : (Guid)row["ParentGuid"];
			this.AutoStart								= row.IsNull("AutoStart") ? false : (bool)row["AutoStart"];
			this.AutoStartTime							= this.IsNullDateTime(row, "AutoStartTime");
			this.AutoStop								= row.IsNull("AutoStop") ? false : (bool)row["AutoStop"];
			this.AutoStopTime							= this.IsNullDateTime(row, "AutoStopTime");
			this.CloseoutDataModifiedBy					= row.IsNull("CloseoutDataModifiedBy") ? string.Empty : (string)row["CloseoutDataModifiedBy"];
			this.CloseoutDensityProductInAir			= this.IsNullDouble(row,  "CloseoutDensityProductInAir");
			this.CloseoutDensityProductObserved			= this.IsNullDouble(row, "CloseoutDensityProductObserved");
			this.CloseoutDensityProductObservedTime		= this.IsNullDateTime(row, "CloseoutDensityProductObservedTime");
			this.CloseoutDensityProductStandard			= this.IsNullDouble(row, "CloseoutDensityProductStandard");
			this.CloseoutDensityProductStandardTime		= this.IsNullDateTime(row, "CloseoutDensityProductStandardTime");
			this.CloseoutDensityProductStandardInAir	= this.IsNullDouble(row, "CloseoutDensityProductStandardInAir");
			this.CloseoutLevelProduct					= this.IsNullDouble(row, "CloseoutLevelProduct");
			this.CloseoutLevelProductTime				= this.IsNullDateTime(row, "CloseoutLevelProductTime");
			this.CloseoutLevelWater						= this.IsNullDouble(row, "CloseoutLevelWater");
			this.CloseoutMassLiquid						= this.IsNullDouble(row, "CloseoutMassLiquid");
			this.CloseoutPercentBsw						= this.IsNullDouble(row, "CloseoutPercentBsw");
			this.CloseoutRoofMass						= this.IsNullDouble(row, "CloseoutRoofMass");
			this.CloseoutTankShellCorrection			= this.IsNullDouble(row, "CloseoutTankShellCorrection");
			this.CloseoutTemperatureAmbient				= this.IsNullDouble(row, "CloseoutTemperatureAmbient");
			this.CloseoutTemperatureAmbientTime			= this.IsNullDateTime(row, "CloseoutTemperatureAmbientTime");
			this.CloseoutTemperatureDensity				= this.IsNullDouble(row, "CloseoutTemperatureDensity");
			this.CloseoutTemperatureProduct				= this.IsNullDouble(row, "CloseoutTemperatureProduct");
			this.CloseoutTime							= this.IsNullDateTime(row, "CloseoutTime");
			this.CloseoutTransferGov					= this.IsNullDouble(row, "CloseoutTransferGov");
			this.CloseoutTransferNsv					= this.IsNullDouble(row, "CloseoutTransferNsv");
			this.CloseoutTransferMassLiquid				= this.IsNullDouble(row, "CloseoutTransferMassLiquid");
			this.CloseoutTransferVolumeWater			= this.IsNullDouble(row, "CloseoutTransferVolumeWater");
			this.CloseoutVolumeBsw						= this.IsNullDouble(row, "CloseoutVolumeBsw");
			this.CloseoutVolumeCorrectionFactor			= this.IsNullDouble(row, "CloseoutVolumeCorrectionFactor");
			this.CloseoutVolumeGrossObserved			= this.IsNullDouble(row, "CloseoutVolumeGrossObserved");
			this.CloseoutVolumeNetStandard				= this.IsNullDouble(row, "CloseoutVolumeNetStandard");
			this.CloseoutVolumeGrossStandard			= this.IsNullDouble(row, "CloseoutVolumeGrossStandard");
			this.CloseoutVolumeRoofCorrection			= this.IsNullDouble(row, "CloseoutVolumeRoofCorrection");
			this.CloseoutVolumeTotalObserved			= this.IsNullDouble(row, "CloseoutVolumeTotalObserved");
			this.CloseoutVolumeWater					= this.IsNullDouble(row, "CloseoutVolumeWater");
			this.Comment									= row.IsNull("Comment") ? string.Empty : (string)row["Comment"];
			this.Type										= row.IsNull("Type") ? string.Empty : (string)row["Type"];
			this.OrderNumber								= row.IsNull("OrderNumber") ? string.Empty : (string)row["OrderNumber"];
			this.PlannedStartTime						= this.IsNullDateTime(row, "PlannedStartTime");
			this.Product									= row.IsNull("Product") ? string.Empty : (string)row["Product"];
			this.ProductDescription						= row.IsNull("ProductDescription") ? string.Empty : (string)row["ProductDescription"];
			this.StartTime									= this.IsNullDateTime(row, "StartTime");
			this.StopTime									= this.IsNullDateTime(row, "StopTime");
			this.StartDensityProductObserved			= this.IsNullDouble(row, "StartDensityProductObserved");
			this.StartDensityProductObservedTime		= this.IsNullDateTime(row, "StartDensityProductObservedTime");
			this.StartDensityProductObservedInAir		= this.IsNullDouble(row, "StartDensityProductObservedInAir");
			this.StartDensityProductStandard			= this.IsNullDouble(row, "StartDensityProductStandard");
			this.StartDensityProductStandardTime		= this.IsNullDateTime(row, "StartDensityProductStandardTime");
			this.StartUserID							= row.IsNull("StartUserID") ? string.Empty : (string)row["StartUserID"];
			this.StartLevelProduct						= this.IsNullDouble(row, "StartLevelProduct");
			this.StartLevelProductTime					= this.IsNullDateTime(row, "StartLevelProductTime");
			this.StartLevelWater						= this.IsNullDouble(row, "StartLevelWater");
			this.StartLevelWaterTime					= this.IsNullDateTime(row, "StartLevelWaterTime");
			this.StartMassLiquid						= this.IsNullDouble(row, "StartMassLiquid");
            this.StartPercentBsw = this.IsNullDouble(row, "StartPercentBsw");
            this.StartTankShellCorrection				= this.IsNullDouble(row, "StartTankShellCorrection");
			this.StartTemperatureAmbient				= this.IsNullDouble(row, "StartTemperatureAmbient");
			this.StartTemperatureAmbientTime			= this.IsNullDateTime(row, "StartTemperatureAmbientTime");
			this.StartTemperatureProduct				= this.IsNullDouble(row, "StartTemperatureProduct");
			this.StartTemperatureProductTime			= this.IsNullDateTime(row, "StartTemperatureProductTime");
			this.StartTemperatureDensity				= this.IsNullDouble(row, "StartTemperatureDensity");
			this.StartTemperatureDensityTime			= this.IsNullDateTime(row, "StartTemperatureDensityTime");
			this.StartVolume							= this.IsNullDouble(row, "StartVolume");
            this.StartVolumeBsw							= this.IsNullDouble(row, "StartVolumeBsw");
            this.StartVolumeCorrectionFactor			= this.IsNullDouble(row, "StartVolumeCorrectionFactor");
			this.StartVolumeGrossObserved				= this.IsNullDouble(row, "StartVolumeGrossObserved");
			this.StartVolumeGrossStandard				= this.IsNullDouble(row, "StartVolumeGrossStandard");
			this.StartVolumeNetStandard					= this.IsNullDouble(row, "StartVolumeNetStandard");
			this.StartVolumeRoofCorrection				= this.IsNullDouble(row, "StartVolumeRoofCorrection");
			this.StartVolumeTotalObserved				= this.IsNullDouble(row, "StartVolumeTotalObserved");
			this.StartVolumeWater						= this.IsNullDouble(row, "StartVolumeWater");
			this.UnitsLevelProductIndex					= this.IsNullInt(row, "UnitsLevelProductIndex");
			this.UnitsTemperatureAmbientIndex			= this.IsNullInt(row, "UnitsTemperatureAmbientIndex");
			this.UnitsTemperatureDensityIndex			= this.IsNullInt(row, "UnitsTemperatureDensityIndex");
			this.UnitsTemperatureProductIndex			= this.IsNullInt(row, "UnitsTemperatureProductIndex");
			this.UnitsDensityProductObservedIndex		= this.IsNullInt(row, "UnitsDensityProductObservedIndex");
			this.UnitsDensityProductStandardIndex		= this.IsNullInt(row, "UnitsDensityProductStandardIndex");
			this.UnitsVolumeIndex						= this.IsNullInt(row, "UnitsVolumeIndex");
			this.UnitsMassIndex							= this.IsNullInt(row, "UnitsMassIndex");
			this.DecimalPlacesDensity					= this.IsNullInt(row, "DecimalPlacesDensity");
			this.DecimalPlacesLevel						= this.IsNullInt(row, "DecimalPlacesLevel");
			this.DecimalPlacesTemperature				= this.IsNullInt(row, "DecimalPlacesTemperature");
			this.DecimalPlacesVolume					= this.IsNullInt(row, "DecimalPlacesVolume");
			this.DecimalPlacesPercent					= this.IsNullInt(row, "DecimalPlacesPercent");
            this.UserData01								= row.IsNull("UserData01") ? string.Empty : (string)row["UserData01"];
			this.UserData02								= row.IsNull("UserData02") ? string.Empty : (string)row["UserData02"];
			this.UserData03								= row.IsNull("UserData03") ? string.Empty : (string)row["UserData03"];
			this.UserData04								= row.IsNull("UserData04") ? string.Empty : (string)row["UserData04"];
			this.UserData05								= row.IsNull("UserData05") ? string.Empty : (string)row["UserData05"];
			this.UserData06								= row.IsNull("UserData06") ? string.Empty : (string)row["UserData06"];
			this.UserData07								= row.IsNull("UserData07") ? string.Empty : (string)row["UserData07"];
			this.UserData08								= row.IsNull("UserData08") ? string.Empty : (string)row["UserData08"];
			this.UserData09								= row.IsNull("UserData09") ? string.Empty : (string)row["UserData09"];
			this.UserData01								= row.IsNull("UserData10") ? string.Empty : (string)row["UserData10"];
			this.TransferDeviation						= this.IsNullDouble(row, "TransferDeviation");
         this.TransferPercentDeviation				= this.IsNullDouble(row, "TransferPercentDeviation");
         this.TransferMode								= this.IsNullInt(row, "TransferMode");
			this.TransferStatus							= this.IsNullInt(row, "TransferStatus");
			this.TransferTarget							= this.IsNullDouble(row, "TransferTarget");
			this.TransferTargetUnitsIndex				= this.IsNullInt(row, "TransferTargetUnitsIndex");
			this.TransferLevelTarget					= this.IsNullDouble(row, "TransferLevelTarget");
			this.TransferVolumeTarget					= this.IsNullDouble(row, "TransferVolumeTarget");
			this.TransferTimeRemaining					= this.IsNullLong(row, "TransferTimeRemaining");
			this.TransferDirection						= row.IsNull("TransferDirection") ? string.Empty : (string)row["TransferDirection"];
			this.CommentDateTime							= this.IsNullDateTimeUtc(row, "CommentDateTime");
			this.CommentUserId							= row.IsNull("CommentUserID") ? string.Empty : (string)row["CommentUserID"];
			this.Status										= this.IsNullInt(row, "Status"); ;
			this.VolumeWater								= this.IsNullDouble(row, "VolumeWater");
			this.LevelProduct								= this.IsNullDouble(row, "LevelProduct");
			this.StartDensityProductStandardInAir	= this.IsNullDouble(row, "StartDensityProductStandardInAir");
			this.TransferredVolumeWater				= this.IsNullDouble(row, "TransferredVolumeWater");
			this.TransferredVolume						= this.IsNullDouble(row, "TransferredVolume");
			this.MidnightRecord							= row.IsNull("MidnightRecord") ? false : (bool)row["MidnightRecord"];
			this.PointGuid									= row.IsNull("PointGuid") ? Guid.Empty : (Guid)row["PointGuid"];
			this.RootParentGuid							= row.IsNull("RootParentGuid") ? Guid.Empty : (Guid)row["RootParentGuid"];
			this.RecordSeq									= row.IsNull("RecordSeq") ? 0 : (int)row["RecordSeq"];
			this.CreatedBy									= row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
			this.UpdatedBy									= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];
			base.CreatedDate								= row.IsNull("CreatedDate") ? DateTimeOffset.Now : (DateTimeOffset)row["CreatedDate"];
			base.UpdatedDate								= row.IsNull("UpdatedDate") ? DateTimeOffset.Now : (DateTimeOffset)row["UpdatedDate"];
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will check for a null double and return the correct value.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Returns the value or null.</returns>
		private double? IsNullDouble(DataRow row, string columnName)
		{
			if (row.IsNull(columnName) == false)
			{
				return (double)row[columnName];
			}

			return null;
		}

		/// <summary>
		/// This method will check for a null Int and return the correct value.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Returns the value or null.</returns>
		private int? IsNullInt(DataRow row, string columnName)
		{
			if (row.IsNull(columnName) == false)
			{
				return (int)row[columnName];
			}

			return null;
		}

		/// <summary>
		/// This method will check for a null Uint and return the correct value.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Returns the value or null.</returns>
		private uint? IsNullUInt(DataRow row, string columnName)
		{
			if (row.IsNull(columnName) == false)
			{
				return (uint)row[columnName];
			}

			return null;
		}

		/// <summary>
		/// This method will check for a null Long and return the correct value.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Returns the value or null.</returns>
		private long? IsNullLong(DataRow row, string columnName)
		{
			if (row.IsNull(columnName) == false)
			{
				return (long)row[columnName];
			}

			return null;
		}

		/// <summary>
		/// This method will check for a null movement record tyep and return the correct value.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Returns the value or movement record type of none.</returns>
		private MovementRecordTypes IsNullRecordType(DataRow row, string columnName)
		{
			if (row.IsNull(columnName) == false)
			{
				return (MovementRecordTypes)row[columnName];
			}

			return MovementRecordTypes.None;
		}

		/// <summary>
		/// This method will check for a null date time offset and return the correct value.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Returns the value or null.</returns>
		private DateTimeOffset? IsNullDateTime(DataRow row, string columnName)
		{
			if (row.IsNull(columnName) == false)
			{
				return (DateTimeOffset)row[columnName];
			}

			return null;
		}

		/// <summary>
		/// This method will check for a null date time  and return the correct value.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <param name="columnName">The column name</param>
		/// <returns>Returns the value or null.</returns>
		private DateTime? IsNullDateTimeUtc(DataRow row, string columnName)
		{
			if (row.IsNull(columnName) == false)
			{
				return (DateTime)row[columnName];
			}

			return null;
		}

		/// <summary>
		/// This method will populate the movement history record with data.
		/// </summary>
		/// <returns>Returns the movement history record.</returns>
		private IEnumerable<SqlDataRecord> CreateSqlDataRecords()
		{
			SqlMetaData[] metaData = new SqlMetaData[126];
			int i = 0;

			metaData[i++] = new SqlMetaData("MovementHistoryGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("Name", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("Node", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("InitiationCount", SqlDbType.BigInt);
			metaData[i++] = new SqlMetaData("RecordType", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("TimeStamp", SqlDbType.DateTime);
			metaData[i++] = new SqlMetaData("ParentGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("AutoStart", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("AutoStartTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("AutoStop", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("AutoStopTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("CloseoutDataModifiedBy", SqlDbType.NVarChar, 50);
			metaData[i++] = new SqlMetaData("CloseoutDensityProductInAir", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutDensityProductObserved", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutDensityProductObservedTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("CloseoutDensityProductStandard", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutDensityProductStandardTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("CloseoutDensityProductStandardInAir]", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutLevelProduct", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutLevelProductTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("CloseoutLevelWater", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutMassLiquid", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutPercentBsw", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutRoofMass", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutTankShellCorrection", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutTemperatureAmbient", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutTemperatureAmbientTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("CloseoutTemperatureDensity", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutTemperatureProduct", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("CloseoutTransferGov", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutTransferGsv", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutTransferMassLiquid", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutTransferVolumeWater", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutVolumeBsw", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutVolumeCorrectionFactor", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutVolumeGrossObserved", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutVolumeGrossStandard", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutVolumeNetStandard", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutVolumeRoofCorrection", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutVolumeTotalObserved", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("CloseoutVolumeWater", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("Comment", SqlDbType.NVarChar, 1000);
			metaData[i++] = new SqlMetaData("Type", SqlDbType.NVarChar, 20);
			metaData[i++] = new SqlMetaData("OrderNumber", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("PlannedStartTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("Product", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("ProductDescription", SqlDbType.NVarChar, 1000);
			metaData[i++] = new SqlMetaData("StartTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StopTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StartDensityProductObserved", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartDensityProductObservedTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StartDensityProductObservedInAir", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartDensityProductStandard", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartDensityProductStandardTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StartUserID", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("StartLevelProduct", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartLevelProductTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StartLevelWater", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartLevelWaterTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StartMassLiquid", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("StartPercentBsw", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("StartTankShellCorrection", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartTemperatureAmbient", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartTemperatureAmbientTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StartTemperatureProduct", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartTemperatureProductTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StartTemperatureDensity", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartTemperatureDensityTime", SqlDbType.DateTimeOffset);
			metaData[i++] = new SqlMetaData("StartVolume", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("StartVolumeBsw", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("StartVolumeCorrectionFactor", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartVolumeGrossObserved", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartVolumeGrossStandard", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartVolumeNetStandard", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartVolumeRoofCorrection", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartVolumeTotalObserved", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartVolumeWater", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("UnitsLevelProductIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("UnitsTemperatureAmbientIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("UnitsTemperatureDensityIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("UnitsTemperatureProductIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("UnitsDensityProductObservedIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("UnitsDensityProductStandardIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("UnitsVolumeIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("UnitsMassIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("DecimalPlacesDensity", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("DecimalPlacesLevel", SqlDbType.Int); 
			metaData[i++] = new SqlMetaData("DecimalPlacesTemperature", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("DecimalPlacesVolume", SqlDbType.Int);
         metaData[i++] = new SqlMetaData("UserData01", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData02", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData03", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData04", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData05", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData06", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData07", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData08", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData09", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UserData10", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("TransferDeviation", SqlDbType.Float);
         metaData[i++] = new SqlMetaData("TransferPercentDeviation", SqlDbType.Float);
         metaData[i++] = new SqlMetaData("DecimalPlacesPercent", SqlDbType.Int);
         metaData[i++] = new SqlMetaData("TransferMode", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("TransferStatus", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("TransferTarget", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("TransferTargetUnitsIndex", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("TransferLevelTarget", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("TransferVolumeTarget", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("TransferTimeRemaining", SqlDbType.BigInt);
			metaData[i++] = new SqlMetaData("TransferDirection", SqlDbType.NVarChar, 20);
			metaData[i++] = new SqlMetaData("CommentDateTime", SqlDbType.DateTime);
			metaData[i++] = new SqlMetaData("CommentUserID", SqlDbType.NVarChar, 50);
			metaData[i++] = new SqlMetaData("Status", SqlDbType.BigInt);
			metaData[i++] = new SqlMetaData("VolumeWater", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("LevelProduct", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("StartDensityProductStandardInAir", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("TransferredVolumeWater", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("TransferredVolume", SqlDbType.Float);
			metaData[i++] = new SqlMetaData("MidnightRecord", SqlDbType.Bit);
			metaData[i++] = new SqlMetaData("PointGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("RootParentGuid", SqlDbType.UniqueIdentifier);
			metaData[i++] = new SqlMetaData("RecordSeq", SqlDbType.Int);
			metaData[i++] = new SqlMetaData("CreatedBy", SqlDbType.NVarChar, 100);
			metaData[i++] = new SqlMetaData("UpdatedBy", SqlDbType.NVarChar, 100);

			SqlDataRecord record = new SqlDataRecord(metaData);

			int j = 0;
			record.SetGuid(j++, this.MovementHistoryGuid);
			record.SetGuid(j++, this.SiteGuid);
			record.SetString(j++, this.Name);
			record.SetString(j++, this.Node);
			this.SetLong(ref record, j++, this.InitiationCount);
			record.SetInt32(j++, (int)this.RecordType);
			this.SetDateTimeUtc(ref record, j++, this.TimeStamp);
			record.SetGuid(j++, this.ParentGuid);
			record.SetBoolean(j++, this.AutoStart);
			this.SetDateTimeOffset(ref record, j++, this.AutoStartTime);
			record.SetBoolean(j++, this.AutoStop);
			this.SetDateTimeOffset(ref record, j++, this.AutoStopTime);
			this.SetString(ref record, j++, this.CloseoutDataModifiedBy);
			this.SetDouble(ref record, j++, this.CloseoutDensityProductInAir);
			this.SetDouble(ref record, j++, this.CloseoutDensityProductObserved);
			this.SetDateTimeOffset(ref record, j++, this.CloseoutDensityProductObservedTime);
			this.SetDouble(ref record, j++, this.CloseoutDensityProductStandard);
			this.SetDateTimeOffset(ref record, j++, this.CloseoutDensityProductStandardTime);
			this.SetDouble(ref record, j++, this.CloseoutDensityProductStandardInAir);
			this.SetDouble(ref record, j++, this.CloseoutLevelProduct);
			this.SetDateTimeOffset(ref record, j++, this.CloseoutLevelProductTime);
			this.SetDouble(ref record, j++, this.CloseoutLevelWater);
			this.SetDouble(ref record, j++, this.CloseoutMassLiquid);
			this.SetDouble(ref record, j++, this.CloseoutPercentBsw);
			this.SetDouble(ref record, j++, this.CloseoutRoofMass);
			this.SetDouble(ref record, j++, this.CloseoutTankShellCorrection);
			this.SetDouble(ref record, j++, this.CloseoutTemperatureAmbient);
			this.SetDateTimeOffset(ref record, j++, this.CloseoutTemperatureAmbientTime);
			this.SetDouble(ref record, j++, this.CloseoutTemperatureDensity);
			this.SetDouble(ref record, j++, this.CloseoutTemperatureProduct);
			this.SetDateTimeOffset(ref record, j++, this.CloseoutTime);
			this.SetDouble(ref record, j++, this.CloseoutTransferGov);
			this.SetDouble(ref record, j++, this.CloseoutTransferNsv);
			this.SetDouble(ref record, j++, this.CloseoutTransferMassLiquid);
			this.SetDouble(ref record, j++, this.CloseoutTransferVolumeWater);
			this.SetDouble(ref record, j++, this.CloseoutVolumeBsw);
			this.SetDouble(ref record, j++, this.CloseoutVolumeCorrectionFactor);
			this.SetDouble(ref record, j++, this.CloseoutVolumeGrossObserved);
			this.SetDouble(ref record, j++, this.CloseoutVolumeGrossStandard);
			this.SetDouble(ref record, j++, this.CloseoutVolumeNetStandard);
			this.SetDouble(ref record, j++, this.CloseoutVolumeRoofCorrection);
			this.SetDouble(ref record, j++, this.CloseoutVolumeTotalObserved);
			this.SetDouble(ref record, j++, this.CloseoutVolumeWater);
			this.SetString(ref record, j++, this.Comment);
			this.SetString(ref record, j++, this.Type);
			this.SetString(ref record, j++, this.OrderNumber);
			this.SetDateTimeOffset(ref record, j++, this.PlannedStartTime);
			this.SetString(ref record, j++, this.Product);
			this.SetString(ref record, j++, this.ProductDescription);
			this.SetDateTimeOffset(ref record, j++, this.StartTime);
			this.SetDateTimeOffset(ref record, j++, this.StopTime);
			this.SetDouble(ref record, j++, this.StartDensityProductObserved);
			this.SetDateTimeOffset(ref record, j++, this.StartDensityProductObservedTime);
			this.SetDouble(ref record, j++, this.StartDensityProductObservedInAir);
			this.SetDouble(ref record, j++, this.StartDensityProductStandard);
			this.SetDateTimeOffset(ref record, j++, this.StartDensityProductStandardTime);
			this.SetString(ref record, j++, this.StartUserID);
			this.SetDouble(ref record, j++, this.StartLevelProduct);
			this.SetDateTimeOffset(ref record, j++, this.StartLevelProductTime);
			this.SetDouble(ref record, j++, this.StartLevelWater);
			this.SetDateTimeOffset(ref record, j++, this.StartLevelWaterTime);
			this.SetDouble(ref record, j++, this.StartMassLiquid);
            this.SetDouble(ref record, j++, this.StartPercentBsw);
            this.SetDouble(ref record, j++, this.StartTankShellCorrection);
			this.SetDouble(ref record, j++, this.StartTemperatureAmbient);
			this.SetDateTimeOffset(ref record, j++, this.StartTemperatureAmbientTime);
			this.SetDouble(ref record, j++, this.StartTemperatureProduct);
			this.SetDateTimeOffset(ref record, j++, this.StartTemperatureProductTime);
			this.SetDouble(ref record, j++, this.StartTemperatureDensity);
			this.SetDateTimeOffset(ref record, j++, this.StartTemperatureDensityTime);
			this.SetDouble(ref record, j++, this.StartVolume);
            this.SetDouble(ref record, j++, this.StartVolumeBsw);
            this.SetDouble(ref record, j++, this.StartVolumeCorrectionFactor);
			this.SetDouble(ref record, j++, this.StartVolumeGrossObserved);
			this.SetDouble(ref record, j++, this.StartVolumeGrossStandard);
			this.SetDouble(ref record, j++, this.StartVolumeNetStandard);
			this.SetDouble(ref record, j++, this.StartVolumeRoofCorrection);
			this.SetDouble(ref record, j++, this.StartVolumeTotalObserved);
			this.SetDouble(ref record, j++, this.StartVolumeWater);
			this.SetInt(ref record, j++, this.UnitsLevelProductIndex);
			this.SetInt(ref record, j++, this.UnitsTemperatureAmbientIndex);
			this.SetInt(ref record, j++, this.UnitsTemperatureDensityIndex);
			this.SetInt(ref record, j++, this.UnitsTemperatureProductIndex);
			this.SetInt(ref record, j++, this.UnitsDensityProductObservedIndex);
			this.SetInt(ref record, j++, this.UnitsDensityProductStandardIndex);
			this.SetInt(ref record, j++, this.UnitsVolumeIndex);
			this.SetInt(ref record, j++, this.UnitsMassIndex);
			this.SetInt(ref record, j++, this.DecimalPlacesDensity);
			this.SetInt(ref record, j++, this.DecimalPlacesLevel);
			this.SetInt(ref record, j++, this.DecimalPlacesTemperature);
			this.SetInt(ref record, j++, this.DecimalPlacesVolume);
         this.SetString(ref record, j++, this.UserData01);
			this.SetString(ref record, j++, this.UserData02);
			this.SetString(ref record, j++, this.UserData03);
			this.SetString(ref record, j++, this.UserData04);
			this.SetString(ref record, j++, this.UserData05);
			this.SetString(ref record, j++, this.UserData06);
			this.SetString(ref record, j++, this.UserData07);
			this.SetString(ref record, j++, this.UserData08);
			this.SetString(ref record, j++, this.UserData09);
			this.SetString(ref record, j++, this.UserData10);
			this.SetDouble(ref record, j++, this.TransferDeviation);
         this.SetDouble(ref record, j++, this.TransferPercentDeviation);
         this.SetInt(ref record, j++, this.DecimalPlacesPercent);
         this.SetInt(ref record, j++, this.TransferMode);
			this.SetInt(ref record, j++, this.TransferStatus);
			this.SetDouble(ref record, j++, this.TransferTarget);
			this.SetInt(ref record, j++, this.TransferTargetUnitsIndex);
			this.SetDouble(ref record, j++, this.TransferLevelTarget);
			this.SetDouble(ref record, j++, this.TransferVolumeTarget);
			this.SetLong(ref record, j++, this.TransferTimeRemaining);
			this.SetString(ref record, j++, this.TransferDirection);
			this.SetDateTimeUtc(ref record, j++, this.CommentDateTime);
			this.SetString(ref record, j++, this.CommentUserId);
			this.SetLong(ref record, j++, this.Status);
			this.SetDouble(ref record, j++, this.VolumeWater);
			this.SetDouble(ref record, j++, this.LevelProduct);
			this.SetDouble(ref record, j++, this.StartDensityProductStandardInAir);
			this.SetDouble(ref record, j++, this.TransferredVolumeWater);
			this.SetDouble(ref record, j++, this.TransferredVolume);
			record.SetBoolean(j++, this.MidnightRecord);
			record.SetGuid(j++, this.PointGuid);
			record.SetGuid(j++, this.RootParentGuid);
			this.SetInt(ref record, j++, this.RecordSeq);
			this.SetString(ref record, j++, this.CreatedBy);
			this.SetString(ref record, j++, this.UpdatedBy);

			yield return record;
		}

		/// <summary>
		/// This method will check for a null string value and set the record
		/// to DB NULL.
		/// </summary>
		/// <param name="record">The record to update.</param>
		/// <param name="index">The column index.</param>
		/// <param name="strValue">The value to store.</param>
		private void SetString(ref SqlDataRecord record, int index, string strValue)
        {
			if(string.IsNullOrEmpty(strValue))
            {
				record.SetDBNull(index);
            }
			else
            {
				record.SetString(index, strValue);
            }
		}

		/// <summary>
		/// This method will check for a null date time offset value and set the record
		/// to DB NULL.
		/// </summary>
		/// <param name="record">The record to update.</param>
		/// <param name="index">The column index.</param>
		/// <param name="DateTimeOffset">The value to store.</param>
		private void SetDateTimeOffset(ref SqlDataRecord record, int index, DateTimeOffset? dateTime)
		{
			if (dateTime == null)
			{
				record.SetDBNull(index);
			}
			else
			{
				record.SetDateTimeOffset(index, dateTime.Value);
			}
		}

		/// <summary>
		/// This method will check for a null date time value and set the record
		/// to DB NULL. This is value should be UTC.
		/// </summary>
		/// <param name="record">The record to update.</param>
		/// <param name="index">The column index.</param>
		/// <param name="DateTime">The UTC value to store.</param>
		private void SetDateTimeUtc(ref SqlDataRecord record, int index, DateTime? dateTime)
		{
			if (dateTime == null)
			{
				record.SetDBNull(index);
			}
			else
			{
				record.SetDateTime(index, dateTime.Value);
			}
		}

		/// <summary>
		/// This method will check for a null double value and set the record
		/// to DB NULL.
		/// </summary>
		/// <param name="record">The record to update.</param>
		/// <param name="index">The column index.</param>
		/// <param name="doubleValue">The value to store.</param>
		private void SetDouble(ref SqlDataRecord record, int index, double? doubleValue)
		{
			if (doubleValue == null)
			{
				record.SetDBNull(index);
			}
			else
			{
				record.SetDouble(index, doubleValue.Value);
			}
		}

		/// <summary>
		/// This method will check for a null integer value and set the record
		/// to DB NULL.
		/// </summary>
		/// <param name="record">The record to update.</param>
		/// <param name="index">The column index.</param>
		/// <param name="intValue">The value to store.</param>
		private void SetInt(ref SqlDataRecord record, int index, int? intValue)
		{
			if (intValue == null)
			{
				record.SetDBNull(index);
			}
			else
			{
				record.SetInt32(index, intValue.Value);
			}
		}

		/// <summary>
		/// This method will check for a null long value and set the record
		/// to DB NULL.
		/// </summary>
		/// <param name="record">The record to update.</param>
		/// <param name="index">The column index.</param>
		/// <param name="intValue">The value to store.</param>
		private void SetLong(ref SqlDataRecord record, int index, long? longValue)
		{
			if (longValue == null)
			{
				record.SetDBNull(index);
			}
			else
			{
				record.SetInt64(index, longValue.Value);
			}
		}

		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
        {
			this.MovementHistoryGuid				= Guid.Empty;
			this.PointGuid							= Guid.Empty;
			base.SiteGuid							= Guid.Empty;
			this.Name								= string.Empty;
			this.Node								= string.Empty;
			this.InitiationCount					= null;
			this.RecordType							= MovementRecordTypes.None;
			this.TimeStamp							= null;
			this.ParentGuid							= Guid.Empty;
			this.AutoStart							= false;
			this.AutoStartTime						= null;
			this.AutoStop							= false;
			this.AutoStopTime						= null;
			this.CloseoutDataModifiedBy				= string.Empty;
			this.CloseoutDensityProductInAir		= null;
			this.CloseoutDensityProductObserved		= null;
			this.CloseoutDensityProductObservedTime = null;
			this.CloseoutDensityProductStandard		= null;
			this.CloseoutDensityProductStandardTime = null;
			this.CloseoutDensityProductStandardInAir = null;
			this.CloseoutLevelProduct				= null;
			this.CloseoutLevelProductTime			= null;
			this.CloseoutLevelWater					= null;
			this.CloseoutMassLiquid					= null;
			this.CloseoutPercentBsw					= null;
			this.CloseoutRoofMass					= null;
			this.CloseoutTankShellCorrection		= null;
			this.CloseoutTemperatureAmbient			= null;
			this.CloseoutTemperatureAmbientTime		= null;
			this.CloseoutTemperatureDensity			= null;
			this.CloseoutTemperatureProduct			= null;
			this.CloseoutTime						= null;
			this.CloseoutTransferGov				= null;
			this.CloseoutTransferNsv				= null;
			this.CloseoutTransferMassLiquid			= null;
			this.CloseoutTransferVolumeWater		= null;
			this.CloseoutVolumeBsw					= null;
			this.CloseoutVolumeCorrectionFactor		= null;
			this.CloseoutVolumeGrossObserved		= null;
			this.CloseoutVolumeGrossStandard		= null;
			this.CloseoutVolumeNetStandard		= null;
			this.CloseoutVolumeRoofCorrection	= null;
			this.CloseoutVolumeTotalObserved		= null;
			this.CloseoutVolumeWater				= null;
			this.Comment								= string.Empty;
			this.Type									= string.Empty;
			this.OrderNumber							= string.Empty;
			this.PlannedStartTime					= null;
			this.Product								= string.Empty;
			this.ProductDescription					= string.Empty;
			this.StartTime								= null;
			this.StopTime								= null;
			this.StartDensityProductObserved		= null;
			this.StartDensityProductObservedTime	= null;
			this.StartDensityProductObservedInAir	= null;
			this.StartDensityProductStandard		= null;
			this.StartDensityProductStandardTime	= null;
			this.StartUserID						= string.Empty;
			this.StartLevelProduct					= null;
			this.StartLevelProductTime				= null;
			this.StartLevelWater					= null;
			this.StartLevelWaterTime				= null;
			this.StartMassLiquid					= null;
            this.StartPercentBsw = null;
            this.StartTankShellCorrection			= null;
			this.StartTemperatureAmbient			= null;
			this.StartTemperatureAmbientTime		= null;
			this.StartTemperatureProduct			= null;
			this.StartTemperatureProductTime		= null;
			this.StartTemperatureDensity			= null;
			this.StartTemperatureDensityTime		= null;
			this.StartVolume						= null;
            this.StartVolumeBsw						= null;
            this.StartVolumeCorrectionFactor		= null;
			this.StartVolumeGrossObserved			= null;
			this.StartVolumeGrossStandard			= null;
			this.StartVolumeNetStandard				= null;
			this.StartVolumeRoofCorrection			= null;
			this.StartVolumeTotalObserved			= null;
			this.StartVolumeWater					= null;
			this.UnitsLevelProductIndex				= null;
			this.UnitsTemperatureAmbientIndex		= null;
			this.UnitsTemperatureDensityIndex		= null;
			this.UnitsTemperatureProductIndex		= null;
			this.UnitsDensityProductObservedIndex	= null;
			this.UnitsDensityProductStandardIndex	= null;
			this.UnitsVolumeIndex					= null;
			this.UnitsMassIndex						= null;
			this.DecimalPlacesDensity				= null;
			this.DecimalPlacesLevel					= null;
			this.DecimalPlacesTemperature			= null;
			this.DecimalPlacesVolume				= null;
            this.DecimalPlacesPercent				= null;
            this.UserData01							= string.Empty;
			this.UserData02							= string.Empty;
			this.UserData03							= string.Empty;
			this.UserData04							= string.Empty;
			this.UserData05							= string.Empty;
			this.UserData06							= string.Empty;
			this.UserData07							= string.Empty;
			this.UserData08							= string.Empty;
			this.UserData09							= string.Empty;
			this.UserData10							= string.Empty;
			this.TransferDeviation					= null;
         this.TransferPercentDeviation			= null;
         this.TransferMode							= null;
			this.TransferStatus						= null;
			this.TransferTarget						= null;
			this.TransferTargetUnitsIndex			= null;
			this.TransferLevelTarget				= null;
			this.TransferVolumeTarget				= null;
			this.TransferTimeRemaining				= null;
			this.TransferDirection					= string.Empty;
			this.CommentUserId						= string.Empty;
			this.CommentDateTime						= null;
			this.Status									= null;
			this.VolumeWater							= null;
			this.LevelProduct							= null;
			this.StartDensityProductStandardInAir	= null;
			this.TransferredVolumeWater				= null;
			this.TransferredVolume					= null;
			this.MidnightRecord						= false;
			this.RootParentGuid						= Guid.Empty;
			this.RecordSeq								= 0;
			base.CreatedBy								= string.Empty;
			base.CreatedDate							= DateTimeOffset.Now;
			base.UpdatedBy								= string.Empty;
			base.UpdatedDate							= DateTimeOffset.Now;
		}
        #endregion
    }
}
