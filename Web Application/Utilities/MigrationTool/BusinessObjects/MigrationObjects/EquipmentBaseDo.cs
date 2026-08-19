namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class EquipmentBaseDo : MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public EquipmentBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public EquipmentBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int SiteIndex { get; set; }
        public string SiteId { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int EqTypeIndex { get; set; }
        public string EqTypeName { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public string IssPtNum { get; set; }
        public int? CompanyIndex { get; set; }
        public string CompanyId { get; set; }
        public bool Fixed { get; set; }
        public string StorageType { get; set; }
        public bool InUse { get; set; }
        public int? ProductIndex { get; set; }
        public string ProductId { get; set; }
        public int? FuelCardIndex { get; set; }
        public string FuelCardId { get; set; }
        public bool FixedVolume { get; set; }
        public bool IntoPlane { get; set; }
        public bool Mobile { get; set; }
        public string AttachedTo { get; set; }
        public string MediaType { get; set; }
        public int? Meters { get; set; }
        public bool DefuelMeterForwards { get; set; }
        public double? PulseRatio { get; set; }
        public bool Round { get; set; }
        public string Xref { get; set; }
        public double? LowStockWarning { get; set; }
        public bool StockTrack { get; set; }
        public string Totalisor1 { get; set; }
        public string Totalisor2 { get; set; }
        public string FuelingState { get; set; }
        public double? Volume { get; set; }
        public double? MeterReading { get; set; }
        public int? ConsectiveOosVariance { get; set; }
        public string Notes { get; set; }
        public double? Capacity { get; set; }
        public double? SafeFill { get; set; }
        public int? VolumeUnitIndex { get; set; }
        public int? TemperatureUnitIndex { get; set; }
        public int? DensityUnitIndex { get; set; }
        public int? MassUnitIndex { get; set; }
        public byte? VolumeDecimalPlaces { get; set; }
        public byte? TemperatureDecimalPlaces { get; set; }
        public byte? DensityDecimalPlaces { get; set; }
        public byte? MassDecimalPlaces { get; set; }
        public int? EquipmentIndex { get; set; }
        public string EquipmentSequence { get; set; }
        public bool LockedOut { get; set; }
        public string LockedOutReason { get; set; }
        public DateTime? LockedOutDate { get; set; }
        public string SerialNumber { get; set; }
        public string CompanyEquipmentId { get; set; }
        public string TruckCardNumber { get; set; }
        public double? RatedGpm { get; set; }
        public double? ActualGpm { get; set; }
        public bool FuelAdditiveFlag { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? InstallationDate { get; set; }
        public DateTime? InspectionDate { get; set; }
        public DateTime? CalibrationDate { get; set; }
        public DateTime? QcDate { get; set; }
        public bool SecondaryStorageFlag { get; set; }
        public bool ManagedEquipmentFlag { get; set; }
        public short? FuelingType { get; set; }
        public bool ScullyRequired { get; set; }
        public string UserData1 { get; set; }
        public string UserData2 { get; set; }
        public string UserData3 { get; set; }
        public string UserData4 { get; set; }
        public string UserData5 { get; set; }
        public string UserData6 { get; set; }
        public string UserData7 { get; set; }
        public string UserData8 { get; set; }
        public string UserData9 { get; set; }
        public string UserData10 { get; set; }
        public string UserData11 { get; set; }
        public string UserData12 { get; set; }
        public string UserData13 { get; set; }
        public string UserData14 { get; set; }
        public string UserData15 { get; set; }
        public string UserData16 { get; set; }
        public string UserData17 { get; set; }
        public string UserData18 { get; set; }
        public string UserData19 { get; set; }
        public string UserData20 { get; set; }
        public string UserData21 { get; set; }
        public string UserData22 { get; set; }
        public string UserData23 { get; set; }
        public string UserData24 { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method populates the SQL command to retrieve all the equipment that are 
        /// not compartments.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        /// <param name="siteIndex">The source site index.</param>
        public virtual void EnumerateEquipmentSql(SqlCommand command, int siteIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT E.*"
                            + ", S.ID AS SiteID"
                            + ", ET.EqTypeName"
                            + ", C.ID AS CompanyID"
                            + ", P.ProductID"
                            + ", F.ID AS FuelCardID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblEquipment E"
                            + " INNER JOIN " + this.SourceDbName + ".dbo.tblEquipmentTypes ET ON ET.EqTypeIndex = E.EqTypeIndex "
                            + " INNER JOIN " + this.SourceDbName + ".dbo.tblSites S ON E.SiteIndex = S.SiteIndex "
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblCompanies C ON C.CompanyIndex = E.CompanyIndex"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblProducts P ON P.ProductIndex = E.ProductIndex"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblFuelCards F ON F.FuelCardIndex = E.FuelCardIndex";

            string where = " WHERE E.SiteIndex = " + siteIndex
                            + " AND E.EqTypeIndex <> 5";


            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method populates the SQL command to retrieve all the equipment that are compartments.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        /// <param name="siteIndex">The source site index.</param>
        public virtual void EnumerateEquipmentCompartments(SqlCommand command, int siteIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT E.*"
                            + ", S.ID AS SiteID"
                            + ", ET.EqTypeName"
                            + ", C.ID AS CompanyID"
                            + ", P.ProductID"
                            + ", F.ID AS FuelCardID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblEquipment E"
                            + " INNER JOIN " + this.SourceDbName + ".dbo.tblEquipmentTypes ET ON ET.EqTypeIndex = E.EqTypeIndex "
                            + " INNER JOIN " + this.SourceDbName + ".dbo.tblSites S ON E.SiteIndex = S.SiteIndex "
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblCompanies C ON C.CompanyIndex = E.CompanyIndex"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblProducts P ON P.ProductIndex = E.ProductIndex"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblFuelCards F ON F.FuelCardIndex = E.FuelCardIndex";

            string where = " WHERE E.SiteIndex = " + siteIndex + " AND E.EqTypeIndex = 5";
            string orderBy = " ORDER BY E.EquipmentIndex, E.EquipmentSequence ";


            command.CommandText = select + from + where + orderBy;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Index                      = row.IsNull("Index") ? -99 : (int)row["Index"];
            this.SiteIndex                  = row.IsNull("SiteIndex") ? -99 : (int)row["SiteIndex"];
            this.SiteId                     = row.IsNull("SiteID") ? string.Empty : (string)row["SiteID"];
            this.Id                         = row.IsNull("ID") ? string.Empty : (string)row["ID"];
            this.Description                = row.IsNull("Description") ? string.Empty : (string)row["Description"];
            this.EqTypeIndex                = row.IsNull("EqTypeIndex") ? -99 : (int)row["EqTypeIndex"];
            this.EqTypeName                 = row.IsNull("EqTypeName") ? string.Empty : (string)row["EqTypeName"];
            this.Make                       = row.IsNull("Make") ? string.Empty : (string)row["Make"];
            this.Model                      = row.IsNull("Model") ? string.Empty : (string)row["Model"];
            this.Year                       = row.IsNull("Year") ? null : (int?)row["Year"];
            this.IssPtNum                   = row.IsNull("IssPtNum") ? string.Empty : (string)row["IssPtNum"];
            this.CompanyIndex               = row.IsNull("CompanyIndex") ? null : (int?)row["CompanyIndex"];
            this.CompanyId                  = row.IsNull("CompanyID") ? string.Empty : (string)row["CompanyID"];
            this.Fixed                      = row.IsNull("Fixed") ? false : (bool)row["Fixed"];
            this.StorageType                = row.IsNull("StorageType") ? string.Empty : (string)row["StorageType"];
            this.InUse                      = row.IsNull("InUse") ? false : (bool)row["InUse"];
            this.ProductIndex               = row.IsNull("ProductIndex") ? null : (int?)row["ProductIndex"];
            this.ProductId                  = row.IsNull("ProductID") ? string.Empty : (string)row["ProductID"];
            this.FuelCardIndex              = row.IsNull("FuelCardIndex") ? null : (int?)row["FuelCardIndex"];
            this.FuelCardId                 = row.IsNull("FuelCardID") ? string.Empty : (string)row["FuelCardID"];
            this.FixedVolume                = row.IsNull("FixedVolume") ? false : (bool)row["FixedVolume"];
            this.IntoPlane                  = row.IsNull("IntoPlane") ? false : (bool)row["IntoPlane"];
            this.Mobile                     = row.IsNull("Mobile") ? false : (bool)row["Mobile"];
            this.AttachedTo                 = row.IsNull("AttachedTo") ? string.Empty : (string)row["AttachedTo"];
            this.MediaType                  = row.IsNull("MediaType") ? string.Empty : (string)row["MediaType"];
            this.Meters                     = row.IsNull("Meters") ? null : (int?)row["Meters"];
            this.DefuelMeterForwards        = row.IsNull("DefuelMeterForwards") ? false : (bool)row["DefuelMeterForwards"];
            this.PulseRatio                 = row.IsNull("PulseRatio") ? null : (double?)row["PulseRatio"];
            this.Round                      = row.IsNull("Round") ? false : (bool)row["Round"];
            this.Xref                       = row.IsNull("Xref") ? string.Empty : (string)row["Xref"];
            this.LowStockWarning            = row.IsNull("LowStockWarning") ? null : (double?)row["LowStockWarning"];
            this.StockTrack                 = row.IsNull("StockTrack") ? false : (bool)row["StockTrack"];
            this.Totalisor1                 = row.IsNull("Totalisor1") ? string.Empty : (string)row["Totalisor1"];
            this.Totalisor2                 = row.IsNull("Totalisor2") ? string.Empty : (string)row["Totalisor2"];
            this.FuelingState               = row.IsNull("FuelingState") ? string.Empty : (string)row["FuelingState"];
            this.Volume                     = row.IsNull("Volume") ? null : (double?)row["Volume"];
            this.MeterReading               = row.IsNull("MeterReading") ? null : (double?)row["MeterReading"];
            this.ConsectiveOosVariance      = row.IsNull("Consecutive_OOS_Variance") ? null : (int?)row["Consecutive_OOS_Variance"];
            this.Notes                      = row.IsNull("Notes") ? string.Empty : (string)row["Notes"];
            this.Capacity                   = row.IsNull("Capacity") ? null : (double?)row["Capacity"];
            this.SafeFill                   = row.IsNull("SafeFill") ? null : (double?)row["SafeFill"];
            this.VolumeUnitIndex            = row.IsNull("VolumeUnitIndex") ? null : (int?)row["VolumeUnitIndex"];
            this.TemperatureUnitIndex       = row.IsNull("TemperatureUnitIndex") ? null : (int?)row["TemperatureUnitIndex"];
            this.DensityUnitIndex           = row.IsNull("DensityUnitIndex") ? null : (int?)row["DensityUnitIndex"];
            this.MassUnitIndex              = row.IsNull("MassUnitIndex") ? null : (int?)row["MassUnitIndex"];
            this.VolumeDecimalPlaces        = row.IsNull("VolumeDecimalPlaces") ? null : (byte?)row["VolumeDecimalPlaces"];
            this.TemperatureDecimalPlaces   = row.IsNull("TemperatureDecimalPlaces") ? null : (byte?)row["TemperatureDecimalPlaces"];
            this.DensityDecimalPlaces       = row.IsNull("DensityDecimalPlaces") ? null : (byte?)row["DensityDecimalPlaces"];
            this.MassDecimalPlaces          = row.IsNull("MassDecimalPlaces") ? null : (byte?)row["MassDecimalPlaces"];
            this.EquipmentIndex             = row.IsNull("EquipmentIndex") ? null : (int?)row["EquipmentIndex"];
            this.EquipmentSequence          = row.IsNull("EquipmentSequence") ? string.Empty : (string)row["EquipmentSequence"];
            this.LockedOut                  = row.IsNull("LockedOut") ? false : (bool)row["LockedOut"];
            this.LockedOutReason            = row.IsNull("LockedOutReason") ? string.Empty : (string)row["LockedOutReason"];
            this.LockedOutDate              = row.IsNull("LockedOutDate") ? null : (DateTime?)row["LockedOutDate"];
            this.SerialNumber               = row.IsNull("SerialNumber") ? string.Empty : (string)row["SerialNumber"];
            this.CompanyEquipmentId         = row.IsNull("CompanyEquipmentID") ? string.Empty : (string)row["CompanyEquipmentID"];
            this.TruckCardNumber            = row.IsNull("TruckCardNumber") ? string.Empty : (string)row["TruckCardNumber"];
            this.RatedGpm                   = row.IsNull("RatedGPM") ? null : (double?)row["RatedGPM"];
            this.ActualGpm                  = row.IsNull("ActualGPM") ? null : (double?)row["ActualGPM"];
            this.FuelAdditiveFlag           = row.IsNull("FuelAdditiveFlag") ? false : (bool)row["FuelAdditiveFlag"];
            this.ManufactureDate            = row.IsNull("ManufactureDate") ? null : (DateTime?)row["ManufactureDate"];
            this.InstallationDate           = row.IsNull("InstallationDate") ? null : (DateTime?)row["InstallationDate"];
            this.InspectionDate             = row.IsNull("InspectionDate") ? null : (DateTime?)row["InspectionDate"];
            this.CalibrationDate            = row.IsNull("CalibrationDate") ? null : (DateTime?)row["CalibrationDate"];
            this.QcDate                     = row.IsNull("QCDate") ? null : (DateTime?)row["QCDate"];
            this.SecondaryStorageFlag       = row.IsNull("SecondaryStorageFlag") ? false : (bool)row["SecondaryStorageFlag"];
            this.ManagedEquipmentFlag       = row.IsNull("ManagedEquipmentFlag") ? false : (bool)row["ManagedEquipmentFlag"];
            this.FuelingType                = row.IsNull("FuelingType") ? null : (short?)row["FuelingType"];
            this.ScullyRequired             = row.IsNull("ScullyRequired") ? false : (bool)row["ScullyRequired"];
            this.UserData1                  = row.IsNull("UserData1") ? string.Empty : (string)row["UserData1"];
            this.UserData2                  = row.IsNull("UserData2") ? string.Empty : (string)row["UserData2"];
            this.UserData3                  = row.IsNull("UserData3") ? string.Empty : (string)row["UserData3"];
            this.UserData4                  = row.IsNull("UserData4") ? string.Empty : (string)row["UserData4"];
            this.UserData5                  = row.IsNull("UserData5") ? string.Empty : (string)row["UserData5"];
            this.UserData6                  = row.IsNull("UserData6") ? string.Empty : (string)row["UserData6"];
            this.UserData7                  = row.IsNull("UserData7") ? string.Empty : (string)row["UserData7"];
            this.UserData8                  = row.IsNull("UserData8") ? string.Empty : (string)row["UserData8"];
            this.UserData9                  = row.IsNull("UserData9") ? string.Empty : (string)row["UserData9"];
            this.UserData10                 = row.IsNull("UserData10") ? string.Empty : (string)row["UserData10"];
            this.UserData11                 = row.IsNull("UserData11") ? string.Empty : (string)row["UserData11"];
            this.UserData12                 = row.IsNull("UserData12") ? string.Empty : (string)row["UserData12"];
            this.UserData13                 = row.IsNull("UserData13") ? string.Empty : (string)row["UserData13"];
            this.UserData14                 = row.IsNull("UserData14") ? string.Empty : (string)row["UserData14"];
            this.UserData15                 = row.IsNull("UserData15") ? string.Empty : (string)row["UserData15"];
            this.UserData16                 = row.IsNull("UserData16") ? string.Empty : (string)row["UserData16"];
            this.UserData17                 = row.IsNull("UserData17") ? string.Empty : (string)row["UserData17"];
            this.UserData18                 = row.IsNull("UserData18") ? string.Empty : (string)row["UserData18"];
            this.UserData19                 = row.IsNull("UserData19") ? string.Empty : (string)row["UserData19"];
            this.UserData20                 = row.IsNull("UserData20") ? string.Empty : (string)row["UserData20"];
            this.UserData21                 = row.IsNull("UserData21") ? string.Empty : (string)row["UserData21"];
            this.UserData22                 = row.IsNull("UserData22") ? string.Empty : (string)row["UserData22"];
            this.UserData23                 = row.IsNull("UserData23") ? string.Empty : (string)row["UserData23"];
            this.UserData24                 = row.IsNull("UserData24") ? string.Empty : (string)row["UserData24"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index = -99;
            this.SiteIndex = -99;
            this.SiteId = string.Empty;
            this.Id = string.Empty;
            this.Description = string.Empty;
            this.EqTypeIndex = -99;
            this.EqTypeName = string.Empty;
            this.Make = string.Empty;
            this.Model = string.Empty;
            this.Year = null;
            this.IssPtNum = string.Empty;
            this.CompanyIndex = null;
            this.CompanyId = string.Empty;
            this.Fixed = false;
            this.StorageType = string.Empty;
            this.InUse = false;
            this.ProductIndex = null;
            this.ProductId = string.Empty;
            this.FuelCardIndex = null;
            this.FuelCardId = string.Empty;
            this.FixedVolume = false;
            this.IntoPlane = false;
            this.Mobile = false;
            this.AttachedTo = string.Empty;
            this.MediaType = string.Empty;
            this.Meters = null;
            this.DefuelMeterForwards = false;
            this.PulseRatio = null;
            this.Round = false;
            this.Xref = string.Empty;
            this.LowStockWarning = null;
            this.StockTrack = false;
            this.Totalisor1 = string.Empty;
            this.Totalisor2 = string.Empty;
            this.FuelingState = string.Empty;
            this.Volume = null;
            this.MeterReading = null;
            this.ConsectiveOosVariance = null;
            this.Notes = string.Empty;
            this.Capacity = null;
            this.SafeFill = null;
            this.VolumeUnitIndex = null;
            this.TemperatureUnitIndex = null;
            this.DensityUnitIndex = null;
            this.MassUnitIndex = null;
            this.VolumeDecimalPlaces = null;
            this.TemperatureDecimalPlaces = null;
            this.DensityDecimalPlaces = null;
            this.MassDecimalPlaces = null;
            this.EquipmentIndex = null;
            this.EquipmentSequence = string.Empty;
            this.LockedOut = false;
            this.LockedOutReason = string.Empty;
            this.LockedOutDate = null;
            this.SerialNumber = string.Empty;
            this.CompanyEquipmentId = string.Empty;
            this.TruckCardNumber = string.Empty;
            this.RatedGpm = null;
            this.ActualGpm = null;
            this.FuelAdditiveFlag = false;
            this.ManufactureDate = null;
            this.InstallationDate = null;
            this.InspectionDate = null;
            this.CalibrationDate = null;
            this.QcDate = null;
            this.SecondaryStorageFlag = false;
            this.ManagedEquipmentFlag = false;
            this.FuelingType = null;
            this.ScullyRequired = false;
            this.UserData1 = string.Empty;
            this.UserData2 = string.Empty;
            this.UserData3 = string.Empty;
            this.UserData4 = string.Empty;
            this.UserData5 = string.Empty;
            this.UserData6 = string.Empty;
            this.UserData7 = string.Empty;
            this.UserData8 = string.Empty;
            this.UserData9 = string.Empty;
            this.UserData10 = string.Empty;
            this.UserData11 = string.Empty;
            this.UserData12 = string.Empty;
            this.UserData13 = string.Empty;
            this.UserData14 = string.Empty;
            this.UserData15 = string.Empty;
            this.UserData16 = string.Empty;
            this.UserData17 = string.Empty;
            this.UserData18 = string.Empty;
            this.UserData19 = string.Empty;
            this.UserData20 = string.Empty;
            this.UserData21 = string.Empty;
            this.UserData22 = string.Empty;
            this.UserData23 = string.Empty;
            this.UserData24 = string.Empty;
        }
        #endregion
    }
}
