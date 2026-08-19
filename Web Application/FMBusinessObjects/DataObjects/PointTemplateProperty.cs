namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Xml.Serialization;
	using System.IO;
	using System.Web.Script.Serialization;
	using System.Xml;

	using FMBusinessObjects.UtilityObjects;
	using FMBusinessObjects.DataObjects.CodedVariables;

	[KnownType(typeof(StrapTable))]
	[KnownType(typeof(Vessel))]
	[KnownType(typeof(VcfModuleSettings))]
	[KnownType(typeof(QuantityModuleSettings))]
	[KnownType(typeof(TankCommandModuleSettings))]
	[KnownType(typeof(RateModuleSettings))]
	[KnownType(typeof(TankTransferModuleSettings))]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(PointCommandStatusListReference))]
	[KnownType(typeof(VolumeTransferModuleSettings))]
	[KnownType(typeof(MovementModuleSettings))]
	[KnownType(typeof(MovementType))]
	[KnownType(typeof(MovementData))]
	[KnownType(typeof(MovementNodeData))]
	[KnownType(typeof(LeakDetectionSettings))]
	[KnownType(typeof(MovementType))]
	[DataContract]
	[Serializable]
	public class PointTemplateProperty : BaseSerializedDataObject, ICloneable
	{

		[EntityImportExportAttribute("SETTINGID*", 200, "ID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get { return base.ID; } set { base.ID = value; } }

		public new Guid SiteGuid
		{
				get
				{
					return base.SiteGuid;
				}
				set
				{
					base.SiteGuid = value;
				}
		}

		[EntityImportExportAttribute("SETTINGGUID", 200, "PointTemplatePropertyGuid")]
		[FMPersistedField]
		public Guid PointTemplatePropertyGuid
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

      [EntityImportExportAttribute("POINTTEMPLATEGUID", 200, "PointTemplateGuid")]
      [FMPersistedField]
		[DataMember]
		public Guid PointTemplateGuid { get; set; }



		[EntityImportExportAttribute("VALUETYPE", 100, "ValueTypeString")]
		[DataMember]
		[FMPersistedField("ValueType")]
		new public string ValueTypeString
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


		[EntityImportExportAttribute("VALUE", 100, "ValueXml")]
		[FMPersistedField("Value")]
		new public string ValueXml
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
				if (string.IsNullOrEmpty(value))
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

		public object Clone()
		{
			var pt = (PointTemplateProperty)this.MemberwiseClone();
			this.BaseClone(pt);
			return pt;
		}

		public void EnumerateByPointTemplateSQL(SqlCommand cmd, Guid pointTemplateGuid)
		{
			cmd.CommandText = "SELECT * FROM tblPointTemplateProperty WHERE PointTemplateGuid = @PointTemplateGuid ORDER BY ID";
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}



		public void GetSQL(SqlCommand cmd, Guid pointTemplatePropertyGuid)
		{
			cmd.CommandText = "SELECT * FROM tblPointTemplateProperty WHERE PointTemplatePropertyGuid = @PointTemplatePropertyGuid";
			cmd.Parameters.AddWithValue("@PointTemplatePropertyGuid", pointTemplatePropertyGuid);
		}
		

		public List<PointValueIdentifier> GetExposedSettingPointValueIdentifiersFilterByType(bool filter, string dataTypeString, PointValueFieldType fieldFilter)
		{
			var pointValueIddentifiers = new List<PointValueIdentifier>();

			var propertyType = this.Value.GetType();
			var propertyInfos = propertyType.GetProperties();

			foreach (var propertyInfo in propertyInfos)
			{
				var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
				if (fmExposedSettings.Length == 0)
				{
					continue;
				}
				if (!filter ||
				(fieldFilter == PointValueFieldType.VALUE && propertyInfo.PropertyType.FullName == dataTypeString) ||
				fieldFilter == PointValueFieldType.ID ||
				fieldFilter == PointValueFieldType.TIMESTAMP ||
				fieldFilter == PointValueFieldType.UNITS)
				{
					var pointValueIdentifier = new PointValueIdentifier()
														{
															IdentityGuid = this.PointTemplatePropertyGuid,
															PointValueType = PointValueType.Setting,
															PropertyID = propertyInfo.Name
														};
					pointValueIddentifiers.Add(pointValueIdentifier);
				}
			}

			return pointValueIddentifiers;
		}

		public List<PointValueIdentifier> GetExposedSettingPointValueIdentifiers()
		{
			return this.GetExposedSettingPointValueIdentifiersFilterByType(false, string.Empty,PointValueFieldType.VALUE);
		}

		public string GetExposedSettingID(PointValueIdentifier pointValueIdentifier)
		{
			string localId = string.Empty;
			var propertyType = this.Value.GetType();
			var propertyInfos = propertyType.GetProperties();

			foreach (var propertyInfo in propertyInfos)
			{
				if (pointValueIdentifier.PropertyID != propertyInfo.Name)
				{
					continue;
				}

				var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);

				if (fmExposedSettings.Length == 0)
				{
					break;
				}

				var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;

				if (fmExposedSetting != null)
				{
					localId = fmExposedSetting.ID;
				}

				break;
			}

			return localId;
		}

		public FMExposedSetting GetExposedSettingAttribute(PointValueIdentifier pointValueIdentifier)
		{
			var propertyType = this.Value.GetType();
			var propertyInfos = propertyType.GetProperties();
			foreach (var propertyInfo in propertyInfos)
			{
				if (pointValueIdentifier.PropertyID != propertyInfo.Name)
				{
					continue;
				}

				var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
				if (fmExposedSettings.Length == 0)
				{
					break;
				}

				return fmExposedSettings[0] as FMExposedSetting;
			}

			return null;
		}


		static public List<KeyValuePair<string, string>> EnumeratePropertyDataTypes()
		{
			var dataTypes = new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>("System.Boolean", "DataType|Boolean"),
					new KeyValuePair<string, string>("System.Double", "DataType|Double64"),
					new KeyValuePair<string, string>("System.Single", "DataType|Float32"),
					new KeyValuePair<string, string>("System.Int16", "DataType|Short16"),
					new KeyValuePair<string, string>("System.UInt16", "DataType|UShort16"),
					new KeyValuePair<string, string>("System.Int32", "DataType|Integer32"),
					new KeyValuePair<string, string>("System.UInt32", "DataType|UInteger32"),
//					new KeyValuePair<string, string>("System.Int64", "DataType|Long64"),
//					new KeyValuePair<string, string>("System.UInt64", "DataType|ULong64"),
					new KeyValuePair<string, string>("System.String", "DataType|String"),
					new KeyValuePair<string, string>("System.DateTimeOffset", "DataType|Date/Time"),
					new KeyValuePair<string, string>("System.DateTime", "DataType|Date"),
					new KeyValuePair<string, string>("System.TimeSpan", "DataType|Duration"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.StrapTable", "DataType|Strap Table"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.Vessel", "DataType|Vessel"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.VcfModuleSettings", "DataType|Vcf Module Settings"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.QuantityModuleSettings", "DataType|Quantity Module Settings"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.RateModuleSettings", "DataType|Rate Module Settings"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.TankCommandModuleSettings", "DataType|Tank Command Module Settings"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.TankTransferModuleSettings", "DataType|Tank Transfer Module Settings"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.PointCommandStatusListReference", "DataType|Point Command-Status"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.MovementNodeModuleSettings", "DataType|Movement Node Module Settings"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.MovementModuleSettings", "DataType|Movement Module Settings"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.MovementData", "DataType|Movement Data"),
					new KeyValuePair<string, string>("FMBusinessObjects.DataObjects.LeakDetectionSettings", "DataType|Leak Detection Settings")
				};

			return dataTypes;
		}
	}
}

