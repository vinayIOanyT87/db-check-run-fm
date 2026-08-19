using FMBusinessObjects.DataObjects;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
    using Crypt;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.DataObjects.CodedVariables;
    using FMPointCommon;
    using FuelsManager.Areas.Controllers;
    using FuelsManager.Areas.InventoryManagement.ViewModels;
    using Opc.Ua;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class PointCalculatorController : FMBaseControllerEx
    {
        public List<string> selectedTagsList = new List<string>
        {
            "Level Product", "Volume Total Observed", "Level Water",
            "Volume Water", "Level Solids", "Volume Solids",
            "Volume Bottoms", "Temperature Product", "Temperature Vapor",
            "Temperature Ambient", "Tank Shell Correction", "Density Product Standard",
            "Temperature Density", "Density Product Observed", "Volume Roof Correction",
            "Volume Gross Observed", "Volume Correction Factor", "Volume Gross Standard",
            "Percent BSW", "Volume BSW", "Volume Net Standard",
            "Volume Total Calculated", "Weight Gross Standard", "Volume Gross Observed Available",
            "Volume Gross Observed Remaining", "Weight Net Standard", "Volume Net Standard Available",
            "Volume Net Standard Remaining", "Pressure Vapor", "Density Vapor",
             "Mass Liquid", "Mass Vapor"
        };

		  public List<string> acronyms = new List<string>
        {
            "", "TOV", "",
            "", "", "",
            "FW", "Temp", "",
            "", "CTSh", "",
            "", "", "FRC + FRA",
            "GOV", "CTL", "GSV",
            "%BSW", "", "NSV",
            "TCV", "GSW", "",
             "", "NSW", "",
             "", "", "",
             "", ""
        };

        public List<bool> isBatchModeTag = new List<bool>
        {
            false, true, false,
            true, false, true,
            true, true, false,
            false, false, true,
            false, true, false,
            true, true, false,
            true, true, true,
            false, false, false,
            false, false, false,
            false, false, false,
            false, false
        };

        public List<string> batchTagNames = new List<string>
        {
            "Volume Total Observed",
            "Volume Correction Factor",
            "Volume Water",
            "Volume Solids",
            "Volume Bottoms",
            "Temperature Product",
            "Density Product Standard",
            "Density Product Observed",
            "Volume Gross Observed",
            "Percent BSW",
            "Volume BSW",
            "Volume Net Standard"
        };

        public List<string> TagsNotEditibleList = new List<string>
        {
            "Tank Shell Correction",
            "Volume Correction Factor",
            "Volume Gross Observed Available",
            "Volume Gross Observed Remaining",
            "Volume Net Standard Available",
            "Volume Net Standard Remaining",
            "Volume Roof Correction",
            "Volume Bottoms",
            "Volume BSW",
            "Mass Vapor",
            "Volume Total Calculated",
            "Volume Gross Standard",
            "Weight Gross Standard",
            "Weight Net Standard",
        };

      [HttpPost]
      public ActionResult PointCalculatorSaveRowVisibilityConfigValue(UInt32 configValue)
      {
         try
         {
            FMChannelHelper.MakeCall<IConfigurationSettings>(
            configSettings =>
               {
                  configSettings.Modify(this.Security, ConfigurationSettingDOClass.Key_PointCalculatorRowVisibilityConfig, configValue.ToString());
               });
		   }
			catch (Exception)
			{
				return this.Json("ERROR");
			}

			return this.Json("SUCCESS");
		}

		// GET: InventoryManagement/PointCalculator
		[HttpPost]
        public ActionResult PointCalculatorView(string pointIdString, string pointGuidString, bool isBatchMode)
        {
            try
            {
                var model = new PointCalculatorModel();
                var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				   string rowConfigValue = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
				   configurationSettings => configurationSettings.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_PointCalculatorRowVisibilityConfig));

				   bool goodRowConfigValue = UInt32.TryParse(rowConfigValue, out uint iRowConfigValue);

				   model.PointCalculatorRowVisibilityConfig = goodRowConfigValue ? iRowConfigValue : 4294967295;

				   model.Format = new NumberFormatInfo
                {
                    NumberGroupSizes = site.GetNumberGroupSizes(),
                    NumberGroupSeparator = site.NumberGroupSeparator,
                    NumberDecimalSeparator = site.NumberDecimalSeparator,
                };
                model.datePattern = site.ShortDatePattern;
                model.timePattern = site.TimePattern;
                model.siteId = site.ID;
                model.selectedBasePoint = pointIdString;
                model.selectedBasePointGuid = new Guid(pointGuidString);
                model.calculatorItemList = this.LoadPointTags(model);
                model.colorswipeIndex = -1;
                model.changedTagGuid = new Guid();
                model.changedTagColumn = "";
                model.isBatchMode = isBatchMode;
                model.batchModeKey = BatchModeKey.None;
                model.enableTransfer = true;

				   bool isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsEnterpriseKey());

				   string syncSettingsValue = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
				   configurationSettings => configurationSettings.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_SynchronizedSettings));

               bool enableRowVisibilityConfigDropdown = true;

					if (!string.IsNullOrWhiteSpace(syncSettingsValue))
					{
						var syncSettingsItems = syncSettingsValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
						var hasPointCalculatorRowVisibilityConfig = syncSettingsItems.Any(
							x => string.Equals(x.Trim(), ConfigurationSettingDOClass.Key_PointCalculatorRowVisibilityConfig, StringComparison.OrdinalIgnoreCase));

						if (hasPointCalculatorRowVisibilityConfig)
						{
							enableRowVisibilityConfigDropdown = false;
						}
					}

               model.EnableRowVisibilityConfigDropdown =
                 this.Security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS) && (enableRowVisibilityConfigDropdown || isEnterprise);


                // calculate the 3rd column values
                ExecutePointCalculator(model);

                var wellKnownTagGuidList = new Guid[] {
                    Guids.TransferModeGuid,
                    Guids.TransferTargetGuid
                };

                var pointGuidlist = new List<Guid> {
                        new Guid(pointGuidString)
                };

                // Getting two tags per point, so expect a list of twice as many identifiers as nodes.
                var pointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(
                x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidlist, wellKnownTagGuidList.ToList()));

                var pointAccessDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, PointValueAccess>>(x => x.EnumerateRestrictedAccessByPointValueIdenfierList(this.Security, pointValueIdentifierList));

                PointValueAccess transferModeValueAccess = null;
                PointValueAccess transferTargetValueAccess = null;

                if (pointAccessDictionary != null)
                {
                    pointAccessDictionary.TryGetValue(pointValueIdentifierList[0], out transferModeValueAccess);
                    pointAccessDictionary.TryGetValue(pointValueIdentifierList[1], out transferTargetValueAccess);

                    if ((transferModeValueAccess != null
                    && !transferModeValueAccess.Modify)
                    || (transferTargetValueAccess != null
                    && !transferTargetValueAccess.Modify))
                    {
                        model.enableTransfer = false;
                    }
                }

                var pointPropertyGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(
                                                    x => x.GetPointPropertyGuid(this.Security, new Guid(pointGuidString), "Tank Transfer Settings"));

                var pointProperty = FMChannelHelper.MakeCall<IPointProperties, PointProperty>(
                                                    x => x.Get(this.Security, pointPropertyGuid));

                if (pointProperty != null)
                {
                    TankTransferModuleSettings tankTransferModuleSettings = pointProperty.Value as TankTransferModuleSettings;

                    if (tankTransferModuleSettings != null)
                    {
                        model.transferByNet = (tankTransferModuleSettings.TransferVolumeMode == TransferVolumeMode.NetStandardVolume) ? true : false;
                    }
                }

                return this.PartialViewWithErrorMessages("PointCalculatorView", model);
            }
            catch (Exception e)
            {
                this.OnError(e);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult PointCalculatorCheckIfOkToTransfer(string pointGuidString, string targetRaw, string targetBatchAsLevelRaw, string mode)
        {
            try
            {
                var wellKnownTagGuidList = new Guid[] {
                    Guids.LevelProductMaxOpLimitWellKnownGuid,
                    Guids.LevelProductMinOpLimitWellKnownGuid,
                    Guids.TransferStatusGuid,
                    Guids.TransferTargetGuid,
                    Guids.TransferLevelTargetGuid,
                };

                var pointGuidlist = new List<Guid> {
                    new Guid(pointGuidString)
                };

                var pointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(
                x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidlist, wellKnownTagGuidList.ToList()));

                var pointAccessDictionary = FMChannelHelper.MakeCall<IPoints, Dictionary<PointValueIdentifier, PointValueAccess>>(x => x.EnumerateRestrictedAccessByPointValueIdenfierList(this.Security, pointValueIdentifierList));

                PointValueAccess transferTargetValueAccess = null;
                var canExceed = true;

                if (pointAccessDictionary != null)
                {
                    pointAccessDictionary.TryGetValue(pointValueIdentifierList[3], out transferTargetValueAccess);
                    canExceed = transferTargetValueAccess == null || transferTargetValueAccess.ExceedRange;
                }

                var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(
                x => x.GetPointValueData(this.Security, pointValueIdentifierList, false));

                double? dblTargetBatchAsLevelRawRounded = null;
                double? dblTargetRawRounded = null;
                double? dblMinLimitRounded = null;
                double? dblMaxLimitRounded = null;
                
                var dblTargetBatchAsLevelRaw = 0.0;
                var dblTargetRaw = double.Parse(targetRaw);
                if (double.TryParse(targetBatchAsLevelRaw, out dblTargetBatchAsLevelRaw))
                {
                    dblTargetBatchAsLevelRawRounded = dblTargetBatchAsLevelRaw;
                }

                switch (pointValueList[4].Units)
                {
                    case EngineeringUnit.FmlFtIn16Th:
                        if (dblTargetBatchAsLevelRawRounded.HasValue)
                        {
                            dblTargetBatchAsLevelRawRounded = PointManager.RoundToFtIn16th(dblTargetBatchAsLevelRawRounded.Value);
                        }
                        dblTargetRawRounded = PointManager.RoundToFtIn16th(dblTargetRaw);
                        dblMinLimitRounded = PointManager.RoundToFtIn16th((double)pointValueList[1].Value);
                        dblMaxLimitRounded = PointManager.RoundToFtIn16th((double)pointValueList[0].Value);
                        break;
                    case EngineeringUnit.FmlFtIn8Th:
                        if (dblTargetBatchAsLevelRawRounded.HasValue)
                        {
                            dblTargetBatchAsLevelRawRounded = PointManager.RoundToFtIn8th(dblTargetBatchAsLevelRawRounded.Value);
                        }
                        dblTargetRawRounded = PointManager.RoundToFtIn8th(dblTargetRaw);
                        dblMinLimitRounded = PointManager.RoundToFtIn8th((double)pointValueList[1].Value);
                        dblMaxLimitRounded = PointManager.RoundToFtIn8th((double)pointValueList[0].Value);
                        break;
                    default:
                        dblTargetRawRounded = dblTargetRaw;
                        dblMinLimitRounded = (double)pointValueList[1].Value;
                        dblMaxLimitRounded = (double)pointValueList[0].Value;
                        break;
                }

                if ((TransferStatuses)pointValueList[2].Value == TransferStatuses.Complete)
                {
                    return this.JsonWithErrorMessages(OkToTransferStatus.NotOkToTransferAlreadyComplete, JsonRequestBehavior.AllowGet);
                }
                else if (mode != "Batch" && dblTargetRawRounded > dblMinLimitRounded && dblTargetRawRounded < dblMaxLimitRounded)
                {
                    return this.JsonWithErrorMessages(OkToTransferStatus.OkToTransfer, JsonRequestBehavior.AllowGet);
                }
                else if (mode != "Batch" && dblTargetRawRounded <= dblMinLimitRounded)
                {
                    if (canExceed)
                    { 
                        return this.JsonWithErrorMessages(OkToTransferStatus.OkToTransferLowerThanMinOp, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return this.JsonWithErrorMessages(OkToTransferStatus.NotOkToTransferNoExceedLoPermission, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (mode != "Batch" && dblTargetRawRounded >= dblMaxLimitRounded)
                {
                    if (canExceed)
                    { 
                        return this.JsonWithErrorMessages(OkToTransferStatus.OkToTransferHigherThanMaxOp, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return this.JsonWithErrorMessages(OkToTransferStatus.NotOkToTransferNoExceedHiPermission, JsonRequestBehavior.AllowGet);
                    }
                }

                else if (mode == "Batch" && dblTargetBatchAsLevelRawRounded.HasValue && dblTargetBatchAsLevelRawRounded > dblMinLimitRounded && dblTargetBatchAsLevelRawRounded < dblMaxLimitRounded)
                {
                    return this.JsonWithErrorMessages(OkToTransferStatus.OkToTransfer, JsonRequestBehavior.AllowGet);
                }
                else if (mode == "Batch" && dblTargetBatchAsLevelRawRounded.HasValue && dblTargetBatchAsLevelRawRounded <= dblMinLimitRounded)
                {
                    if (canExceed)
                    { 
                        return this.JsonWithErrorMessages(OkToTransferStatus.OkToTransferLowerThanMinOp, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return this.JsonWithErrorMessages(OkToTransferStatus.NotOkToTransferNoExceedLoPermission, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (mode == "Batch" && dblTargetBatchAsLevelRawRounded.HasValue && dblTargetBatchAsLevelRawRounded >= dblMaxLimitRounded)
                {
                    if (canExceed)
                    { 
                        return this.JsonWithErrorMessages(OkToTransferStatus.OkToTransferHigherThanMaxOp, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return this.JsonWithErrorMessages(OkToTransferStatus.NotOkToTransferNoExceedHiPermission, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return this.JsonWithErrorMessages(OkToTransferStatus.NotOkToTransfer, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                this.OnError(e);
                return this.JsonWithErrorMessages(OkToTransferStatus.NotOkToTransfer, JsonRequestBehavior.AllowGet);
            }
        }

        private enum OkToTransferStatus { 
            NotOkToTransfer = 0,
            NotOkToTransferAlreadyComplete = 1,
            OkToTransferLowerThanMinOp = 2,
            OkToTransferHigherThanMaxOp = 3,
            OkToTransfer = 4,
            NotOkToTransferNoExceedLoPermission = 5,
            NotOkToTransferNoExceedHiPermission = 6,
        };

        [HttpPost]
        public ActionResult PointCalculatorInitiateTransfer(string pointGuidString, double target, string mode)
        {
            try
            {
                var wellKnownTagGuidList = new Guid[] {
                    Guids.TransferModeGuid,
                    Guids.TransferTargetGuid
                };

                var pointGuidlist = new List<Guid> {
                    new Guid(pointGuidString)
                };

                // Getting two tags per point, so expect a list of twice as many identifiers as nodes.
                var pointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(
                x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.Security, pointGuidlist, wellKnownTagGuidList.ToList()));

                var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(
                x => x.GetPointValueData(this.Security, pointValueIdentifierList, false));

                if (mode == "Batch")
                {
                    pointValueList[0].Value = FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Batch;
                }
                else
                {
                    pointValueList[0].Value = FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode.Level;
                }

                pointValueList[0].Status = StatusCodes.Good;
                pointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
                pointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;


                pointValueList[1].Value = target;
                pointValueList[1].Status = StatusCodes.Good;
                pointValueList[1].ServerTimeStamp = DateTimeOffset.UtcNow;
                pointValueList[1].SourceTimeStamp = DateTimeOffset.UtcNow;

                FMChannelHelper.MakeCall<IPointServiceManager>(
                x => x.SetPointValueData(this.Security, pointValueList, false));

                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                this.OnError(e);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public ActionResult PointCalculatorReport(PointCalculatorModel model)
        {
            var calcResult = new PointCalculatorResult();
            calcResult.PointGuid = model.selectedBasePointGuid;
            calcResult.PointId = model.selectedBasePoint;
            calcResult.SiteGuid = this.Security.SiteGuid;
            calcResult.SiteId = this.Security.SiteID;
            calcResult.UserGuid = this.Security.UserGuid;
            calcResult.UserId = this.Security.UserID;
            calcResult.Token = this.Security.Token;
            calcResult.CalculationMode = model.isBatchMode ? "Batch" : "Differential";

            // build the items
            List<PointCalculatorTagValue> tagValues = new List<PointCalculatorTagValue>(model.calculatorItemList.Count);

            for (int i = 0; i < model.calculatorItemList.Count; i++)
            {
                var calcItem = model.calculatorItemList[i];
                if (!calcItem.isVisible)
                    continue;
                tagValues.Add(new PointCalculatorTagValue(calcItem.tagName, calcItem.startValue, calcItem.endValue,
                    i, calcItem.unitsString, calcItem.acronym ?? string.Empty, ((model.isBatchMode && !calcItem.isBatchModeTag) ? String.Empty : calcItem.diffValue)));
            }
            calcResult.TagValues = tagValues;

            // persist to DB
            Guid? RunGuid = FMChannelHelper.MakeCall<IPointServiceManager, Guid?>(x => x.SavePointCalculatorTagValues(this.Security, calcResult));

            JsonResult jsonResult = new JsonResult();
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.ContentType = "application/json";
            //jsonResult.Data = new { success = (RunGuid != null), RunId = (RunGuid == null) ? "Oops! something went wrong! Please check event log." : RunGuid.ToString() };
            if (RunGuid != null)
            {
                jsonResult.Data = new { success = true, RunId = RunGuid.ToString() };
            }
            else
            {
                jsonResult.Data = new { error = true, message = "Oops! something went wrong! Please check event log." };
            }
            return jsonResult;
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        public ActionResult PointCalculatorUpdateValues(PointCalculatorModel model)
        {

            if (!(new string[] { "start", "end", "diff" }.Any(x => x == model.changedTagColumn)))
            {
                this.OnError("Invalid Calculator Column");
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
            model.Format = new NumberFormatInfo
            {
                NumberGroupSizes = site.GetNumberGroupSizes(),
                NumberGroupSeparator = site.NumberGroupSeparator,
                NumberDecimalSeparator = site.NumberDecimalSeparator,
            };

            try
            {
                ExecutePointCalculator(model);

                model.changedTagGuid = new Guid();
                model.changedTagColumn = "";

                return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                this.OnError(e);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ValidateJsonAntiForgeryToken]
        public ActionResult Acronyms()
        {
            return this.JsonWithErrorMessages(this.acronyms, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ValidateJsonAntiForgeryToken]
        public ActionResult IsBatchModeTags()
        {
            return this.JsonWithErrorMessages(this.isBatchModeTag, JsonRequestBehavior.AllowGet);
        }

        public List<calculatorItems> LoadPointTags(PointCalculatorModel model)
        {
            var selectedGuid = model.selectedBasePointGuid;
            List<calculatorItems> calculatorItemListtoReturn = new List<calculatorItems>();
            var pointTagGuids = new List<Guid>();

            // this is kinda of messed up but because of the changes wanted at the last minute there is no other way around this
            // I do not agree with the requested changes mainly because it changes this from a generic calculator
            // to a hard coded list. Sorry, this is not what I wanted

            for (int i = 0; i < this.selectedTagsList.Count; i++)
            {
                var tempCal = new calculatorItems 
                { 
                   tagName = this.selectedTagsList[i], 
                   tagGuid = null, 
                   editDisabled = 1, 
                   acronym = this.acronyms[i], 
                   isBatchModeTag = this.isBatchModeTag[i], 
                   isVisible = ((model.PointCalculatorRowVisibilityConfig & (1 << i)) != 0) 
                };

                calculatorItemListtoReturn.Add(tempCal);
            }

            Dictionary<Guid, PointTag> pointTagDictionary = null;
            try
            {
                pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
                        x => x.EnumerateByPointGuid(this.Security, selectedGuid));
            }
            catch
            {
            }

            if (pointTagDictionary == null)
            {
                return calculatorItemListtoReturn;
            }

            foreach (var tagdata in pointTagDictionary)
            {
                int pos = calculatorItemListtoReturn.FindIndex(n => n.tagName == tagdata.Value.ID);
                if (pos >= 0)
                {
                    var calcobject = calculatorItemListtoReturn[pos];
                    if (calcobject.tagName == tagdata.Value.ID)
                    {
                        calcobject.tagGuid = tagdata.Value.IdentityGuid;
                        pointTagGuids.Add(tagdata.Value.IdentityGuid);
                        calcobject.editDisabled = this.isTagEditDisabled(tagdata.Value);
                        calcobject.unitsString = EngineeringUnits.GetUnitAbbreviation(tagdata.Value.Units);
                        calcobject.units = ((int)tagdata.Value.Units).ToString();
                        calcobject.dataType = tagdata.Value.ValueType.ToString();
                        calcobject.UnitsType = tagdata.Value.EngineeringUnitsType;
                        calcobject.maximumValue = tagdata.Value.Maximum;
                        calcobject.minimumValue = tagdata.Value.Minimum;
                        calcobject.acronym = acronyms[pos];
                        calcobject.isBatchModeTag = isBatchModeTag[pos];
                    }
                }
            }

            // get the tag values
            List<PointTag> pointTags = null;
            try
            {
                pointTags = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(x => x.GetPointTagDataWithoutPointAccess(this.Security, pointTagGuids));
            }
            catch
            {
            }

            if (pointTags == null)
            {
                return calculatorItemListtoReturn;
            }

            // populate start and end values within calculatorItemList
            foreach (var pointTag in pointTags)
            {
                foreach (var calcobject in calculatorItemListtoReturn)
                {
                    if (calcobject.tagGuid == pointTag.PointTagGuid)
                    {
                        string rawValue = "0";
                        long status = StatusCodes.Bad;
                        calcobject.numberDecimals = pointTag.DecimalPlaces;
                        calcobject.startValue = this.setStartValue(pointTag, model.Format, ref rawValue, ref status);
                        calcobject.startValueRaw = rawValue;
                        calcobject.startStatus = 0;


                        // set the end equal to the start and the dif at 0 initialy
                        calcobject.endValue = calcobject.startValue;
                        calcobject.endValueRaw = calcobject.startValueRaw;
                        calcobject.endStatus = 0;
                    }
                }
            }

            return calculatorItemListtoReturn;
        }

        public void ExecutePointCalculator(PointCalculatorModel model)
        {
            var pointCalculatorData = new PointCalculatorData();
            var pointTagListStart = new List<PointTag>();
            var pointTagListEnd = new List<PointTag>();
            var pointTagListDiff = new List<PointTag>();
            var updateTimeStamp = DateTimeOffset.Now;

            // build the start tags
            Dictionary<Guid, PointTag> pointTagDictionaryStart = null;
            try
            {
                pointTagDictionaryStart =
                    FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
                        x => x.EnumerateByPointGuid(this.Security, model.selectedBasePointGuid));
            }
            catch
            {
            }

            foreach (var pointTagStart in pointTagDictionaryStart)
            {
                foreach (var calcobject in model.calculatorItemList)
                {
                    if (calcobject.tagGuid == pointTagStart.Key)
                    {
                        // TODO: convert the raw data to the value type (assumes all values are double for now)
                        pointTagStart.Value.Value = double.Parse(calcobject.startValueRaw);
                        pointTagStart.Value.Status = calcobject.startStatus;
                        if (model.changedTagColumn == "start")
                        {
                            if (pointTagStart.Value.PointTagGuid == model.changedTagGuid)
                            {
                                pointCalculatorData.ChangedDataSet = ChangedDataSet.Start;
                                pointCalculatorData.ChangedPointTagId = pointTagStart.Value.ID;
                                pointTagStart.Value.SourceTimeStamp = updateTimeStamp;
                            }
                        }
                        pointTagListStart.Add(pointTagStart.Value);
                        break;
                    }
                }
            }

            // build the end tags
            Dictionary<Guid, PointTag> pointTagDictionaryEnd = null;
            try
            {
                pointTagDictionaryEnd =
                    FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
                        x => x.EnumerateByPointGuid(this.Security, model.selectedBasePointGuid));
            }
            catch
            {
            }

            foreach (var pointTagEnd in pointTagDictionaryEnd)
            {
                foreach (var calcobject in model.calculatorItemList)
                {
                    if (calcobject.tagGuid == pointTagEnd.Key)
                    {
                        // TODO: convert the raw data to the value type (assumes all values are double for now)
                        pointTagEnd.Value.Value = double.Parse(calcobject.endValueRaw);
                        pointTagEnd.Value.Status = calcobject.endStatus;

                        if (model.changedTagColumn == "end")
                        {
                            if (pointTagEnd.Value.PointTagGuid == model.changedTagGuid)
                            {
                                pointCalculatorData.ChangedDataSet = ChangedDataSet.End;
                                pointCalculatorData.ChangedPointTagId = pointTagEnd.Value.ID;
                                pointTagEnd.Value.SourceTimeStamp = updateTimeStamp;
                                if (model.isBatchMode)
                                {
                                    model.BatchModeChangedColumn = "end";
                                    switch (pointTagEnd.Value.ID)
                                    {
                                        case "Level Product":
                                            model.batchModeKey = BatchModeKey.EndLevel;
                                            break;
                                        case "Volume Total Observed":
                                            model.batchModeKey = BatchModeKey.EndTOV;
                                            break;
                                        case "Volume Gross Observed":
                                            model.batchModeKey = BatchModeKey.EndGOV;
                                            break;
                                        case "Volume Net Standard":
                                            model.batchModeKey = BatchModeKey.EndNSV;
                                            break;
                                    }
                                }
                            }
                        }
                        pointTagListEnd.Add(pointTagEnd.Value);
                        break;
                    }
                }
            }

            // build the differential / batch values
            Dictionary<Guid, PointTag> pointTagDictionaryDiff = null;
            try
            {
                pointTagDictionaryDiff =
                    FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
                        x => x.EnumerateByPointGuid(this.Security, model.selectedBasePointGuid));
            }
            catch
            {
            }

            foreach (var pointTagDiff in pointTagDictionaryDiff)
            {
                foreach (var calcobject in model.calculatorItemList)
                {
                    if (calcobject.tagGuid == pointTagDiff.Key)
                    {
                        // TODO: convert the raw data to the value type (assumes all values are double for now)
                        if (!string.IsNullOrEmpty(calcobject.diffValueRaw))
                        {
                            pointTagDiff.Value.Value = double.Parse(calcobject.diffValueRaw);
                            pointTagDiff.Value.Status = calcobject.diffStatus;
                            if (model.changedTagColumn == "diff")
                            {
                                if (pointTagDiff.Value.PointTagGuid == model.changedTagGuid)
                                {
                                    pointCalculatorData.ChangedDataSet = ChangedDataSet.Diff;
                                    pointCalculatorData.ChangedPointTagId = pointTagDiff.Value.ID;
                                    pointTagDiff.Value.SourceTimeStamp = updateTimeStamp;
                                    if (model.isBatchMode)
                                    {
                                        model.BatchModeChangedColumn = "batch";
                                        switch (pointTagDiff.Value.ID)
                                        {
                                            case "Volume Total Observed":
                                                model.batchModeKey = BatchModeKey.BatchTOV;
                                                break;
                                            case "Volume Gross Observed":
                                                model.batchModeKey = BatchModeKey.BatchGOV;
                                                break;
                                            case "Volume Net Standard":
                                                model.batchModeKey = BatchModeKey.BatchNSV;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        pointTagListDiff.Add(pointTagDiff.Value);
                        break;
                    }
                }
            }

            // run the pointcalculator
            pointCalculatorData.StartTags = pointTagListStart;
            pointCalculatorData.EndTags = pointTagListEnd;
            pointCalculatorData.DiffTags = pointTagListDiff;
            pointCalculatorData.IsBatchMode = model.isBatchMode;
            pointCalculatorData.BatchModeKey = model.batchModeKey;

            pointCalculatorData =
                FMChannelHelper.MakeCall<IPointServiceManager, PointCalculatorData>(
                    x => x.RunPointCalculator(this.Security, model.selectedBasePointGuid, pointCalculatorData));

            // restore returned results to the list of calculation items
            var rawValue = string.Empty;
            foreach (var calcobject in model.calculatorItemList)
            {
                long status = StatusCodes.Good;

                //process start
                var startPointTag = pointCalculatorData.StartTags.First(x => x.ID == calcobject.tagName);
                calcobject.startValue = this.setStartValue(startPointTag, model.Format, ref rawValue, ref status);
                calcobject.startValueRaw = rawValue;
                calcobject.startStatus = status;


                //process end
                var endPointTag = pointCalculatorData.EndTags.First(x => x.ID == calcobject.tagName);
                calcobject.endValue = this.setStartValue(endPointTag, model.Format, ref rawValue, ref status);
                calcobject.endValueRaw = rawValue;
                calcobject.endStatus = status;

                //process diff
                var diffPointTag = pointCalculatorData.DiffTags.First(x => x.ID == calcobject.tagName);
                calcobject.diffValue = this.setStartValue(diffPointTag, model.Format, ref rawValue, ref status);
                calcobject.diffValueRaw = rawValue;
                calcobject.diffStatus = status;
            }
        }

        public byte isTagEditDisabled(PointTag stTagName)
        {
            byte returnValue = 0;

            foreach (var tagName in TagsNotEditibleList)
            {
                if (stTagName.ID == tagName)
                {
                    return (1);
                }
            }
            // check for the tag configuration
            // if a density element then it needs to be manual to be editable
            if (stTagName.EngineeringUnitsType == EngineeringUnitType.FmuDensity &&
                stTagName.InputOutputType == PointTemplateTag.PointTagInputOutputType.Calculated)
            {
                return 1;
            }
            // if it is unassigned return true
            if (stTagName.InputOutputType == PointTemplateTag.PointTagInputOutputType.UnAssigned)
            {
                return 1;
            }
            return returnValue;
        }

        private string setStartValue(PointTag tagdata, NumberFormatInfo format, ref string rawValue, ref long status)
        {
            string returnValue = string.Empty;
            status = StatusCodes.Bad;

            double archiveValueDouble;

            if (tagdata.Value == null)// tagdata.Status == StatusCodes.Bad ||
            {
                archiveValueDouble = 0;
            }
            else if (double.TryParse(tagdata.Value.ToString(), out archiveValueDouble) == false)
            {
                archiveValueDouble = 0;
            }

            rawValue = archiveValueDouble.ToString();
            // format based on engineering units
            switch (tagdata.Units)
            {
                case EngineeringUnit.FmlFtIn8Th:
                    returnValue = PointManager.FormatValue(typeof(Double), EngineeringUnit.FmlFtIn8Th, format, archiveValueDouble);
                    if (!string.IsNullOrEmpty(returnValue))
                    {
                        status = StatusCodes.Good;
                    }
                    return returnValue;

                case EngineeringUnit.FmlFtIn16Th:

                    returnValue = PointManager.FormatValue(typeof(Double), EngineeringUnit.FmlFtIn16Th, format, archiveValueDouble);
                    if (!string.IsNullOrEmpty(returnValue))
                    {
                        status = StatusCodes.Good;
                    }
                    return returnValue;

                default:
                    returnValue = formatDoubletostring(archiveValueDouble, format, tagdata);
                    if (!string.IsNullOrEmpty(returnValue))
                    {
                        status = StatusCodes.Good;
                    }
                    return returnValue;
            }
        }

        private string calculateDifferentialValue(string startValue,
                                            string endValue,
                                            EngineeringUnit engrUnits,
                                            byte decimalPlaces,
                                            NumberFormatInfo format,
                                            PointTag pointTag,
                                            ref string rawValue)
        {
            string returnValue = string.Empty;

            double startValueDouble = 0.0;
            double endValueDouble = 0.0;
            double calValue = 0.0;

            rawValue = "0";

            if (engrUnits == EngineeringUnit.FmlFtIn16Th ||
                engrUnits == EngineeringUnit.FmlFtIn8Th)
            {
                switch (engrUnits)
                {
                    case EngineeringUnit.FmlFtIn8Th:
                        if (!DecodeFtInFraction(8, startValue, ref startValueDouble))
                            return "00-00-00";
                        else if (!DecodeFtInFraction(8, endValue, ref endValueDouble))
                            return "00-00-00";

                        calValue = startValueDouble - endValueDouble;
                        rawValue = calValue.ToString();
                        return PointManager.FormatValue(typeof(Double), EngineeringUnit.FmlFtIn8Th, format, calValue);

                    case EngineeringUnit.FmlFtIn16Th:
                        if (!DecodeFtInFraction(16, startValue, ref startValueDouble))
                            return "00-00-00";
                        else if (!DecodeFtInFraction(16, endValue, ref endValueDouble))
                            return "00-00-00";

                        calValue = startValueDouble - endValueDouble;
                        rawValue = calValue.ToString();
                        return PointManager.FormatValue(typeof(Double), EngineeringUnit.FmlFtIn16Th, format, calValue);
                }
            }
            else
            {
                if (double.TryParse(startValue.ToString(), out startValueDouble) == false)
                    startValueDouble = 0;
                if (double.TryParse(endValue.ToString(), out endValueDouble) == false)
                    endValueDouble = 0;

                rawValue = (startValueDouble - endValueDouble).ToString();
                returnValue = formatDoubletostring(startValueDouble - endValueDouble, format, pointTag);
            }
            return returnValue;
        }

        private string GetRawValue(string Value,
                                        EngineeringUnit engrUnits)
        {
            string returnValue = string.Empty;

            double ValueDouble = 0.0;

            if (engrUnits == EngineeringUnit.FmlFtIn16Th ||
                engrUnits == EngineeringUnit.FmlFtIn8Th)
            {
                switch (engrUnits)
                {
                    case EngineeringUnit.FmlFtIn8Th:
                        if (!DecodeFtInFraction(8, Value, ref ValueDouble))
                            return "00-00-00";

                        return ValueDouble.ToString();

                    case EngineeringUnit.FmlFtIn16Th:
                        if (!DecodeFtInFraction(16, Value, ref ValueDouble))
                            return "00-00-00";

                        return ValueDouble.ToString();
                }
            }
            else
            {
                if (double.TryParse(Value.ToString(), out ValueDouble) == false)
                    ValueDouble = 0;

                return ValueDouble.ToString();
            }
            return returnValue;
        }

        private bool DecodeFtInFraction(int iDenominator, string pInput, ref double pFeet)
        {
            string tempValue = string.Empty;
            string tempValue1 = string.Empty;
            bool negativeValue = false;
            int ft = 0;
            int inches = 0;
            int sixteens = 0;

            if (string.IsNullOrEmpty(pInput))
                return false;

            tempValue = pInput.Trim();

            if (tempValue.Length <= 0)
                return false;

            negativeValue = false;
            if (tempValue.IndexOf('-') == 0)
                negativeValue = true;

            if (negativeValue)
            {
                string[] tokens = tempValue.Split('-');
                if (tokens.Count() != 4)
                    return false;
                ft = System.Convert.ToInt16(tokens[1]);
                inches = System.Convert.ToInt16(tokens[2]);
                sixteens = System.Convert.ToInt16(tokens[3]);
            }
            else
            {
                string[] tokens = tempValue.Split('-');
                if (tokens.Count() != 3)
                    return false;
                ft = System.Convert.ToInt16(tokens[0]);
                inches = System.Convert.ToInt16(tokens[1]);
                sixteens = System.Convert.ToInt16(tokens[2]);
            }

            double totalInches = inches + (sixteens / (double)iDenominator);
            pFeet = ft + (totalInches / 12.0);

            return true;
        }

        private string formatDoubletostring(double archiveValueDouble, NumberFormatInfo format, PointTag pointTag)
        {
            format.NumberDecimalDigits = pointTag.DecimalPlaces;

            return PointManager.FormatValue(typeof(double), pointTag.Units, format, archiveValueDouble);
        }
    }

}