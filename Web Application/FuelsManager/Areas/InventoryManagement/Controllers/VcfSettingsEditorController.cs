namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.ServiceModel;
	using System.Web.Mvc;

	using Areas.Controllers;
	using ViewModels;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using System.Collections.Generic;
	using System.Globalization;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class VcfSettingsEditorController : FMBaseControllerEx
	{

		[HttpGet]
		public ActionResult VcfSettingsEditor(bool isTemplatePoint, Guid pointGuid, Guid pointPropertyGuid)
		{
			try
			{
				var readOnly = false; // the view is readOnly if the product Associated with the point is set to use the VCFCorrection from the Product
				BasePoint basePoint = null;
				VcfModuleSettings vcfModuleSettings = null;
				string pointPropertyId = string.Empty;

				if (isTemplatePoint)
				{
					basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, pointGuid));
					var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointPropertyGuid));
					vcfModuleSettings = pointTemplateProperty.Value as VcfModuleSettings;
					pointPropertyId = pointTemplateProperty.ID;
				}
				else
				{
					var point = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointGuid));
					basePoint = point;
					var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropertyGuid));
					vcfModuleSettings = pointProperty.Value as VcfModuleSettings;
					pointPropertyId = pointProperty.ID;

					if (point.ProductGuid != null)
					{
						var product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(this.Security, point.ProductID));
						if (product.ApplyVolumeCorrection)
						{
							readOnly = true;
						}
					}
				}


				var model = new VcfSettingsEditorModel(isTemplatePoint, basePoint.ID,  pointGuid, pointPropertyId, pointPropertyGuid, vcfModuleSettings, basePoint.PressureUnit, readOnly);
				return PartialViewWithErrorMessages("VcfSettingsEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception)
			{
				this.OnError(new Exception(this.GetTranslatedText("Error Getting Vcf Property")));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult SaveVcfSettings(VcfSettingsEditorModel model)
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


				model.VcfSettings.CorrectionMethodType = VcfModuleSettings.GetCorrectionTypeMajor(model.StandardsOrganization, model.StandardAndRevision, model.StandardTemperature);
				model.VcfSettings.CorrectionMethodSpecific = VcfModuleSettings.GetCorrectionTypeMinor(model.StandardsOrganization, model.StandardAndRevision, model.CommodityOrTable, model.StandardTemperature);

				ValidateVCFModuleProperty(this.ModelState, numberFormatInfo, basePoint, model.VcfSettings, null);

				if (this.ModelState.IsValid)
				{
					if (this.ModelState.IsValid)
					{
						if (model.IsTemplatePoint)
						{
							var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, model.PointPropertyGuid));
							pointTemplateProperty.Value = model.VcfSettings;
							FMChannelHelper.MakeCall<IPointTemplateProperties>(x => x.ModifyPointTemplatePropertyValue(this.Security, pointTemplateProperty));
						}
						else
						{
							var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, model.PointPropertyGuid));
							pointProperty.Value = model.VcfSettings;
							FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.Security, pointProperty, false, false));
						}

						this.AddSuccess("Save Successful");
					}
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
				this.OnError(new Exception(this.GetTranslatedText("Error Saving Vcf Property")));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		public static void ValidateVCFModuleProperty(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, BasePoint basePoint, VcfModuleSettings VcfValue, List<PointDefaultUnitChangeHistory> defaultUnitConversionHistory)
		{
			if (defaultUnitConversionHistory != null)
			{
				foreach (var historyEntry in defaultUnitConversionHistory)
				{
					if (historyEntry.PerformConversion)
					{
						switch (historyEntry.UnitType)
						{
							case "PressureUnit":
								VcfValue.DensityPressure.Value = EngineeringUnits.Convert(VcfValue.DensityPressure.Value, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 0);
								VcfValue.AlternateBasePressure.Value = EngineeringUnits.Convert(VcfValue.AlternateBasePressure.Value, (EngineeringUnit)historyEntry.OldUnit, (EngineeringUnit)historyEntry.NewUnit, 0);
								break;

							default:
								break;
						}
					}
				}
			}
		}
	}
}