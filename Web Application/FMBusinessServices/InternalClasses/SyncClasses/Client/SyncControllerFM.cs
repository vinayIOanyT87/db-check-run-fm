// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncControllerFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses.SyncClasses.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Text;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.ServiceClasses;

    using Microsoft.Synchronization;
    using Microsoft.Synchronization.Data;

    /// <summary>
    /// The session progress event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    public delegate void SessionProgressEventHandler(object sender, SessionProgressEventArgs args);

    /// <summary>
    /// This class is the main entry point into the Fuels Manager synchronization service platform.  The <see cref="SyncControllerFM"/> uses the information
    /// specified within the passed in <see cref="SyncContextFM"/> to coordinate the synchronization sequence starting at the specified Site or Site Group.
    /// </summary>
    /// <remarks>
    /// The <see cref="SyncControllerFM"/> communicates with the remote server to identify all the sites that need to be included in the synchronization session.
    /// This class is also responsible for creating / closing a synchronization session (not FuelsManager Sessions) that is used for the duration of the synchronization
    /// session.
    /// </remarks>
    public class SyncControllerFM : IDisposable
    {
        #region Static Fields

        /// <summary>
        /// The event log.
        /// </summary>
        private static readonly FMEventLog eventLog = new FMEventLog();

        #endregion Static Fields

        #region Fields

        private SyncSessionLogDO _SyncSessionLog;

        /// <summary>
        /// Contains an instance of a <see cref="SyncContextFM"/> class which contains the synchronization parameters that should be used during synchronization.
        /// </summary>
        private readonly SyncContextFM _SyncContext;

        /// <summary>
        /// Contains an instance of a <see cref="SyncClientConfigurationDO"/> class that provides the client synchronization settings for the local node.
        /// </summary>
        private SyncClientConfigurationDO _ClientSyncConfig;

        /// <summary>
        /// Count of how many sites have been synchronized.
        /// </summary>
        private int _CurrentSiteCount;

        /// <summary>
        /// The _ has errors.
        /// </summary>
        private bool _HasErrors;

        /// <summary>
        /// The _ last error message.
        /// </summary>
        private string _LastErrorMessage = string.Empty;

        /// <summary>
        /// The _ max site count.
        /// </summary>
        private int _MaxSiteCount;

        /// <summary>
        /// The _ site synchronization list.
        /// </summary>
        private SiteSyncList _SiteSynchronizationList = new SiteSyncList();

        /// <summary>
        /// The _ sync service channel factory.
        /// </summary>
        private FMChannelFactory<IEnterpriseSynchronization> _SyncServiceChannelFactory;

        /// <summary>
        /// The _ total sites synchronized.
        /// </summary>
        private int _TotalSitesSynchronized;

        private Dictionary<string, string> _OfflineFileListForClient = new Dictionary<string, string>();

        private Dictionary<string, string> _OfflineFileListForServer = new Dictionary<string, string>();

        private bool isDisposed = false;

        private DateTimeOffset syncDateTimeOffset = DateTimeOffset.Now;

        #endregion Fields

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncControllerFM"/> class.
        /// </summary>
        /// <param name="syncContext">
        /// The p sync context.
        /// </param>
        public SyncControllerFM(SyncContextFM syncContext)
        {
            this.SysStopFlag = false;
            this.UserStopFlag = false;

            this._SyncSessionLog = new SyncSessionLogDO();

            this._SyncContext = syncContext;
            this._SyncContext.SessionType = SYNCSESSION.DEFAULT;
        }

        #endregion Constructors and Destructors

        #region Public Events

        /// <summary>
        /// The session progress event.
        /// </summary>
        public event SessionProgressEventHandler SessionProgressEvent
        {
            add
            {
                this._SessionProgressEvent += value;
            }

            remove
            {
                this._SessionProgressEvent -= value;
            }
        }

        /// <summary>
        /// The sync completed event.
        /// </summary>
        public event SyncCompletedEventHandler SyncCompletedEvent
        {
            add
            {
                this._SyncCompletedEvent += value;
            }

            remove
            {
                this._SyncCompletedEvent -= value;
            }
        }

        /// <summary>
        /// The sync failed event.
        /// </summary>
        public event SyncFailedEventHandler SyncFailedEvent
        {
            add
            {
                this._SyncFailedEvent += value;
            }

            remove
            {
                this._SyncFailedEvent -= value;
            }
        }

        /// <summary>
        /// The sync progress event.
        /// </summary>
        public event SyncProgressEventHandler SyncProgressEvent
        {
            add
            {
                this._SyncProgressEvent += value;
            }

            remove
            {
                this._SyncProgressEvent -= value;
            }
        }

        /// <summary>
        /// The sync table change event.
        /// </summary>
        public event SyncProgressTableStartEventHandler SyncProgressTableStarted
        {
            add
            {
                this._SyncProgressTableStartEvent += value;
            }

            remove
            {
                this._SyncProgressTableStartEvent -= value;
            }
        }

        #endregion Public Events

        #region Events

        /// <summary>
        /// The _ session progress event.
        /// </summary>
        private event SessionProgressEventHandler _SessionProgressEvent = null;

        /// <summary>
        /// The _ sync completed event.
        /// </summary>
        private event SyncCompletedEventHandler _SyncCompletedEvent = null;

        /// <summary>
        /// The _ sync failed event.
        /// </summary>
        private event SyncFailedEventHandler _SyncFailedEvent = null;

        /// <summary>
        /// The _ sync progress event.
        /// </summary>
        private event SyncProgressEventHandler _SyncProgressEvent = null;

        /// <summary>
        /// Each time the synchronization process changes to the next table, this even is fired.
        /// </summary>
        private event SyncProgressTableStartEventHandler _SyncProgressTableStartEvent = null;

        #endregion Events

        #region Public Properties

        private bool SysStopFlag { get; set; }

        /// <summary>
        /// Gets or sets the list files that contains the client changes keyed by ScopeId
        /// </summary>
        public Dictionary<string, string> OfflineFileListForClient
        {
            get
            {
                return this._OfflineFileListForClient;
            }

            set
            {
                this._OfflineFileListForClient = value;
            }
        }

        /// <summary>
        /// Gets or sets the list files that contains the Server changes keyed by ScopeId
        /// </summary>
        public Dictionary<string, string> OfflineFileListForServer
        {
            get
            {
                return this._OfflineFileListForServer;
            }

            set
            {
                this._OfflineFileListForServer = value;
            }
        }

        /// <summary>
        /// Gets or sets the client sync config.
        /// </summary>
        public SyncClientConfigurationDO ClientSyncConfig
        {
            get
            {
                return this._ClientSyncConfig;
            }

            set
            {
                if (value.Equals(this._ClientSyncConfig))
                {
                    return;
                }

                this._ClientSyncConfig = value;
            }
        }

        /// <summary>
        /// Gets or sets the current site count.
        /// </summary>
        public int CurrentSiteCount
        {
            get
            {
                return this._CurrentSiteCount;
            }

            set
            {
                this._CurrentSiteCount = value;
            }
        }

        /// <summary>
        /// Gets the current sync context.
        /// </summary>
        public SyncContextFM SyncContext
        {
            get
            {
                return this._SyncContext;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether has errors.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return this._HasErrors;
            }

            set
            {
                this._HasErrors = value;
            }
        }

        /// <summary>
        /// Gets or sets the last error message.
        /// </summary>
        public string LastErrorMessage
        {
            get
            {
                return this._LastErrorMessage;
            }

            set
            {
                if (value.Equals(this._LastErrorMessage))
                {
                    return;
                }

                this._LastErrorMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the max site count.
        /// </summary>
        public int MaxSiteCount
        {
            get
            {
                return this._MaxSiteCount;
            }

            set
            {
                this._MaxSiteCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the total sites synchronized.
        /// </summary>
        public int TotalSitesSynchronized
        {
            get
            {
                return this._TotalSitesSynchronized;
            }

            set
            {
                this._TotalSitesSynchronized = value;
            }
        }

        public bool SyncStopped
        {
            get
            {
                return (this.SysStopFlag || this.UserStopFlag);
            }
        }

        public bool UserStopFlag { get; set; }

        public bool IsDisposed
        {
            get
            {
                return (this.isDisposed);
            }
        }

        #endregion Properties

        #region Public Methods and Operators

        /// <summary>
        /// The synchronize databases.
        /// </summary>
        /// <param name="clientSyncConfig">
        /// The p client sync config.
        /// </param>
        /// <returns>
        /// The <see cref="SYNCSESSIONSTATUS"/>.
        /// </returns>
        public SYNCSESSIONSTATUS SynchronizeDatabases(SyncClientConfigurationDO clientSyncConfig)
        {
            return this.Synchronize(SyncConstants.DEFAULT_PROFILE_COMPLETE, clientSyncConfig);
        }

        /// <summary>
        /// The synchronize databases.
        /// </summary>
        /// <param name="syncProfileId">
        /// The p sync profile id.
        /// </param>
        /// <param name="clientSyncConfig">
        /// The p client sync config.
        /// </param>
        /// <returns>
        /// The <see cref="SYNCSESSIONSTATUS"/>.
        /// </returns>
        public SYNCSESSIONSTATUS SynchronizeDatabases(string syncProfileId, SyncClientConfigurationDO clientSyncConfig)
        {
            return this.Synchronize(syncProfileId, clientSyncConfig);
        }

        /// <summary>
        /// The synchronize offline databases.
        /// </summary>
        /// <param name="clientSyncConfig">
        /// The client sync config.
        /// </param>
        /// <param name="outputFilename">
        /// The output filename.
        /// </param>
        /// <returns>
        /// The <see cref="SYNCSESSIONSTATUS"/>.
        /// </returns>
        public SYNCSESSIONSTATUS SynchronizeOfflineDatabases(SyncClientConfigurationDO clientSyncConfig, ref string outputFilename)
        {
            outputFilename = "Zip Archive Of Changes";
            return this.Synchronize(SyncConstants.DEFAULT_PROFILE_COMPLETE, clientSyncConfig);
        }

        /// <summary>
        /// The synchronize offline databases.
        /// </summary>
        /// <param name="syncProfileId">
        /// The sync profile id.
        /// </param>
        /// <param name="clientSyncConfig">
        /// The client sync config.
        /// </param>
        /// <param name="outputFilename">
        /// The output filename.
        /// </param>
        /// <returns>
        /// The <see cref="SYNCSESSIONSTATUS"/>.
        /// </returns>
        public SYNCSESSIONSTATUS SynchronizeOfflineDatabases(string syncProfileId, SyncClientConfigurationDO clientSyncConfig, ref string outputFilename)
        {
            outputFilename = "Zip Archive Of Changes";
            return this.Synchronize(syncProfileId, clientSyncConfig);
        }

        #endregion Public Methods and Operators

        #region Methods

        /// <summary>
        /// The agent_ apply change failed.
        /// </summary>
        /// <param name="sender">
        /// The p sender.
        /// </param>
        /// <param name="eventArgs">
        /// The p event args.
        /// </param>
        protected void Agent_ApplyChangeFailed(object sender, ApplyChangeFailedEventArgs eventArgs)
        {
            if (SyncTracer.IsVerboseEnabled() == false
                && eventArgs.Conflict.ConflictType != ConflictType.ErrorsOccurred)
            {
                DataTable conflictingServerChange = eventArgs.Conflict.ServerChange;
                DataTable conflictingClientChange = eventArgs.Conflict.ClientChange;
                int serverColumnCount = conflictingServerChange.Columns.Count;
                int clientColumnCount = conflictingClientChange.Columns.Count;
                var clientRowAsString = new StringBuilder();
                var serverRowAsString = new StringBuilder();

                for (int i = 0; i < clientColumnCount; i++)
                {
                    clientRowAsString.Append(conflictingClientChange.Rows[0][i] + " | ");
                }

                for (int i = 0; i < serverColumnCount; i++)
                {
                    serverRowAsString.Append(conflictingServerChange.Rows[0][i] + " | ");
                }

                SyncTracer.Warning(1, "CONFLICT DETECTED FOR CLIENT {0}", eventArgs.Session.ClientId);
                SyncTracer.Warning(2, "** Client change **");
                SyncTracer.Warning(2, clientRowAsString.ToString());
                SyncTracer.Warning(2, "** Server change **");
                SyncTracer.Warning(2, serverRowAsString.ToString());
            }

            this._HasErrors = true;
        }

        /// <summary>
        /// The agent_ applying changes.
        /// </summary>
        /// <param name="sender">
        /// The p sender.
        /// </param>
        /// <param name="eventArgs">
        /// The p event args.
        /// </param>
        protected void Agent_ApplyingChanges(object sender, ApplyingChangesEventArgs eventArgs)
        {
            if (!SyncTracer.IsVerboseEnabled())
            {
                return;
            }

            foreach (DataTable table in eventArgs.Changes.Tables)
            {
                SyncTracer.Info(1, "Table: {0}", table.TableName);

                foreach (DataRow changedRow in table.Rows)
                {
                    StringBuilder rowData = new StringBuilder();
                    
                    foreach (DataColumn column in table.Columns)
                    {
	                    if (changedRow.RowState == DataRowState.Deleted)
	                    {
		                    rowData.Append(string.Format("{0} = {1}, ", column.ColumnName, changedRow[column, DataRowVersion.Original]));
	                    }
	                    else
	                    {
		                    rowData.Append(string.Format("{0} = {1}, ", column.ColumnName, changedRow[column]));
	                    }
                    }

                    SyncTracer.Info(2, "RowData: {0} ", rowData.ToString());
                }
            }
        }

        /// <summary>
        /// The agent_ changes applied.
        /// </summary>
        /// <param name="sender">
        /// The p sender.
        /// </param>
        /// <param name="eventArgs">
        /// The p event args.
        /// </param>
        protected void Agent_ChangesApplied(object sender, ChangesAppliedEventArgs eventArgs)
        {
            if (!SyncTracer.IsVerboseEnabled())
            {
                return;
            }

            SyncTracer.Info(1, "** Changes Applied **");
        }

        /// <summary>
        /// The agent_ changes selected.
        /// </summary>
        /// <param name="sender">
        /// The p sender.
        /// </param>
        /// <param name="eventArgs">
        /// The p event args.
        /// </param>
        protected void Agent_ChangesSelected(object sender, ChangesSelectedEventArgs eventArgs)
        {
            if (!SyncTracer.IsVerboseEnabled())
            {
                return;
            }

            SyncTracer.Info(1, "** Changes Selected **");
            SyncTracer.Info(2, "Table Count: {0}", eventArgs.Context.DataSet.Tables.Count);
        }

        /// <summary>
        /// The agent_ session progress.
        /// </summary>
        /// <param name="sender">
        /// The p sender.
        /// </param>
        /// <param name="eventArgs">
        /// The p event args.
        /// </param>
        protected void Agent_SessionProgress(object sender, SessionProgressEventArgs eventArgs)
        {
            if (null != this._SessionProgressEvent)
            {
                this._SessionProgressEvent.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        /// The agent_ sync progress.
        /// </summary>
        /// <param name="sender">
        /// The p sender.
        /// </param>
        /// <param name="eventArgs">
        /// The p event args.
        /// </param>
        protected void Agent_SyncProgress(object sender, SyncProgressEventArgsFM eventArgs)
        {
            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Info(1, "** Sync Progress **");
                SyncTracer.Info(2, "Stage: {0}", eventArgs.SyncStage);
                SyncTracer.Info(3, "Group Info: {0}", eventArgs.GroupProgress.GroupName);
                SyncTracer.Info(
                    4, 
                    "Table Info: {0}, Total Changes: {1}, Changes Applied: {2}", 
                    eventArgs.TableProgress.TableName, 
                    eventArgs.TableProgress.TotalChanges, 
                    eventArgs.TableProgress.Updates);
            }

            if (null != this._SyncProgressEvent)
            {
                this._SyncProgressEvent.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        /// This method handles the table start event which is fired when a new table is being synchronized.
        /// </summary>
        /// <param name="sender">
        /// The p sender.
        /// </param>
        /// <param name="tableName">
        /// Name of the table that synchronization is working on.
        /// </param>
        protected void Agent_SyncProgressTableStartEvent(object sender, string tableName)
        {
            if (null != this._SyncProgressTableStartEvent)
            {
                this._SyncProgressTableStartEvent(this, tableName);
            }
        }

        /// <summary>
        /// The cleanup synchronization session.
        /// </summary>
        private void CleanupSynchronizationSession()
        {
            if (null != this._SiteSynchronizationList)
            {
                this._SiteSynchronizationList.Clear();
            }
        }

        /// <summary>
        /// Using the information in the <see cref="SyncContextFM"/>
        /// </summary>
        /// <returns>
        /// The <see cref="SiteSyncList"/>.
        /// </returns>
        private SiteSyncList GetLocalSiteSynchronizationList()
        {
            // Get the local list of Sites that need to be synchronized.
            // For a new install, we might have a SiteID but we won't be able to resolve the SiteClass.  In this scenario
            // the localSyncList will contain a single Site entry with just the ID.  
            // The server will take the SiteID in the SyncContext and locate the SiteClass for the SiteID we've provided.
            var localSyncList = new SiteSyncList();

            var sitesClass = new SitesClass();
            SiteClass siteClass = sitesClass.GetByID(this._SyncContext.Security, this._SyncContext.SiteID);

            if (siteClass.IdentityGuid != Guid.Empty)
            {
                if (!this._SyncContext.SiteGuid.HasValue || (this._SyncContext.SiteGuid == Guid.Empty))
                {
                    this._SyncContext.SiteGuid = siteClass.IdentityGuid;
                }

                localSyncList = sitesClass.EnumerateSiteSynchronizationListBySiteSQL(
                    this._SyncContext.Security, this._SyncContext.SiteGuid.Value);
            }
            else
            {
                siteClass.ID = this._SyncContext.SiteID;
                localSyncList.Add(0, siteClass);
            }

            return localSyncList;
        }

        /// <summary>
        /// The get remote site synchronization list.
        /// </summary>
        /// <param name="localSyncList">
        /// The p local sync list.
        /// </param>
        /// <returns>
        /// The <see cref="SiteSyncList"/>.
        /// </returns>
        private SiteSyncList GetRemoteSiteSynchronizationList(SiteSyncList localSyncList)
        {
            this._SiteSynchronizationList = FMChannelHelper.MakeCall(
                this._SyncServiceChannelFactory, 
                (x) => x.GetSynchronizationSiteList(localSyncList, this._SyncContext));
            return this._SiteSynchronizationList;
        }

        /// <summary>
        ///     Using the information in the <see cref="SyncContextFM" />
        /// </summary>
        private void InitializeEnterpriseServerEndpoint()
        {
            try
            {
                string syncServiceBindingType = ConfigurationManager.AppSettings["syncEnterpriseBusinessBindingType"];
                if (string.IsNullOrEmpty(syncServiceBindingType))
                {
                    throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding missing
                }

                string syncServiceBindingConfiguration =
                    ConfigurationManager.AppSettings["syncEnterpriseBusinessBindingConfiguration"];

                this._SyncServiceChannelFactory =
                    FMSyncChannelHelper.SyncChannelFactory<IEnterpriseSynchronization>(
                        this._ClientSyncConfig, 
                        syncServiceBindingType, 
                        syncServiceBindingConfiguration, 
                        this._ClientSyncConfig.EnterpriseURL);
            }
            catch (Exception eX)
            {
                // PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
                eventLog.WriteEntry(
                    string.Format("Unable to bind to the Remote FM Business Services: {0}", eX.Message), 
                    FMEventLogEntryType.Error);
            }
        }

        /// <summary>
        /// This method is responsible for starting, processing and closing a synchronization session.  This method identifies the list
        /// of SiteIDs that need to be included in the synchronization process, identifies which synchronization scopes should be synchronized for each
        /// Site and instructs SyncAgents to perform the specific synchronization tasks.  Upon completion of the synchronization process, this method
        /// will close out the sync session record (not the user session).
        /// </summary>
        /// <param name="syncProfileId">
        /// The ID of the synchronization profile that we have been instructed to use for synchronization.
        /// </param>
        /// <param name="clientSyncConfig">
        /// The client synchronization configuration information.  This information is used to establish connections to the remote
        /// synchronization server and provides authentication information.
        /// </param>
        /// <returns>
        /// A <see cref="SYNCSESSIONSTATUS"/> of COMP or COMPOK if synchronization completed, otherwise; false if the process was disrupted due to errors.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// An exception will be thrown if a Sync Profile ID was not provided.
        /// </exception>
        private SYNCSESSIONSTATUS Synchronize(string syncProfileId, SyncClientConfigurationDO clientSyncConfig)
        {
            this.UserStopFlag = false;
            this.SysStopFlag = false;

            SYNCSESSIONSTATUS syncStatus = SYNCSESSIONSTATUS.NEW;

            this._ClientSyncConfig = clientSyncConfig;

            if (string.IsNullOrEmpty(syncProfileId))
            {
                throw new ArgumentException(@"A SyncProfile ID Must be Specified.", "syncProfileId");
            }

            this._SyncContext.CurrentSyncProfileID = syncProfileId;

            try
            {
                this._HasErrors = false;
                this._LastErrorMessage = string.Empty;

                this.StartSyncSession(syncProfileId);

                SiteSyncList localSyncList = this.GetLocalSiteSynchronizationList();

                if (this._SyncContext.TransferType == SYNCTRANSFERTYPE.ONLINE)
                {
                    this.InitializeEnterpriseServerEndpoint();

                    this._SiteSynchronizationList = this.GetRemoteSiteSynchronizationList(localSyncList);
                }
                else
                {
                    this._SiteSynchronizationList = localSyncList;
                }

                var syncProfiles = new SyncProfiles();
                SyncProfileDO syncProfile = syncProfiles.GetById(this._SyncContext.Security, syncProfileId);

                if (null != syncProfile)
                {
                    var scopes = new SyncScopes();
                    SyncScopeCollection syncScopes = scopes.EnumerateExt(this._SyncContext.Security, syncProfile);

                    // Create the sync session log detail entries up-front for status reporting.
                    foreach (SyncScopeDO syncScope in syncScopes)
                    {
                        switch (syncScope.SyncScopeTypeIndex)
                        {
                            case SYNCSCOPETYPE.GLOBAL:
                                // Create a single SyncSessionScopeLog entry.
                                this.CreateSyncSessionScopeLog(syncScope, SYNCSITETYPE.REFERENCE, null);
                                break;
                            case SYNCSCOPETYPE.REFERENCE_ONLY:
                            case SYNCSCOPETYPE.HOSTED_ONLY:
                            case SYNCSCOPETYPE.BOTH:
                                SiteCollectionClass siteList = this._SiteSynchronizationList.EnumerateInsertUpdateSynchronizationList(syncScope.SyncScopeTypeIndex);

                                // For each site specified in the SiteList, create a single SyncSessionScopeLog entry.
                                foreach (SiteClass site in siteList)
                                {
                                    bool isRootSite = site.SiteID.Equals(
                                        this.SyncContext.SiteID, StringComparison.InvariantCultureIgnoreCase);

                                    SYNCSITETYPE siteSyncType = isRootSite ? SYNCSITETYPE.ROOT : SYNCSITETYPE.REFERENCE;

                                    if (siteSyncType != SYNCSITETYPE.ROOT)
                                    {
                                        siteSyncType = (syncScope.SyncScopeTypeIndex == SYNCSCOPETYPE.HOSTED_ONLY)
                                                           ? SYNCSITETYPE.HOSTED
                                                           : SYNCSITETYPE.REFERENCE;
                                    }

                                    this.CreateSyncSessionScopeLog(syncScope, siteSyncType, site.SiteGuid);
                                }

                                break;
                        }
                    }

					if (!this.SyncStopped)
					{
						syncScopes.Reverse();
						this.UpdateSessionState(SYNCSESSIONSTATE.PROCESSDEL);
						this.SynchronizeDeletes(syncScopes);
					}

					if (!this.SyncStopped)
                    {
						syncScopes.Reverse();
						this.UpdateSessionState(SYNCSESSIONSTATE.PROCESSINSUPD);
                        this.SynchronizeInsertUpdates(syncScopes);
                    }

                }
                else
                {
                    eventLog.WriteEntry(
                        string.Format("Unable to locate specified SyncProfile: {0}", syncProfileId), 
                        FMEventLogEntryType.Warning);
                }

                this.UpdateSessionState(SYNCSESSIONSTATE.POSTSYNC);

                this.CleanupSynchronizationSession();

                if (this.UserStopFlag)
                {
                    syncStatus = SYNCSESSIONSTATUS.USERSTOP;
                }
                else if (this.SysStopFlag)
                {
                    syncStatus = SYNCSESSIONSTATUS.SYSSTOP;
                }
                else
                {
                    if (!this._HasErrors)
                    {
                        syncStatus = SYNCSESSIONSTATUS.COMPOK;
                    }
                    else
                    {
                        syncStatus = SYNCSESSIONSTATUS.COMPCON;
                    }
                }

                this._SyncSessionLog.SyncSessionStatusIndex = syncStatus;
            }
            catch (Exception eX)
            {
				string msg =
					 string.Format(
						 "Synchronization exception encountered: {0}",eX.Message);

				eventLog.WriteEntry(msg, FMEventLogEntryType.Error);
				SyncHelperFM.WriteErrorAlarmAndEvent(this._SyncContext.Security, msg);

				syncStatus = SYNCSESSIONSTATUS.FAILED;
				UpdateSessionStatus(syncStatus);
            }
            finally
            {
                if (this._SyncSessionLog.SyncSessionStateIndex != SYNCSESSIONSTATE.END)
                {
                    this.EndSyncSession();
                }
            }

            return syncStatus;
        }

        /// <summary>
        /// The synchronize deletes.
        /// </summary>
        /// <param name="syncScopes">
        /// A collection of synchronization scope definitions that should be synchronized.
        /// </param>
        private void SynchronizeDeletes(SyncScopeCollection syncScopes)
        {
            if (null != syncScopes)
            {
                // If a scope is Global then it isn't specific to any particular site and should only be executed once.
                foreach (SyncScopeDO syncScope in syncScopes)
                {
                    if (this.SyncStopped)
                    {
                        break;
                    }

                    switch (syncScope.SyncScopeTypeIndex)
                    {
                        case SYNCSCOPETYPE.GLOBAL:
                            this.SynchronizeScope(syncScope, SYNCCONTROLLERSTEP.PROCESS_DELETE);
                            break;
                        case SYNCSCOPETYPE.REFERENCE_ONLY:
                        case SYNCSCOPETYPE.HOSTED_ONLY:
                        case SYNCSCOPETYPE.BOTH:
                            SiteCollectionClass siteList =
                                this._SiteSynchronizationList.EnumerateDeleteSynchronizationList(
                                    syncScope.SyncScopeTypeIndex);

                            this.SynchronizeForSites(syncScope, siteList, SYNCCONTROLLERSTEP.PROCESS_DELETE);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Process all of the Inserts / Updates for the Sites listed in the Site Synchronization List.
        /// </summary>
        /// <param name="syncScopes">
        /// A collection of synchronization scope definitions that should be synchronized.
        /// </param>
        private void SynchronizeInsertUpdates(SyncScopeCollection syncScopes)
        {
            if (null != syncScopes)
            {
                // If a scope is Global then it isn't specific to any particular site and should only be executed once.
                foreach (SyncScopeDO syncScope in syncScopes)
                {
                    if (this.SyncStopped)
                    {
                        break;
                    }

                    if (SyncTracer.IsVerboseEnabled())
                    {
                        SyncTracer.Verbose(string.Empty);
                        SyncTracer.Verbose("CURRENT Scope Synchronization: {0}", syncScope.ID);
                    }

                    switch (syncScope.SyncScopeTypeIndex)
                    {
                        case SYNCSCOPETYPE.GLOBAL:
                            this.SynchronizeScope(syncScope, SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE);
                            break;
                        case SYNCSCOPETYPE.REFERENCE_ONLY:
                        case SYNCSCOPETYPE.HOSTED_ONLY:
                        case SYNCSCOPETYPE.BOTH:
                            SiteCollectionClass siteList = this._SiteSynchronizationList.EnumerateInsertUpdateSynchronizationList(syncScope.SyncScopeTypeIndex);

                            this.SynchronizeForSites(syncScope, siteList, SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE);
                            break;
                    }
                }

                if (SyncTracer.IsVerboseEnabled())
                {
                    SyncTracer.Verbose("COMPLETED Scope Synchronization.");
                    SyncTracer.Verbose(string.Empty);
                }
            }
        }

        /// <summary>
        /// This method will initiate synchronization of the specified <see cref="SyncScopeDO"/> for each of the Site IDs specified in the SiteList.
        /// </summary>
        /// <param name="syncScope">
        /// An instance of the synchronization scope that needs to be synchronized.
        /// </param>
        /// <param name="siteList">
        /// A list of SiteIDs to synchronize.
        /// </param>
        /// <param name="controllerStep">
        /// This parameter indicates whether we are synchronizing the inserts/updates or the deletes for the specified scope.
        /// </param>
        private void SynchronizeForSites(SyncScopeDO syncScope, SiteCollectionClass siteList, SYNCCONTROLLERSTEP controllerStep)
        {
            SyncContextFM syncContext = this._SyncContext.Clone();

            this.MaxSiteCount = siteList.Count;
            this.CurrentSiteCount = 0;

            if (SyncTracer.IsInfoEnabled())
            {
                SyncTracer.Info("Synchronizing Site Level Scope: \"{0}\"", syncScope.ID);
                SyncTracer.Info(1, "Number of sites which will synchronize this scope: {0}", siteList.Count);
                SyncTracer.Info(string.Empty);
            }

            syncContext.SiteSynchronizationList.Clear();
            syncContext.SiteSynchronizationList.AddRange(siteList);

            // For each site specified in the SiteList, Synchronize the specified SyncScope.
            foreach (SiteClass sc in siteList)
            {
                syncContext.CurrentSiteID = sc.ID;
                syncContext.CurrentSiteGuid = sc.SiteGuid;
                syncContext.CurrentControllerStep = controllerStep;

                this.CurrentSiteCount++;

                var syncSessionLogDetail = this.GetSyncSessionScopeLog(syncScope, sc.SiteGuid);

                if (SyncTracer.IsInfoEnabled())
                {
                    SyncTracer.Info(2, "#{2}: Current Site ID/Guid: {0} / {1}", syncContext.CurrentSiteID, syncContext.CurrentSiteGuid, this.CurrentSiteCount);
                }

                try
                {
                    var completedArgs = new SyncCompletedEventArgsFM(
                        this._MaxSiteCount,
                        this._CurrentSiteCount,
                        this._TotalSitesSynchronized,
                        syncContext.CurrentSiteID);

                    using (
                        var agent = new SyncAgentFM(
                            syncContext,
                            this._ClientSyncConfig,
                            this._SyncSessionLog,
                            syncSessionLogDetail))
                    {
                        agent.SyncProgressTableStarted += this.Agent_SyncProgressTableStartEvent;
                        agent.SyncProgress += this.Agent_SyncProgress;
                        agent.ApplyingChanges += this.Agent_ApplyingChanges;
                        agent.ApplyChangeFailed += this.Agent_ApplyChangeFailed;
                        agent.ChangesApplied += this.Agent_ChangesApplied;
                        agent.ChangesSelected += this.Agent_ChangesSelected;
                        agent.SessionProgress += this.Agent_SessionProgress;

                        completedArgs.SyncStats = agent.Synchronize(syncScope);

                        if (completedArgs.SyncStats.UploadChangesFailed > 0
                            || completedArgs.SyncStats.DownloadChangesFailed > 0)
                        {
                            this.UpdateSyncSessionScopeLogState(syncSessionLogDetail, SYNCSESSIONSTATE.SYNCED);
                            this.UpdateSyncSessionScopeLogStatus(syncSessionLogDetail, SYNCSESSIONSTATUS.COMPCON);
                        }
                        else
                        {
                            this.UpdateSyncSessionScopeLogState(syncSessionLogDetail, SYNCSESSIONSTATE.SYNCED);
                            this.UpdateSyncSessionScopeLogStatus(syncSessionLogDetail, SYNCSESSIONSTATUS.COMPOK);
                        }

                        // this.OfflineFileListForClient.Add(syncScope.ID, agent.OfflineClientChangesFilename);

                        if (null != this._SyncCompletedEvent)
                        {
                            this._SyncCompletedEvent(this, completedArgs);
                        }
                    }
                }
                catch (Exception eX)
                {
                    this._HasErrors = true;
                    this._LastErrorMessage = string.Format("SyncAgent exception encountered: {0}", eX.Message);

                    if (SyncTracer.IsErrorEnabled())
                    {
                        SyncTracer.Error("***Sync Exception***: {0}", this._LastErrorMessage);
                    }

                    eventLog.WriteEntry(this._LastErrorMessage, FMEventLogEntryType.Error);

                    this.UpdateSyncSessionScopeLogState(syncSessionLogDetail, SYNCSESSIONSTATE.END);
                    this.UpdateSyncSessionScopeLogStatus(syncSessionLogDetail, SYNCSESSIONSTATUS.FAILED);

                    if (null != this._SyncFailedEvent)
                    {
                        this._SyncFailedEvent.Invoke(this, this._LastErrorMessage);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// This method is responsible for synchronizing a single synchronization scope and is typically not associated with a single Site or Site Group.
        /// </summary>
        /// <param name="syncScope">
        /// The scope definition to synchronize
        /// </param>
        /// <param name="pControllerStep">
        /// This parameter indicates whether we are synchronizing the inserts/updates or the deletes for the specified scope.
        /// </param>
        private void SynchronizeScope(SyncScopeDO syncScope, SYNCCONTROLLERSTEP pControllerStep)
        {
            SyncContextFM syncContext = this._SyncContext.Clone();

            // This core information is outside of the scope of any single Site
            syncContext.CurrentSiteID = string.Empty;
            syncContext.CurrentSiteGuid = Guid.Empty;
            syncContext.CurrentControllerStep = pControllerStep;

            var syncSessionScopeLog = this.GetSyncSessionScopeLog(syncScope, null);

            try
            {
                if (SyncTracer.IsInfoEnabled())
                {
                    SyncTracer.Info("Synchronizing Global Level Scope: \"{0}\"", syncScope.ID);
                }

                var completedArgs = new SyncCompletedEventArgsFM(
                    this._MaxSiteCount, this._CurrentSiteCount, this._TotalSitesSynchronized, syncContext.CurrentSiteID);

                // Need to cycle through all the lookup synchronization groups automatically.
                using (var agent = new SyncAgentFM(syncContext, this._ClientSyncConfig, this._SyncSessionLog, syncSessionScopeLog))
                {
                    agent.SyncProgressTableStarted += this.Agent_SyncProgressTableStartEvent;
                    agent.SyncProgress += this.Agent_SyncProgress;
                    agent.ApplyingChanges += this.Agent_ApplyingChanges;
                    agent.ApplyChangeFailed += this.Agent_ApplyChangeFailed;
                    agent.ChangesApplied += this.Agent_ChangesApplied;
                    agent.ChangesSelected += this.Agent_ChangesSelected;
                    agent.SessionProgress += this.Agent_SessionProgress;

                    completedArgs.SyncStats = agent.Synchronize(syncScope);

                    if (completedArgs.SyncStats.UploadChangesFailed > 0
                        || completedArgs.SyncStats.DownloadChangesFailed > 0)
                    {
                        this.UpdateSyncSessionScopeLogState(syncSessionScopeLog, SYNCSESSIONSTATE.SYNCED);
                        this.UpdateSyncSessionScopeLogStatus(syncSessionScopeLog, SYNCSESSIONSTATUS.COMPCON);
                    }
                    else
                    {
                        this.UpdateSyncSessionScopeLogState(syncSessionScopeLog, SYNCSESSIONSTATE.SYNCED);
                        this.UpdateSyncSessionScopeLogStatus(syncSessionScopeLog, SYNCSESSIONSTATUS.COMPOK);
                    }

                    // this.OfflineFileListForClient.Add(syncScope.ID, agent.OfflineClientChangesFilename);

                    if (null != this._SyncCompletedEvent)
                    {
                        this._SyncCompletedEvent(this, completedArgs);
                    }
                }
            }
            catch (Exception eX)
            {
                this._HasErrors = true;
                this._LastErrorMessage = string.Format("SyncAgent exception encountered: {0}", eX.Message);

                this.UpdateSyncSessionScopeLogState(syncSessionScopeLog, SYNCSESSIONSTATE.END);
                this.UpdateSyncSessionScopeLogStatus(syncSessionScopeLog, SYNCSESSIONSTATUS.FAILED);
                
                if (SyncTracer.IsErrorEnabled())
                {
                    SyncTracer.Error("***Sync Exception***: {0}", this._LastErrorMessage);
                }

                if (null != this._SyncFailedEvent)
                {
                    this._SyncFailedEvent.Invoke(this, this._LastErrorMessage);
                }
            }
        }

        /// <summary>
        /// Creates a synchronization session log record that can be used to track the progress of a new synchronization process.
        /// </summary>
        /// <param name="syncProfileId">
        /// The sync profile id.
        /// </param>
        private void StartSyncSession(string syncProfileId)
        {
            // Create an internal synchronization session that we can associate synchronization details, conflicts, errors and status information with.
            this._SyncSessionLog.IdentityGuid = this._SyncContext.ClientSessionToken;

            // Our synchronization session is tracked in a different table, but we'll use the same session token so that we can
            // close it out when the session ends.
            this._SyncSessionLog.StartDate = DateTimeOffset.Now;
            this._SyncSessionLog.SyncProfileID = syncProfileId;
            this._SyncSessionLog.RemoteNodeGuid = this._SyncContext.ServerID;
            this._SyncSessionLog.RemoteNodeMachineName = this._SyncContext.ServerName;
            this._SyncSessionLog.SyncAnchorMax = this._SyncContext.MaxEnterpriseSyncAnchor;
            this._SyncSessionLog.SyncRequestTypeIndex = this._SyncContext.RequestType;
            this._SyncSessionLog.SyncTransferTypeIndex = this._SyncContext.TransferType;

            if (this._SyncContext.UseDateRangeSynchronization)
            {
                this._SyncSessionLog.SyncDateRangeStart = this._SyncContext.StartDateRange;
                this._SyncSessionLog.SyncDateRangeEnd = this._SyncContext.EndDateRange;
            }
            else
            {
                this._SyncSessionLog.SyncDateRangeStart = null;
                this._SyncSessionLog.SyncDateRangeEnd = null;
            }

            this._SyncSessionLog.SyncSessionStatusIndex = SYNCSESSIONSTATUS.NEW;

            ISyncSessionLogs syncSessions = new SyncSessionLogs();
            syncSessions.Add(this._SyncContext.Security, this._SyncSessionLog);
        }

        /// <summary>
        /// Updates the current synchronization session log record with the final state of the synchronization process.
        /// </summary>
        private void EndSyncSession()
        {
            this._SyncSessionLog.EndDate = DateTimeOffset.Now;
            this._SyncSessionLog.SyncSessionStateIndex = SYNCSESSIONSTATE.END;

            ISyncSessionLogs syncSessions = new SyncSessionLogs();
            syncSessions.Modify(this._SyncContext.Security, this._SyncSessionLog);
        }

        /// <summary>
        /// Updates the current synchronization session log record with the current state of the synchronization process.
        /// </summary>
        /// <param name="sessionState">
        /// The session state.
        /// </param>
        /// <remarks>
        /// The state represents the answer to the question "What is the synchronization process doing?" or "Where is the synchronization process at?"
        /// </remarks>
        private void UpdateSessionState(SYNCSESSIONSTATE sessionState)
        {
            this._SyncSessionLog.SyncSessionStateIndex = sessionState;

            ISyncSessionLogs syncSessions = new SyncSessionLogs();
            syncSessions.Modify(this._SyncContext.Security, this._SyncSessionLog);
        }

        /// <summary>
        /// Updates the current synchronization session log record with the current status of the synchronization process.
        /// </summary>
        /// <param name="sessionStatus">
        /// The session status.
        /// </param>
        /// <remarks>
        /// The status represents the answer to the question "How is/did the synchronization process go?"
        /// </remarks>
        private void UpdateSessionStatus(SYNCSESSIONSTATUS sessionStatus)
        {
            this._SyncSessionLog.SyncSessionStatusIndex = sessionStatus;
	        if (sessionStatus == SYNCSESSIONSTATUS.FAILED)
	        {
		        this._SyncSessionLog.EndDate = DateTimeOffset.Now;
				this._SyncSessionLog.SyncSessionStateIndex = SYNCSESSIONSTATE.CLOSE;
			}

            ISyncSessionLogs syncSessions = new SyncSessionLogs();
            syncSessions.Modify(this._SyncContext.Security, this._SyncSessionLog);
        }

        /// <summary>
        /// The create sync session scope log entry
        /// </summary>
        /// <param name="syncScope">
        /// The sync Scope.
        /// </param>
        /// <param name="syncSiteType">
        /// The sync Site Type.
        /// </param>
        /// <param name="siteGuid">
        /// The site GUID.
        /// </param>
        private void CreateSyncSessionScopeLog(SyncScopeDO syncScope, SYNCSITETYPE? syncSiteType, Guid? siteGuid)
        {
            var syncSessionScopeLogDo = new SyncSessionScopeLogDO();
            syncSessionScopeLogDo.IdentityGuid = Guid.NewGuid();
            syncSessionScopeLogDo.SyncSessionLogGuid = this._SyncSessionLog.IdentityGuid;
            syncSessionScopeLogDo.SiteGuid = siteGuid.HasValue ? siteGuid.Value : Guid.Empty;
            syncSessionScopeLogDo.SyncScopeID = syncScope.ID;
            
            syncSessionScopeLogDo.SiteTypeIndex =
                (SYNCSITETYPE?)(syncSiteType.HasValue ? syncSiteType : (object)DBNull.Value);
            
            syncSessionScopeLogDo.SyncSessionStateIndex = SYNCSESSIONSTATE.QUEUED;
            syncSessionScopeLogDo.SyncSessionStatusIndex = SYNCSESSIONSTATUS.NEW;

            ISyncSessionScopeLogs sessionLogDetails = new SyncSessionScopeLogs();
            sessionLogDetails.Modify(this._SyncContext.Security, syncSessionScopeLogDo);

            return;
        }

        /// <summary>
        /// The get sync session scope log entry
        /// </summary>
        /// <param name="syncScope">
        /// The sync Scope.
        /// </param>
        /// <param name="siteGuid">
        /// The site GUID.
        /// </param>
        /// <returns>
        /// The <see cref="SyncSessionScopeLogDO"/>.
        /// </returns>
        private SyncSessionScopeLogDO GetSyncSessionScopeLog(SyncScopeDO syncScope, Guid? siteGuid)
        {
            SyncSessionScopeLogDO sessionScopeLogDo = null;

            ISyncSessionScopeLogs sessionLogDetails = new SyncSessionScopeLogs();
            sessionScopeLogDo = sessionLogDetails.GetByCompositeKey(
                this._SyncContext.Security,
                this._SyncSessionLog.IdentityGuid,
                siteGuid.HasValue ? siteGuid.Value : Guid.Empty,
                syncScope.ID);
        
            return sessionScopeLogDo;
        }

        /// <summary>
        /// Updates the current synchronization session scope log record updated statistics
        /// </summary>
        /// <param name="syncSessionLogDetailDo">
        /// The sync Session Scope Log entry to update
        /// </param>
        private void UpdateSyncSessionScopeLog(SyncSessionScopeLogDO syncSessionLogDetailDo)
        {
            ISyncSessionScopeLogs sessionLogDetails = new SyncSessionScopeLogs();
            sessionLogDetails.Modify(this._SyncContext.Security, syncSessionLogDetailDo);
        }

        /// <summary>
        /// Updates the current synchronization session log detail record with the current state of the synchronization process.
        /// </summary>
        /// <param name="syncSessionScopeLogDo">
        /// The sync Session Scope Log to update.
        /// </param>
        /// <param name="sessionState">
        /// The session state.
        /// </param>
        /// <remarks>
        /// The state represents the answer to the question "What is the synchronization process doing?" or "Where is the synchronization process at?"
        /// </remarks>
        private void UpdateSyncSessionScopeLogState(SyncSessionScopeLogDO syncSessionScopeLogDo, SYNCSESSIONSTATE sessionState)
        {
            syncSessionScopeLogDo.SyncSessionStateIndex = sessionState;

            ISyncSessionScopeLogs sessionLogDetails = new SyncSessionScopeLogs();
            sessionLogDetails.Modify(this._SyncContext.Security, syncSessionScopeLogDo);
        }

        /// <summary>
        /// Updates the current synchronization session log detail record with the current status of the synchronization process.
        /// </summary>
        /// <param name="syncSessionScopeLogDo">
        /// The sync Session Scope Log to update.
        /// </param>
        /// <param name="sessionStatus">
        /// The session status.
        /// </param>
        /// <remarks>
        /// The status represents the answer to the question "How is/did the synchronization process go?"
        /// </remarks>
        private void UpdateSyncSessionScopeLogStatus(SyncSessionScopeLogDO syncSessionScopeLogDo, SYNCSESSIONSTATUS sessionStatus)
        {
            syncSessionScopeLogDo.SyncSessionStatusIndex = sessionStatus;

            ISyncSessionScopeLogs sessionLogDetails = new SyncSessionScopeLogs();
            sessionLogDetails.Modify(this._SyncContext.Security, syncSessionScopeLogDo);
        }

        #endregion Methods

        #region IDisposable Interface Implementation

        /// <summary>
        /// Disposes this Client Sync Provider instance 
        /// </summary>
        /// <param name="disposing">True if explicit finalization, false if through GC</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                //
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Disposes this Client Sync Provider instance 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Interface Implementation
    }
}