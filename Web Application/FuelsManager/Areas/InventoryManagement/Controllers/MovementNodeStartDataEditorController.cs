using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.DataObjects.CodedVariables;
using FMPointCommon;
using FuelsManager.Areas.Controllers;
using FuelsManager.Areas.InventoryManagement.ViewModels;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	 public class MovementNodeStartDataEditorController : FMBaseControllerEx
	 {
		#region Data members
		const string ErrorMsgPrefix = "MovementNodeStartDataEditorView: ";
		#endregion

		struct NodeData
		{
			public PointValue transferStartTime;
			public PointValue transferStartLevel;
			public PointValue transferStartTemperature;
			public PointValue transferStartGOV;
			public PointValue transferStartNSV;
			public PointValue transferStartMass;
			public PointValue transferStartDensity;
			public PointValue transferStartStdDensity;
			public PointValue transferMode;
		}
			private const string TagLevelProduct = "Level Product";
			private const string TagTemperatureProduct = "Temperature Product";
			private const string TagDensityProductStd = "Density Product Standard";
			private const string TagDensityProductObserved = "Density Product Observed";
			private const string TagVolumeGrossObserved = "Volume Gross Observed";
			private const string TagVolumeNetStd = "Volume Net Standard";
			private const string TagMassLiquid = "Mass Liquid";

			private const string TagStartLevelProduct = "Transfer Start Level";
			private const string TagStartTemperatureProduct = "Temperature Product";
			private const string TagStartDensityProductStd = "Density Product Standard";
			private const string TagStartDensityProductObserved = "Density Product Observed";
			private const string TagStartVolumeGrossObserved = "Transfer Start GOV";
			private const string TagStartVolumeNetStd = "Transfer Start NSV";
			private const string TagStartMassLiquid = "Mass Liquid";

			public List<string> lookupTagsList = new List<string>
			{
				TagLevelProduct,
				TagTemperatureProduct,
				TagDensityProductStd,
				TagDensityProductObserved,
				TagVolumeGrossObserved,
				TagVolumeNetStd,
				TagMassLiquid
			};
      #region Constructors
      /// <summary>
      /// This is the default constructor.
      /// </summary>
      public MovementNodeStartDataEditorController()
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
		public static string SerializeModel(MovementNodeStartDataEditorModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="modelStr">The model to serialize</param>
		/// <returns>Returns the movement user data editor model.</returns>
		[NonAction]
		public static MovementStartDataEditorModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var obj = JsonConvert.DeserializeObject<MovementStartDataEditorModel>(modelStr, jsonSerializerSettings);
			return obj;
		}
		#endregion


		#region Public methods
		/// <summary>
		/// This method retrieves the movement user data editor model based on the movement point GUID.
		/// </summary>
		/// <param name="movementPointGuid">The movement point GUID</param>
		/// <returns>Returns the Movement User Data Editor model.</returns>
		[HttpGet]
		public ActionResult MovementNodeStartDataEditor(Guid movementPointGuid, Guid movementNodePointGuid)
		{
			try
			{
				MovementNodeStartDataEditorModel model = this.GetMovementNodeStartDataEditorModel(movementPointGuid, movementNodePointGuid);
            return PartialViewWithErrorMessages("MovementNodeStartDataEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				string msgBasic = this.GetTranslatedText("Error Getting Movement Start Data.");
				string msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

				OnError(new Exception(msgBasic));
				return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method handles the save call from the UI to save the movement start data.
		/// </summary>
		/// <param name="movementStartDataEditorModelStr">The model string to save.</param>
		/// <returns>Return successful or an error.</returns>
		[HttpPost]
		public ActionResult SaveMovementNodeStartData(string movementNodeStartDataEditorModelStr)
		{
			string msgBasic = string.Empty;
			string msgEventLog = string.Empty;

			if (string.IsNullOrEmpty(movementNodeStartDataEditorModelStr))
			{
				msgBasic = this.GetTranslatedText("Error, movement node start data editor model is empty.");
				msgEventLog = ErrorMsgPrefix + msgBasic;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

				OnError(new Exception(msgBasic));
				return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			MovementNodeStartDataEditorModel model = null;

			try
			{
				var jsonSerializerSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore };
				model = JsonConvert.DeserializeObject<MovementNodeStartDataEditorModel>(movementNodeStartDataEditorModelStr, jsonSerializerSettings);
			}
			catch (Exception ex)
			{
				msgBasic = this.GetTranslatedText("Error deserializing the model string.");
				msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

				OnError(new Exception(msgBasic));
				return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			if (model != null)
			{
				try
				{
					this.SaveMovementNodeStartDataHelper(model);
					AddSuccess("Save Successful");

					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
				catch (Exception ex)
				{
					msgBasic = this.GetTranslatedText("Error saving the movement node start data.");
					msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

					OnError(new Exception(msgBasic));
					return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
			}

			//msgBasic = this.GetTranslatedText("Error movement start data model is null.");
			//msgEventLog = ErrorMsgPrefix + msgBasic;
			//FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

			//OnError(new Exception(msgBasic));
			return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}

      [HttpPost]
      public ActionResult GetArchivedMovementNodeData(string movementNodeStartDataEditorModelStr)
      {

         string msgBasic = string.Empty;
			string msgEventLog = string.Empty;

			if (string.IsNullOrEmpty(movementNodeStartDataEditorModelStr))
			{
				msgBasic = this.GetTranslatedText("Error, movement node start data editor model is empty.");
					msgEventLog = ErrorMsgPrefix + msgBasic;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

				OnError(new Exception(msgBasic));
				return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			MovementNodeStartDataEditorModel model = null;

			try
			{
				var jsonSerializerSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore };

            model = JsonConvert.DeserializeObject<MovementNodeStartDataEditorModel>(movementNodeStartDataEditorModelStr, jsonSerializerSettings);
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

				if (model != null && DateTimeOffset.TryParse(model.TransferStartTime, out DateTimeOffset end))
				{
					Guid movementPointGuid = model.MovementPointGuid;
					Guid movementNodePointGuid = model.MovementNodePointGuid;

					if (movementPointGuid == null || movementPointGuid == Guid.Empty)
					{
						return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
					}

					var movementPoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, movementPointGuid)) ?? throw new Exception("No Movement Point found.");
					var movementNodePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, movementNodePointGuid)) ?? throw new Exception("Movement Node not found");

					//	FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry($"{end}", FMEventLogEntryType.Warning));

					foreach (string tagName in lookupTagsList)
					{
						var ptGuid = FMChannelHelper.MakeCall<IPointTags, Guid>(x => x.GetIdentityGuid(this.Security, tagName, movementNodePointGuid));
						if (ptGuid != Guid.Empty)
						{
							PointTag pt = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, ptGuid));
							if (pt != null)
							{
								PointValue pv = new PointValue(pt);
								GetArchivedValueAndUnit(end, pv);
								SetModelTagValues(model, pv);
							}
						}
					}

					//  FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry($"{end}  model.TransferStartTime={model.TransferStartTime}  model.Temperature={model.Temperature}  model.Level={model.Level}", FMEventLogEntryType.Warning));
					return JsonWithErrorMessages(SerializeModel(model), JsonRequestBehavior.AllowGet);

				}
			}
			catch (Exception ex)
			{
				msgBasic = this.GetTranslatedText("Error getting the archived movement node data.");
				msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));
				OnError(new Exception(msgBasic));
         }

         return JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);

      }
      #endregion

      #region Private methods
      private string FormatDoubleToString(double archiveValueDouble, int decimalPlaces, EngineeringUnit units, NumberFormatInfo numberFormatInfo)
      {
			
         numberFormatInfo.NumberDecimalDigits = decimalPlaces;
         return PointManager.FormatValue(typeof(double), units, numberFormatInfo, archiveValueDouble);
      }
      private void SetModelTagValues(MovementNodeStartDataEditorModel model, PointValue pv)
		{
			if (model == null || pv == null)
			{
				return;
			}

         switch (pv.ID) 
			{
            case TagLevelProduct:
            case TagStartLevelProduct:
               model.Level = pv?.Value is null ? null : (double?)(double)pv.Value;
					model.LevelUnits = pv?.Units ?? EngineeringUnit.FmuNone;
					model.LevelUnitsStr = EngineeringUnits.GetUnitAbbreviation(model.LevelUnits);
					model.LevelPrecision = pv.DecimalPlaces;
					model.LevelFmtStr = model.Level.HasValue ? FormatDoubleToString(model.Level.Value, model.LevelPrecision,  model.LevelUnits, model.NumberFormatInfo) : string.Empty;
					break;
            case TagDensityProductObserved:
       //     case TagStartDensityProductObserved:
               model.Density = pv?.Value is null ? null : (double?)(double)pv.Value;
               model.DensityUnits = pv?.Units ?? EngineeringUnit.FmuNone;
               model.DensityUnitsStr = EngineeringUnits.GetUnitAbbreviation(model.DensityUnits);
               model.DensityPrecision = pv.DecimalPlaces;
               model.DensityFmtStr = model.Density.HasValue ? FormatDoubleToString(model.Density.Value, model.DensityPrecision, model.DensityUnits, model.NumberFormatInfo) : string.Empty;
               break;
            case TagDensityProductStd:
    //        case TagStartDensityProductStd:
               model.StdDensity = pv?.Value is null ? null : (double?)(double)pv.Value;
               model.StdDensityUnits = pv?.Units ?? EngineeringUnit.FmuNone;
               model.StdDensityUnitsStr = EngineeringUnits.GetUnitAbbreviation(model.DensityUnits);
               model.StdDensityPrecision = pv.DecimalPlaces;
               model.StdDensityFmtStr = model.StdDensity.HasValue ? FormatDoubleToString(model.StdDensity.Value, model.StdDensityPrecision, model.StdDensityUnits, model.NumberFormatInfo) : string.Empty;
               break;
            case TagMassLiquid:
       //     case TagStartMassLiquid:
               model.Mass = pv?.Value is null ? null : (double?)(double)pv.Value;
               model.MassUnits = pv?.Units ?? EngineeringUnit.FmuNone;
               model.MassUnitsStr = EngineeringUnits.GetUnitAbbreviation(model.MassUnits);
               model.MassPrecision = pv.DecimalPlaces;
               model.MassFmtStr = model.Mass.HasValue ? FormatDoubleToString(model.Mass.Value, model.MassPrecision, model.MassUnits, model.NumberFormatInfo) : string.Empty;
               break;
            case TagTemperatureProduct:
       //     case TagStartTemperatureProduct:
               model.Temperature = pv?.Value is null ? null : (double?)(double)pv.Value;
               model.TemperatureUnits = pv?.Units ?? EngineeringUnit.FmuNone;
               model.TemperatureUnitsStr = EngineeringUnits.GetUnitAbbreviation(model.TemperatureUnits);
               model.TemperaturePrecision = pv.DecimalPlaces;
               model.TemperatureFmtStr = model.Temperature.HasValue ? FormatDoubleToString(model.Temperature.Value, model.TemperaturePrecision, model.TemperatureUnits, model.NumberFormatInfo) : string.Empty;
               break;
            case TagVolumeGrossObserved:
            case TagStartVolumeGrossObserved:
               model.GrossVolume = pv?.Value is null ? null : (double?)(double)pv.Value;
               model.GrossVolumeUnits = pv?.Units ?? EngineeringUnit.FmuNone;
               model.GrossVolumeUnitsStr = EngineeringUnits.GetUnitAbbreviation(model.GrossVolumeUnits);
               model.GrossVolumePrecision = pv.DecimalPlaces;
               model.GrossVolumeFmtStr = model.GrossVolume.HasValue ? FormatDoubleToString(model.GrossVolume.Value, model.GrossVolumePrecision, model.GrossVolumeUnits, model.NumberFormatInfo) : string.Empty;
               break;
            case TagVolumeNetStd:
            case TagStartVolumeNetStd:
               model.NetVolume = pv?.Value is null ? null : (double?)(double)pv.Value;
               model.NetVolumeUnits = pv?.Units ?? EngineeringUnit.FmuNone;
               model.NetVolumeUnitsStr = EngineeringUnits.GetUnitAbbreviation(model.NetVolumeUnits);
               model.NetVolumePrecision = pv.DecimalPlaces;
               model.NetVolumeFmtStr = model.NetVolume.HasValue ? FormatDoubleToString(model.NetVolume.Value, model.NetVolumePrecision, model.NetVolumeUnits, model.NumberFormatInfo) : string.Empty;
               break;
				default:
               FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry($"not found Start tag name = {pv.ID}", FMEventLogEntryType.Error));
               break;

         }

      }
      /// <summary>
      /// This method will retrieve the movement start data information based on the movement point Guid.
      /// </summary>
      /// <param name="movementPointGuid">The movement point Guid used to retrieved the data.</param>
      /// <param name="movementNodePointGuid">The movement node point Guid used to retrieved the data.</param>
      /// <returns>Return a movement start data editor model.</returns>
      private MovementNodeStartDataEditorModel GetMovementNodeStartDataEditorModel(Guid movementPointGuid, Guid movementNodePointGuid)
		{
			if (movementPointGuid == null || movementPointGuid == Guid.Empty)
			{
				return new MovementNodeStartDataEditorModel();
			}

			var movementPoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, movementPointGuid)) ?? throw new Exception("No Movement Point found.");
			var movementNodePoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.GetPointBaseData(this.Security, movementNodePointGuid)) ?? throw new Exception("Movement Node not found");
			Guid transferStatusGuid = FMChannelHelper.MakeCall<IPointTags, Guid>(x => x.GetIdentityGuid(this.Security, "Transfer Status", movementNodePoint.IdentityGuid));
         PointTag transactionStatusTag = FMChannelHelper.MakeCall<IPointTags, PointTag>(x => x.Get(this.Security, transferStatusGuid));

         var model = new MovementNodeStartDataEditorModel
			{
				MovementPointGuid = movementPointGuid,
				MovementNodePointGuid = movementNodePointGuid,
				PointId = movementPoint.ID,
				NodeId = movementNodePoint.ID,
            TransferStatus = TransferStatuses.Inactive
         };
			if (transactionStatusTag != null)
			{
				model.TransferStatus = (TransferStatuses)transactionStatusTag.Value ;

			}
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			// Set the date and number formats based on the site;
			this.SetDateAndNumberFormats(site, model);


			// Get the Transfer Start Time data point value.
			NodeData startDataPointValues = this.GetStartDataPointValues(movementPointGuid, movementNodePointGuid);

			var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
			var value = TimeZoneInfo.ConvertTime((DateTimeOffset)startDataPointValues.transferStartTime.Value, siteTimeZoneInfo);
			model.NumberFormatInfo = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);
			model.TransferStartTime = value.ToString(site.ShortDatePattern + " " + site.TimePattern);
         //var mass = startDataPointValues.transferStartMass?.Value is null ? null : (double?)(double)startDataPointValues.transferStartMass.Value;

         SetModelTagValues(model, startDataPointValues.transferStartLevel);
         SetModelTagValues(model, startDataPointValues.transferStartTemperature);
         SetModelTagValues(model, startDataPointValues.transferStartGOV);
         SetModelTagValues(model, startDataPointValues.transferStartNSV);
         SetModelTagValues(model, startDataPointValues.transferStartMass);
         SetModelTagValues(model, startDataPointValues.transferStartDensity);
         SetModelTagValues(model, startDataPointValues.transferStartStdDensity);

       //  FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry($"Start tag name transferStartLevel = {startDataPointValues.transferStartLevel.ID}   transferStartTemperature = {startDataPointValues.transferStartTemperature.ID}   transferStartGOV = {startDataPointValues.transferStartGOV.ID}  transferStartNSV = {startDataPointValues.transferStartNSV.ID}  transferStartMass = {startDataPointValues.transferStartMass.ID}   transferStartDensity = {startDataPointValues.transferStartDensity.ID}    transferStartStdDensity = {startDataPointValues.transferStartStdDensity.ID}  ", FMEventLogEntryType.Warning));


         model.IsVolumeTransferNode = startDataPointValues.transferMode?.Value is null ? false : (startDataPointValues.transferMode.Value is VolumeTransferMode) ? true : false;

         return model;
		}

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

		/// <summary>
		/// This method will get a list of user data point values based on the movement point Guid
		/// and well know tag Guids.
		/// </summary>
		/// <param name="movementNodePointGuidList">The movement node point Guid list to retrieve.</param>
		/// <returns>Returns a set of point value, then the full movement data property.</returns>
		private NodeData GetStartDataPointValues(Guid movementPointGuid, Guid movementNodePointGuid)
		{
			NodeData nodeData = new NodeData();

			var movementDataGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, movementPointGuid, "Movement Data"));
			PointValueIdentifier pointValueIdentifier;
			List<PointValue> pointValues;

			// get Transfer StartTime.  Try from the Node first, then the Movement proper if we don't have a specific one here
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferStartTime");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferStartTime = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);
			if (nodeData.transferStartTime?.Value == null)
			{
				nodeData.transferStartTime = (pointValues?[0]?.Value as List<PointValue>)?[0]; // the first entry will be from the movement point itself
			}

			// Now get Transfer Start Level
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferStartLevel");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferStartLevel = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);

			// Now get Transfer Start Temperature
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "StartTemperatureProduct");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferStartTemperature = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);

			// Now get Transfer Start Gross Volume
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferStartGOV");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferStartGOV = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);

			// Now get Transfer Start Net Standard Volume
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferStartNSV");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferStartNSV = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);

			// Now get Transfer Start Mass Liquid
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "StartMassLiquid");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferStartMass = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);

			// Now get Transfer Start Density Product Observed
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "StartDensityProductObserved");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferStartDensity = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);

			// Now get Transfer Start Density Standard
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "StartDensityProductStandard");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferStartStdDensity = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);

			// Now get Transfer Mode
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferMode");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			nodeData.transferMode = (pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid);

         return nodeData;
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
		/// This method make that call to the service to save the movement user data.
		/// </summary>
		/// <param name="model">The model to save.</param>
		private void SaveMovementNodeStartDataHelper(MovementNodeStartDataEditorModel model)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			List<PointValue> updatedPointValues = new List<PointValue>();
			Guid movementPointGuid = model.MovementPointGuid;
			Guid movementNodePointGuid = model.MovementNodePointGuid;

			var movementDataGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, movementPointGuid, "Movement Data"));
			PointValueIdentifier pointValueIdentifier;
			List<PointValue> pointValues;

			// Update Transfer StartTime.  This goes directly back to the setting
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferStartTime");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			var transferStartTimeValue = ((pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid))
				?? throw new Exception("Unable to retrieve Transfer Start Time for update");
			
			var transferStartTime = DateTimeOffset.ParseExact(model.TransferStartTime, site.ShortDatePattern + " " + site.TimePattern, site.GetDateTimeFormatInfo());
			transferStartTime = transferStartTime.ToUniversalTime();

			if (transferStartTime != (DateTimeOffset)transferStartTimeValue.Value)
			{
				// As this goes back to the setting, update in place and write the full setting point value back
				transferStartTimeValue.Value = transferStartTime;
				updatedPointValues.Add(pointValues[0]);
			}

			// Now update Transfer Start Level.  This gets written to the tag, updating the tank or volume point
			// Not applicable to Volume Transfer Points
			if (!model.IsVolumeTransferNode)
			{
				pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferStartLevel");
				pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

				var transferStartLevelValue = ((pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid))
					?? throw new Exception("Unable to retreive Transfer Start Level for update");

				if (model.Level.HasValue
				&& (transferStartLevelValue.Value == null || (transferStartLevelValue.Value is double
				&&	model.Level != (double)transferStartLevelValue.Value)))
				{
					PointValue newTransferStartLevel = (PointValue)transferStartLevelValue.Clone();
					newTransferStartLevel.Value = model.Level;
					newTransferStartLevel.Status = Opc.Ua.StatusCodes.GoodLocalOverride;
					updatedPointValues.Add(newTransferStartLevel);
				}
			}

			// Now update Transfer Start Temperature.  This goes directly back to the setting
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "StartTemperatureProduct");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			var transferStartTemperatureValue = ((pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid))
				?? throw new Exception("Unable to retrieve Transfer Start Time for update");

			if (model.Temperature.HasValue
			&& (transferStartTemperatureValue.Value == null || (transferStartTemperatureValue.Value is double
			&& model.Temperature != (double)transferStartTemperatureValue.Value)))
			{
				// As this goes back to the setting, update in place and write the full setting point value back
				transferStartTemperatureValue.Value = model.Temperature;
				updatedPointValues.Add(pointValues[0]);
			}

			// Now get Transfer Start Gross Volume.  This gets written to the tag, updating the tank or volume point
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferStartGOV");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			var transferStartGovValue = ((pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid))
				?? throw new Exception("Unable to retreive Transfer Start Gov for update");

			if (model.GrossVolume.HasValue
			&& (transferStartGovValue.Value == null || (transferStartGovValue.Value is double
			&& model.GrossVolume != (double)transferStartGovValue.Value)))
			{
				PointValue newTransferStartGov = (PointValue)transferStartGovValue.Clone();
				newTransferStartGov.Value = model.GrossVolume;
				newTransferStartGov.Status = Opc.Ua.StatusCodes.GoodLocalOverride;
				updatedPointValues.Add(newTransferStartGov);
			}

			// Now get Transfer Start Net Volume.  This gets written to the tag, updating the tank or volume point
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "TransferStartNSV");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			var transferStartNsvValue = ((pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid))
				?? throw new Exception("Unable to retreive Transfer Start Nsv for update");

			if (model.NetVolume.HasValue
			&& (transferStartNsvValue.Value == null || (transferStartNsvValue.Value is double
			&& model.NetVolume != (double)transferStartNsvValue.Value)))
			{
				PointValue newTransferStartNsv = (PointValue)transferStartNsvValue.Clone();
				newTransferStartNsv.Value = model.NetVolume;
				newTransferStartNsv.Status = Opc.Ua.StatusCodes.GoodLocalOverride;
				updatedPointValues.Add(newTransferStartNsv);
			}

			// Now update Transfer Start Mass.  This goes directly back to the setting
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "StartMassLiquid");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			var transferStartMassValue = ((pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid))
				?? throw new Exception("Unable to retrieve Transfer Start Mass for update");

			if (model.Mass.HasValue 
				&& (transferStartMassValue.Value == null ||
					(transferStartMassValue.Value is double && model.Mass != (double)transferStartMassValue.Value)))
			{
            transferStartMassValue.Value = model.Mass;
            updatedPointValues.Add(pointValues[0]);
			}

			// Now update Transfer Start Density.  This goes directly back to the setting
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "StartDensityProductObserved");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			var transferStartDensityValue = ((pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid))
				?? throw new Exception("Unable to retrieve Transfer Start Density for update");

			if (model.Density.HasValue
			&& (transferStartDensityValue.Value == null || 
			(transferStartDensityValue.Value is double && model.Density != (double)transferStartDensityValue.Value)))
			{
				// As this goes back to the setting, update in place and write the full setting point value back
				transferStartDensityValue.Value = model.Density;
				updatedPointValues.Add(pointValues[0]);
			}

			// Now update Transfer Start Standard Density.  This goes directly back to the setting
			pointValueIdentifier = new PointValueIdentifier(movementDataGuid, PointValueType.Setting, "StartDensityProductStandard");
			pointValues = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			var transferStartStdDensityValue = ((pointValues?[0]?.Value as List<PointValue>)?.FirstOrDefault(x => x.PointGuid == movementNodePointGuid))
				?? throw new Exception("Unable to retrieve Transfer Start Standard Density for update");

			if (model.StdDensity.HasValue
			&& (transferStartStdDensityValue.Value == null || 
			(transferStartStdDensityValue.Value is double && model.StdDensity != (double)transferStartStdDensityValue.Value)))
			{
				// As this goes back to the setting, update in place and write the full setting point value back
				transferStartStdDensityValue.Value = model.StdDensity;
				updatedPointValues.Add(pointValues[0]);
			}

			EditValueController.SetPointValues(this.Security, site, updatedPointValues);
		}

		/// <summary>
		/// This method will get the site's number and date/time formats and set the 
		/// model.
		/// </summary>
		/// <param name="model">The model to be updated.</param>
		private void SetDateAndNumberFormats(SiteClass site, MovementNodeStartDataEditorModel model)
		{
			model.NumberGroupSeparator = site.NumberGroupSeparator;
			model.NumberDecimalSeparator = site.NumberDecimalSeparator;
			model.NumberGroupSizes = site.GetNumberGroupSizes();
			model.ShortDatePattern = site.ShortDatePattern;
			model.TimePattern = site.TimePattern;
			model.TimeZone = site.TimeZone;
		}
		#endregion
	}
}