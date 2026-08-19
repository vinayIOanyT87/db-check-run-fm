
namespace FuelsManager.Areas.DataAnalyticsArea.Controllers
{
    using System;
    using System.Web.Mvc;

    using Areas.Controllers;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
   using FMBusinessServices.ServiceClasses;
   using ViewModels;

   /// <summary>
   /// The controller for the data analytics page
   /// </summary>
    public class DataAnalyticsController : FMBaseController
    {
        /// <summary>
        /// The name of the setting in tblConfigurationSettings that represents the Data Analytics Server address. 
        /// </summary>
        private const string DataAnalyticsServerURLSettingName = "DataAnalyticsServerURL";

        /// <summary>
        /// The worksheet to load when the page starts up. 
        /// </summary>
        private const string MainWorksheetURL = "/views/Aviation/MainDashboard";

        /// <summary>
        /// The main Get action for the data analytics viewer page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DataAnalyticsViewer()
        {
            DataAnalyticsViewerModel model = new DataAnalyticsViewerModel();

            try
            {
            
               bool isDataAnalyticsKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDataAnalyticsKey());

               if (!isDataAnalyticsKey)
               {
                  throw new Exception("License key does not include Data Analytics.");
               }
               // Grab the first part of the URL from tblConfigurationSettings
               model.DataAnalyticsServerUrl =
                       FMChannelHelper.MakeCall<IConfigurationSettings, string>(
                           configurationSettings =>
                           configurationSettings.GetKeyValueByKey(this.Security, DataAnalyticsServerURLSettingName));

                // The second part of the URL, which contains the worksheet or dashboard we want to display first, is hard coded. This could be configurable.
                model.MainWorksheetUrl = MainWorksheetURL;
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return this.View(model);
        }
    }
}