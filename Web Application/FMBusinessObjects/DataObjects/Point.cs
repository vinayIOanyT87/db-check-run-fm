namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

	using Attributes;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using System.Reflection;
	using System.Globalization;
	using System.Xml.Serialization;

	#region Point Collection Class
	[Serializable]
	[CollectionDataContract]
	public class PointCollection : List<Point>
	{
		public PointCollection Clone()
		{
			var pointCollection = new PointCollection();
			foreach (var p in this)
			{
				pointCollection.Add(p.Clone());
			}
			return pointCollection;
		}
	}
	#endregion

	[EntityImportExportWorksheetAttribute("POINTS", "POINTID*")]
	[DataContract]
	[Serializable]
	public class Point : BasePoint, IAlarmAndEventDiscovery
	{
		#region Constructors and Destructors
		public Point(PointTemplate template)
		{
			this.PointTemplateGuid			= template.IdentityGuid;
			this.ExecutionInterval = template.ExecutionInterval;
			this.Standard = template.Standard;
			base.DensityDecimalPlaces		= template.DensityDecimalPlaces;
			base.DensityMaximum				= template.DensityMaximum;
			base.DensityMinimum				= template.DensityMinimum;
			base.DensityUnit					= template.DensityUnit;
			base.LevelDecimalPlaces			= template.LevelDecimalPlaces;
			base.LevelMaximum					= template.LevelMaximum;
			base.LevelMinimum					= template.LevelMinimum;
			base.LevelUnit						= template.LevelUnit;
			base.PressureDecimalPlaces		= template.PressureDecimalPlaces;
			base.PressureMaximum				= template.PressureMaximum;
			base.PressureMinimum				= template.PressureMinimum;
			base.PressureUnit					= template.PressureUnit;
			base.FlowDecimalPlaces			= template.FlowDecimalPlaces;
			base.VolumetricFlowMaximum		= template.VolumetricFlowMaximum;
			base.VolumetricFlowMinimum		= template.VolumetricFlowMinimum;
			base.FlowUnit						= template.FlowUnit;
			base.MassFlowDecimalPlaces		= template.MassFlowDecimalPlaces;
			base.MassFlowMaximum				= template.MassFlowMaximum;
			base.MassFlowMinimum				= template.MassFlowMinimum;
			base.MassFlowUnit					= template.MassFlowUnit;
			base.MassDecimalPlaces			= template.MassDecimalPlaces;
			base.MassMaximum					= template.MassMaximum;
			base.MassMinimum					= template.MassMinimum;
			base.MassUnit						= template.MassUnit;
			base.VelocityDecimalPlaces		= template.VelocityDecimalPlaces;
			base.VelocityMaximum				= template.VelocityMaximum;
			base.VelocityMinimum				= template.VelocityMinimum;
			base.VelocityUnit					= template.VelocityUnit;
			base.TemperatureDecimalPlaces	= template.TemperatureDecimalPlaces;
			base.TemperatureMaximum			= template.TemperatureMaximum;
			base.TemperatureMinimum			= template.TemperatureMinimum;
			base.TemperatureUnit				= template.TemperatureUnit;
			base.VolumeDecimalPlaces		= template.VolumeDecimalPlaces;
			base.VolumeMaximum				= template.VolumeMaximum;
			base.VolumeMinimum				= template.VolumeMinimum;
			base.VolumeUnit					= template.VolumeUnit;
			this.ProfileImageGuid			= template.ProfileImageGuid;
			this.Notes							= string.Empty;
			this.DefaultDrawingGuid			= template.DefaultDrawingGuid;
			this.PointDetailDrawingGuid	= null;
			this.PointTemplateVersion		= template.Version;
		}

		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public Point()
		{
			this.Init();
		}

		public Point Clone()
		{
			Point p = (Point)this.MemberwiseClone();
			p.Tags = new Dictionary<Guid, PointTag>();
			foreach (var tKey in this.Tags.Keys)
			{
				var t = this.Tags[tKey];
				p.Tags.Add(tKey, (PointTag)t.Clone());
			}

			p.ModuleInstances = new Dictionary<Guid, ModuleToPointTemplateMap>();
			foreach (var modInst in this.ModuleInstances.Values)
			{
				p.ModuleInstances.Add(modInst.IdentityGuid, modInst.Clone());
			}

			p.Properties = new Dictionary<Guid, PointProperty>();
			foreach (var prop in this.Properties.Values)
			{
				p.Properties.Add(prop.PointPropertyGuid, (PointProperty)prop.Clone());
			}

			p.RowVersion = new byte[this.RowVersion.Length];
			for (int i = 0; i < this.RowVersion.Length; i++)
			{
				p.RowVersion[i] = this.RowVersion[i];
			}
			return p;
		}

		public static void Copy(Point pFrom, Point pTo)
		{
			pTo.ID = pFrom.ID;
			pTo.Description = pFrom.Description;
			pTo.LevelDecimalPlaces = pFrom.LevelDecimalPlaces;
			pTo.LevelMaximum = pFrom.LevelMaximum;
			pTo.LevelMaximum = pFrom.LevelMinimum;
			pTo.LevelUnit = pFrom.LevelUnit;
			pTo.TemperatureDecimalPlaces = pFrom.TemperatureDecimalPlaces;
			pTo.TemperatureMaximum = pFrom.TemperatureMaximum;
			pTo.TemperatureMaximum = pFrom.TemperatureMinimum;
			pTo.TemperatureUnit = pFrom.TemperatureUnit;
			pTo.DensityDecimalPlaces = pFrom.DensityDecimalPlaces;
			pTo.DensityMaximum = pFrom.DensityMaximum;
			pTo.DensityMaximum = pFrom.DensityMinimum;
			pTo.DensityUnit = pFrom.DensityUnit;
			pTo.PressureDecimalPlaces = pFrom.PressureDecimalPlaces;
			pTo.PressureMaximum = pFrom.PressureMaximum;
			pTo.PressureMaximum = pFrom.PressureMinimum;
			pTo.PressureUnit = pFrom.PressureUnit;
			pTo.FlowDecimalPlaces = pFrom.FlowDecimalPlaces;
			pTo.VolumetricFlowMaximum = pFrom.VolumetricFlowMaximum;
			pTo.VolumetricFlowMaximum = pFrom.VolumetricFlowMinimum;
			pTo.FlowUnit = pFrom.FlowUnit;
			pTo.VolumeDecimalPlaces = pFrom.VolumeDecimalPlaces;
			pTo.VolumeMaximum = pFrom.VolumeMaximum;
			pTo.VolumeMaximum = pFrom.VolumeMinimum;
			pTo.VolumeUnit = pFrom.VolumeUnit;
			pTo.MassDecimalPlaces = pFrom.MassDecimalPlaces;
			pTo.MassMaximum = pFrom.MassMaximum;
			pTo.MassMaximum = pFrom.MassMinimum;
			pTo.MassUnit = pFrom.MassUnit;
			pTo.VelocityDecimalPlaces = pFrom.VelocityDecimalPlaces;
			pTo.VelocityMaximum = pFrom.VelocityMaximum;
			pTo.VelocityMaximum = pFrom.VelocityMinimum;
			pTo.VelocityUnit = pFrom.VelocityUnit;
			pTo.MassFlowDecimalPlaces = pFrom.MassFlowDecimalPlaces;
			pTo.MassFlowMaximum = pFrom.MassFlowMaximum;
			pTo.MassFlowMaximum = pFrom.MassFlowMinimum;
			pTo.MassFlowUnit = pFrom.MassFlowUnit;
			pTo.ProfileImageGuid = pFrom.ProfileImageGuid;
			pTo.DefaultDrawingGuid = pFrom.DefaultDrawingGuid;
			pTo.PointDetailDrawingGuid = pFrom.PointDetailDrawingGuid;
			pTo.ProductGuid = pFrom.ProductGuid;
			pTo.ProductID = pFrom.ProductID;
			pTo.OverrideDefaultDrawingGuid = pFrom.OverrideDefaultDrawingGuid;
			pTo.PointTemplateVersion = pFrom.PointTemplateVersion;
			pTo.PointTemplateGuid = pFrom.PointTemplateGuid;

			var tagDictionary = new Dictionary<Guid, PointTag>();

			foreach (var tag in pFrom.Tags.Values)
			{
				PointTag tagClone = null;
				if (pTo.Tags.TryGetValue(tag.PointTagGuid, out tagClone))
				{
					bool changed = false;

					if (tag.UpdatedDate != tagClone.UpdatedDate)
					{
						changed = true;
					}
					else
					{
						foreach (Alarm alarm in tag.Alarms.Values)
						{
							if (alarm.UpdatedDate != tagClone.Alarms[alarm.AlarmGuid].UpdatedDate)
							{
								changed = true;
								break;
							}
							else
							{
								foreach (AlarmTest alarmTest in alarm.AlarmTests.Values)
								{
									if (alarmTest.UpdatedDate != tagClone.Alarms[alarm.AlarmGuid].AlarmTests[alarmTest.AlarmTestGuid].UpdatedDate)
									{
										changed = true;
										break;
									}
								}
							}

							if (changed)
							{
								break;
							}
						}
					}

					if (changed)
					{
						PointTag.Copy(tag, tagClone);
					}
					else
					{
						if (tag.Value is ValueType)
						{
							tagClone.Value = tag.Value;
						}
						else
						{
							tagClone.ValueXml = tag.ValueXml;
						}

						tagClone.Status = tag.Status;
						tagClone.ServerTimeStamp = tag.ServerTimeStamp;
						tagClone.SourceTimeStamp = tag.SourceTimeStamp;
					}
					tagDictionary.Add(tagClone.PointTagGuid, tagClone);
				}
				else
				{
					tagDictionary.Add(tag.PointTagGuid, tag);
				}
			}

			pTo.Tags = tagDictionary;

			var propertyDictionary = new Dictionary<Guid, PointProperty>();

			foreach (var property in pFrom.Properties.Values)
			{
				PointProperty propertyClone = null;
				if (pTo.Properties.TryGetValue(property.PointPropertyGuid, out propertyClone))
				{
					if (property.UpdatedDate != propertyClone.UpdatedDate)
					{
						propertyClone.ValueXml = property.ValueXml;
						propertyClone.UpdatedDate = property.UpdatedDate;
					}
					propertyDictionary.Add(propertyClone.PointPropertyGuid, propertyClone);
				}
				else
				{
					propertyDictionary.Add(property.PointPropertyGuid, property);
				}
			}

			pTo.Properties = propertyDictionary;

			pTo.UpdatedDate = pFrom.UpdatedDate;

		}

		#endregion

		#region Properties

		[EntityImportExportAttribute("POINTID*", 100, "POINTID")]
		[FMExposedSetting("Point ID", ModifyDisabled = false)]
		public string PointId
		{
			get
			{
				return this.ID;
			}

			set
			{
				this.ID = value;
			}
		}

		[EntityImportExportAttribute("SITE*", 200, "SiteGuid")]
		[XmlIgnore]
		[FMPersistedField]
		public override Guid SiteGuid { get { return base._SiteGuid; } set { base._SiteGuid = value; } }

		[EntityImportExportAttribute("POINTGUID", 200, "PointGuid")]
		[FMPersistedField]
		public Guid PointGuid {	get { return this.IdentityGuid; } set { base.IdentityGuid = value; }	}

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
      public double StandardDensityMax { get { return base.DensityMaximum; }  }

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

		[FMExposedSetting("Site Name", ModifyDisabled = true)]
		[FMPersistedField(ReadOnly = true)]
		public override string SiteID { get { return base._SiteID; } set { base._SiteID = value; } }

		[FMExposedSetting("Site Number", ModifyDisabled = true)]
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string SiteNumber { get; set;  }


		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string PointType { get; set; }

		[EntityImportExportWorksheet("TAGS", "TAGID*")]
		[DataMember]
		public Dictionary<Guid, PointTag> Tags { get; set; }

		[DataMember]
		public Dictionary<Guid, ModuleToPointTemplateMap> ModuleInstances { get; set; }

		[EntityImportExportWorksheet("SETTINGS", "SETTINGID*")]
		[DataMember]
		public Dictionary<Guid, PointProperty> Properties { get; set; }

		[EntityImportExportAttribute("DESCRIPTION", 100, "DESCRIPTION")]
		[DataMember]
		[FMPersistedField]
		[FMExposedSetting("Point Description", ModifyDisabled = false)]
		public string Description { get; set; }

		[EntityImportExportAttribute("ENABLED", 100, "ENABLED")]
		[DataMember]
		[FMExposedSetting("Point Enabled")]
		[FMPersistedField(DefaultValue = false)]
		public bool Enabled { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Standard { get; set; }

		[DataMember]
		[FMPersistedField]
		public int? ExecutionInterval { get; set; }


		[EntityImportExportAttribute("POINTTEMPLATEGUID", 200, "PointTemplateGuid")]
		[DataMember]
		[FMPersistedField]
		public Guid PointTemplateGuid { get; set; }

		[EntityImportExportAttribute("TEMPLATENAME", 100, "TEMPLATENAME")]
		[DataMember]
		[FMPersistedField("TemplateName", ReadOnly = true)]
		public string TemplateName { get; private set; }

		[EntityImportExportAttribute("NOTES", 100, "NOTES")]
		[DataMember]
		[FMPersistedField]
		public string Notes { get; set; }

		[DataMember]
		public ApplicationStringMapCollectionClass PointCategoryCollection;

      [EntityImportExportAttribute("PROFILEIMAGEID", 100, "PROFILEIMAGEID")]
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string ProfileImageID { get; set; }

		[EntityImportExportAttribute("PROFILEIMAGEGUID", 200, "PROFILEIMAGEGUID")]
		[DataMember]
		[FMPersistedField]
		public Guid? ProfileImageGuid { get; set; }

		[EntityImportExportAttribute("PRODUCTID", 100, "PRODUCTID")]
		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		[FMExposedSetting("Product")]
		public string ProductID { get; set; }

		[EntityImportExportAttribute("PRODUCTGUID", 200, "PRODUCTGUID")]
		[DataMember]
		[FMPersistedField]
		public Guid? ProductGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		[FMExposedSetting("Product Description", ModifyDisabled = false)]
		public string ProductDescription { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public Guid? DefaultDrawingGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string DefaultDrawingID { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid? OverrideDefaultDrawingGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public string OverrideDefaultDrawingID { get; set; }


		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public int PointTemplateVersion { get; set; }

		[FMExposedSetting("Point Detail", ModifyDisabled = true)]
		[EntityImportExportAttribute("POINTDETAILDRAWINGID", 100, "POINTDETAILDRAWINGID")]
		public string PointDetailDrawingID
		{
			get
			{
				return (this.OverrideDefaultDrawingGuid.HasValue == false)
					? this.DefaultDrawingID
					: this.OverrideDefaultDrawingID;
			}
			set
			{
				this.OverrideDefaultDrawingID = value;
			}
		}


		[EntityImportExportAttribute("POINTDETAILDRAWINGGUID", 200, "POINTDETAILDRAWINGGUID")]
		public Guid? PointDetailDrawingGuid
		{
			get
			{
				return (this.OverrideDefaultDrawingGuid.HasValue == false)
					? this.DefaultDrawingGuid
					: this.OverrideDefaultDrawingGuid;
			}
			set
			{
				this.OverrideDefaultDrawingGuid = value;
			}
		}


		static string PointUpdateInitiatedKey = "Point Update Initiated";
		public static AlarmAndEventDescriptorClass PointUpdateInitiatedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.PointManagerKey, PointUpdateInitiatedKey);

		static string PointUpdateFailedKey = "Point Update Failed";
		public static AlarmAndEventDescriptorClass PointUpdateFailedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.PointManagerKey, PointUpdateFailedKey);

		static string PointUpdateCompletedKey = "Point Update Completed";
		public static AlarmAndEventDescriptorClass PointUpdateCompletedDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.PointManagerKey, PointUpdateCompletedKey);



		#endregion

		#region RuntimeOnlyMembers
		/// <summary>
		/// In general this should not be set outside the actual point execution
		/// process.  Queue a point for execution using the PointExecutionQueuer class.
		/// </summary>
		public bool NeedsCalculation;

		#endregion

		#region Public Methods

		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			 {
				AlarmAndEventDescriptorClass[] Descriptors ={  PointUpdateInitiatedDescriptor,
																			  PointUpdateFailedDescriptor,
																			  PointUpdateCompletedDescriptor
																				};

				return Descriptors;
			}
		}

		public bool PointTagModuleReference(Guid pointTemplateTagGuid)
		{
			foreach (var moduleInstance in this.ModuleInstances.Values)
			{
				foreach (var tagToModule in moduleInstance.ModuleToPointTemplateData.TagToModules)
				{
					if (tagToModule.TagGuid == pointTemplateTagGuid)
					{
						return true;
					}
				}
			}

			return false;
		}



		public AlarmAndEventLogClass PointUpdateInitiatedEvent(string PointID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PointUpdateInitiatedDescriptor);
			AlarmAndEventLog.AssociatedData = PointID;
			return (AlarmAndEventLog);
		}

		public AlarmAndEventLogClass PointUpdateFailedEvent(string PointID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PointUpdateFailedDescriptor);
			AlarmAndEventLog.AssociatedData = PointID;
			return (AlarmAndEventLog);
		}

		public AlarmAndEventLogClass PointUpdateCompletedEvent(string PointID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PointUpdateCompletedDescriptor);
			AlarmAndEventLog.AssociatedData = PointID;
			return (AlarmAndEventLog);
		}



		public override void Reset()
		{
			this.Init();
		}

		public List<PointValue> GetExposedSettings()
		{
				var pointValues = new List<PointValue>();

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

					var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = this.PointGuid, PointValueType = PointValueType.Point, PropertyID = propertyInfo.Name };

					var pointValue = new PointValue(pointValueIdentifier, this);
					pointValues.Add(pointValue);
				}

				return pointValues;
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

					var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = this.PointGuid, PointValueType = PointValueType.Point, PropertyID = propertyInfo.Name };
					pointValueIddentifiers.Add(pointValueIdentifier);
				}

				return pointValueIddentifiers;
		}

		public string GetExposedSettingID(PointValueIdentifier pointValueIdentifier)
		{
			return this.GetExposedSettingIDFilterByType(pointValueIdentifier, false, string.Empty, PointValueFieldType.VALUE);
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


		public string GetExposedSettingIDFilterByType(PointValueIdentifier pointValueIdentifier, bool filter, string dataTypeString, PointValueFieldType fieldFilter)
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
				if (!filter ||
				(fieldFilter == PointValueFieldType.VALUE && propertyInfo.PropertyType.FullName == dataTypeString) ||
				fieldFilter == PointValueFieldType.ID ||
				fieldFilter == PointValueFieldType.TIMESTAMP ||
				fieldFilter == PointValueFieldType.UNITS)
				{
					var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;
					ID = fmExposedSetting.ID;
					break;
				}
			}

			return ID;
		}

		public List<PointProperty> GetPropertiesForModuleInstance(ModuleToPointTemplateMap modInstance)
		{
			var ret = new List<PointProperty>();
			foreach (var propertyToModule in modInstance.ModuleToPointTemplateData.PropertyToModules)
			{
				ret.Add(this.Properties.Values.Single(x => x.PointTemplatePropertyGuid == propertyToModule.PropertyGuid));
			}
			return ret;
		}

		public List<PointTag> GetTagsForModuleInstance(ModuleToPointTemplateMap modInstance)
		{
			var ret = new List<PointTag>();
			foreach (var tagToModule in modInstance.ModuleToPointTemplateData.TagToModules)
			{
				ret.Add(this.Tags.Values.Single(x => x.PointTemplateTagGuid == tagToModule.TagGuid));
			}
			return ret;
		}

		public Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPointFilterByType(PointValueType valueType, bool filter, string dataTypeString, PointValueFieldType fieldFilter)
		{

			var pointValueIdentifierDictionary = new Dictionary<PointValueIdentifier, string>();

			if (valueType == PointValueType.Tag
			|| valueType == PointValueType.All)
			{
				foreach (var pointTag in this.Tags.Values)
				{
					if (!filter ||
						(fieldFilter == PointValueFieldType.VALUE && pointTag.ValueTypeString == dataTypeString) ||
						(fieldFilter == PointValueFieldType.ALARMSTATUS && pointTag.Alarms.Count > 0) ||
						fieldFilter == PointValueFieldType.ID ||
						fieldFilter == PointValueFieldType.TIMESTAMP ||
						fieldFilter == PointValueFieldType.UNITS)
					{
						pointValueIdentifierDictionary.Add(new PointValueIdentifier(pointTag), pointTag.ID);
					}
				}
			}

			if (valueType == PointValueType.Setting
			|| valueType == PointValueType.All)
			{

				foreach (var pointProperty in this.Properties.Values)
				{
					foreach (var pointValueIdentifier in pointProperty.GetExposedSettingPointValueIdentifiersFilterByType(filter, dataTypeString, fieldFilter))
					{
						pointValueIdentifierDictionary.Add(pointValueIdentifier, pointProperty.GetExposedSettingID(pointValueIdentifier));
					}
				}
			}

			if (valueType == PointValueType.Point
			|| valueType == PointValueType.All)
			{
				foreach (var pointValueIdentifier in this.GetExposedSettingPointValueIdentifiers())
				{
					var pointValueId = this.GetExposedSettingIDFilterByType(pointValueIdentifier, filter, dataTypeString, fieldFilter);
					if (pointValueId != string.Empty)
					{
						pointValueIdentifierDictionary.Add(pointValueIdentifier, pointValueId);
					}
				}
			}

			return pointValueIdentifierDictionary;
		}

		#endregion

		#region Public static methods
		public static void GetSQL(SqlCommand cmd, Guid pointGuid)
		{
			cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber, d1.ID AS DefaultDrawingID, d2.ID AS OverrideDefaultDrawingID, pic.ID AS ProfileImageID"
										+ " FROM dbo.tblPoint p"
										+ " LEFT JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
										+ " LEFT JOIN tblPointTemplate pt ON p.PointTemplateGuid = pt.PointTemplateGuid"
										+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
									   + " LEFT JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
										+ " LEFT JOIN tblDrawings d1 ON d1.DrawingGuid = pt.DefaultDrawingGuid"
										+ " LEFT JOIN tblDrawings d2 ON d2.DrawingGuid = p.OverrideDefaultDrawingGuid"
										+ " LEFT JOIN tblPictures pic ON pic.PictureGuid = p.ProfileImageGuid"
										+ " WHERE p.PointGuid = @PointGuid";

			cmd.Parameters.AddWithValue( "@PointGuid", pointGuid );
		}


		public static void GetListSQL(SqlCommand cmd, List<Guid> pointGuidList)
		{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber, d1.ID AS DefaultDrawingID, d2.ID AS OverrideDefaultDrawingID, pic.ID AS ProfileImageID"
												+ " FROM dbo.tblPoint p"
												+ " LEFT JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
												+ " LEFT JOIN tblPointTemplate pt ON p.PointTemplateGuid = pt.PointTemplateGuid"
												+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
												+ " LEFT JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
												+ " LEFT JOIN tblDrawings d1 ON d1.DrawingGuid = pt.DefaultDrawingGuid"
												+ " LEFT JOIN tblDrawings d2 ON d2.DrawingGuid = p.OverrideDefaultDrawingGuid"
												+ " LEFT JOIN tblPictures pic ON pic.PictureGuid = p.ProfileImageGuid"
												+ " INNER JOIN @PointTable ptbl ON ptbl.Guid = p.PointGuid"
												+ " WHERE p._RowVersion < MIN_ACTIVE_ROWVERSION()";

				var pointTable = new DataTable();
				pointTable.Columns.Add("Guid", typeof(Guid));
				foreach (var pointGuid in pointGuidList)
				{
					var row = pointTable.NewRow();
					row[0] = pointGuid;

					pointTable.Rows.Add(row);
				}

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTable", SqlDbType.Structured);
				tableValuedParameter.Value = pointTable;
				tableValuedParameter.TypeName = "dbo.GuidListType";
		}

		public static void EnumerateBySiteSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber, d1.ID AS DefaultDrawingID, d2.ID AS OverrideDefaultDrawingID, pic.ID AS ProfileImageID"
								+ " FROM dbo.tblPoint p"
								+ " LEFT JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
								+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
								+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
								+ " LEFT JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
								+ " LEFT JOIN tblDrawings d1 ON d1.DrawingGuid = pt.DefaultDrawingGuid"
								+ " LEFT JOIN tblDrawings d2 ON d2.DrawingGuid = p.OverrideDefaultDrawingGuid"
								+ " LEFT JOIN tblPictures pic ON pic.PictureGuid = p.ProfileImageGuid"
								+ " WHERE p.SiteGuid = @SiteGuid AND p._RowVersion < MIN_ACTIVE_ROWVERSION() ORDER BY p.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

				public static void EnumerateBySiteSQLFiltered(SqlCommand cmd, Guid siteGuid, Guid userGuid, PointGroupFilterRules pointFilter)
				{

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
			cmd.Parameters.AddWithValue("@PointName", "%" + pointFilter.point_name + "%");
			cmd.Parameters.AddWithValue("@ProductName", "%" + pointFilter.product_name + "%");

			var pointCategory = new DataTable();
			pointCategory.Columns.Add("Guid", typeof(Guid));
			foreach (var pointCategoryGuid in pointFilter.point_category)
			{
				var row = pointCategory.NewRow();
				row[0] = pointCategoryGuid;
				pointCategory.Rows.Add(row);
			}

			SqlParameter pointCategoryParameter = cmd.Parameters.Add("@PointCategoryTable", SqlDbType.Structured);
			pointCategoryParameter.Value = pointCategory;
			pointCategoryParameter.TypeName = "dbo.GuidListType";

			var pointType = new DataTable();
			pointType.Columns.Add("Guid", typeof(Guid));
			foreach (var pointTypeGuid in pointFilter.point_type)
			{
				var row = pointType.NewRow();
				row[0] = pointTypeGuid;
				pointType.Rows.Add(row);
			}

			SqlParameter pointTypeParameter = cmd.Parameters.Add("@PointTypeTable", SqlDbType.Structured);
			pointTypeParameter.Value = pointType;
			pointTypeParameter.TypeName = "dbo.GuidListType";

			var productGroup = new DataTable();
			productGroup.Columns.Add("Guid", typeof(Guid));
			foreach (var productGroupGuid in pointFilter.product_group)
			{
				var row = productGroup.NewRow();
				row[0] = productGroupGuid;
				productGroup.Rows.Add(row);
			}

			SqlParameter productGroupParameter = cmd.Parameters.Add("@ProductGroupTable", SqlDbType.Structured);
			productGroupParameter.Value = productGroup;
			productGroupParameter.TypeName = "dbo.GuidListType";

			cmd.CommandText = "SET NOCOUNT ON" + "\n"
			                  + " DECLARE @PointAccessGroupGuidTable TABLE(PointAccessGroupGuid UniqueIdentifier)" + "\n"
			                  + " INSERT INTO @PointAccessGroupGuidTable" + "\n"
			                  + " SELECT DISTINCT pagtug.PointAccessGroupGuid FROM map.tblUserToGroup utg" + "\n"
			                  + " INNER JOIN map.tblPointAccessGroupToUserGroup pagtug ON pagtug.UserGroupGuid = utg.GroupGuid"
			                  + "\n" + " WHERE utg.SiteGuid = @SiteGuid" + "\n" + " AND utg.UserGuid = @UserGuid" + "\n"
			                  + " IF OBJECT_ID('tempdb.#PointTable') IS NOT NULL" + "\n" + " DROP TABLE tempdb.#PointTable"
			                  + "\n" + " CREATE TABLE tempdb.#PointTable" + "\n" + " (PointGuid UniqueIdentifier," + "\n"
			                  + " PointAccessGroupGuid UniqueIdentifier)" + "\n"
			                  + " INSERT INTO #PointTable SELECT DISTINCT PointGuid, PointAccessGroupGuid FROM" + "\n"
			                  + " (SELECT p.PointGuid, pagtpt.PointAccessGroupGuid FROM dbo.tblPoint p" + "\n"
			                  + " INNER JOIN map.tblPointAccessGroupToPointTemplate pagtpt ON pagtpt.PointTemplateGuid = p.PointTemplateGuid"
			                  + "\n"
			                  + " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtpt.PointAccessGroupGuid"
			                  + "\n" + " WHERE p.SiteGuid = @SiteGuid" + "\n" + " UNION" + "\n"
			                  + " SELECT p.PointGuid, pagtp.PointAccessGroupGuid FROM dbo.tblPoint p" + "\n"
			                  + " INNER JOIN map.tblPointAccessGroupToPoint pagtp ON pagtp.PointGuid = p.PointGuid" + "\n"
			                  + " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtp.PointAccessGroupGuid"
			                  + "\n" + " WHERE p.SiteGuid = @SiteGuid) s" + "\n";

			cmd.CommandText += "SELECT DISTINCT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber " + "\n"
									+ " FROM dbo.tblPoint p" + "\n"
									+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid" + "\n"
									+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid" + "\n"
									+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid" + "\n"
									+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid" + "\n";


			// filter by point type
			if (pointFilter.point_type.Count > 0)
			{
				cmd.CommandText += " JOIN @PointTypeTable PTT " + "\n"
										+ "ON PTT.guid = pt.PointTemplateTypeApplicationStringGuid " + "\n";
			}

			// filter by point category
			if (pointFilter.point_category.Count > 0)
			{
				cmd.CommandText += " JOIN map.tblApplicationStringToPointCategory PC" + "\n"
										+ " ON PC.PointGuid = p.PointGuid" + "\n"
										+ " JOIN @PointCategoryTable PCT " + "\n"
										+ " ON PCT.guid = PC.ApplicationStringGuid " + "\n";
			}

			// filter by product group
			if (pointFilter.product_group.Count > 0)
			{
				cmd.CommandText += " JOIN map.tblProductToProductGroup PPG" + "\n"
										+ " ON PPG.ProductGuid = p.ProductGuid" + "\n"
										+ " JOIN @ProductGroupTable PGT " + "\n"
										+ " ON PGT.guid = PPG.AssignedToApplicationStringGuid " + "\n";
			}

			// filter by product name
			if (!string.IsNullOrEmpty(pointFilter.product_name)) 
			{
				cmd.CommandText += " JOIN tblProducts Prod" + "\n"
										+ " ON Prod.ProductGuid = p.ProductGuid" + "\n"
										+ " AND Prod.ProductID like @ProductName" + "\n";
			}

			cmd.CommandText += " WHERE p.SiteGuid = @SiteGuid " + "\n";
			cmd.CommandText += " AND EXISTS (SELECT 1 FROM #PointTable pt WHERE pt.pointGuid = p.pointGuid) " + "\n";

			// Filter by PointName
			if ( !string.IsNullOrEmpty( pointFilter.point_name) ) { 
			cmd.CommandText += " AND p.ID Like @PointName " + "\n";
			}

			cmd.CommandText += " AND p._RowVersion < MIN_ACTIVE_ROWVERSION() ORDER BY p.ID" + "\n";

		}


		public static void EnumerateByPointPropertyListSQL(SqlCommand cmd, List<Guid> pointPropertyGuidList)
		{
			cmd.CommandText = "SELECT DISTINCT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber  FROM @PointPropertyTable pptbl"
									+ " LEFT JOIN tblPointProperty pp on pptbl.Guid = pp.PointPropertyGuid"
									+ " LEFT JOIN tblPoint p ON p.PointGuid = pp.PointGuid"
									+ " LEFT JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
									+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
									+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
									+ " LEFT JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid";

				var pointPropertyTable = new DataTable();
			pointPropertyTable.Columns.Add("Guid", typeof(Guid));
			foreach (var pointPropertyGuid in pointPropertyGuidList)
			{
				var row = pointPropertyTable.NewRow();
				row[0] = pointPropertyGuid;

				pointPropertyTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointPropertyTable", SqlDbType.Structured);
			tableValuedParameter.Value = pointPropertyTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}



		public static void EnumerateActiveAlarmsBySiteSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber FROM dbo.tblPoint p"
									+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
									+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
									+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
									+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
									+ " INNER JOIN (Select Distinct(p1.PointGuid) FROM tblPoint p1"
									+ " INNER JOIN tblPointTag tag ON tag.PointGuid = p1.PointGuid"
									+ " INNER JOIN tblAlarm a on a.InputTagGuid = tag.PointTagGuid"
									+ " INNER JOIN tblAlarmTest at on at.AlarmGuid = a.AlarmGuid"
									+ " INNER JOIN tblPointTagAlarmStatus ptas on at.AlarmTestGuid = ptas.AlarmTestGuid"
									+ " WHERE p1.SiteGuid = @SiteGuid AND p1._RowVersion < MIN_ACTIVE_ROWVERSION() AND(ptas.Acknowledged = 0 OR(ptas.Acknowledged = 1 AND ptas.AlarmTestFailed = 1))"
									+ " AND a.Suppressed = 0 AND a.ShelvedOneShot = 0  AND (a.ShelvedEndTimeStamp is null OR a.ShelvedEndTimeStamp < SYSDATETIMEOFFSET())"
									+ " )v ON v.PointGuid = p.PointGuid ORDER BY p.ID";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public static void EnumerateByPointTemplateGuidSQL(SqlCommand cmd, Guid pointTemplateGuid)
		{
				cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
												+ " FROM dbo.tblPoint P"
												+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
												+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
												+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
												+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
												+ " WHERE p.PointTemplateGuid = @PointTemplateGuid AND p._RowVersion < MIN_ACTIVE_ROWVERSION() ORDER BY p.ID";

				cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		public static void EnumerateByPointTemplateGuidsSQL(SqlCommand cmd, SecurityClass security, Guid[] pointTemplateGuids)
		{
			string pointTemplateGuidList = "(";

			foreach (Guid pointTemplateGuid in pointTemplateGuids)
			{
				pointTemplateGuidList += "'" + pointTemplateGuid.ToString() + "',";
			}

			if (pointTemplateGuidList.Length > 1)
			{
				pointTemplateGuidList = pointTemplateGuidList.Remove(pointTemplateGuidList.Length - 1);
			}

			pointTemplateGuidList += ")";

			cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
											+ " FROM dbo.tblPoint P"
											+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
											+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
											+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
											+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
											+ " WHERE s.SiteGuid = @SiteGuid AND p.PointTemplateGuid IN " + pointTemplateGuidList + " AND p._RowVersion < MIN_ACTIVE_ROWVERSION() ORDER BY p.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
		}


		public static void EnumerateBySiteAndPointTemplateSQL(SqlCommand cmd, Guid siteGuid, Guid pointTemplateGuid)
		{
				cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
										+ " FROM dbo.tblPoint p"
										+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
										+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
										+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
										+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
										+ " WHERE p.SiteGuid = @SiteGuid AND p.PointTemplateGuid = @PointTemplateGuid AND p._RowVersion < MIN_ACTIVE_ROWVERSION() ORDER BY p.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		public static void EnumerateForSummarySQL( SqlCommand cmd, Guid siteGuid, Guid userGuid, bool applyPointAccess = false, string propertyID = null)
		{

			if (applyPointAccess)
			{
				cmd.CommandText += " DECLARE @PointAccessGroupGuidTable TABLE(PointAccessGroupGuid UniqueIdentifier)";

				cmd.CommandText += " INSERT INTO @PointAccessGroupGuidTable"
								   + " SELECT pagtug.PointAccessGroupGuid FROM map.tblUserToGroup utg"
								   + " INNER JOIN map.tblPointAccessGroupToUserGroup pagtug ON pagtug.UserGroupGuid = utg.GroupGuid"
								   + " WHERE utg.UserGuid = @UserGuid";

				cmd.CommandText += " IF OBJECT_ID('tempdb.#PointTable') IS NOT NULL" + " DROP TABLE tempdb.#PointTable"
								   + " CREATE TABLE tempdb.#PointTable (PointGuid UniqueIdentifier,PointAccessGroupGuid UniqueIdentifier)";

				cmd.CommandText += " INSERT INTO #PointTable SELECT DISTINCT PointGuid, PointAccessGroupGuid FROM"
								   + " (SELECT p.PointGuid, pagtpt.PointAccessGroupGuid FROM dbo.tblPoint p"
								   + " INNER JOIN map.tblPointAccessGroupToPointTemplate pagtpt ON pagtpt.PointTemplateGuid = p.PointTemplateGuid"
								   + " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtpt.PointAccessGroupGuid"
								   + " WHERE p.SiteGuid = @SiteGuid" + " UNION"
								   + " SELECT p.PointGuid, pagtp.PointAccessGroupGuid FROM dbo.tblPoint p"
								   + " INNER JOIN map.tblPointAccessGroupToPoint pagtp ON pagtp.PointGuid = p.PointGuid"
								   + " INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtp.PointAccessGroupGuid"
								   + " WHERE p.SiteGuid = @SiteGuid) s ";
			}

			cmd.CommandText +=
				"SELECT DISTINCT p.ID, p.PointGuid, p.Description, p.ProfileImageGuid, p.PointTemplateGuid, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
				+ " FROM dbo.tblPoint p" + " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
				+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
				+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
				+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid";

			if (!string.IsNullOrWhiteSpace(propertyID))
			{
				cmd.CommandText += " INNER JOIN tblPointProperty pp  ON p.PointGuid = pp.PointGuid";
			}

			cmd.CommandText += " WHERE p.SiteGuid = @SiteGuid "
							+ " AND p._RowVersion < MIN_ACTIVE_ROWVERSION()";

			if (applyPointAccess)
			{
				cmd.CommandText += " AND EXISTS ( SELECT 1 FROM  #PointTable pgt WHERE pgt.PointGuid = p.PointGuid ) ";
			}

			if (!string.IsNullOrWhiteSpace(propertyID))
			{
				cmd.CommandText += " AND pp.ID = @PropertyID";
			}

			cmd.CommandText += " ORDER BY p.ID";

			if (applyPointAccess)
			{
				cmd.Parameters.AddWithValue("@UserGuid", userGuid);
			}

			cmd.Parameters.AddWithValue( "@SiteGuid", siteGuid );

			if (!string.IsNullOrWhiteSpace(propertyID))
			{
				cmd.Parameters.AddWithValue("@PropertyID", propertyID);
			}

		}
		public static void EnumerateEnabledBySiteSQL( SqlCommand cmd, Guid siteGuid )
		{
			cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
								+ " FROM dbo.tblPoint p"
								+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
								+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
								+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
								+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
								+ " WHERE p.SiteGuid = @SiteGuid AND p._RowVersion < MIN_ACTIVE_ROWVERSION()"
								+ "	AND p.Enabled=1"
								+ " ORDER BY p.ID";

			cmd.Parameters.AddWithValue( "@SiteGuid", siteGuid );
		}

		public static void EnumerateEnabledSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
								+ " FROM dbo.tblPoint p"
								+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
								+ " JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
								+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
								+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
								+ " WHERE p._RowVersion < MIN_ACTIVE_ROWVERSION()"
								+ "	AND p.Enabled=1"
								+ " ORDER BY p.ID";

		}

		public static void EnumerateEnabledForPointServiceSQL(SqlCommand cmd, string hostname, int startIndex, int count)
		{
				cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
										+ " FROM dbo.tblPoint p"
										+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
										+ " JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
										+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
										+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
										+ " INNER JOIN map.tblPointToPointService m ON m.PointGuid = p.PointGuid"
										+ " INNER JOIN dbo.tblPointService s ON s.PointServiceGuid = m.PointServiceGuid"
										+ " WHERE s.Hostname = @Hostname AND p._RowVersion < MIN_ACTIVE_ROWVERSION()"
										+ " AND p.Enabled = 1"
										+ " ORDER BY p.CreatedDate ASC OFFSET @StartingRec ROWS FETCH NEXT @NumRecs ROWS ONLY";

				cmd.Parameters.AddWithValue("@Hostname", hostname);
				cmd.Parameters.AddWithValue("@StartingRec", startIndex);
				cmd.Parameters.AddWithValue("@NumRecs", count);
		}

		public static void CountEnabledForSimulatorSQL(SqlCommand cmd, string opcUaEndPoint)
		{
				cmd.CommandText = "Select COUNT(p.PointGuid) AS PointCount"
						+ " FROM dbo.tblPoint P"
						+ " INNER JOIN"
						+ " ("
						+ "	Select Distinct(t.PointGuid) from tblPointTag t"
						+ "	INNER JOIN tblOpcUaServer os"
						+ "	ON os.OpcUaServerGuid = t.OpcUaServerGuid"
						+ "	where os.ServerEndPoint = @OpcUaEndPoint"
						+ " ) cp"
						+ " ON cp.PointGuid = p.PointGuid"
						+ " AND p.Enabled = 1";
				cmd.Parameters.AddWithValue("@OpcUaEndPoint", opcUaEndPoint);
		}

		public static void EnumerateEnabledForSimulatorSQL(SqlCommand cmd, string opcUaEndPoint, int startIndex, int count)
		{
				cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
										+ " FROM dbo.tblPoint p"
										+ " LEFT JOIN dbo.tblSites S ON S.SiteGuid = p.SiteGuid"
										+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
										+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
										+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
										+ " INNER JOIN"
										+ " ("
										+ "	Select Distinct(t.PointGuid) from tblPointTag t"
										+ "	INNER JOIN tblOpcUaServer os"
										+ "	ON os.OpcUaServerGuid = t.OpcUaServerGuid"
										+ "	where os.ServerEndPoint = @OpcUaEndPoint"
										+ " ) cp"
										+ " ON cp.PointGuid = p.PointGuid"
										+ " AND p.Enabled = 1"
										+ " ORDER BY S.ID, p.ID ASC OFFSET @StartingRec ROWS FETCH NEXT @NumRecs ROWS ONLY";

				cmd.Parameters.AddWithValue("@OpcUaEndPoint", opcUaEndPoint);
				cmd.Parameters.AddWithValue("@StartingRec", startIndex);
				cmd.Parameters.AddWithValue("@NumRecs", count);
		}

		public static void EnumerateSQL( SqlCommand cmd )
		{
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = "SELECT p.* FROM dbo.tblPoint p";
		}

		public static void EnumerateByPointListSQL(SqlCommand cmd, List<Guid> pointGuidList)
		{
			cmd.CommandText = "SELECT p.*, pt.ID as TemplateName, pt.DefaultDrawingGuid, aps.ID AS PointType, pd.ProductID, pd.Description AS ProductDescription, s.ID AS SiteID, s.Number AS SiteNumber "
										+ " FROM dbo.tblPoint p"
										+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid"
										+ " JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
										+ " LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
										+ " LEFT OUTER JOIN tblApplicationString aps ON pt.PointTemplateTypeApplicationStringGuid = aps.ApplicationStringGuid"
										+ " INNER JOIN @PointTable ptbl ON ptbl.Guid = p.PointGuid";

			var pointTable = new DataTable();
			pointTable.Columns.Add("Guid", typeof(Guid));
			foreach (var pointGuid in pointGuidList)
			{
					var row = pointTable.NewRow();
					row[0] = pointGuid;

					pointTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTable", SqlDbType.Structured);
			tableValuedParameter.Value = pointTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}


		/// <summary>
		/// This method will populate the SQL command to retrieve product info for all points.
		/// </summary>
		/// <param name="cmd">The SQL Command to populate</param>
		public static void EnumeratePointProductInfoSql(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT p.PointGuid, pd.ProductColor, pd.PatternColor, pd.PatternNumber"
								+ " FROM dbo.tblPoint p LEFT JOIN dbo.tblProducts pd ON pd.ProductGuid = p.ProductGuid"
								+ " INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid";
		}


		public static void EnumerateWellKnownIdentitySQL(SqlCommand cmd, Guid siteGuid, Guid pointTypeGuid, List<Guid> wellKnownIdentityGuidList)
		{
			cmd.CommandText = "SELECT p.ID, pttag.WellKnownIdentityGuid, ptag.PointTagGuid FROM tblPoint p"
									+ " LEFT JOIN tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid"
									+ " LEFT JOIN tblPointTag ptag on ptag.PointGuid = p.PointGuid"
									+ " LEFT JOIN tblPointTemplateTag pttag ON pttag.PointTemplateTagGuid = ptag.PointTemplateTagGuid"
									+ " INNER JOIN @WellKnownIdentityGuidTable wki ON wki.Guid = pttag.WellKnownIdentityGuid"
									+ " WHERE p.SiteGuid = @SiteGuid AND pt.PointTemplateTypeApplicationStringGuid = @PointTypeGuid"
									+ " ORDER BY p.ID";

			var wellKnownIdentityGuidTable = new DataTable();
			wellKnownIdentityGuidTable.Columns.Add("Guid", typeof(Guid));
			foreach (var wellKnownIdentityGuid in wellKnownIdentityGuidList)
			{
				var row = wellKnownIdentityGuidTable.NewRow();
				row[0] = wellKnownIdentityGuid;

				wellKnownIdentityGuidTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@WellKnownIdentityGuidTable", SqlDbType.Structured);
			tableValuedParameter.Value = wellKnownIdentityGuidTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTypeGuid", pointTypeGuid);
		}


		#endregion

		#region Private methods

		private Dictionary<string, PointTag> CreatePointTagByIdDictionary()
		{
			var ret = new Dictionary<string, PointTag>();
			foreach (var tag in this.Tags.Values)
			{
				ret.Add(tag.ID, tag);
			}
			return ret;
		}

		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			base.Reset();
			this.Tags				= new Dictionary<Guid, PointTag>();
			this.ModuleInstances	= new Dictionary<Guid, ModuleToPointTemplateMap>();
			this.Properties			= new Dictionary<Guid, PointProperty>();

			this.Description				= string.Empty;
			this.Standard					= false;
			this.ExecutionInterval		= null;
			this.PointTemplateGuid			= Guid.Empty;
			this.PointCategoryCollection	= new ApplicationStringMapCollectionClass();
			this.PointType					= string.Empty;
			this.Notes						= string.Empty;
			this.ProductID = string.Empty;
			this.ProductDescription = string.Empty;
			this.PointTemplateVersion = 0;
		}
		#endregion
	}
}
