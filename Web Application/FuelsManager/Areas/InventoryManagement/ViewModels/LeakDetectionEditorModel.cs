

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using FMBusinessObjects.DataObjects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [Serializable]
	public class LeakDetectionEditorModelSettings
	{
		public LeakDetectionEditorModelSettings()
		{
  		}


	}


	[Serializable]
	[Bind(Exclude = "EditorEntries")]
	public class LeakDetectionEditorModel
	{
		public LeakAnalysisMethod LeakAnalysisMethod { get; set; }
		public LeakAnalysisType LeakAnalysisType  { get; set; }

		public bool LeakAutoPrint  { get; set; }

		public int MinimumFillPercentage { get; set; }

		public int LeakPrintDaysBeforeEndOfMonth  { get; set; }
		public DateTime LeakPrintTime { get; set; }
		public string GaugeType { get; set; }

        public string PointId { get; set; }
        public string PointPropertyId { get; set; }
		public List<KeyValuePair<LeakAnalysisMethod, string>> LeakAnalysisMethodList { get; set; }
		public List<KeyValuePair<LeakAnalysisType, string>> LeakAnalysisTypeList { get; set; }
		public List<KeyValuePair<int, string>> LeakPrintDaysBeforeEndOfMonthList { get; set; }

		public List<KeyValuePair<string, string>> GaugeTypeList { get; set; }

		public Guid PointGuid { get; set; }
		public Guid PointPropertyGuid { get; set; }

		public SiteClass Site { get; set; }
		public bool IsTemplatePoint { get; set; }

		private const int MaxDaysBeforeEndOfMonth = 27;

		public LeakDetectionEditorModel(bool isTemplatePoint, string pointPropertyID, Guid pointPropertyGuid, BasePoint basePoint, SiteClass site, LeakDetectionSettings leakDetectionSettings, int activeTab, GaugeTypeCollectionClass gaugeTypes)
		{
			this.IsTemplatePoint = isTemplatePoint;
			this.Site = site;
			this.PointId = pointPropertyGuid.ToString();
			this.PointId = basePoint.ID;

			this.PointPropertyGuid = pointPropertyGuid;
			this.PointPropertyId = pointPropertyID;
            this.GaugeType = leakDetectionSettings.GaugeType;
            this.LeakAnalysisMethod = leakDetectionSettings.AnalysisMethod;
			this.LeakPrintDaysBeforeEndOfMonth = leakDetectionSettings.PrintDaysBeforeEOM;

			this.LeakAnalysisType = leakDetectionSettings.AnalysisType;
            this.LeakPrintTime = leakDetectionSettings.PrintTime;
			this.LeakAutoPrint = leakDetectionSettings.AutoPrint;
			this.MinimumFillPercentage = leakDetectionSettings.MinimumFillPercentage;
			popultateDropDownListOptions( gaugeTypes);
		}

		private void  popultateDropDownListOptions(GaugeTypeCollectionClass gaugeTypes)
        {
			this.LeakAnalysisMethodList = new List<KeyValuePair<LeakAnalysisMethod, string>>();
            foreach (var method in (LeakAnalysisMethod[]) Enum.GetValues(typeof(LeakAnalysisMethod)))
            {
                LeakAnalysisMethodList.Add(new KeyValuePair<LeakAnalysisMethod, string>(method,LeakDetectionSettings.GetLeakAnalysisMethodDisplayName( method)));
            }

            this.LeakAnalysisTypeList = new List<KeyValuePair<LeakAnalysisType, string>>();
            foreach (var analysisType in (LeakAnalysisType[]) Enum.GetValues(typeof(LeakAnalysisType)))
            {
                LeakAnalysisTypeList.Add(new KeyValuePair<LeakAnalysisType, string>(analysisType, LeakDetectionSettings.GetLeakAnalysisTypeDisplayName(analysisType)));
            }

            LeakPrintDaysBeforeEndOfMonthList = new List<KeyValuePair<int, string>>();
			for (int num = 0; num <= MaxDaysBeforeEndOfMonth; num++)
			{
				LeakPrintDaysBeforeEndOfMonthList.Add(new KeyValuePair<int, string>(num, num.ToString()));
			}


			GaugeTypeList = new List<KeyValuePair<string, string>>();

            foreach (var gaugeType in gaugeTypes.OrderByDescending(g => g.ID == "Generic" || g.ID == "Undefined").ThenBy(g => g.Name))
			{
				GaugeTypeList.Add(new KeyValuePair<string, string>(gaugeType.ID, gaugeType.Name));
			}

		}
	}
}
