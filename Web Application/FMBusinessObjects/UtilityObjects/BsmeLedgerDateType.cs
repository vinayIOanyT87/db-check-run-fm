namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	public static class BsmeLedgerDateType
	{
		public const string InventoryDateType = "Inventory Date";

		public enum DateProcessTypes { ByInventoryDate = 0, ByEbsPostDate = 1, ByCreateDate = 2, ByEbsSentToDate = 3, ByEbsAcknowledgedDate = 4 };

		private static readonly Dictionary<DateProcessTypes, string> DisplayText = new Dictionary<DateProcessTypes, string>
		{
			{ DateProcessTypes.ByInventoryDate, InventoryDateType },
			{ DateProcessTypes.ByCreateDate, "Create Date" },
			{ DateProcessTypes.ByEbsSentToDate, "EBS Sent Date" },
			{ DateProcessTypes.ByEbsAcknowledgedDate, "EBS Ack Date" },
			{ DateProcessTypes.ByEbsPostDate, "EBS Posted Date" }
		};

		public static string GetDisplayText(DateProcessTypes dateType)
		{
			var displayText = DisplayText[dateType];
			return displayText;
		}

		public static string GetDisplayText(object dateTypeIndex)
		{
			var index = dateTypeIndex as string;
			if (string.IsNullOrEmpty(index))
				return GetDisplayText(DateProcessTypes.ByInventoryDate);
			
			var dateType = (DateProcessTypes)Convert.ToInt32(index);
			return GetDisplayText(dateType);
		}

		public static string GetDisplayValue(DateProcessTypes dateType)
		{
			var displayValue = ((int)dateType).ToString(CultureInfo.InvariantCulture);
			return displayValue;
		}
	}
}
