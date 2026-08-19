// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncControllerFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses.SyncClasses.Client
{
    using System;
    using System.Collections.Generic;
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
    /// This class is the main entry point into the Fuels Manager synchronization service platform.  The <see cref="SyncScopeControllerFM"/> uses the information
    /// specified within the passed in <see cref="SyncContextFM"/> to coordinate the synchronization sequence starting at the specified Site or Site Group.
    /// </summary>
    /// <remarks>
    /// The <see cref="SyncScopeControllerFM"/> communicates with the remote server to identify all the sites that need to be included in the synchronization session.
    /// This class is also responsible for creating / closing a synchronization session (not FuelsManager Sessions) that is used for the duration of the synchronization
    /// session.
    /// </remarks>
    public class SyncScopeControllerFM : IDisposable
    {
        #region Static Fields

        /// <summary>
        /// The event log.
        /// </summary>
        private static readonly FMEventLog eventLog = new FMEventLog();

        #endregion Static Fields

        #region Fields

        /// <summary>
        /// Contains an instance of a <see cref="SyncContextFM"/> class which contains the synchronization parameters that should be used during synchronization.
        /// </summary>
        private readonly SyncContextFM _SyncContext;


		/// <summary>
        /// The _ has errors.
        /// </summary>
        private bool _HasErrors;

        /// <summary>
        /// The _ last error message.
        /// </summary>
        private string _LastErrorMessage = string.Empty;


        /// <summary>
        /// The _ site synchronization list.
        /// </summary>
        private SiteSyncList _SiteSynchronizationList = new SiteSyncList();

        /// <summary>
        /// The _ sync service channel factory.
        /// </summary>
        private FMChannelFactory<IEnterpriseSynchronization> _SyncServiceChannelFactory;

		private Dictionary<string, string> _OfflineFileListForClient = new Dictionary<string, string>();

        private Dictionary<string, string> _OfflineFileListForServer = new Dictionary<string, string>();

        private bool isDisposed = false;

        private DateTimeOffset syncDateTimeOffset = DateTimeOffset.Now;

        #endregion Fields

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncScopeControllerFM"/> class.
        /// </summary>
        /// <param name="syncContext">
        /// The p sync context.
        /// </param>
        public SyncScopeControllerFM(SyncContextFM syncContext)
        {
			this._SyncContext = syncContext;
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


        public bool IsDisposed
        {
            get
            {
                return (this.isDisposed);
            }
        }

        #endregion Properties

        #region Public Methods and Operators

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
        ///     Using the information in the <see cref="SyncContextFM" />
        /// </summary>
		private void InitializeEnterpriseServerEndpoint(SyncClientConfigurationDO clientSyncConfig)
        {
            try
            {
				string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingTypeConfigKey];
                if (string.IsNullOrEmpty(syncServiceBindingType))
                {
                    throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding missing
                }

                string syncServiceBindingConfiguration =
					ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingConfigurationConfigKey];

                this._SyncServiceChannelFactory =
                    FMSyncChannelHelper.SyncChannelFactory<IEnterpriseSynchronization>(
                        clientSyncConfig, 
                        clientSyncConfig.EnterpriseURL);
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
		/// Synchronizes the scope.
		/// </summary>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="syncSessionLog">The synchronize session log.</param>
		/// <param name="syncScope">The synchronize scope.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <param name="siteID">The site identifier.</param>
		/// <returns>number of changes synchronized</returns>
		public (bool, SYNCSINGLEPASSPHASE) SynchronizeScope(SyncClientConfigurationDO clientSyncConfig, SyncSessionLogDO syncSessionLog, SyncScopeDO syncScope, Guid? siteGuid, string siteID)
		{
			this.InitializeEnterpriseServerEndpoint(clientSyncConfig);

			var syncSessionScopeLog = this.GetSyncSessionScopeLog(syncSessionLog, syncScope, siteGuid);

			try
			{
				if (SyncTracer.IsInfoEnabled())
				{
					SyncTracer.Info(string.Empty);
					SyncTracer.Info("BEGIN Scope Synchronization : \"{0}\""+ ((siteGuid.HasValue && siteGuid.Value != Guid.Empty) ? " Site : \"{1}\"" : " Site : N/A"), syncScope.ID, siteID);
				}
 
				// Need to cycle through all the lookup synchronization groups automatically.
				using (var agent = new SyncAgentFM(this._SyncContext, clientSyncConfig, syncSessionLog, syncSessionScopeLog))
				{
					agent.SyncProgressTableStarted += this.Agent_SyncProgressTableStartEvent;
					agent.SyncProgress += this.Agent_SyncProgress;
					agent.ApplyingChanges += this.Agent_ApplyingChanges;
					agent.ApplyChangeFailed += this.Agent_ApplyChangeFailed;
					agent.ChangesApplied += this.Agent_ChangesApplied;
					agent.ChangesSelected += this.Agent_ChangesSelected;
					agent.SessionProgress += this.Agent_SessionProgress;

					var syncStatsFM = agent.Synchronize(syncScope);

					if (syncStatsFM.UploadChangesFailed > 0
					|| syncStatsFM.DownloadChangesFailed > 0)
					{
						this.UpdateSyncSessionScopeLogState(syncSessionScopeLog, SYNCSESSIONSTATE.SYNCED);
						this.UpdateSyncSessionScopeLogStatus(syncSessionScopeLog, SYNCSESSIONSTATUS.COMPCON);
					}
					else
					{
						this.UpdateSyncSessionScopeLogState(syncSessionScopeLog, SYNCSESSIONSTATE.SYNCED);
						this.UpdateSyncSessionScopeLogStatus(syncSessionScopeLog, SYNCSESSIONSTATUS.COMPOK);
					}
				}

				if (SyncTracer.IsInfoEnabled())
				{
					SyncTracer.Info("COMPLETED Scope Synchronization : \"{0}\"" + ((siteGuid.HasValue && siteGuid.Value != Guid.Empty) ? " Site : \"{1}\"" : " Site : N/A"), syncScope.ID, siteID);
					SyncTracer.Info(string.Empty);
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

				this.UpdateSyncSessionScopeLogState(syncSessionScopeLog, SYNCSESSIONSTATE.END);
				this.UpdateSyncSessionScopeLogStatus(syncSessionScopeLog, SYNCSESSIONSTATUS.FAILED);
                
				if (null != this._SyncFailedEvent)
				{
					this._SyncFailedEvent.Invoke(this, this._LastErrorMessage);
				}

				throw;
			}

			return (this.SyncContext.MaxBatchSegmentRowCountEncountered, this.SyncContext.SyncSinglePassPhase);
		}

		/// <summary>
		/// The get sync session scope log entry
		/// </summary>
		/// <param name="syncSessionLog">The synchronize session log.</param>
		/// <param name="syncScope">The sync Scope.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <returns>
		/// The <see cref="SyncSessionScopeLogDO" />.
		/// </returns>
		private SyncSessionScopeLogDO GetSyncSessionScopeLog(SyncSessionLogDO syncSessionLog, SyncScopeDO syncScope, Guid? siteGuid)
		{
			var syncSessionScopeLogs = new SyncSessionScopeLogs();

			return syncSessionScopeLogs.GetByCompositeKey(
				this._SyncContext.Security,
				syncSessionLog.IdentityGuid,
				siteGuid.HasValue ? siteGuid.Value : Guid.Empty,
				syncScope.ID);
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