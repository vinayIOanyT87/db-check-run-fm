

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using Newtonsoft.Json;

	public class AlarmSummaryTabController : FMBaseControllerEx
	{
		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="model">The model to serialize</param>
		/// <returns>An array of data dictionary keys.</returns>
		[NonAction]
		public static string SerializeModel(AlarmSummaryTabModel model)
		{
			return JsonConvert.SerializeObject(model);
		}

		/// <summary>
		/// Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="modelStr">The model to serialize</param>
		/// <returns>An array of data dictionary keys.</returns>
		[NonAction]
		public static AlarmSummaryTabModel DeserializeModel(string modelStr)
		{
			var jsonSerializerSettings = new JsonSerializerSettings
			{
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			var obj = JsonConvert.DeserializeObject<AlarmSummaryTabModel>(modelStr, jsonSerializerSettings);
			return obj;
		}

		[NonAction]
		public static AlarmSummaryTabModel GetModel(SecurityClass security, bool unacknowledged, bool unsilenced)
		{
			var isAcknowledgeAndSilenceEnabgled = FMChannelHelper.MakeCall<ISites, bool>(x => x.IsAcknowledgeAndSilenceEnabled(security));
			var alarmStatusList = FMChannelHelper.MakeCall<IAlarmStatus, List<AlarmStatusClass2>>(x => x.GetActiveAlarms(security, unacknowledged, unsilenced, false));
			var model = new AlarmSummaryTabModel { AlarmSummaries = alarmStatusList, 
													HasAcknowledgeAllRight = (security.HasRight(RIGHT.ACKNOWLEDGE_ALL_ALARMS) && isAcknowledgeAndSilenceEnabgled) ? true : false,
													HasAcknowledgeCommentsRight = (security.HasRight(RIGHT.ACKNOWLEDGE_WITH_COMMENTS) && isAcknowledgeAndSilenceEnabgled) ? true : false,
													HasSilenceRight = (security.HasRight(RIGHT.SILENCE_ALARMS) && isAcknowledgeAndSilenceEnabgled) ?true : false,
													HasViewPointDetailRight = security.HasRight(RIGHT.OPERATE_VIEW_POINTS),
			};
			return model;
		}



		// GET: InventoryManagement/AlarmSummaryTab
		[HttpGet]
		public ActionResult AlarmSummaryTabView()
		{
			var model = GetModel(this.Security, false, false);
			return this.View(model);
		}

		// GET: InventoryManagement/AlarmSummaryTab
		[HttpGet]
		public ActionResult AlarmSummaryView()
		{
			if (this.Security.HasRight(RIGHT.OPERATE_VIEW_ALARM_SUMMARY))
			{
				var isAcknowledgeAndSilenceEnabgled = FMChannelHelper.MakeCall<ISites, bool>(x => x.IsAcknowledgeAndSilenceEnabled(this.Security));

				var model = new AlarmSummaryTabModel
			            {
				            AlarmSummaries = new List<AlarmStatusClass2>(),
								HasAcknowledgeAllRight = (this.Security.HasRight(RIGHT.ACKNOWLEDGE_ALL_ALARMS) && isAcknowledgeAndSilenceEnabgled) ? true : false,
								HasAcknowledgeCommentsRight = (this.Security.HasRight(RIGHT.ACKNOWLEDGE_WITH_COMMENTS) && isAcknowledgeAndSilenceEnabgled) ? true : false,
								HasSilenceRight = (this.Security.HasRight(RIGHT.SILENCE_ALARMS) && isAcknowledgeAndSilenceEnabgled) ? true : false,
								HasViewPointDetailRight = this.Security.HasRight(RIGHT.OPERATE_VIEW_POINTS)
							};
			return this.PartialViewWithErrorMessages("AlarmSummaryTabView", model, JsonRequestBehavior.AllowGet);
			}
			else
			{
				this.OnError(this.GetTranslatedText("You have no rights to access this screen."));
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult AcknowledgeAlarms(List<string> alarmGuidList)
		{
			try
			{

				if (alarmGuidList == null || alarmGuidList.Count < 1)
				{
					throw new Exception("No Alarms To Acknowledge");
				}

				this.AcknowledgeAllAlarms(alarmGuidList);
				return this.JsonWithErrorMessages(null);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

		}

		protected void AcknowledgeAllAlarms(List<string> alarmGuidStringList)
		{
			var alarmGuidList = new List<Guid>();

			foreach(var alarmGuidString in alarmGuidStringList)
			{
				alarmGuidList.Add(new Guid(alarmGuidString));
			}
	
			FMChannelHelper.MakeCall<IAlarmStatus>(x => x.AcknowledgeAlarms(this.Security, string.Empty, alarmGuidList));
		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult AcknowledgeAlarmsWithComment(string modelStr, List<string> alarmGuidList)
		{
			try
			{
				var model = DeserializeModel(modelStr);

				if (model == null)
				{
					throw new Exception("No Model in Session");
				}

				if (alarmGuidList == null || alarmGuidList.Count < 1)
				{
					throw new Exception("No Alarms To Acknowledge Selected");
				}

				return this.AcknowledgeAllAlarmsWithComment(model, alarmGuidList);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SilenceAlarms()
		{
			try
			{
				FMChannelHelper.MakeCall<IAlarmStatus>(x => x.SilenceAlarms(this.Security));
				return this.JsonWithErrorMessages(null);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

		}

		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult Refresh()
		{
			try
			{
				var model = GetModel(this.Security, false, false );

				return this.JsonWithErrorMessages(model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

		}

		protected ActionResult AcknowledgeAllAlarmsWithComment(AlarmSummaryTabModel model, List<string> alarmGuidList)
		{
			var unackAlarms = new List<Guid>();

			foreach (var alarmSummary in model.AlarmSummaries)
			{
				if (alarmSummary.Acknowledged == false && alarmGuidList.Contains(alarmSummary.AlarmGuid.ToString()))
				{
					unackAlarms.Add(alarmSummary.AlarmGuid);
				}
			}
			// if there are no unack alarms selected throw an error
			if(unackAlarms == null || unackAlarms.Count < 1)
			{
				throw new Exception("No Unacknowledged Alarms Selected");
			}
			AckCommentDataEntryModel ackCommentModel = new AckCommentDataEntryModel(unackAlarms);

			return this.AckWithComment(ackCommentModel);
		}

		protected ActionResult AckWithComment(AckCommentDataEntryModel model)
		{
			try
			{
				return this.PartialViewWithErrorMessages("../AckCommentDataEntry/AckCommentDataEntryView", model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Shelve(string modelStr, List<string> alarmGuidList)
		{
			try
			{
				var model = DeserializeModel(modelStr);

				if (model == null)
				{
					throw new Exception("No Model in Session");
				}

				if (alarmGuidList == null || alarmGuidList.Count < 1)
				{
					throw new Exception("No Alarms Selected For Shelve");
				}

				if (alarmGuidList.Count > 1)
				{
					throw new Exception("Only One Alarm Can Be Selected For Shelve");
				}

				return this.ShelveAlarms(model, alarmGuidList);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}
		}

		protected ActionResult ShelveAlarms(AlarmSummaryTabModel model, List<string> alarmGuidList)
		{
			try
			{
				var alarmsToShelve = new List<Guid>();
				var alarmsIdString = "";

				foreach (var alarmSummary in model.AlarmSummaries)
				{
					if (alarmGuidList.Contains(alarmSummary.AlarmGuid.ToString()))
					{
						alarmsToShelve.Add(alarmSummary.AlarmGuid);
						if (!string.IsNullOrWhiteSpace(alarmsIdString))
						{
							alarmsIdString += ", ";
						}
						alarmsIdString += FMBaseController.TranslateText(alarmSummary.PointID) + ":" + FMBaseController.TranslateText(alarmSummary.TagID) + ":" + FMBaseController.TranslateText(alarmSummary.Status);
					}
				}
				if (!alarmsToShelve.Any())
				{
					throw new Exception("No Valid Alarms To Shelve Were Found");
				}
				ShelveDataEntryModel shelveModel = new ShelveDataEntryModel(alarmsToShelve, alarmsIdString);
				return this.PartialViewWithErrorMessages("../ShelveDataEntry/ShelveDataEntryView", shelveModel);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}
		}
	}
}