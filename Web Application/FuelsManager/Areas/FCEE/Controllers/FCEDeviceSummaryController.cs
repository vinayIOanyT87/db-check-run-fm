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

    using FMBusinessObjects.Constants;

    using FuelsManager.FMWebApp;

    using global::FMWebApp;
    using System.Runtime.Serialization;
    using FuelsManager.Areas.FCEE.ViewModels;


    public class FCEDeviceSummaryController : FMBaseControllerEx
    {
        // GET: FCEE/FCEDevice
        public ActionResult FCEDeviceSummaryView()
        {

            var fceDevicesList = FMChannelHelper.MakeCall<IFCEDevices, List<FCEDevice>>(x => x.EnumerateBySiteGuid(this.Security, this.Security.SiteGuid));
            var model = new FCEDeviceSummaryModel(fceDevicesList);
            var menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;
            model.ReadOnly = !this.Security.HasRight(RIGHT.MODIFY_FCEE_DATA);

         return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete (string guidString)
        {
            try
            {
                var fceDeviceGuid = new Guid(guidString);
                FMChannelHelper.MakeCall<IFCEDevices>(x => x.Purge(this.Security, fceDeviceGuid));
            }
            catch (Exception except)
            {
                this.OnError(except);
            }
            return this.JsonWithErrorMessages(null);
        }
    }
}