namespace FuelsManager.FMReportWebMain
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public class ReportingConfigTreeNav : IMenuDiscovery
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
			var menuItems = new List<FMMenuItem>();
			var helper = new ReportingTreeNavHelper();

            if (useNewLicenseKey == 1)
            {
                if ((word1 & 0x04) != 0x04)
                    return null;
            }
            else
            {
                if (!helper.HasHardwareKey(options))
                    return null;
            }

            // Determine if the user has configuration privileges. If so, then display the configuration
            // node.
            if (helper.HasConfigurationPermissions(security))
			{
				const string ReportUrl = "../FMReportWebMain/";

				var reportDetailtMenuItem = new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_REPORTS_QUERIES_REPORT_ASSIGNMENT,
						RootMenuName = "Configuration",
						CategoryName = "Reports/Queries",
						ItemName = "Report Assignment",
						NavigateUrl = ReportUrl + "ReportConfigurationSettingsPage.aspx",
                        ApplyDataDictionary = ApplyDataDictionary.Apply
                };
				var reportGroupsMenuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_REPORTS_QUERIES_REPORT_GROUPS,
					RootMenuName = "Configuration",
					CategoryName = "Reports/Queries",
					ItemName = "Report Groups",
					NavigateUrl = ReportUrl + "ReportConfigurationSettingsPage.aspx?Group=Yes",
                    ApplyDataDictionary = ApplyDataDictionary.Apply
                };

				menuItems.Add(reportDetailtMenuItem);
				menuItems.Add(reportGroupsMenuItem);
			}

			return menuItems;
		}
	}

}

