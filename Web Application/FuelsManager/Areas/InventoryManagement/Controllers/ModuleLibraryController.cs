
namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.Web;
	using System.Web.Mvc;
	using System.Diagnostics;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;
	using FMBusinessObjects.Constants;

	using FuelsManager.FMWebApp;
	using FMBusinessObjects.LogClient;

	public class ModuleLibraryController : FMBaseControllerEx
	{
		/// <summary>
		/// The number of minutes we should give the module modify process to complete for each module before timing out.
		/// </summary>
		public const int ModuleModifyOperationTimeoutMinutes = 10;


		// GET: InventoryManagement/ModuleLibrary
		public ActionResult ModuleLibraryView()
		{
			var modDict = FMChannelHelper.MakeCall<IModules, Dictionary<Guid, Module>>(x => x.EnumerateBySiteGuid(this.Security, this.Security.SiteGuid));

			var model = new ModuleLibraryModel(this.Security.SiteGuid, modDict);
			var menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;
			string js = menuData.GetHelpUrl(true) + "CustomModuleProgrammersGuide.pdf";
			string jscript = "window.open('" + HttpUtility.JavaScriptStringEncode(js) + "')";
			model.GuideOpenerScript = new MvcHtmlString (jscript);

			model.ReadOnly = !this.Security.HasRight(RIGHT.MODIFY_MODULE_LIBRARY);

			return this.View(model);
		}

		/// <summary>
		/// This method will handle the delete action.
		/// </summary>
		/// <param name="id">The ID of the item to delete.</param>
		/// <returns>Returns any error message.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string id)
		{
			try
			{
				var moduleGuid = new Guid(id);
				FMChannelHelper.MakeCall<IModules>(x => x.Purge(this.Security, moduleGuid));
			}
			catch (Exception except)
			{
				this.OnError(except);
			}
			return this.JsonWithErrorMessages(null);
		}

		/// <summary>
		/// This method will handle the save all modules action.
		/// </summary>
		/// <returns>Returns any error message.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SaveAllModules()
		{
			try
			{
				var moduleDictionary = FMChannelHelper.MakeCall<IModules, Dictionary<Guid, Module>>(x => x.EnumerateBySiteGuid(this.Security, this.Security.SiteGuid));

                FMChannelHelper.MakeCall<IModules>(x => x.LogToAlarmAndEventLog(this.Security, "Executing Module Library Save All"));
                foreach (var module in moduleDictionary.Values)
				{
					FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(this.Security));
					FMChannelHelper.MakeCall<IModules>(x =>
					{
						((IClientChannel) x).OperationTimeout = new TimeSpan(0, ModuleModifyOperationTimeoutMinutes, 0);
						x.Modify(this.Security, module);
                    });
                }
            }
            catch (Exception except)
			{
                FMFormBase.LogErrorMessage(except.Message + (except.InnerException != null ? except.InnerException.Message : ""));
                FMChannelHelper.MakeCall<IModules>(x => x.LogToAlarmAndEventLog(this.Security, string.Format("Module Library Save All - {0}", except.Message + (except.InnerException != null ? except.InnerException.Message : ""))));

                this.OnError(except);
			}
			return this.JsonWithErrorMessages(null);
		}
	}
}