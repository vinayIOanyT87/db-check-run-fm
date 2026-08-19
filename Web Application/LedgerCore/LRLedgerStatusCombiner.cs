namespace LedgerCore
{
	using System.Collections;

	class LRLedgerStatusCombiner
	{
		#region Public Data members
		public enum CombineRule { Any, All, NONE }
		#endregion

		#region Protected data members
		protected Hashtable ruleList;
		protected bool resetRuleList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Ledger Status Combiner class.
		/// </summary>
		public LRLedgerStatusCombiner()
		{
			this.resetRuleList = true;
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method sets the combination of rules for an inventory line item, since there can be 
		/// numerous rules for a given line item.
		/// </summary>
		/// <param name="finalLineItem"></param>
		/// <param name="status"></param>
		/// <param name="rule"></param>
		public void SetCombineRule(LRInventoryLineItemDO finalLineItem, LRBaseInventoryLineItemDO.Status status, CombineRule rule)
		{
			this.ruleList.Add(status, rule);

			if (rule == CombineRule.All)
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

		/// <summary>
		/// This method will combince the ledger line item status flags.
		/// </summary>
		/// <param name="lineItem"></param>
		/// <param name="finalLineItem"></param>
		public void CombineLedgerLineItemStatusFlags(LRInventoryLineItemDO lineItem, LRInventoryLineItemDO finalLineItem)
		{
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.Na);
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.PhysInvExists);
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.SuppressLink);
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.Suppress);
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.OutOfToleranceGross);
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.OutOfToleranceNet);
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.InvError);
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.ClosedOut);
			this.CombineLineItemFlag(finalLineItem, lineItem, LRBaseInventoryLineItemDO.Status.BrokenBlends);

			this.CombineLineItemCellFlags(finalLineItem, lineItem);
		}

		/// <summary>
		/// This method will reset the rule list to its initial state.
		/// </summary>
		public void ResetRuleList()
		{
			this.ruleList = new Hashtable();
			this.resetRuleList = false;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will combine the line items flags based on the rules.
		/// </summary>
		/// <param name="finalLineItem"></param>
		/// <param name="lineItem"></param>
		/// <param name="status"></param>
		void CombineLineItemFlag(LRInventoryLineItemDO finalLineItem,
								LRInventoryLineItemDO lineItem,
								LRBaseInventoryLineItemDO.Status status)
		{
			this.resetRuleList = true;

			CombineRule rule = (CombineRule)this.ruleList[status];
			bool flagIsSet = lineItem.CheckFlag(status);

			if (rule == CombineRule.NONE)
			{
				finalLineItem.Flags -= status;
			}
			else if (flagIsSet & (rule == CombineRule.Any))
			{
				finalLineItem.Flags += status;
			}
			else if ((flagIsSet == false) & (rule == CombineRule.All))
			{
				finalLineItem.Flags -= status;
			}
		}

		/// <summary>
		/// This method will combine the item cell flags for the final ledger.
		/// </summary>
		/// <param name="finalLineItem"></param>
		/// <param name="lineItem"></param>
		void CombineLineItemCellFlags(LRInventoryLineItemDO finalLineItem, LRInventoryLineItemDO lineItem)
		{
			this.resetRuleList = true;

			this.CombineFlagForSingleCell("Inventory Date", finalLineItem, lineItem);
			this.CombineFlagForSingleCell("Begin Inventory", finalLineItem, lineItem);
			this.CombineFlagForSingleCell("Book Inventory", finalLineItem, lineItem);

			foreach (string cellName in finalLineItem.QuantityList.Keys)
			{
				this.CombineFlagForSingleCell(cellName, finalLineItem, lineItem);
			}
		}

		/// <summary>
		/// This method will combine flags for a single cell on the final ledger line item.
		/// </summary>
		/// <param name="cellName"></param>
		/// <param name="finalLineItem"></param>
		/// <param name="lineItem"></param>
		void CombineFlagForSingleCell(string cellName, LRInventoryLineItemDO finalLineItem, LRInventoryLineItemDO lineItem)
		{
			var flags = (LRBaseInventoryLineItemDO.StatusFlags)lineItem.GetCellFlags()[cellName];

			foreach (LRBaseInventoryLineItemDO.Status status in this.ruleList.Keys)
			{
				var rule = (CombineRule)this.ruleList[status];
				bool flagIsSet = (flags != null) && flags.CheckFlag(status);

				if (rule == CombineRule.NONE)
				{
					finalLineItem.ClearCellFlag(cellName, status);
				}
				else if (flagIsSet & (rule == CombineRule.Any))
				{
					finalLineItem.SetCellFlag(cellName, status);
				}
				else if ((flagIsSet == false) & (rule == CombineRule.All))
				{
					finalLineItem.ClearCellFlag(cellName, status);
				}
			}
		}
		#endregion
	}
}