namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(SiteSelectInfo))]
	public class SiteSelectList : List<SiteSelectInfo>
	{
	}

	[Serializable]
	[DataContract]
	public class SiteSelectInfo
	{
		[DataMember]
		public Guid SiteGuid { get; set; }

		[DataMember]
		public bool IsSiteGroup { get; set; }

		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public string Number { get; set; }
	}
}
