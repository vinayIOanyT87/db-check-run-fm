// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerSyncProviderFM.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The server sync provider fm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses.Server
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.ServiceClasses;

    using Microsoft.Synchronization.Data;
    using Microsoft.Synchronization.Data.Server;

    /// <summary>
    /// Implementation of the server side synchronization provider that maps / un-maps the synchronization SQL connection with
    /// the FuelsManager session.  This class is responsible for identifying the tables in the synchronization context
    /// and allocating a <see cref="SyncAdapter"/> for each one.
    /// </summary>
    public partial class ServerSyncProviderFM : SqlServerSyncProvider
    {
        #region Attributes
        /// <summary>
        /// The is disposed.
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// The session to SQL index.
        /// </summary>
        private long? sessionToSqlIndex = null;

        #endregion Attributes

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSyncProviderFM"/> class.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncScope">
        /// The sync scope.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="syncSessionDo">
        /// The sync Session Do.
        /// </param>
        public ServerSyncProviderFM(SecurityClass security, SyncScopeDO syncScope, SyncContextFM context, SyncSessionLogDO syncSessionDo)
            : base()
        {
            this.isDisposed = false;
            this.sessionToSqlIndex = null;
            this.CurrentSyncSession = syncSessionDo;
            this.CurrentSyncSessionDetail = null;

            this.InitializeSynchronization(security, syncScope, context);
			((SqlConnection)this.Connection).StateChange += this.ConnectionStateChangeHandler;
        }
        #endregion Constructors/Destructors

        #region Properties

        #region CurrentSyncSession property

        /// <summary>
        /// Gets or sets the current sync session.
        /// </summary>
        protected SyncSessionLogDO CurrentSyncSession { get; set; }

        #endregion CurrentSyncSession property

        #region CurrentSyncSessionDetail property

        /// <summary>
        /// Gets or sets the current sync session detail.
        /// </summary>
        protected SyncSessionScopeLogDO CurrentSyncSessionDetail { get; set; }

        #endregion CurrentSyncSessionDetail property

        #endregion Properties

        #region Synchronization Event Handlers
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
            // Now call the main handler with the current Security context.
            SyncServerProviderHelperFM.ApplyChangeFailed(this.Context, this.CurrentSyncSessionDetail, sender, e);
        }

		public void ChangesSelectedHandler(object sender, ChangesSelectedEventArgs e)
		{

		}

		public void SelectingChangesHandler(object sender, SelectingChangesEventArgs e)
		{

		}


        #endregion Synchronization Event Handlers

        #region Private Synchronization Methods

        /// <summary>
        /// The on initialize synchronization.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncScope">
        /// The sync scope.
        /// </param>
        protected override void OnInitializeSynchronization(SecurityClass security, SyncScopeDO syncScope)
        {
            if (null != this.CurrentSyncSession)
            {
                this.CurrentSyncSessionDetail = this.GetCurrentSyncSessionLogDetail();
            }

			this.MapSqlConnection();

            // Create a command to retrieve a new anchor value from
            // the server. In this case, we use a time stamp value
            // that is retrieved and stored in the client database.
            // During each synchronization, the new anchor value and
            // the last anchor value from the previous synchronization
            // are used: the set of changes between these upper and
            // lower bounds is synchronized.
            this.SelectNewAnchorCommand = SyncProviderHelperFM.CreateAnchorCommand((this.BatchSize > 0) ? true : false, SyncProviderHelperFM.ServerNodeType);
            this.SelectNewAnchorCommand.Connection = this.Connection;

            // For each Table that's mapped to this provider, create and register SyncAdapters.
            foreach (SyncTableToScopeMapDO mappedTable in syncScope.SyncScopeTables)
            {
                this.SyncAdapters.Add(SyncServerProviderHelperFM.GetSyncAdapter(security, mappedTable, this.Context, (SqlConnection)this.Connection));
            }

            // Hook Events
            this.ApplyChangeFailed += new EventHandler<ApplyChangeFailedEventArgs>(this.ApplyChangeFailedHandler);
			this.ChangesSelected += new EventHandler<ChangesSelectedEventArgs>(this.ChangesSelectedHandler);
			this.SelectingChanges += new EventHandler<SelectingChangesEventArgs>(this.SelectingChangesHandler);
		}

		/// <summary>
        /// The synchronize synchronization session with database session.
        /// </summary>
        /// <returns>
        /// The <see cref="SyncSessionScopeLogDO"/>.
        /// </returns>
        private SyncSessionScopeLogDO GetCurrentSyncSessionLogDetail()
        {
            if (SyncTracer.IsVerboseEnabled())
            {
                SyncTracer.Info(2, "** GetCurrentSyncSessionLogDetail.  Server Session Token: {0} **", this.Context.ServerSecurity.Token);
            }

            SyncSessionScopeLogDO sessionDetailDo = null;

            // Make sure we have a synchronization session reference.
            if (null != this.CurrentSyncSession && this.Context.CurrentSiteGuid.HasValue)
            {
                var syncSessionDetails = new SyncSessionScopeLogs();
                sessionDetailDo = syncSessionDetails.GetByCompositeKey(this.Context.ServerSecurity, this.CurrentSyncSession.IdentityGuid, this.Context.CurrentSiteGuid.Value, this.Context.CurrentSyncScopeID);

                if (null == sessionDetailDo)
                {
                    sessionDetailDo = new SyncSessionScopeLogDO();
                    sessionDetailDo.IdentityGuid = Guid.NewGuid();
                    sessionDetailDo.SyncSessionLogGuid = this.CurrentSyncSession.IdentityGuid;
                    sessionDetailDo.SiteGuid = this.Context.CurrentSiteGuid.Value;
                    sessionDetailDo.SiteID = this.Context.CurrentSiteID;
					sessionDetailDo.SiteTypeIndex = this.Context.SiteType;
					sessionDetailDo.SyncScopeID = this.Context.CurrentSyncScopeID;
                    sessionDetailDo.SyncSessionStatusIndex = SYNCSESSIONSTATUS.NEW;
                    sessionDetailDo.SyncSessionStateIndex = (this.Context.CurrentControllerStep
                                                                == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE)
                                                                ? SYNCSESSIONSTATE.PROCESSINSUPD
                                                                : SYNCSESSIONSTATE.PROCESSDEL;
                    sessionDetailDo.StartDate = DateTimeOffset.Now;

                    using (var dbi = new SyncSessionScopeLogDBI(this.Context.ServerSecurity.UserID))
                    {
                        dbi.Save(this.Context.ServerSecurity, sessionDetailDo);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid synchronization tracking session.");
            }

            return sessionDetailDo;
        }

        #endregion Private Synchronization Methods

        #region Dispose
        /// <summary>
        /// Disposes this Server Sync Provider instance 
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
					this.UnMapSqlConnection();

					SqlConnection conn = this.Connection as SqlConnection;
					if (conn != null)
					{
						conn.StateChange -= this.ConnectionStateChangeHandler;
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
		/// Check state transition for a close. If so, then immediately unmap the SQL connection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ConnectionStateChangeHandler(object sender, StateChangeEventArgs e)
		{
		}

		/// <summary>
		/// The synchronize FuelsManager synchronization session with database connection/session.
		/// </summary>
		private void MapSqlConnection()
		{
			this.sessionToSqlIndex = ConsolidatedDAClass.MapSqlConnectionToSession(
				this.Context.ServerSecurity,
				(SqlConnection)this.Connection);

			if (this.sessionToSqlIndex.HasValue && SyncTracer.IsVerboseEnabled())
			{
				SyncTracer.Info(
					2,
					"** UpdateSessionContext Connection State: {0}, Server Session Token: {1} **",
					this.Connection.State.ToString(),
					this.Context.ServerSecurity.Token);
			}
		}

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

        #endregion Dispose
    }
}
