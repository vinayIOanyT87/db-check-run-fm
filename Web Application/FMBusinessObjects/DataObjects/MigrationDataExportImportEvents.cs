// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationDataExportImportEvents.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MigrationDataExportImportEvents type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    /// <summary>
    /// The migration data export import events.
    /// </summary>
    public class MigrationDataExportImportEvents : IAlarmAndEventDiscovery
    {
        /// <summary>
        /// The migration export guid mapping key.
        /// </summary>
        private const string MigrationExportGuidMappingKey = "Migration Export Guid Mapping Data";

        /// <summary>
        /// The migration export guid mapping descriptor.
        /// </summary>
        private static AlarmAndEventDescriptorClass MigrationExportGuidMappingDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, MigrationExportGuidMappingKey);

        /// <summary>
        /// The migration export guid mapping error key.
        /// </summary>
        private const string MigrationExportGuidMappingErrorKey = "Migration Export Guid Mapping Error Encountered";

        /// <summary>
        /// The migration export guid mapping error descriptor.
        /// </summary>
        private static AlarmAndEventDescriptorClass MigrationExportGuidMappingErrorDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.DataSynchronization, MigrationExportGuidMappingErrorKey);

        /// <summary>
        /// Gets the alarm and events.
        /// </summary>
        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] Descriptors =
                    {
                        MigrationExportGuidMappingDescriptor,
                        MigrationExportGuidMappingErrorDescriptor
                    };

                return Descriptors;
            }
        }

        /// <summary>
        /// The migration export GUID mapping event.
        /// </summary>
        /// <param name="siteId">
        /// The site id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/>.
        /// </returns>
        public AlarmAndEventLogClass MigrationExportGuidMappingEvent(string siteId, string userId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(MigrationExportGuidMappingDescriptor);
            alarmAndEventLog.AssociatedData = string.Format("Site Id: {0}, User Id: {1}", siteId, userId);
            return alarmAndEventLog;
        }

        /// <summary>
        /// The migration export GUID mapping error event.
        /// </summary>
        /// <param name="siteId">
        /// The site id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/>.
        /// </returns>
        public AlarmAndEventLogClass MigrationExportGuidMappingErrorEvent(string siteId, string userId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(MigrationExportGuidMappingErrorDescriptor);
            alarmAndEventLog.AssociatedData = string.Format("Site Id: {0}, User Id: {1}", siteId, userId);
            return alarmAndEventLog;
        }
    }
}
