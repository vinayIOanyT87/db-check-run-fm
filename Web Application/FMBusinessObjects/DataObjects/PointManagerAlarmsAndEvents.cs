
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;


	public class PointManagerAlarmsAndEvents : BaseObjectClass, IAlarmAndEventDiscovery
	{
		private const string EnterpriseVisibilityPushInitiatedKey = "Enterprise Visibility Push Initiated";
		private const string EnterpriseVisibilityPushCompleteKey = "Enterprise Visibility Push Complete";


		// Alarm and Events
		public static AlarmAndEventDescriptorClass EnterpriseVisibilityPushInitiatedEventDescriptor = new AlarmAndEventDescriptorClass(false, PointManagerKey, EnterpriseVisibilityPushInitiatedKey);
		public static AlarmAndEventDescriptorClass EnterpriseVisibilityPushCompleteEventDescriptor = new AlarmAndEventDescriptorClass(false, PointManagerKey, EnterpriseVisibilityPushCompleteKey);

		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] Descriptors = {	EnterpriseVisibilityPushInitiatedEventDescriptor,
																				EnterpriseVisibilityPushCompleteEventDescriptor,
																			};
				return Descriptors;
			}
		}

		public AlarmAndEventLogClass EnterpriseVisibilityPushInitiatedEvent()
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(EnterpriseVisibilityPushInitiatedEventDescriptor);
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass EnterpriseVisibilityPushCompleteEvent(string result)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(EnterpriseVisibilityPushCompleteEventDescriptor);
			AlarmAndEventLog.AssociatedData = result;
			return AlarmAndEventLog;
		}
	}
}
