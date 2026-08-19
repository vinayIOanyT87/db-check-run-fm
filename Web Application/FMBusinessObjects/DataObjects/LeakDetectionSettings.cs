namespace FMBusinessObjects.DataObjects
{
    using FMBusinessObjects.Attributes;
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    public enum LeakAnalysisMethod
    {
        NetVolume = 0,
        UnroundedNet = 1,
        Hydrostatic = 2   // Barton Series 3500
    };

    public enum LeakAnalysisType
    {
        Static = 0,
        Continuous = 1,
        RealTime = 2
    };



    [DataContract(Namespace = "")]
    [Serializable()]
    public class LeakDetectionSettings
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public LeakDetectionSettings()
        {
            this.Init();
        }
        #endregion


        #region Properties
        [DataMember(Order = 0)]
        public LeakAnalysisMethod AnalysisMethod { get; set; }

        [DataMember(Order = 1)]
        public LeakAnalysisType AnalysisType { get; set; }

        [FMExposedSetting("Leak Analysis Method", ModifyDisabled = true)]
        public string LeakAnalysisMethodString
        { get { return this.AnalysisMethod.ToString(); } }

        [FMExposedSetting("Leak Analysis Type", ModifyDisabled = true)]
        public string LeakAnalysisTypeString
        { get { return this.AnalysisType.ToString(); } }

        [FMExposedSetting("Gauge Type", ModifyDisabled = true)]
        [DataMember(Order = 2)]
        public string GaugeType { get; set; }

        [DataMember(Order = 3)]
        public bool AutoPrint { get; set; }

        [DataMember(Order = 4)]
        public int PrintDaysBeforeEOM
        {
            get
            {
                return _printDaysBeforeEOM;
            }
            set
            {
                if (value >= 0 && value <= 30)
                    _printDaysBeforeEOM = value;
                else
                    _printDaysBeforeEOM = 0;
            }
        }

        [DataMember(Order = 5)]
        [XmlIgnore]
        public DateTime PrintTime { get; set; }

        [XmlElement("PrintTime")]
        public string PrintTimeString { get { return PrintTime.ToString(TimeFormat); } set { PrintTime = string.IsNullOrWhiteSpace(value) ? DateTime.MinValue : DateTime.ParseExact(value, TimeFormat, null); } }

        [DataMember(Order = 6)]
        public int MinimumFillPercentage { get; set; }
     
        public static string GetLeakAnalysisMethodDisplayName(LeakAnalysisMethod analysisMethod)
        {
            switch (analysisMethod)
            {
                case LeakAnalysisMethod.UnroundedNet: return "Unrounded Net";
                case LeakAnalysisMethod.NetVolume: return "Net Volume";
                case LeakAnalysisMethod.Hydrostatic: return "Hydrostatic (Barton)";
                default: return null;
            }

        }

        
        public static string GetLeakAnalysisTypeDisplayName(LeakAnalysisType analysisType)
        {
            switch (analysisType)
            {
                case LeakAnalysisType.RealTime: return "Real Time";
                case LeakAnalysisType.Static: return "Static";
                case LeakAnalysisType.Continuous: return "Continuous";
                default: return null;
            }
        }

        #endregion
        [XmlIgnore]
        public const string LeakDetectionSettingsIdentifier =  "FMBusinessObjects.DataObjects.LeakDetectionSettings";
        #region Private methods
        [XmlIgnore]
        private int _printDaysBeforeEOM = 0;

        [XmlIgnore]
        const string TimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
        // <summary>
        // This method initializes the object to its initial state.
        // </summary>
        private void Init()
        {
            GaugeType = "Generic";
        }
        #endregion
    }
}
