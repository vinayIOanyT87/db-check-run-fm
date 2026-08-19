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
    public class FCEEMessagesSummaryController : FMBaseControllerEx
    {
        // GET: FCEE/FCEESummary
        public ActionResult FCEEMessagesSummaryView(int? page, string imei, int? msgType, int? index, string startDate, string endDate)
        {
            int pageNumber = page ?? 1;
            pageNumber = Math.Max(pageNumber, 1);

            var fceeMessagesList = FMChannelHelper.MakeCall<IFCEEServiceManager, List<FCEEMessage>>(x => x.EnumerateMessages(this.Security, startDate, endDate));
            var model = new FCEEMessagesSummaryModel(fceeMessagesList, pageNumber);

            if (!string.IsNullOrEmpty(startDate))
            {
                model.startDate = startDate;
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                model.endDate = endDate;
            }

            if (!string.IsNullOrEmpty(imei))
            {
                model.FCEEMessagesFilterByImei(imei);
                model.imei = imei;
            }
            
            if (index != null)
            {
                model.FCEEMessagesFilterByIndex(index);
                model.index = index;
            }

            if (msgType != null && msgType != -1)
            {
                model.FCEEMessagesFilterByType(msgType);
                model.message = msgType;
            }
           
            model.pageMax = Math.Min((int)Math.Ceiling((double)model.fceeMessages.Count / 100), 10);

            model.page = Math.Min(model.pageMax, model.page);
            
            model.FCEEMessagesPage();

            var menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;
            string js = menuData.GetHelpUrl(true) + "CustomModuleProgrammersGuide.pdf";
            string jscript = "window.open('" + HttpUtility.JavaScriptStringEncode(js) + "')";
            if (!TryValidateModel(model))
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                string msg = string.Empty;
                foreach (var errList in errors)
                {
                    foreach (var err in errList)
                    {
                        msg += string.Format("{0}{1}", err.ErrorMessage, Environment.NewLine);
                    }
                }
                this.ErrorHandler(new Exception(msg));
            }
            return this.View(model);
        }
    }
}