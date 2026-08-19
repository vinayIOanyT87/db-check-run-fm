using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.AlarmAndEvents
{
    using FMBusinessObjects.DataObjects;

    public class GasboyCommunicationEvents : IAlarmAndEventDiscovery
    {
        private const string GasboyDownloadDisabledKey = "Gasboy Download Currently Disabled";
        public static AlarmAndEventDescriptorClass GasboyDownloadDisabledDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, GasboyDownloadDisabledKey);

        private const string ManualGasboyDownloadInitiatedKey = "Manual Gasboy Download Initiated";
        public static AlarmAndEventDescriptorClass ManualGasboyDownloadInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, ManualGasboyDownloadInitiatedKey);

        private const string ManualGasboyDownloadCompleteKey = "Manual Gasboy Download Complete";
        public static AlarmAndEventDescriptorClass ManualGasboyDownloadCompleteDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, ManualGasboyDownloadCompleteKey);

        private const string PeriodicGasboyDownloadInitiatedKey = "Periodic Gasboy Download Initiated";
        public static AlarmAndEventDescriptorClass PeriodicGasboyDownloadInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, PeriodicGasboyDownloadInitiatedKey);

        private const string PeriodicGasboyDownloadCompleteKey = "Periodic Gasboy Download Complete";
        public static AlarmAndEventDescriptorClass PeriodicGasboyDownloadCompleteDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, PeriodicGasboyDownloadCompleteKey);

        private const string StopGasboyDownloadInitiatedKey = "Stop Gasboy Download Initiated";
        public static AlarmAndEventDescriptorClass StopGasboyDownloadInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, StopGasboyDownloadInitiatedKey);

        private const string StopGasboyDownloadCompleteKey = "Stop Gasboy Download Complete";
        public static AlarmAndEventDescriptorClass StopGasboyDownloadCompleteDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, StopGasboyDownloadCompleteKey);

        private const string GasboyDownloadErrorEncounteredKey = "Gasboy Download Error Encountered";
        public static AlarmAndEventDescriptorClass GasboyDownloadErrorEncounteredDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, GasboyDownloadErrorEncounteredKey);

        private const string GasboyCommunicationErrorDetectedKey = "Gasboy Communication Error Detected";
        public static AlarmAndEventDescriptorClass GasboyCommunicationErrorDetectedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, GasboyCommunicationErrorDetectedKey);

        private const string GasboyConfigurationErrorKey = "Gasboy Configuration Error";
        public static AlarmAndEventDescriptorClass GasboyConfigurationErrorDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, GasboyConfigurationErrorKey);

        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] descriptors ={	GasboyDownloadDisabledDescriptor,
                                                                ManualGasboyDownloadInitiatedDescriptor,
                                                                ManualGasboyDownloadCompleteDescriptor,
                                                                PeriodicGasboyDownloadInitiatedDescriptor,
                                                                PeriodicGasboyDownloadCompleteDescriptor,
                                                                StopGasboyDownloadInitiatedDescriptor,
                                                                StopGasboyDownloadCompleteDescriptor,
                                                                GasboyDownloadErrorEncounteredDescriptor,
                                                                GasboyCommunicationErrorDetectedDescriptor,
                                                                GasboyConfigurationErrorDescriptor
                                                            };

                return descriptors;
            }
        }

        public AlarmAndEventLogClass GasboyDownloadDisabledEvent(string userID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(GasboyDownloadDisabledDescriptor)
                                       {
                                           AssociatedData = userID
                                       };
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass ManualGasboyDownloadInitiatedEvent(string userID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ManualGasboyDownloadInitiatedDescriptor)
                                       {
                                           AssociatedData
                                               = userID
                                       };
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass ManualGasboyDownloadCompleteEvent(string userID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ManualGasboyDownloadCompleteDescriptor)
                                       {
                                           AssociatedData
                                               = userID
                                       };
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass PeriodicGasboyDownloadInitiatedEvent(string userID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(PeriodicGasboyDownloadInitiatedDescriptor)
                                       {
                                           AssociatedData
                                               = userID
                                       };
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass PeriodicGasboyDownloadCompleteEvent(string userID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(PeriodicGasboyDownloadCompleteDescriptor)
                                       {
                                           AssociatedData
                                               = userID
                                       };
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass StopGasboyDownloadInitiatedEvent(string userID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(StopGasboyDownloadInitiatedDescriptor)
                                       {
                                           AssociatedData =
                                               userID
                                       };
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass StopGasboyDownloadCompleteEvent(string userID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(StopGasboyDownloadCompleteDescriptor)
                                       {
                                           AssociatedData =
                                               userID
                                       };
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass GasboyDownloadErrorEncounteredEvent(string errorMessage)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(GasboyDownloadErrorEncounteredDescriptor)
                                       {
                                           AssociatedData
                                               =
                                               errorMessage
                                       };
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass GasboyCommunicationErrorDetectedEvent(string errorMessage)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(GasboyCommunicationErrorDetectedDescriptor)
                                       {
                                           AssociatedData
                                               =
                                               errorMessage
                                       };
            return (alarmAndEventLog);
        }
        public AlarmAndEventLogClass GasboyConfigurationErrorEvent(string errorMessage)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(GasboyConfigurationErrorDescriptor)
                                       {
                                           AssociatedData =
                                               errorMessage
                                       };
            return (alarmAndEventLog);
        }
    }
}
