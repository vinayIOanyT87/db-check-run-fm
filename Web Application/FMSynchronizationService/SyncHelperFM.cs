using System;

using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;


using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace FMSynchronizationService
{
    /// <summary>
    /// Helper class that provides static conversion methods and generic synchronization processing routines.
    /// </summary>
    public class SyncHelperFM
    {
        public enum ObjectNamePart
        {
            NAMEPART_SERVER = 0,
            NAMEPART_DATABASE = 1,
            NAMEPART_SCHEMA = 2,
            NAMEPART_OBJECTNAME = 3
        }

        #region Static Alarm And Event Reporting Methods
        public static void WriteSyncDisabledAlarmAndEvent(SecurityClass pSecurity, string pErrorMessage)
        {
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel =>
            {
                EnterpriseSynchronizationEvents enterpriseSynchronizationEvents = new EnterpriseSynchronizationEvents();
                alarmAndEventChannel.Add(pSecurity, enterpriseSynchronizationEvents.SynchronizationDisabledEvent(pErrorMessage));
            });
        }
        public static void WriteConfigurationAlarmAndEvent(SecurityClass pSecurity, string pErrorMessage)
        {
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel =>
            {
                EnterpriseSynchronizationEvents enterpriseSynchronizationEvents = new EnterpriseSynchronizationEvents();
                alarmAndEventChannel.Add(pSecurity, enterpriseSynchronizationEvents.SynchronizationConfigurationErrorEvent(pErrorMessage));
            });
        }
        public static void WriteErrorAlarmAndEvent(SecurityClass pSecurity, string pErrorMessage)
        {
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel =>
            {
                EnterpriseSynchronizationEvents enterpriseSynchronizationEvents = new EnterpriseSynchronizationEvents();
                alarmAndEventChannel.Add(pSecurity, enterpriseSynchronizationEvents.SynchronizationErrorEncounteredEvent(pErrorMessage));
            });
        }
        public static void WriteConflictAlarmAndEvent(SecurityClass pSecurity, string pConflictDetails)
        {
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel =>
            {
                EnterpriseSynchronizationEvents enterpriseSynchronizationEvents = new EnterpriseSynchronizationEvents();
                alarmAndEventChannel.Add(pSecurity, enterpriseSynchronizationEvents.SynchronizationConflictDetectedEvent(pConflictDetails));
            });
        }
        #endregion Static Alarm And Event Reporting Methods

        #region Static Conversion Methods FROM MS SyncFramework Types

		/// <summary>
		/// Converts Microsoft SyncFramework's <see cref="SyncDirection"/> type to our own abstracted <see cref="SYNCDIRECTION"/> data type./>
		/// </summary>
		/// <param name="Direction">A synchronization direction enumeration of type <see cref="SyncDirection"/>.</param>
		/// <returns>A synchronization direction enumeration of type <see cref="SYNCDIRECTION"/>.</returns>
		public static SYNCDIRECTION ConvertSyncDirection(SyncDirection Direction)
		{
			SYNCDIRECTION direction = SYNCDIRECTION.BIDIRECTIONAL;

			switch (Direction)
			{
				case SyncDirection.DownloadOnly:
					direction = SYNCDIRECTION.DOWNLOADONLY;
					break;
				case SyncDirection.UploadOnly:
					direction = SYNCDIRECTION.UPLOADONLY;
					break;
				case SyncDirection.Snapshot:
					direction = SYNCDIRECTION.SNAPSHOT;
					break;
				case SyncDirection.Bidirectional:
				default:
					direction = SYNCDIRECTION.BIDIRECTIONAL;
					break;
			}

			return (direction);
		}

        /// <summary>
        /// Converts Microsoft SyncFramework's <see cref="SyncStage"/> type to our own abstracted <see cref="SYNCSTAGE"/> data type./>
        /// </summary>
        /// <param name="Stage">A synchronization stage enumeration of type <see cref="SyncStage"/>.</param>
        /// <returns>A synchronization stage enumeration of type <see cref="SYNCSTAGE"/>.</returns>
        public static SYNCSTAGE ConvertSyncStage(SyncStage Stage)
        {
            SYNCSTAGE stage = SYNCSTAGE.READY;

            switch (Stage)
            {
                case SyncStage.ReadingSchema:
                    stage = SYNCSTAGE.READING_SCHEMA;
                    break;
                case SyncStage.CreatingSchema:
                    stage = SYNCSTAGE.CREATING_SCHEMA;
                    break;
                case SyncStage.ReadingMetadata:
                    stage = SYNCSTAGE.READING_METADATA;
                    break;
                case SyncStage.CreatingMetadata:
                    stage = SYNCSTAGE.CREATING_METADATA;
                    break;
                case SyncStage.DeletingMetadata:
                    stage = SYNCSTAGE.DELETING_METADATA;
                    break;
                case SyncStage.WritingMetadata:
                    stage = SYNCSTAGE.WRITING_METADATA;
                    break;
                case SyncStage.UploadingChanges:
                    stage = SYNCSTAGE.UPLOADING_CHANGES;
                    break;
                case SyncStage.DownloadingChanges:
                    stage = SYNCSTAGE.DOWNLOADING_CHANGES;
                    break;
                case SyncStage.ApplyingInserts:
                    stage = SYNCSTAGE.APPLYING_INSERTS;
                    break;
                case SyncStage.ApplyingUpdates:
                    stage = SYNCSTAGE.APPLYING_UPDATES;
                    break;
                case SyncStage.ApplyingDeletes:
                    stage = SYNCSTAGE.APPLYING_DELETES;
                    break;
                case SyncStage.GettingInserts:
                    stage = SYNCSTAGE.GETTING_INSERTS;
                    break;
                case SyncStage.GettingUpdates:
                    stage = SYNCSTAGE.GETTING_UPDATES;
                    break;
                case SyncStage.GettingDeletes:
                    stage = SYNCSTAGE.GETTING_DELETES;
                    break;
                default:
                    stage = SYNCSTAGE.READY;
                    break;
            }

            return (stage);
        }

        /// <summary>
        /// Converts Microsoft SyncFramework's <see cref="ConflictType"/> type to our own abstracted <see cref="SYNCCONFLICTTYPE"/> data type./>
        /// </summary>
        /// <param name="ConflictTypeValue">A synchronization conflict enumeration of type <see cref="ConflictType"/>.</param>
        /// <returns>A synchronization conflict enumeration of type <see cref="SYNCCONFLICTTYPE"/>.</returns>
        public static SYNCCONFLICTTYPE ConvertSyncConflictType(ConflictType ConflictTypeValue)
        {
            SYNCCONFLICTTYPE conflictType = SYNCCONFLICTTYPE.UNKNOWN;

            switch (ConflictTypeValue)
            {
                case ConflictType.ErrorsOccurred:
                    conflictType = SYNCCONFLICTTYPE.ERRORS_OCCURRED;
                    break;
                case ConflictType.ClientUpdateServerUpdate:
                    conflictType = SYNCCONFLICTTYPE.CLIENTUPDATE_SERVERUPDATE;
                    break;
                case ConflictType.ClientUpdateServerDelete:
                    conflictType = SYNCCONFLICTTYPE.CLIENTUPDATE_SERVERDELETE;
                    break;
                case ConflictType.ClientDeleteServerUpdate:
                    conflictType = SYNCCONFLICTTYPE.CLIENTDELETE_SERVERUPDATE;
                    break;
                case ConflictType.ClientInsertServerInsert:
                    conflictType = SYNCCONFLICTTYPE.CLIENTINSERT_SERVERINSERT;
                    break;
                case ConflictType.Unknown:
                default:
                    conflictType = SYNCCONFLICTTYPE.UNKNOWN;
                    break;
            }

            return (conflictType);
        }

        /// <summary>
        /// Converts Microsoft SyncFramework's <see cref="SyncStatistics"/> type to our own abstracted <see cref="SyncStatsFM"/> data type./>
        /// </summary>
        /// <param name="Stats">A synchronization statistics object of type <see cref="SyncStatistics"/>.</param>
        /// <returns>A synchronization statistics object of type <see cref="SyncStatsFM"/>.</returns>
        public static SyncStatsFM ConvertSyncStatistics(SyncStatistics Stats)
        {
            SyncStatsFM stats = new SyncStatsFM();

            if (null != Stats)
            {
                stats.CompleteTime = Stats.SyncCompleteTime;
                stats.DownloadChangesApplied = Stats.DownloadChangesApplied;
                stats.DownloadChangesFailed = Stats.DownloadChangesFailed;
                stats.StartTime = Stats.SyncStartTime;
                stats.TotalChangesDownloaded = Stats.TotalChangesDownloaded;
                stats.TotalChangesUploaded = Stats.TotalChangesUploaded;
                stats.UploadChangesApplied = Stats.UploadChangesApplied;
                stats.UploadChangesFailed = Stats.UploadChangesFailed;
            }

            return (stats);



        }

        #endregion Static Conversion Methods FROM MS SyncFramework Types

        #region Static Conversion Methods TO MS SyncFramework Types

        /// <summary>
        /// Converts our own abstracted <see cref="SYNCDIRECTION"/> data type to Microsoft SyncFramework's <see cref="SyncDirection"/> type./>
        /// </summary>
        /// <param name="Direction">A synchronization direction enumeration of type <see cref="SYNCDIRECTION"/>.</param>
        /// <returns>A synchronization direction enumeration of type <see cref="SyncDirection"/>.</returns>
        public static SyncDirection ConvertSyncDirection(SYNCDIRECTION Direction)
        {
            SyncDirection direction = SyncDirection.Bidirectional;

            switch (Direction)
            {
                case SYNCDIRECTION.DOWNLOADONLY:
                    direction = SyncDirection.DownloadOnly;
                    break;
                case SYNCDIRECTION.UPLOADONLY:
                    direction = SyncDirection.UploadOnly;
                    break;
                case SYNCDIRECTION.SNAPSHOT:
                    direction = SyncDirection.Snapshot;
                    break;
                case SYNCDIRECTION.BIDIRECTIONAL:
                default:
                    direction = SyncDirection.Bidirectional;
                    break;
            }

            return (direction);
        }

        /// <summary>
        /// Converts our own abstracted <see cref="SYNCSTAGE"/> data type to Microsoft SyncFramework's <see cref="SyncStage"/> type./>
        /// </summary>
        /// <param name="Stage">A synchronization stage enumeration of type <see cref="SYNCSTAGE"/>.</param>
        /// <returns>A synchronization stage enumeration of type <see cref="SyncStage"/>.</returns>
        public static SyncStage ConvertSyncStage(SYNCSTAGE Stage)
        {
            SyncStage stage = SyncStage.ReadingMetadata;

            switch (Stage)
            {
                case SYNCSTAGE.READING_SCHEMA:
                    stage = SyncStage.ReadingSchema;
                    break;
                case SYNCSTAGE.CREATING_SCHEMA:
                    stage = SyncStage.CreatingSchema;
                    break;
                case SYNCSTAGE.READING_METADATA:
                    stage = SyncStage.ReadingMetadata;
                    break;
                case SYNCSTAGE.CREATING_METADATA:
                    stage = SyncStage.CreatingMetadata;
                    break;
                case SYNCSTAGE.DELETING_METADATA:
                    stage = SyncStage.DeletingMetadata;
                    break;
                case SYNCSTAGE.WRITING_METADATA:
                    stage = SyncStage.WritingMetadata;
                    break;
                case SYNCSTAGE.UPLOADING_CHANGES:
                    stage = SyncStage.UploadingChanges;
                    break;
                case SYNCSTAGE.DOWNLOADING_CHANGES:
                    stage = SyncStage.DownloadingChanges;
                    break;
                case SYNCSTAGE.APPLYING_INSERTS:
                    stage = SyncStage.ApplyingInserts;
                    break;
                case SYNCSTAGE.APPLYING_UPDATES:
                    stage = SyncStage.ApplyingUpdates;
                    break;
                case SYNCSTAGE.APPLYING_DELETES:
                    stage = SyncStage.ApplyingDeletes;
                    break;
                case SYNCSTAGE.GETTING_INSERTS:
                    stage = SyncStage.GettingInserts;
                    break;
                case SYNCSTAGE.GETTING_UPDATES:
                    stage = SyncStage.GettingUpdates;
                    break;
                case SYNCSTAGE.GETTING_DELETES:
                    stage = SyncStage.GettingDeletes;
                    break;
                default:
                    stage = SyncStage.ReadingMetadata;
                    break;
            }

            return (stage);
        }

        /// <summary>
        /// Converts our own abstracted <see cref="SYNCCONFLICTTYPE"/> data type to Microsoft SyncFramework's <see cref="ConflictType"/> type./>
        /// </summary>
        /// <param name="ConflictTypeValue">A synchronization conflict enumeration of type <see cref="SYNCCONFLICTTYPE"/>.</param>
        /// <returns>A synchronization conflict enumeration of type <see cref="ConflictType"/>.</returns>
        public static ConflictType ConvertSyncConflictType(SYNCCONFLICTTYPE ConflictTypeValue)
        {
            ConflictType conflictType = ConflictType.Unknown;

            switch (ConflictTypeValue)
            {
                case SYNCCONFLICTTYPE.ERRORS_OCCURRED:
                    conflictType = ConflictType.ErrorsOccurred;
                    break;
                case SYNCCONFLICTTYPE.CLIENTUPDATE_SERVERUPDATE:
                    conflictType = ConflictType.ClientUpdateServerUpdate;
                    break;
                case SYNCCONFLICTTYPE.CLIENTUPDATE_SERVERDELETE:
                    conflictType = ConflictType.ClientUpdateServerDelete;
                    break;
                case SYNCCONFLICTTYPE.CLIENTDELETE_SERVERUPDATE:
                    conflictType = ConflictType.ClientDeleteServerUpdate;
                    break;
                case SYNCCONFLICTTYPE.CLIENTINSERT_SERVERINSERT:
                    conflictType = ConflictType.ClientInsertServerInsert;
                    break;
                case SYNCCONFLICTTYPE.UNKNOWN:
                default:
                    conflictType = ConflictType.Unknown;
                    break;
            }

            return (conflictType);
        }

        /// <summary>
        /// Converts our own abstracted <see cref="SyncStatsFM"/> data type to Microsoft SyncFramework's <see cref="SyncStatistics"/> type./>
        /// </summary>
        /// <param name="Stats">A synchronization statistics object of type <see cref="SyncStatsFM"/>.</param>
        /// <returns>A synchronization statistics object of type <see cref="SyncStatistics"/>.</returns>
        public static SyncStatistics ConvertSyncStatistics(SyncStatsFM Stats)
        {
            SyncStatistics stats = new SyncStatistics();

            if (null != Stats)
            {
                if (Stats.CompleteTime.HasValue)
                {
                    stats.SyncCompleteTime = Stats.CompleteTime.Value;
                }

                stats.DownloadChangesApplied = Stats.DownloadChangesApplied;
                stats.DownloadChangesFailed = Stats.DownloadChangesFailed;

                if (Stats.StartTime.HasValue)
                {
                    stats.SyncStartTime = Stats.StartTime.Value;
                }

                stats.TotalChangesDownloaded = Stats.TotalChangesDownloaded;
                stats.TotalChangesUploaded = Stats.TotalChangesUploaded;
                stats.UploadChangesApplied = Stats.UploadChangesApplied;
                stats.UploadChangesFailed = Stats.UploadChangesFailed;
            }

            return (stats);
        }

        #endregion Static Conversion Methods TO MS SyncFramework Types

        #region Utility Methods

        /// <summary>
        /// The get name part from table name.
        /// </summary>
        /// <param name="fullTableName">
        /// The full table name.
        /// </param>
        /// <param name="namePart">
        /// The name part.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetNamePartFromTableName(string fullTableName, ObjectNamePart namePart)
        {
            string fullTableNameNoBracket = fullTableName.Replace("]", string.Empty);
            fullTableNameNoBracket = fullTableNameNoBracket.Replace("[", string.Empty);

            string retValue = string.Empty;

            string serverName = ".";
            string databaseName = ".";
            string schemaName = "dbo";
            string objectName = string.Empty;

            if (!string.IsNullOrEmpty(fullTableNameNoBracket))
            {
                // If there is at least 1 period, we're guaranteed to have at least 2 entries (might be blank, but the [0] and [1] index is valid)
                if (fullTableNameNoBracket.Contains("."))
                {
                    string[] tableNameParts = fullTableNameNoBracket.Split(new char[] { '.' }, StringSplitOptions.None);

                    // At most, we could end up with server.database.schema.objectname (any of these could be blank
                    // but we used the Split option to return even blank entries in the place holders)
                    // 
                    for (int i = tableNameParts.Length; i > 0; i--)
                    {
                        switch (i)
                        {
                            case 4:
                                serverName = tableNameParts[tableNameParts.Length - i];
                                break;
                            case 3:
                                databaseName = tableNameParts[tableNameParts.Length - i];
                                break;
                            case 2:
                                schemaName = tableNameParts[tableNameParts.Length - i];
                                break;
                            case 1:
                                objectName = tableNameParts[tableNameParts.Length - i];
                                break;
                            default:
                                break;
                        }
                    }
                }

                switch (namePart)
                {
                    case SyncHelperFM.ObjectNamePart.NAMEPART_SERVER:
                        retValue = serverName;
                        break;
                    case SyncHelperFM.ObjectNamePart.NAMEPART_DATABASE:
                        retValue = databaseName;
                        break;
                    case SyncHelperFM.ObjectNamePart.NAMEPART_SCHEMA:
                        retValue = schemaName;
                        break;
                    case SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME:
                        retValue = objectName;
                        break;
                }
            }

            return retValue;
        }
        #endregion Utility Methods

    }
}

