using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

class LedgerStatusCombiner
{
   #region Public Data members
   public enum CombineRule { ANY, ALL, NONE }
   #endregion

   #region Protected data members
   protected Hashtable ruleList;
   protected bool resetRuleList;
   #endregion

   #region Constructors
   /// <summary>
   /// This is the default constructor for the Ledger Status Combiner class.
   /// </summary>
   public LedgerStatusCombiner()
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
   public void SetCombineRule(InventoryLineItemDO finalLineItem, BaseInventoryLineItemDO.Status status, CombineRule rule)
   {
      this.ruleList.Add(status, rule);

      if (rule == CombineRule.ALL)
      {
         // line item level status
         finalLineItem.Flags += status;

         // cell level status
         finalLineItem.SetCellFlag("Inventory Date",  status);
         finalLineItem.SetCellFlag("Begin Inventory", status);
         finalLineItem.SetCellFlag("Book Inventory",  status);

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
   public void CombineLedgerLineItemStatusFlags(InventoryLineItemDO lineItem, InventoryLineItemDO finalLineItem)
   {
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.NA);
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.PHYS_INV_EXISTS);
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.SUPPRESS_LINK);
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.SUPPRESS);
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.OUT_OF_TOLERANCE_GROSS);
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.OUT_OF_TOLERANCE_NET);
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.INV_ERROR);
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.CLOSED_OUT);
      this.CombineLineItemFlag(finalLineItem, lineItem, BaseInventoryLineItemDO.Status.BROKEN_BLENDS);

      this.CombineLineItemCellFlags(finalLineItem, lineItem);
   }

   /// <summary>
   /// This method will reset the rule list to its initial state.
   /// </summary>
   public void ResetRuleList()
   {
      this.ruleList      = new Hashtable();
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
   void CombineLineItemFlag(InventoryLineItemDO            finalLineItem, 
                            InventoryLineItemDO            lineItem,
                            BaseInventoryLineItemDO.Status status)
   {
      this.resetRuleList = true;

      CombineRule rule = (CombineRule) this.ruleList[status];
      bool flagIsSet   = lineItem.CheckFlag(status);

      if (rule == CombineRule.NONE)
      {
         finalLineItem.Flags -= status;
      }
      else if ((flagIsSet == true) & (rule == CombineRule.ANY))
      {
         finalLineItem.Flags += status;
      }
      else if ((flagIsSet == false) & (rule == CombineRule.ALL))
      {
         finalLineItem.Flags -= status;
      }
   }

   /// <summary>
   /// This method will combine the item cell flags for the final ledger.
   /// </summary>
   /// <param name="finalLineItem"></param>
   /// <param name="lineItem"></param>
   void CombineLineItemCellFlags(InventoryLineItemDO finalLineItem, InventoryLineItemDO lineItem)
   {
      this.resetRuleList = true;

      Hashtable cellFlags = finalLineItem.GetCellFlags();

      this.CombineFlagForSingleCell("Inventory Date",  finalLineItem, lineItem);
      this.CombineFlagForSingleCell("Begin Inventory", finalLineItem, lineItem);
      this.CombineFlagForSingleCell("Book Inventory",  finalLineItem, lineItem);

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
   void CombineFlagForSingleCell(string cellName, InventoryLineItemDO finalLineItem, InventoryLineItemDO lineItem)
   {
      BaseInventoryLineItemDO.StatusFlags flags = (BaseInventoryLineItemDO.StatusFlags) lineItem.GetCellFlags()[cellName];

      foreach (BaseInventoryLineItemDO.Status status in this.ruleList.Keys)
      {
         CombineRule rule = (CombineRule) this.ruleList[status];
         bool flagIsSet   = (flags != null) && flags.CheckFlag(status);

         if (rule == CombineRule.NONE)
         {
            finalLineItem.ClearCellFlag(cellName, status);
         }
         else if ((flagIsSet == true) & (rule == CombineRule.ANY))
         {
            finalLineItem.SetCellFlag(cellName, status);
         }
         else if ((flagIsSet == false) & (rule == CombineRule.ALL))
         {
            finalLineItem.ClearCellFlag(cellName, status);
         }
      }
   }
   #endregion
}
