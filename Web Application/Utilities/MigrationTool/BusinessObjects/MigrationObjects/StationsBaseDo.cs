namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class StationsBaseDo : MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public StationsBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public StationsBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int SiteIndex { get; set; }
        public string Id { get; set; }
        public int Type { get; set; }
        public bool SwingArmPosition { get; set; }
        public bool VaporRecovery { get; set; }
        public int InterfaceType { get; set; }
        public bool Enabled { get; set; }
        public string BolPrinter { get; set; }
        public string PreloadPrinter { get; set; }
        public int BolAgeInMinutes { get; set; }
        public int? IssueByVolumeTransactionAliasIndex { get; set; }
        public int? IssueByWeightTransactionAliasIndex { get; set; }
        public int? ReceiptByVolumeTransactionAliasIndex { get; set; }
        public int? ReceiptByWeightTransactionAliasIndex { get; set; }
        public bool CardReader { get; set; }
        public bool ThirtyFiveBitCardSupport { get; set; }
        public int? NumberOfCopies { get; set; }
        public int? NumberOfPreloadCopies { get; set; }
        public bool InhibitLoadingByLoadId { get; set; }
        public bool InhibitOperatingModePrompt { get; set; }
        public bool SynchronizeReferenceDensity { get; set; }
        public string SignatureDevice { get; set; }
        public bool SetDefaultPresetToZero { get; set; }
        public int? AssociatedTankIndex { get; set; }
        public string ArmsServiced { get; set; }
        public bool InhibitSettingRecipeNames { get; set; }
        public int? SignatureDevicePort { get; set; }
        public int? SignatureDeviceBaudRate { get; set; }
        public string MeterRecircCardNumber { get; set; }
        public int? RecircTransactionAliasIndex { get; set; }
        public bool TouchKeyReader { get; set; }
        public bool OffLoadByOffLoadId { get; set; }
        public bool UseManualMeterData { get; set; }
        public bool PromptForBolNumber { get; set; }
        public int? StationPromptTimeout { get; set; }
        public int? StationMessageTimeout { get; set; }
        public bool LogCommunications { get; set; }
        public string LogCommPath { get; set; }
        public int? LastTransactionNumber { get; set; }
        public DateTime? LastTransactionNumberDateTime { get; set; }
        public bool UseTankDensTemp { get; set; }
        public bool QueryForTrailers { get; set; }
        public string WeightPrinter { get; set; }
        public int? NumberOfWeightCopies { get; set; }
        public bool EnableScully { get; set; }
        public bool EnableEquipmentValidate { get; set; }

        public string IssueByVolumeAliasName { get; set; }
        public string IssueByWeightAliasName { get; set; }
        public string ReceiptByVolumeAliasName { get; set; }
        public string ReceiptByWeightAliasName { get; set; }
        public string RecircAliasName { get; set; }
        public string TankId { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateStationsSql(SqlCommand command, int siteIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT S.*"
                            + ", TA1.AliasName AS IssueByVolumeAliasName"
                            + ", TA2.AliasName AS IssueByWeightAliasName"
                            + ", TA3.AliasName AS ReceiptByVolumeAliasName"
                            + ", TA4.AliasName AS ReceiptByWeightAliasName"
                            + ", TA5.AliasName AS RecircAliasName"
                            + ", T.TankID";

            string from = " FROM " + this.SourceDbName + ".dbo.tblStations S"
                + " LEFT JOIN " + this.SourceDbName + ".dbo.tblTransactionAliases TA1 ON TA1.AliasID = S.IssueByVolumeTransactionAliasIndex"
                + " LEFT JOIN " + this.SourceDbName + ".dbo.tblTransactionAliases TA2 ON TA2.AliasID = S.IssueByWeightTransactionAliasIndex"
                + " LEFT JOIN " + this.SourceDbName + ".dbo.tblTransactionAliases TA3 ON TA3.AliasID = S.ReceiptByVolumeTransactionAliasIndex"
                + " LEFT JOIN " + this.SourceDbName + ".dbo.tblTransactionAliases TA4 ON TA4.AliasID = S.ReceiptByWeightTransactionAliasIndex"
                + " LEFT JOIN " + this.SourceDbName + ".dbo.tblTransactionAliases TA5 ON TA5.AliasID = S.RecircTransactionAliasIndex"
                + " LEFT JOIN " + this.SourceDbName + ".dbo.tblTanks T ON T.TankIndex = S.AssociatedTankIndex";
            string where = " WHERE S.SiteIndex = " + siteIndex;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Index                                  = row.IsNull("Index") ? -99 : (int)row["Index"];
            this.SiteIndex                              = row.IsNull("SiteIndex") ? -99 : (int)row["SiteIndex"];
            this.Id                                     = row.IsNull("ID") ? string.Empty : (string)row["ID"];
            this.SwingArmPosition                       = row.IsNull("SwingArmPosition") ? false : (bool)row["SwingArmPosition"];
            this.Type                                   = row.IsNull("Type") ? -99 : (int)row["Type"];
            this.VaporRecovery                          = row.IsNull("VaporRecovery") ? false : (bool)row["VaporRecovery"];
            this.InterfaceType                          = row.IsNull("InterfaceType") ? -99 : (int)row["InterfaceType"];
            this.Enabled                                = row.IsNull("Enabled") ? false : (bool)row["Enabled"];
            this.BolPrinter                             = row.IsNull("BOLPrinter") ? string.Empty : (string)row["BOLPrinter"];
            this.PreloadPrinter                         = row.IsNull("PreloadPrinter") ? string.Empty : (string)row["PreloadPrinter"];
            this.BolAgeInMinutes                        = row.IsNull("BOLAgeInMinutes") ? -99 : (int)row["BOLAgeInMinutes"];
            this.IssueByVolumeTransactionAliasIndex     = row.IsNull("IssueByVolumeTransactionAliasIndex") ? null : (int?)row["IssueByVolumeTransactionAliasIndex"];
            this.IssueByWeightTransactionAliasIndex     = row.IsNull("IssueByWeightTransactionAliasIndex") ? null : (int?)row["IssueByWeightTransactionAliasIndex"];
            this.ReceiptByVolumeTransactionAliasIndex   = row.IsNull("ReceiptByVolumeTransactionAliasIndex") ? null : (int?)row["ReceiptByVolumeTransactionAliasIndex"];
            this.ReceiptByWeightTransactionAliasIndex   = row.IsNull("ReceiptByWeightTransactionAliasIndex") ? null : (int?)row["ReceiptByWeightTransactionAliasIndex"];
            this.CardReader                             = row.IsNull("CardReader") ? false : (bool)row["CardReader"];
            this.ThirtyFiveBitCardSupport               = row.IsNull("ThirtyFiveBitCardSupport") ? false : (bool)row["ThirtyFiveBitCardSupport"];
            this.NumberOfCopies                         = row.IsNull("NumberOfCopies") ? null : (int?)row["NumberOfCopies"];
            this.NumberOfPreloadCopies                  = row.IsNull("NumberOfPreloadCopies") ? null : (int?)row["NumberOfPreloadCopies"];
            this.InhibitLoadingByLoadId                 = row.IsNull("InhibitLoadingByLoadID") ? false : (bool)row["InhibitLoadingByLoadID"];
            this.InhibitOperatingModePrompt             = row.IsNull("InhibitOperatingModePrompt") ? false : (bool)row["InhibitOperatingModePrompt"];
            this.SynchronizeReferenceDensity            = row.IsNull("SynchronizeReferenceDensity") ? false : (bool)row["SynchronizeReferenceDensity"];
            this.SignatureDevice                        = row.IsNull("SignatureDevice") ? string.Empty : (string)row["SignatureDevice"];
            this.SetDefaultPresetToZero                 = row.IsNull("SetDefaultPresetToZero") ? false : (bool)row["SetDefaultPresetToZero"];
            this.AssociatedTankIndex                    = row.IsNull("AssociatedTankIndex") ? null : (int?)row["AssociatedTankIndex"];
            this.ArmsServiced                           = row.IsNull("ArmsServiced") ? string.Empty : (string)row["ArmsServiced"];
            this.InhibitSettingRecipeNames              = row.IsNull("InhibitSettingRecipeNames") ? false : (bool)row["InhibitSettingRecipeNames"];
            this.SignatureDevicePort                    = row.IsNull("SignatureDevicePort") ? null : (int?)row["SignatureDevicePort"];
            this.SignatureDeviceBaudRate                = row.IsNull("SignatureDeviceBaudRate") ? null : (int?)row["SignatureDeviceBaudRate"];
            this.MeterRecircCardNumber                  = row.IsNull("MeterRecircCardNumber") ? string.Empty : (string)row["MeterRecircCardNumber"];
            this.RecircTransactionAliasIndex            = row.IsNull("RecircTransactionAliasIndex") ? null : (int?)row["RecircTransactionAliasIndex"];
            this.TouchKeyReader                         = row.IsNull("TouchKeyReader") ? false : (bool)row["TouchKeyReader"];
            this.OffLoadByOffLoadId                     = row.IsNull("OffLoadByOffLoadID") ? false : (bool)row["OffLoadByOffLoadID"];
            this.UseManualMeterData                     = row.IsNull("UseManualMeterData") ? false : (bool)row["UseManualMeterData"];
            this.PromptForBolNumber                     = row.IsNull("PromptForBOLNumber") ? false : (bool)row["PromptForBOLNumber"];
            this.StationPromptTimeout                   = row.IsNull("StationPromptTimeout") ? null : (int?)row["StationPromptTimeout"];
            this.StationMessageTimeout                  = row.IsNull("StationMessageTimeout") ? null : (int?)row["StationMessageTimeout"];
            this.LogCommunications                      = row.IsNull("LogCommunications") ? false : (bool)row["LogCommunications"];
            this.LogCommPath                            = row.IsNull("LogCommPath") ? string.Empty : (string)row["LogCommPath"];
            this.LastTransactionNumber                  = row.IsNull("LastTransactionNumber") ? null : (int?)row["LastTransactionNumber"];
            this.LastTransactionNumberDateTime          = row.IsNull("LastTransactionNumberDateTime") ? null : (DateTime?)row["LastTransactionNumberDateTime"];
            this.UseTankDensTemp                        = row.IsNull("UseTankDensTemp") ? false : (bool)row["UseTankDensTemp"];
            this.QueryForTrailers                       = row.IsNull("QueryForTrailers") ? false : (bool)row["QueryForTrailers"];
            this.WeightPrinter                          = row.IsNull("WeightPrinter") ? string.Empty : (string)row["WeightPrinter"];
            this.NumberOfWeightCopies                   = row.IsNull("NumberOfWeightCopies") ? null : (int?)row["NumberOfWeightCopies"];
            this.EnableScully                           = row.IsNull("EnableScully") ? false : (bool)row["EnableScully"];
            this.EnableEquipmentValidate                = row.IsNull("EnableEquipmentValidate") ? false : (bool)row["EnableEquipmentValidate"];
            this.IssueByVolumeAliasName                 = row.IsNull("IssueByVolumeAliasName") ? string.Empty : (string)row["IssueByVolumeAliasName"];
            this.IssueByWeightAliasName                 = row.IsNull("IssueByWeightAliasName") ? string.Empty : (string)row["IssueByWeightAliasName"];
            this.ReceiptByVolumeAliasName               = row.IsNull("ReceiptByVolumeAliasName") ? string.Empty : (string)row["ReceiptByVolumeAliasName"];
            this.ReceiptByWeightAliasName               = row.IsNull("ReceiptByWeightAliasName") ? string.Empty : (string)row["ReceiptByWeightAliasName"];
            this.RecircAliasName                        = row.IsNull("RecircAliasName") ? string.Empty : (string)row["RecircAliasName"];
            this.TankId                                 = row.IsNull("TankID") ? string.Empty : (string)row["TankID"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index                                  = -99;
            this.SiteIndex                              = -99;
            this.Id                                     = string.Empty;
            this.Type                                   = -99;
            this.SwingArmPosition                       = false;
            this.VaporRecovery                          = false;
            this.InterfaceType                          = -99;
            this.Enabled                                = false;
            this.BolPrinter                             = string.Empty;
            this.PreloadPrinter                         = string.Empty;
            this.BolAgeInMinutes                        = -99;
            this.IssueByVolumeTransactionAliasIndex     = null;
            this.IssueByWeightTransactionAliasIndex     = null;
            this.ReceiptByVolumeTransactionAliasIndex   = null;
            this.ReceiptByWeightTransactionAliasIndex   = null;
            this.CardReader                             = false;
            this.ThirtyFiveBitCardSupport               = false;
            this.NumberOfCopies                         = null;
            this.NumberOfPreloadCopies                  = null;
            this.InhibitLoadingByLoadId                 = false;
            this.InhibitOperatingModePrompt             = false;
            this.SynchronizeReferenceDensity            = false;
            this.SignatureDevice                        = string.Empty;
            this.SetDefaultPresetToZero                 = false;
            this.AssociatedTankIndex                    = null;
            this.ArmsServiced                           = string.Empty;
            this.InhibitSettingRecipeNames              = false;
            this.SignatureDevicePort                    = null;
            this.SignatureDeviceBaudRate                = null;
            this.MeterRecircCardNumber                  = string.Empty;
            this.RecircTransactionAliasIndex            = null;
            this.TouchKeyReader                         = false;
            this.OffLoadByOffLoadId                     = false;
            this.UseManualMeterData                     = false;
            this.PromptForBolNumber                     = false;
            this.StationPromptTimeout                   = null;
            this.StationMessageTimeout                  = null;
            this.LogCommunications                      = false;
            this.LogCommPath                            = string.Empty;
            this.LastTransactionNumber                  = null;
            this.LastTransactionNumberDateTime          = null;
            this.UseTankDensTemp                        = false;
            this.QueryForTrailers                       = false;
            this.WeightPrinter                          = string.Empty;
            this.NumberOfWeightCopies                   = null;
            this.EnableScully                           = false;
            this.EnableEquipmentValidate                = false;
            this.IssueByVolumeAliasName                 = string.Empty;
            this.IssueByWeightAliasName                 = string.Empty;
            this.ReceiptByVolumeAliasName               = string.Empty;
            this.ReceiptByWeightAliasName               = string.Empty;
            this.RecircAliasName                        = string.Empty;
            this.TankId                                 = string.Empty;
        }
        #endregion

    }
}
