using System;
using System.Collections.Generic;
using System.Web.Mvc;

using FuelsManager.Areas.Controllers;
using FuelsManager.Areas.InventoryManagement.ViewModels;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using System.Data;

namespace FuelsManager.Areas.InventoryManagement.Controllers
{
    public class OperateStatisticsController : FMBaseController
    {
	// GET: InventoryManagement/OperateStatistics
	[HttpGet]
	public ActionResult Summary()
	{
		if (!this.Security.HasRight(RIGHT.VIEW_OPERATE_STATISTICS))
		{
		//../InventoryManagement/OperateStatistics/Summary
		throw new Exception("Access Denied");
		}

		var model = new OperateStatisticsSummaryModel();

		var userScreens = FMChannelHelper.MakeCall<ISessions, DataSet>(sessions => sessions.GetActiveOperateScreensList(this.Security));
		var userSccreensTable = userScreens.Tables[0];
		if (userScreens != null && userScreens.Tables != null && userScreens.Tables.Count == 1)
		{
			foreach (DataRow row in userSccreensTable.Rows)
			{
					model.SessionDetails.Add(new OperateStatisticsDetailModel()
					{
						User = row["UserId"].ToString(),
						Screen = row["WindowName"].ToString(),
						ClientIpAddress = row["ClientIpAddress"].ToString(),
						ServerIpAddress = row["WebServerIpAddress"].ToString(),
						AlarmNotificationsSessionAverage = Convert.ToInt32(row["AvgSessionTimeAlarmNotifications"]),
						AlarmNotificationsSessionMaximum = Convert.ToInt32(row["MaxSessionTimeAlarmNotifications"]),
						AlarmNotificationsMinuteAverage = Convert.ToInt32(row["AvgMinuteTimeAlarmNotifications"]),
						AlarmNotificationsMinuteMaximum = Convert.ToInt32(row["MaxMinuteTimeAlarmNotifications"]),
						AlarmRefreshSessionAverage = Convert.ToInt32(row["AvgSessionTimeAlarmRefresh"]),
						AlarmRefreshSessionMaximum = Convert.ToInt32(row["MaxSessionTimeAlarmRefresh"]),
						AlarmRefreshMinuteAverage = Convert.ToInt32(row["AvgMinuteTimeAlarmRefresh"]),
						AlarmRefreshMinuteMaximum = Convert.ToInt32(row["MaxMinuteTimeAlarmRefresh"]),
						UpdateDynamicTagGroupsSessionAverage = Convert.ToInt32(row["AvgSessionTimeDynamicPointGroup"]),
						UpdateDynamicTagGroupsSessionMaximum = Convert.ToInt32(row["MaxSessionTimeDynamicPointGroup"]),
						UpdateDynamicTagGroupsMinuteAverage = Convert.ToInt32(row["AvgMinuteTimeDynamicPointGroup"]),
						UpdateDynamicTagGroupsMinuteMaximum = Convert.ToInt32(row["MaxMinuteTimeDynamicPointGroup"]),
						UpdateValuesSessionAverage = Convert.ToInt32(row["AvgSessionTimeUpdateValues"]),
						UpdateValuesSessionMaximum = Convert.ToInt32(row["MaxSessionTimeUpdateValues"]),
						UpdateValuesMinuteAverage = Convert.ToInt32(row["AvgMinuteTimeUpdateValues"]),
						UpdateValuesMinuteMaximum = Convert.ToInt32(row["MaxMinuteTimeUpdateValues"]),
					}); ;
				}
			}

			return View("Summary", model);
		}
	}
}

