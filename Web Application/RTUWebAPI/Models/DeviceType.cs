using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
	[Serializable]
	public class DeviceType
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string DeviceTypeValue { get; set; }

		public List<string> AvailableCommands { get; set; }
	}

	public class AlarmNumberingClass
	{
		public string pointName { get; set; }
		public string VariableName { get; set; }
		public string AlarmNumber { get; set; }
		//public AlarmNumberingClass(string PointName, string VariableName, string AlarmNumber)
		//{
			//this.pointName = PointName;
			//this.VariableName = VariableName;
			//this.AlarmNumber = AlarmNumber;
		//}
	}

}
