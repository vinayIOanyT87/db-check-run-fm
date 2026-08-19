using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
	[Serializable]
	public class Parameter
	{
		public ConfigurationClass configClass { get; set; }
		public string parameter { get; set; }
		public string description { get; set; }
		public string dataType { get; set; }
		public string displayFormat { get; set; }
		public float? minimumValue { get; set; }
		public float? maximumValue { get; set; }
		public string value { get; set; }
		public UInt32 status { get; set; }
		public DateTime? serverTimeStamp { get; set; }
		public string pendingValue { get; set; }
		public UInt32 pendingStatus { get; set; }
		public DateTime? pendingServerTimeStamp { get; set; }
		public string availableCommands { get; set; }
		public UInt32 identifier { get; set; }
		public UInt32 opcstartNodeID { get; set; }
		public string tab { get; set; }
		public string section { get; set; }
		public UInt32 parameterIsVisible { get; set; }

		public UInt32 availableCommandsOutputMatches { get; set; }

		public string variableAlarmNumber { get; set; }
		public string datatypeLength { get; set; }

		public Parameter(ConfigurationClass configClass,
									string parameter,
									string description,
									string dataType,
									string displayFormat = "",
									float? minimumValue = null,
									float? maximumValue = null,
									string value = "",
									UInt32 status = 0x80000000,
									DateTime? serverTimeStamp = null,
									string pendingValue = "",
									UInt32 pendingStatus = 0x80000000,
									DateTime? pendingServerTimeStamp = null,
									string availableCommands = "",
									UInt32 opcStartNodeId = 0xFFFFFFFF,
									UInt32 identifier = 0xFFFFFFFF,
									string tab = "",
									string section = "",
									UInt32 parameterIsVisible = 1,
									UInt32 availableCommandsOutputMatches = 0,
									string variableAlarmNumber = "0",
									string datatypeLength = "")
		{
			this.configClass = configClass;
			this.parameter = parameter;
			this.description = description;
			this.dataType = dataType;
			this.displayFormat = displayFormat;
			this.minimumValue = minimumValue;
			this.maximumValue = maximumValue;
			this.value = value;
			this.status = status;
			this.serverTimeStamp = serverTimeStamp.HasValue ? serverTimeStamp.Value : DateTime.UtcNow;
			this.pendingValue = pendingValue;
			this.pendingStatus = pendingStatus;
			this.pendingServerTimeStamp = pendingServerTimeStamp.HasValue ? pendingServerTimeStamp.Value : DateTime.UtcNow;
			this.availableCommands = availableCommands;
			this.opcstartNodeID = opcStartNodeId;
			this.identifier = identifier;
			this.tab = tab;
			this.section = section;
			this.parameterIsVisible = parameterIsVisible;
			this.availableCommandsOutputMatches = availableCommandsOutputMatches;
			this.variableAlarmNumber = variableAlarmNumber;
			this.datatypeLength = datatypeLength;
		}
	}
}
