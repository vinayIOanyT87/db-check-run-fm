using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
    public class EnterpriseSynchronizationEvents : IAlarmAndEventDiscovery
    {
        static string SynchronizationDisabledKey = "Synchronization Currently Disabled";
        public static AlarmAndEventDescriptorClass SynchronizationDisabledDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, SynchronizationDisabledKey);

        static string ManualSynchronizationInitiatedKey = "Manual Synchronization Initiated";
        public static AlarmAndEventDescriptorClass ManualSynchronizationInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, ManualSynchronizationInitiatedKey);

        static string ManualSynchronizationCompleteKey = "Manual Synchronization Complete";
        public static AlarmAndEventDescriptorClass ManualSynchronizationCompleteDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, ManualSynchronizationCompleteKey);

		static string InitialSynchronizationAutoResumeKey = "Initial Synchronization Auto Resume Detected";
		public static AlarmAndEventDescriptorClass InitialSynchronizationAutoResumeDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, InitialSynchronizationAutoResumeKey);

		static string PeriodicSynchronizationInitiatedKey = "Periodic Synchronization Initiated";
        public static AlarmAndEventDescriptorClass PeriodicSynchronizationInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, PeriodicSynchronizationInitiatedKey);

        static string PeriodicSynchronizationCompleteKey = "Periodic Synchronization Complete";
        public static AlarmAndEventDescriptorClass PeriodicSynchronizationCompleteDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, PeriodicSynchronizationCompleteKey);

        static string StopSynchronizationInitiatedKey = "Stop Synchronization Initiated";
        public static AlarmAndEventDescriptorClass StopSynchronizationInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, StopSynchronizationInitiatedKey);

        static string StopSynchronizationCompleteKey = "Stop Synchronization Complete";
        public static AlarmAndEventDescriptorClass StopSynchronizationCompleteDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, StopSynchronizationCompleteKey);

        static string SynchronizationConfigurationErrorKey = "Synchronization Configuration Error";
        public static AlarmAndEventDescriptorClass SynchronizationConfigurationErrorDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, SynchronizationConfigurationErrorKey);

        static string SynchronizationErrorEncounteredKey = "Synchronization Error Encountered";
        public static AlarmAndEventDescriptorClass SynchronizationErrorEncounteredDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, SynchronizationErrorEncounteredKey);

        static string SynchronizationConflictDetectedKey = "Synchronization Conflict(s) Detected";
        public static AlarmAndEventDescriptorClass SynchronizationConflictDetectedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, SynchronizationConflictDetectedKey);

        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] descriptors ={	SynchronizationDisabledDescriptor,
                                                                ManualSynchronizationInitiatedDescriptor,
                                                                ManualSynchronizationCompleteDescriptor,
																InitialSynchronizationAutoResumeDescriptor,
                                                                PeriodicSynchronizationInitiatedDescriptor,
                                                                PeriodicSynchronizationCompleteDescriptor,
                                                                StopSynchronizationInitiatedDescriptor,
                                                                StopSynchronizationCompleteDescriptor,
                                                                SynchronizationConfigurationErrorDescriptor,
                                                                SynchronizationErrorEncounteredDescriptor,
                                                                SynchronizationConflictDetectedDescriptor
                                                            };

                return descriptors;
            }
        }

        public AlarmAndEventLogClass SynchronizationDisabledEvent(string userID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(SynchronizationDisabledDescriptor);
            alarmAndEventLog.AssociatedData = userID;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass ManualSynchronizationInitiatedEvent(string userID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(ManualSynchronizationInitiatedDescriptor);
			alarmAndEventLog.AssociatedData = userID;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass ManualSynchronizationCompleteEvent(string userID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(ManualSynchronizationCompleteDescriptor);
            alarmAndEventLog.AssociatedData = userID;
            return (alarmAndEventLog);
        }

		public AlarmAndEventLogClass InitialSynchronizationAutoResumeEvent(string userID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(InitialSynchronizationAutoResumeDescriptor);
			alarmAndEventLog.AssociatedData = userID;
			return (alarmAndEventLog);
		}

		public AlarmAndEventLogClass PeriodicSynchronizationInitiatedEvent(string userID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(PeriodicSynchronizationInitiatedDescriptor);
            alarmAndEventLog.AssociatedData = userID;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass PeriodicSynchronizationCompleteEvent(string userID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(PeriodicSynchronizationCompleteDescriptor);
            alarmAndEventLog.AssociatedData = userID;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass StopSynchronizationInitiatedEvent(string userID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(StopSynchronizationInitiatedDescriptor);
            alarmAndEventLog.AssociatedData = userID;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass StopSynchronizationCompleteEvent(string userID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(StopSynchronizationCompleteDescriptor);
            alarmAndEventLog.AssociatedData = userID;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass SynchronizationConfigurationErrorEvent(string errorMessage)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(SynchronizationConfigurationErrorDescriptor);
            alarmAndEventLog.AssociatedData = errorMessage;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass SynchronizationErrorEncounteredEvent(string errorMessage)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(SynchronizationErrorEncounteredDescriptor);
            alarmAndEventLog.AssociatedData = errorMessage;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass SynchronizationConflictDetectedEvent(string conflictDetails)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(SynchronizationConflictDetectedDescriptor);
            alarmAndEventLog.AssociatedData = conflictDetails;
            return (alarmAndEventLog);
        }
    }
}
