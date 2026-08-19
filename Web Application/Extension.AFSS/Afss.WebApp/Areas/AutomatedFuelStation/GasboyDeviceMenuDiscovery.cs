// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDeviceMenuDiscovery.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Adds menu items for the Gasboy Station functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	/// Adds menu items for the Gasboy Device functionality
	/// </summary>
	public class GasboyDeviceMenuDiscovery : IMenuDiscovery
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
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, uint options)
		{
			var items = new List<FMMenuItem>();

			var externalStationSummaryMenu = GetGasboyDeviceSummaryMenu(security);

			if (externalStationSummaryMenu != null)
			{
				items.Add(externalStationSummaryMenu);
			}

			return items;
		}

		/// <summary>
		/// Get the Gasboy Device Summary Menu Item
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>The Gasboy Device Summary Menu Item if the user has proper privileges</returns>
		private static FMMenuItem GetGasboyDeviceSummaryMenu(SecurityClass security)
		{
			// No need to check Modify rights in addition to this because the
			// HasRight() call will check the Modify right as an implied View right.
			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_AUTOMATED_FUEL_SERVICE_DEVICES,
				RootMenuName = "Configuration",
				CategoryName = "Other",
				ItemName = "Payment Cards",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AutomatedFuelStation/GasboyDevice/GasboyDeviceSummaryIndex",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply
			};
		}
	}
}