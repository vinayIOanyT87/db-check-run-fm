using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System.Globalization;
	using System.Web.Script.Serialization;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Newtonsoft.Json;

	using FMBusinessObjects.DataObjects.CodedVariables;

	using FMPointCommon;

	using Microsoft.Ajax.Utilities;
	using System.Data.SqlClient;
	using FMBusinessObjects.Constants;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
	public class PointCustomTemplateDetailController : FMBaseControllerEx
	{

		#region Private data members
		private const string SuppressedString = "Suppressed";
		private const string ActiveAlarmString = "Active";
		private const string OneShelvedString = "One Shot Shelved";
		private const string TimeShelvedString = "Shelved Until ";
		#endregion

		private void SetFCEERights(PointEditDetailModel model)
		{
			model.HasModifyFCEERight = this.Security.HasRight(RIGHT.MODIFY_FCEE_DATA);
			model.HasFCEERight = model.HasModifyFCEERight || this.Security.HasRight(RIGHT.VIEW_FCEE_DATA);
		}

		// GET: InventoryManagement/PointCustomTemplateDetail
		[HttpGet]
		public ActionResult PointTemplateDetail(string id)
		{
			var model = new PointEditDetailModel();

         try
         {
				this.SetFCEERights(model);
				PointTemplate pointTemplate = null;
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				if (string.IsNullOrEmpty(id) || id.Equals("PointIndex", StringComparison.InvariantCultureIgnoreCase))
				{
				}
				else
				{
					var pointTemplateGuid = new Guid(id);
					pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));
					var categories = new ApplicationStringCollectionClass();
					var productIdList = new List<string>();
					productIdList.Insert(0, string.Empty);
					var associatedDrawings = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(x => x.EnumerateAvailableDrawingNamesByPointTemplate(this.Security, pointTemplate.PointTemplateGuid));

					var wellKnownTags = this.EnumerateWellKnownTags();
					var tagDataTypes = PointCustomTemplateDetailController.EnumerateTagDataTypes(this.Security, base.UseDataDictionary);
					var tagInputOutputTypes = PointCustomTemplateDetailController.EnumerateTagInputOutputTypes();
					var outputTagChangeAgents = PointCustomTemplateDetailController.EnumerateOutputTagChangeAgents();
					var pointCommandStatusListDictionary = GetPointCommaandStatusListDictionary(pointTemplate.Properties, pointTemplate.PointCommandStatus);

					foreach (var drawingName in associatedDrawings)
					{
						if (drawingName.DrawingGuid == Guid.Empty)
						{
							drawingName.ID = "<" + FMBaseController.TranslateText("None") + ">";
						}
					}


					// if we found a point initialize the model with the point
					model = new PointEditDetailModel(pointTemplate, site, categories, productIdList, associatedDrawings, wellKnownTags, tagDataTypes, tagInputOutputTypes, outputTagChangeAgents, pointCommandStatusListDictionary);
					this.SetFCEERights(model);

					if (model.ActionListPointTypes.Count == 0)
					{
						var pointTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																		x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));

						var pointTypeList = new List<SelectListItem>();

						foreach (var pointType in pointTypes)
						{
							if (pointTemplate.ID != "Standard Movement" && pointType.ID == "Movement"
							|| pointTemplate.ID != "Standard Movement Node Vol"  && pointType.ID == "Movement Node")
							{
								continue;
							}


							var pointTypeSelectItem = new SelectListItem { Value = pointType.IdentityGuid.ToString(), Text = pointType.ID };
							pointTypeList.Add(pointTypeSelectItem);
						}

						model.ActionListPointTypes = pointTypeList;
					}
					model.Alarms = this.GetTemplateAlarmModel(this.Security, pointTemplate, site);
				}

				model.ModifyEnabled = this.Security.HasRight(RIGHT.MODIFY_POINT_TEMPLATES)
							&& (this.Security.SiteGuid == model.SiteGuid
							|| model.SiteGuid == Guid.Empty);


				model.HasCopyRight = this.Security.HasRight(RIGHT.COPY_POINT_TEMPLATES) && (pointTemplate == null || !pointTemplate.ID.StartsWith("Standard Movement"));
				model.HasViewPCSList = this.Security.HasRight(RIGHT.VIEW_POINT_COMMANDSTATUS_LIST);
				model.HasModifyPCSList = this.Security.HasRight(RIGHT.MODIFY_POINT_COMMANDSTATUS_LIST);
				model.OpenFormForTag = string.Empty;
				model.OpenFormForModule = string.Empty;

				// set up the Custom Module Programmers Guide Link
				var menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;
				string js = menuData.GetHelpUrl(true) + "CustomModuleProgrammersGuide.pdf";
				string jscript = "window.open('" + HttpUtility.JavaScriptStringEncode(js) + "')";
				model.GuideOpenerScript = new MvcHtmlString(jscript);
			}
			catch (Exception except)
			{
				this.OnError(except);
			}

			return View("../Points/PointsDetail", model);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PointsCustomTemplateEdit(PointEditDetailModel model, string tagGrid, string assignedCategories, string productId, string settings, string profileImageGuid, string unitConversionHistory, string alarmModel, string overrideDefaultDrawingGuidString, string moduleInstances)
		{
			var modulesView = "";
			var results = new List<KeyValuePair<string, string>>();
			bool copyWithNewNameFlag = false;

         var defaultUnitConversionHistory = new List<PointDefaultUnitChangeHistory>();
			try
			{
				this.SetFCEERights(model);
				// Revalidate the model.  If the validation fails with the data annotations (we are checking for require fields there) it will
				// not call the IValidatableObject.Validate method so we may be missing error messages.
				// By forcing a call to the validation we may get duplicate error messages so we need to remove them in the client.
				this.TryValidateModel(model);

				var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, model.IdentityGuid));
				if (pointTemplate.ID != model.Name)
				{
					copyWithNewNameFlag = true;
				}
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator
				};

				Guid tempGuid = Guid.Empty;
				Guid? defaultDrawingGuid = null;
				if (Guid.TryParse(overrideDefaultDrawingGuidString, out tempGuid) && tempGuid != Guid.Empty)
				{
					defaultDrawingGuid = tempGuid;
				}

				model.Tags = new List<PointTagEditGridModel>();
				model.Site = site;
				model.PointTemplate = pointTemplate;

				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(tagGrid))
				{
					model.Tags = jss.Deserialize<List<PointTagEditGridModel>>(tagGrid);
				}

				// deserialize the alarms
				AlarmEditorModel alarms = new AlarmEditorModel();
				if (!string.IsNullOrEmpty(alarmModel))
				{
					var jsonSerializerSettings = new JsonSerializerSettings
					{
						MissingMemberHandling = MissingMemberHandling.Ignore
					};

					alarms = JsonConvert.DeserializeObject<AlarmEditorModel>(alarmModel, jsonSerializerSettings);

					//This is a hack to overcome limitations of serializing and deserilazing JSON objects using the
					//Newtonsoft JsonConvert.DeserializeObject method.  If &&s are in the string it causes an exception to be 
					//thrown.
					if (alarms != null)
					{
						foreach (var tag in alarms.Tags)
						{
							if (tag.Alarms == null){
								tag.Alarms = new List<AlarmEditorAlarmModel>();
							}
							foreach (var alarm in tag.Alarms)
							{
								if (alarm.AlarmTests == null)
								{
									alarm.AlarmTests = new List<AlarmEditorAlarmTestModel>();
								}
								foreach (var test in alarm.AlarmTests)
								{
									if (test.AlarmTestEquation.Contains("#"))
									{
										test.AlarmTestEquation = test.AlarmTestEquation.Replace("#", "&");
									}
								}
							}
						}
					}
				}

				// MVC does not support validation of a list of objects so it has to be done manually
				int i = 0;
				foreach (var tag in model.Tags)
				{

					if(!PointTemplateTag.IsNumeric(tag.DataType))
					{
						continue;
					}


					int index = model.PointTemplate.Tags.Keys.ToList().FindIndex(x => x == tag.PointTagGuid);

					var hasValues = true;
					// sometimes the negative symbol is a hyphen instead of a dash and the ParseDouble generates an exception
					tag.Minimum = tag.Minimum.Replace("–", "-");
					tag.Maximum = tag.Maximum.Replace("–", "-");

					if (tag.DecimalPlaces == null && tag.ApplyPointDecimalPlaces == false)
					{
						this.ModelState.AddModelError("Tags[" + index + "].DecimalPlaces", tag.Name + " " + TranslateText("PointEditor|Precision is required."));
					}

					if (string.IsNullOrEmpty(tag.Maximum) && tag.ApplyPointMaximum == false)
					{
						this.ModelState.AddModelError("Tags[" + index + "].Maximum", tag.Name + " " + TranslateText("PointEditor|Maximum is required."));
						hasValues = false;
					}
					if (string.IsNullOrEmpty(tag.Minimum) && tag.ApplyPointMinimum == false)
					{
						this.ModelState.AddModelError("Tags[" + index + "].Minimum", tag.Name + " " + TranslateText("PointEditor|Minimum is required."));
						hasValues = false;
					}

					// if the max or min are updatable and max is grater than the min
					if ((tag.ApplyPointMaximum == false || tag.ApplyPointMinimum == false) && hasValues && Double.Parse(tag.Maximum) < Double.Parse(tag.Minimum))
					{
						this.ModelState.AddModelError("Tags[" + index + "].Minimum", tag.Name + " " + TranslateText("PointEditor|Maximum is less than the Minimum."));
						this.ModelState.AddModelError("Tags[" + index + "].Maximum", tag.Name + " " + TranslateText("PointEditor|Maximum is less than the Minimum."));
					}

					// if the max and min are updatable and max is same as the min
					if ((tag.ApplyPointMaximum == false || tag.ApplyPointMinimum == false) && hasValues && Math.Abs(Double.Parse(tag.Maximum) - Double.Parse(tag.Minimum)) == 0.0)
					{
						this.ModelState.AddModelError("Tags[" + index + "].Minimum", tag.Name + " " + TranslateText("PointEditor|Minimum cannot be the same as the Maximum."));
						this.ModelState.AddModelError("Tags[" + index + "].Maximum", tag.Name + " " + TranslateText("PointEditor|Minimum cannot be the same as the Maximum."));
					}

					i++;
				}

				if (!string.IsNullOrEmpty(settings))
				{
					model.Properties = jss.Deserialize<List<PointPropertyEditModel>>(settings);
				}

				// get the default unit conversion history
				if (!string.IsNullOrEmpty(unitConversionHistory))
				{
					defaultUnitConversionHistory = jss.Deserialize<List<PointDefaultUnitChangeHistory>>(unitConversionHistory);
				}

				// get the moduleInstances
				var moduleInstanceList = new List<Guid>();
				if(!string.IsNullOrEmpty(moduleInstances))
				{
					moduleInstanceList = jss.Deserialize<List<Guid>>(moduleInstances);
				}

				/* modify the tags */
				if (this.ModelState.IsValid)
				{
					try
					{
						var tags = new Dictionary<Guid, PointTemplateTag>();

						foreach (var tag in model.Tags)
						{

							var pointTemplateTag = model.PointTemplate.Tags.FirstOrDefault(t => t.Key == tag.PointTagGuid);
							if (pointTemplateTag.Key != Guid.Empty)
							{
								pointTemplateTag.Value.ID = tag.Name;
								pointTemplateTag.Value.ValueTypeString = tag.DataType;
								pointTemplateTag.Value.EngineeringUnitsType = tag.EngineeringUnitsType;
								pointTemplateTag.Value.Units = tag.Unit;
								pointTemplateTag.Value.DecimalPlaces = Convert.ToByte(tag.DecimalPlaces);
								pointTemplateTag.Value.ServerUnits = tag.ServerUnit;
								pointTemplateTag.Value.Maximum = Double.Parse(tag.Maximum);
								pointTemplateTag.Value.Minimum = Double.Parse(tag.Minimum);
								pointTemplateTag.Value.InputOutputType = tag.InputOutputType;
								pointTemplateTag.Value.Input = tag.Input;
								pointTemplateTag.Value.InhibitInputOutputTypeConfiguration = !tag.InhibitInputOutputTypeConfiguration;
								pointTemplateTag.Value.InhibitOverride = !tag.InhibitOverride;
								pointTemplateTag.Value.Archived = tag.Archived;
								pointTemplateTag.Value.ApplyPointTemplateEngineeringUnits = tag.ApplyPointEngineeringUnits;
								pointTemplateTag.Value.ApplyPointTemplateDecimalPlaces = tag.ApplyPointDecimalPlaces;
								pointTemplateTag.Value.ApplyPointTemplateMaximum = tag.ApplyPointMaximum;
								pointTemplateTag.Value.ApplyPointTemplateMinimum = tag.ApplyPointMinimum;
								pointTemplateTag.Value.WellKnownIdentityGuid = tag.WellKnownIdentityGuidString.IsNullOrWhiteSpace() ? Guid.Empty : new Guid(tag.WellKnownIdentityGuidString);
								if (tag.Value != null)
								{
									this.SetValueForTemplateTag(pointTemplateTag.Value, jss.Deserialize<string>(tag.Value), site);
								}
								tags.Add(pointTemplateTag.Value.PointTemplateTagGuid, pointTemplateTag.Value);
							}
							else // we have a new tag
							{
								var newPointTemplateTag = new PointTemplateTag();
								newPointTemplateTag.PointTemplateGuid = model.IdentityGuid;
								newPointTemplateTag.PointTemplateTagGuid = tag.PointTagGuid;
								newPointTemplateTag.ID = tag.Name;
								newPointTemplateTag.ValueTypeString = tag.DataType;
								newPointTemplateTag.EngineeringUnitsType = tag.EngineeringUnitsType;
								newPointTemplateTag.Units = tag.Unit;
								newPointTemplateTag.DecimalPlaces = Convert.ToByte(tag.DecimalPlaces);
								newPointTemplateTag.ServerUnits = tag.ServerUnit;
								newPointTemplateTag.Maximum = Double.Parse(tag.Maximum);
								newPointTemplateTag.Minimum = Double.Parse(tag.Minimum);
								newPointTemplateTag.InputOutputType = tag.InputOutputType;
								newPointTemplateTag.Input = tag.Input;
								newPointTemplateTag.InhibitInputOutputTypeConfiguration = !tag.InhibitInputOutputTypeConfiguration;
								newPointTemplateTag.InhibitOverride = !tag.InhibitOverride;
								newPointTemplateTag.Archived = tag.Archived;
								newPointTemplateTag.Module = false;
								newPointTemplateTag.ApplyPointTemplateEngineeringUnits = tag.ApplyPointEngineeringUnits;
								newPointTemplateTag.ApplyPointTemplateDecimalPlaces = tag.ApplyPointDecimalPlaces;
								newPointTemplateTag.ApplyPointTemplateMaximum = tag.ApplyPointMaximum;
								newPointTemplateTag.ApplyPointTemplateMinimum = tag.ApplyPointMinimum;
								newPointTemplateTag.WellKnownIdentityGuid = tag.WellKnownIdentityGuidString.IsNullOrWhiteSpace() ? Guid.Empty : new Guid(tag.WellKnownIdentityGuidString);
								if (tag.Value != null)
								{
									this.SetValueForTemplateTag(newPointTemplateTag, jss.Deserialize<string>(tag.Value), site);
								}
								tags.Add(newPointTemplateTag.PointTemplateTagGuid, newPointTemplateTag);
							}
						}

						model.PointTemplate.Tags = tags;

						var moduleInstanceDictionary = new Dictionary<Guid, ModuleToPointTemplateMap>();

						var order = 1;
						foreach(var identityGuid in moduleInstanceList)
						{
							if(!model.PointTemplate.ModuleInstances.ContainsKey(identityGuid))
							{
								throw new Exception("PointTemplateEditor|Missing Module Instance Identifier - " + identityGuid.ToString());
							}
							moduleInstanceDictionary.Add(identityGuid, model.PointTemplate.ModuleInstances[identityGuid]);
							moduleInstanceDictionary[identityGuid].Order = order++;
						}

						model.PointTemplate.ModuleInstances = moduleInstanceDictionary;
					}
					catch (Exception except)
					{
						this.OnError(except);
						return this.JsonWithErrorMessages(null);
					}
				}

				// modify the point Template
				if (this.ModelState.IsValid)
				{
					if (model.IdentityGuid == Guid.Empty)
					{
						throw new Exception("GUID cannot be blank.");
					}

					model.PointTemplate.ID = model.Name;
					model.PointTemplate.Description = (model.Description == null) ? string.Empty : model.Description;

					// when copying a point template with derived points the Point Type Dropdown is disabled and model.PointTypeGuid is null.
					if (model.PointTypeGuid.HasValue)
					{
						model.PointTemplate.PointTemplateTypeGuid = model.PointTypeGuid;
					}

					model.PointTemplate.LevelUnit = model.LevelUnit;
					model.PointTemplate.LevelDecimalPlaces = (byte)model.LevelDecimalPlaces;
					model.PointTemplate.LevelMinimum = model.LevelMinimumRaw ?? 0;
					model.PointTemplate.LevelMaximum = model.LevelMaximumRaw ?? 0;

					model.PointTemplate.TemperatureUnit = model.TemperatureUnit;
					model.PointTemplate.TemperatureDecimalPlaces = (byte)model.TemperatureDecimalPlaces;
					model.PointTemplate.TemperatureMinimum = model.TemperatureMinimumRaw ?? 0;
					model.PointTemplate.TemperatureMaximum = model.TemperatureMaximumRaw ?? 0;

					model.PointTemplate.DensityUnit = model.DensityUnit;
					model.PointTemplate.DensityDecimalPlaces = (byte)model.DensityDecimalPlaces;
					model.PointTemplate.DensityMinimum = model.DensityMinimumRaw ?? 0;
					model.PointTemplate.DensityMaximum = model.DensityMaximumRaw ?? 0;

					model.PointTemplate.PressureUnit = model.PressureUnit;
					model.PointTemplate.PressureDecimalPlaces = (byte)model.PressureDecimalPlaces;
					model.PointTemplate.PressureMinimum = model.PressureMinimumRaw ?? 0;
					model.PointTemplate.PressureMaximum = model.PressureMaximumRaw ?? 0;

					model.PointTemplate.FlowUnit = model.FlowUnit;
					model.PointTemplate.FlowDecimalPlaces = (byte)model.FlowDecimalPlaces;
					model.PointTemplate.VolumetricFlowMinimum = model.VolumetricFlowMinimumRaw ?? 0;
					model.PointTemplate.VolumetricFlowMaximum = model.VolumetricFlowMaximumRaw ?? 0;

					model.PointTemplate.VolumeUnit = model.VolumeUnit;
					model.PointTemplate.VolumeDecimalPlaces = (byte)model.VolumeDecimalPlaces;
					model.PointTemplate.VolumeMinimum = model.VolumeMinimumRaw ?? 0;
					model.PointTemplate.VolumeMaximum = model.VolumeMaximumRaw ?? 0;

					model.PointTemplate.MassUnit = model.MassUnit;
					model.PointTemplate.MassDecimalPlaces = (byte)model.MassDecimalPlaces;
					model.PointTemplate.MassMinimum = model.MassMinimumRaw ?? 0;
					model.PointTemplate.MassMaximum = model.MassMaximumRaw ?? 0;

					model.PointTemplate.VelocityUnit = model.VelocityUnit;
					model.PointTemplate.VelocityDecimalPlaces = (byte)model.VelocityDecimalPlaces;
					model.PointTemplate.VelocityMinimum = model.VelocityMinimumRaw ?? 0;
					model.PointTemplate.VelocityMaximum = model.VelocityMaximumRaw ?? 0;

					model.PointTemplate.MassFlowUnit = model.MassFlowUnit;
					model.PointTemplate.MassFlowDecimalPlaces = (byte)model.MassFlowDecimalPlaces;
					model.PointTemplate.MassFlowMinimum = model.MassFlowMinimumRaw ?? 0;
					model.PointTemplate.MassFlowMaximum = model.MassFlowMaximumRaw ?? 0;

					model.PointTemplate.DefaultDrawingGuid = defaultDrawingGuid;
					model.PointTemplate.ProfileImageGuid = new Guid(profileImageGuid);

				}

				if (this.ModelState.IsValid)
				{
					// modify the properties (they depend on the default values) 
					try
					{
						foreach (var pointTemplateProperty in pointTemplate.Properties.Select(x => x.Value))
						{
							switch (pointTemplateProperty.ValueType.ToString())
							{
								case "FMBusinessObjects.DataObjects.StrapTable":
									StrapTableEditorController.ValidateStrapTable(this.ModelState, numberFormatInfo, pointTemplate, pointTemplateProperty.Value as StrapTable, defaultUnitConversionHistory);
									break;
								case "FMBusinessObjects.DataObjects.Vessel":
									VesselSettingsEditorController.ValidateVesselProperty(this.ModelState, numberFormatInfo, pointTemplate, pointTemplateProperty.Value as Vessel, defaultUnitConversionHistory);
									break;
								case "FMBusinessObjects.DataObjects.VcfModuleSettings":
									VcfSettingsEditorController.ValidateVCFModuleProperty(this.ModelState, numberFormatInfo, pointTemplate, pointTemplateProperty.Value as VcfModuleSettings, defaultUnitConversionHistory);
									break;
								case "FMBusinessObjects.DataObjects.QuantityModuleSettings":
									PointsController.ValidateQuantityModuleProperty(this.ModelState, numberFormatInfo, pointTemplate, model, pointTemplateProperty.PointTemplatePropertyGuid, pointTemplateProperty.Value as QuantityModuleSettings);
									break;
								case "FMBusinessObjects.DataObjects.TankTransferModuleSettings":
									PointsController.ValidateTankTransferModuleProperty(this.ModelState, numberFormatInfo, pointTemplate, model, pointTemplateProperty.PointTemplatePropertyGuid, pointTemplateProperty.Value as TankTransferModuleSettings);
									break;
								case "FMBusinessObjects.DataObjects.TankCommandModuleSettings":
									PointsController.ValidateTankCommandProperty(this.ModelState, numberFormatInfo, pointTemplate, model, pointTemplateProperty.PointTemplatePropertyGuid, pointTemplateProperty.Value as TankCommandModuleSettings);
									break;
								case "FMBusinessObjects.DataObjects.VolumeTransferModuleSettings":
									PointsController.ValidateVolumeTransferModuleProperty(this.ModelState, numberFormatInfo, pointTemplate, model, pointTemplateProperty.PointTemplatePropertyGuid, pointTemplateProperty.Value as VolumeTransferModuleSettings);
									break;
                                case "System.Boolean":
									pointTemplateProperty.Value = PointsController.ParsePointSettingBooleanValue(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), pointTemplateProperty.ID);
									break;
								case "System.DateTime":
									pointTemplateProperty.Value = PointsController.ParsePointSettingDateTimeValue(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.DateTimeOffset":
									pointTemplateProperty.Value = PointsController.ParsePointSettingDateTimeOffsetValue(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.TimeSpan":
									pointTemplateProperty.Value = PointsController.ParsePointSettingTimeSpanValue(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.String":
									pointTemplateProperty.Value = model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last();
									break;
								case "System.Double":
									pointTemplateProperty.Value = PointsController.ParsePointSettingDoubleValue(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.Single":
									pointTemplateProperty.Value = PointsController.ParsePointSettingSingleValue(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.Int64":
									pointTemplateProperty.Value = PointsController.ParsePointSettingInt64Value(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.Int32":
									pointTemplateProperty.Value = PointsController.ParsePointSettingInt32Value(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.Int16":
									pointTemplateProperty.Value = PointsController.ParsePointSettingInt16Value(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.UInt64":
									pointTemplateProperty.Value = PointsController.ParsePointSettingUInt64Value(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.UInt32":
									pointTemplateProperty.Value = PointsController.ParsePointSettingUInt32Value(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "System.UInt16":
									pointTemplateProperty.Value = PointsController.ParsePointSettingUInt16Value(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
								case "FMBusinessObjects.DataObjects.PointCommandStatusListReference":
									((PointCommandStatusListReference)pointTemplateProperty.Value).CurrentValue = PointsController.ParsePointSettingPointCommandStatusListReferenceValue(this.ModelState, pointTemplateProperty.ID, model.Properties.Where(x => x.PointPropertyGuid == pointTemplateProperty.PointTemplatePropertyGuid).Select(x => x.Value).Last(), site, pointTemplateProperty.ID);
									break;
							}
						}
					}
					catch (Exception except)
					{
						this.OnError(except);
						return this.JsonWithErrorMessages(null);
					}
				}

				alarms.AlarmCategories = DeviceAlarmMapsEditorController.GetAllAlarmCategories(this.Security);

				// process the alarms
				if (this.ModelState.IsValid)
				{

					foreach (var alarmTag in alarms.Tags)
					{
						var tag = pointTemplate.Tags[alarmTag.PointTagGuid];

						if (alarmTag.AlarmsEnabled != tag.AlarmsEnabled)
						{
							tag.AlarmsEnabled = alarmTag.AlarmsEnabled;
						}

						// delete the Alarms for the tag that are not part of the model anymore (deleted in the UI)
						var alarmsToDelete = tag.AlarmTemplates.Values.Where(o => !alarmTag.Alarms.Any(x => x.AlarmGuid == o.AlarmTemplateGuid));

						foreach (var alarm in alarmsToDelete.ToArray())
						{
							tag.AlarmTemplates.Remove(alarm.AlarmTemplateGuid);
						}

						// add or update the new alarms
						foreach (var modelAlarm in alarmTag.Alarms)
						{

							AlarmTemplate alarm = new AlarmTemplate();
							if (tag.AlarmTemplates.ContainsKey(modelAlarm.AlarmGuid))
							{
								alarm = tag.AlarmTemplates[modelAlarm.AlarmGuid];
							}
							else
							{
								tag.AlarmTemplates.Add(modelAlarm.AlarmGuid, alarm);
																
							}

							alarm.PointTemplateGuid = model.IdentityGuid;
							alarm.PointTemplateTagGuid = tag.PointTemplateTagGuid;
							alarm.InputTemplateTagGuid = tag.PointTemplateTagGuid;
							alarm.AlarmTemplateGuid = modelAlarm.AlarmGuid;
							alarm.ID = modelAlarm.Id;
							alarm.NotAlarmState = modelAlarm.NotAlarmState;
							alarm.Order = modelAlarm.Order;
							var alarmCategory = alarms.AlarmCategories.FirstOrDefault(x => x.Value == modelAlarm.Category).Key;
							alarm.AlarmCategoryApplicationStringGuid = alarmCategory;
							alarm.AlarmStateTemplateTagGuid = modelAlarm.AlarmStatusTagGuid;
							alarm.Enabled = modelAlarm.Enabled;
							
							// add or update the alarm tests
							foreach (var modelAlarmTest in modelAlarm.AlarmTests)
							{
								AlarmTestTemplate alarmTest = new AlarmTestTemplate();
								if (alarm.AlarmTestTemplates.ContainsKey(modelAlarmTest.AlarmTestGuid))
								{
									alarmTest = alarm.AlarmTestTemplates[modelAlarmTest.AlarmTestGuid];
								}
								else
								{
									alarm.AlarmTestTemplates.Add(modelAlarmTest.AlarmTestGuid, alarmTest);

									// new alarm test which requires a new entry in the alarm status
									PointTemplateTagAlarmStatus alarmStatus = new PointTemplateTagAlarmStatus();
									alarmStatus.PointTemplateTagAlarmStatusGuid = Guid.NewGuid();
									alarmStatus.IdentityGuid = alarmStatus.PointTemplateTagAlarmStatusGuid;
									alarmStatus.AlarmTestTemplateGuid = modelAlarmTest.AlarmTestGuid;
									alarm.AlarmStatusTemplates.Add(alarmStatus.PointTemplateTagAlarmStatusGuid, alarmStatus);
								}

								alarmTest.AlarmTemplateGuid = modelAlarm.AlarmGuid;
								alarmTest.AlarmTestTemplateGuid = modelAlarmTest.AlarmTestGuid;
								alarmTest.LimitTemplateTagGuid = modelAlarmTest.LimitTagGuid;
								alarmTest.PointTemplateGuid = model.IdentityGuid;
								alarmTest.PointTemplateTagGuid = tag.PointTemplateTagGuid;
								alarmTest.ID = modelAlarmTest.Id;
								alarmTest.AlarmPriorityGuid = modelAlarmTest.AlarmPriorityGuid;
								alarmTest.NormalUnacknowledgedAlarmPriorityGuid = modelAlarmTest.NormalUnacknowledgedAlarmPriorityGuid;
								alarmTest.BitMask = modelAlarmTest.BitMask;
								alarmTest.BitwiseOperator = modelAlarmTest.BitwiseOperator;
								alarmTest.TagField = modelAlarmTest.TagField;
								alarmTest.TestType = modelAlarmTest.TestType;
								alarmTest.Order = modelAlarmTest.Order;

								var limitTag = pointTemplate.Tags[alarmTest.LimitTemplateTagGuid];
								var hysteresis = GenerateHysteresisDoubleFromString(pointTemplate.Tags[alarmTag.PointTagGuid], pointTemplate, site, alarmTag.Hysteresis);
								var holdOffSeconds = GenerateHoldOffIntegerFromString(limitTag, pointTemplate, site, modelAlarmTest.HoldOffSeconds);
								var holdOffMinutes = GenerateHoldOffIntegerFromString(limitTag, pointTemplate, site, modelAlarmTest.HoldOffMinutes);
								var holdOff = (int)(holdOffMinutes * 60 + holdOffSeconds);


								if (modelAlarmTest.AlarmState != alarmTest.AlarmState || modelAlarmTest.AlarmText != alarmTest.AlarmText || hysteresis != alarmTest.Holdoff || holdOff != alarmTest.TimedHoldOffInSeconds)
								{
									alarmTest.AlarmState = modelAlarmTest.AlarmState;
									alarmTest.AlarmText = modelAlarmTest.AlarmText;
									alarmTest.Holdoff = hysteresis;
									alarmTest.TimedHoldOffInSeconds = holdOff;
									//Need to do alarm equation stuff
								}

							}

							// delete the Alarm Tests that are not part of the model anymore (deleted in the UI)
							var alarmTestsToDelete = alarm.AlarmTestTemplates.Values.Where(o => !modelAlarm.AlarmTests.Any(x => x.AlarmTestGuid == o.AlarmTestTemplateGuid));

							foreach (var alarmTest in alarmTestsToDelete.ToArray())
							{
								alarm.AlarmTestTemplates.Remove(alarmTest.AlarmTestTemplateGuid);
							}

						}
					}

					// delete all the Alarms for tags that don't have alarms anymore (deleted in the UI)
					var tagsToRemoveAlarms = model.PointTemplate.Tags.Values.Where(o => !alarms.Tags.Any(x => x.PointTagGuid == o.PointTemplateTagGuid));
					foreach (var tag in tagsToRemoveAlarms.ToArray())
					{
						tag.AlarmTemplates = new Dictionary<Guid, AlarmTemplate>();
					}
				}
				else // if failed validation
				{
					return this.JsonWithErrorMessages(null);
				}

				if (this.ModelState.IsValid)
				{
					string resultMessage;
					if (this.ReorderModuleInstances(model.PointTemplate))
					{
						modulesView = this.RenderRazorViewToString("PointCustomTemplateDetailModules", model);
						results.Add(new KeyValuePair<string, string>("modulesView", modulesView));
						resultMessage = "PointTemplateEditor|Save Successful, module instances reordered";
					}
					else
					{
						resultMessage = "PointTemplateEditor|Save Successful";
					}

					if (!copyWithNewNameFlag)
						FMChannelHelper.MakeCall<IPointTemplates>(x => x.Modify(this.Security, model.PointTemplate));
					else //Save As functionality
					{
						//track guid changes for relationships
						List<Guid[]> guidTable = new List<Guid[]>();

						//assign new guid and update the name
						if (pointTemplate.IdentityGuid == new Guid("0ADB4947-1CC4-4A44-91F8-E76F281EA718")) //standard tank template
						{
							model.PointTemplate.PointTemplateTypeGuid = new Guid("E78CD406-4C19-4978-8940-FA4E404E3E53"); //set it to tank
						}
						else
						{
							model.PointTemplate.PointTemplateTypeGuid = pointTemplate.PointTemplateTypeGuid;
						}
						model.PointTemplate.PointTemplateGuid = Guid.NewGuid();
						model.PointTemplate.ID = model.Name;
						model.PointTemplate.DefaultDrawingGuid = null;



						//the new template can be deleted
						model.PointTemplate.Standard = false;

						//reset these variables in case the user saves again, altough the redirect should take care of this
						copyWithNewNameFlag = false;
						model.IdentityGuid = model.PointTemplate.PointTemplateGuid;

						//give new guids to all the tags
						List<Guid> tagGuidsToDelete = new List<Guid>();
						List<PointTemplateTag> tagTemplatesToAdd = new List<PointTemplateTag>();
						foreach (var tag in model.PointTemplate.Tags)
						{
							tagGuidsToDelete.Add(tag.Value.IdentityGuid);
							guidTable.Add(new Guid[] { tag.Value.IdentityGuid, tag.Value.IdentityGuid = Guid.NewGuid() });
							tag.Value.PointTemplateGuid = model.PointTemplate.PointTemplateGuid;

							//give new guids to all the alarms, and point them to the new tag/template
							List<Guid> alarmTemplateGuidsToDelete = new List<Guid>();
							List<AlarmTemplate> alarmTemplatestoAdd = new List<AlarmTemplate>();
							foreach (var alarmTemplate in tag.Value.AlarmTemplates)
							{
								alarmTemplateGuidsToDelete.Add(alarmTemplate.Value.IdentityGuid);
								alarmTemplate.Value.IdentityGuid = Guid.NewGuid();
								alarmTemplate.Value.PointTemplateGuid = model.PointTemplate.PointTemplateGuid;
								alarmTemplate.Value.PointTemplateTagGuid = tag.Value.IdentityGuid;
								alarmTemplatestoAdd.Add(alarmTemplate.Value);

								//give new guids to all the alarm tests and point them to the new alarm/tag/template
								List<Guid> alarmTemplateTestGuidstoDelete = new List<Guid>();
								List<AlarmTestTemplate> alarmTemplateTeststoAdd = new List<AlarmTestTemplate>();
								foreach (var alarmTemplateTest in alarmTemplate.Value.AlarmTestTemplates)
								{
									alarmTemplateTestGuidstoDelete.Add(alarmTemplateTest.Value.IdentityGuid);
									guidTable.Add(new Guid[] { alarmTemplateTest.Value.IdentityGuid, alarmTemplateTest.Value.IdentityGuid = Guid.NewGuid()	});

									alarmTemplateTest.Value.PointTemplateTagGuid = tag.Value.IdentityGuid;
									alarmTemplateTest.Value.AlarmTemplateGuid = alarmTemplate.Value.IdentityGuid;
									alarmTemplateTest.Value.PointTemplateGuid = model.PointTemplate.PointTemplateGuid;
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
							model.PointTemplate.Tags.Remove(guidtoDelete);
						}
						foreach (PointTemplateTag tagToAdd in tagTemplatesToAdd)
						{
							model.PointTemplate.Tags.Add(tagToAdd.IdentityGuid, tagToAdd);
						}

						//repair alarm and alarmtest to tag relationships. This is why we tracked the guid changes.
						foreach (var tag in model.PointTemplate.Tags)
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
						foreach (var property in model.PointTemplate.Properties)
						{
							propertyGuidstoDelete.Add(property.Value.IdentityGuid);
							guidTable.Add(new Guid[] { property.Value.IdentityGuid, property.Value.IdentityGuid = Guid.NewGuid() });
							propertiesToAdd.Add(property.Value);
						}
						foreach (var propertyGuidtoDelete in propertyGuidstoDelete)
						{
							model.PointTemplate.Properties.Remove(propertyGuidtoDelete);
						}
						foreach (var propertyToAdd in propertiesToAdd)
						{
							model.PointTemplate.Properties.Add(propertyToAdd.IdentityGuid, propertyToAdd);
						}

						//give new guids to all the moduleInstances
						List<Guid> moduleInstanceGuidstoDelete = new List<Guid>();
						List<ModuleToPointTemplateMap> moduleInstancestoAdd = new List<ModuleToPointTemplateMap>();
						foreach (var moduleInstance in model.PointTemplate.ModuleInstances)
						{
							moduleInstanceGuidstoDelete.Add(moduleInstance.Value.IdentityGuid);
							moduleInstance.Value.IdentityGuid = Guid.NewGuid(); 
							moduleInstance.Value.PointTemplateGuid = model.PointTemplate.PointTemplateGuid;

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
							model.PointTemplate.ModuleInstances.Remove(moduleInstanceGuidtoDelete);
						}
						moduleInstancestoAdd.Reverse(); //moduleInstance order matters for point script execution
						foreach (var moduleInstancetoAdd in moduleInstancestoAdd)
						{
							model.PointTemplate.ModuleInstances.Add(moduleInstancetoAdd.IdentityGuid, moduleInstancetoAdd);
						}

						FMChannelHelper.MakeCall<IPointTemplates>(x => x.Add(this.Security, model.PointTemplate));

						resultMessage = "PointTemplateEditor|Copy Successful";
						//This key tells FMErrorAndExceptionHandling.HandleMessages to redirect the page to the new PointTemplate
						results.Add(new KeyValuePair<string, string>("redirectGuid", model.PointTemplate.IdentityGuid.ToString()));
					}

					this.AddSuccess(this.GetTranslatedText(resultMessage));
				}
				else // if failed validation
				{
					return this.JsonWithErrorMessages(null);
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);

			}

			return this.JsonWithErrorMessages(results, JsonRequestBehavior.AllowGet);
		}


		#region AlarmConfiguration and Editing

		[NonAction]
		private AlarmEditorModel GetTemplateAlarmModel(SecurityClass security, PointTemplate pointTemplate, SiteClass site)
		{
			var model = new AlarmEditorModel { Tags = new List<AlarmEditorTagModel>() };

			model.AlarmCategories = DeviceAlarmMapsEditorController.GetAllAlarmCategories(security);
			var drawings = this.GetDrawingIds(security, pointTemplate);
			model.NumberDecimalSeparator = site.NumberDecimalSeparator;
			model.NumberGroupSeparator = site.NumberGroupSeparator;
			model.NumberGroupSizes = site.GetNumberGroupSizes();
			model.DecimalPlaces = 2;
			model.ShortDatePattern = site.ShortDatePattern;
			model.PointGuid = pointTemplate.PointTemplateGuid;
			model.PointTemplateGuid = pointTemplate.PointTemplateGuid;
			this.FillInModelTagsForAlarmModel(pointTemplate, model.AlarmCategories, drawings, model, site);

			model.AlarmPriorities = new AlarmPriorityCollectionClass();
			model.NormalPriorities = new AlarmPriorityCollectionClass();

			var allAlarmPriorityDictionary = DeviceAlarmMapsEditorController.GetAllAlarmPriorities(this.Security);
			foreach (var alarmPriority in allAlarmPriorityDictionary.Values)
			{
				if (alarmPriority.Priority.HasValue)
				{
					model.AlarmPriorities.Add(alarmPriority);
				}
				else
				{
					model.NormalPriorities.Add(alarmPriority);
				}
			}

			model.HasEnableAlarmOnPointTemplateRight = this.Security.HasRight(RIGHT.ENABLE_ALARMS_ON_POINT_TEMPLATES);
			model.HasDisableAlarmOnPointTemplateRight = this.Security.HasRight(RIGHT.DISABLE_ALARMS_ON_POINT_TEMPLATES);
			model.HasPTEditRight = this.Security.HasRight(RIGHT.MODIFY_POINT_TEMPLATES);
			model.HasModifyEnabled = this.Security.SiteGuid == pointTemplate.SiteGuid || pointTemplate.PointTemplateGuid == Guid.Empty;
			return model;
		}


		[NonAction]
		private Dictionary<Guid, string> GetDrawingIds(SecurityClass security, PointTemplate point)
		{
			var drawingDictionary = new Dictionary<Guid, string>();
			var drawingGuidList = new List<Guid>();
			foreach (var tag in point.Tags.Values)
			{
				foreach (var alarm in tag.AlarmTemplates.Values)
				{
					foreach (var test in alarm.AlarmTestTemplates.Values)
					{
						if (test.DrawingGuid != null && test.DrawingGuid != Guid.Empty)
						{
							if (!drawingGuidList.Contains((Guid)test.DrawingGuid))
							{
								drawingGuidList.Add((Guid)test.DrawingGuid);
							}
						}
					}
				}
			}
			if (drawingGuidList.Count > 0)
			{
				var drawingDict =
					FMChannelHelper.MakeCall<IDrawings, Dictionary<Guid, DrawingName>>(
						x => x.EnumerateByDrawingGuids(security, drawingGuidList));
				foreach (var drawing in drawingDict.Values)
				{
					drawingDictionary.Add(drawing.DrawingGuid, drawing.ID);
				}
			}

			return drawingDictionary;
		}


		[NonAction]
		private void FillInModelTagsForAlarmModel(PointTemplate pointTemplate, Dictionary<Guid, string> categories, Dictionary<Guid, string> drawings, AlarmEditorModel model, SiteClass s)
		{
			foreach (var tag in pointTemplate.Tags.Values)
			{
				if (tag.AlarmTemplates != null && tag.AlarmTemplates.Count > 0)
				{
					var firstAlarmTeset = tag.AlarmTemplates.First().Value.AlarmTestTemplates.First().Value;
					var limitTag = pointTemplate.Tags[firstAlarmTeset.LimitTemplateTagGuid];
					var hysteresisStr = GenerateHysteresisString(tag, pointTemplate, s, firstAlarmTeset.Holdoff);
					var modelTag = new AlarmEditorTagModel { PointTagGuid = tag.PointTemplateTagGuid, Id = tag.ID, Hysteresis = hysteresisStr, AlarmsEnabled = tag.AlarmsEnabled, TagDecimalPlaces = tag.DecimalPlaces, TagUnits = tag.Units, DataType = tag.ValueTypeString};
					this.FillInModelAlarms(pointTemplate, tag, categories, drawings, modelTag, s);
					model.Tags.Add(modelTag);
				}
			}
		}

		[NonAction]
		private void FillInModelAlarms(PointTemplate pointTemplate, PointTemplateTag tag, Dictionary<Guid, string> categories, Dictionary<Guid, string> drawings, AlarmEditorTagModel modelTag, SiteClass s)
		{
			modelTag.Alarms = new List<AlarmEditorAlarmModel>();
			foreach (var alarm in tag.AlarmTemplates.Values)
			{
				var modelAlarm = new AlarmEditorAlarmModel
				{
					Id = alarm.ID,
					AlarmGuid = alarm.AlarmTemplateGuid,
					Order = alarm.Order,
					AlarmStatusTagGuid = alarm.AlarmStateTemplateTagGuid,
					Enabled = alarm.Enabled,
					Exclusive = true, //Need to update this when we implement Exclusive, Non-Exclusive functionality
					NotAlarmState = alarm.NotAlarmState
				};
				string category;
				if (categories.TryGetValue(alarm.AlarmCategoryApplicationStringGuid, out category))
				{
					modelAlarm.Category = category;
				}
				var suppressedAndShelvedStatus = ActiveAlarmString;
				if (alarm.Suppressed)
				{
					suppressedAndShelvedStatus = SuppressedString;
				}
				else
				{
					if (alarm.ShelvedOneShot)
					{
						suppressedAndShelvedStatus = OneShelvedString;
					}
					else
					{
						if (alarm.ShelvedEndTimeStamp > DateTimeOffset.Now)
						{
							//need to output time is site format
							suppressedAndShelvedStatus = TimeShelvedString + alarm.ShelvedEndTimeStamp.ToString();
						}
					}
				}
				modelAlarm.SuppressedAndShelvedStatus = suppressedAndShelvedStatus;

				PointTemplateTag alarmStatusTag;
				if (pointTemplate.Tags.TryGetValue(alarm.AlarmStateTemplateTagGuid, out alarmStatusTag))
				{
					modelAlarm.AlarmStatusTagId = alarmStatusTag.ID;
				}
				FillInModelTests(pointTemplate, tag, alarm, drawings, modelAlarm, s);
				modelTag.Alarms.Add(modelAlarm);
			}
			modelTag.Alarms = modelTag.Alarms.OrderBy(o => o.Id).ToList();
		}

		[NonAction]
		private void FillInModelTests(
			PointTemplate point,
			PointTemplateTag inputTag,
			AlarmTemplate alarm,
			Dictionary<Guid, string> drawings,
			AlarmEditorAlarmModel modelAlarm,
			SiteClass s)
		{
			modelAlarm.AlarmTests = new List<AlarmEditorAlarmTestModel>();
			foreach (var test in alarm.AlarmTestTemplates.Values)
			{
				
				var modelTest = new AlarmEditorAlarmTestModel
				{
					AlarmTestGuid = test.AlarmTestTemplateGuid,
					Id = test.ID,
					LimitTagGuid = test.LimitTemplateTagGuid,
					AlarmState = test.AlarmState,
					AlarmText = test.AlarmText,
					Order = test.Order,
					HoldOff = test.Holdoff,
					HelpFile = test.HelpFile,
					DrawingGuid = test.DrawingGuid == null ? Guid.Empty : (Guid)test.DrawingGuid,
					Enabled = test.Enabled,
					AlarmPriorityGuid = test.AlarmPriorityGuid,
					NormalUnacknowledgedAlarmPriorityGuid = test.NormalUnacknowledgedAlarmPriorityGuid,
					BitMask = test.BitMask,
					BitwiseOperator = test.BitwiseOperator,
					TagField = test.TagField,
					TestType = test.TestType
				};
				PointTemplateTag limitTag;
				if (point.Tags.TryGetValue(test.LimitTemplateTagGuid, out limitTag))
				{
					var holdOffSeconds = (int)(test.TimedHoldOffInSeconds % 60);
					var holdOffMinutes = (int)(test.TimedHoldOffInSeconds / 60);
					var holdOffSecondsStr = GenerateHoldOffString(limitTag, point, s, holdOffSeconds);
					var holdOffMinutesStr = GenerateHoldOffString(limitTag, point, s, holdOffMinutes);
					modelTest.HoldOffMinutes = holdOffMinutesStr;
					modelTest.HoldOffSeconds = holdOffSecondsStr;
					modelTest.LimitTagId = limitTag.ID;
					modelTest.AlarmTestEquation = this.GenerateTestEquationString(point, inputTag, limitTag, test, s);
					modelTest.LimitValue = this.GenerateValueString(limitTag, point, s);
					modelTest.LimitTagDecimalPlaces = limitTag.GetDecimalPlaces(point);
					modelTest.LimitTagUnitsType = limitTag.EngineeringUnitsType;
					modelTest.LimitTagUnits = limitTag.Units;
					modelTest.LimitTagMax = limitTag.GetMaximum(point);
					modelTest.LimitTagMin = limitTag.GetMinimum(point);
					modelTest.LimitTagValueType = limitTag.ValueTypeString;
					modelTest.LimitTagEditable = limitTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.Manual;
				}
				if (test.DrawingGuid != null && test.DrawingGuid != Guid.Empty)
				{
					string drawing;
					if (drawings.TryGetValue((Guid)test.DrawingGuid, out drawing))
					{
						modelTest.DrawingId = drawing;
					}
				}
				modelAlarm.AlarmTests.Add(modelTest);
			}
		}

		[NonAction]
		private static string GenerateHysteresisString(PointTemplateTag inputTag, PointTemplate p, SiteClass s, double hysteresis)
		{
			var valueStr = hysteresis.ToString();
			if (inputTag != null)
			{
				var units = inputTag.GetEngineeringUnits(p);
				var decimalPlaces = inputTag.GetDecimalPlaces(p);
				if (inputTag.ValueTypeString == "System.Double" || inputTag.ValueTypeString == "System.Single")
				{
					decimalPlaces = 9;
				}
				var numFormatProvider = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
				var val = EngineeringUnitsHelperClass.FormatValue(hysteresis, inputTag.Units);
				var formattedString =  s.FormatValue(val, decimalPlaces);
				if(formattedString.IndexOf(numFormatProvider.NumberDecimalSeparator) > -1)
				{
					// remove the trailing '0'
					formattedString = formattedString.TrimEnd('0');
					if(formattedString.IndexOf(numFormatProvider.NumberDecimalSeparator) == formattedString.Length - 1)
					{
						formattedString = formattedString.Remove(formattedString.Length - 1, 1);
					}
				}
				return formattedString;
			}
			return valueStr;
		}

		[NonAction]
		private static string GenerateHoldOffString(PointTemplateTag limitTag, PointTemplate p, SiteClass s, int holdOff)
		{
			var valueStr = holdOff.ToString();
			if (limitTag != null)
			{
				var units = limitTag.GetEngineeringUnits(p);
				var decimalPlaces = limitTag.GetDecimalPlaces(p);
				if (limitTag.ValueTypeString == "System.Double" || limitTag.ValueTypeString == "System.Single")
				{
					decimalPlaces = 9;
				}
				var numFormatProvider = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
				var val = EngineeringUnitsHelperClass.FormatValue(holdOff, limitTag.Units);
				var formattedString = s.FormatValue(val, decimalPlaces);
				if (formattedString.IndexOf(numFormatProvider.NumberDecimalSeparator) > -1)
				{
					// remove the trailing '0'
					formattedString = formattedString.TrimEnd('0');
					if (formattedString.IndexOf(numFormatProvider.NumberDecimalSeparator) == formattedString.Length - 1)
					{
						formattedString = formattedString.Remove(formattedString.Length - 1, 1);
					}
				}
				return formattedString;
			}
			return valueStr;
		}

		[NonAction]
		private string GenerateValueString(PointTemplateTag limitTag, PointTemplate pointTemplate, SiteClass site)
		{
			return limitTag.Value == null ? null : limitTag.Value.ToString();
		}

		[NonAction]
		private string GenerateTestEquationString(PointTemplate point, PointTemplateTag inputTag, PointTemplateTag limitTag, AlarmTestTemplate test, SiteClass s)
		{
			string limitTagId = limitTag != null ? limitTag.ID : "Unknown";
			var equation = inputTag.ID + "." + test.TagField + GenerateBitmaskString(inputTag, test) + this.GenerateComparisonString(test.TestType) + limitTagId + "." + AlarmTestTemplate.TagFieldEnum.Value;
			return equation;
		}

		[NonAction]
		private static string GenerateBitmaskString(PointTemplateTag inputTag, AlarmTestTemplate test)
		{
			var bitmask = test.BitMask;
			if (bitmask == -1)
			{
				return "";
			}

			var bitmaskPrefix = " & ";

			switch (test.BitwiseOperator)
			{
				case AlarmTestTemplate.BitwiseOperatorEnum.And:
					bitmaskPrefix = " & ";
					break;
				case AlarmTestTemplate.BitwiseOperatorEnum.Or:
					bitmaskPrefix = " | ";
					break;
				case AlarmTestTemplate.BitwiseOperatorEnum.Nor:
					bitmaskPrefix = " NOR ";
					break;
				case AlarmTestTemplate.BitwiseOperatorEnum.Xor:
					bitmaskPrefix = " ^ ";
					break;
				default:
					bitmaskPrefix = "";
					break;
			}

			var bitmaskPostfix = "";

			Type valueToCompareType;
			switch (test.TagField)
			{
				case AlarmTestTemplate.TagFieldEnum.OpcStatusSubCode:
					valueToCompareType = typeof(uint); // PointTag.OpcStatusSubCode is of type uint;
					break;
				case AlarmTestTemplate.TagFieldEnum.Status:
					valueToCompareType = typeof(Int32); // PointTag.Status is Int32;
					break;
				default:
					valueToCompareType = inputTag.ValueType;
					break;
			}
			if (valueToCompareType == typeof(sbyte))
			{
				var bitmaskObj = (sbyte)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X1");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(short))
			{
				var bitmaskObj = (short)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X4");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(int))
			{
				var bitmaskObj = (int)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X8");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(long))
			{
				var bitmaskObj = (long)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X16");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(byte))
			{
				var bitmaskObj = (byte)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X1");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(ushort))
			{
				var bitmaskObj = (ushort)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X4");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(uint) || valueToCompareType == typeof(DeviceAlarmMapReference))
			{
				var bitmaskObj = (uint)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X8");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(ulong))
			{
				var bitmaskObj = (ulong)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X16");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			if (valueToCompareType == typeof(Int64))
			{
				var bitmaskObj = (Int64)bitmask;
				var bitMaskStr = bitmaskObj.ToString("X16");
				return bitmaskPrefix + bitMaskStr + bitmaskPostfix;
			}

			return "";
		}

		[NonAction]
		private string GenerateComparisonString(AlarmTestTemplate.TestTypeEnum comparison)
		{
			switch (comparison)
			{
				case AlarmTestTemplate.TestTypeEnum.GreaterThan:
					return " > ";
				case AlarmTestTemplate.TestTypeEnum.GreaterThanOrEqual:
					return " >= ";
				case AlarmTestTemplate.TestTypeEnum.LessThan:
					return " < ";
				case AlarmTestTemplate.TestTypeEnum.LessThanOrEqual:
					return " <= ";
				case AlarmTestTemplate.TestTypeEnum.Equals:
					return " = ";
				case AlarmTestTemplate.TestTypeEnum.NotEquals:
					return " != ";
				default:
					return " Undefined ";
			}
		}

		[NonAction]
		private static double GenerateHysteresisDoubleFromString(PointTemplateTag sourceTag, PointTemplate p, SiteClass s, string hysteresis)
		{
			var numFormatInfo = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME);
			numFormatInfo.NumberDecimalDigits = sourceTag.GetDecimalPlaces(p);
			// only numeric values have hysteresis
			if (sourceTag.ValueTypeString != "System.Double" && sourceTag.ValueTypeString != "System.Int16"
			    && sourceTag.ValueTypeString != "System.Int32" && sourceTag.ValueTypeString != "System.Int64")
			{
				return 0;
			}
			return (double)EngineeringUnitsHelperClass.ParseValue(typeof(double), hysteresis, sourceTag.Units, numFormatInfo);
		}

		[NonAction]
		private static int GenerateHoldOffIntegerFromString(PointTemplateTag limitTag, PointTemplate p, SiteClass s, string holdOff)
		{
			var numFormatInfo = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME);
			return (int)EngineeringUnitsHelperClass.ParseValue(typeof(int), holdOff, limitTag.Units, numFormatInfo);
		}

		[NonAction]
		private static object GetLimitValue(PointTemplateTag limitTag, PointTemplate p, SiteClass s, string modelLimitValue)
		{
			var numFormatInfo = s.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME);
			numFormatInfo.NumberDecimalDigits = limitTag.GetDecimalPlaces(p);
			return EngineeringUnitsHelperClass.ParseValue(limitTag.ValueType, modelLimitValue, limitTag.Units, numFormatInfo);
		}

		#endregion

		#region Parse Different types of Properties for validation and saving


		private void ValidateQuantityModuleProperty(NumberFormatInfo numberFormatInfo, PointEditDetailModel model, Guid pointPropertyGuid, QuantityModuleSettings quantityValue)
		{

			// get all the settings in the UI for the Quantity setting
			var propertyList = model.Properties.Where(x => x.PointPropertyGuid == pointPropertyGuid);

			if (propertyList.Any(x => x.Name == "VolumeCalculationType"))
			{
				// CorrectionMethodSpecific is a enum TankMaterialEnum
				quantityValue.VolumeCalculationType = PointsController.ParsePointSettingEnumValue<VolumeCalculationType>(this.ModelState,
																																							"VolumeCalculationType",
																																							propertyList.Where(x => x.Name == "VolumeCalculationType").Select(x => x.Value).Last(),
																																							"Volume Calculation Type");
			}

			if (propertyList.Any(x => x.Name == "MassOrWeightCalculationType"))
			{
				// CorrectionMethodSpecific is a enum TankMaterialEnum
				quantityValue.MassOrWeightCalculationType = PointsController.ParsePointSettingEnumValue<MassOrWeightCalculationType>(this.ModelState,
																																											"MassOrWeightCalculationType",
																																											propertyList.Where(x => x.Name == "MassOrWeightCalculationType").Select(x => x.Value).Last(),
																																											"Mass/Weight Calculation Type");
			}

		}

		public bool ReorderModuleInstances(PointTemplate pointTemplate)
		{
			bool anyChanges = false;

			while (true)
			{
				var orderedModuleInstances = pointTemplate.ModuleInstances.OrderBy(x => x.Value.Order);
				var moduleInstances = new Dictionary<Guid, ModuleToPointTemplateMap>();
				int order = 1;
				bool moduleInstancesChanged = false;
				foreach (var moduleInstanceKeyValuePair in orderedModuleInstances)
				{
					var module = pointTemplate.Modules[moduleInstanceKeyValuePair.Value.ModuleGuid];
					foreach (var dependency in module.ModuleData.Dependencies)
					{
						var dependentModuleInstances = pointTemplate.ModuleInstances.Values.Where(x => x.ModuleGuid == dependency);
						foreach (var dependentModuleInstance in dependentModuleInstances)
						{
							if (moduleInstanceKeyValuePair.Value.Order < dependentModuleInstance.Order)
							{
								if (!moduleInstances.ContainsKey(dependentModuleInstance.ModuleToPointTemplateGuid))
								{
									if (dependentModuleInstance.Order != order)
									{
										dependentModuleInstance.Order = order;
										moduleInstancesChanged = true;
										anyChanges = true;
									}
									order++;
									moduleInstances.Add(dependentModuleInstance.ModuleToPointTemplateGuid, dependentModuleInstance);
								}
							}
						}
					}

					// moduleInstances may contain moduleInstance if added as a dependency
					if (moduleInstances.ContainsKey(moduleInstanceKeyValuePair.Key))
					{
						continue;
					}

					if (moduleInstanceKeyValuePair.Value.Order != order)
					{
						moduleInstanceKeyValuePair.Value.Order = order;
						moduleInstancesChanged = true;
						anyChanges = true;
					}

					order++;
					moduleInstances.Add(moduleInstanceKeyValuePair.Key, moduleInstanceKeyValuePair.Value);
				}

				pointTemplate.ModuleInstances = moduleInstances;

				if (!moduleInstancesChanged)
				{
					break;
				}
			}

			return anyChanges;
		}

		#endregion

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult PointsCustomTemplateAddModule(string id, string moduleGuid, string InstanceName, string tagList, string settingsList)
		{
			var tagView = "";
			var settingsView = "";
			var modulesView = "";
			var results = new List<KeyValuePair<string, string>>();
			try
			{ 

				var pointTemplateGuid = new Guid(id);
				var newModuleGuid = new Guid(moduleGuid);

				var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
				var module = FMChannelHelper.MakeCall<IModules, Module>(x => x.Get(this.Security, newModuleGuid));

				var Tags = new List<PointTemplateTagResolveGridModel>();
				var settings = new List<PointTemplateSettingResolveGridModel>();

				// check if the instance name is the template name
				if(InstanceName == pointTemplate.ID)
				{
					this.ModelState.AddModelError(
						"",
						InstanceName + ": " + TranslateText("PointTemplateEditor|Module Instance Name is the same as the Point Template Name."));
				}

				// check if the instance name is already in use.
				if (pointTemplate.ModuleInstances.Select(x => x.Value).Any(x => x.ID == InstanceName))
				{
					this.ModelState.AddModelError(
						"",
						InstanceName + ": " + TranslateText("PointTemplateEditor|There is a Module Instance with the same name."));
				}

				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(tagList))
				{
					Tags = jss.Deserialize<List<PointTemplateTagResolveGridModel>>(tagList);
					// MVC does not support validation of a list of objects so it has to be done manually
					int i = 0;
					foreach (var tag in Tags)
					{
						if (tag.ParameterName == "")
						{
							this.ModelState.AddModelError(
								"",
								tag.ParameterName + " " + TranslateText("PointTemplateEditor|Missing Parameter Name."));
						}
						else if(tag.TagName == pointTemplate.ID)
						{
							this.ModelState.AddModelError(
								"",
								InstanceName + ": " + TranslateText("PointTemplateEditor|Tag Name is the same as the Point Template Name."));
						}


						if (tag.TagName == "" && tag.TagGuid == null)
						{
							this.ModelState.AddModelError(
								"",
								tag.ParameterName + " " + TranslateText("PointTemplateEditor|Missing Tag identifier."));

						}
						else if (tag.TagName != "" && pointTemplate.Tags.Select(x => x.Value).Any(x => x.ID == tag.TagName))
						{
							this.ModelState.AddModelError(
								"",
								tag.TagName + ": " + TranslateText("PointTemplateEditor|There is a tag with the same name."));
						}
						i++;
					}

				}

				if (!string.IsNullOrEmpty(settingsList))
				{
					settings = jss.Deserialize<List<PointTemplateSettingResolveGridModel>>(settingsList);
					int i = 0;
					foreach (var setting in settings)
					{
						var propertyNewName = setting.newSettingName;
						if (module.ID != InstanceName)
						{
							propertyNewName = InstanceName + " " + propertyNewName;
						}

						if (propertyNewName == "")
						{
							this.ModelState.AddModelError("", TranslateText("PointTemplateEditor|Missing Settings Name."));
						}
						else if (propertyNewName == pointTemplate.ID)
						{
							this.ModelState.AddModelError(
								"",
								InstanceName + ": " + TranslateText("PointTemplateEditor|Setting Name is the same as the Point Template Name."));
						}


						if (setting.newSettingName == "" && setting.newSettingGuid == null)
						{
							this.ModelState.AddModelError(
								"",
								setting.SettingType + " " + TranslateText("PointTemplateEditor|Missing Setting identifier."));
						}

						i++;
					}
				}
				if (this.ModelState.IsValid)
				{

					try
					{
						/* Add the tags */
						var newTagList = new List<PointTemplateTag>();
						foreach (var tag in Tags)
						{
							// if the tagGuid is null then we need to create a template tag from the module definition using the defaults
							if (tag.TagGuid == null)
							{

								var moduleTagDefaults = module.ModuleData.ModuleTags.FirstOrDefault(x => x.ParameterName == tag.ParameterName);

								// get the following values from the module definitions
								if (moduleTagDefaults != null)
								{
									string valueTypeString = moduleTagDefaults.DataType;
									PointTemplateTag.PointTagInputOutputType inputOutputType = moduleTagDefaults.InputOutputType;
									bool input = moduleTagDefaults.Input;
									EngineeringUnit unit = EngineeringUnit.FmuNone;
									byte decimalPlaces = 0;
									EngineeringUnit serverUnit = EngineeringUnit.FmuNone;
									double maximum = 0.0;
									double minimum = 0.0;

									if (valueTypeString == "System.Double"  || valueTypeString == "System.Single")
									{
										switch (tag.EngineeringUnitsType)
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
												if(valueTypeString == "System.Double")
												{
													decimalPlaces = 2;
													maximum = moduleTagDefaults.Maximum;
													minimum = moduleTagDefaults.Minimum;
												}
												else
												{
													decimalPlaces = 2;
													maximum = moduleTagDefaults.Maximum;
													minimum = moduleTagDefaults.Minimum;
												}
												break;
										}
									}
									else
									{
										maximum = moduleTagDefaults.Maximum;
										minimum = moduleTagDefaults.Minimum;
									}

									/* Add the module reference to the point template */
									var pointTemplateTag = new PointTemplateTag();
									pointTemplateTag.PointTemplateGuid = pointTemplateGuid;
									pointTemplateTag.PointTemplateTagGuid = Guid.NewGuid();
									pointTemplateTag.ID = tag.TagName;
									pointTemplateTag.ValueTypeString = valueTypeString;
									pointTemplateTag.Value = moduleTagDefaults.Value;
									pointTemplateTag.EngineeringUnitsType = tag.EngineeringUnitsType;
									pointTemplateTag.Units = (moduleTagDefaults.ApplyPointTemplateEngineeringUnits) ? unit : moduleTagDefaults.Units;
									pointTemplateTag.DecimalPlaces = (moduleTagDefaults.ApplyPointTemplateDecimalPlaces) ? decimalPlaces : moduleTagDefaults.DecimalPlaces;
									pointTemplateTag.ServerUnits = (moduleTagDefaults.ApplyPointTemplateEngineeringUnits) ? unit : moduleTagDefaults.ServerUnits;
									pointTemplateTag.Maximum = (moduleTagDefaults.ApplyPointTemplateMaximum) ? maximum : moduleTagDefaults.Maximum;
									pointTemplateTag.Minimum = (moduleTagDefaults.ApplyPointTemplateMinimum) ? minimum : moduleTagDefaults.Minimum;
									pointTemplateTag.InputOutputType = inputOutputType;
									pointTemplateTag.AlarmStatus = false;
									pointTemplateTag.AlarmsEnabled = false;
									pointTemplateTag.Input = input;
									pointTemplateTag.SiteGuid = site.SiteGuid;
									pointTemplateTag.WellKnownIdentityGuid = moduleTagDefaults.WellKnownIdentityGuid;
									pointTemplateTag.ApplyPointTemplateEngineeringUnits = moduleTagDefaults.ApplyPointTemplateEngineeringUnits;
									pointTemplateTag.ApplyPointTemplateDecimalPlaces = moduleTagDefaults.ApplyPointTemplateDecimalPlaces;
									pointTemplateTag.ApplyPointTemplateMaximum = moduleTagDefaults.ApplyPointTemplateMaximum;
									pointTemplateTag.ApplyPointTemplateMinimum = moduleTagDefaults.ApplyPointTemplateMinimum;
									pointTemplateTag.InhibitInputOutputTypeConfiguration = moduleTagDefaults.InhibitInputOutputTypeConfiguration;
									pointTemplateTag.InhibitOverride = moduleTagDefaults.InhibitOverride;
									pointTemplateTag.Archived = true;
									pointTemplateTag.Module = true;

									newTagList.Add(pointTemplateTag);
									tag.TagGuid = pointTemplateTag.PointTemplateTagGuid;
								}
							}
						}

						// add settings
						var pointTemplatePropertyList = new List<PointTemplateProperty>();
						foreach (var setting in settings)
						{
							var propertyNewName = setting.newSettingName;
							if (module.ID != InstanceName)
							{
								propertyNewName = InstanceName + " " + propertyNewName;
							}
							// if there is another setting/property with the same name then reuse the Guid and don't create a new one
							var existingModuleInstance = pointTemplate.Properties.Select(x => x.Value).Where(x => x.ID == propertyNewName).ToList();
							if (existingModuleInstance.Count() > 0)
							{

								setting.newSettingGuid = existingModuleInstance.FirstOrDefault().PointTemplatePropertyGuid;
							}
							else
							{


								// create a new point Template Property
								// Add the module reference to the point template 
								var pointTemplateProperty = new PointTemplateProperty();
								pointTemplateProperty.PointTemplateGuid = pointTemplateGuid;
								pointTemplateProperty.PointTemplatePropertyGuid = Guid.NewGuid();
								pointTemplateProperty.SiteGuid = site.SiteGuid;
								pointTemplateProperty.ID = propertyNewName;
								Type newType = null;
								if (setting.SettingType.StartsWith("System."))
								{
									newType = Type.GetType(setting.SettingType);
								}
								else if(setting.SettingType.StartsWith("FMBusinessObjects."))
								{
									newType = Type.GetType(setting.SettingType + ",FMBusinessObjects", true);
								}

								if (setting.SettingType.StartsWith("System.String"))
								{
									pointTemplateProperty.Value = string.Empty;
								}
								else
								{
									pointTemplateProperty.Value = (Activator.CreateInstance(newType));
								}

								if(pointTemplateProperty.Value is DateTime)
								{
									pointTemplateProperty.Value = DateTime.Today;
								}
								else if (pointTemplateProperty.Value is DateTimeOffset)
								{
									pointTemplateProperty.Value = DateTimeOffset.Now;
								}

								pointTemplatePropertyList.Add(pointTemplateProperty);

								setting.newSettingGuid = pointTemplateProperty.PointTemplatePropertyGuid;
							}
						}

						// add the module to the template
						var newModule = new ModuleToPointTemplateMap();
						var newModuleList = new List<ModuleToPointTemplateMap>();

						newModule.ModuleGuid = newModuleGuid;
						newModule.ModuleToPointTemplateGuid = Guid.NewGuid();
						newModule.Order = pointTemplate.ModuleInstances.Count() + 1;
						newModule.PointTemplateGuid = pointTemplateGuid;
						newModule.SiteGuid = site.SiteGuid;
						newModule.ID = InstanceName;
						newModule.ModuleToPointTemplateData = new ModuleToPointTemplateData();

						var newTagModuleRefs = new List<TagToModule>();
						foreach (var tag in Tags)
						{
							var newTagModuleRef = new TagToModule();
							newTagModuleRef.ModuleParameter = tag.ParameterName;
							newTagModuleRef.TagGuid = tag.TagGuid.GetValueOrDefault();
							newTagModuleRefs.Add(newTagModuleRef);
						}

						newModule.ModuleToPointTemplateData.TagToModules = newTagModuleRefs.ToArray();

						var newSettingsModuleRefs = new List<PropertyToModule>();
						foreach (var setting in settings)
						{
							var newPropertyModuleRef = new PropertyToModule();
							var moduleSettingDefaults = module.ModuleData.ModuleSettings.FirstOrDefault(x => x.SettingName == setting.newSettingName);
							if (moduleSettingDefaults != null)
							{
								newPropertyModuleRef.PropertyName = moduleSettingDefaults.PropertyName;
								newPropertyModuleRef.PropertyGuid = setting.newSettingGuid.GetValueOrDefault();
								newSettingsModuleRefs.Add(newPropertyModuleRef);
							}
						}
						newModule.ModuleToPointTemplateData.PropertyToModules = newSettingsModuleRefs.ToArray();

						newModuleList.Add(newModule);

						pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.AddModule(this.Security, pointTemplateGuid, newTagList, pointTemplatePropertyList, newModuleList));


						

						if(this.ReorderModuleInstances(pointTemplate))
						{
							foreach (var moduleInstance in pointTemplate.ModuleInstances.Values)
							{
								FMChannelHelper.MakeCall<IModulePointTemplateMaps>(x => x.Modify(this.Security, moduleInstance));
							}
							this.AddSuccess(this.GetTranslatedText("PointTemplateEditor|Module Instance Added Successfully, module instances reordered"));
						}
						else
						{
							this.AddSuccess(this.GetTranslatedText("PointTemplateEditor|Module Instance Added Successfully"));

						}
					}
					catch (Exception except)
					{
						this.OnError(except);
						return this.JsonWithErrorMessages(null);

					}

					var wellKnownTags = this.EnumerateWellKnownTags();
					var tagDataTypes = PointCustomTemplateDetailController.EnumerateTagDataTypes(this.Security, base.UseDataDictionary);
					var tagInputOutputTypes = PointCustomTemplateDetailController.EnumerateTagInputOutputTypes();
					var outputTagChangeAgents = PointCustomTemplateDetailController.EnumerateOutputTagChangeAgents();
					var pointCommandStatusListDictionary = GetPointCommaandStatusListDictionary(pointTemplate.Properties, pointTemplate.PointCommandStatus);

					var model = new PointEditDetailModel(
						pointTemplate,
						site,
						new ApplicationStringCollectionClass(),
						new List<string>(),
						new List<DrawingName>(),
						wellKnownTags,
						tagDataTypes,
						tagInputOutputTypes,
						outputTagChangeAgents,
						pointCommandStatusListDictionary);
					this.SetFCEERights(model);

					tagView = this.RenderRazorViewToString("PointCustomTemplateDetailTags", model);
					modulesView = this.RenderRazorViewToString("PointCustomTemplateDetailModules", model);

					foreach (var property in model.PointTemplate.Properties)
					{
						var propertyModel = new PropertyModel()
													 {
															Site = model.Site,
															IsTemplatePoint = model.IsTemplatePoint,
															IsStandard = model.PointTemplate.Standard,
															DerivedPointCount = model.PointTemplate.DerivedPointCount,
															PointGuid = property.Value.PointTemplateGuid,
															PropertyBase = property.Value,
															PointCommandStatusListDictionary = pointCommandStatusListDictionary,
															ModifyEnabled = model.ModifyEnabled
													 };
						settingsView += this.RenderRazorViewToString("~/Areas/InventoryManagement/Views/Points/PointSettings.cshtml", propertyModel);
					}


					results.Add(new KeyValuePair<string, string>("tagView", tagView));
					results.Add(new KeyValuePair<string, string>("settingsView", settingsView));
					results.Add(new KeyValuePair<string, string>("modulesView", modulesView));
					results.Add(
						new KeyValuePair<string, string>(
							"tagList",
							System.Web.Helpers.Json.Encode(
								model.Tags.Select(
									(x, index) =>
										new
										{
											index,
											x.Name,
											x.PointTagGuid,
											Units = (int)x.Unit,
											ServerUnits = (int)x.ServerUnit,
											EngineeringUnitsType = x.EngineeringUnitsType.ToString(),
											x.ApplyPointEngineeringUnits,
											x.ApplyPointDecimalPlaces,
											x.ApplyPointMinimum,
											x.ApplyPointMaximum,
											x.DecimalPlaces,
											Minimum = x.MinimumRaw,
											Maximum = x.MaximumRaw,
											InputOutputType = (int)x.InputOutputType,
											x.DataType
										}).ToList())));

				}
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);

			}

			return this.JsonWithErrorMessages(results, JsonRequestBehavior.AllowGet);

		}

		[HttpPost]
		[ValidateJsonAntiForgeryToken]
		public ActionResult PointsCustomTemplateDeleteModule(Guid pointTemplateGuid, Guid moduleInstanceGuid)
		{
			var tagsView = "";
			var settingsView = "";
			var modulesView = "";
			var alarmsView = "";
			var results = new List<KeyValuePair<string, string>>();
			try
			{
				var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
				ModuleToPointTemplateMap moduleInstance = pointTemplate.ModuleInstances.Values.FirstOrDefault(x => x.ModuleToPointTemplateGuid == moduleInstanceGuid);


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

				if (moduleInstance != null)
				{
					// Remove any Tags added by Modules and no longer referenced by Module Instances, Alarm State, Alarm Limit
					var tagDictionary = new Dictionary<Guid, PointTemplateTag>();
					foreach (var tag in pointTemplate.Tags.Values)
					{
						if (pointTemplate.ModuleInstances.Values
							.Where(x => x.ModuleToPointTemplateGuid != moduleInstanceGuid)
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
							.Where(x => x.ModuleToPointTemplateGuid != moduleInstanceGuid)
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



					// delete the settings in the module instance if not in use
					foreach (var setting in moduleInstance.ModuleToPointTemplateData.PropertyToModules)
					{
						// check if the property is not being used anywhere else so we can delete it
						if (!pointTemplate.ModuleInstances.Values.Where(x => x.ModuleToPointTemplateGuid != moduleInstanceGuid)
								.SelectMany(x => x.ModuleToPointTemplateData.PropertyToModules)
								.Any(y => y.PropertyGuid == setting.PropertyGuid))
						{
							pointTemplate.Properties.Remove(setting.PropertyGuid);
						}
					}

					var tagGuidList = pointTemplate.Tags.Keys.ToList();

					var tagsWithAlarmsGuidList = pointTemplate.Tags.Values.Where(x => x.AlarmTemplates.Any()).Select(x => x.PointTemplateTagGuid).ToList();

					var propertyGuidList = pointTemplate.Properties.Keys.ToList();

					pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.DeleteModule(this.Security, pointTemplate.PointTemplateGuid, tagGuidList, tagsWithAlarmsGuidList, propertyGuidList, moduleInstanceGuid));
				}

				this.AddSuccess("Module Instance Successfully Removed.");

				var wellKnownTags = this.EnumerateWellKnownTags();
				var tagDataTypes = PointCustomTemplateDetailController.EnumerateTagDataTypes(this.Security, base.UseDataDictionary);
				var tagInputOutputTypes = PointCustomTemplateDetailController.EnumerateTagInputOutputTypes();
				var outputTagChangeAgents = PointCustomTemplateDetailController.EnumerateOutputTagChangeAgents();
				var pointCommandStatusListDictionary = GetPointCommaandStatusListDictionary(pointTemplate.Properties, pointTemplate.PointCommandStatus);

				var model = new PointEditDetailModel(
					pointTemplate,
					site,
					new ApplicationStringCollectionClass(),
					new List<string>(),
					new List<DrawingName>(),
					wellKnownTags,
					tagDataTypes,
					tagInputOutputTypes,
					outputTagChangeAgents,
					pointCommandStatusListDictionary);
				this.SetFCEERights(model);

				model.Alarms = this.GetTemplateAlarmModel(this.Security, pointTemplate, site);


				tagsView = this.RenderRazorViewToString("PointCustomTemplateDetailTags", model);
				modulesView = this.RenderRazorViewToString("PointCustomTemplateDetailModules", model);
				alarmsView = this.RenderRazorViewToString("PointCustomTemplateDetailAlarms", model.Alarms);

				foreach (var property in model.PointTemplate.Properties)
				{
					var propertyModel = new PropertyModel()
					{
						Site = model.Site,
						IsTemplatePoint = model.IsTemplatePoint,
						IsStandard = model.PointTemplate.Standard,
						DerivedPointCount = model.PointTemplate.DerivedPointCount,
						PointGuid = property.Value.PointTemplateGuid,
						PropertyBase = property.Value,
						PointCommandStatusListDictionary = pointCommandStatusListDictionary
					};
					settingsView += this.RenderRazorViewToString("~/Areas/InventoryManagement/Views/Points/PointSettings.cshtml", propertyModel);
				}



				results.Add(new KeyValuePair<string, string>("tagView", tagsView));
				results.Add(new KeyValuePair<string, string>("settingsView", settingsView));
				results.Add(new KeyValuePair<string, string>("modulesView", modulesView));
				results.Add(new KeyValuePair<string, string>("alarmsView", alarmsView));
				results.Add(
					new KeyValuePair<string, string>(
						"tagList",
						System.Web.Helpers.Json.Encode(
							model.Tags.Select(
								(x, index) =>
									new
									{
										index,
										x.Name,
										x.PointTagGuid,
										Units = (int)x.Unit,
										ServerUnits = (int)x.ServerUnit,
										EngineeringUnitsType = x.EngineeringUnitsType.ToString(),
										x.ApplyPointEngineeringUnits,
										x.ApplyPointDecimalPlaces,
										x.ApplyPointMinimum,
										x.ApplyPointMaximum,
										x.DecimalPlaces,
										Minimum = x.MinimumRaw,
										Maximum = x.MaximumRaw,
										InputOutputType = (int)x.InputOutputType,
										x.DataType
									}).ToList())));
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);

			}

			return this.JsonWithErrorMessages(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetListOfImagesPartialView()
		{

			try
			{
				var model = new PictureSummaryModel
				{
					DeleteEnabled = true,
					Pictures =
										FMChannelHelper.MakeCall<IPictures, PictureCollection>(
											x => x.Enumerate(this.Security))
				};
				return PartialViewWithErrorMessages("PointCustomTemplateImageSelection", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetListOfModules(string identityGuid)
		{
			try
			{
				var moduleList = FMChannelHelper.MakeCall<IModules, Dictionary<Guid, Module>>(x => x.EnumerateForAddToPointTemplateGuid(this.Security, new Guid(identityGuid)));
				var returnList = moduleList.Select(x => new { x.Value.ID, x.Value.ModuleGuid, isStandard = !string.IsNullOrEmpty(x.Value.ModuleTypeName) }).ToList().OrderBy(x => x.ID);
				return this.JsonWithErrorMessages(returnList, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetModulesDetails(string moduleGuid)
		{
			try
			{
				var newModuleGuid = new Guid(moduleGuid);
				var module = FMChannelHelper.MakeCall<IModules, Module>(x => x.Get(this.Security, newModuleGuid));
				return this.JsonWithErrorMessages(module, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		protected List<KeyValuePair<Guid, string>> EnumerateWellKnownTags()
		{
			var wellKnownTags = PointTemplateTag.EnumerateWellKnownTags();

			List<KeyValuePair<Guid, string>> translatedWellKnownTags = new List<KeyValuePair<Guid, string>>(wellKnownTags.Count);

			var dictionaryCollection = new DataDictionaryCollectionClass();

			if (!base.UseDataDictionary)
			{
				foreach (var wellKnownTag in wellKnownTags)
				{
					translatedWellKnownTags.Add(new KeyValuePair<Guid, string>(wellKnownTag.Key, dictionaryCollection[wellKnownTag.Value]));
				}
			}

			else
			{

				var dictionary = new Dictionary<string, string>(wellKnownTags.Count);
				foreach (var wellKnownTag in wellKnownTags)
				{
					dictionary.Add(wellKnownTag.Value, string.Empty);
				}

				dictionary = FMChannelHelper.MakeCall<IDataDictionariesClass, Dictionary<string, string>>(x => x.TranslateKeyPairTable(this.Security.SiteGuid, dictionary));


				foreach (var wellKnownTag in wellKnownTags)
				{
					if (string.IsNullOrEmpty(dictionary[wellKnownTag.Value]))
					{
						translatedWellKnownTags.Add(new KeyValuePair<Guid, string>(wellKnownTag.Key, dictionaryCollection[wellKnownTag.Value]));
					}
					else
					{
						translatedWellKnownTags.Add(new KeyValuePair<Guid, string>(wellKnownTag.Key, dictionary[wellKnownTag.Value]));
					}
				}
			}

			return translatedWellKnownTags;
		}

		public static List<KeyValuePair<string, string>> EnumerateTagDataTypes(SecurityClass security, bool userDataDictionary)
		{
			var tagDataTypes = PointTemplateTag.EnumerateTagDataTypes();

			List<KeyValuePair<string, string>> translatedTagDataTypes = new List<KeyValuePair<string, string>>(tagDataTypes.Count);

			var dictionaryCollection = new DataDictionaryCollectionClass();

			if (!userDataDictionary)
			{
				foreach (var tagDataType in tagDataTypes)
				{
					translatedTagDataTypes.Add(new KeyValuePair<string, string>(tagDataType.Key, dictionaryCollection[tagDataType.Value]));
				}
			}

			else
			{

				var dictionary = new Dictionary<string, string>(tagDataTypes.Count);
				foreach (var tagDataType in tagDataTypes)
				{
					dictionary.Add(tagDataType.Value, string.Empty);
				}

				dictionary = FMChannelHelper.MakeCall<IDataDictionariesClass, Dictionary<string, string>>(x => x.TranslateKeyPairTable(security.SiteGuid, dictionary));


				foreach (var tagDataType in tagDataTypes)
				{
					if (string.IsNullOrEmpty(dictionary[tagDataType.Value]))
					{
						translatedTagDataTypes.Add(new KeyValuePair<string, string>(tagDataType.Key, dictionaryCollection[tagDataType.Value]));
					}
					else
					{
						translatedTagDataTypes.Add(new KeyValuePair<string, string>(tagDataType.Key, dictionary[tagDataType.Value]));
					}
				}
			}

			return translatedTagDataTypes;
		}

		public static List<SelectListItem> EnumerateTagInputOutputTypes()
		{
			var list = FMBaseController.GetEnumSelectList<PointTemplateTag.PointTagInputOutputType>();

			return (List<SelectListItem>)list;
		}

		public static List<SelectListItem> EnumerateOutputTagChangeAgents()
		{
			var list = FMBaseController.GetEnumSelectList<PointTemplateTag.OutputPointTagChangeAgent>();

			return (List<SelectListItem>)list;
		}

		public Dictionary<Guid, List<SelectListItem>> GetPointCommaandStatusListDictionary(Dictionary<Guid, PointTemplateProperty> properties, PointCommandStatus pointCommandStatus)
		{
			var pointCommandStatusListDictionary = new Dictionary<Guid, List<SelectListItem>>();
			foreach (var pointProperty in properties.Values)
			{
				var pointCommandStatusListReference = pointProperty.Value as FMBusinessObjects.DataObjects.PointCommandStatusListReference;
				if (pointCommandStatusListReference == null)
				{
					continue;
				}

				// Multiple Properties reference same list
				if (pointCommandStatusListDictionary.ContainsKey(pointCommandStatusListReference.PointCommandStatusListGuid))
				{
					continue;
				}


				if (pointCommandStatusListReference.PointCommandStatusListGuid != Guid.Empty)
				{
					var pointCommandStatusList = pointCommandStatus.CommandStatusLists.Where(x => x.CommandStatusListGuid == pointCommandStatusListReference.PointCommandStatusListGuid).First();
					if (pointCommandStatusList == null)
					{
						continue;
					}

					var commandStatusList = new List<SelectListItem>();
					commandStatusList.Add(new SelectListItem() { Text = "", Value = "" });
					foreach (var commandStatusElement in pointCommandStatusList.CommandStatusList)
					{
						commandStatusList.Add(new SelectListItem() { Text = commandStatusElement.Key, Value = commandStatusElement.Value.ToString() });
					}

					pointCommandStatusListDictionary.Add(pointCommandStatusListReference.PointCommandStatusListGuid, commandStatusList);
				}
				else
				{
					var commandStatusList = new List<SelectListItem>();
					commandStatusList.Add(new SelectListItem() { Text = "", Value = "" });
					pointCommandStatusListDictionary.Add(pointCommandStatusListReference.PointCommandStatusListGuid, commandStatusList);
				}
			}

			return pointCommandStatusListDictionary;
		}


		public static List<KeyValuePair<string, string>> EnumeratePropertyDataTypes(SecurityClass security, bool userDataDictionary)
		{
			var propertyDataTypes = PointTemplateProperty.EnumeratePropertyDataTypes();

			List<KeyValuePair<string, string>> translatedPropertyDataTypes = new List<KeyValuePair<string, string>>(propertyDataTypes.Count);

			var dictionaryCollection = new DataDictionaryCollectionClass();

			if (!userDataDictionary)
			{
				foreach (var propertyDataType in propertyDataTypes)
				{
					translatedPropertyDataTypes.Add(new KeyValuePair<string, string>(propertyDataType.Key, dictionaryCollection[propertyDataType.Value]));
				}
			}

			else
			{

				var dictionary = new Dictionary<string, string>(propertyDataTypes.Count);
				foreach (var propertyDataType in propertyDataTypes)
				{
					dictionary.Add(propertyDataType.Value, string.Empty);
				}

				dictionary = FMChannelHelper.MakeCall<IDataDictionariesClass, Dictionary<string, string>>(x => x.TranslateKeyPairTable(security.SiteGuid, dictionary));


				foreach (var propertyDataType in propertyDataTypes)
				{
					if (string.IsNullOrEmpty(dictionary[propertyDataType.Value]))
					{
						translatedPropertyDataTypes.Add(new KeyValuePair<string, string>(propertyDataType.Key, dictionaryCollection[propertyDataType.Value]));
					}
					else
					{
						translatedPropertyDataTypes.Add(new KeyValuePair<string, string>(propertyDataType.Key, dictionary[propertyDataType.Value]));
					}
				}
			}

			return translatedPropertyDataTypes;
		} 

		protected void SetValueForTemplateTag( PointTemplateTag pointTemplateTag, string value, SiteClass site)
		{
			if (!value.IsNullOrWhiteSpace())
			{
				var numberFormatInfo = new NumberFormatInfo()
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
					NumberDecimalDigits = pointTemplateTag.DecimalPlaces
				};

				if (pointTemplateTag.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") != -1)
				{
					Type type = Type.GetType(pointTemplateTag.ValueTypeString + ",FMBusinessObjects");
					pointTemplateTag.Value = Enum.ToObject(type, int.Parse(value));

				}
				else if (pointTemplateTag.ValueTypeString == "System.DateTime")
				{
					pointTemplateTag.Value = DateTime.SpecifyKind(DateTime.ParseExact(value, site.ShortDatePattern, site.GetDateTimeFormatInfo()), DateTimeKind.Local);
				}
				else if (pointTemplateTag.ValueTypeString == "System.DateTimeOffset")
				{
					if (!string.IsNullOrEmpty(value))
					{
						pointTemplateTag.Value = DateTimeOffset.ParseExact(value, site.ShortDatePattern + " " + site.TimePattern, site.GetDateTimeFormatInfo());
					}
					else
					{
						pointTemplateTag.Value = null;
					}
				}
				else if (pointTemplateTag.ValueTypeString == "System.TimeSpan")
				{
					pointTemplateTag.Value = TimeSpan.Parse(value);
				}
				else if (pointTemplateTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
				{
					PointCommandStatusListReference pclr = JsonConvert.DeserializeObject<PointCommandStatusListReference>(value);
					if (pclr != null)
					{
						pointTemplateTag.Value = pclr;
					}
				}
				else if (pointTemplateTag.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				{
					DeviceAlarmMapReference damr = JsonConvert.DeserializeObject<DeviceAlarmMapReference>(value);
					if (damr != null)
					{
						pointTemplateTag.Value = damr;
					}
				}



				else
				{
					pointTemplateTag.Value = PointManager.ParseValue(
						Type.GetType(pointTemplateTag.ValueTypeString),
						pointTemplateTag.Units,
						numberFormatInfo,
						value);
				}
			}
			else
			{
				pointTemplateTag.Value = null;
			}
			return;
		}
	}
}
