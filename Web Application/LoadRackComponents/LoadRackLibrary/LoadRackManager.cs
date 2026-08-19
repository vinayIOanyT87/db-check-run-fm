/******************************************************************************

	FILE NAME:		LoadRackManager.cs


	PURPOSE:			LoadRackManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		07/21/2005	W.Gray		7.0.0.1 - Changed to Get Site on Modify and not create SiteManager
										if !Site.SiteGroup

		11/14/2006	W.Gray		7.1.0.2 - Changed to ping Report Server in EventAndAlarmScan
		
		02/06/2008	I.Orndorff	7.1.0.3 - Renamed "EventAndAlarmScan" to "LoadRackProcessScan".
										  The thread now handles report server pinging,
										  alarm and event processing and processing 
										  PIDX BOLs.
										- Move code from "EventAndAlarmScan" to specific methods:
										  PingReportingService() and ProcessEventsAndAlarms().
										- Added new method ProcessPIDXBOLs().
										
		14-Apr-08	B. Schaal	7.4.0.0 - Added code to disable editing a transaction as long as the loadrackmanager
												is currently controlling or the transaction is in-progress		
									
		17-Apr-2008	C. Knight	7.4.0.1	- Added GetSignature method as gateway to signature stations to 
											support on-file signatures - CSI 5503

		16-May-2008	W.Gray		7.4.3.0 Changed ProcessEventsAndAlarms such that PriorityID of {None}
										matches PriorityCollection.Count of 0 and CatetoryID of {None}
										matches CatetoryCollection.Count of 0.  This allows for a group
										to be created that can handle Events which have no associated priority									

		2008-09-19	W.Gray		7.4.6.0 - Added ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD
										and method ResetOwnerAllocations (CSI 5558)

		10-13-2008  V. Thompson .NET 3.0 Upgrade: Changed references to SmtpMail to SmtpClient
		 
		10-16-2008  A. Coker            'To' Email addresses are added to message.
  
		08/07/2009	W.Gray		7.4.6.1 - Revised to not attempt to send mail with mail server host is not set
  
		10/19/2009	I.Orndorff	- Modified "ProcessEventsAndAlarms()" to send emails based on the email group
										  filter. This addresses task #4022.
										- Removed not loading of SiteGroups.

		2009-10-27	W.Gray		7.5.1.0 - Changed UserID test from "LoadRack.NET" to DBAccess.ServiceLogin for
										compatibility with BSME securityParam requirements
 *      2012-7-26   B. Main     8.0.5.0  -- Changed code for report server connection to accomodate Azure Reporting Services. 
*******************************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using System.Net;


using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ReportSvr2005;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;
using System.ServiceModel;
using System.Configuration;

namespace LoadRackLibrary
{
    using System.Collections;
    using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Text.RegularExpressions;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.LogClient;

	public class ResetAllocationRequest
	{
		public SecurityClass Security;
		public string ProductId;
		public DateTime? InventoryDate;
	}

	/// <summary>
	/// Summary description for LoadRackManagerClass.
	/// </summary>
	public class LoadRackManagerClass : MarshalByRefObject, FMBusinessObjects.Interfaces.ILoadRackManager, IDisposable
	{
		protected bool AlreadyDisposed;
		protected SiteManagerCollectionClass SiteManagerCollection = new SiteManagerCollectionClass();
		protected EventLog EventLog;
		internal AutoResetEvent EventOrAlarmEvent;
		protected ManualResetEvent KillEvent;
		protected Thread LoadRackProcessThread;
		protected Thread PingSessionThread = null;
		public SystemSettingClass SystemSetting;
		public Policy[] Policies;
		protected bool ReportingServiceErrorLogged;
		private readonly SecurityClass security = new SecurityClass();
		private readonly List<Thread> allocationResetThreads = new List<Thread>();
		private readonly object allocationThreadSyncObject = new object();

		public SecurityClass Security => this.security;

		public LoadRackManagerClass(EventLog eventLog)
		{
			try
			{
				this.EventLog = eventLog;

				//this credential in app.config is temperature and will be replaced once encryped methodology got created
				string userID = ConfigurationManager.AppSettings["LoadRackUserName"];
				string password = ConfigurationManager.AppSettings["LoadRackPassword"];
				string siteID = ConfigurationManager.AppSettings["LoadRackSiteID"];
				string timeout = ConfigurationManager.AppSettings["LoadRackTimeout"];
				string encryptedPassword = ConfigurationManager.AppSettings["LoadRackAutoEncryptedPassword"];

				if (this.IsPasswordEncrypted(password))
				{
					password = this.DecryptPassword(encryptedPassword, Guids.SiteAdminGuid);
				}
				else
				{
					this.SaveEncryptedPasswordToConfigFile(password);
				}

				if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(siteID) || string.IsNullOrEmpty(timeout))
				{
					eventLog.WriteEntry("Load Rack Service Credential doesn't exist", EventLogEntryType.Error);
				}
				else
				{
					SecurityLoginRequest loginRequest = new SecurityLoginRequest()
					{
						CACEnabled = true,
						UserID = userID,
						Password = password,
						SiteID = siteID,
						TimeOut = int.Parse(timeout)
					};
					SecurityLoginResponse loginResponse =
						 FMChannelHelper.MakeCall<ISites, SecurityLoginResponse>(x => x.Login2(loginRequest));

					if (loginResponse?.Result != null && (loginResponse.Result.ToUpper().StartsWith("USER")
																	  || loginResponse.Result.ToUpper().StartsWith("LOGIN FAILED")))
					{
						eventLog.WriteEntry("Load Rack Service Login Failed.", EventLogEntryType.Error);
					}
					else
					{
						this.security = loginResponse?.Security;
					}
				}
				this.SystemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(
																x =>
																x.Get(this.security)
														  );

				SiteCollectionClass siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.Enumerate(this.security)
																);

				// ReSharper disable once ForCanBeConvertedToForeach
				for (int index = 0; index < siteCollection.Count; index++)
				{
					SiteClass site = siteCollection[index];

					if (!site.Enabled)
					{
						continue;
					}

					if (site.SiteGroup)
					{
						continue;
					}

					this.security.SiteGuid = site.IdentityGuid;
					this.security.LoginSiteGuid = site.IdentityGuid;
					SiteClass fullSite = this.GetSites(this.security, site.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true);
					SiteManagerClass siteManager = new SiteManagerClass(eventLog, this, fullSite);
					this.SiteManagerCollection.Add(siteManager);
				}

				this.KillEvent = new ManualResetEvent(false);
				this.EventOrAlarmEvent = new AutoResetEvent(false);

				ThreadStart loadRackProcessScanStart = this.LoadRackProcessScan;
				this.LoadRackProcessThread = new Thread(loadRackProcessScanStart);
				this.LoadRackProcessThread.Start();

				ThreadStart pingSessionStart = new ThreadStart(this.PingSessionScan);
				this.PingSessionThread = new Thread(pingSessionStart);
				this.PingSessionThread.Start();
				this.PingSessionThread.Priority = ThreadPriority.Normal;
			}

			catch (Exception e)
			{
				eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		public bool IsPasswordEncrypted(string plaintextPassword)
		{
			if (plaintextPassword == "******")
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		public string DecryptPassword(string encryptedPassword, Guid siteGuid)
		{
			byte[] bytePassword = Convert.FromBase64String(encryptedPassword);
			string decryptedPassword = "";
			try
			{
				decryptedPassword = UserClass.decode(bytePassword, siteGuid);
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry("Load Rack Service username or password error. " + e.Message, EventLogEntryType.Error);
			}

			return decryptedPassword;
		}

		public void SaveEncryptedPasswordToConfigFile(string plaintextPassword)
		{
			byte[] encryptedPassword = UserClass.encode(plaintextPassword, Guids.SiteAdminGuid);
			string base64PasswordString = Convert.ToBase64String(encryptedPassword);
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			configuration.AppSettings.Settings["LoadRackPassword"].Value = "******";
			configuration.AppSettings.Settings["LoadRackAutoEncryptedPassword"].Value = base64PasswordString;
			configuration.Save();
		}

		private SiteClass GetSites(SecurityClass securityParam, Guid guid, bool getMemberSites = true, bool getSchedulesAndProcessVariables = true,
										 bool bGetAssociatedAliases = true)
		{
			return FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(securityParam, guid, getMemberSites, getSchedulesAndProcessVariables, bGetAssociatedAliases)
			);
		}

		~LoadRackManagerClass()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				try
				{
					// expect that there should be no threads still hanging around, but just in case.
					foreach (Thread resetThread in this.allocationResetThreads)
					{
						resetThread.Join();
					}

					foreach (SiteManagerClass siteManager in this.SiteManagerCollection)
					{
						siteManager.Dispose();
					}

					this.KillEvent?.Set();
					this.LoadRackProcessThread?.Join();
					this.PingSessionThread?.Join();
				}
				catch (Exception e)
				{
					this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
				}

				GC.SuppressFinalize(this);
				this.AlreadyDisposed = true;
			}
		}

		public void PingSessionScan()
		{
			WaitHandle[] events = { this.KillEvent };

			int waitResult;
			while (0 != (waitResult = WaitHandle.WaitAny(events, 30000, true)))
			{
				try
				{
					switch (waitResult)
					{

						case WaitHandle.WaitTimeout:
							{
								try
								{
									FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(this.Security));
								}
								catch (Exception)
								{
									// Likely source of this exception is database timeout during reindexing.
									// Just catch it and try again next pass
									this.EventLog.WriteEntry("LoadRackManager.PingSessionScan():  encountered exception pinging session.  Catching and retrying on next pass",
																EventLogEntryType.Warning);
								}
								break;
							}
					}
				}
				catch (Exception e)
				{
					throw new Exception(e.Message);
				}
			}
		}

		public void Add(SecurityClass securityParam, Type type, Guid identityGuid)
		{
			Monitor.Enter(this);
			try
			{
				if (securityParam == null)
					throw new ArgumentNullException(nameof(securityParam));

				if (type == null)
					throw new ArgumentNullException(nameof(type));

				// Site, Create a Site Manager
				if (type == typeof(SiteClass))
				{
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
								x =>
								x.Get(securityParam, identityGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
										bGetAssociatedAliases: true)
						);
					if (site.Enabled
					&& !site.SiteGroup)
					{
						SiteManagerClass siteManager = new SiteManagerClass(this.EventLog, this, site);
						this.SiteManagerCollection.Add(siteManager);
					}
				}

				// All other objects are handled by SiteManager
				else
				{
					SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(securityParam.SiteGuid);
					if (siteManager == null)
						return;

					Monitor.Enter(siteManager);
					try
					{
						siteManager.Add(securityParam, type, identityGuid);
					}

					// Log any errors, has already been committed to the database
					catch (Exception e)
					{
						this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					}
					finally
					{
						Monitor.Exit(siteManager);
					}
				}
			}

			// Log any errors, has already been committed to the database
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}

			finally
			{
				Monitor.Exit(this);
			}
		}


		public void Modify(SecurityClass securityParam, Type type, Guid identityGuid)
		{
			Monitor.Enter(this);
			try
			{
				if (securityParam == null)
					throw new ArgumentNullException(nameof(securityParam));

				if (type == null)
					throw new ArgumentNullException(nameof(type));

				if (type == typeof(SystemSettingClass))
				{
					this.SystemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(
																	x =>
																	x.Get(securityParam)
															  );
				}

				else if (type == typeof(SiteClass))
				{
					if (!securityParam.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !securityParam.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
						throw new FMInsufficientRightsException();

					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
													x =>
													x.Get(securityParam, identityGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
														bGetAssociatedAliases: true)
											);

					if (!site.Enabled
					|| site.SiteGroup) this.Purge(securityParam, type, identityGuid);
					else
					{
						SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(identityGuid);
						if (siteManager == null)
						{
							this.Add(securityParam, type, identityGuid);
							return;
						}

						siteManager.ModifySite(site);
					}
				}

				else if (type == typeof(TransactionAliasClass))
				{
					foreach (SiteManagerClass siteManager in this.SiteManagerCollection)
						siteManager.Modify(securityParam, type, identityGuid);
				}
				else
				{
					SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(securityParam.SiteGuid);
					if (siteManager == null)
						return;

					Monitor.Enter(siteManager);
					try
					{
						siteManager.Modify(securityParam, type, identityGuid);
					}

					// Log any errors, has already been committed to the database
					catch (Exception e)
					{
						this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					}
					finally
					{
						Monitor.Exit(siteManager);
					}
				}
			}

			// Log any errors, has already been committed to the database
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}

			finally
			{
				Monitor.Exit(this);
			}
		}

		public void PurgeEntityToSiteMap(SecurityClass securityParam, EntityToSiteMapClass entityToSiteMap)
		{
			Monitor.Enter(this);
			try
			{
				// Remove any assignment to Site
				TransactionAliasClass transactionAlias = new TransactionAliasClass();
				if (entityToSiteMap.TypeID == transactionAlias.EntityType)
				{
					foreach (SiteManagerClass siteManager in this.SiteManagerCollection)
						siteManager.RemoveSiteTransactionAliasAssignment(entityToSiteMap.IdentityGuid);
				}
				else
				{
					CompanyClass company = new CompanyClass();
					if (entityToSiteMap.TypeID == company.EntityType)
					{
						foreach (SiteManagerClass siteManager in this.SiteManagerCollection)
							siteManager.RemoveTankManagerAssignment(entityToSiteMap.IdentityGuid);
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}


		public void CheckOkToPurge(Type type, Guid identityGuid)
		{
			if (type == typeof(SiteClass))
			{
				SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(identityGuid);

				if (siteManager != null)
				{
					if (siteManager.LoadingInProgress())
					{
						throw new Exception("LoadRack|Cannot purge site while loading in progress");

					}
				}
			}
		}

		public StationClass GetStation(SecurityClass securityParam, Guid stationGuid)
		{
			SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(securityParam.SiteGuid);

			return siteManager?.GetStation(stationGuid);
		}

		public TransactionDO GetStationTransaction(SecurityClass securityParam, Guid stationGuid)
		{
			SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(securityParam.SiteGuid);

			return siteManager?.GetStationTransaction(stationGuid);
		}

		public TransactionDO AccountingRequest(TransactionSR sr)
		{
			Monitor.Enter(this);
			try
			{
				SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(sr.Security.SiteGuid);

				if (siteManager == null ||
				!siteManager.LoadingInProgress())
				{
					throw new Exception("Not Loading");
				}

				try
				{
					Monitor.Enter(siteManager);
					foreach (StationManagerClass stationManager in siteManager.StationManagerCollection)
					{
						TransactionDO trans = stationManager.Transaction;
						if (trans != null
						&& trans.TransID == sr.TransID)
						{
							return trans;
						}
					}
				}
				finally
				{
					Monitor.Exit(siteManager);
				}
				throw new Exception("Not Loading");
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public SaveTransactionsResultDO AccountingRequest(SaveTransactionsSR sr)
		{
			Monitor.Enter(this);
			try
			{
				SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(sr.Security.SiteGuid);
				if (siteManager == null)
				{
					SaveTransactionsResultDO result = new SaveTransactionsResultDO();

					TransactionValidationResult transResult = new TransactionValidationResult();
					var loadRackStr = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(sr.Security.SiteGuid, "Load Rack|Site Not Found")
																);
					transResult.ErrorList.Add(loadRackStr);
					result.Results.Add(transResult);
					SaveTransactionsException saveException = new SaveTransactionsException(result.Results);
					throw new FaultException<SaveTransactionsException>(saveException, SaveTransactionsException.FaultExceptionReason);
				}

				try
				{
					Monitor.Enter(siteManager);
					return siteManager.SaveTransaction(sr);
				}
				finally
				{
					Monitor.Exit(siteManager);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public void Purge(SecurityClass securityParam, Type type, Guid identityGuid)
		{
			Monitor.Enter(this);
			try
			{
				if (securityParam == null)
					throw new ArgumentNullException(nameof(securityParam));

				if (type == null)
					throw new ArgumentNullException(nameof(type));

				if (typeof(SiteClass) == type)
				{
					this.CheckOkToPurge(type, identityGuid);

					SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(identityGuid);
					if (siteManager != null)
					{
						siteManager.Dispose();
						this.SiteManagerCollection.Remove(siteManager);
					}
				}

				else if (typeof(TransactionAliasClass) == type)
				{
					foreach (SiteManagerClass siteManager in this.SiteManagerCollection)
						siteManager.RemoveSiteTransactionAliasAssignment(identityGuid);
				}

				else if (typeof(CompanyClass) == type)
				{
					foreach (SiteManagerClass siteManager in this.SiteManagerCollection)
						siteManager.RemoveTankManagerAssignment(identityGuid);
				}

				// All other objects are handled by Site Manager
				else
				{
					SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(securityParam.SiteGuid);
					if (siteManager == null)
						return;

					Monitor.Enter(siteManager);
					try
					{
						siteManager.Purge(securityParam, type, identityGuid);
					}
					finally
					{
						Monitor.Exit(siteManager);
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public bool GetStationCommunicationsStatus(Guid siteGuid, Guid stationGuid)
		{
			StationManagerClass stationManager = null;

			// Find the site manager for the indicated site Guid
			SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(siteGuid);

			if (siteManager != null)
			{
				stationManager = siteManager.StationManagerCollection.FindByStationIdentityGuid(stationGuid);
				return stationManager.CommunicationsFailure;
			}

			return true;
		}

		public StationManagerClass FindStation(Guid siteGuid, Guid stationGuid)
		{
			StationManagerClass stationManager = null;

			// Find the site manager for the indicated site Guid
			SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(siteGuid);

			if (siteManager != null)
			{
				stationManager = siteManager.StationManagerCollection.FindByStationIdentityGuid(stationGuid);
			}

			return stationManager;
		}

		public void InitiateEndOfDay(SecurityClass securityParam)
		{
			Monitor.Enter(this);
			try
			{
				if (securityParam == null)
					throw new ArgumentNullException(nameof(securityParam));

				if (!securityParam.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
					throw new FMInsufficientRightsException();

				SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(securityParam.SiteGuid);
				if (siteManager == null)
					throw new Exception("LoadRack|Site Not Found");

				siteManager.InitiateEndOfDay(securityParam);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public Hashtable GetEndOfDayStatus(SecurityClass security) {
			SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(security.SiteGuid);
			if (siteManager == null)
				throw new Exception("LoadRack|Site Not Found");
			var result = new Hashtable();
			result.Add("endOfDayMessage", siteManager.endOfDayMessage);
			result.Add("endOfDayProcessingPercentage", siteManager.endOfDayProcessingPercentage);
			result.Add("lastSuccessfulEndOfDayTime", siteManager.lastSuccessfulEndOfDayTime);
			result.Add("endOfDayError", siteManager.endOfDayError);
			return result;

		}

		public void ResetOwnerAllocations(SecurityClass securityParam)
		{
			if (securityParam == null)
			{
				throw new ArgumentNullException(nameof(securityParam));
			}

			if (!securityParam.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			{
				throw new FMInsufficientRightsException();
			}

			var resetRequest = new ResetAllocationRequest()
			{
				Security = securityParam,
				ProductId = string.Empty,
				InventoryDate = null
			};

			ParameterizedThreadStart allocationResetThreadStart = this.DeferredResetAllocations;
			Thread allocationResetThread = new Thread(allocationResetThreadStart);
			allocationResetThread.Start(resetRequest);
			this.allocationResetThreads.Add(allocationResetThread);
		}

		public void ResetOwnerAllocationsInventoryDate(SecurityClass securityParam, DateTime inventoryDate)
		{
			if (securityParam == null)
			{
				throw new ArgumentNullException(nameof(securityParam));
			}

			if (!securityParam.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			{
				throw new FMInsufficientRightsException();
			}

			var resetRequest = new ResetAllocationRequest()
			{
				Security = securityParam,
				ProductId = string.Empty,
				InventoryDate = inventoryDate
			};

			ParameterizedThreadStart allocationResetThreadStart = this.DeferredResetAllocations;
			Thread allocationResetThread = new Thread(allocationResetThreadStart);
			allocationResetThread.Start(resetRequest);
			this.allocationResetThreads.Add(allocationResetThread);
		}

		public void ResetOwnerAllocationsForSingleProduct(SecurityClass securityParam, string productId)
		{
			if (securityParam == null)
			{
				throw new ArgumentNullException(nameof(securityParam));
			}

			if (!securityParam.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			{
				throw new FMInsufficientRightsException();
			}

			var resetRequest = new ResetAllocationRequest()
			{
				Security = securityParam,
				ProductId = productId,
				InventoryDate = null
			};

			ParameterizedThreadStart allocationResetThreadStart = this.DeferredResetAllocations;
			Thread allocationResetThread = new Thread(allocationResetThreadStart);
			allocationResetThread.Start(resetRequest);
			this.allocationResetThreads.Add(allocationResetThread);
		}

		void DeferredResetAllocations(object request)
		{
			var allocationRequest = request as ResetAllocationRequest;
			if (allocationRequest == null)
			{
				return;
			}

			Monitor.Enter(this.allocationThreadSyncObject);
			try
			{
				SiteManagerClass siteManager = this.SiteManagerCollection.FindBySiteGuid(allocationRequest.Security.SiteGuid);
				if (siteManager == null)
				{
					return;
				}

				if (string.IsNullOrEmpty(allocationRequest.ProductId))
				{
					if (allocationRequest.InventoryDate.HasValue)
					{
						siteManager.ResetOwnerAllocations(allocationRequest.InventoryDate.Value);
					}
					else
					{
						siteManager.ResetOwnerAllocations(this.GetCurrentInventoryDate(allocationRequest.Security));
					}
				}
				else
				{
					siteManager.ResetOwnerAllocationsForSingleProduct(allocationRequest.Security, this.GetCurrentInventoryDate(allocationRequest.Security), allocationRequest.ProductId);
				}
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry("DeferredResetAllocations Error. " + e.Message, EventLogEntryType.Error);
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, AllocationClass.AllocationResetFailure));
			}
			finally
			{
				Monitor.Exit(this.allocationThreadSyncObject);
			}
		}

		public void PingReportingService()
		{
			// Access the ReportingService to activate it so that it will be
			// ready when print requests are issued.
			try
			{
                

                //**Azure or Defense
                ////**** Use ReportServerCredentials when running in azure. Use dbAccessClient when not Azure *******
			    ReportingService2005 reportingService = new ReportingService2005
			                                            {
			                                                Url = this.SystemSetting.ReportServerUrl + "/ReportService2005.asmx"
			                                            };
                reportingService.Credentials = CredentialCache.DefaultCredentials;
                this.Policies = reportingService.GetSystemPolicies();
                this.ReportingServiceErrorLogged = false;
			}
			catch (Exception e)
			{
				if (!this.ReportingServiceErrorLogged)
				{
					this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					this.ReportingServiceErrorLogged = true;
				}
			}
		}

		public void ProcessPidxBols()
		{
			Monitor.Enter(this);
			try
			{
				TransactionBolPidxSR sr = new TransactionBolPidxSR();

				foreach (SiteManagerClass siteManager in this.SiteManagerCollection)
				{
					sr.Security = siteManager.Security;
					this.ProcessIPIDXBOLProcessor(sr);
				}
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		private void ProcessIPIDXBOLProcessor(TransactionBolPidxSR sr)
		{
			FMChannelHelper.MakeCall<IPidxBolProcessor>(x => x.Process(sr));
		}

		private void CheckForDriverTimeout()
		{
			var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
			Monitor.Enter(this);

			try
			{
				// this routine calls each station manager and checks if a driver is logged in
				// if they are logged in we check for a time out and shutdown the operation
				// get a list of persons that have the carded in flag set to true that exceed our time computation
				foreach (SiteManagerClass siteManager in this.SiteManagerCollection)
				{
					if (siteManager.Site.SiteGroup)
					{
						continue;
					}

					if (string.IsNullOrEmpty(siteManager.Site.CardInTimeout)
						 || string.Equals("0", siteManager.Site.CardInTimeout, StringComparison.InvariantCulture))
					{
						// empty or zero timeout means don't time out drivers (WI 37353)
						continue;
					}

					try
					{
						DataSet dataSet = FMChannelHelper.MakeCall<IPersonnel, DataSet>(x => x.EnumerateCardedInPersonnelPartTimeoutPeriod(siteManager.Security, DateTimeOffset.Now - new TimeSpan(0, Convert.ToInt32(siteManager.Site.CardInTimeout), 0)));

						var personCollection = new PersonCollectionClass();
						DataTable table = dataSet.Tables[0];
						while (table.Rows.Count != 0)
						{
							var person = new PersonClass();
							person.Load(dataSet);
							personCollection.Add(person);
							table.Rows.RemoveAt(0);
						}

						if (personCollection.Count > 0)
						{
							// check each station
							siteManager.StationManagerCollection.Lock();
							try
							{
								// check if this person is currently using a load computer and if they are do nothing
								// if they are not then set them to carded out
								foreach (PersonClass localPerson in personCollection)
								{
									bool resetPersonsStatus = true;
									foreach (StationManagerClass stationManager in siteManager.StationManagerCollection)
									{
										// get the logged in driver
										if (stationManager.Driver != null)
										{
											if (localPerson.ID == stationManager.Driver.ID)
											{
												resetPersonsStatus = false;
												break;
											}
										}
									}

									if (resetPersonsStatus)
									{
										localPerson.CardedIn = false;
										FMChannelHelper.MakeCall<IPersonnel>(x => x.Modify(siteManager.Security, DATA_TYPE.DYNAMIC, localPerson));
										FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(siteManager.Security, localPerson.CardedInExceededAllowedCardInPeriodEvent()));
										this.EventOrAlarmEvent.Set();

										var getTransactionSR = new GetTransactionSR
										{
											Security = siteManager.Security,
											Request = GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID,
											BeginningDate = new DateTime(1900, 1, 1),
											EndingDate = new DateTime(9999, 12, 31),
											Site = siteManager.Site.ID,
											TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
											OperatorPersonnelGuid = localPerson.MasterRecordGuid,
											Status = ((int)TransactionStatus.InProgress).ToString(CultureInfo.InvariantCulture)
										};

										var getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(x => x.Process(getTransactionSR));

										if (getTransactionDO?.TransactionDataSet != null && getTransactionDO.TransactionDataSet.Tables.Count != 0 && getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
										{
											foreach (DataRow row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
											{
												if (siteManager.TransactionLoading((string)row["TransID"]))
												{
													// Transactions currently active at another station can't be completed,
													// as they will be overwritten and cause major problems.
													// It's better to just let the terminal manager force it completed when he's ready.
													continue;
												}

												var transactionSR = new TransactionSR { Security = siteManager.Security, TransID = (string)row["TransID"] };

												var transaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(transactionSR));
												transaction.Status = TransactionStatus.Completed;

												logger.Debug("LoadRackManagerClass.CheckForDriverTimeout() - saving transaction " + transaction.TransID + ":" + transaction.DocumentNumber + " as completed");
												var saveTransactionsSR = new SaveTransactionsSR
												{
													Security = siteManager.Security,
													ConvertUnits = true,
													CurrentSiteGuid = siteManager.Security.SiteGuid,
													BOLFromLoadRackFlag = true
												};
												saveTransactionsSR.Transactions.Add(transaction);
												FMChannelHelper.MakeCall<ISaveTransactionsProcessor>(x => x.SaveTransactions(saveTransactionsSR));

												if (!string.IsNullOrEmpty(transaction.TransRefID))
												{
													transactionSR.TransID = transaction.TransRefID;
													var order = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(transactionSR));
													if (order != null && order.Status == TransactionStatus.Scheduled
														&& StationManagerClass.IsTransactionScheduledOrder(order))
													{
														order.Status = TransactionStatus.Completed;
														var saveOrderSR = new SaveTransactionsSR
														{
															Security = siteManager.Security,
															ConvertUnits = true,
															CurrentSiteGuid = siteManager.Security.SiteGuid,
															BOLFromLoadRackFlag = true
														};
														saveOrderSR.Transactions.Add(order);
														FMChannelHelper.MakeCall<ISaveTransactionsProcessor>(x => x.SaveTransactions(saveOrderSR));
													}
												}
											}
										}
									}
								}
							}
							finally
							{
								siteManager.StationManagerCollection.UnLock();
							}
						}

					}
					catch (Exception)
					{
						// Likely source of this exception is database timeout during reindexing.
						// Just catch it and try again next pass
						this.EventLog.WriteEntry("LoadRackManager.CheckForDriverTimeout():  encountered exception checking drivers.  Catching and retrying on next pass",
													EventLogEntryType.Warning);
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		void LoadRackProcessScan()
		{
			WaitHandle[] events = { this.KillEvent };

			while (0 != (WaitHandle.WaitAny(events, 60000, true)))
			{
				this.PingReportingService();
				this.ProcessPidxBols();
				this.CheckForDriverTimeout();
				this.CleanUpAllocationResetThreads();
			}
		}

		private void CleanUpAllocationResetThreads()
		{
			this.allocationResetThreads?.RemoveAll(x => x.IsAlive == false);
		}

		/// <summary>
		/// Calls down to the specified signature station to immediately grab 
		/// a signature from the signature pad
		/// </summary>
		/// <param name="securityParam">Current securityParam object</param>
		/// <param name="stationGuid">Signature station.  This method will throw for any other
		/// station type</param>
		/// <returns>bitmap of signature as a byte array</returns>
		/// <exception cref="NotImplementedException">Specified station is any station type other than a signature station</exception>
		/// <exception cref="Exception">Specified station not found</exception>
		/// <exception cref="ArgumentNullException">null securityParam object passed in</exception>
		public byte[] GetSignature(SecurityClass securityParam, Guid stationGuid)
		{
			if (securityParam == null)
				throw new ArgumentNullException(nameof(securityParam));

			var stationManager = this.FindStation(securityParam.SiteGuid, stationGuid);
			if (stationManager == null)
				throw new Exception("LoadRack|Signature Station Not Found");

			return stationManager.GetSignature();
		}

		/// <summary>
		/// Update the current value for an internal Additive Totalizer 
		/// <para />
		/// <param name="securityParam">Current securityParam object</param>
		/// <param name="stationGuid">Guid from Station object</param>
		/// <param name="loadArmGuid">Guid from LoadArm object</param>
		/// <param name="productGuid">Guid from Product object and AssignedGuid from ProductMap object</param>
		/// </summary>
		public void SetAdditiveMeterTotalizer(SecurityClass securityParam, Guid stationGuid, Guid loadArmGuid, Guid productGuid, double value)
		{
			if (securityParam == null)
				throw new ArgumentNullException(nameof(securityParam));

			if (!securityParam.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			var stationManager = this.FindStation(securityParam.SiteGuid, stationGuid);
			if (stationManager == null)
				throw new Exception("LoadRack|Station Not Found");

			stationManager.SetAdditiveMeterTotalizer(loadArmGuid, productGuid, value);
		}


		/// <summary>
		/// Helper function to build an Inventory Date request and then pass it to the local
		/// AccountingRequest function, from whence it will be passed to the AccountingBLL
		/// </summary>
		/// <param name="securityParam">securityParam object for user/site makeing the call</param>
		/// <returns>Inventory Date for the site specified in the securityParam object.</returns>
		public DateTime GetCurrentInventoryDate(SecurityClass securityParam)
		{
			InventoryDateSR inventoryDateSR = new InventoryDateSR { Security = securityParam, CurrentSiteGuid = securityParam.SiteGuid };


			var inventoryDateDO = FMChannelHelper.MakeCall<IInventoryDateProcessor, InventoryDateDO>(
				x =>
				x.Process(inventoryDateSR)
				);

			return inventoryDateDO.InventoryDate;
		}

		/// <summary>
		/// Helper function to download the information necessary to run the device in a local configuration
		/// </summary>
		/// <param name="securityParam">securityParam object for user</param>
		/// <param name="stationGuid"> Guid of the station to set the data in</param>
		/// <returns>void</returns>
		public void DownloadLocalConfigurationToStation(SecurityClass securityParam, Guid stationGuid)
		{
			if (securityParam == null)
				throw new ArgumentNullException(nameof(securityParam));

			var stationManager = this.FindStation(securityParam.SiteGuid, stationGuid);
			if (stationManager == null)
				throw new Exception("Station Not Found");

			// set the event to download the configuration
			stationManager.DownloadConfigurationEvent.Set();
		}
	}
}
