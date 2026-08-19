namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class LRLedgerManager
	{
		#region Attributes
		private readonly LRTransactionAliasListDO aliasListDO;
		private readonly Hashtable aliasTypeList;
		private readonly bool usePreviousPhysicalInventory;
		private readonly LRLedgerProcessor.SystemEditions systemEdition;
		private readonly LedgerConnection ledgerConnection;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		/// <param name="aliasListDO"></param>
		/// <param name="usePreviousPhysicalInventory"></param>
		/// <param name="edition"></param>
		/// <param name="inLedgerConnection"></param>
		public LRLedgerManager(	LRTransactionAliasListDO aliasListDO, 
								bool usePreviousPhysicalInventory, 
								LRLedgerProcessor.SystemEditions edition,
								LedgerConnection inLedgerConnection)
		{
			this.aliasListDO					= aliasListDO;
			this.usePreviousPhysicalInventory	= usePreviousPhysicalInventory;
			this.aliasTypeList					= new Hashtable();
			this.systemEdition					= edition;
			this.ledgerConnection				= inLedgerConnection;
		}

		/// <summary>
		/// This constructor is used when alias type lists are necessary.
		/// </summary>
		/// <param name="aliasListDO"></param>
		/// <param name="aliasTypeList"></param>
		/// <param name="usePreviousPhysicalInventory"></param>
		/// <param name="edition"></param>
		/// <param name="inLedgerConnection"></param>
		public LRLedgerManager(	LRTransactionAliasListDO aliasListDO,
								Hashtable aliasTypeList,
								bool usePreviousPhysicalInventory,
								LRLedgerProcessor.SystemEditions edition,
								LedgerConnection inLedgerConnection)
		{
			this.aliasListDO					= aliasListDO;
			this.aliasTypeList					= aliasTypeList;
			this.usePreviousPhysicalInventory	= usePreviousPhysicalInventory;
			this.systemEdition					= edition;
			this.ledgerConnection				= inLedgerConnection;
		}
		#endregion

		#region Public Methods

		/// <summary>
		/// This method will create the ledger and fill the ledger with zeros and then
		/// with data.
		/// </summary>
		/// <param name="siteGuid"></param>
		/// <param name="ledgers">Contains a list of LedgerLineItem collections.  Each collection contains ledger transactions
		/// for a particular site</param>
		/// <param name="startDate">The first day of the month selected on the ledger UI</param>
		/// <param name="stopDate">The last day of the month selected on the ledger UI</param>
		/// <param name="siteList"></param>
		/// <returns></returns>
		public LRLedgerLineItemCollection CreateLedger(	Guid siteGuid,
														List<LRLedgerLineItemCollection> ledgers,
														DateTime startDate,
														DateTime stopDate,
														List<LRSiteDO> siteList)
		{
			var calculator = new LRLedgerCalculator(this.aliasListDO, 
													this.usePreviousPhysicalInventory, 
													this.systemEdition,
													this.ledgerConnection);

			foreach (LRLedgerLineItemCollection ledger in ledgers)
			{
				// Among other things, put a blank ledger item in for days in which there were
				// no transactions.  This allows the ledger to display "0" for these days
				this.FillLedger(ledger, startDate, stopDate);

				LRSiteDO siteDO = siteList.Find(x => x.SiteGuid == ledger.SiteGuid);
				LRQuantityDO initialBeginInventory = siteDO.InitialBookInventory;

				calculator.CalculateLedger(siteGuid, ledger, initialBeginInventory);
				this.TrimLedger(ledger, startDate, stopDate);

				if (string.IsNullOrEmpty(siteDO.LedgerCloseoutStatusDateStr) == false)
				{
					this.SetCloseoutStatus(ledger, this.ConvertMonthDayYearToDate(siteDO.LedgerCloseoutStatusDateStr));
				}

				if (string.IsNullOrEmpty(siteDO.LedgerBrokenBlendStatusDateStr) == false)
				{
					this.SetBrokenBlendStatus(ledger, this.ConvertMonthDayYearToDate(siteDO.LedgerBrokenBlendStatusDateStr));
				}

				// Set the alias/type list in the 1st line item of a ledger. It will be
				// used if an URL for a given alias column needs special naviagtion.
				var lineItem = (LRInventoryLineItemDO) ledger[0];
				lineItem.AliasTypeList = this.aliasTypeList;
			}

			LRLedgerLineItemCollection finalLedger = this.CombineLedgers(ledgers, startDate, stopDate);

			return finalLedger;
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ledger"></param>
		/// <param name="startDate">The first day of the month selected on the ledger UI page</param>
		/// <param name="stopDate">The last day of the month selected on the ledger UI page</param>
		protected void FillLedger(LRLedgerLineItemCollection ledger, DateTime startDate, DateTime stopDate)
		{
			TimeSpan span = stopDate - startDate;
			int dayCount = span.Days + 1;
			int prespanDays = 0;

			for (int nextDay = 0; nextDay < (dayCount + prespanDays); ++nextDay)
			{
				const bool SuppressLink = false;
				DateTime currentDate = startDate.AddDays(nextDay - prespanDays);
				var lineItem = (LRInventoryLineItemDO)ledger[nextDay];

				if (lineItem == null)
				{
					// We have reached the end of the existing ledger. We will be adding the rest of the inventory dates.
					// Add an empty ledger item representing the date in the currentDate variable.  This item will
					// have no activity but will be represented on the ledger
					lineItem = this.CreateLineItem(currentDate, SuppressLink);
					ledger.Insert(nextDay, lineItem);
					continue;
				}

				string inventoryDateStr = lineItem.InventoryDate;
				DateTimeOffset lineItemInventoryDate = this.ConvertMonthDayYearToDate(inventoryDateStr);

				if (currentDate == lineItemInventoryDate)
				{
					//If only it were always true...  a row exists for this date;
					//Add Volumes for Transaction Aliases that do not exist for this row.
					this.FillLineItem(lineItem, SuppressLink);
					continue;
				}

				if (currentDate < lineItemInventoryDate)
				{
					//The next date in the ledger is after this one, meaning the ledger skipped at least 1 day.
					//We have to insert a row.
					lineItem = this.CreateLineItem(currentDate, SuppressLink);

					//We insert the new lineItem in front of the lineItem we just examined,
					//pushing the existing one back 1 spot.
					ledger.Insert(nextDay, lineItem);
					continue;
				}

				//The only choice left here is that the existing lineItem's inventory date is an earlier date than the one we
				//want to insert. This will happen because we have lineItems prior to the startDate that we needed to determine
				//the starting Begin Inventory. We will keep a count of these in prespanDays.
				this.FillLineItem(lineItem, SuppressLink);
				++prespanDays;
			}
		}

		/// <summary>
		/// This method will create an inventory line item for the given date.
		/// </summary>
		/// <param name="inventoryDate"></param>
		/// <param name="suppressLinks"></param>
		/// <returns></returns>
		protected LRInventoryLineItemDO CreateLineItem(DateTime inventoryDate, bool suppressLinks)
		{
			var lineItem = new LRInventoryLineItemDO { InventoryDate = this.ConvertToMonthDayYear(inventoryDate) };

			this.FillLineItem(lineItem, suppressLinks);

			if (suppressLinks)
			{
				lineItem.Flags = LRBaseInventoryLineItemDO.Status.SuppressLink;
			}

			return lineItem;
		}

		/// <summary>
		/// This method will fill the line item to its initial state of zero for all aliases
		/// except a physical inventory alias, which will be initialized to "n/a".
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="suppressLinks"></param>
		protected void FillLineItem(LRInventoryLineItemDO lineItem, bool suppressLinks)
		{
			foreach (string key in this.aliasListDO.AliasList.Keys)
			{
				if (lineItem.QuantityList.ContainsKey(key))
				{
					// Set the flag that will add an asterisk if the transaction volume is zero.
					var quantityDO = lineItem.QuantityList[key];

					if (quantityDO != null &&
						(quantityDO.GrossInventoryChange == 0.0
						|| quantityDO.NetInventoryChange == 0.0
						|| quantityDO.MassInventoryChange == 0.0
						|| quantityDO.PackageInventoryChange == 0.0))
					{
						lineItem.SetCellFlag(key, LRBaseInventoryLineItemDO.Status.TransWithZeroQuantity);
					}

					// Since there is a transaction that created this quantity, then the Filler
					// Quantity flag is set to false.
					if (quantityDO != null)
					{
						quantityDO.IsFillerQuantity = false;

						// Set the volume to have a transaction error, if any of the transactions
						// representing the volume has the error flag set.
						if (quantityDO.TransErrorFlag)
						{
							lineItem.SetCellFlag(key, LRBaseInventoryLineItemDO.Status.TransErrorFlag);
						}
					}
				}
				else
				{
					lineItem.AddQuantity(key, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

					if (this.aliasListDO.IsPhysicalInventory(key))
					{
						lineItem.SetCellFlag(key, LRBaseInventoryLineItemDO.Status.Na);
					}
					else if (suppressLinks)
					{
						lineItem.SetCellFlag(key, LRBaseInventoryLineItemDO.Status.SuppressLink);
					}
				}
			}
		}

		/// <summary>
		/// This method will trim the ledger to current month.
		/// </summary>
		/// <param name="ledger"></param>
		/// <param name="startDate"></param>
		/// <param name="stopDate"></param>
		protected void TrimLedger(LRLedgerLineItemCollection ledger, DateTime startDate, DateTime stopDate)
		{
			LRInventoryLineItemDO lineItem;

			//Remove rows prior to the start date
			for (lineItem = (LRInventoryLineItemDO)ledger[0];
				  (this.ConvertMonthDayYearToDate(lineItem.InventoryDate)).CompareTo(startDate) < 0;
				  lineItem = (LRInventoryLineItemDO)ledger[0])
			{
				ledger.RemoveAt(0);
			}

			//Remove days later than the stop date.  Can these really exist??
			TimeSpan span = stopDate - startDate;
			int removeRow = span.Days + 1;

			while (ledger.Count > removeRow)
			{
				ledger.RemoveAt(removeRow);
			}
		}

		/// <summary>
		/// This method will set the closeout flag if the inventory date is greater than the 
		/// closeout date.
		/// </summary>
		/// <param name="ledger"></param>
		/// <param name="closeoutDate"></param>
		protected void SetCloseoutStatus(LRLedgerLineItemCollection ledger, DateTimeOffset closeoutDate)
		{
			foreach (LRInventoryLineItemDO lineItem in ledger)
			{
				// The inventory date should be formatted based on site settings.  In order for the
				// rest of this to work it has to be unformatted then compared to the closeout date
				DateTimeOffset inventoryDate = this.ConvertMonthDayYearToDate(lineItem.InventoryDate);

				if (inventoryDate > closeoutDate)
				{
					return;
				}

				lineItem.Flags += LRBaseInventoryLineItemDO.Status.ClosedOut;
			}
		}

		/// <summary>
		/// This method will set the broken blend flags if the inventory date is less than
		/// the broken blend date.
		/// </summary>
		/// <param name="ledger"></param>
		/// <param name="brokenBlendDate"></param>
		protected void SetBrokenBlendStatus(LRLedgerLineItemCollection ledger, DateTimeOffset brokenBlendDate)
		{
			foreach(LRInventoryLineItemDO lineItem in ledger)
			{
				DateTimeOffset inventoryDate = this.ConvertMonthDayYearToDate(lineItem.InventoryDate);

				if (inventoryDate < brokenBlendDate)
				{
					return;
				}

				lineItem.Flags += LRBaseInventoryLineItemDO.Status.BrokenBlends;
			}
		}


		/// <summary>
		/// This method will combine the individual ledgers into one ledger.
		/// </summary>
		/// <param name="ledgers"></param>
		/// <param name="startDate"></param>
		/// <param name="stopDate"></param>
		/// <returns></returns>
		protected LRLedgerLineItemCollection CombineLedgers(List<LRLedgerLineItemCollection> ledgers,
															DateTime startDate,
															DateTime stopDate)
		{
			// Get the list of physical alias names.
			var aliasNameList = new Hashtable();
			this.GetPhysicalInvAliasNames(aliasNameList);

			var finalLedger = new LRLedgerLineItemCollection();

			LRInventoryLineItemDO totalLineItem = this.CreateLineItem(LedgerTime.Today().Date, true);
			totalLineItem.InventoryDate = "Total:";

			totalLineItem.SetCellFlag("Begin Inventory", LRBaseInventoryLineItemDO.Status.Suppress);
			totalLineItem.SetCellFlag("Book Inventory", LRBaseInventoryLineItemDO.Status.Suppress);
			totalLineItem.SetCellFlag("Total Physical Inventory", LRBaseInventoryLineItemDO.Status.Suppress);
			totalLineItem.SetCellFlag("Total Variance", LRBaseInventoryLineItemDO.Status.Suppress);
			totalLineItem.SetCellFlag("Variance", LRBaseInventoryLineItemDO.Status.Suppress);
			totalLineItem.SetCellFlag("Total Activity", LRBaseInventoryLineItemDO.Status.Suppress);

			// Also supress any Physical Inventory aliases on the total line
			foreach (string key in this.aliasListDO.AliasList.Keys)
			{
				if (this.aliasListDO.IsPhysicalInventory(key))
				{
					totalLineItem.SetCellFlag(key, LRBaseInventoryLineItemDO.Status.Suppress);
				}
			}

			var statusCombiner = new LRLedgerStatusCombiner();
			DateTime currentDate = startDate;

			for (int nextRow = 0; currentDate <= stopDate; ++nextRow)
			{
				LRInventoryLineItemDO finalLineItem = this.CreateLineItem(currentDate, false);
				finalLedger.Add(finalLineItem);

				statusCombiner.ResetRuleList();

				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.Na, LRLedgerStatusCombiner.CombineRule.All);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.PhysInvExists, LRLedgerStatusCombiner.CombineRule.All);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.SuppressLink, LRLedgerStatusCombiner.CombineRule.All);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.Suppress, LRLedgerStatusCombiner.CombineRule.All);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.OutOfToleranceGross, LRLedgerStatusCombiner.CombineRule.Any);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.OutOfToleranceNet, LRLedgerStatusCombiner.CombineRule.Any);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.InvError, LRLedgerStatusCombiner.CombineRule.Any);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.TransErrorFlag, LRLedgerStatusCombiner.CombineRule.Any);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.ClosedOut, LRLedgerStatusCombiner.CombineRule.All);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.BrokenBlends, LRLedgerStatusCombiner.CombineRule.Any);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.TransWithZeroQuantity, LRLedgerStatusCombiner.CombineRule.Any);
				statusCombiner.SetCombineRule(finalLineItem, LRBaseInventoryLineItemDO.Status.TransWithReversals, LRLedgerStatusCombiner.CombineRule.Any);

				foreach (LRLedgerLineItemCollection ledger in ledgers)
				{
					var lineItem = (LRInventoryLineItemDO)ledger[nextRow];

					if (lineItem.MaxTransVersion > finalLineItem.MaxTransVersion)
					{
						finalLineItem.MaxTransVersion = lineItem.MaxTransVersion;
					}

					if (finalLineItem.MaxTransVersion > totalLineItem.MaxTransVersion)
					{
						totalLineItem.MaxTransVersion = finalLineItem.MaxTransVersion;
					}

					finalLineItem.BookInventory.GrossInventoryChange			+= lineItem.BookInventory.GrossInventoryChange;
					finalLineItem.BookInventory.NetInventoryChange				+= lineItem.BookInventory.NetInventoryChange;
					finalLineItem.BookInventory.MassInventoryChange				+= lineItem.BookInventory.MassInventoryChange;
					finalLineItem.BookInventory.PackageInventoryChange			+= lineItem.BookInventory.PackageInventoryChange;
					finalLineItem.BeginInventory.GrossInventoryChange			+= lineItem.BeginInventory.GrossInventoryChange;
					finalLineItem.BeginInventory.NetInventoryChange				+= lineItem.BeginInventory.NetInventoryChange;
					finalLineItem.BeginInventory.MassInventoryChange			+= lineItem.BeginInventory.MassInventoryChange;
					finalLineItem.BeginInventory.PackageInventoryChange			+= lineItem.BeginInventory.PackageInventoryChange;
					finalLineItem.TotalVariance.GrossInventoryChange			+= lineItem.TotalVariance.GrossInventoryChange;
					finalLineItem.TotalVariance.MassInventoryChange				+= lineItem.TotalVariance.MassInventoryChange;
					finalLineItem.TotalVariance.PackageInventoryChange			+= lineItem.TotalVariance.PackageInventoryChange;
					finalLineItem.TotalVariance.NetInventoryChange				+= lineItem.TotalVariance.NetInventoryChange;
					finalLineItem.Variance.GrossInventoryChange					+= lineItem.Variance.GrossInventoryChange;
					finalLineItem.Variance.NetInventoryChange					+= lineItem.Variance.NetInventoryChange;
					finalLineItem.Variance.MassInventoryChange					+= lineItem.Variance.MassInventoryChange;
					finalLineItem.Variance.PackageInventoryChange				+= lineItem.Variance.PackageInventoryChange;
					finalLineItem.TotalActivity.GrossInventoryChange			+= lineItem.TotalActivity.GrossInventoryChange;
					finalLineItem.TotalActivity.NetInventoryChange				+= lineItem.TotalActivity.NetInventoryChange;
					finalLineItem.TotalActivity.MassInventoryChange				+= lineItem.TotalActivity.MassInventoryChange;
					finalLineItem.TotalActivity.PackageInventoryChange			+= lineItem.TotalActivity.PackageInventoryChange;
					finalLineItem.TotalPhysicalInventory.GrossInventoryChange	+= lineItem.TotalPhysicalInventory.GrossInventoryChange;
					finalLineItem.TotalPhysicalInventory.NetInventoryChange		+= lineItem.TotalPhysicalInventory.NetInventoryChange;
					finalLineItem.TotalPhysicalInventory.MassInventoryChange	+= lineItem.TotalPhysicalInventory.MassInventoryChange;
					finalLineItem.TotalPhysicalInventory.PackageInventoryChange += lineItem.TotalPhysicalInventory.PackageInventoryChange;
					finalLineItem.TotalMovement.GrossInventoryChange			+= lineItem.TotalMovement.GrossInventoryChange;
					finalLineItem.TotalMovement.NetInventoryChange				+= lineItem.TotalMovement.NetInventoryChange;
					finalLineItem.TotalMovement.MassInventoryChange				+= lineItem.TotalMovement.MassInventoryChange;
					finalLineItem.TotalMovement.PackageInventoryChange			+= lineItem.TotalMovement.PackageInventoryChange;

					finalLineItem.BookInventory.GrossPriceInventoryChange			+= lineItem.BookInventory.GrossPriceInventoryChange;
					finalLineItem.BookInventory.NetPriceInventoryChange				+= lineItem.BookInventory.NetPriceInventoryChange;
					finalLineItem.BookInventory.MassPriceInventoryChange			+= lineItem.BookInventory.MassPriceInventoryChange;
					finalLineItem.BeginInventory.GrossPriceInventoryChange			+= lineItem.BeginInventory.GrossPriceInventoryChange;
					finalLineItem.BeginInventory.NetPriceInventoryChange			+= lineItem.BeginInventory.NetPriceInventoryChange;
					finalLineItem.BeginInventory.MassPriceInventoryChange			+= lineItem.BeginInventory.MassPriceInventoryChange;
					finalLineItem.TotalVariance.GrossPriceInventoryChange			+= lineItem.TotalVariance.GrossPriceInventoryChange;
					finalLineItem.TotalVariance.NetPriceInventoryChange				+= lineItem.TotalVariance.NetPriceInventoryChange;
					finalLineItem.TotalVariance.MassPriceInventoryChange			+= lineItem.TotalVariance.MassPriceInventoryChange;
					finalLineItem.Variance.GrossPriceInventoryChange				+= lineItem.Variance.GrossPriceInventoryChange;
					finalLineItem.Variance.NetPriceInventoryChange					+= lineItem.Variance.NetPriceInventoryChange;
					finalLineItem.Variance.MassPriceInventoryChange					+= lineItem.Variance.MassPriceInventoryChange;
					finalLineItem.TotalActivity.GrossPriceInventoryChange			+= lineItem.TotalActivity.GrossPriceInventoryChange;
					finalLineItem.TotalActivity.NetPriceInventoryChange				+= lineItem.TotalActivity.NetPriceInventoryChange;
					finalLineItem.TotalActivity.MassPriceInventoryChange			+= lineItem.TotalActivity.MassPriceInventoryChange;
					finalLineItem.TotalPhysicalInventory.GrossPriceInventoryChange	+= lineItem.TotalPhysicalInventory.GrossPriceInventoryChange;
					finalLineItem.TotalPhysicalInventory.NetPriceInventoryChange	+= lineItem.TotalPhysicalInventory.NetPriceInventoryChange;
					finalLineItem.TotalPhysicalInventory.MassPriceInventoryChange	+= lineItem.TotalPhysicalInventory.MassPriceInventoryChange;
					finalLineItem.TotalMovement.GrossPriceInventoryChange			+= lineItem.TotalMovement.GrossPriceInventoryChange;
					finalLineItem.TotalMovement.NetPriceInventoryChange				+= lineItem.TotalMovement.NetPriceInventoryChange;
					finalLineItem.TotalMovement.MassPriceInventoryChange			+= lineItem.TotalMovement.MassPriceInventoryChange;

					// Add per-line tolerance calcs
					finalLineItem.ToleranceTestedQuantity.GrossInventoryChange	+= lineItem.ToleranceTestedQuantity.GrossInventoryChange;
					finalLineItem.ToleranceTestedQuantity.NetInventoryChange	+= lineItem.ToleranceTestedQuantity.NetInventoryChange;
					finalLineItem.ToleranceTestedQuantity.MassInventoryChange	+= lineItem.ToleranceTestedQuantity.MassInventoryChange;
					
					finalLineItem.Tolerance = Math.Max(finalLineItem.Tolerance, lineItem.Tolerance);
					finalLineItem.AllowableGainLoss.GrossInventoryChange = Math.Abs(finalLineItem.ToleranceTestedQuantity.GrossInventoryChange * finalLineItem.Tolerance / 100.0);
					finalLineItem.AllowableGainLoss.NetInventoryChange = Math.Abs(finalLineItem.ToleranceTestedQuantity.NetInventoryChange * finalLineItem.Tolerance / 100.0);
					finalLineItem.AllowableGainLoss.MassInventoryChange = Math.Abs(finalLineItem.ToleranceTestedQuantity.MassInventoryChange * finalLineItem.Tolerance / 100.0);

					try
					{
						finalLineItem.VariancePercentage.GrossInventoryChange = finalLineItem.Variance.GrossInventoryChange * 100.0 / finalLineItem.ToleranceTestedQuantity.GrossInventoryChange;
						
						if (double.IsNaN(finalLineItem.VariancePercentage.GrossInventoryChange) ||
							double.IsInfinity(finalLineItem.VariancePercentage.GrossInventoryChange))
						{
							finalLineItem.VariancePercentage.GrossInventoryChange = 0.0;
						}
					}
					catch (DivideByZeroException)
					{
						finalLineItem.VariancePercentage.GrossInventoryChange = 0.0;
					}
					try
					{
						finalLineItem.VariancePercentage.NetInventoryChange = finalLineItem.Variance.NetInventoryChange * 100.0 / finalLineItem.ToleranceTestedQuantity.NetInventoryChange;
						
						if (double.IsNaN(finalLineItem.VariancePercentage.NetInventoryChange) ||
							double.IsInfinity(finalLineItem.VariancePercentage.NetInventoryChange))
						{
							finalLineItem.VariancePercentage.NetInventoryChange = 0.0;
						}
					}
					catch (DivideByZeroException)
					{
						finalLineItem.VariancePercentage.NetInventoryChange = 0.0;
					}
					try
					{
						finalLineItem.VariancePercentage.MassInventoryChange = finalLineItem.Variance.NetInventoryChange * 100.0 / finalLineItem.ToleranceTestedQuantity.NetInventoryChange;
						
						if (double.IsNaN(finalLineItem.VariancePercentage.MassInventoryChange) ||
							double.IsInfinity(finalLineItem.VariancePercentage.MassInventoryChange))
						{
							finalLineItem.VariancePercentage.MassInventoryChange = 0.0;
						}
					}
					catch (DivideByZeroException)
					{
						finalLineItem.VariancePercentage.MassInventoryChange = 0.0;
					}

					foreach (string alias in finalLineItem.QuantityList.Keys)
					{
						var finalQuantity = finalLineItem.QuantityList[alias];
						var quantity = lineItem.QuantityList[alias];

						finalQuantity.GrossInventoryChange		+= quantity.GrossInventoryChange;
						finalQuantity.NetInventoryChange		+= quantity.NetInventoryChange;
						finalQuantity.MassInventoryChange		+= quantity.MassInventoryChange;
						finalQuantity.PackageInventoryChange	+= quantity.PackageInventoryChange;
						finalQuantity.GrossPriceInventoryChange += quantity.GrossPriceInventoryChange;
						finalQuantity.NetPriceInventoryChange	+= quantity.NetPriceInventoryChange;
						finalQuantity.MassPriceInventoryChange	+= quantity.MassPriceInventoryChange;

						finalQuantity.Number01Change += quantity.Number01Change;
						finalQuantity.Number02Change += quantity.Number02Change;
						finalQuantity.Number03Change += quantity.Number03Change;
						finalQuantity.Number04Change += quantity.Number04Change;
						finalQuantity.Number05Change += quantity.Number05Change;
						finalQuantity.Number06Change += quantity.Number06Change;

						// Combine the monikers. It will ensure that the monikers
						// are not duplicated.
						finalQuantity.CombineMonikers(quantity.Moniker);

						if (quantity.Gross != 0 || quantity.Net != 0 || quantity.Mass != 0)
						{
							// Do not total the physical inventory aliases
							if (aliasNameList.Contains(alias) == false)
							{
								this.AddToTotal(totalLineItem, alias, quantity);
							}
						}

						// Set the transaction error flag if the volume is associated to an aggregate
						// column and the transaction error flag was set in the volume.
						if (quantity.IsAggregateQuantity && quantity.TransErrorFlag)
						{
							lineItem.SetCellFlag(alias, LRBaseInventoryLineItemDO.Status.TransErrorFlag);
						}
					}

					statusCombiner.CombineLedgerLineItemStatusFlags(lineItem, finalLineItem);
				}

				statusCombiner.ResetRuleList();
				currentDate = currentDate.AddDays(1);
			}

			finalLedger.Add(totalLineItem);
			return finalLedger;
		}

		/// <summary>
		/// Adds to total.
		/// </summary>
		/// <param name="totalLineItem">The total line item.</param>
		/// <param name="key">The key.</param>
		/// <param name="quantity">The quantity.</param>
		protected void AddToTotal(LRInventoryLineItemDO totalLineItem, string key, LRQuantityDO quantity)
		{
			var totalQuantity = totalLineItem.QuantityList[key];

			totalQuantity.GrossInventoryChange		+= quantity.GrossInventoryChange;
			totalQuantity.NetInventoryChange		+= quantity.NetInventoryChange;
			totalQuantity.MassInventoryChange		+= quantity.MassInventoryChange;
			totalQuantity.GrossPriceInventoryChange += quantity.GrossPriceInventoryChange;
			totalQuantity.NetPriceInventoryChange	+= quantity.NetPriceInventoryChange;
			totalQuantity.MassPriceInventoryChange	+= quantity.MassPriceInventoryChange;

			totalQuantity.Number01Change += quantity.Number01Change;
			totalQuantity.Number02Change += quantity.Number02Change;
			totalQuantity.Number03Change += quantity.Number03Change;
			totalQuantity.Number04Change += quantity.Number04Change;
			totalQuantity.Number05Change += quantity.Number05Change;
			totalQuantity.Number06Change += quantity.Number06Change;
		}


		/// <summary>
		/// This method will retrieve all the physical inventory alias names. It will load up the
		/// hash table that was passed in with the list.
		/// </summary>
		/// <param name="aliasNameHshTbl"></param>
		protected void GetPhysicalInvAliasNames(Hashtable aliasNameHshTbl)
		{
			if ((this.aliasTypeList != null) && (this.aliasTypeList.Count > 0))
			{
				IDictionaryEnumerator enumerator = this.aliasTypeList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var aliasName = (string)enumerator.Key;
					var type = (LRTransactionAliases.TransactionTypes)enumerator.Value;

					if (type == LRTransactionAliases.TransactionTypes.T14PhysicalInventory)
					{
						if (aliasNameHshTbl.Contains(aliasName) == false)
						{
							aliasNameHshTbl.Add(aliasName, type);
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will convert a date string to a DateTimeOffset object.
		/// </summary>
		/// <param name="dateStr"></param>
		/// <returns></returns>
		protected DateTime ConvertMonthDayYearToDate(string dateStr)
		{
			int month = 1;
			int day = 1;
			int year = 1;
			int index1 = -1;
			int index2 = -1;
			const string Slash = "/";
			const string Dash = "-";
			DateTime dateTime;

			if (!string.IsNullOrEmpty(dateStr))
			{
				string searchChar = dateStr.IndexOf(Slash) < 0 ? Dash : Slash;
				index1 = dateStr.IndexOf(searchChar);
				index2 = dateStr.IndexOf(searchChar, index1 + 1);
			}

			if ((index1 >= 0) && (index2 > index1))
			{
				if (dateStr != null)
				{
					month = int.Parse(dateStr.Substring(0, index1));
					day = int.Parse(dateStr.Substring((index1 + 1), (index2 - index1 - 1)));
					year = int.Parse(dateStr.Substring((index2 + 1), (dateStr.Length - index2 - 1)));
				}

				if (year < 50)
				{
					//map 0, 1, ..., 48, 49 to 2000, 2001, ..., 2048, 2049
					year += 2000;
				}
				else if (year < 100)
				{
					//map 50, 51, ..., 98, 99 to 1950, 1951, ..., 1998, 1999
					year += 1900;
				}
			}

			try
			{
				dateTime = new DateTime(year, month, day, 0, 0, 0);
			}
			catch (Exception)
			{
				dateTime = LedgerTime.MinFMDate.Date;
			}

			return dateTime;
		}

		/// <summary>
		/// This method will convert the date to the following format:
		/// mm/dd/yyyy.
		/// </summary>
		protected string ConvertToMonthDayYear(DateTime inDate)
		{
			int month = inDate.Month;
			int day = inDate.Day;
			int year = inDate.Year;

			string monthDayYearFormat = this.ZeroFill(month) + "/" + this.ZeroFill(day) + "/" + Convert.ToString(year);

			return monthDayYearFormat;
		}

		/// <summary>
		/// This method will convert a number to a string and if less
		/// than 10, it will prefix it with a zero.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		protected string ZeroFill(int number)
		{
			string zeroFillNumber = Convert.ToString(number);

			if (number < 10)
			{
				zeroFillNumber = "0" + zeroFillNumber;
			}

			return zeroFillNumber;
		}
		#endregion
	}
}