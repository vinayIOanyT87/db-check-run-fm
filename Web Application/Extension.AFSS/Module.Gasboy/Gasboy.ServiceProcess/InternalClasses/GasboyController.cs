// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyController.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.ServiceProcess.InternalClasses
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
    using System.Threading.Tasks;
    using System.Web.UI.WebControls;
    using System.Xml;
    using System.Xml.Serialization;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Generic;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FuelsManager.Afss.BusinessObjects.Constants;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
    using FuelsManager.Afss.Module.Gasboy.ServiceProcess.Gasboy;

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
        private static EventLog _EventLog = new EventLog("Application", ".", "GasboyController");


        private static Object _ActiveGasboyControllerLock = new Object();

        /// <summary>
        /// Used to transform raw external station transaction data into an object
        /// </summary>
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(GasboyStationTransaction));

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
                byte[] passThruClientCertificate)
        {
            return this.DownloadProductListFromGasboyStation(security, externalStation, passThruClientCertificate);
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
            return this.DownloadTransactionsFromGasboyStation(security, externalStation, sessionType, passThruClientCertificate);
        }

        /// <summary>
        /// Get new events from the specified station
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStation">The station to get events for</param>
        /// <param name="sessionType">Type of session being initiated</param>
        /// <returns>True if the download was successful</returns>
        public bool GetEvents(SecurityClass security, GasboyStation externalStation, ExternalStationSessionType sessionType)
        {
            return this.DownloadEventsFromGasboyStation(security, externalStation, sessionType);
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
                byte[] passThruClientCertificate)
        {
            return this.DownloadSelectedTransactionsFromGasboyStation(security, externalStation, beginTransactionID, endTransactionID, passThruClientCertificate);
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
            var state = new SiteOmatClassSoapResponse();

            if (!string.IsNullOrEmpty(externalStation.IpAddress) && externalStation.SiteCode.HasValue)
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                var svc = CreateGasboyServiceEndpoint(externalStation.IpAddress, "http://orpak.com/SiteOmatServices/");

                state.Service = svc;

                try
                {
                    var response = svc.SOLogin(externalStation.UserName, externalStation.Password);

                    if (response.rc == 0)
                    {
                        return true;
                    }
                }
                catch (Exception eX)
                {
                    throw eX;
                }
                finally
                {
                }
            }

            return false;
        }

        private void SOLoginCallback(IAsyncResult ar)
        {
            var response = (SiteOmatClassSoapResponse)ar.AsyncState;

            response.Response = response.Service.EndSOLogin(ar);
        }

        /// <summary>
        ///	The get transfer state.
        /// </summary>
        /// <param name="security">
        ///	The security.
        /// </param>
        /// <returns>
        ///	The />.
        /// </returns>
        public GasboyServiceState GetSessionState(SecurityClass security)
        {
            return this.GetCurrentGasboySessionState(security);
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

            if ((!lastDownloadDateTime.HasValue) || (lastDownloadDateTime.Value < callerLastDownloadDateTime))
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

        #region Methods

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
        ///	The get current communications state.
        /// </summary>
        /// <param name="security">
        ///	The security.
        /// </param>
        /// <returns>
        ///	The <see cref="GasboyServiceState" />.
        /// </returns>
        private GasboyServiceState GetCurrentGasboySessionState(SecurityClass security)
        {
            var serviceState = new GasboyServiceState()
                                        {
                                            WorkInProgress = false,
                                            AsOfDate = DateTimeOffset.Now,
                                            ServiceState = ExternalStationServiceProcessState.Ready
                                        };

            // First we should see if synchronization is enabled or not on the client.
            try
            {
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
            }
            catch (Exception eX)
            {
                // PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
                //_EventLog.WriteEntry(
                //    string.Format(
                //            "Synchronization exception encountered while checking synchronization status: {0}",
                //            eX.Message),
                //    EventLogEntryType.Error);
                //SyncHelperFM.WriteErrorAlarmAndEvent(
                //    security,
                //    string.Format(
                //            "Synchronization encountered an exception while checking synchronization status: {0}",
                //            eX.Message));
            }

            return serviceState;
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
            return DateTimeOffset.Now;
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

            try
            {
                if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
                {
                    _EventLog.WriteEntry(
                            string.Format(
                                "Periodic download failed.  Insufficient User Rights for {0}",
                                security.UserID),
                            EventLogEntryType.Error);

                    //SyncHelperFM.WriteErrorAlarmAndEvent(
                    //        security,
                    //        string.Format(
                    //            "Periodic synchronization failed.  Insufficient User Rights for {0}",
                    //            security.UserID));
                }
                else
                {
                    GasboyStation station = new GasboyStation(); // Change this to get the actual record.
                    GasboyStationGeneralConfiguration configuration = new GasboyStationGeneralConfiguration(); // Change this to get the actual record too

                    if (station.IdentityGuid != Guid.Empty)
                    {
                        if (station.DownloadTransactionsAutomatically &&
                            ((configuration.DownloadTransactionsIntervalMinutes.HasValue && configuration.DownloadTransactionsIntervalMinutes.Value > 0)
                            && (DateTime.Now >= lastDownloadDateTime.AddMinutes(configuration.DownloadTransactionsIntervalMinutes.Value))))
                        {
                            performPeriodicDownload = true;
                        }
                    }
                }
            }
            catch (Exception eX)
            {
                // PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
                //_EventLog.WriteEntry(
                //    string.Format("Synchronization exception encountered: {0}", eX.Message),
                //    EventLogEntryType.Error);
                //SyncHelperFM.WriteErrorAlarmAndEvent(
                //    security,
                //    string.Format("Synchronization encountered an exception: {0}", eX.Message));
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
        private List<GasboyStationProduct> DownloadProductListFromGasboyStation(
                SecurityClass security,
                GasboyStation externalStation,
                byte[] passThruClientCertificate)
        {
            var gasboyProductList = new List<GasboyStationProduct>();

            try
            {
                if (!string.IsNullOrEmpty(externalStation.IpAddress) && externalStation.SiteCode.HasValue)
                {
                    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                    var svc = CreateGasboyServiceEndpoint(externalStation.IpAddress, "http://orpak.com/SiteOmatServices/");

                    try
                    {
                        Gasboy.LoginResponse loginResponse = svc.SOLogin(externalStation.UserName, externalStation.Password);

                        if (loginResponse.rc == 0)
                        {
                            Gasboy.SOProductsResponse productResponse = svc.SOGetProductList(
                                loginResponse.SessionID,
                                externalStation.SiteCode.Value);

                            if (productResponse.rc == 0)
                            {
                                if (productResponse.num_of_products == 0)
                                {

                                }
                                else
                                {
                                    foreach (var product in productResponse.a_soProduct)
                                    {
                                        var gasboyProduct = GasboyController.CreateGasboyProduct(
                                            security,
                                            product,
                                            externalStation,
                                            loginResponse);

                                        // Should we attempt to resolve any mappings?

                                        gasboyProductList.Add(gasboyProduct);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception eX)
                    {
                        throw eX;
                    }
                    finally
                    {
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
            catch (Exception eX)
            {
                // PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
                //_EventLog.WriteEntry(
                //    string.Format("Synchronization exception encountered: {0}", eX.Message),
                //    EventLogEntryType.Error);
                //SyncHelperFM.WriteErrorAlarmAndEvent(
                //    security,
                //    string.Format("Synchronization encountered an exception: {0}", eX.Message));
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
        private bool DownloadTransactionsFromGasboyStation(
                SecurityClass security,
                GasboyStation externalStation,
                ExternalStationSessionType sessionType, 
                byte[] passThruClientCertificate)
        {
            bool transferFinished = false;

            SecurityClass localSecurity = null;

            var sessionStatus = 0;

            try
            {
                if (sessionType != ExternalStationSessionType.Manual)
                {
                    // We need to establish the service's operational credentials - limited access.
                    ;
                }

                if (!string.IsNullOrEmpty(externalStation.IpAddress) && externalStation.SiteCode.HasValue)
                {
                    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                    var svc = CreateGasboyServiceEndpoint(externalStation.IpAddress, "http://orpak.com/SiteOmatServices/");

                    try
                    {
                        Gasboy.LoginResponse loginResponse = svc.SOLogin(externalStation.UserName, externalStation.Password);

                        if (loginResponse.rc == 0)
                        {
                            Gasboy.SOTransactionsResponse transResponse =
                                svc.SOHOGetNewUpdatedTransactions(
                                    loginResponse.SessionID,
                                    externalStation.SiteCode.Value,
                                    3,
                                    150);

                            if (transResponse.rc == 0)
                            {
                                if (transResponse.num_transactions == 0)
                                {
                                    
                                }
                                else
                                {
                                    var gasboyTransactions = new List<GasboyStationTransaction>();

                                    foreach (var transaction in transResponse.a_soTransaction)
                                    {
                                        gasboyTransactions.Add(GasboyController.CreateGasboyTransaction(
                                            security,
                                            transaction,
                                            externalStation,
                                            loginResponse));
                                    }

                                    var result = GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, string>(x => x.ImportTransactions(security, externalStation, gasboyTransactions));

                                    if (result.ToUpper().Equals("SUCCESS!"))
                                    {
                                        this.MarkTransactionsReceivedOnGasboyStation(
                                            security,
                                            externalStation,
                                            gasboyTransactions);

                                        transferFinished = true;
                                    }
                                }
                            }

                            return transferFinished;
                        }

                    }
                    catch (Exception eX)
                    {
                        throw eX;
                    }
                    finally
                    {
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
            catch (Exception eX)
            {
                // PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
                //_EventLog.WriteEntry(
                //    string.Format("Synchronization exception encountered: {0}", eX.Message),
                //    EventLogEntryType.Error);
                //SyncHelperFM.WriteErrorAlarmAndEvent(
                //    security,
                //    string.Format("Synchronization encountered an exception: {0}", eX.Message));
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
        private bool DownloadEventsFromGasboyStation(
                SecurityClass security,
                GasboyStation externalStation,
                ExternalStationSessionType sessionType)
        {
            bool transferFinished = false;

            if (sessionType != ExternalStationSessionType.Manual)
            {
                // We need to establish the service's operational credentials - limited access.
            }

            if (!string.IsNullOrEmpty(externalStation.IpAddress) && externalStation.SiteCode.HasValue)
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                var svc = CreateGasboyServiceEndpoint(externalStation.IpAddress, "http://orpak.com/SiteOmatServices/");

                LoginResponse loginResponse = svc.SOLogin(externalStation.UserName, externalStation.Password);

                if (loginResponse.rc == 0)
                {
                    SOEventsResponse eventsResponse =
                        svc.SOHOGetNewUpdatedEvents(
                            loginResponse.SessionID,
                            externalStation.SiteCode.Value,
                            3,
                            150);

                    if (eventsResponse.rc != 0)
                    {
                        return false;
                    }

                    if (eventsResponse.num_events > 0)
                    {
                        List<GasboyStationLog> downloadedEvents = new List<GasboyStationLog>();

                        foreach (var stationEvent in eventsResponse.a_soEventLog)
                        {
                            var gasboyTransaction = GasboyController.CreateGasboyEvent(
                                stationEvent,
                                externalStation);

                            downloadedEvents.Add(gasboyTransaction);
                        }

                        GasboyChannelHelper.MakeCall<IGasboyStations>(
                                gasboyStationsChannel => gasboyStationsChannel.AddExternalStationLogs(security, downloadedEvents));

                        transferFinished = true;
                    }

                    return transferFinished;
                }
            }
               
            return false;
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
        private bool DownloadSelectedTransactionsFromGasboyStation(
                SecurityClass security,
                GasboyStation externalStation,
                int beginTransactionID,
                int endTransactionID,
                byte[] passThruClientCertificate)
        {
            bool transferFinished = false;

            SecurityClass localSecurity = null;

            var sessionStatus = 0;

            try
            {
                if (!string.IsNullOrEmpty(externalStation.IpAddress) && externalStation.SiteCode.HasValue)
                {
                    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                    var svc = CreateGasboyServiceEndpoint(externalStation.IpAddress, "http://orpak.com/SiteOmatServices/");

                    try
                    {
                        Gasboy.LoginResponse loginResponse = svc.SOLogin(externalStation.UserName, externalStation.Password);

                        if (loginResponse.rc == 0)
                        {
                            Gasboy.SOTransactionsResponse transResponse =
                                svc.SOHOGetTransactionsByRange(
                                    loginResponse.SessionID,
                                    externalStation.SiteCode.Value,
                                    3,
                                    beginTransactionID,
                                    endTransactionID,
                                    0);

                            if (transResponse.rc == 0)
                            {
                                if (transResponse.num_transactions == 0)
                                {
                                    
                                }
                                else
                                {
                                    List<GasboyStationTransaction> downloadedTransactions = new List<GasboyStationTransaction>();
                                    foreach (var transaction in transResponse.a_soTransaction)
                                    {
                                        var gasboyTransaction = GasboyController.CreateGasboyTransaction(
                                            security,
                                            transaction,
                                            externalStation,
                                            loginResponse);

                                        downloadedTransactions.Add(gasboyTransaction);
                                    }

                                    var result =
                                        GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, string>(
                                            x => x.ImportTransactions(security, externalStation, downloadedTransactions));

                                    if (result.ToUpper().Equals("SUCCESS!"))
                                    {
                                        transferFinished = true;
                                    }
                                }
                            }

                            return transferFinished;
                        }

                    }
                    catch (Exception eX)
                    {
                        throw eX;
                    }
                    finally
                    {
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
            catch (Exception eX)
            {
                // PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
                //_EventLog.WriteEntry(
                //    string.Format("Synchronization exception encountered: {0}", eX.Message),
                //    EventLogEntryType.Error);
                //SyncHelperFM.WriteErrorAlarmAndEvent(
                //    security,
                //    string.Format("Synchronization encountered an exception: {0}", eX.Message));
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
        private bool MarkTransactionsReceivedOnGasboyStation(
                SecurityClass security,
                GasboyStation externalStation,
                List<GasboyStationTransaction> gasboyTransactions)
        {
            bool transactionsMarked = false;

            SecurityClass localSecurity = null;

            var sessionStatus = 0;

            ConfigurationManager.RefreshSection("appSettings");

            if (!AppSettingsHelper.GetKeyValue<bool>("gasboySkipTransactionReceivedNotification", false))
            {
                try
                {
                    string gasboySessionID = string.Empty;

                    var svc = GasboyController.GasboyLogin(security, externalStation, out gasboySessionID);

                    if (null != svc && externalStation.SiteCode.HasValue)
                    {
                        var request = new SOHONotifyTransactionLoadedRequest();
                        request.SessionID = gasboySessionID;
                        request.site_code = externalStation.SiteCode.Value;
                        request.a_soTransactionIDs = GasboyController.CreateGasboyTransactionIdCollection(
                            security,
                            gasboyTransactions);
                        request.ho_role = 3;
                        request.num_trans = gasboyTransactions.Count();

                        SOHONotifyTransactionLoadedResponse response = svc.SOHONotifyTransactionLoaded(request);

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
                catch (Exception eX)
                {
                    // PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
                    //_EventLog.WriteEntry(
                    //    string.Format("Synchronization exception encountered: {0}", eX.Message),
                    //    EventLogEntryType.Error);
                    //SyncHelperFM.WriteErrorAlarmAndEvent(
                    //    security,
                    //    string.Format("Synchronization encountered an exception: {0}", eX.Message));
                }
                finally
                {
                    //if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
                    //{
                    //    FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity));
                    //}
                }
            }

            return transactionsMarked;
        }

        private static soTransactionID[] CreateGasboyTransactionIdCollection(
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
            
            return idList.ToArray();
        }

        private static GasboyStationProduct CreateGasboyProduct(
            SecurityClass security,
            Gasboy.soProduct product,
            GasboyStation gasboyStation,
            Gasboy.LoginResponse loginResponse)
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

        private static GasboyStationTransaction CreateGasboyTransaction(
            SecurityClass security,
            Gasboy.soTransaction transaction,
            GasboyStation gasboyStation,
            Gasboy.LoginResponse loginResponse)
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
                CashCustomerID = transaction.cash_customer_id.ToString(CultureInfo.CurrentCulture)
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

        /// <summary>
        /// Transform event data from the station to a GasboyStationEvent object so we can save it in the log.
        /// </summary>
        /// <param name="eventLog">Event data from the gasboy station</param>
        /// <param name="gasboyStation">The station to create the event for</param>
        /// <returns>A GasboyStationEvent object populated with event data from the station</returns>
        private static GasboyStationEvent CreateGasboyEvent(soEventLog eventLog, GasboyStation gasboyStation)
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
                                             ErrorCode = eventLog.error_code,
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

        private static bool ValidateRemoteCertificate(
            object sender,
            X509Certificate cert,
            X509Chain chain,
            SslPolicyErrors policyErrors)
        {
            bool result = cert.Subject.ToUpper().Equals(@"E=HELPDESK@GASBOY.COM, CN=GASBOY, OU=GASBOY, O=GASBOY, L=GREENSBORO, S=NORTH CAROLINA, C=US");

            return result;
        }

        #endregion Methods

        #region Gasboy Communication Methods

        private static Gasboy.SiteOmatClassSoap GasboyLogin(SecurityClass security, GasboyStation externalStation, out string sessionID)
        {
            Gasboy.SiteOmatClassSoap svc = null;

            sessionID = string.Empty;

            if (externalStation == null)
            {
                throw new ArgumentNullException("externalStation");
            }

            if (string.IsNullOrEmpty(externalStation.IpAddress))
            {
                throw new Exception("Gasboy Station Address is required.");
            }

            if (!externalStation.SiteCode.HasValue)
            {
                throw new Exception("Gasboy Station SiteCode is required.");
            }

            ServicePointManager.ServerCertificateValidationCallback +=
                new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            svc = GasboyController.CreateGasboyServiceEndpoint(externalStation.IpAddress, "http://orpak.com/SiteOmatServices/");

            try
            {
                Gasboy.LoginResponse loginResponse = svc.SOLogin(externalStation.UserName, externalStation.Password);

                if (loginResponse.rc == 0)
                {
                    sessionID = loginResponse.SessionID;
                    return svc;
                }
                else
                {
                    // HOCOMM
                    // 123456
                    switch (loginResponse.rc)
                    {
                            // Service bad user or password
                        case 15:
                            var logEntry = new GasboyStationLog()
                                               {
                                                   IdentityGuid = Guid.NewGuid(),
                                                   LogType = ExternalStationLogType.ValidationFailure,
                                                   CreatedBy = security.UserID,
                                                   CreatedDate = DateTimeOffset.Now,
                                                   ExternalStationGuid = externalStation.IdentityGuid,
                                                   SiteGuid = security.SiteGuid
                                               };

                            GasboyChannelHelper.MakeCall<IGasboyStations>(
                                service => service.AddExternalStationLog(security, logEntry));

                            break;


                    }
                }
            }
            catch
            {

            }

            return svc;
        }

        private static Gasboy.SiteOmatClassSoap CreateGasboyServiceEndpoint(string hostName, string endpointNamespace)
        {
            // strangely, these two are equivalent
            var binding = new CustomBinding("SiteOmatClassSoap");
            //WSHttpBinding binding = new WSHttpBinding("SiteOmatClassSoap");

            var remoteAddress = new EndpointAddress(new Uri(string.Format("https://{0}/SiteOmatService/SiteOmatService.asmx", hostName)), new UpnEndpointIdentity(endpointNamespace));

            return (Gasboy.SiteOmatClassSoap)(new Gasboy.SiteOmatClassSoapClient(binding, remoteAddress));
        }

        #endregion Gasboy Communication Methods
    }

    internal class SiteOmatClassSoapResponse
    {
        public SiteOmatClassSoap Service { get; set; }
        public object Response { get; set; }
    }
}