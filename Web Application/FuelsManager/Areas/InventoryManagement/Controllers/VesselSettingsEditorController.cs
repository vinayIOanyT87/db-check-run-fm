namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.ServiceModel;
	using System.Web.Mvc;

	using Areas.Controllers;
	using ViewModels;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMBusinessObjects.DataObjects;
	using Varec.CommonComponents.EngineeringUnitsLibrary;


	public class VesselSettingsEditorController : FMBaseControllerEx
	{


		[HttpGet]
		public ActionResult VesselSettingsEditor(bool isTemplatePoint, Guid pointGuid, Guid pointPropertyGuid)
		{
			try
			{
				BasePoint basePoint = null;
				Vessel vessel = null;
				string pointPropertyId = string.Empty;

				if (isTemplatePoint)
				{
					basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, pointGuid));
					var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointPropertyGuid));
					vessel = pointTemplateProperty.Value as Vessel;
					pointPropertyId = pointTemplateProperty.ID;
				}
				else
				{
					basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointGuid));
					var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropertyGuid));
					vessel = pointProperty.Value as Vessel;
					pointPropertyId = pointProperty.ID;
				}

				var model = new VesselSettingsEditorModel(isTemplatePoint, basePoint, pointPropertyId, pointPropertyGuid, vessel);
				return PartialViewWithErrorMessages("VesselSettingsEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Getting Vessel Property")));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		protected static void UpdateExpansionCoefficient(Vessel vesselValue, EngineeringUnit temperatureUnit)
		{
			switch (vesselValue.TankMaterial)
			{
				case TankMaterialEnum.Unknown:
					vesselValue.TankExpansionCoefficient.Value = 0;
					vesselValue.AreaCoefficient.Value = 0;
					break;

				case TankMaterialEnum.MildCarbon:
					if (temperatureUnit == EngineeringUnit.FmtDegC || temperatureUnit == EngineeringUnit.FmtDegK)
					{
						vesselValue.TankExpansionCoefficient.Value = 1.12E-05;
					}
					else
					{
						vesselValue.TankExpansionCoefficient.Value = 6.20E-06;
					}
					vesselValue.AreaCoefficient.Value = 4.0e-009;
					break;

				case TankMaterialEnum.Stainless304:
					if (temperatureUnit == EngineeringUnit.FmtDegC || temperatureUnit == EngineeringUnit.FmtDegK)
					{
						vesselValue.TankExpansionCoefficient.Value = 1.73E-05;
					}
					else
					{
						vesselValue.TankExpansionCoefficient.Value = 9.60E-06;
					}
					vesselValue.AreaCoefficient.Value = 4.0e-009;
					break;

				case TankMaterialEnum.Stainless316:
					if (temperatureUnit == EngineeringUnit.FmtDegC || temperatureUnit == EngineeringUnit.FmtDegK)
					{
						vesselValue.TankExpansionCoefficient.Value = 1.59E-05;
					}
					else
					{
						vesselValue.TankExpansionCoefficient.Value = 8.83E-06;
					}
					vesselValue.AreaCoefficient.Value = 4.0e-009;
					break;

				case TankMaterialEnum.Stainless174PH:
					if (temperatureUnit == EngineeringUnit.FmtDegC || temperatureUnit == EngineeringUnit.FmtDegK)
					{
						vesselValue.TankExpansionCoefficient.Value = 1.08E-05;
					}
					else
					{
						vesselValue.TankExpansionCoefficient.Value = 6.00E-06;
					}
					vesselValue.AreaCoefficient.Value = 4.0e-009;
					break;


				case TankMaterialEnum.Aluminum:
					if (temperatureUnit == EngineeringUnit.FmtDegC || temperatureUnit == EngineeringUnit.FmtDegK)
					{
						vesselValue.TankExpansionCoefficient.Value = 22.00E-06;
					}
					else
					{
						vesselValue.TankExpansionCoefficient.Value = 12.80E-06;
					}
					vesselValue.AreaCoefficient.Value = 4.0e-009;
					break;

				case TankMaterialEnum.Other:
					break;

				default:
					break;
			}
		}

		public static void ValidateVesselProperty(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, BasePoint basePoint, Vessel vesselValue, List<PointDefaultUnitChangeHistory> defaultUnitConversionHistory)
		{

			if (defaultUnitConversionHistory != null)
			{
				foreach (var historyEntry in defaultUnitConversionHistory)
				{
					if (historyEntry.PerformConversion)
					{
						switch (historyEntry.UnitType)
						{
							case "LevelUnit":
								vesselValue.TankHeight.Value = EngineeringUnits.Convert(vesselValue.TankHeight.Value, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 0);
								vesselValue.TankRadius.Value = EngineeringUnits.Convert(vesselValue.TankRadius.Value, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 0);
								vesselValue.TankShellThickness.Value = EngineeringUnits.Convert(vesselValue.TankShellThickness.Value, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 0);
								break;
							case "VolumeUnit":
								vesselValue.TankVolume.Value = EngineeringUnits.Convert(vesselValue.TankVolume.Value, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 0);
								vesselValue.CSTCapacity.Value = EngineeringUnits.Convert(vesselValue.CSTCapacity.Value, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 0);
								break;

							case "TemperatureUnit":
								vesselValue.TankInstallationTemperature.Value = EngineeringUnits.Convert(vesselValue.TankInstallationTemperature.Value, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 0);
								break;

							default:
								break;
						}
					}

					if (historyEntry.UnitType == "TemperatureUnit")
					{
						UpdateExpansionCoefficient(vesselValue, (EngineeringUnit)historyEntry.NewUnit);
					}
				}
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SaveVesselSettings(VesselSettingsEditorModel model)
		{
			try
			{
				BasePoint basePoint = null;
				if (model.IsTemplatePoint)
				{
					basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, model.PointGuid));
				}
				else
				{
					basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, model.PointGuid));
				}

				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, basePoint.SiteGuid, false, false, false));

				var numberFormatInfo = new NumberFormatInfo
				{
					NumberGroupSizes = site.GetNumberGroupSizes(),
					NumberGroupSeparator = site.NumberGroupSeparator,
					NumberDecimalSeparator = site.NumberDecimalSeparator
				};


				model.VesselSettings.TankInstallationDateString = model.TankInstallationDate;
				model.VesselSettings.CSTCommissionDateString = model.CSTCommissionDate;
				model.VesselSettings.CSTManufactureDateString = model.CSTManufactureDate;

				ValidateVesselProperty(this.ModelState, numberFormatInfo, basePoint, model.VesselSettings, null);

				if (this.ModelState.IsValid)
				{
					if (model.IsTemplatePoint)
					{
						var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, model.PointPropertyGuid));
						pointTemplateProperty.Value = model.VesselSettings;
						FMChannelHelper.MakeCall<IPointTemplateProperties>(x => x.ModifyPointTemplatePropertyValue(this.Security, pointTemplateProperty));
					}
					else
					{
						var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, model.PointPropertyGuid));
						pointProperty.Value = model.VesselSettings;
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
				this.OnError(new Exception(this.GetTranslatedText("Error Saving Vessel Property")));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}
	}
}