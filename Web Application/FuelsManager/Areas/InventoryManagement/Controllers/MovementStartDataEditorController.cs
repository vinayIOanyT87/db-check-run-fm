namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
	using Newtonsoft.Json;
	using Opc.Ua;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	
	public class MovementStartDataEditorController : FMBaseControllerEx
	{
		#region Data members
		const string ErrorMsgPrefix = "MovementStartDataEditorView: ";
		List<Guid> wellKnownTagGuids;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementStartDataEditorController()
		{
			wellKnownTagGuids = new List<Guid>
				{
					 Guids.TransferStartTimeGuid
				};
		}
		#endregion

		#region Public static methods

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="model">The model to serialize</param>
		/// <returns>Returns a string of the model.</returns>
		[NonAction]
		public static string SerializeModel(MovementStartDataEditorModel model)
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
		public ActionResult MovementStartDataEditor(Guid movementPointGuid)
		{
			try
			{
				MovementStartDataEditorModel model = this.GetMovementStartDataEditorModel(movementPointGuid);
				return base.PartialViewWithErrorMessages("MovementStartDataEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				string msgBasic = this.GetTranslatedText("Error Getting Movement Start Data.");
				string msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				base.OnError(new Exception(msgBasic));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// This method handles the save call from the UI to save the movement start data.
		/// </summary>
		/// <param name="movementStartDataEditorModelStr">The model string to save.</param>
		/// <returns>Return successful or an error.</returns>
		[HttpPost]
		public ActionResult SaveMovementStartData(string movementStartDataEditorModelStr)
		{
			string msgBasic = string.Empty;
			string msgEventLog = string.Empty;

			if (string.IsNullOrEmpty(movementStartDataEditorModelStr))
			{
				msgBasic = this.GetTranslatedText("Error, movement start data editor model is empty.");
				msgEventLog = ErrorMsgPrefix + msgBasic;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				base.OnError(new Exception(msgBasic));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			MovementStartDataEditorModel model = null;

			try
			{
				var jsonSerializerSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore };
				model = JsonConvert.DeserializeObject<MovementStartDataEditorModel>(movementStartDataEditorModelStr, jsonSerializerSettings);
			}
			catch (Exception ex)
			{
				msgBasic = this.GetTranslatedText("Error deserializing the model string.");
				msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

				base.OnError(new Exception(msgBasic));
				return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}

			if (model != null)
			{
				try
				{
					this.SaveMovementStartDataHelper(model);
					base.AddSuccess("Saved Successful");

					return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
				catch (Exception ex)
				{
					msgBasic = this.GetTranslatedText("Error saving the movement start data.");
					msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

					base.OnError(new Exception(msgBasic));
					return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
				}
			}

			msgBasic = this.GetTranslatedText("Error movement start data model is null.");
			msgEventLog = ErrorMsgPrefix + msgBasic;
			FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMBusinessObjects.Constants.FMEventLogEntryType.Error));

			base.OnError(new Exception(msgBasic));
			return base.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
		}


		#endregion


		#region Private methods
		/// <summary>
		/// This method will retrieve the movement start data information based on the movement point Guid.
		/// </summary>
		/// <param name="movementPointGuid">The movement point Guid used to retrieved the data.</param>
		/// <returns>Return a movement start data editor model.</returns>
		private MovementStartDataEditorModel GetMovementStartDataEditorModel(Guid movementPointGuid)
		{
			if (movementPointGuid == null || movementPointGuid == Guid.Empty)
			{
				return new MovementStartDataEditorModel();
			}

			var movementPoint = FMChannelHelper.MakeCall<IPoints, Point>(x => x.Get(this.Security, movementPointGuid, false));

			if (movementPoint == null)
			{
				throw new Exception("No Movement Point found.");
			}

			var model = new MovementStartDataEditorModel
			{
				MovementPointGuid = movementPointGuid,
				PointId = movementPoint.ID
			};

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			// Set the date and number formats based on the site;
			this.SetDateAndNumberFormats(site, model);


			// Get the Transfer Start Time data point value.
			List<PointValue> startDataPointValues = this.GetStartDataPointValues(movementPointGuid);

			if (startDataPointValues == null
			|| startDataPointValues.Count == 0
			|| !(startDataPointValues[0].Value is List<PointValue>))
			{
				throw new Exception("Transfer Start Time not available");
			}

			var transferStartTimeList = startDataPointValues[0].Value as List<PointValue>;

			if(transferStartTimeList.Count == 0
			|| !(transferStartTimeList[0].Value is DateTimeOffset))
			{
				throw new Exception("Transfer Start Time is not set");
			}

			var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
			var value = TimeZoneInfo.ConvertTime((DateTimeOffset)transferStartTimeList[0].Value, siteTimeZoneInfo);
			model.TransferStartTime = value.ToString(site.ShortDatePattern + " " + site.TimePattern);

			return model;
		}


		/// <summary>
		/// This method will get a list of user data point values based on the movement point Guid
		/// and well know tag Guids.
		/// </summary>
		/// <param name="movementPointGuidList">The movement point Guid list to retrieve.</param>
		/// <returns>Returns a list of point values.</returns>
		private List<PointValue> GetStartDataPointValues(Guid movementPointGuid)
		{
			var movementDataGuid = FMChannelHelper.MakeCall<IPointProperties, Guid>(x => x.GetPointPropertyGuid(this.Security, movementPointGuid, "Movement Data"));

			var pointValueIdentifier = new PointValueIdentifier(movementDataGuid,PointValueType.Setting, "TransferStartTime");

			var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(this.Security, new List<PointValueIdentifier>() { pointValueIdentifier }, false));

			return pointValueList;
		}


		/// <summary>
		/// This method will set the user data string from the point value.
		/// </summary>
		/// <param name="pointValue">The point value containing the data.</param>
		/// <param name="userData">The user data to update.</param>
		private void SetStartDataValue(PointValue pointValue, string startData)
		{
			if (pointValue != null)
			{
				pointValue.ServerTimeStamp = DateTimeOffset.UtcNow;
				pointValue.SourceTimeStamp = DateTimeOffset.UtcNow;
				pointValue.Status = StatusCodes.Good;
				pointValue.Value = startData;
			}
		}


		/// <summary>
		/// This method make that call to the service to save the movement user data.
		/// </summary>
		/// <param name="model">The model to save.</param>
		private void SaveMovementStartDataHelper(MovementStartDataEditorModel model)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			// Get the Transfer Start Time data point value.
			List<PointValue> startDataPointValues = this.GetStartDataPointValues(model.MovementPointGuid);

			if (startDataPointValues == null
			|| startDataPointValues.Count == 0
			|| !(startDataPointValues[0].Value is List<PointValue>))
			{
				throw new Exception("Transfer Start Time not available");
			}

			var transferStartTimeList = startDataPointValues[0].Value as List<PointValue>;

			if (transferStartTimeList.Count == 0
			|| !(transferStartTimeList[0].Value is DateTimeOffset))
			{
				throw new Exception("Transfer Start Time is not set");
			}

			var transferStartTime = DateTimeOffset.ParseExact(model.TransferStartTime, site.ShortDatePattern + " " + site.TimePattern, site.GetDateTimeFormatInfo());
			transferStartTime = transferStartTime.ToUniversalTime();

			if(model.ApplyToNodes)
			{
				var updatedTransferStartTimeList = new List<PointValue>();
				foreach(var pointValue in transferStartTimeList)
				{
					// Only apply to nodes that have started
					if(pointValue.Value != null)
					{
						pointValue.Value = transferStartTime;
						updatedTransferStartTimeList.Add(pointValue);
					}
				}

				startDataPointValues[0].Value = updatedTransferStartTimeList;
			}
			else
			{
				transferStartTimeList[0].Value = transferStartTime;
				transferStartTimeList.RemoveRange(1, transferStartTimeList.Count - 1);
			}

			EditValueController.SetPointValues(this.Security, site, startDataPointValues);
		}

		/// <summary>
		/// This method will get the site's number and date/time formats and set the 
		/// model.
		/// </summary>
		/// <param name="model">The model to be updated.</param>
		private void SetDateAndNumberFormats(SiteClass site, MovementStartDataEditorModel model)
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