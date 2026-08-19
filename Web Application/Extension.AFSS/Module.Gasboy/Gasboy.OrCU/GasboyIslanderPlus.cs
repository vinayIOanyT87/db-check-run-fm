// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyIslanderPlus.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.OrCU
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;
	using System.ServiceModel;
	using System.Xml.Serialization;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Afss.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.OrCU.Communications;
	using FuelsManager.Afss.Module.Gasboy.OrCU.GasboyBOS;

	/// <summary>
	///	External Gasboy Communications class that interfaces with remote Gasboy units.
	/// </summary>
	public class GasboyIslanderPlus
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
		///	The event log.
		/// </summary>
		private static EventLog EventLogWriter = new EventLog("Application", ".", "Gasboy.OrCU");

		private GasboyConnection Connection = new GasboyConnection();

		/// <summary>
		/// Used to transform raw external station transaction data into an object
		/// </summary>
		private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(GasboyStationTransaction));

		#endregion Constants and Fields

		#region Constructors and Destructors

		/// <summary>
		///	Initializes a new instance of the <see cref="GasboyIslanderPlus" /> class.
		/// </summary>
		public GasboyIslanderPlus()
		{
		}

		#endregion Constructors and Destructors

		#region Public Properties

		#endregion Public Properties

		#region Properties

		#endregion Properties

		#region Public Methods and Operators

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
			if (string.IsNullOrEmpty(externalStation.IpAddress))
			{
				throw new Exception("Station configuration does not contain an IP address value.");
			}

			if (!externalStation.SiteCode.HasValue)
			{
				throw new Exception("Station configuration does not contain a SiteCode value.");
			}

			// Here we create a new instance so we don't interfere with any existing Islander sessions.
			try
			{
				var gasboySession = this.Connection.GetConnection(security, externalStation);

				if (!string.IsNullOrEmpty(gasboySession.SessionID))
				{
					return true;
				}
			}
			catch (Exception eX)
			{
				throw;
			}
			finally
			{
				this.Connection.CloseConnection();
			}

			return false;
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
		/// Returns a list of fuel cards defined on the target Gasboy Station
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">
		/// The remote Gasboy station to communicate with
		/// </param>
		/// <param name="sessionType"></param>
		/// <param name="passThruClientCertificate">
		/// The pass through client certificate.
		/// </param>
		/// <param name="deviceList"></param>
		/// <returns>
		/// The <see cref="bool" />.
		/// </returns>
		public bool UploadStationMeans(
				SecurityClass security,
				GasboyStation externalStation,
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate,
				List<GasboyBOS.soMean> deviceList)
		{
			if (null == externalStation)
			{
				throw new ArgumentNullException("externalStation");
			}

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
			}

			if (string.IsNullOrEmpty(externalStation.IpAddress))
			{
				throw new Exception("Station configuration does not contain an IP address value.");
			}

			if (!externalStation.SiteCode.HasValue)
			{
				throw new Exception("Station configuration does not contain a SiteCode value.");
			}

			try
			{
				var gasboySession = this.Connection.GetConnection(security, externalStation);

				if (!string.IsNullOrEmpty(gasboySession.SessionID))
				{
					var devices = deviceList.ToArray();

					SOUpdateMeansRequest request = new SOUpdateMeansRequest(
						gasboySession.SessionID,
						externalStation.SiteCode.Value,
						deviceList.Count,
						deviceList.ToArray());

					GasboyBOS.SOUpdateMeansResponse updateResponse = gasboySession.Service.SOUpdateMeans(request);

					if (updateResponse.SOUpdateMeansResult.rc == 0)
					{
						return true;
					}
				}
				else
				{
					//SyncHelperFM.WriteConfigurationAlarmAndEvent(
					//        security,
					//        @"Unable to process synchronization request.  Site ID not specified");
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
				this.Connection.CloseConnection();
			}

			return false;
		}

		/// <summary>
		///	The get current communications state.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <returns>
		///	The <see cref="GasboyServiceState" />.
		/// </returns>
		public GasboyServiceState GetCurrentGasboySessionState(SecurityClass security)
		{
			var serviceState = new GasboyServiceState()
										{
											WorkInProgress = false,
											AsOfDate = DateTimeOffset.Now,
											ServiceState = ExternalStationServiceProcessState.Ready
										};

			// If we still have the default value of READY, look for any active sessions
			if (serviceState.ServiceState == ExternalStationServiceProcessState.Ready)
			{
				//ExternalStationSessionLogCollection activeSessionLogs = FMChannelHelper.MakeCall<IExternalStationSession, ExternalStationSessionCollection>(x => x.EnumerateActive(
				//    security));

				//// Need to filter by Station Type and Station ID

				//if (null != activeSessionLogs && activeSessionLogs.Count > 0)
				//{
				//    serviceState.GasboyServiceState = ExternalStationStatus.Good;

				//    foreach (ExternalStationSession session in activeSessionLogs)
				//    {
				//        if (session.IdentityGuid == security.Token)
				//        {
				//            serviceState.WorkInProgress = true;
				//            break;
				//        }
				//    }
				//}
			}

			return serviceState;
		}

		/// <summary>
		/// Returns a list of products defined on the target Gasboy Station
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
		public List<GasboyBOS.soProduct> DownloadProductListFromGasboyStation(
				SecurityClass security,
				GasboyStation externalStation,
				ExternalStationSessionType sessionType,
				byte[] passThruClientCertificate)
		{
			var gasboyProductList = new List<GasboyBOS.soProduct>();

			if (null == externalStation)
			{
				throw new ArgumentNullException("externalStation");
			}

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
			}

			if (string.IsNullOrEmpty(externalStation.IpAddress))
			{
				throw new Exception("Station configuration does not contain an IP address value.");
			}

			if (!externalStation.SiteCode.HasValue)
			{
				throw new Exception("Station configuration does not contain a SiteCode value.");
			}

			try
			{
				var gasboySession = this.Connection.GetConnection(security, externalStation);

				if (!string.IsNullOrEmpty(gasboySession.SessionID))
				{
					var productResponse = gasboySession.Service.SOGetProductList(
						gasboySession.SessionID,
						gasboySession.SiteCode);

					if (productResponse.rc == 0)
					{
						if (productResponse.num_of_products > 0)
						{
							gasboyProductList.AddRange(productResponse.a_soProduct);
						}
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
			}
			finally
			{
				//if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				//{
				//    FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity));
				//}
				this.Connection.CloseConnection();
			}

			return gasboyProductList;
		}

		/// <summary>
		/// This method is responsible for setting up a Gasboy session and acknowledging recieved events
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="externalStation">
		/// The selected Gasboy Station.
		/// </param>
		/// <param name="gasboyTransactions"></param>
		/// <returns>
		/// true if the entire data transfer process completed, otherwise; false which indicates the process was interrupted
		/// prior to completion.
		/// </returns>
		/// <remarks>
		/// </remarks>
		public bool MarkEventsReceivedOnGasboyStation(
				SecurityClass security,
				GasboyStation externalStation,
				soEventID[] eventIDs)
		{
			bool eventsMarked = false;

			ConfigurationManager.RefreshSection("appSettings");
				try
				{
					var gasboySession = this.Connection.GetConnection(security, externalStation);

					if (!string.IsNullOrEmpty(gasboySession.SessionID))
					{
						var request = new SOHONotifyEventLoadedRequest();
						request.SessionID = gasboySession.SessionID;
						request.site_code = gasboySession.SiteCode;
						request.a_soEventIDs = eventIDs;
						request.ho_role = 3;
						request.num_events = eventIDs.Count();

						SOHONotifyEventLoadedResponse response = gasboySession.Service.SOHONotifyEventLoaded(request);

						if (response.SOHONotifyEventLoadedResult.rc == 0)
						{
						eventsMarked = true;
						}

						return eventsMarked;
					}
					else
					{
						//SyncHelperFM.WriteConfigurationAlarmAndEvent(
						//        security,
						//        @"Unable to process synchronization request.  Site ID not specified");
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
					this.Connection.CloseConnection();
				}
			

			return eventsMarked;
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
		/// <param name="sessionType">
		/// Type of session being initiated
		/// </param>
		/// <returns>
		/// true if the entire data transfer process completed, otherwise; false which indicates the process was interrupted
		/// prior to completion.
		/// </returns>
		/// <remarks>
		/// </remarks>
		public List<GasboyBOS.soTransaction> DownloadTransactionsFromGasboyStation(
				SecurityClass security,
				GasboyStation externalStation,
				ExternalStationSessionType sessionType, 
				byte[] passThruClientCertificate)
		{
			var gasboyTransactions = new List<GasboyBOS.soTransaction>();

			if (null == externalStation)
			{
				throw new ArgumentNullException("externalStation");
			}

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
			}

			if (string.IsNullOrEmpty(externalStation.IpAddress))
			{
				throw new Exception("Station configuration does not contain an IP address value.");
			}

			if (!externalStation.SiteCode.HasValue)
			{
				throw new Exception("Station configuration does not contain a SiteCode value.");
			}

			try
			{
				var gasboySession = this.Connection.GetConnection(security, externalStation);

				if (!string.IsNullOrEmpty(gasboySession.SessionID))
				{
					SOTransactionsResponse transResponse =
						gasboySession.Service.SOHOGetNewUpdatedTransactions(
							gasboySession.SessionID,
							externalStation.SiteCode.Value,
							5,
							150);

					if (transResponse.rc == 0)
					{
						if (transResponse.num_transactions > 0)
						{
							gasboyTransactions.AddRange(transResponse.a_soTransaction);
						}
					}
				}
				else
				{
					//SyncHelperFM.WriteConfigurationAlarmAndEvent(
					//        security,
					//        @"Unable to process synchronization request.  Site ID not specified");
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
				this.Connection.CloseConnection();
			}

			return gasboyTransactions;
		}

		/// <summary>
		/// This method is responsible for setting up a Gasboy session and downloading events from the specified station
		/// </summary>
		/// <param name="security">
		/// Contains Security Information
		/// </param>
		/// <param name="externalStation">
		/// The Gasboy station to get events for.
		/// </param>
		/// <param name="sessionType">
		/// The type of session being initiated
		/// </param>
		/// <returns>
		/// True if the entire data transfer process completed, otherwise; false which indicates the process was interrupted
		/// prior to completion.
		/// </returns>
		public List<GasboyBOS.soEventLog> DownloadEventsFromGasboyStation(
			SecurityClass security,
			GasboyStation externalStation,
			ExternalStationSessionType sessionType,
			byte[] passThruClientCertificate)
		{
			List<GasboyBOS.soEventLog> downloadedEvents = new List<GasboyBOS.soEventLog>();

			if (null == externalStation)
			{
				throw new ArgumentNullException("externalStation");
			}

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
			}

			if (string.IsNullOrEmpty(externalStation.IpAddress))
			{
				throw new Exception("Station configuration does not contain an IP address value.");
			}

			if (!externalStation.SiteCode.HasValue)
			{
				throw new Exception("Station configuration does not contain a SiteCode value.");
			}

			try
			{
				var gasboySession = this.Connection.GetConnection(security, externalStation);

				if (!string.IsNullOrEmpty(gasboySession.SessionID))
				{
					SOEventsResponse eventsResponse =
						gasboySession.Service.SOHOGetNewUpdatedEvents(
							gasboySession.SessionID,
							externalStation.SiteCode.Value,
							3,
							1000);

					if ((eventsResponse.rc == 0) && eventsResponse.num_events > 0)
					{
						downloadedEvents.AddRange(eventsResponse.a_soEventLog);
					}
				}
			}
			catch (Exception eX)
			{
				throw;
			}
			finally
			{
				this.Connection.CloseConnection();
			}

			return downloadedEvents;
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
		/// <param name="sessionType">
		/// Type of session being initiated
		/// </param>
		/// <returns>
		/// true if the entire data transfer process completed, otherwise; false which indicates the process was interrupted
		/// prior to completion.
		/// </returns>
		/// <remarks>
		/// </remarks>
		public List<GasboyBOS.soTransaction> DownloadSelectedTransactionsFromGasboyStation(
			SecurityClass security,
			GasboyStation externalStation,
			int beginTransactionID,
			int endTransactionID,
			ExternalStationSessionType sessionType,
			byte[] passThruClientCertificate)
		{
			var gasboyTransactions = new List<GasboyBOS.soTransaction>();

			if (null == externalStation)
			{
				throw new ArgumentNullException("externalStation");
			}

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
			}

			if (string.IsNullOrEmpty(externalStation.IpAddress))
			{
				throw new Exception("Station configuration does not contain an IP address value.");
			}

			if (!externalStation.SiteCode.HasValue)
			{
				throw new Exception("Station configuration does not contain a SiteCode value.");
			}

			try
			{
				var gasboySession = this.Connection.GetConnection(security, externalStation);

				if (!string.IsNullOrEmpty(gasboySession.SessionID))
				{
					SOTransactionsResponse transResponse =
						gasboySession.Service.SOHOGetTransactionsByRange(
							gasboySession.SessionID,
							gasboySession.SiteCode,
							5,
							beginTransactionID,
							endTransactionID,
							0);

					if (transResponse.rc == 0)
					{
						if (transResponse.num_transactions > 0)
						{
							gasboyTransactions.AddRange(transResponse.a_soTransaction);
						}
					}
				}
			}
			catch (Exception eX)
			{
				throw;
			}
			finally
			{
				this.Connection.CloseConnection();
			}

			return gasboyTransactions;
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
		/// <param name="gasboyTransactions"></param>
		/// <returns>
		/// true if the entire data transfer process completed, otherwise; false which indicates the process was interrupted
		/// prior to completion.
		/// </returns>
		/// <remarks>
		/// </remarks>
		public bool MarkTransactionsReceivedOnGasboyStation(
				SecurityClass security,
				GasboyStation externalStation,
				List<GasboyBOS.soTransactionID> gasboyTransactions)
		{
			bool transactionsMarked = false;

			ConfigurationManager.RefreshSection("appSettings");

			if (!AppSettingsHelper.GetKeyValue<bool>("gasboySkipTransactionReceivedNotification", false))
			{
				try
				{
					var gasboySession = this.Connection.GetConnection(security, externalStation);

					if (!string.IsNullOrEmpty(gasboySession.SessionID))
					{
						var request = new SOHONotifyTransactionLoadedRequest();
						request.SessionID = gasboySession.SessionID;
						request.site_code = gasboySession.SiteCode;
						request.a_soTransactionIDs = gasboyTransactions.ToArray();
						request.ho_role = 3;
						request.num_trans = gasboyTransactions.Count();

						SOHONotifyTransactionLoadedResponse response = gasboySession.Service.SOHONotifyTransactionLoaded(request);

						if (response.SOHONotifyTransactionLoadedResult.rc == 0)
						{
							transactionsMarked = true;
						}

						return transactionsMarked;
					}
					else
					{
						//SyncHelperFM.WriteConfigurationAlarmAndEvent(
						//        security,
						//        @"Unable to process synchronization request.  Site ID not specified");
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
					this.Connection.CloseConnection();
				}
			}

			return transactionsMarked;
		}

		public bool DeleteGasboyObjects(SecurityClass security, GasboyStation externalStation, int type, soID[] ids )
		{

			var gasboySession = this.Connection.GetConnection(security, externalStation);
			SODeleteObjectsByIDRequest request = new SODeleteObjectsByIDRequest(
				gasboySession.SessionID,
				externalStation.SiteCode.Value,
				type,
				ids.Length,
				ids);

			GasboyBOS.SODeleteObjectsByIDResponse updateResponse = gasboySession.Service.SODeleteObjectsByID(request);

			if (updateResponse.SODeleteObjectsByIDResult.rc == 0)
			{
				return true;
			}

			return false;
		}

		public bool PushGasboyFleets(
			SecurityClass security,
			GasboyStation externalStation,
			ExternalStationSessionType sessionType,
			byte[] passThruClientCertificate)
		{
			if (null == externalStation)
			{
				throw new ArgumentNullException("externalStation");
			}

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
			}

			if (string.IsNullOrEmpty(externalStation.IpAddress))
			{
				throw new Exception("Station configuration does not contain an IP address value.");
			}

			if (!externalStation.SiteCode.HasValue)
			{
				throw new Exception("Station configuration does not contain a SiteCode value.");
			}

			try
			{
				var gasboySession = this.Connection.GetConnection(security, externalStation);

				if (!string.IsNullOrEmpty(gasboySession.SessionID))
				{
					//should be refactored if/when fleets and depts become configurable 
					soFleet[] fleet = new soFleet[1];
					fleet[0] = new soFleet();
					fleet[0].id = GasboySpecialConstants.DefaultFleetID;
					fleet[0].name = GasboySpecialConstants.DefaultFleetName;
					fleet[0].status = 2;
					fleet[0].code = GasboySpecialConstants.DefaultFleetCode;
					fleet[0].default_rule = 200000000;
					fleet[0].address = " ";
					fleet[0].phone = " ";
					fleet[0].fax = " ";
					fleet[0].email = " ";
					fleet[0].contact = " ";
					fleet[0].acctyp = 0;
					fleet[0].available_amount = 0;
					fleet[0].min_allowed = 0;
					fleet[0].use_pin_code = 0;
					fleet[0].auth_pin_from = 2;
					fleet[0].nr_pin_retries = 0;
					fleet[0].block_if_pin_retries_fail = 0;
					fleet[0].opos_prompt_for_plate = 0;
					fleet[0].opos_prompt_for_odometer = 0;
					fleet[0].do_odo_reasonability_check = 0;
					fleet[0].max_eh_delta_allowed = 0;
					fleet[0].nr_odo_retries = 0;
					fleet[0].price_list_id = 0;
					fleet[0].use_rule_limit = 0;
					fleet[0].max_rules = 0;
					fleet[0].max_group_rules = 0;
					fleet[0].eft_id = 0;
					fleet[0].wex_renewal_fee = 0;
					fleet[0].wex_billing_fee_56 = 0;
					fleet[0].on_line_fee_68 = 0;
					fleet[0].line_of_credit = 0;
					fleet[0].opos_prompt_for_engine_hours = 0;
					fleet[0].prompt_always_for_viu = 1;
					fleet[0].do_eh_reasonability_check = 1;
					fleet[0].max_eh_delta_allowed = 0;
					fleet[0].nr_eh_retries = 0;
					fleet[0].reject_if_eh_check_fails = 0;
					fleet[0].mobile = "0";


					SOUpdateFleetsRequest request = new SOUpdateFleetsRequest(gasboySession.SessionID,externalStation.SiteCode.Value,1,fleet);

					GasboyBOS.SOUpdateFleetsResponse updateResponse = gasboySession.Service.SOUpdateFleets(request);

					if (updateResponse.SOUpdateFleetsResult.rc == 0)
					{
						return true;
					}
				}
				else
				{
					//SyncHelperFM.WriteConfigurationAlarmAndEvent(
					//        security,
					//        @"Unable to process synchronization request.  Site ID not specified");
				}
			}
			
			catch (Exception)
			{
				
				throw;
			}
			finally
			{
				//if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				//{
				//    FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity));
				//}
				this.Connection.CloseConnection();
			}
			return false;
		}

		public bool PushGasboyDepartments(
			SecurityClass security,
			GasboyStation externalStation,
			ExternalStationSessionType sessionType,
			byte[] passThruClientCertificate)
		{
			if (null == externalStation)
			{
				throw new ArgumentNullException("externalStation");
			}

			if (sessionType != ExternalStationSessionType.Manual)
			{
				// We need to establish the service's operational credentials - limited access.
			}

			if (string.IsNullOrEmpty(externalStation.IpAddress))
			{
				throw new Exception("Station configuration does not contain an IP address value.");
			}

			if (!externalStation.SiteCode.HasValue)
			{
				throw new Exception("Station configuration does not contain a SiteCode value.");
			}

			try
			{
				var gasboySession = this.Connection.GetConnection(security, externalStation);

				if (!string.IsNullOrEmpty(gasboySession.SessionID))
				{
					//should be refactored if/when fleets and depts become configurable 
					soDept[] departments = new soDept[2];
					departments[0] = new soDept();
					departments[1] = new soDept();

//configure the default department
					departments[0].id = GasboySpecialConstants.DefaultDepartmentID;
					departments[0].fleet_id = GasboySpecialConstants.DefaultFleetID;
					departments[0].name = GasboySpecialConstants.DefaultDepartmentName;
					departments[0].status = 2;
					departments[0].code = GasboySpecialConstants.DefaultDepartmentCode;
					departments[0].default_rule = 200000000;
					departments[0].address = " ";
					departments[0].phone = " ";
					departments[0].fax = " ";
					departments[0].email = " ";
					departments[0].contact = " ";
					departments[0].use_pin_code = 0;
					departments[0].auth_pin_from = 2;
					departments[0].nr_pin_retries = 0;
					departments[0].block_if_pin_retries_fail = 0;
					departments[0].opos_prompt_for_plate = 0;
					departments[0].opos_prompt_for_odometer = 0;
					departments[0].do_odo_reasonability_check = 1;
					departments[0].max_odo_delta_allowed = 0;
					departments[0].nr_odo_retries = 0;
					departments[0].price_list_id = 0;
					departments[0].black_white_type = 1;
					departments[0].opos_prompt_for_engine_hours = 0;
					departments[0].prompt_always_for_viu = 1;
					departments[0].do_eh_reasonability_check = 1;
					departments[0].max_eh_delta_allowed = 0;
					departments[0].nr_eh_retries = 0;
					departments[0].reject_if_eh_check_fails = 0;

//configure the blacklist department
					departments[1].id = GasboySpecialConstants.DefaultBlackListDepartmentID;
					departments[1].fleet_id = GasboySpecialConstants.DefaultFleetID;
					departments[1].name = GasboySpecialConstants.DefaultBlackListDepartmentName;
					departments[1].status = 2;
					departments[1].code = GasboySpecialConstants.DefaultBlackListDepartmentCode;
					departments[1].default_rule = 200000000;
					departments[1].address = " ";
					departments[1].phone = " ";
					departments[1].fax = " ";
					departments[1].email = " ";
					departments[1].contact = " ";
					departments[1].use_pin_code = 0;
					departments[1].auth_pin_from = 2;
					departments[1].nr_pin_retries = 0;
					departments[1].block_if_pin_retries_fail = 0;
					departments[1].opos_prompt_for_plate = 0;
					departments[1].opos_prompt_for_odometer = 0;
					departments[1].do_odo_reasonability_check = 1;
					departments[1].max_odo_delta_allowed = 0;
					departments[1].nr_odo_retries = 0;
					departments[1].price_list_id = 0;
					departments[1].black_white_type = 2;
					departments[1].opos_prompt_for_engine_hours = 0;
					departments[1].prompt_always_for_viu = 1;
					departments[1].do_eh_reasonability_check = 1;
					departments[1].max_eh_delta_allowed = 0;
					departments[1].nr_eh_retries = 0;
					departments[1].reject_if_eh_check_fails = 0;



					SOUpdateDeptsRequest request = new SOUpdateDeptsRequest(gasboySession.SessionID, externalStation.SiteCode.Value, 2, departments);

					GasboyBOS.SOUpdateDeptsResponse updateResponse = gasboySession.Service.SOUpdateDepts(request);

					if (updateResponse.SOUpdateDeptsResult.rc == 0)
					{
						return true;
					}
				}
				else
				{
					//SyncHelperFM.WriteConfigurationAlarmAndEvent(
					//        security,
					//        @"Unable to process synchronization request.  Site ID not specified");
				}
			}

			catch (Exception)
			{

				throw;
			}
			finally
			{
				//if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				//{
				//    FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity));
				//}
				this.Connection.CloseConnection();
			}
			return false;
		}

		#endregion Public Methods and Operators

		#region Helper Methods

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

		#endregion Helper Methods
	}
}