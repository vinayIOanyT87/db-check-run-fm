
namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.ServiceModel;
	using System.Web;
	using System.Web.Mvc;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using System.Web.Script.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class ModuleEditorController : FMBaseControllerEx
	{
		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult ModuleEditor(string moduleGuidString, bool moduleLibrary)
		{
			Module module;
			ModuleEditorModel moduleEditorModel;

			try
			{
				var moduleGuid = new Guid(moduleGuidString);
				if (moduleGuid != Guid.Empty)
				{
					module = FMChannelHelper.MakeCall<IModules, Module>(x => x.Get(this.Security, moduleGuid));
				}
				else
				{
					module = new Module
					{
						ID = string.Empty,
						Description = string.Empty,
						Standard = false,
						ModuleScript = "//The CustomLogic.Calculate() method is called by the Point Execution engine\r\n"
											+ "var CustomLogic = {\r\n"
											+ "	Calculate: function () {\r\n"
											+ "		//Add your custom code here\r\n"
											+ "	}\r\n"
											+ "}\r\n",
						ModuleData = new ModuleData()
					};
				}

				moduleEditorModel = new ModuleEditorModel(moduleLibrary,
																		module,
																		PointCustomTemplateDetailController.EnumerateTagDataTypes(this.Security, base.UseDataDictionary),
																		PointCustomTemplateDetailController.EnumerateTagInputOutputTypes(),
																		PointCustomTemplateDetailController.EnumerateOutputTagChangeAgents(),
																		PointCustomTemplateDetailController.EnumeratePropertyDataTypes(this.Security, base.UseDataDictionary),
																		(!this.Security.HasRight(RIGHT.MODIFY_MODULE_LIBRARY) || (module.SiteGuid != this.Security.SiteGuid && module.ModuleGuid != Guid.Empty)));
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return PartialViewWithErrorMessages("ModuleEditor", moduleEditorModel, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult SaveModule(string id, string guid, string description, string tags, string settings, string script)
		{
			var results = new List<KeyValuePair<string, string>>();
			var tagList = new List<ModuleEditorTagModel>();
			var settingList = new List<ModuleEditorSettingModel>();


			try
			{

				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(tags))
				{
					tagList = jss.Deserialize<List<ModuleEditorTagModel>>(tags);
				}

				if (!string.IsNullOrEmpty(settings))
				{
					settingList = jss.Deserialize<List<ModuleEditorSettingModel>>(settings);
				}

				var moduleGuid = new Guid(guid);
				Module module;
				if (moduleGuid != Guid.Empty)
				{
					module = FMChannelHelper.MakeCall<IModules, Module>(x => x.Get(this.Security, moduleGuid));

					// Changing the ID results in a copy
					if(module.ID != id)
					{
						moduleGuid = Guid.Empty;
						module.Standard = false;
					}
				}
				else
				{
					module = new Module() { ModuleCalculation = string.Empty, ModuleTypeName = string.Empty, ModuleData = new ModuleData() };
				}

				var moduleTags = new List<ModuleTag>();
				var moduleSettings = new List<ModuleSetting>();

				module.ID = id;
				module.Description = description;
				module.ModuleGuid = moduleGuid;
				module.ModuleScript = script;
				if (module.ModuleData == null)
				{
					module.ModuleData = new ModuleData();
				}

				var isCSharpModule = string.IsNullOrEmpty(script);
				var existingTagsByParameterName = new Dictionary<string, ModuleTag>();
				var existingSettingsByPropertyName = new Dictionary<string, ModuleSetting>();

				if (isCSharpModule)
				{
					foreach (var existingTag in module.ModuleData.ModuleTags ?? new ModuleTag[0])
					{
						if (string.IsNullOrEmpty(existingTag.ParameterName))
						{
							continue;
						}

						if (!existingTagsByParameterName.ContainsKey(existingTag.ParameterName))
						{
							existingTagsByParameterName.Add(existingTag.ParameterName, existingTag);
						}
					}

					foreach (var existingSetting in module.ModuleData.ModuleSettings ?? new ModuleSetting[0])
					{
						if (string.IsNullOrEmpty(existingSetting.PropertyName))
						{
							continue;
						}

						if (!existingSettingsByPropertyName.ContainsKey(existingSetting.PropertyName))
						{
							existingSettingsByPropertyName.Add(existingSetting.PropertyName, existingSetting);
						}
					}
				}

				foreach (var tag in tagList)
				{
					// CSharp modules have empty script and only TagID may be alterted
					if (isCSharpModule)
					{
						ModuleTag moduleTag;
						if (existingTagsByParameterName.TryGetValue(tag.ParameterName, out moduleTag))
						{
							moduleTag.TagID = tag.TagName;
							moduleTags.Add(moduleTag);
						}
					}
					else
					{
						var moduleTag = new ModuleTag()
							               {
								               TagID = tag.TagName,
								               ParameterName = tag.ParameterName,
								               InputOutputType = tag.PointTagInputOutputTypeIndex,
								               DataType = tag.ValueType,
								               UnitType = tag.EngineeringUnitsType,
													Units = EngineeringUnit.FmuNone,
													ServerUnits = EngineeringUnit.FmuNone,
								               Input = tag.Input,
								               InhibitInputOutputTypeConfiguration = false,
								               InhibitOverride = false,
							               };

						if (tag.EngineeringUnitsType != EngineeringUnitType.FmuNodim
						&& tag.EngineeringUnitsType != EngineeringUnitType.FmuNone
						&& tag.EngineeringUnitsType != EngineeringUnitType.FmuAll
						&& (moduleTag.DataType == "System.Single"
						|| moduleTag.DataType == "System.Double"))
						{
							moduleTag.ApplyPointTemplateDecimalPlaces = true;
							moduleTag.ApplyPointTemplateEngineeringUnits = true;
							moduleTag.ApplyPointTemplateMaximum = true;
							moduleTag.ApplyPointTemplateMinimum = true;
						}
						else
						{
							moduleTag.ApplyPointTemplateDecimalPlaces = false;
							moduleTag.ApplyPointTemplateEngineeringUnits = false;
							moduleTag.ApplyPointTemplateMaximum = false;
							moduleTag.ApplyPointTemplateMinimum = false;
						}


						switch (moduleTag.DataType)
						{
							case "System.UInt16":
								moduleTag.Maximum = UInt16.MaxValue;
								moduleTag.Minimum = UInt16.MinValue;
								break;
							case "System.UInt32":
							case "FMBusinessObjects.DataObjects.DeviceAlarmMapReference":
								moduleTag.Maximum = UInt32.MaxValue;
								moduleTag.Minimum = UInt32.MinValue;
								break;
							case "System.UInt64":
								moduleTag.Maximum = UInt64.MaxValue;
								moduleTag.Minimum = UInt64.MinValue;
								break;
							case "System.Int16":
								moduleTag.Maximum = Int16.MaxValue;
								moduleTag.Minimum = Int16.MinValue;
								break;
							case "System.Int32":
								moduleTag.Maximum = Int32.MaxValue;
								moduleTag.Minimum = Int32.MinValue;
								break;
							case "System.Int64":
								moduleTag.Maximum = Int64.MaxValue;
								moduleTag.Minimum = Int64.MinValue;
								break;
							case "System.Single":
								moduleTag.Maximum = 1000000.00;
								moduleTag.Minimum = 0.00;
								break;
							case "System.Double":
								moduleTag.Maximum = 1000000.00;
								moduleTag.Minimum = 0.00;
								break;
							default:
								moduleTag.Maximum = 0;
								moduleTag.Minimum = 0;
								break;
						}

						if(moduleTag.DataType == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
						{
							moduleTag.Value = new DeviceAlarmMapReference();
						}

						moduleTags.Add(moduleTag);
					}
				}

				foreach (var setting in settingList)
				{
					// CSharp Modules have empty script and only SettingName may be altered.
					if (isCSharpModule)
					{
						ModuleSetting moduleSetting;
						if (existingSettingsByPropertyName.TryGetValue(setting.PropertyName, out moduleSetting))
						{
							moduleSetting.SettingName = setting.SettingName;
							moduleSettings.Add(moduleSetting);
						}
					}
					else
					{
						var moduleSetting = new ModuleSetting()
							                  {
								                  DataType = setting.ValueType,
								                  SettingName = setting.SettingName,
								                  PropertyName = setting.PropertyName
							                  };

						moduleSettings.Add(moduleSetting);
					}
				}

				module.ModuleData.ModuleTags = moduleTags.ToArray();
				module.ModuleData.ModuleSettings = moduleSettings.ToArray();

				if (module.ModuleGuid == Guid.Empty)
				{
					module.ModuleGuid = FMChannelHelper.MakeCall<IModules, Guid>(x => x.Add(this.Security, module));
				}
				else
				{
					FMChannelHelper.MakeCall<IModules>(x =>
					{
						((IClientChannel)x).OperationTimeout = new TimeSpan(0, ModuleLibraryController.ModuleModifyOperationTimeoutMinutes, 0);
						x.Modify(this.Security, module);
					});
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);

			}

			return this.JsonWithErrorMessages(results, JsonRequestBehavior.AllowGet);
		}
	}
}