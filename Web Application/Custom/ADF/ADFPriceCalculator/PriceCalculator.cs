/// <summary>
///   FILE NAME:  PriceCalculator.cs
///   PURPOSE:		PriceCalculator Class
///   	         This class is a custom class that is load and invoked via late binding.  The logic
///   	         in this class is confined to the calculation specifications laid out in the JFEM
///   	         FPS.
///   COMMENTS:   Copyright (C) Varec, Inc. Norcross, GA, USA, 2008
///               This file shall not be copied or reproduced in any form without
///               the express written consent of Varec, Inc.
///               
///   AUTHOR(S):	Eric Simmons
///   VERSION:		1.0.0  Current version
///   
///   MODIFICATION HISTORY:
///   Date:			By:			      Reason:
///   ----------	----------------- -------------------------------------------
///   09-17-2008	E. Simmons	      Initial Revison to support CSI #6153
///   
///   12-16-2008	V. Thompson	      Updated calls used to retrieve excise rates to use
///                                 the product index instead of the product id
///                           
///   12-18-2008  A. Coker          Updated price and cost calculations.
///   2009-01-09  Richard Panachida Updated the price and cost calculation due to an error
///                                 with the recalculation amounts when GST, Excise, and
///                                 mark-up have values. Related to defect 918 & 452.
///                                 
///   01-27-2009  A. Coker          Fixed defect 1161.
///   
///   2009-02-17  Richard Panachida Fixed defect 1477.
///   2009-02-20  Richard Panachida Fixed to handle a check to see if the price and quantity fields were
///                                 updated. If so, then recalculate.
///   2009-02-23  Richard Panachida Fixed for defect 1691. It was a divide by zero issue that does not throw an exception.
///                                 Instead the field is populated with NaN.
///   2009-02-26  Richard Panachida Fixed for defect 1448. The standing offer was not taking delivery location into account.
///   2009-03-03  Richard Panachida Removed the calculation for get standing offer and AUP because it should have been in the
///                                 Finance Object. Now the code calls the finance object. Defect 1696.
///                                 
///   2009-03-26  Richard Panachida Request from Amanda that the Markup data retrieval will not look at the service the 
///                                 company is assoicated to. In addition, markup will only look at the markup aviation
///                                 type. All the code is commented out until we find out from JFLA how they want it to
///                                 work.
///                                 
///   2009-03-27  Richard Panachida Defect 2482: Due to a requirement change that excise may not be configured, I updated 
///                                 the code not to recalculate the price if the excise rate is zero.
///                                 
///   2009-03-30  Richard Panachida Defect 2486: Added code to ensure that type 14 (physical inventory) transactions use
///                                 average unit price first.
///                                 
///   2009-04-03  Richard Panachida Defect 2802. Added a check in the get markup method to check to see if the ship-to
///                                 company index is null.
///                                 
///   2009-04-06  A. Coker          Defect 2856. After setting price, continue with next line item.
///   
///   2009-05-11  Richard Panachida Defect 3629: The Cost Inclusive value changed.  The problem was due to rounding and
///                                 in the recalculation.  I added a check not to recalculation if the recalculation flag
///                                 is to false.
///                                 
///   2009-06-19  A. Coker          Defect 4072 and 4136. Fixed difference between expected and calculated values due to rounding.
///   
///   2009-06-29  Jack Shen         Defect 4455. Now transactions update properly when "Asset Leaving Australia"
///                                 checkbox state is changed.
///                                 
///   2009 07 20  Jack Shen         Defect 4606, FAT-674. Now correctly calculates GST, Excise and On-Cost for physical inventory.
///   
/// </summary>
using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using FM7Accounting;
using ConsolidatedDataObjects;
using ConsolidatedBLL;
using FMCommon;
using FinanceBLL;
using FinanceDataObjects;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace FuelsManager.Custom.ADF
{
    public class ADFPriceCalculatorClass : IPriceCalculator
    {
        #region Private data members
        private string standingOfferID = null;
        private enum LineItemTypes { EXCISE, GST, MARKUP, PRODUCT_PRICE, WAC, ONCOST, QUANTITY, NONE };
        private bool transIsSale;
        private bool transIsIssue;
        private bool leavingAustralia;
        private bool forceSaleRecalculation;
        private TransactionDO trans;
        private SecurityClass security;
        private ArrayList origLineItems;
        private const int CurrencyPercision = 7;
        private bool recalculateTaxes;
        private bool wacCalculatedThisRun;
        private TransactionDO origTrans = null;
        private FinanceDO financeDO = null;

        protected static string NOT_CONFIGURED = "is not configured";
        protected Hashtable m_serviceTable = null;
        protected AccountingService m_accountingService = new AccountingClient().connect();
        #endregion

        bool IPriceCalculator.Calculate(SecurityClass security, TransactionDO trans, ArrayList origLineItems, bool bForceRecalculation)
        {
            FMStandingOfferException standingOfferException = null;

            // need a try for the whole price calculator because exceptions are being shown on the interface
            try
            {
#if DEBUG
               Stopwatch watch = new Stopwatch();
               watch.Start();
#endif // DEBUG

               this.security = security;
               this.trans = trans;
               this.origLineItems = origLineItems;
               this.forceSaleRecalculation = false;

               if ((this.security == null) || (this.trans == null) || (this.trans.LineItems == null) || (this.trans.LineItems.Count == 0) ||
                  // JS20100716 performance, ignore first ADF Price Calculator call when saving invoice or recoveries
                  (this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice && origLineItems == null) ||
                  (this.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice && origLineItems == null))
               {
                  return false;
               }

               // Direct Fuel Purchase and Commercial Purchase both are immune to price calculations
               if ((this.trans.TransTypeID == TransactionTypes.T12_Type12)
                  && ((this.trans.Alias.ToUpper().Contains("DIRECT FUEL PURCHASE") == true)
                  || (this.trans.Alias.ToUpper().Contains("COMMERCIAL") == true)))
               {
                  return true;
               }

               ExciseTaxBL exciseTaxBL = new ExciseTaxBL();
               this.transIsSale = trans.Alias.ToUpper().Contains("SALE");
               this.transIsIssue = trans.Alias.ToUpper().Contains("ISSUE");
               int lineItemIndex = -1;

               if (null == m_serviceTable)
               {
                  this.InitServiceTable(ref m_serviceTable);
               }

               foreach (LineItemDO lineItem in this.trans.LineItems)
               {
                  bool lineItemHadNoWac = false;
                  wacCalculatedThisRun = false;

                  this.leavingAustralia = lineItem.Flag04;

                  // Used for finding the original line item values. JS20100621 changed to ++lineItemIndex for performance gain
                  ++lineItemIndex;

                  // Cannot not calculate pricing if the product does not exist.
                  // Go to the next line item.
                  if (lineItem.ProductIndex == null)
                  {
                     continue;
                  }

                  this.standingOfferID = null;

                  double revenue = 0.0; //baseCost + markUpAmount
                  double totalCost = 0.0; //gstAmount + exciseAmount + revenue
                  double cost = 0.0; //This will be either (so * q) or (unit price * q)
                  int supplierIndex = 0;

                  if (this.trans.SupplierIndex != null)
                  {
                     supplierIndex = (int)this.trans.SupplierIndex.Value;
                  }

                  // The the amounts from the lineItem.
                  double unitPrice = this.SetLineItemValues(lineItem, ADFPriceCalculatorClass.LineItemTypes.PRODUCT_PRICE);
                  double markupAmount = this.SetLineItemValues(lineItem, ADFPriceCalculatorClass.LineItemTypes.MARKUP);
                  double gstAmount = this.SetLineItemValues(lineItem, ADFPriceCalculatorClass.LineItemTypes.GST);
                  double exciseAmount = this.SetLineItemValues(lineItem, ADFPriceCalculatorClass.LineItemTypes.EXCISE);
                  double wacPrice = this.SetLineItemValues(lineItem, ADFPriceCalculatorClass.LineItemTypes.WAC);
                  double oncostAmount = this.SetLineItemValues(lineItem, ADFPriceCalculatorClass.LineItemTypes.ONCOST);
                  double quantity = this.SetLineItemValues(lineItem, ADFPriceCalculatorClass.LineItemTypes.QUANTITY);

                  //Set Quantity
                  double so = -1.0;

                  double quantityChange = 0.0;

                  // JS20100716 Performance, transactions which do not impact inventory do not care about the inventory change
                  if (trans.TransTypeID != TransactionTypes.T21_AccountPayableInvoice &&
                     trans.TransTypeID != TransactionTypes.T22_AccountReceivableInvoice &&
                     trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
                  {
                     quantityChange = SaveWeightedAverageCostsProcessor.QuantityChangedSinceLastSave(this.trans, lineItem);

                     // if WAC is not set then retrieve the latest for the new line item
                     if ((wacPrice <= 0.0 || ShouldUseLatestWac(quantityChange, lineItem)) && origLineItems != null)
                     {
                        wacPrice = this.RetrieveLatestWAC(security, (int)trans.SiteIndex.Value, (int)lineItem.ProductIndex.Value);
                        lineItemHadNoWac = true;
                     }
                  }

                  // Always look for the average unit price of physical inventories.
                  if (this.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory)
                  {
                     so = this.GetAUPPrice(lineItem, supplierIndex);
                  }
                  else
                  {
                     so = this.GetStandingOfferPrice(lineItem, supplierIndex);
                  }

                  // Cannot calculate pricing if the standing offer is less than zero
                  // and the user did not enter in a unit price. Or that the WAC doesn't exist.
                  if ((so <= 0.0) && (unitPrice <= 0.0) && (wacPrice <= 0.0))
                  {
                     if (trans.TransTypeID == TransactionTypes.T14_PhysicalInventory)
                     {
                        lineItem.ProductPrice = new VDouble(0.0);
                     }
                     else
                     {
                        // This sets the product price to null.
                        lineItem.ProductPrice = new VDouble();
                     }

                     if ((financeDO != null) &&
                        (financeDO.HasMessage == true) &&
                        (trans.TransTypeID == TransactionTypes.T18_SupplyOrder) &&
                        (trans.Alias.ToUpper().Contains("FUEL ORDER") == true))
                     {
                        standingOfferException = new FMStandingOfferException(financeDO.InfoMessage);
                        standingOfferException.ContinueOn = false;
                     }

                     continue;
                  }
                  else if ((so > 0.0) && 
                           (financeDO != null) && 
                           (financeDO.HasMessage == true) && 
                           (trans.TransTypeID == TransactionTypes.T18_SupplyOrder) &&
                           (trans.Alias.ToUpper().Contains("FUEL ORDER") == true))
                  {
                     standingOfferException = new FMStandingOfferException(financeDO.InfoMessage);
                     standingOfferException.ContinueOn = true;
                  }

                  // JS20100621 CCP if on-cost or asset leaving australia has changed then force sales recalculation
                  if (transIsSale && origLineItems != null)
                  {
                     if (quantityChange != 0.0)
                     {
                        forceSaleRecalculation = true;

                        // sale updates where the quantity has changed is always a new update, so allow overwriting the actuals
                        if (trans.ReversalType == TransactionDO.Update)
                        {
                           lineItem.Number01 = null;
                           lineItem.Number02 = null;
                           lineItem.Number03 = null;
                           lineItem.Number04 = null;
                           lineItem.Number05 = null;
                           lineItem.Number06 = null;
                        }
                     }

                     if (origLineItems.Count > lineItemIndex)
                     {
                        double origOncost = 0.0;
                        bool oncostParseFailure = false;
                        try
                        {
                           if ((origLineItems[lineItemIndex] as LineItemDO).UserData["Transaction Alias Line Item User Data 14"] != null &&
                              (origLineItems[lineItemIndex] as LineItemDO).UserData["Transaction Alias Line Item User Data 14"].ToString().Length > 0)
                           {
                              origOncost = double.Parse((origLineItems[lineItemIndex] as LineItemDO).UserData["Transaction Alias Line Item User Data 14"].ToString());
                           }
                        }
                        catch (Exception)
                        {
                           oncostParseFailure = true;
                        }
                        if (oncostAmount != origOncost ||
                           lineItem.Flag04 != (origLineItems[lineItemIndex] as LineItemDO).Flag04 ||
                           oncostParseFailure)
                        {
                           forceSaleRecalculation = true;
                        }
                     }
                     else
                     {
                        // doesn't have original line item
                        forceSaleRecalculation = true;
                     }
                  }

                  //Get rates based on the configuration or based on amounts. 
                  double gstRate = this.GetGSTRate(
                          trans.TransactionDateTime == null ? DateTime.UtcNow : trans.TransactionDateTime.Value);
                  double exciseRate = this.GetExciseRate((int)lineItem.ProductIndex.Value,
                          trans.TransactionDateTime == null ? DateTime.UtcNow : trans.TransactionDateTime.Value);
                  double markUpRate = this.GetMarkupRate();

                  if (!lineItem.WacCalculated &&
                      trans.TransTypeID != TransactionTypes.T18_SupplyOrder &&
                     // something changed with transaction detail works, gross is sometimes 0 on postback
                      lineItem.Quantity.Net != 0.0
                     )
                  {
							double sign = lineItem.Quantity.NetInventoryChange > 0 ? 1.0 : -1.0;

                     // WAC will be calculated sequentially as lineitems are ordered
                     double price = 0.0;
                     double latestWac = 0.0;

                     if (this.trans.TransTypeID == TransactionTypes.T8_Receipt ||
                         this.trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade ||
                         this.trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
                     {
                        switch (this.trans.TransTypeID)
                        {
                           // CCP-043 receipts now strips certain taxes depending on the service of the fuel;
                           case TransactionTypes.T8_Receipt: // TBC retrieve original transaction and see if old status was quaranteened
                              // This assumes once receipt line items are changed to usable, they CANNOT be changed again.
                              if (((lineItem.Quality == TransactionQuality.Usable && (quantityChange != 0.0 || trans.TransVersion == 0)) ||
                                  (lineItem.Quality == TransactionQuality.Usable && trans.TransVersion != 0 && this.QualityWasNotUsable(security, trans, lineItem))))
                              {
                                 latestWac = this.RetrieveLatestWAC(security, (int)trans.SiteIndex.Value, (int)lineItem.ProductIndex.Value);

                                 // check reverse here (common method)
                                 if (this.isReverse(this.trans) && latestWac != lineItem.Tax4.Value)
                                 {
                                    sign = -1.0;

                                    price = lineItem.Number06.Value;
                                 }
                                 else
                                 {
                                    price = unitPrice <= 0.0 ? so : unitPrice;

                                    // CCP-043 strip taxes depending on the type of fuel
                                    price = this.StripTaxes(lineItem, price, gstRate, exciseRate);

                                    // store price into tax5 because it cannot be recalculated
                                    lineItem.Number06 = new VDouble(price);
                                 }

                                 if (quantityChange == 0)
                                 {
                                    // quantity change could be 0 when changed to usable only after a transaction has been saved
                                    // since it can only be changed to usable once, we use the gross as quantity change
												quantityChange = lineItem.Quantity.Net;
                                 }

                                 //wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange * sign, price, latestWac);
                                 wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange, price, latestWac);
                                 wacCalculatedThisRun = lineItem.WacCalculated = true;

                                 // when wac is calculated from a receipt / changed to complete/usable, then we should update transaction date time
                                 // TransactionDO dates are stored in display format
                                 SiteTimeConverter converter = new SiteTimeConverter(new SitesClass().GetByID(security, security.SiteID));
                                 this.trans.TransactionDateTime.Value = this.trans.InventoryDate = converter.ConvertToSiteTime(DateTime.UtcNow);
                              }

                              if (lineItem.Quality != TransactionQuality.Usable && !wacCalculatedThisRun && (origLineItems != null || trans.TransVersion == 0))
                              {
                                 WeightedAverageCostsClass wacs = new WeightedAverageCostsClass();
                                 WeightedAverageCostClass wac = wacs.GetLatest(security, (int) trans.SiteIndex.Value, (int) lineItem.ProductIndex.Value);

                                 lineItem.Tax4 = new VDouble(wac.WacValue);
                              }

                              break;
                           case TransactionTypes.T15_PrimaryRegrade:
                           case TransactionTypes.T16_SecondaryRegrade:

                              if ((quantityChange != 0.0 || trans.TransVersion == 0))
                              {
                                 RegradeLineItemDO li = lineItem as RegradeLineItemDO;

                                 if (null == li)
                                 {
                                    throw new Exception("ADF Price Calculator could not process regrade for WAC calculation");
                                 }

                                 double toLatestWac = this.RetrieveLatestWAC(security, (int)trans.SiteIndex.Value, (int)li.ToProductIndex.Value);
                                 double fromLatestWac = this.RetrieveLatestWAC(security, (int)trans.SiteIndex.Value, (int)li.ProductIndex.Value);

                                 if (this.isReverse(this.trans))
                                 {
                                    li.Tax5 = new VDouble(this.CalculateWAC(security, trans, li.ToProduct, -quantityChange, lineItem.Tax4.Value, toLatestWac)); // original destination
                                    wacPrice = this.CalculateWAC(security, trans, li.Product, quantityChange, lineItem.Tax4.Value, fromLatestWac); // original source
                                 }
                                 else
                                 {
                                    li.Tax5 = new VDouble(this.CalculateWAC(security, trans, li.ToProduct, quantityChange, wacPrice, toLatestWac)); // destination
                                    wacPrice = this.CalculateWAC(security, trans, li.Product, -quantityChange, wacPrice, fromLatestWac); // source
                                 }
                                 wacCalculatedThisRun = lineItem.WacCalculated = true;
                              }

                              break;  // rule 2, regrades
                         }
                     }
                     // for all other transactions...
                     else
                     {
                        // means a change to existing inventory, need to check if latest WAC is different to current for inventory
                        // transactions
                        switch (this.trans.TransTypeID)
                        {
                           case TransactionTypes.T5_PrimaryDisbursement:
                           case TransactionTypes.T6_SecondaryDisbursement:
                           case TransactionTypes.T3_PrimaryDefuel:
                           case TransactionTypes.T4_SecondaryDefuel:
                           case TransactionTypes.T1_PrimaryAdjustment:
                           case TransactionTypes.T2_SecondaryAdjustment:
									case TransactionTypes.T25_Shipment:
                              //case TransactionTypes.T14_PhysicalInventory:

                              if ((trans.TransTypeID != TransactionTypes.T3_PrimaryDefuel &&
                                   trans.TransTypeID != TransactionTypes.T4_SecondaryDefuel) ||
                                   lineItem.Quality == TransactionQuality.Usable)
                              {
                                 double currentWac = this.RetrieveLatestWAC(security, (int)trans.SiteIndex.Value, (int)lineItem.ProductIndex.Value);

                                 // check reverse here (common method)
                                 if (this.isReverse(this.trans) && lineItem.Tax4.Value != currentWac && (quantityChange != 0.0 || trans.TransVersion == 0))
                                 {
                                    //wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange * modifier * sign, lineItem.Tax4.Value, currentWac);
                                    wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange, lineItem.Tax4.Value, currentWac);
                                    wacCalculatedThisRun = lineItem.WacCalculated = true;
                                 }
                                 // old, no longer relevant because a transaction can be applied multiple times, but will leave this here
                                 // in case someone changes their mind
                                 //else if (0.0 != lineItem.Volume.GrossInventoryChange && quantityChange != 0.0)
											else if(0.0 != lineItem.Quantity.NetInventoryChange && quantityChange != 0.0 && trans.TransVersion != 0)
                                 {
                                    //if (wacPrice != currentWac)
                                    if (currentWac != lineItem.Tax4.Value)
                                    {
                                       wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange * sign, wacPrice, currentWac);
                                       wacCalculatedThisRun = lineItem.WacCalculated = true;
                                    }
                                 }
                              }
                              else if (origLineItems != null || trans.TransVersion == 0)
                              {
                                 // if is unusable / quarantined defuel or return, then should use latest WAC
                                 WeightedAverageCostsClass wacs = new WeightedAverageCostsClass();
                                 WeightedAverageCostClass wac = wacs.GetLatest(security, 
                                    (int) trans.SiteIndex.Value, 
                                    (int)lineItem.ProductIndex.Value);

                                 if (wac != null)
                                    lineItem.Tax4 = new VDouble(wac.WacValue);
                              }

                              break;
                        }
                     }
                  }

                  if (lineItemHadNoWac || wacCalculatedThisRun)
                  {
                     lineItem.Tax4 = new VDouble(wacPrice);
                  }

                  //if (this.ShouldUseLatestWac(quantityChange, lineItem))

                  if ((TransactionTypes.T9_Request == trans.TransTypeID) ||
                      (TransactionTypes.T18_SupplyOrder == trans.TransTypeID) || // JS20100317 fuel orders will be standing offer as price
                      (TransactionTypes.T22_AccountReceivableInvoice == trans.TransTypeID))
                  {
                     //Just return price if already set and transaction type is one of Supply Order, Payment, or Recovery.
                     //Rest will be aggregated in TransactionDetails page by AggregateAssociatedTxValues
                     if (this.standingOfferID != null)
                     {
                        lineItem.ContractNumber = standingOfferID;
                     }

                     if (unitPrice == -1.0)
                     {
                        //Price was not set by user. Return standing offer price.
                        lineItem.ProductPrice.Value = so;
                     }
                  }

                  // JS20100208 fuel orders are unaffected by the WAC, use old method
                  if (trans.TransTypeID == TransactionTypes.T17_Order ||
                      trans.TransTypeID == TransactionTypes.T18_SupplyOrder)
                  {
                     if (unitPrice == -1.0)
                     {
                        lineItem.ProductPrice.Value = so;
                     }
                     else
                     {
                        lineItem.ProductPrice.Value = unitPrice;
                     }
                  }
                  // receipts always use fuel order price so do not set
                  else if (trans.TransTypeID != TransactionTypes.T8_Receipt)
                  {
                     // reversals should keep original WAC as product price
                     if (isReverse(trans) == false)
                     {
                        if (this.ShouldUseLatestWac(quantityChange, lineItem))
                        {
                           lineItem.ProductPrice.Value = wacPrice;
                        }
                        else
                        {
                           lineItem.ProductPrice.Value = lineItem.Tax4.Value;
                        }
                     }
                  }

                  // If receipt, quantity comes from alternative net volume
                  // If the product price was not found (-1), set the cost to be (so * q).
                  // Else if the product price was found set the cost to (unit price * q).
                  if (trans.TransTypeID == TransactionTypes.T8_Receipt)
                  {
                     double productPrice = unitPrice;
                     if (lineItem.ProductPrice != null)
                     {
                        if (!lineItem.ProductPrice.IsNull)
                           productPrice = lineItem.ProductPrice.Value;
                     }

                     //cost = lineItem.NetQuantityReceived * (unitPrice == -1.0 ? so : unitPrice);
                     // if you change this, change all places marked with [ReceiptPriceQuantity]
                     cost = (lineItem.AlternativeNetVolume == null ? 0.0 : lineItem.AlternativeNetVolume.Value)
                             * (productPrice == -1.0 ? so : productPrice);
                  }
                  else if (wacPrice > 0.0 &&
                          trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
                  {
                     cost = quantity * lineItem.Tax4.Value;
                  }
                  else if (unitPrice == -1.0)
                  {
                     cost = quantity * so;
                  }
                  else
                  {
                     cost = unitPrice * quantity;
                     this.DidPriceChange(lineItem, lineItemIndex);
                  }

                  // JS20091222 never recalculate price, it was deemed confusing to allow for that. Instad always recalculate taxes no matter
                  // what the user changes it to CCP-042. Exception is for invoices
                  this.recalculateTaxes = true;

                  if (trans.TransTypeID == TransactionTypes.T8_Receipt)
                  {
                     quantity = lineItem.AlternativeNetVolume == null ? 0.0 : lineItem.AlternativeNetVolume.Value;
                  }

                  // Recalculate GST, Excise, and Markup amounts.
                  if (!transIsSale)
                  {
                     // sales are calculated later
                     exciseAmount = this.CalculateExciseAmount(exciseAmount, quantity, exciseRate);
                     gstAmount = this.CalculateGSTAmount(gstAmount, gstRate, exciseAmount, cost);
                     markupAmount = this.CalculateMarkupAmount(markupAmount, gstAmount, exciseAmount, cost, markUpRate);
                  }

                  // if is reverse, then do not re-calculate taxes
                  if (!(trans.ReversalType == TransactionDO.Reversal ||
                      trans.ReversalType == TransactionDO.ReversalWithUpdate))
                  {

                     if ((TransactionTypes.T21_AccountPayableInvoice != trans.TransTypeID) && (this.transIsSale == false))
                     {
                        // do not set values for invoices, its values are populated from the association

                        //Excise Amount , GST Amount, and Markup Amount assgiments are defined in TR-FIN-0010
                        this.AllowExciseTaxOverride(lineItem, lineItemIndex, exciseAmount);
                        this.AllowGSTTaxOverride(lineItem, lineItemIndex, gstAmount);
                        this.AllowMarkupTaxOverride(lineItem, lineItemIndex, markupAmount);
                     }
                  }

                  // Handle Sales type transaction differently than other type of transactions. Sales
                  // must handle fuel being sold to non-Australian defence both foreign and domestic.
                  // JS20100212 Different cost of inventory also apply as per CCP-043
                  if (this.transIsSale == true || forceSaleRecalculation)
                  {
                     /**
                      * JS20100212 As per CCP-042, transaction no longer differentiate between overseas and local,
                      * instead they simply use the source and destination site/company rates to determine proper
                      * sales cost. However revenue is still identical in that it is cost ex GST inc margin.
                      **/
                     double baseCost = this.CalculateSaleRevenue(lineItem, lineItem.ProductPrice.Value, 0, 0, 0, Math.Abs(oncostAmount));
                     revenue = this.CalculateSaleRevenue(lineItem, lineItem.ProductPrice.Value, exciseRate, 0, markUpRate, Math.Abs(oncostAmount));
                     totalCost = this.CalculateSaleRevenue(lineItem, lineItem.ProductPrice.Value, exciseRate, gstRate, markUpRate, Math.Abs(oncostAmount));

                     // re-calculate GST rate
                     gstAmount = totalCost - revenue;

                     // re-calculate Excise rate
							exciseAmount = lineItem.Quantity.Gross * exciseRate;

                     // re-calculate Markup rate
                     markupAmount = this.CalculateSaleRevenue(lineItem, lineItem.ProductPrice.Value, 0, 0, markUpRate - 1.0, Math.Abs(oncostAmount));

                     if (!forceSaleRecalculation)
                     {
                        this.AllowGSTTaxOverride(lineItem, lineItemIndex, gstAmount);
                        this.AllowExciseTaxOverride(lineItem, lineItemIndex, exciseAmount);
                        this.AllowMarkupTaxOverride(lineItem, lineItemIndex, markupAmount);
                     }

                     // WI-15139 
                     if (lineItem.Tax1 == null || lineItem.Tax1.Value < 0 || forceSaleRecalculation)
                        lineItem.Tax1 = new VDouble(exciseAmount);
                     if (lineItem.Tax2 == null || lineItem.Tax2.Value < 0 || forceSaleRecalculation)
                        lineItem.Tax2 = new VDouble(gstAmount);
                     if (lineItem.Tax3 == null || lineItem.Tax3.Value < 0 || forceSaleRecalculation)
                        lineItem.Tax3 = new VDouble(markupAmount);

                     // if taxes are overwritten then adjust revenue and total cost
                     if (lineItem.Tax1.Value != exciseAmount)
                     {
                        totalCost = totalCost - exciseAmount + lineItem.Tax1.Value;
                        revenue = revenue - exciseAmount + lineItem.Tax1.Value;
                     }
                     if (lineItem.Tax2.Value != gstAmount)
                     {
                        // GST is calculated slightly differently
                        totalCost = totalCost - gstAmount + lineItem.Tax2.Value;
                        revenue = totalCost - lineItem.Tax2.Value;
                     }
                     if (lineItem.Tax3.Value != markupAmount)
                     {
                        totalCost = totalCost - markupAmount + lineItem.Tax3.Value;
                        revenue = revenue - markupAmount + lineItem.Tax3.Value;
                     }

                     if (trans.ReversalType == TransactionDO.Reversal ||
                     trans.ReversalType == TransactionDO.ReversalWithUpdate)
                     {
                        if (lineItem.Tax1 != null)
                           lineItem.Tax1.Value *= -1;
                        if (lineItem.Tax2 != null)
                           lineItem.Tax2.Value *= -1;
                        if (lineItem.Tax3 != null)
                           lineItem.Tax3.Value *= -1;

                        revenue *= -1;
                        totalCost *= -1;
                     }

                     // actual fuel price
                     if (lineItem.Number01 == null)
                     {
                        lineItem.Number01 = new VDouble(lineItem.ProductPrice.Value);
                     }
                     // actual excise
                     if (lineItem.Number02 == null)
                     {
                        lineItem.Number02 = new VDouble(lineItem.Tax1.Value);
                     }
                     // actual gst
                     if (lineItem.Number03 == null)
                     {
                        lineItem.Number03 = new VDouble(lineItem.Tax2.Value);
                     }
                     // actual margin
                     if (lineItem.Number04 == null)
                     {
                        lineItem.Number04 = new VDouble(lineItem.Tax3.Value);
                     }
                     // actual cost excl GST
                     if (lineItem.Number05 == null)
                     {
                        lineItem.Number05 = new VDouble(revenue);
                     }
                     // actual cost incl GST
                     if (lineItem.Number06 == null)
                     {
                        lineItem.Number06 = new VDouble(totalCost);
                     }
                  }
                  else
                  {
                     // Handle non-Sales transactions. Unit price setting is removed since Issue transactions
                     // do not have the ability to override the GST and Excise amounts.
                     revenue = cost - gstAmount + oncostAmount;
                     totalCost = cost + oncostAmount;
                  }

                  lineItem.TotalPriceWithTax = totalCost;
                  lineItem.TotalValue = revenue;

                  if (this.standingOfferID != null)
                  {
                     if (TransactionTypes.T18_SupplyOrder == trans.TransTypeID)
                     {
                        lineItem.ContractNumber = standingOfferID;
                     }
                  }
               }
#if DEBUG
               watch.Stop();

               EventLog eventLog = new EventLog("Application", ".", "FuelsManager");
               eventLog.WriteEntry("ADFPriceCalculator run for took " + watch.ElapsedMilliseconds + "ms\n" +
                                    "Transaction Alias = " + trans.Alias + "\n" +
                                    "Transaction ID = " + trans.TransID, EventLogEntryType.Information);
#endif // DEBUG
            }
            catch (Exception e)
            {
               EventLog eventLog = new EventLog("Application", ".", "FuelsManager");
               eventLog.WriteEntry(e.Message, EventLogEntryType.Error);

               throw e;
            }

            // Standing offer exception occurs if the standing offer is not found
            // or the a more recent one is found.
            if (standingOfferException != null)
            {
               throw standingOfferException;
            }

            return true;
        }

        /// <summary>
        /// This method will allow excise tax override for sales transactions.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="lineItemIndex"></param>
        /// <param name="exciseAmount"></param>
        private void AllowExciseTaxOverride(LineItemDO lineItem, int lineItemIndex, double exciseAmount)
        {
           if (this.transIsSale == true)
           {
              if ((this.origLineItems != null) && (this.origLineItems.Count > 0))
              {
                 if (lineItemIndex >= this.origLineItems.Count)
                 {
                    lineItem.Tax1.Value = exciseAmount;
                 }
                 else
                 {
                    LineItemDO origLineItem = this.origLineItems[lineItemIndex] as LineItemDO;

                    if ((lineItem.Tax1 != null) && (string.IsNullOrEmpty(lineItem.Tax1.Value.ToString()) == true))
                    {
                       lineItem.Tax1.Value = exciseAmount;
                    }
                    /*else if ((lineItem.Tax1 != null) && (origLineItem.Tax1 != null)) -- incorrect, will mess up costs on second apply
                    {
                       if (lineItem.Tax1.Value == origLineItem.Tax1.Value)
                       {
                          lineItem.Tax1.Value = exciseAmount;
                       }
                    }*/
                 }
              }
           }
           else
           {
              lineItem.Tax1 = new VDouble(exciseAmount);
           }
        }

        /// <summary>
        /// This method will allow GST tax override for sales transactions.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="lineItemIndex"></param>
        /// <param name="gstAmount"></param>
        private void AllowGSTTaxOverride(LineItemDO lineItem, int lineItemIndex, double gstAmount)
        {
           if (this.transIsSale == true)
           {
              if ((this.origLineItems != null) && (this.origLineItems.Count > 0))
              {
                 if (lineItemIndex >= this.origLineItems.Count)
                 {
                    lineItem.Tax2.Value = gstAmount;
                 }
                 else
                 {
                    LineItemDO origLineItem = this.origLineItems[lineItemIndex] as LineItemDO;

                    if ((lineItem.Tax2 != null) && (string.IsNullOrEmpty(lineItem.Tax2.Value.ToString()) == true))
                    {
                       lineItem.Tax2.Value = gstAmount;
                    }
                    /*else if ((lineItem.Tax2 != null) && (origLineItem.Tax2 != null)) -- incorrect, will mess up costs on second apply
                    {
                       if (lineItem.Tax2.Value == origLineItem.Tax2.Value)
                       {
                          lineItem.Tax2.Value = gstAmount;
                       }
                    }*/
                 }
              }
           }
           else
           {
              lineItem.Tax2 = new VDouble(gstAmount);
           }
        }

        /// <summary>
        /// This method will allow Markup tax override for sales transactions.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="lineItemIndex"></param>
        /// <param name="markupAmount"></param>
        private void AllowMarkupTaxOverride(LineItemDO lineItem, int lineItemIndex, double markupAmount)
        {
           if (this.transIsSale == true)
           {
              if ((this.origLineItems != null) && (this.origLineItems.Count > 0))
              {
                 if (lineItemIndex >= this.origLineItems.Count)
                 {
                    lineItem.Tax3.Value = markupAmount;
                 }
                 else
                 {
                    LineItemDO origLineItem = this.origLineItems[lineItemIndex] as LineItemDO;

                    if ((lineItem.Tax3 != null) && (string.IsNullOrEmpty(lineItem.Tax3.Value.ToString()) == true))
                    {
                       lineItem.Tax3.Value = markupAmount;
                    }
                    /*else if ((lineItem.Tax3 != null) && (origLineItem.Tax3 != null)) -- incorrect, will mess up costs on second apply
                    {
                       if (lineItem.Tax3.Value == origLineItem.Tax3.Value)
                       {
                          lineItem.Tax3.Value = markupAmount;
                       }
                    }*/
                 }
              }
           }
           else
           {
              lineItem.Tax3 = new VDouble(markupAmount);
           }
        }

        private bool isReverse(TransactionDO a_trans)
        {
            return a_trans.ReversalType == TransactionDO.Reversal
                    || a_trans.ReversalType == TransactionDO.Update
                    || a_trans.ReversalType == TransactionDO.ReversalWithUpdate;
        }

        private ArrayList RetrieveUniqueProductIndex(ArrayList a_lineItems)
        {
            ArrayList resultList = new ArrayList();
            Hashtable uniqueTable = new Hashtable();

            foreach (LineItemDO lineItem in a_lineItems)
            {
                uniqueTable[lineItem.ProductIndex] = new Object();
            }

            resultList.AddRange(uniqueTable.Keys);

            return resultList;
        }

        private double RetrieveLatestWAC(SecurityClass a_security, int a_siteIndex, int a_productIndex)
        {
            double returnVal = 0.0;

            // need to use the consolidated data object -> weighted average cost class here
            WeightedAverageCostsClass wacCollection = new WeightedAverageCostsClass();

            WeightedAverageCostClass wac = wacCollection.GetLatest(a_security, a_siteIndex, a_productIndex);

            if (wac != null)
            {
                returnVal = (double)wac.WacValue;
            }

            return returnVal; // placeholder
        }

        private ArrayList FilterLineItems(ArrayList a_lineItems, int a_productIndex)
        {
            ArrayList resultList = new ArrayList();

            if (null == a_lineItems) // failsafe
            {
                return resultList;
            }

            foreach (LineItemDO lineItem in a_lineItems)
            {
                if (a_productIndex == (int)lineItem.ProductIndex.Value)
                {
                    resultList.Add(lineItem);
                }
            }

            return resultList;
        }

        private double RetrieveBookInventory(SecurityClass a_security, TransactionDO a_trans, string a_product, DateTime a_dateTime)
        {
            double returnVal = 0.0;

            LedgerDO ledgerDO = null;

            LedgerSR ledgerSR = new LedgerSR();
            ledgerSR.Security = a_security;
            ledgerSR.Site = a_security.SiteID;
            ledgerSR.CurrentSiteIndex = a_security.SiteIndex;
            ledgerSR.Manager = a_trans.ManagerID;

            if (string.IsNullOrEmpty(a_trans.OwnerID))
            {
                ledgerSR.Owner = a_trans.ManagerID;
            }
            else
            {
                ledgerSR.Owner = a_trans.OwnerID;
            }
            ledgerSR.Product = a_product;
            ledgerSR.Month = FM7Accounting.DateEfficacy.ConvertToMonthAndYear(a_dateTime);
            ledgerSR.setRequestType(LedgerSR.LedgerRequests.REFRESH);

            try
            {
                // Get the ledger data
                ledgerDO = m_accountingService.request(ledgerSR) as LedgerDO;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            if (ledgerDO != null)
            {
                // use the book inventory for the current day (not the EOM because we do NOT want to take in account of today's physical inventory)
                LedgerLineItemDO curLedgerLineItem = ledgerDO.LedgerLineItems[a_dateTime.Day - 1] as LedgerLineItemDO;

                returnVal = curLedgerLineItem.BookInventory.NetInventoryChange;
            }

            return returnVal;
        }

        private double CalculateWAC(SecurityClass a_security, TransactionDO a_trans, string a_product, double a_delta, double a_price, double a_valueWac)
        {
           double origQty =
              this.RetrieveBookInventory(a_security, a_trans, a_product, DateTime.Now); ;

           // JS20100621 [SKAFTSV001]: for reverse and reverse updates, WAC is calculated after transactions are SAVED,
           // this means the original quantity is the inventory minus the one on the current transaction. this is
           // generic, on reverse will add the delta, on update will subtract the delta.
           if (a_trans.ReversalType.Equals(TransactionDO.ReversalWithUpdate) ||
               a_trans.ReversalType.Equals(TransactionDO.Update))
           {
              double modifier = 0.0;

              if (!string.IsNullOrEmpty((a_trans.LineItems[0] as LineItemDO).UserData["Transaction Alias Line Item User Data 24"].ToString()))
              {
                 modifier = (double) (a_trans.LineItems[0] as LineItemDO).UserData["Transaction Alias Line Item User Data 24"];

                 if (a_trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade ||
                    a_trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
                 {
                    if (a_delta < 0)
                    {
                       modifier *= -1;
                    }
                 }
              }

              origQty -= a_delta + modifier;
           }

            double origValue = origQty * a_valueWac;

            double result = this.CalculateWACEx(a_delta, a_price, origQty, origValue);

            return result;
        }

        private double CalculateWACEx(double a_delta, double a_price, double a_origQty, double a_origValue)
        {
            double wac = 0.0;

            double transactionValue = a_price * a_delta;

            double quantityOnHand = a_origQty + a_delta;
            double valueOnHand = a_origValue + transactionValue;

            if (quantityOnHand != 0.0)
            {
                wac = valueOnHand / quantityOnHand;
            }

            return wac;
        }

        /// <summary>
        /// This method will return true if the Product Price was changed by the 
        /// user. Otherwise, it return false.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="lineItemIndex"></param>
        private void DidPriceChange(LineItemDO lineItem, int lineItemIndex)
        {
            this.recalculateTaxes = false;

            if ((this.origLineItems != null) && (lineItemIndex < this.origLineItems.Count))
            {
                LineItemDO origLineItem = this.origLineItems[lineItemIndex] as LineItemDO;

                if (origLineItem != null)
                {
                    if ((lineItem.ProductPrice != null)
                       && (lineItem.ProductPrice.IsNull == false)
                       && (origLineItem.ProductPrice != null)
                       && (origLineItem.ProductPrice.IsNull == false))
                    {
                        // Since there were doubles the price may never be exactly the same, therefore
                        // only check to the 4th decimal position.
                        double prodPrice1 = Math.Round(lineItem.ProductPrice.Value, ADFPriceCalculatorClass.CurrencyPercision, MidpointRounding.AwayFromZero);
                        double prodPrice2 = Math.Round(origLineItem.ProductPrice.Value, ADFPriceCalculatorClass.CurrencyPercision, MidpointRounding.AwayFromZero);

                        if (prodPrice1 != prodPrice2)
                        {
                            this.recalculateTaxes = true;
                        }
                    }

						  if((lineItem.Quantity != null) && (origLineItem.Quantity != null))
                    {
							  double quantity1 = lineItem.Quantity.Gross;
							  double quantity2 = origLineItem.Quantity.Gross;

                        if (quantity1 != quantity2)
                        {
                            this.recalculateTaxes = true;
                        }
                    }

                    // mark up needs to be recalculated if asset is leaving australia
                    if (lineItem.Flag04 != origLineItem.Flag04)
                    {
                        this.recalculateTaxes = true;
                    }
                }
            }
        }

        /// <summary>
        /// This method will calculate the new SO based on the changes to Excise and GST amounts.
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="exciseAmt"></param>
        /// <param name="exciseRate"></param>
        /// <param name="gstAmt"></param>
        /// <returns></returns>
        private double GetNewCalculatedSO(double quantity, double exciseAmt, double exciseRate, double gstAmt)
        {
            double newSO = 0.0;
            double a = (exciseAmt + (exciseAmt * exciseRate)) + (exciseRate * gstAmt);
            double b = quantity * exciseRate;

            if (b != 0.0)
            {
                newSO = a / b;
            }

            return newSO;
        }

        /// <summary>
        /// This method will return GST amount based on the GST rate. It will return the same value
        /// if the GST greater than zero.
        /// </summary>
        /// <param name="gstRate"></param>
        /// <param name="cost"></param>
        /// <param name="recalculate"></param>
        /// <returns></returns>
        private double CalculateGSTAmount(double gstAmt, double gstRate, double exciseAmt, double cost)
        {
            if ((gstAmt < 0.0) || (this.recalculateTaxes == true))
            {
               if (trans.TransTypeID == TransactionTypes.T8_Receipt)
               {
                  gstAmt = cost - cost / (1 + gstRate);
               }
               else
               {
                  gstAmt = (cost + exciseAmt) * gstRate;
               }
            }

            return gstAmt;
        }

        /// <summary>
        /// This method will return excise amount based on the excise rate. It will return the same value
        /// if the excise greater than zero.
        /// </summary>
        /// <param name="gstAmount"></param>
        /// <param name="cost"></param>
        /// <param name="excistRate"></param>
        /// <returns></returns>
        private double CalculateExciseAmount(double exciseAmount,
                                             double quantity,
                                             double exciseRate)
        {
            if ((exciseAmount < 0.0) || (this.recalculateTaxes == true))
            {
                exciseAmount = quantity * exciseRate;
            }

            return exciseAmount;
        }

        /// <summary>
        /// This method will return the markup amount based on markup rate when leaving Australia and
        /// zero if consumed in Australia.
        /// </summary>
        /// <param name="gstAmount"></param>
        /// <param name="exciseAmount"></param>
        /// <param name="costWithTax"></param>
        /// <param name="markupRate"></param>
        /// <param name="recalculate"></param>
        /// <returns></returns>
        private double CalculateMarkupAmount(double markupAmount,
                                             double gstAmount,
                                             double exciseAmount,
                                             double cost,
                                             double markupRate)
        {
            if ((markupAmount < 0.0) || (this.recalculateTaxes == true))
            {
                if (this.transIsSale == true)
                {
                    if (this.leavingAustralia == true)
                    {
                        double gstAmt = gstAmount;
                        double exciseAmt = exciseAmount;

                        // Fuels consumed outside Australia will include mark-ups with taxes
                        markupAmount = (cost - exciseAmount) * markupRate;
                    }
                    else
                    {
                        markupAmount = cost * markupRate;
                    }
                }
                else
                {
                    markupAmount = 0.0;
                }
            }

            return markupAmount;
        }

        /// <summary>
        /// This method will return the GST configurated in the system. It will
        /// return zero if not found.
        /// </summary>
        /// <returns></returns>
        private double GetGSTRate(DateTime transactionDateTime)
        {
            double gstRate = 0.0;

            if (this.leavingAustralia == true)
            {
               return 0.0;
            }

            string sourceCompany = "";
            if (transIsSale)
            {
                sourceCompany = trans.BillToID == null ? "" : trans.BillToID;
            }
            else if (trans.TransTypeID == TransactionTypes.T18_SupplyOrder ||
                    trans.TransTypeID == TransactionTypes.T8_Receipt)
            {
                sourceCompany = trans.SupplierID == null ? "" : trans.SupplierID;
            }
            else
            {
                return 0.0; // all other transactions do not have taxes
            }

            if (!string.IsNullOrEmpty(sourceCompany) // has a bill to
               && (this.trans.TransTypeID != TransactionTypes.T21_AccountPayableInvoice) // invoice
               && (this.trans.TransTypeID != TransactionTypes.T22_AccountReceivableInvoice) // recovery
               && (this.trans.TransTypeID != TransactionTypes.T9_Request) // demand
                )
            {
                GoodsAndServicesTaxBL gstTaxBL = new GoodsAndServicesTaxBL();
                GoodsAndServicesTaxDOCollection col = gstTaxBL.GetAll();

                GoodsAndServicesTaxDO gstDO = null;
                DateTime curGstDateTime = DateTime.MinValue;

                foreach (GoodsAndServicesTaxDO curGstDO in col)
                {
                    if (curGstDO != null)
                    {
                        List<TaxCompanyMapDO> companyMap = gstTaxBL.GetGSTCompanies(curGstDO, this.security);
                        if (IsAssignedToCompany(companyMap, sourceCompany) && // assigned to bill-to
                            (ulong)curGstDO.GstDate.ToBinary() > (ulong)curGstDateTime.ToBinary() && // only find most recent
                            (ulong)curGstDO.GstDate.ToBinary() <= (ulong)transactionDateTime.ToBinary() // not in the future
                            )
                        {
                            gstDO = curGstDO;
                            curGstDateTime = gstDO.GstDate;
                        }
                    }
                }

                if (gstDO == null)
                {
                    ThrowNotConfigured(sourceCompany + " GST");
                }

                gstRate = gstDO.GstValue / 100.0;
            }

            return gstRate;
        }

        /// <summary>
        /// This method will return the Excise configurated in the system. It will
        /// return zero if not found.
        /// </summary>
        /// <param name="prodIndex"></param>
        /// <param name="transactionDateTime"></param>
        /// <param name="sourceCompany"></param>
        /// <returns></returns>
        private double GetExciseRate(int prodIndex, DateTime transactionDateTime, string sourceCompany)
        {
            double exciseRate = 0.0;

            if (!string.IsNullOrEmpty(sourceCompany) // has a bill to
               && (this.trans.TransTypeID != TransactionTypes.T21_AccountPayableInvoice) // invoice
               && (this.trans.TransTypeID != TransactionTypes.T22_AccountReceivableInvoice) // recovery
               && (this.trans.TransTypeID != TransactionTypes.T9_Request) // demand
                )
            {
                ExciseTaxBL exciseTaxBL = new ExciseTaxBL();
                ExciseTaxDOCollection col = exciseTaxBL.GetAll(this.security);

                ExciseTaxDO exciseDO = null;
                DateTime curExciseDateTime = DateTime.MinValue;

                foreach (ExciseTaxDO curExciseDO in col)
                {
                    if (curExciseDO != null)
                    {
                        List<TaxCompanyMapDO> companyMap = exciseTaxBL.GetExciseCompanies(curExciseDO, this.security);
                        if (IsAssignedToCompany(companyMap, sourceCompany) && // assigned to bill-to
                            (ulong)curExciseDO.ExciseDate.ToBinary() > (ulong)curExciseDateTime.ToBinary() && // only find most recent
                            (ulong)curExciseDO.ExciseDate.ToBinary() <= (ulong)transactionDateTime.ToBinary() && // not in the future
                            curExciseDO.ProductIndex == prodIndex // matching product
                            )
                        {
                            exciseDO = curExciseDO;
                            curExciseDateTime = exciseDO.ExciseDate;
                        }
                    }
                }

                if (exciseDO == null)
                {
                    ThrowNotConfigured(sourceCompany + " Excise");
                }

                exciseRate = exciseDO.ExciseRate; // / 100.0;
            }

            return exciseRate;
        }

        /// <summary>
        /// This method will return the excise rate based on the change in the Excise Amount or
        /// based on the Excise rate retrieved from the configuration.
        /// </summary>
        /// <param name="prodIndex"></param>
        /// <returns></returns>
        private double GetExciseRate(int prodIndex, DateTime transactionDateTime)
        {
            string sourceCompany = "";

            if (transIsSale)
            {
                sourceCompany = trans.BillToID == null ? "" : trans.BillToID;
            }
            else if (trans.TransTypeID == TransactionTypes.T18_SupplyOrder ||
                        trans.TransTypeID == TransactionTypes.T8_Receipt)
            {
                sourceCompany = trans.SupplierID == null ? "" : trans.SupplierID;
            }
            else
            {
                return 0.0; // all other transactions do not have taxes
            }

            return this.GetExciseRate(prodIndex, transactionDateTime, sourceCompany);
        }

        /// <summary>
        /// This method will return the markup rate based on the change in the markup Amount or
        /// based on the markup rate retrieved from the configuration.
        /// </summary>
        /// <returns></returns>
        private double GetMarkupRate()
        {
            double markUpRate = 0.0;

            if (this.transIsSale)
            {
                CompaniesClass companiesBL = new CompaniesClass();
                int companyIndex = (this.trans.BillToID == null || this.trans.BillToIndex.IsNull) ? -1 : (int)this.trans.BillToIndex.Value;
                CompanyClass billTo = companiesBL.Get(this.security, companyIndex);

                if (billTo != null)
                {
                    markUpRate = this.GetCompanyMarkupRate(billTo.ID);
                    markUpRate /= 100.0;
                }
            }

            return markUpRate;
        }

        /// <summary>
        /// This method will set the default values for excise, GST, markup, unit price and the line item
        /// values. It will return the either excise, gst, markup, or unit price.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private double SetLineItemValues(LineItemDO lineItem, ADFPriceCalculatorClass.LineItemTypes type)
        {
            double returnValue = -1.0;

            switch (type)
            {
                case ADFPriceCalculatorClass.LineItemTypes.EXCISE:
                    if (lineItem.Tax1 != null)
                    {
                        returnValue = lineItem.Tax1.Value;
                    }
                    break;
                case ADFPriceCalculatorClass.LineItemTypes.GST:
                    if (lineItem.Tax2 != null)
                    {
                        returnValue = lineItem.Tax2.Value;
                    }
                    break;
                case ADFPriceCalculatorClass.LineItemTypes.MARKUP:
                    if (lineItem.Tax3 != null)
                    {
                        returnValue = lineItem.Tax3.Value;
                    }
                    break;
                case ADFPriceCalculatorClass.LineItemTypes.PRODUCT_PRICE:
                    // for receipts, product price is pulled from the fuel order
                    if ((trans.TransTypeID == TransactionTypes.T8_Receipt)
                       && lineItem.AssociatedTransactions.Count > 0)
                    {
                        AssociatedTxDO atx = lineItem.AssociatedTransactions[0] as AssociatedTxDO;
                        
                        // find the fuel order linked
                        AssociatedTxSR sr = new AssociatedTxSR();
                        sr.Security = security;
                        sr.LineItemID = (int) atx.LineItemID;
                        sr.TransID = atx.TransID;
                        sr.RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions;

                        AssociatedTxListDO result = m_accountingService.request(sr) as AssociatedTxListDO;

                        foreach (DataRow dr in result.AssociatedTransactions.Tables[0].Rows)
                        {
                            AssociatedTxDO demandParentTrans = new AssociatedTxDO();
                            demandParentTrans.Load(dr);

                            if (demandParentTrans.TransTypeID == TransactionTypes.T18_SupplyOrder && 
                                demandParentTrans.LinkedLineItemID == atx.LineItemID &&
                                demandParentTrans.Product.ToUpper().Equals(atx.Product.ToUpper()))
                            {
                                lineItem.ProductPrice = new VDouble(demandParentTrans.ProductPrice);

                               // also should pull in the fuel order forex stuff if any
                                SetForeignCurrencyValues(ref lineItem, demandParentTrans.TransID, demandParentTrans.LineItemID);
                            }
                        }
                    }
                    /*if ((trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
                      && lineItem.AssociatedTransactions.Count > 0)
                    {
                       AssociatedTxDO atx = lineItem.AssociatedTransactions[0] as AssociatedTxDO;
                       if (atx != null)
                       {
                          lineItem.CurrencyUnit = atx.CurrencyUnit;
                       }
                    }*/
                    if ((lineItem.ProductPrice != null) && (lineItem.ProductPrice.IsNull == false) && (lineItem.ProductPrice.Value > 0.0))
                    {
                        returnValue = lineItem.ProductPrice.Value;
                    }

                    if (lineItem.ProductPrice == null)
                    {
                        lineItem.ProductPrice = new VDouble(0.0);
                    }
                    break;
                case ADFPriceCalculatorClass.LineItemTypes.WAC:
                    if (lineItem.Tax4 == null)
                    {
                        lineItem.Tax4 = new VDouble(0.0);
                    }
                    else
                    {
                        returnValue = lineItem.Tax4.Value;
                    }
                    break;
                case ADFPriceCalculatorClass.LineItemTypes.ONCOST:
                    if (string.IsNullOrEmpty(lineItem.UserData["Transaction Alias Line Item User Data 14"] as string))
                    {
                        lineItem.UserData[14] = "0.0";
                        returnValue = 0.0;
                    }
                    else
                    {
                        string udValue = lineItem.UserData["Transaction Alias Line Item User Data 14"] as string;
                        try
                        {
                            returnValue = double.Parse(udValue);
                        }
                        catch (Exception)
                        {
                            returnValue = 0.0;
                        }
                    }
                    break;
                case ADFPriceCalculatorClass.LineItemTypes.QUANTITY:
						  returnValue = lineItem.Quantity.Gross;
                    break;
            }

            return returnValue;
        }

       protected void SetForeignCurrencyValues(ref LineItemDO a_lineItem, string a_transID, long a_lineItemID)
       {
          LineItemDO foundLineItem = null;

          try
          {
             TransactionSR transSr = new TransactionSR();
             transSr.Security = security;
             transSr.TransID = a_transID;

             TransactionDO parentTrans = m_accountingService.request(transSr) as TransactionDO;
             if (parentTrans != null)
             {
                foreach (LineItemDO li in parentTrans.LineItems)
                {
                   if (li.LineItemID == a_lineItemID)
                   {
                      foundLineItem = li;
                      break;
                   }
                }
             }
          }
          catch (Exception) { }

          if (foundLineItem != null)
          {
             a_lineItem.CurrencyUnit = foundLineItem.CurrencyUnit;
             a_lineItem.NonDomesticPrice = foundLineItem.NonDomesticPrice;
          }
       }

        protected bool IsAssignedToCompany(List<TaxCompanyMapDO> a_companyMap, string a_companyName)
        {
            foreach (TaxCompanyMapDO map in a_companyMap)
            {
                if (map.CompanyID.ToUpper().Equals(a_companyName.ToUpper()))
                {
                    return true;
                }
            }

            return false;
        }

        protected double GetCompanyMarkupRate(string a_companyName)
        {
            double markupRate = 0.0;

            MarkupBL bl = new MarkupBL();

            MarkupDOCollection collection = bl.GetAll(this.security);

            bool found = false;

            DateTime curMarkupDateTime = DateTime.MinValue;

            foreach (MarkupDO markup in collection)
            {
                List<TaxCompanyMapDO> companyMap = bl.GetMarkupCompanies(markup, this.security);

                found = IsAssignedToCompany(companyMap, a_companyName);
                if (found)
                {
                    markupRate = markup.MarkupRate;
                    break;
                }
            }

            if (!found)
            {
                ThrowNotConfigured(a_companyName + " Margin");
            }

            return markupRate; // placeholder
        }

        protected void ThrowNotConfigured(string a_source)
        {
            string errorMsg = a_source + " " + ADFPriceCalculatorClass.NOT_CONFIGURED;

            throw new Exception(errorMsg);
        }

        /// <summary>
        /// This method will retrieve the markup rate associated to the ship-to company.
        /// </summary>
        /// <returns></returns>
        private double GetAssociatedMarkupRate()
        {
            //CompanyGroupsClass CompanyGroups = new CompanyGroupsClass();
            //CompanyGroupCollectionClass CompanyGroupColl = CompanyGroups.Enumerate(this.security);
            double markUpRate = 0.0;

            if (this.trans.ShipToIndex != null)
            {
                markUpRate = this.GetCompanyMarkupRate(this.trans.ShipToID);
            }

            return markUpRate;
        }

        /// <summary>
        /// This method will return the standing offer price for all transaction except the
        /// physical inventory transactions. For the physical inventory transaction, the
        /// average unit price is calculated from the receipts.
        /// </summary>
        /// <param name="standingoffers"></param>
        /// <param name="productIndex"></param>
        /// <returns></returns>
        private double GetStandingOfferPrice(LineItemDO lineItem, int supplierIndex)
        {
            double price = 0.0;

            FinanceSR financeSR        = new FinanceSR();
            financeSR.SiteIndex        = this.security.SiteIndex;
            financeSR.Site             = this.security.SiteID;
            financeSR.StartDate        = this.trans.InventoryDate;
            financeSR.ProductIndex     = (int)lineItem.ProductIndex.Value;
            financeSR.SupplierIndex    = supplierIndex;
            financeSR.Security         = this.security;
            financeSR.DeliveryLocation = lineItem.DeliveryLocation;
            financeSR.SubRequest       = FinanceSR.SUB_REQUEST.STANDING_OFFER_PRICE;
				financeSR.Quantity         = lineItem.Quantity.Gross;

            if ((this.trans.Alias.ToUpper().Contains("FUEL ORDER") == true)
               &&(lineItem.AssociatedTransactions != null) 
               && (lineItem.AssociatedTransactions.Count > 0))
            {
               AssociatedTxDO associatedTxDO = lineItem.AssociatedTransactions[0] as AssociatedTxDO;
               if (associatedTxDO != null)
               {
                  financeSR.DeliveryLocation = associatedTxDO.DeliveryLocation;
               }
            }

            this.financeDO = m_accountingService.request(financeSR) as FinanceDO;

            if (financeDO != null)
            {
                price = financeDO.CurrentStandingOfferPrice;
            }

            return price;
        }

        /// <summary>
        /// This method will return either a AUP price if one in found or a 
        /// standing offer price.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="supplierIndex"></param>
        /// <returns></returns>
        private double GetAUPPrice(LineItemDO lineItem, int supplierIndex)
        {
            double price = 0.0;

            FinanceSR financeSR = new FinanceSR();
            financeSR.SiteIndex = this.security.SiteIndex;
            financeSR.Site = this.security.SiteID;
            financeSR.StartDate = this.trans.InventoryDate;
            financeSR.ProductIndex = (int)lineItem.ProductIndex.Value;
            financeSR.SupplierIndex = supplierIndex;
            financeSR.Security = this.security;
            financeSR.DeliveryLocation = lineItem.DeliveryLocation;
            financeSR.SubRequest = FinanceSR.SUB_REQUEST.AVERAGE_UNIT_PRICE;

            FinanceDO financeDO = m_accountingService.request(financeSR) as FinanceDO;

            if (financeDO.ContainsAverageUnitPrice == true)
            {
                price = financeDO.AverageGrossUnitPrice;
            }
            else
            {
                price = this.GetStandingOfferPrice(lineItem, supplierIndex);
            }

            return price;
        }

        /// <summary>
        /// This method returns the accounting connection string to the database.
        /// </summary>
        /// <returns></returns>
        private string AccountingConnectionString()
        {
            string ValueString = "ConnectString";
            Microsoft.Win32.RegistryKey Key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Varec\\Accounting", true);
            string Connect = null;

            if (Key != null)
            {
                Connect = (string)Key.GetValue(ValueString);
            }

            return Connect;
        }

        protected enum ServiceType
        {
            AVIATION,
            GROUND,
            MARINE,
            WASTE
        }

        protected bool InProductGroup(LineItemDO a_lineItem, string a_match)
        {
            bool result = false;

            /*ProductGroupsClass prodGroups = new ProductGroupsClass();
            ProductGroupCollectionClass prodGroupCol = prodGroups.Enumerate(this.security);

            ProductGroupClass prodGroup = null;

            foreach (ProductGroupClass grp in prodGroupCol)
            {
                if (grp.IsProductInGroup((int)a_lineItem.ProductIndex.Value))
                {
                    prodGroup = grp;
                    break;
                }
            }

            if (prodGroup != null)
            {
                foreach (ProductMapClass map in prodGroup.ProductMapCollection)
                {
                    result = map.ID.ToUpper().Equals(a_match.ToUpper());
                    if (result)
                        break;
                }
            }*/
            
            ProductMapsClass productMaps = new ProductMapsClass();
            ProductMapCollectionClass mapCol = productMaps.EnumerateByType(this.security, PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP);

            for (int i = 0; i < mapCol.Count; ++i)
            {
                ProductMapClass map = mapCol[i];
                if (map.AssignedToID.ToUpper().Equals(a_match.ToUpper()) &&
                    map.AssignedID.ToUpper().Equals(a_lineItem.Product.ToUpper()))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        protected void InitServiceTable(ref Hashtable m_serviceTable)
        {
            m_serviceTable = new Hashtable();

            foreach (ServiceType service in Enum.GetValues(typeof(ServiceType)))
            {
                m_serviceTable.Add(service, service.ToString());
            }
        }

        protected double StripTaxes(LineItemDO a_lineItem, double a_price, double a_gstRate, double a_exciseRate)
        {
            double newPrice = a_price;

            if (this.InProductGroup(a_lineItem, m_serviceTable[ServiceType.AVIATION].ToString()) ||
                    this.InProductGroup(a_lineItem, m_serviceTable[ServiceType.GROUND].ToString()))
            {
                // aviation & ground fuel strips gst only
                newPrice = a_price / (1 + a_gstRate);
            }
            else if (this.InProductGroup(a_lineItem, m_serviceTable[ServiceType.MARINE].ToString()))
            {
                // marine fuel strips gst and excise
                //newPrice = (a_price / (1 + a_gstRate)) / (1 + a_exciseRate);
                newPrice = (a_price / (1 + a_gstRate)) - a_exciseRate;
                //newPrice = newPrice * a_gstRate;
                //newPrice = newPrice * a_exciseRate;
            }

            //newPrice = (a_price / (1 + a_gstRate)) - 1 + a_exciseRate; -- leave this here in case someone changes their mind

            return newPrice;
        }

        protected double CalculateSaleRevenue(LineItemDO lineItem, double wacPrice, double exciseRate, double gstRate, double markUpRate, double oncostAmount)
        {
            double result = 0.0;

            // need to work out seller excise
				double qty = lineItem.Quantity.Gross;
            ExciseTaxBL exciseBl = new ExciseTaxBL();

            double sellerExciseRate = this.GetExciseRate(
                    (int) lineItem.ProductIndex.Value,
                    this.trans.TransactionDateTime == null ? DateTime.Now : this.trans.TransactionDateTime.Value,
                    this.trans.Site);

            // equation pulled from CCP-043 - ((QTY x (WAC - Seller Excise) + On-Cost) x (1 + Margin) + (QTY x Customer Excise)) x (1 + GST)
            result = ((qty * (wacPrice - sellerExciseRate) + oncostAmount) * (1 + markUpRate) + (qty * exciseRate)) * (1 + gstRate);

            return result;
        }

        protected bool QualityWasNotUsable(SecurityClass a_security, TransactionDO a_trans, LineItemDO a_lineItem)
        {
            bool returnVal = false;

           // JS20100716 Performance, on most transactions this is not needed, only transactions which has a usable function
           // which impact the WAC.
            if (trans.TransTypeID == TransactionTypes.T8_Receipt ||
               trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade ||
               trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade ||
               trans.TransTypeID == TransactionTypes.T3_PrimaryDefuel ||
               trans.TransTypeID == TransactionTypes.T4_SecondaryDefuel)
            {
               try
               {
                  TransactionSR sr = new TransactionSR();
                  sr.Security = a_security;
                  sr.TransID = a_trans.TransID;

                  TransactionDO orig = (TransactionDO)m_accountingService.request(sr);
                  if (orig != null)
                  {
                     // find the line item
                     foreach (LineItemDO li in orig.LineItems)
                     {
                        if (li.LineItemID == a_lineItem.LineItemID && li.Quality != TransactionQuality.Usable)
                        {
                           returnVal = true;
                           break;
                        }
                     }
                  }
               }
               catch (Exception e)
               {
                  throw e;
               }
            }

            return returnVal;
        }

        protected bool ShouldUseLatestWac(double quantityChange, LineItemDO lineItem)
        {
           bool returnVal;

           if (trans.ReversalType == TransactionDO.Update && origTrans == null)
           {
              // new updates are different because they use original WAC price
              returnVal = false;
           }
           else
           {
              returnVal = (quantityChange != 0.0 && lineItem.LineItemID > 0) ||
                  ((lineItem.Quality == TransactionQuality.Usable && QualityWasNotUsable(security, trans, lineItem)) &&
                  (trans.Alias.ToUpper().Equals("DEFUEL") || trans.Alias.ToUpper().Equals("RETURN")));
           }

           return returnVal;
        }
    }
}
