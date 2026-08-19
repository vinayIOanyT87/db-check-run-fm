// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataImportExportWizardOption.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard.InternalClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using DataImportExportWizard.Constants;

    /// <summary>
    /// The options.
    /// </summary>
    public static class DataImportExportWizardOption
    {
        #region Static Attributes
        #endregion Static Attributes

        #region Static Properties

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public static DataImportExportActionType Action { get; set; }

        /// <summary>
        /// Gets or sets the site id.
        /// </summary>
        public static string SiteId { get; set; }

        /// <summary>
        /// Gets or sets the SQL Server instance.
        /// </summary>
        public static string InstanceName { get; set; }

        /// <summary>
        /// Gets or sets the database to use.
        /// </summary>
        public static string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public static string Path { get; set; }

        /// <summary>
        /// Gets or sets the filename option.
        /// </summary>
        public static string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the utility should perform a re-encryption on the passwords stored in the database using AES Crypt.
        /// </summary>
        public static bool ReEncryptFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the utility should perform a database backup prior to performing any tasks.
        /// </summary>
        public static bool BackupDatabaseFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the utility should run without a user interface.
        /// </summary>
        public static bool QuietFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the utility should display a status dialog during processing.
        /// </summary>
        public static bool ShowStatusFlag { get; set; }

        /// <summary>
        /// Gets or sets the current import export step.
        /// </summary>
        public static WizardStepType CurrentImportExportStep { get; set; }

        /// <summary>
        /// Gets or sets the selected import export option.
        /// </summary>
        public static InstallationType SelectedInstallationType { get; set; }

        #endregion Static Properties

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="DataImportExportWizardOption"/> class.
        /// </summary>
        static DataImportExportWizardOption()
        {
            Action = DataImportExportActionType.Other;
            SiteId = string.Empty;
            InstanceName = string.Empty;
            DatabaseName = string.Empty;
            Path = string.Empty;
            FileName = string.Empty;
            ReEncryptFlag = false;
            BackupDatabaseFlag = true;
            QuietFlag = false;
            ShowStatusFlag = true;
            CurrentImportExportStep = WizardStepType.SelectAction;
            SelectedInstallationType = InstallationType.BaseServer;
        }
        #endregion Constructors

        #region Public Static Methods

        /// <summary>
        /// The set action type.
        /// </summary>
        /// <param name="actionString">
        /// The action string.
        /// </param>
        public static void SetActionType(string actionString)
        {
            switch (actionString.ToLower().Trim())
            {
                case "exportkeys":
                    DataImportExportWizardOption.Action = DataImportExportActionType.ExportKeys;
                    break;
                case "importkeys":
                    DataImportExportWizardOption.Action = DataImportExportActionType.ImportKeys;
                    break;
                case "exportdata":
                    DataImportExportWizardOption.Action = DataImportExportActionType.ExportData;
                    break;
                case "importdata":
                    DataImportExportWizardOption.Action = DataImportExportActionType.ImportData;
                    break;
                case "encryptonly":
                    DataImportExportWizardOption.Action = DataImportExportActionType.EncryptOnly;
                    break;
                default:
                    DataImportExportWizardOption.Action = DataImportExportActionType.Other;
                    break;
            }
        }
        #endregion Public Static Methods
    }
}
