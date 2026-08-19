namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [DataContract]
   [Serializable]
	public class InventoryReconciliationLineItemDO : LedgerLineItemDO
	{
		#region Constructor
		/// <summary>
		/// This is the default constructor for the Inventory Reconcilation Line Item DO.
		/// </summary>
		public InventoryReconciliationLineItemDO ( )
		{
			base.Initialize ( null );
		}

		/// <summary>
		/// This constructor sets the convert engineering units object.
		/// </summary>
		/// <param name="convEngUnits"></param>
		public InventoryReconciliationLineItemDO ( EngineeringUnit? convEngUnits )
		{
			base.Initialize ( convEngUnits );
		}
		#endregion
	}
}
