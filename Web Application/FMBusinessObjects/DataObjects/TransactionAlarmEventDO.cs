using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMBusinessObjects.Interfaces;

namespace FMBusinessObjects.DataObjects
{
	public class TransactionAlarmEventDO : TransactionDO, IAlarmAndEventDiscovery
	{
		#region Public static members
		public static string TransactionT1CreatedKey = "Transaction Type 01 Created";
		public static string TransactionT2CreatedKey = "Transaction Type 02 Created";
		public static string TransactionT3CreatedKey = "Transaction Type 03 Created";
		public static string TransactionT4CreatedKey = "Transaction Type 04 Created";
		public static string TransactionT5CreatedKey = "Transaction Type 05 Created";
		public static string TransactionT6CreatedKey = "Transaction Type 06 Created";
		public static string TransactionT7CreatedKey = "Transaction Type 07 Created";
		public static string TransactionT8CreatedKey = "Transaction Type 08 Created";
		public static string TransactionT9CreatedKey = "Transaction Type 09 Created";
		public static string TransactionT10CreatedKey = "Transaction Type 10 Created";
		public static string TransactionT11CreatedKey = "Transaction Type 11 Created";
		public static string TransactionT12CreatedKey = "Transaction Type 12 Created";
		public static string TransactionT13CreatedKey = "Transaction Type 13 Created";
		public static string TransactionT14CreatedKey = "Transaction Type 14 Created";
		public static string TransactionT15CreatedKey = "Transaction Type 15 Created";
		public static string TransactionT16CreatedKey = "Transaction Type 16 Created";
		public static string TransactionT17CreatedKey = "Transaction Type 17 Created";
		public static string TransactionT18CreatedKey = "Transaction Type 18 Created";
		public static string TransactionT19CreatedKey = "Transaction Type 19 Created";
		public static string TransactionT20CreatedKey = "Transaction Type 20 Created";
		public static string TransactionT21CreatedKey = "Transaction Type 21 Created";
		public static string TransactionT22CreatedKey = "Transaction Type 22 Created";
		public static string TransactionT23CreatedKey = "Transaction Type 23 Created";
		public static string TransactionT25CreatedKey = "Transaction Type 25 Created";

		public static string TransactionT1UpdatedKey = "Transaction Type 01 Updated";
		public static string TransactionT2UpdatedKey = "Transaction Type 02 Updated";
		public static string TransactionT3UpdatedKey = "Transaction Type 03 Updated";
		public static string TransactionT4UpdatedKey = "Transaction Type 04 Updated";
		public static string TransactionT5UpdatedKey = "Transaction Type 05 Updated";
		public static string TransactionT6UpdatedKey = "Transaction Type 06 Updated";
		public static string TransactionT7UpdatedKey = "Transaction Type 07 Updated";
		public static string TransactionT8UpdatedKey = "Transaction Type 08 Updated";
		public static string TransactionT9UpdatedKey = "Transaction Type 09 Updated";
		public static string TransactionT10UpdatedKey = "Transaction Type 10 Updated";
		public static string TransactionT11UpdatedKey = "Transaction Type 11 Updated";
		public static string TransactionT12UpdatedKey = "Transaction Type 12 Updated";
		public static string TransactionT13UpdatedKey = "Transaction Type 13 Updated";
		public static string TransactionT14UpdatedKey = "Transaction Type 14 Updated";
		public static string TransactionT15UpdatedKey = "Transaction Type 15 Updated";
		public static string TransactionT16UpdatedKey = "Transaction Type 16 Updated";
		public static string TransactionT17UpdatedKey = "Transaction Type 17 Updated";
		public static string TransactionT18UpdatedKey = "Transaction Type 18 Updated";
		public static string TransactionT19UpdatedKey = "Transaction Type 19 Updated";
		public static string TransactionT20UpdatedKey = "Transaction Type 20 Updated";
		public static string TransactionT21UpdatedKey = "Transaction Type 21 Updated";
		public static string TransactionT22UpdatedKey = "Transaction Type 22 Updated";
		public static string TransactionT23UpdatedKey = "Transaction Type 23 Updated";
		public static string TransactionT25UpdatedKey = "Transaction Type 25 Updated";

		// vthompson
		public static string TransactionT01StatusChangedKey = "Transaction Type 01 Status Changed";
		public static string TransactionT02StatusChangedKey = "Transaction Type 02 Status Changed";
		public static string TransactionT03StatusChangedKey = "Transaction Type 03 Status Changed";
		public static string TransactionT04StatusChangedKey = "Transaction Type 04 Status Changed";
		public static string TransactionT05StatusChangedKey = "Transaction Type 05 Status Changed";
		public static string TransactionT06StatusChangedKey = "Transaction Type 06 Status Changed";
		public static string TransactionT07StatusChangedKey = "Transaction Type 07 Status Changed";
		public static string TransactionT08StatusChangedKey = "Transaction Type 08 Status Changed";
		public static string TransactionT09StatusChangedKey = "Transaction Type 09 Status Changed";
		public static string TransactionT10StatusChangedKey = "Transaction Type 10 Status Changed";
		public static string TransactionT11StatusChangedKey = "Transaction Type 11 Status Changed";
		public static string TransactionT12StatusChangedKey = "Transaction Type 12 Status Changed";
		public static string TransactionT13StatusChangedKey = "Transaction Type 13 Status Changed";
		public static string TransactionT14StatusChangedKey = "Transaction Type 14 Status Changed";
		public static string TransactionT15StatusChangedKey = "Transaction Type 15 Status Changed";
		public static string TransactionT16StatusChangedKey = "Transaction Type 16 Status Changed";
		public static string TransactionT17StatusChangedKey = "Transaction Type 17 Status Changed";
		public static string TransactionT18StatusChangedKey = "Transaction Type 18 Status Changed";
		public static string TransactionT19StatusChangedKey = "Transaction Type 19 Status Changed";
		public static string TransactionT20StatusChangedKey = "Transaction Type 20 Status Changed";
		public static string TransactionT21StatusChangedKey = "Transaction Type 21 Status Changed";
		public static string TransactionT22StatusChangedKey = "Transaction Type 22 Status Changed";
		public static string TransactionT23StatusChangedKey = "Transaction Type 23 Status Changed";
		public static string TransactionT25StatusChangedKey = "Transaction Type 25 Status Changed";

		public static string AllocatedQuantityExceededKey = "Allocated Quantity Exceeded";
		public static string AllocatedValueExceededKey = "Allocated Value Exceeded";
		public static string QuantityToleranceLevelExceededKey = "Quantity Tolerance Level Exceeded";
		public static string ValueToleranceLevelExceededKey = "Value Tolerance Level Exceeded";

		public static string ReserveLevelAlarmKey = "Reserve Level Notification";

		public static string FMAEInterfaceImportErrorsKey = "FMAE Interface Import Errors";

        public static string AutomaticCloseoutNoPhysicalInventoryErrorsKey = "Automatic Closeout No Physical Inventory";

        private const string CloseoutAllStartKey = "Begin close out all";
        private const string CloseoutAllEndKey = "Finish close out all";
        private const string CloseoutProductStartKey = "Begin close out";
        private const string CloseoutProductEndKey = "Finish close out";
        private const string InventoryReconciliationStartKey = "Start Inventory Reconciliation";
        private const string InventoryReconciliationEndKey = "Finish Inventory Reconciliation";
        private const string GetPreviousCloseoutStartKey = "Start retrieval of last closeout date";
        private const string GetPreviousCloseoutEndKey = "Finish retrieval of last closeout date";
        private const string GetUnpostedBolsStartKey = "Start retrieval of unposted BOLs";
        private const string GetUnpostedBolsEndKey = "Finish retrieval of unposted BOLs";

        private static readonly AlarmAndEventDescriptorClass CloseoutAllStartEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, CloseoutAllStartKey);
        private static readonly AlarmAndEventDescriptorClass CloseoutAllEndEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, CloseoutAllEndKey);
        private static readonly AlarmAndEventDescriptorClass CloseoutProductStartEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, CloseoutProductStartKey);
        private static readonly AlarmAndEventDescriptorClass CloseoutProductEndEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, CloseoutProductEndKey);
        private static readonly AlarmAndEventDescriptorClass InventoryReconciliationStartEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, InventoryReconciliationStartKey);
        private static readonly AlarmAndEventDescriptorClass InventoryReconciliationEndEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, InventoryReconciliationEndKey);
        private static readonly AlarmAndEventDescriptorClass GetPreviousCloseoutStartEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, GetPreviousCloseoutStartKey);
        private static readonly AlarmAndEventDescriptorClass GetPreviousCloseoutEndEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, GetPreviousCloseoutEndKey);
        private static readonly AlarmAndEventDescriptorClass GetUnpostedBolsStartEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, GetUnpostedBolsStartKey);
        private static readonly AlarmAndEventDescriptorClass GetUnpostedBolsEndEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseDataObject.WebApplicationKey, GetUnpostedBolsEndKey);

        public static AlarmAndEventDescriptorClass TransactionT1CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT1CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT2CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT2CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT3CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT3CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT4CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT4CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT5CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT5CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT6CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT6CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT7CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT7CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT8CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT8CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT9CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT9CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT10CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT10CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT11CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT11CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT12CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT12CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT13CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT13CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT14CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT14CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT15CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT15CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT16CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT16CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT17CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT17CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT18CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT18CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT19CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT19CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT20CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT20CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT21CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT21CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT22CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT22CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT23CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT23CreatedKey );
		public static AlarmAndEventDescriptorClass TransactionT25CreationEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT25CreatedKey );

		public static AlarmAndEventDescriptorClass TransactionT1UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT1UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT2UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT2UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT3UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT3UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT4UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT4UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT5UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT5UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT6UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT6UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT7UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT7UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT8UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT8UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT9UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT9UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT10UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT10UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT11UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT11UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT12UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT12UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT13UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT13UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT14UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT14UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT15UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT15UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT16UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT16UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT17UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT17UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT18UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT18UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT19UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT19UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT20UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT20UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT21UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT21UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT22UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT22UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT23UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT23UpdatedKey );
		public static AlarmAndEventDescriptorClass TransactionT25UpdateEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT25UpdatedKey );

		// vthompson - Adding Event descriptors for transaction status changes
		public static AlarmAndEventDescriptorClass TransactionT01StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT01StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT02StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT02StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT03StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT03StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT04StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT04StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT05StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT05StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT06StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT06StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT07StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT07StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT08StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT08StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT09StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT09StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT10StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT10StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT11StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT11StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT12StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT12StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT13StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT13StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT14StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT14StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT15StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT15StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT16StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT16StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT17StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT17StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT18StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT18StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT19StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT19StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT20StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT20StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT21StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT21StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT22StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT22StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT23StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT23StatusChangedKey );
		public static AlarmAndEventDescriptorClass TransactionT25StatusChangedEventDescriptor = new AlarmAndEventDescriptorClass ( false, BaseDataObject.TransactionKey, TransactionT25StatusChangedKey );

		public static AlarmAndEventDescriptorClass AllocatedQuantityExceededEventDescriptor =
			new AlarmAndEventDescriptorClass(false, BaseDataObject.TransactionKey, AllocatedQuantityExceededKey);
		public static AlarmAndEventDescriptorClass AllocatedValueExceededEventDescriptor =
			new AlarmAndEventDescriptorClass(false, BaseDataObject.TransactionKey, AllocatedValueExceededKey);
		public static AlarmAndEventDescriptorClass QtyToleranceLevelExceededEventDescriptor =
			new AlarmAndEventDescriptorClass(false, BaseDataObject.TransactionKey, QuantityToleranceLevelExceededKey);
		public static AlarmAndEventDescriptorClass ValueToleranceLevelExceededEventDescriptor =
			new AlarmAndEventDescriptorClass(false, BaseDataObject.TransactionKey, ValueToleranceLevelExceededKey);

		public static AlarmAndEventDescriptorClass ReserveLevelAlarmEventDescriptor =
			new AlarmAndEventDescriptorClass(true, BaseDataObject.TransactionKey, ReserveLevelAlarmKey);

		public static AlarmAndEventDescriptorClass FMAEInterfaceImportErrorEventDescriptor =
			new AlarmAndEventDescriptorClass(false, BaseDataObject.TransactionKey, FMAEInterfaceImportErrorsKey);

        public static AlarmAndEventDescriptorClass AutomaticCloseoutNoPhysicalInventoryEventDescriptor =
            new AlarmAndEventDescriptorClass(false, BaseDataObject.TransactionKey, AutomaticCloseoutNoPhysicalInventoryErrorsKey);

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction alarm and event data
		/// object class.
		/// </summary>
		public TransactionAlarmEventDO ( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property exposes the base class alias name data member.
		/// </summary>
		public string AliasName
		{
			get { return base.aliasName; }
			set { base.aliasName = value; }
		}

		/// <summary>
		/// This property exposes the base class transaction type ID data member.
		/// </summary>
		new public TransactionTypes TransTypeID
		{
			get { return base.TransTypeID; }
			set { base.TransTypeID = value; }
		}

		/// <summary>
		/// This property exposes the base class transaction DocumentNumber data member.
		/// </summary>
		new public string DocumentNumber
		{
			get { return base.documentNumber; }
			set { base.documentNumber = value; }
		}

        #endregion

        #region Alarm and Event Descriptors
        /// <summary>
        /// This property return an array of alarm and event descriptors for handling
        /// transaction events. It is called by the discovery routine.
        /// </summary>
        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] Descriptors = {TransactionT1CreationEventDescriptor,
															 TransactionT2CreationEventDescriptor,
															 TransactionT3CreationEventDescriptor,
															 TransactionT4CreationEventDescriptor,
															 TransactionT5CreationEventDescriptor,
															 TransactionT6CreationEventDescriptor,
															 TransactionT7CreationEventDescriptor,
															 TransactionT8CreationEventDescriptor,
															 TransactionT9CreationEventDescriptor,
															 TransactionT10CreationEventDescriptor,
															 TransactionT11CreationEventDescriptor,
															 TransactionT12CreationEventDescriptor,
															 TransactionT13CreationEventDescriptor,
															 TransactionT14CreationEventDescriptor,
															 TransactionT15CreationEventDescriptor,
															 TransactionT16CreationEventDescriptor,
															 TransactionT17CreationEventDescriptor,
															 TransactionT18CreationEventDescriptor,
															 TransactionT19CreationEventDescriptor,
															 TransactionT20CreationEventDescriptor,
															 TransactionT21CreationEventDescriptor,
															 TransactionT22CreationEventDescriptor,
															 TransactionT23CreationEventDescriptor,
															 TransactionT1UpdateEventDescriptor,
															 TransactionT2UpdateEventDescriptor,
															 TransactionT3UpdateEventDescriptor,
															 TransactionT4UpdateEventDescriptor,
															 TransactionT5UpdateEventDescriptor,
															 TransactionT6UpdateEventDescriptor,
															 TransactionT7UpdateEventDescriptor,
															 TransactionT8UpdateEventDescriptor,
															 TransactionT9UpdateEventDescriptor,
															 TransactionT10UpdateEventDescriptor,
															 TransactionT11UpdateEventDescriptor,
															 TransactionT12UpdateEventDescriptor,
															 TransactionT13UpdateEventDescriptor,
															 TransactionT14UpdateEventDescriptor,
															 TransactionT15UpdateEventDescriptor,
															 TransactionT16UpdateEventDescriptor,
															 TransactionT17UpdateEventDescriptor,
															 TransactionT18UpdateEventDescriptor,
															 TransactionT19UpdateEventDescriptor,
															 TransactionT20UpdateEventDescriptor,
															 TransactionT21UpdateEventDescriptor,
															 TransactionT22UpdateEventDescriptor,
															 TransactionT23UpdateEventDescriptor,
															 TransactionT01StatusChangedEventDescriptor,	// vthompson - Adding Status Changed event descriptors
															 TransactionT02StatusChangedEventDescriptor,
															 TransactionT03StatusChangedEventDescriptor,
															 TransactionT04StatusChangedEventDescriptor,
															 TransactionT05StatusChangedEventDescriptor,
															 TransactionT06StatusChangedEventDescriptor,
															 TransactionT07StatusChangedEventDescriptor,
															 TransactionT08StatusChangedEventDescriptor,
															 TransactionT09StatusChangedEventDescriptor,
															 TransactionT10StatusChangedEventDescriptor,
															 TransactionT11StatusChangedEventDescriptor,
															 TransactionT12StatusChangedEventDescriptor,
															 TransactionT13StatusChangedEventDescriptor,
															 TransactionT14StatusChangedEventDescriptor,
															 TransactionT15StatusChangedEventDescriptor,
															 TransactionT16StatusChangedEventDescriptor,
															 TransactionT17StatusChangedEventDescriptor,
															 TransactionT18StatusChangedEventDescriptor,
															 TransactionT19StatusChangedEventDescriptor,
															 TransactionT20StatusChangedEventDescriptor,
															 TransactionT21StatusChangedEventDescriptor,
															 TransactionT22StatusChangedEventDescriptor,
															 TransactionT23StatusChangedEventDescriptor,
															 AllocatedQuantityExceededEventDescriptor,
															 AllocatedValueExceededEventDescriptor,
															 QtyToleranceLevelExceededEventDescriptor,
															 ValueToleranceLevelExceededEventDescriptor,
															 ReserveLevelAlarmEventDescriptor,
															 FMAEInterfaceImportErrorEventDescriptor,
                                                             AutomaticCloseoutNoPhysicalInventoryEventDescriptor,

                                                             CloseoutAllStartEventDescriptor,
                                                             CloseoutAllEndEventDescriptor,
                                                             CloseoutProductStartEventDescriptor,
                                                             CloseoutProductEndEventDescriptor,
                                                             InventoryReconciliationStartEventDescriptor,
                                                             InventoryReconciliationEndEventDescriptor,
                                                             GetPreviousCloseoutStartEventDescriptor,
                                                             GetPreviousCloseoutEndEventDescriptor,
                                                             GetUnpostedBolsStartEventDescriptor,
                                                             GetUnpostedBolsEndEventDescriptor
                                                         };
				return Descriptors;
			}
		}
     
        /// <summary>
        /// This property will return the an event log for transaction creation.
        /// </summary>
        public AlarmAndEventLogClass TransactionT1CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT1CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT2CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT2CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT3CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT3CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT4CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT4CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT5CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT5CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT6CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT6CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT7CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT7CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT8CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT8CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT9CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT9CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT10CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT10CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT11CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT11CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT12CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT12CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT13CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT13CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT14CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT14CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT15CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT15CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT16CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT16CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT17CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT17CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT18CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT18CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT19CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT19CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT20CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT20CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT21CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT21CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT22CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT22CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT23CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT23CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT25CreateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT25CreationEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		/// <summary>
		/// This property will return the an event log for transaction creation.
		/// </summary>
		public AlarmAndEventLogClass TransactionT1UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT1UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT2UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT2UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT3UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT3UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT4UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT4UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT5UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT5UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT6UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT6UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT7UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT7UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT8UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT8UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT9UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT9UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT10UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT10UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT11UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT11UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT12UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT12UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT13UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT13UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT14UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT14UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT15UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT15UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT16UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT16UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT17UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT17UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT18UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT18UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT19UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT19UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT20UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT20UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT21UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT21UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT22UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT22UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT23UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT23UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}
		public AlarmAndEventLogClass TransactionT25UpdateEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT25UpdateEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		// vthompson - Adding Transaction Status Changed events
		public AlarmAndEventLogClass TransactionT01StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT01StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT02StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT02StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT03StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT03StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT04StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT04StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT05StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT05StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT06StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT06StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT07StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT07StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT08StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT08StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT09StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT09StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT10StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT10StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT11StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT11StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT12StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT12StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT13StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT13StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT14StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT14StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT15StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT15StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT16StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT16StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT17StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT17StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT18StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT18StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT19StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT19StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT20StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT20StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT21StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT21StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT22StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT22StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT23StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT23StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransactionT25StatusChangedEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass ( TransactionT25StatusChangedEventDescriptor );
				alarmAndEventLog.AssociatedData = base.aliasName + this.GetDateString ( ) + " " + base.documentNumber;
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass AllocatedQuantityExceededEvent
		{
			get
			{
				AlarmAndEventLogClass eventLog = new AlarmAndEventLogClass (
					AllocatedQuantityExceededEventDescriptor );
				eventLog.AssociatedData = base.aliasName + " " + this.GetDateString ( ) +
					" " + base.documentNumber + " " + base.transTypeID.ToString ( );
				return eventLog;
			}
		}

		public AlarmAndEventLogClass AllocatedValueExceededEvent
		{
			get
			{
				AlarmAndEventLogClass eventLog = new AlarmAndEventLogClass (
					AllocatedValueExceededEventDescriptor );
				eventLog.AssociatedData = base.aliasName + " " + this.GetDateString ( ) +
					" " + base.documentNumber + " " + base.transTypeID.ToString ( );
				return eventLog;
			}
		}

		public AlarmAndEventLogClass QuantityToleranceLevelExceededEvent
		{
			get
			{
				AlarmAndEventLogClass eventLog = new AlarmAndEventLogClass (
					QtyToleranceLevelExceededEventDescriptor );
				eventLog.AssociatedData = base.aliasName + " " + this.GetDateString ( ) +
					" " + base.documentNumber + " " + base.transTypeID.ToString ( );
				return eventLog;
			}
		}

		public AlarmAndEventLogClass ValueToleranceLevelExceededEvent
		{
			get
			{
				AlarmAndEventLogClass eventLog = new AlarmAndEventLogClass (
					ValueToleranceLevelExceededEventDescriptor );
				eventLog.AssociatedData = base.aliasName + " " + this.GetDateString ( ) +
					" " + base.documentNumber + " " + base.transTypeID.ToString ( );
				return eventLog;
			}
		}

		public AlarmAndEventLogClass ReserveLevelAlarmEvent
		{
			get
			{
				AlarmAndEventLogClass eventLog = new AlarmAndEventLogClass (
					ReserveLevelAlarmEventDescriptor );
				eventLog.AssociatedData = base.aliasName + " " + this.GetDateString ( ) +
					" " + base.documentNumber + " " + base.transTypeID.ToString ( );
				return eventLog;
			}
		}

        public static AlarmAndEventLogClass CloseoutAllStartEvent
        {
            get
            {
                var eventLog = new AlarmAndEventLogClass(
                    CloseoutAllStartEventDescriptor)
                {
                    AssociatedData =
                        "Close out has started"
                };
                return eventLog;
            }
        }

        public static AlarmAndEventLogClass CloseoutAllEndEvent
        {
            get
            {
                var eventLog = new AlarmAndEventLogClass(
                    CloseoutAllEndEventDescriptor)
                {
                    AssociatedData =
                        "Close out has completed"
                };
                return eventLog;
            }
        }

        public static AlarmAndEventLogClass CloseoutProductStartEvent(string product)
        {
            var eventLog = new AlarmAndEventLogClass(CloseoutProductStartEventDescriptor)
            {
                AssociatedData = "Product: " + product
            };
            return eventLog;
        }

        public static AlarmAndEventLogClass CloseoutProductEndEvent(string product)
        {
            var eventLog = new AlarmAndEventLogClass(CloseoutProductEndEventDescriptor)
            {
                AssociatedData = "Product: " + product
            };
            return eventLog;
        }

        public static AlarmAndEventLogClass InventoryReconciliationStartEvent(string product)
        {
            var eventLog = new AlarmAndEventLogClass(InventoryReconciliationStartEventDescriptor)
            {
                AssociatedData = "Product: " + product
            };
            return eventLog;
        }

        public static AlarmAndEventLogClass InventoryReconciliationEndEvent(string product)
        {
            var eventLog = new AlarmAndEventLogClass(InventoryReconciliationEndEventDescriptor)
            {
                AssociatedData = "Product: " + product
            };
            return eventLog;
        }

        public static AlarmAndEventLogClass GetPreviousCloseoutStartEvent(string product)
        {
            var eventLog = new AlarmAndEventLogClass(GetPreviousCloseoutStartEventDescriptor)
            {
                AssociatedData = "Product: " + product
            };
            return eventLog;
        }

        public static AlarmAndEventLogClass GetPreviousCloseoutEndEvent(string product)
        {
            var eventLog = new AlarmAndEventLogClass(GetPreviousCloseoutEndEventDescriptor)
            {
                AssociatedData = "Product: " + product
            };
            return eventLog;
        }

        public static AlarmAndEventLogClass GetUnpostedBolsStartEvent(string product)
        {
            var eventLog = new AlarmAndEventLogClass(GetUnpostedBolsStartEventDescriptor)
            {
                AssociatedData = "Product: " + product
            };
            return eventLog;
        }

        public static AlarmAndEventLogClass GetUnpostedBolsEndEvent(string product)
        {
            var eventLog = new AlarmAndEventLogClass(GetUnpostedBolsEndEventDescriptor)
            {
                AssociatedData = "Product: " + product
            };
            return eventLog;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will return the date string in the following format "yyyy-mm-dd".
        /// </summary>
        /// <returns></returns>
        private string GetDateString ( )
		{
			string dateStr = base.InventoryDate.Year.ToString ( ) + "-" + base.InventoryDate.Month.ToString ( ) + "-" +
				base.InventoryDate.Day.ToString ( );

			return " " + dateStr;
		}
		#endregion
	}
}
