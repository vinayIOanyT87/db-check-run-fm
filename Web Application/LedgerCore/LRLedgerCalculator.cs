namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;

    // ReSharper disable once InconsistentNaming
	public class LRLedgerCalculator
	{
		#region attributes
		protected enum WhichDailyWac { CurrentDay, PreviousDay };

		private readonly bool usePreviousPhysicalInventory;
		private readonly LRTransactionAliasListDO aliasListDO;
		private readonly LRCustomMathFunctionDO customMathFunctionDO;
		private readonly LRCustomMovementFunctionDO customMovementFunctionDO;
		private readonly LRLedgerProcessor.SystemEditions systemEdition;
		private readonly LedgerConnection ledgerConnection;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Ledger Calculator class.
		/// </summary>
		/// <param name="aliasListDO"></param>
		/// <param name="usePreviousPhysicalInventory"></param>
		/// <param name="systemEdition"></param>
		/// <param name="ledgerConn"></param>
		public LRLedgerCalculator(	LRTransactionAliasListDO aliasListDO, 
									bool usePreviousPhysicalInventory, 
									LRLedgerProcessor.SystemEditions systemEdition,
									LedgerConnection ledgerConn)
		{
			this.aliasListDO					= aliasListDO;
			this.usePreviousPhysicalInventory	= usePreviousPhysicalInventory;
			this.customMathFunctionDO			= new LRCustomMathFunctionDO();
			this.customMovementFunctionDO		= new LRCustomMovementFunctionDO();
			this.systemEdition					= systemEdition;
			this.ledgerConnection				= ledgerConn;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will orchestrate the calculation for begin, book, total physical, variance,
		/// and total variance inventories.
		/// </summary>
		/// <param name="siteGuid"></param>
		/// <param name="ledger"></param>
		/// <param name="initialBeginInventory"></param>
		public void CalculateLedger(Guid siteGuid, LRLedgerLineItemCollection ledger, LRQuantityDO initialBeginInventory)
		{
			LRInventoryLineItemDO previousLineItem = null;

			// Determine if we need to support a custom Total Movement function in the database
			var totalMovementSp = this.GetCustomMovementCalculationSpName();

			// The the product information which includes the conversion factor and
			// precision. If the product is configured with the conversion factor, then
			// class factor and precision members will be set use the product settings.
			// Otherwise the site's settings will be used.
            LRProductDO productDO = this.RetrieveProductInfo(ledger.ProductGuid, ledger.SiteGuid);

			foreach (LRInventoryLineItemDO lineItem in ledger)
			{
				double previousDailyWacValue = 0.0;
				double dailyWacValue = 0.0;

				if (this.systemEdition == LRLedgerProcessor.SystemEditions.Adf)
				{
					// Get the most current WAC value for the previous inventory date, site, and product combination.
					previousDailyWacValue = this.GetCurrentWac(
						lineItem.InventoryDate,
						ledger.SiteGuid,
						ledger.ProductGuid,
						WhichDailyWac.PreviousDay);

					// Get the most current WAC value for the inventory date, site, and product combination.
					dailyWacValue = this.GetCurrentWac(
						lineItem.InventoryDate,
						ledger.SiteGuid,
						ledger.ProductGuid,
						WhichDailyWac.CurrentDay);
				}

				this.CalculateBeginInventory(lineItem, previousLineItem, initialBeginInventory, previousDailyWacValue);
				this.CalculateBookInventory(lineItem, dailyWacValue);
				this.CalculateTotalPhysicalInventory(lineItem, dailyWacValue);
				this.CalculateVariance(lineItem, dailyWacValue);
				this.CalculateTotalVariance(lineItem, previousLineItem, dailyWacValue);

				if (string.IsNullOrEmpty(totalMovementSp) || this.systemEdition != LRLedgerProcessor.SystemEditions.Mod)
				{
					this.CalculateCoreTotalMovement(lineItem, previousLineItem, productDO);
				}
				else
				{
					this.CalculateCustomTotalMovement(	lineItem, 
														previousLineItem, 
														ledger.ProductGuid, 
														ledger.TankGuid, 
														siteGuid, 
														totalMovementSp );
				}

				this.CalculateToleranceTestedVolume(lineItem, initialBeginInventory, productDO);
				this.CalculateAggregateColumns(lineItem);
				this.CalculateTolerance(lineItem, initialBeginInventory, productDO);
				this.CalculateVariancePercentage(lineItem);
				this.CalculateAllowableGainLoss(lineItem);
				this.CalculateOutOfToleranceFlags(lineItem);

				previousLineItem = lineItem;
			}
		}
		#endregion

		#region protected Methods
		/// <summary>
		/// This method calculates the beginning book inventory. For a multi-owner system, the
		/// beginning book inventory is always the previous day's ending book inventory. If the
		/// system is a single owner system, then the beginning book is either the physical
		/// inventory of the previous day if it exists or the ending book of the previous day.
		/// This is the same for pricing.
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="previousLineItem"></param>
		/// <param name="initialBeginInventory"></param>
		/// <param name="dailyWacValue"></param>
		protected void CalculateBeginInventory(	LRInventoryLineItemDO lineItem,
												LRInventoryLineItemDO previousLineItem,
												LRQuantityDO initialBeginInventory,
												double dailyWacValue)
		{
			// If calculating the first item for a Single Owner system, it comes from a Closeout Record or 
			// a Previous Physical Inventory.
			if (previousLineItem == null)
			{
				LRQuantityDO beginInventory = lineItem.BeginInventory;
				beginInventory.GrossInventoryChange = initialBeginInventory.GrossInventoryChange;
				beginInventory.NetInventoryChange = initialBeginInventory.NetInventoryChange;
				beginInventory.MassInventoryChange = initialBeginInventory.MassInventoryChange;
				beginInventory.PackageInventoryChange = initialBeginInventory.PackageInventoryChange;

				// Changed to calculate price using average unit price
				beginInventory.GrossPriceInventoryChange = initialBeginInventory.GrossInventoryChange * dailyWacValue;
				beginInventory.NetPriceInventoryChange = initialBeginInventory.NetInventoryChange * dailyWacValue;
				beginInventory.MassPriceInventoryChange = initialBeginInventory.MassPriceInventoryChange * dailyWacValue;
				return;
			}


			// For a Single Owner System, or for a Manager ledger(Inventory Reconciliation, Closeout),
			// a previous Physical Inventory may used for Begin Inventory.
			if (this.usePreviousPhysicalInventory)
			{
				if (previousLineItem.HasPhysicalInventory)
				{
					foreach (string aliasName in previousLineItem.QuantityList.Keys)
					{
						LRTransactionAliasDO alias = this.aliasListDO[aliasName];

						if ((alias != null) && (this.aliasListDO.IsPhysicalInventory(aliasName)))
						{
							lineItem.BeginInventory.GrossInventoryChange +=
													((LRQuantityDO)previousLineItem.QuantityList[aliasName]).GrossInventoryChange;
							lineItem.BeginInventory.NetInventoryChange +=
													((LRQuantityDO)previousLineItem.QuantityList[aliasName]).NetInventoryChange;
							lineItem.BeginInventory.MassInventoryChange +=
													((LRQuantityDO)previousLineItem.QuantityList[aliasName]).MassInventoryChange;
							lineItem.BeginInventory.PackageInventoryChange +=
													((LRQuantityDO)previousLineItem.QuantityList[aliasName]).PackageInventoryChange;

							lineItem.BeginInventory.GrossPriceInventoryChange +=
								(((LRQuantityDO)previousLineItem.QuantityList[aliasName]).GrossInventoryChange * dailyWacValue);
							lineItem.BeginInventory.NetPriceInventoryChange +=
								(((LRQuantityDO)previousLineItem.QuantityList[aliasName]).NetInventoryChange * dailyWacValue);
							lineItem.BeginInventory.MassPriceInventoryChange +=
								(((LRQuantityDO)previousLineItem.QuantityList[aliasName]).MassInventoryChange * dailyWacValue);
						}
					}
				}
				else
				{
					lineItem.BeginInventory.GrossInventoryChange = previousLineItem.BookInventory.GrossInventoryChange;
					lineItem.BeginInventory.NetInventoryChange = previousLineItem.BookInventory.NetInventoryChange;
					lineItem.BeginInventory.MassInventoryChange = previousLineItem.BookInventory.MassInventoryChange;
					lineItem.BeginInventory.PackageInventoryChange = previousLineItem.BookInventory.PackageInventoryChange;
					lineItem.BeginInventory.GrossPriceInventoryChange = previousLineItem.BookInventory.GrossInventoryChange * dailyWacValue;
					lineItem.BeginInventory.NetPriceInventoryChange = previousLineItem.BookInventory.NetInventoryChange * dailyWacValue;
					lineItem.BeginInventory.MassPriceInventoryChange = previousLineItem.BookInventory.MassInventoryChange * dailyWacValue;
				}

				return;
			}

			lineItem.BeginInventory.GrossInventoryChange = previousLineItem.BookInventory.GrossInventoryChange;
			lineItem.BeginInventory.NetInventoryChange = previousLineItem.BookInventory.NetInventoryChange;
			lineItem.BeginInventory.MassInventoryChange = previousLineItem.BookInventory.MassInventoryChange;
			lineItem.BeginInventory.PackageInventoryChange = previousLineItem.BookInventory.PackageInventoryChange;
			lineItem.BeginInventory.GrossPriceInventoryChange = previousLineItem.BookInventory.GrossInventoryChange * dailyWacValue;
			lineItem.BeginInventory.NetPriceInventoryChange = previousLineItem.BookInventory.NetInventoryChange * dailyWacValue;
			lineItem.BeginInventory.MassPriceInventoryChange = previousLineItem.BookInventory.MassInventoryChange * dailyWacValue;
		}

		/// <summary>
		/// This method will calculate the book inventory for a given day.
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="dailyWacValue"></param>
		protected void CalculateBookInventory(LRInventoryLineItemDO lineItem, double dailyWacValue)
		{
			lineItem.BookInventory.GrossInventoryChange = lineItem.BeginInventory.GrossInventoryChange;
			lineItem.BookInventory.NetInventoryChange = lineItem.BeginInventory.NetInventoryChange;
			lineItem.BookInventory.MassInventoryChange = lineItem.BeginInventory.MassInventoryChange;
			lineItem.BookInventory.PackageInventoryChange = lineItem.BeginInventory.PackageInventoryChange;
			lineItem.BookInventory.GrossPriceInventoryChange = lineItem.BeginInventory.GrossPriceInventoryChange;
			lineItem.BookInventory.NetPriceInventoryChange = lineItem.BeginInventory.NetPriceInventoryChange;
			lineItem.BookInventory.MassPriceInventoryChange = lineItem.BeginInventory.MassPriceInventoryChange;
			lineItem.TotalActivity.GrossInventoryChange = 0.0;
			lineItem.TotalActivity.NetInventoryChange = 0.0;
			lineItem.TotalActivity.MassInventoryChange = 0.0;
			lineItem.TotalActivity.PackageInventoryChange = 0.0;
			lineItem.TotalActivity.GrossPriceInventoryChange = 0.0;
			lineItem.TotalActivity.NetPriceInventoryChange = 0.0;
			lineItem.TotalActivity.MassPriceInventoryChange = 0.0;

			foreach (string aliasName in lineItem.QuantityList.Keys)
			{
				var quantity = (LRQuantityDO)lineItem.QuantityList[aliasName];

				if (this.aliasListDO.AffectsInventory(aliasName))
				{
					lineItem.BookInventory.GrossInventoryChange += quantity.GrossInventoryChange;
					lineItem.BookInventory.NetInventoryChange += quantity.NetInventoryChange;
					lineItem.BookInventory.MassInventoryChange += quantity.MassInventoryChange;
					lineItem.BookInventory.PackageInventoryChange += quantity.PackageInventoryChange;

					// Total activity is based on transaction that affect inventories a given day.
					// It will be used in inventory reconciliation.
					lineItem.TotalActivity.GrossInventoryChange += quantity.GrossInventoryChange;
					lineItem.TotalActivity.NetInventoryChange += quantity.NetInventoryChange;
					lineItem.TotalActivity.MassInventoryChange += quantity.MassInventoryChange;
					lineItem.TotalActivity.PackageInventoryChange += quantity.PackageInventoryChange;

				}
			}

		    if (this.systemEdition == LRLedgerProcessor.SystemEditions.Bsme)
		    {
                lineItem.BookInventory.NetInventoryChange = lineItem.BookInventory.GrossInventoryChange;
                lineItem.TotalActivity.NetInventoryChange = lineItem.TotalActivity.GrossInventoryChange;
		    }

			// Calculate price based on average unit price
			lineItem.BookInventory.GrossPriceInventoryChange = lineItem.BookInventory.GrossInventoryChange * dailyWacValue;
			lineItem.BookInventory.NetPriceInventoryChange = lineItem.BookInventory.NetInventoryChange * dailyWacValue;
			lineItem.BookInventory.MassPriceInventoryChange = lineItem.BookInventory.MassInventoryChange * dailyWacValue;
			lineItem.TotalActivity.GrossPriceInventoryChange = lineItem.TotalActivity.GrossInventoryChange * dailyWacValue;
			lineItem.TotalActivity.NetPriceInventoryChange = lineItem.TotalActivity.NetInventoryChange * dailyWacValue;
			lineItem.TotalActivity.MassPriceInventoryChange = lineItem.TotalActivity.MassInventoryChange * dailyWacValue;
		}

		/// <summary>
		/// This method will calculate the variance for a given day. The variance is based on the
		/// difference of the book and physical inventories.
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="dailyWacValue"></param>
		protected void CalculateVariance(LRInventoryLineItemDO lineItem, double dailyWacValue)
		{
			var dailyPhysical = new LRQuantityDO(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
			bool physicalPresent = false;

			foreach (string aliasName in lineItem.QuantityList.Keys)
			{
				var alias = this.aliasListDO.AliasList[aliasName] as LRTransactionAliasDO;

				if ((alias != null) && (alias.TransactionTypeID == LRTransactionAliases.TransactionTypes.T14PhysicalInventory))
				{
					var quantity = lineItem.QuantityList[aliasName] as LRQuantityDO;

					if (quantity != null)
					{
						dailyPhysical.GrossInventoryChange += quantity.GrossInventoryChange;
						dailyPhysical.NetInventoryChange += quantity.NetInventoryChange;
						dailyPhysical.MassInventoryChange += quantity.MassInventoryChange;
						dailyPhysical.PackageInventoryChange += quantity.PackageInventoryChange;

						// Updated to calculate price using average unit price
						dailyPhysical.GrossPriceInventoryChange += quantity.GrossInventoryChange * dailyWacValue;
						dailyPhysical.NetPriceInventoryChange += quantity.NetInventoryChange * dailyWacValue;
						dailyPhysical.MassPriceInventoryChange += quantity.MassInventoryChange * dailyWacValue;
					}

					if (lineItem.HasPhysicalInventory)
					{
						physicalPresent = true;
					}
				}
			}

			// Set the variance to zero if there is not a physical inventory for the current day.
			if (physicalPresent)
			{
				lineItem.Variance.GrossInventoryChange =
									dailyPhysical.GrossInventoryChange - lineItem.BookInventory.GrossInventoryChange;
				lineItem.Variance.NetInventoryChange =
									dailyPhysical.NetInventoryChange - lineItem.BookInventory.NetInventoryChange;
				lineItem.Variance.MassInventoryChange =
									dailyPhysical.MassInventoryChange - lineItem.BookInventory.MassInventoryChange;
				lineItem.Variance.PackageInventoryChange =
									dailyPhysical.PackageInventoryChange - lineItem.BookInventory.PackageInventoryChange;

				// Updated to calculate price using average unit price
				lineItem.Variance.GrossPriceInventoryChange =
									dailyPhysical.GrossPriceInventoryChange - (lineItem.BookInventory.GrossInventoryChange * dailyWacValue);
				lineItem.Variance.NetPriceInventoryChange =
										 dailyPhysical.NetPriceInventoryChange - (lineItem.BookInventory.NetInventoryChange * dailyWacValue);
				lineItem.Variance.MassPriceInventoryChange =
										 dailyPhysical.MassPriceInventoryChange - (lineItem.BookInventory.MassInventoryChange * dailyWacValue);
			}
			else
			{
				lineItem.Variance.GrossInventoryChange = 0.0;
				lineItem.Variance.NetInventoryChange = 0.0;
				lineItem.Variance.MassInventoryChange = 0.0;
				lineItem.Variance.PackageInventoryChange = 0.0;
				lineItem.Variance.GrossPriceInventoryChange = 0.0;
				lineItem.Variance.NetPriceInventoryChange = 0.0;
				lineItem.Variance.MassPriceInventoryChange = 0.0;
			}
		}

		/// <summary>
		/// This method will calculate the total variance. This variance is the running
		/// total.
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="previousLineItem"></param>
		/// <param name="dailyWacValue"></param>
		protected void CalculateTotalVariance(LRInventoryLineItemDO lineItem, LRInventoryLineItemDO previousLineItem, double dailyWacValue)
		{
			if (previousLineItem == null)
			{
				lineItem.TotalVariance.GrossInventoryChange = lineItem.Variance.GrossInventoryChange;
				lineItem.TotalVariance.NetInventoryChange = lineItem.Variance.NetInventoryChange;
				lineItem.TotalVariance.MassInventoryChange = lineItem.Variance.MassInventoryChange;
				lineItem.TotalVariance.PackageInventoryChange = lineItem.Variance.PackageInventoryChange;

				// Updated to calculate price based on average unit price
				lineItem.TotalVariance.GrossPriceInventoryChange = lineItem.Variance.GrossInventoryChange * dailyWacValue;
				lineItem.TotalVariance.NetPriceInventoryChange = lineItem.Variance.NetInventoryChange * dailyWacValue;
				lineItem.TotalVariance.MassPriceInventoryChange = lineItem.Variance.MassInventoryChange * dailyWacValue;
			}
			else
			{
				// Take the previous day's daily variance and add it to the total for a given day.
				lineItem.TotalVariance.GrossInventoryChange +=
						  (previousLineItem.TotalVariance.GrossInventoryChange + lineItem.Variance.GrossInventoryChange);
				lineItem.TotalVariance.NetInventoryChange +=
						  (previousLineItem.TotalVariance.NetInventoryChange + lineItem.Variance.NetInventoryChange);
				lineItem.TotalVariance.MassInventoryChange +=
						  (previousLineItem.TotalVariance.MassInventoryChange + lineItem.Variance.MassInventoryChange);
				lineItem.TotalVariance.PackageInventoryChange +=
						  (previousLineItem.TotalVariance.PackageInventoryChange + lineItem.Variance.PackageInventoryChange);

				// Updated to calculate price based on average unit price
				lineItem.TotalVariance.GrossPriceInventoryChange +=
						 (previousLineItem.TotalVariance.GrossPriceInventoryChange + (lineItem.Variance.GrossInventoryChange * dailyWacValue));
				lineItem.TotalVariance.NetPriceInventoryChange +=
						  (previousLineItem.TotalVariance.NetPriceInventoryChange + (lineItem.Variance.NetInventoryChange * dailyWacValue));
				lineItem.TotalVariance.MassPriceInventoryChange +=
						  (previousLineItem.TotalVariance.MassPriceInventoryChange + (lineItem.Variance.MassInventoryChange * dailyWacValue));
			}
		}

		protected void CalculateOutOfToleranceFlags(LRInventoryLineItemDO lineItem)
		{
			if (lineItem.VariancePercentage.Gross > lineItem.Tolerance)
			{
			    // ReSharper disable once EmptyStatement
				; // Moved somewhere?  Stubbed in MOD as well
			}
		}

		/// <summary>
		/// This method will calculate the total physical inventory if there is more than one physical inventory.
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="dailyWacValue"></param>
		protected void CalculateTotalPhysicalInventory(LRInventoryLineItemDO lineItem, double dailyWacValue)
		{
			foreach (string aliasName in lineItem.QuantityList.Keys)
			{
				var alias = this.aliasListDO.AliasList[aliasName] as LRTransactionAliasDO;

				if ((alias != null) && (alias.TransactionTypeID == LRTransactionAliases.TransactionTypes.T14PhysicalInventory))
				{
					var quantity = lineItem.QuantityList[aliasName] as LRQuantityDO;

					// Recalculate the physical inventory price using the correct AUP.
					if (quantity != null)
					{
						quantity.GrossPrice = quantity.GrossInventoryChange * dailyWacValue;
						quantity.NetPrice = quantity.NetInventoryChange * dailyWacValue;
						quantity.MassPrice = quantity.MassInventoryChange * dailyWacValue;

						// Sum up the gross and net inventory changes for the total.
						lineItem.TotalPhysicalInventory.GrossInventoryChange += quantity.GrossInventoryChange;
						lineItem.TotalPhysicalInventory.NetInventoryChange += quantity.NetInventoryChange;
						lineItem.TotalPhysicalInventory.MassInventoryChange += quantity.MassInventoryChange;
						lineItem.TotalPhysicalInventory.PackageInventoryChange += quantity.PackageInventoryChange;

						// Updated to calculate price using average unit price
						lineItem.TotalPhysicalInventory.GrossPriceInventoryChange += quantity.GrossInventoryChange * dailyWacValue;
						lineItem.TotalPhysicalInventory.NetPriceInventoryChange += quantity.NetInventoryChange * dailyWacValue;
						lineItem.TotalPhysicalInventory.MassPriceInventoryChange += quantity.MassInventoryChange * dailyWacValue;
					}
				}
			}
		}

		/// <summary>
		/// This method will calculate the total movement (issues) inventories.  This is used to calculate
		/// the variance percentage.
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="previousLineItem"></param>
		/// <param name="productGuid"></param>
		/// <param name="tankGuid"></param>
		/// <param name="siteGuid"></param>
		/// <param name="customMovementFunctionName"></param>
		protected void CalculateCustomTotalMovement(LRInventoryLineItemDO lineItem,
													LRInventoryLineItemDO previousLineItem,
													Guid productGuid,
													Guid tankGuid,
													Guid siteGuid,
													string customMovementFunctionName)
		{
			// customMovementFunctionName = "usp_CustomLedgerMovement";
			string customParameterXML = string.Empty;

			foreach (string aliasName in lineItem.QuantityList.Keys)
			{
				if (string.Empty != aliasName)
				{
					// Build an XML string that will be passed to the custom SQL function.
					// This string will contain all the alias names, gross and net volumes.
					var quantityDO = lineItem.QuantityList[aliasName] as LRQuantityDO;
					customParameterXML = this.BuildMovementXMLString(aliasName, quantityDO, productGuid, tankGuid, customParameterXML);
				}
			}

			// Perform custom movement calculations using a custom SQL function and update the total
			// movement in the lineitem object. (IGO 26-Aug-2010)
			this.customMovementFunctionDO.ExecuteCustomFunction(customMovementFunctionName, customParameterXML, siteGuid, this.ledgerConnection);

			if (this.customMovementFunctionDO.Quantity != null)
			{
				lineItem.TotalMovement.GrossInventoryChange += this.customMovementFunctionDO.Quantity.GrossInventoryChange;
				lineItem.TotalMovement.NetInventoryChange += this.customMovementFunctionDO.Quantity.NetInventoryChange;
				lineItem.TotalMovement.MassInventoryChange += this.customMovementFunctionDO.Quantity.MassInventoryChange;
			}

			lineItem.Tolerance = this.customMovementFunctionDO.Tolerance;

			if (previousLineItem != null)
			{
				lineItem.TotalMovement.GrossInventoryChange += previousLineItem.TotalMovement.GrossInventoryChange;
				lineItem.TotalMovement.NetInventoryChange += previousLineItem.TotalMovement.NetInventoryChange;
				lineItem.TotalMovement.MassInventoryChange += previousLineItem.TotalMovement.MassInventoryChange;
			}

			// For pricing, total movement does not make sense. Therefore, set it to zero.
			lineItem.TotalMovement.GrossPriceInventoryChange = 0.0;
			lineItem.TotalMovement.NetPriceInventoryChange = 0.0;
			lineItem.TotalMovement.MassPriceInventoryChange = 0.0;
		}

		/// <summary>
		/// This method will calculate the total movement for core functionality.
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="previousLineItem"></param>
		/// <param name="productDO"></param>
		private void CalculateCoreTotalMovement(LRInventoryLineItemDO lineItem, 
												LRInventoryLineItemDO previousLineItem, 
												LRProductDO productDO)
		{
			foreach ( string aliasName in lineItem.QuantityList.Keys )
			{
				var alias = this.aliasListDO.AliasList[aliasName] as LRTransactionAliasDO;

				if ( alias != null )
				{
					if (alias.TransactionTypeID == LRTransactionAliases.TransactionTypes.T4SecondaryDefuel
						|| alias.TransactionTypeID == LRTransactionAliases.TransactionTypes.T8Receipt
						|| alias.TransactionTypeID == LRTransactionAliases.TransactionTypes.T1PrimaryAdjustment
						|| alias.TransactionTypeID == LRTransactionAliases.TransactionTypes.T15PrimaryRegrade)
					{
						var quantity = lineItem.QuantityList[aliasName] as LRQuantityDO;

						if (quantity != null)
						{
							if ((alias.TransactionTypeID == LRTransactionAliases.TransactionTypes.T1PrimaryAdjustment
							|| alias.TransactionTypeID == LRTransactionAliases.TransactionTypes.T15PrimaryRegrade)
								&& quantity.GrossInventoryChange <= 0.0)
							{
								continue;
							}

							lineItem.TotalMovement.GrossInventoryChange += quantity.GrossInventoryChange;
							lineItem.TotalMovement.NetInventoryChange += quantity.NetInventoryChange;
							lineItem.TotalMovement.MassInventoryChange += quantity.MassInventoryChange;
						}

						lineItem.Tolerance = productDO.VarianceTolerance;
					}
				}
			}

			if ( previousLineItem != null )
			{
				lineItem.TotalMovement.GrossInventoryChange += previousLineItem.TotalMovement.GrossInventoryChange;
				lineItem.TotalMovement.NetInventoryChange += previousLineItem.TotalMovement.NetInventoryChange;
				lineItem.TotalMovement.MassInventoryChange += previousLineItem.TotalMovement.MassInventoryChange;
			}

			// For pricing, total movement does not make sense. Therefore, set it to zero.
			lineItem.TotalMovement.GrossPriceInventoryChange = 0.0;
			lineItem.TotalMovement.NetPriceInventoryChange = 0.0;
			lineItem.TotalMovement.MassPriceInventoryChange = 0.0;
		}

		/// <summary>
		/// This method will perform the horizontal math aggregating alias into the aggregate
		/// columns.
		/// </summary>
		/// <param name="lineItem"></param>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		protected void CalculateAggregateColumns(LRInventoryLineItemDO lineItem)
		{
			IDictionaryEnumerator transAliasEnumerator = this.aliasListDO.AliasList.GetEnumerator();

			while (transAliasEnumerator.MoveNext())
			{
				var transAliasDO = transAliasEnumerator.Value as LRTransactionAliasDO;

				if (transAliasDO != null && transAliasDO.IsAggregateAlias)
				{
					bool hasReversals = false;
					string aggregateColumnName = transAliasDO.AliasName;
					string customFunctionName = transAliasDO.CustomFunctionName;
					string customParameterXML = string.Empty;
					var aggregateQuantityDO = lineItem.QuantityList[aggregateColumnName] as LRQuantityDO;

					for (int nextAlias = 0; nextAlias < transAliasDO.AliasesToAggregate.Count; nextAlias++)
					{
						var aliasName = transAliasDO.AliasesToAggregate[nextAlias] as string;
						var moniker = transAliasDO.AliasesToAggregateSymbols[nextAlias] as string;

						if (!string.IsNullOrEmpty(aliasName))
						{
							var quantityDO = lineItem.QuantityList[aliasName] as LRQuantityDO;

							if (quantityDO != null && aggregateQuantityDO != null)
							{
								// Only add a moniker if the volume (alias to aggregate) was created by having a 
								// transaction. Otherwise, it was a ledger filler volume.
								if (quantityDO.IsFillerQuantity == false)
								{
									aggregateQuantityDO.AppendMoniker(moniker);
									aggregateQuantityDO.OrErrorFlag(quantityDO.TransErrorFlag);
									aggregateQuantityDO.IsAggregateQuantity = true;
									hasReversals = hasReversals 
													|| lineItem.CheckFlag(aliasName, LRBaseInventoryLineItemDO.Status.TransWithReversals);
								}

								// Perform default volume aggregation if there is no custom function name.
								if (string.IsNullOrEmpty(customFunctionName))
								{
									aggregateQuantityDO.GrossInventoryChange += quantityDO.GrossInventoryChange;
									aggregateQuantityDO.GrossPriceInventoryChange += quantityDO.GrossPriceInventoryChange;
									aggregateQuantityDO.NetInventoryChange += quantityDO.NetInventoryChange;
									aggregateQuantityDO.NetPriceInventoryChange += quantityDO.NetPriceInventoryChange;
									aggregateQuantityDO.MassInventoryChange += quantityDO.MassInventoryChange;
									aggregateQuantityDO.MassPriceInventoryChange += quantityDO.MassPriceInventoryChange;
									aggregateQuantityDO.Number01Change += quantityDO.Number01Change;
									aggregateQuantityDO.Number02Change += quantityDO.Number02Change;
									aggregateQuantityDO.Number03Change += quantityDO.Number03Change;
									aggregateQuantityDO.Number04Change += quantityDO.Number04Change;
									aggregateQuantityDO.Number05Change += quantityDO.Number05Change;
									aggregateQuantityDO.Number06Change += quantityDO.Number06Change;
								}
								else
								{
									// Build an XML string that will be passed to the custom SQL function.
									// This string will contain the alias names and their volumes.
									customParameterXML = this.BuildXMLString(aliasName, quantityDO, customParameterXML);
								}
							}
						}
					}

					// Perform custom calculation using a custom SQL function and update the aggregate
					// column volume data object.
					if (!string.IsNullOrEmpty(customFunctionName))
					{
						this.customMathFunctionDO.ExecuteCustomFunction(this.ledgerConnection, customFunctionName, customParameterXML);

						if (this.customMathFunctionDO.Quantity != null && aggregateQuantityDO != null)
						{
							aggregateQuantityDO.GrossInventoryChange = this.customMathFunctionDO.Quantity.GrossInventoryChange;
							aggregateQuantityDO.GrossPriceInventoryChange = this.customMathFunctionDO.Quantity.GrossPriceInventoryChange;
							aggregateQuantityDO.NetInventoryChange = this.customMathFunctionDO.Quantity.NetInventoryChange;
							aggregateQuantityDO.NetPriceInventoryChange = this.customMathFunctionDO.Quantity.NetPriceInventoryChange;
							aggregateQuantityDO.MassInventoryChange = this.customMathFunctionDO.Quantity.MassInventoryChange;
							aggregateQuantityDO.MassPriceInventoryChange = this.customMathFunctionDO.Quantity.MassPriceInventoryChange;
							aggregateQuantityDO.Number01Change = this.customMathFunctionDO.Quantity.Number01Change;
							aggregateQuantityDO.Number02Change = this.customMathFunctionDO.Quantity.Number02Change;
							aggregateQuantityDO.Number03Change = this.customMathFunctionDO.Quantity.Number03Change;
							aggregateQuantityDO.Number04Change = this.customMathFunctionDO.Quantity.Number04Change;
							aggregateQuantityDO.Number05Change = this.customMathFunctionDO.Quantity.Number05Change;
							aggregateQuantityDO.Number06Change = this.customMathFunctionDO.Quantity.Number06Change;
						}
					}

					if (aggregateQuantityDO != null)
					{
						if (aggregateQuantityDO.IsAggregateQuantity && 
							(aggregateQuantityDO.GrossInventoryChange == 0.0 || aggregateQuantityDO.NetInventoryChange == 0.0))
						{
							lineItem.SetCellFlag(aggregateColumnName, LRBaseInventoryLineItemDO.Status.TransWithZeroQuantity);
						}

						if (aggregateQuantityDO.TransErrorFlag)
						{
							lineItem.SetCellFlag(aggregateColumnName, LRBaseInventoryLineItemDO.Status.TransErrorFlag);
						}

						if (hasReversals)
						{
							lineItem.SetCellFlag(aggregateColumnName, LRBaseInventoryLineItemDO.Status.TransWithReversals);
						}
					}
				}
			}
		}

		protected void CalculateAllowableGainLoss(LRInventoryLineItemDO lineItem)
		{
			lineItem.AllowableGainLoss.GrossInventoryChange = lineItem.ToleranceTestedQuantity.GrossInventoryChange * lineItem.Tolerance / 100.0;
			lineItem.AllowableGainLoss.NetInventoryChange = lineItem.ToleranceTestedQuantity.NetInventoryChange * lineItem.Tolerance / 100.0;
			lineItem.AllowableGainLoss.MassInventoryChange = lineItem.ToleranceTestedQuantity.MassInventoryChange * lineItem.Tolerance / 100.0;
		}

		protected void CalculateToleranceTestedVolume(LRInventoryLineItemDO lineItem, LRQuantityDO initialInventory, LRProductDO productDO)
		{
			if (this.systemEdition == LRLedgerProcessor.SystemEditions.Mod)
			{
				if (productDO.AviationProduct)
				{
					if (lineItem.TotalMovement.GrossInventoryChange > (initialInventory.GrossInventoryChange / 2.0))
					{
						lineItem.ToleranceTestedQuantity.GrossInventoryChange = lineItem.TotalMovement.GrossInventoryChange;
						lineItem.ToleranceTestedQuantity.NetInventoryChange = lineItem.TotalMovement.NetInventoryChange;
					}
					else
					{
						lineItem.ToleranceTestedQuantity.GrossInventoryChange = initialInventory.GrossInventoryChange;
						lineItem.ToleranceTestedQuantity.NetInventoryChange = initialInventory.NetInventoryChange;
					}
				}
				else
				{
					lineItem.ToleranceTestedQuantity.GrossInventoryChange = 
									initialInventory.GrossInventoryChange + lineItem.TotalMovement.GrossInventoryChange;
					lineItem.ToleranceTestedQuantity.NetInventoryChange = 
									initialInventory.NetInventoryChange + lineItem.TotalMovement.NetInventoryChange;
				}

			}
			else
			{
				lineItem.ToleranceTestedQuantity.GrossInventoryChange = lineItem.TotalMovement.GrossInventoryChange;
				lineItem.ToleranceTestedQuantity.NetInventoryChange = lineItem.TotalMovement.NetInventoryChange;
				lineItem.ToleranceTestedQuantity.MassInventoryChange = lineItem.TotalMovement.MassInventoryChange;
			}
		}

		protected void CalculateTolerance(LRInventoryLineItemDO lineItem, LRQuantityDO initialInventory, LRProductDO productDO)
		{
			if (this.systemEdition == LRLedgerProcessor.SystemEditions.Mod)
			{
				if (productDO.AviationProduct)
				{
					if (lineItem.TotalMovement.GrossInventoryChange > (initialInventory.GrossInventoryChange / 2.0))
					{
						lineItem.Tolerance = .5;
					}
					else
					{
						lineItem.Tolerance = .25;
					}
				}
				else
				{
					lineItem.Tolerance = 1;
				}
			}
			else
			{
				// Do nothing.  Use tolerance returned by the Custom Movement Function
			}
		}

		protected void CalculateVariancePercentage(LRInventoryLineItemDO lineItem)
		{
			lineItem.VariancePercentage.GrossInventoryChange = lineItem.TotalVariance.GrossInventoryChange * 100.0 
																/ lineItem.ToleranceTestedQuantity.GrossInventoryChange;
			lineItem.VariancePercentage.NetInventoryChange = lineItem.TotalVariance.NetInventoryChange * 100.0 
																/ lineItem.ToleranceTestedQuantity.NetInventoryChange;
			lineItem.VariancePercentage.MassInventoryChange = lineItem.TotalVariance.MassInventoryChange * 100.0 
																/ lineItem.ToleranceTestedQuantity.MassInventoryChange;
		}

		/// <summary>
		/// This method will return the most recent WAC based on the inventory date,
		/// product, and site.
		/// </summary>
		/// <param name="inventoryDateStr"></param>
		/// <param name="siteGuid"></param>
		/// <param name="productGuid"></param>
		/// <param name="whichDailyWac"></param>
		/// <returns></returns>
		protected double GetCurrentWac(	string inventoryDateStr, 
										Guid siteGuid, 
										Guid productGuid, 
										WhichDailyWac whichDailyWac)
		{
			double wacValue = 0;

		    if (this.systemEdition == LRLedgerProcessor.SystemEditions.Adf)
		    {
		        DateTimeOffset inventoryDate = this.ParseDate(inventoryDateStr);
		        var wacDO = new LRWeightAverageCostDO();

		        if (whichDailyWac == WhichDailyWac.PreviousDay)
		        {
		            var minusOneDay = new TimeSpan(-1, 0, 0, 0);

		            inventoryDate = inventoryDate.Add(minusOneDay);
		        }

		        wacDO.PerformWacQuery(this.ledgerConnection, siteGuid, productGuid, inventoryDate);
		        wacValue = wacDO.WacValue;
		    }

		    return wacValue;
		}

		/// <summary>
		/// This method will return a DateTimeOffset object representing the inventory
		/// date.
		/// </summary>
		/// <param name="inventoryDate"></param>
		/// <returns></returns>
		private DateTimeOffset ParseDate(string inventoryDate)
		{
			if ((inventoryDate != null) && (inventoryDate.Length >= 8))
			{
				try
				{
					string monthStr = inventoryDate.Substring(0, 2);
					string dayStr = inventoryDate.Substring(3, 2);
					string yearStr = inventoryDate.Substring(6, 4);

					int year = Convert.ToInt32(yearStr);
					int month = Convert.ToInt32(monthStr);
					int day = Convert.ToInt32(dayStr);

					var invDate = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
					return invDate;
				}
				catch (Exception)
				{
					return LedgerTime.Today();
				}
			}
			
			return LedgerTime.Today();
		}

		/// <summary>
		/// This method will build an XML string that contains the alias name and all the
		/// volumes (gross, net, number01 ... number06). The purpose is to pass to a custom
		/// SQL function to perform special math.
		/// </summary>
		/// <param name="aliasName"></param>
		/// <param name="quantityDO"></param>
		/// <param name="xmlString"></param>
		/// <returns></returns>
		private string BuildXMLString(string aliasName, LRQuantityDO quantityDO, string xmlString)
		{
			string newAliasName = aliasName.Replace(" ", "");

			string temp = "<" + newAliasName + ">";

			temp += "<g>" + quantityDO.GrossInventoryChange + "</g>";
			temp += "<nt>" + quantityDO.NetInventoryChange + "</nt>";
			temp += "<nt>" + quantityDO.MassInventoryChange + "</nt>";
			temp += "<gp>" + quantityDO.GrossPriceInventoryChange + "</gp>";
			temp += "<ntp>" + quantityDO.NetPriceInventoryChange + "</ntp>";
			temp += "<ntp>" + quantityDO.MassPriceInventoryChange + "</ntp>";
			temp += "<n1>" + quantityDO.Number01Change + "</n1>";
			temp += "<n2>" + quantityDO.Number02Change + "</n2>";
			temp += "<n3>" + quantityDO.Number03Change + "</n3>";
			temp += "<n4>" + quantityDO.Number04Change + "</n4>";
			temp += "<n5>" + quantityDO.Number05Change + "</n5>";
			temp += "<n6>" + quantityDO.Number06Change + "</n6>";

			temp += "</" + newAliasName + ">";

			xmlString += temp;
			return xmlString;
		}

		/// <summary>
		/// This method will build an XML string that contains the alias name and gross/net.
		/// The purpose is to pass to a custom SQL function to perform special movement
		/// calculations.
		/// </summary>
		/// <param name="aliasName"></param>
		/// <param name="quantityDO"></param>
		/// <param name="tankGuid"></param>
		/// <param name="xmlString"></param>
		/// <param name="productGuid"></param>
		/// <returns></returns>
		private string BuildMovementXMLString(string aliasName, LRQuantityDO quantityDO, Guid productGuid, Guid tankGuid, string xmlString)
		{
			string temp = "<Alias>";
			temp += "<name>" + aliasName.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("'", "&apos;").Replace("\"", "&quot;") + "</name>";
			temp += "<g>" + quantityDO.GrossInventoryChange.ToString(CultureInfo.InvariantCulture) + "</g>";
			temp += "<nt>" + quantityDO.NetInventoryChange.ToString(CultureInfo.InvariantCulture) + "</nt>";
			temp += "<m>" + quantityDO.MassInventoryChange.ToString(CultureInfo.InvariantCulture) + "</m>";
			temp += "<tankGuid>" + tankGuid + "</tankGuid>";
			temp += "<productGuid>" + productGuid + "</productGuid>";
			temp += "</Alias>";

			xmlString += temp;
			return xmlString;
		}

		/// <summary>
		/// This method will return the current site information.
		/// </summary>
		/// <returns></returns>
		private LRProductDO RetrieveProductInfo(Guid productGuid, Guid siteGuid)
		{
			// Use the product conversion factor and precision if the product is configured to
			// have them.
			var productDO = new LRProductDO { ProductGuid = productGuid };
			productDO.RetrieveProductInfo(this.ledgerConnection, siteGuid);

			return productDO;
		}

		private string GetCustomMovementCalculationSpName()
		{
			const string SQL = "Select SettingValue FROM dbo.tblConfigurationSetting WHERE SettingKey = 'TotalMovementCalculationName'";
			string toRet = "";

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = SQL;
				DataSet dataSet = this.ledgerConnection.GetDataSet(cmd);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					toRet = (string)dataSet.Tables[0].Rows[0][0];
				}
			}
			
			return toRet;
		}
		#endregion Protected Methods
	}
}