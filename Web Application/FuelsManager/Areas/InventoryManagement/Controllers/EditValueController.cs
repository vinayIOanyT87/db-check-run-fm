namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.Mvc;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMPointCommon;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using Microsoft.Ajax.Utilities;
	using Newtonsoft.Json;
	using Opc.Ua;
	using Softing.Opc.Ua.Client;
	using Softing.Opc.Ua.Configuration;
	using System.Security.Cryptography.X509Certificates;
	using System.Xml;
	using System.Xml.Serialization;
	using System.IO;
    using System.Linq;
    using FMBusinessObjects.Constants;
    using System.Security.Policy;

    public class EditValueController : FMBaseControllerEx
	{

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult EditValue(PointValueIdentifier pointValueIdentifier)
		{
			var model = new EditValueModel();

			try
			{
				model.Site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				model.TimeZone = model.Site.TimeZone;
				TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(model.Site.TimeZone);
				double timezoneOffset = sitesTimezone.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
				DateTime currentSiteTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, sitesTimezone);
				model.TimeZoneOffset = timezoneOffset;
				model.DatepickerTimezoneString = String.Format("{0:D4}", (sitesTimezone.GetUtcOffset(DateTimeOffset.Now).Hours * 100) + sitesTimezone.GetUtcOffset(DateTimeOffset.Now).Minutes); //must be formatted "-0500" for jquery datepicker
				if (timezoneOffset >= 0)
				{
					model.DatepickerTimezoneString = model.DatepickerTimezoneString.PadLeft(5,'+'); // + sign needed for UTC +1 or more
				}
				List<PointValueIdentifier> pointValueIdentifiers = new List<PointValueIdentifier>(1);
				pointValueIdentifiers.Add(pointValueIdentifier);
				var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifiers));
				model.SelectedPointValue = pointValues[0];

				if(model.SelectedPointValue.PointValueIdentifier.PointValueType == PointValueType.Setting
				&& (model.SelectedPointValue.PointValueIdentifier.PropertyID == "ProductTable"
				|| model.SelectedPointValue.PointValueIdentifier.PropertyID == "BottomsTable"
				|| model.SelectedPointValue.PointValueIdentifier.PropertyID == "SolidsTable"))
				{
					model.EnumerationList = this.GetStrapTableEnumSelectList(model.SelectedPointValue.PointValueIdentifier.IdentityGuid);
				}
				else  if (model.SelectedPointValue.PointValueIdentifier.PointValueType == PointValueType.Point
				&& model.SelectedPointValue.PointValueIdentifier.PropertyID == "ProductID")
				{
					model.EnumerationList = this.GetProductEnumSelectList(model.SelectedPointValue.PointValueIdentifier.IdentityGuid);
				}
				else if ((model.SelectedPointValue.PointValueIdentifier.PointValueType == PointValueType.Tag)
				&& (model.SelectedPointValue.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference"))
				{
					model.EnumerationList = this.GetPointCommandStatusEnumSelectList(model.SelectedPointValue.PointValueIdentifier.IdentityGuid);
				}
				else if (model.SelectedPointValue.ValueTypeString == "System.Boolean")
				{
					model.EnumerationList = GetBooleanEnumSelectList();
				}
				else if (model.SelectedPointValue.ValueTypeString == "System.DateTimeOffset" && model.SelectedPointValue.Value != null)
				{
					model.SelectedPointValue.Value = TimeZoneInfo.ConvertTime((DateTimeOffset)model.SelectedPointValue.Value, sitesTimezone);
				}
				// JSON.parse is used on client side which doesn't handle NaN
				else if((model.SelectedPointValue.ValueTypeString == "System.Double"
				|| model.SelectedPointValue.ValueTypeString == "System.Single")
				&& Double.IsNaN(Convert.ToDouble(model.SelectedPointValue.Value)))
				{
					model.SelectedPointValue.Value = "NaN";
				}

				model.UpdatePointService = true; // call point service to update the value
				model.AllowOverUnderRange = true;

				return PartialViewWithErrorMessages("EditValue", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Getting Value") + " " + e.Message));
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="PointGuid"></param>
		/// <param name="TankOrVolume">True for a Tank</param>
		/// <param name="SourceOrDest">True for a Source</param>
		/// <param name="LevelOrBatch">True for Level</param>
		/// <param name="GrossOrNet">True for Gross</param>
		/// <returns></returns>
		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult EditValueForMovement(Guid PointGuid, bool TankOrVolume, bool SourceOrDest, bool LevelOrBatch, bool GrossOrNet, string CurrentValue, int Units)
		{
			var model = new EditValueModel();
			
			try
			{
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				model.Site = site;

				var numberFormatInfo = new NumberFormatInfo()
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
					NumberDecimalDigits = 0
				};

				Dictionary<Guid, PointTag> pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
															x => x.EnumerateByPointGuid(this.Security, PointGuid));

				string TagLevelProductMinOpLimit = "Level Product Min Op Limit";
				string TagLevelProductMaxOpLimit = "Level Product Max Op Limit";

				PointTag levelProductTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.LevelProductGuid);
				PointTag levelProductMinOpLimitTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.ID == TagLevelProductMinOpLimit);
				PointTag levelProductMaxOpLimitTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.ID == TagLevelProductMaxOpLimit);

				PointTag volumeGrossObservedTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedGuid);
				PointTag volumeGrossAvailableTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedAvailableGuid);
				PointTag volumeGrossRemainingTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeGrossObservedRemainingGuid);

				PointTag volumeNetStandardTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardGuid);
				PointTag volumeNetStandardAvailableTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardAvailableGuid);
				PointTag volumeNetStandardRemainingTag = (PointTag)pointTagDictionary.Values.FirstOrDefault(tag => tag.WellKnownIdentityGuid == Guids.VolumeNetStandardRemainingGuid);

				PointValueIdentifier pointValueIdentifier = new PointValueIdentifier(levelProductTag);
				List<PointValueIdentifier> pointValueIdentifiers = new List<PointValueIdentifier>(10);

				if(GrossOrNet)
				{
					pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossObservedTag));
					if(TankOrVolume) pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossAvailableTag));
					if(TankOrVolume) pointValueIdentifiers.Add(new PointValueIdentifier(volumeGrossRemainingTag));
				}
				else
                {
					pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardTag));
					if (TankOrVolume) pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardAvailableTag));
					if (TankOrVolume) pointValueIdentifiers.Add(new PointValueIdentifier(volumeNetStandardRemainingTag));
				}

				if(TankOrVolume)
                {
					pointValueIdentifiers.Add(new PointValueIdentifier(levelProductTag));
					pointValueIdentifiers.Add(new PointValueIdentifier(levelProductMinOpLimitTag));
					pointValueIdentifiers.Add(new PointValueIdentifier(levelProductMaxOpLimitTag));
				}

				var pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifiers));

				PointValue pValue = null;

				if (LevelOrBatch) // Level
				{
					pValue = pointValues[3];

					if (pointValues[4].Value != null)
					{
						Double.TryParse(pointValues[4].Value.ToString(), out double minValue);
						pValue.Minimum = minValue;
					}

					if (pointValues[5].Value != null)
					{
						Double.TryParse(pointValues[5].Value.ToString(), out double maxValue);
						pValue.Maximum = maxValue;
					}

					if (pValue.Value != null)
					{
						if (SourceOrDest)
						{
							Double.TryParse(pointValues[3].Value.ToString(), out double maxValue);
							pValue.Maximum = maxValue;
						}
						else
                  {
							Double.TryParse(pointValues[3].Value.ToString(), out double minValue);
							pValue.Minimum = minValue;
						}
					}
				}
				else
				{
					pValue = pointValues[0];

					if (SourceOrDest)
					{
						// available vol is the qty allowed to be removed from the current value before dropping below min op limit
						if (TankOrVolume && pointValues[1].Value != null)// Tanks only
						{
							Double.TryParse(pointValues[0].Value.ToString(), out double currentVolume);
							Double.TryParse(pointValues[1].Value.ToString(), out double availableVolume);
							pValue.Minimum = -availableVolume;// currentVolume - availableVolume;
							pValue.Maximum = 0;// currentVolume;
						}
					}
					else
					{
						// remaining vol is the qty allowed to be added from the current value before exceeding max op limit
						if (TankOrVolume && pointValues[2].Value != null)// Tanks only
						{
							Double.TryParse(pointValues[0].Value.ToString(), out double currentVolume);
							Double.TryParse(pointValues[2].Value.ToString(), out double remainingVolume);
							pValue.Minimum = 0;// currentVolume;
							pValue.Maximum = remainingVolume;// currentVolume + remainingVolume;
						}
					}
				}

				if (pValue != null)
				{
					if (pValue.ValueTypeString == "System.Double" || pValue.ValueTypeString == "System.Single")
					{
						numberFormatInfo.NumberDecimalDigits = 0;
					}

					if (CurrentValue != null && String.IsNullOrEmpty(CurrentValue.Trim()))  // Unintialized, so start with minimum
					{
						if (LevelOrBatch) // Level mode
						{
							CurrentValue = pValue.Minimum.ToString();
						}
						else // Batch mode
						{
							if(TankOrVolume && SourceOrDest) // Tank - Source - Batch
								CurrentValue = pValue.Maximum.ToString();
							else
								CurrentValue = pValue.Minimum.ToString();
						}
					}

					pValue.Value = PointManager.ParseValue(
						Type.GetType(pValue.ValueTypeString),
						pValue.Units,
						numberFormatInfo,
						CurrentValue);


					model.SelectedPointValue = pValue;
				}

				model.UpdatePointService = false; // call point service to update the value
				model.AllowOverUnderRange = true;

				return PartialViewWithErrorMessages("EditMovementValue", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				string msg = this.GetTranslatedText("Movement|Error Getting Movement Node Value");
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + " " + except.Message, FMEventLogEntryType.Error));

				this.OnError(new Exception(msg));
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		// ReSharper disable once InconsistentNaming
		public ActionResult EditValueForUI(bool isPointTemplate, PointValueIdentifier pointValueIdentifier, string id, string value, string valueTypeString, byte decimalPlaces, EngineeringUnitType unitType, EngineeringUnit unit, string maximum, string minimum, Guid pointTemplateGuid, string pointName, string AllowOverUnderRange)
		{
			var model = new EditValueModel();
			model.SelectedPointValue = new PointValue();

			model.SelectedPointValue.PointValueIdentifier = pointValueIdentifier;
			model.SelectedPointValue.ValueTypeString = valueTypeString;
			model.SelectedPointValue.DecimalPlaces = decimalPlaces;
			model.SelectedPointValue.EngineeringUnitsType = unitType;
			model.SelectedPointValue.Units = unit;
			model.SelectedPointValue.PointID = pointName;
			model.SelectedPointValue.ID = id;
			model.SelectedPointValue.InputOutputType = PointTemplateTag.PointTagInputOutputType.Manual;
			model.UpdatePointService = false;
			if(AllowOverUnderRange == "False")
				model.AllowOverUnderRange = false;
			else
				model.AllowOverUnderRange = true;

			Double tempMaximum = 0;
			if (Double.TryParse(maximum, out tempMaximum))
			{
				model.SelectedPointValue.Maximum = tempMaximum;
			}
			Double tempMinimum = 0;
			if (Double.TryParse(minimum, out tempMinimum))
			{
				model.SelectedPointValue.Minimum = tempMinimum;
			}

			//checked and reset the decimal places if they are incorrect
			switch (model.SelectedPointValue.ValueTypeString)
			{
				case "System.Double":
				case "System.Single":
					model.SelectedPointValue.DecimalPlaces = 9;
					break;
				case "System.Int64":
				case "System.Int32":
				case "System.Int16":
				case "System.UInt64":
				case "System.UInt32":
				case "System.UInt16":
					model.SelectedPointValue.DecimalPlaces = 0;
					break;
			}

			try
			{
				model.Site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				if (model.SelectedPointValue.PointValueIdentifier.PointValueType == PointValueType.Setting
				&& (model.SelectedPointValue.PointValueIdentifier.PropertyID == "ProductTable"
				|| model.SelectedPointValue.PointValueIdentifier.PropertyID == "BottomsTable"
				|| model.SelectedPointValue.PointValueIdentifier.PropertyID == "SolidsTable"))
				{
					model.EnumerationList = this.GetStrapTableEnumSelectList(model.SelectedPointValue.PointValueIdentifier.IdentityGuid);
				}
				else if (model.SelectedPointValue.PointValueIdentifier.PointValueType == PointValueType.Point
				&& model.SelectedPointValue.PointValueIdentifier.PropertyID == "ProductID")
				{
					model.EnumerationList = this.GetProductEnumSelectList(model.SelectedPointValue.PointValueIdentifier.IdentityGuid);
				}
				else if ((model.SelectedPointValue.PointValueIdentifier.PointValueType == PointValueType.Tag)
					&& (model.SelectedPointValue.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference"))
				{
					PointCommandStatusListReference pointCommandStatusListReference = new PointCommandStatusListReference();

					// Point Editor doesn't have the full value, only the CurrentKey
					if (!isPointTemplate)
					{
						var pointTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, pointValueIdentifier.IdentityGuid));
						var pointTemplateTag = FMChannelHelper.MakeCall<IPointTemplateTags, PointTemplateTag>(x => x.Get(this.Security, pointTag.PointTemplateTagGuid));
						if (pointTag.Value is PointCommandStatusListReference)
						{
							pointCommandStatusListReference = pointTag.Value as PointCommandStatusListReference;
							pointCommandStatusListReference.CurrentKey = value;
							model.EnumerationList = this.GetPointCommandStatusEnumSelectListForTemplate(pointTemplateTag.PointTemplateGuid, pointCommandStatusListReference);
							foreach (var selectListItem in model.EnumerationList)
							{
								if (selectListItem.Text == value)
								{
									pointCommandStatusListReference.CurrentValue = System.Convert.ToInt32(selectListItem.Value);
									break;
								}
							}
						}
						else
						{
							pointTag.Value = pointCommandStatusListReference;
						}
					}
					else
					{
						if (!value.IsNullOrWhiteSpace())
						{
							pointCommandStatusListReference = JsonConvert.DeserializeObject<PointCommandStatusListReference>(value);
						}
						model.EnumerationList = this.GetPointCommandStatusEnumSelectListForTemplate(pointTemplateGuid, pointCommandStatusListReference);
					}
					model.SelectedPointValue.Value = pointCommandStatusListReference;
				}
				else if ((model.SelectedPointValue.PointValueIdentifier.PointValueType == PointValueType.Tag)
					&& (model.SelectedPointValue.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference"))
				{
					DeviceAlarmMapReference deviceAlarmMapReference = new DeviceAlarmMapReference();
					if (!value.IsNullOrWhiteSpace())
					{
						deviceAlarmMapReference = JsonConvert.DeserializeObject<DeviceAlarmMapReference>(value);
					}
					model.SelectedPointValue.Value = deviceAlarmMapReference;
				}
				else if (model.SelectedPointValue.ValueTypeString == "System.Boolean")
				{
					model.EnumerationList = GetBooleanEnumSelectList();
				}
				else if (model.SelectedPointValue.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables", StringComparison.Ordinal) != -1)
				{
					if (!value.IsNullOrWhiteSpace())
					{
						foreach (var enumVal in GetEnumSelectList(model.SelectedPointValue.ValueTypeString))
						{
							if (enumVal.Text.Equals(value, StringComparison.InvariantCultureIgnoreCase))
							{
								model.SelectedPointValue.Value = enumVal.Value;
							}
						}
					}
				}			

			// get the proper value object
			switch (model.SelectedPointValue.ValueTypeString)
				{
					case "System.Boolean":
						if (value.IsNullOrWhiteSpace())  // if no value default to false
						{
							model.SelectedPointValue.Value = (bool)false;
						}
						else
						{
							model.SelectedPointValue.Value = PointsController.ParsePointSettingBooleanValue(this.ModelState, id, value, id);
						}
						break;
					case "System.DateTime":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingDateTimeValue(this.ModelState, id, value, model.Site, id);
						break;
					case "System.DateTimeOffset":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingDateTimeOffsetValue(this.ModelState, id, value, model.Site, id);
						break;
					case "System.TimeSpan":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingTimeSpanValue(this.ModelState, id, value, model.Site, id);
						break;
					case "System.String":
						model.SelectedPointValue.Value = value;
						break;
					case "System.Double":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingDoubleValue(this.ModelState, id, value, model.Site, id);
						break;
					case "System.Single":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingSingleValue(this.ModelState, id, value, model.Site, id);
						break;
					case "System.Int64":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingInt64Value(this.ModelState, id, value, model.Site, id);
						break;
					case "System.Int32":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingInt32Value(this.ModelState, id, value, model.Site, id);
						break;
					case "System.Int16":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingInt16Value(this.ModelState, id, value, model.Site, id);
						break;
					case "System.UInt64":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingUInt64Value(this.ModelState, id, value, model.Site, id);
						break;
					case "System.UInt32":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingUInt32Value(this.ModelState, id, value, model.Site, id);
						break;
					case "System.UInt16":
						model.SelectedPointValue.Value = PointsController.ParsePointSettingUInt16Value(this.ModelState, id, value, model.Site, id);
						break;
				}

				// JSON.parse is used on client side which doesn't handle NaN
				if ((model.SelectedPointValue.ValueTypeString == "System.Double"
					|| model.SelectedPointValue.ValueTypeString == "System.Single")
					&& Double.IsNaN(Convert.ToDouble(model.SelectedPointValue.Value)))
				{
					model.SelectedPointValue.Value = "NaN";
				}


				return PartialViewWithErrorMessages("EditValue", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Getting Value") + " " + e.Message));
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}
		[HttpPost, ValidateJsonAntiForgeryToken]
		// ReSharper disable once InconsistentNaming

			public ActionResult EditValueForCalculatorUI(string identityGuid, 
														string pointValueType,
														string propertyID,
														string valueTypeString,
														string id,
														string value,
														string decimalPlaces,
														string unitType,
														string unit,
														string maximum,
														string minimum,
														string pointName)
		{
			// IdentityGuid,PointValueType and  PropertyID are used in the object PointValueIdentifier
			PointValueType pointvalueType = PointValueType.Tag;// new FMBusinessObjects.DataObjects.PointValueType();

			Guid pointTemplateGuid = Guid.Empty;

			PointValueIdentifier pointValueIdentifier = new PointValueIdentifier(new Guid(identityGuid), pointvalueType, propertyID);

			byte idecimalPlaces = System.Convert.ToByte(decimalPlaces);
			EngineeringUnitType engrunitType = EngineeringUnitType.FmuNone;

			switch (unitType)
			{
				case "FmuTemp": // Temperature
					engrunitType = EngineeringUnitType.FmuTemp;
					break;
				case "FmuLength": // Level
					engrunitType = EngineeringUnitType.FmuLength;
					break;
				case "FmuVolume": // Volume
					engrunitType = EngineeringUnitType.FmuVolume;
					break;
				case "FmuMass": // Mass
					engrunitType = EngineeringUnitType.FmuMass;
					break;
				case "FmuPressure": // Pressure
					engrunitType = EngineeringUnitType.FmuPressure;
					break;
				case "FmuVolflow": // Volumetric Flow
					engrunitType = EngineeringUnitType.FmuVolflow;
					break;
				case "FmuMassflow": // Mass Flow
					engrunitType = EngineeringUnitType.FmuMassflow;
					break;
				case "FmuVelocity": // Velocity
					engrunitType = EngineeringUnitType.FmuVelocity;
					break;
				case "FmuDensity": // Density
					engrunitType = EngineeringUnitType.FmuDensity;
					break;
				default:
					break;
			}

			EngineeringUnit engrUnit = (EngineeringUnit)System.Convert.ToInt16(unit);

			return EditValueForUI(false,
									pointValueIdentifier,
									id,
									value,
									valueTypeString,
									idecimalPlaces,
									engrunitType,
									engrUnit,
									maximum,
									minimum,
									pointTemplateGuid,
									pointName,
									"False");

		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SetValue(PointValueIdentifier pointValueIdentifier, string value, string localoverride)
		{
			try
			{
                
            List<PointValueIdentifier> pointValueIdentifiers = new List<PointValueIdentifier>(1);
				pointValueIdentifiers.Add(pointValueIdentifier);
				List<PointValue> pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, pointValueIdentifiers));

				if(pointValues == null || pointValues.Count != 1 || pointValues[0] == null)
				{
					throw new Exception("EditController SetValue: error reading value");
				}

				var pointValue = pointValues[0];
				bool FCEEClearOverride = (localoverride == "False" && pointValue.IsForced() && pointValue.Input && pointValue.InputOutputType == PointTemplateTag.PointTagInputOutputType.FCEE);

            var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo()
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator,
					NumberDecimalDigits = pointValue.DecimalPlaces
				};

				if (pointValue.ValueTypeString.IndexOf("FMBusinessObjects.DataObjects.CodedVariables") != -1)
				{
					Type type = Type.GetType(pointValue.ValueTypeString + ",FMBusinessObjects");
					pointValue.Value = Enum.ToObject(type, int.Parse(value));
				}

				else if (pointValue.ValueTypeString == "System.DateTime")
				{
					pointValue.Value = DateTime.SpecifyKind(DateTime.ParseExact(value, site.ShortDatePattern, site.GetDateTimeFormatInfo()), DateTimeKind.Local);
				}
				else if (pointValue.ValueTypeString == "System.DateTimeOffset")
				{
					if (!string.IsNullOrEmpty(value))
					{
                  TimeZoneInfo sitesTimezone = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
                  DateTime thisTime = DateTime.Now;
                  double systemTimezoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
                  double timezoneOffset = sitesTimezone.GetUtcOffset(DateTimeOffset.Now).TotalMinutes;
                  DateTime currentSiteTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, sitesTimezone);

                  // The value from the jquery date picker gets parsed using the UTC offset of the server
                  pointValue.Value = DateTimeOffset.ParseExact(value, site.ShortDatePattern + " " + site.TimePattern, site.GetDateTimeFormatInfo());
					   var dateSiteFormat = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
                  DateTimeOffset correctedDateTimeOffsetValue = (DateTimeOffset)pointValue.Value;

                  // Use the difference in offset between the site time and system time to get the correct time
						// Although it will still have the system UTC offset
                  pointValue.Value = correctedDateTimeOffsetValue.AddMinutes(-(timezoneOffset - systemTimezoneOffset));

					   // Formats the correct time to use the site UTC offset
                  pointValue.Value = TimeZoneInfo.ConvertTime((DateTimeOffset)pointValue.Value, dateSiteFormat);

					}
					else
					{
						pointValue.Value = null;
					}
				}

				else if (pointValue.ValueTypeString == "System.TimeSpan")
				{
					pointValue.Value = TimeSpan.Parse(value);
				}
				else if (pointValue.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
				{
					if (!value.IsNullOrWhiteSpace())
					{
						PointCommandStatusListReference pclr = JsonConvert.DeserializeObject<PointCommandStatusListReference>(value);
						pointValue.Value = pclr;
					}
					else
					{
						PointCommandStatusListReference pclr = pointValue.Value as PointCommandStatusListReference;

						if (pclr != null)
						{
							pclr.CurrentValue = null;
							if ((value != null) && (value.Trim().Length > 0))
							{
								pclr.CurrentValue = null;
							}

							pointValue.Value = pclr;
						}
						else
						{
							pointValue.Value = null;
						}
					}
				}
				else if (pointValue.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
				{
					if (!value.IsNullOrWhiteSpace())
					{
						DeviceAlarmMapReference damr = JsonConvert.DeserializeObject<DeviceAlarmMapReference>(value);
						pointValue.Value = damr;
					}
					else
					{
						DeviceAlarmMapReference damr = pointValue.Value as DeviceAlarmMapReference;

						if (damr != null)
						{
							damr.CurrentValue = null;
							if ((value != null) && (value.Trim().Length > 0))
							{
								damr.CurrentValue = null;
							}

							pointValue.Value = damr;
						}
						else
						{
							pointValue.Value = null;
						}
					}
				}
				else
				{
					if(pointValue.ValueTypeString ==  "System.Double" ||
						pointValue.ValueTypeString == "System.Single")
					{
						numberFormatInfo.NumberDecimalDigits = 9;
					}
					pointValue.Value = PointManager.ParseValue(
						Type.GetType(pointValue.ValueTypeString),
						pointValue.Units,
						numberFormatInfo,
						value);
				}

				pointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
				pointValue.SourceTimeStamp = DateTimeOffset.UtcNow;

				if (pointValue.Value == null
				|| (pointValue.Value is PointCommandStatusListReference
				&& !(pointValue.Value as PointCommandStatusListReference).CurrentValue.HasValue)
                || (pointValue.Value is DeviceAlarmMapReference
                && (pointValue.Value as DeviceAlarmMapReference).CurrentValue == null))
				{
					pointValue.Status = StatusCodes.Bad;
				}
				else
				{
					if (localoverride == "False")
					{
						// When clearing a force on an input set the value to null, it will be recalculated or refreshed from OpcUa
						if (pointValue.IsForced()
						&& pointValue.Input)
						{
							if (pointValue.ValueTypeString == "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
							{
								(pointValue.Value as PointCommandStatusListReference).CurrentValue = null;
							}
							else if (pointValue.ValueTypeString == "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
							{
								(pointValue.Value as DeviceAlarmMapReference).CurrentValue = null;
							}
							else
							{
								pointValue.Value = null;
							}

							if (pointValue.Enabled)
							{
								pointValue.Status = StatusCodes.Bad;
							}
							else
							{
								pointValue.Status = StatusCodes.BadOutOfService;
							}

						}
						else
						{
							pointValue.Status = StatusCodes.Good;
						}
					}
					else
					{
						pointValue.Status = StatusCodes.GoodLocalOverride;
					}


					// check and set over and under range settings
					var statusCode = new StatusCode((uint)pointValue.Status);
					// before checking the values we need to format and round the value to the precision of the variable
					// if not then any reading close to the bounds will report overrange when it is not
					if (pointValue.Value is double)
					{
						object dPointMax = PointManager.ParseValue(Type.GetType(pointValue.ValueTypeString),
																				pointValue.Units,
																				numberFormatInfo,
																				pointValue.Maximum.ToString("N", numberFormatInfo));

						object dPointMIN = PointManager.ParseValue(Type.GetType(pointValue.ValueTypeString),
																				pointValue.Units,
																				numberFormatInfo,
																				pointValue.Minimum.ToString("N", numberFormatInfo));

						if ((double)pointValue.Value < (double)dPointMIN)
						{
							statusCode.LimitBits = LimitBits.Low;
						}
						else if ((double)pointValue.Value > (double)dPointMax)
						{
							statusCode.LimitBits = LimitBits.High;
						}
						else
						{
							statusCode.LimitBits = LimitBits.None;
						}
					}
					pointValue.Status = (long)statusCode;

				}

				// Update the point Value to the Point Service Manager and/or OPC UA.
				var pointValueList = new List<PointValue> { pointValue };
				SetPointValues(this.Security, site, pointValueList);

				// initiate ClearFCEEOverrideValue process if applicable
				if (FCEEClearOverride)
				{
					FMChannelHelper.MakeCall<IFCEEServiceManager, bool>(x => x.Refresh(this.Security, pointValue));
				}
			}
			catch (Exception e)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Setting Value : " + e.Message)));
			}

			return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}



		[NonAction]
		public IEnumerable<SelectListItem> GetStrapTableEnumSelectList(Guid propertyGuid)
		{
			var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, propertyGuid));
			var strapTable = pointProperty.Value as StrapTable;
			if (strapTable != null)
			{

				var list = new List<SelectListItem>();

				foreach (var individualStrapTable in strapTable.StrapTables)
				{
					list.Add(new SelectListItem()
					{
						Value = individualStrapTable.StrapTableDescription,
						Text = individualStrapTable.StrapTableDescription
					});


				}
				return list;
			}
			throw new ArgumentException("Property must be a StrapTable");
		}

		[NonAction]
		public IEnumerable<SelectListItem> GetProductEnumSelectList(Guid pointGuid)
		{
			var productIdList = FMChannelHelper.MakeCall<IProducts, List<string>>(x => x.EnumerateIdBySite(this.Security));
			var list = new List<SelectListItem>();

			list.Add(new SelectListItem()
			{
				Value = "",
				Text = ""
			});

			foreach (var productID in productIdList)
			{
				list.Add(new SelectListItem()
				{
					Value = productID,
					Text = productID
				});

			}
			return list;
		}


		[NonAction]
		public IEnumerable<SelectListItem> GetPointCommandStatusEnumSelectList(Guid pointTagGuid)
		{
			var list = new List<SelectListItem>();
			list.Add(new SelectListItem() { Value = "", Text = "" });

			PointTag pointTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, pointTagGuid));
			if ((pointTag == null) || (pointTag.ValueType != typeof(FMBusinessObjects.DataObjects.PointCommandStatusListReference))
				|| (pointTag.Value == null) || (pointTag.Value.GetType() != typeof(FMBusinessObjects.DataObjects.PointCommandStatusListReference)))
				return list;

			PointTemplateTag pointTemplateTag = FMChannelHelper.MakeCall<IPointTemplateTags, PointTemplateTag>(x => x.Get(this.Security, pointTag.PointTemplateTagGuid));

			FMBusinessObjects.DataObjects.PointCommandStatusListReference pclr = (FMBusinessObjects.DataObjects.PointCommandStatusListReference)(pointTag.Value);
			if ((pclr.PointCommandStatusListGuid == Guid.Empty))
				return list;

			var pointCommandStatusList = FMChannelHelper.MakeCall<IPointTemplates, PointCommandStatusList>(x => x.GetPointCommandStatusList(this.Security, pointTemplateTag.PointTemplateGuid, pclr.PointCommandStatusListGuid));

			foreach(var commandStatusElement in pointCommandStatusList.CommandStatusList)
			{
				list.Add(new SelectListItem()
				{

					Value = System.Convert.ToString(commandStatusElement.Value),
					Text = commandStatusElement.Key
				});
			}

			return list;
		}

		[NonAction]
		public IEnumerable<SelectListItem> GetPointCommandStatusEnumSelectListForTemplate(Guid pointTemplateGuid, FMBusinessObjects.DataObjects.PointCommandStatusListReference pclr)
		{
			var list = new List<SelectListItem>();

			if ((pclr.PointCommandStatusListGuid == Guid.Empty))
				return list;

			var pointCommandStatusList = FMChannelHelper.MakeCall<IPointTemplates, PointCommandStatusList>(x => x.GetPointCommandStatusList(this.Security, pointTemplateGuid, pclr.PointCommandStatusListGuid));

			foreach(var commandStatusElement in pointCommandStatusList.CommandStatusList)
			{
				list.Add(new SelectListItem()
				{

					Value = System.Convert.ToString(commandStatusElement.Value),
					Text = commandStatusElement.Key
				});
			}

			return list;
		}
		[NonAction]
		public IEnumerable<SelectListItem> GetBooleanEnumSelectList()
		{
			var list = new List<SelectListItem>();
			list.Add(new SelectListItem() { Text = this.GetTranslatedText("True"), Value = "True"});
			list.Add(new SelectListItem() { Text = this.GetTranslatedText("False"), Value = "False" });
			return list;
		}
	}
}