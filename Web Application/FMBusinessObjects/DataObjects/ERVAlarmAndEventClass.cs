using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    using BusinessInterfaces;
    using ChannelFactories;
    using FMBusinessObjects.Constants;
    using UtilityObjects;


    public class ERVAlarmAndEventClass : IAlarmAndEventDiscovery
    {
        public static string ERVKey = "EntityRecordVersioning";
        public static AlarmAndEventDescriptorClass ERVEventDescriptor = new AlarmAndEventDescriptorClass(false, ERVKey, "Entity Record Versioning Event");
        public static AlarmAndEventDescriptorClass GlobalFieldsInhibitTimeThresholdEventDescriptor = new AlarmAndEventDescriptorClass(false, ERVKey, "Global Fields Inhibit Time Threshold Event");

        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] descriptors =
                    {
                        ERVEventDescriptor,
                        GlobalFieldsInhibitTimeThresholdEventDescriptor
                    };
                return descriptors;
            }
        }


        public AlarmAndEventLogClass ERVAlarmAndEvent(AlarmAndEventDescriptorClass ervAlarmAndEventDescriptorClass, string contentID)
        // ReSharper restore InconsistentNaming
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ervAlarmAndEventDescriptorClass);
            if (contentID.Length > 120)
            {
                alarmAndEventLog.AssociatedData = contentID.Substring(0, 120); // ID only up to 120 characters
            }
            else
            {
                alarmAndEventLog.AssociatedData = contentID;
            }

            return alarmAndEventLog;
        }

    }
}



