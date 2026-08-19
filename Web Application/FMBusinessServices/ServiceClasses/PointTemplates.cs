// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointTemplates.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Reflection;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.Attributes;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;

	using Newtonsoft.Json;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
	using System.Collections.Generic;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PointTemplates : FMServiceBase, IPointTemplates, IDependency
	{
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Add( SecurityClass security, PointTemplate pointTemplate)
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights

			if (!pointTemplate.Standard)
			{
				pointTemplate.PointLogicScript = this.GeneratePointLogicScript(pointTemplate);
			}

			pointTemplate.Version = 1;

			var consolidatedDA = new ConsolidatedDAClass();
			var oldGuid = pointTemplate.IdentityGuid;
			using ( var cmd = new SqlCommand() )
			{

				pointTemplate.SetCreationStamp(security);
				pointTemplate.AutoGenerateInsertProcSQL( cmd, "gsp_PointTemplateInsertByPK" );
				cmd.Parameters["@PointTemplateGuid"].Direction = ParameterDirection.InputOutput;

				consolidatedDA.ExecuteQuery( security, cmd );

				pointTemplate.IdentityGuid = new Guid(cmd.Parameters["@PointTemplateGuid"].Value.ToString());
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(pointTemplate);
			entityToSiteMaps.Add( security, entityToSiteMap, GetType().GUID );

			// Save Tags
			var tags = new PointTemplateTags();
			pointTemplate.Tags.ToList().ForEach(x => x.Value.PointTemplateGuid = pointTemplate.PointTemplateGuid);
			tags.AddTags(security, pointTemplate.Tags);

			// Save Properties
			var props = new PointTemplateProperties();
			pointTemplate.Properties.Values.ToList().ForEach(x => x.PointTemplateGuid = pointTemplate.PointTemplateGuid);
			props.AddProperties(security, pointTemplate.Properties.Values.ToList());

			// Save ModuleInstances
			var modInsts = new ModuleToPointTemplateMaps();
			pointTemplate.ModuleInstances.Values.ToList().ForEach(x => x.PointTemplateGuid = pointTemplate.PointTemplateGuid);
			modInsts.AddModuletoPointTemplateMaps(security, pointTemplate.ModuleInstances.Values.ToList());
		}



		public string GeneratePointLogicScript(PointTemplate template)
		{
			var deviceAlarmMapTagList = new List<Guid>();

			foreach (var tag in template.Tags.Values)
			{
				// Tags of type FMBusinessObjects.DataObjects.DeviceAlarmMapReference will not be part of the logic
				// due to the fact that they do not contain PointTemplateGuid in derived points
				if (tag.ValueTypeString != "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				{
					continue;
				}

				// Any tags associated with DeviceAlarmMapReference will not be part of the logic also
				// due to the fact that they do not contain PointTemplateGuid in derived points
				if (!tag.AlarmTemplates.Any())
				{ 
					continue;
				}

				foreach (var alarmTemplate in tag.AlarmTemplates.Values)
				{
					deviceAlarmMapTagList.Add(alarmTemplate.AlarmStateTemplateTagGuid);

					foreach (var alarmTestTemplate in alarmTemplate.AlarmTestTemplates.Values)
					{
						deviceAlarmMapTagList.Add(alarmTestTemplate.LimitTemplateTagGuid);
					}
				}
			}


			string script = string.Empty;
			script += "namespace FMPointService.PointExecution\r\n";
			script += "{\r\n";

			// Standard Using statements
			script += "	using System;\r\n";
			script += "	using System.Collections.Generic;\r\n";
			script += "	using Microsoft.ClearScript.V8;\r\n";
			script += "	using FMBusinessObjects.DataObjects;\r\n";
			script += "	using VCF;\r\n";
			script += "	using Quantities;\r\n";
			script += "	using StrapTables;\r\n";
			script += "	using ShellCorrection;\r\n";
			script += "	using FloatingRoofCorrection;\r\n";
			script += "	using RateModules;\r\n";
			script += "	using TankCommands;\r\n";
			script += "	using TankTransfer;\r\n";
			script += "	using AvailableAndRemainingVolume;\r\n";
			script += "	using CustomModule;\r\n";
			script += "	using StandardTankCalculator;\r\n";
			script += "	using VolumeTransfer;\r\n";
         script += "	using Movement;\r\n";
         script += "	using MovementControl;\r\n";
         script += "	using LeakDetection;\r\n";
			script += "	using TotalizerCalculation;\r\n";
			script += " using NodeTransfer;\r\n";
			script += "\r\n";

			// Class definition
			script += "	public class " + template.ID.Replace(" ", string.Empty) + " : PointTemplateLogic\r\n";
			script += "	{\r\n";

			// Tag References
			script += "		#region Private data members\r\n";
			script += "		// Tags\r\n";
			foreach(var tag in template.Tags.Values)
			{
				if(deviceAlarmMapTagList.Contains(tag.PointTemplateTagGuid))
				{
					continue;
				}

				var id = tag.ID.Replace("&", "Amp").Replace(" ", string.Empty).Replace("/", string.Empty);
				script += "		private PointTag	" + id + ";\r\n"; 
			}

			// Property References
			script += "\r\n";
			script += "		// Properties\r\n";
			foreach(var property in template.Properties.Values)
			{
				script += "		private PointProperty	" + property.ID.Replace(" ", string.Empty).Replace("/", string.Empty) + "Property;\r\n";
			}

			// Module Instance References
			script += "\r\n";
			script += "		// Modules\r\n";
			foreach(var moduleInstance in template.ModuleInstances.Values)
			{
				var module = template.Modules[moduleInstance.ModuleGuid];
				// C# Module denoted by empty ModuleScript
				if (string.IsNullOrEmpty(module.ModuleScript))
				{
					script += "		private " + module.ModuleTypeName + " " + moduleInstance.ID.Replace(" ", string.Empty) + ";\r\n";
				}
				else
				{
					script += "		private FMCustomModule " + moduleInstance.ID.Replace(" ", string.Empty) + ";\r\n";
				}
			}
			script += "		#endregion\r\n";
			script += "\r\n";

			// Constructor
			script += "		#region Constructors\r\n";
			script += "		/// <summary>\r\n";
			script += "		/// This is the constructor for the " + template.ID + " object.\r\n";
			script += "		/// </summary>\r\n";
			script += "		/// <param name=\"point\">The point that contains the tags.</param>\r\n";
			script += "		public " + template.ID.Replace(" ", string.Empty) + "(Point point, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Dictionary<Guid, string> moduleLogicScript) : base(point)\r\n";
			script += "		{\r\n";

			script += "\r\n";
			script += "			// Initialize Tag References\r\n";
			foreach (var tag in template.Tags.Values)
			{
				if (deviceAlarmMapTagList.Contains(tag.PointTemplateTagGuid))
				{
					continue;
				}

				var id = tag.ID.Replace("&", "Amp").Replace(" ", string.Empty).Replace("/", string.Empty);
				script += "			this." + id + " = base.GetTag(\"" + tag.PointTemplateTagGuid.ToString() + "\");\r\n";
			}

			script += "\r\n";
			script += "			// Initialize Property References\r\n";
			foreach (var property in template.Properties.Values)
			{
				script += "			this." + property.ID.Replace(" ", string.Empty).Replace("/", string.Empty) + "Property = base.GetProperty(\"" + property.PointTemplatePropertyGuid.ToString() + "\");\r\n";
			}

			script += "\r\n";
			script += "			// Instantiate Modules\r\n";
			foreach (var moduleInstance in template.ModuleInstances.Values)
			{
				var module = template.Modules[moduleInstance.ModuleGuid];

				// C# Module denoted by empty ModuleScript
				if (string.IsNullOrEmpty(module.ModuleScript))
				{
					script += "			this." + moduleInstance.ID.Replace(" ", string.Empty) + " = new " + module.ModuleTypeName + "();\r\n";
				}
				else
				{
					script += "			this." + moduleInstance.ID.Replace(" ", string.Empty) + " = new FMCustomModule(point, \"" + moduleInstance.IdentityGuid.ToString() + "\", moduleInstances, moduleLogicScript);\r\n";
				}
			}

			script += "\r\n";
			script += "			// Set Module References\r\n";
			foreach (var moduleInstance in template.ModuleInstances.Values)
			{
				var module = template.Modules[moduleInstance.ModuleGuid];


				if (!string.IsNullOrEmpty(module.ModuleScript))
				{
					continue;
				}

				foreach (var reference in module.ModuleData.ModuleReferences)
				{
					script += "			this." + moduleInstance.ID.Replace(" ", string.Empty) + "." + reference.Property + " = this." + reference.ID.Replace(" ", string.Empty) + ";\r\n";
				}
			}

			script += "		}\r\n";
			script += "		#endregion\r\n";
			script += "\r\n";

			// Execute Method
			script += "		#region Public methods\r\n";
			script += "		/// <summary>\r\n";
			script += "		/// This method overrides the Execute base class to apply settings and execute logic.\r\n";
			script += "		/// </summary>\r\n";
			script += "		public override void Execute(V8ScriptEngine v8Engine,PointTemplateLogic.CalculationType calculationType, PointCalculatorData pointCalculatorData)\r\n";
			script += "		{\r\n";
			script += "			if (this.InitializationFailed)\r\n";
			script += "			{\r\n";
			script += "				return;\r\n";
			script += "			}\r\n";

			script += "\r\n";
			script += "			// Apply Module Settings\r\n";
			foreach (var moduleInstance in template.ModuleInstances.Values)
			{
				var module = template.Modules[moduleInstance.ModuleGuid];
				if (!string.IsNullOrEmpty(module.ModuleScript))
				{
					continue;
				}

				foreach (var propertyToModule in moduleInstance.ModuleToPointTemplateData.PropertyToModules)
				{
					var property = template.Properties[propertyToModule.PropertyGuid];
					script += "			this." + moduleInstance.ID.Replace(" ", string.Empty) + "." + propertyToModule.PropertyName + " = this." + property.ID.Replace(" ", string.Empty) + "Property.Value as " + property.ValueTypeString + ";\r\n";
				}
			}

			script += "\r\n";
			script += "			// Module Calculations\r\n";
			script += "\r\n";
			script += "			if(calculationType == PointTemplateLogic.CalculationType.Calculator)\r\n";
			script += "			{\r\n";

            // Calculator Branch
            foreach (var moduleInstance in template.ModuleInstances.Values)
			{
				var module = template.Modules[moduleInstance.ModuleGuid];
				if(!module.ModuleData.Calculator)
				{
					continue;
				}

				if (moduleInstance.ModuleGuid == Guids.StrapTableModuleGuid
                || moduleInstance.ModuleGuid == Guids.VCFModuleGuid
                || moduleInstance.ModuleGuid == Guids.ShellCorrectionModuleGuid
                || moduleInstance.ModuleGuid == Guids.RoofCorrectionModuleGuid
                || moduleInstance.ModuleGuid == Guids.QuantityModuleGuid
                || moduleInstance.ModuleGuid == Guids.AvailableAndRemainingVolumeModuleGuid)
				{
					if(template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.StandardTankCalculatorModuleGuid)
                    && template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.StrapTableModuleGuid)
                    && template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.VCFModuleGuid)
                    && template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.ShellCorrectionModuleGuid)
                    && template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.RoofCorrectionModuleGuid)
                    && template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.QuantityModuleGuid)
                    && template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.AvailableAndRemainingVolumeModuleGuid))
					{ 
						continue;
					}
				}

				if(moduleInstance.ModuleGuid == Guids.StandardTankCalculatorModuleGuid
				&& template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.StrapTableModuleGuid)
				&& template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.VCFModuleGuid)
				&& template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.ShellCorrectionModuleGuid)
				&& template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.RoofCorrectionModuleGuid)
				&& template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.QuantityModuleGuid)
				&& template.ModuleInstances.Values.Any(x => x.ModuleGuid == Guids.AvailableAndRemainingVolumeModuleGuid))
				{
					script += "				if (pointCalculatorData != null) \r\n";
					script += "				{\r\n";
					script += "\r\n";
					script += "					var pointLogicTags = new List<PointTag>(); \r\n";
					script += "					pointLogicTags.Add(this.VolumeCorrectionFactorUnrounded); \r\n";
					script += "					pointLogicTags.Add(this.VolumeNetStandardUnrounded); \r\n";
					script += "					pointLogicTags.Add(this.VolumeCorrectionforTemperature); \r\n";
					script += "					pointLogicTags.Add(this.VolumeCorrectionforPressure); \r\n";
					script += "					pointLogicTags.Add(this.VolumeCorrectionforTemperatureandPressure); \r\n";
					script += "					pointLogicTags.Add(this.APICorrectionError); \r\n";
					script += "					pointLogicTags.Add(this.DensityProductinAir); \r\n";
					script += "					pointLogicTags.Add(this.DensityProductStandardinAir); \r\n";
					script += "					pointLogicTags.Add(this.RoofCriticalZone); \r\n";
					script += "					pointLogicTags.Add(this.VolumeBSW); \r\n";
					script += "					pointLogicTags.Add(this.VolumeVaporNet); \r\n";
					script += "					pointLogicTags.Add(this.LevelProductMinOpLimit); \r\n";
					script += "					pointLogicTags.Add(this.LevelProductMaxOpLimit); \r\n";
					script += "					pointLogicTags.Add(this.DensityProductGauge); \r\n";
					script += "\r\n";
					script += "					// call the standardtankcalculator module\r\n";
					script += "					this.StandardTankCalculator.TankCalculatorCalculation(pointCalculatorData, pointLogicTags); \r\n";
					script += "				}\r\n";
					script += "				else\r\n";
					script += "				{\r\n";
				}

				script += "					// call the standardtankcalculator module using mapped in valuess\r\n";
				script += "					this." + moduleInstance.ID.Replace(" ", string.Empty) + "." + module.ModuleCalculation + "(\r\n";
				var parameterCount = 0;
				foreach (var tagToModule in moduleInstance.ModuleToPointTemplateData.TagToModules)
				{
					var tag = template.Tags[tagToModule.TagGuid];
					var id = tag.ID.Replace("&", "Amp").Replace(" ", string.Empty).Replace("/", string.Empty);
					script += "					this." + id + ((parameterCount == moduleInstance.ModuleToPointTemplateData.TagToModules.Length - 1) ? ");\r\n" : ",\r\n");
					parameterCount++;
				}

				script += "				}\r\n";
				script += "\r\n";
			}

         script += "			}\r\n";
			script += "\r\n";
			script += "			else\r\n";
			script += "			{\r\n";

			// Standard Branch
			foreach (var moduleInstance in template.ModuleInstances.Values)
			{
				var module = template.Modules[moduleInstance.ModuleGuid];

				if (module.ModuleData.Calculator)
				{
					continue;
				}

				if (!string.IsNullOrEmpty(module.ModuleScript))
				{
					script += "				this." + moduleInstance.ID.Replace(" ", string.Empty) + ".CustomModuleCalculation(v8Engine);\r\n";
				}
				else
				{

					script += "				this." + moduleInstance.ID.Replace(" ", string.Empty) + "." + module.ModuleCalculation + "(\r\n";
					var parameterCount = 0;
					foreach (var tagToModule in moduleInstance.ModuleToPointTemplateData.TagToModules)
					{
						var tag = template.Tags[tagToModule.TagGuid];
						var id = tag.ID.Replace("&", "Amp").Replace(" ", string.Empty).Replace("/", string.Empty);
						script += "				this." + id + ((parameterCount == moduleInstance.ModuleToPointTemplateData.TagToModules.Length - 1) ? ");\r\n" : ",\r\n");
						parameterCount++;
					}
				}

				script += "\r\n";
			}

			script += "			}\r\n";
			script += "		}\r\n";
			script += "		#endregion\r\n";
			script += "\r\n";
			script += "	}\r\n";
			script += "}\r\n";

			return script;
		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Modify( SecurityClass security, PointTemplate pointTemplate )
		{
			this.Modify(security, pointTemplate, false);
		}

		public void Modify(SecurityClass security, PointTemplate pointTemplate, bool incrementVersion)
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights

			var existingTemplate = this.Get(security, pointTemplate.PointTemplateGuid);
			if (existingTemplate.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Point template not found for update.");
			}

			if (!pointTemplate.Standard)
			{
				pointTemplate.PointLogicScript = this.GeneratePointLogicScript(pointTemplate);
			}

			if (!incrementVersion && this.AreEquivalentForPointTemplateModify(pointTemplate, existingTemplate))
			{
				pointTemplate.Version = existingTemplate.Version;
				return;
			}

			pointTemplate.Version = existingTemplate.Version + 1;

			pointTemplate.SetModifyStamp(security);

			var consolidatedDA = new ConsolidatedDAClass();
			using ( var cmd = new SqlCommand() )
			{
				pointTemplate.AutoGenerateModifyProcSQL( cmd, "gsp_PointTemplateUpdateByPK" );
				cmd.Parameters.AddWithValue("@NullOverrideDefaultDrawingGuid", true);
				consolidatedDA.ExecuteQuery( security, cmd );
			}

			if (pointTemplate.SiteGuid != existingTemplate.SiteGuid)
			{
				var entityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
					security, pointTemplate.EntityType, pointTemplate.IdentityGuid);
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = pointTemplate.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(pointTemplate);
					entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
				}

			// TODO: Update all the additional items like modules and tags

			var pointTemplateTags = new PointTemplateTags();
			pointTemplateTags.UpdatePointTemplateTags(security, pointTemplate.PointTemplateGuid, pointTemplate.Tags);

			var pointTemplateProperties = new PointTemplateProperties();
			pointTemplateProperties.UpdatePointTemplateProperties(security, pointTemplate.PointTemplateGuid, pointTemplate.Properties);

			var moduleToPointTemplateMaps = new ModuleToPointTemplateMaps();
			var existingModuleMapsByGuid = moduleToPointTemplateMaps.EnumerateByTemplateGuid(security, pointTemplate.PointTemplateGuid);

			foreach(var moduleInstance in pointTemplate.ModuleInstances.Values)
			{
				ModuleToPointTemplateMap existingModuleMap;
				if (existingModuleMapsByGuid.TryGetValue(moduleInstance.ModuleToPointTemplateGuid, out existingModuleMap)
					&& this.AreEquivalentForModuleMapModify(moduleInstance, existingModuleMap))
				{
					continue;
				}

				moduleToPointTemplateMaps.Modify(security, moduleInstance);
			}


			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE tblPoint SET UpdatedBy = @UserId, UpdatedDate = SYSDATETIMEOFFSET(), PointTemplateVersion = (SELECT Version FROM tblPointTemplate WHERE PointTemplateGuid = @PointTemplateGuid) WHERE PointTemplateGuid = @PointTemplateGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplate.PointTemplateGuid);
				cmd.Parameters.AddWithValue("@UserId", security.UserID);
				consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		private bool AreEquivalentForModuleMapModify(ModuleToPointTemplateMap currentMap, ModuleToPointTemplateMap existingMap)
		{
			return currentMap.ModuleToPointTemplateGuid == existingMap.ModuleToPointTemplateGuid
				&& currentMap.PointTemplateGuid == existingMap.PointTemplateGuid
				&& currentMap.ModuleGuid == existingMap.ModuleGuid
				&& currentMap.SiteGuid == existingMap.SiteGuid
				&& currentMap.Order == existingMap.Order
				&& string.Equals(currentMap.ID, existingMap.ID, StringComparison.Ordinal)
				&& string.Equals(currentMap.ModuleToPointTemplateDataXml, existingMap.ModuleToPointTemplateDataXml, StringComparison.Ordinal);
		}

		private bool AreEquivalentForPointTemplateModify(PointTemplate currentTemplate, PointTemplate existingTemplate)
		{
			return this.AreEquivalentPersistedFieldsForModify(currentTemplate, existingTemplate)
				&& this.AreEquivalentForPointTemplateTagsModify(currentTemplate.Tags, existingTemplate.Tags)
				&& this.AreEquivalentForPointTemplatePropertiesModify(currentTemplate.Properties, existingTemplate.Properties)
				&& this.AreEquivalentForModuleMapsModify(currentTemplate.ModuleInstances, existingTemplate.ModuleInstances);
		}

		private bool AreEquivalentForPointTemplateTagsModify(Dictionary<Guid, PointTemplateTag> currentTags, Dictionary<Guid, PointTemplateTag> existingTags)
		{
			var current = currentTags ?? new Dictionary<Guid, PointTemplateTag>();
			var existing = existingTags ?? new Dictionary<Guid, PointTemplateTag>();

			if (current.Count != existing.Count)
			{
				return false;
			}

			foreach (var currentTag in current)
			{
				PointTemplateTag existingTag;
				if (!existing.TryGetValue(currentTag.Key, out existingTag)
					|| !this.AreEquivalentPersistedFieldsForModify(currentTag.Value, existingTag)
					|| !this.AreEquivalentForAlarmTemplatesModify(currentTag.Value.AlarmTemplates, existingTag.AlarmTemplates))
				{
					return false;
				}
			}

			return true;
		}

		private bool AreEquivalentForAlarmTemplatesModify(Dictionary<Guid, AlarmTemplate> currentAlarmTemplates, Dictionary<Guid, AlarmTemplate> existingAlarmTemplates)
		{
			var current = currentAlarmTemplates ?? new Dictionary<Guid, AlarmTemplate>();
			var existing = existingAlarmTemplates ?? new Dictionary<Guid, AlarmTemplate>();

			if (current.Count != existing.Count)
			{
				return false;
			}

			foreach (var currentAlarmTemplate in current)
			{
				AlarmTemplate existingAlarmTemplate;
				if (!existing.TryGetValue(currentAlarmTemplate.Key, out existingAlarmTemplate)
					|| !this.AreEquivalentPersistedFieldsForModify(currentAlarmTemplate.Value, existingAlarmTemplate)
					|| !this.AreEquivalentPersistedFieldDictionariesForModify(currentAlarmTemplate.Value.AlarmTestTemplates, existingAlarmTemplate.AlarmTestTemplates)
					|| !this.AreEquivalentPersistedFieldDictionariesForModify(currentAlarmTemplate.Value.AlarmStatusTemplates, existingAlarmTemplate.AlarmStatusTemplates))
				{
					return false;
				}
			}

			return true;
		}

		private bool AreEquivalentForPointTemplatePropertiesModify(Dictionary<Guid, PointTemplateProperty> currentProperties, Dictionary<Guid, PointTemplateProperty> existingProperties)
		{
			return this.AreEquivalentPersistedFieldDictionariesForModify(currentProperties, existingProperties);
		}

		private bool AreEquivalentForModuleMapsModify(Dictionary<Guid, ModuleToPointTemplateMap> currentModuleMaps, Dictionary<Guid, ModuleToPointTemplateMap> existingModuleMaps)
		{
			var current = currentModuleMaps ?? new Dictionary<Guid, ModuleToPointTemplateMap>();
			var existing = existingModuleMaps ?? new Dictionary<Guid, ModuleToPointTemplateMap>();

			if (current.Count != existing.Count)
			{
				return false;
			}

			foreach (var currentModuleMap in current)
			{
				ModuleToPointTemplateMap existingModuleMap;
				if (!existing.TryGetValue(currentModuleMap.Key, out existingModuleMap)
					|| !this.AreEquivalentForModuleMapModify(currentModuleMap.Value, existingModuleMap))
				{
					return false;
				}
			}

			return true;
		}

		private bool AreEquivalentPersistedFieldDictionariesForModify<T>(Dictionary<Guid, T> currentItems, Dictionary<Guid, T> existingItems)
		{
			var current = currentItems ?? new Dictionary<Guid, T>();
			var existing = existingItems ?? new Dictionary<Guid, T>();

			if (current.Count != existing.Count)
			{
				return false;
			}

			foreach (var currentItem in current)
			{
				T existingItem;
				if (!existing.TryGetValue(currentItem.Key, out existingItem)
					|| !this.AreEquivalentPersistedFieldsForModify(currentItem.Value, existingItem))
				{
					return false;
				}
			}

			return true;
		}

		private bool AreEquivalentPersistedFieldsForModify<T>(T currentObject, T existingObject)
		{
			if (currentObject == null || existingObject == null)
			{
				return object.Equals(currentObject, existingObject);
			}

            var properties = currentObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				var persistedField = property.GetCustomAttributes(typeof(FMPersistedField), true).FirstOrDefault() as FMPersistedField;
				if (persistedField == null
					|| persistedField.AddOnly
					|| persistedField.ReadOnly
					|| property.Name == "UpdatedDate"
					|| property.Name == "UpdatedBy"
					|| property.Name == "Version")
				{
					continue;
				}

            object currentValue = null;
            object existingValue = null;

            if (property.Name == "ValueTypeString")
            {
               currentValue  = this.GetValueTypeStringSafe(currentObject as BaseSerializedDataObject);
			existingValue = this.GetValueTypeStringSafe(existingObject as BaseSerializedDataObject);
            }
            else
            {
			currentValue = property.GetValue(currentObject, null);
               existingValue = property.GetValue(existingObject, null);
            }

            if (!object.Equals(currentValue, existingValue))
				{
					return false;
				}
			}

			var currentDataObject = currentObject as BaseDataObject;
			var existingDataObject = existingObject as BaseDataObject;
			return currentDataObject == null
				|| existingDataObject == null
				|| currentDataObject.SiteGuid == existingDataObject.SiteGuid;
		}

		private string GetValueTypeStringSafe(BaseSerializedDataObject dataObject)
		{
			return dataObject == null || dataObject.ValueType == null
				? string.Empty
				: dataObject.ValueType.ToString();
		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge( SecurityClass security, Guid pointTemplateGuid )
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights

			var pointTemplate = this.Get(security, pointTemplateGuid);
			if (pointTemplate.IdentityGuid == Guid.Empty)
			{
				throw ( new Exception( "Point Template Not Found" ) );
			}

			var drawings = new Drawings();
			var drawingList = drawings.EnumerateAllAvailableDrawingNamesByPointTemplate(security, pointTemplateGuid);
			if (drawingList.Count > 0)
			{
				throw (new Exception("The Point Template you are trying to delete has associated Point Details. You must delete the associated Point Details first before you can delete the Point Template."));
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, pointTemplate);

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid( security, pointTemplate.EntityType, pointTemplateGuid );

			foreach ( var entityToSiteMap in entityToSiteMapCollection )
			{
				entityToSiteMap.ID = pointTemplate.ID;
				entityToSiteMaps.Purge( security, entityToSiteMap );
			}

			var tags = new PointTemplateTags();
			tags.PurgeAll(security, pointTemplate.IdentityGuid);

			var moduleToPointTemplateMaps = new ModuleToPointTemplateMaps();
			foreach ( var modInst in pointTemplate.ModuleInstances.Values )
			{
				moduleToPointTemplateMaps.Purge(security, modInst.IdentityGuid);
			}

			var props = new PointTemplateProperties();
			foreach (var prop in pointTemplate.Properties.Values)
			{
				props.Purge(security, prop.IdentityGuid);
			}

			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				cmd.CommandText = "dbo.usp_PointTemplateDeleteByPointTemplateGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplate.IdentityGuid);
				consolidatedDA.ExecuteQuery( security, cmd );
			}

			// Save the tags
		}

		public PointTemplate Get(SecurityClass security, Guid pointTemplateGuid)
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights

			var consolidatedDA = new ConsolidatedDAClass();

			var template = new PointTemplate() { IdentityGuid = pointTemplateGuid };
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				template.SelectSQL(cmd);
				set = consolidatedDA.GetDataSet( cmd, security );
			}

			var tags = new PointTemplateTags();
			var moduleInstances = new ModuleToPointTemplateMaps();
			var props = new PointTemplateProperties();
			var modules = new Modules();

			DataTable table = set.Tables[0];
			template = new PointTemplate();
			if (table.Rows.Count > 0)
			{
				template.AutoLoad(table.Rows[0]);

				template.Tags = tags.EnumerateByPointTemplateGuid(security, template.IdentityGuid);

				template.ModuleInstances = moduleInstances.EnumerateByTemplateGuid(security, template.IdentityGuid);

				template.Properties = props.EnumerateByPointTemplateGuid(security, template.IdentityGuid);

				template.Modules = modules.EnumerateByPointTemplateGuid(security, template.IdentityGuid);

			}

			return template;
		}

		public PointTemplate GetPointTemplateBaseData(SecurityClass security, Guid pointTemplateGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			var consolidatedDA = new ConsolidatedDAClass();

			var template = new PointTemplate() { IdentityGuid = pointTemplateGuid };
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				template.SelectSQL(cmd);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count > 0)
			{
				template.AutoLoad(table.Rows[0]);
			}

			return template;
		}

		public PointTemplateCollection EnumerateByType( SecurityClass security, Guid? pointTemplateTypeGuid )
		{
			var pointTemplate = new PointTemplate
									{
										PointTemplateTypeGuid = pointTemplateTypeGuid
									};

			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				pointTemplate.EnumerateSQL(cmd, security.SiteGuid);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			var pointTemplateCollection = new PointTemplateCollection();

			DataTable table = set.Tables[0];
			var tags = new PointTemplateTags();
			var props = new PointTemplateProperties();
			var modules = new Modules();
			var moduleInstances = new ModuleToPointTemplateMaps();

			var hardwareKey = new HardwareKeyClass();
			bool isMovementKey = hardwareKey.IsMovementKey();
			bool isTacFuelsKey = hardwareKey.IsTacFuelsKey();



			foreach (DataRow row in table.Rows)
			{
				pointTemplate = new PointTemplate();

				pointTemplate.AutoLoad(row);

				if((pointTemplate.PointTemplateGuid == Guids.MovementTemplateGuid
				|| pointTemplate.PointTemplateGuid == Guids.StandardMovementControlTemplateGuid
				|| pointTemplate.PointTemplateGuid == Guids.StandardNodeTemplateGuid)
				&& !isMovementKey)
				{
					continue;
				}

				if(pointTemplate.PointTemplateGuid == Guids.TduTemplateWellKnownGuid
				&& !isTacFuelsKey)
				{
					continue;
				}

				pointTemplateCollection.Add(pointTemplate);

				pointTemplate.Tags = tags.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.ModuleInstances = moduleInstances.EnumerateByTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.Properties = props.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.Modules = modules.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);
			}

			return pointTemplateCollection;
		}

		public PointTemplateCollection EnumerateForSiteCreation(SecurityClass security)
		{
			var pointTemplate = new PointTemplate
			{
				PointTemplateTypeGuid = null
			};

			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				pointTemplate.EnumerateSQL(cmd, security.SiteGuid);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			var pointTemplateCollection = new PointTemplateCollection();

			DataTable table = set.Tables[0];
			var tags = new PointTemplateTags();
			var props = new PointTemplateProperties();
			var modules = new Modules();
			var moduleInstances = new ModuleToPointTemplateMaps();

			foreach (DataRow row in table.Rows)
			{
				pointTemplate = new PointTemplate();

				pointTemplate.AutoLoad(row);

				pointTemplateCollection.Add(pointTemplate);

				pointTemplate.Tags = tags.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.ModuleInstances = moduleInstances.EnumerateByTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.Properties = props.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.Modules = modules.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);
			}

			return pointTemplateCollection;
		}

		public PointTemplateCollection EnumerateByModule(SecurityClass security, Guid moduleGuid)
		{
			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				PointTemplate.EnumerateByModuleSQL(cmd, moduleGuid);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			var pointTemplateCollection = new PointTemplateCollection();

			DataTable table = set.Tables[0];
			var tags = new PointTemplateTags();
			var props = new PointTemplateProperties();
			var modules = new Modules();
			var moduleInstances = new ModuleToPointTemplateMaps();


			foreach (DataRow row in table.Rows)
			{
				var pointTemplate = new PointTemplate();

				pointTemplate.AutoLoad(row);
				pointTemplateCollection.Add(pointTemplate);

				pointTemplate.Tags = tags.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.ModuleInstances = moduleInstances.EnumerateByTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.Properties = props.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);

				pointTemplate.Modules = modules.EnumerateByPointTemplateGuid(security, pointTemplate.IdentityGuid);
			}

			return pointTemplateCollection;
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
				cmd.CommandText = "SELECT PointTemplateGuid FROM tblPointTemplate WHERE ID = @ID and SiteGuid = @SiteGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@ID", ID);
				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count > 0)
			{
				return (Guid) table.Rows[0]["PointTemplateGuid"];
			}

			return Guid.Empty;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid? GetDuplicate(SecurityClass security, string id, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}


			if (id == null)
			{
				throw new ArgumentNullException("id");
			}

			if (siteGuid == null)
			{
				throw new ArgumentNullException("siteGuid");
			}

			var consolidatedDA = new ConsolidatedDAClass();
			var pointGroup = new PointGroup();
			DataSet set;
			// get the main PointGroup data
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_PointTemplateGetDuplicate";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ID", id);
				cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);

				set = consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				pointGroup.PointGroupGuid = (Guid)table.Rows[0].ItemArray[0];  // returns only 1 value
			}
			return pointGroup.PointGroupGuid;
		}

		public Dictionary<Guid, string> GetPointCommandStatusDictionary(SecurityClass security, Guid pointTemplateGuid)
		{
			Dictionary<Guid, string> results = new Dictionary<Guid, string>();
			PointTemplate pt = Get(security, pointTemplateGuid);
			if ((pt != null) && (pt.PointCommandStatus != null) && (pt.PointCommandStatus.CommandStatusLists != null))
			{
				foreach (var commandStatusList in pt.PointCommandStatus.CommandStatusLists)
					results.Add(commandStatusList.CommandStatusListGuid, commandStatusList.ID);
			}
			return results;
		}

		public Dictionary<Guid, string> GetDeviceAlarmMapDictionary(SecurityClass security, Guid pointTemplateGuid)
		{
			Dictionary<Guid, string> results = new Dictionary<Guid, string>();
			PointTemplate pt = Get(security, pointTemplateGuid);
			if (pt != null && pt.DeviceAlarmMaps != null)
			{
				foreach(var deviceAlarmMap in pt.DeviceAlarmMaps)
					results.Add(deviceAlarmMap.DeviceAlarmMapGuid, deviceAlarmMap.ID);
			}
			return results;
		}

		public PointCommandStatusList GetPointCommandStatusList(SecurityClass security, Guid pointTemplateGuid, Guid commandStatusListGuid)
		{
			List<PointCommandStatusList.CommandStatusElement> results = new List<PointCommandStatusList.CommandStatusElement>();
			PointTemplate pt = Get(security, pointTemplateGuid);
			if ((pt != null) && (pt.PointCommandStatus != null) && (pt.PointCommandStatus.CommandStatusLists != null))
			{
				foreach(var pointCommandStatusList in pt.PointCommandStatus.CommandStatusLists)
				{
					if (pointCommandStatusList.CommandStatusListGuid.Equals(commandStatusListGuid))
					{
						return pointCommandStatusList;
					}
				}
			}
			return null;
		}

		public DeviceAlarmMap GetDeviceAlarmMap(SecurityClass security, Guid pointTemplateGuid, Guid deviceAlarmMapGuid)
		{
			PointTemplate pt = Get(security, pointTemplateGuid);
			if (pt != null && pt.DeviceAlarmMaps != null)
			{
				foreach (var deviceAlarmMap in  pt.DeviceAlarmMaps)
				{
					if (deviceAlarmMap.DeviceAlarmMapGuid.Equals(deviceAlarmMapGuid))
					{
						return deviceAlarmMap;
					}
				}
			}
			return null;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PointCommandStatusListDeleted(SecurityClass security, Guid pointTemplateGuid, List<string> pointCommandStatusListsDeleted)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}


			var consolidatedDA = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
				using (var parameterTempTable = new DataTable())
				{
					parameterTempTable.Columns.Add("value", typeof(String));

					foreach (var pointGuid in pointCommandStatusListsDeleted)
					{
						parameterTempTable.Rows.Add(pointGuid);
					}

					cmd.CommandText = "dbo.usp_DeletePointTemplatePointCommandStatusList";
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);

					var pList = new SqlParameter("@listDeletedPointCommandStatusList", SqlDbType.Structured);
					pList.TypeName = "dbo.StringListType";
					pList.Value = parameterTempTable;

					cmd.Parameters.Add(pList);
					consolidatedDA.ExecuteQuery(security, cmd);
				}

			}

		}

		public Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPointTemplateFilterByType(SecurityClass security, Guid pointTemplateGuid, PointValueType valueType, bool filter, string dataTypeString, PointValueFieldType fieldFilter)
		{
			PointTemplate pointTemplate = this.Get(security, pointTemplateGuid);
			return pointTemplate.EnumeratePointValueIdentifiersForPointTemplateFilterByType(
				valueType,
				filter,
				dataTypeString,
				fieldFilter);

		}

		public Dictionary<PointValueIdentifier, string> EnumeratePointValueIdentifiersForPointTemplate(SecurityClass security, Guid pointTemplateGuid, PointValueType valueType)
		{
			return this.EnumeratePointValueIdentifiersForPointTemplateFilterByType(security, pointTemplateGuid, valueType, false, string.Empty,PointValueFieldType.VALUE);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public PointTemplate AddModule(SecurityClass security, Guid pointTemplateGuid, List<PointTemplateTag> tags, List<PointTemplateProperty> properties, List<ModuleToPointTemplateMap> moduleInstances)
		{
			security.ThrowIfNull("security");
			tags.ThrowIfNull("tags");
			properties.ThrowIfNull("properties");
			moduleInstances.ThrowIfNull("moduleInstances");

			this.ApplyModuleTagServerUnits(security, tags, moduleInstances);

			if (tags.Count > 0)
			{
				var pointTemplateTags = new PointTemplateTags();
				pointTemplateTags.AddModuleTags(security, tags);
			}

			if (properties.Count > 0)
			{
				var pointTemplateProperties = new PointTemplateProperties();
				pointTemplateProperties.AddProperties(security, properties);
			}

			if (moduleInstances.Count > 0)
			{
				var moduleToPointTemplateMaps = new ModuleToPointTemplateMaps();
				moduleToPointTemplateMaps.AddModuletoPointTemplateMaps(security, moduleInstances);
			}

			var pointTemplate = Get(security, pointTemplateGuid);

			if (!pointTemplate.Standard)
			{
				pointTemplate.PointLogicScript = this.GeneratePointLogicScript(pointTemplate);
			}

			pointTemplate.Version += 1;

			pointTemplate.SetModifyStamp(security);

			var consolidatedDA = new ConsolidatedDAClass();
			using (var cmd = new SqlCommand())
			{
				pointTemplate.AutoGenerateModifyProcSQL(cmd, "gsp_PointTemplateUpdateByPK");
				cmd.Parameters.AddWithValue("@NullOverrideDefaultDrawingGuid", true);
				consolidatedDA.ExecuteQuery(security, cmd);
			}


			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE tblPoint SET UpdatedBy = @UserId, UpdatedDate = SYSDATETIMEOFFSET(), PointTemplateVersion = (SELECT Version FROM tblPointTemplate WHERE PointTemplateGuid = @PointTemplateGuid) WHERE PointTemplateGuid = @PointTemplateGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplate.PointTemplateGuid);
				cmd.Parameters.AddWithValue("@UserId", security.UserID);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

			return pointTemplate;
		}

		private void ApplyModuleTagServerUnits(SecurityClass security, List<PointTemplateTag> tags, List<ModuleToPointTemplateMap> moduleInstances)
		{
			if (tags.Count == 0 || moduleInstances.Count == 0)
			{
				return;
			}

			var tagsByGuid = tags.ToDictionary(x => x.PointTemplateTagGuid);
			var modules = new Modules();
			var modulesByGuid = new Dictionary<Guid, FMBusinessObjects.DataObjects.Module>();

			foreach (var moduleInstance in moduleInstances)
			{
				if (moduleInstance.ModuleToPointTemplateData == null
				|| moduleInstance.ModuleToPointTemplateData.TagToModules == null)
				{
					continue;
				}

				FMBusinessObjects.DataObjects.Module module;
				if (!modulesByGuid.TryGetValue(moduleInstance.ModuleGuid, out module))
				{
					module = modules.Get(security, moduleInstance.ModuleGuid);
					modulesByGuid.Add(moduleInstance.ModuleGuid, module);
				}

				if (module.ModuleData == null
				|| module.ModuleData.ModuleTags == null)
				{
					continue;
				}

				foreach (var tagToModule in moduleInstance.ModuleToPointTemplateData.TagToModules)
				{
					PointTemplateTag tag;
					if (!tagsByGuid.TryGetValue(tagToModule.TagGuid, out tag))
					{
						continue;
					}

					var moduleTag = module.ModuleData.ModuleTags.SingleOrDefault(x => x.ParameterName == tagToModule.ModuleParameter);
					if (moduleTag == null)
					{
						continue;
					}

					if (Modules.ShouldCascadeServerUnits(tag.EngineeringUnitsType, tag.ServerUnits))
					{
						tag.ServerUnits = Modules.GetModuleTagServerUnits(moduleTag);
					}
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public PointTemplate DeleteModule(SecurityClass security, Guid pointTemplateGuid, List<Guid> tagGuidList, List<Guid> tagsWithAlarmsGuidList, List<Guid> propertyGuidList, Guid moduleInstanceGuid)
		{
			security.ThrowIfNull("security");
			tagGuidList.ThrowIfNull("tagGuidList");
			propertyGuidList.ThrowIfNull("propertyGuidList");

			AlarmTemplates alarms = new AlarmTemplates();
			alarms.DeleteAlarmTemplatesFromTagsNotInList(security, pointTemplateGuid, tagsWithAlarmsGuidList);

			var pointTemplateTags = new PointTemplateTags();
			pointTemplateTags.PurgeByPointTemplateGuidAndNotInList(security, pointTemplateGuid, tagGuidList);

			var pointTemplateProperties = new PointTemplateProperties();
			pointTemplateProperties.PurgeByPointTemplateGuidAndNotInList(security, pointTemplateGuid, propertyGuidList);

			var modulePointTemplateMaps = new ModuleToPointTemplateMaps();
			modulePointTemplateMaps.Purge(security, moduleInstanceGuid);


			var pointTemplate = Get(security, pointTemplateGuid);

			if (!pointTemplate.Standard)
			{
				pointTemplate.PointLogicScript = this.GeneratePointLogicScript(pointTemplate);
			}

			pointTemplate.Version += 1;

			pointTemplate.SetModifyStamp(security);

			var consolidatedDA = new ConsolidatedDAClass();
			using (var cmd = new SqlCommand())
			{
				pointTemplate.AutoGenerateModifyProcSQL(cmd, "gsp_PointTemplateUpdateByPK");
				cmd.Parameters.AddWithValue("@NullOverrideDefaultDrawingGuid", true);
				consolidatedDA.ExecuteQuery(security, cmd);
			}


			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE tblPoint SET UpdatedBy = @UserId, UpdatedDate = SYSDATETIMEOFFSET(), PointTemplateVersion = (SELECT Version FROM tblPointTemplate WHERE PointTemplateGuid = @PointTemplateGuid) WHERE PointTemplateGuid = @PointTemplateGuid";
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplate.PointTemplateGuid);
				cmd.Parameters.AddWithValue("@UserId", security.UserID);
				consolidatedDA.ExecuteQuery(security, cmd);
			}


			return pointTemplate;
		}


		public PointTemplatePointServiceData GetPointTemplatePointServiceData(SecurityClass security, Guid pointTemplateGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var consolidatedDA = new ConsolidatedDAClass();

			var template = new PointTemplate() { PointTemplateGuid = pointTemplateGuid };

			DataSet set = null;

			using (var cmd = new SqlCommand())
			{
				template.SelectPointServiceDataSQL(cmd);
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			var pointTemplatePointServiceData = new PointTemplatePointServiceData();

			if (set.Tables.Count > 0)
			{

				var table = set.Tables[0];

				if (table.Rows.Count > 0)
				{
					pointTemplatePointServiceData.AutoLoad(table.Rows[0]);
				}
			}

			var modules = new Modules();
			
			var moduleDictionary = modules.EnumerateByPointTemplateGuid(security, pointTemplateGuid);

			var moduleInstances = new ModuleToPointTemplateMaps();

			pointTemplatePointServiceData.ModuleInstances = moduleInstances.EnumerateByTemplateGuid(security, pointTemplateGuid);

			foreach(var module in moduleDictionary.Values)
			{
				if(!string.IsNullOrEmpty(module.ModuleScript))
				{
					pointTemplatePointServiceData.ModuleLogicScript.Add(module.ModuleGuid, module.ModuleScript);
				}
			}

			return pointTemplatePointServiceData;
		}


		// Update all template PointLogicScript's after a database update
		internal static void UpdatePointLogicScripts(SecurityClass security)
        {
			SitesClass sites = new SitesClass();
			var stiecollection = sites.Enumerate(security);
			PointTemplates pointTemplates = new PointTemplates();
			foreach (var site in stiecollection)
			{
				security.SiteGuid = site.SiteGuid;
				PointTemplateCollection pointTemplatesCollection = pointTemplates.EnumerateByType(security, null);
				foreach (PointTemplate pointTemplate in pointTemplatesCollection)
				{
					pointTemplates.Modify(security, pointTemplate);
				}
			}
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
				PointTemplateCollection pointTemplateCollection = this.EnumerateByType(security, null);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (var pointTemplate in pointTemplateCollection)
				{
					if (site.SiteGuid == pointTemplate.SiteGuid)
					{
						this.Purge(security, pointTemplate.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass
						{
							TypeID = pointTemplate.EntityType,
							SiteGuid = site.SiteGuid,
							IdentityGuid = pointTemplate.IdentityGuid
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

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass security, PointTemplate pointTemplate)
		{
			security.ThrowIfNull("security");
			pointTemplate.ThrowIfNull("pointTemplate");

            try
            {
                if (pointTemplate.IdentityGuid == FMBusinessObjects.Constants.Guids.VerticalTankTemplateGuid)
                {
                    throw new Exception("Cannot import the standard tank template");
                }

                var systemPointTemplate = this.Get(security, pointTemplate.PointTemplateGuid);

                var appStrings = new ApplicationStringsClass();
                //Check the Point Template Type and set it to system if it does not exist
                if (pointTemplate.PointTemplateTypeGuid.HasValue)
                {
                    var pointTypeGuid = pointTemplate.PointTemplateTypeGuid.GetValueOrDefault();
                    ApplicationStringClass pointTypeID = appStrings.Get(security, pointTypeGuid);
                    //if it doesn't exist, assign it to point type 'System' 
                    if (pointTypeID.ID == "")
                    {
                        pointTemplate.PointTemplateTypeGuid = Guid.Parse("2DDEB3E0-545C-444B-B1BF-9CAB048F21B7");
                    }
                }

                var drawingClass = new Drawings();
                if (pointTemplate.DefaultDrawingGuid.HasValue)
                {
                    var defaultDrawingGuid = pointTemplate.DefaultDrawingGuid.GetValueOrDefault();
                    Drawing systemDrawing = drawingClass.Get(security, defaultDrawingGuid);
                    //if it doesn't exist, blank out the guid
                    if (systemDrawing.ID == null)
                    {
                        pointTemplate.DefaultDrawingGuid = null;
                    }
                }

                if (GetIdentityGuid(security, pointTemplate.ID) != Guid.Empty && systemPointTemplate.IdentityGuid == Guid.Empty)
                 throw new Exception("ID was found in the database. ");

                if (systemPointTemplate.IdentityGuid == Guid.Empty && pointTemplate.IdentityGuid == Guid.Parse("00000000-0000-0000-0000-000000000000"))
				{
					pointTemplate.IdentityGuid = Guid.NewGuid();
					// need to load modules that are listed in the module to template map
					var modules = new Modules();
					pointTemplate.Modules = modules.EnumerateFromModuleInstances(security, pointTemplate.ModuleInstances);

					//we have to assume that a new pt is a copied row, so all entities need new identity guids
					//however, the old guids identify the relationships, so we need to keep track of what is changed
					//so that we are able to repair the relationships. this is similar to the copy point template functionality. 

					//track guid changes for relationships
					List<Guid[]> guidTable = new List<Guid[]>();
					//give new guids to all the tags
					List<Guid> tagGuidsToDelete = new List<Guid>();
					List<PointTemplateTag> tagTemplatesToAdd = new List<PointTemplateTag>();
					foreach (var tag in pointTemplate.Tags)
					{
						tagGuidsToDelete.Add(tag.Value.IdentityGuid);
						guidTable.Add(new Guid[] { tag.Value.IdentityGuid, tag.Value.IdentityGuid = Guid.NewGuid() });
						tag.Value.PointTemplateGuid = pointTemplate.PointTemplateGuid;

						//give new guids to all the alarms, and point them to the new tag/template
						List<Guid> alarmTemplateGuidsToDelete = new List<Guid>();
						List<AlarmTemplate> alarmTemplatestoAdd = new List<AlarmTemplate>();
						foreach (var alarmTemplate in tag.Value.AlarmTemplates)
						{
							alarmTemplateGuidsToDelete.Add(alarmTemplate.Value.IdentityGuid);
							alarmTemplate.Value.IdentityGuid = Guid.NewGuid();
							alarmTemplate.Value.PointTemplateGuid = pointTemplate.PointTemplateGuid;
							alarmTemplate.Value.PointTemplateTagGuid = tag.Value.IdentityGuid;


							//give new guids to all the alarm tests and point them to the new alarm/tag/template
							List<Guid> alarmTemplateTestGuidstoDelete = new List<Guid>();
							List<AlarmTestTemplate> alarmTemplateTeststoAdd = new List<AlarmTestTemplate>();
							foreach (var alarmTemplateTest in alarmTemplate.Value.AlarmTestTemplates)
							{
								alarmTemplateTestGuidstoDelete.Add(alarmTemplateTest.Value.IdentityGuid);
								guidTable.Add(new Guid[] { alarmTemplateTest.Value.IdentityGuid, alarmTemplateTest.Value.IdentityGuid = Guid.NewGuid() });

								alarmTemplateTest.Value.PointTemplateTagGuid = tag.Value.IdentityGuid;
								alarmTemplateTest.Value.AlarmTemplateGuid = alarmTemplate.Value.IdentityGuid;
								alarmTemplateTest.Value.PointTemplateGuid = pointTemplate.PointTemplateGuid;
								alarmTemplateTeststoAdd.Add(alarmTemplateTest.Value);
							}
							foreach (Guid alarmTemplateTestGuidtoDelete in alarmTemplateTestGuidstoDelete)
							{
								alarmTemplate.Value.AlarmTestTemplates.Remove(alarmTemplateTestGuidtoDelete);
							}
							foreach (AlarmTestTemplate alarmTemplateTesttoAdd in alarmTemplateTeststoAdd)
							{
								alarmTemplate.Value.AlarmTestTemplates.Add(alarmTemplateTesttoAdd.IdentityGuid, alarmTemplateTesttoAdd);
							}
							alarmTemplatestoAdd.Add(alarmTemplate.Value);
						}
						foreach (Guid alarmTemplateGuidtoDelete in alarmTemplateGuidsToDelete)
						{
							tag.Value.AlarmTemplates.Remove(alarmTemplateGuidtoDelete);
						}
						foreach (AlarmTemplate alarmTemplateToAdd in alarmTemplatestoAdd)
						{
							tag.Value.AlarmTemplates.Add(alarmTemplateToAdd.IdentityGuid, alarmTemplateToAdd);
						}
						tagTemplatesToAdd.Add(tag.Value);
					}
					foreach (Guid guidtoDelete in tagGuidsToDelete)
					{
						pointTemplate.Tags.Remove(guidtoDelete);
					}
					foreach (PointTemplateTag tagToAdd in tagTemplatesToAdd)
					{
						pointTemplate.Tags.Add(tagToAdd.IdentityGuid, tagToAdd);
					}

					//repair alarm and alarmtest to tag relationships. This is why we tracked the guid changes.
					foreach (var tag in pointTemplate.Tags)
					{
						foreach (var alarmTemplate in tag.Value.AlarmTemplates)
						{
							foreach (Guid[] guidSet in guidTable)
								if (alarmTemplate.Value.InputTemplateTagGuid == guidSet[0])
								{
									alarmTemplate.Value.InputTemplateTagGuid = guidSet[1];
								}
							foreach (Guid[] guidSet in guidTable)
								if (alarmTemplate.Value.AlarmStateTemplateTagGuid == guidSet[0])
								{
									alarmTemplate.Value.AlarmStateTemplateTagGuid = guidSet[1];
								}
							foreach (var alarmTemplateTest in alarmTemplate.Value.AlarmTestTemplates)
							{
								foreach (Guid[] guidSet in guidTable)
									if (alarmTemplateTest.Value.LimitTemplateTagGuid == guidSet[0])
									{
										alarmTemplateTest.Value.LimitTemplateTagGuid = guidSet[1];
									}
							}
							List<Guid> alarmStatusTemplateGuidstoDelete = new List<Guid>();
							List<PointTemplateTagAlarmStatus> alarmStatusTemplatestoAdd = new List<PointTemplateTagAlarmStatus>();
							foreach (var alarmStatusTemplate in alarmTemplate.Value.AlarmStatusTemplates)
							{
								PointTemplateTagAlarmStatus alarmStatus = new PointTemplateTagAlarmStatus();
								var serialized = JsonConvert.SerializeObject(alarmStatusTemplate.Value);
								alarmStatus = JsonConvert.DeserializeObject<PointTemplateTagAlarmStatus>(serialized);
								alarmStatus.PointTemplateTagAlarmStatusGuid = Guid.NewGuid();
								alarmStatus.IdentityGuid = alarmStatus.PointTemplateTagAlarmStatusGuid;
								foreach (Guid[] guidSet in guidTable)
									if (alarmStatus.AlarmTestTemplateGuid == guidSet[0])
									{
										alarmStatus.AlarmTestTemplateGuid = guidSet[1];
									}
								alarmStatusTemplateGuidstoDelete.Add(alarmStatusTemplate.Value.IdentityGuid);
								alarmStatusTemplatestoAdd.Add(alarmStatus);
							}
							foreach (Guid alarmStatusTemplateGuid in alarmStatusTemplateGuidstoDelete)
							{
								alarmTemplate.Value.AlarmStatusTemplates.Remove(alarmStatusTemplateGuid);
							}
							foreach (PointTemplateTagAlarmStatus alarmStatusTemplatetoAdd in alarmStatusTemplatestoAdd)
							{
								alarmTemplate.Value.AlarmStatusTemplates.Add(alarmStatusTemplatetoAdd.IdentityGuid, alarmStatusTemplatetoAdd);
							}
						}
					}

					//give new guids to all the properties
					List<Guid> propertyGuidstoDelete = new List<Guid>();
					List<PointTemplateProperty> propertiesToAdd = new List<PointTemplateProperty>();
					foreach (var property in pointTemplate.Properties)
					{
						propertyGuidstoDelete.Add(property.Value.IdentityGuid);
						guidTable.Add(new Guid[] { property.Value.IdentityGuid, property.Value.IdentityGuid = Guid.NewGuid() });
						propertiesToAdd.Add(property.Value);
					}
					foreach (var propertyGuidtoDelete in propertyGuidstoDelete)
					{
						pointTemplate.Properties.Remove(propertyGuidtoDelete);
					}
					foreach (var propertyToAdd in propertiesToAdd)
					{
						pointTemplate.Properties.Add(propertyToAdd.IdentityGuid, propertyToAdd);
					}

					//give new guids to all the moduleInstances
					List<Guid> moduleInstanceGuidstoDelete = new List<Guid>();
					List<ModuleToPointTemplateMap> moduleInstancestoAdd = new List<ModuleToPointTemplateMap>();
					foreach (var moduleInstance in pointTemplate.ModuleInstances)
					{
						moduleInstanceGuidstoDelete.Add(moduleInstance.Value.IdentityGuid);
						moduleInstance.Value.IdentityGuid = Guid.NewGuid();
						moduleInstance.Value.PointTemplateGuid = pointTemplate.PointTemplateGuid;

						foreach (var TagToModule in moduleInstance.Value.ModuleToPointTemplateData.TagToModules)
						{

							foreach (Guid[] guidSet in guidTable)
								if (TagToModule.TagGuid == guidSet[0])
								{
									TagToModule.TagGuid = guidSet[1];
								}
						}

						foreach (var propertyToModule in moduleInstance.Value.ModuleToPointTemplateData.PropertyToModules)
						{
							foreach (Guid[] guidSet in guidTable)
								if (propertyToModule.PropertyGuid == guidSet[0])
								{
									propertyToModule.PropertyGuid = guidSet[1];
								}
						}


						moduleInstancestoAdd.Add(moduleInstance.Value);
					}

					foreach (var moduleInstanceGuidtoDelete in moduleInstanceGuidstoDelete)
					{
						pointTemplate.ModuleInstances.Remove(moduleInstanceGuidtoDelete);
					}
					moduleInstancestoAdd.Reverse(); //moduleInstance order matters for point script execution
					foreach (var moduleInstancetoAdd in moduleInstancestoAdd)
					{
						pointTemplate.ModuleInstances.Add(moduleInstancetoAdd.IdentityGuid, moduleInstancetoAdd);
					}
					this.Add(security, pointTemplate);
				}
				else if (systemPointTemplate.IdentityGuid == Guid.Empty)
				{
					// need to load modules that are listed in the module to template map
					var modules = new Modules();
					pointTemplate.Modules = modules.EnumerateFromModuleInstances(security, pointTemplate.ModuleInstances);
					this.Add(security, pointTemplate);
				}
				else
				{																							
					if (systemPointTemplate.ID != pointTemplate.ID)
						throw new Exception("[Point Template Import Error ID] : '" + pointTemplate.ID + "' does not match the system Point Template ID '" + systemPointTemplate.ID + "'");
					// need to load modules that are listed in the module to template map
					var modules = new Modules();
					pointTemplate.Modules = modules.EnumerateFromModuleInstances(security, pointTemplate.ModuleInstances);

                    var moduleToPointTemplateMaps = new ModuleToPointTemplateMaps();
                    var moduleToPointTemplateMapsToAdd = new List<ModuleToPointTemplateMap>();
                    var moduleToPointTemplateMapsToPurge = new List<ModuleToPointTemplateMap>();
                    foreach (KeyValuePair<Guid,ModuleToPointTemplateMap> modInstance in pointTemplate.ModuleInstances)
                    {
                        if (!systemPointTemplate.ModuleInstances.ContainsKey(modInstance.Key))
                        {
                            moduleToPointTemplateMapsToAdd.Add(modInstance.Value);
                        }
                    }

                    foreach (KeyValuePair<Guid,ModuleToPointTemplateMap> modInstance in systemPointTemplate.ModuleInstances)
                    {
                        if (!pointTemplate.ModuleInstances.ContainsKey(modInstance.Key))
                        {
                            moduleToPointTemplateMaps.Purge(security, modInstance.Key);
                        }
                    }

                    if (moduleToPointTemplateMapsToAdd.Count > 0)
                    {
                        moduleToPointTemplateMaps.AddModuletoPointTemplateMaps(security, moduleToPointTemplateMapsToAdd);
                    }
                    pointTemplate.Version = systemPointTemplate.Version;
               this.Modify(security, pointTemplate);
				}
			}
			catch (Exception ex)
			{
				while(ex.InnerException != null)
				{
					ex = ex.InnerException;
				}

				throw new Exception("[Point Template Import Error ID] : " + pointTemplate.ID + ", " + ex.Message);
			}
		}


		#endregion

	}
}
