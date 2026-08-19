namespace FMBusinessObjects.DataObjects
{
	using System.Collections.Generic;

	public class MovementHistoryTabColumnFilterInfo
	{
		#region Public members
		public enum ColumnFilterNameEnums
		{
			TimeStamp,
			Name,
			Node,
			InitiationCount,
			Site,
			Comment,
			CloseoutDataModifiedBy,
            CloseoutDensityProductInAir,
            CloseoutDensityProductObserved,
            CloseoutDensityProductObservedTime,
            CloseoutDensityProductStandard,
            CloseoutDensityProductStandardTime,
            CloseoutDensityProductStandardInAir,
            CloseoutLevelProduct,
            CloseoutLevelProductTime,
            CloseoutLevelWater,
            CloseoutMassLiquid,
            CloseoutPercentBsw,
            CloseoutRoofMass,
            CloseoutTankShellCorrection,
            CloseoutTemperatureAmbient,
            CloseoutTemperatureAmbientTime,
            CloseoutTemperatureDensity,
            CloseoutTemperatureProduct,
            CloseoutTime,
            CloseoutTransferGov,
            CloseoutTransferNsv,
            CloseoutTransferMassLiquid,
            CloseoutTransferVolumeWater,
            CloseoutVolumeBsw,
            CloseoutVolumeCorrectionFactor,
            CloseoutVolumeGrossObserved,
            CloseoutVolumeGrossStandard,
            CloseoutVolumeNetStandard,
            CloseoutVolumeRoofCorrection,
            CloseoutVolumeTotalObserved,
            CloseoutVolumeWater,
            LevelProduct,
            Type,
            OrderNumber,
            PlannedStartTime,
            Product,
            ProductDescription,
            StartTime,
            StartDensityProductObserved,
            StartDensityProductObservedTime,
            StartDensityProductObservedInAir,
            StartDensityProductStandard,
            StartDensityProductStandardTime,
            StartDensityProductStandardInAir,
            StartUserId,
            StartLevelProduct,
            StartLevelProductTime,
            StartLevelWater,
            StartLevelWaterTime,
            StartMassLiquid,
            StartPercentBsw,
            StartTankShellCorrection,
            StartTemperatureAmbient,
            StartTemperatureAmbientTime,
            StartTemperatureProduct,
            StartTemperatureProductTime,
            StartTemperatureDensity,
            StartTemperatureDensityTime,
            StartVolume,
            StartVolumeBsw,
            StartVolumeCorrectionFactor,
            StartVolumeGrossObserved,
            StartVolumeGrossStandard,
            StartVolumeNetStandard,
            StartVolumeRoofCorrection,
            StartVolumeTotalObserved,
            StartVolumeWater,
            StopTime,
            Status,
            TransferDeviation,
            TransferPercentDeviation,
            TransferDirection,
            TransferMode,
            TransferStatus,
            TransferTarget,
				TransferTargetUnits,
				TransferLevelTarget,
				TransferVolumeTarget,
            TransferTimeRemaining,
            TransferredVolumeWater,
            TransferredVolume,
            UnitsLevelProduct,
            UnitsTemperatureAmbient,
            UnitsTemperatureDensity,
            UnitsTemperatureProduct,
            UnitsDensityProductObserved,
            UnitsDensityProductStandard,
            UnitsVolume,
            UnitsMass,
            UserData01,
            UserData02,
            UserData03,
            UserData04,
            UserData05,
            UserData06,
            UserData07,
            UserData08,
            UserData09,
            UserData10,
            VolumeWater,
            CommentUserName,
            CommentDateTime,
            RecordType,
            MidnightRecord,
            None = -999
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementHistoryTabColumnFilterInfo()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string Name { get; set; }
		public int Index { get; set; }
		public List<string> FilterCollection { get; set; }
		public string FromDateStr { get; set; }
		public string ToDateStr { get; set; }
		public string CommentFromDateStr { get; set; }
		public string CommentToDateStr { get; set; }
        public bool ShowAutoGauge { get; set; }
        public bool ShowHandGauge { get; set; }
        public bool ShowMidnightRecord { get; set; }
        public ColumnFilterNameEnums SelectedColumnFilterEnum
		{
			get { return (ColumnFilterNameEnums)this.Index; }
			set { this.Index = (int)value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Name               = string.Empty;
			this.Index              = -99;
			this.FilterCollection   = new List<string>();
			this.FromDateStr        = string.Empty;
			this.ToDateStr          = string.Empty;
			this.CommentFromDateStr = string.Empty;
			this.CommentToDateStr   = string.Empty;
            this.ShowAutoGauge      = false;
            this.ShowHandGauge      = false;
            this.ShowMidnightRecord = false;
        }
		#endregion
	}
}

