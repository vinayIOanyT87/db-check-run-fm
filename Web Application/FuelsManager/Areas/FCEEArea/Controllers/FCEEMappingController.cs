
namespace FuelsManager.Areas.FCEE.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.FCEE.ViewModels;
	using FMBusinessObjects.Constants;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;
	 using System.Runtime.Serialization;

	 public class FCEEMappingController : FMBaseControllerEx
	{
		// GET: 
		public ActionResult FCEEMappingView()
		{
			var fceeMappingDict = FMChannelHelper.MakeCall<IFCEE, Dictionary<Guid, Dictionary<Guid, Tuple<string, Guid, string, Guid, long>>>>(x => x.EnumerateBySiteGuid(this.Security, this.Security.SiteGuid));

			var model = fceeMappingModel(this.Security.SiteGuid, modDict);
			var menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;
			string js = menuData.GetHelpUrl(true) + "ADM037_v10.pdf";
			string jscript = "window.open('" + HttpUtility.JavaScriptStringEncode(js) + "')";
			model.GuideOpenerScript = new MvcHtmlString (jscript);

			model.ReadOnly = !this.Security.HasRight(RIGHT.MODIFY_MODULE_LIBRARY);

			return this.View(model);
		}

		/// <summary>
		/// This method will handle the delete action.
		/// </summary>
		/// <param name="id">The ID of the item to delete.</param>
		/// <returns>Returns the view.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(string id)
		{
			try
			{
				var mappingGuid = new Guid(id);
				FMChannelHelper.MakeCall<IFCEE>(x => x.Purge(this.Security, mappingGuid));
			}
			catch (Exception except)
			{
				this.OnError(except);
			}
			return this.JsonWithErrorMessages(null);
		}
	}
}