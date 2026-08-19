// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigrationDataExportImport.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IMigrationDataExportImport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System.Diagnostics;
    using System.IO;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// The MigrationDataExportImport interface.
    /// </summary>
    [ServiceContract]
    public interface IMigrationDataExportImport
    {
        /// <summary>
        /// Generates a ZIP Archive containing the GUID mapping information for all the data associated with the passed in <see cref="SiteClass"/>.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <param name="eventLogSource">
        /// The event Log Source.
        /// </param>
        /// <param name="zipArchiveFilename">
        /// Output string that contains the filename of the ZIP Archive that was generated.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
        /// </returns>
        [OperationContract]
        byte[] GetGuidMappingZipArchive(SecurityClass security, SiteClass site, string eventLogSource, out string zipArchiveFilename);

        /// <summary>
        /// The write to event logs.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="eventLogSource">
        /// The event log source.
        /// </param>
        /// <param name="message">
        /// Message to write to the event log
        /// </param>
        /// <param name="eventLogEntryType">
        /// The event log entry type.
        /// </param>
        [OperationContract]
        void WriteToEventLogs(SecurityClass security, string eventLogSource, string message, EventLogEntryType eventLogEntryType);

        /// <summary>
        /// The get migration data export import configuration.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="eventlogSource">
        /// The event log source.
        /// </param>
        /// <returns>
        /// An instance of a populated <see cref="MigrationDataExportImportSettingDO"/> object.
        /// </returns>
        [OperationContract]
        MigrationDataExportImportSettingDO GetMigrationDataExportImportConfiguration(
            SecurityClass security, string eventlogSource);
    }
}
