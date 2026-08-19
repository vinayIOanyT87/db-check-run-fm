using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;
    using CodedVariables;
    using Attributes;
    using System.Xml.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
    /// The TankVessel class encapulates all the necessary settings for a Tank Point Type
    /// </summary>
    [DataContract(Namespace = "")]
    [Serializable()]
    public class Vessel
    {
        const string TimeFormat = "yyyy-MM-ddTHH:mm:sszzz";

        public Vessel()
        {
            this.TankMaterial = TankMaterialEnum.MildCarbon;
            this.TankGeometry = TankGeometryEnum.VerticalCylinder;
            this.TankVolume = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuVolume);
            this.TankHeight = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuLength);
            this.TankRadius = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuLength);
            this.TankShellThickness = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuLength);
            this.TankLiningMaterial = string.Empty;
            this.TankInstallationDate = DateTime.Now.Date;
            this.TankExpansionCoefficient = new PointPropertyUnitTypedDouble(PointPropertyConstants.DefaultTankExpansionCoefficient, EngineeringUnitType.FmuNone);
            this.TankInstallationTemperature = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuTemp);
            this.CathodicProtectionSupported = false;
            this.OverfillProtectionSupported = false;
            this.SpillProtectionSupported = false;
            this.TankShellCorrectionEnabled = false;
            this.TankShellInsulated = false;
            this.AreaCoefficient = new PointPropertyUnitTypedDouble(PointPropertyConstants.DefaultAreaCoefficient, EngineeringUnitType.FmuNone);

            this.CSTManufacturerName = string.Empty;
            this.CSTManufactureDate = DateTime.Now.Date;
            this.CSTCapacity = new PointPropertyUnitTypedDouble(0.0, EngineeringUnitType.FmuVolume);
            this.CSTSerialNumber = string.Empty;

            this.CSTLocationName = string.Empty;
            this.CSTLatitude = null;
            this.CSTLongitude = null;
            this.CSTCommissionDate = DateTime.Now.Date;
        }
        #region Tank Characteristics
        [FMExposedSetting("Tank Geometry", ModifyDisabled = true)]
        public string TankGeometryEnumText
        { get { return this.TankGeometry.ToString(); } }

        [DataMember(Order = 0)]
        public TankGeometryEnum TankGeometry { get; set; }

        [FMExposedSetting("Tank Volume", ModifyDisabled = true)]
        [DataMember(Order = 1)]
        public PointPropertyUnitTypedDouble TankVolume { get; set; }

        [FMExposedSetting("Tank Height", ModifyDisabled = true)]
        [DataMember(Order = 2)]
        public PointPropertyUnitTypedDouble TankHeight { get; set; }

        [FMExposedSetting("Tank Radius", ModifyDisabled = true)]
        [DataMember(Order = 3)]
        public PointPropertyUnitTypedDouble TankRadius { get; set; }

        [FMExposedSetting("Tank Shell Thickness", ModifyDisabled = true)]
        [DataMember(Order = 4)]
        public PointPropertyUnitTypedDouble TankShellThickness { get; set; }

        [FMExposedSetting("Tank Lining Material", ModifyDisabled = true)]
        [DataMember(Order = 5)]
        public string TankLiningMaterial { get; set; }

        [DataMember(Order = 6)]
        [FMExposedSetting("Tank Installation Date")]
        [XmlIgnore]
        public DateTime TankInstallationDate { get; set; }

        [XmlElement("TankInstallationDate")]
        public string TankInstallationDateString { get { return TankInstallationDate.ToString(TimeFormat); } set { TankInstallationDate = DateTime.ParseExact(value, TimeFormat, null); } }

        [FMExposedSetting("Tank Material", ModifyDisabled = true)]
        public string TankMaterialEnumText
        { get { return this.TankMaterial.ToString(); } }

        [DataMember(Order = 7)]
        public TankMaterialEnum TankMaterial { get; set; }

        [FMExposedSetting("Tank Expansion Coefficient", ModifyDisabled = true)]
        [DataMember(Order = 8)]
        public PointPropertyUnitTypedDouble TankExpansionCoefficient { get; set; }

        [DataMember(Order = 9)]
        public PointPropertyUnitTypedDouble TankInstallationTemperature { get; set; }
        #endregion

        #region Tank Protection Options
        [FMExposedSetting("Cathodic Protection Supported", ModifyDisabled = true)]
        [DataMember(Order = 10)]
        public bool CathodicProtectionSupported { get; set; }
        [FMExposedSetting("Overfill Protection Supported", ModifyDisabled = true)]
        [DataMember(Order = 11)]
        public bool OverfillProtectionSupported { get; set; }
        [FMExposedSetting("Spill Protection Supported", ModifyDisabled = true)]
        [DataMember(Order = 12)]
        public bool SpillProtectionSupported { get; set; }


        #endregion

        #region Tank Shell Correction Options
        [DataMember(Order = 13)]
        public bool TankShellCorrectionEnabled { get; set; }

        [FMExposedSetting("Tank Shell Insulated", ModifyDisabled = true)]
        [DataMember(Order = 14)]
        public bool TankShellInsulated { get; set; }

        [FMExposedSetting("Area Coefficient", ModifyDisabled = true)]
        [DataMember(Order = 15)]
        public PointPropertyUnitTypedDouble AreaCoefficient { get; set; }

        #endregion

        #region Collapsible Storage Tank (CST) Data
        [FMExposedSetting("CST Manufacturer")]
        [DataMember(Order = 16)]
        public string CSTManufacturerName { get; set; }

        [DataMember(Order = 17)]
        [FMExposedSetting("CST Manufacture Date")]
        [XmlIgnore]
        public DateTime CSTManufactureDate { get; set; }

        [XmlElement("CSTManufactureDate")]
        public string CSTManufactureDateString { get { return CSTManufactureDate.ToString(TimeFormat); } set { CSTManufactureDate = DateTime.ParseExact(value, TimeFormat, null); } }

        [DataMember(Order = 18)]
        public PointPropertyUnitTypedDouble CSTCapacity { get; set; }

        [FMExposedSetting("CST Serial Number", ModifyDisabled = true)]
        [DataMember(Order = 19)]
        public string CSTSerialNumber { get; set; }

        [FMExposedSetting("CST Location", ModifyDisabled = true)]
        [DataMember(Order = 20)]
        public string CSTLocationName { get; set; }

        [FMExposedSetting("CST Latitude", ModifyDisabled = true)]
        [DataMember(Order = 21)]
        public Double? CSTLatitude { get; set; }

        [FMExposedSetting("CST Longitude", ModifyDisabled = true)]
        [DataMember(Order = 22)]
        public Double? CSTLongitude { get; set; }

        [FMExposedSetting("Latitude Degrees", ModifyDisabled = true)]
        public Double? LatitudeDegrees
        { get 
            {
                if (CSTLatitude == null)
                    return null;
                {
                    double coord = (double)CSTLatitude;
                    int sec = (int)Math.Round(coord * 3600);
                    int deg = sec / 3600;
                    return deg;
                }
            } 
        }

        [FMExposedSetting("Latitude Minutes", ModifyDisabled = true)]
        public Double? LatitudeMinutes
        { get 
            {
                if (CSTLatitude == null)
                    return null;
                {
                    double coord = (double)CSTLatitude;
                    int sec = (int)Math.Round(coord * 3600);
                    int deg = sec / 3600;
                    sec = Math.Abs(sec % 3600);
                    int min = sec / 60;
                    return min;
                }
            } 
        }

        [FMExposedSetting("Latitude Seconds", ModifyDisabled = true)]
        public Double? LatitudeSeconds
        { get 
            {
                if (CSTLatitude == null)
                    return null;
                {
                    double coord = (double)CSTLatitude;
                    double sec = (coord * 3600);
                    double deg = sec / 3600;
                    sec = Math.Abs(sec % 3600);
                    double min = sec / 60;
                    sec %= 60;
                    return sec;
                }
            } 
        }

        [DataMember(Order = 23)]
        [FMExposedSetting("CST Commission Date")]
        [XmlIgnore]
        public DateTime CSTCommissionDate { get; set; }

        [XmlElement("CSTCommissionDate")]
        public string CSTCommissionDateString { get { return CSTCommissionDate.ToString(TimeFormat); } set { CSTCommissionDate = DateTime.ParseExact(value, TimeFormat, null); } }
        #endregion

    }
}
