using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{

	[Serializable]
	public class AlarmHistoryFilterObject
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
	public class AlarmHistoryUserViewStateSettings
	{
		//public int[] VisibleArr;

		//public int[] InvisibleArr;

		//public AlarmHistoryFilterObject[] Filters;

		public string JsonViewState;
	}
}
