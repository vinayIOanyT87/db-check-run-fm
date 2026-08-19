// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataImportExportOption.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines common enumeration types used throughout the import / export wizard.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Constants
{
    /// <summary>
    /// The installation type.
    /// </summary>
    public enum InstallationType
    {
        /// <summary>
        /// The client node.
        /// </summary>
        ClientNode = 0,

        /// <summary>
        /// The base server.
        /// </summary>
        BaseServer = 1,

        /// <summary>
        /// The enterprise server.
        /// </summary>
        EnterpriseServer = 2
    }

    /// <summary>
    /// The data import export option.
    /// </summary>
    public enum DataImportExportOption
    {
        /// <summary>
        /// The export keys
        /// </summary>
        ExportKeys = 0,

        /// <summary>
        /// The import keys
        /// </summary>
        ImportKeys = 1,

        /// <summary>
        /// Export migration data
        /// </summary>
        ExportData = 2,

        /// <summary>
        /// Import migration data
        /// </summary>
        ImportData = 3
    }
}