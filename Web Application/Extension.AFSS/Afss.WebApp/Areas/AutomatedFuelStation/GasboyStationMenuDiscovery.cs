// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationMenuDiscovery.cs" company="Varec, Inc.">
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
	/// Adds menu items for the AutomatedFuelStation functionality
	/// </summary>
	public class GasboyStationMenuDiscovery : IMenuDiscovery
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

			var externalStationSummaryMenu = GetGasboyStationSummaryMenu(security);

			if (externalStationSummaryMenu != null)
			{
				items.Add(externalStationSummaryMenu);
			}

			var externalStationLogSummaryMenu = GetGasboyStationLogSummaryMenu(security);

			if (externalStationLogSummaryMenu != null)
			{
				items.Add(externalStationLogSummaryMenu);
			}

			var externalStationDataImportMenu = GetGasboyStationDataImportMenu(security);

			if (externalStationDataImportMenu != null)
			{
				items.Add(externalStationDataImportMenu);
			}

			var externalStationImportBlacklistMenu = GetGasboyStationImportBlacklistMenu(security);

			if (externalStationImportBlacklistMenu != null)
			{
				items.Add(externalStationImportBlacklistMenu);
			}

			var externalStationOperationsMenu = GetGasboyStationOperationsMenu(security);

			if (externalStationOperationsMenu != null)
			{
				items.Add(externalStationOperationsMenu);
			}

			var externalStationFailedTransactionsMenu = GetGasboyStationFailedTransactionsMenu(security);

			if (externalStationFailedTransactionsMenu != null)
			{
				items.Add(externalStationFailedTransactionsMenu);
			}

			var externalStationGeneralConfigurationMenu = GetGasboyStationGeneralConfigurationMenu(security);

			if (externalStationGeneralConfigurationMenu != null)
			{
				items.Add(externalStationGeneralConfigurationMenu);
			}

			return items;
		}

		/// <summary>
		/// Get the AutomatedFuelStations Summary Menu Item
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>The AutomatedFuelStations Summary Menu Item if the user has proper privileges</returns>
		private static FMMenuItem GetGasboyStationSummaryMenu(SecurityClass security)
		{
			// No need to check Modify rights in addition to this because the
			// HasRight() call will check the Modify right as an implied View right.
			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_AUTOMATED_FUEL_SERVICE_STATIONS,
				RootMenuName = "Configuration",
				CategoryName = "Automated Fuel Stations",
				ItemName = "Gasboy Stations",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AutomatedFuelStation/GasboyStation/GasboyStationSummaryIndex",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply
			};
		}

		/// <summary>
		/// Get the Fueling Stations Log Summary Menu Item
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>The Fueling Stations Log Summary Menu Item if the user has proper privileges</returns>
		private static FMMenuItem GetGasboyStationLogSummaryMenu(SecurityClass security)
		{
			// No need to check Modify rights in addition to this because the
			// HasRight() call will check the Modify right as an implied View right.
			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_LOG,
				RootMenuName = "Operations",
				CategoryName = "System Logs",
				ItemName = "Gasboy Station Logs",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AutomatedFuelStation/GasboyStationLog/GasboyStationLogSummaryIndex",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply
			};
		}

		/// <summary>
		/// Get the AutomatedFuelStations Data Import Menu Item
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>The AutomatedFuelStations Data Import Menu Item if the user has proper privileges</returns>
		private static FMMenuItem GetGasboyStationDataImportMenu(SecurityClass security)
		{
			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_DATA_IMPORT,
				RootMenuName = "Configuration",
				CategoryName = "Import/Export",
				ItemName = "Gasboy Station Data Import",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AutomatedFuelStation/GasboyStation/GasboyStationDataImport",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply
			};
		}

		/// <summary>
		/// Get the AutomatedFuelStations Data Import Menu Item
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>The AutomatedFuelStations Data Import Menu Item if the user has proper privileges</returns>
		private static FMMenuItem GetGasboyStationImportBlacklistMenu(SecurityClass security)
		{
			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_IMPORT_BLACKLIST,
				RootMenuName = "Configuration",
				CategoryName = "Import/Export",
				ItemName = "Payment Card Import",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AutomatedFuelStation/GasboyStation/GasboyStationImportBlacklist",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply
			};
		}

		/// <summary>
		/// Get the AutomatedFuelStations Operations Menu Item
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>The AutomatedFuelStations Operations Menu Item if the user has proper privileges</returns>
		private static FMMenuItem GetGasboyStationOperationsMenu(SecurityClass security)
		{
			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_OPERATIONS,
				RootMenuName = "Operations",
				CategoryName = "Operations",
				ItemName = "Gasboy Station Operations",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AutomatedFuelStation/GasboyStation/GasboyStationOperationsIndex",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply
			};
		}

		/// <summary>
		/// Get the AutomatedFuelStations Failed Transactions Menu Item
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>The AutomatedFuelStations Failed Transactions Menu Item if the user has proper privileges</returns>
		private static FMMenuItem GetGasboyStationFailedTransactionsMenu(SecurityClass security)
		{
			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_FAILED_TRANSACTIONS,
				RootMenuName = "Operations",
				CategoryName = "Operations",
				ItemName = "Gasboy Station Failed Transactions",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AutomatedFuelStation/GasboyStation/GasboyStationFailedTransactionSummaryIndex",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply
			};
		}

		/// <summary>
		/// Get the AutomatedFuelStations General Configuration menu Item
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <returns>The AutomatedFuelStations General Configuration menu Item if the user has proper privileges</returns>
		private static FMMenuItem GetGasboyStationGeneralConfigurationMenu(SecurityClass security)
		{
			// No need to check Modify rights in addition to this because the
			// HasRight() call will check the Modify right as an implied View right.
			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return null;
			}

			return new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_AUTOMATED_FUEL_SERVICE_STATION_GENERAL_CONFIGURATION,
				RootMenuName = "Configuration",
				CategoryName = "Automated Fuel Stations",
				ItemName = "Gasboy Station General Configuration",
				NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../AutomatedFuelStation/GasboyStation/GasboyStationGeneralConfiguration",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply
			};
		}
	}
}