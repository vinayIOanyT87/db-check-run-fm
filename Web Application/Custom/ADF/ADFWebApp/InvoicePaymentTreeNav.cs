/// <summary>
/// File name:	InvoiceTreeNav.cs
/// Purpose:	Used by Shared Components left tree navigation to discover
///				the Invoice tree structure.
///				
/// Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2007.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///				
/// Author(s):	Richard R. Panachida
/// Version:	1.0.0  Current version
///	
/// Modification History:
///   Date:			By:						Reason:
///   ----------  --------------------	----------------------------------
///   2007-11-15  Richard Panachida		Intial Revision.
///   2008-09-04  Eric Simmons			Added "Add New Invoice" subnode to support CSI #6125 
///												(ADF FPS PARA NO. 3.4.9.3.19.  TR-ITT-0065 in ADF_Techincal_Req.doc)
///	2009-01-12  A. Coker             Added code that to add/remove Recovery and Payment nodes to left tree view based on
///	                                 modify and view security rights for payment and recovery transactions. (defects 732 and 966) 
///	2009-01-27  A. Coker             Fixed defect 1159. Display Invoice to site groups only.
///	
///   2009-03-17  G.Kendall            WI# 1416 - Use new alias name list class to speed performance
///   
///   2009-03-31  Richard Panachida    Defect 2660: Added code to retrieve the transaction alias
///                                    collection from session. It is retrieve by FMLeftView.
///                                    
///   2009-03-31  A. Coker             CR 4500: Remove nodes that have no child nodes and 
///                                    request a page.
/// 
///                                    
///	2009-07-06	I.Orndorff				- Modified "AddInvoices()" to read the TransactionDetail URL from the 
///												  Web.config file. This addresses task 4585.
///												  
///	2009-07-07	I.Orndorff				- Renamed class from TransactionDetail to TransactionDetailBase. This addresses task 4585.
///													  
/// </summary>
using System;
using System.Configuration;
using System.Collections.Generic;
using Accounting;
using FMControls;
using System.Web.UI;
using Microsoft.Web.UI.WebControls;
using FMWebApp;

using FMBusinessObjects.Interfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace ADFWebApp
{
	public class InvoicePaymentTreeNav : FMFormBase, IMenuDiscovery
	{
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, uint options)
		{
			var menuItems = new List<FMMenuItem>();

			if (siteGroup && (this.HasHardwareKey(options) == true) &&
				((security.HasRight(RIGHT.VIEW_FINANCIAL_DATA) == true) || (security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == true)) &&
				(FMChannelHelper.MakeCall<IHardwareKey, bool>(x =>x.IsDescProfessionalKey() ) == false) &&
				security.SiteID.ToUpper().Equals("JFLA") // only appear at JFLA
				)
			{
				string invoiceUrl = "../InvoiceWebApp/";
				string baseUrl = Common.ADFWEBAPP_URL;
				string summaryUrl = InvoicePaymentSummary.FILENAME;

				bool hasPayableViewRights = security.HasViewTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice);
				bool hasReceivableViewRights = security.HasViewTransactionRightByTransTypeID(TransactionTypes.T22_AccountReceivableInvoice);

				if (!hasPayableViewRights && !hasReceivableViewRights)
				{
					return null;
				}

				if (security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice))
				{
					menuItems.Add(new FMMenuItem
						{
							MenuItemType = FMMenuItemType.ACCOUNTING_INVOICE_ENTRY_BULK_PAYMENT_SUMMARY,
							RootMenuName = "Accounting",
							CategoryName = "Invoice Entry",
							ItemName = "Bulk Payment Summary",
							NavigateUrl = baseUrl + summaryUrl + "?mode=" + (int)InvoicePaymentMode.PAYMENT,
							ApplyDataDictionary = ApplyDataDictionary.Apply
						});
				}

				if (hasPayableViewRights)
				{
					menuItems.Add(new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ACCOUNTING_INVOICE_ENTRY_INVOICE_SUMMARY,
						RootMenuName = "Accounting",
						CategoryName = "Invoice Entry",
						ItemName = "Invoice Summary",
						NavigateUrl = baseUrl + summaryUrl + "?mode=" + (int)InvoicePaymentMode.INVOICE,
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});
				}

				if (hasReceivableViewRights)
				{
					// recovery just use base functionality
					menuItems.Add(new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ACCOUNTING_INVOICE_ENTRY_RECEIVABLE_SUMMARY_ADF,
						RootMenuName = "Accounting",
						CategoryName = "Invoice Entry",
						ItemName = "Invoice Receivable Summary",
						NavigateUrl = invoiceUrl + "InvoiceSummary.aspx?invoicetype=receivable",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});
				}

				//Added by Eric Simmons on 9-3-3008 to resolve CSI #6125
				this.AddFMMenuItemInvoices(ref menuItems, baseUrl, security);
			}

			return menuItems;
		}

		/// <summary>
		/// This method will add all the invoice tranasaction types to the Invoice Entry menu
		/// </summary>
		/// <param name="menuItems"></param>
		/// <param name="invoiceAppURL"></param>
		/// <param name="security"></param>
		private void AddFMMenuItemInvoices(ref List<FMMenuItem> menuItems, string invoiceAppURL, SecurityClass security)
		{
			if (
				  (!security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T21_AccountPayableInvoice)
				&& !security.HasModifyTransactionRightByTransTypeID(TransactionTypes.T22_AccountReceivableInvoice))
			 || security.HasRight(RIGHT.MODIFY_FINANCIAL_DATA) == false)
			{
				return;
			}

			TransactionAliasNameCollectionClass aliasNames = null;

			if (Page.Session[FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION] == null)
			{
				return;
			}
			else
			{
				try
				{
					aliasNames = (TransactionAliasNameCollectionClass)Page.Session[FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION];
				}
				catch (Exception)
				{
					return;
				}
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
			string transactionDetailURL;
			transactionDetailURL = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

			foreach (TransactionAliasNameClass alias in aliasNames)
			{
				if (alias.TransTypeID == TransactionTypes.T21_AccountPayableInvoice ||
					 alias.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
				{
					if (security.HasModifyTransactionRightByAliasName(alias.AliasName))
					{
						//AddNode(addInvoce, alias.AliasName, "../" + transactionDetailURL + "?" +
						//   TransactionDetailBase.ModeKey + "=ADD&TransAlias=" + alias.AliasName, false);
						menuItems.Add(new FMMenuItem()
						{
							MenuItemType = FMMenuItemType.DYNAMIC_ACCOUNTING_INVOICE_ENTRY_ADF,
							RootMenuName = "Accounting",
							CategoryName = "Invoice Entry",
							ItemName = alias.AliasName,
							NavigateUrl = "../" + transactionDetailURL + "?" + TransactionDetailBase.ModeKey + "=ADD&TransAlias=" + alias.AliasName,
							ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
							DynamicMenuItemGuid = alias.IdentityGuid,
							SortOrder = 1	// Put these before the summary items
						});
					}
				}
			}
		}


		//Eric Simmons
		//Added on 11-19-2007 to resolve CSI #5366
		/// <summary>
		/// This method will return a true value if the machine is configured with a FuelsManager Defense Professional Key.
		/// </summary>
		/// <param name="SpecialKeyCodes"></param>
		/// <returns></returns>
		public bool HadProfessionalSetting ( uint SpecialKeyCodes )
		{
			bool hasSetting = true;
			if (( SpecialKeyCodes & 0x00000020 ) == 0)
			{
				hasSetting = false;
			}
			return hasSetting;
		}

		/// <summary>
		/// This method will return true if there is a valid hardware key for Enterprise Reports.
		/// Otherwise, it will return false. The key is located in the upper word of a 32 bit word 
		/// and the value is 0x10.
		/// </summary>
		/// <returns></returns>
		public bool HasHardwareKey ( uint Options )
		{
			bool hasKey = true;

			if (( Options & 0x100000 ) == 0)
			{
				hasKey = false;
			}

			return hasKey;
		}
	}
}
