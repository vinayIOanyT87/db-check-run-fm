// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvoiceTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.InvoiceWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMWebApp;

	using FuelsManager.Accounting;

	using global::FMWebApp;

	public class InvoiceTreeNav : FMFormBase, IMenuDiscovery
	{
		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                if (!this.HasHardwareKey(options))
                    return null;
            }

			var items = new List<FMMenuItem>();

			if (Convert.ToBoolean(ConfigurationManager.AppSettings["SupressInvoiceWebAppMenu"]))
			{
				return null;
			}

			if (siteGroup && this.HasHardwareKey(0)
			    && (security.HasRight(RIGHT.VIEW_FINANCIAL_DATA) || security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA))
			    && (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescProfessionalKey()) == false))
			{
				const string InvoiceAppUrl = "../InvoiceWebApp/";

				if (security.HasViewTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice))
				{
					items.Add(
						new FMMenuItem
							{
								MenuItemType = FMMenuItemType.ACCOUNTING_INVOICE_ENTRY_INVOICE_PAYABLE_SUMMARY, 
								RootMenuName = "Accounting", 
								CategoryName = "Invoice Entry", 
								ItemName = "Invoice Payable Summary", 
								NavigateUrl = InvoiceAppUrl + "InvoiceSummary.aspx?invoicetype=payable", 
								ApplyDataDictionary = ApplyDataDictionary.Apply, 
								SortOrder = 999 // Put this after the Transaction Aliases
							});
				}

				if (security.HasViewTransactionRightByTransTypeID(TransactionTypes.T22_AccountReceivableInvoice))
				{
					items.Add(
						new FMMenuItem
							{
								MenuItemType = FMMenuItemType.ACCOUNTING_INVOICE_ENTRY_RECEIVABLE_SUMMARY, 
								RootMenuName = "Accounting", 
								CategoryName = "Invoice Entry", 
								ItemName = "Invoice Receivable Summary", 
								NavigateUrl = InvoiceAppUrl + "InvoiceSummary.aspx?invoicetype=receivable", 
								ApplyDataDictionary = ApplyDataDictionary.Apply, 
								SortOrder = 1000 // Put this after the Transaction Aliases
							});
				}

				// Added by Eric Simmons on 9-3-3008 to resolve CSI #6125
				this.AddFMMenuItemInvoices(ref items, security);
			}

			return items;
		}

		/// <summary>
		/// This method will return a true value if the machine is configured with a FuelsManager Defense Professional Key.
		/// </summary>
		/// <param name="specialKeyCodes">
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HadProfessionalSetting(uint specialKeyCodes)
		{
			bool hasSetting = (specialKeyCodes & 0x00000020) != 0;
			return hasSetting;
		}

		/// <summary>
		/// This method will add a node to the parent tree node.
		/// </summary>
		/// <param name="options">
		/// The Options.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		/// <summary>
		/// This method will return true if there is a valid hardware key for Enterprise Reports.
		///     Otherwise, it will return false. The key is located in the upper word of a 32 bit word
		///     and the value is 0x10.
		/// </summary>
		/// <returns>
		/// </returns>
		public bool HasHardwareKey(uint options)
		{
			bool hasKey = (options & 0x100000) != 0;
			return hasKey;
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method will add all the invoice tranasaction types to the Invoices Tree Node.
		///     Added by Eric Simmons on 9-3-3008 to resolve CSI #6125
		/// </summary>
		/// <param name="menuItems">
		/// The menu Items.
		/// </param>
		/// <param name="security">
		/// SecurityClass
		/// </param>
		private void AddFMMenuItemInvoices(ref List<FMMenuItem> menuItems, SecurityClass security)
		{
			if ((!security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice)
			     && !security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T22_AccountReceivableInvoice))
			    || security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
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

			if ((aliasNames.Find(x => x.TransTypeID == TransactionTypes.T21_AccountPayableInvoice) == null)
			    && (aliasNames.Find(x => x.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice) == null))
			{
				return;
			}

			// Read the TransactionDetail URL from the Web.config file (06-Jul-2009 IGO)
			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

			foreach (TransactionAliasNameClass alias in aliasNames)
			{
				if (alias.TransTypeID == TransactionTypes.T21_AccountPayableInvoice
				    || alias.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
				{
					if (security.HasModifyTransactionRightByAliasName(alias.AliasName))
					{
						// AddNode(addInvoce, alias.AliasName, "../" + transactionDetailURL + "?" +
						// TransactionDetailBase.ModeKey + "=ADD&TransAlias=" + alias.AliasName, false);
						menuItems.Add(
							new FMMenuItem
								{
									MenuItemType = FMMenuItemType.DYNAMIC_ACCOUNTING_INVOICE_ENTRY, 
									RootMenuName = "Accounting", 
									CategoryName = "Invoice Entry", 
									ItemName = alias.AliasName, 
									NavigateUrl =
										"../" + transactionDetailUrl + "?" + TransactionDetailBase.ModeKey + "=ADD&TransAlias=" + alias.AliasName, 
									ApplyDataDictionary = ApplyDataDictionary.Apply, 
									DynamicMenuItemGuid = alias.IdentityGuid, 
									SortOrder = 1 // Put these before the summary items
								});
					}
				}
			}
		}

		#endregion
	}
}
