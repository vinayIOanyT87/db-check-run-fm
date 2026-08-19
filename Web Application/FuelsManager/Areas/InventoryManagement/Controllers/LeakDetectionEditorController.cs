using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FuelsManager.Areas.Controllers;
using FuelsManager.Areas.InventoryManagement.ViewModels;
using System;
using System.Web.Mvc;
using System.ServiceModel;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
    public class LeakDetectionEditorController : FMBaseControllerEx
    {

        [HttpGet, ValidateJsonAntiForgeryToken]
        public ActionResult LeakDetectionEditor(bool isTemplatePoint, Guid pointGuid, Guid pointPropertyGuid)
        {
            LeakDetectionSettings leakDetectionSettings = null;
            BasePoint basePoint = null;
            SiteClass site = null;
            string pointPropertyID = string.Empty;

            try
            {
                if (isTemplatePoint)
                {
                    basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, pointGuid));
                    var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, pointPropertyGuid));
                    pointPropertyID = pointTemplateProperty.ID;
                    leakDetectionSettings = pointTemplateProperty.Value as LeakDetectionSettings;
                }
                else
                {
                    basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointGuid));
                    var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, pointPropertyGuid));
                    pointPropertyID = pointProperty.ID;
                    leakDetectionSettings = pointProperty.Value as LeakDetectionSettings;
                }

                site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, basePoint.SiteGuid, false, false, false));

                if (leakDetectionSettings == null)
                {
                    throw new InvalidOperationException("Leak Detection not found in the Point.");
                }
            }
            catch (Exception except)
            {
                this.OnError(except);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
            GaugeTypeCollectionClass gaugeTypes = FMChannelHelper.MakeCall<IGaugeTypes, GaugeTypeCollectionClass>(x => x.Enumerate(this.Security));
            return this.PartialViewWithErrorMessages("LeakDetectionEditor", new LeakDetectionEditorModel(isTemplatePoint, pointPropertyID, pointPropertyGuid, basePoint, site, leakDetectionSettings, 0, gaugeTypes), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveLeakDetectionEditorSettings(LeakAnalysisMethod LeakAnalysisMethod, LeakAnalysisType LeakAnalysisType, bool LeakAutoPrint, int LeakPrintDaysBeforeEndOfMonth, string GaugeType, bool isTemplatePoint, Guid pointGuid, Guid PointPropertyGuid, DateTime LeakPrintTime, string MinimumFillPercentageStr)
        {


            try
            {
                BasePoint basePoint = null;

                if (isTemplatePoint)
                {
                    basePoint = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.GetPointTemplateBaseData(this.Security, pointGuid));
                }
                else
                {
                    basePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, pointGuid));
                }
                int MinimumFillPercentage =0;
                int number;
                if(int.TryParse(MinimumFillPercentageStr, out number))
                {
                    MinimumFillPercentage = number;
                    if (MinimumFillPercentage < 0 || MinimumFillPercentage > 100)
                    {
                        ModelState.AddModelError("MinimumFillPercentage", "Minimum Fill Percentage must be between 0 and 100 inclusive");
                    }
                }
                else
                {
                    ModelState.AddModelError("MinimumFillPercentage", "Minimum Fill Percentage must be a integer between 0 and 100 inclusive");
                }

                if (this.ModelState.IsValid)
                {
                    LeakDetectionSettings leakDetectionSettings = new LeakDetectionSettings
                    {
                        AnalysisMethod = LeakAnalysisMethod,
                        AnalysisType = LeakAnalysisType,
                        AutoPrint = LeakAutoPrint,
                        PrintDaysBeforeEOM = LeakPrintDaysBeforeEndOfMonth,
                        GaugeType = GaugeType,
                        PrintTime = LeakPrintTime,
                        MinimumFillPercentage = MinimumFillPercentage
                    };

                    // Auto print is only applicable to Real time
                    if (leakDetectionSettings.AnalysisType != LeakAnalysisType.RealTime)
                    {
                        leakDetectionSettings.AutoPrint = false;
                    }


                    if (isTemplatePoint)
                    {
                        var pointTemplateProperty = FMChannelHelper.MakeCall<IPointTemplateProperties, PointTemplateProperty>(x => x.Get(this.Security, PointPropertyGuid));
                        pointTemplateProperty.Value = leakDetectionSettings;
                        FMChannelHelper.MakeCall<IPointTemplateProperties>(x => x.ModifyPointTemplatePropertyValue(this.Security, pointTemplateProperty));
                    }
                    else
                    {
                        var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(x => x.Get(this.Security, PointPropertyGuid));
                        pointProperty.Value = leakDetectionSettings;
                        FMChannelHelper.MakeCall<IPointProperties>(x => x.ModifyPointPropertyValue(this.Security, pointProperty, false, false));
                    }

                    this.AddSuccess("Saved Successful");
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
                this.OnError(new Exception(this.GetTranslatedText("Error Saving Leak Detection Module Settings")));
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}