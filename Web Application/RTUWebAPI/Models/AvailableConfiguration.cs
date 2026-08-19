using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
	[Serializable]
	public class AvailableConfiguration
	{

		public List<AvailableModules> modules { get; set; }
		public List<AvailableProtocols> protocols { get; set; }
		public List<AvailablePoints> points { get; set; }
		public List<AlarmNumberingClass> PointAlarmNumberLookupDictionary { get; set; }
        public List<AlarmNumberingClass> PointRefMapNumberLookupDictionary { get; set; }


        public AvailableConfiguration()
		{
			this.modules = new List<AvailableModules>();
			this.protocols = new List<AvailableProtocols>();
			this.points = new List<AvailablePoints>();
			this.PointAlarmNumberLookupDictionary = new List<AlarmNumberingClass>();
            this.PointRefMapNumberLookupDictionary = new List<AlarmNumberingClass>();
        }

	}
}
