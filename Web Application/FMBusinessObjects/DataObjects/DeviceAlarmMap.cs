
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;


	[DataContract]
	[Serializable]
	[KnownType(typeof(DeviceAlarmMap))]
	public class DeviceAlarmMaps : List<DeviceAlarmMap>
	{
		public DeviceAlarmMaps()
		{

		}

		public DeviceAlarmMaps(List<DeviceAlarmMap> list) : base(list)
		{
		}
	}

	[DataContract]
	[Serializable]
	public class DeviceAlarmMap
	{
		[DataContract]
		[Serializable]
		public struct DeviceAlarmMapEntry
		{
			[DataMember]
			public string TestName { get; set; }

			[DataMember]
			public UInt32 BitMask { get; set; }

			[DataMember]
			public Guid AlarmPriority { get; set; }


			public DeviceAlarmMapEntry(string TestName, UInt32 BitMask, Guid AlarmPrioirity)
			{
				this.TestName = TestName;
				this.BitMask = BitMask;
				this.AlarmPriority = AlarmPrioirity;
			}
		}

		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public Guid DeviceAlarmMapGuid { get; set; }

		[DataMember]
		public string NotAlarmText { get; set; }

		[DataMember]
		public Guid AlarmCategory { get; set; }

		[DataMember]
		public Guid NormalUnacknowledgedPriority { get; set; }

		[DataMember]
		[XmlArray("DeviceAlarmMapEntryList")]
		public List<DeviceAlarmMapEntry> DeviceAlarmMapEntryList = new List<DeviceAlarmMapEntry>();
	}
}
