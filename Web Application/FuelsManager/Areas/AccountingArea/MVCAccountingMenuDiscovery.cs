namespace FuelsManager.Areas.AccountingArea
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Accounting;

	using FuelsManager.FMWebApp;

	public class MvcAccountingMenuDiscovery : IMenuDiscovery
	{
		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems( SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
		{
			var items = new List<FMMenuItem>();
            var transactionSummary = this.GetTransactionSummaryMenu(security, word1,useNewLicenseKey);
            if (transactionSummary != null)
            {
                items.Add(transactionSummary);
            }

			this.GetTransactionEditorMenu(security, items, siteGroup, options,word1,word2,useNewLicenseKey);

			return items;
		}

		private void GetTransactionEditorMenu(SecurityClass security, List<FMMenuItem> items, bool isSiteGroup, uint options, ushort word1, ushort word2, ushort useNewLicenseKey)
		{
            if(useNewLicenseKey == 1)
            {

            }
            else
            {

            }
			var txEditor = AppSettingsHelper.GetKeyValue( "TransactionEditor", false );
			if ( txEditor == false )
			{
				return;
			}

			if ( security.HasRight( RIGHT.MODIFY_TRANSACTION_DATA ) == false )
			{
				return;
			}

			var sr = new TransactionAliasListSR
			         {
				         CurrentSiteGuid = security.SiteGuid,
						 GetOwnerSiteID = false,
						 Security = security,
						 Site = security.SiteID
			         };

			var aliasCollection = FMChannelHelper.MakeCall<ITransactionAliasListProcessor, TransactionAliasListDO>(x => x.Process(sr));

			if ( ( aliasCollection == null ) || ( aliasCollection.aliasList.Count == 0 ) )
			{
				return;
			}

			foreach ( TransactionAliasClass alias in aliasCollection.Values )
			{
				if ( isSiteGroup )
				{
					if ( alias.TransTypeID != TransactionTypes.T9_Request
						&& alias.TransTypeID != TransactionTypes.T18_SupplyOrder )
					{
						continue;
					}
				}

				if ( !security.HasModifyTransactionRightByAliasName( alias.ID ) )
				{
					continue;
				}

				// Security around orders
				if ( alias.TransTypeID == TransactionTypes.T17_Order
					&& AccountingTreeNav.CheckOrderSecurity( security, options ) == false )
				{
					continue;
				}

				if ( alias.TransTypeID == TransactionTypes.T18_SupplyOrder
					&& AccountingTreeNav.CheckSupplyOrderSecurity( security, options ) == false )
				{
					continue;
				}

				// Escape the alias name for any URL special characters (i.e. & ' / ? ! # $ * + , : ; = @ [ ])
				string aliasName = Uri.EscapeDataString( alias.ID );

				// Read the TransactionDetail URL from the Web.config file
				var transactionDetailUrl = string.Format(
					"../{0}?{1}=ADD&TransAlias={2}",
					ConfigurationManager.AppSettings["AccountingTransactionDetailURL"],
					TransactionDetailBase.ModeKey,
					aliasName );

				var url = "../MenuBar/FMMenuBar.aspx?target=../AccountingArea/TransactionEditor/TransactionEditorAdd/" + alias.ID;

				var transactionMenuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.DYNAMIC_ACCOUNTING_TRANSACTION_EDITOR,
					RootMenuName = "Accounting",
					CategoryName = "Add Transaction",
					ItemName = "<b>*" + alias.ID + "*</b>",
					NavigateUrl = url,
					DynamicMenuItemGuid = alias.IdentityGuid,
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

				items.Add( transactionMenuItem );
			}
		}

		private FMMenuItem GetTransactionSummaryMenu(SecurityClass security, ushort word1, ushort useNewLicenseKey)
		{
            if(useNewLicenseKey == 1 && (word1 & 0x10) != 0x10)
                return null;

			var transSummary = AppSettingsHelper.GetKeyValue( "TransactionSummary", true );
			if ( transSummary == false )
			{
				return null;
			}

			// No need to check Modify rights in addition to this because the
			// HasRight() call will check the Modify right as an implied View right.
			if ( security.HasRight( RIGHT.VIEW_TRANSACTION_DATA ) == false )
			{
				return null;
			}

			return new FMMenuItem
			       {
				       MenuItemType = FMMenuItemType.ACCOUNTING_MAIN_TRANSACTION_SUMMARY,
				       RootMenuName = "Accounting",
				       CategoryName = "Main",
				       ItemName = "Transaction Summary",
				       NavigateUrl =
					       "../MenuBar/FMMenuBar.aspx?target=../AccountingArea/TransactionSummary/TransactionSummaryIndex",
				       ApplyDataDictionary = ApplyDataDictionary.Apply
			       };
		}
	}
}
