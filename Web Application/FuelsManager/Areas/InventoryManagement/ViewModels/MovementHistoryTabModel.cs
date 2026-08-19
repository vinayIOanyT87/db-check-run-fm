namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class MovementHistoryTabRow
	{
		public const string RowPrefix = "Row_";

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementHistoryTabRow()
        {
			this.Init();
        }
		#endregion

		#region Properties
		public string DT_RowId { get; set; }
		public Guid MovementHistoryGuid { get; set; }
		public string Name { get; set; }
		public string Node { get; set; }
		public string SiteId { get; set; }
		public long? InitiationCount { get; set; }
		public int RecordType { get; set; }
		public string TimeStampStr { get; set; }
		public Guid ParentGuid { get; set; }
		public bool AutoStart { get; set; }
		public string AutoStartTimeStr { get; set; }
		public bool AutoStop { get; set; }
		public string AutoStopTimeStr { get; set; }
		public string CloseoutDataModifiedBy { get; set; }
		public string CloseoutDensityProductInAirStr { get; set; }
		public string CloseoutDensityProductObservedStr { get; set; }
		public string CloseoutDensityProductObservedTimeStr { get; set; }
		public string CloseoutDensityProductStandardStr { get; set; }
		public string CloseoutDensityProductStandardTimeStr { get; set; }
		public string CloseoutDensityProductStandardInAirStr { get; set; }
		public string CloseoutLevelProductStr { get; set; }
		public string CloseoutLevelProductTimeStr { get; set; }
		public string CloseoutLevelWaterStr { get; set; }
		public string CloseoutMassLiquidStr { get; set; }
		public string CloseoutPercentBswStr { get; set; }
		public string CloseoutRoofMassStr { get; set; }
		public string CloseoutTankShellCorrectionStr { get; set; }
		public string CloseoutTemperatureAmbientStr { get; set; }
		public string CloseoutTemperatureAmbientTimeStr { get; set; }
		public string CloseoutTemperatureDensityStr { get; set; }
		public string CloseoutTemperatureProductStr { get; set; }
		public string CloseoutTimeStr { get; set; }
		public string CloseoutTransferGovStr { get; set; }
		public string CloseoutTransferNsvStr { get; set; }
		public string CloseoutTransferMassLiquidStr { get; set; }
		public string CloseoutTransferVolumeWaterStr { get; set; }
		public string CloseoutVolumeBswStr { get; set; }
		public string CloseoutVolumeCorrectionFactorStr { get; set; }
		public string CloseoutVolumeGrossObservedStr { get; set; }
		public string CloseoutVolumeGrossStandardStr { get; set; }
		public string CloseoutVolumeNetStandardStr { get; set; }
		public string CloseoutVolumeRoofCorrectionStr { get; set; }
		public string CloseoutVolumeTotalObservedStr { get; set; }
		public string CloseoutVolumeWaterStr { get; set; }
		public string Comment { get; set; }
		public string Type { get; set; }
		public string OrderNumber { get; set; }
		public string PlannedStartTimeStr { get; set; }
		public string Product { get; set; }
		public string ProductDescription { get; set; }
		public string StartTimeStr { get; set; }
		public string StopTimeStr { get; set; }
		public string StartDensityProductObservedStr { get; set; }
		public string StartDensityProductObservedTimeStr { get; set; }
		public string StartDensityProductObservedInAirStr { get; set; }
		public string StartDensityProductStandardStr { get; set; }
		public string StartDensityProductStandardTimeStr { get; set; }
		public string StartUserID { get; set; }
		public string StartLevelProductStr { get; set; }
		public string StartLevelProductTimeStr { get; set; }
		public string StartLevelWaterStr { get; set; }
		public string StartLevelWaterTimeStr { get; set; }
		public string StartMassLiquidStr { get; set; }
        public string StartPercentBswStr { get; set; }
        public string StartTankShellCorrectionStr { get; set; }
		public string StartTemperatureAmbientStr { get; set; }
		public string StartTemperatureAmbientTimeStr { get; set; }
		public string StartTemperatureProductStr { get; set; }
		public string StartTemperatureProductTimeStr { get; set; }
		public string StartTemperatureDensityStr { get; set; }
		public string StartTemperatureDensityTimeStr { get; set; }
		public string StartVolumeStr { get; set; }
        public string StartVolumeBswStr { get; set; }
        public string StartVolumeCorrectionFactorStr { get; set; }
		public string StartVolumeGrossObservedStr { get; set; }
		public string StartVolumeGrossStandardStr { get; set; }
		public string StartVolumeNetStandardStr { get; set; }
		public string StartVolumeRoofCorrectionStr { get; set; }
		public string StartVolumeTotalObservedStr { get; set; }
		public string StartVolumeWaterStr { get; set; }
		public string UnitsLevelProduct { get; set; }
		public string UnitsTemperatureAmbient { get; set; }
		public string UnitsTemperatureDensity { get; set; }
		public string UnitsTemperatureProduct { get; set; }
		public string UnitsDensityProductObserved { get; set; }
		public string UnitsDensityProductStandard { get; set; }
		public string UnitsVolume { get; set; }
		public string UnitsMass { get; set; }
		public string UserData01 { get; set; }
		public string UserData02 { get; set; }
		public string UserData03 { get; set; }
		public string UserData04 { get; set; }
		public string UserData05 { get; set; }
		public string UserData06 { get; set; }
		public string UserData07 { get; set; }
		public string UserData08 { get; set; }
		public string UserData09 { get; set; }
		public string UserData10 { get; set; }
		public string TransferDeviationStr { get; set; }
        public string TransferPercentDeviationStr { get; set; }
        public string TransferModeStr { get; set; }
		public string TransferStatusStr { get; set; }
		public string TransferTargetStr { get; set; }
		public string TransferTargetUnits { get; set; }
		public string TransferLevelTargetStr { get; set; }
		public string TransferVolumeTargetStr { get; set; }
		public string TransferTimeRemainingStr { get; set; }
		public string TransferDirection { get; set; }
		public string CreatedDateStr { get; set; }
		public string CommentUserName { get; set; }
		public string CommentDateTimeStr { get; set; }
		public string StatusStr { get; set; }
		public string VolumeWaterStr { get; set; }
		public string LevelProductStr { get; set; }
		public string StartDensityProductStandardInAirStr { get; set; }
		public string TransferredVolumeWaterStr { get; set; }
		public string TransferredVolumeStr { get; set; }
		public Guid PointGuid { get; set; }
		public Guid RootParentGuid { get; set; }
		public int RecordSeq { get; set; }
		public string MinDateTimeStr { get; set; }
		public string MaxDateTimeStr { get; set; }
		public bool MidnightRecord { get; set; }
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
        {
			this.DT_RowId								= string.Empty;
			this.MovementHistoryGuid					= Guid.Empty;
			this.Name									= string.Empty;
			this.Node									= string.Empty;
			this.SiteId									= string.Empty;
			this.InitiationCount						= 0;
			this.RecordType								= 2;
			this.TimeStampStr							= string.Empty;
			this.ParentGuid								= Guid.Empty;
			this.AutoStart								= false;
			this.AutoStartTimeStr						= string.Empty;
			this.AutoStop								= false;
			this.AutoStopTimeStr						= string.Empty;
			this.CloseoutDataModifiedBy					= string.Empty;
			this.CloseoutDensityProductInAirStr			= string.Empty;
			this.CloseoutDensityProductObservedStr		= string.Empty;
			this.CloseoutDensityProductObservedTimeStr	= string.Empty;
			this.CloseoutDensityProductStandardStr		= string.Empty;
			this.CloseoutDensityProductStandardTimeStr	= string.Empty;
			this.CloseoutDensityProductStandardInAirStr = string.Empty;
			this.CloseoutLevelProductStr				= string.Empty;
			this.CloseoutLevelProductTimeStr			= string.Empty;
			this.CloseoutLevelWaterStr					= string.Empty;
			this.CloseoutMassLiquidStr					= string.Empty;
			this.CloseoutPercentBswStr					= string.Empty;
			this.CloseoutRoofMassStr					= string.Empty;
			this.CloseoutTankShellCorrectionStr			= string.Empty;
			this.CloseoutTemperatureAmbientStr			= string.Empty;
			this.CloseoutTemperatureAmbientTimeStr		= string.Empty;
			this.CloseoutTemperatureDensityStr			= string.Empty;
			this.CloseoutTemperatureProductStr			= string.Empty;
			this.CloseoutTimeStr						= string.Empty;
			this.CloseoutTransferGovStr					= string.Empty;
			this.CloseoutTransferNsvStr					= string.Empty;
			this.CloseoutTransferMassLiquidStr			= string.Empty;
			this.CloseoutTransferVolumeWaterStr			= string.Empty;
			this.CloseoutVolumeBswStr					= string.Empty;
			this.CloseoutVolumeCorrectionFactorStr		= string.Empty;
			this.CloseoutVolumeGrossObservedStr			= string.Empty;
			this.CloseoutVolumeGrossStandardStr			= string.Empty;
			this.CloseoutVolumeNetStandardStr			= string.Empty;
			this.CloseoutVolumeRoofCorrectionStr		= string.Empty;
			this.CloseoutVolumeTotalObservedStr			= string.Empty;
			this.CloseoutVolumeWaterStr					= string.Empty;
			this.Comment										= string.Empty;
			this.Type											= string.Empty;
			this.OrderNumber							= string.Empty;
			this.PlannedStartTimeStr					= string.Empty;
			this.Product								= string.Empty;
			this.ProductDescription						= string.Empty;
			this.StartTimeStr							= string.Empty;
			this.StopTimeStr							= string.Empty;
			this.StartDensityProductObservedStr			= string.Empty;
			this.StartDensityProductObservedTimeStr		= string.Empty;
			this.StartDensityProductObservedInAirStr	= string.Empty;
			this.StartDensityProductStandardStr			= string.Empty;
			this.StartDensityProductStandardTimeStr		= string.Empty;
			this.StartUserID							= string.Empty;
			this.StartLevelProductStr					= string.Empty;
			this.StartLevelProductTimeStr				= string.Empty;
			this.StartLevelWaterStr						= string.Empty;
			this.StartLevelWaterTimeStr					= string.Empty;
			this.StartMassLiquidStr						= string.Empty;
            this.StartPercentBswStr = string.Empty;
            this.StartTankShellCorrectionStr			= string.Empty;
			this.StartTemperatureAmbientStr				= string.Empty;
			this.StartTemperatureAmbientTimeStr			= string.Empty;
			this.StartTemperatureProductStr				= string.Empty;
			this.StartTemperatureProductTimeStr			= string.Empty;
			this.StartTemperatureDensityStr				= string.Empty;
			this.StartTemperatureDensityTimeStr			= string.Empty;
			this.StartVolumeStr							= string.Empty;
            this.StartVolumeBswStr						= string.Empty;
            this.StartVolumeCorrectionFactorStr			= string.Empty;
			this.StartVolumeGrossObservedStr			= string.Empty;
			this.StartVolumeGrossStandardStr			= string.Empty;
			this.StartVolumeNetStandardStr				= string.Empty;
			this.StartVolumeRoofCorrectionStr			= string.Empty;
			this.StartVolumeTotalObservedStr			= string.Empty;
			this.StartVolumeWaterStr					= string.Empty;
			this.UnitsLevelProduct						= string.Empty;
			this.UnitsTemperatureAmbient				= string.Empty;
			this.UnitsTemperatureDensity				= string.Empty;
			this.UnitsTemperatureProduct				= string.Empty;
			this.UnitsDensityProductObserved			= string.Empty;
			this.UnitsDensityProductStandard			= string.Empty;
			this.UnitsVolume							= string.Empty;
			this.UnitsMass								= string.Empty;
			this.UserData01								= string.Empty;
			this.UserData02								= string.Empty;
			this.UserData03								= string.Empty;
			this.UserData04								= string.Empty;
			this.UserData05								= string.Empty;
			this.UserData06								= string.Empty;
			this.UserData07								= string.Empty;
			this.UserData08								= string.Empty;
			this.UserData09								= string.Empty;
			this.UserData10								= string.Empty;
			this.TransferDeviationStr					= string.Empty;
         this.TransferPercentDeviationStr			= string.Empty;
         this.TransferModeStr							= string.Empty;
			this.TransferStatusStr						= string.Empty;
			this.TransferTargetStr						= string.Empty;
			this.TransferTargetUnits					= string.Empty;
			this.TransferLevelTargetStr				= string.Empty;
			this.TransferVolumeTargetStr				= string.Empty;
			this.TransferTimeRemainingStr				= string.Empty;
			this.TransferDirection						= string.Empty;
			this.CommentUserName							= string.Empty;
			this.CommentDateTimeStr						= string.Empty;
			this.StatusStr									= string.Empty;
			this.VolumeWaterStr							= string.Empty;
			this.LevelProductStr							= string.Empty;
			this.StartDensityProductStandardInAirStr	= string.Empty;
			this.TransferredVolumeWaterStr				= string.Empty;
			this.TransferredVolumeStr					= string.Empty;
			this.CreatedDateStr							= string.Empty;
			this.PointGuid									= Guid.Empty;
			this.RootParentGuid							= Guid.Empty;
			this.RecordSeq									= 0;
			this.MaxDateTimeStr							= string.Empty;
			this.MinDateTimeStr							= string.Empty;
			this.MidnightRecord							= false;
		}
        #endregion
    }

    [Serializable]
	public class MovementHistoryTabModel
	{
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementHistoryTabModel()
        {
			this.Init();
        }
        #endregion

        #region Properties
        public List<MovementHistoryTabRow> MovementHistories;
		public SiteClass Site { get; set; }
		public bool HasModifyMovementHistoryRight { get; set; }
		public bool HasViewMovementHistoryRight { get; set; }

		public bool HasMovementTicketReport { get; set; }
		public bool HasMovementTicketPrinter { get; set; }

		public MovementHistoryUserViewStateSettings ViewStateSettings { get; set; }
        #endregion

        #region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
        {
			this.HasModifyMovementHistoryRight	= false;
			this.HasViewMovementHistoryRight	= false;
			this.MovementHistories				= new List<MovementHistoryTabRow>();
			this.Site							= null;
        }
        #endregion
    }
}
