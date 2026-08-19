using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	public class OperateStatisticsDetailModel
	{
		public string User { get; set; }
		public string Screen { get; set; }
		public string ClientIpAddress { get; set; }
		public string ServerIpAddress { get; set; }
		public int UpdateValuesSessionAverage { get; set; }
		public int UpdateValuesSessionMaximum { get; set; }
		public int UpdateValuesMinuteAverage { get; set; }
		public int UpdateValuesMinuteMaximum { get; set; }
		public int AlarmNotificationsSessionAverage { get; set; }
		public int AlarmNotificationsSessionMaximum { get; set; }
		public int AlarmNotificationsMinuteAverage { get; set; }
		public int AlarmNotificationsMinuteMaximum { get; set; }
		public int AlarmRefreshSessionAverage { get; set; }
		public int AlarmRefreshSessionMaximum { get; set; }
		public int AlarmRefreshMinuteAverage { get; set; }
		public int AlarmRefreshMinuteMaximum { get; set; }
		public int UpdateDynamicTagGroupsSessionAverage { get; set; }
		public int UpdateDynamicTagGroupsSessionMaximum { get; set; }
		public int UpdateDynamicTagGroupsMinuteAverage { get; set; }
		public int UpdateDynamicTagGroupsMinuteMaximum { get; set; }
	}
}