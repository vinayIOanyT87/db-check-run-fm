// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterpriseSynchronization.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the EnterpriseSynchronization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;
	using System.ServiceModel;
	using System.Text;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
	using FMBusinessServices.InternalClasses.SyncClasses;
	using FMBusinessServices.InternalClasses.SyncClasses.Server;
	using FMBusinessServices.InternalInterfaces;
	using FMBusinessServices.ServiceClasses;
	using Microsoft.Synchronization.Data;

	/// <summary>
	/// The enterprise synchronization.
	/// </summary>
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class EnterpriseSynchronization : IEnterpriseSynchronization
	{
		#region Attributes
		/// <summary>
		/// The event log.
		/// </summary>
		/// 
		private static FMEventLog eventLog = new FMEventLog();

		/// <summary>
		/// The last server sync profile id.
		/// </summary>
		private string _LastServerSyncProfileID = string.Empty;

		/// <summary>
		/// The last server sync scope id.
		/// </summary>
		private string _LastServerSyncScopeID = string.Empty;

		const int MaxConflictsPerCall = 1000;
		/// <summary>
		/// An instance of the most recent server synchronization provider.
		/// </summary>
		private ISyncServerProviderFM _ServerSyncProvider = null;

		private static readonly IPointTagArchiveDatabase PointTagArchiveDatabase = new PointTagArchiveDatabase();

		private static readonly IAandEArchiveDatabase AlarmAndEventArchiveDataBase = new AandEArchiveDatabase();

		#endregion Attributes

		#region Constructor / Initialization

		#endregion Constructor

		#region Static Private Helper Methods

		/// <summary>
		/// The get server synchronization readiness.
		/// </summary>
		/// <param name="serverSyncConfig">
		/// The server sync config.
		/// </param>
		/// <returns>
		/// The <see cref="SYNCSERVICESTATE"/>.
		/// </returns>
		private static SYNCSERVICESTATE GetServerSynchronizationReadiness(SecurityClass security, out SyncServerConfigurationDO serverSyncConfig)
		{
			SyncServerConfigurations serverSyncConfigs = new SyncServerConfigurations();
			serverSyncConfig = serverSyncConfigs.Get(security);

			SYNCSERVICESTATE serverReadiness = SYNCSERVICESTATE.READY;

			if (!serverSyncConfig.AllowSynchronizationFlag)
			{
				serverReadiness = SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING;
			}
			else if (!serverSyncConfig.AcceptFMUserAuthenticationFlag && !serverSyncConfig.AcceptClientCertificateAuthenticationFlag)
			{
				// Server must be configured to accept either FuelsManager User Credentials or a Client Certificate in order to synchronize.  
				// Otherwise the synchronization engine can't validate whether or not the sites being requested for synchronization are permitted to be
				// synchronized by the remote initiator.
				serverReadiness = SYNCSERVICESTATE.ENTERPRISE_FM_AUTHENTICATION_NOT_CONFIGURED;
			}

			return serverReadiness;
		}

		#endregion Static Private Helper Methods

		#region Service Interface Implementation - IEnterpriseSyncProxy

		/// <summary>
		/// The create session.
		/// </summary>
		/// <param name="syncLoginRequest">
		/// The sync login request.
		/// </param>
		/// <param name="securityContext">
		/// The security context.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <returns>
		/// The <see cref="SYNCSERVICESTATE"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// </exception>
		/// <exception cref="FaultException">
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SYNCSERVICESTATE CreateSession(SecuritySyncLoginRequest syncLoginRequest, out SecurityClass securityContext, out string message)
		{
			SYNCSERVICESTATE serverReadiness = SYNCSERVICESTATE.UNAVAILABLE;
			SecurityClass security = null;
			message = string.Empty;

			securityContext = null;

			var innerSecurity = new SecurityClass();
			innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
			innerSecurity.AddRight(RIGHT.VIEW_SYNC_CONFIG_SERVER_SETTINGS);


			try
			{
				SyncServerConfigurationDO syncServerConfig = null;
				serverReadiness = GetServerSynchronizationReadiness(innerSecurity, out syncServerConfig);

				switch (serverReadiness)
				{
					case SYNCSERVICESTATE.ENTERPRISE_FM_AUTHENTICATION_NOT_CONFIGURED:
						message = ErrorConstants.SYNC_ERR_MSG_08011;
						break;

					case SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING:
						message = ErrorConstants.SYNC_ERR_MSG_08010;
						break;

					case SYNCSERVICESTATE.READY:
						SecurityLoginRequest loginRequest = null;

						switch (this.CreateSecurityLoginRequest(syncServerConfig, syncLoginRequest, out loginRequest))
						{
							case LOGINCONVERSIONRESULT.OK:

								ISites sites = new SitesClass();
								var site = sites.GetByID(innerSecurity, loginRequest.SiteID, true);
								if(site.DisableSyncTransferFlag)
								{
									serverReadiness = SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING_SITE;
									message = ErrorConstants.SYNC_ERR_MSG_08017;
									break;
								}
								else
								{
									SecurityLoginResponse loginResult = sites.Login2(loginRequest);

									// For invalid logins, the app must update the User table with the number of invalid
									// attempts. Therefore, in order to persist the update to the user table an exception
									// cannot be throw so the return value is set to error message which starts is "User".
									if ((loginResult != null) && loginResult.Result != null
									&& (loginResult.Result.ToUpper().StartsWith("USER") || loginResult.Result.ToUpper().StartsWith("LOGIN FAILED")))
									{
										serverReadiness = SYNCSERVICESTATE.FMAUTH_LOGIN_FAILURE;

										message = loginResult.Result.ToUpper().StartsWith("USER") ? loginResult.Result : ErrorConstants.SYNC_ERR_MSG_08014;
									}
									else
									{
										// Return the security syncContext for the Enterprise Synchronization Session.
										security = loginResult.Security;
										securityContext = security;

										// Updates the session with the node guid of the incoming synchronization client.
										// This session is a standard interactive FuelsManager session.  They are created/deleted.
										SyncDBI.UpdateSessionWithSynchronizationNode(security, syncLoginRequest.SourceNodeGuid);

										// Create an internal synchronization session that we can associate synchronization details, conflicts, errors and status information with.
										var syncSession = new SyncSessionLogDO();
										syncSession.StartDate = DateTimeOffset.Now;
										syncSession.IdentityGuid = syncLoginRequest.SyncSessionID;

										// Our synchronization session is tracked in a different table, but we'll use the same session token so that we can
										// close it out when the session ends.
										syncSession.RemoteNodeGuid = syncLoginRequest.SourceNodeGuid;
										syncSession.RemoteNodeMachineName = syncLoginRequest.SourceNodeMachineName;
										syncSession.SyncRequestTypeIndex = syncLoginRequest.SyncRequestTypeIndex;
										syncSession.SyncTransferTypeIndex = syncLoginRequest.SyncTransferTypeIndex;

										ISyncSessionLogs syncSessions = new SyncSessionLogs();
										syncSessions.Add(security, syncSession);

										message = "Enterprise Synchronization Session Opened";
									}
								}

								break;
							case LOGINCONVERSIONRESULT.LOGINMISSING:
								message = ErrorConstants.SYNC_ERR_MSG_08012;
								break;

							case LOGINCONVERSIONRESULT.CLIENTCERTMISSING:
								message = ErrorConstants.SYNC_ERR_MSG_08013;
								break;
							}

							break;
				}
			}
			catch (Exception eX)
			{
				string errorMessage = string.Format("Unable to create enterprise synchronization session: {0}", eX.Message);

				var logger = new Logger("EnterpriseSynchronizationService");
				logger.Error(errorMessage);

				throw new FaultException(errorMessage);
			}

			return serverReadiness;
		}

		/// <summary>
		/// The end session.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionID">Synchronization Session ID associated with the FuelsManager Session being terminated.</param>
		/// <param name="finalSessionStatus">
		/// The final session status.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string EndSession(SecurityClass security, Guid syncSessionID, SYNCSESSIONSTATUS finalSessionStatus)
		{
			string endSessionMessage = string.Empty;

			try
			{
				ISyncSessionLogs syncSessions = new SyncSessionLogs();
				SyncSessionLogDO syncSession = syncSessions.Get(security, syncSessionID);

				if (null != syncSession)
				{
					syncSession.EndDate = DateTimeOffset.Now;
					syncSession.SyncSessionStateIndex = SYNCSESSIONSTATE.CLOSE;
					syncSession.SyncSessionStatusIndex = finalSessionStatus;
				if (finalSessionStatus == SYNCSESSIONSTATUS.USERSTOP)
				{
					syncSession.SyncSessionStatusIndex = SYNCSESSIONSTATUS.USERSTOP;
				}
				else if (finalSessionStatus == SYNCSESSIONSTATUS.SYSSTOP)
				{
					syncSession.SyncSessionStatusIndex = SYNCSESSIONSTATUS.SYSSTOP;
				}
				else if (syncSession.Conflicts == 0)
				{
					syncSession.SyncSessionStatusIndex = SYNCSESSIONSTATUS.COMPOK;
				}
				else
				{
					syncSession.SyncSessionStatusIndex = SYNCSESSIONSTATUS.COMPCON;
				}
			}

				syncSessions.Modify(security, syncSession);

				ISites sites = new SitesClass();
				sites.Logout(security);

				endSessionMessage = "Enterprise Synchronization Session Closed";
			}
			catch (Exception eX)
			{
				string message = string.Format("Unable to close enterprise synchronization session: {0}", eX.Message);

				var logger = new Logger("EnterpriseSynchronizationService");
				logger.Error(message);

				throw new FaultException(message);
			}

			return endSessionMessage;
		}

		/// <summary>
		/// The get server id.
		/// </summary>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		public Guid GetServerID()
		{
			var innerSecurity = new SecurityClass();
			innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
			innerSecurity.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);

			return SyncDBI.GetServerNodeID(innerSecurity);
		}

		/// <summary>
		/// The get node name.
		/// </summary>
		/// <returns>
		/// The name of the synchronization node.
		/// </returns>
		public string GetNodeName()
		{
			var innerSecurity = new SecurityClass();
			innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
			innerSecurity.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);

			return SyncDBI.GetServerNodeName(innerSecurity);
		}

		/// <summary>
		/// The get synchronization site list.
		/// </summary>
		/// <param name="remoteSiteList">
		/// The remote site list.
		/// </param>
		/// <param name="syncContext">
		/// The sync context.
		/// </param>
		/// <returns>
		/// The <see cref="SiteSyncList"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// </exception>
		public SiteSyncList GetSynchronizationSiteList(SiteSyncList remoteSiteList, SyncContextFM syncContext)
		{
			if (null != syncContext)
			{
				var sites = new SitesClass();
				Guid remoteSiteGuid = syncContext.SiteGuid.HasValue ? syncContext.SiteGuid.Value : Guid.Empty;

				if (remoteSiteGuid == Guid.Empty)
				{
					SiteClass remoteSite = sites.GetByID(syncContext.ServerSecurity, syncContext.SiteID);
					remoteSiteGuid = (null != remoteSite) ? remoteSite.SiteGuid : Guid.Empty;
				}

				// If this is still not set, we can't synchronize for this remoteSite because we don't know who it is
				if (remoteSiteGuid == Guid.Empty)
				{
					throw new FaultException(string.Format("Unable to locate a valid Site for the specified information {0} : {1}", syncContext.SiteID, syncContext.SiteGuid));
				}

				SiteSyncList orderedList = sites.EnumerateSiteSynchronizationListBySiteSQL(syncContext.ServerSecurity, remoteSiteGuid);

				// Merge the Remote Node's Site Synchronization List with ours to provide the Remote Node with the complete list of Sites that need to be synchronized.
				// Note: If the performance can be maintained, we may want to run a query that determines the following for EACH Site:
				//			1) Does the Site have any Entities Assigned to the Target Site (SyncContext.SiteGuid)
				//			2) If Entities are Assigned, have they changed?
				//
				//			(Don't do this for Sites that were listed in the RemoteSiteList.  The Remote Node is already telling us that it has synchronization data to 
				//			upload for those Sites so don't waste time checking on this end.  We're already committed to including that Site in the Sync Session)
				// If the answer to either one of the above checks is NO, then the Site is removed from the Synchronization List.
				orderedList.Merge(remoteSiteList);

				return orderedList;
			}

			return remoteSiteList;
		}

		/// <summary>
		/// Gets the current enterprise synchronization anchor.
		/// </summary>
		/// <returns>
		/// The current Enterprise Sync Anchor
		/// </returns>
		public long GetEnterpriseSyncAnchor()
		{
			return SyncDBI.GetMaxSyncAnchor();
		}

		/// <summary>
		/// The apply changes.
		/// </summary>
		/// <param name="groupMetadata">
		/// The group meta data.
		/// </param>
		/// <param name="dataSetSurrogateBytes">
		/// The data set.
		/// </param>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <param name="syncContextFMBytes">
		/// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		/// </param>
		/// <returns>
		/// The <see cref="SyncContext"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Throws a new <see cref="FaultException"/> wrapping any exception encountered while applying the client changes to the server.
		/// </exception>
		public SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, byte[] dataSetSurrogateBytes, SyncSession syncSession, byte[] syncContextFMBytes)
		{
			SyncContext context = null;

			try
			{
				SyncContextFM syncContextFM = (SyncContextFM)CompressionProcessor.DecompressObject(syncContextFMBytes);

				// If we don't have an existing ServerSyncProvider OR 
				// the incoming ClientProvider is different from the last one, get a new ServerProvider
				if (null == this._ServerSyncProvider || !this.IsSameSyncProfileAndScope(syncContextFM))
				{
					var currentSyncSession = this.GetCurrentSyncSession(syncContextFM.ServerSecurity, syncContextFM);
					this._ServerSyncProvider = this.GetServerProvider(syncContextFM.ServerSecurity, syncContextFM, currentSyncSession);
				}

				if (null != this._ServerSyncProvider)
				{
					this._LastServerSyncProfileID = syncContextFM.CurrentSyncProfileID;
					this._LastServerSyncScopeID = syncContextFM.CurrentSyncScopeID;

					DataSet dataSet = CompressionProcessor.DecompressDataSet(dataSetSurrogateBytes);

					context = this._ServerSyncProvider.ApplyChanges(groupMetadata, dataSet, syncSession);

					// To improve online synchronization, don't return the data set that was originally sent in, the client sync proxy will need to
					// set this back by referencing the original GroupProgress dataset.
					context.DataSet = null;
					context.GroupProgress.Changes.Clear();
				}

				return context;
			}
			catch (Exception eX)
			{
				// string message = ExceptionFormatter.Format(eX, string.Format("{0} - Exception at the service boundary", _ServiceName));
				// TraceProxy.Instance.Write(LogMessageSeverity.Critical, "TraceProxy", provider, "", eX, null, _ServiceName, "ApplyChanges", message, null);
				// throw new FaultException(message);
				string message = string.Format("Error Applying Changes to Server: {0}", eX.Message);

				var logger = new Logger("EnterpriseSynchronizationService");
				logger.Error(message);

				throw new FaultException(message, new FaultCode("APPLYCHANGESERROR"));
			}
			finally
			{
				if (null != this._ServerSyncProvider)
				{
					((IDisposable)this._ServerSyncProvider).Dispose();
					this._ServerSyncProvider = null;
				}
			}
		}

		/// <summary>
		/// The get changes.
		/// </summary>
		/// <param name="groupMetadata">
		/// The group meta data.
		/// </param>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <param name="syncContextFMBytes">
		/// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		/// </param>
		/// <param name="dataSetSurrogateBytes">
		/// Dataset as a byte array.
		/// </param>
		/// <returns>
		/// The <see cref="SyncContext"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Throws a new <see cref="FaultException"/> wrapping any exception encountered while retrieving server changes to be sent down to the client.
		/// </exception>
		public SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession, byte[] syncContextFMBytes, out byte[] dataSetSurrogateBytes)
		{
			SyncContext context = null;

			dataSetSurrogateBytes = null;

			try
			{
				SyncContextFM syncContextFM = (SyncContextFM)CompressionProcessor.DecompressObject(syncContextFMBytes);

				// If we don't have an existing ServerSyncProvider OR 
				// the incoming ClientProvider is different from the last one, get a new ServerProvider
				if (null == this._ServerSyncProvider || !this.IsSameSyncProfileAndScope(syncContextFM))
				{
					var currentSyncSession = this.GetCurrentSyncSession(syncContextFM.ServerSecurity, syncContextFM);
					this._ServerSyncProvider = this.GetServerProvider(
							syncContextFM.ServerSecurity, syncContextFM, currentSyncSession);
				}

				if (null != this._ServerSyncProvider)
				{
					this._LastServerSyncProfileID = syncContextFM.CurrentSyncProfileID;
					this._LastServerSyncScopeID = syncContextFM.CurrentSyncScopeID;

					context = this._ServerSyncProvider.GetChanges(groupMetadata, syncSession);

					dataSetSurrogateBytes = CompressionProcessor.CompressDataSet(context.DataSet);

					context.DataSet = null;
					context.GroupProgress = null;
				}

				return context;
			}
			catch (Exception eX)
			{
				// string message = ExceptionFormatter.Format(eX, string.Format("{0} - Exception at the service boundary", _ServiceName));
				// TraceProxy.Instance.Write(LogMessageSeverity.Critical, "TraceProxy", provider, "", eX, null, _ServiceName, "GetChanges", message, null);
				// throw new FaultException(message);
				string message = string.Format("Error Retrieving Changes from Server: {0}", eX.Message);

				var logger = new Logger("EnterpriseSynchronizationService");
				logger.Error(message);

				throw new FaultException(message, new FaultCode("GETCHANGESERROR"));
			}
			finally
			{
				if (null != this._ServerSyncProvider)
				{
					((IDisposable)this._ServerSyncProvider).Dispose();
					this._ServerSyncProvider = null;
				}
			}
		}

		/// <summary>
		/// The get schema.
		/// </summary>
		/// <param name="tableNames">
		/// The table names.
		/// </param>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <param name="syncContextFMBytes">
		/// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSchema"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Throws a new <see cref="FaultException"/> wrapping any exception encountered while retrieving schema information from the server.
		/// </exception>
		public SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession, byte[] syncContextFMBytes)
		{
			SyncSchema schema = null;

			try
			{
				SyncContextFM syncContextFM = (SyncContextFM)CompressionProcessor.DecompressObject(syncContextFMBytes);

				// If we don't have an existing ServerSyncProvider OR 
				// the incoming ClientProvider is different from the last one, get a new ServerProvider
				if (null == this._ServerSyncProvider || !this.IsSameSyncProfileAndScope(syncContextFM))
				{
					var currentSyncSession = this.GetCurrentSyncSession(syncContextFM.ServerSecurity, syncContextFM);
					this._ServerSyncProvider = this.GetServerProvider(
							syncContextFM.ServerSecurity, syncContextFM, currentSyncSession);
				}

				if (null != this._ServerSyncProvider)
				{
					this._LastServerSyncProfileID = syncContextFM.CurrentSyncProfileID;
					this._LastServerSyncScopeID = syncContextFM.CurrentSyncScopeID;

					schema = this._ServerSyncProvider.GetSchema(tableNames, syncSession);
				}

				return schema;
			}
			catch (Exception eX)
			{
				// string message = ExceptionFormatter.Format(eX, string.Format("{0} - Exception at the service boundary", _ServiceName));
				// TraceProxy.Instance.Write(LogMessageSeverity.Critical, "TraceProxy", provider, "", eX, null, _ServiceName, "GetSchema", message, null);
				// throw new FaultException(message);
				string message = string.Format("Error Retrieving Schema from Server: {0}", eX.Message);

				var logger = new Logger("EnterpriseSynchronizationService");
				logger.Error(message);

				throw new FaultException(message, new FaultCode("GETSCHEMAERROR"));
			}
			finally
			{
				if (null != this._ServerSyncProvider)
				{
					((IDisposable)this._ServerSyncProvider).Dispose();
					this._ServerSyncProvider = null;
				}
			}
		}

		/// <summary>
		/// The get server info.
		/// </summary>
		/// <param name="syncSession">
		/// The sync session.
		/// </param>
		/// <param name="syncContextFMBytes">
		/// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		/// </param>
		/// <returns>
		/// The <see cref="SyncServerInfo"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// Throws a new <see cref="FaultException"/> wrapping any exception encountered while retrieving server information.
		/// </exception>
		public SyncServerInfo GetServerInfo(SyncSession syncSession, byte[] syncContextFMBytes)
		{
			var serverInfo = new SyncServerInfo();

			try
			{
				SyncContextFM syncContextFM = (SyncContextFM)CompressionProcessor.DecompressObject(syncContextFMBytes);

				// If we don't have an existing ServerSyncProvider OR 
				// the incoming ClientProvider is different from the last one, get a new ServerProvider
				if (null == this._ServerSyncProvider || !this.IsSameSyncProfileAndScope(syncContextFM))
				{
					var currentSyncSession = this.GetCurrentSyncSession(syncContextFM.ServerSecurity, syncContextFM);
					this._ServerSyncProvider = this.GetServerProvider(syncContextFM.ServerSecurity, syncContextFM, currentSyncSession);
				}

				if (null != this._ServerSyncProvider)
				{
					this._LastServerSyncProfileID = syncContextFM.CurrentSyncProfileID;
					this._LastServerSyncScopeID = syncContextFM.CurrentSyncScopeID;

					serverInfo = this._ServerSyncProvider.GetServerInfo(syncSession);
				}

				return serverInfo;
			}
			catch (Exception eX)
			{
				// string message = ExceptionFormatter.Format(eX, string.Format("{0} - Exception at the service boundary", _ServiceName));
				// TraceProxy.Instance.Write(LogMessageSeverity.Critical, "TraceProxy", provider, "", eX, null, _ServiceName, "GetServerInfo", message, null);
				// throw new FaultException(message);
				string message = string.Format("Error Retrieving Server Info: {0}", eX.Message);

				var logger = new Logger("EnterpriseSynchronizationService");
				logger.Error(message);

				throw new FaultException(message, new FaultCode("GETSERVERINFOERROR"));
			}
			finally
			{
				if (null != this._ServerSyncProvider)
				{
					((IDisposable)this._ServerSyncProvider).Dispose();
					this._ServerSyncProvider = null;
				}
			}
		}

		public void PurgeLogs(SecurityClass security, Guid syncNodeGuid, int maximumDaysToRetainLogs)
		{
			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.CommandText = "[sync].[usp_SyncPurgeLogs]";
				sqlCommand.Parameters.AddWithValue("@RemoteNodeGuid", syncNodeGuid);
				sqlCommand.Parameters.AddWithValue("@MaximumDaysToRetainLogs", maximumDaysToRetainLogs);
				try
				{
					var consolidatedDA = new ConsolidatedDAClass();
					consolidatedDA.ExecuteQuery(security, sqlCommand);
				}
				catch (Exception e)
				{
					eventLog.WriteEntry(
						string.Format("PurgeLogs exception encountered: {0}", e.Message),
						FMEventLogEntryType.Error);
				}
			}
		}

		/// <summary>
		/// Reprocesses the conflicts.
		/// </summary>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SyncConflictResolutionStatus ReprocessConflicts(SecurityClass security, Guid syncNodeGuid, Guid syncSessionID, SyncConflictResolutionStatus syncConflictResultionStatus)
		{
			// LastRowVersion indicates completion.  When there are still records to process it will be the last RecordRowVersion processed
			Int64 startRowVersion = syncConflictResultionStatus.LastRowVersion.Value;
			syncConflictResultionStatus.LastRowVersion = null;

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var syncTableToScopeMappings = new SyncTableToScopeMappings();
			string tableName = null;
			SyncTableToScopeMapColumnCollection syncTableToScopeMapColumnCollection = null;
 
			var consolidatedDA = new ConsolidatedDAClass();
			var syncRecordConflicts = new SyncRecordConflicts();
			var syncSessionLogs = new SyncSessionLogs();

			try
			{
				var syncSessionLog = syncSessionLogs.Get(security, syncSessionID);

				if (null == syncSessionLog)
				{
					throw new Exception(
						string.Format(
							"ReprocessConflicts : Unable to locate Synchronization Session Log associated with the specified Synchronization Session. {0}",
							syncSessionID.ToString()));
				}

				// in the first pass process all conflicts for current session
				// in the second pass process conflicts chronologically breaking out on the first failure
				while(syncConflictResultionStatus.Pass < 2)
				{
					SyncRecordConflictCollection syncRecordConflictCollection;

					if (syncConflictResultionStatus.Pass == 0)
					{
						if (syncSessionLog.Conflicts == 0)
						{
							syncConflictResultionStatus.Pass++;                  
							continue;
						}

						syncSessionLog.SyncSessionStatusIndex = SYNCSESSIONSTATUS.COMPCON;
						syncRecordConflictCollection = syncRecordConflicts.EnumerateBySyncSessionLog(security, syncSessionLog.IdentityGuid, MaxConflictsPerCall, startRowVersion );
					}

					else
					{
						if (syncSessionLog.SyncSessionStatusIndex == SYNCSESSIONSTATUS.COMPCON)
						{
							if (syncSessionLog.Conflicts == 0)
							{
								syncSessionLog.SyncSessionStatusIndex = SYNCSESSIONSTATUS.COMPOK;
								syncSessionLogs.Modify(security, syncSessionLog);
							}
							else
							{
								SyncHelperFM.WriteConflictAlarmAndEvent(security, "Sync Session : " + syncSessionLog.IdentityGuid);
							}
						}

						try
						{
							syncRecordConflictCollection = syncRecordConflicts.EnumerateUnresolved(security, syncNodeGuid, MaxConflictsPerCall, startRowVersion);
						}
						catch (Exception e)
						{
							eventLog.WriteEntry(
								string.Format("Synchronization exception encountered (ReprocessConflicts - EnumerateUnresolved): {0}", e.Message), FMEventLogEntryType.Error);

							eventLog.WriteEntry(string.Format("StackTrace:", e.StackTrace), FMEventLogEntryType.Error);

							throw;
						}
					}

					if(syncRecordConflictCollection.Count > 0
					&& syncRecordConflictCollection.Count == MaxConflictsPerCall)
					{
						syncConflictResultionStatus.LastRowVersion = syncRecordConflictCollection[syncRecordConflictCollection.Count-1].RecordRowVersion;
					}

					foreach (var syncRecordConflict in syncRecordConflictCollection)
					{

						syncRecordConflict.Retrys++;

						try
						{
							var variable = syncRecordConflicts.Get(security, syncRecordConflict.IdentityGuid);

							if (variable == null)
							{
								eventLog.WriteEntry(string.Format("Conflict resolution for table {0}, recordkey: {1} and conflictguid {2} was skipped.  Possibly resolved by a conflict with the same recordkey.", tableName, syncRecordConflict.RecordKey, syncRecordConflict.IdentityGuid), FMEventLogEntryType.Warning);
								continue;
							}

							syncRecordConflict.Parameters = variable.Parameters;
						}
						catch (Exception e)
						{
							eventLog.WriteEntry(
								string.Format("Synchronization exception encountered (ReprocessConflicts - Retrieving Conflict Parameters): {0}", e.Message), FMEventLogEntryType.Error);

							eventLog.WriteEntry(string.Format("StackTrace:", e.StackTrace), FMEventLogEntryType.Error);
							throw;
						}

						if (syncRecordConflict.Parameters == null)
						{
							throw new Exception(
								string.Format("ReprocessConflicts : No Parameters available for conflict: {0}", syncRecordConflict.IdentityGuid.ToString()));
						}

						try
						{
							if (string.IsNullOrEmpty(tableName)
								|| tableName
								!= syncRecordConflict.TableName.Substring(
									1 + syncRecordConflict.TableName.IndexOf(".", System.StringComparison.Ordinal)))
							{
								tableName =
									syncRecordConflict.TableName.Substring(
										1 + syncRecordConflict.TableName.IndexOf(".", System.StringComparison.Ordinal));

								SyncTableToScopeMapDO syncTableToScopeMapDO = null;

								try
								{
									syncTableToScopeMapDO = syncTableToScopeMappings.GetById(security, tableName);

									if (syncTableToScopeMapDO == null)
									{
										throw new Exception("ReprocessConflicts : Error loading syncTableToScopeMapping for table " + tableName);
									}
								}
								catch (Exception e)
								{
									eventLog.WriteEntry(
										string.Format("Synchronization exception encountered (ReprocessConflicts - Retrieving SyncTableToSyncScopeMap record for {1}): {0}", e.Message, tableName), FMEventLogEntryType.Error);

									eventLog.WriteEntry(string.Format("StackTrace:", e.StackTrace), FMEventLogEntryType.Error);

									throw;
								}

								try
								{
									syncTableToScopeMapColumnCollection = SyncProviderHelperFM.GetSyncTableColumns(
										security,
										syncTableToScopeMapDO.IdentityGuid);

									if (syncTableToScopeMapColumnCollection == null)
									{
										throw new Exception(
											"ReprocessConflicts : Error loading syncTableToScopeMapColumnCollection for table " + tableName);
									}
								}
								catch (Exception e)
								{
									eventLog.WriteEntry(
										string.Format("Synchronization exception encountered (ReprocessConflicts - Retrieving SyncTableToScopeMapColumn records for {1} ): {0}", e.Message, tableName), FMEventLogEntryType.Error);

									eventLog.WriteEntry(string.Format("StackTrace:", e.StackTrace), FMEventLogEntryType.Error);

									throw;
								}
							}
						}
						catch (Exception e)
						{
							eventLog.WriteEntry(
								string.Format("Synchronization exception encountered (ReprocessConflicts - Retrieving SyncTableToSyncScopeMapColumn(s) ): {0}", e.Message), FMEventLogEntryType.Error);

							eventLog.WriteEntry(string.Format("StackTrace:", e.StackTrace), FMEventLogEntryType.Error);

							throw;
						}

						using (var sqlCommand = new SqlCommand())
						{
							if (string.IsNullOrEmpty(syncRecordConflict.CommandText))
							{
								throw new Exception(
									"ReprocessConflicts : No CommandText available for conflict " + syncRecordConflict.IdentityGuid.ToString());
							}

							sqlCommand.CommandText = syncRecordConflict.CommandText;
							sqlCommand.CommandType = syncRecordConflict.CommandType;

							try
							{
								foreach (var parameter in syncRecordConflict.Parameters)
								{
									if (parameter.Key.Contains("sync") || syncTableToScopeMapColumnCollection == null)
									{
										sqlCommand.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
										if (parameter.Key.Equals("@sync_row_count"))
										{
											sqlCommand.Parameters["@sync_row_count"].SqlDbType = SqlDbType.Int;
											sqlCommand.Parameters["@sync_row_count"].Direction = ParameterDirection.Output;
										}
									}
									else
									{
										var syncTableToScopeMapColumnDo =
											syncTableToScopeMapColumnCollection.Find(x => "@" + x.ColumnName == parameter.Key);
										if (syncTableToScopeMapColumnDo == null)
										{
											sqlCommand.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
										}
										else
										{
											var sqlParameter = new SqlParameter(
												parameter.Key,
												SyncProviderHelperFM.GetSqlDbTypeFromString(syncTableToScopeMapColumnDo.ColumnType),
												syncTableToScopeMapColumnDo.ColumnSize.HasValue ? syncTableToScopeMapColumnDo.ColumnSize.Value : 0);
											sqlParameter.Value = parameter.Value;
											sqlCommand.Parameters.Add(sqlParameter);
										}
									}
								}
							}
							catch (Exception e)
							{
								eventLog.WriteEntry(
									string.Format("Synchronization exception encountered (ReprocessConflicts - Adding SQL Parameters to Command for {1}): {0}", e.Message, tableName), FMEventLogEntryType.Error);

								eventLog.WriteEntry(string.Format("StackTrace:", e.StackTrace), FMEventLogEntryType.Error);

								throw;
							}

							try
							{
								consolidatedDA.ExecuteQuery(security, sqlCommand);
								if ((sqlCommand.CommandText.Contains("Inserts") || sqlCommand.CommandText.Contains("Updates"))
									&& (int)sqlCommand.Parameters["@sync_row_count"].Value == 0)
								{
									if (syncConflictResultionStatus.Pass == 1)
									{
										syncConflictResultionStatus.Pass++;
										break;
									}
								}
								else
								{
									syncRecordConflict.SyncConflictResolutionStatusIndex = SYNCCONFLICTRESOLUTIONSTATUS.RESOLVED;
									syncRecordConflict.ResolvedBy = security.UserID;
									syncRecordConflict.ResolvedDate = DateTimeOffset.Now;
								}
							}
							catch (Exception e)
							{
								var rowError = new StringBuilder();
								rowError.AppendLine(
									string.Format(
										"Conflict Type: {0}{1}",
										SyncTypes.GetSyncConflictTypeString(syncRecordConflict.SyncConflictTypeIndex),
										Environment.NewLine));

								if (!string.IsNullOrEmpty(e.Message))
								{
									rowError.AppendLine(string.Format("Conflict Error Message: {0}{1}", e.Message, Environment.NewLine));
								}

								var actionMessage = SyncProviderHelperFM.GetConflictApplyActionMessage(ApplyAction.RetryNextSync, true);
								if (!string.IsNullOrEmpty(actionMessage))
								{
									rowError.AppendLine(actionMessage);
								}


								syncRecordConflict.ConflictDescription = rowError.ToString();
								eventLog.WriteEntry(
									string.Format("Synchronization exception encountered: {0}", e.Message),
									FMEventLogEntryType.Error);

								if (syncConflictResultionStatus.Pass == 1)
								{
									syncConflictResultionStatus.Pass++;
									break;
								}
							}
							finally
							{
								if (syncConflictResultionStatus.Pass == 0
								&& syncRecordConflict.SyncConflictResolutionStatusIndex == SYNCCONFLICTRESOLUTIONSTATUS.RESOLVED)
								{
									syncSessionLog.Conflicts--;
									syncRecordConflicts.Purge(security, syncRecordConflict.IdentityGuid);
								}
								else
								{
									syncRecordConflicts.Modify(security, syncRecordConflict);
								}
							}
						}
					}

					if(syncConflictResultionStatus.LastRowVersion.HasValue)
					{
						break;
					}

					syncConflictResultionStatus.Pass++;
					startRowVersion = 0;
				}
			}
			catch (Exception e)
			{
				eventLog.WriteEntry(
					string.Format("Synchronization exception encountered (ReprocessConflicts): {0}", e.Message), FMEventLogEntryType.Error);

				eventLog.WriteEntry(string.Format("StackTrace:", e.StackTrace), FMEventLogEntryType.Error);

				throw;
			}
			finally
			{

			}


			return syncConflictResultionStatus;
		}

		public void SynchronizeValueArchive(SecurityClass security, List<ArchiveDataElement> archiveDataElementList)
		{
			PointTagArchiveDatabase.AddArchiveData(security, archiveDataElementList);
		}

		public void SynchronizeAlarmAndEventArchive(SecurityClass security, List<AandEDataElement> archiveDataElementList)
		{
			AlarmAndEventArchiveDataBase.AddArchiveData(security, archiveDataElementList);
		}



		#endregion Service Interface Implementation - IEnterpriseSyncProxy

		#region Private Helper Methods
		/// <summary>
		/// The is same sync profile and scope.
		/// </summary>
		/// <param name="syncContext">
		/// The sync context.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Throws an exception if the passed in <see cref="SyncContextFM"/> parameter is null.
		/// </exception>
		private bool IsSameSyncProfileAndScope(SyncContextFM syncContext)
		{
			if (null == syncContext)
			{
					throw new ArgumentNullException("syncContext", @"SyncContext cannot be null.");
			}

			if (this._LastServerSyncProfileID == syncContext.CurrentSyncProfileID
					&& this._LastServerSyncScopeID == syncContext.CurrentSyncScopeID)
			{
					return true;
			}

			return false;
		}

		/// <summary>
		/// The create security login request.
		/// </summary>
		/// <param name="serverSyncConfig">
		/// The server sync config.
		/// </param>
		/// <param name="syncLoginRequest">
		/// The sync login request.
		/// </param>
		/// <param name="loginRequest">
		/// The login request.
		/// </param>
		/// <returns>
		/// The <see cref="LOGINCONVERSIONRESULT"/>.
		/// </returns>
		private LOGINCONVERSIONRESULT CreateSecurityLoginRequest(SyncServerConfigurationDO serverSyncConfig, SecuritySyncLoginRequest syncLoginRequest, out SecurityLoginRequest loginRequest)
		{
			LOGINCONVERSIONRESULT result = LOGINCONVERSIONRESULT.OK;

			loginRequest = new SecurityLoginRequest() { SiteID = syncLoginRequest.SiteID };

			// Only client cert authentication is permitted and we're missing a client cert
			if ((serverSyncConfig.AcceptClientCertificateAuthenticationFlag && (syncLoginRequest.ClientCertificate != null && syncLoginRequest.ClientCertificate.Length > 0))
			|| (serverSyncConfig.AcceptFMUserAuthenticationFlag && !string.IsNullOrEmpty(syncLoginRequest.UserID) && !string.IsNullOrEmpty(syncLoginRequest.Password)))
			{
				// If we accept client certificates and one was provided, we'll use it first.
				if (serverSyncConfig.AcceptClientCertificateAuthenticationFlag && (syncLoginRequest.ClientCertificate != null && syncLoginRequest.ClientCertificate.Length > 0))
				{
					var cs = new X509Certificate2(syncLoginRequest.ClientCertificate);

					//if (1 == cs.Flags)
					if (cs.Verify())
					{
						loginRequest.CACEnabled = true;

						string strCn = cs.Subject;
						strCn = strCn.Substring(strCn.IndexOf("CN=", StringComparison.Ordinal) + 3);
						int index = strCn.IndexOf(',');

						if (index >= 0)
						{
								strCn = strCn.Remove(index);
						}

						strCn = strCn.Trim().Replace(",", string.Empty); // remove comma from CAC login

						loginRequest.UserID = strCn.Trim().Replace("'", string.Empty); // remove apostrophes from CAC login
						loginRequest.Password = syncLoginRequest.Password;
					}
				}
				else if (serverSyncConfig.AcceptFMUserAuthenticationFlag)
				{
					loginRequest.UserID = syncLoginRequest.UserID;
					loginRequest.Password = syncLoginRequest.Password;
				}
			}
			else if (serverSyncConfig.AcceptClientCertificateAuthenticationFlag
			&& (syncLoginRequest.ClientCertificate == null 
			|| (syncLoginRequest.ClientCertificate != null && syncLoginRequest.ClientCertificate.Length > 0)))
			{
				// Client certs are accepted and we're missing the certificate.
				result = LOGINCONVERSIONRESULT.CLIENTCERTMISSING;
			}
			else if (serverSyncConfig.AcceptFMUserAuthenticationFlag
			&& (string.IsNullOrEmpty(syncLoginRequest.UserID)
			|| string.IsNullOrEmpty(syncLoginRequest.Password)))
			{
				// Username/passwords are accepted and we're missing parts of the login information.
				result = LOGINCONVERSIONRESULT.LOGINMISSING;
			}

			return result;
		}

		/// <summary>
		/// The get server provider.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncContext">
		/// The sync context.
		/// </param>
		/// <param name="syncSessionDo">
		/// The sync Session Do.
		/// </param>
		/// <returns>
		/// The <see cref="ISyncServerProviderFM"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// An exception is thrown if there are exceptions encountered while locating a corresponding synchronization scope that is associated with this request.
		/// </exception>
		private ISyncServerProviderFM GetServerProvider(SecurityClass security, SyncContextFM syncContext, SyncSessionLogDO syncSessionDo)
		{
			object serverProvider = null;

			if (!string.IsNullOrEmpty(syncContext.CurrentSyncProfileID)
					&& !string.IsNullOrEmpty(syncContext.CurrentSyncScopeID))
			{
					try
					{
						// From an Enterprise point of view, we should ALWAYS base our RECEIVING Provider on the {Complete} profile because it represents the
						// superset of the scopes and the master order of the table synchronization.
						var syncProfiles = new SyncProfiles();
						var syncProfile = syncProfiles.GetById(syncContext.ServerSecurity, SyncConstants.DEFAULT_PROFILE_COMPLETE);

						if (null != syncProfile)
						{
							// Technically the client can define its own set of scopes which may not match 1-to-1 against how the master scope list is segmented.
							// We'll create a dynamic scope that matches the the client and then we'll fill it with the tables they've specified. 
							SyncScopeDO syncScope = new SyncScopeDO();

							syncScope.SyncProfileGuid = syncProfile.IdentityGuid;
							syncScope.ID = syncContext.CurrentSyncScopeID;
							syncScope.FriendlyName = syncContext.CurrentSyncScopeID;

							// Populate this dynamic scope with the TableToScope Mapping information so the provider knows which procedures to use on the
							// Enterprise server and what columns we support.
							using (var dbi = new SyncTableToScopeMapDBI(syncContext.ServerSecurity.UserID))
							{
									List<string> tableList = syncContext.SupportedColumnsByTable.Keys.ToList();

									syncScope.SyncScopeTables = dbi.GetListForProfileByTableNames(syncContext.ServerSecurity, syncProfile.IdentityGuid, string.Join(",", tableList.ToArray()));
							}

							if (null != syncScope.SyncScopeTables)
							{
									serverProvider = new ServerSyncProviderFM(syncContext.ServerSecurity, syncScope, syncContext, syncSessionDo);
							}
						}
					}
					catch (Exception eX)
					{
						string message = string.Format("Error Locating ServerSyncProvider Profile/Scope that matches incoming Profile/Scope: {0}/{1}, Error: {2}", syncContext.CurrentSyncProfileID, syncContext.CurrentSyncScopeID, eX.Message);
						var logger = new Logger("EnterpriseSynchronizationService");
						logger.Error(message);

						throw new FaultException(message);
					}
			}

			return (ISyncServerProviderFM)serverProvider;
		}

		/// <summary>
		/// The get current sync session.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncContext">
		/// The sync context.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSessionLogDO"/>.
		/// </returns>
		/// <exception cref="FaultException">
		/// An exception is thrown if there was a problem retrieving an existing synchronization tracking 
		/// session or creating a new session if one did not already exist.
		/// </exception>
		private SyncSessionLogDO GetCurrentSyncSession(SecurityClass security, SyncContextFM syncContext)
		{
			SyncSessionLogDO sessionDo = null;

			if (Guid.Empty != syncContext.SyncSessionID)
			{
					try
					{
						var syncSessions = new SyncSessionLogs();
						sessionDo = syncSessions.Get(syncContext.ServerSecurity, syncContext.SyncSessionID);

						// If we didn't find a matching session, we'll create a new session.
						if (null == sessionDo)
						{
							sessionDo = new SyncSessionLogDO();
							sessionDo.IdentityGuid = syncContext.SyncSessionID;
							sessionDo.RemoteNodeGuid = syncContext.ClientID;
							sessionDo.RemoteNodeMachineName = syncContext.ClientName;

							if (syncContext.UseDateRangeSynchronization)
							{
								sessionDo.SyncDateRangeStart = syncContext.StartDateRange;
								sessionDo.SyncDateRangeEnd = syncContext.EndDateRange;
							}
							else
							{
						sessionDo.SyncDateRangeStart = null;
						sessionDo.SyncDateRangeEnd = null;
					}

					sessionDo.SyncProfileID = syncContext.CurrentSyncProfileID;
							sessionDo.SyncRequestTypeIndex = syncContext.RequestType;
							sessionDo.SyncTransferTypeIndex = syncContext.TransferType;

							using (var dbi = new SyncSessionLogDBI(syncContext.ServerSecurity.UserID))
							{
									dbi.Save(syncContext.ServerSecurity, sessionDo);
							}
						}
					}
					catch (Exception eX)
					{
						string message = string.Format("Error establishing enterprise server synchronization tracking session. Error: {0}", eX.Message);
						var logger = new Logger("EnterpriseSynchronizationService");
						logger.Error(message);

						throw new FaultException(message);
					}
			}

			return sessionDo;
		}



		#endregion Private Helper Methods
	}
}
