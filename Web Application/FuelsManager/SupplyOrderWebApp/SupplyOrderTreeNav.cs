// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeNodeDiscovery.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This class is responsible for generating the tree node information for the main
//   Order Entry pages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.SupplyOrderWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;

	using FMWebApp;

	using FuelsManager.Accounting;

	using global::FMWebApp;

	/// <summary>
	/// This class is responsible for generating the tree node information for the main
	/// Order Entry pages.
	/// </summary>
	public class SupplyOrderTreeNav : FMFormBase, IMenuDiscovery
	{

		//*************************************************************************
		// Member variables
		//*************************************************************************    

		private const string SupplyorderUrl = "../SupplyOrderWebApp/";

		//*************************************************************************
		// Member functions
		//*************************************************************************    

		protected bool CheckHardwareKey(uint options)
		{
			// Must have accounting and order entry keys
			bool bSupplyOrder = (options & 0x1000000) == 0x1000000;
			bool bAccounting = (options & 0x80000) == 0x80000;

			// Check for Order Entry key in hardware options
			return (bSupplyOrder && bAccounting);
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
            var timer = new StopWatch(StopWatch.Appnames.SupplyOrderWebApp, "SupplyOrderWebApp.GetMenuItems()");
            if (useNewLicenseKey == 1)
            {
                if ((word1 & 0x10) != 0x10)
                    return null;
            }
            else
            {
                // Does the user have a hardware key that allows Order Entry?
                if (this.CheckHardwareKey(options) == false)
                {
                    timer.Error("SupplyOrderWebApp.GetMenuItems(): Hardware Key Options field is null. (Old FMUtil version?)");
                    return null;
                }
            }
            if (security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) == false
                && security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS) == false
                && security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) == false)
            {
                return null;
            }
            var items = new List<FMMenuItem>();

			if (!security.HasViewTransactionRightByTransTypeID(TransactionTypes.T18_SupplyOrder))
			{
				return null;
			}

            if (security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) || security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS))
            {
                // Add the Order Summary nodes
                items.Add(new FMMenuItem
                {
                    MenuItemType = FMMenuItemType.OPERATIONS_PROCUREMENT_SUPPLY_ORDER_SUMMARY,
                    RootMenuName = "Operations",
                    CategoryName = "Procurement",
                    ItemName = "Supply Order Summary",
                    NavigateUrl = SupplyorderUrl + "SupplyOrderSummary.aspx",
                    ApplyDataDictionary = ApplyDataDictionary.Apply,
                    SortOrder = 999 // Have this come after the transactions
                });
            }
            // Add "Add New Order" nodes
            this.AddNewFMMenuItemOrderNodes(items, security);

			// If they had no rights at all, don't allow the system to display the main node
			if (items.Count == 0)
			{
				return null;
			}

			timer.Stop();

			return items;
		}

		private void AddNewFMMenuItemOrderNodes(List<FMMenuItem> items, SecurityClass security)
		{
            if (security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS) == false && security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) == false)
            {
                return;
            }

            if (!security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T18_SupplyOrder))
			{
				return;
			}

			TransactionAliasNameCollectionClass aliasNames;

			if (this.Page.Session[FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION] == null)
			{
				return;
			}
			
			try
			{
				aliasNames =
					(TransactionAliasNameCollectionClass)this.Page.Session[FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION];
			}
			catch (Exception)
			{
				return;

			}

			if ((aliasNames == null) || (aliasNames.Count == 0))
			{
				return;
			}

			if (aliasNames.Find(x => x.TransTypeID == TransactionTypes.T18_SupplyOrder) == null)
			{
				return;
			}
			
			foreach (TransactionAliasNameClass alias in aliasNames)
			{
				if (alias.TransTypeID == TransactionTypes.T18_SupplyOrder &&
					security.HasModifyTransactionRightByAliasName(alias.AliasName))
				{
					string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];
					items.Add(new FMMenuItem
					{
						MenuItemType = FMMenuItemType.DYNAMIC_OPERATIONS_PROCUREMENT,
						RootMenuName = "Operations",
						CategoryName = "Procurement",
						ItemName = alias.AliasName,
						NavigateUrl = "../" + transactionDetailUrl + "?" + TransactionDetailBase.ModeKey + "=ADD&TransAlias=" + alias.AliasName,
						ApplyDataDictionary = ApplyDataDictionary.Apply,
						DynamicMenuItemGuid = alias.IdentityGuid,
						SortOrder = 1	// Have these come before the summary
					});
				}
			}
		}
	}
}
