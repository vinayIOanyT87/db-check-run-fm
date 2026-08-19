

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using Areas.Controllers;
    using FMBusinessObjects.DataObjects;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    


    [Serializable]
	public class PointValueConfigurationEditorModel
	{
		public bool IsTemplatePoint { get; set; }
		public bool IsSetting { get; set; }
		public Guid PointObjectGuid { get; set; }
		public string PointObjectName { get; set; }
		public Dictionary<Guid, string> ValueReferenceDictionary { get; set; }
		public Guid PointTemplateGuid { get; set; }
		public Guid? ValueGuid { get; set; }
		public int[] ValueId { get; set; }
		public Dictionary<Int64, string> ValueEntryDictionary { get; set; }
		public string ValueTypeString { get; set; }
		public object ValueReferenceObject { get; set; }
		public Dictionary<Guid, AlarmPriorityClass> AlarmPriorityDictionary { get; set; }
		public Dictionary<Guid, AlarmPriorityClass> NormalPriorityDictionary { get; set; }
		public Dictionary<Guid, string> AlarmCategoryDictionary { get; set; }



		public PointValueConfigurationEditorModel()
		{
			this.ValueReferenceDictionary = new Dictionary<Guid, string>();
			this.ValueGuid = null;
			this.ValueReferenceObject = null;
		}
	}
}