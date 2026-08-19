namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Newtonsoft.Json;

	using System;
	using System.Globalization;
	using System.Web.Mvc;

	public class MovementHistoryMovementDataEditorController : FMBaseControllerEx
	{
		#region Data members
		const string ErrorMsgPrefix = "MovementHistoryMovementDataEditorView: ";
		private NumberFormatInfo numberFormatInfo = null;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public MovementHistoryMovementDataEditorController()
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
		public static string SerializeModel(MovementHistoryMovementDataEditorModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="modelStr">The model to serialize</param>
		/// <returns>Returns the movement user data editor model.</returns>
		[NonAction]
		public static MovementHistoryMovementDataEditorModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var model = JsonConvert.DeserializeObject<MovementHistoryMovementDataEditorModel>(modelStr, jsonSerializerSettings);
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
		public ActionResult MovementHistoryMovementDataEditor(Guid movementHistoryGuid)
		{
			try
			{
				var model = new MovementHistoryMovementDataEditorModel { MovementHistoryGuid = movementHistoryGuid };
				this.SetDateAndNumberFormats(model);
				this.SetNumberFormatting(model);

				// Populate the model from the movement history record.
				this.PopulateModel(model);

				model.HasModifyRights = this.Security.HasRight(RIGHT.OPERATE_MODIFY_MOVEMENT_HISTORY);

				return base.PartialViewWithErrorMessages("MovementHistoryMovementDataEditor", model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				string msgBasic = this.GetTranslatedText("Movement|Error Getting Movement History Movement Data.");
				string msgEventLog = ErrorMsgPrefix + msgBasic + " " + ex.Message;
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msgEventLog, FMEventLogEntryType.Error));

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
		public ActionResult SaveMovementHistoryMovementData(string movementHistoryMovementDataEditorModelStr)
		{
			MovementHistoryMovementDataEditorModel model = DeserializeModel(movementHistoryMovementDataEditorModelStr);

			try
			{
				this.SaveMovementHistoryMovementDataHelper(model);
				base.AddSuccess("Saved Successful");
			}
			catch (Exception ex)
			{
				string msg = ErrorMsgPrefix + "Error saving Movement Data. ";
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg + ex.Message, FMEventLogEntryType.Error));
				base.OnError(new Exception(msg));
				return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
			}

			return this.JsonWithErrorMessages(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will save the movement data to the movement history.
		/// </summary>
		/// <param name="model">The model used to update the record.</param>
		private void SaveMovementHistoryMovementDataHelper(MovementHistoryMovementDataEditorModel model)
		{
			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(this.Security, this.Security.SiteGuid));
			var movementHistoryDo = FMChannelHelper.MakeCall<IMovementHistories, MovementHistoryDO>(x => x.GetMovementRecordByGuid(this.Security, model.MovementHistoryGuid));

			if (movementHistoryDo == null || movementHistoryDo.MovementHistoryGuid == Guid.Empty)
			{
				string msg = ErrorMsgPrefix + "Error, could not find Hand Gauge record.";
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMEventLogEntryType.Error));
				throw new Exception("Could not find movement history final record.");
			}

			movementHistoryDo.StartTime = this.ConvertStringToDateTimeOffset(model.StartDateTimeStr, site);
			movementHistoryDo.CloseoutTime = this.ConvertStringToDateTimeOffset(model.CloseoutDateTimeStr, site);

			FMChannelHelper.MakeCall<IMovementHistories>(x => x.UpdateNodeDataToFinalRecord(this.Security, movementHistoryDo));
		}

		/// <summary>
		/// This method will populate the model from the database.
		/// </summary>
		/// <param name="model">The model to update.</param>
		private void PopulateModel(MovementHistoryMovementDataEditorModel model)
        {
			var movementHistoryRecord =
						FMChannelHelper.MakeCall<IMovementHistories, MovementHistoryDO>(x => x.GetMovementRecordByGuid(this.Security, model.MovementHistoryGuid));

			if (movementHistoryRecord == null || movementHistoryRecord.MovementHistoryGuid == Guid.Empty)
			{
				string msg = ErrorMsgPrefix + "Error, could not find Movement History record.";
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(msg, FMEventLogEntryType.Error));
				return;
			}

			model.RootParentGuid		= movementHistoryRecord.RootParentGuid;
			model.ParentGuid			= movementHistoryRecord.ParentGuid;
			model.MovementPointGuid		= movementHistoryRecord.PointGuid;
			model.PointId				= movementHistoryRecord.Name;
			model.NodeId				= movementHistoryRecord.Node;
			model.StartDateTimeStr		= this.ConvertDateTimeToLocalTime(movementHistoryRecord.StartTime, model.TimePattern, model.ShortDatePattern);
			model.CloseoutDateTimeStr	= this.ConvertDateTimeToLocalTime(movementHistoryRecord.CloseoutTime, model.TimePattern, model.ShortDatePattern);
		}

		/// <summary>
		/// This method will set the number format info.
		/// </summary>
		/// <param name="model">The model that contains the numbering information.</param>
		private void SetNumberFormatting(MovementHistoryMovementDataEditorModel model)
		{
			this.numberFormatInfo = new NumberFormatInfo
			{
				NumberDecimalSeparator	= model.NumberDecimalSeparator,
				NumberGroupSeparator	= model.NumberGroupSeparator,
				NumberGroupSizes		= model.NumberGroupSizes
			};
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
		private void SetDateAndNumberFormats(MovementHistoryMovementDataEditorModel model)
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