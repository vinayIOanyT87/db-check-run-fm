using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
    public class FCEEEvents : IAlarmAndEventDiscovery
    {
        static string FCEEConfigurationOverwrittenKey = "FCEE Device Configuration Overwritten";
        public static AlarmAndEventDescriptorClass FCEEConfigurationOverwrittenDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.FCEE, FCEEConfigurationOverwrittenKey);

        static string FCEEMappingCollusionKey = "FCEE Mapping Collusion";
        public static AlarmAndEventDescriptorClass FCEEMappingCollusionKeyDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.FCEE, FCEEMappingCollusionKey);

        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] descriptors ={  FCEEConfigurationOverwrittenDescriptor,
                                                               FCEEMappingCollusionKeyDescriptor

                                                            };

                return descriptors;
            }
        }

        public AlarmAndEventLogClass FCEEConfigurationOverwrittenEvent(string imei)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(FCEEConfigurationOverwrittenDescriptor);
            alarmAndEventLog.AssociatedData = imei;
            return (alarmAndEventLog);
        }

        public AlarmAndEventLogClass mappingCollusionEvent(string pointAndTag)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(FCEEMappingCollusionKeyDescriptor);
            alarmAndEventLog.AssociatedData = pointAndTag;
            return (alarmAndEventLog);
        }
    }
}
