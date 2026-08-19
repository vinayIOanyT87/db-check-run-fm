// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountingConfigTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   A class which provides menu items for certain accounting configuration operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// This class provides menu items for certain accounting operations.
	/// </summary>
	public class AccountingConfigTreeNav : AccountingTreeNav, IMenuDiscovery
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
		List<FMMenuItem> IMenuDiscovery.GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            bool accountingaccessdenied = false;
            bool mobilaccessdenied = false;
            if (useNewLicenseKey == 1)
            {
                if ((word1 & 0x10) != 0x10)
                    accountingaccessdenied = true;
                if ((word1 & 0x200) != 0x200)
                    mobilaccessdenied = true;
            }
            else
            {
                if (this.GetHardwareKey(options) == false)
                {
                    return null;
                }
            }

            this.GetWebAppName();

			if (security.HasRight(RIGHT.CONFIGURE_ACCOUNTING) == false)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

            if (accountingaccessdenied == false)
                this.AddSystemDatesMenuItem(security, menuItems);
            if (mobilaccessdenied == false)
                this.AddImportExportConfigurationMenuItem(security, menuItems);
            // pcarpenter ExStars
            this.AddIrsExstarsMenuItem(security, menuItems);
            if(mobilaccessdenied == false)
                this.AddIntoPlaneImportMenuItem(security, menuItems);
            if (accountingaccessdenied == false)
                this.AddGeneralConfigurationMenuItem(security, menuItems);
            if(accountingaccessdenied == false)
			    this.AddLedgerAggregateColumnsMenuItem(security, menuItems);
            if (accountingaccessdenied == false)
                this.AddLedgerViewsMenuItem(security, menuItems);
            if (accountingaccessdenied == false)
                this.AddAutoDistributionConfigMenuItems(security, menuItems);

			return menuItems;
		}

		/// <summary>
		/// Adds the system dates menu item.
		/// </summary>
		/// <param name="security">The FuelsManager security.</param>
		/// <param name="menuItems">The menu items collection to which to add the menu item.</param>
		private void AddSystemDatesMenuItem(SecurityClass security, List<FMMenuItem> menuItems)
		{
			// JS20100913 WI-17722 Never show lock dates for ADF because it is not relevant - in ADF we can never backdate transactions
			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			    || FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey())
			    || FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
			{
				return;
			}

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_LOCK_DATES,
					RootMenuName = "Configuration",
					CategoryName = "Accounting",
					ItemName = "Lock Dates",
					NavigateUrl = this.AccountingUrl + "SystemDates.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};
			menuItems.Add(menuItem);
		}

		/// <summary>
		/// Adds the import/export configuration menu item.
		/// </summary>
		/// <param name="security">The FuelsManager security class.</param>
		/// <param name="menuItems">The menu items collection to which the item will be added.</param>
		private void AddImportExportConfigurationMenuItem(SecurityClass security, List<FMMenuItem> menuItems)
		{
			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
			{
				return;
			}

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_IMPORT_EXPORT,
					RootMenuName = "Accounting",
					CategoryName = "Import/Export",
					ItemName = "Import/Export",
					NavigateUrl = this.AccountingUrl + "ImportExportConfiguration.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply,
					SortOrder = 1
				};
			menuItems.Add(menuItem);
		}

		private void AddIrsExstarsMenuItem(SecurityClass security, List<FMMenuItem> menuItems)
		{
			if (!security.HasRight(RIGHT.VIEW_IRS_EXSTARS_REPORT))
			{
				return;
			}

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_IRS_EXSTARS,
				RootMenuName = "Accounting",
				CategoryName = "Import/Export",
				ItemName = "IRS/ExSTARS",
				NavigateUrl = this.AccountingUrl + "ExStarsForm.aspx",
				//ApplyDataDictionary = ApplyDataDictionary.Apply,
				SortOrder = 2
			};
			menuItems.Add(menuItem);
		}

        /// <summary>
        /// Add the IRS ExSTARS reporting to the menu
        /// </summary>
        /// <param name="security"></param>
        /// <param name="menuItems"></param>
        private void AddIntoPlaneImportMenuItem(SecurityClass security, List<FMMenuItem> menuItems)
        {
			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
            {
                return;
            }

            var menuItem = new FMMenuItem
            {
                MenuItemType = FMMenuItemType.ACCOUNTING_IMPORT_INTOPLANE_DATA,
                RootMenuName = "Accounting",
                CategoryName = "Import/Export",
                ItemName = "Import IntoPlane Data",
                NavigateUrl = this.AccountingUrl + "IntoPlaneImportWebPage.aspx",
                //ApplyDataDictionary = ApplyDataDictionary.Apply,
                SortOrder = 4
            };
            menuItems.Add(menuItem);
        }


		/// <summary>
		/// Adds the general configuration menu item.
		/// </summary>
		/// <param name="security">The FuelsManager security class.</param>
		/// <param name="menuItems">The menu items collection to which the item will be added.</param>
		private void AddGeneralConfigurationMenuItem(SecurityClass security, List<FMMenuItem> menuItems)
		{
			if (!security.HasRight(RIGHT.CONFIGURE_ACCOUNTING))
			{
				return;
			}

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_GENERAL_CONFIGURATION,
					RootMenuName = "Configuration",
					CategoryName = "Accounting",
					ItemName = "General Configuration",
					NavigateUrl = this.AccountingUrl + "GeneralConfiguration.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};
			menuItems.Add(menuItem);
		}

		/// <summary>
		/// Adds the ledger aggregate columns menu item.
		/// </summary>
		/// <param name="security">The FuelsManager security class.</param>
		/// <param name="menuItems">The menu items collection to which the item will be added.</param>
		private void AddLedgerAggregateColumnsMenuItem(SecurityClass security, List<FMMenuItem> menuItems)
		{
			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return;
			}

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_LEDGER_AGGREGATE_COLUMNS,
					RootMenuName = "Configuration",
					CategoryName = "Accounting",
					ItemName = "Ledger Aggregate Columns",
					NavigateUrl = this.AccountingUrl + "LedgerAggregateColumnsForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};
			menuItems.Add(menuItem);
		}

		/// <summary>
		/// Adds the ledger views menu item.
		/// </summary>
		/// <param name="security">The FuelsManager security class.</param>
		/// <param name="menuItems">The menu items collection to which the item will be added.</param>
		private void AddLedgerViewsMenuItem(SecurityClass security, List<FMMenuItem> menuItems)
		{
			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return;
			}

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_LEDGER_VIEWS,
					RootMenuName = "Configuration",
					CategoryName = "Accounting",
					ItemName = "Ledger Views",
					NavigateUrl = this.AccountingUrl + "LedgerViewsForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};
			menuItems.Add(menuItem);
		}

		/// <summary>
		/// Adds the auto distribution config menu items.
		/// </summary>
		/// <param name="security">The FuelsManager security class.</param>
		/// <param name="menuItems">The menu items collection to which the item will be added.</param>
		private void AddAutoDistributionConfigMenuItems(SecurityClass security, List<FMMenuItem> menuItems)
		{
			if (security.HasRight(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION) == false && security.HasRight(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION) == false)
			{
				return;
			}

			var ruleConfigMenuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_AUTO_DISTRIBUTION_RULES,
					RootMenuName = "Configuration",
					CategoryName = "Accounting",
					ItemName = "Auto Distribution Rules",
					NavigateUrl = this.AccountingUrl + AutoDistributionRulesForm.PageUrl,
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};
			var reasonConfigMenuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_AUTO_DISTRIBUTION_REASONS,
					RootMenuName = "Configuration",
					CategoryName = "Accounting",
					ItemName = "Auto Distribution Reasons",
					NavigateUrl = this.AccountingUrl + "AutoDistributionReasonCodesForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};
			menuItems.Add(ruleConfigMenuItem);
			menuItems.Add(reasonConfigMenuItem);
		}
	}
}