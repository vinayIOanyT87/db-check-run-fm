// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyController.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ServiceProcessInterfaces;

namespace FuelsManager.Afss.Module.Gasboy.OrCU
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.Text;
	using System.Xml;
	using System.Xml.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Afss.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.OrCU.Communications;
	using FuelsManager.Afss.Module.Gasboy.OrCU.GasboyBOS;

	/// <summary>
	///	The Gasboy controller.
	/// </summary>
	public class GasboyController
	{
		#region Enumerations

		private enum ActiveControllerState
		{
			NotFound = 0,
			CurrentOwner = 1,
			AnotherOwner = 2
		}

		#endregion Enumerations

		#region Constants and Fields

		/// <summary>
		///	Created and owned by an active GasboyController instance
		///	Used to determine if cleanup is required for open gasboy session log entries.
		/// </summary>
		public const string ActiveGasboyControllerMutexName = "ActiveGasboyControllerMutex";

		/// <summary>
		///	The event log.
		/// </summary>
		private static readonly EventLog _EventLog = new EventLog("Application", ".", "GasboyController");
		
		private static Object _ActiveGasboyControllerLock = new Object();

		private GasboySessionController sessionController = new GasboySessionController();

		private GasboyIslanderPlus gasboyIslanderPlus = new GasboyIslanderPlus();

		/// <summary>
		/// Used to transform raw external station transaction data into an object
		/// </summary>
		private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(GasboyStationTransaction));

		/// <summary>
		/// Contains the security context before the station connection test started
		/// </summary>
		private static SecurityClass _GasboyDownloadTransactionsSecurity = null;

		#endregion Constants and Fields

		#region Constructors and Destructors

		/// <summary>
		///	Initializes a new instance of the <see cref="GasboyController" /> class.
		/// </summary>
		public GasboyController()
		{
		}

		#endregion Constructors and Destructors

		#region Public Properties

		#endregion Public Properties

		#region Properties

		#endregion Properties

		#region Public Methods and Operators

		/// <summary>
		/// Requests a list of products configured in a Gasboy Station
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <param name="passThruClientCertificate">
		/// The pass through client certificate.
		/// </param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public List<GasboyStationProduct> GetProductList(
				SecurityClass security,
				GasboyStation externalStation,
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate)
		{
			var gasboyProductList = new List<GasboyStationProduct>();

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
				;
			}

			try
			{
				var externalProducts = this.gasboyIslanderPlus.DownloadProductListFromGasboyStation(
					security,
					externalStation,
					sessionType,
					passThruClientCertificate);

				if (externalProducts.Count > 0)
				{
					foreach (var product in externalProducts)
					{
						var gasboyProduct = this.CreateGasboyProduct(
							security,
							product,
							externalStation);

						// Should we attempt to resolve any mappings?

						gasboyProductList.Add(gasboyProduct);
					}
				}
			}
			catch (EndpointNotFoundException eX)
			{
				string msg =
				    string.Format(
				            "Unable to connect to Gasboy Islander.  Check config settings. Exception: {0}",
				            eX.Message);

				_EventLog.WriteEntry(msg, EventLogEntryType.Error);
				// SyncHelperFM.WriteErrorAlarmAndEvent(security, msg);
			}
			finally
			{
				//if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				//{
				//    FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity));
				//}
			}

			return gasboyProductList;
		}

		/// <summary>
		/// Get new events from the specified station
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStation">The station to get events for</param>
		/// <param name="sessionType">Type of session being initiated</param>
		/// <returns>True if the download was successful</returns>
		public bool GetEvents(
			SecurityClass security,
			GasboyStation externalStation,
			ExternalStationSessionType sessionType,
			byte[] passThruClientCertificate)
		{
			bool transferFinished = false;

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
				;
			}

			try
			{

				GasboyEvents gasboyEvents = new GasboyEvents();
				GasboyController._GasboyDownloadTransactionsSecurity = security;
				var downloadedEvents = this.GetGasboyStationLogsFromGasboyStation(
					security,
					externalStation,
					sessionType,
					passThruClientCertificate);

				if (downloadedEvents.Count > 0)
				{
					GasboyChannelHelper.MakeCall<IGasboyStations>(
						gasboyStationsChannel => gasboyStationsChannel.AddExternalStationLogs(security, downloadedEvents));
						soEventID[] EventIDs = new soEventID[downloadedEvents.Count];
					int count = 0;
					foreach (var GasboyEvent in downloadedEvents)
					{
						EventIDs[count] = new soEventID();
							GasboyStationEvent gasboyStationEvent = GasboyEvent as GasboyStationEvent;
							EventIDs[count].id = Convert.ToInt32(gasboyStationEvent.EventID);
						count++;

					}
					this.gasboyIslanderPlus.MarkEventsReceivedOnGasboyStation(security, externalStation, EventIDs);
				}

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
								GasboyController._GasboyDownloadTransactionsSecurity,
								gasboyEvents.GasboyEventCollectionCompleteEvent(Convert.ToString(downloadedEvents.Count), externalStation.ID));
					});


				transferFinished = true;
			}
			catch (EndpointNotFoundException eX)
			{
				//string msg =
				//    string.Format(
				//            "Unable to connect to enterprise synchronization service.  Check config settings syncEnterpriseBusinessBindingType and syncEnterpriseBusinessBindingConfiguration.  These should match the enterprise server service endpoint settings.  Exception: {0}",
				//            eX.Message);

				//_EventLog.WriteEntry(msg, EventLogEntryType.Error);
				//SyncHelperFM.WriteErrorAlarmAndEvent(security, msg);
			}
			finally
			{
				//if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				//{
				//    FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity));
				//}
			}

			return transferFinished;
		}

		/// <summary>
		/// The execute data transfer request with remote Gasboy Station
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <param name="sessionType">
		/// Type of session being initiated
		/// </param>
		/// <param name="passThruClientCertificate">
		/// The pass through client certificate.
		/// </param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public bool GetTransactions(
				SecurityClass security,
				GasboyStation externalStation,
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate)
		{
			bool transferFinished = false;
			GasboyEvents gasboyEvents = new GasboyEvents();
			GasboyController._GasboyDownloadTransactionsSecurity = security;
			externalStation.LastConnectionAttempt = DateTimeOffset.Now;
			GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(security, externalStation));

			try
			{
				var externalTransactions = this.gasboyIslanderPlus.DownloadTransactionsFromGasboyStation(
					security,
					externalStation,
					sessionType,
					passThruClientCertificate);

				if (null != externalTransactions)
				{
					if (externalTransactions.Count > 0)
					{
						externalStation.LastTransactionID = externalTransactions[externalTransactions.Count - 1].id;
						GasboyChannelHelper.MakeCall<IGasboyStations>(
							externalStationsService => externalStationsService.Modify(security, externalStation));
						transferFinished = this.ProcessTransactionsFromGasboyStation(
							security,
							externalStation,
							externalTransactions,
							sessionType,
							passThruClientCertificate);
					}
					else
					{
						transferFinished = true;
						//  need to log that no transactions were received from the Gasboy Unit.
					}

					externalStation.LastSuccessfulConnection = DateTimeOffset.Now;
					GasboyChannelHelper.MakeCall<IGasboyStations>(
						externalStationsService => externalStationsService.Modify(security, externalStation));

					if (sessionType == ExternalStationSessionType.Manual)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
							alarmAndEventChannel =>
								{
									alarmAndEventChannel.Add(
										GasboyController._GasboyDownloadTransactionsSecurity,
										gasboyEvents.GasboyManualTransactionDownloadCompleteEvent(
											externalStation.ID,
											externalTransactions.Count.ToString()));
								});
					}
					if (sessionType == ExternalStationSessionType.Periodic)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
							alarmAndEventChannel =>
								{
									alarmAndEventChannel.Add(
										GasboyController._GasboyDownloadTransactionsSecurity,
										gasboyEvents.GasboyPeriodicTransactionDownloadCompleteEvent(
											externalStation.ID,
											externalTransactions.Count.ToString()));
								});
					}

				}
			}
			catch (EndpointNotFoundException eX)
			{
				//string msg =
				//    string.Format(
				//            "Unable to connect to enterprise synchronization service.  Check config settings syncEnterpriseBusinessBindingType and syncEnterpriseBusinessBindingConfiguration.  These should match the enterprise server service endpoint settings.  Exception: {0}",
				//            eX.Message);

				//_EventLog.WriteEntry(msg, EventLogEntryType.Error);
				//SyncHelperFM.WriteErrorAlarmAndEvent(security, msg);

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(
								GasboyController._GasboyDownloadTransactionsSecurity,
								gasboyEvents.GasboyManualTransactionDownloadErrorEvent(
									externalStation.ID,
									" The endpoint was not found. Please verify the external station configuration is correct."));
						});
			}
			catch (Exception eX)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
		alarmAndEventChannel =>
		{
			alarmAndEventChannel.Add(
							GasboyController._GasboyDownloadTransactionsSecurity,
							gasboyEvents.GasboyManualTransactionDownloadErrorEvent(
								externalStation.ID,
								eX.ToString()));
		});
				throw;
			}
			finally
			{
				//if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				//{
				//    FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity));
				//}
			}

			return transferFinished;
		}

		/// <summary>
		/// The execute data transfer request with remote Gasboy Station
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <param name="endTransactionID"></param>
		/// <param name="passThruClientCertificate">
		/// The pass through client certificate.
		/// </param>
		/// <param name="beginTransactionID"></param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public bool GetSelectedTransactionRange(
				SecurityClass security,
				GasboyStation externalStation,
				int beginTransactionID,
				int endTransactionID,
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate)
		{
			bool transferFinished = false;

			try
			{
				var externalTransactions =
					this.gasboyIslanderPlus.DownloadSelectedTransactionsFromGasboyStation(
						security,
						externalStation,
						beginTransactionID,
						endTransactionID,
						sessionType,
						passThruClientCertificate);

				transferFinished = this.ProcessTransactionsFromGasboyStation(security, externalStation, externalTransactions, sessionType, passThruClientCertificate);

			}
			catch (EndpointNotFoundException eX)
			{
				string msg =
					string.Format(
						"Unable to download specified range of transactions from the Gasboy Islander Station ({0}).  Exception: {1}\nStack Trace: {2}",
						externalStation.SiteCode,
						eX.Message,
						eX.StackTrace);

				_EventLog.WriteEntry(msg, EventLogEntryType.Error);
			}

			return transferFinished;
		}

		/// <summary>
		/// The execute data transfer request with remote Gasboy Station
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <param name="gasboyStationTransaction"></param>
		/// <param name="passThruClientCertificate">
		/// The pass through client certificate.
		/// </param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public bool ReprocessTransaction(
				SecurityClass security,
				GasboyStation externalStation,
				GasboyStationTransaction gasboyStationTransaction,
				byte[] passThruClientCertificate)
		{
			try
			{
				var gasboyTransactions = new List<GasboyStationTransaction>() { gasboyStationTransaction };

				var result = GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, string>(x => x.ImportTransactions(security, externalStation, gasboyTransactions));

				if (result.ToUpper().Equals("SUCCESS!"))
				{
					GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, string>(x => x.ImportTransactions(security, externalStation, gasboyTransactions));
					return true;
				}
			}
			catch (Exception)
			{
				throw;
			}

			return false;
		}

		/// <summary>
		/// This method is responsible for retrieving a list of devices that should be downloaded to the a remote Gasboy station.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="passThruClientCertificate">
		/// A byte[] that contains the client certificate of the user who originally initiated the request.
		/// </param>
		/// <param name="sessionType">
		/// Type of session being initiated
		/// </param>
		/// <returns>
		/// true if the entire data transfer process completed, otherwise; false which indicates the process was interrupted
		/// prior to completion.
		/// </returns>
		/// <remarks>
		/// </remarks>
		public List<GasboyDevice> GetGasboyDeviceList(
				SecurityClass security,
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate)
		{
			List<GasboyDevice> gasboyDevices = null;

			try
			{
				gasboyDevices = GasboyChannelHelper.MakeCall<IGasboyDevices, List<GasboyDevice>>(x => x.EnumerateWithDeleted(security));
			}
			catch (EndpointNotFoundException eX)
			{
                //Changed from Gasboy Devices to Payment Cards since we are only using the Gasboy devices for payment cards at this point.
				string msg =
					string.Format(
							"Unable to retrieve a list of Payment Cards from FuelsManager.  Exception: {0}\nStack Trace: {1}",
							eX.Message, eX.StackTrace);

				_EventLog.WriteEntry(msg, EventLogEntryType.Error);
			}

			return gasboyDevices;
		}

		/// <summary>
		/// Updates devices (vehicles, drivers, cards, etc.) on a remote Gasboy Station with the most recent FuelsManager values.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <param name="gasboyDeviceList"></param>
		/// <param name="sessionType">
		/// Type of session being initiated
		/// </param>
		/// <param name="passThruClientCertificate">
		/// The pass through client certificate.
		/// </param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public bool UpdateStationDevices(
				SecurityClass security,
				GasboyStation externalStation,
				List<GasboyDevice> gasboyDeviceList,
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate)
		{
			bool transferFinished = false;

			try
			{

				List<soMean> gasboyMeans = new List<soMean>();
				List<GasboyDevice> devicesToDelete = new List<GasboyDevice>();


				// Since the Islander's API supports sending all the means in a single batch, create a single collection before sending them.
				foreach (var gasboyDevice in gasboyDeviceList)
				{
					if (gasboyDevice.RecordStatus == GasboyRecordStatus.Active)
					gasboyMeans.Add(this.CreateStationMean(security, gasboyDevice));
					else if (gasboyDevice.RecordStatus == GasboyRecordStatus.Deleted)
					devicesToDelete.Add(gasboyDevice);
				}

				soID[] ids = new soID[devicesToDelete.Count];
				int count = 0;

				foreach (var deviceToDelete in devicesToDelete)
				{
					ids[count] = new soID();
					ids[count].id = Convert.ToInt32(deviceToDelete.DeviceCode);
					count++;
				}

				this.gasboyIslanderPlus.DeleteGasboyObjects(security, externalStation, 19, ids);

				if (gasboyMeans.Count > 0)
				{
					transferFinished = this.gasboyIslanderPlus.UploadStationMeans(
						security,
						externalStation,
						sessionType,
						passThruClientCertificate,
						gasboyMeans);

					if (transferFinished)
					{
						//enable the following code when the blacklist completely replaces current blacklist instead of appending. 
						//if (externalStation.LastDeviceCount != null && externalStation.LastDeviceCount > gasboyMeans.Count)
						//{
						//	int startMean = gasboyMeans.Count;
						//	int numToDelete = externalStation.LastDeviceCount.GetValueOrDefault() - gasboyMeans.Count;

						//	soID[] ids = new soID[numToDelete];
						//	for (int i = startMean; i < numToDelete; ++i)
						//	{
						//		ids[i] = new soID();
						//		ids[i].id = i + 900000001;
						//	}

						//	this.gasboyIslanderPlus.DeleteGasboyObjects(security, externalStation, 19, ids);
						//}

						externalStation.LastDeviceCount = gasboyMeans.Count; // may be unecessary now
						GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(security, externalStation));
					}
				}
			}
			catch (EndpointNotFoundException eX)
			{
				string msg = string.Format(
					"Unable to connect to the Gasboy Islander Station ({0}).  Exception: {1}\nStack Trace: {2}",
					externalStation.SiteCode,
					eX.Message,
					eX.StackTrace);

				_EventLog.WriteEntry(msg, EventLogEntryType.Error);
			}

			return transferFinished;
		}

		public bool DeleteAllMeans(
			SecurityClass security,
			GasboyStation externalStation,
			ExternalStationSessionType sessionType,
			byte[] passThruClientCertificate)
		{
			soID[] ids = new soID[3000];
			int deviceCode = 900000001;
			for (int i = 0; i < 3000; ++i)
			{
				ids[i] = new soID();
				ids[i].id = i + 900000001;
			}


			return (this.gasboyIslanderPlus.DeleteGasboyObjects(security, externalStation, 19, ids));



		}

		/// <summary>
		/// Pushes the default fleet and departments to the Gasboy Islander
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <param name="sessionType">
		/// Type of session being initiated
		/// </param>
		/// <param name="passThruClientCertificate">
		/// The pass through client certificate.
		/// </param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public bool PushDefaultFleetAndDepartments(
				SecurityClass security,
				GasboyStation externalStation,
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate)
		{
			bool transferFinished = false;

			try
			{

					transferFinished = this.gasboyIslanderPlus.PushGasboyFleets(
						security,
						externalStation,
						sessionType,
						passThruClientCertificate
						);
				if (transferFinished)
				{
						transferFinished = this.gasboyIslanderPlus.PushGasboyDepartments(
							security,
							externalStation,
							sessionType,
							passThruClientCertificate
							);
				}

			}
			catch (EndpointNotFoundException eX)
			{
				string msg = string.Format(
					"Unable to connect to the Gasboy Islander Station ({0}).  Exception: {1}\nStack Trace: {2}",
					externalStation.SiteCode,
					eX.Message,
					eX.StackTrace);

				_EventLog.WriteEntry(msg, EventLogEntryType.Error);
			}

			return transferFinished;
		}

		/// <summary>
		/// The execute periodic data transfer with remote Gasboy Station
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public bool ExecutePeriodicDataTransfer(SecurityClass security, GasboyStation externalStation)
		{
			return this.GetTransactions(security, externalStation, ExternalStationSessionType.Periodic, null);
		}

		/// <summary>
		/// Attempts to connect to a Gasboy Station and authenticate.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public bool TestConnection(SecurityClass security, GasboyStation externalStation)
		{
			return this.gasboyIslanderPlus.TestConnection(security, externalStation);
		}

		/// <summary>
		///	Determines if there are any pending periodic download actions that need to take place.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <param name="callerLastDownloadDateTime">
		///	The caller Last Download Date Time.
		/// </param>
		/// <returns>
		///	The <see cref="bool" />.
		/// </returns>
		public bool HasPendingPeriodicDownloadActions(
				SecurityClass security,
				DateTimeOffset callerLastDownloadDateTime)
		{
			DateTimeOffset? lastDownloadDateTime = this.GetLastDataTransferDateTime(security);

			//GetLastDataTransferDateTime - I don't understand what this is for even if implemented properly. I believe the callerLastDownloadDateTime should always take precedence
			//if ((!lastDownloadDateTime.HasValue) || (lastDownloadDateTime.Value < callerLastDownloadDateTime))
			{
				lastDownloadDateTime = callerLastDownloadDateTime;
			}

			return this.NeedToPerformPeriodicDownload(security, lastDownloadDateTime.Value);
		}

		/// <summary>
		/// Determines if the specified Gasboy station is configured and ready to process transactions.
		/// </summary>
		/// <param name="security">
		/// Instance of the current security context
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <returns>
		///	True <see cref="bool" /> 
		///	otherwise; False to indicate that this system has downloaded at least once in the past.
		/// </returns>
		public bool InitialGasboyConfigurationRequired(SecurityClass security, GasboyStation externalStation)
		{
			bool isConfigured = true;

			return !isConfigured;
		}

		/// <summary>
		/// The stop data transfer per a system initiated request
		/// </summary>
		public void SystemStopDataTransfer()
		{
			;
		}


		/// <summary>
		///	The stop data transfer per a user initiated request
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		public void StopDataTransfer(SecurityClass security)
		{
		}

		#endregion Public Methods and Operators


		#region Internal Support Methods - Private

		/// <summary>
		///	The get transport authentication certificate.
		/// </summary>
		/// <param name="clientCredentials">
		///	The client credentials.
		/// </param>
		/// <returns>
		///	The <see cref="X509Certificate2" />.
		/// </returns>
		private static X509Certificate2 GetTransportAuthenticationCertificate(
				ClientServiceCredentials clientCredentials)
		{
			X509Certificate2 serverAuthCertificate = null;

			if (!string.IsNullOrEmpty(clientCredentials.NetworkAuthClientCertificate))
			{
				var certStore = new X509Store(StoreLocation.LocalMachine);
				certStore.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection certColl = certStore.Certificates.Find(
					X509FindType.FindBySubjectName,
					clientCredentials.NetworkAuthClientCertificate,
					true);

				if (certColl.Count > 0)
				{
					serverAuthCertificate = certColl[0];
				}

				certStore.Close();
			}

			return serverAuthCertificate;
		}

		/// <summary>
		///	The get last communications date time.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <returns>
		///	The
		///	<see>
		///			<cref>DateTimeOffset?</cref>
		///	</see>
		///	.
		/// </returns>
		private DateTimeOffset? GetLastDataTransferDateTime(SecurityClass security)
		{
			return DateTimeOffset.Now; // needs to change
		}

		/// <summary>
		///	Checks the local database for any Gasboy station that has been configured for periodic downloads.
		/// </summary>
		/// <param name="security">
		///	The current security context of the caller.  <see cref="SecurityClass" />
		/// </param>
		/// <param name="lastDownloadDateTime">
		///	The last download Date Time.
		/// </param>
		/// <returns>
		///	Returns a value of true if a periodic download request should be made, otherwise; false.
		/// </returns>
		/// <remarks>
		///	Other factors will determine whether or not periodic downloads should be performed.  For example,
		/// </remarks>
		private bool NeedToPerformPeriodicDownload(SecurityClass security, DateTimeOffset lastDownloadDateTime)
		{
			bool performPeriodicDownload = false;

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				_EventLog.WriteEntry(
					string.Format("Periodic download failed.  Insufficient User Rights for {0}", security.UserID),
					EventLogEntryType.Error);

				//SyncHelperFM.WriteErrorAlarmAndEvent(
				//        security,
				//        string.Format(
				//            "Periodic synchronization failed.  Insufficient User Rights for {0}",
				//            security.UserID));
			}
			else
			{

				List<GasboyStation> stations =
					GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(
						externalStationsService => externalStationsService.Enumerate(security));

				GasboyStationGeneralConfiguration configuration = new GasboyStationGeneralConfiguration();
				

				foreach (GasboyStation station in stations)
				{
					if (station.IdentityGuid != Guid.Empty)
					{
						if (station.LastSuccessfulConnection.Value > DateTimeOffset.Now.AddHours(-6))
						{
							station.Status = ExternalStationStatus.Good;
						}
						else
						{
							station.Status = ExternalStationStatus.NoCommunication;
						}
						GasboyChannelHelper.MakeCall<IGasboyStations>(
							externalStationsService => externalStationsService.Modify(security, station));

						configuration = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStationGeneralConfiguration>(
								externalStationsService => externalStationsService.GetGeneralConfigurationBySiteGuid(security, station.SiteGuid))
							?? new GasboyStationGeneralConfiguration();


						if (station.DownloadTransactionsAutomatically
						    && ((configuration.DownloadTransactionsIntervalMinutes.HasValue
						         && configuration.DownloadTransactionsIntervalMinutes.Value > 0)
						        && (DateTime.Now
						            >= lastDownloadDateTime.AddMinutes(configuration.DownloadTransactionsIntervalMinutes.Value))))
						{
							performPeriodicDownload = true;
						}
					}
				}
			}

			return performPeriodicDownload;
		}

		/// <summary>
		/// Purges the local logs.
		/// </summary>
		/// <param name="localSyncSecurity">The local synchronize security.</param>
		/// <param name="maximumDaysToRetainLogs">The maximum days to retain logs.</param>
		private void PurgeLocalLogs(SecurityClass localSyncSecurity, int maximumDaysToRetainLogs)
		{
			// Call into service layer to purge logs
		}

		private void CleanupAbandonedGasboyController(SecurityClass security)
		{
			//
		}

		/// <summary>
		/// This method is responsible for setting up a Gasboy session and determining which data components should be downloaded / uploaded
		/// to the specified Gasboy station. 
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="externalStation">
		/// The selected Gasboy Station.
		/// </param>
		/// <param name="passThruClientCertificate">
		/// A byte[] that contains the client certificate of the user who originally initiated the request.
		/// </param>
		/// <param name="externalTransactions"></param>
		/// <param name="sessionType">
		/// Type of session being initiated
		/// </param>
		/// <returns>
		/// true if the entire data transfer process completed, otherwise; false which indicates the process was interrupted
		/// prior to completion.
		/// </returns>
		/// <remarks>
		/// </remarks>
		private bool ProcessTransactionsFromGasboyStation(
				SecurityClass security,
				GasboyStation externalStation,
				List<soTransaction> externalTransactions, 
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate)
		{
			bool transferFinished = false;

			try
			{
				List<GasboyStationTransaction> downloadedTransactions = new List<GasboyStationTransaction>();

				if (null != externalTransactions && externalTransactions.Count > 0)
				{
					foreach (var transaction in externalTransactions)
					{
						var gasboyTransaction = this.CreateGasboyTransaction(
							security,
							transaction,
							externalStation);

						downloadedTransactions.Add(gasboyTransaction);
					}

					var result =
						GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, string>(
							x => x.ImportTransactions(security, externalStation, downloadedTransactions));

					if (result.ToUpper().Equals("SUCCESS!"))
					{
						this.gasboyIslanderPlus.MarkTransactionsReceivedOnGasboyStation(
							security,
							externalStation,
							this.CreateGasboyTransactionIdCollection(security, downloadedTransactions));

						transferFinished = true;
					}
				}
				else
				{
					transferFinished = true;
				}
			}
			catch (EndpointNotFoundException eX)
			{
				//string msg =
				//    string.Format(
				//            "Unable to connect to enterprise synchronization service.  Check config settings syncEnterpriseBusinessBindingType and syncEnterpriseBusinessBindingConfiguration.  These should match the enterprise server service endpoint settings.  Exception: {0}",
				//            eX.Message);

				//_EventLog.WriteEntry(msg, EventLogEntryType.Error);
				//SyncHelperFM.WriteErrorAlarmAndEvent(security, msg);
			}
			finally
			{
				//if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				//{
				//    FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity));
				//}
			}

			return transferFinished;
		}

		private List<GasboyStationLog> GetGasboyStationLogsFromGasboyStation(SecurityClass security,
			GasboyStation externalStation,
			ExternalStationSessionType sessionType,
			byte[] passThruClientCertificate)
		{
			List<GasboyStationLog> downloadedEvents = new List<GasboyStationLog>();

			try
			{
				var externalEvents = this.gasboyIslanderPlus.DownloadEventsFromGasboyStation(
					security,
					externalStation,
					sessionType,
					passThruClientCertificate);

				if (externalEvents.Count > 0)
				{
					foreach (var stationEvent in externalEvents)
					{
						var gasboyEvent = this.CreateGasboyEvent(stationEvent, externalStation);

						downloadedEvents.Add(gasboyEvent);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			return downloadedEvents;
		}

		private GasboyStationProduct CreateGasboyProduct(
			SecurityClass security,
			GasboyBOS.soProduct product,
			GasboyStation gasboyStation)
		{
			var gasboyProduct = new GasboyStationProduct()
									{
										ExternalStationGuid = gasboyStation.IdentityGuid,
										ExternalStationID = gasboyStation.ID,
										Name = product.name,
										IdentityId = product.id,
										CodeId = product.code,
										StatusIndex = product.status,
										ProductTypeIndex = product.type,
										Color = product.color,
										Density = product.density,
										Price = product.price,
										LevelLowA = product.a_level_low,
										LevelLowADeadband = product.a_level_low_deadband,
										LevelLowADeadbandTypeIndex =
										product.a_level_low_deadband_type,
										Code2 = product.code2,
										Code3 = product.code3
									};

			return gasboyProduct;
		}

		private GasboyStationTransaction CreateGasboyTransaction(
			SecurityClass security,
			GasboyBOS.soTransaction transaction,
			GasboyStation gasboyStation)
		{
			var gasboyTransaction = new GasboyStationTransaction()
			{
				ID = transaction.id.ToString(CultureInfo.CurrentCulture),

				ExternalStationGuid = gasboyStation.IdentityGuid,
				ExternalStationID = gasboyStation.ID,
				FleetID = transaction.fleet_id.ToString(CultureInfo.CurrentCulture),
				FleetName = transaction.fleet_name,
				FleetCode = transaction.fleet_code.ToString(CultureInfo.CurrentCulture),
				ProductName = transaction.product_name,
				ProductCode = transaction.product_code.ToString(CultureInfo.CurrentCulture),
				MeanID = transaction.mean_id.ToString(CultureInfo.CurrentCulture),
				MeanName = transaction.mean_name,
				FuelingVehiclePlate = transaction.plate,
				DriverMeanID = transaction.driver_mean_id.ToString(CultureInfo.CurrentCulture),
				DriverPlate = transaction.driver_plate,
				DriverTag = transaction.driver_tag,
				ExternalAuthorizationNumber = transaction.ext_auth_number,
				Density = transaction.density.ToString(CultureInfo.CurrentCulture),
				Temperature = transaction.temperature.ToString(CultureInfo.CurrentCulture),
				EngineHours = transaction.engine_hours.ToString(CultureInfo.CurrentCulture),
				PumpID = transaction.pump_id.ToString(CultureInfo.CurrentCulture),
				Pump = transaction.pump.ToString(CultureInfo.CurrentCulture),
				NozzleID = transaction.nozzle_id.ToString(CultureInfo.CurrentCulture),
				Nozzle = transaction.nozzle.ToString(CultureInfo.CurrentCulture),
				HoseNumber = transaction.hose_number.ToString(CultureInfo.CurrentCulture),
				TankName = transaction.tank_name,
				ShiftID = transaction.shift_id.ToString(CultureInfo.CurrentCulture),
				Odometer = transaction.odometer.ToString(CultureInfo.CurrentCulture),
				Quantity = transaction.quantity.ToString(CultureInfo.CurrentCulture),
				PricePerVolume = transaction.ppv.ToString(CultureInfo.CurrentCulture),
				TotalPrice = transaction.total_price.ToString(CultureInfo.CurrentCulture),
				ProxyDeviceID = transaction.proxy_id.ToString(CultureInfo.CurrentCulture),
				TransactionTimeStamp = transaction.timestamp.ToString(CultureInfo.CurrentCulture),
				TransactionType = transaction.type.ToString(CultureInfo.CurrentCulture),
				TrackData1 = transaction.track1.ToString(CultureInfo.CurrentCulture),
				TrackData2 = transaction.track2.ToString(CultureInfo.CurrentCulture),
				Tag = transaction.tag.ToString(CultureInfo.CurrentCulture),
				CashCustomerID = transaction.cash_customer_id.ToString(CultureInfo.CurrentCulture),
				DriverName = transaction.driver_name
			};

			// Serialize the message
			using (TextWriter textWriter = new StringWriter())
			{
				XmlWriterSettings settings = new XmlWriterSettings
												 {
													 Encoding = new UnicodeEncoding(false, false),
													 Indent = false,
													 OmitXmlDeclaration = false
												 };

				using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
				{
					Serializer.Serialize(xmlWriter, gasboyTransaction);
				}

				gasboyTransaction.RawTransactionData = textWriter.ToString();
			}

			return gasboyTransaction;
		}

		private GasboyBOS.soMean CreateStationMean(
			SecurityClass security,
			GasboyDevice gasboyDevice)
		{
			if (!gasboyDevice.DeviceCode.HasValue)
			{
				throw new Exception(@"Device Code must be specified.");
			}

			var gasboyMean = new GasboyBOS.soMean()
							 {
								dept_id = gasboyDevice.DepartmentID ?? GasboySpecialConstants.DefaultBlackListDepartmentID,
								fleet_id = GasboySpecialConstants.DefaultFleetID,
								id = (int)gasboyDevice.DeviceID,
								 name = gasboyDevice.DeviceName,
								 @string = gasboyDevice.CardNumber,
								 rule = GasboySpecialConstants.NoRestrictionGroupRuleCode,
								 type = (int)gasboyDevice.DeviceType,
								 status = (int)gasboyDevice.RecordStatus,
								 hardware_type = (int)gasboyDevice.HardwareType,
								 auttyp = (int)gasboyDevice.AuthorizationType,
								 employee_type = (int)gasboyDevice.EmployeeType,
								 plate = gasboyDevice.VehiclePlate,
								 driver_required = (int)gasboyDevice.DriverValidationType,
								 use_pin_code = gasboyDevice.UsePINCode ? 1 : 0,
								 pin_code = gasboyDevice.PINCode,
								 opos_prompt_for_plate = gasboyDevice.PromptForVehiclePlate ? 1 : 0,
								 opos_plate_check_type = (int)gasboyDevice.VehiclePlateCheckType,
								prompt_always_for_viu = gasboyDevice.AlwaysPromptForAdditionalValidation ? 1 : 0,
								opos_prompt_for_odometer = 1
							 };

			return gasboyMean;
		}

		/// <summary>
		/// Transform event data from the station to a GasboyStationEvent object so we can save it in the log.
		/// </summary>
		/// <param name="eventLog">Event data from the gasboy station</param>
		/// <param name="gasboyStation">The station to create the event for</param>
		/// <returns>A GasboyStationEvent object populated with event data from the station</returns>
		private GasboyStationEvent CreateGasboyEvent(soEventLog eventLog, GasboyStation gasboyStation)
		{
			DateTimeOffset logDate;

			DateTimeOffset.TryParse(eventLog.error_timestamp, out logDate);
			var gasboyStationEvent = new GasboyStationEvent
										 {
											 ExternalStationGuid = gasboyStation.IdentityGuid,
											 ID = eventLog.error_code.ToString(CultureInfo.InvariantCulture),
											 LogType = ExternalStationLogType.StationEvent,
											 LogDate = logDate,
											 ErrorClassCode = (GasboyEventErrorClassCode)eventLog.errcls_code,
											 ErrorCode = (ErrorCode)eventLog.error_code,
											 EventID = eventLog.id,
											 FleetID = eventLog.fleet_id,
											 ObjectID = eventLog.object_id,
											 EventObjectType = (GasboyEventObjectType)eventLog.object_type,
											 DeviceName = eventLog.device_name,
											 Field1 = eventLog.field1,
											 Field2 = eventLog.field2,
											 Field3 = eventLog.field3,
											 Field4 = eventLog.field4,
											 Field5 = eventLog.field5,
											 Field6 = eventLog.field6,
											 Field7 = eventLog.field7,
											 Field8 = eventLog.field8
										 };

			return gasboyStationEvent;
		}

		private List<soTransactionID> CreateGasboyTransactionIdCollection(
			SecurityClass security,
			IEnumerable<GasboyStationTransaction> gasboyTransactions)
		{
			var idList = new List<soTransactionID>();

			foreach (var trans in gasboyTransactions)
			{
				if (!string.IsNullOrEmpty(trans.ID) && TypeHelper.IsNumeric(trans.ID))
				{
					idList.Add(new soTransactionID() { id = Convert.ToInt32(trans.ID) });
				}
			}

			return idList;
		}

		#endregion Internal Support Methods - Private
	}
}