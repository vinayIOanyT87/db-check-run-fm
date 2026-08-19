namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using FMBusinessObjects.DataObjects;
	using System.Collections.Generic;


	[Serializable]
	public class DeviceAlarmMapsEditorModel
	{
		public Guid PointTemplateGuid { get; set; }
		public string PointTemplateId { get; set; }
		public DeviceAlarmMaps DeviceAlarmMaps { get; set; }
		public Dictionary<Guid, AlarmPriorityClass> NormalPriorityDictionary { get; set; }
		public Dictionary<Guid, AlarmPriorityClass> AlarmPriorityDictionary { get; set; }
		public Dictionary<Guid, string> AlarmCategoryDictionary { get; set; }

		public bool HasModifyRight;
		public SiteClass Site { get; set; }

		public DeviceAlarmMapsEditorModel()
		{
			this.DeviceAlarmMaps = new DeviceAlarmMaps();
		}

		public DeviceAlarmMapsEditorModel(PointTemplate pointTemplate, SiteClass site, Dictionary<Guid, AlarmPriorityClass> normalPriorityDictionary, Dictionary<Guid, AlarmPriorityClass> alarmPriorityDictionary, Dictionary<Guid, string> alarmCategoryDictionary)
		{
			this.Site = site;
			this.DeviceAlarmMaps = pointTemplate.DeviceAlarmMaps;
			this.NormalPriorityDictionary = normalPriorityDictionary;
			this.AlarmPriorityDictionary = alarmPriorityDictionary;
			this.AlarmCategoryDictionary = alarmCategoryDictionary;

			if (this.DeviceAlarmMaps == null)
			{
				this.DeviceAlarmMaps = new DeviceAlarmMaps();
			}


			this.PointTemplateGuid = pointTemplate.IdentityGuid;
			this.PointTemplateId = pointTemplate.ID;
		}
	}
}