namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Globalization;
	using System.ServiceModel;
	using System.Web.Mvc;

	using Areas.Controllers;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using System.Linq;
	using System.Collections.Generic;

	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class RateModuleSettingsEditorController : FMBaseControllerEx
	{
		#region Private data members
		private SiteClass site;
		#endregion

		protected Guid GetInputTagForRateModuleUsingSetting(Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Guid pointTemplatePropertyGuid)
		{
			foreach (var moduleInstance in moduleInstances.Values)
			{
				foreach(var propertyModule in moduleInstance.ModuleToPointTemplateData.PropertyToModules)
				{ 
					if(propertyModule.PropertyGuid == pointTemplatePropertyGuid)
					{
						foreach (var tagToModule in moduleInstance.ModuleToPointTemplateData.TagToModules)
						{
							if(tagToModule.ModuleParameter == "Rate")
							{
								return tagToModule.TagGuid;
							}
						}
					}
				}
			}

			return Guid.Empty;
		}

		protected EngineeringUnit GetKnownUnits(EngineeringUnitType type)
		{
			var knownUnits = EngineeringUnit.FmvrMMin;
			switch (type)
			{
				case EngineeringUnitType.FmuVelocity:
					knownUnits = EngineeringUnit.FmvrMMin;
					break;
				case EngineeringUnitType.FmuVolflow:
					knownUnits = EngineeringUnit.FmvfM3Min;
					break;
				case EngineeringUnitType.FmuMassflow:
					knownUnits = EngineeringUnit.FmmfKgMin;
					break;
			}
			return knownUnits;
		}

		/// <summary>
		/// This method is called by the UI to retreive the rate module settings.
		/// </summary>
		/// <param name="pointGuid">The point GUID used to retrieve the rate module settings.</param>
		/// <param name="pointPropertyGuid">The point property GUID to retrieve the rate module settings property.</param>
		/// <returns>Returns the Rate Module Settings editor model.</returns>
		[HttpGet]
		public ActionResult RateModuleSettingsEditor(bool isTemplatePoint, Guid pointGuid, Guid pointPropertyGuid)
		{
			try
			{
				const bool ReadOnly = false;
				this.site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				BasePoint basePoint = null;
				RateModuleSettings rateModuleSettings = null;
				string rateModuleSettingsPropertyID = string.Empty;
				Guid pointTemplatePropertyGuid;
				EngineeringUnit units;
				EngineeringUnit knownUnits;
				EngineeringUnitType unitsType;
				int precision;

				if (isTemplatePoint)
				{
					var pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointGuid));
					basePoint = pointTemplate;
					var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointPropertyGuid));
					rateModuleSettings = pointTemplateProperty.Value as RateModuleSettings;
					rateModuleSettingsPropertyID = pointTemplateProperty.ID;
					pointTemplatePropertyGuid = pointTemplateProperty.PointTemplatePropertyGuid;
					var pointTemplateTagGuid = this.GetInputTagForRateModuleUsingSetting(pointTemplate.ModuleInstances, pointTemplatePropertyGuid);
					var pointTemplateTag = pointTemplate.Tags.Values.Single(x => x.PointTemplateTagGuid == pointTemplateTagGuid);
					units = pointTemplateTag.Units;
					knownUnits = this.GetKnownUnits(pointTemplateTag.EngineeringUnitsType);
					unitsType = pointTemplateTag.EngineeringUnitsType;
					if (unitsType == EngineeringUnitType.FmuVelocity)
						precision = pointTemplate.VelocityDecimalPlaces;
					else
						precision = pointTemplate.FlowDecimalPlaces;
				}
				else
				{
					var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, pointGuid));
					basePoint = point;
					var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropertyGuid));
					rateModuleSettings = pointProperty.Value as RateModuleSettings;
					rateModuleSettingsPropertyID = pointProperty.ID;
					pointTemplatePropertyGuid = pointProperty.PointTemplatePropertyGuid;
					var pointTemplateTagGuid = this.GetInputTagForRateModuleUsingSetting(point.ModuleInstances, pointTemplatePropertyGuid);
					var pointTag = point.Tags.Values.Single(x => x.PointTemplateTagGuid == pointTemplateTagGuid);
					units = pointTag.Units;
					knownUnits = this.GetKnownUnits(pointTag.EngineeringUnitsType);
					unitsType = pointTag.EngineeringUnitsType;
					if (unitsType == EngineeringUnitType.FmuVelocity) 
						precision = point.VelocityDecimalPlaces;
					else
						precision = point.FlowDecimalPlaces;
				}

				// Set deadband to zero if there isn't one set.
				if (string.IsNullOrEmpty(rateModuleSettings.Deadband))
				{
					rateModuleSettings.Deadband = "0";
				}

				double convertedDeadbandValue = this.ConvertDeadbandToNewUnit(knownUnits, units, rateModuleSettings.Deadband);
				rateModuleSettings.Deadband = convertedDeadbandValue.ToString();

				var model = new RateModuleSettingsEditorModel(	isTemplatePoint,
																				basePoint.ID, 
																				pointGuid,
																				rateModuleSettingsPropertyID,
																				pointPropertyGuid,
																				rateModuleSettings,
																				EngineeringUnits.GetUnitAbbreviation(units),
																				units,
																				unitsType,
																				ReadOnly);
				return this.PartialViewWithErrorMessages("RateModuleSettingsEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Getting Rate Module Property")));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will save the changes to the rate module settings.
		/// </summary>
		/// <param name="model">The rate module settings model to be saved.</param>
		/// <returns>Returns a success or failure message.</returns>
		public ActionResult SaveRateModuleSettings(RateModuleSettingsEditorModel model)
		{
			try
			{
				this.site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				if (this.ValidateStaleTimePeriod(model.RateModuleSettings.StaleTimePeriodInSeconds) == false)
				{
					this.OnError(new Exception(this.GetTranslatedText("Time Period must be between 0 and 3600 seconds.")));
					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}

				// this is being hard coded at averaging because people do not understand what we do. Even though the least squared calculation is correct and even
				// though it works people want it disabled because it is far to complicated and should not of been used for this company.
				if (model.RateModuleSettings.FlowCalculationType != "Averaging")
					model.RateModuleSettings.FlowCalculationType = "Averaging";

				if (model.RateModuleSettings.FlowCalculationType == "Averaging")
				{
					// check the sample and the sample time entries
					if (model.RateModuleSettings.AveragingNumberSamples < 2 ||
						model.RateModuleSettings.AveragingNumberSamples > 10)
					{
						this.OnError(new Exception(this.GetTranslatedText("Averaging Samples must be between 2 and 10.")));
						return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
					}
					if (model.RateModuleSettings.AveragingSampleTimeSeconds < 1 ||
						model.RateModuleSettings.AveragingSampleTimeSeconds > 120)
					{
						this.OnError(new Exception(this.GetTranslatedText("Averaging Samples Time must be between 1 and 120.")));
						return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
					}
				}
				else
				{
					// if the values are invalid just set them back to the defaults
					if (model.RateModuleSettings.AveragingNumberSamples < 2 ||
						model.RateModuleSettings.AveragingNumberSamples > 10)
					{
						model.RateModuleSettings.AveragingNumberSamples = 4;
					}
					if (model.RateModuleSettings.AveragingSampleTimeSeconds < 1 ||
						model.RateModuleSettings.AveragingSampleTimeSeconds > 120)
					{
						model.RateModuleSettings.AveragingSampleTimeSeconds = 10;
					}
				}
				// Check to see if the number separator is something other than a decimal point.
				// Valid the deadband is a number and not an empty string.
				string unformattedDeadband = this.UnformatDeadband(model.RateModuleSettings.Deadband);
				var knownType = this.GetKnownUnits(model.RateUnitType);
				double convertedDeadbandValue = this.ConvertDeadbandToNewUnit(model.RateUnit, knownType, unformattedDeadband);
				model.RateModuleSettings.Deadband = convertedDeadbandValue.ToString(CultureInfo.InvariantCulture);

				if (this.ModelState.IsValid)
				{
					if (model.IsTemplatePoint)
					{
						var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, model.PointPropertyGuid));
						pointTemplateProperty.Value = model.RateModuleSettings;
						FMChannelHelper.MakeCall<IPointTemplateProperties>(x => x.ModifyPointTemplatePropertyValue(this.Security, pointTemplateProperty));
					}
					else
					{
						var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, model.PointPropertyGuid));
						pointProperty.Value = model.RateModuleSettings;
						FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.Security, pointProperty, false, false));
					}

					this.AddSuccess("Save Successful");
				}

				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (CommunicationException e)
			{
				this.OnError(new Exception(this.GetTranslatedText(e.Message)));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Saving Rate Module Settings Property")));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}


		/// <summary>
		/// This method will convert the dead band to another unit. It will return zero if the dead band is
		/// null or empty.
		/// </summary>
		/// <param name="fromUnit">The current dead band unit.</param>
		/// <param name="toUnit">The unit to convert to.</param>
		/// <param name="deadband">The dead band value as a string.</param>
		/// <returns>Returns the converted dead band as a string formatted by the sites settings.</returns>
		private double ConvertDeadbandToNewUnit(EngineeringUnit fromUnit, EngineeringUnit toUnit, string deadband)
		{
			if (string.IsNullOrEmpty(deadband) || toUnit == EngineeringUnit.FmuNone || fromUnit == EngineeringUnit.FmuNone)
			{
				return 0.0;
			}

			double deadbandValue;

			if (double.TryParse(deadband, out deadbandValue) == false)
			{
				return 0.0;
			}

			double convertedDeadbandValue = EngineeringUnits.Convert(deadbandValue, fromUnit, toUnit, deadbandValue);

			return convertedDeadbandValue;
		}

		/// <summary>
		/// This method will format the deadband value to the proper display format.
		/// </summary>
		/// <param name="deadband">The dead band value.</param>
		/// <param name="precision">The number of decimals; should be loaded from the point or the point template.</param>
		/// <returns>Returns a formatted dead band string.</returns>
		private string FormatDeadband(double deadband, int precision)
		{
			double deadbandRounded = Math.Round(deadband, precision, MidpointRounding.AwayFromZero);

			var numericFormatInfo = this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
			numericFormatInfo.NumberDecimalDigits = precision;
			string newDeadbandStr = deadbandRounded.ToString("N",numericFormatInfo);
			return newDeadbandStr;
		}

		/// <summary>
		/// This method will unformat the number decimal separator if it is something
		/// other than a decimal point. In addition, it will valid the deadband is a
		/// valid number and not an empty string.
		/// </summary>
		/// <param name="deadbandStr"></param>
		/// <returns>Returns the dead band string with a decimal if needed.</returns>
		private string UnformatDeadband(string deadbandStr)
		{
			double deadband = 0;

			double.TryParse(deadbandStr, NumberStyles.Number, this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT), out deadband);

			return deadband.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// This method will validate the stale time period to ensure that it is
		/// between 0 and 3600 (hour) seconds.
		/// </summary>
		/// <param name="staleTimePeriod">The stale time period.</param>
		/// <returns>Returns turn if valid, otherwise returns false.</returns>
		private bool ValidateStaleTimePeriod(int staleTimePeriod)
		{
			if (staleTimePeriod < 0 || staleTimePeriod > 3600)
			{
				return false;
			}

			return true;
		}
	}
}