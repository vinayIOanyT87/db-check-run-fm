namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
   using FMBusinessObjects.DataObjects.CodedVariables;
   using System;
	using System.Globalization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class MovementNodeStartDataEditorModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementNodeStartDataEditorModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid MovementPointGuid { get; set; }
		public Guid MovementNodePointGuid { get; set; }
		public string TransferStartTime { get; set; }
		public string PointId { get; set; }
		public string NodeId { get; set; }
		public NumberFormatInfo NumberFormatInfo { get; set; }
		public string NumberGroupSeparator { get; set; }
		public string NumberDecimalSeparator { get; set; }
		public int[] NumberGroupSizes { get; set; }
		public string ShortDatePattern { get; set; }
		public string TimePattern { get; set; }
		public string TimeZone { get; set; }

      public double? Level { get; set; }
      public string LevelFmtStr { get; set; }
      public EngineeringUnit LevelUnits { get; set; }
      public string LevelUnitsStr { get; set; }
      public short LevelPrecision { get; set; }

      public double? Temperature { get; set; }
      public string TemperatureFmtStr { get; set; }
      public EngineeringUnit TemperatureUnits { get; set; }
      public string TemperatureUnitsStr { get; set; }
      public short TemperaturePrecision { get; set; }

      public double? GrossVolume { get; set; }
      public string GrossVolumeFmtStr { get; set; }
      public EngineeringUnit GrossVolumeUnits { get; set; }
      public string GrossVolumeUnitsStr { get; set; }
      public short GrossVolumePrecision { get; set; }

      public double? NetVolume { get; set; }
      public string NetVolumeFmtStr { get; set; }
      public EngineeringUnit NetVolumeUnits { get; set; }
      public string NetVolumeUnitsStr { get; set; }
      public short NetVolumePrecision { get; set; }

      public double? Mass { get; set; }
      public string MassFmtStr { get; set; }
      public EngineeringUnit MassUnits { get; set; }
      public string MassUnitsStr { get; set; }
      public short MassPrecision { get; set; }

      public double? Density { get; set; }
      public string DensityFmtStr { get; set; }
      public EngineeringUnit DensityUnits { get; set; }
      public string DensityUnitsStr { get; set; }
      public short DensityPrecision { get; set; }

      public double? StdDensity { get; set; }
      public string StdDensityFmtStr { get; set; }
      public EngineeringUnit StdDensityUnits { get; set; }
      public string StdDensityUnitsStr { get; set; }
      public short StdDensityPrecision { get; set; }

      public bool IsVolumeTransferNode {get; set; }

		public TransferStatuses TransferStatus { get; set; }
      #endregion


      #region Private methods
      /// <summary>
      /// This method initializes the object to its initial state.
      /// </summary>
      private void Init()
		{
			this.MovementPointGuid = Guid.Empty;
			this.MovementNodePointGuid = Guid.Empty;
			this.TransferStartTime = string.Empty;
			this.PointId = string.Empty;
			this.NumberGroupSeparator = string.Empty;
			this.NumberDecimalSeparator = string.Empty;
			this.NumberGroupSizes = new int[1];
			this.ShortDatePattern = string.Empty;
			this.TimePattern = string.Empty;
			this.TimeZone = string.Empty;
			this.Level = 0.0;
			this.LevelUnits = EngineeringUnit.FmuNone;
			this.Temperature = 0.0;
			this.TemperatureUnits = EngineeringUnit.FmuNone;
			this.GrossVolume = 0.0;
			this.GrossVolumeUnits = EngineeringUnit.FmuNone;
			this.NetVolume = 0.0;
			this.NetVolumeUnits = EngineeringUnit.FmuNone;
			this.Mass = 0.0;
			this.MassUnits = EngineeringUnit.FmuNone;
			this.Density = 0.0;
			this.DensityUnits = EngineeringUnit.FmuNone;
			this.StdDensity = 0.0;
			this.StdDensityUnits = EngineeringUnit.FmuNone;
		}
		#endregion
	}
}