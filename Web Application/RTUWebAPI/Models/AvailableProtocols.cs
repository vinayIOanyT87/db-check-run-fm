using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
    [Serializable]
    public class AvailableProtocols
    {
        public string Name { get; set; }
        public Dictionary<UInt32,Parameter> protocolConfiguration { get; set; }

        public List<DeviceType> AvailableDeviceTypes { get; set; }

		public List<AlarmNumberingClass> PointAlarmNumberLookupDictionary { get; set; }

        public List<AlarmNumberingClass> PointRefMapNumberLookupDictionary { get; set; }

    }
}
