using System;
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Collections.Generic;
	using System.Data;
	using System.Xml.Serialization;
	using System.IO;
	using System.Web.Script.Serialization;
	using System.Xml;

	using FMBusinessObjects.UtilityObjects;

	[DataContract]
	[Serializable]
	public class BaseSerializedDataObject : BaseDataObject
	{
		
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

		[EntityImportExportAttribute("VALUETYPE", 100, "ValueTypeString")]
		[DataMember]
		[FMPersistedField("ValueType")]
		public string ValueTypeString
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

		[FMPersistedField("Value")]
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
					var emptyNameSpaces = new XmlSerializerNamespaces(new [] { XmlQualifiedName.Empty });
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
				if(string.IsNullOrEmpty(value))
				{
					this._value = null;
					return;
				}

				XmlSerializer serializer;

				if (this._valueType == typeof(DateTimeOffset))
				{
					serializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("DateTimeOffset"));
				}
				else if(this._valueType == typeof(TimeSpan))
				{
					serializer = CachingXmlSerializerFactory.Create(typeof(string), new XmlRootAttribute("TimeSpan"));
				}
				else
				{
					serializer = CachingXmlSerializerFactory.Create(this._valueType);
				}
				var tempReader = new StringReader(value);
				this._value = (object)serializer.Deserialize(tempReader);

				if(this._valueType == typeof(DateTimeOffset))
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

		protected void BaseClone(BaseSerializedDataObject o)
		{
				o.ValueTypeString = this.ValueTypeString;
				o.ValueXml = this.ValueXml;
				o.RowVersion = new byte[this.RowVersion.Length];
				for (var i = 0; i < this.RowVersion.Length; i++)
				{
					o.RowVersion[i] = this.RowVersion[i];
				}
		}
	}
}
