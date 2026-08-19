namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.ServiceModel;
	using System.Net;
	using System.Web.Mvc;
	using System.Web.Script.Serialization;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMPointCommon;
	using Areas.Controllers;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FuelsManager.FMWebApp;
	using ViewModels;
	using Newtonsoft.Json;
	using global::FMWebApp;
	using Microsoft.Ajax.Utilities;
	using Opc.Ua;

	[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
	public class PointsController : FMBaseControllerEx
	{
		//private string TranslatedNone = FMBaseController.TranslateText("None");//"<" + FMBaseController.TranslateText("None") + ">";

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PointsEdit(PointEditDetailModel model, string tagGrid, string assignedCategories, string productId, string settings, string unitConversionHistory, string alarmModel, string overrideDefaultDrawingGuidString, string deviceAlarmMap, string tagGuid)
		{	
			var defaultUnitConversionHistory = new List<PointDefaultUnitChangeHistory>();
			try
			{
				// Revalidate the model.  If the validation fails with the data annotations (we are checking for require fields there) it will
				// not call the IValidatableObject.Validate method so we may be missing error messages.
				// By forcing a call to the validation we may get duplicate error messages so we need to remove them in the client.
				this.TryValidateModel(model);

				var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, model.IdentityGuid));

				if (productId == FMBaseController.TranslateText("None"))
				{
					point.ProductID = string.Empty;
				}
				else
				{
					point.ProductID = productId;
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
				if(Guid.TryParse(overrideDefaultDrawingGuidString, out tempGuid) && tempGuid != Guid.Empty)
				{
					defaultDrawingGuid = tempGuid;
				}

				model.Tags = new List<PointTagEditGridModel>();
				model.Point = point;
				model.Site = site;

				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(tagGrid))
				{
					model.Tags = jss.Deserialize<List<PointTagEditGridModel>>(tagGrid);
				}

				// MVC does not support validation of a list of objects so it has to be done manually
				int i = 0;
				foreach (var tag in model.Tags)
				{

					if (!PointTemplateTag.IsNumeric(tag.DataType))
					{
						continue;
					}


					int index = model.Point.Tags.Keys.ToList().FindIndex(x => x == tag.PointTagGuid);

					var hasValues = true;
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

				/* modify the tags */
				if (this.ModelState.IsValid)
				{
					try
					{
						var tags = new Dictionary<Guid, PointTag>();

						var alarmLimitDictionary = new Dictionary<Guid, PointTag>();

						foreach(var tag in model.Point.Tags.Values)
						{
							if (tag.Alarms.Any())
							{
								foreach (var alarm in tag.Alarms.Values)
								{
									foreach (var alarmTest in alarm.AlarmTests.Values)
									{
										alarmLimitDictionary.Add(alarmTest.LimitTagGuid, model.Point.Tags[alarmTest.LimitTagGuid]);
									}
								}
							}
						}

						foreach (var tag in model.Tags)
						{
							PointTag pointTag = null;

							model.Point.Tags.TryGetValue(tag.PointTagGuid, out pointTag);

							if (pointTag != null)
							{
								pointTag.InputOutputType = tag.InputOutputType;
								if (pointTag.ValueType != typeof(FMBusinessObjects.DataObjects.PointCommandStatusListReference)
								&& pointTag.ValueType != typeof(FMBusinessObjects.DataObjects.DeviceAlarmMapReference))
								{
									if (!pointTag.IsForced())
									{
										if ((pointTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.UnAssigned
										|| pointTag.InputOutputType == PointTemplateTag.PointTagInputOutputType.OpcUa)
										&& (pointTag.Value != null
										|| pointTag.Status != StatusCodes.Bad))
										{
											pointTag.Value = null;
											pointTag.Status = StatusCodes.Bad;
											pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
											pointTag.SourceTimeStamp = DateTimeOffset.UtcNow;
										}
									}
								}


								if (pointTag.EngineeringUnitsType == tag.EngineeringUnitsType
								&& pointTag.Units != tag.Unit
								&& tag.Unit != EngineeringUnit.FmuNone
								&& pointTag.ValueType == typeof(System.Double)
								&& pointTag.Value is double)
								{
									if (!alarmLimitDictionary.ContainsKey(pointTag.PointTagGuid))
									{
										pointTag.Value = EngineeringUnits.Convert((double)pointTag.Value, pointTag.Units, tag.Unit, 60.0);
										pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
										pointTag.SourceTimeStamp = DateTimeOffset.UtcNow;
									}
								}

								if (pointTag.EngineeringUnitsType == tag.EngineeringUnitsType)
								{
									pointTag.Units = tag.Unit;
									pointTag.DecimalPlaces = Convert.ToByte(tag.DecimalPlaces);
									pointTag.ServerUnits = tag.ServerUnit;
									pointTag.Maximum = Double.Parse(tag.Maximum);
									pointTag.Minimum = Double.Parse(tag.Minimum);
								}
								tags.Add(pointTag.PointTagGuid, pointTag);
							}
							else
							{
								var newPointTag = new PointTag();
								newPointTag.PointGuid = model.IdentityGuid;
								newPointTag.PointTagGuid = tag.PointTagGuid;
								newPointTag.ID = tag.Name;
								newPointTag.ValueTypeString = tag.DataType;
								newPointTag.EngineeringUnitsType = tag.EngineeringUnitsType;
								newPointTag.Units = tag.Unit;
								newPointTag.DecimalPlaces = Convert.ToByte(tag.DecimalPlaces);
								newPointTag.ServerUnits = tag.ServerUnit;
								newPointTag.Maximum = Double.Parse(tag.Maximum);
								newPointTag.Minimum = Double.Parse(tag.Minimum);
								newPointTag.InputOutputType = tag.InputOutputType;
								newPointTag.Input = tag.Input;
								newPointTag.InhibitInputOutputTypeConfiguration = tag.InhibitInputOutputTypeConfiguration;
								newPointTag.InhibitOverride = tag.InhibitOverride;
								newPointTag.Archived = tag.Archived;
								newPointTag.ApplyPointEngineeringUnits = tag.ApplyPointEngineeringUnits;
								newPointTag.ApplyPointDecimalPlaces = tag.ApplyPointDecimalPlaces;
								newPointTag.ApplyPointMaximum = tag.ApplyPointMaximum;
								newPointTag.ApplyPointMinimum = tag.ApplyPointMinimum;
								newPointTag.WellKnownIdentityGuid = tag.WellKnownIdentityGuidString.IsNullOrWhiteSpace() ? Guid.Empty : new Guid(tag.WellKnownIdentityGuidString);
								if (tag.Value != null)
								{
									newPointTag.Value = tag.Value;
								}
								tags.Add(newPointTag.PointTagGuid, newPointTag);
							}
						}

						model.Point.Tags = tags;
					}
					catch (Exception except)
					{
							this.OnError(except);
							return this.JsonWithErrorMessages(null);
					}
				}

				// Set the DeviceAlarmMapReference
				if(!string.IsNullOrEmpty(deviceAlarmMap)
				&& !string.IsNullOrEmpty(tagGuid))
				{
					var pointTagGuid = jss.Deserialize<string>(tagGuid);
					if (!string.IsNullOrEmpty(pointTagGuid))
					{
						PointTag pointTag = null;
						if (model.Point.Tags.TryGetValue(new Guid(pointTagGuid), out pointTag)
						&& pointTag.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
						{
							DeviceAlarmMapReference damr = JsonConvert.DeserializeObject<DeviceAlarmMapReference>(jss.Deserialize<string>(deviceAlarmMap));
							if (damr != null)
							{
								pointTag.Value = damr;
							}
						}
					}
				}

				// modify the point
				if (this.ModelState.IsValid)
				{
					if (model.IdentityGuid == Guid.Empty)
					{
							throw new Exception("GUID cannot be blank.");
					}

					model.Point.ID = model.Name;
					model.Point.Enabled = model.Enabled;
					model.Point.Description = model.Description;
					if (System.String.IsNullOrEmpty(model.Notes))
					{
							model.Point.Notes = "";
					}
					else
					{
							model.Point.Notes = model.Notes;
					}

					model.Point.LevelUnit = model.LevelUnit;
					model.Point.LevelDecimalPlaces = (byte)model.LevelDecimalPlaces;
					model.Point.LevelMinimum = model.LevelMinimumRaw ?? 0;
					model.Point.LevelMaximum = model.LevelMaximumRaw ?? 0;

					model.Point.TemperatureUnit = model.TemperatureUnit;
					model.Point.TemperatureDecimalPlaces = (byte)model.TemperatureDecimalPlaces;
					model.Point.TemperatureMinimum = model.TemperatureMinimumRaw ?? 0;
					model.Point.TemperatureMaximum = model.TemperatureMaximumRaw ?? 0;

					model.Point.DensityUnit = model.DensityUnit;
					model.Point.DensityDecimalPlaces = (byte)model.DensityDecimalPlaces;
					model.Point.DensityMinimum = model.DensityMinimumRaw ?? 0;
					model.Point.DensityMaximum = model.DensityMaximumRaw ?? 0;

					model.Point.PressureUnit = model.PressureUnit;
					model.Point.PressureDecimalPlaces = (byte)model.PressureDecimalPlaces;
					model.Point.PressureMinimum = model.PressureMinimumRaw ?? 0;
					model.Point.PressureMaximum = model.PressureMaximumRaw ?? 0;

					model.Point.FlowUnit = model.FlowUnit;
					model.Point.FlowDecimalPlaces = (byte)model.FlowDecimalPlaces;
					model.Point.VolumetricFlowMinimum = model.VolumetricFlowMinimumRaw ?? 0;
					model.Point.VolumetricFlowMaximum = model.VolumetricFlowMaximumRaw ?? 0;

					model.Point.VolumeUnit = model.VolumeUnit;
					model.Point.VolumeDecimalPlaces = (byte)model.VolumeDecimalPlaces;
					model.Point.VolumeMinimum = model.VolumeMinimumRaw ?? 0;
					model.Point.VolumeMaximum = model.VolumeMaximumRaw ?? 0;

					model.Point.MassUnit = model.MassUnit;
					model.Point.MassDecimalPlaces = (byte)model.MassDecimalPlaces;
					model.Point.MassMinimum = model.MassMinimumRaw ?? 0;
					model.Point.MassMaximum = model.MassMaximumRaw ?? 0;

					model.Point.VelocityUnit = model.VelocityUnit;
					model.Point.VelocityDecimalPlaces = (byte)model.VelocityDecimalPlaces;
					model.Point.VelocityMinimum = model.VelocityMinimumRaw ?? 0;
					model.Point.VelocityMaximum = model.VelocityMaximumRaw ?? 0;

					model.Point.MassFlowUnit = model.MassFlowUnit;
					model.Point.MassFlowDecimalPlaces = (byte)model.MassFlowDecimalPlaces;
					model.Point.MassFlowMinimum = model.MassFlowMinimumRaw ?? 0;
					model.Point.MassFlowMaximum = model.MassFlowMaximumRaw ?? 0;

					model.Point.PointCategoryCollection.Clear();

					model.Point.OverrideDefaultDrawingGuid = defaultDrawingGuid;

					if (!string.IsNullOrEmpty(assignedCategories))
					{
						var localAssignedCategories = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(assignedCategories);

						foreach (var assignedCategory in localAssignedCategories)
						{
							var category = new ApplicationStringMapClass
							{
								ApplicationStringGuid = new Guid(assignedCategory.Key),
								ID = assignedCategory.Value,
								AssignedToGuid = model.Point.IdentityGuid,
								AssignedToID = model.Point.ID,
								Type = STRING_MAP_TYPE.POINT_CATEGORY
							};

							model.Point.PointCategoryCollection.Add(category);
						}
					}
				}

				if (this.ModelState.IsValid)
				{



					// modify the properties (they depend on the default values) 
					try
					{
						foreach (var property in point.Properties.Select(x => x.Value))
						{
							switch (property.ValueType.ToString())
							{
								case "FMBusinessObjects.DataObjects.StrapTable":
									StrapTableEditorController.ValidateStrapTable(this.ModelState, numberFormatInfo, point, property.Value as StrapTable, defaultUnitConversionHistory);
									break;
								case "FMBusinessObjects.DataObjects.Vessel":
									VesselSettingsEditorController.ValidateVesselProperty(this.ModelState, numberFormatInfo, point, property.Value as Vessel, defaultUnitConversionHistory);
									break;
								case "FMBusinessObjects.DataObjects.VcfModuleSettings":
									VcfSettingsEditorController.ValidateVCFModuleProperty(this.ModelState, numberFormatInfo, point, property.Value as VcfModuleSettings, defaultUnitConversionHistory);
									break;
								case "FMBusinessObjects.DataObjects.QuantityModuleSettings":
									PointsController.ValidateQuantityModuleProperty(this.ModelState, numberFormatInfo, point, model, property.PointPropertyGuid, property.Value as QuantityModuleSettings);
									break;
								case "FMBusinessObjects.DataObjects.TankTransferModuleSettings":
									PointsController.ValidateTankTransferModuleProperty(this.ModelState, numberFormatInfo, point, model, property.PointPropertyGuid, property.Value as TankTransferModuleSettings);
									break;
								case "FMBusinessObjects.DataObjects.VolumeTransferModuleSettings":
									PointsController.ValidateVolumeTransferModuleProperty(this.ModelState, numberFormatInfo, point, model, property.PointPropertyGuid, property.Value as VolumeTransferModuleSettings);
									break;
								case "FMBusinessObjects.DataObjects.TankCommandModuleSettings":
									PointsController.ValidateTankCommandProperty(this.ModelState, numberFormatInfo, point, model, property.PointPropertyGuid, property.Value as TankCommandModuleSettings);
									break;
								case "System.Boolean":
									property.Value = PointsController.ParsePointSettingBooleanValue(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), property.ID);
									break;
								case "System.DateTime":
									property.Value = PointsController.ParsePointSettingDateTimeValue(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.DateTimeOffset":
									property.Value = PointsController.ParsePointSettingDateTimeOffsetValue(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.TimeSpan":
									property.Value = PointsController.ParsePointSettingTimeSpanValue(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.String":
									property.Value = model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last();
									break;
								case "System.Double":
									property.Value = PointsController.ParsePointSettingDoubleValue(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.Single":
									property.Value = PointsController.ParsePointSettingSingleValue(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.Int64":
									property.Value = PointsController.ParsePointSettingInt64Value(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.Int32":
									property.Value = PointsController.ParsePointSettingInt32Value(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.Int16":
									property.Value = PointsController.ParsePointSettingInt16Value(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.UInt64":
									property.Value = PointsController.ParsePointSettingUInt64Value(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.UInt32":
									property.Value = PointsController.ParsePointSettingUInt32Value(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "System.UInt16":
									property.Value = PointsController.ParsePointSettingUInt16Value(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
									break;
								case "FMBusinessObjects.DataObjects.PointCommandStatusListReference":
									((PointCommandStatusListReference) property.Value).CurrentValue = PointsController.ParsePointSettingPointCommandStatusListReferenceValue(this.ModelState, property.ID, model.Properties.Where(x => x.PointPropertyGuid == property.PointPropertyGuid).Select(x => x.Value).Last(), site, property.ID);
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

				
				if (this.ModelState.IsValid)
				{
					//Save Alarm Data
					var errorlist = AlarmEditorController.UpdateAlarms(this.Security, site, point, alarmModel);
					if (errorlist != null && errorlist.Count > 0)
					{
						string errorMsg = string.Empty;
						foreach (string err in errorlist)
						{
							errorMsg += $"{err}{Environment.NewLine}";
						}
						this.OnError(errorMsg);
						return this.JsonWithErrorMessages(errorMsg, JsonRequestBehavior.AllowGet);

					}


					FMChannelHelper.MakeCall<IPoints>(x => x.Modify(this.Security, model.Point));

					this.AddSuccess("Save Successful");
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

			return this.JsonWithErrorMessages(null);
		}


		[HttpGet]
		public ActionResult PointsDetail(string id, string tagId, string moduleId)
		{
			var model = new PointEditDetailModel();
			try
			{
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
				model.Site = site;
				if (string.IsNullOrEmpty(id) || id.Equals("PointIndex", StringComparison.InvariantCultureIgnoreCase))
				{
				}
				else
				{
					var pointGuid = new Guid(id);
					var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, pointGuid));
					var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, point.PointTemplateGuid));
					var categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
					var productIdList = FMChannelHelper.MakeCall<IProducts, List<string>>(x => x.EnumerateIdBySite(this.Security));
					productIdList.Insert(0, "<" + FMBaseController.TranslateText("None") + ">");
					var associatedDrawings = FMChannelHelper.MakeCall<IDrawings, List<DrawingName>>(x => x.EnumerateAvailableDrawingNamesByPointTemplate(this.Security, point.PointTemplateGuid));
					DrawingName pointTemplateDefaultDrawing = null;
					foreach (var drawingName in associatedDrawings)
					{
						//Rename ID of drawing name to Point Template Dervied if we wish to reset back to the 
						//Point Template default drawing.  Normally the value of the Empty Guid drawing Name
						//is <None>.  This is because the Point Template Editor uses the same method 
						//EnumerateAvailableDrawingNamesByPointTemplate() which always inserts a drawing name of
						//<None> with a Drawing Guid of empty.  This is so the drop down list provide a very distinct
						//value to select when the user want to clear the default drawing for a specific point template.
						if (drawingName.DrawingGuid == Guid.Empty)
						{
							drawingName.ID = "<" + FMBaseController.TranslateText("Point Template Derived") + ">";
						}

						//If the Point template has a default drawing associated then we don't want that drawing to 
						//show up in our override selection as it would cause a circular reference issue.
						//If we want to revert back to the drawing that the Point Template is referencing then we need to
						//select the <Point Template Derived" option
						if( point.DefaultDrawingGuid != null && 
							point.DefaultDrawingGuid != Guid.Empty &&
							point.DefaultDrawingGuid == drawingName.DrawingGuid)
						{
							pointTemplateDefaultDrawing = drawingName;
						}
					}

					//If we found a drawing name that matches the one associated with te Point Template then remove that
					//from the list of available drawing to select from.
					if(pointTemplateDefaultDrawing != null)
					{
							associatedDrawings.Remove(pointTemplateDefaultDrawing);
					}
					// if we found a point initialize the model with the point
					var tagInputOutputTypes = PointCustomTemplateDetailController.EnumerateTagInputOutputTypes();
					var outputTagChangeAgents = PointCustomTemplateDetailController.EnumerateOutputTagChangeAgents();
					var pointCommandStatusListDictionary = GetPointCommaandStatusListDictionary(point.Properties, pointTemplate.PointCommandStatus);
					var fceeMappings = FMChannelHelper.MakeCall<IFCEEServiceManager, Dictionary<Guid, FCEEMapping>>(x => x.EnumerateByPointGuid(this.Security, point.PointGuid));
               model = new PointEditDetailModel(point, site, categories, productIdList, associatedDrawings, tagInputOutputTypes, outputTagChangeAgents, pointCommandStatusListDictionary, fceeMappings);
				}

				// Get filters to be used by Point Setting list rollout menu.
				this.GetPointSettingSelectionOptions(model);

				model.TagDataTypes = PointCustomTemplateDetailController.EnumerateTagDataTypes(this.Security, base.UseDataDictionary);
				model.ModifyEnabled = this.Security.HasRight(RIGHT.MODIFY_POINTS)
																	&& (this.Security.SiteGuid == model.SiteGuid
																					|| model.SiteGuid == Guid.Empty);

				model.HasEnablePointRight = this.Security.HasRight(RIGHT.ENABLE_POINTS);
				model.HasDisablePointRight = this.Security.HasRight(RIGHT.DISABLE_POINTS);
				model.HasModifyFCEERight = this.Security.HasRight(RIGHT.MODIFY_FCEE_DATA);
				model.HasFCEERight = model.HasModifyFCEERight || this.Security.HasRight(RIGHT.VIEW_FCEE_DATA);

				// if there was a tag specified as a parameter by default open the view for that tag or module
				if (!string.IsNullOrEmpty(tagId))
				{
					model.OpenFormForTag = tagId;
				}
				else
				{
					model.OpenFormForTag = string.Empty;
				}


				if (!string.IsNullOrEmpty(moduleId))
				{
					model.OpenFormForModule = moduleId;
				}
				else
				{
					model.OpenFormForModule = string.Empty;
				}

			}
			catch (Exception except)
			{
				this.OnError(except);
			}

			return this.View(model);
		}

		[HttpGet]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetModuleEditorPartialView(string moduleTemplateGuid, string pointGuid)
		{
			var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, new Guid(pointGuid)));
			var moduleInstance = point.ModuleInstances[new Guid(moduleTemplateGuid)];
			var moduleProperties = point.GetPropertiesForModuleInstance(moduleInstance);
			var moduleTags = point.GetTagsForModuleInstance(moduleInstance);
			//						var module = FMChannelHelper.MakeCall<IModulePointTemplateMaps, ModuleToPointTemplateMap>(x => x.Get(this.Security, new Guid(pointGuid), new Guid(moduleTemplateGuid)));
			
			return this.PartialViewWithErrorMessages(
				"PointModuleEditorPartial",
				new PointModuleEditorModel
				{
					ID = moduleInstance.ID,
					ModuleTemplateGuid = moduleTemplateGuid,
					Properties = moduleProperties,
					Tags = moduleTags,
					HasModifyModuleLibraryRight = this.Security.HasRight(RIGHT.MODIFY_MODULE_LIBRARY)
				}, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[ValidateJsonAntiForgeryToken]
		public ActionResult GetListOfPointsPartialView()
		{
			var model = new PointsFilterModel();

			try
			{
				// Populate the model
				model.Points =
					FMChannelHelper.MakeCall<IPoints, List<Point>>(
							x => x.EnumerateForSummaryWithCategories(this.Security, this.Security.SiteGuid, includeDictionaries: false));
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			return PartialViewWithErrorMessages("PointNavigationMenu", model, JsonRequestBehavior.AllowGet);
		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult ConvertTagValue(string oldUnit, string newUnit, string valueList)
		{

			var conversionList = new List<PointTagValueConversionModel>();

			var jss = new JavaScriptSerializer();

			if (!string.IsNullOrEmpty(valueList))
			{
				conversionList = jss.Deserialize<List<PointTagValueConversionModel>>(valueList);
			}

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			var numberFormatInfo = new NumberFormatInfo
			{
				NumberGroupSizes = site.GetNumberGroupSizes(),
				NumberGroupSeparator = site.NumberGroupSeparator,
				NumberDecimalSeparator = site.NumberDecimalSeparator
			};

			//validate parameters
			Boolean passValidation = true;
			EngineeringUnit sourceUnit = EngineeringUnit.FmuNone;
			EngineeringUnit targetUnit = EngineeringUnit.FmuNone;

			List<PointTagValueConvertedModel> results = new List<PointTagValueConvertedModel>();

			try
			{
				if (string.IsNullOrEmpty(oldUnit)) { passValidation = false; } else sourceUnit = (EngineeringUnit)int.Parse(oldUnit);

				if (string.IsNullOrEmpty(newUnit)) { passValidation = false; } else targetUnit = (EngineeringUnit)int.Parse(newUnit);

				if (passValidation)
				{

					// remove formatting
					foreach (var entry in conversionList)
					{
							var newEntry = new PointTagValueConvertedModel { id = entry.id };

							numberFormatInfo.NumberDecimalDigits = int.Parse(entry.numDecimals);

							var newDblValue = Double.Parse(entry.value);
							newDblValue = EngineeringUnits.Convert(newDblValue, sourceUnit, targetUnit, newDblValue);
							var formatted = PointManager.FormatValue(
								Type.GetType(entry.dataType),
								targetUnit,
								numberFormatInfo,
								newDblValue);

							newEntry.rawValue = newDblValue.ToString(CultureInfo.InvariantCulture);
							newEntry.formattedValue = formatted;
							newEntry.success = true;

							results.Add(newEntry);
					}
				}
				return this.JsonWithErrorMessages(results);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(results);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetRawTagValue(string unit, string valueList)
		{

			var conversionList = new List<PointTagValueConversionModel>();

			JavaScriptSerializer jss = new JavaScriptSerializer();
			if (!string.IsNullOrEmpty(valueList))
			{
				conversionList = jss.Deserialize<List<PointTagValueConversionModel>>(valueList);
			}
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			var numberFormatInfo = new NumberFormatInfo
			{
				NumberGroupSizes = site.GetNumberGroupSizes(),
				NumberGroupSeparator = site.NumberGroupSeparator,
				NumberDecimalSeparator = site.NumberDecimalSeparator
			};

			//validate parameters
			Boolean passValidation = true;
			EngineeringUnit sourceUnit = EngineeringUnit.FmuNone;

			List<PointTagValueConvertedModel> results = new List<PointTagValueConvertedModel>();
			try
			{
				if (unit == "null") unit = "255"; // if no unit defined (passed as null) set to NONE
				if (string.IsNullOrEmpty(unit)) { passValidation = false; } else sourceUnit = (EngineeringUnit)int.Parse(unit);

				if (passValidation)
				{

					// remove formatting
					foreach (var entry in conversionList)
					{
							var newEntry = new PointTagValueConvertedModel { id = entry.id };

							if (entry.value == "")
							{
								newEntry.rawValue = "";
							}
							else
							{
								numberFormatInfo.NumberDecimalDigits = int.Parse(entry.numDecimals);
								var newValue = PointManager.ParseValue(
									Type.GetType(entry.dataType),
									sourceUnit,
									numberFormatInfo,
									entry.value);

								var newDblValue = (double)newValue;

								newEntry.rawValue = newDblValue.ToString("G");

							}

							newEntry.success = true;

							results.Add(newEntry);
					}
				}
				return this.JsonWithErrorMessages(results);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(results);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult GetFormattedTagValue(string newUnit, string valueList)
		{
				var conversionList = new List<PointTagValueConversionModel>();

				var jss = new JavaScriptSerializer();

				if (!string.IsNullOrEmpty(valueList))
				{
					conversionList = jss.Deserialize<List<PointTagValueConversionModel>>(valueList);
				}

				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator
				};

				//validate parameters
				Boolean passValidation = true;
				EngineeringUnit targetUnit = EngineeringUnit.FmuNone;

				List<PointTagValueConvertedModel> results = new List<PointTagValueConvertedModel>();

				try
				{
					if (newUnit == "null") newUnit = "255"; // if no unit defined (passed as null) set to NONE
					if (string.IsNullOrEmpty(newUnit)) { passValidation = false; } else targetUnit = (EngineeringUnit)int.Parse(newUnit);

					if (passValidation)
					{

						// remove formatting
						foreach (var entry in conversionList)
						{
								var newEntry = new PointTagValueConvertedModel { id = entry.id };

								numberFormatInfo.NumberDecimalDigits = int.Parse(entry.numDecimals);

								var newDblValue = Double.Parse(entry.value);
								var formatted = PointManager.FormatValue(
									Type.GetType(entry.dataType),
									targetUnit,
									numberFormatInfo,
									newDblValue);

								newEntry.rawValue = newDblValue.ToString(CultureInfo.InvariantCulture);
								newEntry.formattedValue = formatted;
								newEntry.success = true;

								results.Add(newEntry);
						}
					}
					return this.JsonWithErrorMessages(results);
				}
				catch (Exception e)
				{
					this.OnError(e);
					return this.JsonWithErrorMessages(results);
				}
		}



		/// <summary>
		/// This method will get populate the model with the filter dorwpdown selections
		/// </summary>
		/// <returns>Returns a collection of Point Setting List Models</returns>
		private void GetPointSettingSelectionOptions(PointEditDetailModel detailModel)
		{
				var allCategoryList = new List<SelectListItem>();
				var pointTypeList = new List<SelectListItem>();

				try
				{
					if (detailModel != null)
					{
						ApplicationStringCollectionClass categories = detailModel.Categories;

						if (detailModel.ActionListCategories.Count == 0)
						{
								categories = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																						x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_CATEGORY));
						}

						var categorySelectItem = new SelectListItem { Value = "-99", Text = "Select Category" };
						allCategoryList.Add(categorySelectItem);

						foreach (var category in categories)
						{
								categorySelectItem = new SelectListItem { Value = category.ID, Text = category.ID };
								allCategoryList.Add(categorySelectItem);
						}

						detailModel.ActionListCategories = allCategoryList;

						if (detailModel.ActionListPointTypes.Count == 0)
						{
								var pointTypes = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
																				x => x.EnumerateByType(this.Security, STRING_TYPE.POINT_TEMPLATE_TYPE));

								var pointTypeSelectItem = new SelectListItem { Value = "-99", Text = "Select Point Type" };
								pointTypeList.Add(pointTypeSelectItem);

								foreach (var pointType in pointTypes)
								{
									pointTypeSelectItem = new SelectListItem { Value = pointType.ID, Text = pointType.ID };
									pointTypeList.Add(pointTypeSelectItem);
								}

								detailModel.ActionListPointTypes = pointTypeList;
						}
					}
				}
				catch (Exception except)
				{
					this.OnError(except);
				}

		}



		#region Parse Different types of Properties for validation and saving


		public static void ValidateQuantityModuleProperty(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, BasePoint pointBase, PointEditDetailModel model, Guid pointPropertyGuid, QuantityModuleSettings quantityValue)
		{
				// get all the settings in the UI for the Quantity setting
				var propertyList = model.Properties.Where(x => x.PointPropertyGuid == pointPropertyGuid);

				if (propertyList.Any(x => x.Name == "VolumeCalculationType"))
				{
					// CorrectionMethodSpecific is a enum TankMaterialEnum
					quantityValue.VolumeCalculationType = PointsController.ParsePointSettingEnumValue<VolumeCalculationType>(modelState,
																																								"VolumeCalculationType",
																																								propertyList.Where(x => x.Name == "VolumeCalculationType").Select(x => x.Value).Last(),
																																								"Volume Calculation Type");
				}

				if (propertyList.Any(x => x.Name == "MassOrWeightCalculationType"))
				{
					// CorrectionMethodSpecific is a enum TankMaterialEnum
					quantityValue.MassOrWeightCalculationType = PointsController.ParsePointSettingEnumValue<MassOrWeightCalculationType>(modelState,
																																												"MassOrWeightCalculationType",
																																												propertyList.Where(x => x.Name == "MassOrWeightCalculationType").Select(x => x.Value).Last(),
																																												"Mass/Weight Calculation Type");
				}

		}

		public static void ValidateTankTransferModuleProperty(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, BasePoint pointBase, PointEditDetailModel model, Guid pointPropertyGuid, TankTransferModuleSettings tankTransferValue)
		{
			// get all the settings in the UI for the Transfesr Module settings
			var propertyList = model.Properties.Where(x => x.PointPropertyGuid == pointPropertyGuid);

			if (propertyList.Any(x => x.Name == "TransferVolumeMode"))
			{
				// TransferVolumeMode is a enum TransferVolumeMode
				tankTransferValue.TransferVolumeMode = PointsController.ParsePointSettingEnumValue<TransferVolumeMode>(modelState,
																																						"TransferVolumeMode",
																																						propertyList.Where(x => x.Name == "TransferVolumeMode").Select(x => x.Value).Last(),
																																						"Volume Transfer Mode");
			}

			if (propertyList.Any(x => x.Name == "TransferAdvisoryTime"))
			{
				tankTransferValue.TransferAdvisoryTime = PointsController.ParsePointSettingDoubleValue(modelState, 
																																	"TransferAdvisoryTime",
																																	propertyList.Where(x => x.Name == "TransferAdvisoryTime").Select(x => x.Value).Last(),
																																	EngineeringUnitType.FmuNone,
																																	model,
																																	"Transfer Advisory Time");
				if (tankTransferValue.TransferAdvisoryTime < 0)
				{
					modelState.AddModelError(
							"TransferAdvisoryTime",
							TranslateText("PointEditor|Transfer Advisory Time cannot be negative."));
				}
			}
		}



		public static void ValidateVolumeTransferModuleProperty(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, BasePoint pointBase, PointEditDetailModel model, Guid pointPropertyGuid, VolumeTransferModuleSettings volumeTransferValue)
		{
			// get all the settings in the UI for the Transfesr Module settings
			var propertyList = model.Properties.Where(x => x.PointPropertyGuid == pointPropertyGuid);


			if (propertyList.Any(x => x.Name == "TransferVolumeMode"))
			{
				// TransferVolumeMode is a enum TransferVolumeMode
				volumeTransferValue.TransferVolumeMode = PointsController.ParsePointSettingEnumValue<TransferVolumeMode>(modelState,
																																						"TransferVolumeMode",
																																						propertyList.Where(x => x.Name == "TransferVolumeMode").Select(x => x.Value).Last(),
																																						"Volume Transfer Mode");
			}



			if (propertyList.Any(x => x.Name == "TransferAdvisoryTime"))
			{
				volumeTransferValue.TransferAdvisoryTime = PointsController.ParsePointSettingDoubleValue(modelState,
																																	"TransferAdvisoryTime",
																																	propertyList.Where(x => x.Name == "TransferAdvisoryTime").Select(x => x.Value).Last(),
																																	EngineeringUnitType.FmuNone,
																																	model,
																																	"Transfer Advisory Time");
				if (volumeTransferValue.TransferAdvisoryTime < 0)
				{
					modelState.AddModelError(
							"TransferAdvisoryTime",
							TranslateText("PointEditor|Transfer Advisory Time cannot be negative."));
				}
			}
		}


        public static void ValidateTankCommandProperty(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, BasePoint basePoint, PointEditDetailModel model, Guid pointPropertyGuid, TankCommandModuleSettings tankCommandValue )
		{
			// get all the settings in the UI for the Tank Command setting
			var propertyList = model.Properties.Where(x => x.PointPropertyGuid == pointPropertyGuid);
			if (propertyList.Any(x => x.Name == "MovementAlarmDifferential"))
			{
				double movementAlarmDifferential = PointsController.ParsePointSettingDoubleValue(modelState,
																											"MovementAlarmDifferential",
																											propertyList.Where(x => x.Name == "MovementAlarmDifferential").Select(x => x.Value).Last(),
																											EngineeringUnitType.FmuLength,
																											model,
																											"Movement Alarm Differential");

				tankCommandValue.MovementAlarmDifferential = new PointPropertyUnitTypedDouble(movementAlarmDifferential, EngineeringUnitType.FmuLength);
			}
		}



		#endregion


		#region Point Setting Parsing Values

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a double value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="unit"></param>
		/// <param name="model"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static double ParsePointSettingDoubleValue(ModelStateDictionary modelState, string property, string value, EngineeringUnitType unit, PointEditDetailModel model, string errorMessageHeader)
		{

			var numberFormatInfo = new NumberFormatInfo
			{
				NumberGroupSizes = model.Site.GetNumberGroupSizes(),
				NumberGroupSeparator = model.Site.NumberGroupSeparator,
				NumberDecimalSeparator = model.Site.NumberDecimalSeparator
			};

			double newDblValue = 0.0;
			if (unit == EngineeringUnitType.FmuNone)
			{
				// if we have no units specified then we can parse the string as a normal numeric value
				if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out newDblValue))
				{
					modelState.AddModelError(
								property,
								TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
				}
			}
			else {
				try
				{

					var newValue = Double.Parse(value, numberFormatInfo);
					newDblValue = (double)newValue;
				}
				catch (Exception)
				{
					modelState.AddModelError(property, TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
				}
			}

			return newDblValue;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a boolean value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static bool ParsePointSettingBooleanValue(ModelStateDictionary modelState, string property, string value, string errorMessageHeader)
		{
			bool flag;
			if (!Boolean.TryParse(
				value,
				out flag))
			{

				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return flag;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a enum value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static T ParsePointSettingEnumValue<T>(ModelStateDictionary modelState, string property, string value, string errorMessageHeader)
		{
			int newId;
			T newValue = default(T);
			//make sure the can be converted to an id
			if (Int32.TryParse(value, out newId))
			{
				// make sure that the int is defined in the enum
				if (Enum.IsDefined(typeof(T), newId))
				{
					newValue = (T)(object)newId;
				}
				else
				{
					modelState.AddModelError("property", string.Format("PointEditor|{0}: Invalid option.", errorMessageHeader));
				}
			}
			else
			{
				modelState.AddModelError("property", string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader));
			}
			return newValue;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static DateTime? ParsePointSettingDateTimeValue(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			DateTime? dateTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value))
				{
					dateTemp = DateTime.SpecifyKind(DateTime.ParseExact(value, site.ShortDatePattern, site.GetDateTimeFormatInfo()), DateTimeKind.Local);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return dateTemp;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static DateTimeOffset? ParsePointSettingDateTimeOffsetValue(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			DateTimeOffset? dateTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value))
				{
					dateTemp = DateTimeOffset.ParseExact(value, site.ShortDatePattern + " " + site.TimePattern, site.GetDateTimeFormatInfo());
					var dateSiteFormat = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
					dateTemp = TimeZoneInfo.ConvertTime(dateTemp.Value, dateSiteFormat);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return dateTemp;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static TimeSpan? ParsePointSettingTimeSpanValue(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			TimeSpan? timespanTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value))
				{
					timespanTemp = TimeSpan.Parse(value);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return timespanTemp;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static Double? ParsePointSettingDoubleValue(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			Double? doubleTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value)
				&& value != "NaN")
				{
					var numberFormatInfo = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);

					doubleTemp = Double.Parse(value, numberFormatInfo);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return doubleTemp;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static Single? ParsePointSettingSingleValue(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			Single? singleTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value)
				&& value != "NaN")
				{
					var numberFormatInfo = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);

					singleTemp = Single.Parse(value, numberFormatInfo);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return singleTemp;
		}


		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static Int64? ParsePointSettingInt64Value(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			Int64? intTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value)
				&& value != "NaN")
				{
					intTemp = Int64.Parse(value);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return intTemp;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static Int32? ParsePointSettingInt32Value(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			Int32? intTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value)
				&& value != "NaN")
				{
					intTemp = Int32.Parse(value);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return intTemp;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static Int16? ParsePointSettingInt16Value(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			Int16? intTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value)
				&& value != "NaN")
				{
					intTemp = Int16.Parse(value);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return intTemp;
		}



		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static UInt64? ParsePointSettingUInt64Value(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			UInt64? uintTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value)
				&& value != "NaN")
				{
					uintTemp = UInt64.Parse(value);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return uintTemp;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static UInt32? ParsePointSettingUInt32Value(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			UInt32? uintTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value)
				&& value != "NaN")
				{
					uintTemp = UInt32.Parse(value);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return uintTemp;
		}

		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static UInt16? ParsePointSettingUInt16Value(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			UInt16? uintTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value)
				&& value != "NaN")
				{
					uintTemp = UInt16.Parse(value);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return uintTemp;
		}


		/// <summary>
		/// Try to convert a setting value from string (received in AJAX call) to a Date value
		/// </summary>
		/// <paramName="modelState'></paramName>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="site"></param>
		/// <param name="errorMessageHeader"></param>
		/// <returns></returns>
		public static Int32? ParsePointSettingPointCommandStatusListReferenceValue(ModelStateDictionary modelState, string property, string value, SiteClass site, string errorMessageHeader)
		{
			Int32? intTemp = null;
			try
			{
				if (!string.IsNullOrEmpty(value))
				{
					intTemp = Int32.Parse(value);
				}
			}
			catch (Exception)
			{
				modelState.AddModelError(
					property,
					TranslateText(string.Format("PointEditor|{0}: Invalid format.", errorMessageHeader)));
			}
			return intTemp;
		}

		public Dictionary<Guid, List<SelectListItem>> GetPointCommaandStatusListDictionary(Dictionary<Guid, PointProperty> properties, PointCommandStatus pointCommandStatus)
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

		#endregion

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult PointValueConfigurationEditor(bool isTemplatePoint, bool isSetting, Guid pointObjectGuid, string pointObjectName)
		{
			var model = new PointValueConfigurationEditorModel();
			Guid pointTemplateGuid = Guid.Empty;
			Dictionary<Guid, string> pointCommandStatusList = new Dictionary<Guid, string>();
			object value = null;
			string valueTypeString = string.Empty;
			try
			{
				if (!isTemplatePoint)
				{
					if (isSetting)
					{
						var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointObjectGuid));
						var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointProperty.PointTemplatePropertyGuid));
						pointTemplateGuid = pointTemplateProperty.PointTemplateGuid;
						value = pointProperty.Value;
						valueTypeString = pointProperty.ValueTypeString;
					}
					else
					{
						var pointTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, pointObjectGuid));
						var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointTag.PointGuid));
						pointTemplateGuid = point.PointTemplateGuid;
						value = pointTag.Value;
						valueTypeString = pointTag.ValueTypeString;
					}
				}
				else
				{
					if (isSetting)
					{
						var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointObjectGuid));
						pointTemplateGuid = pointTemplateProperty.PointTemplateGuid;
						value = pointTemplateProperty.Value;
						valueTypeString = pointTemplateProperty.ValueTypeString;
					}
					else
					{
						var pointTemplateTag = FMChannelHelper.MakeCall<IPointTemplateTags, PointTemplateTag>(x => x.Get(this.Security, pointObjectGuid));
						pointTemplateGuid = pointTemplateTag.PointTemplateGuid;
						value = pointTemplateTag.Value;
						valueTypeString = pointTemplateTag.ValueTypeString;
					}
				}

				Dictionary<Guid, string> valueReferenceDictionary = null;
				Guid? valueGuid = null;
				Dictionary<Int64, string> valueEntryDictionary = new Dictionary<Int64, string>();
				object valueReferenceObject = null;

				if (valueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				{
					valueReferenceDictionary = FMChannelHelper.MakeCall<IPointTemplates, Dictionary<Guid, string>>(x => x.GetDeviceAlarmMapDictionary(this.Security, pointTemplateGuid));

					if (value != null && value is DeviceAlarmMapReference)
					{
						valueGuid = (value as DeviceAlarmMapReference).DeviceAlarmMapGuid;
					}
					if (valueGuid != null && valueGuid != Guid.Empty)
					{
						var deviceAlarmMap = FMChannelHelper.MakeCall<IPointTemplates, DeviceAlarmMap>(x => x.GetDeviceAlarmMap(this.Security, pointTemplateGuid, (Guid)valueGuid));
						if (deviceAlarmMap != null)
						{
							foreach (var deviceAlarmMapEntry in deviceAlarmMap.DeviceAlarmMapEntryList)
							{
								valueEntryDictionary.Add(deviceAlarmMapEntry.BitMask, "0x" + deviceAlarmMapEntry.BitMask.ToString("X8") + " - " + deviceAlarmMapEntry.TestName);
							}
						}
						valueReferenceObject = deviceAlarmMap;
					}
				}
				else if (valueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
				{
					valueReferenceDictionary = FMChannelHelper.MakeCall<IPointTemplates, Dictionary<Guid, string>>(x => x.GetPointCommandStatusDictionary(this.Security, pointTemplateGuid));

					if (value != null && value is PointCommandStatusListReference)
					{
						valueGuid = ((PointCommandStatusListReference)(value)).PointCommandStatusListGuid;
					}
					if (valueGuid != null && valueGuid != Guid.Empty)
					{
						var pointCommandstatusList = FMChannelHelper.MakeCall<IPointTemplates, PointCommandStatusList>(x => x.GetPointCommandStatusList(this.Security, pointTemplateGuid, (Guid)valueGuid));
						foreach (var commandStatusElement in pointCommandstatusList.CommandStatusList)
						{
							valueEntryDictionary.Add(Convert.ToUInt32(commandStatusElement.Value), commandStatusElement.Value + " - " + commandStatusElement.Key);
						}
						valueReferenceObject = pointCommandstatusList;
					}
				}

				model.AlarmPriorityDictionary = new Dictionary<Guid, AlarmPriorityClass>();
				model.NormalPriorityDictionary = new Dictionary<Guid, AlarmPriorityClass>();

				var allAlarmPriorityDictionary = DeviceAlarmMapsEditorController.GetAllAlarmPriorities(this.Security);
				foreach (var alarmPriority in allAlarmPriorityDictionary.Values)
				{
					if (alarmPriority.Priority.HasValue)
					{
						model.AlarmPriorityDictionary.Add(alarmPriority.IdentityGuid, alarmPriority);
					}
					else
					{
						model.NormalPriorityDictionary.Add(alarmPriority.IdentityGuid, alarmPriority);
					}
				}


				model.AlarmCategoryDictionary = DeviceAlarmMapsEditorController.GetAllAlarmCategories(this.Security);

				model.IsTemplatePoint = isTemplatePoint;
				model.IsSetting = isSetting;
				model.PointObjectGuid = pointObjectGuid;
				model.PointObjectName = pointObjectName;
				model.PointTemplateGuid = pointTemplateGuid;
				model.ValueReferenceDictionary = valueReferenceDictionary;
				model.ValueGuid = valueGuid;
				model.ValueEntryDictionary = valueEntryDictionary;
				model.ValueTypeString = valueTypeString;
				model.ValueReferenceObject = valueReferenceObject;
			}
			catch (Exception except)
			{
				FMFormBase.LogErrorMessage(except.Message + (except.InnerException != null ? except.InnerException.Message : ""));
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return PartialViewWithErrorMessages("PointValueConfigurationEditor", model, JsonRequestBehavior.AllowGet);
		}


		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult GetValueReferenceEntryList(Guid pointTemplateGuid, Guid valueReferenceGuid, string valueTypeString)
		{
			try
			{
				if (valueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				{
					List<DeviceAlarmMap.DeviceAlarmMapEntry> deviceAlarmMapEntryList = new List<DeviceAlarmMap.DeviceAlarmMapEntry>();
					var valueReference = FMChannelHelper.MakeCall<IPointTemplates, DeviceAlarmMap>(x => x.GetDeviceAlarmMap(this.Security, pointTemplateGuid, valueReferenceGuid));
					return Json(valueReference, JsonRequestBehavior.AllowGet);

				}
				else if (valueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
				{
					List<PointCommandStatusList.CommandStatusElement> commandStatusElements = new List<PointCommandStatusList.CommandStatusElement>();
					var pointCommaandStatusList = FMChannelHelper.MakeCall<IPointTemplates, PointCommandStatusList>(x => x.GetPointCommandStatusList(this.Security, pointTemplateGuid, valueReferenceGuid));
					var commandStatusElementList = from c in pointCommaandStatusList.CommandStatusList select new KeyValuePair<string, int> ( c.Key, c.Value );
					var valueReference = commandStatusElementList.ToList();
					return Json(valueReference, JsonRequestBehavior.AllowGet);
				}
			}
			catch (Exception except)
			{
				FMFormBase.LogErrorMessage(except.Message + (except.InnerException != null ? except.InnerException.Message : ""));
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			return Json(null, JsonRequestBehavior.AllowGet);
		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SaveValueConfigurationChanges(bool isTemplatePoint, bool isSetting, Guid pointObjectGuid, Guid? valueReferenceGuid)
		{
			try
			{
				if (!isTemplatePoint)
				{
					if (isSetting)
					{
						var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointObjectGuid));

						if (pointProperty != null && pointProperty.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
						{
							PointCommandStatusListReference pclr = new PointCommandStatusListReference();
							if(valueReferenceGuid != null)
							{ 
								pclr.PointCommandStatusListGuid = (Guid)valueReferenceGuid;
								if (pointProperty.Value is PointCommandStatusListReference
								    && (pointProperty.Value as PointCommandStatusListReference).PointCommandStatusListGuid == (Guid)valueReferenceGuid)
								{
									pclr.CurrentValue = (pointProperty.Value as PointCommandStatusListReference).CurrentValue;
								}

							}
							pointProperty.Value = pclr;
						}

						FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.Security, pointProperty, bypassUpdatePointRecordVersion : false, bypassIsPointInSystemUse : true));
					}
					else
					{
						var pointTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, pointObjectGuid));

						if (pointTag != null && pointTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
						{
							PointCommandStatusListReference pclr = new PointCommandStatusListReference();
							if (valueReferenceGuid != null)
							{
								pclr.PointCommandStatusListGuid = (Guid)valueReferenceGuid;

								if(pointTag.Value is PointCommandStatusListReference
								&& (pointTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid == (Guid)valueReferenceGuid)
								{
									pclr.CurrentValue = (pointTag.Value as PointCommandStatusListReference).CurrentValue;
								}
								else
								{
									pointTag.Status = StatusCodes.Bad;
								}
							}
							pointTag.Value = pclr;
                     pointTag.ServerTimeStamp = DateTimeOffset.UtcNow;
                     FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointTagData(this.Security, new List<PointTag>() { pointTag }, false));
						}
					}
				}
				else
				{
					if (isSetting)
					{
						var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointObjectGuid));

						if (pointTemplateProperty != null && pointTemplateProperty.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
						{
							PointCommandStatusListReference pclr = new PointCommandStatusListReference();
							if (valueReferenceGuid != null)
							{
								pclr.PointCommandStatusListGuid = (Guid)valueReferenceGuid;
								if (pointTemplateProperty.Value is PointCommandStatusListReference
								&& (pointTemplateProperty.Value as PointCommandStatusListReference).PointCommandStatusListGuid == (Guid)valueReferenceGuid)
								{
									pclr.CurrentValue = (pointTemplateProperty.Value as PointCommandStatusListReference).CurrentValue;
								}
							}
							pointTemplateProperty.Value = pclr;
						}

						FMChannelHelper.MakeCall<IPointTemplateProperties>(x => x.ModifyPointTemplatePropertyValue(this.Security, pointTemplateProperty));
					}
					else
					{
						var pointTemplateTag = FMChannelHelper.MakeCall<IPointTemplateTags, PointTemplateTag>(x => x.Get(this.Security, pointObjectGuid));

						if (pointTemplateTag != null && pointTemplateTag.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
						{
							PointCommandStatusListReference pclr = new PointCommandStatusListReference();
							if (valueReferenceGuid != null)
							{
								pclr.PointCommandStatusListGuid = (Guid)valueReferenceGuid;
								if (pointTemplateTag.Value is PointCommandStatusListReference
								&& (pointTemplateTag.Value as PointCommandStatusListReference).PointCommandStatusListGuid == (Guid)valueReferenceGuid)
								{
									pclr.CurrentValue = (pointTemplateTag.Value as PointCommandStatusListReference).CurrentValue;
								}
							}
							pointTemplateTag.Value = pclr;
						}

						FMChannelHelper.MakeCall<IPointTemplateTags>(x => x.ModifyTag(this.Security, pointTemplateTag, false));
					}
				}
			}
			catch (CommunicationException e)
			{
				this.OnError(new Exception(this.GetTranslatedText(e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			if (this.ModelState.IsValid)
			{
				this.AddSuccess("Save Successful");
			}
			return this.JsonWithErrorMessages(null);
		}
	}
}
