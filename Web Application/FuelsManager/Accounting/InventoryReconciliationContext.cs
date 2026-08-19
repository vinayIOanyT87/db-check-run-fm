namespace FuelsManager.Accounting
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
    /// Summary description for InventoryReconciliationContext.
    /// </summary>
    [Serializable]
    public class InventoryReconciliationContext
    {
        public string ManagerID;
        public string ProductID;
        public string Month;
        public InventoryReconciliationDO inventoryReconciliationDO;
        public System.Collections.Hashtable AllProductsIRDOCollection;

        public InventoryReconciliationContext()
        {
        }
    }
}
