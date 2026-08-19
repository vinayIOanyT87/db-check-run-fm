// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeNodeDiscovery.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.OrderEntryWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;

	using FuelsManager.Accounting;
	using FuelsManager.FMWebApp;

	/// <summary>
	/// This class is responsible for generating the tree node information for the main
	/// Order Entry pages.
	/// </summary>
	public class LsiTreeNodeDiscoveryMain : FMFormBase, IMenuDiscovery
	{
		//*************************************************************************
		// Member variables
		//*************************************************************************    

		#region Constants and Fields

		private const string OrderEntryUrl = "../OrderEntryWebApp/";

		#endregion

		//*************************************************************************
		// Member functions
		//*************************************************************************    

		#region Public Methods and Operators

		/// <summary>
		///     Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///     List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            var timer = new StopWatch(StopWatch.Appnames.OrderEntry, "OrderEntry.GetMenuItems()");
            if (useNewLicenseKey == 1)
            {
                if ((word1 & 0x10) != 0x10)
                    return null;
            }
            else
            {
                if (this.CheckHardwareKey(options) == false)
                {
                    timer.Error("OrderEntry.GetMenuItems(): Hardware Key Options field is null. (Old FMUtil version?)");
                    return null;
                }
            }
            if (security.HasRight(RIGHT.VIEW_ORDERS) == false
                && security.HasRight(RIGHT.MODIFY_ORDERS) == false
                && security.HasRight(RIGHT.CREATE_ORDERS) == false)
            {
                return null;
            }

            var items = new List<FMMenuItem>();

			if (!security.HasViewTransactionRightByTransTypeID(TransactionTypes.T17_Order))
			{
				return null;
			}

         if (security.HasRight(RIGHT.VIEW_ORDERS) || security.HasRight(RIGHT.MODIFY_ORDERS))
         {
               items.Add(
               new FMMenuItem
               {
                  MenuItemType = FMMenuItemType.OPERATIONS_SALES_SALES_ORDER_SUMMARY,
                  RootMenuName = "Operations",
                  CategoryName = "Sales",
                  ItemName = "Sales Order Summary",
                  NavigateUrl = OrderEntryUrl + "OrderSummary.aspx",
                  ApplyDataDictionary = ApplyDataDictionary.Apply,
                  SortOrder = 999 // Have this be at the end
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

		#endregion

		#region Methods

		/// <summary>
		///     This method will add order nodes to the tree view. If there are no transaction
		///     aliases, then it just return without adding nodes.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="security"></param>
		protected void AddNewFMMenuItemOrderNodes(List<FMMenuItem> parent, SecurityClass security)
		{
            if (security.HasRight(RIGHT.MODIFY_ORDERS) == false && security.HasRight(RIGHT.CREATE_ORDERS) == false)
            {
                return;
            }
            if (!security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T17_Order))
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

			if (aliasNames.Find(x => x.TransTypeID == TransactionTypes.T17_Order) == null)
			{
				return;
			}

			// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

			foreach (TransactionAliasNameClass alias in aliasNames)
			{
				if (alias.TransTypeID == TransactionTypes.T17_Order
				    && security.HasModifyTransactionRightByAliasName(alias.AliasName))
				{

					//AddNode(addTransNode, alias.AliasName, "../" + transactionDetailURL + "?" +
					//    TransactionDetailBase.ModeKey + "=ADD&TransAlias=" + alias.AliasName, false);
					parent.Add(
						new FMMenuItem
							{
								MenuItemType = FMMenuItemType.DYNAMIC_ADD_SALES_ORDER,
								RootMenuName = "Operations",
								CategoryName = "Sales",
								ItemName = alias.AliasName,
								NavigateUrl =
									"../" + transactionDetailUrl + "?" + TransactionDetailBase.ModeKey + "=ADD&TransAlias=" + alias.AliasName,
								ApplyDataDictionary = ApplyDataDictionary.Apply,
								DynamicMenuItemGuid = alias.IdentityGuid,
								SortOrder = 1 // Have these come before summary
							});
				}
			}
		}

		protected bool CheckHardwareKey(uint options)
		{
			// Must have accounting and order entry keys
			bool bOrderEntry = (options & 0x1000000) == 0x1000000;
			bool bAccounting = (options & 0x80000) == 0x80000;

			// Check for Order Entry key in hardware options
			return (bOrderEntry && bAccounting);
		}

		#endregion
	}
}
