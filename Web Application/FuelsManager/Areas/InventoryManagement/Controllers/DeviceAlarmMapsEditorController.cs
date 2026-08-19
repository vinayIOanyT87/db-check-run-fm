namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Globalization;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
	using System.Web.Mvc;
	using System.Collections.Generic;
	using System.Web.Script.Serialization;

	public class DeviceAlarmMapsEditorController : FMBaseControllerEx
	{

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult DeviceAlarmMapsEditor(Guid pointTemplateGuid)
		{
			PointTemplate pointTemplate = null;
			SiteClass site = null;
			Dictionary<Guid, AlarmPriorityClass> normalPriorityDictionary = new Dictionary<Guid, AlarmPriorityClass>();
			Dictionary<Guid, AlarmPriorityClass> alarmPriorityDictionary = new Dictionary<Guid, AlarmPriorityClass>(); ;
			Dictionary<Guid, string> alarmCategoryDictionary = null;

			try
			{
				pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));
				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));
				var allAlarmPriorityDictionary = DeviceAlarmMapsEditorController.GetAllAlarmPriorities(this.Security);
				foreach(var alarmPriority in allAlarmPriorityDictionary.Values)
				{
					if(alarmPriority.Priority.HasValue)
					{
						alarmPriorityDictionary.Add(alarmPriority.IdentityGuid, alarmPriority);
					}
					else
					{
						normalPriorityDictionary.Add(alarmPriority.IdentityGuid, alarmPriority);
					}
				}

				alarmCategoryDictionary = DeviceAlarmMapsEditorController.GetAllAlarmCategories(this.Security);
			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			var model = new DeviceAlarmMapsEditorModel(pointTemplate, site, normalPriorityDictionary, alarmPriorityDictionary, alarmCategoryDictionary);
			model.HasModifyRight = this.Security.HasRight(RIGHT.MODIFY_POINT_COMMANDSTATUS_LIST);

			return PartialViewWithErrorMessages("DeviceAlarmMapsEditor", model, JsonRequestBehavior.AllowGet);
		}


		[HttpPost, ValidateJsonAntiForgeryToken]
		public ActionResult SaveDeviceAlarmMaps(string editorEntries, Guid pointTemplateGuid, List<string> deletedDeviceAlarmMapLists)
		{

			var DeviceAlarmMaps = new DeviceAlarmMaps();

			PointTemplate pointTemplate = null;

			try
			{
				pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));


				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(editorEntries))
				{
					var editorRawDeviceAlarmMapList = jss.Deserialize<List<DeviceAlarmMap>>(editorEntries);

					pointTemplate.DeviceAlarmMaps = new DeviceAlarmMaps(editorRawDeviceAlarmMapList);
				}

				// update the point template with the new device alarm map
				FMChannelHelper.MakeCall<IPointTemplates>(x => x.Modify(this.Security, pointTemplate));

				this.AddSuccess("Save Successful");

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}


			return this.JsonWithErrorMessages(null);
		}


		public static void ValidateDeviceAlarmMapEntry(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, Point point, PointProperty property, List<PointDefaultUnitChangeHistory> defaultUnitConversionHistory)
		{

		}

		[NonAction]
		public static Dictionary<Guid, string> GetAllAlarmCategories(SecurityClass security)
		{
			var alarmCategoriesDictionary = new Dictionary<Guid, string>();
			var appStrDict = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
					x => x.EnumerateByType(security, STRING_TYPE.ALARM_EVENT_CATEGORY));
			for (var i = 0; i < appStrDict.Count; i++)
			{
				alarmCategoriesDictionary.Add(appStrDict[i].IdentityGuid, appStrDict[i].ID);
			}

			return alarmCategoriesDictionary;
		}


		[NonAction]
		public static Dictionary<Guid, AlarmPriorityClass> GetAllAlarmPriorities(SecurityClass security)
		{
			var alarmPriorityDictionary = new Dictionary<Guid, AlarmPriorityClass>();
			var alarmPriorityCollection = FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityCollectionClass>(x => x.Enumerate(security));
			foreach (var alarmPriority in alarmPriorityCollection)
			{
				alarmPriorityDictionary.Add(alarmPriority.IdentityGuid, alarmPriority);
			}

			return alarmPriorityDictionary;
		}
	}
}