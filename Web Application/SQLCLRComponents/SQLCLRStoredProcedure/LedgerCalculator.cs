/// <summary>
///   File name:	LedgerCalculator.cs
///   Purpose:	   The purpose of this class is to calculate the beginning book, ending
///				   book, and total physical inventories.
///				
///   Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA,
///				   2000.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Endress+Hauser.
///				
/// 
///	Author(s):	Richard Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///   2009-05-07     Richard Panachida    Defect 3592: Variance should be calculate even though physical inventory has
///                                       a value of zero. Updated the code to include physicals of value zero.
///
/// </summary>

using System;
using System.Collections;
using System.Data.SqlClient;
using System.Globalization;

public class LedgerCalculator
{
	#region Constants and Fields

	protected TransactionAliasListDO aliasList;

	protected CustomMathFunctionDO customMathFunctionDO;

	protected CustomMovementFunctionDO customMovementFunctionDO;

	protected CLRLedgerProcessor.SystemEditions systemEdition;

	protected bool usePreviousPhysicalInventory;

	#endregion

	#region Constructors and Destructors

	/// <summary>
	///     This is the default constructor for the Ledger Calculator class.
	/// </summary>
	/// <param name="aliasList"></param>
	/// <param name="usePreviousPhysicalInventory"></param>
	public LedgerCalculator(
		TransactionAliasListDO aliasList,
		bool usePreviousPhysicalInventory,
		CLRLedgerProcessor.SystemEditions systemEdition)
	{
		this.aliasList = aliasList;
		this.usePreviousPhysicalInventory = usePreviousPhysicalInventory;
		this.customMathFunctionDO = new CustomMathFunctionDO();
		this.customMovementFunctionDO = new CustomMovementFunctionDO();
	}

	#endregion

	#region Enums

	protected enum WhichDailyWAC
	{
		CurrentDay,

		PreviousDay
	};

	#endregion

	#region Public Methods and Operators

	/// <summary>
	///     This method will orchestrate the calculation for begin, book, total physical, variance,
	///     and total variance inventories.
	/// </summary>
	/// <param name="ledger"></param>
	/// <param name="initialBeginInventory"></param>
	public void CalculateLedger(
		SqlConnection a_connection,
		LedgerLineItemCollection ledger,
		QuantityDO initialBeginInventory)
	{
		InventoryLineItemDO previousLineItem = null;

		// The the product information which includes the conversion factor and
		// precision. If the product is configured with the conversion factor, then
		// class factor and precision members will be set use the product settings.
		// Otherwise the site's settings will be used.
		ProductDO productDO = this.RetrieveProductInfo(a_connection, ledger.ProductIndex);

		for (int iRow = 0; iRow < ledger.Count; ++iRow)
		{
			var lineItem = (InventoryLineItemDO)ledger[iRow];

			// Get the most current WAC value for the previous inventory date, site, and product combination.
			double previousDailyWacValue = this.GetCurrentWAC(
				a_connection,
				lineItem.InventoryDate,
				ledger.SiteIndex,
				ledger.ProductIndex,
				WhichDailyWAC.PreviousDay);

			// Get the most current WAC value for the inventory date, site, and product combination.
			double dailyWacValue = this.GetCurrentWAC(
				a_connection,
				lineItem.InventoryDate,
				ledger.SiteIndex,
				ledger.ProductIndex,
				WhichDailyWAC.CurrentDay);

			this.CalculateBeginInventory(lineItem, previousLineItem, initialBeginInventory, previousDailyWacValue);
			this.CalculateBookInventory(lineItem, dailyWacValue);
			this.CalculateTotalPhysicalInventory(lineItem, dailyWacValue);
			this.CalculateVariance(lineItem, dailyWacValue);
			this.CalculateTotalVariance(lineItem, previousLineItem, dailyWacValue);
			this.CalculateTotalMovement(lineItem, previousLineItem, ledger.ProductIndex, ledger.TankIndex, a_connection);
			this.CalculateToleranceTestedVolume(lineItem, initialBeginInventory, productDO);
			this.CalculateAggregateColumns(a_connection, lineItem);
			this.CalculateTolerance(lineItem, initialBeginInventory, productDO);
			this.CalculateVariancePercentage(lineItem);
			this.CalculateAllowableGainLoss(lineItem);
			this.CalculateOutOfToleranceFlags(lineItem);

			previousLineItem = lineItem;
		}
	}

	#endregion

	#region Methods

	/// <summary>
	///     This method will perform the horizontal math aggregating alias into the aggregate
	///     columns.
	/// </summary>
	/// <param name="lineItem"></param>
	protected void CalculateAggregateColumns(SqlConnection a_connection, InventoryLineItemDO lineItem)
	{
		IDictionaryEnumerator transAliasEnumerator = this.aliasList.aliasList.GetEnumerator();

		while (transAliasEnumerator.MoveNext() == true)
		{
			var transAliasDO = transAliasEnumerator.Value as TransactionAliasDO;

			if ((transAliasDO != null) && (transAliasDO.IsAggregateAlias == true))
			{
				string aggregateColumnName = transAliasDO.AliasName;
				string customFunctionName = transAliasDO.CustomFunctionName;
				string customParameterXML = "";
				var aggregateQuantityDO = lineItem.QuantityList[aggregateColumnName] as QuantityDO;

				for (int nextAlias = 0; nextAlias < transAliasDO.AliasesToAggregate.Count; nextAlias++)
				{
					var aliasName = transAliasDO.AliasesToAggregate[nextAlias] as string;
					var moniker = transAliasDO.AliasesToAggregateSymbols[nextAlias] as string;
					var quantityDO = lineItem.QuantityList[aliasName] as QuantityDO;

					if ((quantityDO != null) && (aggregateQuantityDO != null))
					{
						// Only add a moniker if the volume (alias to aggregate) was created by having a 
						// transaction. Otherwise, it was a ledger filler volume.
						if (quantityDO.IsFillerQuantity == false)
						{
							aggregateQuantityDO.AppendMoniker(moniker);
							aggregateQuantityDO.OrErrorFlag(quantityDO.TransErrorFlag);
							aggregateQuantityDO.IsAggregateQuantity = true;
						}

						// Perform default volume aggregation if there is no custom function name.
						if ((customFunctionName == null) || (customFunctionName.Length <= 0))
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

				// Perform custom calculation using a custom SQL function and update the aggregate
				// column volume data object.
				if ((customFunctionName != null) && (customFunctionName.Length > 0))
				{
					this.customMathFunctionDO.ExecuteCustomFunction(a_connection, customFunctionName, customParameterXML);

					if (this.customMathFunctionDO.Quantity != null)
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
					if (aggregateQuantityDO.IsAggregateQuantity
					    && (aggregateQuantityDO.GrossInventoryChange == 0.0 || aggregateQuantityDO.NetInventoryChange == 0.0))
					{
						lineItem.SetCellFlag(aggregateColumnName, BaseInventoryLineItemDO.Status.TRANS_WITH_ZERO_QUANTITY);
					}

					if (aggregateQuantityDO.TransErrorFlag == true)
					{
						lineItem.SetCellFlag(aggregateColumnName, BaseInventoryLineItemDO.Status.TRANS_ERROR_FLAG);
					}
				}
			}
		}
	}

	protected void CalculateAllowableGainLoss(InventoryLineItemDO lineItem)
	{
		lineItem.AllowableGainLoss.GrossInventoryChange = lineItem.ToleranceTestedQuantity.GrossInventoryChange
		                                                  * lineItem.Tolerance / 100.0;
		lineItem.AllowableGainLoss.NetInventoryChange = lineItem.ToleranceTestedQuantity.NetInventoryChange
		                                                * lineItem.Tolerance / 100.0;
		lineItem.AllowableGainLoss.MassInventoryChange = lineItem.ToleranceTestedQuantity.MassInventoryChange
		                                                 * lineItem.Tolerance / 100.0;
	}

	/// <summary>
	///     This method calculates the beginning book inventory. For a multi-owner system, the
	///     beginning book inventory is always the previous day's ending book inventory. If the
	///     system is a single owner system, then the beginning book is either the physical
	///     inventory of the previous day if it exists or the ending book of the previous day.
	///     This is the same for pricing.
	/// </summary>
	/// <param name="lineItem"></param>
	/// <param name="previousLineItem"></param>
	/// <param name="initialBeginInventory"></param>
	protected void CalculateBeginInventory(
		InventoryLineItemDO lineItem,
		InventoryLineItemDO previousLineItem,
		QuantityDO initialBeginInventory,
		double dailyWacValue)
	{
		// If calculating the first item for a Single Owner system, it comes from a Closeout Record or 
		// a Previous Physical Inventory.
		if (previousLineItem == null)
		{
			var beginInventory = (QuantityDO)lineItem.BeginInventory;
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
		if (this.usePreviousPhysicalInventory == true)
		{
			if (previousLineItem.HasPhysicalInventory == true)
			{
				foreach (string aliasName in previousLineItem.QuantityList.Keys)
				{
					TransactionAliasDO alias = this.aliasList[aliasName];

					if ((alias != null) && (this.aliasList.IsPhysicalInventory(aliasName)))
					{
						lineItem.BeginInventory.GrossInventoryChange +=
							((QuantityDO)previousLineItem.QuantityList[aliasName]).GrossInventoryChange;
						lineItem.BeginInventory.NetInventoryChange +=
							((QuantityDO)previousLineItem.QuantityList[aliasName]).NetInventoryChange;
						lineItem.BeginInventory.MassInventoryChange +=
							((QuantityDO)previousLineItem.QuantityList[aliasName]).MassInventoryChange;
						lineItem.BeginInventory.PackageInventoryChange +=
							((QuantityDO)previousLineItem.QuantityList[aliasName]).PackageInventoryChange;

						lineItem.BeginInventory.GrossPriceInventoryChange +=
							(((QuantityDO)previousLineItem.QuantityList[aliasName]).GrossInventoryChange * dailyWacValue);
						lineItem.BeginInventory.NetPriceInventoryChange +=
							(((QuantityDO)previousLineItem.QuantityList[aliasName]).NetInventoryChange * dailyWacValue);
						lineItem.BeginInventory.MassPriceInventoryChange +=
							(((QuantityDO)previousLineItem.QuantityList[aliasName]).MassInventoryChange * dailyWacValue);
					}
				}
			}
			else
			{
				lineItem.BeginInventory.GrossInventoryChange = previousLineItem.BookInventory.GrossInventoryChange;
				lineItem.BeginInventory.NetInventoryChange = previousLineItem.BookInventory.NetInventoryChange;
				lineItem.BeginInventory.MassInventoryChange = previousLineItem.BookInventory.MassInventoryChange;
				lineItem.BeginInventory.PackageInventoryChange = previousLineItem.BookInventory.PackageInventoryChange;
				lineItem.BeginInventory.GrossPriceInventoryChange = previousLineItem.BookInventory.GrossInventoryChange
				                                                    * dailyWacValue;
				lineItem.BeginInventory.NetPriceInventoryChange = previousLineItem.BookInventory.NetInventoryChange * dailyWacValue;
				lineItem.BeginInventory.MassPriceInventoryChange = previousLineItem.BookInventory.MassInventoryChange
				                                                   * dailyWacValue;
			}

			return;
		}

		lineItem.BeginInventory.GrossInventoryChange = previousLineItem.BookInventory.GrossInventoryChange;
		lineItem.BeginInventory.NetInventoryChange = previousLineItem.BookInventory.NetInventoryChange;
		lineItem.BeginInventory.MassInventoryChange = previousLineItem.BookInventory.MassInventoryChange;
		lineItem.BeginInventory.PackageInventoryChange = previousLineItem.BookInventory.PackageInventoryChange;
		lineItem.BeginInventory.GrossPriceInventoryChange = previousLineItem.BookInventory.GrossInventoryChange
		                                                    * dailyWacValue;
		lineItem.BeginInventory.NetPriceInventoryChange = previousLineItem.BookInventory.NetInventoryChange * dailyWacValue;
		lineItem.BeginInventory.MassPriceInventoryChange = previousLineItem.BookInventory.MassInventoryChange * dailyWacValue;
	}

	/// <summary>
	///     This method will calculate the book inventory for a given day.
	/// </summary>
	/// <param name="lineItem"></param>
	protected void CalculateBookInventory(InventoryLineItemDO lineItem, double dailyWacValue)
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
			var quantity = (QuantityDO)lineItem.QuantityList[aliasName];

			if (this.aliasList.AffectsInventory(aliasName))
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

		// Calculate price based on average unit price
		lineItem.BookInventory.GrossPriceInventoryChange = lineItem.BookInventory.GrossInventoryChange * dailyWacValue;
		lineItem.BookInventory.NetPriceInventoryChange = lineItem.BookInventory.NetInventoryChange * dailyWacValue;
		lineItem.BookInventory.MassPriceInventoryChange = lineItem.BookInventory.MassInventoryChange * dailyWacValue;
		lineItem.TotalActivity.GrossPriceInventoryChange = lineItem.TotalActivity.GrossInventoryChange * dailyWacValue;
		lineItem.TotalActivity.NetPriceInventoryChange = lineItem.TotalActivity.NetInventoryChange * dailyWacValue;
		lineItem.TotalActivity.MassPriceInventoryChange = lineItem.TotalActivity.MassInventoryChange * dailyWacValue;
	}

	protected void CalculateOutOfToleranceFlags(InventoryLineItemDO lineItem)
	{
		if (lineItem.VariancePercentage.Gross > lineItem.Tolerance)
		{
			; // Moved somewhere?  Stubbed in MOD as well
		}
	}

	protected void CalculateTolerance(InventoryLineItemDO lineItem, QuantityDO initialInventory, ProductDO productDO)
	{
		if (this.systemEdition == CLRLedgerProcessor.SystemEditions.MOD)
		{
			if (productDO.AviationProduct == true)
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

	protected void CalculateToleranceTestedVolume(
		InventoryLineItemDO lineItem,
		QuantityDO initialInventory,
		ProductDO productDO)
	{
		if (this.systemEdition == CLRLedgerProcessor.SystemEditions.MOD)
		{
			if (productDO.AviationProduct == true)
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
				lineItem.ToleranceTestedQuantity.GrossInventoryChange = initialInventory.GrossInventoryChange
				                                                        + lineItem.TotalMovement.GrossInventoryChange;
				lineItem.ToleranceTestedQuantity.NetInventoryChange = initialInventory.NetInventoryChange
				                                                      + lineItem.TotalMovement.NetInventoryChange;
			}
		}
		else
		{
			lineItem.ToleranceTestedQuantity.GrossInventoryChange = lineItem.TotalMovement.GrossInventoryChange;
			lineItem.ToleranceTestedQuantity.NetInventoryChange = lineItem.TotalMovement.NetInventoryChange;
			lineItem.ToleranceTestedQuantity.MassInventoryChange = lineItem.TotalMovement.MassInventoryChange;
		}
	}

	/// <summary>
	///     This method will calculate the total movement (issues) inventories.  This is used to calculate
	///     the variance percentage.
	/// </summary>
	/// <param name="lineItem"></param>
	protected void CalculateTotalMovement(
		InventoryLineItemDO lineItem,
		InventoryLineItemDO previousLineItem,
		int productIndex,
		int tankIndex,
		SqlConnection connection)
	{
		string customMovementFunctionName = "usb_CustomLedgerMovement";
		string customParameterXML = "";
		foreach (string aliasName in lineItem.QuantityList.Keys)
		{
			if (string.Empty != aliasName)
			{
				// Build an XML string that will be passed to the custom SQL function.
				// This string will contain all the alias names, gross and net volumes.
				var quantityDO = lineItem.QuantityList[aliasName] as QuantityDO;
				customParameterXML = this.BuildMovementXMLString(aliasName, quantityDO, productIndex, tankIndex, customParameterXML);
			}
		}

		if ((customMovementFunctionName != null) && (customMovementFunctionName.Length > 0))
		{
			// Perform custom movement calculations using a custom SQL function and update the total
			// movement in the lineitem object. (IGO 26-Aug-2010)
			this.customMovementFunctionDO.ExecuteCustomFunction(customMovementFunctionName, customParameterXML, connection);
			if (this.customMovementFunctionDO.Quantity != null)
			{
				lineItem.TotalMovement.GrossInventoryChange += this.customMovementFunctionDO.Quantity.GrossInventoryChange;
				lineItem.TotalMovement.NetInventoryChange += this.customMovementFunctionDO.Quantity.NetInventoryChange;
				lineItem.TotalMovement.MassInventoryChange += this.customMovementFunctionDO.Quantity.MassInventoryChange;
			}
			lineItem.Tolerance = this.customMovementFunctionDO.Tolerance;
		}

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
	///     This method will calculate the total physical inventory if there is more than one physical inventory.
	/// </summary>
	/// <param name="lineItem"></param>
	protected void CalculateTotalPhysicalInventory(InventoryLineItemDO lineItem, double dailyWacValue)
	{
		foreach (string aliasName in lineItem.QuantityList.Keys)
		{
			var alias = this.aliasList.aliasList[aliasName] as TransactionAliasDO;

			if ((alias != null) && (alias.TransactionTypeID == TransactionAliases.TransactionTypes.T14_PhysicalInventory))
			{
				var quantity = lineItem.QuantityList[aliasName] as QuantityDO;

				// Recalculate the physical inventory price using the correct AUP.
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

	/// <summary>
	///     This method will calculate the total variance. This variance is the running
	///     total.
	/// </summary>
	/// <param name="lineItem"></param>
	protected void CalculateTotalVariance(
		InventoryLineItemDO lineItem,
		InventoryLineItemDO previousLineItem,
		double dailyWacValue)
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
			lineItem.TotalVariance.GrossInventoryChange += (previousLineItem.TotalVariance.GrossInventoryChange
			                                                + lineItem.Variance.GrossInventoryChange);
			lineItem.TotalVariance.NetInventoryChange += (previousLineItem.TotalVariance.NetInventoryChange
			                                              + lineItem.Variance.NetInventoryChange);
			lineItem.TotalVariance.MassInventoryChange += (previousLineItem.TotalVariance.MassInventoryChange
			                                               + lineItem.Variance.MassInventoryChange);
			lineItem.TotalVariance.PackageInventoryChange += (previousLineItem.TotalVariance.PackageInventoryChange
			                                                  + lineItem.Variance.PackageInventoryChange);

			// Updated to calculate price based on average unit price
			lineItem.TotalVariance.GrossPriceInventoryChange += (previousLineItem.TotalVariance.GrossPriceInventoryChange
			                                                     + (lineItem.Variance.GrossInventoryChange * dailyWacValue));
			lineItem.TotalVariance.NetPriceInventoryChange += (previousLineItem.TotalVariance.NetPriceInventoryChange
			                                                   + (lineItem.Variance.NetInventoryChange * dailyWacValue));
			lineItem.TotalVariance.MassPriceInventoryChange += (previousLineItem.TotalVariance.MassPriceInventoryChange
			                                                    + (lineItem.Variance.MassInventoryChange * dailyWacValue));
		}
	}

	/// <summary>
	///     This method will calculate the variance for a given day. The variance is based on the
	///     difference of the book and physical inventories.
	/// </summary>
	/// <param name="lineItem"></param>
	protected void CalculateVariance(InventoryLineItemDO lineItem, double dailyWacValue)
	{
		var dailyPhysical = new QuantityDO(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
		bool physicalPresent = false;

		foreach (string aliasName in lineItem.QuantityList.Keys)
		{
			var alias = this.aliasList.aliasList[aliasName] as TransactionAliasDO;

			if ((alias != null) && (alias.TransactionTypeID == TransactionAliases.TransactionTypes.T14_PhysicalInventory))
			{
				var quantity = lineItem.QuantityList[aliasName] as QuantityDO;

				dailyPhysical.GrossInventoryChange += quantity.GrossInventoryChange;
				dailyPhysical.NetInventoryChange += quantity.NetInventoryChange;
				dailyPhysical.MassInventoryChange += quantity.MassInventoryChange;
				dailyPhysical.PackageInventoryChange += quantity.PackageInventoryChange;

				// Updated to calculate price using average unit price
				dailyPhysical.GrossPriceInventoryChange += quantity.GrossInventoryChange * dailyWacValue;
				dailyPhysical.NetPriceInventoryChange += quantity.NetInventoryChange * dailyWacValue;
				dailyPhysical.MassPriceInventoryChange += quantity.MassInventoryChange * dailyWacValue;

				if (lineItem.HasPhysicalInventory == true)
				{
					physicalPresent = true;
				}
			}
		}

		// Set the variance to zero if there is not a physical inventory for the current day.
		if (physicalPresent == true)
		{
			lineItem.Variance.GrossInventoryChange = dailyPhysical.GrossInventoryChange
			                                         - lineItem.BookInventory.GrossInventoryChange;
			lineItem.Variance.NetInventoryChange = dailyPhysical.NetInventoryChange - lineItem.BookInventory.NetInventoryChange;
			lineItem.Variance.MassInventoryChange = dailyPhysical.MassInventoryChange
			                                        - lineItem.BookInventory.MassInventoryChange;
			lineItem.Variance.PackageInventoryChange = dailyPhysical.PackageInventoryChange
			                                           - lineItem.BookInventory.PackageInventoryChange;

			// Updated to calculate price using average unit price
			lineItem.Variance.GrossPriceInventoryChange = dailyPhysical.GrossPriceInventoryChange
			                                              - (lineItem.BookInventory.GrossInventoryChange * dailyWacValue);
			lineItem.Variance.NetPriceInventoryChange = dailyPhysical.NetPriceInventoryChange
			                                            - (lineItem.BookInventory.NetInventoryChange * dailyWacValue);
			lineItem.Variance.MassPriceInventoryChange = dailyPhysical.MassPriceInventoryChange
			                                             - (lineItem.BookInventory.MassInventoryChange * dailyWacValue);
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

	protected void CalculateVariancePercentage(InventoryLineItemDO lineItem)
	{
		lineItem.VariancePercentage.GrossInventoryChange = lineItem.TotalVariance.GrossInventoryChange * 100.0
		                                                   / lineItem.ToleranceTestedQuantity.GrossInventoryChange;
		lineItem.VariancePercentage.NetInventoryChange = lineItem.TotalVariance.NetInventoryChange * 100.0
		                                                 / lineItem.ToleranceTestedQuantity.NetInventoryChange;
		lineItem.VariancePercentage.MassInventoryChange = lineItem.TotalVariance.MassInventoryChange * 100.0
		                                                  / lineItem.ToleranceTestedQuantity.MassInventoryChange;
	}

	/// <summary>
	///     This method will return the most recent WAC based on the inventory date,
	///     product, and site.
	/// </summary>
	/// <param name="inventoryDate"></param>
	/// <param name="siteIndex"></param>
	/// <param name="productIndex"></param>
	/// <returns></returns>
	protected double GetCurrentWAC(
		SqlConnection a_connection,
		string inventoryDateStr,
		int siteIndex,
		int productIndex,
		WhichDailyWAC whichDailyWAC)
	{
		double wacValue = 0;
		DateTime inventoryDate = this.ParseDate(inventoryDateStr);
		var wacDO = new WeightAverageCostDO();

		if (whichDailyWAC == WhichDailyWAC.PreviousDay)
		{
			var minusOneDay = new TimeSpan(-1, 0, 0, 0);

			inventoryDate = inventoryDate.Add(minusOneDay);
		}

		wacDO.PerformWACQuery(a_connection, siteIndex, productIndex, inventoryDate);

		if (wacDO != null)
		{
			wacValue = wacDO.WacValue;
		}

		return wacValue;
	}

	/// <summary>
	///     This method will build an XML string that contains the alias name and gross/net.
	///     The purpose is to pass to a custom SQL function to perform special movement
	///     calculations.
	/// </summary>
	/// <param name="aliasName"></param>
	/// <param name="volumeDO"></param>
	/// <param name="xmlString"></param>
	/// <returns></returns>
	private string BuildMovementXMLString(
		string aliasName,
		QuantityDO quantityDO,
		int productIndex,
		int tankIndex,
		string xmlString)
	{
		string temp = "<Alias>";
		temp += "<name>" + aliasName + "</name>";
		temp += "<g>" + quantityDO.GrossInventoryChange.ToString(CultureInfo.InvariantCulture) + "</g>";
		temp += "<nt>" + quantityDO.NetInventoryChange.ToString(CultureInfo.InvariantCulture) + "</nt>";
		temp += "<m>" + quantityDO.MassInventoryChange.ToString(CultureInfo.InvariantCulture) + "</m>";
		temp += "<tankIndex>" + tankIndex + "</tankIndex>";
		temp += "<productIndex>" + productIndex + "</productIndex>";
		temp += "</Alias>";

		xmlString += temp;
		return xmlString;
	}

	/// <summary>
	///     This method will build an XML string that contains the alias name and all the
	///     volumes (gross, net, number01 ... number06). The purpose is to pass to a custom
	///     SQL function to perform special math.
	/// </summary>
	/// <param name="aliasName"></param>
	/// <param name="volumeDO"></param>
	/// <param name="xmlString"></param>
	/// <returns></returns>
	private string BuildXMLString(string aliasName, QuantityDO quantityDO, string xmlString)
	{
		string newAliasName = aliasName.Replace(" ", "");

		string temp = "<" + newAliasName + ">";

		temp += "<g>" + quantityDO.GrossInventoryChange.ToString() + "</g>";
		temp += "<nt>" + quantityDO.NetInventoryChange.ToString() + "</nt>";
		temp += "<nt>" + quantityDO.MassInventoryChange.ToString() + "</nt>";
		temp += "<gp>" + quantityDO.GrossPriceInventoryChange.ToString() + "</gp>";
		temp += "<ntp>" + quantityDO.NetPriceInventoryChange.ToString() + "</ntp>";
		temp += "<ntp>" + quantityDO.MassPriceInventoryChange.ToString() + "</ntp>";
		temp += "<n1>" + quantityDO.Number01Change.ToString() + "</n1>";
		temp += "<n2>" + quantityDO.Number02Change.ToString() + "</n2>";
		temp += "<n3>" + quantityDO.Number03Change.ToString() + "</n3>";
		temp += "<n4>" + quantityDO.Number04Change.ToString() + "</n4>";
		temp += "<n5>" + quantityDO.Number05Change.ToString() + "</n5>";
		temp += "<n6>" + quantityDO.Number06Change.ToString() + "</n6>";

		temp += "</" + newAliasName + ">";

		xmlString += temp;
		return xmlString;
	}

	/// <summary>
	///     This method will return a DateTime object representing the inventory
	///     date.
	/// </summary>
	/// <param name="inventoryDate"></param>
	/// <returns></returns>
	private DateTime ParseDate(string inventoryDate)
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

				var invDate = new DateTime(year, month, day);
				return invDate;
			}
			catch (Exception)
			{
				return DateTime.Now;
			}
		}
		else
		{
			return DateTime.Now;
		}
	}

	/// <summary>
	///     This method will return the current site information.
	/// </summary>
	/// <returns></returns>
	private ProductDO RetrieveProductInfo(SqlConnection a_connection, int productIndex)
	{
		// Use the product conversion factor and precision if the product is configured to
		// have them.
		var productDO = new ProductDO();
		productDO.ProductIndex = productIndex;
		productDO.RetrieveProductInfo(a_connection);

		return productDO;
	}

	#endregion
}