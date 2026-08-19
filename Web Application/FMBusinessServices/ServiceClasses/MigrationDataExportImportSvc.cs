// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationDataExportImportSvc.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MigrationDataExportImportSvc type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using FMBusinessServices.InternalClasses;
    using FMBusinessServices.InternalClasses.SyncClasses;

    /// <summary>
    /// The enterprise data migration export import.
    /// </summary>
    /// <remarks>
    /// Common classes and functions used in the Enterprise Export Windows Service and and Import Web service.	
    /// Prerequisite:  connection string in registry, settings in database table <![CDATA[tblSettings]]>.
    /// </remarks>
    public class MigrationDataExportImportSvc : IMigrationDataExportImport
    {
        #region Attributes
        /// <summary>
        /// The alarm and event log.
        /// </summary>
        private AlarmAndEventLogClass alarmAndEventLog;

        /// <summary>
        /// The event log.
        /// </summary>
        private EventLog eventLog;

        /// <summary>
        /// The migration export import settings, used to hold standard setting information.
        /// </summary>
        private MigrationDataExportImportSettingDO migrationDataExportImportSettingDo;
        #endregion Attributes

        #region Initialization Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationDataExportImportSvc"/> class.
        /// </summary>
        protected MigrationDataExportImportSvc()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationDataExportImportSvc"/> class.
        /// </summary>
        /// <param name="security">
        /// The current _Security context.
        /// </param>
        /// <param name="eventlogSource">
        /// The event log source.
        /// </param>
        public MigrationDataExportImportSvc(SecurityClass security, string eventlogSource)
        {
            this.InitLogging(security, eventlogSource);
        }

        /// <summary>
        /// Initialization method
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="eventlogSource">
        /// The event log source.
        /// </param>
        private void InitLogging(SecurityClass security, string eventlogSource)
        {
            if (string.IsNullOrEmpty(eventlogSource))
            {
                eventlogSource = "FuelsManager";
            }

            if (EventLog.SourceExists(eventlogSource) == false)
            {
                EventLog.CreateEventSource(eventlogSource, "Application");
            }

            this.eventLog = new EventLog("Application", ".", eventlogSource);

            this.alarmAndEventLog = new AlarmAndEventLogClass();
            this.alarmAndEventLog.Source = eventlogSource;

            if (null == this.migrationDataExportImportSettingDo)
            {
                this.migrationDataExportImportSettingDo = this.GetMigrationDataExportImportConfiguration(
                    security, eventlogSource);
            }
        }
        #endregion Initialization Methods

        #region Public Methods

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
        /// The event log source.
        /// </param>
        /// <param name="zipArchiveFilename">
        /// Output string that contains the name of the <see cref="ZipArchive"/> file that was generated.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        public byte[] GetGuidMappingZipArchive(SecurityClass security, SiteClass site, string eventLogSource, out string zipArchiveFilename)
        {
            this.InitLogging(security, eventLogSource);

            SiteTimeConverter timeConverter = new SiteTimeConverter(site);
            string exportTimestamp = timeConverter.Now().ToString("yyyyMMdd_HHmmss");

            string exportPath = this.migrationDataExportImportSettingDo.ExportArchivePath;

            if (!exportPath.EndsWith(@"\"))
            {
                exportPath += @"\";
            }

            // This is the final output file that will be downloaded to the user.
            string zipArchiveFile = string.Format(@"{0}{1}_EntKeyData_{2}.{3}", exportPath, site.ID, exportTimestamp, "vzip");
            zipArchiveFilename = Path.GetFileName(zipArchiveFile);

            this.ExportGuidMappingInformation(security, site, zipArchiveFile, exportTimestamp);

            FileInfo fi = new FileInfo(zipArchiveFile);

            MemoryStream mappingData = new MemoryStream();

            if (fi.Length > 0)
            {
                byte[] buffer = new byte[4096];
                int byteCount = 0;

                using (FileStream streamReader = new FileStream(zipArchiveFile, FileMode.Open, FileAccess.Read))
                {
                    while ((byteCount = streamReader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        mappingData.Write(buffer, 0, byteCount);
                    }
                }

                // Cleanup the zip archive file now that we have it in memory.
                File.Delete(zipArchiveFile);
            }

            return mappingData.ToArray();
        }

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
        /// The message.
        /// </param>
        /// <param name="eventLogEntryType">
        /// The event log entry type.
        /// </param>
        public void WriteToEventLogs(SecurityClass security, string eventLogSource, string message, EventLogEntryType eventLogEntryType)
        {
            this.InitLogging(security, eventLogSource);

            if (string.IsNullOrEmpty(message) == true)
            {
                return;
            }

            this.eventLog.WriteEntry(message, eventLogEntryType);

            this.alarmAndEventLog.Alarm = true;
            const int MaxlengthOfMessage = 1000; // this is a large as the associated data can hold.

            string strSqlSafeMessage = message.Replace("'", "''");  // escape the single quote that shows up in some error messages. 

            if (strSqlSafeMessage.Length > MaxlengthOfMessage)
            {
                this.alarmAndEventLog.AssociatedData = strSqlSafeMessage.Remove(MaxlengthOfMessage);
            }
            else
            {
                this.alarmAndEventLog.AssociatedData = strSqlSafeMessage;
            }

            this.alarmAndEventLog.SiteGuid = security.SiteGuid;
            this.alarmAndEventLog.UpdatedDate = DateTimeOffset.Now;
            this.alarmAndEventLog.UpdatedBy = security.UserID ?? "FuelsManager";
            this.alarmAndEventLog.CreatedDate = DateTimeOffset.Now;
            this.alarmAndEventLog.CreatedBy = security.UserID ?? "FuelsManager";
            this.alarmAndEventLog.ID = "Migration Data Export Import Event";

            AlarmAndEventLogsClass alarmAndEventLogs = new AlarmAndEventLogsClass();
            alarmAndEventLogs.Add(security, this.alarmAndEventLog);
        }

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
        /// <exception cref="Exception">
        /// An exception will be thrown if an error is encountered by the <see cref="ConfigurationSettingsClass" />
        /// </exception>
        public MigrationDataExportImportSettingDO GetMigrationDataExportImportConfiguration(SecurityClass security, string eventlogSource)
        {
            var settings = new MigrationDataExportImportSettingDO { AlarmAndEventSourceName = eventlogSource };

            try
            {
                ConfigurationSettingsClass setting = new ConfigurationSettingsClass();

                settings.ExportArchivePath = setting.GetKeyValueByKey(security, "MigrationExportDir");
                settings.ImportArchivePath = setting.GetKeyValueByKey(security, "MigrationImportDir");
                settings.SelectedSiteGuid = Guid.Empty;
            }
            catch (Exception ex)
            {
                string strAdditionalMessage = string.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), "InitLogging", ex.Message);
                this.WriteToEventLogs(security, this.alarmAndEventLog.Source, strAdditionalMessage, EventLogEntryType.Error);
                throw ex;
            }

            return settings;
        }

        #endregion Public Methods
        
        #region Private Methods
        /// <summary>
        /// The write stream to file.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="pathToWriteTo">
        /// The path to write to.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if an error is encountered while writing the specified stream to the output folder.
        /// </exception>
        private string WriteStreamToFile(SecurityClass security, MemoryStream stream, string pathToWriteTo, string fileName)
        {
            string result = null;
            const string FunctionName = "WriteStreamToFile(MemoryStream stream, String pathToWriteTo)";

            try
            {
                // If the Export Archive Directory exist save the file to the directory.			
                if (pathToWriteTo != null)
                {
                    string path = pathToWriteTo.Trim();

                    if (path.Length > 0)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(path);

                        if (!directoryInfo.Exists)
                        {
                            throw new Exception("Directory Error, Check system export/import directory settings.");
                        }

                        const string BackSlash = "\\";

                        if (!path.EndsWith(BackSlash))
                        {
                            path += BackSlash;
                        }

                        string strArchiveDirAndFileName = path + fileName;
                        result = strArchiveDirAndFileName;
                        FileStream fstream = new FileStream(strArchiveDirAndFileName, FileMode.Create);
                        stream.WriteTo(fstream);
                        fstream.Flush();
                        fstream.Close();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                string strAdditionalMessage = string.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), FunctionName, ex.Message);
                this.WriteToEventLogs(security, this.alarmAndEventLog.Source, strAdditionalMessage, EventLogEntryType.Error);
                throw ex;
            }
        }

        /// <summary>
        /// The export GUID mapping information.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <param name="exportFile">
        /// The export File.
        /// </param>
        /// <param name="batchTimestamp">
        /// The batch Timestamp.
        /// </param>
        private void ExportGuidMappingInformation(SecurityClass security, SiteClass site, string exportFile, string batchTimestamp)
        {
            // Multiple files will be generated, a single file for each table since some tables will need to return more information than others.
            // At the end of this process, the file will be zipped into a single file
            // this.recordCollection =
            //    FMChannelHelper.MakeCall<IChangeQueueRecordsClass, ChangeQueueRecordCollection>(
            //        x => x.EnumerateIncompleteRecords(this.Security));
            using (var exportFileStream = new FileStream(exportFile, FileMode.CreateNew))
            {
                using (var exportFileArchive = new ZipArchive(exportFileStream, ZipArchiveMode.Create, true))
                {
                    string exportPath = Path.GetDirectoryName(exportFile);

                    List<string> fileList = this.ProcessExportGroupsAsync(security, this.alarmAndEventLog, exportPath, batchTimestamp, site);

                    if (fileList.Count > 0)
                    {
                        // Zip all of the files together.
                        foreach (string file in fileList)
                        {
                            FileInfo fi = new FileInfo(file);

                            if (fi.Length > 0)
                            {
                                exportFileArchive.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
                            }

                            // Cleanup the individual export files now that we have loaded them into a ZipArchiveEntry.
                            File.Delete(file);
                        }
                    }
                }

                exportFileStream.Flush();
            }
        }

        /// <summary>
        /// The process export groups async.
        /// </summary>
        /// <param name="security">
        /// Current security context.
        /// </param>
        /// <param name="alarmAndEventLog">
        /// The alarm and event log.
        /// </param>
        /// <param name="exportFolder">
        /// The export folder.
        /// </param>
        /// <param name="batchTimestamp">
        /// The batch timestamp.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private List<string> ProcessExportGroupsAsync(SecurityClass security, AlarmAndEventLogClass alarmAndEventLog, string exportFolder, string batchTimestamp, SiteClass site)
        {
            List<string> exportFiles = new List<string>();

            try
            {
                // Process each dependency level in parallel.
                using (SyncProfileDBI profileDbi = new SyncProfileDBI(security.UserID))
                {
                    SyncProfileDO profile = profileDbi.Get(security, null, SyncProfileDBI.CompleteProfileName);

                    if (null != profile)
                    {
                        SyncTableDBI tableDbi = new SyncTableDBI(security.UserID);
                        List<SyncTableDO> syncTableInfoList = tableDbi.GetList(security);
                        tableDbi.Dispose();

                        IEnumerable<string> includedTables = from s in syncTableInfoList
                                                             where !s.TableName.StartsWith("lookup")
                                                             select s.TableName.Substring(s.TableName.IndexOf('.') + 1);

                        using (SyncScopeDBI scopeDbi = new SyncScopeDBI(security.UserID))
                        {
                            List<SyncScopeDO> scopeList = scopeDbi.GetList(security, profile.IdentityGuid);

                            if (null != scopeList)
                            {
                                foreach (SyncScopeDO syncScope in scopeList)
                                {
                                    using (
                                        SyncTableToScopeMapDBI scopeTableListDbi =
                                            new SyncTableToScopeMapDBI(security.UserID))
                                    {
                                        SyncTableToScopeMapCollection scopeTableList = scopeTableListDbi.GetList(security, syncScope.IdentityGuid);

                                        var tableList = (from s in scopeTableList select s.ID).Intersect(includedTables);

                                        List<string> exportedTables = this.ProcessExportGroupItemsAsync(
                                            security, alarmAndEventLog, tableList, exportFolder, batchTimestamp, site);

                                        foreach (string exportedFileName in exportedTables)
                                        {
                                            exportFiles.Add(exportedFileName);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            finally
            {

            }

            return exportFiles;
        }

        /// <summary>
        /// The process export group items async.
        /// </summary>
        /// <param name="security">
        /// Current security context.
        /// </param>
        /// <param name="alarmAndEventLog">
        /// The alarm and event log.
        /// </param>
        /// <param name="tableList">
        /// The table list.
        /// </param>
        /// <param name="exportFolder">
        /// The export folder.
        /// </param>
        /// <param name="batchTimestamp">
        /// The batch timestamp.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// May thrown an exception from the FileStream, XmlSerializer or MemoryStream.
        /// </exception>
        private List<string> ProcessExportGroupItemsAsync(SecurityClass security, AlarmAndEventLogClass alarmAndEventLog, IEnumerable<string> tableList, string exportFolder, string batchTimestamp, SiteClass site)
        {
            List<string> exportFiles = new List<string>();

            try
            {
                // Process each table within the current level.
                // This logic needs to be in a lower Async Method so that this outer dependency loop can wait for all the internal tasks to complete prior 
                // to moving on to the next group.
                foreach (string tableName in tableList)
                {
                    if (!exportFolder.EndsWith(@"\"))
                    {
                        exportFolder += @"\";
                    }

                    string exportFile = exportFolder
                                        + string.Format(
                                            "{0}_EntKeyData_{1}_{2}_Temp.{3}",
                                            site.ID,
                                            batchTimestamp,
                                            tableName,
                                            "vkeyef");

                    // Get the dataset for this table
                    DataSet resultSet = null;

                    using (MigrationExportImportDBI dbi = new MigrationExportImportDBI(security.UserID))
                    {
                        resultSet = dbi.GetKeyMappingListForTable(security, tableName);
                    }

                    // Only add something to the results if we actually had data
                    if (null != resultSet && resultSet.Tables.Count > 0 && resultSet.Tables[0].Rows.Count > 0)
                    {
                        FileStream fstream = new FileStream(
                            exportFile,
                            FileMode.CreateNew,
                            FileAccess.Write,
                            FileShare.None,
                            bufferSize: 4096,
                            useAsync: true);

                        var stream = new MemoryStream();

                        var serializer = new XmlSerializer(typeof(DataSet));
                        serializer.Serialize(stream, resultSet);

                        fstream.Write(stream.GetBuffer(), 0, (int)stream.Length);

                        exportFiles.Add(exportFile);
                        fstream.Flush();
                        fstream.Close();
                    }
                }
            }
            catch (Exception eX)
            {
                throw eX;
            }
            finally
            {
                //foreach (FileStream sourceStream in exportStreams)
                //{
                //    sourceStream.Flush();
                //    sourceStream.Close();
                //}
            }

            return exportFiles;
        }

        /// <summary>
        /// The export key mapping file for table.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="selectedSite">
        /// The selected site.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        private void ExportKeyMappingFileForTable(SecurityClass security, SiteClass selectedSite, string tableName)
        {
            Guid savedSiteGuid = security.SiteGuid;
            string savedSiteID = security.SiteID;

            security.SiteGuid = selectedSite.IdentityGuid;
            security.SiteID = selectedSite.SiteID;

            MigrationExportImportDBI migrationDbi = new MigrationExportImportDBI(security.UserID);
            migrationDbi.GetKeyMappingListForTable(security, tableName);

            security.SiteGuid = savedSiteGuid;
            security.SiteID = savedSiteID;
        }
        #endregion Private Methods
    }
}
