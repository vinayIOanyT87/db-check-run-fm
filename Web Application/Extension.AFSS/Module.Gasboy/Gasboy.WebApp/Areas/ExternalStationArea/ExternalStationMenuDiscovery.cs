// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationMenuDiscovery.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Adds menu items for the External Station functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.WebApp.Areas.ExternalStationArea
{
    using System.Collections.Generic;

    using FMBusinessObjects.DataObjects;

    using global::FMWebApp;

    /// <summary>
    /// Adds menu items for the External Station functionality
    /// </summary>
    public class ExternalStationMenuDiscovery : IMenuDiscovery
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

            var externalStationSummaryMenu = GetExternalStationSummaryMenu(security);

            if (externalStationSummaryMenu != null)
            {
                items.Add(externalStationSummaryMenu);
            }

            var externalStationLogSummaryMenu = GetExternalStationLogSummaryMenu(security);

            if (externalStationLogSummaryMenu != null)
            {
                items.Add(externalStationLogSummaryMenu);
            }

            var externalStationDataImportMenu = GetExternalStationDataImportMenu(security);

            if (externalStationDataImportMenu != null)
            {
                items.Add(externalStationDataImportMenu);
            }

            var externalStationOperationsMenu = GetExternalStationOperationsMenu(security);

            if (externalStationOperationsMenu != null)
            {
                items.Add(externalStationOperationsMenu);
            }

            var externalStationFailedTransactionsMenu = GetExternalStationFailedTransactionsMenu(security);

            if (externalStationFailedTransactionsMenu != null)
            {
                items.Add(externalStationFailedTransactionsMenu);
            }

            var externalStationGeneralConfigurationMenu = GetExternalStationGeneralConfigurationMenu(security);

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
        private static FMMenuItem GetExternalStationSummaryMenu(SecurityClass security)
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
                CategoryName = "Other",
                ItemName = "External Stations",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStationArea/ExternalStation/ExternalStationSummaryIndex",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations Log Summary Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Log Summary Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetExternalStationLogSummaryMenu(SecurityClass security)
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
                ItemName = "External Station Log",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStationArea/ExternalStationLog/ExternalStationLogSummaryIndex",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations Data Import Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Data Import Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetExternalStationDataImportMenu(SecurityClass security)
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
                ItemName = "External Station Import",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStationArea/ExternalStation/ExternalStationDataImport",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations Operations Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Operations Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetExternalStationOperationsMenu(SecurityClass security)
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
                ItemName = "External Station Operations",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStationArea/ExternalStation/ExternalStationOperationsIndex",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations Failed Transactions Menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations Failed Transactions Menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetExternalStationFailedTransactionsMenu(SecurityClass security)
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
                ItemName = "External Station Failed Transactions",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStationArea/ExternalStation/ExternalStationFailedTransactionSummaryIndex",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }

        /// <summary>
        /// Get the External Stations General Configuration menu Item
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <returns>The External Stations General Configuration menu Item if the user has proper privileges</returns>
        private static FMMenuItem GetExternalStationGeneralConfigurationMenu(SecurityClass security)
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
                ItemName = "External Station General Configuration",
                NavigateUrl = "../MenuBar/FMMenuBar.aspx?target=../ExternalStationArea/ExternalStation/ExternalStationGeneralConfiguration",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply
            };
        }
    }
}