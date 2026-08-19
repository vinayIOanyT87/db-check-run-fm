namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using System;
    using System.Globalization;

    #region Main Model
    [Serializable]
    public class MovementHandgaugeModel
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MovementHandgaugeModel()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public Guid MovementHangaugeGuid { get; set; }
        public Guid PointGuid { get; set; }
        public Guid ParentGuid { get; set; }
        public Guid RootParentGuid { get; set; }
        public int RecordSeq { get; set; }
        public string NumberGroupSeparator { get; set; }
        public string NumberDecimalSeparator { get; set; }
        public int[] NumberGroupSizes { get; set; }
        public string ShortDatePattern { get; set; }
        public string TimePattern { get; set; }
        public string TimeZone { get; set; }
        public bool SaveToFinalRecord { get; set; }
        public bool HasModifyRights { get; set; }


        // Start values
        public string StartLevel { get; set; }
        public string StartLevelTime { get; set; }
        public string StartTemperature { get; set; }
        public string StartTemperatureTime { get; set; }
        public string StartDensity { get; set; }
        public string StartDensityTime { get; set; }
        public string StartStdDensity { get; set; }
        public string StartStdDensityTime { get; set; }
        public string StartDensityTemperature { get; set; }
        public string StartDensityTemperatureTime { get; set; }
        public string StartAmbientTemperature { get; set; }
        public string StartAmbientTemperatureTime { get; set; }
        public string StartWaterLevel { get; set; }
        public string StartWaterLevelTime { get; set; }
        public string StartRefHeight { get; set; }
        public string StartRefHeightTime { get; set; }
        public string StartVolumeTov { get; set; }
        public string StartGrossVolume { get; set; }
        public string StartNetVolume { get; set; }
        public string StartVolumeWater { get; set; }
        public string StartMass { get; set; }
        public string StartVcf { get; set; }
        public string StartCtsh { get; set; }
        public string StartEmployeeId { get; set; }
        public bool StartEnterIndTimestamps { get; set; }
        public string StartLevelUnits { get; set; }
        public string StartTemperatureUnits { get; set; }
        public string StartDensityUnits { get; set; }
        public string StartStandardDensityUnits { get; set; }
        public string StartDensityTemperatureUnits { get; set; }
        public string StartAmbientTemperatureUnits { get; set; }
        public string StartWaterLevelUnits { get; set; }
        public string StartRefHeightUnits { get; set; }
        public string StartVolumeUnits { get; set; }
        public string StartGrossVolumeUnits { get; set; }
        public string StartNetVolumeUnits { get; set; }
        public string StartWaterVolumeUnits { get; set; }
        public string StartMassUnits { get; set; }
        public int StartLevelUnitsInt { get; set; }
        public int StartTemperatureUnitsInt { get; set; }
        public int StartDensityUnitsInt { get; set; }
        public int StartStandardDensityUnitsInt { get; set; }
        public int StartDensityTemperatureUnitsInt { get; set; }
        public int StartAmbientTemperatureUnitsInt { get; set; }
        public int StartWaterLevelUnitsInt { get; set; }
        public int StartRefHeightUnitsInt { get; set; }
        public int StartVolumeUnitsInt { get; set; }
        public int StartGrossVolumeUnitsInt { get; set; }
        public int StartNetVolumeUnitsInt { get; set; }
        public int StartWaterVolumeUnitsInt { get; set; }
        public int StartMassUnitsInt { get; set; }

        // End values
        public string EndLevel { get; set; }
        public string EndLevelTime { get; set; }
        public string EndTemperature { get; set; }
        public string EndTemperatureTime { get; set; }
        public string EndDensity { get; set; }
        public string EndDensityTime { get; set; }
        public string EndStdDensity { get; set; }
        public string EndStdDensityTime { get; set; }
        public string EndDensityTemperature { get; set; }
        public string EndDensityTemperatureTime { get; set; }
        public string EndAmbientTemperature { get; set; }
        public string EndAmbientTemperatureTime { get; set; }
        public string EndWaterLevel { get; set; }
        public string EndWaterLevelTime { get; set; }
        public string EndRefHeight { get; set; }
        public string EndRefHeightTime { get; set; }
        public string EndVolumeTov { get; set; }
        public string EndGrossVolume { get; set; }
        public string EndNetVolume { get; set; }
        public string EndVolumeWater { get; set; }
        public string EndMass { get; set; }
        public string EndVcf { get; set; }
        public string EndCtsh { get; set; }
        public string EndEmployeeId { get; set; }
        public bool EndEnterIndTimestamps { get; set; }
        public string EndLevelUnits { get; set; }
        public string EndTemperatureUnits { get; set; }
        public string EndDensityUnits { get; set; }
        public string EndStandardDensityUnits { get; set; }
        public string EndDensityTemperatureUnits { get; set; }
        public string EndAmbientTemperatureUnits { get; set; }
        public string EndWaterLevelUnits { get; set; }
        public string EndRefHeightUnits { get; set; }
        public string EndVolumeUnits { get; set; }
        public string EndGrossVolumeUnits { get; set; }
        public string EndNetVolumeUnits { get; set; }
        public string EndWaterVolumeUnits { get; set; }
        public string EndMassUnits { get; set; }
        public int EndLevelUnitsInt { get; set; }
        public int EndTemperatureUnitsInt { get; set; }
        public int EndDensityUnitsInt { get; set; }
        public int EndStandardDensityUnitsInt { get; set; }
        public int EndDensityTemperatureUnitsInt { get; set; }
        public int EndAmbientTemperatureUnitsInt { get; set; }
        public int EndWaterLevelUnitsInt { get; set; }
        public int EndRefHeightUnitsInt { get; set; }
        public int EndVolumeUnitsInt { get; set; }
        public int EndGrossVolumeUnitsInt { get; set; }
        public int EndNetVolumeUnitsInt { get; set; }
        public int EndWaterVolumeUnitsInt { get; set; }
        public int EndMassUnitsInt { get; set; }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.MovementHangaugeGuid   = Guid.Empty;
            this.PointGuid              = Guid.Empty;
            this.ParentGuid             = Guid.Empty;
            this.RootParentGuid         = Guid.Empty;
            this.RecordSeq              = 0;
            this.NumberGroupSeparator   = string.Empty;
            this.NumberDecimalSeparator = string.Empty;
            this.NumberGroupSizes       = new int[1];
            this.ShortDatePattern       = string.Empty;
            this.TimePattern            = string.Empty;
            this.TimeZone               = string.Empty;
            this.SaveToFinalRecord      = true;
            this.HasModifyRights        = false;

            // Start values
            this.StartLevel                     = string.Empty;
            this.StartLevelTime                 = string.Empty;
            this.StartTemperature               = string.Empty;
            this.StartTemperatureTime           = string.Empty;
            this.StartDensity                   = string.Empty;
            this.StartDensityTime               = string.Empty;
            this.StartStdDensity                = string.Empty;
            this.StartStdDensityTime            = string.Empty;
            this.StartDensityTemperature        = string.Empty;
            this.StartDensityTemperatureTime    = string.Empty;
            this.StartAmbientTemperature        = string.Empty;
            this.StartAmbientTemperatureTime    = string.Empty;
            this.StartWaterLevel                = string.Empty;
            this.StartWaterLevelTime            = string.Empty;
            this.StartRefHeight                 = string.Empty;
            this.StartRefHeightTime             = string.Empty;
            this.StartVolumeTov                 = string.Empty;
            this.StartGrossVolume               = string.Empty;
            this.StartNetVolume                 = string.Empty;
            this.StartVolumeWater               = string.Empty;
            this.StartMass                      = string.Empty;
            this.StartVcf                       = string.Empty;
            this.StartCtsh                      = string.Empty;
            this.StartEmployeeId                = string.Empty;
            this.StartEnterIndTimestamps        = true;

            // End values
            this.EndLevel                   = string.Empty;
            this.EndLevelTime               = string.Empty;
            this.EndTemperature             = string.Empty;
            this.EndTemperatureTime         = string.Empty;
            this.EndDensity                 = string.Empty;
            this.EndDensityTime             = string.Empty;
            this.EndStdDensity              = string.Empty;
            this.EndStdDensityTime          = string.Empty;
            this.EndDensityTemperature      = string.Empty;
            this.EndDensityTemperatureTime  = string.Empty;
            this.EndAmbientTemperature      = string.Empty;
            this.EndAmbientTemperatureTime  = string.Empty;
            this.EndWaterLevel              = string.Empty;
            this.EndWaterLevelTime          = string.Empty;
            this.EndRefHeight               = string.Empty;
            this.EndRefHeightTime           = string.Empty;
            this.EndVolumeTov               = string.Empty;
            this.EndGrossVolume             = string.Empty;
            this.EndNetVolume               = string.Empty;
            this.EndVolumeWater             = string.Empty;
            this.EndMass                    = string.Empty;
            this.EndVcf                     = string.Empty;
            this.EndCtsh                    = string.Empty;
            this.EndEmployeeId              = string.Empty;
            this.EndEnterIndTimestamps      = true;

            // Start Units
            this.StartLevelUnits                = "ft-in-16th";
            this.StartTemperatureUnits          = "F";
            this.StartDensityUnits              = "API";
            this.StartStandardDensityUnits      = "API";
            this.StartDensityTemperatureUnits   = "F";
            this.StartAmbientTemperatureUnits   = "F";
            this.StartWaterLevelUnits           = "ft-in-16th";
            this.StartRefHeightUnits            = string.Empty;
            this.StartVolumeUnits               = "gal(US)";
            this.StartGrossVolumeUnits          = "gal(US)";
            this.StartNetVolumeUnits            = "gal(US)";
            this.StartWaterVolumeUnits          = "gal(US)";
            this.StartMassUnits                 = "lb";

            this.StartTemperatureUnitsInt           = 0;
            this.StartDensityUnitsInt               = 0;
            this.StartStandardDensityUnitsInt       = 0;
            this.StartDensityTemperatureUnitsInt    = 0;
            this.StartAmbientTemperatureUnitsInt    = 0;
            this.StartWaterLevelUnitsInt            = 0;
            this.StartRefHeightUnitsInt             = 0;
            this.StartVolumeUnitsInt                = 0;
            this.StartGrossVolumeUnitsInt           = 0;
            this.StartNetVolumeUnitsInt             = 0;
            this.StartWaterVolumeUnitsInt           = 0;
            this.StartMassUnitsInt                  = 0;

            // End Units
            this.EndLevelUnits              = "ft-in-16th";
            this.EndTemperatureUnits        = "F";
            this.EndDensityUnits            = "API";
            this.EndStandardDensityUnits    = "API";
            this.EndDensityTemperatureUnits = "F";
            this.EndAmbientTemperatureUnits = "F";
            this.EndWaterLevelUnits         = "ft-in-16th";
            this.EndRefHeightUnits          = string.Empty;
            this.EndVolumeUnits             = "gal(US)";
            this.EndGrossVolumeUnits        = "gal(US)";
            this.EndNetVolumeUnits          = "gal(US)";
            this.EndWaterVolumeUnits        = "gal(US)";
            this.EndMassUnits               = "lb";

            this.EndTemperatureUnitsInt         = 0;
            this.EndDensityUnitsInt             = 0;
            this.EndStandardDensityUnitsInt     = 0;
            this.EndDensityTemperatureUnitsInt  = 0;
            this.EndAmbientTemperatureUnitsInt  = 0;
            this.EndWaterLevelUnitsInt          = 0;
            this.EndRefHeightUnitsInt           = 0;
            this.EndVolumeUnitsInt              = 0;
            this.EndGrossVolumeUnitsInt         = 0;
            this.EndNetVolumeUnitsInt           = 0;
            this.EndWaterVolumeUnitsInt         = 0;
            this.EndMassUnitsInt                = 0;
        }
        #endregion
    }
    #endregion
}