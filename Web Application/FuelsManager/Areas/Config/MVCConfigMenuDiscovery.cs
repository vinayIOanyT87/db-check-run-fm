namespace FuelsManager.Areas.Config
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public class MvcConfigMenuDiscovery : IMenuDiscovery
	{
		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
			var items = new List<FMMenuItem>();
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }


            if (!siteGroup)
			{
				return null;
			}

			// No need to check Modify rights in addition to this because the
			// HasRight() call will check the Modify right as an implied View right.
			if (security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS) == false)
			{
				return null;
			}

			items.Add(
				new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ADMIN_SYSTEM_CONFIGURATION_SETTINGS,
					RootMenuName = "Administration",
					CategoryName = "System",
					ItemName = "Configuration Settings",
					NavigateUrl =
						"../MenuBar/FMMenuBar.aspx?target=../Config/ConfigurationSettings/ConfigurationSettingsIndex",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				});


			return items;
		}
	}
}
