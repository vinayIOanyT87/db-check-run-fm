// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientSyncProviderFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ClientSyncProviderFM type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses.Client
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.ServiceClasses;

    using Microsoft.Synchronization.Data;

    public partial class ClientSyncProviderFM : SqlExpressClientSyncProvider
    {
        #region Attributes
        /// <summary>
        /// The is disposed.
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// The session to sql index.
        /// </summary>
        private long? sessionToSqlIndex = null;

        #endregion Attributes

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSyncProviderFM"/> class.
        /// </summary>
        /// <param name="syncScope">
        /// The sync scope.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="syncSessionLog">
        /// The sync session.
        /// </param>
        /// <param name="syncSessionScopeLog">
        /// The sync session scope log entry.
        /// </param>
        public ClientSyncProviderFM(SyncScopeDO syncScope, SyncContextFM context, SyncSessionLogDO syncSessionLog, SyncSessionScopeLogDO syncSessionScopeLog)
            : base()
        {
            this.isDisposed = false;
            this.sessionToSqlIndex = null;
            this.InitializeSynchronization(syncScope, context, syncSessionLog, syncSessionScopeLog);
        }
        #endregion Constructors/Destructors

        #region Properties
        #endregion Properties

        /// <summary>
        /// Update the FuelsManager session with the current SQL SPID and the synchronization node id of the server so that any changes
        /// that are applied to the synchronized data will be associated with the server.
        /// </summary>
        /// <param name="syncSession">
        /// The sync Session.
        /// </param>
        protected override void OnBeginningTransaction(SyncSession syncSession)
        {
            base.OnBeginningTransaction(syncSession);
        }

        #region Private Synchronization Methods

        /// <summary>
        /// The initialize synchronization.
        /// </summary>
        /// <param name="syncScope">
        /// The sync scope.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="syncSessionLog">
        /// The sync session log entry
        /// </param>
        /// <param name="syncSessionScopeLog">
        /// The sync Session Scope Log.
        /// </param>
        private void InitializeSynchronization(SyncScopeDO syncScope, SyncContextFM context, SyncSessionLogDO syncSessionLog, SyncSessionScopeLogDO syncSessionScopeLog)
        {
            SqlConnection clientConn = SyncDBI.CreateClientConnection();

            // We don't want to close the connection because it's used throughout the entire synchronization process.
            clientConn.Open();
            
            this.Connection = clientConn;
            this.SqlChangeTracking = false; // May need to adjust this concept.
            this.SyncScope = syncScope;
            this.SyncSessionLog = syncSessionLog;
            this.SyncSessionScopeLog = syncSessionScopeLog;
            this.Context = context;

            this.MapSqlConnection();

            // Reset the batch size, this will be set based on the tables mapped to this scope below.
            this.Context.RecordsPerBatch = 0;

            this.SetAnchors();

            // For each Table that's mapped to this provider, create and register SyncAdapters.
            foreach (SyncTableToScopeMapDO mappedTable in syncScope.SyncScopeTables)
            {
                this.SyncAdapters.Add(SyncClientProviderHelperFM.GetSyncAdapter(this.Context.Security, mappedTable, this.Context, clientConn));

                SyncTableDO syncTable = SyncProviderHelperFM.GetSyncTable(
                    this.Context.Security,
                    mappedTable.SyncTableGuid);

                string tableName = string.IsNullOrEmpty(mappedTable.ClientTableNameOverride)
                                        ? syncTable.TableName
                                        : mappedTable.ClientTableNameOverride;

                // We need to update the syncContextFM so that it can provide this list to the enterprise.
                if (!context.SyncTableMaxBatchSegmentRowCountByTable.ContainsKey(tableName))
                {
                    context.SyncTableMaxBatchSegmentRowCountByTable.Add(tableName, ((mappedTable.MaxBatchSegmentRowCount.HasValue) ? mappedTable.MaxBatchSegmentRowCount.Value : 0));
                }

					// We need to update the syncContextFM so that it can provide this list to the enterprise.
					if (!context.SyncTableFirstTimeSyncOptionsByTable.ContainsKey(tableName))
					{
						context.SyncTableFirstTimeSyncOptionsByTable.Add(tableName, ((mappedTable.FirstTimeSyncOption.HasValue) ? mappedTable.FirstTimeSyncOption.Value : 0));
					}

				}

				// Hook Events
				this.ChangesSelected += new EventHandler<ChangesSelectedEventArgs>(SyncProviderHelperFM.ChangesSelected);
            this.ChangesApplied += new EventHandler<ChangesAppliedEventArgs>(SyncProviderHelperFM.ChangesApplied);
            this.ApplyChangeFailed += new EventHandler<ApplyChangeFailedEventArgs>(this.ApplyChangeFailedHandler);
        }

        /// <summary>
        /// The apply change failed handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void ApplyChangeFailedHandler(object sender, ApplyChangeFailedEventArgs e)
        {
            // Now call the main handler with the current Security Context.
            SyncClientProviderHelperFM.ApplyChangeFailed(this.Context, this.SyncSessionScopeLog, sender, e);
        }

        /// <summary>
        /// This method is called during pre-synchronization to binds IDbCommand properties to various Commands instances.
        /// </summary>
        /// <remarks>
        /// We are currently utilizing 2 sets of Anchor points.  Since we process Inserts/Updates separate from Deletes, we need to be able to 
        /// track their anchors independently so that we can "go back" and pick up the deletes.
        /// </remarks>
        private void SetAnchors()
        {
            this.SelectNewAnchorCommand = SyncProviderHelperFM.CreateAnchorCommand(false, SyncProviderHelperFM.ClientNodeType);  // Client side doesn't perform batching (for now)

            this.SelectTableReceivedAnchorCommand = SyncProviderHelperFM.CreateSelectTableReceivedAnchorCommand();
            this.UpdateTableReceivedAnchorCommand = SyncProviderHelperFM.CreateUpdateTableReceivedAnchorCommand();

            this.SelectTableReceivedAnchor2Command = SyncProviderHelperFM.CreateSelectTableReceivedAnchor2Command();
            this.UpdateTableReceivedAnchor2Command = SyncProviderHelperFM.CreateUpdateTableReceivedAnchor2Command();

            this.SelectTableSentAnchorCommand = SyncProviderHelperFM.CreateSelectTableSentAnchorCommand();
            this.UpdateTableSentAnchorCommand = SyncProviderHelperFM.CreateUpdateTableSentAnchorCommand();

            this.SelectTableSentAnchor2Command = SyncProviderHelperFM.CreateSelectTableSentAnchor2Command();
            this.UpdateTableSentAnchor2Command = SyncProviderHelperFM.CreateUpdateTableSentAnchor2Command();
        }

        /// <summary>
        /// Updates the current synchronization session scope log record updated statistics
        /// </summary>
        /// <param name="syncSessionLogDetailDo">
        /// The sync Session Scope Log entry to update.
        /// </param>
        private void UpdateSyncSessionScopeLog(SyncSessionScopeLogDO syncSessionLogDetailDo)
        {
            ISyncSessionScopeLogs sessionLogDetails = new SyncSessionScopeLogs();
            sessionLogDetails.Modify(this.Context.Security, syncSessionLogDetailDo);
        }
        #endregion Private Synchronization Methods

        #region Disposable Pattern Implementation

        /// <summary>
        /// Disposes this Client Sync Provider instance 
        /// </summary>
        /// <param name="disposing">True if explicit finalization, false if through GC</param>
        protected override void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            try
            {
                if (disposing)
                {
							if (this.Connection != null
							&& this.Connection.State != ConnectionState.Closed)
							{
								this.UnMapSqlConnection();
								this.Connection.Close();
							}
					}
				}
            finally
            {
                this.isDisposed = true;
                base.Dispose(disposing);
            }
        }

		/// <summary>
		/// Maps the SQL connection SPID to the FuelsManager Synchronization Session
		/// </summary>
		private void MapSqlConnection()
        {
            this.sessionToSqlIndex = ConsolidatedDAClass.MapSqlConnectionToSession(
                this.Context.Security,
                (SqlConnection)this.Connection);

            if (this.sessionToSqlIndex.HasValue && SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Info(
                    string.Format(
                        "Map the current FMSession with the SQL SPID of the synchronization connection. Index: {0}, Provider: {1}",
                        this.sessionToSqlIndex.Value,
                        this.Context.CurrentSyncScopeID));
            }
        }

		/// <summary>
		/// Un-Maps the SQL connection SPID from the FuelsManager Synchronization Session
		/// </summary>
		private void UnMapSqlConnection()
		{
			if (this.sessionToSqlIndex.HasValue)
			{
				ConsolidatedDAClass.UnMapSqlConnectionFromSession((SqlConnection)this.Connection, this.sessionToSqlIndex.Value);

				if (SyncTracer.IsVerboseEnabled())
				{
					SyncTracer.Info(
					string.Format(
					"UnMap the current FMSession from the SQL SPID. Index: {0}, Provider: {1}",
					this.sessionToSqlIndex.Value,
					this.Context.CurrentSyncScopeID));
				}

				this.sessionToSqlIndex = null;
			}
		}

      #endregion Disposable Pattern Implementation

    }
}

