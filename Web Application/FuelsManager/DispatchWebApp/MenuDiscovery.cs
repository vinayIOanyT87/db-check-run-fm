// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuDiscovery.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Menu discovery class file for Dispatch.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Contains menu discovery code for Dispatch.
	/// </summary>
	public class MenuDiscovery : IMenuDiscovery
	{
		/// <summary>
		/// Identifies the data dictionary keys needed for this page.
		/// </summary>
		/// <param name="security">The current security object.</param>
		/// <returns>An array of data dictionary keys.</returns>
		public string[] Keys(SecurityClass security)
		{
			string[] keys = 
			{
				"Grid Columns",
				"Settings",
				"Toolbars",
				"Validations",
				"Release To Accounting",
				"Standby Status Board",
				"Dispatch",
				"Dispatch Control Log",
				"Dispatchers List",
				"Flight Line Status Display"
			};

			return keys;
		}

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
            if (useNewLicenseKey == 1)
            {
                if ((word1 & 0x40) != 0x40)
                    return null;
            }
            else
            {
                if ((options & 0x1000) != 0x1000)
                    return null;
            }

			// Enforce certain security to access this menu -- Remember: Modify implies View in the HasRight() check
			if (security.HasRight(RIGHT.VIEW_DISPATCH) == false)
			{
				return null;
			}

			var items = new List<FMMenuItem>
				{
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DISPATCH_CONFIG_GRID_COLUMNS,
							RootMenuName = "Dispatch",
							CategoryName = "Configuration",
							ItemName = "Grid Columns",
							NavigateUrl = "../DispatchWebApp/DispatchGridColumnConfigurationPage.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						},
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DISPATCH_CONFIG_SETTINGS,
							RootMenuName = "Dispatch",
							CategoryName = "Configuration",
							ItemName = "Settings",
							NavigateUrl = "../DispatchWebApp/DispatchSettingsConfigurationPage.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						},
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DISPATCH_CONFIG_TOOLBARS,
							RootMenuName = "Dispatch",
							CategoryName = "Configuration",
							ItemName = "Toolbars",
							NavigateUrl = "../DispatchWebApp/DispatchToolbarConfigurationPage.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						},
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DISPATCH_OPERATION_RELEASE_TO_ACCOUNTING,
							RootMenuName = "Dispatch",
							CategoryName = "Operations",
							ItemName = "Release To Accounting",
							// The showReleaseToAccounting flag is passed to the page as a query string.
							NavigateUrl = "../DispatchWebApp/TabularView.aspx?showReleaseToAccounting=true",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						},
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DISPATCH_OPERATION_STANDBY_STATUS_BOARD,
							RootMenuName = "Dispatch",
							CategoryName = "Operations",
							ItemName = "Standby Status Board",
							NavigateUrl = "../DispatchWebApp/TabularView.aspx?triggerStandbyStatusBoard=true",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						},
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DISPATCH_VIEW_CONTROL_LOG,
							RootMenuName = "Dispatch",
							CategoryName = "Views",
							ItemName = "Dispatch Control Log",
							NavigateUrl = "../DispatchWebApp/ControlLogForm.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						},
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DISPATCH_VIEW_DISPATCHERS_LIST,
							RootMenuName = "Dispatch",
							CategoryName = "Views",
							ItemName = "Dispatchers List",
							NavigateUrl = "../DispatchWebApp/ListOfDispatchers.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						},
					new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DISPATCH_VIEW_TABULAR_VIEW,
							RootMenuName = "Dispatch",
							CategoryName = "Views",
							ItemName = "Dispatch",
							NavigateUrl = "../DispatchWebApp/TabularView.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						},
					new FMMenuItem
					{
						MenuItemType = FMMenuItemType.DISPATCH_VIEW_FLIGHT_LINE_STATUS_DISPLAY,
						RootMenuName = "Dispatch",
						CategoryName = "Views",
						ItemName = "Flight Line Status Display",

						// The dispatchStatus filter parameter is passed to the page as a query string.
						NavigateUrl = "../DispatchWebApp/DispatchingView.aspx?dispatchStatus=FlightLine",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					},
				};

			if (security.HasRight(RIGHT.CONFIGURE_DISPATCH_VALIDATIONS))
			{
				items.Add(new FMMenuItem
				{
					MenuItemType = FMMenuItemType.DISPATCH_CONFIG_VALIDATIONS,
					RootMenuName = "Dispatch",
					CategoryName = "Configuration",
					ItemName = "Validations",
					NavigateUrl = "../DispatchWebApp/DispatchValidationsConfigurationPage.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				});
			}

			return items;
		}
	}
}
