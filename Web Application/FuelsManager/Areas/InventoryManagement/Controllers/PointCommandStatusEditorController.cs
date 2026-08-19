namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using System.Web.Script.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;


	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	using System.Globalization;

	public class PointCommandStatusEditorController : FMBaseControllerEx
	{

		[HttpGet, ValidateJsonAntiForgeryToken]
		public ActionResult PointCommandStatusEditor(Guid pointTemplateGuid)
		{
			PointTemplate pointTemplate = null;
			SiteClass site = null;

			try
			{
				// we need ot know what format we are going to use, we need to use the point information
				pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));
				site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
			}
			var model = new PointCommandStatusEditorModel(pointTemplate, site);
			model.HasModifyRight = this.Security.HasRight(RIGHT.MODIFY_POINT_COMMANDSTATUS_LIST);

			return PartialViewWithErrorMessages("PointCommandStatusEditor", model, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public ActionResult SavePointCommandStatus(string editorEntries, Guid pointTemplateGuid, List<string> deletedPointCommandStatusLists)
		{

			var pointCommandStatus = new PointCommandStatus();

			PointTemplate pointTemplate = null;

			try
			{
				pointTemplate = FMChannelHelper.MakeCall<IPointTemplates, PointTemplate>(x => x.Get(this.Security, pointTemplateGuid));

				var pointCommandStatuses = new List<PointCommandStatusList>();

				// convert the editor entries into a list (had problems with the default MVC binder automatically doing it so I need to do it manually )
				JavaScriptSerializer jss = new JavaScriptSerializer();
				if (!string.IsNullOrEmpty(editorEntries))
				{
					var editorRawPointCommandStatusList = jss.Deserialize<List<EditorPointCommandStatusList>>(editorEntries);

					// remove duplicates for the same key (use the last value) and then sort by key entry
					for (int pointCommandStatusIndex = 0; pointCommandStatusIndex < editorRawPointCommandStatusList.Count; pointCommandStatusIndex++)
					{
						var editorRawPointCommandStatus = editorRawPointCommandStatusList[pointCommandStatusIndex];

						var individualPointCommandStatusList = new PointCommandStatusList();

						foreach (var editorRawRow in editorRawPointCommandStatus.PointCommandStatusEntries)
						{
							individualPointCommandStatusList.CommandStatusList.Add(
								new PointCommandStatusList.CommandStatusElement(editorRawRow.KeyEntry, Convert.ToInt32(editorRawRow.ValueEntry)));
						}
						individualPointCommandStatusList.ID = editorRawPointCommandStatus.CommandStatusListID;
						individualPointCommandStatusList.CommandStatusListGuid = editorRawPointCommandStatus.CommandStatusListGuid;

						pointCommandStatuses.Add(individualPointCommandStatusList);

					}
				}

				pointCommandStatus.CommandStatusLists = pointCommandStatuses;
				pointTemplate.PointCommandStatus = pointCommandStatus;

				// update the point template with the new point command
				FMChannelHelper.MakeCall<IPointTemplates>(x => x.Modify(this.Security, pointTemplate));

				// if we have deleted point command lists make sure we delete dependencies
				if (deletedPointCommandStatusLists != null)
				{
					FMChannelHelper.MakeCall<IPointTemplates>(x => x.PointCommandStatusListDeleted(this.Security, pointTemplateGuid, deletedPointCommandStatusLists));

				}
				this.AddSuccess("Save Successful");

			}
			catch (Exception except)
			{
				this.OnError(except);
				return this.JsonWithErrorMessages(null);
			}


			return this.JsonWithErrorMessages(null);
		}


		public static void ValidatePointCommandStatusEntry(ModelStateDictionary modelState, NumberFormatInfo numberFormatInfo, Point point, PointProperty property, List<PointDefaultUnitChangeHistory> defaultUnitConversionHistory)
		{

		}
	}
}