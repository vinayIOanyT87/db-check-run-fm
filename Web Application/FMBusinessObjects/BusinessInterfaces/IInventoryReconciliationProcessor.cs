// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInventoryReconciliationProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IInventoryReconciliationProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// The InventoryReconciliationProcessor interface.
	/// </summary>
	[ServiceContract]
	public interface IInventoryReconciliationProcessor
	{
		/// <summary>
		/// The process.
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <returns>
		/// The <see cref="InventoryReconciliationDO"/>.
		/// </returns>
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		InventoryReconciliationDO Process ( InventoryReconciliationSR sr, AccountingSite accountingSite = null);
	}
}
