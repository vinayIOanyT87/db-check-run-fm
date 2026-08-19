namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Data;
	using System.Data.SqlClient;
	using System.Xml.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.Attributes;
	using System.IO;

	using FMBusinessObjects.UtilityObjects;

	#region PointTemplate Collection Class
	[Serializable]
	[CollectionDataContract]
	public class PointTemplateCollection : List<PointTemplate>
	{
	}
	#endregion

	[EntityImportExportWorksheetAttribute("POINTTEMPLATES")]
	[DataContract]
	[Serializable]
	[KnownType(typeof(PointCommandStatus))]
	public class PointTemplate : BasePoint
	{

		[EntityImportExportAttribute("SITE*", 200, "SiteGuid")]
		[XmlIgnore]
		[FMPersistedField]
		public override Guid SiteGuid { get { return base._SiteGuid; } set { base._SiteGuid = value; } }

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.POINT_TEMPLATE; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		[EntityImportExportWorksheet("TEMPLATETAGS", "TAGID*")]
		[DataMember]
		public Dictionary<Guid, PointTemplateTag> Tags { get; set; }

		[EntityImportExportWorksheet("MODULEINSTANCES", "MODULEID*")]
		[DataMember]
		public Dictionary<Guid, ModuleToPointTemplateMap> ModuleInstances{ get; set; }

		[EntityImportExportWorksheet("TEMPLATESETTINGS", "SETTINGID*")]
		[DataMember]
		public Dictionary<Guid, PointTemplateProperty> Properties { get; set; }

		[DataMember]
		public Dictionary<Guid, Module> Modules { get; set; }

		[EntityImportExportAttribute("POINTTEMPLATEID*", 100, "POINTTEMPLATEID")]
		[FMExposedSetting("Point ID", ModifyDisabled = true)]
		public string PointId { get { return this.ID; } set {this.ID = value; } }

		[FMPersistedField]
		[DataMember]
		public override string ID { get { return base.ID; } set { if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9 ]*$")) { base.ID = value; } else throw new Exception("Point Template ID must be Alphanumeric"); } }


		[EntityImportExportAttribute("DESCRIPTION", 100, "DESCRIPTION")]
		[DataMember]
		[FMPersistedField]
		[FMExposedSetting("Point Description", ModifyDisabled = true)]
		public string Description { get; set; }

        [FMExposedSetting("Level Units", ModifyDisabled = true)]
        public string LevelUnits
        {
            get { return base.LevelUnit.GetAbbreviation(); }
            set
            {
                if (Enum.TryParse(value, out EngineeringUnit parsedUnit))
                    base.LevelUnit = parsedUnit;
            }
        }

        [FMExposedSetting("Level Min", ModifyDisabled = false)]
        public double LevelMin { get { return base.LevelMinimum; } set { base.LevelMinimum = value; } }

        [FMExposedSetting("Level Max", ModifyDisabled = false)]
        public double LevelMax { get { return base.LevelMaximum; } set { base.LevelMaximum = value; } }

        [FMExposedSetting("Temperature Units", ModifyDisabled = true)]
        public string TemperatureUnits
        {
            get { return base.TemperatureUnit.GetAbbreviation(); }
            set
            {
                if (Enum.TryParse(value, out EngineeringUnit parsedUnit))
                    base.TemperatureUnit = parsedUnit;
            }
        }

        [FMExposedSetting("Temp Min", ModifyDisabled = false)]
        public double TempMin { get { return base.TemperatureMinimum; } set { base.TemperatureMinimum = value; } }

        [FMExposedSetting("Temp Max", ModifyDisabled = false)]
        public double TempMax { get { return base.TemperatureMaximum; } set { base.TemperatureMaximum = value; } }

        [FMExposedSetting("Volume Units", ModifyDisabled = true)]
        public string VolumeUnits
        {
            get { return base.VolumeUnit.GetAbbreviation(); }
            set
            {
                if (Enum.TryParse(value, out EngineeringUnit parsedUnit))
                    base.VolumeUnit = parsedUnit;
            }
        }

        [FMExposedSetting("Volume Min", ModifyDisabled = false)]
        public double VolumeMin { get { return base.VolumeMinimum; } set { base.VolumeMinimum = value; } }

        [FMExposedSetting("Volume Max", ModifyDisabled = false)]
        public double VolumeMax { get { return base.VolumeMaximum; } set { base.VolumeMaximum = value; } }

        [FMExposedSetting("Mass Units", ModifyDisabled = true)]
        public string MassUnits
        {
            get { return base.MassUnit.GetAbbreviation(); }
            set
            {
                if (Enum.TryParse(value, out EngineeringUnit parsedUnit))
                    base.MassUnit = parsedUnit;
            }
        }

        [FMExposedSetting("Mass Min", ModifyDisabled = false)]
        public double MassMin { get { return base.MassMinimum; } set { base.MassMinimum = value; } }

        [FMExposedSetting("Mass Max", ModifyDisabled = false)]
        public double MassMax { get { return base.MassMaximum; } set { base.MassMaximum = value; } }

        [FMExposedSetting("Density Units", ModifyDisabled = true)]
        public string DensityUnits
        {
            get { return base.DensityUnit.GetAbbreviation(); }
            set
            {
                if (Enum.TryParse(value, out EngineeringUnit parsedUnit))
                    base.DensityUnit = parsedUnit;
            }
        }

        [FMExposedSetting("Density Min", ModifyDisabled = false)]
        public double DensityMin { get { return base.DensityMinimum; } set { base.DensityMinimum = value; } }

        [FMExposedSetting("Density Max", ModifyDisabled = false)]
        public double DensityMax { get { return base.DensityMaximum; } set { base.DensityMaximum = value; } }

        [FMExposedSetting("Standard Density Units", ModifyDisabled = true)]
        public string StandardDensityUnits
        {
            get { return base.DensityUnit.GetAbbreviation(); }
            set
            {
                if (Enum.TryParse(value, out EngineeringUnit parsedUnit))
                    base.DensityUnit = parsedUnit;
            }
        }

        [FMExposedSetting("Standard Density Min", ModifyDisabled = false)]
        public double StandardDensityMin { get { return base.DensityMinimum; } }

        [FMExposedSetting("Standard Density Max", ModifyDisabled = false)]
        public double StandardDensityMax { get { return base.DensityMaximum; } }

        [FMExposedSetting("Volume Rate Units", ModifyDisabled = true)]
        public string VolumeRateUnits
        {
            get { return base.FlowUnit.GetAbbreviation(); }
            set
            {
                if (Enum.TryParse(value, out EngineeringUnit parsedUnit))
                    base.FlowUnit = parsedUnit;
            }
        }

        [FMExposedSetting("Pressure Units", ModifyDisabled = true)]
        public string PressureUnits
        {
            get { return base.PressureUnit.GetAbbreviation(); }
            set
            {
                if (Enum.TryParse(value, out EngineeringUnit parsedUnit))
                    base.PressureUnit = parsedUnit;
            }
        }

      [DataMember]
		[FMExposedSetting("Product")]
		public string ProductID { get; set; }

		[DataMember]
		[FMExposedSetting("Product Description", ModifyDisabled = true)]
		public string ProductDescription { get; set; }

		[DataMember]
		[FMExposedSetting("Site Name", ModifyDisabled = true)]
		public string SiteID { get; set; }

		[DataMember]
		[FMExposedSetting("Site Number", ModifyDisabled = true)]
		public string SiteNumber { get; set; }




		[DataMember]
		[FMExposedSetting("Point Enabled")]
		public bool Enabled { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Standard { get; set; }

		[DataMember]
		[FMPersistedField]
		public int? ExecutionInterval { get; set; }

		[EntityImportExportAttribute("POINTTEMPLATETYPEAPPLICATIONGUID", 100, "POINTTEMPLATETYPEAPPLICATIONGUID")]
		[DataMember]
		[FMPersistedField("PointTemplateTypeApplicationStringGuid")]
		public Guid? PointTemplateTypeGuid { get; set; }

		[EntityImportExportAttribute("PROFILEIMAGEGUID", 100, "PROFILEIMAGEGUID")]
		[DataMember]
		[FMPersistedField]
		public Guid? ProfileImageGuid { get; set; }

		[DataMember]
		public PointCommandStatus PointCommandStatus { get; set; }

		[EntityImportExportAttribute("POINTCOMMANDSTATUS", 200, "POINTCOMMANDSTATUS")]
		[FMPersistedField("PointCommandStatus")]
		public string PointCommandStatusXml {
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
				if (String.IsNullOrEmpty(value))
				{
					this.PointCommandStatus = null;
					return;
				}

				var serializer = CachingXmlSerializerFactory.Create(typeof(PointCommandStatus));
				var stringReader = new StringReader(value);
				this.PointCommandStatus = (PointCommandStatus) serializer.Deserialize(stringReader);

			}
		}

		[DataMember]
		public DeviceAlarmMaps DeviceAlarmMaps { get; set; }

		[EntityImportExportAttribute("DEVICEALARMMAPS", 200, "DEVICEALARMMAPS")]
		[FMPersistedField("DeviceAlarmMaps")]
		public string DeviceAlarmMapsXml
		{
			get
			{
				var retValue = "";
				if (this.DeviceAlarmMaps == null)
				{
					retValue = null;
				}
				else
				{

					var serializer = CachingXmlSerializerFactory.Create(typeof(DeviceAlarmMaps));
					var stringWriter = new StringWriter();
					serializer.Serialize(stringWriter, this.DeviceAlarmMaps);
					retValue = stringWriter.ToString();
				}

				return retValue;

			}
			set
			{
				if (String.IsNullOrEmpty(value))
				{
					this.DeviceAlarmMaps = null;
					return;
				}

				var serializer = CachingXmlSerializerFactory.Create(typeof(DeviceAlarmMaps));
				var stringReader = new StringReader(value);
				this.DeviceAlarmMaps = (DeviceAlarmMaps)serializer.Deserialize(stringReader);

			}
		}


		[EntityImportExportAttribute("DEFAULTDRAWINGGUID", 100, "DEFAULTDRAWINGGUID")]
		[DataMember]
		[FMPersistedField]
		public Guid? DefaultDrawingGuid { get; set; }

		[EntityImportExportAttribute("POINTTEMPLATEGUID", 200, "POINTTEMPLATEGUID")]
		[FMPersistedField]
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

		[EntityImportExportAttribute("POINTLOGICSCRIPT", 100, "POINTLOGICSCRIPT")]
		[DataMember]
		[FMPersistedField]
		public string PointLogicScript { get; set; }

		[DataMember]
		[FMPersistedField]
		public int Version { get; set; }


		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public int DerivedPointCount { get; set; }


		#region Constructors and Destructors

		public PointTemplate()
		{
			this.Reset();
		}

		#endregion

		public override void Reset()
		{
			base.Reset();
			Tags = new Dictionary<Guid, PointTemplateTag>();
			ModuleInstances = new Dictionary<Guid, ModuleToPointTemplateMap>();
			Properties = new Dictionary<Guid, PointTemplateProperty>();
			Description = string.Empty;
			Standard = false;
			ExecutionInterval = null;
			PointTemplateTypeGuid = null;
			DefaultDrawingGuid = null;
		}

		public PointTemplateTag GetTagById(string tagID)
		{
			foreach (var tagEntry in Tags)
			{
				if (tagEntry.Value.ID == tagID)
				{
					return tagEntry.Value;
				}
			}
			return null;
		}


		public void EnumerateSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = "SELECT PT.*, (SELECT COUNT(*) FROM tblPoint P WHERE P.PointTemplateGuid = PT.PointTemplateGuid) AS DerivedPointCount FROM dbo.tblPointTemplate PT"
								+	" INNER JOIN map.tblEntityPointTemplateToSite ESM ON ESM.PointTemplateGuid = PT.PointTemplateGuid"
								+	" WHERE ESM.SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = siteGuid;

			if (PointTemplateTypeGuid.HasValue)
			{
				cmd.CommandText += " AND PT.PointTemplateTypeApplicationStringGuid = @PointTemplateTypeGuid";

				cmd.Parameters.Add( "@PointTemplateTypeGuid", SqlDbType.UniqueIdentifier );
					cmd.Parameters["@PointTemplateTypeGuid"].Value = PointTemplateTypeGuid;
			}
			cmd.CommandText += " ORDER BY PT.ID";
		}

		public static void EnumerateByModuleSQL(SqlCommand cmd, Guid moduleGuid)
		{
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = "SELECT PT.*, (SELECT COUNT(*) FROM tblPoint P WHERE P.PointTemplateGuid = PT.PointTemplateGuid) AS DerivedPointCount FROM dbo.tblPointTemplate PT"
										+ " WHERE PT.PointTemplateGuid IN (SELECT DISTINCT PointTemplateGuid FROM map.tblModuleToPointTemplate mp WHERE mp.ModuleGuid = @ModuleGuid)";

			cmd.Parameters.Add("@ModuleGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ModuleGuid"].Value = moduleGuid;

			cmd.CommandText += " ORDER BY PT.ID";
		}




		public void SelectSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT PT.*, (SELECT COUNT(*) FROM tblPoint P WHERE P.PointTemplateGuid = PT.PointTemplateGuid) AS DerivedPointCount FROM tblPointTemplate PT WHERE PT.PointTemplateGuid = @templateGuid";

			cmd.Parameters.AddWithValue("@templateGuid", this.IdentityGuid);
		}

		public void SelectPointServiceDataSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, PointLogicScript, PointCommandStatus, Version, PointTemplateGuid, SiteGuid FROM tblPointTemplate WHERE PointTemplateGuid = @templateGuid";

			cmd.Parameters.AddWithValue("@templateGuid", this.IdentityGuid);
		}

		public List<PointValueIdentifier> GetExposedSettingPointValueIdentifiers()
		{
			var pointValueIddentifiers = new List<PointValueIdentifier>();

			var propertyType = this.GetType();
			var propertyInfos = propertyType.GetProperties();
			foreach (var propertyInfo in propertyInfos)
			{
				var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
				if (fmExposedSettings.Length == 0)
				{
					continue;
				}

				var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;

				var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = this.PointTemplateGuid, PointValueType = PointValueType.Point, PropertyID = propertyInfo.Name };
				pointValueIddentifiers.Add(pointValueIdentifier);
			}

			return pointValueIddentifiers;
		}

		public string GetExposedSettingID(PointValueIdentifier pointValueIdentifier)
		{
			return this.GetExposedSettingIDFilterByType(pointValueIdentifier, false, string.Empty);
		}

		public FMExposedSetting GetExposedSettingAttribute(PointValueIdentifier pointValueIdentifier)
		{
			var propertyType = this.GetType();
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


		public string GetExposedSettingIDFilterByType(PointValueIdentifier pointValueIdentifier, bool filter, string dataTypeString)
		{
			string ID = string.Empty;
			var propertyType = this.GetType();
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
				if (!filter || propertyInfo.PropertyType.FullName == dataTypeString)
				{
					var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;

					ID = fmExposedSetting.ID;
					break;
				}
			}

			return ID;
		}

		public List<PointValue> GetPointTemplateValueData(
			List<PointValueIdentifier> pointValueIdentifiers)
		{
			List<PointValue> pointValues = new List<PointValue>();
			foreach (var identifier in pointValueIdentifiers)
			{
					switch (identifier.PointValueType)
					{
						case PointValueType.Tag:
							var tag = this.Tags[identifier.IdentityGuid];
								pointValues.Add(new PointValue(tag, this.ID));
							break;
						case PointValueType.Point:
								pointValues.Add(new PointValue(identifier, this));
								break;
						case PointValueType.Setting:
							var setting = this.Properties[identifier.IdentityGuid];
								pointValues.Add(new PointValue(identifier,setting,this));
							break; 
					}
			}
			return pointValues;
		}

		public bool PointTemplateTagModuleReference(Guid pointTemplateTagGuid)
		{
			foreach(var moduleInstance in this.ModuleInstances.Values)
			{
				foreach(var tagToModule in moduleInstance.ModuleToPointTemplateData.TagToModules)
				{
					if(tagToModule.TagGuid == pointTemplateTagGuid)
					{
						return true;
					}		
				}
			}

			return false;
		}

        public bool PointTemplateTagInputOutputConfigurationInhibitedByModule(Guid pointTemplateTagGuid)
        {
            foreach (var moduleInstance in this.ModuleInstances.Values)
            {
                foreach (var tagToModule in moduleInstance.ModuleToPointTemplateData.TagToModules)
                {
                    if (tagToModule.TagGuid == pointTemplateTagGuid)
                    {
						var module = Modules[moduleInstance.ModuleGuid];
						foreach(var modueTag in module.ModuleData.ModuleTags)
						{
							if (modueTag.ParameterName == tagToModule.ModuleParameter && modueTag.InhibitInputOutputTypeConfiguration)
							{
                                return true;
                            }
						}
                        
                    }
                }
            }

            return false;
        }
        
		public Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPointTemplateFilterByType(PointValueType valueType, bool filter, string dataTypeString, PointValueFieldType fieldFilter)
		{

			var pointValueIdentifierDictionary = new Dictionary<PointValueIdentifier, string>();

			if (valueType == PointValueType.Tag
			|| valueType == PointValueType.All)
			{
				foreach (var pointTemplateTag in this.Tags.Values)
				{
					if (!filter ||
					(fieldFilter == PointValueFieldType.VALUE && pointTemplateTag.ValueTypeString == dataTypeString) ||
					(fieldFilter == PointValueFieldType.ALARMSTATUS && pointTemplateTag.AlarmTemplates.Count > 0) ||
					fieldFilter == PointValueFieldType.ID ||
					fieldFilter == PointValueFieldType.TIMESTAMP ||
					fieldFilter == PointValueFieldType.UNITS)
					{
						pointValueIdentifierDictionary.Add(new PointValueIdentifier(pointTemplateTag), pointTemplateTag.ID);
					}
				}
			}

			if (valueType == PointValueType.Setting
			|| valueType == PointValueType.All)
			{

				foreach (var pointTemplateProperty in this.Properties.Values)
				{
					foreach (var pointValueIdentifier in pointTemplateProperty.GetExposedSettingPointValueIdentifiersFilterByType(filter, dataTypeString, fieldFilter))
					{
						pointValueIdentifierDictionary.Add(pointValueIdentifier, pointTemplateProperty.GetExposedSettingID(pointValueIdentifier));
					}
				}
			}

			if (valueType == PointValueType.Point
			|| valueType == PointValueType.All)
			{
				var point = new Point();
				foreach (var pointValueIdentifier in point.GetExposedSettingPointValueIdentifiers())
				{
					var pointValueId = point.GetExposedSettingIDFilterByType(pointValueIdentifier, filter, dataTypeString, fieldFilter);
					if (pointValueId != string.Empty)
					{
						pointValueIdentifier.IdentityGuid = this.PointTemplateGuid;
						pointValueIdentifierDictionary.Add(pointValueIdentifier, pointValueId);
					}
				}
			}

			return pointValueIdentifierDictionary;
		}
	}
}
