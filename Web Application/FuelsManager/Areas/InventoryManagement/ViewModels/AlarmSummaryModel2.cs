

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;

	[Serializable]
	public class AlarmSummaryModel2
	{
		public string Greetings = "Hello World!";
		public bool HasAlarmHistoryRight;
	}

	[Serializable]
	public class AlarmNotificationModel
	{
		public AlarmSummaryTabModel AlarmDetail;
		public int NumberOfAlarms;
	}

}
