using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMBusinessObjects.DataObjects;
using System.Globalization;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	public class PointCalculatorModel
	{
		public NumberFormatInfo Format;
      public string datePattern { get; set; }
      public string timePattern { get; set; }
      public string siteId { get; set; }
      public string selectedBasePoint { get; set; }
		public Guid selectedBasePointGuid { get; set; }
		public Guid changedTagGuid { get; set; }
		public string changedTagColumn { get; set; }
		public int colorswipeIndex { get; set; }
		public bool enableTransfer { get; set; }
		public bool transferByNet { get; set; }
		public bool isBatchMode { get; set; }
		public BatchModeKey batchModeKey { get; set; }
		public string BatchModeChangedColumn {get; set; } = "none";
        public List<string> BatchTagNames {get;} = new List<string> {
            "Volume Total Observed",
            "Volume Water",
            "Volume Solids",
            "Volume Bottoms",
            "Temperature Product",
            "Density Product Standard",
            "Density Product Observed",
            "Volume Gross Observed",
            "Percent BSW",
            "Volume Net Standard",
            };

        public List<string> BatchNoEditTagNames = new List<string>
            {
            "Density Product Observed",
            "Volume Water",
            "Volume Solids",
            "Volume Bottoms",
            };
        public List<calculatorItems> calculatorItemList { get; set; }

		public bool EnableRowVisibilityConfigDropdown { get; set; } = false;
		public UInt32 PointCalculatorRowVisibilityConfig { get; set; } = 4294967295;
	}

	public class calculatorItems
	{
		public string tagName { get; set; }
		public Guid? tagGuid { get; set; }
		public byte editDisabled { get; set; }
		public byte numberDecimals { get; set; }
		public string unitsString { get; set; }
		public string units { get; set; }
		public string startValue { get; set; }
		public string startValueRaw { get; set; }
		public long startStatus { get; set; }
		public string endValue { get; set; }
		public string endValueRaw { get; set; }
		public long endStatus { get; set; }
		public string diffValue { get; set; }
		public string diffValueRaw { get; set; }
		public long diffStatus { get; set; }
      public string dataType { get; set; }
		public EngineeringUnitType UnitsType { get; set; }
		public double maximumValue { get; set; }
		public double minimumValue { get; set; }
		public DateTimeOffset startSourceDateTime { get; set; }
		public DateTimeOffset endSourceDateTime { get; set; }
		public bool isExposedSetting { get;set;} = false;
		public string acronym { get; set; } = string.Empty;
		public bool isBatchModeTag { get; set; } = false;

		public bool isVisible { get; set; } = true;
	}
}