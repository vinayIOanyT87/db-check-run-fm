using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	public class EquipmentMeterMap
	{
		public enum MeterEquipment { None, Source, Destination };

		public static MeterEquipment GetMeterEquipment ( TransactionTypes transType )
		{
			switch (transType)
			{
				case TransactionTypes.T3_PrimaryDefuel:
				case TransactionTypes.T4_SecondaryDefuel:
					return MeterEquipment.Destination;

				case TransactionTypes.T5_PrimaryDisbursement:
				case TransactionTypes.T6_SecondaryDisbursement:
				case TransactionTypes.T7_FillStand:
				case TransactionTypes.T10_Unload:
				case TransactionTypes.T12_InventoryNotAffected:
				case TransactionTypes.T11_ConsumerTransfer:
				case TransactionTypes.T13_OwnerTransfer:
				case TransactionTypes.T25_Shipment:
					return MeterEquipment.Source;

				case TransactionTypes.T1_PrimaryAdjustment:
				case TransactionTypes.T2_SecondaryAdjustment:
				case TransactionTypes.T9_Request:
				case TransactionTypes.T8_Receipt:
				case TransactionTypes.T14_PhysicalInventory:
				case TransactionTypes.T15_PrimaryRegrade:
				case TransactionTypes.T16_SecondaryRegrade:
					return MeterEquipment.None;
			}
			return MeterEquipment.None;
		}

		public static string GetFuelingEQ ( TransactionDO trans, string sourceEQ1, string sourceEQ2,
			string sourceEQ3, string destinationEQ1, string destinationEQ2, string destinationEQ3 )
		{
			if (EquipmentMeterMap.GetMeterEquipment ( trans.TransTypeID ) == EquipmentMeterMap.MeterEquipment.Destination)
			{
				if (( destinationEQ3 != null ) && ( destinationEQ3.Length > 0 )) return destinationEQ3;
				if (( destinationEQ2 != null ) && ( destinationEQ2.Length > 0 )) return destinationEQ2;
				if (( destinationEQ1 != null ) && ( destinationEQ1.Length > 0 )) return destinationEQ1;
			}
			else if (EquipmentMeterMap.GetMeterEquipment ( trans.TransTypeID ) == EquipmentMeterMap.MeterEquipment.Source)
			{
				if (( sourceEQ3 != null ) && ( sourceEQ3.Length > 0 )) return sourceEQ3;
				if (( sourceEQ2 != null ) && ( sourceEQ2.Length > 0 )) return sourceEQ2;
				if (( sourceEQ1 != null ) && ( sourceEQ1.Length > 0 )) return sourceEQ1;
			}
			return null;
		}

		public static string GetConsumerEQ ( TransactionDO trans, string sourceEQ1, string sourceEQ2,
			string sourceEQ3, string destinationEQ1, string destinationEQ2, string destinationEQ3 )
		{
			if (EquipmentMeterMap.GetMeterEquipment ( trans.TransTypeID ) == EquipmentMeterMap.MeterEquipment.Source)
			{
				if (( destinationEQ3 != null ) && ( destinationEQ3.Length > 0 )) return destinationEQ3;
				if (( destinationEQ2 != null ) && ( destinationEQ2.Length > 0 )) return destinationEQ2;
				if (( destinationEQ1 != null ) && ( destinationEQ1.Length > 0 )) return destinationEQ1;
			}
			else if (EquipmentMeterMap.GetMeterEquipment ( trans.TransTypeID ) == EquipmentMeterMap.MeterEquipment.Destination)
			{
				if (( sourceEQ3 != null ) && ( sourceEQ3.Length > 0 )) return sourceEQ3;
				if (( sourceEQ2 != null ) && ( sourceEQ2.Length > 0 )) return sourceEQ2;
				if (( sourceEQ1 != null ) && ( sourceEQ1.Length > 0 )) return sourceEQ1;
			}
			return null;
		}
	}
}
