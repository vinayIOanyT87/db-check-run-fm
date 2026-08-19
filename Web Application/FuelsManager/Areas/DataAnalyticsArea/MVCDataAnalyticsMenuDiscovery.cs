
namespace FuelsManager.Areas.DataAnalyticsArea
{
    using FMBusinessObjects.DataObjects;
    using System.Collections.Generic;

    using FuelsManager.FMWebApp;
   using FMBusinessServices.ServiceClasses;
   using FMBusinessObjects.BusinessInterfaces;
   using FMBusinessObjects.ChannelFactories;

   public class MVCDataAnalyticsMenuDiscovery : IMenuDiscovery
    {
        /// <summary>
        /// Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="options">Hardware key options</param>
        /// <returns>
        /// List of menu items to be displayed
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
        {
            var items = new List<FMMenuItem>();

            var transactionSummary = this.GetDataAnalyticsViewerMenu(security);
            if (transactionSummary != null)
            {
                items.Add(transactionSummary);
            }

            return items;
        }

        private FMMenuItem GetDataAnalyticsViewerMenu(SecurityClass security)
        {
            if (security.HasRight(RIGHT.VIEW_DATA_ANALYTICS) == false)
            {
                return null;
            }
            bool isEnterpriseKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsEnterpriseKey());
            if (isEnterpriseKey == false)
            {
               return null;
            }
            bool isDataAnalyticsKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDataAnalyticsKey());

            return new FMMenuItem
            {
                MenuItemType = FMMenuItemType.DATA_ANALYTICS_VIEWER,
                RootMenuName = "Data Analytics",
                CategoryName = "Main",
                ItemName = (isDataAnalyticsKey ? "Data Analytics Viewer" : "About License"),
                NavigateUrl = (isDataAnalyticsKey ? "../MenuBar/FMMenuBar.aspx?target=../DataAnalyticsArea/DataAnalytics/DataAnalyticsViewer" : "../FMWebApp/AboutDatawarehouseLicense.aspx"),
                ApplyDataDictionary = ApplyDataDictionary.Apply
            };
        }
    }
}