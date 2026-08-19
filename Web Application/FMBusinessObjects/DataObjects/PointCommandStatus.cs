namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	[DataContract]
	[Serializable]
	[KnownType(typeof(PointCommandStatusList))]
	public class PointCommandStatus
	{
		[DataMember]
		public List<PointCommandStatusList> CommandStatusLists = new List<PointCommandStatusList>();

		public PointCommandStatus()
		{
		}
	}
}
