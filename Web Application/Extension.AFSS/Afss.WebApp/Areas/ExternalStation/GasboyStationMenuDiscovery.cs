// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationMenuDiscovery.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Adds menu items for the External Station functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.WebApp.Areas.ExternalStation
{
    using System.Collections.Generic;

    using FMBusinessObjects.DataObjects;

    using global::FMWebApp;

    /// <summary>
    /// Adds menu items for the External Station functionality
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
        /// Get the External Stations Summary Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Summary Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetGasboyStationSummaryMenu(SecurityClass security)
        {
            // No need to check Modify rights in addition to this because the
            // HasRight() call will check the Modify right as an implied View right.
            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION))
            {
                return null;
            }

            return new FMMenuItem
            {
                MenuItemType = FMMenuItemType.CONFIG_OTHER_EXTERNAL_STATIONS,
                RootMenuName = "Configuration",
                CategoryName = "External Stations",
                ItemName = "Gasboy Stations",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../GasboyStation/GasboyStation/GasboyStationSummaryIndex",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations Log Summary Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Log Summary Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetGasboyStationLogSummaryMenu(SecurityClass security)
        {
            // No need to check Modify rights in addition to this because the
            // HasRight() call will check the Modify right as an implied View right.
            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION))
            {
                return null;
            }

            return new FMMenuItem
            {
                MenuItemType = FMMenuItemType.OPERATIONS_EXTERNAL_STATION_LOG,
                RootMenuName = "Operations",
                CategoryName = "System Logs",
                ItemName = "Gasboy Station Logs",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStation/GasboyStationLog/GasboyStationLogSummaryIndex",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations Data Import Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Data Import Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetGasboyStationDataImportMenu(SecurityClass security)
        {
            if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
            {
                return null;
            }

            return new FMMenuItem
            {
                MenuItemType = FMMenuItemType.CONFIG_IMPORT_EXPORT_EXTERNAL_STATION_DATA_IMPORT,
                RootMenuName = "Configuration",
                CategoryName = "Import/Export",
                ItemName = "Gasboy Station Import",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStation/GasboyStation/GasboyStationDataImport",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations Operations Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Operations Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetGasboyStationOperationsMenu(SecurityClass security)
        {
            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION))
            {
                return null;
            }

            return new FMMenuItem
            {
                MenuItemType = FMMenuItemType.OPERATIONS_EXTERNAL_STATION_OPERATIONS,
                RootMenuName = "Operations",
                CategoryName = "Operations",
                ItemName = "Gasboy Station Operations",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStation/GasboyStation/GasboyStationOperationsIndex",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations Failed Transactions Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Failed Transactions Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetGasboyStationFailedTransactionsMenu(SecurityClass security)
        {
            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION))
            {
                return null;
            }

            return new FMMenuItem
            {
                MenuItemType = FMMenuItemType.OPERATIONS_EXTERNAL_STATION_FAILED_TRANSACTIONS,
                RootMenuName = "Operations",
                CategoryName = "Operations",
                ItemName = "Gasboy Station Failed Transactions",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStation/GasboyStation/GasboyStationFailedTransactionSummaryIndex",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations General Configuration menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations General Configuration menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetGasboyStationGeneralConfigurationMenu(SecurityClass security)
        {
            // No need to check Modify rights in addition to this because the
            // HasRight() call will check the Modify right as an implied View right.
            if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION))
            {
                return null;
            }

            return new FMMenuItem
            {
                MenuItemType = FMMenuItemType.CONFIG_OTHER_EXTERNAL_STATION_GENERAL_CONFIGURATION,
                RootMenuName = "Configuration",
                CategoryName = "Other",
                ItemName = "Gasboy Station General Configuration",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStation/GasboyStation/GasboyStationGeneralConfiguration",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }
    }
}