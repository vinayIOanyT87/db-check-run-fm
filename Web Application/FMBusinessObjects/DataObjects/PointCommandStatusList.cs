namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	[DataContract]
	[Serializable]
	public class PointCommandStatusList
	{
		[DataContract]
		[Serializable]
		public struct CommandStatusElement
		{
			[DataMember]
			public string Key;

			[DataMember]
			public int Value;

			public CommandStatusElement(string Key, int Value)
			{
				this.Key = Key;
				this.Value = Value;
			}
		}

		[DataMember]
		public Guid CommandStatusListGuid;

		[DataMember]
		public string ID;

		[DataMember]
		[XmlArray("CommandStatusList")]
		public List<CommandStatusElement> CommandStatusList = new List<CommandStatusElement>();

		public PointCommandStatusList()
		{
		}
	}
}
