/// <summary>
///   File name:	LedgerManager.cs
///   Purpose:	   The purpose of this class is to orchestrate the building and calculation
///				   of the ledger.
///   Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA,
///				   2000.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Endress+Hauser.
///				   
///	Author(s):	Richard Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///	2010-03-19		W.Gray					Revised FillLineItem to not set the SUPRESS_LINK cell flag
///													for Physical Inventory WI 12125
///
/// </summary
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using System.Data.SqlClient;

public class LedgerManager
{
	#region Attributes
	protected TransactionAliasListDO aliasList;
	protected Hashtable aliasTypeList;
	protected bool usePreviousPhysicalInventory;
   protected CLRLedgerProcessor.SystemEditions systemEdition;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default constructor.
	/// </summary>
	/// <param name="aliasList"></param>
	/// <param name="usePreviousPhysicalInventory"></param>
	public LedgerManager(TransactionAliasListDO aliasList,bool usePreviousPhysicalInventory, CLRLedgerProcessor.SystemEditions edition)
	{
		this.aliasList                    = aliasList;
		this.usePreviousPhysicalInventory = usePreviousPhysicalInventory;
		this.aliasTypeList                = new Hashtable();
		this.systemEdition = edition;
	}

	/// <summary>
	/// This constructor is used when alias type lists are necessary.
	/// </summary>
	/// <param name="aliasList"></param>
	/// <param name="aliasTypeList"></param>
	/// <param name="usePreviousPhysicalInventory"></param>
	public LedgerManager(TransactionAliasListDO aliasList,
								Hashtable aliasTypeList,
								bool usePreviousPhysicalInventory,
								CLRLedgerProcessor.SystemEditions systemEdition)
	{
		this.aliasList                    = aliasList;
		this.aliasTypeList                = aliasTypeList;
		this.usePreviousPhysicalInventory = usePreviousPhysicalInventory;
		this.systemEdition = systemEdition;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// This method will create the ledger and fill the ledger with zeros and then
	/// with data.
	/// </summary>
	/// <param name="ledgerList">Contains a list of LedgerLineItem collections.  Each collection contains ledger transactions
	/// for a particular site</param>
	/// <param name="initialBeginInventory"></param>
	/// <param name="startDate">The first day of the month selected on the ledger UI</param>
	/// <param name="stopDate">The last day of the month selected on the ledger UI</param>
	/// <param name="closeoutDates"></param>
	/// <returns></returns>
	public LedgerLineItemCollection CreateLedger(SqlConnection a_connection,
																ArrayList ledgerList,
																ArrayList initialBeginInventoryList,
																DateTime startDate,
																DateTime stopDate,
																ArrayList closeoutDates,
																ArrayList brokenBlendDates)
	{
		int nextLedgerCount = ledgerList.Count;
		LedgerCalculator calculator = new LedgerCalculator(aliasList,usePreviousPhysicalInventory, systemEdition);

		for(int nextLedger = 0;nextLedger < ledgerList.Count;++nextLedger)
		{
			// Get the collection of ledger items
			LedgerLineItemCollection ledger = (LedgerLineItemCollection)ledgerList[nextLedger];

			// Among other things, put a blank ledger item in for days in which there were
			// no transactions.  This allows the ledger to display "0" for these days
			this.FillLedger(ledger,startDate,stopDate);
			QuantityDO initialBeginInventory = (QuantityDO)initialBeginInventoryList[nextLedger];

			calculator.CalculateLedger(a_connection,ledger,initialBeginInventory);
			this.TrimLedger(ledger,startDate,stopDate);

			string closeoutDate    = null;
			string brokenBlendDate = null;

			if(nextLedger < closeoutDates.Count)
			{
				closeoutDate = closeoutDates[nextLedger] as string;
			}

			if(nextLedger < brokenBlendDates.Count)
			{
				brokenBlendDate = brokenBlendDates[nextLedger] as string;
			}

			if(closeoutDate != null)
			{
				this.SetCloseoutStatus(ledger,this.ConvertMonthDayYearToDateTime(closeoutDate));
			}

			if(brokenBlendDate != null)
			{
				this.SetBrokenBlendStatus(ledger,this.ConvertMonthDayYearToDateTime(brokenBlendDate));
			}


			// Set the alias/type list in the 1st line item of a ledger. It will be
			// used if an URL for a given alias column needs special naviagtion.
			InventoryLineItemDO lineItem = (InventoryLineItemDO)ledger[0];
			lineItem.AliasTypeList       = this.aliasTypeList;
		}

		LedgerLineItemCollection finalLedger = this.CombineLedgers(ledgerList,startDate,stopDate);

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
	protected void FillLedger(LedgerLineItemCollection ledger,DateTime startDate,DateTime stopDate)
	{
		TimeSpan span   = stopDate - startDate;
		int dayCount    = span.Days + 1;
		int prespanDays = 0;

		for(int nextDay = 0;nextDay < (dayCount + prespanDays);++nextDay)
		{
			bool suppressLink            = false;
			DateTime currentDate         = startDate.AddDays(nextDay - prespanDays);
			InventoryLineItemDO lineItem = (InventoryLineItemDO)ledger[nextDay];

			if(lineItem == null)
			{
				// We have reached the end of the existing ledger. We will be adding the rest of the inventory dates.
				// Add an empty ledger item representing the date in the currentDate variable.  This item will
				// have no activity but will be represented on the ledger
				suppressLink = false;
				lineItem = this.CreateLineItem(currentDate,suppressLink);
				ledger.Insert(nextDay,lineItem);
				continue;
			}

			string inventoryDateStr        = lineItem.InventoryDate;
			DateTime lineItemInventoryDate = this.ConvertMonthDayYearToDateTime(inventoryDateStr);

			if(currentDate == lineItemInventoryDate)
			{
				//If only it were always true...  a row exists for this date;
				//Add Volumes for Transaction Aliases that do not exist for this row.
				suppressLink = false;
				this.FillLineItem(lineItem,suppressLink);
				continue;
			}

			if(currentDate < lineItemInventoryDate)
			{
				//The next date in the ledger is after this one, meaning the ledger skipped at least 1 day.
				//We have to insert a row.
				suppressLink = false;
				lineItem = this.CreateLineItem(currentDate,suppressLink);

				//We insert the new lineItem in front of the lineItem we just examined,
				//pushing the existing one back 1 spot.
				ledger.Insert(nextDay,lineItem);
				continue;
			}

			//The only choice left here is that the existing lineItem's inventory date is an earlier date than the one we
			//want to insert. This will happen because we have lineItems prior to the startDate that we needed to determine
			//the starting Begin Inventory. We will keep a count of these in prespanDays.
			suppressLink = false;
			this.FillLineItem(lineItem,suppressLink);
			++prespanDays;
		}
	}

	/// <summary>
	/// This method will create an inventory line item for the given date.
	/// </summary>
	/// <param name="inventoryDate"></param>
	/// <param name="suppressLinks"></param>
	/// <returns></returns>
	protected InventoryLineItemDO CreateLineItem(DateTime inventoryDate,bool suppressLinks)
	{
		InventoryLineItemDO lineItem = new InventoryLineItemDO();
		lineItem.InventoryDate = this.ConvertToMonthDayYear(inventoryDate);

		this.FillLineItem(lineItem,suppressLinks);

		if(suppressLinks == true)
		{
			lineItem.Flags = BaseInventoryLineItemDO.Status.SUPPRESS_LINK;
		}

		return lineItem;
	}

	/// <summary>
	/// This method will fill the line item to its initial state of zero for all aliases
	/// except a physical inventory alias, which will be initialized to "n/a".
	/// </summary>
	/// <param name="lineItem"></param>
	/// <param name="suppressLinks"></param>
	protected void FillLineItem(InventoryLineItemDO lineItem,bool suppressLinks)
	{
		foreach(string key in this.aliasList.aliasList.Keys)
		{
			if(lineItem.QuantityList[key] == null)
			{
				lineItem.AddQuantity(key,0,0,0,0,0,0,0,0,0,0,0,0,0);

				if(aliasList.IsPhysicalInventory(key) == true)
				{
					lineItem.SetCellFlag(key,BaseInventoryLineItemDO.Status.NA);
				}
				else if(suppressLinks == true)
				{
					lineItem.SetCellFlag(key,BaseInventoryLineItemDO.Status.SUPPRESS_LINK);
				}
			}
			else
			{
				// Set the flag that will add an asterisk if the transaction volume is zero.
				QuantityDO quantityDO = lineItem.QuantityList[key] as QuantityDO;

				if(quantityDO != null
            && (quantityDO.GrossInventoryChange == 0.0
				|| quantityDO.NetInventoryChange == 0.0
				|| quantityDO.MassInventoryChange == 0.0
				|| quantityDO.PackageInventoryChange == 0.0))
				{
					lineItem.SetCellFlag(key,BaseInventoryLineItemDO.Status.TRANS_WITH_ZERO_QUANTITY);
				}

				// Since there is a transaction that created this quantity, then the Filler
				// Quantity flag is set to false.
				if(quantityDO != null)
				{
					quantityDO.IsFillerQuantity = false;

					// Set the volume to have a transaction error, if any of the transactions
					// representing the volume has the error flag set.
					if(quantityDO.TransErrorFlag == true)
					{
						lineItem.SetCellFlag(key,BaseInventoryLineItemDO.Status.TRANS_ERROR_FLAG);
					}
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
	protected void TrimLedger(LedgerLineItemCollection ledger,DateTime startDate,DateTime stopDate)
	{
		InventoryLineItemDO lineItem;

		//Remove rows prior to the start date
		for(lineItem = (InventoryLineItemDO)ledger[0];
			  (this.ConvertMonthDayYearToDateTime(lineItem.InventoryDate)).CompareTo(startDate) < 0;
			  lineItem = (InventoryLineItemDO)ledger[0])
		{
			ledger.RemoveAt(0);
		}

		//Remove days later than the stop date.  Can these really exist??
		TimeSpan span = stopDate - startDate;
		int removeRow = span.Days + 1;

		while(ledger.Count > removeRow)
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
	protected void SetCloseoutStatus(LedgerLineItemCollection ledger,DateTime closeoutDate)
	{
		for(int nextLineItem = 0;nextLineItem < ledger.Count;++nextLineItem)
		{
			InventoryLineItemDO lineItem = (InventoryLineItemDO)ledger[nextLineItem];

			// The inventory date should be formatted based on site settings.  In order for the
			// rest of this to work it has to be unformatted then compared to the closeout date
			DateTime inventoryDate = this.ConvertMonthDayYearToDateTime(lineItem.InventoryDate);

			if(inventoryDate > closeoutDate)
			{
				return;
			}

			lineItem.Flags += BaseInventoryLineItemDO.Status.CLOSED_OUT;
		}
	}

	/// <summary>
	/// This method will set the broken blend flags if the inventory date is less than
	/// the broken blend date.
	/// </summary>
	/// <param name="ledger"></param>
	/// <param name="brokenBlendDate"></param>
	protected void SetBrokenBlendStatus(LedgerLineItemCollection ledger,DateTime brokenBlendDate)
	{
		for(int nextLineItem = ledger.Count - 1;nextLineItem >= 0;--nextLineItem)
		{
			InventoryLineItemDO lineItem = (InventoryLineItemDO)ledger[nextLineItem];
			DateTime inventoryDate = this.ConvertMonthDayYearToDateTime(lineItem.InventoryDate);

			if(inventoryDate < brokenBlendDate)
			{
				return;
			}

			lineItem.Flags += BaseInventoryLineItemDO.Status.BROKEN_BLENDS;
		}
	}


	/// <summary>
	/// This method will combine the individual ledgers into one ledger.
	/// </summary>
	/// <param name="ledgerList"></param>
	/// <param name="startDate"></param>
	/// <param name="stopDate"></param>
	/// <returns></returns>
	protected LedgerLineItemCollection CombineLedgers(ArrayList ledgerList,
																	  DateTime startDate,
																	  DateTime stopDate)
	{
		// Get the list of physical alias names.
		Hashtable aliasNameList = new Hashtable();
		this.GetPhysicalInvAliasNames(aliasNameList);

		LedgerLineItemCollection finalLedger = new LedgerLineItemCollection();

		InventoryLineItemDO totalLineItem = this.CreateLineItem(DateTime.Today,true);
		totalLineItem.InventoryDate = "Total:";
		totalLineItem.SetCellFlag("Begin Inventory",BaseInventoryLineItemDO.Status.SUPPRESS);
		totalLineItem.SetCellFlag("Book Inventory",BaseInventoryLineItemDO.Status.SUPPRESS);
		totalLineItem.SetCellFlag("Total Physical Inventory",BaseInventoryLineItemDO.Status.SUPPRESS);
		totalLineItem.SetCellFlag("Total Variance",BaseInventoryLineItemDO.Status.SUPPRESS);
		totalLineItem.SetCellFlag("Variance",BaseInventoryLineItemDO.Status.SUPPRESS);
		totalLineItem.SetCellFlag("Total Activity",BaseInventoryLineItemDO.Status.SUPPRESS);

		// Also supress any Physical Inventory aliases on the total line
		foreach(string key in aliasList.aliasList.Keys)
		{
			if(aliasList.IsPhysicalInventory(key))
			{
				totalLineItem.SetCellFlag(key,BaseInventoryLineItemDO.Status.SUPPRESS);
			}
		}

		LedgerStatusCombiner statusCombiner = new LedgerStatusCombiner();
		DateTime currentDate = startDate;

		for(int nextRow = 0;currentDate <= stopDate;++nextRow)
		{
			InventoryLineItemDO finalLineItem = this.CreateLineItem(currentDate,false);
			finalLedger.Add(finalLineItem);

			statusCombiner.ResetRuleList();

			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.NA,LedgerStatusCombiner.CombineRule.ALL);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.PHYS_INV_EXISTS,LedgerStatusCombiner.CombineRule.ALL);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.SUPPRESS_LINK,LedgerStatusCombiner.CombineRule.ALL);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.SUPPRESS,LedgerStatusCombiner.CombineRule.ALL);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.OUT_OF_TOLERANCE_GROSS,LedgerStatusCombiner.CombineRule.ANY);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.OUT_OF_TOLERANCE_NET,LedgerStatusCombiner.CombineRule.ANY);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.INV_ERROR,LedgerStatusCombiner.CombineRule.ANY);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.TRANS_ERROR_FLAG,LedgerStatusCombiner.CombineRule.ANY);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.CLOSED_OUT,LedgerStatusCombiner.CombineRule.ALL);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.BROKEN_BLENDS,LedgerStatusCombiner.CombineRule.ANY);
			statusCombiner.SetCombineRule(finalLineItem,BaseInventoryLineItemDO.Status.TRANS_WITH_ZERO_QUANTITY,LedgerStatusCombiner.CombineRule.ANY);

			for(int nextLedger = 0;nextLedger < ledgerList.Count;++nextLedger)
			{
				LedgerLineItemCollection ledger = (LedgerLineItemCollection)ledgerList[nextLedger];
				InventoryLineItemDO lineItem    = (InventoryLineItemDO)ledger[nextRow];

				if(lineItem.MaxTransVersion > finalLineItem.MaxTransVersion)
					finalLineItem.MaxTransVersion=lineItem.MaxTransVersion;

				if(finalLineItem.MaxTransVersion > totalLineItem.MaxTransVersion)
					totalLineItem.MaxTransVersion=finalLineItem.MaxTransVersion;	

				finalLineItem.BookInventory.GrossInventoryChange				+= lineItem.BookInventory.GrossInventoryChange;
				finalLineItem.BookInventory.NetInventoryChange					+= lineItem.BookInventory.NetInventoryChange;
				finalLineItem.BookInventory.MassInventoryChange					+= lineItem.BookInventory.MassInventoryChange;
				finalLineItem.BookInventory.PackageInventoryChange				+= lineItem.BookInventory.PackageInventoryChange;
				finalLineItem.BeginInventory.GrossInventoryChange				+= lineItem.BeginInventory.GrossInventoryChange;
				finalLineItem.BeginInventory.NetInventoryChange					+= lineItem.BeginInventory.NetInventoryChange;
				finalLineItem.BeginInventory.MassInventoryChange				+= lineItem.BeginInventory.MassInventoryChange;
				finalLineItem.BeginInventory.PackageInventoryChange			+= lineItem.BeginInventory.PackageInventoryChange;
				finalLineItem.TotalVariance.GrossInventoryChange				+= lineItem.TotalVariance.GrossInventoryChange;
				finalLineItem.TotalVariance.MassInventoryChange					+= lineItem.TotalVariance.MassInventoryChange;
				finalLineItem.TotalVariance.PackageInventoryChange				+= lineItem.TotalVariance.PackageInventoryChange;
				finalLineItem.TotalVariance.NetInventoryChange					+= lineItem.TotalVariance.NetInventoryChange;
				finalLineItem.Variance.GrossInventoryChange						+= lineItem.Variance.GrossInventoryChange;
				finalLineItem.Variance.NetInventoryChange							+= lineItem.Variance.NetInventoryChange;
				finalLineItem.Variance.MassInventoryChange						+= lineItem.Variance.MassInventoryChange;
				finalLineItem.Variance.PackageInventoryChange					+= lineItem.Variance.PackageInventoryChange;
				finalLineItem.TotalActivity.GrossInventoryChange				+= lineItem.TotalActivity.GrossInventoryChange;
				finalLineItem.TotalActivity.NetInventoryChange					+= lineItem.TotalActivity.NetInventoryChange;
				finalLineItem.TotalActivity.MassInventoryChange					+= lineItem.TotalActivity.MassInventoryChange;
				finalLineItem.TotalActivity.PackageInventoryChange				+= lineItem.TotalActivity.PackageInventoryChange;
				finalLineItem.TotalPhysicalInventory.GrossInventoryChange	+= lineItem.TotalPhysicalInventory.GrossInventoryChange;
				finalLineItem.TotalPhysicalInventory.NetInventoryChange		+= lineItem.TotalPhysicalInventory.NetInventoryChange;
				finalLineItem.TotalPhysicalInventory.MassInventoryChange		+= lineItem.TotalPhysicalInventory.MassInventoryChange;
				finalLineItem.TotalPhysicalInventory.PackageInventoryChange	+= lineItem.TotalPhysicalInventory.PackageInventoryChange;
				finalLineItem.TotalMovement.GrossInventoryChange				+= lineItem.TotalMovement.GrossInventoryChange;
				finalLineItem.TotalMovement.NetInventoryChange					+= lineItem.TotalMovement.NetInventoryChange;
				finalLineItem.TotalMovement.MassInventoryChange					+= lineItem.TotalMovement.MassInventoryChange;
				finalLineItem.TotalMovement.PackageInventoryChange				+= lineItem.TotalMovement.PackageInventoryChange;

				finalLineItem.BookInventory.GrossPriceInventoryChange          += lineItem.BookInventory.GrossPriceInventoryChange;
				finalLineItem.BookInventory.NetPriceInventoryChange            += lineItem.BookInventory.NetPriceInventoryChange;
				finalLineItem.BookInventory.MassPriceInventoryChange           += lineItem.BookInventory.MassPriceInventoryChange;
				finalLineItem.BeginInventory.GrossPriceInventoryChange         += lineItem.BeginInventory.GrossPriceInventoryChange;
				finalLineItem.BeginInventory.NetPriceInventoryChange           += lineItem.BeginInventory.NetPriceInventoryChange;
				finalLineItem.BeginInventory.MassPriceInventoryChange          += lineItem.BeginInventory.MassPriceInventoryChange;
				finalLineItem.TotalVariance.GrossPriceInventoryChange          += lineItem.TotalVariance.GrossPriceInventoryChange;
				finalLineItem.TotalVariance.NetPriceInventoryChange            += lineItem.TotalVariance.NetPriceInventoryChange;
				finalLineItem.TotalVariance.MassPriceInventoryChange           += lineItem.TotalVariance.MassPriceInventoryChange;
				finalLineItem.Variance.GrossPriceInventoryChange               += lineItem.Variance.GrossPriceInventoryChange;
				finalLineItem.Variance.NetPriceInventoryChange                 += lineItem.Variance.NetPriceInventoryChange;
				finalLineItem.Variance.MassPriceInventoryChange                += lineItem.Variance.MassPriceInventoryChange;
				finalLineItem.TotalActivity.GrossPriceInventoryChange          += lineItem.TotalActivity.GrossPriceInventoryChange;
				finalLineItem.TotalActivity.NetPriceInventoryChange            += lineItem.TotalActivity.NetPriceInventoryChange;
				finalLineItem.TotalActivity.MassPriceInventoryChange           += lineItem.TotalActivity.MassPriceInventoryChange;
				finalLineItem.TotalPhysicalInventory.GrossPriceInventoryChange += lineItem.TotalPhysicalInventory.GrossPriceInventoryChange;
				finalLineItem.TotalPhysicalInventory.NetPriceInventoryChange   += lineItem.TotalPhysicalInventory.NetPriceInventoryChange;
				finalLineItem.TotalPhysicalInventory.MassPriceInventoryChange  += lineItem.TotalPhysicalInventory.MassPriceInventoryChange;
				finalLineItem.TotalMovement.GrossPriceInventoryChange          += lineItem.TotalMovement.GrossPriceInventoryChange;
				finalLineItem.TotalMovement.NetPriceInventoryChange            += lineItem.TotalMovement.NetPriceInventoryChange;
				finalLineItem.TotalMovement.MassPriceInventoryChange           += lineItem.TotalMovement.MassPriceInventoryChange;

				// Add per-line tolerance calcs
				finalLineItem.ToleranceTestedQuantity.GrossInventoryChange += lineItem.ToleranceTestedQuantity.GrossInventoryChange;
				finalLineItem.ToleranceTestedQuantity.NetInventoryChange += lineItem.ToleranceTestedQuantity.NetInventoryChange;
				finalLineItem.ToleranceTestedQuantity.MassInventoryChange += lineItem.ToleranceTestedQuantity.MassInventoryChange;
				finalLineItem.Tolerance = Math.Max(finalLineItem.Tolerance,lineItem.Tolerance);
				finalLineItem.AllowableGainLoss.GrossInventoryChange = Math.Abs(finalLineItem.ToleranceTestedQuantity.GrossInventoryChange * finalLineItem.Tolerance / 100.0);
				finalLineItem.AllowableGainLoss.NetInventoryChange = Math.Abs(finalLineItem.ToleranceTestedQuantity.NetInventoryChange * finalLineItem.Tolerance / 100.0);
				finalLineItem.AllowableGainLoss.MassInventoryChange = Math.Abs(finalLineItem.ToleranceTestedQuantity.MassInventoryChange * finalLineItem.Tolerance / 100.0);

				try
				{
					finalLineItem.VariancePercentage.GrossInventoryChange = finalLineItem.Variance.GrossInventoryChange * 100.0 / finalLineItem.ToleranceTestedQuantity.GrossInventoryChange;
					if(Double.IsNaN(finalLineItem.VariancePercentage.GrossInventoryChange) == true ||
					Double.IsInfinity(finalLineItem.VariancePercentage.GrossInventoryChange) == true)
					{
						finalLineItem.VariancePercentage.GrossInventoryChange = 0.0;
					}
				}
				catch(DivideByZeroException)
				{
					finalLineItem.VariancePercentage.GrossInventoryChange = 0.0;
				}
				try
				{
					finalLineItem.VariancePercentage.NetInventoryChange = finalLineItem.Variance.NetInventoryChange * 100.0 / finalLineItem.ToleranceTestedQuantity.NetInventoryChange;
					if(Double.IsNaN(finalLineItem.VariancePercentage.NetInventoryChange) == true ||
					Double.IsInfinity(finalLineItem.VariancePercentage.NetInventoryChange) == true)
					{
						finalLineItem.VariancePercentage.NetInventoryChange = 0.0;
					}
				}
				catch(DivideByZeroException)
				{
					finalLineItem.VariancePercentage.NetInventoryChange = 0.0;
				}
				try
				{
					finalLineItem.VariancePercentage.MassInventoryChange = finalLineItem.Variance.NetInventoryChange * 100.0 / finalLineItem.ToleranceTestedQuantity.NetInventoryChange;
					if(Double.IsNaN(finalLineItem.VariancePercentage.MassInventoryChange) == true ||
					Double.IsInfinity(finalLineItem.VariancePercentage.MassInventoryChange) == true)
					{
						finalLineItem.VariancePercentage.MassInventoryChange = 0.0;
					}
				}
				catch(DivideByZeroException)
				{
					finalLineItem.VariancePercentage.MassInventoryChange = 0.0;
				}

				foreach(string alias in finalLineItem.QuantityList.Keys)
				{
					QuantityDO finalQuantity = (QuantityDO)finalLineItem.QuantityList[alias];
					QuantityDO quantity      = (QuantityDO)lineItem.QuantityList[alias];

					finalQuantity.GrossInventoryChange      += quantity.GrossInventoryChange;
					finalQuantity.NetInventoryChange        += quantity.NetInventoryChange;
					finalQuantity.MassInventoryChange       += quantity.MassInventoryChange;
					finalQuantity.PackageInventoryChange    += quantity.PackageInventoryChange;
					finalQuantity.GrossPriceInventoryChange += quantity.GrossPriceInventoryChange;
					finalQuantity.NetPriceInventoryChange   += quantity.NetPriceInventoryChange;
					finalQuantity.MassPriceInventoryChange  += quantity.MassPriceInventoryChange;
					finalQuantity.Number01Change            += quantity.Number01Change;
					finalQuantity.Number02Change            += quantity.Number02Change;
					finalQuantity.Number03Change            += quantity.Number03Change;
					finalQuantity.Number04Change            += quantity.Number04Change;
					finalQuantity.Number05Change            += quantity.Number05Change;
					finalQuantity.Number06Change            += quantity.Number06Change;

					// Combine the monikers. It will ensure that the monikers
					// are not duplicated.
					finalQuantity.CombineMonikers(quantity.Moniker);

					if(quantity.Gross != 0)
					{
						// Do not total the physical inventory aliases
						if(aliasNameList.Contains(alias) == false)
						{
							this.AddToTotal(totalLineItem,alias,quantity);
						}
					}

					// Set the transaction error flag if the volume is associated to an aggregate
					// column and the transaction error flag was set in the volume.
					if((quantity.IsAggregateQuantity == true) && (quantity.TransErrorFlag == true))
					{
						lineItem.SetCellFlag(alias,BaseInventoryLineItemDO.Status.TRANS_ERROR_FLAG);
					}
				}

				statusCombiner.CombineLedgerLineItemStatusFlags(lineItem,finalLineItem);
			}

			statusCombiner.ResetRuleList();
			currentDate = currentDate.AddDays(1);
		}

		finalLedger.Add(totalLineItem);
		return finalLedger;
	}

	/// <summary>
	/// This method will add all the total columns totals.
	/// </summary>
	/// <param name="totalLineItem"></param>
	/// <param name="key"></param>
	/// <param name="volume"></param>
	protected void AddToTotal(InventoryLineItemDO totalLineItem,string key,QuantityDO quantity)
	{
		QuantityDO totalQuantity = (QuantityDO)totalLineItem.QuantityList[key];

		totalQuantity.GrossInventoryChange      += quantity.GrossInventoryChange;
		totalQuantity.NetInventoryChange        += quantity.NetInventoryChange;
		totalQuantity.MassInventoryChange       += quantity.MassInventoryChange;
		totalQuantity.GrossPriceInventoryChange += quantity.GrossPriceInventoryChange;
		totalQuantity.NetPriceInventoryChange   += quantity.NetPriceInventoryChange;
		totalQuantity.MassPriceInventoryChange  += quantity.MassPriceInventoryChange;
		totalQuantity.Number01Change            += quantity.Number01Change;
		totalQuantity.Number02Change            += quantity.Number02Change;
		totalQuantity.Number03Change            += quantity.Number03Change;
		totalQuantity.Number04Change            += quantity.Number04Change;
		totalQuantity.Number05Change            += quantity.Number05Change;
		totalQuantity.Number06Change            += quantity.Number06Change;
	}

	/// <summary>
	/// This method will retrieve all the physical inventory alias names. It will load up the
	/// hash table that was passed in with the list.
	/// </summary>
	/// <param name="aliasNameHshTbl"></param>
	protected void GetPhysicalInvAliasNames(Hashtable aliasNameHshTbl)
	{
		string aliasName = "";
		TransactionAliases.TransactionTypes type = TransactionAliases.TransactionTypes.T_Maximum;

		if((this.aliasTypeList != null) && (this.aliasTypeList.Count > 0))
		{
			IDictionaryEnumerator enumerator = this.aliasTypeList.GetEnumerator();
			while(enumerator.MoveNext() == true)
			{
				aliasName = (string)enumerator.Key;
				type = (TransactionAliases.TransactionTypes)enumerator.Value;

				if(type == TransactionAliases.TransactionTypes.T14_PhysicalInventory)
				{
					if(aliasNameHshTbl.Contains(aliasName) == false)
					{
						aliasNameHshTbl.Add(aliasName,type);
					}
				}
			}
		}
	}

	/// <summary>
	/// This method will convert a date string to a DateTime object.
	/// </summary>
	/// <param name="dateStr"></param>
	/// <returns></returns>
	protected DateTime ConvertMonthDayYearToDateTime(string dateStr)
	{
		int month    = 1;
		int day      = 1;
		int year     = 1;
		int index1   = -1;
		int index2   = -1;
		string slash = "/";
		string dash  = "-";
		string searchChar;
		DateTime dateTime;

		if((dateStr != null) && (dateStr.Length > 0))
		{
			if(dateStr.IndexOf(slash) < 0)
			{
				searchChar = dash;
			}
			else
			{
				searchChar = slash;
			}

			index1 = dateStr.IndexOf(searchChar);
			index2 = dateStr.IndexOf(searchChar,index1 + 1);
		}

		if((index1 >= 0) && (index2 > index1))
		{
			month = int.Parse(dateStr.Substring(0,index1));
			day   = int.Parse(dateStr.Substring((index1 + 1),(index2 - index1 - 1)));
			year  = int.Parse(dateStr.Substring((index2 + 1),(dateStr.Length - index2 - 1)));

			if(year < 50)
			{
				//map 0, 1, ..., 48, 49 to 2000, 2001, ..., 2048, 2049
				year += 2000;
			}
			else if(year < 100)
			{
				//map 50, 51, ..., 98, 99 to 1950, 1951, ..., 1998, 1999
				year += 1900;
			}
		}

		try
		{
			dateTime = new DateTime(year,month,day);
		}
		catch(Exception)
		{
			dateTime = new System.DateTime(1,1,1);
		}

		return dateTime;
	}

	/// <summary>
	/// This method will convert the date to the following format:
	/// mm/dd/yyyy.
	/// </summary>
	protected string ConvertToMonthDayYear(DateTime dateTime)
	{
		int month = dateTime.Month;
		int day   = dateTime.Day;
		int year  = dateTime.Year;

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

		if(number < 10)
			zeroFillNumber = "0" + zeroFillNumber;

		return zeroFillNumber;
	}
	#endregion
}