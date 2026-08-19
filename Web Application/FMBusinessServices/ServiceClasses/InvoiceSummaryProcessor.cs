 #pragma warning disable 1587
/// <summary>
/// File name:	InvoiceSummaryProcessor.cs
/// Purpose:	The purpose of the Invoice Summary Processor is to handle the request to retrieve
///				invoice summary header information and line item information.  The processor will
///				package up the data and return a data object to the requesting module.
///				
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2005.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
///		2008-12-17  Richard Panachida    Updated for defect 865.
///		
///		2009-02-20  G. Kendall           WI#1494 - Updated field names for sorting problems (unknown fields) on Invoice
///		                                 Summary grid.
///		                                 
///		2009-02-24  A. Coker             Updated code so that individual transaction are not displayed multiple times
///		                                 when there are multiple line items or associated transactions.
///		                                 Updated code so that Payment and Recovery transactions created by parent site are not visible to 
///		                                 child sites.
///		                                 Updated code so that Account Code and Cost Centre Code filters are populated.
///
///		2009-03-11  Richard Panachida    Change Request 1903: Updated the List for the grid to handle the Rebate.
/// </summary>
#pragma warning restore 1587

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	public class InvoiceSummaryProcessorClass : IInvoiceSummaryProcessor
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		private InvoiceListSR invoiceListSR;
		private InvoiceListDO invoiceListDO;
		private AccountingSite accountingSite;
		private const string InvoiceSummaryProcessorAllValue = "ALL";
		private const string InvoiceSummaryProcessorAllText = "{All}";
		private enum CodeTypes { AccountCode, CostCenterCode };
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Invoice Summary Processor class.
		/// </summary>
		public InvoiceSummaryProcessorClass()
		{
			this.invoiceListDO = null;
			this.invoiceListSR = null;
			this.consolidatedDA = new ConsolidatedDAClass();
		}
        #endregion

        #region Methods
        /// <summary>
        /// This override method is the entry point for the invoice summary processor.
        /// </summary>
        /// <param name="inInvoiceListSR"></param>
        /// <returns></returns>
        public InvoiceListDO Process(InvoiceListSR inInvoiceListSR)
		{
			this.invoiceListSR = inInvoiceListSR;
			this.invoiceListDO = new InvoiceListDO();

			switch (this.invoiceListSR.SubRequest)
			{
				case InvoiceListSR.RequestTypes.GET_HEADER_DATA:
					this.GetHeaderData();
					break;

				case InvoiceListSR.RequestTypes.GET_DETAIL:
					this.accountingSite = (AccountingSite)this.invoiceListSR.AccountingSite;
					this.GetDetailData();
					break;
			}

			return this.invoiceListDO;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method orchestras the retrieval of invoice summary header data.
		/// </summary>
		private void GetHeaderData()
		{
			this.LoadProductData();
			this.LoadAccountCodes();
			this.LoadCostCenterCodes();
		}

		/// <summary>
		/// This method will get the product data and add it to the product list in
		/// the invoice list data object.
		/// </summary>
		private void LoadProductData()
		{
			ProductsClass products = new ProductsClass();
			ProductCollectionClass productCollection = products.EnumerateByFilterAndLocalize(this.invoiceListSR.Security, null, false);

			if ((productCollection != null) && (productCollection.Count > 0))
			{
			    // Add the "All" selection.
			    var valuePair = new DropdownValuePairDO
			                    {
			                        Text = InvoiceSummaryProcessorAllText,
			                        TextValue = InvoiceSummaryProcessorAllValue
			                    };
			    this.invoiceListDO.ProductList.Add(valuePair);

				foreach (ProductClass product in productCollection)
				{
				    valuePair = new DropdownValuePairDO { Text = product.ID, TextValue = product.IdentityGuid.ToString() };

				    this.invoiceListDO.ProductList.Add(valuePair);
				}
			}
		}

		/// <summary>
		/// This method will load the account codes into an array for the gui dropdown.
		/// </summary>
		private void LoadAccountCodes()
		{
		    // Add the "All" selection to the list.
		    var valuePair = new DropdownValuePairDO
		                    {
		                        Text = InvoiceSummaryProcessorAllText,
		                        TextValue = InvoiceSummaryProcessorAllValue
		                    };
		    this.invoiceListDO.AccountCodeList.Add(valuePair);

			ArrayList accountCodes = this.GetCodes(CodeTypes.AccountCode);

			foreach (string accountCode in accountCodes)
			{
			    valuePair = new DropdownValuePairDO { Text = accountCode, TextValue = accountCode };
			    this.invoiceListDO.AccountCodeList.Add(valuePair);
			}
		}

		/// <summary>
		/// This method will load the cost center codes into an array for the gui dropdown.
		/// </summary>
		private void LoadCostCenterCodes()
		{
		    // Add the "All" selection to the list.
		    var valuePair = new DropdownValuePairDO
		                    {
		                        Text = InvoiceSummaryProcessorAllText,
		                        TextValue = InvoiceSummaryProcessorAllValue
		                    };
		    this.invoiceListDO.CostCenterCodeList.Add(valuePair);

			ArrayList costCenterCodes = this.GetCodes(CodeTypes.CostCenterCode);

			foreach (string costCenterCode in costCenterCodes)
			{
			    valuePair = new DropdownValuePairDO { Text = costCenterCode, TextValue = costCenterCode };
			    this.invoiceListDO.CostCenterCodeList.Add(valuePair);
			}
		}

		/// <summary>
		/// This method will retrieve the invoice summary detail information and add the 
		/// data to the invoice data object.
		/// </summary>
		private void GetDetailData()
		{
		    // Determine the sites needed for filtering since Invoice Summary should show not only the
			// current site, but invoices from and groups this site is a member of.
			this.invoiceListSR.SiteList = this.DetermineSites();

			// Should we include deletes?
			//			this.orderListSR.Criteria.ShowDeleted = this.IncludeDeleted();

			// Read order list from the database
			DataSet dataset;
			using (SqlCommand cmd = new SqlCommand())
			{
				this.invoiceListDO.GetInvoiceSelectCommand(cmd, this.invoiceListSR);
				dataset = this.consolidatedDA.GetDataSet(cmd, this.invoiceListSR.Security);
			}

			// Load the data object line items
			foreach (DataRow row in dataset.Tables[0].Rows)
			{
				// Create a new line item object
				var lineItem = new InvoiceListLineItemDO();
				string siteID = row["SiteID"] == DBNull.Value ? "" : row["SiteID"].ToString();

				if ((this.invoiceListSR.SiteList.Contains(siteID) == false)
					&& (this.invoiceListSR.SiteList.Contains(siteID.ToLower()) == false)
					&& (this.invoiceListSR.SiteList.Contains(siteID.ToUpper()) == false))
				{
					continue;
				}

				lineItem.TransID = row["TransID"] == DBNull.Value ? "" : row["TransID"].ToString();
				lineItem.OrderNumber = row["OrderNumber"] == DBNull.Value ? "" : row["OrderNumber"].ToString();

				lineItem.Excise = this.ConvertDoubleToString(row, "Excise", SITE_VARIABLE_TYPE.DEFAULT);
				lineItem.GST = this.ConvertDoubleToString(row, "GST", SITE_VARIABLE_TYPE.DEFAULT);

				lineItem.PaymentNumber = row["PaymentNumber"] == DBNull.Value ? "" : row["PaymentNumber"].ToString();
				lineItem.TransactionAlias = row["AliasName"] == DBNull.Value ? "" : row["AliasName"].ToString();
				lineItem.Owner = row["OwnerID"] == DBNull.Value ? "" : row["OwnerID"].ToString();
				lineItem.Manager = row["ManagerID"] == DBNull.Value ? "" : row["ManagerID"].ToString();

				lineItem.GrossQuantity = this.ConvertDoubleToString(row, "GrossQuantity", SITE_VARIABLE_TYPE.VOLUME);
				lineItem.NetQuantity = this.ConvertDoubleToString(row, "NetQuantity", SITE_VARIABLE_TYPE.VOLUME);
				lineItem.ProductPrice = this.ConvertDoubleToString(row, "ProductPrice", SITE_VARIABLE_TYPE.DEFAULT);
				lineItem.TotalAmount = this.ComputeTotalAmount(row, "ProductPrice", "GrossQuantity", "Excise", "GST");

				lineItem.Product = row["Product"] == DBNull.Value ? "" : row["Product"].ToString();
				lineItem.AccountCode = row["AccountCode"] == DBNull.Value ? "" : row["AccountCode"].ToString();
				lineItem.CostCentreCode = row["CostCentreCode"] == DBNull.Value ? "" : row["CostCentreCode"].ToString();
				lineItem.InvoiceNumber = row["InvoiceNumber"] == DBNull.Value ? "" : row["InvoiceNumber"].ToString();
				lineItem.InvoiceLineNumber = row["InvoiceLineNumber"] == DBNull.Value ? "" : row["InvoiceLineNumber"].ToString();
				lineItem.ShipToID = row["ShipToID"] == DBNull.Value ? "" : row["ShipToID"].ToString();
				lineItem.SupplierID = row["SupplierID"] == DBNull.Value ? "" : row["SupplierID"].ToString();
				lineItem.BatchNumber = row["BatchNumber"] == DBNull.Value ? "" : row["BatchNumber"].ToString();
				lineItem.ShipmentNumber = row["ShipmentNumber"] == DBNull.Value ? "" : row["ShipmentNumber"].ToString(); // Receipt Number
				lineItem.DocumentNumber = row["DocumentNumber"] == DBNull.Value ? "" : row["DocumentNumber"].ToString(); // JS20100729 WI-16279

				if (row["Flag02"] == DBNull.Value)
				{
					lineItem.Rebate = "";
				}
				else if (((bool)row["Flag02"]))
				{
					lineItem.Rebate = "Yes";
				}
				else
				{
					lineItem.Rebate = "";
				}

				if (row["TransDateTime"] is DateTimeOffset)
				{
					lineItem.TransactionDateTime = DataObject.getValue<DateTimeOffset>(row["TransDateTime"], TimeConverter.Today());
				}
				else
				{
					lineItem.TransactionDateTime = new DateTimeOffset();
				}

				if (row["InventoryDate"] is DateTime)
				{
					lineItem.InventoryDate = DataObject.getValue<DateTime>(row["InventoryDate"], DateTime.Today);
				}
				else
				{
					lineItem.InventoryDate = new DateTime();
				}

				lineItem.TransactionStatus = row["LookupTransactionStatusIndex"] == DBNull.Value ? TransactionStatus.Completed : (TransactionStatus)row["LookupTransactionStatusIndex"];
				/*	lineItem.ShipToName = row["ShipToName"] == System.DBNull.Value ? "" : row["ShipToName"].ToString();
					lineItem.ShipToAddress = row["ShipToAddress"] == System.DBNull.Value ? "" : row["ShipToAddress"].ToString();
					lineItem.ShipToCity = row["ShipToCity"] == System.DBNull.Value ? "" : row["ShipToCity"].ToString();
					lineItem.ShipToState = row["ShipToState"] == System.DBNull.Value ? "" : row["ShipToState"].ToString();
					lineItem.SupplierName = row["SupplierName"] == System.DBNull.Value ? "" : row["SupplierName"].ToString();
					lineItem.SupplierAddress = row["SupplierAddress"] == System.DBNull.Value ? "" : row["SupplierAddress"].ToString();
					lineItem.SupplierCity = row["SupplierCity"] == System.DBNull.Value ? "" : row["SupplierCity"].ToString();
					lineItem.SupplierState = row["SupplierState"] == System.DBNull.Value ? "" : row["SupplierState"].ToString();
				*/

				// Add it to the order list data object
				this.invoiceListDO.LineItems.Add(lineItem);
			}
		}

	    /// <summary>
	    /// This method will compute the total amount using the following formula:
	    /// total amount = (price * quanity) + excise + gst.
	    /// </summary>
	    /// <param name="row"></param>
	    /// <param name="rowNamePrice"></param>
	    /// <param name="rowNameQuantity"></param>
	    /// <param name="rowNameExcise"></param>
	    /// <param name="rowNameGst"></param>
	    /// <returns></returns>
	    private string ComputeTotalAmount(DataRow row, string rowNamePrice, string rowNameQuantity, string rowNameExcise, string rowNameGst)
		{
			if ((row[rowNamePrice] == DBNull.Value) || (row[rowNameQuantity] == DBNull.Value))
			{
				return "";
			}
			else
			{
				double excise = 0.0;
				double gst = 0.0;

				if (row[rowNameExcise] != DBNull.Value)
				{
					excise = (double)row[rowNameExcise];
				}

				if (row[rowNameGst] != DBNull.Value)
				{
					gst = (double)row[rowNameGst];
				}

				double price = (double)row[rowNamePrice];
				double quantity = (double)row[rowNameQuantity];

				quantity = this.accountingSite.ConvertFromSi(quantity, AccountingSite.ConversionUnits.VOLUME);
				double totalAmount = (price * quantity) + excise + gst;

				return this.accountingSite.GetFormattedValue(totalAmount, SITE_VARIABLE_TYPE.DEFAULT);
			}
		}

		/// <summary>
		/// This method will convert a double to a string value and if the
		/// database value is null, an empty string is returned.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="rowName"></param>
		/// <param name="formatType"></param>
		/// <returns></returns>
		private string ConvertDoubleToString(DataRow row, string rowName, SITE_VARIABLE_TYPE formatType)
		{
			if (row[rowName] == DBNull.Value)
			{
				return "";
			}
			else
			{
				double rowValue = (double)row[rowName];

				if (formatType == SITE_VARIABLE_TYPE.DEFAULT)
				{
					return this.accountingSite.GetFormattedValue(rowValue, formatType);
				}
				else
				{
					return this.accountingSite.ConvertFromSiFormatted(rowValue, AccountingSite.ConversionUnits.VOLUME);
				}
			}
		}

		/// <summary>
		/// This method returns an array of sites to be used to find
		/// invoices.
		/// </summary>
		/// <returns></returns>
		private ArrayList DetermineSites()
		{

			// Get the sites info list
			SitesInfoClass sitesInfo = new SitesInfoClass();
			SiteInfoDO siteInfoDO = sitesInfo.RefreshSiteInfo(this.invoiceListSR.Security);

		    ArrayList siteArray = new ArrayList { this.invoiceListSR.Security.SiteID };

		    // Add the current site

		    // If the base Site is a group, enumerate all the member sites
			SiteCollectionClass siteCollection = siteInfoDO.EnumerateByParentSite(this.invoiceListSR.Security.SiteGuid);

			foreach (SiteClass site in siteCollection)
			{
				siteArray.Add(site.ID);
			}

			return siteArray;
		}


/*
        /// <summary>
        /// This method enumerates the parent sites if there are any for a given site.  It builds
        /// a final site list that is used to determine which sites to search for invoice transactions.
        /// </summary>
        /// <param name="siteInfoDO"></param>
        /// <param name="site"></param>
        /// <param name="siteGroupsEvaluated"></param>
        /// <param name="finalSiteList"></param>
        private void EnumerateParentSites(SiteInfoDO siteInfoDO, SiteClass site, ArrayList siteGroupsEvaluated, ArrayList finalSiteList)
		{
			// Is the group we are looking at a Site Group?
			if (site.SiteGroup == true)
			{
				// Do we need to evaluate this group?
				foreach (Guid siteID in siteGroupsEvaluated)
				{
					if (siteID == site.SiteGuid)
					{
						return;
					}
				}

				// Mark this one as having been evaluated
				siteGroupsEvaluated.Add(site.SiteGuid);

				// Get the member sites of this group
				SiteCollectionClass children = siteInfoDO.EnumerateByParentSite(site.SiteGuid);

				// Is our site a member of this group?
				foreach (SiteClass child in children)
				{
					// Is this a group?
					if (child.SiteGroup == true)
					{
						this.EnumerateParentSites(siteInfoDO, child, siteGroupsEvaluated, finalSiteList);
					}

					if (child.SiteGuid == this.invoiceListSR.Security.SiteGuid)
					{
						finalSiteList.Add(site.ID);
					}
				}
			}
		}
*/

		/// <summary>
		/// This method will return the either a list of account codes or
		/// cost center codes. These values are retrieved from the Site user
		/// data fields 1 and 2.
		/// </summary>
		/// <param name="codeType"></param>
		/// <returns></returns>
		private ArrayList GetCodes(CodeTypes codeType)
		{

			TransactionAliasesClass aliases = new TransactionAliasesClass();
			TransactionAliasCollectionClass txAliasCollection;
			ArrayList codeList = new ArrayList();

			if (InvoiceListSR.INVOICE_PAYABLE == this.invoiceListSR.InvoiceType)
			{
				txAliasCollection = aliases.EnumerateByTransTypeID(this.invoiceListSR.Security, TransactionTypes.T21_AccountPayableInvoice);
			}
			else if (InvoiceListSR.INVOICE_RECEIVABLE == this.invoiceListSR.InvoiceType)
			{
				txAliasCollection = aliases.EnumerateByTransTypeID(this.invoiceListSR.Security, TransactionTypes.T22_AccountReceivableInvoice);
			}
			else
				return codeList;

			if (txAliasCollection.Count == 0)
			{
				return codeList;
			}

			TransactionAliasClass alias = txAliasCollection[0];
			Guid aliasGuid = alias.IdentityGuid;

			// Define the user data variables.
		    UserDataFieldsClass userDataFields = new UserDataFieldsClass();

			// Retrieve a collection of user data fields for the current site.
			var userDataFieldList = userDataFields.EnumerateByEntityType(this.invoiceListSR.Security, ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM, aliasGuid, false, false);

			if ((userDataFieldList != null) && (userDataFieldList.Count >= 2))
			{
				// Get the value list collection from the user data object.
				foreach (var fieldClass in userDataFieldList)
				{
				    var userDataField = (UserDataFieldClass)fieldClass;
				    if ((codeType == CodeTypes.AccountCode &&
						 userDataField.DisplayName == ListViewFieldClass.StandardFieldTypeID(STANDARD_FIELD_TYPE.ACCOUNT_CODE, true) ||
						 (codeType == CodeTypes.CostCenterCode &&
						 userDataField.DisplayName == ListViewFieldClass.StandardFieldTypeID(STANDARD_FIELD_TYPE.COST_CENTRE_CODE, true))))
					{
						var userDataListValueList = userDataField.UserDataListValueCollection;

						if ((userDataListValueList != null) && (userDataListValueList.Count > 0))
						{
							// Loop through the user data value list and build an array of values.
							foreach (UserDataListValueClass userDataListValue in userDataListValueList)
							{
								codeList.Add(userDataListValue.ID);
							}
						}
						break;
					}
				}
			}

			return codeList;
		}
		#endregion
	}
}