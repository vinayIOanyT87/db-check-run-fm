namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
   using FMBusinessObjects.DataObjects.CodedVariables;
   using System;
	using System.Globalization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable]
	public class MovementHistoryNodeEditorModel
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementHistoryNodeEditorModel()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid MovementHistoryGuid { get; set; }
		public Guid MovementPointGuid { get; set; }
		public Guid RootParentGuid { get; set; }
		public Guid ParentGuid { get; set; }
		public int CallingTypes { get; set; }
		public string StartOrClosoutTime { get; set; }
		public string PointId { get; set; }
		public string NodeId { get; set; }
		public NumberFormatInfo NumberFormatInfo { get; set; }
		public string NumberGroupSeparator { get; set; }
		public string NumberDecimalSeparator { get; set; }
		public int[] NumberGroupSizes { get; set; }
		public string ShortDatePattern { get; set; }
		public string TimePattern { get; set; }
		public string TimeZone { get; set; }

		public string LevelStr { get; set; }
		public string TemperatureStr { get; set; }
		public string GrossVolumeStr { get; set; }
		public string NetVolumeStr { get; set; }
		public string MassStr { get; set; }
		public string DensityStr { get; set; }
		public string StdDensityStr { get; set; }
		public string WaterLevelStr { get; set; }
		public string BswStr { get; set; }
		public string AmbientTemperatureStr { get; set; }

      public string LevelRawStr { get; set; }
      public string TemperatureRawStr { get; set; }
      public string GrossVolumeRawStr { get; set; }
      public string NetVolumeRawStr { get; set; }
      public string MassRawStr { get; set; }
      public string DensityRawStr { get; set; }
      public string StdDensityRawStr { get; set; }
      public string WaterLevelRawStr { get; set; }
      public string BswRawStr { get; set; }
      public string AmbientTemperatureRawStr { get; set; }

		public bool ArchiveDataMode { get; set; } = false;

      public bool IgnoreCalculation { get; set; }

		public EngineeringUnit LevelUnits { get; set; }
		public EngineeringUnit TemperatureUnits { get; set; }
		public EngineeringUnit StdDensityUnits { get; set; }
		public EngineeringUnit DensityUnits { get; set; }
		public EngineeringUnit NetVolumeUnits { get; set; }
		public EngineeringUnit MassUnits { get; set; }
		public EngineeringUnit GrossVolumeUnits { get; set; }
		public EngineeringUnit AmbientTemperatureUnits { get; set; }

		public string LevelUnitsStr { get; set; }
		public string TemperatureUnitsStr { get; set; }
		public string StdDensityUnitsStr { get; set; }
		public string DensityUnitsStr { get; set; }
		public string NetVolumeUnitsStr { get; set; }
		public string MassUnitsStr { get; set; }
		public string GrossVolumeUnitsStr { get; set; }
		public string AmbientTemperatureUnitsStr { get; set; }
      public TransferStatuses TransferStatus { get; set; }
      public MovementStatus MovementStatus { get; set; }

      public short LevelPrecision { get; set; }
      public short TemperaturePrecision { get; set; }
      public short StdDensityPrecision { get; set; }
      public short DensityPrecision { get; set; }
      public short NetVolumePrecision { get; set; }
      public short MassPrecision { get; set; }
      public short GrossVolumePrecision { get; set; }
      public short AmbientTemperaturePrecision { get; set; }

      public bool HasModifyRights { get; set; }
		#endregion


		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.MovementPointGuid		= Guid.Empty;
			this.MovementHistoryGuid	= Guid.Empty;
			this.RootParentGuid			= Guid.Empty;
			this.ParentGuid				= Guid.Empty;
			this.CallingTypes			= 3;
			this.StartOrClosoutTime		= string.Empty;
			this.PointId				= string.Empty;
			this.NodeId					= string.Empty;
			this.NumberGroupSeparator	= string.Empty;
			this.NumberDecimalSeparator = string.Empty;
			this.NumberGroupSizes		= new int[1];
			this.ShortDatePattern		= string.Empty;
			this.TimePattern			= string.Empty;
			this.TimeZone				= string.Empty;
			this.LevelStr				= string.Empty;
			this.TemperatureStr			= string.Empty;
			this.GrossVolumeStr			= string.Empty;
			this.NetVolumeStr			= string.Empty;
			this.MassStr				= string.Empty;
			this.DensityStr				= string.Empty;
			this.StdDensityStr			= string.Empty;
			this.WaterLevelStr			= string.Empty;
			this.BswStr					= string.Empty;
			this.AmbientTemperatureStr	= string.Empty;
			this.IgnoreCalculation		= false;

			this.AmbientTemperatureUnits	= EngineeringUnit.FmuNone;
			this.StdDensityUnits			= EngineeringUnit.FmuNone;
			this.DensityUnits				= EngineeringUnit.FmuNone;
			this.MassUnits					= EngineeringUnit.FmuNone;
			this.NetVolumeUnits				= EngineeringUnit.FmuNone;
			this.GrossVolumeUnits			= EngineeringUnit.FmuNone;
			this.TemperatureUnits			= EngineeringUnit.FmuNone;
			this.LevelUnits					= EngineeringUnit.FmuNone;

			this.LevelUnitsStr				= string.Empty;
			this.TemperatureUnitsStr		= string.Empty;
			this.StdDensityUnitsStr			= string.Empty;
			this.DensityUnitsStr			= string.Empty;
			this.NetVolumeUnitsStr			= string.Empty;
			this.MassUnitsStr				= string.Empty;
			this.GrossVolumeUnitsStr		= string.Empty;
			this.AmbientTemperatureUnitsStr = string.Empty;

			this.HasModifyRights			= false;
			#endregion
		}
	}
}