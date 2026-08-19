

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Linq;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ChannelFactories;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
   using FMBusinessObjects.Exceptions;

   [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
	public class AlarmSummary2Controller : FMBaseControllerEx
	{
		// GET: InventoryManagement/AlarmSummary2
		[HttpGet]
		public ActionResult AlarmSummary2View()
		{
			var model = new AlarmSummaryModel2
			{
				HasAlarmHistoryRight = this.Security.HasRight(RIGHT.OPERATE_VIEW_ALARM_HISTORY)
			};
			return this.View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GetAlarmSummaryTab()
		{
			try
			{
				var model = AlarmSummaryTabController.GetModel(this.Security, false, false);
				return this.PartialViewWithErrorMessages("../AlarmSummaryTab/AlarmSummaryTabView", model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GetAlarmHistoryTab()
		{
			try
			{
				var model = AlarmHistoryTabController.GetBlankModel(this.Security);
				return this.PartialViewWithErrorMessages("../AlarmHistoryTab/AlarmHistoryTabView", model);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}

		}

		// GET: InventoryManagement/AlarmSummary
		[HttpGet]
		public ActionResult AlarmNotifications()
		{
         if (this.Security == null)
         {
            //Invalid session
            var r = this.Json(-1, JsonRequestBehavior.AllowGet);
            return r;
         }
         try
			{
				var alarmNotificationDetail = new AlarmNotificationModel();
				var alarmSummaryModel = AlarmSummaryTabController.GetModel(this.Security, true, true);

				// limit the number of alarms returned to just the last one
				var unAcknowledgedAlarms = alarmSummaryModel.AlarmSummaries.Where(x => x.Acknowledged == false).OrderByDescending(x => x.Timestamp);
				var highestUnAcknowledgedAlarm = unAcknowledgedAlarms.Where(x => x.IsNormal == false).OrderBy(x => x.AlarmPriority).ThenByDescending(x => x.Timestamp);
				if (highestUnAcknowledgedAlarm.Any())
				{
					alarmSummaryModel.AlarmSummaries = highestUnAcknowledgedAlarm.Take(1).ToList();
					if (alarmSummaryModel.AlarmSummaries.Count > 0)
					{
						var highestUnSilencedAlarms = unAcknowledgedAlarms.Where(x => x.Silenced == false && x.IsNormal == false).OrderBy(x => x.AlarmPriority).ThenByDescending(x => x.Timestamp);
						if (highestUnSilencedAlarms.Any())
						{
							var highestUnSilenced = highestUnSilencedAlarms.Take(1).ToList();

							if (highestUnSilenced.Count > 0)
							{
								alarmSummaryModel.AlarmSummaries[0].SoundFile = highestUnSilenced[0].SoundFile;
								alarmSummaryModel.AlarmSummaries[0].Silenced = highestUnSilenced[0].Silenced;
							}
						}
						else
						{
							highestUnSilencedAlarms = unAcknowledgedAlarms.Where(x => x.Silenced == false && x.IsNormal == true).OrderBy(x => x.AlarmPriority).ThenByDescending(x => x.Timestamp);
							if (highestUnSilencedAlarms.Any())
							{
								var highestUnSilenced = highestUnSilencedAlarms.Take(1).ToList();

								if (highestUnSilenced.Count > 0)
								{
									alarmSummaryModel.AlarmSummaries[0].SoundFile = highestUnSilenced[0].SoundFile;
									alarmSummaryModel.AlarmSummaries[0].Silenced = highestUnSilenced[0].Silenced;
								}
							}
						}
					}
				}

				alarmNotificationDetail.AlarmDetail = alarmSummaryModel;
				alarmNotificationDetail.NumberOfAlarms = unAcknowledgedAlarms.Count();
				return this.Json(alarmNotificationDetail, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.Json("Exception", JsonRequestBehavior.AllowGet);
			}
		}

		[HttpGet]
		public ActionResult AlarmNotificationsForMenu()
		{
         if (this.Security == null)
         {
            //Invalid session
            var r = this.Json(-1, JsonRequestBehavior.AllowGet);
            return r;
         }
         try
			{
				var alarmNotificationDetail = new AlarmNotificationModel();
				var alarmSummaryModel = AlarmSummaryTabController.GetModel(this.Security, true, true);

				// limit the number of alarms returned to just the last one
				var unAcknowledgedAlarms = alarmSummaryModel.AlarmSummaries.Where(x => x.Acknowledged == false).OrderByDescending(x => x.Timestamp);
				var highestUnAcknowledgedAlarm = unAcknowledgedAlarms.Where(x => x.IsNormal == false).OrderBy(x => x.AlarmPriority).ThenByDescending(x => x.Timestamp);
				if (highestUnAcknowledgedAlarm.Any())
				{
					alarmSummaryModel.AlarmSummaries = highestUnAcknowledgedAlarm.Take(1).ToList();
					if (alarmSummaryModel.AlarmSummaries.Count > 0)
					{
						var highestUnSilencedAlarms = unAcknowledgedAlarms.Where(x => x.Silenced == false && x.IsNormal == false).OrderBy(x => x.AlarmPriority).ThenByDescending(x => x.Timestamp);
						if (highestUnSilencedAlarms.Any())
						{
							var highestUnSilenced = highestUnSilencedAlarms.Take(1).ToList();

							if (highestUnSilenced.Count > 0)
							{
								alarmSummaryModel.AlarmSummaries[0].SoundFile = highestUnSilenced[0].SoundFile;
								alarmSummaryModel.AlarmSummaries[0].Silenced = highestUnSilenced[0].Silenced;
							}
						}
						else
						{
							highestUnSilencedAlarms = unAcknowledgedAlarms.Where(x => x.Silenced == false && x.IsNormal == true).OrderBy(x => x.AlarmPriority).ThenByDescending(x => x.Timestamp);
							if (highestUnSilencedAlarms.Any())
							{
								var highestUnSilenced = highestUnSilencedAlarms.Take(1).ToList();

								if (highestUnSilenced.Count > 0)
								{
									alarmSummaryModel.AlarmSummaries[0].SoundFile = highestUnSilenced[0].SoundFile;
									alarmSummaryModel.AlarmSummaries[0].Silenced = highestUnSilenced[0].Silenced;
								}
							}
						}
					}
				}
				else
				{
					alarmSummaryModel.AlarmSummaries = unAcknowledgedAlarms.OrderByDescending(x => x.Timestamp).Take(1).ToList();
				}

				alarmNotificationDetail.AlarmDetail = alarmSummaryModel;
				alarmNotificationDetail.NumberOfAlarms = unAcknowledgedAlarms.Count();
				return this.Json(alarmNotificationDetail, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.Json("Exception", JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult SyncUnresolvedConflictsCount()
		{
         if (this.Security == null)
         {
            //Invalid session
            var r = this.Json(-1, JsonRequestBehavior.AllowGet);
            return r;
         }
         try
			{
				var syncRecordConflictCount = FMChannelHelper.MakeCall<ISyncRecordConflicts, SyncRecordConflictCountDO>(x => x.GetUnresolvedConflictsCount(this.Security, null));

				return this.Json(syncRecordConflictCount?.Count ?? 0, JsonRequestBehavior.AllowGet);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.Json(null, JsonRequestBehavior.AllowGet);
			}
		}

		public ActionResult ControllerPingMechanism()
		{
			if (this.Security == null)
			{
				if (SessionStatus == 0)
				{
               //Invalid session
               SessionStatus = -1;
				}
				//SessionStatus -2 means session timed-out.
            return this.Json(SessionStatus, JsonRequestBehavior.AllowGet);
         }
			try
			{
            int somethingToReturn;
				if (this.Security.HasRight(RIGHT.OPERATE_VIEW_ALARM_SUMMARY)) //easy way to check if we have a valid session
					somethingToReturn = 1;
				else
					somethingToReturn = 0;

				return this.Json(somethingToReturn, JsonRequestBehavior.AllowGet);
			}
         catch (Exception except)
			{
				Global.WriteToEventLog(except.Message, System.Diagnostics.EventLogEntryType.Error);

				this.OnError(except);
				return this.Json(null, JsonRequestBehavior.AllowGet);
			}
		}


	}
}


