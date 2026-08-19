// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseSyncAgentFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the BaseSyncAgentFM type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses.Client
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Text;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.ServiceClasses;

    using Microsoft.Synchronization;
    using Microsoft.Synchronization.Data;

    public delegate void SyncProgressTableStartEventHandler(object sender, string targetName);

    /// <summary>
    /// The base sync agent fm.
    /// </summary>
    public abstract class BaseSyncAgentFM : SyncAgent
    {
        #region Attributes

        private bool isDisposed = false;

        private SyncScopeDO _SyncScope = null;
        private SyncContextFM _Context = null;

        private SyncClientConfigurationDO _ClientSyncConfig = null;

        private SyncSessionLogDO _CurrentSyncSession = null;

        private SyncSessionScopeLogDO _CurrentSyncSessionScopeLog = null;

        private string _SyncProgressCurrentTable = "";
        #endregion Attributes

        #region Event Notifications
        private event SyncProgressTableStartEventHandler _SyncProgressTableStartEvent = null;

        public event SyncProgressTableStartEventHandler SyncProgressTableStarted
        {
            add { _SyncProgressTableStartEvent += value; }
            remove { _SyncProgressTableStartEvent -= value; }
        }

        #endregion Event Notifications

        #region Properties

        /// <summary>
        /// Gets the context.
        /// </summary>
        public SyncContextFM Context
        {
            get { return this._Context; }
        }

        /// <summary>
        /// Gets a value indicating whether is batching.
        /// </summary>
        public bool IsBatching
        {
            get
            {
                return (null != this._Context) && this._Context.IsBatching;
            }
        }

        /// <summary>
        /// Gets or sets the records per batch.
        /// </summary>
        public int RecordsPerBatch
        {
            get
            {
                return (null != this._Context) ? this._Context.RecordsPerBatch : 0;
            }

            set
            {
                if (null != this._Context)
                {
                    this._Context.RecordsPerBatch = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the client sync config.
        /// </summary>
        protected SyncClientConfigurationDO ClientSyncConfig
        {
            get
            {
                return this._ClientSyncConfig;
            }

            set
            {
                if (value == this._ClientSyncConfig)
                {
                    return;
                }

                this._ClientSyncConfig = value;
            }
        }

        /// <summary>
        /// Gets or sets the current sync session.
        /// </summary>
        protected SyncSessionLogDO CurrentSyncSession
        {
            get
            {
                return this._CurrentSyncSession;
            }

            set
            {
                if (value == this._CurrentSyncSession)
                {
                    return;
                }

                this._CurrentSyncSession = value;
            }
        }

        /// <summary>
        /// Gets or sets the current sync session scope log entry
        /// </summary>
        protected SyncSessionScopeLogDO CurrentSyncSessionScopeLog
        {
            get
            {
                return this._CurrentSyncSessionScopeLog;
            }

            set
            {
                if (value == this._CurrentSyncSessionScopeLog)
                {
                    return;
                }

                this._CurrentSyncSessionScopeLog = value;
            }
        }

        /// <summary>
        /// Gets or sets the sync scope.
        /// </summary>
        protected SyncScopeDO SyncScope
        {
            get
            {
                return this._SyncScope;
            }

            set
            {
                if (value == this._SyncScope)
                {
                    return;
                }

                this._SyncScope = value;
            }
        }

        #endregion Properties

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSyncAgentFM"/> class.
        /// </summary>
        /// <param name="syncContext">
        /// The sync context.
        /// </param>
        /// <param name="clientSyncConfig">
        /// The client sync config.
        /// </param>
        /// <param name="syncSessionDO">
        /// The sync session do.
        /// </param>
        /// <param name="syncSessionScopeLogDo">
        /// The sync Session Log Detail DO.
        /// </param>
        protected BaseSyncAgentFM(SyncContextFM syncContext, SyncClientConfigurationDO clientSyncConfig, SyncSessionLogDO syncSessionDO, SyncSessionScopeLogDO syncSessionScopeLogDo)
            : base()
        {
            this.isDisposed = false;
            this._Context = syncContext;
            this._ClientSyncConfig = clientSyncConfig;
            this._CurrentSyncSession = syncSessionDO;
            this._CurrentSyncSessionScopeLog = syncSessionScopeLogDo;
        }
        #endregion Constructors/Destructors

        #region Abstract Methods
        protected abstract SyncProvider OnGetLocalSyncProvider();
        protected abstract ISyncServerProviderFM OnGetRemoteSyncProvider();
        protected abstract void OnInitializeSyncAgent();
        #endregion Abstract Methods

        #region Virtual Methods

        /// <summary>
        /// The on get local sync provider name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected virtual string OnGetLocalSyncProviderName()
        {
            return this._SyncScope.ID;
        }
        #endregion Virtual Methods

        #region Public Method

        /// <summary>
        /// The synchronize.
        /// </summary>
        /// <param name="syncScope">
        /// The sync scope.
        /// </param>
        /// <returns>
        /// The <see cref="SyncStatsFM"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An exception is thrown if the passed in <see cref="SyncScopeDO"/> instance is null.
        /// </exception>
        public SyncStatsFM Synchronize(SyncScopeDO syncScope)
        {
            // Make sure that a syncScope was provided, if it was, initialize the Agent and call the Base Synchronization method
            if (null == syncScope)
            {
                throw new ArgumentNullException("syncScope", @"syncScope must be specified.");
            }

            this._SyncScope = syncScope;

            return this.Synchronize();
        }
        #endregion Public Method

        #region Overrides

        /// <summary>
        /// The synchronize.
        /// </summary>
        /// <returns>
        /// The <see cref="SyncStatsFM"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// An exception is thrown if the <see cref="SyncScopeDO"/> property has not been set.
        /// </exception>
        public new SyncStatsFM Synchronize()
        {
            // Make sure that a syncScope was provided, if it was, initialize the Agent and call the Base Synchronization method
            if (null == this._SyncScope)
            {
                throw new Exception("SyncScope must be set prior to calling Synchronize.");
            }

            this.InitializeSynchronizationAgent();

            // SynchronizeMethod so that we can call it and translate the SyncStatistics
            SyncStatistics stats = base.Synchronize();

            return SyncHelperFM.ConvertSyncStatistics(stats);
        }
        #endregion Overrides

        #region SyncProgess.TotalChanges Kludge

        /// <summary>
        /// Int variable used to handle a bug in the SyncProgress TotalChanges value
        /// on outgoing changes. This variable will be used to accumulate the largest
        /// value of TotalChanges that is sent for a stage of SelectingChanges and will
        /// be used as the TotalChanges value in events that have stage ApplyingInserts
        /// or ApplyingUpdates or ApplyingDeletes. This is a known bug as is acknowledged at a link from here:
        /// http://social.microsoft.com/Forums/en/syncdevdiscussions/thread/b0211430-63d4-4446-a40b-6ca1856df6a8
        /// </summary>
        private int _kludgeSyncProgressTotalChanges;

        /// <summary>
        /// The kludge sync progress initialize.
        /// </summary>
        private void KludgeSyncProgressInitialize()
        {
            this._kludgeSyncProgressTotalChanges = 0;
        }

        /// <summary>
        /// The kludge sync progress selecting changes.
        /// </summary>
        /// <param name="pSyncStage">
        /// The p sync stage.
        /// </param>
        /// <param name="pScopeProgress">
        /// The p scope progress.
        /// </param>
        private void KludgeSyncProgressSelectingChanges(SyncStage pSyncStage, SyncGroupProgress pScopeProgress)
        {
            if (pSyncStage == SyncStage.DownloadingChanges
                || pSyncStage == SyncStage.UploadingChanges
                || pSyncStage == SyncStage.WritingMetadata
                || pSyncStage == SyncStage.ReadingMetadata
                || pSyncStage == SyncStage.ReadingSchema
                || pSyncStage == SyncStage.CreatingMetadata
                || pSyncStage == SyncStage.CreatingSchema
                || pSyncStage == SyncStage.DeletingMetadata)
                return;

            // Save the largest value in a selecting changes event
            if (pScopeProgress.TotalChanges > this._kludgeSyncProgressTotalChanges)
                this._kludgeSyncProgressTotalChanges = pScopeProgress.TotalChanges;
        }

        /// <summary>
        /// The kludge sync progress total changes.
        /// </summary>
        /// <param name="pSyncStage">
        /// The p sync stage.
        /// </param>
        /// <param name="pScopeProgress">
        /// The p scope progress.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int KludgeSyncProgressTotalChanges(SyncStage pSyncStage, SyncGroupProgress pScopeProgress)
        {
            // When going to server:
            // substitute the largest value found in a selecting changes event into any applying event.
            if (pSyncStage == SyncStage.UploadingChanges)
                return this._kludgeSyncProgressTotalChanges;

            // When going from to client, only the first file has the right number, meaning that
            // TotalChangesApplied <= TotalChanges. After that the TotalChanges never changes but
            // the TotalChangesApplied continues to be increment. Our kludge will be to add 20% to the
            // TotalChangesApplied. Obviously when sync is done, this needs to be fixed. This is done by
            // directly updating the control.

            if (pScopeProgress.TotalChangesApplied > pScopeProgress.TotalChanges)
            {
                if (this._kludgeSyncProgressTotalChanges < pScopeProgress.TotalChangesApplied)
                    this._kludgeSyncProgressTotalChanges = ((120 * pScopeProgress.TotalChangesApplied) / 100) + 1;

                return this._kludgeSyncProgressTotalChanges;
            }

            return pScopeProgress.TotalChanges;
        }
        #endregion SyncProgess.TotalChanges Kludge

        #region Private Initialization Methods

        /// <summary>
        /// The initialize synchronization agent.
        /// </summary>
        /// <exception cref="Exception">
        /// An exception will be thrown if we are unable to obtain an instance of a <see cref="ServerSyncProviderProxy"/>.
        /// </exception>
        protected void InitializeSynchronizationAgent()
        {
            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Verbose("BEGIN SyncAgentFM.InitializeSynchronizationAgent");
            }

            if (null != this._SyncScope.SyncScopeTables)
            {
                this._SyncScope.SyncScopeTables.Clear();
            }

            // Clear out any previous supported columns list so we don't construct a compounding list.
            if (null != this._Context.SupportedColumnsByTable)
            {
                this._Context.SupportedColumnsByTable.Clear();
            }

            // Clear out any previous table batch count sizes so we don't construct a compounding list.
            if (null != this._Context.SyncTableMaxBatchSegmentRowCountByTable)
            {
                this._Context.SyncTableMaxBatchSegmentRowCountByTable.Clear();
            }

			// Clear out any previous table first time sync options so we don't construct a compounding list.
			if (null != this._Context.SyncTableFirstTimeSyncOptionsByTable)
			{
				this._Context.SyncTableFirstTimeSyncOptionsByTable.Clear();
			}

			if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Verbose("Getting Scope Mappings");
            }

            // Retrieve the list of SyncTables that are mapped into the specified syncScope
            using (var dbi = new SyncTableToScopeMapDBI(this.Context.Security.UserID))
            {
                this._SyncScope.SyncScopeTables = dbi.GetList(this.Context.Security, this.SyncScope.IdentityGuid);
            }

            // Instantiate a client synchronization provider and specify it
            // as the local provider for this synchronization agent.
            // Note: When the local sync provider is being created, the 
            // _Context.SupportedColumnsByTable gets built up for each table.
            this.LocalProvider = this.OnGetLocalSyncProvider();

            // Hook Events
            var localProvider = this.LocalProvider;

            if (localProvider != null && localProvider is ISyncClientProviderFM)
            {
                ((ISyncClientProviderFM)this.LocalProvider).SyncProgress += new EventHandler<SyncProgressEventArgs>(this.OnSyncProgress);
                ((ISyncClientProviderFM)this.LocalProvider).ApplyingChanges += new EventHandler<ApplyingChangesEventArgs>(this.OnApplyingChanges);
                ((ISyncClientProviderFM)this.LocalProvider).ApplyChangeFailed += new EventHandler<ApplyChangeFailedEventArgs>(this.OnApplyChangeFailed);
                ((ISyncClientProviderFM)this.LocalProvider).ChangesSelected += new EventHandler<ChangesSelectedEventArgs>(this.OnChangesSelected);
                ((ISyncClientProviderFM)this.LocalProvider).ChangesApplied += new EventHandler<ChangesAppliedEventArgs>(this.OnChangesApplied);
            }

            ISyncServerProviderFM serviceProxy = this.OnGetRemoteSyncProvider();

            if (null == serviceProxy)
            {
                throw new Exception("Server SyncProvider missing.");
            }

            this.RemoteProvider = new ServerSyncProviderProxy(serviceProxy);

            this.OnInitializeSyncAgent();

            SyncTracer.Verbose("Adding Global SyncParameters");

            // This is how we will provide additional runtime information to the syncproviders to be used by the framework.
            // NOTE: The ClientID in the SyncFramework represents the FM "Node ID".  This ID is automatically retrieved by the 
            // Client SyncProvider.  However; we can pass in a value that the engine will return if needed.

            // Add the request type to the parameter list so the data extractors can tell if we're performing an initial synchronization or regular incremental.
            this.Configuration.SyncParameters.Add(
                SyncParamsFM.SYNC_REQUEST_TYPE_PARAMETER,
                (int)this.Context.RequestType);

            // Synchronization is being done incrementally for each site in the Site Hierarchy.  We need to register the Current SiteID to the 
            // SyncParameters list so that we can use it throughout the Synchronization Process.  This Agent will never represent more than one Site.
            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_PARAMETER, this._Context.CurrentSiteID);

            if (this._Context.CurrentSiteGuid.HasValue)
            {
                SyncTracer.Verbose(1, "Adding Sync Context Site Guid: {0}", this._Context.CurrentSiteGuid.Value);

                this.Configuration.SyncParameters.Add(
                    SyncParamsFM.SyncContextSiteGuidName, this._Context.CurrentSiteGuid.Value);
            }
            else
            {
                SyncTracer.Verbose(1, "Adding Sync Context Site Guid: N/A (First time sync?)");
                this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_PARAMETER, Guid.Empty);
            }

            // For FuelsManager, we should have already contacted the Enterprise Server and obtained it's serverID.  If we don't have it at this point,
            // something is seriously wrong.
            if (null == this._Context || (null != this._Context && this._Context.ServerID == Guid.Empty))
            {
                string err =
                    @"Synchronization session error.  The ServerID has not been obtained by the synchronization context.";

                SyncTracer.Error(err);
                throw new Exception(err);
            }

            SyncTracer.Verbose(1, "Adding Sync Server ID: {0}", this._Context.ServerID);

            byte[] serverIdBinary = this._Context.ServerID.ToByteArray();
            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_SERVER_ID_PARAMETER, this._Context.ServerID);
            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_SERVER_ID_BINARY_PARAMETER, serverIdBinary);

            StringBuilder siteGuidList = new StringBuilder();
            StringBuilder siteIDList = new StringBuilder();

            // Now Add a comma separated list of the SiteID's and SiteGuids that will follow the CurrentSiteID (Child Sites)
            if (this._Context.SiteSynchronizationList.Count > 0)
            {
                foreach (SiteClass sc in this._Context.SiteSynchronizationList)
                {
                    if (siteGuidList.Length > 0)
                    {
                        siteGuidList.Append(",");
                    }

                    siteGuidList.Append(string.Format("{0}", sc.SiteGuid.ToString()));

                    if (siteIDList.Length > 0)
                    {
                        siteIDList.Append(",");
                    }

                    siteIDList.Append(string.Format("{0}", sc.ID));
                }
            }

            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_GUID_LIST_PARAMETER, siteGuidList.ToString());
            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_CONTEXT_SITE_ID_LIST_PARAMETER, siteIDList.ToString());

            if (this._Context.UseDateRangeSynchronization)
            {
                this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, this._Context.StartDateRange.ToString());
                this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, this._Context.EndDateRange.ToString());
            }
            else
            {
                this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_START_DATERANGE_PARAMETER, DateTimeOffset.MinValue.ToString());
                this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_END_DATERANGE_PARAMETER, DateTimeOffset.MinValue.ToString());
            }

            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_FILTER_BY_DATERANGE_PARAMETER, this._Context.UseDateRangeSynchronization);
            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_SUPPORTED_COLUMNS_PARAMETER, string.Empty);

            this.Configuration.SyncParameters.Add(
                SyncParamsFM.SYNC_MAX_CLIENT_ANCHOR_PARAMETER,
                this._Context.MaxClientSyncAnchor);

            this.Configuration.SyncParameters.Add(
                SyncParamsFM.SYNC_MAX_SERVER_ANCHOR_PARAMETER,
                this._Context.MaxEnterpriseSyncAnchor);

            // Add in a list of supported columns for each table included in this session.
            foreach (var tableColumns in this._Context.SupportedColumnsByTable)
            {
                StringBuilder syncScopeColumns = new StringBuilder();

                foreach (SyncTableToScopeMapColumnDO columnInfo in tableColumns.Value)
                {
                    if (syncScopeColumns.Length > 0)
                    {
                        syncScopeColumns.Append(",");
                    }

                    syncScopeColumns.Append(string.Format("|{0}|", columnInfo.ColumnName));
                }

                var tableNameOnly = SyncHelperFM.GetNamePartFromTableName(tableColumns.Key, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME);

                this.Configuration.SyncParameters.Add(
                    string.Format(
                        "{0}_{1}",
                        SyncParamsFM.SYNC_SUPPORTED_COLUMNS_PARAMETER,
                        tableNameOnly),
                    syncScopeColumns.ToString());
            }

            // Add in a list of max batch sizes for each table included in this session.
            foreach (var tableKey in this._Context.SyncTableMaxBatchSegmentRowCountByTable.Keys)
            {
                int maxBatchSize = this._Context.SyncTableMaxBatchSegmentRowCountByTable[tableKey];

                var tableNameOnly = SyncHelperFM.GetNamePartFromTableName(tableKey, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME);

                this.Configuration.SyncParameters.Add(
                    string.Format(
                        "{0}_{1}",
                        SyncParamsFM.SYNC_BATCH_SIZE_PARAMETER,
                        tableNameOnly),
                    maxBatchSize);
            }

			// Add in a list of first time sync options for each table included in this session.
			foreach (var tableKey in this._Context.SyncTableFirstTimeSyncOptionsByTable.Keys)
			{
				int firstTimeOption = this._Context.SyncTableFirstTimeSyncOptionsByTable[tableKey];

				var tableNameOnly = SyncHelperFM.GetNamePartFromTableName(tableKey, SyncHelperFM.ObjectNamePart.NAMEPART_OBJECTNAME);

				this.Configuration.SyncParameters.Add(
					string.Format(
						"{0}_{1}",
						SyncParamsFM.SYNC_FIRST_TIME_SYNC_OPTION,
						tableNameOnly),
					firstTimeOption);
			}

			// Depending on which step we're executing, we should set some control parameters that will be used to help minimize the amount of
			// unnecessary data extraction during the various phases of synchronization.
			bool performDeletes = this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL || this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_DELETE || this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_DELETE_CONFLICT;
            bool performInserts = this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL || this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE || this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT;

            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_BYPASS_DELETE_EXTRACTION_PARAMETER, (!performDeletes) ? 1 : 0);
            this.Configuration.SyncParameters.Add(SyncParamsFM.SYNC_BYPASS_INSERT_UPDATE_EXTRACTION_PARAMETER, (!performInserts) ? 1 : 0);

            this._CurrentSyncSessionScopeLog.StartDate = DateTimeOffset.Now;

            ISyncSessionScopeLogs sessionScopeLogs = new SyncSessionScopeLogs();
            sessionScopeLogs.Modify(this._Context.Security, this._CurrentSyncSessionScopeLog);
        }
        #endregion Private Initialization Methods

        #region SyncAgent Event Handlers

        public event EventHandler<SyncProgressEventArgsFM> SyncProgress;
        public event EventHandler<ApplyChangeFailedEventArgs> ApplyChangeFailed;
        public event EventHandler<ApplyingChangesEventArgs> ApplyingChanges;
        public event EventHandler<ChangesSelectedEventArgs> ChangesSelected;
        public event EventHandler<ChangesAppliedEventArgs> ChangesApplied;

        /// <summary>
        /// Intercept the Synchronization Framework's Progress Event <see cref="SyncProgressEventArgs"/> and convert it to a <see cref="SyncProgressEventArgsFM"/>.
        /// </summary>
        /// <param name="sender">
        /// The originator of the SyncProgress event.
        /// </param>
        /// <param name="e">
        /// An instance of a <see cref="SyncProgressEventArgs"/> containing the current synchronization progress.
        /// </param>
        /// <remarks>
        /// The synchronization engine was originally intended to hide the details of the Microsoft SyncFramework by providing a FuelsManager set of corresponding objects.
        /// In hindsight, it might be easier to simply allow these objects to propagate through.
        /// </remarks>
        protected virtual void OnSyncProgress(object sender, SyncProgressEventArgs e)
        {
            this.KludgeSyncProgressSelectingChanges(e.SyncStage, e.GroupProgress);
            int totalChanges = this.KludgeSyncProgressTotalChanges(e.SyncStage, e.GroupProgress);
            int changes = e.GroupProgress.TotalChangesApplied;

            // When this changes, send a notification
            if (!this._SyncProgressCurrentTable.Equals(e.TableProgress.TableName))
            {
                this._SyncProgressCurrentTable = e.TableProgress.TableName;

                if (null != this._SyncProgressTableStartEvent)
                {
                    this._SyncProgressTableStartEvent(this, this._SyncProgressCurrentTable);
                }
            }

            if (this.SyncProgress != null)
            {
                this.SyncProgress(sender, this.GetSynchronizationProgressFromSyncProgress(e, totalChanges, changes));
            }
        }

        /// <summary>
        /// The on apply change failed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnApplyChangeFailed(object sender, ApplyChangeFailedEventArgs e)
        {
            if (this.ApplyChangeFailed != null)
            {
                this.ApplyChangeFailed(sender, e);
            }
        }

        /// <summary>
        /// The on applying changes.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnApplyingChanges(object sender, ApplyingChangesEventArgs e)
        {
            if (this.ApplyingChanges != null)
            {
                this.ApplyingChanges(sender, e);
            }
        }

        /// <summary>
        /// The on changes selected.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnChangesSelected(object sender, ChangesSelectedEventArgs e)
        {
            if (this.ChangesSelected != null)
            {
                this.ChangesSelected(sender, e);
            }
        }

        /// <summary>
        /// The on changes applied.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnChangesApplied(object sender, ChangesAppliedEventArgs e)
        {
            if (this.ChangesApplied != null)
            {
                this._CurrentSyncSessionScopeLog.TableCount = e.GroupMetadata.TablesMetadata.Count;

                this._CurrentSyncSessionScopeLog.TableSuccessCount += 0;
                this._CurrentSyncSessionScopeLog.TableErrorCount += 0;

                this._CurrentSyncSessionScopeLog.TotalChangesCount += e.Context.GroupProgress.TotalChanges;

                this._CurrentSyncSessionScopeLog.TotalChangesFailedCount += e.Context.GroupProgress.TotalChangesFailed;

                this._CurrentSyncSessionScopeLog.TotalChangesAppliedCount +=
                    e.Context.GroupProgress.TotalChangesApplied;

                this._CurrentSyncSessionScopeLog.TotalChangesPendingCount +=
                    e.Context.GroupProgress.TotalChangesPending;

                this._CurrentSyncSessionScopeLog.TotalDeleteCount += e.Context.GroupProgress.TotalDeletes;
                this._CurrentSyncSessionScopeLog.TotalInsertCount += e.Context.GroupProgress.TotalInserts;
                this._CurrentSyncSessionScopeLog.TotalUpdateCount += e.Context.GroupProgress.TotalUpdates;

                this._CurrentSyncSessionScopeLog.EndDate = DateTimeOffset.Now;

                ISyncSessionScopeLogs sessionScopeLogs = new SyncSessionScopeLogs();
                sessionScopeLogs.Modify(this._Context.Security, this._CurrentSyncSessionScopeLog);

                this.ChangesApplied(sender, e);
            }
        }
        #endregion SyncAgent Event Handlers

        #region Private Helper Methods
        /// <summary>
        /// Converts the Microsoft Synchronization Framework Progress Event Args into a generic SynchronizationProgressEventArgs instance.
        /// </summary>
        /// <param name="progressEventArgs">
        /// </param>
        /// <param name="maxProgress">
        /// The Max Progress.
        /// </param>
        /// <param name="currentProgress">
        /// The Current Progress.
        /// </param>
        /// <returns>
        /// </returns>
        private SyncProgressEventArgsFM GetSynchronizationProgressFromSyncProgress(SyncProgressEventArgs progressEventArgs, int maxProgress, int currentProgress)
        {
            SYNCSTAGE stage = SyncHelperFM.ConvertSyncStage(progressEventArgs.SyncStage);

            SyncTableMetadataFM tableMeta = new SyncTableMetadataFM(progressEventArgs.TableMetadata.TableName);
            tableMeta.SynchronizationDirection = SyncHelperFM.ConvertSyncDirection(progressEventArgs.TableMetadata.SyncDirection);
            tableMeta.LastReceivedAnchor = progressEventArgs.TableMetadata.LastReceivedAnchor.Anchor;
            tableMeta.LastSentAnchor = progressEventArgs.TableMetadata.LastSentAnchor.Anchor;

            SyncTableProgressFM tableProgress = new SyncTableProgressFM(progressEventArgs.TableProgress.TableName);
            tableProgress.ChangesApplied = progressEventArgs.TableProgress.ChangesApplied;
            tableProgress.ChangesFailed = progressEventArgs.TableProgress.ChangesFailed;
            tableProgress.ChangesPending = progressEventArgs.TableProgress.ChangesPending;

            foreach (SyncConflict conflict in progressEventArgs.TableProgress.Conflicts)
            {
                tableProgress.Conflicts.Add(new SyncConflictFM(SyncHelperFM.ConvertSyncConflictType(conflict.ConflictType), SyncHelperFM.ConvertSyncStage(conflict.SyncStage)));
            }

            tableProgress.Deletes = progressEventArgs.TableProgress.Deletes;
            tableProgress.Inserts = progressEventArgs.TableProgress.Inserts;
            tableProgress.RowIndex = progressEventArgs.TableProgress.RowIndex;
            tableProgress.TotalChanges = progressEventArgs.TableProgress.TotalChanges;
            tableProgress.Updates = progressEventArgs.TableProgress.Updates;

            SyncGroupMetadataFM groupMeta = new SyncGroupMetadataFM(progressEventArgs.GroupMetadata.GroupName);
            groupMeta.BatchCount = progressEventArgs.GroupMetadata.BatchCount;

            if (null != progressEventArgs.GroupMetadata.MaxAnchor)
            {
                groupMeta.MaxAnchor = progressEventArgs.GroupMetadata.MaxAnchor.Anchor;
            }

            if (null != progressEventArgs.GroupMetadata.NewAnchor)
            {
                groupMeta.NewAnchor = progressEventArgs.GroupMetadata.NewAnchor.Anchor;
            }

            SyncGroupProgressFM groupProgress = new SyncGroupProgressFM(progressEventArgs.GroupMetadata.GroupName);
            groupProgress.TotalChanges = progressEventArgs.GroupProgress.TotalChanges;
            groupProgress.TotalChangesApplied = progressEventArgs.GroupProgress.TotalChangesApplied;
            groupProgress.TotalChangesFailed = progressEventArgs.GroupProgress.TotalChangesFailed;
            groupProgress.TotalChangesPending = progressEventArgs.GroupProgress.TotalChangesPending;
            groupProgress.TotalDeletes = progressEventArgs.GroupProgress.TotalDeletes;
            groupProgress.TotalInserts = progressEventArgs.GroupProgress.TotalInserts;
            groupProgress.TotalUpdates = progressEventArgs.GroupProgress.TotalUpdates;

            return new SyncProgressEventArgsFM(tableMeta, tableProgress, groupMeta, groupProgress, stage, maxProgress, currentProgress);
        }
        #endregion Private Helper Methods

        #region Static Support Methods
        /// <summary>
        /// Add the table: specify a synchronization direction of
        /// Bidirectional, and that an existing table should be dropped.
        /// </summary>
        /// <param name="tableName">
        /// The Table Name.
        /// </param>
        /// <param name="syncGroup">
        /// The Sync Group.
        /// </param>
        /// <returns>
        /// The <see cref="SyncTable"/>.
        /// </returns>
        protected SyncTable AddSyncTable(string tableName, SyncGroup syncGroup)
        {
            return this.AddSyncTable(tableName, syncGroup, SyncDirection.Bidirectional);
        }

        /// <summary>
        /// The add sync table.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="syncGroup">
        /// The sync group.
        /// </param>
        /// <param name="pSyncDirection">
        /// The p sync direction.
        /// </param>
        /// <returns>
        /// The <see cref="SyncTable"/>.
        /// </returns>
        protected SyncTable AddSyncTable(string tableName, SyncGroup syncGroup, SyncDirection pSyncDirection)
        {
            SyncTable syncTable = new SyncTable(tableName);
            syncTable.CreationOption = TableCreationOption.UseExistingTableOrFail;
            syncTable.SyncGroup = syncGroup;
            syncTable.SyncDirection = pSyncDirection;

            return syncTable;
        }
        #endregion Static Support Methods

        #region Disposable Interface Pattern Implementation

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.isDisposed = true;
            }
            
            base.Dispose(disposing);
        }
        #endregion Disposable Interface Pattern Implementation
    }
}

