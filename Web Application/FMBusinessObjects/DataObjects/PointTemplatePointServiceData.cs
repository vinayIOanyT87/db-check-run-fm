namespace FMBusinessObjects.DataObjects
{
	using Attributes;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Threading.Tasks;
	using System.Xml.Serialization;

	using FMBusinessObjects.UtilityObjects;

	[DataContract]
	[Serializable]
	public class PointTemplatePointServiceData : BaseDataObject
	{
		[FMPersistedField(ReadOnly = true)]
		public Guid PointTemplateGuid
		{
			get
			{
				return this.IdentityGuid;
			}

			set
			{
				this.IdentityGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string PointLogicScript { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public int Version { get; set; }

		[DataMember]
		public Dictionary<Guid, ModuleToPointTemplateMap> ModuleInstances { get; set; }


		[DataMember]
		public Dictionary<Guid, string> ModuleLogicScript;

		[DataMember]
		public PointCommandStatus PointCommandStatus { get; set; }

		[FMPersistedField("PointCommandStatus")]
		public string PointCommandStatusXml
		{
			get
			{
				var retValue = "";
				if (this.PointCommandStatus == null)
				{
					retValue = null;
				}
				else
				{

					var serializer = CachingXmlSerializerFactory.Create(typeof(PointCommandStatus));
					var stringWriter = new StringWriter();
					serializer.Serialize(stringWriter, this.PointCommandStatus);
					retValue = stringWriter.ToString();
				}

				return retValue;

			}

			set
			{
				if (value == null)
				{
					this.PointCommandStatus = null;
					return;
				}

				var serializer = CachingXmlSerializerFactory.Create(typeof(PointCommandStatus));
				var stringReader = new StringReader(value);
				this.PointCommandStatus = (PointCommandStatus)serializer.Deserialize(stringReader);

			}

		}

		public PointTemplatePointServiceData()
		{
			this.ModuleLogicScript = new Dictionary<Guid, string>();
		}
	}
}