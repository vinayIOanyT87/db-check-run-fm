// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncAgentFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncAgentFM type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses.Client
{
    using System;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.ServiceClasses;

    using Microsoft.Synchronization;
    using Microsoft.Synchronization.Data;

    /// <summary>
    /// The sync agent fm.
    /// </summary>
    public partial class SyncAgentFM : BaseSyncAgentFM
    {

        #region Attributes

        private bool isDisposed = false;

        private string offlineClientChangesFilename;

        private string offlineServerChangesFilename;

        #endregion Attributes

        #region Public Properties

        /// <summary>
        /// Gets or sets the offline file that contains the client changes
        /// </summary>
        public string OfflineClientChangesFilename
        {
            get
            {
                return this.offlineClientChangesFilename;
            }

            set
            {
                this.offlineClientChangesFilename = value;
            }
        }

        /// <summary>
        /// Gets or sets the offline file that contains the server changes
        /// </summary>
        public string OfflineServerChangesFilename
        {
            get
            {
                return this.offlineServerChangesFilename;
            }

            set
            {
                this.offlineServerChangesFilename = value;
            }
        }

        #endregion Public Properties

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncAgentFM"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
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
        public SyncAgentFM(SyncContextFM context, SyncClientConfigurationDO clientSyncConfig, SyncSessionLogDO syncSessionDO, SyncSessionScopeLogDO syncSessionScopeLogDo)
            : base(context, clientSyncConfig, syncSessionDO, syncSessionScopeLogDo)
        {
            this.isDisposed = false;
        }
        #endregion Constructors/Destructors

        #region Abstract Method Implementations

        /// <summary>
        /// The on get local sync provider.
        /// </summary>
        /// <returns>
        /// The <see cref="SyncProvider"/>.
        /// </returns>
        protected override SyncProvider OnGetLocalSyncProvider()
        {
            // Instantiate a client synchronization provider and specify it
            // as the local provider for this synchronization agent.
            return new ClientSyncProviderFM(this.SyncScope, this.Context, this.CurrentSyncSession, this.CurrentSyncSessionScopeLog);
        }

        /// <summary>
        /// The on get remote sync provider.
        /// </summary>
        /// <returns>
        /// The <see cref="ISyncServerProviderFM"/>.
        /// </returns>
        protected override ISyncServerProviderFM OnGetRemoteSyncProvider()
        {
            // Instantiate a server synchronization proxy and specify it
            // as the remote provider for this synchronization agent.
            if (this.Context.TransferType == SYNCTRANSFERTYPE.ONLINE)
            {
				this.Context.CurrentSyncScopeID = this.OnGetLocalSyncProviderName();
				ISyncServerProviderFM serviceProxy = new SyncClientServerProxyFM(this.ClientSyncConfig, this.Context, this.SyncScope);

                return serviceProxy;
            }
            else
            {
                string type = "ALL";

                if (this.Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE)
                {
                    type = "INSUPD";
                }
                else if (this.Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT)
                {
                    type = "INSUPDCON";
                }
                else if (this.Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_DELETE)
                {
                    type = "DEL";
                }
                else if (this.Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_DELETE_CONFLICT)
                {
                    type = "DELCON";
                }

                this.offlineClientChangesFilename = string.Format("SYNC_{0}_{1}_{2}_{3}.csyncvcef", this.Context.ContextCreatedDate.ToString("yyyyMMdd_HHmmss"), this.OnGetLocalSyncProviderName(), type, this.CurrentSyncSession.SiteID);

				this.Context.CurrentSyncScopeID = this.OnGetLocalSyncProviderName();
				ISyncServerProviderFM serviceProxy = new SyncClientServerProxyFMOffline(this.offlineClientChangesFilename, this.ClientSyncConfig, this.Context, this.SyncScope);

                return serviceProxy;
            }
        }

        /// <summary>
        /// The on initialize sync agent.
        /// </summary>
        protected override void OnInitializeSyncAgent()
        {
            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Verbose("Adding SyncGroup");
            }

            // Create our SyncGroup so that everything is synchronized together
            // Note, this is part of the MS SyncFramework
            SyncGroup syncGroup = new SyncGroup(this.SyncScope.ID);

            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Verbose("Adding SyncTableToScopeMaps to SyncGroup");
            }

            // Add the table: specify a synchronization direction of
            // Bidirectional, and that an existing table should be preserved since we are using SQL Server Change Tracking, we are not provisioning.
            foreach (SyncTableToScopeMapDO mappedTable in this.SyncScope.SyncScopeTables)
            {
                SyncTableDO syncTable = SyncProviderHelperFM.GetSyncTable(this.Context.Security, mappedTable.SyncTableGuid);

                this.Configuration.SyncTables.Add(this.AddSyncTable(syncTable.TableName, syncGroup, (SyncDirection)mappedTable.SyncDirection));
            }
        }
        #endregion Abstract Method Implementations

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
                if (null != this.LocalProvider)
                {
                    ((IDisposable)this.LocalProvider).Dispose();
                    this.LocalProvider = null;
                }

                if (null != this.RemoteProvider)
                {
                    ((IDisposable)this.RemoteProvider).Dispose();
                    this.RemoteProvider = null;
                }

                this.isDisposed = true;
            }

            base.Dispose(disposing);
        }
        #endregion Disposable Interface Pattern Implementation
    }
}
