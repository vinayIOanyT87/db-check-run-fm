// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncControllerFM.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace FMSynchronizationService
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.ServiceModel;
	using System.Threading;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Constants;

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
		private static EventLog _EventLog = new EventLog("Application", ".", "FMSynchronizationService.SyncControllerFM");

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
		/// The _ total sites synchronized.
		/// </summary>
		private int _TotalSitesSynchronized;

		private Dictionary<string, string> _OfflineFileListForClient = new Dictionary<string, string>();

		private Dictionary<string, string> _OfflineFileListForServer = new Dictionary<string, string>();

		private bool isDisposed = false;

		private DateTimeOffset syncDateTimeOffset = DateTimeOffset.Now;

		private Mutex PointTagDataMutex = new Mutex(initiallyOwned: false, name: SynchronizationConstants.PointTagDataMutexName);

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

		#endregion Public Events

		#region Events

		#endregion Events

		#region Public Properties


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

		public bool SysStopFlag { get; set; }

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

			SiteClass siteClass = FMChannelHelper.MakeCall<ISites,SiteClass> (x => x.GetByID(this._SyncContext.Security, this._SyncContext.SiteID, false));

			if (siteClass.IdentityGuid != Guid.Empty)
			{
				if (!this._SyncContext.SiteGuid.HasValue || (this._SyncContext.SiteGuid == Guid.Empty))
				{
					this._SyncContext.SiteGuid = siteClass.IdentityGuid;
				}

				localSyncList = FMChannelHelper.MakeCall<ISites, SiteSyncList> (x => x.EnumerateSiteSynchronizationListBySiteSQL(
					this._SyncContext.Security, this._SyncContext.SiteGuid.Value));
			}
			else
			{
				siteClass.ID = this._SyncContext.SiteID;
				localSyncList.Add(0, siteClass);
			}

			return localSyncList;
		}

		public void SynchronizeArchive()
		{
	
			try
			{
				// Synchronize to within a minute of current time.
				var startDateTimeOffset = DateTimeOffset.UtcNow.AddMinutes(-1);

				var siteList = this._SiteSynchronizationList.EnumerateHostedSitesList();


				foreach (var site in siteList)
				{
					bool moreData = true;
					while (!this.SyncStopped && moreData)
					{
						moreData = FMChannelHelper.MakeCall<ISyncControllerProcessor, bool>
						(
							x =>
							{
								((IClientChannel)x).OperationTimeout = new TimeSpan(0, 62, 0);
								return x.SynchronizeArchiveValues(this._SyncContext.Security, this._SyncContext.ServerSecurity, this._ClientSyncConfig, startDateTimeOffset, site.SiteGuid);
							});
					}


					moreData = true;
					while (!this.SyncStopped && moreData)
					{
						moreData = FMChannelHelper.MakeCall<ISyncControllerProcessor, bool>
						(
							x =>
							{
								((IClientChannel)x).OperationTimeout = new TimeSpan(0, 62, 0);
								return x.SynchronizeArchiveAlarmAndEvents(this._SyncContext.Security, this._SyncContext.ServerSecurity, this._ClientSyncConfig, startDateTimeOffset, site.SiteGuid);
							});
					}
				}
			}
			catch (Exception e)
			{
				string msg = string.Format("Archive Synchronization exception encountered: {0}", e.Message);

				_EventLog.WriteEntry(msg, EventLogEntryType.Error);
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

			this._ClientSyncConfig = clientSyncConfig;

			if (string.IsNullOrEmpty(syncProfileId))
			{
				throw new ArgumentException(@"A SyncProfile ID Must be Specified.", "syncProfileId");
			}

			this._SyncContext.CurrentSyncProfileID = syncProfileId;

			try
			{
				this._LastErrorMessage = string.Empty;

				this.StartSyncSession(syncProfileId);

				SiteSyncList localSyncList = this.GetLocalSiteSynchronizationList();

				if (this._SyncContext.TransferType == SYNCTRANSFERTYPE.ONLINE)
				{
					this._SiteSynchronizationList =
						FMChannelHelper.MakeCall<ISyncControllerProcessor, SiteSyncList>(
							x => x.GetRemoteSiteSynchronizationList(this._ClientSyncConfig, localSyncList, this._SyncContext));
				}
				else
				{
					this._SiteSynchronizationList = localSyncList;
				}

				SyncProfileDO syncProfile = FMChannelHelper.MakeCall<ISyncProfiles, SyncProfileDO> (x => x.GetById(this._SyncContext.Security, syncProfileId));

				if (null != syncProfile)
				{

					SyncScopeCollection syncScopes = FMChannelHelper.MakeCall<ISyncScopes, SyncScopeCollection>(x => x.EnumerateExt(this._SyncContext.Security, syncProfile));

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
						this.UpdateSessionState(SYNCSESSIONSTATE.PROCESSINSUPD);
						this.SynchronizeInsertUpdates(syncScopes);
					}
					if (!this.SyncStopped)
					{
						syncScopes.Reverse();
						this.UpdateSessionState(SYNCSESSIONSTATE.PROCESSDEL);
						this.SynchronizeDeletes(syncScopes);
					}

					if(!this.SyncStopped)
					{
						this.SynchronizeArchive();
					}
				}
				else
				{
					_EventLog.WriteEntry(
							string.Format("Unable to locate specified SyncProfile: {0}", syncProfileId), 
							EventLogEntryType.Warning);
				}

				this.UpdateSessionState(SYNCSESSIONSTATE.POSTSYNC);

				this.CleanupSynchronizationSession();
			}
			catch (Exception eX)
			{
				string msg = string.Format("Synchronization exception encountered: {0}", eX.Message);

				_EventLog.WriteEntry(string.Format("{0}; {1}", msg, eX.StackTrace), EventLogEntryType.Error);

				SyncHelperFM.WriteErrorAlarmAndEvent(this._SyncContext.Security, msg);

				this.UpdateSessionStatus(SYNCSESSIONSTATUS.FAILED);

				throw;
			}
			finally
			{
				if (this._SyncSessionLog.SyncSessionStateIndex != SYNCSESSIONSTATE.END)
				{
					this.EndSyncSession();
				}
			}

			return this._SyncSessionLog.SyncSessionStatusIndex;
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
					if (syncScope.SyncSinglePass)
					{
						continue;
					}

					if (this.SyncStopped)
					{
							break;
					}

					switch (syncScope.SyncScopeTypeIndex)
					{
						case SYNCSCOPETYPE.GLOBAL:
							{
								SyncContextFM syncContext = this._SyncContext.Clone();

								// This core information is outside of the scope of any single Site
								syncContext.CurrentSiteID = string.Empty;
								syncContext.CurrentSiteGuid = Guid.Empty;
								syncContext.SiteType = SYNCSITETYPE.REFERENCE;
								syncContext.CurrentControllerStep = SYNCCONTROLLERSTEP.PROCESS_DELETE;
								syncContext.SyncSinglePassPhase = SYNCSINGLEPASSPHASE.SYNCROOT;

								// Sync until there are no more changes for the scope.
								while (!this.SyncStopped)
								{
									var syncResult = FMChannelHelper.MakeCall<ISyncControllerProcessor, (bool, SYNCSINGLEPASSPHASE)>(
									x =>
										{
											return x.SynchronizeScope(
												this._ClientSyncConfig,
												syncContext,
												this._SyncSessionLog,
												syncScope);
										});

									syncContext.MaxBatchSegmentRowCountEncountered = syncResult.Item1;
									syncContext.SyncSinglePassPhase = syncResult.Item2;

									if (!syncContext.MaxBatchSegmentRowCountEncountered
									&& (!syncScope.SyncSinglePass
									|| syncContext.SyncSinglePassPhase == SYNCSINGLEPASSPHASE.COMPLETE))
									{
										break;
									}
								}

								break;
							}
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

					var controllerStep = (syncScope.SyncSinglePass)
											 ? SYNCCONTROLLERSTEP.PROCESS_ALL
											 : SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE;

					switch (syncScope.SyncScopeTypeIndex)
					{
						case SYNCSCOPETYPE.GLOBAL:
							{
								SyncContextFM syncContext = this._SyncContext.Clone();

								// This core information is outside of the scope of any single Site
								syncContext.CurrentSiteID = string.Empty;
								syncContext.CurrentSiteGuid = Guid.Empty;
								syncContext.SiteType = SYNCSITETYPE.REFERENCE;
								syncContext.CurrentControllerStep = controllerStep;
								syncContext.SyncSinglePassPhase = SYNCSINGLEPASSPHASE.SYNCROOT;

								// Scope 10b (Points) requires synchronization with Enterprise Visibility; no other scope requires that
								switch (syncScope.ID)
								{
									case "Level10b":
										// Sync until there are no more changes for the scope
										while (!this.SyncStopped)
										{
											var syncResult = FMChannelHelper.MakeCall<ISyncControllerProcessor, (bool, SYNCSINGLEPASSPHASE)>(
											x =>
											{
												var hasHandle = false;
												try
												{
													bool waitResult = PointTagDataMutex.WaitOne(30000); // Wait 30 seconds
												if (!waitResult)
													{
														_EventLog.WriteEntry("SynchronizeInsertUpdates waited to long on Enterprise Visibility to finish point data", EventLogEntryType.Error);
														return (false, SYNCSINGLEPASSPHASE.COMPLETE);
													}

													hasHandle = true;
												}
												catch (AbandonedMutexException)
												{
													hasHandle = true;
												}

												try
												{
													return x.SynchronizeScope(
													this._ClientSyncConfig,
													syncContext,
													this._SyncSessionLog,
													syncScope);
												}
												finally
												{
													if (hasHandle)
													{
														PointTagDataMutex.ReleaseMutex();
													}
												}
											});

											syncContext.MaxBatchSegmentRowCountEncountered = syncResult.Item1;
											syncContext.SyncSinglePassPhase = syncResult.Item2;

											if (!syncContext.MaxBatchSegmentRowCountEncountered
											&& (!syncScope.SyncSinglePass
											|| syncContext.SyncSinglePassPhase == SYNCSINGLEPASSPHASE.COMPLETE))
											{
												break;
											}
										}
										break;
	
									default:
										// Sync until there are no more changes for the scope.
										while (!this.SyncStopped)
										{
											var syncResult = FMChannelHelper.MakeCall<ISyncControllerProcessor, (bool, SYNCSINGLEPASSPHASE)>(
											x =>
											{
												return x.SynchronizeScope(
											this._ClientSyncConfig,
											syncContext,
											this._SyncSessionLog,
											syncScope);
											});

											syncContext.MaxBatchSegmentRowCountEncountered = syncResult.Item1;
											syncContext.SyncSinglePassPhase = syncResult.Item2;

											if (!syncContext.MaxBatchSegmentRowCountEncountered
											&& (!syncScope.SyncSinglePass
											|| syncContext.SyncSinglePassPhase == SYNCSINGLEPASSPHASE.COMPLETE))
											{
												break;
											}
										}

										break;
								}
								break;
							}
						case SYNCSCOPETYPE.REFERENCE_ONLY:
						case SYNCSCOPETYPE.HOSTED_ONLY:
						case SYNCSCOPETYPE.BOTH:
							SiteCollectionClass siteList = this._SiteSynchronizationList.EnumerateInsertUpdateSynchronizationList(syncScope.SyncScopeTypeIndex);

							this.SynchronizeForSites(syncScope, siteList, controllerStep);
							break;
					}
				}
			}
		}

		/// <summary>
		/// Synchronizes for sites.
		/// </summary>
		/// <param name="syncScope">The synchronize scope.</param>
		/// <param name="siteList">The site list.</param>
		private void SynchronizeForSites(SyncScopeDO syncScope, SiteCollectionClass siteList, SYNCCONTROLLERSTEP controllerStep)
		{
			SyncContextFM syncContext = this._SyncContext.Clone();

			this.MaxSiteCount = siteList.Count;
			this.CurrentSiteCount = 0;

			syncContext.SiteSynchronizationList.Clear();
			syncContext.SiteSynchronizationList.AddRange(siteList);

			// For each site specified in the SiteList, Synchronize the specified SyncScope.
			foreach (SiteClass sc in siteList)
			{
				syncContext.CurrentSiteID = sc.ID;
				syncContext.CurrentSiteGuid = sc.SiteGuid;
				syncContext.CurrentControllerStep = controllerStep;
				syncContext.SyncSinglePassPhase = SYNCSINGLEPASSPHASE.SYNCROOT;
				bool isRootSite = sc.ID.Equals(this.SyncContext.SiteID, StringComparison.InvariantCultureIgnoreCase);

				SYNCSITETYPE siteSyncType = isRootSite ? SYNCSITETYPE.ROOT : SYNCSITETYPE.REFERENCE;

				if (siteSyncType != SYNCSITETYPE.ROOT)
				{
					siteSyncType = (syncScope.SyncScopeTypeIndex == SYNCSCOPETYPE.HOSTED_ONLY)
											? SYNCSITETYPE.HOSTED
											: SYNCSITETYPE.REFERENCE;
				}

				syncContext.SiteType = siteSyncType;

				this.CurrentSiteCount++;

				// Sync until there are no more changes for the scope.
				// Scope 10b (Points) requires synchronization with Enterprise Visibility; no other scope requires that
				switch (syncScope.ID)
				{
					case "Level10b":
						// Sync until there are no more changes for the scope
						while (!this.SyncStopped)
						{
							var syncResult = FMChannelHelper.MakeCall<ISyncControllerProcessor, (bool, SYNCSINGLEPASSPHASE)>(
							x =>
							{
								var hasHandle = false;
								try
								{
									bool waitResult = PointTagDataMutex.WaitOne(30000); // Wait 30 seconds
													if (!waitResult)
									{
										_EventLog.WriteEntry("SynchronizeInsertUpdates waited to long on Enterprise Visibility to finish point data", EventLogEntryType.Error);
										return (false, SYNCSINGLEPASSPHASE.COMPLETE);
									}

									hasHandle = true;
								}
								catch (AbandonedMutexException)
								{
									hasHandle = true;
								}

								try
								{
									return x.SynchronizeScope(
									this._ClientSyncConfig,
									syncContext,
									this._SyncSessionLog,
									syncScope);
								}
								finally
								{
									if (hasHandle)
									{
										PointTagDataMutex.ReleaseMutex();
									}
								}
							});

							syncContext.MaxBatchSegmentRowCountEncountered = syncResult.Item1;
							syncContext.SyncSinglePassPhase = syncResult.Item2;

							if (!syncContext.MaxBatchSegmentRowCountEncountered
							&& (!syncScope.SyncSinglePass
							|| syncContext.SyncSinglePassPhase == SYNCSINGLEPASSPHASE.COMPLETE))
							{
								break;
							}
						}
						break;

					default:
						// Sync until there are no more changes for the scope
						while (!this.SyncStopped)
						{
							var syncResult = FMChannelHelper.MakeCall<ISyncControllerProcessor, (bool, SYNCSINGLEPASSPHASE)>(
							x =>
							{
								return x.SynchronizeScope(
											this._ClientSyncConfig,
											syncContext,
											this._SyncSessionLog,
											syncScope);
							});

							syncContext.MaxBatchSegmentRowCountEncountered = syncResult.Item1;
							syncContext.SyncSinglePassPhase = syncResult.Item2;

							if (!syncContext.MaxBatchSegmentRowCountEncountered
							&& (!syncScope.SyncSinglePass
							|| syncContext.SyncSinglePassPhase == SYNCSINGLEPASSPHASE.COMPLETE))
							{
								break;
							}
						}
						break;
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
			this._SyncSessionLog.IdentityGuid = this._SyncContext.SyncSessionID;

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

			FMChannelHelper.MakeCall<ISyncSessionLogs>(x => x.Add(this._SyncContext.Security, this._SyncSessionLog));
		}

		/// <summary>
		/// Updates the current synchronization session log record with the final state of the synchronization process.
		/// </summary>
		private void EndSyncSession()
		{
			// Retrieve the syncSession so that the Conflicts may be determined.
			this._SyncSessionLog = FMChannelHelper.MakeCall<ISyncSessionLogs, SyncSessionLogDO>(x => x.Get(this._SyncContext.Security, this._SyncSessionLog.IdentityGuid));

			if (null != this._SyncSessionLog)
			{
				this._SyncSessionLog.EndDate = DateTimeOffset.Now;
				this._SyncSessionLog.SyncSessionStateIndex = SYNCSESSIONSTATE.END;
				this._SyncSessionLog.SyncSessionStatusIndex = this._SyncSessionLog.SyncSessionStatusIndex;

				if (this.UserStopFlag)
				{
					this._SyncSessionLog.SyncSessionStatusIndex = SYNCSESSIONSTATUS.USERSTOP;
				}
				else if (this.SysStopFlag)
				{
					this._SyncSessionLog.SyncSessionStatusIndex = SYNCSESSIONSTATUS.SYSSTOP;
				}
				else if (this._SyncSessionLog.Conflicts == 0)
				{
					this._SyncSessionLog.SyncSessionStatusIndex = SYNCSESSIONSTATUS.COMPOK;
				}
				else
				{
					this._SyncSessionLog.SyncSessionStatusIndex = SYNCSESSIONSTATUS.COMPCON;
				}
			}

			FMChannelHelper.MakeCall<ISyncSessionLogs>(x => x.Modify(this._SyncContext.Security, this._SyncSessionLog));
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

			FMChannelHelper.MakeCall<ISyncSessionLogs>(x => x.Modify(this._SyncContext.Security, this._SyncSessionLog));
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

			FMChannelHelper.MakeCall<ISyncSessionLogs>(x => x.Modify(this._SyncContext.Security, this._SyncSessionLog));
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

			FMChannelHelper.MakeCall<ISyncSessionScopeLogs>(x => x.Modify(this._SyncContext.Security, syncSessionScopeLogDo));

			return;
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