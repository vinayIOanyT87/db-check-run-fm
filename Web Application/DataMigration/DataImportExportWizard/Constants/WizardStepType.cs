// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WizardStepType.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard.Constants
{
    /// <summary>
    /// Enumeration of data import export wizard migration steps.
    /// </summary>
    public enum WizardStepType
    {
        /// <summary>
        /// Select the action to perform.
        /// </summary>
        SelectAction = 0,

        /// <summary>
        /// The export keys
        /// </summary>
        ExportingKeys = 1,

        /// <summary>
        /// The import keys
        /// </summary>
        ImportingKeys = 2,

        /// <summary>
        /// Export migration data
        /// </summary>
        ExportingData = 3,

        /// <summary>
        /// Import migration data
        /// </summary>
        ImportingData = 4,

        /// <summary>
        /// Backing up database
        /// </summary>
        BackingUpDatabase = 5,

        /// <summary>
        /// Restoring database
        /// </summary>
        RestoringUpDatabase = 6,

        /// <summary>
        /// ReEncrypting passwords with AESCrypt.
        /// </summary>
        ReEncryptingPasswords = 7
    }
}
