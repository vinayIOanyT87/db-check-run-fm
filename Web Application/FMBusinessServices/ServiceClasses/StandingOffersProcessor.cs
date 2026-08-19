 #pragma warning disable 1587
/// <summary>
///   File name:	FinanceProcessor.cs
///   Purpose:	   To decipher the request to update Sale and Issue Transaction fuel prices.
///				   data object.
///				   
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				   
///	Author(s):	A. Coker
///	Version:	1.0.0  Current version
///	
///	Modification History:
///   Date:			By:						Reason:
///   ----------  -------------------- ----------------------------------------------------------
///   2009-04-20  A. Coker             Defect 3252 - Update fuel price and tax amounts of line items
///                                    of Sale and Issue type transactions 
///                                    when standing offer price is added/modified. Transactions have to  have
///                                    inventory date that is within current month and within the standing offer's
///                                    effective date range.
/// </summary>
#pragma warning restore 1587
namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessServices.InternalClasses;

    using IsolationLevel = System.Transactions.IsolationLevel;

    [ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	class StandingOffersProcessorClass : IStandingOffersProcessor
	{
		#region Private data members
		private AccountingSite accountingSite;
		private StandingOffersSR sr;
		private TransactionHierarchyUtil hierarchyUtil;
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public StandingOffersProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
			this.accountingSite = new AccountingSite();
			this.hierarchyUtil = null;
		}
		#endregion

		#region Public methods
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Process(StandingOffersSR inSR)
		{
			this.sr = inSR;

			//Validate that the user has permission to retrieve the transaction.
			if (this.sr.Security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false)
			{
				string msg = "User " + this.sr.Security.UserID + " does not have permission to modify transactions.";
				throw new AccountingServicesException(msg);
			}

			if (this.sr.StandingOfferGuid != Guid.Empty)
			{
				StandingOffersClass standingOffers = new StandingOffersClass();
				StandingOfferClass standingOffer = standingOffers.Get(this.sr.Security, this.sr.StandingOfferGuid);
				this.UpdateAssociatedTransactions(standingOffer);
			}
		}
		#endregion

		/// <summary>
		/// This method will update the transaction product price that matches the supplier, product,
		/// effective and expiration dates. All the transactions that meet the criterion will be updated
		/// with the new price list (aka standing offer) price.
		/// </summary>
		/// <param name="standingOffer"></param>
		private void UpdateAssociatedTransactions(StandingOfferClass standingOffer)
		{
			//There has to be a product ID. Return immediately if not set.
			if (string.IsNullOrEmpty(standingOffer.ProductID))
			{
				return;
			}

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					//Retrieve line items of Sale and Issue type transactions with inventory date falling within the current month and
					//effective price list (aka standing offer) dates
					cmd.CommandText =
						"SELECT t.LookupTransTypeIndex, t.AliasName, t.TransVersion, t.SupplierCompanyGuid, t.SupplierID, t.InventoryDate, " +
									"t.ShipToCompanyGuid, t.ShipToID, l.* " +
						"FROM tblTransactionLineItems l INNER JOIN tblTransactions t ON t.TransactionGuid = l.TransactionGuid " +
						"WHERE LookupTransTypeIndex = 5 AND t.DeleteFlag = 0 AND l.DeleteFlag = 0 AND " +
							"InventoryDate > (SELECT ISNULL(MAX(CloseoutDate),'1/1/1900') FROM tblCloseoutInventory  " +
														"WHERE SiteGuid=t.SiteGuid AND ProductGuid=l.ProductGuid) AND " +
							"(SupplierID IS NULL OR SupplierID = '' OR SupplierID = @SupplierID) AND  " +
							"Product = @Product AND  " +
							"InventoryDate BETWEEN @BeginDate AND  @EndDate  AND " +
							"MONTH(InventoryDate) = MONTH(SYSDATETIMEOFFSET()) AND YEAR(InventoryDate) = YEAR(SYSDATETIMEOFFSET()) AND " +
								"(SiteGuid = @SiteGuid OR SiteGuid IN (SELECT SiteGuid FROM map.tblEntityStandingOfferToSite " +
																					"WHERE StandingOfferGuid = @StandingOfferGuid)) " +
					 "ORDER BY t.transID";

					cmd.Parameters.AddWithValue("@SupplierID", standingOffer.SupplierID);
					cmd.Parameters.AddWithValue("@Product", standingOffer.ProductID);
					// Truncate to only the date part for the date fields
					cmd.Parameters.AddWithValue("@BeginDate", TimeConverter.ToDate(standingOffer.EffectiveDate).Date);
					cmd.Parameters.AddWithValue("@EndDate", TimeConverter.ToDate(standingOffer.ExpirationDate).Date);
					cmd.Parameters.AddWithValue("@SiteGuid", this.sr.Security.SiteGuid);
					cmd.Parameters.AddWithValue("@StandingOfferGuid", this.sr.StandingOfferGuid);


					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, this.sr.Security);

					if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
					{
						string transID = null;
						TransactionDO trans = new TransactionDO();

						AccountingSites accountingSites = new AccountingSites();
						this.accountingSite = accountingSites.LoadSiteInfo(this.sr.Security, this.sr.Security.SiteGuid);

					    this.hierarchyUtil = new TransactionHierarchyUtil(this.sr.Security);

						PriceCalculatorInvokerClass priceCalculator = new PriceCalculatorInvokerClass();

						foreach (DataRow row in dataSet.Tables[0].Rows)
						{
							transID = row["TransID"] as string;

							if (trans.TransID != transID)
							{
								if (trans.TransID != null)
								{
									priceCalculator.Calculate(this.sr.Security, trans);
									this.SaveLineItems(this.sr.Security, trans);
								}

								//Setup minimum transaction info to handle line item update.
								trans = this.CreateTransaction(row);
							}

							LineItemDO lineItem = new LineItemDO();
							lineItem.Load(row, trans.TransTypeID);

							//Set fuel price and tax amounts to null so that PriceCalculator is forced calculate them.
							lineItem.ProductPrice = null;
							lineItem.Tax1 = null;
							lineItem.Tax2 = null;
							lineItem.Tax3 = null;
							lineItem.Quantity.GrossInventoryChange = this.accountingSite.ConvertFromSi(lineItem.Quantity.GrossInventoryChange, AccountingSite.ConversionUnits.VOLUME);
							trans.LineItems.Add(lineItem);
						}

						if (transID != null)
						{
							priceCalculator.Calculate(this.sr.Security, trans);
							this.SaveLineItems(this.sr.Security, trans);
						}

					}
				}
			}
			    // ReSharper disable once EmptyGeneralCatchClause
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Creates a transaction with minimal information in order to run the PriceCalculator on the line items.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		private TransactionDO CreateTransaction(DataRow row)
		{
		    TransactionDO trans = new TransactionDO
		                          {
		                              TransID = row["TransID"] as string,
		                              InventoryDate = DataObject.getValue(row["InventoryDate"], DateTimeOffset.MinValue.Date),
		                              Alias = row["AliasName"] as string,
		                              TransVersion = DataObject.getValue<long>(row["TransVersion"], 0),
		                              SupplierID = row["SupplierID"] as string,
		                              SupplierCompanyGuid = DataObject.getGuid(row["SupplierCompanyGuid"]),
		                              TransTypeID = DataObject.getValue(row["LookupTransTypeIndex"], TransactionTypes.T_Maximum),
		                              ShipToID = row["ShipToID"] as string,
		                              ShipToCompanyGuid = DataObject.getGuid(row["ShipToCompanyGuid"])
		                          };
		    return trans;
		}

		/// <summary>
		/// Saves line items owned by the TransactionDO object. Also, update aggregate values of parent
		/// transactions.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="trans">
		/// The trans.
		/// </param>
		private void SaveLineItems(SecurityClass security, TransactionDO trans)
		{
            List<LineItemWithTransactionInformation> lineItemsWithTransactionInformation = new List<LineItemWithTransactionInformation>();

            foreach (LineItemDO lineItem in trans.LineItems)
            {
                lineItem.Quantity.GrossInventoryChange = this.accountingSite.ConvertToSi(lineItem.Quantity.GrossInventoryChange, AccountingSite.ConversionUnits.VOLUME);

                LineItemWithTransactionInformation withTransactionInformation = new LineItemWithTransactionInformation
                {
                    LineItem = lineItem,
                    TransactionGuid = trans.TransactionGuid,
                    InventoryDate = trans.InventoryDate,
                    TransVersion = trans.TransVersion
                };

                lineItemsWithTransactionInformation.Add(withTransactionInformation);
            }

            var lineItemDbi = new TransactionLineItemDBI(this.sr.Security.UserID);
            lineItemDbi.Save(security, lineItemsWithTransactionInformation);

            foreach (LineItemDO lineItem in trans.LineItems)
            {
                this.hierarchyUtil.UpdateAggregatedParents(lineItem.TransactionLineItemGuid, false);
            }
		}
	}
}
