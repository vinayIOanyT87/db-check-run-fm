namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
   using FMBusinessObjects.DataObjects.CodedVariables;
   using FMBusinessServices.ServiceClasses;
   using FMPointCommon;
    using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Newtonsoft.Json;

	using System;
	using System.Collections.Generic;
    using System.Globalization;
	using System.Web.Mvc;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	public class MovementHistoryNodeEditorController : FMBaseControllerEx
	{
		#region Data members
		const string ErrorMsgPrefix = "MovementHistoryNodeEditorView: ";
		public enum CallingTypes { Start, Closeout, None}
		private NumberFormatInfo numberFormatInfo = null;

		private const string LevelField			= "Level";
		private const string TemperatureField	= "Temperature";
		private const string DensityField		= "Density";
		private const string StdDensityField	= "StdDensity";
		private const string AmbTempField		= "AmbientTemperature";
		private const string WaterLevelField	= "WaterLevel";
		private const string GrossVolumeField	= "GrossVolume";
		private const string NetVolumeField		= "NetVolume";
		private const string MassField			= "Mass";
		private const string BswField			= "Bsw";

		private const string TagLevelProduct			= "Level Product";
		private const string TagLevelWater				= "Level Water";
		private const string TagTemperatureProduct		= "Temperature Product";
		private const string TagTemperatureAmbient		= "Temperature Ambient";
		private const string TagDensityProductStd		= "Density Product Standard";
		private const string TagDensityProductObserved	= "Density Product Observed";
		private const string TagVolumeGrossObserved		= "Volume Gross Observed";
		private const string TagVolumeNetStd			= "Volume Net Standard";
		private const string TagMassLiquid				= "Mass Liquid";
		private const string TagPercentBsw				= "Percent BSW";

		public List<string> lookupTagsList = new List<string>
		{
			TagLevelProduct,
			TagLevelWater,
			TagTemperatureProduct,
			TagTemperatureAmbient,
			TagDensityProductStd,
			TagDensityProductObserved,
			TagVolumeGrossObserved,
			TagVolumeNetStd,
			TagMassLiquid,
			TagPercentBsw
		};
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementHistoryNodeEditorController()
		{
		}
		#endregion

		#region Public static methods
		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="model">The model to serialize</param>
		/// <returns>Returns a string of the model.</returns>
		[NonAction]
		public static string SerializeModel(MovementHistoryNodeEditorModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="modelStr">The model to serialize</param>
		/// <returns>Returns the movement user data editor model.</returns>
		[NonAction]
		public static MovementHistoryNodeEditorModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var model = JsonConvert.DeserializeObject<MovementHistoryNodeEditorModel>(modelStr, jsonSerializerSettings);
			return model;
		}
		#endregion


		#region Public methods
		/// <summary>
		/// This method retrieves the movement user data editor model based on the movement point GUID.
		/// </summary>
		/// <param name="movementPointGuid">The movement point GUID</param>
		/// <returns>Returns the Movement User Data Editor model.</returns>
		[HttpGet]
		public ActionResult MovementHistoryNodeEditor(Guid movementHistoryGuid, CallingTypes callingType)
		{		
			try
			{
				var model = new MovementHistoryNodeEditorModel { MovementHistoryGuid = movementHistoryGuid };
				this.SetDateAndNumberFormats(model);
				this.SetNumberFormatting(model);
				model.CallingTypes = (int)callingType;
		
				// Get the movement history calculated list.
				List<calculatorItems> calculatedList = this.GetHistoryCalculatedList(model, callingType);
				this.UpdateModelFromPoints(ref model, ref calculatedList);

				// Set the movement name and node ID in the model.
				this.GetMovementNameAndNodeID(movementHistoryGuid, out string name, out string nodeId);
				model.PointId = name;
				model.NodeId = nodeId;

				bool isEnterpriseFlag = this.IsEnterprise();
				bool isTankTransferFlag = this.IsTankTransfer(model.MovementPointGuid);

				// We do not want to perform point calculation if the system is an enterprise system
				// or it is not a tank transfer.
				if(isEnterpriseFlag || isTankTransferFlag == false)
            {
					model.IgnoreCalculation = true;
            }

				model.HasModifyRights = this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_HISTORY);

            Guid movementNodePointGuid = FMChannelHelper.MakeCall<IPoints, Guid>(x => x.GetIdentityGuid(this.Security, model.NodeId));
            Guid transferStatusGuid = FMChannelHelper.MakeCall<IPointTags, Guid>(x => x.GetIdentityGuid(this.Security, "Transfer Status", movementNodePointGuid));
            PointTag transferStatusTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, transferStatusGuid));
            if (transferStatusTag != null)
            {
               model.TransferStatus = (TransferStatuses)transferStatusTag.Value;

            }
            return base.PartialViewWithErrorMessages("MovementHistoryNodeEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				string msgBasic = this.GetTranslatedText("Movement|Error Getting Movement History Data.");
				string msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				base.OnError(new Exception(msgBasic));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method will save the data.
		/// </summary>
		/// <param name="modelStr">The model string.</param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult SaveMovementHistoryNodeData(string movementHistoryNodeEditorModelStr)
		{
			MovementHistoryNodeEditorModel model = DeserializeModel(movementHistoryNodeEditorModelStr);
	
			try
			{
				this.SaveMovementHistoryNodeDataHelper(model);
				base.AddSuccess("Saved Successful");
			}
			catch (Exception ex)
			{
				string msg = ErrorMsgPrefix + "Error saving hand gauge data. ";
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + ex.Message, FMEventLogEntryType.Error));
				base.OnError(new Exception(msg));
				return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// This method will call the legacy system to recalculate the handgauge data based on
		/// the field change.
		/// </summary>
		/// <param name="fieldId">The field ID of the field that has a change</param>
		/// <param name="modelStr">The model string to update on return.</param>
		/// <returns>Return the handgauge model</returns>
		public ActionResult CalculateMovementHistoryNodeData(string fieldId, string modelStr)
		{
			MovementHistoryNodeEditorModel model = DeserializeModel(modelStr);
			this.SetDateAndNumberFormats(model);
			this.SetNumberFormatting(model);

			if(model.IgnoreCalculation)
            {
				return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
			}

			if (string.IsNullOrEmpty(fieldId))
			{
				return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
			}

			Dictionary<Guid, PointTag> pointTagDictionary = null;

			try
			{
				pointTagDictionary = FMChannelHelper.MakeCall<IPointTags, Dictionary<Guid, PointTag>>(
															x => x.EnumerateByPointGuid(this.Security, model.MovementPointGuid));
			}
			catch (Exception ex)
			{
				string msgBasic = this.GetTranslatedText("Movement|Error Getting Movement History Data.");
				string msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

				base.OnError(new Exception(msgBasic));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			var pointTagList = new List<PointTag>();

			try
			{
				this.UpdateCalculatedValues(fieldId, model, pointTagDictionary);
				return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				this.OnError(e);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}


      [HttpPost]
      public ActionResult GetArchivedMovementHistoryNodeData(string MovementHistoryNodeEditorModelStr, string dateTime)
      {

         string msgBasic = string.Empty;
         string msgEventLog = string.Empty;

         if (string.IsNullOrEmpty(MovementHistoryNodeEditorModelStr))
         {
            msgBasic = this.GetTranslatedText("Error, movement node start data editor model is empty.");
            msgEventLog = ErrorMsgPrefix + msgBasic;
            FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

            OnError(new Exception(msgBasic));
            return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }

         MovementHistoryNodeEditorModel model = null;

         try
         {
            var jsonSerializerSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore };

            model = JsonConvert.DeserializeObject<MovementHistoryNodeEditorModel>(MovementHistoryNodeEditorModelStr, jsonSerializerSettings);
         }
         catch (Exception ex)
         {
            msgBasic = this.GetTranslatedText("Error deserializing the model string.");
            msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
            FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

            OnError(new Exception(msgBasic));
            return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
         }

			try 
			{
				if (model != null && DateTimeOffset.TryParse(dateTime, out DateTimeOffset dt))
				{
					SetNumberFormatting(model);

					Guid movementNodePointGuid = FMChannelHelper.MakeCall<IPoints, Guid>(x => x.GetIdentityGuid(this.Security, model.NodeId));

					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry($"{dt}", FMEventLogEntryType.Warning));
					List<calculatorItems> listOfTags = new List<calculatorItems>();
					foreach (string archivedTagName in lookupTagsList)
					{

						Guid ptGuid = FMChannelHelper.MakeCall<IPointTags, Guid>(x => x.GetIdentityGuid(this.Security, archivedTagName, movementNodePointGuid));
						PointTag pt = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, ptGuid));
						if (pt != null)
						{
							PointValue pv = new PointValue(pt);
							GetArchivedValueAndUnit(dt, pv);
							double val = pv?.Value is null ? 0 : (double)pv.Value;
							string valStr = $"{val}";
							pt.Value = val;
							pt.Units = pv?.Units ?? EngineeringUnit.FmuNone;
							//	pt.DecimalPlaces = pv?.DecimalPlaces ?? 0;
                     int unit = (int)pt.Units;

							calculatorItems item = new calculatorItems()
							{
								tagName = archivedTagName,
								startValue = this.SetStartValue(pt, ref valStr, false),//do not use full precision but one provided with point tag
								startValueRaw = valStr,
								numberDecimals = pt.DecimalPlaces,
								units = $"{unit}",
								unitsString = EngineeringUnits.GetUnitAbbreviation(pt.Units)
							};
							listOfTags.Add(item);

						}
					}
					UpdateModelFromPoints(ref model, ref listOfTags);


					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry($"datetime={dt}  model.Temperature={model.TemperatureStr}  model.Level={model.LevelStr}", FMEventLogEntryType.Warning));
					return JsonWithErrorMessages(SerializeModel(model), JsonRequestBehavior.AllowGet);
				}

         }
         catch (Exception ex)
         {
            string msgBasic2 = this.GetTranslatedText("Error retrieving archived movement history node data.");
            string msgEventLog2 = ErrorMsgPrefix + msgBasic2 + " " + ex.Message;
            FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

            OnError(new Exception(msgBasic2));
         }

         return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);

      }

      #endregion

      #region Private methods
      void GetArchivedValueAndUnit(DateTimeOffset dateTime, PointValue pv)
      {
         DateTimeOffset end = dateTime;
         DateTimeOffset start = end.AddHours(-(end.Hour + 1));

         Guid pointTagGuid = pv.PointValueIdentifier.IdentityGuid;

         if (pointTagGuid != Guid.Empty)
         {
            SimpleArchiveDataElement res = GetArchivedTagData(pointTagGuid, start, end);
            if (res.Value != null)
            {
               pv.ValueTypeString = res.DataType;
               pv.Units = (EngineeringUnit)res.EngineeringUnitsIndex;
               if (pv.ValueTypeString != null)
               {
                  switch (pv.ValueTypeString)
                  {
                     case "System.Boolean":
                        pv.Value = Convert.ToBoolean(res.Value);
                        break;

                     case "System.Int16":
                        pv.Value = Convert.ToInt16(res.Value);
                        break;

                     case "System.UInt16":
                        pv.Value = Convert.ToUInt16(res.Value);
                        break;

                     case "System.Int32":
                        pv.Value = Convert.ToInt32(res.Value);
                        break;

                     case "System.UInt32":
                        pv.Value = Convert.ToUInt32(res.Value);
                        break;
                     case "System.Single":

                        pv.Value = Convert.ToSingle(res.Value);

                        break;

                     case "System.Double":
                        pv.Value = Convert.ToDouble(res.Value);
                        break;

                     case "System.String":
                        pv.Value = Convert.ToString(res.Value);
                        break;

                     case "System.DateTimeOffset":
                        pv.Value = new DateTimeOffset(Convert.ToDateTime(res.Value));

                        break;

                     case "System.DateTime":
                        pv.Value = Convert.ToDateTime(res.Value);
                        break;

                     case "System.TimeSpan":
                        pv.Value = new TimeSpan(Convert.ToInt64(res.Value));
                        break;
                     case "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode":
                        pv.Value = (FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode)Convert.ToInt32(res.Value);
                        break;

                     default:
                        break;
                  }

               }

            }
         }

      }
      private SimpleArchiveDataElement GetArchivedTagData(Guid tagGuid, DateTimeOffset start, DateTimeOffset end)
      {
         List<Guid> tagGuids = new List<Guid>();

         tagGuids.Add(tagGuid);

         List<SimpleArchiveDataElement> archivedData = FMChannelHelper.MakeCall<IPointTagArchive, List<SimpleArchiveDataElement>>(x => x.GetArchiveDataValues(Security, tagGuids, start, end));
         if (archivedData.Count > 0)
         {
            return archivedData[0];
         }

         return null;

      }

      /// <summary>
      /// This method will check for whether the application is enterprise.
      /// </summary>
      /// <returns>Returns true if enterprise, otherwise returns false.</returns>
      private bool IsEnterprise()
        {
			var configDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, "IsEnterprise"));

			if(configDo == null || string.IsNullOrEmpty(configDo.SettingValue))
            {
				return false;
            }

			if(int.TryParse(configDo.SettingValue, out int outValue) == false)
            {
				return false;
            }

			if( outValue == 1)
            {
				return true;
            }

			return false;
        }

		/// <summary>
		/// This method will determine if the movement node is a tank transfer template.
		/// </summary>
		/// <param name="pointGuid">The movement node point Guid to search.</param>
		/// <returns>Returns true if a tank transfer, otherwise false.</returns>
		private bool IsTankTransfer(Guid pointGuid)
        {
			if(pointGuid == null || pointGuid == Guid.Empty)
            {
				return false;
            }

			var movementPoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, pointGuid, false));

			if(movementPoint == null || movementPoint.IdentityGuid == Guid.Empty)
            {
				return false;
            }

			var pointTemplateGuid = FMChannelHelper.MakeCall<IPointTemplates, Guid>(x => x.GetIdentityGuid(this.Security, "Standard Tank"));

			if(pointTemplateGuid == null || pointTemplateGuid == Guid.Empty)
            {
				Guid currentSiteGuid = this.Security.SiteGuid;
				this.Security.SiteGuid = Guids.SiteAdminGuid;

				pointTemplateGuid = FMChannelHelper.MakeCall<IPointTemplates, Guid>(x => x.GetIdentityGuid(this.Security, "Standard Tank"));
				this.Security.SiteGuid = currentSiteGuid;

				if (pointTemplateGuid == null || pointTemplateGuid == Guid.Empty)
				{
					return false;
				}
            }

			if(movementPoint.PointTemplateGuid == pointTemplateGuid)
            {
				return true;
            }

			return false;
        }

		/// <summary>
		/// This method will save the node start/closeout data to the movement history.
		/// </summary>
		/// <param name="model">The model used to update the record.</param>
		private void SaveMovementHistoryNodeDataHelper(MovementHistoryNodeEditorModel model)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(this.Security, this.Security.SiteGuid));
			var movementHistoryDo = FMChannelHelper.MakeCall<IMovementHistories, MovementHistoryDO>(x => x.GetMovementRecordByGuid(this.Security, model.MovementHistoryGuid));

			if (movementHistoryDo == null || movementHistoryDo.MovementHistoryGuid == Guid.Empty)
			{
				string msg = ErrorMsgPrefix + "Error, could not find Hand Gauge record.";
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMEventLogEntryType.Error));
				throw new Exception("Could not find movement history final record.");
			}

			var callingType = (CallingTypes)model.CallingTypes;

			if (model.ArchiveDataMode)
			{
            if (callingType == CallingTypes.Start)
            {
               movementHistoryDo.StartLevelProduct = this.ConvertToDouble(model.LevelRawStr);
               movementHistoryDo.StartTemperatureProduct = this.ConvertToDouble(model.TemperatureRawStr);
               movementHistoryDo.StartDensityProductObserved = this.ConvertToDouble(model.DensityRawStr);
               movementHistoryDo.StartDensityProductStandard = this.ConvertToDouble(model.StdDensityRawStr);
               movementHistoryDo.StartTemperatureAmbient = this.ConvertToDouble(model.AmbientTemperatureRawStr);
               movementHistoryDo.StartLevelWater = this.ConvertToDouble(model.WaterLevelRawStr);
               movementHistoryDo.StartVolumeGrossObserved = this.ConvertToDouble(model.GrossVolumeRawStr);
               movementHistoryDo.StartVolumeNetStandard = this.ConvertToDouble(model.NetVolumeRawStr);
               movementHistoryDo.StartMassLiquid = this.ConvertToDouble(model.MassRawStr);
               movementHistoryDo.StartPercentBsw = this.ConvertToDouble(model.BswRawStr);
               movementHistoryDo.StartTime = this.ConvertStringToDateTimeOffset(model.StartOrClosoutTime, site);
            }

            if (callingType == CallingTypes.Closeout)
            {
               movementHistoryDo.CloseoutLevelProduct = this.ConvertToDouble(model.LevelRawStr);
               movementHistoryDo.CloseoutTemperatureProduct = this.ConvertToDouble(model.TemperatureRawStr);
               movementHistoryDo.CloseoutDensityProductObserved = this.ConvertToDouble(model.DensityRawStr);
               movementHistoryDo.CloseoutDensityProductStandard = this.ConvertToDouble(model.StdDensityRawStr);
               movementHistoryDo.CloseoutTemperatureAmbient = this.ConvertToDouble(model.AmbientTemperatureRawStr);
               movementHistoryDo.CloseoutLevelWater = this.ConvertToDouble(model.WaterLevelRawStr);
               movementHistoryDo.CloseoutVolumeGrossObserved = this.ConvertToDouble(model.GrossVolumeRawStr);
               movementHistoryDo.CloseoutVolumeNetStandard = this.ConvertToDouble(model.NetVolumeRawStr);
               movementHistoryDo.CloseoutMassLiquid = this.ConvertToDouble(model.MassRawStr);
               movementHistoryDo.CloseoutPercentBsw = this.ConvertToDouble(model.BswRawStr);
               movementHistoryDo.CloseoutTime = this.ConvertStringToDateTimeOffset(model.StartOrClosoutTime, site);
            }
         }
         else
			{
				if (callingType == CallingTypes.Start)
				{
					movementHistoryDo.StartLevelProduct = this.ConvertLevelToDouble(model.LevelStr, model.LevelUnits);
					movementHistoryDo.StartTemperatureProduct = this.ConvertToDouble(model.TemperatureStr);
					movementHistoryDo.StartDensityProductObserved = this.ConvertToDouble(model.DensityStr);
					movementHistoryDo.StartDensityProductStandard = this.ConvertToDouble(model.StdDensityStr);
					movementHistoryDo.StartTemperatureAmbient = this.ConvertToDouble(model.AmbientTemperatureStr);
					movementHistoryDo.StartLevelWater = this.ConvertLevelToDouble(model.WaterLevelStr, model.LevelUnits);
					movementHistoryDo.StartVolumeGrossObserved = this.ConvertToDouble(model.GrossVolumeStr);
					movementHistoryDo.StartVolumeNetStandard = this.ConvertToDouble(model.NetVolumeStr);
					movementHistoryDo.StartMassLiquid = this.ConvertToDouble(model.MassStr);
					movementHistoryDo.StartPercentBsw = this.ConvertToDouble(model.BswStr);
					movementHistoryDo.StartTime = this.ConvertStringToDateTimeOffset(model.StartOrClosoutTime, site);
				}

				if (callingType == CallingTypes.Closeout)
				{
					movementHistoryDo.CloseoutLevelProduct = this.ConvertLevelToDouble(model.LevelStr, model.LevelUnits);
					movementHistoryDo.CloseoutTemperatureProduct = this.ConvertToDouble(model.TemperatureStr);
					movementHistoryDo.CloseoutDensityProductObserved = this.ConvertToDouble(model.DensityStr);
					movementHistoryDo.CloseoutDensityProductStandard = this.ConvertToDouble(model.StdDensityStr);
					movementHistoryDo.CloseoutTemperatureAmbient = this.ConvertToDouble(model.AmbientTemperatureStr);
					movementHistoryDo.CloseoutLevelWater = this.ConvertLevelToDouble(model.WaterLevelStr, model.LevelUnits);
					movementHistoryDo.CloseoutVolumeGrossObserved = this.ConvertToDouble(model.GrossVolumeStr);
					movementHistoryDo.CloseoutVolumeNetStandard = this.ConvertToDouble(model.NetVolumeStr);
					movementHistoryDo.CloseoutMassLiquid = this.ConvertToDouble(model.MassStr);
					movementHistoryDo.CloseoutPercentBsw = this.ConvertToDouble(model.BswStr);
					movementHistoryDo.CloseoutTime = this.ConvertStringToDateTimeOffset(model.StartOrClosoutTime, site);
				}
			}

			FMChannelHelper.MakeCall<IMovementHistories>(x => x.UpdateNodeDataToFinalRecord(this.Security, movementHistoryDo));
		}

		/// <summary>
		/// This method will load the model with the point information based on the data from the
		/// point tags.
		/// </summary>
		/// <param name="model">The model to update.</param>
		/// <param name="calculatedList">The list of calculated values</param>
		/// <param name="startEndType">The calculated type.</param>
		private void UpdateModelFromPoints(ref MovementHistoryNodeEditorModel model, ref List<calculatorItems> calculatedList)
		{
			if (calculatedList != null && calculatedList.Count > 0)
			{
				foreach (calculatorItems item in calculatedList)
				{
					switch (item.tagName)
					{
						case TagLevelProduct:
                     model.LevelStr = item.startValue;
                     model.LevelRawStr = item.startValueRaw;
                     model.LevelUnitsStr = item.unitsString;
							model.LevelUnits = item.units == null ? EngineeringUnit.FmlFtIn16Th : (EngineeringUnit)int.Parse(item.units);
							model.LevelPrecision = item.numberDecimals;
							break;
						case TagLevelWater:
                     model.WaterLevelStr = item.startValue;
                     model.WaterLevelRawStr = item.startValueRaw;
                     model.LevelUnitsStr = item.unitsString;
							model.LevelUnits = item.units == null ? EngineeringUnit.FmlFtIn16Th : (EngineeringUnit)int.Parse(item.units);
                     model.LevelPrecision = item.numberDecimals;
                     break;
						case TagTemperatureProduct:
                     model.TemperatureStr = item.startValue;
                     model.TemperatureRawStr = item.startValueRaw;
                     model.TemperatureUnitsStr = item.unitsString;
							model.TemperatureUnits = item.units == null ? EngineeringUnit.FmtDegF : (EngineeringUnit)int.Parse(item.units);
                     model.TemperaturePrecision = item.numberDecimals;
                     break;
						case TagTemperatureAmbient:
                     model.AmbientTemperatureStr = item.startValue;
                     model.AmbientTemperatureRawStr = item.startValueRaw;
                     model.AmbientTemperatureUnitsStr = item.unitsString;
							model.AmbientTemperatureUnits = item.units == null ? EngineeringUnit.FmtDegF : (EngineeringUnit)int.Parse(item.units);
                     model.AmbientTemperaturePrecision = item.numberDecimals;
                     break;
						case TagDensityProductStd:
                     model.StdDensityStr = item.startValue;
                     model.StdDensityRawStr = item.startValueRaw;
                     model.StdDensityUnitsStr = item.unitsString;
							model.StdDensityUnits = item.units == null ? EngineeringUnit.FmdLbFt3 : (EngineeringUnit)int.Parse(item.units);
                     model.StdDensityPrecision = item.numberDecimals;
                     break;
						case TagDensityProductObserved:
                     model.DensityStr = item.startValue;
                     model.DensityRawStr = item.startValueRaw;
                     model.DensityUnitsStr = item.unitsString;
							model.DensityUnits = item.units == null ? EngineeringUnit.FmdLbFt3 : (EngineeringUnit)int.Parse(item.units);
                     model.DensityPrecision = item.numberDecimals;
                     break;
						case TagVolumeGrossObserved:
                     model.GrossVolumeStr = item.startValue;
                     model.GrossVolumeRawStr = item.startValueRaw;
                     model.GrossVolumeUnitsStr = item.unitsString;
							model.GrossVolumeUnits = item.units == null ? EngineeringUnit.FmvUsGal : (EngineeringUnit)int.Parse(item.units);
                     model.GrossVolumePrecision = item.numberDecimals;
                     break;
						case TagVolumeNetStd:
                     model.NetVolumeStr = item.startValue;
                     model.NetVolumeRawStr = item.startValueRaw;
                     model.NetVolumeUnitsStr = item.unitsString;
							model.NetVolumeUnits = item.units == null ? EngineeringUnit.FmvUsGal : (EngineeringUnit)int.Parse(item.units);
                     model.NetVolumePrecision = item.numberDecimals;
                     break;
						case TagMassLiquid:
                     model.MassStr = item.startValue;
                     model.MassRawStr = item.startValueRaw;
                     model.MassUnitsStr = item.unitsString;
							model.MassUnits = item.units == null ? EngineeringUnit.FmmLb : (EngineeringUnit)int.Parse(item.units);
                     model.MassPrecision = item.numberDecimals;
                     break;
						case TagPercentBsw:
							model.BswStr = item.startValue;
							break;
					}
				}
			}
		}

		/// <summary>
		/// This method will return the movement name and node ID for the given history record.
		/// </summary>
		/// <param name="movementHistoryGuid">The movement history Guid to retrieve</param>
		/// <param name="name">Return the movement name.</param>
		/// <param name="nodeId">Return the node ID.</param>
		private void GetMovementNameAndNodeID(Guid movementHistoryGuid, out string name, out string nodeId)
        {
			var movementHistoryRecord =
						FMChannelHelper.MakeCall<IMovementHistories, MovementHistoryDO>(x => x.GetMovementRecordByGuid(this.Security, movementHistoryGuid));

			if (movementHistoryRecord == null || movementHistoryRecord.MovementHistoryGuid == Guid.Empty)
			{
				name = string.Empty;
				nodeId = string.Empty;
				return;
			}

			name = movementHistoryRecord.Name;
			nodeId = movementHistoryRecord.Node;
		}

		/// <summary>
		/// This method will a list of calculated items and update them based on the movement history record.
		/// </summary>
		/// <param name="movementHistoryGuid">The movement history handgauge Guid.</param>
		/// <param name="callingType">The start/closeout</param>
		/// <returns>Returns a list of calculated items.</returns>
		private List<calculatorItems> GetHistoryCalculatedList(MovementHistoryNodeEditorModel model, CallingTypes callingType)
		{
			var movementHistoryRecord =
						FMChannelHelper.MakeCall<IMovementHistories, MovementHistoryDO>(x => x.GetMovementRecordByGuid(this.Security, model.MovementHistoryGuid));

			if (movementHistoryRecord == null || movementHistoryRecord.MovementHistoryGuid == Guid.Empty || movementHistoryRecord.PointGuid == Guid.Empty)
			{
				return new List<calculatorItems>();
			}

			model.MovementPointGuid = movementHistoryRecord.PointGuid;
			model.ParentGuid		= movementHistoryRecord.ParentGuid;
			model.RootParentGuid	= movementHistoryRecord.RootParentGuid;

			if (callingType == CallingTypes.Start)
            {
				model.StartOrClosoutTime = this.ConvertDateTimeToLocalTime(movementHistoryRecord.StartTime, model.TimePattern, model.ShortDatePattern);
			}

			if (callingType == CallingTypes.Closeout)
			{
				model.StartOrClosoutTime = this.ConvertDateTimeToLocalTime(movementHistoryRecord.CloseoutTime, model.TimePattern, model.ShortDatePattern);
			}

			// Get the calculator items based on the point guid.
			List<calculatorItems> listOfCalculatorItems = this.GetMovementPointInfoFromCalculator(movementHistoryRecord.PointGuid);

			if (listOfCalculatorItems == null || listOfCalculatorItems.Count == 0)
			{
				return new List<calculatorItems>();
			}

			// Populate the calculated items with the history record.
			foreach (calculatorItems calculatorItem in listOfCalculatorItems)
			{
				string rawValue = "0";
				var pointTag = new PointTag
				{
					DecimalPlaces = calculatorItem.numberDecimals
				};

				if(calculatorItem.units != null)
                {
					pointTag.Units = (EngineeringUnit)int.Parse(calculatorItem.units);
				}

				if (callingType == CallingTypes.Start)
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
						case TagDensityProductStd:
							pointTag.Value = movementHistoryRecord.StartDensityProductStandard;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagDensityProductObserved:
							pointTag.Value = movementHistoryRecord.StartDensityProductObserved;
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
						case TagMassLiquid:
							pointTag.Value = movementHistoryRecord.StartMassLiquid;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagPercentBsw:
							pointTag.Value = movementHistoryRecord.StartPercentBsw;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
					}
				}

				if (callingType == CallingTypes.Closeout)
				{
					switch (calculatorItem.tagName)
					{
						case TagLevelProduct:
							pointTag.Value = movementHistoryRecord.CloseoutLevelProduct;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagLevelWater:
							pointTag.Value = movementHistoryRecord.CloseoutLevelWater;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagTemperatureProduct:
							pointTag.Value = movementHistoryRecord.CloseoutTemperatureProduct;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagTemperatureAmbient:
							pointTag.Value = movementHistoryRecord.CloseoutTemperatureAmbient;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagDensityProductStd:
							pointTag.Value = movementHistoryRecord.CloseoutDensityProductStandard;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagDensityProductObserved:
							pointTag.Value = movementHistoryRecord.CloseoutDensityProductObserved;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagVolumeGrossObserved:
							pointTag.Value = movementHistoryRecord.CloseoutVolumeGrossObserved;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagVolumeNetStd:
							pointTag.Value = movementHistoryRecord.CloseoutVolumeNetStandard;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagMassLiquid:
							pointTag.Value = movementHistoryRecord.CloseoutMassLiquid;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
						case TagPercentBsw:
							pointTag.Value = movementHistoryRecord.CloseoutPercentBsw;
							calculatorItem.startValue = this.SetStartValue(pointTag, ref rawValue);
							break;
					}
				}
			}

			return listOfCalculatorItems;
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
			catch (Exception ex)
			{
				string msg = ErrorMsgPrefix + "Error retrieving Point Guid. " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMEventLogEntryType.Error));
			}

			if (pointTagDictionary == null || pointTagDictionary.Count == 0)
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

						calculatorItem.tagGuid		= pointTagData.Value.IdentityGuid;
						calculatorItem.editDisabled = 0; // this.isTagEditDisabled(pointTagData.Value);
						calculatorItem.unitsString	= EngineeringUnits.GetUnitAbbreviation(pointTagData.Value.Units);
						calculatorItem.units		= ((int)pointTagData.Value.Units).ToString();
						calculatorItem.dataType		= pointTagData.Value.ValueType.ToString();
						calculatorItem.UnitsType	= pointTagData.Value.EngineeringUnitsType;
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
			catch (Exception ex)
			{
				string msg = ErrorMsgPrefix + "Error retrieving Point Tags. " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMEventLogEntryType.Error));
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
						calculatorItem.numberDecimals		= pointTag.DecimalPlaces;
						calculatorItem.startValue			= this.SetStartValue(pointTag, ref rawValue);
						calculatorItem.startValueRaw		= rawValue;
						calculatorItem.startSourceDateTime	= pointTag.SourceTimeStamp;

						// set the end equal to the start and the dif at 0 initialy
						calculatorItem.endValue				= calculatorItem.startValue;
						calculatorItem.endValueRaw			= calculatorItem.startValueRaw;
						calculatorItem.endSourceDateTime	= pointTag.SourceTimeStamp;
						rawValue = "0";
						calculatorItem.diffValue		= string.Empty;
						calculatorItem.diffValueRaw		= rawValue;
						calculatorItem.startValueRaw	= this.GetRawValue(calculatorItem.startValue, pointTag.Units);
					}
				}
			}

			return listOfCalculatorItems;
		}

		/// <summary>
		/// This method will update the newly calculated start/end values based on the field that changed.
		/// </summary>
		/// <param name="fieldId">The field ID that changed</param>
		/// <param name="model">The model.</param>
		/// <param name="pointTagDictionary">The point tag dictionary.</param>
		/// <param name="site">The current site.</param>
		private void UpdateCalculatedValues(string fieldId, MovementHistoryNodeEditorModel model, Dictionary<Guid, PointTag> pointTagDictionary)
		{
			string changedTagId = this.GetTagIdFromFieldId(fieldId);
			var pointTagList = new List<PointTag>();


			foreach (var tagData in pointTagDictionary)
			{
				switch (tagData.Value.ID)
				{
					case TagLevelProduct:
						double valueDouble = 0;
						int denominator = model.LevelUnits == EngineeringUnit.FmlFtIn8Th ? 8 : 16;

						if (this.ConvertLevelToDecimal(denominator, model.LevelStr, ref valueDouble))
						{
							tagData.Value.Value = valueDouble;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagTemperatureProduct:
						if (double.TryParse(model.TemperatureStr, out double convertedValue))
						{
							tagData.Value.Value = convertedValue;
							pointTagList.Add(tagData.Value);
							tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagDensityProductStd:
						if (double.TryParse(model.StdDensityStr, out convertedValue))
						{
							tagData.Value.Value = convertedValue;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagTemperatureAmbient:
						if (double.TryParse(model.AmbientTemperatureStr, out convertedValue))
						{
							tagData.Value.Value = convertedValue;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagLevelWater:
						valueDouble = 0;
						denominator = model.LevelUnits == EngineeringUnit.FmlFtIn16Th ? 16 : 8;

						if (this.ConvertLevelToDecimal(denominator, model.WaterLevelStr, ref valueDouble))
						{
							tagData.Value.Value = valueDouble;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagDensityProductObserved:
						if (double.TryParse(model.DensityStr, out convertedValue))
						{
							tagData.Value.Value = convertedValue;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagVolumeGrossObserved:
						if (double.TryParse(model.GrossVolumeStr, out convertedValue))
						{
							tagData.Value.Value = convertedValue;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagVolumeNetStd:
						if (double.TryParse(model.NetVolumeStr, out convertedValue))
						{
							tagData.Value.Value = convertedValue;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagMassLiquid:
						if (double.TryParse(model.MassStr, out convertedValue))
						{
							tagData.Value.Value = convertedValue;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
					case TagPercentBsw:
						if (double.TryParse(model.BswStr, out convertedValue))
						{
							tagData.Value.Value = convertedValue;
							pointTagList.Add(tagData.Value);
							if (tagData.Value.ID == changedTagId) tagData.Value.SourceTimeStamp = DateTimeOffset.Now;
						}
						break;
				}
			}

			var calculatedPointTagList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointTag>>(
																	x => x.RunPointCalculatorX(this.Security, model.MovementPointGuid, pointTagList));

			var calculatedList = new List<calculatorItems>();

			foreach (var pointTag in calculatedPointTagList)
			{
				var calculatorItem = new calculatorItems();
				calculatedList.Add(calculatorItem);

				string rawValue = "0";
				calculatorItem.numberDecimals		= pointTag.DecimalPlaces;
				calculatorItem.startValue			= this.SetStartValue(pointTag, ref rawValue);
				calculatorItem.startValueRaw		= rawValue;
				calculatorItem.startSourceDateTime	= pointTag.SourceTimeStamp;
				calculatorItem.tagGuid				= pointTag.IdentityGuid;
				calculatorItem.tagName				= pointTag.ID;
				calculatorItem.editDisabled			= 0; // this.isTagEditDisabled(pointTagData.Value);
				calculatorItem.unitsString			= EngineeringUnits.GetUnitAbbreviation(pointTag.Units);
				calculatorItem.units				= ((int)pointTag.Units).ToString();
				calculatorItem.dataType				= pointTag.ValueType.ToString();
				calculatorItem.UnitsType			= pointTag.EngineeringUnitsType;
				calculatorItem.maximumValue			= pointTag.Maximum;
				calculatorItem.minimumValue			= pointTag.Minimum;
			}

			this.UpdateModelFromPoints(ref model, ref calculatedList);
		}

		/// <summary>
		/// This method will return the Tag ID for a given field ID.
		/// </summary>
		/// <param name="fieldId">The field ID filter</param>
		/// <returns>Returns a tag ID.</returns>
		private string GetTagIdFromFieldId(string fieldId)
		{
			switch (fieldId)
			{
				// Start fields
				case LevelField:
					return TagLevelProduct;
				case TemperatureField:
					return TagTemperatureProduct;
				case DensityField:
					return TagDensityProductObserved;
				case StdDensityField:
					return TagDensityProductStd;
				case AmbTempField:
					return TagTemperatureAmbient;
				case WaterLevelField:
					return TagLevelWater;
				case GrossVolumeField:
					return TagVolumeGrossObserved;
				case NetVolumeField:
					return TagVolumeNetStd;
				case MassField:
					return TagMassLiquid;
				case BswField:
					return TagPercentBsw;
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will set the starting tag values.
		/// </summary>
		/// <param name="pointTagData"></param>
		/// <param name="rawValue"></param>
		/// <returns>Returns the starting value.</returns>
		private string SetStartValue(PointTag pointTagData, ref string rawValue, bool useFullPrecision = true)
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
					return this.FormatDoubleToString(archiveValueDouble, pointTagData, useFullPrecision);
			}
		}

		/// <summary>
		/// This method will set the number format info.
		/// </summary>
		/// <param name="model">The model that contains the numbering information.</param>
		private void SetNumberFormatting(MovementHistoryNodeEditorModel model)
		{
			this.numberFormatInfo = new NumberFormatInfo
			{
				NumberDecimalSeparator = model.NumberDecimalSeparator,
				NumberGroupSeparator = model.NumberGroupSeparator,
				NumberGroupSizes = model.NumberGroupSizes
			};
		}

		/// <summary>
		/// This method will format a double to a string.
		/// </summary>
		/// <param name="archiveValueDouble">The value to format.</param>
		/// <param name="format"></param>
		/// <param name="pointTag"></param>
		/// <returns>Returns a double formatted to string.</returns>
		private string FormatDoubleToString(double archiveValueDouble, PointTag pointTag, bool useFullPrecision)
		{
			this.numberFormatInfo.NumberDecimalDigits = pointTag.DecimalPlaces;
			if (useFullPrecision)
			{
				return PointManager.FormatValueFullPrecision(typeof(double), pointTag.Units, this.numberFormatInfo, archiveValueDouble);
			}
			return PointManager.FormatValue(typeof(double), pointTag.Units, this.numberFormatInfo, archiveValueDouble);
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

			if (levelUnit != EngineeringUnit.FmlFtIn16Th && levelUnit != EngineeringUnit.FmlFtIn8Th)
			{
				return this.ConvertToDouble(inValue);
			}

			int denominator = levelUnit == EngineeringUnit.FmlFtIn16Th ? 16 : 8;
			double feet = 0.0;

			if (this.ConvertLevelToDecimal(denominator, inValue, ref feet) == false)
			{
				return null;
			}

			return feet;
		}

		/// <summary>
		/// This method will convert a string into a double.
		/// </summary>
		/// <param name="inValue">The string value to convert.</param>
		/// <returns>Returns a double or null.</returns>
		private double? ConvertToDouble(string inValue)
		{
			if (string.IsNullOrEmpty(inValue))
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
		/// This method will get the site's number and date/time formats and set the 
		/// model.
		/// </summary>
		/// <param name="model">The model to be updated.</param>
		private void SetDateAndNumberFormats(MovementHistoryNodeEditorModel model)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			model.NumberGroupSeparator		= site.NumberGroupSeparator;
			model.NumberDecimalSeparator	= site.NumberDecimalSeparator;
			model.NumberGroupSizes			= site.GetNumberGroupSizes();
			model.ShortDatePattern			= site.ShortDatePattern;
			model.TimePattern				= site.TimePattern;
			model.TimeZone					= site.TimeZone;
		}
		#endregion
	}
}