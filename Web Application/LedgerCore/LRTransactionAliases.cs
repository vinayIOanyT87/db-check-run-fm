namespace LedgerCore
{
	public class LRTransactionAliases
	{
		public enum TransactionTypes : short
		{
			T1PrimaryAdjustment = 1,
			T2SecondaryAdjustment,
			T3PrimaryDefuel,
			T4SecondaryDefuel,
			T5PrimaryDisbursement,
			T6SecondaryDisbursement,
			T7FillStand,
			T8Receipt,
			T9Request,
			T10Unload,
			T11ConsumerTransfer,
			T12Type12,
			T13OwnerTransfer,
			T14PhysicalInventory,
			T15PrimaryRegrade,
			T16SecondaryRegrade,
			T17Order,
			T18SupplyOrder,
			T19EndOfDay,
			T20EndOfMonth,
			T21AccountPayableInvoice,
			T22AccountReceivableInvoice,
			T23StorageTransfer,
			T_Aggregate,
			T25Shipment,
			T_Maximum
		}
	}
}