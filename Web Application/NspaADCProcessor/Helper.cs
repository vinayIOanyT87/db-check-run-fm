// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadReferenceDataRequestProcessor.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the DownloadReferenceDataRequestProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Nspa
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

    internal class Helper : IAlarmAndEventDiscovery
	{
		#region Alarm and EventLog
        private const string ADCFailureKey = "ADC Failure Encountered";
        private const string ADCSuccessKey = "ADC Success";
        private const string ADCWarningKey = "ADC Warning";
        private const string ADCInformationKey = "ADC Information";

        public static AlarmAndEventDescriptorClass ADCFailureDescriptor = new AlarmAndEventDescriptorClass(
	        false,
	        BaseObjectClass.DataSynchronization,
            ADCFailureKey);

	    public static AlarmAndEventDescriptorClass ADCSuccessDescriptor = new AlarmAndEventDescriptorClass(
	        false,
	        BaseObjectClass.DataSynchronization,
            ADCSuccessKey);

        public static AlarmAndEventDescriptorClass ADCWarningDescriptor = new AlarmAndEventDescriptorClass(
            false,
            BaseObjectClass.DataSynchronization,
            ADCWarningKey);

        public static AlarmAndEventDescriptorClass ADCInformationDescriptor = new AlarmAndEventDescriptorClass(
            false,
            BaseObjectClass.DataSynchronization,
            ADCInformationKey);

        public static AlarmAndEventLogClass AdcLogSuccess(string clientHostName, string message)
        {
            return CreateAdcLog(ADCSuccessDescriptor, clientHostName, message);
        }

        public static AlarmAndEventLogClass AdcLogFailure(string clientHostName, string message)
        {
            return CreateAdcLog(ADCFailureDescriptor, clientHostName, message);
        }

        public static AlarmAndEventLogClass AdcLogWarning(string clientHostName, string message)
        {
            return CreateAdcLog(ADCWarningDescriptor, clientHostName, message);
        }

        public static AlarmAndEventLogClass AdcLogInformation(string clientHostName, string message)
        {
            return CreateAdcLog(ADCInformationDescriptor, clientHostName, message);
        }

        public static void LogFmEventSuccess(SecurityClass securityObject, string clientHostName, string message)
        {
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
                        alarmAndEventService => alarmAndEventService.Add(securityObject, AdcLogSuccess(clientHostName, message)
                        ));
        }

        public static void LogFmEventFailure(SecurityClass securityObject, string clientHostName, string message)
        {
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
                        alarmAndEventService => alarmAndEventService.Add(securityObject, AdcLogFailure(clientHostName, message)
                        ));
        }

        public static void LogFmEventADCWarning(SecurityClass securityObject, string clientHostName, string message)
        {
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
                        alarmAndEventService => alarmAndEventService.Add(securityObject, AdcLogWarning(clientHostName, message)
                        ));
        }

        public static void LogFmEventADCInformation(SecurityClass securityObject, string clientHostName, string message)
        {
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
                        alarmAndEventService => alarmAndEventService.Add(securityObject, AdcLogInformation(clientHostName, message)
                        ));
        }

        public static AlarmAndEventLogClass CreateAdcLog(AlarmAndEventDescriptorClass logDescriptor, string clientHostName, string message)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(logDescriptor)
            {
                AssociatedData = FormatFMLogMessage(clientHostName, message)
            };
            return alarmAndEventLog;
        }

        private static string FormatFMLogMessage(string clientHostname, string message)
        {
            string logMessage = string.Format("Client: {0} : {1}", clientHostname, message);
            return logMessage;
        }

        public AlarmAndEventDescriptorClass[] AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] Descriptors =
                {
                    ADCFailureDescriptor,
                    ADCSuccessDescriptor,
                    ADCWarningDescriptor,
                    ADCInformationDescriptor
                };
                return Descriptors;
            }
        }

        #endregion

        #region Windows EventLog
        public static string WindowsEventLogName = "Application";
        public static string WindowsEventLogMachineName = ".";
        public static string WindowsEventLogLogSource = "FuelsManager Nspa ADC Processor";
        public static string WindowsEventLogModuleName = "NspaADCProcessor";

        private static EventLog _eventLogger;

        public static EventLog NspaADCEventLog
        {
            get
            {
                if (_eventLogger == null)
                {
                    _eventLogger = new EventLog(WindowsEventLogName, WindowsEventLogMachineName, WindowsEventLogLogSource);
                }
                return _eventLogger;
            }
        }
        #endregion

        public static DispatchTransactionsSR CreateDispatchSr(SecurityClass security, DateTime targetDate)
		{
			var dispatchSR = new DispatchTransactionsSR
			{
				Security = security,
				BeginDate = targetDate,
				EndDate = targetDate.Add(new TimeSpan(24, 0, 0)),
				Site = security.SiteID,
				CurrentSiteGuid = security.SiteGuid,
				AliasNames = new List<string> { "Defuel", "Sale" },
				Statuses = new List<string> { "Dispatched" },
				SubCommand = DispatchTransactionsSR.SubCommands.None
			};

			return dispatchSR;
		}

		public static DispatchTransactionsDO GetDispatchTransactions(DispatchTransactionsSR sr)
		{

			var results = FMChannelHelper.MakeCall<IDispatchTransactionsProcessor, DispatchTransactionsDO>(
				accountingInterface =>
				{
					var innerResults = accountingInterface.Process(sr);
					return innerResults;
				});
			return results;
		}

		public static bool HasValidIdentityGuid(BaseDataObject theObject)
		{
			return theObject != null && !theObject.IdentityGuid.IsEmpty();
		}


		public static string GetSourceEquipmentId(int transTypeId, string aircraftId, string vehicleId)
		{
			// DispatchTransactionsSR's GetSQL

			var transType = (TransactionTypes)transTypeId;
			var equipmentId = string.Empty;
			switch (transType)
			{
				case TransactionTypes.T4_SecondaryDefuel:
					equipmentId = aircraftId;
					break;
				case TransactionTypes.T6_SecondaryDisbursement:
				case TransactionTypes.T10_Unload:
					equipmentId = vehicleId;
					break;
			}
			return equipmentId ?? string.Empty;
		}

		public static string GetDestinationEquipmentId(int transTypeId, string aircraftId, string vehicleId)
		{
			// DispatchTransactionsSR's GetSQL
			var transType = (TransactionTypes)transTypeId;
			var equipmentId = string.Empty;
			switch (transType)
			{
				case TransactionTypes.T4_SecondaryDefuel:
				case TransactionTypes.T7_FillStand:
					equipmentId = vehicleId;
					break;
				case TransactionTypes.T6_SecondaryDisbursement:
					equipmentId = aircraftId;
					break;
			}
			return equipmentId ?? string.Empty;
		}

        /// <summary>
        /// Logins using the given Info
        /// (THis duplicates the logic in FMDataExchange's ExchangeServic.svc.cs Exchange
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="password">The password.</param>
        /// <param name="siteId">The site identifier.</param>
        /// <param name="newSecurity"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <returns></returns>
        /// <exception cref="System.Security.SecurityException">
        /// User \ + userID + \  + result
        /// or
        /// User \ + userID + \  + not authorized to perform import or export operations.
        /// </exception>
        /// <exception cref="System.Exception"></exception>
        public static bool Login(
            string userID,
            string password,
            string siteId,
            out SecurityClass newSecurity,
            out string error)
        {
            SecurityClass mySecurity = null;
            error = string.Empty;
            try
            {
                bool changePassword = false;
                int daysUntilExpiration = 0;

                var loginRequest = new SecurityLoginRequest();
                loginRequest.UserID = userID;
                loginRequest.Password = password;
                loginRequest.SiteID = siteId;
                loginRequest.CACEnabled = false;

                string result =
                    FMChannelHelper.MakeCall<ISites, string>(
                        x => x.Login(out changePassword, out daysUntilExpiration, out mySecurity, loginRequest));

                if (result != null)
                {
                    error += "User \"" + userID + "\" " + result;
                }

                if (mySecurity == null)
                {
                    error += "Login failed.";
                }

                if (!mySecurity.HasRight(RIGHT.INTERFACE_IMPORT))
                {
                    error += string.Format(
                        "User \"{0}\" " + "not authorized to perform import or export operations.",
                        userID);
                }

                if (string.IsNullOrWhiteSpace(error) == false)
                {
                    Helper.NspaADCEventLog.WriteEntry("Error occurred during login: " + error, EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                error += string.Format("Unknown error occurred during login: {0} {1}", error, ex.Message);
                Nspa.Helper.LogFmEventADCInformation(mySecurity, "Unknown client", error);
                Helper.NspaADCEventLog.WriteEntry(error, EventLogEntryType.Error);
                error = ex.Message;
            }
            var success = string.IsNullOrWhiteSpace(error);
            newSecurity = success ? mySecurity : null;
            return success;
        }
	}
}
