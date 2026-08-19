// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliases.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliases type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

/// <summary>
/// Wrapper class for definition for TransTypeID
/// </summary>
public class TransactionAliases
{
	#region Enums

	/// <summary>
	/// Definition for TransTypeID
	/// </summary>
	public enum TransactionTypes : short
	{
		T1_PrimaryAdjustment = 1, 

		T2_SecondaryAdjustment, 

		T3_PrimaryDefuel, 

		T4_SecondaryDefuel, 

		T5_PrimaryDisbursement, 

		T6_SecondaryDisbursement, 

		T7_FillStand, 

		T8_Receipt, 

		T9_Request, 

		T10_Unload, 

		T11_ConsumerTransfer, 

		T12_Type12, 

		T13_OwnerTransfer, 

		T14_PhysicalInventory, 

		T15_PrimaryRegrade, 

		T16_SecondaryRegrade, 

		T17_Order, 

		T18_SupplyOrder, 

		T19_EndOfDay, 

		T20_EndOfMonth, 

		T21_AccountPayableInvoice, 

		T22_AccountReceivableInvoice, 

		T23_StorageTransfer, 

		T_Aggregate, 

		T25_Shipment, 

		T_Maximum
	}

	#endregion
}