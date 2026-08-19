// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedgerStatusCombiner.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System.Collections;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	public class LedgerStatusCombiner
	{
		#region Constants and Fields

		protected bool bResetRuleList;

		protected Hashtable ruleList;

		#endregion

		#region Constructors and Destructors

		public LedgerStatusCombiner()
		{
			this.bResetRuleList = true;
		}

		#endregion

		#region Enums

		public enum CombineRule
		{
			ANY, 

			ALL, 

			NONE
		}

		#endregion

		#region Public Methods and Operators

		public void CombineLedgerLineItemStatusFlags(LedgerLineItemDO lineItem, LedgerLineItemDO finalLineItem)
		{
			this.CombineLineItemFlag(finalLineItem, lineItem, BaseLineItemDO.Status.NA);
			this.CombineLineItemFlag(finalLineItem, lineItem, BaseLineItemDO.Status.PHYS_INV_EXISTS);
			this.CombineLineItemFlag(finalLineItem, lineItem, BaseLineItemDO.Status.SUPPRESS_LINK);
			this.CombineLineItemFlag(finalLineItem, lineItem, BaseLineItemDO.Status.SUPPRESS);
			this.CombineLineItemFlag(finalLineItem, lineItem, BaseLineItemDO.Status.OUT_OF_TOLERANCE_GROSS);
			this.CombineLineItemFlag(finalLineItem, lineItem, BaseLineItemDO.Status.OUT_OF_TOLERANCE_NET);
			this.CombineLineItemFlag(finalLineItem, lineItem, BaseLineItemDO.Status.INV_ERROR);
			this.CombineLineItemFlag(finalLineItem, lineItem, BaseLineItemDO.Status.CLOSED_OUT);
			this.CombineLineItemCellFlags(finalLineItem, lineItem);
		}

		public void ResetRuleList()
		{
			this.ruleList = new Hashtable();
			this.bResetRuleList = false;
		}

		public void SetCombineRule(LedgerLineItemDO finalLineItem, BaseLineItemDO.Status status, CombineRule rule)
		{
			this.ruleList.Add(status, rule);

			if (rule == CombineRule.ALL)
			{
				// line item level status
				finalLineItem.Flags += status;

				// cell level status
				finalLineItem.SetCellFlag("Inventory Date", status);
				finalLineItem.SetCellFlag("Begin Inventory", status);
				finalLineItem.SetCellFlag("Book Inventory", status);

				foreach (string cellName in finalLineItem.QuantityList.Keys)
				{
					finalLineItem.SetCellFlag(cellName, status);
				}
			}
		}

		#endregion

		#region Methods

		private void CombineFlagForSingleCell(string cellName, LedgerLineItemDO finalLineItem, LedgerLineItemDO lineItem)
		{
			BaseLineItemDO.StatusFlags flags = lineItem.GetCellFlags()[cellName];

			foreach (BaseLineItemDO.Status status in this.ruleList.Keys)
			{
				var rule = (CombineRule)this.ruleList[status];
				bool bFlagIsSet = (flags != null) && flags.CheckFlag(status);

				if (rule == CombineRule.NONE)
				{
					finalLineItem.ClearCellFlag(cellName, status);
				}
				else if (bFlagIsSet & (rule == CombineRule.ANY))
				{
					finalLineItem.SetCellFlag(cellName, status);
				}
				else if ((bFlagIsSet == false) & (rule == CombineRule.ALL))
				{
					finalLineItem.ClearCellFlag(cellName, status);
				}
			}
		}

		private void CombineLineItemCellFlags(LedgerLineItemDO finalLineItem, LedgerLineItemDO lineItem)
		{
			this.bResetRuleList = true;

			Dictionary<string, BaseLineItemDO.StatusFlags> cellFlags = finalLineItem.GetCellFlags();
			this.CombineFlagForSingleCell("Inventory Date", finalLineItem, lineItem);
			this.CombineFlagForSingleCell("Begin Inventory", finalLineItem, lineItem);
			this.CombineFlagForSingleCell("Book Inventory", finalLineItem, lineItem);

			foreach (string cellName in finalLineItem.QuantityList.Keys)
			{
				this.CombineFlagForSingleCell(cellName, finalLineItem, lineItem);
			}
		}

		private void CombineLineItemFlag(
			LedgerLineItemDO finalLineItem, LedgerLineItemDO lineItem, BaseLineItemDO.Status status)
		{
			this.bResetRuleList = true;

			var rule = (CombineRule)this.ruleList[status];
			bool bFlagIsSet = lineItem.CheckFlag(status);

			if (rule == CombineRule.NONE)
			{
				finalLineItem.Flags -= status;
			}
			else if (bFlagIsSet & (rule == CombineRule.ANY))
			{
				finalLineItem.Flags += status;
			}
			else if ((bFlagIsSet == false) & (rule == CombineRule.ALL))
			{
				finalLineItem.Flags -= status;
			}
		}

		#endregion
	}
}