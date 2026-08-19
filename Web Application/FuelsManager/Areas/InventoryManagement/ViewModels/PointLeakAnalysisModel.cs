using System;
using FMBusinessObjects.DataObjects;
using System.Globalization;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	public class PointLeakAnalysisModel
	{
		public NumberFormatInfo Format;
		public string selectedBasePoint { get; set; }
		public Guid selectedBasePointGuid { get; set; }
		public LeakDetectionSettings leakDetectionSettings { get; set; }

		public string gaugeTypeName { get; set; }

		public DateTimeOffset startTime { get; set; }

		public DateTimeOffset endTime { get; set; }

		public string dateTimeFormat { get; set; }
		public Guid siteGuid { get; set; }
		public string siteId { get; set; }

		public int volumeDecimalPlaces { get; set; }
		public int temperatureDecimalPlaces { get; set; }
		public int flowDecimalPlaces  { get; set; }

		public Guid LeakReportGuid { get; set; }
		public string LeakReportName { get; set; }
	}
}