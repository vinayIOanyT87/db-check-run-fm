namespace FuelsManager.Areas.UserAdministrationArea
{
    using System.Collections.Generic;

    using FMBusinessObjects.DataObjects;

    using FuelsManager.FMWebApp;

    public class MvcUserAdministrationMenuDiscovery : IMenuDiscovery
    {
        /// <summary>
        ///    Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="useNewLicenseKey"></param>
        /// <param name="options">Hardware key options</param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <returns>
        ///    List of menu items to be displayed
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
        {
            var items = new List<FMMenuItem>();

            //List<FMMenuItem> mapViewMenuItems = this.GetViewMapMenu(security);

            //if (mapViewMenuItems != null && mapViewMenuItems.Count > 0)
            //{
            //    foreach (FMMenuItem menuItem in mapViewMenuItems)
            //    {
            //        items.Add(menuItem);
            //    }
            //}

            //if (iconConfigurationMenuItem != null)
            //{
            //    items.Add(iconConfigurationMenuItem);
            //}

            return items;
        }
    }
}