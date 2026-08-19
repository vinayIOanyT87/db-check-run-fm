namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Linq;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.Attributes;
	using FMBusinessObjects.UtilityObjects;

	using Opc.Ua;

	#region PointTag Collection Class
	[KnownType(typeof(string))]
	[KnownType(typeof(double))]
	[KnownType(typeof(bool))]
	[Serializable]
	[CollectionDataContract]
	public class PointTagCollection : List<PointTag>
	{
	}
	#endregion

	[DataContract]
	[Serializable]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(TimeSpan))]
	[KnownType(typeof(CodedVariables.RoofTypeEnum))]
	[KnownType(typeof(PointCommandStatusListReference))]
	[KnownType(typeof(DeviceAlarmMapReference))]
	[KnownType(typeof(CodedVariables.TankStatuses))]
	[KnownType(typeof(CodedVariables.TankCommands))]
	[KnownType(typeof(CodedVariables.TransferModes))]
	[KnownType(typeof(CodedVariables.TankTransferMode))]
	[KnownType(typeof(CodedVariables.VolumeTransferMode))]
	[KnownType(typeof(CodedVariables.TransferStatuses))]
	[KnownType(typeof(CodedVariables.TankOperationalMode))]
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
	[KnownType(typeof(MovementNodeData))]
	[KnownType(typeof(LeakDetectionSettings))]
	[KnownType(typeof(CodedVariables.MovementType))]
	public class PointTag : BaseSerializedDataObject, ICloneable
	{
		[EntityImportExportAttribute("TAGID*", 200, "ID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get {return base.ID;} set	{ base.ID = value; }}

		[EntityImportExportAttribute("TAGGUID", 200, "PointTagGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid PointTagGuid
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

		[EntityImportExportWorksheet("ALARMS", "ALARMID*")]
		[DataMember]
		public Dictionary<Guid, Alarm> Alarms = new Dictionary<Guid, Alarm>();

		[EntityImportExport("UNITSTYPE", 110, "ENGINEERINGUNITSTYPE")]
		[DataMember]
		[FMPersistedField]
		public EngineeringUnitType EngineeringUnitsType { get; set; }

		[EntityImportExport("UNITS", 110, "UNITS")]
		[DataMember]
		[FMPersistedField("EngineeringUnitsIndex")]
		public EngineeringUnit Units { get; set; }

		[DataMember]
		[FMPersistedField("ServerEngineeringUnitsIndex")]
		public EngineeringUnit ServerUnits { get; set; }

		[EntityImportExport("PRECISION", 110, "DECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public byte DecimalPlaces { get; set; }

		[EntityImportExport("STATUS", 100, "Status")]
		[DataMember]
		[FMPersistedField]
		public Int64 Status { get; set; }

		[EntityImportExport("SERVERTIMESTAMP", 100, "ServerTimeStamp")]
		[DataMember]
		[FMPersistedField]
		public DateTimeOffset ServerTimeStamp { get; set; }

		[EntityImportExport("SOURCEIMESTAMP", 100, "SourceTimeStamp")]
		[DataMember]
		[FMPersistedField]
		public DateTimeOffset SourceTimeStamp { get; set; }

		[DataMember]
		[FMPersistedField]
		public double Minimum { get; set; }

		[DataMember]
		[FMPersistedField]
		public double Maximum { get; set; }

		[EntityImportExport("MINIMUM", 100, "MINIMUM")]
		public string MinimumForImportExport
		{
			get
			{
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					// negative values in FmlFtIn16Th or FmlFtIn8Th are formatted incorrectly ( 0-00-00-00 instead of -00-00-00)
					// we need to manually correct it
					if (this.Minimum < 0)
					{
						this.Minimum *= -1;
						var newNegativeValue = EngineeringUnitsHelperClass.FormatValue(this.Minimum, this.Units);
						return "-" + newNegativeValue.ToString();
					}
					var newValue = EngineeringUnitsHelperClass.FormatValue(this.Minimum, this.Units);
					return newValue.ToString();
				}
				else return this.Minimum.ToString();
			}
			set
			{
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					var isNegative = false;
					if (value.StartsWith("-"))
					{
						value = value.Remove(0, 1);
						isNegative = true;
					}
					this.Minimum = (double)EngineeringUnitsHelperClass.ParseValue(typeof(double), value, this.Units, new NumberFormatInfo());
					if (isNegative)
					{
						this.Minimum *= -1;
					}
				}
				else
				{
					this.Minimum = double.Parse(value);
				}
			}
		}

		[EntityImportExportAttribute("MAXIMUM", 100, "MAXIMUM")]
		public string MaximumForImportExport
		{
			get
			{
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					// negative values in FmlFtIn16Th or FmlFtIn8Th are formatted incorrectly ( 0-00-00-00 instead of -00-00-00)
					// we need to manually correct it
					if (this.Maximum < 0)
					{
						this.Maximum *= -1;
						var newNegativeValue = EngineeringUnitsHelperClass.FormatValue(this.Maximum, this.Units);
						return "-" + newNegativeValue.ToString();
					}
					var newValue = EngineeringUnitsHelperClass.FormatValue(this.Maximum, this.Units);
					return newValue.ToString();
				}
				else return this.Maximum.ToString();
			}
			set
			{
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					var isNegative = false;
					if (value.StartsWith("-"))
					{
						value = value.Remove(0, 1);
						isNegative = true;
					}
					this.Maximum = (double)EngineeringUnitsHelperClass.ParseValue(typeof(double), value, this.Units, new NumberFormatInfo());
					if (isNegative)
					{
						this.Maximum *= -1;
					}
				}
				else
				{
					this.Maximum = double.Parse(value);
				}
			}
		}


		[EntityImportExport("DEADBAND", 110, "DEADBAND")]
		[DataMember]
		[FMPersistedField]
		public double Deadband { get; set; }

		[EntityImportExport("HOLDOFF", 110, "HOLDOFF")]
		[DataMember]
		[FMPersistedField]
		public int Holdoff { get; set; }

		[EntityImportExport("INPUTOUPUTTYPE", 110, "INPUTOUTPUTTYPE")]
		[DataMember]
		[FMPersistedField("PointTagInputOutputTypeIndex")]
		public PointTemplateTag.PointTagInputOutputType InputOutputType { get; set; }

        [DataMember]
        [FMPersistedField("LastPointTagInputOutputTypeIndex")]
        public PointTemplateTag.PointTagInputOutputType LastInputOutputType { get; set; }

        [EntityImportExport("INPUT", 110, "INPUT")]
		[DataMember]
		[FMPersistedField]
		public bool Input { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool AlarmStatus { get; set; }

		[EntityImportExport("APPLYUNITS", 110, "APPLYPOINTENGINEERINGUNITS")]
		[DataMember]
		[FMPersistedField]
		public bool ApplyPointEngineeringUnits { get; set; }

		[EntityImportExport("APPLYPRECISION", 110, "APPLYPOINTDECIMALPLACES")]
		[DataMember]
		[FMPersistedField]
		public bool ApplyPointDecimalPlaces { get; set; }

		[EntityImportExport("APPLYMAXIMUM", 110, "APPLYPOINTMAXIMUM")]
		[DataMember]
		[FMPersistedField]
		public bool ApplyPointMaximum { get; set; }

		[EntityImportExport("APPLYMINIMUM", 110, "APPLYMINIMUM")]
		[DataMember]
		[FMPersistedField]
		public bool ApplyPointMinimum { get; set; }

		[EntityImportExportAttribute("POINTGUID", 200, "PointGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid PointGuid { get; set; }

		[EntityImportExportAttribute("POINTTEMPLATETAGGUID", 200, "PointTemplateTagGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid PointTemplateTagGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
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

		[DataMember]
		[FMPersistedField("SiteID", ReadOnly = true)]
		public string SiteID{ get; set; }

		[DataMember]
		[FMPersistedField("PointID", ReadOnly = true)]
		public string PointID { get; set; }

		[DataMember]
		[FMPersistedField("PointType", ReadOnly = true)]
		public string PointType { get; set; }

		[DataMember]
		[FMPersistedField("PointDescription", ReadOnly = true)]
		public string PointDescription { get; set; }


		[DataMember]
		[FMPersistedField("Enabled", ReadOnly = true)]
		public bool Enabled { get; set; }


		public UInt32 OpcStatusSubCode => new StatusCode((uint)this.Status).SubCode;


		public UInt32 OpcStatusCodeBits => new StatusCode((uint) this.Status).CodeBits;

		[EntityImportExport("OPCUABROWSEPATH", 110, "OPCUABROWSEPATH")]
		[DataMember]
		[FMPersistedField]
		public string OpcUaBrowsePath { get; set; }

		[EntityImportExport("OPCUANAMESPACEURI", 110, "OPCUANAMESPACEURI")]
		[DataMember]
		[FMPersistedField]
		public string OpcUaNamespaceUri { get; set; }

		[EntityImportExport("OPCUAPUBLISHINGINTERVAL", 110, "OPCUAPUBLISHINGINTERVAL")]
		[DataMember]
		[FMPersistedField]
		public Int32? OpcUaPublishingInterval { get; set; }

		[EntityImportExport("OPCUANODEID", 110, "OPCUANODEID")]
		[DataMember]
		[FMPersistedField]
		public string OpcUaNodeId { get; set; }

		[EntityImportExport("OPCUAISREADABLE", 110, "OPCUAISREADABLE")]
		[DataMember]
		[FMPersistedField("OpcUaIsReadable")]
		public bool OpcUaIsReadable { get; set; }

		[DataMember]
		[FMPersistedField("OpcUaServerDataType")]
		public Int32? OpcUaServerDataType { get; set; }


		[EntityImportExport("OPCUASERVERRDATATYPE", 110, "OPCUASERVERDATATYPESTRING")]
		public string OpcUaServerDataTypeString
		{
			get
			{
				try
				{
					if(this.OpcUaServerDataType.HasValue)
					{
						return ((BuiltInType)this.OpcUaServerDataType.Value).ToString();
					}
					return BuiltInType.Null.ToString();
				}
				catch(Exception)
				{
					return BuiltInType.Null.ToString();
				}
			}
			set
			{
				this.OpcUaServerDataType = new Int32?((Int32) Enum.Parse(typeof(BuiltInType), value));
			}
		}


		[EntityImportExport("OPCUAWRITEHOLDOFFTIME", 110, "OPCUAWRITEHOLDOFFTIME")]
		[DataMember]
		[FMPersistedField]
		public Int32? OpcUaWriteHoldoffTime { get; set; }

		[EntityImportExport("OPCUAWRITEPERIODICUPDATEINTERVAL", 110, "OPCUAWRITEPERIODICUPDATEINTERVAL")]
		[DataMember]
		[FMPersistedField]
		public Int32? OpcUaWritePeriodicUpdateInterval { get; set; }

		[DataMember]
		public Int64 RowVersionLong { get; set; }		

		/* OPC UA Server Configuration Properties */
		[DataMember]
		[FMPersistedField]
		public Guid OpcUaServerGuid { get; set; }

		[EntityImportExport("OPCUASERVERENDPOINT", 110, "OPCUASERVERENDPOINT")]
		[DataMember]
		[FMPersistedField("ServerEndPoint", ReadOnly = true)]
		public string OpcUaServerEndPoint { get; set; }

		[EntityImportExport("OPCUASECURITYMODE", 110, "OPCUASECURITYMODE")]
		[DataMember]
		[FMPersistedField("SecurityMode", ReadOnly = true)]
		public string OpcUaSecurityMode { get; set; }

		[EntityImportExport("OPCUASECURITYPOLICY", 110, "OPCUASECURITYPOLICY")]
		[DataMember]
		[FMPersistedField("SecurityPolicy", ReadOnly = true)]
		public string OpcUaSecurityPolicy { get; set; }

		[EntityImportExport("OPCUAMESSSAGEENCODING", 110, "OPCUAMESSSAGEENCODING")]
		[DataMember]
		[FMPersistedField("MessageEncoding", ReadOnly = true)]
		public string OpcUaMessageEncoding { get; set; }

		[EntityImportExport("OPCUAIDENTITYMETHOD", 110, "OPCUAIDENTITYMETHOD")]
		[DataMember]
		[FMPersistedField("UserIdentityMethod", ReadOnly = true)]
		public string OpcUaUserIdentityMethod { get; set; }

		[EntityImportExport("OPCUAUSERID", 110, "OPCUAUSERID")]
		[DataMember]
		[FMPersistedField("UserId", ReadOnly = true)]
		public string OpcUaUserId { get; set; }

		[EntityImportExport("OPCUAUSERPASSWORD", 110, "OPCUAUSERPASSWORD")]
		[DataMember]
		[FMPersistedField("UserPassword", ReadOnly = true)]
		public string OpcUaUserPassword { get; set; }

		[EntityImportExport("OPCUAUSERCERTIFICATEPATH", 110, "OPCUAUSERCERTIFICATEPATH")]
		[DataMember]
		[FMPersistedField("UserCertificatePath", ReadOnly = true)]
		public string OpcUaUserCertificatePath { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public Byte[] OpcUaServerRowVersion { get; set; }

		[DataMember]
		public Int64 OpcUaServerRowVersionLong { get; set; }

		[DataMember]
		public Boolean WrittenToEnterprise { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public Guid AlarmPriorityGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public bool Acknowledged { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string AlarmState { get; set; }

		public Alarm HighestPriorityAlarm { get; set; }

		public AlarmTest HighestOrderAlarmTest { get; set; }

		public PointTagAlarmStatus HighestOrderPointTagAlarmStatus { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public Guid WellKnownIdentityGuid { get; set; }

		[EntityImportExport("ALARMSENABLED", 100, "AlarmsEnabled")]
		[DataMember]
		[FMPersistedField]
		public bool AlarmsEnabled { get; set; }

		[EntityImportExport("CONFIGSOURCE", 100, "InhibitInputOutputTypeConfiguration")]
		[DataMember]
		[FMPersistedField]
		public bool InhibitInputOutputTypeConfiguration { get; set; }

		[EntityImportExport("OVERRIDE", 100, "InhibitOverride")]
		[DataMember]
		[FMPersistedField]
		public bool InhibitOverride { get; set; }

		[EntityImportExport("ARCHIVED", 100, "Archived")]
		[DataMember]
		[FMPersistedField]
		public bool Archived { get; set; }


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
		public string ValueXmlForExport
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

					if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
					{
						XmlDocument doc = new XmlDocument();
						doc.LoadXml(retValue);
						XmlNode root = doc.DocumentElement;

						var tempValue = double.Parse(root.InnerText);
						var isNegativeValue = false;
						if (tempValue < 0)
						{
							tempValue *= -1;
							isNegativeValue = true;
						}

						var newValue = EngineeringUnitsHelperClass.FormatValue(
							tempValue,
							this.Units);
						root.InnerText = (isNegativeValue ? "-" : "") + newValue.ToString();
						retValue = doc.OuterXml;
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
				if (this.Units == EngineeringUnit.FmlFtIn16Th || this.Units == EngineeringUnit.FmlFtIn8Th)
				{
					string xmlString = tempReader.ReadToEnd();
					XmlDocument doc = new XmlDocument();
					if (xmlString != null)
					{
						doc.LoadXml(xmlString);
						XmlNode root = doc.DocumentElement;
						var xmlValue = root.InnerText;
						var isNegative = false;
						if (xmlValue.StartsWith("-"))
						{
							xmlValue = xmlValue.Remove(0, 1);
							isNegative = true;
						}
						var newValue = EngineeringUnitsHelperClass.ParseValue(
							typeof(double),
							xmlValue,
							this.Units,
							new NumberFormatInfo());

						if (isNegative)
						{
							newValue = (double)newValue * -1;
						}
						root.InnerText = newValue.ToString();
					}
					var tempReader2 = new StringReader(doc.OuterXml);
					this._value = (object)serializer.Deserialize(tempReader2);
				}
				else
				{
					this._value = (object)serializer.Deserialize(tempReader);
				}
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

		public string QualityAbbreviation
		{
			get
			{

				if (this.IsManualTag())
				{
					return "MAN";
				}

				if (this.IsForced())
				{
					return "FRC";
				}

				if (this.IsOverrange())
				{
					return "OVR";
				}

				if (this.IsUnderrange())
				{
					return "UNR";
				}

				if (this.IsUncertain())
				{
					return "UNC";
				}
				//Needs to return blank string for spacing in Draw and Operate
				return "";
			}
		}

		public object Clone()
		{
			var t = (PointTag)this.MemberwiseClone();
			this.BaseClone(t);
			if (this.Alarms != null)
			{
				t.Alarms = new Dictionary<Guid, Alarm>();
				foreach (var alarm in this.Alarms)
				{
					t.Alarms.Add(alarm.Key, (Alarm)alarm.Value.Clone());
				}
			}
			return t;
		}

		public static void Copy(PointTag fromTag, PointTag toTag)
		{
			toTag.ID = fromTag.ID;
			if (fromTag.Value is ValueType)
			{
				toTag.Value = fromTag.Value;
			}
			else
			{
				toTag.ValueXml = fromTag.ValueXml;
			}

			toTag.Status = fromTag.Status;
			toTag.ServerTimeStamp = fromTag.ServerTimeStamp;
			toTag.SourceTimeStamp = fromTag.SourceTimeStamp;

			toTag.EngineeringUnitsType = fromTag.EngineeringUnitsType;
			toTag.Units = fromTag.Units;
			toTag.DecimalPlaces = fromTag.DecimalPlaces;
			toTag.ServerUnits = fromTag.ServerUnits;
			toTag.Maximum = fromTag.Maximum;
			toTag.Minimum = fromTag.Minimum;
			toTag.InputOutputType = fromTag.InputOutputType;
			toTag.Input = fromTag.Input;
			toTag.AlarmStatus = fromTag.AlarmStatus;
			toTag.SiteID = fromTag.SiteID;
			toTag.PointID = fromTag.PointID;
			toTag.PointType = fromTag.PointType;
			toTag.PointDescription = fromTag.PointDescription;
			toTag.Enabled = fromTag.Enabled;
			toTag.ApplyPointDecimalPlaces = fromTag.ApplyPointDecimalPlaces;
			toTag.ApplyPointEngineeringUnits = fromTag.ApplyPointEngineeringUnits;
			toTag.ApplyPointMaximum = fromTag.ApplyPointMaximum;
			toTag.ApplyPointMinimum = fromTag.ApplyPointMinimum;
			toTag.OpcUaBrowsePath = fromTag.OpcUaBrowsePath;
			toTag.OpcUaIsReadable = fromTag.OpcUaIsReadable;
			toTag.OpcUaServerDataType = fromTag.OpcUaServerDataType;
			toTag.OpcUaMessageEncoding = fromTag.OpcUaMessageEncoding;
			toTag.OpcUaNamespaceUri = fromTag.OpcUaNamespaceUri;
			toTag.OpcUaNodeId = fromTag.OpcUaNodeId;
			toTag.OpcUaPublishingInterval = fromTag.OpcUaPublishingInterval;
			toTag.OpcUaWriteHoldoffTime = fromTag.OpcUaWriteHoldoffTime;
			toTag.OpcUaWritePeriodicUpdateInterval = fromTag.OpcUaWritePeriodicUpdateInterval;
			toTag.OpcUaSecurityMode = fromTag.OpcUaSecurityMode;
			toTag.OpcUaSecurityPolicy = fromTag.OpcUaSecurityPolicy;
			toTag.OpcUaServerEndPoint = fromTag.OpcUaServerEndPoint;
			toTag.OpcUaServerGuid = fromTag.OpcUaServerGuid;
			toTag.OpcUaUserCertificatePath = fromTag.OpcUaUserCertificatePath;
			toTag.OpcUaUserId = fromTag.OpcUaUserId;
			toTag.OpcUaUserIdentityMethod = fromTag.OpcUaUserIdentityMethod;
			toTag.OpcUaUserPassword = fromTag.OpcUaUserPassword;
			toTag.AlarmsEnabled = fromTag.AlarmsEnabled;
			toTag.InhibitInputOutputTypeConfiguration = fromTag.InhibitInputOutputTypeConfiguration;
			toTag.InhibitOverride = fromTag.InhibitOverride;
			toTag.Archived = fromTag.Archived;
			toTag.Deadband = fromTag.Deadband;
			toTag.Holdoff = fromTag.Holdoff;


			toTag.Alarms = new Dictionary<Guid, Alarm>();
			if (fromTag.Alarms != null)
			{
				foreach (var alarm in fromTag.Alarms)
				{
					toTag.Alarms.Add(alarm.Key, (Alarm)alarm.Value.Clone());
				}
			}

			toTag.UpdatedDate = fromTag.UpdatedDate;

		}

		public PointTag()
		{
			this.Status = StatusCodes.Bad;
			this.WrittenToEnterprise = false;
			this.OpcUaPublishingInterval = 1000;
			this.OpcUaWriteHoldoffTime = null;
			this.OpcUaWritePeriodicUpdateInterval = null;
			this.ValueType = typeof(System.DBNull);
		}

		public PointTag(PointTemplateTag tag, bool referenceTemplate)
		{
			this.ID = tag.ID;
			this.ApplyPointDecimalPlaces = tag.ApplyPointTemplateDecimalPlaces;
			this.ApplyPointEngineeringUnits = tag.ApplyPointTemplateEngineeringUnits;
			this.ApplyPointMaximum = tag.ApplyPointTemplateMaximum;
			this.ApplyPointMinimum = tag.ApplyPointTemplateMinimum;
			this.ValueType = tag.ValueType;
			this.DecimalPlaces = tag.DecimalPlaces;
			this.Input = tag.Input;
			this.AlarmStatus = tag.AlarmStatus;
			this.InputOutputType = tag.InputOutputType;
			this.Maximum = tag.Maximum;
			this.Minimum = tag.Minimum;
			this.ServerUnits = tag.ServerUnits;
			this.EngineeringUnitsType = tag.EngineeringUnitsType;
			this.Units = tag.Units;
			this.Value = tag.Value;
			if (referenceTemplate)
			{
				this.PointTemplateTagGuid = tag.PointTemplateTagGuid;
			}
			this.ServerTimeStamp = DateTimeOffset.UtcNow;
			this.SourceTimeStamp = DateTimeOffset.UtcNow;
			this.AlarmsEnabled = tag.AlarmsEnabled;
			this.InhibitInputOutputTypeConfiguration = tag.InhibitInputOutputTypeConfiguration;
			this.InhibitOverride = tag.InhibitOverride;
			this.Archived = tag.Archived;
			this.Deadband = 0.0;

			// a null value is bad, but if the value is a refereence type then it is a complex object is effectivly null until initialized
			if (this.Value == null)
			{
				this.Status = StatusCodes.Bad;
			}
			else if (this.Value is PointCommandStatusListReference
						&& !(this.Value as PointCommandStatusListReference).CurrentValue.HasValue)
			{
				this.Status = StatusCodes.Bad;
			}
			else if (this.Value is DeviceAlarmMapReference
						&& !(this.Value as DeviceAlarmMapReference).CurrentValue.HasValue)
			{
				this.Status = StatusCodes.Bad;
			}
			else
			{
				this.Status = StatusCodes.Good;
			}

			this.IdentityGuid = Guid.NewGuid();
			this.WrittenToEnterprise = false;
			this.OpcUaPublishingInterval = 1000;

			if(Input)
			{
				this.OpcUaWriteHoldoffTime = null;
				this.OpcUaWritePeriodicUpdateInterval = null;
			}
			else
			{
				this.OpcUaWriteHoldoffTime = null;
				this.OpcUaWritePeriodicUpdateInterval = null;
			}
		}

		public PointTag(PointTag tag)
		{
			this.ID = tag.ID;
			this.ApplyPointDecimalPlaces = tag.ApplyPointDecimalPlaces;
			this.ApplyPointEngineeringUnits = tag.ApplyPointEngineeringUnits;
			this.ApplyPointMaximum = tag.ApplyPointMaximum;
			this.ApplyPointMinimum = tag.ApplyPointMinimum;
			this.ValueType = tag.ValueType;
			this.DecimalPlaces = tag.DecimalPlaces;
			this.Input = tag.Input;
			this.AlarmStatus = tag.AlarmStatus;
			this.InputOutputType = tag.InputOutputType;
			this.Maximum = tag.Maximum;
			this.Minimum = tag.Minimum;
			this.ServerUnits = tag.ServerUnits;
			this.EngineeringUnitsType = tag.EngineeringUnitsType;
			this.Units = tag.Units;
			this.Value = tag.Value;
			this.ServerTimeStamp = tag.ServerTimeStamp;
			this.SourceTimeStamp = tag.SourceTimeStamp;
			this.Status = tag.Status;
			this.IdentityGuid = tag.IdentityGuid;
			this.PointGuid = tag.PointGuid;
			this.PointTemplateTagGuid = tag.PointTemplateTagGuid;
			this.SiteID = tag.SiteID;
			this.PointID = tag.PointID;
			this.PointType = tag.PointType;
			this.PointDescription = tag.PointDescription;
			this.Enabled = tag.Enabled;
			this.SiteGuid = tag.SiteGuid;
			this.OpcUaBrowsePath = tag.OpcUaBrowsePath;
			this.OpcUaIsReadable = tag.OpcUaIsReadable;
			this.OpcUaServerDataType = tag.OpcUaServerDataType;
			this.OpcUaMessageEncoding = tag.OpcUaMessageEncoding;
			this.OpcUaNamespaceUri = tag.OpcUaNamespaceUri;
			this.OpcUaNodeId = tag.OpcUaNodeId;
			this.OpcUaPublishingInterval = tag.OpcUaPublishingInterval;
			this.OpcUaWriteHoldoffTime = tag.OpcUaWriteHoldoffTime;
			this.OpcUaWritePeriodicUpdateInterval = tag.OpcUaWritePeriodicUpdateInterval;
			this.OpcUaSecurityMode = tag.OpcUaSecurityMode;
			this.OpcUaSecurityPolicy = tag.OpcUaSecurityPolicy;
			this.OpcUaUserIdentityMethod = tag.OpcUaUserIdentityMethod;
			this.OpcUaUserId = tag.OpcUaUserId;
			this.OpcUaUserPassword = tag.OpcUaUserPassword;
			this.OpcUaUserCertificatePath = tag.OpcUaUserCertificatePath;
			this.OpcUaServerEndPoint = tag.OpcUaServerEndPoint;
			this.OpcUaServerGuid = tag.OpcUaServerGuid;
			this.WrittenToEnterprise = tag.WrittenToEnterprise;
			this.AlarmPriorityGuid = tag.AlarmPriorityGuid;
			this.Acknowledged = tag.Acknowledged;
			this.AlarmState = tag.AlarmState;
			this.AlarmsEnabled = tag.AlarmsEnabled;
			this.InhibitInputOutputTypeConfiguration = tag.InhibitInputOutputTypeConfiguration;
			this.InhibitOverride = tag.InhibitOverride;
			this.Archived = tag.Archived;
			this.Deadband = tag.Deadband;
			this.Holdoff = tag.Holdoff;
		}

		public bool IsActiveAlarm()
		{
			foreach (var alarm in this.Alarms.Values)
			{
				if (alarm.IsActiveAlarm())
				{
					return true;
				}
			}
			return false;
		}
		public bool UpdateShelvedAlarmInfo()
		{
			if (this.HighestPriorityAlarm != null)
			{
				if (this.HighestOrderPointTagAlarmStatus.AlarmTestFailed)
				{
					if (this.AlarmPriorityGuid != this.HighestOrderAlarmTest.AlarmPriorityGuid
					&& !this.HighestPriorityAlarm.ShelvedOneShot
					&& (this.HighestPriorityAlarm.ShelvedEndTimeStamp.HasValue && this.HighestPriorityAlarm.ShelvedEndTimeStamp.Value <= DateTimeOffset.UtcNow))
					{
						this.AlarmPriorityGuid = this.HighestOrderAlarmTest.AlarmPriorityGuid;
						this.ServerTimeStamp = DateTimeOffset.UtcNow;
						return true;
					}
				}
				else
				{
					if(!this.HighestOrderPointTagAlarmStatus.Acknowledged
					&& this.AlarmPriorityGuid != this.HighestOrderAlarmTest.NormalUnacknowledgedAlarmPriorityGuid
					&& !this.HighestPriorityAlarm.ShelvedOneShot
					&& (this.HighestPriorityAlarm.ShelvedEndTimeStamp.HasValue && this.HighestPriorityAlarm.ShelvedEndTimeStamp.Value <= DateTimeOffset.UtcNow))
					{
						this.AlarmPriorityGuid = this.HighestOrderAlarmTest.NormalUnacknowledgedAlarmPriorityGuid;
						this.ServerTimeStamp = DateTimeOffset.UtcNow;
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Verifies if a given PointTag is the same as the current PointTag, from an OPC UA Monitoring standpoint.
		/// The comparison ignores fields to which value changes would not necessitate a change in OPC UA Monitoring.
		/// </summary>
		/// <param name="targetPointTag"></param>
		/// <returns></returns>
		public bool IsOpcUaEqual(PointTag targetPointTag)
		{
			bool result = true;
			if (targetPointTag == null)
			{
				return false;
			}

			List<string> fieldsToIgnore = new List<string>
			 {
				 "RowVersion",
				 "RowVersionLong",
				 "OpcUaServerRowVersion",
				 "OpcUaServerRowVersionLong",
				 "OpcUaChangeType",
				 "Value",
				 "ValueXml",
														"SourceTimeStamp",
														"ServerTimestamp",
				 "UpdatedBy",
				 "UpdatedDate",
				 "AlarmPriorityGuid",
				 "Acknowledgement",
				 "AlarmState"
			 };

			PropertyInfo[] properties = this.GetType().GetProperties();

			foreach (PropertyInfo property in properties)
			{
				if (fieldsToIgnore.Contains(property.Name))
				{
					continue;
				}

				var baseValue = property.GetValue(this);
				var targetValue = property.GetValue(targetPointTag);

				if (baseValue == null && targetValue == null)
				{
					continue;
				}

				if (baseValue == null || targetValue == null)
				{
					result = false;
					break;
				}

				if (!baseValue.Equals(targetValue))
				{
					result = false;
					break;
				}
			}

			return result;
		}

		public bool IsOverrange()
		{
			if(this.Value is double && (double)this.Value > this.Maximum)
			{
				return true;
			}
			var statusCode = new StatusCode((uint)this.Status);
				
			return statusCode.LimitBits == LimitBits.High;
		}

		public bool IsUnderrange()
		{
			if (this.Value is double && (double)this.Value < this.Minimum)
			{
				return true;
			}
			var statusCode = new StatusCode((uint)this.Status);

			return statusCode.LimitBits == LimitBits.Low;
		}

		public bool IsForced()
		{
			return this.OpcStatusCodeBits == StatusCodes.GoodLocalOverride;
		}

		public bool IsUncertain()
		{
			return StatusCode.IsUncertain((uint)this.Status);
		}

		public bool IsGood()
		{
			return StatusCode.IsGood((uint)this.Status);
		}

		public bool IsBad()
		{
			return StatusCode.IsBad((uint)this.Status);
		}

		public bool IsManualTag()
		{
			return this.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual;
		}

		public int GetDecimalPlaces(Point p)
		{
			if (this.ApplyPointDecimalPlaces)
			{
			 int ret = p.GetDecimalPlaces(this.EngineeringUnitsType);
			 return (ret >= 0) ? ret : this.DecimalPlaces;
			}
			return this.DecimalPlaces;
		}

		public double GetMaximum(Point p)
		{
			if (this.ApplyPointMaximum)
			{
				return p.GetMaximum(this.EngineeringUnitsType);
			}
			return this.Maximum;
		}

		public double GetMinimum(Point p)
		{
			if (this.ApplyPointMinimum)
			{
				return p.GetMinimum(this.EngineeringUnitsType);
			}
			return this.Minimum;
		}

		public EngineeringUnit GetEngineeringUnits(Point p)
		{
			if (this.ApplyPointEngineeringUnits)
			{
				return p.GetEngineeringUnits(this.EngineeringUnitsType);
			}
			return this.Units;
		}

		public string FormatValue(Point p, SiteClass s)
		{
			var decimalPlaces = this.GetDecimalPlaces(p);
			var val = EngineeringUnitsHelperClass.FormatValue(this.Value, this.Units);
			string retVal = s.FormatValue(val, decimalPlaces);
			return retVal;
		}

		public string FormatValueFullPrecision(Point p, SiteClass s)
		{
			string retVal = string.Empty;
			var decimalPlaces = this.GetDecimalPlaces(p);
			var val = EngineeringUnitsHelperClass.FormatValue(this.Value, this.Units);
			if (val is double || val is float)
			{
				decimalPlaces = 9;
				var numFormatProvider = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
				retVal = s.FormatValue(val, decimalPlaces);
				if (retVal.IndexOf(numFormatProvider.NumberDecimalSeparator) > -1)
				{
					retVal = retVal.Trim('0');
				}
				if (retVal.IndexOf(numFormatProvider.NumberDecimalSeparator)  == retVal.Length - 1)
				{
					retVal = retVal.Remove(retVal.Length - 1, 1);
				}
			}
			else
			{
				retVal = s.FormatValue(val, decimalPlaces);
			}
			return retVal;
		}
	}
}
