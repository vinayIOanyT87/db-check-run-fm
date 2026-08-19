
namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class AlarmHistoryTabRow
	{
		public string DT_RowId { get; set; }

		public string PointDescription { get; set; }

		public string AlarmAndEventRecordGuid { get; set; }

		public string AlarmOrTagGuid { get; set; }

		public string AlarmTestGuid { get; set; }

		public string DateAndTime { get; set; }

		public string Point { get; set; }

		public string Site { get; set; }

		public string AlarmState { get; set; }

		public string PointType { get; set; }

		public string Variable { get; set; }

		public string Value { get; set; }

		public string Units { get; set; }

		public string Priority { get; set; }

		public string Action { get; set; }

		public string User { get; set; }

		public string Comments { get; set; }

		public string CommentUserName { get; set; }

		public string CommentDateTime { get; set; }

		public long DateAndTimeTicks { get; set; }

		public long CommentDateTimeTicks { get; set; }
	}

	[Serializable]
	public class AlarmHistoryTabModel
	{
		public List<AlarmHistoryTabRow> AlarmHistories;
		public SiteClass Site { get; set; }
		public AlarmHistoryUserViewStateSettings ViewStateSettings { get; set; }
	}
}
