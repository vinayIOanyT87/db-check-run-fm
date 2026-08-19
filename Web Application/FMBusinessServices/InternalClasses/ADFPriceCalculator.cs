// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ADFPriceCalculator.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.ServiceClasses;

	public class ADFPriceCalculatorClass
	{
		#region Constants and Fields

		protected const string NOT_CONFIGURED = "is not configured";

		protected Hashtable serviceTable = null;

		private const int CurrencyPercision = 7;

		private readonly AssociatedTxProcessorClass associatedTxProcessor;

		private readonly ExcisesClass exciseTaxBL;

		private readonly GoodsAndServicesClass gstTaxBL;

		private readonly LedgerProcessorClass ledgerProcessor;

		private readonly MarkupsClass markupBL;

		private readonly TransactionDO origTrans = null;

		private readonly TransactionProcessorClass txProcessor;

		private readonly StopWatch watch;

		private FinanceDO financeDO;

		private bool forceSaleRecalculation;

		private bool leavingAustralia;

		private List<LineItemDO> origLineItems;

		private bool recalculateTaxes;

		private SaveWeightedAverageCostsProcessorClass saveWacProcessor;

		private SecurityClass security;

		private string standingOfferID;

		private TransactionDO trans;

		private bool transIsIssue;

		private bool transIsSale;

		private bool wacCalculatedThisRun;

		#endregion

		#region Constructors and Destructors

		public ADFPriceCalculatorClass()
		{
			this.ledgerProcessor = new LedgerProcessorClass();
			this.txProcessor = new TransactionProcessorClass();
			this.associatedTxProcessor = new AssociatedTxProcessorClass();
			this.exciseTaxBL = new ExcisesClass();
			this.markupBL = new MarkupsClass();
			this.gstTaxBL = new GoodsAndServicesClass();
			this.saveWacProcessor = new SaveWeightedAverageCostsProcessorClass();

			this.watch = new StopWatch(StopWatch.Appnames.AccountingBLL, string.Empty);
		}

		#endregion

		#region Enums

		protected enum ServiceType
		{
			AVIATION, 

			GROUND, 

			MARINE, 

			WASTE
		}

		private enum LineItemTypes
		{
			EXCISE, 

			GST, 

			MARKUP, 

			PRODUCT_PRICE, 

			WAC, 

			ONCOST, 

			QUANTITY, 

			NONE
		};

		#endregion

		#region Public Properties

		public TransactionDO TransDO
		{
			get
			{
				return this.TransDO;
			}
		}

		#endregion

		#region Public Methods and Operators

		public bool Calculate(
			SecurityClass security, TransactionDO trans, List<LineItemDO> origLineItems, bool bForceRecalculation)
		{
			FMStandingOfferException standingOfferException = null;

			// need a try for the whole price calculator because exceptions are being shown on the interface
			try
			{
#if DEBUG
				this.watch.Start();
#endif

				// DEBUG
				this.security = security;
				this.trans = trans;
				this.origLineItems = origLineItems;
				this.forceSaleRecalculation = false;

				if ((this.security == null) || (this.trans == null) || (this.trans.LineItems == null)
				    || (this.trans.LineItems.Count == 0)
				    || (this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice && origLineItems == null)
				    || (this.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice && origLineItems == null))
				{
					return false;
				}

				// Direct Fuel Purchase and Commercial Purchase both are immune to price calculations
				if ((this.trans.TransTypeID == TransactionTypes.T12_InventoryNotAffected)
				    && (this.trans.Alias.ToUpper().Contains("DIRECT FUEL PURCHASE")
				        || this.trans.Alias.ToUpper().Contains("COMMERCIAL")))
				{
					return true;
				}

				this.transIsSale = trans.Alias.ToUpper().Contains("SALE");
				this.transIsIssue = trans.Alias.ToUpper().Contains("ISSUE");
				int lineItemIndex = -1;

				if (null == this.serviceTable)
				{
					this.InitServiceTable(ref this.serviceTable);
				}

				foreach (LineItemDO lineItem in this.trans.LineItems)
				{
					bool lineItemHadNoWac = false;
					this.wacCalculatedThisRun = false;

					this.leavingAustralia = lineItem.Flag04;

					// Used for finding the original line item values. JS20100621 changed to ++lineItemIndex for performance gain
					++lineItemIndex;

					// Cannot not calculate pricing if the product does not exist.
					// Go to the next line item.
					if (lineItem.ProductGuid == Guid.Empty)
					{
						continue;
					}

					this.standingOfferID = null;

					double revenue = 0.0; // baseCost + markUpAmount
					double totalCost = 0.0; // gstAmount + exciseAmount + revenue
					double cost = 0.0; // This will be either (so * q) or (unit price * q)
					Guid supplierGuid = Guid.Empty;

					if (this.trans.SupplierCompanyGuid != Guid.Empty)
					{
						supplierGuid = this.trans.SupplierCompanyGuid;
					}

					// The the amounts from the lineItem.
					double unitPrice = this.SetLineItemValues(lineItem, LineItemTypes.PRODUCT_PRICE);
					double markupAmount = this.SetLineItemValues(lineItem, LineItemTypes.MARKUP);
					double gstAmount = this.SetLineItemValues(lineItem, LineItemTypes.GST);
					double exciseAmount = this.SetLineItemValues(lineItem, LineItemTypes.EXCISE);
					double wacPrice = this.SetLineItemValues(lineItem, LineItemTypes.WAC);
					double oncostAmount = this.SetLineItemValues(lineItem, LineItemTypes.ONCOST);
					double quantity = this.SetLineItemValues(lineItem, LineItemTypes.QUANTITY);

					// Set Quantity
					double so = -1.0;

					double quantityChange = 0.0;

					// JS20100716 Performance, transactions which do not impact inventory do not care about the inventory change
					if (trans.TransTypeID != TransactionTypes.T21_AccountPayableInvoice
					    && trans.TransTypeID != TransactionTypes.T22_AccountReceivableInvoice
					    && trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
					{
						var saveWacProcessor = new SaveWeightedAverageCostsProcessorClass();
						quantityChange = saveWacProcessor.QuantityChangedSinceLastSave(this.trans, lineItem);

						// if WAC is not set then retrieve the latest for the new line item
						if ((wacPrice <= 0.0 || this.ShouldUseLatestWac(quantityChange, lineItem)) && origLineItems != null)
						{
							wacPrice = this.RetrieveLatestWAC(security, trans.SiteGuid, lineItem.ProductGuid);
							lineItemHadNoWac = true;
						}
					}

					// Always look for the average unit price of physical inventories.
					if (this.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory)
					{
						so = this.GetAUPPrice(lineItem, supplierGuid);
					}
					else
					{
						so = this.GetStandingOfferPrice(lineItem, supplierGuid);
					}

					// Cannot calculate pricing if the price list price (aka standing offer) is less than zero
					// and the user did not enter in a unit price. Or that the WAC doesn't exist.
					if ((so <= 0.0) && (unitPrice <= 0.0) && (wacPrice <= 0.0))
					{
						if (trans.TransTypeID == TransactionTypes.T14_PhysicalInventory)
						{
							lineItem.ProductPrice = 0.0;
						}
						else
						{
							// This sets the product price to null.
							lineItem.ProductPrice = null;
						}

						if ((this.financeDO != null) && this.financeDO.HasMessage
						    && (trans.TransTypeID == TransactionTypes.T18_SupplyOrder) && trans.Alias.ToUpper().Contains("FUEL ORDER"))
						{
							standingOfferException = new FMStandingOfferException(this.financeDO.InfoMessage);
							standingOfferException.ContinueOn = false;
						}

						continue;
					}
					else if ((so > 0.0) && (this.financeDO != null) && this.financeDO.HasMessage
					         && (trans.TransTypeID == TransactionTypes.T18_SupplyOrder) && trans.Alias.ToUpper().Contains("FUEL ORDER"))
					{
						standingOfferException = new FMStandingOfferException(this.financeDO.InfoMessage);
						standingOfferException.ContinueOn = true;
					}

					// JS20100621 CCP if on-cost or asset leaving australia has changed then force sales recalculation
					if (this.transIsSale && origLineItems != null)
					{
						if (quantityChange != 0.0)
						{
							this.forceSaleRecalculation = true;

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
								if (!string.IsNullOrWhiteSpace(origLineItems[lineItemIndex].UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_14]))
								{
									origOncost = double.Parse(origLineItems[lineItemIndex].UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_14]);
								}
							}
							catch (Exception)
							{
								oncostParseFailure = true;
							}

							if (oncostAmount != origOncost || lineItem.Flag04 != origLineItems[lineItemIndex].Flag04 || oncostParseFailure)
							{
								this.forceSaleRecalculation = true;
							}
						}
						else
						{
							// doesn't have original line item
							this.forceSaleRecalculation = true;
						}
					}

					// Get rates based on the configuration or based on amounts. 
					double gstRate =
						this.GetGSTRate(trans.TransactionDateTime == null ? DateTimeOffset.Now : trans.TransactionDateTime.Value);
					double exciseRate = this.GetExciseRate(
						lineItem.ProductGuid, trans.TransactionDateTime == null ? DateTimeOffset.Now : trans.TransactionDateTime.Value);
					double markUpRate = this.GetMarkupRate();

					if (!lineItem.WacCalculated && trans.TransTypeID != TransactionTypes.T18_SupplyOrder
					    && // something changed with transaction detail works, gross is sometimes 0 on postback
					    lineItem.Quantity.Net != 0.0)
					{
						double sign = lineItem.Quantity.NetInventoryChange > 0 ? 1.0 : -1.0;

						// WAC will be calculated sequentially as lineitems are ordered
						double price = 0.0;
						double latestWac = 0.0;

						if (this.trans.TransTypeID == TransactionTypes.T8_Receipt
						    || this.trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade
						    || this.trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
						{
							switch (this.trans.TransTypeID)
							{
									// CCP-043 receipts now strips certain taxes depending on the service of the fuel;
								case TransactionTypes.T8_Receipt: // TBC retrieve original transaction and see if old status was quaranteened

									// This assumes once receipt line items are changed to usable, they CANNOT be changed again.
									if ((lineItem.Quality == TransactionQuality.Usable && (quantityChange != 0.0 || trans.TransVersion == 0))
									    || (lineItem.Quality == TransactionQuality.Usable && trans.TransVersion != 0
									        && this.QualityWasNotUsable(security, trans, lineItem)))
									{
										latestWac = this.RetrieveLatestWAC(security, trans.SiteGuid, lineItem.ProductGuid);

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
											lineItem.Number06 = price;
										}

										if (quantityChange == 0)
										{
											// quantity change could be 0 when changed to usable only after a transaction has been saved
											// since it can only be changed to usable once, we use the gross as quantity change
											quantityChange = lineItem.Quantity.Net;
										}

										// wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange * sign, price, latestWac);
										wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange, price, latestWac);
										this.wacCalculatedThisRun = lineItem.WacCalculated = true;

										// when wac is calculated from a receipt / changed to complete/usable, then we should update transaction date time
										// TransactionDO dates are stored in display format
										SiteClass site = new SitesClass().GetByID(security, security.SiteID);
										this.trans.TransactionDateTime = this.trans.InventoryDate = TimeConverter.Now(site).Date;
									}

									if (lineItem.Quality != TransactionQuality.Usable && !this.wacCalculatedThisRun
									    && (origLineItems != null || trans.TransVersion == 0))
									{
										var wacs = new WeightedAverageCostsClass();
										WeightedAverageCostClass wac = wacs.GetLatest(security, trans.SiteGuid, lineItem.ProductGuid);

										lineItem.Tax4 = wac.WacValue;
									}

									break;
								case TransactionTypes.T15_PrimaryRegrade:
								case TransactionTypes.T16_SecondaryRegrade:

									if (quantityChange != 0.0 || trans.TransVersion == 0)
									{
										var li = lineItem as RegradeLineItemDO;

										if (null == li)
										{
											throw new ApplicationException("ADF Price Calculator could not process re-grade for WAC calculation.");
										}

										double toLatestWac = this.RetrieveLatestWAC(security, trans.SiteGuid, li.ToProductGuid);
										double fromLatestWac = this.RetrieveLatestWAC(security, trans.SiteGuid, li.ToProductGuid);

										if (this.isReverse(this.trans))
										{
											li.Tax5 = this.CalculateWAC(security, trans, li.ToProduct, -quantityChange, lineItem.Tax4.Value, toLatestWac);
												
												// original destination
											wacPrice = this.CalculateWAC(security, trans, li.Product, quantityChange, lineItem.Tax4.Value, fromLatestWac);
												
												// original source
										}
										else
										{
											li.Tax5 = this.CalculateWAC(security, trans, li.ToProduct, quantityChange, wacPrice, toLatestWac);
												
												// destination
											wacPrice = this.CalculateWAC(security, trans, li.Product, -quantityChange, wacPrice, fromLatestWac);
												
												// source
										}

										this.wacCalculatedThisRun = lineItem.WacCalculated = true;
									}

									break; // rule 2, regrades
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

									// case TransactionTypes.T14_PhysicalInventory:
									if ((trans.TransTypeID != TransactionTypes.T3_PrimaryDefuel
									     && trans.TransTypeID != TransactionTypes.T4_SecondaryDefuel)
									    || lineItem.Quality == TransactionQuality.Usable)
									{
										double currentWac = this.RetrieveLatestWAC(security, trans.SiteGuid, lineItem.ProductGuid);

										// check reverse here (common method)
										if (this.isReverse(this.trans) && lineItem.Tax4.Value != currentWac
										    && (quantityChange != 0.0 || trans.TransVersion == 0))
										{
											// wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange * modifier * sign, lineItem.Tax4.Value, currentWac);
											wacPrice = this.CalculateWAC(
												security, trans, lineItem.Product, quantityChange, lineItem.Tax4.Value, currentWac);
											this.wacCalculatedThisRun = lineItem.WacCalculated = true;
										}
											
											// old, no longer relevant because a transaction can be applied multiple times, but will leave this here
											// in case someone changes their mind
											// else if (0.0 != lineItem.Volume.GrossInventoryChange && quantityChange != 0.0)
										else if (0.0 != lineItem.Quantity.NetInventoryChange && quantityChange != 0.0 && trans.TransVersion != 0)
										{
											// if (wacPrice != currentWac)
											if (currentWac != lineItem.Tax4.Value)
											{
												wacPrice = this.CalculateWAC(security, trans, lineItem.Product, quantityChange * sign, wacPrice, currentWac);
												this.wacCalculatedThisRun = lineItem.WacCalculated = true;
											}
										}
									}
									else if (origLineItems != null || trans.TransVersion == 0)
									{
										// if is unusable / quarantined defuel or return, then should use latest WAC
										var wacs = new WeightedAverageCostsClass();
										WeightedAverageCostClass wac = wacs.GetLatest(security, trans.SiteGuid, lineItem.ProductGuid);

										if (wac != null)
										{
											lineItem.Tax4 = wac.WacValue;
										}
									}

									break;
							}
						}
					}

					if (lineItemHadNoWac || this.wacCalculatedThisRun)
					{
						lineItem.Tax4 = wacPrice;
					}

					// if (this.ShouldUseLatestWac(quantityChange, lineItem))
					if ((TransactionTypes.T9_Request == trans.TransTypeID) || (TransactionTypes.T18_SupplyOrder == trans.TransTypeID)
					    || // JS20100317 fuel orders will be price list price (aka standing offer) as price
					    (TransactionTypes.T22_AccountReceivableInvoice == trans.TransTypeID))
					{
						// Just return price if already set and transaction type is one of Supply Order, Payment, or Recovery.
						// Rest will be aggregated in TransactionDetails page by AggregateAssociatedTxValues
						if (this.standingOfferID != null)
						{
							lineItem.ContractNumber = this.standingOfferID;
						}

						if (unitPrice == -1.0)
						{
							// Price was not set by user. Return price list (aka standing offer) price.
							lineItem.ProductPrice = so;
						}
					}

					// JS20100208 fuel orders are unaffected by the WAC, use old method
					if (trans.TransTypeID == TransactionTypes.T17_Order || trans.TransTypeID == TransactionTypes.T18_SupplyOrder)
					{
						if (unitPrice == -1.0)
						{
							lineItem.ProductPrice = so;
						}
						else
						{
							lineItem.ProductPrice = unitPrice;
						}
					}
						
						// receipts always use fuel order price so do not set
					else if (trans.TransTypeID != TransactionTypes.T8_Receipt)
					{
						// reversals should keep original WAC as product price
						if (this.isReverse(trans) == false)
						{
							if (this.ShouldUseLatestWac(quantityChange, lineItem))
							{
								lineItem.ProductPrice = wacPrice;
							}
							else
							{
								lineItem.ProductPrice = lineItem.Tax4.Value;
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
							if (lineItem.ProductPrice != null)
							{
								productPrice = lineItem.ProductPrice.Value;
							}
						}

						// cost = lineItem.NetQuantityReceived * (unitPrice == -1.0 ? so : unitPrice);
						// if you change this, change all places marked with [ReceiptPriceQuantity]
						cost = (lineItem.AlternativeNetVolume == null ? 0.0 : lineItem.AlternativeNetVolume.Value)
						       * (productPrice == -1.0 ? so : productPrice);
					}
					else if (wacPrice > 0.0 && trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
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
					if (!this.transIsSale)
					{
						// sales are calculated later
						exciseAmount = this.CalculateExciseAmount(exciseAmount, quantity, exciseRate);
						gstAmount = this.CalculateGSTAmount(gstAmount, gstRate, exciseAmount, cost);
						markupAmount = this.CalculateMarkupAmount(markupAmount, gstAmount, exciseAmount, cost, markUpRate);
					}

					// if is reverse, then do not re-calculate taxes
					if (!(trans.ReversalType == TransactionDO.Reversal || trans.ReversalType == TransactionDO.ReversalWithUpdate))
					{
						if ((TransactionTypes.T21_AccountPayableInvoice != trans.TransTypeID) && (this.transIsSale == false))
						{
							// do not set values for invoices, its values are populated from the association

							// Excise Amount , GST Amount, and Markup Amount assgiments are defined in TR-FIN-0010
							this.AllowExciseTaxOverride(lineItem, lineItemIndex, exciseAmount);
							this.AllowGSTTaxOverride(lineItem, lineItemIndex, gstAmount);
							this.AllowMarkupTaxOverride(lineItem, lineItemIndex, markupAmount);
						}
					}

					// Handle Sales type transaction differently than other type of transactions. Sales
					// must handle fuel being sold to non-Australian defence both foreign and domestic.
					// JS20100212 Different cost of inventory also apply as per CCP-043
					if (this.transIsSale || this.forceSaleRecalculation)
					{
						/**
						 * JS20100212 As per CCP-042, transaction no longer differentiate between overseas and local,
						 * instead they simply use the source and destination site/company rates to determine proper
						 * sales cost. However revenue is still identical in that it is cost ex GST inc margin.
						 **/
						double baseCost = this.CalculateSaleRevenue(
							lineItem, lineItem.ProductPrice.Value, 0, 0, 0, Math.Abs(oncostAmount));
						revenue = this.CalculateSaleRevenue(
							lineItem, lineItem.ProductPrice.Value, exciseRate, 0, markUpRate, Math.Abs(oncostAmount));
						totalCost = this.CalculateSaleRevenue(
							lineItem, lineItem.ProductPrice.Value, exciseRate, gstRate, markUpRate, Math.Abs(oncostAmount));

						// re-calculate GST rate
						gstAmount = totalCost - revenue;

						// re-calculate Excise rate
						exciseAmount = lineItem.Quantity.Gross * exciseRate;

						// re-calculate Markup rate
						markupAmount = this.CalculateSaleRevenue(
							lineItem, lineItem.ProductPrice.Value, 0, 0, markUpRate - 1.0, Math.Abs(oncostAmount));

						if (!this.forceSaleRecalculation)
						{
							this.AllowGSTTaxOverride(lineItem, lineItemIndex, gstAmount);
							this.AllowExciseTaxOverride(lineItem, lineItemIndex, exciseAmount);
							this.AllowMarkupTaxOverride(lineItem, lineItemIndex, markupAmount);
						}

						// WI-15139 
						if (lineItem.Tax1 == null || lineItem.Tax1.Value < 0 || this.forceSaleRecalculation)
						{
							lineItem.Tax1 = exciseAmount;
						}

						if (lineItem.Tax2 == null || lineItem.Tax2.Value < 0 || this.forceSaleRecalculation)
						{
							lineItem.Tax2 = gstAmount;
						}

						if (lineItem.Tax3 == null || lineItem.Tax3.Value < 0 || this.forceSaleRecalculation)
						{
							lineItem.Tax3 = markupAmount;
						}

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

						if (trans.ReversalType == TransactionDO.Reversal || trans.ReversalType == TransactionDO.ReversalWithUpdate)
						{
							if (lineItem.Tax1 != null)
							{
								lineItem.Tax1 *= -1;
							}

							if (lineItem.Tax2 != null)
							{
								lineItem.Tax2 *= -1;
							}

							if (lineItem.Tax3 != null)
							{
								lineItem.Tax3 *= -1;
							}

							revenue *= -1;
							totalCost *= -1;
						}

						// actual fuel price
						if (lineItem.Number01 == null)
						{
							lineItem.Number01 = lineItem.ProductPrice.Value;
						}

						// actual excise
						if (lineItem.Number02 == null)
						{
							lineItem.Number02 = lineItem.Tax1.Value;
						}

						// actual gst
						if (lineItem.Number03 == null)
						{
							lineItem.Number03 = lineItem.Tax2.Value;
						}

						// actual margin
						if (lineItem.Number04 == null)
						{
							lineItem.Number04 = lineItem.Tax3.Value;
						}

						// actual cost excl GST
						if (lineItem.Number05 == null)
						{
							lineItem.Number05 = revenue;
						}

						// actual cost incl GST
						if (lineItem.Number06 == null)
						{
							lineItem.Number06 = totalCost;
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
							lineItem.ContractNumber = this.standingOfferID;
						}
					}
				}

#if DEBUG
				this.watch.Stop();

				string entry = string.Format(
					Resource1.ResourceManager.GetString(string.Empty), this.watch.ElapsedTime.Milliseconds, trans.Alias, trans.TransID);

				this.watch.Perform(entry);

#endif

				// DEBUG
			}
			catch (Exception e)
			{
				FMChannelHelper.MakeCall<IFMEventLog>((x) => { x.WriteEntry(e.Message, FMEventLogEntryType.Error); });

				throw;
			}

			// price list (aka standing offer) exception occurs if the price list (aka standing offer) is not found
			// or the a more recent one is found.
			if (standingOfferException != null)
			{
				throw standingOfferException;
			}

			return true;
		}

		#endregion

		#region Methods

		protected double CalculateSaleRevenue(
			LineItemDO lineItem, double wacPrice, double exciseRate, double gstRate, double markUpRate, double oncostAmount)
		{
			double result = 0.0;

			// need to work out seller excise
			double qty = lineItem.Quantity.Gross;

			double sellerExciseRate = this.GetExciseRate(
				lineItem.ProductGuid, 
				this.trans.TransactionDateTime == null ? DateTimeOffset.Now : this.trans.TransactionDateTime.Value, 
				this.trans.Site);

			// equation pulled from CCP-043 - ((QTY x (WAC - Seller Excise) + On-Cost) x (1 + Margin) + (QTY x Customer Excise)) x (1 + GST)
			result = ((qty * (wacPrice - sellerExciseRate) + oncostAmount) * (1 + markUpRate) + (qty * exciseRate))
			         * (1 + gstRate);

			return result;
		}

		protected double GetCompanyMarkupRate(string a_companyName)
		{
			double markupRate = 0.0;

			MarkupDOCollection collection = this.markupBL.GetAll(this.security);

			bool found = false;

			DateTimeOffset curMarkupDateTime = DateTimeOffset.MinValue;

			foreach (MarkupDO markup in collection)
			{
				List<TaxCompanyMapDO> companyMap = this.markupBL.GetMarkupCompanies(markup, this.security);

				found = this.IsAssignedToCompany(companyMap, a_companyName);
				if (found)
				{
					markupRate = markup.MarkupRate;
					break;
				}
			}

			if (!found)
			{
				this.ThrowNotConfigured(a_companyName + " Margin");
			}

			return markupRate; // placeholder
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
			var productMaps = new ProductMapsClass();
			ProductMapCollectionClass mapCol = productMaps.EnumerateByType(this.security, PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP);

			for (int i = 0; i < mapCol.Count; ++i)
			{
				ProductMapClass map = mapCol[i];
				if (map.AssignedToID.ToUpper().Equals(a_match.ToUpper())
				    && map.AssignedID.ToUpper().Equals(a_lineItem.Product.ToUpper()))
				{
					result = true;
					break;
				}
			}

			return result;
		}

		protected void InitServiceTable(ref Hashtable serviceTable)
		{
			serviceTable = new Hashtable();

			foreach (ServiceType service in Enum.GetValues(typeof(ServiceType)))
			{
				serviceTable.Add(service, service.ToString());
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

		protected bool QualityWasNotUsable(SecurityClass a_security, TransactionDO a_trans, LineItemDO a_lineItem)
		{
			bool returnVal = false;

			// JS20100716 Performance, on most transactions this is not needed, only transactions which has a usable function
			// which impact the WAC.
			if (this.trans.TransTypeID == TransactionTypes.T8_Receipt
			    || this.trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade
			    || this.trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade
			    || this.trans.TransTypeID == TransactionTypes.T3_PrimaryDefuel
			    || this.trans.TransTypeID == TransactionTypes.T4_SecondaryDefuel)
			{
				var sr = new TransactionSR();
				sr.Security = a_security;
				sr.TransID = a_trans.TransID;

				var txProcessor = new TransactionProcessorClass();
				TransactionDO orig = txProcessor.Process(sr);

				if (orig != null)
				{
					// find the line item
					foreach (LineItemDO li in orig.LineItems)
					{
						if (li.TransactionLineItemGuid == a_lineItem.TransactionLineItemGuid && li.Quality != TransactionQuality.Usable)
						{
							returnVal = true;
							break;
						}
					}
				}
			}

			return returnVal;
		}

		protected void SetForeignCurrencyValues(ref LineItemDO a_lineItem, string a_transID, Guid a_transLineItemGuid)
		{
			LineItemDO foundLineItem = null;

			try
			{
				var transSr = new TransactionSR();
				transSr.Security = this.security;
				transSr.TransID = a_transID;

				TransactionDO parentTrans = this.txProcessor.Process(transSr);

				if (parentTrans != null)
				{
					foreach (LineItemDO li in parentTrans.LineItems)
					{
						if (li.TransactionLineItemGuid == a_transLineItemGuid)
						{
							foundLineItem = li;
							break;
						}
					}
				}
			}
			catch (Exception)
			{
			}

			if (foundLineItem != null)
			{
				a_lineItem.CurrencyGuid = foundLineItem.CurrencyGuid;
				a_lineItem.NonDomesticPrice = foundLineItem.NonDomesticPrice;
			}
		}

		protected bool ShouldUseLatestWac(double quantityChange, LineItemDO lineItem)
		{
			bool returnVal;

			if (this.trans.ReversalType == TransactionDO.Update && this.origTrans == null)
			{
				// new updates are different because they use original WAC price
				returnVal = false;
			}
			else
			{
				returnVal = (quantityChange != 0.0 && lineItem.TransactionLineItemGuid != Guid.Empty)
				            || ((lineItem.Quality == TransactionQuality.Usable
				                 && this.QualityWasNotUsable(this.security, this.trans, lineItem))
				                && (this.trans.Alias.ToUpper().Equals("DEFUEL") || this.trans.Alias.ToUpper().Equals("RETURN")));
			}

			return returnVal;
		}

		protected double StripTaxes(LineItemDO a_lineItem, double a_price, double a_gstRate, double a_exciseRate)
		{
			double newPrice = a_price;

			if (this.InProductGroup(a_lineItem, this.serviceTable[ServiceType.AVIATION].ToString())
			    || this.InProductGroup(a_lineItem, this.serviceTable[ServiceType.GROUND].ToString()))
			{
				// aviation & ground fuel strips gst only
				newPrice = a_price / (1 + a_gstRate);
			}
			else if (this.InProductGroup(a_lineItem, this.serviceTable[ServiceType.MARINE].ToString()))
			{
				// marine fuel strips gst and excise
				// newPrice = (a_price / (1 + a_gstRate)) / (1 + a_exciseRate);
				newPrice = (a_price / (1 + a_gstRate)) - a_exciseRate;

				// newPrice = newPrice * a_gstRate;
				// newPrice = newPrice * a_exciseRate;
			}

			// newPrice = (a_price / (1 + a_gstRate)) - 1 + a_exciseRate; -- leave this here in case someone changes their mind
			return newPrice;
		}

		protected void ThrowNotConfigured(string a_source)
		{
			string errorMsg = a_source + " " + NOT_CONFIGURED;

			throw new Exception(errorMsg);
		}

		/// <summary>
		/// This method will allow excise tax override for sales transactions.
		/// </summary>
		/// <param name="lineItem">
		/// </param>
		/// <param name="lineItemIndex">
		/// </param>
		/// <param name="exciseAmount">
		/// </param>
		private void AllowExciseTaxOverride(LineItemDO lineItem, int lineItemIndex, double exciseAmount)
		{
			if (this.transIsSale)
			{
				if ((this.origLineItems != null) && (this.origLineItems.Count > 0))
				{
					if (lineItemIndex >= this.origLineItems.Count)
					{
						lineItem.Tax1 = exciseAmount;
					}
					else
					{
						LineItemDO origLineItem = this.origLineItems[lineItemIndex];

						if ((lineItem.Tax1 != null) && string.IsNullOrEmpty(lineItem.Tax1.Value.ToString()))
						{
							lineItem.Tax1 = exciseAmount;
						}
					}
				}
			}
			else
			{
				lineItem.Tax1 = exciseAmount;
			}
		}

		/// <summary>
		/// This method will allow GST tax override for sales transactions.
		/// </summary>
		/// <param name="lineItem">
		/// </param>
		/// <param name="lineItemIndex">
		/// </param>
		/// <param name="gstAmount">
		/// </param>
		private void AllowGSTTaxOverride(LineItemDO lineItem, int lineItemIndex, double gstAmount)
		{
			if (this.transIsSale)
			{
				if ((this.origLineItems != null) && (this.origLineItems.Count > 0))
				{
					if (lineItemIndex >= this.origLineItems.Count)
					{
						lineItem.Tax2 = gstAmount;
					}
					else
					{
						LineItemDO origLineItem = this.origLineItems[lineItemIndex];

						if ((lineItem.Tax2 != null) && string.IsNullOrEmpty(lineItem.Tax2.Value.ToString()))
						{
							lineItem.Tax2 = gstAmount;
						}
					}
				}
			}
			else
			{
				lineItem.Tax2 = gstAmount;
			}
		}

		/// <summary>
		/// This method will allow Markup tax override for sales transactions.
		/// </summary>
		/// <param name="lineItem">
		/// </param>
		/// <param name="lineItemIndex">
		/// </param>
		/// <param name="markupAmount">
		/// </param>
		private void AllowMarkupTaxOverride(LineItemDO lineItem, int lineItemIndex, double markupAmount)
		{
			if (this.transIsSale)
			{
				if ((this.origLineItems != null) && (this.origLineItems.Count > 0))
				{
					if (lineItemIndex >= this.origLineItems.Count)
					{
						lineItem.Tax3 = markupAmount;
					}
					else
					{
						LineItemDO origLineItem = this.origLineItems[lineItemIndex];

						if ((lineItem.Tax3 != null) && string.IsNullOrEmpty(lineItem.Tax3.Value.ToString()))
						{
							lineItem.Tax3 = markupAmount;
						}
					}
				}
			}
			else
			{
				lineItem.Tax3 = markupAmount;
			}
		}

		/// <summary>
		/// This method will return excise amount based on the excise rate. It will return the same value
		///     if the excise greater than zero.
		/// </summary>
		/// <param name="exciseAmount">
		/// The excise Amount.
		/// </param>
		/// <param name="quantity">
		/// The quantity.
		/// </param>
		/// <param name="exciseRate">
		/// The excise Rate.
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double CalculateExciseAmount(double exciseAmount, double quantity, double exciseRate)
		{
			if ((exciseAmount < 0.0) || this.recalculateTaxes)
			{
				exciseAmount = quantity * exciseRate;
			}

			return exciseAmount;
		}

		/// <summary>
		/// This method will return GST amount based on the GST rate. It will return the same value
		///     if the GST greater than zero.
		/// </summary>
		/// <param name="gstAmt">
		/// The gst Amt.
		/// </param>
		/// <param name="gstRate">
		/// </param>
		/// <param name="exciseAmt">
		/// The excise Amt.
		/// </param>
		/// <param name="cost">
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double CalculateGSTAmount(double gstAmt, double gstRate, double exciseAmt, double cost)
		{
			if ((gstAmt < 0.0) || this.recalculateTaxes)
			{
				if (this.trans.TransTypeID == TransactionTypes.T8_Receipt)
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
		/// This method will return the markup amount based on markup rate when leaving Australia and
		///     zero if consumed in Australia.
		/// </summary>
		/// <param name="markupAmount">
		/// The markup Amount.
		/// </param>
		/// <param name="gstAmount">
		/// </param>
		/// <param name="exciseAmount">
		/// </param>
		/// <param name="cost">
		/// The cost.
		/// </param>
		/// <param name="markupRate">
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double CalculateMarkupAmount(
			double markupAmount, double gstAmount, double exciseAmount, double cost, double markupRate)
		{
			if ((markupAmount < 0.0) || this.recalculateTaxes)
			{
				if (this.transIsSale)
				{
					if (this.leavingAustralia)
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

		private double CalculateWAC(
			SecurityClass a_security, TransactionDO a_trans, string a_product, double a_delta, double a_price, double a_valueWac)
		{
			double origQty = this.RetrieveBookInventory(a_security, a_trans, a_product, DateTimeOffset.Now);

			// JS20100621 [SKAFTSV001]: for reverse and reverse updates, WAC is calculated after transactions are SAVED,
			// this means the original quantity is the inventory minus the one on the current transaction. this is
			// generic, on reverse will add the delta, on update will subtract the delta.
			if (a_trans.ReversalType.Equals(TransactionDO.ReversalWithUpdate)
			    || a_trans.ReversalType.Equals(TransactionDO.Update))
			{
				double modifier = 0.0;

				if (!string.IsNullOrWhiteSpace(a_trans.LineItems[0].UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_24]))
				{
					string data24 = a_trans.LineItems[0].UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_24];

					try
					{
						modifier = Convert.ToDouble(data24);
					}
					catch (Exception)
					{
						modifier = 0.0;
					}

					if (a_trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade
					    || a_trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
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
		///     user. Otherwise, it return false.
		/// </summary>
		/// <param name="lineItem">
		/// </param>
		/// <param name="lineItemIndex">
		/// </param>
		private void DidPriceChange(LineItemDO lineItem, int lineItemIndex)
		{
			this.recalculateTaxes = false;

			if ((this.origLineItems != null) && (lineItemIndex < this.origLineItems.Count))
			{
				LineItemDO origLineItem = this.origLineItems[lineItemIndex];

				if (origLineItem != null)
				{
					if ((lineItem.ProductPrice != null) && (lineItem.ProductPrice != null) && (origLineItem.ProductPrice != null)
					    && (origLineItem.ProductPrice != null))
					{
						// Since there were doubles the price may never be exactly the same, therefore
						// only check to the 4th decimal position.
						double prodPrice1 = Math.Round(lineItem.ProductPrice.Value, CurrencyPercision, MidpointRounding.AwayFromZero);
						double prodPrice2 = Math.Round(origLineItem.ProductPrice.Value, CurrencyPercision, MidpointRounding.AwayFromZero);

						if (prodPrice1 != prodPrice2)
						{
							this.recalculateTaxes = true;
						}
					}

					if ((lineItem.Quantity != null) && (origLineItem.Quantity != null))
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

		private ArrayList FilterLineItems(ArrayList a_lineItems, Guid a_productGuid)
		{
			var resultList = new ArrayList();

			if (null == a_lineItems)
			{
				// failsafe
				return resultList;
			}

			foreach (LineItemDO lineItem in a_lineItems)
			{
				if (a_productGuid == lineItem.ProductGuid)
				{
					resultList.Add(lineItem);
				}
			}

			return resultList;
		}

		/// <summary>
		/// This method will return either a AUP price if one in found or a
		///     price list (aka standing offer) price.
		/// </summary>
		/// <param name="lineItem">
		/// </param>
		/// <param name="supplierCompanyGuid">
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double GetAUPPrice(LineItemDO lineItem, Guid supplierCompanyGuid)
		{
			double price = 0.0;

			var financeSR = new FinanceSR();
			financeSR.SiteGuid = this.security.SiteGuid;
			financeSR.Site = this.security.SiteID;
			financeSR.StartDate = this.trans.InventoryDate;
			financeSR.ProductGuid = lineItem.ProductGuid;
			financeSR.SupplierCompanyGuid = supplierCompanyGuid;
			financeSR.Security = this.security;
			financeSR.DeliveryLocation = lineItem.DeliveryLocation;
			financeSR.SubRequest = FinanceSR.SUB_REQUEST.AVERAGE_UNIT_PRICE;

			var financeProcessor = new FinanceProcessorClass();
			FinanceDO financeDO = financeProcessor.Process(financeSR);

			if (financeDO.ContainsAverageUnitPrice)
			{
				price = financeDO.AverageGrossUnitPrice;
			}
			else
			{
				price = this.GetStandingOfferPrice(lineItem, supplierCompanyGuid);
			}

			return price;
		}

		/// <summary>
		/// This method will retrieve the markup rate associated to the ship-to company.
		/// </summary>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double GetAssociatedMarkupRate()
		{
			// CompanyGroupsClass CompanyGroups = new CompanyGroupsClass();
			// CompanyGroupCollectionClass CompanyGroupColl = CompanyGroups.Enumerate(this.security);
			double markUpRate = 0.0;

			if (this.trans.ShipToCompanyGuid != Guid.Empty)
			{
				markUpRate = this.GetCompanyMarkupRate(this.trans.ShipToID);
			}

			return markUpRate;
		}

		/// <summary>
		/// This method will return the Excise configurated in the system. It will
		///     return zero if not found.
		/// </summary>
		/// <param name="prodGuid">
		/// </param>
		/// <param name="transactionDateTime">
		/// </param>
		/// <param name="sourceCompany">
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double GetExciseRate(Guid prodGuid, DateTimeOffset transactionDateTime, string sourceCompany)
		{
			double exciseRate = 0.0;

			if (!string.IsNullOrEmpty(sourceCompany) // has a bill to
			    && (this.trans.TransTypeID != TransactionTypes.T21_AccountPayableInvoice) // invoice
			    && (this.trans.TransTypeID != TransactionTypes.T22_AccountReceivableInvoice) // recovery
			    && (this.trans.TransTypeID != TransactionTypes.T9_Request) // demand
				)
			{
				ExciseTaxDOCollection col = this.exciseTaxBL.GetAll(this.security);

				ExciseTaxDO exciseDO = null;
				DateTimeOffset curExciseDateTime = DateTimeOffset.MinValue;

				foreach (ExciseTaxDO curExciseDO in col)
				{
					if (curExciseDO != null)
					{
						List<TaxCompanyMapDO> companyMap = this.exciseTaxBL.GetExciseCompanies(curExciseDO, this.security);
						if (this.IsAssignedToCompany(companyMap, sourceCompany) && // assigned to bill-to
						    curExciseDO.ExciseDate > curExciseDateTime && // only find most recent
						    curExciseDO.ExciseDate <= transactionDateTime && // not in the future
						    curExciseDO.ProductGuid == prodGuid // matching product
							)
						{
							exciseDO = curExciseDO;
							curExciseDateTime = exciseDO.ExciseDate;
						}
					}
				}

				if (exciseDO == null)
				{
					this.ThrowNotConfigured(sourceCompany + " Excise");
				}

				exciseRate = exciseDO.ExciseRate; // / 100.0;
			}

			return exciseRate;
		}

		/// <summary>
		/// This method will return the excise rate based on the change in the Excise Amount or
		///     based on the Excise rate retrieved from the configuration.
		/// </summary>
		/// <param name="prodGuid">
		/// </param>
		/// <param name="transactionDateTime">
		/// The transaction Date Time.
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double GetExciseRate(Guid prodGuid, DateTimeOffset transactionDateTime)
		{
			string sourceCompany = string.Empty;

			if (this.transIsSale)
			{
				sourceCompany = this.trans.BillToID == null ? string.Empty : this.trans.BillToID;
			}
			else if (this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder
			         || this.trans.TransTypeID == TransactionTypes.T8_Receipt)
			{
				sourceCompany = this.trans.SupplierID == null ? string.Empty : this.trans.SupplierID;
			}
			else
			{
				return 0.0; // all other transactions do not have taxes
			}

			return this.GetExciseRate(prodGuid, transactionDateTime, sourceCompany);
		}

		/// <summary>
		/// This method will return the GST configurated in the system. It will
		///     return zero if not found.
		/// </summary>
		/// <param name="transactionDateTime">
		/// The transaction Date Time.
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double GetGSTRate(DateTimeOffset transactionDateTime)
		{
			double gstRate = 0.0;

			if (this.leavingAustralia)
			{
				return 0.0;
			}

			string sourceCompany = string.Empty;
			if (this.transIsSale)
			{
				sourceCompany = this.trans.BillToID == null ? string.Empty : this.trans.BillToID;
			}
			else if (this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder
			         || this.trans.TransTypeID == TransactionTypes.T8_Receipt)
			{
				sourceCompany = this.trans.SupplierID == null ? string.Empty : this.trans.SupplierID;
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
				GoodsAndServicesTaxDOCollection col = this.gstTaxBL.GetAll(this.security);

				GoodsAndServicesTaxDO gstDO = null;
				DateTimeOffset curGstDateTime = DateTimeOffset.MinValue;

				foreach (GoodsAndServicesTaxDO curGstDO in col)
				{
					if (curGstDO != null)
					{
						List<TaxCompanyMapDO> companyMap = this.gstTaxBL.GetGSTCompanies(curGstDO, this.security);
						if (this.IsAssignedToCompany(companyMap, sourceCompany) && // assigned to bill-to
						    curGstDO.GstDate > curGstDateTime && // only find most recent
						    curGstDO.GstDate <= transactionDateTime // not in the future
							)
						{
							gstDO = curGstDO;
							curGstDateTime = gstDO.GstDate;
						}
					}
				}

				if (gstDO == null)
				{
					this.ThrowNotConfigured(sourceCompany + " GST");
				}

				gstRate = gstDO.GstValue / 100.0;
			}

			return gstRate;
		}

		/// <summary>
		/// This method will return the markup rate based on the change in the markup Amount or
		///     based on the markup rate retrieved from the configuration.
		/// </summary>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double GetMarkupRate()
		{
			double markUpRate = 0.0;

			if (this.transIsSale)
			{
				var companiesBL = new CompaniesClass();
				Guid companyGuid = (this.trans.BillToID == null || this.trans.BillToCompanyGuid == Guid.Empty)
					                   ? Guid.Empty
					                   : this.trans.BillToCompanyGuid;
				CompanyClass billTo = companiesBL.Get(this.security, companyGuid);

				if (billTo != null)
				{
					markUpRate = this.GetCompanyMarkupRate(billTo.ID);
					markUpRate /= 100.0;
				}
			}

			return markUpRate;
		}

		/// <summary>
		/// This method will calculate the new SO based on the changes to Excise and GST amounts.
		/// </summary>
		/// <param name="quantity">
		/// </param>
		/// <param name="exciseAmt">
		/// </param>
		/// <param name="exciseRate">
		/// </param>
		/// <param name="gstAmt">
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
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
		/// This method will return the price list (aka standing offer) price for all transaction except the
		///     physical inventory transactions. For the physical inventory transaction, the
		///     average unit price is calculated from the receipts.
		/// </summary>
		/// <param name="lineItem">
		/// The line Item.
		/// </param>
		/// <param name="supplierCompanyGuid">
		/// The supplier Company Guid.
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double GetStandingOfferPrice(LineItemDO lineItem, Guid supplierCompanyGuid)
		{
			double price = 0.0;

			var financeSR = new FinanceSR();
			financeSR.SiteGuid = this.security.SiteGuid;
			financeSR.Site = this.security.SiteID;
			financeSR.StartDate = this.trans.InventoryDate;
			financeSR.ProductGuid = lineItem.ProductGuid;
			financeSR.SupplierCompanyGuid = supplierCompanyGuid;
			financeSR.Security = this.security;
			financeSR.DeliveryLocation = lineItem.DeliveryLocation;
			financeSR.SubRequest = FinanceSR.SUB_REQUEST.STANDING_OFFER_PRICE;
			financeSR.Quantity = lineItem.Quantity.Gross;

			if (this.trans.Alias.ToUpper().Contains("FUEL ORDER") && (lineItem.AssociatedTransactions != null)
			    && (lineItem.AssociatedTransactions.Count > 0))
			{
				var associatedTxDO = lineItem.AssociatedTransactions[0] as AssociatedTxDO;
				if (associatedTxDO != null)
				{
					financeSR.DeliveryLocation = associatedTxDO.DeliveryLocation;
				}
			}

			var financeProcessor = new FinanceProcessorClass();
			this.financeDO = financeProcessor.Process(financeSR);

			if (this.financeDO != null)
			{
				price = this.financeDO.CurrentStandingOfferPrice;
			}

			return price;
		}

		private double RetrieveBookInventory(
			SecurityClass a_security, TransactionDO a_trans, string a_product, DateTimeOffset a_dateTime)
		{
			double returnVal = 0.0;

			LedgerDO ledgerDO = null;

			var ledgerSR = new LedgerSR();
			ledgerSR.Security = a_security;
			ledgerSR.Site = a_security.SiteID;
			ledgerSR.CurrentSiteGuid = a_security.SiteGuid;
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
			ledgerSR.Month = DateEfficacy.ConvertToMonthAndYear(a_dateTime);
			ledgerSR.SetRequestType(LedgerSR.LedgerRequests.Refresh);

			try
			{
				// Get the ledger data
				ledgerDO = this.ledgerProcessor.Process(ledgerSR);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}

			if (ledgerDO != null)
			{
				// use the book inventory for the current day (not the EOM because we do NOT want to take in account of today's physical inventory)
				var curLedgerLineItem = ledgerDO.LedgerLineItems[a_dateTime.Day - 1] as LedgerLineItemDO;

				returnVal = curLedgerLineItem.BookInventory.NetInventoryChange;
			}

			return returnVal;
		}

		private double RetrieveLatestWAC(SecurityClass a_security, Guid a_siteGuid, Guid a_productGuid)
		{
			double returnVal = 0.0;

			// need to use the consolidated data object -> weighted average cost class here
			var wacCollection = new WeightedAverageCostsClass();

			WeightedAverageCostClass wac = wacCollection.GetLatest(a_security, a_siteGuid, a_productGuid);

			if (wac != null)
			{
				returnVal = wac.WacValue;
			}

			return returnVal; // placeholder
		}

		private ArrayList RetrieveUniqueProductIndex(ArrayList a_lineItems)
		{
			var resultList = new ArrayList();
			var uniqueTable = new Hashtable();

			foreach (LineItemDO lineItem in a_lineItems)
			{
				uniqueTable[lineItem.ProductGuid] = new Object();
			}

			resultList.AddRange(uniqueTable.Keys);

			return resultList;
		}

		/// <summary>
		/// This method will set the default values for excise, GST, markup, unit price and the line item
		///     values. It will return the either excise, gst, markup, or unit price.
		/// </summary>
		/// <param name="lineItem">
		/// </param>
		/// <param name="type">
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double SetLineItemValues(LineItemDO lineItem, LineItemTypes type)
		{
			double returnValue = -1.0;

			switch (type)
			{
				case LineItemTypes.EXCISE:
					if (lineItem.Tax1 != null)
					{
						returnValue = lineItem.Tax1.Value;
					}

					break;
				case LineItemTypes.GST:
					if (lineItem.Tax2 != null)
					{
						returnValue = lineItem.Tax2.Value;
					}

					break;
				case LineItemTypes.MARKUP:
					if (lineItem.Tax3 != null)
					{
						returnValue = lineItem.Tax3.Value;
					}

					break;
				case LineItemTypes.PRODUCT_PRICE:

					// for receipts, product price is pulled from the fuel order
					if ((this.trans.TransTypeID == TransactionTypes.T8_Receipt) && lineItem.AssociatedTransactions.Count > 0)
					{
						var atx = lineItem.AssociatedTransactions[0] as AssociatedTxDO;

						// find the fuel order linked
						var sr = new AssociatedTxSR();
						sr.Security = this.security;
						sr.TransactionLineItemGuid = atx.TransactionLineItemGuid;
						sr.TransID = atx.TransID;
						sr.RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions;

						AssociatedTxListDO result = this.associatedTxProcessor.Process(sr);

						foreach (DataRow dr in result.AssociatedTransactions.Tables[0].Rows)
						{
							var demandParentTrans = new AssociatedTxDO();
							demandParentTrans.Load(dr);

							if (demandParentTrans.TransTypeID == TransactionTypes.T18_SupplyOrder
							    && demandParentTrans.LinkedTransactionLineItemGuid == atx.TransactionLineItemGuid
							    && demandParentTrans.Product.ToUpper().Equals(atx.Product.ToUpper()))
							{
								lineItem.ProductPrice = demandParentTrans.ProductPrice;

								// also should pull in the fuel order forex stuff if any
								this.SetForeignCurrencyValues(
									ref lineItem, demandParentTrans.TransID, demandParentTrans.TransactionLineItemGuid);
							}
						}
					}

					if ((lineItem.ProductPrice != null) && (lineItem.ProductPrice != null) && (lineItem.ProductPrice.Value > 0.0))
					{
						returnValue = lineItem.ProductPrice.Value;
					}

					if (lineItem.ProductPrice == null)
					{
						lineItem.ProductPrice = 0.0;
					}

					break;
				case LineItemTypes.WAC:
					if (lineItem.Tax4 == null)
					{
						lineItem.Tax4 = 0.0;
					}
					else
					{
						returnValue = lineItem.Tax4.Value;
					}

					break;
				case LineItemTypes.ONCOST:
					if (!lineItem.UserData.ContainsKey(TransactionDO.USER_DATA_LINE_ITEM_KEY_14)
						|| string.IsNullOrWhiteSpace(lineItem.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_14]))
					{
						lineItem.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_14] = "0.0";
						returnValue = 0.0;
					}
					else
					{
						string udValue = lineItem.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_14];
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
				case LineItemTypes.QUANTITY:
					returnValue = lineItem.Quantity.Gross;
					break;
			}

			return returnValue;
		}

		private bool isReverse(TransactionDO a_trans)
		{
			return a_trans.ReversalType == TransactionDO.Reversal || a_trans.ReversalType == TransactionDO.Update
			       || a_trans.ReversalType == TransactionDO.ReversalWithUpdate;
		}

		#endregion
	}
}