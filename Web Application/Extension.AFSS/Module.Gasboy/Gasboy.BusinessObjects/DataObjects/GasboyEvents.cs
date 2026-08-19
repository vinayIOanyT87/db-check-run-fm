using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
	using FMBusinessObjects.DataObjects;

	public class GasboyEvents : IAlarmAndEventDiscovery
	{
		private static string GasboyTestConnectionSuccessKey = "Test connection success";
		public static AlarmAndEventDescriptorClass GasboyTestConnectionSuccessDescriptor = new AlarmAndEventDescriptorClass(false,"Gasboy", GasboyTestConnectionSuccessKey);

		private static string GasboyTestConnectionErrorKey = "Test connection error";
		public static AlarmAndEventDescriptorClass GasboyTestConnectionErrorDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyTestConnectionErrorKey);

		private static string GasboyManualTransactionDownloadInitiatedKey = "Gasboy Station Manual Transaction Download Initiated";
		public static AlarmAndEventDescriptorClass GasboyManualTransactionDownloadInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyManualTransactionDownloadInitiatedKey);

		private static string GasboyManualTransactionDownloadCompleteKey = "Gasboy Station Manual Transaction Download Complete";
		public static AlarmAndEventDescriptorClass GasboyManualTransactionDownloadCompleteDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyManualTransactionDownloadCompleteKey);

		private static string GasboyManualTransactionDownloadErrorKey = "Gasboy Station Manual Transaction Download Error";
		public static AlarmAndEventDescriptorClass GasboyManualTransactionDownloadErrorDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyManualTransactionDownloadErrorKey);

		private static string GasboyDevicePushInitiatedKey = "Gasboy Station Device Push Initiated";
		public static AlarmAndEventDescriptorClass GasboyDevicePushInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyDevicePushInitiatedKey);

		private static string GasboyDevicePushCompleteKey = "Gasboy Station Device Push Complete";
		public static AlarmAndEventDescriptorClass GasboyDevicePushCompleteDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyDevicePushCompleteKey);

		private static string GasboyDevicePushErrorKey = "Gasboy Station Device Push Error";
		public static AlarmAndEventDescriptorClass GasboyDevicePushErrorDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyDevicePushErrorKey);

		private static string GasboyPeriodicTransactionDownloadInitiatedKey = "Gasboy Station Periodic Transaction Download Initiated";
		public static AlarmAndEventDescriptorClass GasboyPeriodicTransactionDownloadInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyPeriodicTransactionDownloadInitiatedKey);

		private static string GasboyPeriodicTransactionDownloadCompleteKey = "Gasboy Station Periodic Transaction Download Complete";
		public static AlarmAndEventDescriptorClass GasboyPeriodicTransactionDownloadCompleteDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyPeriodicTransactionDownloadCompleteKey);

		private static string GasboyPeriodicTransactionDownloadErrorKey = "Gasboy Station Periodic Transaction Download Error";
		public static AlarmAndEventDescriptorClass GasboyPeriodicTransactionDownloadErrorDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyPeriodicTransactionDownloadErrorKey);

		private static string GasboyTransactionImportInitiatedKey = "Gasboy Transaction Import Initiated";
		public static AlarmAndEventDescriptorClass GasboyTransactionImportInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyTransactionImportInitiatedKey);

		private static string GasboyTransactionImportCompleteKey = "Gasboy Transaction Import Completed";
		public static AlarmAndEventDescriptorClass GasboyTransactionImportCompleteDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyTransactionImportCompleteKey);

		private static string GasboyTransactionImportErrorKey = "Gasboy Transaction Import Error";
		public static AlarmAndEventDescriptorClass GasboyTransactionImportErrorDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyTransactionImportErrorKey);

		private static string GasboyReprocessTransactionInitiatedKey = "Gasboy Reprocess Transaction Initiated";
		public static AlarmAndEventDescriptorClass GasboyReprocessTransactionInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyReprocessTransactionInitiatedKey);

		private static string GasboyReprocessTransactionCompleteKey = "Gasboy Reprocess Transaction Completed";
		public static AlarmAndEventDescriptorClass GasboyReprocessTransactionCompleteDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyReprocessTransactionCompleteKey);

		private static string GasboyReprocessTransactionErrorKey = "Gasboy Reprocess Transaction Error";
		public static AlarmAndEventDescriptorClass GasboyReprocessTransactionErrorDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyReprocessTransactionErrorKey);

		private static string GasboyOnlineAuthorizationDeniedKey = "Gasboy Online Authorization Denied";
		public static AlarmAndEventDescriptorClass GasboyOnlineAuthorizationDeniedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyOnlineAuthorizationDeniedKey);

		private static string GasboyOnlineAuthorizationApprovedKey = "Gasboy Online Authorization Approved";
		public static AlarmAndEventDescriptorClass GasboyOnlineAuthorizationApprovedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyOnlineAuthorizationApprovedKey);

		private static string GasboyEventCollectionCompleteKey = "Gasboy Event Collection Complete";
		public static AlarmAndEventDescriptorClass GasboyEventCollectionCompleteDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyEventCollectionCompleteKey);

		private static string GasboyDuplicateTransactionRejected = "Gasboy Duplicate Transaction Rejected";
		public static AlarmAndEventDescriptorClass GasboyDuplicateTransactionRejectedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyDuplicateTransactionRejected);

		private static string GasboyTransactionsReceivedKey = "Gasboy Initiated Transaction Transfer Received";
		public static AlarmAndEventDescriptorClass GasboyTransactionReceivedDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyTransactionsReceivedKey);

		private static string GasboyTransactionsReceivedErrorKey = "Gasboy Initiated Transaction Transfer Error";
		public static AlarmAndEventDescriptorClass GasboyTransactionReceivedErrorDescriptor = new AlarmAndEventDescriptorClass(false,"Gasboy", GasboyTransactionsReceivedErrorKey);

		private static string GasboyFleetDataTransferredKey = "Gasboy Initiated Fleet Data Transfer Complete";
		public static AlarmAndEventDescriptorClass GasboyFleetDataTransferredDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyFleetDataTransferredKey);

		private static string GasboyDepartmentDataTransferredKey = "Gasboy Initiated Department Data Transfer Complete";
		public static AlarmAndEventDescriptorClass GasboyDepartmentDataTransferredDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyDepartmentDataTransferredKey);

		private static string GasboyMeanDataTransferredKey = "Gasboy Initiated Mean/Device Data Transfer Complete";
		public static AlarmAndEventDescriptorClass GasboyMeanDataTransferredDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyMeanDataTransferredKey);

		private static string GasboyDataTransferErrorKey = "Gasboy Initiated Data Transfer Error";
		public static AlarmAndEventDescriptorClass GasboyDataTransferErrorDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyDataTransferErrorKey);

		private static string GasboyEventDataTransferredKey = "Gasboy Initiated Event Data Transfer Complete";
		public static AlarmAndEventDescriptorClass GasboyEventDataTransferredDescriptor = new AlarmAndEventDescriptorClass(false, "Gasboy", GasboyEventDataTransferredKey);

		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents 
		{
			get
			{
				AlarmAndEventDescriptorClass[] descriptors =
					{
						GasboyTestConnectionSuccessDescriptor,
						GasboyTestConnectionErrorDescriptor,
						GasboyManualTransactionDownloadInitiatedDescriptor,
						GasboyManualTransactionDownloadCompleteDescriptor,
						GasboyManualTransactionDownloadErrorDescriptor,
						GasboyDevicePushInitiatedDescriptor,
						GasboyDevicePushCompleteDescriptor, 
						GasboyDevicePushErrorDescriptor,
						GasboyPeriodicTransactionDownloadInitiatedDescriptor,
						GasboyPeriodicTransactionDownloadCompleteDescriptor,
						GasboyPeriodicTransactionDownloadErrorDescriptor,
						GasboyTransactionImportInitiatedDescriptor,
						GasboyTransactionImportCompleteDescriptor,
						GasboyTransactionImportErrorDescriptor,
						GasboyReprocessTransactionInitiatedDescriptor,
						GasboyReprocessTransactionCompleteDescriptor,
						GasboyReprocessTransactionErrorDescriptor,
						GasboyOnlineAuthorizationDeniedDescriptor,
						GasboyOnlineAuthorizationApprovedDescriptor,
						GasboyEventCollectionCompleteDescriptor,
						GasboyDuplicateTransactionRejectedDescriptor,
						GasboyTransactionReceivedDescriptor,
						GasboyTransactionReceivedErrorDescriptor,
						GasboyFleetDataTransferredDescriptor,
						GasboyDepartmentDataTransferredDescriptor,
						GasboyMeanDataTransferredDescriptor,
						GasboyDataTransferErrorDescriptor,
						GasboyEventDataTransferredDescriptor,


							};
					return descriptors;
			}

		}

		public AlarmAndEventLogClass GasboyTestConnectionSuccessEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyTestConnectionSuccessDescriptor);
			alarmAndEventLog.AssociatedData = stationID;
			return (alarmAndEventLog);
		}
		public AlarmAndEventLogClass GasboyTestConnectionErrorEvent(string stationID, string errorMessage)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyTestConnectionErrorDescriptor);
			alarmAndEventLog.AssociatedData = stationID + ": " + errorMessage;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyManualTransactionDownloadInitiatedEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyManualTransactionDownloadInitiatedDescriptor);
			alarmAndEventLog.AssociatedData = stationID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyManualTransactionDownloadCompleteEvent(string stationID, string numTransactions)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyManualTransactionDownloadCompleteDescriptor);
			alarmAndEventLog.AssociatedData = numTransactions + " transactions were downloaded from " + stationID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyManualTransactionDownloadErrorEvent(string stationID, string errorMessage)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyManualTransactionDownloadErrorDescriptor);
			alarmAndEventLog.AssociatedData = stationID + ": " + errorMessage;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyDevicePushInitiatedEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyDevicePushInitiatedDescriptor);
			alarmAndEventLog.AssociatedData = stationID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyDevicePushCompleteEvent(string stationID, string numDevices)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyDevicePushCompleteDescriptor);
			alarmAndEventLog.AssociatedData = numDevices + " devices were pushed to " + stationID;
			return (alarmAndEventLog);
		}
		public AlarmAndEventLogClass GasboyDevicePushErrorEvent(string stationID, string errorMessage)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyDevicePushErrorDescriptor);
			alarmAndEventLog.AssociatedData = stationID + ": " + errorMessage;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyPeriodicTransactionDownloadInitiatedEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyPeriodicTransactionDownloadInitiatedDescriptor);
			alarmAndEventLog.AssociatedData = stationID;
			return (alarmAndEventLog);
		}
		public AlarmAndEventLogClass GasboyPeriodicTransactionDownloadCompleteEvent(string stationID, string numTransactions)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyPeriodicTransactionDownloadCompleteDescriptor);
			alarmAndEventLog.AssociatedData = numTransactions + " transactions were downloaded from " + stationID;
			return (alarmAndEventLog);
		}
		public AlarmAndEventLogClass GasboyPeriodicTransactionDownloadErrorEvent(string stationID, string errorMessage)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyPeriodicTransactionDownloadErrorDescriptor);
			alarmAndEventLog.AssociatedData = stationID + ": " + errorMessage; ;
			return (alarmAndEventLog);
		}
		public AlarmAndEventLogClass GasboyTransactionImportInitiatedEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyTransactionImportInitiatedDescriptor);
			alarmAndEventLog.AssociatedData = stationID; ;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyTransactionImportCompleteEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyTransactionImportCompleteDescriptor);
			alarmAndEventLog.AssociatedData = stationID; ;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyTransactionImportErrorEvent(string stationID, string errorMessage)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyTransactionImportErrorDescriptor);
			alarmAndEventLog.AssociatedData = stationID + ": " + errorMessage; ;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyReproccessTransactionInitiatedEvent(string transactionID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyReprocessTransactionInitiatedDescriptor);
			alarmAndEventLog.AssociatedData = "Attempting to reprocess transaction " + transactionID;
			return (alarmAndEventLog);
		}
		public AlarmAndEventLogClass GasboyReproccessTransactionCompletedEvent(string transactionID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyReprocessTransactionCompleteDescriptor);
			alarmAndEventLog.AssociatedData = "Reprocessed " + transactionID;
			return (alarmAndEventLog);
		}
		public AlarmAndEventLogClass GasboyReproccessTransactionErrorEvent(string transactionID, string message)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyReprocessTransactionErrorDescriptor);
			alarmAndEventLog.AssociatedData = "Reprocessing " + transactionID + " failed. " + message;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyOnlineAuthorizationDeniedEvent(string cardNum, string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyOnlineAuthorizationDeniedDescriptor);
			alarmAndEventLog.AssociatedData = cardNum + " was denied at station " + stationID + ".";
			return (alarmAndEventLog);
		}
		public AlarmAndEventLogClass GasboyOnlineAuthorizationApprovedEvent(string cardNum, string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyOnlineAuthorizationApprovedDescriptor);
			alarmAndEventLog.AssociatedData = cardNum + " was approved at station " + stationID + ".";
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyEventCollectionCompleteEvent(string numEvents, string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyEventCollectionCompleteDescriptor);
			alarmAndEventLog.AssociatedData = numEvents + " events were collected from station " + stationID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyDuplicateTransactionRejectedEvent(string transactionID, string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyDuplicateTransactionRejectedDescriptor);
			alarmAndEventLog.AssociatedData = "Duplicate transaction " + transactionID + " was rejected from station " + stationID + ".";
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyTransactionReceivedEvent(string numTransactions, string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyTransactionReceivedDescriptor);
			alarmAndEventLog.AssociatedData = numTransactions + " transaction(s) were received from " + stationID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyTransactionReceivedErrorEvent(string errorMsg)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyTransactionReceivedErrorDescriptor);
			alarmAndEventLog.AssociatedData = errorMsg;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyFleetDataTransferredEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyFleetDataTransferredDescriptor);
			alarmAndEventLog.AssociatedData = "Fleet Data was transferred to " + stationID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyDepartmentDataTransferredEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyDepartmentDataTransferredDescriptor);
			alarmAndEventLog.AssociatedData = "Department Data was transferred to " + stationID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyMeantDataTransferredEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyMeanDataTransferredDescriptor);
			alarmAndEventLog.AssociatedData = "Mean Data was transferred to " + stationID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyDataTransferErrorEvent(string stationID, string dataRequestType, string errorMsg)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyDataTransferErrorDescriptor);
			alarmAndEventLog.AssociatedData = stationID + " requested " + dataRequestType + " data, but the request failed: " + errorMsg;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass GasboyEventDataTransferredEvent(string stationID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(GasboyEventDataTransferredDescriptor);
			alarmAndEventLog.AssociatedData = "Event Data was transferred from " + stationID;
			return (alarmAndEventLog);
		}


	}
	
}
