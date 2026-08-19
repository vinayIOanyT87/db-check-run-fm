
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class TagToModule
	{
		[DataMember]
		public Guid TagGuid { get; set; }

		[DataMember]
		public string ModuleParameter { get; set; }
	}

	[DataContract]
	[Serializable]
	public class PropertyToModule
	{
		[DataMember]
		public Guid PropertyGuid { get; set; }

		[DataMember]
		public string PropertyName { get; set; }
	}


	[DataContract]
	[Serializable]
	[KnownType(typeof(TagToModule))]
	[KnownType(typeof(PropertyToModule))]

	public class ModuleToPointTemplateData
	{
		[DataMember]
		public TagToModule[] TagToModules;

		[DataMember]
		public PropertyToModule[] PropertyToModules;
	}
}
