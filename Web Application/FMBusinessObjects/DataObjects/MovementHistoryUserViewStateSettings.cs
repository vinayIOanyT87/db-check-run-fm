namespace FMBusinessObjects.DataObjects
{
	using System;

	[Serializable]
	public class MovementHistoryFilterObject
	{
		public string Name;

		public int Index;

		public string FilterCollection;

		public string FromDateStr;

		public string ToDateStr;

		public string CommentFromDateStr;

		public string CommentToDateStr;
	}


	[Serializable]
	public class MovementHistoryUserViewStateSettings
	{
		//public int[] VisibleArr;

		//public int[] InvisibleArr;

		//public AlarmHistoryFilterObject[] Filters;

		public string JsonViewState;
	}
}
