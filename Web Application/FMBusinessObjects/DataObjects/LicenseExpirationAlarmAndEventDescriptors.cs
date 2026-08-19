using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    public class LicenseExpirationAlarmAndEventDescriptors: IAlarmAndEventDiscovery
    {
        public const int NUMBER_DAYS_FOR_DAILY_REMINDER = 7;
        public const string ID30DayWarningAck = "30 day license warning acknowledged";
        public const string ID60DayWarningAck = "60 day license warning acknowledged";
        public const string ID90DayWarningAck = "90 day license warning acknowledged";

        public const string IDxDaysBeforeLicenseExpire = "Your FuelsManager license will expire in {0} day{1}";
        public const string ID30DayBeforeLicenseExpire = "Your FuelsManager license will expire in 30 days or less";
        public const string ID60DayBeforeLicenseExpire = "Your FuelsManager license will expire in 60 days or less";
        public const string ID90DayBeforeLicenseExpire = "Your FuelsManager license will expire in 90 days or less";

        public static AlarmAndEventDescriptorClass alarmEventDescriptorFor90DayWarningAck = new AlarmAndEventDescriptorClass(false, BaseObjectClass.License, ID90DayWarningAck);
        public static AlarmAndEventDescriptorClass alarmEventDescriptorFor60DayWarningAck = new AlarmAndEventDescriptorClass(false, BaseObjectClass.License, ID60DayWarningAck);
        public static AlarmAndEventDescriptorClass alarmEventDescriptorFor30DayWarningAck = new AlarmAndEventDescriptorClass(false, BaseObjectClass.License, ID30DayWarningAck);
        public static AlarmAndEventDescriptorClass alarmEventDescriptorFor90DayBeforeLicenseExpire = new AlarmAndEventDescriptorClass(true, BaseObjectClass.License, ID90DayBeforeLicenseExpire);
        public static AlarmAndEventDescriptorClass alarmEventDescriptorFor60DayBeforeLicenseExpire = new AlarmAndEventDescriptorClass(true, BaseObjectClass.License, ID60DayBeforeLicenseExpire);
        public static AlarmAndEventDescriptorClass alarmEventDescriptorFor30DayBeforeLicenseExpire = new AlarmAndEventDescriptorClass(true, BaseObjectClass.License, ID30DayBeforeLicenseExpire);

       
        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                List<AlarmAndEventDescriptorClass> descriptors = new List<AlarmAndEventDescriptorClass>
                        {
                            alarmEventDescriptorFor90DayWarningAck,
                            alarmEventDescriptorFor60DayWarningAck,
                            alarmEventDescriptorFor30DayWarningAck,
                            alarmEventDescriptorFor90DayBeforeLicenseExpire,
                            alarmEventDescriptorFor60DayBeforeLicenseExpire,
                            alarmEventDescriptorFor30DayBeforeLicenseExpire,

                         };
                for(int i= NUMBER_DAYS_FOR_DAILY_REMINDER; i >= 1; i--)
                {
                    descriptors.Add(new AlarmAndEventDescriptorClass(true, BaseObjectClass.License, string.Format(IDxDaysBeforeLicenseExpire,i,i>1?"s":string.Empty)));
                }
                return descriptors.ToArray();
            }

        }


        public static string[] AlarmEventIds
        {
            get
            {
                List<string> eventIds = new List<string>
                {
                    ID30DayBeforeLicenseExpire,
                    ID60DayBeforeLicenseExpire,
                    ID90DayBeforeLicenseExpire
                };
                for (int i = NUMBER_DAYS_FOR_DAILY_REMINDER; i >= 1; i--)
                {
                    eventIds.Add(string.Format(IDxDaysBeforeLicenseExpire, i, i > 1 ? "s" : string.Empty));
                }
                return eventIds.ToArray();
            }
        }
    }


}
