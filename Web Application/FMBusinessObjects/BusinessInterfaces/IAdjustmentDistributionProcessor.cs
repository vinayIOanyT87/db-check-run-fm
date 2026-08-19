// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdjustmentDistributionProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IAdjustmentDistributionProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Interface for the adjustment distribution processor
	/// </summary>
	[ServiceContract]
	public interface IAdjustmentDistributionProcessor
	{
		#region Public Methods and Operators

		/// <summary>
		/// Processes the specified sr.
		/// </summary>
		/// <param name="sr">The sr.</param>
		/// <returns>An adjustment distribution data object.</returns>
		[OperationContract]
		AdjustmentDistributionDO Process(AdjustmentDistributionSR sr);

		#endregion
	}
}