// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinanceTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FinanceTreeNav type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FinanceWebApp
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// The finance menu navigation.
	/// </summary>
	public class FinanceTreeNav : IMenuDiscovery
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
			// Added call to HasProfessionalSetting determine if Finacne Menu is displayed.  See CSI #5366 fore more
			// details.
			var menuItems = new List<FMMenuItem>();

			//TODO: Temporary commented out so that QA does not test financial configuration features.
			//bool isDescProfessionalKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescProfessionalKey());

			//if (this.HasHardwareKey(options) && 
			//	(this.HasViewPermissions(security) || this.HasModifyPermissions(security)) &&
			//	!isDescProfessionalKey )
			//{
			//	const string FinanceAppUrl = "../FinanceWebApp/";

			//	// Create the Price List, Currency, and Tax configuration nodes under the
			//	// accounting main node.
			//	if (security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) || security.HasRight(RIGHT.VIEW_STANDING_OFFERS))
			//	{
			//		var financeMenuItem = new FMMenuItem
			//			{
			//				MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_STANDING_OFFER_PRICES,
			//				RootMenuName = "Configuration",
			//				CategoryName = "Accounting",
			//				ItemName = "Price List",
			//				NavigateUrl = FinanceAppUrl + "StandingOfferPriceForm.aspx",
			//				ApplyDataDictionary = ApplyDataDictionary.Apply
			//			};
			//		menuItems.Add(financeMenuItem);
			//	}

			//	if (security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) || security.HasRight(RIGHT.VIEW_FINANCIAL_DATA))
			//	{
			//		var currencyConfigMenuItem = new FMMenuItem
			//			{
			//				MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_CURRENCIES,
			//				RootMenuName = "Configuration", 
			//				CategoryName = "Accounting", 
			//				ItemName = "Currencies",
			//				NavigateUrl = FinanceAppUrl + "CurrenciesForm.aspx"
			//			};
			//		var gstTaxConfigMenuItem = new FMMenuItem
			//			{
			//				MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_GST_TAX,
			//				RootMenuName = "Configuration",
			//				CategoryName = "Accounting",
			//				ItemName = "GST Tax",
			//				NavigateUrl = FinanceAppUrl + "TaxRateGstSummaryForm.aspx"
			//			};
			//		var exciseTaxConfigMenuItem = new FMMenuItem
			//			{
			//				MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_EXCISE_TAX,
			//				RootMenuName = "Configuration",
			//				CategoryName = "Accounting",
			//				ItemName = "Excise Tax",
			//				NavigateUrl = FinanceAppUrl + "TaxRateExciseSummaryForm.aspx"
			//			};
			//		var markupConfigMenuItem = new FMMenuItem
			//			{
			//				MenuItemType = FMMenuItemType.CONFIG_ACCOUNTING_MARKUP,
			//				RootMenuName = "Configuration",
			//				CategoryName = "Accounting",
			//				ItemName = "Markup",
			//				NavigateUrl = FinanceAppUrl + "TaxRateMarkupSummaryForm.aspx"
			//			};

			//		menuItems.Add(currencyConfigMenuItem);
			//		menuItems.Add(gstTaxConfigMenuItem);
			//		menuItems.Add(exciseTaxConfigMenuItem);
			//		menuItems.Add(markupConfigMenuItem);
			//	}
			//}

			return menuItems;
		}

		/// <summary>
		/// This method will return true if there is a valid hardware key for Enterprise Reports. Otherwise, it will return false. The key is located in the upper word of a 32 bit word and the value is 0x10.
		/// </summary>
		/// <param name="options">The Options.</param>
		/// <returns>
		/// The has hardware key.
		/// </returns>
		public bool HasHardwareKey(uint options)
		{
			bool hasKey = (options & 0x100000) != 0;

			return hasKey;
		}

		/// <summary>
		/// This method will determine if the user has modify permissions. If so, the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>
		/// The has modify permissions.
		/// </returns>
		public bool HasModifyPermissions(SecurityClass security)
		{
			return security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) || security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA);
		}

		/// <summary>
		/// This method will determine if the user has view only permissions. If so, the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>
		/// The has view permissions.
		/// </returns>
		public bool HasViewPermissions(SecurityClass security)
		{
			return security.HasRight( RIGHT.VIEW_STANDING_OFFERS ) || security.HasRight( RIGHT.VIEW_FINANCIAL_DATA );
		}
	}
}