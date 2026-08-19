namespace FuelsManager.Areas.InventoryManagement.Controllers
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMPointCommon;
    using FuelsManager.Areas.Controllers;
    using FuelsManager.Areas.InventoryManagement.ViewModels;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Mvc;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class MovementHandgaugeEditorController : FMBaseControllerEx
    {
        #region Data Members
        private const string StartLevelField        = "StartLevel";
        private const string StartTemperatureField  = "StartTemperature";
        private const string StartStdDensityField   = "StartStdDensity";
        private const string StartDensityTempField  = "StartDensityTemp";
        private const string StartAmbTempField      = "StartAmbTemp";
        private const string StartWaterLevelField   = "StartWaterLevel";
        private const string StartRefHeightField    = "StartRefHeight";

        private const string EndLevelField          = "EndLevel";
        private const string EndTemperatureField    = "EndTemperature";
        private const string EndStdDensityField     = "EndStdDensity";
        private const string EndDensityTempField    = "EndDensityTemp";
        private const string EndAmbTempField        = "EndAmbTemp";
        private const string EndWaterLevelField     = "EndWaterLevel";
        private const string EndRefHeightField      = "EndRefHeight";

        private const string TagLevelProduct            = "Level Product";
        private const string TagLevelWater              = "Level Water";
        private const string TagTemperatureProduct      = "Temperature Product";
        private const string TagTemperatureAmbient      = "Temperature Ambient";
        private const string TagTemperatureDensity      = "Temperature Density";
        private const string TagDensityProductStd       = "Density Product Standard";
        private const string TagDensityProductObserved  = "Density Product Observed";
        private const string TagVcf                     = "Volume Correction Factor";
        private const string TagVolumeWater             = "Volume Water";
        private const string TagVolumeGrossObserved     = "Volume Gross Observed";
        private const string TagVolumeNetStd            = "Volume Net Standard";
        private const string TagVolumeRoofCorrection    = "Volume Roof Correction";
        private const string TagVolumeTotalObserved     = "Volume Total Observed";
        private const string TagMassLiquid              = "Mass Liquid";
        private const string TagTankShellCorrection     = "Tank Shell Correction";

        private NumberFormatInfo numberFormatInfo = null;

        private enum StartEndType { Start, End, Both };
        public enum ClientCaller { CallerMovementSummary, CallerMovementHistory };

        public List<string> lookupTagsList = new List<string>
        {
            TagLevelProduct,
            TagLevelWater,
            TagTemperatureProduct,
            TagTemperatureAmbient,
            TagTemperatureDensity,
            TagDensityProductStd,
            TagDensityProductObserved,
            TagVcf,
            TagVolumeWater,
            TagVolumeGrossObserved,
            TagVolumeNetStd,
            TagVolumeRoofCorrection,
            TagVolumeTotalObserved,
            TagMassLiquid,
            TagTankShellCorrection
        };
        #endregion

        #region Public static methods
        /// <summary>
        /// This method returns the string version of the model.
        /// </summary>
        /// <param name="model">The model to serialize.</param>
        /// <returns>Returns the string version of the model.</returns>
        [NonAction]
        public static string SerializeModel(MovementHandgaugeModel model)
        {
            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// This method will deserialize the model string into an object.
        /// </summary>
        /// <param name="modelStr">The string version of the model.</param>
        /// <returns>Returns the model as an object.</returns>
        [NonAction]
        public static MovementHandgaugeModel DeserializeModel(string modelStr)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var model = JsonConvert.DeserializeObject<MovementHandgaugeModel>(modelStr, jsonSerializerSettings);

            return model;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This is the main entry point.
        /// </summary>
        /// <param name="movementPointGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MovementHandgaugeEditor(Guid movementPointGuid, int caller, Guid movementHistoryGuid)
        {
            try
            {
                var clientCaller = (ClientCaller)caller;
                var model = new MovementHandgaugeModel { PointGuid = movementPointGuid, MovementHangaugeGuid = movementHistoryGuid };
                this.SetDateAndNumberFormats(model);
                this.SetNumberFormatting(model);

                if (clientCaller == ClientCaller.CallerMovementSummary)
                {
                    // Get the movement point information base on the movement point.
                    List<calculatorItems> calculatedList = this.GetMovementPointInfoFromCalculator(movementPointGuid);
                    this.UpdateModelFromPoints(ref model, ref calculatedList, StartEndType.Both);
                }

                if (clientCaller == ClientCaller.CallerMovementHistory)
                {
                    // Get the movement history calculated list.
                    List<calculatorItems> calculatedList = this.GetHistoryCalculatedList(movementHistoryGuid, StartEndType.Both);
                    this.UpdateModelFromPoints(ref model, ref calculatedList, StartEndType.Both);
                }

                model.HasModifyRights = this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_HISTORY);

                return base.PartialViewWithErrorMessages("MovementHandgaugeEditor", model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msgBasic = this.GetTranslatedText("Movement|Error Getting Movement Handgauge Data.");
                string msgEventLog = "MovementHandgaugeEditor: " + msgBasic + " " + ex.Message;
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

                base.OnError(new Exception(msgBasic));
                return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// This method is called by the UI to retrieve the movement hand gauge data for
        /// a given handgauge node.
        /// </summary>
        /// <param name="nodeId">The hand gauge node ID to retrieve data.</param>
        /// <param name="modelStr">The movement hand gauge model.</param>
        /// <returns>Returns the view.</returns>
        public ActionResult GetMovementHandgaugeData(string nodeId, string modelStr)
        {
            MovementHandgaugeModel model = DeserializeModel(modelStr);
            // TODO

            return this.View(model);
        }

        /// <summary>
        /// This method will call the legacy system to recalculate the handgauge data based on
        /// the field change.
        /// </summary>
        /// <param name="fieldId">The field ID of the field that has a change</param>
        /// <param name="modelStr">The model string to update on return.</param>
        /// <returns>Return the handgauge model</returns>
        public ActionResult CalculateMovementHandgaugeData(string fieldId, string modelStr)
        {
            MovementHandgaugeModel model = DeserializeModel(modelStr);
            this.SetDateAndNumberFormats(model);
            this.SetNumberFormatting(model);

            if (string.IsNullOrEmpty(fieldId))
            {
                return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
            }
            
            Dictionary<Guid, PointTag> pointTagDictionary = null;

            try
            {
                pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
                                                            x => x.EnumerateByPointGuid(this.Security, model.PointGuid));
            }
            catch(Exception ex)
            {
                string msgBasic = this.GetTranslatedText("Movement|Error Getting Movement Handgauge Data.");
                string msgEventLog = "MovementHandgaugeEditor: " + msgBasic + " " + ex.Message;
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

                base.OnError(new Exception(msgBasic));
                return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }

            var pointTagList = new List<PointTag>();

            try
            {
                // Process the start column
                if (fieldId.Contains("Start"))
                {
                    this.UpdateCalculatedValues(fieldId, model, pointTagDictionary, StartEndType.Start);                   
                }
                // process the end column
                else
                {
                    this.UpdateCalculatedValues(fieldId, model, pointTagDictionary, StartEndType.End);                   
                }

                return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                this.OnError(e);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method will save the data.
        /// </summary>
        /// <param name="modelStr">The model string.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveMovementHandgaugeData(string movementHandgaugeEditorModelStr, int caller)
        {
            MovementHandgaugeModel model = DeserializeModel(movementHandgaugeEditorModelStr);

            if(caller == (int)ClientCaller.CallerMovementHistory)
            {
                try
                {
                    this.SaveHandgaugeDataFromHistory(model);
                    base.AddSuccess("Saved Successful");
                }
                catch(Exception ex)
                {
                    string msg = "MovementHandgaugeController: Error saving hand gauge data. ";
                    FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + ex.Message, FMBusinessObjects.Constants.FMEventLogEntryType.Error));
                    base.OnError(new Exception(msg));
                    return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
                }
            }

            return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will save the hand gauge data to the movement history.
        /// </summary>
        /// <param name="model">The model used to update the record.</param>
        private void SaveHandgaugeDataFromHistory(MovementHandgaugeModel model)
        {
            var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(this.Security, this.Security.SiteGuid));
            var movementHistoryDo = FMChannelHelper.MakeCall<IMovementHistories, MovementHistoryDO>(x => x.GetMovementRecordByGuid(this.Security, model.MovementHangaugeGuid));

            if(movementHistoryDo == null || movementHistoryDo.MovementHistoryGuid == Guid.Empty)
            {
                string msg = "MovementHandgaugeController: Error, could not find Hand Gauge record.";
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMBusinessObjects.Constants.FMEventLogEntryType.Error));
                throw new Exception("Could not find Hand Gauge record.");
            }

            movementHistoryDo.StartLevelProduct             = this.ConvertLevelToDouble(model.StartLevel, (EngineeringUnit)model.StartLevelUnitsInt);
            movementHistoryDo.StartTemperatureProduct       = this.ConvertToDouble(model.StartTemperature);
            movementHistoryDo.StartDensityProductObserved   = this.ConvertToDouble(model.StartDensity);
            movementHistoryDo.StartDensityProductStandard   = this.ConvertToDouble(model.StartStdDensity);
            movementHistoryDo.StartTemperatureDensity       = this.ConvertToDouble(model.StartDensityTemperature);
            movementHistoryDo.StartTemperatureAmbient       = this.ConvertToDouble(model.StartAmbientTemperature);
            movementHistoryDo.StartLevelWater               = this.ConvertLevelToDouble(model.StartWaterLevel, (EngineeringUnit)model.StartLevelUnitsInt);
            movementHistoryDo.StartVolumeTotalObserved      = this.ConvertToDouble(model.StartVolumeTov);
            movementHistoryDo.StartVolumeGrossObserved      = this.ConvertToDouble(model.StartGrossVolume);
            movementHistoryDo.StartVolumeNetStandard        = this.ConvertToDouble(model.StartNetVolume);
            movementHistoryDo.StartVolumeWater              = this.ConvertToDouble(model.StartVolumeWater);
            movementHistoryDo.StartMassLiquid               = this.ConvertToDouble(model.StartMass);
            movementHistoryDo.StartVolumeCorrectionFactor   = this.ConvertToDouble(model.StartVcf);
            movementHistoryDo.StartTankShellCorrection      = this.ConvertToDouble(model.StartCtsh);

            movementHistoryDo.StartLevelProductTime             = this.ConvertStringToDateTimeOffset(model.StartLevelTime, site);
            movementHistoryDo.StartTemperatureProductTime       = this.ConvertStringToDateTimeOffset(model.StartTemperatureTime, site);
            movementHistoryDo.StartDensityProductObservedTime   = this.ConvertStringToDateTimeOffset(model.StartDensityTime, site);
            movementHistoryDo.StartDensityProductStandardTime   = this.ConvertStringToDateTimeOffset(model.StartStdDensityTime, site);
            movementHistoryDo.StartTemperatureDensityTime       = this.ConvertStringToDateTimeOffset(model.StartDensityTemperatureTime, site);
            movementHistoryDo.StartTemperatureAmbientTime       = this.ConvertStringToDateTimeOffset(model.StartAmbientTemperatureTime, site);
            movementHistoryDo.StartLevelWaterTime               = this.ConvertStringToDateTimeOffset(model.StartWaterLevelTime, site);

            movementHistoryDo.CloseoutLevelProduct              = this.ConvertLevelToDouble(model.EndLevel, (EngineeringUnit)model.EndLevelUnitsInt);
            movementHistoryDo.CloseoutTemperatureProduct        = this.ConvertToDouble(model.EndTemperature);
            movementHistoryDo.CloseoutDensityProductObserved    = this.ConvertToDouble(model.EndDensity);
            movementHistoryDo.CloseoutDensityProductStandard    = this.ConvertToDouble(model.EndStdDensity);
            movementHistoryDo.CloseoutTemperatureDensity        = this.ConvertToDouble(model.EndDensityTemperature);
            movementHistoryDo.CloseoutTemperatureAmbient        = this.ConvertToDouble(model.EndAmbientTemperature);
            movementHistoryDo.CloseoutLevelWater                = this.ConvertLevelToDouble(model.EndWaterLevel, (EngineeringUnit)model.EndLevelUnitsInt);
            movementHistoryDo.CloseoutVolumeTotalObserved       = this.ConvertToDouble(model.EndVolumeTov);
            movementHistoryDo.CloseoutVolumeGrossObserved       = this.ConvertToDouble(model.EndGrossVolume);
            movementHistoryDo.CloseoutVolumeNetStandard         = this.ConvertToDouble(model.EndNetVolume);
            movementHistoryDo.CloseoutVolumeWater               = this.ConvertToDouble(model.EndVolumeWater);
            movementHistoryDo.CloseoutMassLiquid                = this.ConvertToDouble(model.EndMass);
            movementHistoryDo.CloseoutVolumeCorrectionFactor    = this.ConvertToDouble(model.StartVcf);
            movementHistoryDo.CloseoutTankShellCorrection       = this.ConvertToDouble(model.EndCtsh);

            movementHistoryDo.CloseoutLevelProductTime              = this.ConvertStringToDateTimeOffset(model.EndLevelTime, site);
            movementHistoryDo.CloseoutTemperatureProductTime        = this.ConvertStringToDateTimeOffset(model.EndTemperatureTime, site);
            movementHistoryDo.CloseoutDensityProductObservedTime    = this.ConvertStringToDateTimeOffset(model.EndDensityTime, site);
            movementHistoryDo.CloseoutDensityProductStandardTime    = this.ConvertStringToDateTimeOffset(model.EndStdDensityTime, site);
            movementHistoryDo.CloseoutTemperatureDensityTime        = this.ConvertStringToDateTimeOffset(model.EndDensityTemperatureTime, site);
            movementHistoryDo.CloseoutTemperatureAmbientTime        = this.ConvertStringToDateTimeOffset(model.EndAmbientTemperatureTime, site);
            movementHistoryDo.CloseoutLevelWaterTime                = this.ConvertStringToDateTimeOffset(model.EndWaterLevelTime, site);

            movementHistoryDo.UnitsLevelProductIndex            = model.StartLevelUnitsInt;
            movementHistoryDo.UnitsTemperatureProductIndex      = model.StartTemperatureUnitsInt;
            movementHistoryDo.UnitsDensityProductObservedIndex  = model.StartDensityUnitsInt;
            movementHistoryDo.UnitsDensityProductStandardIndex  = model.StartStandardDensityUnitsInt;
            movementHistoryDo.UnitsTemperatureDensityIndex      = model.StartDensityTemperatureUnitsInt;
            movementHistoryDo.UnitsTemperatureAmbientIndex      = model.StartAmbientTemperatureUnitsInt;
            movementHistoryDo.UnitsMassIndex                    = model.StartMassUnitsInt;

            FMChannelHelper.MakeCall<IMovementHistories>(x => x.UpdateHandgaugeFromHistory(this.Security, movementHistoryDo, model.SaveToFinalRecord));
        }

        /// <summary>
        /// This method will convert a string into a double.
        /// </summary>
        /// <param name="inValue">The string value to convert.</param>
        /// <returns>Returns a double or null.</returns>
        private double? ConvertToDouble(string inValue)
        {
            if(string.IsNullOrEmpty(inValue))
            {
                return null;
            }

            if (double.TryParse(inValue, out double convertedValue) == false)
            {
                return null;
            }

            return convertedValue;
        }

        /// <summary>
        /// This method will convert the level to double.
        /// </summary>
        /// <param name="inValue">The value to convert.</param>
        /// <param name="levelUnit">The level units.</param>
        /// <returns>Return a double or null.</returns>
        private double? ConvertLevelToDouble(string inValue, EngineeringUnit levelUnit)
        {
            if (string.IsNullOrEmpty(inValue))
            {
                return null;
            }

            if(levelUnit != EngineeringUnit.FmlFtIn16Th && levelUnit != EngineeringUnit.FmlFtIn8Th)
            {
                return this.ConvertToDouble(inValue);
            }

            int denominator = levelUnit == EngineeringUnit.FmlFtIn16Th ? 16 : 8;
            double feet = 0.0;

            if(this.ConvertLevelToDecimal(denominator, inValue, ref feet) == false)
            {
                return null;
            }

            return feet;
        }

        /// <summary>
        /// This method will update the newly calculated start/end values based on the field that changed.
        /// </summary>
        /// <param name="fieldId">The field ID that changed</param>
        /// <param name="model">The model.</param>
        /// <param name="pointTagDictionary">The point tag dictionary.</param>
        /// <param name="site">The current site.</param>
        private void UpdateCalculatedValues(string fieldId, MovementHandgaugeModel model, Dictionary<Guid, PointTag> pointTagDictionary, StartEndType startEndType)
        {
            string changedTagId = this.GetTagIdFromFieldId(fieldId);
            var pointTagList = new List<PointTag>();

            int levelUnitsInt               = 0;
            string level                    = string.Empty;
            string temperature              = string.Empty;
            string density                  = string.Empty;
            string densityStd               = string.Empty;
            string densityTemperature       = string.Empty;
            string ambientTemperature       = string.Empty;
            string waterLevel               = string.Empty;
            int waterLevelUnitsInt          = 0;
            string volumeTov                = string.Empty;
            string grossVolume              = string.Empty;
            string netVolume                = string.Empty;
            string waterVolume              = string.Empty;
            string mass                     = string.Empty;
            string vcf                      = string.Empty;
            string tankShellCorrection      = string.Empty;

            if(startEndType == StartEndType.Start)
            {
                levelUnitsInt       = model.StartLevelUnitsInt;
                level               = model.StartLevel;
                temperature         = model.StartTemperature;
                densityStd          = model.StartStdDensity;
                densityTemperature  = model.StartDensityTemperature;
                ambientTemperature  = model.StartAmbientTemperature;
                waterLevel          = model.StartWaterLevel;
                waterLevelUnitsInt  = model.StartWaterLevelUnitsInt;

                density             = model.StartDensity;
                volumeTov           = model.StartVolumeTov;
                grossVolume         = model.StartGrossVolume;
                netVolume           = model.StartNetVolume;
                waterVolume         = model.StartVolumeWater;
                mass                = model.StartMass;
                vcf                 = model.StartVcf;
                tankShellCorrection = model.StartCtsh;
            }
            else
            {
                levelUnitsInt       = model.EndLevelUnitsInt;
                level               = model.EndLevel;
                temperature         = model.EndTemperature;
                densityStd          = model.EndStdDensity;
                densityTemperature  = model.EndDensityTemperature;
                ambientTemperature  = model.EndAmbientTemperature;
                waterLevel          = model.EndWaterLevel;
                waterLevelUnitsInt  = model.EndWaterLevelUnitsInt;

                density             = model.EndDensity;
                volumeTov           = model.EndVolumeTov;
                grossVolume         = model.EndGrossVolume;
                netVolume           = model.EndNetVolume;
                waterVolume         = model.EndVolumeWater;
                mass                = model.EndMass;
                vcf                 = model.EndVcf;
                tankShellCorrection = model.EndCtsh;
            }

            foreach (var tagData in pointTagDictionary)
            {
                switch (tagData.Value.ID)
                {
                    case TagLevelProduct:
                        double valueDouble = 0;
                        int denominator = (EngineeringUnit)levelUnitsInt == EngineeringUnit.FmlFtIn8Th ? 8 : 16;

                        if (this.ConvertLevelToDecimal(denominator, level, ref valueDouble))
                        {
                            tagData.Value.Value = valueDouble;
                            pointTagList.Add(tagData.Value);
                            if(tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
                        }
                        break;
                    case TagTemperatureProduct:
                        if (double.TryParse(temperature, out double convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                            if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
                        }
                        break;
                    case TagDensityProductStd:
                        if (double.TryParse(densityStd, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                            if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
                        }
                        break;
                    case TagTemperatureDensity:
                        if (double.TryParse(densityTemperature, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                            if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
                        }
                        break;
                    case TagTemperatureAmbient:
                        if (double.TryParse(ambientTemperature, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                            if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
                        }
                        break;
                    case TagLevelWater:
                        valueDouble = 0;
                        denominator = (EngineeringUnit)waterLevelUnitsInt == EngineeringUnit.FmlFtIn16Th ? 16 : 8;

                        if (this.ConvertLevelToDecimal(denominator, waterLevel, ref valueDouble))
                        {
                            tagData.Value.Value = valueDouble;
                            pointTagList.Add(tagData.Value);
                            if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
                        }
                        break;

                    // These values are only used for display and are not changed by the UI.
                    case TagDensityProductObserved:
                        if (double.TryParse(density, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                        }
                        break;
                    case TagVolumeTotalObserved:
                        if (double.TryParse(volumeTov, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                        }
                        break;
                    case TagVolumeGrossObserved:
                        if (double.TryParse(grossVolume, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                        }
                        break;
                    case TagVolumeNetStd:
                        if (double.TryParse(netVolume, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                        }
                        break;
                    case TagVolumeWater:
                        if (double.TryParse(waterVolume, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                        }
                        break;
                    case TagMassLiquid:
                        if (double.TryParse(mass, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                        }
                        break;
                    case TagVcf:
                        if (double.TryParse(vcf, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                        }
                        break;
                    case TagTankShellCorrection:
                        if (double.TryParse(tankShellCorrection, out convertedValue))
                        {
                            tagData.Value.Value = convertedValue;
                            pointTagList.Add(tagData.Value);
                        }
                        break;
                }
            }

            var calculatedPointTagList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(
                                                                    x => x.RunPointCalculatorX(this.Security, model.PointGuid, pointTagList));

            var calculatedList = new List<calculatorItems>();

            foreach (var pointTag in calculatedPointTagList)
            {
                var calculatorItem = new calculatorItems();
                calculatedList.Add(calculatorItem);

                string rawValue = "0";
                calculatorItem.numberDecimals = pointTag.DecimalPlaces;

                if(startEndType == StartEndType.Start)
                {
                    calculatorItem.startValue           = this.SetStartValue(pointTag, ref rawValue);
                    calculatorItem.startValueRaw        = rawValue;
                    calculatorItem.startSourceDateTime  = pointTag.SourceTimeStamp;
                }
                else
                {
                    calculatorItem.endValue             = this.SetStartValue(pointTag, ref rawValue);
                    calculatorItem.endValueRaw          = rawValue;
                    calculatorItem.endSourceDateTime    = pointTag.SourceTimeStamp;
                }

                calculatorItem.tagGuid      = pointTag.IdentityGuid;
                calculatorItem.tagName      = pointTag.ID;
                calculatorItem.editDisabled = 0; // this.isTagEditDisabled(pointTagData.Value);
                calculatorItem.unitsString  = EngineeringUnits.GetUnitAbbreviation(pointTag.Units);
                calculatorItem.units        = ((int)pointTag.Units).ToString();
                calculatorItem.dataType     = pointTag.ValueType.ToString();
                calculatorItem.UnitsType    = pointTag.EngineeringUnitsType;
                calculatorItem.maximumValue = pointTag.Maximum;
                calculatorItem.minimumValue = pointTag.Minimum;
            }

            this.UpdateModelFromPoints(ref model, ref calculatedList, startEndType);
        }

        /// <summary>
        /// This method will return the Tag ID for a given field ID.
        /// </summary>
        /// <param name="fieldId">The field ID filter</param>
        /// <returns>Returns a tag ID.</returns>
        private string GetTagIdFromFieldId(string fieldId)
        {
            switch(fieldId)
            {
                // Start fields
                case StartLevelField:
                    return TagLevelProduct;
                case StartTemperatureField:
                    return TagTemperatureProduct;
                case StartStdDensityField:
                    return TagDensityProductStd;
                case StartDensityTempField:
                    return TagTemperatureDensity;
                case StartAmbTempField:
                    return TagTemperatureAmbient;
                case StartWaterLevelField:
                    return TagLevelWater;
                case StartRefHeightField:
                    return string.Empty;

                // End Fields
                case EndLevelField:
                    return TagLevelProduct;
                case EndTemperatureField:
                    return TagTemperatureProduct;
                case EndStdDensityField:
                    return TagDensityProductStd;
                case EndDensityTempField:
                    return TagTemperatureDensity;
                case EndAmbTempField:
                    return TagTemperatureAmbient;
                case EndWaterLevelField:
                    return TagLevelWater;
                case EndRefHeightField:
                    return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// This method will set the number format info.
        /// </summary>
        /// <param name="model">The model that contains the numbering information.</param>
        private void SetNumberFormatting(MovementHandgaugeModel model)
        {
            this.numberFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = model.NumberDecimalSeparator,
                NumberGroupSeparator = model.NumberGroupSeparator,
                NumberGroupSizes = model.NumberGroupSizes
            };
        }

        /// <summary>
        /// This method will load the model with the point information based on the data from the
        /// point tags.
        /// </summary>
        /// <param name="model">The model to update.</param>
        /// <param name="calculatedList">The list of calculated values</param>
        /// <param name="startEndType">The calculated type.</param>
        private void UpdateModelFromPoints(ref MovementHandgaugeModel model, ref List<calculatorItems> calculatedList, StartEndType startEndType)
        {
            if(calculatedList != null && calculatedList.Count > 0)
            {
                foreach(calculatorItems item in calculatedList)
                {
                    if (startEndType == StartEndType.Start || startEndType == StartEndType.Both)
                    {
                        switch (item.tagName)
                        {
                            case TagLevelProduct:
                                model.StartLevel = item.startValue;
                                model.StartLevelUnits = item.unitsString;
                                model.StartLevelUnitsInt = int.Parse(item.units);
                                model.StartLevelTime = this.ConvertDateTimeToLocalTime(item.startSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagLevelWater:
                                model.StartWaterLevel = item.startValue;
                                model.StartWaterLevelUnits = item.unitsString;
                                model.StartWaterLevelUnitsInt = int.Parse(item.units);
                                model.StartWaterLevelTime = this.ConvertDateTimeToLocalTime(item.startSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagTemperatureProduct:
                                model.StartTemperature = item.startValue;
                                model.StartTemperatureUnits = item.unitsString;
                                model.StartTemperatureUnitsInt = int.Parse(item.units);
                                model.StartTemperatureTime = this.ConvertDateTimeToLocalTime(item.startSourceDateTime, model.TimePattern, model.ShortDatePattern);

                                break;
                            case TagTemperatureAmbient:
                                model.StartAmbientTemperature = item.startValue;
                                model.StartAmbientTemperatureUnits = item.unitsString;
                                model.StartAmbientTemperatureUnitsInt = int.Parse(item.units);
                                model.StartAmbientTemperatureTime = this.ConvertDateTimeToLocalTime(item.startSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagTemperatureDensity:
                                model.StartDensityTemperature = item.startValue;
                                model.StartDensityTemperatureUnits = item.unitsString;
                                model.StartDensityTemperatureUnitsInt = int.Parse(item.units);
                                model.StartDensityTemperatureTime = this.ConvertDateTimeToLocalTime(item.startSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagDensityProductStd:
                                model.StartStdDensity = item.startValue;
                                model.StartStandardDensityUnits = item.unitsString;
                                model.StartStandardDensityUnitsInt = int.Parse(item.units);
                                model.StartStdDensityTime = this.ConvertDateTimeToLocalTime(item.startSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagDensityProductObserved:
                                model.StartDensity = item.startValue;
                                model.StartDensityUnits = item.unitsString;
                                model.StartDensityUnitsInt = int.Parse(item.units);
                                model.StartDensityTime = this.ConvertDateTimeToLocalTime(item.startSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagVcf:
                                model.StartVcf = item.startValue;
                                break;
                            case TagTankShellCorrection:
                                model.StartCtsh = item.startValue;
                                break;
                            case TagVolumeWater:
                                model.StartVolumeWater = item.startValue;
                                model.StartWaterVolumeUnits = item.unitsString;
                                break;
                            case TagVolumeGrossObserved:
                                model.StartGrossVolume = item.startValue;
                                model.StartGrossVolumeUnits = item.unitsString;
                                break;
                            case TagVolumeNetStd:
                                model.StartNetVolume = item.startValue;
                                model.StartNetVolumeUnits = item.unitsString;
                                break;
                            case TagVolumeTotalObserved:
                                model.StartVolumeTov = item.startValue;
                                model.StartVolumeUnits = item.unitsString;
                                break;
                            case TagMassLiquid:
                                model.StartMass = item.startValue;
                                model.StartMassUnits = item.unitsString;
                                break;
                        }
                    }

                    if (startEndType == StartEndType.End || startEndType == StartEndType.Both)
                    {
                        switch (item.tagName)
                        {
                            case TagLevelProduct:
                                model.EndLevel = item.endValue;
                                model.EndLevelUnits = item.unitsString;
                                model.EndLevelUnitsInt = int.Parse(item.units);
                                model.EndLevelTime = this.ConvertDateTimeToLocalTime(item.endSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagLevelWater:
                                model.EndWaterLevel = item.endValue;
                                model.EndWaterLevelUnits = item.unitsString;
                                model.EndWaterLevelUnitsInt = int.Parse(item.units);
                                model.EndWaterLevelTime = this.ConvertDateTimeToLocalTime(item.endSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagTemperatureProduct:
                                model.EndTemperature = item.endValue;
                                model.EndTemperatureUnits = item.unitsString;
                                model.EndTemperatureUnitsInt = int.Parse(item.units);
                                model.EndTemperatureTime = this.ConvertDateTimeToLocalTime(item.endSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagTemperatureAmbient:
                                model.EndAmbientTemperature = item.endValue;
                                model.EndAmbientTemperatureUnits = item.unitsString;
                                model.EndAmbientTemperatureUnitsInt = int.Parse(item.units);
                                model.EndAmbientTemperatureTime = this.ConvertDateTimeToLocalTime(item.endSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagTemperatureDensity:
                                model.EndDensityTemperature = item.endValue;
                                model.EndDensityTemperatureUnits = item.unitsString;
                                model.EndDensityTemperatureUnitsInt = int.Parse(item.units);
                                model.EndDensityTemperatureTime = this.ConvertDateTimeToLocalTime(item.endSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagDensityProductStd:
                                model.EndStdDensity = item.endValue;
                                model.EndStandardDensityUnits = item.unitsString;
                                model.EndStandardDensityUnitsInt = int.Parse(item.units);
                                model.EndStdDensityTime = this.ConvertDateTimeToLocalTime(item.endSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagDensityProductObserved:
                                model.EndDensity = item.endValue;
                                model.EndDensityUnitsInt = int.Parse(item.units);
                                model.EndDensityUnits = item.unitsString;
                                model.EndDensityTime = this.ConvertDateTimeToLocalTime(item.endSourceDateTime, model.TimePattern, model.ShortDatePattern);
                                break;
                            case TagVcf:
                                model.EndVcf = item.endValue;
                                break;
                            case TagTankShellCorrection:
                                model.EndCtsh = item.endValue;
                                break;
                            case TagVolumeWater:
                                model.EndVolumeWater = item.endValue;
                                model.EndWaterVolumeUnits = item.unitsString;
                                break;
                            case TagVolumeGrossObserved:
                                model.EndGrossVolume = item.endValue;
                                model.EndGrossVolumeUnits = item.unitsString;
                                break;
                            case TagVolumeNetStd:
                                model.EndNetVolume = item.endValue;
                                model.EndNetVolumeUnits = item.unitsString;
                                break;
                            case TagVolumeTotalObserved:
                                model.EndVolumeTov = item.endValue;
                                model.EndVolumeUnits = item.unitsString;
                                break;
                            case TagMassLiquid:
                                model.EndMass = item.endValue;
                                model.EndMassUnits = item.unitsString;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method will retrieve the calculator information based on a point Guid.
        /// </summary>
        /// <param name="movementPointGuid"></param>
        /// <param name="model"></param>
        private List<calculatorItems> GetMovementPointInfoFromCalculator(Guid movementPointGuid)
        {
            var listOfCalculatorItems = new List<calculatorItems>();
            var pointTagGuids = new List<Guid>();

            // this is kinda of messed up but because of the changes wanted at the last minute there is no other way around this
            // I do not agree with the requested changes mainly because it changes this from a generic calculator
            // to a hard coded list. Sorry, this is not what I wanted
            foreach (var tagName in this.lookupTagsList)
            {
                var newCalculatorItem = new calculatorItems { tagName = tagName, tagGuid = null, editDisabled = 1 };
                listOfCalculatorItems.Add(newCalculatorItem);
            }

            Dictionary<Guid, PointTag> pointTagDictionary = null;

            try
            {
                pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(x => x.EnumerateByPointGuid(this.Security, movementPointGuid));
            }
            catch(Exception ex)
            {
                string msg = "MovementHandgaugeEditorController: Error retrieving Point Guid. " + ex.Message;
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMBusinessObjects.Constants.FMEventLogEntryType.Error));
            }

            if (pointTagDictionary == null)
            {
                return listOfCalculatorItems;
            }

            foreach (KeyValuePair<Guid, PointTag> pointTagData in pointTagDictionary)
            {
                foreach (calculatorItems calculatorItem in listOfCalculatorItems)
                {
                    if (calculatorItem.tagName == pointTagData.Value.ID)
                    {
                        pointTagGuids.Add(pointTagData.Value.IdentityGuid);

                        calculatorItem.tagGuid      = pointTagData.Value.IdentityGuid;
                        calculatorItem.editDisabled = 0; // this.isTagEditDisabled(pointTagData.Value);
                        calculatorItem.unitsString  = EngineeringUnits.GetUnitAbbreviation(pointTagData.Value.Units);
                        calculatorItem.units        = ((int)pointTagData.Value.Units).ToString();
                        calculatorItem.dataType     = pointTagData.Value.ValueType.ToString();
                        calculatorItem.UnitsType    = pointTagData.Value.EngineeringUnitsType;
                        calculatorItem.maximumValue = pointTagData.Value.Maximum;
                        calculatorItem.minimumValue = pointTagData.Value.Minimum;
                        break;
                    }
                }
            }

            // Retrieve the tag values
            List<PointTag> pointTags = null;

            try
            {
                pointTags = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(x => x.GetPointTagDataWithoutPointAccess(this.Security, pointTagGuids));
            }
            catch(Exception ex)
            {
                string msg = "MovementHandgaugeEditorController: Error retrieving Point Tags. " + ex.Message;
                FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMBusinessObjects.Constants.FMEventLogEntryType.Error));
            }

            if (pointTags == null)
            {
                return listOfCalculatorItems;
            }

            foreach (PointTag pointTag in pointTags)
            {
                foreach (calculatorItems calculatorItem in listOfCalculatorItems)
                {
                    if (calculatorItem.tagGuid == pointTag.PointTagGuid)
                    {
                        string rawValue = "0";
                        calculatorItem.numberDecimals = pointTag.DecimalPlaces;
                        calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                        calculatorItem.startValueRaw = rawValue;
                        calculatorItem.startSourceDateTime = pointTag.SourceTimeStamp;

                        // set the end equal to the start and the dif at 0 initialy
                        calculatorItem.endValue = calculatorItem.startValue;
                        calculatorItem.endValueRaw = calculatorItem.startValueRaw;
                        calculatorItem.endSourceDateTime = pointTag.SourceTimeStamp;
                        rawValue = "0";
                        calculatorItem.diffValue = string.Empty;
                        calculatorItem.diffValueRaw = rawValue;
                        calculatorItem.startValueRaw = this.GetRawValue(calculatorItem.startValue, pointTag.Units);
                    }
                }
            }

            return listOfCalculatorItems;
        }

        /// <summary>
        /// This method will retrieve the raw value.
        /// </summary>
        /// <param name="inValue">The starting value.</param>
        /// <param name="engineeringUnits">The engineering units</param>
        /// <returns>Returns the raw value as a string.</returns>
        private string GetRawValue(string inValue, EngineeringUnit engineeringUnits)
        {
            double ValueDouble = 0.0;

            if (engineeringUnits == EngineeringUnit.FmlFtIn16Th || engineeringUnits == EngineeringUnit.FmlFtIn8Th)
            {
                switch (engineeringUnits)
                {
                    case EngineeringUnit.FmlFtIn8Th:
                        if (this.ConvertLevelToDecimal(8, inValue, ref ValueDouble) == false)
                        {
                            return "00-00-00";
                        }

                        return ValueDouble.ToString();

                    case EngineeringUnit.FmlFtIn16Th:
                        if (this.ConvertLevelToDecimal(16, inValue, ref ValueDouble) == false)
                        {
                            return "00-00-00";
                        }

                        return ValueDouble.ToString();
                }
            }
            else
            {
                if (double.TryParse(inValue.ToString(), out ValueDouble) == false)
                {
                    ValueDouble = 0;
                }

                return ValueDouble.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// This method will set the starting tag values.
        /// </summary>
        /// <param name="pointTagData"></param>
        /// <param name="rawValue"></param>
        /// <returns>Returns the starting value.</returns>
        private string SetStartValue(PointTag pointTagData, ref string rawValue)
        {
            double archiveValueDouble;

            // tagdata.Status == StatusCodes.Bad
            if (pointTagData.Value == null)
            {
                archiveValueDouble = 0;
            }
            else if (double.TryParse(pointTagData.Value.ToString(), out archiveValueDouble) == false)
            {
                archiveValueDouble = 0;
            }

            rawValue = archiveValueDouble.ToString();

            // format based on engineering units
            switch (pointTagData.Units)
            {
                case EngineeringUnit.FmlFtIn8Th:
                    return PointManager.FormatValue(typeof(Double), EngineeringUnit.FmlFtIn8Th, this.numberFormatInfo, archiveValueDouble);
                case EngineeringUnit.FmlFtIn16Th:
                    return PointManager.FormatValue(typeof(Double), EngineeringUnit.FmlFtIn16Th, this.numberFormatInfo, archiveValueDouble);
                default:
                    return this.FormatDoubleToString(archiveValueDouble, pointTagData);
            }
        }

        /// <summary>
        /// This method will format a double to a string.
        /// </summary>
        /// <param name="archiveValueDouble">The value to format.</param>
        /// <param name="format"></param>
        /// <param name="pointTag"></param>
        /// <returns>Returns a double formatted to string.</returns>
        private string FormatDoubleToString(double archiveValueDouble, PointTag pointTag)
        {
            this.numberFormatInfo.NumberDecimalDigits = pointTag.DecimalPlaces;
            return PointManager.FormatValueFullPrecision(typeof(double), pointTag.Units, this.numberFormatInfo, archiveValueDouble);
        }

        /// <summary>
        /// This method will convert the level to a decimal value.
        /// </summary>
        /// <param name="denominator">It will either be 16th or 8th.</param>
        /// <param name="inValue">The level to convert.</param>
        /// <param name="feet">The conversion result.</param>
        /// <returns>Returns true if successful, otherwise false.</returns>
        private bool ConvertLevelToDecimal(int denominator, string levelValue, ref double feet)
        {
            if (string.IsNullOrEmpty(levelValue))
            {
                return false;
            }

            string trimmedValue = levelValue.Trim();

            if (trimmedValue.Length <= 0)
            {
                return false;
            }

            bool negativeValue = false;

            if (trimmedValue.IndexOf('-') == 0)
            {
                negativeValue = true;
            }

            int ft;
            int inches;
            int sixteens;

            if (negativeValue)
            {
                string[] parts = trimmedValue.Split('-');

                if (parts.Length != 4)
                {
                    return false;
                }

                ft = Convert.ToInt16(parts[1]);
                inches = Convert.ToInt16(parts[2]);
                sixteens = Convert.ToInt16(parts[3]);
            }
            else
            {
                string[] tokens = trimmedValue.Split('-');

                if (tokens.Length != 3)
                {
                    return false;
                }

                ft = Convert.ToInt16(tokens[0]);
                inches = Convert.ToInt16(tokens[1]);
                sixteens = Convert.ToInt16(tokens[2]);
            }

            double totalInches = inches + (sixteens / (double)denominator);
            feet = ft + (totalInches / 12.0);

            return true;
        }

        /// <summary>
        /// This method will convert the date time to the site's format to the site
        /// configured format for local time.
        /// </summary>
        /// <param name="dateTime">The database date/time.</param>
        /// <param name="timePattern">The site time pattern</param>
        /// <param name="shortDatePattern">The site date pattern</param>
        /// <returns>Return the date time as a string.</returns>
        private string ConvertDateTimeToLocalTime(DateTimeOffset? dateTime, string timePattern, string shortDatePattern)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            string localTimeStr = dateTime.Value.ToString(timePattern);
            string localDateStr = dateTime.Value.ToString(shortDatePattern);
            string localDateTimeStr = localDateStr + " " + localTimeStr;

            return localDateTimeStr;
        }

        /// <summary>
        /// This method will convert a date time string into an data time offset.
        /// </summary>
        /// <param name="dateTimeStr">The date time string to convert.</param>
        /// <param name="site">The site object.</param>
        /// <returns>Returns the date time offset.</returns>
        private DateTimeOffset ConvertStringToDateTimeOffset(string dateTimeStr, SiteClass site)
        {
            var defaultDateTime = DateTimeOffset.Now;

            if (string.IsNullOrEmpty(dateTimeStr))
            {
                return defaultDateTime;
            }

            if (dateTimeStr.Length >= 14)
            {
                string dateTimeFormat = site.ShortDatePattern + " " + site.TimePattern;
                var mainParts = dateTimeStr.Split(' ');

                if (mainParts.Length == 3)
                {
                    if (mainParts[2].Equals(site.PMSymbol))
                    {
                        mainParts[2] = "PM";
                    }

                    if (mainParts[2].Equals(site.AMSymbol))
                    {
                        mainParts[2] = "AM";
                    }
                }

                if (mainParts.Length >= 2)
                {
                    mainParts[0] = mainParts[0].Replace(site.DateSeparator, "/");
                    mainParts[1] = mainParts[1].Replace(site.TimeSeparator, ":");
                    string newDateStr = mainParts[0] + " " + mainParts[1];

                    if (mainParts.Length == 3)
                    {
                        newDateStr = newDateStr + " " + mainParts[2];
                    }

                    try
                    {
                        var newDateTime = DateTime.ParseExact(newDateStr, dateTimeFormat, CultureInfo.InvariantCulture);
                        return newDateTime;
                    }
                    catch (Exception)
                    {
                        return defaultDateTime;
                    }
                }
            }

            return defaultDateTime;
        }

        /// <summary>
        /// This method will get the site's number and date/time formats and set the 
        /// model.
        /// </summary>
        /// <param name="model">The model to be updated.</param>
        private void SetDateAndNumberFormats(MovementHandgaugeModel model)
        {
            var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

            model.NumberGroupSeparator      = site.NumberGroupSeparator;
            model.NumberDecimalSeparator    = site.NumberDecimalSeparator;
            model.NumberGroupSizes          = site.GetNumberGroupSizes();
            model.ShortDatePattern          = site.ShortDatePattern;
            model.TimePattern               = site.TimePattern;
            model.TimeZone                  = site.TimeZone;
        }

        /// <summary>
        /// This method will a list of calculated items and update them based on the movement history record.
        /// </summary>
        /// <param name="movementHistoryGuid">The movement history handgauge Guid.</param>
        /// <param name="startEndType">The start/end or both.</param>
        /// <returns>Returns a list of calculated items.</returns>
        private List<calculatorItems> GetHistoryCalculatedList (Guid movementHistoryGuid, StartEndType startEndType)
        {
            var movementHistoryRecord =
                        FMChannelHelper.MakeCall<IMovementHistories, MovementHistoryDO>(x => x.GetMovementRecordByGuid(this.Security, movementHistoryGuid));

            if (movementHistoryRecord == null || movementHistoryRecord.MovementHistoryGuid == Guid.Empty || movementHistoryRecord.PointGuid == Guid.Empty)
            {
                return new List<calculatorItems>();
            }

            // Get the calculator items based on the point guid.
            List<calculatorItems> listOfCalculatorItems = GetMovementPointInfoFromCalculator(movementHistoryRecord.PointGuid);
            
            if(listOfCalculatorItems == null || listOfCalculatorItems.Count == 0)
            {
                return new List<calculatorItems>();
            }

            // Populate the calculated items with the history record.
            foreach (calculatorItems calculatorItem in listOfCalculatorItems)
            {
                string rawValue = "0";
                var pointTag = new PointTag
                {
                    DecimalPlaces = calculatorItem.numberDecimals,
                    Units = (EngineeringUnit)int.Parse(calculatorItem.units)
                };

                if (startEndType == StartEndType.Start || startEndType == StartEndType.Both)
                {
                    switch (calculatorItem.tagName)
                    {
                        case TagLevelProduct:
                            pointTag.Value = movementHistoryRecord.StartLevelProduct;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagLevelWater:
                            pointTag.Value = movementHistoryRecord.StartLevelWater;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagTemperatureProduct:
                            pointTag.Value = movementHistoryRecord.StartTemperatureProduct;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagTemperatureAmbient:
                            pointTag.Value = movementHistoryRecord.StartTemperatureAmbient;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagTemperatureDensity:
                            pointTag.Value = movementHistoryRecord.StartTemperatureDensity;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagDensityProductStd:
                            pointTag.Value = movementHistoryRecord.StartDensityProductStandard;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagDensityProductObserved:
                            pointTag.Value = movementHistoryRecord.StartDensityProductObserved;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVcf:
                            pointTag.Value = movementHistoryRecord.StartVolumeCorrectionFactor;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagTankShellCorrection:
                            pointTag.Value = movementHistoryRecord.StartTankShellCorrection;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeWater:
                            pointTag.Value = movementHistoryRecord.StartVolumeWater;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeGrossObserved:
                            pointTag.Value = movementHistoryRecord.StartVolumeGrossObserved;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeNetStd:
                            pointTag.Value = movementHistoryRecord.StartVolumeNetStandard;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeRoofCorrection:
                            pointTag.Value = movementHistoryRecord.StartVolumeRoofCorrection;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeTotalObserved:
                            pointTag.Value = movementHistoryRecord.StartVolumeTotalObserved;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagMassLiquid:
                            pointTag.Value = movementHistoryRecord.StartMassLiquid;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                    }
                }

                if (startEndType == StartEndType.End || startEndType == StartEndType.Both)
                {
                    switch (calculatorItem.tagName)
                    {
                        case TagLevelProduct:
                            pointTag.Value = movementHistoryRecord.CloseoutLevelProduct;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagLevelWater:
                            pointTag.Value = movementHistoryRecord.CloseoutLevelWater;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagTemperatureProduct:
                            pointTag.Value = movementHistoryRecord.CloseoutTemperatureProduct;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagTemperatureAmbient:
                            pointTag.Value = movementHistoryRecord.CloseoutTemperatureAmbient;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagTemperatureDensity:
                            pointTag.Value = movementHistoryRecord.CloseoutTemperatureDensity;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagDensityProductStd:
                            pointTag.Value = movementHistoryRecord.CloseoutDensityProductStandard;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagDensityProductObserved:
                            pointTag.Value = movementHistoryRecord.CloseoutDensityProductObserved;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVcf:
                            pointTag.Value = movementHistoryRecord.CloseoutVolumeCorrectionFactor;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagTankShellCorrection:
                            pointTag.Value = movementHistoryRecord.CloseoutTankShellCorrection;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeWater:
                            pointTag.Value = movementHistoryRecord.CloseoutVolumeWater;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeGrossObserved:
                            pointTag.Value = movementHistoryRecord.CloseoutVolumeGrossObserved;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeNetStd:
                            pointTag.Value = movementHistoryRecord.CloseoutVolumeNetStandard;
                            calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeRoofCorrection:
                            pointTag.Value = movementHistoryRecord.CloseoutVolumeRoofCorrection;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagVolumeTotalObserved:
                            pointTag.Value = movementHistoryRecord.CloseoutVolumeTotalObserved;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                        case TagMassLiquid:
                            pointTag.Value = movementHistoryRecord.CloseoutMassLiquid;
                            calculatorItem.endValue = this.SetStartValue(pointTag, ref rawValue);
                            break;
                    }
                }
            }

            return listOfCalculatorItems;
        }
        #endregion
    }
}