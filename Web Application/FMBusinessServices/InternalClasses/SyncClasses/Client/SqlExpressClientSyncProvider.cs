//-------------------------------------------------------------------------- 
//
//  Copyright (c) Microsoft Corporation.  All rights reserved. 
//
//  File: SqlExpressClientSynchronizationProvider.cs 
//
//  Description: Generic client synchronization provider.
//
//--------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses.Client
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlClient;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization.Formatters.Binary;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses.SyncClasses;
	using FMBusinessServices.ServiceClasses;

	using Microsoft.Synchronization.Data;
	using Microsoft.Synchronization.Data.Server;

	///<summary>
	/// A CUSTOMIZED generic client sync provider that can connect to SQL Express
	/// </summary>
	/// <remarks>    
	/// <see cref="SqlExpressClientSyncProvider"/> inherits from <see cref="ClientSyncProvider"/> and is a generic 
	/// implementation of ServerSynchronizationProvider. <see cref="SqlExpressClientSyncProvider"/> uses the mechanisms
	/// of <see cref="DbServerSyncProvider"/> to connect to the client (IE using a DBConnection)
	/// </remarks>
	public class SqlExpressClientSyncProvider : ClientSyncProvider, ISyncClientProviderFM
	{
		#region Private Fields

		/// <summary>
		/// The is disposed.
		/// </summary>
		private bool isDisposed = false;

		private DbServerSyncProvider _DbSyncProvider;
		private SyncContextFM _Context = null;
		private SyncScopeDO _SyncScope = null;
		private SyncSessionLogDO _SyncSessionLog = null;
		private SyncSessionScopeLogDO _SyncSessionScopeLog = null;
		private SyncGroupMetadata _SyncGroupMetadata = null;

		private int _RefCntSession;
		private IDbTransaction _Transaction;
		private IDbCommand _SelectClientIDCommand;
		private IDbCommand _SelectTableReceivedAnchorCommand;
		private IDbCommand _SelectTableReceivedAnchor2Command;
		private IDbCommand _SelectTableSentAnchorCommand;
		private IDbCommand _SelectTableSentAnchor2Command;
		private IDbCommand _UpdateTableSentAnchorCommand;
		private IDbCommand _UpdateTableSentAnchor2Command;
		private IDbCommand _UpdateTableReceivedAnchorCommand;
		private IDbCommand _UpdateTableReceivedAnchor2Command;
		private Dictionary<string, SyncAnchor> _ModifiedTables;
		private bool _IsSqlChangeTracking;

		#endregion Private Fields

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlExpressClientSyncProvider"/> class. 
		/// Default constructor
		/// </summary>
		public SqlExpressClientSyncProvider()
		{
			this.isDisposed = false;

			this._DbSyncProvider = new DbServerSyncProvider();
			this._DbSyncProvider.ApplyingChanges += new EventHandler<ApplyingChangesEventArgs>(this.OnApplyingChanges);
			this._DbSyncProvider.ApplyChangeFailed += new EventHandler<ApplyChangeFailedEventArgs>(this.OnApplyChangeFailed);
			this._DbSyncProvider.ChangesSelected += new EventHandler<ChangesSelectedEventArgs>(this.OnChangesSelected);
			this._DbSyncProvider.ChangesApplied += new EventHandler<ChangesAppliedEventArgs>(this.OnChangesApplied);
			this._DbSyncProvider.SyncProgress += new EventHandler<SyncProgressEventArgs>(this.OnSyncProgress);
			this._DbSyncProvider.SelectingChanges += new EventHandler<SelectingChangesEventArgs>(this.OnSelectingChanges);
			this._Context = null;
			this._RefCntSession = 0;
			this._Transaction = null;

			this.SelectClientIDCommand = this.CreateClientIDCommand();
		}

		#endregion Constructor

		#region Events
		/// <summary>
		/// The sync progress.
		/// </summary>
		public event EventHandler<SyncProgressEventArgs> SyncProgress;

		/// <summary>
		/// The apply change failed.
		/// </summary>
		public event EventHandler<ApplyChangeFailedEventArgs> ApplyChangeFailed;

		/// <summary>
		/// The applying changes.
		/// </summary>
		public event EventHandler<ApplyingChangesEventArgs> ApplyingChanges;

		/// <summary>
		/// The changes selected.
		/// </summary>
		public event EventHandler<ChangesSelectedEventArgs> ChangesSelected;

		/// <summary>
		/// The changes applied.
		/// </summary>
		public event EventHandler<ChangesAppliedEventArgs> ChangesApplied;

		#endregion Events

		#region Public Properties

		#region context property
		public SyncContextFM Context
		{
			get
			{
				return (this._Context);
			}
			set
			{
				if (value == this._Context)
				{
					return;
				}

				this._Context = value;
			}
		}
		#endregion context property

		#region SyncScope property
		public SyncScopeDO SyncScope
		{
			get { return (this._SyncScope); }
			set
			{
				if (value == this._SyncScope)
					return;

				this._SyncScope = value;
			}
		}
		#endregion SyncScope property

		#region SyncSessionLog property
		public SyncSessionLogDO SyncSessionLog
		{
			get
			{
				return (this._SyncSessionLog);
			}
			set
			{
				if (value == this._SyncSessionLog)
				{
					return;
				}

				this._SyncSessionLog = value;
			}
		}
		#endregion SyncSessionLog property

		#region SyncSessionScopeLog property
		public SyncSessionScopeLogDO SyncSessionScopeLog
		{
			get
			{
				return (this._SyncSessionScopeLog);
			}

			set
			{
				if (value == this._SyncSessionScopeLog)
				{
					return;
				}

				this._SyncSessionScopeLog = value;
			}
		}
		#endregion SyncSessionScopeLog property

		/// <summary>
		/// Sets whether this instance is using SQL Change Tracking.
		/// (if true, anchor 2 values will be maintained)
		/// </summary>
		public bool SqlChangeTracking
		{
			get
			{
				return this._IsSqlChangeTracking;
			}

			set
			{
				this._IsSqlChangeTracking = value;
			}
		}

		/// <summary>
		/// Gets or sets the command that will return the new anchor value. 
		/// </summary>
		/// <value>
		/// A <b>IDbCommand</b>-inherited object.
		/// </value>
		public IDbCommand SelectNewAnchorCommand
		{
			get
			{
				return this._DbSyncProvider.SelectNewAnchorCommand;
			}

			set
			{
				this._DbSyncProvider.SelectNewAnchorCommand = value;
			}
		}

		/// <summary>
		/// Gets or sets the server connection object.
		/// </summary>
		/// <value>
		/// A <b><see cref="SqlConnection"/></b> object.
		/// </value>
		public IDbConnection Connection
		{
			get
			{
				return this._DbSyncProvider.Connection;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				if (value.State == ConnectionState.Closed)
				{
					// giving the connection as open 
					// will prevent it from closing off the connection at end 
					// of operations
					value.Open();
					this._DbSyncProvider.Connection = value;
					value.Close();
				}
				else
				{
					this._DbSyncProvider.Connection = value;
				}
			}
		}

		/// <summary>
		/// Gets the collection of <b>SyncAdapter</b>.
		/// </summary>
		/// <value>
		/// A <b>SyncAdapterCollection</b> object.
		/// </value>
		public SyncAdapterCollection SyncAdapters
		{
			get
			{
				return this._DbSyncProvider.SyncAdapters;
			}
		}

		/// <summary>
		/// Override Sync Framework Provider's Property for ClientId
		/// </summary>
		/// <remarks>
		/// A new client is generated and saved if none is present
		/// </remarks>
		/// <value>
		/// A <b>GUID</b> object.
		/// </value>
		public override Guid ClientId
		{
			get
			{
				this.EnsureClientID();

				return this._Context.ClientID;
			}

			set
			{
				this._Context.ClientID = value;
			}
		}

		/// <summary>
		/// Gets or sets the command that will return the current client ID (GUID). 
		/// </summary>
		/// <value>
		/// An <b><see cref="IDbCommand"/></b>-inherited object.
		/// </value>
		/// <remarks>Expects a scalar value or a parameterized command using <see cref="SqlExpressClientSyncProvider"/>.PARAMETER_CLIENT_ID</remarks>
		public IDbCommand SelectClientIDCommand
		{
			get
			{
				return this._SelectClientIDCommand;
			}

			set
			{
				this._SelectClientIDCommand = value;
			}
		}

		/// <summary>
		/// Gets or sets the command that will return the current table received anchor. 
		/// </summary>
		/// <value>
		/// An <b><see cref="IDbCommand"/></b>-inherited object.
		/// </value>
		/// <remarks>Expects a scalar value or a parametrized command using <see cref="SqlExpressClientSyncProvider"/>.PARAMETER_TABLE_RECEIVED_ANCHOR</remarks>
		public IDbCommand SelectTableReceivedAnchorCommand
		{
			get
			{
				return this._SelectTableReceivedAnchorCommand;
			}

			set
			{
				this._SelectTableReceivedAnchorCommand = value;
			}
		}

		/// <summary>
		/// Gets or sets the command that will return the current table received anchor2 value. 
		/// </summary>
		/// <value>
		/// An <b><see cref="IDbCommand"/></b>-inherited object.
		/// </value>
		/// <remarks>Expects a scalar value or a parametrized command using <see cref="SqlExpressClientSyncProvider"/>.PARAMETER_TABLE_RECEIVED_ANCHOR</remarks>
		public IDbCommand SelectTableReceivedAnchor2Command
		{
			get
			{
				return this._SelectTableReceivedAnchor2Command;
			}
			set
			{
				this._SelectTableReceivedAnchor2Command = value;
			}
		}

		/// <summary>
		/// Gets or sets the command that will return the current table sent anchor. 
		/// </summary>
		/// <value>
		/// An <b><see cref="IDbCommand"/></b>-inherited object.
		/// </value>
		/// <remarks>Expects a scalar value or a parametrized command using <see cref="SqlExpressClientSyncProvider"/>.PARAMETER_TABLE_SENT_ANCHOR</remarks>
		public IDbCommand SelectTableSentAnchorCommand
		{
			get
			{
				return this._SelectTableSentAnchorCommand;
			}
			set
			{
				this._SelectTableSentAnchorCommand = value;
			}
		}

		public IDbCommand SelectTableSentAnchor2Command
		{
			get
			{
				return this._SelectTableSentAnchor2Command;
			}
			set
			{
				this._SelectTableSentAnchor2Command = value;
			}
		}

		/// <summary>
		/// Gets or sets the command that will update the current table sent anchor. 
		/// </summary>
		/// <value>
		/// An <b><see cref="IDbCommand"/></b>-inherited object.
		/// </value>
		/// <remarks>Expects a scalar value or a parametrized command using <see cref="SqlExpressClientSyncProvider"/>.PARAMETER_TABLE_SENT_ANCHOR</remarks>
		public IDbCommand UpdateTableSentAnchorCommand
		{
			get
			{
				return this._UpdateTableSentAnchorCommand;
			}
			set
			{
				this._UpdateTableSentAnchorCommand = value;
			}
		}

		public IDbCommand UpdateTableSentAnchor2Command
		{
			get
			{
				return this._UpdateTableSentAnchor2Command;
			}
			set
			{
				this._UpdateTableSentAnchor2Command = value;
			}
		}

		/// <summary>
		/// Gets or sets the command that will update the current table received anchor. 
		/// </summary>
		/// <value>
		/// An <b><see cref="IDbCommand"/></b>-inherited object.
		/// </value>
		/// <remarks>Expects a scalar value or a parametrized command using <see cref="SqlExpressClientSyncProvider"/>.PARAMETER_TABLE_RECEIVED_ANCHOR</remarks>
		public IDbCommand UpdateTableReceivedAnchorCommand
		{
			get
			{
				return this._UpdateTableReceivedAnchorCommand;
			}
			set
			{
				this._UpdateTableReceivedAnchorCommand = value;
			}
		}

		/// <summary>
		/// Gets or sets the command that will update the current table received anchor2 value. 
		/// </summary>
		/// <value>
		/// An <b><see cref="IDbCommand"/></b>-inherited object.
		/// </value>
		/// <remarks>Expects a scalar value or a parametrized command using <see cref="SqlExpressClientSyncProvider"/>.PARAMETER_TABLE_RECEIVED_ANCHOR</remarks>
		public IDbCommand UpdateTableReceivedAnchor2Command
		{
			get
			{
				return this._UpdateTableReceivedAnchor2Command;
			}

			set
			{
				this._UpdateTableReceivedAnchor2Command = value;
			}
		}

		#endregion Public Properties

		#region Static Public Methods
		/// <summary>
		/// duplicate of the function found in synchronization utility
		/// </summary>
		/// <param name="parameter">
		/// The parameter.
		/// </param>
		/// <param name="command">
		/// The command.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		public static object GetSyncObjectOutParameter(string parameter, IDbCommand command)
		{
			bool flag;
			return SqlExpressClientSyncProvider.GetSyncObjectOutParameter(parameter, command, out flag);
		}

		/// <summary>
		/// duplicate of the function found in synchronization utility
		/// </summary>
		/// <param name="parameter">
		/// The parameter.
		/// </param>
		/// <param name="command">
		/// The command.
		/// </param>
		/// <param name="found">
		/// The found.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		public static object GetSyncObjectOutParameter(string parameter, IDbCommand command, out bool found)
		{
			found = true;
			DbParameter parameter2 = SqlExpressClientSyncProvider.GetParameter(command, parameter);
			if (parameter2 != null)
			{
				return parameter2.Value;
			}
			found = false;
			return null;
		}

		/// <summary>
		/// duplicate of the function found in synchronization utility
		/// </summary>
		/// <param name="command">
		/// The command.
		/// </param>
		/// <param name="parameterName">
		/// The parameter name.
		/// </param>
		/// <returns>
		/// The <see cref="DbParameter"/>.
		/// </returns>
		public static DbParameter GetParameter(IDbCommand command, string parameterName)
		{
			if (command != null)
			{
				if (command.Parameters.Contains("@" + parameterName))
				{
					return (DbParameter)command.Parameters["@" + parameterName];
				}
				if (command.Parameters.Contains(":" + parameterName))
				{
					return (DbParameter)command.Parameters[":" + parameterName];
				}
				if (command.Parameters.Contains(parameterName))
				{
					return (DbParameter)command.Parameters[parameterName];
				}
			}
			return null;
		}
		#endregion Static Public Methods

		#region Default ClientID Command

		/// <summary>
		/// The create client id command.
		/// </summary>
		/// <returns>
		/// The <see cref="IDbCommand"/>.
		/// </returns>
		public IDbCommand CreateClientIDCommand()
		{
			SqlCommand selectClientIDCommand = new SqlCommand();
			string clientIDVariable = "@" + SyncSession.SyncClientId;
			selectClientIDCommand.CommandText =
				"SELECT " + clientIDVariable + " = SettingValue FROM [dbo].[tblConfigurationSetting] WITH (NOLOCK) WHERE SettingKey = 'InstallDetailsSynchronizationNodeGuid'";
			selectClientIDCommand.Parameters.Add(clientIDVariable, SqlDbType.UniqueIdentifier);
			selectClientIDCommand.Parameters[clientIDVariable].Direction = ParameterDirection.Output;

			return selectClientIDCommand;
		}
		#endregion Default ClientID Command

		#region Event Methods

		/// <summary>
		/// The on sync progress.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected virtual void OnSyncProgress(object sender, SyncProgressEventArgs e)
		{
			if (this.SyncProgress != null)
			{
				this.SyncProgress(sender, e);
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
		/// <remarks>
		/// **Keep in mind that this event handler is designed to handle events triggered by the custom <see cref="SqlExpressClientSyncProvider"/> type that allows client nodes 
		/// to run SQL Server Express and/or regular SQL Server installations.  To facilitate this, the SQL Server Express synchronization provider internally wraps an instance 
		/// of a <see cref="Microsoft.Synchronization.Data.Server.DbServerSyncProvider"/>.
		/// The end result is that all events will be triggered by a provider that "thinks" it is a server node.  So, when accessing the contents of the events, server and client
		/// information is inverted.  ServerChanges = ClientChanges AND ClientChanges = ServerChanges.
		/// </remarks>
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
			// Roll ApplyChanges() modifications done by inner _dbSyncProvider into one transaction with anchor changes
			if (this._Transaction != null)
			{
				e.Transaction = this._Transaction;
			}

			this.SyncSessionScopeLog.SyncSessionStateIndex = SYNCSESSIONSTATE.APPLYCHANGESTOCLIENT;
			this.UpdateSyncSessionScopeLog();

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
			this.SyncSessionScopeLog.SyncSessionStateIndex = SYNCSESSIONSTATE.GETCLIENTCHANGES;
			this.UpdateSyncSessionScopeLog();

			if (this.ChangesSelected != null)
			{
				this.ChangesSelected(sender, e);
			}
		}

		/// <summary>
		/// The on selecting changes.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The value.</param>
		protected virtual void OnSelectingChanges(object sender, SelectingChangesEventArgs e)
		{
			using (var command = new SqlCommand())
			{
				command.Connection = (SqlConnection)e.Connection;
				command.CommandType = CommandType.Text;
				command.CommandText = "CREATE TABLE #SyncTable (PK UniqueIdentifier, ChangeType CHAR(1))"
									  + " CREATE NONCLUSTERED INDEX [IX_SyncTable_ChangeType_PK] ON #SyncTable (ChangeType ASC, PK ASC)";
				command.ExecuteNonQuery();
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
				this.ChangesApplied(sender, e);
			}
		}

		#endregion Event Methods

		#region Public Overrides

		protected virtual void OnBeginningTransaction(SyncSession syncSession)
		{
			return;
		}

		protected virtual void OnTransactionBegan(SyncSession syncSession)
		{
			return;
		}

		/// <summary>
		/// Begin a transaction. This method is invoked to mark atomic operations.   
		/// </summary>
		/// <param name="syncSession">SyncSession information</param>               
		public override void BeginTransaction(SyncSession syncSession)
		{
			this._RefCntSession++;

			if (SyncTracer.IsVerboseEnabled())
			{
				SyncTracer.Info("Begin Transaction: RefCnt = {0}", this._RefCntSession);
			}


			this.OnBeginningTransaction(syncSession);

			if (this._Transaction == null)
			{
				this._Transaction = this._DbSyncProvider.Connection.BeginTransaction();
			}

			this.OnTransactionBegan(syncSession);
		}


		/// <summary>
		/// End transaction. This method is called by the agent at to conclude an atomic operation.
		/// </summary>
		/// <param name="commit">Commit/Abort SyncSession</param>               
		/// <param name="syncSession">SyncSession information</param>               
		public override void EndTransaction(bool commit, SyncSession syncSession)
		{
			this._RefCntSession--;

			if (SyncTracer.IsVerboseEnabled())
			{
				SyncTracer.Info("End Transaction: RefCnt = {0}", this._RefCntSession);
			}

			if (this._RefCntSession == 0)
			{
				try
				{
					if (commit)
					{
						this._Transaction.Commit();
					}
					else
					{
						this._Transaction.Rollback();
					}
				}
				finally
				{
					this._Transaction.Dispose();
					this._Transaction = null;
				}
			}

			if (this._RefCntSession < 0)
			{
				this._RefCntSession = 0;
			}

			// Added to get sent anchor 2 values.. Outside of the sync transaction, 
			// but it's a small window that only applies to SQL Server Change Tracking.  
			// Right now, this is the only place where we can intercept this and get back the correct anchors.
			if (this.SqlChangeTracking && this._RefCntSession == 0 && commit)
			{
				// If last transaction
				// If we have tables to clean up
				// if ((_ModifiedTables != null))
				// {
				//    //Dictionary<string, SyncAnchor> tablesUpdated = _ModifiedTables;
				//    _ModifiedTables = null;
				//    //UpdateTableSentAnchorFixForSQLChangeTrackingValues(tablesUpdated, GetServerID(syncSession));
				// }
			}
		}

		/// <summary>
		/// Creates the database schema on client database -- NOT IMPLEMENTED
		/// </summary>
		/// <param name="syncTable">
		/// The sync Table.
		/// </param>
		/// <param name="syncSchema">
		/// The sync Schema.
		/// </param>
		/// <remarks>
		/// In the current implementation of this class, we assume that the 
		/// client already has the same schema as the server (run the demo scripts).
		/// </remarks>
		public override void CreateSchema(SyncTable syncTable, SyncSchema syncSchema)
		{
			throw new NotSupportedException("Create Schema is not supported in this version."
					+ "Please make sure client and server have same schema!");
		}

		/// <summary>
		/// Gets the changes made on the client since last sync.
		/// </summary>
		/// <param name="groupMetadata"> Contains table metadata </param>
		/// <param name="syncSession"> The current sync session </param>
		/// <returns> SyncContext populated with the incremental changes </returns>
		public override SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession)
		{
			this._SyncGroupMetadata = groupMetadata;

			SyncTracer.Info("BEGIN (Client) GetChanges()");

			// need to set the LastReceivedAnchor as the LastSentAnchor since 
			// DbServerSyncProvider operates from the server's perspective, so
			// we swap the two fields temporarily. 
			foreach (SyncTableMetadata metaTable in groupMetadata.TablesMetadata)
			{
				SyncAnchor temp = metaTable.LastReceivedAnchor;
				metaTable.LastReceivedAnchor = metaTable.LastSentAnchor;
				metaTable.LastSentAnchor = temp;
			}

			if (SyncTracer.IsInfoEnabled())
			{
				SyncTracer.Verbose("Calling (Client) Internal DbSyncProvider.GetChanges()");
				SyncTracer.Verbose(1, "** GETTING CLIENT CHANGES FOR SERVER **");
				SyncTracer.Verbose(1, "** (Invert direction on underlying DbSyncProvider messages) **");
			}

			SyncContext syncContext = this._DbSyncProvider.GetChanges(groupMetadata, syncSession);

			SyncProviderHelperFM.GetMaxRowVersions(groupMetadata, this.Context, this.SyncScope, syncContext, false);

			SyncTracer.Info(1, "** FINISHED GETTING CLIENT CHANGES FOR SERVER **");
			SyncTracer.Info("Called Internal DbSyncProvider.GetChanges()");

			// swap them back for consistency
			foreach (SyncTableMetadata metaTable in groupMetadata.TablesMetadata)
			{
				SyncAnchor temp = metaTable.LastReceivedAnchor;
				metaTable.LastReceivedAnchor = metaTable.LastSentAnchor;
				metaTable.LastSentAnchor = temp;
			}

			SyncTracer.Info("END (Client) GetChanges()");

			return syncContext;
		}

		private static DataTable FindTable(SyncContext context, string tableName)
		{
			foreach (DataTable table in context.DataSet.Tables)
			{
				if (table.TableName == tableName)
				{
					return table;
				}
			}

			return null;
		}

		/// <summary>
		/// Apply changes downloaded from the server. 
		/// </summary>
		/// <remarks>
		/// Inner SyncProvider will take care of applying changes to actual 
		/// data, but we need to take care of updating anchor meta data. 
		/// </remarks>
		/// <param name="groupMetadata"> Contains table meta data info </param>
		/// <param name="dataSet"> Contains changes to be applied </param>
		/// <param name="syncSession"> Current sync session </param>
		/// <returns> SyncContext object to Sync Agent </returns>
		public override SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, SyncSession syncSession)
		{
			SyncTracer.Info("BEGIN (Client) ApplyChanges()");

			this.EnsureServerIdParameter(syncSession);

			// Map SyncDirection from client POV to our internal server POV

			foreach (SyncTableMetadata tableMetadata in groupMetadata.TablesMetadata)
			{
				if (tableMetadata.SyncDirection == SyncDirection.DownloadOnly || tableMetadata.SyncDirection == SyncDirection.Snapshot)
				{
					// This SyncDirection DownloadOnly/Snapshot is from a Client point of view. But our client is inturn a Server provider.   Hence switch this to UploadOnly
					tableMetadata.SyncDirection = SyncDirection.UploadOnly;
				}
				else if (tableMetadata.SyncDirection == SyncDirection.UploadOnly)
				{
					// This SyncDirection UploadOnly is from Client POV. But our client is inturn a Server provider. Hence switch this to DownloadOnly
					tableMetadata.SyncDirection = SyncDirection.DownloadOnly;
				}
			}

			// Need to set the LastReceivedAnchor as the LastSentAnchor since 
			// DbServerSyncProvider operates from the server's perspective, so
			// we swap the two fields temporarily. 
			// Note that even if we do this, the NewAnchor value will be the one
			// from the server, not local which is invalid since the client and server
			// clocks are always at least the tiniest bit misaligned
			foreach (SyncTableMetadata metaTable in groupMetadata.TablesMetadata)
			{
				SyncAnchor temp = metaTable.LastReceivedAnchor;
				metaTable.LastReceivedAnchor = metaTable.LastSentAnchor;
				metaTable.LastSentAnchor = temp;
			}

			SyncTracer.Info("Calling (Client) Internal DbSyncProvider.ApplyChanges()");
			SyncTracer.Info(1, "** APPLYING SERVER CHANGES TO CLIENT **");
			SyncTracer.Info(1, "** (Invert direction on underlying DbSyncProvider messages) **");

			// Swap ClientID with ServerID Value
			Guid actualClientId = syncSession.ClientId;
			Guid actualServerId = this.GetServerId(syncSession);

			syncSession.ClientId = actualServerId;
			SyncContext syncContext = this._DbSyncProvider.ApplyChanges(groupMetadata, dataSet, syncSession);

			// Swap ServerID with ClientID
			syncSession.ClientId = actualClientId;

			SyncTracer.Info(1, "** FINISHED APPLYING SERVER CHANGES TO CLIENT **");
			SyncTracer.Info("Called Internal DbSyncProvider.ApplyChanges()");

			// swap them back for consistency
			foreach (SyncTableMetadata metaTable in groupMetadata.TablesMetadata)
			{
				SyncAnchor temp = metaTable.LastReceivedAnchor;
				metaTable.LastReceivedAnchor = metaTable.LastSentAnchor;
				metaTable.LastSentAnchor = temp;
			}

			if (this.SqlChangeTracking)
			{
				this._ModifiedTables = new Dictionary<string, SyncAnchor>(StringComparer.InvariantCultureIgnoreCase);
			}

			foreach (SyncTableMetadata table in groupMetadata.TablesMetadata)
			{
				this.SetTableReceivedAnchor(table.TableName, table.LastReceivedAnchor);

				if (this.SqlChangeTracking)
				{
					// Remember which tables the server has updated, we need to update their sent anchor 2 values.
					this._ModifiedTables[table.TableName] = table.LastSentAnchor;
				}
			}

			SyncTracer.Info("END (Client) ApplyChanges()");

			return syncContext;
		}

		#endregion Public Overrides

		#region Public Anchor Related Overrides

		/// <summary>
		/// Retrieves the last received anchor from the 'anchor' meta table.
		/// </summary>
		/// <param name="tableName"> The name of the table which we want the anchor for. </param>
		/// <returns> A sync anchor object containing the last received anchor. </returns>
		/// <remarks>
		/// Under typical scenarios, the LastReceivedAnchor is returned.  However; if Inserts/Updates are processed 
		/// separately from Deletes, then the LastReceivedAnchor2 is returned for the Delete anchors.
		/// </remarks>
		public override SyncAnchor GetTableReceivedAnchor(string tableName)
		{
			if (this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT)
			{
				if (this.SelectTableReceivedAnchorCommand == null)
				{
					throw new NotImplementedException("You must provide a SelectTableReceivedAnchorCommand");
				}

				return this.ExecuteAnchorSelectCommand(this.SelectTableReceivedAnchorCommand, SyncParamsFM.SyncTableReceivedAnchorName, tableName);
			}
			else
			{
				if (this.SelectTableReceivedAnchor2Command == null)
				{
					throw new NotImplementedException("You must provide a SelectTableReceivedAnchor2Command");
				}

				return this.ExecuteAnchorSelectCommand(this.SelectTableReceivedAnchor2Command, SyncParamsFM.SyncTableReceivedAnchorName, tableName);
			}
		}

		/// <summary>
		/// Retrieves the last sent anchor from the 'anchor' meta table.
		/// </summary>
		/// <param name="tableName"> The name of the table for which we want the anchor. </param>
		/// <returns> A sync anchor object containing the last sent anchor. </returns>
		/// <remarks>
		/// Under typical scenarios, the LastSentAnchor1 value is returned.  However; if Inserts/Updates are processed 
		/// separately from Deletes, then the LastSentAnchor2 will be returned for the Delete anchors.
		/// </remarks>
		public override SyncAnchor GetTableSentAnchor(string tableName)
		{
			if (this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT)
			{
				if (this.SelectTableSentAnchorCommand == null)
				{
					throw new NotImplementedException("You must provide a SelectTableSentAnchorCommand");
				}

				return this.ExecuteAnchorSelectCommand(this.SelectTableSentAnchorCommand, SyncParamsFM.SyncTableSentAnchorName, tableName);
			}
			else
			{
				if (this.SelectTableSentAnchor2Command == null)
				{
					throw new NotImplementedException("You must provide a SelectTableSentAnchor2Command");
				}

				return this.ExecuteAnchorSelectCommand(this.SelectTableSentAnchor2Command, SyncParamsFM.SyncTableSentAnchorName, tableName);
			}
		}

		//public SyncAnchor GetTableSentAnchorFixForSQLChangeTracking(string tableName)
		//{
		//    return (null);
		//}

		/// <summary>
		/// Sets the last received anchor in the 'anchor' meta table.
		/// </summary>
		/// <param name="tableName"> The name of the table for which we want to set the anchor </param>
		/// <param name="anchor"> SyncAnchor object containing the anchor. </param>
		/// <remarks>
		/// Under typical scenarios, the LastReceivedAnchor contains this value.  However; if Inserts/Updates are processed 
		/// separately from Deletes, then the LastReceivedAnchor2 will be used to track the Delete anchors.
		/// </remarks>
		public override void SetTableReceivedAnchor(string tableName, SyncAnchor anchor)
		{
			if (this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT)
			{
				if (this.UpdateTableReceivedAnchorCommand == null)
				{
					throw new NotImplementedException("You must provide a UpdateTableReceivedAnchorCommand");
				}

				this.ExecuteAnchorUpdateCommand(this.UpdateTableReceivedAnchorCommand, tableName, anchor);
			}
			else
			{
				if (this.UpdateTableReceivedAnchor2Command == null)
				{
					throw new NotImplementedException("You must provide a UpdateTableReceivedAnchor2Command");
				}

				this.ExecuteAnchorUpdateCommand(this.UpdateTableReceivedAnchor2Command, tableName, anchor);
			}
		}

		/// <summary>
		/// Sets the last sent anchor in the 'anchor' meta table
		/// </summary>
		/// <param name="tableName"> The name of the table for which we want to set the anchor </param>
		/// <param name="anchor"> SyncAnchor object containing the anchor. </param>
		/// <remarks>
		/// Under typical scenarios, the LastSentAnchor1 contains this value.  However; if Inserts/Updates are processed 
		/// separately from Deletes, then the LastSentAnchor2 will be used to track the Delete anchors.
		/// </remarks>
		public override void SetTableSentAnchor(string tableName, SyncAnchor anchor)
		{
			if (this._SyncGroupMetadata == null)
			{
				throw new Exception("SetTableSentAnchor : No GroupMetadata");
			}

			foreach (var tableMetaData in this._SyncGroupMetadata.TablesMetadata)
			{
				if (tableMetaData.TableName != tableName)
				{
					continue;
				}

				if (this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL
					|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE
					|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT)
				{
					if (this.UpdateTableSentAnchorCommand == null)
					{
						throw new NotImplementedException("You must provide a UpdateTableSentAnchorCommand");
					}

					this.ExecuteAnchorUpdateCommand(this.UpdateTableSentAnchorCommand, tableName, tableMetaData.LastSentAnchor);
				}
				else
				{
					if (this.UpdateTableSentAnchor2Command == null)
					{
						throw new NotImplementedException("You must provide a UpdateTableSentAnchor2Command");
					}

					this.ExecuteAnchorUpdateCommand(this.UpdateTableSentAnchor2Command, tableName, tableMetaData.LastSentAnchor);
				}

				break;
			}
		}

		// public void SetTableSentAnchorFixForSQLChangeTracking(string tableName, SyncAnchor anchor)
		// {
		// }

		#endregion Public Anchor Related Overrides

		#region Virtual Methods

		/// <summary>
		/// Ensures any anchor commands that use ServerID are updated
		/// </summary>
		/// <param name="syncSession">
		/// The sync Session.
		/// </param>
		protected virtual void EnsureServerIdParameter(SyncSession syncSession)
		{
			byte[] serverId = this.SerializeAnchorValue(this.GetServerId(syncSession)); // this may be used by masked _dbSyncProvider

			DbParameter serverIdParam = null;

			// Anchor Commands
			if (this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT)
			{
				serverIdParam = SqlExpressClientSyncProvider.GetParameter(this.UpdateTableReceivedAnchorCommand, SyncParamsFM.SyncServerIDName);
			}
			else
			{
				serverIdParam = SqlExpressClientSyncProvider.GetParameter(this.UpdateTableReceivedAnchor2Command, SyncParamsFM.SyncServerIDName);
			}

			if (serverIdParam != null)
			{
				serverIdParam.Value = serverId;
			}

			if (this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_ALL
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE
				|| this._Context.CurrentControllerStep == SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT)
			{
				serverIdParam = SqlExpressClientSyncProvider.GetParameter(this.UpdateTableSentAnchorCommand, SyncParamsFM.SyncServerIDName);
			}
			else
			{
				serverIdParam = SqlExpressClientSyncProvider.GetParameter(this.UpdateTableSentAnchor2Command, SyncParamsFM.SyncServerIDName);
			}

			if (serverIdParam != null)
			{
				serverIdParam.Value = serverId;
			}
		}

		/// <summary>
		/// Gets the current server id or generates one if not given.
		/// </summary>
		/// <param name="syncSession">
		/// The sync Session.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		/// <remarks>
		/// The Varec implementation of the synchronization framework introduced a new <see cref="SyncContextFM"/> object that allows the calling 
		/// application to define and control the synchronization process.  Rather than having the sync client provider identify the 
		/// remote sync Server, we now just extract it from the <see cref="SyncContextFM"/> instance.
		/// </remarks>
		protected virtual Guid GetServerId(SyncSession syncSession)
		{
			return this.Context.ServerID;
		}

		/// <summary>
		/// The get site id.
		/// </summary>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		/// <remarks>
		/// The Varec implementation of the synchronization framework introduced a new <see cref="SyncContextFM"/> object that allows the calling 
		/// application to define and control the synchronization process.  Rather than having the sync client provider identify the 
		/// remote current SiteId, we now just extract it from the <see cref="SyncContextFM"/> instance.
		/// </remarks>
		protected string GetSiteId(SyncSession syncSession)
		{
			return this.Context.CurrentSiteID;
		}

		////protected virtual SyncAnchor GetTableSentAnchorFixForSQLChangeTrackingValue(string tableName, Guid serverID, SyncAnchor syncAnchor)
		////{
		////    IDbCommand command = GenerateTableSentAnchorFixForSQLChangeTrackingValueCommand(tableName);
		////
		////    object anchorVal = null;
		////    bool commandPassed = false;
		////
		////    try
		////    {
		////        BeginTransaction(null);
		////        command.connection = _DbSyncProvider.connection;
		////        command.Transaction = _Transaction;
		////
		////        DbParameter serverIDParam = GetParameter(command, SyncParamsFM.SyncServerIDBinaryName);
		////        if (serverIDParam != null)
		////        {
		////            byte[] serverIDValue = serverID.ToByteArray();
		////            serverIDParam.Value = serverIDValue;
		////        }
		////        DbParameter tableParam = GetParameter(command, SyncParamsFM.SyncTableName);
		////        if (tableParam != null)
		////        {
		////            tableParam.Value = tableName;
		////        }
		////        DbParameter anchorParam = GetParameter(command, SyncParamsFM.SyncTableSentAnchorName);
		////        if (anchorParam != null)
		////        {
		////            anchorParam.Value = DeserializeAnchorValue(syncAnchor.Anchor);
		////        }
		////        if (command.connection.State == ConnectionState.Closed)
		////        {
		////            command.connection.Open();
		////        }
		////        anchorVal = command.ExecuteScalar();
		////        commandPassed = true;
		////    }
		////    finally
		////    {
		////        EndTransaction(commandPassed, null);
		////    }

		////    if ((anchorVal == null) || (anchorVal == System.DBNull.Value))
		////    {
		////        return syncAnchor; // inputted value
		////    }
		////    else
		////    {
		////        return new SyncAnchor(SerializeAnchorValue(anchorVal));
		////    }
		////}

		///// <summary>
		///// Generates the command needed to retrieve a secondary set of sent anchor values that are
		///// required when using SQL Server Change Tracking.  There's a bug where the local change context is 
		///// cleared causing the record to be picked up as an insert back to the server and generating a 
		///// PK violation.
		///// </summary>
		////protected virtual IDbCommand GenerateTableSentAnchorFixForSQLChangeTrackingValueCommand(string tableName)
		////{
		////    var cmd = new SqlCommand();
		////    cmd.CommandType = CommandType.StoredProcedure;
		////    cmd.CommandText = "sync.usp_SyncAnchorGenerateSentAnchor2ByTableName";

		////    cmd.Parameters.Add(SyncParamsFM.SYNC_SERVER_ID_BINARY_PARAMETER, SqlDbType.Binary);
		////    cmd.Parameters.Add(SyncParamsFM.SYNC_CURRENT_TABLE_PARAMETER, SqlDbType.NVarChar, 1024); // it can't auto-add it (because it operates at abstract level)
		////    cmd.Parameters.Add(SyncParamsFM.SYNC_ANCHOR_VALUE_PARAMETER, SqlDbType.BigInt).Direction = ParameterDirection.Output;

		////    return (cmd);
		////}

		///// <summary>
		///// Processes all downloaded tables sent parameters
		///// </summary>
		////        protected virtual void UpdateTableSentAnchorFixForSQLChangeTrackingValues(Dictionary<string, SyncAnchor> tablesToClean, Guid serverID)
		////        {
		////            foreach (string tableName in tablesToClean.Keys)
		////            {
		////                SyncAnchor anchor2 = GetTableSentAnchorFixForSQLChangeTrackingValue(tableName, serverID, tablesToClean[tableName]);
		////#if DEBUG
		////                string fixedAnchorVal = AnchorAsString(anchor2);
		////#endif
		////                SetTableSentAnchorFixForSQLChangeTracking(tableName, anchor2);
		////            }
		////        }

		/// <summary>
		/// If no ClientID is provided, executes SelectClientIDCommand
		/// </summary>
		protected virtual void EnsureClientID()
		{
			if (this._Context.ClientID == Guid.Empty)
			{
				if (this.SelectClientIDCommand == null)
				{
					throw new NotImplementedException("Unable to acquire ClientID. You must either provide it via the ClientID property or by providing a SelectClientIDCommand");
				}

				bool commandPassed = false;

				IDataReader reader = null;
				try
				{
					this.BeginTransaction(null);
					this.SelectClientIDCommand.Connection = this._DbSyncProvider.Connection;
					this.SelectClientIDCommand.Transaction = this._Transaction;

					if (this.SelectClientIDCommand.Connection.State == ConnectionState.Closed)
					{
						this.SelectClientIDCommand.Connection.Open();
					}
					if (GetParameter(this.SelectClientIDCommand, SyncSession.SyncClientId) != null)
					{
						// parameter mode
						this.SelectClientIDCommand.ExecuteNonQuery();
						object result = SqlExpressClientSyncProvider.GetSyncObjectOutParameter(SyncSession.SyncClientId, this.SelectClientIDCommand);

						if ((result != null) && (result != DBNull.Value))
						{
							this._Context.ClientID = new Guid(result.ToString());
						}
					}
					else
					{
						// assume scalar mode
						reader = this.SelectClientIDCommand.ExecuteReader();

						if (null != reader)
						{
							if (reader.Read())
							{
								this._Context.ClientID = reader.GetGuid(0);
								reader.Close();
							}
						}
					}

					commandPassed = true;
				}
				finally
				{
					try
					{
						if (reader != null)
						{
							if (!reader.IsClosed)
							{
								reader.Close();
							}
							reader.Dispose();
						}
					}
					finally
					{
						this.EndTransaction(commandPassed, null);
					}
				}
			}
		}

		/// <summary>
		/// The serialize anchor value.
		/// </summary>
		/// <param name="anchorVal">
		/// The anchor val.
		/// </param>
		/// <returns>
		/// The <see>
		///         <cref>byte[]</cref>
		///     </see>
		///     .
		/// </returns>
		protected virtual byte[] SerializeAnchorValue(object anchorVal)
		{
			using (MemoryStream serializationStream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(serializationStream, anchorVal);
				return serializationStream.ToArray();
			}
		}

		/// <summary>
		/// The execute anchor select command.
		/// </summary>
		/// <param name="command">
		/// The command.
		/// </param>
		/// <param name="outParameterName">
		/// The out parameter name.
		/// </param>
		/// <param name="tableName">
		/// The table name.
		/// </param>
		/// <returns>
		/// The <see cref="SyncAnchor"/>.
		/// </returns>
		protected virtual SyncAnchor ExecuteAnchorSelectCommand(IDbCommand command, string outParameterName, string tableName)
		{
			object anchorVal = null;
			bool commandPassed = false;

			try
			{
				this.BeginTransaction(null);
				command.Connection = this._DbSyncProvider.Connection;
				command.Transaction = this._Transaction;

				DbParameter siteIdParam = SqlExpressClientSyncProvider.GetParameter(command, SyncParamsFM.SyncContextSiteIDName);

				if (siteIdParam != null && siteIdParam.Value != DBNull.Value)
				{
					siteIdParam.Value = this._Context.CurrentSiteID;
				}

				DbParameter tableParam = SqlExpressClientSyncProvider.GetParameter(command, SyncParamsFM.SyncCurrentTableName);
				if (tableParam != null && tableParam.Value != DBNull.Value)
				{
					tableParam.Value = tableName;
				}

				SyncTracer.Info(2, "Selecting Anchors for Site: {0}, TableName: {1} **", (null != siteIdParam) ? siteIdParam.Value : "Invalid", (null != tableParam) ? tableParam.Value : "Invalid");

				if (command.Connection.State == ConnectionState.Closed)
				{
					command.Connection.Open();
				}

				if (GetParameter(command, outParameterName) != null)
				{
					// parameter mode
					command.ExecuteNonQuery();
					anchorVal = SqlExpressClientSyncProvider.GetSyncObjectOutParameter(outParameterName, command);
				}
				else
				{
					// assume scalar mode
					anchorVal = command.ExecuteScalar();
				}

				commandPassed = true;
			}
			finally
			{
				this.EndTransaction(commandPassed, null);
			}

			if ((anchorVal == null) || (anchorVal == System.DBNull.Value))
			{
				return new SyncAnchor();
			}
			else
			{
				return new SyncAnchor(this.SerializeAnchorValue(anchorVal));
			}
		}

		/// <summary>
		/// The execute anchor update command.
		/// </summary>
		/// <param name="command">
		/// The command.
		/// </param>
		/// <param name="tableName">
		/// The table name.
		/// </param>
		/// <param name="anchor">
		/// The anchor.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// An exception will be thrown if we attempt to update the synchronization anchors and nothing gets updated.  This means that we're not able to track the synchronization anchors for the specified table.
		/// </exception>
		protected virtual void ExecuteAnchorUpdateCommand(IDbCommand command, string tableName, SyncAnchor anchor)
		{
			bool commandPassed = false;
			try
			{
				this.BeginTransaction(null);
				command.Connection = this._DbSyncProvider.Connection;
				command.Transaction = this._Transaction;

				DbParameter siteIdParam = GetParameter(command, SyncParamsFM.SyncContextSiteIDName);
				if (siteIdParam != null && siteIdParam.Value != DBNull.Value)
				{
					siteIdParam.Value = this._Context.CurrentSiteID;
				}

				DbParameter tableParam = SqlExpressClientSyncProvider.GetParameter(command, SyncParamsFM.SyncCurrentTableName);
				if (tableParam != null && tableParam.Value != DBNull.Value)
				{
					tableParam.Value = tableName;
				}

				DbParameter anchorParam = SqlExpressClientSyncProvider.GetParameter(command, SyncParamsFM.SyncAnchorValueName);
				if (anchorParam != null && anchorParam.Value != DBNull.Value)
				{
					anchorParam.Value = SyncDBI.DeserializeAnchorValue(anchor.Anchor);
				}

				SyncTracer.Info(2, "Updating Anchors for Site: {0}, TableName: {1}, Anchor Value: {2} **", (null != siteIdParam) ? siteIdParam.Value : "Invalid", (null != tableParam) ? tableParam.Value : "Invalid", (null != anchorParam) ? anchorParam.Value : "Invalid");

				if (command.Connection.State == ConnectionState.Closed)
				{
					command.Connection.Open();
				}

				if (command.ExecuteNonQuery() == 0)
				{
					throw new InvalidOperationException("Unable to complete sync. Anchor update had no effect.");
				}

				commandPassed = true;
			}
			finally
			{
				this.EndTransaction(commandPassed, null);
			}
		}
		#endregion Virtual Methods

		#region Helpers
		/// <summary>
		/// REALLY DON'T WANT TO PUT THIS HERE BUT DOING IT FOR NOW.
		/// THIS SHOULD BE HANDLED BY THE EVENT HANDLERS THAT REGISTER TO THE EVENTS RAISED BY THIS CLASS
		/// THIS LAYER SHOULDN'T BE TIED TO A SPECIFIC LOGGING MECHANISM.
		/// Updates the current synchronization session scope log record updated statistics
		/// </summary>
		private void UpdateSyncSessionScopeLog()
		{
			ISyncSessionScopeLogs sessionLogDetails = new SyncSessionScopeLogs();
			sessionLogDetails.Modify(this.Context.Security, this.SyncSessionScopeLog);
		}

		#endregion Helpers

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
				this._DbSyncProvider.Dispose();

			}

			this.isDisposed = true;
		}

		/// <summary>
		/// Disposes this Client Sync Provider instance 
		/// </summary>
		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Interface Implementation
	}
}
