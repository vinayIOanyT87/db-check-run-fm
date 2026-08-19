namespace FMBusinessObjects.DataObjects
{
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;
	using System.IO;
	using System.Xml;
	using System.Web.Script.Serialization;

	using FMBusinessObjects.UtilityObjects;

	[KnownType(typeof(string))]
	[KnownType(typeof(double))]
	[KnownType(typeof(bool))]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(TimeSpan))]
	[KnownType(typeof(PointCommandStatusListReference))]
	[KnownType(typeof(DeviceAlarmMapReference))]
	[KnownType(typeof(CodedVariables.TankStatuses))]
	[KnownType(typeof(CodedVariables.TankCommands))]
	[KnownType(typeof(CodedVariables.TransferModes))]
	[KnownType(typeof(CodedVariables.TankTransferMode))]
	[KnownType(typeof(CodedVariables.VolumeTransferMode))]
	[KnownType(typeof(CodedVariables.TransferStatuses))]
	[KnownType(typeof(CodedVariables.MovementCommand))]
	[KnownType(typeof(CodedVariables.MovementStatus))]
	[KnownType(typeof(CodedVariables.TransferDirection))]
   [KnownType(typeof(CodedVariables.StrapTableSelect))]
	[KnownType(typeof(CodedVariables.Reset))]
	[KnownType(typeof(CodedVariables.NodeTransferMode))]
	[KnownType(typeof(CodedVariables.NodeTransferStatus))]
	[KnownType(typeof(VolumeTransferModuleSettings))]
	[KnownType(typeof(MovementModuleSettings))]
	[KnownType(typeof(MovementData))]
	[KnownType(typeof(LeakDetectionSettings))]
	[KnownType(typeof(CodedVariables.MovementType))]
	[DataContract]
	[Serializable]
	public class ModuleTag
	{
		[DataMember]
		public string ParameterName { get; set; }

		[DataMember]
		public string TagID { get; set; }

		[DataMember]
		public EngineeringUnitType UnitType { get; set; }

		[DataMember]
		public EngineeringUnit Units { get; set; }

		[DataMember]
		public EngineeringUnit ServerUnits { get; set; }

		protected Type _valueType;
		protected object _value;

		[ScriptIgnore]
		[XmlIgnore]
		public Type ValueType
		{
			get
			{
				return this._valueType;
			}
			set
			{
				this._valueType = value;
			}
		}

		[DataMember]
		public string DataType
		{
			get
			{
				return this._valueType.ToString();
			}
			set
			{
				this._valueType = Type.GetType(value);
			}
		}


		[ScriptIgnore]
		[XmlIgnore]
		[DataMember]
		public object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
				if (value != null)
				{
					this._valueType = value.GetType();
				}
			}
		}

		public string ValueXml
		{
			get
			{
				var retValue = "";
				object value = this._value;
				if (value == null)
				{
					retValue = null;
				}
				else
				{
					XmlSerializer xmlserializer;
					if (value.GetType() == typeof(DateTimeOffset))
					{
						xmlserializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("DateTimeOffset"));
						value = XmlConvert.ToString((DateTimeOffset)value);
					}
					else if (value.GetType() == typeof(TimeSpan))
					{
						xmlserializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("TimeSpan"));
						value = XmlConvert.ToString((TimeSpan)value);
					}

					else
					{
						xmlserializer = CachingXmlSerializerFactory.Create(value.GetType());
					}

					var stringWriter = new StringWriter();
					var emptyNameSpaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
					// explicitly remove the xml declaration
					var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
					using (var writer = XmlWriter.Create(stringWriter, settings))
					{
						xmlserializer.Serialize(writer, value, emptyNameSpaces);
						retValue = stringWriter.ToString();
					}
				}

				return retValue;
			}
			set
			{
				if (value == null)
				{
					this._value = null;
					return;
				}

				XmlSerializer serializer;

				if (this._valueType == typeof(DateTimeOffset))
				{
					serializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("DateTimeOffset"));
				}
				else if (this._valueType == typeof(TimeSpan))
				{
					serializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("TimeSpan"));
				}
				else
				{
					serializer = CachingXmlSerializerFactory.Create(this._valueType);
				}
				var tempReader = new StringReader(value);
				this._value = (object)serializer.Deserialize(tempReader);

				if (this._valueType == typeof(DateTimeOffset))
				{
					this._value = XmlConvert.ToDateTimeOffset(this._value as string);
				}
				else if (this._valueType == typeof(TimeSpan))
				{
					this._value = XmlConvert.ToTimeSpan(this._value as string);
				}
			}
		}

		public string ValueJson
		{
			get
			{
				var retValue = "";
				if (this._value == null)
				{
					retValue = null;
				}
				else
				{
					var serializer = new JavaScriptSerializer();
					retValue = serializer.Serialize(this._value);
				}


				return retValue;
			}
			set
			{
				if (value == null)
				{
					this._value = null;
					return;
				}

				var serializer = new JavaScriptSerializer();
				this._value = (object)serializer.Deserialize(value, this._valueType);

			}
		}


		[DataMember]
		public byte DecimalPlaces { get; set; }

		[DataMember]
		public double Maximum { get; set; }

		[DataMember]
		public double Minimum { get; set; }

		[DataMember]
		public bool ApplyPointTemplateEngineeringUnits { get; set; }

		[DataMember]
		public bool ApplyPointTemplateDecimalPlaces { get; set; }

		[DataMember]
		public bool ApplyPointTemplateMaximum { get; set; }

		[DataMember]
		public bool ApplyPointTemplateMinimum { get; set; }

		[DataMember]
		public PointTemplateTag.PointTagInputOutputType InputOutputType { get; set; }

		[DataMember]
		public bool Input { get; set; }

		[DataMember]
		public bool InhibitInputOutputTypeConfiguration { get; set; }

		[DataMember]
		public bool InhibitOverride { get; set; }

		[DataMember]
		public Guid WellKnownIdentityGuid { get; set; }

	}

	[DataContract]
	[Serializable]
	public class ModuleSetting
	{
		[DataMember]
		public string PropertyName { get; set; }

		[DataMember]
		public string SettingName { get; set; }

		[DataMember]
		public string DataType { get; set; }
	}

	[DataContract]
	[Serializable]
	public class ModuleReference
	{
		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public string Property { get; set; }
	}


	[DataContract(Namespace = "")]
	[Serializable]
	[KnownType(typeof(ModuleTag))]
	[KnownType(typeof(ModuleSetting))]
	[KnownType(typeof(ModuleReference))]
	public class ModuleData
	{
		[DataMember]
		public ModuleTag [] ModuleTags { get; set; }

		[DataMember]
		public ModuleSetting [] ModuleSettings { get; set; }

		[DataMember]
		public ModuleReference [] ModuleReferences { get; set; }

		[DataMember]
		public Guid [] Dependencies { get; set; }

		[DataMember]
		public bool MultipleInstances { get; set; }

		[DataMember]
		public bool Calculator { get; set; }

		public ModuleData()
		{
			ModuleTags = new ModuleTag[0];
			ModuleSettings = new ModuleSetting[0];
			Dependencies = new Guid[0];
			ModuleReferences = new ModuleReference[0];
			MultipleInstances = false;
			Calculator = false;
		}
	}
}
