// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountingTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   A class which provides menu items for certain accounting configuration operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Configuration;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Provides a vehicle for generating nodes for Accounting menus.
	/// </summary>
	public class AccountingTreeNav : FMFormBase, IMenuDiscovery
	{
		/// <summary>
		/// Root directory for location of FM Web App nodes.
		/// </summary>
		protected const string FuelsManagerUrl = "../FMWebApp/";

		/// <summary>
		/// Gets or sets the string used for storing a root directory location for accounting nodes.
		/// </summary>
		protected string AccountingUrl { get; set; }

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		List<FMMenuItem> IMenuDiscovery.GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            var logger = new Logger("Accounting");
            if (useNewLicenseKey == 1)
            {
                if ((word1 & 0x10) != 0x10)
                    return null;
            }
            else
            {
                if (this.GetHardwareKey(options) == false)
                {
                    logger.Error("AccountingTreeNav.GetLeftViewTreeNode(): Hardware Key Options field is null. (Old FMUtil version?)");
                    return null;
                }
            }

            var startTime = DateTime.Now;
	
			this.GetWebAppName();

			var menuItems = new List<FMMenuItem>();

			if (security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) || security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
			{
				if (security.HasRight(RIGHT.ACCESS_ACCOUNTING_LEDGER))
				{
					var ledgerMenuItem = new FMMenuItem
						{
							MenuItemType = FMMenuItemType.ACCOUNTING_MAIN_LEDGER,
							RootMenuName = "Accounting",
							CategoryName = "Main",
							ItemName = "Ledger",
							NavigateUrl = this.AccountingUrl + "Ledger.aspx",
							ApplyDataDictionary = ApplyDataDictionary.Apply
						};
					menuItems.Add(ledgerMenuItem);
				}

				if (!siteGroup)
				{
					// Addition of the Closeout Summary and Inventoty Reconciliation nodes are to be based on VIEW_CLOSEOUT_DATA right. (IGO 15-May-2009)
					if (security.HasRight(RIGHT.VIEW_CLOSEOUT_DATA))
					{
						var closeoutSummaryMenuItem = new FMMenuItem
							{
								MenuItemType = FMMenuItemType.ACCOUNTING_MAIN_CLOSEOUT_SUMMARY,
								RootMenuName = "Accounting",
								CategoryName = "Main",
								ItemName = "Closeout Summary",
								NavigateUrl = this.AccountingUrl + "Closeout.aspx",
								ApplyDataDictionary = ApplyDataDictionary.Apply
							};

						menuItems.Add( closeoutSummaryMenuItem );
					}

					if (security.HasRight(RIGHT.VIEW_INVENTORY_RECONCILIATION))
					{
						var inventoryReconciliationMenuItem = new FMMenuItem
						    {
							    MenuItemType =
								    FMMenuItemType.ACCOUNTING_MAIN_INVENTORY_RECONCILIATION,
							    RootMenuName = "Accounting",
							    CategoryName = "Main",
							    ItemName = "Inventory Reconciliation",
							    NavigateUrl =
								    this.AccountingUrl + "InventoryReconciliation.aspx",
							    ApplyDataDictionary = ApplyDataDictionary.Apply
						    };

						menuItems.Add(inventoryReconciliationMenuItem);
					}
					// Add the Meter Reconciliation node, but only if this isn't a site group and the user has the associated security right
					if (security.HasRight(RIGHT.VIEW_METER_RECONCILIATION))
					{
						var meterReconciliationMenuItem = new FMMenuItem
							{
								MenuItemType = FMMenuItemType.ACCOUNTING_MAIN_METER_RECONCILIATION,
								RootMenuName = "Accounting",
								CategoryName = "Main",
								ItemName = "Meter Reconciliation",
								NavigateUrl = this.AccountingUrl + "MeterReconciliationSummary.aspx",
								ApplyDataDictionary = ApplyDataDictionary.Apply
							};
						
						menuItems.Add(meterReconciliationMenuItem);
					}

					if (security.HasRight(RIGHT.PERFORM_AUTO_DISTRIBUTION))
					{
						var queryParameters = new NameValueCollection 
						{
							{
								AutoDistributionOperationPage.UrlParamOperationType, 
								AutoDistributionOperationTypes.Manual.ToString()
							}
						};

						var autoDistributionMenuItem = new FMMenuItem
						{
							MenuItemType = FMMenuItemType.ACCOUNTING_AUTO_DISTRIBUTION,
							RootMenuName = "Accounting",
							CategoryName = "Main",
							ItemName = AutoDistributionOperationPage.MenuName,
							NavigateUrl = this.FMFormatUrl(this.AccountingUrl + AutoDistributionOperationPage.PageUrl, queryParameters),
							ApplyDataDictionary = ApplyDataDictionary.Apply
						};
						menuItems.Add(autoDistributionMenuItem);
					}
				}

				if (security.HasRight(RIGHT.ACCESS_ACCOUNTING_OPERATIONS))
				{
					if (!siteGroup)
					{
						var operationsMenuItem = new FMMenuItem
							{
								MenuItemType = FMMenuItemType.ACCOUNTING_MAIN_OPERATIONS,
								RootMenuName = "Accounting",
								CategoryName = "Main",
								ItemName = "Operations",
								NavigateUrl = this.AccountingUrl + "AccountingOperationsForm.aspx",
								ApplyDataDictionary = ApplyDataDictionary.Apply
							};
						menuItems.Add(operationsMenuItem);
					}
				}

                if (useNewLicenseKey == 1)
                {
                    this.AddTransactionMenuItems(security, siteGroup, menuItems, 0x1000000);
                }
                else
                {
                    this.AddTransactionMenuItems(security, siteGroup, menuItems, options);
                }

				this.AddImportExportMenuItems(security, menuItems);

                var stopTime = DateTimeOffset.Now;
				var elapsedTime = new TimeSpan(stopTime.Ticks - startTime.Ticks);
				logger.Perform("AccountingTreeNav.GetLeftViewTreeNode() completed in " + elapsedTime.ToString() + ".");
			}

            this.AddStdTranImportMenuItem(security, menuItems);

            // Only show accounting node if any items added to accounting node
            if (menuItems.Count == 0)
			{
				return null;
			}

			return menuItems;
		}

        private void AddStdTranImportMenuItem(SecurityClass security, List<FMMenuItem> menuItems)
        {
            if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
                && !security.HasRight(RIGHT.IMPORT_TRANSACTION) )
            {
                return;
            }

            var menuItem = new FMMenuItem
            {
                MenuItemType = FMMenuItemType.ACCOUNTING_STANDARD_IMPORT_TRANSACTION_DATA,
                RootMenuName = "Accounting",
                CategoryName = "Import/Export",
                ItemName = "Standard Transaction Import",
                NavigateUrl = this.AccountingUrl + "StandardTransactionImportInterface.aspx",
                //ApplyDataDictionary = ApplyDataDictionary.Apply,
                SortOrder = 3
            };
            menuItems.Add(menuItem);
        }

        /// <summary>
        /// Adds the transaction menu items.
        /// </summary>
        /// <param name="security">The security.</param>
        /// <param name="siteGroup">if set to <c>true</c> [site group].</param>
        /// <param name="menuItems">The menu items.</param>
        /// <param name="options">The options.</param>
        private void AddTransactionMenuItems(SecurityClass security, bool siteGroup, List<FMMenuItem> menuItems, uint options)
		{
			TransactionAliasNameCollectionClass aliasCollection;

			if (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false)
			{
				return;
			}

			if (this.Page.Session[FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION] == null)
			{
				return;
			}
			
			try
			{
				aliasCollection = (TransactionAliasNameCollectionClass)this.Page.Session[FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION];
			}
			catch (Exception)
			{
				return;
			}

			if ((aliasCollection == null) || (aliasCollection.Count == 0))
			{
				return;
			}
			
			foreach (TransactionAliasNameClass alias in aliasCollection)
			{
				if (siteGroup)
				{
					if (alias.TransTypeID != TransactionTypes.T9_Request
						&& alias.TransTypeID != TransactionTypes.T18_SupplyOrder)
					{
						continue;
					}
				}

				if (!security.HasModifyTransactionRightByAliasName(alias.AliasName))
				{
					continue;
				}

				// Security around orders
				if (alias.TransTypeID == TransactionTypes.T17_Order
					&& CheckOrderSecurity(security, options) == false)
				{
					continue;
				}

				if (alias.TransTypeID == TransactionTypes.T18_SupplyOrder
					&& CheckSupplyOrderSecurity(security, options) == false)
				{
					continue;
				}

				// Escape the alias name for any URL special characters (i.e. & ' / ? ! # $ * + , : ; = @ [ ])
				string aliasName = Uri.EscapeDataString(alias.AliasName);

				// Read the TransactionDetail URL from the Web.config file (01-Jul-2009 IGO)
				var transactionDetailUrl = string.Format(
					"../{0}?{1}=ADD&TransAlias={2}", 
					ConfigurationManager.AppSettings["AccountingTransactionDetailURL"], 
					TransactionDetailBase.ModeKey, 
					aliasName);

				var transactionMenuItem = new FMMenuItem
					{
						MenuItemType = FMMenuItemType.DYNAMIC_ADD_TRANSACTION,
						RootMenuName = "Accounting",
						CategoryName = "Add Transaction",
						ItemName = alias.AliasName,
						NavigateUrl = transactionDetailUrl,
						DynamicMenuItemGuid = alias.IdentityGuid,
						ApplyDataDictionary = ApplyDataDictionary.Apply
					};
				menuItems.Add(transactionMenuItem);
			}
		}

		/// <summary>
		/// Adds the import/export menu items.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="menuItems">The menu items.</param>
		private void AddImportExportMenuItems(SecurityClass security, List<FMMenuItem> menuItems)
		{
			if (security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false)
			{
				return;
			}

			var pluginSr = new ImportExportPluginSR
				{
					Security = security, 
					Site = security.SiteID
				};

			var pluginDo = FMChannelHelper.MakeCall<IImportExportPluginProcessor, ImportExportPluginDO>(x => x.Process(pluginSr));

			var importSr = new ImportSR
				{
					Site = security.SiteID, 
					Security = security
				};

			ImportExportListDO importDo = FMChannelHelper.MakeCall<IImportProcessor, ImportExportListDO>(
				x => x.Process(importSr));
			
			if (importDo.ImportExportList.Count > 0)
			{
				foreach (ImportExportListItemDO item in importDo.ImportExportList)
				{
					string url = string.Empty;
					foreach (ImportExportPluginItemDO plugin in pluginDo.PluginList)
					{
						if (plugin.PluginType == item.PluginType)
						{
							url = string.Format("{0}?Site={1}&Name={2}", plugin.RunURL, security.SiteGuid, item.DisplayName);
							break;
						}
					}

					// This needs the dynamic GUID set from somewhere...
					var menuItem = new FMMenuItem
						{
							MenuItemType = FMMenuItemType.DYNAMIC_ACCOUNTING_IMPORT_EXPORT,
							RootMenuName = "Accounting",
							CategoryName = "Import/Export",
							ItemName = item.DisplayName,
							NavigateUrl = url,
							ApplyDataDictionary = ApplyDataDictionary.Apply,
							DynamicMenuItemGuid = item.ImportExportConfigGuid
						};
					menuItems.Add(menuItem);
				}
			}
		}

		/// <summary>
		/// Checks security for order entry system.
		/// </summary>
		/// <param name="security">FuelsManager SecurityClass</param>
		/// <param name="options">Options value from the hardware key.</param>
		/// <returns>True if the hardware key has Order Entry turned on.</returns>
		public static bool CheckOrderSecurity(SecurityClass security, uint options)
		{
			bool result = (options & 0x1000000) == 0x1000000; // check key for order entry system
            result = result && (security.HasRight(RIGHT.CREATE_ORDERS) || security.HasRight(RIGHT.MODIFY_ORDERS));
            return result;
		}

		/// <summary>
		/// Checks security for supply order entry system.
		/// </summary>
		/// <param name="security">FuelsManager SecurityClass</param>
		/// <param name="options">Options value from the hardware key.</param>
		/// <returns>True if the hardware key has Supply Order Entry turned on.</returns>
		public static bool CheckSupplyOrderSecurity(SecurityClass security, uint options)
		{
			bool result = (options & 0x1000000) == 0x1000000; // check key for order entry system
            result = result && (security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) || security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS));
            return result;
		}

		/// <summary>
		/// Checks security for adjustment permissions on hardware key.
		/// </summary>
		/// <param name="security">FuelsManager SecurityClass</param>
		/// <param name="options">Options value from the hardware key.</param>
		/// <returns>True if the hardware key has Adjustment permissions.</returns>
		protected bool CheckAdjustmentSecurity(SecurityClass security, uint options)
		{
			bool result = (options & 0x80000) == 0x80000;	// check key for enterprise accounting
			return result;
		}

		/// <summary>
		/// Initializes the AccountingURL root.
		/// </summary>
		protected void GetWebAppName()
		{
			this.AccountingUrl = "../Accounting/";
		}

		/// <summary>
		/// Determines if the hardwarekey options include accounting permissions.
		/// </summary>
		/// <param name="options">Options value from the hardware key.</param>
		/// <returns>Returns true if hardware key includes accounting permissions.</returns>
		protected bool GetHardwareKey(uint options)
		{
			if (( options & 0x80000 ) == 0x80000)
			{
				return true;
			}

			return false;
		}
	}
}
