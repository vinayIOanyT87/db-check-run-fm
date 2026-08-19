using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
	[Serializable]
	public enum ChannelType { Virtual, Physical }

	[Serializable]
	public class RTUConfigurationDO
	{

		public string name { get; set; }
		public RTUCPUModuleDO module0 { get; set; }
		public RTUInterfaceModuleDO module1 { get; set; }
		public RTUInterfaceModuleDO module2 { get; set; }
		public RTUInterfaceModuleDO module3 { get; set; }
		public RTUInterfaceModuleDO module4 { get; set; }
		public RTUInterfaceModuleDO module5 { get; set; }
		public RTUInterfaceModuleDO module6 { get; set; }
		public List<Point> points { get; set; }
		public List<DiagnosticView> diagViews { get; set; }
		public UInt32 globalPendingChanges { get; set; }
		public Boolean defaultBlankConfiguration { get; set; }
		public List<AlarmNumberingClass> PointAlarmNumberLookupDictionary { get; set; }
        public List<AlarmNumberingClass> PointRefMapNumberLookupDictionary { get; set; }

        public RTUConfigurationDO()
		{
			this.name = "blank";
			this.module0 = new RTUCPUModuleDO();
			this.module1 = new RTUInterfaceModuleDO();
			this.module2 = new RTUInterfaceModuleDO();
			this.module3 = new RTUInterfaceModuleDO();
			this.module4 = new RTUInterfaceModuleDO();
			this.module5 = new RTUInterfaceModuleDO();
			this.module6 = new RTUInterfaceModuleDO();
			this.points = new List<Point>();
			this.diagViews = new List<DiagnosticView>();
			this.PointAlarmNumberLookupDictionary = new List<AlarmNumberingClass>();
            this.PointRefMapNumberLookupDictionary = new List<AlarmNumberingClass>();
            this.globalPendingChanges = 0;
			this.defaultBlankConfiguration = true;
		}
	}
}
