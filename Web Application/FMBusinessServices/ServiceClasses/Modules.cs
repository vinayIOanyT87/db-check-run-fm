namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Constants;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using System.Linq;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class Modules : FMServiceBase, IModules, IDependency
	{
		public ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, Module module)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

            // TODO: Check security rights

            // Create identity GUID if needed. (If it is already populated, this is probably coming from import function)
            if (module.IdentityGuid == Guid.Empty)
            {
                module.IdentityGuid = Guid.NewGuid();
            }

            using (var cmd = new SqlCommand())
			{
				module.SetCreationStamp(security);
				module.AutoGenerateInsertProcSQL(cmd, "[dbo].[gsp_ModuleInsertByPK]");
				cmd.Parameters["@ModuleGuid"].Direction = ParameterDirection.InputOutput;

				ConsolidatedDa.ExecuteQuery(security, cmd);

				module.ModuleGuid = new Guid(cmd.Parameters["@ModuleGuid"].Value.ToString());
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(module);
			entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);


			return module.ModuleGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, Module module)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

            // TODO: Check security rights

            var existingModule = this.Get(security, module.ModuleGuid);
			if (existingModule.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Module not found for update.");
			}

			var incrementPointTemplateVersions =
				!string.Equals(module.ModuleScript, existingModule.ModuleScript, StringComparison.Ordinal)
				|| !string.Equals(module.ModuleDataXml, existingModule.ModuleDataXml, StringComparison.Ordinal)
				|| !string.Equals(module.ModuleCalculation, existingModule.ModuleCalculation, StringComparison.Ordinal);

			using (var cmd = new SqlCommand())
			{
				module.SetModifyStamp(security);
				module.AutoGenerateModifyProcSQL(cmd, "[dbo].[gsp_ModuleUpdateByPK]");

				ConsolidatedDa.ExecuteQuery(security, cmd);
			}

			if (module.SiteGuid != existingModule.SiteGuid)
			{
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
					security, module.EntityType, module.IdentityGuid);
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = module.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(module);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}



			var pointTemplates = new PointTemplates();
			var pointTemplateCollection = pointTemplates.EnumerateByModule(security, module.ModuleGuid);

			foreach (var pointTemplate in pointTemplateCollection)
			{


				// Build an alarm dictionary, statusTag vs alarm dictionary, and limitTag vs alarm test dictionary to use in deleting a tag
				var alarmDictionary = new Dictionary<Guid, AlarmTemplate>();
				var stateTagAlarmDictionary = new Dictionary<Guid, AlarmTemplate>();
				var limitTagAlarmTestDictionary = new Dictionary<Guid, AlarmTestTemplate>();

				foreach (var pointTag in pointTemplate.Tags.Values)
				{
					if (pointTag.AlarmTemplates.Any())
					{
						foreach (var alarmTemplate in pointTag.AlarmTemplates.Values)
						{
							alarmDictionary.Add(alarmTemplate.AlarmTemplateGuid, alarmTemplate);

							if (!stateTagAlarmDictionary.ContainsKey(alarmTemplate.AlarmStateTemplateTagGuid))
							{
								stateTagAlarmDictionary.Add(alarmTemplate.AlarmStateTemplateTagGuid, alarmTemplate);
							}

							foreach (var alarmTestTemplate in alarmTemplate.AlarmTestTemplates.Values)
							{
								if (!limitTagAlarmTestDictionary.ContainsKey(alarmTestTemplate.LimitTemplateTagGuid))
								{
									limitTagAlarmTestDictionary.Add(alarmTestTemplate.LimitTemplateTagGuid, alarmTestTemplate);
								}
							}
						}
					}
				}


				var moduleInstanceList = pointTemplate.ModuleInstances.Values.Where(x => x.ModuleGuid == module.ModuleGuid).ToList();

				if (moduleInstanceList == null
				|| moduleInstanceList.Count == 0)
				{
					throw new Exception("Modules Modify - PointTemplate " + pointTemplate.ID + " without Module Instance for Module " + module.ID);
				}

				foreach (var moduleInstance in moduleInstanceList)
				{
					var indexOfModuleID = moduleInstance.ID.IndexOf(module.ID);
					var moduleInstanceName = moduleInstance.ID;

					// Module Instance Name should be prefixed by Module ID followed by space
					if(moduleInstanceList.Count > 1
					&& indexOfModuleID != -1)
					{
						moduleInstanceName = moduleInstance.ID.Remove(indexOfModuleID, module.ID.Length + 1);
					}

					// delete TagToModules that are no longer in the module
					// when the module permits multiple instances the  Property ID will be the Module Instance ID combined with the Parameter Name unless it is Value
					var tagToModuleList = new List<TagToModule>();
					foreach (var tagToModule in moduleInstance.ModuleToPointTemplateData.TagToModules)
					{
						var tag = pointTemplate.Tags[tagToModule.TagGuid];
						var moduleTag = module.ModuleData.ModuleTags.SingleOrDefault(x => (moduleInstanceList.Count == 1) ? x.TagID == tag.ID : (x.ParameterName == "Value") ? moduleInstanceName == tag.ID : moduleInstanceName + " " + x.ParameterName == tag.ID);
						if (moduleTag != null)
						{
							tagToModule.ModuleParameter = moduleTag.ParameterName;
							tagToModuleList.Add(tagToModule);
						}
					}

					moduleInstance.ModuleToPointTemplateData.TagToModules = tagToModuleList.ToArray();


					// Remove any Tags added by Modules and no longer referenced by Module Instances, Alarm State, Alarm Limit
					var tagDictionary = new Dictionary<Guid, PointTemplateTag>();
					foreach (var tag in pointTemplate.Tags.Values)
					{
						if (pointTemplate.ModuleInstances.Values
							 .SelectMany(x => x.ModuleToPointTemplateData.TagToModules)
							 .Any(y => y.TagGuid == tag.PointTemplateTagGuid)
						|| !tag.Module
						|| stateTagAlarmDictionary.ContainsKey(tag.PointTemplateTagGuid)
						|| limitTagAlarmTestDictionary.ContainsKey(tag.PointTemplateTagGuid))
						{
							tagDictionary.Add(tag.PointTemplateTagGuid, tag);
						}
					}

					pointTemplate.Tags = tagDictionary;

					// Remove any Tags no longer referenced by Module Instances and no longer referenced by Alarm
					tagDictionary = new Dictionary<Guid, PointTemplateTag>();
					foreach (var tag in pointTemplate.Tags.Values)
					{
						if (pointTemplate.ModuleInstances.Values
							.SelectMany(x => x.ModuleToPointTemplateData.TagToModules)
							.Any(y => y.TagGuid == tag.PointTemplateTagGuid))
						{
							tagDictionary.Add(tag.PointTemplateTagGuid, tag);
						}

						else if (stateTagAlarmDictionary.ContainsKey(tag.PointTemplateTagGuid))
						{
							var alarmTemplate = stateTagAlarmDictionary[tag.PointTemplateTagGuid];
							if (pointTemplate.Tags.ContainsKey(alarmTemplate.InputTemplateTagGuid))
							{
								tagDictionary.Add(tag.PointTemplateTagGuid, tag);
							}
						}

						else if (limitTagAlarmTestDictionary.ContainsKey(tag.PointTemplateTagGuid))
						{
							var alarmTestTemplate = limitTagAlarmTestDictionary[tag.PointTemplateTagGuid];
							var alarmTemplate = alarmDictionary[alarmTestTemplate.AlarmTemplateGuid];

							if (pointTemplate.Tags.ContainsKey(alarmTemplate.InputTemplateTagGuid))
							{
								tagDictionary.Add(tag.PointTemplateTagGuid, tag);
							}
						}

						else if (!tag.Module)
						{
							tagDictionary.Add(tag.PointTemplateTagGuid, tag);
						}
					}

					pointTemplate.Tags = tagDictionary;

					// delete PropertyToModules that are no longer in the module
					// when the module permits multiple instances the  Property ID will be the Module Instance ID combined with the Property Name
					var propertyToModuleList = new List<PropertyToModule>();
					foreach (var propertyToModule in moduleInstance.ModuleToPointTemplateData.PropertyToModules)
					{
						var property = pointTemplate.Properties[propertyToModule.PropertyGuid];
						var moduleProperty = module.ModuleData.ModuleSettings.SingleOrDefault(x => (moduleInstanceList.Count == 1) ? x.SettingName == property.ID : moduleInstance.ID + " " + x.SettingName == property.ID );
						if (moduleProperty != null)
						{
							propertyToModule.PropertyName = moduleProperty.PropertyName;
							propertyToModuleList.Add(propertyToModule);
						}
					}

					moduleInstance.ModuleToPointTemplateData.PropertyToModules = propertyToModuleList.ToArray();


					// Remove any Properties not longer referenced by ModuleInstances
					var propertyDictionary = new Dictionary<Guid, PointTemplateProperty>();
					foreach (var property in pointTemplate.Properties.Values)
					{
						// check if the property is not being used anywhere else so we can delete it
						if (pointTemplate.ModuleInstances.Values
							.SelectMany(x => x.ModuleToPointTemplateData.PropertyToModules)
							.Any(y => y.PropertyGuid == property.PointTemplatePropertyGuid))
						{
							propertyDictionary.Add(property.PointTemplatePropertyGuid, property);
						}
					}

					pointTemplate.Properties = propertyDictionary;


					// Update tagToModule mapping.
					var tagToModulesList = new List<TagToModule>();
					foreach (var moduleTag in module.ModuleData.ModuleTags)
					{

						if (moduleTag.TagID == pointTemplate.ID)
						{
							throw new Exception(string.Format("ModuleEditor|Tag Name is the same as the Point Template Name for {0}.", pointTemplate.ID));
						}


						var tagToModule = new TagToModule();
						tagToModule.ModuleParameter = moduleTag.ParameterName;

						// if there is Tag with the same name then reuse the Guid and don't create a new one
						string tagID = string.Empty;
						if(moduleInstanceList.Count == 1)
						{
							tagID = moduleTag.TagID;
						}
						else
						{
							if(moduleTag.ParameterName == "Value")
							{
								tagID = moduleInstanceName;
							}
							else
							{
								tagID = moduleInstanceName + " " + moduleTag.ParameterName;
							}
						}
						
						var existingTagList = pointTemplate.Tags.Select(x => x.Value).Where(x => x.ID == tagID).ToList();
						if (existingTagList.Any())
						{
							var tag = existingTagList.FirstOrDefault();

							if (tag.ValueTypeString == moduleTag.DataType && tag.Input == moduleTag.Input)
							{
								// provided module doesn't support multiple instances update the WellKnownIdentityGuid as some may have been changed
								// in the current module version
								if (module.Standard
								&& !module.ModuleData.MultipleInstances)
								{
									tag.WellKnownIdentityGuid = moduleTag.WellKnownIdentityGuid;
								}

								if (ShouldCascadeServerUnits(tag.EngineeringUnitsType, tag.ServerUnits))
								{
									tag.ServerUnits = GetModuleTagServerUnits(moduleTag);
								}

								tagToModule.TagGuid = existingTagList.FirstOrDefault().PointTemplateTagGuid;
							}
							else
							{
								throw new Exception(string.Format("ModuleEditor|Tag Mismatch on Type or Input for {0}.", pointTemplate.ID + "." + tag.ID));
							}
						}
						else
						{
							string valueTypeString = moduleTag.DataType;
							PointTemplateTag.PointTagInputOutputType inputOutputType = moduleTag.InputOutputType;
							bool input = moduleTag.Input;
							EngineeringUnit unit = EngineeringUnit.FmuNone;
							byte decimalPlaces = 0;
							EngineeringUnit serverUnit = EngineeringUnit.FmuNone;
							double maximum = 0.0;
							double minimum = 0.0;

							if (valueTypeString == "System.Double" || valueTypeString == "System.Single")
							{
								switch (moduleTag.UnitType)
								{
									case EngineeringUnitType.FmuDensity:
										unit = pointTemplate.DensityUnit;
										decimalPlaces = pointTemplate.DensityDecimalPlaces;
										serverUnit = pointTemplate.DensityUnit;
										maximum = pointTemplate.DensityMaximum;
										minimum = pointTemplate.DensityMinimum;
										break;
									case EngineeringUnitType.FmuLength:
										unit = pointTemplate.LevelUnit;
										decimalPlaces = pointTemplate.LevelDecimalPlaces;
										serverUnit = pointTemplate.LevelUnit;
										maximum = pointTemplate.LevelMaximum;
										minimum = pointTemplate.LevelMinimum;
										break;
									case EngineeringUnitType.FmuMass:
										unit = pointTemplate.MassUnit;
										decimalPlaces = pointTemplate.MassDecimalPlaces;
										serverUnit = pointTemplate.MassUnit;
										maximum = pointTemplate.MassMaximum;
										minimum = pointTemplate.MassMinimum;
										break;
									case EngineeringUnitType.FmuMassflow:
										unit = pointTemplate.MassFlowUnit;
										decimalPlaces = pointTemplate.MassFlowDecimalPlaces;
										serverUnit = pointTemplate.MassFlowUnit;
										maximum = pointTemplate.MassFlowMaximum;
										minimum = pointTemplate.MassFlowMinimum;
										break;
									case EngineeringUnitType.FmuPressure:
										unit = pointTemplate.PressureUnit;
										decimalPlaces = pointTemplate.PressureDecimalPlaces;
										serverUnit = pointTemplate.PressureUnit;
										maximum = pointTemplate.PressureMaximum;
										minimum = pointTemplate.PressureMinimum;
										break;
									case EngineeringUnitType.FmuTemp:
										unit = pointTemplate.TemperatureUnit;
										decimalPlaces = pointTemplate.TemperatureDecimalPlaces;
										serverUnit = pointTemplate.TemperatureUnit;
										maximum = pointTemplate.TemperatureMaximum;
										minimum = pointTemplate.TemperatureMinimum;
										break;
									case EngineeringUnitType.FmuVelocity:
										unit = pointTemplate.VelocityUnit;
										decimalPlaces = pointTemplate.VelocityDecimalPlaces;
										serverUnit = pointTemplate.VelocityUnit;
										maximum = pointTemplate.VelocityMaximum;
										minimum = pointTemplate.VelocityMinimum;
										break;
									case EngineeringUnitType.FmuVolflow:
										unit = pointTemplate.FlowUnit;
										decimalPlaces = pointTemplate.FlowDecimalPlaces;
										serverUnit = pointTemplate.FlowUnit;
										maximum = pointTemplate.VolumetricFlowMaximum;
										minimum = pointTemplate.VolumetricFlowMinimum;
										break;
									case EngineeringUnitType.FmuVolume:
										unit = pointTemplate.VolumeUnit;
										decimalPlaces = pointTemplate.VolumeDecimalPlaces;
										serverUnit = pointTemplate.VolumeUnit;
										maximum = pointTemplate.VolumeMaximum;
										minimum = pointTemplate.VolumeMinimum;
										break;
									default:
										if (valueTypeString == "System.Double")
										{
											decimalPlaces = 2;
											maximum = moduleTag.Maximum;
											minimum = moduleTag.Minimum;
										}
										else
										{
											decimalPlaces = 2;
											maximum = moduleTag.Maximum;
											minimum = moduleTag.Minimum;
										}
										break;
								}
							}
							else
							{
								maximum = moduleTag.Maximum;
								minimum = moduleTag.Minimum;
							}

							/* Add the module reference to the point template */
							var pointTemplateTag = new PointTemplateTag();
							pointTemplateTag.PointTemplateGuid = pointTemplate.PointTemplateGuid;
							pointTemplateTag.PointTemplateTagGuid = Guid.NewGuid();
							pointTemplateTag.ID = moduleTag.TagID;
							pointTemplateTag.ValueTypeString = valueTypeString;
							pointTemplateTag.Value = moduleTag.Value;
							pointTemplateTag.EngineeringUnitsType = moduleTag.UnitType;
							pointTemplateTag.Units = (moduleTag.ApplyPointTemplateEngineeringUnits) ? unit : moduleTag.Units;
							pointTemplateTag.DecimalPlaces = (moduleTag.ApplyPointTemplateDecimalPlaces) ? decimalPlaces : moduleTag.DecimalPlaces;
							pointTemplateTag.Maximum = (moduleTag.ApplyPointTemplateMaximum) ? maximum : moduleTag.Maximum;
							pointTemplateTag.Minimum = (moduleTag.ApplyPointTemplateMinimum) ? minimum : moduleTag.Minimum;
							pointTemplateTag.InputOutputType = inputOutputType;
							pointTemplateTag.AlarmStatus = false;
							pointTemplateTag.AlarmsEnabled = false;
							pointTemplateTag.Input = input;
							pointTemplateTag.ServerUnits = GetModuleTagServerUnits(moduleTag);
							pointTemplateTag.SiteGuid = security.SiteGuid;
							pointTemplateTag.WellKnownIdentityGuid = moduleTag.WellKnownIdentityGuid;
							pointTemplateTag.ApplyPointTemplateEngineeringUnits = moduleTag.ApplyPointTemplateEngineeringUnits;
							pointTemplateTag.ApplyPointTemplateDecimalPlaces = moduleTag.ApplyPointTemplateDecimalPlaces;
							pointTemplateTag.ApplyPointTemplateMaximum = moduleTag.ApplyPointTemplateMaximum;
							pointTemplateTag.ApplyPointTemplateMinimum = moduleTag.ApplyPointTemplateMinimum;
							pointTemplateTag.InhibitInputOutputTypeConfiguration = moduleTag.InhibitInputOutputTypeConfiguration;
							pointTemplateTag.InhibitOverride = moduleTag.InhibitOverride;
							pointTemplateTag.Module = true;

							// Update Archived flag for all tags in the module
							pointTemplateTag.Archived = true;

							pointTemplate.Tags.Add(pointTemplateTag.PointTemplateTagGuid, pointTemplateTag);

							tagToModule.TagGuid = pointTemplateTag.PointTemplateTagGuid;
						}

						tagToModulesList.Add(tagToModule);
					}

					moduleInstance.ModuleToPointTemplateData.TagToModules = tagToModulesList.ToArray();


					// Attempt to add propertyToModule entries for newly added Settings.
					var propertyToModulesList = new List<PropertyToModule>();
					foreach (var moduleSetting in module.ModuleData.ModuleSettings)
					{
						var propertyName = moduleSetting.SettingName;
						if (module.ID != moduleInstance.ID)
						{
							propertyName = moduleInstance.ID + " " + propertyName;
						}

						if (propertyName == "")
						{
							throw new Exception("ModuleEditor|Missing Settings Name.");
						}
						if (propertyName == pointTemplate.ID)
						{
							throw new Exception(string.Format("ModuleEditor|Setting Name is the same as the Point Template Name for {0}.", pointTemplate.ID));
						}


						var propertyToModule = new PropertyToModule();
						propertyToModule.PropertyName = moduleSetting.PropertyName;

						// if there is another setting/property with the same name then reuse the Guid and don't create a new one
						var existingPropertyList = pointTemplate.Properties.Select(x => x.Value).Where(x => x.ID == propertyName).ToList();
						if (existingPropertyList.Any())
						{
							var property = existingPropertyList.FirstOrDefault();
							if (property.ValueTypeString == moduleSetting.DataType)
							{
								propertyToModule.PropertyGuid = existingPropertyList.FirstOrDefault().PointTemplatePropertyGuid;
							}
							else
							{
								throw new Exception(string.Format("ModuleEditor|Duplicate Setting Names for {0}", pointTemplate.ID + "." + property.ID));
							}
						}
						else
						{
							// create a new point Template Property
							// Add the module reference to the point template 
							var pointTemplateProperty = new PointTemplateProperty();
							pointTemplateProperty.PointTemplateGuid = pointTemplate.PointTemplateGuid;
							pointTemplateProperty.PointTemplatePropertyGuid = Guid.NewGuid();
							pointTemplateProperty.SiteGuid = pointTemplate.SiteGuid;
							pointTemplateProperty.ID = propertyName;
							Type newType = null;
							if (moduleSetting.DataType.StartsWith("System."))
							{
								newType = Type.GetType(moduleSetting.DataType);
							}
							else if (moduleSetting.DataType.StartsWith("FMBusinessObjects."))
							{
								newType = Type.GetType(moduleSetting.DataType + ",FMBusinessObjects", true);
							}

							if (moduleSetting.DataType.StartsWith("System.String"))
							{
								pointTemplateProperty.Value = string.Empty;
							}
							else
							{
								pointTemplateProperty.Value = (Activator.CreateInstance(newType));
							}

							if (pointTemplateProperty.Value is DateTime)
							{
								pointTemplateProperty.Value = DateTime.Today;
							}
							else if (pointTemplateProperty.Value is DateTimeOffset)
							{
								pointTemplateProperty.Value = DateTimeOffset.Now;
							}

							pointTemplate.Properties.Add(pointTemplateProperty.PointTemplatePropertyGuid, pointTemplateProperty);

							propertyToModule.PropertyGuid = pointTemplateProperty.PointTemplatePropertyGuid;
						}


						propertyToModulesList.Add(propertyToModule);
					}

					moduleInstance.ModuleToPointTemplateData.PropertyToModules = propertyToModulesList.ToArray();
				}

				pointTemplates.Modify(security, pointTemplate, incrementPointTemplateVersions);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid moduleGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var module = this.Get(security, moduleGuid);
			if (module.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Module Not Found"));
			}


			var pointTemplates = new PointTemplates();
			var pointTemplateCollection = pointTemplates.EnumerateByModule(security, moduleGuid);

			foreach (var pointTemplate in pointTemplateCollection)
			{
				// Build an alarm dictionary, statusTag vs alarm dictionary, and limitTag vs alarm test dictionary to use in deleting a tag
				var alarmDictionary = new Dictionary<Guid, AlarmTemplate>();
				var stateTagAlarmDictionary = new Dictionary<Guid, AlarmTemplate>();
				var limitTagAlarmTestDictionary = new Dictionary<Guid, AlarmTestTemplate>();

				foreach (var pointTag in pointTemplate.Tags.Values)
				{
					if (pointTag.AlarmTemplates.Any())
					{
						foreach (var alarmTemplate in pointTag.AlarmTemplates.Values)
						{
							alarmDictionary.Add(alarmTemplate.AlarmTemplateGuid, alarmTemplate);

							if (!stateTagAlarmDictionary.ContainsKey(alarmTemplate.AlarmStateTemplateTagGuid))
							{
								stateTagAlarmDictionary.Add(alarmTemplate.AlarmStateTemplateTagGuid, alarmTemplate);
							}

							foreach (var alarmTestTemplate in alarmTemplate.AlarmTestTemplates.Values)
							{
								if (!limitTagAlarmTestDictionary.ContainsKey(alarmTestTemplate.LimitTemplateTagGuid))
								{
									limitTagAlarmTestDictionary.Add(alarmTestTemplate.LimitTemplateTagGuid, alarmTestTemplate);
								}
							}
						}
					}
				}

				var moduleInstanceList = pointTemplate.ModuleInstances.Values.Where(x => x.ModuleGuid == moduleGuid).ToList();

				if (moduleInstanceList.Count == 0)
				{
					throw new Exception("Modules Modify - PointTemplate " + pointTemplate.ID + " without Module Instance for Module Guid " + moduleGuid.ToString());
				}

				foreach (var moduleInstance in moduleInstanceList)
				{
					pointTemplate.ModuleInstances.Remove(moduleInstance.ModuleToPointTemplateGuid);

					var modulePointTemplateMaps = new ModuleToPointTemplateMaps();
					modulePointTemplateMaps.Purge(security, moduleInstance.ModuleToPointTemplateGuid);



					// Remove any Tags added by Modules and no longer referenced by Module Instances, Alarm State, Alarm Limit
					var tagDictionary = new Dictionary<Guid, PointTemplateTag>();
					foreach (var tag in pointTemplate.Tags.Values)
					{
						if (pointTemplate.ModuleInstances.Values
							 .SelectMany(x => x.ModuleToPointTemplateData.TagToModules)
							 .Any(y => y.TagGuid == tag.PointTemplateTagGuid)
						|| !tag.Module
						|| stateTagAlarmDictionary.ContainsKey(tag.PointTemplateTagGuid)
						|| limitTagAlarmTestDictionary.ContainsKey(tag.PointTemplateTagGuid))
						{
							tagDictionary.Add(tag.PointTemplateTagGuid, tag);
						}
					}

					if (pointTemplate.Tags.Count != tagDictionary.Count)
					{
						pointTemplate.Tags = tagDictionary;
					}

					// Remove any Tags no longer referenced by Module Instances and no longer referenced by Alarm
					tagDictionary = new Dictionary<Guid, PointTemplateTag>();
					foreach (var tag in pointTemplate.Tags.Values)
					{
						if (pointTemplate.ModuleInstances.Values
							.SelectMany(x => x.ModuleToPointTemplateData.TagToModules)
							.Any(y => y.TagGuid == tag.PointTemplateTagGuid))
						{
							tagDictionary.Add(tag.PointTemplateTagGuid, tag);
						}

						else if (stateTagAlarmDictionary.ContainsKey(tag.PointTemplateTagGuid))
						{
							var alarmTemplate = stateTagAlarmDictionary[tag.PointTemplateTagGuid];
							if (pointTemplate.Tags.ContainsKey(alarmTemplate.InputTemplateTagGuid))
							{
								tagDictionary.Add(tag.PointTemplateTagGuid, tag);
							}
						}

						else if (limitTagAlarmTestDictionary.ContainsKey(tag.PointTemplateTagGuid))
						{
							var alarmTestTemplate = limitTagAlarmTestDictionary[tag.PointTemplateTagGuid];
							var alarmTemplate = alarmDictionary[alarmTestTemplate.AlarmTemplateGuid];

							if (pointTemplate.Tags.ContainsKey(alarmTemplate.InputTemplateTagGuid))
							{
								tagDictionary.Add(tag.PointTemplateTagGuid, tag);
							}
						}

						else if (!tag.Module)
						{
							tagDictionary.Add(tag.PointTemplateTagGuid, tag);
						}
					}

					if (pointTemplate.Tags.Count != tagDictionary.Count)
					{
						pointTemplate.Tags = tagDictionary;
					}


					// Remove any Properties not longer referenced by ModuleInstances
					var propertyDictionary = new Dictionary<Guid, PointTemplateProperty>();
					foreach (var property in pointTemplate.Properties.Values)
					{
						// check if the property is not being used anywhere else so we can delete it
						if (pointTemplate.ModuleInstances.Values
							.SelectMany(x => x.ModuleToPointTemplateData.PropertyToModules)
							.Any(y => y.PropertyGuid == property.PointTemplatePropertyGuid))
						{
							propertyDictionary.Add(property.PointTemplatePropertyGuid, property);
						}
					}

					if (pointTemplate.Properties.Count != propertyDictionary.Count)
					{
						pointTemplate.Properties = propertyDictionary;
					}
				}

				pointTemplates.Modify(security, pointTemplate);
			}

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, module.EntityType, moduleGuid);

			foreach (var entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = module.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}


			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.gsp_ModuleDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ModuleGuid", moduleGuid);
				ConsolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		public Guid GetIdentityGuid(SecurityClass security, string ID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			var consolidatedDA = new ConsolidatedDAClass();

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT ModuleGuid FROM tblModule WHERE ID = @ID and SiteGuid = @SiteGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ID", ID);
				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count > 0)
			{
				return (Guid)table.Rows[0]["ModuleGuid"];
			}

			return Guid.Empty;
		}

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LogToAlarmAndEventLog (SecurityClass security, string message) {
            AlarmAndEventLogsClass alarmAndEventLogs = new AlarmAndEventLogsClass();
            alarmAndEventLogs.Add(security, new AlarmAndEventLogClass(new AlarmAndEventDescriptorClass(false, BaseObjectClass.WebApplicationKey, message)));
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Module Get(SecurityClass security, Guid moduleGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			DataSet set = null;
			var module = new Module();

			using (var cmd = new SqlCommand())
			{
				module.EnumerateByModuleGuidSQL(cmd, moduleGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

         DataTable table = set.Tables[0];
         if (table.Rows.Count > 0)
         {
            var row = set.Tables[0].Rows[0];
            module.AutoLoad(row);
         }

			return module;
		}
		public List<Guid> EnumeratePointTemplatesByAnyModuleTypeNames(SecurityClass security, string[] moduleTypeNames)
		{
			var pointTemplateGuidList = new List<Guid>();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (moduleTypeNames == null)
			{
				throw new ArgumentNullException("Missing Module Type Names");
			}

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				Module.EnumeratePointTemplatesByAnyModuleTypeNamesSQL(cmd, moduleTypeNames);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (set != null
			&& set.Tables.Count == 1)
			{

				DataTable table = set.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					pointTemplateGuidList.Add((System.Guid)row[0]);
				}
			}

			return pointTemplateGuidList;
		}

		public List<Guid> EnumeratePointTemplatesByAllModuleTypeNames(SecurityClass security, string[] moduleTypeNames)
		{
			var pointTemplateGuidList = new List<Guid>();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (moduleTypeNames == null)
			{
				throw new ArgumentNullException("Missing Module Type Names");
			}


			DataSet set = null;


			using (var cmd = new SqlCommand())
			{
				Module.EnumeratePointTemplatesByAllModuleTypeNamesSQL(cmd, moduleTypeNames);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			if (set != null
			&& set.Tables.Count == 1)
			{

				DataTable table = set.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					pointTemplateGuidList.Add((System.Guid) row[0]);
				}
			}

			return pointTemplateGuidList;
		}

		public Dictionary<Guid, Module> EnumerateByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			DataSet set = null;
			var module = new Module();

			using (var cmd = new SqlCommand())
			{
				module.EnumerateByPointTemplateGuidSQL(cmd, pointTemplateGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			return PopulateDictionary(set);
		}


		public Dictionary<Guid, Module> EnumerateBySiteGuid(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			DataSet set = null;
			var module = new Module();

			using (var cmd = new SqlCommand())
			{
				module.EnumerateBySiteGuidSQL(cmd, siteGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			return PopulateDictionary(set);
		}

		public Dictionary<Guid, Module> EnumerateForAddToPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			DataSet set = null;
			var module = new Module();

			using (var cmd = new SqlCommand())
			{
				module.EnumerateForAddToPointTemplateGuidSQL(cmd, security.SiteGuid, pointTemplateGuid);
				set = ConsolidatedDa.GetDataSet(cmd, security);
			}

			return PopulateDictionary(set);
		}

		public Dictionary<Guid, Module> EnumerateFromModuleInstances(SecurityClass security, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			Dictionary<Guid, Module> moduleDictionary = new Dictionary<Guid, Module>();

			foreach (var moduleInstance in moduleInstances)
			{
				if (!moduleDictionary.ContainsKey(moduleInstance.Value.ModuleGuid))
				{
					var module = Get(security, moduleInstance.Value.ModuleGuid);
					moduleDictionary.Add(module.ModuleGuid, module);
				}
			}

			return moduleDictionary;
		}


		//Dictionary<ModuleGuid,Module>
		protected Dictionary<Guid, Module> PopulateDictionary(DataSet set)
		{

			Dictionary<Guid, Module> moduleDictionary = new Dictionary<Guid, Module>();

			DataTable table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
				var module = new Module();

				module.AutoLoad(row);

				moduleDictionary.Add(module.ModuleGuid, module);
			}

			return moduleDictionary;
		}

		internal static EngineeringUnit GetModuleTagServerUnits(ModuleTag moduleTag)
		{
			return moduleTag.ServerUnits == EngineeringUnit.FmuNone
				? moduleTag.Units
				: moduleTag.ServerUnits;
		}

		internal static bool SupportsServerEngineeringUnits(EngineeringUnitType unitType)
		{
			return unitType != EngineeringUnitType.FmuNodim
				&& unitType != EngineeringUnitType.FmuNone;
		}

		internal static bool ShouldCascadeServerUnits(EngineeringUnitType unitType, EngineeringUnit serverUnits)
		{
			return SupportsServerEngineeringUnits(unitType)
				&& (serverUnits == EngineeringUnit.FmuNone
					|| serverUnits == EngineeringUnit.FmSiteUnits);
		}

		public static bool IsModuleEnabled(Guid moduleGuid)
		{
			if(moduleGuid == Guids.LeakDetectionModuleGuid)
			{
				var hardwareKey = new HardwareKeyClass();
				return hardwareKey.IsLeakDetectionKey();
			}
			return true;
		}

		#region Explicit Interface Methods

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			// Purge Points
			var o = Object as SiteClass;
			if (o != null)
			{
				var site = o;
				var moduleDictionary = this.EnumerateBySiteGuid(security, site.SiteGuid);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (var module in moduleDictionary.Values)
				{
					if (site.SiteGuid == module.SiteGuid)
					{
						this.Purge(security, module.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass
						{
							TypeID = module.EntityType,
							SiteGuid = site.SiteGuid,
							IdentityGuid = module.IdentityGuid
						};
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

        // Import module function. Will reject the import if there is a module with an existing GUID already present in the database.
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public String Import(SecurityClass security, Module module)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }
            if (module == null)

            {
                throw new ArgumentNullException("module");
            }

            var systemModule = this.Get(security, module.ModuleGuid);
            var systemModuleByName = this.GetIdentityGuid(security, module.ID);

            if (systemModule.IdentityGuid != Guid.Empty)
                return ("GUID for module " + module.ID + " was found in the database. Skipping this module");
            else if (systemModuleByName != Guid.Empty)
            {
                return ("ID for ID " + module.ID + " was found in the database. Point Templates that reference this module ID will not import. Skipping this module");
            }
            else
            {
                this.Add(security, module);
                return module.ID + " was imported succesfully";
            }
        }

        #endregion

    }
}
